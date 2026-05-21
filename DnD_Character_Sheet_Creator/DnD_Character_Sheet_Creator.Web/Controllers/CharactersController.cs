using DnD_Character_Sheet_Creator.Repositories;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DnD_Character_Sheet_Creator.Web.Controllers
{
    [Route("[controller]")]
    [Route("Adventurers")]
    public class CharactersController : Controller
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IPlayerRepository _playerRepository;

        public CharactersController(ICharacterRepository characterRepository, IPlayerRepository playerRepository)
        {
            _characterRepository = characterRepository;
            _playerRepository = playerRepository;
        }

        [HttpGet]
        [Route("")]
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

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            var viewModel = new CharacterFormViewModel
            {
                AvailablePlayers = GetPlayerOptions()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(CharacterFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailablePlayers = GetPlayerOptions();
                return View(viewModel);
            }

            var character = new Character
            {
                PlayerId = viewModel.PlayerId,
                CharacterName = viewModel.CharacterName,
                Race = viewModel.Race!.Value,
                Background = viewModel.Background!.Value,
                Alignment = viewModel.Alignment!.Value,
                Class = viewModel.Class!.Value,
                Level = new CharacterLevel
                {
                    Level = 1,
                    CurrentExperiencePoints = 0,
                    ExperiencePointsToNextLevel = 300,
                    ProficiencyBonus = 2,
                    DateOfLastLevelUp = DateTime.UtcNow
                }
            };

            _characterRepository.AddCharacter(character);

            return RedirectToAction("Details", new { id = character.CharacterId });
        }

        [HttpGet]
        [Route("Details/{id?}")]
        [Route("Info/{id?}")]
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

        [HttpGet]
        [Route("Remove/{id?}")]
        public IActionResult Remove(int id)
        {
            var character = _characterRepository.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            var player = _playerRepository.GetPlayerById(character.PlayerId);
            if (player == null)
            {
                return NotFound();
            }

            return View(new CharacterWithPlayerViewModel
            {
                Character = character,
                PlayerId = player.PlayerId,
                PlayerName = $"{player.Name} {player.Surname}"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Remove")]
        [Route("Remove/{id?}")]
        public IActionResult RemoveConfirmed(int id)
        {
            _characterRepository.DeleteCharacter(id);

            return RedirectToAction("Index");
        }

        private List<SelectListItem> GetPlayerOptions()
        {
            return _playerRepository.GetAllPlayers()
                .Select(player => new SelectListItem
                {
                    Value = player.PlayerId.ToString(),
                    Text = $"{player.Name} {player.Surname}"
                })
                .ToList();
        }
    }
}
