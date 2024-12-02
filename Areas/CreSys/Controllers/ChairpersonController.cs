using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Models;
using System.Security.Claims;

namespace ResearchManagementSystem.Areas.CreSys.Controllers
{
    [Area("CreSys")]
    [Authorize]
    public class ChairpersonController : Controller
    {
        private readonly CreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEthicsEmailService _emailService;
        private readonly IAllServices _allServices;
        public ChairpersonController(CreDbContext context, UserManager<ApplicationUser> userManager, IEthicsEmailService emailService, IAllServices allServices)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
            _allServices = allServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Chairperson")]
        [HttpGet]
        public async Task<IActionResult> SelectApplication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ethicsApplications = await _allServices.GetApplicationsByFieldOfStudyAsync(userId);
            var evaluatorNames = await _allServices.GetEvaluatorNamesAsync(ethicsApplications);
            var unassignedApplications = await _allServices.GetUnassignedApplicationsAsync(ethicsApplications);
            var underEvaluationApplications = await _allServices.GetUnderEvaluationApplicationsAsync(ethicsApplications);
            var evaluationResultApplications = await _allServices.GetEvaluationResultApplicationsAsync(ethicsApplications);
            var nonFundedResearchInfos = await _allServices.GetNonFundedResearchInfosAsync(ethicsApplications.Select(a => a.UrecNo).ToList());

            var viewModel = new ChairpersonApplicationsViewModel
            {
                UnassignedApplications = unassignedApplications.ToList(),
                UnderEvaluationApplications = underEvaluationApplications.ToList(),
                EvaluationResultApplications = evaluationResultApplications.ToList(),
                ApplicationEvaluatorNames = evaluatorNames,
                NonFundedResearchInfo = nonFundedResearchInfos
            };

            return View(viewModel);
        }


        [Authorize(Roles = "Chairperson")]
        [HttpGet]
        public async Task<IActionResult> GetStarted()
        {
            // Get the current user ID from the claims (assuming you're using Identity)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the user exists in the CRE_Chairperson table
            var chairpersonExists = await _context.CRE_Chairperson
                .AnyAsync(c => c.UserId == userId);

            if (chairpersonExists)
            {
                // If the chairperson exists, set a success message and hardcoded redirect to Index in the Home controller within CreSys area
                TempData["Message"] = "You're all set up!";
                return RedirectToAction("Index", "Home", new { area = "CreSys" });
            }

            // Retrieve the list of fields of study and exclude the ones already taken by other Chairpersons
            var assignedFields = await _context.CRE_Chairperson
                .Select(c => c.FieldOfStudy)
                .ToListAsync();

            // Predefined list of all fields of study
            var allFieldsOfStudy = new List<string>
    {
        "Education",
        "Computer Science, Information Systems, and Technology",
        "Engineering, Architecture, and Design",
        "Humanities, Language, and Communication",
        "Business",
        "Social Sciences",
        "Science, Mathematics, and Statistics"
    };

            // Filter out the fields that are already assigned to another Chairperson
            var availableFields = allFieldsOfStudy.Except(assignedFields).ToList();

            // Pass the available fields to the view
            return View(availableFields);
        }


        [Authorize(Roles = "Chairperson")]
         
        [HttpPost]
        public async Task<IActionResult> SelectFieldOfStudy(string selectedField)
        {
            // Get the current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user record using UserManager (ApplicationUser)
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["Message"] = "User not found!";
                return RedirectToAction("GetStarted");
            }

            // Concatenate the full name: FirstName + MiddleName (initial) + LastName
            var fullName = $"{user.FirstName} {user.MiddleName?.Substring(0, 1)}. {user.LastName}".Trim();

            // Check if the user has already been assigned a field of study
            var chairpersonExists = await _context.CRE_Chairperson
                .AnyAsync(c => c.UserId == userId);

            if (chairpersonExists)
            {
                // Show a message if the user has already selected a field of study
                TempData["S"] = "You have already selected a field of study.";
                return RedirectToAction("GetStarted");
            }

            // Assign the selected field of study to the user
            var chairperson = new Chairperson
            {
                UserId = userId,
                FieldOfStudy = selectedField,
                Name = fullName // Assuming you have a FullName property in the Chairperson model
            };

