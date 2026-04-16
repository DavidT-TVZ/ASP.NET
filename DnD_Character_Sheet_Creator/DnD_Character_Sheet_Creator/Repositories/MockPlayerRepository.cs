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
            return _players.AsReadOnly();
        }

        public Player? GetPlayerById(int playerId)
        {
            return _players.FirstOrDefault(player => player.PlayerId == playerId);
        }

        public Player? GetPlayerByUsername(string username)
        {
            return _players.FirstOrDefault(player => player.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
