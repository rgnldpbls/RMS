using System.ComponentModel.DataAnnotations;

namespace rscSys_final.Models
{
    public class Draft
    {
        public int DraftId { get; set; }
        public string UserId { get; set; }  // Foreign key from Users table
        public string Name { get; set; }
        public string Branch { get; set; }
        public string? College { get; set; }
        public string Email { get; set; }
        public string? DtsNo { get; set; }
        [Required(ErrorMessage = "Application Type is required")]
        [StringLength(100, ErrorMessage = "Application Type cannot be longer than 100 characters")]
        public string ApplicationType { get; set; }
        [Required(ErrorMessage = "Field of Study is required")]
        [StringLength(100, ErrorMessage = "Field of Study cannot be longer than 100 characters")]
        public string FieldOfStudy { get; set; }
        public string? ResearchTitle { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        public ICollection<Requirement>? Requirements { get; set; }
    }
}
