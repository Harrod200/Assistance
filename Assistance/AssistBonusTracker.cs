using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Tracks assist bonuses granted to councilors so they can be removed when missions complete.
    /// Also tracks total bonus amounts for control point cap exclusion calculation.
    /// </summary>
    public static class AssistBonusTracker
    {
        private static Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>> trackedBonuses = 
            new Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>>();

        // Track total assist bonus amount per councilor for control point cap calculation
        private static Dictionary<TICouncilorState, int> totalBonusAmounts = 
            new Dictionary<TICouncilorState, int>();

        /// <summary>
        /// Records an assist bonus for a councilor
        /// </summary>
        public static void RecordBonus(TICouncilorState councilor, CouncilorAttribute stat, int amount)
        {
            if (councilor == null || amount <= 0)
                return;

            if (!trackedBonuses.ContainsKey(councilor))
            {
                trackedBonuses[councilor] = new Dictionary<CouncilorAttribute, int>();
            }

            if (!trackedBonuses[councilor].ContainsKey(stat))
            {
                trackedBonuses[councilor][stat] = 0;
            }

            trackedBonuses[councilor][stat] += amount;

            // Track total bonus amount for control point cap calculation
            if (!totalBonusAmounts.ContainsKey(councilor))
            {
                totalBonusAmounts[councilor] = 0;
            }
            totalBonusAmounts[councilor] += amount;

            if (Main.mod != null)
            {
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Recorded bonus for '{0}': {1} +{2}, Total={3}", 
                    councilor.displayName, stat.ToString(), amount, totalBonusAmounts[councilor]));
            }
        }

        /// <summary>
        /// Gets the assist bonus for a specific stat (used for control point cap exclusion)
        /// </summary>
        public static int GetStatBonus(TICouncilorState councilor, CouncilorAttribute stat)
        {
            if (councilor == null || !trackedBonuses.ContainsKey(councilor))
                return 0;

            if (!trackedBonuses[councilor].ContainsKey(stat))
                return 0;

            return trackedBonuses[councilor][stat];
        }

        /// <summary>
        /// Removes all tracked bonuses for a councilor
        /// </summary>
        public static void RemoveBonuses(TICouncilorState councilor)
        {
            if (councilor == null)
                return;

            if (trackedBonuses.ContainsKey(councilor))
            {
                Dictionary<CouncilorAttribute, int> bonuses = trackedBonuses[councilor];

                foreach (KeyValuePair<CouncilorAttribute, int> bonus in bonuses)
                {
                    // Restore the bonus (negative value removes it)
                    councilor.ModifyAttribute(bonus.Key, -bonus.Value);
                }

                trackedBonuses.Remove(councilor);
            }

            // Clear total bonus amount tracking
            if (totalBonusAmounts.ContainsKey(councilor))
            {
                totalBonusAmounts.Remove(councilor);
            }
        }

        /// <summary>
        /// Gets the total CP adjustment for a faction (sum of all CP-affecting bonuses for all councilors in that faction)
        /// Used by faction-level GetControlPointMaintenanceFreebieCap patch to apply flat adjustment
        /// </summary>
        public static int GetFactionCPAdjustment(TIFactionState faction)
        {
            if (faction == null)
                return 0;

            int totalAdjustment = 0;

            // Sum all CP-affecting bonuses for councilors in this faction
            foreach (var kvp in totalBonusAmounts)
            {
                TICouncilorState councilor = kvp.Key;
                int totalBonus = kvp.Value;

                // Only count bonuses for councilors in this faction
                if (councilor != null && councilor.faction == faction)
                {
                    // Calculate CP impact: only Persuasion, Command, Administration affect CP
                    int persuasionBonus = GetStatBonus(councilor, CouncilorAttribute.Persuasion);
                    int commandBonus = GetStatBonus(councilor, CouncilorAttribute.Command);
                    int administrationBonus = GetStatBonus(councilor, CouncilorAttribute.Administration);

                    int cpBonus = persuasionBonus + commandBonus + administrationBonus;
                    totalAdjustment += cpBonus;
                }
            }

            return totalAdjustment;
        }

        /// <summary>
        /// Clears all tracked bonuses (for mod reload/unload)
        /// </summary>
        public static void ClearAll()
        {
            trackedBonuses.Clear();
            totalBonusAmounts.Clear();
        }
    }
}
