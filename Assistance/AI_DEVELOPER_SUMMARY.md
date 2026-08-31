# Assistance Mod - AI Developer Handoff Summary

**Version:** 0.3.10  
**Last Updated:** 2026-09-07  
**Status:** ✅ Stable and Tested  
**Target Game:** Terra Invicta 1.0.53+  
**UMM Version:** 0.33.0.0+

---

## 📋 Developer Workflow Instructions

**IMPORTANT:** Every commit to this project must follow these steps:

### Consent Protocol ⚠️
**Before making ANY changes to the solution:**
1. **Describe** the proposed changes clearly
2. **Wait for explicit approval** from the project maintainer
3. **Only proceed** after receiving consent (e.g., "yes", "proceed", "approved")
4. **Report results** with a summary of what was changed

**This protocol applies to:**
- ✅ Source code modifications (`.cs` files)
- ✅ Project configuration (`.csproj`)
- ✅ Localization files (`.en`)
- ✅ Documentation updates (`.md`)
- ✅ Assembly info or version changes
- ✅ Any file additions, deletions, or edits

**Purpose:** Ensures all modifications are intentional and aligned with project goals.

---

### Before Committing
1. **Increment the version number** in `Properties/AssemblyInfo.cs`:
   ```csharp
   [assembly: AssemblyVersion("X.Y.Z.0")]
   [assembly: AssemblyFileVersion("X.Y.Z.0")]
   ```
   - Use semantic versioning: MAJOR.MINOR.PATCH
   - MAJOR: Breaking changes or major feature additions
   - MINOR: New features or significant fixes
   - PATCH: Bug fixes, small improvements, documentation updates

2. **Update this developer summary**:
   - Update the **Version** at the top to match `AssemblyInfo.cs`
   - Update the **Last Updated** date (format: YYYY-MM-DD)
   - Add new row to **Version History** table at the top:
     ```markdown
     | 0.X.Y | YYYY-MM-DD | Summary of changes made in this version |
     ```
   - Ensure CURRENT marker is on the latest version
   - Update any relevant sections (Project Structure, Critical Details, Known Issues, etc.)

3. **Build and test** the solution to ensure no compilation errors:
   ```powershell
   dotnet build Assistance/Assistance.csproj
   ```

4. **Copy to game folder** when ready for testing:
   ```powershell
   Copy-Item "Assistance\bin\Debug\Assistance.dll" "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force
   ```

### Committing
5. **Stage changes**:
   ```powershell
   git add -A
   ```

6. **Commit with semantic message**:
   ```powershell
   git commit -m "feat/fix/docs: Brief description

   - Detailed bullet point 1
   - Detailed bullet point 2
   - Detailed bullet point 3"
   ```
   - Use prefix: `feat:` (feature), `fix:` (bug fix), `docs:` (documentation), `refactor:` (code restructuring), `perf:` (performance)
   - Include detailed changelog in body
   - Reference version number in commit body if significant

7. **Push to master**:
   ```powershell
   git push origin master
   ```

### Example Workflow
```powershell
# 1. Make code changes
# 2. Update AssemblyInfo.cs version from 0.3.1 to 0.3.2
# 3. Update AI_DEVELOPER_SUMMARY.md:
#    - Change Version: 0.3.1 → 0.3.2
#    - Change Last Updated: 2026-09-02 → 2026-09-03
#    - Add row to Version History table
#    - Update relevant documentation sections
# 4. Build
cd "C:\Users\Chris\source\repos\Assistance"
dotnet build Assistance/Assistance.csproj

# 5. Copy to game folder for testing
Copy-Item "Assistance\bin\Debug\Assistance.dll" "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force

# 6. Git operations
git add -A
git commit -m "fix: Resolve issue with bonus calculation

- Fixed bonus amount calculation when source stat is 0
- Added null check in AssistBonusTracker
- Updated version to 0.3.2"
git push origin master
```

### Quick Command Reference (Copy & Paste)
```powershell
# Navigate to solution
cd "C:\Users\Chris\source\repos\Assistance"

# Build solution
dotnet build Assistance/Assistance.csproj

# Copy DLL to game folder when ready for testing
Copy-Item "Assistance\bin\Debug\Assistance.dll" "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force

# Copy localization file (TIMissionTemplate.en - correct naming convention per v0.3.8)
Copy-Item "Assistance\TIMissionTemplate.en" "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force

# Clear mod cache to force fresh load (important after code changes with Harmony patches)
Remove-Item "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\*.cache" -Force -ErrorAction SilentlyContinue

# Check game log for mod messages
Get-Content "$env:LOCALAPPDATA\..\LocalLow\Pavonis Interactive\TerraInvicta\Player.log" -Tail 50
```

