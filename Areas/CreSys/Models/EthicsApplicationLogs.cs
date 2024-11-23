using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsApplicationLogs
    {
        [Key]
        public int LogId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string UrecNo { get; set; }
        public string UserId { get; set; }
        public string? Name { get; set; }
        public string Status { get; set; }
        public DateTime ChangeDate { get; set; }
        public string? Comments { get; set; }

        public EthicsApplication EthicsApplication { get; set; }
    }
}
