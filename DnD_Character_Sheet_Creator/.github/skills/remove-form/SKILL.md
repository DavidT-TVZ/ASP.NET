---
name: remove-form
description: "Use when creating a remove/delete workflow for ASP.NET MVC entities in the DnD Character Sheet Creator app. Covers soft delete, DeletedAt support, controller actions, confirmation view, repository updates, and filtering removed entities from lists."
---

# Remove Form Page Builder

Creates a complete soft-delete workflow for D&D Character Sheet entities such as Players, Characters, and related data objects. Produces a confirmation page, controller actions, repository updates, and list filtering so removed records are hidden from normal entity lists.

## Workflow

### 1. Add Soft-Delete Support to the Entity
- Add a nullable `DeletedAt` property to every entity that can be removed
- Use `DateTime? DeletedAt` so active entities stay visible and removed entities can be marked with a timestamp
- Scaffold a real EF Core migration from the DAL project with the Web project as the startup project, so EF generates the migration `.cs`, the matching `.Designer.cs`, and the snapshot update together
- Apply the migration immediately after scaffolding with `dotnet ef database update` against the same DAL/Web project pair
- Verify the database state by checking `__EFMigrationsHistory` and the `Characters`/target table schema to confirm the new nullable `DeletedAt` column exists
- Keep the existing primary key and domain fields unchanged

**Checklist:**
- [ ] Entity has a nullable `DeletedAt` property
- [ ] EF migration is scaffolded with the correct DAL/Web project pairing
- [ ] `dotnet ef database update` applies the migration to the target database
- [ ] `__EFMigrationsHistory` includes the new migration and the table has a nullable `DeletedAt` column

### 2. Update Repository Logic
- Change delete operations to perform a soft delete instead of removing rows
- Set `DeletedAt = DateTime.UtcNow` or the project’s chosen timestamp standard when the object is removed
- Update repository query methods so normal reads ignore rows where `DeletedAt` is not null
- If the repository exposes a details or lookup method, decide whether it should return removed rows or hide them consistently with the rest of the app

**Checklist:**
- [ ] Delete methods set `DeletedAt` instead of calling hard delete
- [ ] Read methods filter out removed entities by default
- [ ] Repository behavior is consistent across list, details, and edit flows

### 3. Build the Remove Confirmation Page
- Create `Views/<Entity>/Remove.cshtml` or `Delete.cshtml` depending on the existing naming in the controller
- Use the page to confirm the user wants to remove the object before applying the soft delete
- Show the key identifying fields only, enough for the user to confirm the target record
- Use the existing parchment/card styling and the project button conventions
- Include a primary remove button and a cancel/back link

**Checklist:**
- [ ] Confirmation page identifies the record clearly
- [ ] Primary action is clearly destructive
- [ ] Cancel link returns to a safe page
- [ ] Layout matches the rest of the app

### 4. Add Controller Actions
Add remove actions to the relevant controller, usually `PlayersController` or `CharactersController`.

**GET Remove/Delete**
- Load the entity by id
- If the entity does not exist, return `NotFound()`
- If the entity is already removed, either return `NotFound()` or redirect to a sensible page based on existing app behavior
- Return the confirmation view with the entity or a small view model

**POST Remove/Delete**
- Accept the entity id or a minimal view model
- Re-check that the entity exists and is not already removed
- Set `DeletedAt` to the current timestamp
- Save changes through the repository
- Redirect to `Index` or another appropriate list page

**Checklist:**
- [ ] GET action loads the target entity
- [ ] POST action performs soft delete
- [ ] Missing entities are handled safely
- [ ] Success redirects to the right page

### 5. Filter All List Pages
- Update every list query so it only returns rows where `DeletedAt == null`
- Apply the filter in controller actions, repository methods, or shared query helpers so it is consistent everywhere
- Make sure search, paging, and related list features also honor the same filter
- If admin or maintenance views need to see removed records, keep those separate and explicit

**Checklist:**
- [ ] Index/list pages exclude removed entities
- [ ] Filtering is applied consistently across all entity lists
- [ ] Search and paging do not reintroduce removed records

