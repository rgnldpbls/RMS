using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class Memorandum
    {
        [Key] 
        public int memoId { get; set; }
        [Required]
        public string memoNumber { get; set; }
        public string? memoName { get; set; }
        [Required]
        public string memoDescription { get; set; }
        [Required]
        public byte[] memoFile { get; set; }
    }
}
