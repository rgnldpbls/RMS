using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrdlSys.Models
{
    [Table("CRDL_RenewalHistory")]
    public class RenewalHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [ForeignKey("StakeholderUpload")]
        public string DocumentId { get; set; } 

        public string TypeOfDocument { get; set; } 
        public DateOnly PreviousEndDate { get; set; }
        public DateOnly NewEndDate { get; set; } 
        public DateTime RenewalDate { get; set; } = DateTime.UtcNow; 

        public virtual StakeholderUpload StakeholderUpload { get; set; }

        public static string GenerateRenewalId()
        {
            var year = DateTime.UtcNow.Year.ToString();
            var random4Digits = new Random().Next(1000, 9999).ToString(); 
            var random2Digits = new Random().Next(10, 99).ToString(); 
            return $"RENEWAL-{year}-{random4Digits}-{random2Digits}";
        }
    }
}
