using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class PlayerSignInViewModel
    {
        [Required(ErrorMessage = "Please choose a player")]
        [Display(Name = "Player")]
        public int PlayerId { get; set; }

        public IEnumerable<SelectListItem> Players { get; set; } = Enumerable.Empty<SelectListItem>();

        public string? ReturnUrl { get; set; }
    }
}