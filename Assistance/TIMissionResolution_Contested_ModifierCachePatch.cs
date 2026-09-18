using System;
using System.Collections.Generic;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches the GetAttackingNonZeroModifiers and GetDefendingNonZeroModifiers methods
    /// to cache the exact modifier lists at the point of mission calculation.
    /// This ensures the breakdown display shows modifiers as they were during resolution,
    /// not as they appear after rewards have been added.
    /// 
    /// GUARD CLAUSES: All non-player AI operations exit silently without log allocation
    /// to prevent thread-corrupting AI log spam and buffer crashes.
    /// </summary>
    [HarmonyPatch]
    public class TIMissionResolution_Contested_ModifierCachePatch
    {
        /// <summary>
        /// Determines if a mission is relevant to the player (player councilor attacking, or targeting a player councilor).
        /// We cache all player councilor attacks and all attacks targeting player councilors, as these reflect
        /// the player's involvement and must capture modifiers at calculation time to avoid post-mission pollution.
        /// 
        /// GUARD CLAUSE: Returns false immediately for null or AI-controlled attackers to skip processing.
        /// </summary>
        private static bool IsRelevantMission(TICouncilorState attacker, TIGameState target)
        {
            // Structural Guard Clause: Terminate AI execution paths instantly
            if (attacker == null || !AssistBonusTracker.IsPlayerControlled(attacker))
            {
                return false; // Silent exit - no logging, no allocations
            }

            // Always cache player-controlled councilor missions (the player is attacking)
            return true;
        }

        /// <summary>
        /// Caches attacking modifiers after they're retrieved by GetAttackingNonZeroModifiers.
        /// Only caches relevant missions (player-controlled or targeting player).
        /// 
        /// GUARD CLAUSE: AI faction actions exit immediately without log buffer allocation.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.GetAttackingNonZeroModifiers))]
        [HarmonyPostfix]
        public static void GetAttackingNonZeroModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            float resourcesSpent,
            List<TIMissionModifier> __result)
        {
            // Structural Guard Clause: Stop AI execution paths before any allocation
            if (mission == null || councilor == null || target == null || __result == null)
            {
                return;
            }

            if (!IsRelevantMission(councilor, target))
            {
                return; // Silent exit - zero log buffer allocation for AI actions
            }

            // Safe Isolated Execution: Only runs for player-controlled councilors
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log($"[ModifierCachePatch] GetAttackingNonZeroModifiers_Postfix: mission={mission.friendlyName}, attacker={councilor.displayName}, target={target.displayName}, modCount={__result.Count}");
            }

            MissionCalculationCache.CacheAttackingModifiers(mission, councilor, target, __result);
        }

        /// <summary>
        /// Postfix Patch for caching defending modifiers layout snapshots.
        /// Resolves cache lookup key inversion bugs on offensive contested rolls.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.GetDefendingNonZeroModifiers))]
        [HarmonyPostfix]
        public static void GetDefendingNonZeroModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            float resourcesSpent,
            List<TIMissionModifier> __result)
        {
            // Structural Guard Clause: Reject corrupted execution frames instantly
            if (mission == null || councilor == null || target == null || __result == null)
            {
                return;
            }

            // Resolve the objective defender target node to compute faction relevance
            TICouncilorState trueDefender = councilor;
            if (target is TICouncilorState targetedCouncilor)
            {
                trueDefender = targetedCouncilor;
            }

            // CRITICAL GUARD CLAUSE: Silently skip AI-exclusive paths to suppress background thread noise.
            // Optimization: Reuses centralized helper block from the core tracker.
            if (!AssistBonusTracker.IsPlayerControlled(councilor) && !AssistBonusTracker.IsPlayerControlled(trueDefender))
            {
                return;
            }

            if (Main.mod != null && Main.settings.debugLogging)
            {
                // Optimization: String interpolation avoids heavy heap allocations from legacy formatting loops
                Main.mod.Logger.Log($"[AssistMission] Caching UI screen layout breakdown for: {mission.friendlyName}");
            }

            // FIX: Always preserve 'councilor' (the primary actor) as the key anchor!
            // This forces the cache string schema to align precisely with the UI panel lookup parameters,
            // restoring legitimate difficulty value strings in the mission overview pane.
            MissionCalculationCache.CacheDefendingModifiers(mission, councilor, target, __result);
        }
    }
}

