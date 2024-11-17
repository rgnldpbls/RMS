using CrdlSys.Data;
using CrdlSys.Models;
using CrdlSys.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace CrdlSys.Controllers
{
    [Area("CrdlSys")]
    public class HomeController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly CrdlDbContext _context;

        public HomeController(NotificationService notificationService, CrdlDbContext context) =>
        (_notificationService, _context) = (notificationService, context);

        public async Task<IActionResult> Index()
        {

            var stakeholderUploads = _context.StakeholderUpload.ToList();
            foreach (var upload in stakeholderUploads)
            {
                _notificationService.CheckAndSendNotification(upload);
                _notificationService.UpdateDocumentExpirationStatus();
            }

            var chiefUploads = _context.ChiefUpload.ToList();
            foreach (var upload in chiefUploads)
            {
                _notificationService.CheckAndSendChiefNotification(upload);
                _notificationService.UpdateDocumentChiefExpirationStatus();
            }

            await _notificationService.CheckAndSendEventReminders();
            await _notificationService.CheckAndSendPendingStakeholderReminders();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new CrdlSys.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
