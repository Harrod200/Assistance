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
        /// Caches attacking modifiers after they're retrieved by GetAttackingNonZeroModifiers.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.GetAttackingNonZeroModifiers))]
        [HarmonyPostfix]
        public static void GetAttackingNonZeroModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            float baseValue,
            List<TIMissionModifier> __result)
        {
            if (mission != null && councilor != null && target != null && __result != null)
            {
                MissionCalculationCache.CacheAttackingModifiers(mission, councilor, target, __result);
            }
        }

        /// <summary>
        /// Caches defending modifiers after they're retrieved by GetDefendingNonZeroModifiers.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.GetDefendingNonZeroModifiers))]
        [HarmonyPostfix]
        public static void GetDefendingNonZeroModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            float baseValue,
            List<TIMissionModifier> __result)
        {
            if (mission != null && councilor != null && target != null && __result != null)
            {
                MissionCalculationCache.CacheDefendingModifiers(mission, councilor, target, __result);
            }
        }
    }
}
