namespace rscSys_final.Models
{
    public class Request
    {
        public int RequestId { get; set; }
        public string Name {  get; set; }
        public string? College { get; set; }
        public string Branch { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }  // Foreign key from Users table
/*        public ApplicationUser User { get; set; }*/
        public string DtsNo { get; set; }
        public string ApplicationType { get; set; }
        public string FieldOfStudy { get; set; }
        public string ResearchTitle { get; set; }
        public string Status { get; set; } = "For Evaluation";  // Default status
        public double RequestSpent { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? SubmittedDate { get; set; }
        public bool IsHardCopyReceived { get; set; } = false;

        public ICollection<Requirement>? Requirements { get; set; }
        public ICollection<StatusHistory>? StatusHistories { get; set; }
        public ICollection<DocumentHistory>? DocumentHistories { get; set; }
        public ICollection<EvaluatorAssignment> EvaluatorAssignments { get; set; } = new List<EvaluatorAssignment>();
        public ICollection<FinalDocument> FinalDocuments { get; set; }
    }
}
