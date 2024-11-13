using System.ComponentModel.DataAnnotations;

namespace rscSys_final.Models
{
    public class FinalDocument
    {
        [Key]
        public int FinalDocuID { get; set; }
        public string FinalDocuName { get; set; }
        public byte[] FileData { get; set; }
        public string FileType { get; set; }
        // Foreign key to connect with the Request table
        public int? RequestId { get; set; }

        public Request Request { get; set; }
    }
}
