using System;

namespace API.IKModernWebApp.ViewModel
{
    public enum IncapacityReportType
    {
        All,
        Illness,
        Birth,
        JobAccident,
        Archive
    }

    public enum ReportProcessType
    {
        Submit,
        Query
    }
    public class TemporaryIncapacityReportQueryViewModel : QueryBaseViewModel
    {
        public IncapacityReportType IncapacityReportType { get; set; }
        public ReportProcessType ReportProcessType { get; set; }
        public DateTime? ArchiveQueryDate { get; set; }
        public DateTime? BirthReportSubmitDate { get; set; }
    }
}