using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Models;
using ResearchManagementSystem.Services;
using RemcSys.Data;
using RemcSys.Models;
using rscSys_final.Data;
using CrdlSys.Data;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.ViewModels;
using ResearchManagementSystem.Data;

namespace ResearchManagementSystem.Controllers
{
    public class DirectorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CreDbContext _cre_context;
        private readonly CrdlDbContext _crdl_context;
        private readonly RemcDBContext _remc_context;
        private readonly rscSysfinalDbContext _rsc_context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public DirectorController(ApplicationDbContext context, RemcDBContext remc_context, rscSysfinalDbContext rsc_context, CreDbContext cre_context, CrdlDbContext crdl_context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, UserService userService, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _remc_context = remc_context;
            _rsc_context = rsc_context;
            _cre_context = cre_context;
            _crdl_context = crdl_context;
            _roleManager = roleManager;
            _userManager = userManager;
            _userService = userService;
            _signInManager = signInManager; // Initialize SignInManager
        }
        public async Task<IActionResult> Index()
        {
            if (_remc_context.REMC_Settings.First().isMaintenance)
            {
                return RedirectToAction("UnderMaintenance", "Home");
            }

            //await RemindEvaluations();
            //await RemindSubmitProgressReport();

            var fundedResearch = await _remc_context.REMC_FundedResearches.ToListAsync();

            var rankedBranch = _remc_context.REMC_FundedResearches
                .GroupBy(r => r.branch)
                .Select(g => new
                {
                    BranchName = g.Key,
                    TotalResearch = g.Count(),
                })
                .OrderByDescending(g => g.TotalResearch)
                .Take(3)
                .ToList();

            var model = new Tuple<IEnumerable<FundedResearch>, IEnumerable<dynamic>>(fundedResearch, rankedBranch);

            // RSCCCCCCC


            // Count for grants (Master's Thesis, Dissertation)
            var totalGrants = _rsc_context.RSC_Requests
                .Where(r => (r.ApplicationType == "Master's Thesis" || r.ApplicationType == "Dissertation" || r.ApplicationType.Contains("Grants")) &&
                            (r.Status == "Approved" || r.Status == "Endorsed by RMO"))
                .Count();

            // Count other categories
            var totalIncentives = _rsc_context.RSC_Requests
                .Count(r => r.ApplicationType.Contains("Incentive") || r.ApplicationType.Contains("Literary"));

            var totalAssistance = _rsc_context.RSC_Requests
                .Count(r => r.ApplicationType.Contains("Presentation") || r.ApplicationType.Contains("Assistance"));

            // Pass data to the view

            ViewBag.TotalGrants = totalGrants;
            ViewBag.TotalIncentives = totalIncentives;
            ViewBag.TotalAssistance = totalAssistance;
            ViewBag.TotalEndorsed = totalGrants + totalIncentives + totalAssistance;

            //CRDLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL
            var ongoingPartnersCount = _crdl_context.StakeholderUpload
                .Count(s => s.ContractStatus == "Active" && !s.IsArchived);
            ViewBag.OngoingPartnersCount = ongoingPartnersCount;

            var upcomingEventsCount = _crdl_context.ResearchEvent
                .Count(e => e.EventDate >= DateTime.Now && !e.IsArchived);
            ViewBag.UpcomingEventsCount = upcomingEventsCount;

            var finishedEventsCount = _crdl_context.ResearchEvent
                .Count(e => e.EndTime <= DateTime.Now && !e.IsArchived);
            ViewBag.FinishedEventsCount = finishedEventsCount;

            var ongoingEventsCount = _crdl_context.ResearchEvent
                .Count(e => e.EventDate <= DateTime.Now && e.EndTime >= DateTime.Now && !e.IsArchived);
            ViewBag.OngoingEventsCount = ongoingEventsCount;

            var expiredContractsCount = _crdl_context.StakeholderUpload
                .Count(s => s.ContractStatus == "Expired");
            ViewBag.ExpiredContractsCount = expiredContractsCount;

            var pendingContractsCount = _crdl_context.StakeholderUpload
                .Count(s => s.Status == "Pending" && !s.IsArchived);
            ViewBag.PendingContractsCount = pendingContractsCount;

            var terminatedContractsCount = _crdl_context.StakeholderUpload
                .Count(s => s.ContractStatus == "Terminated");
            ViewBag.TerminatedContractsCount = terminatedContractsCount;

            var extendedContractsCount = _crdl_context.RenewalHistory.Count();
            ViewBag.ExtendedContractsCount = extendedContractsCount;

            var totEvents = upcomingEventsCount + ongoingPartnersCount + finishedEventsCount;
            var totLinkages = ongoingPartnersCount + expiredContractsCount + pendingContractsCount + terminatedContractsCount + extendedContractsCount;
            var totEventsAndLinkages = totEvents + totLinkages;

            ViewBag.EventsTotal = totEvents;
            ViewBag.LinkagesTotal = totLinkages;
            ViewBag.EventsAndLinkagesTotal = totEventsAndLinkages;

            //CREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
            // Get the total number of applications with logs
            var totalApplications = _cre_context.CRE_EthicsApplication
                .Where(application => application.EthicsApplicationLogs.Any()) // Only count applications with logs
                .Count();

            // Get the total number of ethics clearances
            var totalClearancesIssued = _cre_context.CRE_EthicsClearance.Count();

            // Get the total number of terminal reports
            var totalTerminalReports = _cre_context.CRE_CompletionReports.Count();

            // Get the total number of certificates issued
            var totalCertificatesIssued = _cre_context.CRE_CompletionCertificates.Count();


            ViewBag.TotalEthicsApplication = totalApplications;
            ViewBag.TotalClearancesIssued = totalClearancesIssued;
            ViewBag.TotalTerminalReports = totalTerminalReports;
            ViewBag.TotalCertificatesIssued = totalCertificatesIssued;



            return View(model);


        }

