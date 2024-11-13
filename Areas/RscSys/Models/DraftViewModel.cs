namespace rscSys_final.Models
{
    public class DraftViewModel
    {
        public Draft Draft { get; set; }
        public List<Memorandum>? Memorandums { get; set; }

        public List<ApplicationSubCategory>? ApplicationSubCategories { get; set; } = new List<ApplicationSubCategory>();
        public List<ApplicationType>? ApplicationTypes { get; set; } = new List<ApplicationType>(); 
    }
}
