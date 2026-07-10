using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class CharacterWithPlayerViewModel
    {
        public required Character Character { get; set; }
        public int PlayerId { get; set; }
        public required string PlayerName { get; set; }
        public string? OwnerUsername { get; set; }
    }
}
