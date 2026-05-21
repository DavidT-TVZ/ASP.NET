using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DnD_Character_Sheet_Creator.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }

        [ForeignKey("Character")]
        public int CharacterId { get; set; }

        public string? Type { get; set; }

        public required string Name { get; set; }

        public int Cost { get; set; }

        public int Weight { get; set; }

        public virtual Character? Character { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}