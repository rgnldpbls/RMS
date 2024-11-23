using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ChiefEvaluationViewModel
    {
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public ICollection<CoProponent>? CoProponent { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public IEnumerable<EthicsEvaluator>? EthicsEvaluators { get; set; }
        public EthicsApplication? EthicsApplication { get; set; }
        public InitialReview? InitialReview { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLogs>? EthicsApplicationLog { get; set; }
        public EthicsEvaluation? EthicsEvaluation { get; set; } = new EthicsEvaluation();

        [Required(ErrorMessage = "Please upload the Protocol Review Sheet.")]
        public IFormFile ProtocolReviewSheet { get; set; }

        [Required(ErrorMessage = "Please upload the Informed Consent Form.")]
        public IFormFile InformedConsentForm { get; set; }
    }
}
