using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsEvaluatorExpertiseServices : IEthicsEvaluatorExpertiseServices
    {
        private readonly CreDbContext _context;
        public EthicsEvaluatorExpertiseServices(CreDbContext context)
        {
            _context = context;
        }
    }
}
