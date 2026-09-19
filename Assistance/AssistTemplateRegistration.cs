using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityModManagerNet;

namespace Assistance
{
    /// <summary>
    /// Registers the Assist mission template with TemplateManager after
    /// all vanilla and mod templates have loaded.
    ///
    /// A postfix on TemplateManager.Initialize is used because:
    /// - UMM mod entry runs before the game loads its templates, so
    ///   TemplateManager.self is still null at mod-load time.
    /// - Registering after Initialize ensures the Assist template exists
    ///   before any mission granting or resolution logic can run.
    /// - The template is defined in code (not JSON), so it can encode
    ///   behavior that JSON templates cannot (custom effect + conditions).
    /// </summary>
    [HarmonyPatch(typeof(TemplateManager), "Initialize")]
    internal static class AssistTemplateRegistration
    {
        private static bool registered = false;

        private static void Postfix()
        {
            if (registered)
                return;

            try
            {
                if (TemplateManager.Find<TIMissionTemplate>("Assist", false) == null)
                {
                    TemplateManager.Add(new TIMissionTemplate_Assist(), typeof(TIMissionTemplate));
                    if (Main.mod != null && Main.settings.debugLogging)
                        Main.mod.Logger.Log("[AssistTemplateRegistration] Assist mission template registered.");
                }
                registered = true;
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                    Main.mod.Logger.Error("[AssistTemplateRegistration] Failed to register Assist template: " + ex);
            }
        }
    }
}
