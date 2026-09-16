# Mission Breakdown Display Fix - Implementation Summary

## Issues Addressed

### 1. **Base Stats Counted Twice**
The breakdown was displaying base stats separately AND including them in the modifiers list, resulting in double-counting:
- **Before**: Base Espionage: 14, then Modifiers including Espionage: 14
- **After**: Only modifiers listed (which already include the base stat)

### 2. **Defending Stats Missing for Control Points**
When attacking control points, defending modifiers like "Council Administration", "Size of National Economy", "Difference in Ideology", and "Popular Support" were being skipped with "Object reference not set to an instance of an object" errors because:
- The code was passing `targetCouncilor` (null for control points) to modifier calculations
- No fallback was available for control point defenders

## Root Cause Analysis

From the logs:
```
[MissionDetailBreakdown] Attacking modifiers count: 3
  - Espionage          (base stat)
  - Popular Support
  - Target Over Control Point Cap

[MissionDetailBreakdown] Defending modifiers count: 6
  - Mission Difficulty
  - Council Administration
  - Size of National Economy
  - Difference in Ideology
  - Popular Support
  - Difficulty: Forgiving

[MissionDetailBreakdown] Skipping defending modifier 'Size of National Economy' (incompatible with target type): Object reference not set to an instance of an object
```

The modifiers returned by `GetAttackingNonZeroModifiers()` and `GetDefendingNonZeroModifiers()` already include the base stat as the first modifier. Displaying it separately and then adding all modifiers was double-counting.

## Changes Made

### File: `TINotificationQueueState_MissionDetailBreakdownPatch.cs`

#### 1. **Updated Method Signature**
- Added `TIFactionState defendingFaction` parameter to `BuildMissionBreakdown()` method
- Updated the call to pass `heldTargetFaction` from the patch parameters

#### 2. **Removed Duplicate Base Stat Display for Attacking**
```csharp
// BEFORE:
breakdown.AppendFormat("  Base {0}: {1:0.00}\n", primaryAttackerStat, baseAttackValue);
// Then added BaseValue + ModifierTotal + AssistBonus

// AFTER:
// Removed separate base stat display
// Now just show modifiers (which already include base stat) + assist bonus
```

#### 3. **Removed Duplicate Base Stat Display for Defending**
```csharp
// BEFORE:
breakdown.AppendFormat("  Base {0}: {1:0.00}\n", primaryDefenderStat, baseDefenseValue);
// Or for control points:
breakdown.AppendFormat("  Base Defense: {0:0.00} (control point baseline)\n", baselineDifficulty);

// AFTER:
// Removed all base defense display
// Modifiers already include the base stat
```

#### 4. **Fixed Control Point Defending Modifiers**
- Added logic to use `defendingFaction` parameter when target is a control point
- Attempts to extract a defending councilor from the faction's active councilors
- Falls back gracefully if no defending councilor is found
- Uses reflection to access `activeCouncilors` property (handles potential API variations)

#### 5. **Cleaned Up Unused Variables**
- Removed unused `baseAttackValue` variable
- Removed unused `primaryAttackerStat` and `primaryDefenderStat` variables
- Simplified the calculation to only what's needed

## Expected Results

After applying this fix:

**Before (Wrong):**
```
ATTACKING:
  Attacker: Moustapha Aboud (Espionage)
  Base Espionage: 14.00
  Assist Bonus: +X.XX
  Modifiers:
	• Espionage: +14.00       ← DUPLICATE!
	• Popular Support: +Y.YY
	• Target Over CP Cap: +Z.ZZ
  Total Attack: 14.00 + 14.00 + Y.YY + Z.ZZ + X.XX = TOO HIGH!

DEFENDING:
  Defender: European Union
  Base Defense: 15.00
  Modifiers:
	• Mission Difficulty: +10.00
	• Council Administration: SKIPPED (error)
	• Size of National Economy: SKIPPED (error)
	• Difference in Ideology: SKIPPED (error)
	• Popular Support: SKIPPED (error)
	• Difficulty: Forgiving: +2.00
  Total Defense: 10.00 + 2.00 = TOO LOW! (missing 4 modifiers)
```

**After (Fixed):**
```
ATTACKING:
  Attacker: Moustapha Aboud
  Modifiers:
	• Espionage: +14.00
	• Popular Support: +Y.YY
	• Target Over CP Cap: +Z.ZZ
	• Assist Bonus: +X.XX
  Total Attack: 14.00 + Y.YY + Z.ZZ + X.XX = CORRECT!

DEFENDING:
  Defender: European Union
  Modifiers:
	• Mission Difficulty: +10.00
	• Council Administration: +Z.ZZ
	• Size of National Economy: +W.WW
	• Difference in Ideology: +V.VV
	• Popular Support: +U.UU
	• Difficulty: Forgiving: +2.00
  Total Defense: 10.00 + Z.ZZ + W.WW + V.VV + U.UU + 2.00 = CORRECT!
```

## Testing

The fix has been built and deployed to the Steam mod folder:
- **Compiled**: Release build successful
- **Deployed**: `Assistance.dll` (47 KB) copied to `C:\Games\Steam\steamapps\common\Terra Invicta\Mods\Enabled\Assistance Mission\`
- **Timestamp**: 16/09/2026 20:41:04

## Next Steps

1. Launch TerraInvicta with the updated mod
2. Run missions against control points (e.g., "Purge" mission against "European Union")
3. Check mission end-of-mission breakdown:
   - Verify base stats are NOT shown twice
   - Verify all 6 defending modifiers are displayed (not skipped)
   - Verify Total Attack and Total Defense values make sense

The logs will show "Successfully built breakdown" and should no longer show "Skipping defending modifier" errors.
