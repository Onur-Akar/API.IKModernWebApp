using API.IKModernWebApp.ViewModel;
using HtmlAgilityPack;
using ScrapySharp.Html;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;
using System;
using System.Linq;

namespace API.IKModernWebApp.Workers
{
    public class TemporaryIncapacityReportOperationWorkers
    {
        public bool TemporaryIncapacityReportQuery(WebPage webPage, TemporaryIncapacityReportQueryViewModel reportQueryViewModel)
        {
            WebPage queryPage = webPage.FindLinks(By.Text("Tarihe Göre Rapor Arama")).Single().Click();
            PageWebForm queryForm = queryPage.FindForm("TarihAramaForm");
            queryForm["tarih"] = DateTime.Now.ToString("dd.MM.yyyy");

            WebPage resultPage;
            switch (reportQueryViewModel.IncapacityReportType)
            {
                case IncapacityReportType.All:
                    queryForm["vaka"] = IncapacityReportType.All.ToString();
                    resultPage = queryForm.Submit();
                    CreateJsonFileFromResults(resultPage);
                    break;
                case IncapacityReportType.Illness:
                    queryForm["vaka"] = IncapacityReportType.Illness.ToString();
                    resultPage = queryForm.Submit();
                    CreateJsonFileFromResults(resultPage);
                    break;
                case IncapacityReportType.Birth:
                    queryForm["vaka"] = IncapacityReportType.Birth.ToString();
                    resultPage = queryForm.Submit();
                    CreateJsonFileFromResults(resultPage);
                    break;
                case IncapacityReportType.JobAccident:
                    break;
                case IncapacityReportType.Archive:
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static void CreateJsonFileFromResults(WebPage resultPage)
        {
            HtmlDocument resultListDoc = new HtmlDocument();
            resultListDoc.LoadHtml(resultPage.Content);

            HtmlNodeCollection resultListRows = resultListDoc.DocumentNode.SelectNodes("//table//tr");
        }
    }
}
