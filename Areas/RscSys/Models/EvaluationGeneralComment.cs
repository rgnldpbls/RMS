using System.ComponentModel.DataAnnotations;

namespace rscSys_final.Models
{
    public class EvaluationGeneralComment
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int EvaluatorAssignmentId { get; set; }

        public virtual EvaluatorAssignment EvaluatorAssignment { get; set; } // Navigation property
    }
}
