using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CRE.Services
{
    public class ChairpersonServices : IChairpersonServices
    {
        private readonly CreDbContext _context;

        public ChairpersonServices(CreDbContext context)
        {
            _context = context;
        }

        public async Task<List<EthicsApplication>> GetApplicationsByFieldOfStudyAsync(string userId)
        {
            // Get the chairperson's faculty using the user ID
            var chairperson = await _context.Chairperson
                .Include(c => c.Faculty)
                .FirstOrDefaultAsync(c => c.Faculty.userId == userId); // Adjusted to match your userId

            if (chairperson == null)
                return new List<EthicsApplication>(); // Return an empty list if chairperson not found

            // Retrieve applications matching the chairperson's field of study and specific review types, including evaluations, evaluators, and logs
            var applications = await _context.EthicsApplication
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponent)
                .Include(e => e.InitialReview) // Include InitialReview
                .Include(e => e.EthicsEvaluation) // Include EthicsEvaluation entities
                    .ThenInclude(e => e.EthicsEvaluator) // Include the related EthicsEvaluator entities
                        .ThenInclude(ev => ev.Faculty) // Include Faculty for each EthicsEvaluator
                .Include(e => e.EthicsApplicationLog) // Include EthicsApplicationLog
                .Where(e => e.fieldOfStudy == chairperson.fieldOfStudy // Ensure correct casing for FieldOfStudy
                             && (e.InitialReview.ReviewType == "Full Review"
                             || e.InitialReview.ReviewType == "Expedited"))
                .ToListAsync();

            return applications;
        }

        public async Task AssignEvaluatorsAsync(string urecNo, List<int> evaluatorIds)
        {
            foreach (var evaluatorId in evaluatorIds)
            {
                var evaluation = new EthicsEvaluation
                {
                    urecNo = urecNo,
                    ethicsEvaluatorId = evaluatorId,
                    startDate = DateOnly.FromDateTime(DateTime.Now),

                    // Initialize fields for protocol evaluation
                    ProtocolRecommendation = "Pending", // Default status
                    ProtocolRemarks = string.Empty,

                    // Initialize fields for consent evaluation
                    ConsentRecommendation = "Pending", // Default status
                    ConsentRemarks = string.Empty,

                    // Set initial status
                    evaluationStatus = "Assigned" // Mark as assigned since evaluators are assigned
                };

                _context.EthicsEvaluation.Add(evaluation);
            }
            await _context.SaveChangesAsync();
        }


        public async Task<EthicsApplication> GetApplicationAsync(string urecNo)
        {
            return await _context.EthicsApplication
                .Include(e => e.InitialReview) // Include InitialReview for ReviewType
                .FirstOrDefaultAsync(e => e.urecNo == urecNo);
        }
        public async Task<Dictionary<string, List<EthicsEvaluator>>> GetEvaluatorNamesAsync(IEnumerable<EthicsApplication> ethicsApplications)
        {
            var evaluatorNames = new Dictionary<string, List<EthicsEvaluator>>();

            foreach (var application in ethicsApplications)
            {
                // Retrieve the evaluations for the current application and include the evaluator
                var evaluators = await _context.EthicsEvaluation
                    .Where(e => e.urecNo == application.urecNo && e.evaluationStatus != "Declined") // Exclude declined evaluators
                    .Select(e => e.EthicsEvaluator) // Select the EthicsEvaluator object
                    .ToListAsync();

                // Store the evaluators in the dictionary
                evaluatorNames[application.urecNo] = evaluators;
            }

            return evaluatorNames;
        }

        public async Task<IEnumerable<EthicsApplication>> GetUnassignedApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications)
        {
            return ethicsApplications.Where(a =>
                a.InitialReview != null &&
                (a.InitialReview.ReviewType == "Expedited" || a.InitialReview.ReviewType == "Full Review") &&
                !a.EthicsEvaluation.Any(e => e.evaluationStatus == "Evaluated") && // Exclude applications with at least one evaluated status
                (
                    !a.EthicsEvaluation.Any() ||  // Include applications with no evaluations
                    (
                        // Expedited review conditions: fewer than 3 evaluators in pending/assigned status
                        a.InitialReview.ReviewType == "Expedited" &&
                        a.EthicsEvaluation.Count(e => e.evaluationStatus == "Pending" || e.evaluationStatus == "Assigned") <= 3 &&
                        !a.EthicsEvaluation.All(e => e.endDate != null) // Exclude if all evaluations are completed
                    ) ||
                    (
                        // Full review conditions: fewer than 3 evaluators in pending/assigned status
                        a.InitialReview.ReviewType == "Full Review" &&
                        a.EthicsEvaluation.Count(e => e.evaluationStatus == "Pending" || e.evaluationStatus == "Assigned") < 3 &&
                        !a.EthicsEvaluation.All(e => e.endDate != null) // Exclude if all evaluations are completed
                    ) ||
                    (
                        // Applications with evaluations not fully completed or declined
                        a.EthicsEvaluation.Any(e => e.endDate == null) &&
                        a.EthicsEvaluation.Any(e => e.EthicsEvaluator.declinedAssignment > 0)
                    )
                ) &&
                // Exclude applications where all assigned evaluators have fully accepted and completed their evaluations
                (!a.EthicsEvaluation.Any() || a.EthicsEvaluation.Any(e => e.evaluationStatus != "Accepted" || e.endDate == null))
            );
        }



        public async Task<IEnumerable<EthicsApplication>> GetUnderEvaluationApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications)
        {
            return ethicsApplications.Where(a =>
                (a.InitialReview != null &&
                 ((a.InitialReview.ReviewType == "Expedited" &&
                   (a.EthicsEvaluation.Count == 2 || a.EthicsEvaluation.Count == 3) &&
                   a.EthicsEvaluation.All(e => e.ProtocolRecommendation == "Pending" && e.ConsentRecommendation == "Pending")) ||
                  (a.InitialReview.ReviewType == "Full Review" &&
                   a.EthicsEvaluation.Count == 3 &&
                   a.EthicsEvaluation.All(e => e.ProtocolRecommendation == "Pending" && e.ConsentRecommendation == "Pending"))) ||
                a.EthicsEvaluation.Any(e => e.endDate == null || e.evaluationStatus == "Assigned")));
        }


        public async Task<IEnumerable<EthicsApplication>> GetEvaluationResultApplicationsAsync(IEnumerable<EthicsApplication> ethicsApplications)
        {
            return ethicsApplications.Where(a =>
                a.InitialReview != null &&
                ((a.InitialReview.ReviewType == "Expedited" &&
                  a.EthicsEvaluation.Count >= 2 && a.EthicsEvaluation.Count <= 3 &&
                  a.EthicsEvaluation.All(e => e.endDate != null)) || // For Expedited: 2 to 3 evaluations with end dates
                 (a.InitialReview.ReviewType == "Full Review" &&
                  a.EthicsEvaluation.Count == 3 &&
                  a.EthicsEvaluation.All(e => e.endDate != null)))); // For Full Review: exactly 3 evaluations with end dates
        }


        public async Task<Dictionary<string, List<string>>> GetApplicationEvaluatorNamesAsync(IEnumerable<EthicsApplication> ethicsApplications)
        {
            var evaluatorNames = new Dictionary<string, List<string>>();

            foreach (var application in ethicsApplications)
            {
                var names = application.EthicsEvaluation
                    .Where(e => e.EthicsEvaluator?.Faculty != null) // Check for nulls to avoid errors
                    .Select(e => $"{e.EthicsEvaluator.Faculty.Name}") // Concatenate first and last name
                    .ToList();

                evaluatorNames[application.urecNo] = names; // Assign list of names to the application urecNo key
            }

            return evaluatorNames;
        }
    } 
}
