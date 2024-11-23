using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsReport
    {
        [Key]
        public string EthicsReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportFileType { get; set; }
        public byte[] ReportFile { get; set; }
        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }
        public string College { get; set; }
        public DateTime DateGenerated { get; set; }
        public string? UserId { get; set; }
        public bool IsArchived { get; set; }
    }
}
