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
    public async Task<ActionResult<IEnumerable<EquipmentDto>>> GetAll([FromQuery(Name = "search")] string? search, [FromQuery] int? characterId)
    {
        var equipment = await _context.Equipment
            .Where(item => item.DeletedAt == null)
            .Include(item => item.Characters)
                .ThenInclude(character => character.Level)
            .AsNoTracking()
            .ToListAsync();

        if (characterId.HasValue)
        {
            equipment = equipment.Where(item => item.Characters.Any(character => character.CharacterId == characterId.Value)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedQuery = search.Trim();
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
            .Include(item => item.Characters)
                .ThenInclude(character => character.Level)
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
            Type = dto.Type?.Trim(),
            Name = dto.Name.Trim(),
            Cost = dto.Cost,
            Weight = dto.Weight
        };

        _context.Equipment.Add(equipment);
        character.EquipmentList.Add(equipment);
        await _context.SaveChangesAsync();

        await _context.Entry(equipment).Collection(item => item.Characters).LoadAsync();
        if (equipment.Characters.Count > 0)
        {
            await _context.Entry(equipment.Characters.First()).Reference(item => item.Level).LoadAsync();
        }

        return CreatedAtAction(nameof(GetById), new { id = equipment.EquipmentId }, ToDto(equipment));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EquipmentDto>> Update(int id, [FromBody] EquipmentUpsertDto dto)
    {
        var equipment = await _context.Equipment
            .Include(item => item.Characters)
                .ThenInclude(character => character.Level)
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

        if (!equipment.Characters.Any(item => item.CharacterId == character.CharacterId))
        {
            equipment.Characters.Add(character);
        }

        await _context.SaveChangesAsync();

        await _context.Entry(equipment).Collection(item => item.Characters).LoadAsync();
        if (equipment.Characters.Count > 0)
        {
            await _context.Entry(equipment.Characters.First()).Reference(item => item.Level).LoadAsync();
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
            || equipment.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || (equipment.Type?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            || equipment.Cost.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || equipment.Weight.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            || equipment.Characters.Any(character => character.CharacterName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    private static EquipmentDto ToDto(Equipment equipment)
    {
        var linkedCharacter = equipment.Characters.FirstOrDefault();

        return new EquipmentDto
        {
            EquipmentId = equipment.EquipmentId,
            CharacterId = linkedCharacter?.CharacterId ?? equipment.CharacterId ?? 0,
            Type = equipment.Type,
            Name = equipment.Name,
            Cost = equipment.Cost,
            Weight = equipment.Weight,
            Character = linkedCharacter == null ? null : new CharacterSummaryDto
            {
                CharacterId = linkedCharacter.CharacterId,
                PlayerId = linkedCharacter.PlayerId,
                CharacterName = linkedCharacter.CharacterName,
                Race = linkedCharacter.Race,
                Background = linkedCharacter.Background,
                Alignment = linkedCharacter.Alignment,
                Class = linkedCharacter.Class,
                CurrentExperiencePoints = linkedCharacter.CurrentExperiencePoints,
                DateOfLastLevelUp = linkedCharacter.DateOfLastLevelUp,
                Strength = linkedCharacter.Strength,
                Dexterity = linkedCharacter.Dexterity,
                Constitution = linkedCharacter.Constitution,
                Intelligence = linkedCharacter.Intelligence,
                Wisdom = linkedCharacter.Wisdom,
                Charisma = linkedCharacter.Charisma,
                StrengthSaveProficient = linkedCharacter.StrengthSaveProficient,
                DexteritySaveProficient = linkedCharacter.DexteritySaveProficient,
                ConstitutionSaveProficient = linkedCharacter.ConstitutionSaveProficient,
                IntelligenceSaveProficient = linkedCharacter.IntelligenceSaveProficient,
                WisdomSaveProficient = linkedCharacter.WisdomSaveProficient,
                CharismaSaveProficient = linkedCharacter.CharismaSaveProficient,
                AcrobaticsProficient = linkedCharacter.AcrobaticsProficient,
                AnimalHandlingProficient = linkedCharacter.AnimalHandlingProficient,
                ArcanaProficient = linkedCharacter.ArcanaProficient,
                AthleticsProficient = linkedCharacter.AthleticsProficient,
                DeceptionProficient = linkedCharacter.DeceptionProficient,
                HistoryProficient = linkedCharacter.HistoryProficient,
                InsightProficient = linkedCharacter.InsightProficient,
                IntimidationProficient = linkedCharacter.IntimidationProficient,
                InvestigationProficient = linkedCharacter.InvestigationProficient,
                MedicineProficient = linkedCharacter.MedicineProficient,
                NatureProficient = linkedCharacter.NatureProficient,
                PerceptionProficient = linkedCharacter.PerceptionProficient,
                PerformanceProficient = linkedCharacter.PerformanceProficient,
                PersuasionProficient = linkedCharacter.PersuasionProficient,
                ReligionProficient = linkedCharacter.ReligionProficient,
                SleightOfHandProficient = linkedCharacter.SleightOfHandProficient,
                StealthProficient = linkedCharacter.StealthProficient,
                SurvivalProficient = linkedCharacter.SurvivalProficient,
                Level = linkedCharacter.Level == null ? null : new CharacterLevelDto
                {
                    LevelId = linkedCharacter.Level.LevelId,
                    Level = linkedCharacter.Level.Level,
                    CurrentExperiencePoints = linkedCharacter.Level.CurrentExperiencePoints,
                    ExperiencePointsToNextLevel = linkedCharacter.Level.ExperiencePointsToNextLevel,
                    ProficiencyBonus = linkedCharacter.Level.ProficiencyBonus,
                    DateOfLastLevelUp = linkedCharacter.Level.DateOfLastLevelUp
                }
            }
        };
    }
}