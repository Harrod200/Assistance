# TerraInvicta Vanilla Decompiled Files Reference

**Last Updated**: 16/09/2026  
**Context**: Assistance Mod - Mission Breakdown Display System  
**Assembly**: `Assembly-CSharp.dll`

---

## Quick Reference - Most Useful Vanilla Classes

| Class | Purpose | Key Methods/Properties | Used By |
|-------|---------|----------------------|----------|
| `TINotificationQueueState` | Manages in-game notifications queue | `LogMissionOutcome()`, `notificationQueue` | Patch hook point |
| `NotificationQueueItem` | Individual notification data | `mission`, `itemDetail` | Display modification |
| `TIMissionState` | Mission instance data | `displayName`, `councilor`, `target`, `missionTemplate` | Core mission info |
| `TIMissionTemplate` | Mission configuration | `ContestedMission`, `primaryAttackerStat`, `primaryDefenderStat()`, `resolutionMethod` | Mission setup |
| `TIMissionResolution_Contested` | Contested mission rules | `GetAttackingNonZeroModifiers()`, `GetDefendingNonZeroModifiers()`, `Difficulty()` | Critical for breakdown |
| `TIMissionModifier` | Modifier data entries | `displayName`, `GetModifier()` | Iterating modifiers |
| `TICouncilorState` | Councilor state | `displayName`, `GetAttribute()` | Attacker/defender info |
| `TIFactionState` | Faction data | `activeCouncilors` (reflection) | Finding defending leader |
| `TIControlPoint` | Control point target | Type checking only | Target discrimination |
| `MissionResult` | Mission outcome | `successChance`, `roll`, `missionOutcome` | Result display |
| `CouncilorAttribute` | Attribute enum | `Espionage`, `Administration`, etc. | Stat selection |
| `TIGameState` | Base game object | `displayName` | Generic target type |

---

## Detailed Class Documentation

### 1. **TINotificationQueueState**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Manages the queue of in-game notifications

#### Key Members:
- **`List<NotificationQueueItem> notificationQueue`** - Collection of pending notifications (most recent at index 0)
- **`void LogMissionOutcome(...)`** - Called when mission completes, perfect hook point for patches

#### Patch Pattern:
```csharp
[HarmonyPatch(typeof(TINotificationQueueState), nameof(TINotificationQueueState.LogMissionOutcome))]
public class Patcher
{
	[HarmonyPostfix]
	public static void LogMissionOutcome_Postfix(
		TIMissionState mission,
		MissionResult result,
		TIFactionState heldTargetFaction,  // Defending faction for control points
		List<TIGameState> newControlPoints = null,
		List<TIGameState> oldControlPoints = null,
		bool spy = false,
		string abortedReason = "")
}
```

#### Important Notes:
- Most recent notification is at `notificationQueue[0]`
- Access `notificationQueue[0].itemDetail` to append breakdown text
- Check for null before accessing collection

---

### 2. **NotificationQueueItem**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Data container for a single notification

#### Key Members:
- **`TIMissionState mission`** - Reference to the mission
- **`string itemDetail`** - Detailed text displayed in notification
- **`string itemHeader`** - Headline text

#### Usage Pattern:
```csharp
NotificationQueueItem item = notificationQueue.notificationQueue[0];
if (item.mission == targetMission)
{
	item.itemDetail += "\n\n" + breakdownText;  // Append detailed breakdown
}
```

---

### 3. **TIMissionState**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Instance data for a specific mission attempt

#### Key Members:
- **`string displayName`** - Mission name (e.g., "Purge")
- **`TICouncilorState councilor`** - Attacking councilor
- **`TIGameState target`** - Target (can be TICouncilorState or TIControlPoint)
- **`TIMissionTemplate missionTemplate`** - Template definition
- **`TIMissionState targetLocation`** - For location-based checks

