using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Tracks assist bonuses that are applied only during contested mission resolution.
    /// Bonuses are NOT added to base attributes; they are applied as temporary modifiers
    /// only when calculating contested mission success chances.
    /// </summary>
    public static class AssistBonusTracker
    {
        private static Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>> trackedBonuses = 
            new Dictionary<TICouncilorState, Dictionary<CouncilorAttribute, int>>();

        // Track total assist bonus amount per councilor (for logging and contested checks)
        private static Dictionary<TICouncilorState, int> totalBonusAmounts = 
            new Dictionary<TICouncilorState, int>();

        /// <summary>
        /// Records an assist bonus for a councilor (tracked but not yet applied to attributes).
        /// Bonuses will be applied when the councilor faces contested missions.
        /// </summary>
        public static void RecordBonus(TICouncilorState councilor, CouncilorAttribute stat, int amount)
        {
            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] RecordBonus called - Councilor: {0}, Stat: {1}, Amount: {2}", 
                    councilor != null ? councilor.displayName : "NULL", stat, amount));

            if (councilor == null || amount <= 0)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] RecordBonus rejected - councilor null: {0}, amount <= 0: {1}", 
                        councilor == null, amount <= 0));
                return;
            }

            if (!trackedBonuses.ContainsKey(councilor))
            {
                trackedBonuses[councilor] = new Dictionary<CouncilorAttribute, int>();
            }

            if (!trackedBonuses[councilor].ContainsKey(stat))
            {
                trackedBonuses[councilor][stat] = 0;
            }

            trackedBonuses[councilor][stat] += amount;

            // Track total bonus amount for contested mission checks
            if (!totalBonusAmounts.ContainsKey(councilor))
            {
                totalBonusAmounts[councilor] = 0;
            }
            totalBonusAmounts[councilor] += amount;

            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Recorded bonus for '{0}': {1} +{2}, Total={3}", 
                    councilor.displayName, stat.ToString(), amount, totalBonusAmounts[councilor]));
            }
        }

        /// <summary>
        /// Gets the assist bonus for a specific stat (used during contested mission checks).
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
        /// Gets the total bonus pool for a councilor (sum of all stat bonuses).
        /// Used by contested mission patches to apply bonuses during checks.
        /// </summary>
        public static int GetTotalBonus(TICouncilorState councilor)
        {
            if (councilor == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[AssistBonusTracker] GetTotalBonus called with NULL councilor!");
                return 0;
            }

            if (!totalBonusAmounts.ContainsKey(councilor))
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] GetTotalBonus: No bonuses tracked for '{0}'", councilor.displayName));
                return 0;
            }

            int bonus = totalBonusAmounts[councilor];
            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] GetTotalBonus for '{0}': {1} points", councilor.displayName, bonus));

            return bonus;
        }

        /// <summary>
        /// Clears all tracked bonuses for a councilor when their mission completes.
        /// Note: Bonuses were never applied to attributes, so no reversal is needed.
        /// </summary>
        public static void RemoveBonuses(TICouncilorState councilor)
        {
            if (councilor == null)
                return;

            if (trackedBonuses.ContainsKey(councilor))
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    int totalBonus = GetTotalBonus(councilor);
                    Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Clearing {0} total bonus points for '{1}'", 
                        totalBonus, councilor.displayName));
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
        /// Clears all tracked bonuses (for mod reload/unload).
        /// </summary>
        public static void ClearAll()
        {
            trackedBonuses.Clear();
            totalBonusAmounts.Clear();
        }
    }
}
