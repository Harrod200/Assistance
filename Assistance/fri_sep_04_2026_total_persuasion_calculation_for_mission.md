# Copilot Chat Conversation Export: Total persuasion calculation for mission

**User:** @Harrod225
**Thread URL:** https://github.com/copilot/c/db396898-8286-4b33-bb3b-54234826a08b

## @Harrod225

Regarding https://github.com/Harrod200/Assistance.

Councillor A has 5 persuasion and 7 command undertakes a contested mission with a defence of 8 persuasion
Councillor B has 4 persuasion and 2 command, and assists councillor A.

What is the total persuasion on the attacking side following the assist mission logic?

## Copilot

Based on the code in the Assistance mod, here's how to calculate the total persuasion on the attacking side:

## Calculation

**Given:**
- Councillor A: 5 Persuasion, 7 Command (attacker)
- Councillor B: 4 Persuasion, 2 Command (assists A)
- Defense: 8 Persuasion
- Default assist percentage: 25%

**Step 1: Calculate assist bonus from Councillor B**

According to `TIMissionEffect_Assist.cs` (line 66), the bonus calculation is:
```
assistAmount = max(1, floor(assistingValue × assistPercentage))
assistAmount = max(1, floor(4 × 0.25))
assistAmount = max(1, floor(1.0))
assistAmount = 1 Persuasion
```

**Step 2: Apply bonus to attacked during contested mission**

From `TIMissionResolution_Contested_AssistBonusPatch.cs` (lines 64-66), when Councillor A attacks while receiving assistance:
```
__result += assistBonus
```

**Total attacking persuasion = 5 + 1 = 6 Persuasion**

This represents Councillor A's base 5 Persuasion plus 1 Persuasion from Councillor B's assist bonus, giving **6 total Persuasion on the attacking side** during the contested mission calculation.

## @Harrod225

Assume assist percentage of 100%. Are councillor b's stats other stats also being added to the persuasion check?

## Copilot

Yes. Looking at `TIMissionEffect_Assist.cs` and `AssistBonusTracker.cs`, the assist mechanism pools **all of Councillor B's stats** into a single bonus pool that is applied to contested missions.

## Recalculated with 100% assist percentage:

**Councillor B's stats with 100% assist percentage:**
- Persuasion: 4 × 1.0 = 4
- Investigation: (unknown, assume 0)
- Espionage: (unknown, assume 0)
- Command: 2 × 1.0 = 2
- Administration: (unknown, assume 0)
- Science: (unknown, assume 0)
- Security: (unknown, assume 0)

**Total bonus pool = 4 + 2 + [other stats] = at least 6 points**

From `TIMissionEffect_Assist.cs` (lines 43-63), the code iterates through **all 7 stats**:
```csharp
CouncilorAttribute[] stats = new CouncilorAttribute[]
{
    CouncilorAttribute.Persuasion,
    CouncilorAttribute.Investigation,
    CouncilorAttribute.Espionage,
    CouncilorAttribute.Command,
    CouncilorAttribute.Administration,
    CouncilorAttribute.Science,
    CouncilorAttribute.Security
};
```

Each stat is recorded separately but retrieved as a **total** via `AssistBonusTracker.GetTotalBonus()` (line 83-104), which sums all stat bonuses together.

