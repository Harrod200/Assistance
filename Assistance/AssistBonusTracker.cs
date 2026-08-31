using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Tracks assist bonuses granted to councilors so they can be removed when missions complete.
    /// </summary>
    public static class AssistBonusTracker
    {
        private static Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>> trackedBonuses = 
            new Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>>();

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
        }

        /// <summary>
        /// Clears all tracked bonuses (for mod reload/unload)
        /// </summary>
        public static void ClearAll()
        {
            trackedBonuses.Clear();
        }
    }
}
