using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// UI injection patch for the contested mission detail breakdown screen.
    /// Patches UIMissionDetailBreakdownContested.RefreshWindow to inject additional modifier rows
    /// showing organization contributions and squad assistance bonuses.
    /// </summary>
    [HarmonyPatch]
    public static class UIMissionDetailBreakdown_RowInjectionPatch
    {
        /// <summary>
        /// Prepares the Harmony patch by specifying the target type via reflection.
        /// This allows patching UI classes that aren't available at compile-time.
        /// Returns null if the UI type is not found (graceful degradation).
        /// </summary>
        private static System.Reflection.MethodBase TargetMethod()
        {
            try
            {
                // Try to find the UIMissionDetailBreakdownContested type in the game assembly
                var targetType = Type.GetType("PavonisInteractive.TerraInvicta.UIMissionDetailBreakdownContested, Assembly-CSharp");
                if (targetType == null)
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log("[UIMissionDetailBreakdown] Warning: UIMissionDetailBreakdownContested type not found. UI injection patch disabled.");
                    }
                    return null;
                }

                // Find the RefreshWindow method
                var method = targetType.GetMethod("RefreshWindow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (method == null)
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log("[UIMissionDetailBreakdown] Warning: RefreshWindow method not found. UI injection patch disabled.");
                    }
                    return null;
                }

                return method;
            }
            catch (Exception ex)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[UIMissionDetailBreakdown] Error loading UI patch target: {0}", ex.Message));
                }
                return null;
            }
        }

        /// <summary>
        /// Postfix patch targeting the active UI screen drawing system.
        /// Injects modifier rows for organization contributions and squad assistance bonuses.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(object __instance, TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
        {
            try
            {
                if (__instance == null)
                {
                    return;
                }

                // 1. Guard Clause: Skip visual operations instantly if this context frame is non-player
                if (councilor == null || !AssistBonusTracker.IsPlayerControlled(councilor))
                {
                    return;
                }

                // --- RECOVERING HIDDEN VECTOR 1: ORG INJECTIONS & UN-TRACKED BASES ---
                if (mission != null)
                {
                    CouncilorAttribute activeAttribute = mission.primaryAttackerStat;

                    if (activeAttribute != CouncilorAttribute.None)
                    {
                        // Pull the full final derived stat value (includes Orgs, cybernetics, items, and gear)
                        int totalStatSheetValue = councilor.GetAttribute(activeAttribute, true, true, true, false, false, false);

                        // Pull the base stat without any modifiers
                        int baseStatValue = councilor.GetAttribute(activeAttribute, false, false, false, false, false, false);
                        int hiddenOrgContribution = totalStatSheetValue - baseStatValue;

                        if (hiddenOrgContribution > 0)
                        {
                            // Inject the missing Organization layer into the modifier rows
                            var addModifierRowMethod = __instance.GetType().GetMethod("AddModifierRow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            if (addModifierRowMethod != null)
                            {
                                try
                                {
                                    addModifierRowMethod.Invoke(__instance, new object[] { $"Attached Orgs & Assets ({activeAttribute})", (float)hiddenOrgContribution, false });
                                }
                                catch (Exception ex)
                                {
                                    if (Main.mod != null && Main.settings.debugLogging)
                                    {
                                        Main.mod.Logger.Log(string.Format("[UIMissionDetailBreakdown] Error adding org modifier row: {0}", ex.Message));
                                    }
                                }
                            }
                        }
                    }
                }

                // --- RECOVERING HIDDEN VECTOR 2: COMPILING SQUAD ASSIST VALUES ---
                if (mission != null)
                {
                    CouncilorAttribute activeAttribute = mission.primaryAttackerStat;
                    int activeAssistanceBonus = AssistBonusTracker.GetStatBonus(councilor, activeAttribute);
                    if (activeAssistanceBonus > 0)
                    {
                        // Inject the squad assistance totals into the modifier rows
                        var addModifierRowMethod = __instance.GetType().GetMethod("AddModifierRow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (addModifierRowMethod != null)
                        {
                            try
                            {
                                addModifierRowMethod.Invoke(__instance, new object[] { "Adjacent Squad Assistance Pool", (float)activeAssistanceBonus, false });
                            }
                            catch (Exception ex)
                            {
                                if (Main.mod != null && Main.settings.debugLogging)
                                {
                                    Main.mod.Logger.Log(string.Format("[UIMissionDetailBreakdown] Error adding assist modifier row: {0}", ex.Message));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[UIMissionDetailBreakdown] Error in RowInjectionPatch postfix: {0}", ex.Message));
                }
            }
        }
    }
}
