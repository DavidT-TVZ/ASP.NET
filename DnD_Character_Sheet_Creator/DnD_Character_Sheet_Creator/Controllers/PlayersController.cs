using DnD_Character_Sheet_Creator.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DnD_Character_Sheet_Creator.Controllers
{
    public class PlayersController : Controller
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ICharacterRepository _characterRepository;

        public PlayersController(IPlayerRepository playerRepository, ICharacterRepository characterRepository)
        {
            _playerRepository = playerRepository;
            _characterRepository = characterRepository;
        }

        public IActionResult Index()
        {
            var players = _playerRepository.GetAllPlayers()
                .Select(player =>
                {
                    player.CharacterList = _characterRepository.GetCharactersByPlayerId(player.PlayerId).ToList();
                    return player;
                })
                .ToList();

            return View(players);
        }

        public IActionResult Details(int id)
        {
            var player = _playerRepository.GetPlayerById(id);
            if (player == null)
            {
                return NotFound();
            }

            player.CharacterList = _characterRepository.GetCharactersByPlayerId(id).ToList();
            var characters = player.CharacterList;
            ViewBag.Characters = characters;
            return View(player);
        }
    }
}
