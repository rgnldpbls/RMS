using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRE.Services
{
    public class NonFundedResearchInfoServices : INonFundedResearchInfoServices
    {
        private readonly CreDbContext _context;

        public NonFundedResearchInfoServices(CreDbContext context)
        {
            _context = context;
        }
        public async Task AddNonFundedResearchAsync(NonFundedResearchInfo research)
        {
            _context.NonFundedResearchInfo.Add(research);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteNonFundedResearchAsync(string researchId)
        {
            var research = await _context.NonFundedResearchInfo.FindAsync(researchId);
            if (research != null)
            {
                _context.NonFundedResearchInfo.Remove(research);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<NonFundedResearchInfo>> GetNonFundedResearchInfosAsync(List<string> urecNos)
        {
            // Fetch NonFundedResearchInfo entries based on the provided urecNo list
            var nonFundedResearchInfos = await _context.NonFundedResearchInfo
                .Where(info => urecNos.Contains(info.urecNo)) // Assuming UrecNo is the property in NonFundedResearchInfo
                .ToListAsync();

            return nonFundedResearchInfos;
        }
        // Method to generate a unique primary key (NFID-XXXX)
        public async Task<string> GenerateNonFundedResearchIdAsync()
        {
            // Get the current year as a string
            string year = DateTime.Now.Year.ToString();  // YYYY format

            // Define the prefix for the current year
            string prefix = $"NFID-{year}-";

            // Retrieve IDs for the current year and process them in memory
            var currentYearIds = await _context.NonFundedResearchInfo
                .Where(r => r.nonFundedResearchId.StartsWith(prefix))
                .ToListAsync(); // Retrieve matching records into memory

            // Extract the numerical sequence from valid IDs
            var sequenceNumbers = currentYearIds
                .Where(r => r.nonFundedResearchId.Length == prefix.Length + 4 && // Ensure correct length
                            int.TryParse(r.nonFundedResearchId.Substring(prefix.Length, 4), out _)) // Ensure last 4 characters are numeric
                .Select(r => int.Parse(r.nonFundedResearchId.Substring(prefix.Length, 4))) // Extract and parse sequence
                .ToList();

            // Determine the next sequence number
            int nextSequence = (sequenceNumbers.Any() ? sequenceNumbers.Max() : 0) + 1;

            // Format the ID as NFID-YYYY-XXXX with leading zeros for the sequence part
            string id = $"{prefix}{nextSequence:D4}";

            return id;
        }



        public async Task<IEnumerable<NonFundedResearchInfo>> GetAllNonFundedResearchAsync()
        {
            return await _context.NonFundedResearchInfo.ToListAsync();
        }

        public async Task<NonFundedResearchInfo> GetNonFundedResearchByUrecNoAsync(string urecNo)
        {
            return await _context.NonFundedResearchInfo
                .FirstOrDefaultAsync(r => r.urecNo == urecNo);
        }

        public async Task<IEnumerable<NonFundedResearchInfo>> GetNonFundedResearchByUserAsync(string userId)
        {
            return await _context.NonFundedResearchInfo
                //.Where(r => r.userId == userId) // Assuming UserId is the foreign key to the User
                .ToListAsync();
        }

        public async Task<NonFundedResearchInfo> SearchByTitleAsync(string title)
        {
            string normalizedTitle = title.ToLower().Replace(" ", ""); // Normalize title by lowering case and removing spaces
            return await _context.NonFundedResearchInfo
                                 .FirstOrDefaultAsync(r => r.title.ToLower().Replace(" ", "") == normalizedTitle);
        }
        public async Task UpdateNonFundedResearchAsync(NonFundedResearchInfo research)
        {
            _context.NonFundedResearchInfo.Update(research);
            await _context.SaveChangesAsync();
        }
    }
}
