using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrdlSys.Models
{
    [Table("CRDL_ResearchEventInvitation")]
    public class ResearchEventInvitation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string InvitationId { get; set; }

        [Required]
        [ForeignKey("ResearchEvent")]
        public string ResearchEventId { get; set; }

        public string UserName {  get; set; }
        public string UserEmail { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(10)]
        public string InvitationStatus { get; set; } = "Pending";

        [Required]
        public DateTime InvitedAt { get; set; } = DateTime.Now;
        public virtual ResearchEvent ResearchEvent { get; set; }

        public static string GenerateInvitationId()
        {
            var year = DateTime.Now.Year;
            var random = new Random();
            var fourRandomDigits = random.Next(1000, 9999);
            var oneRandomDigit = random.Next(1, 9);

            return $"{year}-INV-{fourRandomDigits}-{oneRandomDigit}";
        }
    }
}
