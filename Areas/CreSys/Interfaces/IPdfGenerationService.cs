using ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels;
using ResearchManagementSystem.Areas.CreSys.ViewModels;

namespace ResearchManagementSystem.Areas.CreSys.Interfaces
{
    public interface IPdfGenerationService
    {
        Task<byte[]> GenerateInformedConsentPdf(InformedConsentFormViewModel model);
        Task<byte[]> GenerateProtocolReviewPdf(ProtocolReviewFormViewModel model);
        Task<byte[]> GenerateClearancePdf(EthicsClearanceViewModel viewModel);
    }
}
