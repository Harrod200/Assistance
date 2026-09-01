# Assistance Mod - AI Developer Handoff Summary

**Version:** 0.5.5  
**Last Updated:** 2026-09-12
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

2. **Increment version in `modinfo.json`**:
   ```json
   "Version": "X.Y.Z"
   ```
   - Must match the version in `AssemblyInfo.cs`
   - Ensures the game mod manager displays the correct version

3. **Update this developer summary**:
   - Update the **Version** at the top to match `AssemblyInfo.cs`
   - Update the **Last Updated** date (format: YYYY-MM-DD)
   - Add new row to **Version History** table at the top:
     ```markdown
     | 0.X.Y | YYYY-MM-DD | Summary of changes made in this version |
     ```
   - Ensure CURRENT marker is on the latest version
   - Update any relevant sections (Project Structure, Critical Details, Known Issues, etc.)

4. **Build and test** the solution to ensure no compilation errors:
   ```powershell
   dotnet build Assistance/Assistance.csproj
   ```

5. **Copy to game folder** when ready for testing:
   ```powershell
   Copy-Item "Assistance\bin\Debug\Assistance.dll" "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force
   Copy-Item "Assistance\modinfo.json" "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force
   ```

### Committing
6. **Stage changes**:
   ```powershell
   git add -A
   ```

7. **Commit with semantic message**:
   ```powershell
   git commit -m "feat/fix/docs: Brief description

   - Detailed bullet point 1
   - Detailed bullet point 2
   - Detailed bullet point 3"
   ```
   - Use prefix: `feat:` (feature), `fix:` (bug fix), `docs:` (documentation), `refactor:` (code restructuring), `perf:` (performance)
   - Include detailed changelog in body
   - Reference version number in commit body if significant

8. **Push to master**:
   ```powershell
   git push origin master
   ```

