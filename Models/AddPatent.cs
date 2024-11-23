using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Models
{
    public class AddPatent
    {

        [Key]
        public string patentId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(AddAccomplishment))]
        [Display(Name = "ProductionNo")]
        public virtual string? ProductionId { get; set; }

        public AddAccomplishment? AddAccomplishment { get; set; }

        [Display(Name = "Patent Number")]
        public string? PatentNo { get; set; }

        public byte[]? ApplicationFormData { get; set; }
        public string? ApplicationFormFileName { get; set; }
    }
}


