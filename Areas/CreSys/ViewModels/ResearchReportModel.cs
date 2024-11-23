namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ResearchReportModel
    {
        public string UrecNo { get; set; }
        public string TitleOfResearch { get; set; }
        public string ProponentsAuthors { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? DateReviewedForCompleteness { get; set; }
        public DateTime? DateReceivedFromEvaluation { get; set; }
        public DateTime? DateNoticeToProponents { get; set; }
        public DateTime? DateApprovedByUREB { get; set; }
        public DateTime? DateIssuedCertificate { get; set; }
        public string Remarks { get; set; } = string.Empty;  // Empty for now
    }
}
