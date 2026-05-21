using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Repositories
{
    public interface ICharacterRepository
    {
        IEnumerable<Character> GetAllCharacters();
        IEnumerable<Character> GetCharactersByPlayerId(int playerId);
        IEnumerable<Character> SearchCharacters(string? searchTerm);
        Character? GetCharacterById(int characterId);
        void AddCharacter(Character character);
        void UpdateCharacter(Character character);
        void DeleteCharacter(int characterId);
    }
}
