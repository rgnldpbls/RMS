using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class Expertise
    {
        [Key]
        public int ExpertiseId { get; set; }
        public string ExpertiseName { get; set; }

        public ICollection<EthicsEvaluatorExpertise> EthicsEvaluatorExpertise { get; set; }
    }
}
