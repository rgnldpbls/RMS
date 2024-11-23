namespace rscSys_final.Models
{
    public class ApplicationViewModel
    {
        public Draft Draft { get; set; }
        public List<Checklist>? Checklists { get; set; }
        public List<Requirement>? Requirements { get; set; }
    }
}
