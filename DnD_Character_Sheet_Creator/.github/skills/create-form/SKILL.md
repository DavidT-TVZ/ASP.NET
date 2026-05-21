> UI note: Use `btn-create` for Create actions. Add the `btn-create` styles to `wwwroot/css/site.css` to produce a light-blue button with a subtle magical sparkle animation on hover.

                    <div class="d-flex gap-2">
                        <button type="submit" class="btn btn-create">Create</button>
                        <a asp-action="Index" class="btn btn-outline-ink">Cancel</a>
                    </div>
---
name: create-form
description: "Use when creating a Create page for adding new data in the DnD Character Sheet Creator ASP.NET MVC app. Covers ViewModel, controller actions, Razor view, validation, index links, and the parchment-paper UI conventions from UI_Web_agent."
---

# Create Form Page Builder

Creates a complete Create page workflow for new D&D Character Sheet entities such as Players or Characters. Produces a ViewModel, controller GET/POST actions, and a Razor view with validation, consistent project styling, and the paper/parchment visual language used by the UI_Web_agent conventions.

## Workflow

### 1. Define the Create ViewModel
- Place it in `DnD_Character_Sheet_Creator.Web/ViewModels/<EntityName>FormViewModel.cs`
- Include only fields needed to create the new record
- Mark required fields with `[Required]`
- Add `[StringLength]`, `[Range]`, `[EmailAddress]`, and other validation attributes where appropriate
- Add friendly labels with `[Display(Name = "...")]`
- Include any dropdown sources as lists, select lists, or enum option collections

**Checklist:**
- [ ] ViewModel contains only create-time fields
- [ ] Validation attributes match the model rules
- [ ] Display labels are user-friendly
- [ ] Dropdown or enum option data is available when needed

### 2. Add Controller Actions
Add the Create actions to the relevant controller, usually `PlayersController` or `CharactersController`.

**GET Create**
- Use `[HttpGet]` and `[Route("Create")]`
- Instantiate the ViewModel
- Populate dropdown or enum option collections
- Return the view with the prepared ViewModel

**POST Create**
- Use `[HttpPost]` and `[Route("Create")]`
- Accept the same ViewModel type
- Check `ModelState.IsValid`
- Rebuild dropdown or enum option collections before returning the view on validation failure
- Map the ViewModel to the entity
- Set any defaults that should exist only at creation time, such as timestamps or initial values
- Save through the repository
- Redirect to `Details` or `Index` after success

**Checklist:**
- [ ] GET action prepares the form data
- [ ] POST action validates input before saving
- [ ] Form data is repopulated when validation fails
- [ ] Entity is saved through the repository
- [ ] Success redirects go to the right page

### 3. Build the Razor View
- Create `Views/<Entity>/Create.cshtml`
- Use `@model <EntityName>FormViewModel`
- Follow the UI_Web_agent theme rules: parchment-like surfaces, earthy tones, ink-like text, medieval-heading typography, and a form layout that feels like a character sheet
- Bind inputs with `asp-for`
- Show validation messages with `asp-validation-for`
- Use `type="email"`, `type="number"`, or `textarea` when the field type requires it
- Include a submit button and a cancel/back link
- Add `@section Scripts` with `_ValidationScriptsPartial`

**Checklist:**
- [ ] View matches the ViewModel type
- [ ] Every editable field has a form control
- [ ] Validation messages are visible under each field
- [ ] Cancel link returns to a sensible page
- [ ] Validation scripts are included

### 4. Add Entry Points
- Add a Create button on the entity Index page
- If useful, add a Create link in the navigation or details page
- Point links to the controller’s `Create` action

**Checklist:**
- [ ] Users can discover the Create page from the list view
- [ ] Link targets the correct controller and action
- [ ] Button styling matches the rest of the site

## Decision Points

**Does the entity need dropdowns?**
- If yes, load the options in the GET action and rebuild them in the POST action on validation failure
- If the option set is small and fixed, an enum-backed select list is fine
- If the option set comes from another repository, load it from that repository in the controller

