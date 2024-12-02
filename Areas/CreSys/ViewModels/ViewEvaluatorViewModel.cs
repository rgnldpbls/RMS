using ResearchManagementSystem.Areas.CreSys.Models;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ViewEvaluatorViewModel
    {
        public EthicsEvaluator Evaluator { get; set; }
        public List<Expertise> ExpertiseList { get; set; }
        public List<int> SelectedExpertise { get; set; }
    }
}
