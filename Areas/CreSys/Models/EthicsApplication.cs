using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsApplication
    {
        [Key]
        public string UrecNo { get; set; }
        public string? UserId { get; set; } //Foreign key to ApplicationUser
        public string? Name { get; set; }
        public DateTime SubmissionDate { get; set; }
        [RegularExpression(@"\d{4}-\d{4}-\d{2}", ErrorMessage = "DTS No. must be in the format xxxx-xxxx-xx")]
        public string? DtsNo { get; set; }
        public string FieldOfStudy { get; set; }

        public CompletionCertificate CompletionCertificate { get; set; }
        public CompletionReport CompletionReport { get; set; }
        public DeclinedEthicsEvaluation DeclinedEthicsEvaluation { get; set; }
        public ReceiptInfo ReceiptInfo { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public InitialReview InitialReview { get; set; }
        public ICollection<EthicsNotifications> EthicsNotifications { get; set; }
        public EthicsClearance EthicsClearance { get; set; }
        public ICollection<EthicsEvaluation> EthicsEvaluation { get; set; }
        public ICollection<EthicsApplicationLogs> EthicsApplicationLogs { get; set; }
        public ICollection<EthicsApplicationForms> EthicsApplicationForms { get; set; }
    }
}
