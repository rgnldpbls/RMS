using System.ComponentModel.DataAnnotations;

namespace rscSys_final.Models
{
    public class Template
    {
        [Key]
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public byte[] FileData { get; set; }
        public string FileType { get; set; }
        public DateTime FileUploaded { get; set; }
    }
}
