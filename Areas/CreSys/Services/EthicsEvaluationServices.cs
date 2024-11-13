using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ResearchManagementSystem.Models;

namespace CRE.Services
{
    public class EthicsEvaluationServices : IEthicsEvaluationServices
    {
        private readonly IConfiguration _configuration;
        private readonly CreDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public EthicsEvaluationServices(CreDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<int> CreateEvaluationAsync(EthicsEvaluation evaluation)
        {
            // Add the new evaluation to the context
            await _context.EthicsEvaluation.AddAsync(evaluation);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the created evaluation ID
            return evaluation.evaluationId; // Assuming EvaluationId is auto-generated
        }
        public async Task<List<EthicsEvaluator>> GetAllEvaluatorsAsync()
        {
            return await _context.EthicsEvaluator
                .Include(e => e.Faculty)                      // Include Faculty data
                .Include(e => e.EthicsEvaluatorExpertise)     // Include evaluator expertise collection
                    .ThenInclude(exp => exp.Expertise)        // Include each Expertise within EthicsEvaluatorExpertise
                .ToListAsync();
        }
        public EthicsEvaluation GetEvaluationByUrecNo(string urecNo)
        {
            return _context.EthicsEvaluation
          .FirstOrDefault(e => e.urecNo == urecNo);
        }
        public async Task<EthicsEvaluation> GetEvaluationByUrecNoAndIdAsync(string urecNo, int evaluationId)
        {
            return await _context.EthicsEvaluation
                .FirstOrDefaultAsync(e => e.urecNo == urecNo && e.evaluationId == evaluationId);
        }
        public async Task UpdateEvaluationAsync(EthicsEvaluation ethicsEvaluation)
        {
            _context.EthicsEvaluation.Update(ethicsEvaluation); // Marks the entity as modified
            await _context.SaveChangesAsync();
        }
        public async Task<EthicsEvaluation> GetEvaluationByUrecNoAndEvaluatorIdAsync(string urecNo, int ethicsEvaluatorId)
        {
            return await _context.EthicsEvaluation
                .FirstOrDefaultAsync(e => e.urecNo == urecNo && e.ethicsEvaluatorId == ethicsEvaluatorId);
        }

        public async Task<EvaluationDetailsViewModel> GetEvaluationDetailsAsync(int evaluationId)
        {
            // Fetch the evaluation along with related entities
            var evaluation = await _context.EthicsEvaluation
                
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.NonFundedResearchInfo)
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.ReceiptInfo)
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.EthicsApplicationForms) // Include forms
                .Include(ev => ev.EthicsApplication)
                    .ThenInclude(ea => ea.EthicsApplicationLog) // Include logs
                .FirstOrDefaultAsync(ev => ev.evaluationId == evaluationId); // Use EvaluationId as int

            if (evaluation == null)
            {
                throw new Exception("Evaluation not found.");
            }

