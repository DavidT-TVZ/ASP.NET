param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "./publish/web"
)

$ErrorActionPreference = "Stop"

$projectPath = "./DnD_Character_Sheet_Creator.Web/DnD_Character_Sheet_Creator.Web.csproj"

Write-Host "Publishing $projectPath ($Configuration) to $OutputDir..."

& dotnet publish $projectPath -c $Configuration -o $OutputDir

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE"
}

Write-Host "Publish completed successfully. Output: $OutputDir"
