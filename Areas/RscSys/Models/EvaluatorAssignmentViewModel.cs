namespace rscSys_final.Models
{
    public class EvaluatorAssignmentViewModel
    {
        public int AssignmentId { get; set; }
        public int RequestId { get; set; }  // Foreign key to Request table
        public string EvaluatorId { get; set; }  // Foreign key to Users table for Evaluators
        public string EvaluatorName { get; set; }

        public string EvaluationStatus { get; set; } = "Pending";  // Initial status is Pending
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public DateTime EvaluationDeadline { get; set; }
        public string? Feedback { get; set; }
        public Request Request { get; set; }
    }
}
