using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Patches TIMissionResolution_Contested to apply assist bonuses when calculating
    /// contested mission modifiers. Assist bonuses apply to both:
    /// 1. Target councilor's DEFENDING modifiers (helps them resist attacks)
    /// 2. Attacking councilor's ATTACKING modifiers (helps them attack the target)
    /// 
    /// This dual application means assist bonuses boost the target's defensive capability
    /// while also providing an offensive bonus when the assisted councilor attacks.
    /// 
    /// Bonuses are applied as flat bonus points, using the same percentage-based pool 
    /// calculated in TIMissionEffect_Assist.
    /// </summary>
    [HarmonyPatch]
    public class TIMissionResolution_Contested_AssistBonusPatch
    {
        /// <summary>
        /// Applies assist bonus to attacking modifiers when the assisted councilor attacks.
        /// If the attacking councilor received assist bonuses, they boost their attack power.
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

            // Apply assist bonuses to attacking councilor's attacking modifiers
            // (The councilor doing the attacking might have received assist bonuses)
            if (councilor == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] Attacking councilor is NULL, skipping bonus application");
                return;
            }

            int assistBonus = AssistBonusTracker.GetTotalBonus(councilor);

            if (assistBonus <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] No assist bonus for attacker '{0}' (bonus: {1})", councilor.displayName, assistBonus));
                return;
            }

            // Apply flat bonus points directly to attacking modifiers
            float originalResult = __result;
            __result += assistBonus;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] APPLIED {0} assist bonus points to attacking modifier for '{1}' - Result changed from {2} to {3}",
                    assistBonus, councilor.displayName, originalResult, __result));
            }
        }

        /// <summary>
        /// Applies assist bonus to defending modifiers when the assisted councilor is attacked.
        /// If the defending councilor received assist bonuses, they boost their defense power.
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

            // Apply assist bonuses to target councilor's defending modifiers
            if (target == null || !target.isCouncilorState)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] Target is NULL or not a councilor, skipping bonus application");
                return;
            }

            TICouncilorState targetCouncilor = target as TICouncilorState;
            if (targetCouncilor == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] Failed to cast target to TICouncilorState");
                return;
            }

            int assistBonus = AssistBonusTracker.GetTotalBonus(targetCouncilor);

            if (assistBonus <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] No assist bonus for defender '{0}' (bonus: {1})", targetCouncilor.displayName, assistBonus));
                return;
            }

            // Convert flat bonus points directly to defending modifier bonus
            // The bonus pool was calculated using the same percentage method,
            // so we preserve the flat point system by adding directly
            float originalResult = __result;
            __result += assistBonus;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[AssistBonusTracker] APPLIED {0} assist bonus points to defending modifier for '{1}' - Result changed from {2} to {3}",
                    assistBonus, targetCouncilor.displayName, originalResult, __result));
            }
        }
    }
}
