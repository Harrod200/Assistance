# Assistance Mod

A Terra Invicta mod that adds an **Assist Councilor** mission, allowing players to temporarily boost a councilor's stats by assigning another councilor to help them.

## Overview

The Assistance mod introduces a cooperative mission system where councilors can support each other by providing temporary stat boosts. This mission is:

- **Support-focused**: Helps allied councilors without combat or opposition
- **Configurable**: Adjustable assistance percentage and enable/disable toggle
- **Integrated**: Seamlessly works with the existing game mission system
- **Bonus-tracking**: Maintains temporary stat bonuses separate from base attributes

## Features

### Core Gameplay
- **Assist Councilor Mission**: Assign a councilor to temporarily boost another councilor's stats
- **Stat-based Assistance**: Bonus amount is based on the assisting councilor's stats (multiplied by configurable percentage)
- **Temporary Bonuses**: Bonuses apply only during the assistance period and are cleared when the mission completes
- **All Stats Supported**: Works with all 7 councilor attributes
  - Persuasion
  - Investigation
  - Espionage
  - Command
  - Administration
  - Science
  - Security

### Mission Behavior
- **Automatic Resolution**: Guaranteed success with no opposition or dice rolls
- **Fast Resolution**: Resolves first each turn for reliable application
- **Target Validation**: Prevents self-targeting and targets already receiving assistance
- **Player-Only**: Available only to player-controlled factions (AI cannot use)
- **Mission Context**: Works in unlimited contexts, similar to Inspire missions

### UI Integration
- **In-Game Display**: Shows stat bonuses with visual indicators in orange
- **Settings Panel**: Configure assistance percentage and enable/disable the mission
- **Debug Logging**: Optional debug output for troubleshooting

## Configuration

### Settings
Located in the mod settings panel:

- **Assist Percentage (0-100%)**: Percentage of assisting councilor's stat to grant as bonus (default: 25%)
- **Enable Assist Mission**: Toggle the availability of the Assist mission (default: enabled)
- **Enable Debug Logging**: Show debug logs in the output (default: disabled)

### Localization
Mission text is defined in `TIMissionTemplate.en`:
- Display name and description
- Success/failure messages

## Project Architecture

### Core Components

**Mission Definition**
- `TIMissionTemplate_Assist.cs`: Mission template with game mechanics configuration

**Mission Logic**
- `TIMissionEffect_Assist.cs`: Applies stat bonuses when mission completes
- `TIMissionCondition_*.cs`: Validates mission targets
- `TIMissionModifier_*.cs`: Provides modifiers for contested missions

**Integration Patches** (HarmonyLib)
- `AssistMissionBootstrapPatch.cs`: Registers mission and grants to councilors
- `TIMissionResolution_Contested_AssistBonusPatch.cs`: Applies bonuses to contested mission calculations
- `FinalizeCouncilorMissions_AssistPriorityPatch.cs`: Ensures assist missions resolve first
- `TICouncilorState_GetPossibleMissionListPatch.cs`: Filters missions for AI factions
- `TICouncilorState_CompleteMissionPatch.cs`: Clears bonuses on mission completion

**UI Display Patches**
- `CouncilorView_GetAttributeStringPatch.cs`: Shows combined stat totals in councilor view
- `CouncilGridController_SetStatValuePatch.cs`: Updates grid display with bonus information
- `CouncilorMissionCanvasController_UpdateModifierListPatch.cs`: Displays modifiers safely

**Bonus System**
- `AssistBonusTracker.cs`: Central tracking for temporary stat bonuses

**Entry Point**
- `Main.cs`: Mod initialization and settings UI

## How It Works

### Mission Execution Flow

