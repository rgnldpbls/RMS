using CRE.Models;

namespace CRE.ViewModels
{
    public class PendingIssuance
    {
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public List<EthicsEvaluation> EthicsEvaluation { get; set; }
        public List<EthicsEvaluator> EthicsEvaluator { get; set; }
        public InitialReview InitialReview { get; set; }
        public ICollection<EthicsApplicationLog> EthicsApplicationLog { get; set; }
        public EthicsClearance EthicsClearance { get; set; }
        public bool HasClearanceIssued { get; set; }

        public bool AllEvaluationsCompleted { get; set; } 
        public bool HasForm15Uploaded { get; set; } 
        public bool HasMinorOrMajorRevisions { get; set; } 
    }
}
