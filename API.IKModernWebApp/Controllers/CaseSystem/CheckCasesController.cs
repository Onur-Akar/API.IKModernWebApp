using API.IKModernWebApp.Model.DAL;
using API.IKModernWebApp.ViewModel;
using API.IKModernWebApp.Workers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.IKModernWebApp.Helpers;

namespace API.IKModernWebApp.Controllers.CaseSystem
{
    [Route("api/{namespace}/[controller]")]
    [ApiController]
    public class CheckCasesController : ControllerBase
    {
        private readonly ILogger<CheckCasesController> _logger;
        private readonly IKMDbContext _ikmDbContext;

        public CheckCasesController(IKMDbContext ikmDbContext, ILogger<CheckCasesController> logger)
        {
            _ikmDbContext = ikmDbContext;
            _logger = logger;
        }
        public IActionResult Get()
        {
            _logger.LogInformation("Emanet kontrolü başladı");

            string groupName = "";
            List<Company> companies = new List<Company>();
            companies = _ikmDbContext.Company.Where(c => c.CompanyGroupId == 1).ToList();

            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 2;
            int skipSize = 0;
            while ((skipSize - 4) <= companies.Count)
            {
                Parallel.ForEach(companies.Skip(skipSize).Take(4), opt, company =>
                {
                    CompanyViewModel companyView = DBHelper.GetViewModelForCompany(company, groupName);

                    ScrapingWorkers workers = new ScrapingWorkers();
                    string result = workers.VaultScraper(companyView);
                });
                skipSize += 4;
            }
            return Ok("Thats OK!");
        }
    }
}
