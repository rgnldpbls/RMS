using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrdlSys.Models
{
    [Table("CRDL_ResearchEventRegistration")]
    public class ResearchEventRegistration
    {
        [Key]
        public string RegistrationId { get; set; } = GenerateRegistrationId();

        [ForeignKey("ResearchEvent")]
        public string ResearchEventId { get; set; }
        public virtual ResearchEvent ResearchEvent { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserId { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public DateTime? LastNotificationSent { get; set; }

        private static string GenerateRegistrationId()
        {
            var year = DateTime.Now.Year;
            var randomPart1 = new Random().Next(1000, 9999);
            var randomPart2 = new Random().Next(10, 99);
            return $"REG-RCBAE-{year}-{randomPart1}-{randomPart2}";
        }
    }
}
