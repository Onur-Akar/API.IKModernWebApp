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
    public class EmployerIllegalPromotionSupportsController : ControllerBase
    {
        private readonly ILogger<EmployerIllegalPromotionSupportsController> _logger;
        private readonly IKMDbContext _ikmDbContext;

        public EmployerIllegalPromotionSupportsController(IKMDbContext iKMDbContext, ILogger<EmployerIllegalPromotionSupportsController> logger)
        {
            _ikmDbContext = iKMDbContext;
            _logger = logger;
        }


        [HttpGet("illegalpromotionsupportsbyid")]
        public IActionResult IllegalPromotionSupportsById(int groupId, int? companyId)
        {
            _logger.LogInformation("İşveren Yersiz Faydalanılan Teşvik Kontrolü başladı");

            string groupName = "";
            List<Company> companies = DBHelper.GetCompanies(_ikmDbContext, groupId, companyId, out groupName);

            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 2;
            int skipSize = 0;
            while ((skipSize - 4) <= companies.Count)
            {
                Parallel.ForEach(companies.Skip(skipSize).Take(4), opt, company =>
                {
                    CompanyViewModel companyView = DBHelper.GetViewModelForCompany(company, groupName);

                    ScrapingWorkers workers = new ScrapingWorkers();
                    string result = workers.SgkEmployerSystemScraper(companyView, "illegal_promotion_support");
                });
                skipSize += 4;
            }

            return Ok("That's OK");
        }

        [HttpPost("illegalpromotionsupportsbycompanyinfo")]
        public IActionResult IllegalPromotionSupportsByCompanyInfo([FromBody] CompanyViewModel company)
        {
            _logger.LogInformation("İşveren Yersiz Faydalanılan Teşvik Kontrolü başladı");

            ScrapingWorkers workers = new ScrapingWorkers();
            string result = workers.SgkEmployerSystemScraper(company, "illegal_promotion_support");

            return Ok("That's OK");
        }

        [HttpPost("illegalpromotionsupportsbycompanylist")]
        public IActionResult IllegalPromotionSupportsByCompanyList([FromBody] List<CompanyViewModel> companies)
        {
            _logger.LogInformation("İşveren Yersiz Faydalanılan Teşvik Kontrolü başladı");

            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 2;
            int skipSize = 0;
            Parallel.ForEach(companies.Skip(skipSize).Take(4), opt, company =>
            {
                ScrapingWorkers workers = new ScrapingWorkers();
                string result = workers.SgkEmployerSystemScraper(company, "illegal_promotion_support");
            });
            skipSize += 4;

            return Ok("That's OK");
        }
    }
}
