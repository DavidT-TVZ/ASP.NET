---
name: ajax-list-search
description: "Use when adding or updating search on any list view in the DnD Character Sheet Creator ASP.NET MVC app, especially AJAX filtering, autocomplete dropdowns, and nested lists such as Characters inside Player details."
---

# AJAX List Search Skill

Use this skill when a view renders a list of rows, cards, or related child items and needs live search with AJAX-backed autocomplete. The target patterns in this project are pages like Players Index, Characters Index, and nested lists such as the Characters section inside Player Details.
When the page shows several fields, search across the displayed fields unless the user explicitly excludes some of them. For this project, the Characters index should search the displayed character fields plus player name and level, and autocomplete should make those fields obvious.

## Workflow

### 1. Identify the List Surface
- Find every view that renders a repeatable list of entities or child entities.
- Decide whether the search applies to the page root list, a nested list, or both.
- For detail pages, search only the child collection section unless the user explicitly asks for parent filtering too.

**Checklist:**
- [ ] The searchable list is clearly identified
- [ ] Nested lists are treated separately from the main entity details
- [ ] Searchable fields are known before editing code

### 2. Define the Search Behavior
- Use AJAX for both filtering and autocomplete retrieval.
- Prefer a dedicated autocomplete endpoint that returns lightweight JSON results.
- Prefer a separate list-search endpoint or partial-result endpoint when the visible list must update without a full page reload.
- Search should feel live, but should not fire on every keystroke without debounce.

**Checklist:**
- [ ] Autocomplete has its own AJAX endpoint
- [ ] List filtering has a clear server endpoint
- [ ] Requests are debounced or otherwise rate-limited

### 3. Add Repository Filtering
- Add or extend repository methods so the query layer can filter by search text.
- Keep the filtering logic server-side so autocomplete and list results use the same rules.
- If the list is nested, include the parent key in the query, such as `playerId` for a player's characters.
- Return only the fields needed for the autocomplete dropdown when possible.

**Checklist:**
- [ ] Repository exposes a search-aware method
- [ ] Parent-scoped lists keep their parent filter
- [ ] Autocomplete data is lightweight

### 4. Add Controller Endpoints
- Add a GET endpoint for filtered list results.
- Add a GET endpoint for autocomplete suggestions, usually returning JSON.
- Accept a search term parameter such as `query`, `term`, or `search` and keep the naming consistent within the controller.
- If the page is a nested list inside details, keep the parent id parameter in both the list and autocomplete endpoints.

**Checklist:**
- [ ] List endpoint returns filtered data
- [ ] Autocomplete endpoint returns JSON
- [ ] Empty or short terms are handled safely

### 5. Update the Razor View
- Add a visible search box above the list.
- Add an autocomplete dropdown or suggestion panel bound to the search input.
- Add a placeholder and short helper text when that improves discoverability.
- For nested lists in details views, place the search box directly above the child collection section.
- Keep the existing card and sheet styling intact.
- Make the autocomplete panel use a higher stacking context than surrounding cards so it appears above nearby content.

**Checklist:**
- [ ] Search UI is easy to find
- [ ] Dropdown suggestions are visible and usable
- [ ] Nested list search stays near the list it filters

### 6. Wire the AJAX Script
- Use jQuery AJAX when fitting this app's existing layout and script stack.
- Fetch autocomplete results as the user types.
- Update the list area when a suggestion is selected or when search is submitted.
- Hide the dropdown when the input is empty or when the user clicks outside.
- Avoid stale updates by ignoring older responses when a newer request has already started.

**Checklist:**
- [ ] Typing loads suggestions asynchronously
- [ ] Selecting a suggestion updates the list or navigates to the chosen record
- [ ] Stale AJAX responses do not overwrite newer results

### 7. Verify the Experience
- Confirm the list still renders correctly with no search term.
- Confirm search works after clearing the field.
- Confirm autocomplete suggestions match the same server-side rules as the filtered list.
- Confirm the nested Characters list in Player Details only searches that section and not the whole player record.

**Checklist:**
- [ ] Default list state still works
- [ ] Search returns expected matches
- [ ] Autocomplete and list filtering agree
- [ ] Nested lists stay scoped correctly

## Decision Points

**Should search reload the whole page or only the list area?**
- Prefer only the list area when the page is a list-first screen.
- Prefer a full-page navigation only if the page already uses standard query-string filtering and the list is simple.
- For this project, default to AJAX list replacement.

**Should autocomplete return full records or labels only?**
- Return minimal JSON needed to render the dropdown.
- Include the id plus the display label, and add any secondary text only if the UI needs it.

**Should nested lists reuse the same endpoint as the main index page?**
- Only when the search scope and fields are identical.
- Otherwise keep a dedicated endpoint so the parent entity and the child collection do not interfere with each other.

## Template Pattern

### Controller Pattern
```csharp
[HttpGet]
public IActionResult Search(string? query)
{
    var results = _playerRepository.SearchPlayers(query);
    return PartialView("_PlayerList", results);
}

[HttpGet]
public IActionResult Autocomplete(string term)
{
    var suggestions = _playerRepository.SearchPlayers(term)
        .Select(player => new
        {
            id = player.PlayerId,
            label = player.Name + " " + player.Surname
        })
        .ToList();

    return Json(suggestions);
}
```

### Razor View Pattern
```html
<div class="sheet-card mb-4 reveal">
    <div class="row g-3 align-items-end">
        <div class="col-12 col-lg-8">
            <label for="searchInput" class="form-label">Search</label>
            <input id="searchInput" class="form-control" type="search" placeholder="Search players..." autocomplete="off" />
            <div id="autocompletePanel" class="list-group position-absolute w-100 d-none"></div>
        </div>
    </div>
</div>

<div id="listRegion">
    @await Html.PartialAsync("_PlayerList", Model)
</div>
```

### AJAX Pattern
```javascript
let activeRequest = null;

$('#searchInput').on('input', function () {
    const term = $(this).val();

    if (activeRequest) {
        activeRequest.abort();
    }

    activeRequest = $.ajax({
        url: '/Players/Autocomplete',
        data: { term: term },
        success: function (items) {
            // render autocomplete dropdown
        }
    });
});
```

## Quality Checklist

- [ ] Search is AJAX-driven
- [ ] Autocomplete uses a separate async endpoint
- [ ] Search scope matches the view being edited
- [ ] Repository filtering is shared by list and autocomplete logic
- [ ] Empty states still render cleanly
- [ ] Search remains scoped when used inside details pages
- [ ] Existing styling and navigation stay intact
- [ ] Autocomplete overlays nearby content cleanly

## When to Use This Skill
- Use it when adding search to a list page for Players, Characters, or any similar repeating UI.
- Use it when the user wants live suggestions while typing.
- Use it when a details page contains a searchable child list, such as the Characters section inside Player Details.

## When Updating an Existing Search
- Keep the endpoint names and query parameter names consistent.
- Reuse the repository filter method instead of duplicating search rules in the controller and JavaScript.
- If the list already supports paging, make sure the search endpoint respects the same paging and scoping rules.