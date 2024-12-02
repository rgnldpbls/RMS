using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class EvaluationDataViewModel
    {
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public List<CoProponent> CoProponents { get; set; } = new List<CoProponent>();
        public InitialReview InitialReview { get; set; }
        public EthicsEvaluation EthicsEvaluation { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLogs>? EthicsApplicationLog { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public string? EvaluatorType { get; set; }
    }
}
