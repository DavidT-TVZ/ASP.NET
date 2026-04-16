using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Repositories
{
    public class MockCharacterRepository : ICharacterRepository
    {
        private readonly Dictionary<int, List<Character>> _charactersByPlayerId;

        public MockCharacterRepository()
        {
            _charactersByPlayerId = MockRepositorySeed.CreateCharactersByPlayerId();
        }

        public IEnumerable<Character> GetAllCharacters()
        {
            return _charactersByPlayerId.Values.SelectMany(characters => characters).ToList();
        }

        public IEnumerable<Character> GetCharactersByPlayerId(int playerId)
        {
            return _charactersByPlayerId.TryGetValue(playerId, out var characters)
                ? characters.AsReadOnly()
                : Enumerable.Empty<Character>();
        }

        public Character? GetCharacterById(int characterId)
        {
            foreach (var characterList in _charactersByPlayerId.Values)
            {
                var character = characterList.FirstOrDefault(existingCharacter => existingCharacter.CharacterId == characterId);
                if (character != null)
                {
                    return character;
                }
            }

            return null;
        }
    }
}
