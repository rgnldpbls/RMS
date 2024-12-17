using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class NonFundedResearchInfo
    {
        [Key]
        public string NonFundedResearchId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]  
        public string UrecNo { get; set; }
        [ForeignKey(nameof(EthicsClearance))]
        public int? EthicsClearanceId { get; set; }
        [ForeignKey(nameof(CompletionCertificate))]
        public int? CompletionCertId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string? College { get; set; }
        public string? Campus { get; set; }
        public string University { get; set; }
        public DateTime? CompletionDate { get; set; }


        public EthicsApplication EthicsApplication { get; set; }
        public EthicsClearance EthicsClearance { get; set; }
        public CompletionCertificate CompletionCertificate { get; set; }
        public ICollection<CoProponent> CoProponents { get; set; }
    }
}
