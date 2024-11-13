using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class CompletionReport
    {
        [Key]
        public int completionReportId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string? urecNo { get; set; }
        [Required]
        public DateOnly submissionDate { get; set; }
        [Required]
        public byte[] terminalReport { get; set; }
        [Required]
        public DateOnly researchStartDate { get; set; }
        public DateOnly? researchEndDate { get; set; } //issuenace date of certificaate


        //navigation properties
        public EthicsApplication EthicsApplication { get; set; }
    }
}
