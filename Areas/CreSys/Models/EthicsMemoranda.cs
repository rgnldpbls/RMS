using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsMemoranda
    {
        [Key]
        public int MemoId { get; set; }
        public string MemoNumber { get; set; }
        public string? MemoName { get; set; }
        public string MemoDescription { get; set; }
        public byte[] MemoFile { get; set; }
    }
}
