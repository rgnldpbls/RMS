using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Presentation;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO;
using ResearchManagementSystem.Data;
using ResearchManagementSystem.Models;
using ResearchManagementSystem.Services;
using static ResearchManagementSystem.Models.AddAccomplishment;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using static ResearchManagementSystem.Models.AddAccomplishment.RMCCAccomplishmentViewModel;
using RemcSys.Data;
using rscSys_final.Data;

namespace ResearchManagementSystem.Controllers
{
    public class AddAccomplishmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserService _userService;
        private readonly RemcDBContext _remcdbContext;
        private readonly rscSysfinalDbContext _rscdbContext;

        public AddAccomplishmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, UserService userService, RemcDBContext remcdbContext, rscSysfinalDbContext rscdbContext)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _remcdbContext = remcdbContext;
            _rscdbContext = rscdbContext;
        }

        [HttpGet]
        public async Task<IActionResult> FetchAndAddAccomplishments()
        {
            var productionData = await _context.Production.Select(prod => prod.ResearchTitle).ToListAsync();

            var remcData = await _remcdbContext.REMC_FundedResearches.Where(e => e.status == "Completed" && !productionData.Contains(e.research_Title)).ToListAsync();

            var rscData = await _rscdbContext.RSC_Requests.Where(e => e.Status == "Endorsed by RMO" && !productionData.Contains(e.ResearchTitle)).ToListAsync();


            var viewModel = remcData.Select(r => new AccomplishmentViewModel
            {
                ResearchTitle = r.research_Title,
                LeadResearcherName = r.team_Leader,
                LeadResearcherEmail = r.teamLead_Email,
                College = r.college,
                BranchCampus = r.branch,
                DateStarted = r.start_Date,
                DateCompleted = r.end_Date
            }).ToList();

            var viewModel2 = rscData.Select(r => new AccomplishmentViewModel
            {
                ResearchTitle = r.ResearchTitle,
                LeadResearcherName = r.Name,
                LeadResearcherEmail = r.Email,
                College = r.College,
                BranchCampus = r.Branch,
                DateStarted = r.CreatedDate,
                DateCompleted = r.SubmittedDate
            }).ToList();

            var model = new Tuple<List<AccomplishmentViewModel>, List<AccomplishmentViewModel>>(viewModel, viewModel2);

            return View(model);


        }


        [HttpPost]
        public async Task<IActionResult> Approve(string researchTitle)
        {
            var remcData = _remcdbContext.REMC_FundedResearches.Any(e => e.research_Title == researchTitle);
            if(remcData == true)
            {
                var research = await _remcdbContext.REMC_FundedResearches
                .FirstOrDefaultAsync(r => r.research_Title == researchTitle);

                if (research == null)
                {
                    return NotFound("No record found");
                }

                var accomplishment = new AddAccomplishment
                {
                    ResearchTitle = research.research_Title,
                    LeadResearcherId = research.UserId, // Map team_Leader
                    College = research.college,
                    BranchCampus = research.branch,
                    DateStarted = research.start_Date,
                    DateCompleted = research.end_Date,
                    Status = "Approved",
                    CreatedById = _userManager.GetUserId(User), // Get current user ID
                    CreatedOn = DateTime.Now,
                    Year = DateTime.Now.Year
                };

                _context.Add(accomplishment);
                await _context.SaveChangesAsync();
            }

            var rscData = _rscdbContext.RSC_Requests.Any(e => e.ResearchTitle == researchTitle);
            if(rscData == true)
            {
                var research2 = await _rscdbContext.RSC_Requests
                    .FirstOrDefaultAsync(e => e.ResearchTitle == researchTitle);

                if (research2 == null)
                {
                    return NotFound("No record found");
                }

                var accomplishment = new AddAccomplishment
                {
                    ResearchTitle = research2.ResearchTitle,
                    LeadResearcherId = research2.UserId, // Map team_Leader
                    College = research2.College,
                    BranchCampus = research2.Branch,
                    DateStarted = research2.CreatedDate,
                    DateCompleted = research2.SubmittedDate,
                    Status = "Approved",
                    CreatedById = _userManager.GetUserId(User), // Get current user ID
                    CreatedOn = DateTime.Now,
                    Year = DateTime.Now.Year
                };

                _context.Add(accomplishment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("IndexRmcc");
        }


        [HttpPost]
        public async Task<IActionResult> Reject(string researchTitle)
        {
            // Log or handle rejection as needed
            return RedirectToAction("FetchAndAddAccomplishments");
        }



        [HttpGet("IndexFaculty")]
        public async Task<IActionResult> IndexFaculty()
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Query to get all accomplishments tagged with this faculty user
            var query = _context.Production
                .Include(a => a.CoLeadResearcher)
                .Include(a => a.LeadResearcher)
                .Include(a => a.Memberone)
                .Include(a => a.Membertwo)
                .Include(a => a.Memberthree)

                .Include(a => a.CreatedBy);

            // Only get accomplishments where the user is involved as Lead, Co-Lead, or Member
            var facultyResult = await query
                .Where(a =>
                    a.LeadResearcherId == currentUser.Id ||
                    a.CoLeadResearcherId == currentUser.Id ||
                    a.MemberoneId == currentUser.Id ||
                    a.MembertwoId == currentUser.Id ||
                    a.MemberthreeId == currentUser.Id)
                .Select(a => new FacultyAccomplishmentViewModel
                {
                    ProductionId = a.ProductionId.ToString(),
                    Year = a.Year,
                    ResearchTitle = a.ResearchTitle,
                    LeadResearcherId = a.LeadResearcherId,
                    LeadResearcherFirstName = a.LeadResearcher.FirstName,
                    LeadResearcherLastName = a.LeadResearcher.LastName,
                    CoLeadResearcherId = a.CoLeadResearcherId,
                    CoLeadResearcherFirstName = a.CoLeadResearcher.FirstName,
                    CoLeadResearcherLastName = a.CoLeadResearcher.LastName,
                    MemberoneId = a.MemberoneId,
                    MemberOneFirstName = a.Memberone.FirstName,
                    MemberOneLastName = a.Memberone.LastName,
                    MembertwoId = a.MembertwoId,
                    MemberTwoFirstName = a.Membertwo.FirstName,
                    MemberTwoLastName = a.Membertwo.LastName,
                    MemberthreeId = a.MemberthreeId,
                    MemberThreeFirstName = a.Memberthree.FirstName,
                    MemberThreeLastName = a.Memberthree.LastName,
                    College = a.College,
                    Department = a.Department,
                    BranchCampus = a.BranchCampus,
                    DateStarted = a.DateStarted,
                    DateCompleted = a.DateCompleted
                })
                .ToListAsync();

            return View("IndexFaculty", facultyResult); // Return the Faculty view
        }


        // RMCC layout logic
        [HttpGet]
        public async Task<IActionResult> IndexRmcc()
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Query to get only accomplishments created by the RMCC user
            var query = _context.Production
                .Include(a => a.CoLeadResearcher)
                .Include(a => a.LeadResearcher)
                .Include(a => a.Memberone)
                .Include(a => a.Membertwo)
                .Include(a => a.Memberthree)

                .Include(a => a.CreatedBy);

            var rmccResult = await query
                .Where(a => a.Status == "Approved" || a.CreatedById == currentUser.Id)// Only show accomplishments created by the RMCC user
                .Select(a => new RMCCAccomplishmentViewModel
                {
                    ProductionId = a.ProductionId.ToString(),
                    Year = a.Year,
                    ResearchTitle = a.ResearchTitle,
                    LeadResearcherId = a.LeadResearcherId,
                    LeadResearcherFirstName = a.LeadResearcher.FirstName,
                    LeadResearcherLastName = a.LeadResearcher.LastName,
                    CoLeadResearcherId = a.CoLeadResearcherId,
                    CoLeadResearcherFirstName = a.CoLeadResearcher.FirstName,
                    CoLeadResearcherLastName = a.CoLeadResearcher.LastName,
                    MemberoneId = a.MemberoneId,
                    MemberOneFirstName = a.Memberone.FirstName,
                    MemberOneLastName = a.Memberone.LastName,
                    MembertwoId = a.MembertwoId,
                    MemberTwoFirstName = a.Membertwo.FirstName,
                    MemberTwoLastName = a.Membertwo.LastName,
                    MemberthreeId = a.MemberthreeId,
                    MemberThreeFirstName = a.Memberthree.FirstName,
                    MemberThreeLastName = a.Memberthree.LastName,
                    College = a.College,
                    Department = a.Department,
                    BranchCampus = a.BranchCampus,
                    CreatedById = a.CreatedById,
                    CreatedByFirstName = a.CreatedBy.FirstName,
                    CreatedByLastName = a.CreatedBy.LastName,
                    CreatedOn = a.CreatedOn,
                    DateStarted = a.DateStarted,
                    DateCompleted = a.DateCompleted

                })
                .ToListAsync();

            return View("IndexRmcc", rmccResult); // Return the RMCC view
        }

        [HttpGet]
        public async Task<IActionResult> IndexDirector()
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Query to get all accomplishments
            var query = _context.Production
                .Include(a => a.CoLeadResearcher)
                .Include(a => a.LeadResearcher)
                .Include(a => a.Memberone)
                .Include(a => a.Membertwo)
                .Include(a => a.Memberthree)
                .Include(a => a.CreatedBy);

            // Retrieve all accomplishments without filtering by user
            var directorResult = await query
                .Select(a => new FacultyAccomplishmentViewModel
                {
                    ProductionId = a.ProductionId.ToString(),
                    Year = a.Year,
                    ResearchTitle = a.ResearchTitle,
                    LeadResearcherId = a.LeadResearcherId,
                    LeadResearcherFirstName = a.LeadResearcher.FirstName,
                    LeadResearcherLastName = a.LeadResearcher.LastName,
                    CoLeadResearcherId = a.CoLeadResearcherId,
                    CoLeadResearcherFirstName = a.CoLeadResearcher.FirstName,
                    CoLeadResearcherLastName = a.CoLeadResearcher.LastName,
                    MemberoneId = a.MemberoneId,
                    MemberOneFirstName = a.Memberone.FirstName,
                    MemberOneLastName = a.Memberone.LastName,
                    MembertwoId = a.MembertwoId,
                    MemberTwoFirstName = a.Membertwo.FirstName,
                    MemberTwoLastName = a.Membertwo.LastName,
                    MemberthreeId = a.MemberthreeId,
                    MemberThreeFirstName = a.Memberthree.FirstName,
                    MemberThreeLastName = a.Memberthree.LastName,
                    College = a.College,
                    Department = a.Department,
                    BranchCampus = a.BranchCampus,
                    DateStarted = a.DateStarted,
                    DateCompleted = a.DateCompleted,
                    CreatedById = a.CreatedById,
                    CreatedByFirstName = a.CreatedBy.FirstName,
                    CreatedByLastName = a.CreatedBy.LastName,
                    CreatedOn = a.CreatedOn
                })
                .ToListAsync();

            return View("IndexDirector", directorResult); // Return the Director view
        }






        public IActionResult SwitchLayout(string layout)
        {
            // Clear the previous layout if any
            HttpContext.Session.Remove("CurrentLayout");

            // Set the current layout based on the parameter
            if (layout == "RMCC" || layout == "Faculty")
            {
                HttpContext.Session.SetString("CurrentLayout", layout);
                Console.WriteLine("Switched Layout to: " + layout); // Debugging log
            }

            // Redirect to Index
            return RedirectToAction("Index");
        }





        public async Task<IActionResult> GetFilteredAccomplishments(AddAccomplishmentFilter filter)
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Start with the base query
            var accomplishments = _context.Production.AsQueryable();

            // Apply filters based on user input
            if (!string.IsNullOrEmpty(filter.ResearchTitle) || filter.Year > 0)
            {
                accomplishments = accomplishments.Where(a =>
                    (!string.IsNullOrEmpty(filter.ResearchTitle) && a.ResearchTitle.Contains(filter.ResearchTitle)) ||
                    (filter.Year > 0 && a.Year == filter.Year));
            }




            // Add other filters here...

            // Check user's role and filter accordingly
            if (User.IsInRole("Faculty"))
            {
                accomplishments = accomplishments.Where(a =>
                    a.LeadResearcherId == currentUser.Id ||
                    a.CoLeadResearcherId == currentUser.Id ||
                    a.MemberoneId == currentUser.Id ||
                    a.MembertwoId == currentUser.Id ||
                    a.MemberthreeId == currentUser.Id);
            }
            else if (User.IsInRole("RMCC"))
            {
                accomplishments = accomplishments.Where(a => a.CreatedById == currentUser.Id);
            }

            // Execute the query
            var result = await accomplishments.ToListAsync();

            // Transform the result to the expected view model type
            var viewModel = result.Select(a => new RMCCAccomplishmentViewModel
            {
                // Map properties from AddAccomplishment to RMCCAccomplishmentViewModel
                // For example:
                ResearchTitle = a.ResearchTitle,
                College = a.College,
                Department = a.Department,
                Year = a.Year
                // Add other properties as needed
            }).ToList();

            return View("IndexRmcc", viewModel); // Pass the view model to the view
        }




        // GET: AddAccomplishments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the production and related presentations
            var addAccomplishment = await _context.Production
                .Include(a => a.CoLeadResearcher)
                .Include(a => a.LeadResearcher)
                .Include(a => a.Memberone)
                .Include(a => a.Membertwo)
                .Include(a => a.Memberthree)
                .Include(a => a.AddPresentations) // Include related presentations
                .Include(a => a.AddPublications)
                .Include(a => a.AddUtilizations)
                .Include(a => a.AddPatents)
                .Include(a => a.AddCitations)
                .FirstOrDefaultAsync(m => m.ProductionId == id);

            if (addAccomplishment == null)
            {
                return NotFound();
            }
            // Create a view model to pass to the view
            var viewModel = new FacultyDetailsViewModel
            {
                Production = addAccomplishment,
                Presentations = addAccomplishment.AddPresentations.ToList(),
                Publications = addAccomplishment.AddPublications.ToList(),
                // Add other properties as needed
            };


            return View(addAccomplishment);
        }


        // GET: AddAccomplishments/Details/5
        [HttpGet]

        public async Task<IActionResult> DetailsFaculty(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the production and related presentations
            var addAccomplishment = await _context.Production
                .Include(a => a.CoLeadResearcher)
                .Include(a => a.LeadResearcher)
                .Include(a => a.Memberone)
                .Include(a => a.Membertwo)
                .Include(a => a.Memberthree)
                .Include(a => a.AddPresentations) // Include related presentations
                .Include(a => a.AddPublications)
                .Include(a => a.AddUtilizations)
                .Include(a => a.AddPatents)
                .Include(a => a.AddCitations)
                .FirstOrDefaultAsync(m => m.ProductionId == id);

            if (addAccomplishment == null)
            {
                return NotFound();
            }

            return View(addAccomplishment);
        }
        [HttpGet]

        public async Task<IActionResult> DetailsDirector(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the production and related presentations
            var addAccomplishment = await _context.Production
                .Include(a => a.CoLeadResearcher)
                .Include(a => a.LeadResearcher)
                .Include(a => a.Memberone)
                .Include(a => a.Membertwo)
                .Include(a => a.Memberthree)
                .Include(a => a.AddPresentations) // Include related presentations
                .Include(a => a.AddPublications)
                .Include(a => a.AddUtilizations)
                .Include(a => a.AddPatents)
                .Include(a => a.AddCitations)
                .FirstOrDefaultAsync(m => m.ProductionId == id);

            if (addAccomplishment == null)
            {
                return NotFound();
            }

            return View(addAccomplishment);
        }

        [Authorize(Roles = "RMCC")]
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            // Get current user to ensure they're RMCC
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Define college options
            var collegeOptions = new List<string>
    {
        "College of Engineering",
        "College of Arts and Sciences",
        "College of Business Administration",
        "College of Education"
    };
            ViewBag.CollegeOptions = new SelectList(collegeOptions);

            // Define department options
            var departmentOptions = new List<string>
    {
        "Information Technology",
        "Arts and Sciences",
        "Business Administration",
        "Education"
    };
            ViewBag.DepartmentOptions = new SelectList(departmentOptions);

            // Define branchcampus options
            var BranchCampusOptions = new List<string>
    {
        "PUPManila,Sta-Mesa",
        "PUP-Taguig",
        "PUP-QuezonCity",

    };
            ViewBag.BranchCampusOptions = new SelectList(BranchCampusOptions);

            // Get all faculty users
            var facultyUsers = await _userManager.GetUsersInRoleAsync("Faculty");
            var facultyList = facultyUsers.Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName} ({u.Email})" // Adjust based on your ApplicationUser properties
            });

            // Create SelectLists for researchers with proper display names
            ViewData["LeadResearcherId"] = new SelectList(facultyList, "Id", "FullName");
            ViewData["CoLeadResearcherId"] = new SelectList(facultyList, "Id", "FullName");
            ViewData["MemberoneId"] = new SelectList(facultyList, "Id", "FullName");
            ViewData["MembertwoId"] = new SelectList(facultyList, "Id", "FullName");
            ViewData["MemberthreeId"] = new SelectList(facultyList, "Id", "FullName");

            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RMCC")]
        public async Task<IActionResult> Create(
     [Bind("ProductionId,Year, ResearchTitle,LeadResearcherId,CoLeadResearcherId,MemberoneId,MembertwoId,MemberthreeId,BranchCampus, College,Department,FundingAgency,FundingAmount,DateStarted,DateCompleted,IsStudentInvolved")]
    AddAccomplishment addAccomplishment,
     IFormFile? RequiredFile1Data,
     IFormFile? RequiredFile2Data,
     IFormFile? RequiredFile3Data,
     IFormFile? ConditionalFileData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the current RMCC user
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    // Set creation metadata - just store the name
                    addAccomplishment.ProductionId = Guid.NewGuid().ToString();
                    addAccomplishment.CreatedById = currentUser.Id;
                    addAccomplishment.Notes = currentUser.FirstName + " " + currentUser.LastName; // Just store the name

                    // Handle RequiredFile1
                    if (RequiredFile1Data != null && RequiredFile1Data.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await RequiredFile1Data.CopyToAsync(memoryStream);
                        addAccomplishment.RequiredFile1Data = memoryStream.ToArray();
                        addAccomplishment.RequiredFile1Name = RequiredFile1Data.FileName;
                    }

                    // Handle RequiredFile2
                    if (RequiredFile2Data != null && RequiredFile2Data.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await RequiredFile2Data.CopyToAsync(memoryStream);
                        addAccomplishment.RequiredFile2Data = memoryStream.ToArray();
                        addAccomplishment.RequiredFile2Name = RequiredFile2Data.FileName;
                    }

                    // Handle RequiredFile3
                    if (RequiredFile3Data != null && RequiredFile3Data.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await RequiredFile3Data.CopyToAsync(memoryStream);
                        addAccomplishment.RequiredFile3Data = memoryStream.ToArray();
                        addAccomplishment.RequiredFile3Name = RequiredFile3Data.FileName;
                    }

                    // Handle ConditionalFile
                    if (ConditionalFileData != null && ConditionalFileData.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await ConditionalFileData.CopyToAsync(memoryStream);
                        addAccomplishment.ConditionalFileData = memoryStream.ToArray();
                        addAccomplishment.ConditionalFileName = ConditionalFileData.FileName;
                    }
                    addAccomplishment.CreatedOn = DateTime.Now;
                    _context.Add(addAccomplishment);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(IndexRmcc));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the accomplishment.");
                }
            }

            // If we got this far, something failed, redisplay form
            // Repopulate all dropdown lists
            var collegeOptions = new List<string>
    {
        "College of Engineering",
        "College of Arts and Sciences",
        "College of Business Administration",
        "College of Education"
    };
            ViewBag.CollegeOptions = new SelectList(collegeOptions);

            var departmentOptions = new List<string>
    {
        "Information Technology",
        "Arts and Sciences",
        "Business Administration",
        "Education"
    };
            ViewBag.DepartmentOptions = new SelectList(departmentOptions);


            // Define branchcampus options
            var BranchCampusOptions = new List<string>
    {
        "PUPManila,Sta-Mesa",
        "PUP-Taguig",
        "PUP-QuezonCity",

    };
            ViewBag.BranchCampusOptions = new SelectList(BranchCampusOptions);


            // Repopulate faculty lists
            var facultyUsers = await _userManager.GetUsersInRoleAsync("Faculty");
            var facultyList = facultyUsers.Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName} ({u.Email})"
            });

            ViewData["LeadResearcherId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.LeadResearcherId);
            ViewData["CoLeadResearcherId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.CoLeadResearcherId);
            ViewData["MemberoneId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.MemberoneId);
            ViewData["MembertwoId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.MembertwoId);
            ViewData["MemberthreeId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.MemberthreeId);

            return View(addAccomplishment);
        }

        // GET: AddAccomplishments/Edit/5
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addAccomplishment = await _context.Production.FindAsync(id);
            if (addAccomplishment == null)
            {
                return NotFound();
            }

            // Fetch faculty list and populate dropdowns
            var facultyUsers = await _userManager.GetUsersInRoleAsync("Faculty");
            var facultyList = facultyUsers.Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName} ({u.Email})"
            });

            ViewData["LeadResearcherId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.LeadResearcherId);
            ViewData["CoLeadResearcherId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.CoLeadResearcherId);
            ViewData["MemberoneId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.MemberoneId);
            ViewData["MembertwoId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.MembertwoId);
            ViewData["MemberthreeId"] = new SelectList(facultyList, "Id", "FullName", addAccomplishment.MemberthreeId);

            return View(addAccomplishment);

        }

        // POST: AddAccomplishments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProductionId,ResearchTitle,LeadResearcherId,CoLeadResearcherId,MemberoneId,MembertwoId,MemberthreeId,College,Department,FundingAgency,FundingAmount,DateStarted,DateCompleted,RequiredFile1Data,RequiredFile2Data,ConditionalFileData,RequiredFile1Name,RequiredFile2Name,ConditionalFileName,IsStudentInvolved,Notes")] AddAccomplishment addAccomplishment)
        {
            if (id != addAccomplishment.ProductionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addAccomplishment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddAccomplishmentExists(addAccomplishment.ProductionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexRmcc));
            }
            return RedirectToAction(nameof(IndexRmcc));
        }


            // GET: AddAccomplishments/Delete/5
            public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addAccomplishment = await _context.Production
                .Include(a => a.CoLeadResearcher)
                .Include(a => a.LeadResearcher)
                .Include(a => a.Memberone)
                .Include(a => a.Memberthree)
                .Include(a => a.Membertwo)
                .FirstOrDefaultAsync(m => m.ProductionId == id);
            if (addAccomplishment == null)
            {
                return NotFound();
            }

            return View(addAccomplishment);
        }

        // POST: AddAccomplishments/Delete/5
        [HttpPost("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var addAccomplishment = await _context.Production.FindAsync(id);
            if (addAccomplishment != null)
            {
                _context.Production.Remove(addAccomplishment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddAccomplishmentExists(string id)
        {
            return _context.Production.Any(e => e.ProductionId == id);
        }


        [HttpGet("DownloadFile")]
        public IActionResult DownloadFile(string id, string fileType)
        {
            // Retrieve the production and presentation data
            var accomplishment = _context.Production.Find(id);



            if (accomplishment == null) return NotFound("Accomplishment not found.");


            byte[] fileData;
            string fileName;

            // Determine which file to return based on fileType
            switch (fileType)
            {
                case "RequiredFile1":
                    fileData = accomplishment.RequiredFile1Data;
                    fileName = accomplishment.RequiredFile1Name;
                    break;
                case "RequiredFile2":
                    fileData = accomplishment.RequiredFile2Data;
                    fileName = accomplishment.RequiredFile2Name;
                    break;
                case "RequiredFile3":
                    fileData = accomplishment.RequiredFile3Data;
                    fileName = accomplishment.RequiredFile3Name;
                    break;
                case "ConditionalFile":
                    fileData = accomplishment.ConditionalFileData;
                    fileName = accomplishment.ConditionalFileName;
                    break;


                default:
                    return BadRequest("Invalid file type specified.");
            }




            if (fileData == null || fileName == null)
                return NotFound("File data not found.");

            // Return the file to the client
            return File(fileData, "application/octet-stream", fileName);
        }


        [HttpGet("DownloadFilePresentation")]
        public IActionResult DownloadFilePresentation(string id, string fileType)
        {
            // Retrieve the production and presentation data
            var presentation = _context.Presentation.Find(id);


            if (presentation == null) return NotFound("Presentation not found.");


            byte[] fileData;
            string fileName;

            // Determine which file to return based on fileType
            switch (fileType)
            {

                case "CertificateofParticipationFileData":
                    fileData = presentation.CertificateofParticipationFileData;
                    fileName = presentation.CertificateofParticipationFileName;
                    break;
                default:
                    return BadRequest("Invalid file type specified.");
            }

            if (fileData == null || fileName == null)
                return NotFound("File data not found.");

            // Return the file to the client
            return File(fileData, "application/octet-stream", fileName);
        }

        [HttpGet("DownloadFilePublication")]
        public IActionResult DownloadFilePublication(string id, string fileType)
        {
            // Retrieve the production and presentation data
            var publication = _context.Publication.Find(id);


            if (publication == null) return NotFound("Publication not found.");


            byte[] fileData;
            string fileName;

            // Determine which file to return based on fileType
            switch (fileType)
            {

                case "ManuscriptJournalFile":
                    fileData = publication.ManuscriptJournalData;
                    fileName = publication.ManuscriptJournalFileName;
                    break;
                default:
                    return BadRequest("Invalid file type specified.");
            }

            if (fileData == null || fileName == null)
                return NotFound("File data not found.");

            // Return the file to the client
            return File(fileData, "application/octet-stream", fileName);
        }

        [HttpGet("DownloadFilePatent")]
        public IActionResult DownloadFilePatent(string id, string fileType)
        {
            // Retrieve the production and presentation data
            var patent = _context.Patent.Find(id);


            if (patent == null) return NotFound("Patent not found.");


            byte[] fileData;
            string fileName;

            // Determine which file to return based on fileType
            switch (fileType)
            {

                case "ApplicationFormFile":
                    fileData = patent.ApplicationFormData;
                    fileName = patent.ApplicationFormFileName;
                    break;
                default:
                    return BadRequest("Invalid file type specified.");
            }

            if (fileData == null || fileName == null)
                return NotFound("File data not found.");

            // Return the file to the client
            return File(fileData, "application/octet-stream", fileName);
        }


        [HttpGet("DownloadFileUtilization")]
        public IActionResult DownloadFileUtilization(string id, string fileType)
        {
            // Retrieve the production and presentation data
            var utilization = _context.Utilization.Find(id);


            if (utilization == null) return NotFound("Utilization not found.");


            byte[] fileData;
            string fileName;

            // Determine which file to return based on fileType
            switch (fileType)
            {

                case "CertificateofUtilizationFile":
                    fileData = utilization.CertificateofUtilizationData;
                    fileName = utilization.CertificateofUtilizationFileName;
                    break;
                default:
                    return BadRequest("Invalid file type specified.");
            }

            if (fileData == null || fileName == null)
                return NotFound("File data not found.");

            // Return the file to the client
            return File(fileData, "application/octet-stream", fileName);
        }

        [HttpGet("DownloadFileCitation")]
        public IActionResult DownloadFileCitation(string id, string fileType)
        {
            // Retrieve the production and presentation data
            var citation = _context.Citation.Find(id);


            if (citation == null) return NotFound("Citation not found.");


            byte[] fileData;
            string fileName;

            // Determine which file to return based on fileType
            switch (fileType)
            {

                case "CitationProof":
                    fileData = citation.CitationProofData;
                    fileName = citation.CitationProofFileName;
                    break;
                default:
                    return BadRequest("Invalid file type specified.");
            }

            if (fileData == null || fileName == null)
                return NotFound("File data not found.");

            // Return the file to the client
            return File(fileData, "application/octet-stream", fileName);
        }


        [Authorize(Roles = "RMCC")]
        [HttpGet("AddPresentation")]
        public async Task<IActionResult> AddPresentation(string productionId)
        {
            if (string.IsNullOrEmpty(productionId))
            {
                return BadRequest("Production ID is required.");
            }

            // Verify that the production exists
            var production = await _context.Production.FindAsync(productionId);
            if (production == null)
            {
                return NotFound("Production not found.");
            }

            // Prepopulate the presentation form with the ProductionId
            var model = new AddPresentation
            {
                ProductionId = productionId
            };



            return View(model);
        }

        [HttpPost("AddPresentation")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RMCC")]
        public async Task<IActionResult> AddPresentation(
     [Bind("ConferenceId, ProductionId, OrganizerOne, OrganizerTwo, PresenterOne, PresenterTwo, PresenterThree, PresenterFour, PresenterFive, DateofPresentation, Level, Venue, CertificateofParticipationFileData, CertificateofParticipationFileName")]
    AddPresentation addPresentation,
     IFormFile? CertificateofParticipationFileData)
        {
            if (!ModelState.IsValid)
            {
                return View(addPresentation);
            }

            try
            {
                // Verify that the linked production exists
                var production = await _context.Production.FindAsync(addPresentation.ProductionId);
                if (production == null)
                {
                    ModelState.AddModelError("", "Invalid Production ID.");
                    return View(addPresentation);
                }

                // Handle the Certificate of Participation file
                if (CertificateofParticipationFileData != null && CertificateofParticipationFileData.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await CertificateofParticipationFileData.CopyToAsync(memoryStream);
                    addPresentation.CertificateofParticipationFileData = memoryStream.ToArray();
                    addPresentation.CertificateofParticipationFileName = CertificateofParticipationFileData.FileName;

                }



                // Add the new presentation
                _context.Presentation.Add(addPresentation);
                await _context.SaveChangesAsync();


                return RedirectToAction("Details", new { id = addPresentation.ProductionId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the presentation: " + ex.Message);
                return View(addPresentation);
            }
        }

        [Authorize(Roles = "RMCC")]
        [HttpGet("AddPublication")]
        public async Task<IActionResult> AddPublication(string productionId)
        {
            if (string.IsNullOrEmpty(productionId))
            {
                return BadRequest("Production ID is required.");
            }

            // Verify that the production exists
            var production = await _context.Production.FindAsync(productionId);
            if (production == null)
            {
                return NotFound("Production not found.");
            }

            // Prepopulate the presentation form with the ProductionId
            var model = new AddPublication
            {
                ProductionId = productionId
            };



            return View(model);
        }


        [HttpPost("AddPublication")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RMCC")]
        public async Task<IActionResult> AddPublication(
    [Bind("ProductionId,ArticleTitle,PublicationDate,JournalPubTitle,DocumentType,VolnoIssueNo,IssnIsbnEssn,DOI,IndexJournal,SuppDocs,Link,ManuscriptJournalData,ManuscriptJournalFileName")] AddPublication addPublication,
    IFormFile? ManuscriptJournalFile)
        {
            if (!ModelState.IsValid)
            {
                return View(addPublication);
            }

            try
            {
                // Verify that the linked production exists
                var production = await _context.Production.FindAsync(addPublication.ProductionId);
                if (production == null)
                {
                    ModelState.AddModelError("", "Invalid Production ID.");
                    return View(addPublication);
                }

                // Handle the ManuscriptJournalFile
                if (ManuscriptJournalFile != null && ManuscriptJournalFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await ManuscriptJournalFile.CopyToAsync(memoryStream);
                    addPublication.ManuscriptJournalData = memoryStream.ToArray();
                    addPublication.ManuscriptJournalFileName = ManuscriptJournalFile.FileName;
                }

                // Add the new publication
                _context.Publication.Add(addPublication);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = addPublication.ProductionId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the publication: " + ex.Message);
                return View(addPublication);
            }


        }



        [Authorize(Roles = "RMCC")]
        [HttpGet("AddUtilization")]
        public async Task<IActionResult> AddUtilization(string productionId)
        {
            if (string.IsNullOrEmpty(productionId))
            {
                return BadRequest("Production ID is required.");
            }

            // Verify that the production exists
            var production = await _context.Production.FindAsync(productionId);
            if (production == null)
            {
                return NotFound("Production not found.");
            }

            // Prepopulate the presentation form with the ProductionId
            var model = new AddUtilization
            {
                ProductionId = productionId
            };



            return View(model);
        }

        [HttpPost("AddUtilization")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RMCC")]
        public async Task<IActionResult> AddUtilization(
     [Bind("ProductionId, SubmittedOn")] AddUtilization addUtilization,
     IFormFile? CertificateofUtilizationFile)
        {
            if (!ModelState.IsValid)
            {
                return View(addUtilization);
            }

            try
            {
                // Verify that the linked production exists
                var production = await _context.Production.FindAsync(addUtilization.ProductionId);
                if (production == null)
                {
                    ModelState.AddModelError("", "Invalid Production ID.");
                    return View(addUtilization);
                }

                // Process the Certificate of Utilization file
                if (CertificateofUtilizationFile != null && CertificateofUtilizationFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await CertificateofUtilizationFile.CopyToAsync(memoryStream);
                    addUtilization.CertificateofUtilizationData = memoryStream.ToArray();
                    addUtilization.CertificateofUtilizationFileName = CertificateofUtilizationFile.FileName;
                }
                else
                {
                    ModelState.AddModelError("", "File is required.");
                    return View(addUtilization);
                }

                // Add the new utilization
                _context.Utilization.Add(addUtilization);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = addUtilization.ProductionId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the utilization: " + ex.Message);
                return View(addUtilization);
            }
        }


        [Authorize(Roles = "RMCC")]
        [HttpGet("AddPatent")]
        public async Task<IActionResult> AddPatent(string productionId)
        {
            if (string.IsNullOrEmpty(productionId))
            {
                return BadRequest("Production ID is required.");
            }

            // Verify that the production exists
            var production = await _context.Production.FindAsync(productionId);
            if (production == null)
            {
                return NotFound("Production not found.");
            }

            // Prepopulate the presentation form with the ProductionId
            var model = new AddPatent
            {
                ProductionId = productionId
            };



            return View(model);
        }

        [HttpPost("AddPatent")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RMCC")]
        public async Task<IActionResult> AddPatent(
        [Bind("ProductionId,PatentNo,ApplicationFormData,ApplicationFormFileName")] AddPatent addPatent,
        IFormFile? ApplicationFormFile)
        {
            if (!ModelState.IsValid)
            {
                return View(addPatent);
            }

            try
            {
                // Verify that the linked production exists
                var production = await _context.Production.FindAsync(addPatent.ProductionId);
                if (production == null)
                {
                    ModelState.AddModelError("", "Invalid Production ID.");
                    return View(addPatent);
                }

                // Process the Certificate of Utilization file
                if (ApplicationFormFile != null && ApplicationFormFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await ApplicationFormFile.CopyToAsync(memoryStream);
                    addPatent.ApplicationFormData = memoryStream.ToArray();
                    addPatent.ApplicationFormFileName = ApplicationFormFile.FileName;
                }
                else
                {
                    ModelState.AddModelError("", "File is required.");
                    return View(addPatent);
                }

                // Add the new utilization
                _context.Patent.Add(addPatent);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = addPatent.ProductionId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the utilization: " + ex.Message);
                return View(addPatent);
            }
        }


        [Authorize(Roles = "RMCC")]
        [HttpGet("AddCitation")]
        public async Task<IActionResult> AddCitation(string productionId)
        {
            if (string.IsNullOrEmpty(productionId))
            {
                return BadRequest("Production ID is required.");
            }

            // Verify that the production exists
            var production = await _context.Production.FindAsync(productionId);
            if (production == null)
            {
                return NotFound("Production not found.");
            }

            // Populate the dropdowns for researchers
            ViewData["CoLeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["LeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["MemberoneId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["MemberthreeId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["MembertwoId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");

            // Prepopulate the form with the ProductionId
            var model = new AddCitation
            {
                ProductionId = productionId
            };

            return View(model);
        }

        [HttpPost("AddCitation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCitation([Bind("ProductionId,ArticleTitle,LeadResearcherId,CoLeadResearcherId,MemberoneId,MembertwoId,MemberthreeId,OriginalArticlePublished,NewPublicationTitle,AuthorsofNewArticle,NewArticlePublished,VolNoIssueNo,Pages,YearofPublication,Indexing")] AddCitation addCitation, IFormFile CitationProof)
        {
            // Manual validation for required fields
            if (string.IsNullOrEmpty(addCitation.ProductionId))
            {
                ModelState.AddModelError("ProductionId", "Production ID is required.");
            }

            // Verify ProductionId exists
            var production = await _context.Production.FindAsync(addCitation.ProductionId);
            if (production == null)
            {
                ModelState.AddModelError("ProductionId", "Invalid Production ID.");
            }

            // If a file is uploaded, save its content
            if (CitationProof != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await CitationProof.CopyToAsync(memoryStream);
                    addCitation.CitationProofData = memoryStream.ToArray();
                    addCitation.CitationProofFileName = CitationProof.FileName;
                }
            }

            // Check ModelState errors explicitly if validation above added errors
            if (!ModelState.Values.Any(x => x.Errors.Count > 0))
            {
                _context.Add(addCitation);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(AddCitation), new { productionId = addCitation.ProductionId });
            }

            // Populate dropdowns again in case of error
            ViewData["CoLeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.CoLeadResearcherId);
            ViewData["LeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.LeadResearcherId);
            ViewData["MemberoneId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MemberoneId);
            ViewData["MemberthreeId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MemberthreeId);
            ViewData["MembertwoId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MembertwoId);
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.ProductionId);

            return View(addCitation);
        }


        //[Authorize(Roles = "RMCC")]
        //public async Task<IActionResult> ExportToExcel(DateTime? startDate = null, DateTime? endDate = null)
        //{
        //    try
        //    {
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //        using var excelPackage = new ExcelPackage();

        //        // Create a worksheet for each category
        //        var productionSheet = excelPackage.Workbook.Worksheets.Add("Productions");
        //        var presentationSheet = excelPackage.Workbook.Worksheets.Add("Presentations");
        //        var publicationSheet = excelPackage.Workbook.Worksheets.Add("Publications");
        //        var patentSheet = excelPackage.Workbook.Worksheets.Add("Patents");
        //        var utilizationSheet = excelPackage.Workbook.Worksheets.Add("Utilizations");

        //        // Fill each worksheet with data
        //        await FillProductions(productionSheet, startDate, endDate);
        //        await FillPresentations(presentationSheet, startDate, endDate);
        //        await FillPublications(publicationSheet, startDate, endDate);
        //        await FillPatents(patentSheet, startDate, endDate);
        //        await FillUtilizations(utilizationSheet, startDate, endDate);

        //        var excelData = excelPackage.GetAsByteArray();
        //        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Accomplishments.xlsx");
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "An error occurred while generating the Excel file: " + ex.Message);
        //        return View("Error");
        //    }
        //}

        //private async Task FillProductions(ExcelWorksheet sheet, DateTime? startDate, DateTime? endDate)
        //{
        //    // Set headers
        //    sheet.Cells[1, 1].Value = "Production ID";
        //    sheet.Cells[1, 2].Value = "Research Title";
        //    sheet.Cells[1, 3].Value = "Year";
        //    sheet.Cells[1, 4].Value = "Lead Researcher";
        //    sheet.Cells[1, 5].Value = "Co-Lead Researcher";
        //    sheet.Cells[1, 6].Value = "Members";
        //    sheet.Cells[1, 7].Value = "StartDate";
        //    sheet.Cells[1, 8].Value = "CompletedDate";

        //    var row = 2;
        //    var productions = await _context.Production
        //        .Include(p => p.LeadResearcher)
        //        .Include(p => p.CoLeadResearcher)
        //        .Include(p => p.Memberone)
        //        .Include(p => p.Membertwo)
        //        .Include(p => p.Memberthree)
        //        .Where(p => (!startDate.HasValue || p.DateStarted >= startDate.Value) &&
        //                    (!endDate.HasValue || p.DateCompleted <= endDate.Value))
        //        .ToListAsync();

        //    foreach (var production in productions)
        //    {
        //        sheet.Cells[row, 1].Value = production.ProductionId;
        //        sheet.Cells[row, 2].Value = production.ResearchTitle;
        //        sheet.Cells[row, 3].Value = production.Year;
        //        sheet.Cells[row, 4].Value = $"{production.LeadResearcher.FirstName} {production.LeadResearcher.LastName}";
        //        sheet.Cells[row, 5].Value = $"{production.CoLeadResearcher.FirstName} {production.CoLeadResearcher.LastName}";
        //        sheet.Cells[row, 6].Value = $"{production.Memberone.FirstName} {production.Memberone.LastName}, {production.Membertwo.FirstName} {production.Membertwo.LastName}, {production.Memberthree.FirstName} {production.Memberthree.LastName}";
        //        sheet.Cells[row, 7].Value = production.DateStarted.ToString("MM/dd/yyyy") ?? "";
        //        sheet.Cells[row, 8].Value = production.DateCompleted?.ToString("MM/dd/yyyy") ?? "";
        //        row++;
        //    }

        //    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        //}

        //private async Task FillPresentations(ExcelWorksheet sheet, DateTime? startDate, DateTime? endDate)
        //{
        //    // Set headers

        //    sheet.Cells[1, 1].Value = "Date of Presentation";
        //    sheet.Cells[1, 2].Value = "Venue";
        //    sheet.Cells[1, 3].Value = "Organizer";

        //    var row = 2;
        //    var presentations = await _context.Presentation
        //        .Where(pr => (!startDate.HasValue || pr.DateofPresentation >= startDate.Value) &&
        //                     (!endDate.HasValue || pr.DateofPresentation <= endDate.Value))
        //        .ToListAsync();

        //    foreach (var presentation in presentations)
        //    {

        //        sheet.Cells[row, 1].Value = presentation.DateofPresentation?.ToString("MM/dd/yyyy");
        //        sheet.Cells[row, 2].Value = presentation.Venue;
        //        sheet.Cells[row, 3].Value = $"{presentation.OrganizerOne}, {presentation.OrganizerTwo}";
        //        row++;
        //    }

        //    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        //}

        //private async Task FillPublications(ExcelWorksheet sheet, DateTime? startDate, DateTime? endDate)
        //{
        //    // Set headers

        //    sheet.Cells[1, 1].Value = "Article Title";
        //    sheet.Cells[1, 2].Value = "Publication Date";
        //    sheet.Cells[1, 3].Value = "Journal Title";
        //    // Add more headers as needed

        //    var row = 2;
        //    var publications = await _context.Publication
        //        .Where(pub => (!startDate.HasValue || pub.PublicationDate >= startDate.Value) &&
        //                      (!endDate.HasValue || pub.PublicationDate <= endDate.Value))
        //        .ToListAsync();

        //    foreach (var publication in publications)
        //    {

        //        sheet.Cells[row, 1].Value = publication.ArticleTitle;
        //        sheet.Cells[row, 2].Value = publication.PublicationDate?.ToString("MM/dd/yyyy");
        //        sheet.Cells[row, 3].Value = publication.JournalPubTitle;
        //        // Add more fields as needed
        //        row++;
        //    }

        //    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        //}

        //private async Task FillPatents(ExcelWorksheet sheet, DateTime? startDate, DateTime? endDate)
        //{
        //    // Set headers
        //    sheet.Cells[1, 1].Value = "Patent ID";
        //    sheet.Cells[1, 2].Value = "Patent Number";
        //    sheet.Cells[1, 3].Value = "Application Form Filename";

        //    var row = 2;
        //    var patents = await _context.Patent
        //    .ToListAsync();

        //    foreach (var patent in patents)
        //    {
        //        sheet.Cells[row, 1].Value = patent.patentId;
        //        sheet.Cells[row, 2].Value = patent.PatentNo;
        //        sheet.Cells[row, 3].Value = patent.ApplicationFormFileName;
        //        row++;
        //    }

        //    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        //}

        //private async Task FillUtilizations(ExcelWorksheet sheet, DateTime? startDate, DateTime? endDate)
        //{
        //    // Set headers
        //    sheet.Cells[1, 1].Value = "Utilization ID";
        //    sheet.Cells[1, 2].Value = "Submitted On";
        //    sheet.Cells[1, 3].Value = "Certificate Filename";

        //    var row = 2;
        //    var utilizations = await _context.Utilization
        //        .Where(u => (!startDate.HasValue || u.SubmittedOn >= startDate.Value) &&
        //                    (!endDate.HasValue || u.SubmittedOn <= endDate.Value))
        //        .ToListAsync();

        //    foreach (var utilization in utilizations)
        //    {
        //        sheet.Cells[row, 1].Value = utilization.UtilizationId;
        //        sheet.Cells[row, 2].Value = utilization.SubmittedOn.ToString("MM/dd/yyyy");
        //        sheet.Cells[row, 3].Value = utilization.CertificateofUtilizationFileName;
        //        row++;
        //    }

        //    sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        //}

        [Authorize(Roles = "RMCC, Director")]
        public async Task<IActionResult> ExporttoExcel(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // Set the license context for EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Create a new Excel package
                using var excelPackage = new ExcelPackage();

                // Add a worksheet for unified data
                var unifiedSheet = excelPackage.Workbook.Worksheets.Add("Production, Presentations, Publications, Patents, and Utilizations");

                // Set header titles
                unifiedSheet.Cells[1, 1].Value = "Production ID";
                unifiedSheet.Cells[1, 2].Value = "Research Title";
                unifiedSheet.Cells[1, 3].Value = "Year";
                unifiedSheet.Cells[1, 4].Value = "Lead Researcher";
                unifiedSheet.Cells[1, 5].Value = "Co-Lead Researcher";
                unifiedSheet.Cells[1, 6].Value = "Members";
                unifiedSheet.Cells[1, 7].Value = "StartDate";
                unifiedSheet.Cells[1, 8].Value = "CompletedDate";

                unifiedSheet.Cells[1, 9].Value = "Date of Presentation";
                unifiedSheet.Cells[1, 10].Value = "Venue";
                unifiedSheet.Cells[1, 11].Value = "Organizer";

                // Publications headers
                unifiedSheet.Cells[1, 12].Value = "Article Title";
                unifiedSheet.Cells[1, 13].Value = "Publication Date";
                unifiedSheet.Cells[1, 14].Value = "Journal Title";
                unifiedSheet.Cells[1, 15].Value = "Document Type";
                unifiedSheet.Cells[1, 16].Value = "Volume/Issue";
                unifiedSheet.Cells[1, 17].Value = "ISSN/ISBN";
                unifiedSheet.Cells[1, 18].Value = "DOI";
                unifiedSheet.Cells[1, 19].Value = "Index Journal";
                unifiedSheet.Cells[1, 20].Value = "Supporting Document";
                unifiedSheet.Cells[1, 21].Value = "Link";

                // Patents headers
                unifiedSheet.Cells[1, 22].Value = "Patent ID";
                unifiedSheet.Cells[1, 23].Value = "Patent Number";
                unifiedSheet.Cells[1, 24].Value = "Application Form Filename";

                // Utilizations headers
                unifiedSheet.Cells[1, 25].Value = "Utilization ID";
                unifiedSheet.Cells[1, 26].Value = "Submitted On";
                unifiedSheet.Cells[1, 27].Value = "Certificate Filename";


                // Apply yellow color to Production header
                var productionHeaderRange = unifiedSheet.Cells[1, 1, 1, 12]; // Columns 1 to 12
                productionHeaderRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                productionHeaderRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                // Apply orange color to Presentation header
                var presentationHeaderRange = unifiedSheet.Cells[2, 1, 2, 12]; // Columns 1 to 12 in the next row
                presentationHeaderRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                presentationHeaderRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);


                var publicationHeaderRange = unifiedSheet.Cells[1, 12, 1, 22]; // Publication columns
                publicationHeaderRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                publicationHeaderRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                // Patent header range
                var patentHeaderRange = unifiedSheet.Cells[1, 23, 1, 25]; // Patent columns
                patentHeaderRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                patentHeaderRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);

                // Utilization header range
                var utilizationHeaderRange = unifiedSheet.Cells[1, 26, 1, 28]; // Utilization columns
                utilizationHeaderRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                utilizationHeaderRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Purple);



                var row = 2;

                // Fetch Productions and related data
                var productions = await _context.Production
                    .Include(p => p.LeadResearcher)
                    .Include(p => p.CoLeadResearcher)
                    .Include(p => p.Memberone)
                    .Include(p => p.Membertwo)
                    .Include(p => p.Memberthree)
                    .Include(p => p.CreatedBy)
                    .Where(p => !startDate.HasValue || p.DateStarted >= startDate.Value)
                    .Where(p => !endDate.HasValue || p.DateCompleted <= endDate.Value)
                    .ToListAsync();

                foreach (var production in productions)
                {
                    // Fetch related items
                    var presentations = await _context.Presentation
                        .Where(pr => pr.ProductionId == production.ProductionId)
                        .ToListAsync();

                    var publications = await _context.Publication
                        .Where(pub => pub.ProductionId == production.ProductionId)
                        .ToListAsync();

                    var patents = await _context.Patent
                        .Where(p => p.ProductionId == production.ProductionId)
                        .ToListAsync();

                    var utilizations = await _context.Utilization
                        .Where(u => u.ProductionId == production.ProductionId)
                        .ToListAsync();

                    // Determine max rows for related items
                    var maxRows = Math.Max(presentations.Count, Math.Max(publications.Count, Math.Max(patents.Count, utilizations.Count)));

                    for (int i = 0; i < maxRows; i++)
                    {
                        // Write Production data
                        unifiedSheet.Cells[row, 1].Value = production.ProductionId;
                        unifiedSheet.Cells[row, 2].Value = production.ResearchTitle;
                        unifiedSheet.Cells[row, 3].Value = production.Year;
                        unifiedSheet.Cells[row, 4].Value = $"{production.LeadResearcher.FirstName} {production.LeadResearcher.LastName}";
                        unifiedSheet.Cells[row, 5].Value = $"{production.CoLeadResearcher.FirstName} {production.CoLeadResearcher.LastName}";
                        unifiedSheet.Cells[row, 6].Value = $"{production.Memberone.FirstName} {production.Memberone.LastName}, {production.Membertwo.FirstName} {production.Membertwo.LastName}, {production.Memberthree.FirstName} {production.Memberthree.LastName}";

                        // Handle nullable DateTime
                        unifiedSheet.Cells[row, 7].Value = production.DateStarted.ToString("MM/dd/yyyy") ?? ""; // Use null-coalescing operator
                        unifiedSheet.Cells[row, 8].Value = production.DateCompleted?.ToString("MM/dd/yyyy") ?? ""; // Use null-coalescing operator

                        // Other data writing logic...

                        // Presentation data
                        if (i < presentations.Count)
                        {
                            var presentation = presentations[i];
                            unifiedSheet.Cells[row, 9].Value = presentation.DateofPresentation?.ToString("MM/dd/yyyy");
                            unifiedSheet.Cells[row, 10].Value = presentation.Venue;
                            unifiedSheet.Cells[row, 11].Value = $"{presentation.OrganizerOne}, {presentation.OrganizerTwo}";
                        }
                        else
                        {
                            unifiedSheet.Cells[row, 9].Value = ""; // Leave blank if no presentation
                            unifiedSheet.Cells[row, 10].Value = "";
                            unifiedSheet.Cells[row, 11].Value = "";
                        }

                        // Publication data
                        if (i < publications.Count)
                        {
                            var publication = publications[i];
                            unifiedSheet.Cells[row, 12].Value = publication.ArticleTitle;
                            unifiedSheet.Cells[row, 13].Value = publication.PublicationDate?.ToString("MM/dd/yyyy");
                            unifiedSheet.Cells[row, 14].Value = publication.JournalPubTitle;
                            unifiedSheet.Cells[row, 15].Value = publication.DocumentType;
                            unifiedSheet.Cells[row, 16].Value = publication.VolnoIssueNo;
                            unifiedSheet.Cells[row, 17].Value = publication.IssnIsbnEssn;
                            unifiedSheet.Cells[row, 18].Value = publication.DOI;
                            unifiedSheet.Cells[row, 19].Value = publication.IndexJournal;
                            unifiedSheet.Cells[row, 20].Value = publication.SuppDocs;
                            unifiedSheet.Cells[row, 21].Value = publication.Link;
                        }
                        else
                        {
                            unifiedSheet.Cells[row, 12].Value = ""; // Leave blank if no publication
                            unifiedSheet.Cells[row, 13].Value = "";
                            unifiedSheet.Cells[row, 14].Value = "";
                            unifiedSheet.Cells[row, 15].Value = "";
                            unifiedSheet.Cells[row, 16].Value = "";
                            unifiedSheet.Cells[row, 17].Value = "";
                            unifiedSheet.Cells[row, 18].Value = "";
                            unifiedSheet.Cells[row, 19].Value = "";
                            unifiedSheet.Cells[row, 20].Value = "";
                            unifiedSheet.Cells[row, 21].Value = "";
                        }

                        // Patent data
                        if (i < patents.Count)
                        {
                            var patent = patents[i];
                            unifiedSheet.Cells[row, 22].Value = patent.patentId;
                            unifiedSheet.Cells[row, 23].Value = patent.PatentNo;
                            unifiedSheet.Cells[row, 24].Value = patent.ApplicationFormFileName;
                        }
                        else
                        {
                            unifiedSheet.Cells[row, 22].Value = ""; // Leave blank if no patent
                            unifiedSheet.Cells[row, 23].Value = "";
                            unifiedSheet.Cells[row, 24].Value = "";
                        }

                        // Utilization data
                        if (i < utilizations.Count)
                        {
                            var utilization = utilizations[i];
                            unifiedSheet.Cells[row, 25].Value = utilization.UtilizationId;
                            unifiedSheet.Cells[row, 26].Value = utilization.SubmittedOn.ToString("MM/dd/yyyy");
                            unifiedSheet.Cells[row, 27].Value = utilization.CertificateofUtilizationFileName;
                        }
                        else
                        {
                            unifiedSheet.Cells[row, 25].Value = ""; // Leave blank if no utilization
                            unifiedSheet.Cells[row, 26].Value = "";
                            unifiedSheet.Cells[row, 27].Value = "";
                        }

                        row++;
                    }
                }

                // Auto-fit columns for better readability
                unifiedSheet.Cells[unifiedSheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a downloadable file
                var excelData = excelPackage.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AccomplishmentReport.xlsx");
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "An error occurred while generating the Excel file: " + ex.Message);
                return View("Error"); // Handle the error as needed
            }
        }


        [Authorize(Roles = "RMCC, Director")]
        public async Task<IActionResult> ProductionExportToExcel()
        {
            try
            {
                // Set the license context for EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial if applicable

                // Fetch accomplishments from the database
                var accomplishments = await _context.Production
                    .Include(a => a.LeadResearcher)
                    .Include(a => a.CoLeadResearcher)
                    .Include(a => a.Memberone)
                    .Include(a => a.Membertwo)
                    .Include(a => a.Memberthree)
                    .Include(a => a.CreatedBy)
                    .ToListAsync();

                // Create a new Excel package
                using var excelPackage = new ExcelPackage();
                var workSheet = excelPackage.Workbook.Worksheets.Add("Accomplishments");

                // Add headers

                workSheet.Cells[1, 1].Value = "Year";
                workSheet.Cells[1, 2].Value = "Research Title";
                workSheet.Cells[1, 3].Value = "Lead Researcher";
                workSheet.Cells[1, 4].Value = "Co-Lead Researcher";
                workSheet.Cells[1, 5].Value = "Members";
                workSheet.Cells[1, 6].Value = "Branch/Campus/College/Department";
                //workSheet.Cells[1, 7].Value = "Created By";
                workSheet.Cells[1, 7].Value = "Created On";

                // Add data
                var row = 2;
                foreach (var item in accomplishments)
                {
                    var year = item.Year == null ? string.Empty : item.Year.ToString();
                    var leadResearcher = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == item.LeadResearcherId);
                    var coleadResearcher = string.Empty;
                    var otherMembers = string.Empty;
                    var branchCampus = item.BranchCampus == null ? string.Empty : item.BranchCampus;
                    var college = item.College == null ? string.Empty : item.College;
                    var dept = item.Department == null ? string.Empty : item.Department;

                    workSheet.Cells[row, 1].Value = year;
                    workSheet.Cells[row, 2].Value = item.ResearchTitle;
                    workSheet.Cells[row, 3].Value = $"{leadResearcher.FirstName} {leadResearcher.LastName}";
                    workSheet.Cells[row, 4].Value = coleadResearcher;
                    workSheet.Cells[row, 5].Value = otherMembers;
                    workSheet.Cells[row, 6].Value = $"{branchCampus} {college} {dept}";
                    //workSheet.Cells[row, 7].Value = $"{item.CreatedBy.FirstName} {item.CreatedBy.LastName}";
                    workSheet.Cells[row, 7].Value = item.CreatedOn;

                    row++;
                }

                // Auto-fit columns for better readability
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a downloadable file
                var excelData = excelPackage.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Accomplishments.xlsx");
            }
            catch (Exception ex)
            {
                // Log the exception (you can implement your logging mechanism here)
                ModelState.AddModelError("", "An error occurred while generating the Excel file: " + ex.Message);
                return NotFound("Error"); // Return an error view or handle as needed
            }
        }


        [Authorize(Roles = "RMCC, Director")]
        public async Task<IActionResult> PresentationExportToExcel()
        {
            try
            {
                // Set the license context for EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial if applicable

                // Fetch presentation data from the database
                var presentations = await _context.Presentation
                    .Include(p => p.AddAccomplishment)
                    .ToListAsync();

                // Create a new Excel package
                using var excelPackage = new ExcelPackage();
                var workSheet = excelPackage.Workbook.Worksheets.Add("Presentations");

                // Add headers
                workSheet.Cells[1, 1].Value = "Conference ID";
                workSheet.Cells[1, 2].Value = "Production ID";
                workSheet.Cells[1, 3].Value = "Organizers";
                workSheet.Cells[1, 4].Value = "Presenters";
                workSheet.Cells[1, 5].Value = "Date of Presentation";
                workSheet.Cells[1, 6].Value = "Level";
                workSheet.Cells[1, 7].Value = "Venue";

                // Add data
                var row = 2;
                foreach (var item in presentations)
                {
                    workSheet.Cells[row, 1].Value = item.ConferenceId;
                    workSheet.Cells[row, 2].Value = item.ProductionId;
                    workSheet.Cells[row, 3].Value = $"{item.OrganizerOne}, {item.OrganizerTwo}";
                    workSheet.Cells[row, 4].Value = $"{item.PresenterOne}, {item.PresenterTwo}, {item.PresenterThree}, {item.PresenterFour}, {item.PresenterFive}";
                    workSheet.Cells[row, 5].Value = item.DateofPresentation?.ToString("yyyy-MM-dd");
                    workSheet.Cells[row, 6].Value = item.Level;
                    workSheet.Cells[row, 7].Value = item.Venue;

                    row++;
                }

                // Auto-fit columns for better readability
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a downloadable file
                var excelData = excelPackage.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Presentations.xlsx");
            }
            catch (Exception ex)
            {
                // Log the exception (you can implement your logging mechanism here)
                ModelState.AddModelError("", "An error occurred while generating the Excel file: " + ex.Message);
                return View("Error"); // Return an error view or handle as needed
            }
        }

        [Authorize(Roles = "RMCC, Director")]
        public async Task<IActionResult> PublicationExportToExcel()
        {
            try
            {
                // Set the license context for EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Fetch publication data from the database
                var publications = await _context.Publication
                    .Include(p => p.AddAccomplishment)
                    .ToListAsync();

                // Create a new Excel package
                using var excelPackage = new ExcelPackage();
                var workSheet = excelPackage.Workbook.Worksheets.Add("Publications");

                // Add headers
                workSheet.Cells[1, 1].Value = "Publication ID";
                workSheet.Cells[1, 2].Value = "Production ID";
                workSheet.Cells[1, 3].Value = "Article/Title";
                workSheet.Cells[1, 4].Value = "Date of Publication";
                workSheet.Cells[1, 5].Value = "Title of Journal Publication";
                workSheet.Cells[1, 6].Value = "Document Type";
                workSheet.Cells[1, 7].Value = "Vol. No and Issue No";
                workSheet.Cells[1, 8].Value = "ISSN/ISBN/ESSN";
                workSheet.Cells[1, 9].Value = "DOI";
                workSheet.Cells[1, 10].Value = "Journal Index";
                workSheet.Cells[1, 11].Value = "Supporting Documents";
                workSheet.Cells[1, 12].Value = "Link";

                // Add data
                var row = 2;
                foreach (var item in publications)
                {
                    workSheet.Cells[row, 1].Value = item.publicationId;
                    workSheet.Cells[row, 2].Value = item.ProductionId;
                    workSheet.Cells[row, 3].Value = item.ArticleTitle;
                    workSheet.Cells[row, 4].Value = item.PublicationDate?.ToString("yyyy-MM-dd");
                    workSheet.Cells[row, 5].Value = item.JournalPubTitle;
                    workSheet.Cells[row, 6].Value = item.DocumentType;
                    workSheet.Cells[row, 7].Value = item.VolnoIssueNo;
                    workSheet.Cells[row, 8].Value = item.IssnIsbnEssn;
                    workSheet.Cells[row, 9].Value = item.DOI;
                    workSheet.Cells[row, 10].Value = item.IndexJournal;
                    workSheet.Cells[row, 11].Value = item.SuppDocs;
                    workSheet.Cells[row, 12].Value = item.Link;

                    row++;
                }

                // Auto-fit columns for better readability
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a downloadable file
                var excelData = excelPackage.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Publications.xlsx");
            }
            catch (Exception ex)
            {
                // Log the exception (you can implement your logging mechanism here)
                ModelState.AddModelError("", "An error occurred while generating the Excel file: " + ex.Message);
                return View("Error"); // Return an error view or handle as needed
            }
        }


        [Authorize(Roles = "RMCC, Director")]
        public async Task<IActionResult> PatentExportToExcel()
        {
            try
            {
                // Set the license context for EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Fetch patent data from the database
                var patents = await _context.Patent
                    .Include(p => p.AddAccomplishment)
                    .ToListAsync();

                // Create a new Excel package
                using var excelPackage = new ExcelPackage();
                var workSheet = excelPackage.Workbook.Worksheets.Add("Patents");

                // Add headers
                workSheet.Cells[1, 1].Value = "Patent ID";
                workSheet.Cells[1, 2].Value = "Production ID";
                workSheet.Cells[1, 3].Value = "Patent Number";
                workSheet.Cells[1, 4].Value = "Application Form File Name";

                // Add data
                var row = 2;
                foreach (var item in patents)
                {
                    workSheet.Cells[row, 1].Value = item.patentId;
                    workSheet.Cells[row, 2].Value = item.ProductionId;
                    workSheet.Cells[row, 3].Value = item.PatentNo;
                    workSheet.Cells[row, 4].Value = item.ApplicationFormFileName;

                    row++;
                }

                // Auto-fit columns for better readability
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a downloadable file
                var excelData = excelPackage.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Patents.xlsx");
            }
            catch (Exception ex)
            {
                // Log the exception (you can implement your logging mechanism here)
                ModelState.AddModelError("", "An error occurred while generating the Excel file: " + ex.Message);
                return View("Error"); // Return an error view or handle as needed
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCounts(DateTime? startDate, DateTime? endDate)
        {
            var productionCount = await _context.Production
                .CountAsync(p => p.DateStarted >= startDate && p.DateStarted <= endDate);
            var presentationCount = await _context.Presentation
                .CountAsync(p => p.DateofPresentation >= startDate && p.DateofPresentation <= endDate);
            var publicationCount = await _context.Publication
                .CountAsync(p => p.PublicationDate >= startDate && p.PublicationDate <= endDate);

            var result = new
            {
                Productions = productionCount,
                Presentations = presentationCount,
                Publications = publicationCount
            };

            return Ok(result);
        }




    }
}

