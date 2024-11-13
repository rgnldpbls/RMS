using CRE.Models;

namespace CRE.Interfaces
{
    public interface IEthicsApplicationFormsServices
    {
        Task<IEnumerable<EthicsApplicationForms>> GetAllFormsByUrecAsync(string urecNo); 
        Task<EthicsForm> GetFormByIdAsync(string ethicsFormId);
        Task<EthicsApplicationForms> GetApplicationFormByIdAsync(int ethicsApplicationFormId);
        Task<EthicsApplicationForms> GetFormByIdAndUrecNoAsync(string formId, string urecNo);
        Task AddFormAsync(EthicsApplicationForms form);
        Task UpdateFormAsync(EthicsApplicationForms form);
        Task RemoveFormAsync(int ethicsApplicationFormId);
        Task<bool> SaveEthicsFormAsync(EthicsApplicationForms form);
        Task<EthicsApplicationForms> GetForm15ByUrecNoAsync(string urecNo);
    }
}
