namespace CRE.ViewModels
{
    public class FilteredApplicationsViewModel
    {
        public List<ApplicationViewModel> ApplicationsApprovedForEvaluation { get; set; }
        public List<ApplicationViewModel> Evaluations { get; set; }
        public List<ApplicationViewModel> ExemptApplications { get; set; }
        public List<ApplicationViewModel> AllApplications { get; set; }
    }
}
