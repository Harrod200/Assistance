using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008A8 RID: 2216
	public class NotificationsOptionsController : MonoBehaviour
	{
		// Token: 0x06005405 RID: 21509 RVA: 0x0025FDEC File Offset: 0x0025DFEC
		private void Awake()
		{
			this.activePlayer = GameControl.control.activePlayer;
		}

		// Token: 0x06005406 RID: 21510 RVA: 0x0025FDFE File Offset: 0x0025DFFE
		private void Start()
		{
			this.InitializeLoc();
		}

		// Token: 0x06005407 RID: 21511 RVA: 0x0025FE08 File Offset: 0x0025E008
		private void InitializeLoc()
		{
			this.notificationOptionsHeader.SetText(Loc.T("UI.Options.Notifications"));
			this.notificationOptionsApplyChanges.SetText(Loc.T("UI.Options.ApplyChanges"));
			this.alertBehaviorHeader.SetText(Loc.T("UI.Options.Notifications.Alerts"));
			this.newsFeedBehaviorHeader.SetText(Loc.T("UI.Options.Notifications.NewsFeed"));
			this.timerFeedBehaviorHeader.SetText(Loc.T("UI.Options.Notifications.TimerFeed"));
			this.summaryFeedBehaviorHeader.SetText(Loc.T("UI.Options.Notifications.SummaryFeed"));
			this.setDefaultsButtonText.SetText(Loc.T("UI.Options.Notifications.ResetDefault"));
			this.setProfileDefaultsButtonText.SetText(Loc.T("UI.Options.Notifications.UseProfileDefault"));
			this.applyCurrentasDefaultsButtonText.SetText(Loc.T("UI.Options.Notifications.ApplyCurrentAsDefaults"));
		}

		// Token: 0x06005408 RID: 21512 RVA: 0x0025FED2 File Offset: 0x0025E0D2
		public void InitializeNotificationOptions()
		{
			this.SetNotificationModels();
		}

		// Token: 0x06005409 RID: 21513 RVA: 0x0025FEDA File Offset: 0x0025E0DA
		public void ToggleCategoryVisibility(string categoryKey)
		{
			this.categoriesOpened[categoryKey] = !this.categoriesOpened[categoryKey];
			this.SetNotificationModels();
		}

		// Token: 0x0600540A RID: 21514 RVA: 0x0025FF00 File Offset: 0x0025E100
		public void SetNotificationModels()
		{
			List<TIObjectiveTemplate> objectivesByStatus = this.activePlayer.GetObjectivesByStatus(ObjectiveStatus.Completed);
			List<string> list = new List<string>();
			int num = 0;
			foreach (TIObjectiveTemplate tiobjectiveTemplate in objectivesByStatus)
			{
				list.Add(objectivesByStatus[num++].dataName);
			}
			List<TINotificationTemplate> list2 = new List<TINotificationTemplate>(from o in TemplateManager.IterateByClass<TINotificationTemplate>(true)
				where o.allowAnyChanges
				select o);
			list2 = (from x in list2
				orderby x.summaryAudience.category == SummaryCategory.None descending, x.summaryAudience.category.ToString()
				select x).ToList<TINotificationTemplate>();
			this.notificationModels.Clear();
			this.categoriesCreated.Clear();
			for (int i = 0; i < list2.Count; i++)
			{
				string text = list2[i].summaryAudience.category.ToString();
				if (!this.categoriesOpened.ContainsKey(text))
				{
					this.categoriesOpened.Add(text, text == SummaryCategory.None.ToString() || text == SummaryCategory.Missions.ToString());
				}
				if (!this.categoriesCreated.ContainsKey(text))
				{
					this.categoriesCreated.Add(text, text == SummaryCategory.None.ToString());
					NotificationOptionListItemModel notificationOptionListItemModel = new NotificationOptionListItemModel();
					notificationOptionListItemModel.notificationOptionListItemData = new NotificationOptionListItem_Data
					{
						isCollapsibleHeader = true,
						categoryHeader = text,
						showInList = true,
						controller = this
					};
					this.notificationModels.Add(notificationOptionListItemModel);
				}
				NotificationOptionListItemModel notificationOptionListItemModel2 = new NotificationOptionListItemModel();
				NotificationOptionListItem_Data notificationOptionListItem_Data = new NotificationOptionListItem_Data();
				notificationOptionListItem_Data.isCollapsibleHeader = false;
				notificationOptionListItem_Data.showInList = this.categoriesOpened[text];
				if (list2[i].unlockingObjectives != null)
				{
					if (!list2[i].unlockingObjectives.Any<string>((string o) => string.IsNullOrEmpty(o)))
					{
						if (!list2[i].unlockingObjectives.Where<string>((string o) => !string.IsNullOrEmpty(o)).Intersect<string>(list).Any<string>())
						{
							notificationOptionListItem_Data.showInList = false;
						}
					}
				}
				bool flag = this.activePlayer.notificationOverrides.ContainsKey(list2[i].dataName);
				notificationOptionListItem_Data.SetNotificationOptionData(list2[i], this, flag ? this.activePlayer.notificationOverrides[list2[i].dataName] : null);
				notificationOptionListItemModel2.notificationOptionListItemData = notificationOptionListItem_Data;
				this.notificationModels.Add(notificationOptionListItemModel2);
			}
			this.notificationOptionListAdapter.SetItems(this.notificationModels);
		}

		// Token: 0x0600540B RID: 21515 RVA: 0x00260238 File Offset: 0x0025E438
		public void SetDefaults()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			foreach (NotificationOptionListItemModel notificationOptionListItemModel in this.notificationModels)
			{
				if (notificationOptionListItemModel.notificationOptionListItemData.notificationTemplate != null)
				{
					this.activePlayer.playerControl.StartAction(new SetNotificationOptions(this.activePlayer, notificationOptionListItemModel.notificationOptionListItemData.notificationTemplate.dataName, 0, NotificationOverrideBehavior.DefaultBehavior));
					this.activePlayer.playerControl.StartAction(new SetNotificationOptions(this.activePlayer, notificationOptionListItemModel.notificationOptionListItemData.notificationTemplate.dataName, 1, NotificationOverrideBehavior.DefaultBehavior));
					this.activePlayer.playerControl.StartAction(new SetNotificationOptions(this.activePlayer, notificationOptionListItemModel.notificationOptionListItemData.notificationTemplate.dataName, 2, NotificationOverrideBehavior.DefaultBehavior));
					this.activePlayer.playerControl.StartAction(new SetNotificationOptions(this.activePlayer, notificationOptionListItemModel.notificationOptionListItemData.notificationTemplate.dataName, 3, NotificationOverrideBehavior.DefaultBehavior));
				}
			}
			this.activePlayer.notificationOverrides.Clear();
			this.InitializeNotificationOptions();
		}

		// Token: 0x0600540C RID: 21516 RVA: 0x00260370 File Offset: 0x0025E570
		public void SetUserDefaults()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			for (int i = 0; i < TIPlayerProfileManager.notificationTemplates.Count; i++)
			{
				this.activePlayer.playerControl.StartAction(new SetNotificationOptions(this.activePlayer, TIPlayerProfileManager.notificationTemplates[i].dataName, 0, TIPlayerProfileManager.notificationOverrides[i].alert));
				this.activePlayer.playerControl.StartAction(new SetNotificationOptions(this.activePlayer, TIPlayerProfileManager.notificationTemplates[i].dataName, 1, TIPlayerProfileManager.notificationOverrides[i].timerFeed));
				this.activePlayer.playerControl.StartAction(new SetNotificationOptions(this.activePlayer, TIPlayerProfileManager.notificationTemplates[i].dataName, 2, TIPlayerProfileManager.notificationOverrides[i].newsFeed));
				this.activePlayer.playerControl.StartAction(new SetNotificationOptions(this.activePlayer, TIPlayerProfileManager.notificationTemplates[i].dataName, 3, TIPlayerProfileManager.notificationOverrides[i].summaryFeed));
			}
			this.InitializeNotificationOptions();
		}

		// Token: 0x0600540D RID: 21517 RVA: 0x0026049C File Offset: 0x0025E69C
		public void ApplyCurrentSettingsToProfile()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			int i;
			int j;
			for (i = 0; i < TIPlayerProfileManager.notificationTemplates.Count; i = j + 1)
			{
				TINotificationTemplateOverride notificationTemplateOverride = this.notificationModels.Where<NotificationOptionListItemModel>(delegate(NotificationOptionListItemModel x)
				{
					NotificationOptionListItem_Data notificationOptionListItemData = x.notificationOptionListItemData;
					string text;
					if (notificationOptionListItemData == null)
					{
						text = null;
					}
					else
					{
						TINotificationTemplate notificationTemplate = notificationOptionListItemData.notificationTemplate;
						text = ((notificationTemplate != null) ? notificationTemplate.dataName : null);
					}
					return text == TIPlayerProfileManager.notificationTemplates[i].dataName;
				}).First<NotificationOptionListItemModel>().notificationOptionListItemData.notificationTemplateOverride;
				if (notificationTemplateOverride != null)
				{
					TIPlayerProfileManager.notificationOverrides[i].alert = notificationTemplateOverride.alert;
					TIPlayerProfileManager.notificationOverrides[i].newsFeed = notificationTemplateOverride.newsFeed;
					TIPlayerProfileManager.notificationOverrides[i].timerFeed = notificationTemplateOverride.timerFeed;
					TIPlayerProfileManager.notificationOverrides[i].summaryFeed = notificationTemplateOverride.summaryFeed;
				}
				j = i;
			}
			TIPlayerProfileManager.SavePlayerConfig();
		}

		// Token: 0x04003A3F RID: 14911
		public NotificationOptionListAdapter notificationOptionListAdapter;

		// Token: 0x04003A40 RID: 14912
		public List<NotificationOptionListItemModel> notificationModels = new List<NotificationOptionListItemModel>();

		// Token: 0x04003A41 RID: 14913
		public TMP_Text notificationOptionsHeader;

		// Token: 0x04003A42 RID: 14914
		public TMP_Text notificationOptionsApplyChanges;

		// Token: 0x04003A43 RID: 14915
		public TMP_Text alertBehaviorHeader;

		// Token: 0x04003A44 RID: 14916
		public TMP_Text newsFeedBehaviorHeader;

		// Token: 0x04003A45 RID: 14917
		public TMP_Text timerFeedBehaviorHeader;

		// Token: 0x04003A46 RID: 14918
		public TMP_Text summaryFeedBehaviorHeader;

		// Token: 0x04003A47 RID: 14919
		public TMP_Text setDefaultsButtonText;

		// Token: 0x04003A48 RID: 14920
		public TMP_Text setProfileDefaultsButtonText;

		// Token: 0x04003A49 RID: 14921
		public TMP_Text applyCurrentasDefaultsButtonText;

		// Token: 0x04003A4A RID: 14922
		public Dictionary<string, bool> categoriesOpened = new Dictionary<string, bool>();

		// Token: 0x04003A4B RID: 14923
		private Dictionary<string, bool> categoriesCreated = new Dictionary<string, bool>();

		// Token: 0x04003A4C RID: 14924
		private TIFactionState activePlayer;
	}
}
