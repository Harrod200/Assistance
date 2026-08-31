using System;
using System.Collections.Generic;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;

namespace Assistance
{
    [HarmonyPatch(typeof(AICouncilorMissionPlanner), "GetMissionsForCouncilor")]
    internal static class AICouncilorMissionPlanner_GetMissionsForCouncilorPatch
    {
        /// <summary>
        /// Defensive patch to filter Assist mission from AI mission planner evaluation.
        /// 
        /// Purpose: Prevent AI-controlled factions from attempting to use the Assist mission,
        /// which would cause KeyNotFoundException in the mission planner's modifier evaluation.
        /// 
        /// How it works:
        /// - Intercepts GetMissionsForCouncilor() after it returns the list of available missions
        /// - Checks if the councilor's faction is AI-controlled (faction.playerControl == null)
        /// - Removes Assist mission from the list for AI factions only
        /// - Player-controlled factions can still use Assist mission normally
        /// 
        /// Why this is needed:
        /// - Mission conditions alone are not sufficient to prevent AI evaluation
        /// - AI mission planner evaluates modifiers BEFORE checking conditions
        /// - Assist mission has empty modifier lists which cause dictionary lookup errors
        /// - Filtering at the mission retrieval stage prevents the issue entirely
        /// </summary>
        public static void Postfix(
            AICouncilorMissionPlanner __instance,
            TICouncilorState councilor,
            ref List<TIMissionTemplate> __result)
        {
            if (!Main.enabled)
            {
                return;
            }

            try
            {
                // Safety checks
                if (__result == null || councilor == null)
                {
                    return;
                }

                // Only filter for AI factions
                // Player-controlled factions have faction.playerControl != null
                if (councilor.faction == null || councilor.faction.playerControl != null)
                {
                    return;
                }

                // Remove Assist mission from AI's available missions
                int removedCount = __result.RemoveAll(mission => mission != null && mission.dataName == "Assist");

                if (Main.mod != null && removedCount > 0)
                {
                    Main.mod.Logger.Log(string.Format("[AICouncilorMissionPlanner_GetMissionsForCouncilorPatch] Filtered Assist mission from AI faction '{0}' ({1} mission(s) removed from evaluation list)", councilor.faction.displayName, removedCount));
                }
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                {
                    Main.mod.Logger.Error(string.Format("[AICouncilorMissionPlanner_GetMissionsForCouncilorPatch] Error filtering missions: {0}", ex));
                }
            }
        }
    }
}
