using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ChairpersonApplicationsViewModel
    {
        public IEnumerable<EthicsApplication> UnassignedApplications { get; set; } = Enumerable.Empty<EthicsApplication>();
        public IEnumerable<EthicsApplication> UnderEvaluationApplications { get; set; } = Enumerable.Empty<EthicsApplication>();
        public IEnumerable<EthicsApplication> EvaluationResultApplications { get; set; } = Enumerable.Empty<EthicsApplication>();
        public Dictionary<string, List<EthicsEvaluator>> ApplicationEvaluatorNames { get; set; }
        public IEnumerable<NonFundedResearchInfo> NonFundedResearchInfo { get; set; } = Enumerable.Empty<NonFundedResearchInfo>();
    }
}