#### Type Checking Pattern:
```csharp
TICouncilorState targetCouncilor = mission.target as TICouncilorState;
if (targetCouncilor == null && mission.target is TIControlPoint)
{
	// Handle control point target
}
```

---

### 4. **TIMissionTemplate**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Defines mission rules and configuration

#### Key Members:
- **`bool ContestedMission`** - Whether mission is contested (has attack/defense rolls)
- **`CouncilorAttribute primaryAttackerStat`** - Attacking stat (e.g., Espionage)
- **`CouncilorAttribute primaryDefenderStat()`** - Method returning defending stat
- **`TIMissionResolution resolutionMethod`** - Resolution system (cast to `TIMissionResolution_Contested`)
- **`TIResource primaryResource`** - Primary resource involved
- **`string friendlyName`** - Display name

#### Critical Pattern:
```csharp
if (!(missionTemplate.resolutionMethod is TIMissionResolution_Contested))
{
	return;  // Not a contested mission
}
TIMissionResolution_Contested contestedResolution = 
	missionTemplate.resolutionMethod as TIMissionResolution_Contested;
```

---

### 5. **TIMissionResolution_Contested** ⭐ CRITICAL
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Defines contested mission rules and calculations

#### Key Methods:
```csharp
// Returns non-zero attacking modifiers
List<TIMissionModifier> GetAttackingNonZeroModifiers(
	TIMissionTemplate template,
	TICouncilorState councilor,
	TIGameState target,
	float baseValue);

// Returns non-zero defending modifiers
List<TIMissionModifier> GetDefendingNonZeroModifiers(
	TIMissionTemplate template,
	TICouncilorState councilor,
	TIGameState target,
	float baseValue);

// Gets baseline difficulty for control points
float Difficulty(
	TIMissionTemplate template,
	TICouncilorState councilor,
	TIGameState target,
	float baseValue);
```

#### ⚠️ Critical Understanding:
**The modifiers list ALREADY INCLUDES the base stat as one modifier entry!**

Example:
- `GetAttackingNonZeroModifiers()` returns: `[Espionage (+14), Popular Support (+5), ...]`
- The first entry IS the base stat
- **Do NOT add base stat separately or you'll double-count**

---

### 6. **TIMissionModifier**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Individual modifier that affects mission outcome

#### Key Members:
- **`string displayName`** - Display name (e.g., "Espionage", "Council Administration")
- **`float GetModifier(...)`** - Calculate modifier value
  ```csharp
  float modValue = modifier.GetModifier(
	  councilor,           // Can be null for control points (may fail!)
	  target,
	  baseValue,
	  primaryResource);
  ```

#### Exception Handling Required:
```csharp
try
{
	float value = modifier.GetModifier(councilor, target, 0f, resource);
	// Use value
}
catch (Exception ex)
{
	// Some modifiers throw if councilor is null
	// Especially: Council Administration, Size of National Economy, etc.
}
```

#### Fallback Pattern (for Control Points):
```csharp
if (councilor == null && target is TIControlPoint && defendingFaction != null)
{
	// Get defending faction's leader via reflection
	var prop = defendingFaction.GetType().GetProperty("activeCouncilors",
		System.Reflection.BindingFlags.IgnoreCase | 
		System.Reflection.BindingFlags.Public | 
		System.Reflection.BindingFlags.Instance);

	if (prop != null && prop.GetValue(defendingFaction) is System.Collections.IList list)
	{
		TICouncilorState leader = list[0] as TICouncilorState;
		// Try again with leader
	}
}
```

---

### 7. **TICouncilorState**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Represents a councilor character

#### Key Members:
- **`string displayName`** - Councilor name (e.g., "Moustapha Aboud")
- **`TICouncilorAttribute GetAttribute(...)`** - Gets stat value
  ```csharp
  float statValue = councilor.GetAttribute(
	  CouncilorAttribute.Espionage,
	  includeBase: true,
	  includeTraits: true,
	  includeAssignments: true,
	  includeCurrentAssignment: false,
	  includeHeadquarters: false,
	  includeLoyalty: false);
  ```