---

## 🎯 Quick Overview

The **Assistance Mod** adds an "Assist Councilor" mission to Terra Invicta that allows one player councilor to temporarily share a configurable percentage (0-100%, default 25%) of their stats with another friendly councilor. This helps completing difficult missions faster.

**Key Features:**
- ✅ Targets any friendly councilor (including those with active missions)
- ✅ Configurable assist percentage (0-100%, stored in game settings)
- ✅ 7 affected stats: Persuasion, Investigation, Espionage, Command, Administration, Science, Security
- ✅ Free mission (no IP/Influence cost)
- ✅ Bonuses automatically disappear when target completes their next mission
- ✅ Resolves immediately (resolutionOrder = 0, resolves first each turn)
- ✅ English localization included

---

## 📋 Version History

| Version | Date | Changes |
|---------|------|---------|
| 0.3.10 | 2026-09-07 | **CURRENT** - Fixed KeyNotFoundException crash in AI mission planner. Created AICouncilorMissionPlanner_GetMissionsForCouncilorPatch to filter Assist mission from AI councilor mission evaluation. Patch intercepts GetMissionsForCouncilor() and removes Assist from list for AI-controlled factions only. Player-controlled factions can still use Assist mission. Added comprehensive logging and error handling. Resolves critical crash when AI factions attempt mission planning. |
| 0.3.9 | 2026-09-06 | Changed mission resolution from Contested to Automatic for guaranteed 100% success rate. Removed dice roll mechanic that was causing 50% success rate. Matches GoToGround and DefendInterests pattern - appropriate for uncontested support missions. Updated context lists to {Context.None, Context.None} matching vanilla pattern. Modifiers now empty lists as required by Automatic resolution. |
| 0.3.8 | 2026-09-06 | Fixed localization file naming from English.en to TIMissionTemplate.en (language code, not file extension). Removed Persuasion stat check - replaced CouncilorAttackStat modifier with neutral FlatModifier(0). Added comprehensive assumptions section documenting condition return values, localization format, context lists, and modifier requirements. Localization now properly integrated. |
| 0.3.7 | 2026-09-05 | Fixed critical condition return value bug preventing valid targets from being found. Created custom TIMissionCondition_MyFactionCouncilor implementation to replace vanilla. Fixed both custom conditions to return plain "_Pass"/"_Fail" constants instead of "ClassName_Pass"/"ClassName_Fail". Mission targeting now correctly validates all conditions and displays valid targets. |
| 0.3.6 | 2026-09-04 | Fixed "no valid targets" error by removing overly restrictive mission conditions. Removed TargetInRange, Human, and FreeCouncilor conditions. Now only requires: MyFactionCouncilor (same faction) and PlayerFactionOnly (player-controlled faction). Allows targeting ANY councilor in player faction regardless of location or mission status. |
| 0.3.5 | 2026-09-04 | Removed broken Harmony patch TIMissionTemplate_CanUseTemplatePatch that targeted non-existent method. Mission loads cleanly without patch errors. TIMissionCondition_PlayerFactionOnly condition properly prevents AI usage via game's native condition evaluation. Mod loading verified in game log: "Assist mission registered. Grants: councilorTypes=26". |
| 0.3.4 | 2026-09-04 | Fixed persistent AICouncilorMissionPlanner KeyNotFoundException crash. Removed ineffective Harmony patch on non-existent method. Created TIMissionTemplate_CanUseTemplatePatch to intercept CanUseTemplate() for Assist mission. Now blocks Assist mission for AI factions before evaluation, preventing dictionary lookup errors. Simplified modifiers (removed ResourceSpent). |
| 0.3.3 | 2026-09-04 | Fixed critical AI mission planner KeyNotFoundException crash (v0.3.2 regression). Changed context lists from Context.None to empty lists in mission template. Added AICouncilorMissionPlanner_GetMissionsForCouncilorPatch to filter Assist mission from AI planning, providing defensive safeguard. Prevents AI from ever evaluating player-only mission. |
| 0.3.2 | 2026-09-03 | Restricted Assist mission to player-controlled factions only. Created TIMissionCondition_PlayerFactionOnly to prevent AI players from triggering this mission. Mission now requires councilor's faction to have a player controller. |
| 0.3.1 | 2026-09-02 | Fixed critical AI mission planner KeyNotFoundException crash. Restructured mission template to exactly match vanilla Inspire mission structure. Added TIMissionModifier_ResourceSpent, defensive modifiers, proper Context lists, and all required conditions (Human, FreeCouncilor). Enhanced modifier inheritance and bootstrap logging. |
| 0.3.0 | 2026-09-01 | Fixed target validation to match Inspire mission exactly. Removed CouncilorOnEarth condition and TargetHasNoMission custom condition. Assist now targets any friendly councilor regardless of mission status. |
| 0.2.0 | 2026-08-31 | Fixed UI crash by switching from TIMissionResolution_Automatic to TIMissionResolution_Contested with proper modifiers. Added custom TIMissionModifier_AssistStat modifier. |
| 0.1.0 | 2026-08-30 | Initial working version. Implemented core assist mission, stat transfer logic, bonus tracking with auto-removal, and English localization. |