            _context.CRE_Chairperson.Add(chairperson);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Field of Study successfully assigned!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [Authorize(Roles = "Chairperson")]
        [HttpGet]
        public async Task<IActionResult> AssignEvaluators(string urecNo)
        {
            var viewModel = await _allServices.GetApplicationDetailsForEvaluationAsync(urecNo);
            string requiredFieldOfStudy = viewModel.EthicsApplication.FieldOfStudy;

            var applicantUser = await _userManager.FindByIdAsync(viewModel.EthicsApplication.UserId);

            viewModel.PendingEvaluators = await _allServices.GetPendingEvaluatorsAsync(urecNo);
            viewModel.AcceptedEvaluators = await _allServices.GetAcceptedEvaluatorsAsync(urecNo);
            viewModel.DeclinedEvaluators = await _allServices.GetDeclinedEvaluatorsAsync(urecNo);
            viewModel.EvaluatedEvaluators = await _allServices.GetEvaluatedEvaluatorsAsync(urecNo); // Fetch evaluated evaluators

            var allEvaluators = await _allServices.GetAllEvaluatorsAsync();
            var assignedOrEvaluatedEvaluatorIds = viewModel.PendingEvaluators
                .Concat(viewModel.AcceptedEvaluators)
                .Concat(viewModel.DeclinedEvaluators)
                .Concat(viewModel.EvaluatedEvaluators)
                .Select(e => e.EthicsEvaluatorId)
                .ToHashSet();

            var applicantId = viewModel.EthicsApplication.UserId;

            viewModel.AllAvailableEvaluators = allEvaluators
                .Where(e => e.UserID != applicantId && // Ensure the evaluator isn't the applicant
                            !assignedOrEvaluatedEvaluatorIds.Contains(e.EthicsEvaluatorId)) // Exclude already assigned or evaluated evaluators
                .OrderBy(e => e.Pending)
                .ToList();

            viewModel.RecommendedEvaluators = (await _allServices
                .GetRecommendedEvaluatorsAsync(allEvaluators, requiredFieldOfStudy, applicantId, _userManager))
                .Where(e => !assignedOrEvaluatedEvaluatorIds.Contains(e.EthicsEvaluatorId))
                .ToList();

            viewModel.IsEvaluatorLimitReached = (viewModel.PendingEvaluators.Count + viewModel.AcceptedEvaluators.Count) >= 3;

            return View(viewModel);
        }
        [Authorize(Roles = "Chairperson")]
         
        [HttpPost]
        public async Task<IActionResult> AssignEvaluators(string urecNo, List<int> selectedEvaluatorIds)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(urecNo) || selectedEvaluatorIds == null || selectedEvaluatorIds.Count == 0)
            {
                return BadRequest("Invalid parameters.");
            }

            // Get the user ID of the logged-in chairperson
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Assign evaluators asynchronously
            foreach (var evaluatorId in selectedEvaluatorIds)
            {
                // Fetch the evaluator record from the database (replace with your actual logic to get the evaluator)
                var evaluator = await _context.CRE_EthicsEvaluator
                    .FirstOrDefaultAsync(e => e.EthicsEvaluatorId == evaluatorId);

                if (evaluator != null)
                {
                    // Retrieve the user ID from the evaluator
                    var evaluatorUserId = evaluator.UserID; // Assuming 'UserId' is the column in 'EthicsEvaluator' table

                    // Fetch the evaluator's user record using UserManager
                    var evaluatorUser = await _userManager.FindByIdAsync(evaluatorUserId);

                    if (evaluatorUser != null)
                    {
                        // Construct the full name with first, middle initial, and last name
                        string evaluatorFullName = $"{evaluatorUser.FirstName} {evaluatorUser.MiddleName?.FirstOrDefault()}. {evaluatorUser.LastName}";

                        // Now assign the evaluator with their full name
                        await _allServices.AssignEvaluatorAsync(urecNo, evaluatorId, evaluatorFullName);
                    }
                }
            }

            // Create a new log entry
            var logEntry = new EthicsApplicationLogs
            {
                UrecNo = urecNo,
                UserId = userId,
                Status = "Evaluators Assigned",
                ChangeDate = DateTime.UtcNow
            };

            // Save the log entry using the EthicsApplicationLogServices
            await _allServices.AddLogAsync(logEntry);

            // Redirect to a specific action after assignment
            return RedirectToAction("SelectApplication", new { urecNo = urecNo });
        }

    }
}
