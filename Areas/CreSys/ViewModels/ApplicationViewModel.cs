using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ApplicationViewModel
    {
        public EthicsApplication? EthicsApplication { get; set; }
        public InitialReview? InitialReview { get; set; }
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public IEnumerable<EthicsApplicationForms>? EthicsApplicationForms { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; }
        public IEnumerable<EthicsApplicationLogs>? EthicsApplicationLogs { get; set; }
        public IEnumerable<CoProponent>? CoProponent { get; set; }
    }
}
