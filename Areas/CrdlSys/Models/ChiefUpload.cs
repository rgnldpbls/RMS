using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrdlSys.Models
{
    [Table("CRDL_ChiefUpload")]
    public class ChiefUpload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string DocumentId { get; set; }

        public string NameOfDocument { get; set; }

        public string TypeOfDocument { get; set; } 

        public byte[] DocumentFile { get; set; }

        public string DocumentDescription { get; set; }

        public string StakeholderName { get; set; }

        public string EmailOfStakeholder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateOnly? ContractStartDate { get; set; }

        public DateOnly? ContractEndDate { get; set; }

        public string Status { get; set; } 

        public string? Comment { get; set; }

        public string? ContractStatus { get; set; }
        public bool IsArchived { get; set; } = false;

        public DateTime? LastNotificationSent { get; set; }

        public bool IsManuallyUnarchived { get; set; } = false;

        public static string GenerateDocumentId()
        {
            Random random = new Random();
            int random5Digits = random.Next(10000, 99999); 
            int random2Digits = random.Next(10, 99);       
            int currentYear = DateTime.UtcNow.Year;

            return $"{currentYear}-{random5Digits}-CHF-{random2Digits}";
        }
    }
}
