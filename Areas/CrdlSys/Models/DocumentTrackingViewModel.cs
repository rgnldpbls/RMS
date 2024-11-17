namespace CrdlSys.Models
{
    public class DocumentTrackingViewModel
    {
        public string TrackingId { get; set; }
        public bool IsReceivedByRMO { get; set; }
        public bool IsSubmittedToOVPRED { get; set; }
        public bool IsSubmittedToLegalOffice { get; set; }
        public bool IsReceivedByOVPRED { get; set; }
        public bool IsReceivedByRMOAfterOVPRED { get; set; }
        public bool IsSubmittedToOfficeOfThePresident { get; set; }
        public bool IsReceivedByRMOAfterOfficeOfThePresident { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ActivityHistoryViewModel> ActivityHistory { get; set; } = new();
    }

    public class ActivityHistoryViewModel
    {
        public string Status { get; set; }
        public bool IsReceivedByRMO { get; set; }
        public DateTime? IsReceivedByRMOUpdatedAt { get; set; }
        public bool IsSubmittedToOVPRED { get; set; }
        public DateTime? IsSubmittedToOVPREDUpdatedAt { get; set; }
        public bool IsSubmittedToLegalOffice { get; set; }
        public DateTime? IsSubmittedToLegalOfficeUpdatedAt { get; set; }
        public bool IsReceivedByOVPRED { get; set; }
        public DateTime? IsReceivedByOVPREDUpdatedAt { get; set; }
        public bool IsReceivedByRMOAfterOVPRED { get; set; }
        public DateTime? IsReceivedByRMOAfterOVPREDUpdatedAt { get; set; }
        public bool IsSubmittedToOfficeOfThePresident { get; set; }
        public DateTime? IsSubmittedToOfficeOfThePresidentUpdatedAt { get; set; }
        public bool IsReceivedByRMOAfterOfficeOfThePresident { get; set; }
        public DateTime? IsReceivedByRMOAfterOfficeOfThePresidentUpdatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
