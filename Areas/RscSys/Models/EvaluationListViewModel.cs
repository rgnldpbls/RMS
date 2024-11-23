namespace rscSys_final.Models
{
    public class EvaluationListViewModel
    {
        public List<EvaluatorAssignment> ToBeEvaluated { get; set; } = new List<EvaluatorAssignment>();
        public List<EvaluatorAssignment> Evaluated { get; set; } = new List<EvaluatorAssignment>();
    }
}
