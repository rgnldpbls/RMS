using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class SecretariatServices : ISecretariatServices
    {
        private readonly CreDbContext _context;
        public SecretariatServices(CreDbContext context)
        {
            _context = context;
        }
    }
}
