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

            // Get the notification queue to find and modify the most recently added item
            TINotificationQueueState notificationQueue = GameStateManager.NotificationQueue();
            if (notificationQueue == null || notificationQueue.notificationQueue.Count == 0)
                return;

            // The most recently added item is at index 0 (items are inserted at front)
            NotificationQueueItem recentNotification = notificationQueue.notificationQueue[0];
            if (recentNotification == null || recentNotification.mission != mission)
                return;

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
        }

        /// <summary>
        /// Builds a detailed breakdown of attack and defense values and their contributors.
        /// Includes running totals, the outcome, and an explicit assist-bonus line when one
        /// contributed to the roll.
        /// </summary>
        private static string BuildMissionBreakdown(TIMissionState mission, MissionResult result)
        {
            try
            {
                TIMissionTemplate missionTemplate = mission.missionTemplate;
                TICouncilorState councilor = mission.councilor;
                TIGameState target = mission.target;
                TICouncilorState targetCouncilor = target as TICouncilorState;

                if (councilor == null || targetCouncilor == null || !(missionTemplate.resolutionMethod is TIMissionResolution_Contested))
                    return string.Empty;

                TIMissionResolution_Contested contestedResolution = missionTemplate.resolutionMethod as TIMissionResolution_Contested;

                // Get attacking and defending modifiers
                List<TIMissionModifier> attackingModifiers = contestedResolution.GetAttackingNonZeroModifiers(
                    missionTemplate, councilor, target, 0f);
                List<TIMissionModifier> defendingModifiers = contestedResolution.GetDefendingNonZeroModifiers(
                    missionTemplate, councilor, target, 0f);

                CouncilorAttribute primaryAttackerStat = missionTemplate.primaryAttackerStat;
                CouncilorAttribute primaryDefenderStat = missionTemplate.primaryDefenderStat();

                // Assist bonus for the mission's primary stat (may be zero)
                float attackerAssistBonus = AssistBonusTracker.GetStatBonus(councilor, primaryAttackerStat);
                float defenderAssistBonus = AssistBonusTracker.GetStatBonus(targetCouncilor, primaryDefenderStat);

                StringBuilder breakdown = new StringBuilder();
                breakdown.AppendLine("═══ MISSION BREAKDOWN ═══");

                // --- ATTACK ---
                float attackBase = councilor.GetAttribute(primaryAttackerStat, true, true, true, false, false, false);
                float attackModifiers = 0f;
                foreach (TIMissionModifier modifier in attackingModifiers)
                    attackModifiers += modifier.GetModifier(councilor, target, 0f, missionTemplate.primaryResource);

                breakdown.AppendLine();
                breakdown.AppendLine("ATTACK:");
                breakdown.AppendFormat("  {0} — {1} base\n", councilor.displayName, primaryAttackerStat);
                breakdown.AppendFormat("    Base: {0:0.00}\n", attackBase);

                if (attackerAssistBonus > 0)
                    breakdown.AppendFormat("    Assist bonus: {0:+0.00;-0.00}\n", attackerAssistBonus);

                foreach (TIMissionModifier modifier in attackingModifiers)
                {
                    float modValue = modifier.GetModifier(councilor, target, 0f, missionTemplate.primaryResource);
                    breakdown.AppendFormat("    {0}: {1:+0.00;-0.00}\n", modifier.displayName, modValue);
                }

                breakdown.AppendFormat("    TOTAL: {0:0.00}\n", attackBase + attackerAssistBonus + attackModifiers);

                // --- DEFENSE ---
                float defenseBase = targetCouncilor.GetAttribute(primaryDefenderStat, true, true, true, false, false, false);
                float defenseModifiers = 0f;
                foreach (TIMissionModifier modifier in defendingModifiers)
                    defenseModifiers += modifier.GetModifier(targetCouncilor, mission.target, 0f, missionTemplate.primaryResource);

                breakdown.AppendLine();
                breakdown.AppendLine("DEFENSE:");
                breakdown.AppendFormat("  {0} — {1} base\n", targetCouncilor.displayName, primaryDefenderStat);
                breakdown.AppendFormat("    Base: {0:0.00}\n", defenseBase);

                if (defenderAssistBonus > 0)
                    breakdown.AppendFormat("    Assist bonus: {0:+0.00;-0.00}\n", defenderAssistBonus);

                foreach (TIMissionModifier modifier in defendingModifiers)
                {
                    float modValue = modifier.GetModifier(targetCouncilor, mission.target, 0f, missionTemplate.primaryResource);
                    breakdown.AppendFormat("    {0}: {1:+0.00;-0.00}\n", modifier.displayName, modValue);
                }

                breakdown.AppendFormat("    TOTAL: {0:0.00}\n", defenseBase + defenderAssistBonus + defenseModifiers);

                // --- OUTCOME ---
                breakdown.AppendLine();
                breakdown.AppendFormat("Margin: {0:+0.00;-0.00} | Success Chance: {1:P2} | Roll: {2:P2}\n",
                    attackBase + attackerAssistBonus + attackModifiers - (defenseBase + defenderAssistBonus + defenseModifiers),
                    result.successChance, result.roll);
                breakdown.AppendFormat("Outcome: {0}", result.missionOutcome);

                return breakdown.ToString();
            }
            catch (Exception ex)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionDetailBreakdown] Error building breakdown: {0}", ex.Message));
                }
                return string.Empty;
            }
        }
    }
}
