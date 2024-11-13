using CRE.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRE.Interfaces
{
    public interface IEthicsClearanceServices
    {
        Task<bool> IssueEthicsClearanceAsync(EthicsClearance ethicsClearance, IFormFile uploadedFile, string remarks, string userId);
        Task<EthicsClearance> GetClearanceByUrecNoAsync(string urecNo);
        Task<bool> HandleRevisionsAsync(string urecNo, string applicationDecision, string remarks, string userId);
    }
}
