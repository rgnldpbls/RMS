using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class EthicsEvaluatorServices : IEthicsEvaluatorServices
    {
        private readonly CreDbContext _context;
        public EthicsEvaluatorServices(CreDbContext context)
        {
            _context = context;
        }
    }
}
