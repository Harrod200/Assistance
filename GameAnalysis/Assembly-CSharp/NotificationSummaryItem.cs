using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class NotificationSummaryItem
{
	// Token: 0x06000554 RID: 1364 RVA: 0x00017A10 File Offset: 0x00015C10
	public NotificationSummaryItem(string itemSummary, string iconResource, string iconBackgroundResource, Color backgroundColor, TIGameState gotoGameState, bool alienRelated, TIDateTime dateTime, string templateName, List<TIFactionState> timerFactions, List<TIFactionState> newsFeedFactions, List<TIFactionState> summaryLogFactions, TIMissionOutcome outcome)
	{
		this.itemSummary = itemSummary;
		this.dateTimeString = ((dateTime != null) ? dateTime.ToCustomDateString() : null) ?? string.Empty;
		this.iconResource = iconResource;
		this.iconBackgroundResource = iconBackgroundResource;
		this.backgroundColor = backgroundColor;
		this.gotoGameState = gotoGameState;
		this.alienRelated = alienRelated;
		this.dateTime = dateTime;
		this.templateName = templateName;
		this.timerFactions = timerFactions;
		this.newsFeedFactions = newsFeedFactions;
		this.summaryLogFactions = summaryLogFactions;
		this.outcome = outcome;
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000555 RID: 1365 RVA: 0x00017ABE File Offset: 0x00015CBE
	public TINotificationTemplate template
	{
		get
		{
			if (this._template == null)
			{
				this._template = TemplateManager.Find<TINotificationTemplate>(this.templateName, false);
			}
			return this._template;
		}
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x00017AE0 File Offset: 0x00015CE0
	public void UpdateGotoGameState(TIGameState state)
	{
		this.gotoGameState = state;
	}

	// Token: 0x040004C7 RID: 1223
	public string templateName;

	// Token: 0x040004C8 RID: 1224
	public string itemSummary;

	// Token: 0x040004C9 RID: 1225
	public string dateTimeString;

	// Token: 0x040004CA RID: 1226
	public string iconResource;

	// Token: 0x040004CB RID: 1227
	public string iconBackgroundResource;

	// Token: 0x040004CC RID: 1228
	public Color backgroundColor;

	// Token: 0x040004CD RID: 1229
	public TIMissionOutcome outcome;

	// Token: 0x040004CE RID: 1230
	public TIGameState gotoGameState;

	// Token: 0x040004CF RID: 1231
	public bool alienRelated;

	// Token: 0x040004D0 RID: 1232
	public TIDateTime dateTime;

	// Token: 0x040004D1 RID: 1233
	private TINotificationTemplate _template;

	// Token: 0x040004D2 RID: 1234
	public List<TIFactionState> timerFactions = new List<TIFactionState>();

	// Token: 0x040004D3 RID: 1235
	public List<TIFactionState> newsFeedFactions = new List<TIFactionState>();

	// Token: 0x040004D4 RID: 1236
	public List<TIFactionState> summaryLogFactions = new List<TIFactionState>();
}