---

## 🐛 Debugging & Troubleshooting

### Game Log Location
The primary log file for Terra Invicta is located at:
```
C:\Users\{Username}\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log
```

**Key things to check in the log:**
- Search for `"Assist"` to find mod-related messages
- Look for `"KeyNotFoundException"` or `"NullReferenceException"` if the game crashes
- Check for `"Assist mission registered"` to verify mod loaded successfully
- Bootstrap logging will show mission property details if enabled

### Mod Loading
The mod uses UMM (UnityModManager) to load. Check for:
- Mod is placed in `Mods/Enabled/AssistMission/` folder
- `Assistance.dll` and `English.xml` are present
- `ModInfo.json` is properly formatted
- No conflicts with other mods that modify missions

### Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| Mission doesn't appear | Mod didn't load or mission wasn't registered | Check `Player.log` for load errors; verify UMM is active |
| Game crashes when using Assist | v0.3.0 issue (fixed in v0.3.1) | Update to v0.3.1+ which fixed AI planner crash |
| AI cannot use Assist (v0.3.2+) | By design - player-only mission | This is intended behavior; AI is restricted |
| Bonus not applied | Effect didn't run or assist % is 0% | Check settings; verify mission succeeded |
| Bonus doesn't disappear | Bonus removal patch didn't trigger | Check that target councilor completes next mission |

---

## 📁 Project Structure

```
Assistance/                                 [Solution Root]
├── Assistance/                             [Project Root]
│   ├── TIMissionTemplate_Assist.cs              [Mission definition - CORE FILE]
│   ├── TIMissionEffect_Assist.cs                [Stat transfer logic during mission completion]
│   ├── TIMissionModifier_AssistStat.cs          [Mission resolution modifier for contested resolution]
│   ├── TIMissionModifier_AssistFlat.cs          [Placeholder flat modifier]
│   ├── TIMissionCondition_PlayerFactionOnly.cs  [Custom condition to restrict mission to player factions]
│   ├── AssistBonusTracker.cs                    [Tracks and removes bonuses after mission complete]
│   ├── TICouncilorState_CompleteMissionPatch.cs [Harmony patch to trigger bonus removal]
│   ├── CouncilorMissionCanvasController_UpdateModifierListPatch.cs [UI rendering patch]
│   ├── Main.cs                                  [UMM entry point & GUI settings slider]
│   ├── Settings.cs                              [Configuration storage]
│   ├── AssistMissionBootstrapPatch.cs           [Harmony bootstrap patch for mission registration]
│   ├── English.xml                              [Localization strings]
│   ├── Properties/AssemblyInfo.cs               [Assembly version: 0.3.5]
│   └── bin/Debug/Assistance.dll                 [Compiled mod, ~11 KB]
│
└── Deploy.ps1                              [⭐ Deployment script - run after building]
    Copies mod files to Terra Invicta Mods folder
│   ├── AssistMissionBootstrapPatch.cs           [Harmony bootstrap patch for mission registration]
│   ├── English.xml                              [Localization strings]
│   ├── Properties/AssemblyInfo.cs               [Assembly version: 0.3.4]
│   └── bin/Debug/Assistance.dll                 [Compiled mod, ~11 KB]
│
└── Deploy.ps1                              [⭐ Deployment script - run after building]
    Copies mod files to Terra Invicta Mods folder
```

