# DnD Character Sheet Creator - Semantic Model

## Database Schema & Domain Model

### Core Entities

#### Player
Represents a user/player account in the system.

| Property | Type | Notes |
|----------|------|-------|
| PlayerId | int | Primary Key |
| PlayerName | string | Required |
| PlayerEmail | string | Required, Unique |
| OIB | string | Croatian ID number |
| DeletedAt | DateTime? | Soft-delete marker |

**Relationships:**
- One Player → Many Characters
- One Player → One AppUser (Identity)

---

#### AppUser (Identity)
ASP.NET Core Identity user mapping for authentication.

| Property | Type | Notes |
|----------|------|-------|
| Id | string | Primary Key (Identity) |
| UserName | string | Required |
| Email | string | Required, Unique |
| PlayerId | int? | Foreign Key to Player |
| OIB | string | Croatian ID |
| JMBG | string | Birth number |

**Roles:** 
- Admin (full access)
- Manager (can manage players/characters)

---

#### Character
Represents a D&D character created by a player.

| Property | Type | Notes |
|----------|------|-------|
| CharacterId | int | Primary Key |
| CharacterName | string | Required |
| PlayerId | int | Foreign Key |
| Class | ClassEnum | Barbarian, Bard, Cleric, etc. |
| Race | RaceEnum | Human, Elf, Dwarf, etc. |
| Background | BackgroundEnum | Acolyte, Criminal, etc. |
| Alignment | AlignmentEnum | LawfulGood, ChaoticEvil, etc. |
| LevelId | int? | Foreign Key to CharacterLevel |
| DeletedAt | DateTime? | Soft-delete marker |

**Relationships:**
- Many Characters → One Player
- Many Characters → One CharacterLevel
- One Character → Many Equipment
- One Character → Many Attachments

---

#### CharacterLevel
Represents experience and level progression for a character.

| Property | Type | Notes |
|----------|------|-------|
| LevelId | int | Primary Key |
| LevelNumber | int | 1-20 |
| Experience | int | XP total |
| HitPoints | int | Max HP |

**Relationships:**
- Many CharacterLevels ← Many Characters

---

#### Equipment
Represents items carried by a character (weapons, armor, gear).

| Property | Type | Notes |
|----------|------|-------|
| EquipmentId | int | Primary Key |
| CharacterId | int | Foreign Key |
| Equipment_Type | EquipmentType | Weapon, Armour, AdventuringGear, Tools |
| Equipment_Name | string | Item name |
| Cost | decimal | Gold pieces |
| Weight | decimal | Pounds |
| DeletedAt | DateTime? | Soft-delete marker |

**Derived/Domain Classes:**
- Weapon (extends Equipment with DamageAmount, DamageType, WeaponPropertiesEnum)
- Armour (extends Equipment with armor class info)
- Tools, AdventuringGear (specialized equipment types)

---

#### Attachment
File attachments uploaded to a character (images, documents, notes).

| Property | Type | Notes |
|----------|------|-------|
| AttachmentId | int | Primary Key |
| CharacterId | int | Foreign Key |
| FileName | string | Original filename |
| FilePath | string | Server path (wwwroot/uploads/characters/{id}/) |
| DateUploaded | DateTime | Upload timestamp |
| DeletedAt | DateTime? | Soft-delete marker |

**Relationships:**
- Many Attachments ← One Character

---

### Supporting Enums

| Enum | Values |
|------|--------|
| ClassEnum | Barbarian, Bard, Cleric, Druid, Fighter, Monk, Paladin, Ranger, Rogue, Sorcerer, Warlock, Wizard |
| RaceEnum | Human, Elf, Dwarf, Halfling, Tiefling, Dragonborn, Gnome |
| BackgroundEnum | Acolyte, Charlatan, Criminal, Entertainer, Folk Hero, Guild Artisan, Hermit, Noble, Outlander, Sage, Sailor, Soldier, Urchin |
| AlignmentEnum | LawfulGood, NeutralGood, ChaoticGood, LawfulNeutral, TrueNeutral, ChaoticNeutral, LawfulEvil, NeutralEvil, ChaoticEvil |
| WeaponPropertiesEnum | Ammunition, Finesse, Heavy, Light, Loading, Range, Reach, Special, Thrown, TwoHanded, Versatile |

---

## Authentication & Authorization

### Authentication Flow
1. User registers via /Account/Register with username/email/password
2. AppUser created in Identity, linked to Player record
3. User can sign in with credentials or via Google OAuth
4. ASP.NET Core Identity handles session/cookies
5. Claims-based authorization enforces role access

### Authorization Rules

| Resource | Anon | Auth | Manager | Admin |
|----------|------|------|---------|-------|
| Browse Players | ✓ | ✓ | ✓ | ✓ |
| View Own Player | ✓ | ✓ | ✓ | ✓ |
| Create/Edit/Delete Player | ✗ | ✗ | ✓ | ✓ |
| Browse Characters | ✓ | ✓ | ✓ | ✓ |
| View Character Details | ✗ | ✓ | ✓ | ✓ |
| Create/Edit/Delete Character | ✗ | ✗ | ✓ | ✓ |
| Manage Attachments | ✗ | ✗ | ✓ | ✓ |

---

## API DTOs

The Web API returns Data Transfer Objects (DTOs) to clients rather than raw entities:

- **PlayerUpsertDto** - for POST/PUT player operations
- **PlayerDto** - full player details with characters
- **PlayerSummaryDto** - lightweight player info
- **CharacterUpsertDto** - for POST/PUT character operations
- **CharacterDto** - full character with level, equipment, attachments
- **CharacterSummaryDto** - lightweight character info
- **EquipmentUpsertDto** - for POST/PUT equipment operations
- **EquipmentDto** - full equipment details
- **CharacterLevelDto** - level progression info

---

## Soft-Delete Pattern

Entities with DeletedAt field (Player, Character, Equipment, Attachment) are soft-deleted:
- Record retained in database
- DeletedAt timestamp set to current time
- Queries exclude deleted records by convention
- EF Core configurations filter automatically
