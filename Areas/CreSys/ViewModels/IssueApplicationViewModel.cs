using CRE.Models;

namespace CRE.ViewModels
{
    public class IssueApplicationViewModel
    {
        public EthicsApplication EthicsApplication { get; set; } 
        public EthicsClearance EthicsClearance { get; set; }
        public IEnumerable<EthicsEvaluation> EthicsEvaluations { get; set; } 
        public IEnumerable<EthicsApplicationLog> ApplicationLogs { get; set; } 
        public IFormFile UploadedFile { get; set; }
        public IssueApplicationViewModel()
        {
            EthicsEvaluations = new List<EthicsEvaluation>();
            ApplicationLogs = new List<EthicsApplicationLog>();
        }
    }
}
