using API.IKModernWebApp.Helpers;
using API.IKModernWebApp.ViewModel;
using HtmlAgilityPack;
using OfficeOpenXml;
using ScrapySharp.Html;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.IKModernWebApp.Workers
{
    public class EmployerSystemOperationWorkers
    {
        List<UnsuccessCompanyProcessViewModel> _unsuccessCompanyProcesses = new List<UnsuccessCompanyProcessViewModel>();
        public bool EmployeeApplyPormotionScrapper(WebPage webPage, CompanyViewModel companyView)
        {
            ParserWorkers worker = new ParserWorkers();
            WebPage pg = GetIFramePage(webPage, null, "menuForm:anaMenu1subMenu2subItem2");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pg.Content);
            HtmlNode select = doc.DocumentNode.SelectSingleNode("//select[@name='donem_yil_ay_index']");

            List<KeyValuePair<int, string>> periodList = worker.SelectToPeriodParserWorker(select);
            foreach (KeyValuePair<int, string> period in periodList)
            {
                PageWebForm promotionQueryForm = pg.FindFormById("tesvikTanimlama");
                promotionQueryForm["donem_yil_ay_index"] = period.Key.ToString();
                WebPage personalListPage = promotionQueryForm.Submit();
                List<EmployeeViewModel> employeeList = worker.TableToClassParserWorker(personalListPage);
            }
            return true;
        }

        public bool EmployeePromotionDownloadScrapper(WebPage webPage, CompanyViewModel companyView)
        {
            PromotionLawFilesPageScraper(webPage, companyView, "menuForm:anaMenu1subMenu2subItem4",
                $"Rapor_4447_10_{companyView.GroupId}_{companyView.CompanyId}_{IOHelper.WhiteListIllegalChars(companyView.CompanyName)}");
            PromotionLawFilesPageScraper(webPage, companyView, "menuForm:anaMenu1subMenu2subItem5",
                $"Rapor_4447_15_{companyView.GroupId}_{companyView.CompanyId}_{IOHelper.WhiteListIllegalChars(companyView.CompanyName)}");
            PromotionLawFilesPageScraper(webPage, companyView, "menuForm:anaMenu1subMenu2subItem6",
                $"Rapor_4447_17_{companyView.GroupId}_{companyView.CompanyId}_{IOHelper.WhiteListIllegalChars(companyView.CompanyName)}");
            PromotionLawFilesPageScraper(webPage, companyView, "menuForm:anaMenu1subMenu2subItem7",
                $"Rapor_4447_19_{companyView.GroupId}_{companyView.CompanyId}_{IOHelper.WhiteListIllegalChars(companyView.CompanyName)}");
            if (_unsuccessCompanyProcesses.Count > 0)
            {
                CreateUnsuccessCompanyProcessExcel(companyView);
            }

            return true;
        }

        public bool PromotionLawFilesPageScraper(WebPage webPage, CompanyViewModel companyView, string menuId, string fileName)
        {
            webPage = CheckSessionForPage(webPage, companyView, menuId);
            while (true)
            {
                Thread.Sleep(3000);
                try
                {
                    WebPage pg = GetIFramePage(webPage, companyView, menuId);
                    if (pg.Content.Contains("LİSTELENECEK VERİ BULUNAMAMIŞTIR."))
                    {
                        UnsuccessCompanyProcessViewModel unsuccessCompanyProcess = new UnsuccessCompanyProcessViewModel()
                        {
                            GroupName = companyView.GroupName,
                            CompanyName = companyView.CompanyName,
                            RegistryNumber = companyView.RegistryNumber,
                            ConnectedSystem = $"İşveren Sistemi / Kanun_{fileName.Split('_')[1]}_{fileName.Split('_')[2]}",
                            FailReason = "Listelenecek Veri Bulunamamıştır"
                        };

                        _unsuccessCompanyProcesses.Add(unsuccessCompanyProcess);
                        return false;
                    }
                    else
                    {
                        string filePath = IOHelper.CreateFilePathWithType(fileName, "xlsx", $@"\{companyView.GroupId}\{companyView.CompanyId}");

                        PageWebForm excelForm = pg.FindFormById("excelForm");
                        WebPage fileResult = excelForm.Submit();
                        IOHelper.WriteFileFromByte(fileResult.RawResponse.Body, filePath);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("500"))
                    {
                        Thread.Sleep(3000);
                        continue;
                    }
                }
            }
            return true;
        }

        public bool MinimumWageSupportPageScrapper(WebPage webPage, CompanyViewModel companyView)
        {
            ParserWorkers worker = new ParserWorkers();
            webPage = CheckSessionForPage(webPage, companyView, "menuForm:anaMenu1subItem0");
            webPage = GetIFramePage(webPage, companyView, "menuForm:anaMenu1subItem0");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(webPage.Content);
            HtmlNode select = doc.DocumentNode.SelectSingleNode("//select[@name='sec']");

            List<KeyValuePair<int, string>> periodList = worker.SelectToPeriodParserWorker(select, false);
            foreach (KeyValuePair<int, string> period in periodList)
            {
                do
                {
                    webPage = CheckSessionForPage(webPage, companyView, "menuForm:anaMenu1subItem0");
                    webPage = GetIFramePage(webPage, companyView, "menuForm:anaMenu1subItem0");

                    PageWebForm pageWebForm = webPage.FindFormById("secimBelirle");
                    pageWebForm["secim"] = "3";
                    pageWebForm["sec"] = period.Key.ToString();
                    pageWebForm["fisTarih"] = DateTime.Now.ToString("dd.MM.yyyy");
                    pageWebForm.FormFields.RemoveAt(pageWebForm.FormFields.FindIndex(x => x.Name == "action:mufredatKartiPdf"));
                    webPage = pageWebForm.Submit();
                } while (webPage.Content.Contains("Uzun süredir"));

                if (!webPage.Content.Contains("Kayıt Bulunamamıştır."))
                {
                    HtmlDocument wageDoc = new HtmlDocument();
                    wageDoc.LoadHtml(webPage.Content);
                    HtmlNodeCollection minimumWageResults = wageDoc.DocumentNode.SelectNodes("//table[@class='gradienttable']//tr");
                    List<MinimumWageSupportModelView> minimumWages = new List<MinimumWageSupportModelView>();
                    foreach (HtmlNode minWageResult in minimumWageResults)
                    {
                        HtmlDocument resultDoc = new HtmlDocument();
                        resultDoc.LoadHtml(minWageResult.InnerHtml);
                        if (minWageResult.InnerText.Contains("hesaplaması yapılmayacaktır"))
                        {
                            UnsuccessCompanyProcessViewModel unsuccessCompanyProcess = new UnsuccessCompanyProcessViewModel()
                            {
                                CompanyId = companyView.CompanyId,
                                CompanyName = companyView.CompanyName,
                                RegistryNumber = companyView.RegistryNumber,
                                GroupId = companyView.GroupId,
                                GroupName = companyView.GroupName,
                                ConnectedSystem = "İş Veren Sistemi: Asgari Ücret Destek Tutar",
                                FailReason = minWageResult.InnerText.Trim()
                            };

                            _unsuccessCompanyProcesses.Add(unsuccessCompanyProcess);
                            continue;
                        }
                        else if (minWageResult.InnerText.Contains("Sıra"))
                        {
                            continue;
                        }

                        MinimumWageSupportModelView minimumWage = new MinimumWageSupportModelView()
                        {
                            CompanyId = companyView.CompanyId,
                            CompanyName = companyView.CompanyName,
                            RegistryNumber = companyView.RegistryNumber,
                            GroupId = companyView.GroupId,
                            GroupName = companyView.GroupName,
                            ApplyYear = resultDoc.DocumentNode.SelectSingleNode("//td[2]").InnerText,
                            ApplyMonth = resultDoc.DocumentNode.SelectSingleNode("//td[3]").InnerText,
                            NumberOfDaysUsed = resultDoc.DocumentNode.SelectSingleNode("//td[4]").InnerText,
                            SupportAmount = Convert.ToDecimal(resultDoc.DocumentNode.SelectSingleNode("//td[5]").InnerText.Replace("TL", "").Trim(), new CultureInfo("en-US")),
                            CollectionDate = Convert.ToDateTime(resultDoc.DocumentNode.SelectSingleNode("//td[6]").InnerText.Trim()).ToString("dd.MM.yyyy")
                        };

                        minimumWages.Add(minimumWage);
                    }

                    string jsonFileName = $"Asgari_Ucret_Destegi_Donem_{period.Value}";
                    string jsonPath = IOHelper.CreateFilePathWithType(jsonFileName, "json", $@"\{companyView.GroupId}\{companyView.CompanyId}");
                    IOHelper.CreateJsonFileFromResults(minimumWages, jsonPath);

                    PageWebForm pageWebFormPdf = webPage.FindFormById("asgariDestekHazirlaPdfOlustur");
                    pageWebFormPdf["jsp"] = webPage.Content;
                    WebPage pdfPage = pageWebFormPdf.Submit();

                    string fileName = $"Asgari_Ucret_Destegi_{IOHelper.WhiteListIllegalChars(companyView.GroupName).Trim()}_{IOHelper.WhiteListIllegalChars(companyView.CompanyName).Trim()}_Donem_{period.Value}";
                    string path = IOHelper.CreateFilePathWithType(fileName, "pdf", $@"\{companyView.GroupId}\{companyView.CompanyId}");
                    IOHelper.WriteFileFromByte(pdfPage.RawResponse.Body, path);

                }
            }

            CreateUnsuccessCompanyProcessExcel(companyView);
            return true;
        }

        public bool IllegalPromotionSupportControlPageScraper(WebPage webPage, CompanyViewModel companyView)
        {
            List<PassedMonthIllegalPromotionSupportViewModel> passedMonthIllegalPromotions = new List<PassedMonthIllegalPromotionSupportViewModel>();
            do
            {
                webPage = CheckSessionForPage(webPage, companyView, "menuForm:anaMenu1subMenu2subItem12");
            } while (webPage.Content.Contains("Uzun süredir"));
            webPage = GetIFramePage(webPage, null, "menuForm:anaMenu1subMenu2subItem12");

            PageWebForm currentMonthIllegalQuery = webPage.FindFormById("caridonemTesvikSorgulama_duzeltme");
            webPage = currentMonthIllegalQuery.Submit();
            if (webPage.Content.Contains("Talebiniz alınmıştır"))
            {
                do
                {
                    webPage = CheckSessionForPage(webPage, null, "menuForm:anaMenu1subMenu2subItem11");
                } while (webPage.Content.Contains("Uzun süredir"));
                webPage = GetIFramePage(webPage, null, "menuForm:anaMenu1subMenu2subItem11");

                if (webPage.Content.Contains("kayıt bulunamadı."))
                {

                }
                else
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(webPage.Content);

                    HtmlNodeCollection illegalSupportList = doc.DocumentNode.SelectNodes("//table[contains(@class,'paginated gradienttable')]//tr");
                    illegalSupportList.RemoveAt(0);
                    illegalSupportList.RemoveAt(0);

                    foreach (HtmlNode illegal in illegalSupportList)
                    {
                        HtmlDocument illegalDoc = new HtmlDocument();
                        illegalDoc.LoadHtml(illegal.InnerHtml);

                        PassedMonthIllegalPromotionSupportViewModel passedMonthIllegal = new PassedMonthIllegalPromotionSupportViewModel()
                        {
                            CompanyId = companyView.CompanyId,
                            CompanyName = companyView.CompanyName,
                            RegistryNumber = companyView.RegistryNumber,
                            GroupId = companyView.GroupId,
                            GroupName = companyView.GroupName,
                            IllegalPromotionYear = illegalDoc.DocumentNode.SelectSingleNode("//td[2]")?.InnerText.Trim(),
                            IllegalPromotionMonth = illegalDoc.DocumentNode.SelectSingleNode("//td[3]")?.InnerText.Trim(),
                            SgkCity = illegalDoc.DocumentNode.SelectSingleNode("//td[4]")?.InnerText.Trim(),
                            OldBranch = illegalDoc.DocumentNode.SelectSingleNode("//td[5]")?.InnerText.Trim(),
                            NewBranch = illegalDoc.DocumentNode.SelectSingleNode("//td[6]")?.InnerText.Trim(),
                            QueNumber = illegalDoc.DocumentNode.SelectSingleNode("//td[7]")?.InnerText.Trim(),
                            Mediator = illegalDoc.DocumentNode.SelectSingleNode("//td[8]")?.InnerText.Trim(),
                            PromotionLaw = illegalDoc.DocumentNode.SelectSingleNode("//td[9]")?.InnerText.Trim(),
                            NID = illegalDoc.DocumentNode.SelectSingleNode("//td[10]")?.InnerText.Trim(),
                            IllegalSupportReason = illegalDoc.DocumentNode.SelectSingleNode("//td[11]")?.InnerText.Trim(),
                            Status = illegalDoc.DocumentNode.SelectSingleNode("//td[12]")?.InnerText.Trim(),
                            CaseOpenDate = illegalDoc.DocumentNode.SelectSingleNode("//td[13]")?.InnerText.Trim(),
                            CaseOpenTime = illegalDoc.DocumentNode.SelectSingleNode("//td[14]")?.InnerText.Trim(),
                            SubmitWaitDocument = illegalDoc.DocumentNode.SelectSingleNode("//td[15]")?.InnerText.Trim()
                        };
                        passedMonthIllegalPromotions.Add(passedMonthIllegal);
                    }
                }
                if (passedMonthIllegalPromotions.Count > 0)
                {
                    string jsonFileName = $"Geçmiş_Donem_Yersiz_Tesvik_Sonuc";
                    string jsonPath = IOHelper.CreateFilePathWithType(jsonFileName, "json", $@"\{companyView.GroupId}\{companyView.CompanyId}");
                    IOHelper.CreateJsonFileFromResults(passedMonthIllegalPromotions, jsonPath);
                }

                Thread.Sleep(3000);
                do
                {
                    webPage = CheckSessionForPage(webPage, companyView, "menuForm:anaMenu1subMenu2subItem12");
                } while (webPage.Content.Contains("Uzun süredir"));
                webPage = GetIFramePage(webPage, null, "menuForm:anaMenu1subMenu2subItem12");
            }
            else if (webPage.Content.Contains("İsteğiniz işleme alınamamıştır"))
            {

            }

            return true;
        }

        public WebPage CheckSessionForPage(WebPage webPage, CompanyViewModel company, string menuId)
        {
            while (!webPage.Content.Contains(menuId))
            {
                webPage = webPage.Browser.NavigateToPage(new Uri("https://uyg.sgk.gov.tr/IsverenSistemi/pages/baslangic.jsf"));
                if (!webPage.Content.Contains(menuId))
                {
                    SgkLoginHelper loginHelper = new SgkLoginHelper();
                    webPage = loginHelper.SgkEmployerSystemLogin(company);
                }
            }
            return webPage;
        }

        public WebPage GetIFramePage(WebPage webPage, CompanyViewModel companyView, string menuId)
        {
            WebPage pg = null;
            bool isSession = false;
            while (!isSession)
            {
                webPage = webPage.FindLinks(By.Id(menuId)).Single().Click();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(webPage.Content);
                HtmlNode tokenDoc = doc.DocumentNode.SelectSingleNode("//iframe[@id='pencereLinkIdYeni']/@src");
                string link = tokenDoc.GetAttributeValue("src", "");
                pg = webPage.Browser.NavigateToPage(new Uri(link));

                if (pg.Content.Contains("Uzun süredir"))
                {
                    SgkLoginHelper loginHelper = new SgkLoginHelper();
                    webPage = loginHelper.SgkEmployerSystemLogin(companyView);
                    webPage = CheckSessionForPage(webPage, companyView, menuId);
                    webPage = webPage.FindLinks(By.Id(menuId)).Single().Click();
                }
                else
                {
                    isSession = true;
                }
            }

            return pg;
        }

        public void CreateUnsuccessCompanyProcessExcel(CompanyViewModel companyView)
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(Path.Combine(Environment.CurrentDirectory, "Templates"), "UnsuccessCompanyProcess.xlsx"));

            string fileName = $"Firma_Sonuc_{companyView.GroupId}_{companyView.CompanyId}";
            string path = IOHelper.CreateFilePathWithType(fileName, "xlsx", $@"\{companyView.GroupId}\{companyView.CompanyId}");
            FileInfo resultFileInfo = new FileInfo(path);

            if (!File.Exists(path))
            {
                using (ExcelPackage excelPackageCopy = new ExcelPackage(fileInfo))
                {
                    excelPackageCopy.File.CopyTo(path);
                }
            }

            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage(resultFileInfo))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;

                ExcelWorksheet excelWorksheet1 = excelWorkBook.Worksheets.First();
                int lastRow1 = excelWorksheet1.Dimension.End.Row;
                int nextRow1 = (lastRow1 + 1);

                if (_unsuccessCompanyProcesses != null && _unsuccessCompanyProcesses?.Count > 0)
                {
                    foreach (UnsuccessCompanyProcessViewModel unsuccessCompanyProcess in _unsuccessCompanyProcesses)
                    {
                        excelWorksheet1.Cells[nextRow1, 1].Value = unsuccessCompanyProcess.GroupName;
                        excelWorksheet1.Cells[nextRow1, 2].Value = unsuccessCompanyProcess.CompanyName;
                        excelWorksheet1.Cells[nextRow1, 3].Value = unsuccessCompanyProcess.ConnectedSystem;
                        excelWorksheet1.Cells[nextRow1, 4].Value = unsuccessCompanyProcess.FailReason;

                        nextRow1++;
                    }
                }

                excelPackage.Save();
            }
        }
    }
}
