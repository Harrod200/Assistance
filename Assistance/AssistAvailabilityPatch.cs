using System;
using System.Collections.Generic;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Grants the Assist mission to every player-controlled councilor by
    /// postfixing the single choke point the game uses to build a councilor's
    /// available mission list. Covers starting councilors, recruits, and
    /// saves made before the mod was installed; LearnMission-equivalent
    /// bookkeeping (learnedMissions) is kept in sync so AI evaluation,
    /// save round-trips, and UI checks see a consistent state.
    /// </summary>
    [HarmonyPatch(typeof(TICouncilorState), "GetPossibleMissionList")]
    internal static class AssistAvailabilityPatch
    {
        private static void Postfix(TICouncilorState __instance, ref List<TIMissionTemplate> __result)
        {
            try
            {
                if (!Main.enabled || Main.settings == null || !Main.settings.enableAssistMission)
                    return;
                if (__instance == null || __instance.faction == null || __instance.faction.player == null || __instance.faction.player.isAI)
                    return;

                TIMissionTemplate assist = TemplateManager.Find<TIMissionTemplate>("Assist", false);
                if (assist == null)
                    return;

                // Record it as learned so downstream consumers (saves, UI,
                // AI) see the same state the mission list reports.
                if (!__instance.learnedMissions.Contains(assist))
                    __instance.learnedMissions.Add(assist);
                if (!__instance.learnedMissionsTemplateNames.Contains(assist.dataName))
                    __instance.learnedMissionsTemplateNames.Add(assist.dataName);

                if (!__result.Contains(assist))
                    __result.Add(assist);
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                    Main.mod.Logger.Error("Assist availability patch failed: " + ex);
            }
        }
    }
}
