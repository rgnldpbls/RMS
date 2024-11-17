using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CrdlSys.Models
{
    [Table("CRDL_StakeholderUpload")]

    public class StakeholderUpload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string DocumentId { get; set; }

        public string StakeholderId { get; set; } 
        public string StakeholderName { get; set; }
        public string StakeholdeEmail { get; set; }

        public string NameOfDocument { get; set; } 

        public string TypeOfDocument { get; set; } 

        public string? TypeOfMOA { get; set; } 

        public byte[] DocumentFile { get; set; } 

        public string DocumentDescription { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; 

        public DateOnly? ContractStartDate { get; set; } 

        public DateOnly? ContractEndDate { get; set; }

        public string Status { get; set; }

        public string? Comment { get; set; } 

        public string? ContractStatus { get; set; }

        public bool IsArchived { get; set; } = false;

        public bool IsManuallyUnarchived { get; set; } = false;

        public DateTime? LastNotificationSent { get; set; }

        public static string GenerateDocumentId()
        {
            var year = DateTime.UtcNow.Year.ToString();
            var random5Digits = new Random().Next(10000, 99999).ToString();
            var random2Digits = new Random().Next(10, 99).ToString();
            return $"{year}-{random5Digits}-STH-{random2Digits}";
        }
    }
}
