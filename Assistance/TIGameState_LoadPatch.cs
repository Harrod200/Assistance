using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches GameControl.Initialize to clear session-scoped static state (AssistBonusTracker)
    /// when a game session is initialized or loaded. This prevents stale councilor references
    /// from crashing the UI when loading a save in an already-running game instance.
    /// </summary>
    [HarmonyPatch(typeof(GameControl), "Initialize")]
    internal class TIGameState_LoadPatch
    {
        /// <summary>
        /// Postfix: Called after GameControl.Initialize completes.
        /// Clears the AssistBonusTracker to ensure no stale data persists from previous sessions.
        /// </summary>
        [HarmonyPostfix]
        private static void Postfix(bool loadingSave)
        {
            if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[TIGameState_LoadPatch] GameControl.Initialize called (loadingSave={0}), clearing AssistBonusTracker", loadingSave));

            AssistBonusTracker.Clear();
        }
    }
}
