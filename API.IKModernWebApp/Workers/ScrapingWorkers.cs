using API.IKModernWebApp.Helpers;
using API.IKModernWebApp.ViewModel;
using HtmlAgilityPack;
using ScrapySharp.Html;
using ScrapySharp.Network;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.IKModernWebApp.Workers
{
    public class ScrapingWorkers
    {

        public string VaultScraper(CompanyViewModel companyView)
        {
            SgkLoginHelper loginHelper = new SgkLoginHelper();
            WebPage homePage = loginHelper.SgkEmployerBorrowQuerySystemLogin(companyView);
            WebPage casePage = homePage.FindLinks(By.Text("Emanetteki Tahsilatlar")).Single().Click();

            HtmlDocument doc = new HtmlDocument();
            HtmlToPdf htmlToPdf = new HtmlToPdf();
            doc.LoadHtml(casePage.Content);
            doc.DocumentNode.SelectSingleNode("//input[@type='submit']").Remove();
            doc.DocumentNode.SelectSingleNode("//table[@class='tabloHeader']").Remove();
            doc.DocumentNode.SelectNodes("//table[@class='tabloCss1']")[1].Remove();

            PdfDocument pdf = htmlToPdf.ConvertHtmlString(doc.DocumentNode.OuterHtml);
            pdf.Save($@"C:\pdf\{companyView.CompanyId}_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.pdf");
            return "OK";
        }

        public string SgkEmployerSystemScraper(CompanyViewModel companyView, string processType = "promotion_submit")
        {
            SgkLoginHelper loginHelper = new SgkLoginHelper();
            WebPage homePage = loginHelper.SgkEmployerSystemLogin(companyView);
            EmployerSystemOperationWorkers workers = new EmployerSystemOperationWorkers();
            bool isSuccess;
            switch (processType)
            {
                case "promotion_submit":
                    isSuccess = workers.EmployeeApplyPormotionScrapper(homePage, companyView);
                    break;
                case "promotion_download":
                    isSuccess = workers.EmployeePromotionDownloadScrapper(homePage, companyView);
                    break;
                case "minimumwage_download":
                    isSuccess = workers.MinimumWageSupportPageScrapper(homePage, companyView);
                    break;
                case "illegal_promotion_support":
                    isSuccess = workers.IllegalPromotionSupportControlPageScraper(homePage, companyView);
                    break;
                default:
                    break;
            }

            return "OK";
        }

        public string SgkEDeclarationV2SystemScraperAsync(CompanyViewModel companyView, List<string> filePaths = null, string processType = "xml_upload")
        {
            SgkLoginHelper loginHelper = new SgkLoginHelper();
            WebPage homePage = loginHelper.SgkEDeclarationV2SystemLogin(companyView);
            EDeclarationV2SystemOperationWorkers workers = new EDeclarationV2SystemOperationWorkers();
            bool isSuccess;
            switch (processType)
            {
                case "xml_upload":
                    isSuccess = workers.EDeclarationV2SystemXmlUploadAsync(homePage, companyView, filePaths);
                    break;
                default:
                    break;
            }
            return "OK";
        }

        public string SgkTemporaryIncapacityReportSystem(CompanyViewModel companyView, TemporaryIncapacityReportQueryViewModel reportQueryViewModel)
        {
            SgkLoginHelper loginHelper = new SgkLoginHelper();
            WebPage homePage = loginHelper.SgkTemporaryIncapacityReportSystemLogin(companyView);
            TemporaryIncapacityReportOperationWorkers workers = new TemporaryIncapacityReportOperationWorkers();
            bool isSuccess;
            switch (reportQueryViewModel.ReportProcessType)
            {
                case ReportProcessType.Submit:
                    break;
                case ReportProcessType.Query:
                    isSuccess = workers.TemporaryIncapacityReportQuery(homePage, reportQueryViewModel);
                    break;
                default:
                    break;
            }
            return "OK";
        }

    }
}