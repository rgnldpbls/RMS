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
using ResearchManagementSystem.Migrations;



namespace ResearchManagementSystem.Controllers
{
    public class AddPublicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddPublicationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AddPublications
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Publication.Include(a => a.AddAccomplishment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AddPublications/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addPublication = await _context.Publication
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.publicationId == id);
            if (addPublication == null)
            {
                return NotFound();
            }

            return View(addPublication);
        }

        // GET: AddPublications/Create
        public IActionResult Create()
        {
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            return View();
        }

        // POST: AddPublications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.AddPublication addPublication, IFormFile file)
        {


            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    addPublication.ManuscriptJournalData = memoryStream.ToArray();
                    addPublication.ManuscriptJournalFileName = file.FileName;
                }
            }

            _context.Add(addPublication);
            await _context.SaveChangesAsync();


            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addPublication.ProductionId);
            //return View(addPublication);
            return RedirectToAction(nameof(Index));
        }


        // GET: AddPublications/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addPublication = await _context.Publication.FindAsync(id);
            if (addPublication == null)
            {
                return NotFound();
            }
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addPublication.ProductionId);
            return View(addPublication);
        }

        // POST: AddPublications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("publicationId,ProductionId,ArticleTitle,PublicationDate,JournalPubTitle,DocumentType,VolnoIssueNo,IssnIsbnEssn,DOI,IndexJournal,SuppDocs,Link,ManuscriptJournalData,ManuscriptJournalFileName")] Models.AddPublication addPublication)
        {
            if (id != addPublication.publicationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addPublication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddPublicationExists(addPublication.publicationId))
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
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addPublication.ProductionId);
            return View(addPublication);
        }

        // GET: AddPublications/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addPublication = await _context.Publication
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.publicationId == id);
            if (addPublication == null)
            {
                return NotFound();
            }

            return View(addPublication);
        }

        // POST: AddPublications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var addPublication = await _context.Publication.FindAsync(id);
            if (addPublication != null)
            {
                _context.Publication.Remove(addPublication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddPublicationExists(string id)
        {
            return _context.Publication.Any(e => e.publicationId == id);
        }




        public IActionResult DownloadFile(string id, string fileType)
        {
            // Example: Retrieve file data from the database based on the file ID and fileType
            var publication = _context.Publication.Find(id);
            if (publication == null) return NotFound();

            byte[] fileData;
            string fileName;


            switch (fileType)
            {
                case "ManuscriptJournalData":
                    fileData = publication.ManuscriptJournalData;
                    fileName = publication.ManuscriptJournalFileName;
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