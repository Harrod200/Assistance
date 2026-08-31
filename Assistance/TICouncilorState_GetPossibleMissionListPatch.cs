using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches TICouncilorState.GetPossibleMissionList() to filter Assist mission from AI councilors.
    /// 
    /// WHY THIS IS NEEDED:
    /// - The Assist mission has empty modifier lists in its resolution method
    /// - When AI faction mission planner evaluates missions via GetPossibleMissionList(), it tries to access modifiers before checking conditions
    /// - This causes KeyNotFoundException when the planner looks for Assist mission modifiers in a dictionary
    /// - Filtering at the councilor's mission list level (before AI evaluation) prevents the crash entirely
    /// 
    /// HOW IT WORKS:
    /// - Intercepts GetPossibleMissionList() after it returns the list for a specific councilor
    /// - Checks if the councilor's faction is AI-controlled (faction.playerControl == null)
    /// - Removes Assist mission from the result list for AI faction councilors only
    /// - Player-controlled faction councilors can still use Assist mission normally
    /// </summary>
    [HarmonyPatch(typeof(TICouncilorState), "GetPossibleMissionList", new System.Type[] { typeof(bool), typeof(bool), typeof(bool), typeof(TIOrgState), typeof(bool) })]
    internal static class TICouncilorState_GetPossibleMissionListPatch
    {
        public static void Postfix(TICouncilorState __instance, ref List<TIMissionTemplate> __result)
        {
            if (!Main.enabled || __result == null)
            {
                return;
            }

            try
            {
                // Only filter for AI factions
                // AI-controlled factions have faction.player.isAI == true
                if (__instance == null || __instance.faction == null || __instance.faction.player == null || !__instance.faction.player.isAI)
                {
                    return;
                }

                // Remove Assist mission from AI councilor's possible missions
                __result.RemoveAll(mission => mission != null && mission.dataName == "Assist");
            }
            catch
            {
                // Silently fail to avoid spam
            }
        }
    }
}
