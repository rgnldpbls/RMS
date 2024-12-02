using ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels.FormClasses
{
    public class EvaluationSheetsViewModel
    {
        public InformedConsentFormViewModel InformedConsentForm { get; set; }
        public ProtocolReviewFormViewModel ProtocolReviewForm { get; set; }

        public EvaluationSheetsViewModel()
        {
            InformedConsentForm = new InformedConsentFormViewModel();
            ProtocolReviewForm = new ProtocolReviewFormViewModel();
        }
    }
}
