using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rscSys_final.Models
{
    public class ApplicationSubCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public int ApplicationTypeId { get; set; }
        public string CategoryName { get; set; }
        [Column(TypeName = "numeric(11,2)")]
        public decimal SubAmount { get; set; }
        public virtual ApplicationType ApplicationType { get; set; }
        // Each sub-category can have a collection of Checklists
        public ICollection<Checklist> Checklists { get; set; }

    }
}