#### Parameters Explained:
- `includeBase` - Base stat only (no bonuses)
- `includeTraits` - Add trait bonuses
- `includeAssignments` - Add assignment bonuses
- `includeCurrentAssignment` - Add current assignment bonus
- `includeHeadquarters` - Add HQ bonuses
- `includeLoyalty` - Add loyalty bonuses

---

### 8. **TIFactionState**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Faction state and configuration

#### Key Members:
- **`List<TICouncilorState> activeCouncilors`** - Collection of faction councilors
  - **⚠️ Access via Reflection Only** (property visibility may vary)
  ```csharp
  var prop = factionState.GetType().GetProperty("activeCouncilors",
	  System.Reflection.BindingFlags.IgnoreCase | 
	  System.Reflection.BindingFlags.Public | 
	  System.Reflection.BindingFlags.Instance);
  ```

#### Usage in Patches:
- Passed as `heldTargetFaction` parameter to `LogMissionOutcome_Postfix()`
- Used to find defending leader for control point missions
- Always null-check before accessing

---

### 9. **TIControlPoint**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Represents a geographical control point target

#### Type Discrimination:
```csharp
if (mission.target is TIControlPoint controlPoint)
{
	// This is a control point mission
	// targetCouncilor will be null
	// Use heldTargetFaction instead
}
```

#### Note:
- Does NOT have public `owningFaction` property
- Use `heldTargetFaction` parameter from `LogMissionOutcome()` instead
- Avoid trying to directly access faction info from the control point

---

### 10. **MissionResult**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Outcome data for a completed mission

#### Key Members:
- **`TIMissionOutcome missionOutcome`** - Success/Failure/Aborted enum
  ```csharp
  if (result.missionOutcome == TIMissionOutcome.Aborted)
  {
	  return;  // Don't show breakdown for aborted missions
  }
  ```
- **`float successChance`** - Probability of success (0.0-1.0)
- **`float roll`** - Random roll that determined outcome (0.0-1.0)

#### Display Pattern:
```csharp
breakdown.AppendFormat("Success Chance: {0:P2}", result.successChance);
breakdown.AppendFormat(" | Roll: {0:P2}", result.roll);
```

---

### 11. **CouncilorAttribute**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Enumeration of councilor statistics

#### Common Values:
- `Espionage` - Spy missions, sabotage
- `Administration` - Control point influence
- `Persuasion` - Diplomatic missions
- `Investigation` - Research/investigation
- `Command` - Military operations
- `Science` - Science generation
- `Security` - Defense/protection

#### Usage:
```csharp
CouncilorAttribute stat = missionTemplate.primaryAttackerStat;
float value = councilor.GetAttribute(stat, true, true, true, false, false, false);
```

---

### 12. **TIGameState**
**Namespace**: `PavonisInteractive.TerraInvicta`  
**Purpose**: Base class for all game objects

#### Key Members:
- **`string displayName`** - Display name for UI
- **`string GetType().Name`** - Type name for debugging

#### Usage:
```csharp
TIGameState target = mission.target;
if (target != null)
{
	string typeName = target.GetType().Name;  // "TIControlPoint" or "TICouncilorState"
}
```

---

## Common Patterns & Pitfalls

### ✅ CORRECT: Handling Control Point Missions
```csharp
TICouncilorState targetCouncilor = mission.target as TICouncilorState;

if (targetCouncilor != null)
{
	// Councilor vs Councilor
	modValue = modifier.GetModifier(targetCouncilor, mission.target, 0f, resource);
}
else if (mission.target is TIControlPoint && defendingFaction != null)
{
	// Control Point: Get defending faction's leader
	TICouncilorState defendingLeader = GetFactionLeader(defendingFaction);
	modValue = modifier.GetModifier(defendingLeader, mission.target, 0f, resource);
}
```

