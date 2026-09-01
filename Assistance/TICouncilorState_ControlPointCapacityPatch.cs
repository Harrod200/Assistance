using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches TICouncilorState.controlPointCapacity property to exclude Assist mission bonuses.
    /// Uses attribute-based Harmony patching with precise method targeting.
    /// </summary>
    [HarmonyPatch]
    internal static class TICouncilorState_ControlPointCapacityPatch
    {
        public static System.Reflection.MethodBase TargetMethod()
        {
            // Use reflection to find the controlPointCapacity property getter
            var prop = typeof(TICouncilorState).GetProperty("controlPointCapacity", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (prop == null)
            {
                if (Main.mod != null)
                    Main.mod.Logger.Error("[CP_PATCH] Could not find controlPointCapacity property");
                return null;
            }

            var getMethod = prop.GetGetMethod(nonPublic: false);
            if (getMethod == null)
            {
                if (Main.mod != null)
                    Main.mod.Logger.Error("[CP_PATCH] Could not find getter for controlPointCapacity");
                return null;
            }

            if (Main.mod != null)
                Main.mod.Logger.Log("[CP_PATCH] Targeting method: " + getMethod.Name);

            return getMethod;
        }

        private static void Postfix(TICouncilorState __instance, ref int __result)
        {
            if (!Main.enabled || __instance == null || Main.mod == null)
            {
                return;
            }

            try
            {
                // Get per-stat assist bonuses for CP-affecting stats
                int persuasionBonus = AssistBonusTracker.GetStatBonus(__instance, CouncilorAttribute.Persuasion);
                int commandBonus = AssistBonusTracker.GetStatBonus(__instance, CouncilorAttribute.Command);
                int administrationBonus = AssistBonusTracker.GetStatBonus(__instance, CouncilorAttribute.Administration);

                int totalCPBonus = persuasionBonus + commandBonus + administrationBonus;

                if (totalCPBonus > 0)
                {
                    // Subtract assist bonuses from control point capacity
                    int adjustedCap = Math.Max(0, __result - totalCPBonus);
                    Main.mod.Logger.Log(string.Format("[CP_PATCH] Councilor '{0}': Original CP={1}, CP Bonuses={2}, Adjusted CP={3}", 
                        __instance.displayName, __result, totalCPBonus, adjustedCap));
                    __result = adjustedCap;
                }
            }
            catch (Exception ex)
            {
                Main.mod.Logger.Error("[CP_PATCH] Postfix error: " + ex.Message);
            }
        }
    }
}

