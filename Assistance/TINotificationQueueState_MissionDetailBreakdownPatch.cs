using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches TINotificationQueueState.LogMissionOutcome() to add a detailed breakdown of
    /// attack and defense rolls and their contributors to the mission result notification.
    /// 
    /// For contested missions, this adds:
    /// - Attacker stat and base modifier
    /// - Defending stat and base modifier
    /// - Breakdown of contributing modifiers (traits, assists, etc.)
    /// - Final attack and defense values
    /// </summary>
    [HarmonyPatch(typeof(TINotificationQueueState), nameof(TINotificationQueueState.LogMissionOutcome))]
    public class TINotificationQueueState_MissionDetailBreakdownPatch
    {
        /// <summary>
        /// Postfix to LogMissionOutcome that enhances contested mission notifications
        /// with detailed attack/defense breakdown information.
        /// </summary>
        [HarmonyPostfix]
        public static void LogMissionOutcome_Postfix(
            TIMissionState mission,
            MissionResult result,
            TIFactionState heldTargetFaction,
            List<TIGameState> newControlPoints = null,
            List<TIGameState> oldControlPoints = null,
            bool spy = false,
            string abortedReason = "")
        {
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log("[MissionDetailBreakdown] ===== BREAKDOWN PATCH ENTRY =====");
                Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Mission: {0}", mission?.displayName ?? "NULL"));
                Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Result outcome: {0}", mission != null ? result.missionOutcome.ToString() : "NULL"));
                Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Is contested: {0}", mission != null ? mission.missionTemplate.ContestedMission : false));
            }

            // Only enhance contested missions (not aborted ones)
            if (mission == null || result.missionOutcome == TIMissionOutcome.Aborted)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[MissionDetailBreakdown] EARLY EXIT: mission null or aborted");
                }
                return;
            }

            if (!mission.missionTemplate.ContestedMission)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[MissionDetailBreakdown] EARLY EXIT: mission not contested");
                }
                return;
            }

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Processing contested mission: {0}", mission.displayName));
            }

            // Get the notification queue to find and modify the most recently added item
            TINotificationQueueState notificationQueue = GameStateManager.NotificationQueue();
            if (notificationQueue == null || notificationQueue.notificationQueue.Count == 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[MissionDetailBreakdown] EARLY EXIT: Notification queue is null or empty");
                }
                return;
            }

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Notification queue has {0} items", notificationQueue.notificationQueue.Count));
            }

            // The most recently added item is at index 0 (items are inserted at front)
            NotificationQueueItem recentNotification = notificationQueue.notificationQueue[0];
            if (recentNotification == null || recentNotification.mission != mission)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] EARLY EXIT: Notification mismatch"));
                    Main.mod.Logger.Log(string.Format("  Recent notification mission: {0}", recentNotification?.mission?.displayName ?? "NULL"));
                    Main.mod.Logger.Log(string.Format("  Expected mission: {0}", mission.displayName));
                }
                return;
            }

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log("[MissionDetailBreakdown] Notification found, building breakdown...");
            }

            // Build the detailed breakdown
            string breakdown = BuildMissionBreakdown(mission, result, heldTargetFaction);

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] heldTargetFaction: {0}", heldTargetFaction?.displayName ?? "NULL"));
            }
            if (!string.IsNullOrEmpty(breakdown))
            {
                // Append breakdown to the existing detail text
                recentNotification.itemDetail += "\n\n" + breakdown;

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionDetailBreakdown] SUCCESS: Enhanced mission '{0}' with attack/defense breakdown",
                        mission.displayName));
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Breakdown length: {0} chars", breakdown.Length));
                }
            }
            else if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log("[MissionDetailBreakdown] WARNING: Failed to build breakdown - returned empty string");
            }

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log("[MissionDetailBreakdown] ===== BREAKDOWN PATCH EXIT =====");
            }
        }



        /// <summary>
        /// Builds a detailed breakdown of attack and defense values and their contributors.
        /// Handles both councilor vs councilor and councilor vs control point missions.
        /// </summary>
        private static string BuildMissionBreakdown(TIMissionState mission, MissionResult result, TIFactionState defendingFaction = null)
        {
            try
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[MissionDetailBreakdown] ===== BUILD BREAKDOWN START =====");
                }

                TIMissionTemplate missionTemplate = mission.missionTemplate;
                TICouncilorState councilor = mission.councilor;
                TIGameState target = mission.target;
                TICouncilorState targetCouncilor = target as TICouncilorState;

                // defendingFaction is already provided by the game via the parameter

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Mission template: {0}", missionTemplate?.friendlyName ?? "NULL"));
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Councilor (attacker): {0}", councilor?.displayName ?? "NULL"));
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Target: {0}", target?.displayName ?? "NULL"));
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Target type: {0}", target != null ? target.GetType().Name : "NULL"));
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Target is councilor: {0}", targetCouncilor != null));
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Defending faction after resolution: {0}", defendingFaction?.displayName ?? "NULL"));
                }

                // Validate core requirements: councilor must exist, target must exist, must be contested
                if (councilor == null || target == null || !(missionTemplate.resolutionMethod is TIMissionResolution_Contested))
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log("[MissionDetailBreakdown] VALIDATION FAILED:");
                        Main.mod.Logger.Log(string.Format("  Councilor null: {0}", councilor == null));
                        Main.mod.Logger.Log(string.Format("  Target null: {0}", target == null));
                        Main.mod.Logger.Log(string.Format("  Is contested: {0}", missionTemplate.resolutionMethod is TIMissionResolution_Contested));
                    }
                    return string.Empty;
                }

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[MissionDetailBreakdown] Validation passed, retrieving modifiers...");
                }

                TIMissionResolution_Contested contestedResolution = missionTemplate.resolutionMethod as TIMissionResolution_Contested;

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[MissionDetailBreakdown] Retrieving attacking modifiers...");
                }

                // Retrieve attacking modifiers from the cache (populated during mission calculation)
                List<TIMissionModifier> attackingModifiers = MissionCalculationCache.GetCachedAttackingModifiers(
                    missionTemplate, councilor, target);

                // Fallback to GetNonZeroModifiers if not in cache (shouldn't happen in normal flow)
                if (attackingModifiers == null)
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log("[MissionDetailBreakdown] Attacking modifiers not in cache, using fallback");
                    }
                    attackingModifiers = contestedResolution.GetAttackingNonZeroModifiers(
                        missionTemplate, councilor, target, 0f) ?? new List<TIMissionModifier>();
                }

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Attacking modifiers count: {0}", attackingModifiers?.Count ?? 0));
                    if (attackingModifiers != null)
                    {
                        foreach (var mod in attackingModifiers)
                        {
                            Main.mod.Logger.Log(string.Format("  - {0}", mod.displayName));
                        }
                    }
                }

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[MissionDetailBreakdown] Retrieving defending modifiers...");
                }

                // Retrieve defending modifiers from the cache (populated during mission calculation)
                List<TIMissionModifier> defendingModifiers = MissionCalculationCache.GetCachedDefendingModifiers(
                    missionTemplate, councilor, target);

                // Fallback to GetNonZeroModifiers if not in cache (shouldn't happen in normal flow)
                if (defendingModifiers == null)
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log("[MissionDetailBreakdown] Defending modifiers not in cache, using fallback");
                    }
                    defendingModifiers = contestedResolution.GetDefendingNonZeroModifiers(
                        missionTemplate, councilor, target, 0f) ?? new List<TIMissionModifier>();
                }

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Defending modifiers count: {0}", defendingModifiers?.Count ?? 0));
                    if (defendingModifiers != null)
                    {
                        foreach (var mod in defendingModifiers)
                        {
                            Main.mod.Logger.Log(string.Format("  - {0}", mod.displayName));
                        }
                    }
                }

                // Build header
                StringBuilder breakdown = new StringBuilder();
                breakdown.AppendLine("═══ ATTACK / DEFENSE BREAKDOWN ═══");

                // Calculate attacking totals
                float attackModifierTotal = 0f;
                float assistBonusAttack = AssistBonusTracker.GetStatBonus(councilor, missionTemplate.primaryAttackerStat);

                breakdown.AppendLine();
                breakdown.AppendLine("ATTACKING:");
                breakdown.AppendFormat("  Attacker: {0}\n", councilor.displayName);

                // List attacking modifiers (which already include the base stat)
                if (attackingModifiers.Count > 0)
                {
                    foreach (TIMissionModifier modifier in attackingModifiers)
                    {
                        try
                        {
                            float modValue = modifier.GetModifier(councilor, target, 0f, missionTemplate.primaryResource);
                            breakdown.AppendFormat("    • {0}: {1:+0.00;-0.00}\n", modifier.displayName, modValue);
                            attackModifierTotal += modValue;
                        }
                        catch (Exception modEx)
                        {
                            if (Main.mod != null && Main.settings.debugLogging)
                            {
                                Main.mod.Logger.Log(string.Format(
                                    "[MissionDetailBreakdown] Skipping attacking modifier '{0}' (incompatible with target type): {1}",
                                    modifier.displayName, modEx.Message));
                            }
                            // Skip this modifier silently - some modifiers don't support non-councilor targets
                        }
                    }
                }

                // Show assist bonus separately
                if (assistBonusAttack > 0)
                {
                    breakdown.AppendFormat("    • Assist Bonus: {0:+0.00}\n", assistBonusAttack);
                }

                // Show total attack (modifiers already include the base stat)
                float totalAttackValue = attackModifierTotal + assistBonusAttack;
                breakdown.AppendFormat("  Total Attack: {0:0.00}\n", totalAttackValue);

                breakdown.AppendLine();
                breakdown.AppendLine("DEFENDING:");

                // Calculate defending totals
                float defendModifierTotal = 0f;

                // Handle both councilor and control point targets
                if (targetCouncilor != null)
                {
                    breakdown.AppendFormat("  Defender: {0}\n", targetCouncilor.displayName);
                }
                else
                {
                    // Non-councilor target (control point, faction, etc.)
                    breakdown.AppendFormat("  Defender: {0}\n", target.displayName);
                }

                // List defending modifiers (which already include the base defense stat)
                if (defendingModifiers.Count > 0)
                {
                    foreach (TIMissionModifier modifier in defendingModifiers)
                    {
                        try
                        {
                            // Try with the target councilor first (for councilor vs councilor missions)
                            float modValue = modifier.GetModifier(targetCouncilor, mission.target, 0f, missionTemplate.primaryResource);
                            breakdown.AppendFormat("    • {0}: {1:+0.00;-0.00}\n", modifier.displayName, modValue);
                            defendModifierTotal += modValue;
                        }
                        catch (Exception modEx)
                        {
                            // For control points, some modifiers may need the defending faction instead
                            if (targetCouncilor == null && mission.target is TIControlPoint && defendingFaction != null)
                            {
                                try
                                {
                                    // Get the leader of the defending faction to use for the modifier
                                    TICouncilorState defendingLeader = null;
                                    // Try to get the faction leader or councilors
                                    var activeCouncilors = defendingFaction.GetType().GetProperty("activeCouncilors", System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                    if (activeCouncilors != null)
                                    {
                                        var councilors = activeCouncilors.GetValue(defendingFaction) as System.Collections.IList;
                                        if (councilors != null && councilors.Count > 0)
                                        {
                                            defendingLeader = councilors[0] as TICouncilorState;
                                        }
                                        else if (Main.mod != null && Main.settings.debugLogging)
                                        {
                                            Main.mod.Logger.Log(string.Format(
                                                "[MissionDetailBreakdown] activeCouncilors property found but empty or null (count: {0})",
                                                councilors?.Count ?? -1));
                                        }
                                    }
                                    else if (Main.mod != null && Main.settings.debugLogging)
                                    {
                                        Main.mod.Logger.Log("[MissionDetailBreakdown] activeCouncilors property not found on defending faction");
                                    }

                                    if (defendingLeader != null)
                                    {
                                        // Try calling the modifier with the defending faction's leader
                                        float modValue = modifier.GetModifier(defendingLeader, mission.target, 0f, missionTemplate.primaryResource);
                                        breakdown.AppendFormat("    • {0}: {1:+0.00;-0.00}\n", modifier.displayName, modValue);
                                        defendModifierTotal += modValue;
                                    }
                                    else
                                    {
                                        if (Main.mod != null && Main.settings.debugLogging)
                                        {
                                            Main.mod.Logger.Log(string.Format(
                                                "[MissionDetailBreakdown] No defending leader found for modifier '{0}'",
                                                modifier.displayName));
                                        }
                                        throw modEx; // No defending leader found
                                    }
                                }
                                catch (Exception innerEx)
                                {
                                    if (Main.mod != null && Main.settings.debugLogging)
                                    {
                                        Main.mod.Logger.Log(string.Format(
                                            "[MissionDetailBreakdown] Skipping defending modifier '{0}' (fallback failed): {1}",
                                            modifier.displayName, innerEx.Message));
                                    }
                                    // Skip this modifier - can't get defending councilor or modifier doesn't support it
                                }
                            }
                            else
                            {
                                if (Main.mod != null && Main.settings.debugLogging)
                                {
                                    Main.mod.Logger.Log(string.Format(
                                        "[MissionDetailBreakdown] Skipping defending modifier '{0}' (no fallback): targetCouncilor={1}, isControlPoint={2}, defendingFaction={3}",
                                        modifier.displayName, 
                                        targetCouncilor != null,
                                        mission.target is TIControlPoint,
                                        defendingFaction != null));
                                }
                                // Skip this modifier silently - some modifiers don't support non-councilor targets
                            }
                        }
                    }
                }

                // Show total defense (modifiers already include the baseline)
                float totalDefenseValue = defendModifierTotal;
                breakdown.AppendFormat("  Total Defense: {0:0.00}\n", totalDefenseValue);

                breakdown.AppendLine();
                breakdown.AppendFormat("Success Chance: {0:P2}", result.successChance);
                breakdown.AppendFormat(" | Roll: {0:P2}", result.roll);

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[MissionDetailBreakdown] Successfully built breakdown");
                }

                return breakdown.ToString();
            }
            catch (Exception ex)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionDetailBreakdown] Error building breakdown: {0}\n{1}", ex.Message, ex.StackTrace));
                }
                return string.Empty;
            }
        }
    }
}