### ❌ WRONG: Trying to Access owningFaction
```csharp
// This will compile but fail at runtime:
TIFactionState faction = controlPoint.owningFaction;  // Property doesn't exist!
```

### ✅ CORRECT: Avoiding Double-Counting Base Stat
```csharp
// Modifiers ALREADY include base stat as first entry
List<TIMissionModifier> modifiers = GetAttackingNonZeroModifiers(...);
float total = 0f;
foreach (var mod in modifiers)
{
	float value = mod.GetModifier(councilor, target, 0f, resource);
	total += value;  // Includes base stat!
}
// Do NOT add baseAttackValue again!
```

### ❌ WRONG: Double-Counting Base Stat
```csharp
float baseValue = councilor.GetAttribute(stat, true, true, true, false, false, false);
float modifierTotal = 0f;
foreach (var mod in modifiers)
{
	modifierTotal += mod.GetModifier(...);  // Already includes base!
}
float total = baseValue + modifierTotal;  // DOUBLE-COUNT!
```

### ✅ CORRECT: Safe Reflection Access
```csharp
var prop = factionState.GetType().GetProperty("activeCouncilors",
	System.Reflection.BindingFlags.IgnoreCase |    // Case-insensitive
	System.Reflection.BindingFlags.Public |        // Public access
	System.Reflection.BindingFlags.Instance);      // Instance, not static

if (prop != null)
{
	var list = prop.GetValue(factionState) as System.Collections.IList;
	if (list != null && list.Count > 0)
	{
		TICouncilorState leader = list[0] as TICouncilorState;
	}
}
```

---

## Debugging Tips

### Enable Debug Logging
```csharp
if (Main.mod != null && Main.settings.debugLogging)
{
	Main.mod.Logger.Log($"[Tag] Variable: {value}");
}
```

### Common Debug Output Examples
```
[MissionDetailBreakdown] Mission template: Purge
[MissionDetailBreakdown] Councilor (attacker): Moustapha Aboud
[MissionDetailBreakdown] Target type: TIControlPoint
[MissionDetailBreakdown] Attacking modifiers count: 3
  - Espionage
  - Popular Support
  - Target Over Control Point Cap
```

### Player Log Location
```
C:\Users\<User>\AppData\LocalLow\Pavonis Interactive\TerraInvicta\Player.log
```

---

## Useful Search Terms for Decompiling

When working with dnSpy or similar decompilers:

- Search for: `LogMissionOutcome` → Entry point for patches
- Search for: `GetAttackingNonZeroModifiers` → Modifier collection logic
- Search for: `TIMissionResolution_Contested` → Core resolution system
- Search for: `NotificationQueueState` → Notification management
- Search for: `TIControlPoint` → Control point specific logic

---

## Related Mod Components

### AssistBonusTracker
- **Purpose**: Tracks assist bonuses from other councilors
- **Key Method**: `GetStatBonus(councilor, attribute)` - Returns bonus for stat
- **Used In**: Mission breakdown display to show assist contributions

### TIMissionResolution_Contested_AssistBonusPatch
- **Purpose**: Adds assist bonuses to contested missions
- **Related to**: The base patch that enables assist missions

---

## Version Compatibility Notes

- **.NET Framework**: 4.8
- **TerraInvicta Version**: Currently targeting vanilla assembly
- **HarmonyLib**: Used for IL patching
- **Reflection**: Required for property access when direct API not available

---

## Future Reference Checklist

- [ ] Always check `mission == null` before accessing properties
- [ ] Always check `notificationQueue.Count > 0` before indexing
- [ ] Always wrap modifier calculations in try-catch
- [ ] Always use reflection safely with null checks
- [ ] Always test both councilor-vs-councilor AND councilor-vs-controlpoint missions
- [ ] Always verify modifiers list doesn't include base stat separately
- [ ] Always null-check `heldTargetFaction` for control point missions

---

**Last Reviewed**: 16/09/2026  
**Next Review**: After next major mod feature or TerraInvicta update
