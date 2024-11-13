using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class EthicsApplicationForms
    {
        [Key]
        public int ethicsApplicationFormId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        public string? ethicsFormId { get; set; }
        [Required]
        public DateOnly dateUploaded { get; set; }
        [Required]
        public string fileName { get; set; }
        [Required]
        public byte[] file { get; set; }


        //navigation properties
        public EthicsApplication EthicsApplication { get; set; }
    }
}
