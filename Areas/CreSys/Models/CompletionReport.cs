using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class CompletionReport
    {
        [Key]
        public int CompletionReportId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string? UrecNo { get; set; }
        public DateTime SubmissionDate { get; set; }
        public byte[] TerminalReport { get; set; }
        public DateTime ResearchStartDate { get; set; }
        public DateTime? ResearchEndDate { get; set; } //issuenace date of certificate

        public EthicsApplication EthicsApplication { get; set; }

    }
}