### Example Workflow
```powershell
# 1. Make code changes
# 2. Update AssemblyInfo.cs version from 0.3.1 to 0.3.2
# 3. Update modinfo.json version from 0.3.1 to 0.3.2
# 4. Update AI_DEVELOPER_SUMMARY.md:
#    - Change Version: 0.3.1 → 0.3.2
#    - Change Last Updated: 2026-09-02 → 2026-09-03
#    - Add row to Version History table
#    - Update relevant documentation sections
# 5. Build
cd "C:\Users\Chris\source\repos\Assistance"
dotnet build Assistance/Assistance.csproj

# 6. Copy to game folder for testing
Copy-Item "Assistance\bin\Debug\Assistance.dll" "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force
Copy-Item "Assistance\modinfo.json" "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force

# 7. Git operations
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

# Copy modinfo.json to game folder (keeps mod version in sync)
Copy-Item "Assistance\modinfo.json" "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\" -Force

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
- ✅ Optional debug logging toggle (disabled by default, toggleable in UMM settings)
- ✅ Control point cap adjustment via faction-level Harmony patch
- ✅ Comprehensive error handling and exception logging

---

## 📋 Version History

| Version | Date | Changes |
|---------|------|---------|
| 0.5.5 | 2026-09-12 | **CURRENT** - Fixed debug logging leak - added missing debugLogging flag checks. AssistBonusTracker.RecordBonus() now respects debugLogging setting when logging bonus records. TICouncilorState_CompleteMissionPatch now respects debugLogging setting when logging bonus removal. Ensures no debug messages appear when debug logging is disabled (default). All logging handlers now consistently check Main.settings.debugLogging before logging. Version bumped to 0.5.5 (PATCH: bug fix for logging flag). All changes verified with successful build and deployed. |
| 0.5.4 | 2026-09-12 | Changed debug logging default to disabled (debugLogging = false). Users can still enable via UMM settings if needed. Reduces log spam for standard gameplay. Hotfix applied for AudioManager VO event errors: added empty voiceEvent entries for Assign/Success/Failure/Aborted in TIMissionTemplate.en to prevent AudioManager from attempting to load non-existent Assist mission VO audio files. No functional changes to mission behavior. All changes verified with successful build and deployed. |
| 0.5.3 | 2026-09-12 | Added debug logging toggle to UMM settings (default enabled). New Settings.debugLogging boolean with GUI toggle in Main.OnGUI(). Updated TIFactionState_ControlPointMaintenanceCapPatch with Prefix patch to log vanilla method entry point details (faction name, alien flag). Postfix logging now conditional on debugLogging flag to reduce spam. Prefix logs every GetControlPointMaintenanceFreebieCap() call. Postfix logs adjustment details and zero-adjustment cases only when debug enabled. Enables detailed tracing of control point cap calculation flow for troubleshooting. All changes verified with successful build and deployed. |
| 0.5.2 | 2026-09-11 | Refactored CP bonus subtraction from per-councilor to faction-level (Option B). Replaced TICouncilorState_ControlPointCapacityPatch with TIFactionState_ControlPointMaintenanceCapPatch. New patch targets GetControlPointMaintenanceFreebieCap() Postfix, applying flat adjustment AFTER all components (global freebies, AI bonus, councilor sum, hab sum, effects modifiers). This avoids per-councilor ledger display changes while ensuring faction CP maintenance reflects assist bonus impact. Added AssistBonusTracker.GetFactionCPAdjustment(faction) to sum CP-affecting bonuses across all faction councilors (Persuasion + Command + Administration only). Simplified TICouncilorState_CompleteMissionPatch: removed per-councilor CP capture/logging, just removes bonus from tracking. Faction-level patch handles recalculation automatically. Advantages: cleaner architecture, no double-counting risk, no activeCouncilors filtering interference, single focused injection point. All changes verified with successful build and deployed. |
| 0.5.1 | 2026-09-11 | Simplified CP capacity patch to use cleaner attribute-based Harmony approach (removed reflection boilerplate). Refactored TICouncilorState_ControlPointCapacityPatch: replaced TargetMethod() reflection with direct [HarmonyPatch(typeof(TICouncilorState), "get_controlPointCapacity")] attribute declaration. Postfix logic unchanged and correct. Enhanced mission completion flow: TICouncilorState_CompleteMissionPatch now explicitly captures CP before/after bonus removal and logs restoration with [CP_UPDATE] messages, forcing recalculation of controlPointCapacity via property getter. Code cleanup: Removed 3 unused using statements (System, System.Text) from TIMissionModifier_AssistStat, TIMissionCondition_MyFactionCouncilor, TIMissionCondition_PlayerFactionOnly. Created CODE_CLEANUP_REVIEW.md documenting additional cleanup opportunities (bare catch blocks, defensive UI code) for future phases. All changes verified with successful build. |
| 0.5.0 | 2026-09-10 | Implemented control point cap bonus exclusion with reflection-based Harmony patching. Initial attempt using [HarmonyPatch] attribute on property getter failed to intercept correctly. Switched to [HarmonyPatch] with custom TargetMethod() that uses reflection to locate controlPointCapacity property getter at runtime. Updated AssistBonusTracker with GetStatBonus() method for per-stat bonus tracking (more accurate than total). Patch now calculates CP-bonus as sum of only CP-affecting stats (Persuasion, Command, Administration) and subtracts from controlPointCapacity. Ensures temporary bonuses don't inflate faction control point maintenance capacity. Removed excessive logging from TIMissionCondition_PlayerFactionOnly (was causing 25+ repeated log entries per turn). Created comprehensive testing guides: TESTING_GUIDE.md, LOG_REFERENCE.md, VERIFICATION_QUICK.md. Added LOG_ANALYSIS.md documenting bonus tracking works perfectly (45-80 point totals confirmed), CP patch interception fixed with reflection approach. |
| 0.4.2 | 2026-09-10 | Removed verbose debug logging from TICouncilorState_GetPossibleMissionListPatch to reduce log spam. Simplified filtering logic while maintaining core functionality. Mission still properly filtered from AI faction councilors. |
| 0.4.0 | 2026-09-09 | Fixed compiler warning CS0108 in TIMissionModifier_AssistStat. Added 'new' keyword to attackerAttribute field override to explicitly indicate intentional field shadowing. Game loads and runs successfully with no warnings or errors. Clean build achieved. |
| 0.3.12 | 2026-09-08 | Fixed persistent KeyNotFoundException crash in AI mission planner. Initial approach (AICouncilorMissionPlanner_GetMissionsForCouncilorPatch targeting non-existent method) was incorrect. Root cause: Assist mission added to ALL councilor types, but AI planner evaluates modifiers before checking conditions, causing crash on empty modifier lists. Solution: Created TIFactionState_GetAllPossibleMissionsPatch to filter Assist mission from AI factions at mission retrieval stage (before evaluation). This prevents AI planner from ever seeing the mission. Player factions unaffected. |
| 0.3.10 | 2026-09-07 | Fixed KeyNotFoundException crash in AI mission planner. Created AICouncilorMissionPlanner_GetMissionsForCouncilorPatch to filter Assist mission from AI councilor mission evaluation. Patch intercepts GetMissionsForCouncilor() and removes Assist from list for AI-controlled factions only. Player-controlled factions can still use Assist mission. Added comprehensive logging and error handling. Resolves critical crash when AI factions attempt mission planning. |
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
│   ├── TIMissionCondition_MyFactionCouncilor.cs [Custom condition - target must be same faction, not self]
│   ├── TIMissionCondition_PlayerFactionOnly.cs  [Custom condition to restrict mission to player factions]
│   ├── AssistBonusTracker.cs                    [Tracks and removes bonuses after mission complete, faction-level adjustment]
│   ├── TICouncilorState_CompleteMissionPatch.cs [Harmony patch - remove bonus from tracking on mission complete]
│   ├── TIFactionState_ControlPointMaintenanceCapPatch.cs [⭐ NEW - Faction-level CP cap adjustment (v0.5.2)]
│   ├── TICouncilorState_GetPossibleMissionListPatch.cs [Filter Assist mission from AI councilors]
│   ├── CouncilorMissionCanvasController_UpdateModifierListPatch.cs [UI rendering patch for localization safety]
│   ├── AssistMissionBootstrapPatch.cs           [Harmony bootstrap patch for mission registration]
│   ├── Main.cs                                  [UMM entry point & GUI settings slider]
│   ├── Settings.cs                              [Configuration storage]
│   ├── TIMissionTemplate.en                     [Localization strings (English)]
│   ├── ModInfo.json                             [UMM mod metadata (v0.5.2)]
│   ├── Properties/AssemblyInfo.cs               [Assembly version: 0.5.2]
│   └── bin/Debug/Assistance.dll                 [Compiled mod, ~12 KB]
│
├── CODE_CLEANUP_REVIEW.md                   [Code quality audit with cleanup recommendations]
├── AI_DEVELOPER_SUMMARY.md                  [This file - comprehensive handoff documentation]
│
└── GameAnalysis/                            [Decompiled Terra Invicta assembly (reference only)]
    └── Assembly-CSharp/                     [Game source code analysis]
```

---

## 🏗️ Architecture: Bonus Tracking & CP Adjustment (v0.5.2)

### Overview

The Assist mission grants temporary stat bonuses that must be excluded from faction control point maintenance calculations. The architecture uses **two levels of operation**:

1. **Per-Councilor Tracking** (AssistBonusTracker)
   - Records which councilors received which stat bonuses
   - Removes bonuses when councilor completes their mission

2. **Faction-Level Adjustment** (TIFactionState_ControlPointMaintenanceCapPatch)
   - Calculates total CP impact of all active bonuses in a faction
   - Applies flat adjustment to `GetControlPointMaintenanceFreebieCap()` result
   - Ensures faction CP maintenance reflects bonus impact

### How It Works

