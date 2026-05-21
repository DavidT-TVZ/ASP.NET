using System;
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
                .Where(character => character.DeletedAt == null)
                .ToList();
        }

        public IEnumerable<Character> GetCharactersByPlayerId(int playerId)
        {
            return _context.Characters
                .Where(c => c.PlayerId == playerId && c.DeletedAt == null)
                .ToList();
        }

        public Character? GetCharacterById(int characterId)
        {
            return _context.Characters
                .FirstOrDefault(c => c.CharacterId == characterId && c.DeletedAt == null);
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
            var character = _context.Characters.FirstOrDefault(c => c.CharacterId == characterId && c.DeletedAt == null);
            if (character == null)
            {
                return;
            }

            character.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
