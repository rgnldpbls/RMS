using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRE.Models
{
    public class EthicsEvaluationDeclined
    {
        [Key]
        public int id { get; set; } // Primary key
        public int evaluationId { get; set; } // Foreign key to the original evaluation
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        [ForeignKey(nameof(EthicsEvaluator))]
        public int ethicsEvaluatorId { get; set; }
        public string? reasonForDecline { get; set; } // Reason for decline
        public DateOnly declineDate { get; set; } // Date when it was declined


        //nav properties
        public EthicsApplication EthicsApplication { get; set; }
        public EthicsEvaluator EthicsEvaluator { get; set; }    
    }
}
