namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public int userId { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationMessage { get; set; }
        public DateTime NotificationCreationDate { get; set; }
        public bool NotificationStatus { get; set; }
    }
}