1. **Player assigns** Assist mission to Councilor A, targeting Councilor B
2. **Mission resolves** automatically (no opposition)
3. **TIMissionEffect_Assist** applies:
   - Calculates bonus = (assisting councilor's stat) × (configured percentage)
   - Records bonus in `AssistBonusTracker`
4. **UI updates** to show combined stat totals (base + bonus) in orange
5. **Contested missions** involving Councilor B automatically include assist bonus in calculations
6. **Mission completes** and bonuses are cleared

### Bonus Calculation Example

If Councilor A has 8 Persuasion and assists Councilor B:
- Assist Percentage: 25%
- Bonus = 8 × 0.25 = 2 Persuasion for Councilor B
- Councilor B displays: Base (e.g., 6) + Bonus (2) = 8 total

### Contested Mission Integration

When Councilor B participates in a contested mission while receiving assistance:
- **As attacker**: Assist bonus is added to attacking modifiers
- **As defender**: Assist bonus is added to defending modifiers
- Bonuses help resolve contested missions favorably

## Technical Details

### Harmony Patches
The mod uses [HarmonyLib](https://github.com/pardeike/Harmony) to patch game methods without modifying game files. Key patches include:

| Class | Method | Purpose |
|-------|--------|---------|
| SolarSystemBootstrap | Initialize | Register mission template |
| TIMissionResolution_Contested | SumAttackingModifiers | Apply assist bonuses to attacks |
| TIMissionResolution_Contested | SumDefendingModifiers | Apply assist bonuses to defense |
| FinalizeCouncilorMissions | StaggerMissionResolutions | Prioritize assist missions |
| TICouncilorState | GetPossibleMissionList | Filter missions for AI |
| TICouncilorState | SetCompletedMission | Clear bonuses on completion |
| CouncilorView | GetAttributeString | Display combined stats |
| CouncilGridController | SetStatValue | Update grid display |

### Mission Conditions
Three mission conditions restrict valid targets:

1. **MyFactionCouncilor**: Target must be same faction
2. **PlayerFactionOnly**: Mission only available for player-controlled factions
3. **NotCurrentlyAssisting**: Prevents multiple assist missions on same target

### Bonus Tracking System
`AssistBonusTracker` maintains a dictionary of temporary bonuses:
- Key: Councilor ID
- Value: Dictionary of stat bonuses by attribute
- Bonuses persist only until mission completion
- Separate from base attributes to avoid permanent changes

## Files & Responsibilities

| File | Lines | Purpose |
|------|-------|---------|
| TIMissionTemplate_Assist.cs | 82 | Mission template configuration |
| TIMissionEffect_Assist.cs | 94 | Apply bonuses on mission success |
| TIMissionCondition_*.cs | ~20-40 | Target validation logic |
| TIMissionModifier_*.cs | ~40 | Mission modifier calculations |
| AssistBonusTracker.cs | 143 | Central bonus tracking |
| AssistMissionBootstrapPatch.cs | 166 | Register mission and grant to councilors |
| TIMissionResolution_Contested_AssistBonusPatch.cs | 139 | Integrate bonuses into contested missions |
| FinalizeCouncilorMissions_AssistPriorityPatch.cs | 165 | Prioritize assist mission resolution |
| TICouncilorState_GetPossibleMissionListPatch.cs | 52 | Filter missions for AI safety |
| TICouncilorState_CompleteMissionPatch.cs | 33 | Clear bonuses on completion |
| CouncilorView_GetAttributeStringPatch.cs | 60 | Display combined stats |
| CouncilGridController_SetStatValuePatch.cs | 139 | Update grid UI with bonuses |
| CouncilorMissionCanvasController_UpdateModifierListPatch.cs | 111 | Safely display modifiers |
| Settings.cs | 16 | Configuration storage |
| Main.cs | 81 | Mod entry point and settings UI |
| TIMissionTemplate.en | 11 | Localization strings |

## Dependencies

- **HarmonyLib**: For patching game methods
- **UnityModManager**: For mod framework and settings persistence
- **Terra Invicta**: Game API and classes

## Notes

- Bonuses are **temporary** and only last while the assist mission is active
- Assist missions can be prioritized to resolve before other missions
- Bonuses are applied via a separate tracking system, not by modifying base councilor attributes
- The mod is compatible with player-controlled factions only; AI cannot use assist missions

## Known Limitations

- Assist missions are not available for AI-controlled factions (by design for stability)
- Bonuses apply only to direct stat calculations, not computed values derived from stats
- Debug logging can be enabled in settings for troubleshooting

## Version History

- **v0.6.0+**: Bonus system redesigned to use separate tracking instead of base attribute modification
- **v0.5.5**: Initial stable release with contested mission integration
