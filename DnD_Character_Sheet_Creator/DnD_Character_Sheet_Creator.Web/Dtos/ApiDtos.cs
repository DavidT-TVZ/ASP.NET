using System.ComponentModel.DataAnnotations;
using DnD_Character_Sheet_Creator.Models;

namespace DnD_Character_Sheet_Creator.Web.Dtos;

public class PlayerUpsertDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Password { get; set; } = string.Empty;

    public RoleEnum Role { get; set; } = RoleEnum.User;
}

public class CharacterUpsertDto
{
    [Range(1, int.MaxValue)]
    public int PlayerId { get; set; }

    [Required]
    [StringLength(150)]
    public string CharacterName { get; set; } = string.Empty;

    [Required]
    public RaceEnum? Race { get; set; }

    [Required]
    public BackgroundEnum? Background { get; set; }

    [Required]
    public AlignmentEnum? Alignment { get; set; }

    [Required]
    public ClassEnum? Class { get; set; }

    public int? LevelId { get; set; }

    [Range(0, int.MaxValue)]
    public int? CurrentExperiencePoints { get; set; }

    public DateTime? DateOfLastLevelUp { get; set; }

    [Range(1, 30)] public int? Strength { get; set; }
    [Range(1, 30)] public int? Dexterity { get; set; }
    [Range(1, 30)] public int? Constitution { get; set; }
    [Range(1, 30)] public int? Intelligence { get; set; }
    [Range(1, 30)] public int? Wisdom { get; set; }
    [Range(1, 30)] public int? Charisma { get; set; }

    public bool StrengthSaveProficient { get; set; }
    public bool DexteritySaveProficient { get; set; }
    public bool ConstitutionSaveProficient { get; set; }
    public bool IntelligenceSaveProficient { get; set; }
    public bool WisdomSaveProficient { get; set; }
    public bool CharismaSaveProficient { get; set; }

    public bool AcrobaticsProficient { get; set; }
    public bool AnimalHandlingProficient { get; set; }
    public bool ArcanaProficient { get; set; }
    public bool AthleticsProficient { get; set; }
    public bool DeceptionProficient { get; set; }
    public bool HistoryProficient { get; set; }
    public bool InsightProficient { get; set; }
    public bool IntimidationProficient { get; set; }
    public bool InvestigationProficient { get; set; }
    public bool MedicineProficient { get; set; }
    public bool NatureProficient { get; set; }
    public bool PerceptionProficient { get; set; }
    public bool PerformanceProficient { get; set; }
    public bool PersuasionProficient { get; set; }
    public bool ReligionProficient { get; set; }
    public bool SleightOfHandProficient { get; set; }
    public bool StealthProficient { get; set; }
    public bool SurvivalProficient { get; set; }
}

public class EquipmentUpsertDto
{
    [Range(1, int.MaxValue)]
    public int CharacterId { get; set; }

    [StringLength(100)]
    public string? Type { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Cost { get; set; }

    [Range(0, int.MaxValue)]
    public int Weight { get; set; }
}

public class CharacterLevelUpsertDto
{
    [Range(1, int.MaxValue)]
    public int Level { get; set; }

    [Range(0, int.MaxValue)]
    public int CurrentExperiencePoints { get; set; }

    [Range(0, int.MaxValue)]
    public int ExperiencePointsToNextLevel { get; set; }

    [Range(0, int.MaxValue)]
    public int ProficiencyBonus { get; set; }

    public DateTime DateOfLastLevelUp { get; set; }
}

public class PlayerSummaryDto
{
    public int PlayerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public RoleEnum Role { get; set; } = RoleEnum.User;
}

public class CharacterLevelDto
{
    public int LevelId { get; set; }

    public int Level { get; set; }

    public int CurrentExperiencePoints { get; set; }

    public int ExperiencePointsToNextLevel { get; set; }

    public int ProficiencyBonus { get; set; }

    public DateTime DateOfLastLevelUp { get; set; }
}

public class CharacterSummaryDto
{
    public int CharacterId { get; set; }

    public int PlayerId { get; set; }

    public string CharacterName { get; set; } = string.Empty;

    public RaceEnum Race { get; set; }

    public BackgroundEnum Background { get; set; }

    public AlignmentEnum Alignment { get; set; }

    public ClassEnum Class { get; set; }

    public int CurrentExperiencePoints { get; set; }

    public DateTime? DateOfLastLevelUp { get; set; }

    public int Strength { get; set; }

    public int Dexterity { get; set; }

    public int Constitution { get; set; }

    public int Intelligence { get; set; }

    public int Wisdom { get; set; }

    public int Charisma { get; set; }

    public bool StrengthSaveProficient { get; set; }

    public bool DexteritySaveProficient { get; set; }

    public bool ConstitutionSaveProficient { get; set; }

    public bool IntelligenceSaveProficient { get; set; }

    public bool WisdomSaveProficient { get; set; }

    public bool CharismaSaveProficient { get; set; }

    public bool AcrobaticsProficient { get; set; }

    public bool AnimalHandlingProficient { get; set; }

    public bool ArcanaProficient { get; set; }

    public bool AthleticsProficient { get; set; }

    public bool DeceptionProficient { get; set; }

    public bool HistoryProficient { get; set; }

    public bool InsightProficient { get; set; }

    public bool IntimidationProficient { get; set; }

    public bool InvestigationProficient { get; set; }

    public bool MedicineProficient { get; set; }

    public bool NatureProficient { get; set; }

    public bool PerceptionProficient { get; set; }

    public bool PerformanceProficient { get; set; }

    public bool PersuasionProficient { get; set; }

    public bool ReligionProficient { get; set; }

    public bool SleightOfHandProficient { get; set; }

    public bool StealthProficient { get; set; }

    public bool SurvivalProficient { get; set; }

    public CharacterLevelDto? Level { get; set; }
}

public class EquipmentSummaryDto
{
    public int EquipmentId { get; set; }

    public int CharacterId { get; set; }

    public string? Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Cost { get; set; }

    public int Weight { get; set; }
}

public class PlayerDto : PlayerSummaryDto
{
    public List<CharacterSummaryDto> Characters { get; set; } = new();
}

public class CharacterDto : CharacterSummaryDto
{
    public PlayerSummaryDto? Player { get; set; }

    public List<EquipmentSummaryDto> Equipment { get; set; } = new();
}

public class EquipmentDto : EquipmentSummaryDto
{
    public CharacterSummaryDto? Character { get; set; }
}