using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches TICouncilorState.GetCurrentMissionIcon to safely handle cases where
    /// activeMission.missionTemplate is null.
    ///
    /// This can occur during save/load transitions when:
    /// - A saved mission references the "Assist" template by string name
    /// - The mission is deserialized before the template is fully resolved
    /// - The missionTemplate field ends up null despite activeMission being valid
    ///
    /// Without this patch, the vanilla code tries to access missionTemplate.missionIconImagePath
    /// on a null reference, causing NullReferenceException during Finder list rendering.
    /// </summary>
    [HarmonyPatch(typeof(TICouncilorState), "GetCurrentMissionIcon")]
    internal static class TICouncilorState_GetCurrentMissionIconPatch
    {
        /// <summary>
        /// Prefix: Runs before vanilla GetCurrentMissionIcon.
        /// Checks if missionTemplate is null and returns early if so.
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix(TICouncilorState __instance, bool on, ref string __result)
        {
            if (__instance == null)
            {
                __result = string.Empty;
                return false;
            }

            // Check if councilor has an active mission
            if (__instance.activeMission == null)
            {
                __result = string.Empty;
                return false;
            }

            // CRITICAL: Check if missionTemplate is null
            // This can happen during save/load when template resolution is incomplete
            if (__instance.activeMission.missionTemplate == null)
            {
                if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format(
                        "[TICouncilorState_GetCurrentMissionIcon] Null missionTemplate for '{0}' - returning empty string",
                        __instance.displayName));

                __result = string.Empty;
                return false; // Skip vanilla implementation
            }

            // Mission template is valid, let vanilla handle it
            return true;
        }
    }
}
