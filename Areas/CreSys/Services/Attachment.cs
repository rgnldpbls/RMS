namespace ResearchManagementSystem.Areas.CreSys.Services
{
    public class Attachment
    {
        public Stream ContentStream { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        // Constructor to initialize the attachment properties
        public Attachment(Stream contentStream, string fileName, string contentType)
        {
            ContentStream = contentStream;
            FileName = fileName;
            ContentType = contentType;
        }
    }
}
