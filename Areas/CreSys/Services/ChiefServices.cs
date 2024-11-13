using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class ChiefServices : IChiefServices
    {
        private readonly CreDbContext _context;
        public ChiefServices(CreDbContext context)
        {
            _context = context;
        }
    }
}
