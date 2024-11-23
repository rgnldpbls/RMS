using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsEvaluator
    {
        [Key]
        public int EthicsEvaluatorId { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Declined { get; set; }
        public string AccountStatus { get; set; }

        public ICollection<EthicsEvaluation> EthicsEvaluation { get; set; }
        public ICollection<EthicsEvaluatorExpertise> EthicsEvaluatorExpertises { get; set; }
        public ICollection<DeclinedEthicsEvaluation> DeclinedEthicsEvaluation { get; set; }
    }
}
