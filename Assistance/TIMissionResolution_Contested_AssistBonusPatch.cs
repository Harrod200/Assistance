using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

namespace Assistance
{
    /// <summary>
    /// Patches TIMissionResolution_Contested to apply assist bonuses when calculating
    /// contested mission modifiers. Each stat bonus is applied separately based on which
    /// mission attribute is being checked.
    /// 
    /// For example:
    /// - Persuasion checks get only the Persuasion assist bonus
    /// - Command checks get only the Command assist bonus
    /// 
    /// This ensures that assist bonuses are stat-specific rather than pooled together.
    /// </summary>
    [HarmonyPatch]
    public class TIMissionResolution_Contested_AssistBonusPatch
    {
        /// <summary>
        /// Helper method: Determines if a councilor is player-controlled.
        /// Reuses the consistent structural logic added in commit b95300d.
        /// </summary>
        private static bool IsPlayerControlled(TICouncilorState councilor)
        {
            return councilor != null && 
                   councilor.faction != null && 
                   councilor.faction.player != null && 
                   !councilor.faction.player.isAI;
        }

        /// <summary>
        /// Determines which CouncilorAttribute is used by a mission for its attacking stat check.
        /// Uses the mission's primaryAttackerStat property which is determined by the mission's
        /// attacking modifiers.
        /// </summary>
        private static CouncilorAttribute GetMissionAttackerAttribute(TIMissionTemplate mission)
        {
            if (mission == null)
                return CouncilorAttribute.Persuasion; // Default fallback

            // Use the mission's own primaryAttackerStat property to determine which stat is used
            CouncilorAttribute attackerStat = mission.primaryAttackerStat;

            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Mission '{0}' primary attacker stat: {1}", 
                    mission.friendlyName, attackerStat));

