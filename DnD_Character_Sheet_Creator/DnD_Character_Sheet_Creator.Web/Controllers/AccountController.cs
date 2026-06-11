using System.Security.Claims;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Repositories;
using DnD_Character_Sheet_Creator.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DnD_Character_Sheet_Creator.Web.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(
            IPlayerRepository playerRepository,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _playerRepository = playerRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("SignIn")]
        public IActionResult SignIn(string? returnUrl = null)
        {
            return View(new AccountSignInViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SignIn")]
        public async Task<IActionResult> SignIn(AccountSignInViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, viewModel.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(viewModel);
            }

            var player = _playerRepository.GetPlayerByUsername(viewModel.UserName);
            if (player != null)
            {
                player.LastLogin = DateTime.UtcNow;
                _playerRepository.UpdatePlayer(player);
            }

            if (!string.IsNullOrWhiteSpace(viewModel.ReturnUrl) && Url.IsLocalUrl(viewModel.ReturnUrl))
            {
                return Redirect(viewModel.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Register")]
        public IActionResult Register(string? returnUrl = null)
        {
            return View(new AccountRegisterViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        public async Task<IActionResult> Register(AccountRegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (_playerRepository.GetPlayerByUsername(viewModel.UserName) != null)
            {
                ModelState.AddModelError(nameof(viewModel.UserName), "That username is already taken.");
                return View(viewModel);
            }

            if (await _userManager.FindByNameAsync(viewModel.UserName) != null)
            {
                ModelState.AddModelError(nameof(viewModel.UserName), "That username is already taken in Identity.");
                return View(viewModel);
            }

            if (await _userManager.FindByEmailAsync(viewModel.Email) != null)
            {
                ModelState.AddModelError(nameof(viewModel.Email), "That email address is already registered.");
                return View(viewModel);
            }

            var player = new Player
            {
                Name = viewModel.Name,
                Surname = viewModel.Surname,
                Username = viewModel.UserName,
                Email = viewModel.Email,
                Password = viewModel.Password,
                LastLogin = DateTime.UtcNow,
                Role = RoleEnum.User,
                CharacterList = new List<Character>()
            };

            _playerRepository.AddPlayer(player);

            var user = new AppUser
            {
                PlayerId = player.PlayerId,
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                OIB = viewModel.OIB,
                JMBG = viewModel.JMBG
            };

            var createResult = await _userManager.CreateAsync(user, viewModel.Password);
            if (!createResult.Succeeded)
            {
                _playerRepository.DeletePlayer(player.PlayerId);
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(viewModel);
            }

            await _userManager.AddToRoleAsync(user, RoleEnum.User.ToString());
            await _signInManager.SignInAsync(user, isPersistent: true);

            if (!string.IsNullOrWhiteSpace(viewModel.ReturnUrl) && Url.IsLocalUrl(viewModel.ReturnUrl))
            {
                return Redirect(viewModel.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ExternalLogin")]
        public IActionResult ExternalLogin(string provider = "Google", string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (!string.IsNullOrWhiteSpace(remoteError))
            {
                ModelState.AddModelError(string.Empty, remoteError);
                return RedirectToAction(nameof(SignIn), new { returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(SignIn), new { returnUrl });
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (existingUser != null)
                {
                    var existingPlayer = _playerRepository.GetPlayerByUsername(existingUser.UserName ?? string.Empty);
                    if (existingPlayer != null)
                    {
                        existingPlayer.LastLogin = DateTime.UtcNow;
                        _playerRepository.UpdatePlayer(existingPlayer);
                    }
                }

                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? $"{info.ProviderKey}@external.local";
            var name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? info.Principal.FindFirstValue(ClaimTypes.Name) ?? "External";
            var surname = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "Player";
            var userName = email.Contains('@') ? email.Split('@')[0] : email;

            var player = new Player
            {
                Name = name,
                Surname = surname,
                Username = userName,
                Email = email,
                Password = string.Empty,
                LastLogin = DateTime.UtcNow,
                Role = RoleEnum.User,
                CharacterList = new List<Character>()
            };

            _playerRepository.AddPlayer(player);

            var user = new AppUser
            {
                PlayerId = player.PlayerId,
                UserName = userName,
                Email = email,
                OIB = player.PlayerId.ToString().PadLeft(11, '0'),
                JMBG = player.PlayerId.ToString().PadLeft(13, '0')
            };

            var createUserResult = await _userManager.CreateAsync(user);
            if (!createUserResult.Succeeded)
            {
                foreach (var error in createUserResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return RedirectToAction(nameof(SignIn), new { returnUrl });
            }

            await _userManager.AddLoginAsync(user, info);
            await _userManager.AddToRoleAsync(user, RoleEnum.User.ToString());
            await _signInManager.SignInAsync(user, isPersistent: true);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
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
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}