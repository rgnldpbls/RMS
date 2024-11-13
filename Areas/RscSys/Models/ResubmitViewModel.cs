namespace rscSys_final.Models
{
    public class ResubmitViewModel
    {
        public Request Request { get; set; }
        public List<Checklist>? Checklists { get; set; }
        public List<Requirement>? Requirements { get; set; }
        public List<DocumentHistory> DocumentHistories { get; set; }
    }
}
