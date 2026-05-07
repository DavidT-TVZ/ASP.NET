using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Repositories
{
    public class EFCharacterRepository : ICharacterRepository
    {
        private readonly DnDDbContext _context;

        public EFCharacterRepository(DnDDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Character> GetAllCharacters()
        {
            return _context.Characters
                .Include(c => c.Level)
                .Include(c => c.EquipmentList)
                .Include(c => c.Player)
                .ToList();
        }

        public IEnumerable<Character> GetCharactersByPlayerId(int playerId)
        {
            return _context.Characters
                .Include(c => c.Level)
                .Include(c => c.EquipmentList)
                .Include(c => c.Player)
                .Where(c => c.PlayerId == playerId)
                .ToList();
        }

        public Character? GetCharacterById(int characterId)
        {
            return _context.Characters
                .Include(c => c.Level)
                .Include(c => c.EquipmentList)
                .Include(c => c.Player)
                .FirstOrDefault(c => c.CharacterId == characterId);
        }

        public void AddCharacter(Character character)
        {
            _context.Characters.Add(character);
            _context.SaveChanges();
        }

        public void UpdateCharacter(Character character)
        {
            _context.Characters.Update(character);
            _context.SaveChanges();
        }

        public void DeleteCharacter(int characterId)
        {
            var character = _context.Characters.Find(characterId);
            if (character != null)
            {
                _context.Characters.Remove(character);
                _context.SaveChanges();
            }
        }
    }
}
