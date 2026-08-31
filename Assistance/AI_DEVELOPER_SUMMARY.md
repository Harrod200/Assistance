# Assistance Mod - AI Developer Handoff Summary

**Version:** 0.3.0  
**Last Updated:** 2026-09-01  
**Status:** ✅ Stable and Tested  
**Target Game:** Terra Invicta 1.0.53+  
**UMM Version:** 0.33.0.0+  

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
| 0.3.0 | 2026-09-01 | **CURRENT** - Fixed target validation to match Inspire mission exactly. Removed CouncilorOnEarth condition and TargetHasNoMission custom condition. Assist now targets any friendly councilor regardless of mission status. |
| 0.2.0 | 2026-08-31 | Fixed UI crash by switching from TIMissionResolution_Automatic to TIMissionResolution_Contested with proper modifiers. Added custom TIMissionModifier_AssistStat modifier. |
| 0.1.0 | 2026-08-30 | Initial working version. Implemented core assist mission, stat transfer logic, bonus tracking with auto-removal, and English localization. |

---

## 📁 Project Structure

```
Assistance/
├── TIMissionTemplate_Assist.cs              [Mission definition - CORE FILE]
├── TIMissionEffect_Assist.cs                [Stat transfer logic during mission completion]
├── TIMissionModifier_AssistStat.cs          [Contested resolution modifier for UI rendering]
├── TIMissionModifier_AssistFlat.cs          [Placeholder flat modifier]
├── AssistBonusTracker.cs                    [Tracks and removes bonuses after mission complete]
├── TICouncilorState_CompleteMissionPatch.cs [Harmony patch to trigger bonus removal]
├── CouncilorMissionCanvasController_UpdateModifierListPatch.cs [UI rendering patch]
├── Main.cs                                  [UMM entry point & GUI settings slider]
├── Settings.cs                              [Configuration storage]
├── AssistMissionBootstrapPatch.cs           [Harmony bootstrap patch for mission registration]
├── English.xml                              [Localization strings]
├── Properties/AssemblyInfo.cs               [Assembly version: 0.3.0]
└── bin/Debug/Assistance.dll                 [Compiled mod, ~11 KB]
```

---

## 🔑 Critical Implementation Details

### Mission Targeting (Matches Inspire Mission)

The Assist mission targets are validated using **only 2 conditions:**

```csharp
this.conditions = new List<TIMissionCondition>
{
	new TIMissionCondition_TargetInRange(),      // Target must be in range
	new TIMissionCondition_MyFactionCouncilor()  // Target must be same faction, not self
};
```

**IMPORTANT:** There is NO "TargetHasNoMission" restriction. The Assist mission can target councilors who are actively on other missions. This matches the vanilla Inspire mission behavior.

### Resolution Method

Uses `TIMissionResolution_Contested` (NOT Automatic) with:
- **Attacking Modifiers:** `TIMissionModifier_AssistStat()`
- **Defending Modifiers:** Empty list

This is required because the game's `TIMissionTemplate.get_primaryAttackerStat()` property iterates through `resolutionMethod.attackingModifiers` during UI rendering. Without this, the game crashes with NullReferenceException.

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

## 🐛 Known Issues & Solutions

| Issue | Root Cause | Solution |
|-------|-----------|----------|
| AI mission planner crashes | Having CouncilorOnEarth condition + TargetHasNoMission caused dictionary access errors in AICouncilorMissionPlanner.PlanMissionsTask | Removed both conditions, kept only TargetInRange + MyFactionCouncilor |
| UI crash on mission hover | TIMissionResolution_Automatic doesn't expose `attackingModifiers` property | Changed to TIMissionResolution_Contested with proper modifier list |
| Bonuses were permanent | No removal mechanism | Implemented AssistBonusTracker + SetCompletedMission patch |

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
- **Decision:** No IP/Influence cost
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

### 4. Contested Resolution (vs. Automatic)
- **Decision:** Use Contested so game engine can render UI properly
- **Rationale:** Game requires `attackingModifiers` list for UI (non-negotiable)
- **Tradeoff:** Technically supports failures (though rare due to high Persuasion)

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
