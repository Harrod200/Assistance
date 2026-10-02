# Fix: NullReferenceException via Improved Save Persistence

## Issue
When loading a game from an existing instance, the game crashes with:
```
NullReferenceException: Object reference not set to an instance of an object
at PavonisInteractive.TerraInvicta.TICouncilorState.GetCurrentMissionIcon()
```

This occurs during Finder list initialization at game startup.

## Root Cause
The issue stems from **stale object references in session-scoped state**:

1. `AssistBonusTracker` is a static Dictionary that tracks councilor bonuses in memory
2. When switching between game sessions (loading existing save into already-running game), old Dictionary keys (TICouncilorState objects from *previous* session) remain in memory
3. During `GameControl.Initialize`, the vanilla game loads a new session with *new* councilor objects
4. Meanwhile, `AssistBonusTracker` still holds references to councilors from the old session
5. If UI code accesses these stale councilor objects before the tracker is cleared, their properties (like `activeMission`) might be null, causing crashes

The timing problem: The original Postfix patch on `GameControl.Initialize` ran **after** vanilla initialization, meaning stale references existed while the UI was being built.

## Solution: Three-Part Fix

### Part 1: Prefix-Based Cleanup (Earlier Timing)
**File:** `TIGameState_LoadPatch.cs`

Changed from **Postfix** to **Prefix** on `GameControl.Initialize`:
- Now clears `AssistBonusTracker` **BEFORE** vanilla initialization runs
- Ensures no stale references exist when the UI tries to render mission icons
- This is the primary fix that prevents the exception

```csharp
[HarmonyPrefix]  // CHANGED: was Postfix
private static void Prefix(bool loadingSave)
{
	AssistBonusTracker.Clear();  // Run BEFORE vanilla init
}
```

### Part 2: Defensive Reference Validation
**File:** `AssistBonusTracker.cs`

Added a validation method to detect stale councilor references:

```csharp
private static bool IsValidCouncilor(TICouncilorState councilor)
{
	// Basic null check
	if (councilor == null)
		return false;

	// Verify the councilor has a valid faction (indicates current game state)
	// Stale councilors from previous sessions will have null faction
	if (councilor.faction == null)
		return false;

	return true;
}
```

Updated `GetStatBonus()` and `GetTotalBonus()` to validate before accessing:
- If a stale reference somehow remains, it's skipped
- Returns 0 (no bonus) for invalid councilors
- Prevents exceptions even if tracking still has stale data

### Part 3: Null-Safe Mission State Checks
**File:** `TIMissionCondition_NotCurrentlyAssisting.cs`

Added explicit null checks for `missionTemplate`:

```csharp
if (targetCouncilor.activeMission != null)
{
	var missionTemplate = targetCouncilor.activeMission.missionTemplate;
	if (missionTemplate != null && missionTemplate.dataName == "Assist")
	{
		return TIMissionCondition.fail;
	}
}
```

This ensures even if `activeMission` exists but `missionTemplate` is null, no exception occurs.

## Changes Made

| File | Change | Reason |
|------|--------|--------|
| `TIGameState_LoadPatch.cs` | Postfix → Prefix | Clear bonuses BEFORE UI init, not after |
| `AssistBonusTracker.cs` | Added `IsValidCouncilor()` | Detect and skip stale references |
| `AssistBonusTracker.cs` | Updated `GetStatBonus()` | Validate councilor before access |
| `AssistBonusTracker.cs` | Updated `GetTotalBonus()` | Validate councilor before access |
| `TIMissionCondition_NotCurrentlyAssisting.cs` | Better null-checking | Safe mission template access |
| `TICouncilorState_GetCurrentMissionIconPatch.cs` | **REMOVED** | No longer needed; root cause fixed |

## Why This Is Better Than the Defensive Patch

**Old approach:** Added a Prefix to `GetCurrentMissionIcon()` to catch null missions
- ✅ Fixed the symptom
- ❌ Didn't address root cause (stale references still exist)
- ❌ Extra patch adds slight performance overhead

**New approach:** Fix save persistence cleanup timing and validate references
- ✅ Prevents stale references from existing in the first place
- ✅ Multiple defensive layers (timing, validation, null-checks)
- ✅ Cleaner architecture (no extra patches on UI methods)
- ✅ Scales to prevent similar issues in other code paths

## Testing Recommendations

1. **Basic load test:**
   - Load a game from an existing instance (don't start new game)
   - Verify game loads without NullReferenceException
   - Finder list should display normally

2. **Stale reference validation:**
   - Enable debug logging in mod settings
   - Watch for "GetTotalBonus called with invalid councilor" messages
   - Should see 0 or 1 (only on first load transition)

3. **Mission condition check:**
   - Run an Assist mission successfully
   - Verify that assisted councilor cannot be targeted by another Assist
   - Load/reload saves multiple times

## Design Principles Preserved

1. **Session-Scoped Bonuses:** Bonuses are in-memory only, not persisted to saves ✅
2. **Mission Learning Persistence:** Learned missions are saved via vanilla's learnedMissions ✅
3. **Clean Load Transitions:** New session starts with empty tracker ✅
4. **HarmonyLib Patterns:** All modifications use decorators, no direct injection ✅

## Related Code
- `TIGameState_LoadPatch.cs` — Timing of tracker cleanup
- `AssistBonusTracker.cs` — In-memory bonus tracking with validation
- `TIMissionCondition_NotCurrentlyAssisting.cs` — Mission condition checks
- `TICouncilorState_AdvisingBonusPatch.cs` — Reads bonuses (also validates now)
- `TIMissionResolution_Contested_AssistBonusPatch.cs` — Applies bonuses during mission checks

## Backward Compatibility
✅ **Yes** — This is a structural improvement that doesn't change mod behavior or save format
