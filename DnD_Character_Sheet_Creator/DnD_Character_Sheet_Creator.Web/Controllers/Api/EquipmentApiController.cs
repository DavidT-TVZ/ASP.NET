using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Web.Controllers;

[ApiController]
[Route("api/equipment")]
public class EquipmentApiController : ControllerBase
{
    private readonly DnDDbContext _context;

    public EquipmentApiController(DnDDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipmentDto>>> GetAll([FromQuery] string? query, [FromQuery] int? characterId)
    {
        var equipment = await _context.Equipment
            .Where(item => item.DeletedAt == null)
            .Include(item => item.Character)
                .ThenInclude(character => character!.Level)
            .AsNoTracking()
            .ToListAsync();

        if (characterId.HasValue)
        {
            equipment = equipment.Where(item => item.CharacterId == characterId.Value).ToList();
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim();
            equipment = equipment.Where(item => EquipmentMatchesSearch(item, normalizedQuery)).ToList();
        }

        return Ok(equipment
            .OrderBy(item => item.Name)
            .ThenBy(item => item.EquipmentId)
            .Select(ToDto)
            .ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EquipmentDto>> GetById(int id)
    {
        var equipment = await _context.Equipment
            .Where(item => item.EquipmentId == id && item.DeletedAt == null)
            .Include(item => item.Character)
                .ThenInclude(character => character!.Level)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (equipment == null)
        {
            return NotFound();
        }

        return Ok(ToDto(equipment));
    }

    [HttpPost]
    public async Task<ActionResult<EquipmentDto>> Create([FromBody] EquipmentUpsertDto dto)
    {
        var character = await _context.Characters.FirstOrDefaultAsync(item => item.CharacterId == dto.CharacterId && item.DeletedAt == null);
        if (character == null)
        {
            return NotFound($"Character {dto.CharacterId} was not found.");
        }

        var equipment = new Equipment
        {
            CharacterId = dto.CharacterId,
            Type = dto.Type?.Trim(),
            Name = dto.Name.Trim(),
            Cost = dto.Cost,
            Weight = dto.Weight
        };

        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();

        await _context.Entry(equipment).Reference(item => item.Character).LoadAsync();
        if (equipment.Character != null)
        {
            await _context.Entry(equipment.Character).Reference(item => item.Level).LoadAsync();
        }

        return CreatedAtAction(nameof(GetById), new { id = equipment.EquipmentId }, ToDto(equipment));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EquipmentDto>> Update(int id, [FromBody] EquipmentUpsertDto dto)
    {
        var equipment = await _context.Equipment
            .Include(item => item.Character)
                .ThenInclude(character => character!.Level)
            .FirstOrDefaultAsync(item => item.EquipmentId == id && item.DeletedAt == null);

        if (equipment == null)
        {
            return NotFound();
        }

        var character = await _context.Characters.FirstOrDefaultAsync(item => item.CharacterId == dto.CharacterId && item.DeletedAt == null);
        if (character == null)
        {
            return NotFound($"Character {dto.CharacterId} was not found.");
        }

        equipment.CharacterId = dto.CharacterId;
        equipment.Type = dto.Type?.Trim();
        equipment.Name = dto.Name.Trim();
        equipment.Cost = dto.Cost;
        equipment.Weight = dto.Weight;

        await _context.SaveChangesAsync();

        await _context.Entry(equipment).Reference(item => item.Character).LoadAsync();
        if (equipment.Character != null)
        {
            await _context.Entry(equipment.Character).Reference(item => item.Level).LoadAsync();
        }

        return Ok(ToDto(equipment));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var equipment = await _context.Equipment.FirstOrDefaultAsync(item => item.EquipmentId == id && item.DeletedAt == null);
        if (equipment == null)
        {
            return NotFound();
        }

        equipment.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static bool EquipmentMatchesSearch(Equipment equipment, string searchTerm)
    {
        return equipment.EquipmentId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || equipment.CharacterId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || equipment.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || (equipment.Type?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            || equipment.Cost.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || equipment.Weight.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || (equipment.Character?.CharacterName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static EquipmentDto ToDto(Equipment equipment)
    {
        return new EquipmentDto
        {
            EquipmentId = equipment.EquipmentId,
            CharacterId = equipment.CharacterId,
            Type = equipment.Type,
            Name = equipment.Name,
            Cost = equipment.Cost,
            Weight = equipment.Weight,
            Character = equipment.Character == null ? null : new CharacterSummaryDto
            {
                CharacterId = equipment.Character.CharacterId,
                PlayerId = equipment.Character.PlayerId,
                CharacterName = equipment.Character.CharacterName,
                Race = equipment.Character.Race,
                Background = equipment.Character.Background,
                Alignment = equipment.Character.Alignment,
                Class = equipment.Character.Class,
                Level = equipment.Character.Level == null ? null : new CharacterLevelDto
                {
                    LevelId = equipment.Character.Level.LevelId,
                    Level = equipment.Character.Level.Level,
                    CurrentExperiencePoints = equipment.Character.Level.CurrentExperiencePoints,
                    ExperiencePointsToNextLevel = equipment.Character.Level.ExperiencePointsToNextLevel,
                    ProficiencyBonus = equipment.Character.Level.ProficiencyBonus,
                    DateOfLastLevelUp = equipment.Character.Level.DateOfLastLevelUp
                }
            }
        };
    }
}