---

## 🔑 Critical Implementation Details

### Vanilla Mission Template Reference

The Assist mission template is built to match the vanilla **Inspire** mission exactly. Vanilla mission templates are stored in:
- **Game Location:** `C:\Games\Steam\steamapps\common\Terra Invicta\TerraInvicta_Data\StreamingAssets\Templates\TIMissionTemplate.json`

This JSON file contains all vanilla mission definitions. Compare your Assist mission against the Inspire mission in this file to verify compatibility.

### Mission Targeting (Player Faction Only - Minimal Restrictions)

The Assist mission uses **2 conditions** (no location, status, or race restrictions):

```csharp
this.conditions = new List<TIMissionCondition>
{
	new TIMissionCondition_MyFactionCouncilor()   // Target must be same faction as source, not self
	new TIMissionCondition_PlayerFactionOnly()    // Source faction must be player-controlled (AI CANNOT USE)
};
```

**Key Features:**
- The Assist mission can target **ANY councilor in the player's faction**
- No restrictions on location (communication range not required)
- No restrictions on race/species (Human and alien councilors both work)
- No restrictions on mission status (can target councilors already on other missions)
- No restrictions on detention status (can target detained councilors)
- The `PlayerFactionOnly` condition (v0.3.2+) **restricts this mission to player factions only** - AI players cannot use it
  - Checks if `councilor.faction.playerControl != null`
  - If null, faction is AI-controlled and mission is disabled
  - This prevents AI players from "gaming" the assist mechanic

> **Note:** Earlier versions (v0.3.5 and prior) included `TIMissionCondition_FreeCouncilor()`, which does NOT mean "councilors with no mission". It specifically refers to **councilors who are not imprisoned/detained**. This condition was removed in v0.3.6 to allow targeting of detained councilors.

### Resolution Method (Automatic for Guaranteed Success)

Uses `TIMissionResolution_Automatic` with:
- **Attacking Modifiers:** Empty list
- **Defending Modifiers:** Empty list
- **Context Lists:** `{Context.None, Context.None}` for both attacker and defender

**Design Rationale:** Automatic resolution provides guaranteed 100% success rate, appropriate for support missions with no opposition. This matches the pattern used by vanilla support missions (GoToGround, DefendInterests). No dice rolls, no contested mechanics - the mission always succeeds.

**Note:** In v0.3.7, the mission used `TIMissionResolution_Contested` which resulted in 50% success rate due to dice roll mechanics. This was changed in v0.3.9 to use Automatic resolution for reliability.

### Bonus Application & Removal

1. **Application:** When mission succeeds, `TIMissionEffect_Assist.ApplyEffect()` runs:
   - Gets target councilor stats
   - Calculates bonuses as: `sourceValue * (assistPercentage / 100)`
   - Applies bonuses via `ModifyAttribute()`
   - Tracks bonuses in `AssistBonusTracker.RecordBonus()`

2. **Removal:** When target completes ANY mission:
   - `TICouncilorState_CompleteMissionPatch` intercepts `SetCompletedMission()`
   - Calls `AssistBonusTracker.RemoveExpiredBonuses()`
   - Reverts all assist bonuses

---

## 🐛 Fixed Issues (v0.3.1)

| Issue | Root Cause | Solution |
|-------|-----------|----------|
| AI mission planner crash (KeyNotFoundException) | Mission template properties didn't match vanilla Inspire structure. Empty Context lists caused dictionary lookup errors. | Restructured template to match Inspire exactly: added ResourceSpent modifier, defensive modifier, proper Context lists, all 4 conditions |
| Missing attacking modifier type | AI planner specifically looks for TIMissionModifier_CouncilorAttackStat in attackingModifiers | Changed from custom TIMissionModifier_AssistStat only to include TIMissionModifier_CouncilorAttackStat(Persuasion) + ResourceSpent |
| Empty Context lists | AI planner tries to access context-based modifiers for each context in the lists | Changed from empty lists to {Context.None} |
| Missing defensive modifier | Game doesn't handle null defending modifiers well | Added TIMissionModifier_FlatModifier(0) to defendingModifiers |
| Missing conditions | Vanilla missions have complete condition sets | Added TIMissionCondition_Human() and TIMissionCondition_FreeCouncilor() to match Inspire |
| Incorrect cost type | Used TIMissionCost_Flat instead of vanilla pattern | Changed to TIMissionCost_Bonus(FactionResource.None) |

