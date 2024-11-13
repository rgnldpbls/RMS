using CRE.Models;
using CRE.ViewModels;

namespace CRE.Interfaces
{
    public interface IChairpersonServices
    {
        Task<List<EthicsApplication>> GetApplicationsByFieldOfStudyAsync(string userId);
        Task AssignEvaluatorsAsync(string urecNo, List<int> evaluatorIds);
        Task<EthicsApplication> GetApplicationAsync(string urecNo);
        Task<Dictionary<string, List<EthicsEvaluator>>> GetEvaluatorNamesAsync(IEnumerable<EthicsApplication> ethicsApplications);
        Task<Dictionary<string, List<string>>> GetApplicationEvaluatorNamesAsync(IEnumerable<EthicsApplication> ethicsApplications);
        Task<IEnumerable<EthicsApplication>> GetUnassignedApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications);
        Task<IEnumerable<EthicsApplication>> GetUnderEvaluationApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications);
        Task<IEnumerable<EthicsApplication>> GetEvaluationResultApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications);
    }
}
