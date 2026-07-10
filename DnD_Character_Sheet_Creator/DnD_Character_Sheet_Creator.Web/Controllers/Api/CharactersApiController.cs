using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/characters")]
public class CharactersApiController : ControllerBase
{
    private readonly DnDDbContext _context;

    public CharactersApiController(DnDDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterDto>>> GetAll([FromQuery(Name = "search")] string? search, [FromQuery] int? playerId)
    {
        var current = await GetCurrentPlayerAsync();

        var characters = await _context.Characters
            .Where(character => character.DeletedAt == null)
            .Include(character => character.Player)
            .Include(character => character.Level)
            .Include(character => character.EquipmentList)
            .AsNoTracking()
            .ToListAsync();

        if (current != null && current.Role == RoleEnum.User)
        {
            characters = characters.Where(character => character.PlayerId == current.PlayerId).ToList();
        }
        else if (current != null && current.Role == RoleEnum.Manager)
        {
            // Managers can see all players/characters per the requested rules.
        }

        if (playerId.HasValue)
        {
            characters = characters.Where(character => character.PlayerId == playerId.Value).ToList();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedQuery = search.Trim();
            characters = characters.Where(character => CharacterMatchesSearch(character, normalizedQuery)).ToList();
        }

        return Ok(characters
            .OrderBy(character => character.CharacterName)
            .ThenBy(character => character.CharacterId)
            .Select(ToDto)
            .ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CharacterDto>> GetById(int id)
    {
        var current = await GetCurrentPlayerAsync();

        var character = await _context.Characters
            .Where(item => item.CharacterId == id && item.DeletedAt == null)
            .Include(item => item.Player)
            .Include(item => item.Level)
            .Include(item => item.EquipmentList)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (character == null)
        {
            return NotFound();
        }

        if (current != null && current.Role == RoleEnum.User && character.PlayerId != current.PlayerId)
        {
            return Forbid();
        }

        return Ok(ToDto(character));
    }

    [HttpPost]
    public async Task<ActionResult<CharacterDto>> Create([FromBody] CharacterUpsertDto dto)
    {
        var current = await GetCurrentPlayerAsync();
        if (current == null)
        {
            return Forbid();
        }

        if (current.Role != RoleEnum.Admin)
        {
            dto.PlayerId = current.PlayerId;
        }

        var player = await _context.Players.FirstOrDefaultAsync(item => item.PlayerId == dto.PlayerId && item.DeletedAt == null);
        if (player == null)
        {
            return BadRequest($"Player {dto.PlayerId} was not found.");
        }

        CharacterLevel? level = null;
        if (dto.LevelId.HasValue)
        {
            level = await _context.CharacterLevels.FirstOrDefaultAsync(item => item.LevelId == dto.LevelId.Value);
            if (level == null)
            {
                return NotFound($"Character level {dto.LevelId.Value} was not found.");
            }
            // Note: allow assigning existing levels to multiple characters on create
        }
        else
        {
            level = CreateDefaultLevel();
        }

        var character = new Character
        {
            PlayerId = dto.PlayerId,
            CharacterName = dto.CharacterName.Trim(),
            Race = dto.Race!.Value,
            Background = dto.Background!.Value,
            Alignment = dto.Alignment!.Value,
            Class = dto.Class!.Value,
            CurrentExperiencePoints = dto.CurrentExperiencePoints ?? 0,
            DateOfLastLevelUp = dto.DateOfLastLevelUp,
            Strength = dto.Strength ?? 10,
            Dexterity = dto.Dexterity ?? 10,
            Constitution = dto.Constitution ?? 10,
            Intelligence = dto.Intelligence ?? 10,
            Wisdom = dto.Wisdom ?? 10,
            Charisma = dto.Charisma ?? 10,
            StrengthSaveProficient = dto.StrengthSaveProficient,
            DexteritySaveProficient = dto.DexteritySaveProficient,
            ConstitutionSaveProficient = dto.ConstitutionSaveProficient,
            IntelligenceSaveProficient = dto.IntelligenceSaveProficient,
            WisdomSaveProficient = dto.WisdomSaveProficient,
            CharismaSaveProficient = dto.CharismaSaveProficient,
            AcrobaticsProficient = dto.AcrobaticsProficient,
            AnimalHandlingProficient = dto.AnimalHandlingProficient,
            ArcanaProficient = dto.ArcanaProficient,
            AthleticsProficient = dto.AthleticsProficient,
            DeceptionProficient = dto.DeceptionProficient,
            HistoryProficient = dto.HistoryProficient,
            InsightProficient = dto.InsightProficient,
            IntimidationProficient = dto.IntimidationProficient,
            InvestigationProficient = dto.InvestigationProficient,
            MedicineProficient = dto.MedicineProficient,
            NatureProficient = dto.NatureProficient,
            PerceptionProficient = dto.PerceptionProficient,
            PerformanceProficient = dto.PerformanceProficient,
            PersuasionProficient = dto.PersuasionProficient,
            ReligionProficient = dto.ReligionProficient,
            SleightOfHandProficient = dto.SleightOfHandProficient,
            StealthProficient = dto.StealthProficient,
            SurvivalProficient = dto.SurvivalProficient,
            Level = level
        };

        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        await _context.Entry(character).Reference(item => item.Player).LoadAsync();
        await _context.Entry(character).Reference(item => item.Level).LoadAsync();
        await _context.Entry(character).Collection(item => item.EquipmentList).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = character.CharacterId }, ToDto(character));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CharacterDto>> Update(int id, [FromBody] CharacterUpsertDto dto)
    {
        var current = await GetCurrentPlayerAsync();
        if (current == null)
        {
            return Forbid();
        }

        var character = await _context.Characters
            .Include(item => item.Player)
            .Include(item => item.Level)
            .Include(item => item.EquipmentList)
            .FirstOrDefaultAsync(item => item.CharacterId == id && item.DeletedAt == null);

        if (character == null)
        {
            return NotFound();
        }

        if (current.Role == RoleEnum.User && character.PlayerId != current.PlayerId)
        {
            return Forbid();
        }

        if (current.Role != RoleEnum.Admin)
        {
            dto.PlayerId = current.PlayerId;
        }

        var player = await _context.Players.FirstOrDefaultAsync(item => item.PlayerId == dto.PlayerId && item.DeletedAt == null);
        if (player == null)
        {
            return NotFound($"Player {dto.PlayerId} was not found.");
        }

        character.PlayerId = dto.PlayerId;
        character.CharacterName = dto.CharacterName.Trim();
        character.Race = dto.Race!.Value;
        character.Background = dto.Background!.Value;
        character.Alignment = dto.Alignment!.Value;
        character.Class = dto.Class!.Value;
        character.CurrentExperiencePoints = dto.CurrentExperiencePoints ?? character.CurrentExperiencePoints;
        character.DateOfLastLevelUp = dto.DateOfLastLevelUp;
        character.Strength = dto.Strength ?? character.Strength;
        character.Dexterity = dto.Dexterity ?? character.Dexterity;
        character.Constitution = dto.Constitution ?? character.Constitution;
        character.Intelligence = dto.Intelligence ?? character.Intelligence;
        character.Wisdom = dto.Wisdom ?? character.Wisdom;
        character.Charisma = dto.Charisma ?? character.Charisma;
        character.StrengthSaveProficient = dto.StrengthSaveProficient;
        character.DexteritySaveProficient = dto.DexteritySaveProficient;
        character.ConstitutionSaveProficient = dto.ConstitutionSaveProficient;
        character.IntelligenceSaveProficient = dto.IntelligenceSaveProficient;
        character.WisdomSaveProficient = dto.WisdomSaveProficient;
        character.CharismaSaveProficient = dto.CharismaSaveProficient;
        character.AcrobaticsProficient = dto.AcrobaticsProficient;
        character.AnimalHandlingProficient = dto.AnimalHandlingProficient;
        character.ArcanaProficient = dto.ArcanaProficient;
        character.AthleticsProficient = dto.AthleticsProficient;
        character.DeceptionProficient = dto.DeceptionProficient;
        character.HistoryProficient = dto.HistoryProficient;
        character.InsightProficient = dto.InsightProficient;
        character.IntimidationProficient = dto.IntimidationProficient;
        character.InvestigationProficient = dto.InvestigationProficient;
        character.MedicineProficient = dto.MedicineProficient;
        character.NatureProficient = dto.NatureProficient;
        character.PerceptionProficient = dto.PerceptionProficient;
        character.PerformanceProficient = dto.PerformanceProficient;
        character.PersuasionProficient = dto.PersuasionProficient;
        character.ReligionProficient = dto.ReligionProficient;
        character.SleightOfHandProficient = dto.SleightOfHandProficient;
        character.StealthProficient = dto.StealthProficient;
        character.SurvivalProficient = dto.SurvivalProficient;

        if (dto.LevelId.HasValue)
        {
            var level = await _context.CharacterLevels.FirstOrDefaultAsync(item => item.LevelId == dto.LevelId.Value);
            if (level == null)
            {
                return NotFound($"Character level {dto.LevelId.Value} was not found.");
            }

            var levelInUse = await _context.Characters.AnyAsync(item => item.LevelId == level.LevelId && item.CharacterId != id);
            if (levelInUse)
            {
                return Conflict($"Character level {level.LevelId} is already assigned to a character.");
            }

            character.LevelId = level.LevelId;
            character.Level = level;
        }

        await _context.SaveChangesAsync();

        await _context.Entry(character).Reference(item => item.Player).LoadAsync();
        await _context.Entry(character).Reference(item => item.Level).LoadAsync();
        await _context.Entry(character).Collection(item => item.EquipmentList).LoadAsync();

        return Ok(ToDto(character));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var current = await GetCurrentPlayerAsync();
        if (current == null)
        {
            return Forbid();
        }

        var character = await _context.Characters
            .Include(item => item.EquipmentList)
            .FirstOrDefaultAsync(item => item.CharacterId == id && item.DeletedAt == null);

        if (character == null)
        {
            return NotFound();
        }

        if (current.Role == RoleEnum.User && character.PlayerId != current.PlayerId)
        {
            return Forbid();
        }

        if (current.Role != RoleEnum.Admin && character.PlayerId != current.PlayerId)
        {
            return Forbid();
        }

        character.DeletedAt = DateTime.UtcNow;

        foreach (var equipment in character.EquipmentList.Where(item => item.DeletedAt == null))
        {
            equipment.DeletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Player?> GetCurrentPlayerAsync()
    {
        var username = User?.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        return await _context.Players.FirstOrDefaultAsync(player => player.Username == username && player.DeletedAt == null);
    }

    private static bool CharacterMatchesSearch(Character character, string searchTerm)
    {
        return character.CharacterId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || character.PlayerId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || character.CharacterName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || character.Race.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || character.Background.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || character.Alignment.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || character.Class.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || (character.Level?.Level.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static CharacterLevel CreateDefaultLevel()
    {
        return new CharacterLevel
        {
            Level = 1,
            CurrentExperiencePoints = 0,
            ExperiencePointsToNextLevel = 300,
            ProficiencyBonus = 2,
            DateOfLastLevelUp = DateTime.UtcNow
        };
    }

    private static CharacterDto ToDto(Character character)
    {
        return new CharacterDto
        {
            CharacterId = character.CharacterId,
            PlayerId = character.PlayerId,
            CharacterName = character.CharacterName,
            Race = character.Race,
            Background = character.Background,
            Alignment = character.Alignment,
            Class = character.Class,
            CurrentExperiencePoints = character.CurrentExperiencePoints,
            DateOfLastLevelUp = character.DateOfLastLevelUp,
            Strength = character.Strength,
            Dexterity = character.Dexterity,
            Constitution = character.Constitution,
            Intelligence = character.Intelligence,
            Wisdom = character.Wisdom,
            Charisma = character.Charisma,
            StrengthSaveProficient = character.StrengthSaveProficient,
            DexteritySaveProficient = character.DexteritySaveProficient,
            ConstitutionSaveProficient = character.ConstitutionSaveProficient,
            IntelligenceSaveProficient = character.IntelligenceSaveProficient,
            WisdomSaveProficient = character.WisdomSaveProficient,
            CharismaSaveProficient = character.CharismaSaveProficient,
            AcrobaticsProficient = character.AcrobaticsProficient,
            AnimalHandlingProficient = character.AnimalHandlingProficient,
            ArcanaProficient = character.ArcanaProficient,
            AthleticsProficient = character.AthleticsProficient,
            DeceptionProficient = character.DeceptionProficient,
            HistoryProficient = character.HistoryProficient,
            InsightProficient = character.InsightProficient,
            IntimidationProficient = character.IntimidationProficient,
            InvestigationProficient = character.InvestigationProficient,
            MedicineProficient = character.MedicineProficient,
            NatureProficient = character.NatureProficient,
            PerceptionProficient = character.PerceptionProficient,
            PerformanceProficient = character.PerformanceProficient,
            PersuasionProficient = character.PersuasionProficient,
            ReligionProficient = character.ReligionProficient,
            SleightOfHandProficient = character.SleightOfHandProficient,
            StealthProficient = character.StealthProficient,
            SurvivalProficient = character.SurvivalProficient,
            Player = character.Player == null ? null : new PlayerSummaryDto
            {
                PlayerId = character.Player.PlayerId,
                Name = character.Player.Name,
                Surname = character.Player.Surname,
                Username = character.Player.Username,
                Email = character.Player.Email,
                Role = character.Player.Role
            },
            Level = character.Level == null ? null : new CharacterLevelDto
            {
                LevelId = character.Level.LevelId,
                Level = character.Level.Level,
                CurrentExperiencePoints = character.Level.CurrentExperiencePoints,
                ExperiencePointsToNextLevel = character.Level.ExperiencePointsToNextLevel,
                ProficiencyBonus = character.Level.ProficiencyBonus,
                DateOfLastLevelUp = character.Level.DateOfLastLevelUp
            },
            Equipment = character.EquipmentList
                .Where(item => item.DeletedAt == null)
                .OrderBy(item => item.Name)
                .ThenBy(item => item.EquipmentId)
                .Select(item => new EquipmentSummaryDto
                {
                    EquipmentId = item.EquipmentId,
                    CharacterId = item.CharacterId ?? 0,
                    Type = item.Type,
                    Name = item.Name,
                    Cost = item.Cost,
                    Weight = item.Weight
                })
                .ToList()
        };
    }
}