using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class CoProponent
    {
        [Key]
        public int CoProponentId { get; set; }
        [ForeignKey(nameof(NonFundedResearchInfo))]
        public string NonFundedResearchId { get; set; }
        public string CoProponentName { get; set; }

        public NonFundedResearchInfo NonFundedResearchInfo { get; set; }
    }
}
