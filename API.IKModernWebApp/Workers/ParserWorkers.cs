using API.IKModernWebApp.ViewModel;
using HtmlAgilityPack;
using ScrapySharp.Html;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.IKModernWebApp.Workers
{
    public class ParserWorkers
    {
        public List<EmployeeViewModel> TableToClassParserWorker(WebPage personalListPage)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(personalListPage.Content);
            HtmlNodeCollection personalList = doc.DocumentNode.SelectNodes("//input[@type='radio']");
            foreach (HtmlNode personal in personalList)
            {
                PageWebForm personalForm = personalListPage.FindFormById("uygunSigortaliBilgileriDonem");

                personalForm.FormFields.RemoveAt(personalForm.FormFields.FindIndex(x => x.Name == "action:uygunSigortaliBilgileriDonem"));
                personalForm.FormFields.RemoveAt(personalForm.FormFields.FindIndex(x => x.Name == "action:uygunSigortaliBilgileriKimlik"));
                personalForm.FormFields.RemoveAt(personalForm.FormFields.FindIndex(x => x.Name == "action:tesvikTanimlamaSigortaliSirala"));
                personalForm.FormFields.RemoveAt(personalForm.FormFields.FindIndex(x => x.Name == "action:tesvikTanimlamaSigortaliSirala"));

                personalForm["ucretDestegiTalebiVarMi"] = string.Empty;
                personalForm["egitimDurumu"] = "1";
                personalForm["iseGirisMapIndex"] = personal.GetAttributeValue("value", "");

                try
                {
                    WebPage personalPage = personalForm.Submit();
                }
                catch (Exception ex)
                {
                    string err = ex.Message;
                }
            }

            return null;
        }

        public void PromotionResultParserWorker(WebPage resultPage)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(resultPage.Content);
            HtmlNodeCollection results = doc.DocumentNode.SelectNodes("//");
            foreach (HtmlNode result in results)
            {

            }
        }

        public List<KeyValuePair<int, string>> SelectToPeriodParserWorker(HtmlNode periodSelect, bool isRemove = true)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(periodSelect.InnerHtml);
            HtmlNodeCollection options = doc.DocumentNode.SelectNodes("//option");
            if (isRemove)
            {
                options.RemoveAt(0);
            }
            List<KeyValuePair<int, string>> periodList = new List<KeyValuePair<int, string>>();

            foreach (HtmlNode option in options)
            {
                KeyValuePair<int, string> period = new KeyValuePair<int, string>(Convert.ToInt32(option.GetAttributeValue("value", "")), option.InnerText);
                periodList.Add(period);
            }
            periodList.Sort((x, y) => y.Key.CompareTo(x.Key));
            return periodList;
        }

        ~ParserWorkers() { }
    }
}
