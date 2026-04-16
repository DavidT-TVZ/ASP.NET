using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Repositories
{
    public interface IPlayerRepository
    {
        IEnumerable<Player> GetAllPlayers();
        Player? GetPlayerById(int playerId);
        Player? GetPlayerByUsername(string username);
    }
}