```
MISSION GRANTED (TIMissionEffect_Assist)
  ├─ Transfer 0-100% of source councilor's stats to target
  ├─ Record bonuses by stat in AssistBonusTracker
  │  ├─ Target.Persuasion += (Source.Persuasion * 25%)
  │  ├─ Target.Command += (Source.Command * 25%)
  │  ├─ Target.Administration += (Source.Administration * 25%)
  │  └─ ... (other stats tracked but don't affect CP)
  └─ [Bonuses now active, faction CP cap reduced]

FACTION CP CALCULATION (GetControlPointMaintenanceFreebieCap)
  ├─ Sum: Global freebies + AI bonus + councilor sum + hab sum
  ├─ Subtract: Effects modifiers
  ├─ [TIFactionState_ControlPointMaintenanceCapPatch POSTFIX RUNS HERE]
  │  ├─ Calculate total CP bonus for faction:
  │  │  └─ Sum (Persuasion_bonus + Command_bonus + Admin_bonus) for all councilors
  │  └─ Subtract from result: __result -= factionCPBonus
  └─ Return: Final CP maintenance cap (reduced by bonuses)

UI LEDGER DISPLAY (Per-Councilor)
  └─ Shows actual councilor stats (bonuses are applied)
     [UI unchanged - displays correct councilor CP value]

FACTION LEDGER DISPLAY (Faction Level)
  └─ Shows faction CP maintenance (reduced by bonus adjustment)
     [Reflects reduced capacity due to assist bonuses]

MISSION COMPLETED (TICouncilorState_CompleteMissionPatch)
  ├─ Remove bonuses from target councilor
  │  └─ AssistBonusTracker.RemoveBonuses(councilor)
  ├─ Bonus tracking cleared
  └─ Next GetControlPointMaintenanceFreebieCap() call:
     └─ No bonuses to subtract, faction cap returns to full value
```

### Code Flow Details

**1. Bonus Application** (Resolved mission)
```csharp
// TIMissionEffect_Assist.ApplyEffect()
foreach (stat in [Persuasion, Investigation, Espionage, Command, Administration, Science, Security])
{
    assistAmount = Math.Max(1, Floor(sourceCouncilor[stat] * 0.25));  // 25% default
    targetCouncilor.ModifyAttribute(stat, assistAmount);              // Apply bonus
    AssistBonusTracker.RecordBonus(targetCouncilor, stat, assistAmount); // Track it
}
```

**2. Bonus Tracking** (AssistBonusTracker)
```csharp
private static Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>> trackedBonuses;
private static Dictionary<TICouncilorState, int> totalBonusAmounts;

// GetFactionCPAdjustment(faction) - called by patch
foreach (councilor in faction.activeCouncilors)
{
    cpBonus = GetStatBonus(councilor, Persuasion)
            + GetStatBonus(councilor, Command)
            + GetStatBonus(councilor, Administration);
    totalAdjustment += cpBonus;
}
return totalAdjustment;  // Sum of all CP-affecting bonuses in faction
```

**3. Faction CP Adjustment** (New Postfix Patch)
```csharp
// TIFactionState_ControlPointMaintenanceCapPatch.Postfix()
if (!faction.IsAlienFaction)  // Skip alien factions (different mechanics)
{
    int factionCPAdjustment = AssistBonusTracker.GetFactionCPAdjustment(faction);
    __result -= factionCPAdjustment;  // Subtract from calculated cap
}
```

**4. Bonus Removal** (Mission completion)
```csharp
// TICouncilorState_CompleteMissionPatch.Postfix()
AssistBonusTracker.RemoveBonuses(__instance);
// Removes bonuses from attributes and clears tracking
// Next faction CP calculation will see no bonuses, cap restored to full
```

### CP-Affecting Stats

Only these three stats impact control point capacity:
- **Persuasion** - Affects councilor CP value (+1 Persuasion ≈ +0.25 CP)
- **Command** - Affects councilor CP value (+1 Command ≈ +0.25 CP)
- **Administration** - Affects councilor CP value (+1 Administration ≈ +0.25 CP)

Other stats (Investigation, Espionage, Science, Security) are bonused but don't affect CP.

### Faction Cap Formula (Post-Patch)

```
Faction CP Cap = [
    100 (global base freebies)
    + AI_bonus (if AI faction)
    + SUM(councilor.controlPointCapacity for each active councilor)
    + SUM(hab.controlPointCapacityValue for each sector)
    - effects_modifiers
] - ASSIST_BONUS_ADJUSTMENT

Where:
  ASSIST_BONUS_ADJUSTMENT = SUM(
      (councilor.persuasion_bonus + councilor.command_bonus + councilor.admin_bonus)
      for each councilor in faction
  )
```

---

## 📚 Lessons Learned (v0.3.10-0.3.12 Debugging)

### 1. **Patching at the Wrong Level Causes Silent Failures**
- **Lesson:** When creating Harmony patches, always verify the target method exists in the decompiled game assembly before implementing
- **What Happened:** Created `AICouncilorMissionPlanner_GetMissionsForCouncilorPatch` targeting method "GetMissionsForCouncilor" which doesn't exist. Patch silently failed during load, but mod version still showed as v0.2.0 instead of crashing with explicit error
- **Solution:** Use decompiled game code (GameAnalysis folder) to verify method signatures, names, and namespaces BEFORE writing patches
- **Takeaway:** Test patch attribute strings against actual decompiled methods first

### 2. **Mission Conditions Are NOT Sufficient to Prevent Evaluation**
- **Lesson:** AI mission planner evaluates mission properties (including modifiers) BEFORE thoroughly checking mission conditions
- **What Happened:** Although `TIMissionCondition_PlayerFactionOnly` correctly returned `fail` for AI factions, the AI planner still tried to access the mission's empty modifier lists, causing KeyNotFoundException
- **Root Cause:** The mission was in the councilor type's `missionNames` array, so it appeared in `GetAllPossibleMissions()` result. AI planner fetches the mission template and starts evaluating payoff/modifiers before checking conditions
- **Solution:** Filter missions at the RETRIEVAL stage (in `GetAllPossibleMissions()`) rather than at the condition-checking stage
- **Takeaway:** For missions that can't be properly evaluated by AI (empty modifiers, etc.), prevent them from appearing in the available mission list entirely

