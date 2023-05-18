using API.IKModernWebApp.Helpers;
using API.IKModernWebApp.ViewModel;
using HtmlAgilityPack;
using ScrapySharp.Html;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.IKModernWebApp.Workers
{
    public class EDeclarationV2SystemOperationWorkers
    {
        public bool EDeclarationV2SystemXmlUploadAsync(WebPage webPage, CompanyViewModel companyView, List<string> filePaths)
        {
            webPage = SgkLoginHelper.CheckSessionForPage(webPage, companyView, "https://ebildirge.sgk.gov.tr/EBildirgeV2", "XML Dosyası ile Yükleme");
            webPage = webPage.FindLinks(By.Text("XML Dosyası ile Yükleme")).Single().Click();
            //Random rand = new Random();
            //string boundary = "----boundary" + rand.Next().ToString();
            //byte[] header = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"UploadFile\"; filename=\"" + Path.GetFileName(filePaths[0]) + "\"\r\nContent-Type: text/xml\r\n\r\n");
            //byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            //webPage.Browser.Headers.Add("Content-Type", "multipart/form-data; boundary="+boundary);
            //webPage.Browser.Headers.Add("xmlTahakkukdosyaYukle", ""+header);
            //NameValueCollection formData = new NameValueCollection();
            //formData["xmlTahakkukdosyaYukle"] = Path.GetFileName(filePaths[0]);
            byte[] file = System.Text.Encoding.ASCII.GetBytes(File.ReadAllText(filePaths[0]));
            try
            {
                //string sgkToken = AnahtarAl(webPage);
                //var page = webPage.Browser.NavigateToPage(new Uri($"https://ebildirge.sgk.gov.tr/EBildirgeV2/tahakkuk/xmlTahakkukdosyaYukle.action?struts.token.name=token&token={sgkToken}"), HttpVerb.Post);
                webPage.Find("input", By.Name("UploadFile")).Single().SetAttributeValue("value", filePaths[0]);
                PageWebForm form = webPage.FindFormById("xmlTahakkukdosyaYukle");
                webPage = form.Submit();

            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return true;
        }

        public WebPage CheckSessionForPage(WebPage webPage, CompanyViewModel company, string menuId)
        {
            while (!webPage.Content.Contains(menuId))
            {
                webPage = webPage.Browser.NavigateToPage(new Uri("https://ebildirge.sgk.gov.tr/EBildirgeV2"));
                if (!webPage.Content.Contains(menuId))
                {
                    SgkLoginHelper loginHelper = new SgkLoginHelper();
                    webPage = loginHelper.SgkEmployerSystemLogin(company);
                }
            }
            return webPage;
        }

        private string AnahtarAl(WebPage webPage)
        {


            HtmlDocument tokenDoc = new HtmlDocument();

            tokenDoc.LoadHtml(webPage.Content);

            string token = tokenDoc.DocumentNode.SelectSingleNode("//input[@name='token']/@value").GetAttributeValue("value", "");

            return token;
        }
    }
}
