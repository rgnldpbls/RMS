namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ChiefDashboardViewModel
    {
        public int TotalApplications { get; set; }
        public int TotalClearancesIssued { get; set; }
        public int TotalTerminalReports { get; set; }
        public int TotalCertificatesIssued { get; set; }
        public List<TopFieldViewModel> TopFields { get; set; }
        public List<ApplicationsPerMonth> ApplicationsPerMonthByYear { get; set; }
        public int ApplicationsPerMonth { get; set; }
        public List<BestEvaluatorViewModel> BestPerformingEvaluators { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public int TotalApplicationsForYear { get; set; }

        public List<int> AvailableYears { get; set; }
        public List<MonthViewModel> AvailableMonths { get; set; }
    }
    public class TopFieldViewModel
    {
        public string FieldName { get; set; }
        public int ApplicationCount { get; set; }
        public int Rank { get; set; }
    }
    public class BestEvaluatorViewModel
    {
        public string EvaluatorName { get; set; }
        public int CompletedCount { get; set; }
        public int Rank { get; set; }
    }
    public class MonthViewModel
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
    }
    public class ApplicationsPerMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int ApplicationCount { get; set; }
    }


}
