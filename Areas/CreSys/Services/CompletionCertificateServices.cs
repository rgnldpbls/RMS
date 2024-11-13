using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class CompletionCertificateServices : ICompletionCertificateServices
    {
        private readonly CreDbContext _context;
        public CompletionCertificateServices(CreDbContext context)
        {
            _context = context;
        }

        public async Task<CompletionCertificate?> GetCompletionCertificateByUrecNoAsync(string urecNo)
        {
            return await _context.CompletionCertificate
                .FirstOrDefaultAsync(cc => cc.urecNo == urecNo); // Adjust the query if necessary
        }

        public async Task SaveCompletionCertificateAsync(CompletionCertificate certificate)
        {
            _context.CompletionCertificate.Add(certificate);
            await _context.SaveChangesAsync();
        }

    }
}