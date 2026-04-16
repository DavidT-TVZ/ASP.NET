using DnD_Character_Sheet_Creator.Repositories;
using DnD_Character_Sheet_Creator.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DnD_Character_Sheet_Creator.Controllers
{
    public class CharactersController : Controller
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IPlayerRepository _playerRepository;

        public CharactersController(ICharacterRepository characterRepository, IPlayerRepository playerRepository)
        {
            _characterRepository = characterRepository;
            _playerRepository = playerRepository;
        }

        public IActionResult Index()
        {
            var characters = _playerRepository.GetAllPlayers()
                .SelectMany(player => _characterRepository.GetCharactersByPlayerId(player.PlayerId)
                    .Select(character => new CharacterWithPlayerViewModel
                    {
                        Character = character,
                        PlayerId = player.PlayerId,
                        PlayerName = $"{player.Name} {player.Surname}"
                    }))
                .ToList();

            return View(characters);
        }

        public IActionResult Details(int id)
        {
            var owner = _playerRepository.GetAllPlayers()
                .Select(player => new
                {
                    Player = player,
                    Characters = _characterRepository.GetCharactersByPlayerId(player.PlayerId)
                })
                .FirstOrDefault(item => item.Characters.Any(character => character.CharacterId == id));

            var character = owner?.Characters.FirstOrDefault(item => item.CharacterId == id);
            if (character == null || owner == null)
            {
                return NotFound();
            }

            return View(new CharacterWithPlayerViewModel
            {
                Character = character,
                PlayerId = owner.Player.PlayerId,
                PlayerName = $"{owner.Player.Name} {owner.Player.Surname}"
            });
        }
    }
}
