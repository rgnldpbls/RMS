using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRE.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string? UserId { get; set; }
        public string? NotificationTitle { get; set; }
        public string NotificationMessage { get; set; }
        public DateTime NotificationCreationDate { get; set; } = DateTime.Now;
        public bool NotificationStatus { get; set; } = false;
        public string? Role { get; set; }
        public string? PerformedBy { get; set; } = "System";
    }
}
