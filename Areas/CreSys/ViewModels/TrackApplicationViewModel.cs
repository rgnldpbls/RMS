using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class TrackApplicationViewModel
    {
        public EthicsApplication Application { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public List<CoProponent> CoProponents { get; set; }
        public List<EthicsApplicationLogs> Logs { get; set; }
    }
}
