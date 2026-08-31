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
            if (!Main.enabled || Main.settings == null || !Main.settings.enableAssistMission)
            {
                return string.Empty;
            }

            TICouncilorState assistingCouncilor = mission.councilor;
            TICouncilorState targetCouncilor = target as TICouncilorState;

            if (assistingCouncilor == null || targetCouncilor == null)
            {
                return string.Empty;
            }

            float assistPercentage = Main.settings.assistPercentage / 100f;

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

                if (assistAmount > 0 && MissionSuccess(outcome))
                {
                    targetCouncilor.ModifyAttribute(stat, assistAmount);
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
                return string.Format("Assisted {0}: {1}", targetCouncilor.displayName, result.ToString());
            }

            return string.Empty;
        }
    }
}
