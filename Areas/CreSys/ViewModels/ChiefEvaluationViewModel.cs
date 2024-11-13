using CRE.Models;
using System.ComponentModel.DataAnnotations;

namespace CRE.ViewModels
{
    public class ChiefEvaluationViewModel
    {
        public Secretariat? Secretariat { get; set; }
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public ICollection<CoProponent>? CoProponent { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public Chairperson? Chairperson { get; set; }
        public EthicsEvaluator? EthicsEvaluator { get; set; }
        public EthicsApplication? EthicsApplication { get; set; }
        public InitialReview? InitialReview { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLog>? EthicsApplicationLog { get; set; }
        public EthicsEvaluation? EthicsEvaluation { get; set; } = new EthicsEvaluation();

        [Required(ErrorMessage = "Please upload the Protocol Review Sheet.")]
        public IFormFile ProtocolReviewSheet { get; set; }

        [Required(ErrorMessage = "Please upload the Informed Consent Form.")]
        public IFormFile InformedConsentForm { get; set; }

    }
}
