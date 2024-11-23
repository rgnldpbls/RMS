using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsClearance
    {
        [Key]
        public int EthicsClearanceId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string UrecNo { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public byte[]? ClearanceFile { get; set; }

        public EthicsApplication EthicsApplication { get; set; }
    }
}
