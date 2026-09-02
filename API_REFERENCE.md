# Assistance Mod - API Reference

## Core Classes

### TIMissionTemplate_Assist

**File**: `TIMissionTemplate_Assist.cs`

Defines the Assist mission template configuration.

#### Constructor
```csharp
public TIMissionTemplate_Assist() : base("Assist")
```

#### Configuration Properties Set

| Property | Type | Value | Purpose |
|----------|------|-------|---------|
| `dataName` | string | "Assist" | Unique mission identifier |
| `friendlyName` | string | "Assist Councilor" | Display name |
| `disable` | bool | false | Mission is enabled |
| `baseMission` | bool | false | Not a base mission |
| `persistentEffect` | bool | false | Bonuses don't persist |
| `noise` | float[] | [0, -2, -4, 0, -4, -4] | Noise profile matching Inspire |
| `hate` | float[] | [0, 0, 0, 0, 0, 0] | No hate generation |
| `XPonSuccess` | int | 2 | Experience reward |
| `resolutionOrder` | int | 0 | Resolves first each turn |
| `missionContext` | MissionContext | Unlimited | Works in all contexts |
| `resolutionMethod` | TIMissionResolution | Automatic | Guaranteed success |

#### Mission Conditions
- `TIMissionCondition_MyFactionCouncilor`: Target must be same faction
- `TIMissionCondition_PlayerFactionOnly`: Player factions only
- `TIMissionCondition_NotCurrentlyAssisting`: No multiple assists on same target

#### Mission Target
- `TIMissionTarget_Councilor`: Targets other councilors

#### Mission Effect
- `TIMissionEffect_Assist`: Applies temporary stat bonuses

---

### TIMissionEffect_Assist

**File**: `TIMissionEffect_Assist.cs`

Applies temporary stat bonuses when assist mission succeeds.

#### Method: ApplyEffect

```csharp
public void ApplyEffect(
	TIGameState targetObj,
	bool success,
	TICouncilorState councilor,
	TIMissionTemplate missionTemplate,
	TIMissionOutcome outcome)
```

**Parameters**:
- `targetObj` (TIGameState): Target being assisted (councilor)
- `success` (bool): Whether mission succeeded
- `councilor` (TICouncilorState): Assisting councilor
- `missionTemplate` (TIMissionTemplate): Mission template (Assist)
- `outcome` (TIMissionOutcome): Mission outcome

**Logic**:
1. Validates success and enabled state
2. Casts targetObj to TICouncilorState
3. For each councilor attribute:
   - Calculates bonus = assister's stat × (assistPercentage / 100)
   - Calls `AssistBonusTracker.RecordBonus()`
4. Returns formatted message of applied bonuses

**Return**: String describing applied bonuses

---

### AssistBonusTracker

**File**: `AssistBonusTracker.cs`

Central system for tracking temporary stat bonuses.

#### Static Methods

##### RecordBonus

```csharp
public static void RecordBonus(
	TICouncilorState councilor,
	CouncilorAttribute stat,
	int amount)
```

Records a temporary stat bonus for a councilor.

**Parameters**:
- `councilor` (TICouncilorState): Target councilor
- `stat` (CouncilorAttribute): Stat attribute being boosted
- `amount` (int): Bonus amount in points

**Behavior**:
- Creates entry if councilor not tracked
- Adds amount to existing bonus for stat
- Only records if amount > 0
- Logs operation if debug logging enabled

**Example**:
```csharp
AssistBonusTracker.RecordBonus(targetCouncilor, 
	CouncilorAttribute.Persuasion, 2);
```

---

##### GetStatBonus

```csharp
public static int GetStatBonus(
	TICouncilorState councilor,
	CouncilorAttribute stat)
```

Retrieves bonus for a specific stat.

**Parameters**:
- `councilor` (TICouncilorState): Target councilor
- `stat` (CouncilorAttribute): Stat attribute

**Returns**: Bonus amount for stat (0 if no bonus)

**Example**:
```csharp
int persuasionBonus = AssistBonusTracker.GetStatBonus(
	councilor, CouncilorAttribute.Persuasion);
```

---

##### GetTotalBonus

```csharp
public static int GetTotalBonus(TICouncilorState councilor)
```

Retrieves total bonus across all stats for a councilor.

**Parameters**:
- `councilor` (TICouncilorState): Target councilor

**Returns**: Sum of all stat bonuses (0 if no bonuses)

**Example**:
```csharp
int totalBonus = AssistBonusTracker.GetTotalBonus(councilor);
if (totalBonus > 0)
{
	// Apply total bonus to contested mission
}
```

---

##### RemoveBonuses

```csharp
public static void RemoveBonuses(TICouncilorState councilor)
```

Clears all tracked bonuses for a councilor.

**Parameters**:
- `councilor` (TICouncilorState): Target councilor

**Behavior**:
- Removes all bonuses for councilor
- Called when assist mission completes
- Safe to call if no bonuses exist

