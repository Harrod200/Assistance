using UnityModManagerNet;

namespace Assistance
{
    public class Settings : UnityModManager.ModSettings
    {
        public float assistPercentage = 100f;
        public bool enableAssistMission = true;
        public bool debugLogging = false;
        public bool statCapEnabled = false;
        public int statCapLimit = 25;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }
    }
}
