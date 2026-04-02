namespace DnD_Character_Sheet_Creator.Models
{
    public class Armour : Equipment
    {
        public int ArmourClass { get; set; }

        public int StrenghtRequierment { get; set; }

        public bool StealthDisadvantage { get; set; }
    }
}
