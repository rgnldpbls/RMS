namespace rscSys_final.Models
{
    public class EvaluatorViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string? Specialization { get; set; }
        public int? CompletedCount { get; set; }
        public int? PendingCount { get; set; }
        public int? DeclinedCount { get; set; }
    }
}
