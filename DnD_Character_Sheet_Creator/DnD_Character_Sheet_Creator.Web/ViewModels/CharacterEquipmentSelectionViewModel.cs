using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class CharacterEquipmentSelectionViewModel
    {
        public int CharacterId { get; set; }

        [Required(ErrorMessage = "Select an equipment item")]
        [Display(Name = "Equipment")]
        public int? SelectedEquipmentId { get; set; }

        public List<SelectListItem> AvailableEquipment { get; set; } = new();
    }
}