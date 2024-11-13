using CRE.Models;

namespace CRE.Interfaces
{
    public interface ICompletionCertificateServices
    {
        Task<CompletionCertificate?> GetCompletionCertificateByUrecNoAsync(string urecNo);
        Task SaveCompletionCertificateAsync(CompletionCertificate certificate);
    }
}
