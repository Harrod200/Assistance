# Assistance Mod - Quick Start Guide

## 🚀 One-Step Deploy

After making code changes:

```powershell
# Build in Visual Studio
#    Ctrl+Shift+B

# Files are automatically copied to the game mod folder!
# No additional deployment step needed.
```

That's it! The build automatically:
- ✅ Compiles Assistance.dll
- ✅ Copies files to Terra Invicta Mods folder
- ✅ Ready to test in-game

**Configure Auto-Copy** (optional one-time setup):
- Project → Properties → Build Events → Post-build event
- Paste the PowerShell copy command from AI_DEVELOPER_SUMMARY.md
- Now every build automatically deploys

## 📋 Full Workflow

1. **Edit Code** - Make changes in Visual Studio
2. **Update Version** - Edit `Assistance/Properties/AssemblyInfo.cs`
   - Change: `[assembly: AssemblyVersion("0.3.5.0")]`
3. **Update Docs** - Edit `Assistance/AI_DEVELOPER_SUMMARY.md`
   - Update Version number at top
   - Update Last Updated date
   - Add entry to Version History table
4. **Build** - `Ctrl+Shift+B` in Visual Studio (files auto-copy)
5. **Test** - Restart Terra Invicta and load your save
6. **Verify** - Check game log for mod messages
7. **Commit** - Git commit with semantic message
5. **Deploy** - Run `.\Deploy.ps1`
6. **Test** - Restart Terra Invicta and check game log
7. **Commit** - Git commit with semantic message

## 🎮 Test in Game

After deployment, restart Terra Invicta and:

1. Load a save game (or start new campaign)
2. Select any player councilor
3. Check "Assist Councilor" appears in mission list
4. Verify the mission works with another councilor
5. Check game log for mod messages:
   ```
   Get-Content "$env:LOCALAPPDATA\..\LocalLow\Pavonis Interactive\TerraInvicta\Player.log" -Tail 50
   ```

## 📂 Important Paths

| Item | Path |
|------|------|
| **Solution Root** | `C:\Users\Chris\source\repos\Assistance\` |
| **Project Root** | `C:\Users\Chris\source\repos\Assistance\Assistance\` |
| **Mod Directory** | `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\` |
| **Game Log** | `%LOCALAPPDATA%\..\LocalLow\Pavonis Interactive\TerraInvicta\Player.log` |
| **Deploy Script** | `C:\Users\Chris\source\repos\Assistance\Deploy.ps1` |

## 📝 Key Files

- `Assistance/TIMissionTemplate_Assist.cs` - Main mission definition
- `Assistance/TIMissionEffect_Assist.cs` - Stat transfer logic
- `Assistance/English.xml` - UI text/localization
- `Assistance/AI_DEVELOPER_SUMMARY.md` - Full technical documentation
- `Deploy.ps1` - Automated deployment script (solution root)

## ❓ Troubleshooting

**Game doesn't load the mod?**
- Check mod files are in: `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\`
- Verify `ModInfo.json` exists (created by UMM)
- Check game log for error messages

**Script errors?**
- Make sure you built the solution (`Ctrl+Shift+B`)
- Run PowerShell as Administrator if permission errors occur
- Check paths match your system (C:\Games\Steam path might differ)

**Game crashes?**
- Check `Player.log` for stack traces
- See "Debugging & Troubleshooting" section in AI_DEVELOPER_SUMMARY.md
- Look for "Assist" in the log to find mod-related messages

## 🔗 Resources

- **Full Documentation**: See `Assistance/AI_DEVELOPER_SUMMARY.md`
- **Game Log Location**: `C:\Users\Chris\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log`
- **UMM Version**: 0.33.0.0+
- **Game**: Terra Invicta 1.0.53+
- **Target Framework**: .NET Framework 4.8

---

**Current Version**: 0.3.4  
**Last Updated**: 2026-09-04
