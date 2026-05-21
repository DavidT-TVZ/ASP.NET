using System.ComponentModel.DataAnnotations;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class EquipmentFormViewModel
    {
        public int? EquipmentId { get; set; }

        [Required]
        public int CharacterId { get; set; }

        [Display(Name = "Type")]
        public string? Type { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Cost (gp)")]
        public int Cost { get; set; }

        [Display(Name = "Weight")]
        public int Weight { get; set; }
    }
}
