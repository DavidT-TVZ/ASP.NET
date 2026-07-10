# ASP.NET
Program za popunjavanje DnD (5e, 2014 verzija) character sheet-a

## DnD Character Sheet Creator

### Prerequisites

- .NET SDK 9.0+
- SQL Server (Express, LocalDB, Azure SQL, ili drugi kompatibilni SQL Server)

### Local development

1. Postavite connection string kroz user-secrets ili environment varijablu.

	User-secrets primjer:

	```powershell
	cd DnD_Character_Sheet_Creator/DnD_Character_Sheet_Creator.Web
	dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\\SQLEXPRESS;Database=DnDSheetManager;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
	```

2. Pokrenite aplikaciju:

	```powershell
	dotnet run --project DnD_Character_Sheet_Creator/DnD_Character_Sheet_Creator.Web
	```

### Deployment readiness (Azure/VM)

Za deployment nemojte držati produkcijske tajne u `appsettings.json`.
Koristite environment varijable ili platformske postavke (Azure App Service Configuration).

Minimalne varijable:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection=<production-connection-string>`
- `Authentication__Google__ClientId=<google-client-id>`
- `Authentication__Google__ClientSecret=<google-client-secret>`

Napomena:

- Dvostruka donja crta (`__`) mapira se na `:` u .NET konfiguraciji.
- Aplikacija automatski pokreće migracije pri startupu u non-testing okruženjima.

### Publish

Primjer publish komande:

```powershell
dotnet publish DnD_Character_Sheet_Creator/DnD_Character_Sheet_Creator.Web/DnD_Character_Sheet_Creator.Web.csproj -c Release -o .\publish\web
```

Alternativno koristite skriptu:

```powershell
powershell -ExecutionPolicy Bypass -File DnD_Character_Sheet_Creator/scripts/publish_release.ps1
```
