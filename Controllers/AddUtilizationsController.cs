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
    public class AddUtilizationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddUtilizationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: AddUtilizations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Utilization.Include(a => a.AddAccomplishment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AddUtilizations/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addUtilization = await _context.Utilization
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.UtilizationId == id);
            if (addUtilization == null)
            {
                return NotFound();
            }

            return View(addUtilization);
        }

        // GET: AddUtilizations/Create
        public IActionResult Create()
        {
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId");
            return View();
        }

        // POST: AddUtilizations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUtilization addUtilization, IFormFile file)
        {
            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    addUtilization.CertificateofUtilizationData = memoryStream.ToArray();
                    addUtilization.CertificateofUtilizationFileName = file.FileName;
                }
            }

            _context.Add(addUtilization);
            await _context.SaveChangesAsync();

            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addUtilization.ProductionId);
            //return View(addUtilization);
            return RedirectToAction(nameof(Index));
        }

        // GET: AddUtilizations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addUtilization = await _context.Utilization.FindAsync(id);
            if (addUtilization == null)
            {
                return NotFound();
            }
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addUtilization.ProductionId);
            return View(addUtilization);
        }

        // POST: AddUtilizations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UtilizationId,ProductionId,CertificateofUtlizationData,CertificateofUtlizationFileName")] AddUtilization addUtilization)
        {
            if (id != addUtilization.UtilizationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addUtilization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddUtilizationExists(addUtilization.UtilizationId))
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
            ViewData["ProductionId"] = new SelectList(_context.Production, "ProductionId", "ProductionId", addUtilization.ProductionId);
            return View(addUtilization);
        }

        // GET: AddUtilizations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addUtilization = await _context.Utilization
                .Include(a => a.AddAccomplishment)
                .FirstOrDefaultAsync(m => m.UtilizationId == id);
            if (addUtilization == null)
            {
                return NotFound();
            }

            return View(addUtilization);
        }

        // POST: AddUtilizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var addUtilization = await _context.Utilization.FindAsync(id);
            if (addUtilization != null)
            {
                _context.Utilization.Remove(addUtilization);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddUtilizationExists(string id)
        {
            return _context.Utilization.Any(e => e.UtilizationId == id);
        }

        public IActionResult DownloadFile(string id, string fileType)
        {
            // Example: Retrieve file data from the database based on the file ID and fileType
            var utilization = _context.Utilization.Find(id);
            if (utilization == null) return NotFound();

            byte[] fileData;
            string fileName;


            switch (fileType)
            {
                case "CertificateofUtilizationData":
                    fileData = utilization.CertificateofUtilizationData;
                    fileName = utilization.CertificateofUtilizationFileName;
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