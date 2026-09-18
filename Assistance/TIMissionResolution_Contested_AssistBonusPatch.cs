using System;
using System.Collections.Generic;
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
        /// GUARD CLAUSE: Silently ignores AI attacker calculations to prevent thread-unsafe string allocation.
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
            // Structural Guard Clause: Reject corrupted frames instantly
            if (mission == null || councilor == null)
            {
                return;
            }

            // CRITICAL GUARD CLAUSE: Drop out silently if the attacker isn't human-controlled
            if (!AssistBonusTracker.IsPlayerControlled(councilor))
            {
                return; // Silent exit - zero log allocation for AI background turn phases
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

            // Safe Player Execution: This code block will now ONLY execute if a player-controlled attacker is active.
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("[AssistMission] Processing player attacker: {0}", councilor.displayName));
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
                    "[AssistBonusTracker] ✓ APPLIED {0} assist bonus ({1} points) to attacking modifier", missionAttribute, cappedBonus));
                Main.mod.Logger.Log(string.Format("  Result changed: {0} → {1}", originalResult, __result));
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
            TICouncilorState councilor, // Fix: Parameter name matching restored to fix runtime crash
            TIGameState target,
            float resourcesSpent,
            ref float __result)
        {
            // Structural Guard Clause: Reject corrupted frames instantly
            if (mission == null || councilor == null)
            {
                return;
            }

            // FIXING THE VARIABLE LEAK:
            // The primary game actor maps to 'councilor'.
            // For aggressive missions like Assassinate, the true target is stored in the 'target' argument.
            TICouncilorState trueDefender = councilor;

            if (target is TICouncilorState targetedVictim)
            {
                trueDefender = targetedVictim; // Correctly redirects the evaluation target to Alfa-18
            }

            // CRITICAL GUARD CLAUSE: Drop out silently if the actual target isn't human-controlled
            // Optimization: References centralized helper block to eliminate code redundancy
            if (trueDefender == null || !AssistBonusTracker.IsPlayerControlled(trueDefender))
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

        /// <summary>
        /// Postfix patch for GetDefendingNonZeroModifiers to cache defending modifiers at calculation time.
        /// This ensures the breakdown display shows modifiers as they were during resolution,
        /// not as they appear after rewards have been added.
        /// 
        /// CRITICAL: We pass the primary mission actor (councilor) as the cache key to match
        /// what the UI breakdown screen looks up, ensuring cache hits are consistent.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.GetDefendingNonZeroModifiers))]
        [HarmonyPostfix]
        public static void GetDefendingNonZeroModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState councilor, // The primary mission actor (Catherine)
            TIGameState target,
            float resourcesSpent,
            List<TIMissionModifier> __result)
        {
            if (mission == null || councilor == null || target == null || __result == null)
            {
                return;
            }

            // 1. For structural logic, resolve the true defender to check if player-relevant
            TICouncilorState trueDefender = councilor;
            if (target is TICouncilorState targetedCouncilor)
            {
                trueDefender = targetedCouncilor; 
            }

            // 2. Ensure we only cache player-relevant actions to suppress AI bloat
            if (!AssistBonusTracker.IsPlayerControlled(councilor) && !AssistBonusTracker.IsPlayerControlled(trueDefender))
            {
                return; 
            }

            // 3. FIX: Always pass 'councilor' (the attacker) as the second argument here!
            // This forces the cache key string to generate as "Assassinate:Catherine:Alfa-18",
            // perfectly matching what the UI breakdown screen looks up later.
            MissionCalculationCache.CacheDefendingModifiers(mission, councilor, target, __result);
        }
    }
}
