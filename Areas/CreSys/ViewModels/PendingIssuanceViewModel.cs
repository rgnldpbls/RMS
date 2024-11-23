using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class PendingIssuanceViewModel
    {
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsEvaluation> EthicsEvaluation { get; set; }

        public ICollection<EthicsApplicationLogs> EthicsApplicationLog { get; set; }
        public EthicsClearance? EthicsClearance { get; set; }
        public bool HasClearanceIssued { get; set; }

        public bool AllEvaluationsCompleted { get; set; }
        public bool HasForm15Uploaded { get; set; }
        public bool HasMinorOrMajorRevisions { get; set; }
    }
}
