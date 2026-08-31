using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Removes temporary assist bonuses when a councilor completes their mission
    /// Patches SetCompletedMission which is called when mission resolution completes
    /// </summary>
    [HarmonyPatch(typeof(TICouncilorState), "SetCompletedMission")]
    internal static class TICouncilorState_CompleteMissionPatch
    {
        public static void Postfix(TICouncilorState __instance)
        {
            if (Main.enabled && Main.settings != null && Main.settings.enableAssistMission)
            {
                // Remove any temporary assist bonuses this councilor received
                AssistBonusTracker.RemoveBonuses(__instance);
            }
        }
    }
}
