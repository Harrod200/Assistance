using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Removes temporary assist bonuses when a councilor completes their mission.
    /// Patches SetCompletedMission which is called when mission resolution completes.
    /// 
    /// Note: The faction-level CP adjustment is handled by TIFactionState_ControlPointMaintenanceCapPatch,
    /// which recalculates automatically when GetControlPointMaintenanceFreebieCap() is called.
    /// We just need to remove the bonus from our tracking here.
    /// </summary>
    [HarmonyPatch(typeof(TICouncilorState), "SetCompletedMission")]
    internal static class TICouncilorState_CompleteMissionPatch
    {
        public static void Postfix(TICouncilorState __instance)
        {
            if (Main.enabled && Main.settings != null && Main.settings.enableAssistMission && __instance != null)
            {
                // Remove any temporary assist bonuses this councilor received
                AssistBonusTracker.RemoveBonuses(__instance);

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[AssistMission] Removed bonuses for '{0}' on mission complete", 
                        __instance.displayName));
                }
            }
        }
    }
}
