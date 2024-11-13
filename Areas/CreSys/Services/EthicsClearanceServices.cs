using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class EthicsClearanceServices : IEthicsClearanceServices
    {
        private readonly CreDbContext _context;
        public EthicsClearanceServices(CreDbContext context)
        {
            _context = context;
        }

        public async Task<EthicsClearance> GetClearanceByUrecNoAsync(string urecNo)
        {
            return await _context.EthicsClearance
            .FirstOrDefaultAsync(clearance => clearance.urecNo == urecNo);
        }

            public async Task<bool> HandleRevisionsAsync(string urecNo, string applicationDecision, string remarks, string userId)
            {
                // Retrieve the application from the database
                var application = await _context.EthicsApplication.FindAsync(urecNo);

                if (application == null)
                {
                    return false; // Application not found
                }

                // Create a log entry for the revision request
                var logEntry = new EthicsApplicationLog
                {
                    urecNo = urecNo,
                    userId = userId, // The ID of the chief who requested the revisions
                    comments = remarks,
                    status = applicationDecision,
                    changeDate = DateTime.Now // Log the time of the revision request
                };

                _context.EthicsApplicationLog.Add(logEntry); // Add log entry to the context

                await _context.SaveChangesAsync(); // Save changes to the database

                return true; // Indicate success
            }
        public async Task<bool> IssueEthicsClearanceAsync(EthicsClearance ethicsClearance, IFormFile uploadedFile, string remarks, string userId)
        {
            // Check if there's an uploaded file to process
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await uploadedFile.CopyToAsync(memoryStream); // Copy file to memory stream
                    ethicsClearance.file = memoryStream.ToArray(); // Store the file as byte array
                }
            }

            // Add the EthicsClearance record to the database
            _context.EthicsClearance.Add(ethicsClearance);

            // Create a new log entry for the issuance process
            var applicationLog = new EthicsApplicationLog
            {
                urecNo = ethicsClearance.urecNo,
                status = "Clearance Issued",
                comments = remarks,              
                changeDate = DateTime.Now,
                userId = userId 
            };

            // Add the log entry to the logs table
            _context.EthicsApplicationLog.Add(applicationLog);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true; // Return true if the operation is successful
        }

    }
}
