using System;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Custom modifier that calculates assist bonus based on source councilor's Persuasion stat.
    /// This is applied as part of the mission resolution to determine success chance.
    /// The actual stat bonuses are applied in TIMissionEffect_Assist.
    /// </summary>
    public class TIMissionModifier_AssistStat : TIMissionModifier_CouncilorStat
    {
        public new CouncilorAttribute attackerAttribute = CouncilorAttribute.Persuasion;

        public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
        {
            if (attackingCouncilor == null)
                return 0f;

            // Return the Persuasion stat of the assisting councilor
            // multiplied by the base multiplier from TIMissionModifier_CouncilorStat (default 1.0)
            return (float)attackingCouncilor.GetAttribute(this.attackerAttribute, true, true, true, false, false, false) * this.multiplier;
        }

        public override string displayName
        {
            get
            {
                try
                {
                    return TIUtilities.GetAttributeString(this.attackerAttribute);
                }
                catch
                {
                    return "Persuasion";
                }
            }
        }
    }
}