### 3. **Empty Modifier Lists Cause Dictionary KeyNotFoundExceptions**
- **Lesson:** The AI mission planner maintains dictionaries of modifiers by mission. If a mission has empty modifier lists, lookups fail when the planner tries to access payoff calculations
- **What Happened:** `TIMissionTemplate_Assist` has `attackingModifiers = new List<TIMissionModifier>()` and `defendingModifiers = new List<TIMissionModifier>()` for the Automatic resolution type. When AI planner tried to evaluate, it crashed with "Key not in dictionary"
- **Why This Happened:** v0.3.9 changed from Contested to Automatic resolution. Automatic has no modifier evaluation, but AI planner doesn't know about this mission type
- **Solution:** Either (1) populate modifier lists (not applicable here), or (2) prevent AI from seeing/evaluating the mission at all
- **Takeaway:** Custom missions with non-standard resolution types need special AI handling

### 4. **The Right Interception Point Matters**
- **Lesson:** There are multiple places to filter missions: bootstrap registration, condition checking, mission retrieval, and during evaluation. Each has different tradeoffs
- **Wrong Approach:** Filter during bootstrap `GrantToAllCouncilors()` - but this requires identifying AI faction types, which don't have faction information at template level
- **Wrong Approach:** Rely on mission conditions - they're checked too late in the evaluation pipeline
- **Correct Approach:** Patch `TIFactionState.GetAllPossibleMissions()` - this is where missions are retrieved FOR a specific faction instance (which has faction.playerControl information)
- **Key Insight:** Patch at the level closest to where the decision can be made with full context (the faction instance)
- **Takeaway:** Before writing a patch, trace the execution path to find the earliest point where you have the necessary context to make the decision

### 5. **Debugging Game Mods Requires Multiple Tools**
- **Lesson:** Fixing a silent mod load failure requires: (1) Player.log inspection, (2) decompiled game assembly analysis, (3) mod source code review, (4) iterative rebuild/test cycles
- **Process:** 
  - Player.log showed Harmony exception and v0.2.0 load failure
  - Decompiled assembly revealed actual method names and locations
  - Source code review showed multiple potential interception points
  - Build/test revealed which approach was correct
- **Takeaway:** When a patch fails silently, check logs first, then verify target method names in decompiled game code

### 6. **Semantic Versioning and Release Notes Matter**
- **Lesson:** Clear version history and release notes help track which fix was attempted and when it was reverted
- **What Happened:** v0.3.10 (broken approach), v0.3.11 (incomplete fix), v0.3.12 (correct fix). Having each documented helped understand the problem evolution
- **Takeaway:** Always document failed attempts in version history with specific reasons - it prevents re-trying the same broken approach

### 7. **PlayerControl Property Semantics Clarified**
- **Lesson:** `faction.playerControl` returns a Player component for ALL factions (both player and AI), but the Player component has an `isAI` property that indicates actual control
- **Discovery:** Initial assumption that `playerControl != null` distinguishes player vs AI was incorrect. The correct check is `faction.player.isAI` (game uses `faction.player`, not `faction.playerControl`)
- **Root Cause:** `playerControl` is a cached reference to the Player component. It's non-null for all factions. The distinction is made via `TIPlayerState.isAI` property
- **Solution:** Updated `TIMissionCondition_PlayerFactionOnly` to check `!faction.player.isAI` instead of `faction.playerControl != null`
- **Decompiled Evidence:** Game's own `TIFactionCondition_bAIControlled` condition uses exactly this pattern: `state.ref_faction.player.isAI`
- **Takeaway:** Always examine decompiled game code for similar conditions to understand the correct pattern

### 8. **Mission Registration and Availability Are Different Operations**
- **Lesson:** Registering a mission template with `TemplateManager.Add()` does NOT automatically make it available to councilors. You must ALSO grant it to councilor types
- **Discovery (v0.4.0+):** Mission was registered but never appeared for player councilors. Investigation revealed: (1) Template registration succeeded, (2) But `GrantToAllCouncilors()` was disabled with a comment about AI planner crashes, (3) Without granting to councilor types, mission never appeared in available missions list
- **Root Cause:** Missions must be explicitly added to each `TICouncilorTypeTemplate.missionNames` array. Mission conditions like `PlayerFactionOnly` filter which councilors CAN USE the mission, but they don't make it AVAILABLE at all
- **Key Distinction:** 
  - **Registration:** `TemplateManager.Add()` - makes mission template exist in the game
  - **Granting:** Add mission name to `councilType.missionNames` - makes it available to that councilor type
  - **Conditions:** `TIMissionCondition_*` - restricts WHO can use the mission once available
  - **Filtering:** Harmony patches - prevent mission from appearing in automatic AI evaluation pipelines
- **Current Status (v0.4.0+):** `GrantToAllCouncilors()` re-enabled but `TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true)` returns 0 results at `SolarSystemBootstrap.Initialize()` time. This suggests councilor type templates haven't been loaded yet at this bootstrap phase
- **Takeaway:** Understand the full lifecycle: Registration -> Granting -> Conditions -> Filtering. Each step is necessary and serves a different purpose

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
  - Currently implemented as: `if (councilor.faction.player != null && councilor.faction.player.isAI) return fail;`
  - Checks the `isAI` property of the faction's associated `TIPlayerState` object
  - Matches the game's own `TIFactionCondition_bAIControlled` pattern for detecting AI-controlled factions
  - This prevents AI players from using the Assist mission

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

