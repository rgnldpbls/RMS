using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class EthicsForm
    {
        [Key, MaxLength(10)]
        public string ethicsFormId { get; set; }
        [Required, MaxLength(100)]
        public string formName { get; set; }
        public string? formDescription { get; set; }
        [Required]
        public byte[] file { get; set; }
    }
}
