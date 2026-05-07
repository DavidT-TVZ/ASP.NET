using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Repositories
{
    public class MockCharacterRepository : ICharacterRepository
    {
        private readonly Dictionary<int, List<Character>> _charactersByPlayerId;
        private int _nextCharacterId = 1;

        public MockCharacterRepository()
        {
            _charactersByPlayerId = MockRepositorySeed.CreateCharactersByPlayerId();
            _nextCharacterId = _charactersByPlayerId.Values.SelectMany(c => c).Max(c => c.CharacterId) + 1;
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

        public void AddCharacter(Character character)
        {
            character.CharacterId = _nextCharacterId++;

            if (!_charactersByPlayerId.ContainsKey(character.PlayerId))
            {
                _charactersByPlayerId[character.PlayerId] = new List<Character>();
            }

            _charactersByPlayerId[character.PlayerId].Add(character);
        }

        public void UpdateCharacter(Character character)
        {
            if (_charactersByPlayerId.TryGetValue(character.PlayerId, out var characters))
            {
                var existingCharacter = characters.FirstOrDefault(c => c.CharacterId == character.CharacterId);
                if (existingCharacter != null)
                {
                    var index = characters.IndexOf(existingCharacter);
                    characters[index] = character;
                }
            }
        }

        public void DeleteCharacter(int characterId)
        {
            foreach (var characterList in _charactersByPlayerId.Values)
            {
                var character = characterList.FirstOrDefault(c => c.CharacterId == characterId);
                if (character != null)
                {
                    characterList.Remove(character);
                    return;
                }
            }
        }
    }
}
