using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class EthicsApplicationFormsServices : IEthicsApplicationFormsServices   
    {
        private readonly CreDbContext _context;
        public EthicsApplicationFormsServices(CreDbContext context)
        {
            _context = context;
        }

        public async Task AddFormAsync(EthicsApplicationForms form)
        {
            _context.EthicsApplicationForms.Add(form);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EthicsApplicationForms>> GetAllFormsByUrecAsync(string urecNo)
        {
            return await _context.EthicsApplicationForms
                                 .Where(f => f.urecNo == urecNo)
                                 .ToListAsync();
        }

        public async Task<EthicsApplicationForms> GetApplicationFormByIdAsync(int ethicsApplicationFormId)
        {
            return await _context.EthicsApplicationForms
                                 .FirstOrDefaultAsync(f => f.ethicsApplicationFormId == ethicsApplicationFormId);
        }

        public async Task<EthicsForm> GetFormByIdAsync(string ethicsFormId)
        {
            return await _context.EthicsForm
                                 .FirstOrDefaultAsync(f => f.ethicsFormId == ethicsFormId);
        }

        public async Task RemoveFormAsync(int ethicsApplicationFormId)
        {
            var form = await _context.EthicsApplicationForms
                                     .FirstOrDefaultAsync(f => f.ethicsApplicationFormId == ethicsApplicationFormId);

            if (form != null)
            {
                _context.EthicsApplicationForms.Remove(form);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateFormAsync(EthicsApplicationForms form)
        {
            _context.EthicsApplicationForms.Update(form);
            await _context.SaveChangesAsync();
        }
        public async Task<EthicsApplicationForms> GetFormByIdAndUrecNoAsync(string formId, string urecNo)
        {
            return await _context.EthicsApplicationForms
                .FirstOrDefaultAsync(f => f.ethicsFormId == formId && f.urecNo == urecNo);
        }

        public async Task<bool> SaveEthicsFormAsync(EthicsApplicationForms form)
        {
            try
            {
                _context.EthicsApplicationForms.Add(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                // Handle or log the exception as needed
                return false;
            }
        }

        public async Task<EthicsApplicationForms> GetForm15ByUrecNoAsync(string urecNo)
        {
            return await _context.EthicsApplicationForms
          .FirstOrDefaultAsync(form => form.urecNo == urecNo && form.ethicsFormId == "FORM15"); // Adjust condition as necessary
        }
    }
}
