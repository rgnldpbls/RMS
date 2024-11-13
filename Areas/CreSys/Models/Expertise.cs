using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class Expertise
    {
        [Key]
        public int expertiseId { get; set; }
        [Required]
        public string expertiseName { get; set; }

        //navigation property
        public ICollection<EthicsEvaluatorExpertise> EthicsEvaluatorExpertise { get; set; } = new List<EthicsEvaluatorExpertise>();
    }
}
