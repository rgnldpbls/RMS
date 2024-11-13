using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRE.Models
{
    public class CoProponent
    {
        [Key]
        public int coProponentId { get; set; }
        [ForeignKey(nameof(NonFundedResearchInfo))]
        public string nonFundedResearchId { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; } // Navigation property

        [Required(ErrorMessage ="Name is Required")]
        [Display(Name ="Project Co-proponent: ")]
        public string coProponentName { get; set; }
    }
}
