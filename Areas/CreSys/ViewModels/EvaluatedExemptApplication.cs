using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class EvaluatedExemptApplication
    {
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationLogs> EthicsApplicationLog { get; set; }
        public EthicsEvaluation EthicsEvaluation { get; set; } // Ensure this property exists
        public InitialReview InitialReview { get; set; }
        public ApplicationUser User { get; set; }

        public IEnumerable<CoProponent> CoProponents { get; set; }

    }
}
