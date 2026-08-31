using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A75 RID: 2677
	public class FinalizeCouncilorMissions : PlayerAction
	{
		// Token: 0x06006539 RID: 25913 RVA: 0x002FBAC9 File Offset: 0x002F9CC9
		public FinalizeCouncilorMissions(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x0600653A RID: 25914 RVA: 0x002FBAD8 File Offset: 0x002F9CD8
		public override void Execute()
		{
			TIMissionPhaseState timissionPhaseState = GameStateManager.MissionPhase();
			if (timissionPhaseState.phaseActive)
			{
				timissionPhaseState.factionsSignallingComplete.Add(this.faction);
			}
			else
			{
				Debug.LogError(this.faction.displayName + " tried to finalize councilor missions outside of mission phase, this should never happen, post a bug report with the previous 2 autosaves");
			}
			TIPromptQueueState.RemovePromptStatic(this.faction, timissionPhaseState, null, "PromptSelectCouncilorMissions", 0);
			GameControl.eventManager.TriggerEvent(new FactionFinalizesMissions(this.faction), null, Array.Empty<object>());
			if (timissionPhaseState.AllFactionsHaveAssignedMissions())
			{
				this.StaggerMissionResolutions();
				TIFactionState[] array = GameStateManager.AllFactions();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].DegradeIntelOnVariousThings();
				}
				foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
				{
					foreach (TICouncilorState ticouncilorState in tifactionState.councilors)
					{
						tifactionState.GainIntel(ticouncilorState, float.Epsilon, null, false);
					}
				}
				timissionPhaseState.SetMissionPhaseInactive();
			}
		}

		// Token: 0x0600653B RID: 25915 RVA: 0x002FBBEC File Offset: 0x002F9DEC
		private int HoursInTurn()
		{
			TITimeEvent titimeEvent = GameStateManager.FindByTemplate<TITimeEvent>("CouncilorMissionUpdate", false);
			TIDateTime tidateTime = TITimeState.Now();
			TIDateTime nextEventTime = titimeEvent.GetNextEventTime(tidateTime);
			return Mathf.Max(24, (int)nextEventTime.DifferenceInHours(tidateTime) - 24);
		}

		// Token: 0x0600653C RID: 25916 RVA: 0x002FBC24 File Offset: 0x002F9E24
		private void StaggerMissionResolutions()
		{
			List<TIMissionState> list = new List<TIMissionState>();
			int resolutionSegmentsPerPhase = GameStateManager.MissionPhase().resolutionSegmentsPerPhase;
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions().ToList<TIFactionState>().Shuffle<TIFactionState>())
			{
				foreach (TICouncilorState ticouncilorState in tifactionState.activeCouncilors)
				{
					if (ticouncilorState.HasMission)
					{
						TIMissionState activeMission = ticouncilorState.activeMission;
						TIMissionTemplate missionTemplate = activeMission.missionTemplate;
						List<string> list2 = missionTemplate.target.ValidateSingleTarget(missionTemplate, ticouncilorState, activeMission.target);
						if (missionTemplate.target.ValidTarget(list2))
						{
							list.Add(ticouncilorState.activeMission);
						}
						else
						{
							tifactionState.playerControl.StartAction(new AbortMission(ticouncilorState, false, TIMissionState.AbortReason.TargetInvalid, null, MarkerController.BuildInvalidTargetTooltip(list2)));
						}
					}
				}
			}
			List<TIMissionState> list3 = list.OrderBy<TIMissionState, float>((TIMissionState o) => o.getResolutionOrder).ToList<TIMissionState>();
			int[] array = new int[resolutionSegmentsPerPhase];
			foreach (TIMissionState timissionState in list3)
			{
				array[(int)Math.Truncate((double)timissionState.getResolutionOrder)]++;
			}
			float num = (float)this.HoursInTurn() - 12f;
			float num2 = num / (float)resolutionSegmentsPerPhase;
			float[] array2 = new float[resolutionSegmentsPerPhase];
			for (int i = 0; i < resolutionSegmentsPerPhase; i++)
			{
				array2[i] = num2 / (float)(array[i] + 1);
			}
			int num3 = 0;
			int num4 = 0;
			foreach (TIMissionState timissionState2 in list3)
			{
				TIDateTime tidateTime = TITimeState.Now();
				int num5 = num3;
				num3 = (int)Math.Truncate((double)timissionState2.getResolutionOrder);
				if (num3 > num5)
				{
					num4 = 0;
				}
				float num6 = 0.25f;
				num6 += num2 * (float)num3;
				num6 += array2[num3] * (float)(++num4);
				tidateTime.AddHours((double)num6);
				timissionState2.resolveTime = tidateTime;
				timissionState2.startTime = TITimeState.Now();
				if ((double)num6 + 0.5 > (double)num)
				{
					Log.Warn(string.Concat(new string[]
					{
						timissionState2.councilor.displayName,
						" ",
						timissionState2.displayName,
						" Resolve: ",
						timissionState2.resolveTime.ToString()
					}), Array.Empty<object>());
				}
				TITimeEvent.CreateNewTimeEvent(tidateTime, timissionState2, null, null, timissionState2.getMissionEventName, true, false, TITimeQueueRepeatType.None, 1, true, false);
				timissionState2.ListenForResolutionTime();
			}
			foreach (TIMissionState timissionState3 in list3)
			{
				if (timissionState3.target is TICouncilorState && timissionState3.councilor.location != timissionState3.targetLocation)
				{
					timissionState3.councilor.CheckAndChaseMissionTarget();
				}
			}
			list3.Reverse();
			foreach (TIMissionState timissionState4 in list3)
			{
				if (timissionState4.target is TICouncilorState && timissionState4.councilor.location != timissionState4.targetLocation)
				{
					timissionState4.councilor.CheckAndChaseMissionTarget();
				}
			}
		}

		// Token: 0x04004768 RID: 18280
		private TIFactionState faction;
	}
}
