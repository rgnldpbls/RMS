namespace rscSys_final.Models
{
    public class TechEvalViewModel
    {
        public Request Request { get; set; }
        public List<Checklist>? Checklists { get; set; }
        public List<Requirement>? Requirements { get; set; }
        public IEnumerable<DocumentHistory> DocumentHistories { get; set; } // New property
        public bool IsDecisionDisabled { get; set; } // Add this flag
    }
}
