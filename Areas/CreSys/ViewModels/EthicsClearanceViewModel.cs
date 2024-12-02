using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class EthicsClearanceViewModel
    {
        public EthicsApplication EthicsApplication { get; set; }
        public InitialReview InitialReview { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public IEnumerable<CoProponent> CoProponents { get; set; }
        public string ChiefName { get; set; }
    }
}
