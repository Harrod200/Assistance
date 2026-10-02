using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using FullSerializer;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityModManagerNet;

namespace Assistance
{
    /// <summary>
    /// UMM settings-panel button that cleanly removes the mod's data from the
    /// savegame referenced by the main menu's Continue button, so the mod can be
    /// uninstalled without corrupting that save. Works straight from the main
    /// menu - no campaign load required.
    ///
    /// Design (verified against the decompiled game code):
    /// - The Continue button targets StartMenuController.continueSaveFilepath
    ///   (== TIUtilities.GetMostRecentSave()); LoadMenuController only enables
    ///   the button when that file exists, and StartMenuController hands the same
    ///   path to SolarSystemBootstrap.LoadGame.
    /// - A save file is a SaveStructure (currentID + gamestates dictionary),
    ///   pretty-printed JSON, gzipped when TIPlayerProfileManager.compressSaves
    ///   is on (extension .gz, else .json). SaveStructure.Load deserializes it
    ///   standalone; TIGameStateConverter resolves nested gamestate references
    ///   by ID within the file, so councilor -> mission links come back intact
    ///   even with no campaign running.
    /// - Serialization is depth-based: top-level gamestates write fully, nested
    ///   ones as IDs. Mutating the deserialized dictionary and re-serializing
    ///   with StringSerializationAPI reproduces the game's own save format.
    /// - The mod's only persistent data is assist TIMissionStates. They are
    ///   identified by their serialized templateName ("Assist"), removed from
    ///   the gamestates dictionary, and every TIGameState field that referenced
    ///   one (councilor activeMission and any others) is nulled so no dangling
    ///   ID remains in the file. Templates never persist - no cleanup needed.
    /// - A backup of the original file is written first as *.bak, which the
    ///   save list and GetMostRecentSave ignore (they filter by extension).
    /// If a campaign happens to be loaded, the live session is cleaned too -
    /// otherwise its next save would resurrect the missions into the file.
    /// </summary>
    public static class ModCleanupButton
    {
        private static string _status;
        private static bool _statusIsError;

        // Dropdown state: selected save file, cached listing, popup open flag.
        private static List<FileInfo> _saves;
        private static int _selected = -1;
        private static bool _dropdownOpen;
        private static Vector2 _dropdownScroll;

        private static readonly FieldInfo GamestatesField = typeof(GameStateManager).GetField(
            "gamestates", BindingFlags.NonPublic | BindingFlags.Static);

        private static void RefreshSaveList()
        {
            try
            {
                string dir = CreateSaveFileScrollList.GetSaveFolderPath();
                var files = new DirectoryInfo(dir).GetFiles();
                // Same filter the game uses (TIUtilities.GetMostRecentSave):
                // keep only the active save extension(s), newest first.
                string ext = TIUtilities.GetSaveFileExtension();
                _saves = new List<FileInfo>(files.Where(f => ext.Contains(f.Extension)));
                _saves.Sort((a, b) => b.LastWriteTime.CompareTo(a.LastWriteTime));
            }
            catch (Exception e)
            {
                _saves = null;
                UnityEngine.Debug.LogError("[Assistance] Failed to enumerate savegames: " + e.Message);
            }
            if (_saves != null && _selected >= _saves.Count) _selected = _saves.Count - 1;
            if (_saves != null && _selected < 0 && _saves.Count > 0) _selected = 0;
        }

        public static void DrawGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Space(12f);
            GUILayout.Label("Mod removal:", new GUILayoutOption[0]);

            RefreshSaveList();
            if (_saves == null || _saves.Count == 0)
            {
                GUILayout.Label("No savegames found.", new GUILayoutOption[0]);
                return;
            }

            // Dropdown (IMGUI approximation): header button toggles a scrollable list.
            var sel = _saves[Mathf.Clamp(_selected, 0, _saves.Count - 1)];
            string header = string.Format("Savegame: {0}  ({1:yyyy-MM-dd HH:mm}) {2}",
                Path.GetFileNameWithoutExtension(sel.Name), sel.LastWriteTime, _dropdownOpen ? "▲" : "▼");
            if (GUILayout.Button(header, new GUILayoutOption[0]))
                _dropdownOpen = !_dropdownOpen;

            if (_dropdownOpen)
            {
                _dropdownScroll = GUILayout.BeginScrollView(_dropdownScroll, GUI.skin.box, GUILayout.Height(Mathf.Min(160f, 22f * _saves.Count + 8f)));
                for (int i = 0; i < _saves.Count; i++)
                {
                    var f = _saves[i];
                    string label = string.Format("{0}{1}  {2:yyyy-MM-dd HH:mm}  ({3:0.0} KB)",
                        i == _selected ? "● " : "   ", Path.GetFileNameWithoutExtension(f.Name), f.LastWriteTime, f.Length / 1024f);
                    var btnStyle = new GUIStyle(GUI.skin.button);
                    if (GUILayout.Button(label, btnStyle, new GUILayoutOption[0]))
                    {
                        _selected = i;
                        _dropdownOpen = false;
                        _status = null;
                    }
                }
                GUILayout.EndScrollView();
            }

