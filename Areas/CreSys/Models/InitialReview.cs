using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class InitialReview
    {
        [Key]
        public int InitalReviewId { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string UrecNo { get; set; }
        public DateTime DateReviewed { get; set; }
        public string Status { get; set; }
        public string Feedback { get; set; }
        public string ReviewType { get; set; }


        public EthicsApplication EthicsApplication { get; set; }
    }
}
