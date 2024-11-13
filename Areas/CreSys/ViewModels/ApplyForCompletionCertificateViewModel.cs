using CRE.Models;
using Microsoft.Identity.Client;

namespace CRE.ViewModels
{
    public class ApplyForCompletionCertificateViewModel
    {
        public EthicsApplication? EthicsApplication { get; set; }
        public EthicsClearance? EthicsClearance { get; set; }    
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }    
        public IEnumerable<CoProponent>? CoProponents { get; set; }
        public IEnumerable<EthicsApplicationLog>? EthicsApplicationLog { get; set; }
        public CompletionReport? CompletionReport { get; set; }
    }
}
