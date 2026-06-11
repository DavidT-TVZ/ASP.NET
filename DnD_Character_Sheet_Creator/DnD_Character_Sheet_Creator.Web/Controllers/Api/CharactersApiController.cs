using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Web.Controllers;

[ApiController]
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
        var characters = await _context.Characters
            .Where(character => character.DeletedAt == null)
            .Include(character => character.Player)
            .Include(character => character.Level)
            .Include(character => character.EquipmentList)
            .AsNoTracking()
            .ToListAsync();

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

        return Ok(ToDto(character));
    }

    [HttpPost]
    public async Task<ActionResult<CharacterDto>> Create([FromBody] CharacterUpsertDto dto)
    {
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
        var character = await _context.Characters
            .Include(item => item.Player)
            .Include(item => item.Level)
            .Include(item => item.EquipmentList)
            .FirstOrDefaultAsync(item => item.CharacterId == id && item.DeletedAt == null);

        if (character == null)
        {
            return NotFound();
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
        var character = await _context.Characters
            .Include(item => item.EquipmentList)
            .FirstOrDefaultAsync(item => item.CharacterId == id && item.DeletedAt == null);

        if (character == null)
        {
            return NotFound();
        }

        character.DeletedAt = DateTime.UtcNow;

        foreach (var equipment in character.EquipmentList.Where(item => item.DeletedAt == null))
        {
            equipment.DeletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return NoContent();
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
                    CharacterId = item.CharacterId,
                    Type = item.Type,
                    Name = item.Name,
                    Cost = item.Cost,
                    Weight = item.Weight
                })
                .ToList()
        };
    }
}