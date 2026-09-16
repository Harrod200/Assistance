using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

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
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] SumAttackingModifiers called - Mission: {0}, Attacker: {1}, Target: {2}, Result before bonus: {3}",
                    mission != null ? mission.friendlyName : "NULL",
                    councilor != null ? councilor.displayName : "NULL",
                    target != null ? (target.isCouncilorState ? ((TICouncilorState)target).displayName : target.ToString()) : "NULL",
                    __result));
            }

            if (councilor == null || mission == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] Attacking councilor or mission is NULL, skipping bonus application");
                return;
            }

            // Get the mission's attacking attribute (e.g., Persuasion, Command)
            CouncilorAttribute missionAttribute = GetMissionAttackerAttribute(mission);

            // Apply only the bonus for this specific stat
            int statBonus = AssistBonusTracker.GetStatBonus(councilor, missionAttribute);

            if (statBonus <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] No {0} assist bonus for attacker '{1}' (bonus: {2})", 
                        missionAttribute, councilor.displayName, statBonus));
                return;
            }

            // Apply only this stat's bonus to attacking modifiers
            float originalResult = __result;
            __result += statBonus;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] APPLIED {0} {1} assist bonus points to attacking modifier for '{2}' - Result changed from {3} to {4}",
                    statBonus, missionAttribute, councilor.displayName, originalResult, __result));
            }
        }

        /// <summary>
        /// Applies assist bonus to defending modifiers when the assisted councilor is attacked.
        /// Only the bonus for the mission's specific stat attribute is applied.
        /// Works for both councilor vs councilor and councilor vs control point missions.
        /// </summary>
        [HarmonyPatch(typeof(TIMissionResolution_Contested), nameof(TIMissionResolution_Contested.SumDefendingModifiers))]
        [HarmonyPostfix]
        public static void SumDefendingModifiers_Postfix(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            float resourcesSpent,
            ref float __result)
        {
            // Log entry to contested mission check
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] SumDefendingModifiers called - Mission: {0}, Defender: {1}, Target: {2}, Result before bonus: {3}",
                    mission != null ? mission.friendlyName : "NULL",
                    councilor != null ? councilor.displayName : "NULL",
                    target != null ? (target.isCouncilorState ? ((TICouncilorState)target).displayName : target.ToString()) : "NULL",
                    __result));
            }

            // Check defending councilor and mission - target type doesn't matter (can be councilor or control point)
            if (councilor == null || mission == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] Defending councilor or mission is NULL - skipping bonus application");
                return;
            }

            // Get the mission's defending attribute (e.g., Persuasion, Command)
            CouncilorAttribute missionAttribute = GetMissionDefenderAttribute(mission);

            // Apply only the bonus for this specific stat
            int statBonus = AssistBonusTracker.GetStatBonus(councilor, missionAttribute);

            if (statBonus <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] No {0} assist bonus for defender '{1}' (bonus: {2})", 
                        missionAttribute, councilor.displayName, statBonus));
                return;
            }

            // Apply only this stat's bonus to defending modifiers
            float originalResult = __result;
            __result += statBonus;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] APPLIED {0} {1} assist bonus points to defending modifier for '{2}' - Result changed from {3} to {4}",
                    statBonus, missionAttribute, councilor.displayName, originalResult, __result));
            }
        }
    }
}
