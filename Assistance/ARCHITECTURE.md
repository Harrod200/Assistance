# Assistance Mod - Architecture Documentation

## System Overview

The Assistance mod implements a bonus tracking system that temporarily enhances councilor stats without modifying base attributes. The architecture follows a layered pattern:

```
┌─────────────────────────────────────────────────────────┐
│              Mission Definition Layer                    │
│  (TIMissionTemplate_Assist, Conditions, Effects)        │
└──────────────────┬──────────────────────────────────────┘
				   │
┌──────────────────▼──────────────────────────────────────┐
│            Bonus Application Layer                       │
│  (TIMissionEffect_Assist, AssistBonusTracker)           │
└──────────────────┬──────────────────────────────────────┘
				   │
┌──────────────────▼──────────────────────────────────────┐
│           Game Integration Layer                         │
│  (HarmonyLib patches to game methods)                    │
└──────────────────┬──────────────────────────────────────┘
				   │
┌──────────────────▼──────────────────────────────────────┐
│              UI Display Layer                            │
│  (Display patches, Settings UI)                          │
└─────────────────────────────────────────────────────────┘
```

## Component Interactions

### 1. Mission Initialization Flow

```
Main.Load()
  └─> Harmony.PatchAll()
	  ├─> AssistMissionBootstrapPatch.Initialize()
	  │   ├─> RegisterMissionTemplate()
	  │   │   └─> Create TIMissionTemplate_Assist instance
	  │   └─> GrantToAllCouncilors()
	  │       └─> Add mission to all councilor types
	  │
	  └─> All other patches applied
```

### 2. Mission Assignment & Resolution Flow

```
Player assigns Assist mission to Councilor A, targeting Councilor B

SetMissionData(mission, councilor)
  └─> Validate conditions
	  ├─> TIMissionCondition_MyFactionCouncilor ✓
	  ├─> TIMissionCondition_PlayerFactionOnly ✓
	  └─> TIMissionCondition_NotCurrentlyAssisting ✓

Mission Resolution (automatic)
  └─> FinalizeCouncilorMissions.StaggerMissionResolutions()
	  └─> AssistPriorityPatch: Sort assist missions to resolve first
		  └─> TIMissionResolution_Automatic.Resolve()
			  └─> Success (no opposition)
				  └─> TIMissionEffect_Assist.ApplyEffect()
					  └─> Calculate bonus for each stat
						  └─> AssistBonusTracker.RecordBonus()
```

### 3. Contested Mission Integration

```
Councilor B (with assistance bonus) engages in contested mission

TIMissionResolution_Contested.SumAttackingModifiers/SumDefendingModifiers()
  └─> TIMissionResolution_Contested_AssistBonusPatch (Postfix)
	  └─> Is this councilor being assisted?
		  └─> Yes: AssistBonusTracker.GetTotalBonus(councilor)
			  └─> Add bonus to modifier result
```

### 4. UI Display Flow

```
Player opens CouncilorView

CouncilorView.GetAttributeString(attribute)
  └─> CouncilorView_GetAttributeStringPatch (Postfix)
	  ├─> Get base stat value
	  ├─> AssistBonusTracker.GetStatBonus(councilor, attribute)
	  ├─> Combine: base + bonus
	  └─> Format in orange color for display

CouncilGridController.SetStatValue(attribute, value)
  └─> CouncilGridController_SetStatValuePatch (Postfix)
	  ├─> Get base stat
	  ├─> AssistBonusTracker.GetStatBonus()
	  ├─> Update text field with combined total
	  └─> Render in grid UI
```

### 5. Mission Completion & Bonus Cleanup

```
Mission completes

TICouncilorState.SetCompletedMission()
  └─> TICouncilorState_CompleteMissionPatch (Prefix)
	  └─> Is this an Assist mission?
		  └─> Yes: AssistBonusTracker.RemoveBonuses(councilor)
			  └─> Clear all tracked bonuses for this councilor
```

## Data Flow Diagram

