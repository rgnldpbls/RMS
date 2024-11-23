namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ChiefDashboardViewModel
    {
        public int TotalApplications { get; set; }
        public int TotalClearancesIssued { get; set; }
        public int TotalTerminalReports { get; set; }
        public int TotalCertificatesIssued { get; set; }
        public List<TopFieldViewModel> TopFields { get; set; }
    }
    public class TopFieldViewModel
    {
        public string FieldName { get; set; }
        public int ApplicationCount { get; set; }
        public int Rank { get; set; }
    }
}
