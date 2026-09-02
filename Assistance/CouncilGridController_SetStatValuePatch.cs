using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches CouncilGridController.SetStatValue() to display combined stat totals with bonuses.
    /// This affects the stat display in the council grid/councilor list view.
    /// </summary>
    [HarmonyPatch]
    public static class CouncilGridController_SetStatValuePatch
    {
        [HarmonyTargetMethod]
        public static System.Reflection.MethodBase TargetMethod()
        {
            // Find the SetStatValue method which takes a CouncilorAttribute and CouncilorView
            var controllerType = Type.GetType("PavonisInteractive.TerraInvicta.CouncilGridController, Assembly-CSharp");
            if (controllerType != null)
            {
                var method = controllerType.GetMethod("SetStatValue",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    new Type[] { typeof(CouncilorAttribute), typeof(CouncilorView) },
                    null);

                if (method != null)
                {
                    if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                        Main.mod.Logger.Log("[AssistBonusDisplay] Successfully found SetStatValue method for patching");
                    return method;
                }
            }

            if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                Main.mod.Logger.Log("[AssistBonusDisplay] Could not find SetStatValue method for patching");
            return null;
        }

        [HarmonyPostfix]
        public static void Postfix(object __instance, CouncilorAttribute attribute, CouncilorView councilorView)
        {
            try
            {
                TICouncilorState councilor = councilorView.councilor;
                if (councilor == null)
                    return;

                // Get the assist bonus for this stat
                int bonus = AssistBonusTracker.GetStatBonus(councilor, attribute);

                if (bonus > 0)
                {
                    // Find the text field that was just updated and append the bonus
                    // The SetStatValue method sets a text field based on the attribute type
                    var controllerType = __instance.GetType();

                    // Get the field name based on the attribute
                    string fieldName = GetFieldNameForAttribute(attribute);
                    if (string.IsNullOrEmpty(fieldName))
                        return;

                    // Try to get the TMP_Text field and update with combined total
                    var field = controllerType.GetField(fieldName, 
                        System.Reflection.BindingFlags.NonPublic | 
                        System.Reflection.BindingFlags.Instance | 
                        System.Reflection.BindingFlags.Public);

                    if (field != null)
                    {
                        var fieldValue = field.GetValue(__instance);
                        if (fieldValue != null)
                        {
                            // Access text property via reflection (TextMeshProUGUI.text)
                            var textProperty = fieldValue.GetType().GetProperty("text",
                                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                            if (textProperty != null)
                            {
                                string currentText = (string)textProperty.GetValue(fieldValue);
                                if (currentText != null && int.TryParse(currentText, out int baseStat))
                                {
                                    // Calculate combined total
                                    int totalStat = baseStat + bonus;

                                    // Format: show only combined total (in orange)
                                    string newText = FormatOrange(totalStat.ToString());
                                    textProperty.SetValue(fieldValue, newText);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Safely log any errors without crashing the UI
                if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                    Main.mod.Logger.Error("[AssistBonusDisplay] Error in SetStatValue postfix: " + e.Message);
            }
        }

        /// <summary>
        /// Maps CouncilorAttribute enum to the text field name in CouncilGridController.
        /// Based on the search results showing field names like: persuasion, investigation, espionage, etc.
        /// </summary>
        private static string GetFieldNameForAttribute(CouncilorAttribute attribute)
        {
            switch (attribute)
            {
                case CouncilorAttribute.Persuasion:
                    return "persuasion";
                case CouncilorAttribute.Investigation:
                    return "investigation";
                case CouncilorAttribute.Espionage:
                    return "espionage";
                case CouncilorAttribute.Command:
                    return "command";
                case CouncilorAttribute.Administration:
                    return "administration";
                case CouncilorAttribute.Science:
                    return "science";
                case CouncilorAttribute.Security:
                    return "security";
                default:
                    return null;
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
