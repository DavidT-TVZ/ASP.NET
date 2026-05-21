param(
    [string]$MigrationName = $("AutoMigration_$(Get-Date -Format 'yyyyMMddHHmmss')")
)

Write-Host "Creating EF migration: $MigrationName"
dotnet ef migrations add $MigrationName --project DnD_Character_Sheet_Creator.DAL\DnD_Character_Sheet_Creator.DAL.csproj --startup-project DnD_Character_Sheet_Creator.Web\DnD_Character_Sheet_Creator.Web.csproj

if ($LASTEXITCODE -ne 0) { Write-Error "Migration add failed"; exit $LASTEXITCODE }

Write-Host "Applying database update"
dotnet ef database update --project DnD_Character_Sheet_Creator.DAL\DnD_Character_Sheet_Creator.DAL.csproj --startup-project DnD_Character_Sheet_Creator.Web\DnD_Character_Sheet_Creator.Web.csproj

if ($LASTEXITCODE -ne 0) { Write-Error "Database update failed"; exit $LASTEXITCODE }

Write-Host "Migration $MigrationName created and applied successfully."
