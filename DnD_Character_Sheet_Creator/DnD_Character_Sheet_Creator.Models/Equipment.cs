using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DnD_Character_Sheet_Creator.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }

        [NotMapped]
        public int? CharacterId { get; set; }

        public string? Type { get; set; }

        public required string Name { get; set; }

        public int Cost { get; set; }

        public int Weight { get; set; }

        public virtual ICollection<Character> Characters { get; set; } = new List<Character>();
        public DateTime? DeletedAt { get; set; }
    }
}