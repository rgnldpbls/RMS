using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class Chief
    {
        [Key]
        public int chiefId { get; set; }
        public string Name {  get; set; }
        public string userId { get; set; }
        [Required]
        public string center { get; set; }



        //navigation properties
        public ICollection<InitialReview> InitialReview { get; set; } = new List<InitialReview>();
        public ICollection<EthicsReport> EthicsReport { get; set; } = new List<EthicsReport>();
        public EthicsEvaluation Evaluation{ get; set; }
    }
}
