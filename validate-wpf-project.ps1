$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $PSScriptRoot 'SutraMind.sln'
$desktopProject = Join-Path $PSScriptRoot 'SutraMind.Desktop/SutraMind.Desktop.csproj'

if (-not (Test-Path -LiteralPath $solution)) {
    throw "Solution not found: $solution"
}

if (-not (Test-Path -LiteralPath $desktopProject)) {
    throw "Desktop project not found: $desktopProject"
}

$tempProjects = Get-ChildItem -LiteralPath $PSScriptRoot -Recurse -Filter '*_wpftmp.csproj' -File -ErrorAction SilentlyContinue
if ($tempProjects) {
    throw "Temporary WPF project files are present: $($tempProjects.FullName -join ', ')"
}

$solutionProjects = dotnet sln $solution list | Out-String
if ($solutionProjects -notmatch 'SutraMind.Desktop.csproj') {
    throw 'The real WPF desktop project is not registered in the solution.'
}

Write-Output 'WPF project validation passed: real project exists, solution registration is valid, and no temporary _wpftmp.csproj remains.'
