# Sitemap

## Route map

| URL pattern | Controller action | View used | Notes |
| --- | --- | --- | --- |
| `/` | `HomeController.Index()` | `Views/Home/Index.cshtml` | Also available through `/Home` and `/Home/Index` because of the default route |
| `/Home` | `HomeController.Index()` | `Views/Home/Index.cshtml` | Default route alias |
| `/Home/Index` | `HomeController.Index()` | `Views/Home/Index.cshtml` | Default route alias |
| `/TheAdventurerLedger` | `HomeController.Index()` | `Views/Home/Index.cshtml` | DnD-themed custom route |
| `/Home/Privacy` | `HomeController.Privacy()` | `Views/Home/Privacy.cshtml` | Standard static page |
| `/Home/Error` | `HomeController.Error()` | `Views/Shared/Error.cshtml` | Used by the exception handler in non-development environments |
| `/Account/SignIn` | `AccountController.SignIn()` | `Views/Account/SignIn.cshtml` | Cookie-auth sign-in page where the active player is selected |
| `/Account/SignOut` | `AccountController.SignOutAction()` | redirect | Clears the active player session |
| `/Players` | `PlayersController.Index()` | `Views/Players/Index.cshtml` | Also available through `/Players/Index` |
| `/Players/Index` | `PlayersController.Index()` | `Views/Players/Index.cshtml` | Default route alias |
| `/Actors` | `PlayersController.Index()` | `Views/Players/Index.cshtml` | DnD-themed custom route |
| `/Players/Create` | `PlayersController.Create()` | `Views/Players/Create.cshtml` | Admin-only player creation form |
| `/Players/Search` | `PlayersController.Search(string? query)` | `Views/Players/_PlayerCards.cshtml` | AJAX list-search endpoint for the player index |
| `/Players/Autocomplete` | `PlayersController.Autocomplete(string term)` | JSON | AJAX autocomplete endpoint for the player index |
| `/Players/Details/{id}` | `PlayersController.Details(int id)` | `Views/Players/Details.cshtml` | Returns `NotFound()` when the player does not exist |
| `/Players/Info/{id}` | `PlayersController.Details(int id)` | `Views/Players/Details.cshtml` | DnD-themed custom route for details |
| `/Actors/Details/{id}` | `PlayersController.Details(int id)` | `Views/Players/Details.cshtml` | DnD-themed custom route for details |
| `/Actors/Info/{id}` | `PlayersController.Details(int id)` | `Views/Players/Details.cshtml` | DnD-themed custom route for details |
| `/Players/Edit/{id}` | `PlayersController.Edit(int id)` | `Views/Players/Edit.cshtml` | Edit player form; returns `NotFound()` when the player is missing |
| `/Players/Remove/{id}` | `PlayersController.Remove(int id)` | `Views/Players/Remove.cshtml` | Soft-delete confirmation page for players |
| `/Players/{id}/CharactersSearch` | `PlayersController.CharactersSearch(int id, string? query)` | `Views/Players/_PlayerCharacters.cshtml` | AJAX list-search endpoint for the player's character section |
| `/Players/{id}/CharactersAutocomplete` | `PlayersController.CharactersAutocomplete(int id, string term)` | JSON | AJAX autocomplete endpoint for the player's character section |
| `/Characters` | `CharactersController.Index()` | `Views/Characters/Index.cshtml` | Also available through `/Characters/Index` |
| `/Characters/Index` | `CharactersController.Index()` | `Views/Characters/Index.cshtml` | Default route alias |
| `/Adventurers` | `CharactersController.Index()` | `Views/Characters/Index.cshtml` | DnD-themed custom route |
| `/Characters/Create` | `CharactersController.Create()` | `Views/Characters/Create.cshtml` | Create new character form |
| `/Characters/Details/{id}` | `CharactersController.Details(int id)` | `Views/Characters/Details.cshtml` | Returns `NotFound()` when the character does not exist |
| `/Characters/Info/{id}` | `CharactersController.Details(int id)` | `Views/Characters/Details.cshtml` | DnD-themed custom route for details |
| `/Adventurers/Details/{id}` | `CharactersController.Details(int id)` | `Views/Characters/Details.cshtml` | DnD-themed custom route for details |
| `/Adventurers/Info/{id}` | `CharactersController.Details(int id)` | `Views/Characters/Details.cshtml` | DnD-themed custom route for details |
| `/Characters/Remove/{id}` | `CharactersController.Remove(int id)` | `Views/Characters/Remove.cshtml` | Soft-delete confirmation page for characters |
| `/Adventurers/Remove/{id}` | `CharactersController.Remove(int id)` | `Views/Characters/Remove.cshtml` | DnD-themed custom route for removal |
| `/Characters/{id}/equipment/create-form` | `CharactersController.EquipmentCreateForm(int id)` | `Views/Characters/CreateEquipment.cshtml` | Equipment creation form for the selected character |
| `/Characters/{characterId}/equipment/{equipmentId}/edit-form` | `CharactersController.EquipmentEditForm(int characterId, int equipmentId)` | `Views/Characters/EditEquipment.cshtml` | Equipment edit form |
| `/Characters/{characterId}/equipment/{equipmentId}/remove-form` | `CharactersController.EquipmentRemoveForm(int characterId, int equipmentId)` | `Views/Characters/RemoveEquipment.cshtml` | Equipment soft-delete confirmation page |

## View model and data passed to views

- `Views/Home/Index.cshtml` is rendered with no explicit model.
- `Views/Home/Privacy.cshtml` is rendered with no explicit model.
- `Views/Shared/Error.cshtml` receives `ErrorViewModel`.
- `Views/Account/SignIn.cshtml` receives `PlayerSignInViewModel` with the available player list and optional return URL.
- `Views/Players/Index.cshtml` receives a list of `Player` objects with their `CharacterList` populated; non-admin users only see their own record.
- `Views/Players/Details.cshtml` receives a single `Player` object and also uses `ViewBag.Characters`; non-admin users can only open their own record.
- `Views/Players/Create.cshtml` and `Views/Players/Edit.cshtml` receive `PlayerFormViewModel` with an `IsAdmin` flag, but only admins can create or assign admin status.
- `Views/Characters/Index.cshtml` receives a list of `CharacterWithPlayerViewModel` objects.
- `Views/Characters/Details.cshtml` receives one `CharacterWithPlayerViewModel`.
- `Views/Characters/Create.cshtml` and `Views/Characters/Edit.cshtml` receive `CharacterFormViewModel`; non-admin users are scoped to their own player id.
- `Views/Characters/CreateEquipment.cshtml`, `Views/Characters/EditEquipment.cshtml`, and `Views/Characters/RemoveEquipment.cshtml` receive `EquipmentFormViewModel` or an equipment entity depending on the route.

## Navigation links

- The shared layout links to Home, Players, Characters, Privacy, and now shows the active signed-in player plus Sign in/Sign out actions.
- The home page also links to the Players and Characters index pages.
- The list/detail pages cross-link back to their related player or character views.
- The active browser session is authenticated by cookie-based sign-in rather than ASP.NET identity accounts, so the selected player controls what the user can see and edit.