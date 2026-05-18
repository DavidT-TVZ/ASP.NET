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
| `/Players` | `PlayersController.Index()` | `Views/Players/Index.cshtml` | Also available through `/Players/Index` |
| `/Players/Index` | `PlayersController.Index()` | `Views/Players/Index.cshtml` | Default route alias |
| `/Actors` | `PlayersController.Index()` | `Views/Players/Index.cshtml` | DnD-themed custom route |
| `/Players/Details/{id}` | `PlayersController.Details(int id)` | `Views/Players/Details.cshtml` | Returns `NotFound()` when the player does not exist |
| `/Players/Info/{id}` | `PlayersController.Details(int id)` | `Views/Players/Details.cshtml` | DnD-themed custom route for details |
| `/Actors/Details/{id}` | `PlayersController.Details(int id)` | `Views/Players/Details.cshtml` | DnD-themed custom route for details |
| `/Actors/Info/{id}` | `PlayersController.Details(int id)` | `Views/Players/Details.cshtml` | DnD-themed custom route for details |
| `/Characters` | `CharactersController.Index()` | `Views/Characters/Index.cshtml` | Also available through `/Characters/Index` |
| `/Characters/Index` | `CharactersController.Index()` | `Views/Characters/Index.cshtml` | Default route alias |
| `/Adventurers` | `CharactersController.Index()` | `Views/Characters/Index.cshtml` | DnD-themed custom route |
| `/Characters/Details/{id}` | `CharactersController.Details(int id)` | `Views/Characters/Details.cshtml` | Returns `NotFound()` when the character does not exist |
| `/Characters/Info/{id}` | `CharactersController.Details(int id)` | `Views/Characters/Details.cshtml` | DnD-themed custom route for details |
| `/Adventurers/Details/{id}` | `CharactersController.Details(int id)` | `Views/Characters/Details.cshtml` | DnD-themed custom route for details |
| `/Adventurers/Info/{id}` | `CharactersController.Details(int id)` | `Views/Characters/Details.cshtml` | DnD-themed custom route for details |

## View model and data passed to views

- `Views/Home/Index.cshtml` is rendered with no explicit model.
- `Views/Home/Privacy.cshtml` is rendered with no explicit model.
- `Views/Shared/Error.cshtml` receives `ErrorViewModel`.
- `Views/Players/Index.cshtml` receives a list of `Player` objects with their `CharacterList` populated.
- `Views/Players/Details.cshtml` receives a single `Player` object and also uses `ViewBag.Characters`.
- `Views/Characters/Index.cshtml` receives a list of `CharacterWithPlayerViewModel` objects.
- `Views/Characters/Details.cshtml` receives one `CharacterWithPlayerViewModel`.

## Navigation links

- The shared layout links to Home, Players, Characters, and Privacy.
- The home page also links to the Players and Characters index pages.
- The list/detail pages cross-link back to their related player or character views.