            if (GUILayout.Button("Remove mod data from the selected savegame", new GUILayoutOption[0]))
            {
                _status = CleanSaveFile(sel.FullName, modEntry);
                _statusIsError = _status != null && _status.StartsWith("!");
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
        /// Cleans the given save file: removes assist missions and all references
        /// to them, then rewrites the file (a .bak backup is kept alongside).
        /// Returns a readable result.
        /// </summary>
        public static string CleanContinueSave(UnityModManager.ModEntry modEntry)
        {
            return CleanSaveFile(StartMenuController.continueSaveFilepath, modEntry);
        }

        public static string CleanSaveFile(string path, UnityModManager.ModEntry modEntry)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return "!No savegame found for the Continue button. Load and save a campaign first.";

                string backup = path + ".pre-assist-clean.bak";
                File.Copy(path, backup, true);

                SaveStructure save = SaveStructure.Load(path);
                if (save == null || save.gamestates == null)
                    return "!Could not parse '{0}' - see the player log.".Replace("{0}", Path.GetFileName(path));

                var removed = new HashSet<GameStateID>();
                Dictionary<GameStateID, TIGameState> missionDict;
                if (!save.gamestates.TryGetValue(typeof(TIMissionState), out missionDict) || missionDict == null)
                    return "No assist missions were active; mod data was clean. ('{0}' left unchanged.)"
                        .Replace("{0}", Path.GetFileName(path));

                foreach (var pair in missionDict)
                {
                    var mission = pair.Value as TIMissionState;
                    if (mission != null && mission.templateName == "Assist")
                        removed.Add(mission.ID);
                }

                if (removed.Count == 0)
                {
                    return "No assist missions were active; mod data was clean. ('{0}' left unchanged.)"
                        .Replace("{0}", Path.GetFileName(path));
                }

                // Null every TIMissionState-typed field that references a removed
                // mission - activeMission (private-set auto property) and any
                // others, across all gamestate types.
                int cleared = ClearRemovedMissionReferences(save.gamestates, removed);

                // Drop the mission states themselves from the save.
                foreach (GameStateID id in removed)
                    missionDict.Remove(id);

                if (!WriteSave(path, save))
                    return "!Cleaned {0} assist mission(s) and cleared {1} reference(s), but writing the file failed - " +
                        "the original is intact at '{2}'. See the player log."
                        .Replace("{0}", removed.Count.ToString()).Replace("{1}", cleared.ToString())
                        .Replace("{2}", Path.GetFileName(backup));

                // If a session is live, clean it too so its next save can't
                // resurrect the removed missions into the file.
                int liveMissions = 0;
                var gamestates = (Dictionary<Type, Dictionary<GameStateID, TIGameState>>)GamestatesField.GetValue(null);
                if (gamestates != null && GameStateManager.HasGamestates)
                {
                    var liveDict = gamestates[typeof(TIMissionState)];
                    if (liveDict != null)
                        foreach (GameStateID id in removed)
                            if (liveDict.Remove(id))
                                liveMissions++;
                    ClearRemovedMissionReferences(gamestates, removed);
                    AssistBonusTracker.ClearAll();
                }

                modEntry.Logger.Log(string.Format(
                    "[ModCleanup] Cleaned '{0}': removed {1} assist mission(s), cleared {2} reference(s){3}. Backup: '{4}'.",
                    path, removed.Count, cleared, liveMissions > 0 ? " (+ " + liveMissions + " in the live session)" : "", backup));

                return string.Format("Removed {0} assist mission(s) and cleared {1} reference(s) from '{2}' " +
                    "(live session too: {3}). Backup kept at '{4}'. The save now loads cleanly without the mod.",
                    removed.Count, cleared, Path.GetFileName(path),
                    liveMissions > 0 ? "yes, " + liveMissions + " mission(s)" : "not loaded",
                    Path.GetFileName(backup));
            }
            catch (Exception ex)
            {
                modEntry.Logger.Error("[ModCleanup] " + ex);
                return "!Cleanup failed: " + ex.Message + " - see the player log.";
            }
        }

        /// <summary>Nulls every instance field of type TIMissionState whose value
        /// is one of the removed missions, on every gamestate in the registry.
        /// Covers private-set auto-properties via their backing fields.</summary>
        private static int ClearRemovedMissionReferences(
            Dictionary<Type, Dictionary<GameStateID, TIGameState>> gamestates, HashSet<GameStateID> removed)
        {
            int cleared = 0;
            var missionFields = new Dictionary<Type, List<FieldInfo>>();
            foreach (var dict in gamestates.Values)
            {
                if (dict == null) continue;
                foreach (TIGameState state in dict.Values)
                {
                    if (state == null || state is TIMissionState) continue;
                    List<FieldInfo> fields;
                    var type = state.GetType();
                    if (!missionFields.TryGetValue(type, out fields))
                    {
                        fields = new List<FieldInfo>();
                        for (var t = type; t != null && t != typeof(object); t = t.BaseType)
                            foreach (FieldInfo f in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                                if (typeof(TIMissionState).IsAssignableFrom(f.FieldType))
                                    fields.Add(f);
                        missionFields[type] = fields;
                    }
                    foreach (FieldInfo f in fields)
                    {
                        var mission = f.GetValue(state) as TIMissionState;
                        if (mission != null && removed.Contains(mission.ID))
                        {
                            f.SetValue(state, null);
                            cleared++;
                        }
                    }
                }
            }
            return cleared;
        }

        /// <summary>Serializes the SaveStructure back to disk in the same format
        /// the game writes: pretty JSON, gzipped iff the file path ends in .gz.</summary>
        private static bool WriteSave(string path, SaveStructure save)
        {
            string json = fsJsonPrinter.PrettyJson(StringSerializationAPI.Serialize(typeof(SaveStructure), save));
            try
            {
                if (path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
                {
                    using (var file = File.Create(path))
                    using (var gzip = new GZipStream(file, CompressionMode.Compress))
                    using (var writer = new StreamWriter(gzip))
                        writer.Write(json);
                }
                else
                {
                    File.WriteAllText(path, json);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("[ModCleanup] write failed: " + ex);
                return false;
            }
        }
    }
}