## 🧪 Testing Objectives - v0.5.2

### Primary Focus: Faction-Level CP Cap Adjustment

**Test Date:** 2026-09-11  
**Architecture Change:**
- Moved from per-councilor CP reduction (v0.5.1) to faction-level cap adjustment (v0.5.2)
- Bonus impact now applied to `GetControlPointMaintenanceFreebieCap()` instead of individual `controlPointCapacity`
- Per-councilor ledger unchanged; faction maintenance cap reflects bonus adjustment
- Eliminates double-counting and activeCouncilors filtering issues

### Test Scenarios

#### 1. **Faction CP Maintenance Reflects Assist Bonuses** ✅
**Objective:** Verify that faction CP maintenance cap is reduced by assist bonus amount

**Setup:**
- Player-controlled faction with 2 councilors (A and B)
- Councilor A: Persuasion=40, Command=35, Administration=30 (sum affects CP: +26 per point ≈ +26 CP when base)
- Councilor B: Starting CP capacity (get from Nations screen)
- Expected bonus: 25% of A's stats = ~+10 to +12 CP impact

**Test Steps:**
1. Open Nations screen, record faction CP Maintenance Cap (before assist)
2. Use Assist Mission: A → B (25% default bonus)
3. Navigate away and back to Nations screen
4. **VERIFY:** CP Maintenance Cap REDUCED by approximately the bonus amount
5. Check log: Should see `[CP_CAP_PATCH] Faction '...': Original Cap=X, Assist Bonus Adjustment=-Y, Adjusted Cap=Z`
6. B completes their mission
7. Navigate away and back to Nations screen
8. **VERIFY:** CP Maintenance Cap returns to original value

**Expected Logs:**
```
[CP_CAP_PATCH] Faction 'Earth Government': Original Cap=450, Assist Bonus Adjustment=-35, Adjusted Cap=415
[AssistMission] Removed bonuses for 'Smith' on mission complete
[CP_CAP_PATCH] Faction 'Earth Government': Original Cap=450, Assist Bonus Adjustment=-0, Adjusted Cap=450
```

**Important:** Per-councilor ledger should still show correct individual CP values (unchanged)

---

#### 2. **Per-Councilor Ledger Unaffected** ✅
**Objective:** Verify that per-councilor CP display shows actual councilor CP, not reduced value

**Setup:**
- Councilor A before assist
- Councilor B before assist
- Record individual CP values from councilor detail screen

**Test Steps:**
1. Apply Assist A → B
2. View B's detail screen → CP value should still show ACTUAL value (bonuses applied to stats, not hidden)
3. Compare to pre-assist value (may have changed due to bonus-affected stats)
4. View ledger for B: Should show normal CP value (not reduced)
5. **VERIFY:** No UI shows "adjusted" or "reduced" CP per councilor

**Expected Behavior:**
- B's stats increased (Persuasion, Command, Administration)
- B's CP reflects the increased stats normally
- Faction CP cap reduced (happens at faction level, not councilor level)

---

#### 3. **Multiple Assist Missions Stack Correctly** ✅
**Objective:** Verify that multiple overlapping assist bonuses all count in faction cap reduction

**Setup:**
- Faction with 3+ councilors
- Councilor A: High base stats
- Councilors B, C, D: Target for assist

**Test Steps:**
1. Record initial faction CP cap
2. Assist A → B (expect ~35 CP reduction)
3. Record faction CP cap (should be reduced by ~35)
4. Assist A → C (expect another ~35 CP reduction)
5. Record faction CP cap (should be reduced by ~70 total)
6. Assist A → D (expect another ~35 CP reduction)
7. Record faction CP cap (should be reduced by ~105 total)
8. B completes mission
9. Record faction CP cap (should be reduced by ~70 now, B's bonus gone)
10. C completes mission
11. Record faction CP cap (should be reduced by ~35 now, C's bonus gone)
12. D completes mission
13. Record faction CP cap (should return to original, all bonuses gone)

**Expected Behavior:**
- Each bonus independently tracked
- Faction cap reduction is sum of all active bonuses
- Bonuses removed independently as councilors complete missions
- No interaction between simultaneous bonuses

---

#### 4. **Alien Faction Unaffected** ✅
**Objective:** Verify that alien factions are skipped by the patch

**Setup:**
- Game with alien faction (if testable)
- Or examine patch code for alien faction guard

**Test Steps:**
1. Verify alien factions return 20000f from GetControlPointMaintenanceFreebieCap()
2. Check patch: `if (__instance.IsAlienFaction) return;` prevents modification
3. **VERIFY:** No CP_CAP_PATCH logs for alien factions

**Expected Behavior:**
- Alien faction CP cap unchanged by assist bonuses
- Guard logic prevents patch execution

---

#### 5. **Bonus Removal on Mission Complete** ✅
**Objective:** Verify that GetFactionCPAdjustment() correctly identifies removed bonuses

**Setup:**
- Active assist bonuses in faction
- Tracking data shows bonuses in AssistBonusTracker

**Test Steps:**
1. Inspect AssistBonusTracker.totalBonusAmounts (in logs or via debug)
2. Verify council with active bonus appears in tracking
3. Council completes mission
4. Inspect AssistBonusTracker.totalBonusAmounts again
5. **VERIFY:** Council removed from tracking dict
6. **VERIFY:** Next GetFactionCPAdjustment() returns lower value

**Expected Behavior:**
- RemoveBonuses() clears tracking for that councilor
- GetFactionCPAdjustment() stops counting that councilor's bonuses
- Faction cap recalculation accurate

---

#### 6. **Edge Cases**
**Objective:** Verify robustness in unusual scenarios

**Test Scenarios:**

**a) Rapid multiple mission completions**
- Give councilors A, B, C overlapping assist bonuses
- Complete all 3 missions rapidly
- Verify each bonus removal updates faction cap independently

**b) Mission completion without assist**
- Complete normal mission (no assist involved)
- Verify no [CP_CAP_PATCH] log (no bonus to adjust)

