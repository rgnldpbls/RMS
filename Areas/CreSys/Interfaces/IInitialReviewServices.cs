using CRE.Models;
using CRE.ViewModels;

namespace CRE.Interfaces
{
    public interface IInitialReviewServices
    {
        Task<IEnumerable<InitialReviewViewModel>> GetEthicsApplicationsForInitialReviewAsync();
        Task<InitialReviewViewModel> GetApplicationDetailsAsync(string urecNo);
        Task<IEnumerable<CoProponent>> GetCoProponentsByNonFundedResearchIdAsync(string nonFundedResearchId);
        Task<InitialReview> GetInitialReviewByUrecNoAsync(string urecNo);
        Task UpdateInitialReviewAsync(InitialReview initialReview);
        Task<EvaluationDetailsViewModel> GetApplicationDetailsAsync(string urecNo, int evaluationId);
        Task ApproveApplicationAsync(string urecNo, string comments, string userId);
        Task ReturnApplicationAsync(string urecNo, string comments, string userId);
        Task<EthicsEvaluation> GetEthicsEvaluationAsync(string urecNo);
        Task<IEnumerable<InitialReviewViewModel>> GetPendingApplicationsAsync();
        Task<IEnumerable<InitialReviewViewModel>> GetApprovedApplicationsAsync();
        Task<IEnumerable<InitialReviewViewModel>> GetReturnedApplicationsAsync();
        Task<IEnumerable<EthicsApplication>> GetApprovedEthicsApplicationsAsync();
        Task<IEnumerable<ChiefEvaluationViewModel>> GetExemptApplicationsAsync();
    }
}