---

## 🔧 Important Code Patterns

### Getting Councilor Stats (7 Stats Used)
```csharp
CouncilorAttribute[] stats = new CouncilorAttribute[] {
	CouncilorAttribute.Persuasion,
	CouncilorAttribute.Investigation,
	CouncilorAttribute.Espionage,
	CouncilorAttribute.Command,
	CouncilorAttribute.Administration,
	CouncilorAttribute.Science,
	CouncilorAttribute.Security
};

foreach (var stat in stats) {
	int value = councilor.GetAttribute(stat, true, true, true, false, false, false);
}
```

### Harmony Patching Entry Point
```csharp
[HarmonyPatch(typeof(SolarSystemBootstrap), "Initialize")]
internal static class AssistMissionBootstrapPatch {
	public static void Postfix() {
		// Mission registration and template granting happens here
	}
}
```

### Bonus Tracking
```csharp
// Record bonus when applied
AssistBonusTracker.RecordBonus(targetCouncilor, stat, assistAmount);

// Remove when mission completes
AssistBonusTracker.RemoveExpiredBonuses(completedCouncilor);
```

---

## 💡 Key Assumptions

These assumptions were validated through implementation and testing:

### 1. Condition Return Value Format
- **Assumption:** Conditions must return plain `TIMissionCondition.pass` / `TIMissionCondition.fail` constants (which equal "_Pass" / "_Fail")
- **Evidence:** Target validation system uses `All()` LINQ to check if ALL condition results equal "_Pass"
- **Impact:** Returning "ClassName_Pass" format breaks target filtering and causes "no valid targets" error

### 2. Localization File Naming Convention
- **Assumption:** Localization files are named `TIMissionTemplate.<languagecode>` (e.g., `.en` for English, not `.xml`)
- **Evidence:** More Realistic Nukes mod uses `TIMissionTemplate.en` format; XML localization does not work
- **Impact:** Using incorrect format (English.xml) prevents localization strings from appearing in-game

### 3. Custom Condition Implementation Required
- **Assumption:** Using vanilla `TIMissionCondition_MyFactionCouncilor` as a class reference requires a custom implementation to be included
- **Evidence:** Vanilla class exists but needs explicit custom implementation to work reliably with custom missions
- **Impact:** Creating a custom TIMissionCondition_MyFactionCouncilor class ensures consistent behavior

### 4. Modifiers for Contested Resolution
- **Assumption:** Contested resolution requires modifiers in both attacking and defending lists (cannot be empty or null)
- **Evidence:** AI mission planner iterates through modifiers; missing modifiers cause crashes
- **Impact:** Using neutral FlatModifier(0) is safe and doesn't affect mission mechanics

### 5. Empty Context Lists for Custom Missions
- **Assumption:** Custom mission templates should use empty context lists rather than {Context.None} or specific contexts
- **Evidence:** Vanilla missions with simple resolution use empty lists; non-empty lists cause AI planner issues
- **Impact:** Empty lists prevent dictionary lookup errors in mission planning

---

## ⚠️ Critical Decisions & Tradeoffs

### 1. Free Mission (vs. Cost)
- **Decision:** No IP/Influence cost (TIMissionCost_Bonus with FactionResource.None)
- **Rationale:** Assist is a support mission; adding cost would limit usefulness
- **Tradeoff:** Potentially overpowered if assist % is too high

### 2. Immediate Resolution (resolutionOrder = 0)
- **Decision:** Mission resolves first each turn (before other missions)
- **Rationale:** Get bonuses to target councilor ASAP to help their active mission
- **Tradeoff:** Could theoretically be gamed for ordering advantage

### 3. Auto-Remove Bonuses on Mission Complete
- **Decision:** Bonuses disappear when target completes ANY mission
- **Rationale:** Prevents permanent stat inflation
- **Tradeoff:** Bonuses only last 1-3 days depending on mission duration

### 4. Neutral Modifiers (Removed Persuasion Check)
- **Decision:** Use neutral FlatModifier(0) instead of CouncilorAttackStat(Persuasion)
- **Rationale:** Mission doesn't need to tie to any specific stat; neutral modifier avoids unwanted UI displays
- **Tradeoff:** Removes the Persuasion association that was cosmetic anyway
- **Update (v0.3.8):** Changed from CouncilorAttackStat to FlatModifier for cleaner implementation

---

