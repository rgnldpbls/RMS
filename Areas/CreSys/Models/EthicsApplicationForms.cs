using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsApplicationForms
    {
        [Key]
        public int EthicsApplicationFormId { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string UrecNo { get; set; }
        [ForeignKey(nameof(EthicsForms))]
        public string? EthicsFormId { get; set; }
        public DateTime DateUploaded { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }

        public EthicsApplication EthicsApplication { get; set; }
        public ICollection<EthicsForms> EthicsForms { get; set; }
    }
}
