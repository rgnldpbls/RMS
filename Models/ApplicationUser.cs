using Microsoft.AspNetCore.Identity;

namespace ResearchManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public DateOnly Birthday { get; set; }

        public string? College { get; set; }

        public string? Campus { get; set; }

        public string? Department { get; set; }

        public string? Webmail { get; set; }

        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastLogoutTime { get; set; }


        public string? Rank { get; set; } // Current rank of the faculty

        public DateTime? RankStartDate { get; set; } // Date when the rank was assigned

        public DateTime? RankEndDate { get; set; } // Optional: Date when the rank ended

    }
}