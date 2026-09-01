using UnityModManagerNet;

namespace Assistance
{
    public class Settings : UnityModManager.ModSettings
    {
        public float assistPercentage = 25f;
        public bool enableAssistMission = true;
        public bool debugLogging = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            UnityModManager.ModSettings.Save<Settings>(this, modEntry);
        }
    }
}
