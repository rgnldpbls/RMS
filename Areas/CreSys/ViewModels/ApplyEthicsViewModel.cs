using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class ApplyEthicsViewModel
    {
        public ApplicationUser User { get; set; }
        public string FullName { get; set; }
        public EthicsApplication? EthicsApplication { get; set; }
        public NonFundedResearchInfo? NonFundedResearchInfo { get; set; }
        public ReceiptInfo? ReceiptInfo { get; set; } = null; //will be initialized once the user is determined as external
        public IEnumerable<EthicsApplicationLogs>? EthicsApplicationLogs { get; set; } = new List<EthicsApplicationLogs>();
        public List<CoProponent>? CoProponent { get; set; } = new List<CoProponent>();//multiple researchers

        // New property for FieldOfStudies
        public List<string> FieldOfStudies { get; set; } = new List<string>();

        public List<string> Colleges { get; set; } = new List<string>();
        public List<string> Universities { get; set; } = new List<string>();
        public bool IsExternalResearcher { get; set; }
        //other properties needed
        [Required(ErrorMessage = "Payment Receipt is Required.")]
        [Display(Name = "Upload Payment Receipt")]
        public IFormFile receiptFile { get; set; } //placeholder for conversion
    }
}
