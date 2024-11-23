using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrdlSys.Models
{
    [Table("CRDL_GeneratedReport")]

    public class GeneratedReport
    {
        [Key]
        public string ReportId { get; set; } = GenerateReportId();

        [Required]
        public string FileName { get; set; }

        public string TypeOfReport { get; set; }

        [Required]
        public byte[] GenerateReportFile { get; set; }

        public int? Year { get; set; }

        [ForeignKey("ResearchEvent")]
        public string? ResearchEventId { get; set; }

        [Required]
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public ResearchEvent? ResearchEvent { get; set; }

        public bool IsArchived { get; set; } = false;

        public static string GenerateReportId()
        {
            var year = DateTime.Now.Year;
            var random4Digits = new Random().Next(1000, 9999);
            var random2Digits = new Random().Next(10, 99);
            return $"{year}-GENREPORT-{random4Digits}-{random2Digits}";
        }
    }
}
