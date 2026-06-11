# DnD Character Sheet Creator - Route Map & Sitemap

## Overview
This document maps all routes (MVC and REST API) to their corresponding controllers, actions, and views.

---

## Account Routes (Authentication & Authorization)

### Sign In / Sign Out (ASP.NET Core Identity)

| Route | Method | Controller | Action | View | Notes |
|-------|--------|-----------|--------|------|-------|
| /Account/SignIn | GET | Account | SignIn | SignIn.cshtml | Display login form |
| /Account/SignIn | POST | Account | SignIn | - | Submit credentials |
| /Account/Register | GET | Account | Register | Register.cshtml | Display registration form |
| /Account/Register | POST | Account | Register | - | Create new AppUser + Player |
| /Account/ExternalLogin | POST | Account | ExternalLogin | - | Initiate OAuth (Google) |
| /Account/ExternalLoginCallback | GET | Account | ExternalLoginCallback | - | Handle OAuth redirect |
| /Account/SignOut | POST | Account | SignOut | - | Logout user |

**ViewModels:**
- AccountSignInViewModel - Username/Email, Password
- AccountRegisterViewModel - Username, Email, Password, ConfirmPassword

---

## Player Routes (MVC)

| Route | Method | Controller | Action | View | Auth | Notes |
|-------|--------|-----------|--------|------|------|-------|
| /Players | GET | Players | Index | Index.cshtml | Anon | List all players (paginated, searchable) |
| /Players/Details/{id} | GET | Players | Details | Details.cshtml | Auth | View specific player + characters |
| /Players/Create | GET | Players | Create | Create.cshtml | Admin/Mgr | Display form |
| /Players/Create | POST | Players | Create | - | Admin/Mgr | Save new player |
| /Players/Edit/{id} | GET | Players | Edit | Edit.cshtml | Admin/Mgr | Display edit form |
| /Players/Edit/{id} | PUT | Players | Edit | - | Admin/Mgr | Update player |
| /Players/Remove/{id} | DELETE | Players | Remove | - | Admin/Mgr | Soft-delete player |
| /Players/Search | GET | Players | Search | - | Anon | AJAX autocomplete |

**ViewModels:**
- PlayerFormViewModel - PlayerId, PlayerName, PlayerEmail, OIB

---

## Character Routes (MVC)

| Route | Method | Controller | Action | View | Auth | Notes |
|-------|--------|-----------|--------|------|------|-------|
| /Characters | GET | Characters | Index | Index.cshtml | Anon | List all characters |
| /Characters/Details/{id} | GET | Characters | Details | Details.cshtml | Auth | View character + equipment + attachments |
| /Characters/Create | GET | Characters | Create | Create.cshtml | Auth | Display form |
| /Characters/Create | POST | Characters | Create | - | Auth | Save new character |
| /Characters/Edit/{id} | GET | Characters | Edit | Edit.cshtml | Auth | Display edit form |
| /Characters/Edit/{id} | PUT | Characters | Edit | - | Auth | Update character |
| /Characters/Remove/{id} | DELETE | Characters | Remove | - | Auth | Soft-delete character |

**Equipment Management (nested under Characters):**

| Route | Method | Controller | Action | View | Auth | Notes |
|-------|--------|-----------|--------|------|------|-------|
| /Characters/{characterId}/Equipment/CreateForm | GET | Characters | EquipmentCreateForm | _EquipmentCreateForm.cshtml | Auth | Display equipment form |
| /Characters/{characterId}/Equipment/Create | POST | Characters | EquipmentCreate | - | Auth | Save new equipment |
| /Characters/{characterId}/Equipment/EditForm/{equipmentId} | GET | Characters | EquipmentEditForm | _EquipmentEditForm.cshtml | Auth | Display edit form |
| /Characters/{characterId}/Equipment/Edit/{equipmentId} | POST | Characters | EquipmentEdit | - | Auth | Update equipment |
| /Characters/{characterId}/Equipment/Remove/{equipmentId} | POST | Characters | EquipmentRemove | - | Auth | Delete equipment |

**File Attachments (Dropzone):**

| Route | Method | Controller | Action | View | Auth | Notes |
|-------|--------|-----------|--------|------|------|-------|
| /Characters/{characterId}/Attachments | GET | Characters | Attachments | _AttachmentList.cshtml | Auth | List attachments (AJAX) |
| /Characters/{characterId}/AttachmentUpload | POST | Characters | AttachmentUpload | - | Auth | Upload file via Dropzone |
| /Characters/{characterId}/AttachmentDelete/{attachmentId} | DELETE | Characters | AttachmentDelete | - | Auth | Remove attachment |

**ViewModels:**
- CharacterFormViewModel - CharacterName, PlayerId, ClassId, RaceId, BackgroundId, AlignmentId, LevelId
- CharacterWithPlayerViewModel - Character with nested Player info
- EquipmentFormViewModel - Equipment_Type, Equipment_Name, Cost, Weight

---

## REST API Routes

### Players API

| Route | Method | Status | Response | Notes |
|-------|--------|--------|----------|-------|
| /api/players | GET | 200 | List[PlayerSummaryDto] | Fetch all players (paginated) |
| /api/players/{id} | GET | 200/404 | PlayerDto | Fetch by ID |
| /api/players | POST | 201 | PlayerDto | Create new player |
| /api/players/{id} | PUT | 200/204 | PlayerDto | Update player |
| /api/players/{id} | DELETE | 200/204 | - | Soft-delete player |

