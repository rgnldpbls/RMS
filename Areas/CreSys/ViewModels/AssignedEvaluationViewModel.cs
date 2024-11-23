using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class AssignedEvaluationViewModel
    {
        public EthicsApplication? EthicsApplication { get; set; }
        public EthicsEvaluation? EthicsEvaluation { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationLogs>? EthicsApplicationLogs { get; set; }
        public InitialReview? InitialReview { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public DeclinedEthicsEvaluation DeclinedEthicsEvaluation { get; set; }
    }
}
