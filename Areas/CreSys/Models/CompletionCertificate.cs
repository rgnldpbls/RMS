using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class CompletionCertificate
    {
        [Key]
        public int CompletionCertId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string UrecNo { get; set; }
        public string? Message { get; set; }
        public DateOnly IssuedDate { get; set; }
        public byte[] CertificateFile { get; set; }

        public EthicsApplication EthicsApplication { get; set; }
        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
    }
}
