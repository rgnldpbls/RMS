using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class Chairperson
    {
        [Key]
        public int ChairpersonId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string FieldOfStudy { get; set; }
    }
}