**c) Assist to councilor with zero CP**
- Apply assist to councilor with 0 CP capacity
- Verify faction cap still reduced by bonus amount
- Verify cap doesn't go negative (clamped if needed)

**d) AI factions**
- Verify Assist mission doesn't appear in AI councilor lists
- Verify bonus tracking ignores AI faction councilors
- Verify GetFactionCPAdjustment returns 0 for AI factions (no active bonuses)

---

### Monitoring & Logging

**Key Log Patterns to Watch:**

```
✅ SUCCESS patterns:
[CP_CAP_PATCH] Faction 'Name': Original Cap=X, Assist Bonus Adjustment=-Y, Adjusted Cap=Z
[AssistMission] Removed bonuses for 'CouncilorName' on mission complete

⚠️ WARNING patterns (investigate):
No [CP_CAP_PATCH] logs when bonus applied
GetFactionCPAdjustment returning unexpected values

❌ ERROR patterns (blocking):
Exception in GetFactionCPAdjustment
Negative CP cap values
IsAlienFaction null reference
```

---

### Success Criteria

✅ Faction CP maintenance cap reduces when bonuses applied  
✅ Reduction equals sum of CP-affecting bonuses (Persuasion + Command + Administration)  
✅ Per-councilor ledger unchanged (displays actual councilor CP)  
✅ Multiple simultaneous bonuses stack correctly  
✅ Bonuses removed independently as missions complete  
✅ Alien factions unaffected (return 20000f unchanged)  
✅ Logs show correct before/after cap values  
✅ No crashes or UI errors  

---

## 🎯 v0.5.2 Architecture vs v0.5.1 Comparison

| Aspect | v0.5.1 (Per-Councilor) | v0.5.2 (Faction-Level) |
|--------|------|---------|
| **Patch Target** | `TICouncilorState.get_controlPointCapacity` | `TIFactionState.GetControlPointMaintenanceFreebieCap()` |
| **Patch Type** | Postfix on getter | Postfix on calculation method |
| **Adjustment Scope** | Per-councilor | Faction-wide flat adjustment |
| **Ledger Display** | Shows reduced CP per councilor | Shows actual CP per councilor, reduced faction cap |
| **Double-Count Risk** | Higher (getter called multiple times) | Minimal (single calculation point) |
| **activeCouncilors Filtering** | Can interact with filtering logic | No interaction (already summed) |
| **Calculation Order** | During councilor lookup | After all components calculated |
| **Alien Faction Guard** | In getter logic | In postfix condition |
| **Code Complexity** | Higher (must match v0.5.0 approach) | Lower (focused single patch) |
| **Maintainability** | Harder to verify correct context | Easier to understand flat adjustment |

---

## 🎯 v0.5.2 Quick Reference - What Changed

**Major Architecture Change:**

| Component | v0.5.1 | v0.5.2 | Status |
|-----------|--------|--------|--------|
| CP Adjustment Location | Per-councilor getter | Faction-level cap | ✅ Refactored |
| Patch File | TICouncilorState_ControlPointCapacityPatch.cs | TIFactionState_ControlPointMaintenanceCapPatch.cs | ✅ Replaced |
| Mission Completion | CP before/after capture + logging | Simple bonus removal | ✅ Simplified |
| Bonus Calculation | GetStatBonus() per councilor | GetFactionCPAdjustment() per faction | ✅ Elevated |

**Code Changes:**
- ✅ **ADDED:** `TIFactionState_ControlPointMaintenanceCapPatch.cs` (new faction-level patch)
- ✅ **REMOVED:** `TICouncilorState_ControlPointCapacityPatch.cs` (old per-councilor patch)
- ✅ **ENHANCED:** `AssistBonusTracker.cs` - added `GetFactionCPAdjustment(faction)` method
- ✅ **SIMPLIFIED:** `TICouncilorState_CompleteMissionPatch.cs` - removed CP logging

**Testing Checklist (v0.5.2):**
- [ ] Faction CP maintenance cap reduces when bonus applied
- [ ] Reduction = sum of (Persuasion_bonus + Command_bonus + Admin_bonus)
- [ ] Per-councilor ledger shows actual CP (unchanged)
- [ ] Multiple assist missions stack correctly
- [ ] Each mission completion independently removes bonus
- [ ] Faction cap returns to full value when all bonuses gone
- [ ] AI factions unaffected (20000f baseline)
- [ ] No crashes or UI errors
- [ ] Check logs: `[CP_CAP_PATCH]` and `[AssistMission]` messages

**Key Log Messages to Monitor:**
```
✅ Expected:
[CP_CAP_PATCH] Faction 'Earth Government': Original Cap=450, Assist Bonus Adjustment=-35, Adjusted Cap=415
[AssistMission] Removed bonuses for 'CouncilorName' on mission complete

❌ Problematic:
No [CP_CAP_PATCH] logs
GetFactionCPAdjustment returning 0 when bonuses active
Exception in patch execution
```

**If Issues Occur:**
1. Check mod loads: Search Player.log for mod name and version
2. Verify patch applied: Look for `[CP_CAP_PATCH]` messages in first few seconds
3. Check bonus tracking: `[AssistMission]` logs should appear when mission completes
4. Verify faction selection: Test with player faction (not AI)
5. Clear cache: `Remove-Item "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\*.cache" -Force`
6. Restart game

**See Also:**
- CODE_CLEANUP_REVIEW.md - Code quality audit
- TESTING_GUIDE.md - Comprehensive testing methodology
- LOG_ANALYSIS.md - Example log analysis

---

**Next AI Developer:** If issues arise, focus on:
1. **Architecture:** Faction-level cap adjustment (not per-councilor)
2. **Bonus Tracking:** AssistBonusTracker tracks all active bonuses by faction
3. **Patch Point:** GetControlPointMaintenanceFreebieCap() - late in calculation chain
4. **Guard Condition:** Skip alien factions (return 20000f)

