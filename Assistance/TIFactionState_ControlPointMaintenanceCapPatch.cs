using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches TIFactionState.GetControlPointMaintenanceFreebieCap() to exclude assist mission bonuses.
    /// 
    /// This is a faction-level patch that adjusts the aggregate maintenance cap after all components
    /// (global freebies, AI bonus, councilor sum, hab sum) and effects-modifier subtraction.
    /// 
    /// By patching at this level instead of the per-councilor getter, we:
    /// - Apply a clean flat adjustment that doesn't affect the UI ledger display per-councilor
    /// - Avoid double-counting or interaction issues with activeCouncilors filtering
    /// - Apply the adjustment after all other calculation stages are complete
    /// 
    /// The postfix runs even for alien factions (returns 20000f), but we guard against those
    /// since assist bonuses should only apply to player-controlled factions anyway.
    /// 
    /// Includes a Prefix patch to log method entry point details for debugging.
    /// </summary>
    [HarmonyPatch(typeof(TIFactionState), nameof(TIFactionState.GetControlPointMaintenanceFreebieCap))]
    internal static class TIFactionState_ControlPointMaintenanceCapPatch
    {
        private static void Prefix(TIFactionState __instance)
        {
            if (!Main.enabled || __instance == null || Main.mod == null || !Main.settings.debugLogging)
            {
                return;
            }

            try
            {
                Main.mod.Logger.Log(string.Format("[CP_CAP_PATCH_PREFIX] GetControlPointMaintenanceFreebieCap() called for faction '{0}' (IsAlien={1})",
                    __instance.displayName, __instance.IsAlienFaction));
            }
            catch (System.Exception ex)
            {
                if (Main.mod != null)
                    Main.mod.Logger.Error("[CP_CAP_PATCH_PREFIX] Prefix error: " + ex.Message);
            }
        }

        private static void Postfix(TIFactionState __instance, ref float __result)
        {
            if (!Main.enabled || __instance == null || Main.mod == null)
            {
                return;
            }

            // Skip adjustment for alien factions (they return 20000f baseline and don't use normal CP mechanics)
            if (__instance.IsAlienFaction)
            {
                return;
            }

            try
            {
                // Calculate total CP adjustment for this faction
                // This is the sum of all CP-affecting bonuses (Persuasion + Command + Administration) across all councilors in this faction
                int factionCPAdjustment = AssistBonusTracker.GetFactionCPAdjustment(__instance);

                if (factionCPAdjustment > 0)
                {
                    // Subtract the assist bonuses from the maintenance freebie cap
                    float adjustedCap = __result - factionCPAdjustment;

                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log(string.Format("[CP_CAP_PATCH_POSTFIX] Faction '{0}': Original Cap={1}, Assist Bonus Adjustment=-{2}, Adjusted Cap={3}",
                            __instance.displayName, __result, factionCPAdjustment, adjustedCap));
                    }

                    __result = adjustedCap;
                }
                else if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[CP_CAP_PATCH_POSTFIX] Faction '{0}': No assist bonuses to adjust. Cap={1}",
                        __instance.displayName, __result));
                }
            }
            catch (System.Exception ex)
            {
                if (Main.mod != null)
                    Main.mod.Logger.Error("[CP_CAP_PATCH_POSTFIX] Postfix error: " + ex.Message);
            }
        }
    }
}
