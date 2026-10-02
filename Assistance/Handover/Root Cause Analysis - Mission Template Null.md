# Root Cause Analysis: Mission Template Null During Deserialization

## The Crash
```
NullReferenceException: Object reference not set to an instance of an object
at PavonisInteractive.TerraInvicta.TICouncilorState.GetCurrentMissionIcon()
```

Stack trace shows it happens during Finder list rendering at game startup when loading a save.

## Root Cause
The vanilla `GetCurrentMissionIcon()` method (lines 3883-3895 of TICouncilorState.cs) does this:

```csharp
public string GetCurrentMissionIcon(bool on)
{
	if (!this.HasMission)  // Line 3885: Checks if activeMission != null
		return string.Empty;

	TIMissionTemplate missionTemplate = this.activeMission.missionTemplate;  // Line 3889: CRASH HERE
	if (!on)
		return missionTemplate.missionIconImagePath_Off;
	return missionTemplate.missionIconImagePath_On;
}
```

**The Problem:** 
- `HasMission` returns true (activeMission is not null)
- But `activeMission.missionTemplate` is NULL
- Code tries to access properties on null reference → crash

## Why Does missionTemplate Become Null?

During save/load when the mod-added Assist missions are present:

1. **Save contains:** Active TIMissionState with `missionTemplate` referencing "Assist" template
2. **Game deserializes** the saved mission state
3. **Template resolution** tries to find the "Assist" template by name
4. **If resolution happens before template registration:** Lookup fails
5. **Result:** Mission state has null missionTemplate field

The timing issue is subtle:
- `AssistTemplateRegistration.cs` registers the template via `TemplateManager.Initialize` postfix
- But missions may be deserialized DURING the broader `GameControl.Initialize` flow
- If template lookup happens at deserialization time (before our postfix runs), it fails
- Mission ends up with valid mission state but null template reference

## The Fix

**File:** `TICouncilorState_GetCurrentMissionIconPatch.cs`

A Prefix patch on `GetCurrentMissionIcon()` that:

1. **Checks if missionTemplate is null** before vanilla code tries to access it
2. **Returns an empty string** (graceful fallback) if template is missing
3. **Lets vanilla handle it normally** if template is valid

```csharp
[HarmonyPrefix]
public static bool Prefix(TICouncilorState __instance, bool on, ref string __result)
{
	// Safety checks
	if (__instance == null || __instance.activeMission == null)
	{
		__result = string.Empty;
		return false;
	}

	// CRITICAL: Check if template is null (can happen during save/load)
	if (__instance.activeMission.missionTemplate == null)
	{
		__result = string.Empty;  // Return safe empty string
		return false;  // Skip vanilla implementation
	}

	// All valid, let vanilla handle it
	return true;
}
```

## Why This Is Necessary

This is **defensive patching that's required, not optional**, because:

1. **Root cause is in vanilla game logic:** Template resolution timing is controlled by vanilla's deserialization system
2. **Mod extends vanilla:** We add new mission templates that vanilla doesn't know about initially
3. **Save/load timing mismatch:** Saved missions can reference our template before it's registered
4. **No hook earlier than TemplateManager.Initialize:** Template registration at mod startup is the earliest possible time
5. **Mission data can persist:** Saves from before mod was uninstalled/reinstalled can have stale references

## Why Earlier Approaches Didn't Work

We tried fixing this via AssistBonusTracker validation and TIGameState_LoadPatch timing, but those addressed the wrong issue:
- Those fixes handled **stale councilor references** in session state
- The actual problem is **null mission template during deserialization**
- These are different issues requiring different fixes

## Design Decisions

✅ **Defensive patching on UI method is appropriate here because:**
- The root cause (template resolution timing) is in vanilla game code
- The mod cannot control when missions are deserialized
- A safe fallback (empty icon string) is better than a crash
- Debug logging helps users identify when this occurs

✅ **Compare to other defensive patterns in the mod:**
- `TIMissionCondition_NotCurrentlyAssisting.cs` checks if missionTemplate is null
- `TIMissionResolution_Contested_AssistBonusPatch.cs` has null-safe GetMissionAttribute()
- Defensive null-checking is established pattern in this codebase

## Testing

1. **Reproduce the crash:** Load a save with active Assist mission
2. **Verify fix:** Game should load without NullReferenceException
3. **Check debug logs:** If debug logging enabled, should see messages like:
   ```
   [TICouncilorState_GetCurrentMissionIcon] Null missionTemplate for 'CouncilorName' - returning empty string
   ```
4. **Normal missions:** Should still display icons normally

## Related Issues

This is a separate issue from the earlier investigation into stale councilor references. Both issues can cause crashes during save/load, but they have different causes:

- **Stale councilor references** → Would cause crashes when accessing councilor properties
- **Null mission template** → Crashes specifically in UI rendering during icon lookup

Both are defensive patterns we now implement.

## Backward Compatibility

✅ **Yes** — Returns empty string (no icon) for malformed missions, which is a safe fallback that matches vanilla's behavior for missions with no icon.
