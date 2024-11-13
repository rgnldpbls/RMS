using CRE.Models;

namespace CRE.Interfaces
{
    public interface ICoProponentServices
    {
        Task AddCoProponentAsync(CoProponent coProponent);
        Task<IEnumerable<CoProponent>> GetCoProponentsByResearchIdAsync(string nonFundedResearchId); // Display all co-proponents for a given research ID
    }
}
