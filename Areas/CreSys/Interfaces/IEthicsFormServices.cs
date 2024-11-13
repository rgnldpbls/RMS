using CRE.Models;

namespace CRE.Interfaces
{
    public interface IEthicsFormServices
    {
        Task<IEnumerable<EthicsForm>> GetAllFormsAsync(); // To display all forms for download
        Task<EthicsForm> GetEthicsFormByIdAsync(string ethicsFormId); // To get the form for editing
        Task CreateEthicsFormAsync(EthicsForm form); // Add new form if a new memo is implemented
        Task EditEthicsFormAsync(EthicsForm form); // Edit the form name or template files
        Task DeleteEthicsFormAsync(string ethicsFormId); // Remove outdated or unnecessary forms
    }
}
