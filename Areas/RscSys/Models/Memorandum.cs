namespace rscSys_final.Models
{
    public class Memorandum
    {
        public int memorandumId { get; set; }
        public string UserId { get; set; }
        public string memorandumName { get; set; }
        public byte[] memorandumData { get; set; } // Store file as bytes
        public string filetype { get; set; }

        public DateOnly memorandumUploadDate { get; set; }
        public DateOnly? memorandumUpdateDate { get; set; }

    }
}