## Decision Points

**Should the page be called Remove or Delete?**
- Follow the naming already used in the controller and views
- If the project already uses `Delete`, keep that route and view name
- If the UI language should feel softer, use `Remove` in labels while keeping route names aligned with the codebase

**Should removed entities be recoverable?**
- If yes, keep the `DeletedAt` field and add a future restore workflow
- If no, still avoid hard delete unless the user explicitly requests permanent deletion
- For this project, default to soft delete only

**Should details pages show removed entities?**
- Usually no, because the list views are filtered and the object is treated as inactive
- If a detail page can still be reached directly, show a not found message or a removed-state message consistently

## Template Pattern

### Entity Pattern
```csharp
public class Player
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public DateTime? DeletedAt { get; set; }
}
```

### Repository Pattern
```csharp
public Player? GetPlayerById(int id)
{
    return _context.Players
        .FirstOrDefault(player => player.PlayerId == id && player.DeletedAt == null);
}

public void SoftDeletePlayer(int id)
{
    var player = _context.Players.FirstOrDefault(player => player.PlayerId == id && player.DeletedAt == null);
    if (player == null)
    {
        return;
    }

    player.DeletedAt = DateTime.UtcNow;
    _context.SaveChanges();
}
```

### Controller Pattern
```csharp
[HttpGet]
[Route("Delete/{id}")]
public IActionResult Delete(int id)
{
    var player = _playerRepository.GetPlayerById(id);
    if (player == null)
    {
        return NotFound();
    }

    return View(player);
}

[HttpPost]
[ValidateAntiForgeryToken]
[Route("Delete/{id}")]
public IActionResult DeleteConfirmed(int id)
{
    _playerRepository.SoftDeletePlayer(id);
    return RedirectToAction("Index");
}
```

### View Pattern
```html
@model Player

@{
    ViewData["Title"] = "Remove Player";
}

<section class="sheet-card mb-4 reveal">
    <p class="sheet-kicker mb-1">Confirm Removal</p>
    <h1 class="h3 mb-1">@ViewData["Title"]</h1>
    <p class="text-muted mb-0">This will hide the record from normal lists.</p>
</section>

<div class="row justify-content-center">
    <div class="col-12 col-lg-8">
        <div class="card sheet-card reveal mb-4">
            <div class="card-body">
                <dl class="row mb-4">
                    <dt class="col-sm-4">Name</dt>
                    <dd class="col-sm-8">@Model.Name</dd>
                </dl>

                <form asp-action="Delete" asp-route-id="@Model.PlayerId" method="post">
                    @Html.AntiForgeryToken()
                    <div class="d-flex gap-2">
                        <button type="submit" class="btn btn-danger">Remove</button>
                        <a asp-action="Index" class="btn btn-outline-ink">Cancel</a>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>
```

## Quality Checklist

- [ ] Entity includes nullable `DeletedAt`
- [ ] Delete operation writes the timestamp instead of removing the row
- [ ] All normal lists filter to `DeletedAt == null`
- [ ] Confirmation page is clear and matches site styling
- [ ] Controller handles missing or already removed entities safely
- [ ] Repository methods stay consistent across the app
- [ ] Migration and model snapshot are updated when needed

## When updating lists
- After adding `DeletedAt`, audit all index/list queries in controllers and repositories
- Make sure the same filter applies to lookup methods used by dropdowns, related data, and search results
- If a view should intentionally show removed records, make that behavior explicit instead of relying on default queries

## Database Update Steps
- Run `dotnet ef migrations add <MigrationName> --project <DAL csproj> --startup-project <Web csproj>` when introducing `DeletedAt` or any related schema change
- Ensure the generated migration has both the migration body and the matching designer file
- Run `dotnet ef database update --project <DAL csproj> --startup-project <Web csproj>` to apply the migration to the local SQL Server database
- Confirm the change by querying `INFORMATION_SCHEMA.COLUMNS` and `__EFMigrationsHistory` after the update completes
