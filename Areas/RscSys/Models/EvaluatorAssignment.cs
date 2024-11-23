namespace rscSys_final.Models
{
    public class EvaluatorAssignment
    {
        public int AssignmentId { get; set; }
        public int RequestId { get; set; }  // Foreign key to Request table
        public string EvaluatorId { get; set; }  // Foreign key to Users table for Evaluators
                                                
        public string EvaluationStatus { get; set; } = "Pending";  // Initial status is Pending
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public DateTime EvaluationDeadline { get; set; }
        public string? Feedback { get; set; }

        public Request Request { get; set; }  // Navigation property
        public Evaluator Evaluators { get; set; }
        public virtual ICollection<Notifications> Notifications { get; set; } = new List<Notifications>();
        // New navigation property for related EvaluationFormResponses
        /*public virtual ICollection<EvaluationFormResponse> EvaluationFormResponses { get; set; }*/
        /*public virtual ICollection<EvaluationGeneralComment> GeneralComments { get; set; } = new List<EvaluationGeneralComment>();*/
    }

}
