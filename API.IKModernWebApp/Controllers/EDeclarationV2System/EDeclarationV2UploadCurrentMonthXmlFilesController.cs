using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.IKModernWebApp.Helpers;
using API.IKModernWebApp.Model.DAL;
using API.IKModernWebApp.ViewModel;
using API.IKModernWebApp.Workers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.IKModernWebApp.Controllers.EDeclarationV2System
{
    [Route("api/{namespace}/[controller]")]
    [ApiController]
    public class EDeclarationV2UploadCurrentMonthXmlFilesController : ControllerBase
    {
        private readonly ILogger<EDeclarationV2UploadCurrentMonthXmlFilesController> _logger;
        private readonly IKMDbContext _ikmDbContext;

        public EDeclarationV2UploadCurrentMonthXmlFilesController(IKMDbContext ikmDbContext, ILogger<EDeclarationV2UploadCurrentMonthXmlFilesController> logger)
        {
            _ikmDbContext = ikmDbContext;
            _logger = logger;
        }


        [HttpPost("xmlupladbyid")]
        public IActionResult UploadCurrentMonthXmlFilesById([FromForm] FileRequestViewModel fileData)
        {
            _logger.LogInformation("İşveren Teşvik Alan Çalışan Kontrolü başladı");
            string groupName;
            List<Company> companies = DBHelper.GetCompanies(_ikmDbContext, fileData.GroupId, fileData.CompanyId, out groupName);

            var filePaths = new List<string>();
            foreach (var formFile in fileData.FormFiles)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(Environment.CurrentDirectory, $"SystemUploads/{formFile.FileName}");
                    filePaths.Add(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                }
            }

            ParallelOptions opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 2;
            int skipSize = 0;
            while ((skipSize - 4) <= companies.Count)
            {
                Parallel.ForEach(companies.Skip(skipSize).Take(4), opt, company =>
                {
                    CompanyViewModel companyView = DBHelper.GetViewModelForCompany(company, groupName);

                    ScrapingWorkers workers = new ScrapingWorkers();
                    string result = workers.SgkEDeclarationV2SystemScraperAsync(companyView);
                });
                skipSize += 4;
            }

            return Ok("That's OK");
        }
    }
}
