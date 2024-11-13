using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Models
{
    public class AddUtilization
    {
        [Key]
        public string UtilizationId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(AddAccomplishment))]
        [Display(Name = "ProductionNo")]
        public virtual string? ProductionId { get; set; }

        public AddAccomplishment? AddAccomplishment { get; set; }

        public byte[]? CertificateofUtilizationData { get; set; }
        public string? CertificateofUtilizationFileName { get; set; }

    }
}