### Bonus Tracking System

```
AssistBonusTracker (Static Dictionary)
│
├─ Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>>
│  └─ Councilor ID → Attribute Bonuses
│
├─ RecordBonus(councilor, attribute, amount)
│  └─ Stores bonus in dictionary
│
├─ GetStatBonus(councilor, attribute)
│  └─ Retrieves bonus for specific stat
│
├─ GetTotalBonus(councilor)
│  └─ Sums all bonuses for councilor
│
└─ RemoveBonuses(councilor)
   └─ Clears all bonuses for councilor
```

### Bonus Storage Pattern

```
AssistBonusTracker.bounciesByCouncilor
{
  [Councilor_A]: {
	CouncilorAttribute.Persuasion: 2,
	CouncilorAttribute.Command: 1
  },
  [Councilor_B]: {
	CouncilorAttribute.Espionage: 3
  }
}
```

## Patch Categories

### Registration & Initialization Patches

| Patch | Target | Purpose | Type |
|-------|--------|---------|------|
| AssistMissionBootstrapPatch | SolarSystemBootstrap.Initialize | Register mission template and grant to all councilors | Postfix |

### Mission Resolution Patches

| Patch | Target | Purpose | Type |
|-------|--------|---------|------|
| FinalizeCouncilorMissions_AssistPriorityPatch | FinalizeCouncilorMissions.StaggerMissionResolutions | Prioritize assist missions to resolve first | Postfix |
| TIMissionResolution_Contested_AssistBonusPatch | TIMissionResolution_Contested.SumAttackingModifiers, SumDefendingModifiers | Apply assist bonuses to contested mission calculations | Postfix |

### Mission Filtering Patches

| Patch | Target | Purpose | Type |
|-------|--------|---------|------|
| TICouncilorState_GetPossibleMissionListPatch | TICouncilorState.GetPossibleMissionList | Remove assist mission from AI councilors | Postfix |

### Mission Completion Patches

| Patch | Target | Purpose | Type |
|-------|--------|---------|------|
| TICouncilorState_CompleteMissionPatch | TICouncilorState.SetCompletedMission | Clear bonuses when mission completes | Prefix |

### UI Display Patches

| Patch | Target | Purpose | Type |
|-------|--------|---------|------|
| CouncilorView_GetAttributeStringPatch | CouncilorView.GetAttributeString | Display combined stats (base + bonus) in orange | Postfix |
| CouncilGridController_SetStatValuePatch | CouncilGridController.SetStatValue | Update grid display with combined totals | Postfix |
| CouncilorMissionCanvasController_UpdateModifierListPatch | CouncilorMissionCanvasController.UpdateModifierList | Safely display modifier information | Postfix |

## Mission Condition Validation

When a player selects a target for Assist mission, three conditions are checked:

```
Is valid target?
├─ TIMissionCondition_MyFactionCouncilor
│  └─ target.faction == assigner.faction
│     └─ AND target != assigner (prevent self-target)
│
├─ TIMissionCondition_PlayerFactionOnly
│  └─ faction.IsPlayerControlled
│
└─ TIMissionCondition_NotCurrentlyAssisting
   └─ target is NOT currently assigned to assist mission
```

## Contested Mission Bonus Application

### Attack Scenario

```
Councilor A (assisted) attacks Councilor C

TIMissionResolution_Contested.SumAttackingModifiers(mission, a_councilor, target)
  └─> Calculate base attacking modifiers
  └─> TIMissionResolution_Contested_AssistBonusPatch (Postfix)
	  └─> assistBonus = AssistBonusTracker.GetTotalBonus(a_councilor)
	  └─> result += assistBonus
	  └─> return modified result
```

### Defense Scenario

```
Councilor B (assisted) defends against Councilor A

TIMissionResolution_Contested.SumDefendingModifiers(mission, defender, target)
  └─> Calculate base defending modifiers
  └─> TIMissionResolution_Contested_AssistBonusPatch (Postfix)
	  └─> targetCouncilor = target as TICouncilorState
	  └─> assistBonus = AssistBonusTracker.GetTotalBonus(targetCouncilor)
	  └─> result += assistBonus
	  └─> return modified result
```

