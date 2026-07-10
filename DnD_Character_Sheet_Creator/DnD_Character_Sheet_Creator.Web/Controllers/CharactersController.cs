using DnD_Character_Sheet_Creator.Repositories;
using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Web.Controllers
{
    [Route("[controller]")]
    [Route("Adventurers")]
    public class CharactersController : Controller
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly DnDDbContext _dbContext;

        public CharactersController(ICharacterRepository characterRepository, IPlayerRepository playerRepository, DnDDbContext dbContext)
        {
            _characterRepository = characterRepository;
            _playerRepository = playerRepository;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public IActionResult Index(string? query)
        {
            ViewData["SearchQuery"] = query ?? string.Empty;

            return View(BuildCharacterCards(query));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string? query)
        {
            ViewData["SearchQuery"] = query ?? string.Empty;

            return PartialView("_CharacterCards", BuildCharacterCards(query));
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

        [Authorize]
        [HttpGet]
        [Route("Create")]
        public IActionResult Create(int? playerId)
        {
            var current = GetCurrentPlayer();
            if (current != null && current.Role != RoleEnum.Admin)
            {
                playerId = current.PlayerId;
            }

            var viewModel = new CharacterFormViewModel
            {
                PlayerId = playerId ?? 0,
                AvailablePlayers = GetPlayerOptions()
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [Route("Create")]
        public IActionResult Create(CharacterFormViewModel viewModel)
        {
            var current = GetCurrentPlayer();
            if (current != null && current.Role != RoleEnum.Admin)
            {
                viewModel.PlayerId = current.PlayerId;
            }

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
                CurrentExperiencePoints = viewModel.CurrentExperiencePoints,
                DateOfLastLevelUp = viewModel.DateOfLastLevelUp,
                Strength = viewModel.Strength,
                Dexterity = viewModel.Dexterity,
                Constitution = viewModel.Constitution,
                Intelligence = viewModel.Intelligence,
                Wisdom = viewModel.Wisdom,
                Charisma = viewModel.Charisma,
                StrengthSaveProficient = viewModel.StrengthSaveProficient,
                DexteritySaveProficient = viewModel.DexteritySaveProficient,
                ConstitutionSaveProficient = viewModel.ConstitutionSaveProficient,
                IntelligenceSaveProficient = viewModel.IntelligenceSaveProficient,
                WisdomSaveProficient = viewModel.WisdomSaveProficient,
                CharismaSaveProficient = viewModel.CharismaSaveProficient,
                AcrobaticsProficient = viewModel.AcrobaticsProficient,
                AnimalHandlingProficient = viewModel.AnimalHandlingProficient,
                ArcanaProficient = viewModel.ArcanaProficient,
                AthleticsProficient = viewModel.AthleticsProficient,
                DeceptionProficient = viewModel.DeceptionProficient,
                HistoryProficient = viewModel.HistoryProficient,
                InsightProficient = viewModel.InsightProficient,
                IntimidationProficient = viewModel.IntimidationProficient,
                InvestigationProficient = viewModel.InvestigationProficient,
                MedicineProficient = viewModel.MedicineProficient,
                NatureProficient = viewModel.NatureProficient,
                PerceptionProficient = viewModel.PerceptionProficient,
                PerformanceProficient = viewModel.PerformanceProficient,
                PersuasionProficient = viewModel.PersuasionProficient,
                ReligionProficient = viewModel.ReligionProficient,
                SleightOfHandProficient = viewModel.SleightOfHandProficient,
                StealthProficient = viewModel.StealthProficient,
                SurvivalProficient = viewModel.SurvivalProficient,
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

        [Authorize]
        [HttpGet]
        [Route("Edit/{id?}")]
        public IActionResult Edit(int id)
        {
            var character = _characterRepository.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            if (!CanModifyCharacter(character.PlayerId))
            {
                return Forbid();
            }

            var viewModel = new CharacterFormViewModel
            {
                CharacterId = character.CharacterId,
                PlayerId = character.PlayerId,
                CharacterName = character.CharacterName,
                Race = character.Race,
                Background = character.Background,
                Alignment = character.Alignment,
                Class = character.Class,
                LevelId = character.Level?.LevelId,
                AvailablePlayers = GetPlayerOptions(),
                LevelOptions = GetLevelOptions()
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id?}")]
        public IActionResult Edit(CharacterFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailablePlayers = GetPlayerOptions();
                viewModel.LevelOptions = GetLevelOptions();
                return View(viewModel);
            }

            if (viewModel.CharacterId == null)
            {
                return BadRequest();
            }

            var character = _characterRepository.GetCharacterById(viewModel.CharacterId.Value);
            if (character == null)
            {
                return NotFound();
            }

            if (!CanModifyCharacter(character.PlayerId))
            {
                return Forbid();
            }

            var current = GetCurrentPlayer();
            if (current != null && current.Role != RoleEnum.Admin)
            {
                viewModel.PlayerId = current.PlayerId;
            }

            character.PlayerId = viewModel.PlayerId;
            character.CharacterName = viewModel.CharacterName;
            character.Race = viewModel.Race!.Value;
            character.Background = viewModel.Background!.Value;
            character.Alignment = viewModel.Alignment!.Value;
            character.Class = viewModel.Class!.Value;
            character.CurrentExperiencePoints = viewModel.CurrentExperiencePoints;
            character.DateOfLastLevelUp = viewModel.DateOfLastLevelUp;
            character.Strength = viewModel.Strength;
            character.Dexterity = viewModel.Dexterity;
            character.Constitution = viewModel.Constitution;
            character.Intelligence = viewModel.Intelligence;
            character.Wisdom = viewModel.Wisdom;
            character.Charisma = viewModel.Charisma;
            character.StrengthSaveProficient = viewModel.StrengthSaveProficient;
            character.DexteritySaveProficient = viewModel.DexteritySaveProficient;
            character.ConstitutionSaveProficient = viewModel.ConstitutionSaveProficient;
            character.IntelligenceSaveProficient = viewModel.IntelligenceSaveProficient;
            character.WisdomSaveProficient = viewModel.WisdomSaveProficient;
            character.CharismaSaveProficient = viewModel.CharismaSaveProficient;
            character.AcrobaticsProficient = viewModel.AcrobaticsProficient;
            character.AnimalHandlingProficient = viewModel.AnimalHandlingProficient;
            character.ArcanaProficient = viewModel.ArcanaProficient;
            character.AthleticsProficient = viewModel.AthleticsProficient;
            character.DeceptionProficient = viewModel.DeceptionProficient;
            character.HistoryProficient = viewModel.HistoryProficient;
            character.InsightProficient = viewModel.InsightProficient;
            character.IntimidationProficient = viewModel.IntimidationProficient;
            character.InvestigationProficient = viewModel.InvestigationProficient;
            character.MedicineProficient = viewModel.MedicineProficient;
            character.NatureProficient = viewModel.NatureProficient;
            character.PerceptionProficient = viewModel.PerceptionProficient;
            character.PerformanceProficient = viewModel.PerformanceProficient;
            character.PersuasionProficient = viewModel.PersuasionProficient;
            character.ReligionProficient = viewModel.ReligionProficient;
            character.SleightOfHandProficient = viewModel.SleightOfHandProficient;
            character.StealthProficient = viewModel.StealthProficient;
            character.SurvivalProficient = viewModel.SurvivalProficient;
            character.LevelId = viewModel.LevelId;

            _characterRepository.UpdateCharacter(character);

            return RedirectToAction("Details", new { id = character.CharacterId });
        }

        [Authorize]
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

            var current = GetCurrentPlayer();
            if (!CanViewCharacter(owner.Player.PlayerId))
            {
                return Forbid();
            }

            return View(new CharacterWithPlayerViewModel
            {
                Character = character,
                PlayerId = owner.Player.PlayerId,
                PlayerName = $"{owner.Player.Name} {owner.Player.Surname}",
                OwnerUsername = owner.Player.Username
            });
        }

        [Authorize]
        [HttpGet]
        [Route("{id:int}/Attachments")]
        public IActionResult Attachments(int id)
        {
            var character = _characterRepository.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            if (!CanViewCharacter(character.PlayerId))
            {
                return Forbid();
            }

            var attachments = _dbContext.Attachments
                .AsNoTracking()
                .Where(attachment => attachment.CharacterId == id && attachment.DeletedAt == null)
                .OrderByDescending(attachment => attachment.CreatedAt)
                .ToList();

            return PartialView("_AttachmentList", attachments);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id:int}/UploadAttachment")]
        public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
        {
            var character = _characterRepository.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            if (!CanModifyCharacter(character.PlayerId))
            {
                return Forbid();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "characters", id.ToString());
            Directory.CreateDirectory(uploadsRoot);

            var storedFileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var storedRelativePath = Path.Combine("/uploads/characters", id.ToString(), storedFileName).Replace("\\", "/");
            var storedPhysicalPath = Path.Combine(uploadsRoot, storedFileName);

            await using (var stream = new FileStream(storedPhysicalPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                CharacterId = id,
                FileName = Path.GetFileName(file.FileName),
                FilePath = storedRelativePath,
                ContentType = file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Attachments.Add(attachment);
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteAttachment/{id:int}")]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            var attachment = await _dbContext.Attachments
                .Include(item => item.Character)
                .FirstOrDefaultAsync(item => item.AttachmentId == id && item.DeletedAt == null);

            if (attachment == null)
            {
                return NotFound();
            }

            if (attachment.Character == null || !CanModifyCharacter(attachment.Character.PlayerId))
            {
                return Forbid();
            }

            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attachment.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            attachment.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Json(new { success = true });
        }

        [Authorize]
        [HttpGet]
        [Route("Remove/{id?}")]
        public IActionResult Remove(int id)
        {
            var character = _characterRepository.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            if (!CanModifyCharacter(character.PlayerId))
            {
                return Forbid();
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
                PlayerName = $"{player.Name} {player.Surname}",
                OwnerUsername = player.Username
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Remove")]
        [Route("Remove/{id?}")]
        public IActionResult RemoveConfirmed(int id)
        {
            var character = _characterRepository.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            if (!CanModifyCharacter(character.PlayerId))
            {
                return Forbid();
            }

            var attachments = _dbContext.Attachments
                .Where(attachment => attachment.CharacterId == id && attachment.DeletedAt == null)
                .ToList();

            foreach (var attachment in attachments)
            {
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", attachment.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }

                attachment.DeletedAt = DateTime.UtcNow;
            }

            _dbContext.SaveChanges();

            _characterRepository.DeleteCharacter(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("{id:int}/EquipmentSearch")]
        public IActionResult EquipmentSearch(int id, string? query)
        {
            var character = _characterRepository.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            if (!CanViewCharacter(character.PlayerId))
            {
                return Forbid();
            }

            var equipment = BuildEquipmentCards(id, query);

            return PartialView("_EquipmentCards", equipment);
        }

        [HttpGet]
        [Route("{id:int}/EquipmentAutocomplete")]
        public IActionResult EquipmentAutocomplete(int id, string term)
        {
            var character = _characterRepository.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            if (!CanViewCharacter(character.PlayerId))
            {
                return Forbid();
            }

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
            var current = GetCurrentPlayer();
            var cards = _playerRepository.GetAllPlayers()
                .Where(player => current == null || current.Role is RoleEnum.Admin or RoleEnum.Manager || player.PlayerId == current.PlayerId)
                .SelectMany(player => _characterRepository.GetCharactersByPlayerId(player.PlayerId)
                    .Select(character => new CharacterWithPlayerViewModel
                    {
                        Character = character,
                        PlayerId = player.PlayerId,
                        PlayerName = $"{player.Name} {player.Surname}",
                        OwnerUsername = player.Username
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
                .Where(equipment => equipment.DeletedAt == null)
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

        private Player? GetCurrentPlayer()
        {
            var username = HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            return _playerRepository.GetPlayerByUsername(username);
        }

        private bool CanViewCharacter(int playerId)
        {
            var current = GetCurrentPlayer();
            return current == null || current.Role is RoleEnum.Admin or RoleEnum.Manager || current.PlayerId == playerId;
        }

        private bool CanModifyCharacter(int playerId)
        {
            var current = GetCurrentPlayer();
            return current == null || current.Role == RoleEnum.Admin || current.PlayerId == playerId;
        }

        private List<SelectListItem> GetPlayerOptions()
        {
            var current = GetCurrentPlayer();

            return _playerRepository.GetAllPlayers()
                .Where(player => current == null || current.Role == RoleEnum.Admin || player.PlayerId == current.PlayerId)
                .Select(player => new SelectListItem
                {
                    Value = player.PlayerId.ToString(),
                    Text = $"{player.Name} {player.Surname}"
                })
                .ToList();
        }

        private List<SelectListItem> GetLevelOptions()
        {
            return _characterRepository.GetAllCharacters()
                .Select(c => c.Level)
                .Where(l => l != null)
                .Select(l => new SelectListItem
                {
                    Value = l!.LevelId.ToString(),
                    Text = $"Level {l!.Level}"
                })
                .GroupBy(item => item.Value)
                .Select(g => g.First())
                .OrderBy(item => item.Text)
                .ToList();
        }

        [Authorize]
        [HttpGet]
        [Route("{id:int}/equipment/create-form")]
        public IActionResult EquipmentCreateForm(int id)
        {
            var character = _characterRepository.GetCharacterById(id);
            if (character == null) return NotFound();
            if (!CanModifyCharacter(character.PlayerId)) return Forbid();

            var vm = new ViewModels.EquipmentFormViewModel
            {
                CharacterId = id
            };

            return View("CreateEquipment", vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id:int}/equipment/create")]
        public IActionResult CreateEquipment(ViewModels.EquipmentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateEquipment", vm);
            }

            var character = _characterRepository.GetCharacterById(vm.CharacterId);
            if (character == null) return NotFound();
            if (!CanModifyCharacter(character.PlayerId)) return Forbid();

            var equipment = new Models.Equipment
            {
                Type = vm.Type,
                Name = vm.Name,
                Cost = vm.Cost,
                Weight = vm.Weight
            };

            equipment.CharacterId = vm.CharacterId;
            character.EquipmentList.Add(equipment);
            _characterRepository.UpdateCharacter(character);

            return RedirectToAction("Details", new { id = vm.CharacterId });
        }

        [Authorize]
        [HttpGet]
        [Route("{characterId:int}/equipment/{equipmentId:int}/edit-form")]
        public IActionResult EquipmentEditForm(int characterId, int equipmentId)
        {
            var character = _characterRepository.GetCharacterById(characterId);
            if (character == null) return NotFound();
            if (!CanModifyCharacter(character.PlayerId)) return Forbid();

            var equipment = character.EquipmentList.FirstOrDefault(e => e.EquipmentId == equipmentId);
            if (equipment == null) return NotFound();
            equipment.CharacterId = characterId;

            var vm = new ViewModels.EquipmentFormViewModel
            {
                EquipmentId = equipment.EquipmentId,
                CharacterId = characterId,
                Type = equipment.Type,
                Name = equipment.Name,
                Cost = equipment.Cost,
                Weight = equipment.Weight
            };

            return View("EditEquipment", vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{characterId:int}/equipment/{equipmentId:int}/edit")]
        public IActionResult EditEquipment(ViewModels.EquipmentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("EditEquipment", vm);
            }

            var character = _characterRepository.GetCharacterById(vm.CharacterId);
            if (character == null) return NotFound();
            if (!CanModifyCharacter(character.PlayerId)) return Forbid();

            var equipment = character.EquipmentList.FirstOrDefault(e => e.EquipmentId == vm.EquipmentId);
            if (equipment == null) return NotFound();

            equipment.CharacterId = vm.CharacterId;
            equipment.Type = vm.Type;
            equipment.Name = vm.Name;
            equipment.Cost = vm.Cost;
            equipment.Weight = vm.Weight;

            _characterRepository.UpdateCharacter(character);

            return RedirectToAction("Details", new { id = vm.CharacterId });
        }

        [Authorize]
        [HttpGet]
        [Route("{characterId:int}/equipment/{equipmentId:int}/remove-form")]
        public IActionResult EquipmentRemoveForm(int characterId, int equipmentId)
        {
            var character = _characterRepository.GetCharacterById(characterId);
            if (character == null) return NotFound();
            if (!CanModifyCharacter(character.PlayerId)) return Forbid();

            var equipment = character.EquipmentList.FirstOrDefault(e => e.EquipmentId == equipmentId);
            if (equipment == null) return NotFound();

            equipment.CharacterId = characterId;

            return View("RemoveEquipment", equipment);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{characterId:int}/equipment/{equipmentId:int}/remove")]
        public IActionResult RemoveEquipment(int characterId, int equipmentId)
        {
            var character = _characterRepository.GetCharacterById(characterId);
            if (character == null) return NotFound();
            if (!CanModifyCharacter(character.PlayerId)) return Forbid();

            var equipment = character.EquipmentList.FirstOrDefault(e => e.EquipmentId == equipmentId);
            if (equipment == null) return NotFound();

            character.EquipmentList.Remove(equipment);

            if (!equipment.Characters.Any())
            {
                equipment.DeletedAt = DateTime.UtcNow;
            }

            _characterRepository.UpdateCharacter(character);

            return RedirectToAction("Details", new { id = characterId });
        }
    }
}
