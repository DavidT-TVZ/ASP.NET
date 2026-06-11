using System.ComponentModel.DataAnnotations.Schema;

namespace DnD_Character_Sheet_Creator.Models
{
    public class Weapon : Equipment
    {
        public string DamageAmount { get; set; } = string.Empty;

        public string DamageType { get; set; } = string.Empty;

        public List<WeaponPropertiesEnum> WeaponProperties { get; set; } = new List<WeaponPropertiesEnum>();
    }
}