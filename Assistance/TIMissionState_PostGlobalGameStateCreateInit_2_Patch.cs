using System.Reflection;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches TIMissionState.PostGlobalGameStateCreateInit_2() to resolve null mission templates
    /// that can occur during save/load when missions reference templates that haven't been resolved yet.
    ///
    /// This is called AFTER deserialization completes, making it an ideal hook to fix missing template
    /// references before any code tries to use the mission template (like GetCurrentMissionIcon).
    ///
    /// Instead of defensively patching GetCurrentMissionIcon to handle null templates,
    /// we fix the root cause here by ensuring templates are properly resolved during initialization.
    /// </summary>
    [HarmonyPatch(typeof(TIMissionState), "PostGlobalGameStateCreateInit_2")]
    internal static class TIMissionState_PostGlobalGameStateCreateInit_2_Patch
    {
        private static FieldInfo _missionTemplateField = null;

        /// <summary>
        /// Get the backing field for _missionTemplate (cached for performance).
        /// </summary>
        private static FieldInfo GetMissionTemplateField()
        {
            if (_missionTemplateField == null)
            {
                _missionTemplateField = typeof(TIMissionState).GetField("_missionTemplate", 
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return _missionTemplateField;
        }

        /// <summary>
        /// Postfix: Runs after vanilla PostGlobalGameStateCreateInit_2.
        /// Attempts to resolve any null mission templates by looking them up by name.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(TIMissionState __instance)
        {
            if (__instance == null)
                return;

            // If template is already valid, nothing to do
            if (__instance.missionTemplate != null)
                return;

            // If we have a template name, try to resolve it
            if (string.IsNullOrEmpty(__instance.templateName))
            {
                if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format(
                        "[TIMissionState_PostGlobalGameStateCreateInit_2] Mission {0} has null template and no templateName to resolve",
                        __instance.ID));
                return;
            }

            // Try to find the template by name
            TIMissionTemplate resolvedTemplate = TemplateManager.Find<TIMissionTemplate>(__instance.templateName, false);

            if (resolvedTemplate != null)
            {
                // Use reflection to set the backing field since missionTemplate property is read-only
                FieldInfo field = GetMissionTemplateField();
                if (field != null)
                {
                    field.SetValue(__instance, resolvedTemplate);

                    if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                        Main.mod.Logger.Log(string.Format(
                            "[TIMissionState_PostGlobalGameStateCreateInit_2] Successfully resolved template '{0}' for mission {1}",
                            __instance.templateName, __instance.ID));
                }
            }
            else
            {
                if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format(
                        "[TIMissionState_PostGlobalGameStateCreateInit_2] WARNING: Could not resolve template '{0}' for mission {1}",
                        __instance.templateName, __instance.ID));
            }
        }
    }
}
