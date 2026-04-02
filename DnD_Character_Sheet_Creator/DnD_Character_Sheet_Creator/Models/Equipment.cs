namespace DnD_Character_Sheet_Creator.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }

        public string? Type { get; set; }

        public required string Name { get; set; }

        public int Cost { get; set; }

        public int Weight { get; set; }
    }
}