## AI Faction Safety

AI factions cannot use Assist missions due to:

1. **Modifier Complexity**: AI pathfinding and decision-making doesn't account for temporary bonuses
2. **State Management**: AI could crash when evaluating modifiers that reference bonus tracker
3. **Mission Filtering**: `TICouncilorState_GetPossibleMissionListPatch` removes mission from AI councilors

```
GetPossibleMissionList(councilor)
  └─> Collect all possible missions
  └─> TICouncilorState_GetPossibleMissionListPatch (Postfix)
	  └─> Is AI faction?
		  └─ Yes: Remove all Assist missions from list
		  └─ No: Keep list as-is
```

## Configuration & Persistence

```
UnityModManager
  └─> Loads Settings on startup
	  ├─ assistPercentage (float, default 25.0)
	  ├─ enableAssistMission (bool, default true)
	  └─ debugLogging (bool, default false)

  └─> OnSaveGUI() persists settings to disk
```

## Error Handling & Logging

All patches include try-catch blocks:

```
try
{
	// Patch logic
}
catch (Exception ex)
{
	if (Main.mod != null)
	{
		Main.mod.Logger.Error("[ComponentName] Error: " + ex.Message);
	}
}
```

Debug logging can be enabled for troubleshooting:

```
if (Main.mod != null)
{
	Main.mod.Logger.Log("[ComponentName] Debug info: " + details);
}
```

## Key Design Decisions

### 1. Separate Bonus Tracking vs. Base Modification

**Decision**: Store bonuses separately in `AssistBonusTracker` instead of modifying councilor base attributes.

**Rationale**:
- Bonuses are **temporary** and must be cleared cleanly
- Avoids polluting game state
- Prevents interference with save/load systems
- Simplifies debugging

### 2. Mission Resolution Priority

**Decision**: Patch `StaggerMissionResolutions` to prioritize Assist missions to resolve first.

**Rationale**:
- Ensures bonuses are applied before contested missions using them
- Prevents race conditions
- Guarantees consistent bonus application timing

### 3. AI Exclusion

**Decision**: Filter Assist missions from AI councilors.

**Rationale**:
- AI decision-making doesn't account for temporary bonuses
- Prevents crashes when evaluating mission modifiers
- Simplifies AI behavior

### 4. Flat Bonus Application

**Decision**: Apply bonuses as flat points in contested missions, not multiplicative.

**Rationale**:
- Simpler calculation
- More predictable results
- Matches other modifier applications in game

## Extension Points

To extend the mod, consider these areas:

1. **New Bonus Sources**: Extend `TIMissionEffect` to other mission types that grant bonuses
2. **UI Enhancements**: Add visual indicators or animations for active bonuses
3. **Bonus Types**: Create specialized bonus tracking for different bonus categories
4. **Mission Variations**: Create variants of Assist mission with different parameters
5. **AI Support**: Implement AI-safe bonus calculations to enable AI use

## Performance Considerations

- **Bonus Tracking**: O(1) lookup by councilor and attribute using dictionary
- **Patch Overhead**: Minimal - postfix patches only execute after original method
- **UI Updates**: Only recompute when councilor stats are displayed
- **Memory**: Single static dictionary, scales with active councilors (typically < 100)

## Testing Considerations

Key areas to test:

1. **Mission Assignment**: Can only target valid councilors
2. **Bonus Calculation**: Correct percentage applied
3. **UI Display**: Bonuses show in correct orange color
4. **Contested Missions**: Bonuses correctly modify results
5. **Completion**: Bonuses cleared after mission ends
6. **AI Safety**: No crashes when AI evaluates missions
7. **Settings**: Adjusting percentage affects bonus calculation
8. **Enabling/Disabling**: Mission disappears/reappears with toggle
