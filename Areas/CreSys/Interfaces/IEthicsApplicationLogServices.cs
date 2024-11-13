using CRE.Models;

namespace CRE.Interfaces
{
    public interface IEthicsApplicationLogServices
    {
        Task AddLogAsync(EthicsApplicationLog log);
        Task<EthicsApplicationLog> GetLatestLogAsync(string urecNo); // Retrieve the latest log by date for a specific application
        Task<List<EthicsApplicationLog>> GetLogsByUrecNoAsync(string urecNo); // Retrieve all logs for a specific application (urecNo) for tracking
        Task<IEnumerable<EthicsApplicationLog>> GetLatestLogsByApplicationIdsAsync(IEnumerable<string> urecNos); //retrieve the logs of all of the application sent
        Task<string> GetLatestCommentByUrecNoAsync(string urecNo);
    }
}
                