**Example**:
```csharp
// Called in TICouncilorState_CompleteMissionPatch
AssistBonusTracker.RemoveBonuses(__instance);
```

---

##### ClearAll

```csharp
public static void ClearAll()
```

Clears all bonuses for all councilors (system reset).

**Behavior**:
- Clears entire tracking dictionary
- Useful for game reset/cleanup
- Call before new game or major state change

---

### TIMissionCondition_MyFactionCouncilor

**File**: `TIMissionCondition_MyFactionCouncilor.cs`

Ensures target is from assisting councilor's faction.

#### Method: Validate

```csharp
public override string Validate(
	TIMissionTemplate mission,
	TICouncilorState councilor,
	TIGameState target)
```

**Returns**:
- `null` if valid (target is same faction and not self)
- Error message if invalid

**Checks**:
- Target is TICouncilorState
- Target faction == councilor faction
- Target != councilor (prevent self-target)

---

### TIMissionCondition_PlayerFactionOnly

**File**: `TIMissionCondition_PlayerFactionOnly.cs`

Restricts assist missions to player-controlled factions.

#### Method: Validate

```csharp
public override string Validate(
	TIMissionTemplate mission,
	TICouncilorState councilor,
	TIGameState target)
```

**Returns**:
- `null` if faction is player-controlled
- Error message if AI faction

**Check**: `councilor.faction.IsPlayerControlled`

---

### TIMissionCondition_NotCurrentlyAssisting

**File**: `TIMissionCondition_NotCurrentlyAssisting.cs`

Prevents multiple assist missions on same target.

#### Method: Validate

```csharp
public override string Validate(
	TIMissionTemplate mission,
	TICouncilorState councilor,
	TIGameState target)
```

**Returns**:
- `null` if target not currently receiving assistance
- Error message if already being assisted

**Check**: Searches active missions for assist missions targeting this councilor

---

## Harmony Patches

### AssistMissionBootstrapPatch

**File**: `AssistMissionBootstrapPatch.cs`

**Target**: `SolarSystemBootstrap.Initialize()`

**Type**: Postfix

**Methods**:

#### RegisterMissionTemplate

```csharp
private static void RegisterMissionTemplate()
```

Creates `TIMissionTemplate_Assist` instance and registers with game.

---

#### GrantToAllCouncilors

```csharp
private static void GrantToAllCouncilors()
```

Adds assist mission to all councilor types' available missions.

---

### FinalizeCouncilorMissions_AssistPriorityPatch

**File**: `FinalizeCouncilorMissions_AssistPriorityPatch.cs`

**Target**: `FinalizeCouncilorMissions.StaggerMissionResolutions()`

**Type**: Postfix

**Behavior**: Sorts missions to resolve assist missions first

**Key Logic**:
```csharp
var assistMissions = sortedMissions
	.Where(m => m.missionTemplate?.dataName == "Assist")
	.ToList();

// Move assist missions to front
sortedMissions = assistMissions
	.Concat(sortedMissions.Except(assistMissions))
	.ToList();
```

---

### TIMissionResolution_Contested_AssistBonusPatch

**File**: `TIMissionResolution_Contested_AssistBonusPatch.cs`

**Targets**:
- `TIMissionResolution_Contested.SumAttackingModifiers()`
- `TIMissionResolution_Contested.SumDefendingModifiers()`

**Type**: Postfix

**Behavior**: Adds assist bonus to contested mission calculations

**Logic** (both methods):
1. Get attacking/defending councilor
2. Query `AssistBonusTracker.GetTotalBonus(councilor)`
3. Add bonus to result

**Example**:
```csharp
int assistBonus = AssistBonusTracker.GetTotalBonus(councilor);
if (assistBonus > 0)
{
	__result = (int)__result + assistBonus;
}
```

---

### TICouncilorState_GetPossibleMissionListPatch

**File**: `TICouncilorState_GetPossibleMissionListPatch.cs`

**Target**: `TICouncilorState.GetPossibleMissionList()`

**Type**: Postfix

**Behavior**: Removes assist mission from AI-controlled faction councilors

**Logic**:
```csharp
if (!__instance.faction.IsPlayerControlled)
{
	// Remove Assist missions for AI
	returnValue.RemoveAll(m => m.dataName == "Assist");
}
```

---

### TICouncilorState_CompleteMissionPatch

**File**: `TICouncilorState_CompleteMissionPatch.cs`

**Target**: `TICouncilorState.SetCompletedMission()`

**Type**: Prefix

**Behavior**: Clears bonuses when assist mission completes

**Logic**:
```csharp
AssistBonusTracker.RemoveBonuses(__instance);
```

---

### CouncilorView_GetAttributeStringPatch

**File**: `CouncilorView_GetAttributeStringPatch.cs`

**Target**: `CouncilorView.GetAttributeString()`

**Type**: Postfix

**Behavior**: Appends bonus to attribute display string

**Logic**:
1. Gets base stat from original return value
2. Queries `AssistBonusTracker.GetStatBonus()`
3. Formats as: `"base +bonus"` in orange color
4. Returns modified string

