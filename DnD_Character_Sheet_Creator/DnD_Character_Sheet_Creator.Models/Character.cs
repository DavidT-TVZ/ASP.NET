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

        public DateTime? DeletedAt { get; set; }

        public virtual CharacterLevel? Level { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        public virtual ICollection<Equipment> EquipmentList { get; set; } = new List<Equipment>();

        public virtual Player? Player { get; set; }
    }
}