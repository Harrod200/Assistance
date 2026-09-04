#!/usr/bin/env powershell
<#
.SYNOPSIS
Deploys the Assistance mod to Terra Invicta with proper file filtering.

.DESCRIPTION
This script compiles the mod and copies only the necessary files to the 
Terra Invicta mods folder, excluding PDB debug files.

.PARAMETER BuildConfiguration
The build configuration to use (Debug or Release). Defaults to Debug.

.EXAMPLE
.\Deploy-AssistanceMod.ps1 -BuildConfiguration Debug
#>

param(
	[ValidateSet("Debug", "Release")]
	[string]$BuildConfiguration = "Debug"
)

# Set paths
$repoRoot = "C:\Users\Chris\source\repos\Assistance"
$projectPath = "$repoRoot\Assistance\Assistance.csproj"
$sourceDll = "$repoRoot\Assistance\bin\$BuildConfiguration\Assistance.dll"
$sourceModInfo = "$repoRoot\Assistance\ModInfo.json"
$deployDir = "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission"

# Files to include in deployment (everything except PDB files)
$filesToDeploy = @(
	"*.dll",
	"*.json",
	"*.en",
	"*.xml"
)

# Files to exclude
$filesToExclude = @(
	"*.pdb",
	"*.cache.pdb"
)

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║          Assistance Mod Deployment Script                  ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan

Write-Host "`n📦 Deployment Configuration:"
Write-Host "  Build Config: $BuildConfiguration"
Write-Host "  Source: $repoRoot"
Write-Host "  Target: $deployDir"

# Step 1: Build
Write-Host "`n🔨 Building solution..."
try {
	$msbuild = "C:\Program Files\Microsoft Visual Studio\2026\Community\MSBuild\Current\Bin\MSBuild.exe"
	if (-not (Test-Path $msbuild)) {
		throw "MSBuild not found at expected path"
	}

	& $msbuild $projectPath /p:Configuration=$BuildConfiguration /p:Platform="Any CPU" /v:minimal

	if ($LASTEXITCODE -ne 0) {
		throw "Build failed with exit code $LASTEXITCODE"
	}
	Write-Host "✅ Build successful" -ForegroundColor Green
}
catch {
	Write-Host "❌ Build failed: $_" -ForegroundColor Red
	exit 1
}

# Step 2: Verify source files
Write-Host "`n🔍 Verifying source files..."
if (-not (Test-Path $sourceDll)) {
	Write-Host "❌ Compiled DLL not found: $sourceDll" -ForegroundColor Red
	exit 1
}
Write-Host "✅ Source DLL found: $sourceDll" -ForegroundColor Green

if (-not (Test-Path $sourceModInfo)) {
	Write-Host "❌ ModInfo.json not found: $sourceModInfo" -ForegroundColor Red
	exit 1
}
Write-Host "✅ ModInfo.json found: $sourceModInfo" -ForegroundColor Green

# Step 3: Create deployment directory
Write-Host "`n📁 Creating deployment directory..."
if (-not (Test-Path $deployDir)) {
	New-Item -ItemType Directory -Path $deployDir -Force | Out-Null
	Write-Host "✅ Created: $deployDir" -ForegroundColor Green
}
else {
	Write-Host "✅ Directory exists: $deployDir" -ForegroundColor Green
}

# Step 4: Clean cache and PDB files
Write-Host "`n🧹 Cleaning cache and PDB files..."
$removed = 0

# Remove cache files
Get-ChildItem -Path $deployDir -Filter "*.cache*" -Force -Recurse | Remove-Item -Force -ErrorAction SilentlyContinue
$cacheCount = (Get-ChildItem -Path $deployDir -Filter "*.cache*" -Force -Recurse | Measure-Object).Count
$removed += $cacheCount

# Remove PDB files
Get-ChildItem -Path $deployDir -Filter "*.pdb" -Force | Remove-Item -Force -ErrorAction SilentlyContinue
$pdbCount = (Get-ChildItem -Path $deployDir -Filter "*.pdb" -Force | Measure-Object).Count
$removed += $pdbCount

Write-Host "✅ Removed $removed cache and PDB files" -ForegroundColor Green

# Step 5: Copy files
Write-Host "`n📤 Deploying files..."

# Copy DLL
Copy-Item -Path $sourceDll -Destination $deployDir -Force
Write-Host "  ✓ Assistance.dll ($('{0:N0}' -f (Get-Item $sourceDll).Length) bytes)" -ForegroundColor Green

# Copy ModInfo.json
Copy-Item -Path $sourceModInfo -Destination $deployDir -Force
Write-Host "  ✓ ModInfo.json ($('{0:N0}' -f (Get-Item $sourceModInfo).Length) bytes)" -ForegroundColor Green

# Copy other supporting files (excluding PDB)
$supportFiles = Get-ChildItem -Path "$repoRoot\Assistance" -Include $filesToDeploy -Exclude $filesToExclude -Force
foreach ($file in $supportFiles) {
	if ($file.Name -ne "Assistance.dll" -and $file.Name -ne "ModInfo.json") {
		Copy-Item -Path $file.FullName -Destination $deployDir -Force -ErrorAction SilentlyContinue
		if ($?) {
			Write-Host "  ✓ $($file.Name) ($('{0:N0}' -f $file.Length) bytes)" -ForegroundColor Green
		}
	}
}

# Step 6: Verify deployment
Write-Host "`n✔️  Verifying deployment..."
$deployedFiles = Get-ChildItem -Path $deployDir -Force | Where-Object { $_.Name -notmatch "\.pdb|\.cache" }
Write-Host "`nDeployed files:"
$deployedFiles | ForEach-Object {
	$size = if ($_.PSIsContainer) { "DIR" } else { "{0:N0} bytes" -f $_.Length }
	Write-Host "  - $($_.Name) ($size)" -ForegroundColor Cyan
}

# Check for any remaining PDB files (should be none)
$pdbFiles = Get-ChildItem -Path $deployDir -Filter "*.pdb" -Force
if ($pdbFiles.Count -eq 0) {
	Write-Host "`n✅ No PDB files present" -ForegroundColor Green
}
else {
	Write-Host "`n⚠️  Warning: Found $($pdbFiles.Count) PDB files" -ForegroundColor Yellow
}

Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                  ✅ Deployment Complete                    ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan

Write-Host "`nReady to launch Terra Invicta!`n" -ForegroundColor Green
