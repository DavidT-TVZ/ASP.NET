using DnD_Character_Sheet_Creator.Repositories;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DnD_Character_Sheet_Creator.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [Route("Actors")]
    public class PlayersController : Controller
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ICharacterRepository _characterRepository;

        public PlayersController(IPlayerRepository playerRepository, ICharacterRepository characterRepository)
        {
            _playerRepository = playerRepository;
            _characterRepository = characterRepository;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index(string? query)
        {
            ViewData["SearchQuery"] = query ?? string.Empty;

            var current = GetCurrentPlayer();

            if (current != null && !current.IsAdmin)
            {
                // Non-admins only see their own data
                current.CharacterList = _characterRepository.GetCharactersByPlayerId(current.PlayerId)
                    .Where(character => character.DeletedAt == null)
                    .ToList();
                ViewBag.Characters = BuildPlayerCharacterCards(current, null);
                return View(new List<Player> { current });
            }

            return View(BuildPlayerCards(query));
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string? query)
        {
            ViewData["SearchQuery"] = query ?? string.Empty;

            return PartialView("_PlayerCards", BuildPlayerCards(query));
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

            var suggestions = BuildPlayerCards(normalizedTerm)
                .Take(8)
                .Select(player => new
                {
                    value = $"{player.Name} {player.Surname}",
                    label = $"{player.Name} {player.Surname} - {player.Username} - {player.Email}",
                    playerId = player.PlayerId
                })
                .ToList();

            return Json(suggestions);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create(int? playerId)
        {
            if (!IsCurrentPlayerAdmin())
            {
                return Forbid();
            }

            ViewBag.CanSetAdmin = GetCurrentPlayer()?.IsAdmin == true;
            return View(new PlayerFormViewModel
            {
                Name = string.Empty,
                Surname = string.Empty,
                Username = string.Empty,
                Email = string.Empty
            });
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult Create(PlayerFormViewModel viewModel)
        {
            if (!IsCurrentPlayerAdmin())
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var player = new Player
            {
                Name = viewModel.Name,
                Surname = viewModel.Surname,
                Username = viewModel.Username,
                Email = viewModel.Email,
                Password = string.Empty,
                LastLogin = DateTime.UtcNow
            };

            // Only an admin can create another admin user
            var current = GetCurrentPlayer();
            if (current != null && current.IsAdmin)
            {
                player.IsAdmin = viewModel.IsAdmin;
            }

            _playerRepository.AddPlayer(player);

            return RedirectToAction("Details", new { id = player.PlayerId });
        }

        [HttpGet]
        [Route("Details/{id?}")]
        [Route("Info/{id?}")]
        public IActionResult Details(int id)
        {
            var player = _playerRepository.GetPlayerById(id);
            if (player == null)
            {
                return NotFound();
            }
            var current = GetCurrentPlayer();
            if (current != null && !current.IsAdmin && current.PlayerId != id)
            {
                return Forbid();
            }

            player.CharacterList = _characterRepository.GetCharactersByPlayerId(id)
                .Where(character => character.DeletedAt == null)
                .ToList();
            ViewBag.Characters = BuildPlayerCharacterCards(player, null);
            return View(player);
        }

        [HttpGet]
        [Route("{id:int}/CharactersSearch")]
        public IActionResult CharactersSearch(int id, string? query)
        {
            var player = _playerRepository.GetPlayerById(id);
            if (player == null)
            {
                return NotFound();
            }

            return PartialView("_PlayerCharacters", BuildPlayerCharacterCards(player, query));
        }

        [HttpGet]
        [Route("{id:int}/CharactersAutocomplete")]
        public IActionResult CharactersAutocomplete(int id, string term)
        {
            var player = _playerRepository.GetPlayerById(id);
            if (player == null)
            {
                return NotFound();
            }

            var normalizedTerm = term?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedTerm) || normalizedTerm.Length < 2)
            {
                return Json(Array.Empty<object>());
            }

            var suggestions = BuildPlayerCharacterCards(player, normalizedTerm)
                .Take(8)
                .Select(character => new
                {
                    value = character.Character.CharacterName,
                    label = $"{character.Character.CharacterName} - {character.Character.Class} - Level {character.Character.Level?.Level ?? 0}",
                    characterId = character.Character.CharacterId
                })
                .ToList();

            return Json(suggestions);
        }

        [HttpGet]
        [Route("Edit/{id?}")]
        public IActionResult Edit(int id)
        {
            var player = _playerRepository.GetPlayerById(id);
            if (player == null)
            {
                return NotFound();
            }

            var current = GetCurrentPlayer();
            if (current != null && !current.IsAdmin && current.PlayerId != id)
            {
                return Forbid();
            }

            var viewModel = new PlayerFormViewModel
            {
                PlayerId = player.PlayerId,
                Name = player.Name,
                Surname = player.Surname,
                Username = player.Username,
                Email = player.Email
            };

            viewModel.IsAdmin = player.IsAdmin;
            ViewBag.CanSetAdmin = GetCurrentPlayer()?.IsAdmin == true;

            return View(viewModel);
        }

        [HttpPost]
        [Route("Edit/{id?}")]
        public IActionResult Edit(int id, PlayerFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var player = _playerRepository.GetPlayerById(id);
            if (player == null)
            {
                return NotFound();
            }
            // Update player properties from viewModel
            player.Name = viewModel.Name;
            player.Surname = viewModel.Surname;
            player.Username = viewModel.Username;
            player.Email = viewModel.Email;

            // Only admins may change the IsAdmin flag
            var current = GetCurrentPlayer();
            if (current != null && current.IsAdmin)
            {
                player.IsAdmin = viewModel.IsAdmin;
            }

            _playerRepository.UpdatePlayer(player);

            return RedirectToAction("Details", new { id = player.PlayerId });
        }

        [HttpGet]
        [Route("Remove/{id?}")]
        public IActionResult Remove(int id)
        {
            if (!IsCurrentPlayerAdmin())
            {
                return Forbid();
            }

            var player = _playerRepository.GetPlayerById(id);
            if (player == null)
            {
                return NotFound();
            }

            var current = GetCurrentPlayer();
            if (current != null && !current.IsAdmin && current.PlayerId != id)
            {
                return Forbid();
            }

            player.CharacterList = _characterRepository.GetCharactersByPlayerId(id).ToList();

            return View(player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Remove")]
        [Route("Remove/{id?}")]
        public IActionResult RemoveConfirmed(int id)
        {
            if (!IsCurrentPlayerAdmin())
            {
                return Forbid();
            }

            var current = GetCurrentPlayer();
            if (current != null && !current.IsAdmin && current.PlayerId != id)
            {
                return Forbid();
            }

            _playerRepository.DeletePlayer(id);

            return RedirectToAction("Index");
        }

        private Player? GetCurrentPlayer()
        {
            var username = HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            return _playerRepository.GetPlayerByUsername(username);
        }

        private bool IsCurrentPlayerAdmin()
        {
            return GetCurrentPlayer()?.IsAdmin == true;
        }

        private List<Player> BuildPlayerCards(string? query)
        {
            var current = GetCurrentPlayer();
            var players = _playerRepository.GetAllPlayers()
                .Where(player => current == null || current.IsAdmin || player.PlayerId == current.PlayerId)
                .Select(player =>
                {
                    player.CharacterList = _characterRepository.GetCharactersByPlayerId(player.PlayerId)
                        .Where(character => character.DeletedAt == null)
                        .ToList();
                    return player;
                })
                .ToList();

            var normalizedTerm = query?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedTerm))
            {
                return players
                    .OrderBy(player => player.Name)
                    .ThenBy(player => player.Surname)
                    .ThenBy(player => player.PlayerId)
                    .ToList();
            }

            return players
                .Where(player => PlayerMatchesSearch(player, normalizedTerm))
                .OrderBy(player => player.Name)
                .ThenBy(player => player.Surname)
                .ThenBy(player => player.PlayerId)
                .ToList();
        }

        private static bool PlayerMatchesSearch(Player player, string searchTerm)
        {
            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (terms.Length == 0)
            {
                return false;
            }

            return terms.All(term =>
                player.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                || player.Surname.Contains(term, StringComparison.OrdinalIgnoreCase)
                || player.Username.Contains(term, StringComparison.OrdinalIgnoreCase)
                || player.Email.Contains(term, StringComparison.OrdinalIgnoreCase)
                || player.PlayerId.ToString().Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        private List<CharacterWithPlayerViewModel> BuildPlayerCharacterCards(Player player, string? query)
        {
            var characters = _characterRepository.GetCharactersByPlayerId(player.PlayerId)
                .Where(character => character.DeletedAt == null)
                .Select(character => new CharacterWithPlayerViewModel
                {
                    Character = character,
                    PlayerId = player.PlayerId,
                    PlayerName = $"{player.Name} {player.Surname}"
                })
                .ToList();

            var normalizedTerm = query?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedTerm))
            {
                return characters
                    .OrderBy(character => character.Character.CharacterName)
                    .ThenBy(character => character.Character.CharacterId)
                    .ToList();
            }

            return characters
                .Where(character => CharacterMatchesSearch(character, normalizedTerm))
                .OrderBy(character => character.Character.CharacterName)
                .ThenBy(character => character.Character.CharacterId)
                .ToList();
        }

        private static bool CharacterMatchesSearch(CharacterWithPlayerViewModel character, string searchTerm)
        {
            return character.Character.CharacterName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || character.Character.Class.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || character.Character.Race.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || character.Character.Background.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || character.Character.Alignment.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || (character.Character.Level?.Level.ToString() ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }

    }
}
