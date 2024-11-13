namespace rscSys_final.Models
{
    public class EvaluationFormResponse
    {
        public int ResponseId { get; set; }
        public int CriterionId { get; set; }
        public decimal UserPercentage { get; set; }
        public string Comment { get; set; }
        public Criterion Criterion { get; set; }
        public string EvaluatorId { get; set; } // Foreign key to Evaluator
        public virtual Evaluator Evaluator { get; set; } // Navigation property
        public int EvaluatorAssignmentId { get; set; } // Foreign key to EvaluatorAssignment
        public virtual EvaluatorAssignment EvaluatorAssignment { get; set; } // Navigation property
    }
}
