namespace CRE.ViewModels
{
    public class InitialReviewListViewModel
    {
        public IEnumerable<InitialReviewViewModel> PendingApplications { get; set; }
        public IEnumerable<InitialReviewViewModel> ApprovedApplications { get; set; }
        public IEnumerable<InitialReviewViewModel> ReturnedApplications { get; set; }
    }
}
