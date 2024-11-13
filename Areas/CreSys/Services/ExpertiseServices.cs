using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class ExpertiseServices: IExpertiseServices
    {
        private readonly CreDbContext _context;
        public ExpertiseServices(CreDbContext context)
        {
            _context = context;
        }
    }
}
