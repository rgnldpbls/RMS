using DocumentTrackingSystemBackend.Models;
using System.ComponentModel.DataAnnotations;

namespace DocumentTrackingSystemBackend.ViewModels
{
    public class RoleViewModel
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50)]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
    }
}
