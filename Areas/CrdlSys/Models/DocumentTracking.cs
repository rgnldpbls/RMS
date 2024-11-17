using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CrdlSys.Models
{
    [Table("CRDL_DocumentTracking")]
    public class DocumentTracking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string TrackingId { get; set; }
        public string DocumentId { get; set; }
        public bool IsReceivedByRMO { get; set; } = true;
        public DateTime? IsReceivedByRMOUpdatedAt { get; set; }

        public bool IsSubmittedToOVPRED { get; set; } = false;
        public DateTime? IsSubmittedToOVPREDUpdatedAt { get; set; }

        public bool IsSubmittedToLegalOffice { get; set; } = false;
        public DateTime? IsSubmittedToLegalOfficeUpdatedAt { get; set; }

        public bool IsReceivedByOVPRED { get; set; } = false;
        public DateTime? IsReceivedByOVPREDUpdatedAt { get; set; }

        public bool IsReceivedByRMOAfterOVPRED { get; set; } = false;
        public DateTime? IsReceivedByRMOAfterOVPREDUpdatedAt { get; set; }

        public bool IsSubmittedToOfficeOfThePresident { get; set; } = false;
        public DateTime? IsSubmittedToOfficeOfThePresidentUpdatedAt { get; set; }

        public bool IsReceivedByRMOAfterOfficeOfThePresident { get; set; } = false;
        public DateTime? IsReceivedByRMOAfterOfficeOfThePresidentUpdatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


        [ForeignKey("DocumentId")]
        public virtual StakeholderUpload StakeholderUpload { get; set; }

        public static string GenerateTrackingId()
        {
            var year = DateTime.UtcNow.Year.ToString();
            var random4Digits = new Random().Next(1000, 9999).ToString();
            var random2Digits = new Random().Next(10, 99).ToString();
            return $"DTS-{year}-{random4Digits}-{random2Digits}";
        }
    }
}
