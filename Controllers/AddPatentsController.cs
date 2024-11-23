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
    public class AddPatentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddPatentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AddPatents
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Patent.Include(a => a.AddAccomplishment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AddPatents/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addPatent = await _context.Patent
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.patentId == id);
            if (addPatent == null)
            {
                return NotFound();
            }

            return View(addPatent);
        }

        // GET: AddPatents/Create
        public IActionResult Create()
        {
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            return View();
        }

        // POST: AddPatents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddPatent addPatent, IFormFile file)
        {

            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    addPatent.ApplicationFormData = memoryStream.ToArray();
                    addPatent.ApplicationFormFileName = file.FileName;
                }
            }

            _context.Add(addPatent);
            await _context.SaveChangesAsync();


            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addPatent.ProductionId);
            //return View(addPatent);
            return RedirectToAction(nameof(Index));
        }

        // GET: AddPatents/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addPatent = await _context.Patent.FindAsync(id);
            if (addPatent == null)
            {
                return NotFound();
            }
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addPatent.ProductionId);
            return View(addPatent);
        }

        // POST: AddPatents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AddPatent addPatent, IFormFile file)
        {
            if (id != addPatent.patentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addPatent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddPatentExists(addPatent.patentId))
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
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addPatent.ProductionId);
            return View(addPatent);
        }

        // GET: AddPatents/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addPatent = await _context.Patent
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.patentId == id);
            if (addPatent == null)
            {
                return NotFound();
            }

            return View(addPatent);
        }

        // POST: AddPatents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var addPatent = await _context.Patent.FindAsync(id);
            if (addPatent != null)
            {
                _context.Patent.Remove(addPatent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddPatentExists(string id)
        {
            return _context.Patent.Any(e => e.patentId == id);
        }


        public IActionResult DownloadFile(string id, string fileType)
        {
            // Example: Retrieve file data from the database based on the file ID and fileType
            var patent = _context.Patent.Find(id);
            if (patent == null) return NotFound();

            byte[] fileData;
            string fileName;


            switch (fileType)
            {
                case "ApplicationFormData":
                    fileData = patent.ApplicationFormData;
                    fileName = patent.ApplicationFormFileName;
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