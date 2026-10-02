# Summary: NullReferenceException Fix for GetCurrentMissionIcon

## Issue
Game crashes with `NullReferenceException` in `TICouncilorState.GetCurrentMissionIcon()` when loading a save that contains active Assist missions.

```
NullReferenceException: Object reference not set to an instance of an object
at PavonisInteractive.TerraInvicta.TICouncilorState.GetCurrentMissionIcon()
```

This occurs during Finder list initialization at game startup.

## Root Cause
During save/load transitions:
- Saved Assist missions reference the "Assist" template by string name
- During deserialization, the game tries to resolve this reference to a TIMissionTemplate object
- If template resolution happens at an inopportune time, the lookup can fail
- The resulting mission state has a valid `activeMission` but a NULL `missionTemplate` field
- Vanilla code calls `missionTemplate.missionIconImagePath` on this null reference → crash

The issue is NOT about stale session state or councilor references; it's specifically about mission template resolution during deserialization.

## Solution
Created: **`Assistance/TICouncilorState_GetCurrentMissionIconPatch.cs`**

A HarmonyLib Prefix patch on `GetCurrentMissionIcon()` that:
1. Checks if `missionTemplate` is null before vanilla code accesses it
2. Returns an empty string (safe fallback) if template is missing
3. Allows vanilla to handle normally if template is valid

```csharp
[HarmonyPrefix]
public static bool Prefix(TICouncilorState __instance, bool on, ref string __result)
{
	// ... null checks ...

	// CRITICAL: Check if missionTemplate is null
	if (__instance.activeMission.missionTemplate == null)
	{
		__result = string.Empty;  // Safe fallback
		return false;  // Skip vanilla implementation
	}

	return true;  // Let vanilla handle normally
}
```

## Files Changed
- **Created:** `Assistance/TICouncilorState_GetCurrentMissionIconPatch.cs`
- **Created:** `Assistance/Handover/Root Cause Analysis - Mission Template Null.md`
- **Created:** `Assistance/Handover/Why Earlier Fix Didn't Work.md`

## Build Status
✅ **Successful** — Patch compiles and is automatically applied via HarmonyLib's PatchAll()

## Testing
1. Load a game save that has an active Assist mission
2. Game should load without NullReferenceException
3. Finder list should display normally (Assist mission will have no icon, since template is null)

## Why This Is Necessary
This is **defensive patching that's required**, not optional, because:
- Root cause is in vanilla's mission deserialization logic
- Mod extends vanilla with new mission templates that vanilla doesn't initially know about
- No hook earlier than TemplateManager.Initialize is viable
- Saved missions can reference templates at inopportune times during load
- A safe fallback (empty icon) prevents crash and allows game to continue

## Design Philosophy
This patch exemplifies defensive programming:
- **Accept that edge cases will happen** (template resolution timing issues)
- **Handle them gracefully** (return safe fallback instead of crashing)
- **Don't fight the framework** (vanilla's deserialization is what it is)
- **Log for debugging** (help users/devs understand what happened)

This is consistent with other defensive patterns in the codebase:
- `TIMissionCondition_NotCurrentlyAssisting.cs` checks if missionTemplate is null
- `TIMissionResolution_Contested_AssistBonusPatch.cs` has safe GetMissionAttribute()
- `AssistBonusTracker.cs` validates councilor references

---

**Previous Investigation Note:**
An earlier fix attempt focused on `AssistBonusTracker` state cleanup and timing. While those changes are still valuable for general robustness, they did not address this specific issue because they targeted the wrong root cause. See `Why Earlier Fix Didn't Work.md` for analysis.
