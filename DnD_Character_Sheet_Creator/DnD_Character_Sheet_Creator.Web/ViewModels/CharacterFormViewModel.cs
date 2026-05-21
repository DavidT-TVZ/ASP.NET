using System.ComponentModel.DataAnnotations;
using DnD_Character_Sheet_Creator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class CharacterFormViewModel
    {
        public int? CharacterId { get; set; }

        [Display(Name = "Level")]
        public int? LevelId { get; set; }

        public List<SelectListItem> LevelOptions { get; set; } = new();

        [Range(1, int.MaxValue, ErrorMessage = "A player must be selected")]
        [Display(Name = "Player")]
        public int PlayerId { get; set; }

        [Required(ErrorMessage = "Character name is required")]
        [StringLength(100)]
        [Display(Name = "Character Name")]
        public string CharacterName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Race is required")]
        [Display(Name = "Race")]
        public RaceEnum? Race { get; set; }

        [Required(ErrorMessage = "Background is required")]
        [Display(Name = "Background")]
        public BackgroundEnum? Background { get; set; }

        [Required(ErrorMessage = "Alignment is required")]
        [Display(Name = "Alignment")]
        public AlignmentEnum? Alignment { get; set; }

        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "Class")]
        public ClassEnum? Class { get; set; }

        public List<SelectListItem> AvailablePlayers { get; set; } = new();
        public List<SelectListItem> RaceOptions { get; set; } = new();
        public List<SelectListItem> BackgroundOptions { get; set; } = new();
        public List<SelectListItem> AlignmentOptions { get; set; } = new();
        public List<SelectListItem> ClassOptions { get; set; } = new();
    }
}