Good luck! 🚀
## 🔍 Debugging Tips - v0.5.2

### If CP Cap Doesn't Reduce When Bonus Applied
1. Check `AssistBonusTracker.GetFactionCPAdjustment(faction)` returns > 0
2. Verify `TIFactionState_ControlPointMaintenanceCapPatch` is loaded (check mod logs)
3. Verify councilor with bonus is in faction.activeCouncilors
4. Check patch targets correct method: `GetControlPointMaintenanceFreebieCap()`

### If CP Cap Reduces But Doesn't Return to Full Value
1. Check `AssistBonusTracker.RemoveBonuses()` is called on mission complete
2. Verify bonus tracking dict is cleared (no lingering entries)
3. Verify next `GetFactionCPAdjustment()` call returns 0
4. Check for stale bonus entries from other councilors

### If Alien Faction Cap Changed
1. Verify `if (__instance.IsAlienFaction) return;` guard exists
2. Alien factions should return 20000f baseline, patch should not modify
3. Check logs for unexpected [CP_CAP_PATCH] entries with alien faction name


2. Check ledger for B: CP should be REDUCED (bonus subtracted from capacity)
3. Wait for B to complete their active mission (any mission)
4. **VERIFY:** 
   - CP value returns to original before-assist value
   - Log shows `[CP_UPDATE]` message with before/after values
   - Game doesn't crash or show UI errors

**Expected Logs:**
```
[CP_PATCH] Councilor 'B': Original CP=50, CP Bonuses=12, Adjusted CP=38
[CP_UPDATE] Mission complete for 'B': CP restored from 38 to 50
```

---

#### 2. **Mission Completion Triggers CP Review**
**Objective:** Verify that multiple mission completions correctly reset CP each time

**Setup:**
- Player-controlled faction with 3+ councilors
- Councilor A: Static high stats (for consistent bonuses)
- Councilors B, C, D: Different CP starting values

**Test Steps:**
1. Assist A → B
2. B completes mission → Check CP restored (log should show delta)
3. Assist A → C
4. C completes mission → Check CP restored (log should show delta)
5. Assist A → D (multiple times if possible)
6. D completes mission → Check CP restored

**Expected Behavior:**
- Each mission completion triggers independent CP recalculation
- Logs show correct before/after for each councilor
- No orphaned bonuses remain in AssistBonusTracker

---

#### 3. **Bonus Tracking Accuracy**
**Objective:** Verify that per-stat bonuses are correctly tracked and removed

**Setup:**
- Councilor A with Persuasion=30, Command=25, Administration=20
- Councilor B to receive assist (25% bonus)
- Expected bonuses: +8 Persuasion, +6 Command, +5 Administration = 19 total CP reduction

**Test Steps:**
1. Record A's stats in detail
2. Use Assist A → B
3. Check B's ledger: Should show 19 CP reduction (8+6+5)
4. B completes mission
5. Verify CP log shows correct delta calculation

**Expected Behavior:**
- CP reduction = sum of (Persuasion_bonus + Command_bonus + Admin_bonus)
- Log message includes all three components
- Bonus tracker correctly identifies which stats contributed

---

#### 4. **Code Cleanup - No Regressions**
**Objective:** Verify that unused using statement removal didn't break anything

**Setup:**
- Fresh game load with v0.5.1 mod
- Normal assist mission usage (multiple scenarios)

**Test Steps:**
1. Create assist mission (verify mission appears in available list)
2. Use assist mission (verify bonuses apply correctly)
3. Complete mission (verify cleanup works)
4. Check logs for ANY compilation warnings or errors
5. Verify UI doesn't show localization or display issues

**Expected Behavior:**
- Zero mod-related errors in logs
- All features work identically to v0.5.0
- No new warnings in VS output

---

#### 5. **Faction CP Maintenance Calculation**
**Objective:** Verify that faction-level CP calculations don't include assist bonuses

**Setup:**
- Player faction with 2-3 councilors
- Check faction CP maintenance costs BEFORE assist mission
- Apply assist mission
- Check faction CP maintenance costs AFTER assist mission

**Test Steps:**
1. Open Nations screen, view faction details
2. Note Total CP Capacity before assist
3. Apply Assist A → B (25% bonus)
4. Navigate away and back to Nations screen
5. Verify CP Capacity shows REDUCED by bonus amount
6. Complete B's mission
7. Verify CP Capacity returns to original value

**Expected Behavior:**
- Faction CP maintenance reflects REDUCED capacity when bonuses active
- After bonus removal, faction CP returns to full value
- UI ledger values match internal calculation

---

#### 6. **Edge Cases**
**Objective:** Verify robustness in unusual scenarios

**Test Scenarios:**

**a) Rapid mission completions**
- Give councilor A 3 overlapping assist bonuses
- Complete mission quickly
- Verify all 3 bonuses are removed correctly

**b) Mission completion without assist**
- Complete normal mission (no assist involved)
- Verify [CP_UPDATE] log doesn't appear (no bonus to restore)

**c) Assist to councilor with zero CP**
- Apply assist to councilor with 0 CP capacity
- Verify CP doesn't go negative (clamped to 0 minimum)
- Verify bonus is still tracked for removal

**d) AI vs Player factions**
- Load game with both player and AI factions
- Verify Assist mission doesn't appear in AI faction mission lists
- Verify player faction can use Assist normally

---

### Monitoring & Logging

**Key Log Patterns to Watch:**

```
✅ SUCCESS patterns:
[CP_PATCH] Councilor 'Name': Original CP=X, CP Bonuses=Y, Adjusted CP=Z
[CP_UPDATE] Mission complete for 'Name': CP restored from X to Y

⚠️ WARNING patterns (investigate):
[CP_PATCH] Postfix error: 
[AssistBonusTracker] Error
[CP_UPDATE] ... CP restored from X to X  (no change = bonus not removed)

❌ ERROR patterns (blocking):
KeyNotFoundException
NullReferenceException
ArithmeticException (negative CP)
```