**Route:** /api/players  
**Controller:** PlayersApiController  
**DTO Classes:** PlayerUpsertDto, PlayerDto, PlayerSummaryDto

---

### Characters API

| Route | Method | Status | Response | Notes |
|-------|--------|--------|----------|-------|
| /api/characters | GET | 200 | List[CharacterSummaryDto] | Fetch all characters |
| /api/characters/{id} | GET | 200/404 | CharacterDto | Fetch by ID with level/equipment |
| /api/characters | POST | 201 | CharacterDto | Create with auto-generated level |
| /api/characters/{id} | PUT | 200/204 | CharacterDto | Update character |
| /api/characters/{id} | DELETE | 200/204 | - | Soft-delete character |

**Route:** /api/characters  
**Controller:** CharactersApiController  
**DTO Classes:** CharacterUpsertDto, CharacterDto, CharacterSummaryDto

---

### Equipment API

| Route | Method | Status | Response | Notes |
|-------|--------|--------|----------|-------|
| /api/equipment | GET | 200 | List[EquipmentSummaryDto] | Fetch all equipment |
| /api/equipment/{id} | GET | 200/404 | EquipmentDto | Fetch by ID |
| /api/equipment | POST | 201 | EquipmentDto | Create new equipment |
| /api/equipment/{id} | PUT | 200/204 | EquipmentDto | Update equipment |
| /api/equipment/{id} | DELETE | 200/204 | - | Soft-delete equipment |

**Route:** /api/equipment  
**Controller:** EquipmentApiController  
**DTO Classes:** EquipmentUpsertDto, EquipmentDto, EquipmentSummaryDto

---

### Character Levels API

| Route | Method | Status | Response | Notes |
|-------|--------|--------|----------|-------|
| /api/character-levels | GET | 200 | List[CharacterLevelDto] | Fetch all levels |
| /api/character-levels/{id} | GET | 200/404 | CharacterLevelDto | Fetch by ID |
| /api/character-levels | POST | 201 | CharacterLevelDto | Create new level |
| /api/character-levels/{id} | PUT | 200/204 | CharacterLevelDto | Update level |
| /api/character-levels/{id} | DELETE | 200/204 | - | Unlink from characters |

**Route:** /api/character-levels  
**Controller:** CharacterLevelsApiController  
**DTO Class:** CharacterLevelDto

---

## Testing Routes

### Integration Tests
- **Project:** DnD_Character_Sheet_Creator.Tests
- **Test Framework:** xUnit
- **Fixture:** WebApplicationFactoryFixture (in-memory SQLite DB)
- **Test Files:**
  - PlayersApiControllerTests.cs - 8 tests (GET/POST/PUT/DELETE)
  - CharactersApiControllerTests.cs - 7 tests
  - EquipmentApiControllerTests.cs - 7 tests
  - CharacterLevelsApiControllerTests.cs - 7 tests

**Run Tests:**
``ash
cd DnD_Character_Sheet_Creator.Tests
dotnet test
``

---

## Home & Shared Routes

| Route | Method | Controller | Action | View | Notes |
|-------|--------|-----------|--------|------|-------|
| / | GET | Home | Index | Index.cshtml | Landing page |
| /Privacy | GET | Home | Privacy | Privacy.cshtml | Privacy policy |
| /Error | GET | Home | Error | Error.cshtml | Error handler |

---

## Custom DnD-Themed Routes (Aliases)

| Custom Route | Maps To | Purpose |
|--------------|---------|---------|
| /TheAdventurerLedger | /Home/Index | Thematic home |
| /Actors | /Players | Alternative player listing |
| /Adventurers | /Characters | Alternative character listing |

---

## ViewModels Summary

| ViewModel | Properties | Used In |
|-----------|-----------|---------|
| AccountSignInViewModel | Username, Password | Account/SignIn |
| AccountRegisterViewModel | Username, Email, Password, ConfirmPassword | Account/Register |
| PlayerFormViewModel | PlayerId, PlayerName, PlayerEmail, OIB | Players/Create, Players/Edit |
| CharacterFormViewModel | CharacterName, PlayerId, Class, Race, Background, Alignment, LevelId | Characters/Create, Characters/Edit |
| EquipmentFormViewModel | EquipmentType, EquipmentName, Cost, Weight | Equipment forms |
| CharacterWithPlayerViewModel | Character (with Player nested) | Characters/Details |

---

## API Authorization

All API endpoints use role-based authorization (configured in ApiController base or method attributes):

- **Public GET endpoints** (read-only): Accessible to authenticated users
- **POST/PUT/DELETE endpoints**: Require [Authorize(Roles = "Admin,Manager")]
- **Character attachments/equipment**: Require authentication

---

## Data Files & Uploads

**File Upload Location:**
- Physical path: wwwroot/uploads/characters/{characterId}/
- Metadata stored in Attachment entity
- Soft-delete supported

---

## Error Handling

- **400 Bad Request** - validation errors (ModelState invalid)
- **401 Unauthorized** - user not authenticated
- **403 Forbidden** - user lacks required role/permission
- **404 Not Found** - resource doesn't exist or is deleted
- **500 Internal Server Error** - unhandled exception

---

## Notes for Developers

1. **Soft Delete**: All list endpoints exclude deleted records (DeletedAt != null)
2. **Identity Integration**: Player record linked to AppUser via PlayerId
3. **File Uploads**: Stored server-side; metadata in database
4. **CORS**: Not enabled; internal app only
5. **API Versioning**: Not currently implemented; all APIs at v1.0 (implicit)
