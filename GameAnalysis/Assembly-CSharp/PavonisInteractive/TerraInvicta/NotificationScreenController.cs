using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FMOD.Studio;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008A6 RID: 2214
	public class NotificationScreenController : CanvasControllerBase
	{
		// Token: 0x17000EF4 RID: 3828
		// (get) Token: 0x0600538D RID: 21389 RVA: 0x00256C60 File Offset: 0x00254E60
		// (set) Token: 0x0600538E RID: 21390 RVA: 0x00256C67 File Offset: 0x00254E67
		public static NotificationScreenController singleton { get; private set; }

		// Token: 0x0600538F RID: 21391 RVA: 0x00256C70 File Offset: 0x00254E70
		public override void Initialize()
		{
			base.Initialize();
			NotificationScreenController.singleton = this;
			this.singleAlertBox.SetActive(false);
			this.masterPolicyPanelObject.SetActive(false);
			this.factionDiplomacyGreetingUIObject.SetActive(false);
			this.heldNewsItems = new List<NotificationQueueItem>();
			this.ExpandedNewsFeedSettings();
			GameControl.eventManager.AddListener<InfoPanelOpened>(new EventManager.EventDelegate<InfoPanelOpened>(this.ContractNewsFeed), null, null, false, false);
			GameControl.eventManager.AddListener<InfoWindowEntirelyClosed>(new EventManager.EventDelegate<InfoWindowEntirelyClosed>(this.ExpandNewsFeed), null, null, false, false);
			GameControl.eventManager.AddListener<TradeToPlayerInitiated>(new EventManager.EventDelegate<TradeToPlayerInitiated>(this.StartAIToPlayerDiplomacyMission), null, null, false, false);
			this.promptQueue = GameStateManager.FindGameState<TIPromptQueueState>();
			this.newsQueue = GameStateManager.FindGameState<TINotificationQueueState>();
			this.newsList.SetListSize<NewsFeedListItemController>(30, false, false);
			this.timerList.SetListSize<NewsFeedListItemController>(6, false, false);
			foreach (object obj in Enum.GetValues(typeof(SummaryCategory)))
			{
				SummaryCategory summaryCategory = (SummaryCategory)obj;
				if (summaryCategory != SummaryCategory.None)
				{
					this.summaryLogs[summaryCategory - SummaryCategory.Missions].SetListSize<NotificationSummaryItemController>(this.maxSummaryListItems[(int)summaryCategory], false, false);
				}
			}
			this.confirmButtonText.SetText(Loc.T("UI.Notifications.Confirm"));
			this.cancelButtonText.SetText(Loc.T("UI.Notifications.Cancel"));
			this.responseConfirmButtonText.SetText(Loc.T("UI.Notifications.Confirm"));
			this.responseDeclineButtonText.SetText(Loc.T("UI.Notifications.Decline"));
			this.backButtonText.SetText(Loc.T("UI.Notifications.Back"));
			this.allyAcceptButtonText.SetText(Loc.T("UI.Notifications.AllyJoinWar"));
			this.allyDeclineButtonText.SetText(Loc.T("UI.Notifications.AllyDeclineWar"));
			this.oldControlPointTextHeader.SetText(Loc.T("UI.Notifications.oldControlPoints"));
			this.newControlPointTextHeader.SetText(Loc.T("UI.Notifications.newControlPoints"));
			this.okayButtonText.text = Loc.T("UI.Notifications.ContinueButtonText");
			this.closeButtonText.SetText(Loc.T("UI.Notifications.CloseButtonText"));
			this.gotoButtonText.text = Loc.T("UI.Notifications.GotoButtonText");
			this.missionTargetConfirmButtonText.SetText(Loc.T("UI.Notifications.Confirm"));
			this.missionTargetCancelButtonText.SetText(Loc.T("UI.Notifications.Cancel"));
			this.summaryLogHeaderText.SetText(Loc.T("UI.Notifications.MissionPhaseReport"));
			this.alertOptionHeader.SetText(Loc.T("UI.Options.Notifications.Alerts"));
			this.newsFeedOptionHeader.SetText(Loc.T("UI.Options.Notifications.NewsFeed"));
			this.timerFeedOptionHeader.SetText(Loc.T("UI.Options.Notifications.TimerFeed"));
			this.summaryFeedOptionHeader.SetText(Loc.T("UI.Options.Notifications.SummaryFeed"));
			this.currentNotificationSettingTooltip.SetDelegate("BodyText", () => Loc.T("UI.Options.Notifications.CurrentOption"));
			this.altCurrentNotificationSettingTooltip.SetDelegate("BodyText", () => Loc.T("UI.Options.Notifications.CurrentOption"));
			this.alertsNotificationSettingTooltip.SetDelegate("BodyText", () => Loc.T("UI.Options.Notifications.AlertsTooltip"));
			this.newsFeedNotificationSettingTooltip.SetDelegate("BodyText", () => Loc.T("UI.Options.Notifications.NewsFeedTooltip"));
			this.timerFeedNotificationSettingTooltip.SetDelegate("BodyText", () => Loc.T("UI.Options.Notifications.TimerFeedTooltip"));
			this.summaryFeedNotificationSettingTooltip.SetDelegate("BodyText", () => Loc.T("UI.Options.Notifications.SummaryFeedTooltip"));
			this.customDelegateDropdownToggleText.SetText(Loc.T("UI.Habs.ReplaceAllModules"));
			this.notificationVideo.playOnAwake = false;
			this.notificationOptionsPanel.SetActive(false);
			this.missionTargetingUIObject.SetActive(false);
			this.illustration.gameObject.SetActive(true);
			this.timerPanelObject.SetActive(true);
			this.UpdateActivePlayerUIElements(true);
			this.removeArmiesPromptObject.SetActive(false);
			this.sendArmiesHomeButtonText.SetText(Loc.T("UI.Notifications.GoHomeArmiesPrompt"));
			this.factionDiplomacyGreetingContinueButtonText.SetText(Loc.T("UI.StartScreen.Continue"));
			this.summaryLogReportObject.SetActive(false);
		}

		// Token: 0x06005390 RID: 21392 RVA: 0x002570F8 File Offset: 0x002552F8
		public override void UpdateActivePlayerUIElements(bool startup)
		{
			if (!startup)
			{
				GameControl.eventManager.RemoveListener<NewsItemCreated>(new EventManager.EventDelegate<NewsItemCreated>(this.UpdateNewsFeed), null);
				GameControl.eventManager.RemoveListener<RapidLogItemCreated>(new EventManager.EventDelegate<RapidLogItemCreated>(this.UpdateLogs), null);
			}
			GameControl.eventManager.AddListener<NewsItemCreated>(new EventManager.EventDelegate<NewsItemCreated>(this.UpdateNewsFeed), null, base.activePlayer, !TemplateManager.global.immediateNewsAlert, false);
			GameControl.eventManager.AddListener<RapidLogItemCreated>(new EventManager.EventDelegate<RapidLogItemCreated>(this.UpdateLogs), null, base.activePlayer, true, false);
			this.SetTimerList(base.activePlayer);
			this.SetNewsList(base.activePlayer);
			foreach (object obj in Enum.GetValues(typeof(SummaryCategory)))
			{
				SummaryCategory summaryCategory = (SummaryCategory)obj;
				if (summaryCategory != SummaryCategory.None)
				{
					this.SetSummaryLogReport(summaryCategory);
				}
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(new StringBuilder(base.activePlayer.template.winMissionPath).Append("_off").ToString(), this.factionMissionListIcon);
		}

		// Token: 0x06005391 RID: 21393 RVA: 0x00257224 File Offset: 0x00255424
		public override void Show()
		{
			base.Show();
			this.notificationCanvas.enabled = true;
			this.newsFeedCanvas.enabled = true;
		}

		// Token: 0x06005392 RID: 21394 RVA: 0x00257244 File Offset: 0x00255444
		public override void Hide()
		{
			base.Hide();
			this.notificationCanvas.enabled = false;
			this.newsFeedCanvas.enabled = false;
		}

		// Token: 0x17000EF5 RID: 3829
		// (get) Token: 0x06005393 RID: 21395 RVA: 0x00257264 File Offset: 0x00255464
		private bool OkayToPushNextNotification
		{
			get
			{
				return !this.singleAlertBox.activeSelf && !this.masterPolicyPanelObject.activeSelf && !this.responsePanelObject.activeSelf && !this.callAllyResponseObject.activeSelf && !this.missionTargetingUIObject.activeSelf;
			}
		}

		// Token: 0x06005394 RID: 21396 RVA: 0x002572B8 File Offset: 0x002554B8
		public override void Refresh()
		{
			if (this.OkayToPushNextNotification)
			{
				if (this.promptQueue.activePlayerNationPromptList.Count > 0)
				{
					this.PushNationPromptResponse();
					return;
				}
				if (this.promptQueue.activePlayerFactionPromptList.Count > 0 && (TIPromptQueueState.PlayerMissionPrompt(this.promptQueue.activePlayerFactionPromptList[0]) || TIPromptQueueState.PlayerOperationPrompt(this.promptQueue.activePlayerFactionPromptList[0])))
				{
					if (TIPromptQueueState.PlayerMissionPrompt(this.promptQueue.activePlayerFactionPromptList[0]))
					{
						this.PushMissionPromptResponse(this.promptQueue.activePlayerFactionPromptList[0]);
					}
					if (this.promptQueue.activePlayerFactionPromptList.Count > 0 && TIPromptQueueState.PlayerOperationPrompt(this.promptQueue.activePlayerFactionPromptList[0]))
					{
						this.PushOperationPromptResponse(this.promptQueue.activePlayerFactionPromptList[0]);
						return;
					}
				}
				else if (this.heldNewsItems.Count > 0)
				{
					this.PushNextAlert();
					return;
				}
			}
			else if (this.singleAlertBox.activeSelf && !this.okayButtonObject.activeSelf && !this.masterPolicyPanelObject.activeSelf && !this.responsePanelObject.activeSelf && !this.callAllyResponseObject.activeSelf && !this.missionTargetingUIObject.activeSelf)
			{
				this.okayButtonObject.SetActive(this.currentItem.alertBlockFaction != base.activePlayer && !TIPromptQueueState.anyBlockingPrompt && !this.currentItem.triggerEndGame);
			}
		}

		// Token: 0x06005395 RID: 21397 RVA: 0x0025744C File Offset: 0x0025564C
		private void UpdateNewsFeed(NewsItemCreated e)
		{
			if (!e.newItem.narrativeEventAlert)
			{
				if (e.newItem.putInTimerQueue)
				{
					this.UpdateTimerList(base.activePlayer, e.newSummary);
				}
				if (e.newItem.putInNewsFeed)
				{
					this.UpdateNewsList(base.activePlayer, e.newSummary);
				}
				if (e.newItem.putInSummaryLog)
				{
					this.UpdateSummaryLogReport(base.activePlayer, e.newSummary);
				}
			}
			NotificationQueueItem newItem = e.newItem;
			List<TIFactionState> alertFactions = newItem.alertFactions;
			if (alertFactions.Contains(base.activePlayer) || newItem.alertBlockFaction == base.activePlayer)
			{
				this.heldNewsItems.Add(newItem);
			}
			else
			{
				if (!string.IsNullOrEmpty(newItem.soundToPlay) && newItem.putInTimerQueue)
				{
					AudioManager.PlayOneShot(newItem.soundToPlay, false, false);
				}
				if (newItem.musicIntensityDelta != 0f && alertFactions.Count == 0)
				{
					float num = 0f;
					float num2 = AudioManager.GetIntensity() + newItem.musicIntensityDelta;
					if (newItem.musicIntensityDelta < 0f)
					{
						num = GameStateManager.NotificationQueue().GetBaselineMusicIntensity(base.activePlayer);
					}
					AudioManager.SetIntensity(Mathf.Clamp(num2, num, 1f));
				}
			}
			if (this._tutorialNewsItemCounter == 10)
			{
				this.newsFeedTutorial.HoldTutorial(CampaignMilestone.UITutorial_NewsFeed, false, true);
				return;
			}
			this._tutorialNewsItemCounter++;
		}

		// Token: 0x06005396 RID: 21398 RVA: 0x002575A4 File Offset: 0x002557A4
		private void UpdateLogs(RapidLogItemCreated e)
		{
			if (e.newSummary.timerFactions.Contains(base.activePlayer))
			{
				this.UpdateTimerList(base.activePlayer, e.newSummary);
			}
			if (e.newSummary.newsFeedFactions.Contains(base.activePlayer))
			{
				this.UpdateNewsList(base.activePlayer, e.newSummary);
			}
			if (e.newSummary.summaryLogFactions.Contains(base.activePlayer))
			{
				this.UpdateSummaryLogReport(base.activePlayer, e.newSummary);
			}
		}

		// Token: 0x06005397 RID: 21399 RVA: 0x00257630 File Offset: 0x00255830
		private void PushNextAlert()
		{
			this.currentItem = this.heldNewsItems[0];
			if (this.currentItem.template.stacking != StackingBehavior.None && this.currentItem.alertFactions.Count == 1 && NotificationScreenController.<PushNextAlert>g__PassDelegates|192_0(this.currentItem) && this.heldNewsItems.Count > 1)
			{
				bool flag = false;
				StringBuilder stringBuilder = new StringBuilder(this.currentItem.itemDetail);
				if (stringBuilder.Length > 100)
				{
					flag = true;
				}
				stringBuilder.AppendLine();
				int num = 0;
				foreach (NotificationQueueItem notificationQueueItem in this.heldNewsItems.ToList<NotificationQueueItem>())
				{
					if (notificationQueueItem != this.currentItem && notificationQueueItem.alertFactions.Count == 1 && notificationQueueItem.gotoGameState != null && notificationQueueItem.templateName == this.currentItem.templateName && notificationQueueItem.alertFactions[0] == this.currentItem.alertFactions[0] && NotificationScreenController.<PushNextAlert>g__PassDelegates|192_0(notificationQueueItem))
					{
						bool flag2 = false;
						switch (this.currentItem.template.stacking)
						{
						case StackingBehavior.SameRegion:
							flag2 = this.currentItem.gotoGameState.ref_region == notificationQueueItem.gotoGameState.ref_region;
							break;
						case StackingBehavior.SameNation:
							flag2 = this.currentItem.gotoGameState.ref_nation == notificationQueueItem.gotoGameState.ref_nation;
							break;
						case StackingBehavior.SameHab:
							flag2 = this.currentItem.gotoGameState.ref_hab == notificationQueueItem.gotoGameState.ref_hab;
							break;
						case StackingBehavior.SameFleet:
							flag2 = this.currentItem.gotoGameState.ref_fleet == notificationQueueItem.gotoGameState.ref_fleet;
							break;
						case StackingBehavior.SameCouncilor:
							flag2 = this.currentItem.gotoGameState.ref_councilor == notificationQueueItem.gotoGameState.ref_councilor;
							break;
						}
						if (flag2)
						{
							if (flag)
							{
								stringBuilder.AppendLine(Loc.T("UI.Global.AltDivCentered"));
							}
							stringBuilder.AppendLine(notificationQueueItem.itemDetail);
							this.heldNewsItems.Remove(notificationQueueItem);
							num++;
						}
					}
				}
				if (num > 0)
				{
					this.currentItem.itemDetail = stringBuilder.ToString();
				}
			}
			this.MaximizeNotificationWindow();
			this.PrepAlert(this.currentItem);
			if (!string.IsNullOrEmpty(this.currentItem.soundToPlay))
			{
				if (this.currentItem.soundToPlay.Contains("/Faction/"))
				{
					if (AudioManager.VerifyPath(this.currentItem.soundToPlay, true))
					{
						this.voEventInstance = AudioManager.CreateFMODInstance(this.currentItem.soundToPlay);
						if (this.voEventInstance.isValid() && !this.voEventInstance.IsPlaying())
						{
							this.voEventInstance.Play(base.gameObject);
						}
					}
				}
				else
				{
					AudioManager.PlayOneShot(this.currentItem.soundToPlay, false, false);
				}
			}
			if (!string.IsNullOrEmpty(this.currentItem.fanfareToPlay))
			{
				MusicController.Instance.PlayFanfare(this.currentItem.fanfareToPlay);
			}
			this.singleAlertBox.SetActive(true);
			Action onOpenNotification = this.currentItem.OnOpenNotification;
			if (onOpenNotification != null)
			{
				onOpenNotification();
			}
			if (!string.IsNullOrEmpty(this.currentItem.animationSpriteSheetPath))
			{
				this.alertRightImageAnimator.Play();
			}
			if (!string.IsNullOrEmpty(this.currentItem.videoResource))
			{
				TIUtilities.TryPrepareVideo(this.notificationVideo);
				base.StartCoroutine(this.PlayVideoWhenPrepared(this.notificationVideo));
			}
			base.gameTime.Pause();
			if (this.currentItem.musicIntensityDelta != 0f && this.currentItem.alertFactions.Contains(base.activePlayer))
			{
				float num2 = 0f;
				float num3 = AudioManager.GetIntensity() + this.currentItem.musicIntensityDelta;
				if (this.currentItem.musicIntensityDelta < 0f)
				{
					num2 = GameStateManager.NotificationQueue().GetBaselineMusicIntensity(base.activePlayer);
				}
				AudioManager.SetIntensity(Mathf.Clamp(num3, num2, 1f));
			}
			if (this.currentItem.narrativeEventAlert)
			{
				GameControl.eventManager.TriggerEvent(new NarrativeEventPushedToPlayer(), null, Array.Empty<object>());
				this.currentNarrativeEvent = new CurrentNarrativeEventData(this.currentItem.relatedTemplate as TINarrativeEventTemplate, base.activePlayer, this.currentItem.promptingGameState, this.currentItem.alertRelatedState, this.currentItem.allNarrativeEventTargetsAndSeconds);
				this.narrativeEventButtonsPanel.SetActive(true);
				this.alertButtonsPanel.SetActive(false);
				this.exitButtonObject.SetActive(false);
				this.DisableAllDelegatePanelObjects();
				this.FillOutOptionButtons(this.currentItem.relatedTemplate as TINarrativeEventTemplate, this.currentItem.promptingGameState, this.currentItem.alertRelatedState, this.currentItem.allNarrativeEventTargetsAndSeconds);
				GameControl.eventManager.AddListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnResourcesUpdatedWhileNarrativeEventActive), null, base.activePlayer, true, false);
			}
			else
			{
				if (GameStateManager.MissionPhase().newCampaignStart && TIMissionPhaseState.timeToNextMissionPhase_d <= 1.0)
				{
					this.gotoButtonObject.SetActive(false);
					this.closeButtonObject.SetActive(false);
					this.exitButtonObject.SetActive(false);
					this.alertButtonsPanel.SetActive(true);
					this.okayButtonObject.SetActive(true);
					this.narrativeEventButtonsPanel.SetActive(false);
					this.DisableAllDelegatePanelObjects();
				}
				else
				{
					bool flag3 = this.currentItem.alertBlockFaction != base.activePlayer && !TIPromptQueueState.anyBlockingPrompt;
					bool flag4 = TIGameState.Valid(this.currentItem.gotoGameState);
					bool flag5 = this.currentItem.alertBlockFaction == base.activePlayer && flag4 && GameStateManager.PromptQueue().activePlayerFactionPromptList.Any<Prompt>((Prompt x) => x.name == this.currentItem.alertBlockEventName);
					this.currentItem.alertBlockFaction != base.activePlayer;
					this.heldNewsItems.Count<NotificationQueueItem>((NotificationQueueItem x) => x.alertFactions.Contains(base.activePlayer));
					bool triggerEndGame = this.currentItem.triggerEndGame;
					this.alertButtonsPanel.SetActive(true);
					this.exitButtonObject.SetActive(!triggerEndGame);
					this.narrativeEventButtonsPanel.SetActive(false);
					this.okayButtonObject.SetActive(!triggerEndGame && flag3 && !flag5);
					this.gotoButtonObject.SetActive(flag4 && !triggerEndGame);
					this.closeButtonObject.SetActive(!flag5 || (!this.okayButtonObject.activeSelf && !this.gotoButtonObject.activeSelf) || triggerEndGame);
					if (this.okayButtonObject.activeSelf)
					{
						this.okayButton.interactable = false;
					}
					base.StartCoroutine(this.EnableNarrativeButtonWithDelay(this.okayButton));
					if (this.gotoButtonObject.activeSelf)
					{
						this.gotoButton.interactable = false;
					}
					base.StartCoroutine(this.EnableNarrativeButtonWithDelay(this.gotoButton));
					if (this.closeButtonObject.activeSelf)
					{
						this.closeButton.interactable = false;
					}
					base.StartCoroutine(this.EnableNarrativeButtonWithDelay(this.closeButton));
					if (this.gotoButtonObject.activeInHierarchy)
					{
						if (this.currentItem.gotoGameState == GameStateManager.MissionPhase())
						{
							this.gotoButtonText.SetText(Loc.T("UI.Notifications.GotoButtonTextObjectives"));
						}
						else if (this.currentItem.gotoGameState == GameStateManager.GlobalResearch())
						{
							this.gotoButtonText.SetText(Loc.T("UI.Notifications.GotoButtonTextResearch"));
						}
						else if (this.currentItem.gotoGameState == GameStateManager.GlobalValues())
						{
							this.gotoButtonText.SetText(Loc.T("UI.Notifications.GotoButtonTextIntel"));
						}
						else
						{
							this.gotoButtonText.SetText(Loc.T("UI.Notifications.GotoButtonText"));
						}
					}
					if (this.currentItem.notificationDelegates.Count > 0)
					{
						this.SetCustomNotificationButtons(this.currentItem);
					}
					else
					{
						this.DisableAllDelegatePanelObjects();
					}
				}
				TINotificationQueueState.CheckAndSetFirstNotificationOfType(this.heldNewsItems[0]);
			}
			this.notificationPushTime = Time.time;
		}

		// Token: 0x06005398 RID: 21400 RVA: 0x00257EA8 File Offset: 0x002560A8
		private void PrepAlert(NotificationQueueItem newsItem)
		{
			this.hammerText.SetText(newsItem.itemHammer);
			bool flag;
			if (!string.IsNullOrEmpty(newsItem.itemHeadline))
			{
				this.alertHeadlineText.SetText(newsItem.itemHeadline);
				this.alertHeadline.SetActive(true);
				flag = false;
				this.notificationOptionsPanel.transform.SetParent(this.alertHeadline.transform);
				(this.notificationOptionsPanel.transform as RectTransform).anchoredPosition = Vector2.zero;
			}
			else
			{
				this.alertHeadline.SetActive(false);
				flag = true;
				this.notificationOptionsPanel.transform.SetParent(this.corePanel.transform);
				(this.notificationOptionsPanel.transform as RectTransform).anchoredPosition = Vector2.zero;
			}
			Dictionary<TIFactionState, string> factionSpecificDetail = newsItem.factionSpecificDetail;
			if (factionSpecificDetail != null && factionSpecificDetail.ContainsKey(base.activePlayer))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(newsItem.itemDetail).AppendLine().AppendLine(newsItem.factionSpecificDetail[base.activePlayer]);
				this.alertBodyText.SetText(stringBuilder);
			}
			else
			{
				this.alertBodyText.text = newsItem.itemDetail;
			}
			bool flag2 = !string.IsNullOrEmpty(newsItem.illustrationResource);
			bool flag3;
			if (!(newsItem.illustrationResource == World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath))
			{
				string illustrationResource = newsItem.illustrationResource;
				TIGameState gotoGameState = newsItem.gotoGameState;
				string text;
				if (gotoGameState == null)
				{
					text = null;
				}
				else
				{
					TIHabSiteState ref_habSite = gotoGameState.ref_habSite;
					text = ((ref_habSite != null) ? ref_habSite.template.backgroundPath : null);
				}
				flag3 = illustrationResource == text;
			}
			else
			{
				flag3 = true;
			}
			bool flag4 = flag3;
			bool flag5 = flag2 && !flag4;
			bool flag6 = !string.IsNullOrEmpty(newsItem.videoResource);
			bool flag7 = !string.IsNullOrEmpty(newsItem.movieResource);
			bool flag8;
			if (!flag6 && !flag5)
			{
				TIGameState gotoGameState2 = newsItem.gotoGameState;
				flag8 = gotoGameState2 != null && gotoGameState2.isSpaceObjectState;
			}
			else
			{
				flag8 = false;
			}
			bool flag9 = flag8;
			if (flag9 && !flag2)
			{
				newsItem.illustrationResource = World.Active.GetExistingManager<CameraManager>().skyboxBackdropPath;
				flag2 = true;
			}
			if (flag4 && newsItem.gotoGameState == null)
			{
				flag2 = false;
			}
			bool flag10 = flag2 || flag9 || flag6;
			bool flag11 = flag6 || newsItem.showSideArt;
			bool flag12 = !flag11;
			bool flag13 = !string.IsNullOrEmpty(newsItem.popupResource2);
			bool flag14 = !string.IsNullOrEmpty(newsItem.animationSpriteSheetPath);
			bool flag15 = (flag6 && flag13) || newsItem.showSideArt;
			bool flag16 = flag13 && !flag15 && !flag14;
			bool flag17 = !flag6 && flag14;
			bool flag18 = flag16 || flag17 || flag;
			this.illustrationObject.SetActive(flag10);
			this.alertLeftImageBackground.gameObject.SetActive(flag12);
			this.alertRightImage.gameObject.SetActive(flag16);
			this.alertRightImagePanelObject.SetActive(flag18);
			this.leftVideoSmallImage.enabled = flag11;
			this.rightVideoSmallImage.enabled = flag15;
			this.alertRightAnimatedImage.gameObject.SetActive(flag17);
			this.alertLeftImageBackground.enabled = false;
			this.leftVideoSmallImageBackground.enabled = false;
			if (flag2)
			{
				this.illustration.color = new Color(1f, 1f, 1f, 1f);
				this.illustrationObjectBackgroundImage.enabled = true;
				if (flag9)
				{
					this.illustration.enabled = false;
					GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.illustrationResource, this.maskedIllustrationImage);
					this.maskedIllustrationImage.rectTransform.localPosition = new Vector3((float)TIUtilities.RandomRange(-190, 190), (float)TIUtilities.RandomRange(-95, 95), 0f);
					this.maskedIllustrationImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, (float)TIUtilities.RandomRange(0, 359));
					this.maskedIllustrationObject.SetActive(true);
				}
				else
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.illustrationResource, this.illustration);
					this.illustration.enabled = true;
					this.maskedIllustrationObject.SetActive(false);
				}
			}
			else
			{
				this.illustration.sprite = null;
				this.illustration.enabled = false;
				this.maskedIllustrationObject.SetActive(false);
			}
			if (flag6)
			{
				this.illustrationObjectBackgroundImage.enabled = false;
				this.notificationVideo.gameObject.SetActive(true);
				this.notificationVideo.enabled = true;
				this.notificationVideo.clip = GameControl.assetLoader.LoadAsset<VideoClip>(newsItem.videoResource);
			}
			else
			{
				this.notificationVideo.Stop();
				this.notificationVideo.gameObject.SetActive(false);
				this.notificationVideo.enabled = false;
				this.notificationVideo.targetTexture.Release();
			}
			if (flag7 && !TemplateManager.global.dontPlayCinematicVideos)
			{
				Cinematic2DController component = this.cinematicObject.GetComponent<Cinematic2DController>();
				this.cinematicObject.SetActive(true);
				this.cinematicVideoPlayer.clip = GameControl.assetLoader.LoadAsset<VideoClip>(newsItem.movieResource);
				this.cinematicVideoPlayer.SetDirectAudioVolume(0, TIPlayerProfileManager.masterVolumeModifier());
				TIUtilities.TryPrepareVideo(this.cinematicVideoPlayer);
				component.cinematicPathString = newsItem.movieResource;
				component.audioPath = newsItem.soundToPlay;
				component.StartCoroutine(component.BeginWhenPrepared(true, false));
			}
			if (flag9 && TIGameState.Valid(newsItem.gotoGameState))
			{
				this.cameraViewObject.SetActive(true);
				this.illustrationObjectBackgroundImage.enabled = true;
				this.UpdateCameraImage(newsItem.gotoGameState.ref_spaceObject);
			}
			else
			{
				this.cameraViewObject.SetActive(false);
			}
			if (newsItem.template.allowAnyChanges)
			{
				if (base.activePlayer.notificationOverrides.ContainsKey(newsItem.template.dataName))
				{
					this.currentNotificationOptionItem.UpdateListItem(newsItem.template, base.activePlayer.notificationOverrides[newsItem.template.dataName]);
				}
				else
				{
					this.currentNotificationOptionItem.UpdateListItem(newsItem.template, null);
				}
				this.currentNotificationOptionName.SetText(newsItem.template.displayName);
				if (!flag)
				{
					this.openNotificationOptionPanelButton.gameObject.SetActive(true);
					this.altOpenNotificationOptionPanelButton.gameObject.SetActive(false);
				}
				else
				{
					this.openNotificationOptionPanelButton.gameObject.SetActive(false);
					this.altOpenNotificationOptionPanelButton.gameObject.SetActive(true);
				}
			}
			else
			{
				this.openNotificationOptionPanelButton.gameObject.SetActive(false);
				this.altOpenNotificationOptionPanelButton.gameObject.SetActive(false);
			}
			if (flag11)
			{
				if (!string.IsNullOrEmpty(newsItem.popupResource1))
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.popupResource1, this.leftVideoSmallImage);
					if (!string.IsNullOrEmpty(newsItem.popup1BackgroundResource))
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.popup1BackgroundResource, this.leftVideoSmallImageBackground);
						this.leftVideoSmallImageBackground.enabled = true;
						this.leftVideoSmallImageBackground.color = newsItem.backgroundColor;
					}
					else
					{
						this.leftVideoSmallImageBackground.enabled = false;
					}
				}
				else
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.icon, this.leftVideoSmallImage);
					if (!string.IsNullOrEmpty(newsItem.iconBackgroundResource))
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.iconBackgroundResource, this.leftVideoSmallImageBackground);
						this.leftVideoSmallImageBackground.enabled = true;
						this.leftVideoSmallImageBackground.color = newsItem.backgroundColor;
					}
					else
					{
						this.leftVideoSmallImageBackground.enabled = false;
					}
				}
			}
			if (flag15)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.popupResource2, this.rightVideoSmallImage);
			}
			if (flag12)
			{
				if (!string.IsNullOrEmpty(newsItem.popupResource1))
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.popupResource1, this.alertLeftImage);
					if (!string.IsNullOrEmpty(newsItem.popup1BackgroundResource))
					{
						this.alertLeftImageBackground.enabled = true;
						GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.popup1BackgroundResource, this.alertLeftImageBackground);
						this.alertLeftImageBackground.color = newsItem.backgroundColor;
					}
					else
					{
						this.alertLeftImageBackground.enabled = false;
					}
				}
				else
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.icon, this.alertLeftImage);
					if (!string.IsNullOrEmpty(newsItem.iconBackgroundResource))
					{
						this.alertLeftImageBackground.enabled = true;
						GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.iconBackgroundResource, this.alertLeftImageBackground);
						this.alertLeftImageBackground.color = newsItem.backgroundColor;
					}
					else
					{
						this.alertLeftImageBackground.enabled = false;
					}
				}
			}
			if (flag17)
			{
				this.alertRightImageAnimator.SetSpriteSheet(newsItem.animationSpriteSheetPath, 0.1f);
				if (this.alertRightImageAnimator.SpriteCount() == 0)
				{
					flag16 = true;
					this.alertRightAnimatedImage.gameObject.SetActive(false);
					this.alertRightImage.gameObject.SetActive(true);
					newsItem.animationSpriteSheetPath = "";
				}
			}
			if (flag16)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(newsItem.popupResource2, this.alertRightImage);
			}
			if (newsItem.controlPointsRelevant && newsItem.newControlPoints != null)
			{
				this.controlPointPanel.SetActive(true);
				this.newControlPointPanel.SetActive(true);
				for (int i = 0; i < 6; i++)
				{
					if (i < newsItem.newControlPoints.Count)
					{
						if (newsItem.newControlPoints[i].isFactionState)
						{
							this.newControlPointImages[i].gameObject.SetActive(true);
							this.newControlPointImages[i].enabled = true;
							this.newControlPointImages[i].sprite = newsItem.newControlPoints[i].ref_faction.factionIcon64;
							this.newControlPointImages[i].color = new Color(1f, 1f, 1f, 1f);
						}
						else if (newsItem.newControlPoints[i].isNationState)
						{
							this.newControlPointImages[i].gameObject.SetActive(true);
							this.newControlPointImages[i].enabled = true;
							this.newControlPointImages[i].color = newsItem.newControlPoints[i].ref_nation.template.UIColor;
							this.newControlPointImages[i].sprite = null;
						}
					}
					else
					{
						this.newControlPointImages[i].gameObject.SetActive(false);
						this.newControlPointImages[i].enabled = false;
					}
				}
				if (newsItem.oldControlPoints != null && newsItem.oldControlPoints != newsItem.newControlPoints)
				{
					this.oldControlPointPanel.SetActive(true);
					for (int j = 0; j < 6; j++)
					{
						if (j < newsItem.oldControlPoints.Count)
						{
							if (newsItem.oldControlPoints[j].isFactionState)
							{
								this.oldControlPointImages[j].gameObject.SetActive(true);
								this.oldControlPointImages[j].sprite = newsItem.oldControlPoints[j].ref_faction.factionIcon64;
								this.oldControlPointImages[j].enabled = true;
								this.oldControlPointImages[j].color = new Color(1f, 1f, 1f, 1f);
							}
							else if (newsItem.oldControlPoints[j].isNationState)
							{
								this.oldControlPointImages[j].gameObject.SetActive(true);
								this.oldControlPointImages[j].enabled = true;
								this.oldControlPointImages[j].color = newsItem.oldControlPoints[j].ref_nation.template.UIColor;
								this.oldControlPointImages[j].sprite = null;
							}
						}
						else
						{
							this.oldControlPointImages[j].gameObject.SetActive(false);
							this.oldControlPointImages[j].enabled = false;
						}
					}
				}
				else
				{
					this.oldControlPointPanel.SetActive(false);
				}
			}
			else
			{
				this.controlPointPanel.SetActive(false);
			}
			int num = 700 - (flag10 ? 384 : 0) - (this.controlPointPanel.activeSelf ? 50 : 0) - ((newsItem.mission != null && !TIMissionPhaseState.InMissionPhase() && newsItem.mission.councilor.CanRepeatMission(newsItem.mission)) ? 50 : 0) - (newsItem.narrativeEventAlert ? 145 : 0);
			if (TIUtilities.GetScreenRatio() > 2.3f)
			{
				num = (int)((float)num * 0.92f);
			}
			if (TIPlayerProfileManager.uiScaleSetting > 0)
			{
				num = (int)((float)num * 0.9f);
			}
			this.alertBodyText.text = this.alertBodyText.text.TrimEnd(new char[] { '\r', '\n' });
			base.StartCoroutine(this.HandleNotificationTextScrollview(num));
		}

		// Token: 0x06005399 RID: 21401 RVA: 0x00258B21 File Offset: 0x00256D21
		private IEnumerator HandleNotificationTextScrollview(int maxTextHeight)
		{
			this.alertBodyText.gameObject.SetActive(true);
			yield return null;
			if (this.alertBodyText.GetPreferredValues().y > (float)maxTextHeight)
			{
				this.ToggleBodyScrollText(true, (float)maxTextHeight);
			}
			else
			{
				this.ToggleBodyScrollText(false, 0f);
			}
			this.alertPanelCanvas.enabled = true;
			yield break;
		}

		// Token: 0x0600539A RID: 21402 RVA: 0x00258B38 File Offset: 0x00256D38
		public void OnClickToggleNotificationOptionPanel()
		{
			this.notificationOptionsPanel.SetActive(!this.notificationOptionsPanel.activeSelf);
			AudioManager.PlayOneShot(this.notificationOptionsPanel.activeSelf ? "event:/SFX/UI_SFX/trig_SFX_OpenFinder" : "event:/SFX/UI_SFX/trig_SFX_CloseFinder", false, false);
			this.SetOpenNotificationButtonSprites();
		}

		// Token: 0x0600539B RID: 21403 RVA: 0x00258B84 File Offset: 0x00256D84
		public void ToggleNotificationOptionPanel(bool show)
		{
			this.notificationOptionsPanel.SetActive(show);
			this.SetOpenNotificationButtonSprites();
		}

		// Token: 0x0600539C RID: 21404 RVA: 0x00258B98 File Offset: 0x00256D98
		private void SetOpenNotificationButtonSprites()
		{
			if (this.notificationOptionsPanel.activeSelf)
			{
				this.openNotificationOptionPanelButton.image.sprite = AssetCacheManager.minusButtonIcon;
				this.altOpenNotificationOptionPanelButton.image.sprite = AssetCacheManager.minusButtonIcon;
				SpriteState spriteState = this.openNotificationOptionPanelButton.spriteState;
				spriteState.highlightedSprite = AssetCacheManager.minusButtonHoverIcon;
				spriteState.pressedSprite = AssetCacheManager.minusButtonHoverIcon;
				spriteState.selectedSprite = AssetCacheManager.minusButtonHoverIcon;
				this.openNotificationOptionPanelButton.spriteState = spriteState;
				this.altOpenNotificationOptionPanelButton.spriteState = spriteState;
				return;
			}
			this.openNotificationOptionPanelButton.image.sprite = AssetCacheManager.plusButtonIcon;
			this.altOpenNotificationOptionPanelButton.image.sprite = AssetCacheManager.plusButtonIcon;
			SpriteState spriteState2 = this.openNotificationOptionPanelButton.spriteState;
			spriteState2.highlightedSprite = AssetCacheManager.plusButtonHoverIcon;
			spriteState2.pressedSprite = AssetCacheManager.plusButtonHoverIcon;
			spriteState2.selectedSprite = AssetCacheManager.plusButtonHoverIcon;
			this.openNotificationOptionPanelButton.spriteState = spriteState2;
			this.altOpenNotificationOptionPanelButton.spriteState = spriteState2;
		}

		// Token: 0x0600539D RID: 21405 RVA: 0x00258C98 File Offset: 0x00256E98
		public void ToggleBodyScrollText(bool show, float heightToSet = 0f)
		{
			if (show)
			{
				this.alertBodyScrollText.text = this.alertBodyText.text;
				this.alertBodyTextScrollObject.GetComponent<LayoutElement>().minHeight = heightToSet;
				this.alertBodyTextScrollObject.GetComponentsInChildren<LayoutElement>().ToList<LayoutElement>().ForEach(delegate(LayoutElement x)
				{
					x.minHeight = heightToSet;
				});
				this.alertBodyTextScrollObject.SetActive(true);
				base.StartCoroutine(this.ResetScrollbarPosition());
				this.alertBodyText.gameObject.SetActive(false);
				return;
			}
			this.alertBodyTextScrollObject.SetActive(false);
			this.alertBodyText.gameObject.SetActive(true);
		}

		// Token: 0x0600539E RID: 21406 RVA: 0x00258D4A File Offset: 0x00256F4A
		public IEnumerator ResetScrollbarPosition()
		{
			yield return null;
			this.alertBodyTextScrollRect.verticalScrollbar.value = 1f;
			yield break;
		}

		// Token: 0x0600539F RID: 21407 RVA: 0x00258D59 File Offset: 0x00256F59
		private IEnumerator UpdateRenderTexture()
		{
			yield return null;
			if (this.notificationCameraInstance != null)
			{
				this.cameraRenderTextureImage.texture = this.notificationCameraInstance.GetComponent<Camera>().targetTexture;
			}
			yield break;
		}

		// Token: 0x060053A0 RID: 21408 RVA: 0x00258D68 File Offset: 0x00256F68
		public void UpdateCameraImage(TISpaceObjectState spaceObjectState)
		{
			if (this.notificationCameraInstance == null)
			{
				this.notificationCameraInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.notificationCamera);
				this.previewPosition = this.notificationCameraInstance.transform.Find("NotificationPanelPreviewPosition").gameObject;
				this.originalPreviewPosition = this.previewPosition.transform.localPosition;
				this.originalPreviewRotation = this.previewPosition.transform.localRotation.eulerAngles;
			}
			base.StartCoroutine(this.UpdateRenderTexture());
			this.previewPosition.transform.localPosition = this.originalPreviewPosition;
			this.previewPosition.transform.localRotation = Quaternion.Euler(this.originalPreviewRotation);
			foreach (object obj in this.previewPosition.transform)
			{
				Transform transform = (Transform)obj;
				transform.parent = null;
				global::UnityEngine.Object.Destroy(transform.gameObject);
			}
			GameObject gameObject = null;
			float num = 0.5f;
			TIHabState tihabState = (spaceObjectState.isHabState ? spaceObjectState.ref_hab : null);
			if (spaceObjectState.isHabState && tihabState.IsBase)
			{
				spaceObjectState = spaceObjectState.ref_hab.habSite.parentBody;
			}
			TISpaceFleetState tispaceFleetState = (spaceObjectState.isSpaceFleetState ? spaceObjectState.ref_fleet : null);
			if (tispaceFleetState != null && tispaceFleetState.ships.Count == 0)
			{
				spaceObjectState = tispaceFleetState.barycenter;
			}
			if (spaceObjectState.isSpaceBodyState)
			{
				gameObject = GameControl.assetLoader.LoadAsset<GameObject>(spaceObjectState.modelResource);
				num = (float)(20.0 / (double)spaceObjectState.modelScale);
			}
			else
			{
				Transform transform2 = GameControl.solarSystem.FindObject(spaceObjectState.ID.ToString());
				GameObject gameObject2 = ((transform2 != null) ? transform2.gameObject : null);
				if (gameObject2 != null)
				{
					if (tispaceFleetState != null)
					{
						gameObject = gameObject2.transform.Find(string.Format("{0} Container", spaceObjectState.ID)).gameObject;
						num = (tispaceFleetState.landed ? 0.1f : (0.6f / ((float)Mathf.Max(tispaceFleetState.ships.Count, 1) / 2f)));
						if (tispaceFleetState.ships.Count == 1 && !tispaceFleetState.landed)
						{
							num = 0.7f;
						}
					}
					else
					{
						gameObject = gameObject2.transform.Find("Model").gameObject;
						if (tihabState != null && tihabState.IsStation)
						{
							num = 0.5f / ((float)tihabState.tier * 1.5f);
						}
					}
				}
			}
			if (gameObject != null)
			{
				if (!spaceObjectState.isSpaceFleetState)
				{
					this.modelInstance = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, this.previewPosition.transform);
					this.modelInstance.SetActive(true);
					this.modelInstance.transform.localPosition = Vector3.zero;
					this.modelInstance.transform.SetLayer(10, true);
					this.modelInstance.transform.localScale = Vector3.one * num;
				}
				else
				{
					this.modelInstance = new GameObject();
					this.modelInstance.transform.SetParent(this.previewPosition.transform);
					this.modelInstance.name = gameObject.name;
					this.modelInstance.transform.localPosition = Vector3.zero;
					this.modelInstance.transform.localScale = Vector3.one * num;
					for (int i = 0; i < gameObject.transform.childCount; i++)
					{
						if (i < 20)
						{
							global::UnityEngine.Object.Instantiate<Transform>(gameObject.transform.GetChild(i), this.modelInstance.transform).localRotation = Quaternion.identity;
						}
					}
					this.modelInstance.transform.SetLayer(10, true);
				}
				if (spaceObjectState.isSpaceFleetState)
				{
					for (int j = 0; j < this.modelInstance.transform.childCount; j++)
					{
						Transform transform3 = this.modelInstance.transform.GetChild(j).transform;
						if (transform3.childCount > 0)
						{
							ShipVisController visController = transform3.GetChild(0).GetComponent<ShipVisController>();
							if (visController != null)
							{
								visController.SetAsUIVisualization(tispaceFleetState.ships.SingleOrDefault<TISpaceShipState>((TISpaceShipState x) => x.ID.ToString() == visController.name), true);
							}
						}
					}
					if (tispaceFleetState.landed)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(tispaceFleetState.ref_habSite.template.backgroundPath, this.maskedIllustrationImage);
						this.modelInstance.transform.Rotate(Vector3.right, 270f);
						this.maskedIllustrationImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
						this.maskedIllustrationImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
						return;
					}
					this.modelInstance.transform.Rotate(Vector3.right, 170f);
					this.modelInstance.transform.Rotate(Vector3.forward, 180f);
					this.previewPosition.transform.localPosition = new Vector3(5f, 0f, 40f);
					if (tispaceFleetState.ships.Count == 1)
					{
						this.previewPosition.transform.localPosition = new Vector3(5f, 5f, Mathf.Max(tispaceFleetState.ships[0].hull.length_m / 2f, 80f));
					}
					this.previewPosition.transform.localRotation = Quaternion.Euler(-10f, 0f, 0f);
					float num2 = 150f;
					int num3 = 0;
					int num4 = 1;
					Vector3[] pipPosition = tispaceFleetState.pipPosition;
					for (int k = 0; k < this.modelInstance.transform.childCount; k++)
					{
						if (this.modelInstance.transform.GetChild(k).ActiveChildCount() > 0)
						{
							this.modelInstance.transform.GetChild(k).localPosition = new Vector3(num2 * pipPosition[num3].x * (float)num4, num2 * pipPosition[num3].y * (float)num4, num2 * pipPosition[num3].z * (float)num4);
							this.modelInstance.transform.GetChild(k).GetChild(0).transform.localPosition = Vector3.zero;
							this.modelInstance.transform.GetChild(k).GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
							num3++;
						}
						if (num3 == 5)
						{
							num3 = 1;
							num4++;
						}
					}
					return;
				}
				else
				{
					if (tihabState != null && tihabState.IsStation)
					{
						this.modelInstance.GetComponent<HabModelController>().Initialize(tihabState, false, null);
						this.modelInstance.transform.Rotate(Vector3.right, 155f);
						return;
					}
					if (spaceObjectState.isSpaceBodyState)
					{
						SpaceObjectDetailController.TurnOffNaughtyShaderForUI(this.modelInstance);
					}
				}
			}
		}

		// Token: 0x060053A1 RID: 21409 RVA: 0x002594C0 File Offset: 0x002576C0
		private void StopLeaderVO()
		{
			if (this.voEventInstance.isValid())
			{
				this.voEventInstance.Stop(STOP_MODE.IMMEDIATE);
				this.voEventInstance.Release();
			}
		}

		// Token: 0x060053A2 RID: 21410 RVA: 0x002594E8 File Offset: 0x002576E8
		public void CleanUp(NotificationQueueItem forceItem = null)
		{
			if (forceItem == null)
			{
				if (this.heldNewsItems.Count > 0)
				{
					Action onCloseNotification = this.heldNewsItems[0].OnCloseNotification;
					if (onCloseNotification != null)
					{
						onCloseNotification();
					}
					this.heldNewsItems.RemoveAt(0);
				}
			}
			else
			{
				Action onCloseNotification2 = forceItem.OnCloseNotification;
				if (onCloseNotification2 != null)
				{
					onCloseNotification2();
				}
				this.heldNewsItems.Remove(forceItem);
			}
			this.singleAlertBox.SetActive(false);
			if (this.alertRightImageAnimator.isPlaying)
			{
				this.alertRightImageAnimator.Stop();
			}
			if (this.notificationVideo.clip != null)
			{
				this.notificationVideo.Stop();
				this.notificationVideo.clip = null;
			}
			global::UnityEngine.Object.Destroy(this.modelInstance, 0f);
			if (this.notificationCameraInstance != null)
			{
				Camera component = this.notificationCameraInstance.GetComponent<Camera>();
				if (component.targetTexture != null)
				{
					RenderTexture targetTexture = component.targetTexture;
					component.targetTexture = null;
					targetTexture.Release();
				}
			}
			global::UnityEngine.Object.Destroy(this.notificationCameraInstance, 0f);
			this.notificationCameraInstance = null;
			this.StopLeaderVO();
			this.ToggleNotificationOptionPanel(false);
			if (TIGlobalValuesState.isTutorialActive && this.heldNewsItems.Count == 0)
			{
				UITutorialController.CanHoldTutorials = true;
				(base.canvasManager.StrategyHud as GeneralControlsController).mainHUDTutorialController.HoldTutorial(CampaignMilestone.UITutorial_GeneralControlsCanvas, false, true);
				if (!GameControl.control.activePlayer.MilestoneCompleted(CampaignMilestone.UITutorial_Intro))
				{
					(base.canvasManager.StrategyHud as GeneralControlsController).introTutorialNewController.ShowTutorialTips(CampaignMilestone.UITutorial_Intro, false, true);
				}
			}
			GameControl.eventManager.RemoveListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.UpdateCustomNotificationButtons), null);
			GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.UpdateCustomNotificationButtons), null);
			GameControl.eventManager.RemoveListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.UpdateCustomNotificationButtons), null);
			GameControl.eventManager.RemoveListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateCustomNotificationButtons), null);
			GameControl.eventManager.RemoveListener<HabDesignTemplateModified>(new EventManager.EventDelegate<HabDesignTemplateModified>(this.UpdateCustomNotificationButtons), null);
		}

		// Token: 0x060053A3 RID: 21411 RVA: 0x002596F4 File Offset: 0x002578F4
		public void CleanupTextures()
		{
			this.notificationCamera = null;
			if (this.notificationCameraInstance != null)
			{
				Camera component = this.notificationCameraInstance.GetComponent<Camera>();
				if (component.targetTexture != null)
				{
					RenderTexture targetTexture = component.targetTexture;
					component.targetTexture = null;
					targetTexture.Release();
				}
			}
		}

		// Token: 0x060053A4 RID: 21412 RVA: 0x00259744 File Offset: 0x00257944
		public void OkayButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.CleanUp(null);
			base.gameTime.Play();
		}

		// Token: 0x060053A5 RID: 21413 RVA: 0x00259764 File Offset: 0x00257964
		public void CloseButtonPressed()
		{
			NotificationQueueItem notificationQueueItem = this.heldNewsItems[0];
			if (notificationQueueItem.triggerEndGame)
			{
				StartMenuController.ForceCredits();
				GameControl.control.viewMgr.GotoView(ViewType.MainMenu);
				return;
			}
			if (notificationQueueItem.alertBlockFaction == base.activePlayer && TIPromptQueueState.anyActivePlayerBlockingPrompt)
			{
				this.GotoButtonPressed();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CleanUp(null);
		}

		// Token: 0x060053A6 RID: 21414 RVA: 0x002597D0 File Offset: 0x002579D0
		public void GotoButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OptionSelect", false, false);
			this.CleanUp(this.currentItem);
			if (this.currentItem.gotoGameState != null && !this.currentItem.gotoGameState.deleted)
			{
				TIUtilities.GotoGameState(this.currentItem.gotoGameState, true, true, true, true, false, -1f);
			}
			else
			{
				Log.Error("GotoButtonPressed passed null or deleted gameState: " + this.currentItem.itemHeadline, Array.Empty<object>());
			}
			if ((!this.currentItem.alertFactions.Contains(base.activePlayer) || !(this.currentItem.alertBlockFaction != base.activePlayer)) && this.currentItem.alertBlockFaction == base.activePlayer)
			{
				string alertBlockEventName = this.currentItem.alertBlockEventName;
				if (alertBlockEventName != null)
				{
					if (alertBlockEventName == "PromptSelectProject")
					{
						GameControl.eventManager.TriggerEvent(new ForceProjectSelectionUI(base.activePlayer), "ForceProjectSelectionUI", new object[] { base.activePlayer });
						return;
					}
					if (!(alertBlockEventName == "PromptSelectTech"))
					{
						return;
					}
					GameControl.eventManager.TriggerEvent(new ForceTechSelectionUI(base.activePlayer), "ForceTechSelectionUI", new object[] { base.activePlayer });
				}
			}
		}

		// Token: 0x060053A7 RID: 21415 RVA: 0x0025991D File Offset: 0x00257B1D
		public void MinimizeNotificationWindowPressed()
		{
			this.singleAlertBoxBody.SetActive(!this.singleAlertBoxBody.activeSelf);
			this.UpdateNotificationWindowMinimizeStatus();
		}

		// Token: 0x060053A8 RID: 21416 RVA: 0x0025993E File Offset: 0x00257B3E
		private void MaximizeNotificationWindow()
		{
			this.singleAlertBoxBody.SetActive(true);
			this.UpdateNotificationWindowMinimizeStatus();
		}

		// Token: 0x060053A9 RID: 21417 RVA: 0x00259954 File Offset: 0x00257B54
		public void MinimizePolicyWindowPressed()
		{
			this.selectPolicyBodyPanelObject.SetActive(!this.selectPolicyBodyPanelObject.activeSelf);
			if (this.selectPolicyBodyPanelObject.activeSelf)
			{
				TIUtilities.UpdateButtonSpritesPlusMinus(this.minimizePolicyBodyButton, false, false);
				this.notificationCanvas.sortingOrder = 14;
				return;
			}
			TIUtilities.UpdateButtonSpritesPlusMinus(this.minimizePolicyBodyButton, true, false);
			this.notificationCanvas.sortingOrder = 9;
		}

		// Token: 0x060053AA RID: 21418 RVA: 0x002599BC File Offset: 0x00257BBC
		private void UpdateNotificationWindowMinimizeStatus()
		{
			if (this.singleAlertBoxBody.activeSelf)
			{
				TIUtilities.UpdateButtonSpritesPlusMinus(this.minimizeSingleAlertBodyButton, false, true);
				this.notificationCanvas.sortingOrder = 14;
				return;
			}
			TIUtilities.UpdateButtonSpritesPlusMinus(this.minimizeSingleAlertBodyButton, true, true);
			this.notificationCanvas.sortingOrder = 9;
		}

		// Token: 0x060053AB RID: 21419 RVA: 0x00259A0B File Offset: 0x00257C0B
		private void UpdateCustomNotificationButtons(SpaceCombatInitiated e)
		{
			this.SetCustomNotificationButtons(this.currentItem);
		}

		// Token: 0x060053AC RID: 21420 RVA: 0x00259A19 File Offset: 0x00257C19
		private void UpdateCustomNotificationButtons(ShipsRemovedFromFleet e)
		{
			this.SetCustomNotificationButtons(this.currentItem);
		}

		// Token: 0x060053AD RID: 21421 RVA: 0x00259A27 File Offset: 0x00257C27
		private void UpdateCustomNotificationButtons(HabDestroyed e)
		{
			this.SetCustomNotificationButtons(this.currentItem);
		}

		// Token: 0x060053AE RID: 21422 RVA: 0x00259A35 File Offset: 0x00257C35
		private void UpdateCustomNotificationButtons(CouncilCompositionChanged e)
		{
			this.SetCustomNotificationButtons(this.currentItem);
		}

		// Token: 0x060053AF RID: 21423 RVA: 0x00259A43 File Offset: 0x00257C43
		private void UpdateCustomNotificationButtons(HabDesignTemplateModified e)
		{
			this.SetCustomNotificationButtons(this.currentItem);
		}

		// Token: 0x060053B0 RID: 21424 RVA: 0x00259A54 File Offset: 0x00257C54
		private void SetCustomNotificationButtons(NotificationQueueItem item)
		{
			NotificationScreenController.<>c__DisplayClass220_0 CS$<>8__locals1 = new NotificationScreenController.<>c__DisplayClass220_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.item = item;
			if (CS$<>8__locals1.item.notificationDelegates.Count > 0)
			{
				bool[] array = new bool[6];
				bool[] array2 = new bool[6];
				int[] array3 = new int[6];
				string[] array4 = new string[6];
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				List<TISpaceFleetState> list = null;
				List<TIHabState> list2 = null;
				List<TISpaceFleetState> list3 = null;
				List<TISpaceFleetState> list4 = null;
				List<TISpaceFleetState> list5 = null;
				List<TISpaceFleetState> list6 = null;
				List<TIGameState> list7 = null;
				List<TIGameState> list8 = null;
				bool flag = false;
				foreach (SpecialNotificationDelegate specialNotificationDelegate in CS$<>8__locals1.item.notificationDelegates)
				{
					this.customDelegateTooltip[num].enabled = false;
					switch (specialNotificationDelegate)
					{
					case SpecialNotificationDelegate.RepeatMission:
						if (CS$<>8__locals1.item.mission != null && CS$<>8__locals1.item.mission.councilor.faction == base.activePlayer && CS$<>8__locals1.item.mission.councilor.CanRepeatMission(CS$<>8__locals1.item.mission))
						{
							array[num] = true;
							if (CS$<>8__locals1.item.mission.resources > 0f)
							{
								array4[num] = Loc.T("UI.Notifications.RepeatMissionButtonText_Spend", new object[]
								{
									TIUtilities.InlineResourceStr(CS$<>8__locals1.item.mission.missionTemplate.cost.resourceType),
									CS$<>8__locals1.item.mission.resources
								});
							}
							else
							{
								array4[num] = Loc.T("UI.Notifications.RepeatMissionButtonText");
							}
							array3[num] = 1;
							GameControl.eventManager.AddListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateCustomNotificationButtons), null, null, false, false);
							NotificationScreenController.CustomButtonPressed[] array5 = this.customButtonAction;
							int num7 = num;
							NotificationScreenController.CustomButtonPressed customButtonPressed;
							if ((customButtonPressed = CS$<>8__locals1.<>9__2) == null)
							{
								customButtonPressed = (CS$<>8__locals1.<>9__2 = delegate
								{
									GameControl.eventManager.RemoveListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
									CS$<>8__locals1.item.mission.councilor.faction.playerControl.StartAction(new SetCouncilorRepeatMission(CS$<>8__locals1.item.mission.councilor, true));
									CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
								});
							}
							array5[num7] = customButtonPressed;
						}
						break;
					case SpecialNotificationDelegate.RepeatMissionContinue:
						if (CS$<>8__locals1.item.mission != null && CS$<>8__locals1.item.mission.councilor.faction == base.activePlayer && CS$<>8__locals1.item.mission.councilor.CanRepeatMission(CS$<>8__locals1.item.mission))
						{
							array[num] = true;
							if (CS$<>8__locals1.item.mission.resources > 0f)
							{
								array4[num] = Loc.T("UI.Notifications.RepeatContinueMissionButtonText_Spend", new object[]
								{
									TIUtilities.InlineResourceStr(CS$<>8__locals1.item.mission.missionTemplate.cost.resourceType),
									CS$<>8__locals1.item.mission.resources
								});
							}
							else
							{
								array4[num] = Loc.T("UI.Notifications.RepeatContinueMissionButtonText");
							}
							array3[num] = 2;
							GameControl.eventManager.AddListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateCustomNotificationButtons), null, null, false, false);
							NotificationScreenController.CustomButtonPressed[] array6 = this.customButtonAction;
							int num8 = num;
							NotificationScreenController.CustomButtonPressed customButtonPressed2;
							if ((customButtonPressed2 = CS$<>8__locals1.<>9__3) == null)
							{
								customButtonPressed2 = (CS$<>8__locals1.<>9__3 = delegate
								{
									GameControl.eventManager.RemoveListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
									CS$<>8__locals1.item.mission.councilor.faction.playerControl.StartAction(new SetCouncilorRepeatMission(CS$<>8__locals1.item.mission.councilor, true));
									CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
									CS$<>8__locals1.<>4__this.CleanUp(null);
									CS$<>8__locals1.<>4__this.gameTime.Play();
								});
							}
							array6[num8] = customButtonPressed2;
						}
						break;
					case SpecialNotificationDelegate.PermanentAssignment:
						if (CS$<>8__locals1.item.mission != null && CS$<>8__locals1.item.mission.councilor.faction == base.activePlayer && CS$<>8__locals1.item.mission.councilor.CanRepeatMission(CS$<>8__locals1.item.mission))
						{
							array[num] = true;
							array4[num] = Loc.T("UI.Notifications.PermanentAssignment");
							array3[num] = 1;
							GameControl.eventManager.AddListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateCustomNotificationButtons), null, null, false, false);
							NotificationScreenController.CustomButtonPressed[] array7 = this.customButtonAction;
							int num9 = num;
							NotificationScreenController.CustomButtonPressed customButtonPressed3;
							if ((customButtonPressed3 = CS$<>8__locals1.<>9__4) == null)
							{
								customButtonPressed3 = (CS$<>8__locals1.<>9__4 = delegate
								{
									GameControl.eventManager.RemoveListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
									CS$<>8__locals1.item.mission.councilor.faction.playerControl.StartAction(new SetCouncilorRepeatMission(CS$<>8__locals1.item.mission.councilor, true));
									CS$<>8__locals1.item.mission.councilor.faction.playerControl.StartAction(new SetCouncilorPermanentAssignment(CS$<>8__locals1.<>4__this.currentItem.mission.councilor, true));
									CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
								});
							}
							array7[num9] = customButtonPressed3;
						}
						break;
					case SpecialNotificationDelegate.RepeatOperation:
					{
						IOperation operation = this.currentItem.operation.operationData.operation;
						if (TIGameState.Valid(this.currentItem.operation.actor) && this.currentItem.operation.actor.ref_faction == base.activePlayer && operation.Repeatable() && TIGameState.Valid(this.currentItem.operation.operationData.target) && operation.ActorCanPerformOperation(this.currentItem.operation.actor, this.currentItem.operation.operationData.target) && operation.GetPossibleTargets(this.currentItem.operation.actor, null).Contains(this.currentItem.operation.operationData.target))
						{
							array[num] = true;
							array4[num] = Loc.T("UI.Notifications.RepeatOpButtonText");
							array3[num] = 1;
							GameControl.eventManager.AddListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.UpdateCustomNotificationButtons), null, null, false, false);
							GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.UpdateCustomNotificationButtons), null, null, false, false);
							NotificationScreenController.CustomButtonPressed[] array8 = this.customButtonAction;
							int num10 = num;
							NotificationScreenController.CustomButtonPressed customButtonPressed4;
							if ((customButtonPressed4 = CS$<>8__locals1.<>9__75) == null)
							{
								customButtonPressed4 = (CS$<>8__locals1.<>9__75 = delegate
								{
									GameControl.eventManager.RemoveListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
									GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
									CS$<>8__locals1.item.operation.actor.ref_faction.playerControl.StartAction(new ConfirmOperationAction(CS$<>8__locals1.item.operation.actor, CS$<>8__locals1.<>4__this.currentItem.operation.operationData.target, CS$<>8__locals1.<>4__this.currentItem.operation.operationData.operation, null, null));
									CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
								});
							}
							array8[num10] = customButtonPressed4;
						}
						break;
					}
					case SpecialNotificationDelegate.RepeatOperationContinue:
					{
						IOperation operation2 = this.currentItem.operation.operationData.operation;
						if (TIGameState.Valid(this.currentItem.operation.actor) && this.currentItem.operation.actor.ref_faction == base.activePlayer && operation2.Repeatable() && TIGameState.Valid(this.currentItem.operation.operationData.target) && operation2.ActorCanPerformOperation(this.currentItem.operation.actor, this.currentItem.operation.operationData.target) && operation2.GetPossibleTargets(this.currentItem.operation.actor, null).Contains(this.currentItem.operation.operationData.target))
						{
							array[num] = true;
							array4[num] = Loc.T("UI.Notifications.RepeatContinueOpButtonText");
							array3[num] = 2;
							NotificationScreenController.CustomButtonPressed[] array9 = this.customButtonAction;
							int num11 = num;
							NotificationScreenController.CustomButtonPressed customButtonPressed5;
							if ((customButtonPressed5 = CS$<>8__locals1.<>9__76) == null)
							{
								customButtonPressed5 = (CS$<>8__locals1.<>9__76 = delegate
								{
									CS$<>8__locals1.item.operation.actor.ref_faction.playerControl.StartAction(new ConfirmOperationAction(CS$<>8__locals1.item.operation.actor, CS$<>8__locals1.<>4__this.currentItem.operation.operationData.target, CS$<>8__locals1.<>4__this.currentItem.operation.operationData.operation, null, null));
									CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
									CS$<>8__locals1.<>4__this.CleanUp(null);
									CS$<>8__locals1.<>4__this.gameTime.Play();
								});
							}
							array9[num11] = customButtonPressed5;
						}
						break;
					}
					case SpecialNotificationDelegate.OnFleetArrival:
					{
						TIGameState gotoGameState = CS$<>8__locals1.item.gotoGameState;
						if (gotoGameState != null && gotoGameState.isSpaceFleetState)
						{
							TISpaceFleetState arrivingFleet = CS$<>8__locals1.item.gotoGameState.ref_fleet;
							TIOrbitState ref_orbit = CS$<>8__locals1.item.gotoGameState.ref_orbit;
							if (num2 == 0 && !arrivingFleet.dockedAtHab && ref_orbit != null && arrivingFleet.faction == base.activePlayer)
							{
								TransferOperation transferOperation = new TransferOperation();
								Func<TISpaceFleetState, bool> <>9__41;
								List<TIHabState> list9 = ref_orbit.stationsInOrbit.Where<TIHabState>(delegate(TIHabState x)
								{
									if (x.AllowsResupply(arrivingFleet.faction, false, false))
									{
										IEnumerable<TISpaceFleetState> dockedFleets = x.dockedFleets;
										Func<TISpaceFleetState, bool> func5;
										if ((func5 = <>9__41) == null)
										{
											func5 = (<>9__41 = (TISpaceFleetState x) => x.faction.permanentAlly(arrivingFleet.faction));
										}
										return dockedFleets.All<TISpaceFleetState>(func5);
									}
									return false;
								}).ToList<TIHabState>();
								if (transferOperation.ActorCanPerformOperation(arrivingFleet, null) && list9.Count > 0)
								{
									TIHabState bestHab;
									if (arrivingFleet.homeport != null && list9.Contains(arrivingFleet.homeport))
									{
										bestHab = arrivingFleet.homeport;
									}
									else
									{
										bestHab = list9.Where<TIHabState>((TIHabState x) => x.AllowsShipConstruction(arrivingFleet.faction, false, false)).MinBy<TIHabState, int>((TIHabState x) => x.dockedFleets.Count);
									}
									if (bestHab == null)
									{
										bestHab = list9.MinBy<TIHabState, int>((TIHabState x) => x.dockedFleets.Count);
									}
									if (bestHab != null && transferOperation.ValidTransferDestinationForFleet(arrivingFleet, bestHab) && arrivingFleet.AnyValidTrajectory(bestHab))
									{
										array[num] = true;
										array3[num] = 0;
										array4[num] = Loc.T("UI.Notifications.LaunchTo", new object[] { bestHab.displayName });
										this.customButtonAction[num] = delegate
										{
											CS$<>8__locals1.<>4__this.CleanUp(null);
											GameControl.eventManager.TriggerEvent(new ForceTrajectorySelectionUI_NoCurrentTrajectory(arrivingFleet, bestHab, null), null, Array.Empty<object>());
										};
										num2++;
										break;
									}
								}
								else if (ref_orbit.interfaceOrbit && ref_orbit.ref_spaceBody != null)
								{
									Func<TISpaceFleetState, bool> <>9__47;
									List<TIHabState> list10 = ref_orbit.ref_spaceBody.surfaceBases.Where<TIHabState>(delegate(TIHabState x)
									{
										if (!x.underBombardment && x.AllowsResupply(arrivingFleet.faction, false, false))
										{
											IEnumerable<TISpaceFleetState> dockedFleets2 = x.dockedFleets;
											Func<TISpaceFleetState, bool> func6;
											if ((func6 = <>9__47) == null)
											{
												func6 = (<>9__47 = (TISpaceFleetState x) => x.faction.permanentAlly(arrivingFleet.faction));
											}
											return dockedFleets2.All<TISpaceFleetState>(func6);
										}
										return false;
									}).ToList<TIHabState>();
									if (list10.Count > 0)
									{
										TIHabState bestBase = list9.Where<TIHabState>((TIHabState x) => x.AllowsShipConstruction(arrivingFleet.faction, false, false)).MinBy<TIHabState, int>((TIHabState x) => x.dockedFleets.Count);
										if (bestBase == null)
										{
											bestBase = list10.MinBy<TIHabState, int>((TIHabState x) => x.dockedFleets.Count);
										}
										LandOnSurfaceOperation landOp = new LandOnSurfaceOperation();
										if (bestBase != null && landOp.ActorCanPerformOperation(arrivingFleet, bestBase))
										{
											array[num] = true;
											array3[num] = 0;
											array4[num] = Loc.T("UI.Notifications.LandAt", new object[] { bestBase.displayName });
											this.customButtonAction[num] = delegate
											{
												CS$<>8__locals1.<>4__this.CleanUp(null);
												GameControl.eventManager.TriggerEvent(new ForceFleetOperation(arrivingFleet, bestBase.habSite, landOp), null, Array.Empty<object>());
												CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
											};
											num2++;
											break;
										}
									}
								}
							}
							if (num4 == 0 && arrivingFleet.dockedAtHab && arrivingFleet.faction == base.activePlayer && arrivingFleet.CurrentOperations().Count == 0 && arrivingFleet.ref_hab.AllowsResupply(arrivingFleet.faction, false, false))
							{
								if (arrivingFleet.NeedsRepair())
								{
									if (arrivingFleet.NeedsRearm() || arrivingFleet.NeedsRefuel())
									{
										ResupplyAndRepairOperation repairOp2 = new ResupplyAndRepairOperation();
										if (repairOp2.ActorCanPerformOperation(arrivingFleet, arrivingFleet.dockedLocation))
										{
											array[num] = true;
											array3[num] = 0;
											array4[num] = Loc.T("UI.Notifications.RepairAt", new object[] { arrivingFleet.dockedLocation.displayName });
											GameControl.eventManager.AddListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.UpdateCustomNotificationButtons), null, null, false, false);
											GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.UpdateCustomNotificationButtons), null, null, false, false);
											this.customButtonAction[num] = delegate
											{
												if (TIGameState.Valid(arrivingFleet))
												{
													GameControl.eventManager.RemoveListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
													GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
													GameControl.eventManager.TriggerEvent(new ForceFleetOperation(arrivingFleet, arrivingFleet.dockedLocation, repairOp2), null, Array.Empty<object>());
												}
												CS$<>8__locals1.<>4__this.CleanUp(null);
											};
											num4++;
											break;
										}
									}
									else
									{
										RepairFleetOperation repairOp = new RepairFleetOperation();
										if (repairOp.ActorCanPerformOperation(arrivingFleet, arrivingFleet.dockedLocation))
										{
											array[num] = true;
											array3[num] = 2;
											array4[num] = Loc.T("UI.Notifications.RepairAt", new object[] { arrivingFleet.dockedLocation.displayName });
											GameControl.eventManager.AddListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.UpdateCustomNotificationButtons), null, null, false, false);
											GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.UpdateCustomNotificationButtons), null, null, false, false);
											this.customButtonAction[num] = delegate
											{
												if (TIGameState.Valid(arrivingFleet))
												{
													GameControl.eventManager.RemoveListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
													GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
													GameControl.eventManager.TriggerEvent(new ForceFleetOperation(arrivingFleet, arrivingFleet.dockedLocation, repairOp), null, Array.Empty<object>());
												}
												CS$<>8__locals1.<>4__this.CleanUp(null);
											};
											num4++;
											break;
										}
									}
								}
								else if (arrivingFleet.NeedsRearm() || arrivingFleet.NeedsRefuel())
								{
									ResupplyOperation resupplyOp = new ResupplyOperation();
									if (resupplyOp.ActorCanPerformOperation(arrivingFleet, arrivingFleet.dockedLocation))
									{
										array[num] = true;
										array3[num] = 2;
										array4[num] = Loc.T("UI.Notifications.ResupplyAt", new object[] { arrivingFleet.dockedLocation.displayName });
										GameControl.eventManager.AddListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.UpdateCustomNotificationButtons), null, null, false, false);
										GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.UpdateCustomNotificationButtons), null, null, false, false);
										this.customButtonAction[num] = delegate
										{
											if (TIGameState.Valid(arrivingFleet))
											{
												GameControl.eventManager.RemoveListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
												GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
												GameControl.eventManager.TriggerEvent(new ForceFleetOperation(arrivingFleet, arrivingFleet.dockedLocation, resupplyOp), null, Array.Empty<object>());
											}
											CS$<>8__locals1.<>4__this.CleanUp(null);
										};
										num4++;
										break;
									}
								}
							}
							if (ref_orbit != null)
							{
								List<TISpaceFleetState> list11;
								if (list4 == null)
								{
									IEnumerable<TISpaceFleetState> fleetsInOrbit = ref_orbit.fleetsInOrbit;
									Func<TISpaceFleetState, bool> func;
									if ((func = CS$<>8__locals1.<>9__55) == null)
									{
										func = (CS$<>8__locals1.<>9__55 = (TISpaceFleetState x) => x.faction == CS$<>8__locals1.<>4__this.activePlayer);
									}
									list11 = fleetsInOrbit.Where<TISpaceFleetState>(func).ToList<TISpaceFleetState>();
									List<TISpaceFleetState> list12 = list11;
									IEnumerable<TIHabState> stationsInOrbit = ref_orbit.stationsInOrbit;
									Func<TIHabState, IEnumerable<TISpaceFleetState>> func2;
									if ((func2 = CS$<>8__locals1.<>9__56) == null)
									{
										func2 = (CS$<>8__locals1.<>9__56 = delegate(TIHabState x)
										{
											IEnumerable<TISpaceFleetState> dockedFleets3 = x.dockedFleets;
											Func<TISpaceFleetState, bool> func7;
											if ((func7 = CS$<>8__locals1.<>9__59) == null)
											{
												func7 = (CS$<>8__locals1.<>9__59 = (TISpaceFleetState x) => x.faction == CS$<>8__locals1.<>4__this.activePlayer);
											}
											return dockedFleets3.Where<TISpaceFleetState>(func7);
										});
									}
									list12.AddRange(stationsInOrbit.SelectMany<TIHabState, TISpaceFleetState>(func2));
									if (list11.Count == 0)
									{
										IEnumerable<TISpaceFleetState> fleetsInOrbit2 = ref_orbit.ref_naturalSpaceObject.fleetsInOrbit;
										Func<TISpaceFleetState, bool> func3;
										if ((func3 = CS$<>8__locals1.<>9__57) == null)
										{
											func3 = (CS$<>8__locals1.<>9__57 = (TISpaceFleetState x) => x.faction == CS$<>8__locals1.<>4__this.activePlayer);
										}
										list11 = fleetsInOrbit2.Where<TISpaceFleetState>(func3).ToList<TISpaceFleetState>();
										List<TISpaceFleetState> list13 = list11;
										IEnumerable<TIHabState> stationsInOrbit2 = ref_orbit.ref_naturalSpaceObject.stationsInOrbit;
										Func<TIHabState, IEnumerable<TISpaceFleetState>> func4;
										if ((func4 = CS$<>8__locals1.<>9__58) == null)
										{
											func4 = (CS$<>8__locals1.<>9__58 = delegate(TIHabState x)
											{
												IEnumerable<TISpaceFleetState> dockedFleets4 = x.dockedFleets;
												Func<TISpaceFleetState, bool> func8;
												if ((func8 = CS$<>8__locals1.<>9__60) == null)
												{
													func8 = (CS$<>8__locals1.<>9__60 = (TISpaceFleetState x) => x.faction == CS$<>8__locals1.<>4__this.activePlayer);
												}
												return dockedFleets4.Where<TISpaceFleetState>(func8);
											});
										}
										list13.AddRange(stationsInOrbit2.SelectMany<TIHabState, TISpaceFleetState>(func4));
									}
									list11 = list11.Distinct<TISpaceFleetState>().ToList<TISpaceFleetState>();
									list11.Remove(arrivingFleet);
									list4 = new List<TISpaceFleetState>(list11);
								}
								else
								{
									list11 = list4;
								}
								if (list11.Count > 0)
								{
									TransferOperation transferOp2 = new TransferOperation();
									if (arrivingFleet.ref_faction == base.activePlayer)
									{
										MergeFleetOperation mergeOp2 = new MergeFleetOperation();
										if (list8 == null)
										{
											list8 = mergeOp2.GetPossibleTargets(arrivingFleet, null);
										}
										List<TIGameState> mergeOpTargets2 = list8;
										List<TISpaceFleetState> list14 = (from x in list11
											where mergeOp2.ActorCanPerformOperation(arrivingFleet, x) && mergeOpTargets2.Contains(x)
											orderby x.SpaceCombatValue() descending
											select x).ToList<TISpaceFleetState>();
										if (list14.Count > num3)
										{
											array[num] = true;
											array3[num] = 1;
											TISpaceFleetState targetFleet2 = list14[num3];
											array4[num] = Loc.T("UI.Notifications.JoinFleet", new object[] { targetFleet2.GetDisplayName(arrivingFleet.faction) });
											this.customDelegateTooltip[num].SetDelegate("BodyText", () => targetFleet2.FleetQuickDescription(CS$<>8__locals1.<>4__this.activePlayer));
											this.customDelegateTooltip[num].enabled = true;
											GameControl.eventManager.AddListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.UpdateCustomNotificationButtons), null, null, false, false);
											GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.UpdateCustomNotificationButtons), null, null, false, false);
											this.customButtonAction[num] = delegate
											{
												if (targetFleet2.exists && arrivingFleet.exists)
												{
													GameControl.eventManager.RemoveListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
													GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
													CS$<>8__locals1.<>4__this.activePlayer.playerControl.StartAction(new ConfirmOperationAction(targetFleet2, arrivingFleet, new MergeFleetOperation(), null, null));
												}
												CS$<>8__locals1.<>4__this.CleanUp(null);
											};
											num3++;
											break;
										}
										List<TISpaceFleetState> list15 = new List<TISpaceFleetState>();
										if (list5 == null)
										{
											list15 = (from x in list11
												where x.faction == arrivingFleet.faction && transferOp2.ValidTransferDestinationForFleet(arrivingFleet, x)
												orderby x.SpaceCombatValue() descending
												select x).ToList<TISpaceFleetState>();
											list15 = list15.Where<TISpaceFleetState>((TISpaceFleetState x) => arrivingFleet.AnyValidTrajectory(x)).ToList<TISpaceFleetState>();
											list5 = new List<TISpaceFleetState>(list15);
										}
										else
										{
											list15 = list5;
										}
										if (transferOp2.ActorCanPerformOperation(arrivingFleet, null) && list15.Count > num5)
										{
											TISpaceFleetState bestFriendly = list15[num5];
											array[num] = true;
											array3[num] = 0;
											array4[num] = Loc.T("UI.Notifications.Rally", new object[] { bestFriendly.GetDisplayName(base.activePlayer) });
											this.customDelegateTooltip[num].SetDelegate("BodyText", () => bestFriendly.FleetQuickDescription(CS$<>8__locals1.<>4__this.activePlayer));
											this.customDelegateTooltip[num].enabled = true;
											this.customButtonAction[num] = delegate
											{
												CS$<>8__locals1.<>4__this.CleanUp(null);
												GameControl.eventManager.TriggerEvent(new ForceTrajectorySelectionUI_NoCurrentTrajectory(arrivingFleet, bestFriendly, null), null, Array.Empty<object>());
											};
											num5++;
											break;
										}
									}
									List<TISpaceFleetState> list16;
									if (list6 == null)
									{
										list16 = (from x in list11
											where x.faction != arrivingFleet.faction && transferOp2.ActorCanPerformOperation(x, null) && transferOp2.ValidTransferDestinationForFleet(x, arrivingFleet)
											orderby x.SpaceCombatValue() descending
											select x).ToList<TISpaceFleetState>();
										list16 = list16.Where<TISpaceFleetState>((TISpaceFleetState x) => x.AnyValidTrajectory(arrivingFleet)).ToList<TISpaceFleetState>();
										list6 = new List<TISpaceFleetState>(list16);
									}
									else
									{
										list16 = list6;
									}
									if (list16.Count > num6)
									{
										array[num] = true;
										array3[num] = 0;
										TISpaceFleetState interceptor = list16[num6];
										this.customDelegateTooltip[num].SetDelegate("BodyText", () => interceptor.FleetQuickDescription(CS$<>8__locals1.<>4__this.activePlayer));
										this.customDelegateTooltip[num].enabled = true;
										array4[num] = Loc.T("UI.Notifications.Intercept", new object[] { interceptor.GetDisplayName(base.activePlayer) });
										this.customButtonAction[num] = delegate
										{
											CS$<>8__locals1.<>4__this.CleanUp(null);
											GameControl.eventManager.TriggerEvent(new ForceTrajectorySelectionUI_NoCurrentTrajectory(interceptor, arrivingFleet, null), null, Array.Empty<object>());
										};
										num6++;
									}
								}
							}
						}
						break;
					}
					case SpecialNotificationDelegate.OnEnemyFleetLaunches:
					{
						TIGameState gotoGameState2 = CS$<>8__locals1.item.gotoGameState;
						if (gotoGameState2 != null && gotoGameState2.isSpaceFleetState)
						{
							TISpaceFleetState fleet2 = CS$<>8__locals1.item.gotoGameState.ref_fleet;
							if (fleet2 != null)
							{
								array[num] = true;
								array3[num] = 2;
								array4[num] = Loc.T("UI.Notifications.SetAlarm");
								this.customButtonAction[num] = delegate
								{
									CS$<>8__locals1.<>4__this.CleanUp(null);
									SpaceObjectDetailController.CreateFleetAlarm(CS$<>8__locals1.<>4__this.activePlayer, fleet2);
									CS$<>8__locals1.<>4__this.gameTime.Play();
								};
							}
						}
						break;
					}
					case SpecialNotificationDelegate.FleetAvailable:
					{
						TIGameState gotoGameState3 = CS$<>8__locals1.item.gotoGameState;
						if (gotoGameState3 != null && gotoGameState3.isSpaceFleetState)
						{
							TISpaceFleetState availableFleet = CS$<>8__locals1.item.gotoGameState.ref_fleet;
							if (!availableFleet.dockedAtHab)
							{
								TransferOperation transferOp3 = new TransferOperation();
								if (transferOp3.ActorCanPerformOperation(availableFleet, null))
								{
									List<TIHabState> list17;
									if (list2 == null)
									{
										list17 = new List<TIHabState>();
										if (availableFleet.homeport != null)
										{
											list17.Add(availableFleet.homeport);
										}
										TIOrbitState ref_orbit2 = availableFleet.ref_orbit;
										if (ref_orbit2 != null)
										{
											TIFactionState faction = availableFleet.faction;
											list17.AddRangeUnique<TIHabState>((from x in ref_orbit2.stationsInOrbit
												where x.AllowsShipConstruction(faction, false, false)
												orderby x.dockedFleets.Count
												select x).ToList<TIHabState>());
											list17.AddRangeUnique<TIHabState>((from x in ref_orbit2.stationsInOrbit
												where x.AllowsResupply(faction, false, false)
												orderby x.dockedFleets.Count
												select x).ToList<TIHabState>());
											list17.AddRangeUnique<TIHabState>((from x in ref_orbit2.ref_naturalSpaceObject.stationsInOrbit
												where x.AllowsShipConstruction(faction, false, false)
												orderby x.dockedFleets.Count
												select x).ToList<TIHabState>());
											list17.AddRangeUnique<TIHabState>((from x in ref_orbit2.ref_naturalSpaceObject.stationsInOrbit
												where x.AllowsResupply(faction, false, false)
												orderby x.dockedFleets.Count
												select x).ToList<TIHabState>());
										}
										list17 = list17.Where<TIHabState>((TIHabState x) => transferOp3.ValidTransferDestinationForFleet(availableFleet, x)).ToList<TIHabState>();
										list17 = list17.Where<TIHabState>((TIHabState x) => availableFleet.AnyValidTrajectory(x)).ToList<TIHabState>();
										list2 = new List<TIHabState>(list17);
									}
									else
									{
										list17 = list2;
									}
									if (list17.Count > num)
									{
										array[num] = true;
										array3[num] = 0;
										TIHabState hab2 = list17[num];
										array4[num] = Loc.T("UI.Notifications.LaunchTo", new object[] { hab2.displayName });
										this.customButtonAction[num] = delegate
										{
											CS$<>8__locals1.<>4__this.CleanUp(null);
											GameControl.eventManager.TriggerEvent(new ForceTrajectorySelectionUI_NoCurrentTrajectory(availableFleet, hab2, null), null, Array.Empty<object>());
										};
									}
								}
							}
						}
						break;
					}
					case SpecialNotificationDelegate.OnFleetAlarm:
					{
						TIGameState gotoGameState4 = CS$<>8__locals1.item.gotoGameState;
						if (gotoGameState4 != null && gotoGameState4.isSpaceFleetState)
						{
							TISpaceFleetState alertedFleet = CS$<>8__locals1.item.gotoGameState.ref_fleet;
							Trajectory trajectory = alertedFleet.trajectory;
							TIOrbitState tiorbitState = ((trajectory != null) ? trajectory.destinationOrbit : null) ?? null;
							if (tiorbitState != null)
							{
								List<TISpaceFleetState> list18;
								if (list3 == null)
								{
									TransferOperation transferOp4 = new TransferOperation();
									list18 = (from x in tiorbitState.fleetsInOrbit
										where x.faction == CS$<>8__locals1.<>4__this.activePlayer && transferOp4.ActorCanPerformOperation(x, alertedFleet) && transferOp4.ValidTransferDestinationForFleet(x, alertedFleet)
										orderby x.SpaceCombatValue() descending
										select x).ToList<TISpaceFleetState>();
									if (list18.Count < 6)
									{
										list18.AddRangeUnique<TISpaceFleetState>((from x in tiorbitState.ref_naturalSpaceObject.fleetsInOrbit
											where x.faction == CS$<>8__locals1.<>4__this.activePlayer && transferOp4.ActorCanPerformOperation(x, alertedFleet)
											orderby x.SpaceCombatValue() descending
											select x).ToList<TISpaceFleetState>());
									}
									if (list18.Count < 6)
									{
										list18.AddRangeUnique<TISpaceFleetState>((from x in tiorbitState.ref_naturalSpaceObject.fleetsInSystem
											where x.faction == CS$<>8__locals1.<>4__this.activePlayer && transferOp4.ActorCanPerformOperation(x, alertedFleet)
											orderby x.SpaceCombatValue() descending
											select x).ToList<TISpaceFleetState>());
									}
									list18 = list18.Where<TISpaceFleetState>((TISpaceFleetState x) => x.AnyValidTrajectory(alertedFleet)).ToList<TISpaceFleetState>();
									list3 = new List<TISpaceFleetState>(list18);
								}
								else
								{
									list18 = list3;
								}
								if (list18.Count > num)
								{
									array[num] = true;
									array3[num] = 0;
									TISpaceFleetState fleet3 = list18[num];
									array4[num] = Loc.T("UI.Notifications.Intercept", new object[] { fleet3.GetDisplayName(base.activePlayer) });
									this.customDelegateTooltip[num].SetDelegate("BodyText", () => fleet3.FleetQuickDescription(CS$<>8__locals1.<>4__this.activePlayer));
									this.customDelegateTooltip[num].enabled = true;
									this.customButtonAction[num] = delegate
									{
										CS$<>8__locals1.<>4__this.CleanUp(null);
										GameControl.eventManager.TriggerEvent(new ForceTrajectorySelectionUI_NoCurrentTrajectory(fleet3, alertedFleet, null), null, Array.Empty<object>());
									};
								}
							}
						}
						break;
					}
					case SpecialNotificationDelegate.FavoriteUnlockedProject:
						if (TemplateManager.Find<TIProjectTemplate>(CS$<>8__locals1.item.customButtonTemplateName, false) != null)
						{
							array[num] = true;
							array4[num] = Loc.T("UI.Notifications.FavoriteUnlockedProject");
							array3[num] = 2;
							NotificationScreenController.CustomButtonPressed[] array10 = this.customButtonAction;
							int num12 = num;
							NotificationScreenController.CustomButtonPressed customButtonPressed6;
							if ((customButtonPressed6 = CS$<>8__locals1.<>9__5) == null)
							{
								customButtonPressed6 = (CS$<>8__locals1.<>9__5 = delegate
								{
									CS$<>8__locals1.<>4__this.activePlayer.playerControl.StartAction(new FavorProjectAction(CS$<>8__locals1.<>4__this.activePlayer, CS$<>8__locals1.item.customButtonTemplateName, true));
									GameControl.eventManager.TriggerEvent(new ProjectUIOptionsChanged(CS$<>8__locals1.<>4__this.activePlayer), null, new object[] { CS$<>8__locals1.<>4__this.activePlayer });
									CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
									CS$<>8__locals1.<>4__this.CleanUp(null);
									CS$<>8__locals1.<>4__this.gameTime.Play();
								});
							}
							array10[num12] = customButtonPressed6;
						}
						break;
					case SpecialNotificationDelegate.HideUnlockedProject:
						if (TemplateManager.Find<TIProjectTemplate>(CS$<>8__locals1.item.customButtonTemplateName, false) != null)
						{
							array[num] = true;
							array4[num] = Loc.T("UI.Notifications.HideUnlockedProject");
							array3[num] = 2;
							NotificationScreenController.CustomButtonPressed[] array11 = this.customButtonAction;
							int num13 = num;
							NotificationScreenController.CustomButtonPressed customButtonPressed7;
							if ((customButtonPressed7 = CS$<>8__locals1.<>9__6) == null)
							{
								customButtonPressed7 = (CS$<>8__locals1.<>9__6 = delegate
								{
									CS$<>8__locals1.<>4__this.activePlayer.playerControl.StartAction(new HideProjectAction(CS$<>8__locals1.<>4__this.activePlayer, CS$<>8__locals1.item.customButtonTemplateName, true));
									GameControl.eventManager.TriggerEvent(new ProjectUIOptionsChanged(CS$<>8__locals1.<>4__this.activePlayer), null, new object[] { CS$<>8__locals1.<>4__this.activePlayer });
									CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
									CS$<>8__locals1.<>4__this.CleanUp(null);
									CS$<>8__locals1.<>4__this.gameTime.Play();
								});
							}
							array11[num13] = customButtonPressed7;
						}
						break;
					case SpecialNotificationDelegate.RepeatProject:
					{
						TIProjectTemplate project = TemplateManager.Find<TIProjectTemplate>(CS$<>8__locals1.item.customButtonTemplateName, false);
						if (project != null && project.repeatable && base.activePlayer.availableProjects.Contains(project))
						{
							array[num] = true;
							array4[num] = Loc.T("UI.Notifications.RepeatProject");
							array3[num] = 2;
							this.customButtonAction[num] = delegate
							{
								int num14 = CS$<>8__locals1.<>4__this.activePlayer.GetSlotForProject(project);
								if (num14 == -1)
								{
									num14 = CS$<>8__locals1.<>4__this.activePlayer.BestAvailableEmptySlot();
								}
								CS$<>8__locals1.<>4__this.activePlayer.playerControl.StartAction(new SelectProjectForDevelopmentAction(CS$<>8__locals1.<>4__this.activePlayer, num14, project));
								GameControl.eventManager.TriggerEvent(new ProjectSelectedFromRemoteUI(CS$<>8__locals1.<>4__this.activePlayer, project), null, new object[] { CS$<>8__locals1.<>4__this.activePlayer });
								CS$<>8__locals1.<>4__this.CleanUp(null);
								CS$<>8__locals1.<>4__this.gameTime.Play();
							};
						}
						break;
					}
					case SpecialNotificationDelegate.JoinOrRallyToFleet:
						if (TIGameState.Valid(CS$<>8__locals1.item.gotoGameState) && (CS$<>8__locals1.item.gotoGameState.isSpaceFleetState || CS$<>8__locals1.item.gotoGameState.isSpaceShipState))
						{
							TISpaceFleetState fleet4 = CS$<>8__locals1.item.gotoGameState.ref_fleet;
							if (fleet4.ships.Count != 0)
							{
								TISpaceGameState dockedLocation = fleet4.dockedLocation;
								TIHabState tihabState = ((dockedLocation != null) ? dockedLocation.ref_hab : null);
								if (tihabState != null)
								{
									List<TISpaceFleetState> list19;
									if (list == null)
									{
										MergeFleetOperation mergeOp = new MergeFleetOperation();
										if (list7 == null)
										{
											list7 = mergeOp.GetPossibleTargets(fleet4, null);
										}
										List<TIGameState> mergeOpTargets = list7;
										list19 = (from x in tihabState.dockedFleets
											where x != fleet4 && x.faction == fleet4.faction && mergeOp.ActorCanPerformOperation(fleet4, x) && mergeOpTargets.Contains(x)
											orderby x.SpaceCombatValue() descending
											select x).ToList<TISpaceFleetState>();
										if (tihabState.IsStation)
										{
											TransferOperation transferOp = new TransferOperation();
											list19.AddRange((from x in tihabState.ref_orbit.fleetsInOrbit
												where x != fleet4 && x.faction == fleet4.faction && transferOp.ActorCanPerformOperation(fleet4, x) && transferOp.ValidTransferDestinationForFleet(fleet4, x) && fleet4.AnyValidTrajectory(x)
												orderby x.SpaceCombatValue() descending
												select x).ToList<TISpaceFleetState>());
										}
										list19 = list19.Distinct<TISpaceFleetState>().ToList<TISpaceFleetState>();
										list = new List<TISpaceFleetState>(list19);
									}
									else
									{
										list19 = list;
									}
									if (list19.Count > num)
									{
										array[num] = true;
										TISpaceFleetState targetFleet = list19[num];
										this.customDelegateTooltip[num].SetDelegate("BodyText", () => targetFleet.FleetQuickDescription(CS$<>8__locals1.<>4__this.activePlayer));
										this.customDelegateTooltip[num].enabled = true;
										if (tihabState.dockedFleets.Contains(list19[num]))
										{
											array3[num] = 1;
											array4[num] = Loc.T("UI.Notifications.JoinFleet", new object[] { list19[num].GetDisplayName(base.activePlayer) });
											GameControl.eventManager.AddListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.UpdateCustomNotificationButtons), null, null, false, false);
											GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.UpdateCustomNotificationButtons), null, null, false, false);
											this.customButtonAction[num] = delegate
											{
												GameControl.eventManager.RemoveListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
												GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
												using (IEnumerator<object> enumerator2 = CS$<>8__locals1.<>4__this.timerList.GetEnumerator())
												{
													while (enumerator2.MoveNext())
													{
														if (NotificationScreenController.<>o__220.<>p__0 == null)
														{
															NotificationScreenController.<>o__220.<>p__0 = CallSite<Func<CallSite, object, NewsFeedListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NewsFeedListItemController), typeof(NotificationScreenController)));
														}
														NewsFeedListItemController newsFeedListItemController = NotificationScreenController.<>o__220.<>p__0.Target(NotificationScreenController.<>o__220.<>p__0, enumerator2.Current);
														if (newsFeedListItemController.gameObject.activeInHierarchy && newsFeedListItemController.item.gotoGameState == fleet4)
														{
															newsFeedListItemController.item.gotoGameState = targetFleet;
														}
													}
												}
												CS$<>8__locals1.item.gotoGameState = targetFleet;
												CS$<>8__locals1.<>4__this.activePlayer.playerControl.StartAction(new ConfirmOperationAction(targetFleet, fleet4, new MergeFleetOperation(), null, null));
												CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
											};
										}
										else
										{
											array4[num] = Loc.T("UI.Notifications.LaunchTo", new object[] { targetFleet.GetDisplayName(base.activePlayer) });
											array3[num] = 0;
											this.customButtonAction[num] = delegate
											{
												CS$<>8__locals1.<>4__this.CleanUp(null);
												GameControl.eventManager.TriggerEvent(new ForceTrajectorySelectionUI_NoCurrentTrajectory(fleet4, targetFleet, null), null, Array.Empty<object>());
											};
										}
									}
								}
							}
						}
						break;
					case SpecialNotificationDelegate.OpenHabManager:
						if (CS$<>8__locals1.item.gotoGameState != null && (CS$<>8__locals1.item.gotoGameState.isHabState || CS$<>8__locals1.item.gotoGameState.isHabModuleState))
						{
							TIHabState hab3 = CS$<>8__locals1.item.gotoGameState.ref_hab;
							if (TIGameState.Valid(hab3))
							{
								array[num] = true;
								array3[num] = 1;
								if (hab3.ref_factions.Contains(base.activePlayer))
								{
									array4[num] = Loc.T("UI.Notifications.OpenHabManagerMine", new object[] { hab3.displayName });
								}
								else
								{
									array4[num] = Loc.T("UI.Notifications.OpenHabManager");
								}
								GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.UpdateCustomNotificationButtons), null, null, true, false);
								this.customButtonAction[num] = delegate
								{
									GameControl.eventManager.RemoveListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
									CS$<>8__locals1.<>4__this.CleanUp(null);
									GameControl.eventManager.TriggerEvent(new HabDetailRequested(hab3, TIGameState.Valid(hab3)), null, Array.Empty<object>());
								};
							}
						}
						break;
					case SpecialNotificationDelegate.LaunchAllProbes:
					{
						TIGenericTechTemplate tigenericTechTemplate = TemplateManager.Find<TIGenericTechTemplate>(CS$<>8__locals1.item.customButtonTemplateName, true);
						if (tigenericTechTemplate != null && tigenericTechTemplate.SpaceExplorationTech())
						{
							LaunchAllProbeOperation probeAllOperation = new LaunchAllProbeOperation();
							List<TIGameState> targets = probeAllOperation.GetPossibleTargets(base.activePlayer, null);
							if (targets.Count > 0)
							{
								TIResourcesCost cost = probeAllOperation.ResourceCostOptions(base.activePlayer, null, base.activePlayer, false)[0];
								if (cost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
								{
									array[num] = true;
									array3[num] = 0;
									if (targets.Count > 1)
									{
										string probeCostText = Loc.T("UI.Intel.ProbeAll_wCost", new object[]
										{
											targets.Count.ToString("N0"),
											cost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None)
										});
										array4[num] = probeCostText;
										this.customDelegateTooltip[num].SetDelegate("BodyText", () => probeCostText);
										this.customDelegateTooltip[num].enabled = true;
									}
									else
									{
										string probecostText = Loc.T("UI.Intel.ProbeAllSingle_wCost", new object[]
										{
											targets.Count.ToString("N0"),
											cost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None)
										});
										array4[num] = probecostText;
										this.customDelegateTooltip[num].SetDelegate("BodyText", () => probecostText);
										this.customDelegateTooltip[num].enabled = true;
									}
									this.customButtonAction[num] = delegate
									{
										probeAllOperation.OnOperationConfirm(CS$<>8__locals1.<>4__this.activePlayer, targets[0], cost, null);
										CS$<>8__locals1.<>4__this.DisableAllDelegatePanelObjects();
									};
								}
							}
						}
						break;
					}
					case SpecialNotificationDelegate.OpenNationPriorities:
						if (CS$<>8__locals1.item.gotoGameState != null && CS$<>8__locals1.item.gotoGameState.ref_nation != null)
						{
							TINationState nation = CS$<>8__locals1.item.gotoGameState.ref_nation;
							if (nation.FactionsWithControlPoint.Contains(base.activePlayer))
							{
								array[num] = true;
								array3[num] = 1;
								array4[num] = Loc.T("UI.Notifications.ManagePriorities");
								this.customButtonAction[num] = delegate
								{
									CS$<>8__locals1.<>4__this.CleanUp(null);
									GameControl.eventManager.TriggerEvent(new NationIPManagerRequested(nation, CS$<>8__locals1.item.gotoGameState.ref_region ?? nation.capital), null, Array.Empty<object>());
								};
							}
						}
						break;
					case SpecialNotificationDelegate.SetBodyTagToGreen:
					{
						NotificationScreenController.<>c__DisplayClass220_35 CS$<>8__locals31 = new NotificationScreenController.<>c__DisplayClass220_35();
						CS$<>8__locals31.CS$<>8__locals32 = CS$<>8__locals1;
						NotificationScreenController.<>c__DisplayClass220_35 CS$<>8__locals32 = CS$<>8__locals31;
						TIGameState gotoGameState5 = CS$<>8__locals31.CS$<>8__locals32.item.gotoGameState;
						CS$<>8__locals32.spaceBody = ((gotoGameState5 != null) ? gotoGameState5.ref_spaceBody : null);
						if (CS$<>8__locals31.spaceBody != null && CS$<>8__locals31.spaceBody.playerTag != PlayerTag.Green)
						{
							array[num] = true;
							array3[num] = 0;
							array4[num] = Loc.T("UI.Notifications.TagGreen");
							this.customButtonAction[num] = delegate
							{
								CS$<>8__locals31.CS$<>8__locals32.<>4__this.activePlayer.playerControl.StartAction(new SetPlanetTagAction(CS$<>8__locals31.spaceBody, PlayerTag.Green));
								CS$<>8__locals31.CS$<>8__locals32.<>4__this.DisableAllDelegatePanelObjects();
							};
						}
						break;
					}
					case SpecialNotificationDelegate.SetBodyTagToRed:
					{
						NotificationScreenController.<>c__DisplayClass220_34 CS$<>8__locals33 = new NotificationScreenController.<>c__DisplayClass220_34();
						CS$<>8__locals33.CS$<>8__locals31 = CS$<>8__locals1;
						NotificationScreenController.<>c__DisplayClass220_34 CS$<>8__locals34 = CS$<>8__locals33;
						TIGameState gotoGameState6 = CS$<>8__locals33.CS$<>8__locals31.item.gotoGameState;
						CS$<>8__locals34.spaceBody = ((gotoGameState6 != null) ? gotoGameState6.ref_spaceBody : null);
						if (CS$<>8__locals33.spaceBody != null && CS$<>8__locals33.spaceBody.playerTag != PlayerTag.Red)
						{
							array[num] = true;
							array3[num] = 0;
							array4[num] = Loc.T("UI.Notifications.TagRed");
							this.customButtonAction[num] = delegate
							{
								CS$<>8__locals33.CS$<>8__locals31.<>4__this.activePlayer.playerControl.StartAction(new SetPlanetTagAction(CS$<>8__locals33.spaceBody, PlayerTag.Red));
								CS$<>8__locals33.CS$<>8__locals31.<>4__this.DisableAllDelegatePanelObjects();
							};
						}
						break;
					}
					case SpecialNotificationDelegate.HabTemplateSelection:
						if (CS$<>8__locals1.item.gotoGameState != null && (CS$<>8__locals1.item.gotoGameState.isHabState || CS$<>8__locals1.item.gotoGameState.isHabModuleState))
						{
							TIHabState hab4 = CS$<>8__locals1.item.gotoGameState.ref_hab;
							if (TIGameState.Valid(hab4))
							{
								bool flag2 = this.PopulateDropdownWithHabTemplateSelection(hab4);
								if (hab4.ref_factions.Contains(base.activePlayer) && flag2)
								{
									GameControl.eventManager.AddListener<HabDesignTemplateModified>(new EventManager.EventDelegate<HabDesignTemplateModified>(this.UpdateCustomNotificationButtons), null, null, true, false);
									array[num] = true;
									array2[num] = true;
									array3[num] = 1;
									array4[num] = Loc.T("UI.Habs.ApplySelectedTemplate");
									this.customDropdownDelegateApplyButtonIndex = num;
									this.customDelegateTooltip[num].SetDelegate("BodyText", () => Loc.T("UI.Habs.SelectTemplate"));
									this.customDelegateTooltip[num].enabled = true;
									this.customButtonAction[num] = delegate
									{
										TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(CS$<>8__locals1.<>4__this.selectionDropdownDict[CS$<>8__locals1.<>4__this.customDelegateDropdown.value], false);
										TIResourcesCost tiresourcesCost;
										float num15;
										List<TIHabModuleTemplate> list20;
										hab4.ApplySavedTemplate(tihabTemplate, true, CS$<>8__locals1.<>4__this.customDelegateDropdownToggle.isOn, out tiresourcesCost, out num15, out list20);
										if (CS$<>8__locals1.<>4__this.CostWithNecessaryBoost(CS$<>8__locals1.<>4__this.activePlayer, tiresourcesCost, hab4).CanAfford(CS$<>8__locals1.<>4__this.activePlayer, 1f, null, float.PositiveInfinity))
										{
											CS$<>8__locals1.<>4__this.activePlayer.playerControl.StartAction(new ApplyHabTemplateAction(hab4, tihabTemplate, true));
											SoundEffectController.PlayBuildHabModuleSound(tihabTemplate.sectors[0].habModules[0], hab4);
										}
										else
										{
											AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
										}
										CS$<>8__locals1.<>4__this.templateTargetHab = null;
										CS$<>8__locals1.<>4__this.CleanUp(null);
										GameControl.eventManager.TriggerEvent(new HabDetailRequested(hab4, TIGameState.Valid(hab4)), null, Array.Empty<object>());
										GameControl.eventManager.RemoveListener<HabDesignTemplateModified>(new EventManager.EventDelegate<HabDesignTemplateModified>(CS$<>8__locals1.<>4__this.UpdateCustomNotificationButtons), null);
									};
									this.templateTargetHab = hab4;
									flag = true;
									this.customDelegateDropdownToggle.SetIsOnWithoutNotify(false);
									this.customDelegateDropdown.value = 0;
								}
							}
						}
						break;
					case SpecialNotificationDelegate.SelectFleetNoCamera:
					{
						TIGameState gotoGameState7 = CS$<>8__locals1.item.gotoGameState;
						if (gotoGameState7 != null && gotoGameState7.isSpaceFleetState)
						{
							TISpaceFleetState fleet = CS$<>8__locals1.item.gotoGameState.ref_fleet;
							array[num] = true;
							array3[num] = 0;
							array4[num] = Loc.T("UI.Notifications.SelectSpaceObject", new object[] { fleet.GetDisplayName(base.activePlayer) });
							this.customDelegateTooltip[num].SetDelegate("BodyText", () => fleet.FleetQuickDescription(CS$<>8__locals1.<>4__this.activePlayer));
							this.customDelegateTooltip[num].enabled = true;
							this.customButtonAction[num] = delegate
							{
								TIUtilities.GotoGameState(fleet, false, true, true, false, false, -1f);
							};
						}
						break;
					}
					case SpecialNotificationDelegate.SelectHabNoCamera:
					{
						TIGameState gotoGameState8 = CS$<>8__locals1.item.gotoGameState;
						if (gotoGameState8 != null && gotoGameState8.isHabState)
						{
							TIHabState hab = CS$<>8__locals1.item.gotoGameState.ref_hab;
							array[num] = true;
							array3[num] = 0;
							array4[num] = Loc.T("UI.Notifications.SelectSpaceObject", new object[] { hab.GetDisplayName(base.activePlayer) });
							this.customDelegateTooltip[num].SetDelegate("BodyText", () => hab.BuildShortHabSummary(CS$<>8__locals1.<>4__this.activePlayer));
							this.customDelegateTooltip[num].enabled = true;
							this.customButtonAction[num] = delegate
							{
								TIUtilities.GotoGameState(hab, false, true, true, false, false, -1f);
							};
						}
						break;
					}
					case SpecialNotificationDelegate.SelectNaturalSpaceObjectNoCamera:
					{
						TIGameState gotoGameState9 = CS$<>8__locals1.item.gotoGameState;
						if (((gotoGameState9 != null) ? gotoGameState9.ref_naturalSpaceObject : null) != null)
						{
							TINaturalSpaceObjectState nat = CS$<>8__locals1.item.gotoGameState.ref_naturalSpaceObject;
							array[num] = true;
							array3[num] = 0;
							array4[num] = Loc.T("UI.Notifications.SelectSpaceObject", new object[] { nat.GetDisplayName(base.activePlayer) });
							this.customDelegateTooltip[num].SetDelegate("BodyText", () => nat.SummaryTooltip(CS$<>8__locals1.<>4__this.activePlayer));
							this.customDelegateTooltip[num].enabled = true;
							this.customButtonAction[num] = delegate
							{
								TIUtilities.GotoGameState(nat, false, true, true, false, false, -1f);
							};
						}
						break;
					}
					}
					num++;
					if (num >= 6)
					{
						break;
					}
				}
				for (int i = 0; i < Mathf.Min(6, array.Length); i++)
				{
					if (array[i])
					{
						this.customDelegateButton[i].gameObject.SetActive(true);
						this.customDelegateButton[i].interactable = false;
						if (!array2[i])
						{
							base.StartCoroutine(this.EnableNarrativeButtonWithDelay(this.customDelegateButton[i]));
						}
						this.customDelegateButtonText[i].SetText(array4[i]);
						switch (array3[i])
						{
						default:
							this.customDelegateButtonSprite[i].enabled = false;
							this.customDelegateButtonText[i].margin = new Vector4(5f, 0f, 2f, 0f);
							break;
						case 1:
							GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathPauseButton, this.customDelegateButtonSprite[i]);
							this.customDelegateButtonSprite[i].enabled = true;
							this.customDelegateButtonText[i].margin = new Vector4(5f, 0f, 28f, 0f);
							break;
						case 2:
							GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathPlayButton, this.customDelegateButtonSprite[i]);
							this.customDelegateButtonSprite[i].enabled = true;
							this.customDelegateButtonText[i].margin = new Vector4(5f, 0f, 28f, 0f);
							break;
						}
					}
					else
					{
						this.customDelegateButton[i].gameObject.SetActive(false);
					}
				}
				this.customDelegatePanelObject.SetActive(this.customDelegateButton.Any<Button>((Button x) => x.gameObject.activeSelf));
				this.customDelegatePanelObject2.SetActive(this.customDelegateButton.Count<Button>((Button x) => x.gameObject.activeSelf) >= 4);
				this.customDelegateDropdown.gameObject.SetActive(flag);
				this.customDelegateDropdownToggle.transform.parent.gameObject.SetActive(flag);
				this.customDropdownDelegatePanelObject.SetActive(flag);
				return;
			}
			for (int j = 0; j < 6; j++)
			{
				this.customDelegateButton[j].gameObject.SetActive(false);
				this.customDelegateTooltip[j].enabled = false;
			}
			this.customDelegateDropdown.gameObject.SetActive(false);
			this.customDelegateDropdownToggle.transform.parent.gameObject.SetActive(false);
			this.DisableAllDelegatePanelObjects();
		}

		// Token: 0x060053B1 RID: 21425 RVA: 0x0025C3B8 File Offset: 0x0025A5B8
		private void DisableAllDelegatePanelObjects()
		{
			this.customDelegatePanelObject.SetActive(false);
			this.customDelegatePanelObject2.SetActive(false);
			this.customDropdownDelegatePanelObject.SetActive(false);
		}

		// Token: 0x060053B2 RID: 21426 RVA: 0x0025C3E0 File Offset: 0x0025A5E0
		private bool PopulateDropdownWithHabTemplateSelection(TIHabState hab)
		{
			bool flag = false;
			this.customDelegateDropdown.ClearOptions();
			this.selectionDropdownDict.Clear();
			this.customDelegateDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.SelectTemplate")
			});
			this.selectionDropdownDict.Add(0, null);
			this.customDelegateDropdown.captionText.SetText(Loc.T("UI.Habs.SelectTemplate"));
			int num = 1;
			foreach (TIHabTemplate tihabTemplate in base.activePlayer.habDesigns)
			{
				if (hab.CanApplySavedTemplate(tihabTemplate))
				{
					flag = true;
					this.selectionDropdownDict.Add(num, tihabTemplate.dataName);
					this.customDelegateDropdown.options.Add(new TMP_Dropdown.OptionData
					{
						text = Loc.T("UI.Habs.HabTemplateDropdownEntry", new object[]
						{
							tihabTemplate.displayName,
							tihabTemplate.AllModuleTemplates(false).Count,
							tihabTemplate.simpleBenefitsString
						}),
						image = tihabTemplate.naturalSpaceObject.icon
					});
					num++;
				}
			}
			return flag;
		}

		// Token: 0x060053B3 RID: 21427 RVA: 0x0025C52C File Offset: 0x0025A72C
		private TIResourcesCost CostWithNecessaryBoost(TIFactionState faction, TIResourcesCost baseLineCost, TIHabState hab)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost(baseLineCost);
			if (!tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
			{
				List<ResourceValue> list = baseLineCost.LackingResources(base.activePlayer);
				float num = 0f;
				double num2 = 0.0;
				foreach (ResourceValue resourceValue in list)
				{
					if (TIResourcesCost.replaceableSpaceResources.Contains(resourceValue.resource) && resourceValue.value > 0f)
					{
						tiresourcesCost.RemoveCost(resourceValue.resource);
						tiresourcesCost.AddCost(resourceValue.resource, faction.GetCurrentResourceAmount(resourceValue.resource), false);
						num += resourceValue.value * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(resourceValue.resource);
						num2 += TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, hab, resourceValue.value / TemplateManager.global.spaceResourceToTons);
					}
				}
				tiresourcesCost.AddCost(FactionResource.Money, num, true);
				tiresourcesCost.AddCost(FactionResource.Boost, (float)num2, true);
			}
			return tiresourcesCost;
		}

		// Token: 0x060053B4 RID: 21428 RVA: 0x0025C650 File Offset: 0x0025A850
		public void OnHabTemplateSelected(int selected)
		{
			if (selected == -1)
			{
				selected = this.customDelegateDropdown.value;
			}
			if (selected == 0)
			{
				this.customDelegateButton[this.customDropdownDelegateApplyButtonIndex].interactable = false;
				this.customDelegateTooltip[this.customDropdownDelegateApplyButtonIndex].SetDelegate("BodyText", () => Loc.T("UI.Habs.SelectTemplate"));
				return;
			}
			if (this.templateTargetHab == null)
			{
				return;
			}
			string text = this.selectionDropdownDict[this.customDelegateDropdown.value];
			if (!string.IsNullOrEmpty(text))
			{
				TIHabState tihabState = this.templateTargetHab;
				TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(text, false);
				TIResourcesCost tiresourcesCost;
				float num;
				List<TIHabModuleTemplate> list2;
				List<TIHabModuleTemplate> list = tihabState.ApplySavedTemplate(tihabTemplate, true, this.customDelegateDropdownToggle.isOn, out tiresourcesCost, out num, out list2);
				TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
				bool flag = true;
				if (!tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
				{
					tiresourcesCost2 = this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost, tihabState);
					flag = tiresourcesCost2.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				}
				StringBuilder query = new StringBuilder(tihabTemplate.displayName).AppendLine();
				if (flag)
				{
					StringBuilder query3 = query;
					string text2 = "UI.Habs.ConfirmApplication";
					object[] array = new object[1];
					array[0] = TIUtilities.ConstructTextList(list.ConvertAll<TIDataTemplate>((TIHabModuleTemplate x) => x), false, false);
					query3.AppendLine(Loc.T(text2, array));
					if (list2.Count > 0)
					{
						StringBuilder query2 = query;
						string text3 = "UI.Habs.FailedBuild";
						object[] array2 = new object[1];
						array2[0] = TIUtilities.ConstructTextList(list2.ConvertAll<TIDataTemplate>((TIHabModuleTemplate x) => x), false, false);
						query2.AppendLine(Loc.T(text3, array2));
					}
				}
				else
				{
					query.AppendLine(Loc.T("UI.Habs.CantAffordTemplate"));
				}
				query.AppendLine(Loc.T("UI.Habs.BaseCost", new object[] { tiresourcesCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				if (tiresourcesCost2.anyDebit)
				{
					query.AppendLine(Loc.T("UI.Habs.OurCost", new object[] { tiresourcesCost2.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				}
				if (tihabState.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.semiMajorAxis_AU > 1.0199999809265137)
				{
					if (tihabTemplate.AllModuleTemplates(true).Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.IsSolarPower))
					{
						query.AppendLine(Loc.T("UI.Habs.SolarPowerWarning"));
					}
				}
				if (num < 0f)
				{
					query.AppendLine(Loc.T("UI.Habs.TemplatePowerProblems", new object[]
					{
						(-num).ToString("N0"),
						TemplateManager.global.habPowerInlineSpritePath
					}));
				}
				this.customDelegateTooltip[this.customDropdownDelegateApplyButtonIndex].SetDelegate("BodyText", () => query.ToString());
				bool flag2 = flag && list.Count > 0 && tihabState.CanApplySavedTemplate(tihabTemplate);
				this.customDelegateButton[this.customDropdownDelegateApplyButtonIndex].interactable = flag2;
				return;
			}
			this.customDelegateDropdown.captionText.SetText(Loc.T("UI.Habs.SelectTemplate"));
			this.customDelegateTooltip[this.customDropdownDelegateApplyButtonIndex].SetDelegate("BodyText", () => Loc.T("UI.Habs.SelectTemplate"));
			this.customDelegateButton[this.customDropdownDelegateApplyButtonIndex].interactable = false;
		}

		// Token: 0x060053B5 RID: 21429 RVA: 0x0025CA0E File Offset: 0x0025AC0E
		public void OnCustomButtonPressed(int buttonNum)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.customButtonAction[buttonNum]();
		}

		// Token: 0x060053B6 RID: 21430 RVA: 0x0025CA29 File Offset: 0x0025AC29
		public void ToggleExpandedNewsFeed()
		{
			this.showText = !this.showText;
			this.SetNewsList(base.activePlayer);
		}

		// Token: 0x060053B7 RID: 21431 RVA: 0x0025CA48 File Offset: 0x0025AC48
		private void ExpandedNewsFeedSettings()
		{
			this.newsFeedTransform.offsetMax = new Vector2((float)(this.showText ? 311 : 36), -70f);
			this.newsFeedLayout.spacing = 2f;
			this.backgroundImage.enabled = this.showText;
		}

		// Token: 0x060053B8 RID: 21432 RVA: 0x0025CA9D File Offset: 0x0025AC9D
		private void ExpandNewsFeed(InfoWindowEntirelyClosed e)
		{
			this.ExpandedNewsFeedSettings();
			GameControl.eventManager.RemoveListener<InfoWindowEntirelyClosed>(new EventManager.EventDelegate<InfoWindowEntirelyClosed>(this.ExpandNewsFeed), null);
			GameControl.eventManager.AddListener<InfoPanelOpened>(new EventManager.EventDelegate<InfoPanelOpened>(this.ContractNewsFeed), null, null, true, false);
		}

		// Token: 0x060053B9 RID: 21433 RVA: 0x0025CAD8 File Offset: 0x0025ACD8
		private void ContractNewsFeed(InfoPanelOpened e)
		{
			this.newsFeedTransform.offsetMax = new Vector2((float)(this.showText ? 311 : 36), -915f);
			this.newsFeedLayout.spacing = 0f;
			this.backgroundImage.enabled = this.showText;
			GameControl.eventManager.RemoveListener<InfoPanelOpened>(new EventManager.EventDelegate<InfoPanelOpened>(this.ContractNewsFeed), null);
			GameControl.eventManager.AddListener<InfoWindowEntirelyClosed>(new EventManager.EventDelegate<InfoWindowEntirelyClosed>(this.ExpandNewsFeed), null, null, true, false);
		}

		// Token: 0x060053BA RID: 21434 RVA: 0x0025CB60 File Offset: 0x0025AD60
		private void UpdateNewsList(TIFactionState activePlayer, NotificationSummaryItem item)
		{
			if (item.newsFeedFactions.Contains(activePlayer))
			{
				Transform child = this.newsList.transform.GetChild(29);
				NewsFeedListItemController component = child.GetComponent<NewsFeedListItemController>();
				component.UpdateListItem(item, this.showText, false);
				child.SetAsFirstSibling();
				child.gameObject.SetActive(true);
				component.FlashNewsIcon();
				this.SetCloseAllButton(true);
			}
		}

		// Token: 0x060053BB RID: 21435 RVA: 0x0025CBC0 File Offset: 0x0025ADC0
		private void SetTimerList(TIFactionState activePlayer)
		{
			List<NotificationSummaryItem> list = new List<NotificationSummaryItem>();
			foreach (NotificationSummaryItem notificationSummaryItem in this.newsQueue.timerNotificationQueue)
			{
				List<TIFactionState> timerFactions = notificationSummaryItem.timerFactions;
				if (timerFactions != null && timerFactions.Contains(activePlayer) && TITimeState.Now().DifferenceInDays(notificationSummaryItem.dateTime) < 45.0)
				{
					list.Add(notificationSummaryItem);
				}
				if (list.Count >= 30)
				{
					break;
				}
			}
			int num = 0;
			using (IEnumerator<object> enumerator2 = this.timerList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (NotificationScreenController.<>o__234.<>p__0 == null)
					{
						NotificationScreenController.<>o__234.<>p__0 = CallSite<Func<CallSite, object, NewsFeedListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NewsFeedListItemController), typeof(NotificationScreenController)));
					}
					NewsFeedListItemController newsFeedListItemController = NotificationScreenController.<>o__234.<>p__0.Target(NotificationScreenController.<>o__234.<>p__0, enumerator2.Current);
					if (num < list.Count)
					{
						newsFeedListItemController.UpdateListItem(list[num++], false, true);
						newsFeedListItemController.gameObject.SetActive(true);
					}
					else
					{
						newsFeedListItemController.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060053BC RID: 21436 RVA: 0x0025CD18 File Offset: 0x0025AF18
		private void UpdateTimerList(TIFactionState activePlayer, NotificationSummaryItem item)
		{
			if (item.timerFactions.Contains(activePlayer))
			{
				Transform child = this.timerList.transform.GetChild(5);
				NewsFeedListItemController component = child.GetComponent<NewsFeedListItemController>();
				component.UpdateListItem(item, false, true);
				child.SetAsFirstSibling();
				child.gameObject.SetActive(true);
				component.FlashNewsIcon();
			}
			using (IEnumerator<object> enumerator = this.timerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NotificationScreenController.<>o__235.<>p__0 == null)
					{
						NotificationScreenController.<>o__235.<>p__0 = CallSite<Func<CallSite, object, NewsFeedListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NewsFeedListItemController), typeof(NotificationScreenController)));
					}
					NewsFeedListItemController newsFeedListItemController = NotificationScreenController.<>o__235.<>p__0.Target(NotificationScreenController.<>o__235.<>p__0, enumerator.Current);
					if (newsFeedListItemController.gameObject.activeInHierarchy && TITimeState.Now().DifferenceInDays(newsFeedListItemController.item.dateTime) >= 45.0)
					{
						newsFeedListItemController.FadeOutIcon();
					}
				}
			}
		}

		// Token: 0x060053BD RID: 21437 RVA: 0x0025CE1C File Offset: 0x0025B01C
		private void SetNewsList(TIFactionState activePlayer)
		{
			List<NotificationSummaryItem> list = new List<NotificationSummaryItem>();
			foreach (NotificationSummaryItem notificationSummaryItem in this.newsQueue.notificationSummaryQueue)
			{
				List<TIFactionState> newsFeedFactions = notificationSummaryItem.newsFeedFactions;
				if (newsFeedFactions != null && newsFeedFactions.Contains(activePlayer))
				{
					list.Add(notificationSummaryItem);
				}
				if (list.Count >= 30)
				{
					break;
				}
			}
			if (list.Count == 0 || !this.showText)
			{
				this.backgroundImage.enabled = false;
			}
			else
			{
				this.backgroundImage.enabled = true;
			}
			int num = 0;
			bool flag = false;
			using (IEnumerator<object> enumerator2 = this.newsList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (NotificationScreenController.<>o__236.<>p__0 == null)
					{
						NotificationScreenController.<>o__236.<>p__0 = CallSite<Func<CallSite, object, NewsFeedListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NewsFeedListItemController), typeof(NotificationScreenController)));
					}
					NewsFeedListItemController newsFeedListItemController = NotificationScreenController.<>o__236.<>p__0.Target(NotificationScreenController.<>o__236.<>p__0, enumerator2.Current);
					newsFeedListItemController.transform.SetAsLastSibling();
					if (num < list.Count)
					{
						newsFeedListItemController.UpdateListItem(list[num++], this.showText, false);
						newsFeedListItemController.gameObject.SetActive(true);
						flag = true;
					}
					else
					{
						newsFeedListItemController.gameObject.SetActive(false);
					}
				}
			}
			this.SetCloseAllButton(flag);
		}

		// Token: 0x060053BE RID: 21438 RVA: 0x0025CFA0 File Offset: 0x0025B1A0
		public void SetCloseAllButton(bool setting)
		{
			this.closeAllNewsFeedButton.gameObject.SetActive(setting);
		}

		// Token: 0x060053BF RID: 21439 RVA: 0x0025CFB4 File Offset: 0x0025B1B4
		public void OnClickCloseAllNewsFeed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			using (IEnumerator<object> enumerator = this.newsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NotificationScreenController.<>o__238.<>p__0 == null)
					{
						NotificationScreenController.<>o__238.<>p__0 = CallSite<Func<CallSite, object, NewsFeedListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NewsFeedListItemController), typeof(NotificationScreenController)));
					}
					NewsFeedListItemController newsFeedListItemController = NotificationScreenController.<>o__238.<>p__0.Target(NotificationScreenController.<>o__238.<>p__0, enumerator.Current);
					base.StartCoroutine(newsFeedListItemController.FadeOutIconEffect());
				}
			}
			this.SetCloseAllButton(false);
		}

		// Token: 0x060053C0 RID: 21440 RVA: 0x0025D05C File Offset: 0x0025B25C
		private void OnResourcesUpdatedWhileNarrativeEventActive(FactionResourcesUpdated e)
		{
			TINarrativeEventTemplate eventTemplate = this.currentNarrativeEvent.eventTemplate;
			TIGameState selectedTarget = this.currentNarrativeEvent.selectedTarget;
			TIGameState secondaryTarget = this.currentNarrativeEvent.secondaryTarget;
			if (eventTemplate == null || !TIGameState.Valid(selectedTarget) || (secondaryTarget != null && secondaryTarget.deleted))
			{
				return;
			}
			Dictionary<TIGameState, TIGameState> allTargetsandSeconds = this.currentNarrativeEvent.allTargetsandSeconds;
			for (int i = 0; i < 4; i++)
			{
				if (i < eventTemplate.numOptions)
				{
					this.optionButtons[i].gameObject.SetActive(true);
					this.optionButtonText[i].SetText(eventTemplate.optionButtonText(base.activePlayer, selectedTarget, secondaryTarget, i));
					this.optionButtonDetail[i].SetText("BodyText", eventTemplate.optionButtonDetail(base.activePlayer, selectedTarget, secondaryTarget, i, allTargetsandSeconds));
					bool flag = i == 0 || eventTemplate.eventOptions[i].ValidOption(base.activePlayer, selectedTarget, secondaryTarget);
					this.optionButtons[i].interactable = flag;
				}
				else
				{
					this.optionButtons[i].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x060053C1 RID: 21441 RVA: 0x0025D178 File Offset: 0x0025B378
		public void FillOutOptionButtons(TINarrativeEventTemplate template, TIGameState target, TIGameState secondaryTarget = null, Dictionary<TIGameState, TIGameState> allTargetsandSeconds = null)
		{
			for (int i = 0; i < 4; i++)
			{
				if (i < template.numOptions)
				{
					this.optionButtons[i].gameObject.SetActive(true);
					this.optionButtonText[i].SetText(template.optionButtonText(base.activePlayer, target, secondaryTarget, i));
					this.optionButtonDetail[i].SetText("BodyText", template.optionButtonDetail(base.activePlayer, target, secondaryTarget, i, allTargetsandSeconds));
					if (i == 0 || template.eventOptions[i].ValidOption(base.activePlayer, target, secondaryTarget))
					{
						this.optionButtons[i].interactable = false;
						base.StartCoroutine(this.EnableNarrativeButtonWithDelay(this.optionButtons[i]));
					}
					else
					{
						this.optionButtons[i].interactable = false;
					}
				}
				else
				{
					this.optionButtons[i].gameObject.SetActive(false);
				}
			}
			base.StartCoroutine(this.EnableNarrativeButtonHotkeysWithDelay());
		}

		// Token: 0x060053C2 RID: 21442 RVA: 0x0025D26E File Offset: 0x0025B46E
		private IEnumerator EnableNarrativeButtonWithDelay(Button buttonToEnable)
		{
			yield return new WaitForSeconds(TemplateManager.global.notificationReceiveInputDelay);
			buttonToEnable.interactable = true;
			yield break;
		}

		// Token: 0x060053C3 RID: 21443 RVA: 0x0025D27D File Offset: 0x0025B47D
		private IEnumerator EnableNarrativeButtonHotkeysWithDelay()
		{
			yield return new WaitForSeconds(TemplateManager.global.notificationReceiveInputDelay);
			TIInputManager.receivingInputForNarrativeHotkeys = true;
			yield break;
		}

		// Token: 0x060053C4 RID: 21444 RVA: 0x0025D288 File Offset: 0x0025B488
		public void OnOptionButtonPressed(int value)
		{
			if (!TIInputManager.receivingInputForNarrativeHotkeys || !this.optionButtons[value].gameObject.activeSelf || !this.optionButtons[value].interactable || !this.singleAlertBoxBody.activeSelf)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_NarrativeEventOptionButtonClick", false, false);
			Prompt prompt = TIGlobalValuesState.FindPromptForNarrativeEvent(base.activePlayer, this.currentNarrativeEvent.selectedTarget, this.currentNarrativeEvent.secondaryTarget, this.currentNarrativeEvent.eventTemplateName);
			if (this.currentNarrativeEvent.selectedTarget != null && !this.currentNarrativeEvent.selectedTarget.deleted)
			{
				base.activePlayer.playerControl.StartAction(new SelectNarrativeEventOption(base.activePlayer, this.currentNarrativeEvent.selectedTarget, this.currentNarrativeEvent.secondaryTarget, this.currentNarrativeEvent.eventTemplate, value, this.currentNarrativeEvent.allTargetsandSeconds, prompt));
			}
			else
			{
				Debug.LogError("Target for NarrativeEvent " + this.currentNarrativeEvent.eventTemplateName + " deleted or didn't exist before narrative event was resolved");
			}
			TooltipTrigger[] array = this.optionButtonDetail;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ForceHideTooltip();
			}
			TIInputManager.receivingInputForNarrativeHotkeys = false;
			GameControl.eventManager.RemoveListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnResourcesUpdatedWhileNarrativeEventActive), null);
			this.CleanUp(null);
			base.gameTime.Play();
		}

		// Token: 0x060053C5 RID: 21445 RVA: 0x0025D3E4 File Offset: 0x0025B5E4
		private void OnSetPolicyMission(TICouncilorState councilor)
		{
			this.masterPolicyPanelObject.SetActive(true);
			GameControl.eventManager.TriggerEvent(new PolicyMenuPushedToPlayer(), null, Array.Empty<object>());
			this.currentNation = councilor.currentNation;
			this.currentPolicyCouncilor = councilor;
			this.masterPolicyHeader.SetText(Loc.T("UI.Notifications.SelectPolicy", new object[] { this.currentNation.displayNameWithArticle }));
			this.masterPolicyFlag.sprite = this.currentNation.flag;
			GameControl.eventManager.AddListener<NationRelationsChange>(new EventManager.EventDelegate<NationRelationsChange>(this.UpdatePolicyOptions), null, this.currentNation, true, false);
			this.PopulatePolicyOptions();
		}

		// Token: 0x060053C6 RID: 21446 RVA: 0x0025D489 File Offset: 0x0025B689
		private void UpdatePolicyOptions(NationRelationsChange e)
		{
			this.PopulatePolicyOptions();
		}

		// Token: 0x060053C7 RID: 21447 RVA: 0x0025D494 File Offset: 0x0025B694
		private void PopulatePolicyOptions()
		{
			this.selectPolicyPanelObject.SetActive(true);
			this.selectPolicyTargetPanelObject.SetActive(false);
			this.backButtonObject.SetActive(false);
			List<IPolicyOption> list = PolicyManager.policies.Values.Where<IPolicyOption>((IPolicyOption x) => !x.HandledAtFactionLevel()).ToList<IPolicyOption>();
			this.policyOptionsList.SetListSize<PolicyListItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.policyOptionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NotificationScreenController.<>o__254.<>p__0 == null)
					{
						NotificationScreenController.<>o__254.<>p__0 = CallSite<Func<CallSite, object, PolicyListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PolicyListItemController), typeof(NotificationScreenController)));
					}
					NotificationScreenController.<>o__254.<>p__0.Target(NotificationScreenController.<>o__254.<>p__0, enumerator.Current).SetListItem(this, list[num++] as TIPolicyOption, this.currentNation);
				}
			}
		}

		// Token: 0x060053C8 RID: 21448 RVA: 0x0025D5A4 File Offset: 0x0025B7A4
		public void PolicySelected(TIPolicyOption policy)
		{
			this.currentPolicy = policy;
			if (policy.RequiresTargets())
			{
				this.PopulatePolicyTargets();
				return;
			}
			if (policy.TargetsMyFederation)
			{
				this.currentPolicyTarget = this.currentNation.federation;
			}
			else
			{
				this.currentPolicyTarget = this.currentNation;
			}
			this.ConfirmDialog();
		}

		// Token: 0x060053C9 RID: 21449 RVA: 0x0025D5F4 File Offset: 0x0025B7F4
		private void PopulatePolicyTargets()
		{
			this.selectPolicyPanelObject.SetActive(false);
			this.selectPolicyTargetPanelObject.SetActive(true);
			this.backButtonObject.SetActive(true);
			this.masterPolicyHeader.SetText(this.currentPolicy.GetTargetSelectionHeaderText());
			IList<TIGameState> list = this.currentPolicy.GetPossibleTargets(this.currentNation);
			if (this.currentPolicy.RequiresTargetConfirm())
			{
				TIPolicyOptionWithConfirm confirmablePolicy = this.currentPolicy as TIPolicyOptionWithConfirm;
				list = (from x in list
					orderby confirmablePolicy.AIAgreeChance(this.currentNation, x) descending, x.displayName
					select x).ToList<TIGameState>();
			}
			this.policyTargetsList.SetListSize<PolicyTargetGridItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.policyTargetsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NotificationScreenController.<>o__256.<>p__0 == null)
					{
						NotificationScreenController.<>o__256.<>p__0 = CallSite<Func<CallSite, object, PolicyTargetGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PolicyTargetGridItemController), typeof(NotificationScreenController)));
					}
					PolicyTargetGridItemController policyTargetGridItemController = NotificationScreenController.<>o__256.<>p__0.Target(NotificationScreenController.<>o__256.<>p__0, enumerator.Current);
					policyTargetGridItemController.Init(this);
					policyTargetGridItemController.UpdateListItem(list[num++], this.currentPolicy, this.currentPolicyCouncilor.faction, this.currentNation);
				}
			}
		}

		// Token: 0x060053CA RID: 21450 RVA: 0x0025D770 File Offset: 0x0025B970
		public void PolicyTargetSelected(TIGameState target)
		{
			this.currentPolicyTarget = target;
			this.ConfirmDialog();
		}

		// Token: 0x060053CB RID: 21451 RVA: 0x0025D780 File Offset: 0x0025B980
		public void OnClickBackButton()
		{
			this.currentPolicy = null;
			this.currentPolicyTarget = null;
			this.selectPolicyTargetPanelObject.SetActive(false);
			this.selectPolicyPanelObject.SetActive(true);
			this.masterPolicyHeader.SetText(Loc.T("UI.Notifications.SelectPolicy", new object[] { this.currentNation.displayNameWithArticle }));
			this.confirmPanelObject.SetActive(false);
			this.backButtonObject.SetActive(false);
		}

		// Token: 0x060053CC RID: 21452 RVA: 0x0025D7F4 File Offset: 0x0025B9F4
		private void ConfirmDialog()
		{
			this.confirmPanelObject.SetActive(true);
			this.confirmPanelText.SetText(this.currentPolicy.GetConfirmPrompt(this.currentNation, this.currentPolicyTarget));
		}

		// Token: 0x060053CD RID: 21453 RVA: 0x0025D824 File Offset: 0x0025BA24
		public void OnConfirmPolicy()
		{
			this.ShutdownPolicyPanels();
			base.activePlayer.playerControl.StartAction(new ConfirmPolicyAction(this.currentNation, base.activePlayer, this.currentPolicyTarget, this.currentPolicyCouncilor, this.currentPolicy));
		}

		// Token: 0x060053CE RID: 21454 RVA: 0x0025D85F File Offset: 0x0025BA5F
		private void ShutdownPolicyPanels()
		{
			this.masterPolicyPanelObject.SetActive(false);
			this.selectPolicyTargetPanelObject.SetActive(false);
			this.confirmPanelObject.SetActive(false);
			GameControl.eventManager.RemoveListener<NationRelationsChange>(new EventManager.EventDelegate<NationRelationsChange>(this.UpdatePolicyOptions), null);
		}

		// Token: 0x060053CF RID: 21455 RVA: 0x0025D89C File Offset: 0x0025BA9C
		public void OnCancelPolicy()
		{
			this.confirmPanelObject.SetActive(false);
			GameControl.eventManager.RemoveListener<NationRelationsChange>(new EventManager.EventDelegate<NationRelationsChange>(this.UpdatePolicyOptions), null);
		}

		// Token: 0x060053D0 RID: 21456 RVA: 0x0025D8C1 File Offset: 0x0025BAC1
		public void PushPromptResponse(BlockingPromptOnStartup e)
		{
			this.PushNationPromptResponse();
		}

		// Token: 0x060053D1 RID: 21457 RVA: 0x0025D8CC File Offset: 0x0025BACC
		public void PushNationPromptResponse()
		{
			Prompt prompt = this.promptQueue.activePlayerNationPromptList.First<Prompt>();
			string name = prompt.name;
			if (name != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
				if (num <= 2900450226U)
				{
					if (num <= 2355149519U)
					{
						if (num != 1481396765U)
						{
							if (num != 2355149519U)
							{
								goto IL_022D;
							}
							if (!(name == "PromptRespondToEndRivalryCall"))
							{
								goto IL_022D;
							}
							goto IL_01AB;
						}
						else
						{
							if (!(name == "PromptSelectPolicy"))
							{
								goto IL_022D;
							}
							this.OnSetPolicyMission(prompt.relatedGameState.ref_councilor);
							return;
						}
					}
					else if (num != 2737292978U)
					{
						if (num != 2883068329U)
						{
							if (num != 2900450226U)
							{
								goto IL_022D;
							}
							if (!(name == "PromptRespondToTransferRegionCall"))
							{
								goto IL_022D;
							}
						}
						else
						{
							if (!(name == "PromptNationLeavesDarkFederation_Violent"))
							{
								goto IL_022D;
							}
							this.OnNationLeavesMyDarkFederation_Violent(prompt);
							return;
						}
					}
					else
					{
						if (!(name == "PromptRespondToFormAllianceCall"))
						{
							goto IL_022D;
						}
						goto IL_01AB;
					}
				}
				else if (num <= 3716459916U)
				{
					if (num != 2995569124U)
					{
						if (num != 3394191324U)
						{
							if (num != 3716459916U)
							{
								goto IL_022D;
							}
							if (!(name == "PromptRespondToJoinFederationCall"))
							{
								goto IL_022D;
							}
							goto IL_01AB;
						}
						else
						{
							if (!(name == "PromptArmyOrderedToDepart"))
							{
								goto IL_022D;
							}
							this.OnNationAsksRemoveArmiesPrompt(prompt);
							return;
						}
					}
					else if (!(name == "PromptNationLeavesDarkFederation_Policy"))
					{
						goto IL_022D;
					}
				}
				else if (num != 3757877436U)
				{
					if (num != 4016174268U)
					{
						if (num != 4202808030U)
						{
							goto IL_022D;
						}
						if (!(name == "PromptRespondToUnificationCall"))
						{
							goto IL_022D;
						}
						goto IL_01AB;
					}
					else
					{
						if (!(name == "PromptRespondToAllyOffensiveWarCall"))
						{
							goto IL_022D;
						}
						this.AllyCalledToOffensiveWar(prompt.actingState as TINationState, prompt.promptingGameState as TINationState, prompt.relatedGameState as TIWarState);
						return;
					}
				}
				else if (!(name == "PromptRespondToEndWarCall"))
				{
					goto IL_022D;
				}
				this.NationPromptPolicyResponse(prompt);
				return;
				IL_01AB:
				if (base.activePlayer.ignoreInterstateDiplomacy.Contains(prompt.promptingGameState.ref_faction))
				{
					this.currentPrompt = prompt;
					this.OnResponseDecline();
					return;
				}
				this.NationPromptPolicyResponse(prompt);
				return;
			}
			IL_022D:
			this.promptQueue.RemovePrompt(prompt);
		}

		// Token: 0x060053D2 RID: 21458 RVA: 0x0025DB14 File Offset: 0x0025BD14
		private void NationPromptPolicyResponse(Prompt prompt)
		{
			this.respondingNation = prompt.actingState.ref_nation;
			if (this.respondingNation.executiveFaction == GameControl.control.activePlayer)
			{
				this.currentPrompt = prompt;
				this.promptingNation = prompt.promptingGameState.ref_nation;
				this.relatedGameState = prompt.relatedGameState;
				this.promptName = prompt.name;
				string text = this.promptName;
				if (text != null)
				{
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
					if (num <= 2900450226U)
					{
						if (num != 2355149519U)
						{
							if (num != 2737292978U)
							{
								if (num != 2900450226U)
								{
									return;
								}
								if (!(text == "PromptRespondToTransferRegionCall"))
								{
									return;
								}
								this.policyPromptingResponse = (TIPolicyOption)PolicyManager.policies[PolicyType.TransferRegionsOption];
								this.responsePanelObject.SetActive(true);
								this.responsePanelNationName.SetText(this.respondingNation.displayName);
								this.responsePanelText.SetText(this.policyPromptingResponse.GetResponsePrompt(this.promptingNation, this.respondingNation, this.relatedGameState));
							}
							else
							{
								if (!(text == "PromptRespondToFormAllianceCall"))
								{
									return;
								}
								this.policyPromptingResponse = (TIPolicyOption)PolicyManager.policies[PolicyType.ProposeAllianceOption];
								this.responsePanelObject.SetActive(true);
								this.responsePanelNationName.SetText(this.respondingNation.displayName);
								this.responsePanelText.SetText(this.policyPromptingResponse.GetResponsePrompt(this.promptingNation, this.respondingNation, this.respondingNation));
								return;
							}
						}
						else
						{
							if (!(text == "PromptRespondToEndRivalryCall"))
							{
								return;
							}
							this.policyPromptingResponse = (TIPolicyOption)PolicyManager.policies[PolicyType.EndRivalryOption];
							this.responsePanelObject.SetActive(true);
							this.responsePanelNationName.SetText(this.respondingNation.displayName);
							this.responsePanelText.SetText(this.policyPromptingResponse.GetResponsePrompt(this.promptingNation, this.respondingNation, this.respondingNation));
							return;
						}
					}
					else if (num <= 3716459916U)
					{
						if (num != 2995569124U)
						{
							if (num != 3716459916U)
							{
								return;
							}
							if (!(text == "PromptRespondToJoinFederationCall"))
							{
								return;
							}
							this.policyPromptingResponse = (TIPolicyOption)PolicyManager.policies[PolicyType.JoinFederationOption];
							this.responsePanelObject.SetActive(true);
							this.responsePanelNationName.SetText(this.respondingNation.displayName);
							this.responsePanelText.SetText(this.policyPromptingResponse.GetResponsePrompt(this.promptingNation, this.respondingNation, this.respondingNation));
							return;
						}
						else
						{
							if (!(text == "PromptNationLeavesDarkFederation_Policy"))
							{
								return;
							}
							this.policyPromptingResponse = (TIPolicyOption)PolicyManager.policies[PolicyType.LeaveFederationOption];
							this.responsePanelObject.SetActive(true);
							this.responsePanelNationName.SetText(this.respondingNation.displayName);
							this.responsePanelText.SetText(this.policyPromptingResponse.GetResponsePrompt(this.promptingNation, this.respondingNation, this.respondingNation));
							return;
						}
					}
					else if (num != 3757877436U)
					{
						if (num != 4202808030U)
						{
							return;
						}
						if (!(text == "PromptRespondToUnificationCall"))
						{
							return;
						}
						this.policyPromptingResponse = (TIPolicyOption)PolicyManager.policies[PolicyType.UnificationOption];
						this.responsePanelObject.SetActive(true);
						this.responsePanelNationName.SetText(this.respondingNation.displayName);
						this.responsePanelText.SetText(this.policyPromptingResponse.GetResponsePrompt(this.promptingNation, this.respondingNation, this.respondingNation));
						return;
					}
					else
					{
						if (!(text == "PromptRespondToEndWarCall"))
						{
							return;
						}
						this.policyPromptingResponse = (TIPolicyOption)PolicyManager.policies[PolicyType.EndWarOption];
						this.responsePanelObject.SetActive(true);
						this.responsePanelNationName.SetText(this.respondingNation.displayName);
						this.responsePanelText.SetText(this.policyPromptingResponse.GetResponsePrompt(this.promptingNation, this.respondingNation, this.relatedGameState));
						return;
					}
				}
			}
		}

		// Token: 0x060053D3 RID: 21459 RVA: 0x0025DF04 File Offset: 0x0025C104
		public void OnReponseConfirm()
		{
			this.promptQueue.RemovePrompt(this.currentPrompt);
			if (this.promptName == "PromptNationLeavesDarkFederation_Violent")
			{
				base.activePlayer.playerControl.StartAction(new ConfirmPolicyAction(this.respondingNation, this.respondingNation.executiveFaction, this.promptingNation, null, new WarOption()));
			}
			else
			{
				base.activePlayer.playerControl.StartAction(new RespondToPolicyProposalAction(this.respondingNation, this.promptingNation, this.relatedGameState, this.policyPromptingResponse, true));
			}
			this.ShutdownPolicyPanels();
			this.responsePanelObject.SetActive(false);
		}

		// Token: 0x060053D4 RID: 21460 RVA: 0x0025DFAC File Offset: 0x0025C1AC
		public void OnResponseDecline()
		{
			this.promptQueue.RemovePrompt(this.currentPrompt);
			if (!(this.promptName == "PromptNationLeavesDarkFederation_Violent"))
			{
				base.activePlayer.playerControl.StartAction(new RespondToPolicyProposalAction(this.respondingNation, this.promptingNation, this.relatedGameState, this.policyPromptingResponse, false));
			}
			this.ShutdownPolicyPanels();
			this.responsePanelObject.SetActive(false);
		}

		// Token: 0x060053D5 RID: 21461 RVA: 0x0025E020 File Offset: 0x0025C220
		private void OnNationLeavesMyDarkFederation_Violent(Prompt prompt)
		{
			this.respondingNation = prompt.actingState.ref_nation;
			if (this.respondingNation.executiveFaction == GameControl.control.activePlayer)
			{
				this.currentPrompt = prompt;
				this.respondingNation = prompt.actingState.ref_nation;
				this.promptingNation = prompt.promptingGameState.ref_nation;
				this.relatedGameState = prompt.relatedGameState;
				this.promptName = prompt.name;
				this.responsePanelObject.SetActive(true);
				this.responsePanelNationName.SetText(this.respondingNation.displayName);
				this.responsePanelText.SetText(Loc.T("UI.Notifications.LeaveDarkFederation_Violent_Query", new object[]
				{
					prompt.promptingGameState.ref_nation.displayNameWithArticleAndPlacePrep,
					this.respondingNation.federation.displayNameWithArticle,
					this.respondingNation.nationalAdjective
				}));
			}
		}

		// Token: 0x060053D6 RID: 21462 RVA: 0x0025E118 File Offset: 0x0025C318
		private void AllyCalledToOffensiveWar(TINationState calledAlly, TINationState callingNation, TIWarState war)
		{
			this.calledAlly = calledAlly;
			this.callingNation = callingNation;
			this.war = war;
			List<TIGameState> list = new List<TINationState>(war.defendingAlliance).ConvertAll<TIGameState>((TINationState x) => x);
			this.callAllyPrompt.SetText(Loc.T("UI.Notifications.CallAllyPrompt", new object[]
			{
				callingNation.displayNameWithArticleCapitalized,
				calledAlly.displayNameWithArticle,
				TIUtilities.ConstructTextList(list, false, false)
			}));
			this.callAllyResponseObject.SetActive(true);
		}

		// Token: 0x060053D7 RID: 21463 RVA: 0x0025E1B0 File Offset: 0x0025C3B0
		public void JoinWarButton()
		{
			this.calledAlly.executiveFaction.playerControl.StartAction(new RespondToCallAllyAction(this.calledAlly, this.callingNation, this.war, true));
			this.promptQueue.RemovePrompt(new Prompt(this.calledAlly, this.callingNation, this.war, "PromptRespondToAllyOffensiveWarCall", 0));
			this.callAllyResponseObject.SetActive(false);
		}

		// Token: 0x060053D8 RID: 21464 RVA: 0x0025E220 File Offset: 0x0025C420
		public void DeclineWarButton()
		{
			this.calledAlly.executiveFaction.playerControl.StartAction(new RespondToCallAllyAction(this.calledAlly, this.callingNation, this.war, false));
			this.promptQueue.RemovePrompt(new Prompt(this.calledAlly, this.callingNation, this.war, "PromptRespondToAllyOffensiveWarCall", 0));
			this.callAllyResponseObject.SetActive(false);
		}

		// Token: 0x060053D9 RID: 21465 RVA: 0x0025E290 File Offset: 0x0025C490
		private void OnNationAsksRemoveArmiesPrompt(Prompt prompt)
		{
			this.nationWithArmies = prompt.actingState.ref_nation;
			if (this.nationWithArmies.executiveFaction == GameControl.control.activePlayer && !this.removeArmiesPromptObject.activeInHierarchy)
			{
				this.currentPrompt = prompt;
				this.nationAskingArmiesToLeave = this.currentPrompt.promptingGameState.ref_nation;
				TIResourcesCost factionLevelRelationShipChangeCost = TINationState.FactionLevelRelationShipChangeCost;
				float num = new ProposeAllianceOption().AIAgreeChance_Prospective(this.nationWithArmies, this.nationAskingArmiesToLeave);
				this.removeArmiesPromptText.SetText(Loc.T("UI.Notifications.RemoveArmiesPrompt", new object[]
				{
					this.nationWithArmies.displayNameWithArticle,
					this.nationAskingArmiesToLeave.displayNameWithArticleAndPlacePrep,
					num.ToPercent("P0")
				}));
				this.proposeAllianceButtonText.SetText(Loc.T("UI.Notifications.ProposeAllianceRemoveArmiesPrompt", new object[] { factionLevelRelationShipChangeCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				this.declareWarButtonText.SetText(Loc.T("UI.Notifications.DeclareWarArmiesPrompt", new object[] { factionLevelRelationShipChangeCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				this.removeArmies_proposeAllianceButton.interactable = this.nationWithArmies.CanAllyForRemoveArmyPrompt(this.nationAskingArmiesToLeave);
				this.removeArmies_declareWarButton.interactable = this.nationWithArmies.CanDeclareWarForRemoveArmyPrompt(this.nationAskingArmiesToLeave, prompt.relatedGameState != null);
				this.removeArmies_myNationFlag.sprite = this.nationWithArmies.flag;
				this.removeArmies_myFactionIcon.sprite = this.nationWithArmies.executiveFaction.factionIcon64;
				this.removeArmies_theirNationFlag.sprite = this.nationAskingArmiesToLeave.flag;
				if (this.nationAskingArmiesToLeave.executiveFaction != null)
				{
					this.removeArmies_theirFactionIcon.sprite = this.nationAskingArmiesToLeave.executiveFaction.factionIcon64;
					this.removeArmies_theirFactionIcon.enabled = true;
				}
				else
				{
					this.removeArmies_theirFactionIcon.enabled = false;
				}
				this.removeArmiesPromptObject.SetActive(true);
			}
			if (this.nationWithArmies.executiveFaction == null && !this.nationWithArmies.extant)
			{
				TIPromptQueueState.RemovePromptStatic(prompt);
			}
		}

		// Token: 0x060053DA RID: 21466 RVA: 0x0025E4C9 File Offset: 0x0025C6C9
		public void removeArmies_ProposeAlliancePressed()
		{
			this.nationWithArmies.HandlePromptArmyOrderedToDepartDecision(this.nationAskingArmiesToLeave, ArmyOrderedToDepartOptions.OfferAlliance, this.currentPrompt);
			this.removeArmiesPromptObject.SetActive(false);
		}

		// Token: 0x060053DB RID: 21467 RVA: 0x0025E4EF File Offset: 0x0025C6EF
		public void removeArmies_DeclareWarPressed()
		{
			this.nationWithArmies.HandlePromptArmyOrderedToDepartDecision(this.nationAskingArmiesToLeave, ArmyOrderedToDepartOptions.DeclareWar, this.currentPrompt);
			this.removeArmiesPromptObject.SetActive(false);
		}

		// Token: 0x060053DC RID: 21468 RVA: 0x0025E515 File Offset: 0x0025C715
		public void removeArmies_GoHomePressed()
		{
			this.nationWithArmies.HandlePromptArmyOrderedToDepartDecision(this.nationAskingArmiesToLeave, ArmyOrderedToDepartOptions.Depart, this.currentPrompt);
			this.removeArmiesPromptObject.SetActive(false);
		}

		// Token: 0x060053DD RID: 21469 RVA: 0x0025E53C File Offset: 0x0025C73C
		public void PushMissionPromptResponse(Prompt currentMissionPrompt)
		{
			this.targetOrg = null;
			this.targetProject = null;
			this.missionTargetButton.interactable = false;
			this.currentMissionPrompt = currentMissionPrompt;
			this.activeMission = currentMissionPrompt.relatedGameState as TIMissionState;
			this.missionTargetingUIHeaderText.SetText(Loc.T(new StringBuilder("TIMissionTemplate.Prompt.").Append(this.activeMission.templateName).ToString(), new object[] { this.activeMission.target.displayName }));
			string name = currentMissionPrompt.name;
			if (name != null)
			{
				if (name == "PromptSabotageProject")
				{
					this.BuildSabotageProjectMissionTargetList(this.activeMission);
					return;
				}
				if (name == "PromptStealProject")
				{
					this.BuildStealProjectMissionTargetList(this.activeMission);
					return;
				}
				if (name == "PromptFactionContactMakeOffer")
				{
					this.StartDiplomacyMission(this.activeMission, currentMissionPrompt.promptingGameState as TICouncilorState);
					return;
				}
				if (!(name == "PromptChangeTrajectory"))
				{
					return;
				}
				TIFactionState ref_faction = this.currentPrompt.promptingGameState.ref_faction;
				TISpaceFleetState ref_fleet = this.currentPrompt.promptingGameState.ref_fleet;
				TIGameState tigameState = this.currentPrompt.relatedGameState;
				this.StartPromptChangeTrajectory(ref_faction, ref_fleet, (tigameState != null) ? tigameState.ref_fleet : null, this.currentPrompt.promptingGameState.ref_fleet.proposedTrajectories);
			}
		}

		// Token: 0x060053DE RID: 21470 RVA: 0x0025E68C File Offset: 0x0025C88C
		public void PushOperationPromptResponse(Prompt currentPrompt)
		{
			this.targetOrg = null;
			this.targetProject = null;
			this.missionTargetButton.interactable = false;
			string name = currentPrompt.name;
			if (name != null && name == "PromptChangeTrajectory")
			{
				TIFactionState ref_faction = currentPrompt.promptingGameState.ref_faction;
				TISpaceFleetState ref_fleet = currentPrompt.promptingGameState.ref_fleet;
				TIGameState tigameState = currentPrompt.relatedGameState;
				this.StartPromptChangeTrajectory(ref_faction, ref_fleet, (tigameState != null) ? tigameState.ref_fleet : null, currentPrompt.promptingGameState.ref_fleet.proposedTrajectories);
			}
		}

		// Token: 0x060053DF RID: 21471 RVA: 0x0025E710 File Offset: 0x0025C910
		public void BuildSabotageProjectMissionTargetList(TIMissionState mission)
		{
			List<TIProjectTemplate> list = mission.target.ref_faction.ProjectsVulnerableToSabotage(mission.councilor.faction);
			if (list.Count == 0)
			{
				Log.Error("BuildSabotageProjectMissionTargetList triggered with no targets", Array.Empty<object>());
			}
			this.missionTargetingUIObject.SetActive(true);
			this.missionTargetingUIList.SetListSize<MissionTargetListItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.missionTargetingUIList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NotificationScreenController.<>o__293.<>p__0 == null)
					{
						NotificationScreenController.<>o__293.<>p__0 = CallSite<Func<CallSite, object, MissionTargetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(MissionTargetListItemController), typeof(NotificationScreenController)));
					}
					MissionTargetListItemController missionTargetListItemController = NotificationScreenController.<>o__293.<>p__0.Target(NotificationScreenController.<>o__293.<>p__0, enumerator.Current);
					missionTargetListItemController.Init(this);
					missionTargetListItemController.SetListItem(mission.target.ref_faction, mission.target.ref_faction.GetProjectProgressByTemplate(list[num++]));
				}
			}
			this.UpdateMissionTargetText(mission);
		}

		// Token: 0x060053E0 RID: 21472 RVA: 0x0025E824 File Offset: 0x0025CA24
		public void BuildStealProjectMissionTargetList(TIMissionState mission)
		{
			List<TIProjectTemplate> list = mission.target.ref_faction.StealableProjects(mission.councilor.faction);
			if (list.Count == 0)
			{
				Log.Error("BuildStealProjectMissionTargetList triggered with no targets", Array.Empty<object>());
			}
			this.missionTargetingUIObject.SetActive(true);
			this.missionTargetingUIList.SetListSize<MissionTargetListItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.missionTargetingUIList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NotificationScreenController.<>o__294.<>p__0 == null)
					{
						NotificationScreenController.<>o__294.<>p__0 = CallSite<Func<CallSite, object, MissionTargetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(MissionTargetListItemController), typeof(NotificationScreenController)));
					}
					MissionTargetListItemController missionTargetListItemController = NotificationScreenController.<>o__294.<>p__0.Target(NotificationScreenController.<>o__294.<>p__0, enumerator.Current);
					missionTargetListItemController.Init(this);
					missionTargetListItemController.SetListItem(list[num++]);
				}
			}
			this.UpdateMissionTargetText(mission);
		}

		// Token: 0x060053E1 RID: 21473 RVA: 0x0025E91C File Offset: 0x0025CB1C
		public void StartPromptChangeTrajectory(TIFactionState faction, TISpaceFleetState maneuveringFleet, TISpaceFleetState targetFleet, Trajectory[] validTrajectories = null)
		{
			if (!(base.canvasManager.OperationCanvasController as OperationCanvasController).changeTrajectoryCanvas.enabled)
			{
				GameControl.eventManager.TriggerEvent(new ForceTrajectorySelectionUI(faction, maneuveringFleet, targetFleet, validTrajectories), null, Array.Empty<object>());
			}
		}

		// Token: 0x060053E2 RID: 21474 RVA: 0x0025E954 File Offset: 0x0025CB54
		private void UpdateMissionTargetText(TIMissionState mission)
		{
			string text = string.Empty;
			if (this.targetOrg != null)
			{
				text = this.targetOrg.displayName;
			}
			else if (this.targetProject != null)
			{
				text = this.targetProject.displayName;
			}
			else
			{
				text = Loc.T("TIMissionTargeting_NoTarget");
			}
			this.missionTargetText.SetText(Loc.T("TIMissionTemplate.SecondarySelection", new object[] { text }));
		}

		// Token: 0x060053E3 RID: 21475 RVA: 0x0025E9C3 File Offset: 0x0025CBC3
		public void MissionTargetSelected(TIOrgState org)
		{
			this.missionTargetButton.interactable = true;
			this.targetOrg = org;
			this.UpdateMissionTargetText(this.activeMission);
		}

		// Token: 0x060053E4 RID: 21476 RVA: 0x0025E9E4 File Offset: 0x0025CBE4
		public void MissionTargetSelected(TIProjectTemplate project)
		{
			this.missionTargetButton.interactable = true;
			this.targetProject = project;
			this.UpdateMissionTargetText(this.activeMission);
		}

		// Token: 0x060053E5 RID: 21477 RVA: 0x0025EA08 File Offset: 0x0025CC08
		public void OnClickMissionTargetConfirm()
		{
			this.missionTargetingUIObject.SetActive(false);
			string name = this.currentMissionPrompt.name;
			if (name != null)
			{
				if (name == "PromptSabotageProject")
				{
					base.activePlayer.playerControl.StartAction(new SabotageProjectAction(this.activeMission, this.targetProject));
					return;
				}
				if (!(name == "PromptStealProject"))
				{
					return;
				}
				base.activePlayer.playerControl.StartAction(new StealProjectAction(this.activeMission, this.targetProject));
			}
		}

		// Token: 0x060053E6 RID: 21478 RVA: 0x0025EA90 File Offset: 0x0025CC90
		public void OnClickMissionTargetCancel()
		{
			TIPromptQueueState.RemovePromptStatic(this.activeMission.councilor.faction, this.activeMission.councilor, this.activeMission, this.currentMissionPrompt.name, 0);
			base.activePlayer.playerControl.StartAction(new AbortMission(this.activeMission.councilor, true, TIMissionState.AbortReason.VoluntaryAbort, this.activeMission, ""));
			this.missionTargetingUIObject.SetActive(false);
		}

		// Token: 0x060053E7 RID: 21479 RVA: 0x0025EB0C File Offset: 0x0025CD0C
		private void StartDiplomacyMission(TIMissionState mission, TICouncilorState targetCouncilor)
		{
			if (!this.factionDiplomacyGreetingUIObject.activeSelf && !this.factionDiplomacyTradeUIObject.activeSelf)
			{
				this.diplomacyMissionState = mission;
				this.diplomacyCouncilorState = targetCouncilor;
				string[] array = new string[6];
				array[0] = "Initiating Trade;Initiator:";
				int num = 1;
				TIFactionState ref_faction = this.activeMission.ref_faction;
				array[num] = ((ref_faction != null) ? ref_faction.ToString() : null);
				array[2] = " ,Receiver:";
				int num2 = 3;
				TIFactionState ref_faction2 = this.activeMission.target.ref_faction;
				array[num2] = ((ref_faction2 != null) ? ref_faction2.ToString() : null);
				array[4] = " ,AITradeFlag:";
				array[5] = this.aiOffer.ToString();
				Debug.Log(string.Concat(array));
				this.OpenDiplomacyGreetingUI(targetCouncilor);
			}
		}

		// Token: 0x060053E8 RID: 21480 RVA: 0x0025EBBD File Offset: 0x0025CDBD
		public void StartAIToPlayerDiplomacyMission(TradeToPlayerInitiated e)
		{
			this.contactedFaction = e.contacted_Faction;
			this.diploMission = e.mission;
			this.aiOffer = true;
			Debug.Log("Initiating AI to Player Trade");
		}

		// Token: 0x060053E9 RID: 21481 RVA: 0x0025EBE8 File Offset: 0x0025CDE8
		public void StartAIToPlayerDiplomacyMission(TIMissionState mission, TICouncilorState targetCouncilor, TIFactionState contacted_Faction)
		{
			this.contactedFaction = contacted_Faction;
			this.diploMission = mission;
			this.aiOffer = true;
			Debug.Log("Initiating AI to Player Trade");
		}

		// Token: 0x060053EA RID: 21482 RVA: 0x0025EC0C File Offset: 0x0025CE0C
		private void OpenDiplomacyGreetingUI(TICouncilorState targetCouncilor)
		{
			this.GetDiplomacyWindowText(this.diplomacyCouncilorState);
			AudioManager.PlayOneShot("event:/SFX/UI_Special_SFX/trig_SFX_Incoming_Comms_Player", false, false);
			string[] array = new string[6];
			array[0] = "Launching Trade Greeting;Initiator:";
			int num = 1;
			TIFactionState ref_faction = this.activeMission.ref_faction;
			array[num] = ((ref_faction != null) ? ref_faction.ToString() : null);
			array[2] = " ,Receiver:";
			int num2 = 3;
			TIFactionState ref_faction2 = this.activeMission.target.ref_faction;
			array[num2] = ((ref_faction2 != null) ? ref_faction2.ToString() : null);
			array[4] = " ,AITradeFlag:";
			array[5] = this.aiOffer.ToString();
			Debug.Log(string.Concat(array));
			if (this.aiOffer && this.activeMission.ref_faction == base.activePlayer)
			{
				Debug.LogError("Illegal Trade Mission: Cannot trade with self, attempting fix");
				this.aiOffer = false;
			}
			if (!this.aiOffer)
			{
				if (this.activeMission.target.ref_faction == base.activePlayer || (this.activeMission.target.ref_faction.IsAlienFaction && !base.activePlayer.CanContactAlien))
				{
					this.DiplomacyClose();
					return;
				}
				this.factionDiplomacyGreetingTitleText.text = new StringBuilder(Loc.T("UI.Notifications.Diplomacy.GreetingTitle", new object[] { this.activeMission.target.ref_faction.displayName })).ToString();
				if (this.activeMission.ref_faction.WillingToTrade(this.activeMission.target.ref_faction))
				{
					this.factionDiplomacyGreetingBodyText.SetText(Loc.T("UI.Notifications.Diplomacy.Summary", new object[] { this.activeMission.target.ref_faction.displayName }));
				}
				else
				{
					this.factionDiplomacyGreetingBodyText.SetText(Loc.T("UI.Notifications.Diplomacy.SummaryFail", new object[] { this.activeMission.target.ref_faction.displayName }));
				}
				this.factionDiplomacyGreetingHeadlineText.SetText(this.activeMission.target.ref_faction.DiplomacyGreetingMessage(this.activeMission.ref_faction, false));
				string text = new StringBuilder("event:/VO/ENG/Faction/Faction_Diplomacy_").Append(this.activeMission.target.ref_faction.ideology.ideology).Append("_").Append(this.activeMission.target.ref_faction.GetDiplomacyMood(this.activeMission.ref_faction))
					.Append("_")
					.Append(this.activeMission.ref_faction.ideology.ideology)
					.ToString();
				VOController.Instance.AddVOToQueue(text, true);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.activeMission.target.ref_faction.factionIcon256path, this.factionDiplomacyGreetingIconCenter);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.activeMission.target.ref_faction.template.gradientPath, this.factionDiplomacyGreetingGradientLeft);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.activeMission.target.ref_faction.template.gradientPath, this.factionDiplomacyGreetingGradientRight);
				if (this.activeMission.target.ref_faction.IsAlienFaction)
				{
					if (TIPlayerProfileManager.useCouncilorVideo)
					{
						this.factionDiplomacyGreetingVideoPlayer.gameObject.SetActive(true);
						this.factionDiplomacyGreetingLeaderTorsoPortrait.gameObject.SetActive(false);
						this.factionDiplomacyGreetingVideoPlayer.clip = GameControl.assetLoader.LoadAsset<VideoClip>(this.activeMission.target.ref_faction.pathLeaderHeadVideo);
					}
					else
					{
						this.factionDiplomacyGreetingVideoPlayer.gameObject.SetActive(false);
						this.factionDiplomacyGreetingLeaderTorsoPortrait.gameObject.SetActive(true);
						this.factionDiplomacyGreetingLeaderTorsoPortrait.sprite = GameControl.assetLoader.LoadAssetForSpriteAssignment(this.activeMission.target.ref_faction.pathLeaderHeadPortrait);
					}
				}
				else if (TIPlayerProfileManager.useCouncilorVideo)
				{
					this.factionDiplomacyGreetingVideoPlayer.gameObject.SetActive(true);
					this.factionDiplomacyGreetingLeaderTorsoPortrait.gameObject.SetActive(false);
					this.factionDiplomacyGreetingVideoPlayer.clip = GameControl.assetLoader.LoadAsset<VideoClip>(this.activeMission.target.ref_faction.pathLeaderTorsoVideo);
				}
				else
				{
					this.factionDiplomacyGreetingVideoPlayer.gameObject.SetActive(false);
					this.factionDiplomacyGreetingLeaderTorsoPortrait.gameObject.SetActive(true);
					this.factionDiplomacyGreetingLeaderTorsoPortrait.sprite = GameControl.assetLoader.LoadAssetForSpriteAssignment(this.activeMission.target.ref_faction.pathLeaderTorsoPortration);
				}
			}
			else
			{
				this.factionDiplomacyGreetingTitleText.text = new StringBuilder(Loc.T("UI.Notifications.Diplomacy.GreetingTitle", new object[] { this.activeMission.ref_faction.displayName })).ToString();
				this.factionDiplomacyGreetingBodyText.SetText(Loc.T("UI.Notifications.Diplomacy.SummaryAI", new object[] { this.activeMission.ref_faction.displayName }));
				this.factionDiplomacyGreetingHeadlineText.SetText(this.activeMission.ref_faction.DiplomacyGreetingMessage(this.activeMission.target.ref_faction, false));
				string text2 = new StringBuilder("event:/VO/ENG/Faction/Faction_Diplomacy_").Append(this.activeMission.ref_faction.ideology.ideology).Append("_").Append(this.activeMission.ref_faction.GetDiplomacyMood(this.activeMission.target.ref_faction))
					.Append("_")
					.Append(this.activeMission.target.ref_faction.ideology.ideology)
					.ToString();
				VOController.Instance.AddVOToQueue(text2, true);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.activeMission.ref_faction.factionIcon256path, this.factionDiplomacyGreetingIconCenter);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.activeMission.ref_faction.template.gradientPath, this.factionDiplomacyGreetingGradientLeft);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.activeMission.ref_faction.template.gradientPath, this.factionDiplomacyGreetingGradientRight);
				if (TIPlayerProfileManager.useCouncilorVideo)
				{
					this.factionDiplomacyGreetingVideoPlayer.gameObject.SetActive(true);
					this.factionDiplomacyGreetingLeaderTorsoPortrait.gameObject.SetActive(false);
					this.factionDiplomacyGreetingVideoPlayer.clip = GameControl.assetLoader.LoadAsset<VideoClip>(this.activeMission.ref_faction.pathLeaderTorsoVideo);
				}
				else
				{
					this.factionDiplomacyGreetingVideoPlayer.gameObject.SetActive(false);
					this.factionDiplomacyGreetingLeaderTorsoPortrait.gameObject.SetActive(true);
					this.factionDiplomacyGreetingLeaderTorsoPortrait.sprite = GameControl.assetLoader.LoadAssetForSpriteAssignment(this.activeMission.ref_faction.pathLeaderTorsoPortration);
				}
			}
			this.factionDiplomacyGreetingUIObject.SetActive(true);
		}

		// Token: 0x060053EB RID: 21483 RVA: 0x0025F2CC File Offset: 0x0025D4CC
		private void OpenDiplomacyTradeUI()
		{
			this.GetDiplomacyWindowText(this.diplomacyCouncilorState);
			if (TIPlayerProfileManager.useCouncilorVideo)
			{
				this.factionDiplomacyTradePlayerPortraitImage.gameObject.SetActive(false);
				this.factionDiplomacyTradePlayerVideoPlayer.gameObject.SetActive(true);
				this.factionDiplomacyTradePlayerVideoPlayer.clip = GameControl.assetLoader.LoadAsset<VideoClip>(base.activePlayer.pathLeaderHeadVideo);
			}
			else
			{
				this.factionDiplomacyTradePlayerPortraitImage.gameObject.SetActive(true);
				this.factionDiplomacyTradePlayerVideoPlayer.gameObject.SetActive(false);
				this.factionDiplomacyTradePlayerPortraitImage.sprite = GameControl.assetLoader.LoadAssetForSpriteAssignment(base.activePlayer.pathLeaderHeadPortrait);
			}
			if (TIPlayerProfileManager.useCouncilorVideo)
			{
				this.factionDiplomacyTradeOtherPortraitImage.gameObject.SetActive(false);
				this.factionDiplomacyTradeOtherVideoPlayer.gameObject.SetActive(true);
				if (this.aiOffer)
				{
					this.factionDiplomacyTradeOtherVideoPlayer.clip = GameControl.assetLoader.LoadAsset<VideoClip>(this.diploMission.ref_councilor.ref_faction.pathLeaderHeadVideo);
				}
				else
				{
					this.factionDiplomacyTradeOtherVideoPlayer.clip = GameControl.assetLoader.LoadAsset<VideoClip>(this.activeMission.target.ref_faction.pathLeaderHeadVideo);
				}
			}
			else
			{
				this.factionDiplomacyTradeOtherPortraitImage.gameObject.SetActive(true);
				this.factionDiplomacyTradeOtherVideoPlayer.gameObject.SetActive(false);
				if (this.aiOffer)
				{
					this.factionDiplomacyTradeOtherPortraitImage.sprite = GameControl.assetLoader.LoadAssetForSpriteAssignment(this.diploMission.ref_councilor.ref_faction.pathLeaderHeadPortrait);
				}
				else
				{
					this.factionDiplomacyTradeOtherPortraitImage.sprite = GameControl.assetLoader.LoadAssetForSpriteAssignment(this.activeMission.target.ref_faction.pathLeaderHeadPortrait);
				}
			}
			if (this.aiOffer)
			{
				this.diplomacyController.Setup(this.diploMission.ref_faction, this, this.aiOffer);
			}
			else
			{
				this.diplomacyController.Setup(this.activeMission.target.ref_faction, this, this.aiOffer);
			}
			this.factionDiplomacyTradeUIObject.SetActive(true);
			this.interfactionDiplomacyTradeTutorialController.HoldTutorial(CampaignMilestone.UITutorial_InterfactionDiplomacyTrade, false, true);
		}

		// Token: 0x060053EC RID: 21484 RVA: 0x0025F4EC File Offset: 0x0025D6EC
		public void OnDiplomacyGreetingContinueButton()
		{
			if (this.activeMission.ref_faction.WillingToTrade(this.activeMission.target.ref_faction))
			{
				this.OpenDiplomacyTradeUI();
				this.factionDiplomacyGreetingUIObject.SetActive(false);
				return;
			}
			this.DiplomacyClose();
		}

		// Token: 0x060053ED RID: 21485 RVA: 0x0025F529 File Offset: 0x0025D729
		public void OnDiplomacyCloseButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.DiplomacyClose();
		}

		// Token: 0x060053EE RID: 21486 RVA: 0x0025F53D File Offset: 0x0025D73D
		public void DiplomacyClose()
		{
			this.CleanUpDiplomacy();
			this.factionDiplomacyGreetingUIObject.SetActive(false);
			this.factionDiplomacyTradeUIObject.SetActive(false);
			this.interfactionDiplomacyTradeTutorialController.HideTutorial();
			this.diplomacyController.CleanupListeners();
		}

		// Token: 0x060053EF RID: 21487 RVA: 0x0025F574 File Offset: 0x0025D774
		private void CleanUpDiplomacy()
		{
			if (this.aiOffer)
			{
				TIPromptQueueState.RemovePromptStatic(this.contactedFaction, this.diploMission.target.ref_councilor, this.diploMission, "PromptFactionContactMakeOffer", 0);
				this.diploMission.councilor.ref_faction.playerControl.StartAction(new AbortMission(this.diploMission.councilor, true, TIMissionState.AbortReason.VoluntaryAbort, this.diploMission, ""));
			}
			else
			{
				if (this.activeMission.target.ref_faction == base.activePlayer)
				{
					Debug.Log("Diplomacy Cleaunup");
					TIPromptQueueState.RemovePromptStatic(this.activeMission.target.ref_faction, this.activeMission.target, this.activeMission, "PromptFactionContactMakeOffer", 0);
					this.activeMission.councilor.ref_faction.playerControl.StartAction(new AbortMission(this.activeMission.councilor, true, TIMissionState.AbortReason.VoluntaryAbort, this.activeMission, ""));
				}
				TIPromptQueueState.RemovePromptStatic(this.activeMission.councilor.faction, this.activeMission.councilor, this.activeMission, "PromptFactionContactMakeOffer", 0);
				base.activePlayer.playerControl.StartAction(new AbortMission(this.activeMission.councilor, true, TIMissionState.AbortReason.VoluntaryAbort, this.activeMission, ""));
			}
			Debug.Log("Cleaning Diplomacy, resetting");
			this.aiOffer = false;
		}

		// Token: 0x060053F0 RID: 21488 RVA: 0x0025F6E4 File Offset: 0x0025D8E4
		public void CompletedTrade(bool aiOffer)
		{
			TIPromptQueueState.RemovePromptStatic(this.activeMission.councilor.faction, this.activeMission.councilor, this.activeMission, "PromptFactionContactMakeOffer", 0);
			if (aiOffer)
			{
				TIPromptQueueState.RemovePromptStatic(this.contactedFaction, this.diploMission.target.ref_councilor, this.diploMission, "PromptFactionContactMakeOffer", 0);
			}
			this.factionDiplomacyGreetingUIObject.SetActive(false);
			this.factionDiplomacyTradeUIObject.SetActive(false);
			this.interfactionDiplomacyTradeTutorialController.HideTutorial();
			Debug.Log("Player Accepted Trade; Flag:" + aiOffer.ToString());
			this.aiOffer = false;
		}

		// Token: 0x060053F1 RID: 21489 RVA: 0x0025F788 File Offset: 0x0025D988
		private Dictionary<NotificationScreenController.DiplomacyWindowText, string> GetDiplomacyWindowText(TICouncilorState targetCouncilor)
		{
			Dictionary<NotificationScreenController.DiplomacyWindowText, string> dictionary = new Dictionary<NotificationScreenController.DiplomacyWindowText, string>();
			dictionary.Add(NotificationScreenController.DiplomacyWindowText.title, targetCouncilor.faction.displayNameCapitalized + " Contact");
			dictionary.Add(NotificationScreenController.DiplomacyWindowText.headline, "");
			dictionary.Add(NotificationScreenController.DiplomacyWindowText.body, "");
			Dictionary<NotificationScreenController.DiplomacyWindowText, string> dictionary2 = new Dictionary<NotificationScreenController.DiplomacyWindowText, string>(dictionary);
			foreach (NotificationScreenController.DiplomacyWindowText diplomacyWindowText in dictionary.Keys)
			{
				if (dictionary[diplomacyWindowText] == "")
				{
					dictionary2[diplomacyWindowText] = string.Concat(Enumerable.Repeat<string>("Missing Text ", 10));
				}
			}
			return dictionary2;
		}

		// Token: 0x060053F2 RID: 21490 RVA: 0x0025F844 File Offset: 0x0025DA44
		public void ToggleSummaryLogPanel(SummaryCategory defaultCategory = SummaryCategory.None, bool audio = false)
		{
			if (!this.summaryLogReportObject.activeSelf)
			{
				if (audio)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
				}
				this.summaryLogReportObject.SetActive(true);
				if (defaultCategory != SummaryCategory.None)
				{
					this.summaryTabManager.Toggle(this.summaryLogsTabPanes[defaultCategory - SummaryCategory.Missions]);
				}
				(base.canvasManager.StrategyHud as GeneralControlsController).SetSummaryLogReportButton();
				return;
			}
			this.CloseSummaryReportPanel();
		}

		// Token: 0x060053F3 RID: 21491 RVA: 0x0025F8B1 File Offset: 0x0025DAB1
		public void CloseSummaryReportPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.summaryLogReportObject.SetActive(false);
			(base.canvasManager.StrategyHud as GeneralControlsController).SetSummaryLogReportButton();
		}

		// Token: 0x060053F4 RID: 21492 RVA: 0x0025F8E0 File Offset: 0x0025DAE0
		public bool IsSummaryPanelOpen()
		{
			return this.summaryLogReportObject.activeSelf;
		}

		// Token: 0x060053F5 RID: 21493 RVA: 0x0025F8F0 File Offset: 0x0025DAF0
		public void SetSummaryLogReport(SummaryCategory category)
		{
			ListManagerBase listManagerBase = this.summaryLogs[category - SummaryCategory.Missions];
			int num = 0;
			using (IEnumerator<object> enumerator = listManagerBase.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NotificationScreenController.<>o__324.<>p__0 == null)
					{
						NotificationScreenController.<>o__324.<>p__0 = CallSite<Func<CallSite, object, NotificationSummaryItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NotificationSummaryItemController), typeof(NotificationScreenController)));
					}
					NotificationSummaryItemController notificationSummaryItemController = NotificationScreenController.<>o__324.<>p__0.Target(NotificationScreenController.<>o__324.<>p__0, enumerator.Current);
					if (num < this.newsQueue.panelSummaryQueue[category].Count)
					{
						notificationSummaryItemController.Initialize(this.newsQueue.panelSummaryQueue[category][num++]);
						notificationSummaryItemController.UpdateListItem();
						notificationSummaryItemController.gameObject.SetActive(true);
					}
					else
					{
						notificationSummaryItemController.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060053F6 RID: 21494 RVA: 0x0025F9E0 File Offset: 0x0025DBE0
		private void UpdateSummaryLogReport(TIFactionState activePlayer, NotificationSummaryItem item)
		{
			if (item.summaryLogFactions.Contains(activePlayer) && item.template.summaryAudience.category != SummaryCategory.None)
			{
				Transform child = this.summaryLogs[item.template.summaryAudience.category - SummaryCategory.Missions].transform.GetChild(this.maxSummaryListItems[(int)item.template.summaryAudience.category] - 1);
				NotificationSummaryItemController component = child.GetComponent<NotificationSummaryItemController>();
				component.Initialize(item);
				component.UpdateListItem();
				child.SetAsFirstSibling();
				child.gameObject.SetActive(true);
			}
		}

		// Token: 0x060053F7 RID: 21495 RVA: 0x0025FA70 File Offset: 0x0025DC70
		public void LaunchIntroCinematic()
		{
			if (!Application.isEditor && !TemplateManager.global.dontPlayCinematicVideos)
			{
				this.cinematicObject.SetActive(true);
				this.cinematicVideoPlayer.clip = GameControl.assetLoader.LoadAsset<VideoClip>(GameControl.control.activePlayer.cinematicsPath + "_intro");
				this.cinematicVideoPlayer.SetDirectAudioVolume(0, TIPlayerProfileManager.masterVolumeModifier());
				TIUtilities.TryPrepareVideo(this.cinematicVideoPlayer);
				Cinematic2DController component = this.cinematicObject.GetComponent<Cinematic2DController>();
				component.cinematicPathString = GameControl.control.activePlayer.cinematicsPath + "_intro";
				component.audioPath = GameControl.control.activePlayer.ideology.ideology.ToString() + "_Intro";
				component.ideologyString = base.activePlayer.ideology.ideology.ToString().ToLowerInvariant();
				component.StartCoroutine(component.BeginWhenPrepared(true, true));
			}
		}

		// Token: 0x060053F8 RID: 21496 RVA: 0x0025FB79 File Offset: 0x0025DD79
		public IEnumerator PlayVideoWhenPrepared(VideoPlayer videoPlayer)
		{
			while (!videoPlayer.isPrepared)
			{
				yield return null;
			}
			if (videoPlayer.gameObject.activeInHierarchy)
			{
				TIUtilities.TryPlayVideo(videoPlayer);
			}
			yield break;
		}

		// Token: 0x060053F9 RID: 21497 RVA: 0x0025FB88 File Offset: 0x0025DD88
		public void PromptPlayerForBugReport(string message, bool recommendReload)
		{
			Log.Debug("PromptPlayerForBugReport() : \"" + message + "\", " + message.GetHashCode().ToString(), Array.Empty<object>());
		}

		// Token: 0x060053FA RID: 21498 RVA: 0x0025FBBD File Offset: 0x0025DDBD
		public void CopyErrorCodeToClipboard()
		{
			GUIUtility.systemCopyBuffer = this.reportBugBody.text;
		}

		// Token: 0x060053FB RID: 21499 RVA: 0x0025FBCF File Offset: 0x0025DDCF
		public void OpenDiscord()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			TIUtilities.OpenWebURL("https://discord.com/invite/eQaRRq3y3M");
		}

		// Token: 0x060053FC RID: 21500 RVA: 0x0025FBE7 File Offset: 0x0025DDE7
		public void CloseReportBugPrompt()
		{
			this.reportBugPanel.SetActive(false);
		}

		// Token: 0x060053FE RID: 21502 RVA: 0x0025FC44 File Offset: 0x0025DE44
		[CompilerGenerated]
		internal static bool <PushNextAlert>g__PassDelegates|192_0(NotificationQueueItem item)
		{
			if (item.notificationDelegates.Count == 0)
			{
				return true;
			}
			return item.notificationDelegates.All<SpecialNotificationDelegate>((SpecialNotificationDelegate x) => x == SpecialNotificationDelegate.OpenHabManager || x == SpecialNotificationDelegate.None);
		}

		// Token: 0x0400395F RID: 14687
		public ListManagerBase newsList;

		// Token: 0x04003960 RID: 14688
		public Image backgroundImage;

		// Token: 0x04003961 RID: 14689
		public Canvas notificationCanvas;

		// Token: 0x04003962 RID: 14690
		public VerticalLayoutGroup newsFeedLayout;

		// Token: 0x04003963 RID: 14691
		private bool showText;

		// Token: 0x04003964 RID: 14692
		public Button closeAllNewsFeedButton;

		// Token: 0x04003965 RID: 14693
		public RectTransform newsFeedTransform;

		// Token: 0x04003966 RID: 14694
		public Canvas newsFeedCanvas;

		// Token: 0x04003967 RID: 14695
		public Canvas alertPanelCanvas;

		// Token: 0x04003968 RID: 14696
		public GameObject timerPanelObject;

		// Token: 0x04003969 RID: 14697
		public ListManagerBase timerList;

		// Token: 0x0400396A RID: 14698
		public GameObject singleAlertBox;

		// Token: 0x0400396B RID: 14699
		public GameObject singleAlertBoxBody;

		// Token: 0x0400396C RID: 14700
		public Button minimizeSingleAlertBodyButton;

		// Token: 0x0400396D RID: 14701
		public GameObject corePanel;

		// Token: 0x0400396E RID: 14702
		public Image alertLeftImage;

		// Token: 0x0400396F RID: 14703
		public Image alertLeftImageBackground;

		// Token: 0x04003970 RID: 14704
		public Image alertRightImage;

		// Token: 0x04003971 RID: 14705
		public GameObject alertRightImagePanelObject;

		// Token: 0x04003972 RID: 14706
		public Image alertRightAnimatedImage;

		// Token: 0x04003973 RID: 14707
		public ImageAnimator alertRightImageAnimator;

		// Token: 0x04003974 RID: 14708
		public TMP_Text hammerText;

		// Token: 0x04003975 RID: 14709
		public GameObject alertHeadline;

		// Token: 0x04003976 RID: 14710
		public TMP_Text alertHeadlineText;

		// Token: 0x04003977 RID: 14711
		public TMP_Text alertBodyText;

		// Token: 0x04003978 RID: 14712
		public GameObject alertBodyTextScrollObject;

		// Token: 0x04003979 RID: 14713
		public TMP_Text alertBodyScrollText;

		// Token: 0x0400397A RID: 14714
		public GameObject alertBodyTextScrollContent;

		// Token: 0x0400397B RID: 14715
		public ScrollRect alertBodyTextScrollRect;

		// Token: 0x0400397C RID: 14716
		public GameObject okayButtonObject;

		// Token: 0x0400397D RID: 14717
		public Button okayButton;

		// Token: 0x0400397E RID: 14718
		public TMP_Text okayButtonText;

		// Token: 0x0400397F RID: 14719
		public GameObject gotoButtonObject;

		// Token: 0x04003980 RID: 14720
		public Button gotoButton;

		// Token: 0x04003981 RID: 14721
		public TMP_Text gotoButtonText;

		// Token: 0x04003982 RID: 14722
		public GameObject closeButtonObject;

		// Token: 0x04003983 RID: 14723
		public Button closeButton;

		// Token: 0x04003984 RID: 14724
		public TMP_Text closeButtonText;

		// Token: 0x04003985 RID: 14725
		public Button exitButton;

		// Token: 0x04003986 RID: 14726
		public GameObject customDelegatePanelObject;

		// Token: 0x04003987 RID: 14727
		public GameObject customDelegatePanelObject2;

		// Token: 0x04003988 RID: 14728
		public Button[] customDelegateButton;

		// Token: 0x04003989 RID: 14729
		public TMP_Text[] customDelegateButtonText;

		// Token: 0x0400398A RID: 14730
		public Image[] customDelegateButtonSprite;

		// Token: 0x0400398B RID: 14731
		public TooltipTrigger[] customDelegateTooltip;

		// Token: 0x0400398C RID: 14732
		public GameObject customDropdownDelegatePanelObject;

		// Token: 0x0400398D RID: 14733
		public TMP_Dropdown customDelegateDropdown;

		// Token: 0x0400398E RID: 14734
		public Toggle customDelegateDropdownToggle;

		// Token: 0x0400398F RID: 14735
		public TMP_Text customDelegateDropdownToggleText;

		// Token: 0x04003990 RID: 14736
		private int customDropdownDelegateApplyButtonIndex;

		// Token: 0x04003991 RID: 14737
		private TIHabState templateTargetHab;

		// Token: 0x04003992 RID: 14738
		public GameObject controlPointPanel;

		// Token: 0x04003993 RID: 14739
		public GameObject oldControlPointPanel;

		// Token: 0x04003994 RID: 14740
		public TMP_Text oldControlPointTextHeader;

		// Token: 0x04003995 RID: 14741
		public GameObject newControlPointPanel;

		// Token: 0x04003996 RID: 14742
		public TMP_Text newControlPointTextHeader;

		// Token: 0x04003997 RID: 14743
		public Image[] oldControlPointImages;

		// Token: 0x04003998 RID: 14744
		public Image[] newControlPointImages;

		// Token: 0x04003999 RID: 14745
		public GameObject exitButtonObject;

		// Token: 0x0400399A RID: 14746
		public GameObject alertButtonsPanel;

		// Token: 0x0400399B RID: 14747
		public GameObject narrativeEventButtonsPanel;

		// Token: 0x0400399C RID: 14748
		private List<NotificationQueueItem> heldNewsItems;

		// Token: 0x0400399D RID: 14749
		private NotificationQueueItem currentItem;

		// Token: 0x0400399E RID: 14750
		public GameObject illustrationObject;

		// Token: 0x0400399F RID: 14751
		public Image illustration;

		// Token: 0x040039A0 RID: 14752
		public Image illustrationObjectBackgroundImage;

		// Token: 0x040039A1 RID: 14753
		public GameObject maskedIllustrationObject;

		// Token: 0x040039A2 RID: 14754
		public Image maskedIllustrationImage;

		// Token: 0x040039A3 RID: 14755
		public Image leftVideoSmallImage;

		// Token: 0x040039A4 RID: 14756
		public Image leftVideoSmallImageBackground;

		// Token: 0x040039A5 RID: 14757
		public Image rightVideoSmallImage;

		// Token: 0x040039A6 RID: 14758
		public VideoPlayer notificationVideo;

		// Token: 0x040039A7 RID: 14759
		public GameObject notificationCamera;

		// Token: 0x040039A8 RID: 14760
		public GameObject cameraViewObject;

		// Token: 0x040039A9 RID: 14761
		public RawImage cameraRenderTextureImage;

		// Token: 0x040039AA RID: 14762
		[SerializeField]
		private GameObject notificationCameraInstance;

		// Token: 0x040039AB RID: 14763
		[SerializeField]
		private GameObject previewPosition;

		// Token: 0x040039AC RID: 14764
		[SerializeField]
		private GameObject modelInstance;

		// Token: 0x040039AD RID: 14765
		private Vector3 originalPreviewPosition;

		// Token: 0x040039AE RID: 14766
		private Vector3 originalPreviewRotation;

		// Token: 0x040039AF RID: 14767
		[Header("Policy UI")]
		public GameObject masterPolicyPanelObject;

		// Token: 0x040039B0 RID: 14768
		public TMP_Text masterPolicyHeader;

		// Token: 0x040039B1 RID: 14769
		public Image masterPolicyFlag;

		// Token: 0x040039B2 RID: 14770
		public GameObject selectPolicyBodyPanelObject;

		// Token: 0x040039B3 RID: 14771
		public GameObject selectPolicyPanelObject;

		// Token: 0x040039B4 RID: 14772
		public GameObject selectPolicyTargetPanelObject;

		// Token: 0x040039B5 RID: 14773
		public GameObject confirmPanelObject;

		// Token: 0x040039B6 RID: 14774
		public Button minimizePolicyBodyButton;

		// Token: 0x040039B7 RID: 14775
		public TMP_Text confirmPanelText;

		// Token: 0x040039B8 RID: 14776
		public TMP_Text confirmButtonText;

		// Token: 0x040039B9 RID: 14777
		public TMP_Text cancelButtonText;

		// Token: 0x040039BA RID: 14778
		public GameObject backButtonObject;

		// Token: 0x040039BB RID: 14779
		public TMP_Text backButtonText;

		// Token: 0x040039BC RID: 14780
		public GameObject responsePanelObject;

		// Token: 0x040039BD RID: 14781
		public TMP_Text responsePanelNationName;

		// Token: 0x040039BE RID: 14782
		public TMP_Text responsePanelText;

		// Token: 0x040039BF RID: 14783
		public TMP_Text responseConfirmButtonText;

		// Token: 0x040039C0 RID: 14784
		public TMP_Text responseDeclineButtonText;

		// Token: 0x040039C1 RID: 14785
		public ListManagerBase policyOptionsList;

		// Token: 0x040039C2 RID: 14786
		public ListManagerBase policyTargetsList;

		// Token: 0x040039C3 RID: 14787
		[Header("Ally UI")]
		public GameObject callAllyResponseObject;

		// Token: 0x040039C4 RID: 14788
		public TMP_Text callAllyPrompt;

		// Token: 0x040039C5 RID: 14789
		public TMP_Text allyAcceptButtonText;

		// Token: 0x040039C6 RID: 14790
		public TMP_Text allyDeclineButtonText;

		// Token: 0x040039C7 RID: 14791
		private TIPromptQueueState promptQueue;

		// Token: 0x040039C8 RID: 14792
		private TINotificationQueueState newsQueue;

		// Token: 0x040039C9 RID: 14793
		[Header("Remove Army UI")]
		public GameObject removeArmiesPromptObject;

		// Token: 0x040039CA RID: 14794
		public TMP_Text removeArmiesPromptText;

		// Token: 0x040039CB RID: 14795
		public TMP_Text proposeAllianceButtonText;

		// Token: 0x040039CC RID: 14796
		public TMP_Text declareWarButtonText;

		// Token: 0x040039CD RID: 14797
		public TMP_Text sendArmiesHomeButtonText;

		// Token: 0x040039CE RID: 14798
		public Button removeArmies_proposeAllianceButton;

		// Token: 0x040039CF RID: 14799
		public Button removeArmies_declareWarButton;

		// Token: 0x040039D0 RID: 14800
		public Image removeArmies_myNationFlag;

		// Token: 0x040039D1 RID: 14801
		public Image removeArmies_myFactionIcon;

		// Token: 0x040039D2 RID: 14802
		public Image removeArmies_theirNationFlag;

		// Token: 0x040039D3 RID: 14803
		public Image removeArmies_theirFactionIcon;

		// Token: 0x040039D4 RID: 14804
		[Header("Mission Targeting UI")]
		public TMP_Text missionTargetingUIHeaderText;

		// Token: 0x040039D5 RID: 14805
		public GameObject missionTargetingUIObject;

		// Token: 0x040039D6 RID: 14806
		public ListManagerBase missionTargetingUIList;

		// Token: 0x040039D7 RID: 14807
		public TMP_Text missionTargetConfirmButtonText;

		// Token: 0x040039D8 RID: 14808
		public TMP_Text missionTargetText;

		// Token: 0x040039D9 RID: 14809
		public TMP_Text missionTargetCancelButtonText;

		// Token: 0x040039DA RID: 14810
		public Button missionTargetButton;

		// Token: 0x040039DB RID: 14811
		[Header("Faction Diplomacy Greeting UI")]
		public DiplomacyController diplomacyController;

		// Token: 0x040039DC RID: 14812
		public GameObject factionDiplomacyGreetingUIObject;

		// Token: 0x040039DD RID: 14813
		public TMP_Text factionDiplomacyGreetingTitleText;

		// Token: 0x040039DE RID: 14814
		public VideoPlayer factionDiplomacyGreetingVideoPlayer;

		// Token: 0x040039DF RID: 14815
		public Image factionDiplomacyGreetingLeaderTorsoPortrait;

		// Token: 0x040039E0 RID: 14816
		public TMP_Text factionDiplomacyGreetingHeadlineText;

		// Token: 0x040039E1 RID: 14817
		public TMP_Text factionDiplomacyGreetingBodyText;

		// Token: 0x040039E2 RID: 14818
		public Button factionDiplomacyGreetingContinueButton;

		// Token: 0x040039E3 RID: 14819
		public TMP_Text factionDiplomacyGreetingContinueButtonText;

		// Token: 0x040039E4 RID: 14820
		public Image factionDiplomacyFactionIconL;

		// Token: 0x040039E5 RID: 14821
		public Image factionDiplomacyFactionIconR;

		// Token: 0x040039E6 RID: 14822
		public Image factionDiplomacyGreetingIconCenter;

		// Token: 0x040039E7 RID: 14823
		public Image factionDiplomacyGreetingGradientLeft;

		// Token: 0x040039E8 RID: 14824
		public Image factionDiplomacyGreetingGradientRight;

		// Token: 0x040039E9 RID: 14825
		[Header("Faction Diplomacy Trade UI")]
		public GameObject factionDiplomacyTradeUIObject;

		// Token: 0x040039EA RID: 14826
		public GameObject factionDiplomacyTradePlayerObject;

		// Token: 0x040039EB RID: 14827
		public GameObject factionDiplomacyTradeOtherObject;

		// Token: 0x040039EC RID: 14828
		public VideoPlayer factionDiplomacyTradePlayerVideoPlayer;

		// Token: 0x040039ED RID: 14829
		public VideoPlayer factionDiplomacyTradeOtherVideoPlayer;

		// Token: 0x040039EE RID: 14830
		public Image factionDiplomacyTradePlayerPortraitImage;

		// Token: 0x040039EF RID: 14831
		public Image factionDiplomacyTradeOtherPortraitImage;

		// Token: 0x040039F0 RID: 14832
		public Button factionDiplomacyTradeCancelButton;

		// Token: 0x040039F1 RID: 14833
		public Button factionDiplomacyTradeContinueButton;

		// Token: 0x040039F2 RID: 14834
		public bool aiOffer;

		// Token: 0x040039F3 RID: 14835
		public TIMissionState diploMission;

		// Token: 0x040039F4 RID: 14836
		public TIFactionState contactedFaction;

		// Token: 0x040039F5 RID: 14837
		public UITutorialController interfactionDiplomacyTradeTutorialController;

		// Token: 0x040039F6 RID: 14838
		[Header("Cinematics")]
		public GameObject cinematicObject;

		// Token: 0x040039F7 RID: 14839
		public VideoPlayer cinematicVideoPlayer;

		// Token: 0x040039F8 RID: 14840
		[Header("Tutorial")]
		public UITutorialController newsFeedTutorial;

		// Token: 0x040039F9 RID: 14841
		private int _tutorialNewsItemCounter;

		// Token: 0x040039FA RID: 14842
		[Header("ReportBugPrompt")]
		public GameObject reportBugPanel;

		// Token: 0x040039FB RID: 14843
		public TMP_Text reportBugHeader;

		// Token: 0x040039FC RID: 14844
		public TMP_Text reportBugBody;

		// Token: 0x040039FD RID: 14845
		public TMP_Text reportBugCopyErrorCodeButtonText;

		// Token: 0x040039FE RID: 14846
		public TMP_Text reportBugDiscordButtonText;

		// Token: 0x040039FF RID: 14847
		public TMP_Text reportBugContinueButtonText;

		// Token: 0x04003A00 RID: 14848
		[Header("NotificationOptions")]
		public TMP_Text alertOptionHeader;

		// Token: 0x04003A01 RID: 14849
		public TMP_Text newsFeedOptionHeader;

		// Token: 0x04003A02 RID: 14850
		public TMP_Text timerFeedOptionHeader;

		// Token: 0x04003A03 RID: 14851
		public TMP_Text summaryFeedOptionHeader;

		// Token: 0x04003A04 RID: 14852
		public TMP_Text currentNotificationOptionName;

		// Token: 0x04003A05 RID: 14853
		public GameObject notificationOptionsPanel;

		// Token: 0x04003A06 RID: 14854
		public Button openNotificationOptionPanelButton;

		// Token: 0x04003A07 RID: 14855
		public Button altOpenNotificationOptionPanelButton;

		// Token: 0x04003A08 RID: 14856
		public TooltipTrigger currentNotificationSettingTooltip;

		// Token: 0x04003A09 RID: 14857
		public TooltipTrigger altCurrentNotificationSettingTooltip;

		// Token: 0x04003A0A RID: 14858
		public TooltipTrigger alertsNotificationSettingTooltip;

		// Token: 0x04003A0B RID: 14859
		public TooltipTrigger newsFeedNotificationSettingTooltip;

		// Token: 0x04003A0C RID: 14860
		public TooltipTrigger timerFeedNotificationSettingTooltip;

		// Token: 0x04003A0D RID: 14861
		public TooltipTrigger summaryFeedNotificationSettingTooltip;

		// Token: 0x04003A0E RID: 14862
		public NotificationOptionListItem currentNotificationOptionItem;

		// Token: 0x04003A0F RID: 14863
		private EventInstance voEventInstance;

		// Token: 0x04003A11 RID: 14865
		private CurrentNarrativeEventData currentNarrativeEvent;

		// Token: 0x04003A12 RID: 14866
		public float notificationPushTime;

		// Token: 0x04003A13 RID: 14867
		private NotificationScreenController.CustomButtonPressed[] customButtonAction = new NotificationScreenController.CustomButtonPressed[6];

		// Token: 0x04003A14 RID: 14868
		public const int maxCustomNotificationButtons = 6;

		// Token: 0x04003A15 RID: 14869
		private Dictionary<int, string> selectionDropdownDict = new Dictionary<int, string>();

		// Token: 0x04003A16 RID: 14870
		private const int maxNewsListItems = 30;

		// Token: 0x04003A17 RID: 14871
		private const int maxTimerListItems = 6;

		// Token: 0x04003A18 RID: 14872
		private const int maxEventOptions = 4;

		// Token: 0x04003A19 RID: 14873
		public Button[] optionButtons;

		// Token: 0x04003A1A RID: 14874
		public TMP_Text[] optionButtonText;

		// Token: 0x04003A1B RID: 14875
		public TooltipTrigger[] optionButtonDetail;

		// Token: 0x04003A1C RID: 14876
		private TICouncilorState currentPolicyCouncilor;

		// Token: 0x04003A1D RID: 14877
		private TIPolicyOption currentPolicy;

		// Token: 0x04003A1E RID: 14878
		private TINationState currentNation;

		// Token: 0x04003A1F RID: 14879
		private TIGameState currentPolicyTarget;

		// Token: 0x04003A20 RID: 14880
		private Prompt currentPrompt;

		// Token: 0x04003A21 RID: 14881
		private TINationState respondingNation;

		// Token: 0x04003A22 RID: 14882
		private TINationState promptingNation;

		// Token: 0x04003A23 RID: 14883
		private TIGameState relatedGameState;

		// Token: 0x04003A24 RID: 14884
		private TIPolicyOption policyPromptingResponse;

		// Token: 0x04003A25 RID: 14885
		private string promptName;

		// Token: 0x04003A26 RID: 14886
		private TINationState callingNation;

		// Token: 0x04003A27 RID: 14887
		private TINationState calledAlly;

		// Token: 0x04003A28 RID: 14888
		private TIWarState war;

		// Token: 0x04003A29 RID: 14889
		private TINationState nationWithArmies;

		// Token: 0x04003A2A RID: 14890
		private TINationState nationAskingArmiesToLeave;

		// Token: 0x04003A2B RID: 14891
		private TIOrgState targetOrg;

		// Token: 0x04003A2C RID: 14892
		private TIProjectTemplate targetProject;

		// Token: 0x04003A2D RID: 14893
		private TIMissionState activeMission;

		// Token: 0x04003A2E RID: 14894
		private Prompt currentMissionPrompt;

		// Token: 0x04003A2F RID: 14895
		private TIMissionState diplomacyMissionState;

		// Token: 0x04003A30 RID: 14896
		private TICouncilorState diplomacyCouncilorState;

		// Token: 0x04003A31 RID: 14897
		[Header("SummaryLog")]
		public GameObject summaryLogReportObject;

		// Token: 0x04003A32 RID: 14898
		public TMP_Text summaryLogHeaderText;

		// Token: 0x04003A33 RID: 14899
		public List<ListManagerBase> summaryLogs;

		// Token: 0x04003A34 RID: 14900
		public Image factionMissionListIcon;

		// Token: 0x04003A35 RID: 14901
		public TabbedPaneManager summaryTabManager;

		// Token: 0x04003A36 RID: 14902
		public List<TabbedPaneController> summaryLogsTabPanes;

		// Token: 0x04003A37 RID: 14903
		private readonly int[] maxSummaryListItems = new int[] { 0, 30, 30, 30, 30, 120 };

		// Token: 0x04003A38 RID: 14904
		private HashSet<string> seenBugMessages = new HashSet<string>();

		// Token: 0x0200111E RID: 4382
		// (Invoke) Token: 0x060086A9 RID: 34473
		public delegate void CustomButtonPressed();

		// Token: 0x0200111F RID: 4383
		private enum DiplomacyWindowText
		{
			// Token: 0x0400667D RID: 26237
			title,
			// Token: 0x0400667E RID: 26238
			headline,
			// Token: 0x0400667F RID: 26239
			body
		}
	}
}
