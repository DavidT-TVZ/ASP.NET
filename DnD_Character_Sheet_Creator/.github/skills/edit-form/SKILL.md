---
name: edit-form
description: "Use when: creating or updating a Create/Edit form page for ASP.NET Core Models. Includes ViewModel, controller actions, and Razor view with validation."
---

# Edit/Create Form Page Builder

Creates a complete Create/Edit form workflow for D&D Character Sheet entities (Players, Characters). Produces: ViewModel, controller actions (GET/POST), and form view with client/server validation.

## Workflow

### 1. Create/Update ViewModel
- Located in `DnD_Character_Sheet_Creator.Web/ViewModels/<EntityName>FormViewModel.cs`
- Include all editable properties from the entity model
- Add `[Display]` attributes for labels
- Add `[Required]`, `[Range]`, `[StringLength]` validation attributes
- Include nested collections (e.g., list of available options for dropdowns)
- Example: `PlayerFormViewModel` with Name, Surname, and list of available Classes

**Checklist:**
- [ ] Properties match entity fields
- [ ] All validation attributes applied
- [ ] Display names are user-friendly
- [ ] Collection properties for dropdowns exist

### 2. Controller Actions
Add to appropriate controller (PlayersController, CharactersController):

**GET Action (Display Form)**
- Route: `[HttpGet] [Route("Create")]` for new records or `[HttpGet] [Route("Edit/{id}")]` for existing
- Fetch entity by ID (if editing)
- Populate ViewModel with entity data or defaults
- Load collection properties (dropdown options) from repository
- Return view with populated ViewModel
- Handle missing entity with `NotFound()`

**POST Action (Process Form)**
- Route: `[HttpPost]` matching the GET route
- Accept ViewModel parameter
- Validate `ModelState.IsValid`
- Map ViewModel to entity (use AutoMapper or manual mapping)
- Call repository method: `_repository.AddPlayer()` or `_repository.UpdatePlayer()`
- Redirect to Details or Index on success
- Return view with errors on validation failure

**Checklist:**
- [ ] Both GET and POST actions created
- [ ] ModelState validation in POST
- [ ] Repository methods called correctly
- [ ] Redirect to appropriate page after save
- [ ] NotFound() for missing entities in Edit

### 3. Create View (`Views/<Entity>/Create.cshtml` or `Edit.cshtml`)
- Inherit ViewModel: `@model PlayerFormViewModel`
- Add form with `asp-action` and `asp-controller` attributes
- Use Razor Tag Helpers for input binding:
  - `<input asp-for="PropertyName" />` for text inputs
  - `<select asp-for="PropertyName" asp-items="@Model.Options"></select>` for dropdowns
  - `<textarea asp-for="PropertyName"></textarea>` for longer text
- Add validation messages: `<span asp-validation-for="PropertyName"></span>`
- Include jQuery Validation scripts in view or layout
- Add Submit button with action name

**Form Elements by Property Type:**
| Entity Field | ViewModel Property | Form Element | Validation |
|---|---|---|---|
| String (name, email) | `string Name` | `<input asp-for="Name" />` | Required, StringLength |
| String (long text) | `string Biography` | `<textarea asp-for="Biography" />` | StringLength, Display |
| Number (level, age) | `int Level` | `<input asp-for="Level" type="number" />` | Required, Range |
| Enum (Class, Race) | `ClassEnum SelectedClass` | `<select asp-for="SelectedClass" asp-items="@Model.ClassOptions">` | Required |
| Bool | `bool IsActive` | `<input asp-for="IsActive" type="checkbox" />` | None |
| DateTime | `DateTime CreatedDate` | `<input asp-for="CreatedDate" type="datetime-local" />` | Required |

**Checklist:**
- [ ] View matches ViewModel type
- [ ] All ViewModel properties have form elements
- [ ] Validation messages displayed for each field
- [ ] Submit button targets correct action
- [ ] Cancel/Back link provided
- [ ] Form layout uses Bootstrap classes consistent with site

### 4. Add Validation & Error Handling
- **Server-side:** Check `ModelState.IsValid` in POST action
- **Client-side:** jQuery Validation & Unobtrusive enabled in wwwroot
  - Included via `wwwroot/lib/jquery-validation/`
  - Script references in Shared Layout: `_Layout.cshtml`
- **Custom Validation:** Create `[CustomValidation]` attributes in ViewModel if needed
  - Example: Ensure character level ≤ player level

**Checklist:**
- [ ] ModelState checked in POST action
- [ ] Validation messages appear on client
- [ ] Submit disabled until valid (optional)
- [ ] Server validates even if client-side bypassed
- [ ] Validation error messages are user-friendly

