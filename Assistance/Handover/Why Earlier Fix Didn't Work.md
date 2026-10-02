# Analysis: Why the Earlier Fix Didn't Work

## What We Tried First
We attempted to fix the NullReferenceException by:
1. Changing `TIGameState_LoadPatch` from Postfix to Prefix
2. Adding defensive validation to `AssistBonusTracker`
3. Improving null-checking in `TIMissionCondition_NotCurrentlyAssisting`

This approach focused on **session-scoped state management** and **preventing stale councilor references**.

## Why It Didn't Work
The root cause was **not** about stale councilor references—it was about **null mission template during deserialization**.

### The Actual Problem
Looking at the crash location in vanilla code (TICouncilorState.cs lines 3883-3895):

```csharp
public string GetCurrentMissionIcon(bool on)
{
	if (!this.HasMission)  // ← HasMission returns true (activeMission != null)
		return string.Empty;

	TIMissionTemplate missionTemplate = this.activeMission.missionTemplate;  // ← missionTemplate is NULL
	if (!on)
		return missionTemplate.missionIconImagePath_Off;  // ← CRASH HERE
	return missionTemplate.missionIconImagePath_On;
}
```

The issue: `activeMission` exists but `activeMission.missionTemplate` is null.

This is **NOT** a stale reference issue (those would have null activeMission entirely). This is a **template resolution issue during deserialization**.

### Why Earlier Strategies Missed This

1. **AssistBonusTracker validation** — Addressed stale councilor dictionary keys
   - Problem: AssistBonusTracker isn't even called during UI rendering
   - Not a factor in GetCurrentMissionIcon crash

2. **Prefix on GameControl.Initialize** — Cleared bonuses earlier
   - Problem: Only affected AssistBonusTracker state
   - Did nothing to prevent null mission templates

3. **Better null-checking in mission conditions** — Made conditions safer
   - Problem: Mission condition checks don't run during Finder list rendering
   - Not involved in GetCurrentMissionIcon call

## The Real Root Cause
When loading a save with an active Assist mission:

1. **Deserialization phase:** Vanilla game deserializes saved mission state
   - Mission references "Assist" template by string name
   - Game tries to resolve this string to a TIMissionTemplate object
   - Template lookup runs at deserialization time

2. **Template registration timing:** Our Assist template is registered via TemplateManager.Initialize postfix
   - By the time GameControl.Initialize runs, template registration SHOULD have happened
   - But if template lookup occurs during a specific deserialization phase, it might fail
   - Mission ends up with valid TIMissionState but null missionTemplate field

3. **UI rendering:** Later during Finder list rendering
   - Vanilla calls GetCurrentMissionIcon()
   - HasMission returns true (activeMission exists)
   - Code tries to access missionTemplate.missionIconImagePath on null reference
   - **CRASH**

## Why The Fix Works
The patch on `GetCurrentMissionIcon()` directly addresses the issue:

```csharp
if (__instance.activeMission.missionTemplate == null)
{
	__result = string.Empty;  // Safe fallback
	return false;  // Skip vanilla code that would crash
}
```

This **prevents the crash at the exact point of failure**, rather than trying to prevent the condition earlier.

## Lesson Learned
When investigating crashes:
1. **Trace the exact crash location** (line of code that fails)
2. **Understand what's null** (not just "something is null", but WHAT)
3. **Identify the timing** (when does this specific null state occur)
4. **Match the fix to the problem** (not a related-but-different problem)

In this case:
- ❌ Initial assumption: Stale session state causes crashes
- ✅ Actual problem: Mission template can be null during deserialization
- ✅ Actual fix: Defend against null template in GetCurrentMissionIcon

## Code Artifacts
The following files from the earlier investigation are still useful and remain in place:
- `TIGameState_LoadPatch.cs` (Prefix version) — Useful for cleaning session state
- `AssistBonusTracker.cs` (with IsValidCouncilor) — Good defensive validation
- `TIMissionCondition_NotCurrentlyAssisting.cs` (improved null-checking) — Defensive pattern

These are complementary defensive measures even though they don't directly fix this specific crash. They make the mod more robust overall.

## Final Fix
**File:** `TICouncilorState_GetCurrentMissionIconPatch.cs`

A simple Prefix patch that checks if `missionTemplate` is null before vanilla code tries to use it. This is the correct defensive patching approach for this particular issue.
