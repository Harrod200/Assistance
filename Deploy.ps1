# Deploy.ps1 - Assistance Mod Deployment Script
# This script copies the compiled mod files to the Terra Invicta Mods directory

$ErrorActionPreference = "Stop"

# Define paths
$solutionRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$assemblyDll = Join-Path $solutionRoot "Assistance\bin\Debug\Assistance.dll"
$englishXml = Join-Path $solutionRoot "Assistance\English.xml"
$settingsXml = Join-Path $solutionRoot "Assistance\Settings.xml"
$modPath = "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission"

Write-Host "🚀 Assistance Mod Deployment Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Verify files exist
$filesToCheck = @(
	@{ Path = $assemblyDll; Name = "Assistance.dll" },
	@{ Path = $englishXml; Name = "English.xml" },
	@{ Path = $settingsXml; Name = "Settings.xml" }
)

$missingFiles = @()
foreach ($file in $filesToCheck) {
	if (Test-Path $file.Path) {
		Write-Host "✅ Found: $($file.Name)" -ForegroundColor Green
	} else {
		Write-Host "❌ Missing: $($file.Name) at $($file.Path)" -ForegroundColor Red
		$missingFiles += $file.Name
	}
}

if ($missingFiles.Count -gt 0) {
	Write-Host ""
	Write-Host "⚠️  Build the project first!" -ForegroundColor Yellow
	Write-Host "In Visual Studio: Build → Build Solution (Ctrl+Shift+B)"
	exit 1
}

Write-Host ""

# Ensure mod directory exists
if (!(Test-Path $modPath)) {
	Write-Host "📁 Creating mod directory: $modPath" -ForegroundColor Cyan
	New-Item -ItemType Directory -Path $modPath -Force | Out-Null
} else {
	Write-Host "📁 Mod directory exists: $modPath" -ForegroundColor Green
}

Write-Host ""

# Copy files
Write-Host "📋 Copying files..." -ForegroundColor Cyan
try {
	Copy-Item -Path $assemblyDll -Destination "$modPath\Assistance.dll" -Force
	Write-Host "   ✅ Copied Assistance.dll" -ForegroundColor Green

	Copy-Item -Path $englishXml -Destination "$modPath\English.xml" -Force
	Write-Host "   ✅ Copied English.xml" -ForegroundColor Green

	Copy-Item -Path $settingsXml -Destination "$modPath\Settings.xml" -Force
	Write-Host "   ✅ Copied Settings.xml" -ForegroundColor Green
}
catch {
	Write-Host "   ❌ Error copying files: $_" -ForegroundColor Red
	exit 1
}

Write-Host ""
Write-Host "✅ Deployment successful!" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Next steps:" -ForegroundColor Cyan
Write-Host "   1. Restart Terra Invicta to load the updated mod"
Write-Host "   2. Check the game log for 'Assist mission registered' message"
Write-Host "   3. Verify the Assist mission appears in councilor options"
Write-Host ""
Write-Host "📂 Mod location: $modPath" -ForegroundColor Gray
