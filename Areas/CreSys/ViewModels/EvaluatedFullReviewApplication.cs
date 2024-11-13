using CRE.Models;

namespace CRE.ViewModels
{
    public class EvaluatedFullReviewApplication
    {
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public List<EthicsEvaluation> EthicsEvaluation { get; set; }
        public List<EthicsEvaluator> EthicsEvaluator { get; set; }
        public InitialReview InitialReview { get; set; }
        public ICollection<EthicsApplicationLog> EthicsApplicationLog { get; set; }
    }
}
