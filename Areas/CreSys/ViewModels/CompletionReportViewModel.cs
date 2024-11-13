using CRE.Models;

namespace CRE.ViewModels
{
    public class CompletionReportViewModel
    {
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public IEnumerable<CoProponent>? CoProponent { get; set; }
        public EthicsApplication? EthicsApplication { get; set; }
        public IEnumerable<EthicsApplicationLog>? EthicsApplicationLog { get; set; }
        public CompletionReport? CompletionReport { get; set; }
        public CompletionCertificate? CompletionCertificate { get; set; }
        public IFormFile CertificateFile { get; set; }
    }
}
