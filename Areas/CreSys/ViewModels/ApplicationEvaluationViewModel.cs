using CRE.Models;
using System.Collections.Generic;

namespace CRE.ViewModels
{
    public class ApplicationEvaluationViewModel
    {
        public List<ChiefEvaluationViewModel> ExemptApplications { get; set; }
        public List<EvaluatedExemptApplication> EvaluatedExemptApplications { get; set; }
        public List<EvaluatedExpeditedApplication> EvaluatedExpeditedApplications { get; set; }
        public List<EvaluatedFullReviewApplication> EvaluatedFullReviewApplications { get; set; }
        public List<PendingIssuance> PendingIssuance { get; set; } // New property
        
        public ApplicationEvaluationViewModel()
        {
            ExemptApplications = new List<ChiefEvaluationViewModel>();
            EvaluatedExemptApplications = new List<EvaluatedExemptApplication>();
            EvaluatedExpeditedApplications = new List<EvaluatedExpeditedApplication>();
            EvaluatedFullReviewApplications = new List<EvaluatedFullReviewApplication>();
            PendingIssuance = new List<PendingIssuance>(); // Initialize the new property
        }
    }
}