## 🚀 Build & Deploy

### Quick One-Step Workflow

**In Visual Studio:**
1. Build the solution: `Ctrl+Shift+B`
2. Output files are automatically copied to the game mod folder

**Files copied automatically after build:**
- `Assistance\bin\Debug\Assistance.dll` → `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\Assistance.dll`
- `Assistance\English.xml` → Game mod folder
- (Settings.xml is created by the game after first run)

### Auto-Copy Setup (Optional)

Add a post-build event to Visual Studio for automatic deployment:

**Project → Properties → Build Events → Post-build event command line:**

```powershell
powershell -Command "Copy-Item -Path '$(ProjectDir)bin\Debug\Assistance.dll' -Destination 'C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\Assistance.dll' -Force; Copy-Item -Path '$(ProjectDir)English.xml' -Destination 'C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\English.xml' -Force"
```

This will automatically copy files to the game folder every time you build.

### Manual Deploy (if Auto-Copy not working)

```powershell
# Copy the compiled DLL
Copy-Item -Path "Assistance\bin\Debug\Assistance.dll" `
  -Destination "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\Assistance.dll" -Force

# Copy localization file
Copy-Item -Path "Assistance\English.xml" `
  -Destination "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\English.xml" -Force
```

### Build Output

```powershell
# In Visual Studio:
Build → Build Solution (Ctrl+Shift+B)
# Output: Assistance\bin\Debug\Assistance.dll (~19 KB)
```

### Required Files in Mod Directory
```
C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\
├── Assistance.dll              [Compiled mod DLL - REQUIRED]
├── English.xml                 [Localization strings - REQUIRED]
├── Settings.xml                [User settings (auto-created)]
└── ModInfo.json                [Mod metadata - by UMM]
```

**Note:** After building/deploying, restart Terra Invicta to load the updated mod.

---

## 🧪 Testing Checklist

- [ ] Game loads without crashes
- [ ] Assist mission appears in councilor mission list
- [ ] Can select Assist mission and target another councilor
- [ ] Bonuses applied correctly to target (check stat increase)
- [ ] Bonuses removed when target completes next mission
- [ ] AI councilors can be assigned Assist missions
- [ ] Settings slider works (0-100% configurable)
- [ ] Assist mission icons/descriptions display correctly

---

## 🔮 Future Enhancement Ideas

1. **Mission Cost:** Add IP or Influence cost for balance
2. **Partial Bonuses:** Option to assist with only certain stats
3. **Duration Scaling:** Longer assist duration with higher helper councilor level
4. **Localization:** Add Chinese, Russian, other language support
5. **AI Tuning:** Improve AI evaluation of assist mission usefulness
6. **Visual Effects:** Custom mission completion illustration

---

## 📞 Debugging Tips

### If Game Crashes at Startup
1. Check `Player.log` for "KeyNotFoundException" or "NullReferenceException"
2. If in AICouncilorMissionPlanner: Mission conditions may be too restrictive
3. If in TIMissionTemplate.get_primaryAttackerStat(): Resolution method missing modifiers

### If Bonuses Don't Apply
1. Check `AssistBonusTracker` is recording bonuses
2. Verify `TIMissionEffect_Assist.ApplyEffect()` is being called
3. Check assist percentage in Settings (default 25%)

### If AI Won't Use Mission
1. Verify mission appears in councilor's `possibleMissionList`
2. Check all conditions pass for target (especially MyFactionCouncilor)
3. Check AICouncilorMissionPlanner can evaluate utility

---

## 📚 References

- **Inspire Mission Template:** `C:\Games\Steam\steamapps\common\Terra Invicta\TerraInvicta_Data\StreamingAssets\Templates\TIMissionTemplate.json` (line ~2509)
- **Game Analysis:** `GameAnalysis/Assembly-CSharp/` folder contains decompiled game code
- **UMM Documentation:** Built into UMM; check mod loading logs

---

## 💾 Git Workflow

```bash
# View changes
git status

# Stage changes
git add .

# Commit with version
git commit -m "v0.3.0: Fix target validation to match Inspire mission"

# Push to origin/master
git push origin master
```

---

**Next AI Developer:** Start here if the mod crashes. Check:
1. Mission conditions (must be TargetInRange + MyFactionCouncilor only)
2. Resolution method (must be Contested with attackingModifiers)
3. Game logs in `C:\Users\{User}\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log`

Good luck! 🚀
