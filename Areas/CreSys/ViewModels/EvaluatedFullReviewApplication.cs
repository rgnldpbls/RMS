using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class EvaluatedFullReviewApplication
    {
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public List<EthicsEvaluation> EthicsEvaluation { get; set; }
        public List<string> EthicsEvaluator { get; set; }
        public List<EthicsEvaluator> EthicsEvaluatorDetails { get; set; }
        public InitialReview InitialReview { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<EthicsApplicationLogs> EthicsApplicationLog { get; set; }
    }
}
