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

        [Range(0, int.MaxValue)]
        [Display(Name = "Current Experience")]
        public int CurrentExperiencePoints { get; set; }

        [Display(Name = "Last Level Up")]
        public DateTime? DateOfLastLevelUp { get; set; }

        [Range(1, 30)]
        [Display(Name = "Strength")]
        public int Strength { get; set; } = 10;

        [Range(1, 30)]
        [Display(Name = "Dexterity")]
        public int Dexterity { get; set; } = 10;

        [Range(1, 30)]
        [Display(Name = "Constitution")]
        public int Constitution { get; set; } = 10;

        [Range(1, 30)]
        [Display(Name = "Intelligence")]
        public int Intelligence { get; set; } = 10;

        [Range(1, 30)]
        [Display(Name = "Wisdom")]
        public int Wisdom { get; set; } = 10;

        [Range(1, 30)]
        [Display(Name = "Charisma")]
        public int Charisma { get; set; } = 10;

        [Display(Name = "Strength Save Proficient")]
        public bool StrengthSaveProficient { get; set; }

        [Display(Name = "Dexterity Save Proficient")]
        public bool DexteritySaveProficient { get; set; }

        [Display(Name = "Constitution Save Proficient")]
        public bool ConstitutionSaveProficient { get; set; }

        [Display(Name = "Intelligence Save Proficient")]
        public bool IntelligenceSaveProficient { get; set; }

        [Display(Name = "Wisdom Save Proficient")]
        public bool WisdomSaveProficient { get; set; }

        [Display(Name = "Charisma Save Proficient")]
        public bool CharismaSaveProficient { get; set; }

        [Display(Name = "Acrobatics")]
        public bool AcrobaticsProficient { get; set; }

        [Display(Name = "Animal Handling")]
        public bool AnimalHandlingProficient { get; set; }

        [Display(Name = "Arcana")]
        public bool ArcanaProficient { get; set; }

        [Display(Name = "Athletics")]
        public bool AthleticsProficient { get; set; }

        [Display(Name = "Deception")]
        public bool DeceptionProficient { get; set; }

        [Display(Name = "History")]
        public bool HistoryProficient { get; set; }

        [Display(Name = "Insight")]
        public bool InsightProficient { get; set; }

        [Display(Name = "Intimidation")]
        public bool IntimidationProficient { get; set; }

        [Display(Name = "Investigation")]
        public bool InvestigationProficient { get; set; }

        [Display(Name = "Medicine")]
        public bool MedicineProficient { get; set; }

        [Display(Name = "Nature")]
        public bool NatureProficient { get; set; }

        [Display(Name = "Perception")]
        public bool PerceptionProficient { get; set; }

        [Display(Name = "Performance")]
        public bool PerformanceProficient { get; set; }

        [Display(Name = "Persuasion")]
        public bool PersuasionProficient { get; set; }

        [Display(Name = "Religion")]
        public bool ReligionProficient { get; set; }

        [Display(Name = "Sleight of Hand")]
        public bool SleightOfHandProficient { get; set; }

        [Display(Name = "Stealth")]
        public bool StealthProficient { get; set; }

        [Display(Name = "Survival")]
        public bool SurvivalProficient { get; set; }

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