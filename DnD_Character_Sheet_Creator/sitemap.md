# DnD Character Sheet Creator - Route Map & Sitemap

## Overview
This document maps the current MVC and API routes to their controllers, actions, and views.

## Home and Shared Routes

| Route | Method | Controller | Action | View | Notes |
|-------|--------|-----------|--------|------|-------|
| / | GET | Home | Index | Index.cshtml | Landing page |
| /Home/Privacy | GET | Home | Privacy | Privacy.cshtml | Privacy page |
| /Home/Error | GET | Home | Error | Error.cshtml | Error handler |
| /TheAdventurerLedger | GET | Home | Index | Index.cshtml | Thematic alias for home |

## Account Routes

| Route | Method | Controller | Action | View | Notes |
|-------|--------|-----------|--------|------|-------|
| /Account/SignIn | GET | Account | SignIn | SignIn.cshtml | Login form |
| /Account/SignIn | POST | Account | SignIn | - | Authenticate user |
| /Account/Register | GET | Account | Register | Register.cshtml | Registration form |
| /Account/Register | POST | Account | Register | - | Create Player + AppUser |
| /Account/ExternalLogin | GET | Account | ExternalLogin | - | Start Google login |
| /Account/ExternalLoginCallback | GET | Account | ExternalLoginCallback | - | Handle OAuth callback |
| /Account/SignOut | POST | Account | SignOutAction | - | Logout |
| /Account/Profile | GET | Account | Profile | Profile.cshtml | Current player profile |

**ViewModels:**
- AccountSignInViewModel
- AccountRegisterViewModel
- PlayerSignInViewModel

## Players Routes

| Route | Method | Controller | Action | View | Auth | Notes |
|-------|--------|-----------|--------|------|------|-------|
| /Players | GET | Players | Index | Index.cshtml | Anonymous | Player list with search |
| /Players/Search | GET | Players | Search | _PlayerCards.cshtml | Anonymous | Partial search results |
| /Players/Autocomplete | GET | Players | Autocomplete | - | Anonymous | Search suggestions |
| /Players/Create | GET | Players | Create | Create.cshtml | Admin/Manager | Create form |
| /Players/Create | POST | Players | Create | - | Admin/Manager | Save player |
| /Players/Details/{id} | GET | Players | Details | Details.cshtml | Authenticated | Player details and characters |
| /Players/Info/{id} | GET | Players | Details | Details.cshtml | Authenticated | Alias for details |
| /Players/{id}/CharactersSearch | GET | Players | CharactersSearch | _PlayerCharacters.cshtml | Authenticated | Partial character list |
| /Players/{id}/CharactersAutocomplete | GET | Players | CharactersAutocomplete | - | Authenticated | Character suggestions |
| /Players/Edit/{id} | GET | Players | Edit | Edit.cshtml | Authenticated | Edit form |
| /Players/Edit/{id} | POST | Players | Edit | - | Authenticated | Update player |
| /Players/Remove/{id} | GET | Players | Remove | Remove.cshtml | Admin | Remove confirmation |
| /Players/Remove/{id} | POST | Players | RemoveConfirmed | - | Admin | Soft-delete player |

## Characters Routes

| Route | Method | Controller | Action | View | Auth | Notes |
|-------|--------|-----------|--------|------|------|-------|
| /Characters | GET | Characters | Index | Index.cshtml | Anonymous | Character list with search |
| /Characters/Search | GET | Characters | Search | _CharacterCards.cshtml | Anonymous | Partial search results |
| /Characters/Autocomplete | GET | Characters | Autocomplete | - | Anonymous | Search suggestions |
| /Characters/Create | GET | Characters | Create | Create.cshtml | Authenticated | Create form |
| /Characters/Create | POST | Characters | Create | - | Authenticated | Save character |
| /Characters/Edit/{id} | GET | Characters | Edit | Edit.cshtml | Authenticated | Edit form |
| /Characters/Edit/{id} | POST | Characters | Edit | - | Authenticated | Update character |
| /Characters/Details/{id} | GET | Characters | Details | Details.cshtml | Authenticated | Character details |
| /Characters/Info/{id} | GET | Characters | Details | Details.cshtml | Authenticated | Alias for details |
| /Characters/{id}/Attachments | GET | Characters | Attachments | _AttachmentList.cshtml | Authenticated | Attachment list partial |
| /Characters/{id}/UploadAttachment | POST | Characters | UploadAttachment | - | Authenticated | Upload file |
| /Characters/DeleteAttachment/{id} | POST | Characters | DeleteAttachment | - | Authenticated | Soft-delete attachment |
| /Characters/Remove/{id} | GET | Characters | Remove | Remove.cshtml | Authenticated | Remove confirmation |
| /Characters/Remove/{id} | POST | Characters | RemoveConfirmed | - | Authenticated | Soft-delete character |
| /Characters/{id}/EquipmentSearch | GET | Characters | EquipmentSearch | _EquipmentCards.cshtml | Authenticated | Equipment partial |
| /Characters/{id}/EquipmentAutocomplete | GET | Characters | EquipmentAutocomplete | - | Authenticated | Equipment suggestions |
| /Characters/{id}/equipment/create-form | GET | Characters | EquipmentCreateForm | CreateEquipment.cshtml | Authenticated | Pick existing equipment |
| /Characters/{id}/equipment/create | POST | Characters | CreateEquipment | CreateEquipment.cshtml | Authenticated | Link equipment |
| /Characters/{characterId}/equipment/{equipmentId}/edit-form | GET | Characters | EquipmentEditForm | EditEquipment.cshtml | Authenticated | Edit linked equipment |
| /Characters/{characterId}/equipment/{equipmentId}/edit | POST | Characters | EditEquipment | EditEquipment.cshtml | Authenticated | Save linked equipment |
| /Characters/{characterId}/equipment/{equipmentId}/remove-form | GET | Characters | EquipmentRemoveForm | RemoveEquipment.cshtml | Authenticated | Remove confirmation |
| /Characters/{characterId}/equipment/{equipmentId}/remove | POST | Characters | RemoveEquipment | - | Authenticated | Unlink equipment |

