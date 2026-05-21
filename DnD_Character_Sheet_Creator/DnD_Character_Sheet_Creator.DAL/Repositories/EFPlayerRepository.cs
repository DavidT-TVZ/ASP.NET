using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Repositories
{
    public class EFPlayerRepository : IPlayerRepository
    {
        private readonly DnDDbContext _context;

        public EFPlayerRepository(DnDDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _context.Players
                .Where(player => player.DeletedAt == null)
                .ToList();
        }

        public Player? GetPlayerById(int playerId)
        {
            return _context.Players
                .FirstOrDefault(p => p.PlayerId == playerId && p.DeletedAt == null);
        }

        public Player? GetPlayerByUsername(string username)
        {
            return _context.Players
                .FirstOrDefault(p => p.Username == username && p.DeletedAt == null);
        }

        public void AddPlayer(Player player)
        {
            _context.Players.Add(player);
            _context.SaveChanges();
        }

        public void UpdatePlayer(Player player)
        {
            _context.Players.Update(player);
            _context.SaveChanges();
        }

        public void DeletePlayer(int playerId)
        {
            var player = _context.Players.FirstOrDefault(p => p.PlayerId == playerId && p.DeletedAt == null);
            if (player != null)
            {
                player.DeletedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
}
