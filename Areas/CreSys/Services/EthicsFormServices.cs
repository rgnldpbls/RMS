using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class EthicsFormServices : IEthicsFormServices
    {
        private readonly CreDbContext _context;
        public EthicsFormServices(CreDbContext context)
        {
            _context = context;
        }

        Task IEthicsFormServices.CreateEthicsFormAsync(EthicsForm form)
        {
            throw new NotImplementedException();
        }

        Task IEthicsFormServices.DeleteEthicsFormAsync(string ethicsFormId)
        {
            throw new NotImplementedException();
        }

        Task IEthicsFormServices.EditEthicsFormAsync(EthicsForm form)
        {
            throw new NotImplementedException();
        }

        async Task<IEnumerable<EthicsForm>> IEthicsFormServices.GetAllFormsAsync()
        {
            return await _context.EthicsForm.ToListAsync();
        }

        async Task<EthicsForm> IEthicsFormServices.GetEthicsFormByIdAsync(string ethicsFormId)
        {
            // Retrieve the form from the database using the provided id
            return await _context.EthicsForm
                .FirstOrDefaultAsync(form => form.ethicsFormId == ethicsFormId);
        }
    }
}