**Does the entity need defaults on create?**
- If yes, set them in the POST action before saving
- Examples include timestamps, initial levels, or empty child collections
- Keep edit-only values out of the create form unless the user must choose them explicitly

**Should Create share a view with Edit?**
- If the fields are identical, a shared form can reduce duplication
- If create needs fewer fields or different defaults, keep a dedicated Create view
- For this project, prefer a separate Create page when it keeps the workflow clearer

## Template Pattern

### Controller Pattern
```csharp
[HttpGet]
[Route("Create")]
public IActionResult Create()
{
    var viewModel = new PlayerFormViewModel();
    return View(viewModel);
}

[HttpPost]
[Route("Create")]
public IActionResult Create(PlayerFormViewModel viewModel)
{
    if (!ModelState.IsValid)
    {
        return View(viewModel);
    }

    var player = new Player
    {
        Name = viewModel.Name,
        Surname = viewModel.Surname,
        Username = viewModel.Username,
        Email = viewModel.Email
    };

    _playerRepository.AddPlayer(player);
    return RedirectToAction("Details", new { id = player.PlayerId });
}
```

### View Pattern
```html
@model PlayerFormViewModel

@{
    ViewData["Title"] = "Create Player";
}

<section class="sheet-card mb-4 reveal">
    <p class="sheet-kicker mb-1">New Entry</p>
    <h1 class="h3 mb-1">@ViewData["Title"]</h1>
    <p class="text-muted mb-0">Fill in the details to add a new record to the ledger.</p>
</section>

<div class="row justify-content-center">
    <div class="col-12 col-lg-8">
        <div class="card sheet-card reveal mb-4">
            <div class="card-body">
                <form asp-action="Create" asp-controller="Players" method="post">
                    <div class="mb-4">
                        <label asp-for="Name" class="form-label"></label>
                        <input asp-for="Name" class="form-control" placeholder="First name" />
                        <span asp-validation-for="Name" class="text-danger small d-block mt-1"></span>
                    </div>

                    <div class="mb-4">
                        <label asp-for="Surname" class="form-label"></label>
                        <input asp-for="Surname" class="form-control" placeholder="Last name" />
                        <span asp-validation-for="Surname" class="text-danger small d-block mt-1"></span>
                    </div>

                    <div class="mb-4">
                        <label asp-for="Username" class="form-label"></label>
                        <input asp-for="Username" class="form-control" placeholder="Username" />
                        <span asp-validation-for="Username" class="text-danger small d-block mt-1"></span>
                    </div>

                    <div class="mb-4">
                        <label asp-for="Email" class="form-label"></label>
                        <input asp-for="Email" class="form-control" type="email" placeholder="Email address" />
                        <span asp-validation-for="Email" class="text-danger small d-block mt-1"></span>
                    </div>

                    <div class="d-flex gap-2">
                        <button type="submit" class="btn btn-ink">Create</button>
                        <a asp-action="Index" class="btn btn-outline-ink">Cancel</a>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>
```

## Quality Checklist

- [ ] ViewModel lives in `Web/ViewModels` and matches the entity being created
- [ ] Controller GET returns a populated form model
- [ ] Controller POST validates, maps, saves, and redirects
- [ ] Form uses Tag Helpers instead of hardcoded field names
- [ ] Validation scripts are included in the view
- [ ] Create button is discoverable from the Index page
- [ ] Defaults are set only when the record is created
- [ ] Error paths return the form with state preserved
 - [ ] Sitemap updated when new views/pages are added

## When adding views/pages
- After adding new Razor views or controller routes, update the `sitemap.md` (or project navigation) with the new route and link text so the site index stays in sync.
- For this project, `Players/Index` should always expose a Create entry point when a `Players/Create` page exists.

Example entry:

```
- [Characters — Create](/Characters/Create)
```