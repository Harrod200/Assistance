using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityEngine.UI;

namespace Assistance
{
    /// <summary>
    /// Applies a color tint to the Assist mission icon to differentiate it from the Inspire mission.
    /// This patch intercepts the SetMissionData method and applies a cyan/teal tint after icons are loaded.
    /// </summary>
    [HarmonyPatch(typeof(CouncilorMissionButtonController), "SetMissionData")]
    internal static class CouncilorMissionButtonController_AssistIconColorPatch
    {
        /// <summary>
        /// Postfix patch: runs after SetMissionData completes, applies color tint if it's an Assist mission.
        /// </summary>
        private static void Postfix(
            CouncilorMissionButtonController __instance,
            TIMissionTemplate mission,
            TICouncilorState councilor)
        {
            try
            {
                // Check if this is the Assist mission by comparing dataName
                if (mission == null || mission.dataName != "Assist")
                {
                    return;
                }

                // Get the foregroundImage component (the main icon display)
                Image foregroundImage = __instance.GetType()
                    .GetField("foregroundImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(__instance) as Image;

                if (foregroundImage != null)
                {
                    // Apply a light orange tint to differentiate from Inspire
                    // Using a light orange tint that's visually distinct but not jarring
                    // RGB values: (1.0f, 0.7f, 0.3f) for light orange with full alpha
                    foregroundImage.color = new Color(1.0f, 0.7f, 0.3f, 1.0f);
                }

                // Also apply tint to highlightImage if it exists
                Image highlightImage = __instance.GetType()
                    .GetField("highlightImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(__instance) as Image;

                if (highlightImage != null)
                {
                    // Slightly brighter/more saturated orange for the highlighted state
                    highlightImage.color = new Color(1.0f, 0.8f, 0.4f, 1.0f);
                }

                if (Main.mod != null)
                {
                    Main.mod.Logger.Log("[AssistIconColor] Applied light orange tint to Assist mission icon");
                }
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                {
                    Main.mod.Logger.Error("[AssistIconColor] Error applying color tint to Assist mission icon: " + ex.Message);
                }
            }
        }
    }
}
