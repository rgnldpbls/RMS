namespace rscSys_final.Models
{
    public class Evaluator
    {
        public string EvaluatorId { get; set; } // Foreign key to ApplicationUser
        public string Name { get; set; }
        public string? Specialization { get; set; }
        public int? CompletedCount { get; set; }
        public int? PendingCount { get; set; }
        public int? DeclinedCount { get; set; }

        // Navigation property
        public virtual ICollection<EvaluationFormResponse> EvaluationFormResponses { get; set; } // Connects responses to evaluator
    }
}
