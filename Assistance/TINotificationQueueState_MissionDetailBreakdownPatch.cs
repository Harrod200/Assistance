using System;
using System.Collections.Generic;
using System.Linq;
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
            // Only enhance contested missions (not aborted ones)
            if (mission == null || result.missionOutcome == TIMissionOutcome.Aborted)
                return;

            if (!mission.missionTemplate.ContestedMission)
                return;

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
                    Main.mod.Logger.Log("[MissionDetailBreakdown] Notification queue is null or empty");
                }
                return;
            }

            // The most recently added item is at index 0 (items are inserted at front)
            NotificationQueueItem recentNotification = notificationQueue.notificationQueue[0];
            if (recentNotification == null || recentNotification.mission != mission)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Notification mismatch - recent: {0}, mission: {1}", 
                        recentNotification?.mission?.displayName ?? "NULL", mission.displayName));
                }
                return;
            }

            // Build the detailed breakdown
            string breakdown = BuildMissionBreakdown(mission, result);
            if (!string.IsNullOrEmpty(breakdown))
            {
                // Append breakdown to the existing detail text
                recentNotification.itemDetail += "\n\n" + breakdown;

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionDetailBreakdown] Enhanced mission '{0}' with attack/defense breakdown",
                        mission.displayName));
                }
            }
            else if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log("[MissionDetailBreakdown] Failed to build breakdown");
            }
        }

        /// <summary>
        /// Builds a detailed breakdown of attack and defense values and their contributors.
        /// Handles both councilor vs councilor and councilor vs control point missions.
        /// </summary>
        private static string BuildMissionBreakdown(TIMissionState mission, MissionResult result)
        {
            try
            {
                TIMissionTemplate missionTemplate = mission.missionTemplate;
                TICouncilorState councilor = mission.councilor;
                TIGameState target = mission.target;
                TICouncilorState targetCouncilor = target as TICouncilorState;

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] BuildMissionBreakdown - councilor: {0}, target: {1}, targetCouncilor: {2}",
                        councilor?.displayName ?? "NULL", target?.displayName ?? "NULL", targetCouncilor?.displayName ?? "NULL"));
                }

                // Validate core requirements: councilor must exist, target must exist, must be contested
                if (councilor == null || target == null || !(missionTemplate.resolutionMethod is TIMissionResolution_Contested))
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Invalid parameters - councilor: {0}, target: {1}, isContested: {2}",
                            councilor != null, target != null, missionTemplate.resolutionMethod is TIMissionResolution_Contested));
                    }
                    return string.Empty;
                }

                TIMissionResolution_Contested contestedResolution = missionTemplate.resolutionMethod as TIMissionResolution_Contested;

                // Get attacking and defending modifiers
                List<TIMissionModifier> attackingModifiers = contestedResolution.GetAttackingNonZeroModifiers(
                    missionTemplate, councilor, target, 0f);
                List<TIMissionModifier> defendingModifiers = contestedResolution.GetDefendingNonZeroModifiers(
                    missionTemplate, councilor, target, 0f);

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[MissionDetailBreakdown] Modifiers - attacking: {0}, defending: {1}",
                        attackingModifiers?.Count ?? 0, defendingModifiers?.Count ?? 0));
                }

                // Build header
                StringBuilder breakdown = new StringBuilder();
                breakdown.AppendLine("═══ ATTACK / DEFENSE BREAKDOWN ═══");

                // Get primary attacking stat
                CouncilorAttribute primaryAttackerStat = missionTemplate.primaryAttackerStat;
                CouncilorAttribute primaryDefenderStat = missionTemplate.primaryDefenderStat();

                breakdown.AppendLine();
                breakdown.AppendLine("ATTACKING:");
                breakdown.AppendFormat("  Attacker: {0} ({1})\n", councilor.displayName, primaryAttackerStat);
                breakdown.AppendFormat("  Base {0}: {1}\n", primaryAttackerStat, 
                    councilor.GetAttribute(primaryAttackerStat, true, true, true, false, false, false));

                // List attacking modifiers
                if (attackingModifiers.Count > 0)
                {
                    breakdown.AppendLine("  Modifiers:");
                    foreach (TIMissionModifier modifier in attackingModifiers)
                    {
                        float modValue = modifier.GetModifier(councilor, target, 0f, missionTemplate.primaryResource);
                        breakdown.AppendFormat("    • {0}: {1:+0.00;-0.00}\n", modifier.displayName, modValue);
                    }
                }

                breakdown.AppendLine();
                breakdown.AppendLine("DEFENDING:");

                // Handle both councilor and control point targets
                if (targetCouncilor != null)
                {
                    breakdown.AppendFormat("  Defender: {0} ({1})\n", targetCouncilor.displayName, primaryDefenderStat);
                    breakdown.AppendFormat("  Base {0}: {1}\n", primaryDefenderStat,
                        targetCouncilor.GetAttribute(primaryDefenderStat, true, true, true, false, false, false));
                }
                else
                {
                    // Non-councilor target (control point, faction, etc.)
                    breakdown.AppendFormat("  Defender: {0}\n", target.displayName);
                    breakdown.AppendLine("  Base Defense: N/A (non-councilor target)");
                }

                // List defending modifiers
                if (defendingModifiers.Count > 0)
                {
                    breakdown.AppendLine("  Modifiers:");
                    foreach (TIMissionModifier modifier in defendingModifiers)
                    {
                        // For non-councilor targets, pass null as councilor parameter where needed
                        float modValue = modifier.GetModifier(targetCouncilor, mission.target, 0f, missionTemplate.primaryResource);
                        breakdown.AppendFormat("    • {0}: {1:+0.00;-0.00}\n", modifier.displayName, modValue);
                    }
                }

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
