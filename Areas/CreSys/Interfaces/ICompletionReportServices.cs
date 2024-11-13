using CRE.Models;
using CRE.ViewModels;

namespace CRE.Interfaces
{
    public interface ICompletionReportServices
    {
        Task<ApplyForCompletionCertificateViewModel> GetApplyForCompletionCertificateViewModelAsync(string urecNo);
        Task<bool> SubmitTerminalReportAsync(string urecNo, IFormFile terminalReportFile, DateOnly researchStartDate);
        Task<CompletionReport?> GetCompletionReportByUrecNoAsync(string urecNo);
        Task<IEnumerable<CompletionReportViewModel>> GetCompletionReportsAsync();
        Task<byte[]> GetTerminalReportAsync(string urecNo);
        Task SaveCompletionReportAsync(CompletionReport report);
    }
}
