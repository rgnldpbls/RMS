using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResearchManagementSystem.Data;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Controllers
{
    public class AddPresentationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddPresentationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AddPresentations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Presentation.Include(a => a.AddAccomplishment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AddPresentations/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addPresentation = await _context.Presentation
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.ConferenceId == id);
            if (addPresentation == null)
            {
                return NotFound();
            }

            return View(addPresentation);
        }

        // GET: AddPresentations/Create
        public IActionResult Create()
        {
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            return View();
        }

        // POST: AddPresentations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConferenceId,ProductionId,OrganizerOne,OrganizerTwo,PresenterOne,PresenterTwo,PresenterThree,PresenterFour,PresenterFive,DateofPresentation,Level,Venue,CertificateofParticipationFileData,CertificateofParticipationFileName")] AddPresentation addPresentation, IFormFile? CertificateofParticipationFileData)
        {

            if (CertificateofParticipationFileData != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await CertificateofParticipationFileData.CopyToAsync(memoryStream);
                    addPresentation.CertificateofParticipationFileData = memoryStream.ToArray();
                    addPresentation.CertificateofParticipationFileName = CertificateofParticipationFileData.FileName;
                }
            }

            _context.Add(addPresentation);
            await _context.SaveChangesAsync();


            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addPresentation.ProductionId);
            // return View(addPresentation);
            return RedirectToAction(nameof(Index));
        }

        // GET: AddPresentations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addPresentation = await _context.Presentation.FindAsync(id);
            if (addPresentation == null)
            {
                return NotFound();
            }
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addPresentation.ProductionId);
            return View(addPresentation);
        }

        // POST: AddPresentations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ConferenceId,ProductionId,OrganizerOne,OrganizerTwo,PresenterOne,PresenterTwo,PresenterThree,PresenterFour,PresenterFive,DateofPresentation,Level,Venue,CertificateofParticipationFileData,CertificateofParticipationFileName")] AddPresentation addPresentation)
        {
            if (id != addPresentation.ConferenceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addPresentation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddPresentationExists(addPresentation.ConferenceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addPresentation.ProductionId);
            return View(addPresentation);
        }

        // GET: AddPresentations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addPresentation = await _context.Presentation
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.ConferenceId == id);
            if (addPresentation == null)
            {
                return NotFound();
            }

            return View(addPresentation);
        }

        // POST: AddPresentations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var addPresentation = await _context.Presentation.FindAsync(id);
            if (addPresentation != null)
            {
                _context.Presentation.Remove(addPresentation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddPresentationExists(string id)
        {
            return _context.Presentation.Any(e => e.ConferenceId == id);
        }

        public IActionResult DownloadFile(string id, string fileType)
        {
            // Example: Retrieve file data from the database based on the file ID and fileType
            var presentation = _context.Presentation.Find(id);
            if (presentation == null) return NotFound();

            byte[] fileData;
            string fileName;


            switch (fileType)
            {
                case "CertificateofParticipationFileData":
                    fileData = presentation.CertificateofParticipationFileData;
                    fileName = presentation.CertificateofParticipationFileName;
                    break;

                default:
                    return BadRequest("Invalid file type specified.");
            }

            if (fileData == null) return NotFound("File data not found.");

            // Return the file to the client
            return File(fileData, "application/octet-stream", fileName);
        }


    }
}