namespace rscSys_final.Models
{
    public class StatusHistory
    {
        public int StatusHistoryId { get; set; }
        public int RequestId { get; set; }  // Foreign key from Request table
        public string Status { get; set; }
        public DateTime StatusDate { get; set; } = DateTime.Now;

        // Optional: Include any comments or additional details for the status
        public string? Comments { get; set; }

        // Navigation property
        public Request Request { get; set; }
    }
}
