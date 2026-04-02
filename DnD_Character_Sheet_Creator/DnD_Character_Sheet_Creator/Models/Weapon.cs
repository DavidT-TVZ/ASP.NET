namespace DnD_Character_Sheet_Creator.Models
{
    public class Weapon : Equipment
    {
        public string DamageAmount { get; set; }

        public string DamageType { get; set; }

        public List<WeaponPropertiesEnum> WeaponProperties { get; set; }
    }
}
