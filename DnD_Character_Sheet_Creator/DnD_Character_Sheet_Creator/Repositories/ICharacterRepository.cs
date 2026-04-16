using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Repositories
{
    public interface ICharacterRepository
    {
        IEnumerable<Character> GetAllCharacters();
        IEnumerable<Character> GetCharactersByPlayerId(int playerId);
        Character? GetCharacterById(int characterId);
    }
}
