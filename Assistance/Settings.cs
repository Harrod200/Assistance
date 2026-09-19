using UnityModManagerNet;
using UnityEngine;
using PavonisInteractive.TerraInvicta;

namespace Assistance
{
    public class Settings : UnityModManager.ModSettings
    {
        public float assistPercentage = 50f;
        public bool enableAssistMission = true;
        public bool debugLogging = false;

        [Header("Stat Bonus Cap")]
        public bool capBonusEnabled = true;
        public int maxBonusStatValue = 25;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            // Keep the cap sane when users type odd values
            maxBonusStatValue = Mathf.Max(1, maxBonusStatValue);
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }


        /// <summary>
        /// Clamps a bonus amount so that advisee base stat + total recorded bonus
        /// for that stat never exceeds maxBonusStatValue. Returns 0 when the cap
        /// is disabled or already reached.
        /// </summary>
        public int ApplyCap(TICouncilorState advisee, CouncilorAttribute stat, int desiredBonus)
        {
            if (desiredBonus <= 0) return 0;
            if (!capBonusEnabled) return desiredBonus;

            int baseStat = advisee != null
                ? advisee.GetAttribute(stat, true, true, true, false, false, false)
                : 0;

            int room = maxBonusStatValue - baseStat - AssistBonusTracker.GetStatBonus(advisee, stat);
            return Mathf.Max(0, Mathf.Min(desiredBonus, room));
        }

    }
}
