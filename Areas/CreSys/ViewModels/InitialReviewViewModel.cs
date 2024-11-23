using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class InitialReviewViewModel
    {
        public ApplicationUser User { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public ICollection<CoProponent> CoProponent { get; set; }
        public ReceiptInfo ReceiptInfo { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
        public IEnumerable<EthicsEvaluation>? EthicsEvaluation { get; set; }
        public InitialReview InitialReview { get; set; }
        public IEnumerable<EthicsApplicationForms> EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLogs> EthicsApplicationLog { get; set; }
        public IEnumerable<DeclinedEthicsEvaluation> DeclinedEthicsEvaluation { get; set; }
    }
}
