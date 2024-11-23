using NuGet.Protocol.Plugins;
using ResearchManagementSystem.Areas.CreSys.Models;
using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class EvaluationDetailsViewModel
    {
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public EthicsEvaluator? EthicsEvaluator { get; set; }
        public ICollection<CoProponent>? CoProponent { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public EthicsApplication? EthicsApplication { get; set; }
        public EthicsClearance? EthicsClearance { get; set; }
        public InitialReview? InitialReview { get; set; }
        public IEnumerable<EthicsEvaluation>? EthicsEvaluation { get; set; }
        public bool HasForm15Uploaded => EthicsApplicationForms?.Any(form => form.EthicsFormId == "FORM15") ?? false;
        public List<EvaluationWithEvaluatorViewModel> EvaluationsWithEvaluators { get; set; }
        public EthicsForms EthicsForms { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLogs>? EthicsApplicationLog { get; set; }
        public bool HasEthicsClearance { get; set; }
        public EthicsEvaluation? CurrentEvaluation { get; set; }
        // Optional files for upload, if needed
        [Required(ErrorMessage = "Please upload the Protocol Review Sheet.")]
        public IFormFile? ProtocolReviewSheet { get; set; }

        [Required(ErrorMessage = "Please upload the Informed Consent Form.")]
        public IFormFile? InformedConsentForm { get; set; }

    }
    public class EvaluationWithEvaluatorViewModel
    {
        public EthicsEvaluation Evaluation { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluatorUserId { get; set; }
    }
}
