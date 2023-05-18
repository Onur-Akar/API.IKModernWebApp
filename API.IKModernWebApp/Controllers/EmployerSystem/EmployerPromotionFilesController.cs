using API.IKModernWebApp.ViewModel;
using API.IKModernWebApp.Model.DAL;
using API.IKModernWebApp.Workers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.IKModernWebApp.Helpers;

namespace API.IKModernWebApp.Controllers.EmployerSystem
{
    [Route("api/{namespace}/[controller]")]
    [ApiController]
    public class EmployerPromotionFilesController : ControllerBase
    {
        private readonly ILogger<EmployerPromotionFilesController> _logger;
        private readonly IKMDbContext _ikmDbContext;

        public EmployerPromotionFilesController(IKMDbContext ikmDbContext, ILogger<EmployerPromotionFilesController> logger)
        {
            _ikmDbContext = ikmDbContext;
            _logger = logger;
        }


        [HttpGet("promotionfilesbyid")]
        public IActionResult PromotionFilesById(int groupId, int? companyId)
        {
            _logger.LogInformation("İşveren Sistemi Teşvik Alan Çalışan Listeleri İndirilmeye Başladı");
            string groupName = "";
            List<Company> companies = DBHelper.GetCompanies(_ikmDbContext, groupId, companyId, out groupName);

            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 2;//Environment.ProcessorCount;
            int skipSize = 0;
            while ((skipSize - 4) <= companies.Count)
            {
                Parallel.ForEach(companies.Skip(skipSize).Take(4), opt, company =>
                {
                    CompanyViewModel companyView = DBHelper.GetViewModelForCompany(company, groupName);

                    ScrapingWorkers workers = new ScrapingWorkers();
                    string result = workers.SgkEmployerSystemScraper(companyView, "promotion_download");
                });
                skipSize += 4;
            }

            return Ok("That's OK");
        }

        [HttpPost("promotionfilesbycompanyinfo")]
        public IActionResult PromotionFilesByCustomerInfo([FromBody] CompanyViewModel company)
        {
            _logger.LogInformation("İşveren Sistemi Teşvik Alan Çalışan Listeleri İndirilmeye Başladı");

            ScrapingWorkers workers = new ScrapingWorkers();
            string result = workers.SgkEmployerSystemScraper(company, "promotion_download");

            return Ok("That's OK");
        }

        [HttpPost("promotionfilesbycompanylist")]
        public IActionResult PromotionFilesByCustomerList([FromBody] List<CompanyViewModel> companies)
        {
            _logger.LogInformation("İşveren Sistemi Teşvik Alan Çalışan Listeleri İndirilmeye Başladı");

            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 2;//Environment.ProcessorCount;
            int skipSize = 0;
            while ((skipSize - 4) <= companies.Count)
            {
                Parallel.ForEach(companies.Skip(skipSize).Take(4), opt, company =>
                {
                    ScrapingWorkers workers = new ScrapingWorkers();
                    string result = workers.SgkEmployerSystemScraper(company, "promotion_download");
                });
                skipSize += 4;
            }

            return Ok("That's OK");
        }
    }
}
