using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches CouncilorView.GetAttributeString() to display combined stat totals with bonuses.
    /// This affects stat display in mission panels, council view, and other UI locations.
    /// </summary>
    [HarmonyPatch(typeof(CouncilorView), "GetAttributeString")]
    public static class CouncilorView_GetAttributeStringPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CouncilorView __instance, CouncilorAttribute attribute, ref string __result)
        {
            try
            {
                if (__result == null)
                    return;

                // Get the councilor state from the view
                TICouncilorState councilor = __instance.councilor;
                if (councilor == null)
                    return;

                // Get the assist bonus for this stat
                int bonus = AssistBonusTracker.GetStatBonus(councilor, attribute);

                if (bonus > 0)
                {
                    // Try to parse the base stat from the result string
                    if (int.TryParse(__result, out int baseStat))
                    {
                        // Calculate combined total
                        int totalStat = baseStat + bonus;

                        // Format: show only combined total (in orange)
                        __result = FormatOrange(totalStat.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                // Safely log any errors without crashing the UI
                if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                    Main.mod.Logger.Error("[AssistBonusDisplay] Error in GetAttributeString postfix: " + e.Message);
            }
        }

        /// <summary>
        /// Formats text with orange color using TextMesh Pro color tags.
        /// Orange color: #FFA500 (standard orange)
        /// </summary>
        private static string FormatOrange(string text)
        {
            return "<color=#FFA500>" + text + "</color>";
        }
    }
}
