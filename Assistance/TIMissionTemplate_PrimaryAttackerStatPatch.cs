using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Prevents crash when TIMissionTemplate.get_primaryAttackerStat() is called on missions with Automatic resolution.
    /// The game tries to iterate attackingModifiers list, which is null for Automatic resolution.
    /// </summary>
    [HarmonyPatch(typeof(TIMissionTemplate), "get_primaryAttackerStat")]
    internal static class TIMissionTemplate_PrimaryAttackerStatPatch
    {
        public static bool Prefix(TIMissionTemplate __instance, ref CouncilorAttribute __result)
        {
            try
            {
                // If this is our Assist mission and resolution is Automatic, return a safe default
                if (__instance.dataName == "Assist" && __instance.resolutionMethod.GetType().Name == "TIMissionResolution_Automatic")
                {
                    __result = CouncilorAttribute.Persuasion;
                    return false; // Skip the original method
                }
            }
            catch (Exception)
            {
                // If anything goes wrong, let the original method handle it
            }

            return true; // Continue with original method
        }
    }
}
