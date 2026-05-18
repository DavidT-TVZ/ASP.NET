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
            return _context.Players.ToList();
        }

        public Player? GetPlayerById(int playerId)
        {
            return _context.Players
                .FirstOrDefault(p => p.PlayerId == playerId);
        }

        public Player? GetPlayerByUsername(string username)
        {
            return _context.Players
                .FirstOrDefault(p => p.Username == username);
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
            var player = _context.Players.Find(playerId);
            if (player != null)
            {
                _context.Players.Remove(player);
                _context.SaveChanges();
            }
        }
    }
}
