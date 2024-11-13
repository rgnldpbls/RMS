using Microsoft.AspNetCore.Identity;

namespace ResearchManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public string? College { get; set; }

        public string? Campus { get; set; }

        public string? Webmail { get; set; }
    }
}