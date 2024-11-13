using CRE.Models;

namespace CRE.ViewModels
{
    public class ApplicationsViewModel
    {
        public IEnumerable<NonFundedResearchInfo> NonFundedResearchInfo { get; set; } = new List<NonFundedResearchInfo>();
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLog { get; set; } = new List<EthicsApplicationLog>();
        public IEnumerable<EthicsApplication> EthicsApplication { get; set; } = new List<EthicsApplication>();

        
    }
}
