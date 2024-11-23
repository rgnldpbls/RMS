using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class AssignReviewTypeViewModel
    {
        public ApplicationUser User { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public ICollection<CoProponent> CoProponent { get; set; }
        public ReceiptInfo ReceiptInfo { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
        public InitialReview? InitialReview { get; set; }
        public IEnumerable<EthicsApplicationForms> EthicsApplicationForms { get; set; }
        public IEnumerable<EthicsApplicationLogs> EthicsApplicationLogs { get; set; }
        public string ReviewType { get; set; }
    }
}
