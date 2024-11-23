using Azure.Core;

namespace rscSys_final.Models
{
    public class Requirement
    {
        public int RequirementId { get; set; }
        public int? DraftId { get; set; }  // Foreign key from Drafts table
        public int? RequestId { get; set; }
        public int? ChecklistId { get; set; } // Foreign key to the Checklist
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public bool IsResubmitted { get; set; }
        public Draft Draft { get; set; }
        public Request Request { get; set; }
        public Checklist Checklist { get; set; } // Navigation property
    }
}
