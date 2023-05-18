using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.IKModernWebApp.ViewModel
{
    public class PassedMonthIllegalPromotionSupportViewModel : BaseViewModel
    {
        public string IllegalPromotionYear { get; set; }
        public string IllegalPromotionMonth { get; set; }
        public string SgkCity { get; set; }
        public string OldBranch { get; set; }
        public string NewBranch { get; set; }
        public string QueNumber { get; set; }
        public string Mediator { get; set; }
        public string PromotionLaw { get; set; }
        public string NID { get; set; }
        public string IllegalSupportReason { get; set; }
        public string Status { get; set; }
        public string CaseOpenDate { get; set; }
        public string CaseOpenTime { get; set; }
        public string SubmitWaitDocument { get; set; }
    }
}
