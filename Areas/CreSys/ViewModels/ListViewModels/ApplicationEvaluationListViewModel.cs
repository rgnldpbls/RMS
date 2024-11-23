namespace ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels
{
    public class ApplicationEvaluationListViewModel
    {
        public List<ChiefEvaluationViewModel> ExemptApplications { get; set; }
        public List<EvaluatedExemptApplication> EvaluatedExemptApplications { get; set; }
        public List<EvaluatedExpeditedApplication> EvaluatedExpeditedApplications { get; set; }
        public List<EvaluatedFullReviewApplication> EvaluatedFullReviewApplications { get; set; }
        public List<PendingIssuanceViewModel> PendingIssuance { get; set; } // New property

        public ApplicationEvaluationListViewModel()
        {
            ExemptApplications = new List<ChiefEvaluationViewModel>();
            EvaluatedExemptApplications = new List<EvaluatedExemptApplication>();
            EvaluatedExpeditedApplications = new List<EvaluatedExpeditedApplication>();
            EvaluatedFullReviewApplications = new List<EvaluatedFullReviewApplication>();
            PendingIssuance = new List<PendingIssuanceViewModel>(); // Initialize the new property
        }
    }
}
