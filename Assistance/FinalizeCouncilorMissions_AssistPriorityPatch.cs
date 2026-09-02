using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

namespace Assistance
{
    /// <summary>
    /// Patches FinalizeCouncilorMissions.StaggerMissionResolutions() to ensure Assist missions
    /// resolve before all other missions. This is necessary because negative resolutionOrder
    /// values cause array indexing crashes in the base game.
    /// 
    /// Implementation: Re-sorts the mission list to move all Assist missions to the front
    /// before any other missions are scheduled for resolution.
    /// </summary>
    [HarmonyPatch]
    public class FinalizeCouncilorMissions_AssistPriorityPatch
    {
        [HarmonyPatch(typeof(FinalizeCouncilorMissions), "StaggerMissionResolutions")]
        [HarmonyPrefix]
        public static bool StaggerMissionResolutions_Prefix(FinalizeCouncilorMissions __instance)
        {
            // Replaces the entire method with our version that prioritizes Assist missions
            StaggerMissionResolutions_PrioritizeAssist(__instance);
            return false;  // Skip the original method
        }

        /// <summary>
        /// Modified version of StaggerMissionResolutions that prioritizes Assist missions.
        /// Assist missions are sorted to the front of the list before any other missions.
        /// </summary>
        private static void StaggerMissionResolutions_PrioritizeAssist(FinalizeCouncilorMissions instance)
        {
            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log("[AssistBonusTracker] FinalizeCouncilorMissions.StaggerMissionResolutions_Patch called");

            // Reflection to access private fields and methods
            FieldInfo factionField = typeof(FinalizeCouncilorMissions).GetField("faction", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo hoursInTurnMethod = typeof(FinalizeCouncilorMissions).GetMethod("HoursInTurn", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            TIFactionState faction = (TIFactionState)factionField.GetValue(instance);

            // Collect all missions from all factions
            List<TIMissionState> allMissions = new List<TIMissionState>();
            int resolutionSegmentsPerPhase = GameStateManager.MissionPhase().resolutionSegmentsPerPhase;

            foreach (TIFactionState tifactionState in GameStateManager.AllFactions().ToList<TIFactionState>().Shuffle<TIFactionState>())
            {
                foreach (TICouncilorState ticouncilorState in tifactionState.activeCouncilors)
                {
                    if (ticouncilorState.HasMission)
                    {
                        TIMissionState activeMission = ticouncilorState.activeMission;
                        TIMissionTemplate missionTemplate = activeMission.missionTemplate;
                        List<string> validationErrors = missionTemplate.target.ValidateSingleTarget(missionTemplate, ticouncilorState, activeMission.target);

                        if (missionTemplate.target.ValidTarget(validationErrors))
                        {
                            allMissions.Add(ticouncilorState.activeMission);
                        }
                        else
                        {
                            tifactionState.playerControl.StartAction(new AbortMission(ticouncilorState, false, TIMissionState.AbortReason.TargetInvalid, null, MarkerController.BuildInvalidTargetTooltip(validationErrors)));
                        }
                    }
                }
            }

            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] Total missions collected: {0}", allMissions.Count));

            // PRIORITY SORT: Assist missions first, then by resolutionOrder
            List<TIMissionState> sortedMissions = allMissions
                .OrderBy(m => m.missionTemplate.dataName != "Assist" ? 1 : 0)  // Assist = 0 (first), Others = 1
                .ThenBy(m => m.getResolutionOrder)                             // Then sort by resolution order
                .ToList();

            int assistMissionCount = sortedMissions.Count(m => m.missionTemplate.dataName == "Assist");
            if (Main.mod != null && Main.settings.debugLogging)
                Main.mod.Logger.Log(string.Format("[AssistBonusTracker] After sorting: {0} Assist missions, {1} total missions", assistMissionCount, sortedMissions.Count));

            // Count missions by resolution order segment (with bounds checking)
            int[] missionCountPerSegment = new int[resolutionSegmentsPerPhase];
            foreach (TIMissionState mission in sortedMissions)
            {
                int resOrder = (int)Math.Truncate((double)mission.getResolutionOrder);
                // Clamp to valid array range to prevent crashes
                resOrder = Math.Max(0, Math.Min(resOrder, resolutionSegmentsPerPhase - 1));
                missionCountPerSegment[resOrder]++;
            }

            // Calculate resolution times
            int hoursInTurn = (int)hoursInTurnMethod.Invoke(instance, null);
            float totalHours = (float)hoursInTurn - 12f;
            float hoursPerSegment = totalHours / (float)resolutionSegmentsPerPhase;
            float[] hoursPerMissionInSegment = new float[resolutionSegmentsPerPhase];

            for (int i = 0; i < resolutionSegmentsPerPhase; i++)
            {
                hoursPerMissionInSegment[i] = hoursPerSegment / (float)(missionCountPerSegment[i] + 1);
            }

            // Assign resolution times to each mission
            int currentSegment = 0;
            int missionCountInSegment = 0;

            foreach (TIMissionState mission in sortedMissions)
            {
                TIDateTime resolveTime = TITimeState.Now();
                int missionSegment = (int)Math.Truncate((double)mission.getResolutionOrder);
                // Clamp to valid array range
                missionSegment = Math.Max(0, Math.Min(missionSegment, resolutionSegmentsPerPhase - 1));

                if (missionSegment > currentSegment)
                {
                    missionCountInSegment = 0;
                    currentSegment = missionSegment;
                }

                float offsetHours = 0.25f;
                offsetHours += hoursPerSegment * (float)missionSegment;
                offsetHours += hoursPerMissionInSegment[missionSegment] * (float)(++missionCountInSegment);

                resolveTime.AddHours((double)offsetHours);
                mission.resolveTime = resolveTime;
                mission.startTime = TITimeState.Now();

                if ((double)offsetHours + 0.5 > (double)totalHours)
                {
                    UnityEngine.Debug.LogWarning(string.Format(
                        "{0} {1} Resolve: {2}", 
                        mission.councilor.displayName, 
                        mission.displayName, 
                        mission.resolveTime.ToString()));
                }

                TITimeEvent.CreateNewTimeEvent(resolveTime, mission, null, null, mission.getMissionEventName, true, false, TITimeQueueRepeatType.None, 1, true, false);
                mission.ListenForResolutionTime();
            }

            // Movement logic
            foreach (TIMissionState mission in sortedMissions)
            {
                if (mission.target is TICouncilorState && mission.councilor.location != mission.targetLocation)
                {
                    mission.councilor.CheckAndChaseMissionTarget();
                }
            }

            sortedMissions.Reverse();
            foreach (TIMissionState mission in sortedMissions)
            {
                if (mission.target is TICouncilorState && mission.councilor.location != mission.targetLocation)
                {
                    mission.councilor.CheckAndChaseMissionTarget();
                }
            }
        }
    }
}
