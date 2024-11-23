using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class EthicsForms
    {
        [Key]
        public string EthicsFormId { get; set; }
        public string FormName { get; set; }
        public string? FormDescription { get; set; }
        public DateTime DateCreated { get; set; }
        public byte[] EthicsFormFile { get; set; }
        public bool IsRequired { get; set; } = true;

        public ICollection<EthicsApplicationForms> EthicsApplicationForms { get; set; }
    }
}
