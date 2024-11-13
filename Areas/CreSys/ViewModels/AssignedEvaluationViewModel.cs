using CRE.Models;

namespace CRE.ViewModels
{
    public class AssignedEvaluationViewModel
    {
        public EthicsApplication? EthicsApplication { get; set; }
        public EthicsEvaluation? EthicsEvaluation { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationLog>? EthicsApplicationLogs { get; set; }
        public InitialReview? InitialReview { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public EthicsEvaluationDeclined EthicsEvaluationDeclined { get; set; }
    }
}