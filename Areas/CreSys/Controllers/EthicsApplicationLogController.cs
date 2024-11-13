using CRE.Data;
using CRE.Interfaces;
using CRE.Models;
using CRE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class EthicsApplicationLogController : Controller
    {
        private readonly IEthicsApplicationLogServices _ethicsApplicationLogServices;
        private readonly CreDbContext _context;
        public EthicsApplicationLogController(IEthicsApplicationLogServices ethicsApplicationLogServices, CreDbContext context)
        {
            _ethicsApplicationLogServices = ethicsApplicationLogServices;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> TrackApplication(string urecNo)
        {
            var logs = await _ethicsApplicationLogServices.GetLogsByUrecNoAsync(urecNo);
            ViewBag.urecNo = urecNo;

            var ethicsApp = await _context.EthicsApplication.FirstAsync(u => u.urecNo == urecNo);
            var nonFundedResearch = await _context.NonFundedResearchInfo.FirstAsync(n => n.urecNo == urecNo);

            ViewBag.Title = nonFundedResearch.title;
            ViewBag.Author = ethicsApp.Name;
            ViewBag.Field = ethicsApp.fieldOfStudy;

            if (logs == null || !logs.Any())
            {
                // Handle case where no logs are found (optional)
                return View(new List<EthicsApplicationLog>()); // Pass an empty list if no logs found
            }

            return View(logs); // Pass the list of logs to the view
        }
    }
}
