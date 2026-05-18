using DnD_Character_Sheet_Creator.Repositories;
using DnD_Character_Sheet_Creator.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Route("")]
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

            player.CharacterList = _characterRepository.GetCharactersByPlayerId(id).ToList();
            var characters = player.CharacterList;
            ViewBag.Characters = characters;
            return View(player);
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

            var viewModel = new PlayerFormViewModel
            {
                PlayerId = player.PlayerId,
                Name = player.Name,
                Surname = player.Surname,
                Username = player.Username,
                Email = player.Email
            };

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

            _playerRepository.UpdatePlayer(player);

            return RedirectToAction("Details", new { id = player.PlayerId });
        }
    }
}