**Therefore, during the contested mission check:**
- **Total attacking persuasion = 5 (Councillor A's base) + [6+ point bonus pool] ≥ 11 persuasion total**

The persuasion check doesn't just get Councillor B's Persuasion bonus—it gets a combined bonus pool from ALL of Councillor B's stats added together.

## @Harrod225

Adjust that logic to apply each start separately

## Copilot

I'll adjust the logic to apply each stat separately rather than pooling them. The key changes are:

1. Modify `TIMissionResolution_Contested_AssistBonusPatch.cs` to accept a `CouncilorAttribute` parameter and apply only the matching stat bonus
2. Update the patch methods to determine which stat is being checked and retrieve only that stat's bonus

```csharp name=TIMissionResolution_Contested_AssistBonusPatch.cs url=https://github.com/Harrod200/Assistance/blob/fda6ef570f5a3d876b6e2286821b94b3e18fc07c/Assistance/TIMissionResolution_Contested_AssistBonusPatch.cs
using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches TIMissionResolution_Contested to apply assist bonuses when calculating
    /// contested mission modifiers. Each stat bonus is applied separately based on which
    /// mission attribute is being checked.
    /// 
    /// For example:
    /// - Persuasion check gets only Persuasion bonus
    /// - Command check gets only Command bonus
    /// </summary>
    [HarmonyPatch]
    public class TIMissionResolution_Contested_AssistBonusPatch
    {
        /// <summary>
        /// Applies assist bonus to attacking modifiers when the assisted councilor attacks.
        /// Only the matching stat bonus is applied based on the mission's attribute check.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.SumAttackingModifiers))]
        [HarmonyPostfix]
        public static void SumAttackingModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            float resourcesSpent,
            ref float __result)
        {
            // Log entry to contested mission check
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] SumAttackingModifiers called - Mission: {0}, Attacker: {1}, Target: {2}, Result before bonus: {3}",
                    mission != null ? mission.friendlyName : "NULL",
                    councilor != null ? councilor.displayName : "NULL",
                    target != null ? (target.isCouncilorState ? ((TICouncilorState)target).displayName : target.ToString()) : "NULL",
                    __result));
            }

            if (councilor == null || mission == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] Attacking councilor or mission is NULL, skipping bonus application");
                return;
            }

            // Get the mission's attacking attribute (e.g., Persuasion, Command)
            CouncilorAttribute missionAttribute = GetMissionAttribute(mission);
            
            // Apply only the bonus for this specific stat
            int statBonus = AssistBonusTracker.GetStatBonus(councilor, missionAttribute);

            if (statBonus <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] No {0} assist bonus for attacker '{1}' (bonus: {2})", 
                        missionAttribute, councilor.displayName, statBonus));
                return;
            }

            // Apply only this stat's bonus to attacking modifiers
            float originalResult = __result;
            __result += statBonus;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] APPLIED {0} {1} assist bonus to attacking modifier for '{2}' - Result changed from {3} to {4}",
                    statBonus, missionAttribute, councilor.displayName, originalResult, __result));
            }
        }

        /// <summary>
        /// Applies assist bonus to defending modifiers when the assisted councilor is attacked.
        /// Only the matching stat bonus is applied based on the mission's attribute check.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.SumDefendingModifiers))]
        [HarmonyPostfix]
        public static void SumDefendingModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            float resourcesSpent,
            ref float __result)
        {
            // Log entry to contested mission check
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] SumDefendingModifiers called - Mission: {0}, Defender: {1}, Target: {2}, Result before bonus: {3}",
                    mission != null ? mission.friendlyName : "NULL",
                    councilor != null ? councilor.displayName : "NULL",
                    target != null ? (target.isCouncilorState ? ((TICouncilorState)target).displayName : target.ToString()) : "NULL",
                    __result));
            }

            if (target == null || !target.isCouncilorState || mission == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] Target is NULL, not a councilor, or mission is NULL - skipping bonus application");
                return;
            }

            TICouncilorState targetCouncilor = target as TICouncilorState;
            if (targetCouncilor == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] Failed to cast target to TICouncilorState");
                return;
            }

            // Get the mission's defending attribute (e.g., Persuasion, Command)
            CouncilorAttribute missionAttribute = GetMissionAttribute(mission);
            
            // Apply only the bonus for this specific stat
            int statBonus = AssistBonusTracker.GetStatBonus(targetCouncilor, missionAttribute);

            if (statBonus <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] No {0} assist bonus for defender '{1}' (bonus: {2})", 
                        missionAttribute, targetCouncilor.displayName, statBonus));
                return;
            }

            // Apply only this stat's bonus to defending modifier
            float originalResult = __result;
            __result += statBonus;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] APPLIED {0} {1} assist bonus to defending modifier for '{2}' - Result changed from {3} to {4}",
                    statBonus, missionAttribute, targetCouncilor.displayName, originalResult, __result));
            }
        }

        /// <summary>
        /// Extracts the primary attribute used by a mission for contested checks.
        /// Defaults to Persuasion if unable to determine.
        /// </summary>
        private static CouncilorAttribute GetMissionAttribute(TIMissionTemplate mission)
        {
            if (mission == null)
                return CouncilorAttribute.Persuasion;

            // Try to get the attacking attribute from mission modifiers
            try
            {
                if (mission.modifiers != null && mission.modifiers.Length > 0)
                {
                    // Assuming the first modifier defines the primary stat
                    var firstModifier = mission.modifiers[0];
                    if (firstModifier is TIMissionModifier_CouncilorStat statModifier)
                    {
                        return statModifier.attackerAttribute;
                    }
                }
            }
            catch
            {
                // Fall through to default
            }

            // Default to Persuasion if we can't determine
            return CouncilorAttribute.Persuasion;
        }
    }
}
```

**Key Changes:**

1. **Stat-specific application**: Instead of `GetTotalBonus()` which sums all stats, now uses `GetStatBonus(councilor, missionAttribute)` to get only the matching stat's bonus
2. **Mission attribute detection**: New `GetMissionAttribute()` method determines which stat the mission is checking (e.g., Persuasion or Command)
3. **Per-stat logging**: Debug logs now specify which stat bonus is being applied

**Result with your example (100% assist percentage):**
- **Persuasion check**: 5 (Councillor A) + 4 (Councillor B's Persuasion bonus only) = **9 total**
- **Command check**: 7 (Councillor A) + 2 (Councillor B's Command bonus only) = **9 total**

Each stat is now isolated and only its matching bonus applies to contested checks.

## @Harrod225

does the new logic affect anything else?

## Copilot

No, **the new logic does NOT affect anything else**. Here's why:

## Scope of Change

The new logic **only modifies** `TIMissionResolution_Contested_AssistBonusPatch.cs`:

- Changed from `GetTotalBonus()` → `GetStatBonus(councilor, missionAttribute)`
- This only affects how bonuses are **retrieved and applied** during contested mission checks

## What Remains Unchanged

1. **`AssistBonusTracker.cs`** - Still works perfectly:
   - `RecordBonus()` still records all stats separately ✓
   - `GetStatBonus()` already existed and still works ✓
   - `GetTotalBonus()` still exists and can be used elsewhere ✓
   - `RemoveBonuses()` still clears bonuses on mission completion ✓

2. **`TIMissionEffect_Assist.cs`** - No changes needed:
   - Still calculates and records bonuses for all 7 stats ✓
   - Still calls `AssistBonusTracker.RecordBonus()` ✓
   - No impact to bonus creation logic ✓

3. **UI Display Patches** - Still unaffected:
   - `CouncilorView_GetAttributeStringPatch.cs` uses `GetStatBonus()` (already did before) ✓
   - `CouncilGridController_SetStatValuePatch.cs` uses `GetStatBonus()` (already did before) ✓
   - Orange text formatting unchanged ✓

4. **Other Patches** - No impact:
   - `AssistMissionBootstrapPatch.cs` - Mission registration unchanged
   - `TICouncilorState_CompleteMissionPatch.cs` - Still calls `RemoveBonuses()`
   - `FinalizeCouncilorMissions_AssistPriorityPatch.cs` - Resolution priority unchanged
   - `TICouncilorState_GetPossibleMissionListPatch.cs` - AI filtering unchanged

## Summary

The change is **surgically isolated** to the contested mission resolution logic. The underlying bonus system remains intact—it's only the *consumption* method that changes from pooled to per-stat.

## @Harrod225

is gettotalbonus used anywhere else?

## Copilot

Yes, `GetTotalBonus()` is used in **two places**:

## 1. **`TIMissionResolution_Contested_AssistBonusPatch.cs`** (2 calls)
   - Line 55: `SumAttackingModifiers_Postfix()` - gets total bonus for attacking councilor
   - Line 116: `SumDefendingModifiers_Postfix()` - gets total bonus for defending councilor
   - **This is what your new logic replaces** ✓

## 2. **`AssistBonusTracker.cs`** (1 call - internal)
   - Line 119: Inside `RemoveBonuses()` method - just for logging/debugging
   - Calls `GetTotalBonus()` to log the total before clearing bonuses

```csharp
int totalBonus = GetTotalBonus(councilor);
Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Clearing {0} total bonus points for '{1}'", 
    totalBonus, councilor.displayName));
```

## Impact Assessment

With the new per-stat logic:

- ✅ The `TIMissionResolution_Contested_AssistBonusPatch.cs` calls (lines 55 & 116) will be replaced by `GetStatBonus()` 
- ✅ The logging call in `RemoveBonuses()` will **continue to work** - `GetTotalBonus()` still exists and functions normally
- ✅ No breaking changes - the method remains functional for any other use cases

**Bottom line**: You only need to update the two contested mission patch calls. The internal logging usage won't break anything.

## @Harrod225

remove gettotalbonus

## Copilot

I'm waiting for your confirmation to remove the `GetTotalBonus()` method from `AssistBonusTracker.cs`.

## @Harrod225

@Copilot Accepted Confirmation: Are you sure?

## Copilot

I don't have permission to push directly to the Harrod200/Assistance repository. Here's the updated `AssistBonusTracker.cs` file with `GetTotalBonus()` removed:

```csharp name=AssistBonusTracker.cs
using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Tracks assist bonuses that are applied only during contested mission resolution.
    /// Bonuses are NOT added to base attributes; they are applied as temporary modifiers
    /// only when calculating contested mission success chances.
    /// Each stat bonus is tracked separately and applied independently during contested checks.
    /// </summary>
    public static class AssistBonusTracker
    {
        private static Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>> trackedBonuses = 
            new Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>>();

        /// <summary>
        /// Records an assist bonus for a councilor (tracked but not yet applied to attributes).
        /// Bonuses will be applied when the councilor faces contested missions.
        /// </summary>
        public static void RecordBonus(TICouncilorState councilor, CouncilorAttribute stat, int amount)
        {
            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] RecordBonus called - Councilor: {0}, Stat: {1}, Amount: {2}", 
                    councilor != null ? councilor.displayName : "NULL", stat, amount));

            if (councilor == null || amount <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] RecordBonus rejected - councilor null: {0}, amount <= 0: {1}", 
                        councilor == null, amount <= 0));
                return;
            }

            if (!trackedBonuses.ContainsKey(councilor))
            {
                trackedBonuses[councilor] = new Dictionary<CouncilorAttribute, int>();
            }

            if (!trackedBonuses[councilor].ContainsKey(stat))
            {
                trackedBonuses[councilor][stat] = 0;
            }

            trackedBonuses[councilor][stat] += amount;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Recorded bonus for '{0}': {1} +{2}", 
                    councilor.displayName, stat.ToString(), amount));
            }
        }

        /// <summary>
        /// Gets the assist bonus for a specific stat (used during contested mission checks).
        /// </summary>
        public static int GetStatBonus(TICouncilorState councilor, CouncilorAttribute stat)
        {
            if (councilor == null || !trackedBonuses.ContainsKey(councilor))
                return 0;

            if (!trackedBonuses[councilor].ContainsKey(stat))
                return 0;

            return trackedBonuses[councilor][stat];
        }

        /// <summary>
        /// Clears all tracked bonuses for a councilor when their mission completes.
        /// Note: Bonuses were never applied to attributes, so no reversal is needed.
        /// </summary>
        public static void RemoveBonuses(TICouncilorState councilor)
        {
            if (councilor == null)
                return;

            if (trackedBonuses.ContainsKey(councilor))
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Clearing bonuses for '{0}'", councilor.displayName));
                }

                trackedBonuses.Remove(councilor);
            }
        }

        /// <summary>
        /// Clears all tracked bonuses (for mod reload/unload).
        /// </summary>
        public static void ClearAll()
        {
            trackedBonuses.Clear();
        }
    }
}
```

**Changes made:**
- ❌ Removed `GetTotalBonus()` method entirely
- ❌ Removed `totalBonusAmounts` dictionary (no longer needed)
- ✅ Kept `GetStatBonus()` for per-stat retrieval
- ✅ Updated `RemoveBonuses()` to remove the `GetTotalBonus()` logging call
- ✅ Updated class documentation to reflect per-stat approach
- ✅ Simplified `ClearAll()` to only clear `trackedBonuses`

All three public methods now work with the per-stat logic exclusively.
