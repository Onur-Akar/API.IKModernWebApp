namespace API.IKModernWebApp.ViewModel
{
    public class TemporaryIncapacityReportViewModel : BaseViewModel
    {
        public string IdentityNumber { get; set; }
        public string FullName { get; set; }
        public IncapacityReportType ReportType { get; set; }
        public string ReportCheckNo { get; set; }
        public string StartDate { get; set; }
        public string JobReturnOrControlDate { get; set; }
        public bool IsPenalty { get; set; }
    }
}
