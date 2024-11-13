using CRE.Models;
using CRE.ViewModels;

namespace CRE.Interfaces
{
    public interface IEthicsEvaluationServices
    {
        Task<bool> AreAllEvaluationsEvaluatedAsync(string urecNo);
        EthicsEvaluation GetEvaluationByUrecNo(string urecNo);
        Task<List<EthicsEvaluator>> GetAvailableEvaluatorsAsync(string fieldOfStudy);
        Task UpdateEvaluationStatusAsync(int evaluationId, string status, string? reasonForDecline, int ethicsEvaluatorId);
        Task<int> CreateEvaluationAsync(EthicsEvaluation evaluation);
        Task<List<PendingIssuance>> GetPendingApplicationsForIssuanceAsync();
        Task<List<EthicsEvaluator>> GetAllEvaluatorsAsync();
        Task AssignEvaluatorAsync(string urecNo, int evaluatorId);
        Task<AssignEvaluatorsViewModel> GetApplicationDetailsForEvaluationAsync(string urecNo);
        Task<EvaluatedExemptApplication> GetEvaluationDetailsAsync(string urecNo, int evaluationId);
        Task SaveEvaluationAsync(EthicsEvaluation ethicsEvaluation);
        Task<EthicsEvaluation> GetEvaluationByUrecNoAndIdAsync(string urecNo, int evaluationId);
        Task<EthicsEvaluation> GetEvaluationByUrecNoAndEvaluatorIdAsync(string urecNo, int ethicsEvaluatorId);
        Task<List<string>> GetEvaluatedUrecNosAsync();
        Task<EvaluationDetailsViewModel> GetEvaluationDetailsWithUrecNoAsync(string urecNo, int evaluationId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetAssignedEvaluationsAsync(int evaluatorId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetAcceptedEvaluationsAsync(int evaluatorId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetCompletedEvaluationsAsync(int evaluatorId);
        Task<EthicsEvaluator> GetEvaluatorByUserIdAsync(string userId);
        Task UpdateEvaluationAsync(EthicsEvaluation ethicsEvaluation);
        Task<EvaluationDetailsViewModel> GetEvaluationAndEvaluatorDetailsAsync(string urecNo, int evaluationId);
        Task<IEnumerable<EthicsEvaluator>> GetAvailableEvaluatorsAsync(IEnumerable<EthicsEvaluator> allEvaluators, string applicantName);
        Task<IEnumerable<EthicsEvaluator>> GetRecommendedEvaluatorsAsync(IEnumerable<EthicsEvaluator> allEvaluators, string requiredFieldOfStudy, string applicantName);
        Task<List<ChiefEvaluationViewModel>> GetExemptApplicationsAsync();
        Task<List<EvaluatedExemptApplication>> GetEvaluatedExemptApplicationsAsync();
        Task<List<EvaluatedExpeditedApplication>> GetEvaluatedExpeditedApplicationsAsync();
        Task<List<EvaluatedFullReviewApplication>> GetEvaluatedFullReviewApplicationsAsync();
        Task IncrementDeclinedAssignmentCountAsync(int ethicsEvaluatorId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetDeclinedEvaluationsAsync(int evaluatorId);
        Task<List<EthicsEvaluator>> GetPendingEvaluatorsAsync(string urecNo);
        Task<List<EthicsEvaluator>> GetAcceptedEvaluatorsAsync(string urecNo);
        Task<List<EthicsEvaluator>> GetDeclinedEvaluatorsAsync(string urecNo);
        Task AddEvaluationAsync(EthicsEvaluation evaluation);
    }
}
