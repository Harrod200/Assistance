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
        /// Helper method: Determines if a councilor is player-controlled.
        /// Returns false for null, AI factions, or non-player controllers.
        /// </summary>
        private static bool IsPlayerControlled(TICouncilorState councilor)
        {
            return councilor != null && 
                   councilor.faction != null && 
                   councilor.faction.player != null && 
                   !councilor.faction.player.isAI;
        }

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
            if (attacker == null || !IsPlayerControlled(attacker))
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
                Main.mod.Logger.Log(string.Format(
                    "[ModifierCachePatch] GetAttackingNonZeroModifiers_Postfix: mission={0}, attacker={1}, target={2}, modCount={3}",
                    mission.friendlyName, councilor.displayName, target.displayName, __result.Count));
            }

            MissionCalculationCache.CacheAttackingModifiers(mission, councilor, target, __result);
        }

        /// <summary>
        /// Caches defending modifiers after they're retrieved by GetDefendingNonZeroModifiers.
        /// Only caches relevant missions (player-controlled defender or player target).
        /// 
        /// GUARD CLAUSE: AI faction actions exit immediately without log buffer allocation.
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
            // Structural Guard Clause: Stop AI execution paths before any allocation
            if (mission == null || councilor == null || target == null || __result == null)
            {
                return;
            }

            // For defending modifiers, the 'councilor' parameter is the defender.
            // Check if the defender is player-controlled OR if the target is a player-controlled councilor
            bool isPlayerDefender = IsPlayerControlled(councilor);
            bool isPlayerTarget = false;

            if (target is TICouncilorState targetCouncilor)
            {
                isPlayerTarget = IsPlayerControlled(targetCouncilor);
            }

            if (!isPlayerDefender && !isPlayerTarget)
            {
                return; // Silent exit - zero log buffer allocation for non-player scenarios
            }

            // Safe Isolated Execution: Only runs for player-relevant objectives
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[ModifierCachePatch] GetDefendingNonZeroModifiers_Postfix: mission={0}, defender={1}, target={2}, modCount={3}",
                    mission.friendlyName, councilor.displayName, target.displayName, __result.Count));
            }

            MissionCalculationCache.CacheDefendingModifiers(mission, councilor, target, __result);
        }
    }
}
