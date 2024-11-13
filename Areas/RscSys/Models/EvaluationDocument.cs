namespace rscSys_final.Models
{
    public class EvaluationDocument
    {
        public int EvaluationDocuId { get; set; }
        public int EvaluatorAssignmentId { get; set; } // Foreign key to EvaluatorAssignment
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedAt { get; set; }
        // Navigation property to link to EvaluatorAssignment
        public virtual EvaluatorAssignment EvaluatorAssignment { get; set; }
    }
}
