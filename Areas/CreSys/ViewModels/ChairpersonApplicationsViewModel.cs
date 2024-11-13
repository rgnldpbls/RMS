using CRE.Models;

namespace CRE.ViewModels
{
    public class ChairpersonApplicationsViewModel
    {
        public IEnumerable<EthicsApplication> UnassignedApplications { get; set; }
        public IEnumerable<EthicsApplication> UnderEvaluationApplications { get; set; }
        public IEnumerable<EthicsApplication> EvaluationResultApplications { get; set; }
        public Dictionary<string, List<EthicsEvaluator>> ApplicationEvaluatorNames { get; set; } = new();
        public IEnumerable<EthicsApplicationLog> EthicsApplicationLogs  { get; set; }
        public IEnumerable<NonFundedResearchInfo> NonFundedResearchInfo { get; set; }
    }

}
