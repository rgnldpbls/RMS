using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CRE.Services
{
    public class ReceiptInfoServices : IReceiptInfoServices
    {
        private readonly CreDbContext _context;

        public ReceiptInfoServices(CreDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ReceiptNoExistsAsync(string receiptNo)
        {
            return await _context.ReceiptInfo.AnyAsync(r => r.receiptNo == receiptNo);
        }
        public async Task AddReceiptInfoAsync(ReceiptInfo receipt)
        {
            _context.ReceiptInfo.Add(receipt);
            await _context.SaveChangesAsync();
        }

        public async Task<ReceiptInfo> GetReceiptInfoByUrecNoAsync(string urecNo)
        {
            return await _context.ReceiptInfo
                                 .FirstOrDefaultAsync(r => r.urecNo == urecNo);
        }
    }
}
