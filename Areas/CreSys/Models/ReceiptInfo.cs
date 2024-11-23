using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResearchManagementSystem.Areas.CreSys.Models
{
    public class ReceiptInfo
    {
        [Key]
        public string ReceiptNo { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string UrecNo { get; set; }
        public float AmountPaid { get; set; }
        public DateTime DatePaid { get; set; }
        public byte[] ScanReceipt { get; set; }

        public EthicsApplication EthicsApplication { get; set; }
    }
}
