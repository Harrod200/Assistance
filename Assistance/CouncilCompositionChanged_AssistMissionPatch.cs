using System;
using PavonisInteractive.TerraInvicta;
using UnityModManagerNet;

namespace Assistance
{
    /// <summary>
    /// Handles the CouncilCompositionChanged event to grant the Assist mission to newly recruited councilors.
    /// 
    /// This event fires when a councilor joins or leaves a faction. We use it to grant the Assist mission
    /// to councilors joining the player faction, replacing the previous bootstrap-based approach.
    /// 
    /// WHY THIS APPROACH:
    /// - More efficient: Only grants missions when needed (on recruitment), not to all councilor types globally
    /// - Event-driven: Naturally integrates with the game's own councilor recruitment flow
    /// - Player-faction aware: Can restrict to player-controlled factions without AI complications
    /// - Handles runtime councilor creation: Works for any councilors that join after mod load
    /// </summary>
    internal static class CouncilCompositionChanged_AssistMissionPatch
    {
        private static bool initialized = false;

        /// <summary>
        /// Initialize the event listener. Called from Main.Load() after patches are applied.
        /// </summary>
        public static void Initialize()
        {
            if (initialized)
                return;

            try
            {
                GameControl.eventManager.AddListener<CouncilCompositionChanged>(OnCouncilCompositionChanged, null, null, true, false);
                initialized = true;

                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log("[CouncilCompositionChanged] Event listener initialized successfully.");
                }
            }
            catch (Exception ex)
            {
                if (Main.mod != null)
                {
                    Main.mod.Logger.Error("[CouncilCompositionChanged] Failed to initialize event listener: " + ex);
                }
            }
        }

        /// <summary>
        /// Handles CouncilCompositionChanged events.
        /// Grants the Assist mission to councilors joining the player faction.
        /// </summary>
        private static void OnCouncilCompositionChanged(CouncilCompositionChanged evt)
        {
            if (!Main.enabled || Main.settings == null || !Main.settings.enableAssistMission)
                return;

            if (evt == null)
                return;

            // Only process joining events (not leaving)
            if (!evt.joining)
                return;

            // Only grant to player-controlled factions
            if (evt.council == null || evt.council.player == null || evt.council.player.isAI)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[CouncilCompositionChanged] Skipping Assist mission grant - faction not player-controlled (faction: {0})",
                        evt.council != null ? evt.council.displayName : "NULL"));
                }
                return;
            }

            // Validate councilor
            if (evt.councilor == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[CouncilCompositionChanged] Skipping - councilor is NULL");
                return;
            }

            // Get the Assist mission template
            TIMissionTemplate assistMission = TemplateManager.Find<TIMissionTemplate>("Assist", false);
            if (assistMission == null)
            {
                if (Main.mod != null && Main.settings.debugLogging)
                    Main.mod.Logger.Log("[CouncilCompositionChanged] Assist mission template not found in TemplateManager");
                return;
            }

            // Grant the mission to the councilor
            if (evt.councilor.LearnMission(assistMission))
            {
                if (Main.mod != null && Main.settings.debugLogging)
                {
                    Main.mod.Logger.Log(string.Format(
                        "[CouncilCompositionChanged] Granted Assist mission to newly recruited councilor: {0}",
                        evt.councilor.displayName));
                }
            }
            else if (Main.mod != null && Main.settings.debugLogging)
            {
                Main.mod.Logger.Log(string.Format(
                    "[CouncilCompositionChanged] Assist mission already learned by councilor: {0}",
                    evt.councilor.displayName));
            }
        }
    }
}

