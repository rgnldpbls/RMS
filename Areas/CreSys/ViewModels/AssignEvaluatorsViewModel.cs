using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class AssignEvaluatorsViewModel
    {
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<CoProponent> CoProponent { get; set; }
        public List<EthicsEvaluator> AllAvailableEvaluators { get; set; } = new List<EthicsEvaluator>();
        public List<EthicsEvaluator> RecommendedEvaluators { get; set; }
        public List<EthicsEvaluator> PendingEvaluators { get; set; }
        public List<EthicsEvaluator> AcceptedEvaluators { get; set; }
        public List<EthicsEvaluator> DeclinedEvaluators { get; set; }
        public List<EthicsEvaluator> EvaluatedEvaluators { get; set; } = new List<EthicsEvaluator>();
        public IEnumerable<EthicsApplicationForms> EthicsApplicationForms { get; set; }
        public ReceiptInfo ReceiptInfo { get; set; }
        public bool IsEvaluatorLimitReached { get; set; }
    }
}
