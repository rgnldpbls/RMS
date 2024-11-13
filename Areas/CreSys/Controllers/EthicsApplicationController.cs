using CRE.Interfaces;
using CRE.Models;
using CRE.Services;
using CRE.ViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Models;
using System.Security.Claims;
using static CRE.ViewModels.ApplicationRequirementsViewModel;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class EthicsApplicationController : Controller
    {
        private readonly IEthicsApplicationServices _ethicsApplicationServices;
        private readonly INonFundedResearchInfoServices _nonFundedResearchInfoServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReceiptInfoServices _receiptInfoServices;
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly IConfiguration _configuration;
        private readonly ICoProponentServices _coProponentServices;
        private readonly IEthicsApplicationFormsServices _ethicsApplicationFormsServices;

        // Helper method for email validation
        private bool IsValidEmail(string email)
        {
            // Basic email validation logic here
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }
        // Constructor to initialize the services
        public EthicsApplicationController(
            IEthicsApplicationServices ethicsApplicationServices,
            INonFundedResearchInfoServices nonFundedResearchInfoServices,
            UserManager<ApplicationUser> userManager,
            IReceiptInfoServices receiptInfoServices,
            IEthicsApplicationLogServices ethicsApplicationLogServices,
            IConfiguration configuration, ICoProponentServices coProponentServices,
            IEthicsApplicationFormsServices ethicsApplicationFormsServices)

        {
            _ethicsApplicationServices = ethicsApplicationServices;
            _nonFundedResearchInfoServices = nonFundedResearchInfoServices;
            _userManager = userManager;
            _receiptInfoServices = receiptInfoServices;
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _configuration = configuration;
            _coProponentServices = coProponentServices;
            _ethicsApplicationFormsServices = ethicsApplicationFormsServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Applications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "User is not logged in.");
                return View(); // return view with some error handling
            }

            // Assuming your services are updated to accept the IdentityUser ID (string type)
            var ethicsApplications = await _ethicsApplicationServices.GetApplicationsByUserAsync(userId);
            var nonFundedResearchInfos = await _nonFundedResearchInfoServices.GetNonFundedResearchByUserAsync(userId);

            var ethicsApplicationIds = ethicsApplications.Select(a => a.urecNo).ToList();

            // Fetch the latest status from the logs for each application
            var ethicsApplicationLogs = await _ethicsApplicationLogServices.GetLatestLogsByApplicationIdsAsync(ethicsApplicationIds);

            // Populate the ApplicationsViewModel
            var model = new ApplicationsViewModel
            {
                EthicsApplication = ethicsApplications,
                NonFundedResearchInfo = nonFundedResearchInfos,
                EthicsApplicationLog = ethicsApplicationLogs
            };

            // Return the view for the application form
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ApplyEthics()
        {
            // Retrieve the logged-in user's ID from the Identity system
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                // Fetch the user record using the IdentityUser ID (assumed to be string)
                var user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);
                if (user != null)
                {
                    // Create an instance of the ViewModel
                    var model = new ApplyEthicsViewModel
                    {
                        EthicsApplication = new EthicsApplication
                        {
                            // userId = user.Id // Make sure to map the Identity user ID correctly
                        },
                        EthicsApplicationLog = new List<EthicsApplicationLog>(),
                        NonFundedResearchInfo = new NonFundedResearchInfo
                        {
                            // userId = user.Id // Similar adjustment here if needed
                        },
                        CoProponent = new List<CoProponent>() // Start with an empty list
                    };

                    // Only set ReceiptInfo for external users
                    if (roles.Contains("external researcher"))
                    {
                        model.ReceiptInfo = new ReceiptInfo(); // Create a new instance if external
                    }
                    else
                    {
                        model.ReceiptInfo = null; // Ensure it is null for internal users
                    }

                    // Pass the model to the view
                    return View(model);
                }
            }

            // If user not found or not logged in, create an empty model
            var emptyModel = new ApplyEthicsViewModel
            {
                CoProponent = new List<CoProponent>()
            };

            return View(emptyModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyEthics(ApplyEthicsViewModel model)
        {
            if (model.CoProponent == null)
            {
                model.CoProponent = new List<CoProponent>();
            }

            // Retrieve the logged-in user's ID from the Identity system
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View(model);
            }

            // Fetch the user record using the IdentityUser ID (string type)
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            if (user == null)
            {
                ModelState.AddModelError("", "User ID does not exist.");
                return View(model);
            }

            // Populate the user information in the model

            // Remove validation for fields that are not yet populated
            if (!roles.Contains("external researcher"))
            {
                ModelState.Remove("ReceiptInfo.receiptNo");
                ModelState.Remove("ReceiptInfo.amountPaid");
                ModelState.Remove("receiptFile");
            }

            // Populate ethics application details
            var ethicsApplication = model.EthicsApplication;
            ethicsApplication.urecNo = await _ethicsApplicationServices.GenerateUrecNoAsync();
            ethicsApplication.userId = userId; // Use the logged-in user's ID
            ethicsApplication.submissionDate = DateOnly.FromDateTime(DateTime.Now);
            ethicsApplication.Name = model.Name;

            // Populate non-funded research details
            var nonFundedResearchInfo = model.NonFundedResearchInfo;
            nonFundedResearchInfo.nonFundedResearchId = await _nonFundedResearchInfoServices.GenerateNonFundedResearchIdAsync();
            nonFundedResearchInfo.userId = userId; // Use the logged-in user's ID
            nonFundedResearchInfo.urecNo = ethicsApplication.urecNo;
            nonFundedResearchInfo.dateSubmitted = DateTime.Now;

            // Create a new application log entry
            var ethicsApplyLog = new EthicsApplicationLog
            {
                urecNo = ethicsApplication.urecNo,
                userId = userId, // Use the logged-in user's ID
                status = "Applied",
                changeDate = DateTime.Now
            };

            // Removed some fields are to be inputed later
            ModelState.Remove("User");
            ModelState.Remove("ReceiptInfo");
            ModelState.Remove("EthicsApplication.User");
            ModelState.Remove("EthicsApplication.userId");
            ModelState.Remove("EthicsApplication.urecNo");
            ModelState.Remove("EthicsApplication.ReceiptInfo");
            ModelState.Remove("EthicsApplication.InitialReview");
            ModelState.Remove("EthicsApplication.EthicsClearance");
            ModelState.Remove("EthicsApplication.CompletionReport");
            ModelState.Remove("EthicsApplication.NonFundedResearchInfo");
            ModelState.Remove("NonFundedResearchInfo.AppUser");
            ModelState.Remove("NonFundedResearchInfo.userId");
            ModelState.Remove("NonFundedResearchInfo.urecNo");
            ModelState.Remove("NonFundedResearchInfo.EthicsClearance");
            ModelState.Remove("NonFundedResearchInfo.EthicsApplication");
            ModelState.Remove("NonFundedResearchInfo.nonFundedResearchId");
            ModelState.Remove("NonFundedResearchInfo.CompletionCertificate");
            ModelState.Remove("ReceiptInfo.urecNo");
            ModelState.Remove("ReceiptInfo.scanReceipt");
            ModelState.Remove("ReceiptInfo.EthicsApplication");
            ModelState.Remove("EthicsApplication.CompletionCertificate");
            // Clear existing ModelState errors for CoProponents
            for (int i = 0; i < model.CoProponent.Count; i++)
            {
                ModelState.Remove($"CoProponent[{i}].NonFundedResearchInfo");
                ModelState.Remove($"CoProponent[{i}].nonFundedResearchId");
            }

            // Check for duplicate titles
            var existingResearch = await _nonFundedResearchInfoServices.SearchByTitleAsync(model.NonFundedResearchInfo.title);
            if (existingResearch != null)
            {
                ModelState.AddModelError("NonFundedResearchInfo.title", "This title has already been used for another ethics application.");
                return View(model);
            }

            var externalUser = await _userManager.FindByIdAsync(userId);
            var externalRole = await _userManager.GetRolesAsync(externalUser);

            // Validate receipt number for duplicates if the user type is external
            if (externalUser != null && externalRole.Contains("external"))
            {
                if (model.ReceiptInfo != null && !string.IsNullOrEmpty(model.ReceiptInfo.receiptNo))
                {
                    bool receiptExists = await _receiptInfoServices.ReceiptNoExistsAsync(model.ReceiptInfo.receiptNo);
                    if (receiptExists)
                    {
                        ModelState.AddModelError("ReceiptInfo.receiptNo", "This receipt number has already been used for another application.");
                        return View(model);
                    }
                }
            }
            // Check if the model state is valid
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

                // Save the application
                await _ethicsApplicationServices.ApplyForEthicsAsync(ethicsApplication);
                await _nonFundedResearchInfoServices.AddNonFundedResearchAsync(nonFundedResearchInfo);
                await _ethicsApplicationLogServices.AddLogAsync(ethicsApplyLog);

                // Save CoProponents
                if (model.CoProponent != null && model.CoProponent.Any())
                {
                    foreach (var coProponent in model.CoProponent)
                    {
                        if (coProponent != null)
                        {
                            coProponent.nonFundedResearchId = nonFundedResearchInfo.nonFundedResearchId;
                            await _coProponentServices.AddCoProponentAsync(coProponent);
                        }
                    }
                }

                var internalUser = await _userManager.FindByIdAsync(userId);
                var internalRole = await _userManager.GetRolesAsync(internalUser);

                // Handle success messages and redirection
                if (internalUser != null && !internalRole.Contains("external"))
                {
                    TempData["SuccessMessage"] = "Your application has been submitted successfully (no receipt required).";
                    return RedirectToAction("Applications");
                }

                if (internalUser != null && internalRole.Contains("external"))
                {
                    // Handle receipt upload
                    if (model.receiptFile != null && model.receiptFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.receiptFile.CopyToAsync(memoryStream);
                            model.ReceiptInfo.scanReceipt = memoryStream.ToArray();
                        }

                        // Save receipt info
                        if (model.ReceiptInfo != null)
                        {
                            model.ReceiptInfo.urecNo = ethicsApplication.urecNo;
                            await _receiptInfoServices.AddReceiptInfoAsync(model.ReceiptInfo);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("receiptFile", "Please upload a scanned receipt.");
                        return View(model);
                    }
                }

                // If everything succeeds
                TempData["SuccessMessage"] = "Your application has been submitted successfully.";
                return RedirectToAction("Applications");
            
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckDtsNumber(string dtsNo)
        {
            if (string.IsNullOrEmpty(dtsNo))
            {
                return Json(new { isValid = false, message = "DTS No. cannot be empty." });
            }

            var existingDts = await _ethicsApplicationServices.GetApplicationByDtsNoAsync(dtsNo);
            if (existingDts != null)
            {
                return Json(new { isValid = false, message = "This DTS No. is already in use." });
            }

            return Json(new { isValid = true });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ApplicationRequirements(string urecNo)
        {
            // Check if urecNo is provided
            if (string.IsNullOrEmpty(urecNo))
            {
                ModelState.AddModelError("", "UrecNo is missing.");
                return View("Error"); // Return an error view with appropriate error message
            }

            // Retrieve the logged-in user's ID from Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return View("Error"); // Return an error view if no user ID found
            }

            // Fetch all necessary data
            var ethicsApplication = await _ethicsApplicationServices.GetApplicationByUrecNoAsync(urecNo);
            var nonFundedResearchInfo = await _nonFundedResearchInfoServices.GetNonFundedResearchByUrecNoAsync(urecNo);
            var receiptInfo = await _receiptInfoServices.GetReceiptInfoByUrecNoAsync(urecNo);
            var ethicsApplicationLogs = await _ethicsApplicationLogServices.GetLogsByUrecNoAsync(urecNo);
            var ethicsApplicationForms = await _ethicsApplicationFormsServices.GetAllFormsByUrecAsync(urecNo);
            var user = await _userManager.FindByIdAsync(userId);

            // Ensure all necessary data exists
            if (ethicsApplication == null || nonFundedResearchInfo == null || user == null)
            {
                ModelState.AddModelError("", "Application or user data is missing.");
                return View("Error"); // Return error view for missing data
            }

            // Retrieve co-proponents based on the research ID
            var coProponents = await _coProponentServices.GetCoProponentsByResearchIdAsync(nonFundedResearchInfo.nonFundedResearchId);

            // Populate ViewModel
            var model = new ApplicationRequirementsViewModel
            {
                EthicsApplication = ethicsApplication,
                NonFundedResearchInfo = nonFundedResearchInfo,
                ReceiptInfo = receiptInfo,
                EthicsApplicationForms = ethicsApplicationForms,
                EthicsApplicationLog = ethicsApplicationLogs,
                CoProponent = coProponents.ToList() // Ensure it's a List
            };

            return View(model); // Pass the populated model to the view
        }

    }
}