using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    /// <summary>
    /// Caches the attacking and defending modifiers that were used during mission calculation.
    /// This allows the breakdown display to show the EXACT modifiers that contributed to the
    /// mission outcome, not the modifiers list after rewards have been added.
    /// 
    /// The cache is populated during SumAttackingModifiers and SumDefendingModifiers patches,
    /// which are called DURING mission resolution before LogMissionOutcome.
    /// The breakdown can then retrieve this cached data to display accurate modifier lists.
    /// </summary>
    public static class MissionCalculationCache
    {
        /// <summary>
        /// Key format: "MissionTemplateName:AttackerName:TargetName"
        /// This uniquely identifies a mission calculation.
        /// </summary>
        private static Dictionary<string, MissionModifierSnapshot> calculationCache = 
            new Dictionary<string, MissionModifierSnapshot>();

        /// <summary>
        /// Snapshot of modifiers used during a mission calculation.
        /// </summary>
        private class MissionModifierSnapshot
        {
            public List<TIMissionModifier> AttackingModifiers { get; set; }
            public List<TIMissionModifier> DefendingModifiers { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        /// <summary>
        /// Creates a unique cache key for a mission calculation.
        /// </summary>
        private static string GetCacheKey(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
        {
            if (mission == null || councilor == null || target == null)
                return null;

            return string.Format("{0}:{1}:{2}",
                mission.friendlyName ?? "Unknown",
                councilor.displayName ?? "Unknown",
                target.displayName ?? "Unknown");
        }

        /// <summary>
        /// Caches the attacking modifiers used during mission calculation.
        /// Called from SumAttackingModifiers_Postfix during mission resolution.
        /// Only stores the first version or versions with fewer modifiers to avoid
        /// capturing modifiers added after state changes (e.g., target being detained).
        /// </summary>
        public static void CacheAttackingModifiers(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            List<TIMissionModifier> modifiers)
        {
            if (mission == null || councilor == null || target == null || modifiers == null)
                return;

            string key = GetCacheKey(mission, councilor, target);
            if (string.IsNullOrEmpty(key))
                return;

            if (!calculationCache.ContainsKey(key))
            {
                calculationCache[key] = new MissionModifierSnapshot { CreatedAt = DateTime.UtcNow };
                // First cache entry - store these modifiers
                calculationCache[key].AttackingModifiers = new List<TIMissionModifier>(modifiers ?? new List<TIMissionModifier>());

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionCalculationCache] Cached {0} attacking modifiers for mission '{1}' (attacker: {2}, target: {3})",
                        modifiers.Count, mission.friendlyName, councilor.displayName, target.displayName));
                    foreach (var mod in modifiers)
                    {
                        Main.mod.Logger.Log(string.Format("  - {0}", mod.displayName));
                    }
                }
            }
            else if (calculationCache[key].AttackingModifiers != null && 
                     modifiers.Count < calculationCache[key].AttackingModifiers.Count)
            {
                // Only update if new list has fewer modifiers (likely the original before state changes)
                int oldCount = calculationCache[key].AttackingModifiers.Count;
                calculationCache[key].AttackingModifiers = new List<TIMissionModifier>(modifiers);

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionCalculationCache] Updated attacking modifiers for mission '{0}' ({1} -> {2} modifiers)",
                        mission.friendlyName, 
                        oldCount,
                        modifiers.Count));
                }
            }
            else if (Main.mod != null && Main.settings.debugLogging && 
                     calculationCache[key].AttackingModifiers != null)
            {
                Main.mod.Logger.Log(string.Format(
                    "[MissionCalculationCache] Skipping redundant cache update for mission '{0}' (existing: {1}, new: {2} modifiers)",
                    mission.friendlyName,
                    calculationCache[key].AttackingModifiers.Count,
                    modifiers.Count));
            }
        }

        /// <summary>
        /// Caches the defending modifiers used during mission calculation.
        /// Called from SumDefendingModifiers_Postfix during mission resolution.
        /// Only stores the first version or versions with fewer modifiers to avoid
        /// capturing modifiers added after state changes (e.g., target being detained).
        /// </summary>
        public static void CacheDefendingModifiers(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target,
            List<TIMissionModifier> modifiers)
        {
            if (mission == null || councilor == null || target == null || modifiers == null)
                return;

            string key = GetCacheKey(mission, councilor, target);
            if (string.IsNullOrEmpty(key))
                return;

            if (!calculationCache.ContainsKey(key))
            {
                calculationCache[key] = new MissionModifierSnapshot { CreatedAt = DateTime.UtcNow };
                // First cache entry - store these modifiers
                calculationCache[key].DefendingModifiers = new List<TIMissionModifier>(modifiers ?? new List<TIMissionModifier>());

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionCalculationCache] Cached {0} defending modifiers for mission '{1}' (attacker: {2}, target: {3})",
                        modifiers.Count, mission.friendlyName, councilor.displayName, target.displayName));
                    foreach (var mod in modifiers)
                    {
                        Main.mod.Logger.Log(string.Format("  - {0}", mod.displayName));
                    }
                }
            }
            else if (calculationCache[key].DefendingModifiers != null && 
                     modifiers.Count < calculationCache[key].DefendingModifiers.Count)
            {
                // Only update if new list has fewer modifiers (likely the original before state changes)
                int oldCount = calculationCache[key].DefendingModifiers.Count;
                calculationCache[key].DefendingModifiers = new List<TIMissionModifier>(modifiers);

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionCalculationCache] Updated defending modifiers for mission '{0}' ({1} -> {2} modifiers)",
                        mission.friendlyName, 
                        oldCount,
                        modifiers.Count));
                }
            }
            else if (Main.mod != null && Main.settings.debugLogging && 
                     calculationCache[key].DefendingModifiers != null)
            {
                Main.mod.Logger.Log(string.Format(
                    "[MissionCalculationCache] Skipping redundant cache update for mission '{0}' (existing: {1}, new: {2} modifiers)",
                    mission.friendlyName,
                    calculationCache[key].DefendingModifiers.Count,
                    modifiers.Count));
            }
        }

        /// <summary>
        /// Retrieves the cached attacking modifiers for a mission.
        /// </summary>
        public static List<TIMissionModifier> GetCachedAttackingModifiers(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target)
        {
            string key = GetCacheKey(mission, councilor, target);
            if (string.IsNullOrEmpty(key) || !calculationCache.ContainsKey(key))
                return null;

            var snapshot = calculationCache[key];
            return snapshot?.AttackingModifiers;
        }

        /// <summary>
        /// Retrieves the cached defending modifiers for a mission.
        /// </summary>
        public static List<TIMissionModifier> GetCachedDefendingModifiers(
            TIMissionTemplate mission,
            TICouncilorState councilor,
            TIGameState target)
        {
            string key = GetCacheKey(mission, councilor, target);
            if (string.IsNullOrEmpty(key))
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionCalculationCache] GetCachedDefendingModifiers: NULL or empty key"));
                }
                return null;
            }

            if (!calculationCache.ContainsKey(key))
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[MissionCalculationCache] GetCachedDefendingModifiers: Cache miss for key '{0}'", key));
                }
                return null;
            }

            var snapshot = calculationCache[key];
            if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[MissionCalculationCache] GetCachedDefendingModifiers: Found snapshot, DefendingModifiers={0}",
                    snapshot?.DefendingModifiers?.Count ?? -1));
            }
            return snapshot?.DefendingModifiers;
        }

        /// <summary>
        /// Clears old cache entries (older than 1 minute).
        /// Called periodically to prevent memory buildup.
        /// </summary>
        public static void CleanupOldEntries()
        {
            DateTime cutoff = DateTime.UtcNow.AddMinutes(-1);
            var keysToRemove = new List<string>();

            foreach (var kvp in calculationCache)
            {
                if (kvp.Value.CreatedAt < cutoff)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                calculationCache.Remove(key);

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format("[MissionCalculationCache] Cleaned up cached entry: {0}", key));
                }
            }
        }

        /// <summary>
        /// Clears all cached entries.
        /// </summary>
        public static void ClearCache()
        {
            calculationCache.Clear();
        }
    }
}
