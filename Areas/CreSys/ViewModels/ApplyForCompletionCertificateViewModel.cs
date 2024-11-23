using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ApplyForCompletionCertificateViewModel
    {
        public EthicsApplication? EthicsApplication { get; set; }
        public EthicsClearance? EthicsClearance { get; set; }
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public IEnumerable<CoProponent>? CoProponents { get; set; }
        public IEnumerable<EthicsApplicationLogs>? EthicsApplicationLog { get; set; }
        public CompletionReport? CompletionReport { get; set; }
    }
}
