using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class NonFundedResearchInfo
    {
        // Primary Key
        [Key, MaxLength(30)]
        public string nonFundedResearchId { get; set; }

        // Foreign Keys
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }

        [ForeignKey(nameof(EthicsClearance))]
        public int? ethicsClearanceId { get; set; }

        [ForeignKey(nameof(CompletionCertificate))]
        public int? completionCertId { get; set; }

        [Display(Name = "Project Proponent: ")]
        public string userId { get; set; }

        // Required Fields
        [Required(ErrorMessage ="Title of Research is Required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name ="Research Project Title: ")]
        public string title { get; set; }

        [Required]
        public DateTime dateSubmitted { get; set; }

        // Campus, College, and University attributes
        [Required(ErrorMessage ="Campus is Required.")]
        [StringLength(70, ErrorMessage = "Campus cannot exceed 70 characters")]
        [Display(Name ="Branch/Campus: ")]
        public string campus { get; set; }
        [StringLength(70, ErrorMessage = "College cannot exceed 70 characters")]
        [Required(ErrorMessage = "College is Required.")]
        [Display(Name = "College: ")]
        public string college { get; set; }
        [Required(ErrorMessage = "University is Required.")]
        [StringLength(70, ErrorMessage = "University cannot exceed 70 characters")]
        [Display(Name = "University: ")]
        public string university { get; set; }

        public DateOnly? completion_Date { get; set; }

        // Navigation Properties
        public EthicsApplication EthicsApplication { get; set; }
        public EthicsClearance EthicsClearance { get; set; }
        public CompletionCertificate CompletionCertificate { get; set; }
        public ICollection<CoProponent> CoProponent { get; set; } = new List<CoProponent>();
    }
}
