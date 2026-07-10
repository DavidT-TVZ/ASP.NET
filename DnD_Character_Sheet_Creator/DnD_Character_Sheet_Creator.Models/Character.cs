using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DnD_Character_Sheet_Creator.Models
{
    public class Character
    {
        [Key]
        public int CharacterId { get; set; }

        [ForeignKey("Player")]
        public int PlayerId { get; set; }

        [ForeignKey("Level")]
        public int? LevelId { get; set; }

        public required string CharacterName { get; set; }

        public RaceEnum Race { get; set; }

        public BackgroundEnum Background { get; set; }

        public AlignmentEnum Alignment { get; set; }

        public ClassEnum Class { get; set; }

        public int CurrentExperiencePoints { get; set; }

        public DateTime? DateOfLastLevelUp { get; set; }

        public int Strength { get; set; }

        public int Dexterity { get; set; }

        public int Constitution { get; set; }

        public int Intelligence { get; set; }

        public int Wisdom { get; set; }

        public int Charisma { get; set; }

        public bool StrengthSaveProficient { get; set; }

        public bool DexteritySaveProficient { get; set; }

        public bool ConstitutionSaveProficient { get; set; }

        public bool IntelligenceSaveProficient { get; set; }

        public bool WisdomSaveProficient { get; set; }

        public bool CharismaSaveProficient { get; set; }

        public bool AcrobaticsProficient { get; set; }

        public bool AnimalHandlingProficient { get; set; }

        public bool ArcanaProficient { get; set; }

        public bool AthleticsProficient { get; set; }

        public bool DeceptionProficient { get; set; }

        public bool HistoryProficient { get; set; }

        public bool InsightProficient { get; set; }

        public bool IntimidationProficient { get; set; }

        public bool InvestigationProficient { get; set; }

        public bool MedicineProficient { get; set; }

        public bool NatureProficient { get; set; }

        public bool PerceptionProficient { get; set; }

        public bool PerformanceProficient { get; set; }

        public bool PersuasionProficient { get; set; }

        public bool ReligionProficient { get; set; }

        public bool SleightOfHandProficient { get; set; }

        public bool StealthProficient { get; set; }

        public bool SurvivalProficient { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual CharacterLevel? Level { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        public virtual ICollection<Equipment> EquipmentList { get; set; } = new List<Equipment>();

        public virtual Player? Player { get; set; }
    }
}