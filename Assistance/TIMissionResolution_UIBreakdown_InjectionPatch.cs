using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// UI injection patch for the contested mission detail breakdown screen.
    /// Uses reflection-based patching since UIContestedMissionDetailBreakdown is not available at compile-time.
    /// This patch is currently disabled pending access to the actual UI assembly.
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
                // Try to find the UIContestedMissionDetailBreakdown type in the game assembly
                var targetType = Type.GetType("PavonisInteractive.TerraInvicta.UIContestedMissionDetailBreakdown, Assembly-CSharp");
                if (targetType == null)
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log("[UIMissionDetailBreakdown] Warning: UIContestedMissionDetailBreakdown type not found. UI injection patch disabled.");
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
        /// Reclaims missing modifier splits (Orgs, Item Tiers, Trait Buffs) and blends them into the layout rows.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(object __instance)
        {
            try
            {
                if (__instance == null)
                {
                    return;
                }

                // Use reflection to safely call methods on the UI instance
                var missionProp = __instance.GetType().GetProperty("mission", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                var councilorProp = __instance.GetType().GetProperty("councilor", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);

                if (missionProp == null || councilorProp == null)
                {
                    return;
                }

                var mission = missionProp.GetValue(__instance) as TIMissionTemplate;
                var councilor = councilorProp.GetValue(__instance) as TICouncilorState;

                // 1. Guard Clause: Skip visual operations instantly if this context frame is non-player
                if (councilor == null || !AssistBonusTracker.IsPlayerControlled(councilor))
                {
                    return;
                }

                // --- RECOVERING HIDDEN VECTOR 1: ORG INJECTIONS & UN-TRACKED BASES ---
                // Note: We would need access to mission.attackingAttribute to implement this
                // For now, we'll use the primary attacker stat as a proxy
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
                            // Append the missing Organization layer straight into the visible user overlay menu rows
                            var addRowMethod = __instance.GetType().GetMethod("AddBreakdownRow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            if (addRowMethod != null)
                            {
                                try
                                {
                                    addRowMethod.Invoke(__instance, new object[] { $"Attached Orgs & Assets ({activeAttribute})", (float)hiddenOrgContribution });
                                }
                                catch (Exception ex)
                                {
                                    if (Main.mod != null && Main.settings.debugLogging)
                                    {
                                        Main.mod.Logger.Log(string.Format("[UIMissionDetailBreakdown] Error adding org row: {0}", ex.Message));
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
                        // Explicitly routes and appends the missing squad assistance totals right into the layout pane
                        var addRowMethod = __instance.GetType().GetMethod("AddBreakdownRow", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (addRowMethod != null)
                        {
                            try
                            {
                                addRowMethod.Invoke(__instance, new object[] { "Adjacent Councilors Assistance Pool", (float)activeAssistanceBonus });
                            }
                            catch (Exception ex)
                            {
                                if (Main.mod != null && Main.settings.debugLogging)
                                {
                                    Main.mod.Logger.Log(string.Format("[UIMissionDetailBreakdown] Error adding assist row: {0}", ex.Message));
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
