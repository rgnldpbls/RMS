using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
    public class ReceiptInfo
    {
        [Key]
        [Required(ErrorMessage = "Receipt Number is Required")]
        [Display(Name ="Receipt Number: ")]
        public string receiptNo { get; set; }
        [ForeignKey(nameof(EthicsApplication))]
        public string urecNo { get; set; }
        [Required(ErrorMessage ="Amount Paid is Required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter the paid amount.")]
        [Display(Name = "Amount Paid: ")]
        public float amountPaid { get; set; }
        [Required(ErrorMessage ="Date Paid is Required.")]
        [DataType(DataType.Date)]
        [Display(Name ="Date Paid: ")]
        public DateOnly datePaid { get; set; }
        [Required(ErrorMessage ="Payment Receipt is Required.")]
        public byte[] scanReceipt { get; set; }

        //navigation properties
        public EthicsApplication EthicsApplication { get; set; }
    }
}