        public IActionResult DirectorCenters()
        {
            return View();
        }
        public IActionResult DirectorNotifications()
        {
            return View();
        }
        public IActionResult DirectorProfile()
        {
            return View();
        }
        public IActionResult DirectorHelp()
        {
            return View();
        }
        public IActionResult DirectorReportGenerateReport()
        {
            return View();
        }
        public IActionResult DirectorReportGenerateReportPreview()
        {
            return View();
        }

        public IActionResult DirectorReportRecentReport()
        {
            return View();
        }
        public IActionResult DirectorReportArchivedReport()
        {
            return View();
        }
        public IActionResult DirectorReportRecentReportPreview()
        {
            return View();
        }
        public IActionResult DirectorReportArchivedReportPreview()
        {
            return View();
        }
        public IActionResult DirectorRateUs()
        {
            return View();
        }
        public IActionResult DirectorAbout()
        {
            return View();
        }
        public IActionResult DirectorAccomplishments()
        {
            return View();
        }

        public IActionResult DirectorCitationView()
        {
            return View();
        }
        public IActionResult DirectorCopyrightView()
        {
            return View();
        }
        public IActionResult DirectorPatentView()
        {
            return View();
        }
        public IActionResult DirectorPresentationView()
        {
            return View();
        }
        public IActionResult DirectorProductionView()
        {
            return View();
        }
        public IActionResult DirectorPublicationView()
        {
            return View();
        }
        public IActionResult DirectorUtilizationView()
        {
            return View();
        }



        public async Task<IActionResult> ManageRMCCCluster()
        {
            var faculty = await _userService.GetUsersInRoleAsync("Faculty");
            return View(faculty); // Pass the list to the view

        }

        public async Task<IActionResult> ManageRMCC(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found.";
                return View("Error");  // You may want to redirect to a different view
            }

            var roles = _roleManager.Roles.ToList();  // Get all roles
            var userRoles = await _userManager.GetRolesAsync(user);  // Get the user's current roles

            var model = roles.Select(role => new ManageUserRolesViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Selected = userRoles.Contains(role.Name)  // Check if the user has the role
            }).ToList();

            ViewBag.UserName = user.UserName;
            ViewBag.UserId = userId;

            return View(model);  // Pass the model to the view
        }


    }
}


