using CRE.Models;

namespace CRE.ViewModels
{
    public class EvaluatedExemptApplication
    {
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLog { get; set; }
        public EthicsEvaluation? EthicsEvaluation { get; set; } // Ensure this property exists
        public InitialReview InitialReview { get; set; }
        public Chief Chief { get; set; }

        public IEnumerable<CoProponent> CoProponents => NonFundedResearchInfo?.CoProponent; // Adjust according to your model

        // Reviewer (Chief) Full Name
        public string ReviewerName => $"{EthicsEvaluation?.Chief?.Name}";
    }
}
