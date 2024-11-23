using System.ComponentModel.DataAnnotations;

namespace CrdlSys.Models
{
    public class SendDocumentViewModel
    {
        [Required(ErrorMessage = "Stakeholder name is required.")]
        [StringLength(100)]
        public string StakeholderName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(200)]
        public string EmailOfStakeholder { get; set; }

        [Required(ErrorMessage = "Name of document is required.")]
        [StringLength(100)]
        public string NameOfDocument { get; set; }

        [Required(ErrorMessage = "Type of document is required.")]
        [StringLength(100)]
        public string TypeOfDocument { get; set; }

        [Required(ErrorMessage = "Document description is required.")]
        [StringLength(200)]
        public string DocumentDescription { get; set; }

        [Required(ErrorMessage = "Document file is required.")]
        public IFormFile DocumentFile { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(100)]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Body is required.")]
        [StringLength(10000)]
        public string Body { get; set; }
    }

}
