using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class EthicsEvaluation
    {
        [Key]
        public int evaluationId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string? urecNo { get; set; }
        [ForeignKey(nameof(EthicsEvaluator))]
        public int? ethicsEvaluatorId { get; set; }
        [ForeignKey(nameof(Chief))]
        public int? chiefId { get; set; }
        public DateOnly? startDate { get; set; }
        public DateOnly? endDate { get; set; }
        [Required]
        public string evaluationStatus { get; set; } = "Pending";

        // Protocol Review Sheet Evaluation
        public string? ProtocolRecommendation { get; set; }
        public string? ProtocolRemarks { get; set; }
        public byte[]? ProtocolReviewSheet { get; set; }

        // Informed Consent Form Evaluation
        public string? ConsentRecommendation { get; set; }
        public string? ConsentRemarks { get; set; }
        public byte[]? InformedConsentForm { get; set; }

        public string? reasonForDecline { get; set; }

        //navigation properties
        public EthicsEvaluator EthicsEvaluator { get; set; }
        public EthicsApplication? EthicsApplication { get; set; }
        public Chief? Chief { get; set; }
    }
}
        
