using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Repositories
{
    public class MockPlayerRepository : IPlayerRepository
    {
        private readonly List<Player> _players;

        public MockPlayerRepository()
        {
            _players = MockRepositorySeed.CreatePlayers();
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _players
                .Where(player => player.DeletedAt == null)
                .ToList();
        }

        public Player? GetPlayerById(int playerId)
        {
            return _players.FirstOrDefault(player => player.PlayerId == playerId && player.DeletedAt == null);
        }

        public Player? GetPlayerByUsername(string username)
        {
            return _players.FirstOrDefault(player => player.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && player.DeletedAt == null);
        }

        public void AddPlayer(Player player)
        {
            player.PlayerId = _players.Count > 0 ? _players.Max(p => p.PlayerId) + 1 : 1;
            _players.Add(player);
        }

        public void UpdatePlayer(Player player)
        {
            var existingPlayer = GetPlayerById(player.PlayerId);
            if (existingPlayer != null)
            {
                var index = _players.IndexOf(existingPlayer);
                _players[index] = player;
            }
        }

        public void DeletePlayer(int playerId)
        {
            var player = GetPlayerById(playerId);
            if (player != null)
            {
                player.DeletedAt = DateTime.UtcNow;
            }
        }
    }
}
