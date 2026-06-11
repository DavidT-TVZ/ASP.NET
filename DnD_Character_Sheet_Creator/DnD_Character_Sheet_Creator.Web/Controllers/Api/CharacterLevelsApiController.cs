using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Web.Controllers;

[ApiController]
[Route("api/character-levels")]
public class CharacterLevelsApiController : ControllerBase
{
    private readonly DnDDbContext _context;

    public CharacterLevelsApiController(DnDDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterLevelDto>>> GetAll([FromQuery(Name = "search")] string? search)
    {
        var levels = await _context.CharacterLevels
            .AsNoTracking()
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedQuery = search.Trim();
            levels = levels.Where(level => LevelMatchesSearch(level, normalizedQuery)).ToList();
        }

        return Ok(levels
            .OrderBy(level => level.Level)
            .ThenBy(level => level.LevelId)
            .Select(ToDto)
            .ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CharacterLevelDto>> GetById(int id)
    {
        var level = await _context.CharacterLevels
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.LevelId == id);

        if (level == null)
        {
            return NotFound();
        }

        return Ok(ToDto(level));
    }

    [HttpPost]
    public async Task<ActionResult<CharacterLevelDto>> Create([FromBody] CharacterLevelUpsertDto dto)
    {
        // Prevent creating duplicate level values
        if (await _context.CharacterLevels.AnyAsync(cl => cl.Level == dto.Level))
        {
            return BadRequest($"Level {dto.Level} already exists.");
        }

        var level = new CharacterLevel
        {
            Level = dto.Level,
            CurrentExperiencePoints = dto.CurrentExperiencePoints,
            ExperiencePointsToNextLevel = dto.ExperiencePointsToNextLevel,
            ProficiencyBonus = dto.ProficiencyBonus,
            DateOfLastLevelUp = dto.DateOfLastLevelUp
        };

        _context.CharacterLevels.Add(level);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = level.LevelId }, ToDto(level));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CharacterLevelDto>> Update(int id, [FromBody] CharacterLevelUpsertDto dto)
    {
        var level = await _context.CharacterLevels.FirstOrDefaultAsync(item => item.LevelId == id);
        if (level == null)
        {
            return NotFound();
        }

        // Prevent updating to a Level value that already exists on another record
        if (await _context.CharacterLevels.AnyAsync(cl => cl.Level == dto.Level && cl.LevelId != id))
        {
            return BadRequest($"Level {dto.Level} already exists.");
        }

        level.Level = dto.Level;
        level.CurrentExperiencePoints = dto.CurrentExperiencePoints;
        level.ExperiencePointsToNextLevel = dto.ExperiencePointsToNextLevel;
        level.ProficiencyBonus = dto.ProficiencyBonus;
        level.DateOfLastLevelUp = dto.DateOfLastLevelUp;

        await _context.SaveChangesAsync();

        return Ok(ToDto(level));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var level = await _context.CharacterLevels.FirstOrDefaultAsync(item => item.LevelId == id);
        if (level == null)
        {
            return NotFound();
        }

        var attachedCharacters = await _context.Characters
            .Where(item => item.LevelId == id)
            .ToListAsync();

        foreach (var character in attachedCharacters)
        {
            character.LevelId = null;
        }

        _context.CharacterLevels.Remove(level);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static bool LevelMatchesSearch(CharacterLevel level, string searchTerm)
    {
        return level.LevelId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || level.Level.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || level.CurrentExperiencePoints.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || level.ExperiencePointsToNextLevel.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || level.ProficiencyBonus.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ;
    }

    private static CharacterLevelDto ToDto(CharacterLevel level)
    {
        return new CharacterLevelDto
        {
            LevelId = level.LevelId,
            Level = level.Level,
            CurrentExperiencePoints = level.CurrentExperiencePoints,
            ExperiencePointsToNextLevel = level.ExperiencePointsToNextLevel,
            ProficiencyBonus = level.ProficiencyBonus,
            DateOfLastLevelUp = level.DateOfLastLevelUp
        };
    }
}