using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRE.Models
{
    public class EvaluationForms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int evalFormId { get; set; }
        [Required]
        public string evalFormName { get; set; }
        [Required]
        public byte[] evalFormFile { get; set; }
    }
}
