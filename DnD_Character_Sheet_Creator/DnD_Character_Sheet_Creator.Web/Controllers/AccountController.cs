using System.Security.Claims;
using DnD_Character_Sheet_Creator.Repositories;
using DnD_Character_Sheet_Creator.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DnD_Character_Sheet_Creator.Web.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IPlayerRepository _playerRepository;

        public AccountController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("SignIn")]
        public IActionResult SignIn(string? returnUrl = null)
        {
            return View(new PlayerSignInViewModel
            {
                Players = BuildPlayerOptions(),
                ReturnUrl = returnUrl
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SignIn")]
        public async Task<IActionResult> SignIn(PlayerSignInViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Players = BuildPlayerOptions();
                return View(viewModel);
            }

            var player = _playerRepository.GetPlayerById(viewModel.PlayerId);
            if (player == null)
            {
                ModelState.AddModelError(nameof(viewModel.PlayerId), "That player does not exist.");
                viewModel.Players = BuildPlayerOptions();
                return View(viewModel);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, player.PlayerId.ToString()),
                new(ClaimTypes.Name, player.Username),
                new(ClaimTypes.Role, player.IsAdmin ? "Admin" : "User"),
                new("FullName", $"{player.Name} {player.Surname}")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true
                });

            if (!string.IsNullOrWhiteSpace(viewModel.ReturnUrl) && Url.IsLocalUrl(viewModel.ReturnUrl))
            {
                return Redirect(viewModel.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SignOut")]
        [Route("SignOut")]
        public async Task<IActionResult> SignOutAction()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private IEnumerable<SelectListItem> BuildPlayerOptions()
        {
            return _playerRepository.GetAllPlayers()
                .OrderBy(player => player.Name)
                .ThenBy(player => player.Surname)
                .Select(player => new SelectListItem
                {
                    Value = player.PlayerId.ToString(),
                    Text = $"{player.Name} {player.Surname} {(player.IsAdmin ? "(Admin)" : "(User)")}" 
                })
                .ToList();
        }
    }
}