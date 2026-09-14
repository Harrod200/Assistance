using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Clears ALL assist mission bonuses during start-of-turn bookkeeping.
    /// This ensures bonuses are cleared during the phase transition setup, naturally paired
    /// with other start-of-turn operations.
    /// 
    /// Patches StartofTurnBookkeeping which is called during turn/phase transition cleanup.
    /// </summary>
    [HarmonyPatch(typeof(TIMissionPhaseState), "StartofTurnBookkeeping")]
    internal static class TIMissionPhaseState_StartofTurnBookkeepingPatch
    {
        public static void Postfix()
        {
            if (Main.enabled && Main.settings != null && Main.settings.enableAssistMission)
            {
                // Clear all assist mission bonuses after bookkeeping is done
                AssistBonusTracker.ClearAll();

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[AssistMission] Cleared all assist bonuses during start-of-turn bookkeeping");
                }
            }
        }
    }
}
