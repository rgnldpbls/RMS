using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class CompletionCertificate
    {
        [Key]
        public int completionCertId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        public string? message { get; set; }
        [Required]
        public DateOnly issuedDate { get; set; }
        [Required]
        public byte[] file { get; set; }

        //navigation properties
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
        public EthicsApplication EthicsApplication { get; set; }
    }
}
