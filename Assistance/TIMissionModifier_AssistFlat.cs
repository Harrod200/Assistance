using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    public class TIMissionModifier_AssistFlat : TIMissionModifier
    {
        public float flatModifier = 0f;

        public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
        {
            return this.flatModifier;
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

                    // If localization returned the key itself (not found), return the fallback
                    if (!string.IsNullOrEmpty(locName) && locName != locKey)
                    {
                        return locName;
                    }

                    // Fallback: return a simple name
                    return "Flat Bonus";
                }
                catch
                {
                    // Ultimate fallback
                    return "Bonus";
                }
            }
        }
    }
}
