namespace rscSys_final.Models
{
    public class DocumentHistory
    {
        public int DocumentHistoryId { get; set; }
        public int RequestId { get; set; }  // Foreign key from Request table
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string Comments { get; set; }

        // Optional: Store which evaluator or user performed the action
        public string? PerformedBy { get; set; }

        // Navigation property
        public Request Request { get; set; }
    }
}
