using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRE.Models
{
        public class Secretariat
        {
            [Key]
            public int secretariatId { get; set; }
            public string userId { get; set; }

            //navigation property
        }
}
