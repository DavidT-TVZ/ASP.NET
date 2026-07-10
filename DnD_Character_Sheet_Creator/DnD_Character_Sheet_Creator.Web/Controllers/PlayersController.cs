using DnD_Character_Sheet_Creator.Repositories;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DnD_Character_Sheet_Creator.Web.Controllers
{
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

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public IActionResult Index(string? query)
        {
            ViewData["SearchQuery"] = query ?? string.Empty;

            return View(BuildPlayerCards(query));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string? query)
        {
            ViewData["SearchQuery"] = query ?? string.Empty;

            return PartialView("_PlayerCards", BuildPlayerCards(query));
        }

        [AllowAnonymous]
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

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        [Route("Create")]
        public IActionResult Create(int? playerId)
        {
            var current = GetCurrentPlayer();
            if (current == null || current.Role == RoleEnum.User)
            {
                return Forbid();
            }

            ViewBag.CanSetRole = current.Role is RoleEnum.Admin or RoleEnum.Manager;
            ViewBag.AvailableRoles = GetRoleOptions(current);
            return View(new PlayerFormViewModel
            {
                Name = string.Empty,
                Surname = string.Empty,
                Username = string.Empty,
                Email = string.Empty
            });
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [Route("Create")]
        public IActionResult Create(PlayerFormViewModel viewModel)
        {
            var current = GetCurrentPlayer();
            if (current == null || current.Role == RoleEnum.User)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CanSetRole = current.Role is RoleEnum.Admin or RoleEnum.Manager;
                ViewBag.AvailableRoles = GetRoleOptions(current);
                return View(viewModel);
            }

            if (current.Role == RoleEnum.Manager && viewModel.Role == RoleEnum.Admin)
            {
                ModelState.AddModelError(nameof(viewModel.Role), "Managers cannot assign the Admin role.");
                ViewBag.CanSetRole = true;
                ViewBag.AvailableRoles = GetRoleOptions(current);
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

            player.Role = viewModel.Role;

            _playerRepository.AddPlayer(player);

            return RedirectToAction("Details", new { id = player.PlayerId });
        }

        [Authorize]
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
            if (current != null && current.Role == RoleEnum.User && current.PlayerId != id)
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

        [Authorize]
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
            if (current != null && current.Role != RoleEnum.Admin && current.PlayerId != id)
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

            viewModel.Role = player.Role;
            ViewBag.CanSetRole = current?.Role == RoleEnum.Admin;
            ViewBag.AvailableRoles = GetRoleOptions(current);

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [Route("Edit/{id?}")]
        public IActionResult Edit(int id, PlayerFormViewModel viewModel)
        {
            var current = GetCurrentPlayer();

            if (!ModelState.IsValid)
            {
                ViewBag.CanSetRole = current?.Role == RoleEnum.Admin;
                ViewBag.AvailableRoles = GetRoleOptions(current);
                return View(viewModel);
            }

            var player = _playerRepository.GetPlayerById(id);
            if (player == null)
            {
                return NotFound();
            }

            if (current != null && current.Role != RoleEnum.Admin && current.PlayerId != id)
            {
                return Forbid();
            }

            // Update player properties from viewModel
            player.Name = viewModel.Name;
            player.Surname = viewModel.Surname;
            player.Username = viewModel.Username;
            player.Email = viewModel.Email;

            if (current?.Role == RoleEnum.Admin)
            {
                player.Role = viewModel.Role;
            }

            _playerRepository.UpdatePlayer(player);

            return RedirectToAction("Details", new { id = player.PlayerId });
        }

        [Authorize(Roles = "Admin")]
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
            if (current != null && current.Role != RoleEnum.Admin && current.PlayerId != id)
            {
                return Forbid();
            }

            player.CharacterList = _characterRepository.GetCharactersByPlayerId(id).ToList();

            return View(player);
        }

        [Authorize(Roles = "Admin")]
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
            if (current != null && current.Role != RoleEnum.Admin && current.PlayerId != id)
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
            return GetCurrentPlayer()?.Role == RoleEnum.Admin;
        }

        private static List<RoleEnum> GetRoleOptions(Player? current)
        {
            return current?.Role == RoleEnum.Admin
                ? new List<RoleEnum> { RoleEnum.Admin, RoleEnum.Manager, RoleEnum.User }
                : new List<RoleEnum> { RoleEnum.Manager, RoleEnum.User };
        }

        private List<Player> BuildPlayerCards(string? query)
        {
            var current = GetCurrentPlayer();
            var players = _playerRepository.GetAllPlayers()
                .Where(player => current == null || current.Role is RoleEnum.Admin or RoleEnum.Manager || player.PlayerId == current.PlayerId)
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
