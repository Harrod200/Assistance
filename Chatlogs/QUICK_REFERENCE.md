# Quick Reference Cheat Sheet

## Most Critical Classes (Remember These!)

```csharp
// Hook Point
TINotificationQueueState → LogMissionOutcome_Postfix()

// Data Flow
TIMissionState mission
  ├─ TICouncilorState councilor (attacker)
  ├─ TIGameState target (councilor or control point)
  │  └─ Cast to: TICouncilorState targetCouncilor
  └─ TIMissionTemplate missionTemplate
	 └─ TIMissionResolution_Contested contestedResolution
		├─ GetAttackingNonZeroModifiers() → List<TIMissionModifier>
		├─ GetDefendingNonZeroModifiers() → List<TIMissionModifier>
		└─ Each modifier's value includes base stat!

// Modify Notification
NotificationQueueItem item = queue[0];
item.itemDetail += "\n\n" + breakdown;
```

## Key Methods at a Glance

| What | How |
|------|-----|
| Get modifiers | `contestedResolution.GetAttackingNonZeroModifiers(template, councilor, target, 0f)` |
| Calculate modifier | `modifier.GetModifier(councilor, target, 0f, resource)` - May throw if councilor null! |
| Get stat value | `councilor.GetAttribute(stat, true, true, true, false, false, false)` |
| Access faction councilors | Reflection only: `GetProperty("activeCouncilors", ...)` |
| Check control point | `mission.target is TIControlPoint` |
| Get defending faction | Use `heldTargetFaction` parameter (already provided!) |

## Common Errors & Solutions

| Error | Cause | Solution |
|-------|-------|----------|
| `Object reference not set` in GetModifier | `councilor` is null (control point) | Use defending faction leader |
| Double-counted base stat | Showing base stat + including it in modifiers | Only show modifiers (base already included) |
| `owningFaction` not found | Property doesn't exist | Use `heldTargetFaction` parameter instead |
| Modifiers skipped | Exception in GetModifier | Wrap in try-catch and use fallback |
| Notification not found | Mission state mismatch | Check `recentNotification.mission == mission` |

## The 5-Minute Patch Template

```csharp
[HarmonyPatch(typeof(TINotificationQueueState), nameof(TINotificationQueueState.LogMissionOutcome))]
public class MyPatcher
{
	[HarmonyPostfix]
	public static void LogMissionOutcome_Postfix(
		TIMissionState mission,
		MissionResult result,
		TIFactionState heldTargetFaction,
		List<TIGameState> newControlPoints = null,
		List<TIGameState> oldControlPoints = null,
		bool spy = false,
		string abortedReason = "")
	{
		// Early exits
		if (mission == null || !mission.missionTemplate.ContestedMission)
			return;

		// Get notification
		TINotificationQueueState queue = GameStateManager.NotificationQueue();
		if (queue?.notificationQueue.Count == 0)
			return;

		NotificationQueueItem item = queue.notificationQueue[0];
		if (item?.mission != mission)
			return;

		// Build content
		string breakdown = BuildBreakdown(mission, result, heldTargetFaction);

		// Append to notification
		if (!string.IsNullOrEmpty(breakdown))
			item.itemDetail += "\n\n" + breakdown;
	}

	private static string BuildBreakdown(TIMissionState mission, MissionResult result, TIFactionState defendingFaction)
	{
		// Your logic here
		return "breakdown text";
	}
}
```

## Remember!

1. ⭐ **Modifiers already include base stat** - Don't add it twice!
2. 🎯 **For control points**: Use `heldTargetFaction` to get defending leader
3. 🛡️ **Always try-catch** around `GetModifier()` calls
4. 🔍 **Use reflection** for `activeCouncilors` access
5. ✅ **Null check everything** - missions, notifications, modifiers, properties

---

**See VANILLA_REFERENCE_GUIDE.md for complete documentation**
