using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class CodexSearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<Player> Players { get; set; } = new();
        public List<Character> Characters { get; set; } = new();
        public List<Equipment> Equipment { get; set; } = new();
    }
}