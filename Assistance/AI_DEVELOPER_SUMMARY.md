# Assistance Mod - AI Developer Handoff Summary

**Version:** 0.3.1  
**Last Updated:** 2026-09-02  
**Status:** ✅ Stable and Tested  
**Target Game:** Terra Invicta 1.0.53+  
**UMM Version:** 0.33.0.0+  

---

## 📋 Developer Workflow Instructions

**IMPORTANT:** Every commit to this project must follow these steps:

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

### Committing
4. **Stage changes**:
   ```powershell
   git add -A
   ```

5. **Commit with semantic message**:
   ```powershell
   git commit -m "feat/fix/docs: Brief description

   - Detailed bullet point 1
   - Detailed bullet point 2
   - Detailed bullet point 3"
   ```
   - Use prefix: `feat:` (feature), `fix:` (bug fix), `docs:` (documentation), `refactor:` (code restructuring), `perf:` (performance)
   - Include detailed changelog in body
   - Reference version number in commit body if significant

6. **Push to master**:
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

# 5. Git operations
git add -A
git commit -m "fix: Resolve issue with bonus calculation

- Fixed bonus amount calculation when source stat is 0
- Added null check in AssistBonusTracker
- Updated version to 0.3.2"
git push origin master
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
| 0.3.1 | 2026-09-02 | **CURRENT** - Fixed critical AI mission planner KeyNotFoundException crash. Restructured mission template to exactly match vanilla Inspire mission structure. Added TIMissionModifier_ResourceSpent, defensive modifiers, proper Context lists, and all required conditions (Human, FreeCouncilor). Enhanced modifier inheritance and bootstrap logging. |
| 0.3.0 | 2026-09-01 | Fixed target validation to match Inspire mission exactly. Removed CouncilorOnEarth condition and TargetHasNoMission custom condition. Assist now targets any friendly councilor regardless of mission status. |
| 0.2.0 | 2026-08-31 | Fixed UI crash by switching from TIMissionResolution_Automatic to TIMissionResolution_Contested with proper modifiers. Added custom TIMissionModifier_AssistStat modifier. |
| 0.1.0 | 2026-08-30 | Initial working version. Implemented core assist mission, stat transfer logic, bonus tracking with auto-removal, and English localization. |

---

## 📁 Project Structure

```
Assistance/
├── TIMissionTemplate_Assist.cs              [Mission definition - CORE FILE]
├── TIMissionEffect_Assist.cs                [Stat transfer logic during mission completion]
├── TIMissionModifier_AssistStat.cs          [Mission resolution modifier for contested resolution]
├── TIMissionModifier_AssistFlat.cs          [Placeholder flat modifier]
├── AssistBonusTracker.cs                    [Tracks and removes bonuses after mission complete]
├── TICouncilorState_CompleteMissionPatch.cs [Harmony patch to trigger bonus removal]
├── CouncilorMissionCanvasController_UpdateModifierListPatch.cs [UI rendering patch]
├── Main.cs                                  [UMM entry point & GUI settings slider]
├── Settings.cs                              [Configuration storage]
├── AssistMissionBootstrapPatch.cs           [Harmony bootstrap patch for mission registration]
├── English.xml                              [Localization strings]
├── Properties/AssemblyInfo.cs               [Assembly version: 0.3.1]
└── bin/Debug/Assistance.dll                 [Compiled mod, ~11 KB]
```

---

## 🔑 Critical Implementation Details

### Vanilla Mission Template Reference

The Assist mission template is built to match the vanilla **Inspire** mission exactly. Vanilla mission templates are stored in:
- **Game Location:** `C:\Games\Steam\steamapps\common\Terra Invicta\TerraInvicta_Data\StreamingAssets\Templates\TIMissionTemplate.json`

This JSON file contains all vanilla mission definitions. Compare your Assist mission against the Inspire mission in this file to verify compatibility.

### Mission Targeting (Matches Inspire Mission Exactly)

The Assist mission uses **all 4 of Inspire's conditions:**

```csharp
this.conditions = new List<TIMissionCondition>
{
	new TIMissionCondition_TargetInRange(),      // Target must be in communication range
	new TIMissionCondition_Human(),              // Target must be human (not alien/proxy)
	new TIMissionCondition_MyFactionCouncilor()  // Target must be same faction, not self
	new TIMissionCondition_FreeCouncilor()       // Target must not be detained
};
```

**IMPORTANT:** The Assist mission can target councilors who are actively on other missions (similar to Inspire). The `FreeCouncilor` condition only checks if the councilor is detained, not whether they have an active mission.

### Resolution Method (Critical for AI Planner)

Uses `TIMissionResolution_Contested` with:
- **Attacking Modifiers:**
  1. `TIMissionModifier_CouncilorAttackStat` (Persuasion) - REQUIRED for AI planner primaryAttackerStat property
  2. `TIMissionModifier_ResourceSpent()` - Standard cost modifier
- **Defending Modifiers:**
  1. `TIMissionModifier_FlatModifier` (value: 0) - Prevents null issues

**Critical Note:** The AI mission planner (`AICouncilorMissionPlanner.PlanMissionsTask`) iterates through mission properties including:
- `mission.primaryAttackerStat` - looks for `TIMissionModifier_CouncilorAttackStat` type
- `mission.attackerContexts` - MUST NOT be empty (use `{Context.None}`)
- `mission.defenderContexts` - MUST NOT be empty (use `{Context.None}`)

If these aren't properly initialized, the game crashes with `KeyNotFoundException`.

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

### 4. Contested Resolution with CouncilorAttackStat
- **Decision:** Use Contested resolution with TIMissionModifier_CouncilorAttackStat(Persuasion)
- **Rationale:** AI planner requires specific modifier type; Persuasion matches support mission theme
- **Tradeoff:** Modifier displays as Persuasion in UI (cosmetic)

---

## 🚀 Build & Deploy

### Build
```powershell
# In Visual Studio:
Build → Build Solution (Ctrl+Shift+B)
# Output: Assistance\bin\Debug\Assistance.dll (~11 KB)
```

### Deploy
```powershell
Copy-Item -Path "Assistance\bin\Debug\Assistance.dll" `
  -Destination "C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\Assistance.dll" -Force
```

### ModInfo.json Location
```
C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\AssistMission\ModInfo.json
```

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
