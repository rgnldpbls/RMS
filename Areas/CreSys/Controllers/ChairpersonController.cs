using CRE.Interfaces;
using CRE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using CRE.Services;
using CRE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CRE.Data;
using Microsoft.AspNetCore.Authorization;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class ChairpersonController : Controller
    {

        private readonly IChairpersonServices _chairpersonServices;
        private readonly IEthicsEvaluationServices _ethicsEvaluationServices;
        private readonly INonFundedResearchInfoServices _nonFundedResearchInfoServices;
        private readonly ICoProponentServices _coProponentServices;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;


        public ChairpersonController(IChairpersonServices chairpersonServices,
            IEthicsEvaluationServices ethicsEvaluationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            ICoProponentServices coProponentServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices)
        {
            _chairpersonServices = chairpersonServices;
            _ethicsEvaluationServices = ethicsEvaluationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _coProponentServices = coProponentServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
        }
        
        [Authorize(Roles = "Chairperson")]
        [HttpGet]
        public async Task<IActionResult> SelectApplication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ethicsApplications = await _chairpersonServices.GetApplicationsByFieldOfStudyAsync(userId);
            var evaluatorNames = await _chairpersonServices.GetEvaluatorNamesAsync(ethicsApplications);
            var unassignedApplications = await _chairpersonServices.GetUnassignedApplicationsAsync(ethicsApplications);
            var underEvaluationApplications = await _chairpersonServices.GetUnderEvaluationApplicationsAsync(ethicsApplications);
            var evaluationResultApplications = await _chairpersonServices.GetEvaluationResultApplicationsAsync(ethicsApplications);
            var nonFundedResearchInfos = await _nonFundedResearchInfoServices.GetNonFundedResearchInfosAsync(ethicsApplications.Select(a => a.urecNo).ToList());

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
        public async Task<IActionResult> AssignEvaluators(string urecNo)
        {
            var viewModel = await _ethicsEvaluationServices.GetApplicationDetailsForEvaluationAsync(urecNo);
            string requiredFieldOfStudy = viewModel.EthicsApplication.fieldOfStudy;

            var applicantUser = viewModel.EthicsApplication.Name;
            string applicantName = applicantUser;

            viewModel.PendingEvaluators = await _ethicsEvaluationServices.GetPendingEvaluatorsAsync(urecNo);
            viewModel.AcceptedEvaluators = await _ethicsEvaluationServices.GetAcceptedEvaluatorsAsync(urecNo);
            viewModel.DeclinedEvaluators = await _ethicsEvaluationServices.GetDeclinedEvaluatorsAsync(urecNo);

            var allEvaluators = await _ethicsEvaluationServices.GetAllEvaluatorsAsync();
            var assignedEvaluatorIds = viewModel.PendingEvaluators
                .Concat(viewModel.AcceptedEvaluators)
                .Concat(viewModel.DeclinedEvaluators)
                .Select(e => e.ethicsEvaluatorId)
                .ToHashSet();

            var declinedEvaluatorIds = await _ethicsEvaluationServices.GetDeclinedEvaluatorsAsync(urecNo);
            var declinedEvaluatorIdSet = new HashSet<int>(declinedEvaluatorIds.Select(e => e.ethicsEvaluatorId));

            var applicantId = viewModel.EthicsApplication.userId;

            viewModel.AllAvailableEvaluators = allEvaluators
                .Where(e => e.Faculty.userId != applicantId &&
                             !declinedEvaluatorIdSet.Contains(e.ethicsEvaluatorId) &&
                             !assignedEvaluatorIds.Contains(e.ethicsEvaluatorId))
                .ToList();

            viewModel.RecommendedEvaluators = (await _ethicsEvaluationServices
                .GetRecommendedEvaluatorsAsync(allEvaluators, requiredFieldOfStudy, applicantName))
                .Where(e => !assignedEvaluatorIds.Contains(e.ethicsEvaluatorId))
                .ToList();

            // Set flag if assigned evaluator count reaches 3
            viewModel.IsEvaluatorLimitReached = (viewModel.PendingEvaluators.Count + viewModel.AcceptedEvaluators.Count) >= 3;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                await _ethicsEvaluationServices.AssignEvaluatorAsync(urecNo, evaluatorId);
            }

            // Create a new log entry
            var logEntry = new EthicsApplicationLog
            {
                urecNo = urecNo,
                userId = userId,
                status = "Evaluators Assigned",
                changeDate = DateTime.UtcNow
            };

            // Save the log entry using the EthicsApplicationLogServices
            await _ethicsApplicationLogServices.AddLogAsync(logEntry); // Ensure you have this method in your service

            // Redirect to a specific action after assignment
            return RedirectToAction("SelectApplication", new { urecNo = urecNo });
        }

    }
}
