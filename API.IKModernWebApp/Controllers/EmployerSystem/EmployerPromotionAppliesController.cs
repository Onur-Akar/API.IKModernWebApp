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
    public class EmployerPromotionAppliesController : ControllerBase
    {
        private readonly ILogger<EmployerPromotionAppliesController> _logger;
        private readonly IKMDbContext _ikmDbContext;

        public EmployerPromotionAppliesController(IKMDbContext ikmDbContext, ILogger<EmployerPromotionAppliesController> logger)
        {
            _ikmDbContext = ikmDbContext;
            _logger = logger;
        }

        [HttpGet("promotionapplybyid")]
        public IActionResult PromotionApplyById(int groupId, int? companyId)
        {
            _logger.LogInformation("İşveren Teşvik Alan Çalışan Kontrolü başladı");
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
                    string result = workers.SgkEmployerSystemScraper(companyView);
                });
                skipSize += 4;
            }

            return Ok("That's OK");
        }

        [HttpPost("promotionapplybycompanyinfo")]
        public IActionResult PromotionApplyByCompanyInfo([FromBody] CompanyViewModel company)
        {
            ScrapingWorkers workers = new ScrapingWorkers();
            string result = workers.SgkEmployerSystemScraper(company);
            return Ok("That's OK");
        }

        [HttpPost("promotionapplybycompanylist")]
        public IActionResult PromotionApplyByCompanyList([FromBody] List<CompanyViewModel> companies)
        {
            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 2;
            int skipSize = 0;
            while ((skipSize - 4) <= companies.Count)
            {
                Parallel.ForEach(companies.Skip(skipSize).Take(4), opt, company =>
                {
                    ScrapingWorkers workers = new ScrapingWorkers();
                    string result = workers.SgkEmployerSystemScraper(company);
                });
                skipSize += 4;
            }

            return Ok("That's OK");
        }
    }
}
