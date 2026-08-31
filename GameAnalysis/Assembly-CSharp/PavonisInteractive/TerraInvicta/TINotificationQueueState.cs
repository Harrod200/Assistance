using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000787 RID: 1927
	public class TINotificationQueueState : TIGameState
	{
		// Token: 0x17000ACC RID: 2764
		// (get) Token: 0x06003C5E RID: 15454 RVA: 0x0016ED2A File Offset: 0x0016CF2A
		// (set) Token: 0x06003C5F RID: 15455 RVA: 0x0016ED32 File Offset: 0x0016CF32
		[fsIgnore]
		public List<NotificationQueueItem> notificationQueue { get; private set; }

		// Token: 0x17000ACD RID: 2765
		// (get) Token: 0x06003C60 RID: 15456 RVA: 0x0016ED3B File Offset: 0x0016CF3B
		// (set) Token: 0x06003C61 RID: 15457 RVA: 0x0016ED43 File Offset: 0x0016CF43
		[fsIgnore]
		public Queue<CouncilorMessage> councilorMessages { get; private set; }

		// Token: 0x17000ACE RID: 2766
		// (get) Token: 0x06003C62 RID: 15458 RVA: 0x0016ED4C File Offset: 0x0016CF4C
		// (set) Token: 0x06003C63 RID: 15459 RVA: 0x0016ED54 File Offset: 0x0016CF54
		public List<NotificationSummaryItem> notificationSummaryQueue { get; private set; }

		// Token: 0x17000ACF RID: 2767
		// (get) Token: 0x06003C64 RID: 15460 RVA: 0x0016ED5D File Offset: 0x0016CF5D
		// (set) Token: 0x06003C65 RID: 15461 RVA: 0x0016ED65 File Offset: 0x0016CF65
		public List<NotificationSummaryItem> timerNotificationQueue { get; private set; }

		// Token: 0x17000AD0 RID: 2768
		// (get) Token: 0x06003C66 RID: 15462 RVA: 0x0016ED6E File Offset: 0x0016CF6E
		// (set) Token: 0x06003C67 RID: 15463 RVA: 0x0016ED76 File Offset: 0x0016CF76
		public Dictionary<SummaryCategory, List<NotificationSummaryItem>> panelSummaryQueue { get; private set; }

		// Token: 0x17000AD1 RID: 2769
		// (get) Token: 0x06003C68 RID: 15464 RVA: 0x0016ED7F File Offset: 0x0016CF7F
		public static TIFactionState activePlayer
		{
			get
			{
				return GameControl.control.activePlayer;
			}
		}

		// Token: 0x06003C69 RID: 15465 RVA: 0x0016ED8B File Offset: 0x0016CF8B
		public override bool Initialize()
		{
			this.notificationSummaryQueue = new List<NotificationSummaryItem>();
			return base.Initialize();
		}

		// Token: 0x06003C6A RID: 15466 RVA: 0x0016EDA0 File Offset: 0x0016CFA0
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (this.timerNotificationQueue == null)
			{
				this.timerNotificationQueue = new List<NotificationSummaryItem>();
			}
			if (this.panelSummaryQueue == null)
			{
				this.panelSummaryQueue = new Dictionary<SummaryCategory, List<NotificationSummaryItem>>();
				foreach (object obj in Enum.GetValues(typeof(SummaryCategory)))
				{
					SummaryCategory summaryCategory = (SummaryCategory)obj;
					if (summaryCategory != SummaryCategory.None)
					{
						this.panelSummaryQueue.Add(summaryCategory, new List<NotificationSummaryItem>());
					}
				}
			}
			if (this.firstTimeTracker == null)
			{
				this.firstTimeTracker = GameStateManager.AllFactions().ToDictionary<TIFactionState, TIFactionState, Dictionary<string, int>>((TIFactionState x) => x, (TIFactionState x) => new Dictionary<string, int>());
			}
			this.councilorMessages = new Queue<CouncilorMessage>();
			this.usedCouncilorMessages = new Dictionary<TIFactionState.Advice, int>();
			foreach (object obj2 in Enum.GetValues(typeof(TIFactionState.Advice)))
			{
				TIFactionState.Advice advice = (TIFactionState.Advice)obj2;
				this.usedCouncilorMessages.Add(advice, 0);
			}
			this.notificationQueue = new List<NotificationQueueItem>();
			this.promptQueue = GameStateManager.PromptQueue();
			this.notificationSummaryQueue = this.notificationSummaryQueue.Where<NotificationSummaryItem>((NotificationSummaryItem x) => x.template != null).ToList<NotificationSummaryItem>();
			this.timerNotificationQueue = this.timerNotificationQueue.Where<NotificationSummaryItem>((NotificationSummaryItem x) => x.template != null).ToList<NotificationSummaryItem>();
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnAlarmTriggered), "PlayerAlarm", null, false, false);
		}

		// Token: 0x06003C6B RID: 15467 RVA: 0x0016EF9C File Offset: 0x0016D19C
		public override void PostVisualizerCreationInit_7()
		{
			this.CleanSummaryQueue(true);
			AudioManager.SetIntensity(this.GetBaselineMusicIntensity(TINotificationQueueState.activePlayer));
		}

		// Token: 0x06003C6C RID: 15468 RVA: 0x0016EFB8 File Offset: 0x0016D1B8
		private static void AddItem(NotificationQueueItem item, bool addToAlienQueue = false)
		{
			if (item.template == null)
			{
				Log.Error("Null notification template for " + item.templateName + ". No notification pushed.", Array.Empty<object>());
				return;
			}
			item.dateTime = TITimeState.Now();
			item.dateTimeString = item.dateTime.ToCustomDateString();
			item.primaryFactions = item.primaryFactions.Where<TIFactionState>((TIFactionState x) => x != null).Distinct<TIFactionState>().ToList<TIFactionState>();
			item.relevantFactions = item.relevantFactions.Where<TIFactionState>((TIFactionState x) => x != null).Distinct<TIFactionState>().ToList<TIFactionState>();
			if (string.IsNullOrEmpty(item.itemDetail))
			{
				item.itemDetail = item.itemSummary;
			}
			else if (string.IsNullOrEmpty(item.itemSummary))
			{
				item.itemSummary = item.itemDetail;
			}
			TINotificationTemplate template = item.template;
			item.itemSummary = Loc.T("UI.Notifications.DateLog", new object[] { item.dateTimeString, item.itemSummary });
			TINotificationQueueState tinotificationQueueState = GameStateManager.NotificationQueue();
			tinotificationQueueState.notificationQueue.Insert(0, item);
			if (tinotificationQueueState.notificationQueue.Count > 60)
			{
				tinotificationQueueState.notificationQueue.RemoveRange(60, tinotificationQueueState.notificationQueue.Count - 60);
			}
			if (addToAlienQueue)
			{
				tinotificationQueueState.alienEvents++;
			}
			if (!string.IsNullOrEmpty(item.alertBlockEventName))
			{
				if (item.promptingGameState.isNationState)
				{
					tinotificationQueueState.promptQueue.AddPrompt(item.promptingGameState.ref_nation, item.alertBlockFaction, item.alertRelatedState, item.alertBlockEventName, item.utilityValue);
				}
				else
				{
					tinotificationQueueState.promptQueue.AddPrompt(item.alertBlockFaction, item.promptingGameState, item.alertRelatedState, item.alertBlockEventName, item.utilityValue);
				}
			}
			NotificationSummaryItem notificationSummaryItem = new NotificationSummaryItem(item.itemSummary, item.icon, item.iconBackgroundResource, item.backgroundColor, item.gotoGameState, addToAlienQueue, item.dateTime, item.templateName, item.timerFactions, item.newsFeedFactions, item.summaryLogFactions, item.outcome);
			List<TIFactionState> list = new List<TIFactionState>();
			List<TIFactionState> list2 = new List<TIFactionState>(item.alertFactions);
			if (list2.Count > 0)
			{
				list.AddRangeUnique<TIFactionState>(list2);
			}
			if (item.putInNewsFeed)
			{
				tinotificationQueueState.notificationSummaryQueue.Insert(0, notificationSummaryItem);
				list.AddRangeUnique<TIFactionState>(item.newsFeedFactions);
			}
			if (item.putInTimerQueue)
			{
				tinotificationQueueState.timerNotificationQueue.Insert(0, notificationSummaryItem);
				list.AddRangeUnique<TIFactionState>(item.timerFactions);
				if (tinotificationQueueState.timerNotificationQueue.Count > 60)
				{
					tinotificationQueueState.timerNotificationQueue.RemoveRange(60, tinotificationQueueState.timerNotificationQueue.Count - 60);
				}
			}
			if (item.putInSummaryLog)
			{
				SummaryCategory category = item.template.summaryAudience.category;
				tinotificationQueueState.panelSummaryQueue[category].Insert(0, notificationSummaryItem);
				list.AddRangeUnique<TIFactionState>(item.summaryLogFactions);
				if (tinotificationQueueState.panelSummaryQueue[category].Count > TINotificationQueueState.maxSummaryQueueSize[category])
				{
					tinotificationQueueState.panelSummaryQueue[category].RemoveRange(TINotificationQueueState.maxSummaryQueueSize[category], tinotificationQueueState.panelSummaryQueue[category].Count - TINotificationQueueState.maxSummaryQueueSize[category]);
				}
			}
			if (item.template.firstAlertOverride)
			{
				foreach (TIFactionState tifactionState in list2)
				{
					if (tifactionState.checkNotificationOverrides && TINotificationQueueState.FirstNotificationOfType(tifactionState, item.templateName) && item.template.alertAudience == NotificationAudience.None && (!tifactionState.notificationOverrides.ContainsKey(item.templateName) || tifactionState.notificationOverrides[item.templateName].alert != NotificationOverrideBehavior.Add))
					{
						item.itemDetail = new StringBuilder(item.itemDetail).AppendLine().AppendLine(Loc.T("UI.Notifications.OneTimeOnly")).ToString();
					}
				}
			}
			if (list.Count > 0)
			{
				EventManager eventManager = GameControl.eventManager;
				GameEvent gameEvent = new NewsItemCreated(item, notificationSummaryItem);
				string text = null;
				object[] array = list.ToArray();
				eventManager.TriggerEvent(gameEvent, text, array);
			}
		}

		// Token: 0x06003C6D RID: 15469 RVA: 0x0016F408 File Offset: 0x0016D608
		private static void AddRapidItem(NotificationSummaryItem summary, List<TIFactionState> factions)
		{
			TINotificationQueueState tinotificationQueueState = GameStateManager.NotificationQueue();
			bool flag = false;
			if (summary.template.newsFeedAudience != NotificationAudience.None)
			{
				if (factions.Any<TIFactionState>((TIFactionState x) => x.showRegularNotifications))
				{
					tinotificationQueueState.notificationSummaryQueue.Insert(0, summary);
					flag = true;
				}
			}
			if (summary.template.timerAudience != NotificationAudience.None)
			{
				if (factions.Any<TIFactionState>((TIFactionState x) => x.showTimerNotifications))
				{
					tinotificationQueueState.timerNotificationQueue.Insert(0, summary);
					if (tinotificationQueueState.timerNotificationQueue.Count > 60)
					{
						tinotificationQueueState.timerNotificationQueue.RemoveRange(60, tinotificationQueueState.timerNotificationQueue.Count - 60);
					}
					flag = true;
				}
			}
			if (factions.Any<TIFactionState>((TIFactionState x) => x.showSummaryLogs))
			{
				SummaryCategory category = summary.template.summaryAudience.category;
				tinotificationQueueState.panelSummaryQueue[category].Insert(0, summary);
				if (tinotificationQueueState.panelSummaryQueue[category].Count > TINotificationQueueState.maxSummaryQueueSize[category])
				{
					tinotificationQueueState.panelSummaryQueue[category].RemoveRange(TINotificationQueueState.maxSummaryQueueSize[category], tinotificationQueueState.panelSummaryQueue[category].Count - TINotificationQueueState.maxSummaryQueueSize[category]);
				}
				flag = true;
			}
			if (flag)
			{
				EventManager eventManager = GameControl.eventManager;
				GameEvent gameEvent = new RapidLogItemCreated(summary);
				string text = null;
				object[] array = factions.ToArray();
				eventManager.TriggerEvent(gameEvent, text, array);
			}
		}

		// Token: 0x06003C6E RID: 15470 RVA: 0x0016F590 File Offset: 0x0016D790
		private void OnAlarmTriggered(TimeEventStart e)
		{
			TIFactionState ref_faction = e.eventObject.ref_faction;
			if (ref_faction != null)
			{
				foreach (Alarm alarm in ref_faction.alarms.ToList<Alarm>())
				{
					if (alarm.associatedGameState == e.eventObject2 && e.startTime == alarm.time)
					{
						TINotificationQueueState.LogAlarmTriggered(e.eventObject.ref_faction, e.eventObject2, alarm);
						e.eventObject.ref_faction.alarms.Remove(alarm);
						GameControl.eventManager.TriggerEvent(new AlarmTriggered(e.eventObject.ref_faction, e.eventObject2), null, new object[]
						{
							e.eventObject.ref_faction,
							e.eventObject2
						});
					}
				}
			}
		}

		// Token: 0x06003C6F RID: 15471 RVA: 0x0016F694 File Offset: 0x0016D894
		public static bool FirstNotificationOfType(TIFactionState faction, string notificationTemplateName)
		{
			return !GameStateManager.NotificationQueue().firstTimeTracker[faction].ContainsKey(notificationTemplateName) || GameStateManager.NotificationQueue().firstTimeTracker[faction][notificationTemplateName] < 1;
		}

		// Token: 0x06003C70 RID: 15472 RVA: 0x0016F6CC File Offset: 0x0016D8CC
		public static void CheckAndSetFirstNotificationOfType(NotificationQueueItem item)
		{
			if (item.template.firstAlertOverride)
			{
				foreach (TIFactionState tifactionState in item.alertFactions)
				{
					if (tifactionState.checkNotificationOverrides && TINotificationQueueState.FirstNotificationOfType(tifactionState, item.templateName) && item.template.alertAudience == NotificationAudience.None && (!tifactionState.notificationOverrides.ContainsKey(item.templateName) || tifactionState.notificationOverrides[item.templateName].alert != NotificationOverrideBehavior.Add))
					{
						TINotificationQueueState.SetFirstNotificationofType(tifactionState, item.templateName);
					}
				}
			}
		}

		// Token: 0x06003C71 RID: 15473 RVA: 0x0016F784 File Offset: 0x0016D984
		public static void SetFirstNotificationofType(TIFactionState faction, string notificationTemplateName)
		{
			if (!GameStateManager.NotificationQueue().firstTimeTracker[faction].ContainsKey(notificationTemplateName))
			{
				GameStateManager.NotificationQueue().firstTimeTracker[faction].Add(notificationTemplateName, 0);
			}
			Dictionary<string, int> dictionary = GameStateManager.NotificationQueue().firstTimeTracker[faction];
			dictionary[notificationTemplateName]++;
		}

		// Token: 0x17000AD2 RID: 2770
		// (get) Token: 0x06003C72 RID: 15474 RVA: 0x0016F7E3 File Offset: 0x0016D9E3
		private static List<TIFactionState> AllFactions
		{
			get
			{
				return GameStateManager.AllFactions().ToList<TIFactionState>();
			}
		}

		// Token: 0x06003C73 RID: 15475 RVA: 0x0016F7EF File Offset: 0x0016D9EF
		private static List<TIFactionState> AllFactionsExcept(TIFactionState councilState)
		{
			List<TIFactionState> allFactions = TINotificationQueueState.AllFactions;
			allFactions.Remove(councilState);
			return allFactions;
		}

		// Token: 0x06003C74 RID: 15476 RVA: 0x0016F800 File Offset: 0x0016DA00
		private static List<TIFactionState> AllFactionsWithIntel(TIGameState target, float intelThreshhold)
		{
			List<TIFactionState> list = new List<TIFactionState>();
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				if (tifactionState.SufficientIntel(target, intelThreshhold))
				{
					list.Add(tifactionState);
				}
			}
			return list;
		}

		// Token: 0x06003C75 RID: 15477 RVA: 0x0016F840 File Offset: 0x0016DA40
		private static TIGameState GenericGotoState(TIGameState state)
		{
			if (state.ref_region != null)
			{
				return state.ref_region;
			}
			if (state.ref_fleet != null)
			{
				return state.ref_fleet;
			}
			if (state.ref_hab != null)
			{
				return state.ref_hab;
			}
			if (state.ref_spaceBody != null)
			{
				return state.ref_spaceBody;
			}
			return null;
		}

		// Token: 0x06003C76 RID: 15478 RVA: 0x0016F8A2 File Offset: 0x0016DAA2
		private static NotificationQueueItem InitItem(string templateName)
		{
			return new NotificationQueueItem
			{
				relevantFactions = new List<TIFactionState>(),
				primaryFactions = new List<TIFactionState>(),
				alertBlockFaction = null,
				templateName = templateName
			};
		}

		// Token: 0x06003C77 RID: 15479 RVA: 0x0016F8D0 File Offset: 0x0016DAD0
		public static void CleanQueueOfArchivedState(TIGameState state, TIGameState substituteState = null)
		{
			foreach (NotificationQueueItem notificationQueueItem in GameStateManager.NotificationQueue().notificationQueue)
			{
				if (notificationQueueItem.gotoGameState == state)
				{
					notificationQueueItem.gotoGameState = substituteState;
				}
			}
			foreach (NotificationSummaryItem notificationSummaryItem in GameStateManager.NotificationQueue().notificationSummaryQueue)
			{
				if (notificationSummaryItem.gotoGameState == state)
				{
					notificationSummaryItem.UpdateGotoGameState(substituteState);
				}
			}
		}

		// Token: 0x06003C78 RID: 15480 RVA: 0x0016F98C File Offset: 0x0016DB8C
		public void CleanSummaryQueue(bool logit)
		{
			int num = 0;
			foreach (NotificationSummaryItem notificationSummaryItem in this.notificationSummaryQueue.ToList<NotificationSummaryItem>())
			{
				if (TITimeState.Now().DifferenceInDays(notificationSummaryItem.dateTime) > (double)(notificationSummaryItem.alienRelated ? 1826 : 60) || notificationSummaryItem.templateName.Contains("Rapid_"))
				{
					this.notificationSummaryQueue.Remove(notificationSummaryItem);
					if (notificationSummaryItem.alienRelated)
					{
						this.alienEvents--;
					}
					num++;
				}
			}
			if (logit && num > 0)
			{
				Log.Info("Removing " + num.ToString() + " notification summaries. Remaining: " + this.notificationSummaryQueue.Count.ToString(), Array.Empty<object>());
			}
			num = 0;
			foreach (object obj in Enum.GetValues(typeof(SummaryCategory)))
			{
				SummaryCategory summaryCategory = (SummaryCategory)obj;
				if (this.panelSummaryQueue.ContainsKey(summaryCategory) && this.panelSummaryQueue[summaryCategory].Count > TINotificationQueueState.maxSummaryQueueSize[summaryCategory])
				{
					num += this.panelSummaryQueue[summaryCategory].Count - TINotificationQueueState.maxSummaryQueueSize[summaryCategory];
					this.panelSummaryQueue[summaryCategory].RemoveRange(TINotificationQueueState.maxSummaryQueueSize[summaryCategory], this.panelSummaryQueue[summaryCategory].Count - TINotificationQueueState.maxSummaryQueueSize[summaryCategory]);
				}
			}
			if (logit && num > 0)
			{
				Log.Info("Removing " + num.ToString() + " notification panel summaries. Remaining: " + this.panelSummaryQueue.Values.Count.ToString(), Array.Empty<object>());
			}
			num = 0;
			if (this.timerNotificationQueue.Count > 60)
			{
				num = this.timerNotificationQueue.Count - 60;
				this.timerNotificationQueue.RemoveRange(60, this.timerNotificationQueue.Count - 60);
			}
			if (logit && num > 0)
			{
				Log.Info("Removing " + num.ToString() + " timer notifications. Remaining: " + this.timerNotificationQueue.Count.ToString(), Array.Empty<object>());
			}
		}

		// Token: 0x06003C79 RID: 15481 RVA: 0x0016FC14 File Offset: 0x0016DE14
		public static string councilorGUIIconPath(TICouncilorState councilor)
		{
			return councilor.iconResource;
		}

		// Token: 0x06003C7A RID: 15482 RVA: 0x0016FC1C File Offset: 0x0016DE1C
		public float GetBaselineMusicIntensity(TIFactionState playerFaction)
		{
			if (playerFaction.executiveNations.Any<TINationState>((TINationState x) => x.wars.Count > 0))
			{
				return 0.45f;
			}
			if (playerFaction.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Unlocked).Count > 0)
			{
				return 0.45f;
			}
			if (GameStateManager.AlienFaction().fleets.Any<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				TIOrbitState ref_orbit = x.ref_orbit;
				return ref_orbit != null && ref_orbit.isEarthLEO;
			}))
			{
				return 0.45f;
			}
			return 0f;
		}

		// Token: 0x06003C7B RID: 15483 RVA: 0x0016FCAC File Offset: 0x0016DEAC
		private static string GetRandomQuietExplosion()
		{
			switch (TIUtilities.RandomRange(0, 6))
			{
			default:
				return "event:/SFX/Game_SFX/Explosions/trig_sfx_ExplosionSubtlePoof01Stereo";
			case 1:
				return "event:/SFX/Game_SFX/Explosions/trig_sfx_ExplosionSubtilePoof02Stereo";
			case 2:
				return "event:/SFX/Game_SFX/Explosions/trig_sfx_ExplosionShortSmoothCleanStereo";
			case 3:
				return "event:/SFX/Game_SFX/Explosions/trig_sfx_ExplosionShortSmoothCleanDeepStereo";
			case 4:
				return "event:/SFX/Game_SFX/Explosions/trig_sfx_ExplosionShortSmoothCleanKickbackSmoothTailStereo";
			case 5:
				return "event:/SFX/Game_SFX/Ship_Fire_Impacts/trig_SFX_EXPLOSIONSubtleFoofStereoRR";
			}
		}

		// Token: 0x06003C7C RID: 15484 RVA: 0x0016FD04 File Offset: 0x0016DF04
		public static void LogPrecrashCampaignStart()
		{
			string text = "UI.Notifications.NewCampaign.Detail";
			string text2 = Loc.T_Scenario(text);
			if (text == text2)
			{
				return;
			}
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = GameStateManager.AllHumanFactions().ToList<TIFactionState>();
			notificationQueueItem.primaryFactions = GameStateManager.AllHumanFactions().ToList<TIFactionState>();
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeCrashdown_gui;
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeCrashdown_gui;
			notificationQueueItem.itemSummary = Loc.T_Scenario("UI.Notifications.NewCampaign.Summary");
			notificationQueueItem.itemHeadline = Loc.T_Scenario("UI.Notifications.NewCampaign.Headline");
			notificationQueueItem.itemDetail = Loc.T_Scenario(text2);
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_BSBE_preCrashIntro;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_AlienEarthAlarm";
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C7D RID: 15485 RVA: 0x0016FDD0 File Offset: 0x0016DFD0
		public static void LogCampaignStart(TIFactionState faction, TIRegionState initialLandingLocation)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.icon = faction.factionIcon64path;
			notificationQueueItem.popupResource1 = faction.factionIcon256path;
			notificationQueueItem.popupResource2 = faction.factionIcon256path;
			if (TIPlayerProfileManager.useCouncilorVideo)
			{
				notificationQueueItem.videoResource = faction.pathLeaderTorsoVideo;
			}
			else
			{
				notificationQueueItem.showSideArt = true;
				notificationQueueItem.illustrationResource = faction.pathLeaderTorsoPortration;
			}
			notificationQueueItem.itemSummary = Loc.T("TIFactionTemplate.CampaignStartSummary", new object[] { faction.displayNameWithColor });
			notificationQueueItem.itemDetail = Loc.T_Scenario(faction.template.campaignPlayerIntroPath, new object[]
			{
				faction.leaderAddress,
				faction.displayName,
				faction.template.leaderName,
				initialLandingLocation.nation.displayNameWithArticleAndPlacePrep,
				initialLandingLocation.displayName
			});
			notificationQueueItem.itemHeadline = Loc.T(faction.template.campaignStartHeadline);
			notificationQueueItem.gotoGameState = initialLandingLocation;
			notificationQueueItem.soundToPlay = AudioManager.GetScenarioAudioPostFix(TIUtilities.CombineStrings(new string[]
			{
				"event:/VO/ENG/Faction/Faction_CampaignStart_",
				faction.ideology.ideology.ToString()
			}));
			notificationQueueItem.fanfareToPlay = faction.template.fanfarePath;
			if (faction.isActivePlayer)
			{
				GameControl.control.activePlayer.UnlockAchievement("startCampaign");
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C7E RID: 15486 RVA: 0x0016FF4C File Offset: 0x0016E14C
		public static void LogTutorialStart(TIFactionState faction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.icon = faction.factionIcon64path;
			notificationQueueItem.popupResource1 = faction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.TutorialHed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.TutorialSummary");
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.TutorialDetail", new object[] { TemplateManager.global.tutorialInlineSpritePath });
			notificationQueueItem.gotoGameState = null;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C7F RID: 15487 RVA: 0x0016FFEC File Offset: 0x0016E1EC
		public static string AlarmString(TIFactionState faction, TIGameState target, Alarm alarm)
		{
			string text = string.Empty;
			AlarmType alarmType = alarm.alarmType;
			if (alarmType != AlarmType.FleetApproaching)
			{
				if (alarmType == AlarmType.PlayerAlarm)
				{
					text = alarm.customPlayerString;
				}
			}
			else if (target.ref_fleet.transferAssigned)
			{
				string text2 = "UI.Alarm.FleetApproaching";
				object[] array = new object[4];
				array[0] = target.ref_fleet.GetDisplayName(faction);
				int num = 1;
				TISpaceGameState destination = target.ref_fleet.trajectory.destination;
				array[num] = ((destination != null) ? destination.GetDisplayName(faction) : null) ?? "";
				array[2] = target.ref_fleet.trajectory.arrivalTime.ToCustomTimeString();
				array[3] = target.ref_fleet.trajectory.arrivalTime.ToCustomDateString();
				text = Loc.T(text2, array);
			}
			return text;
		}

		// Token: 0x06003C80 RID: 15488 RVA: 0x001700AC File Offset: 0x0016E2AC
		public static void LogAlarmTriggered(TIFactionState faction, TIGameState target, Alarm alarm)
		{
			if (!TIGameState.Valid(target))
			{
				return;
			}
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			string text = TIUtilities.GetStateIconPath(faction, target, false);
			if (text == null)
			{
				text = faction.factionIcon64path;
			}
			notificationQueueItem.icon = text;
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlarmHed", new object[] { target.GetDisplayName(faction) });
			string text2 = TINotificationQueueState.AlarmString(faction, target, alarm);
			notificationQueueItem.itemSummary = text2;
			notificationQueueItem.itemDetail = text2;
			notificationQueueItem.gotoGameState = (target.hasMapObject ? target : null);
			notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_IncomingComms";
			if (target.isSpaceFleetState)
			{
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetAlarm);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetAlarm);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetAlarm);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetAlarm);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetAlarm);
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C81 RID: 15489 RVA: 0x001701B8 File Offset: 0x0016E3B8
		public static void LogFactionWin(TIFactionState faction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(TINotificationQueueState.AllFactions);
			notificationQueueItem.relevantFactions.AddRange(TINotificationQueueState.AllFactions);
			notificationQueueItem.icon = faction.factionIcon64path;
			notificationQueueItem.popupResource1 = faction.factionIcon256path;
			notificationQueueItem.popupResource2 = faction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.VictoryHed", new object[] { faction.displayNameCapitalized });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.Victory", new object[] { faction.displayNameCapitalizedWithColor });
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.Victory", new object[] { faction.displayNameCapitalizedWithColor }));
			stringBuilder.AppendLine().AppendLine().AppendLine(Loc.T(new StringBuilder("TIFactionTemplate.WinNotification.").Append(faction.templateName).ToString(), new object[] { faction.adjectiveWithColor }));
			if (faction.victoryTemplate.victoryEffect != TIVictoryTemplate.VictoryEffectType.EndGame && faction != GameControl.control.activePlayer)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Notifications.VictoryContinue"));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.illustrationResource = faction.template.winIllustration;
			notificationQueueItem.movieResource = faction.cinematicsPath + "_win";
			notificationQueueItem.triggerEndGame = faction != GameControl.control.activePlayer && faction.victoryTemplate.victoryEffect == TIVictoryTemplate.VictoryEffectType.EndGame;
			if (faction == GameControl.control.activePlayer)
			{
				if (faction.atrocities <= 3)
				{
					faction.UnlockAchievement("lowAtrocityWin");
				}
				if (!TIGlobalValuesState.Customizations.customDifficulty)
				{
					if (TIGlobalValuesState.GlobalValues.difficulty >= 2)
					{
						faction.UnlockAchievement("normalWin");
					}
					if (TIGlobalValuesState.GlobalValues.difficulty >= 3)
					{
						faction.UnlockAchievement("veteranWin");
					}
					if (TIGlobalValuesState.GlobalValues.difficulty >= 4)
					{
						faction.UnlockAchievement("brutalWin");
					}
				}
				switch (faction.ideology.ideology)
				{
				case FactionIdeology.Destroy:
					faction.UnlockAchievement("destroyWin");
					break;
				case FactionIdeology.Resist:
					faction.UnlockAchievement("resistWin");
					foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
					{
						if ((tifactionState.ideology.ideology == FactionIdeology.Appease || tifactionState.ideology.ideology == FactionIdeology.Submit) && tifactionState.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Completed).Count > 0)
						{
							faction.UnlockAchievement("winPheonix");
						}
					}
					break;
				case FactionIdeology.Escape:
					faction.UnlockAchievement("escapeWin");
					break;
				case FactionIdeology.Exploit:
					faction.UnlockAchievement("exploitWin");
					break;
				case FactionIdeology.Cooperate:
					faction.UnlockAchievement("cooperateWin");
					break;
				case FactionIdeology.Appease:
					faction.UnlockAchievement("appeaseWin");
					break;
				case FactionIdeology.Submit:
					faction.UnlockAchievement("submitWin");
					break;
				}
				if (faction.WonWithAllFactions())
				{
					faction.UnlockAchievement("allFactionWin");
				}
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C82 RID: 15490 RVA: 0x001704C8 File Offset: 0x0016E6C8
		public static void LogTimeChangeUpdate(TITimeQueueRepeatType newUpdateTiming)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(TINotificationQueueState.AllFactions);
			notificationQueueItem.relevantFactions.AddRange(TINotificationQueueState.AllFactions);
			notificationQueueItem.icon = "icons_2d/ICO_clock";
			switch (newUpdateTiming)
			{
			case TITimeQueueRepeatType.Semimonthly:
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.CouncilorMissionUpdateHed");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.CouncilorMissionUpdateSummary");
				notificationQueueItem.itemDetail = Loc.T_Scenario("UI.Notifications.CouncilorMissionUpdateDetail");
				break;
			case TITimeQueueRepeatType.EveryThreeWeeksToMonth:
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.CouncilorMissionUpdateHed2");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.CouncilorMissionUpdateSummary2");
				notificationQueueItem.itemDetail = Loc.T_Scenario("UI.Notifications.CouncilorMissionUpdateDetail2");
				break;
			case TITimeQueueRepeatType.Month:
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.CouncilorMissionUpdateHed3");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.CouncilorMissionUpdateSummary3");
				notificationQueueItem.itemDetail = Loc.T_Scenario("UI.Notifications.CouncilorMissionUpdateDetail3");
				break;
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C83 RID: 15491 RVA: 0x001705C4 File Offset: 0x0016E7C4
		public static void LogFleetDetected(TIFactionState detectingFaction, TISpaceFleetState fleet)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			bool flag = fleet.ref_faction == GameStateManager.AlienFaction();
			if (detectingFaction == fleet.ref_faction)
			{
				return;
			}
			if (fleet.isSpaceFleetState)
			{
				notificationQueueItem.relevantFactions.Add(detectingFaction);
				notificationQueueItem.primaryFactions.Add(detectingFaction);
				if (fleet.ships.Count == 1)
				{
					notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetSightedHeadline_1", new object[] { fleet.faction.adjective });
				}
				else
				{
					notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetSightedHeadline", new object[] { fleet.faction.adjective });
				}
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetSightedDetail", new object[]
				{
					fleet.faction.adjectiveWithColor,
					fleet.GetLocationDescription(TINotificationQueueState.activePlayer, false, false)
				});
				notificationQueueItem.popupResource1 = fleet.iconResource;
				notificationQueueItem.soundToPlay = (flag ? "event:/SFX/UI_Special_SFX/trig_SFX_Aliens_Sighted_Space" : string.Empty);
				notificationQueueItem.musicIntensityDelta = 0.45f;
				notificationQueueItem.icon = fleet.iconResource;
				notificationQueueItem.illustrationResource = (fleet.landed ? fleet.ref_habSite.template.backgroundPath : World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath);
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetSightedSummary", new object[]
				{
					fleet.faction.adjectiveWithColor,
					fleet.GetLocationDescription(TINotificationQueueState.activePlayer, false, false)
				});
				notificationQueueItem.gotoGameState = fleet;
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
				TINotificationQueueState.AddItem(notificationQueueItem, flag);
			}
		}

		// Token: 0x06003C84 RID: 15492 RVA: 0x00170768 File Offset: 0x0016E968
		public static void LogHumanHabDetected(TIFactionState detectingFaction, TIHabState hab)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			if (detectingFaction == hab.ref_faction)
			{
				return;
			}
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.gotoGameState = hab;
			if (hab.IsBase)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.BaseSightedHeadline", new object[] { hab.faction.adjectiveWithColor });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.BaseSightedSummary", new object[]
				{
					hab.faction.displayNameWithColor,
					hab.habSite.displayName,
					hab.habSite.parentBody.displayName
				});
			}
			else
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.StationSightedHeadline", new object[] { hab.faction.adjectiveWithColor });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.StationSightedSummary", new object[]
				{
					hab.faction.displayNameWithColor,
					hab.barycenter.displayName
				});
			}
			notificationQueueItem.itemDetail = new StringBuilder(notificationQueueItem.itemSummary).Append(Loc.T("UI.Notifications.HabSightedDetail", new object[] { hab.GetDisplayName(detectingFaction) })).ToString();
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C85 RID: 15493 RVA: 0x001708DC File Offset: 0x0016EADC
		public static void LogAlienHabDetected(TIFactionState detectingFaction, TIHabState hab)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			if (detectingFaction == hab.ref_faction)
			{
				return;
			}
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.musicIntensityDelta = 0.1f;
			notificationQueueItem.popupResource1 = hab.iconResource;
			if (hab.IsBase)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.BaseSightedHeadline", new object[] { hab.faction.adjective });
			}
			else
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.StationSightedHeadline", new object[] { hab.faction.adjective });
			}
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.gotoGameState = hab;
			if (hab.IsBase)
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.BaseSightedSummary", new object[]
				{
					hab.faction.displayNameWithColor,
					hab.habSite.displayName,
					hab.habSite.parentBody.displayName
				});
			}
			else
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.StationSightedSummary", new object[]
				{
					hab.faction.displayNameWithColor,
					hab.barycenter.displayName
				});
			}
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003C86 RID: 15494 RVA: 0x00170A3C File Offset: 0x0016EC3C
		public static void LogAssaultCarrierLaunchesTowardEarth(List<TIFactionState> notifyFactions, TISpaceFleetState alienFleet, TIDateTime arrival)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(notifyFactions);
			notificationQueueItem.relevantFactions.AddRange(notifyFactions);
			notificationQueueItem.musicIntensityDelta = 0.34f;
			notificationQueueItem.icon = alienFleet.iconResource;
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.gotoGameState = alienFleet;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AssaultCarrierLaunchesTowardEarthHed");
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AssaultCarrierLaunchesTowardEarthDetail", new object[] { arrival.ToCustomDateString() });
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnEnemyFleetLaunches);
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003C87 RID: 15495 RVA: 0x00170AF4 File Offset: 0x0016ECF4
		public static void LogFleetLaunchesTowardMyAsset(TISpaceFleetState fleet, ITransferTarget myAsset, List<TIFactionState> notifyFactions, TIDateTime arrival)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(notifyFactions);
			notificationQueueItem.relevantFactions.AddRange(notifyFactions);
			string displayName = myAsset.selfState().GetDisplayName(GameControl.control.activePlayer);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetLaunchesAtMeHed", new object[]
			{
				fleet.faction.adjectiveWithColor,
				displayName
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetLaunchesAtMeSummary", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(GameControl.control.activePlayer),
				displayName
			});
			double num = arrival.DifferenceInDays(TITimeState.Now());
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetLaunchesAtMeDetail", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(GameControl.control.activePlayer),
				displayName,
				(num >= 1.0) ? arrival.ToCustomDateString() : arrival.ToCustomTimeString()
			});
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnEnemyFleetLaunches);
			TINotificationQueueState.AddItem(notificationQueueItem, fleet.IsAlien());
		}

		// Token: 0x06003C88 RID: 15496 RVA: 0x00170C54 File Offset: 0x0016EE54
		public static void LogFleetEjectedFromStation(TISpaceFleetState fleet, TIHabState station)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.Add(fleet.faction);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.LogFleetEjectedFromStation.Hed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.LogFleetEjectedFromStation.Summary", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				station.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.LogFleetEjectedFromStation.Detail", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				station.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectHabNoCamera);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C89 RID: 15497 RVA: 0x00170D3C File Offset: 0x0016EF3C
		public static void LogFleetArrival(TISpaceFleetState fleet, TISpaceGameState origin, TISpaceGameState location, bool willResupply, bool willRepair, Dictionary<TIFactionState, string> factionSpecificText)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			if (location.isSpaceAssetState)
			{
				notificationQueueItem.primaryFactions.AddRangeUnique<TIFactionState>(location.ref_spaceAsset.ref_factions);
				notificationQueueItem.relevantFactions.AddRangeUnique<TIFactionState>(location.ref_spaceAsset.ref_factions);
			}
			if (location.ref_orbit != null && (origin == null || location.ref_orbit != origin.ref_orbit))
			{
				foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions().Except<TIFactionState>(notificationQueueItem.primaryFactions))
				{
					TINaturalSpaceObjectState ref_naturalSpaceObject = location.ref_naturalSpaceObject;
					int num = ((ref_naturalSpaceObject != null && ref_naturalSpaceObject.isEarth) ? tifactionState.defaultFleetArrivalAlert_Earth : tifactionState.defaultFleetArrivalAlert);
					int num2 = 0;
					if (fleet.IsAlien())
					{
						int num3 = num2;
						TINaturalSpaceObjectState ref_naturalSpaceObject2 = location.ref_naturalSpaceObject;
						num2 = num3 + ((ref_naturalSpaceObject2 != null && ref_naturalSpaceObject2.isEarth) ? tifactionState.defaultFleetArrivalAlienModifier_Earth : tifactionState.defaultFleetArrivalAlienModifier);
						num += num2;
					}
					if (num > 0)
					{
						TINaturalSpaceObjectState ref_naturalSpaceObject3 = location.ref_naturalSpaceObject;
						if (ref_naturalSpaceObject3 != null && ref_naturalSpaceObject3.isEarth)
						{
							TIOrbitState ref_orbit = location.ref_orbit;
							if (ref_orbit != null && ref_orbit.interfaceOrbit && fleet.InvasionFleet())
							{
								num2 += 3;
							}
						}
						int num4 = location.ref_orbit.OrbitInterestLevel(tifactionState) + num2;
						if (num4 >= 3)
						{
							notificationQueueItem.primaryFactions.AddUnique(tifactionState);
							notificationQueueItem.relevantFactions.AddUnique(tifactionState);
						}
						else if (num4 == 2)
						{
							notificationQueueItem.relevantFactions.AddUnique(tifactionState);
						}
					}
				}
			}
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.popupResource1 = fleet.iconResource;
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetArrivedHeadline", new object[]
			{
				fleet.faction.adjective,
				fleet.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetArrivedSummary", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				TIUtilities.GetLocationString(fleet.location, true, true)
			});
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.FleetArrivedDetail", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				TIUtilities.GetLocationString(fleet.location, true, true)
			}));
			if (willResupply && !willRepair)
			{
				stringBuilder.Append(Loc.T("UI.Notifications.FleetArrivedResupply"));
			}
			else if (willRepair)
			{
				stringBuilder.Append(Loc.T("UI.Notifications.FleetArrivedRepair"));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.factionSpecificDetail = new Dictionary<TIFactionState, string>(factionSpecificText);
			if (!willRepair && !willResupply)
			{
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C8A RID: 15498 RVA: 0x00171068 File Offset: 0x0016F268
		public static void LogTrajectoryTargetManeuveredAndWeCannotChase(TISpaceFleetState fleet, int cause, TISpaceFleetState targetFleet)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions = notificationQueueItem.primaryFactions;
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetTrajectoryAbortedHed", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetTrajectoryAbortedSummary", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
			string text;
			if (cause != 0)
			{
				if (cause != 1)
				{
					text = Loc.T("UI.Notifications.FleetTrajectoryAbortedDetail_TargetFleetChange", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
				}
				else
				{
					text = Loc.T("UI.Notifications.TargetManeuveredAndWeCannotGiveChaseDetail_InsufficientAccel", new object[]
					{
						fleet.GetDisplayName(TINotificationQueueState.activePlayer),
						targetFleet.GetDisplayName(TINotificationQueueState.activePlayer)
					});
				}
			}
			else
			{
				text = Loc.T("UI.Notifications.TargetManeuveredAndWeCannotGiveChaseDetail_InsufficientDV", new object[]
				{
					fleet.GetDisplayName(TINotificationQueueState.activePlayer),
					targetFleet.GetDisplayName(TINotificationQueueState.activePlayer)
				});
			}
			notificationQueueItem.itemDetail = text.ToString();
			notificationQueueItem.popupResource1 = fleet.iconResource;
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C8B RID: 15499 RVA: 0x001711B0 File Offset: 0x0016F3B0
		public static void LogTrajectoryAborted(TISpaceFleetState fleet, int cause, int outcome, TISpaceFleetState newFleet = null, TISpaceBodyState crashingInto = null)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions = notificationQueueItem.primaryFactions;
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetTrajectoryAbortedHed", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetTrajectoryAbortedSummary", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
			StringBuilder stringBuilder = new StringBuilder();
			TISpaceFleetState tispaceFleetState = fleet;
			switch (cause)
			{
			case 0:
				stringBuilder.AppendLine(Loc.T("UI.Notifications.FleetTrajectoryAbortedDetail_TargetFleetChange", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) })).AppendLine();
				tispaceFleetState = fleet;
				break;
			case 1:
				stringBuilder.AppendLine(Loc.T("UI.Notifications.FleetTrajectoryAbortedDetail_InsufficientDV", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) })).AppendLine();
				tispaceFleetState = fleet;
				break;
			case 2:
				stringBuilder.AppendLine(Loc.T("UI.Notifications.FleetTrajectoryAbortedDetail_SomeShipsInsufficientDV", new object[]
				{
					fleet.GetDisplayName(TINotificationQueueState.activePlayer),
					newFleet.GetDisplayName(TINotificationQueueState.activePlayer)
				})).AppendLine();
				tispaceFleetState = newFleet;
				break;
			case 3:
				stringBuilder.AppendLine(Loc.T("FleetTrajectoryAbortedDetail_NewBoss", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) }));
				break;
			}
			switch (outcome)
			{
			case 0:
			{
				StringBuilder stringBuilder2 = stringBuilder;
				string text = "UI.Notifications.FleetTrajectoryAbortedDetail_TempOrbit";
				object[] array = new object[1];
				int num = 0;
				object obj;
				if (!tispaceFleetState.transferAssigned)
				{
					obj = tispaceFleetState.ref_orbit.barycenter.displayName;
				}
				else
				{
					TISpaceGameState destination = tispaceFleetState.trajectory.destination;
					obj = ((destination != null) ? destination.barycenter.displayName : null) ?? "";
				}
				array[num] = obj;
				stringBuilder2.AppendLine(Loc.T(text, array));
				break;
			}
			case 1:
			{
				Trajectory trajectory = tispaceFleetState.trajectory;
				if (trajectory.nextTrajectory != null)
				{
					trajectory = trajectory.nextTrajectory;
				}
				TINaturalSpaceObjectState barycenter = trajectory.destinationOrbit.barycenter;
				stringBuilder.AppendLine(Loc.T("UI.Notifications.FleetTrajectoryAbortedDetail_TempOrbitWithBurn", new object[] { barycenter.displayName }));
				break;
			}
			case 2:
				stringBuilder.AppendLine(Loc.T("UI.Notifications.FleetTrajectoryAbortedDetail_LeavingSolarSystem", new object[] { tispaceFleetState.trajectory.GetBarycenterAtTime(tispaceFleetState.trajectory.arrivalTime).displayName }));
				break;
			case 3:
				if (newFleet == null)
				{
					Log.Error("LogTrajectoryAborted Cause 3 (some leaving solar system) passed with no doomed fleet", Array.Empty<object>());
				}
				else
				{
					stringBuilder.AppendLine(Loc.T("UI.Notifications.FleetTrajectoryAbortedDetail_SomeLeavingSolarSystem", new object[]
					{
						newFleet.GetDisplayName(TINotificationQueueState.activePlayer),
						newFleet.trajectory.GetBarycenterAtTime(tispaceFleetState.trajectory.arrivalTime).displayName
					}));
				}
				break;
			case 4:
			{
				if (fleet.ref_orbit == null)
				{
					Log.Error("UI.Notifications.FleetTrajectoryAbortedDetail_ReturnToOrbit: fleet's orbit was null." + (fleet.inTransfer ? "  Fleet was in a transfer at the time." : ""), Array.Empty<object>());
				}
				StringBuilder stringBuilder3 = stringBuilder;
				string text2 = "UI.Notifications.FleetTrajectoryAbortedDetail_ReturnToOrbit";
				object[] array2 = new object[2];
				array2[0] = fleet.GetDisplayName(TINotificationQueueState.activePlayer);
				int num2 = 1;
				TIOrbitState ref_orbit = fleet.ref_orbit;
				array2[num2] = ((ref_orbit != null) ? ref_orbit.displayName : null);
				stringBuilder3.AppendLine(Loc.T(text2, array2));
				break;
			}
			case 5:
				stringBuilder.AppendLine(Loc.T("UI.Notifications.FleetTrajectoryAbortedDetail_WillCrash", new object[] { crashingInto.displayName }));
				break;
			case 6:
				stringBuilder.AppendLine(Loc.T("UI.Notifications.FleetTrajectoryAbortedDetail_SomeWillCrash", new object[]
				{
					crashingInto.displayName,
					newFleet.GetDisplayName(GameControl.control.activePlayer)
				}));
				break;
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.popupResource1 = fleet.iconResource;
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C8C RID: 15500 RVA: 0x0017158C File Offset: 0x0016F78C
		public static void LogFleetCrashes(TISpaceFleetState fleet, TISpaceBodyState spaceBody)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.AddRange(TINotificationQueueState.AllFactionsWithIntel(fleet, TemplateManager.global.intelToSeeSpaceAssetLocationandComposition));
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetCrashesHed", new object[]
			{
				fleet.faction.adjectiveWithColor,
				spaceBody.displayName
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetCrashesSummary", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				spaceBody.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetCrashesDetail", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				spaceBody.displayName
			});
			notificationQueueItem.popupResource1 = fleet.iconResource;
			notificationQueueItem.gotoGameState = spaceBody;
			TINotificationQueueState.AddItem(notificationQueueItem, fleet.IsAlien());
		}

		// Token: 0x06003C8D RID: 15501 RVA: 0x001716A4 File Offset: 0x0016F8A4
		public static void LogFleetEscapesSolarSystem(TISpaceFleetState fleet)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.AddRange(TINotificationQueueState.AllFactionsWithIntel(fleet, TemplateManager.global.intelToSeeSpaceAssetLocationandComposition));
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetEscapesHed", new object[] { fleet.faction.adjectiveWithColor });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetEscapesSummary", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetEscapesDetail", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.popupResource1 = fleet.iconResource;
			TINotificationQueueState.AddItem(notificationQueueItem, fleet.IsAlien());
		}

		// Token: 0x06003C8E RID: 15502 RVA: 0x0017179C File Offset: 0x0016F99C
		public static void LogFleetLanded(TISpaceFleetState fleet)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			if (fleet.dockedAtHab && fleet.ref_hab.faction != fleet.faction)
			{
				notificationQueueItem.primaryFactions.Add(fleet.ref_hab.faction);
			}
			notificationQueueItem.relevantFactions = notificationQueueItem.primaryFactions;
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.popupResource1 = fleet.iconResource;
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetLandedHed", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				TIUtilities.GetLocationString(fleet.location, true, true)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetLandedDetail", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				TIUtilities.GetLocationString(fleet.location, true, true)
			});
			notificationQueueItem.illustrationResource = fleet.location.ref_habSite.template.backgroundPath;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C8F RID: 15503 RVA: 0x00171900 File Offset: 0x0016FB00
		public static void LogFleetsMerged(TISpaceFleetState fleet, string oldFleetDisplayName)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.Add(fleet.faction);
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetsMerged.Summary", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(GameControl.control.activePlayer),
				oldFleetDisplayName
			});
			notificationQueueItem.itemDetail = notificationQueueItem.itemSummary;
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			TINotificationQueueState.AddItem(notificationQueueItem, fleet.faction.IsAlienFaction);
		}

		// Token: 0x06003C90 RID: 15504 RVA: 0x001719B8 File Offset: 0x0016FBB8
		public static void LogNoSpaceBattleTakesPlace(TIFactionState attacker, TIFactionState defender, TISpaceObjectState location, string battleName)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(attacker);
			notificationQueueItem.primaryFactions.Add(defender);
			notificationQueueItem.relevantFactions.AddRange(TINotificationQueueState.AllFactions);
			notificationQueueItem.icon = TemplateManager.global.pathFleetCombatIcon;
			notificationQueueItem.popupResource1 = attacker.factionIcon256UIpath;
			notificationQueueItem.popupResource2 = defender.factionIcon256UIpath;
			notificationQueueItem.gotoGameState = location;
			notificationQueueItem.itemHeadline = (string.IsNullOrEmpty(battleName) ? Loc.T("UI.Notifications.SpaceBattleHed") : battleName);
			string text = (location.isSun ? Loc.T("UI.Notifications.SpaceBattleNoSoi") : null);
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.SpaceBattleNoBattleSummary", new object[]
			{
				attacker.displayNameWithColor,
				defender.displayNameWithColor,
				text ?? TIUtilities.GetLocationString(location, false, true)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.SpaceBattleNoBattleSummary", new object[]
			{
				attacker.displayNameWithColor,
				defender.displayNameWithColor,
				text ?? TIUtilities.GetLocationString(location, true, true)
			});
			TINotificationQueueState.AddItem(notificationQueueItem, attacker.IsAlienFaction || defender.IsAlienFaction);
		}

		// Token: 0x06003C91 RID: 15505 RVA: 0x00171AE4 File Offset: 0x0016FCE4
		public static void LogSpaceBattleTakesPlace(TIFactionState attacker, TIFactionState defender, TIFactionState winner, TIFactionState loser, TISpaceFleetState fleet1, TISpaceFleetState fleet2, TIHabState hab, TISpaceObjectState location, string battleName, int attackerLosses, int defenderLosses)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(attacker);
			notificationQueueItem.primaryFactions.Add(defender);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = TemplateManager.global.pathFleetCombatIcon;
			notificationQueueItem.popupResource1 = attacker.factionIcon256UIpath;
			notificationQueueItem.popupResource2 = defender.factionIcon256UIpath;
			notificationQueueItem.itemHeadline = (string.IsNullOrEmpty(battleName) ? Loc.T("UI.Notifications.SpaceBattleHed") : battleName);
			string text = (location.isSun ? Loc.T("UI.Notifications.SpaceBattleNoSoi") : (location.isNaturalSpaceObjectState ? Loc.T("UI.Notifications.SpaceBattleSoi", new object[] { location.displayName }) : null));
			string text2;
			if (winner != null)
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.SpaceBattleSummary", new object[]
				{
					winner.displayName,
					loser.displayName,
					text ?? TIUtilities.GetLocationString(location, false, true)
				});
				text2 = Loc.T("UI.Notifications.SpaceBattleDetail", new object[]
				{
					winner.displayName,
					loser.displayName,
					text ?? TIUtilities.GetLocationString(location, true, true)
				});
			}
			else
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.SpaceBattleDrawSummary", new object[]
				{
					attacker.displayName,
					defender.displayName,
					text ?? TIUtilities.GetLocationString(location, false, true)
				});
				text2 = Loc.T("UI.Notifications.SpaceBattleDrawDetail", new object[]
				{
					attacker.displayName,
					defender.displayName,
					text ?? TIUtilities.GetLocationString(location, true, true)
				});
			}
			if (attackerLosses == 1)
			{
				text2 = new StringBuilder(text2).Append(Loc.T("UI.Notifications.SpaceBattleDetailLosses_Single", new object[] { attacker.displayNameCapitalizedWithColor })).ToString();
			}
			else if (attackerLosses > 0)
			{
				text2 = new StringBuilder(text2).Append(Loc.T("UI.Notifications.SpaceBattleDetailLosses", new object[]
				{
					attacker.displayNameCapitalizedWithColor,
					attackerLosses.ToString("N0")
				})).ToString();
			}
			if (defenderLosses == 1)
			{
				text2 = new StringBuilder(text2).Append(Loc.T("UI.Notifications.SpaceBattleDetailLosses_Single", new object[] { defender.displayNameCapitalizedWithColor })).ToString();
			}
			else if (defenderLosses > 0)
			{
				text2 = new StringBuilder(text2).Append(Loc.T("UI.Notifications.SpaceBattleDetailLosses", new object[]
				{
					defender.displayNameCapitalizedWithColor,
					defenderLosses.ToString("N0")
				})).ToString();
			}
			notificationQueueItem.itemDetail = text2;
			if (fleet1 != null && !fleet1.deleted)
			{
				notificationQueueItem.gotoGameState = fleet1;
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			}
			else if (fleet2 != null && !fleet2.deleted)
			{
				notificationQueueItem.gotoGameState = fleet2;
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			}
			else if (hab != null && !hab.deleted)
			{
				notificationQueueItem.gotoGameState = hab;
			}
			else
			{
				notificationQueueItem.gotoGameState = location.ref_naturalSpaceObject;
			}
			TINotificationQueueState.AddItem(notificationQueueItem, attacker.IsAlienFaction || defender.IsAlienFaction);
		}

		// Token: 0x06003C92 RID: 15506 RVA: 0x00171E04 File Offset: 0x00170004
		public static void LogInitiateOrbitalBombardment(TISpaceFleetState fleet, TIGameState target)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.primaryFactions.AddRange(target.ref_factions);
			notificationQueueItem.relevantFactions = notificationQueueItem.primaryFactions;
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetInitiatesBombardmentHeadline");
			if (target.isArmyState || target is TIRegionEntityState)
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetInitiatesBombardmentSummary_2", new object[]
				{
					fleet.faction.adjectiveWithColor,
					fleet.GetDisplayName(TINotificationQueueState.activePlayer),
					target.GetDisplayName(TINotificationQueueState.activePlayer),
					target.ref_region.displayNameSentIn
				});
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetInitiatesBombardmentDetail_2", new object[]
				{
					fleet.faction.adjectiveWithColor,
					fleet.GetDisplayName(TINotificationQueueState.activePlayer),
					target.GetDisplayName(TINotificationQueueState.activePlayer),
					target.ref_region.displayNameSentIn
				});
			}
			else
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetInitiatesBombardmentSummary", new object[]
				{
					fleet.faction.adjectiveWithColor,
					fleet.GetDisplayName(TINotificationQueueState.activePlayer),
					TIUtilities.GetLocationString(target, true, false)
				});
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetInitiatesBombardmentDetail", new object[]
				{
					fleet.faction.adjectiveWithColor,
					fleet.GetDisplayName(TINotificationQueueState.activePlayer),
					TIUtilities.GetLocationString(target, true, true)
				});
			}
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			TINotificationQueueState.AddItem(notificationQueueItem, fleet.IsAlien());
		}

		// Token: 0x06003C93 RID: 15507 RVA: 0x00171FD8 File Offset: 0x001701D8
		public static void LogOrbitalBombardmentComplete(TISpaceFleetState fleet, TIGameState target, IOperation operation, TISpaceFleetState.EndBombardmentReason reason)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.AddRange(target.ref_factions);
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetCompletesBombardmentHeadline");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetCompletesBombardmentSummary", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.Notifications.FleetCompletesBombardmentDetail", new object[]
			{
				fleet.faction.adjectiveWithColor,
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				TIUtilities.GetStateDisplayName(target, TINotificationQueueState.activePlayer, false, false, true, false, true)
			}));
			if (TISpaceFleetState.ReportableEndBombardmentReasons.Contains(reason))
			{
				stringBuilder.AppendLine().AppendLine().AppendLine(Loc.T(new StringBuilder("UI.Notifications.FleetCompletesBombardmentDetail_").Append(reason.ToString()).ToString()));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.musicIntensityDelta = -0.225f;
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.operation.actor = fleet;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.RepeatOperation);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.RepeatOperationContinue);
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			IOperation operation2 = operation;
			if (operation == null)
			{
				operation2 = OperationsManager.fleetOperations.First<IOperation>((IOperation x) => x is BombardOperation);
			}
			notificationQueueItem2.operation.operationData = new OperationData(operation2, target, new TIDateTime(), new TIDateTime());
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C94 RID: 15508 RVA: 0x001721AC File Offset: 0x001703AC
		public static void LogAutoBombardementCancelled(TISpaceFleetState fleet, bool outOfAmmo)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.Add(fleet.faction);
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetAutobombardmentCancelled", new object[]
			{
				fleet.faction.adjective,
				fleet.GetDisplayName(GameControl.control.activePlayer)
			});
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Loc.T("UI.Notifications.FleetAutobombardmentCancelled", new object[]
			{
				fleet.faction.adjective,
				fleet.GetDisplayName(GameControl.control.activePlayer)
			}));
			if (outOfAmmo)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Notifications.FleetAutobombardmentCancelled_Detail"));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C95 RID: 15509 RVA: 0x001722AC File Offset: 0x001704AC
		public static void LogBombardingFleetEntersDangerZone(TISpaceFleetState fleet)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.Add(fleet.faction);
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.BombardingFleetEntersDangerZone.Hed", new object[] { fleet.GetDisplayName(fleet.faction) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.BombardingFleetEntersDangerZone.Summary", new object[] { fleet.GetDisplayName(fleet.faction) });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.BombardingFleetEntersDangerZone.Detail", new object[] { fleet.GetDisplayName(fleet.faction) });
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C96 RID: 15510 RVA: 0x00172384 File Offset: 0x00170584
		public static void LogShipDestroyedInStrat(TISpaceShipState ship, List<TIFactionState> destroyers, TIGameState location, Dictionary<TIFactionState, string> officerDeaths)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(ship.faction);
			notificationQueueItem.relevantFactions.Add(ship.faction);
			notificationQueueItem.icon = ship.fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ShipDestroyedStratHed", new object[] { ship.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ShipDestroyedStratSummary", new object[]
			{
				ship.faction.adjectiveWithColor,
				ship.displayName,
				TIUtilities.GetLocationString(location, false, true)
			});
			if (destroyers != null && destroyers.Count > 0)
			{
				notificationQueueItem.relevantFactions.AddRange(destroyers);
				NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
				string text = "UI.Notifications.ShipDestroyedStratDetail_1";
				object[] array = new object[4];
				array[0] = ship.faction.adjectiveWithColor;
				array[1] = ship.displayName;
				array[2] = TIUtilities.ConstructTextList(destroyers.ConvertAll<TIGameState>((TIFactionState x) => x.ref_gameState), false, false);
				array[3] = TIUtilities.GetLocationString(location, true, true);
				notificationQueueItem2.itemDetail = Loc.T(text, array);
			}
			else
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ShipDestroyedStratDetail_0", new object[]
				{
					ship.faction.adjectiveWithColor,
					ship.displayName,
					TIUtilities.GetLocationString(location, true, true)
				});
			}
			if (ship.fleet != null)
			{
				notificationQueueItem.gotoGameState = ship.fleet;
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
				notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			}
			else
			{
				notificationQueueItem.gotoGameState = location;
			}
			notificationQueueItem.factionSpecificDetail = officerDeaths;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C97 RID: 15511 RVA: 0x00172538 File Offset: 0x00170738
		public static void LogOurFleetRefueled(TISpaceFleetState fleet, bool interfleet)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.Add(fleet.faction);
			notificationQueueItem.icon = fleet.iconResource;
			if (!interfleet)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetRefueledHed", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
				if (!fleet.landed)
				{
					notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetRefueledSummaryOrbit", new object[]
					{
						fleet.GetDisplayName(TINotificationQueueState.activePlayer),
						fleet.location.GetDisplayName(TINotificationQueueState.activePlayer)
					});
					notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetRefueledSummaryOrbit", new object[]
					{
						fleet.GetDisplayName(TINotificationQueueState.activePlayer),
						fleet.location.GetDisplayName(TINotificationQueueState.activePlayer)
					});
					notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
				}
				else
				{
					notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetRefueledSummary", new object[]
					{
						fleet.GetDisplayName(TINotificationQueueState.activePlayer),
						fleet.location.GetDisplayName(TINotificationQueueState.activePlayer)
					});
					notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetRefueledSummary", new object[]
					{
						fleet.GetDisplayName(TINotificationQueueState.activePlayer),
						fleet.location.GetDisplayName(TINotificationQueueState.activePlayer)
					});
					notificationQueueItem.illustrationResource = fleet.dockedLocation.ref_habSite.template.backgroundPath;
				}
			}
			else
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetSelfRefuelHed", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetSelfRefuelSummary", new object[]
				{
					fleet.GetDisplayName(TINotificationQueueState.activePlayer),
					TIUtilities.GetLocationString(fleet.location, false, true)
				});
				notificationQueueItem.itemDetail = notificationQueueItem.itemSummary;
				if (fleet.landed)
				{
					notificationQueueItem.illustrationResource = fleet.dockedLocation.ref_habSite.template.backgroundPath;
				}
				else
				{
					notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
				}
			}
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_RefuelingComplete";
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C98 RID: 15512 RVA: 0x001727D0 File Offset: 0x001709D0
		public static void LogOurFleetRepaired(TISpaceFleetState fleet)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.Add(fleet.faction);
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetRepairedHed", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetRepairedSummary", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				fleet.location.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetRepairedSummary", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				fleet.location.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_RepairsComplete";
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C99 RID: 15513 RVA: 0x00172910 File Offset: 0x00170B10
		public static void LogFleetAvailableForOperations(TISpaceFleetState fleet)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.Add(fleet.faction);
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetAvailableHed", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetAvailableSummary", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				TIUtilities.GetLocationString(fleet.location, false, true)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetAvailableSummary", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				TIUtilities.GetLocationString(fleet.location, false, true)
			});
			notificationQueueItem.gotoGameState = fleet;
			if (fleet.dockedAtHab)
			{
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			}
			else
			{
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.FleetAvailable);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.FleetAvailable);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.FleetAvailable);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.FleetAvailable);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.FleetAvailable);
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C9A RID: 15514 RVA: 0x00172A98 File Offset: 0x00170C98
		public static void LogFleetStoleOurFuel(TISpaceFleetState fleet, TIHabState hab)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(hab.faction);
			notificationQueueItem.relevantFactions.Add(hab.faction);
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetStoleOurFuelHed", new object[] { fleet.faction.adjectiveWithColor });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetStoleOurFuelSummary", new object[]
			{
				fleet.faction.displayNameWithColor,
				fleet.ref_hab.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetStoleOurFuelSummary", new object[]
			{
				fleet.faction.displayNameWithColor,
				fleet.ref_hab.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.gotoGameState = hab;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C9B RID: 15515 RVA: 0x00172B84 File Offset: 0x00170D84
		public static void LogFleetUndocked(TISpaceFleetState fleet, TIHabState hab)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.relevantFactions.Add(fleet.faction);
			notificationQueueItem.icon = fleet.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FleetUndockedHed", new object[] { fleet.GetDisplayName(TINotificationQueueState.activePlayer) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FleetUndockedSummary", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				hab.GetDisplayName(TINotificationQueueState.activePlayer),
				fleet.ref_orbit.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FleetUndockedDetail", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				hab.GetDisplayName(TINotificationQueueState.activePlayer),
				fleet.ref_orbit.displayName
			});
			notificationQueueItem.gotoGameState = fleet;
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.operation.actor = fleet;
			notificationQueueItem.operation.operationData = new OperationData(OperationsManager.fleetOperations.First<IOperation>((IOperation x) => x.GetType() == typeof(UndockFromStationOperation)), fleet.orbitState, new TIDateTime(), new TIDateTime());
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SelectFleetNoCamera);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OnFleetArrival);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C9C RID: 15516 RVA: 0x00172D34 File Offset: 0x00170F34
		public static void Rapid_LogBombardmentShot(TISpaceFleetState fleet, TIGameState target, TIDateTime dateTime, string outcome)
		{
			List<TIFactionState> list = new List<TIFactionState> { fleet.faction };
			list.AddRangeUnique<TIFactionState>(target.ref_factions);
			list.RemoveAll((TIFactionState x) => x == null);
			TINotificationTemplate tinotificationTemplate = TemplateManager.Find<TINotificationTemplate>("Rapid_LogBombardmentShot", false);
			TINotificationQueueState.AddRapidItem(new NotificationSummaryItem(outcome, fleet.iconResource, string.Empty, Color.white, fleet, false, dateTime, tinotificationTemplate.dataName, new List<TIFactionState>(), new List<TIFactionState>(), list, TIMissionOutcome.None), list);
		}

		// Token: 0x06003C9D RID: 15517 RVA: 0x00172DC4 File Offset: 0x00170FC4
		public static void LogProbeLaunched(TIFactionState faction, TISpaceBodyState spaceBody)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = spaceBody.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ProbeLaunchedSummary", new object[] { faction.displayName, spaceBody.displayName });
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ProbeLaunchedHeadline");
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ProbeLaunchedDetail", new object[] { faction.displayName, spaceBody.displayName });
			notificationQueueItem.popupResource1 = faction.factionIcon256path;
			notificationQueueItem.popupResource2 = spaceBody.iconResource;
			notificationQueueItem.gotoGameState = spaceBody;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_probelaunched;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C9E RID: 15518 RVA: 0x00172E9C File Offset: 0x0017109C
		public static void LogProbeArrived(TIFactionState faction, TISpaceBodyState spaceBody)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.icon = spaceBody.iconResource;
			notificationQueueItem.popupResource1 = spaceBody.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ProbeArrivedSummary", new object[] { faction.displayName, spaceBody.displayName });
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ProbeArrivedHeadline");
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.ProbeArrivedDetail", new object[]
			{
				faction.displayName,
				spaceBody.displayName,
				spaceBody.maxHabTier.ToString()
			})).AppendLine().AppendLine();
			if (faction.AlienTerritoryToAvoid(spaceBody))
			{
				stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.Space.AlienTerritory"))).AppendLine();
			}
			foreach (TIHabSiteState tihabSiteState in spaceBody.habSites)
			{
				if (tihabSiteState.hasPlannedOrOperatingBase)
				{
					stringBuilder.Append(Loc.T("UI.Notifications.ProbeData_Hab", new object[]
					{
						tihabSiteState.displayName,
						tihabSiteState.hab.displayName,
						tihabSiteState.hab.coreFaction.displayNameWithColor,
						tihabSiteState.ProductivityString(true)
					})).AppendLine();
				}
				else
				{
					stringBuilder.Append(Loc.T("UI.Notifications.ProbeData_NoHab", new object[]
					{
						tihabSiteState.displayName,
						tihabSiteState.ProductivityString(true)
					})).AppendLine();
				}
			}
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SetBodyTagToRed);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.SetBodyTagToGreen);
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.gotoGameState = spaceBody;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003C9F RID: 15519 RVA: 0x0017306C File Offset: 0x0017126C
		public static void LogEnemyProbeLaunched(TIFactionState faction, TISpaceBodyState spaceBody)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(TINotificationQueueState.AllFactionsExcept(faction));
			notificationQueueItem.relevantFactions.AddRange(TINotificationQueueState.AllFactionsExcept(faction));
			notificationQueueItem.icon = spaceBody.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ProbeLaunchedSummary", new object[] { faction.displayName, spaceBody.displayName });
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ProbeLaunchedHeadline");
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ProbeLaunchedDetail", new object[] { faction.displayName, spaceBody.displayName });
			notificationQueueItem.popupResource1 = faction.factionIcon256path;
			notificationQueueItem.popupResource2 = spaceBody.iconResource;
			notificationQueueItem.gotoGameState = spaceBody;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_probelaunched;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CA0 RID: 15520 RVA: 0x00173150 File Offset: 0x00171350
		public static void LogEnemyProbeArrived(TIFactionState faction, TISpaceBodyState spaceBody)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(TINotificationQueueState.AllFactionsExcept(faction));
			notificationQueueItem.relevantFactions.AddRange(TINotificationQueueState.AllFactionsExcept(faction));
			notificationQueueItem.icon = spaceBody.iconResource;
			notificationQueueItem.popupResource1 = faction.factionIcon256path;
			notificationQueueItem.popupResource2 = spaceBody.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ProbeArrivedSummary", new object[] { faction.displayName, spaceBody.displayName });
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ProbeArrivedHeadline");
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ProbeArrivedDetail_Enemy", new object[] { faction.displayName, spaceBody.displayName });
			notificationQueueItem.gotoGameState = spaceBody;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CA1 RID: 15521 RVA: 0x00173224 File Offset: 0x00171424
		public static void LogScanningPlanet(TISpaceFleetState fleet, TISpaceBodyState spaceBody, float duration)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(fleet.faction);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.icon = spaceBody.iconResource;
			notificationQueueItem.popupResource1 = fleet.faction.factionIcon256path;
			notificationQueueItem.popupResource2 = spaceBody.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ScanningPlanetSummary", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				spaceBody.displayName
			});
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ScanningPlanetHeadline", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				spaceBody.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ScanningPlanetDetail", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				spaceBody.displayName,
				duration.ToString("N0")
			});
			notificationQueueItem.gotoGameState = spaceBody;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CA2 RID: 15522 RVA: 0x00173330 File Offset: 0x00171530
		public static void LogScannedPlanet(TISpaceFleetState fleet, TISpaceBodyState spaceBody)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.primaryFactions.Add(fleet.faction);
			notificationQueueItem.icon = spaceBody.iconResource;
			notificationQueueItem.popupResource1 = spaceBody.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ScannedPlanetSummary", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				spaceBody.displayName
			});
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ScannedPlanetHeadline", new object[] { spaceBody.displayName });
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.ScannedPlanetDetail", new object[]
			{
				fleet.GetDisplayName(TINotificationQueueState.activePlayer),
				spaceBody.displayName,
				spaceBody.maxHabTier.ToString()
			})).AppendLine().AppendLine();
			foreach (TIHabSiteState tihabSiteState in spaceBody.habSites)
			{
				if (tihabSiteState.hasPlannedOrOperatingBase)
				{
					stringBuilder.Append(Loc.T("UI.Notifications.ProbeData_Hab", new object[]
					{
						tihabSiteState.displayName,
						tihabSiteState.hab.displayName,
						tihabSiteState.hab.coreFaction.displayNameWithColor,
						tihabSiteState.ProductivityString(true)
					})).AppendLine();
				}
				else
				{
					stringBuilder.Append(Loc.T("UI.Notifications.DateLog", new object[]
					{
						tihabSiteState.displayName,
						tihabSiteState.ProductivityString(true)
					})).AppendLine();
				}
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.gotoGameState = spaceBody;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CA3 RID: 15523 RVA: 0x001734E8 File Offset: 0x001716E8
		public static void LogHabFounded(TIFactionState faction, TIHabState hab, TIGameState location)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HabFoundedHed", new object[] { Utilities.Capitalize(hab.description) });
			notificationQueueItem.popupResource1 = faction.factionIcon256path;
			if (faction != null)
			{
				faction.ProcessBuildHabAchievements(hab);
			}
			if (hab.IsStation)
			{
				notificationQueueItem.icon = TemplateManager.global.pathGeoscapeStation_gui;
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.StationFoundedSummary", new object[]
				{
					faction.displayNameCapitalized,
					hab.description,
					hab.displayName,
					location.ref_orbit.displayName
				});
				notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			}
			else
			{
				notificationQueueItem.icon = TemplateManager.global.pathGeoscapeBase_gui;
				notificationQueueItem.popupResource2 = location.ref_habSite.parentBody.iconResource;
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.BaseFoundedSummary", new object[]
				{
					faction.displayNameCapitalized,
					hab.description,
					hab.displayName,
					hab.habSite.displayName,
					hab.habSite.parentBody.displayName
				});
				notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			}
			notificationQueueItem.gotoGameState = hab;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenHabManager);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.HabTemplateSelection);
			TINotificationQueueState.AddItem(notificationQueueItem, hab.IsAlien());
		}

		// Token: 0x06003CA4 RID: 15524 RVA: 0x00173688 File Offset: 0x00171888
		public static void LogHabModuleComplete(TISectorState sector, TIHabModuleTemplate module, string altName = "")
		{
			NotificationQueueItem notificationQueueItem;
			if (altName == "")
			{
				notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			}
			else
			{
				notificationQueueItem = TINotificationQueueState.InitItem(altName);
			}
			notificationQueueItem.primaryFactions.Add(sector.faction);
			notificationQueueItem.relevantFactions.Add(sector.faction);
			notificationQueueItem.gotoGameState = sector.hab;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HabModuleComplete");
			if (sector.hab.IsBase)
			{
				notificationQueueItem.icon = module.baseIconResource;
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.BaseHabModuleCompleted", new object[]
				{
					module.displayName,
					sector.hab.displayName,
					sector.hab.habSite.displayName,
					sector.hab.habSite.parentBody.displayName
				});
				switch (module.tier)
				{
				case 1:
					notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_CompleteBuildBaseModuleT1";
					break;
				case 2:
					notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_CompleteBuildBaseModuleT2";
					break;
				case 3:
					notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_CompleteBuildBaseModuleT3";
					break;
				}
			}
			else
			{
				notificationQueueItem.icon = module.stationIconResource;
				notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.StationHabModuleCompleted", new object[]
				{
					module.displayName,
					sector.hab.displayName,
					sector.hab.orbitState.displayName,
					sector.hab.orbitState.barycenter.displayName
				});
				switch (module.tier)
				{
				case 1:
					notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_CompleteBuildStationModuleT1";
					break;
				case 2:
					notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_CompleteBuildStationModuleT2";
					break;
				case 3:
					notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_CompleteBuildStationModuleT3";
					break;
				}
			}
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenHabManager);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CA5 RID: 15525 RVA: 0x0017388C File Offset: 0x00171A8C
		public static void LogCriticalHabModuleComplete(TISectorState sector, TIHabModuleTemplate module)
		{
			TINotificationQueueState.LogHabModuleComplete(sector, module, MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06003CA6 RID: 15526 RVA: 0x001738A0 File Offset: 0x00171AA0
		public static void LogHabAcquired(TIHabState hab, TIFactionState capturingFaction, TIFactionState oldFaction, bool modulesDestroyed, bool assault, bool mission, Dictionary<TIFactionState, string> factionStrings)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(hab.ref_factions);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions.Where<TIFactionState>((TIFactionState x) => x != oldFaction).ToList<TIFactionState>();
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HabAcquiredHed", new object[]
			{
				capturingFaction.displayNameCapitalized,
				hab.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.HabAcquiredSummary", new object[]
			{
				capturingFaction.displayNameCapitalized,
				hab.GetDisplayName(TINotificationQueueState.activePlayer),
				oldFaction.adjectiveWithColor
			});
			StringBuilder stringBuilder = new StringBuilder();
			if (hab.IsBase)
			{
				if (assault && !mission)
				{
					notificationQueueItem.illustrationResource = TemplateManager.global.illus_assaultBase;
				}
				stringBuilder.Append(Loc.T("UI.Notifications.HabAcquiredDetail.Base", new object[]
				{
					hab.GetDisplayName(TINotificationQueueState.activePlayer),
					hab.habSite.displayName,
					hab.habSite.parentBody.displayName,
					oldFaction.displayNameWithColor,
					oldFaction.adjective
				}));
			}
			else
			{
				if (assault && !mission)
				{
					notificationQueueItem.illustrationResource = TemplateManager.global.illus_assaultBase;
				}
				else if (oldFaction != null && oldFaction == TINotificationQueueState.activePlayer)
				{
					oldFaction.UnlockAchievement("stationStolen");
				}
				else if (capturingFaction != null && capturingFaction == TINotificationQueueState.activePlayer)
				{
					capturingFaction.UnlockAchievement("stealStation");
				}
				stringBuilder.Append(Loc.T("UI.Notifications.HabAcquiredDetail.Station", new object[]
				{
					hab.GetDisplayName(TINotificationQueueState.activePlayer),
					hab.orbitState.displayName,
					oldFaction.displayNameWithColor,
					oldFaction.adjective
				}));
			}
			if (modulesDestroyed)
			{
				stringBuilder.Append(Loc.T("UI.Notifications.HabAcquiredDetail.ModulesDestroyed"));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.factionSpecificDetail = factionStrings;
			notificationQueueItem.gotoGameState = hab;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenHabManager);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.HabTemplateSelection);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CA7 RID: 15527 RVA: 0x00173B0C File Offset: 0x00171D0C
		public static void LogHabDefected(TIHabState hab, TIFactionState capturingFaction, TIFactionState oldFaction, bool modulesDestroyed)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(hab.ref_factions);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions.Where<TIFactionState>((TIFactionState x) => x != oldFaction).ToList<TIFactionState>();
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HabDefectedHed", new object[]
			{
				capturingFaction.displayNameCapitalized,
				hab.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.HabDefectedSummary", new object[]
			{
				capturingFaction.displayNameCapitalized,
				hab.GetDisplayName(TINotificationQueueState.activePlayer),
				oldFaction.adjectiveWithColor
			});
			StringBuilder stringBuilder = new StringBuilder();
			if (hab.IsBase)
			{
				stringBuilder.Append(Loc.T("UI.Notifications.HabDefectedDetail.Base", new object[]
				{
					hab.GetDisplayName(TINotificationQueueState.activePlayer),
					hab.habSite.displayName,
					hab.habSite.parentBody.displayName,
					oldFaction.displayNameWithColor,
					oldFaction.adjective
				}));
			}
			else
			{
				stringBuilder.Append(Loc.T("UI.Notifications.HabDefectedDetail.Station", new object[]
				{
					hab.GetDisplayName(TINotificationQueueState.activePlayer),
					hab.orbitState.displayName,
					oldFaction.displayNameWithColor,
					oldFaction.adjective
				}));
			}
			if (modulesDestroyed)
			{
				stringBuilder.Append(Loc.T("UI.Notifications.HabDefectedDetail.ModulesDestroyed"));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.gotoGameState = hab;
			if (oldFaction != null && oldFaction == TINotificationQueueState.activePlayer)
			{
				oldFaction.UnlockAchievement("stationStolen");
			}
			else if (capturingFaction != null && capturingFaction == TINotificationQueueState.activePlayer)
			{
				capturingFaction.UnlockAchievement("stealStation");
			}
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenHabManager);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.HabTemplateSelection);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CA8 RID: 15528 RVA: 0x00173D38 File Offset: 0x00171F38
		public static void LogHabAssaultFailed(TIGameState assaultingAsset, TIHabState hab, Dictionary<TIFactionState, string> bonusStrings, TIMissionOutcome outcome)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(assaultingAsset.ref_faction);
			notificationQueueItem.primaryFactions.Add(hab.ref_faction);
			notificationQueueItem.relevantFactions.AddRange(notificationQueueItem.primaryFactions);
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HabAssaultFailedHed", new object[]
			{
				assaultingAsset.ref_faction.adjective,
				hab.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.HabAssaultFailedSummary", new object[]
			{
				assaultingAsset.ref_faction.adjective,
				hab.GetDisplayName(TINotificationQueueState.activePlayer)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.HabAssaultFailedDetail", new object[]
			{
				assaultingAsset.ref_faction.adjectiveWithColor,
				assaultingAsset.GetDisplayName(TINotificationQueueState.activePlayer),
				hab.faction.adjectiveWithColor,
				hab.GetDisplayName(assaultingAsset.ref_faction)
			});
			notificationQueueItem.factionSpecificDetail = new Dictionary<TIFactionState, string>(bonusStrings);
			notificationQueueItem.gotoGameState = hab;
			if (assaultingAsset.isSpaceFleetState)
			{
				notificationQueueItem.operation.actor = assaultingAsset;
				notificationQueueItem.operation.operationData = new OperationData(OperationsManager.fleetOperations.First<IOperation>((IOperation x) => x.GetType() == typeof(AssaultHabOperation)), hab, new TIDateTime(), new TIDateTime());
				notificationQueueItem.outcome = outcome;
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CA9 RID: 15529 RVA: 0x00173EC0 File Offset: 0x001720C0
		public static void LogHabDefendInterestEnds(TIHabState hab)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(hab.ref_faction);
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.HabDefendInterestEndsSummary", new object[] { hab.displayName });
			notificationQueueItem.gotoGameState = hab;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CAA RID: 15530 RVA: 0x00173F28 File Offset: 0x00172128
		public static void LogHabDestroyed(TIHabState hab, TIFactionState destroyingFaction, TIFactionState oldFaction, TIResourcesCost recoveredResources, TIGameState destroyingFleet = null)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(destroyingFaction);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions.Where<TIFactionState>((TIFactionState x) => x != oldFaction).ToList<TIFactionState>();
			notificationQueueItem.icon = hab.iconResource;
			StringBuilder stringBuilder = new StringBuilder();
			if (hab.IsAlien() && hab.faction.primaryHab == hab)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HabDestroyedHed_AlienHome", new object[]
				{
					destroyingFaction.displayNameCapitalized,
					hab.GetDisplayName(TINotificationQueueState.activePlayer)
				});
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.HabDestroyedSummary_AlienHome", new object[]
				{
					destroyingFaction.displayNameWithColor,
					hab.GetDisplayName(TINotificationQueueState.activePlayer),
					oldFaction.adjectiveWithColor
				});
				stringBuilder.AppendLine(Loc.T("UI.Notifications.HabDestroyedDetail_AlienHome", new object[]
				{
					hab.GetDisplayName(TINotificationQueueState.activePlayer),
					hab.habSite.displayName,
					hab.habSite.parentBody.displayName,
					destroyingFaction.winningOrgTemplate.displayNameWithArticle
				}));
				notificationQueueItem.gotoGameState = hab;
			}
			else
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HabDestroyedHed", new object[]
				{
					destroyingFaction.displayNameCapitalized,
					hab.GetDisplayName(TINotificationQueueState.activePlayer)
				});
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.HabDestroyedSummary", new object[]
				{
					destroyingFaction.displayNameWithColor,
					hab.GetDisplayName(TINotificationQueueState.activePlayer),
					oldFaction.adjectiveWithColor
				});
				if (hab.IsBase)
				{
					stringBuilder.AppendLine(Loc.T("UI.Notifications.HabDestroyedDetail.Base", new object[]
					{
						oldFaction.adjectiveWithColor,
						hab.GetDisplayName(TINotificationQueueState.activePlayer),
						hab.habSite.displayName,
						hab.habSite.parentBody.displayName
					}));
					notificationQueueItem.gotoGameState = (TIGameState.Valid(destroyingFleet) ? destroyingFleet : hab.habSite);
				}
				else
				{
					stringBuilder.AppendLine(Loc.T("UI.Notifications.HabDestroyedDetail.Station", new object[]
					{
						oldFaction.adjectiveWithColor,
						hab.GetDisplayName(TINotificationQueueState.activePlayer),
						hab.orbitState.displayName
					}));
					notificationQueueItem.gotoGameState = (TIGameState.Valid(destroyingFleet) ? destroyingFleet : hab.orbitState);
				}
			}
			if (recoveredResources.anyDebit)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Notifications.HabDestroyedSalvage", new object[] { recoveredResources.GetString("Relevant", false, false, false, 7, false, false, destroyingFaction, false, FactionResource.None) }));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_AlienSpaceAlarm";
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CAB RID: 15531 RVA: 0x00174218 File Offset: 0x00172418
		public static void LogHabModuleForcedOffDueToPopulationChanges(TIFactionState faction, TINaturalSpaceObjectState state, List<TIHabState> factionHabs)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = TemplateManager.global.pathWarningIcon;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.HabModuleForcedOffDueToPopulationChanges.Summary", new object[] { state.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.HabModuleForcedOffDueToPopulationChanges.Detail", new object[]
			{
				state.displayName,
				TIUtilities.ConstructTextList(factionHabs.Select<TIHabState, string>((TIHabState x) => x.GetDisplayName(faction)).ToList<string>(), false, false),
				false,
				false
			});
			notificationQueueItem.gotoGameState = state;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CAC RID: 15532 RVA: 0x001742F4 File Offset: 0x001724F4
		public static void LogShipComplete(TISpaceShipState ship, TIHabState hab, bool isACancelledRefit = false)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(ship.fleet.faction);
			notificationQueueItem.relevantFactions.Add(ship.fleet.faction);
			notificationQueueItem.itemHeadline = ((!isACancelledRefit) ? Loc.T("UI.Notifications.ShipCompleteHeadline") : Loc.T("UI.Notifications.RefitAbortedHeadline"));
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ShipCompleteSummary", new object[]
			{
				ship.template.fullClassName,
				ship.displayName,
				hab.displayName
			});
			notificationQueueItem.itemDetail = ((!isACancelledRefit) ? Loc.T("UI.Notifications.ShipCompleteDetail", new object[]
			{
				ship.template.fullClassName,
				ship.displayName,
				hab.LocationName,
				ship.fleet.GetDisplayName(ship.fleet.faction),
				hab.displayName
			}) : Loc.T("UI.Notifications.RefitAbortedDetail", new object[]
			{
				ship.template.fullClassName,
				ship.displayName,
				hab.LocationName,
				ship.fleet.GetDisplayName(ship.fleet.faction),
				hab.displayName
			}));
			notificationQueueItem.icon = ship.fleet.iconResource;
			notificationQueueItem.popupResource1 = ship.fleet.iconResource;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_CompleteBuildShip";
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.JoinOrRallyToFleet);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.JoinOrRallyToFleet);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.JoinOrRallyToFleet);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.JoinOrRallyToFleet);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.JoinOrRallyToFleet);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.JoinOrRallyToFleet);
			notificationQueueItem.gotoGameState = ship.fleet;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CAD RID: 15533 RVA: 0x001744E4 File Offset: 0x001726E4
		public static void LogOfficerKilledOutsideofCombat(TIOfficerState officer)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(officer.ref_faction);
			notificationQueueItem.relevantFactions.Add(officer.ref_faction);
			notificationQueueItem.icon = officer.template.GetIconPath(officer.rank);
			notificationQueueItem.popupResource1 = officer.ref_faction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.OfficerKilledOutsideofCombat.Hed", new object[] { officer.ship.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.OfficerKilledOutsideofCombat.Summary", new object[]
			{
				officer.displayName,
				officer.template.displayName,
				officer.ship.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.OfficerKilledOutsideofCombat.Detail", new object[]
			{
				officer.displayName,
				officer.template.displayName,
				officer.ship.displayName
			});
			notificationQueueItem.gotoGameState = officer.ref_fleet;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CAE RID: 15534 RVA: 0x001745F8 File Offset: 0x001727F8
		public static void LogOfficerRetires(TIOfficerState officer)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(officer.ref_faction);
			notificationQueueItem.relevantFactions.Add(officer.ref_faction);
			notificationQueueItem.icon = officer.template.GetIconPath(officer.rank);
			notificationQueueItem.popupResource1 = officer.ref_faction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.OfficerRetires.Hed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.OfficerRetires.Summary", new object[]
			{
				officer.displayName,
				officer.template.displayName,
				officer.ship.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.OfficerRetires.Detail", new object[]
			{
				officer.displayName,
				officer.template.displayName,
				officer.ship.displayName
			});
			notificationQueueItem.gotoGameState = officer.ref_fleet;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CAF RID: 15535 RVA: 0x001746F8 File Offset: 0x001728F8
		public static void LogObjectiveComplete(TIFactionState faction, TIObjectiveTemplate finishedTemplate, List<TIObjectiveTemplate> newlyUnlockedObjectives)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = faction.factionIcon64path;
			notificationQueueItem.popupResource1 = faction.pathLeaderIcon;
			notificationQueueItem.popupResource2 = faction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ObjectiveComplete.Hed");
			notificationQueueItem.itemSummary = Loc.T("UI.Objectives.Summary", new object[] { finishedTemplate.displayName(faction) });
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Objectives.Summary", new object[] { finishedTemplate.displayName(faction) })).Append("\n\n").Append(finishedTemplate.resolution(faction));
			if (finishedTemplate.grantsResources)
			{
				stringBuilder.AppendLine().AppendLine().AppendLine(Loc.T("TIObjectiveTemplate.ResourcesGain", new object[] { TIUtilities.BuildResourceValueString(finishedTemplate.resourcesGranted) }));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.gotoGameState = GameStateManager.MissionPhase();
			notificationQueueItem.illustrationResource = finishedTemplate.completedIllustrationResource;
			if (faction == GameControl.control.activePlayer && (finishedTemplate.dataName.ToLowerInvariant().Contains("investigate") || finishedTemplate.dataName.ToLowerInvariant().Contains("interrogation")))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_Special_SFX/trig_SFX_Interrogation_Negotiation", false, false);
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
			foreach (TIObjectiveTemplate tiobjectiveTemplate in newlyUnlockedObjectives)
			{
				if (!tiobjectiveTemplate.isChildObjective && faction.GetObjectivesByStatus(ObjectiveStatus.Unlocked).Contains(tiobjectiveTemplate))
				{
					TINotificationQueueState.LogObjectiveUnlocked(faction, tiobjectiveTemplate);
				}
			}
			notificationQueueItem.fanfareToPlay = faction.template.fanfarePath;
			notificationQueueItem.soundToPlay = faction.GetObjectiveCompletedVoicePath(finishedTemplate);
			notificationQueueItem.triggerEndGame = faction == GameControl.control.activePlayer && finishedTemplate.objectiveType == ObjectiveType.Victory && faction.victoryTemplate.victoryEffect > TIVictoryTemplate.VictoryEffectType.none;
		}

		// Token: 0x06003CB0 RID: 15536 RVA: 0x0017490C File Offset: 0x00172B0C
		public static void LogMilestoneComplete(TIFactionState faction, CampaignMilestone milestone)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = faction.factionIcon64path;
			notificationQueueItem.popupResource1 = faction.pathLeaderIcon;
			notificationQueueItem.popupResource2 = faction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T(new StringBuilder("TIObjectiveTemplate.MilestoneFulfilled.").Append(milestone.ToString()).ToString());
			notificationQueueItem.itemSummary = Loc.T(new StringBuilder("TIObjectiveTemplate.MilestoneFulfilled.Summary.").Append(milestone.ToString()).ToString());
			notificationQueueItem.itemDetail = Loc.T(new StringBuilder("TIObjectiveTemplate.MilestoneFulfilled.Detail.").Append(milestone.ToString()).ToString());
			switch (milestone)
			{
			case CampaignMilestone.AccessAlienTech:
				notificationQueueItem.illustrationResource = "illustrations/Milestone_AlienTechRecovered";
				break;
			case CampaignMilestone.AccessAlienShip:
				notificationQueueItem.illustrationResource = "illustrations/Milestone_AccessAlienShip";
				break;
			case CampaignMilestone.AccessGriffinCorpus:
				notificationQueueItem.illustrationResource = "illustrations/Milestone_GriffinCorpseRecovered";
				break;
			case CampaignMilestone.AccessLiveGriffin:
				notificationQueueItem.illustrationResource = "illustrations/Milestone_LiveGriffinCaptured";
				break;
			case CampaignMilestone.AccessSalamanderCorpus:
				notificationQueueItem.illustrationResource = "illustrations/Milestone_SalamanderCorpseRecovered";
				break;
			case CampaignMilestone.AccessLiveSalamander:
				notificationQueueItem.illustrationResource = "illustrations/Milestone_LiveSalamanderCaptured";
				break;
			case CampaignMilestone.AccessWarDogCorpus:
				notificationQueueItem.illustrationResource = "illustrations/Milestone_WarDogRecovered";
				break;
			case CampaignMilestone.AliensBombardEarth:
				notificationQueueItem.illustrationResource = "illustrations/Milestone_AlienBombardment";
				break;
			case CampaignMilestone.AliensAttackInSpace:
				notificationQueueItem.illustrationResource = "illustrations/Milestone_AlienAttackHumanTarget";
				break;
			}
			notificationQueueItem.gotoGameState = GameStateManager.MissionPhase();
			notificationQueueItem.musicIntensityDelta = 0.15f;
			notificationQueueItem.fanfareToPlay = faction.template.fanfarePath;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CB1 RID: 15537 RVA: 0x00174AD0 File Offset: 0x00172CD0
		public static void LogGlobalMilestoneComplete(TIFactionState faction, GlobalMilestone milestone, TIGameState winningObject, List<ResourceValue> reward)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.AddRange(GameStateManager.AllHumanFactions());
			notificationQueueItem.icon = faction.factionIcon64path;
			notificationQueueItem.popupResource1 = faction.factionIcon256path;
			notificationQueueItem.popupResource2 = faction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T(new StringBuilder("UI.Notifications.GlobalMilestone.Hed.").Append(milestone.ToString()).ToString());
			notificationQueueItem.itemSummary = Loc.T(new StringBuilder("UI.Notifications.GlobalMilestone.Summary.").Append(milestone.ToString()).ToString(), new object[] { faction.displayNameWithColor });
			StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder("UI.Notifications.GlobalMilestone.Detail.").Append(milestone.ToString()).ToString(), new object[]
			{
				faction.displayNameWithColor,
				winningObject.GetDisplayName(GameControl.control.activePlayer)
			}));
			if (reward.Count > 0)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (ResourceValue resourceValue in reward)
				{
					stringBuilder2.Append(resourceValue.ToString());
				}
				stringBuilder.AppendLine().AppendLine().Append(Loc.T("UI.Notifications.GlobalMilestone.Reward", new object[] { stringBuilder2 }));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.gotoGameState = winningObject;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CB2 RID: 15538 RVA: 0x00174C80 File Offset: 0x00172E80
		public static void LogObjectiveUnlocked(TIFactionState faction, TIObjectiveTemplate unlockedObjective)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = faction.factionIcon64path;
			notificationQueueItem.popupResource1 = faction.pathLeaderIcon;
			notificationQueueItem.popupResource2 = faction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.NewObjective.Hed", new object[] { unlockedObjective.displayName(faction) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NewObjective.Summary", new object[] { unlockedObjective.displayName(faction) });
			notificationQueueItem.itemDetail = unlockedObjective.fullDescription(faction, false);
			notificationQueueItem.gotoGameState = GameStateManager.MissionPhase();
			if (TIGlobalValuesState.GlobalValues.tutorialMode && faction.isActivePlayer)
			{
				notificationQueueItem.soundToPlay = new StringBuilder("event:/VO/ENG/Faction/Tutorial_").Append(unlockedObjective.dataName).ToString();
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CB3 RID: 15539 RVA: 0x00174D70 File Offset: 0x00172F70
		public static void LogOtherFactionUnlocksVictoryCondition(TIFactionState unlockingFaction)
		{
			foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions())
			{
				if (tifactionState != unlockingFaction)
				{
					tifactionState.SetIntelIfValueHigher(unlockingFaction, TemplateManager.global.intelToSeeFactionBasicData, null);
				}
			}
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(TINotificationQueueState.AllFactionsExcept(unlockingFaction));
			notificationQueueItem.relevantFactions.AddRange(notificationQueueItem.primaryFactions);
			notificationQueueItem.icon = unlockingFaction.factionIcon64path;
			notificationQueueItem.popupResource1 = unlockingFaction.factionIcon256path;
			notificationQueueItem.popupResource2 = unlockingFaction.factionIcon256path;
			if (TIPlayerProfileManager.useCouncilorVideo)
			{
				notificationQueueItem.videoResource = unlockingFaction.pathLeaderTorsoVideo;
			}
			else
			{
				notificationQueueItem.showSideArt = true;
				notificationQueueItem.illustrationResource = unlockingFaction.pathLeaderTorsoPortration;
			}
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.VictoryAnnounceHed", new object[]
			{
				unlockingFaction.displayNameCapitalized,
				unlockingFaction.template.victory
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.VictoryAnnounceSummary", new object[] { unlockingFaction.displayNameWithColor });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.VictoryAnnounceDetail", new object[]
			{
				unlockingFaction.displayNameWithColor,
				unlockingFaction.template.victoryAnnouncement
			});
			notificationQueueItem.gotoGameState = unlockingFaction;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.soundToPlay = new StringBuilder("event:/VO/ENG/Faction/Faction_Announcement_").Append(unlockingFaction.ideology.ideology).ToString();
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CB4 RID: 15540 RVA: 0x00174EEC File Offset: 0x001730EC
		public static void LogFirstFactionEncounter(TIFactionState detectingFaction, TIFactionState encounteredFaction, TIGameState encounterSource, TIGameState location)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			string text = new StringBuilder(notificationQueueItem.templateName).Append(encounteredFaction.templateName).ToString();
			if (detectingFaction != encounteredFaction && TINotificationQueueState.FirstNotificationOfType(detectingFaction, text))
			{
				notificationQueueItem.primaryFactions.Add(detectingFaction);
				notificationQueueItem.relevantFactions.Add(detectingFaction);
				notificationQueueItem.icon = encounteredFaction.factionIcon64path;
				notificationQueueItem.popupResource1 = encounteredFaction.factionIcon256path;
				notificationQueueItem.popupResource2 = encounteredFaction.factionIcon256path;
				if (TIPlayerProfileManager.useCouncilorVideo)
				{
					notificationQueueItem.videoResource = encounteredFaction.pathLeaderTorsoVideo;
				}
				else
				{
					notificationQueueItem.showSideArt = true;
					notificationQueueItem.illustrationResource = (encounteredFaction.IsAlienFaction ? encounteredFaction.pathLeaderHeadPortrait : encounteredFaction.pathLeaderTorsoPortration);
				}
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HedFirstFactionEncounter", new object[] { encounteredFaction.displayNameWithColor });
				notificationQueueItem.itemSummary = Loc.T("TIFactionTemplate.Introduction.Summary", new object[] { encounteredFaction.displayNameWithColor });
				notificationQueueItem.itemDetail = encounteredFaction.introduction;
				notificationQueueItem.gotoGameState = ((location == null || !location.hasMapObject) ? encounterSource : location);
				notificationQueueItem.musicIntensityDelta = (encounteredFaction.IsAlienFaction ? 0.9f : 0.1f);
				notificationQueueItem.soundToPlay = new StringBuilder("event:/VO/ENG/Faction/Faction_LeaderIntro_").Append(encounteredFaction.ideology.ideology).ToString();
				notificationQueueItem.OnOpenNotification = delegate
				{
					Mood.BeginFactionEncounter(encounteredFaction);
				};
				notificationQueueItem.OnCloseNotification = delegate
				{
					Mood.EndFactionEncounter();
				};
				TINotificationQueueState.SetFirstNotificationofType(detectingFaction, text);
				TINotificationQueueState.AddItem(notificationQueueItem, encounteredFaction.IsAlienFaction);
			}
		}

		// Token: 0x06003CB5 RID: 15541 RVA: 0x001750FC File Offset: 0x001732FC
		public static void LogFactionDefeated(TIFactionState deadFaction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactionsExcept(deadFaction);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactionsExcept(deadFaction);
			notificationQueueItem.icon = deadFaction.factionIcon64path;
			notificationQueueItem.popupResource1 = deadFaction.factionIcon256path;
			notificationQueueItem.popupResource2 = deadFaction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FactionDisabled.Hed", new object[] { deadFaction.displayNameWithColor });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FactionDisabled.Summary", new object[] { deadFaction.displayNameWithColor });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FactionDisabled.Detail", new object[] { deadFaction.displayNameWithColor });
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CB6 RID: 15542 RVA: 0x001751BC File Offset: 0x001733BC
		public static void LogAlienCrashdown(TIRegionState region, bool firstCrashdown)
		{
			if (firstCrashdown)
			{
				TINotificationQueueState.LogPrecrashCampaignStart();
			}
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = GameStateManager.AllHumanFactions().ToList<TIFactionState>();
			notificationQueueItem.primaryFactions = GameStateManager.AllHumanFactions().ToList<TIFactionState>();
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeCrashdown_gui;
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeCrashdown_gui;
			notificationQueueItem.popupResource2 = region.nation.flagResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienCrashdownSummary", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle,
				region.nation.displayNameWithArticleAndPlacePrep
			});
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienCrashdownHeadline");
			notificationQueueItem.itemDetail = Loc.T_Scenario("UI.Notifications.AlienCrashdownDetail" + (firstCrashdown ? ".First" : ""), new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle,
				region.nation.displayNameWithArticleAndPlacePrep
			});
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_alienCrashdown;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_SFX/trig_SFX_AlienEarthAlarm";
			notificationQueueItem.gotoGameState = region.alienCrashdown;
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CB7 RID: 15543 RVA: 0x00175308 File Offset: 0x00173508
		public static void LogUFOLanding(TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = GameStateManager.AllHumanFactions().ToList<TIFactionState>();
			notificationQueueItem.primaryFactions = GameStateManager.AllHumanFactions().ToList<TIFactionState>();
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeUFOLanding_gui;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.UFOLandingSummary", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle,
				region.nation.displayNameWithArticleAndPlacePrep
			});
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.UFOLandingHeadline");
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.UFOLandingDetail", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle,
				TemplateManager.global.daysToFieldArmyFromUFO,
				region.nation.displayNameWithArticleAndPlacePrep
			});
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_landedUFO;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = region.alienLanding;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Alien_Sighted";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CB8 RID: 15544 RVA: 0x00175428 File Offset: 0x00173628
		public static void LogUFOLandingAssaulted(TIGameState assaultingState, TIFactionState faction, TIRegionUFOLandingState UFO)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = GameStateManager.AllFactions().ToList<TIFactionState>();
			notificationQueueItem.primaryFactions = GameStateManager.AllFactions().ToList<TIFactionState>();
			if (assaultingState.isCouncilorState)
			{
				notificationQueueItem.primaryFactions.Remove(faction);
			}
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeUFOLanding_gui;
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			string text = "UI.Notifications.UFOLandingAssaultHeadline";
			object[] array = new object[1];
			int num = 0;
			string text2;
			if ((text2 = ((faction != null) ? faction.displayNameCapitalized : null)) == null)
			{
				TINationState ref_nation = assaultingState.ref_nation;
				text2 = ((ref_nation != null) ? ref_nation.displayNameWithArticle : null) ?? Loc.T("UI.Notifications.NoFaction");
			}
			array[num] = text2;
			notificationQueueItem2.itemHeadline = Loc.T(text, array);
			NotificationQueueItem notificationQueueItem3 = notificationQueueItem;
			string text3 = "UI.Notifications.UFOLandingAssaultSummary";
			object[] array2 = new object[2];
			int num2 = 0;
			string text4;
			if ((text4 = ((faction != null) ? faction.displayNameWithColor : null)) == null)
			{
				TINationState ref_nation2 = assaultingState.ref_nation;
				text4 = ((ref_nation2 != null) ? ref_nation2.displayNameWithArticle : null) ?? Loc.T("UI.Notifications.NoFaction");
			}
			array2[num2] = text4;
			array2[1] = UFO.region.displayName;
			notificationQueueItem3.itemSummary = Loc.T(text3, array2);
			NotificationQueueItem notificationQueueItem4 = notificationQueueItem;
			string text5 = "UI.Notifications.UFOLandingAssaultDetail";
			object[] array3 = new object[4];
			int num3 = 0;
			string text6;
			if ((text6 = ((faction != null) ? faction.displayNameWithColor : null)) == null)
			{
				TINationState ref_nation3 = assaultingState.ref_nation;
				text6 = ((ref_nation3 != null) ? ref_nation3.displayNameWithArticle : null) ?? Loc.T("UI.Notifications.NoFaction");
			}
			array3[num3] = text6;
			array3[1] = UFO.region.displayName;
			array3[2] = UFO.region.nation.displayNameWithArticle;
			array3[3] = UFO.region.nation.displayNameWithArticleAndPlacePrep;
			notificationQueueItem4.itemDetail = Loc.T(text5, array3);
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_alienLandedUFOBombed;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = UFO.region;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Small_Unit_Combat";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CB9 RID: 15545 RVA: 0x001755E0 File Offset: 0x001737E0
		public static void LogUFOLandingBombed(TISpaceFleetState fleet, TIRegionUFOLandingState UFO)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = GameStateManager.AllFactions().ToList<TIFactionState>();
			notificationQueueItem.primaryFactions = GameStateManager.AllFactions().ToList<TIFactionState>();
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeUFOLanding_gui;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.UFOLandingBombedHeadline");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.UFOLandingBombedSummary", new object[]
			{
				fleet.faction.displayNameWithColor,
				UFO.region.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.UFOLandingBombedDetail", new object[]
			{
				fleet.faction.displayNameWithColor,
				UFO.region.displayName,
				UFO.region.nation.displayNameWithArticle
			});
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_alienLandedUFOBombed;
			notificationQueueItem.gotoGameState = UFO.region;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Explosion_Air_Breach";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CBA RID: 15546 RVA: 0x001756F0 File Offset: 0x001738F0
		public static void LogAlienArmySpawned(TIAlienArmyState alienArmy)
		{
			NotificationQueueItem item = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			item.relevantFactions = GameStateManager.AllHumanFactions().ToList<TIFactionState>();
			item.icon = alienArmy.GetIconForegroundResource;
			item.iconBackgroundResource = alienArmy.GetIconBackgroundResource;
			item.backgroundColor = alienArmy.GetIconBackgroundResourceColor;
			item.itemSummary = Loc.T("UI.Notifications.AlienArmySpawnsSummary", new object[] { alienArmy.currentRegion.displayName });
			item.itemHeadline = Loc.T("UI.Notifications.AlienArmySpawnsHeadline");
			if (item.relevantFactions.Any<TIFactionState>((TIFactionState x) => TINotificationQueueState.FirstNotificationOfType(x, item.templateName)))
			{
				item.itemDetail = Loc.T("UI.Notifications.AlienArmySpawnsFirstDetail", new object[] { alienArmy.currentRegion.displayName });
				item.primaryFactions = GameStateManager.AllHumanFactions().ToList<TIFactionState>();
			}
			else
			{
				item.itemDetail = Loc.T("UI.Notifications.AlienArmySpawnsDetail", new object[] { alienArmy.currentRegion.displayName });
				item.primaryFactions = alienArmy.currentRegion.ref_factions;
			}
			item.musicIntensityDelta = 0.45f;
			item.illustrationResource = TemplateManager.global.illus_alienArmy;
			item.gotoGameState = alienArmy;
			item.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Aliens_Sighted_Earth";
			item.fanfareToPlay = "event:/Music/Fanfares/trig_Aliens_Action";
			TINotificationQueueState.AddItem(item, true);
		}

		// Token: 0x06003CBB RID: 15547 RVA: 0x00175898 File Offset: 0x00173A98
		public static void LogAbductions(TIFactionState detectingFaction, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeAbductions_gui;
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeAbductions_gui;
			notificationQueueItem.popupResource2 = region.nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AbductionsHeadline");
			notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.AbductionsDetail", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle
			})).ToString();
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_abductions;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AbductionsSummary", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle
			});
			notificationQueueItem.gotoGameState = region.alienActivity;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Alien_Mission";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CBC RID: 15548 RVA: 0x001759AC File Offset: 0x00173BAC
		public static void LogEnthrallElites(TIFactionState detectingFaction, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeEnthrallElites_gui;
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeEnthrallElites_gui;
			notificationQueueItem.popupResource2 = region.nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.EnthrallElitesHeadline");
			notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.EnthrallElitesDetail", new object[]
			{
				region.nation.displayNameWithArticle,
				GameStateManager.AlienProxy().displayName,
				TIFactionState.purgeMission.displayName,
				TIUtilities.InlineAttributeStr(CouncilorAttribute.Science),
				TIUtilities.GetAttributeString(CouncilorAttribute.Science)
			})).ToString();
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_enthrallElites;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.EnthrallElitesSummary", new object[] { region.nation.displayNameWithArticle });
			notificationQueueItem.gotoGameState = region.alienActivity;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Alien_Enthrall";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CBD RID: 15549 RVA: 0x00175ADC File Offset: 0x00173CDC
		public static void LogEnthrallOrg(TIFactionState detectingFaction, TIRegionState region, TIOrgState orgTarget, TICouncilorState councilorTarget, TIFactionState factionTarget)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeEnthrallElites_gui;
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeEnthrallElites_gui;
			notificationQueueItem.popupResource2 = region.nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.EnthrallOrgHeadline");
			if (councilorTarget != null)
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.EnthrallOrgSummary_councilor", new object[] { councilorTarget.displayName, orgTarget.displayNameWithArticle });
				notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.EnthrallOrgDetail_councilor", new object[] { councilorTarget.displayName, orgTarget.displayNameWithArticle })).ToString();
			}
			else
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.EnthrallOrgSummary", new object[] { orgTarget.displayNameWithArticle, factionTarget.displayNameWithColor });
				notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.EnthrallOrgDetail", new object[] { orgTarget.displayNameWithArticle, factionTarget.displayNameWithColor })).ToString();
			}
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_enthrallElites;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = region.alienActivity;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Alien_Enthrall";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CBE RID: 15550 RVA: 0x00175C4C File Offset: 0x00173E4C
		public static void LogEnthrallPublic(TIFactionState detectingFaction, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeEnthrallPublic_gui;
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeEnthrallPublic_gui;
			notificationQueueItem.popupResource2 = region.nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.EnthrallPublicHeadline");
			notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.EnthrallPublicDetail", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle,
				GameStateManager.AlienProxy().displayName
			})).ToString();
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.EnthrallPublicSummary", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle
			});
			notificationQueueItem.gotoGameState = region.alienActivity;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Alien_Enthrall";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CBF RID: 15551 RVA: 0x00175D5C File Offset: 0x00173F5C
		public static void LogTerrorize(TIFactionState detectingFaction, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeTerrorize_gui;
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeTerrorize_gui;
			notificationQueueItem.popupResource2 = region.nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.TerrorizeHeadline");
			notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.TerrorizeDetail", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle
			})).ToString();
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.TerrorizeSummary", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle
			});
			notificationQueueItem.gotoGameState = region.alienActivity;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_terrorize;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Aliens_Terrorize_City";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC0 RID: 15552 RVA: 0x00175E70 File Offset: 0x00174070
		public static void LogXenoformMission(TIFactionState detectingFaction, TIRegionState region, bool forcePopup = false)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeXenoform_gui;
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeXenoform_gui;
			notificationQueueItem.popupResource2 = region.nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.XenoformMissionHed");
			notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.XenoformMissionDetail", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle
			})).ToString();
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.XenoformMissionSummary", new object[]
			{
				region.displayName,
				region.nation.displayNameWithArticle
			});
			notificationQueueItem.gotoGameState = region.xenoforming;
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC1 RID: 15553 RVA: 0x00175F60 File Offset: 0x00174160
		public static void LogAlienFacilityDetected(TIFactionState detectingFaction, TIRegionAlienFacilityState alienFacility)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeAlienFacility_gui;
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeAlienFacility_gui;
			notificationQueueItem.popupResource2 = alienFacility.region.nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienFacilityHed", new object[] { alienFacility.region.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienFacilitySummary", new object[]
			{
				alienFacility.region.displayName,
				alienFacility.region.nation.displayNameWithArticle
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AlienFacilityDetail", new object[]
			{
				alienFacility.region.displayName,
				alienFacility.region.nation.displayNameWithArticle
			});
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_alienFacility;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = alienFacility;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Aliens_Sighted_Earth";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC2 RID: 15554 RVA: 0x00176094 File Offset: 0x00174294
		public static void LogAlienFacilityAssaulted(TIGameState assaultingState, TIFactionState faction, TIRegionAlienFacilityState facility, float exoticsGained, int abductionsEliminated)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeAlienFacility_gui;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienFacilityAssaultHeadline");
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			string text = "UI.Notifications.AlienFacilityAssaultSummary";
			object[] array = new object[2];
			int num = 0;
			string text2;
			if ((text2 = ((faction != null) ? faction.displayNameWithColor : null)) == null)
			{
				TINationState ref_nation = assaultingState.ref_nation;
				text2 = ((ref_nation != null) ? ref_nation.displayNameWithArticle : null) ?? Loc.T("UI.Notifications.NoFaction");
			}
			array[num] = text2;
			array[1] = facility.region.displayName;
			notificationQueueItem2.itemSummary = Loc.T(text, array);
			string text3 = "UI.Notifications.AlienFacilityAssaultDetail";
			object[] array2 = new object[5];
			int num2 = 0;
			string text4;
			if ((text4 = ((faction != null) ? faction.displayNameWithColor : null)) == null)
			{
				TINationState ref_nation2 = assaultingState.ref_nation;
				text4 = ((ref_nation2 != null) ? ref_nation2.displayNameWithArticle : null) ?? Loc.T("UI.Notifications.NoFaction");
			}
			array2[num2] = text4;
			array2[1] = facility.region.displayName;
			array2[2] = facility.region.nation.displayNameWithArticle;
			array2[3] = TemplateManager.global.exoticsInlineSpritePath;
			array2[4] = exoticsGained.ToString("N0");
			StringBuilder stringBuilder = new StringBuilder(Loc.T(text3, array2));
			if (facility.region.abductions > 0)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Notifications.AlienFacilityAssaultDetail2", new object[] { abductionsEliminated }));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_assaultXenoFacility;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = facility.region;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Small_Unit_Combat";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC3 RID: 15555 RVA: 0x00176240 File Offset: 0x00174440
		public static void LogAlienFacilityBombed(TISpaceFleetState fleet, TIRegionAlienFacilityState facility)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeAlienFacility_gui;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienFacilityBombedHeadline");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienFacilityBombedSummary", new object[]
			{
				fleet.faction.displayNameWithColor,
				facility.region.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AlienFacilityBombedDetail", new object[]
			{
				fleet.faction.displayNameWithColor,
				facility.region.displayName,
				facility.region.nation.displayNameWithArticle
			});
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_alienFacilityBombed;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = facility.region;
			notificationQueueItem.soundToPlay = TINotificationQueueState.GetRandomQuietExplosion();
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC4 RID: 15556 RVA: 0x00176344 File Offset: 0x00174544
		public static void LogXenoformingDetected(TIFactionState detectingFaction, TIRegionXenoformingState xenoforming)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			notificationQueueItem.icon = TemplateManager.global.pathGeoscapeXenoform_gui;
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.popupResource1 = TemplateManager.global.pathGeoscapeXenoform_gui;
			notificationQueueItem.popupResource2 = xenoforming.region.nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.XenoformHed", new object[] { xenoforming.region.displayName });
			notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.XenoformDetail", new object[]
			{
				xenoforming.region.displayName,
				xenoforming.region.nation.displayNameWithArticle
			})).ToString();
			notificationQueueItem.illustrationResource = xenoforming.GetIllustrationPath(detectingFaction);
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.XenoformSummary", new object[]
			{
				xenoforming.region.displayName,
				xenoforming.region.nation.displayNameWithArticle
			});
			notificationQueueItem.gotoGameState = xenoforming;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Aliens_Sighted_Earth";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC5 RID: 15557 RVA: 0x0017647C File Offset: 0x0017467C
		public static void LogAlienFaunaArmySpawned(TIMegafaunaArmyState army)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			if (army.currentNation != GameStateManager.AlienNation())
			{
				notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
				notificationQueueItem.popupResource1 = army.GetIconForegroundResource;
				notificationQueueItem.popupResource2 = army.currentNation.flagResource;
				notificationQueueItem.popup1BackgroundResource = army.GetIconBackgroundResource;
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienFaunaArmySpawnedHed", new object[] { army.currentRegion.displayName });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AlienFaunaArmySpawnedDetail", new object[]
				{
					army.currentRegion.displayName,
					army.currentRegion.nation.displayNameWithArticle
				});
			}
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_alienFaunaSpawn;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienFaunaArmySpawnedSummary", new object[]
			{
				army.currentRegion.displayName,
				army.currentRegion.nation.displayNameWithArticle
			});
			notificationQueueItem.gotoGameState = army;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Aliens_Sighted_Earth";
			notificationQueueItem.fanfareToPlay = "event:/Music/Fanfares/trig_Aliens_Action";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC6 RID: 15558 RVA: 0x001765E4 File Offset: 0x001747E4
		public static void LogAlienNationFounded(TINationState alienNation, TINationState absorbedNation, bool peaceful)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = alienNation.flagResource;
			notificationQueueItem.popupResource1 = alienNation.flagResource;
			notificationQueueItem.popupResource2 = absorbedNation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienNationFoundedHed", new object[] { alienNation.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienNationFoundedSummary", new object[] { absorbedNation.displayNameWithArticle });
			notificationQueueItem.itemDetail = (peaceful ? Loc.T("UI.Notifications.AlienNationFoundedDetail_Peaceful", new object[] { absorbedNation.displayNameWithArticle }) : Loc.T("UI.Notifications.AlienNationFoundedDetail_War", new object[] { absorbedNation.displayNameWithArticle }));
			notificationQueueItem.gotoGameState = alienNation;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Aliens_Sighted_Earth";
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_alienNationFounded;
			notificationQueueItem.fanfareToPlay = "event:/Music/Fanfares/trig_Aliens_Action";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC7 RID: 15559 RVA: 0x001766F8 File Offset: 0x001748F8
		public static void LogAlienNationGrows(TINationState alienNation, TIGameState absorbedGameState)
		{
			TINationState ref_nation = absorbedGameState.ref_nation;
			TIRegionState tiregionState = absorbedGameState as TIRegionState;
			string displayName = absorbedGameState.displayName;
			string text = ((tiregionState != null) ? tiregionState.displayName : null) ?? ref_nation.displayNameWithArticle;
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = alienNation.flagResource;
			notificationQueueItem.popupResource1 = alienNation.flagResource;
			notificationQueueItem.popupResource2 = ref_nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienNationGrowsHed", new object[] { alienNation.displayNameWithArticleCapitalized, displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienNationGrowsSummary", new object[] { alienNation.displayNameWithArticleCapitalized, text });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AlienNationGrowsDetail", new object[] { alienNation.displayNameWithArticleCapitalized, text });
			notificationQueueItem.gotoGameState = ref_nation.capital;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC8 RID: 15560 RVA: 0x00176804 File Offset: 0x00174A04
		public static void LogAlienNationCapitalConquered(TINationState alienNation, TIRegionState oldCapital, TIRegionState newCapital)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = alienNation.flagResource;
			notificationQueueItem.popupResource1 = alienNation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienCapitalConqueredHed", new object[] { alienNation.nationalAdjective, oldCapital.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienCapitalConqueredSummary", new object[] { oldCapital.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AlienCapitalConqueredDetail", new object[] { alienNation.nationalAdjective, oldCapital.displayName, newCapital.displayName });
			notificationQueueItem.gotoGameState = oldCapital;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CC9 RID: 15561 RVA: 0x001768E0 File Offset: 0x00174AE0
		public static void LogAlienNationOverthrown(TINationState alienNation, TIRegionState capital)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = alienNation.flagResource;
			notificationQueueItem.popupResource1 = alienNation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienNationOverthrownHed", new object[] { alienNation.displayNameWithArticle });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienNationOverthrownSummary", new object[] { alienNation.displayNameWithArticle });
			if (alienNation.extant)
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AlienNationOverthrownDetail_Partial", new object[] { capital.displayName, alienNation.displayNameWithArticle });
			}
			else
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AlienNationOverthrownDetail_Gone", new object[] { capital.displayName, alienNation.displayNameWithArticle });
			}
			notificationQueueItem.gotoGameState = capital;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Unrest_Cheers";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CCA RID: 15562 RVA: 0x001769E8 File Offset: 0x00174BE8
		public static void LogAlienNationConquered(TINationState alienNation, TIArmyState conqueringArmy)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = alienNation.flagResource;
			notificationQueueItem.popupResource1 = alienNation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienNationConqueredHed", new object[] { conqueringArmy.homeNation.displayNameWithArticleCapitalized });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienNationConqueredSummary", new object[] { alienNation.displayNameWithArticle });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AlienNationConqueredDetail", new object[]
			{
				conqueringArmy.homeNation.displayNameWithArticleCapitalized,
				conqueringArmy.currentRegion.displayNameSentIn,
				alienNation.displayNameWithArticleCapitalized
			});
			notificationQueueItem.gotoGameState = conqueringArmy.currentRegion;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CCB RID: 15563 RVA: 0x00176AD0 File Offset: 0x00174CD0
		public static void LogAliensPassTechnologyToMe(TICouncilorState grantingAlien, TICouncilorState receivingCouncilor, float research, float exotics)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(receivingCouncilor.faction);
			notificationQueueItem.relevantFactions.Add(receivingCouncilor.faction);
			if (TIPlayerProfileManager.useCouncilorVideo)
			{
				notificationQueueItem.videoResource = grantingAlien.videoResource;
			}
			else
			{
				notificationQueueItem.showSideArt = true;
				notificationQueueItem.illustrationResource = grantingAlien.faction.pathLeaderHeadPortrait;
			}
			notificationQueueItem.icon = grantingAlien.faction.factionIcon64path;
			notificationQueueItem.popupResource1 = grantingAlien.faction.factionIcon256path;
			notificationQueueItem.popupResource2 = grantingAlien.faction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ReceivePassTechHeadline");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ReceivePassTechSummary");
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			tiresourcesCost.AddCost(FactionResource.Research, -research, true);
			tiresourcesCost.AddCost(FactionResource.Exotics, -exotics, true);
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ReceivePassTechDetail", new object[] { tiresourcesCost.ToString("Relevant", true, false, null, false, FactionResource.None) });
			notificationQueueItem.gotoGameState = receivingCouncilor;
			notificationQueueItem.soundToPlay = "event:/SFX/Game_SFX/Hydra/trig_sfx_Hydra_Affirmative";
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CCC RID: 15564 RVA: 0x00176BEC File Offset: 0x00174DEC
		public static void LogEnemyCouncilorKilledOnMissionTargetingMe(TIMissionState mission)
		{
			TIFactionState ref_faction = mission.target.ref_faction;
			if (ref_faction != null)
			{
				TICouncilorState councilor = mission.councilor;
				NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
				notificationQueueItem.primaryFactions.Add(ref_faction);
				notificationQueueItem.relevantFactions.Add(ref_faction);
				notificationQueueItem.icon = TINotificationQueueState.councilorGUIIconPath(councilor);
				notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(councilor);
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.EnemyCouncilorKilledOnMission.Hed", new object[] { mission.councilor.faction.adjective });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.EnemyCouncilorKilledOnMission.Summary", new object[] { mission.councilor.faction.displayNameWithColor });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.EnemyCouncilorKilledOnMission.Detail", new object[]
				{
					mission.councilor.faction.adjective,
					mission.councilor.displayName,
					mission.missionTemplate.displayName
				});
				notificationQueueItem.gotoGameState = null;
				TINotificationQueueState.AddItem(notificationQueueItem, councilor.isAlien);
			}
		}

		// Token: 0x06003CCD RID: 15565 RVA: 0x00176D04 File Offset: 0x00174F04
		public static void LogMyCouncilorKilledOnMission(TIMissionState mission)
		{
			TICouncilorState councilor = mission.councilor;
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(councilor.faction);
			notificationQueueItem.relevantFactions.Add(councilor.faction);
			notificationQueueItem.icon = TINotificationQueueState.councilorGUIIconPath(councilor);
			notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(councilor);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.CouncilorKilledOnMission.Hed", new object[] { mission.councilor.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.CouncilorKilledOnMission.Summary", new object[] { mission.councilor.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.CouncilorKilledOnMission.Detail", new object[]
			{
				mission.councilor.displayName,
				mission.missionTemplate.displayName
			});
			notificationQueueItem.gotoGameState = null;
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			TIGameState target = mission.target;
			bool? flag;
			if (target == null)
			{
				flag = null;
			}
			else
			{
				TIFactionState ref_faction = target.ref_faction;
				flag = ((ref_faction != null) ? new bool?(ref_faction.IsAlienFaction) : null);
			}
			bool? flag2 = flag;
			TINotificationQueueState.AddItem(notificationQueueItem2, flag2.GetValueOrDefault());
		}

		// Token: 0x06003CCE RID: 15566 RVA: 0x00176E24 File Offset: 0x00175024
		public static void LogMyCouncilorDetained(TIFactionState detainingFaction, TICouncilorState detainedCouncilor)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = detainedCouncilor.iconResource;
			notificationQueueItem.primaryFactions = detainedCouncilor.ref_factions;
			notificationQueueItem.relevantFactions = detainedCouncilor.ref_factions;
			notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(detainedCouncilor);
			notificationQueueItem.popupResource2 = detainingFaction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyCouncilorDetainedHed", new object[] { detainedCouncilor.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyCouncilorDetainedSummary", new object[] { detainedCouncilor.displayName, detainingFaction.displayNameWithColor });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MyCouncilorDetained", new object[]
			{
				detainingFaction.displayNameWithColor,
				detainedCouncilor.displayName,
				TIUtilities.GetLocationString(detainedCouncilor.location, true, true)
			});
			notificationQueueItem.gotoGameState = detainedCouncilor.location;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Jail_Door";
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_myCouncilorDetained;
			TINotificationQueueState.AddItem(notificationQueueItem, detainingFaction == GameStateManager.AlienFaction() || detainedCouncilor.faction == GameStateManager.AlienFaction());
		}

		// Token: 0x06003CCF RID: 15567 RVA: 0x00176F54 File Offset: 0x00175154
		public static void LogMyCouncilorReleased(TIFactionState detainingFaction, TICouncilorState detainedCouncilor)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = detainedCouncilor.iconResource;
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			TIFactionState faction = detainedCouncilor.faction;
			notificationQueueItem2.backgroundColor = ((faction != null) ? faction.template.color : Color.clear);
			notificationQueueItem.primaryFactions = detainedCouncilor.ref_factions;
			notificationQueueItem.primaryFactions.Add(detainingFaction);
			notificationQueueItem.relevantFactions = detainedCouncilor.ref_factions;
			notificationQueueItem.relevantFactions.Add(detainingFaction);
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyCouncilorReleasedSummary", new object[] { detainedCouncilor.displayName, detainingFaction.adjectiveWithColor });
			notificationQueueItem.gotoGameState = detainedCouncilor.location;
			notificationQueueItem.musicIntensityDelta = -0.225f;
			TINotificationQueueState.AddItem(notificationQueueItem, detainingFaction == GameStateManager.AlienFaction() || detainedCouncilor.faction == GameStateManager.AlienFaction());
		}

		// Token: 0x06003CD0 RID: 15568 RVA: 0x00177034 File Offset: 0x00175234
		public static void LogMyCouncilorAssassinated(TICouncilorState deadCouncilor, TICouncilorState killingCouncilor, float hate)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			CouncilorView viewofCouncilor = deadCouncilor.faction.GetViewofCouncilor(killingCouncilor);
			notificationQueueItem.primaryFactions = deadCouncilor.ref_factions;
			notificationQueueItem.relevantFactions = deadCouncilor.ref_factions;
			notificationQueueItem.icon = deadCouncilor.iconResource;
			notificationQueueItem.backgroundColor = deadCouncilor.faction.template.color;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyCouncilorAssassinatedHed", new object[] { deadCouncilor.displayName });
			string text;
			if (viewofCouncilor.factionCurrent != null)
			{
				text = viewofCouncilor.factionCurrent.displayNameCapitalizedWithColor;
			}
			else if (hate > 0f)
			{
				text = killingCouncilor.faction.displayNameWithColor;
			}
			else
			{
				text = Loc.T("UI.Notifications.FactionUnknownWithArticle");
			}
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyCouncilorAssassinatedSummary", new object[]
			{
				deadCouncilor.faction.adjectiveWithColor,
				deadCouncilor.displayName,
				text
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MyCouncilorAssassinatedDetail", new object[]
			{
				deadCouncilor.faction.adjectiveWithColor,
				deadCouncilor.displayName,
				text
			});
			notificationQueueItem.illustrationResource = (deadCouncilor.inSpace ? TemplateManager.global.illus_myCouncilorAssassinated_Space : (((double)TIUtilities.RandomFloatValue() < 0.3) ? TemplateManager.global.illus_myCouncilorAssassinated_Earth_alt : TemplateManager.global.illus_myCouncilorAssassinated_Earth));
			notificationQueueItem.gotoGameState = deadCouncilor.location;
			notificationQueueItem.musicIntensityDelta = 0.9f;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Assasination_Gunshot";
			notificationQueueItem.fanfareToPlay = "event:/Music/Fanfares/trig_Councilor_Assasination";
			TINotificationQueueState.AddItem(notificationQueueItem, viewofCouncilor.factionMemory == GameStateManager.AlienFaction());
		}

		// Token: 0x06003CD1 RID: 15569 RVA: 0x001771E0 File Offset: 0x001753E0
		public static void LogEnemyMissionFailure(TIMissionState mission, MissionResult result)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			List<TIFactionState> list = mission.target.ref_factions.Where<TIFactionState>((TIFactionState x) => x != mission.councilor.faction).ToList<TIFactionState>();
			notificationQueueItem.relevantFactions.AddRange(list);
			TIFactionState tifactionState = ((mission.missionTemplate.hate[(int)result.missionOutcome] > 0f || list.All<TIFactionState>((TIFactionState x) => x.HasIntelOnCouncilorMission(mission.councilor))) ? mission.ref_faction : null);
			string text = ((tifactionState == null) ? Loc.T("UI.Notifications.FactionUnknownWithArticle") : tifactionState.displayNameWithColor);
			string text2 = ((tifactionState == null) ? Utilities.Capitalize(Loc.T("UI.Notifications.FactionUnknownWithArticle")) : tifactionState.displayNameCapitalizedWithColor);
			string stateDisplayName = TIUtilities.GetStateDisplayName(mission.target, null, false, false, false, false, true);
			notificationQueueItem.icon = ((tifactionState == null) ? mission.missionTemplate.missionIconImagePath_Off : tifactionState.factionIcon64path);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.EnemyMissionFailsHed", new object[] { text2, mission.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.EnemyMissionFailsSummary", new object[] { text2, mission.displayName, stateDisplayName });
			if (mission.missionTemplate.UIalertEnemyOnFail)
			{
				notificationQueueItem.primaryFactions.AddRange(list);
				StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.EnemyMissionFailsDetail", new object[] { text, mission.displayName, stateDisplayName }));
				if (mission.target.isCouncilorState)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.Notifications.EnemyMissionFailsAdvice", new object[]
					{
						mission.target.displayName,
						TIFactionState.goToGroundMission.displayName
					}));
				}
				notificationQueueItem.itemDetail = stringBuilder.ToString();
			}
			notificationQueueItem.gotoGameState = mission.target;
			TINotificationQueueState.AddItem(notificationQueueItem, tifactionState != null && tifactionState.IsAlienFaction);
		}

		// Token: 0x06003CD2 RID: 15570 RVA: 0x00177424 File Offset: 0x00175624
		public static void LogCouncilorKilledInAttack(TICouncilorState deadCouncilor, TIGameState location)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = deadCouncilor.ref_factions;
			notificationQueueItem.relevantFactions = deadCouncilor.ref_factions;
			if (deadCouncilor.detained)
			{
				notificationQueueItem.primaryFactions.Add(deadCouncilor.detainingFaction);
				notificationQueueItem.relevantFactions.Add(deadCouncilor.detainingFaction);
			}
			notificationQueueItem.icon = deadCouncilor.iconResource;
			notificationQueueItem.backgroundColor = deadCouncilor.faction.template.color;
			notificationQueueItem.gotoGameState = deadCouncilor.location;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.CouncilorDiesInAttack", new object[] { deadCouncilor.displayName, location.displayName });
			string locationString = TIUtilities.GetLocationString(location, false, true);
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.CouncilorDiesInAttackSummary", new object[]
			{
				deadCouncilor.faction.adjectiveWithColor,
				deadCouncilor.displayName,
				locationString
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.CouncilorDiesInAttackDetail", new object[]
			{
				deadCouncilor.faction.adjectiveWithColor,
				deadCouncilor.displayName,
				locationString
			});
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CD3 RID: 15571 RVA: 0x0017754C File Offset: 0x0017574C
		public static void LogCouncilorPassesAway(TICouncilorState deadCouncilor)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = deadCouncilor.ref_factions;
			notificationQueueItem.relevantFactions = deadCouncilor.ref_factions;
			if (deadCouncilor.detained)
			{
				notificationQueueItem.primaryFactions.Add(deadCouncilor.detainingFaction);
				notificationQueueItem.relevantFactions.Add(deadCouncilor.detainingFaction);
			}
			notificationQueueItem.icon = deadCouncilor.iconResource;
			notificationQueueItem.backgroundColor = deadCouncilor.faction.template.color;
			notificationQueueItem.gotoGameState = deadCouncilor.location;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.CouncilorDiesNaturalCausesHed", new object[] { deadCouncilor.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.CouncilorDiesNaturalCausesSummary", new object[]
			{
				deadCouncilor.faction.adjectiveWithColor,
				deadCouncilor.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.CouncilorDiesNaturalCausesDetail", new object[]
			{
				deadCouncilor.faction.adjectiveWithColor,
				deadCouncilor.displayName
			});
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CD4 RID: 15572 RVA: 0x0017765C File Offset: 0x0017585C
		public static void LogCouncilorGainsTrait(TICouncilorState councilor, TITraitTemplate trait)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = new List<TIFactionState> { councilor.faction };
			notificationQueueItem.relevantFactions = new List<TIFactionState> { councilor.faction };
			notificationQueueItem.icon = councilor.iconResource;
			notificationQueueItem.backgroundColor = councilor.faction.template.color;
			notificationQueueItem.gotoGameState = councilor.location;
			notificationQueueItem.videoResource = councilor.videoResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.CouncilorGainsTraitHed", new object[] { councilor.displayName, trait.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.CouncilorGainsTraitSummary", new object[]
			{
				councilor.faction.adjectiveWithColor,
				councilor.displayName,
				trait.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.CouncilorGainsTraitDetail", new object[]
			{
				councilor.faction.adjectiveWithColor,
				councilor.displayName,
				trait.displayName,
				trait.description
			});
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CD5 RID: 15573 RVA: 0x00177788 File Offset: 0x00175988
		public static void LogDetainedCouncilorDismissed(TIFactionState detainingFaction, TICouncilorState detainedCouncilor)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = detainedCouncilor.iconResource;
			notificationQueueItem.backgroundColor = detainedCouncilor.faction.template.color;
			notificationQueueItem.primaryFactions.Add(detainingFaction);
			notificationQueueItem.relevantFactions.Add(detainingFaction);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.DetainedCouncilorDismissedHed");
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.DetainedCouncilorDismissedDetail", new object[]
			{
				detainedCouncilor.displayName,
				detainedCouncilor.faction.displayNameWithColor
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.DetainedCouncilorDismissedSummary", new object[]
			{
				detainedCouncilor.displayName,
				detainedCouncilor.faction.displayNameWithColor
			});
			notificationQueueItem.gotoGameState = detainedCouncilor.location;
			TINotificationQueueState.AddItem(notificationQueueItem, detainingFaction == GameStateManager.AlienFaction() || detainedCouncilor.faction == GameStateManager.AlienFaction());
		}

		// Token: 0x06003CD6 RID: 15574 RVA: 0x0017787C File Offset: 0x00175A7C
		public static void LogControlPointDefenseExpires(TIControlPoint controlPoint)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = controlPoint.ref_factions;
			notificationQueueItem.relevantFactions = controlPoint.ref_factions;
			notificationQueueItem.icon = TemplateManager.global.defendInterestsMissionIconPath;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ControlPointDefenseExpiresSummary", new object[] { controlPoint.nation.displayNameWithArticle });
			notificationQueueItem.gotoGameState = controlPoint.nation;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CD7 RID: 15575 RVA: 0x001778F8 File Offset: 0x00175AF8
		public static void LogCrackdownExpires(TIControlPoint controlPoint)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = controlPoint.ref_factions;
			notificationQueueItem.relevantFactions = controlPoint.ref_factions;
			notificationQueueItem.icon = TemplateManager.global.crackdownMissionIconPath;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ControlPointCrackdownExpiresSummary", new object[] { controlPoint.nation.displayNameWithArticle });
			notificationQueueItem.gotoGameState = controlPoint.nation;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CD8 RID: 15576 RVA: 0x00177974 File Offset: 0x00175B74
		public static void LogControlConsolidated(TIFactionState faction, TINationState nation)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ConsolidatedControlHed", new object[] { faction.displayNameCapitalized, nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ConsolidatedControlDetail", new object[] { faction.displayNameCapitalizedWithColor, nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ConsolidatedControlSummary", new object[] { faction.displayNameCapitalizedWithColor, nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.gotoGameState = nation;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CD9 RID: 15577 RVA: 0x00177A48 File Offset: 0x00175C48
		public static void LogCPDominated(TIControlPoint controlPoint, TIFactionState newOwner, TIFactionState oldOwner, TIMissionOutcome outcome, List<TIGameState> newControlPoints, List<TIGameState> oldControlPoints)
		{
			NotificationQueueItem item = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			item.relevantFactions = TINotificationQueueState.AllFactionsExcept(newOwner);
			item.icon = newOwner.factionIcon64path;
			item.itemSummary = Loc.T("UI.Notifications.MyControlPointDominatedSummary", new object[]
			{
				newOwner.adjectiveWithColor,
				oldOwner.adjectiveWithColor,
				controlPoint.nation.displayNameWithArticleAndPlacePrep
			});
			item.itemHeadline = Loc.T("UI.Notifications.MyControlPointDominatedHed");
			item.popupResource1 = controlPoint.nation.flagResource;
			item.popupResource2 = newOwner.factionIcon256path;
			string text = string.Empty;
			int numArmies = controlPoint.numArmies;
			if (numArmies > 0)
			{
				text = Loc.T("UI.Notifications.ArmiesLost", new object[]
				{
					(numArmies == 1) ? Loc.T("UI.Notifications.army") : Loc.T("UI.Notifications.armies"),
					newOwner.displayNameWithColor
				});
			}
			if (item.relevantFactions.Any<TIFactionState>((TIFactionState x) => TINotificationQueueState.FirstNotificationOfType(x, item.templateName)))
			{
				item.primaryFactions = item.relevantFactions;
			}
			else
			{
				item.primaryFactions.Add(oldOwner);
			}
			item.newControlPoints = newControlPoints;
			item.oldControlPoints = oldControlPoints;
			if (outcome == TIMissionOutcome.Failure || outcome == TIMissionOutcome.CriticalFailure)
			{
				item.itemDetail = Loc.T("UI.Notifications.MyControlPointDominatedDetail2", new object[]
				{
					newOwner.displayNameWithColor,
					controlPoint.nation.displayNameWithArticleAndPlacePrep,
					text
				});
				item.musicIntensityDelta = 0.45f;
			}
			else
			{
				item.itemDetail = Loc.T("UI.Notifications.MyControlPointDominatedDetail", new object[]
				{
					newOwner.displayNameWithColor,
					controlPoint.nation.displayNameWithArticleAndPlacePrep,
					text
				});
				item.musicIntensityDelta = 0.225f;
			}
			item.controlPointsRelevant = true;
			item.gotoGameState = controlPoint;
			TINotificationQueueState.AddItem(item, false);
		}

		// Token: 0x06003CDA RID: 15578 RVA: 0x00177C6C File Offset: 0x00175E6C
		public static void LogMissionOutcome(TIMissionState mission, MissionResult result, TIFactionState heldTargetFaction, List<TIGameState> newControlPoints = null, List<TIGameState> oldControlPoints = null, bool spy = false, string abortedReason = "")
		{
			string text = "LogMissionOutcome";
			if (mission.councilor.permanentAssignment || mission.councilor.permanentDefenseMode)
			{
				text = new StringBuilder(text).Append("_Permanent").ToString();
			}
			else if (spy)
			{
				text = new StringBuilder(text).Append("_Spy").ToString();
			}
			else if (!mission.missionTemplate.ContestedMission)
			{
				text = new StringBuilder(text).Append("_Uncontested").ToString();
			}
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(text);
			TICouncilorState councilor = mission.councilor;
			if (spy)
			{
				notificationQueueItem.primaryFactions.Add(councilor.agentForFaction);
				notificationQueueItem.relevantFactions.Add(councilor.agentForFaction);
			}
			else
			{
				notificationQueueItem.primaryFactions.Add(councilor.faction);
				notificationQueueItem.relevantFactions.Add(councilor.faction);
			}
			TIFactionState faction = councilor.faction;
			bool flag = result.missionOutcome == TIMissionOutcome.CriticalSuccess || result.missionOutcome == TIMissionOutcome.Success;
			string displayName = councilor.displayName;
			string displayName2 = mission.displayName;
			string locationString = TIUtilities.GetLocationString(councilor.location, true, true);
			string locationString2 = TIUtilities.GetLocationString(councilor.location, true, false);
			string stateDisplayName = TIUtilities.GetStateDisplayName(mission.target, faction, false, false, false, false, false);
			string text2 = Loc.T(new StringBuilder("UI.Notifications.").Append(result.missionOutcome.ToString()).ToString());
			string text3 = (flag ? "<color=#85B260>" : "<color=#B26A60>");
			notificationQueueItem.outcome = result.missionOutcome;
			if (councilor.faction == GameControl.control.activePlayer)
			{
				councilor.PlayMissionVoice(mission.missionTemplate, result.missionOutcome, councilor.OnEarth);
				if (result.missionOutcome == TIMissionOutcome.Success || result.missionOutcome == TIMissionOutcome.CriticalSuccess)
				{
					TIFactionState ref_faction = mission.target.ref_faction;
					if (ref_faction != null && ref_faction.IsAlienFaction && !string.IsNullOrEmpty(mission.missionTemplate.successSFXAlienSpecial))
					{
						notificationQueueItem.soundToPlay = mission.missionTemplate.successSFXAlienSpecial;
					}
					else if (!string.IsNullOrEmpty(mission.missionTemplate.successSFX))
					{
						notificationQueueItem.soundToPlay = mission.missionTemplate.successSFX;
					}
				}
			}
			if (result.missionOutcome == TIMissionOutcome.Aborted)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MissionHeadline", new object[] { displayName2, text2 });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AbortedSummary", new object[] { displayName, displayName2 });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AbortedDetail", new object[]
				{
					displayName,
					displayName2,
					Loc.T(abortedReason)
				});
			}
			else
			{
				string text7;
				if (mission.missionTemplate.ContestedMission)
				{
					bool flag2 = false;
					int num = 0;
					string text4 = string.Empty;
					string text5 = string.Empty;
					while (!flag2 && num < 40)
					{
						string text6 = new StringBuilder("P").Append(num.ToString()).ToString();
						text4 = result.successChance.ToPercent(text6);
						text5 = result.roll.ToPercent(text6);
						if (text5 != text4)
						{
							flag2 = true;
						}
						num++;
					}
					if (!flag2)
					{
						text4 = result.successChance.ToPercent("P0");
						text5 = result.roll.ToPercent("P0");
					}
					text7 = Loc.T("UI.Notifications.ContestedResult", new object[] { displayName, displayName2, stateDisplayName, locationString2, text4, text5, text3, text2 });
					notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MissionHeadline", new object[] { displayName2, text2 });
				}
				else
				{
					text7 = Loc.T("UI.Notifications.UncontestedResult", new object[] { displayName, displayName2, locationString2 });
					notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MissionCompleteHed", new object[] { displayName2 });
				}
				TINationState ref_nation = mission.target.ref_nation;
				TIRegionState ref_region = mission.target.ref_region;
				TICouncilorState ref_councilor = mission.target.ref_councilor;
				TIControlPoint ticontrolPoint = mission.target.ref_controlPoint;
				TIHabState ref_hab = mission.target.ref_hab;
				notificationQueueItem.controlPointsRelevant = oldControlPoints.Count > 0 && newControlPoints.Count > 0 && !oldControlPoints.SequenceEqual<TIGameState>(newControlPoints);
				if (notificationQueueItem.controlPointsRelevant && ticontrolPoint == null)
				{
					foreach (TIControlPoint ticontrolPoint2 in ref_nation.controlPoints)
					{
						if (oldControlPoints.Count > ticontrolPoint2.positionInNation && ticontrolPoint2.ref_faction != oldControlPoints[ticontrolPoint2.positionInNation])
						{
							ticontrolPoint = ticontrolPoint2;
							break;
						}
					}
				}
				StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder().Append("TIMissionTemplate.").Append(result.missionOutcome.ToString()).Append(".")
					.Append(mission.templateName)
					.ToString()));
				if (result.valueChange != null && result.valueChange.Contains("|"))
				{
					string text8 = result.valueChange.Substring(result.valueChange.LastIndexOf("|") + 1);
					result.valueChange = result.valueChange.Substring(0, result.valueChange.IndexOf("|"));
					stringBuilder.Replace("{returnedValue2}", text8);
				}
				else
				{
					stringBuilder.Replace("{returnedValue2}", string.Empty);
				}
				stringBuilder.Replace("{returnedValue}", (result.valueChange == "0%") ? Loc.T("UI.Notifications.ASmallAmount") : result.valueChange);
				stringBuilder.Replace("{myFactionName}", faction.displayNameWithColor);
				stringBuilder.Replace("{myFactionNameCapitalized}", faction.displayNameCapitalizedWithColor);
				stringBuilder.Replace("{targetNationNameWithArticle}", (ref_nation != null) ? ref_nation.displayNameWithArticle : null);
				stringBuilder.Replace("{targetNationNameWithPrep}", (ref_nation != null) ? ref_nation.displayNameWithArticleAndPlacePrep : null);
				stringBuilder.Replace("{targetNationAdjective}", (ref_nation != null) ? ref_nation.nationalAdjective : null);
				stringBuilder.Replace("{myTargetNationControlPoints}", (ref_nation != null) ? ref_nation.CountFactionControlPoints(faction, true, false, true).ToString() : null);
				stringBuilder.Replace("{totalTargetNationControlPoints}", (ref_nation != null) ? ref_nation.numControlPoints.ToString() : null);
				stringBuilder.Replace("{missionName}", mission.displayName);
				stringBuilder.Replace("{targetFactionName}", (heldTargetFaction != null) ? heldTargetFaction.displayNameWithColor : null);
				stringBuilder.Replace("{targetFactionAdjective}", (heldTargetFaction != null) ? heldTargetFaction.adjective : null);
				stringBuilder.Replace("{targetRegionName}", (ref_region != null) ? ref_region.displayName : null);
				stringBuilder.Replace("{targetHabName}", (ref_hab != null) ? ref_hab.GetDisplayName(mission.councilor.faction) : null);
				stringBuilder.Replace("{targetNationUnrestWithString}", (ref_nation != null) ? ref_nation.GetUnrestDescriptiveStringAndValue(1) : null);
				stringBuilder.Replace("{targetDisplayName}", TIUtilities.GetStateDisplayName(mission.target, faction, false, false, false, false, false));
				stringBuilder.Replace("{targetDisplayNameSent}", TIUtilities.GetStateDisplayName(mission.target, faction, true, false, false, false, false));
				stringBuilder.Replace("{targetDisplayNameSentArticle}", TIUtilities.GetStateDisplayName(mission.target, faction, false, true, false, false, false));
				stringBuilder.Replace("{controlPointTypeDisplayName}", (ticontrolPoint != null) ? ticontrolPoint.controlPointTypeDisplayName : null);
				stringBuilder.Replace("{targetLocationStrSentence}", TIUtilities.GetLocationString(mission.targetLocation, false, true));
				StringBuilder stringBuilder2 = stringBuilder;
				string text9 = "{targetOrgDetails}";
				TIOrgState ref_org = mission.target.ref_org;
				stringBuilder2.Replace(text9, (ref_org != null) ? ref_org.description(true, councilor.faction, false, false) : null);
				if (ref_councilor != null)
				{
					stringBuilder.Replace("{targetCouncilorName}", councilor.faction.GetViewofCouncilor(ref_councilor).displayNameCurrentSentence);
				}
				if (notificationQueueItem.controlPointsRelevant)
				{
					TIGameState tigameState = oldControlPoints.Last<TIGameState>();
					TIFactionState ref_faction2 = newControlPoints.Last<TIGameState>().ref_faction;
					if (tigameState != ref_faction2 && ref_faction2 != null)
					{
						stringBuilder.AppendLine().AppendLine().Append(Loc.T("TIMissionResult_ExecutiveControlChange", new object[] { ref_faction2.displayNameWithColor }));
					}
				}
				string text10 = stringBuilder.ToString();
				notificationQueueItem.itemDetail = new StringBuilder(256).AppendLine(text10).AppendLine().AppendLine(text7)
					.ToString();
				if (mission.missionTemplate.ContestedMission)
				{
					notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ContestedMissionSummary", new object[] { displayName, text3, text2, displayName2, locationString });
				}
				else
				{
					notificationQueueItem.itemSummary = Loc.T("UI.Notifications.UncontestedMissionSummary", new object[] { displayName, displayName2, locationString });
				}
				notificationQueueItem.illustrationResource = (flag ? mission.missionTemplate.GetCompletedIllustrationResource(mission.target, ticontrolPoint) : string.Empty);
			}
			if (mission.templateName == TIFactionState.setPolicyMission.dataName && flag)
			{
				notificationQueueItem.alertBlockFaction = faction;
				notificationQueueItem.promptingGameState = mission.target.ref_nation;
				notificationQueueItem.alertBlockEventName = "PromptSelectPolicy";
				notificationQueueItem.alertRelatedState = councilor;
			}
			if (spy)
			{
				notificationQueueItem.itemHeadline = new StringBuilder(Loc.T("UI.Notifications.SpyReport")).AppendLine().AppendLine(notificationQueueItem.itemHeadline).ToString();
			}
			notificationQueueItem.icon = mission.missionTemplate.missionIconImagePath_Off;
			notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(councilor);
			if (councilor.faction != null)
			{
				notificationQueueItem.backgroundColor = councilor.faction.template.color;
			}
			notificationQueueItem.popupResource2 = mission.missionTemplate.missionIconImagePath_Off;
			notificationQueueItem.animationSpriteSheetPath = mission.missionTemplate.resolvingAnimation;
			notificationQueueItem.gotoGameState = councilor;
			notificationQueueItem.mission = mission;
			if (!spy)
			{
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.RepeatMission);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.RepeatMissionContinue);
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.PermanentAssignment);
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CDB RID: 15579 RVA: 0x001786AC File Offset: 0x001768AC
		public static void LogIPassivelyCapturedACouncilor(TICouncilorState capturedCouncilor, TIFactionState myFaction, TIGameState location)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(myFaction);
			notificationQueueItem.relevantFactions.Add(myFaction);
			notificationQueueItem.icon = myFaction.factionIcon64path;
			notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(capturedCouncilor);
			notificationQueueItem.popupResource2 = "councilor_missions/ICO_detain_off";
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HedPassiveCouncilorCapture");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.PassiveCouncilorCaptureSummary", new object[] { capturedCouncilor.faction.displayNameWithColor });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.PassiveCouncilorCapture", new object[]
			{
				myFaction.adjective,
				capturedCouncilor.displayName,
				capturedCouncilor.typeTemplate.displayName,
				capturedCouncilor.faction.displayNameWithColor,
				TIUtilities.GetLocationString(location, false, true)
			});
			notificationQueueItem.gotoGameState = capturedCouncilor;
			TINotificationQueueState.LogFirstFactionEncounter(myFaction, capturedCouncilor.faction, capturedCouncilor, location);
			TINotificationQueueState.AddItem(notificationQueueItem, capturedCouncilor.isAlien);
		}

		// Token: 0x06003CDC RID: 15580 RVA: 0x001787A8 File Offset: 0x001769A8
		public static void LogFactionOrgStolen(TIFactionState stealingFaction, TIFactionState victimFaction, TIOrgState org, List<TIOrgState> discardedOrgs, TICouncilorState victimCouncilor = null)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(victimFaction);
			notificationQueueItem.relevantFactions.AddRange(victimFaction.ref_factions);
			notificationQueueItem.icon = org.orgIconPath;
			notificationQueueItem.popupResource1 = org.orgIconPath;
			if (stealingFaction.IsAlienFaction)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FactionOrgStolenHed_Alien", new object[] { org.displayName });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FactionOrgStolenSummary_Alien", new object[] { org.displayNameWithArticle });
				if (victimCouncilor == null)
				{
					NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
					string text = "UI.Notifications.FactionOrgStolenDetail.Pool_Alien";
					object[] array = new object[3];
					array[0] = org.displayNameWithArticleCapitalized;
					array[1] = victimFaction.displayNameWithColor;
					int num = 2;
					object obj;
					if (discardedOrgs.Count != 0)
					{
						string text2 = "UI.Notifications.MyOrgStolenDiscards";
						object[] array2 = new object[1];
						array2[0] = TIUtilities.ConstructTextList(discardedOrgs.ConvertAll<TIGameState>((TIOrgState x) => x), false, false);
						obj = Loc.T(text2, array2);
					}
					else
					{
						obj = string.Empty;
					}
					array[num] = obj;
					notificationQueueItem2.itemDetail = Loc.T(text, array);
				}
				else
				{
					NotificationQueueItem notificationQueueItem3 = notificationQueueItem;
					string text3 = "UI.Notifications.FactionOrgStolenDetail.Councilor_Alien";
					object[] array3 = new object[3];
					array3[0] = org.displayNameWithArticleCapitalized;
					array3[1] = victimCouncilor.displayName;
					int num2 = 2;
					object obj2;
					if (discardedOrgs.Count != 0)
					{
						string text4 = "UI.Notifications.MyOrgStolenDiscards";
						object[] array4 = new object[1];
						array4[0] = TIUtilities.ConstructTextList(discardedOrgs.ConvertAll<TIGameState>((TIOrgState x) => x), false, false);
						obj2 = Loc.T(text4, array4);
					}
					else
					{
						obj2 = string.Empty;
					}
					array3[num2] = obj2;
					notificationQueueItem3.itemDetail = Loc.T(text3, array3);
				}
			}
			else
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FactionOrgStolenHed", new object[] { org.displayName, stealingFaction.displayName });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FactionOrgStolenSummary", new object[] { org.displayNameWithArticle, stealingFaction.displayNameWithColor });
				if (victimCouncilor == null)
				{
					NotificationQueueItem notificationQueueItem4 = notificationQueueItem;
					string text5 = "UI.Notifications.FactionOrgStolenDetail.Pool";
					object[] array5 = new object[3];
					array5[0] = org.displayNameWithArticleCapitalized;
					array5[1] = stealingFaction.displayNameWithColor;
					int num3 = 2;
					object obj3;
					if (discardedOrgs.Count != 0)
					{
						string text6 = "UI.Notifications.MyOrgStolenDiscards";
						object[] array6 = new object[1];
						array6[0] = TIUtilities.ConstructTextList(discardedOrgs.ConvertAll<TIGameState>((TIOrgState x) => x), false, false);
						obj3 = Loc.T(text6, array6);
					}
					else
					{
						obj3 = string.Empty;
					}
					array5[num3] = obj3;
					notificationQueueItem4.itemDetail = Loc.T(text5, array5);
				}
				else
				{
					NotificationQueueItem notificationQueueItem5 = notificationQueueItem;
					string text7 = "UI.Notifications.FactionOrgStolenDetail.Councilor";
					object[] array7 = new object[4];
					array7[0] = stealingFaction.displayNameCapitalizedWithColor;
					array7[1] = victimCouncilor.displayName;
					array7[2] = org.displayNameWithArticleCapitalized;
					int num4 = 3;
					object obj4;
					if (discardedOrgs.Count != 0)
					{
						string text8 = "UI.Notifications.MyOrgStolenDiscards";
						object[] array8 = new object[1];
						array8[0] = TIUtilities.ConstructTextList(discardedOrgs.ConvertAll<TIGameState>((TIOrgState x) => x), false, false);
						obj4 = Loc.T(text8, array8);
					}
					else
					{
						obj4 = string.Empty;
					}
					array7[num4] = obj4;
					notificationQueueItem5.itemDetail = Loc.T(text7, array7);
				}
			}
			notificationQueueItem.gotoGameState = victimCouncilor ?? victimFaction;
			TINotificationQueueState.AddItem(notificationQueueItem, stealingFaction.IsAlienFaction);
		}

		// Token: 0x06003CDD RID: 15581 RVA: 0x00178AC0 File Offset: 0x00176CC0
		public static void LogMyOrgStolen(TIFactionState stealingFaction, TICouncilorState victim, TIFactionState victimFaction, TIOrgState org, List<TIOrgState> discardedOrgs)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(victimFaction);
			notificationQueueItem.relevantFactions.Add(victimFaction);
			if (victim != null)
			{
				notificationQueueItem.relevantFactions.AddRangeUnique<TIFactionState>(victim.ref_factions);
			}
			notificationQueueItem.icon = org.orgIconPath;
			notificationQueueItem.popupResource1 = org.orgIconPath;
			notificationQueueItem.popupResource2 = "councilor_missions/ICO_hostiletakeover_off";
			string text = ((victim != null) ? victim.displayName : null) ?? victimFaction.displayNameWithColor;
			if (stealingFaction.IsAlienFaction)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyOrgStolenHed_Alien", new object[] { org.displayName });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyOrgStolenSummary_Alien", new object[] { org.displayNameWithArticle });
				if (discardedOrgs.Count == 0)
				{
					notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MyOrgStolenDetail_Alien", new object[]
					{
						org.displayNameWithArticle,
						text,
						string.Empty
					});
				}
				else
				{
					NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
					string text2 = "UI.Notifications.MyOrgStolenDetail_Alien";
					object[] array = new object[3];
					array[0] = org.displayNameWithArticle;
					array[1] = text;
					int num = 2;
					string text3 = "UI.Notifications.MyOrgStolenDiscards";
					object[] array2 = new object[1];
					array2[0] = TIUtilities.ConstructTextList(discardedOrgs.ConvertAll<TIGameState>((TIOrgState x) => x), false, false);
					array[num] = Loc.T(text3, array2);
					notificationQueueItem2.itemDetail = Loc.T(text2, array);
				}
			}
			else
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyOrgStolenHed", new object[] { org.displayName, stealingFaction.displayNameWithColor });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyOrgStolenSummary", new object[] { stealingFaction.displayNameCapitalizedWithColor, text });
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_myOrgStolen;
				if (discardedOrgs.Count == 0)
				{
					notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MyOrgStolenDetail", new object[]
					{
						stealingFaction.displayNameCapitalizedWithColor,
						text,
						org.displayNameWithArticle,
						string.Empty
					});
				}
				else
				{
					NotificationQueueItem notificationQueueItem3 = notificationQueueItem;
					string text4 = "UI.Notifications.MyOrgStolenDetail";
					object[] array3 = new object[4];
					array3[0] = stealingFaction.displayNameCapitalizedWithColor;
					array3[1] = text;
					array3[2] = org.displayNameWithArticle;
					int num2 = 3;
					string text5 = "UI.Notifications.MyOrgStolenDiscards";
					object[] array4 = new object[1];
					array4[0] = TIUtilities.ConstructTextList(discardedOrgs.ConvertAll<TIGameState>((TIOrgState x) => x), false, false);
					array3[num2] = Loc.T(text5, array4);
					notificationQueueItem3.itemDetail = Loc.T(text4, array3);
				}
			}
			notificationQueueItem.itemDetail = new StringBuilder(notificationQueueItem.itemDetail).AppendLine().AppendLine().AppendLine(org.description(true, victimFaction, false, false))
				.ToString();
			notificationQueueItem.musicIntensityDelta = 0.225f;
			notificationQueueItem.gotoGameState = ((victim != null) ? victim.ref_gameState : null) ?? org.homeRegion.ref_gameState;
			TINotificationQueueState.AddItem(notificationQueueItem, stealingFaction.IsAlienFaction);
		}

		// Token: 0x06003CDE RID: 15582 RVA: 0x00178DA8 File Offset: 0x00176FA8
		public static void LogOrgsForcedToPool(TIFactionState faction, List<TIOrgState> lostOrgs)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = lostOrgs[0].orgIconPath;
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			string text = "UI.Notifications.MyOrgsForcedToPool";
			object[] array = new object[1];
			array[0] = TIUtilities.ConstructTextList(lostOrgs.ConvertAll<TIGameState>((TIOrgState x) => x), false, false);
			notificationQueueItem2.itemSummary = Loc.T(text, array);
			notificationQueueItem.gotoGameState = faction;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CDF RID: 15583 RVA: 0x00178E44 File Offset: 0x00177044
		public static void LogOrgPoolOverfull(TIFactionState faction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = faction.unassignedOrgs[0].orgIconPath;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.OrgPoolOverfull.Hed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.OrgPoolOverfull.Summary");
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.OrgPoolOverfull.Detail", new object[] { TemplateManager.global.maxFactionOrgPoolSize.ToString("N0") });
			notificationQueueItem.gotoGameState = faction;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CE0 RID: 15584 RVA: 0x00178EEC File Offset: 0x001770EC
		public static void LogMyTechSabotaged(TIFactionState attackingFaction, TIFactionState sabotagedFaction, TIProjectTemplate project, float hate)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(sabotagedFaction);
			notificationQueueItem.relevantFactions.Add(sabotagedFaction);
			notificationQueueItem.icon = project.IconResource;
			notificationQueueItem.popupResource1 = project.IconResource;
			notificationQueueItem.popupResource2 = "councilor_missions/ICO_sabotageproject_off";
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyProjectSabotagedHed", new object[] { TIUtilities.TechCategoryLine(project.displayName, project.TechCategory) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyProjectSabotagedSummary", new object[]
			{
				(hate > 0f && attackingFaction != null) ? attackingFaction.displayNameCapitalizedWithColor : Utilities.Capitalize(Loc.T("UI.Notifications.FactionUnknownWithArticle")),
				TIUtilities.TechCategoryLine(project.displayName, project.TechCategory)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MyProjectSabotagedDetail", new object[]
			{
				(hate > 0f && attackingFaction != null) ? attackingFaction.displayNameCapitalizedWithColor : Utilities.Capitalize(Loc.T("UI.Notifications.FactionUnknownWithArticle")),
				TIUtilities.TechCategoryLine(project.displayName, project.TechCategory)
			});
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = GameStateManager.FindGameState<TIGlobalResearchState>();
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CE1 RID: 15585 RVA: 0x00179038 File Offset: 0x00177238
		public static void LogMyTechStolen(TIFactionState stealingFaction, TIFactionState victimFaction, TIProjectTemplate project, float hate)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(victimFaction);
			notificationQueueItem.relevantFactions.Add(victimFaction);
			notificationQueueItem.icon = project.IconResource;
			notificationQueueItem.popupResource1 = project.IconResource;
			notificationQueueItem.popupResource2 = "councilor_missions/ICO_stealproject_off";
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyProjectStolenHed", new object[] { TIUtilities.TechCategoryLine(project.displayName, project.TechCategory) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyProjectStolenSummary", new object[]
			{
				(hate > 0f && stealingFaction != null) ? stealingFaction.displayNameCapitalizedWithColor : Utilities.Capitalize(Loc.T("UI.Notifications.FactionUnknownWithArticle")),
				TIUtilities.TechCategoryLine(project.displayName, project.TechCategory)
			});
			string text = Loc.T("UI.Notifications.MyProjectStolenDetail", new object[]
			{
				(hate > 0f && stealingFaction != null) ? stealingFaction.displayNameCapitalizedWithColor : Utilities.Capitalize(Loc.T("UI.Notifications.FactionUnknownWithArticle")),
				TIUtilities.TechCategoryLine(project.displayName, project.TechCategory)
			});
			notificationQueueItem.itemDetail = text.ToString();
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = GameStateManager.FindGameState<TIGlobalResearchState>();
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CE2 RID: 15586 RVA: 0x00179188 File Offset: 0x00177388
		public static void LogSpyDiscovered(TICouncilorState councilor)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(councilor.faction);
			notificationQueueItem.relevantFactions.Add(councilor.faction);
			notificationQueueItem.icon = councilor.iconResource;
			notificationQueueItem.iconBackgroundResource = councilor.iconBackground;
			notificationQueueItem.backgroundColor = councilor.agentForFaction.template.color;
			notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(councilor);
			notificationQueueItem.popup1BackgroundResource = councilor.iconBackground;
			notificationQueueItem.popupResource2 = "councilor_missions/ICO_turn_off";
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.TraitorDiscoveredHed", new object[] { councilor.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.TraitorDiscoveredSummary", new object[] { councilor.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.TraitorDiscoveredDetail", new object[]
			{
				councilor.displayName,
				councilor.agentForFaction.displayNameWithColor
			});
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_spyDiscovered;
			notificationQueueItem.gotoGameState = councilor;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			TINotificationQueueState.AddItem(notificationQueueItem, councilor.agentForFaction == GameStateManager.AlienFaction());
		}

		// Token: 0x06003CE3 RID: 15587 RVA: 0x001792BC File Offset: 0x001774BC
		public static void LogSpyLost(TIFactionState losingFaction, TICouncilorState councilor, bool betraysToFaction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(losingFaction);
			notificationQueueItem.relevantFactions.Add(losingFaction);
			notificationQueueItem.icon = councilor.iconResource;
			notificationQueueItem.iconBackgroundResource = councilor.iconBackground;
			notificationQueueItem.backgroundColor = councilor.faction.template.color;
			notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(councilor);
			notificationQueueItem.popup1BackgroundResource = councilor.iconBackground;
			notificationQueueItem.popupResource2 = "councilor_missions/ICO_turn_off";
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.TurnedCouncilorLostHed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.TurnedCouncilorLostSummary", new object[] { councilor.displayName });
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.TurnedCouncilorLostDetail", new object[] { councilor.displayName }));
			if (betraysToFaction)
			{
				stringBuilder.AppendLine().AppendLine().AppendLine(Loc.T("UI.Notifications.TurnedCouncilorBetrayal"));
			}
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.TurnedCouncilorLostDetail", new object[] { councilor.displayName });
			notificationQueueItem.gotoGameState = losingFaction;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CE4 RID: 15588 RVA: 0x001793DC File Offset: 0x001775DC
		public static void LogInvoluntaryCouncilorDismissal(TICouncilorState councilor, TIFactionState faction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = councilor.iconResource;
			notificationQueueItem.iconBackgroundResource = councilor.iconBackground;
			notificationQueueItem.backgroundColor = faction.template.color;
			notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(councilor);
			notificationQueueItem.popup1BackgroundResource = councilor.iconBackground;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.TurnedCouncilorResignsHed", new object[] { councilor.displayName, faction.displayNameWithColor });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.TurnedCouncilorResignsSummary", new object[] { councilor.displayName, faction.displayNameWithColor });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.TurnedCouncilorResignsDetail", new object[] { councilor.displayName, faction.displayNameWithColor });
			notificationQueueItem.gotoGameState = faction;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CE5 RID: 15589 RVA: 0x001794D8 File Offset: 0x001776D8
		public static void LogMyControlPointCrackedDown(TIControlPoint controlPoint, TIDateTime expiry, TIFactionState crackingFaction, float hate)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(controlPoint.faction);
			notificationQueueItem.relevantFactions.Add(controlPoint.faction);
			notificationQueueItem.icon = controlPoint.nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyControlPointCrackedDownHed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyControlPointCrackedDownSummary", new object[]
			{
				controlPoint.description,
				controlPoint.nation.displayNameWithArticle
			});
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.MyControlPointCrackedDownDetail", new object[]
			{
				controlPoint.description,
				controlPoint.nation.displayNameWithArticle,
				expiry.ToCustomDateString()
			}));
			if (crackingFaction != null && crackingFaction != controlPoint.faction && hate > 0f)
			{
				stringBuilder.AppendLine().AppendLine().AppendLine(Loc.T("TIMissionResult_Responsibility", new object[] { crackingFaction.displayNameWithColor }));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.popupResource1 = controlPoint.faction.factionIcon256path;
			notificationQueueItem.gotoGameState = controlPoint;
			notificationQueueItem.musicIntensityDelta = 0.225f;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Police_Siren";
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_myControlPointCrackdown;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CE6 RID: 15590 RVA: 0x00179638 File Offset: 0x00177838
		public static void LogMyControlPointPurged(TIFactionState myFaction, TIFactionState takingFaction, TIControlPoint controlPoint, List<TIGameState> newControlPoints, List<TIGameState> oldControlPoints)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(myFaction);
			notificationQueueItem.relevantFactions.Add(myFaction);
			notificationQueueItem.icon = controlPoint.nation.flagResource;
			notificationQueueItem.popupResource1 = controlPoint.nation.flagResource;
			notificationQueueItem.popupResource2 = takingFaction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HedMyControlPointTaken");
			string text = string.Empty;
			int numArmies = controlPoint.numArmies;
			if (numArmies > 0)
			{
				text = Loc.T("UI.Notifications.ArmiesLost", new object[]
				{
					(numArmies == 1) ? Loc.T("UI.Notifications.army") : Loc.T("UI.Notifications.armies"),
					takingFaction.displayNameWithColor
				});
			}
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyControlPointTakenSummary", new object[]
			{
				controlPoint.nation.displayNameWithArticle,
				takingFaction.displayNameWithColor
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MyControlPointTaken", new object[]
			{
				controlPoint.nation.displayNameWithArticle,
				takingFaction.displayNameWithColor,
				text,
				controlPoint.description
			});
			notificationQueueItem.newControlPoints = newControlPoints;
			notificationQueueItem.oldControlPoints = oldControlPoints;
			notificationQueueItem.controlPointsRelevant = true;
			notificationQueueItem.gotoGameState = controlPoint;
			notificationQueueItem.musicIntensityDelta = 0.225f;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_myControlPointPurged;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CE7 RID: 15591 RVA: 0x0017979C File Offset: 0x0017799C
		public static void LogLoyaltySwitch(TIFactionState myFaction, TIFactionState oldFaction, TIControlPoint controlPoint, List<TIGameState> newControlPoints, List<TIGameState> oldControlPoints, TIMissionTemplate mission)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(myFaction);
			notificationQueueItem.relevantFactions.Add(myFaction);
			notificationQueueItem.icon = controlPoint.nation.flagResource;
			notificationQueueItem.popupResource1 = controlPoint.nation.flagResource;
			notificationQueueItem.popupResource2 = myFaction.factionIcon256path;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.LoyaltySwitch.Hed", new object[] { controlPoint.nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.LoyaltySwitch.Summary", new object[] { controlPoint.nation.displayNameWithArticleAndPlacePrep });
			if (mission != null && mission.dataName == TIFactionState.terrorizeMission.dataName)
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.LoyaltySwitch.TerrorDetail", new object[]
				{
					controlPoint.description,
					controlPoint.nation.displayNameWithArticle,
					myFaction.displayNameWithColor
				});
			}
			else if (mission != null && (mission.dataName == TIFactionState.enthrallElitesMission.dataName || mission.dataName == TIFactionState.enthrallNonAlignedElitesMission.dataName))
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.LoyaltySwitch.EnthrallDetail", new object[]
				{
					controlPoint.description,
					controlPoint.nation.displayNameWithArticle,
					myFaction.displayNameWithColor
				});
			}
			else
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.LoyaltySwitch.Other", new object[]
				{
					controlPoint.description,
					controlPoint.nation.displayNameWithArticleAndPlacePrep,
					myFaction.displayNameWithColor
				});
			}
			notificationQueueItem.musicIntensityDelta = 0.225f;
			notificationQueueItem.newControlPoints = newControlPoints;
			notificationQueueItem.oldControlPoints = oldControlPoints;
			notificationQueueItem.controlPointsRelevant = true;
			notificationQueueItem.gotoGameState = controlPoint;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CE8 RID: 15592 RVA: 0x00179970 File Offset: 0x00177B70
		public static void LogOurHabControlled(TIFactionState losingFaction, TIFactionState takingFaction, TIHabState hab)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = new List<TIFactionState> { losingFaction };
			notificationQueueItem.relevantFactions = new List<TIFactionState> { losingFaction };
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.OurHabControlledHed", new object[]
			{
				hab.GetDisplayName(losingFaction),
				takingFaction.displayName
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.OurHabControlledSummary", new object[]
			{
				hab.GetDisplayName(losingFaction),
				takingFaction.displayNameWithColor
			});
			if (hab.IsBase)
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.OurHabControlledDetail.Base", new object[]
				{
					hab.GetDisplayName(losingFaction),
					hab.habSite.displayName,
					hab.habSite.parentBody.displayName,
					takingFaction.displayNameWithColor
				});
			}
			else
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.OurHabControlledDetail.Station", new object[]
				{
					hab.GetDisplayName(losingFaction),
					hab.orbitState.displayName,
					takingFaction.displayNameWithColor
				});
			}
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = hab;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Seize_Space_Asset";
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CE9 RID: 15593 RVA: 0x00179AC0 File Offset: 0x00177CC0
		public static void LogDecommissionModuleComplete(TIHabModuleState module)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(module.ref_faction);
			notificationQueueItem.icon = module.moduleTemplate.iconResource(module.hab.habType);
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.HabModuleDecommissioned.Summary", new object[]
			{
				module.displayName,
				module.hab.displayName
			});
			notificationQueueItem.gotoGameState = module.hab;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CEA RID: 15594 RVA: 0x00179B4C File Offset: 0x00177D4C
		public static void LogDecommissionHabComplete(TIHabState hab)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = new List<TIFactionState>(hab.ref_factions);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HabDecommissioned.Hed", new object[] { hab.GetDisplayName(GameControl.control.activePlayer) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.HabDecommissioned.Summary", new object[] { hab.GetDisplayName(GameControl.control.activePlayer) });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.HabDecommissioned.Detail", new object[] { TIUtilities.GetLocationString(hab, true, true) });
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CEB RID: 15595 RVA: 0x00179C0C File Offset: 0x00177E0C
		public static void LogOurHabModuleDestroyed(TIHabModuleState module, TIFactionState responsibleFaction, float hate, bool displayResponsibleFaction, string nameOverride = "")
		{
			NotificationQueueItem notificationQueueItem;
			if (nameOverride == "")
			{
				notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			}
			else
			{
				notificationQueueItem = TINotificationQueueState.InitItem(nameOverride);
			}
			notificationQueueItem.primaryFactions = new List<TIFactionState> { module.sector.faction };
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.OurHabModuleDestroyedHed", new object[] { module.moduleTemplate.displayName });
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.OurHabModuleDestroyedDetail", new object[]
			{
				module.moduleTemplate.displayName,
				module.hab.GetDisplayName(module.sector.faction)
			}));
			if (responsibleFaction != null && hate > 0f && displayResponsibleFaction)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("TIMissionResult_Responsibility", new object[] { responsibleFaction.displayNameWithColor }));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.relevantFactions = new List<TIFactionState> { module.sector.faction };
			notificationQueueItem.icon = module.moduleTemplate.iconResource(module.hab.habType);
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.OurHabModuleDestroyedSummary", new object[]
			{
				module.moduleTemplate.displayName,
				module.hab.GetDisplayName(module.sector.faction)
			});
			notificationQueueItem.gotoGameState = module.hab;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.soundToPlay = TINotificationQueueState.GetRandomQuietExplosion();
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenHabManager);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CEC RID: 15596 RVA: 0x00179DB2 File Offset: 0x00177FB2
		public static void LogOurCriticalHabModuleDestroyed(TIHabModuleState module, TIFactionState responsibleFaction, float hate, bool displayResponsibleFaction)
		{
			TINotificationQueueState.LogOurHabModuleDestroyed(module, responsibleFaction, hate, displayResponsibleFaction, MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06003CED RID: 15597 RVA: 0x00179DC8 File Offset: 0x00177FC8
		public static void LogOurHabDestroyed(TIHabState hab, TIFactionState destroyingFaction, TISpaceFleetState destroyingFleet)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = new List<TIFactionState>(hab.ref_factions);
			notificationQueueItem.relevantFactions = new List<TIFactionState>(hab.ref_factions);
			notificationQueueItem.icon = hab.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.OurHabDestroyedHed", new object[]
			{
				hab.GetDisplayName(hab.coreFaction),
				destroyingFaction.displayName
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.OurHabDestroyedSummary", new object[]
			{
				hab.GetDisplayName(hab.coreFaction),
				destroyingFaction.displayNameWithColor
			});
			if (hab.IsBase)
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.OurHabDestroyedDetail.Base", new object[]
				{
					destroyingFaction.adjectiveWithColor,
					hab.GetDisplayName(hab.coreFaction),
					hab.habSite.parentBody.displayName
				});
				notificationQueueItem.gotoGameState = hab.habSite;
			}
			else
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.OurHabDestroyedDetail.Station", new object[]
				{
					destroyingFaction.adjectiveWithColor,
					hab.GetDisplayName(hab.coreFaction),
					hab.orbitState.displayName
				});
				notificationQueueItem.gotoGameState = hab.orbitState;
			}
			notificationQueueItem.musicIntensityDelta = 0.9f;
			notificationQueueItem.soundToPlay = TINotificationQueueState.GetRandomQuietExplosion();
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CEE RID: 15598 RVA: 0x00179F2C File Offset: 0x0017812C
		public static void LogOurShipChangedSides(TISpaceShipState ship, TIFactionState oldFaction, TIFactionState newFaction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(oldFaction);
			notificationQueueItem.relevantFactions.AddRange(TINotificationQueueState.AllFactionsExcept(newFaction));
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ShipStolenHeadline", new object[] { newFaction.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ShipStolenSummary", new object[] { oldFaction.adjective, ship.displayName, newFaction.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ShipStolenDetail", new object[]
			{
				ship.template.fullClassName,
				ship.displayName,
				newFaction.displayNameWithColor
			});
			notificationQueueItem.icon = ship.fleet.iconResource;
			notificationQueueItem.popupResource1 = ship.fleet.iconResource;
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.gotoGameState = ship.fleet;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Seize_Space_Asset";
			if (oldFaction != null && oldFaction == TINotificationQueueState.activePlayer)
			{
				oldFaction.UnlockAchievement("shipStolen");
			}
			else if (newFaction != null && newFaction == TINotificationQueueState.activePlayer)
			{
				newFaction.UnlockAchievement("stealShip");
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CEF RID: 15599 RVA: 0x0017A088 File Offset: 0x00178288
		public static void LogEnemyShipChangedSidesToUs(TISpaceShipState ship, TIFactionState oldFaction, TIFactionState newFaction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(newFaction);
			notificationQueueItem.relevantFactions.Add(newFaction);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ShipDefectedHeadline", new object[] { newFaction.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ShipDefectedSummary", new object[] { oldFaction.adjective, ship.displayName, newFaction.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ShipDefectedDetail", new object[]
			{
				ship.template.fullClassName,
				ship.displayName,
				newFaction.displayNameWithColor
			});
			notificationQueueItem.icon = ship.fleet.iconResource;
			notificationQueueItem.popupResource1 = ship.fleet.iconResource;
			notificationQueueItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
			notificationQueueItem.gotoGameState = ship.fleet;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Seize_Space_Asset";
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CF0 RID: 15600 RVA: 0x0017A198 File Offset: 0x00178398
		public static void LogMyHabAssaultInitiated(TIHabState hab, TIFactionState assaultingFaction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(hab.faction);
			notificationQueueItem.relevantFactions.Add(hab.faction);
			notificationQueueItem.icon = hab.ref_naturalSpaceObject.iconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyHabAssaultedHeadline", new object[] { hab.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyHabAssaultedSummary", new object[] { assaultingFaction.adjectiveWithColor, hab.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MyHabAssaultedDetail", new object[]
			{
				assaultingFaction.adjectiveWithColor,
				TIUtilities.GetLocationString(hab, true, false)
			});
			notificationQueueItem.gotoGameState = hab;
			notificationQueueItem.illustrationResource = "illustrations/Mission_SeizeSpaceAsset";
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CF1 RID: 15601 RVA: 0x0017A274 File Offset: 0x00178474
		public static void LogNewCouncilorTurn()
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = "icons_2d/ICO_clock";
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NewCouncilorTurn");
			notificationQueueItem.musicIntensityDelta = -0.225f;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CF2 RID: 15602 RVA: 0x0017A2C8 File Offset: 0x001784C8
		public static void LogEnemyCouncilorLocationDetected(TIFactionState detectingFaction, TICouncilorState detectedCouncilor)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			CouncilorView viewofCouncilor = detectingFaction.GetViewofCouncilor(detectedCouncilor);
			string locationString = TIUtilities.GetLocationString(detectedCouncilor.location, true, true);
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.popupResource1 = viewofCouncilor.genericIconResourcePath;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.HedCouncilorSpotted");
			string text;
			if (viewofCouncilor.factionCurrent != null)
			{
				text = Loc.T("UI.Notifications.EnemyCouncilorSpotted", new object[]
				{
					detectingFaction.displayNameCapitalizedWithColor,
					viewofCouncilor.factionStringCurrentKnowledge(false, false),
					locationString
				});
				notificationQueueItem.icon = viewofCouncilor.councilor.iconResource;
				NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
				TIFactionState factionCurrent = viewofCouncilor.factionCurrent;
				notificationQueueItem2.backgroundColor = ((factionCurrent != null) ? factionCurrent.template.color : Color.clear);
				notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.KnownCouncilorSpottedDetail", new object[]
				{
					detectingFaction.displayNameCapitalizedWithColor,
					viewofCouncilor.factionStringCurrentKnowledge(false, false),
					locationString
				})).ToString();
				notificationQueueItem.popupResource2 = viewofCouncilor.factionCurrent.factionIcon256path;
			}
			else
			{
				text = Loc.T("UI.Notifications.UnknownCouncilorSpotted", new object[] { detectingFaction.displayNameCapitalizedWithColor, locationString });
				notificationQueueItem.icon = viewofCouncilor.genericIconResourcePath;
				notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.UnknownCouncilorSpottedDetail", new object[] { detectingFaction.displayNameCapitalizedWithColor, locationString })).ToString();
			}
			notificationQueueItem.itemSummary = text;
			notificationQueueItem.gotoGameState = detectedCouncilor.location;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CF3 RID: 15603 RVA: 0x0017A460 File Offset: 0x00178660
		public static void LogAlienCouncilorDetected(TIFactionState detectingFaction, TICouncilorState detectedCouncilor)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions.Add(detectingFaction);
			string locationString = TIUtilities.GetLocationString(detectedCouncilor.location, true, true);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.AlienCouncilorSpottedHed");
			notificationQueueItem.primaryFactions.Add(detectingFaction);
			notificationQueueItem.icon = detectedCouncilor.iconResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.AlienCouncilorSpotted", new object[] { detectingFaction.displayNameCapitalizedWithColor, locationString });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.AlienCouncilorSpottedDetail", new object[] { detectingFaction.displayNameCapitalizedWithColor, locationString });
			notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(detectedCouncilor);
			notificationQueueItem.popupResource2 = detectedCouncilor.faction.factionIcon256path;
			notificationQueueItem.musicIntensityDelta = 0.5f;
			notificationQueueItem.gotoGameState = detectedCouncilor.location;
			TINotificationQueueState.AddItem(notificationQueueItem, true);
		}

		// Token: 0x06003CF4 RID: 15604 RVA: 0x0017A540 File Offset: 0x00178740
		public static void LogMyCouncilorDetected(TIFactionState detectingFaction, TICouncilorState detectedCouncilor)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			string locationString = TIUtilities.GetLocationString(detectedCouncilor.location, true, true);
			notificationQueueItem.relevantFactions.Add(detectedCouncilor.faction);
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyCouncilorDetectedSummary", new object[]
			{
				detectedCouncilor.faction.adjective,
				detectedCouncilor.displayName,
				locationString
			});
			notificationQueueItem.icon = detectedCouncilor.genericIconPath;
			notificationQueueItem.gotoGameState = detectedCouncilor.location;
			notificationQueueItem.popupResource1 = TINotificationQueueState.councilorGUIIconPath(detectedCouncilor);
			notificationQueueItem.primaryFactions.Add(detectedCouncilor.faction);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyCouncilorDetectedHed");
			notificationQueueItem.itemDetail = new StringBuilder(Loc.T("UI.Notifications.MyCouncilorDetectedDetail", new object[]
			{
				detectedCouncilor.faction.adjective,
				detectedCouncilor.displayName,
				locationString,
				TIFactionState.goToGroundMission.displayName
			})).ToString();
			if (detectedCouncilor.OnEarth)
			{
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_myCouncilorDetected_Earth;
			}
			string itemSummary = GameStateManager.NotificationQueue().notificationQueue[0].itemSummary;
			if (notificationQueueItem.itemSummary != itemSummary)
			{
				TINotificationQueueState.AddItem(notificationQueueItem, false);
			}
		}

		// Token: 0x06003CF5 RID: 15605 RVA: 0x0017A680 File Offset: 0x00178880
		public static void LogSpaceFacilityBombed(TIRegionSpaceFacilityState facilityState, TIFactionState responsibleParty, string newValue, float hate, int fightersDestroyed)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactionsExcept(responsibleParty);
			notificationQueueItem.primaryFactions = facilityState.region.nation.FactionsWithControlPoint;
			notificationQueueItem.icon = facilityState.GetIconResourcePath(null);
			notificationQueueItem.popupResource1 = facilityState.GetIconResourcePath(null);
			string locationString = TIUtilities.GetLocationString(facilityState.region, true, true);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.SpaceFacilitySabotagedHed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.SpaceFacilityBombed", new object[] { facilityState.displayName, locationString });
			switch (facilityState.spaceFacilityType)
			{
			case SpaceFacilityType.launchFacility:
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.BoostFacilityBombed", new object[] { facilityState.displayName, locationString, newValue });
				break;
			case SpaceFacilityType.missionControlFacility:
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MissionControlFacilityBombed", new object[] { facilityState.displayName, locationString, newValue });
				break;
			case SpaceFacilityType.spaceDefenseFacility:
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.SpaceDefenseFacilityBombed", new object[] { facilityState.displayName, locationString });
				break;
			}
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.SpaceFacilityBombed", new object[] { facilityState.displayName, locationString }));
			if (facilityState.spaceFacilityType == SpaceFacilityType.launchFacility && fightersDestroyed > 0)
			{
				if (fightersDestroyed == 1)
				{
					stringBuilder.AppendLine().Append(Loc.T("UI.Notifications.SpaceFacilityBombed_FighterLost"));
				}
				else
				{
					stringBuilder.AppendLine().Append(Loc.T("UI.Notifications.SpaceFacilityBombed_FighterLost_2"));
				}
			}
			if (responsibleParty != null && hate > 0f)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("TIMissionResult_Responsibility", new object[] { responsibleParty.displayNameWithColor }));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			notificationQueueItem.gotoGameState = facilityState;
			notificationQueueItem.musicIntensityDelta = 0.05f;
			notificationQueueItem.soundToPlay = TINotificationQueueState.GetRandomQuietExplosion();
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CF6 RID: 15606 RVA: 0x0017A874 File Offset: 0x00178A74
		public static void LogTechComplete(TIFactionState winningFaction, TITechTemplate techTemplate, int slot, bool cheat = false, string autoPick = "", string techTarget = "")
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.alertBlockFaction = winningFaction;
			notificationQueueItem.relevantFactions.Add(winningFaction);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ResearchCompleteHed", new object[] { techTemplate.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ResearchCompleteSummary", new object[] { TIUtilities.TechCategoryLine(techTemplate.displayName, techTemplate.TechCategory) });
			StringBuilder stringBuilder;
			if (string.IsNullOrEmpty(autoPick))
			{
				stringBuilder = new StringBuilder(Loc.T("UI.Notifications.ResearchCompleteDetailNoAuto", new object[]
				{
					TIUtilities.TechCategoryLine(techTemplate.displayName, techTemplate.TechCategory),
					winningFaction.displayNameCapitalizedWithColor,
					TemplateManager.global.researchInlineSpritePath,
					techTemplate.GetFullDescription(GameControl.control.activePlayer, TechBenefitsContext.JustCompleted, null, false)
				}));
			}
			else
			{
				stringBuilder = new StringBuilder(Loc.T("UI.Notifications.ResearchCompleteDetailPrompt", new object[]
				{
					TIUtilities.TechCategoryLine(techTemplate.displayName, techTemplate.TechCategory),
					winningFaction.displayNameCapitalizedWithColor,
					TemplateManager.global.researchInlineSpritePath,
					Loc.T("UI.Notifications.ResearchCompleteAutoPick", new object[]
					{
						TIUtilities.HighlightLine(autoPick),
						TIUtilities.HighlightLine(techTarget)
					}),
					techTemplate.GetFullDescription(GameControl.control.activePlayer, TechBenefitsContext.JustCompleted, null, false)
				}));
			}
			notificationQueueItem.itemDetail = stringBuilder.Replace("\r\n\r\n\r\n", "\r\n\r\n").ToString();
			notificationQueueItem.icon = techTemplate.IconResource;
			notificationQueueItem.popupResource1 = techTemplate.IconResource;
			if (!cheat || TIGlobalResearchState.CurrentResearchingTechs.Contains(techTemplate))
			{
				notificationQueueItem.alertBlockEventName = "PromptSelectTech";
				notificationQueueItem.utilityValue = slot;
			}
			notificationQueueItem.illustrationResource = techTemplate.GetCompletedIllustrationPath();
			notificationQueueItem.promptingGameState = GameStateManager.GlobalResearch();
			notificationQueueItem.gotoGameState = GameStateManager.GlobalResearch();
			if (!techTemplate.endGameTech)
			{
				notificationQueueItem.soundToPlay = new StringBuilder("event:/VO/ENG/Faction/TechQuote_").Append(techTemplate.dataName).ToString();
			}
			notificationQueueItem.customButtonTemplateName = techTemplate.dataName;
			if (techTemplate.SpaceExplorationTech())
			{
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.LaunchAllProbes);
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CF7 RID: 15607 RVA: 0x0017AA94 File Offset: 0x00178C94
		public static void LogTechCompleteAndNewTechSelected(TIFactionState winningFaction, TITechTemplate oldTechTemplate, TITechTemplate newTechTemplate)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.primaryFactions.Remove(winningFaction);
			notificationQueueItem.relevantFactions = new List<TIFactionState>(notificationQueueItem.primaryFactions);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ResearchCompleteHed", new object[] { TIUtilities.TechCategoryLine(oldTechTemplate.displayName, oldTechTemplate.TechCategory) });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ResearchCompleteSummary", new object[] { TIUtilities.TechCategoryLine(oldTechTemplate.displayName, oldTechTemplate.TechCategory) });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ResearchCompleteDetail", new object[]
			{
				TIUtilities.TechCategoryLine(oldTechTemplate.displayName, oldTechTemplate.TechCategory),
				winningFaction.displayNameCapitalizedWithColor,
				TemplateManager.global.researchInlineSpritePath,
				TIUtilities.TechCategoryLine(newTechTemplate.displayName, newTechTemplate.TechCategory),
				oldTechTemplate.GetFullDescription(GameControl.control.activePlayer, TechBenefitsContext.JustCompleted, null, false)
			});
			notificationQueueItem.icon = oldTechTemplate.IconResource;
			notificationQueueItem.popupResource1 = winningFaction.factionIcon256path;
			notificationQueueItem.popupResource2 = oldTechTemplate.IconResource;
			notificationQueueItem.illustrationResource = oldTechTemplate.GetCompletedIllustrationPath();
			notificationQueueItem.gotoGameState = GameStateManager.GlobalResearch();
			if (!oldTechTemplate.endGameTech)
			{
				notificationQueueItem.soundToPlay = new StringBuilder("event:/VO/ENG/Faction/TechQuote_").Append(oldTechTemplate.dataName).ToString();
			}
			notificationQueueItem.customButtonTemplateName = oldTechTemplate.dataName;
			if (oldTechTemplate.SpaceExplorationTech())
			{
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.LaunchAllProbes);
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CF8 RID: 15608 RVA: 0x0017AC24 File Offset: 0x00178E24
		public static void LogProjectTriggered(TIFactionState faction, TIProjectTemplate projectTemplate, bool special)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			if (!special)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ProjectTriggered.Hed");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ProjectTriggered.Summary", new object[] { TIUtilities.TechCategoryLine(projectTemplate.displayName, projectTemplate.TechCategory) });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ProjectTriggered.Detail", new object[] { TIUtilities.TechCategoryLine(projectTemplate.displayName, projectTemplate.TechCategory) });
			}
			else
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.SpecialProjectTriggered.Hed");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.SpecialProjectTriggered.Summary", new object[] { TIUtilities.TechCategoryLine(projectTemplate.displayName, projectTemplate.TechCategory) });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.SpecialProjectTriggered.Detail", new object[] { TIUtilities.TechCategoryLine(projectTemplate.displayName, projectTemplate.TechCategory) });
			}
			string text = projectTemplate.BenefitsDescription(faction, TechBenefitsContext.Prospective, null);
			if (!string.IsNullOrEmpty(text))
			{
				notificationQueueItem.itemDetail = new StringBuilder(notificationQueueItem.itemDetail).AppendLine().AppendLine().Append(text)
					.ToString();
			}
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = projectTemplate.IconResource;
			notificationQueueItem.popupResource1 = projectTemplate.IconResource;
			notificationQueueItem.popupResource2 = TemplateManager.global.pathProjectsIcon;
			notificationQueueItem.gotoGameState = GameStateManager.GlobalResearch();
			notificationQueueItem.customButtonTemplateName = projectTemplate.dataName;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.FavoriteUnlockedProject);
			if (projectTemplate.FulfillsObjective(TINotificationQueueState.activePlayer, true) == null)
			{
				notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.HideUnlockedProject);
			}
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CF9 RID: 15609 RVA: 0x0017ADCC File Offset: 0x00178FCC
		public static void LogProjectComplete(TIFactionState faction, TIProjectTemplate projectTemplate, int slot, TIOrgState orgAwarded = null, string autoPick = "", string techTarget = "")
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ProjectComplete.Hed", new object[] { projectTemplate.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ProjectComplete.Summary", new object[] { TIUtilities.TechCategoryLine(projectTemplate.displayName, projectTemplate.TechCategory) });
			string text = TIUtilities.TechCategoryLine(projectTemplate.displayName, projectTemplate.TechCategory);
			string fullDescription = projectTemplate.GetFullDescription(faction, TechBenefitsContext.JustCompleted, orgAwarded, false);
			if (string.IsNullOrEmpty(autoPick))
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ProjectComplete.Detail", new object[] { text, fullDescription });
			}
			else
			{
				string text2 = Loc.T("UI.Notifications.ResearchCompleteAutoPick", new object[]
				{
					TIUtilities.HighlightLine(autoPick),
					TIUtilities.HighlightLine(techTarget)
				});
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ProjectComplete.Detail2", new object[] { text, text2, fullDescription });
			}
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.icon = projectTemplate.IconResource;
			notificationQueueItem.popupResource1 = projectTemplate.IconResource;
			notificationQueueItem.popupResource2 = TemplateManager.global.pathProjectsIcon;
			if (faction.ProjectAllowedInSlot(slot) && string.IsNullOrEmpty(autoPick))
			{
				notificationQueueItem.alertBlockFaction = faction;
				notificationQueueItem.alertBlockEventName = "PromptSelectProject";
				notificationQueueItem.utilityValue = slot;
				notificationQueueItem.promptingGameState = faction;
			}
			notificationQueueItem.illustrationResource = projectTemplate.GetCompletedIllustrationPath();
			notificationQueueItem.gotoGameState = GameStateManager.GlobalResearch();
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.RepeatProject);
			notificationQueueItem.customButtonTemplateName = projectTemplate.dataName;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CFA RID: 15610 RVA: 0x0017AF68 File Offset: 0x00179168
		public static void LogUniqueProjectCompleteByAnotherFaction(TIFactionState faction, TIProjectTemplate projectTemplate, IEnumerable<TIFactionState> factionsAlreadyNotified)
		{
			if (TITimeState.CampaignDuration_years_Exact() == 0f)
			{
				return;
			}
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ProjectComplete.Hed", new object[] { projectTemplate.displayName });
			string text = TIUtilities.TechCategoryLine(projectTemplate.displayName, projectTemplate.TechCategory);
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ProjectComplete.Summary.OtherFaction", new object[] { faction.displayNameWithColor, text });
			string fullDescription = projectTemplate.GetFullDescription(faction, TechBenefitsContext.JustCompleted, null, false);
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ProjectComplete.Detail.OtherFaction", new object[] { faction.displayNameWithColor, text, fullDescription });
			notificationQueueItem.primaryFactions.AddRange(TINotificationQueueState.AllFactions.Where<TIFactionState>((TIFactionState x) => x != faction).Except<TIFactionState>(factionsAlreadyNotified));
			notificationQueueItem.relevantFactions.AddRange(notificationQueueItem.primaryFactions);
			notificationQueueItem.icon = projectTemplate.IconResource;
			notificationQueueItem.popupResource1 = projectTemplate.IconResource;
			notificationQueueItem.popupResource2 = TemplateManager.global.pathProjectsIcon;
			notificationQueueItem.illustrationResource = projectTemplate.GetCompletedIllustrationPath();
			notificationQueueItem.customButtonTemplateName = projectTemplate.dataName;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CFB RID: 15611 RVA: 0x0017B0B4 File Offset: 0x001792B4
		public static void LogUniqueProjectSnipedByAnotherFaction(TIFactionState completingFaction, TIFactionState losingFaction, TIProjectTemplate project, int slot)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.UniqueProjectCompletedHed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.UniqueProjectCompletedSummary", new object[]
			{
				completingFaction.displayNameCapitalizedWithColor,
				TIUtilities.TechCategoryLine(project.displayName, project.TechCategory)
			});
			notificationQueueItem.icon = project.IconResource;
			notificationQueueItem.popupResource1 = project.IconResource;
			notificationQueueItem.relevantFactions.Add(losingFaction);
			notificationQueueItem.popupResource2 = TemplateManager.global.pathProjectsIcon;
			if (slot >= 3 && slot <= 5)
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.UniqueProjectCompletedDetail", new object[]
				{
					completingFaction.displayNameCapitalizedWithColor,
					TIUtilities.TechCategoryLine(project.displayName, project.TechCategory)
				});
				notificationQueueItem.primaryFactions.Add(losingFaction);
				notificationQueueItem.alertBlockFaction = losingFaction;
				notificationQueueItem.alertBlockEventName = "PromptSelectProject";
				notificationQueueItem.utilityValue = slot;
				notificationQueueItem.promptingGameState = losingFaction;
			}
			notificationQueueItem.gotoGameState = GameStateManager.GlobalResearch();
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CFC RID: 15612 RVA: 0x0017B1C4 File Offset: 0x001793C4
		public static void LogTechWinnerWarning(TIFactionState oldExpectedWinner, TIFactionState newExpectedWinner, int techSlotIndex)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(oldExpectedWinner);
			notificationQueueItem.relevantFactions.Add(oldExpectedWinner);
			notificationQueueItem.icon = TemplateManager.global.pathResearchIcon;
			notificationQueueItem.popupResource1 = TemplateManager.global.pathResearchIcon;
			notificationQueueItem.popupResource2 = GameStateManager.GlobalResearch().GetTechProgress(techSlotIndex).techTemplate.IconResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.TechWinnerWarning.Hed", new object[]
			{
				newExpectedWinner.displayNameCapitalizedWithColor,
				TIUtilities.TechCategoryLine(GameStateManager.GlobalResearch().GetTechProgress(techSlotIndex).techTemplate.displayName, GameStateManager.GlobalResearch().GetTechProgress(techSlotIndex).techTemplate.TechCategory)
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.TechWinnerWarning.Summary", new object[]
			{
				newExpectedWinner.displayNameCapitalizedWithColor,
				oldExpectedWinner.displayNameWithColor,
				TIUtilities.TechCategoryLine(GameStateManager.GlobalResearch().GetTechProgress(techSlotIndex).techTemplate.displayName, GameStateManager.GlobalResearch().GetTechProgress(techSlotIndex).techTemplate.TechCategory)
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.TechWinnerWarning.Detail", new object[]
			{
				newExpectedWinner.displayNameCapitalizedWithColor,
				oldExpectedWinner.displayNameWithColor,
				TIUtilities.TechCategoryLine(GameStateManager.GlobalResearch().GetTechProgress(techSlotIndex).techTemplate.displayName, GameStateManager.GlobalResearch().GetTechProgress(techSlotIndex).techTemplate.TechCategory),
				newExpectedWinner.displayNameWithColor
			});
			notificationQueueItem.gotoGameState = GameStateManager.GlobalResearch();
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003CFD RID: 15613 RVA: 0x0017B354 File Offset: 0x00179554
		public static void LogFirstExecutiveControlPoint(TIControlPoint controlPoint)
		{
			if (TIGlobalValuesState.GlobalValues.tutorialMode)
			{
				NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
				notificationQueueItem.primaryFactions.Add(controlPoint.faction);
				notificationQueueItem.relevantFactions.Add(controlPoint.faction);
				notificationQueueItem.icon = controlPoint.faction.factionIcon64path;
				notificationQueueItem.popupResource1 = controlPoint.faction.factionIcon256path;
				notificationQueueItem.popupResource2 = controlPoint.nation.flagResource;
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.FirstExecutiveControlPointHed", new object[] { controlPoint.nation.displayNameWithArticleCapitalized });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.FirstExecutiveControlPointSummary", new object[] { controlPoint.nation.displayNameWithArticleCapitalized });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.FirstExecutiveControlPointDetail", new object[]
				{
					controlPoint.nation.displayNameWithArticleCapitalized,
					controlPoint.nation.modifiedConsolidatedExecControl_days
				});
			}
		}

		// Token: 0x06003CFE RID: 15614 RVA: 0x0017B454 File Offset: 0x00179654
		public static void LogControlPointAdded(TINationState nation, TIFactionState owner, TIControlPoint controlPoint, List<TIGameState> oldControlPointList)
		{
			if (owner != null)
			{
				NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
				notificationQueueItem.primaryFactions.Add(owner);
				notificationQueueItem.relevantFactions = nation.FactionsWithControlPoint;
				notificationQueueItem.icon = nation.flagResource;
				notificationQueueItem.popupResource1 = nation.flagResource;
				notificationQueueItem.popupResource2 = owner.factionIcon256path;
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ControlPointAddedHed", new object[] { nation.displayNameWithArticleAndPlacePrep });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ControlPointAddedSummary", new object[] { nation.displayNameWithArticleAndPlacePrep });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ControlPointAddedDetail", new object[] { nation.displayNameWithArticleAndPlacePrep, owner.displayNameWithColor });
				notificationQueueItem.oldControlPoints = oldControlPointList;
				notificationQueueItem.newControlPoints = nation.controlPointOwnersByPoint;
				notificationQueueItem.gotoGameState = nation;
				notificationQueueItem.controlPointsRelevant = true;
				TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
			}
		}

		// Token: 0x06003CFF RID: 15615 RVA: 0x0017B54C File Offset: 0x0017974C
		public static void LogControlPointReduction(TINationState nation, TIFactionState oldOwner, List<TIGameState> oldControlPointList)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = nation.FactionsWithControlPoint;
			if (oldOwner != null)
			{
				notificationQueueItem.primaryFactions.Add(oldOwner);
				notificationQueueItem.popupResource2 = oldOwner.factionIcon256path;
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ControlPointReduction", new object[] { nation.displayNameWithArticleAndPlacePrep, oldOwner.displayName });
			}
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ControlPointReductionHed", new object[] { nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ControlPointReductionSummary", new object[] { nation.displayNameWithArticleCapitalized });
			notificationQueueItem.oldControlPoints = oldControlPointList;
			notificationQueueItem.newControlPoints = nation.controlPointOwnersByPoint;
			notificationQueueItem.gotoGameState = nation;
			notificationQueueItem.controlPointsRelevant = true;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D00 RID: 15616 RVA: 0x0017B63C File Offset: 0x0017983C
		public static void LogRegimeChange(TINationState nation, TINationState instigatingNationState, List<TIGameState> oldControlPointList)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.popupResource2 = instigatingNationState.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.RegimeChangeHed", new object[] { nation.displayNameWithArticle });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.RegimeChangeSummary", new object[] { instigatingNationState.displayNameWithArticle, nation.displayNameWithArticle });
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.RegimeChange", new object[] { nation.displayNameWithArticle, instigatingNationState.displayNameWithArticle }));
			if (instigatingNationState.executiveFaction != null && instigatingNationState.executiveFaction == nation.executiveFaction)
			{
				stringBuilder.AppendLine(Loc.T("UI.Notifications.DIBonus", new object[] { nation.executiveFaction.displayNameWithColor }));
			}
			notificationQueueItem.oldControlPoints = oldControlPointList;
			notificationQueueItem.newControlPoints = nation.controlPointOwnersByPoint;
			notificationQueueItem.controlPointsRelevant = true;
			notificationQueueItem.musicIntensityDelta = 0.05f;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_regimeChange;
			notificationQueueItem.gotoGameState = nation;
			TINotificationQueueState.AddItem(notificationQueueItem, instigatingNationState.executiveFaction == GameStateManager.AlienFaction());
		}

		// Token: 0x06003D01 RID: 15617 RVA: 0x0017B79C File Offset: 0x0017999C
		public static void LogCoup(TINationState nationState, List<TIGameState> oldControlPointList, TIFactionState coupingFaction = null)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			List<TIFactionState> list;
			if (nationState.numControlPoints < 4)
			{
				list = oldControlPointList.Select<TIGameState, TIFactionState>((TIGameState x) => x.ref_faction).ToList<TIFactionState>();
			}
			else
			{
				list = TINotificationQueueState.AllFactions;
			}
			notificationQueueItem2.primaryFactions = list;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.CoupHead", new object[] { nationState.displayName });
			foreach (TIGameState tigameState in oldControlPointList)
			{
				if (tigameState.isFactionState && !notificationQueueItem.primaryFactions.Contains(tigameState))
				{
					notificationQueueItem.primaryFactions.Add(tigameState.ref_faction);
				}
			}
			if (coupingFaction != null)
			{
				if (!notificationQueueItem.primaryFactions.Contains(coupingFaction))
				{
					notificationQueueItem.primaryFactions.Add(coupingFaction);
				}
				notificationQueueItem.icon = nationState.flagResource;
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.CoupSummaryFaction", new object[] { coupingFaction.displayNameWithColor, nationState.displayNameWithArticle });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.CoupDetailFaction", new object[] { coupingFaction.displayNameWithColor, nationState.displayNameWithArticle });
				if (coupingFaction == GameControl.control.activePlayer)
				{
					coupingFaction.UnlockAchievement("coup");
				}
			}
			else
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.CoupSummaryOrganic", new object[] { nationState.displayNameWithArticleCapitalized });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.CoupDetailOrganic", new object[] { nationState.displayNameWithArticleCapitalized });
				notificationQueueItem.icon = nationState.flagResource;
			}
			notificationQueueItem.oldControlPoints = oldControlPointList;
			notificationQueueItem.newControlPoints = nationState.controlPointOwnersByPoint;
			notificationQueueItem.controlPointsRelevant = true;
			notificationQueueItem.musicIntensityDelta = 0.05f;
			notificationQueueItem.gotoGameState = nationState;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_coup;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Unrest_Protests";
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D02 RID: 15618 RVA: 0x0017B9C0 File Offset: 0x00179BC0
		public static void LogRevolution(TINationState nationState, List<TIGameState> oldControlPointList, bool looseNuke)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			List<TIFactionState> list;
			if (nationState.numControlPoints < 4 && !looseNuke)
			{
				list = oldControlPointList.Select<TIGameState, TIFactionState>((TIGameState x) => x.ref_faction).ToList<TIFactionState>();
			}
			else
			{
				list = TINotificationQueueState.AllFactions;
			}
			notificationQueueItem2.primaryFactions = list;
			foreach (TIGameState tigameState in oldControlPointList)
			{
				if (tigameState.isFactionState && !notificationQueueItem.primaryFactions.Contains(tigameState))
				{
					notificationQueueItem.primaryFactions.Add(tigameState.ref_faction);
				}
			}
			notificationQueueItem.icon = nationState.flagResource;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.RevolutionSummary", new object[] { nationState.displayName });
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.RevolutionDetail", new object[] { nationState.displayName }));
			if (looseNuke)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Notifications.RevolutionDetail.LooseNuke"));
			}
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.RevolutionHed", new object[] { nationState.displayName });
			notificationQueueItem.oldControlPoints = oldControlPointList;
			notificationQueueItem.newControlPoints = nationState.controlPointOwnersByPoint;
			notificationQueueItem.controlPointsRelevant = true;
			notificationQueueItem.gotoGameState = nationState;
			notificationQueueItem.musicIntensityDelta = 0.05f;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_revolution;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Unrest_Cheers";
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D03 RID: 15619 RVA: 0x0017BB60 File Offset: 0x00179D60
		public static void LogIndependence(TINationState newNation, TINationState oldNation)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = newNation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.IndependenceHed", new object[] { newNation.displayNameWithArticle });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.IndependenceSummary", new object[] { newNation.displayNameWithArticle, oldNation.displayNameWithArticle });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.IndependenceDetail", new object[] { newNation.displayNameWithArticle, oldNation.displayNameWithArticle });
			notificationQueueItem.popupResource1 = newNation.flagResource;
			notificationQueueItem.popupResource2 = oldNation.flagResource;
			notificationQueueItem.newControlPoints = newNation.controlPointOwnersByPoint;
			notificationQueueItem.controlPointsRelevant = true;
			notificationQueueItem.gotoGameState = newNation;
			notificationQueueItem.musicIntensityDelta = 0.05f;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_independence;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Unrest_Cheers";
			TINotificationQueueState.AddItem(notificationQueueItem, oldNation == GameStateManager.AlienNation() || newNation == GameStateManager.AlienNation());
		}

		// Token: 0x06003D04 RID: 15620 RVA: 0x0017BC88 File Offset: 0x00179E88
		public static void LogNationGainsSpaceProgram(TINationState nation)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = nation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.popupResource2 = TemplateManager.global.pathGeoscapeLaunchSite1;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.NewSpaceProgram.Hed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NewSpaceProgram.Summary", new object[] { nation.displayNameWithArticleCapitalized });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.NewSpaceProgram.Detail", new object[] { nation.displayNameWithArticleCapitalized });
			notificationQueueItem.gotoGameState = nation;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_spaceProgram;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
		}

		// Token: 0x06003D05 RID: 15621 RVA: 0x0017BD64 File Offset: 0x00179F64
		public static void LogNationGainsNukes(TINationState nation)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.popupResource2 = TemplateManager.global.pathNukesIcon;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.NewNukes.Hed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NewNukes.Summary", new object[] { nation.displayNameWithArticleCapitalized });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.NewNukes.Detail", new object[]
			{
				nation.displayNameWithArticleCapitalized,
				TemplateManager.global.nukesInlineSpritePath
			});
			notificationQueueItem.gotoGameState = nation;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_nuclearProgram;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
		}

		// Token: 0x06003D06 RID: 15622 RVA: 0x0017BE58 File Offset: 0x0017A058
		public static void LogNationGainsCoreEcoRegion(TINationState nation, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = nation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.NationGainsCoreEcoRegion.Hed", new object[] { region.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NationGainsCoreEcoRegion.Summary", new object[] { region.displayName, nation.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.NationGainsCoreEcoRegion.Detail", new object[] { region.displayName, nation.displayName });
			notificationQueueItem.gotoGameState = region;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
		}

		// Token: 0x06003D07 RID: 15623 RVA: 0x0017BF34 File Offset: 0x0017A134
		public static void LogNationGainsCoreMineralRegion(TINationState nation, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = nation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.NationGainsCoreMineralRegion.Hed", new object[] { region.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NationGainsCoreMineralRegion.Summary", new object[] { region.displayName, nation.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.NationGainsCoreMineralRegion.Detail", new object[] { region.displayName, nation.displayName });
			notificationQueueItem.gotoGameState = region;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
		}

		// Token: 0x06003D08 RID: 15624 RVA: 0x0017C010 File Offset: 0x0017A210
		public static void LogNationGainsCoreOilRegion(TINationState nation, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = nation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.NationGainsCoreOilRegion.Hed", new object[] { region.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NationGainsCoreOilRegion.Summary", new object[] { region.displayName, nation.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.NationGainsCoreOilRegion.Detail", new object[] { region.displayName, nation.displayName });
			notificationQueueItem.gotoGameState = region;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
		}

		// Token: 0x06003D09 RID: 15625 RVA: 0x0017C0EC File Offset: 0x0017A2EC
		public static void LogDecolonizeComplete(TINationState nation, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = nation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.DecolonizeComplete.Hed", new object[] { region.displayName, nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.DecolonizeComplete.Summary", new object[] { region.displayName, nation.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.DecolonizeComplete.Detail", new object[] { region.displayName, nation.displayName });
			notificationQueueItem.gotoGameState = region;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
		}

		// Token: 0x06003D0A RID: 15626 RVA: 0x0017C1D0 File Offset: 0x0017A3D0
		public static void LogDecontaminateComplete(TINationState nation, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = nation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.DecontaminateComplete.Hed", new object[] { region.displayName, nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.DecontaminateComplete.Summary", new object[]
			{
				TIUtilities.GetStateDisplayName(region, null, true, false, false, false, true),
				nation.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.DecontaminateComplete.Detail", new object[]
			{
				TIUtilities.GetStateDisplayName(region, null, true, false, false, false, true),
				nation.displayName
			});
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			notificationQueueItem.gotoGameState = region;
			TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
		}

		// Token: 0x06003D0B RID: 15627 RVA: 0x0017C2C0 File Offset: 0x0017A4C0
		public static void LogLegitimizeClaimComplete(TIRegionState region)
		{
			TINationState nation = region.nation;
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = nation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = new List<TIFactionState>(notificationQueueItem.primaryFactions);
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.LegitimizeComplete.Hed", new object[] { nation.nationalAdjective, region.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.LegitimizeComplete.Summary", new object[] { nation.nationalAdjective, region.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.LegitimizeComplete.Detail", new object[] { nation.nationalAdjective, region.displayName });
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			notificationQueueItem.gotoGameState = region;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D0C RID: 15628 RVA: 0x0017C3AC File Offset: 0x0017A5AC
		public static void LogMilitaryFounded(TINationState nation)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = nation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MilitaryFounded.Hed", new object[] { nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MilitaryFounded.Summary", new object[] { nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MilitaryFounded.Detail", new object[] { nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			notificationQueueItem.gotoGameState = nation;
			TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
		}

		// Token: 0x06003D0D RID: 15629 RVA: 0x0017C478 File Offset: 0x0017A678
		public static void LogSTOFighterComplete(TINationState nation, TILaunchFacilityState site)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = nation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = nation.flagResource;
			notificationQueueItem.popupResource1 = nation.flagResource;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.STOFighterComplete.Hed", new object[] { nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.STOFighterComplete.Summary", new object[] { nation.displayNameWithArticleAndPlacePrep });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.STOFighterComplete.Detail", new object[]
			{
				site.displayName,
				site.region.displayNameSentIn,
				nation.displayName
			});
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			notificationQueueItem.gotoGameState = site;
			TINotificationQueueState.AddItem(notificationQueueItem, nation.alienNation);
		}

		// Token: 0x06003D0E RID: 15630 RVA: 0x0017C558 File Offset: 0x0017A758
		public static void LogPolicyDeclined(TIPolicyOption policy, TINationState adoptingNation, TINationState targetNation)
		{
			if (adoptingNation.executiveFaction == null && targetNation.executiveFaction == null)
			{
				return;
			}
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			if (adoptingNation.executiveFaction != null)
			{
				notificationQueueItem.primaryFactions.Add(adoptingNation.executiveFaction);
				notificationQueueItem.relevantFactions.Add(adoptingNation.executiveFaction);
			}
			if (targetNation.executiveFaction != null && targetNation.executiveFaction != adoptingNation.executiveFaction)
			{
				notificationQueueItem.relevantFactions.Add(targetNation.executiveFaction);
			}
			notificationQueueItem.icon = adoptingNation.flagResource;
			notificationQueueItem.popupResource1 = adoptingNation.flagResource;
			notificationQueueItem.popupResource2 = targetNation.flagResource;
			notificationQueueItem.gotoGameState = adoptingNation;
			notificationQueueItem.itemHeadline = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Decline.Hed").ToString());
			string displayNameWithArticleCapitalized = targetNation.displayNameWithArticleCapitalized;
			string text = adoptingNation.displayNameWithArticle;
			if (policy is JoinFederationOption)
			{
				if (adoptingNation.inFederation)
				{
					text = adoptingNation.federation.displayNameWithArticle;
					notificationQueueItem.icon = targetNation.flagResource;
					notificationQueueItem.popupResource1 = targetNation.flagResource;
					notificationQueueItem.popupResource2 = adoptingNation.federation.flagResource;
				}
				else if (targetNation.inFederation)
				{
					text = targetNation.federation.displayNameWithArticle;
					notificationQueueItem.popupResource2 = targetNation.federation.flagResource;
				}
			}
			notificationQueueItem.itemSummary = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Decline").ToString(), new object[] { displayNameWithArticleCapitalized, text });
			notificationQueueItem.itemDetail = notificationQueueItem.itemSummary;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D0F RID: 15631 RVA: 0x0017C724 File Offset: 0x0017A924
		public static void LogPolicyAdopted(TIPolicyOption policy, TINationState adoptingNation, TIGameState target = null, TIGameState relatedGameState = null, int importance = 1, string overrideString = "", string overrideArt = "")
		{
			if (importance < 0)
			{
				return;
			}
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.itemHeadline = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Hed").ToString());
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			TINationState tinationState = ((target is TIWarState) ? (target as TIWarState).EnemyWarLeader(adoptingNation, true) : target.ref_nation);
			TIArmyState ref_army = target.ref_army;
			TIRegionState ref_region = target.ref_region;
			if (importance == 2)
			{
				notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
				notificationQueueItem.musicIntensityDelta = 1f;
			}
			else
			{
				if (importance == 1)
				{
					notificationQueueItem.primaryFactions = adoptingNation.FactionsWithControlPoint;
					if (!(tinationState != null))
					{
						goto IL_0149;
					}
					using (List<TIFactionState>.Enumerator enumerator = tinationState.FactionsWithControlPoint.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIFactionState tifactionState = enumerator.Current;
							if (!notificationQueueItem.primaryFactions.Contains(tifactionState))
							{
								notificationQueueItem.primaryFactions.Add(tifactionState);
							}
						}
						goto IL_0149;
					}
				}
				notificationQueueItem.primaryFactions.Add(adoptingNation.executiveFaction);
				if (tinationState != null && tinationState.executiveFaction != adoptingNation.executiveFaction)
				{
					notificationQueueItem.primaryFactions.Add(tinationState.executiveFaction);
				}
			}
			IL_0149:
			notificationQueueItem.gotoGameState = adoptingNation;
			notificationQueueItem.icon = adoptingNation.flagResource;
			notificationQueueItem.popupResource1 = adoptingNation.flagResource;
			if (tinationState != null)
			{
				notificationQueueItem.popupResource2 = tinationState.flagResource;
			}
			else if (ref_army != null)
			{
				notificationQueueItem.popupResource2 = ref_army.GetIconForegroundResource;
			}
			else if (policy is WarOption)
			{
				notificationQueueItem.popupResource2 = ((adoptingNation.numNuclearWeapons > 0) ? TemplateManager.global.pathNukesIcon : TemplateManager.global.pathWarIcon);
			}
			string text = string.Empty;
			string text2 = string.Empty;
			if (policy is JoinFederationOption)
			{
				if (adoptingNation.federation == null)
				{
					Log.Error("Tried to add a nation to a federation it wasn't elible for", Array.Empty<object>());
					return;
				}
				text2 = adoptingNation.federation.displayNameWithArticleCapitalized;
				if (adoptingNation.federation.leadNation == adoptingNation)
				{
					text = tinationState.displayNameWithArticleCapitalized;
					notificationQueueItem.icon = tinationState.flagResource;
					notificationQueueItem.popupResource1 = tinationState.flagResource;
				}
				else
				{
					text = adoptingNation.displayNameWithArticleCapitalized;
				}
				notificationQueueItem.popupResource2 = adoptingNation.federation.flagResource;
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_federation;
			}
			else if (policy is LeaveFederationOption)
			{
				if (adoptingNation.inFederation)
				{
					text = adoptingNation.displayNameWithArticleCapitalized;
					text2 = tinationState.federation.displayNameWithArticle;
					notificationQueueItem.icon = adoptingNation.flagResource;
					notificationQueueItem.popupResource1 = adoptingNation.flagResource;
					notificationQueueItem.popupResource2 = tinationState.federation.flagResource;
				}
				else
				{
					text = adoptingNation.displayNameWithArticleCapitalized;
					text2 = tinationState.federation.displayNameWithArticle;
					notificationQueueItem.popupResource2 = tinationState.federation.flagResource;
				}
			}
			else
			{
				text = adoptingNation.displayNameWithArticleCapitalized;
				if (target != null)
				{
					if (policy is UnificationOption && !string.IsNullOrEmpty(overrideString))
					{
						text2 = overrideString;
						notificationQueueItem.popupResource2 = overrideArt;
					}
					else
					{
						text2 = TIUtilities.GetStateDisplayName(target, null, false, false, true, false, true);
					}
				}
			}
			if (policy is DisarmNuclearWeaponsOption && adoptingNation.numNuclearWeapons == 0)
			{
				notificationQueueItem.itemSummary = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Summary2").ToString(), new object[] { text, text2 });
			}
			else
			{
				notificationQueueItem.itemSummary = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Summary").ToString(), new object[] { text, text2 });
			}
			StringBuilder stringBuilder = new StringBuilder(text);
			StringBuilder stringBuilder2 = new StringBuilder(text2);
			if (adoptingNation.executiveFaction != null)
			{
				stringBuilder.Append(Loc.T("UI.Notifications.ControlNote", new object[] { adoptingNation.executiveFaction.displayNameWithColor }));
			}
			if (policy is WarOption)
			{
				TIWarState tiwarState = target.ref_war;
				if (tiwarState != null)
				{
					notificationQueueItem.itemSummary = Loc.T("UI.Notifications.WarOptionJoin.Summary", new object[]
					{
						adoptingNation.displayNameWithArticleCapitalized,
						tiwarState.displayNameWithArticle,
						tiwarState.EnemyWarLeader(adoptingNation, false).displayNameWithArticle
					});
					notificationQueueItem.itemDetail = Loc.T("UI.Notifications.WarOptionJoin.Detail", new object[]
					{
						adoptingNation.displayNameWithArticleCapitalized,
						tiwarState.displayNameWithArticle,
						tiwarState.EnemyWarLeader(adoptingNation, false).displayNameWithArticle
					});
					notificationQueueItem.musicIntensityDelta = 0.1485f;
				}
				else
				{
					tiwarState = GameStateManager.GlobalValues().FindWarByInitiators(adoptingNation, tinationState);
					List<TINationState> list = new List<TINationState>(tiwarState.Alliance(adoptingNation));
					list.Remove(adoptingNation);
					List<TINationState> list2 = new List<TINationState>(tiwarState.Alliance(tinationState));
					StringBuilder stringBuilder3 = new StringBuilder();
					if (list.Count > 0)
					{
						StringBuilder stringBuilder4 = stringBuilder;
						string text3 = "UI.Notifications.JoiningAllies";
						object[] array = new object[1];
						array[0] = TIUtilities.ConstructTextList(list.ConvertAll<TIGameState>((TINationState x) => x), false, false);
						stringBuilder4.Append(Loc.T(text3, array));
						stringBuilder3.Append(Loc.T("UI.Notifications.AllianceLeaderAttack", new object[] { tiwarState.AllianceWarLeader(adoptingNation).displayNameWithArticleCapitalized }));
					}
					if (list2.Count > 1)
					{
						stringBuilder2 = new StringBuilder(TIUtilities.ConstructTextList(list2.ConvertAll<TIGameState>((TINationState x) => x), false, false));
						stringBuilder3.Append(Loc.T("UI.Notifications.AllianceLeaderDefend", new object[] { tiwarState.AllianceWarLeader(tinationState).displayNameWithArticle }));
					}
					notificationQueueItem.musicIntensityDelta = 0.45f;
					string text4 = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Detail").ToString(), new object[]
					{
						stringBuilder.ToString(),
						stringBuilder2.ToString(),
						stringBuilder3.ToString()
					});
					if (list.Contains(GameStateManager.AlienNation()) || list2.Contains(GameStateManager.AlienNation()))
					{
						text4 = new StringBuilder(text4).AppendLine().AppendLine().Append(Loc.T("UI.Notifications.AlienNationWarRules", new object[] { GameStateManager.AlienNation().displayNameWithArticleCapitalized }))
							.ToString();
					}
					notificationQueueItem.fanfareToPlay = "event:/Music/Fanfares/trig_Warfare";
					notificationQueueItem.itemDetail = text4;
					notificationQueueItem.illustrationResource = TemplateManager.global.illus_war;
				}
			}
			else if (policy is DisarmNuclearWeaponsOption && adoptingNation.numNuclearWeapons == 0)
			{
				notificationQueueItem.itemDetail = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Detail2").ToString(), new object[]
				{
					stringBuilder.ToString(),
					stringBuilder2.ToString()
				});
			}
			else if (policy is EndWarOption)
			{
				TIWarState tiwarState2 = relatedGameState as TIWarState;
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_peace;
				notificationQueueItem.musicIntensityDelta = -0.45f;
				if (tiwarState2.AllianceWarLeader(adoptingNation) == adoptingNation)
				{
					notificationQueueItem.itemDetail = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".DetailTotal").ToString(), new object[]
					{
						stringBuilder.ToString(),
						stringBuilder2.ToString(),
						tiwarState2.displayName
					});
				}
				else
				{
					notificationQueueItem.itemDetail = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".DetailSeparate").ToString(), new object[]
					{
						stringBuilder.ToString(),
						stringBuilder2.ToString(),
						tiwarState2.displayName
					});
				}
			}
			else if (policy is EmployNuclearWeaponsOption && !adoptingNation.enemies.Contains(tinationState))
			{
				notificationQueueItem.itemDetail = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Detail2").ToString(), new object[]
				{
					stringBuilder.ToString(),
					stringBuilder2.ToString()
				});
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_nuclearWeaponsLaunch;
				notificationQueueItem.musicIntensityDelta = 0.9f;
			}
			else if (policy is InitiateRivalryOption && adoptingNation.CohesionLossFromDeclaringWar(tinationState) > 0f)
			{
				StringBuilder stringBuilder5 = new StringBuilder(Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Detail").ToString(), new object[]
				{
					stringBuilder.ToString(),
					stringBuilder2.ToString()
				})).Append(Loc.T("UI.Notifications.InitiateRivalryOption.CohesionHit", new object[]
				{
					adoptingNation.displayNameWithArticleCapitalized,
					adoptingNation.rivalryCooldowns[tinationState].ToCustomDateString()
				}));
				notificationQueueItem.itemDetail = stringBuilder5.ToString();
				notificationQueueItem.musicIntensityDelta = 0.9f;
			}
			else if (policy is TransferRegionsOption)
			{
				notificationQueueItem.itemDetail = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Detail").ToString(), new object[]
				{
					stringBuilder.ToString(),
					target.ref_region.displayNameSentOf,
					relatedGameState.ref_nation.displayNameWithArticle
				});
				notificationQueueItem.musicIntensityDelta = 0.45f;
			}
			else
			{
				notificationQueueItem.itemDetail = Loc.T(new StringBuilder("UI.Notifications.").Append(policy.GetType().ToString()).Append(".Detail").ToString(), new object[]
				{
					stringBuilder.ToString(),
					stringBuilder2.ToString(),
					text,
					((relatedGameState != null) ? relatedGameState.GetDisplayName(TINotificationQueueState.activePlayer) : null) ?? string.Empty
				});
				notificationQueueItem.musicIntensityDelta = 0.9f;
			}
			if (policy.ImprovesRelations() && adoptingNation.extant && !(policy is UnificationOption) && tinationState != null && tinationState.extant && adoptingNation.improveRelationsCooldowns.ContainsKey(tinationState) && adoptingNation.improveRelationsCooldowns[tinationState] > TITimeState.Now())
			{
				notificationQueueItem.itemDetail = new StringBuilder(notificationQueueItem.itemDetail).AppendLine().AppendLine().AppendLine(Loc.T("UI.Notifications.RelationsCooldown", new object[] { adoptingNation.improveRelationsCooldowns[tinationState].ToCustomDateString() }))
					.ToString();
			}
			if (policy is UnificationOption)
			{
				notificationQueueItem.controlPointsRelevant = true;
				notificationQueueItem.newControlPoints = (adoptingNation.extant ? adoptingNation.controlPointOwnersByPoint : tinationState.controlPointOwnersByPoint);
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_unification;
			}
			if (policy is PeacefulBreakupOption)
			{
				notificationQueueItem.controlPointsRelevant = true;
				notificationQueueItem.newControlPoints = tinationState.controlPointOwnersByPoint;
				notificationQueueItem.icon = tinationState.flagResource;
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_independence;
			}
			TINotificationQueueState.AddItem(notificationQueueItem, adoptingNation.alienNation);
		}

		// Token: 0x06003D10 RID: 15632 RVA: 0x0017D274 File Offset: 0x0017B474
		public static void LogRegionChangesHands(TIRegionState region, TINationState oldNation, List<TIGameState> newNationOldControlPoints)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = region.nation.FactionsWithControlPoint.Union<TIFactionState>(oldNation.FactionsWithControlPoint).ToList<TIFactionState>();
			notificationQueueItem.icon = region.nation.flagResource;
			notificationQueueItem.popupResource1 = region.nation.flagResource;
			notificationQueueItem.popupResource2 = oldNation.flagResource;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.gotoGameState = region;
			if (newNationOldControlPoints == region.nation.controlPointOwnersByPoint)
			{
				notificationQueueItem.controlPointsRelevant = true;
				notificationQueueItem.oldControlPoints = newNationOldControlPoints;
				notificationQueueItem.newControlPoints = region.nation.controlPointOwnersByPoint;
			}
			bool flag = region.nation.hostileClaims.Contains(region);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.RegionChangesHandsHed", new object[]
			{
				region.nation.displayName,
				region.displayName
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.RegionChangesHandsSummary", new object[]
			{
				region.nation.displayNameWithArticleCapitalized,
				region.displayName,
				oldNation.displayNameWithArticle
			});
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Notifications.RegionChangesHandsDetail", new object[]
			{
				region.nation.displayNameWithArticleCapitalized,
				region.displayName,
				oldNation.displayNameWithArticle,
				oldNation.displayNameWithArticleCapitalized
			}));
			if (flag)
			{
				stringBuilder.Append(Loc.T("UI.Notifications.RegionChangesHandsHostileWarning", new object[]
				{
					TemplateManager.global.cohesionInlineSpritePath,
					TemplateManager.global.unrestInlineSpritePath
				}));
			}
			notificationQueueItem.itemDetail = stringBuilder.ToString();
			TINotificationQueueState.AddItem(notificationQueueItem, region.nation.alienNation || oldNation.alienNation);
		}

		// Token: 0x06003D11 RID: 15633 RVA: 0x0017D430 File Offset: 0x0017B630
		public static void LogNationsCPPrioritiesReset(TIControlPoint controlPoint, PriorityType priority)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			if (controlPoint.faction != null)
			{
				notificationQueueItem.primaryFactions.Add(controlPoint.faction);
			}
			if (notificationQueueItem.primaryFactions.Count > 0)
			{
				notificationQueueItem.relevantFactions = notificationQueueItem.primaryFactions;
				notificationQueueItem.icon = controlPoint.nation.flagResource;
				notificationQueueItem.popupResource1 = controlPoint.nation.flagResource;
				notificationQueueItem.gotoGameState = controlPoint.nation;
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NationsCPPrioritiesReset.Summary", new object[]
				{
					controlPoint.description,
					controlPoint.nation.displayNameWithArticleAndPlacePrep,
					TIUtilities.GetPriorityString(priority, true)
				});
				notificationQueueItem.itemDetail = notificationQueueItem.itemSummary;
				TINotificationQueueState.AddItem(notificationQueueItem, false);
			}
		}

		// Token: 0x06003D12 RID: 15634 RVA: 0x0017D504 File Offset: 0x0017B704
		public static void LogNationCompletesNuke(TINationState nation)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(nation.FactionsWithControlPoint);
			if (notificationQueueItem.primaryFactions.Count > 0)
			{
				notificationQueueItem.relevantFactions = notificationQueueItem.primaryFactions;
				notificationQueueItem.icon = TIGlobalConfig.globalConfig.pathNukesIcon;
				notificationQueueItem.popupResource1 = nation.flagResource;
				notificationQueueItem.gotoGameState = nation;
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.NationCompletesNuke.Hed", new object[] { nation.displayName });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NationCompletesNuke.Summary", new object[] { nation.displayNameWithArticleCapitalized });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.NationCompletesNuke.Detail", new object[]
				{
					nation.displayNameWithArticleCapitalized,
					TIGlobalConfig.globalConfig.nukesInlineSpritePath,
					nation.numNuclearWeapons
				});
				TINotificationQueueState.AddItem(notificationQueueItem, false);
			}
		}

		// Token: 0x06003D13 RID: 15635 RVA: 0x0017D5F4 File Offset: 0x0017B7F4
		public static void LogNationsGainClaims(Dictionary<TINationState, List<TIRegionState>> newClaims, TIFactionState excludeFaction)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(GameStateManager.AllHumanFactions());
			if (excludeFaction != null)
			{
				notificationQueueItem.primaryFactions.Remove(excludeFaction);
			}
			notificationQueueItem.relevantFactions = notificationQueueItem.primaryFactions;
			notificationQueueItem.icon = newClaims.Keys.First<TINationState>().flagResource;
			notificationQueueItem.gotoGameState = newClaims.Keys.First<TINationState>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TINationState tinationState in newClaims.Keys)
			{
				List<string> list = new List<string>();
				foreach (TIRegionState tiregionState in newClaims[tinationState])
				{
					list.Add(tiregionState.displayName);
				}
				stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksClaim", new object[]
				{
					tinationState.displayNameWithArticleCapitalized,
					TIUtilities.ConstructTextList(list, false, false)
				}));
			}
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.LogNationsGainClaims.Hed");
			if (newClaims.Keys.Count == 1)
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.LogNationsGainClaims.Summary_1", new object[] { newClaims.Keys.First<TINationState>().displayNameWithArticleCapitalized });
			}
			else
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.LogNationsGainClaims.Summary_2");
			}
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.LogNationsGainClaims.Detail", new object[] { stringBuilder.ToString() });
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D14 RID: 15636 RVA: 0x0017D7B0 File Offset: 0x0017B9B0
		public static void LogMyArmyBadlyDamaged(TIArmyState army)
		{
			if (army.faction != null)
			{
				NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
				notificationQueueItem.primaryFactions.Add(army.faction);
				notificationQueueItem.relevantFactions.Add(army.faction);
				notificationQueueItem.icon = army.GetIconForegroundResource;
				notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
				notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
				notificationQueueItem.gotoGameState = army;
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MyArmyBadlyDamaged.Hed", new object[] { army.displayName });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MyArmyBadlyDamaged.Summary", new object[]
				{
					army.displayNameWithArticleCapitalized,
					army.currentRegion.displayNameSentIn
				});
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MyArmyBadlyDamaged.Detail", new object[]
				{
					army.displayNameWithArticleCapitalized,
					army.currentRegion.displayNameSentIn
				});
				TINotificationQueueState.AddItem(notificationQueueItem, false);
			}
		}

		// Token: 0x06003D15 RID: 15637 RVA: 0x0017D8AC File Offset: 0x0017BAAC
		public static void LogEnemyArmyJoinsBattle(TIArmyState arrivingEnemyArmy)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			List<TIArmyState> enemyArmiesInRegion = arrivingEnemyArmy.GetEnemyArmiesInRegion();
			notificationQueueItem.primaryFactions = (from x in enemyArmiesInRegion
				where x.faction != null
				select x.faction).Distinct<TIFactionState>().ToList<TIFactionState>();
			if (notificationQueueItem.primaryFactions.Count > 0)
			{
				notificationQueueItem.relevantFactions = notificationQueueItem.primaryFactions;
				notificationQueueItem.icon = arrivingEnemyArmy.GetIconForegroundResource;
				notificationQueueItem.iconBackgroundResource = arrivingEnemyArmy.GetIconBackgroundResource;
				notificationQueueItem.backgroundColor = arrivingEnemyArmy.GetIconBackgroundResourceColor;
				notificationQueueItem.illustrationResource = arrivingEnemyArmy.illustration;
				notificationQueueItem.gotoGameState = arrivingEnemyArmy;
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.EnemyArmyJoinsBattle.Hed", new object[] { arrivingEnemyArmy.currentRegion.displayNameSentIn });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.EnemyArmyJoinsBattle.Summary", new object[]
				{
					arrivingEnemyArmy.displayNameWithArticleCapitalized,
					arrivingEnemyArmy.currentRegion.displayNameSentIn
				});
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.EnemyArmyJoinsBattle.Detail", new object[]
				{
					arrivingEnemyArmy.displayNameWithArticleCapitalized,
					arrivingEnemyArmy.currentRegion.displayNameSentIn
				});
				TINotificationQueueState.AddItem(notificationQueueItem, false);
			}
		}

		// Token: 0x06003D16 RID: 15638 RVA: 0x0017DA04 File Offset: 0x0017BC04
		public static void LogNationJoinsWar(TIWarState war, TINationState joiner)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = new List<TIFactionState>(war.ref_factions);
			notificationQueueItem.relevantFactions = new List<TIFactionState>(war.ref_factions);
			notificationQueueItem.icon = joiner.flagResource;
			notificationQueueItem.gotoGameState = joiner;
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NationJoinsWar.Summary", new object[]
			{
				joiner.displayNameWithArticleCapitalized,
				war.displayNameWithArticle,
				war.EnemyWarLeader(joiner, false).displayNameWithArticle
			});
			TINotificationQueueState.AddItem(notificationQueueItem, joiner.alienNation);
		}

		// Token: 0x06003D17 RID: 15639 RVA: 0x0017DA9C File Offset: 0x0017BC9C
		public static void LogArmyLaunchesTowardEnemyRegion(TIArmyState army, TIRegionState region)
		{
			if (region.nation.wars.Contains(army.homeNation) || (army.AlienMegafaunaArmy && !region.nation.alienNation))
			{
				List<TIFactionState> factionsWithControlPoint = region.nation.FactionsWithControlPoint;
				factionsWithControlPoint.Remove(army.faction);
				if (factionsWithControlPoint.Count > 0)
				{
					NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
					notificationQueueItem.primaryFactions = factionsWithControlPoint;
					notificationQueueItem.relevantFactions = factionsWithControlPoint;
					notificationQueueItem.icon = army.GetIconForegroundResource;
					notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
					notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
					notificationQueueItem.illustrationResource = army.illustration;
					string text = army.CurrentOperations().First<OperationData>((OperationData x) => x.operation is DeployArmyOperation).completionDate.ToCustomDateString();
					if (army.AlienMegafaunaArmy)
					{
						notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.LogMegafaunaLaunchesTowardEnemyRegionHed", new object[] { region.displayName });
						notificationQueueItem.itemSummary = Loc.T("UI.Notifications.LogMegafaunaLaunchesTowardEnemyRegionSummary", new object[] { army.displayName, region.displayName });
						notificationQueueItem.itemDetail = Loc.T("UI.Notifications.LogMegafaunaLaunchesTowardEnemyRegionDetail", new object[] { army.displayName, region.displayName, text });
					}
					else
					{
						notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.LogArmyLaunchesTowardEnemyRegionHed", new object[]
						{
							army.homeNation.nationalAdjective,
							region.displayName
						});
						notificationQueueItem.itemSummary = Loc.T("UI.Notifications.LogArmyLaunchesTowardEnemyRegionSummary", new object[]
						{
							army.homeNation.displayNameWithArticle,
							region.displayName
						});
						notificationQueueItem.itemDetail = Loc.T("UI.Notifications.LogArmyLaunchesTowardEnemyRegionDetail", new object[]
						{
							army.displayNameWithArticleCapitalized,
							region.displayName,
							army.homeNation.nationalAdjective,
							text
						});
					}
					notificationQueueItem.gotoGameState = army;
					TINotificationQueueState.AddItem(notificationQueueItem, army.homeNation.alienNation);
				}
			}
		}

		// Token: 0x06003D18 RID: 15640 RVA: 0x0017DCB0 File Offset: 0x0017BEB0
		public static void LogArmyArrivesInRegion(TIArmyState army, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			if (army.faction != null)
			{
				if (!army.huntingXenofauna || army.faction.KnownXenoforming.Contains(region.xenoforming))
				{
					notificationQueueItem.primaryFactions.Add(army.faction);
				}
				notificationQueueItem.relevantFactions.Add(army.faction);
			}
			if (region.nation.wars.Contains(army.homeNation))
			{
				foreach (TIFactionState tifactionState in region.nation.FactionsWithControlPoint)
				{
					notificationQueueItem.relevantFactions.Add(tifactionState);
				}
				List<TIArmyState> list = region.FilteredArmiesPresent(true, false, false, false, true);
				if (list.Count > 0)
				{
					foreach (TIArmyState tiarmyState in list)
					{
						if (!(tiarmyState.faction == null) && !notificationQueueItem.relevantFactions.Contains(tiarmyState.faction))
						{
							notificationQueueItem.relevantFactions.Add(tiarmyState.faction);
						}
					}
					notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyArrivesDetailCombat", new object[]
					{
						army.displayNameWithArticleCapitalized,
						army.currentRegion.displayName
					});
				}
				else if (!region.IsFullyOccupied())
				{
					notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyArrivesDetailOccupation", new object[]
					{
						army.displayNameWithArticleCapitalized,
						army.currentRegion.displayName
					});
				}
				else
				{
					notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyArrivesDetailSafe", new object[]
					{
						army.displayNameWithArticleCapitalized,
						army.currentRegion.displayName
					});
				}
			}
			else
			{
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyArrivesDetailSafe", new object[]
				{
					army.displayNameWithArticleCapitalized,
					army.currentRegion.displayName
				});
			}
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyArrivesSummary", new object[]
			{
				army.displayNameWithArticleCapitalized,
				army.currentRegion.displayName
			});
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyArrivesHed");
			notificationQueueItem.gotoGameState = army;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D19 RID: 15641 RVA: 0x0017DF44 File Offset: 0x0017C144
		public static void LogArmyTeleportedToLegalRegion(TIArmyState army, TIRegionState badRegion, TIRegionState newRegion)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			notificationQueueItem.primaryFactions.Add(army.faction);
			notificationQueueItem.relevantFactions.Add(army.faction);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyTeleportHed", new object[]
			{
				army.displayName,
				army.currentRegion.displayName
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyTeleportSummary", new object[]
			{
				army.displayNameWithArticleCapitalized,
				army.currentRegion.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyTeleportDetail", new object[]
			{
				army.displayNameWithArticle,
				badRegion.displayName,
				army.currentRegion.displayName
			});
			notificationQueueItem.gotoGameState = army;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D1A RID: 15642 RVA: 0x0017E048 File Offset: 0x0017C248
		public static void LogArmyCompletesOperation(TIArmyState army, TIArmyOperationTemplate operation, TIGameState target, TIMissionOutcome outcome, string returnStr = "")
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			notificationQueueItem.primaryFactions.Add(army.faction);
			if (!target.isRegionXenoformingState)
			{
				notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			}
			else
			{
				notificationQueueItem.relevantFactions = notificationQueueItem.primaryFactions;
			}
			if (target.ref_factions.Count > 0)
			{
				notificationQueueItem.primaryFactions.AddRange(target.ref_factions);
			}
			notificationQueueItem.outcome = outcome;
			if (outcome >= TIMissionOutcome.Success)
			{
				notificationQueueItem.itemHeadline = operation.GetSuccessHeadline(army, target);
				notificationQueueItem.itemSummary = operation.GetSuccessSummary(army, target);
				notificationQueueItem.itemDetail = operation.GetSuccessDetail(army, target);
			}
			else
			{
				notificationQueueItem.itemHeadline = operation.GetFailureHeadline(army, target);
				notificationQueueItem.itemSummary = operation.GetFailureSummary(army, target);
				notificationQueueItem.itemDetail = operation.GetFailureDetail(army, target);
			}
			if (!string.IsNullOrEmpty(returnStr))
			{
				notificationQueueItem.itemDetail = new StringBuilder(notificationQueueItem.itemDetail).Append(" ").Append(returnStr).ToString();
			}
			if (target.isRegionLandedUFO && (outcome == TIMissionOutcome.Success || outcome == TIMissionOutcome.CriticalSuccess))
			{
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_alienLandedUFOBombed;
			}
			notificationQueueItem.operation.actor = army;
			notificationQueueItem.operation.operationData = new OperationData(operation, target, new TIDateTime(), new TIDateTime());
			notificationQueueItem.gotoGameState = army;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.RepeatOperation);
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.RepeatOperationContinue);
			TINotificationQueueState.AddItem(notificationQueueItem, notificationQueueItem.primaryFactions.Contains(GameStateManager.AlienFaction()));
		}

		// Token: 0x06003D1B RID: 15643 RVA: 0x0017E1E8 File Offset: 0x0017C3E8
		public static void LogArmyCompletesOccupationOfRegion(TIArmyState army)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.primaryFactions.Add(army.faction);
			notificationQueueItem.relevantFactions.Add(army.faction);
			notificationQueueItem.relevantFactions.AddRange(army.currentRegion.ref_factions);
			foreach (TIFactionState tifactionState in army.currentNation.FactionsWithControlPoint)
			{
				if (!notificationQueueItem.primaryFactions.Contains(tifactionState))
				{
					notificationQueueItem.primaryFactions.Add(tifactionState);
				}
			}
			notificationQueueItem.gotoGameState = army;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyOccupiesRegionHed", new object[] { army.currentRegion.displayName });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyOccupiesRegionSummary", new object[]
			{
				army.displayNameWithArticleCapitalized,
				army.currentRegion.displayName
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyOccupiesRegionDetail", new object[]
			{
				army.displayNameWithArticleCapitalized,
				army.currentRegion.displayName
			});
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			TIFactionState faction = army.faction;
			TINotificationQueueState.AddItem(notificationQueueItem2, faction != null && faction.IsAlienFaction);
		}

		// Token: 0x06003D1C RID: 15644 RVA: 0x0017E35C File Offset: 0x0017C55C
		public static void LogArmyBeginsAnnexation(TIArmyState army, TIDateTime endDate)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.relevantFactions = army.currentNation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions.AddRangeUnique<TIFactionState>(army.homeNation.FactionsWithControlPoint);
			notificationQueueItem.primaryFactions = new List<TIFactionState>(notificationQueueItem.relevantFactions);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyBeginsAnnexationHed", new object[]
			{
				army.homeNation.displayName,
				army.currentRegion.displayName
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyBeginsAnnexationSummary", new object[]
			{
				army.homeNation.displayNameWithArticleCapitalized,
				army.currentRegion.displayName,
				army.currentNation.displayNameWithArticleAndPlacePrep
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyBeginsAnnexationDetail", new object[]
			{
				army.displayNameWithArticleCapitalized,
				army.currentRegion.displayName,
				army.currentNation.displayNameWithArticleAndPlacePrep,
				army.homeNation.displayNameWithArticle,
				endDate.ToCustomDateString()
			});
			notificationQueueItem.gotoGameState = army;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D1D RID: 15645 RVA: 0x0017E4A8 File Offset: 0x0017C6A8
		public static void LogArmyCompletesAnnexation(TIArmyState army, TINationState oldNation)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = oldNation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions.AddRangeUnique<TIFactionState>(army.homeNation.FactionsWithControlPoint);
			notificationQueueItem.primaryFactions = new List<TIFactionState>(notificationQueueItem.relevantFactions);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyCompletesAnnexationHed", new object[]
			{
				army.homeNation.displayName,
				army.currentRegion.displayName
			});
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyCompletesAnnexationSummary", new object[]
			{
				army.homeNation.displayNameWithArticleCapitalized,
				army.currentRegion.displayName,
				oldNation.displayNameWithArticleAndPlacePrep
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyCompletesAnnexationDetail", new object[]
			{
				army.homeNation.displayNameWithArticleCapitalized,
				army.currentRegion.displayName,
				oldNation.displayNameWithArticleAndPlacePrep
			});
			notificationQueueItem.icon = army.homeNation.flagResource;
			notificationQueueItem.gotoGameState = army.currentRegion;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D1E RID: 15646 RVA: 0x0017E5C4 File Offset: 0x0017C7C4
		public static void LogArmyAnnexationCancelled(TINationState attemptingAnnexer, TIRegionState region)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.relevantFactions = attemptingAnnexer.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions.AddRangeUnique<TIFactionState>(region.nation.FactionsWithControlPoint);
			notificationQueueItem.primaryFactions = new List<TIFactionState>(notificationQueueItem.relevantFactions);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyCancelsAnnexationHed", new object[] { attemptingAnnexer.nationalAdjective });
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyCancelsAnnexationSummary", new object[]
			{
				attemptingAnnexer.displayNameWithArticle,
				region.displayName,
				region.nation.displayNameWithArticleAndPlacePrep
			});
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyCancelsAnnexationDetail", new object[]
			{
				attemptingAnnexer.displayNameWithArticle,
				region.displayName,
				region.nation.displayNameWithArticleAndPlacePrep
			});
			notificationQueueItem.icon = region.nation.flagResource;
			notificationQueueItem.gotoGameState = region;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D1F RID: 15647 RVA: 0x0017E6C0 File Offset: 0x0017C8C0
		public static void LogArmyConquersNation(TIArmyState army, TINationState endingNation, TIRegionState oldCapital, List<TINationState> conqueringNations)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyConquersNationHed", new object[] { endingNation.displayName });
			notificationQueueItem.primaryFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.icon = endingNation.flagResource;
			notificationQueueItem.popupResource1 = army.homeNation.flagResource;
			notificationQueueItem.popupResource2 = endingNation.flagResource;
			TINationState nation = oldCapital.nation;
			notificationQueueItem.controlPointsRelevant = true;
			notificationQueueItem.newControlPoints = nation.controlPointOwnersByPoint;
			if (conqueringNations.Count == 1)
			{
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyConquersNationSummary", new object[]
				{
					endingNation.displayNameWithArticle,
					army.homeNation.displayName
				});
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyConquersNationDetail", new object[]
				{
					endingNation.displayNameWithArticle,
					army.homeNation.displayName
				});
			}
			else
			{
				string text = TIUtilities.ConstructTextList(conqueringNations.ConvertAll<TIGameState>((TINationState x) => x), false, false);
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyConquersNationSummary", new object[] { endingNation.displayNameWithArticle, text });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyConquersNationDetail", new object[] { endingNation.displayNameWithArticle, text });
			}
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_annexation;
			notificationQueueItem.musicIntensityDelta = 0.45f;
			notificationQueueItem.gotoGameState = oldCapital;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Major_Earth_Event";
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			TIFactionState faction = army.faction;
			TINotificationQueueState.AddItem(notificationQueueItem2, faction != null && faction.IsAlienFaction);
		}

		// Token: 0x06003D20 RID: 15648 RVA: 0x0017E874 File Offset: 0x0017CA74
		public static void LogArmyAssignedToFaction(TIArmyState army, TIFactionState priorFaction)
		{
			TIFactionState faction = army.faction;
			if (faction == null || faction.IsAlienFaction)
			{
				return;
			}
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			TIFactionState faction2 = army.faction;
			TINationState homeNation = army.homeNation;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.primaryFactions.Add(faction2);
			if (army.AlienMegafaunaArmy)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MegafaunaIsOursHed", new object[] { faction2.adjective });
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MegafaunaIsOursSummary", new object[]
				{
					army.currentRegion.displayName,
					faction2.displayNameWithColor
				});
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MegafaunaIsOursDetail", new object[] { army.currentRegion.displayName });
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_xenofaunaArmy;
			}
			else
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyJoinsFactionHed", new object[] { faction2.adjective });
				if (priorFaction != null)
				{
					notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyChangesFaction", new object[] { faction2.displayNameWithColor, army.displayNameWithArticleCapitalized, homeNation.displayName, priorFaction.displayNameWithColor });
					notificationQueueItem.primaryFactions.Add(priorFaction);
					notificationQueueItem.popupResource1 = faction2.factionIcon256path;
					notificationQueueItem.popupResource2 = priorFaction.factionIcon256path;
				}
				else
				{
					notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyJoinsFaction", new object[] { faction2.displayNameWithColor, army.displayNameWithArticleCapitalized, homeNation.displayName });
					notificationQueueItem.popupResource1 = faction2.factionIcon256path;
					notificationQueueItem.popupResource2 = homeNation.flagResource;
				}
			}
			if (army.HumanArmy)
			{
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_armyAssigned;
			}
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			notificationQueueItem.gotoGameState = army.currentRegion;
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D21 RID: 15649 RVA: 0x0017EA7C File Offset: 0x0017CC7C
		public static void LogNewArmyBuilt(TIArmyState army)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions = army.homeNation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyBuiltHed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyBuiltSummary", new object[]
			{
				army.homeNation.displayNameWithArticleCapitalized,
				army.displayNameWithArticle,
				army.homeRegion.displayName
			});
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.popup1BackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.popupResource1 = army.GetIconForegroundResource;
			notificationQueueItem.popupResource2 = army.homeNation.flagResource;
			notificationQueueItem.gotoGameState = army;
			if (army.HumanArmy)
			{
				notificationQueueItem.illustrationResource = TemplateManager.global.illus_armyConstructed;
			}
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			TIFactionState faction = army.faction;
			TINotificationQueueState.AddItem(notificationQueueItem2, faction != null && faction.IsAlienFaction);
		}

		// Token: 0x06003D22 RID: 15650 RVA: 0x0017EB94 File Offset: 0x0017CD94
		public static void LogNewNavyBuilt(TIArmyState army)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.Add(army.faction);
			notificationQueueItem.relevantFactions = army.homeNation.FactionsWithControlPoint;
			notificationQueueItem.relevantFactions.AddRange(army.homeNation.wars.SelectMany<TINationState, TIFactionState>((TINationState x) => x.FactionsWithControlPoint));
			notificationQueueItem.relevantFactions = notificationQueueItem.relevantFactions.Distinct<TIFactionState>().ToList<TIFactionState>();
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.NavyBuilt.Hed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.NavyBuilt.Summary", new object[] { army.homeNation.displayNameWithArticleCapitalized });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.NavyBuilt.Detail", new object[]
			{
				army.homeNation.displayNameWithArticleCapitalized,
				army.displayNameWithArticle,
				army.homeNation.displayNameWithArticle
			});
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.popup1BackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.popupResource1 = army.GetIconForegroundResource;
			notificationQueueItem.popupResource2 = army.homeNation.flagResource;
			notificationQueueItem.gotoGameState = army;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D23 RID: 15651 RVA: 0x0017ECFC File Offset: 0x0017CEFC
		public static void LogSpaceDefensesComplete(TISpaceDefensesFacilityState defenses)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.primaryFactions.AddRange(defenses.ref_nation.FactionsWithControlPoint);
			notificationQueueItem.relevantFactions.AddRange(defenses.ref_nation.FactionsWithControlPoint);
			notificationQueueItem.relevantFactions.AddRange(defenses.ref_nation.wars.SelectMany<TINationState, TIFactionState>((TINationState x) => x.FactionsWithControlPoint));
			notificationQueueItem.icon = defenses.GetIconResourcePath(GameControl.control.activePlayer);
			notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.SpaceDefensesCompleteHed");
			notificationQueueItem.itemSummary = Loc.T("UI.Notifications.SpaceDefensesCompleteSummary", new object[] { defenses.region.displayName });
			notificationQueueItem.itemDetail = Loc.T("UI.Notifications.SpaceDefensesCompleteDetail", new object[]
			{
				defenses.region.displayName,
				defenses.region.nation.displayNameWithArticle
			});
			notificationQueueItem.illustrationResource = TemplateManager.global.illus_spaceDefensesPath;
			notificationQueueItem.gotoGameState = defenses;
			notificationQueueItem.notificationDelegates.Add(SpecialNotificationDelegate.OpenNationPriorities);
			TINotificationQueueState.AddItem(notificationQueueItem, defenses.ref_nation.alienNation);
		}

		// Token: 0x06003D24 RID: 15652 RVA: 0x0017EE38 File Offset: 0x0017D038
		public static void LogArmyIsDestroyed(TIArmyState army, TIRegionState location, TIFactionState attacker)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = army.GetIconForegroundResource;
			notificationQueueItem.iconBackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.backgroundColor = army.GetIconBackgroundResourceColor;
			notificationQueueItem.popupResource1 = army.GetIconForegroundResource;
			notificationQueueItem.popup1BackgroundResource = army.GetIconBackgroundResource;
			notificationQueueItem.popupResource2 = army.homeNation.flagResource;
			notificationQueueItem.relevantFactions = TINotificationQueueState.AllFactions;
			notificationQueueItem.primaryFactions.Add(army.faction);
			notificationQueueItem.primaryFactions.Add(army.homeNation.executiveFaction);
			notificationQueueItem.primaryFactions.Add(army.currentRegion.nation.executiveFaction);
			if (attacker != null && !notificationQueueItem.primaryFactions.Contains(attacker))
			{
				notificationQueueItem.primaryFactions.Add(attacker);
			}
			foreach (TIArmyState tiarmyState in army.currentRegion.FilteredArmiesPresent(true, true, true, false, false))
			{
				notificationQueueItem.primaryFactions.Add(tiarmyState.faction);
			}
			notificationQueueItem.primaryFactions = notificationQueueItem.primaryFactions.Distinct<TIFactionState>().ToList<TIFactionState>();
			notificationQueueItem.primaryFactions.RemoveAll((TIFactionState x) => x == null);
			notificationQueueItem.gotoGameState = location;
			if (army.AlienMegafaunaArmy)
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.MegafaunaArmyDestroyedHed");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.MegafaunaArmyDestroyedSummary", new object[] { army.displayNameWithArticleCapitalized });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.MegafaunaArmyDestroyedDetail", new object[]
				{
					army.displayNameWithArticleCapitalized,
					army.currentRegion.displayName
				});
			}
			else
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.ArmyDestroyedHed");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.ArmyDestroyedSummary", new object[]
				{
					army.homeNation.nationalAdjective,
					army.displayName
				});
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.ArmyDestroyedDetail", new object[]
				{
					army.homeNation.nationalAdjective,
					army.displayName,
					army.currentRegion.displayName
				});
			}
			notificationQueueItem.musicIntensityDelta = 0.05f;
			notificationQueueItem.soundToPlay = "event:/SFX/UI_Special_SFX/trig_SFX_Major_Earth_Event";
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			TIFactionState faction = army.faction;
			TINotificationQueueState.AddItem(notificationQueueItem2, faction != null && faction.IsAlienFaction);
		}

		// Token: 0x06003D25 RID: 15653 RVA: 0x0017F0C0 File Offset: 0x0017D2C0
		public static void LogPactEnds(TIFactionState endingFaction, TIFactionState otherFaction, List<TradeOffer.TreatyType> treaties)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.icon = endingFaction.factionIcon64path;
			notificationQueueItem.popupResource1 = endingFaction.factionIcon256path;
			notificationQueueItem.popupResource2 = otherFaction.factionIcon256path;
			notificationQueueItem.primaryFactions.Add(endingFaction);
			notificationQueueItem.primaryFactions.Add(otherFaction);
			notificationQueueItem.relevantFactions = new List<TIFactionState>(notificationQueueItem.primaryFactions);
			if (treaties.Contains(TradeOffer.TreatyType.Truce))
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.PactEnds.Hed_Truce");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.PactEnds.Summary_Truce", new object[] { endingFaction.displayNameCapitalizedWithColor, otherFaction.displayNameWithColor });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.PactEnds.Detail_Truce", new object[] { endingFaction.displayNameCapitalizedWithColor, otherFaction.displayNameWithColor });
			}
			else if (treaties.Contains(TradeOffer.TreatyType.NAP) && treaties.Contains(TradeOffer.TreatyType.Intel))
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.PactEnds.Hed_NAPIntel");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.PactEnds.Summary_NAPIntel", new object[] { endingFaction.displayNameCapitalizedWithColor, otherFaction.displayNameWithColor });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.PactEnds.Detail_NAPIntel", new object[] { endingFaction.displayNameCapitalizedWithColor, otherFaction.displayNameWithColor });
			}
			else if (treaties.Contains(TradeOffer.TreatyType.NAP))
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.PactEnds.Hed_NAP");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.PactEnds.Summary_NAP", new object[] { endingFaction.displayNameCapitalizedWithColor, otherFaction.displayNameWithColor });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.PactEnds.Detail_NAP", new object[] { endingFaction.displayNameCapitalizedWithColor, otherFaction.displayNameWithColor });
			}
			else if (treaties.Contains(TradeOffer.TreatyType.Intel))
			{
				notificationQueueItem.itemHeadline = Loc.T("UI.Notifications.PactEnds.Hed_Intel");
				notificationQueueItem.itemSummary = Loc.T("UI.Notifications.PactEnds.Summary_Intel", new object[] { endingFaction.displayNameCapitalizedWithColor, otherFaction.displayNameWithColor });
				notificationQueueItem.itemDetail = Loc.T("UI.Notifications.PactEnds.Detail_Intel", new object[] { endingFaction.displayNameCapitalizedWithColor, otherFaction.displayNameWithColor });
			}
			TINotificationQueueState.AddItem(notificationQueueItem, notificationQueueItem.alertFactions.Contains(GameStateManager.AlienFaction()));
		}

		// Token: 0x06003D26 RID: 15654 RVA: 0x0017F2FC File Offset: 0x0017D4FC
		public static void AlertNarrativeEvent(TIFactionState faction, TINarrativeEventTemplate eventTemplate, TIGameState target, TIGameState secondaryTarget = null, Dictionary<TIGameState, TIGameState> allTargetsAndSeconds = null)
		{
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			notificationQueueItem.narrativeEventAlert = true;
			if (eventTemplate.hitAllQualifyingTargets && (target.isRegionState || target.isNationState || eventTemplate.targetType == NarrativeEventTargetType.global))
			{
				notificationQueueItem.icon = GameStateManager.Earth().iconResource;
				notificationQueueItem.popupResource1 = GameStateManager.Earth().iconResource;
				notificationQueueItem.allNarrativeEventTargetsAndSeconds = allTargetsAndSeconds;
			}
			else if (target.isCouncilorState)
			{
				notificationQueueItem.icon = target.ref_councilor.iconResource;
				notificationQueueItem.iconBackgroundResource = target.ref_councilor.iconBackground;
				notificationQueueItem.popupResource1 = target.ref_councilor.iconResource;
				notificationQueueItem.popup1BackgroundResource = target.ref_councilor.iconBackground;
			}
			else if (target.isNaturalSpaceObjectState || target.isHabState)
			{
				notificationQueueItem.icon = target.ref_naturalSpaceObject.iconResource;
				notificationQueueItem.popupResource1 = target.ref_naturalSpaceObject.iconResource;
			}
			else if (target.isOfficerState)
			{
				notificationQueueItem.icon = target.ref_officer.GetIconPath();
				notificationQueueItem.popupResource1 = target.ref_faction.factionIcon256path;
			}
			else
			{
				NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
				TINationState ref_nation = target.ref_nation;
				notificationQueueItem2.icon = ((ref_nation != null) ? ref_nation.flagResource : null) ?? faction.factionIcon64path;
				NotificationQueueItem notificationQueueItem3 = notificationQueueItem;
				TINationState ref_nation2 = target.ref_nation;
				notificationQueueItem3.popupResource1 = ((ref_nation2 != null) ? ref_nation2.flagResource : null) ?? faction.factionIcon256path;
			}
			if (secondaryTarget != null)
			{
				if (secondaryTarget.isCouncilorState)
				{
					notificationQueueItem.popupResource2 = secondaryTarget.ref_councilor.iconResource;
				}
				else if (secondaryTarget.isNaturalSpaceObjectState || secondaryTarget.isHabState)
				{
					notificationQueueItem.popupResource2 = secondaryTarget.ref_naturalSpaceObject.iconResource;
				}
				else if (secondaryTarget.isOfficerState)
				{
					notificationQueueItem.popupResource2 = secondaryTarget.ref_officer.GetIconPath();
				}
				else
				{
					NotificationQueueItem notificationQueueItem4 = notificationQueueItem;
					TINationState ref_nation3 = secondaryTarget.ref_nation;
					notificationQueueItem4.popupResource2 = ((ref_nation3 != null) ? ref_nation3.flagResource : null) ?? string.Empty;
				}
			}
			notificationQueueItem.relevantFactions.Add(faction);
			notificationQueueItem.primaryFactions.Add(faction);
			notificationQueueItem.alertBlockFaction = faction;
			notificationQueueItem.itemHeadline = eventTemplate.displayName;
			notificationQueueItem.itemSummary = eventTemplate.summary(faction, target, secondaryTarget);
			notificationQueueItem.itemDetail = eventTemplate.query(faction, target, secondaryTarget);
			notificationQueueItem.illustrationResource = eventTemplate.illustrationResource;
			notificationQueueItem.soundToPlay = eventTemplate.soundResource;
			notificationQueueItem.promptingGameState = target;
			notificationQueueItem.alertRelatedState = secondaryTarget;
			notificationQueueItem.relatedTemplate = eventTemplate;
			notificationQueueItem.gotoGameState = TINotificationQueueState.GenericGotoState(target);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D27 RID: 15655 RVA: 0x0017F570 File Offset: 0x0017D770
		private static List<TIFactionState> GetFactionsToNotify(TIGameState actingState, TIGameState target, TIGameState secondaryState, PublicityType publicity)
		{
			List<TIFactionState> list = new List<TIFactionState>();
			switch (publicity)
			{
			case PublicityType.global:
				list = TINotificationQueueState.AllFactions.ToList<TIFactionState>();
				break;
			case PublicityType.factions:
				if (actingState.ref_factions != null && actingState.ref_factions.Count > 0)
				{
					list.AddRange(actingState.ref_factions);
				}
				if (target.ref_factions != null && target.ref_factions.Count > 0)
				{
					list.AddRange(target.ref_factions);
				}
				if (((secondaryState != null) ? secondaryState.ref_factions : null) != null && secondaryState.ref_factions.Count > 0)
				{
					list.AddRange(secondaryState.ref_factions);
				}
				list = list.Distinct<TIFactionState>().ToList<TIFactionState>();
				break;
			case PublicityType.target:
				if (actingState.ref_faction != null)
				{
					list.Add(actingState.ref_faction);
				}
				if (target.ref_faction != null)
				{
					list.Add(target.ref_faction);
				}
				if (((secondaryState != null) ? secondaryState.ref_faction : null) != null)
				{
					list.Add(secondaryState.ref_faction);
				}
				list = list.Distinct<TIFactionState>().ToList<TIFactionState>();
				break;
			case PublicityType.target_PrimaryOnly:
				if (actingState.ref_faction != null)
				{
					list.Add(actingState.ref_faction);
				}
				if (target.ref_faction != null)
				{
					list.Add(target.ref_faction);
				}
				list = list.Distinct<TIFactionState>().ToList<TIFactionState>();
				break;
			case PublicityType.actor:
				if (actingState.ref_faction != null)
				{
					list.Add(actingState.ref_faction);
				}
				list = list.Distinct<TIFactionState>().ToList<TIFactionState>();
				break;
			}
			return list;
		}

		// Token: 0x06003D28 RID: 15656 RVA: 0x0017F700 File Offset: 0x0017D900
		public static void LogNarrativeEventResolution(TIGameState actingState, TIGameState target, TIGameState secondaryState, TINarrativeEventTemplate eventTemplate, int optionSelected, int outcomeRolled, bool reportOutcome)
		{
			List<TIFactionState> factionsToNotify = TINotificationQueueState.GetFactionsToNotify(actingState, target, secondaryState, eventTemplate.logPublicity);
			NotificationQueueItem notificationQueueItem = TINotificationQueueState.InitItem(MethodBase.GetCurrentMethod().Name);
			NotificationQueueItem notificationQueueItem2 = notificationQueueItem;
			TINationState ref_nation = actingState.ref_nation;
			notificationQueueItem2.icon = ((ref_nation != null) ? ref_nation.flagResource : null) ?? actingState.ref_faction.factionIcon256path;
			notificationQueueItem.relatedTemplate = eventTemplate;
			if (reportOutcome)
			{
				notificationQueueItem.primaryFactions = TINotificationQueueState.GetFactionsToNotify(actingState, target, secondaryState, eventTemplate.alertPublicity);
				notificationQueueItem.itemHeadline = eventTemplate.displayName;
				notificationQueueItem.itemSummary = eventTemplate.outcomeSummary(actingState, target, secondaryState, optionSelected, outcomeRolled);
				notificationQueueItem.illustrationResource = eventTemplate.illustrationResource;
				NarrativeEventOutcome narrativeEventOutcome = eventTemplate.eventOptions[optionSelected].outcomes[outcomeRolled];
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(eventTemplate.outcomeDetail(actingState, target, secondaryState, optionSelected, outcomeRolled)).AppendLine();
				TIResourcesCost costs = narrativeEventOutcome.GetCosts(target);
				if (costs.anyDebit)
				{
					stringBuilder.AppendLine(Loc.T("NarrativeEventOption.costs", new object[] { costs.GetString("Relevant", false, false, false, 7, true, false, null, false, FactionResource.None) }));
				}
				if (costs.anyCredit)
				{
					stringBuilder.AppendLine(Loc.T("NarrativeEventOption.gains", new object[] { costs.GetString("Relevant", false, false, false, 7, false, true, null, false, FactionResource.None) }));
				}
				TIOrgTemplate orgGranted = narrativeEventOutcome.orgGranted;
				if (orgGranted != null)
				{
					stringBuilder.AppendLine(Loc.T("NarrativeEventOption.org", new object[] { orgGranted.randomized ? Loc.T("TIOrgTemplate.displayName.noNameYetWithArticle") : orgGranted.displayNameWithArticle }));
				}
				TIProjectTemplate projectGranted = narrativeEventOutcome.projectGranted;
				if (projectGranted != null)
				{
					stringBuilder.AppendLine(Loc.T("NarrativeEventOption.project", new object[] { projectGranted.displayName }));
				}
				foreach (TINarrativeEventTemplate tinarrativeEventTemplate in narrativeEventOutcome.eventsToAdd)
				{
					stringBuilder.AppendLine(Loc.T("NarrativeEventOption.unlocksEvent", new object[] { tinarrativeEventTemplate.displayName }));
				}
				foreach (TINarrativeEventTemplate tinarrativeEventTemplate2 in narrativeEventOutcome.eventsToRemove)
				{
					stringBuilder.AppendLine(Loc.T("NarrativeEventOption.removesEvent", new object[] { tinarrativeEventTemplate2.displayName }));
				}
				List<TIEffectTemplate> effectTemplates = narrativeEventOutcome.effectTemplates;
				effectTemplates.AddRange(narrativeEventOutcome.delayedEffectTemplates);
				if (effectTemplates.Count > 0)
				{
					stringBuilder.AppendLine(Loc.T("NarrativeEventOption.effects"));
					foreach (TIEffectTemplate tieffectTemplate in effectTemplates)
					{
						stringBuilder.AppendLine(tieffectTemplate.description(target, secondaryState));
					}
				}
				notificationQueueItem.itemDetail = stringBuilder.ToString();
			}
			else
			{
				notificationQueueItem.itemSummary = eventTemplate.optionSummary(actingState, target, secondaryState, optionSelected);
			}
			notificationQueueItem.relevantFactions = factionsToNotify;
			notificationQueueItem.gotoGameState = TINotificationQueueState.GenericGotoState(target);
			TINotificationQueueState.AddItem(notificationQueueItem, false);
		}

		// Token: 0x06003D29 RID: 15657 RVA: 0x0017FA3C File Offset: 0x0017DC3C
		public static void AddCouncilorMessage(TIGameState speaker, CouncilorChatType chatType, TIFactionState intendedFaction)
		{
			if (intendedFaction != GameControl.control.activePlayer)
			{
				return;
			}
			bool? flag;
			if (speaker == null)
			{
				flag = null;
			}
			else
			{
				TIFactionState ref_faction = speaker.ref_faction;
				flag = ((ref_faction != null) ? new bool?(ref_faction.IsAlienFaction) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				return;
			}
			TINotificationQueueState tinotificationQueueState = GameStateManager.NotificationQueue();
			string text = "Med_";
			TIFactionState tifactionState;
			string text2;
			if (speaker != null && speaker.isCouncilorState)
			{
				TICouncilorState ref_councilor = speaker.ref_councilor;
				tifactionState = ref_councilor.faction;
				text2 = ref_councilor.displayName;
				int attribute = ref_councilor.GetAttribute(CouncilorAttribute.ApparentLoyalty, true, true, true, false, false, false);
				if (attribute <= 6)
				{
					text = "Low_";
				}
				else if (attribute >= 19)
				{
					text = "High_";
				}
			}
			else
			{
				tifactionState = ((speaker != null) ? speaker.ref_faction : null) ?? intendedFaction;
				if (!(tifactionState != null))
				{
					return;
				}
				speaker = tifactionState;
				text2 = tifactionState.leaderName;
			}
			switch (chatType)
			{
			case CouncilorChatType.NewCouncilor:
			case CouncilorChatType.CouncilorReleased:
			case CouncilorChatType.AlienRetaliationDeclared:
			case CouncilorChatType.AlienFullWarDeclared:
			case CouncilorChatType.RecruitPoolUpdated:
			case CouncilorChatType.NewOrgsAvailable:
			case CouncilorChatType.TruceEnded:
			case CouncilorChatType.NAPEnded:
			{
				StringBuilder stringBuilder = new StringBuilder("UI.Chat.").Append(chatType.ToString()).Append(text);
				List<string> list = Loc.FindAllKeys(stringBuilder.ToString());
				if (list.Count == 0)
				{
					stringBuilder = new StringBuilder("UI.Chat.").Append(chatType.ToString()).Append("Med_");
					list = Loc.FindAllKeys(stringBuilder.ToString());
				}
				if (list.Count > 0)
				{
					StringBuilder stringBuilder2 = new StringBuilder(Loc.T("UI.Chat.Base", new object[]
					{
						text2,
						Loc.T(list.SelectRandomItem<string>())
					}));
					stringBuilder2.Replace("{MyFactionName}", intendedFaction.displayNameWithColor);
					stringBuilder2.Replace("{SpeakingFactionName}", tifactionState.displayNameWithColor);
					stringBuilder2.Replace("{LeaderAddress}", intendedFaction.leaderAddress);
					tinotificationQueueState.councilorMessages.Enqueue(new CouncilorMessage(speaker, stringBuilder2.ToString()));
					return;
				}
				Log.Warn("No chat entries for " + stringBuilder.ToString(), Array.Empty<object>());
				return;
			}
			case CouncilorChatType.MissionPhaseHint:
			{
				int min = tinotificationQueueState.usedCouncilorMessages.Min<KeyValuePair<TIFactionState.Advice, int>>((KeyValuePair<TIFactionState.Advice, int> x) => x.Value);
				List<TIFactionState.Advice> list2 = (from x in GameStateManager.NotificationQueue().usedCouncilorMessages
					where x.Value == min
					select x.Key).ToList<TIFactionState.Advice>();
				TIFactionState.AdviceData adviceData = TIFactionState.GetAdvice(speaker, 1, list2).FirstOrDefault<TIFactionState.AdviceData>();
				if (!string.IsNullOrEmpty(adviceData.adviceText))
				{
					tinotificationQueueState.councilorMessages.Enqueue(new CouncilorMessage(speaker, adviceData.adviceText));
					Dictionary<TIFactionState.Advice, int> dictionary = GameStateManager.NotificationQueue().usedCouncilorMessages;
					TIFactionState.Advice adviceType = adviceData.adviceType;
					dictionary[adviceType]++;
					return;
				}
				return;
			}
			case CouncilorChatType.IdleConversation:
				return;
			case CouncilorChatType.WarDeclared:
				tinotificationQueueState.councilorMessages.Enqueue(new CouncilorMessage(speaker, speaker.ref_faction.DiplomacyGreetingMessage(intendedFaction, true)));
				return;
			case CouncilorChatType.GameQuicksaved:
				tinotificationQueueState.councilorMessages.Enqueue(new CouncilorMessage(speaker, Loc.T("UI.GeneralControls.QuicksaveComplete")));
				return;
			default:
				return;
			}
		}

		// Token: 0x06003D2A RID: 15658 RVA: 0x0017FD79 File Offset: 0x0017DF79
		public static CouncilorMessage GetNextCouncilorMessage()
		{
			return GameStateManager.NotificationQueue().councilorMessages.Dequeue();
		}

		// Token: 0x0400267A RID: 9850
		[fsIgnore]
		private Dictionary<TIFactionState.Advice, int> usedCouncilorMessages;

		// Token: 0x0400267B RID: 9851
		public int alienEvents;

		// Token: 0x0400267C RID: 9852
		private TIPromptQueueState promptQueue;

		// Token: 0x0400267D RID: 9853
		public Dictionary<TIFactionState, Dictionary<string, int>> firstTimeTracker;

		// Token: 0x0400267E RID: 9854
		private const int maxNotificationQueueSize = 60;

		// Token: 0x0400267F RID: 9855
		private const int maxBombardmentQueueSize = 120;

		// Token: 0x04002680 RID: 9856
		private static readonly Dictionary<SummaryCategory, int> maxSummaryQueueSize = new Dictionary<SummaryCategory, int>
		{
			{
				SummaryCategory.CouncilorSightings,
				60
			},
			{
				SummaryCategory.EarthEvents,
				60
			},
			{
				SummaryCategory.Missions,
				60
			},
			{
				SummaryCategory.SpaceEvents,
				60
			},
			{
				SummaryCategory.Bombardment,
				120
			},
			{
				SummaryCategory.None,
				0
			}
		};

		// Token: 0x04002681 RID: 9857
		public const float musicIntensityStep = 0.45f;
	}
}
