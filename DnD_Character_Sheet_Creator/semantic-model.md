# DnD Character Sheet Creator - Semantic Model

## Domain Model

### Player
Represents a player account and the owner of one or more characters.

| Property | Type | Notes |
|----------|------|-------|
| PlayerId | int | Primary key |
| Name | string | Required |
| Surname | string | Required |
| Username | string | Required, unique in practice |
| Email | string | Required |
| Password | string | Stored by the app and mirrored in API payloads |
| LastLogin | DateTime | Updated on sign-in |
| Role | RoleEnum | Admin, Manager, User |
| DeletedAt | DateTime? | Soft-delete marker |

**Relationships:**
- One Player -> Many Characters
- One Player -> One AppUser through `PlayerId`

### AppUser (Identity)
ASP.NET Core Identity user for authentication.

| Property | Type | Notes |
|----------|------|-------|
| Id | string | Identity primary key |
| UserName | string | Login name |
| Email | string | Login email |
| PlayerId | int? | Links Identity to Player |
| OIB | string | National identifier |
| JMBG | string | Birth number |

### Character
Represents a D&D character sheet owned by a player.

| Property | Type | Notes |
|----------|------|-------|
| CharacterId | int | Primary key |
| PlayerId | int | Foreign key to Player |
| LevelId | int? | Foreign key to CharacterLevel |
| CharacterName | string | Required |
| Race | RaceEnum | Character race |
| Background | BackgroundEnum | Character background |
| Alignment | AlignmentEnum | Character alignment |
| Class | ClassEnum | Character class |
| CurrentExperiencePoints | int | Current XP total |
| DateOfLastLevelUp | DateTime? | Last level-up timestamp |
| Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma | int | Ability scores |
| Skill/save booleans | bool | Proficiency flags for saves and skills |
| DeletedAt | DateTime? | Soft-delete marker |

**Relationships:**
- Many Characters -> One Player
- Many Characters -> One CharacterLevel
- One Character -> Many Equipment entries
- One Character -> Many Attachments

### CharacterLevel
Represents the level progression snapshot linked to a character.

| Property | Type | Notes |
|----------|------|-------|
| LevelId | int | Primary key |
| Level | int | Character level number |
| CurrentExperiencePoints | int | Current XP |
| ExperiencePointsToNextLevel | int | XP threshold |
| ProficiencyBonus | int | Proficiency bonus |
| DateOfLastLevelUp | DateTime | Timestamp |

### Equipment
Represents an item that can be cataloged globally and linked to characters.

| Property | Type | Notes |
|----------|------|-------|
| EquipmentId | int | Primary key |
| CharacterId | int? | UI/model helper for character-linked workflows |
| Type | string? | Equipment category label |
| Name | string | Required |
| Cost | int | Gold pieces |
| Weight | int | Weight value |
| DeletedAt | DateTime? | Soft-delete marker |

**Relationships:**
- Many Characters <-> Many Equipment entries through the character equipment list
- Equipment rows can be managed globally on `/Equipment`

### Attachment
Represents a file uploaded for a character.

| Property | Type | Notes |
|----------|------|-------|
| AttachmentId | int | Primary key |
| CharacterId | int | Foreign key to Character |
| FileName | string | Original file name |
| FilePath | string | Stored path under `wwwroot/uploads/characters/{id}/` |
| ContentType | string? | MIME type |
| FileSize | long | Size in bytes |
| CreatedAt | DateTime | Upload timestamp |
| DeletedAt | DateTime? | Soft-delete marker |

## Supporting Enums

| Enum | Values |
|------|--------|
| RoleEnum | Admin, Manager, User |
| ClassEnum | Barbarian, Bard, Cleric, Druid, Fighter, Monk, Paladin, Ranger, Rogue, Sorcerer, Warlock, Wizard |
| RaceEnum | Human, Elf, Dwarf, Halfling, Tiefling, Dragonborn, Gnome |
| BackgroundEnum | Acolyte, Charlatan, Criminal, Entertainer, Folk Hero, Guild Artisan, Hermit, Noble, Outlander, Sage, Sailor, Soldier, Urchin |
| AlignmentEnum | LawfulGood, NeutralGood, ChaoticGood, LawfulNeutral, TrueNeutral, ChaoticNeutral, LawfulEvil, NeutralEvil, ChaoticEvil |
| WeaponPropertiesEnum | Ammunition, Finesse, Heavy, Light, Loading, Range, Reach, Special, Thrown, TwoHanded, Versatile |

## MVC Surface

### Account and Identity
- Sign in and register use ASP.NET Core Identity with `AccountController`.
- External login is supported through Google.
- `Profile` shows the current player and their active characters.

### Players and Characters
- `PlayersController` serves player browse, search, autocomplete, create, edit, details, and remove flows.
- `CharactersController` serves character browse, search, autocomplete, create, edit, details, remove, attachment upload/delete, and equipment linking flows.
- Anonymous users can browse lists, but character details and edit flows are role- and ownership-aware.

### Equipment and Codex
- `EquipmentController` manages the global equipment catalog for Admin and Manager users.
- `CodexController` provides a combined search page over players, characters, and equipment.

## Authorization Rules

| Resource | Anonymous | Authenticated | Manager | Admin |
|----------|-----------|---------------|---------|-------|
| Players list/search | Yes | Yes | Yes | Yes |
| Player details | No | Yes | Yes | Yes |
| Player create/edit | No | No | Yes | Yes |
| Player delete | No | No | No | Yes |
| Characters list/search | Yes | Yes | Yes | Yes |
| Character details | No | Yes | Yes | Yes |
| Character create/edit/delete | No | Yes | Ownership-aware | Ownership-aware |
| Attachment upload/delete | No | Yes | Ownership-aware | Ownership-aware |
| Global equipment catalog | No | No | Yes | Yes |
| Codex search | Yes | Yes | Yes | Yes |

## API DTOs

The Web API uses DTOs instead of exposing entities directly:

- PlayerUpsertDto
- PlayerSummaryDto
- PlayerDto
- CharacterUpsertDto
- CharacterSummaryDto
- CharacterDto
- CharacterLevelUpsertDto
- CharacterLevelDto
- EquipmentUpsertDto
- EquipmentSummaryDto
- EquipmentDto

## Soft Delete

Soft delete is used for Player, Character, Equipment, and Attachment rows:

- Records stay in the database.
- `DeletedAt` is set instead of physically removing the row.
- Queries and UI lists filter out deleted records.
- Character deletion also marks linked equipment rows as deleted in the API layer.
