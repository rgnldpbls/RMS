using CrdlSys.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CrdlSys.ViewModels
{
    public class ViewChiefUploadsViewModel
    {
        [Required(ErrorMessage = "Name of document is required.")]
        [StringLength(200)]
        public string NameOfDocument { get; set; }

        [Required(ErrorMessage = "Type of document is required.")]
        [StringLength(100)]
        public string TypeOfDocument { get; set; }

        [Required(ErrorMessage = "Document file is required.")]
        public IFormFile DocumentFile { get; set; }

        [Required(ErrorMessage = "Document description is required.")]
        [StringLength(500)]
        public string DocumentDescription { get; set; }

        [Required(ErrorMessage = "Stakeholder name is required.")]
        [StringLength(200)]
        public string StakeholderName { get; set; }

        [Required(ErrorMessage = "Email of stakeholder is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(500)]
        public string EmailOfStakeholder { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractEndDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [MaxLength(50)]
        public string? ContractStatus { get; set; }
        public List<ChiefUpload> UploadedDocuments { get; set; } = new List<ChiefUpload>();
        public bool IsArchived { get; set; }
    }
}
