using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches TICouncilorState.controlPointCapacity property to exclude Assist mission bonuses.
    /// 
    /// WHY THIS IS NEEDED:
    /// - Control point capacity is calculated as: Persuasion + Command + Administration
    /// - When Assist mission bonuses are applied, they temporarily inflate this sum
    /// - This gives unintended advantage: extra "free" control points during assist
    /// - Solution: Subtract total assist bonus amount from final calculation
    /// 
    /// HOW IT WORKS:
    /// - Intercepts the controlPointCapacity property getter via Postfix
    /// - Retrieves total assist bonus amount from AssistBonusTracker
    /// - Subtracts bonus from the calculated capacity (mathematically pure cancellation)
    /// - Ensures control point cap remains unchanged during assist mission
    /// </summary>
    [HarmonyPatch(typeof(TICouncilorState), "controlPointCapacity", MethodType.Getter)]
    internal static class TICouncilorState_ControlPointCapacityPatch
    {
        public static void Postfix(TICouncilorState __instance, ref int __result)
        {
            if (!Main.enabled || __instance == null)
            {
                return;
            }

            try
            {
                // Get total assist bonus for this councilor
                int assistBonus = AssistBonusTracker.GetCouncilorBonusAmount(__instance);

                if (assistBonus > 0)
                {
                    // Subtract assist bonus from control point capacity
                    __result = Math.Max(0, __result - assistBonus);
                }
            }
            catch (Exception)
            {
                // Silently fail to avoid spam; use unmodified result
            }
        }
    }
}
