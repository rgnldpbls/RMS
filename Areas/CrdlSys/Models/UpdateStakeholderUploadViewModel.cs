namespace CrdlSys.Models
{
    public class UpdateStakeholderUploadViewModel
    {
        public string DocumentId { get; set; }
        public DateOnly? ContractStartDate { get; set; }
        public DateOnly? ContractEndDate { get; set; }
        public string Status { get; set; }
        public string? Comment { get; set; }
        public string? ContractStatus { get; set; }
    }

}
