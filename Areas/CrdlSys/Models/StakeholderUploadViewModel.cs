namespace CrdlSys.ViewModels
{
    public class StakeholderUploadViewModel
    {
        public string DocumentId { get; set; }
        public string TrackingId { get; set; } 
        public string StakeholderId { get; set; }
        public string StakeholderName { get; set; }
        public string NameOfDocument { get; set; }
        public string TypeOfDocument { get; set; }
        public string TypeOfMOA { get; set; }
        public byte[] DocumentFile { get; set; } 
        public string DocumentDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateOnly? ContractStartDate { get; set; }
        public DateOnly? ContractEndDate { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string ContractStatus { get; set; }
        public bool IsArchived { get; set; }
    }
}
