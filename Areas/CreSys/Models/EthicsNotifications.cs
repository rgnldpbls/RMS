using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsNotifications
    {
        [Key]
        public int NotificationId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string UrecNo { get; set; }
        public string? UserId { get; set; }
        public string? NotificationTitle { get; set; }
        public string NotificationMessage { get; set; }
        public DateTime NotificationCreationDate { get; set; } = DateTime.Now;
        public bool NotificationStatus { get; set; } = false;
        public string? Role { get; set; }
        public string? PerformedBy { get; set; } = "System";

        public EthicsApplication EthicsApplication { get; set; }
    }
}
