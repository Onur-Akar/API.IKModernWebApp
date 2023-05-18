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
    public class EmployerMinimumWageSupportsController : ControllerBase
    {
        private readonly ILogger<EmployerMinimumWageSupportsController> _logger;
        private readonly IKMDbContext _ikmDbContext;

        public EmployerMinimumWageSupportsController(IKMDbContext ikmDbContext, ILogger<EmployerMinimumWageSupportsController> logger)
        {
            _ikmDbContext = ikmDbContext;
            _logger = logger;
        }

        [HttpGet("minimumwagesupportbyid")]
        public IActionResult MinimumWageSupportById(int groupId, int? companyId)
        {
            _logger.LogInformation("İşveren Asgari Ücret Destek Kontrolü başladı");

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
                    string result = workers.SgkEmployerSystemScraper(companyView, "minimumwage_download");
                });
                skipSize += 4;
            }

            return Ok("That's OK");
        }

        [HttpPost("minimumwagesupportsbycompanyinfo")]
        public IActionResult MinimumWageSupportsByCompanyInfo([FromBody] CompanyViewModel company)
        {
            _logger.LogInformation("İşveren Asgari Ücret Destek Kontrolü başladı");

            ScrapingWorkers workers = new ScrapingWorkers();
            string result = workers.SgkEmployerSystemScraper(company, "minimumwage_download");

            return Ok("That's OK");
        }

        [HttpPost("minimumwagesupportsbycompanylist")]
        public IActionResult MinimumWageSupportsByCompanyList([FromBody] List<CompanyViewModel> companies)
        {
            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 2;
            int skipSize = 0;
            while ((skipSize - 4) <= companies.Count)
            {
                Parallel.ForEach(companies.Skip(skipSize).Take(4), opt, company =>
                {
                    ScrapingWorkers workers = new ScrapingWorkers();
                    string result = workers.SgkEmployerSystemScraper(company, "minimumwage_download");
                });
                skipSize += 4;
            }

            return Ok("That's OK");
        }
    }
}
