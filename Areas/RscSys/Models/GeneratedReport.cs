using System.ComponentModel.DataAnnotations;

namespace rscSys_final.Models
{
    public class GeneratedReport
    {
        [Key]
        public int ReportId { get; set; }
        public string ReportName { get; set; }         // Name of the report (e.g., "Approved Requests Report")
        public byte[] FileData { get; set; }
        public string FileType { get; set; }           // File type (e.g., "PDF", "Excel")
        public DateTime GeneratedDate { get; set; }    // Date and time when the report was generated
        public string GeneratedBy { get; set; }        // User who generated the report (UserId or name)
    }
}
