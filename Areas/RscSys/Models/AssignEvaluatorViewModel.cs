namespace rscSys_final.Models
{
    public class AssignEvaluatorViewModel
    {
        public Request Request { get; set; }
        public List<Requirement>? Requirements { get; set; }
        public List<Evaluator> EvaluatorProfile { get; set; } = new List<Evaluator>();
        public List<EvaluatorAssignmentViewModel> EvaluatorAssignments { get; set; } = new List<EvaluatorAssignmentViewModel>();
    }
}
