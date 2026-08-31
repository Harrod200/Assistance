using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000834 RID: 2100
	public class CouncilorGridItemController : MonoBehaviour
	{
		// Token: 0x17000EA2 RID: 3746
		// (get) Token: 0x06004C12 RID: 19474 RVA: 0x001FF751 File Offset: 0x001FD951
		// (set) Token: 0x06004C13 RID: 19475 RVA: 0x001FF759 File Offset: 0x001FD959
		public TICouncilorState councilor { get; private set; }

		// Token: 0x17000EA3 RID: 3747
		// (get) Token: 0x06004C14 RID: 19476 RVA: 0x001FF762 File Offset: 0x001FD962
		// (set) Token: 0x06004C15 RID: 19477 RVA: 0x001FF76A File Offset: 0x001FD96A
		public int index { get; private set; }

		// Token: 0x06004C16 RID: 19478 RVA: 0x001FF774 File Offset: 0x001FD974
		public void Awake()
		{
			this.backgroundImageInitialPosition = this.backgroundImage.rectTransform.localPosition;
			this.persuasionTitle.SetText(Loc.T("UI.Global.PersuasionShort"));
			this.investigationTitle.SetText(Loc.T("UI.Global.InvestigationShort"));
			this.espionageTitle.SetText(Loc.T("UI.Global.EspionageShort"));
			this.commandTitle.SetText(Loc.T("UI.Global.CommandShort"));
			this.administrationTitle.SetText(Loc.T("UI.Global.AdministrationShort"));
			this.scienceTitle.SetText(Loc.T("UI.Global.ScienceShort"));
			this.securityTitle.SetText(Loc.T("UI.Global.SecurityShort"));
			this.apparentLoyaltyTitle.SetText(Loc.T("UI.Global.LoyaltyShort"));
		}

		// Token: 0x06004C17 RID: 19479 RVA: 0x001FF840 File Offset: 0x001FDA40
		public void Init(CouncilGridController controller, int index)
		{
			this.controller = controller;
			this.index = index;
			this.XPText.SetText(Loc.T("UI.Councilor.XPText"));
			this.trackingMeTip.SetDelegate("BodyText", () => Loc.T("UI.Councilor.TrackingMeTip", new object[] { TIFactionState.goToGroundMission.displayName }));
		}

		// Token: 0x06004C18 RID: 19480 RVA: 0x001FF89F File Offset: 0x001FDA9F
		public void ItemSelected()
		{
			this.controller.OnClickCouncilGridItem(this.councilor);
		}

		// Token: 0x06004C19 RID: 19481 RVA: 0x001FF8B4 File Offset: 0x001FDAB4
		public void OnClickGotoButton()
		{
			this.controller.CloseInfoScreen(false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyCouncilorSelect", false, false);
			TIUtilities.GotoGameState(this.councilor, true, true, true);
			GameControl.eventManager.TriggerEvent(new CouncilorSelectedOffMap(this.councilor), null, new object[] { this.councilor.ref_region });
		}

		// Token: 0x06004C1A RID: 19482 RVA: 0x001FF914 File Offset: 0x001FDB14
		public void UpdateListItem(TICouncilorState councilor, bool forceRefresh = false)
		{
			bool flag = forceRefresh;
			if (this.councilor != councilor)
			{
				flag = true;
				this.councilor = councilor;
			}
			if (councilor == null)
			{
				Error.Log("Bad councilor state passed to council grid item controller", Array.Empty<object>());
				return;
			}
			if (TIPlayerProfileManager.useCouncilorVideo)
			{
				if (this.councilorVideo.clip == null || flag)
				{
					this.councilorVideo.gameObject.SetActive(true);
					string text = string.Join(" ", new string[]
					{
						"charactervideos/Video Render Texture - Grid Spot",
						(this.index + 1).ToString()
					});
					this.councilorVideoTexture.texture = GameControl.assetLoader.LoadAsset<Texture>(text);
					VideoClip videoClip = GameControl.assetLoader.LoadAsset<VideoClip>(councilor.videoResource);
					if (this.councilorVideo.clip != videoClip)
					{
						this.councilorVideo.clip = videoClip;
						int num = (int)Math.Min(this.councilorVideo.frameCount, 2147483647UL);
						int num2 = TIUtilities.RandomRange(0, num);
						this.councilorVideo.frame = (long)num2;
					}
					this.councilorVideo.playOnAwake = false;
					this.councilorVideo.waitForFirstFrame = false;
					this.councilorVideo.targetTexture = (RenderTexture)this.councilorVideoTexture.texture;
					this.councilorStillImage.sprite = null;
					this.councilorStillImage.enabled = false;
				}
			}
			else if (this.councilorStillImage.sprite == null || flag)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(councilor.portraitResource, this.councilorStillImage);
				this.councilorStillImage.enabled = true;
				this.councilorVideo.Stop();
				this.councilorVideo.clip = null;
				this.councilorVideo.gameObject.SetActive(false);
			}
			CouncilorView viewofCouncilor = this.controller.activePlayer.GetViewofCouncilor(councilor);
			this.councilorName.text = councilor.displayName;
			this.councilorProfession.text = councilor.jobDisplayName;
			this.councilorMission.text = viewofCouncilor.GetCurrentMissionString(false, false, false);
			CouncilorIllustrationData illustrationData = councilor.GetIllustrationData();
			GameControl.assetLoader.LoadAssetForImageAssignment(illustrationData.illustrationPath, this.backgroundImage);
			(this.backgroundImage.transform as RectTransform).anchoredPosition = new Vector2(illustrationData.GetIllustrationLocalPosition(this.backgroundImage, this.backgroundImageInitialPosition).x, 0f);
			this.backgroundMask.enabled = true;
			this.councilorLocation.text = TIUtilities.GetLocationString(councilor.location, true, false);
			this.councilorHomeNationFlag.sprite = councilor.homeNation.flag;
			this.persuasion.text = councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false).ToString();
			this.investigation.text = councilor.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false).ToString();
			this.espionage.text = councilor.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false).ToString();
			this.command.text = councilor.GetAttribute(CouncilorAttribute.Command, true, true, true, false, false, false).ToString();
			this.administration.text = councilor.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false).ToString();
			this.science.text = councilor.GetAttribute(CouncilorAttribute.Science, true, true, true, false, false, false).ToString();
			this.security.text = councilor.GetAttribute(CouncilorAttribute.Security, true, true, true, false, false, false).ToString();
			this.apparentLoyalty.text = viewofCouncilor.GetAttributeString(CouncilorAttribute.Loyalty);
			this.loyaltyTip.SetDelegate("BodyText", () => CouncilorMissionCanvasController.LoyaltyTip(this.controller.activePlayer, councilor));
			this.moneyIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Money), 1, 0, true, false));
			this.influenceIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Influence), 1, 0, true, false));
			this.opsIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Operations), 1, 0, true, false));
			this.researchIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Research), 1, 0, true, false));
			this.boostIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Boost), 1, 0, true, false));
			this.mCIncome.text = councilor.GetMonthlyIncome(FactionResource.MissionControl).ToString("N0");
			this.projects.text = councilor.projectContributionString;
			if (councilor.faction == GameControl.control.activePlayer)
			{
				if (councilor.CanAffordAnyCandidateAugmentations(true))
				{
					if ((float)councilor.XP >= (float)TemplateManager.global.XPToLevelUp * (1f + councilor.XPModifier))
					{
						this.XPValue.SetText(Loc.T("UI.Councilor.XP", new object[] { TIUtilities.GreenLine(councilor.XP.ToString("N0")) }));
					}
					else
					{
						this.XPValue.SetText(Loc.T("UI.Councilor.XP", new object[] { TIUtilities.YellowLine(councilor.XP.ToString("N0")) }));
					}
				}
				else
				{
					this.XPValue.SetText(Loc.T("UI.Councilor.XP", new object[] { TIUtilities.RedLine(councilor.XP.ToString("N0")) }));
				}
			}
			else
			{
				this.XPValue.SetText(Loc.T("UI.Councilor.XP", new object[] { TIUtilities.HeaderCyanLine(councilor.XP.ToString("N0")) }));
			}
			if (councilor.knowsIveBeenSeenBy.Count > 0)
			{
				this.trackingMeList.SetListSize<FactionIconGridItemController>(councilor.knowsIveBeenSeenBy.Count, false, false);
				int num3 = 0;
				using (IEnumerator<object> enumerator = this.trackingMeList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CouncilorGridItemController.<>o__67.<>p__0 == null)
						{
							CouncilorGridItemController.<>o__67.<>p__0 = CallSite<Func<CallSite, object, FactionIconGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionIconGridItemController), typeof(CouncilorGridItemController)));
						}
						CouncilorGridItemController.<>o__67.<>p__0.Target(CouncilorGridItemController.<>o__67.<>p__0, enumerator.Current).SetListItem(councilor.knowsIveBeenSeenBy[num3++]);
					}
				}
				this.trackingMePanel.SetActive(true);
			}
			else
			{
				this.trackingMePanel.SetActive(false);
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (councilor.detained || viewofCouncilor.turned)
			{
				if (councilor.detained)
				{
					if (councilor.detainingFaction == councilor.faction)
					{
						stringBuilder.Append(Loc.T("UI.Councilor.SelfDetainedTooltip", new object[] { councilor.detainedReleaseDate.ToCustomDateString() })).AppendLine().AppendLine()
							.AppendLine(Loc.T("UI.Councilor.AddressSelfDetained"))
							.ToString();
					}
					else
					{
						stringBuilder.Append(Loc.T("UI.Councilor.DetainedTooltip", new object[]
						{
							councilor.detainingFaction.displayNameWithColor,
							councilor.detainedReleaseDate.ToCustomDateString()
						}));
						if (councilor.detainingFaction != this.controller.activePlayer)
						{
							stringBuilder.AppendLine().AppendLine().AppendLine(Loc.T("UI.Councilor.AddressDetained"))
								.ToString();
						}
					}
					if (!viewofCouncilor.turned)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment("councilor_missions/ICO_detain_off", this.statusIcon);
						this.statusText.SetText(Loc.T("UI.Councilor.Detained"));
					}
				}
				if (viewofCouncilor.turned)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment("councilor_missions/ICO_turn_off", this.statusIcon);
					if (councilor.agentForFaction != this.controller.activePlayer)
					{
						this.statusText.SetText(Loc.T("UI.Councilor.Traitor"));
						this.turnedEnemyCouncilorFailurePanel.SetActive(false);
						this.factionLoyaltyIcon.sprite = councilor.agentForFaction.factionIcon128UI;
					}
					else
					{
						this.statusText.SetText(Loc.T("UI.Councilor.Turned"));
						this.turnedEnemyCouncilorFailurePanel.SetActive(true);
						this.factionLoyaltyIcon.sprite = councilor.faction.factionIcon128UI;
						this.turnedEnemyCouncilorSlider.value = councilor.autofailMissionsValue * 100f;
						this.SetAutoFailText();
						stringBuilder.AppendLine();
					}
					stringBuilder.AppendLine().Append(Loc.T("UI.Councilor.TurnedTooltip", new object[]
					{
						councilor.faction.displayName,
						viewofCouncilor.agentForFaction.displayNameWithColor
					})).Append(" ")
						.AppendLine(Loc.T("UI.Councilor.AddressTraitor"));
				}
				else
				{
					this.turnedEnemyCouncilorFailurePanel.SetActive(false);
					this.factionLoyaltyIcon.sprite = councilor.faction.factionIcon128UI;
				}
				this.statusTooltip1.SetText("BodyText", stringBuilder.ToString());
				this.statusTooltip2.SetText("BodyText", stringBuilder.ToString());
				this.statusText.gameObject.SetActive(true);
				this.statusTextContainer.SetActive(true);
				this.statusIcon.gameObject.SetActive(true);
			}
			else
			{
				this.factionLoyaltyIcon.sprite = councilor.faction.factionIcon128UI;
				this.statusText.gameObject.SetActive(false);
				this.statusTextContainer.SetActive(false);
				this.statusIcon.gameObject.SetActive(false);
				this.turnedEnemyCouncilorFailurePanel.SetActive(false);
			}
			if (councilor.detained)
			{
				this.councilorAdviceButton.interactable = false;
				return;
			}
			if (GameControl.control.activePlayer.turnedCouncilors.Contains(councilor))
			{
				this.councilorAdviceButton.gameObject.SetActive(false);
				return;
			}
			this.councilorAdviceButton.interactable = true;
			this.councilorAdviceButton.gameObject.SetActive(true);
		}

		// Token: 0x06004C1B RID: 19483 RVA: 0x00200400 File Offset: 0x001FE600
		private IEnumerator StartVideo()
		{
			WaitForSeconds waitForSeconds = new WaitForSeconds(5f);
			if (!this.councilorVideo.isPrepared)
			{
				yield return waitForSeconds;
			}
			TIUtilities.TryPlayVideo(this.councilorVideo);
			yield break;
		}

		// Token: 0x06004C1C RID: 19484 RVA: 0x0020040F File Offset: 0x001FE60F
		public void OnAdviceButtonClicked()
		{
			if (!this.councilorAdvicePanel.activeInHierarchy)
			{
				this.controller.GenerateAdviceForAllPanels();
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenFinder", false, false);
				return;
			}
			this.controller.CloseAllAdvicePanels();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseFinder", false, false);
		}

		// Token: 0x06004C1D RID: 19485 RVA: 0x00200450 File Offset: 0x001FE650
		public void OnAdvanceAdviceButtonClicked()
		{
			int count = this.controller.generatedAdvice[this.councilor].Count;
			this.adviceIdx++;
			if (this.adviceIdx >= count)
			{
				this.adviceIdx = 0;
			}
			this.adviceText.SetText(this.controller.generatedAdvice[this.councilor][this.adviceIdx].adviceText);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}

		// Token: 0x06004C1E RID: 19486 RVA: 0x002004D4 File Offset: 0x001FE6D4
		public void SetAutoFailText()
		{
			this.turnedEnemyCouncilorFailureText.SetText(Loc.T("UI.Councilor.TurnedSlider", new object[] { this.councilor.autofailMissionsValue.ToPercent("P0") }));
		}

		// Token: 0x06004C1F RID: 19487 RVA: 0x00200509 File Offset: 0x001FE709
		public void OnTurnedSliderChangedValue()
		{
			this.controller.activePlayer.playerControl.StartAction(new SetAutofailValueForTurnedCouncilorAction(this.councilor, this.turnedEnemyCouncilorSlider.value / 100f));
			this.SetAutoFailText();
		}

		// Token: 0x04002D81 RID: 11649
		public CouncilGridController controller;

		// Token: 0x04002D82 RID: 11650
		public GameObject primaryPanel;

		// Token: 0x04002D83 RID: 11651
		public TMP_Text councilorName;

		// Token: 0x04002D84 RID: 11652
		public TMP_Text councilorProfession;

		// Token: 0x04002D85 RID: 11653
		public TMP_Text councilorLocation;

		// Token: 0x04002D86 RID: 11654
		public TMP_Text councilorMission;

		// Token: 0x04002D87 RID: 11655
		public Image councilorHomeNationFlag;

		// Token: 0x04002D88 RID: 11656
		public Image backgroundImage;

		// Token: 0x04002D89 RID: 11657
		public Image councilorStillImage;

		// Token: 0x04002D8A RID: 11658
		private Vector3 backgroundImageInitialPosition;

		// Token: 0x04002D8B RID: 11659
		public RawImage councilorVideoTexture;

		// Token: 0x04002D8C RID: 11660
		public VideoPlayer councilorVideo;

		// Token: 0x04002D8D RID: 11661
		public RectMask2D backgroundMask;

		// Token: 0x04002D8E RID: 11662
		public TMP_Text persuasion;

		// Token: 0x04002D8F RID: 11663
		public TMP_Text investigation;

		// Token: 0x04002D90 RID: 11664
		public TMP_Text espionage;

		// Token: 0x04002D91 RID: 11665
		public TMP_Text command;

		// Token: 0x04002D92 RID: 11666
		public TMP_Text administration;

		// Token: 0x04002D93 RID: 11667
		public TMP_Text science;

		// Token: 0x04002D94 RID: 11668
		public TMP_Text security;

		// Token: 0x04002D95 RID: 11669
		public TMP_Text apparentLoyalty;

		// Token: 0x04002D96 RID: 11670
		public TMP_Text persuasionTitle;

		// Token: 0x04002D97 RID: 11671
		public TMP_Text investigationTitle;

		// Token: 0x04002D98 RID: 11672
		public TMP_Text espionageTitle;

		// Token: 0x04002D99 RID: 11673
		public TMP_Text commandTitle;

		// Token: 0x04002D9A RID: 11674
		public TMP_Text administrationTitle;

		// Token: 0x04002D9B RID: 11675
		public TMP_Text scienceTitle;

		// Token: 0x04002D9C RID: 11676
		public TMP_Text securityTitle;

		// Token: 0x04002D9D RID: 11677
		public TMP_Text apparentLoyaltyTitle;

		// Token: 0x04002D9E RID: 11678
		public TooltipTrigger loyaltyTip;

		// Token: 0x04002D9F RID: 11679
		public TMP_Text moneyIncome;

		// Token: 0x04002DA0 RID: 11680
		public TMP_Text influenceIncome;

		// Token: 0x04002DA1 RID: 11681
		public TMP_Text opsIncome;

		// Token: 0x04002DA2 RID: 11682
		public TMP_Text researchIncome;

		// Token: 0x04002DA3 RID: 11683
		public TMP_Text boostIncome;

		// Token: 0x04002DA4 RID: 11684
		public TMP_Text mCIncome;

		// Token: 0x04002DA5 RID: 11685
		public TMP_Text projects;

		// Token: 0x04002DA6 RID: 11686
		public TMP_Text XPText;

		// Token: 0x04002DA7 RID: 11687
		public TMP_Text XPValue;

		// Token: 0x04002DA8 RID: 11688
		public Image statusIcon;

		// Token: 0x04002DA9 RID: 11689
		public TMP_Text statusText;

		// Token: 0x04002DAA RID: 11690
		public GameObject statusTextContainer;

		// Token: 0x04002DAB RID: 11691
		public TooltipTrigger statusTooltip1;

		// Token: 0x04002DAC RID: 11692
		public TooltipTrigger statusTooltip2;

		// Token: 0x04002DAD RID: 11693
		public GameObject turnedEnemyCouncilorFailurePanel;

		// Token: 0x04002DAE RID: 11694
		public TMP_Text turnedEnemyCouncilorFailureText;

		// Token: 0x04002DAF RID: 11695
		public Slider turnedEnemyCouncilorSlider;

		// Token: 0x04002DB0 RID: 11696
		public Image factionLoyaltyIcon;

		// Token: 0x04002DB1 RID: 11697
		public GameObject trackingMePanel;

		// Token: 0x04002DB2 RID: 11698
		public ListManagerBase trackingMeList;

		// Token: 0x04002DB3 RID: 11699
		public TooltipTrigger trackingMeTip;

		// Token: 0x04002DB4 RID: 11700
		public Button councilorAdviceButton;

		// Token: 0x04002DB5 RID: 11701
		public GameObject councilorAdvicePanel;

		// Token: 0x04002DB6 RID: 11702
		public Button advanceAdviceButton;

		// Token: 0x04002DB7 RID: 11703
		public TMP_Text adviceText;

		// Token: 0x04002DBA RID: 11706
		public int adviceIdx;
	}
}
