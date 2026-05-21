Skill: DateTime Format Editing

Purpose
- Capture a reusable, repo-scoped "skill" that documents how to edit, display, and localize DateTime formatting across the project.

When to use this skill
- Add or update locale-aware display of DateTime values in Razor views and client JS.
- Implement non-native date+time input widgets for editing (avoid `<input type=date>` native pickers).
- Standardize formatting for `hr` and `en` locales using browser Intl APIs or a lightweight JS picker with localization.

Recommended pattern
1. Rendering (display-only): use a shared Razor partial that emits an ISO timestamp and a fallback text.
   - Partial example: `Views/Shared/_DateTimeControl.cshtml` accepts a `DateTime?` and renders `<time data-date-time-control="true" data-date-time-iso="...">fallback</time>`.
2. Client formatting: use `Intl.DateTimeFormat` with the browser locale.
   - Example JS snippet:
     ```js
     const locale = (navigator.languages && navigator.languages[0]) || navigator.language || document.documentElement.lang || 'en-US';
     const fmt = new Intl.DateTimeFormat(locale, { dateStyle: 'medium', timeStyle: 'short' });
     const dt = new Date(isoString);
     element.textContent = fmt.format(dt);
     ```
3. Editable input (non-native): choose one of two options:
   - Option A (recommended): integrate a lightweight open-source picker such as `flatpickr` configured with `enableTime: true` and localized (hr/en). Add the library under `wwwroot/lib/flatpickr` and initialize in a Razor editor partial.
   - Option B: implement a custom JS date+time UI that collects date and time parts and outputs an ISO string on submit.

Server-side binding
- When accepting user input, convert local time to UTC server-side if the app stores UTC. Example: `DateTime.SpecifyKind(parsedLocal, DateTimeKind.Utc)` or convert via `DateTimeOffset`.
- Validate timezone expectations in controllers and document in the partial's README.

Accessibility and testing
- Ensure keyboard support for custom pickers and provide ARIA labels.
- Add unit/integration tests where possible to verify server-side parsing and formatting round-trips.
- QA in browsers with `hr` locale to confirm 24-hour formatting and date order.

Files to add/update
- `Views/Shared/_DateTimeControl.cshtml` — display partial.
- `ViewModels/DateTimeControlViewModel.cs` — optional view model.
- `wwwroot/js/site.js` — code to format controls and reapply after AJAX updates.
- Editor partial: `Views/Shared/_DateTimeEditor.cshtml` — input widget for create/edit forms.
- Add optional library under `wwwroot/lib/flatpickr/` if using `flatpickr`.

Usage checklist
- [ ] Add partial to display locations.
- [ ] Add editor partial to create/edit forms when editing is required.
- [ ] Add localized resources (if using a plugin requiring locale files).
- [ ] Update docs and memory note.

Example commit message
- "Add shared DateTime display partial and Intl-based client formatter; document skill for future editable pickers."

Notes
- Prefer browser Intl for display-only tasks to avoid extra dependencies.
- For editing, prefer a lightweight picker (flatpickr) localized to `hr`/`en` and initialized via a small JS wrapper that returns ISO timestamps to server.

Maintainer
- Add your name and contact here when you take ownership of this skill.
