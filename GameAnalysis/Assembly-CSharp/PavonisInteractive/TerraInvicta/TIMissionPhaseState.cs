using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000761 RID: 1889
	public class TIMissionPhaseState : TIGameState
	{
		// Token: 0x17000847 RID: 2119
		// (get) Token: 0x060034F1 RID: 13553 RVA: 0x0012E71B File Offset: 0x0012C91B
		// (set) Token: 0x060034F2 RID: 13554 RVA: 0x0012E723 File Offset: 0x0012C923
		public bool phaseActive { get; private set; }

		// Token: 0x17000848 RID: 2120
		// (get) Token: 0x060034F3 RID: 13555 RVA: 0x0012E72C File Offset: 0x0012C92C
		// (set) Token: 0x060034F4 RID: 13556 RVA: 0x0012E733 File Offset: 0x0012C933
		public static float phasesPerMonth { get; private set; }

		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x060034F5 RID: 13557 RVA: 0x0012E73B File Offset: 0x0012C93B
		// (set) Token: 0x060034F6 RID: 13558 RVA: 0x0012E743 File Offset: 0x0012C943
		[fsIgnore]
		public int resolutionSegmentsPerPhase { get; private set; }

		// Token: 0x1700084A RID: 2122
		// (get) Token: 0x060034F7 RID: 13559 RVA: 0x0012E74C File Offset: 0x0012C94C
		// (set) Token: 0x060034F8 RID: 13560 RVA: 0x0012E754 File Offset: 0x0012C954
		[fsIgnore]
		public TIDateTime skipTime { get; private set; }

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x060034F9 RID: 13561 RVA: 0x0012E75D File Offset: 0x0012C95D
		// (set) Token: 0x060034FA RID: 13562 RVA: 0x0012E764 File Offset: 0x0012C964
		[fsIgnore]
		public static List<TIMissionTemplate> baseHumanMissions { get; private set; }

		// Token: 0x060034FB RID: 13563 RVA: 0x0012E76C File Offset: 0x0012C96C
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.resolutionSegmentsPerPhase = (from x in TemplateManager.IterateByClass<TIMissionTemplate>(true)
				select x.resolutionOrder).Distinct<int>().Count<int>();
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.StartNewMissionPhase), "CouncilorMissionUpdate", null, false, false);
			GameControl.eventManager.AddListener<MissionPhasePrepComplete>(new EventManager.EventDelegate<MissionPhasePrepComplete>(this.ContinueNewMissionPhase), null, null, false, false);
			this.promptQueue = GameStateManager.PromptQueue();
			TIMissionPhaseState.UpdatePerMonthTurnFrequency();
			if (this.factionsSignallingComplete == null)
			{
				this.factionsSignallingComplete = new List<TIFactionState>();
			}
			this.currentlyResolvingMissions = new List<TIMissionState>();
			TIMissionPhaseState.baseHumanMissions = (from x in TemplateManager.IterateByClass<TIMissionTemplate>(true)
				where x.baseMission
				select x).ToList<TIMissionTemplate>();
		}

		// Token: 0x060034FC RID: 13564 RVA: 0x0012E850 File Offset: 0x0012CA50
		public override void PostVisualizerCreationInit_6()
		{
			if (this.phaseActive)
			{
				GameControl.eventManager.TriggerEvent(new MissionPhaseRestart(), null, Array.Empty<object>());
				IEnumerable<TIFactionState> enumerable = GameStateManager.AllFactions().Except<TIFactionState>(this.factionsSignallingComplete);
				if (enumerable.Any<TIFactionState>((TIFactionState x) => x.player.isAI))
				{
					foreach (TIFactionState tifactionState in enumerable)
					{
						if (tifactionState.player.isAI)
						{
							AICouncilorMissionPlanner.singleton.SetRawNationPayoffsByFaction(tifactionState, true);
						}
					}
				}
				foreach (TIFactionState tifactionState2 in enumerable)
				{
					if (tifactionState2.player.isAI && !this.factionsSignallingComplete.Contains(tifactionState2))
					{
						this.promptQueue.AddPrompt(tifactionState2, this, null, "PromptSelectCouncilorMissions", 0);
					}
				}
				this.skipTime = TITimeState.Now();
			}
		}

		// Token: 0x060034FD RID: 13565 RVA: 0x0012E96C File Offset: 0x0012CB6C
		private void StartNewMissionPhase(TimeEventStart e)
		{
			this.newCampaignStart = false;
			if (this.skipTime == TITimeState.Now())
			{
				return;
			}
			if (this.phaseActive)
			{
				Log.Error("StartNewMissionPhase fired when mission phase was already active", Array.Empty<object>());
				this.phaseActive = false;
				return;
			}
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				if (tifactionState.player.isAI && !tifactionState.defeated)
				{
					tifactionState.preppingForMissions = true;
				}
			}
			foreach (TIFactionState tifactionState2 in GameStateManager.AllFactions())
			{
				if (tifactionState2.defeated)
				{
					this.factionsSignallingComplete.AddUnique(tifactionState2);
				}
				else
				{
					this.promptQueue.AddPrompt(tifactionState2, this, null, "PromptSelectCouncilorMissions", 0);
					TINotificationQueueState.AddCouncilorMessage((tifactionState2.councilors.Count > 0) ? tifactionState2.councilors.SelectRandomItem<TICouncilorState>() : null, CouncilorChatType.MissionPhaseHint, tifactionState2);
				}
			}
			foreach (TIFactionState tifactionState3 in GameStateManager.AllFactions())
			{
				if (tifactionState3.player.isAI && !tifactionState3.defeated)
				{
					AICouncilorMissionPlanner.singleton.MissionPhasePrepCoroutine(tifactionState3);
				}
			}
		}

		// Token: 0x060034FE RID: 13566 RVA: 0x0012EA88 File Offset: 0x0012CC88
		private void ContinueNewMissionPhase(MissionPhasePrepComplete e)
		{
			this.CancelOutstandingMissions();
			this.phaseActive = true;
			this.StartofTurnBookkeeping();
			GameControl.eventManager.TriggerEvent(new MissionPhaseStart(), null, Array.Empty<object>());
			TINotificationQueueState.LogNewCouncilorTurn();
			try
			{
				if (!TIPromptQueueState.ActivePlayerHasSaveBlockingPrompt())
				{
					if (File.Exists(StartMenuController.oldestAutoSaveFilepath) && File.Exists(StartMenuController.oldAutoSaveFilepath))
					{
						File.Delete(StartMenuController.oldestAutoSaveFilepath);
					}
					if (File.Exists(StartMenuController.oldAutoSaveFilepath))
					{
						File.Move(StartMenuController.oldAutoSaveFilepath, StartMenuController.oldestAutoSaveFilepath);
					}
					if (File.Exists(StartMenuController.autoSaveFilepath))
					{
						File.Move(StartMenuController.autoSaveFilepath, StartMenuController.oldAutoSaveFilepath);
					}
					GameStateManager.SaveAllGameStates(StartMenuController.autoSaveFilepath, false);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Failed to save file to path " + StartMenuController.autoSaveFilepath + ex.Message, Array.Empty<object>());
				StringBuilder stringBuilder = new StringBuilder(ex.Message);
				if (ex.Message.Contains("Win32 IO returned 112"))
				{
					stringBuilder.AppendLine(Loc.T("UI.Options.SaveFailLowDiskSpace"));
				}
				SaveMenuController.Singleton.DisplaySavingFailedDialog(stringBuilder.ToString());
			}
		}

		// Token: 0x060034FF RID: 13567 RVA: 0x0012EBA0 File Offset: 0x0012CDA0
		public void SetMissionPhaseInactive()
		{
			this.phaseActive = false;
			this.factionsSignallingComplete.Clear();
			GameControl.eventManager.TriggerEvent(new TimeEventComplete(null, null), "CouncilorMissionUpdate", Array.Empty<object>());
		}

		// Token: 0x06003500 RID: 13568 RVA: 0x0012EBCF File Offset: 0x0012CDCF
		public static bool InMissionPhase()
		{
			return GameStateManager.MissionPhase().phaseActive;
		}

		// Token: 0x06003501 RID: 13569 RVA: 0x0012EBDC File Offset: 0x0012CDDC
		private void StartofTurnBookkeeping()
		{
			foreach (TINationState tinationState in GameStateManager.AllExtantNations())
			{
				tinationState.UpdateControlPointStatus();
				tinationState.UpdateNativeControlPointsCount();
				tinationState.UpdateArmiesControllingFactions();
				tinationState.ClearAdvisingCouncilors();
			}
			foreach (TIHabState tihabState in GameStateManager.IterateByClass<TIHabState>(false))
			{
				tihabState.UpdateDefendHabStatus();
				tihabState.ClearAdvisingCouncilors();
			}
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				tifactionState.ActivateCouncilorOrgs();
				foreach (TICouncilorState ticouncilorState in tifactionState.councilors)
				{
					ticouncilorState.RecordLocation();
					ticouncilorState.EndProtectionOfTarget();
				}
			}
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				foreach (TICouncilorState ticouncilorState2 in array[i].councilors.OrderByDescending<TICouncilorState, int>((TICouncilorState x) => x.SumMissionRelevantAttributes()))
				{
					if (ticouncilorState2.repeatOrder)
					{
						if (ticouncilorState2.CanRepeatMission(ticouncilorState2.completedMission))
						{
							ticouncilorState2.faction.playerControl.StartAction(new AssignCouncilorToMission(ticouncilorState2, ticouncilorState2.completedMission.missionTemplate, ticouncilorState2.completedMission.target, ticouncilorState2.completedMission.resources, false));
						}
						else
						{
							ticouncilorState2.SetPermanentAssignment(false);
						}
					}
					else if (ticouncilorState2.permanentDefenseMode)
					{
						ticouncilorState2.SelectPermanentDefenseModeMission();
					}
					bool flag = false;
					TIMissionState completedMission = ticouncilorState2.completedMission;
					if (completedMission != null && completedMission.missionTemplate.persistentEffect)
					{
						flag = true;
					}
					ticouncilorState2.ClearCompletedMission();
					ticouncilorState2.SetCompletedMission(null);
					if (flag)
					{
						GameControl.eventManager.TriggerEvent(new CouncilorMissionUpdated(ticouncilorState2, null), null, new object[] { this, ticouncilorState2.faction, ticouncilorState2.location, ticouncilorState2.ref_nation }.Where<object>((object x) => x != null).ToArray<object>());
					}
				}
			}
			foreach (TIMissionState timissionState in GameStateManager.IterateByClass<TIMissionState>(false).ToList<TIMissionState>())
			{
				if (!TIGameState.Valid(timissionState.councilor))
				{
					GameStateManager.RemoveGameState<TIMissionState>(timissionState.ID, false);
				}
			}
			GameStateManager.NotificationQueue().CleanSummaryQueue(false);
		}

		// Token: 0x06003502 RID: 13570 RVA: 0x0012EF08 File Offset: 0x0012D108
		private void CancelOutstandingMissions()
		{
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				foreach (TICouncilorState ticouncilorState in array[i].councilors)
				{
					if (ticouncilorState.HasMission)
					{
						string[] array2 = new string[6];
						array2[0] = ticouncilorState.displayName;
						array2[1] = " had unresolved ";
						array2[2] = ticouncilorState.activeMission.displayName;
						array2[3] = ". Resolve time was planned for ";
						int num = 4;
						TIDateTime resolveTime = ticouncilorState.activeMission.resolveTime;
						array2[num] = ((resolveTime != null) ? resolveTime.ToString() : null) ?? "No time assigned";
						array2[5] = ".";
						Log.Error(string.Concat(array2), Array.Empty<object>());
						TITimeQueue gt = World.Active.GetExistingManager<GameTimeManager>().timeQueue;
						TIFactionState.LogAI("# events:" + gt.events.Count.ToString(), false);
						foreach (TITimeEvent titimeEvent in gt.events)
						{
							TIFactionState.LogAI(titimeEvent.eventName + " " + titimeEvent.time.ToCustomDateString(), false);
						}
						if (gt.events.Count != GameStateManager.IterateByClass<TITimeEvent>(false).Count<TITimeEvent>())
						{
							TIFactionState.LogAI("Events in State but not in queue:", false);
							IEnumerable<TITimeEvent> enumerable = GameStateManager.IterateByClass<TITimeEvent>(false);
							Func<TITimeEvent, bool> func;
							Func<TITimeEvent, bool> <>9__0;
							if ((func = <>9__0) == null)
							{
								func = (<>9__0 = (TITimeEvent x) => !gt.events.Contains(x));
							}
							foreach (TITimeEvent titimeEvent2 in enumerable.Where<TITimeEvent>(func))
							{
								TIFactionState.LogAI(titimeEvent2.eventName + " " + titimeEvent2.time.ToCustomDateString(), false);
							}
						}
						ticouncilorState.activeMission.ResolveMission(TIMissionState.AbortReason.BlanketCancel_ProbableError, "");
					}
				}
			}
		}

		// Token: 0x06003503 RID: 13571 RVA: 0x0012F180 File Offset: 0x0012D380
		public static TIGameState CouncilorLastKnownLocation(TIFactionState inspectingFaction, TICouncilorState councilorState)
		{
			if (councilorState.faction == inspectingFaction)
			{
				return councilorState.location;
			}
			if (TIMissionPhaseState.InMissionPhase())
			{
				return councilorState.preMissionPhaseLocation;
			}
			return councilorState.location;
		}

		// Token: 0x06003504 RID: 13572 RVA: 0x0012F1AC File Offset: 0x0012D3AC
		public static List<TICouncilorState> GetVisibleCouncilorsAtLocation(TIFactionState lookingFaction, TIGameState inputLocation, float minimumIntel, float maximumIntel = 1f, bool skipMine = false)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in from x in GameStateManager.IterateByClass<TICouncilorState>(false)
				where x.status == CouncilorStatus.Active && x.faction != null
				select x)
			{
				if (!ticouncilorState.InTransit())
				{
					if (ticouncilorState.faction == lookingFaction && ticouncilorState.location == inputLocation)
					{
						float intel = lookingFaction.GetIntel(ticouncilorState);
						if (!skipMine && intel >= minimumIntel && intel <= maximumIntel)
						{
							list.Add(ticouncilorState);
						}
					}
					else
					{
						float intel2 = lookingFaction.GetIntel(ticouncilorState);
						if (TIMissionPhaseState.CouncilorLastKnownLocation(lookingFaction, ticouncilorState) == inputLocation && intel2 >= minimumIntel && intel2 <= maximumIntel)
						{
							list.Add(ticouncilorState);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06003505 RID: 13573 RVA: 0x0012F288 File Offset: 0x0012D488
		public bool AllFactionsHaveAssignedMissions()
		{
			return this.factionsSignallingComplete.Count >= GameStateManager.AllFactions().Length;
		}

		// Token: 0x06003506 RID: 13574 RVA: 0x0012F2A4 File Offset: 0x0012D4A4
		public static void UpdatePerMonthTurnFrequency()
		{
			TITimeEvent titimeEvent = GameStateManager.FindByTemplate<TITimeEvent>("CouncilorMissionUpdate", false);
			TITimeQueueRepeatType? titimeQueueRepeatType = ((titimeEvent != null) ? new TITimeQueueRepeatType?(titimeEvent.repeatType) : null);
			if (titimeQueueRepeatType != null)
			{
				switch (titimeQueueRepeatType.GetValueOrDefault())
				{
				case TITimeQueueRepeatType.WeekToMonth:
					TIMissionPhaseState.phasesPerMonth = 4f;
					return;
				case TITimeQueueRepeatType.Semimonthly:
					TIMissionPhaseState.phasesPerMonth = 2f;
					return;
				case TITimeQueueRepeatType.EveryThreeWeeksToMonth:
					TIMissionPhaseState.phasesPerMonth = 1.3333334f;
					return;
				case TITimeQueueRepeatType.Month:
					TIMissionPhaseState.phasesPerMonth = 1f;
					return;
				}
			}
			if (!GameControl.control.skirmishMode)
			{
				Log.Error("Bad frequency passed to UpdatePerMonthTurnFrequency()", Array.Empty<object>());
			}
			TIMissionPhaseState.phasesPerMonth = 1f;
		}

		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x06003507 RID: 13575 RVA: 0x0012F352 File Offset: 0x0012D552
		public static TIDateTime nextMissionPhase
		{
			get
			{
				if (!TIMissionPhaseState.InMissionPhase())
				{
					return TIControlPoint.FindMissionPhaseAfter(TITimeState.Now());
				}
				return TITimeState.Now();
			}
		}

		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x06003508 RID: 13576 RVA: 0x0012F36B File Offset: 0x0012D56B
		public static double timeToNextMissionPhase_d
		{
			get
			{
				return TIMissionPhaseState.nextMissionPhase.DifferenceInDays(TITimeState.Now());
			}
		}

		// Token: 0x040023C2 RID: 9154
		public bool newCampaignStart;

		// Token: 0x040023C3 RID: 9155
		private TIPromptQueueState promptQueue;

		// Token: 0x040023C5 RID: 9157
		public List<TIFactionState> factionsSignallingComplete;

		// Token: 0x040023C6 RID: 9158
		[fsIgnore]
		public List<TIMissionState> currentlyResolvingMissions;
	}
}