---

### CouncilGridController_SetStatValuePatch

**File**: `CouncilGridController_SetStatValuePatch.cs`

**Target**: `CouncilGridController.SetStatValue()`

**Type**: Postfix

**Behavior**: Updates UI text fields with combined stat totals

**Key Method**:
```csharp
private static string FormatOrange(string text)
{
	return "<color=#FFA500FF>" + text + "</color>";
}
```

---

### CouncilorMissionCanvasController_UpdateModifierListPatch

**File**: `CouncilorMissionCanvasController_UpdateModifierListPatch.cs`

**Target**: `CouncilorMissionCanvasController.UpdateModifierList()`

**Type**: Postfix

**Behavior**: Safely displays modifier information without crashes

---

## Data Structures

### Bonus Storage

Internal dictionary in `AssistBonusTracker`:

```csharp
private static Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>> bounciesByCouncilor
```

**Structure**:
```
Councilor A
├─ Persuasion: 2
├─ Command: 1
└─ Administration: 1

Councilor B
└─ Espionage: 3
```

---

## Settings

### Settings.cs

```csharp
public class Settings : UnityModManager.ModSettings
{
	public float assistPercentage = 25f;      // 0-100, percentage of stat to grant
	public bool enableAssistMission = true;   // Toggle mission availability
	public bool debugLogging = false;         // Enable debug output
}
```

Access via: `Main.settings`

---

## Enumerations & Types

### CouncilorAttribute

From game API, represents the 7 councilor attributes:

```csharp
public enum CouncilorAttribute
{
	Persuasion,      // 0 - Diplomacy and influence
	Investigation,   // 1 - Research and information
	Espionage,       // 2 - Spying and subterfuge
	Command,         // 3 - Military and leadership
	Administration,  // 4 - Organization and management
	Science,         // 5 - Scientific research
	Security         // 6 - Defense and protection
}
```

### MissionOutcome

From game API:

```csharp
public enum MissionOutcome
{
	Success,
	Failure
}
```

### MissionContext

From game API, mission availability contexts:

```csharp
public enum MissionContext
{
	Unlimited,        // Available everywhere
	Location,         // At specific location
	// ... others
}
```

---

## Integration Points

### How to Get Mod Instance

```csharp
if (Main.mod != null)
{
	Main.mod.Logger.Log("Message");
	Main.mod.Logger.Error("Error message");
}
```

### How to Access Settings

```csharp
float assistPercentage = Main.settings.assistPercentage;
bool enabled = Main.settings.enableAssistMission;
```

### How to Check Debug Logging

```csharp
if (Main.settings.debugLogging)
{
	Main.mod.Logger.Log("[Component] Debug info");
}
```

---

## Common Usage Patterns

### Calculate Bonus in Custom Code

```csharp
int councilStat = 8;
float assistPercentage = Main.settings.assistPercentage / 100f;
int bonus = Mathf.RoundToInt(councilStat * assistPercentage);
// Result: 8 * 0.25 = 2
```

### Get Total Bonus for Contested Mission

```csharp
TICouncilorState attacker = mission.GetAttackingCouncilor();
int totalBonus = AssistBonusTracker.GetTotalBonus(attacker);
int modifiedResult = baseResult + totalBonus;
```

### Clear Bonuses on Event

```csharp
// When councilor changes faction or other cleanup
AssistBonusTracker.RemoveBonuses(councilor);
```

### Log Conditional Debug Info

```csharp
if (Main.mod != null && Main.settings.debugLogging)
{
	Main.mod.Logger.Log($"[PatchName] Variable={variable}");
}
```

---

## Extension Examples

### Adding a New Stat Requirement

Create new condition:

```csharp
public class TIMissionCondition_MinimumStat : TIMissionCondition
{
	private CouncilorAttribute minimumStat;
	private int minimumValue;

	public override string Validate(TIMissionTemplate mission, 
		TICouncilorState councilor, TIGameState target)
	{
		int targetStat = (target as TICouncilorState)?.GetAttribute(minimumStat) ?? 0;
		if (targetStat < minimumValue)
			return $"Target stat too low";
		return null;
	}
}
```

Add to mission in `TIMissionTemplate_Assist`:

```csharp
this.conditions.Add(new TIMissionCondition_MinimumStat());
```

### Adding Bonus Expiration

Extend tracking:

```csharp
public struct BonusRecord
{
	public int Amount;
	public TIDateTime ExpirationTime;

	public bool IsExpired(TIDateTime currentTime)
	{
		return currentTime >= ExpirationTime;
	}
}
```

Modify `GetStatBonus` to check expiration:

```csharp
public static int GetStatBonus(TICouncilorState councilor, CouncilorAttribute stat)
{
	// Check expiration and remove if expired
	var expired = bonuses.Where(b => b.Value.IsExpired(TIDateTime.Now)).ToList();
	foreach (var bonus in expired)
	{
		bonuses.Remove(bonus.Key);
	}
	// Return remaining bonus
}
```
