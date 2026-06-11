using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Web.Controllers;

[ApiController]
[Route("api/players")]
public class PlayersApiController : ControllerBase
{
    private readonly DnDDbContext _context;

    public PlayersApiController(DnDDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll([FromQuery] string? query)
    {
        var players = await _context.Players
            .Where(player => player.DeletedAt == null)
            .Include(player => player.CharacterList)
                .ThenInclude(character => character.Level)
            .AsNoTracking()
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim();
            players = players.Where(player => PlayerMatchesSearch(player, normalizedQuery)).ToList();
        }

        return Ok(players
            .OrderBy(player => player.Name)
            .ThenBy(player => player.Surname)
            .ThenBy(player => player.PlayerId)
            .Select(ToDto)
            .ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlayerDto>> GetById(int id)
    {
        var player = await _context.Players
            .Where(item => item.PlayerId == id && item.DeletedAt == null)
            .Include(item => item.CharacterList)
                .ThenInclude(character => character.Level)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (player == null)
        {
            return NotFound();
        }

        return Ok(ToDto(player));
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Create([FromBody] PlayerUpsertDto dto)
    {
        var player = new Player
        {
            Name = dto.Name.Trim(),
            Surname = dto.Surname.Trim(),
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim(),
            Password = dto.Password,
            LastLogin = DateTime.UtcNow,
            Role = dto.Role
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = player.PlayerId }, ToDto(player));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PlayerDto>> Update(int id, [FromBody] PlayerUpsertDto dto)
    {
        var player = await _context.Players
            .Include(item => item.CharacterList)
                .ThenInclude(character => character.Level)
            .FirstOrDefaultAsync(item => item.PlayerId == id && item.DeletedAt == null);

        if (player == null)
        {
            return NotFound();
        }

        player.Name = dto.Name.Trim();
        player.Surname = dto.Surname.Trim();
        player.Username = dto.Username.Trim();
        player.Email = dto.Email.Trim();
        player.Password = dto.Password;
        player.Role = dto.Role;

        await _context.SaveChangesAsync();

        return Ok(ToDto(player));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var player = await _context.Players.FirstOrDefaultAsync(item => item.PlayerId == id && item.DeletedAt == null);
        if (player == null)
        {
            return NotFound();
        }

        player.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static bool PlayerMatchesSearch(Player player, string searchTerm)
    {
        return player.PlayerId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || player.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || player.Surname.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || player.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || player.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
    }

    private static PlayerDto ToDto(Player player)
    {
        return new PlayerDto
        {
            PlayerId = player.PlayerId,
            Name = player.Name,
            Surname = player.Surname,
            Username = player.Username,
            Email = player.Email,
            Role = player.Role,
            Characters = player.CharacterList
                .Where(character => character.DeletedAt == null)
                .OrderBy(character => character.CharacterName)
                .ThenBy(character => character.CharacterId)
                .Select(ToSummaryDto)
                .ToList()
        };
    }

    private static CharacterSummaryDto ToSummaryDto(Character character)
    {
        return new CharacterSummaryDto
        {
            CharacterId = character.CharacterId,
            PlayerId = character.PlayerId,
            CharacterName = character.CharacterName,
            Race = character.Race,
            Background = character.Background,
            Alignment = character.Alignment,
            Class = character.Class,
            Level = character.Level == null ? null : new CharacterLevelDto
            {
                LevelId = character.Level.LevelId,
                Level = character.Level.Level,
                CurrentExperiencePoints = character.Level.CurrentExperiencePoints,
                ExperiencePointsToNextLevel = character.Level.ExperiencePointsToNextLevel,
                ProficiencyBonus = character.Level.ProficiencyBonus,
                DateOfLastLevelUp = character.Level.DateOfLastLevelUp
            }
        };
    }
}