### 5. Update Index/Details Views with Links
- Add "Create" button on Index page
- Add "Edit" link on Details or Index rows
- Add "Delete" link if applicable (optional)
- Use button helpers: `<a asp-action="Create" asp-controller="Players" class="btn btn-primary">New Player</a>`

**Checklist:**
- [ ] Create button on Index
- [ ] Edit link points to correct route with ID
- [ ] Links use appropriate Bootstrap button styles
- [ ] Links are visually discoverable

## Decision Points

**Shared vs. Separate Views?**
- If Create and Edit forms are identical → Use single view, route both GET actions to it
- If forms differ (e.g., Edit hides created date) → Create separate `Create.cshtml` and `Edit.cshtml`
- Common: Use shared view, conditionally hide fields with `@if (Model.CharacterId == 0) { }`

**Enum Dropdowns?**
- Load from ViewModel collection: `asp-items="@Model.ClassOptions"`
- Populate in controller using `Enum.GetValues()` or hardcoded list
- Example: `Model.ClassOptions = Enum.GetNames(typeof(ClassEnum)).Select(x => new SelectListItem { Text = x, Value = x })`

**Nested Properties (e.g., Equipment)?**
- If simple: Include in single form (Equipment fields in Character form)
- If complex: Redirect to separate Equipment management page
- Current approach: Focus on primary entity, link to nested entities separately

## Template Files

Use these as starting points:

### PlayerFormViewModel.cshtml
```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DnD_Character_Sheet_Creator.Web.ViewModels
{
    public class PlayerFormViewModel
    {
        public int PlayerId { get; set; }
        
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string Surname { get; set; }

        [Display(Name = "Biography")]
        [StringLength(500)]
        public string Biography { get; set; }

        // For dropdowns
        public List<SelectListItem> AvailableClasses { get; set; } = new();
    }
}
```

### Controller Pattern
```csharp
[HttpGet]
[Route("Create")]
public IActionResult Create()
{
    var viewModel = new PlayerFormViewModel 
    { 
        AvailableClasses = GetClassOptions() 
    };
    return View(viewModel);
}

[HttpPost]
[Route("Create")]
public IActionResult Create(PlayerFormViewModel viewModel)
{
    if (!ModelState.IsValid)
    {
        viewModel.AvailableClasses = GetClassOptions();
        return View(viewModel);
    }

    var player = new Player 
    { 
        Name = viewModel.Name, 
        Surname = viewModel.Surname 
    };
    _playerRepository.AddPlayer(player);
    return RedirectToAction("Details", new { id = player.PlayerId });
}

private List<SelectListItem> GetClassOptions()
{
    return Enum.GetNames(typeof(ClassEnum))
        .Select(x => new SelectListItem { Text = x, Value = x })
        .ToList();
}
```

### View Pattern (Create.cshtml)
```html
@model PlayerFormViewModel

@{
    ViewData["Title"] = "Create Player";
}

<div class="container mt-5">
    <div class="row justify-content-center">
        <div class="col-md-6">
            <h2>@ViewData["Title"]</h2>
            
            <form asp-action="Create" asp-controller="Players" method="post">
                <div class="form-group mb-3">
                    <label asp-for="Name"></label>
                    <input asp-for="Name" class="form-control" />
                    <span asp-validation-for="Name" class="text-danger"></span>
                </div>

                <div class="form-group mb-3">
                    <label asp-for="Surname"></label>
                    <input asp-for="Surname" class="form-control" />
                    <span asp-validation-for="Surname" class="text-danger"></span>
                </div>

                <div class="form-group mb-3">
                    <label asp-for="Biography"></label>
                    <textarea asp-for="Biography" class="form-control"></textarea>
                    <span asp-validation-for="Biography" class="text-danger"></span>
                </div>

                <div class="form-group">
                    <button type="submit" class="btn btn-primary">Create</button>
                    <a asp-action="Index" class="btn btn-secondary">Cancel</a>
                </div>
            </form>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

## Quality Checklist

- [ ] ViewModel in `ViewModels/` folder with clear name
- [ ] Validation attributes on all user-editable fields
- [ ] Controller GET displays form with populated dropdowns
- [ ] Controller POST validates, maps, saves, and redirects
- [ ] View uses `asp-for` Tag Helpers (not hardcoded names)
- [ ] All form fields have error message spans
- [ ] Bootstrap styling applied consistently
- [ ] Index/Details views have create/edit links
- [ ] Form tested with valid data → saves ✓
- [ ] Form tested with invalid data → shows errors ✓
- [ ] Form tested with missing entity (Edit) → NotFound ✓

## Related Skills
- Character equipment management (nested forms)
- Form styling & DnD theme customization
