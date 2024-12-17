namespace ResearchManagementSystem.Models
{
    public class UserActivityLog
    {
        public string Id { get; set; }
        public string? UserId { get; set; }
        public string? Activity { get; set; } // e.g., "Login", "Logout"
        public DateTime? Timestamp { get; set; }
    }


    public class UserActivityViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastLogout { get; set; }
        public List<UserActivityLog> Activities { get; set; }
    }

    public class EditFacultyRankViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Rank { get; set; }
        public DateTime? RankStartDate { get; set; }
        public DateTime? RankEndDate { get; set; }

        // Additional property to display duration before updating
        public string RankDuration { get; set; }
    }



}
