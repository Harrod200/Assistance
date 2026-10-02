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
    ///
    /// IMPORTANT: There is deliberately NO one-shot guard here.
    /// SolarSystemBootstrap.LoadGame() runs TemplateManager.ClearAllTemplates()
    /// followed by Initialize() on EVERY session start, including loading a
    /// savegame again after exiting to menu. The template store is wiped each
    /// time, so this postfix must re-register on every Initialize.
    /// It is safe to run repeatedly: the Find<T>() check makes it idempotent,
    /// and TemplateManager.Add(...) is a no-op duplicate guard behind that.
    /// (Game has no per-mod unload hook; re-registering on each init cycle is
    /// the correct counterpart to ClearAllTemplates.)
    /// </summary>
    [HarmonyPatch(typeof(TemplateManager), "Initialize")]
    internal static class AssistTemplateRegistration
    {
        private static void Postfix()
        {
            try
            {
                if (TemplateManager.Find<TIMissionTemplate>("Assist", false) == null)
                {
                    TemplateManager.Add(new TIMissionTemplate_Assist(), typeof(TIMissionTemplate));
                    if (Main.mod != null && Main.settings.debugLogging)
                        Main.mod.Logger.Log("[AssistTemplateRegistration] Assist mission template registered.");
                }
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                    Main.mod.Logger.Error("[AssistTemplateRegistration] Failed to register Assist template: " + ex);
            }
        }
    }
}
