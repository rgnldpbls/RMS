namespace CRE.ViewModels
{
    public class ApplicationListViewModel
    {
        public List<ApplicationViewModel> ApplicationsApprovedForEvaluation { get; set; } // Leftmost tab
        public List<ApplicationViewModel> ExemptApplications { get; set; }
        public List<ApplicationViewModel> ExpeditedApplications { get; set; }
        public List<ApplicationViewModel> FullReviewApplications { get; set; }
        public List<PendingIssuance> PendingIssuance { get; set; }
        public List<ApplicationViewModel> AllApplications { get; set; } // Rightmost tab
    }
}
