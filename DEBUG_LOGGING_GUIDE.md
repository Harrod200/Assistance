# Aggressive Debug Logging Added

## Summary
Added extensive debug logging to trace mission resolution and breakdown display for both councilor and control point targets.

## Changes Made

### 1. TINotificationQueueState_MissionDetailBreakdownPatch.cs
**Patch Entry & Exit:** 
- Added separator lines (═══════) to clearly mark patch entry/exit
- Logs mission name, outcome type, and contested status early

**Queue/Notification Checks:**
- Log notification queue size
- Detailed mismatch logging if notification doesn't match mission
- Shows notification type when mismatched

**Breakdown Building:**
- Logs when breakdown building starts and completes
- Shows mission template, councilor, target details
- Shows target type (class name), whether it's TICouncilorState or TIControlPoint

**Modifier Retrieval:**
- Logs when calling GetAttackingNonZeroModifiers()
- Logs attacking modifier count and names
- Logs when calling GetDefendingNonZeroModifiers()
- Logs defending modifier count and names

**Completion:**
- Success log with breakdown character count
- Warnings if breakdown returns empty string
- Clear exit separator

### 2. TIMissionResolution_Contested_AssistBonusPatch.cs

**SumAttackingModifiers_Postfix:**
- Entry/exit separators (═══════) for clarity
- Logs mission, attacker, target type, target name, result before
- Checks if target is TICouncilorState or TIControlPoint
- Logs mission's attacking attribute
- Logs retrieved stat bonus value
- Shows "NO BONUS" if bonus <= 0 with reason
- Shows "✓ APPLIED" with bonus type and point amount
- Shows before/after result values

**SumDefendingModifiers_Postfix:**
- Identical logging structure as attacking
- Critical for understanding control point defensive bonus application
- Shows target type clearly (TICouncilorState vs TIControlPoint)

### 3. AssistBonusTracker.cs

**GetStatBonus():**
- Logs when called with NULL councilor
- Logs when councilor has no bonuses at all
- Logs when councilor has other stats but not the requested one
- Shows retrieved bonus value with stat type

## What This Logs

When a mission completes, you'll see:

### For Councilor-vs-Councilor Contested Mission:
```
═══════════════════════════════════════════════════════════════
[MissionDetailBreakdown] ===== BREAKDOWN PATCH ENTRY =====
[MissionDetailBreakdown] Mission: <mission name>
[MissionDetailBreakdown] Result outcome: <outcome>
[MissionDetailBreakdown] Is contested: True
[MissionDetailBreakdown] Processing contested mission: <mission>
[MissionDetailBreakdown] Notification queue has X items
[MissionDetailBreakdown] Notification found, building breakdown...
[MissionDetailBreakdown] ===== BUILD BREAKDOWN START =====
[MissionDetailBreakdown] Mission template: <template>
[MissionDetailBreakdown] Councilor (attacker): <attacker name>
[MissionDetailBreakdown] Target: <target name>
[MissionDetailBreakdown] Target type: TICouncilorState
[MissionDetailBreakdown] Target is councilor: True
[MissionDetailBreakdown] Validation passed, retrieving modifiers...
[MissionDetailBreakdown] Calling GetAttackingNonZeroModifiers...
[MissionDetailBreakdown] Attacking modifiers count: X
  - <modifier 1>
  - <modifier 2>
  ...
[MissionDetailBreakdown] Calling GetDefendingNonZeroModifiers...
[MissionDetailBreakdown] Defending modifiers count: Y
  - <modifier 1>
  ...
[MissionDetailBreakdown] SUCCESS: Enhanced mission with breakdown
[MissionDetailBreakdown] Breakdown length: XXX chars
[MissionDetailBreakdown] ===== BREAKDOWN PATCH EXIT =====
```

### Also During Resolution:
```
═══════════════════════════════════════════════════════════════
[AssistBonusTracker] SumDefendingModifiers ENTRY
  Mission: <mission>
  Defender: <defender>
  Target type: TICouncilorState
  Target: <target>
  Result before: X.XX
  Target is TICouncilorState: True
  Target is TIControlPoint: False
  Mission defending attribute: Command
  Stat bonus for Command: 5
  ✓ APPLIED Command assist bonus (5 points) to defending modifier
  Result changed: X.XX → Y.YY
═══════════════════════════════════════════════════════════════
```

### For Control Point Target Missions:
```
[MissionDetailBreakdown] Target type: TIControlPoint
[MissionDetailBreakdown] Target is councilor: False

...

[AssistBonusTracker] SumDefendingModifiers ENTRY
  Target type: TIControlPoint
  Target is TICouncilorState: False
  Target is TIControlPoint: True
```

## Debugging Guide

### Mission Breakdown Not Showing?
Look for:
1. "Notification queue is null or empty" - notification system may be broken
2. "Notification mismatch" - different mission than expected
3. "VALIDATION FAILED" - missing councilor, target, or not contested
4. "Attacking modifiers count: 0" OR "Defending modifiers count: 0" - no modifiers found
5. "Failed to build breakdown - returned empty string" - exception in BuildMissionBreakdown

### Control Point Bonuses Not Applying?
Look for:
1. "Target type: TIControlPoint" + "NO BONUS: <stat> assist bonus is 0" - no bonus recorded
2. "Target is TIControlPoint: False" when it should be True - target type detection issue
3. "Mission defending attribute: <stat>" - verify this is the correct stat for the mission
4. "Stat bonus for <stat>: 0" - check if bonus was recorded at all in AssistBonusTracker

### To Enable Debug Logging
- Must have `debugLogging` enabled in mod settings
- Look in Output → Assistance (or mod log file)

## Build Status
✅ Project builds successfully - no compilation errors
