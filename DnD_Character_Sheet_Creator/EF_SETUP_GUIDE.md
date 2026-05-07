# Entity Framework Core Setup Guide

## Overview
Entity Framework Core has been configured for your D&D Character Sheet Creator project using SQLite for data persistence.

## What Was Added

### 1. NuGet Packages
- `Microsoft.EntityFrameworkCore` - Core EF functionality
- `Microsoft.EntityFrameworkCore.Sqlite` - SQLite provider
- `Microsoft.EntityFrameworkCore.Tools` - CLI tools for migrations

### 2. Data Layer
- **DnDDbContext** (`Data/DnDDbContext.cs`) - Main DbContext class with:
  - DbSet for Players, Characters, CharacterLevels, and Equipment
  - Relationship configurations
  - Cascade delete rules

### 3. Repository Pattern
- **EFPlayerRepository** (`Repositories/EFPlayerRepository.cs`) - EF-based player repository
- **EFCharacterRepository** (`Repositories/EFCharacterRepository.cs`) - EF-based character repository
- Interfaces updated with CRUD methods (Add, Update, Delete)
- Mock repositories also updated with new methods for backward compatibility

### 4. Model Updates
- **Character.cs**: Added `PlayerId`, `LevelId`, and `Player` navigation property
- **Equipment.cs**: Added `CharacterId` and `Character` navigation property

### 5. Configuration
- **appsettings.json**: Added SQLite connection string
- **Program.cs**: Configured DbContext with SQLite and registered repositories

## Relationship Structure

```
Player (1) ──── (Many) Character
            │
            └── (Many) Equipment
                    │
                    └── (1) CharacterLevel
```

## Next Steps: Creating the Database

### Option 1: Using Package Manager Console (Visual Studio)
1. Open Package Manager Console
2. Set Default Project to `DnD_Character_Sheet_Creator`
3. Run:
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```

### Option 2: Using .NET CLI (Terminal/PowerShell)
```bash
cd path\to\DnD_Character_Sheet_Creator
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This will create a `DnDCharacterSheet.db` file in your project root.

## Switching Between Mock and EF Data

In `Program.cs`, you can easily switch:

### To use Entity Framework (default):
```csharp
builder.Services.AddScoped<IPlayerRepository, EFPlayerRepository>();
builder.Services.AddScoped<ICharacterRepository, EFCharacterRepository>();
```

### To use Mock Data:
```csharp
builder.Services.AddSingleton<IPlayerRepository, MockPlayerRepository>();
builder.Services.AddSingleton<ICharacterRepository, MockCharacterRepository>();
```

## Key Features

- **Lazy Loading**: Relationships are configured with `.Include()` for eager loading
- **Cascade Delete**: Deleting a player deletes their characters and equipment
- **CRUD Operations**: All repositories support Create, Read, Update, Delete
- **Type Safety**: Strong typing with navigation properties

## Important Notes

- The database file (`DnDCharacterSheet.db`) will be created in the project root directory
- Add `*.db` to your `.gitignore` to exclude the database from version control
- Use `dotnet ef migrations add <MigrationName>` to create new migrations
- Use `dotnet ef migrations remove` to undo the last migration (before running Update-Database)
- Use `dotnet ef database drop` to delete the database

## Troubleshooting

**Error: "Unable to find an EF Core command"**
- Ensure `Microsoft.EntityFrameworkCore.Tools` is installed
- Rebuild the project

**Error: "The specified module could not be found"**
- Close and reopen Visual Studio or your terminal
- Ensure all packages are restored: `dotnet restore`

**SQLite file not creating?**
- Check that the connection string path is correct in `appsettings.json`
- Ensure the project has write permissions in the output directory
- Check that migrations have been created and applied
