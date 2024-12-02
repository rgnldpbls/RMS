using Microsoft.AspNetCore.Identity;
using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.CreSys.Interfaces
{
    public interface IAllServices
    {
        Task<bool> SubmitTerminalReportAsync(string urecNo, IFormFile terminalReportFile, DateTime researchStartDate, DateTime researchEndDate);
        Task<CompletionReport> GetCompletionReportByUrecNoAsync(string urecNo);
        Task<byte[]> GetTerminalReportAsync(string urecNo);
        Task<IEnumerable<CompletionReportViewModel>> GetCompletionReportsAsync();
        Task<EthicsClearance> GetClearanceByUrecNoAsync(string urecNo);
        Task<List<ResearchReportModel>> GetFilteredResearchData(ReportGenerationViewModel model);
        byte[] GenerateExcelFile(List<ResearchReportModel> researchData, DateTime? startDate, DateTime? endDate, out string fileName);

        Task<List<EthicsApplication>> GetApplicationsByFieldOfStudyAsync(string userId);
        Task<IEnumerable<EthicsApplication>> GetUnderEvaluationApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications);
        Task<Dictionary<string, List<EthicsEvaluator>>> GetEvaluatorNamesAsync(IEnumerable<EthicsApplication> ethicsApplications);  
        Task<IEnumerable<EthicsApplication>> GetUnassignedApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications);
        Task<IEnumerable<EthicsApplication>> GetEvaluationResultApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications);
        Task<List<NonFundedResearchInfo>> GetNonFundedResearchInfosAsync(List<string> urecNos);
        Task<List<ChiefEvaluationViewModel>> GetExemptApplicationsAsync();
        Task<List<EvaluatedExemptApplication>> GetEvaluatedExemptApplicationsAsync();
        Task<List<EvaluatedExpeditedApplication>> GetEvaluatedExpeditedApplicationsAsync();
        Task<List<EvaluatedFullReviewApplication>> GetEvaluatedFullReviewApplicationsAsync();
        Task<List<PendingIssuanceViewModel>> GetPendingApplicationsForIssuanceAsync();
        Task<AssignEvaluatorsViewModel> GetApplicationDetailsForEvaluationAsync(string urecNo);
        Task<List<EthicsEvaluator>> GetPendingEvaluatorsAsync(string urecNo);
        Task<List<EthicsEvaluator>> GetAcceptedEvaluatorsAsync(string urecNo);
        Task<List<EthicsEvaluator>> GetDeclinedEvaluatorsAsync(string urecNo);
        Task<List<EthicsEvaluator>> GetEvaluatedEvaluatorsAsync(string urecNo);
        Task<List<EthicsEvaluator>> GetAllEvaluatorsAsync();
        Task<IEnumerable<EthicsEvaluator>> GetRecommendedEvaluatorsAsync(
             IEnumerable<EthicsEvaluator> allEvaluators,
             string requiredFieldOfStudy,
             string applicantUserId,
             UserManager<ApplicationUser> userManager);
        Task AssignEvaluatorAsync(string urecNo, int evaluatorId, string fullName);
        Task<EvaluationDetailsViewModel> GetEvaluationDetailsWithUrecNoAsync(string urecNo, int evaluationId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetAssignedEvaluationsAsync(int evaluatorId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetAcceptedEvaluationsAsync(int evaluatorId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetCompletedEvaluationsAsync(int evaluatorId);
        Task<IEnumerable<AssignedEvaluationViewModel>> GetDeclinedEvaluationsAsync(int evaluatorId);
        Task<EthicsEvaluation> GetEvaluationByUrecNoAndEvaluatorIdAsync(string urecNo, int ethicsEvaluatorId);
        Task UpdateApplicationStatusAsync(int evaluationId, string urecNo, string status);
        Task UpdateEvaluationStatusAsync(int evaluationId, string status, string? reasonForDecline, string userId);
        Task<InitialReviewViewModel> GetApplicationDetailsAsync(string urecNo);
        Task<bool> AreAllEvaluationsEvaluatedAsync(string urecNo);
        Task<EvaluationDetailsViewModel> GetEvaluationAndEvaluatorDetailsAsync(string urecNo, int evaluationId);
        Task<List<EthicsEvaluator>> GetAssignedEvaluatorsAsync(string urecNo);
        Task AddLogAsync(EthicsApplicationLogs log);
        Task<bool> SaveEthicsFormAsync(EthicsApplicationForms form);

    }
}
