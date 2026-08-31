using System;
using System.Text;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000900 RID: 2304
	public class NotificationOptionListItem : MonoBehaviour
	{
		// Token: 0x06005836 RID: 22582 RVA: 0x00287410 File Offset: 0x00285610
		public void UpdateListItem(NotificationOptionListItem_Data data)
		{
			this.headerCategoryKey = data.categoryHeader;
			this.headerObject.SetActive(data.isCollapsibleHeader);
			this.headerText.SetText(Loc.T(new StringBuilder("UI.NotificationOption.Category.").Append(this.headerCategoryKey).ToString()));
			this.controller = data.controller;
			bool flag = false;
			if (data.isCollapsibleHeader)
			{
				TIUtilities.UpdateButtonSpritesPlusMinus(this.toggleHeaderButton, !this.controller.categoriesOpened[this.headerCategoryKey], false);
			}
			else
			{
				flag = GameControl.control.activePlayer.notificationOverrides.ContainsKey(data.notificationTemplate.dataName);
			}
			this.UpdateListItem(data.notificationTemplate, flag ? GameControl.control.activePlayer.notificationOverrides[data.notificationTemplate.dataName] : null);
		}

		// Token: 0x06005837 RID: 22583 RVA: 0x002874F4 File Offset: 0x002856F4
		public void UpdateListItem(TINotificationTemplate notificationTemplate, TINotificationTemplateOverride notificationOverride = null)
		{
			if (notificationTemplate == null)
			{
				return;
			}
			this.template = notificationTemplate;
			this.notificationName.SetText(notificationTemplate.displayName);
			this.alertBehaviorToggle.interactable = this.template.allowAlertChanges;
			this.summaryFeedBehaviorToggle.interactable = this.template.summaryAudience.category > SummaryCategory.None;
			if (notificationOverride == null)
			{
				this.alertBehaviorToggle.SetIsOnWithoutNotify(this.template.alertAudience == NotificationAudience.RelevantFactions || this.template.alertAudience == NotificationAudience.PrimaryFactions);
				this.newsFeedBehaviorToggle.SetIsOnWithoutNotify(this.template.newsFeedAudience == NotificationAudience.RelevantFactions || this.template.newsFeedAudience == NotificationAudience.PrimaryFactions);
				this.timerFeedBehaviorToggle.SetIsOnWithoutNotify(this.template.timerAudience == NotificationAudience.RelevantFactions || this.template.timerAudience == NotificationAudience.PrimaryFactions);
				this.summaryFeedBehaviorToggle.SetIsOnWithoutNotify(this.template.summaryAudience.audience == NotificationAudience.RelevantFactions || this.template.summaryAudience.audience == NotificationAudience.PrimaryFactions);
				return;
			}
			this.alertBehaviorToggle.SetIsOnWithoutNotify(this.ShowCheckmark(notificationOverride, 0));
			this.timerFeedBehaviorToggle.SetIsOnWithoutNotify(this.ShowCheckmark(notificationOverride, 1));
			this.newsFeedBehaviorToggle.SetIsOnWithoutNotify(this.ShowCheckmark(notificationOverride, 2));
			this.summaryFeedBehaviorToggle.SetIsOnWithoutNotify(this.ShowCheckmark(notificationOverride, 3));
		}

		// Token: 0x06005838 RID: 22584 RVA: 0x00287658 File Offset: 0x00285858
		private bool ShowCheckmark(TINotificationTemplateOverride templateOverride, int notificationType)
		{
			switch (notificationType)
			{
			case 0:
				return templateOverride.alert == NotificationOverrideBehavior.Add || (templateOverride.alert == NotificationOverrideBehavior.DefaultBehavior && (this.template.alertAudience == NotificationAudience.RelevantFactions || this.template.alertAudience == NotificationAudience.PrimaryFactions));
			case 1:
				return templateOverride.timerFeed == NotificationOverrideBehavior.Add || (templateOverride.timerFeed == NotificationOverrideBehavior.DefaultBehavior && (this.template.timerAudience == NotificationAudience.RelevantFactions || this.template.timerAudience == NotificationAudience.PrimaryFactions));
			case 2:
				return templateOverride.newsFeed == NotificationOverrideBehavior.Add || (templateOverride.newsFeed == NotificationOverrideBehavior.DefaultBehavior && (this.template.newsFeedAudience == NotificationAudience.RelevantFactions || this.template.newsFeedAudience == NotificationAudience.PrimaryFactions));
			case 3:
				return templateOverride.summaryFeed == NotificationOverrideBehavior.Add || (templateOverride.summaryFeed == NotificationOverrideBehavior.DefaultBehavior && (this.template.summaryAudience.audience == NotificationAudience.RelevantFactions || this.template.summaryAudience.audience == NotificationAudience.PrimaryFactions));
			default:
				Debug.LogError("Bad notification type passed");
				return false;
			}
		}

		// Token: 0x06005839 RID: 22585 RVA: 0x00287759 File Offset: 0x00285959
		public void UpdateToggle(int notificationType)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.ModifyOverride(notificationType);
		}

		// Token: 0x0600583A RID: 22586 RVA: 0x0028776E File Offset: 0x0028596E
		public void OnToggleHeaderStatus()
		{
			TIUtilities.UpdateButtonSpritesPlusMinus(this.toggleHeaderButton, !this.controller.categoriesOpened[this.headerCategoryKey], false);
			this.controller.ToggleCategoryVisibility(this.headerCategoryKey);
		}

		// Token: 0x0600583B RID: 22587 RVA: 0x002877A8 File Offset: 0x002859A8
		private void ModifyOverride(int notificationType)
		{
			switch (notificationType)
			{
			case 0:
				GameControl.control.activePlayer.playerControl.StartAction(new SetNotificationOptions(GameControl.control.activePlayer, this.template.dataName, 0, this.alertBehaviorToggle.isOn ? NotificationOverrideBehavior.Add : NotificationOverrideBehavior.Remove));
				return;
			case 1:
				GameControl.control.activePlayer.playerControl.StartAction(new SetNotificationOptions(GameControl.control.activePlayer, this.template.dataName, 1, this.timerFeedBehaviorToggle.isOn ? NotificationOverrideBehavior.Add : NotificationOverrideBehavior.Remove));
				return;
			case 2:
				GameControl.control.activePlayer.playerControl.StartAction(new SetNotificationOptions(GameControl.control.activePlayer, this.template.dataName, 2, this.newsFeedBehaviorToggle.isOn ? NotificationOverrideBehavior.Add : NotificationOverrideBehavior.Remove));
				return;
			case 3:
				GameControl.control.activePlayer.playerControl.StartAction(new SetNotificationOptions(GameControl.control.activePlayer, this.template.dataName, 3, this.summaryFeedBehaviorToggle.isOn ? NotificationOverrideBehavior.Add : NotificationOverrideBehavior.Remove));
				return;
			default:
				return;
			}
		}

		// Token: 0x04003FCA RID: 16330
		public TINotificationTemplate template;

		// Token: 0x04003FCB RID: 16331
		public TMP_Text notificationName;

		// Token: 0x04003FCC RID: 16332
		public Toggle alertBehaviorToggle;

		// Token: 0x04003FCD RID: 16333
		public Toggle newsFeedBehaviorToggle;

		// Token: 0x04003FCE RID: 16334
		public Toggle timerFeedBehaviorToggle;

		// Token: 0x04003FCF RID: 16335
		public Toggle summaryFeedBehaviorToggle;

		// Token: 0x04003FD0 RID: 16336
		public Button toggleHeaderButton;

		// Token: 0x04003FD1 RID: 16337
		public GameObject headerObject;

		// Token: 0x04003FD2 RID: 16338
		public TMP_Text headerText;

		// Token: 0x04003FD3 RID: 16339
		public string headerCategoryKey;

		// Token: 0x04003FD4 RID: 16340
		public NotificationsOptionsController controller;
	}
}
