using CRE.Models;

namespace CRE.ViewModels
{
    public class ApplicationEvaluationsViewModel
    {
        public string UrecNo { get; set; }
        public InitialReview InitialReview { get; set; }
        public IEnumerable<EthicsEvaluation> EthicsEvaluations { get; set; } = new List<EthicsEvaluation>();

        // Chief's name property for easier access in the view
        public string? ChiefName => InitialReview?.ReviewType == "Exempt"
            ? EthicsEvaluations.FirstOrDefault()?.Chief?.Name
            : null;
    }
}
