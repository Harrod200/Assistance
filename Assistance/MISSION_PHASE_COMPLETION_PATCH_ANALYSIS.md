# Harmony Patch Analysis: Clearing Assist Mission Bonuses on Mission Phase Completion

## Summary
To clear assist mission bonuses **only upon mission phase completion** (not individual mission completion), you have **2 optimal approaches** depending on your requirements.

---

## Current State
Your mod currently clears bonuses in two places:
1. **`TICouncilorState_CompleteMissionPatch`** - Clears bonuses when **individual missions complete** (called from `SetCompletedMission()`)
2. **`TIMissionResolution_Contested_AssistBonusPatch`** - Applies bonuses during contested mission resolution

---

## Vanilla Code Analysis

### Mission Phase Completion Flow
```
TIMissionPhaseState.SetMissionPhaseInactive()
	↓
GameControl.eventManager.TriggerEvent(new TimeEventComplete(...), "CouncilorMissionUpdate")
	↓
All listeners on "CouncilorMissionUpdate" event channel are invoked
	├─ CouncilorMissionCanvasController.ResetOnComplete()
	├─ GeneralControlsController.OnMissionPhaseComplete()
	└─ [YOUR PATCH GOES HERE]
```

### Key Method in Vanilla
**`TIMissionPhaseState.SetMissionPhaseInactive()`** (Line 174):
```csharp
public void SetMissionPhaseInactive()
{
	this.phaseActive = false;
	this.factionsSignallingComplete.Clear();
	GameControl.eventManager.TriggerEvent(new TimeEventComplete(null, null), 
		"CouncilorMissionUpdate", Array.Empty<object>());
}
```

---

## Option 1: Patch `SetMissionPhaseInactive()` (RECOMMENDED)
**Best for:** Clearing all bonuses at the end of mission phase cycle

### Pros
- ✅ Clean, single event-based trigger
- ✅ Fires exactly once per mission phase
- ✅ Complements your existing `SetCompletedMission` patch
- ✅ Aligns with vanilla event architecture
- ✅ Handles all councilors in one place

### Cons
- ❌ Requires knowledge that phase is ending (less direct)
- ❌ Fires for ALL councilors regardless of whether they had active missions

### Implementation Pattern
```csharp
[HarmonyPatch(typeof(TIMissionPhaseState), "SetMissionPhaseInactive")]
internal static class TIMissionPhaseState_SetMissionPhaseInactivePatch
{
	public static void Prefix(TIMissionPhaseState __instance)
	{
		if (Main.enabled && Main.settings != null && Main.settings.enableAssistMission)
		{
			// Clear all assist mission bonuses before phase ends
			ClearAllAssistBonuses();
		}
	}
}
```

---

## Option 2: Patch `StartofTurnBookkeeping()` (ALTERNATIVE)
**Best for:** Clearing bonuses during phase transition setup

### Pros
- ✅ Happens during explicit cleanup phase
- ✅ Naturally paired with other start-of-turn operations
- ✅ More granular control per councilor
- ✅ Works alongside `ClearCompletedMission()` calls

### Cons
- ❌ Not as clean semantically (clearing at "start of turn" is confusing)
- ❌ Method does a lot of other things (harder to trace)
- ❌ Less event-driven

### Implementation Pattern
```csharp
[HarmonyPatch(typeof(TIMissionPhaseState), "StartofTurnBookkeeping")]
internal static class TIMissionPhaseState_StartofTurnBookkeepingPatch
{
	public static void Postfix(TIMissionPhaseState __instance)
	{
		if (Main.enabled && Main.settings != null && Main.settings.enableAssistMission)
		{
			// Clear after bookkeeping is done
			ClearAllAssistBonuses();
		}
	}
}
```

---

## Option 3: Hook `TimeEventComplete` Event (MOST FLEXIBLE)
**Best for:** Maximum control over cleanup timing

### Pros
- ✅ Event-driven architecture
- ✅ Can distinguish between different `TimeEventComplete` sources
- ✅ Most aligned with existing listener pattern
- ✅ Easy to disable/toggle per-event-type

### Cons
- ❌ Requires event listener registration in `Main.cs`
- ❌ More verbose than a direct patch
- ❌ Different pattern from other patches in your mod

### Implementation Pattern
```csharp
// In your Main mod initialization:
GameControl.eventManager.AddListener<TimeEventComplete>(
	new EventManager.EventDelegate<TimeEventComplete>(OnMissionPhaseCompleted),
	"CouncilorMissionUpdate", null, false, false);

private static void OnMissionPhaseCompleted(TimeEventComplete e)
{
	if (Main.enabled && Main.settings != null && Main.settings.enableAssistMission)
	{
		ClearAllAssistBonuses();
	}
}
```

---

## Comparison Table

| Criteria | Option 1 | Option 2 | Option 3 |
|----------|----------|----------|----------|
| **Clarity** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Code Simplicity** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐ |
| **Performance** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **Consistency with Mod** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| **Flexibility** | ⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ |

---

## Recommendation: **USE OPTION 1**

**Reason:** Your mod already uses `HarmonyPatch` extensively. Option 1 maintains that pattern, is the clearest semantically, and provides the cleanest implementation.

### Implementation Steps:
1. Create new patch file: `TIMissionPhaseState_SetMissionPhaseInactivePatch.cs`
2. Patch `TIMissionPhaseState.SetMissionPhaseInactive()`
3. Use `Prefix` to clear bonuses before phase becomes inactive
4. Update `AssistBonusTracker.RemoveBonuses()` to support clearing all bonuses at once if needed

### Code Structure to Add:
```csharp
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
	/// <summary>
	/// Clears ALL assist mission bonuses when mission phase ends.
	/// This ensures bonuses don't carry over between mission phases.
	/// </summary>
	[HarmonyPatch(typeof(TIMissionPhaseState), "SetMissionPhaseInactive")]
	internal static class TIMissionPhaseState_SetMissionPhaseInactivePatch
	{
		public static void Prefix()
		{
			if (Main.enabled && Main.settings != null && Main.settings.enableAssistMission)
			{
				AssistBonusTracker.ClearAllBonuses();

				if (Main.mod != null && Main.settings.debugLogging)
				{
					Main.mod.Logger.Log("[AssistMission] Cleared all assist bonuses at mission phase end");
				}
			}
		}
	}
}
```

### Helper Method to Add to `AssistBonusTracker`:
```csharp
/// <summary>
/// Clears all tracked assist bonuses for all councilors.
/// Called when mission phase ends to prevent bonus carryover.
/// </summary>
public static void ClearAllBonuses()
{
	trackedBonuses.Clear();
	totalBonusAmounts.Clear();
}
```

---

## Decision Flow Chart

```
Do you want bonuses cleared...?
├─ When individual missions complete → USE TICouncilorState_CompleteMissionPatch (already have)
├─ When entire mission phase ends → USE Option 1 (SetMissionPhaseInactive) ⭐ RECOMMENDED
├─ With other turn bookkeeping → USE Option 2 (StartofTurnBookkeeping)
└─ With maximum flexibility/events → USE Option 3 (TimeEventComplete listener)
```

---

## Notes
- Your existing `TICouncilorState_CompleteMissionPatch` handles individual mission completion cleanup
- Option 1 acts as a **phase-level cleanup** after all individual missions are done
- You may want to use **both** patches:
  - Individual: `SetCompletedMission` clears bonuses used by that councilor
  - Phase-level: `SetMissionPhaseInactive` ensures any stragglers are cleared
