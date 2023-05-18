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
    public class EmployerApplyForLawChangingsController : ControllerBase
    {
        private readonly ILogger<EmployerApplyForLawChangingsController> _logger;
        private readonly IKMDbContext _ikmDbContext;
        public EmployerApplyForLawChangingsController(IKMDbContext ikmDbContext, ILogger<EmployerApplyForLawChangingsController> logger)
        {
            _ikmDbContext = ikmDbContext;
            _logger = logger;
        }

        [HttpGet("applyforpromotionlawchangingbyid")]
        public IActionResult GetApplyForPromotionLawChangingById(int groupId, int? companyId)
        {
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
                    string result = workers.SgkEmployerSystemScraper(companyView, "apply_law_changing");
                });
                skipSize += 4;
            }

            return Ok("That's OK");
        }
    }
}
