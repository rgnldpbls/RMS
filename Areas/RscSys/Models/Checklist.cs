using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace rscSys_final.Models
{
    public class Checklist
    {
        [Key]
        public int ChecklistId { get; set; }
        public string ChecklistName { get; set; }
        public int? ApplicationTypeId { get; set; }

        // This can be optional if you want checklists to be related to a specific sub-category
        public int? ApplicationSubCategoryId { get; set; }
        public virtual ApplicationSubCategory ApplicationSubCategory { get; set; }
        public virtual ICollection<Requirement> Requirements { get; set; }
        public virtual ApplicationType ApplicationType { get; set; }
    }
}
