using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class CompletionReportViewModel
    {
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<CoProponent> CoProponent { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
        public IEnumerable<EthicsApplicationLogs> EthicsApplicationLog { get; set; }
        public CompletionReport? CompletionReport { get; set; }
        public CompletionCertificate CompletionCertificate { get; set; }
        public IFormFile CertificateFile { get; set; }
    }
}
