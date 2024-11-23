using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace rscSys_final.Models
{
    public class ApplicationType
    {
        public int ApplicationTypeId { get; set; }
        public string UserId { get; set; }
        public string ApplicationTypeName { get; set; }
        [Column(TypeName = "numeric(11,2)")]
        public decimal Amount { get; set; }

        public DateTime ApplicationTypeCreated { get; set; } = DateTime.Now;
        public DateTime? ApplicationTypeUpdated { get; set; } = DateTime.Now;
        // Changed to a collection of ApplicationSubCategory
        public ICollection<ApplicationSubCategory> ApplicationSubCategories { get; set; }
        public ICollection<Checklist> Checklists { get; set; }
    }
}
