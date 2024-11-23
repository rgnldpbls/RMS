using System.ComponentModel.DataAnnotations;

namespace CrdlSys.ViewModels
{
    public class CreateRCBAEventViewModel
    {
        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(200)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event description is required.")]
        [StringLength(1000)]
        public string EventDescription { get; set; }
        public IFormFile? EventThumbnail { get; set; }

        [Required(ErrorMessage = "Event location is required.")]
        [StringLength(500)]
        public string EventLocation { get; set; }

        [Required(ErrorMessage = "Event type is required.")]
        [StringLength(100)]
        public string EventType { get; set; }

        [Required(ErrorMessage = "Registration type is required.")]
        [StringLength(100)]
        public string RegistrationType { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Registration open is required.")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationOpen { get; set; }

        [Required(ErrorMessage = "Registration deadline is required.")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDeadline { get; set; }

        [Required(ErrorMessage = "Participant slot is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Participant slot must be at least 1.")]
        public int ParticipantSlot { get; set; }
        public List<string> SelectedResearchers { get; set; } = new List<string>();
    }
}
