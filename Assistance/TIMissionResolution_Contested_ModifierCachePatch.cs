using System;
using System.Collections.Generic;
using HarmonyLib;
using PavonisInteractive.TerraInvicta; // Reference native assembly definitions

namespace Assistance
{
    [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.GetDefendingNonZeroModifiers))]
    public static class GetDefendingNonZeroModifiers_CachePatch
    {
        [HarmonyPostfix]
        public static void Postfix(
            TIMissionResolution_Contested __instance, 
            TIMissionTemplate mission, 
            TICouncilorState councilor, // This is Catherine Waterson (Primary Actor)
            TIGameState target,         // This is Xenoform Alfa-18 (Target Node)
            float resourcesSpent, 
            List<TIMissionModifier> __result)
        {
            // 1. Structural Guard Clause: Drop execution on corrupt data frames instantly
            if (mission == null || councilor == null || target == null || __result == null)
            {
                return;
            }

            // 2. Resolve the correct combat target node for faction validation
            TICouncilorState trueDefender = councilor;
            if (target is TICouncilorState targetedCouncilor)
            {
                trueDefender = targetedCouncilor;
            }

            // 3. SILENT GUARD CLAUSE: Suppress non-player background evaluation loops
            // This isolates player data traces and eliminates multi-threaded log exceptions
            if (!AssistBonusTracker.IsPlayerControlled(councilor) && 
                !AssistBonusTracker.IsPlayerControlled(trueDefender))
            {
                return; 
            }

            if (Main.mod != null && Main.settings.debugLogging)
            {
                // Optimization: String interpolation avoids heavy heap allocations from legacy formatting loops
                Main.mod.Logger.Log($"[AssistMission] Caching UI screen layout breakdown for: {mission.friendlyName}");
            }

            // COSMETIC BUG FIX: By passing 'councilor' (the primary player actor) as the lookup anchor,
            // the internal cache dictionary key string will align perfectly with what the UI screen layer 
            // requests when drawing the breakdown window, resolving the blank 0 Defense display error.
            MissionCalculationCache.CacheDefendingModifiers(mission, councilor, target, __result);
        }
    }
}