---

### Success Criteria

✅ All missions complete without crashes  
✅ CP values update correctly each mission completion  
✅ Logs show consistent before/after CP deltas  
✅ No new compilation warnings  
✅ Unused using statement removal causes zero regressions  
✅ Attribute-based patching works identically to v0.5.0 reflection approach  

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

---

## 🎯 v0.5.1 Quick Reference - What Changed

**Code Changes:**
| File | Change | Impact |
|------|--------|--------|
| TICouncilorState_ControlPointCapacityPatch.cs | Removed TargetMethod() reflection, direct [HarmonyPatch] attribute | Cleaner code, same functionality |
| TICouncilorState_CompleteMissionPatch.cs | Added CP before/after capture + logging | Force CP recalculation, better transparency |
| TIMissionModifier_AssistStat.cs | Removed unused `using System.Text;` | Code cleanup |
| TIMissionCondition_MyFactionCouncilor.cs | Removed unused `using System;` | Code cleanup |
| TIMissionCondition_PlayerFactionOnly.cs | Removed unused `using System;` | Code cleanup |

**Testing Checklist:**
- [ ] Assist mission applies bonuses correctly
- [ ] CP capacity reduces when bonus applied (ledger shows delta)
- [ ] Mission completion removes bonus (log shows [CP_UPDATE] message)
- [ ] CP value returns to pre-assist level
- [ ] No crashes or UI errors
- [ ] Multiple assist missions work independently
- [ ] AI factions cannot use Assist mission
- [ ] Check Player.log for errors (search: "AssistMission", "CP_", "Exception")

**Key Log Messages to Monitor:**
```
[CP_PATCH] Councilor 'Name': Original CP=X, CP Bonuses=Y, Adjusted CP=Z
[CP_UPDATE] Mission complete for 'Name': CP restored from X to Y
```

**If Issues Occur:**
1. Check `Player.log` for error messages
2. Verify ModInfo.json version matches game mod folder (v0.5.1)
3. Clear cache: `Remove-Item "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\*.cache" -Force`
4. Restart game
5. Check logs again

**See Also:**
- CODE_CLEANUP_REVIEW.md - Additional cleanup opportunities for future versions
- TESTING_GUIDE.md - Comprehensive testing methodology
- LOG_ANALYSIS.md - Example log analysis from v0.5.0 testing
- VO_ERROR_FIX.md - Voice-over event error analysis and solution

---

## 🔍 v0.5.4 Debug Logging Reference

### Overview
Version 0.5.4 adds optional debug logging with a user-configurable toggle in UMM settings. Debug logging is **disabled by default** to keep logs clean for normal gameplay.

### Enabling Debug Logging
1. Open Terra Invicta game
2. Open UMM settings for Assistance Mod
3. Toggle "Enable Debug Logging" ON
4. Changes saved automatically

### Debug Log Messages

**Control Point Capacity Patch Logging** (when enabled):

```
[CP_CAP_PATCH_PREFIX] GetControlPointMaintenanceFreebieCap() called for faction 'Project Exodus' (IsAlien=False)
[CP_CAP_PATCH_POSTFIX] Faction 'Project Exodus': Original Cap=385, Assist Bonus Adjustment=-55, Adjusted Cap=330
[CP_CAP_PATCH_POSTFIX] Faction 'the Academy': No assist bonuses to adjust. Cap=305
```

**Bonus Tracking Logging**:

```
[AssistBonusTracker] Recorded bonus for 'Blake Rowland': Administration +14, Total=45
[AssistBonusTracker] Recorded bonus for 'Blake Rowland': Science +6, Total=51
[AssistBonusTracker] Recorded bonus for 'Blake Rowland': Security +3, Total=54
[AssistMission] Removed bonuses for 'Evandro Semerawno' on mission complete
```

### What the Prefix Patch Does

The **Prefix patch** on `TIFactionState.GetControlPointMaintenanceFreebieCap()` logs every call to the vanilla method, showing:
- Faction name
- Whether faction is alien (should be filtered)
- Entry point timestamp

This allows you to trace the control point cap calculation flow for troubleshooting.

### What the Postfix Patch Does

The **Postfix patch** (conditional on debug flag) logs:
- Original calculated cap value
- Total assist bonus amount being subtracted
- Final adjusted cap value
- Handles cases where no bonuses exist

### Performance Impact

- **Debug OFF (default):** Minimal - no logging overhead
- **Debug ON:** 1-2 additional log entries per faction per turn (~50-100 bytes/log)
  - Acceptable for debugging but not recommended for long-term play
  - Disable after troubleshooting to reduce log file size

### Common Debug Scenarios

**Scenario 1: Verify Bonuses Applied**
```
Enable debug logging
Assign Assist mission to councilor
Check logs for:
  [AssistBonusTracker] Recorded bonus for...
  [CP_CAP_PATCH_POSTFIX] ... Assist Bonus Adjustment=...
```

**Scenario 2: Verify Bonuses Removed**
```
Enable debug logging
Have assisted councilor complete any mission
Check logs for:
  [AssistMission] Removed bonuses for ... on mission complete
  [CP_CAP_PATCH_POSTFIX] Faction 'X': No assist bonuses to adjust. Cap=...
```

**Scenario 3: Track CP Cap Changes**
```
Enable debug logging
Play several turns
Search logs for [CP_CAP_PATCH_POSTFIX]
Compare Original Cap vs Adjusted Cap values
Should show reduction equal to assist bonus amounts
```

### Disabling Debug Logging

1. Open UMM settings for Assistance Mod
2. Toggle "Enable Debug Logging" OFF
3. Changes saved automatically
4. Next log file will have no debug messages


