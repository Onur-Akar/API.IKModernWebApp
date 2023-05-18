using API.IKModernWebApp.Model.DAL;
using API.IKModernWebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.IKModernWebApp.Controllers.TemporaryIncapacityReportSystem
{
    [Route("api/{namespace}/[controller]")]
    [ApiController]
    public class TemporaryIncapacityReportController : ControllerBase
    {
        private readonly ILogger<TemporaryIncapacityReportController> _logger;
        private readonly IKMDbContext _ikmDbContext;
        public TemporaryIncapacityReportController(IKMDbContext ikmDbContext, ILogger<TemporaryIncapacityReportController> logger)
        {
            _ikmDbContext = ikmDbContext;
            _logger = logger;
        }

        [HttpPost("incapacityreportquery")]
        public IActionResult TemporaryIncapacityReportQuery([FromBody] TemporaryIncapacityReportQueryViewModel reportQueryViewModel)
        {
            return Ok("That's OK");
        }
    }
}
