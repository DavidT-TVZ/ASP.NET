using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DnD_Character_Sheet_Creator.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }
        public required string Name { get; set; }

        public required string Surname { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool IsAdmin { get; set; } = false;

        public virtual ICollection<Character> CharacterList { get; set; } = new List<Character>();
    }
}