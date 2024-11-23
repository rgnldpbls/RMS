namespace CrdlSys.Models
{
    public class ReportViewModel
    {
        public List<ResearchEvent> ResearchEvents { get; set; } = new List<ResearchEvent>();
        public List<GeneratedReport> GeneratedReports { get; set; } = new List<GeneratedReport>();
    }

}
