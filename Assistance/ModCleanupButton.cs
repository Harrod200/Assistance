using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityModManagerNet;

namespace Assistance
{
    /// <summary>
    /// UMM settings-panel button that cleanly removes the mod's data from the
    /// latest savegame, so the mod can be uninstalled without corrupting the save.
    ///
    /// Design (verified against the decompiled game code):
    /// - A save serializes GameStateManager.gamestates only (SaveStructure.Load /
    ///   GameStateManager.LoadAllGameStates). Templates live in TemplateManager's
    ///   in-memory registry, re-initialized from game data on every session start,
    ///   so templates never persist in a save and need no cleanup.
    /// - The only gamestates the mod creates are assist TIMissionStates (via
    ///   AssistAvailabilityPatch / TIMissionTemplate_Assist effects). Every assist
    ///   mission is removed from the registry with
    ///   GameStateManager.RemoveGameState&lt;TIMissionState&gt;(id, true).
    /// - TICouncilorState.activeMission holds a reference to the removed mission
    ///   state, so it is nulled too (private setter, set via Harmony Traverse) -
    ///   the game's own null checks then behave as if the councilor had no mission.
    /// - AssistBonusTracker (in-memory, runtime-only) is cleared.
    /// - The cleaned session is written back over the most recent save file
    ///   (StartMenuController.continueSaveFilepath == TIUtilities.GetMostRecentSave()).
    /// After this, saving the session - or reloading the cleaned latest save -
    /// yields a save the base game can load with the mod removed.
    /// </summary>
    public static class ModCleanupButton
    {
        private static string _status;
        private static bool _statusIsError;

        private static readonly FieldInfo GamestatesField = typeof(GameStateManager).GetField(
            "gamestates", BindingFlags.NonPublic | BindingFlags.Static);

        public static void DrawGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Space(12f);
            GUILayout.Label("Mod removal:", new GUILayoutOption[0]);

            bool sessionLoaded = GameStateManager.HasGamestates;
            if (sessionLoaded)
            {
                if (GUILayout.Button("Remove mod data from latest save", new GUILayoutOption[0]))
                {
                    _status = CleanSessionAndSave(modEntry);
                    _statusIsError = false;
                }
            }
            else
            {
                if (GUILayout.Button("Remove mod data from latest save", new GUILayoutOption[0]))
                {
                    _status = "No campaign is loaded. Load the save you want to clean, then click the button again.";
                    _statusIsError = true;
                }
            }

            if (!string.IsNullOrEmpty(_status))
            {
                var style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = _statusIsError ? Color.yellow : GUI.skin.label.normal.textColor;
                style.wordWrap = true;
                GUILayout.Label(_status, style, new GUILayoutOption[0]);
            }
        }

        /// <summary>
        /// Removes all assist-mission gamestates from the loaded session, detaches
        /// councilors from them, clears the bonus tracker, and overwrites the most
        /// recent save file with the cleaned state. Returns a human-readable result.
        /// </summary>
        public static string CleanSessionAndSave(UnityModManager.ModEntry modEntry)
        {
            try
            {
                var gamestates = (Dictionary<Type, Dictionary<GameStateID, TIGameState>>)GamestatesField.GetValue(null);
                if (gamestates == null)
                    return "Could not read the gamestate registry - aborting.";

                // Snapshot first: RemoveGameState mutates the dictionaries while we walk them.
                var assistMissions = new List<TIMissionState>();
                foreach (var pair in gamestates)
                {
                    if (!typeof(TIMissionState).IsAssignableFrom(pair.Key))
                        continue;
                    foreach (var state in pair.Value.Values)
                    {
                        if (state != null && state is TIMissionState mission && mission.templateName == "Assist")
                            assistMissions.Add(mission);
                    }
                }

                int missions = 0;
                int councilors = 0;
                foreach (var mission in assistMissions)
                {
                    // Detach the assigned councilor first, so no live reference to the
                    // removed mission remains in any serialized gamestate.
                    var councilor = mission.councilor;
                    if (councilor != null && councilor.activeMission == mission)
                    {
                        Traverse.Create(councilor).Field("activeMission").SetValue(null);
                        councilors++;
                    }
                    GameStateManager.RemoveGameState<TIMissionState>(mission.ID, true);
                    missions++;
                }

                AssistBonusTracker.ClearAll();

                // Overwrite the most recent savegame with the cleaned session.
                string latestSave = TIUtilities.GetMostRecentSave();
                if (string.IsNullOrEmpty(latestSave))
                    return string.Format("Cleaned {0} assist mission(s) and detached {1} councilor(s), " +
                        "but no save file could be located - use the in-game save screen.", missions, councilors);

                if (!GameStateManager.SaveAllGameStates(latestSave, true))
                    return string.Format("Cleaned {0} assist mission(s) and detached {1} councilor(s), " +
                        "but writing '{2}' failed - see the player log.", missions, councilors, latestSave);

                if (Main.settings.debugLogging)
                    modEntry.Logger.Log(string.Format(
                        "[ModCleanup] Removed {0} assist mission(s), detached {1} councilor(s), saved to '{2}'.",
                        missions, councilors, latestSave));

                return missions == 0
                    ? string.Format("No assist missions were active; mod data was clean. Overwrote '{0}'.", latestSave)
                    : string.Format("Removed {0} assist mission(s), detached {1} councilor(s), and overwrote '{2}'. " +
                        "The save now loads cleanly without the mod. (Re-enabled? Just load a different save and reload.)",
                        missions, councilors, latestSave);
            }
            catch (Exception ex)
            {
                modEntry.Logger.Error("[ModCleanup] " + ex);
                return "Cleanup failed: " + ex.Message + " - see the player log.";
            }
        }
    }
}
