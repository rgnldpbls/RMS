using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRE.Models
{
    public class EthicsReport
    {
        [Key]
        public string reportId { get; set; }
        [ForeignKey(nameof(Chief))]
        public int chiefId { get; set; }

        [Required]
        public string reportName { get; set; }
        [Required]
        public byte[] reportFile { get; set; }
        [Required]
        public DateOnly dateGenerated { get; set; }
        public Chief Chief { get; set; }
    }
}
