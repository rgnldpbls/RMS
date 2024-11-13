using System.ComponentModel.DataAnnotations.Schema;

namespace CRE.Models
{
    public class EthicsEvaluatorExpertise
    {

        [ForeignKey(nameof(EthicsEvaluator))]
        public int ethicsEvaluatorId { get; set; }
        [ForeignKey(nameof(Expertise))]
        public int expertiseId { get; set; }

        //navigation properties
        public EthicsEvaluator EthicsEvaluator { get; set; }

        public Expertise Expertise { get; set; }
    }
}
