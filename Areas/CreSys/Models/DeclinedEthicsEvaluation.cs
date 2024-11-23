using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class DeclinedEthicsEvaluation
    {
        [Key]
        public int DeclinedEvaluationId { get; set; }
        public int EvaluationId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string UrecNo { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string? ReasonForDeclining { get; set; }
        public DateOnly DeclineDate { get; set; }
        [ForeignKey(nameof(EthicsEvaluator))]
        public int EthicsEvaluatorId { get; set; }
             
        public EthicsApplication EthicsApplication { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }
    }
}
