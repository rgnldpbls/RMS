namespace rscSys_final.Models
{
    public class EvaluatorDashboardViewModel
    {
        public int PendingCount { get; set; }
        public int CompletedCount { get; set; }
        public List<EvaluatorAssignment>? EvaluatorAssignments { get; set; }
        public List<Memorandum>? Memorandums { get; set; }
    }
}
