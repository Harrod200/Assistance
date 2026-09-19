using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Forces Assist to resolve before every other priority-0 mission.
    ///
    /// Vanilla sorts missions by TIMissionState.getResolutionOrder (the template's
    /// resolutionOrder, plus a success-chance penalty for order > 0). Many vanilla
    /// missions share order 0, and ties resolve in shuffled faction order — so a
    /// plain resolutionOrder = 0 does not guarantee Assist fires first.
    ///
    /// A negative order keeps sorting above everything (OrderBy is stable) while
    /// still truncating to resolution segment 0, so StaggerMissionResolutions'
    /// per-segment indexing stays valid. No other code uses getResolutionOrder.
    /// </summary>
    [HarmonyPatch(typeof(TIMissionState), "getResolutionOrder", MethodType.Getter)]
    internal static class TIMissionState_getResolutionOrder_Patch
    {
        internal const float AssistOrder = -0.001f;

        static void Postfix(TIMissionState __instance, ref float __result)
        {
            if (__result == 0f && __instance.missionTemplate is TIMissionTemplate_Assist)
            {
                __result = AssistOrder;
            }
        }
    }
}
