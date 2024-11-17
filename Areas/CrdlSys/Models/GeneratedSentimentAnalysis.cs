using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrdlSys.Models
{
    [Table("CRDL_GeneratedSentimentAnalysis")]

    public class GeneratedSentimentAnalysis
    {
        [Key]
        public string SentimentAnalysisId { get; set; } = GenerateSentimentAnalysisId();

        [Required]
        public string FileName { get; set; }

        [Required]
        public byte[] GenerateReportFile { get; set; }

        [Required]
        public byte[] SurveyFile { get; set; }

        [ForeignKey("ResearchEvent")]
        public string ResearchEventId { get; set; }

        [Required]
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public bool IsArchived { get; set; } = false;
        public ResearchEvent? ResearchEvent { get; set; }

        public static string GenerateSentimentAnalysisId()
        {
            var year = DateTime.Now.Year;
            var random4Digits = new Random().Next(1000, 9999);
            var random2Digits = new Random().Next(10, 99);
            return $"{year}-GENSENTIMENTANALYSIS-{random4Digits}-{random2Digits}";
        }
    }
}
