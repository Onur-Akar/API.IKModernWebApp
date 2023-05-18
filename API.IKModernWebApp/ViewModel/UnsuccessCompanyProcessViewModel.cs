using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.IKModernWebApp.ViewModel
{
    public class UnsuccessCompanyProcessViewModel : BaseViewModel
    {
        public string ConnectedSystem { get; set; }
        public string FailReason { get; set; }
    }
}
