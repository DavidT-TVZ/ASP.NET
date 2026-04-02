namespace DnD_Character_Sheet_Creator.Models
{
    public class CharacterLevel
    {
        public int LevelId { get; set; }

        public int Level { get; set; }

        public int CurrentExperiencePoints { get; set; }

        public int ExperiencePointsToNextLevel { get; set; }

        public int ProficiencyBonus { get; set; }

        public DateTime DateOfLastLevelUp { get; set; }
    }
}