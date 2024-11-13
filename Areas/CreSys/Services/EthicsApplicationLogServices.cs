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
    public class EthicsApplicationLogServices : IEthicsApplicationLogServices
    {
        private readonly CreDbContext _context;

        public EthicsApplicationLogServices(CreDbContext context)
        {
            _context = context;
        }

        // Add a new log with user input for status and comments
        public async Task AddLogAsync(EthicsApplicationLog log)
        {
            _context.EthicsApplicationLog.Add(log); // Add the log to the DbSet
            await _context.SaveChangesAsync();  // Commit the changes to the database
        }

        // Retrieve the latest log for a specific urecNo
        public async Task<EthicsApplicationLog> GetLatestLogAsync(string urecNo)
        {
            return await _context.EthicsApplicationLog
                                 .Where(log => log.urecNo == urecNo)        // Filter by urecNo
                                 .OrderByDescending(log => log.changeDate)   // Order by changeDate (latest first)
                                 .FirstOrDefaultAsync();                    // Get the latest log entry
        }

        public async Task<IEnumerable<EthicsApplicationLog>> GetLatestLogsByApplicationIdsAsync(IEnumerable<string> urecNos)
        {
            return await _context.EthicsApplicationLog
                .Where(log => urecNos.Contains(log.urecNo)) // Assuming EthicsApplicationId is the foreign key
                .GroupBy(log => log.urecNo) // Group by the application ID
                .Select(g => g.OrderByDescending(log => log.changeDate).FirstOrDefault()) // Get the latest log for each application
                .ToListAsync();
        }


        // Retrieve all logs for a specific urecNo
        public async Task<List<EthicsApplicationLog>> GetLogsByUrecNoAsync(string urecNo)
        {
            return await _context.EthicsApplicationLog
                                 .Where(log => log.urecNo == urecNo)
                                 .OrderByDescending(log => log.changeDate)
                                 .Select(log => new EthicsApplicationLog
                                 {
                                     changeDate = log.changeDate,
                                     status = log.status,
                                     comments = log.comments
                                 })
                                 .ToListAsync();
        }

        public async Task<string> GetLatestCommentByUrecNoAsync(string urecNo)
        {
            var latestLog = await _context.EthicsApplicationLog
                .Where(log => log.EthicsApplication.urecNo == urecNo)
                .OrderByDescending(log => log.changeDate)
                .FirstOrDefaultAsync();

            return latestLog?.comments ?? "No comments available";
        }
    }
}
