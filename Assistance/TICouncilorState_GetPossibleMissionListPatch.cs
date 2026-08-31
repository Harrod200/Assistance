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
                // Log every call for debugging
                if (Main.mod != null)
                {
                    Main.mod.Logger.Log(string.Format("[TICouncilorState_GetPossibleMissionListPatch] Called for councilor '{0}', faction playerControl={1}, result missions={2}", 
                        __instance != null ? __instance.displayName : "NULL",
                        __instance != null && __instance.faction != null ? (__instance.faction.playerControl != null ? "NOT_NULL (PLAYER)" : "NULL (AI)") : "UNKNOWN",
                        __result.Count));
                }

                // Only filter for AI factions
                // Player-controlled factions have faction.playerControl != null
                if (__instance == null || __instance.faction == null || __instance.faction.playerControl != null)
                {
                    return;
                }

                // Remove Assist mission from AI councilor's possible missions
                int removedCount = __result.RemoveAll(mission => mission != null && mission.dataName == "Assist");

                if (removedCount > 0 && Main.mod != null)
                {
                    Main.mod.Logger.Log(string.Format("[TICouncilorState_GetPossibleMissionListPatch] FILTERED Assist from AI councilor '{0}' in faction '{1}' ({2} mission(s) removed)", __instance.displayName, __instance.faction.displayName, removedCount));
                }
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                {
                    Main.mod.Logger.Error(string.Format("[TICouncilorState_GetPossibleMissionListPatch] Error: {0}\n{1}", ex.Message, ex.StackTrace));
                }
            }
        }
    }
}
