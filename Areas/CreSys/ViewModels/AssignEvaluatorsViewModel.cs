using CRE.Models; // Assuming this is where your models are located
using System.Collections.Generic;

namespace CRE.ViewModels
{
    public class AssignEvaluatorsViewModel
    {
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<CoProponent> CoProponent { get; set; }
        public InitialReview InitialReview { get; set; }
        public List<EthicsEvaluator> AllAvailableEvaluators { get; set; } = new List<EthicsEvaluator>();
        public List<EthicsEvaluator> RecommendedEvaluators { get; set; }
        public List<EthicsEvaluator> PendingEvaluators { get; set; }
        public List<EthicsEvaluator> AcceptedEvaluators { get; set; }
        public List<EthicsEvaluator> DeclinedEvaluators { get; set; }
        public List<int> SelectedEvaluatorIds { get; set; } = new List<int>();
        public IEnumerable<EthicsApplicationForms> EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLog { get; set; }
        public ReceiptInfo ReceiptInfo { get; set; }
        public bool IsEvaluatorLimitReached { get; set; }
    }
}
