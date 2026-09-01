using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace Assistance
{
    public static class Main
    {
        public static UnityModManager.ModEntry mod;
        public static Settings settings;
        public static bool enabled = true;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Main.mod = modEntry;
            Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            modEntry.OnToggle = new System.Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
            modEntry.OnGUI = new System.Action<UnityModManager.ModEntry>(Main.OnGUI);
            modEntry.OnSaveGUI = new System.Action<UnityModManager.ModEntry>(Main.OnSaveGUI);

            try
            {
                Harmony harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
                modEntry.Logger.Log("Councilor Assist Mission patches applied.");
                return true;
            }
            catch (Exception ex)
            {
                modEntry.Logger.Error("Failed to apply patches: " + ex);
                return false;
            }
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Main.enabled = value;
            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("Councilor Assist Settings:", new GUILayoutOption[0]);
            Main.settings.assistPercentage = Main.DrawNamedFloat("Assist Percentage (0-100%)", Main.settings.assistPercentage, 200f);
            Main.settings.assistPercentage = Mathf.Clamp(Main.settings.assistPercentage, 0f, 100f);

            GUILayout.Space(8f);
            Main.settings.enableAssistMission = GUILayout.Toggle(Main.settings.enableAssistMission, "Enable Assist Mission for councilors", new GUILayoutOption[0]);

            GUILayout.Space(8f);
            Main.settings.debugLogging = GUILayout.Toggle(Main.settings.debugLogging, "Enable Debug Logging", new GUILayoutOption[0]);
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Main.settings.Save(modEntry);
        }

        private static float DrawNamedFloat(string label, float value, float labelWidth)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, new GUILayoutOption[] { GUILayout.Width(labelWidth) });
            float num = Main.DrawFloat(value, 90f);
            GUILayout.EndHorizontal();
            return num;
        }

        private static float DrawFloat(float value, float width)
        {
            string text = GUILayout.TextField(value.ToString("F1"), new GUILayoutOption[] { GUILayout.Width(width) });
            float num;
            if (float.TryParse(text, out num))
            {
                return num;
            }
            return value;
        }
    }
}
