using System;
using System.ComponentModel.DataAnnotations;

namespace DocumentTrackingSystemBackend.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(100)]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; } 

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Birthday is required.")]
        [DataType(DataType.Date)]
        public DateOnly Birthday { get; set; }

        [Required(ErrorMessage = "College name is required.")]
        [StringLength(200)]
        public string College { get; set; }

        [Required(ErrorMessage = "Branch is required.")]
        [StringLength(200)]
        public string Branch { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(300)]
        public string Email { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(300)]
        public string? Webmail { get; set; } 

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

    }
}
