using CRE.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CRE.Data.Validations
{
    public class RequiredIfHumanSubjects : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UploadFormsViewModel)validationContext.ObjectInstance;
            if (model.InvolvesHumanSubjects && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
