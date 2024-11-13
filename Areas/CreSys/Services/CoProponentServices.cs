using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class CoProponentServices : ICoProponentServices
    {
        private readonly CreDbContext _context;
        public CoProponentServices(CreDbContext context)
        {
            _context = context;
        }

        public async Task AddCoProponentAsync(CoProponent coProponent)
        {
            await _context.CoProponent.AddAsync(coProponent);
            await _context.SaveChangesAsync();
        }

        // Get all co-proponents for a specific research by nonFundedResearchId
        public async Task<IEnumerable<CoProponent>> GetCoProponentsByResearchIdAsync(string nonFundedResearchId)
        {
            return await _context.CoProponent
                                 .Where(c => c.nonFundedResearchId == nonFundedResearchId)
                                 .ToListAsync();
        }
    }
}