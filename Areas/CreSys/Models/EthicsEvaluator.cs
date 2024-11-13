using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class EthicsEvaluator
    {
        [Key]
        public int ethicsEvaluatorId { get; set; }
        [ForeignKey(nameof(Faculty))]
        public int facultyId { get; set; }
        [Required]
        public int completedEval { get; set; }
        [Required]
        public int pendingEval { get; set; }
        [Required]
        public int declinedAssignment { get; set; }
        [Required]
        public string accountStatus { get; set; } = "Active";


        //navigation property
        public Faculty Faculty { get; set; }
        public ICollection<EthicsEvaluatorExpertise> EthicsEvaluatorExpertise { get; set; } = new List<EthicsEvaluatorExpertise>();  
        public ICollection<EthicsEvaluation> EthicsEvaluation { get; set; } = new List<EthicsEvaluation>();
    }
}
