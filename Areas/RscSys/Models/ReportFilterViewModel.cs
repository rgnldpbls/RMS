namespace rscSys_final.Models
{
    public class ReportFilterViewModel
    {
        
        public string ReportType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? College { get; set; }
        public string? Branch { get; set; }
        public string FileType { get; set; }
        // List to hold generated reports
        public List<GeneratedReport> GeneratedReports { get; set; } = new List<GeneratedReport>();
    }
}
