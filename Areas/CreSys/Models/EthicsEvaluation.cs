using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsEvaluation
    {
        [Key]
        public int EvaluationId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string? UrecNo { get; set; }
        public string? UserId { get; set; }
        public int? EthicsEvaluatorId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EvaluationStatus { get; set; }
        public string? ProtocolRecommendation { get; set; }
        public string? ProtocolRemarks { get; set; }
        public byte[]? ProtocolReviewSheet { get; set; }

        public string? ConsentRecommendation { get; set; }
        public string? ConsentRemarks { get; set; }
        public byte[]? InformedConsentForm { get; set; }

        public EthicsApplication EthicsApplication { get; set; }
        public virtual EthicsEvaluator? EthicsEvaluator { get; set; }

    }
}
