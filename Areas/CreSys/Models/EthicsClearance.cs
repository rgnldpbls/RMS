using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class EthicsClearance
    {
        [Key]
        public int ethicsClearanceId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        [Required]
        public DateOnly? issuedDate { get; set; }
        [Required]
        public DateOnly? expirationDate { get; set; }
        public byte[]? file { get; set; }

        //navigation properties
        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
    }
}