            // Retrieve the associated application for further details
            var application = await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo) // Make sure we include related info
                    .ThenInclude(nf => nf.CoProponent) // Assuming you want to get co-proponents too
                .Include(a => a.ReceiptInfo)
                .FirstOrDefaultAsync(a => a.urecNo == evaluation.urecNo); // Use the appropriate ID

            // If application is not found, handle it accordingly
            if (application == null)
            {
                throw new Exception("Associated application not found.");
            }

            // Create and populate the ViewModel
            var viewModel = new EvaluationDetailsViewModel
            {
                CurrentEvaluation = evaluation, // Use the current evaluation
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo, // No null propagation
                ReceiptInfo = application.ReceiptInfo,
                EthicsEvaluator = evaluation.EthicsEvaluator, // Initialize the EthicsEvaluator
                EthicsApplicationForms = application.EthicsApplicationForms, // Initialize forms
                EthicsApplicationLog = application.EthicsApplicationLog // Initialize logs
            };

            return viewModel;
        }

        public async Task<EvaluatedExemptApplication> GetEvaluationDetailsAsync(string urecNo, int evaluationId)
        {
            // Fetching the evaluation based on both urecNo and evaluationId
            var evaluation = await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication) // Include the EthicsApplication
                    .ThenInclude(ea => ea.NonFundedResearchInfo) // Include NonFundedResearchInfo
                .Include(e => e.EthicsApplication)
                    .ThenInclude(ea => ea.EthicsApplicationLog)
                .Include(e => e.EthicsApplication)// Include EthicsApplicationLog
                .ThenInclude(e => e.InitialReview) // Include InitialReview from EthicsApplication
                
                .FirstOrDefaultAsync(e => e.urecNo == urecNo && e.evaluationId == evaluationId);

            if (evaluation == null)
                return null;

            return new EvaluatedExemptApplication
            {
                EthicsApplication = evaluation.EthicsApplication,
                NonFundedResearchInfo = evaluation.EthicsApplication?.NonFundedResearchInfo,
                EthicsApplicationLog = evaluation.EthicsApplication?.EthicsApplicationLog,
                EthicsEvaluation = evaluation,
                InitialReview = evaluation.EthicsApplication.InitialReview, // Set the InitialReview property from EthicsApplication
            };
        }
        public async Task<EvaluationDetailsViewModel> GetEvaluationAndEvaluatorDetailsAsync(string urecNo, int evaluationId)
        {
            var evaluation = await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo) // Ensure NonFundedResearchInfo is included
                        .ThenInclude(nf => nf.CoProponent) // Includes co-proponents within NonFundedResearchInfo
                .Include(e => e.EthicsApplication.EthicsApplicationLog) // Includes application logs
                .Include(e => e.EthicsApplication.InitialReview) // Includes initial review information
                .Include(e => e.EthicsEvaluator)
                    .ThenInclude(evaluator => evaluator.Faculty) // Includes faculty details for evaluator
                .FirstOrDefaultAsync(e => e.EthicsApplication.urecNo == urecNo && e.evaluationId == evaluationId);

            if (evaluation == null)
                return null;

            // Retrieve evaluator user information, if available
            var evaluatorUser = evaluation.EthicsEvaluator?.Faculty;

            // Create and return the view model
            return new EvaluationDetailsViewModel
            {
                EthicsApplication = evaluation.EthicsApplication,
                NonFundedResearchInfo = evaluation.EthicsApplication?.NonFundedResearchInfo,
                EthicsApplicationLog = evaluation.EthicsApplication?.EthicsApplicationLog ?? new List<EthicsApplicationLog>(),
                EthicsEvaluation = new List<EthicsEvaluation> { evaluation },
                InitialReview = evaluation.EthicsApplication?.InitialReview,
                EthicsEvaluator = evaluation.EthicsEvaluator ?? new EthicsEvaluator(),
            };
        }

        public async Task UpdateEvaluationStatusAsync(int evaluationId, string status, string? reasonForDecline, int ethicsEvaluatorId)
        {
            var evaluation = await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication)
                .FirstOrDefaultAsync(e => e.evaluationId == evaluationId);

            if (evaluation != null)
            {
                if (status == "Declined")
                {
                    var declinedEvaluation = new EthicsEvaluationDeclined
                    {
                        evaluationId = evaluationId,
                        reasonForDecline = reasonForDecline,
                        urecNo = evaluation.EthicsApplication?.urecNo,
                        ethicsEvaluatorId = ethicsEvaluatorId,
                        declineDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    };

                    await _context.EthicsEvaluationDeclined.AddAsync(declinedEvaluation);

                    // Instead of removing, just set a property if needed.
                    _context.EthicsEvaluation.Remove(evaluation);

                    await _context.SaveChangesAsync();

                    var declinedRecord = await _context.EthicsEvaluationDeclined
                        .FirstOrDefaultAsync(de => de.evaluationId == evaluationId && de.ethicsEvaluatorId == ethicsEvaluatorId);

                    if (declinedRecord == null)
                    {
                        // Re-add the original evaluation without specifying its identity value
                        var newEvaluation = new EthicsEvaluation
                        {
                            // Copy necessary properties from the original evaluation
                            // Exclude the identity property and any other properties that should not be duplicated
                            evaluationStatus = evaluation.evaluationStatus,
                            startDate = evaluation.startDate,
                            // Add other properties that you need to retain...
                        };

                        await _context.EthicsEvaluation.AddAsync(newEvaluation);
                        await _context.SaveChangesAsync();

                        throw new Exception("Failed to save the declined evaluation record. Decline action canceled.");
                    }
                }
                else
                {
                    evaluation.evaluationStatus = status;
                    evaluation.reasonForDecline = null; // Clear the reason for accepted evaluations
                    evaluation.startDate = DateOnly.FromDateTime(DateTime.UtcNow);

                    await _context.SaveChangesAsync();
                }
            }
        }



        public async Task AssignEvaluatorAsync(string urecNo, int evaluatorId)
        {
            var ethicsEvaluation = new EthicsEvaluation
            {
                urecNo = urecNo,
                ethicsEvaluatorId = evaluatorId,

                // Initialize protocol and consent recommendations to pending
                ProtocolRecommendation = "Pending",
                ProtocolRemarks = string.Empty,

                ConsentRecommendation = "Pending",
                ConsentRemarks = string.Empty,

                // Set initial status
                evaluationStatus = "Assigned"
            };

            await _context.EthicsEvaluation.AddAsync(ethicsEvaluation);
            await _context.SaveChangesAsync();
        }

        public async Task IncrementDeclinedAssignmentCountAsync(int ethicsEvaluatorId)
        {
            var evaluator = await _context.EthicsEvaluator.FindAsync(ethicsEvaluatorId);
            if (evaluator != null)
            {
                evaluator.declinedAssignment += 1;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<EthicsEvaluator>> GetAvailableEvaluatorsAsync(string fieldOfStudy)
        {
            return await _context.EthicsEvaluator
                .Include(e => e.Faculty)
                .Include(e => e.EthicsEvaluatorExpertise)
                .ThenInclude(ee => ee.Expertise)
                .Where(e => e.Faculty != null)  // Check for non-null Faculty and User
                .Where(e => e.EthicsEvaluatorExpertise.Any(ee => ee.Expertise.expertiseName == fieldOfStudy))  // Match expertise
                .ToListAsync();

        }
        public async Task SaveEvaluationAsync(EthicsEvaluation ethicsEvaluation)
        {
            // First, find the existing EthicsApplication by urecNo
            await _context.EthicsEvaluation.AddAsync(ethicsEvaluation);
            await _context.SaveChangesAsync();
        }
        public async Task<List<string>> GetEvaluatedUrecNosAsync()
        {
            return await _context.EthicsEvaluation
                .Select(e => e.urecNo) // Assuming `urecNo` is the identifier
                .ToListAsync();
        }
        public async Task<AssignEvaluatorsViewModel> GetApplicationDetailsForEvaluationAsync(string urecNo)
        {
            var application = await _context.EthicsApplication
                .Include(e => e.InitialReview)
                .Include(e => e.NonFundedResearchInfo)
                    .ThenInclude(nf => nf.CoProponent)
                .Include(e => e.EthicsApplicationLog)
                .Include(e => e.ReceiptInfo)
                .Include(e => e.EthicsApplicationForms)
                .FirstOrDefaultAsync(e => e.urecNo == urecNo);

            if (application == null)
            {
                throw new Exception("Application not found.");
            }

            var appUser = await _userManager.FindByIdAsync(application.userId);

            var viewModel = new AssignEvaluatorsViewModel
            {
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo?.CoProponent,
                EthicsApplicationForms = application.EthicsApplicationForms,
                ReceiptInfo = application.ReceiptInfo
            };

            return viewModel;
        }
        // Assuming EthicsApplication has a navigation property for InitialReview
        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetAssignedEvaluationsAsync(int evaluatorId)
        {
            return await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsEvaluator)
                .Where(e => e.ethicsEvaluatorId == evaluatorId
                            && e.evaluationStatus == "Assigned"
                            && e.evaluationStatus != "Accepted") // Exclude "Accepted" evaluations
                .Select(e => new AssignedEvaluationViewModel
                {
                    EthicsApplication = e.EthicsApplication,
                    EthicsEvaluation = e,
                    EthicsEvaluator = e.EthicsEvaluator, // This should be a single instance
                    NonFundedResearchInfo = e.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = e.EthicsApplication.InitialReview
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetAcceptedEvaluationsAsync(int evaluatorId)
        {
            return await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsEvaluator)
                .Where(e => e.ethicsEvaluatorId == evaluatorId && e.evaluationStatus == "Accepted")
                .Select(e => new AssignedEvaluationViewModel
                {
                    EthicsApplication = e.EthicsApplication,
                    EthicsEvaluation = e,
                    EthicsEvaluator = e.EthicsEvaluator,
                    NonFundedResearchInfo = e.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = e.EthicsApplication.InitialReview
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetCompletedEvaluationsAsync(int evaluatorId)
        {
            return await _context.EthicsEvaluation
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(e => e.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(e => e.EthicsEvaluator)
                .Where(e => e.ethicsEvaluatorId == evaluatorId && e.evaluationStatus == "Evaluated")
                .Select(e => new AssignedEvaluationViewModel
                {
                    EthicsApplication = e.EthicsApplication,
                    EthicsEvaluation = e,
                    EthicsEvaluator = e.EthicsEvaluator,
                    NonFundedResearchInfo = e.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = e.EthicsApplication.InitialReview
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AssignedEvaluationViewModel>> GetDeclinedEvaluationsAsync(int evaluatorId)
        {
            return await _context.EthicsEvaluationDeclined
                    .Include(e => e.EthicsEvaluator) // Include evaluator in EthicsEvaluation
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.InitialReview)
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.NonFundedResearchInfo)
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.ReceiptInfo)
                .Include(de => de.EthicsApplication)
                    .ThenInclude(a => a.EthicsApplicationLog)
                .Where(de => de.ethicsEvaluatorId == evaluatorId)
                .Select(de => new AssignedEvaluationViewModel
                {
                    EthicsApplication = de.EthicsApplication,
                    EthicsEvaluator = de.EthicsEvaluator, // Assign evaluator information
                    NonFundedResearchInfo = de.EthicsApplication.NonFundedResearchInfo,
                    InitialReview = de.EthicsApplication.InitialReview,
                    EthicsApplicationLogs = de.EthicsApplication.EthicsApplicationLog,
                    ReceiptInfo = de.EthicsApplication.ReceiptInfo,
                    EthicsEvaluationDeclined = de // Set the declined evaluation details
                })
                .ToListAsync();
        }


        public async Task<EthicsEvaluator> GetEvaluatorByUserIdAsync(string userId)
        {
            // Find the Faculty associated with the userId
            var faculty = await _context.Faculty
                .FirstOrDefaultAsync(f => f.userId == userId);

            // If faculty is not found, return null or throw an exception based on your error handling preference
            if (faculty == null)
            {
                return null; // or throw new Exception("Faculty not found");
            }

            // Find the EthicsEvaluator associated with the facultyId
            return await _context.EthicsEvaluator
                .FirstOrDefaultAsync(e => e.facultyId == faculty.facultyId);
        }

        public async Task<IEnumerable<EthicsEvaluator>> GetAvailableEvaluatorsAsync(IEnumerable<EthicsEvaluator> allEvaluators, string applicantName)
        {
            return allEvaluators
                .Where(e => e.Faculty?.Name != applicantName) // Exclude applicant by matching names
                .OrderBy(e => e.pendingEval); // Sort by least pending evaluations
        }

        public async Task<IEnumerable<EthicsEvaluator>> GetRecommendedEvaluatorsAsync(IEnumerable<EthicsEvaluator> allEvaluators, string requiredFieldOfStudy, string applicantName)
        {
            return allEvaluators
                .Where(e => e.EthicsEvaluatorExpertise
                    .Any(exp => exp.Expertise != null && exp.Expertise.expertiseName == requiredFieldOfStudy)) // Filter by required expertise
                .Where(e => e.Faculty?.Name != applicantName) // Exclude applicant by name
                .OrderBy(e => e.pendingEval) // Sort by least pending evaluations
                .Take(3); // Take top 3 recommended evaluators
        }


        public async Task<List<ChiefEvaluationViewModel>> GetExemptApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.CoProponent)
                .Where(a => a.InitialReview.ReviewType == "Exempt" && !a.EthicsEvaluation.Any())
                .Select(a => new ChiefEvaluationViewModel
                {
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsApplication = a,
                    InitialReview = a.InitialReview,
                    ReceiptInfo = a.ReceiptInfo,
                    EthicsApplicationForms = a.EthicsApplicationForms,
                    EthicsApplicationLog = a.EthicsApplicationLog
                }).ToListAsync();
        }

        public async Task<List<EvaluatedExemptApplication>> GetEvaluatedExemptApplicationsAsync()
        {
            return await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(n => n.CoProponent)
                .Include(a => a.EthicsEvaluation) // Include the EthicsEvaluation to access ChiefId
                
                .Include(a => a.InitialReview) // Include InitialReview if you need it
                .Where(a => a.InitialReview.ReviewType == "Exempt" && a.EthicsEvaluation.Any())
                .Select(a => new EvaluatedExemptApplication
                {
                    EthicsApplication = a,
                    NonFundedResearchInfo = a.NonFundedResearchInfo,
                    EthicsEvaluation = a.EthicsEvaluation.FirstOrDefault(),
                    InitialReview = a.InitialReview,
                    EthicsApplicationLog = a.EthicsApplicationLog,

                })
                .ToListAsync();
        }

        public async Task<List<EvaluatedExpeditedApplication>> GetEvaluatedExpeditedApplicationsAsync()
        {
            // Retrieve the applications along with their related data, including logs
            var applications = await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo) // Include NonFundedResearchInfo
                .Include(a => a.EthicsEvaluation) // Include EthicsEvaluation entities
                .Include(a => a.EthicsApplicationLog) // Include EthicsApplicationLog
                .Where(a => a.InitialReview.ReviewType == "Expedited" && a.EthicsEvaluation.Any())
                .ToListAsync();

            // Select the evaluated applications with the evaluators and logs
            return applications.Select(a => new EvaluatedExpeditedApplication
            {
                EthicsApplication = a,
                NonFundedResearchInfo = a.NonFundedResearchInfo,
                EthicsEvaluation = a.EthicsEvaluation.ToList(),
                InitialReview = a.InitialReview,
                // Collect all evaluators for all evaluations
                EthicsEvaluators = a.EthicsEvaluation
                    .Select(e => e.EthicsEvaluator)
                    .Where(e => e != null) // Filter out nulls
                    .Distinct()
                    .ToList(),
                EthicsApplicationLog = a.EthicsApplicationLog // Include the logs in the result
            }).ToList();
        }


        public async Task<List<EvaluatedFullReviewApplication>> GetEvaluatedFullReviewApplicationsAsync()
        {
            // Retrieve the applications along with their related data
            var applications = await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                .Include(a => a.EthicsEvaluation)
                    .ThenInclude(e => e.EthicsEvaluator) // Include evaluators
                        .ThenInclude(ee => ee.Faculty)
                .Include(a => a.EthicsApplicationLog) // Include EthicsApplicationLog
                .Where(a => a.InitialReview.ReviewType == "Full Review" && a.EthicsEvaluation.Any())
                .ToListAsync();

            // Select the evaluated applications with the evaluators
            return applications.Select(a => new EvaluatedFullReviewApplication
            {
                EthicsApplication = a,
                NonFundedResearchInfo = a.NonFundedResearchInfo,
                EthicsEvaluation = a.EthicsEvaluation.ToList(),
                InitialReview = a.InitialReview,
                // Collect all evaluators for all evaluations
                EthicsEvaluator = a.EthicsEvaluation
                    .Select(e => e.EthicsEvaluator)
                    .Where(e => e != null) // Filter out nulls
                    .Distinct()
                    .ToList(),
                EthicsApplicationLog = a.EthicsApplicationLog
            }).ToList();
        }
        public async Task<EvaluationDetailsViewModel> GetEvaluationDetailsWithUrecNoAsync(string urecNo, int evaluationId)
        {
            // Fetch the application based on urecNo and evaluationId
            var application = await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                    .ThenInclude(a => a.CoProponent)
                .Include(a => a.EthicsApplicationLog) // Include EthicsApplicationLog
                .Include(a => a.EthicsApplicationForms)
                .Include(a => a.InitialReview)
                .Include(a => a.EthicsEvaluation)
                .FirstOrDefaultAsync(a => a.urecNo == urecNo);

            if (application == null)
            {
                throw new Exception("Application not found");
            }

            var evaluation = await _context.EthicsEvaluation
                .FirstOrDefaultAsync(e => e.evaluationId == evaluationId);

            if (evaluation == null)
            {
                throw new Exception("Evaluation not found");
            }

            // Create and populate the ViewModel
            var model = new EvaluationDetailsViewModel
            {
                EthicsApplication = application,
                NonFundedResearchInfo = application.NonFundedResearchInfo,
                CoProponent = application.NonFundedResearchInfo.CoProponent.ToList(),
                EthicsApplicationForms = application.EthicsApplicationForms,
                InitialReview = application.InitialReview,
                EthicsEvaluation = new List<EthicsEvaluation> { evaluation },
                ReceiptInfo = application.ReceiptInfo // Assuming you have a ReceiptInfo property
            };

            return model;
        }

        public async Task<List<EthicsEvaluator>> GetPendingEvaluatorsAsync(string urecNo)
        {
            return await _context.EthicsEvaluator
                .Where(e => e.EthicsEvaluation.Any(a =>
                    a.EthicsApplication.urecNo == urecNo && a.evaluationStatus == "Assigned"))
                .ToListAsync();
        }

        public async Task<List<EthicsEvaluator>> GetAcceptedEvaluatorsAsync(string urecNo)
        {
            return await _context.EthicsEvaluator
                .Where(e => e.EthicsEvaluation.Any(a =>
                    a.EthicsApplication.urecNo == urecNo && a.evaluationStatus == "Accepted"))
                .ToListAsync();
        }
        public async Task<List<EthicsEvaluator>> GetDeclinedEvaluatorsAsync(string urecNo)
        {
            return await _context.EthicsEvaluator
           .Where(e => e.EthicsEvaluation.Any(a =>
               a.EthicsApplication.urecNo == urecNo && a.evaluationStatus == "Declined"))
           .ToListAsync();
        }
        public async Task<bool> AreAllEvaluationsEvaluatedAsync(string urecNo)
        {
            var evaluations = await _context.EthicsEvaluation
                .Where(e => e.EthicsApplication.urecNo == urecNo)
                .ToListAsync();

            return evaluations.All(e => e.evaluationStatus == "Evaluated");
        }

        public async Task<List<PendingIssuance>> GetPendingApplicationsForIssuanceAsync()
        {
            var pendingApplications = await _context.EthicsApplication
                .Include(a => a.NonFundedResearchInfo)
                .Include(a => a.EthicsApplicationForms)
                .Include(a => a.EthicsEvaluation) // Include evaluations
                .Include(a => a.EthicsApplicationLog)
                .Include(a => a.EthicsClearance)
                .ToListAsync();

            var pendingIssuanceList = new List<PendingIssuance>();

            foreach (var application in pendingApplications)
            {
                var evaluations = await _context.EthicsEvaluation
                    .Where(e => e.urecNo == application.urecNo) // Fetch evaluations for each application
                    .ToListAsync();

                var allEvaluationsCompleted = evaluations.All(e => e.evaluationStatus == "Evaluated");

                // Check if Form 15 is uploaded
                bool hasForm15Uploaded = application.EthicsApplicationForms
                    .Any(f => f.ethicsFormId == "FORM15");

                // Check for minor or major revisions
                bool hasMinorOrMajorRevisions = application.EthicsApplicationLog
                    .Any(log => log.status == "Minor Revisions" || log.status == "Major Revisions");

                var pendingIssuance = new PendingIssuance
                {
                    EthicsApplication = application,
                    EthicsApplicationLog = application.EthicsApplicationLog,
                    NonFundedResearchInfo = application.NonFundedResearchInfo,
                    EthicsEvaluation = evaluations,
                    EthicsClearance = application.EthicsClearance,
                    HasClearanceIssued = application.EthicsClearance != null,
                    AllEvaluationsCompleted = allEvaluationsCompleted,
                    HasForm15Uploaded = hasForm15Uploaded, // Fixed this line
                    HasMinorOrMajorRevisions = hasMinorOrMajorRevisions // Adjusted for clarity
                };

                pendingIssuanceList.Add(pendingIssuance);
            }

            return pendingIssuanceList;
        }

        public async Task AddEvaluationAsync(EthicsEvaluation evaluation)
        {
            _context.EthicsEvaluation.Add(evaluation);
            await _context.SaveChangesAsync();
        }
    
    }
}
