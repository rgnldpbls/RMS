using CRE.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRE.Controllers
{
    [Area("CreSys")]
    public class ReceiptInfoController : Controller
    {
        private readonly IReceiptInfoServices _receiptInfoServices;
        public ReceiptInfoController(IReceiptInfoServices receiptInfoServices)
        {
            _receiptInfoServices = receiptInfoServices;    
        }
        public IActionResult Index()
        {
            return View();
        }
        // Action to return the receipt PDF
        [Authorize]
        public async Task<IActionResult> ViewReceiptAsync(string urecNo)
        { // Use service to get receipt by UREC No.
            var receiptInfo = await _receiptInfoServices.GetReceiptInfoByUrecNoAsync(urecNo);

            if (receiptInfo == null || receiptInfo.scanReceipt == null)
            {
                return NotFound(); // Handle if receipt is not found
            }

            // Serve the PDF from the binary data in the database
            return File(receiptInfo.scanReceipt, "application/pdf");
        }

    }
}
