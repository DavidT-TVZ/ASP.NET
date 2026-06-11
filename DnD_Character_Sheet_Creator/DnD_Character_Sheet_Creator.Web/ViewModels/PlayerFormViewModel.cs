using System.ComponentModel.DataAnnotations;
using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class PlayerFormViewModel
    {
        public int PlayerId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public required string Surname { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50)]
        [Display(Name = "Username")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public required string Email { get; set; }

        [Display(Name = "Role")]
        public RoleEnum Role { get; set; } = RoleEnum.User;
    }
}
