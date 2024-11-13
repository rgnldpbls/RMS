using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class InitialReview
    {
        [Key]
        public int initalReviewId { get; set; }
        public string userId { get; set; }

        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        public DateOnly? dateReviewed { get; set; }
        [Required]
        public string status { get; set; }
        public string feedback { get; set; }
        [Required]
        public string ReviewType { get; set; }
        //navigation properties
        public EthicsApplication EthicsApplication { get; set; }
    }
}
