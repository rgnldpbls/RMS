namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class TabbedEvaluationViewModel
    {
        public IEnumerable<AssignedEvaluationViewModel> EvaluationAssignments { get; set; } = new List<AssignedEvaluationViewModel>();
        public IEnumerable<AssignedEvaluationViewModel> ToBeEvaluated { get; set; } = new List<AssignedEvaluationViewModel>();
        public IEnumerable<AssignedEvaluationViewModel> Evaluated { get; set; } = new List<AssignedEvaluationViewModel>();
        public IEnumerable<AssignedEvaluationViewModel> DeclinedEvaluations { get; set; } = new List<AssignedEvaluationViewModel>();
    }
}
