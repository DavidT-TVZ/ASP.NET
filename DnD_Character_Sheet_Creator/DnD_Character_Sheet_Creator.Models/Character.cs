namespace DnD_Character_Sheet_Creator.Models
{
    public class Character
    {
        public int CharacterId { get; set; }

        public int PlayerId { get; set; }

        public int? LevelId { get; set; }

        public required string CharacterName { get; set; }

        public RaceEnum Race { get; set; }

        public BackgroundEnum Background { get; set; }

        public AlignmentEnum Alignment { get; set; }

        public ClassEnum Class { get; set; }

        public CharacterLevel? Level { get; set; }

        public List<Equipment> EquipmentList { get; set; } = new List<Equipment>();

        public Player? Player { get; set; }
    }
}