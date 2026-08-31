using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    public class TIMissionModifier_AssistStat : TIMissionModifier
    {
        public CouncilorAttribute attackerAttribute = CouncilorAttribute.Persuasion;

        public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
        {
            if (attackingCouncilor == null)
                return 0f;

            return (float)attackingCouncilor.GetAttribute(this.attackerAttribute, true, true, true, false, false, false);
        }

        public override string displayName
        {
            get
            {
                try
                {
                    // Try to get localized name first
                    string locKey = new StringBuilder(this.GetType().Name).Append(".displayName").ToString();
                    string locName = Loc.T(locKey);

                    // If localization returned the key itself (not found), return the attribute name instead
                    if (!string.IsNullOrEmpty(locName) && locName != locKey)
                    {
                        return locName;
                    }

                    // Fallback: return the attribute name
                    string name = this.attackerAttribute.ToString();
                    return string.IsNullOrEmpty(name) ? "Stat Bonus" : name;
                }
                catch
                {
                    // Ultimate fallback
                    return "Stat Bonus";
                }
            }
        }
    }
}
