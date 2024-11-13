using CRE.Models;

namespace CRE.Interfaces
{
    public interface IReceiptInfoServices
    {
        Task AddReceiptInfoAsync(ReceiptInfo receipt);
        Task<ReceiptInfo> GetReceiptInfoByUrecNoAsync(string urecNo); // Retrieve the ReceiptInfo record by matching the urecNo (FK)
        Task<bool> ReceiptNoExistsAsync(string receiptNo);
    }
}
