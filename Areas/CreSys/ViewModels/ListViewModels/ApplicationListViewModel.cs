namespace ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels
{
    public class ApplicationListViewModel
    {
        public List<ApplicationViewModel> ApplicationsApprovedForEvaluation { get; set; } // Leftmost tab
        public List<ApplicationViewModel> ExemptApplications { get; set; }
        public List<ApplicationViewModel> ExpeditedApplications { get; set; }
        public List<ApplicationViewModel> FullReviewApplications { get; set; }
        public List<PendingIssuanceViewModel>? PendingIssuance { get; set; }
        public List<ApplicationViewModel> AllApplications { get; set; } // Rightmost tab
    }
}
