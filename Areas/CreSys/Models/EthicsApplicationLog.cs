using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class EthicsApplicationLog
    {
        // Primary Key
        [Key]
        public int logId { get; set; }

        // Foreign Keys
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        public string userId { get; set; }

        // Required Fields
        [Required]
        [Display(Name ="Status: ")]
        public string status { get; set; }

        [Required]
        public DateTime changeDate { get; set; }

        public string? comments { get; set; }

        // Navigation Properties
        public EthicsApplication EthicsApplication { get; set; }
    }
}
