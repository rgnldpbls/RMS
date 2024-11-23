using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Models
{
    public class AddFAQs
    {
        [Key]
        public string FAQ_id { get; set; }

        [ForeignKey(nameof(superadmin))]
        [Display(Name = "Added by:")]
        public string? added_by { get; set; }

        public ApplicationUser? superadmin { get; set; }

        [Display(Name = "Question:")]
        public string? question_id { get; set; }
        [Display(Name = "Answer:")]
        public string? answer_id { get; set; }

    }
    public class AddReview
    {
        [Key]
        public string review_id { get; set; }

        [ForeignKey(nameof(user_name_id))]
        [Display(Name = "User Name")]
        public string? username_id { get; set; }

        public ApplicationUser? user_name_id { get; set; }

        public int? rating { get; set; }
        public string? comment { get; set; }
        public DateTime? date_posted { get; set; }
        public string? response { get; set; }
        public DateTime? response_date { get; set; }


    }
}
