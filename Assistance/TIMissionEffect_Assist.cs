using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

namespace Assistance
{
    public class TIMissionEffect_Assist : TIMissionEffect
    {
        public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
        {
            if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] TIMissionEffect_Assist.ApplyEffect called - Enabled: {0}, Mission outcome: {1}", Main.enabled, outcome));

            if (!Main.enabled || Main.settings == null || !Main.settings.enableAssistMission)
            {
                if (Main.mod != null && Main.settings != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] Assist mission effect skipped - mod disabled or settings null");
                return string.Empty;
            }

            TICouncilorState assistingCouncilor = mission.councilor;
            TICouncilorState targetCouncilor = target as TICouncilorState;

            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Assisting councilor: {0}, Target: {1}", 
                    assistingCouncilor != null ? assistingCouncilor.displayName : "NULL",
                    targetCouncilor != null ? targetCouncilor.displayName : "NULL"));

            if (assistingCouncilor == null || targetCouncilor == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] ApplyEffect aborted - null councilor");
                return string.Empty;
            }

            float assistPercentage = Main.settings.assistPercentage / 100f;

            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Assist percentage: {0}% (multiplier: {1})", Main.settings.assistPercentage, assistPercentage));

            CouncilorAttribute[] stats = new CouncilorAttribute[]
            {
                CouncilorAttribute.Persuasion,
                CouncilorAttribute.Investigation,
                CouncilorAttribute.Espionage,
                CouncilorAttribute.Command,
                CouncilorAttribute.Administration,
                CouncilorAttribute.Science,
                CouncilorAttribute.Security
            };

            StringBuilder result = new StringBuilder();
            bool hasMeaningfulEffect = false;

            foreach (CouncilorAttribute stat in stats)
            {
                int assistingValue = assistingCouncilor.GetAttribute(stat, true, true, true, false, false, false);
                int assistAmount = Mathf.Max(1, Mathf.FloorToInt(assistingValue * assistPercentage));

                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] {0}: assister value={1}, assist amount={2}, success={3}", 
                        stat, assistingValue, assistAmount, MissionSuccess(outcome)));

                if (assistAmount > 0 && MissionSuccess(outcome))
                {
                    // CHANGED: Only track the bonus, don't apply it to attributes
                    // Bonuses will only be used during contested mission checks
                    AssistBonusTracker.RecordBonus(targetCouncilor, stat, assistAmount);
                    hasMeaningfulEffect = true;

                    if (result.Length > 0)
                    {
                        result.Append(", ");
                    }
                    result.Append(stat.ToString()).Append(" +").Append(assistAmount);
                }
            }

            if (hasMeaningfulEffect)
            {
                string message = string.Format("Assist tracked for {0}: {1}", targetCouncilor.displayName, result.ToString());
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Effect result: {0}", message));
                return message;
            }

            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log("[AssistBonusTracker] No meaningful effect applied (mission failed or all stat amounts <= 0)");

            return string.Empty;
        }
    }
}
