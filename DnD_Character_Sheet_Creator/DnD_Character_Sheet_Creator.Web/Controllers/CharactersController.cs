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
        public IActionResult Index(string? query)
        {
            ViewData["SearchQuery"] = query ?? string.Empty;

            return View(BuildCharacterCards(query));
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string? query)
        {
            ViewData["SearchQuery"] = query ?? string.Empty;

            return PartialView("_CharacterCards", BuildCharacterCards(query));
        }

        [HttpGet]
        [Route("Autocomplete")]
        public IActionResult Autocomplete(string term)
        {
            var normalizedTerm = term?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedTerm) || normalizedTerm.Length < 2)
            {
                return Json(Array.Empty<object>());
            }

            var suggestions = BuildCharacterCards(normalizedTerm)
                .Take(8)
                .Select(character => new
                {
                    value = character.Character.CharacterName,
                    label = $"{character.Character.CharacterName} - {character.PlayerName} - Level {character.Character.Level?.Level ?? 0}",
                    playerId = character.PlayerId,
                    characterId = character.Character.CharacterId
                })
                .ToList();

            return Json(suggestions);
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

        [HttpGet]
        [Route("{id:int}/EquipmentSearch")]
        public IActionResult EquipmentSearch(int id, string? query)
        {
            var equipment = BuildEquipmentCards(id, query);

            if (!equipment.Any() && _characterRepository.GetCharacterById(id) == null)
            {
                return NotFound();
            }

            return PartialView("_EquipmentCards", equipment);
        }

        [HttpGet]
        [Route("{id:int}/EquipmentAutocomplete")]
        public IActionResult EquipmentAutocomplete(int id, string term)
        {
            var normalizedTerm = term?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedTerm) || normalizedTerm.Length < 2)
            {
                return Json(Array.Empty<object>());
            }

            var suggestions = BuildEquipmentCards(id, normalizedTerm)
                .Take(8)
                .Select(equipment => new
                {
                    value = equipment.Name,
                    label = $"{equipment.Name} - {equipment.Type ?? "Equipment"} - {equipment.Cost} gp"
                })
                .ToList();

            return Json(suggestions);
        }

        private List<CharacterWithPlayerViewModel> BuildCharacterCards(string? query)
        {
            var cards = _playerRepository.GetAllPlayers()
                .SelectMany(player => _characterRepository.GetCharactersByPlayerId(player.PlayerId)
                    .Select(character => new CharacterWithPlayerViewModel
                    {
                        Character = character,
                        PlayerId = player.PlayerId,
                        PlayerName = $"{player.Name} {player.Surname}"
                    }))
                .ToList();

            var normalizedTerm = query?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedTerm))
            {
                return cards
                    .OrderBy(character => character.Character.CharacterName)
                    .ThenBy(character => character.Character.CharacterId)
                    .ToList();
            }

            return cards
                .Where(character => CharacterMatchesSearch(character, normalizedTerm))
                .OrderBy(character => character.Character.CharacterName)
                .ThenBy(character => character.Character.CharacterId)
                .ToList();
        }

        private static bool CharacterMatchesSearch(CharacterWithPlayerViewModel character, string searchTerm)
        {
            return character.Character.CharacterName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || character.PlayerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || character.Character.Class.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || character.Character.Race.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || character.Character.Background.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || character.Character.Alignment.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || (character.Character.Level?.Level.ToString() ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }

        private List<Equipment> BuildEquipmentCards(int characterId, string? query)
        {
            var character = _characterRepository.GetCharacterById(characterId);
            if (character == null)
            {
                return new List<Equipment>();
            }

            var normalizedTerm = query?.Trim();
            var equipmentList = character.EquipmentList
                .Where(equipment => string.IsNullOrWhiteSpace(normalizedTerm) || EquipmentMatchesSearch(equipment, normalizedTerm))
                .OrderBy(equipment => equipment.Name)
                .ThenBy(equipment => equipment.EquipmentId)
                .ToList();

            return equipmentList;
        }

        private static bool EquipmentMatchesSearch(Equipment equipment, string searchTerm)
        {
            var normalizedTerm = searchTerm.ToLower();

            return equipment.Name.ToLower().Contains(normalizedTerm) ||
                   (equipment.Type ?? string.Empty).ToLower().Contains(normalizedTerm) ||
                   equipment.Cost.ToString().Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase) ||
                   equipment.Weight.ToString().Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase);
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
