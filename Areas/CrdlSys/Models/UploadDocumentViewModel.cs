using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CrdlSys.ViewModels
{
    public class UploadDocumentViewModel
    {
        [Required(ErrorMessage = "Name of document is required.")]
        [StringLength(200)]
        public string NameOfDocument { get; set; }

        [Required(ErrorMessage = "Type of document is required.")]
        [StringLength(50)]
        public string TypeOfDocument { get; set; }

        [RequiredIfMoa(ErrorMessage = "Type of MOA is required.")]
        public string TypeOfMOA { get; set; }

        [Required(ErrorMessage = "Document file is required.")]
        public IFormFile DocumentFile { get; set; }

        [StringLength(500)]
        [Required(ErrorMessage = "Document Description is required.")]
        public string DocumentDescription { get; set; }
    }

    public class RequiredIfMoaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UploadDocumentViewModel)validationContext.ObjectInstance;

            if (model.TypeOfDocument == "MOA" && string.IsNullOrWhiteSpace(model.TypeOfMOA))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

}
