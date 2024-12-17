namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ResearchReportModel
    {
        public string UrecNo { get; set; }
        public string TitleOfResearch { get; set; }
        public string ProponentsAuthors { get; set; }
        public DateTime? DateReceived { get; set; } = null; // Nullable date defaults to null
        public DateTime? DateReviewedForCompleteness { get; set; } = null;
        public DateTime? DateReceivedFromEvaluation { get; set; } = null;
        public DateTime? DateNoticeToProponents { get; set; } = null;
        public DateTime? DateApprovedByUREB { get; set; } = null;
        public DateTime? DateIssuedCertificate { get; set; } = null;

        // Additional fields for the "Total Terminal Report" and "Total Certificate of Completion"
        public DateTime? ResearchStartDate { get; set; } = null; // For Terminal Report
        public DateTime? ResearchEndDate { get; set; } = null;   // For Terminal Report
        public DateTime? TerminalReportSubmittedDate { get; set; } = null; // For Terminal Report

        // For the "Total Certificate of Completion"
        public DateOnly? CertificateIssuanceDate { get; set; } = null; // For Certificate of Completion

        // For the "Total Ethics Application" (which includes all applications)
        public DateOnly? CertificateOfCompletionDate { get; set; } = null; // Column for completion certificate

        public string Remarks { get; set; } = string.Empty;  // Default to empty string for Remarks
    }
}
