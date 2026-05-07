# Semantic Model

## Database tables and mapped entities

| Entity / table | Main properties | Connections |
| --- | --- | --- |
| `Player` / `Players` | `PlayerId`, `Name`, `Surname`, `Username`, `Email`, `Password`, `LastLogin` | One player has many `Character` records through `CharacterList` |
| `Character` / `Characters` | `CharacterId`, `PlayerId`, `LevelId`, `CharacterName`, `Race`, `Background`, `Alignment`, `Class` | Belongs to one `Player`; optionally belongs to one `CharacterLevel`; has many `Equipment` items |
| `CharacterLevel` / `CharacterLevels` | `LevelId`, `Level`, `CurrentExperiencePoints`, `ExperiencePointsToNextLevel`, `ProficiencyBonus`, `DateOfLastLevelUp` | Used by one `Character` at most; `Character.LevelId` is nullable and unique when present |
| `Equipment` / `Equipment` | `EquipmentId`, `CharacterId`, `Type`, `Name`, `Cost`, `Weight` | Belongs to one `Character`; base type for equipment variants |

## Supporting domain classes

| Class | Purpose | Main properties |
| --- | --- | --- |
| `Weapon` | Equipment subtype for weapons | `DamageAmount`, `DamageType`, `WeaponProperties` |
| `Armour` | Equipment subtype for armor | `ArmourClass`, `StrenghtRequierment`, `StealthDisadvantage` |
| `Tools` | Equipment subtype for tools | No extra properties beyond `Equipment` |
| `AdventuringGear` | Equipment subtype for general gear | No extra properties beyond `Equipment` |

## Enums used by characters and weapons

| Enum | Values |
| --- | --- |
| `RaceEnum` | `Dragonborn`, `Dwarf`, `Elf`, `Gnome`, `Half_Elf`, `Half_Orc`, `Halfling`, `Human`, `Tiefling` |
| `BackgroundEnum` | `Acolyte`, `Charlatan`, `Criminal`, `Entertainer`, `Folk_Hero`, `Guild_Artisan`, `Hermit`, `Noble`, `Outlander`, `Sage`, `Sailor`, `Soldier`, `Urchin` |
| `AlignmentEnum` | `LawfulGood`, `NeutralGood`, `ChaoticGood`, `LawfulNeutral`, `Neutral`, `ChaoticNeutral`, `LawfulEvil`, `NeutralEvil`, `ChaoticEvil` |
| `ClassEnum` | `Barbarian`, `Bard`, `Cleric`, `Druid`, `Fighter`, `Monk`, `Paladin`, `Ranger`, `Rogue`, `Sorcerer`, `Warlock`, `Wizard` |
| `WeaponPropertiesEnum` | `Ammunition`, `Finesse`, `Heavy`, `Light`, `Loading`, `Range`, `Reach`, `Silvered`, `Special`, `Thrown`, `TwoHanded`, `Versatile` |

## Relationship summary

- `Player` 1 -> many `Character`
- `Character` 0..1 -> 1 `CharacterLevel`
- `Character` 1 -> many `Equipment`
- `Character` stores four enum-based fields: race, background, alignment, and class
- `Weapon`, `Armour`, `Tools`, and `AdventuringGear` are domain-level equipment variants built on top of `Equipment`

## Notes

- The current EF model creates tables for `Players`, `Characters`, `CharacterLevels`, and `Equipment`.
- The derived equipment classes exist in the domain model, but the current `DbContext` only exposes `DbSet<Equipment>` and the initial migration only creates the base `Equipment` table.
- `CharacterWithPlayerViewModel` is a web-layer view model, not a database entity.