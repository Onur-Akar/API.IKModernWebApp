using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.IKModernWebApp.ViewModel
{
    public class MinimumWageSupportModelView : BaseViewModel
    {
        public string ApplyYear { get; set; }
        public string ApplyMonth { get; set; }
        public string NumberOfDaysUsed { get; set; }
        public decimal SupportAmount { get; set; }
        public string CollectionDate { get; set; }
    }
}
