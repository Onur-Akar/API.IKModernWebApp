using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.IKModernWebApp.ViewModel
{
    public class FileRequestViewModel
    {
        public int GroupId { get; set; }
        public int CompanyId { get; set; }
        public List<IFormFile> FormFiles { get; set; }
    }
}
