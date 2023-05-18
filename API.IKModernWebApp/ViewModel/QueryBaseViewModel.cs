using System.Collections.Generic;

namespace API.IKModernWebApp.ViewModel
{
    public class QueryBaseViewModel
    {
        public int UserID { get; set; }
        public List<CompanyViewModel> CompanyQueryList { get; set; }
    }
}
