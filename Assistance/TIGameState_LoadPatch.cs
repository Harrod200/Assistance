using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches GameControl.Initialize to clear session-scoped static state (AssistBonusTracker)
    /// when a game session is initialized or loaded. This prevents stale councilor references
    /// from crashing the UI when loading a save in an already-running game instance.
    /// 
    /// Uses Prefix (runs BEFORE vanilla Initialize) to ensure bonuses are cleared before
    /// the UI tries to render mission icons and other elements that reference councilors.
    /// </summary>
    [HarmonyPatch(typeof(GameControl), "Initialize")]
    internal class TIGameState_LoadPatch
    {
        /// <summary>
        /// Prefix: Called BEFORE GameControl.Initialize.
        /// Clears the AssistBonusTracker immediately, before vanilla initialization runs.
        /// This prevents stale councilor references from being accessed during UI setup.
        /// </summary>
        [HarmonyPrefix]
        private static void Prefix(bool loadingSave)
        {
            if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[TIGameState_LoadPatch] GameControl.Initialize starting (loadingSave={0}), clearing AssistBonusTracker BEFORE init", loadingSave));

            AssistBonusTracker.Clear();
        }
    }
}
