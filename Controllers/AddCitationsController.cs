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
    public class AddCitationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddCitationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AddCitations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Citation.Include(a => a.CoLeadResearcher).Include(a => a.LeadResearcher).Include(a => a.Memberone).Include(a => a.Memberthree).Include(a => a.Membertwo).Include(a => a.AddAccomplishment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AddCitations/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addCitation = await _context.Citation
                .Include(a => a.CoLeadResearcher)
                .Include(a => a.LeadResearcher)
                .Include(a => a.Memberone)
                .Include(a => a.Memberthree)
                .Include(a => a.Membertwo)
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.CitationId == id);
            if (addCitation == null)
            {
                return NotFound();
            }

            return View(addCitation);
        }

        // GET: AddCitations/Create
        public IActionResult Create()
        {
            ViewData["CoLeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["LeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["MemberoneId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["MemberthreeId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["MembertwoId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            return View();
        }

        // POST: AddCitations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CitationId,ProductionId,ArticleTitle,LeadResearcherId,CoLeadResearcherId,MemberoneId,MembertwoId,MemberthreeId,OriginalArticlePublished,NewPublicationTitle,AuthorsofNewArticle,NewArticlePublished,VolNoIssueNo,Pages,YearofPublication,Indexing,CitationProofData,CitationProofFileName")] AddCitation addCitation, IFormFile file)
        {

            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    addCitation.CitationProofData = memoryStream.ToArray();
                    addCitation.CitationProofFileName = file.FileName;
                }
            }
            _context.Add(addCitation);
            await _context.SaveChangesAsync();


            ViewData["CoLeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.CoLeadResearcherId);
            ViewData["LeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.LeadResearcherId);
            ViewData["MemberoneId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MemberoneId);
            ViewData["MemberthreeId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MemberthreeId);
            ViewData["MembertwoId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MembertwoId);
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.ProductionId);
            //return View(addCitation);
            return RedirectToAction(nameof(Index));
        }

        // GET: AddCitations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addCitation = await _context.Citation.FindAsync(id);
            if (addCitation == null)
            {
                return NotFound();
            }
            ViewData["CoLeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.CoLeadResearcherId);
            ViewData["LeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.LeadResearcherId);
            ViewData["MemberoneId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MemberoneId);
            ViewData["MemberthreeId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MemberthreeId);
            ViewData["MembertwoId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MembertwoId);
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.ProductionId);
            return View(addCitation);
        }

        // POST: AddCitations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CitationId,ProductionId,ArticleTitle,LeadResearcherId,CoLeadResearcherId,MemberoneId,MembertwoId,MemberthreeId,OriginalArticlePublished,NewPublicationTitle,AuthorsofNewArticle,NewArticlePublished,VolNoIssueNo,Pages,YearofPublication,Indexing,CitationProofData,CitationProofFileName")] AddCitation addCitation)
        {
            if (id != addCitation.CitationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addCitation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddCitationExists(addCitation.CitationId))
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
            ViewData["CoLeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.CoLeadResearcherId);
            ViewData["LeadResearcherId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.LeadResearcherId);
            ViewData["MemberoneId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MemberoneId);
            ViewData["MemberthreeId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MemberthreeId);
            ViewData["MembertwoId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.MembertwoId);
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addCitation.ProductionId);
            return View(addCitation);
        }

        // GET: AddCitations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addCitation = await _context.Citation
                .Include(a => a.CoLeadResearcher)
                .Include(a => a.LeadResearcher)
                .Include(a => a.Memberone)
                .Include(a => a.Memberthree)
                .Include(a => a.Membertwo)
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.CitationId == id);
            if (addCitation == null)
            {
                return NotFound();
            }

            return View(addCitation);
        }

        // POST: AddCitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var addCitation = await _context.Citation.FindAsync(id);
            if (addCitation != null)
            {
                _context.Citation.Remove(addCitation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddCitationExists(string id)
        {
            return _context.Citation.Any(e => e.CitationId == id);
        }

        public IActionResult DownloadFile(string id, string fileType)
        {
            // Example: Retrieve file data from the database based on the file ID and fileType
            var citation = _context.Citation.Find(id);
            if (citation == null) return NotFound();

            byte[] fileData;
            string fileName;


            switch (fileType)
            {
                case "CitationProofData":
                    fileData = citation.CitationProofData;
                    fileName = citation.CitationProofFileName;
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