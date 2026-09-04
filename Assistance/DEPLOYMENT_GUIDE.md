# Assistance Mod Deployment Guide

## Quick Deploy

Run the deployment script from PowerShell:

```powershell
C:\Users\Chris\source\repos\Assistance\Deploy-AssistanceMod.ps1
```

Or with Release configuration:

```powershell
C:\Users\Chris\source\repos\Assistance\Deploy-AssistanceMod.ps1 -BuildConfiguration Release
```

## What Gets Deployed

### Included Files
- ✅ `Assistance.dll` - Compiled mod with patches
- ✅ `ModInfo.json` - Mod metadata
- ✅ `*.en` files - Localization files
- ✅ `Settings.xml` - User settings
- ✅ Other necessary files

### Excluded Files
- ❌ `*.pdb` - Debug symbol files (NOT deployed)
- ❌ `*.cache.pdb` - Cache debug files (NOT deployed)

## Deployment Location

```
C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\
```

## What the Script Does

1. **Builds** the solution in the specified configuration (Debug/Release)
2. **Verifies** all source files are present
3. **Creates** the deployment directory if needed
4. **Cleans** cache and PDB files from the target directory
5. **Copies** only necessary files to the deployment directory
6. **Verifies** the deployment and confirms no PDB files are present

## Manual Deployment (if needed)

If you prefer to deploy manually without the script:

```powershell
# Define paths
$source = "C:\Users\Chris\source\repos\Assistance\Assistance\bin\Debug"
$dest = "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission"

# Copy only DLL (excludes PDB)
Copy-Item -Path "$source\Assistance.dll" -Destination $dest -Force

# Copy ModInfo.json
Copy-Item -Path "C:\Users\Chris\source\repos\Assistance\Assistance\ModInfo.json" -Destination $dest -Force

# Clean any leftover PDB files
Remove-Item -Path "$dest\*.pdb" -Force -ErrorAction SilentlyContinue
```

## Troubleshooting

### PDB Files Still Present
These are debug symbol files and won't affect the mod. They're excluded in future deployments.

### Build Fails
- Check that Visual Studio 2026 Community is installed
- Verify MSBuild path in the script
- Ensure .NET Framework 4.8 SDK is installed

### Files Not Copying
- Verify the source and destination paths exist
- Check file permissions
- Ensure Terra Invicta isn't currently running

## Recent Changes

- ✅ PDB files now excluded from deployments
- ✅ Cache files automatically cleaned on deploy
- ✅ Automated build and deployment in single script
- ✅ Deployment verification included
