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

        public IEnumerable<Character> SearchCharacters(string? searchTerm)
        {
            var normalizedTerm = searchTerm?.Trim();
            var characters = _context.Characters
                .Where(character => character.DeletedAt == null)
                .ToList();

            if (string.IsNullOrWhiteSpace(normalizedTerm))
            {
                return characters;
            }

            normalizedTerm = normalizedTerm.ToLower();

            return characters
                .Where(character =>
                    character.CharacterName.ToLower().Contains(normalizedTerm) ||
                    character.Class.ToString().ToLower().Contains(normalizedTerm) ||
                    character.Race.ToString().ToLower().Contains(normalizedTerm) ||
                    character.Background.ToString().ToLower().Contains(normalizedTerm) ||
                    character.Alignment.ToString().ToLower().Contains(normalizedTerm))
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

            var attachments = _context.Attachments.Where(a => a.CharacterId == characterId && a.DeletedAt == null).ToList();
            foreach (var attachment in attachments)
            {
                attachment.DeletedAt = DateTime.UtcNow;
            }

            _context.SaveChanges();
        }
    }
}