**ViewModels:**
- CharacterFormViewModel
- CharacterWithPlayerViewModel
- CharacterEquipmentSelectionViewModel
- EquipmentFormViewModel

## Equipment Routes

| Route | Method | Controller | Action | View | Auth | Notes |
|-------|--------|-----------|--------|------|------|-------|
| /Equipment | GET | Equipment | Index | Index.cshtml | Admin/Manager | Global equipment catalog |
| /Equipment/Create | POST | Equipment | Create | - | Admin/Manager | Add custom equipment |
| /Equipment/{id}/Delete | POST | Equipment | Delete | - | Admin | Soft-delete custom equipment |

**ViewModels:**
- EquipmentManagementViewModel

## Codex Routes

| Route | Method | Controller | Action | View | Notes |
|-------|--------|-----------|--------|------|-------|
| /Codex | GET | Codex | Index | Index.cshtml | Search players, characters, and equipment together |

**ViewModel:**
- CodexSearchViewModel

## API Routes

### Players API

| Route | Method | Controller | Action | Notes |
|-------|--------|-----------|--------|-------|
| /api/players | GET | PlayersApi | GetAll | Optional `search` query |
| /api/players/{id} | GET | PlayersApi | GetById | Player details with characters |
| /api/players | POST | PlayersApi | Create | Create player |
| /api/players/{id} | PUT | PlayersApi | Update | Update player |
| /api/players/{id} | DELETE | PlayersApi | Delete | Soft-delete player |

### Characters API

| Route | Method | Controller | Action | Notes |
|-------|--------|-----------|--------|-------|
| /api/characters | GET | CharactersApi | GetAll | Optional `search` and `playerId` filters |
| /api/characters/{id} | GET | CharactersApi | GetById | Character details with level and equipment |
| /api/characters | POST | CharactersApi | Create | Create character, default level if omitted |
| /api/characters/{id} | PUT | CharactersApi | Update | Update character |
| /api/characters/{id} | DELETE | CharactersApi | Delete | Soft-delete character and linked equipment |

### Equipment API

| Route | Method | Controller | Action | Notes |
|-------|--------|-----------|--------|-------|
| /api/equipment | GET | EquipmentApi | GetAll | Optional `search` and `characterId` filters |
| /api/equipment/{id} | GET | EquipmentApi | GetById | Equipment details |
| /api/equipment | POST | EquipmentApi | Create | Create and link to a character |
| /api/equipment/{id} | PUT | EquipmentApi | Update | Update and optionally relink |
| /api/equipment/{id} | DELETE | EquipmentApi | Delete | Soft-delete equipment |

### Character Levels API

| Route | Method | Controller | Action | Notes |
|-------|--------|-----------|--------|-------|
| /api/character-levels | GET | CharacterLevelsApi | GetAll | Optional `search` query |
| /api/character-levels/{id} | GET | CharacterLevelsApi | GetById | Level details |
| /api/character-levels | POST | CharacterLevelsApi | Create | Prevents duplicate level values |
| /api/character-levels/{id} | PUT | CharacterLevelsApi | Update | Prevents duplicate level values |
| /api/character-levels/{id} | DELETE | CharacterLevelsApi | Delete | Unlinks characters then removes level |

## Shared View Models and DTOs

### ViewModels
- AccountSignInViewModel
- AccountRegisterViewModel
- CharacterFormViewModel
- CharacterEquipmentSelectionViewModel
- CharacterWithPlayerViewModel
- CodexSearchViewModel
- EquipmentFormViewModel
- EquipmentManagementViewModel
- ErrorViewModel
- PlayerFormViewModel
- PlayerSignInViewModel
- DateTimeControlViewModel

### API DTOs
- PlayerUpsertDto, PlayerDto, PlayerSummaryDto
- CharacterUpsertDto, CharacterSummaryDto, CharacterDto
- CharacterLevelUpsertDto, CharacterLevelDto
- EquipmentUpsertDto, EquipmentSummaryDto, EquipmentDto

## Notes

1. List pages and search endpoints filter out soft-deleted rows.
2. Equipment management is split between the global catalog and character-linked equipment.
3. Custom aliases are preserved for the themed routes used in navigation.
4. API endpoints are under `/api/*` and are protected by ASP.NET Core authorization.
