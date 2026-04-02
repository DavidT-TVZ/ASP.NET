namespace DnD_Character_Sheet_Creator.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public required string Name { get; set; }

        public required string Surname { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public DateTime LastLogin { get; set; }

        public List<Character> CharacterList { get; set; } = new List<Character>();
    }
}
