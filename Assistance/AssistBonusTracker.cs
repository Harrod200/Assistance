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
        }

        /// <summary>
        /// Gets the total assist bonus amount for a councilor (used for control point cap exclusion)
        /// </summary>
        public static int GetCouncilorBonusAmount(TICouncilorState councilor)
        {
            if (councilor == null || !totalBonusAmounts.ContainsKey(councilor))
                return 0;

            return totalBonusAmounts[councilor];
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
        /// Clears all tracked bonuses (for mod reload/unload)
        /// </summary>
        public static void ClearAll()
        {
            trackedBonuses.Clear();
            totalBonusAmounts.Clear();
        }
    }
}