            return attackerStat;
        }

        /// <summary>
        /// Determines which CouncilorAttribute is used by a mission for its defending stat check.
        /// Uses the mission's primaryDefenderStat() method, which can differ from the attacking
        /// stat (e.g. an attacker's Persuasion check resolved against a defender's Investigation).
        /// </summary>
        private static CouncilorAttribute GetMissionDefenderAttribute(TIMissionTemplate mission)
        {
            if (mission == null)
                return CouncilorAttribute.Persuasion; // Default fallback

            CouncilorAttribute defenderStat = mission.primaryDefenderStat();

            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Mission '{0}' primary defender stat: {1}", 
                    mission.friendlyName, defenderStat));

            return defenderStat;
        }

        /// <summary>
        /// Applies assist bonus to attacking modifiers when the assisted councilor attacks.
        /// Only the bonus for the mission's specific stat attribute is applied.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.SumAttackingModifiers))]
        [HarmonyPostfix]
        public static void SumAttackingModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            float resourcesSpent,
            ref float __result)
        {
            // Log entry to contested mission check
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log("[AssistBonusTracker] SumAttackingModifiers ENTRY");
                Main.mod.Logger.Log(string.Format("  Mission: {0}", mission != null ? mission.friendlyName : "NULL"));
                Main.mod.Logger.Log(string.Format("  Attacker: {0}", councilor != null ? councilor.displayName : "NULL"));
                Main.mod.Logger.Log(string.Format("  Target type: {0}", target != null ? target.GetType().Name : "NULL"));
                Main.mod.Logger.Log(string.Format("  Target: {0}", target != null ? target.displayName : "NULL"));
                Main.mod.Logger.Log(string.Format("  Result before: {0}", __result));
            }

            if (councilor == null || mission == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[AssistBonusTracker] EARLY EXIT: Attacking councilor or mission is NULL");
                }
                return;
            }

            // Skip non-player councilor missions unless targeting a player councilor or assisted region
            if (councilor.faction != null && councilor.faction.player != null && councilor.faction.player.isAI)
            {
                // This is an AI councilor - check if we should process it
                bool isRelevant = false;

                // Check if target is a player-controlled councilor
                TICouncilorState targetCouncilor = target as TICouncilorState;
                if (targetCouncilor != null && targetCouncilor.faction != null && 
                    targetCouncilor.faction.player != null && !targetCouncilor.faction.player.isAI)
                {
                    isRelevant = true;
                }

                // Check if target is a control point in a player-assisted region
                if (!isRelevant && target is TIControlPoint)
                {
                    // Skip AI vs control point missions - they're not relevant to player assists
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log("[AssistBonusTracker] SKIP: AI councilor attacking non-player target (control point)");
                    }
                    return;
                }

                // If still not relevant, skip this mission
                if (!isRelevant)
                {
                    if (Main.mod != null && Main.settings.debugLogging)
                    {
                        Main.mod.Logger.Log("[AssistBonusTracker] SKIP: AI councilor attacking non-player target");
                    }
                    return;
                }
            }

            // Cache the attacking modifiers at calculation time (before assist bonus is applied)
            // This ensures the breakdown shows modifiers as they were during resolution
            if (mission != null && councilor != null && target != null)
            {
                TIMissionResolution_Contested contestedResolution = mission.resolutionMethod as TIMissionResolution_Contested;
                if (contestedResolution != null)
                {
                    var attackingModifiers = contestedResolution.GetAttackingNonZeroModifiers(mission, councilor, target, resourcesSpent);
                    if (attackingModifiers != null)
                    {
                        MissionCalculationCache.CacheAttackingModifiers(mission, councilor, target, attackingModifiers);
                    }
                }
            }

            // Get the mission's attacking attribute (e.g., Persuasion, Command)
            CouncilorAttribute missionAttribute = GetMissionAttackerAttribute(mission);

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("  Mission attacking attribute: {0}", missionAttribute));
            }

            // Apply only the bonus for this specific stat
            int statBonus = AssistBonusTracker.GetStatBonus(councilor, missionAttribute);

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("  Stat bonus for {0}: {1}", missionAttribute, statBonus));
            }

            if (statBonus <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] NO BONUS: {0} assist bonus for attacker '{1}' is {2}", 
                        missionAttribute, councilor.displayName, statBonus));
                }
                return;
            }

            // Apply stat cap if enabled
            int cappedBonus = statBonus;
            if (Main.settings.statCapEnabled)
            {
                // Get the base stat value
                int baseStatValue = councilor.GetAttribute(missionAttribute, true, true, true, false, false, false);

                // Calculate total with the full bonus
                int totalWithBonus = baseStatValue + statBonus;

                // Apply cap: max(base, min(total, cap))
                int cappedTotal = Mathf.Max(baseStatValue, Mathf.Min(totalWithBonus, Main.settings.statCapLimit));

                // The capped bonus is the difference between capped total and base
                cappedBonus = cappedTotal - baseStatValue;

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[AssistBonusTracker] Stat cap applied: base={0}, bonus={1}, capped_bonus={2}, total={3}", 
                        baseStatValue, statBonus, cappedBonus, cappedTotal));
                }
            }

            // Apply only this stat's (possibly capped) bonus to attacking modifiers
            float originalResult = __result;
            __result += cappedBonus;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] ? APPLIED {0} assist bonus ({1} points) to attacking modifier", missionAttribute, cappedBonus));
                Main.mod.Logger.Log(string.Format("  Result changed: {0} ? {1}", originalResult, __result));
            }
        }

        /// <summary>
        /// Postfix Patch for SumDefendingModifiers calculation engine.
        /// Resolves the variable leak bug by correctly identifying the targeted asset.
        /// GUARD CLAUSE: Silently ignores calculations for non-player targets to stop log allocation errors.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.SumDefendingModifiers))]
        [HarmonyPostfix]
        public static void SumDefendingModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState defender,
            TIGameState target,
            float resourcesSpent,
            ref float __result)
        {
            // Structural Guard Clause: Reject corrupted frames instantly
            if (mission == null || defender == null)
            {
                return;
            }

            // FIXING THE VARIABLE LEAK:
            // The first argument 'defender' maps to the primary player councilor (Catherine).
            // For aggressive missions like Assassinate, the true target is stored in the 'target' argument.
            TICouncilorState trueDefender = defender;

            if (target is TICouncilorState targetedVictim)
            {
                trueDefender = targetedVictim; // Correctly redirects the evaluation target to Alfa-18
            }

            // CRITICAL GUARD CLAUSE: Drop out silently if the actual target isn't human-controlled
            if (trueDefender == null || !IsPlayerControlled(trueDefender))
            {
                return; // Silent exit - zero log spam or thread exceptions generated by AI activities
            }

            // Cache the defending modifiers at calculation time (before assist bonus is applied)
            // This ensures the breakdown shows modifiers as they were during resolution
            if (mission != null && trueDefender != null && target != null)
            {
                TIMissionResolution_Contested contestedResolution = mission.resolutionMethod as TIMissionResolution_Contested;
                if (contestedResolution != null)
                {
                    var defendingModifiers = contestedResolution.GetDefendingNonZeroModifiers(mission, trueDefender, target, resourcesSpent);
                    if (defendingModifiers != null)
                    {
                        MissionCalculationCache.CacheDefendingModifiers(mission, trueDefender, target, defendingModifiers);
                    }
                }
            }

            // Safe Player Execution: This code block will now ONLY execute if a player-controlled asset is being attacked.
            // When Catherine attacks Alfa-18, trueDefender is Alfa-18 (AI), causing it to exit cleanly above with 0 additions!
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("[AssistMission] Processing true player target defense: {0}", trueDefender.displayName));
            }

            // Get the mission's defending attribute (e.g., Persuasion, Command)
            CouncilorAttribute missionAttribute = GetMissionDefenderAttribute(mission);

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("  Mission defending attribute: {0}", missionAttribute));
            }

            // Apply only the bonus for this specific stat
            int statBonus = AssistBonusTracker.GetStatBonus(trueDefender, missionAttribute);

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("  Stat bonus for {0}: {1}", missionAttribute, statBonus));
            }

            if (statBonus <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] NO BONUS: {0} assist bonus for defender '{1}' is {2}", 
                        missionAttribute, trueDefender.displayName, statBonus));
                }
                return;
            }

            // Apply stat cap if enabled
            int cappedBonus = statBonus;
            if (Main.settings.statCapEnabled)
            {
                // Get the base stat value for the defending councilor
                int baseStatValue = trueDefender.GetAttribute(missionAttribute, true, true, true, false, false, false);

                // Calculate total with the full bonus
                int totalWithBonus = baseStatValue + statBonus;

                // Apply cap: max(base, min(total, cap))
                int cappedTotal = Mathf.Max(baseStatValue, Mathf.Min(totalWithBonus, Main.settings.statCapLimit));

                // The capped bonus is the difference between capped total and base
                cappedBonus = cappedTotal - baseStatValue;

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[AssistBonusTracker] Stat cap applied: base={0}, bonus={1}, capped_bonus={2}, total={3}", 
                        baseStatValue, statBonus, cappedBonus, cappedTotal));
                }
            }

            // Apply only this stat's (possibly capped) bonus to defending modifiers
            float originalResult = __result;
            __result += cappedBonus;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] ✓ APPLIED {0} assist bonus ({1} points) to defending modifier", missionAttribute, cappedBonus));
                Main.mod.Logger.Log(string.Format("  Result changed: {0} → {1}", originalResult, __result));
            }
        }
    }
}
