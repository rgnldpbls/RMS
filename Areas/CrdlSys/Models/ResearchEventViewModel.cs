namespace CrdlSys.ViewModels
{
    public class ResearchEventViewModel
    {
        public string ResearchEventId { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public string EventLocation { get; set; }
        public string EventType { get; set; }
        public string RegistrationType { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime RegistrationOpen { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public string EventStatus { get; set; }
        public int ParticipantsSlot { get; set; }
        public int ParticipantsCount { get; set; }
        public byte[]? EventThumbnail { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsArchived { get; set; }
        public bool IsUserRegistered { get; set; }
        public bool IsInvited { get; set; }
        public bool invitationAccepted { get; set; }
        public string? InvitationStatus { get; set; }
    }
}
