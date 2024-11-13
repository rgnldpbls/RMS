using CRE.Models;

namespace CRE.ViewModels
{
    public class ApplicationViewModel
    {
        public EthicsApplication EthicsApplication { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public InitialReview InitialReview { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLog { get; set; }
        public IEnumerable<CoProponent> CoProponent { get; set; }
    }
}
