using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Entities;
using PavonisInteractive.TerraInvicta.Systems.UI;
using PavonisInteractive.TerraInvicta.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000831 RID: 2097
	public class CouncilGridController : CanvasControllerBase, IInfoScreen, ICanvas
	{
		// Token: 0x06004B56 RID: 19286 RVA: 0x001F7464 File Offset: 0x001F5664
		public override void Initialize()
		{
			base.Initialize();
			this.councilGridCanvas.gameObject.SetActive(true);
			this.councilGridCanvas.enabled = false;
			this.councilorSingleCanvas.gameObject.SetActive(true);
			this.councilorSingleGameObject.SetActive(true);
			this.councilorSingleCanvas.enabled = false;
			this.recruitingCanvas.gameObject.SetActive(true);
			this.orgManagementCanvas.gameObject.SetActive(true);
			this.councilTabButtonText.SetText(Loc.T("UI.Council.CouncilTab"));
			this.recruitTabButtonText.SetText(Loc.T("UI.Council.RecruitingHeader"));
			this.ledgerTabButtonText.SetText(Loc.T("UI.Council.LedgerTab"));
			this.orgManagementTabButtonText.SetText(Loc.T("UI.Council.OrgManagementTitle"));
			this.turnedSlotNotice1.SetText(Loc.T("UI.Council.ReservedForTurned"));
			this.turnedSlotNotice2.SetText(Loc.T("UI.Council.ReservedForTurned"));
			GameControl.eventManager.AddListener<CouncilorDetailRequested>(new EventManager.EventDelegate<CouncilorDetailRequested>(this.UpdateSingleCouncilorInfo), null, null, true, false);
			this.eventManager = GameControl.eventManager;
			this.sb = new StringBuilder();
			this.recruitMissionHeaderText.SetText(Loc.T("UI.Councilor.Missions"));
			this.recruitAttributesHeaderText.SetText(Loc.T("UI.Councilor.Attributes"));
			this.recruitTraitsHeaderText.SetText(Loc.T("UI.Councilor.Traits"));
			this.recruitIncomesHeaderText.SetText(Loc.T("UI.Councilor.Incomes"));
			this.recruitCandidateButtonText.SetText(Loc.T("UI.Council.RecruitCandidateButton"));
			this.candidateAgeTitle.SetText(Loc.T("UI.Councilor.AgeTitle"));
			this.candidateHomeRegionTitle.SetText(Loc.T("UI.Councilor.HometownTitle"));
			this.candidateLocationTitle.SetText(Loc.T("UI.Councilor.CandidateLocation"));
			this.recruitCostTitle.SetText(Loc.T("TIResourceCost.Cost", new object[] { string.Empty }));
			this.confirmRecruitConfirmButtonText.SetText(Loc.T("UI.Councilor.SpendXPConfirm"));
			this.confirmRecruitDeclineButtonText.SetText(Loc.T("UI.Councilor.Orgs.CancelButton"));
			this.attributesHeaderText.SetText(Loc.T("UI.Councilor.Attributes"));
			this.incomesHeaderText.SetText(Loc.T("UI.Councilor.Incomes"));
			this.missionsHeaderText.SetText(Loc.T("UI.Councilor.Missions"));
			this.factionOrgsHeaderText.SetText(Loc.T("UI.Councilor.OrgsGarage"));
			this.marketOrgsHeaderText.SetText(Loc.T("UI.Councilor.OrgsMarket"));
			this.persuasionText.SetText(Loc.T("UI.Global.Persuasion"));
			this.candidatePersuasionText.SetText(Loc.T("UI.Global.Persuasion"));
			this.investigationText.SetText(Loc.T("UI.Global.Investigation"));
			this.candidateInvestigationText.SetText(Loc.T("UI.Global.Investigation"));
			this.espionageText.SetText(Loc.T("UI.Global.Espionage"));
			this.candidateEspionageText.SetText(Loc.T("UI.Global.Espionage"));
			this.commandText.SetText(Loc.T("UI.Global.Command"));
			this.candidateCommandText.SetText(Loc.T("UI.Global.Command"));
			this.administrationText.SetText(Loc.T("UI.Global.Administration"));
			this.candidateAdministrationText.SetText(Loc.T("UI.Global.Administration"));
			this.scienceText.SetText(Loc.T("UI.Global.Science"));
			this.candidateScienceText.SetText(Loc.T("UI.Global.Science"));
			this.securityText.SetText(Loc.T("UI.Global.Security"));
			this.candidateSecurityText.SetText(Loc.T("UI.Global.Security"));
			this.candidateApparentLoyaltyText.SetText(Loc.T("UI.Global.ApparentLoyalty"));
			this.candidatePersuasionTooltip.SetText("BodyText", Loc.T("UI.Councilor.PersuasionTip"));
			this.candidateInvestigationTooltip.SetText("BodyText", Loc.T("UI.Councilor.InvestigationTip"));
			this.candidateEspionageTooltip.SetText("BodyText", Loc.T("UI.Councilor.EspionageTip"));
			this.candidateCommandTooltip.SetText("BodyText", Loc.T("UI.Councilor.CommandTip"));
			this.candidateAdministrationTooltip.SetText("BodyText", Loc.T("UI.Councilor.AdministrationTip"));
			this.candidateScienceTooltip.SetText("BodyText", Loc.T("UI.Councilor.ScienceTip"));
			this.candidateSecurityTooltip.SetText("BodyText", Loc.T("UI.Councilor.SecurityTip"));
			this.loyaltyTooltip.SetText("BodyText", Loc.T("UI.Councilor.LoyaltyTip"));
			this.candidateLoyaltyTooltip.SetText("BodyText", Loc.T("UI.Councilor.LoyaltyTip"));
			this.councilGridCanvas.enabled = false;
			this.recruitingCanvas.enabled = false;
			this.councilorSingleCanvas.enabled = false;
			this.candidateDetailCanvas.enabled = false;
			this.SelectCandidateListItem(null);
			this.selectCandidateCanvas.enabled = true;
			this.confirmMovePanel.SetActive(false);
			this.missionListObject.SetActive(true);
			this.spendXPButton.interactable = !this.lookingAtTurnedCouncilor && !this.spendXPPanel.activeSelf;
			this.candidateBackgroundImageInitialPosition = this.candidateBackgroundImage.rectTransform.localPosition;
			this.councilorBackgroundImageInitialPosition = this.councilorBackgroundImage.rectTransform.localPosition;
			this.orgsTooltip.SetText("BodyText", Loc.T("UI.Councilor.OrgsTooltip", new object[] { TemplateManager.global.councilorMaxOrgs }));
			this.orgMarketplaceTooltip.SetText("BodyText", Loc.T("UI.Councilor.OrgMarketplaceTooltip"));
			this.unassignedOrgsTooltip.SetText("BodyText", Loc.T("UI.Councilor.UnassignedOrgsTooltip"));
			this.oKText.SetText(Loc.T("UI.Councilor.Orgs.AcknowledgeButton"));
			this.cancelText.SetText(Loc.T("UI.Councilor.Orgs.CancelButton"));
			this.confirmFailOKText.SetText(Loc.T("UI.Councilor.Orgs.AcknowledgeButton"));
			this.dismissCancelText.SetText(Loc.T("UI.Councilor.Orgs.CancelButton"));
			this.moneyText.SetText(Loc.T("UI.Global.Money"));
			this.influenceText.SetText(Loc.T("UI.Global.Influence"));
			this.opsText.SetText(Loc.T("UI.Global.Operations"));
			this.researchText.SetText(Loc.T("UI.Global.Research"));
			this.boostText.SetText(Loc.T("UI.Global.Boost"));
			this.MCText.SetText(Loc.T("UI.Global.MissionControl"));
			this.projectsText.SetText(Loc.T("UI.Global.Projects"));
			this.candidatesText.SetText(Loc.T("UI.Council.Candidates"));
			this.selectCandidateWarningText.SetText(Loc.T("UI.Council.SelectCandidate"));
			this.candidateMoneyText.SetText(Loc.T("UI.Global.Money"));
			this.candidateInfluenceText.SetText(Loc.T("UI.Global.Influence"));
			this.candidateOpsText.SetText(Loc.T("UI.Global.Operations"));
			this.candidateResearchText.SetText(Loc.T("UI.Global.Research"));
			this.candidateBoostText.SetText(Loc.T("UI.Global.Boost"));
			this.candidateMissionControlText.SetText(Loc.T("UI.Global.MissionControl"));
			this.candidateProjectsText.SetText(Loc.T("UI.Global.Projects"));
			this.XPTip.SetText("BodyText", Loc.T("UI.Councilor.XPTooltip"));
			this.customizeButtonText.SetText(Loc.T("UI.Councilor.CustomizeButton"));
			this.givenNameText.SetText(Loc.T("UI.Councilor.GivenName"));
			this.familyNameText.SetText(Loc.T("UI.Councilor.FamilyName"));
			this.confirmChangeBioText.SetText(Loc.T("UI.Councilor.ConfirmChanges"));
			this.cancelChangeBioText.SetText(Loc.T("UI.Councilor.AbortCancelButtonText"));
			this.automateMissionsToggleText.SetText(Loc.T("UI.Councilor.AutomateSettings"));
			this.councilorMissionTitle.SetText(Loc.T("UI.Councilor.StatusTitle"));
			this.councilorCurrentLocationTitle.SetText(Loc.T("UI.Councilor.LocationTitle"));
			this.councilorHomeRegionTitle.SetText(Loc.T("UI.Councilor.HometownTitle"));
			this.councilorAgeTitle.SetText(Loc.T("UI.Councilor.AgeTitle"));
			this.XPTitle.SetText(Loc.T("UI.Councilor.XPText"));
			this.infoMyOrgHeader.SetText(Loc.T("UI.Councilor.AssignedOrg"));
			this.infoMyOrgOwned.SetText(Loc.T("UI.Councilor.Owned"));
			DragDestination[] componentsInChildren = base.GetComponentsInChildren<DragDestination>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetControllerBase(this);
			}
			this.spendXPButtonText.SetText(Loc.T("UI.Councilor.SpendXPButton"));
			this.closeSpendXPButtonText.SetText(Loc.T("UI.Councilor.CloseXPMenu"));
			this.spendXPPanelHeaderText.SetText(Loc.T("UI.Councilor.SpendXPHeader"));
			this.confirmSpendXPSelectionButtonText.SetText(Loc.T("UI.Councilor.SpendXPConfirm"));
			this.cancelSpendXPSelectionButtonText.SetText(Loc.T("UI.Councilor.SpendXPDecline"));
			this.customizeCouncilorPanel.SetActive(false);
			this.customizeCouncilorHeaderText.SetText(Loc.T("UI.Councilor.CustomizeHeader"));
			if (!Application.isEditor)
			{
				Log.Time("<color=#00cc00>LoadTime:</color> Initialize Councilor Customization", delegate
				{
					this.CacheCouncilorPortraits();
				}, true, true);
			}
			this.ancestryFilterHeader.SetText(Loc.T("UI.Councilor.CustomizeAncestry"));
			this.genderFilterHeader.SetText(Loc.T("UI.Councilor.CustomizeGender"));
			this.jobFilterHeader.SetText(Loc.T("UI.Councilor.CustomizeProfession"));
			this.duplicateFilterHeader.SetText(Loc.T("UI.Councilor.CustomizeInUse"));
			this.voiceAccentFilterHeader.SetText(Loc.T("UI.Councilor.CustomizeAccent"));
			this.voiceIndexSelectorHeader.SetText(Loc.T("UI.Councilor.CustomizeVoiceIndex"));
			this.size5Project = TemplateManager.Find<TIProjectTemplate>(TemplateManager.global.size5Project, false);
			this.size6Project = TemplateManager.Find<TIProjectTemplate>(TemplateManager.global.size6Project, false);
			this.councilSize5Notice.SetText(Loc.T("UI.Council.CouncilSize5", new object[] { this.size5Project.displayName }));
			this.councilSize6Notice.SetText(Loc.T("UI.Council.CouncilSize6", new object[] { this.size6Project.displayName }));
			this.InitializeLedgerCanvas();
			this.InitializeCalendarCanvas();
			this.revertOrgChangesButtonText.SetText(Loc.T("UI.Objectives.Orgman.Revert.Name"));
			this.confirmOrgChangesButtonText.SetText(Loc.T("UI.Objectives.Orgman.Confirm.Name"));
			this.UpdateActivePlayerUIElements(true);
		}

		// Token: 0x06004B57 RID: 19287 RVA: 0x001F7EF4 File Offset: 0x001F60F4
		public override void UpdateActivePlayerUIElements(bool startup)
		{
			Image[] backgroundImage = this.BackgroundImage;
			for (int i = 0; i < backgroundImage.Length; i++)
			{
				backgroundImage[i].sprite = GameControl.control.activePlayer.factionIcon256UI;
			}
		}

		// Token: 0x06004B58 RID: 19288 RVA: 0x001F7F2D File Offset: 0x001F612D
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.councilTabsManager.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, (float)((base.VerticalScaleValueLimit() >= 940f) ? (-100) : (-85)));
		}

		// Token: 0x06004B59 RID: 19289 RVA: 0x001F7F68 File Offset: 0x001F6168
		public override void Show()
		{
			base.Show();
			this.eventManager.AddListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateCouncilGrid), null, null, true, false);
			this.eventManager.AddListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.UpdateCouncilScreenForValueChange), null, null, true, false);
			this.eventManager.AddListener<CouncilOrgsChanged>(new EventManager.EventDelegate<CouncilOrgsChanged>(this.UpdateCouncilorScreenForMonthlyOrgChanges), null, base.activePlayer, true, false);
			this.eventManager.AddListener<FactionFinalizesMissions>(new EventManager.EventDelegate<FactionFinalizesMissions>(this.UpdateDismissCouncilorButton), null, null, true, false);
			this.eventManager.AddListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnFactionResourcesUpdated), null, null, true, false);
			this.eventManager.AddListener<RecruitListsUpdated>(new EventManager.EventDelegate<RecruitListsUpdated>(this.OnCouncilorRecruitListUpdated), null, null, true, false);
			if (!base.activePlayer.completedProjects.Contains(this.size5Project))
			{
				this.councilSize5Notice.enabled = true;
			}
			else
			{
				this.councilSize5Notice.enabled = false;
			}
			if (!base.activePlayer.completedProjects.Contains(this.size6Project))
			{
				this.councilSize6Notice.enabled = true;
			}
			else
			{
				this.councilSize6Notice.enabled = false;
			}
			this.UpdateGridPrimaryDisplayElements();
			this.hasBeenShown = true;
		}

		// Token: 0x06004B5A RID: 19290 RVA: 0x001F8093 File Offset: 0x001F6293
		public override void Hide()
		{
			this.CleanUpDisplay();
			this.HideAllTutorials();
			base.Hide();
		}

		// Token: 0x06004B5B RID: 19291 RVA: 0x001F80A7 File Offset: 0x001F62A7
		public override bool Visible()
		{
			return base.Visible() && base.canvasManager.IsShowingInfoScreen<CouncilGridController>();
		}

		// Token: 0x06004B5C RID: 19292 RVA: 0x001F80C0 File Offset: 0x001F62C0
		public void CleanUpDisplay()
		{
			this.CloseAllAdvicePanels();
			this.DisableCouncilorGrid();
			this.DisableRecruitingCanvas();
			this.DisableSingleCouncilorScreen();
			this.DisableRecruitDetailCanvas();
			this.LeaveOrgManagement();
			using (IEnumerator<object> enumerator = this.councilorGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__276.<>p__0 == null)
					{
						CouncilGridController.<>o__276.<>p__0 = CallSite<Func<CallSite, object, CouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorGridItemController), typeof(CouncilGridController)));
					}
					CouncilorGridItemController councilorGridItemController = CouncilGridController.<>o__276.<>p__0.Target(CouncilGridController.<>o__276.<>p__0, enumerator.Current);
					if (councilorGridItemController != null && councilorGridItemController.councilorVideo.isPlaying)
					{
						councilorGridItemController.councilorVideo.Stop();
					}
				}
			}
			this.eventManager.RemoveListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateCouncilGrid), null);
			this.eventManager.RemoveListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.UpdateCouncilScreenForValueChange), null);
			this.eventManager.RemoveListener<FactionFinalizesMissions>(new EventManager.EventDelegate<FactionFinalizesMissions>(this.UpdateDismissCouncilorButton), null);
			this.eventManager.RemoveListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnFactionResourcesUpdated), null);
			this.eventManager.RemoveListener<CouncilOrgsChanged>(new EventManager.EventDelegate<CouncilOrgsChanged>(this.UpdateCouncilorScreenForMonthlyOrgChanges), null);
			this.eventManager.RemoveListener<RecruitListsUpdated>(new EventManager.EventDelegate<RecruitListsUpdated>(this.OnCouncilorRecruitListUpdated), null);
		}

		// Token: 0x06004B5D RID: 19293 RVA: 0x001F821C File Offset: 0x001F641C
		private void UpdateSingleCouncilorInfo(CouncilorDetailRequested e)
		{
			base.canvasManager.ToggleInfoScreen<CouncilGridController>();
			this.OnClickCouncilGridItem(e.councilor);
		}

		// Token: 0x06004B5E RID: 19294 RVA: 0x001F8235 File Offset: 0x001F6435
		public void CloseInfoScreen(bool toggle = false)
		{
			this.CleanUpDisplay();
			base.canvasManager.HideInfoScreen<CouncilGridController>(toggle);
		}

		// Token: 0x06004B5F RID: 19295 RVA: 0x001F824C File Offset: 0x001F644C
		private void UpdateCouncilScreenForValueChange(CouncilorValuesChanged e)
		{
			if (this.Visible() && (base.activePlayer.councilors.Contains(e.councilor) || base.activePlayer.turnedCouncilors.Contains(e.councilor)))
			{
				if (this.councilGridCanvas.enabled)
				{
					this.UpdateSingleCouncilorInGrid(e.councilor);
				}
				if (this.councilorSingleCanvas.enabled && this.currentCouncilor == e.councilor)
				{
					this.SetCouncilorInfo();
				}
				if (this.orgManagementCanvas.enabled)
				{
					this.RefreshOrgManagementUI();
				}
			}
		}

		// Token: 0x06004B60 RID: 19296 RVA: 0x001F82E8 File Offset: 0x001F64E8
		private void UpdateCouncilorScreenForMonthlyOrgChanges(CouncilOrgsChanged e)
		{
			if (this.Visible() && this.councilorSingleCanvas.enabled && TIGameState.Valid(this.currentCouncilor) && (base.activePlayer.councilors.Contains(this.currentCouncilor) || base.activePlayer.turnedCouncilors.Contains(this.currentCouncilor)))
			{
				this.MoveOrgCancel();
				this.SetCouncilorInfo();
			}
		}

		// Token: 0x06004B61 RID: 19297 RVA: 0x001F8354 File Offset: 0x001F6554
		private void UpdateDismissCouncilorButton(FactionFinalizesMissions e)
		{
			this.dismissButton.interactable = !GameStateManager.AllFactions().Any<TIFactionState>((TIFactionState x) => x.planningMissions) && !TIPromptQueueState.ActivePlayerHasSaveBlockingPrompt() && this.currentCouncilor != null && (this.currentCouncilor.faction == base.activePlayer || this.currentCouncilor.agentForFaction == base.activePlayer);
		}

		// Token: 0x06004B62 RID: 19298 RVA: 0x001F83E4 File Offset: 0x001F65E4
		private void UpdateCouncilGrid(CouncilCompositionChanged e)
		{
			if (e.council == base.activePlayer)
			{
				this.UpdateGridPrimaryDisplayElements();
				if (e.councilor == this.currentCouncilor && this.councilorSingleCanvas.enabled && !base.activePlayer.councilors.Contains(e.councilor) && !base.activePlayer.turnedCouncilors.Contains(e.councilor))
				{
					this.councilorSingleUITutorial.HideTutorial();
					this.DisableSingleCouncilorScreen();
					this.DisableRecruitingCanvas();
					this.UpdateGridPrimaryDisplayElements();
				}
			}
		}

		// Token: 0x06004B63 RID: 19299 RVA: 0x001F8477 File Offset: 0x001F6677
		private void DisableCouncilorGrid()
		{
			if (this.councilGridCanvas != null)
			{
				this.councilGridUITutorial.HideTutorial();
			}
		}

		// Token: 0x06004B64 RID: 19300 RVA: 0x001F8494 File Offset: 0x001F6694
		public void UpdateCouncilorGridIfAlreadyShown()
		{
			if (this.hasBeenShown)
			{
				this.UpdateCouncilorGrid();
				if (!this.councilorSingleCanvas.enabled)
				{
					this.HideAllTutorials();
					this.councilGridUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Grid, false, true);
				}
			}
			if (this.councilorSingleCanvas.enabled)
			{
				if (TIGameState.Valid(this.currentCouncilor))
				{
					this.SetCouncilorInfo();
					this.HideAllTutorials();
					this.councilorSingleUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Detail, false, true);
					return;
				}
				this.DisableSingleCouncilorScreen();
				this.HideAllTutorials();
				this.councilGridUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Grid, false, true);
			}
		}

		// Token: 0x06004B65 RID: 19301 RVA: 0x001F852C File Offset: 0x001F672C
		public void UpdateCouncilorGrid()
		{
			TICouncilorState[] array = new TICouncilorState[8];
			int num = 0;
			foreach (TICouncilorState ticouncilorState in base.activePlayer.councilors)
			{
				array[num++] = ticouncilorState;
			}
			num = 6;
			foreach (TICouncilorState ticouncilorState2 in base.activePlayer.turnedCouncilors)
			{
				array[num++] = ticouncilorState2;
			}
			this.councilorGrid.SetListSize<CouncilorGridItemController>(8, false, false);
			int num2 = 0;
			using (IEnumerator<object> enumerator2 = this.councilorGrid.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (CouncilGridController.<>o__286.<>p__0 == null)
					{
						CouncilGridController.<>o__286.<>p__0 = CallSite<Func<CallSite, object, CouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorGridItemController), typeof(CouncilGridController)));
					}
					CouncilorGridItemController councilorGridItemController = CouncilGridController.<>o__286.<>p__0.Target(CouncilGridController.<>o__286.<>p__0, enumerator2.Current);
					councilorGridItemController.Init(this, num2);
					if (array[num2] != null)
					{
						councilorGridItemController.UpdateListItem(array[num2], false);
						councilorGridItemController.primaryPanel.SetActive(true);
						if (councilorGridItemController.councilorVideo.clip != null && !councilorGridItemController.councilorVideo.isPlaying)
						{
							if (!this.councilorVideo.isPrepared)
							{
								TIUtilities.TryPrepareVideo(councilorGridItemController.councilorVideo);
							}
							base.StartCoroutine(this.PlayVideoWhenPrepared(councilorGridItemController.councilorVideo));
						}
					}
					else
					{
						councilorGridItemController.primaryPanel.SetActive(false);
					}
					num2++;
				}
			}
		}

		// Token: 0x06004B66 RID: 19302 RVA: 0x001F86FC File Offset: 0x001F68FC
		private void UpdateSingleCouncilorInGrid(TICouncilorState councilor)
		{
			int num = 0;
			using (IEnumerator<object> enumerator = this.councilorGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__287.<>p__0 == null)
					{
						CouncilGridController.<>o__287.<>p__0 = CallSite<Func<CallSite, object, CouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorGridItemController), typeof(CouncilGridController)));
					}
					CouncilorGridItemController councilorGridItemController = CouncilGridController.<>o__287.<>p__0.Target(CouncilGridController.<>o__287.<>p__0, enumerator.Current);
					if (councilorGridItemController.councilor == councilor)
					{
						councilorGridItemController.UpdateListItem(councilor, true);
						if (councilorGridItemController.councilorVideo.clip != null && !councilorGridItemController.councilorVideo.isPlaying)
						{
							if (!councilorGridItemController.councilorVideo.isPrepared)
							{
								TIUtilities.TryPrepareVideo(councilorGridItemController.councilorVideo);
							}
							base.StartCoroutine(this.PlayVideoWhenPrepared(councilorGridItemController.councilorVideo));
						}
					}
					num++;
				}
			}
		}

		// Token: 0x06004B67 RID: 19303 RVA: 0x001F87F0 File Offset: 0x001F69F0
		private void UpdateGridPrimaryDisplayElements()
		{
			this.CouncilName.SetText(Loc.T("UI.Council.CouncilNameCouncil", new object[] { base.activePlayer.displayName }));
			if (this.councilTabsManager.activeTab != this.councilGridTabController)
			{
				this.councilTabsManager.Toggle(this.councilGridTabController);
			}
			this.UpdateCouncilorGrid();
			this.HideAllTutorials();
			if (this.councilorSingleCanvas.enabled)
			{
				this.councilorSingleUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Detail, false, true);
				return;
			}
			this.councilGridUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Grid, false, true);
		}

		// Token: 0x06004B68 RID: 19304 RVA: 0x001F8890 File Offset: 0x001F6A90
		public void OnClickCouncilGridItem(TICouncilorState councilor)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.currentCouncilor = councilor;
			this.DisableCouncilorGrid();
			this.councilorSingleCanvas.enabled = true;
			this.HideAllTutorials();
			this.councilorSingleUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Detail, false, true);
			this.DisableRecruitingCanvas();
			this.missionListObject.SetActive(true);
			this.spendXPPanel.SetActive(false);
			this.spendXPButton.interactable = !this.lookingAtTurnedCouncilor && !this.spendXPPanel.activeSelf;
			this.SetCouncilorInfo();
		}

		// Token: 0x06004B69 RID: 19305 RVA: 0x001F8924 File Offset: 0x001F6B24
		public void OnClickGridRecruitButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.DisableCouncilorGrid();
			this.DisableRecruitDetailCanvas();
			this.DisableSingleCouncilorScreen();
			this.recruitingCanvas.enabled = true;
			GameControl.eventManager.AddListener<RecruitListsUpdated>(new EventManager.EventDelegate<RecruitListsUpdated>(this.OnCouncilorRecruitListUpdated), null, null, true, false);
			this.UpdateRecruitingPrimaryDisplayElements();
		}

		// Token: 0x06004B6A RID: 19306 RVA: 0x001F897B File Offset: 0x001F6B7B
		public void OnExitCouncilorGrid()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.councilGridUITutorial.HideTutorial();
			this.councilorSingleUITutorial.HideTutorial();
			this.CloseInfoScreen(false);
		}

		// Token: 0x06004B6B RID: 19307 RVA: 0x001F89A6 File Offset: 0x001F6BA6
		public void OnCloseandPlaySelected()
		{
			this.OnExitCouncilorGrid();
			base.gameTime.Play();
		}

		// Token: 0x06004B6C RID: 19308 RVA: 0x001F89B9 File Offset: 0x001F6BB9
		public void OnHelpPanelOpenClick()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
			this.helpPanel.SetActive(true);
			this.OnExitCouncilorPage();
			this.councilGridCanvas.enabled = false;
		}

		// Token: 0x06004B6D RID: 19309 RVA: 0x001F89E5 File Offset: 0x001F6BE5
		public void OnHelpPanelCloseClick()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.helpPanel.SetActive(false);
			this.councilGridCanvas.enabled = true;
			this.HideAllTutorials();
			this.councilGridUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Grid, false, true);
		}

		// Token: 0x06004B6E RID: 19310 RVA: 0x001F8A24 File Offset: 0x001F6C24
		public void OnResetTutorialClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.councilGridUITutorial.ResetTutorial(false);
			this.councilorSingleUITutorial.ResetTutorial(false);
			this.councilorRecruitingUITutorial.ResetTutorial(false);
			this.orgManagementUITutorial.ResetTutorial(false);
			this.ledgerUITutorial.ResetTutorial(false);
			this.calendarUITutorial.ResetTutorial(false);
		}

		// Token: 0x06004B6F RID: 19311 RVA: 0x001F8A88 File Offset: 0x001F6C88
		public void HideAllTutorials()
		{
			this.councilGridUITutorial.HideTutorial();
			this.councilorSingleUITutorial.HideTutorial();
			this.councilorRecruitingUITutorial.HideTutorial();
			this.orgManagementUITutorial.HideTutorial();
			this.ledgerUITutorial.HideTutorial();
			this.calendarUITutorial.HideTutorial();
		}

		// Token: 0x06004B70 RID: 19312 RVA: 0x001F8AD7 File Offset: 0x001F6CD7
		public void Tutorial_InitializeSingleCouncilorScreen()
		{
			this.dismissPanel.SetActive(false);
			this.MoveOrgCancel();
			if (this.spendXPPanel.activeSelf)
			{
				this.OnExitAugmentationMenuSelected();
			}
		}

		// Token: 0x06004B71 RID: 19313 RVA: 0x001F8AFE File Offset: 0x001F6CFE
		public void Tutorial_InitializeCouncilRecruitScreen()
		{
			this.confirmRecruitBox.SetActive(false);
		}

		// Token: 0x06004B72 RID: 19314 RVA: 0x001F8B0C File Offset: 0x001F6D0C
		public void Tutorial_OpenOrgMarketplace()
		{
			if (this.orgTabsManager.activeTab != this.orgMarketplaceTabController)
			{
				this.orgMarketplaceButton.onClick.Invoke();
			}
		}

		// Token: 0x06004B73 RID: 19315 RVA: 0x001F8B36 File Offset: 0x001F6D36
		public void Tutorial_OpenUnassignedOrgs()
		{
			if (this.orgTabsManager.activeTab != this.unassignedOrgsTabController)
			{
				this.unassignedOrgsButton.onClick.Invoke();
			}
		}

		// Token: 0x06004B74 RID: 19316 RVA: 0x001F8B60 File Offset: 0x001F6D60
		public void Tutorial_SelectFirstCandidate()
		{
			if (!this.candidateDetailCanvas.enabled && this.candidateList != null && this.candidateList.size > 0)
			{
				using (IEnumerator<object> enumerator = this.candidateList.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						if (CouncilGridController.<>o__301.<>p__0 == null)
						{
							CouncilGridController.<>o__301.<>p__0 = CallSite<Func<CallSite, object, CouncilorRecruitListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorRecruitListItemController), typeof(CouncilGridController)));
						}
						CouncilGridController.<>o__301.<>p__0.Target(CouncilGridController.<>o__301.<>p__0, enumerator.Current).ItemSelected();
					}
				}
			}
		}

		// Token: 0x06004B75 RID: 19317 RVA: 0x001F8C18 File Offset: 0x001F6E18
		public void Tutorial_InitOrgManager()
		{
			Scrollbar[] componentsInChildren = this.orgManagementCanvas.gameObject.GetComponentsInChildren<Scrollbar>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].value = 1f;
			}
		}

		// Token: 0x06004B76 RID: 19318 RVA: 0x001F8C54 File Offset: 0x001F6E54
		public void Tutorial_HighlightFirstCalendarClock()
		{
			GameObject gameObject = null;
			if (this.visibleMonthGridList != null && this.visibleMonthGridList.size > 0)
			{
				using (IEnumerator<object> enumerator = this.visibleMonthGridList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CouncilGridController.<>o__303.<>p__0 == null)
						{
							CouncilGridController.<>o__303.<>p__0 = CallSite<Func<CallSite, object, CalendarDayGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CalendarDayGridItemController), typeof(CouncilGridController)));
						}
						CalendarDayGridItemController calendarDayGridItemController = CouncilGridController.<>o__303.<>p__0.Target(CouncilGridController.<>o__303.<>p__0, enumerator.Current);
						if (calendarDayGridItemController.alarmButton.gameObject.activeInHierarchy)
						{
							gameObject = calendarDayGridItemController.alarmButton.gameObject;
							break;
						}
					}
				}
			}
			if (gameObject != null)
			{
				RectTransform rectTransform = this.alarmClockHighlightDummy.transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.SetParent(gameObject.transform, false);
					rectTransform.anchorMin = Vector2.zero;
					rectTransform.anchorMax = Vector2.one;
					rectTransform.offsetMin = Vector2.zero;
					rectTransform.offsetMax = Vector2.zero;
				}
			}
		}

		// Token: 0x06004B77 RID: 19319 RVA: 0x001F8D74 File Offset: 0x001F6F74
		public IEnumerator PlayVideoWhenPrepared(VideoPlayer videoPlayer)
		{
			while (!videoPlayer.isPrepared)
			{
				yield return null;
			}
			if (videoPlayer.gameObject.activeInHierarchy && videoPlayer.clip != null)
			{
				TIUtilities.TryPlayVideo(videoPlayer);
			}
			yield break;
		}

		// Token: 0x06004B78 RID: 19320 RVA: 0x001F8D83 File Offset: 0x001F6F83
		private void DisableRecruitingCanvas()
		{
			if (this.recruitingCanvas != null)
			{
				this.recruitingCanvas.enabled = false;
				this.DisableRecruitDetailCanvas();
			}
		}

		// Token: 0x06004B79 RID: 19321 RVA: 0x001F8DA8 File Offset: 0x001F6FA8
		private void OnCouncilorRecruitListUpdated(RecruitListsUpdated e)
		{
			if (this.recruitingCanvas.enabled)
			{
				this.UpdateCandidatesList(base.activePlayer);
				if (this.candidateDetailCanvas.enabled && !base.activePlayer.availableCouncilors.Contains(this.selectedCandidate))
				{
					this.DisableRecruitDetailCanvas();
				}
			}
		}

		// Token: 0x06004B7A RID: 19322 RVA: 0x001F8DFC File Offset: 0x001F6FFC
		private void DisableRecruitDetailCanvas()
		{
			if (this.candidateDetailCanvas != null)
			{
				if (this.recruitVideo.clip != null && this.recruitVideo.isPlaying)
				{
					this.recruitVideo.clip = null;
					this.recruitVideo.Stop();
				}
				this.candidateDetailCanvas.enabled = false;
				this.SelectCandidateListItem(null);
				this.selectCandidateCanvas.enabled = true;
				this.confirmRecruitBox.SetActive(false);
			}
			this.selectedCandidate = null;
		}

		// Token: 0x06004B7B RID: 19323 RVA: 0x001F8E80 File Offset: 0x001F7080
		public void RecruitSelectedCandidateButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.confirmRecruitDialog.SetText(Loc.T("UI.CouncilRecruiting.ConfirmRecruitDialog", new object[]
			{
				this.selectedCandidate.displayName,
				this.selectedCandidate.GetRecruitCostString(base.activePlayer, true)
			}));
			this.confirmRecruitBox.SetActive(true);
		}

		// Token: 0x06004B7C RID: 19324 RVA: 0x001F8EE3 File Offset: 0x001F70E3
		public void OnDeclineRecruitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
			this.confirmRecruitBox.SetActive(false);
		}

		// Token: 0x06004B7D RID: 19325 RVA: 0x001F8F00 File Offset: 0x001F7100
		public void OnConfirmRecruitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new RecruitCouncilorAction(this.selectedCandidate, base.activePlayer));
			this.confirmRecruitBox.SetActive(false);
			this.selectedCandidate = null;
			this.UpdateRecruitingPrimaryDisplayElements();
			this.DisableRecruitingCanvas();
		}

		// Token: 0x06004B7E RID: 19326 RVA: 0x001F8F5C File Offset: 0x001F715C
		public void UpdateRecruitingPrimaryDisplayElements()
		{
			this.confirmRecruitBox.SetActive(false);
			this.UpdateCandidatesList(base.activePlayer);
			if (TIGameState.Valid(this.selectedCandidate))
			{
				this.UpdateCandidateDetail(this.selectedCandidate);
			}
			else
			{
				this.selectedCandidate = null;
				this.DisableRecruitDetailCanvas();
			}
			this.HideAllTutorials();
			this.councilorRecruitingUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Recruiting, false, true);
		}

		// Token: 0x06004B7F RID: 19327 RVA: 0x001F8FC4 File Offset: 0x001F71C4
		private void UpdateCandidatesList(TIFactionState councilState)
		{
			if (this.candidateList != null)
			{
				List<TICouncilorState> availableCouncilors = councilState.availableCouncilors;
				this.candidateList.SetListSize<CouncilorRecruitListItemController>(availableCouncilors.Count, false, false);
				int num = 0;
				using (IEnumerator<object> enumerator = this.candidateList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CouncilGridController.<>o__312.<>p__0 == null)
						{
							CouncilGridController.<>o__312.<>p__0 = CallSite<Func<CallSite, object, CouncilorRecruitListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorRecruitListItemController), typeof(CouncilGridController)));
						}
						CouncilorRecruitListItemController councilorRecruitListItemController = CouncilGridController.<>o__312.<>p__0.Target(CouncilGridController.<>o__312.<>p__0, enumerator.Current);
						councilorRecruitListItemController.Init(this);
						councilorRecruitListItemController.UpdateListItem(availableCouncilors[num++], councilState);
					}
				}
			}
		}

		// Token: 0x06004B80 RID: 19328 RVA: 0x001F9090 File Offset: 0x001F7290
		public void UpdateCandidateDetail(TICouncilorState councilor)
		{
			this.candidateDetailCanvas.enabled = true;
			this.selectCandidateCanvas.enabled = false;
			this.candidateName.SetText(councilor.displayName);
			this.candidateJob.SetText(councilor.jobDisplayName);
			CouncilorIllustrationData illustrationData = councilor.GetIllustrationData();
			this.candidateBackgroundImage.transform.localPosition = illustrationData.GetIllustrationLocalPosition(this.candidateBackgroundImage, this.candidateBackgroundImageInitialPosition);
			GameControl.assetLoader.LoadAssetForImageAssignment(illustrationData.illustrationPath, this.candidateBackgroundImage);
			this.candidateJobTooltip.SetText("BodyText", councilor.typeTemplate.description);
			this.candidateLocation.SetText(TIUtilities.GetLocationString(councilor.location, true, false));
			this.candidateAge.SetText(councilor.GetVerboseAgeString());
			this.candidateHomeRegion.SetText(councilor.GetVerboseHomeLocationString());
			this.recruitCost.SetText(councilor.GetRecruitCostString(base.activePlayer, false));
			this.candidatePersuasion.SetText(councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false).ToString());
			this.candidateInvestigation.SetText(councilor.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false).ToString());
			this.candidateEspionage.SetText(councilor.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false).ToString());
			this.candidateCommand.SetText(councilor.GetAttribute(CouncilorAttribute.Command, true, true, true, false, false, false).ToString());
			this.candidateAdministration.SetText(councilor.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false).ToString());
			this.candidateScience.SetText(councilor.GetAttribute(CouncilorAttribute.Science, true, true, true, false, false, false).ToString());
			this.candidateSecurity.SetText(councilor.GetAttribute(CouncilorAttribute.Security, true, true, true, false, false, false).ToString());
			this.candidateLoyalty.SetText(councilor.GetAttribute(CouncilorAttribute.ApparentLoyalty, true, true, true, false, false, false).ToString());
			this.candidateMoneyIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Money), 2, 0, true, false));
			this.candidateInfluenceIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Influence), 2, 0, true, false));
			this.candidateOpsIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Operations), 2, 0, true, false));
			this.candidateResearchIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Research), 2, 0, true, false));
			this.candidateBoostIncome.SetText(TIUtilities.FormatSmallNumber(councilor.GetMonthlyIncome(FactionResource.Boost), 2, 0, true, false));
			this.candidateMissionControlIncome.SetText(councilor.GetMonthlyIncome(FactionResource.MissionControl).ToString("N0"));
			this.candidateProjectsIncome.SetText(councilor.projectContributionString);
			this.UpdateCandidateMissionsList(councilor);
			this.UpdateCandidateTraitsList(councilor);
			this.recruitCandidateButton.interactable = councilor.HireRecruitCost(base.activePlayer).CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity) && base.activePlayer.councilors.Count < base.activePlayer.maxCouncilSize;
			if (TIPlayerProfileManager.useCouncilorVideo)
			{
				this.recruitVideo.clip = GameControl.assetLoader.LoadAsset<VideoClip>(councilor.videoResource);
				this.recruitVideo.gameObject.SetActive(true);
				this.recruitCouncilorStillImage.sprite = null;
				this.recruitCouncilorStillImage.enabled = false;
				if (!this.recruitVideo.isPlaying)
				{
					TIUtilities.TryPlayVideo(this.recruitVideo);
					return;
				}
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(councilor.portraitResource, this.recruitCouncilorStillImage);
				this.recruitCouncilorStillImage.enabled = true;
				this.recruitVideo.Stop();
				this.recruitVideo.clip = null;
				this.recruitVideo.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004B81 RID: 19329 RVA: 0x001F9448 File Offset: 0x001F7648
		public void SelectCandidateListItem(CouncilorRecruitListItemController listItem)
		{
			using (IEnumerator<object> enumerator = this.candidateList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__314.<>p__0 == null)
					{
						CouncilGridController.<>o__314.<>p__0 = CallSite<Func<CallSite, object, CouncilorRecruitListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorRecruitListItemController), typeof(CouncilGridController)));
					}
					CouncilorRecruitListItemController councilorRecruitListItemController = CouncilGridController.<>o__314.<>p__0.Target(CouncilGridController.<>o__314.<>p__0, enumerator.Current);
					councilorRecruitListItemController.SetSelected(listItem != null && councilorRecruitListItemController == listItem);
				}
			}
		}

		// Token: 0x06004B82 RID: 19330 RVA: 0x001F94E8 File Offset: 0x001F76E8
		public void UpdateCandidateMissionsList(TICouncilorState councilorState)
		{
			if (this.candidateMissionsList != null)
			{
				this.UpdateMissionsList(this.candidateMissionsList, councilorState);
			}
		}

		// Token: 0x06004B83 RID: 19331 RVA: 0x001F9508 File Offset: 0x001F7708
		private void UpdateMissionsList(ListManagerBase listManager, TICouncilorState councilorState)
		{
			bool flag = listManager == this.missionsList;
			Dictionary<TIMissionTemplate, int> councilTotalMissionCountDictionary = this.GetCouncilTotalMissionCountDictionary();
			List<TIMissionTemplate> list = councilorState.GetPossibleMissionList(false, true, true, null, false);
			list = list.OrderBy<TIMissionTemplate, CouncilorAttribute>((TIMissionTemplate o) => o.primaryAttackerStat).ToList<TIMissionTemplate>();
			list = list.OrderBy<TIMissionTemplate, bool>((TIMissionTemplate o) => o.primaryAttackerStat > CouncilorAttribute.None).ToList<TIMissionTemplate>();
			list = list.OrderBy<TIMissionTemplate, bool>((TIMissionTemplate o) => o.baseMission).ToList<TIMissionTemplate>();
			CouncilorAttribute councilorAttribute = CouncilorAttribute.None;
			int num = 0;
			bool flag2 = true;
			foreach (TIMissionTemplate timissionTemplate in list)
			{
				if (flag2)
				{
					councilorAttribute = timissionTemplate.primaryAttackerStat;
					num++;
					flag2 = false;
				}
				else if (timissionTemplate.primaryAttackerStat != councilorAttribute)
				{
					num++;
					councilorAttribute = timissionTemplate.primaryAttackerStat;
				}
			}
			listManager.SetListSize<MissionsListItemController>(list.Count + num, false, false);
			bool flag3 = true;
			int num2 = 0;
			using (IEnumerator<object> enumerator2 = listManager.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (CouncilGridController.<>o__316.<>p__0 == null)
					{
						CouncilGridController.<>o__316.<>p__0 = CallSite<Func<CallSite, object, MissionsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(MissionsListItemController), typeof(CouncilGridController)));
					}
					MissionsListItemController missionsListItemController = CouncilGridController.<>o__316.<>p__0.Target(CouncilGridController.<>o__316.<>p__0, enumerator2.Current);
					if (flag3)
					{
						councilorAttribute = list[num2].primaryAttackerStat;
						missionsListItemController.SetListItem(councilorAttribute, councilorState, null, false, true);
						missionsListItemController.gameObject.SetActive(true);
						flag3 = false;
					}
					else if (list[num2].primaryAttackerStat != councilorAttribute)
					{
						councilorAttribute = list[num2].primaryAttackerStat;
						missionsListItemController.SetListItem(councilorAttribute, councilorState, list[num2], true, false);
						missionsListItemController.gameObject.SetActive(true);
					}
					else
					{
						int num3 = (councilTotalMissionCountDictionary.ContainsKey(list[num2]) ? councilTotalMissionCountDictionary[list[num2]] : 0);
						missionsListItemController.SetListItem(list[num2], councilorState, num3, flag && this.automateMissionsToggle.isOn, num2 + 1 >= list.Count || (num2 + 1 < list.Count && list[num2 + 1].primaryAttackerStat != councilorAttribute));
						missionsListItemController.gameObject.SetActive(true);
						num2++;
					}
				}
			}
		}

		// Token: 0x06004B84 RID: 19332 RVA: 0x001F97D8 File Offset: 0x001F79D8
		private Dictionary<TIMissionTemplate, int> GetCouncilTotalMissionCountDictionary()
		{
			List<TIMissionTemplate> list = new List<TIMissionTemplate>();
			foreach (TICouncilorState ticouncilorState in base.activePlayer.councilors)
			{
				List<TIMissionTemplate> possibleMissionList = ticouncilorState.GetPossibleMissionList(false, true, true, null, false);
				list.AddRange(possibleMissionList);
			}
			Dictionary<TIMissionTemplate, int> dictionary = new Dictionary<TIMissionTemplate, int>();
			foreach (TIMissionTemplate timissionTemplate in list)
			{
				if (dictionary.ContainsKey(timissionTemplate))
				{
					Dictionary<TIMissionTemplate, int> dictionary2 = dictionary;
					TIMissionTemplate timissionTemplate2 = timissionTemplate;
					int num = dictionary2[timissionTemplate2];
					dictionary2[timissionTemplate2] = num + 1;
				}
				else
				{
					dictionary.Add(timissionTemplate, 1);
				}
			}
			return dictionary;
		}

		// Token: 0x06004B85 RID: 19333 RVA: 0x001F98B0 File Offset: 0x001F7AB0
		public void UpdateCandidateTraitsList(TICouncilorState councilorState)
		{
			if (this.candidateTraitsList != null)
			{
				this.candidateTraitsList.SetListSize<TraitsListItemController>(councilorState.traits.Count, false, false);
				int num = 0;
				using (IEnumerator<object> enumerator = this.candidateTraitsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CouncilGridController.<>o__318.<>p__0 == null)
						{
							CouncilGridController.<>o__318.<>p__0 = CallSite<Func<CallSite, object, TraitsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TraitsListItemController), typeof(CouncilGridController)));
						}
						TraitsListItemController traitsListItemController = CouncilGridController.<>o__318.<>p__0.Target(CouncilGridController.<>o__318.<>p__0, enumerator.Current);
						traitsListItemController.UpdateListItem(councilorState.traits[num], num, num == this.candidateTraitsList.size - 1);
						traitsListItemController.gameObject.SetActive(true);
						num++;
					}
				}
			}
		}

		// Token: 0x06004B86 RID: 19334 RVA: 0x001F9994 File Offset: 0x001F7B94
		private void DisableSingleCouncilorScreen()
		{
			if (this.councilorSingleCanvas != null)
			{
				DragManager.ResetCurrentItem();
				this.councilorSingleCanvas.enabled = false;
				this.CloseMoveOrgPanel();
				if (this.councilorVideo.clip != null && this.councilorVideo.isPlaying)
				{
					this.councilorVideo.clip = null;
					this.councilorVideo.Stop();
				}
			}
		}

		// Token: 0x06004B87 RID: 19335 RVA: 0x001F99FD File Offset: 0x001F7BFD
		public void OnExitCouncilorPage()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.councilorSingleUITutorial.HideTutorial();
			this.DisableSingleCouncilorScreen();
			this.DisableRecruitingCanvas();
			this.UpdateGridPrimaryDisplayElements();
		}

		// Token: 0x06004B88 RID: 19336 RVA: 0x001F9A28 File Offset: 0x001F7C28
		public void OnClickSpendXPButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.missionListObject.SetActive(false);
			this.spendXPPanel.SetActive(true);
			this.spendXPButton.interactable = !this.lookingAtTurnedCouncilor && !this.spendXPPanel.activeSelf;
			this.SetAugmentationPanel();
		}

		// Token: 0x06004B89 RID: 19337 RVA: 0x001F9A84 File Offset: 0x001F7C84
		public void GotoButtonClicked()
		{
			this.CloseInfoScreen(false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyCouncilorSelect", false, false);
			TIUtilities.GotoGameState(this.currentCouncilor, true, true, true);
			GameControl.eventManager.TriggerEvent(new CouncilorSelectedOffMap(this.currentCouncilor), null, new object[] { this.currentCouncilor.ref_region });
		}

		// Token: 0x06004B8A RID: 19338 RVA: 0x001F9ADC File Offset: 0x001F7CDC
		public void CycleRightButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (this.currentCouncilor.faction == base.activePlayer)
			{
				this.currentCouncilor = this.currentCouncilor.faction.GetNextCouncilor(this.currentCouncilor, false);
			}
			else
			{
				int num = base.activePlayer.turnedCouncilors.FindIndex((TICouncilorState x) => x == this.currentCouncilor);
				int count = base.activePlayer.turnedCouncilors.Count;
				if (num + 1 >= count)
				{
					num = 0;
				}
				else
				{
					num++;
				}
				this.currentCouncilor = base.activePlayer.turnedCouncilors[num];
			}
			this.SetCouncilorInfo();
			if (this.spendXPPanel.activeSelf)
			{
				this.SetAugmentationPanel();
			}
		}

		// Token: 0x06004B8B RID: 19339 RVA: 0x001F9B98 File Offset: 0x001F7D98
		public void CycleLeftButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			if (this.currentCouncilor.faction == base.activePlayer)
			{
				this.currentCouncilor = this.currentCouncilor.faction.GetPreviousCouncilor(this.currentCouncilor);
			}
			else
			{
				int num = base.activePlayer.turnedCouncilors.FindIndex((TICouncilorState x) => x == this.currentCouncilor);
				int count = base.activePlayer.turnedCouncilors.Count;
				if (num - 1 < 0)
				{
					num = base.activePlayer.turnedCouncilors.Count - 1;
				}
				else
				{
					num--;
				}
				this.currentCouncilor = base.activePlayer.turnedCouncilors[num];
			}
			this.SetCouncilorInfo();
			if (this.spendXPPanel.activeSelf)
			{
				this.SetAugmentationPanel();
			}
		}

		// Token: 0x06004B8C RID: 19340 RVA: 0x001F9C64 File Offset: 0x001F7E64
		public void CustomizeButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.councilorSingleGameObject.SetActive(false);
			this.customizeCouncilorPanel.SetActive(true);
			this.councilorSingleUITutorial.HideTutorial();
			this.InitializeCustomizationOptions();
		}

		// Token: 0x06004B8D RID: 19341 RVA: 0x001F9C9C File Offset: 0x001F7E9C
		public void FirstDismissButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			if (this.currentCouncilor.agentForFaction == base.activePlayer)
			{
				this.dismissQueryText.SetText(Loc.T("UI.Councilor.ConfirmDismissEnemyCouncilor", new object[]
				{
					this.currentCouncilor.displayName,
					this.currentCouncilor.faction.displayName
				}));
				this.dismissKeepButton.SetActive(true);
				this.dismissSellButton.SetActive(false);
				this.dismissKeepButtonText.SetText(Loc.T("UI.Councilor.DismissEnemyButton"));
			}
			else
			{
				this.dismissQueryText.SetText(Loc.T("UI.Councilor.ConfirmDismissCouncilor", new object[] { this.currentCouncilor.displayName }));
				this.dismissSellButton.SetActive(true);
				if (this.currentCouncilor.orgs.Count == 0)
				{
					this.dismissKeepButton.SetActive(false);
					this.dismissSellButtonText.SetText(Loc.T("UI.Councilor.DismissButton"));
				}
				else
				{
					TIResourcesCost allOrgsSaleValue = this.currentCouncilor.AllOrgsSaleValue;
					this.dismissKeepButton.SetActive(true);
					this.dismissKeepButtonText.SetText(Loc.T("UI.Councilor.DismissandKeepOrgs"));
					this.dismissSellButtonText.SetText(Loc.T("UI.Councilor.DismissandSellOrgs", new object[] { allOrgsSaleValue.ToString("N0", false, false, null, false, FactionResource.None) }));
				}
			}
			this.dismissPanel.SetActive(true);
		}

		// Token: 0x06004B8E RID: 19342 RVA: 0x001F9E0C File Offset: 0x001F800C
		public void ConfirmDismissAndSellButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			TIFactionState faction = this.currentCouncilor.faction;
			if (faction != null)
			{
				Player playerControl = faction.playerControl;
				List<SellOrgAction> list = new List<SellOrgAction>();
				List<TransferOrgToFactionPoolAction> list2 = new List<TransferOrgToFactionPoolAction>();
				foreach (TIOrgState tiorgState in this.currentCouncilor.orgs)
				{
					if (tiorgState.AllowedOnFactionMarket(faction))
					{
						list.Add(new SellOrgAction(tiorgState, faction, this.currentCouncilor));
					}
					else
					{
						list2.Add(new TransferOrgToFactionPoolAction(tiorgState, this.currentCouncilor));
					}
				}
				foreach (SellOrgAction sellOrgAction in list)
				{
					playerControl.StartAction(sellOrgAction);
				}
				foreach (TransferOrgToFactionPoolAction transferOrgToFactionPoolAction in list2)
				{
					playerControl.StartAction(transferOrgToFactionPoolAction);
				}
				DismissCouncilorAction dismissCouncilorAction = new DismissCouncilorAction(this.currentCouncilor, faction, faction);
				playerControl.StartAction(dismissCouncilorAction);
			}
			this.dismissPanel.SetActive(false);
			this.currentCouncilor = null;
			this.DisableSingleCouncilorScreen();
		}

		// Token: 0x06004B8F RID: 19343 RVA: 0x001F9F7C File Offset: 0x001F817C
		public void ConfirmDismissAndKeepButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			TIFactionState faction = this.currentCouncilor.faction;
			if (faction != null)
			{
				Player playerControl = faction.playerControl;
				List<TransferOrgToFactionPoolAction> list = new List<TransferOrgToFactionPoolAction>();
				foreach (TIOrgState tiorgState in this.currentCouncilor.orgs)
				{
					list.Add(new TransferOrgToFactionPoolAction(tiorgState, this.currentCouncilor));
				}
				foreach (TransferOrgToFactionPoolAction transferOrgToFactionPoolAction in list)
				{
					playerControl.StartAction(transferOrgToFactionPoolAction);
				}
				DismissCouncilorAction dismissCouncilorAction = new DismissCouncilorAction(this.currentCouncilor, faction, base.activePlayer);
				playerControl.StartAction(dismissCouncilorAction);
			}
			this.dismissPanel.SetActive(false);
			this.currentCouncilor = null;
			this.DisableSingleCouncilorScreen();
		}

		// Token: 0x06004B90 RID: 19344 RVA: 0x001FA088 File Offset: 0x001F8288
		public void DismissCancelButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
			this.dismissPanel.SetActive(false);
		}

		// Token: 0x06004B91 RID: 19345 RVA: 0x001FA0A2 File Offset: 0x001F82A2
		public void MoveOrgCancelButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
			this.MoveOrgCancel();
		}

		// Token: 0x06004B92 RID: 19346 RVA: 0x001FA0B6 File Offset: 0x001F82B6
		public void MoveOrgCancel()
		{
			DragManager.ResetCurrentItem();
			this.CloseMoveOrgPanel();
		}

		// Token: 0x06004B93 RID: 19347 RVA: 0x001FA0C3 File Offset: 0x001F82C3
		public void MoveOrgOkButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.CloseMoveOrgPanel();
		}

		// Token: 0x06004B94 RID: 19348 RVA: 0x001FA0D7 File Offset: 0x001F82D7
		public void MoveOrgPurchaseConfirmClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.CompletePurchaseOrg();
		}

		// Token: 0x06004B95 RID: 19349 RVA: 0x001FA0EB File Offset: 0x001F82EB
		public void MoveOrgSellConfirmClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.CompleteSellOrg();
		}

		// Token: 0x06004B96 RID: 19350 RVA: 0x001FA100 File Offset: 0x001F8300
		private void CloseMoveOrgPanel()
		{
			this.confirmMoveQueryText.SetText(string.Empty);
			this.orgCostText.SetText(string.Empty);
			this.sellValueText.SetText(string.Empty);
			this.confirmPurchase.SetActive(false);
			this.confirmSell.SetActive(false);
			this.cancelMoveOrg.SetActive(false);
			this.moveFailureOk.SetActive(false);
			this.confirmMovePanel.SetActive(false);
			if (this.councilorSingleCanvas.enabled && TIGameState.Valid(this.currentCouncilor))
			{
				this.SetCouncilorInfo();
			}
		}

		// Token: 0x06004B97 RID: 19351 RVA: 0x001FA19C File Offset: 0x001F839C
		public void StartOrgPurchase(TIOrgState org)
		{
			this.sb.Clear();
			if (!org.IsEligibleForCouncilor(this.currentCouncilor))
			{
				this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.IneligibleOwner", new object[]
				{
					org.displayName,
					this.currentCouncilor.displayName,
					org.IneligibleReasonString(this.currentCouncilor)
				}));
				return;
			}
			if (!org.CouncilorCanAcquire(this.currentCouncilor))
			{
				this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.CantAcquire", new object[]
				{
					org.displayName,
					this.currentCouncilor.displayName
				}));
				return;
			}
			if (!this.currentCouncilor.SufficientCapacityForOrg(org))
			{
				this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.InsufficientAdminStat", new object[]
				{
					org.displayName,
					this.currentCouncilor.displayName,
					TemplateManager.global.councilorMaxOrgs.ToString("N0")
				}));
				return;
			}
			TIFactionState faction = this.currentCouncilor.faction;
			if (faction.CanPurchaseOrg(org))
			{
				this.purchaseOrgAction = new PurchaseOrgAction(org, faction, this.currentCouncilor);
				this.transferOrgAction = null;
				this.RaiseConfirmPurchaseOrg();
				return;
			}
			if (faction.OwnsOrgInUnassignedPool(org))
			{
				this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.InsufficientResourcesForTransfer", new object[] { org.displayName }));
				return;
			}
			this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.InsufficientResourcesForPurchase", new object[] { org.displayName }));
		}

		// Token: 0x06004B98 RID: 19352 RVA: 0x001FA310 File Offset: 0x001F8510
		private void RaiseConfirmPurchaseOrg()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			TIOrgState org = this.purchaseOrgAction.GetOrg();
			string text;
			if (this.currentCouncilor.faction.OwnsOrgInUnassignedPool(org))
			{
				text = org.GetTransferCost().ToString("Relevant", false, false, null, false, FactionResource.None);
			}
			else
			{
				text = org.GetPurchaseCost(base.activePlayer).ToString("Relevant", false, false, null, false, FactionResource.None);
			}
			if (text != string.Empty)
			{
				this.orgCostText.gameObject.SetActive(true);
				this.orgCostText.SetText(text);
			}
			else
			{
				this.orgCostText.gameObject.SetActive(false);
			}
			this.sb.Clear();
			if (this.currentCouncilor.faction.OwnsOrgInUnassignedPool(org))
			{
				this.confirmPurchaseText.SetText(Loc.T("UI.Councilor.Orgs.ConfirmTransferButton"));
				this.sb.Append(Loc.T("UI.Councilor.Orgs.ConfirmTransferToCouncilor", new object[]
				{
					org.displayName,
					this.purchaseOrgAction.GetCouncilorAssignment().displayName,
					org.GetTransferCost().ToString("N0", false, false, null, false, FactionResource.None)
				}));
			}
			else
			{
				if (this.purchaseOrgAction.HasAssignment())
				{
					this.sb.Append(Loc.T("UI.Councilor.Orgs.ConfirmPurchaseAndAssignment", new object[]
					{
						org.displayName,
						org.GetPurchaseCost(base.activePlayer).ToString("N0", false, false, null, false, FactionResource.None),
						this.purchaseOrgAction.GetCouncilorAssignment().displayName
					}));
				}
				else
				{
					this.sb.Append(Loc.T("UI.Councilor.Orgs.ConfirmPurchase", new object[]
					{
						org.displayName,
						org.GetPurchaseCost(base.activePlayer).ToString("N0", false, false, null, false, FactionResource.None)
					}));
				}
				this.confirmPurchaseText.SetText(Loc.T("UI.Councilor.Orgs.ConfirmPurchaseButton"));
			}
			this.confirmMoveQueryText.SetText(this.sb.ToString());
			this.confirmPurchase.SetActive(true);
			this.cancelMoveOrg.SetActive(true);
			this.moveFailureOk.SetActive(false);
			this.confirmSell.SetActive(false);
			this.confirmMovePanel.SetActive(true);
		}

		// Token: 0x06004B99 RID: 19353 RVA: 0x001FA554 File Offset: 0x001F8754
		private void CompletePurchaseOrg()
		{
			this.council = this.currentCouncilor.faction;
			Player playerControl = this.currentCouncilor.faction.playerControl;
			if (this.transferOrgAction != null)
			{
				playerControl.StartAction(this.transferOrgAction);
			}
			else if (this.purchaseOrgAction != null && this.council.CanPurchaseOrg(this.purchaseOrgAction.GetOrg()))
			{
				playerControl.StartAction(this.purchaseOrgAction);
			}
			DragManager.DestroyCurrentItem();
			this.UpdateOrgGrids(this.currentCouncilor);
			this.CloseMoveOrgPanel();
		}

		// Token: 0x06004B9A RID: 19354 RVA: 0x001FA5DC File Offset: 0x001F87DC
		private bool CouncilorOwnsOrg(TIOrgState org)
		{
			return org.hasCouncilor && org.assignedCouncilor == this.currentCouncilor;
		}

		// Token: 0x06004B9B RID: 19355 RVA: 0x001FA5FC File Offset: 0x001F87FC
		private void OrgActionFailure(string text)
		{
			DragManager.ResetCurrentItem();
			this.transferOrgAction = null;
			this.purchaseOrgAction = null;
			this.confirmMoveQueryText.SetText(text);
			this.confirmPurchase.SetActive(false);
			this.confirmSell.SetActive(false);
			this.cancelMoveOrg.SetActive(false);
			this.moveFailureOk.SetActive(true);
			this.confirmMovePanel.SetActive(true);
		}

		// Token: 0x06004B9C RID: 19356 RVA: 0x001FA664 File Offset: 0x001F8864
		public void StartMoveToCouncilOrgs(TIOrgState org)
		{
			if (!this.CouncilorOwnsOrg(org))
			{
				this.StartOrgPurchase(org);
				return;
			}
			if (!this.currentCouncilor.CanRemoveOrg_Admin(org))
			{
				this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.InsufficientAdminToRemove", new object[]
				{
					org.displayName,
					this.currentCouncilor.displayName
				}));
				return;
			}
			if (this.currentCouncilor.OrgProvidingActiveMission(org))
			{
				this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.ProvidingMission", new object[] { org.displayName }));
				return;
			}
			this.transferOrgAction = new TransferOrgToFactionPoolAction(org, this.currentCouncilor);
			this.purchaseOrgAction = null;
			this.RaiseTransferToCouncilOrgs();
		}

		// Token: 0x06004B9D RID: 19357 RVA: 0x001FA710 File Offset: 0x001F8910
		private void RaiseTransferToCouncilOrgs()
		{
			this.sb.Clear();
			this.orgCostText.SetText("");
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			this.confirmPurchaseText.SetText(Loc.T("UI.Councilor.Orgs.ConfirmTransferButton"));
			this.orgCostText.gameObject.SetActive(false);
			this.confirmMoveQueryText.SetText(Loc.T("UI.Councilor.Orgs.ConfirmTransferToPool", new object[]
			{
				this.transferOrgAction.GetOrg().displayName,
				this.transferOrgAction.GetCouncilorAssignment().displayName
			}));
			this.confirmPurchase.SetActive(true);
			this.cancelMoveOrg.SetActive(true);
			this.moveFailureOk.SetActive(false);
			this.confirmSell.SetActive(false);
			this.confirmMovePanel.SetActive(true);
		}

		// Token: 0x06004B9E RID: 19358 RVA: 0x001FA7E8 File Offset: 0x001F89E8
		public void StartSellOrg(TIOrgState org, bool useSellButton = false)
		{
			if (((org != null) ? org.assignedCouncilor : null) == null && !useSellButton)
			{
				DragManager.ResetCurrentItem();
				return;
			}
			if (((org != null) ? org.assignedCouncilor : null) == null && useSellButton && org.AllowedOnFactionMarket(base.activePlayer))
			{
				this.sellOrgAction = new SellOrgAction(org, this.currentCouncilor.faction, null);
				this.RaiseSellOrg();
				return;
			}
			if (!org.AllowedOnFactionMarket(base.activePlayer))
			{
				this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.CantSellFactionOrg", new object[] { org.displayName }));
				return;
			}
			if (this.currentCouncilor != null)
			{
				if (!this.currentCouncilor.CanRemoveOrg_Admin(org))
				{
					this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.InsufficientAdminToRemove", new object[]
					{
						org.displayName,
						this.currentCouncilor.displayName
					}));
					return;
				}
				if (this.currentCouncilor.OrgProvidingActiveMission(org))
				{
					this.OrgActionFailure(Loc.T("UI.Councilor.Orgs.ProvidingMission", new object[] { org.displayName }));
					return;
				}
				this.sellOrgAction = new SellOrgAction(org, this.currentCouncilor.faction, this.currentCouncilor);
			}
			this.RaiseSellOrg();
		}

		// Token: 0x06004B9F RID: 19359 RVA: 0x001FA924 File Offset: 0x001F8B24
		private void RaiseSellOrg()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			TIOrgState org = this.sellOrgAction.GetOrg();
			this.sellValueText.SetText(org.GetSalePrice(false).ToString("N0", false, false, null, false, FactionResource.Money));
			this.confirmSellText.SetText(Loc.T("UI.Councilor.Orgs.ConfirmSellButton"));
			this.confirmMoveQueryText.SetText(Loc.T("UI.Councilor.Orgs.ConfirmSell", new object[]
			{
				org.displayName,
				org.GetSalePrice(false).ToString("N0", false, false, null, false, FactionResource.Money)
			}));
			this.confirmSell.SetActive(true);
			this.cancelMoveOrg.SetActive(true);
			this.moveFailureOk.SetActive(false);
			this.confirmPurchase.SetActive(false);
			this.confirmMovePanel.SetActive(true);
		}

		// Token: 0x06004BA0 RID: 19360 RVA: 0x001FA9F8 File Offset: 0x001F8BF8
		private void CompleteSellOrg()
		{
			this.currentCouncilor.faction.playerControl.StartAction(this.sellOrgAction);
			DragManager.DestroyCurrentItem();
			this.UpdateOrgGrids(this.currentCouncilor);
			this.CloseMoveOrgPanel();
		}

		// Token: 0x06004BA1 RID: 19361 RVA: 0x001FAA2C File Offset: 0x001F8C2C
		private string SetStatValue(CouncilorAttribute attribute, CouncilorView councilorView)
		{
			if (attribute == CouncilorAttribute.Loyalty)
			{
				return councilorView.GetAttributeString(CouncilorAttribute.Loyalty);
			}
			int attribute2 = this.currentCouncilor.GetAttribute(attribute, true, true, true, false, false, false);
			int attribute3 = this.currentCouncilor.GetAttribute(attribute, false, true, true, false, false, false);
			if (attribute2 > attribute3)
			{
				return TIUtilities.GreenLine(attribute2.ToString());
			}
			if (attribute2 < attribute3)
			{
				return TIUtilities.RedLine(attribute2.ToString());
			}
			return attribute2.ToString();
		}

		// Token: 0x06004BA2 RID: 19362 RVA: 0x001FAA98 File Offset: 0x001F8C98
		public static string StatDetail(TICouncilorState councilor, CouncilorAttribute attribute)
		{
			return new StringBuilder(Loc.T(new StringBuilder("UI.Councilor.").Append(attribute.ToString()).Append("Tip").ToString())).AppendLine().AppendLine().AppendLine(Loc.T("UI.Councilor.BaseStat", new object[] { councilor.GetAttribute(attribute, false, true, true, false, false, false) }))
				.AppendLine(Loc.T("UI.Councilor.StatFromOrgs", new object[]
				{
					councilor.orgs.Sum<TIOrgState>((TIOrgState x) => x.GetStatBonus(attribute)).ToString("N0"),
					TIUtilities.GetAttributeString(attribute)
				}))
				.AppendLine()
				.AppendLine(Loc.T("UI.Councilor.MaxStat", new object[]
				{
					councilor.GetClampedMaxStatValue(attribute),
					TemplateManager.global.maxCouncilorAttribute.ToString("N0")
				}))
				.ToString()
				.ToString();
		}

		// Token: 0x06004BA3 RID: 19363 RVA: 0x001FABC0 File Offset: 0x001F8DC0
		private void SetCouncilorInfo()
		{
			this.dismissPanel.SetActive(false);
			this.councilorSingleGameObject.SetActive(true);
			this.customizeCouncilorPanel.SetActive(false);
			if (!TIGameState.Valid(this.currentCouncilor))
			{
				return;
			}
			this.lookingAtTurnedCouncilor = this.currentCouncilor.faction != GameControl.control.activePlayer;
			if (this.currentCouncilor.faction == null)
			{
				this.councilorSingleUITutorial.HideTutorial();
				this.DisableSingleCouncilorScreen();
				this.DisableRecruitingCanvas();
				this.UpdateGridPrimaryDisplayElements();
				return;
			}
			if (TIPlayerProfileManager.useCouncilorVideo)
			{
				this.councilorVideo.clip = GameControl.assetLoader.LoadAsset<VideoClip>(this.currentCouncilor.videoResource);
				this.councilorVideo.gameObject.SetActive(true);
				this.singleCouncilorStillImage.sprite = null;
				this.singleCouncilorStillImage.enabled = false;
				if (!this.councilorVideo.isPlaying)
				{
					TIUtilities.TryPrepareVideo(this.councilorVideo);
					base.StartCoroutine(this.PlayVideoWhenPrepared(this.councilorVideo));
				}
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(this.currentCouncilor.portraitResource, this.singleCouncilorStillImage);
				this.singleCouncilorStillImage.enabled = true;
				this.councilorVideo.Stop();
				this.councilorVideo.clip = null;
				this.councilorVideo.gameObject.SetActive(false);
			}
			if (this.currentCouncilor.faction.completedProjects.Contains(TemplateManager.Find<TIProjectTemplate>("Project_CyberneticImplants", false)))
			{
				this.traitsHeaderText.SetText(Loc.T("UI.Councilor.Traits2"));
			}
			else
			{
				this.traitsHeaderText.SetText(Loc.T("UI.Councilor.Traits"));
			}
			CouncilorView viewofCouncilor = base.activePlayer.GetViewofCouncilor(this.currentCouncilor);
			this.councilorName.text = this.currentCouncilor.displayName;
			this.councilorJob.text = this.currentCouncilor.jobDisplayName;
			this.jobTooltip.SetDelegate("BodyText", () => this.currentCouncilor.typeTemplate.description);
			this.councilorMission.SetText(this.currentCouncilor.GetCurrentMissionString(true, true, false));
			this.councilorCurrentLocation.SetText(Loc.T("UI.Councilor.Location", new object[] { TIUtilities.GetLocationString(this.currentCouncilor.location, true, false) }));
			this.councilorHomeRegion.SetText(this.currentCouncilor.GetVerboseHomeLocationString());
			this.councilorAge.SetText(this.currentCouncilor.GetVerboseAgeString());
			CouncilGridController.SetCouncilorXPText(this.currentCouncilor, this.XP, false);
			if (this.currentCouncilor.faction != null)
			{
				this.councilorFactionImage.sprite = this.currentCouncilor.faction.factionIcon128UI;
				this.councilorFactionGradient.sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(this.currentCouncilor.faction.template.gradientPath);
				this.infoMyOrgGradient.sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(this.currentCouncilor.faction.template.gradientPath);
				this.infoEquipOrgGradient.sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(this.currentCouncilor.faction.template.gradientPath);
				this.councilorFactionGradient.enabled = true;
				this.infoMyOrgGradient.enabled = true;
				this.infoEquipOrgGradient.enabled = true;
				this.councilorFactionImage.enabled = true;
			}
			else
			{
				this.councilorFactionImage.enabled = false;
				this.councilorFactionGradient.enabled = false;
				this.infoMyOrgGradient.enabled = false;
				this.infoEquipOrgGradient.enabled = false;
			}
			CouncilorIllustrationData illustrationData = this.currentCouncilor.GetIllustrationData();
			GameControl.assetLoader.LoadAssetForImageAssignment(illustrationData.illustrationPath, this.councilorBackgroundImage);
			this.councilorBackgroundImage.transform.localPosition = illustrationData.GetIllustrationLocalPosition(this.councilorBackgroundImage, this.councilorBackgroundImageInitialPosition);
			this.persuasion.SetText(this.SetStatValue(CouncilorAttribute.Persuasion, viewofCouncilor));
			this.investigation.text = this.SetStatValue(CouncilorAttribute.Investigation, viewofCouncilor);
			this.espionage.text = this.SetStatValue(CouncilorAttribute.Espionage, viewofCouncilor);
			this.command.text = this.SetStatValue(CouncilorAttribute.Command, viewofCouncilor);
			this.administration.text = this.SetStatValue(CouncilorAttribute.Administration, viewofCouncilor);
			this.science.text = this.SetStatValue(CouncilorAttribute.Science, viewofCouncilor);
			this.security.text = this.SetStatValue(CouncilorAttribute.Security, viewofCouncilor);
			this.persuasionTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.currentCouncilor, CouncilorAttribute.Persuasion));
			this.investigationTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.currentCouncilor, CouncilorAttribute.Investigation));
			this.espionageTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.currentCouncilor, CouncilorAttribute.Espionage));
			this.commandTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.currentCouncilor, CouncilorAttribute.Command));
			this.administrationTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.currentCouncilor, CouncilorAttribute.Administration));
			this.scienceTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.currentCouncilor, CouncilorAttribute.Science));
			this.securityTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.currentCouncilor, CouncilorAttribute.Security));
			if (base.activePlayer.HasIntelOnCouncilorSecrets(this.currentCouncilor))
			{
				this.LoyaltyText.SetText(Loc.T("UI.Global.Loyalty"));
				this.apparentLoyalty.text = this.SetStatValue(CouncilorAttribute.Loyalty, viewofCouncilor);
			}
			else if (base.activePlayer.lastRecordedLoyalty.ContainsKey(this.currentCouncilor))
			{
				this.LoyaltyText.SetText(Loc.T("UI.Councilor.BothLoyaltyText", new object[] { TIUtilities.RedLine(Loc.T("UI.Global.ApparentLoyalty")) }));
				this.apparentLoyalty.text = Loc.T("UI.Councilor.BothLoyalty", new object[]
				{
					this.SetStatValue(CouncilorAttribute.Loyalty, viewofCouncilor),
					base.activePlayer.lastRecordedLoyalty[this.currentCouncilor].ToString()
				});
			}
			else
			{
				this.LoyaltyText.SetText(TIUtilities.RedLine(Loc.T("UI.Global.ApparentLoyalty")));
				this.apparentLoyalty.text = this.SetStatValue(CouncilorAttribute.Loyalty, viewofCouncilor);
			}
			this.moneyIncome.SetText(TIUtilities.FormatSmallNumber(this.currentCouncilor.GetMonthlyIncome(FactionResource.Money), 2, 0, true, false));
			this.influenceIncome.SetText(TIUtilities.FormatSmallNumber(this.currentCouncilor.GetMonthlyIncome(FactionResource.Influence), 2, 0, true, false));
			this.opsIncome.SetText(TIUtilities.FormatSmallNumber(this.currentCouncilor.GetMonthlyIncome(FactionResource.Operations), 2, 0, true, false));
			this.researchIncome.SetText(TIUtilities.FormatSmallNumber(this.currentCouncilor.GetMonthlyIncome(FactionResource.Research), 2, 0, true, false));
			this.boostIncome.SetText(TIUtilities.FormatSmallNumber(this.currentCouncilor.GetMonthlyIncome(FactionResource.Boost), 2, 0, true, false));
			this.mCIncome.text = this.currentCouncilor.GetMonthlyIncome(FactionResource.MissionControl).ToString("N0");
			this.projectsIncome.text = this.currentCouncilor.projectContributionString;
			if (viewofCouncilor.agentForFaction != null)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("councilor_missions/ICO_turn_off", this.statusIcon);
				this.statusText.SetText(Loc.T("UI.Councilor.Turned"));
				string text = Loc.T("UI.Councilor.TurnedTooltip", new object[]
				{
					this.currentCouncilor.faction.displayName,
					viewofCouncilor.agentForFaction.displayNameWithColor
				});
				if (viewofCouncilor.agentForFaction == base.activePlayer)
				{
					text = new StringBuilder(text).Append(" ").AppendLine(Loc.T("UI.Councilor.AddressTraitor")).ToString();
				}
				this.statusTooltip.SetText("BodyText", text);
				this.statusTooltipCopy.SetText("BodyText", text);
				this.statusText.gameObject.SetActive(true);
				this.statusIcon.gameObject.SetActive(true);
			}
			else if (this.currentCouncilor.detained)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("councilor_missions/ICO_detain_off", this.statusIcon);
				this.statusText.SetText(Loc.T("UI.Councilor.Detained"));
				string text2;
				if (this.currentCouncilor.detainingFaction == this.currentCouncilor.faction)
				{
					text2 = Loc.T("UI.Councilor.SelfDetainedTooltip", new object[] { this.currentCouncilor.detainedReleaseDate.ToCustomDateString() });
				}
				else
				{
					text2 = Loc.T("UI.Councilor.DetainedTooltip", new object[]
					{
						this.currentCouncilor.detainingFaction.displayNameWithColor,
						this.currentCouncilor.detainedReleaseDate.ToCustomDateString()
					});
				}
				if (this.currentCouncilor.faction == base.activePlayer)
				{
					if (this.currentCouncilor.detainingFaction == this.currentCouncilor.faction)
					{
						text2 = new StringBuilder(text2).AppendLine().AppendLine().AppendLine(Loc.T("UI.Councilor.AddressSelfDetained"))
							.ToString();
					}
					else
					{
						text2 = new StringBuilder(text2).AppendLine().AppendLine().AppendLine(Loc.T("UI.Councilor.AddressDetained"))
							.ToString();
					}
				}
				this.statusTooltip.SetText("BodyText", text2);
				this.statusTooltipCopy.SetText("BodyText", text2);
				this.statusText.gameObject.SetActive(true);
				this.statusIcon.gameObject.SetActive(true);
			}
			else
			{
				this.statusText.gameObject.SetActive(false);
				this.statusIcon.gameObject.SetActive(false);
			}
			this.spendXPButton.interactable = !this.lookingAtTurnedCouncilor && !this.spendXPPanel.activeSelf;
			this.customizeCouncilorButton.interactable = !this.lookingAtTurnedCouncilor;
			bool flag = !this.lookingAtTurnedCouncilor || (this.lookingAtTurnedCouncilor && base.activePlayer.turnedCouncilors.Count > 1);
			this.cycleCouncilorLeftButton.interactable = flag;
			this.cycleCouncilorRightButton.interactable = flag;
			this.dismissButton.interactable = !GameStateManager.AllFactions().Any<TIFactionState>((TIFactionState x) => x.planningMissions) && !TIPromptQueueState.ActivePlayerHasSaveBlockingPrompt() && (this.currentCouncilor.faction == base.activePlayer || this.currentCouncilor.agentForFaction == base.activePlayer);
			this.equipOrgGrid.SetActive(!this.lookingAtTurnedCouncilor);
			this.UpdateMissionsList(this.currentCouncilor);
			this.UpdateTraitsList(this.currentCouncilor);
			this.councilorOrgGridTitle.SetText(Loc.T("UI.Councilor.OrgGridTitle", new object[]
			{
				this.currentCouncilor.orgsWeight.ToString(),
				this.currentCouncilor.GetAttribute(CouncilorAttribute.Administration, true, true, true, true, false, false).ToString(),
				this.currentCouncilor.orgs.Count.ToString(),
				TemplateManager.global.councilorMaxOrgs.ToString()
			}));
			this.UpdateOrgGrids(this.currentCouncilor);
			this.confirmSpendXPSelectionPanel.SetActive(false);
			this.SelectAugementationListItem(null);
			if (this.currentCouncilor.faction == base.activePlayer)
			{
				this.dismissButtonText.SetText(Loc.T("UI.Councilor.DismissButton"));
			}
			else
			{
				this.dismissButtonText.SetText(Loc.T("UI.Councilor.DismissEnemyButton"));
			}
			this.HideInfoMyOrg();
			this.HideInfoEquipOrg();
		}

		// Token: 0x06004BA4 RID: 19364 RVA: 0x001FB75C File Offset: 0x001F995C
		public static void SetCouncilorXPText(TICouncilorState councilor, TMP_Text xpText, bool withXPText = false)
		{
			string text = (withXPText ? "UI.Councilor.XPCost" : "UI.Councilor.XP");
			if (!(councilor.faction == GameControl.control.activePlayer))
			{
				xpText.SetText(TIUtilities.HeaderCyanLine(Loc.T(text, new object[] { councilor.XP.ToString() })));
				return;
			}
			if (!councilor.CanAffordAnyCandidateAugmentations(true))
			{
				xpText.SetText(TIUtilities.RedLine(Loc.T(text, new object[] { councilor.XP.ToString() })));
				return;
			}
			if ((float)councilor.XP >= (float)TemplateManager.global.XPToLevelUp * (1f + councilor.XPModifier))
			{
				xpText.SetText(TIUtilities.GreenLine(Loc.T(text, new object[] { councilor.XP.ToString() })));
				return;
			}
			xpText.SetText(TIUtilities.YellowLine(Loc.T(text, new object[] { councilor.XP.ToString() })));
		}

		// Token: 0x06004BA5 RID: 19365 RVA: 0x001FB854 File Offset: 0x001F9A54
		public void ShowInfoMyOrg(string name, string desc, Sprite icon, string tier)
		{
			this.infoMyOrgInfoPanel.SetActive(true);
			this.myOrgGridRect.sizeDelta = new Vector2(525f, this.myOrgGridRect.sizeDelta.y);
			this.infoMyOrgTitle.SetText(name);
			this.infoMyOrgDesc.SetText(desc);
			this.infoMyOrgIcon.sprite = icon;
			this.infoMyOrgTier.SetText(tier);
			this.councilorOrgGrid.gameObject.GetComponent<GridLayoutGroup>().constraintCount = 4;
		}

		// Token: 0x06004BA6 RID: 19366 RVA: 0x001FB8DC File Offset: 0x001F9ADC
		public void ShowInfoEquipOrg(string name, string cost, string desc, Sprite icon, string tier, bool marketplace = false)
		{
			this.infoEquipInfoPanel.SetActive(true);
			this.equipOrgGridRect.sizeDelta = new Vector2(525f, this.equipOrgGridRect.sizeDelta.y);
			this.infoEquipOrgTitle.SetText(name);
			this.infoEquipOrgCost.SetText(cost);
			this.infoEquipOrgDesc.SetText(desc);
			this.infoEquipOrgIcon.sprite = icon;
			this.infoEquipOrgTier.SetText(tier);
			this.infoEquipOrgHeader.SetText(Loc.T(marketplace ? "UI.Councilor.MarketplaceOrg" : "UI.Councilor.UnassignedOrg"));
			this.councilOrgGrid.gameObject.GetComponent<GridLayoutGroup>().constraintCount = 4;
			this.availableOrgGrid.gameObject.GetComponent<GridLayoutGroup>().constraintCount = 4;
		}

		// Token: 0x06004BA7 RID: 19367 RVA: 0x001FB9A4 File Offset: 0x001F9BA4
		public void OnHideInfoMyOrgClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.HideInfoMyOrg();
		}

		// Token: 0x06004BA8 RID: 19368 RVA: 0x001FB9B8 File Offset: 0x001F9BB8
		public void OnHideInfoEquipOrgClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.HideInfoEquipOrg();
		}

		// Token: 0x06004BA9 RID: 19369 RVA: 0x001FB9CC File Offset: 0x001F9BCC
		public void HideInfoMyOrg()
		{
			this.infoMyOrgInfoPanel.SetActive(false);
			this.myOrgGridRect.sizeDelta = new Vector2(781f, this.myOrgGridRect.sizeDelta.y);
			this.councilorOrgGrid.gameObject.GetComponent<GridLayoutGroup>().constraintCount = 6;
			this.SetOrgSelected(null, true);
		}

		// Token: 0x06004BAA RID: 19370 RVA: 0x001FBA28 File Offset: 0x001F9C28
		public void HideInfoEquipOrg()
		{
			this.infoEquipInfoPanel.SetActive(false);
			this.equipOrgGridRect.sizeDelta = new Vector2(781f, this.equipOrgGridRect.sizeDelta.y);
			this.councilOrgGrid.gameObject.GetComponent<GridLayoutGroup>().constraintCount = 6;
			this.availableOrgGrid.gameObject.GetComponent<GridLayoutGroup>().constraintCount = 6;
			this.SetOrgSelected(null, false);
		}

		// Token: 0x06004BAB RID: 19371 RVA: 0x001FBA9A File Offset: 0x001F9C9A
		public void OnClickOrgActionBottom()
		{
			if (!this.lookingAtTurnedCouncilor)
			{
				this.selectedOrgBottom.OnRightClickItem();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004BAC RID: 19372 RVA: 0x001FBABC File Offset: 0x001F9CBC
		public void OnClickOrgActionBottom2()
		{
			if (!this.lookingAtTurnedCouncilor)
			{
				this.StartSellOrg(this.selectedOrgBottom.GetOrg(), true);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004BAD RID: 19373 RVA: 0x001FBAE5 File Offset: 0x001F9CE5
		public void OnClickOrgActionTop()
		{
			if (!this.lookingAtTurnedCouncilor)
			{
				this.StartMoveToCouncilOrgs(this.selectedOrgTop.GetOrg());
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004BAE RID: 19374 RVA: 0x001FBB0D File Offset: 0x001F9D0D
		public void OnClickOrgActionTopSell()
		{
			if (!this.lookingAtTurnedCouncilor)
			{
				this.StartSellOrg(this.selectedOrgTop.GetOrg(), true);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004BAF RID: 19375 RVA: 0x001FBB36 File Offset: 0x001F9D36
		public void UpdateMissionsList(TICouncilorState councilorState)
		{
			if (this.missionsList != null)
			{
				this.UpdateMissionsList(this.missionsList, councilorState);
			}
		}

		// Token: 0x06004BB0 RID: 19376 RVA: 0x001FBB53 File Offset: 0x001F9D53
		public void OnToggleAutomateModeSettingsClicked()
		{
			this.UpdateMissionsList(this.currentCouncilor);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}

		// Token: 0x06004BB1 RID: 19377 RVA: 0x001FBB70 File Offset: 0x001F9D70
		public void UpdateTraitsList(TICouncilorState councilorState)
		{
			if (this.traitsList != null)
			{
				this.traitsList.SetListSize<TraitsListItemController>(councilorState.traits.Count, false, false);
				int num = 0;
				using (IEnumerator<object> enumerator = this.traitsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CouncilGridController.<>o__363.<>p__0 == null)
						{
							CouncilGridController.<>o__363.<>p__0 = CallSite<Func<CallSite, object, TraitsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TraitsListItemController), typeof(CouncilGridController)));
						}
						TraitsListItemController traitsListItemController = CouncilGridController.<>o__363.<>p__0.Target(CouncilGridController.<>o__363.<>p__0, enumerator.Current);
						if (traitsListItemController != null)
						{
							traitsListItemController.UpdateListItem(councilorState.traits[num], num, num == this.traitsList.size - 1);
							traitsListItemController.gameObject.SetActive(true);
							num++;
						}
					}
				}
			}
		}

		// Token: 0x06004BB2 RID: 19378 RVA: 0x001FBC64 File Offset: 0x001F9E64
		private void UpdateOrgGrids(TICouncilorState councilorState)
		{
			if (this.availableOrgGrid != null)
			{
				this.UpdateOrgGrid(this.availableOrgGrid, councilorState.faction.availableOrgs);
			}
			if (this.councilOrgGrid != null)
			{
				this.UpdateOrgGrid(this.councilOrgGrid, councilorState.faction.unassignedOrgs);
			}
			if (this.councilorOrgGrid != null)
			{
				this.UpdateOrgGrid(this.councilorOrgGrid, councilorState.orgs);
			}
		}

		// Token: 0x06004BB3 RID: 19379 RVA: 0x001FBCDC File Offset: 0x001F9EDC
		private void UpdateOrgGrid(ListManagerBase orgGrid, IReadOnlyList<TIOrgState> orgs)
		{
			int num = base.activePlayer.UnassignedPoolOverage();
			if (num > 0)
			{
				this.factionOrgsHeaderText.SetText(TIUtilities.RedLine(new StringBuilder(Loc.T("UI.Councilor.OrgsGarage")).Append(Loc.T("UI.Councilor.GarageFull", new object[] { num })).ToString()));
			}
			else
			{
				this.factionOrgsHeaderText.SetText(Loc.T("UI.Councilor.OrgsGarage"));
			}
			orgGrid.SetListSize<OrgItemView>(orgs.Count, false, false);
			int num2 = 0;
			using (IEnumerator<object> enumerator = orgGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__365.<>p__0 == null)
					{
						CouncilGridController.<>o__365.<>p__0 = CallSite<Func<CallSite, object, OrgItemView>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrgItemView), typeof(CouncilGridController)));
					}
					OrgItemView orgItemView = CouncilGridController.<>o__365.<>p__0.Target(CouncilGridController.<>o__365.<>p__0, enumerator.Current);
					orgItemView.gameObject.SetActive(true);
					orgItemView.councilorController = this;
					orgItemView.UpdateOrgItem(orgs[num2++], this.currentCouncilor);
				}
			}
		}

		// Token: 0x06004BB4 RID: 19380 RVA: 0x001FBE00 File Offset: 0x001FA000
		public void SetOrgSelected(OrgItemView toSelect, bool isEquipped)
		{
			if (isEquipped)
			{
				foreach (OrgItemView orgItemView in this.councilorOrgGrid.GetComponentsInChildren<OrgItemView>(true))
				{
					orgItemView.SetButtonHighlight(orgItemView.Equals(toSelect));
				}
				return;
			}
			foreach (OrgItemView orgItemView2 in this.councilOrgGrid.GetComponentsInChildren<OrgItemView>(true))
			{
				orgItemView2.SetButtonHighlight(orgItemView2.Equals(toSelect));
			}
			foreach (OrgItemView orgItemView3 in this.availableOrgGrid.GetComponentsInChildren<OrgItemView>(true))
			{
				orgItemView3.SetButtonHighlight(orgItemView3.Equals(toSelect));
			}
		}

		// Token: 0x06004BB5 RID: 19381 RVA: 0x001FBE8F File Offset: 0x001FA08F
		public void SetOrgCouncilTab()
		{
			this.orgCouncilTabActive = true;
			this.UpdateOrgGrids(this.currentCouncilor);
		}

		// Token: 0x06004BB6 RID: 19382 RVA: 0x001FBEA4 File Offset: 0x001FA0A4
		public void SetOrgAvailableTab()
		{
			this.orgCouncilTabActive = false;
			this.UpdateOrgGrids(this.currentCouncilor);
		}

		// Token: 0x06004BB7 RID: 19383 RVA: 0x001FBEB9 File Offset: 0x001FA0B9
		private void OnFactionResourcesUpdated(FactionResourcesUpdated e)
		{
			if (!e.council.isActivePlayer)
			{
				return;
			}
			if (this.spendXPPanel.activeSelf && this.currentCouncilor != null)
			{
				this.SetAugmentationPanel();
			}
		}

		// Token: 0x06004BB8 RID: 19384 RVA: 0x001FBEEC File Offset: 0x001FA0EC
		private void SetAugmentationPanel()
		{
			if (this.currentCouncilor == null)
			{
				this.SelectAugementationListItem(null);
				return;
			}
			List<CouncilorAugmentationOption> list = (from x in this.currentCouncilor.GetCandidateAugmentations()
				orderby x.CouncilorCanAfford(this.currentCouncilor) descending
				select x).ToList<CouncilorAugmentationOption>();
			this.augmentationList.SetListSize<CouncilorAugmentationListItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.augmentationList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__370.<>p__0 == null)
					{
						CouncilGridController.<>o__370.<>p__0 = CallSite<Func<CallSite, object, CouncilorAugmentationListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorAugmentationListItemController), typeof(CouncilGridController)));
					}
					CouncilorAugmentationListItemController councilorAugmentationListItemController = CouncilGridController.<>o__370.<>p__0.Target(CouncilGridController.<>o__370.<>p__0, enumerator.Current);
					councilorAugmentationListItemController.Init(this, this.currentCouncilor, list[num++]);
					councilorAugmentationListItemController.UpdateListItem();
				}
			}
			this.SelectAugementationListItem(null);
		}

		// Token: 0x06004BB9 RID: 19385 RVA: 0x001FBFE4 File Offset: 0x001FA1E4
		public void OnAugmentationSelected(CouncilorAugmentationOption option, CouncilorAugmentationListItemController listItem)
		{
			this.selectedAugmentation = option;
			this.confirmSpendXPSelectionPanel.SetActive(true);
			string text;
			string text2;
			string text3;
			string text4;
			this.selectedAugmentation.SetAugmentationStrings(out text, out text2, out text3, out text4);
			this.confirmSpendXPPrompt.SetText(Loc.T("UI.Councilor.ConfirmAugmentationPrompt", new object[] { text, text2, text4 }));
			this.SelectAugementationListItem(listItem);
		}

		// Token: 0x06004BBA RID: 19386 RVA: 0x001FC048 File Offset: 0x001FA248
		private void SelectAugementationListItem(CouncilorAugmentationListItemController listItem)
		{
			using (IEnumerator<object> enumerator = this.augmentationList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__372.<>p__0 == null)
					{
						CouncilGridController.<>o__372.<>p__0 = CallSite<Func<CallSite, object, CouncilorAugmentationListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorAugmentationListItemController), typeof(CouncilGridController)));
					}
					CouncilorAugmentationListItemController councilorAugmentationListItemController = CouncilGridController.<>o__372.<>p__0.Target(CouncilGridController.<>o__372.<>p__0, enumerator.Current);
					councilorAugmentationListItemController.SetSelected(listItem != null && councilorAugmentationListItemController == listItem);
				}
			}
		}

		// Token: 0x06004BBB RID: 19387 RVA: 0x001FC0E8 File Offset: 0x001FA2E8
		public void OnAugmentationButtonConfirmed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.currentCouncilor.faction.playerControl.StartAction(new AugmentCouncilorAction(this.currentCouncilor, this.selectedAugmentation));
			this.confirmSpendXPSelectionPanel.SetActive(false);
			this.SelectAugementationListItem(null);
			this.SetAugmentationPanel();
		}

		// Token: 0x06004BBC RID: 19388 RVA: 0x001FC140 File Offset: 0x001FA340
		public void OnAugmentationButtonDeclined()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
			this.confirmSpendXPSelectionPanel.SetActive(false);
			this.SelectAugementationListItem(null);
		}

		// Token: 0x06004BBD RID: 19389 RVA: 0x001FC164 File Offset: 0x001FA364
		public void OnExitAugmentationMenuSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.confirmSpendXPSelectionPanel.SetActive(false);
			this.SelectAugementationListItem(null);
			this.spendXPPanel.SetActive(false);
			this.missionListObject.SetActive(true);
			this.spendXPButton.interactable = !this.lookingAtTurnedCouncilor && !this.spendXPPanel.activeSelf;
			if (TIPlayerProfileManager.useCouncilorVideo && !this.councilorVideo.isPlaying)
			{
				TIUtilities.TryPlayVideo(this.councilorVideo);
			}
		}

		// Token: 0x06004BBE RID: 19390 RVA: 0x001FC1EC File Offset: 0x001FA3EC
		public void OnCloseCustomizeCouncilorClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.customizeCouncilorPanel.SetActive(false);
			this.councilorSingleGameObject.SetActive(true);
			this.HideAllTutorials();
			this.councilorSingleUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Detail, false, true);
			if (!this.councilorVideo.isPlaying)
			{
				TIUtilities.TryPlayVideo(this.councilorVideo);
			}
		}

		// Token: 0x06004BBF RID: 19391 RVA: 0x001FC250 File Offset: 0x001FA450
		private void CacheCouncilorPortraits()
		{
			List<TICouncilorAppearanceTemplate> list = (from x in TemplateManager.IterateByClass<TICouncilorAppearanceTemplate>(true)
				where x.enable && !x.allowedAncestries.Contains(CouncilorAncestry.Alien)
				select x).ToList<TICouncilorAppearanceTemplate>();
			this.councilorAppearanceGrid.SetListSize<CouncilorAppearanceGridItem>(list.Count * 2, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.councilorAppearanceGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__422.<>p__0 == null)
					{
						CouncilGridController.<>o__422.<>p__0 = CallSite<Func<CallSite, object, CouncilorAppearanceGridItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorAppearanceGridItem), typeof(CouncilGridController)));
					}
					CouncilorAppearanceGridItem councilorAppearanceGridItem = CouncilGridController.<>o__422.<>p__0.Target(CouncilGridController.<>o__422.<>p__0, enumerator.Current);
					councilorAppearanceGridItem.gameObject.SetActive(false);
					councilorAppearanceGridItem.SetListItem(this, list[num / 2], num % 2 == 0);
					num++;
				}
			}
			this.councilorPortraitsCached = true;
			this.jobTemplates = (from x in TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true)
				where x.weight > 0f
				select x).ToList<TICouncilorTypeTemplate>();
			this.ancestrySettings = new Dictionary<CouncilorAncestry, string>
			{
				{
					CouncilorAncestry.None,
					Loc.T("UI.Councilor.CustomizeAny")
				},
				{
					CouncilorAncestry.African,
					Loc.T("UI.Councilor.CustomizeGroup1")
				},
				{
					CouncilorAncestry.Asian,
					Loc.T("UI.Councilor.CustomizeGroup2")
				},
				{
					CouncilorAncestry.EastAsian,
					Loc.T("UI.Councilor.CustomizeGroup3")
				},
				{
					CouncilorAncestry.European,
					Loc.T("UI.Councilor.CustomizeGroup4")
				},
				{
					CouncilorAncestry.Hispanic,
					Loc.T("UI.Councilor.CustomizeGroup5")
				},
				{
					CouncilorAncestry.Oceanic,
					Loc.T("UI.Councilor.CustomizeGroup6")
				}
			};
			this.genderSettings = new Dictionary<CouncilorGender, string>
			{
				{
					CouncilorGender.Female,
					Loc.T("UI.Councilor.Female")
				},
				{
					CouncilorGender.Male,
					Loc.T("UI.Councilor.Male")
				},
				{
					CouncilorGender.Nonbinary,
					Loc.T("UI.Councilor.CustomizeAny")
				}
			};
			this.duplicateSettings = new Dictionary<bool, string>
			{
				{
					false,
					Loc.T("UI.Councilor.CustomizeDuplicate")
				},
				{
					true,
					Loc.T("UI.Councilor.CustomizeUnused")
				}
			};
			this.voiceTemplates = (from x in TemplateManager.IterateByClass<TICouncilorVoiceTemplate>(true)
				where x.enable
				select x).ToList<TICouncilorVoiceTemplate>();
			int num2 = 0;
			foreach (TICouncilorVoiceTemplate ticouncilorVoiceTemplate in this.voiceTemplates)
			{
				string category = ticouncilorVoiceTemplate.category;
				if (!this.allAccentOptions.Values.Contains(category))
				{
					this.allAccentOptions.Add(num2++, category);
				}
			}
		}

		// Token: 0x06004BC0 RID: 19392 RVA: 0x001FC50C File Offset: 0x001FA70C
		public void InitializeCustomizationOptions()
		{
			if (!this.councilorPortraitsCached)
			{
				this.CacheCouncilorPortraits();
			}
			this.proposedGivenName = this.currentCouncilor.personalName;
			this.proposedFamilyName = this.currentCouncilor.familyName;
			this.givenNameEntry.text = this.proposedGivenName;
			this.familyNameEntry.text = this.proposedFamilyName;
			this.proposedCouncilorAppearance = this.currentCouncilor.appearanceTemplate;
			this.proposedVoice = this.currentCouncilor.voiceTemplate;
			this.accentSetting = this.allAccentOptions.Keys.First<int>((int x) => this.allAccentOptions[x] == this.proposedVoice.category);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.currentCouncilor.portraitResource, this.councilorImage);
			this.filterForGender = (this.currentCouncilor.appearanceTemplate.allowedGenders.Contains(this.currentCouncilor.gender) ? this.currentCouncilor.gender : CouncilorGender.Nonbinary);
			this.ancestrySetting = (int)this.filterForAncestry;
			this.genderSetting = (int)this.filterForGender;
			this.voiceIndexSetting = this.currentCouncilor.voiceTemplate.index;
			this.filterForJob = null;
			this.filterForDuplicates = true;
			this.councilorProfessionText.SetText(this.currentCouncilor.typeTemplate.displayName);
			this.councilorHomeRegionText.SetText(this.currentCouncilor.GetVerboseHomeLocationString());
			this.UpdateNameSettings();
			this.UpdateFilteredCouncilorAppearanceGrid();
			this.SetCurrentAccentOptions(true);
			this.confirmChangeBioButton.interactable = false;
		}

		// Token: 0x06004BC1 RID: 19393 RVA: 0x001FC68C File Offset: 0x001FA88C
		public void UpdateNameSettings()
		{
			this.councilorNameText.SetText(new StringBuilder(this.proposedGivenName).Append(" ").Append(this.proposedFamilyName));
		}

		// Token: 0x06004BC2 RID: 19394 RVA: 0x001FC6BC File Offset: 0x001FA8BC
		public void UpdateFilteredCouncilorAppearanceGrid()
		{
			List<string> list = TICouncilorAppearanceTemplate.AppearanceTemplatesInUse();
			list.Remove(this.currentCouncilor.appearanceTemplateName);
			using (IEnumerator<object> enumerator = this.councilorAppearanceGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__425.<>p__0 == null)
					{
						CouncilGridController.<>o__425.<>p__0 = CallSite<Func<CallSite, object, CouncilorAppearanceGridItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorAppearanceGridItem), typeof(CouncilGridController)));
					}
					CouncilorAppearanceGridItem councilorAppearanceGridItem = CouncilGridController.<>o__425.<>p__0.Target(CouncilGridController.<>o__425.<>p__0, enumerator.Current);
					if (councilorAppearanceGridItem.old == this.currentCouncilor.useOldPortrait && (this.filterForGender == CouncilorGender.None || this.filterForGender == CouncilorGender.Nonbinary || councilorAppearanceGridItem.template.allowedGenders.Contains(this.filterForGender)) && (this.filterForAncestry == CouncilorAncestry.None || councilorAppearanceGridItem.template.allowedAncestries.Contains(this.filterForAncestry)) && (this.filterForJob == null || councilorAppearanceGridItem.template.allowedJobs.Contains(this.filterForJob)) && (!this.filterForDuplicates || !list.Contains(councilorAppearanceGridItem.template.dataName)))
					{
						councilorAppearanceGridItem.gameObject.SetActive(true);
					}
					else
					{
						councilorAppearanceGridItem.gameObject.SetActive(false);
					}
				}
			}
			this.ancestryFilterSetting.SetText(this.ancestrySettings[this.filterForAncestry]);
			this.genderFilterSetting.SetText(this.genderSettings[this.filterForGender]);
			this.duplicateFilterSetting.SetText(this.duplicateSettings[this.filterForDuplicates]);
			this.jobFilterSetting.SetText((this.filterForJob == null) ? Loc.T("UI.Councilor.CustomizeAny") : this.filterForJob.displayName);
			this.voiceAccentFilterSetting.SetText(this.proposedVoice.displayName);
			this.voiceIndexFilterSetting.SetText(this.proposedVoice.displayIdx);
		}

		// Token: 0x06004BC3 RID: 19395 RVA: 0x001FC8CC File Offset: 0x001FAACC
		public void OnNewAppearanceSelected(TICouncilorAppearanceTemplate proposedTemplate)
		{
			if (this.proposedCouncilorAppearance != proposedTemplate)
			{
				this.proposedCouncilorAppearance = proposedTemplate;
				GameControl.assetLoader.LoadAssetForImageAssignment(this.currentCouncilor.useOldPortrait ? this.proposedCouncilorAppearance.portraitOld : this.proposedCouncilorAppearance.portraitYoung, this.councilorImage);
				this.confirmChangeBioButton.interactable = true;
			}
		}

		// Token: 0x06004BC4 RID: 19396 RVA: 0x001FC92A File Offset: 0x001FAB2A
		public void OnCycleAncestryFilterRight()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.ancestrySetting++;
			if (this.ancestrySetting > 6)
			{
				this.ancestrySetting = 0;
			}
			this.filterForAncestry = (CouncilorAncestry)this.ancestrySetting;
			this.UpdateFilteredCouncilorAppearanceGrid();
		}

		// Token: 0x06004BC5 RID: 19397 RVA: 0x001FC968 File Offset: 0x001FAB68
		public void OnCycleAncestryFilterLeft()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.ancestrySetting--;
			if (this.ancestrySetting < 0)
			{
				this.ancestrySetting = (int)this.ancestrySettings.Keys.Max<CouncilorAncestry>();
			}
			this.filterForAncestry = (CouncilorAncestry)this.ancestrySetting;
			this.UpdateFilteredCouncilorAppearanceGrid();
		}

		// Token: 0x06004BC6 RID: 19398 RVA: 0x001FC9C0 File Offset: 0x001FABC0
		public void OnCycleGenderFilterRight()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.genderSetting++;
			if (this.genderSetting > 3)
			{
				this.genderSetting = 1;
			}
			this.filterForGender = (CouncilorGender)this.genderSetting;
			this.SetCurrentAccentOptions(false);
			this.UpdateFilteredCouncilorAppearanceGrid();
		}

		// Token: 0x06004BC7 RID: 19399 RVA: 0x001FCA10 File Offset: 0x001FAC10
		public void OnCycleGenderFilterLeft()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.genderSetting--;
			if (this.genderSetting < 1)
			{
				this.genderSetting = 3;
			}
			this.filterForGender = (CouncilorGender)this.genderSetting;
			this.SetCurrentAccentOptions(false);
			this.UpdateFilteredCouncilorAppearanceGrid();
		}

		// Token: 0x06004BC8 RID: 19400 RVA: 0x001FCA60 File Offset: 0x001FAC60
		public void OnCycleJobFilterRight()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.jobSetting++;
			if (this.jobSetting >= this.jobTemplates.Count)
			{
				this.jobSetting = 0;
				this.filterForJob = null;
			}
			else
			{
				this.filterForJob = this.jobTemplates[this.jobSetting - 1];
			}
			this.UpdateFilteredCouncilorAppearanceGrid();
		}

		// Token: 0x06004BC9 RID: 19401 RVA: 0x001FCACC File Offset: 0x001FACCC
		public void OnCycleJobFilterLeft()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.jobSetting--;
			if (this.jobSetting == 0)
			{
				this.filterForJob = null;
				return;
			}
			if (this.jobSetting < 0)
			{
				this.jobSetting = this.jobTemplates.Count;
			}
			this.filterForJob = this.jobTemplates[this.jobSetting - 1];
			this.UpdateFilteredCouncilorAppearanceGrid();
		}

		// Token: 0x06004BCA RID: 19402 RVA: 0x001FCB3C File Offset: 0x001FAD3C
		public void OnCycleDuplicateFilterRight()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.filterForDuplicates = !this.filterForDuplicates;
			this.UpdateFilteredCouncilorAppearanceGrid();
		}

		// Token: 0x06004BCB RID: 19403 RVA: 0x001FCB5F File Offset: 0x001FAD5F
		public void OnCycleDuplicateFilterLeft()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.filterForDuplicates = !this.filterForDuplicates;
			this.UpdateFilteredCouncilorAppearanceGrid();
		}

		// Token: 0x06004BCC RID: 19404 RVA: 0x001FCB82 File Offset: 0x001FAD82
		private void PlayCouncilorSample()
		{
			if (this.voicePreviewCoroutine != null)
			{
				base.StopCoroutine(this.voicePreviewCoroutine);
			}
			this.voicePreviewCoroutine = base.StartCoroutine(this.PlayCouncilorSampleAfterDelay());
		}

		// Token: 0x06004BCD RID: 19405 RVA: 0x001FCBAA File Offset: 0x001FADAA
		private IEnumerator PlayCouncilorSampleAfterDelay()
		{
			yield return new WaitForSeconds(0.2f);
			this.proposedVoice.PlayMissionVoice(this.testVoiceTemplate, TICouncilorVoiceTemplate.VoiceMissionSituation.Assigned, true, false);
			this.voicePreviewCoroutine = null;
			yield break;
		}

		// Token: 0x06004BCE RID: 19406 RVA: 0x001FCBBC File Offset: 0x001FADBC
		private void SetCurrentAccentOptions(bool playVoicePreview = true)
		{
			this.currentAccentOptions.Clear();
			string text = this.allAccentOptions[this.accentSetting];
			CouncilorGender councilorGender = this.filterForGender;
			if (councilorGender != CouncilorGender.Female)
			{
				if (councilorGender == CouncilorGender.Male)
				{
					text = new StringBuilder(text).Append("_M").ToString();
				}
			}
			else
			{
				text = new StringBuilder(text).Append("_F").ToString();
			}
			List<TICouncilorVoiceTemplate> tempList = new List<TICouncilorVoiceTemplate>();
			foreach (TICouncilorVoiceTemplate ticouncilorVoiceTemplate in this.voiceTemplates)
			{
				if (ticouncilorVoiceTemplate.dataName.StartsWith(text))
				{
					tempList.Add(ticouncilorVoiceTemplate);
				}
			}
			tempList = tempList.OrderBy<TICouncilorVoiceTemplate, string>((TICouncilorVoiceTemplate x) => x.dataName).ToList<TICouncilorVoiceTemplate>();
			this.currentAccentOptions = tempList.ToDictionary<TICouncilorVoiceTemplate, int, TICouncilorVoiceTemplate>((TICouncilorVoiceTemplate x) => tempList.IndexOf(x), (TICouncilorVoiceTemplate y) => y);
			if (!tempList.Contains(this.proposedVoice))
			{
				this.voiceIndexSetting = 0;
				this.proposedVoice = tempList[this.voiceIndexSetting];
				this.SetProposedVoice(playVoicePreview);
				return;
			}
			this.voiceIndexSetting = this.currentAccentOptions.First<KeyValuePair<int, TICouncilorVoiceTemplate>>((KeyValuePair<int, TICouncilorVoiceTemplate> x) => x.Value == this.proposedVoice).Key;
		}

		// Token: 0x06004BCF RID: 19407 RVA: 0x001FCD68 File Offset: 0x001FAF68
		public void OnCycleAccentLeft()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.accentSetting--;
			if (this.accentSetting < 0)
			{
				this.accentSetting = this.allAccentOptions.Count - 1;
			}
			this.voiceIndexSetting = 0;
			this.SetCurrentAccentOptions(true);
		}

		// Token: 0x06004BD0 RID: 19408 RVA: 0x001FCDBC File Offset: 0x001FAFBC
		public void OnCycleAccentRight()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.accentSetting++;
			if (this.accentSetting > this.allAccentOptions.Count - 1)
			{
				this.accentSetting = 0;
			}
			this.voiceIndexSetting = 0;
			this.SetCurrentAccentOptions(true);
		}

		// Token: 0x06004BD1 RID: 19409 RVA: 0x001FCE0D File Offset: 0x001FB00D
		public void OnCycleVoiceIndexLeft()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.voiceIndexSetting--;
			if (this.voiceIndexSetting < 0)
			{
				this.voiceIndexSetting = this.currentAccentOptions.Count - 1;
			}
			this.SetProposedVoice(true);
		}

		// Token: 0x06004BD2 RID: 19410 RVA: 0x001FCE4C File Offset: 0x001FB04C
		public void OnCycleVoiceIndexRight()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.voiceIndexSetting++;
			if (this.voiceIndexSetting > this.currentAccentOptions.Count - 1)
			{
				this.voiceIndexSetting = 0;
			}
			this.SetProposedVoice(true);
		}

		// Token: 0x06004BD3 RID: 19411 RVA: 0x001FCE8C File Offset: 0x001FB08C
		public void SetProposedVoice(bool playVoicePreview = true)
		{
			this.proposedVoice = this.currentAccentOptions[this.voiceIndexSetting];
			this.voiceAccentFilterSetting.SetText(this.proposedVoice.displayName);
			this.voiceIndexFilterSetting.SetText(this.proposedVoice.displayIdx);
			if (playVoicePreview)
			{
				this.PlayCouncilorSample();
			}
			this.confirmChangeBioButton.interactable = true;
		}

		// Token: 0x06004BD4 RID: 19412 RVA: 0x001FCEF1 File Offset: 0x001FB0F1
		public void OnCouncilorVOClicked()
		{
			this.PlayCouncilorSample();
		}

		// Token: 0x06004BD5 RID: 19413 RVA: 0x001FCEFC File Offset: 0x001FB0FC
		public void OnEndEditGivenName()
		{
			string text = this.givenNameEntry.text;
			if (text.Trim() == string.Empty)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				this.givenNameEntry.text = this.proposedGivenName;
			}
			else
			{
				this.proposedGivenName = text;
			}
			this.UpdateNameSettings();
			this.confirmChangeBioButton.interactable = true;
		}

		// Token: 0x06004BD6 RID: 19414 RVA: 0x001FCF60 File Offset: 0x001FB160
		public void OnEndEditFamilyName()
		{
			string text = this.familyNameEntry.text;
			if (text.Trim() == string.Empty)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				this.familyNameEntry.text = this.proposedFamilyName;
			}
			else
			{
				this.proposedFamilyName = text;
			}
			this.UpdateNameSettings();
			this.confirmChangeBioButton.interactable = true;
		}

		// Token: 0x06004BD7 RID: 19415 RVA: 0x001FCFC4 File Offset: 0x001FB1C4
		public void OnConfirmCustomizationSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.currentCouncilor.faction.playerControl.StartAction(new ChangeCouncilorBio(this.currentCouncilor, this.proposedGivenName, this.proposedFamilyName, this.proposedCouncilorAppearance, this.proposedVoice));
			this.UpdateSingleCouncilorInGrid(this.currentCouncilor);
			this.HideAllTutorials();
			this.councilorSingleUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Detail, false, true);
		}

		// Token: 0x06004BD8 RID: 19416 RVA: 0x001FD03C File Offset: 0x001FB23C
		private void InitializeLedgerCanvas()
		{
			this.ledger.enabled = false;
			this.ledgerRaycaster.enabled = false;
			this.ledger.gameObject.SetActive(true);
			this.ledgerCollapseAllButtonText.SetText(Loc.T("UI.Council.Ledger.CollapseAll"));
			this.ledgerExpandAllButtonText.SetText(Loc.T("UI.Council.Ledger.ExpandAll"));
		}

		// Token: 0x06004BD9 RID: 19417 RVA: 0x001FD09C File Offset: 0x001FB29C
		public void OpenLedger()
		{
			this.ledger.enabled = true;
			this.ledgerRaycaster.enabled = true;
			this.UpdateLedger(true);
			this.HideAllTutorials();
			this.ledgerUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Ledger, false, true);
		}

		// Token: 0x06004BDA RID: 19418 RVA: 0x001FD0D5 File Offset: 0x001FB2D5
		public void CloseLedger()
		{
			this.ledger.enabled = false;
			this.ledgerRaycaster.enabled = false;
		}

		// Token: 0x06004BDB RID: 19419 RVA: 0x001FD0F0 File Offset: 0x001FB2F0
		public void UpdateLedger(bool collapseAll = true)
		{
			int num = 2 + base.activePlayer.councilors.Count + base.activePlayer.councilors.Sum<TICouncilorState>((TICouncilorState x) => x.traits.Count<TITraitTemplate>((TITraitTemplate y) => y.incomeTrait)) + base.activePlayer.councilors.Sum<TICouncilorState>((TICouncilorState x) => x.orgs.Count) + base.activePlayer.nationsWithMyControlPoints.Count + base.activePlayer.habs.Count + base.activePlayer.habs.Sum<TIHabState>((TIHabState x) => x.OkayModules().Count) + base.activePlayer.fleets.Count + base.activePlayer.ships.Count;
			List<TIDataClass> list = new List<TIDataClass> { base.activePlayer, base.activePlayer };
			if (Enums.FactionResources.Any<FactionResource>((FactionResource x) => base.activePlayer.GetNegativeDailyIncomeFromUnassignedOrgs(x) != 0f))
			{
				list.Add(base.activePlayer);
				num++;
			}
			List<TIFactionState> list2 = (from x in base.activePlayer.dailyResourceTransfers
				where x.transfer.value != 0f
				select x.targetFaction).ToList<TIFactionState>();
			list2.AddRange(from x in GameStateManager.AllFactions()
				where x.dailyResourceTransfers.Any<DailyResourceTransfer>((DailyResourceTransfer y) => y.targetFaction == base.activePlayer && y.transfer.value != 0f)
				select x);
			foreach (TIFactionState tifactionState in list2.Distinct<TIFactionState>().ToList<TIFactionState>())
			{
				list.Add(tifactionState);
			}
			foreach (TICouncilorState ticouncilorState in base.activePlayer.councilors)
			{
				list.Add(ticouncilorState);
				list.AddRange(ticouncilorState.traits.Where<TITraitTemplate>((TITraitTemplate x) => x.incomeTrait));
				list.AddRange(ticouncilorState.orgs);
			}
			list.AddRange(base.activePlayer.nationsWithMyControlPoints);
			foreach (TIHabState tihabState in base.activePlayer.habs)
			{
				list.Add(tihabState);
				list.AddRange(tihabState.OkayModules());
			}
			foreach (TISpaceFleetState tispaceFleetState in base.activePlayer.fleets)
			{
				list.Add(tispaceFleetState);
				list.AddRange(tispaceFleetState.ships);
			}
			this.ledgerDataModels.Clear();
			TICouncilorState ticouncilorState2 = null;
			for (int i = 0; i < num; i++)
			{
				LedgerListItemModel ledgerListItemModel = new LedgerListItemModel();
				LedgerListItem_Data ledgerListItem_Data = new LedgerListItem_Data();
				TIFactionState tifactionState2 = list[i] as TIFactionState;
				if (tifactionState2 != null)
				{
					if (tifactionState2 == base.activePlayer)
					{
						ledgerListItem_Data.SetCommonData(ledgerListItem_Data, false, tifactionState2, null, null, i);
						ledgerListItem_Data.SetItemData(tifactionState2, i);
					}
					else
					{
						ledgerListItem_Data.SetCommonData(ledgerListItem_Data, false, tifactionState2, null, null, 3);
						ledgerListItem_Data.SetItemData(tifactionState2, 3);
					}
				}
				else
				{
					TICouncilorState ticouncilorState3 = list[i] as TICouncilorState;
					if (ticouncilorState3 != null)
					{
						ledgerListItem_Data.SetCommonData(ledgerListItem_Data, false, ticouncilorState3, null, null, 0);
						ledgerListItem_Data.SetItemData(ticouncilorState3);
						ticouncilorState2 = ticouncilorState3;
					}
					else
					{
						TITraitTemplate titraitTemplate = list[i] as TITraitTemplate;
						if (titraitTemplate != null)
						{
							ledgerListItem_Data.SetCommonData(ledgerListItem_Data, true, null, titraitTemplate, ticouncilorState2, 0);
							ledgerListItem_Data.SetItemData(titraitTemplate, ticouncilorState2);
						}
						else
						{
							TIOrgState tiorgState = list[i] as TIOrgState;
							if (tiorgState != null)
							{
								ledgerListItem_Data.SetCommonData(ledgerListItem_Data, true, tiorgState, null, tiorgState.hasCouncilor ? tiorgState.assignedCouncilor.ref_gameState : tiorgState.unassignedCouncil.ref_gameState, 0);
								ledgerListItem_Data.SetItemData(tiorgState);
							}
							else
							{
								TINationState tinationState = list[i] as TINationState;
								if (tinationState != null)
								{
									ledgerListItem_Data.SetCommonData(ledgerListItem_Data, false, tinationState, null, base.activePlayer, 0);
									ledgerListItem_Data.SetItemData(tinationState, base.activePlayer);
								}
								else
								{
									TIHabState tihabState2 = list[i] as TIHabState;
									if (tihabState2 != null)
									{
										ledgerListItem_Data.SetCommonData(ledgerListItem_Data, false, tihabState2, null, null, 0);
										ledgerListItem_Data.SetItemData(tihabState2);
									}
									else
									{
										TIHabModuleState tihabModuleState = list[i] as TIHabModuleState;
										if (tihabModuleState != null)
										{
											ledgerListItem_Data.SetCommonData(ledgerListItem_Data, true, tihabModuleState, null, tihabModuleState.hab, 0);
											ledgerListItem_Data.SetItemData(tihabModuleState);
										}
										else
										{
											TISpaceFleetState tispaceFleetState2 = list[i] as TISpaceFleetState;
											if (tispaceFleetState2 != null)
											{
												ledgerListItem_Data.SetCommonData(ledgerListItem_Data, false, tispaceFleetState2, null, null, 0);
												ledgerListItem_Data.SetItemData(tispaceFleetState2);
											}
											else
											{
												TISpaceShipState tispaceShipState = list[i] as TISpaceShipState;
												if (tispaceShipState != null)
												{
													ledgerListItem_Data.SetCommonData(ledgerListItem_Data, true, tispaceShipState, null, tispaceShipState.fleet, 0);
													ledgerListItem_Data.SetItemData(tispaceShipState);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				if (collapseAll)
				{
					ledgerListItem_Data.collapsed = true;
				}
				ledgerListItemModel.ledgerListItemData = ledgerListItem_Data;
				this.ledgerDataModels.Add(ledgerListItemModel);
			}
			this.ledgerAdapter.SetItems(this.ledgerDataModels);
		}

		// Token: 0x06004BDC RID: 19420 RVA: 0x001FD6A8 File Offset: 0x001FB8A8
		public void LedgerOnSortClicked(int sortCategory)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			List<LedgerListItemModel> list = new List<LedgerListItemModel>();
			List<LedgerListItemModel> list2 = new List<LedgerListItemModel>();
			for (int i = 0; i < this.ledgerDataModels.Count; i++)
			{
				if (!this.ledgerDataModels[i].ledgerListItemData.collapsible)
				{
					list.Add(this.ledgerDataModels[i]);
					list2.Add(this.ledgerDataModels[i]);
				}
			}
			if (sortCategory != this.lastLedgerSort)
			{
				this.ledgerSortDescending = true;
			}
			else
			{
				this.ledgerSortDescending = !this.ledgerSortDescending;
			}
			this.lastLedgerSort = sortCategory;
			if (this.ledgerSortDescending)
			{
				list2 = (from o in list2
					orderby o.ledgerListItemData.sortOverride descending, o.ledgerListItemData.ledgerValues[(LedgerEntryCategory)sortCategory] descending
					select o).ToList<LedgerListItemModel>();
			}
			else
			{
				list2 = (from o in list2
					orderby o.ledgerListItemData.sortOverride, o.ledgerListItemData.ledgerValues[(LedgerEntryCategory)sortCategory]
					select o).ToList<LedgerListItemModel>();
			}
			foreach (LedgerListItemModel ledgerListItemModel in this.ledgerDataModels)
			{
				if (ledgerListItemModel.ledgerListItemData.collapsible)
				{
					foreach (LedgerListItemModel ledgerListItemModel2 in list)
					{
						if (ledgerListItemModel.ledgerListItemData.parentGameState == ledgerListItemModel2.ledgerListItemData.associatedState)
						{
							list2.Insert(list2.IndexOf(ledgerListItemModel2) + 1, ledgerListItemModel);
						}
					}
				}
			}
			this.ledgerDataModels = new List<LedgerListItemModel>();
			this.ledgerDataModels.AddRange(list2);
			this.ledgerAdapter.SetItems(this.ledgerDataModels);
		}

		// Token: 0x06004BDD RID: 19421 RVA: 0x001FD8C4 File Offset: 0x001FBAC4
		public void LedgerCollapseAllClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			for (int i = 0; i < this.ledgerDataModels.Count; i++)
			{
				this.ledgerDataModels[i].ledgerListItemData.collapsed = true;
			}
			this.ledgerAdapter.SetItems(this.ledgerDataModels);
		}

		// Token: 0x06004BDE RID: 19422 RVA: 0x001FD91C File Offset: 0x001FBB1C
		public void LedgerExpandAllClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			for (int i = 0; i < this.ledgerDataModels.Count; i++)
			{
				this.ledgerDataModels[i].ledgerListItemData.collapsed = false;
			}
			this.ledgerAdapter.SetItems(this.ledgerDataModels);
		}

		// Token: 0x06004BDF RID: 19423 RVA: 0x001FD974 File Offset: 0x001FBB74
		public void LedgerCollapseListItem(TIGameState clickedState)
		{
			for (int i = 0; i < this.ledgerDataModels.Count; i++)
			{
				if (!this.ledgerDataModels[i].ledgerListItemData.collapsible && this.ledgerDataModels[i].ledgerListItemData.associatedState == clickedState)
				{
					this.ledgerDataModels[i].ledgerListItemData.collapsed = !this.ledgerDataModels[i].ledgerListItemData.collapsed;
					for (int j = 0; j < this.ledgerDataModels.Count; j++)
					{
						if (this.ledgerDataModels[j].ledgerListItemData.collapsible && this.ledgerDataModels[j].ledgerListItemData.parentGameState == clickedState)
						{
							this.ledgerDataModels[j].ledgerListItemData.collapsed = this.ledgerDataModels[i].ledgerListItemData.collapsed;
						}
					}
				}
			}
			this.ledgerAdapter.SetItems(this.ledgerDataModels);
		}

		// Token: 0x06004BE0 RID: 19424 RVA: 0x001FDA94 File Offset: 0x001FBC94
		public void RefreshOrgManagementUI()
		{
			this.LeaveOrgManagement();
			this.EnterOrgManagement();
			this.MoveOrgCancel();
			this.orgManagementUnnassignedHeaderText.SetText(Loc.T("UI.Councilor.OrgsGarage"));
			this.orgManagementPoolHeaderText.SetText(Loc.T("UI.Councilor.OrgsMarket"));
			this.orgManagementCostHeaderText.SetText(Loc.T("UI.Council.OrgManagement.CostHeader"));
			this.orgManagementFactionIcon.sprite = base.activePlayer.factionIcon256;
			this.tempFactionCouncilorOrgs.Clear();
			this.tempFactionOrgs.Clear();
			this.tempMarketPoolOrgs.Clear();
			foreach (TICouncilorState ticouncilorState in base.activePlayer.councilors)
			{
				foreach (TIOrgState tiorgState in ticouncilorState.orgs)
				{
					this.tempFactionCouncilorOrgs.Add(tiorgState, ticouncilorState);
				}
			}
			foreach (TIOrgState tiorgState2 in base.activePlayer.unassignedOrgs)
			{
				this.tempFactionOrgs.Add(tiorgState2);
			}
			foreach (TIOrgState tiorgState3 in base.activePlayer.availableOrgs)
			{
				this.tempMarketPoolOrgs.Add(tiorgState3);
			}
			this.orgManagementChangesPending = false;
			this.revertOrgChangesButton.interactable = false;
			this.confirmOrgChangesButton.interactable = true;
			int num = 0;
			this.orgManagementCouncilorListManager.SetListSize<OrganizerCouncilorListItem>(base.activePlayer.councilors.Count, false, false);
			using (IEnumerator<object> enumerator3 = this.orgManagementCouncilorListManager.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (CouncilGridController.<>o__492.<>p__0 == null)
					{
						CouncilGridController.<>o__492.<>p__0 = CallSite<Func<CallSite, object, OrganizerCouncilorListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerCouncilorListItem), typeof(CouncilGridController)));
					}
					OrganizerCouncilorListItem organizerCouncilorListItem = CouncilGridController.<>o__492.<>p__0.Target(CouncilGridController.<>o__492.<>p__0, enumerator3.Current);
					organizerCouncilorListItem.SetListItem(base.activePlayer.councilors[num++], this);
					organizerCouncilorListItem.UpdateListItem();
				}
			}
			this.UpdateOrgManagementUI();
			this.ResetDraggableOrgAreas();
			this.HideAllTutorials();
			this.orgManagementUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_OrgManager, false, true);
		}

		// Token: 0x06004BE1 RID: 19425 RVA: 0x001FDD50 File Offset: 0x001FBF50
		public void UpdateOrgManagementUI()
		{
			this.orgManagementChangesPending = false;
			this.orgManagementCouncilorListManager.SetListSize<OrganizerCouncilorListItem>(base.activePlayer.councilors.Count, false, false);
			using (IEnumerator<object> enumerator = this.orgManagementCouncilorListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__493.<>p__0 == null)
					{
						CouncilGridController.<>o__493.<>p__0 = CallSite<Func<CallSite, object, OrganizerCouncilorListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerCouncilorListItem), typeof(CouncilGridController)));
					}
					CouncilGridController.<>o__493.<>p__0.Target(CouncilGridController.<>o__493.<>p__0, enumerator.Current).UpdateListItem();
				}
			}
			this.orgManagementFactionUnnassignedOrgsListManager.SetListSize<OrganizerOrgListItem>(this.tempFactionOrgs.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.orgManagementFactionUnnassignedOrgsListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__493.<>p__1 == null)
					{
						CouncilGridController.<>o__493.<>p__1 = CallSite<Func<CallSite, object, OrganizerOrgListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerOrgListItem), typeof(CouncilGridController)));
					}
					CouncilGridController.<>o__493.<>p__1.Target(CouncilGridController.<>o__493.<>p__1, enumerator.Current).SetListItem(this.tempFactionOrgs[num++], OrganizerOrgListItem.OrgStatus.UNASSIGNED, this, this.unnassignedOrgsContainer);
				}
			}
			this.orgManagementFactionOrgPoolListManager.SetListSize<OrganizerOrgListItem>(this.tempMarketPoolOrgs.Count, false, false);
			num = 0;
			using (IEnumerator<object> enumerator = this.orgManagementFactionOrgPoolListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__493.<>p__2 == null)
					{
						CouncilGridController.<>o__493.<>p__2 = CallSite<Func<CallSite, object, OrganizerOrgListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerOrgListItem), typeof(CouncilGridController)));
					}
					CouncilGridController.<>o__493.<>p__2.Target(CouncilGridController.<>o__493.<>p__2, enumerator.Current).SetListItem(this.tempMarketPoolOrgs[num++], OrganizerOrgListItem.OrgStatus.AVAILABLE, this, this.factionPoolContainer);
				}
			}
			this.pendingOrgChangesCost = new TIResourcesCost();
			foreach (TIOrgState tiorgState in base.activePlayer.unassignedOrgs)
			{
				if (!this.tempFactionOrgs.Contains(tiorgState))
				{
					if (this.tempMarketPoolOrgs.Contains(tiorgState))
					{
						this.pendingOrgChangesCost.SumCosts_NoDuration(tiorgState.GetSalePrice(true));
					}
					if (this.tempFactionCouncilorOrgs.ContainsKey(tiorgState))
					{
						this.pendingOrgChangesCost.SumCosts_NoDuration(tiorgState.GetTransferCost());
					}
					this.orgManagementChangesPending = true;
				}
			}
			foreach (TIOrgState tiorgState2 in base.activePlayer.availableOrgs)
			{
				if (!this.tempMarketPoolOrgs.Contains(tiorgState2))
				{
					if (this.tempFactionOrgs.Contains(tiorgState2))
					{
						this.pendingOrgChangesCost.SumCosts_NoDuration(tiorgState2.GetPurchaseCost(base.activePlayer));
					}
					if (this.tempFactionCouncilorOrgs.ContainsKey(tiorgState2))
					{
						this.pendingOrgChangesCost.SumCosts_NoDuration(tiorgState2.GetPurchaseCost(base.activePlayer));
					}
					this.orgManagementChangesPending = true;
				}
			}
			List<TIOrgState> list = new List<TIOrgState>();
			foreach (TICouncilorState ticouncilorState in base.activePlayer.councilors)
			{
				list.AddRange(ticouncilorState.orgs);
			}
			foreach (TIOrgState tiorgState3 in list)
			{
				if (!this.tempFactionCouncilorOrgs.ContainsKey(tiorgState3))
				{
					this.tempFactionOrgs.Contains(tiorgState3);
					if (this.tempMarketPoolOrgs.Contains(tiorgState3))
					{
						this.pendingOrgChangesCost.SumCosts_NoDuration(tiorgState3.GetSalePrice(true));
					}
					this.orgManagementChangesPending = true;
				}
				else if (this.tempFactionCouncilorOrgs.ContainsKey(tiorgState3) && this.tempFactionCouncilorOrgs[tiorgState3] != tiorgState3.assignedCouncilor)
				{
					this.pendingOrgChangesCost.SumCosts_NoDuration(tiorgState3.GetTransferCost());
					this.orgManagementChangesPending = true;
				}
			}
			this.orgManagementUnnassignedOrgsCountText.SetText(Loc.T("UI.Council.OrgManagement.UnnassignedOrgs", new object[]
			{
				this.tempFactionOrgs.Count,
				TemplateManager.global.maxFactionOrgPoolSize
			}));
			this.orgManagementCostHeaderText.enabled = this.orgManagementChangesPending;
			this.revertOrgChangesButton.interactable = true;
			string text = "";
			this.confirmOrgChangesButton.interactable = this.orgManagementChangesPending && this.CanConfirmOrgChanges(out text);
			this.orgManagementFeedbackObject.SetActive(this.orgManagementChangesPending && !string.IsNullOrEmpty(text));
			this.orgManagementFeedbackText.SetText(text);
			this.orgManagementCostText.SetText(this.pendingOrgChangesCost.ToString("Relevant", false, false, null, false, FactionResource.None));
			this.UpdateBordersForValidCouncilors();
		}

		// Token: 0x06004BE2 RID: 19426 RVA: 0x001FE294 File Offset: 0x001FC494
		public void UpdateDraggableOrgAreas(TIOrgState org)
		{
			using (IEnumerator<object> enumerator = this.orgManagementCouncilorListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__494.<>p__0 == null)
					{
						CouncilGridController.<>o__494.<>p__0 = CallSite<Func<CallSite, object, OrganizerCouncilorListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerCouncilorListItem), typeof(CouncilGridController)));
					}
					CouncilGridController.<>o__494.<>p__0.Target(CouncilGridController.<>o__494.<>p__0, enumerator.Current).UpdateOrgIsValid(org);
				}
			}
			this.factionPoolContainer.UpdateOrgIsValid(org);
			this.unnassignedOrgsContainer.UpdateOrgIsValid(org);
		}

		// Token: 0x06004BE3 RID: 19427 RVA: 0x001FE338 File Offset: 0x001FC538
		public void UpdateBordersForValidCouncilors()
		{
			using (IEnumerator<object> enumerator = this.orgManagementCouncilorListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__495.<>p__0 == null)
					{
						CouncilGridController.<>o__495.<>p__0 = CallSite<Func<CallSite, object, OrganizerCouncilorListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerCouncilorListItem), typeof(CouncilGridController)));
					}
					CouncilGridController.<>o__495.<>p__0.Target(CouncilGridController.<>o__495.<>p__0, enumerator.Current).UpdateBorderForValidCouncilor();
				}
			}
		}

		// Token: 0x06004BE4 RID: 19428 RVA: 0x001FE3C4 File Offset: 0x001FC5C4
		public void ResetDraggableOrgAreas()
		{
			using (IEnumerator<object> enumerator = this.orgManagementCouncilorListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__496.<>p__0 == null)
					{
						CouncilGridController.<>o__496.<>p__0 = CallSite<Func<CallSite, object, OrganizerCouncilorListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerCouncilorListItem), typeof(CouncilGridController)));
					}
					CouncilGridController.<>o__496.<>p__0.Target(CouncilGridController.<>o__496.<>p__0, enumerator.Current).ResetBorder();
				}
			}
			this.factionPoolContainer.ResetBorder();
			this.unnassignedOrgsContainer.ResetBorder();
			this.UpdateBordersForValidCouncilors();
		}

		// Token: 0x06004BE5 RID: 19429 RVA: 0x001FE46C File Offset: 0x001FC66C
		public bool CanConfirmOrgChanges(out string reason)
		{
			StringBuilder stringBuilder = new StringBuilder();
			reason = "";
			bool flag = true;
			foreach (TICouncilorState ticouncilorState in base.activePlayer.councilors)
			{
				string text;
				if (!ticouncilorState.AreProspectiveOrgsValid(out text))
				{
					stringBuilder.AppendLine().Append(Loc.T("UI.Council.OrgManagement.Feedback.CouncilorName", new object[] { ticouncilorState.displayName })).Append(" ")
						.Append(text);
					flag = false;
				}
			}
			if (!this.pendingOrgChangesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
			{
				stringBuilder.AppendLine().Append(TIUtilities.RedLine(Loc.T("UI.Council.OrgManagement.Feedback.CantAfford")));
				flag = false;
			}
			reason = stringBuilder.ToString();
			return flag;
		}

		// Token: 0x06004BE6 RID: 19430 RVA: 0x001FE554 File Offset: 0x001FC754
		public void OnClickConfirmOrgChanges()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.pendingOrgChangesCost = new TIResourcesCost();
			List<TIOrgState> list = new List<TIOrgState>();
			list.AddRange(base.activePlayer.unassignedOrgs);
			foreach (TIOrgState tiorgState in list)
			{
				if (!this.tempFactionOrgs.Contains(tiorgState))
				{
					if (this.tempMarketPoolOrgs.Contains(tiorgState))
					{
						this.sellOrgAction = new SellOrgAction(tiorgState, base.activePlayer, null);
						base.activePlayer.playerControl.StartAction(this.sellOrgAction);
					}
					if (this.tempFactionCouncilorOrgs.ContainsKey(tiorgState))
					{
						this.purchaseOrgAction = new PurchaseOrgAction(tiorgState, base.activePlayer, this.tempFactionCouncilorOrgs[tiorgState]);
						base.activePlayer.playerControl.StartAction(this.purchaseOrgAction);
					}
				}
			}
			list.Clear();
			list.AddRange(base.activePlayer.availableOrgs);
			foreach (TIOrgState tiorgState2 in list)
			{
				if (!this.tempMarketPoolOrgs.Contains(tiorgState2))
				{
					if (this.tempFactionOrgs.Contains(tiorgState2))
					{
						this.purchaseOrgAction = new PurchaseOrgAction(tiorgState2, base.activePlayer, null);
						base.activePlayer.playerControl.StartAction(this.purchaseOrgAction);
					}
					if (this.tempFactionCouncilorOrgs.ContainsKey(tiorgState2))
					{
						this.purchaseOrgAction = new PurchaseOrgAction(tiorgState2, base.activePlayer, this.tempFactionCouncilorOrgs[tiorgState2]);
						base.activePlayer.playerControl.StartAction(this.purchaseOrgAction);
					}
				}
			}
			List<TIOrgState> list2 = new List<TIOrgState>();
			foreach (TICouncilorState ticouncilorState in base.activePlayer.councilors)
			{
				list2.AddRange(ticouncilorState.orgs);
			}
			foreach (TIOrgState tiorgState3 in list2)
			{
				if (!this.tempFactionCouncilorOrgs.ContainsKey(tiorgState3))
				{
					if (this.tempFactionOrgs.Contains(tiorgState3))
					{
						this.transferOrgAction = new TransferOrgToFactionPoolAction(tiorgState3, tiorgState3.assignedCouncilor);
						base.activePlayer.playerControl.StartAction(this.transferOrgAction);
					}
					if (this.tempMarketPoolOrgs.Contains(tiorgState3))
					{
						this.sellOrgAction = new SellOrgAction(tiorgState3, base.activePlayer, tiorgState3.assignedCouncilor);
						base.activePlayer.playerControl.StartAction(this.sellOrgAction);
					}
				}
				else if (this.tempFactionCouncilorOrgs.ContainsKey(tiorgState3) && this.tempFactionCouncilorOrgs[tiorgState3] != tiorgState3.assignedCouncilor)
				{
					TransferOrgToCouncilorAction transferOrgToCouncilorAction = new TransferOrgToCouncilorAction(tiorgState3, base.activePlayer, this.tempFactionCouncilorOrgs[tiorgState3], tiorgState3.assignedCouncilor);
					base.activePlayer.playerControl.StartAction(transferOrgToCouncilorAction);
				}
			}
			this.UpdateOrgManagementUI();
		}

		// Token: 0x06004BE7 RID: 19431 RVA: 0x001FE8EC File Offset: 0x001FCAEC
		public void OnClickRevertOrgManagementChanges()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
			this.RefreshOrgManagementUI();
		}

		// Token: 0x06004BE8 RID: 19432 RVA: 0x001FE900 File Offset: 0x001FCB00
		public void LeaveOrgManagement()
		{
			TIPromptQueueState.RemovePromptStatic(base.activePlayer, base.activePlayer, base.activePlayer, "PromptManagingOrgs", 0);
		}

		// Token: 0x06004BE9 RID: 19433 RVA: 0x001FE91F File Offset: 0x001FCB1F
		public void EnterOrgManagement()
		{
			GameStateManager.PromptQueue().AddPrompt(new Prompt(base.activePlayer, base.activePlayer, base.activePlayer, "PromptManagingOrgs", 0));
		}

		// Token: 0x06004BEA RID: 19434 RVA: 0x001FE948 File Offset: 0x001FCB48
		private void InitializeCalendarCanvas()
		{
			this.calendar.enabled = false;
			this.calendarRaycaster.enabled = false;
			this.calendar.gameObject.SetActive(true);
			this.calendarTabButtonText.SetText(Loc.T("UI.Council.CalendarTab"));
			this.resetToNowButtonText.SetText(Loc.T("UI.Council.Calendar.ResetToNowButtonText"));
			this.visibleMonthGridList.SetListSize<CalendarDayGridItemController>(42, false, false);
			this.currentMonthDropdown.ClearOptions();
			for (int i = 1; i <= 12; i++)
			{
				this.currentMonthDropdown.options.Add(new TMP_Dropdown.OptionData
				{
					text = TIDateTime.GetMonthString(i)
				});
			}
		}

		// Token: 0x06004BEB RID: 19435 RVA: 0x001FE9F0 File Offset: 0x001FCBF0
		public void OpenCalendar()
		{
			this.calendar.enabled = true;
			this.calendarRaycaster.enabled = true;
			this.currentYearDropdown.ClearOptions();
			for (int i = 0; i < 5; i++)
			{
				this.currentYearDropdown.options.Add(new TMP_Dropdown.OptionData
				{
					text = (TITimeState.Now().year + i).ToString()
				});
			}
			GameControl.eventManager.AddListener<AlarmAdded>(new EventManager.EventDelegate<AlarmAdded>(this.OnAlarmAdded), null, base.activePlayer, true, false);
			this.SetCalendar(TITimeState.Now());
			this.HideAllTutorials();
			this.calendarUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Calendar, false, true);
		}

		// Token: 0x06004BEC RID: 19436 RVA: 0x001FEAA0 File Offset: 0x001FCCA0
		public void ForceOpenCalendar()
		{
			if (this.councilTabsManager.activeTab != this.calendarTabController)
			{
				this.councilTabsManager.Toggle(this.calendarTabController);
			}
			this.HideAllTutorials();
			this.calendarUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilManagementCanvas_Calendar, false, true);
		}

		// Token: 0x06004BED RID: 19437 RVA: 0x001FEAEE File Offset: 0x001FCCEE
		public void CloseCalendar()
		{
			GameControl.eventManager.RemoveListener<AlarmAdded>(new EventManager.EventDelegate<AlarmAdded>(this.OnAlarmAdded), null);
			GeneralControlsController.Singleton.openCalendarButton.gameObject.SetActive(true);
		}

		// Token: 0x06004BEE RID: 19438 RVA: 0x001FEB1C File Offset: 0x001FCD1C
		private void OnAlarmAdded(AlarmAdded e)
		{
			if (e.dateTime.month == this.selectedDate.month && e.dateTime.year == this.selectedDate.year)
			{
				this.SetCalendar(this.selectedDate);
			}
		}

		// Token: 0x06004BEF RID: 19439 RVA: 0x001FEB5C File Offset: 0x001FCD5C
		private void SetCalendar(TIDateTime dateForCalendar)
		{
			this.selectedDate = dateForCalendar;
			TIDateTime tidateTime = TITimeState.Now();
			this.currentMonthDropdown.SetValueWithoutNotify(dateForCalendar.month - 1);
			this.currentMonthDropdown.captionText.SetText(TIDateTime.GetMonthString(dateForCalendar.month));
			this.currentMonthDropdown.RefreshShownValue();
			this.currentYearDropdown.SetValueWithoutNotify(dateForCalendar.year - tidateTime.year);
			this.currentYearDropdown.captionText.SetText(dateForCalendar.year.ToString());
			this.currentYearDropdown.RefreshShownValue();
			this.cycleMonthForwardButton.interactable = dateForCalendar.year < tidateTime.year + 5 || dateForCalendar.month < tidateTime.month;
			this.cycleMonthBackwardButton.interactable = dateForCalendar.month > tidateTime.month || dateForCalendar.year > tidateTime.year;
			this.cycleYearForwardButton.interactable = dateForCalendar.year < tidateTime.year + 5;
			this.cycleYearBackwardButton.interactable = dateForCalendar.year > tidateTime.year;
			this.resetCalendarToNowButton.interactable = dateForCalendar.year != tidateTime.year || dateForCalendar.month != tidateTime.month;
			SortedList<int, List<CalendarDayGridItemController.CalendarItem>> monthlyEvents = CalendarDayGridItemController.GetMonthlyEvents(base.activePlayer, dateForCalendar);
			int num = DateTime.DaysInMonth(dateForCalendar.year, dateForCalendar.month);
			int dayOfWeek = (int)new DateTime(dateForCalendar.year, dateForCalendar.month, 1).DayOfWeek;
			int num2 = 0;
			int num3 = 1;
			using (IEnumerator<object> enumerator = this.visibleMonthGridList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__520.<>p__0 == null)
					{
						CouncilGridController.<>o__520.<>p__0 = CallSite<Func<CallSite, object, CalendarDayGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CalendarDayGridItemController), typeof(CouncilGridController)));
					}
					CalendarDayGridItemController calendarDayGridItemController = CouncilGridController.<>o__520.<>p__0.Target(CouncilGridController.<>o__520.<>p__0, enumerator.Current);
					if (num2 < dayOfWeek || num2 >= dayOfWeek + num)
					{
						calendarDayGridItemController.ClearGridItem();
					}
					else
					{
						TIDateTime tidateTime2 = new TIDateTime(new DateTime(dateForCalendar.year, dateForCalendar.month, num3));
						calendarDayGridItemController.UpdateGridItem(tidateTime2, monthlyEvents[num3]);
						num3++;
					}
					num2++;
				}
			}
		}

		// Token: 0x06004BF0 RID: 19440 RVA: 0x001FEDB8 File Offset: 0x001FCFB8
		public void OnDateDropdownChanged()
		{
			int num = this.currentMonthDropdown.value + 1;
			int num2 = this.currentYearDropdown.value + TITimeState.Now().year;
			TIDateTime tidateTime = new TIDateTime(new DateTime(num2, num, 1));
			if (this.selectedDate.month != num || this.selectedDate.year != num2)
			{
				this.SetCalendar(tidateTime);
			}
		}

		// Token: 0x06004BF1 RID: 19441 RVA: 0x001FEE1C File Offset: 0x001FD01C
		public void CycleMonthForward()
		{
			TIDateTime tidateTime = new TIDateTime(this.selectedDate);
			tidateTime.AddMonths(1);
			this.SetCalendar(tidateTime);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}

		// Token: 0x06004BF2 RID: 19442 RVA: 0x001FEE50 File Offset: 0x001FD050
		public void CycleMonthBackward()
		{
			TIDateTime tidateTime = new TIDateTime(this.selectedDate);
			tidateTime.AddMonths(-1);
			this.SetCalendar(tidateTime);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
		}

		// Token: 0x06004BF3 RID: 19443 RVA: 0x001FEE84 File Offset: 0x001FD084
		public void CycleYearForward()
		{
			TIDateTime tidateTime = new TIDateTime(this.selectedDate);
			tidateTime.AddYears(1);
			this.SetCalendar(tidateTime);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}

		// Token: 0x06004BF4 RID: 19444 RVA: 0x001FEEB8 File Offset: 0x001FD0B8
		public void CycleYearBackward()
		{
			TIDateTime tidateTime = new TIDateTime(this.selectedDate);
			tidateTime.AddYears(-1);
			this.SetCalendar(tidateTime);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
		}

		// Token: 0x06004BF5 RID: 19445 RVA: 0x001FEEEB File Offset: 0x001FD0EB
		public void OnResetCalendarToNow()
		{
			this.SetCalendar(TITimeState.Now());
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		}

		// Token: 0x06004BF6 RID: 19446 RVA: 0x001FEF04 File Offset: 0x001FD104
		public void CloseAllAdvicePanels()
		{
			using (IEnumerator<object> enumerator = this.councilorGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilGridController.<>o__527.<>p__0 == null)
					{
						CouncilGridController.<>o__527.<>p__0 = CallSite<Func<CallSite, object, CouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorGridItemController), typeof(CouncilGridController)));
					}
					CouncilGridController.<>o__527.<>p__0.Target(CouncilGridController.<>o__527.<>p__0, enumerator.Current).councilorAdvicePanel.SetActive(false);
				}
			}
		}

		// Token: 0x06004BF7 RID: 19447 RVA: 0x001FEF94 File Offset: 0x001FD194
		public void GenerateAdviceForAllPanels()
		{
			int num = Mathf.Clamp(TITimeState.CampaignDuration_CompleteMonths(), 1, 4);
			List<TIFactionState.Advice> list = new List<TIFactionState.Advice>();
			this.generatedAdvice.Clear();
			foreach (object obj in Enum.GetValues(typeof(TIFactionState.Advice)))
			{
				TIFactionState.Advice advice = (TIFactionState.Advice)obj;
				list.Add(advice);
			}
			Dictionary<TICouncilorState, CouncilorGridItemController> dictionary = new Dictionary<TICouncilorState, CouncilorGridItemController>();
			int num2 = 0;
			using (IEnumerator<object> enumerator2 = this.councilorGrid.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (CouncilGridController.<>o__529.<>p__0 == null)
					{
						CouncilGridController.<>o__529.<>p__0 = CallSite<Func<CallSite, object, CouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorGridItemController), typeof(CouncilGridController)));
					}
					CouncilorGridItemController councilorGridItemController = CouncilGridController.<>o__529.<>p__0.Target(CouncilGridController.<>o__529.<>p__0, enumerator2.Current);
					TICouncilorState councilor = councilorGridItemController.councilor;
					if (TIGameState.Valid(councilor) && !(councilor.faction != base.activePlayer) && !councilor.detained && !base.activePlayer.turnedCouncilors.Contains(councilor) && !this.generatedAdvice.ContainsKey(councilor))
					{
						this.generatedAdvice.Add(councilor, new List<TIFactionState.AdviceData>());
						dictionary.Add(councilor, councilorGridItemController);
						councilorGridItemController.adviceIdx = 0;
						num2++;
						if (num2 >= 6)
						{
							break;
						}
					}
				}
			}
			List<TICouncilorState> list2 = this.generatedAdvice.Keys.ToList<TICouncilorState>();
			for (int i = 0; i < list2.Count; i++)
			{
				TICouncilorState ticouncilorState = list2[i];
				List<TIFactionState.AdviceData> advice2 = TIFactionState.GetAdvice(ticouncilorState, 1, TIFactionState.repeatableAdvice);
				if (advice2.Count > 0)
				{
					this.generatedAdvice[ticouncilorState].AddRange(advice2);
				}
			}
			foreach (TIFactionState.AdviceData adviceData in TIFactionState.GetAdvice(base.activePlayer, num * list2.Count, list))
			{
				TICouncilorState ticouncilorState2 = list2.MinBy<TICouncilorState, int>((TICouncilorState x) => this.generatedAdvice[x].Count);
				this.generatedAdvice[ticouncilorState2].Add(adviceData);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				TICouncilorState ticouncilorState3 = list2[j];
				this.generatedAdvice[ticouncilorState3] = this.generatedAdvice[ticouncilorState3].OrderByDescending<TIFactionState.AdviceData, float>((TIFactionState.AdviceData x) => x.priority).ToList<TIFactionState.AdviceData>();
			}
			using (IEnumerator<object> enumerator2 = this.councilorGrid.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (CouncilGridController.<>o__529.<>p__1 == null)
					{
						CouncilGridController.<>o__529.<>p__1 = CallSite<Func<CallSite, object, CouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorGridItemController), typeof(CouncilGridController)));
					}
					CouncilorGridItemController councilorGridItemController2 = CouncilGridController.<>o__529.<>p__1.Target(CouncilGridController.<>o__529.<>p__1, enumerator2.Current);
					TICouncilorState councilor2 = councilorGridItemController2.councilor;
					if (councilor2 != null && this.generatedAdvice.ContainsKey(councilor2) && this.generatedAdvice[councilor2].Count > 0)
					{
						councilorGridItemController2.adviceText.SetText(this.generatedAdvice[councilor2][0].adviceText);
						councilorGridItemController2.councilorAdvicePanel.SetActive(true);
						councilorGridItemController2.advanceAdviceButton.gameObject.SetActive(this.generatedAdvice[councilor2].Count >= 2);
						councilorGridItemController2.councilorAdviceButton.interactable = true;
					}
					else
					{
						if (councilor2 != null && this.generatedAdvice.ContainsKey(councilor2))
						{
							councilorGridItemController2.councilorAdviceButton.interactable = this.generatedAdvice[councilor2].Count > 0;
						}
						else
						{
							councilorGridItemController2.councilorAdviceButton.interactable = false;
						}
						councilorGridItemController2.councilorAdvicePanel.SetActive(false);
					}
				}
			}
		}

		// Token: 0x04002C02 RID: 11266
		public Canvas MasterCanvas;

		// Token: 0x04002C03 RID: 11267
		public TabbedPaneManager councilTabsManager;

		// Token: 0x04002C04 RID: 11268
		public TMP_Text councilTabButtonText;

		// Token: 0x04002C05 RID: 11269
		public TabbedPaneController councilGridTabController;

		// Token: 0x04002C06 RID: 11270
		public TMP_Text recruitTabButtonText;

		// Token: 0x04002C07 RID: 11271
		public TabbedPaneController recruitTabController;

		// Token: 0x04002C08 RID: 11272
		public TMP_Text ledgerTabButtonText;

		// Token: 0x04002C09 RID: 11273
		public TabbedPaneController ledgerTabController;

		// Token: 0x04002C0A RID: 11274
		public TMP_Text orgManagementTabButtonText;

		// Token: 0x04002C0B RID: 11275
		public TabbedPaneController orgManagementTabController;

		// Token: 0x04002C0C RID: 11276
		public TMP_Text calendarTabButtonText;

		// Token: 0x04002C0D RID: 11277
		public TabbedPaneController calendarTabController;

		// Token: 0x04002C0E RID: 11278
		[Header("Tutorials")]
		public UITutorialController councilGridUITutorial;

		// Token: 0x04002C0F RID: 11279
		public UITutorialController councilorSingleUITutorial;

		// Token: 0x04002C10 RID: 11280
		public UITutorialController councilorRecruitingUITutorial;

		// Token: 0x04002C11 RID: 11281
		public UITutorialController orgManagementUITutorial;

		// Token: 0x04002C12 RID: 11282
		public UITutorialController ledgerUITutorial;

		// Token: 0x04002C13 RID: 11283
		public UITutorialController calendarUITutorial;

		// Token: 0x04002C14 RID: 11284
		public GameObject alarmClockHighlightDummy;

		// Token: 0x04002C15 RID: 11285
		public Button orgMarketplaceButton;

		// Token: 0x04002C16 RID: 11286
		public Button unassignedOrgsButton;

		// Token: 0x04002C17 RID: 11287
		public TabbedPaneManager orgTabsManager;

		// Token: 0x04002C18 RID: 11288
		public TabbedPaneController orgMarketplaceTabController;

		// Token: 0x04002C19 RID: 11289
		public TabbedPaneController unassignedOrgsTabController;

		// Token: 0x04002C1A RID: 11290
		[Header("Grid Controller")]
		public Canvas councilGridCanvas;

		// Token: 0x04002C1B RID: 11291
		public GameObject helpPanel;

		// Token: 0x04002C1C RID: 11292
		public TMP_Text CouncilName;

		// Token: 0x04002C1D RID: 11293
		public ListManagerBase councilorGrid;

		// Token: 0x04002C1E RID: 11294
		public TIFactionState council;

		// Token: 0x04002C1F RID: 11295
		public Image[] BackgroundImage = new Image[8];

		// Token: 0x04002C20 RID: 11296
		public TMP_Text councilSize5Notice;

		// Token: 0x04002C21 RID: 11297
		public TMP_Text councilSize6Notice;

		// Token: 0x04002C22 RID: 11298
		public TMP_Text turnedSlotNotice1;

		// Token: 0x04002C23 RID: 11299
		public TMP_Text turnedSlotNotice2;

		// Token: 0x04002C24 RID: 11300
		private TIProjectTemplate size5Project;

		// Token: 0x04002C25 RID: 11301
		private TIProjectTemplate size6Project;

		// Token: 0x04002C26 RID: 11302
		[Header("Recruiting Controller")]
		public Canvas recruitingCanvas;

		// Token: 0x04002C27 RID: 11303
		public TMP_Text candidatesText;

		// Token: 0x04002C28 RID: 11304
		public TMP_Text recruitCandidateButtonText;

		// Token: 0x04002C29 RID: 11305
		public ListManagerBase candidateList;

		// Token: 0x04002C2A RID: 11306
		public TICouncilorState selectedCandidate;

		// Token: 0x04002C2B RID: 11307
		public Canvas candidateDetailCanvas;

		// Token: 0x04002C2C RID: 11308
		public TMP_Text recruitMissionHeaderText;

		// Token: 0x04002C2D RID: 11309
		public TMP_Text recruitAttributesHeaderText;

		// Token: 0x04002C2E RID: 11310
		public TMP_Text recruitTraitsHeaderText;

		// Token: 0x04002C2F RID: 11311
		public TMP_Text recruitIncomesHeaderText;

		// Token: 0x04002C30 RID: 11312
		public TMP_Text candidateName;

		// Token: 0x04002C31 RID: 11313
		public TMP_Text candidateJob;

		// Token: 0x04002C32 RID: 11314
		public TMP_Text candidateLocationTitle;

		// Token: 0x04002C33 RID: 11315
		public TMP_Text candidateLocation;

		// Token: 0x04002C34 RID: 11316
		public TMP_Text candidateAgeTitle;

		// Token: 0x04002C35 RID: 11317
		public TMP_Text candidateAge;

		// Token: 0x04002C36 RID: 11318
		public TMP_Text candidateHomeRegionTitle;

		// Token: 0x04002C37 RID: 11319
		public TMP_Text candidateHomeRegion;

		// Token: 0x04002C38 RID: 11320
		public TMP_Text recruitCostTitle;

		// Token: 0x04002C39 RID: 11321
		public TMP_Text recruitCost;

		// Token: 0x04002C3A RID: 11322
		public TMP_Text candidatePersuasionText;

		// Token: 0x04002C3B RID: 11323
		public TMP_Text candidatePersuasion;

		// Token: 0x04002C3C RID: 11324
		public TMP_Text candidateInvestigationText;

		// Token: 0x04002C3D RID: 11325
		public TMP_Text candidateInvestigation;

		// Token: 0x04002C3E RID: 11326
		public TMP_Text candidateEspionageText;

		// Token: 0x04002C3F RID: 11327
		public TMP_Text candidateEspionage;

		// Token: 0x04002C40 RID: 11328
		public TMP_Text candidateCommandText;

		// Token: 0x04002C41 RID: 11329
		public TMP_Text candidateCommand;

		// Token: 0x04002C42 RID: 11330
		public TMP_Text candidateAdministrationText;

		// Token: 0x04002C43 RID: 11331
		public TMP_Text candidateAdministration;

		// Token: 0x04002C44 RID: 11332
		public TMP_Text candidateScienceText;

		// Token: 0x04002C45 RID: 11333
		public TMP_Text candidateScience;

		// Token: 0x04002C46 RID: 11334
		public TMP_Text candidateSecurityText;

		// Token: 0x04002C47 RID: 11335
		public TMP_Text candidateSecurity;

		// Token: 0x04002C48 RID: 11336
		public TMP_Text candidateApparentLoyaltyText;

		// Token: 0x04002C49 RID: 11337
		public TMP_Text candidateLoyalty;

		// Token: 0x04002C4A RID: 11338
		public TMP_Text candidateMoneyIncome;

		// Token: 0x04002C4B RID: 11339
		public TMP_Text candidateMoneyText;

		// Token: 0x04002C4C RID: 11340
		public TMP_Text candidateInfluenceIncome;

		// Token: 0x04002C4D RID: 11341
		public TMP_Text candidateInfluenceText;

		// Token: 0x04002C4E RID: 11342
		public TMP_Text candidateOpsIncome;

		// Token: 0x04002C4F RID: 11343
		public TMP_Text candidateOpsText;

		// Token: 0x04002C50 RID: 11344
		public TMP_Text candidateResearchIncome;

		// Token: 0x04002C51 RID: 11345
		public TMP_Text candidateResearchText;

		// Token: 0x04002C52 RID: 11346
		public TMP_Text candidateBoostIncome;

		// Token: 0x04002C53 RID: 11347
		public TMP_Text candidateBoostText;

		// Token: 0x04002C54 RID: 11348
		public TMP_Text candidateMissionControlIncome;

		// Token: 0x04002C55 RID: 11349
		public TMP_Text candidateMissionControlText;

		// Token: 0x04002C56 RID: 11350
		public TMP_Text candidateProjectsIncome;

		// Token: 0x04002C57 RID: 11351
		public TMP_Text candidateProjectsText;

		// Token: 0x04002C58 RID: 11352
		public TooltipTrigger candidatePersuasionTooltip;

		// Token: 0x04002C59 RID: 11353
		public TooltipTrigger candidateInvestigationTooltip;

		// Token: 0x04002C5A RID: 11354
		public TooltipTrigger candidateEspionageTooltip;

		// Token: 0x04002C5B RID: 11355
		public TooltipTrigger candidateCommandTooltip;

		// Token: 0x04002C5C RID: 11356
		public TooltipTrigger candidateAdministrationTooltip;

		// Token: 0x04002C5D RID: 11357
		public TooltipTrigger candidateScienceTooltip;

		// Token: 0x04002C5E RID: 11358
		public TooltipTrigger candidateSecurityTooltip;

		// Token: 0x04002C5F RID: 11359
		public TooltipTrigger candidateLoyaltyTooltip;

		// Token: 0x04002C60 RID: 11360
		public TooltipTrigger candidateJobTooltip;

		// Token: 0x04002C61 RID: 11361
		public VideoPlayer recruitVideo;

		// Token: 0x04002C62 RID: 11362
		public Image recruitCouncilorStillImage;

		// Token: 0x04002C63 RID: 11363
		public Button recruitCandidateButton;

		// Token: 0x04002C64 RID: 11364
		public Image candidateBackgroundImage;

		// Token: 0x04002C65 RID: 11365
		private Vector3 candidateBackgroundImageInitialPosition;

		// Token: 0x04002C66 RID: 11366
		public Canvas selectCandidateCanvas;

		// Token: 0x04002C67 RID: 11367
		public TMP_Text selectCandidateWarningText;

		// Token: 0x04002C68 RID: 11368
		public GameObject confirmRecruitBox;

		// Token: 0x04002C69 RID: 11369
		public TMP_Text confirmRecruitDialog;

		// Token: 0x04002C6A RID: 11370
		public TMP_Text confirmRecruitConfirmButtonText;

		// Token: 0x04002C6B RID: 11371
		public TMP_Text confirmRecruitDeclineButtonText;

		// Token: 0x04002C6C RID: 11372
		public ListManagerBase candidateTraitsList;

		// Token: 0x04002C6D RID: 11373
		public ListManagerBase candidateMissionsList;

		// Token: 0x04002C6E RID: 11374
		[Header("Single Councilor Screen Controller")]
		public Canvas councilorSingleCanvas;

		// Token: 0x04002C6F RID: 11375
		public GameObject councilorSingleGameObject;

		// Token: 0x04002C70 RID: 11376
		public TMP_Text councilorName;

		// Token: 0x04002C71 RID: 11377
		public TMP_Text councilorJob;

		// Token: 0x04002C72 RID: 11378
		public TooltipTrigger jobTooltip;

		// Token: 0x04002C73 RID: 11379
		public TMP_Text councilorMissionTitle;

		// Token: 0x04002C74 RID: 11380
		public TMP_Text councilorMission;

		// Token: 0x04002C75 RID: 11381
		public TMP_Text councilorCurrentLocationTitle;

		// Token: 0x04002C76 RID: 11382
		public TMP_Text councilorCurrentLocation;

		// Token: 0x04002C77 RID: 11383
		public TMP_Text councilorHomeRegionTitle;

		// Token: 0x04002C78 RID: 11384
		public TMP_Text councilorHomeRegion;

		// Token: 0x04002C79 RID: 11385
		public TMP_Text councilorAgeTitle;

		// Token: 0x04002C7A RID: 11386
		public TMP_Text councilorAge;

		// Token: 0x04002C7B RID: 11387
		public TMP_Text attributesHeaderText;

		// Token: 0x04002C7C RID: 11388
		public TMP_Text persuasionText;

		// Token: 0x04002C7D RID: 11389
		public TMP_Text persuasion;

		// Token: 0x04002C7E RID: 11390
		public TooltipTrigger persuasionTooltip;

		// Token: 0x04002C7F RID: 11391
		public TMP_Text investigationText;

		// Token: 0x04002C80 RID: 11392
		public TMP_Text investigation;

		// Token: 0x04002C81 RID: 11393
		public TooltipTrigger investigationTooltip;

		// Token: 0x04002C82 RID: 11394
		public TMP_Text espionageText;

		// Token: 0x04002C83 RID: 11395
		public TMP_Text espionage;

		// Token: 0x04002C84 RID: 11396
		public TooltipTrigger espionageTooltip;

		// Token: 0x04002C85 RID: 11397
		public TMP_Text commandText;

		// Token: 0x04002C86 RID: 11398
		public TMP_Text command;

		// Token: 0x04002C87 RID: 11399
		public TooltipTrigger commandTooltip;

		// Token: 0x04002C88 RID: 11400
		public TMP_Text administrationText;

		// Token: 0x04002C89 RID: 11401
		public TMP_Text administration;

		// Token: 0x04002C8A RID: 11402
		public TooltipTrigger administrationTooltip;

		// Token: 0x04002C8B RID: 11403
		public TMP_Text scienceText;

		// Token: 0x04002C8C RID: 11404
		public TMP_Text science;

		// Token: 0x04002C8D RID: 11405
		public TooltipTrigger scienceTooltip;

		// Token: 0x04002C8E RID: 11406
		public TMP_Text securityText;

		// Token: 0x04002C8F RID: 11407
		public TMP_Text security;

		// Token: 0x04002C90 RID: 11408
		public TooltipTrigger securityTooltip;

		// Token: 0x04002C91 RID: 11409
		public TMP_Text LoyaltyText;

		// Token: 0x04002C92 RID: 11410
		public TMP_Text apparentLoyalty;

		// Token: 0x04002C93 RID: 11411
		public TooltipTrigger loyaltyTooltip;

		// Token: 0x04002C94 RID: 11412
		public TMP_Text incomesHeaderText;

		// Token: 0x04002C95 RID: 11413
		public TMP_Text moneyIncome;

		// Token: 0x04002C96 RID: 11414
		public TMP_Text moneyText;

		// Token: 0x04002C97 RID: 11415
		public TMP_Text influenceIncome;

		// Token: 0x04002C98 RID: 11416
		public TMP_Text influenceText;

		// Token: 0x04002C99 RID: 11417
		public TMP_Text opsIncome;

		// Token: 0x04002C9A RID: 11418
		public TMP_Text opsText;

		// Token: 0x04002C9B RID: 11419
		public TMP_Text researchIncome;

		// Token: 0x04002C9C RID: 11420
		public TMP_Text researchText;

		// Token: 0x04002C9D RID: 11421
		public TMP_Text boostIncome;

		// Token: 0x04002C9E RID: 11422
		public TMP_Text boostText;

		// Token: 0x04002C9F RID: 11423
		public TMP_Text mCIncome;

		// Token: 0x04002CA0 RID: 11424
		public TMP_Text MCText;

		// Token: 0x04002CA1 RID: 11425
		public TMP_Text projectsIncome;

		// Token: 0x04002CA2 RID: 11426
		public TMP_Text projectsText;

		// Token: 0x04002CA3 RID: 11427
		public GameObject councilorInfoObject;

		// Token: 0x04002CA4 RID: 11428
		public Image statusIcon;

		// Token: 0x04002CA5 RID: 11429
		public TMP_Text statusText;

		// Token: 0x04002CA6 RID: 11430
		public Image councilorBackgroundImage;

		// Token: 0x04002CA7 RID: 11431
		private Vector3 councilorBackgroundImageInitialPosition;

		// Token: 0x04002CA8 RID: 11432
		public TooltipTrigger statusTooltip;

		// Token: 0x04002CA9 RID: 11433
		public TooltipTrigger statusTooltipCopy;

		// Token: 0x04002CAA RID: 11434
		public GameObject missionListObject;

		// Token: 0x04002CAB RID: 11435
		public Toggle automateMissionsToggle;

		// Token: 0x04002CAC RID: 11436
		public TMP_Text automateMissionsToggleText;

		// Token: 0x04002CAD RID: 11437
		public TMP_Text missionsHeaderText;

		// Token: 0x04002CAE RID: 11438
		public ListManagerBase missionsList;

		// Token: 0x04002CAF RID: 11439
		public TMP_Text traitsHeaderText;

		// Token: 0x04002CB0 RID: 11440
		public ListManagerBase traitsList;

		// Token: 0x04002CB1 RID: 11441
		public VideoPlayer councilorVideo;

		// Token: 0x04002CB2 RID: 11442
		public Image singleCouncilorStillImage;

		// Token: 0x04002CB3 RID: 11443
		public Image councilorFactionImage;

		// Token: 0x04002CB4 RID: 11444
		public Image councilorFactionGradient;

		// Token: 0x04002CB5 RID: 11445
		public TMP_Text XPTitle;

		// Token: 0x04002CB6 RID: 11446
		public TMP_Text XP;

		// Token: 0x04002CB7 RID: 11447
		public TooltipTrigger XPTip;

		// Token: 0x04002CB8 RID: 11448
		public Button dismissButton;

		// Token: 0x04002CB9 RID: 11449
		public TMP_Text dismissButtonText;

		// Token: 0x04002CBA RID: 11450
		public TMP_Text customizeButtonText;

		// Token: 0x04002CBB RID: 11451
		public GameObject dismissPanel;

		// Token: 0x04002CBC RID: 11452
		public GameObject dismissKeepButton;

		// Token: 0x04002CBD RID: 11453
		public GameObject dismissSellButton;

		// Token: 0x04002CBE RID: 11454
		public TMP_Text dismissQueryText;

		// Token: 0x04002CBF RID: 11455
		public TMP_Text dismissKeepButtonText;

		// Token: 0x04002CC0 RID: 11456
		public TMP_Text dismissSellButtonText;

		// Token: 0x04002CC1 RID: 11457
		public TMP_Text dismissCancelText;

		// Token: 0x04002CC2 RID: 11458
		public TMP_Text councilorOrgGridTitle;

		// Token: 0x04002CC3 RID: 11459
		public TMP_Text factionOrgsHeaderText;

		// Token: 0x04002CC4 RID: 11460
		public TMP_Text marketOrgsHeaderText;

		// Token: 0x04002CC5 RID: 11461
		public ListManagerBase councilorOrgGrid;

		// Token: 0x04002CC6 RID: 11462
		public ListManagerBase councilOrgGrid;

		// Token: 0x04002CC7 RID: 11463
		public ListManagerBase availableOrgGrid;

		// Token: 0x04002CC8 RID: 11464
		public TooltipTrigger orgsTooltip;

		// Token: 0x04002CC9 RID: 11465
		public TooltipTrigger orgMarketplaceTooltip;

		// Token: 0x04002CCA RID: 11466
		public TooltipTrigger unassignedOrgsTooltip;

		// Token: 0x04002CCB RID: 11467
		public GameObject confirmMovePanel;

		// Token: 0x04002CCC RID: 11468
		public TMP_Text confirmMoveQueryText;

		// Token: 0x04002CCD RID: 11469
		public GameObject confirmPurchase;

		// Token: 0x04002CCE RID: 11470
		public TMP_Text confirmPurchaseText;

		// Token: 0x04002CCF RID: 11471
		public TMP_Text orgCostText;

		// Token: 0x04002CD0 RID: 11472
		public GameObject confirmSell;

		// Token: 0x04002CD1 RID: 11473
		public TMP_Text confirmSellText;

		// Token: 0x04002CD2 RID: 11474
		public TMP_Text sellValueText;

		// Token: 0x04002CD3 RID: 11475
		public GameObject cancelMoveOrg;

		// Token: 0x04002CD4 RID: 11476
		public GameObject moveFailureOk;

		// Token: 0x04002CD5 RID: 11477
		public TMP_Text confirmFailOKText;

		// Token: 0x04002CD6 RID: 11478
		public TMP_Text oKText;

		// Token: 0x04002CD7 RID: 11479
		public TMP_Text cancelText;

		// Token: 0x04002CD8 RID: 11480
		[Header("Org Info Expand")]
		public TMP_Text infoMyOrgHeader;

		// Token: 0x04002CD9 RID: 11481
		public TMP_Text infoEquipOrgHeader;

		// Token: 0x04002CDA RID: 11482
		public TMP_Text infoMyOrgTitle;

		// Token: 0x04002CDB RID: 11483
		public TMP_Text infoEquipOrgTitle;

		// Token: 0x04002CDC RID: 11484
		public TMP_Text infoMyOrgOwned;

		// Token: 0x04002CDD RID: 11485
		public TMP_Text infoEquipOrgCost;

		// Token: 0x04002CDE RID: 11486
		public Image infoMyOrgGradient;

		// Token: 0x04002CDF RID: 11487
		public Image infoEquipOrgGradient;

		// Token: 0x04002CE0 RID: 11488
		public TMP_Text infoMyOrgTier;

		// Token: 0x04002CE1 RID: 11489
		public TMP_Text infoEquipOrgTier;

		// Token: 0x04002CE2 RID: 11490
		public TMP_Text infoMyOrgDesc;

		// Token: 0x04002CE3 RID: 11491
		public TMP_Text infoEquipOrgDesc;

		// Token: 0x04002CE4 RID: 11492
		public Image infoMyOrgIcon;

		// Token: 0x04002CE5 RID: 11493
		public Image infoEquipOrgIcon;

		// Token: 0x04002CE6 RID: 11494
		public RectTransform myOrgGridRect;

		// Token: 0x04002CE7 RID: 11495
		public RectTransform equipOrgGridRect;

		// Token: 0x04002CE8 RID: 11496
		public GameObject infoMyOrgInfoPanel;

		// Token: 0x04002CE9 RID: 11497
		public GameObject infoEquipInfoPanel;

		// Token: 0x04002CEA RID: 11498
		public OrgItemView selectedOrgTop;

		// Token: 0x04002CEB RID: 11499
		public OrgItemView selectedOrgBottom;

		// Token: 0x04002CEC RID: 11500
		public TMP_Text orgActionButtonTextBottom;

		// Token: 0x04002CED RID: 11501
		public TMP_Text orgActionButtonTextBottom2;

		// Token: 0x04002CEE RID: 11502
		public TMP_Text orgActionButtonTextTop;

		// Token: 0x04002CEF RID: 11503
		public TMP_Text orgActionButtonTextTop2;

		// Token: 0x04002CF0 RID: 11504
		public Button orgActionButtonBottom2;

		// Token: 0x04002CF1 RID: 11505
		public Button orgActionButtonTop;

		// Token: 0x04002CF2 RID: 11506
		[Header("GameStates")]
		public TICouncilorState currentCouncilor;

		// Token: 0x04002CF3 RID: 11507
		[Header("Spend XP")]
		public Button spendXPButton;

		// Token: 0x04002CF4 RID: 11508
		public TMP_Text spendXPButtonText;

		// Token: 0x04002CF5 RID: 11509
		public GameObject spendXPPanel;

		// Token: 0x04002CF6 RID: 11510
		public TMP_Text closeSpendXPButtonText;

		// Token: 0x04002CF7 RID: 11511
		public TMP_Text spendXPPanelHeaderText;

		// Token: 0x04002CF8 RID: 11512
		public GameObject confirmSpendXPSelectionPanel;

		// Token: 0x04002CF9 RID: 11513
		public TMP_Text confirmSpendXPPrompt;

		// Token: 0x04002CFA RID: 11514
		public TMP_Text confirmSpendXPSelectionButtonText;

		// Token: 0x04002CFB RID: 11515
		public TMP_Text cancelSpendXPSelectionButtonText;

		// Token: 0x04002CFC RID: 11516
		public ListManagerBase augmentationList;

		// Token: 0x04002CFD RID: 11517
		private CouncilorAugmentationOption selectedAugmentation;

		// Token: 0x04002CFE RID: 11518
		public Button customizeCouncilorButton;

		// Token: 0x04002CFF RID: 11519
		public TMP_Text customizeCouncilorButtonText;

		// Token: 0x04002D00 RID: 11520
		public GameObject customizeCouncilorPanel;

		// Token: 0x04002D01 RID: 11521
		public TMP_Text customizeCouncilorHeaderText;

		// Token: 0x04002D02 RID: 11522
		public TMP_Text customizeCouncilorCloseButton;

		// Token: 0x04002D03 RID: 11523
		public Button cycleCouncilorLeftButton;

		// Token: 0x04002D04 RID: 11524
		public Button cycleCouncilorRightButton;

		// Token: 0x04002D05 RID: 11525
		private Coroutine voicePreviewCoroutine;

		// Token: 0x04002D06 RID: 11526
		[Header("Org Drag")]
		public DragDestination councilorDragDestination;

		// Token: 0x04002D07 RID: 11527
		public DragDestination councilDragDestination;

		// Token: 0x04002D08 RID: 11528
		public DragDestination availableDragDestination;

		// Token: 0x04002D09 RID: 11529
		public bool orgCouncilTabActive;

		// Token: 0x04002D0A RID: 11530
		public GameObject equipOrgGrid;

		// Token: 0x04002D0B RID: 11531
		private PurchaseOrgAction purchaseOrgAction;

		// Token: 0x04002D0C RID: 11532
		private TransferOrgToFactionPoolAction transferOrgAction;

		// Token: 0x04002D0D RID: 11533
		private SellOrgAction sellOrgAction;

		// Token: 0x04002D0E RID: 11534
		private StringBuilder sb;

		// Token: 0x04002D0F RID: 11535
		private EventManager eventManager;

		// Token: 0x04002D10 RID: 11536
		private bool hasBeenShown;

		// Token: 0x04002D11 RID: 11537
		public bool lookingAtTurnedCouncilor;

		// Token: 0x04002D12 RID: 11538
		[Header("Customize Councilor Panel")]
		public ListManagerBase councilorAppearanceGrid;

		// Token: 0x04002D13 RID: 11539
		public Image councilorImage;

		// Token: 0x04002D14 RID: 11540
		public Button confirmChangeBioButton;

		// Token: 0x04002D15 RID: 11541
		private CouncilorGender filterForGender = CouncilorGender.Nonbinary;

		// Token: 0x04002D16 RID: 11542
		private CouncilorAncestry filterForAncestry;

		// Token: 0x04002D17 RID: 11543
		private TICouncilorTypeTemplate filterForJob;

		// Token: 0x04002D18 RID: 11544
		private bool filterForDuplicates = true;

		// Token: 0x04002D19 RID: 11545
		private string proposedGivenName;

		// Token: 0x04002D1A RID: 11546
		private string proposedFamilyName;

		// Token: 0x04002D1B RID: 11547
		private TICouncilorAppearanceTemplate proposedCouncilorAppearance;

		// Token: 0x04002D1C RID: 11548
		private bool councilorPortraitsCached;

		// Token: 0x04002D1D RID: 11549
		public TMP_Text councilorNameText;

		// Token: 0x04002D1E RID: 11550
		public TMP_Text councilorProfessionText;

		// Token: 0x04002D1F RID: 11551
		public TMP_Text councilorHomeRegionText;

		// Token: 0x04002D20 RID: 11552
		public TMP_Text ancestryFilterHeader;

		// Token: 0x04002D21 RID: 11553
		public TMP_Text ancestryFilterSetting;

		// Token: 0x04002D22 RID: 11554
		public TMP_Text genderFilterHeader;

		// Token: 0x04002D23 RID: 11555
		public TMP_Text genderFilterSetting;

		// Token: 0x04002D24 RID: 11556
		public TMP_Text jobFilterHeader;

		// Token: 0x04002D25 RID: 11557
		public TMP_Text jobFilterSetting;

		// Token: 0x04002D26 RID: 11558
		public TMP_Text duplicateFilterHeader;

		// Token: 0x04002D27 RID: 11559
		public TMP_Text duplicateFilterSetting;

		// Token: 0x04002D28 RID: 11560
		public TMP_Text voiceAccentFilterHeader;

		// Token: 0x04002D29 RID: 11561
		public TMP_Text voiceAccentFilterSetting;

		// Token: 0x04002D2A RID: 11562
		public TMP_Text voiceIndexSelectorHeader;

		// Token: 0x04002D2B RID: 11563
		public TMP_Text voiceIndexFilterSetting;

		// Token: 0x04002D2C RID: 11564
		public TMP_Text confirmChangesButtonText;

		// Token: 0x04002D2D RID: 11565
		public TMP_Text givenNameText;

		// Token: 0x04002D2E RID: 11566
		public TMP_Text familyNameText;

		// Token: 0x04002D2F RID: 11567
		public TMP_Text confirmChangeBioText;

		// Token: 0x04002D30 RID: 11568
		public TMP_Text cancelChangeBioText;

		// Token: 0x04002D31 RID: 11569
		private Dictionary<CouncilorAncestry, string> ancestrySettings;

		// Token: 0x04002D32 RID: 11570
		private Dictionary<CouncilorGender, string> genderSettings;

		// Token: 0x04002D33 RID: 11571
		private Dictionary<bool, string> duplicateSettings;

		// Token: 0x04002D34 RID: 11572
		private TICouncilorVoiceTemplate proposedVoice;

		// Token: 0x04002D35 RID: 11573
		private int ancestrySetting;

		// Token: 0x04002D36 RID: 11574
		private int genderSetting;

		// Token: 0x04002D37 RID: 11575
		private int jobSetting;

		// Token: 0x04002D38 RID: 11576
		private int accentSetting;

		// Token: 0x04002D39 RID: 11577
		private int voiceIndexSetting;

		// Token: 0x04002D3A RID: 11578
		private List<TICouncilorTypeTemplate> jobTemplates = new List<TICouncilorTypeTemplate>();

		// Token: 0x04002D3B RID: 11579
		private List<TICouncilorVoiceTemplate> voiceTemplates = new List<TICouncilorVoiceTemplate>();

		// Token: 0x04002D3C RID: 11580
		private Dictionary<int, string> allAccentOptions = new Dictionary<int, string>();

		// Token: 0x04002D3D RID: 11581
		private Dictionary<int, TICouncilorVoiceTemplate> currentAccentOptions = new Dictionary<int, TICouncilorVoiceTemplate>();

		// Token: 0x04002D3E RID: 11582
		private readonly TIMissionTemplate testVoiceTemplate = TemplateManager.Find<TIMissionTemplate>("GoToGround", false);

		// Token: 0x04002D3F RID: 11583
		public TMP_InputField givenNameEntry;

		// Token: 0x04002D40 RID: 11584
		public TMP_InputField familyNameEntry;

		// Token: 0x04002D41 RID: 11585
		public Canvas ledger;

		// Token: 0x04002D42 RID: 11586
		public GraphicRaycaster ledgerRaycaster;

		// Token: 0x04002D43 RID: 11587
		public ListManagerBase ledgerListManager;

		// Token: 0x04002D44 RID: 11588
		private List<LedgerListItemModel> ledgerDataModels = new List<LedgerListItemModel>();

		// Token: 0x04002D45 RID: 11589
		public TMP_Text ledgerCollapseAllButtonText;

		// Token: 0x04002D46 RID: 11590
		public TMP_Text ledgerExpandAllButtonText;

		// Token: 0x04002D47 RID: 11591
		public LedgerListAdapter ledgerAdapter;

		// Token: 0x04002D48 RID: 11592
		private int lastLedgerSort = -1;

		// Token: 0x04002D49 RID: 11593
		private bool ledgerSortDescending = true;

		// Token: 0x04002D4A RID: 11594
		[Header("Org Management")]
		public Canvas orgManagementCanvas;

		// Token: 0x04002D4B RID: 11595
		public TMP_Text orgManagementUnnassignedHeaderText;

		// Token: 0x04002D4C RID: 11596
		public TMP_Text orgManagementPoolHeaderText;

		// Token: 0x04002D4D RID: 11597
		public TMP_Text orgManagementCostHeaderText;

		// Token: 0x04002D4E RID: 11598
		public TMP_Text orgManagementCostText;

		// Token: 0x04002D4F RID: 11599
		public TMP_Text orgManagementUnnassignedOrgsCountText;

		// Token: 0x04002D50 RID: 11600
		public TMP_Text orgManagementFeedbackText;

		// Token: 0x04002D51 RID: 11601
		public GameObject orgManagementFeedbackObject;

		// Token: 0x04002D52 RID: 11602
		public ListManagerBase orgManagementCouncilorListManager;

		// Token: 0x04002D53 RID: 11603
		public ListManagerBase orgManagementFactionUnnassignedOrgsListManager;

		// Token: 0x04002D54 RID: 11604
		public ListManagerBase orgManagementFactionOrgPoolListManager;

		// Token: 0x04002D55 RID: 11605
		public DragDestination orgManagementFactionUnnassignedDragDestination;

		// Token: 0x04002D56 RID: 11606
		public DragDestination orgManagementFactionOrgPoolDragDestination;

		// Token: 0x04002D57 RID: 11607
		public OrganizerCouncilorListItem unnassignedOrgsContainer;

		// Token: 0x04002D58 RID: 11608
		public OrganizerCouncilorListItem factionPoolContainer;

		// Token: 0x04002D59 RID: 11609
		public Button revertOrgChangesButton;

		// Token: 0x04002D5A RID: 11610
		public Button confirmOrgChangesButton;

		// Token: 0x04002D5B RID: 11611
		public TMP_Text revertOrgChangesButtonText;

		// Token: 0x04002D5C RID: 11612
		public TMP_Text confirmOrgChangesButtonText;

		// Token: 0x04002D5D RID: 11613
		public Image orgManagementFactionIcon;

		// Token: 0x04002D5E RID: 11614
		public Image orgManagementMarketIcon;

		// Token: 0x04002D5F RID: 11615
		public Dictionary<TIOrgState, TICouncilorState> tempFactionCouncilorOrgs = new Dictionary<TIOrgState, TICouncilorState>();

		// Token: 0x04002D60 RID: 11616
		public List<TIOrgState> tempFactionOrgs = new List<TIOrgState>();

		// Token: 0x04002D61 RID: 11617
		public List<TIOrgState> tempMarketPoolOrgs = new List<TIOrgState>();

		// Token: 0x04002D62 RID: 11618
		public bool orgManagementChangesPending;

		// Token: 0x04002D63 RID: 11619
		public TIResourcesCost pendingOrgChangesCost = new TIResourcesCost();

		// Token: 0x04002D64 RID: 11620
		[Header("Calendar")]
		public Canvas calendar;

		// Token: 0x04002D65 RID: 11621
		public GraphicRaycaster calendarRaycaster;

		// Token: 0x04002D66 RID: 11622
		private TIDateTime selectedDate;

		// Token: 0x04002D67 RID: 11623
		public ListManagerBase visibleMonthGridList;

		// Token: 0x04002D68 RID: 11624
		public TMP_Dropdown currentMonthDropdown;

		// Token: 0x04002D69 RID: 11625
		public TMP_Dropdown currentYearDropdown;

		// Token: 0x04002D6A RID: 11626
		public Button cycleMonthBackwardButton;

		// Token: 0x04002D6B RID: 11627
		public Button cycleYearBackwardButton;

		// Token: 0x04002D6C RID: 11628
		public Button cycleMonthForwardButton;

		// Token: 0x04002D6D RID: 11629
		public Button cycleYearForwardButton;

		// Token: 0x04002D6E RID: 11630
		public Button resetCalendarToNowButton;

		// Token: 0x04002D6F RID: 11631
		public TMP_Text resetToNowButtonText;

		// Token: 0x04002D70 RID: 11632
		public const int MAX_CALENDAR_YEARS = 5;

		// Token: 0x04002D71 RID: 11633
		public Dictionary<TICouncilorState, List<TIFactionState.AdviceData>> generatedAdvice = new Dictionary<TICouncilorState, List<TIFactionState.AdviceData>>();
	}
}
