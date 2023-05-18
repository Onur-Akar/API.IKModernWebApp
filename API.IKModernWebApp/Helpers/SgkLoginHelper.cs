using API.IKModernWebApp.Helpers.AntiCaptchaHelper;
using API.IKModernWebApp.ViewModel;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace API.IKModernWebApp.Helpers
{
    public class SgkLoginHelper
    {
        private const string ANTI_CAPTCHA_CLIENT_ID = "ANTI_CAPTCHA_CLIENT_ID";

        public WebPage SgkEmployerSystemLogin(CompanyViewModel company)
        {
            ScrapingBrowser scrapingBrowser = new ScrapingBrowser();
            WebPage homePage = null;
            scrapingBrowser.ClearCookies();
            scrapingBrowser.KeepAlive = true;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            scrapingBrowser.Encoding = Encoding.UTF8;

            WebPage loginPage = scrapingBrowser.NavigateToPage(new Uri("https://uyg.sgk.gov.tr/IsverenSistemi"));
            string captcha = GetSecurityCodeFromBrowser(scrapingBrowser, "https://uyg.sgk.gov.tr/IsverenSistemi/PG");
            PageWebForm loginForm = loginPage.FindFormById("kullaniciIlkKontrollerGiris");
            loginForm["username"] = company.UserName;
            loginForm["isyeri_kod"] = company.Extension;
            loginForm["password"] = company.SystemPassword;
            loginForm["isyeri_sifre"] = company.WorkPassword;
            loginForm["isyeri_guvenlik"] = captcha;

            try
            {
                homePage = loginForm.Submit();
            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().ToLower().Contains("302"))
                {
                    scrapingBrowser.Encoding = Encoding.UTF8;
                    homePage = scrapingBrowser.NavigateToPage(new Uri("https://uyg.sgk.gov.tr/IsverenSistemi/pages/baslangic.jsf"));
                }
            }
            WebPage main = homePage.Browser.NavigateToPage(new Uri("https://uyg.sgk.gov.tr/IsverenSistemi/pages/baslangic.jsf"));
            return main;
        }

        public WebPage SgkEmployerBorrowQuerySystemLogin(CompanyViewModel company)
        {
            ScrapingBrowser scrapingBrowser = new ScrapingBrowser();
            WebPage homePage = null;
            scrapingBrowser.ClearCookies();
            scrapingBrowser.KeepAlive = true;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            scrapingBrowser.Encoding = Encoding.UTF8;

            WebPage loginPage = scrapingBrowser.NavigateToPage(new Uri("https://uyg.sgk.gov.tr/IsverenBorcSorgu/borc/donemselBorc.action"));
            string captcha = GetSecurityCodeFromBrowser(scrapingBrowser, "https://uyg.sgk.gov.tr/IsverenBorcSorgu/simpleCaptcha.png");
            PageWebForm loginForm = loginPage.FindFormById("userLogin");
            loginForm["basvuru.tcKimlikNo"] = company.UserName;
            loginForm["basvuru.isyeriKodu"] = company.Extension;
            loginForm["basvuru.sistemSifre"] = company.SystemPassword;
            loginForm["basvuru.isyeriSifre"] = company.WorkPassword;
            loginForm["captchaStr"] = captcha;
            try
            {
                homePage = loginForm.Submit();
            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().ToLower().Contains("302"))
                {
                    scrapingBrowser.Encoding = Encoding.UTF8;
                    homePage = scrapingBrowser.NavigateToPage(new Uri("https://uyg.sgk.gov.tr/IsverenBorcSorgu/borc/donemselBorc.action"));
                }
            }

            return homePage;
        }

        public WebPage SgkEDeclarationV2SystemLogin(CompanyViewModel company)
        {
            ScrapingBrowser scrapingBrowser = new ScrapingBrowser();
            scrapingBrowser.ClearCookies();
            scrapingBrowser.KeepAlive = true;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            scrapingBrowser.Encoding = Encoding.UTF8;
            WebPage loginPage = scrapingBrowser.NavigateToPage(new Uri("https://ebildirge.sgk.gov.tr/EBildirgeV2"));
            string captcha = GetSecurityCodeFromBrowser(scrapingBrowser, "https://ebildirge.sgk.gov.tr/EBildirgeV2/EBildirgeV2/PG");
            PageWebForm loginForm = loginPage.FindFormById("kullaniciIlkKontrollerGiris");
            loginForm["username"] = company.UserName;
            loginForm["isyeri_kod"] = company.Extension;
            loginForm["password"] = company.SystemPassword;
            loginForm["isyeri_sifre"] = company.WorkPassword;
            loginForm["isyeri_guvenlik"] = captcha;
            WebPage homePage = loginForm.Submit();

            if (homePage.Content.Contains("ANASAYFA"))
            {
                //string filePath = IOHelper.CreateFilePathWithType(@$"{company.CompanyName}", "json", $@"\{company.GroupId}\{company.CompanyId}");
                //IOHelper.CreateJsonFileFromResults(company, filePath);
                return homePage;
            }
            else
            {
                return null;
            }
        }

        public WebPage SgkTemporaryIncapacityReportSystemLogin(CompanyViewModel company)
        {
            ScrapingBrowser scrapingBrowser = new ScrapingBrowser();
            scrapingBrowser.ClearCookies();
            scrapingBrowser.KeepAlive = true;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            scrapingBrowser.Encoding = Encoding.UTF8;
            WebPage loginPage = scrapingBrowser.NavigateToPage(new Uri("https://uyg.sgk.gov.tr/vizite/welcome.do"));
            string captcha = GetSecurityCodeFromBrowser(scrapingBrowser, "https://uyg.sgk.gov.tr/vizite/Captcha.jpg");
            PageWebForm loginForm = loginPage.FindFormById("KullaniciLoginForm");
            loginForm["kullaniciAdi"] = company.UserName;
            loginForm["isyeriKodu"] = company.Extension;
            loginForm["isyeriSifresi"] = company.WorkPassword;
            loginForm["guvenlikKodu"] = captcha;
            WebPage homePage = loginForm.Submit();

            if (homePage.Content.Contains("DUYURU!!"))
            {
                return homePage;
            }
            else
            {
                return null;
            }
        }

        private static string GetSecurityCodeFromBrowser(ScrapingBrowser browser, string captchaUrl)
        {
            string code;
            WebPage captchaPage = browser.NavigateToPage(new Uri(captchaUrl));
            byte[] captcha = captchaPage.RawResponse.Body;

            using (MemoryStream ms = new MemoryStream(captcha))
            {
                Bitmap image = new Bitmap(ms);
                bool solveCaptcha = SolveCaptcha(image, out code);
            }
            return code;
        }


        public static bool SolveCaptcha(Bitmap captchaImage, out string code)
        {
            if (!AntiCaptchaUtils.GetBalance(ANTI_CAPTCHA_CLIENT_ID, out var balance))
            {
                throw new ApplicationException("Captcha servisinde bakiye sorgulanamıyor");
            }

            if (balance <= double.Epsilon)
            {
                throw new ApplicationException("Captcha servisinde bakiye bitti. Yeniden yükleme yapılması gerek");
            }

            string tempFileName = Path.GetTempFileName();

            using (MemoryStream m = new MemoryStream())
            {
                if (captchaImage == null || captchaImage.Width == 0)
                {
                    code = "";
                    return false;
                }
                captchaImage.Save(m, ImageFormat.Png);
                byte[] imageBytes = m.ToArray();
                File.WriteAllBytes(tempFileName, imageBytes);
            }

            //_hub?.CaptchaResmi(_userId, "Captcha çözülüyor", tempFileName, balance);

            var res = AntiCaptchaUtils.SolveImage(ANTI_CAPTCHA_CLIENT_ID, tempFileName, out code);
            code = code.ToUpperInvariant();

            if (!res)
            {
                return false;
            }

            try
            {
                string path = @"C:\Temp\";
                string target = Path.Combine(path, code + ".png");
                if (File.Exists(target))
                {
                    File.Delete(target);
                }

                File.Copy(tempFileName, target);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return true;
        }

        public static WebPage CheckSessionForPage(WebPage webPage, CompanyViewModel company, string url, string menuId)
        {
            while (!webPage.Content.Contains(menuId))
            {
                webPage = webPage.Browser.NavigateToPage(new Uri(url));
                if (!webPage.Content.Contains(menuId))
                {
                    SgkLoginHelper loginHelper = new SgkLoginHelper();
                    webPage = loginHelper.SgkEmployerSystemLogin(company);
                }
            }
            return webPage;
        }
    }
}
