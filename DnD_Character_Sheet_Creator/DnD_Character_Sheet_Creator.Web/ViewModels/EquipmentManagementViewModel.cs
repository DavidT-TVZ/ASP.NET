using System.ComponentModel.DataAnnotations;
using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class EquipmentManagementViewModel
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Cost (gp)")]
        public int Cost { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Weight")]
        public int Weight { get; set; }

        public string? Query { get; set; }

        public List<Equipment> EquipmentItems { get; set; } = new();
    }
}