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
    /// </summary>
    [HarmonyPatch]
    public class TIMissionResolution_Contested_ModifierCachePatch
    {
        /// <summary>
        /// Determines if a mission is relevant to the player (player councilor attacking, or targeting a player councilor).
        /// We cache all player councilor attacks and all attacks targeting player councilors, as these reflect
        /// the player's involvement and must capture modifiers at calculation time to avoid post-mission pollution.
        /// </summary>
        private static bool IsRelevantMission(TICouncilorState attacker, TIGameState target)
        {
            if (attacker == null)
                return false;

            // Always cache player-controlled councilor missions (the player is attacking)
            if (attacker.faction != null && attacker.faction.player != null && !attacker.faction.player.isAI)
            {
                return true;
            }

            // For AI councilors, only cache if targeting a player councilor (defending against player interest)
            TICouncilorState targetCouncilor = target as TICouncilorState;
            if (targetCouncilor != null && targetCouncilor.faction != null && 
                targetCouncilor.faction.player != null && !targetCouncilor.faction.player.isAI)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Caches attacking modifiers after they're retrieved by GetAttackingNonZeroModifiers.
        /// Only caches relevant missions (player-controlled or targeting player).
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
            if (mission != null && councilor != null && target != null && __result != null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[ModifierCachePatch] GetAttackingNonZeroModifiers_Postfix: mission={0}, attacker={1}, target={2}, modCount={3}",
                        mission.friendlyName, councilor.displayName, target.displayName, __result.Count));
                }

                // Only cache relevant missions
                if (IsRelevantMission(councilor, target))
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log(string.Format(
                            "[ModifierCachePatch] CACHING attacking modifiers: {0}", mission.friendlyName));
                    }
                    MissionCalculationCache.CacheAttackingModifiers(mission, councilor, target, __result);
                }
                else
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log(string.Format(
                            "[ModifierCachePatch] SKIPPING attacking modifiers (not relevant): {0}", mission.friendlyName));
                    }
                }
            }
        }

        /// <summary>
        /// Caches defending modifiers after they're retrieved by GetDefendingNonZeroModifiers.
        /// Only caches relevant missions (player-controlled attacker or player target).
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
            if (mission != null && councilor != null && target != null && __result != null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[ModifierCachePatch] GetDefendingNonZeroModifiers_Postfix: mission={0}, defender={1}, target={2}, modCount={3}",
                        mission.friendlyName, councilor.displayName, target.displayName, __result.Count));
                }

                // Cache defending modifiers for relevant missions (same as attacking modifiers)
                if (IsRelevantMission(councilor, target))
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log(string.Format(
                            "[ModifierCachePatch] CACHING defending modifiers: {0}", mission.friendlyName));
                    }
                    MissionCalculationCache.CacheDefendingModifiers(mission, councilor, target, __result);
                }
                else
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log(string.Format(
                            "[ModifierCachePatch] SKIPPING defending modifiers (not relevant): {0}", mission.friendlyName));
                    }
                }
            }
        }
    }
}
