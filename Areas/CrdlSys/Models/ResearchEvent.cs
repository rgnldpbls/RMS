using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrdlSys.Models
{
    [Table("CRDL_ResearchEvent")]
    public class ResearchEvent
    {
        [Key]
        public string ResearchEventId { get; set; }

        public string EventName { get; set; }

        public string EventDescription { get; set; }

        public string EventLocation { get; set; }

        public byte[]? EventThumbnail { get; set; }

        public string EventType { get; set; }

        public string RegistrationType { get; set; }

        public DateTime EventDate { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime RegistrationOpen { get; set; }

        public DateTime RegistrationDeadline { get; set; }

        public string EventStatus { get; set; }

        public int ParticipantsSlot { get; set; }

        public int ParticipantsCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool IsArchived { get; set; } = false;

        public static string GenerateResearchEventId()
        {
            var year = DateTime.Now.Year;
            var random4Digits = new Random().Next(1000, 9999);
            var random2Digits = new Random().Next(10, 99);
            return $"RCBA-EVNT-{year}-{random4Digits}-{random2Digits}";
        }
    }
}
