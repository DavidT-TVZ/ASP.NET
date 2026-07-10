using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Repositories;
using DnD_Character_Sheet_Creator.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Web.Controllers
{
    [Route("[controller]")]
    [Route("Codex")]
    public class CodexController : Controller
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly DnDDbContext _dbContext;

        public CodexController(IPlayerRepository playerRepository, ICharacterRepository characterRepository, DnDDbContext dbContext)
        {
            _playerRepository = playerRepository;
            _characterRepository = characterRepository;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public IActionResult Index(string? query)
        {
            var normalizedQuery = query?.Trim() ?? string.Empty;
            var model = new CodexSearchViewModel
            {
                Query = normalizedQuery,
                Players = SearchPlayers(normalizedQuery),
                Characters = SearchCharacters(normalizedQuery),
                Equipment = SearchEquipment(normalizedQuery)
            };

            ViewData["Title"] = "Codex";
            ViewData["SearchCount"] = model.Players.Count + model.Characters.Count + model.Equipment.Count;

            return View(model);
        }

        private List<Player> SearchPlayers(string query)
        {
            var players = _playerRepository.GetAllPlayers().ToList();
            if (string.IsNullOrWhiteSpace(query))
            {
                return players.OrderBy(player => player.Name).ThenBy(player => player.Surname).ToList();
            }

            return players
                .Where(player =>
                    player.PlayerId.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    player.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    player.Surname.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    player.Username.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    player.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(player => player.Name)
                .ThenBy(player => player.Surname)
                .ToList();
        }

        private List<Character> SearchCharacters(string query)
        {
            var characters = _characterRepository.GetAllCharacters().ToList();
            if (string.IsNullOrWhiteSpace(query))
            {
                return characters.OrderBy(character => character.CharacterName).ToList();
            }

            return characters
                .Where(character =>
                    character.CharacterId.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    character.CharacterName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    character.Class.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    character.Race.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    character.Background.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    character.Alignment.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (character.Level?.Level.ToString() ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(character => character.CharacterName)
                .ToList();
        }

        private List<Equipment> SearchEquipment(string query)
        {
            var equipment = _dbContext.Equipment
                .Where(item => item.DeletedAt == null)
                .AsNoTracking()
                .ToList();

            if (string.IsNullOrWhiteSpace(query))
            {
                return equipment.OrderBy(item => item.Name).ToList();
            }

            return equipment
                .Where(item =>
                    item.EquipmentId.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    item.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (item.Type ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    item.Cost.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    item.Weight.ToString().Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Name)
                .ToList();
        }
    }
}