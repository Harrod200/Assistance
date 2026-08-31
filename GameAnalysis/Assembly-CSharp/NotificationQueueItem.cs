using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class NotificationQueueItem
{
	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000557 RID: 1367 RVA: 0x00017AE9 File Offset: 0x00015CE9
	public TINotificationTemplate template
	{
		get
		{
			return TemplateManager.Find<TINotificationTemplate>(this.templateName, false);
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000558 RID: 1368 RVA: 0x00017AF7 File Offset: 0x00015CF7
	public string itemHammer
	{
		get
		{
			return Loc.T(this.template.alertHammerLoc);
		}
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x00017B09 File Offset: 0x00015D09
	private List<TIFactionState> DefaultAudienceFactions(NotificationAudience audience)
	{
		switch (audience)
		{
		case NotificationAudience.AllFactions:
			return GameStateManager.AllFactions().ToList<TIFactionState>();
		case NotificationAudience.PrimaryFactions:
			return this.primaryFactions;
		case NotificationAudience.RelevantFactions:
			return this.relevantFactions;
		}
		return new List<TIFactionState>();
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x0600055A RID: 1370 RVA: 0x00017B44 File Offset: 0x00015D44
	public List<TIFactionState> alertFactions
	{
		get
		{
			List<TIFactionState> list = new List<TIFactionState>(this.DefaultAudienceFactions(this.template.alertAudience));
			List<TIFactionState> list2 = new List<TIFactionState>(this.primaryFactions);
			list2.AddRangeUnique<TIFactionState>(this.relevantFactions);
			foreach (TIFactionState tifactionState in list2)
			{
				if (tifactionState.checkNotificationOverrides && this.template.firstAlertOverride && TINotificationQueueState.FirstNotificationOfType(tifactionState, this.templateName))
				{
					list.AddUnique(tifactionState);
				}
				if (tifactionState.notificationOverrides.ContainsKey(this.templateName))
				{
					NotificationOverrideBehavior alert = tifactionState.notificationOverrides[this.templateName].alert;
					if (alert != NotificationOverrideBehavior.Add)
					{
						if (alert == NotificationOverrideBehavior.Remove)
						{
							list.Remove(tifactionState);
						}
					}
					else
					{
						list.AddUnique(tifactionState);
					}
				}
			}
			return list;
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x0600055B RID: 1371 RVA: 0x00017C30 File Offset: 0x00015E30
	public List<TIFactionState> timerFactions
	{
		get
		{
			List<TIFactionState> list = new List<TIFactionState>(this.DefaultAudienceFactions(this.template.timerAudience));
			List<TIFactionState> list2 = new List<TIFactionState>(this.primaryFactions);
			list2.AddRangeUnique<TIFactionState>(this.relevantFactions);
			foreach (TIFactionState tifactionState in list2)
			{
				if (tifactionState.checkNotificationOverrides && tifactionState.notificationOverrides.ContainsKey(this.templateName))
				{
					NotificationOverrideBehavior timerFeed = tifactionState.notificationOverrides[this.templateName].timerFeed;
					if (timerFeed != NotificationOverrideBehavior.Add)
					{
						if (timerFeed == NotificationOverrideBehavior.Remove)
						{
							list.Remove(tifactionState);
						}
					}
					else
					{
						list.AddUnique(tifactionState);
					}
				}
			}
			return list;
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x0600055C RID: 1372 RVA: 0x00017CF4 File Offset: 0x00015EF4
	public List<TIFactionState> newsFeedFactions
	{
		get
		{
			List<TIFactionState> list = new List<TIFactionState>(this.DefaultAudienceFactions(this.template.newsFeedAudience));
			List<TIFactionState> list2 = new List<TIFactionState>(this.primaryFactions);
			list2.AddRangeUnique<TIFactionState>(this.relevantFactions);
			foreach (TIFactionState tifactionState in list2)
			{
				if (tifactionState.checkNotificationOverrides && tifactionState.notificationOverrides.ContainsKey(this.templateName))
				{
					NotificationOverrideBehavior newsFeed = tifactionState.notificationOverrides[this.templateName].newsFeed;
					if (newsFeed != NotificationOverrideBehavior.Add)
					{
						if (newsFeed == NotificationOverrideBehavior.Remove)
						{
							list.Remove(tifactionState);
						}
					}
					else
					{
						list.AddUnique(tifactionState);
					}
				}
			}
			return list;
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x0600055D RID: 1373 RVA: 0x00017DB8 File Offset: 0x00015FB8
	public List<TIFactionState> summaryLogFactions
	{
		get
		{
			List<TIFactionState> list = new List<TIFactionState>(this.DefaultAudienceFactions(this.template.summaryAudience.audience));
			List<TIFactionState> list2 = new List<TIFactionState>(this.primaryFactions);
			list2.AddRangeUnique<TIFactionState>(this.relevantFactions);
			foreach (TIFactionState tifactionState in list2)
			{
				if (tifactionState.checkNotificationOverrides && tifactionState.notificationOverrides.ContainsKey(this.templateName))
				{
					NotificationOverrideBehavior summaryFeed = tifactionState.notificationOverrides[this.templateName].summaryFeed;
					if (summaryFeed != NotificationOverrideBehavior.Add)
					{
						if (summaryFeed == NotificationOverrideBehavior.Remove)
						{
							list.Remove(tifactionState);
						}
					}
					else
					{
						list.AddUnique(tifactionState);
					}
				}
			}
			return list;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x0600055E RID: 1374 RVA: 0x00017E80 File Offset: 0x00016080
	public bool putInTimerQueue
	{
		get
		{
			return this.timerFactions.Any<TIFactionState>((TIFactionState x) => x.showTimerNotifications);
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x0600055F RID: 1375 RVA: 0x00017EAC File Offset: 0x000160AC
	public bool putInNewsFeed
	{
		get
		{
			return this.newsFeedFactions.Any<TIFactionState>((TIFactionState x) => x.showRegularNotifications);
		}
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000560 RID: 1376 RVA: 0x00017ED8 File Offset: 0x000160D8
	public bool putInSummaryLog
	{
		get
		{
			if (this.template.summaryAudience.category != SummaryCategory.None)
			{
				return this.summaryLogFactions.Any<TIFactionState>((TIFactionState x) => x.showSummaryLogs);
			}
			return false;
		}
	}

	// Token: 0x040004D5 RID: 1237
	public string templateName;

	// Token: 0x040004D6 RID: 1238
	public List<TIFactionState> primaryFactions;

	// Token: 0x040004D7 RID: 1239
	public List<TIFactionState> relevantFactions;

	// Token: 0x040004D8 RID: 1240
	public string itemHeadline;

	// Token: 0x040004D9 RID: 1241
	public string itemSummary;

	// Token: 0x040004DA RID: 1242
	public string itemDetail;

	// Token: 0x040004DB RID: 1243
	public string dateTimeString;

	// Token: 0x040004DC RID: 1244
	public string icon;

	// Token: 0x040004DD RID: 1245
	public string iconBackgroundResource;

	// Token: 0x040004DE RID: 1246
	public Dictionary<TIFactionState, string> factionSpecificDetail;

	// Token: 0x040004DF RID: 1247
	public string popupResource1;

	// Token: 0x040004E0 RID: 1248
	public string popup1BackgroundResource;

	// Token: 0x040004E1 RID: 1249
	public string popupResource2;

	// Token: 0x040004E2 RID: 1250
	public string illustrationResource = string.Empty;

	// Token: 0x040004E3 RID: 1251
	public string videoResource;

	// Token: 0x040004E4 RID: 1252
	public string movieResource;

	// Token: 0x040004E5 RID: 1253
	public string animationSpriteSheetPath;

	// Token: 0x040004E6 RID: 1254
	public Color backgroundColor;

	// Token: 0x040004E7 RID: 1255
	public TIMissionOutcome outcome;

	// Token: 0x040004E8 RID: 1256
	public TIFactionState alertBlockFaction;

	// Token: 0x040004E9 RID: 1257
	public TIGameState promptingGameState;

	// Token: 0x040004EA RID: 1258
	public TIGameState alertRelatedState;

	// Token: 0x040004EB RID: 1259
	public TIDataTemplate relatedTemplate;

	// Token: 0x040004EC RID: 1260
	public int utilityValue;

	// Token: 0x040004ED RID: 1261
	public string alertBlockEventName;

	// Token: 0x040004EE RID: 1262
	public bool controlPointsRelevant;

	// Token: 0x040004EF RID: 1263
	public IList<TIGameState> oldControlPoints;

	// Token: 0x040004F0 RID: 1264
	public IList<TIGameState> newControlPoints;

	// Token: 0x040004F1 RID: 1265
	public Dictionary<TIGameState, TIGameState> allNarrativeEventTargetsAndSeconds;

	// Token: 0x040004F2 RID: 1266
	public string soundToPlay;

	// Token: 0x040004F3 RID: 1267
	public string fanfareToPlay;

	// Token: 0x040004F4 RID: 1268
	public float musicIntensityDelta;

	// Token: 0x040004F5 RID: 1269
	public Action OnOpenNotification;

	// Token: 0x040004F6 RID: 1270
	public Action OnCloseNotification;

	// Token: 0x040004F7 RID: 1271
	public TIMissionState mission;

	// Token: 0x040004F8 RID: 1272
	public ActorOperationData operation;

	// Token: 0x040004F9 RID: 1273
	public TIGameState gotoGameState;

	// Token: 0x040004FA RID: 1274
	public bool narrativeEventAlert;

	// Token: 0x040004FB RID: 1275
	public bool triggerEndGame;

	// Token: 0x040004FC RID: 1276
	public bool showSideArt;

	// Token: 0x040004FD RID: 1277
	public string customButtonTemplateName;

	// Token: 0x040004FE RID: 1278
	public TIDateTime dateTime;

	// Token: 0x040004FF RID: 1279
	public List<SpecialNotificationDelegate> notificationDelegates = new List<SpecialNotificationDelegate>();
}
