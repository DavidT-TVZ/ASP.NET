using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.ViewModels
{
    public class CharacterWithPlayerViewModel
    {
        public required Character Character { get; set; }
        public int PlayerId { get; set; }
        public required string PlayerName { get; set; }
    }
}
