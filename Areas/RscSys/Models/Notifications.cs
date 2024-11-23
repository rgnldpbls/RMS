using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace rscSys_final.Models
{
    public class Notifications
    {
        [Key]
        public int NotificationId { get; set; }
        public string? UserId { get; set; }
        public string? NotificationTitle { get; set; }
        public string NotificationMessage { get; set; }
        public DateTime NotificationCreationDate { get; set; } = DateTime.Now;
        public bool NotificationStatus { get; set; } = false;  // False means unread, true means read
        public string? Role { get; set; }
        public string? PerformedBy { get; set; } = "System";
        public int? EvaluatorAssignmentId { get; set; } // Make it nullable if not always set
        public virtual EvaluatorAssignment? EvaluatorAssignment { get; set; }
    }
}
