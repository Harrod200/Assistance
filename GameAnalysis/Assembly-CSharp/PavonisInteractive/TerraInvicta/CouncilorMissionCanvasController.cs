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
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000843 RID: 2115
	public class CouncilorMissionCanvasController : CanvasControllerBase, ICanvas
	{
		// Token: 0x17000EA5 RID: 3749
		// (get) Token: 0x06004C79 RID: 19577 RVA: 0x00204CFC File Offset: 0x00202EFC
		private TIMissionTemplate missionTemplate
		{
			get
			{
				if (!(this.activeButton != null))
				{
					return null;
				}
				return this.activeButton.missionType;
			}
		}

		// Token: 0x17000EA6 RID: 3750
		// (get) Token: 0x06004C7A RID: 19578 RVA: 0x00204D19 File Offset: 0x00202F19
		// (set) Token: 0x06004C7B RID: 19579 RVA: 0x00204D21 File Offset: 0x00202F21
		public TICouncilorState enemyCouncilor { get; private set; }

		// Token: 0x06004C7C RID: 19580 RVA: 0x00204D2C File Offset: 0x00202F2C
		public override void Initialize()
		{
			base.Initialize();
			GameControl.eventManager.AddListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.CouncilorSelected), null, null, false, false);
			GameControl.eventManager.AddListener<MissionPhaseRestart>(new EventManager.EventDelegate<MissionPhaseRestart>(this.ResetOnStart), null, null, true, false);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ResetOnStart), "CouncilorMissionUpdate", null, false, false);
			GameControl.eventManager.AddListener<TimeEventComplete>(new EventManager.EventDelegate<TimeEventComplete>(this.ResetOnComplete), "CouncilorMissionUpdate", null, false, false);
			GameControl.eventManager.AddListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.OnCouncilCompositionChanged), null, null, false, false);
			GameControl.eventManager.AddListener<CouncilorSelectedOffMap>(new EventManager.EventDelegate<CouncilorSelectedOffMap>(this.CouncilorSelected), null, null, false, false);
			GameControl.eventManager.AddListener<FactionFinalizesMissions>(new EventManager.EventDelegate<FactionFinalizesMissions>(this.OnFactionFinalizesMissions), null, null, false, false);
			this._modifierAnimator = this.successOrFailurePanel.GetComponent<Animator>();
			this.costPanel = this.resourcesSlider.transform.parent;
			this.myCouncilorHeader.SetText(Loc.T("UI.Councilor.Header"));
			this.orgsTabText.SetText(Loc.T("UI.Councilor.Orgs"));
			this.enemyCouncilorTraitsHeader.SetText(Loc.T("UI.Councilor.Traits"));
			this.missionsTabText.SetText(Loc.T("UI.Councilor.Missions"));
			this.confirmAssignmentsText.SetText(Loc.T("UI.MissionPhase.ConfirmAssignments"));
			this.unassignedWarningHeader.SetText(Loc.T("UI.MissionPhase.ConfirmPromptHeader"));
			this.unassignedWarningPrompt.SetText(Loc.T("UI.MissionPhase.ConfirmPrompt"));
			this.unassignedWarningConfirmButton.SetText(Loc.T("UI.MissionPhase.ConfirmYes"));
			this.unassignedWarningDeclineButton.SetText(Loc.T("UI.MissionPhase.ConfirmNo"));
			this.successOrFailureText.SetText(Loc.T("UI.MissionPhase.Success"));
			this.confirmAssignmentsText.SetText(Loc.T("UI.MissionPhase.ConfirmAssignments"));
			this.abortButtonText.SetText(Loc.T("UI.Councilor.AbortButtonText"));
			this.abortWarningHeader.SetText(Loc.T("UI.Councilor.AbortWarningHeader"));
			this.abortWarningBody.SetText(Loc.T("UI.Councilor.AbortWarningBody"));
			this.abortConfirmButtonText.SetText(Loc.T("UI.Councilor.AbortConfirmButtonText"));
			this.abortCancelButtonText.SetText(Loc.T("UI.Councilor.AbortCancelButtonText"));
			this.perTitle.SetText(Loc.T("UI.Global.PersuasionShort"));
			this.invTitle.SetText(Loc.T("UI.Global.InvestigationShort"));
			this.espTitle.SetText(Loc.T("UI.Global.EspionageShort"));
			this.cmdTitle.SetText(Loc.T("UI.Global.CommandShort"));
			this.admTitle.SetText(Loc.T("UI.Global.AdministrationShort"));
			this.sciTitle.SetText(Loc.T("UI.Global.ScienceShort"));
			this.secTitle.SetText(Loc.T("UI.Global.SecurityShort"));
			this.loyTitle.SetText(Loc.T("UI.Global.LoyaltyShort"));
			this.enemyPerTitle.SetText(Loc.T("UI.Global.PersuasionShort"));
			this.enemyInvTitle.SetText(Loc.T("UI.Global.InvestigationShort"));
			this.enemyEspTitle.SetText(Loc.T("UI.Global.EspionageShort"));
			this.enemyCmdTitle.SetText(Loc.T("UI.Global.CommandShort"));
			this.enemyAdmTitle.SetText(Loc.T("UI.Global.AdministrationShort"));
			this.enemySciTitle.SetText(Loc.T("UI.Global.ScienceShort"));
			this.enemySecTitle.SetText(Loc.T("UI.Global.SecurityShort"));
			this.enemyLoyTitle.SetText(Loc.T("UI.Global.LoyaltyShort"));
			this.trackingMeTip.SetDelegate("BodyText", () => Loc.T("UI.Councilor.TrackingMeTip", new object[] { TIFactionState.goToGroundMission.displayName }));
			this.abortConfirmUI.SetActive(false);
			this.unassignedWarningPanel.SetActive(false);
			base.canvasManager.RegisterAssetPanelDisableOrder(AssetPanel.MyCouncilor, new Action(this.CloseMyCouncilorPanel));
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.CouncilorDetail, new Action(this.CloseEnemyCouncilorPanel));
			this.missionPhaseControlsCanvas.enabled = false;
			this.enemyCouncilorInfoPanel.enabled = false;
			this.enemyCouncilorGraphicRaycaster.enabled = false;
			this.myCouncilorPanel.enabled = false;
			this.myCouncilorPanelGraphicRaycaster.enabled = true;
			this.myCouncilorBackgroundImageInitialPosition = this.councilorBackgroundImage.rectTransform.localPosition;
			this.enemyCouncilorBackgroundImageInitialPosition = this.councilorBackgroundImage.rectTransform.localPosition;
			this.ResetMissionDisplayElements();
			this.automateTooltip.SetDelegate("BodyText", () => this.SetAutomateCouncilorTooltip());
			this.InitializeOrgList();
		}

		// Token: 0x06004C7D RID: 19581 RVA: 0x002051CC File Offset: 0x002033CC
		public override void Show()
		{
			GameControl.eventManager.AddListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null, null, true, false);
			GameControl.eventManager.AddListener<SpaceBodySelectedEvent>(new EventManager.EventDelegate<SpaceBodySelectedEvent>(this.OnNaturalSpaceObjectSelected), null, null, true, false);
			GameControl.eventManager.AddListener<LagrangePointSelectedEvent>(new EventManager.EventDelegate<LagrangePointSelectedEvent>(this.OnNaturalSpaceObjectSelected), null, null, true, false);
			GameControl.eventManager.AddListener<DeTargetCouncilors>(new EventManager.EventDelegate<DeTargetCouncilors>(this.OnDeTargetCouncilors), null, null, true, false);
			if (this.showCouncilList)
			{
				GameControl.eventManager.AddListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateCouncilorStatus), null, null, true, false);
			}
			this.missionPhaseControlsCanvas.gameObject.SetActive(true);
			this.myCouncilorPanel.gameObject.SetActive(true);
			this.enemyCouncilorInfoPanel.gameObject.SetActive(true);
			if (this.myCouncilor != null)
			{
				this.myCouncilorDataDirty = true;
			}
			if (this.enemyCouncilor != null)
			{
				this.enemyCouncilorDataDirty = true;
			}
			this.Refresh();
			base.Show();
		}

		// Token: 0x06004C7E RID: 19582 RVA: 0x002052CC File Offset: 0x002034CC
		public override void Hide()
		{
			GameControl.eventManager.RemoveListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null);
			GameControl.eventManager.RemoveListener<SpaceBodySelectedEvent>(new EventManager.EventDelegate<SpaceBodySelectedEvent>(this.OnNaturalSpaceObjectSelected), null);
			GameControl.eventManager.RemoveListener<LagrangePointSelectedEvent>(new EventManager.EventDelegate<LagrangePointSelectedEvent>(this.OnNaturalSpaceObjectSelected), null);
			GameControl.eventManager.RemoveListener<DeTargetCouncilors>(new EventManager.EventDelegate<DeTargetCouncilors>(this.OnDeTargetCouncilors), null);
			if (this.showCouncilList)
			{
				GameControl.eventManager.RemoveListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateCouncilorStatus), null);
			}
			Canvas canvas = this.myCouncilorPanel;
			if (canvas != null)
			{
				canvas.gameObject.SetActive(false);
			}
			Canvas canvas2 = this.enemyCouncilorInfoPanel;
			if (canvas2 != null)
			{
				canvas2.gameObject.SetActive(false);
			}
			Canvas canvas3 = this.missionPhaseControlsCanvas;
			if (canvas3 != null)
			{
				canvas3.gameObject.SetActive(false);
			}
			this.SelectCouncilorTutorialController.HideTutorial();
			this.CouncilorMissionCanvasUITutorial.HideTutorial();
			base.Hide();
		}

		// Token: 0x06004C7F RID: 19583 RVA: 0x002053B8 File Offset: 0x002035B8
		public override void Refresh()
		{
			if (this.myCouncilorDataDirty)
			{
				if (this.myCouncilorPanel.enabled)
				{
					this.UpdateMyCouncilorPanel();
					this.UpdateCouncilorActionBar();
					this.SelectCouncilorTutorialController.HoldTutorial(CampaignMilestone.UITutorial_SelectCouncilor, false, true);
				}
				this.myCouncilorDataDirty = false;
			}
			if (this.enemyCouncilorDataDirty)
			{
				if (this.enemyCouncilorInfoPanel.enabled)
				{
					this.UpdateEnemyCouncilorPanel();
				}
				this.enemyCouncilorDataDirty = false;
			}
			if (this.currentTargeting != null && Input.GetKeyUp(KeyCode.Tab))
			{
				if (TIInputManager.IsShiftKeyDown)
				{
					this.currentTargeting.CycleTargetBackward();
				}
				else
				{
					this.currentTargeting.CycleTargetForward();
				}
			}
			if (this.confirmAssignmentsButton.interactable && !this._isConfirmAssignmentsHovered)
			{
				if (base.activePlayer.activeCouncilors.All<TICouncilorState>((TICouncilorState x) => x.HasMission) && this._confirmAssignmentsNextFlashTime < Time.time)
				{
					if (this._isConfirmAssignmentsFlashOn)
					{
						this._confirmAssignmentsNextFlashTime = Time.time + this.confirmFlashOffTime;
						this.confirmAssignmentsButton.OnDeselect(null);
						this._isConfirmAssignmentsFlashOn = false;
						return;
					}
					this._confirmAssignmentsNextFlashTime = Time.time + this.confirmFlashOnTime;
					this.confirmAssignmentsButton.OnSelect(null);
					this._isConfirmAssignmentsFlashOn = true;
				}
			}
		}

		// Token: 0x06004C80 RID: 19584 RVA: 0x002054FD File Offset: 0x002036FD
		private void OnInfoScreenOpened(InfoScreenOpened e)
		{
			if (this.Visible())
			{
				this.ShutdownTargetSelection(true);
				this.Hide();
				GameControl.eventManager.AddListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreCouncilorActionCanvas), null, null, true, false);
			}
		}

		// Token: 0x06004C81 RID: 19585 RVA: 0x0020552E File Offset: 0x0020372E
		private void RestoreCouncilorActionCanvas(InfoScreenClosed e)
		{
			this.Show();
			GameControl.eventManager.RemoveListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreCouncilorActionCanvas), null);
		}

		// Token: 0x06004C82 RID: 19586 RVA: 0x0020554D File Offset: 0x0020374D
		private void ResetOnStart(MissionPhaseRestart e)
		{
			this.StartMissionPhase(true);
		}

		// Token: 0x06004C83 RID: 19587 RVA: 0x00205556 File Offset: 0x00203756
		private void ResetOnStart(TimeEventStart e)
		{
			if (GameStateManager.MissionPhase().skipTime != TITimeState.Now())
			{
				this.StartMissionPhase(false);
			}
		}

		// Token: 0x06004C84 RID: 19588 RVA: 0x00205575 File Offset: 0x00203775
		private void ResetOnComplete(TimeEventComplete e)
		{
			this.ResetDisplay();
			this.ShutdownTargetSelection(true);
			this.ShowMissionIcons(false);
		}

		// Token: 0x06004C85 RID: 19589 RVA: 0x0020558B File Offset: 0x0020378B
		private void UpdateCouncilorStatus(CouncilorMissionUpdated e)
		{
			if (e.councilor != null)
			{
				this.UpdateCouncilList();
			}
		}

		// Token: 0x06004C86 RID: 19590 RVA: 0x002055A1 File Offset: 0x002037A1
		public void UpdateMyCouncilorPanel(CouncilorValuesChanged e)
		{
			this.myCouncilorDataDirty = true;
		}

		// Token: 0x06004C87 RID: 19591 RVA: 0x002055AA File Offset: 0x002037AA
		public void UpdateMyCouncilorPanel(CouncilorMissionUpdated e)
		{
			this.myCouncilorDataDirty = true;
		}

		// Token: 0x06004C88 RID: 19592 RVA: 0x002055B3 File Offset: 0x002037B3
		public void UpdateMyCouncilorPanel(CouncilorVisibilityChanged e)
		{
			this.myCouncilorDataDirty = true;
		}

		// Token: 0x06004C89 RID: 19593 RVA: 0x002055BC File Offset: 0x002037BC
		public void UpdateEnemyCouncilorPanel(CouncilorValuesChanged e)
		{
			this.enemyCouncilorDataDirty = true;
		}

		// Token: 0x06004C8A RID: 19594 RVA: 0x002055C5 File Offset: 0x002037C5
		public void UpdateEnemyCouncilorPanel(CouncilorMissionUpdated e)
		{
			this.enemyCouncilorDataDirty = true;
		}

		// Token: 0x06004C8B RID: 19595 RVA: 0x002055CE File Offset: 0x002037CE
		public void UpdateEnemyCouncilorPanel(CouncilorVisibilityChanged e)
		{
			this.enemyCouncilorDataDirty = true;
		}

		// Token: 0x06004C8C RID: 19596 RVA: 0x002055D7 File Offset: 0x002037D7
		public void OnNaturalSpaceObjectSelected(SpaceBodySelectedEvent e)
		{
			this.OnNaturalSpaceObjectSelected(e.spaceBody);
		}

		// Token: 0x06004C8D RID: 19597 RVA: 0x002055E5 File Offset: 0x002037E5
		public void OnNaturalSpaceObjectSelected(LagrangePointSelectedEvent e)
		{
			this.OnNaturalSpaceObjectSelected(e.lagrangePoint);
		}

		// Token: 0x06004C8E RID: 19598 RVA: 0x002055F3 File Offset: 0x002037F3
		public void OnNaturalSpaceObjectSelected(TINaturalSpaceObjectState spaceObject)
		{
			if (base.activePlayer.VisibleOperationList(spaceObject).Count > 0 && !GeneralControlsController.CurrentValidTarget(spaceObject))
			{
				this.ResetMissionDisplayElements();
				this.ShutdownTargetSelection(true);
			}
		}

		// Token: 0x06004C8F RID: 19599 RVA: 0x0020561E File Offset: 0x0020381E
		private void CouncilorSelected(CouncilorMapItemSelected e)
		{
			this.CouncilorSelected(e.councilor);
		}

		// Token: 0x06004C90 RID: 19600 RVA: 0x0020562C File Offset: 0x0020382C
		private void CouncilorSelected(CouncilorSelectedOffMap e)
		{
			this.CouncilorSelected(e.councilor);
		}

		// Token: 0x06004C91 RID: 19601 RVA: 0x0020563A File Offset: 0x0020383A
		public void OnDeTargetCouncilors(DeTargetCouncilors e)
		{
			if (this.enemyCouncilorInfoPanel.enabled && this.enemyCouncilor.faction == base.activePlayer)
			{
				base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
			}
		}

		// Token: 0x06004C92 RID: 19602 RVA: 0x00205674 File Offset: 0x00203874
		public void OnCouncilCompositionChanged(CouncilCompositionChanged e)
		{
			if (!(e.councilor == this.enemyCouncilor))
			{
				if (e.councilor == this.myCouncilor)
				{
					if (this.myCouncilor.archived || this.myCouncilor.faction != base.activePlayer)
					{
						base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
						return;
					}
					this.UpdateMyCouncilorPanel();
				}
				return;
			}
			if (this.enemyCouncilor.archived || this.enemyCouncilor.faction == null || this.enemyCouncilor.faction == base.activePlayer)
			{
				base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
				return;
			}
			this.UpdateEnemyCouncilorPanel();
		}

		// Token: 0x06004C93 RID: 19603 RVA: 0x00205735 File Offset: 0x00203935
		public void OnFactionFinalizesMissions(FactionFinalizesMissions e)
		{
			this.CheckReleaseUIToPlayer();
		}

		// Token: 0x06004C94 RID: 19604 RVA: 0x00205740 File Offset: 0x00203940
		public void CheckReleaseUIToPlayer()
		{
			this.confirmAssignmentsButton.interactable = GameStateManager.MissionPhase().factionsSignallingComplete.Intersect<TIFactionState>((from x in GameStateManager.AllFactions()
				where x.player.isAI
				select x).ToList<TIFactionState>()).Count<TIFactionState>() == this.AIPlayerCount;
			if (!this.confirmAssignmentsButton.interactable)
			{
				this.confirmAssignmentsText.SetText(Loc.T("UI.MissionPhase.AIWorking"));
				return;
			}
			this.confirmAssignmentsText.SetText(Loc.T("UI.MissionPhase.ConfirmAssignments"));
		}

		// Token: 0x06004C95 RID: 19605 RVA: 0x002057DC File Offset: 0x002039DC
		private void StartMissionPhase(bool initial = false)
		{
			if (!this.Visible())
			{
				this.Show();
			}
			if (this.myCouncilor == null)
			{
				this.myCouncilor = base.activePlayer.FirstCouncilorAvailableForMissionAssignment;
			}
			this.AIPlayerCount = (from x in GameStateManager.AllFactions()
				where x.player.isAI
				select x).Count<TIFactionState>();
			if (TIPlayerProfileManager.assignmentPhaseCouncilorCameraFocus)
			{
				this.needToForceCouncilorSelection = true;
				if (this.myCouncilor != null)
				{
					base.canvasManager.CloseActiveInfoScreen();
					this.nextCouncilor = this.myCouncilor;
					if (!initial)
					{
						base.Invoke("ForcePlayerNewCouncilor", 1f);
					}
				}
				else
				{
					base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
				}
			}
			this.missionPhaseControlsCanvas.enabled = true;
			if (this.showCouncilList)
			{
				this.councilStatusPanel.SetActive(true);
				this.UpdateCouncilList();
			}
			this.CheckReleaseUIToPlayer();
		}

		// Token: 0x06004C96 RID: 19606 RVA: 0x002058CF File Offset: 0x00203ACF
		public void PlayerConfirmsMissionAssignments()
		{
			this.missionPhaseControlsCanvas.enabled = false;
			base.activePlayer.playerControl.StartAction(new FinalizeCouncilorMissions(base.activePlayer));
		}

		// Token: 0x06004C97 RID: 19607 RVA: 0x002058F8 File Offset: 0x00203AF8
		private void CheckForCloseCanvas()
		{
			if (this.myCouncilorPanel != null && this.enemyCouncilorInfoPanel != null && this.missionPhaseControlsCanvas != null && !this.myCouncilorPanel.enabled && !this.enemyCouncilorInfoPanel.enabled && !this.missionPhaseControlsCanvas.enabled)
			{
				this.Hide();
			}
		}

		// Token: 0x06004C98 RID: 19608 RVA: 0x0020595C File Offset: 0x00203B5C
		public void OnCompleteAssignmentsClick()
		{
			if (base.activePlayer.activeCouncilors.Any<TICouncilorState>((TICouncilorState x) => x.activeMission == null) && !Application.isEditor)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				this.unassignedWarningPanel.SetActive(true);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.assignmentsArrowLeft.SetActive(false);
			this.assignmentsArrowRight.SetActive(false);
			this.PlayerConfirmsMissionAssignments();
			this.SelectCouncilorTutorialController.HideTutorial();
			base.activePlayer.CompleteMilestone(CampaignMilestone.TutorialAssignMission);
		}

		// Token: 0x06004C99 RID: 19609 RVA: 0x002059FC File Offset: 0x00203BFC
		public void OnCompleteAssignmentsMouseEnter()
		{
			this._isConfirmAssignmentsHovered = true;
		}

		// Token: 0x06004C9A RID: 19610 RVA: 0x00205A05 File Offset: 0x00203C05
		public void OnCompleteAssignmentsMouseExit()
		{
			this._isConfirmAssignmentsHovered = false;
			this._isConfirmAssignmentsFlashOn = false;
			this._confirmAssignmentsNextFlashTime = Time.time + this.confirmFlashOffTime;
			this.confirmAssignmentsButton.OnDeselect(null);
		}

		// Token: 0x06004C9B RID: 19611 RVA: 0x00205A33 File Offset: 0x00203C33
		public void OnConfirmGoForwardClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.unassignedWarningPanel.SetActive(false);
			this.PlayerConfirmsMissionAssignments();
		}

		// Token: 0x06004C9C RID: 19612 RVA: 0x00205A54 File Offset: 0x00203C54
		public void OnDeclineGoForwardClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Decline", false, false);
			this.unassignedWarningPanel.SetActive(false);
			TICouncilorState ticouncilorState = base.activePlayer.activeCouncilors.FirstOrDefault<TICouncilorState>((TICouncilorState x) => x.activeMission == null);
			if (ticouncilorState != null)
			{
				TIUtilities.GotoGameState(ticouncilorState, false, true, true);
			}
		}

		// Token: 0x17000EA7 RID: 3751
		// (get) Token: 0x06004C9D RID: 19613 RVA: 0x00205ABB File Offset: 0x00203CBB
		// (set) Token: 0x06004C9E RID: 19614 RVA: 0x00205ACD File Offset: 0x00203CCD
		public bool modifierPanesOpen
		{
			get
			{
				return this._modifierAnimator.GetBool("IsOpen");
			}
			set
			{
				if (this._modifierAnimator.gameObject.activeSelf)
				{
					this._modifierAnimator.SetBool("IsOpen", value);
				}
			}
		}

		// Token: 0x06004C9F RID: 19615 RVA: 0x00205AF2 File Offset: 0x00203CF2
		public void OnGotoMyCouncilorClicked()
		{
			SoundEffectController.PlaySelectSound(this.myCouncilor);
			TIUtilities.GotoGameState(this.myCouncilor, true, true, true);
		}

		// Token: 0x06004CA0 RID: 19616 RVA: 0x00205B0D File Offset: 0x00203D0D
		public void OnGotoEnemyCouncilorClicked()
		{
			SoundEffectController.PlaySelectSound(this.enemyCouncilor);
			TIUtilities.GotoGameState(new CouncilorView(this.enemyCouncilor, base.activePlayer), base.activePlayer.HasIntelOnCouncilorLocation(this.enemyCouncilor), true, true, true);
		}

		// Token: 0x06004CA1 RID: 19617 RVA: 0x00205B44 File Offset: 0x00203D44
		public void ItemSelected()
		{
			this.myCouncilorPanel.enabled = false;
			this.myCouncilorPanelGraphicRaycaster.enabled = false;
			if (TIPlayerProfileManager.useCouncilorVideo && this.myCouncilorVideo != null)
			{
				this.clearingMyCouncilorVideo = true;
				this.myCouncilorVideo.clip = null;
				this.myCouncilorVideo.Stop();
			}
			TIUtilities.GotoGameState(this.myCouncilor, false, true, true);
			GameControl.eventManager.TriggerEvent(new CouncilorDetailRequested(this.myCouncilor), null, Array.Empty<object>());
		}

		// Token: 0x06004CA2 RID: 19618 RVA: 0x00205BC5 File Offset: 0x00203DC5
		public void OnClickCloseMyCouncilorPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.SelectCouncilorTutorialController.HideTutorial();
			base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
		}

		// Token: 0x06004CA3 RID: 19619 RVA: 0x00205BEF File Offset: 0x00203DEF
		public void OnClickExitEnemyCouncilorPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x06004CA4 RID: 19620 RVA: 0x00205C0E File Offset: 0x00203E0E
		public void OnClickCloseTargetSelectionButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.ShutdownTargetSelection(true);
		}

		// Token: 0x06004CA5 RID: 19621 RVA: 0x00205C23 File Offset: 0x00203E23
		public void OnClickAbortMissionButtion()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			this.abortConfirmUI.SetActive(true);
		}

		// Token: 0x06004CA6 RID: 19622 RVA: 0x00205C40 File Offset: 0x00203E40
		public void OnClickAbortConfirmButtion()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmMission", false, false);
			this.abortConfirmUI.SetActive(false);
			this.myCouncilor.faction.playerControl.StartAction(new AbortMission(this.myCouncilor, false, TIMissionState.AbortReason.VoluntaryAbort, null, ""));
		}

		// Token: 0x06004CA7 RID: 19623 RVA: 0x00205C8E File Offset: 0x00203E8E
		public void OnClickCancelAbortButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.abortConfirmUI.SetActive(false);
		}

		// Token: 0x06004CA8 RID: 19624 RVA: 0x00205CA8 File Offset: 0x00203EA8
		private void OnCouncilorAutomationStatusChanged(CouncilorChangesAutoDefenseMode e)
		{
			if (e.councilor == this.myCouncilor)
			{
				this.SetAutomateButtonText();
			}
		}

		// Token: 0x06004CA9 RID: 19625 RVA: 0x00205CC4 File Offset: 0x00203EC4
		public void OnClickAutomateButton()
		{
			this.myCouncilor.faction.playerControl.StartAction(new ToggleAutomateCouncilorAction(this.myCouncilor, !this.myCouncilor.permanentDefenseMode));
			if (TIMissionPhaseState.InMissionPhase() && this.myCouncilor.permanentDefenseMode && !this.myCouncilor.HasMission)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			if (this.myCouncilor.permanentDefenseMode)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
		}

		// Token: 0x06004CAA RID: 19626 RVA: 0x00205D53 File Offset: 0x00203F53
		private void SetAutomateButtonText()
		{
			if (this.myCouncilor.permanentDefenseMode)
			{
				this.automateButtonText.SetText(Loc.T("UI.Councilor.CancelAutomateCouncilor"));
				return;
			}
			this.automateButtonText.SetText(Loc.T("UI.Councilor.AutomateCouncilor"));
		}

		// Token: 0x06004CAB RID: 19627 RVA: 0x00205D90 File Offset: 0x00203F90
		private string SetAutomateCouncilorTooltip()
		{
			if (this.myCouncilor != null)
			{
				string text = (from x in this.myCouncilor.GetPossibleMissionList(false, false, false, null, false)
					where x.allowedForAutoDefense
					select x.displayName).ToCommaSeparatedString<string>(null);
				return Loc.T("UI.Councilor.AutomateTooltip", new object[] { text });
			}
			return string.Empty;
		}

		// Token: 0x06004CAC RID: 19628 RVA: 0x00205E24 File Offset: 0x00204024
		private void ForcePlayerNewCouncilor()
		{
			if (!this.needToForceCouncilorSelection)
			{
				return;
			}
			TIUtilities.GotoGameState(this.nextCouncilor, false, true, true, true, false, -1f);
		}

		// Token: 0x06004CAD RID: 19629 RVA: 0x00205E44 File Offset: 0x00204044
		private void ForcePlayerNewCouncilorAndFocusCamera()
		{
			TIUtilities.GotoGameState(this.nextCouncilor, true, true, true, true, false, -1f);
		}

		// Token: 0x06004CAE RID: 19630 RVA: 0x00205E5C File Offset: 0x0020405C
		public void CouncilorSelected(TICouncilorState councilor)
		{
			if (councilor != null)
			{
				if (!this.Visible())
				{
					this.Show();
				}
				if (councilor.faction == base.activePlayer && !GeneralControlsController.CurrentlyTargetingStateType(typeof(TICouncilorState)))
				{
					if (base.canvasManager.GetActiveInfoPanel() == InfoPanel.None)
					{
						GeneralControlsController.SetUIOtherSelectedState(null);
					}
					this.SetMyCouncilor(councilor);
					base.canvasManager.SetActiveAssetPanel(AssetPanel.MyCouncilor, this.myCouncilorPanel.gameObject.GetComponent<RectTransform>().sizeDelta.y);
					this.SetMyCouncilorPanel();
					return;
				}
				if (councilor.faction != base.activePlayer || GeneralControlsController.CurrentValidTarget(councilor))
				{
					this.SetEnemyCouncilor(councilor);
					base.canvasManager.SetActiveInfoPanel(InfoPanel.CouncilorDetail, 0f);
					this.SetEnemyCouncilorPanel();
				}
			}
		}

		// Token: 0x06004CAF RID: 19631 RVA: 0x00205F28 File Offset: 0x00204128
		public void SetMyCouncilor(TICouncilorState councilor)
		{
			this.needToForceCouncilorSelection = false;
			if (this.myCouncilor != null)
			{
				if (this.myCouncilor != councilor && GeneralControlsController.UIPlayerInTargetingMode)
				{
					this.ShutdownTargetSelection(true);
				}
				if (this.myCouncilor != councilor)
				{
					councilor.PlaySelectionVoice();
				}
				this.RemoveMyCouncilorListeners();
			}
			this.myCouncilor = councilor;
			GeneralControlsController.SetUISelectedAssetState(councilor);
			if (TIMissionPhaseState.InMissionPhase())
			{
				councilor.faction.CompleteMilestone(CampaignMilestone.TutorialSelectCouncilor);
			}
			this.AddMyCouncilorListeners();
		}

		// Token: 0x06004CB0 RID: 19632 RVA: 0x00205FA8 File Offset: 0x002041A8
		public void SetEnemyCouncilor(TICouncilorState councilor)
		{
			if (this.enemyCouncilor != null)
			{
				this.RemoveEnemyCouncilorListeners();
			}
			if (this.enemyCouncilor != councilor && councilor.turned && councilor.agentForFaction == base.activePlayer)
			{
				councilor.PlaySelectionVoice();
			}
			this.enemyCouncilor = councilor;
			GeneralControlsController.SetUIOtherSelectedState(councilor);
			this.AddEnemyCouncilorListeners();
		}

		// Token: 0x06004CB1 RID: 19633 RVA: 0x0020600C File Offset: 0x0020420C
		public void ReverseSelectionTriggered(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
		{
			this.SetMyCouncilor(councilor);
			if (!this.Visible())
			{
				this.Show();
			}
			this.myCouncilorPanel.enabled = true;
			this.myCouncilorPanelGraphicRaycaster.enabled = true;
			this.UpdateMyCouncilorPanel();
			this.ShowMissionIcons(true);
			this.UpdateCouncilorActionBar();
			this.activeButton = this.FindActionButton(mission);
			this.OnMissionSelected(this.activeButton, target);
			this.currentTargeting.ForceTarget(target);
		}

		// Token: 0x06004CB2 RID: 19634 RVA: 0x00206080 File Offset: 0x00204280
		private void ResetMissionDisplayElements()
		{
			this.activeButton = null;
			this.ShowMissionDetails(false);
			this.ShowMissionIcons(false);
			this.ShowMissionOutome(false);
			if (this.UseResourceSlider || this.UseFixedResourceCost)
			{
				this.SetResourceAmount(0f);
			}
			this.actionOptionsPanel.SetActive(false);
			this.CouncilorMissionCanvasUITutorial.HideTutorial();
		}

		// Token: 0x06004CB3 RID: 19635 RVA: 0x002060DC File Offset: 0x002042DC
		private void AddMyCouncilorListeners()
		{
			GameControl.eventManager.AddListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.UpdateMyCouncilorPanel), null, this.myCouncilor, true, false);
			GameControl.eventManager.AddListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateMyCouncilorPanel), null, this.myCouncilor, true, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateMyCouncilorPanel), null, this.myCouncilor, true, false);
			GameControl.eventManager.AddListener<CouncilorChangesAutoDefenseMode>(new EventManager.EventDelegate<CouncilorChangesAutoDefenseMode>(this.OnCouncilorAutomationStatusChanged), null, this.myCouncilor, true, false);
			GameControl.eventManager.AddListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnFactionResourcesUpdated), null, base.activePlayer, false, false);
		}

		// Token: 0x06004CB4 RID: 19636 RVA: 0x00206184 File Offset: 0x00204384
		private void RemoveMyCouncilorListeners()
		{
			GameControl.eventManager.RemoveListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.UpdateMyCouncilorPanel), null);
			GameControl.eventManager.RemoveListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateMyCouncilorPanel), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateMyCouncilorPanel), null);
			GameControl.eventManager.RemoveListener<CouncilorChangesAutoDefenseMode>(new EventManager.EventDelegate<CouncilorChangesAutoDefenseMode>(this.OnCouncilorAutomationStatusChanged), null);
			GameControl.eventManager.RemoveListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnFactionResourcesUpdated), null);
		}

		// Token: 0x06004CB5 RID: 19637 RVA: 0x00206204 File Offset: 0x00204404
		private void SetMyCouncilorPanel()
		{
			if (this.myCouncilor != null)
			{
				if (!this.myCouncilorPanel.enabled)
				{
					this.myCouncilorPanel.enabled = true;
					this.myCouncilorPanelGraphicRaycaster.enabled = true;
					base.canvasManager.SetActiveAssetPanel(AssetPanel.MyCouncilor, this.myCouncilorPanel.gameObject.GetComponent<RectTransform>().sizeDelta.y);
				}
				this.UpdateMyCouncilorPanel();
				if (TIMissionPhaseState.InMissionPhase())
				{
					this.ShowMissionIcons(true);
					this.UpdateCouncilorActionBar();
					if (this.activeButton != null && this.missionTemplate != null)
					{
						this.UpdateAllMissionData(true);
						return;
					}
				}
			}
			else
			{
				if (TIPlayerProfileManager.useCouncilorVideo && this.myCouncilorVideo != null)
				{
					this.myCouncilorVideo.clip = null;
					this.myCouncilorVideo.Stop();
				}
				base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
			}
		}

		// Token: 0x06004CB6 RID: 19638 RVA: 0x002062E4 File Offset: 0x002044E4
		private void AddEnemyCouncilorListeners()
		{
			GameControl.eventManager.AddListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.UpdateEnemyCouncilorPanel), null, this.enemyCouncilor, true, false);
			GameControl.eventManager.AddListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateEnemyCouncilorPanel), null, this.enemyCouncilor, true, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateEnemyCouncilorPanel), null, this.enemyCouncilor, true, false);
		}

		// Token: 0x06004CB7 RID: 19639 RVA: 0x00206350 File Offset: 0x00204550
		private void RemoveEnemyCouncilorListeners()
		{
			GameControl.eventManager.RemoveListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.UpdateEnemyCouncilorPanel), null);
			GameControl.eventManager.RemoveListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateEnemyCouncilorPanel), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateEnemyCouncilorPanel), null);
		}

		// Token: 0x06004CB8 RID: 19640 RVA: 0x002063A4 File Offset: 0x002045A4
		private void SetEnemyCouncilorPanel()
		{
			if (this.enemyCouncilor != null && this.enemyCouncilor.faction != null)
			{
				if (!this.enemyCouncilorInfoPanel.enabled)
				{
					this.enemyCouncilorInfoPanel.enabled = true;
					this.enemyCouncilorGraphicRaycaster.enabled = true;
					base.canvasManager.SetActiveInfoPanel(InfoPanel.CouncilorDetail, 0f);
				}
				this.UpdateEnemyCouncilorPanel();
				return;
			}
			if (TIPlayerProfileManager.useCouncilorVideo && this.enemyCouncilorVideo != null)
			{
				this.enemyCouncilorVideo.Stop();
			}
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x06004CB9 RID: 19641 RVA: 0x00206440 File Offset: 0x00204640
		private void ResetDisplay()
		{
			this.ResetMissionDisplayElements();
			this.UpdateMyCouncilorPanel();
			this.UpdateEnemyCouncilorPanel();
		}

		// Token: 0x06004CBA RID: 19642 RVA: 0x00206454 File Offset: 0x00204654
		private void UpdateMyCouncilorPanel()
		{
			if (this.myCouncilorPanel.enabled)
			{
				if (TIPlayerProfileManager.useCouncilorVideo && !this.clearingMyCouncilorVideo)
				{
					this.myCouncilorVideo.clip = GameControl.assetLoader.LoadAsset<VideoClip>(this.myCouncilor.videoResource);
					this.myCouncilorVideo.gameObject.SetActive(true);
					this.myCouncilorStillImage.sprite = null;
					this.myCouncilorStillImage.enabled = false;
					if (!this.myCouncilorVideo.isPlaying)
					{
						TIUtilities.TryPrepareVideo(this.myCouncilorVideo);
						base.StartCoroutine(this.PlayVideoWhenPrepared(this.myCouncilorVideo));
					}
				}
				else
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(this.myCouncilor.portraitResource, this.myCouncilorStillImage);
					this.myCouncilorStillImage.enabled = true;
					this.myCouncilorVideo.clip = null;
					this.myCouncilorVideo.Stop();
					this.myCouncilorVideo.gameObject.SetActive(false);
				}
				this.clearingMyCouncilorVideo = false;
				CouncilorView viewofCouncilor = base.activePlayer.GetViewofCouncilor(this.myCouncilor);
				this.councilorName.SetText(this.myCouncilor.displayName);
				this.councilorCurrentMission.text = viewofCouncilor.GetCurrentMissionString(true, false, true);
				this.abortButtonGameObject.SetActive(this.myCouncilor.HasMission && !TIMissionPhaseState.InMissionPhase());
				this.councilorType.text = this.myCouncilor.typeTemplate.displayName;
				this.factionIcon.sprite = this.myCouncilor.faction.factionIcon256;
				this.UpdateMyCouncilorBackground(viewofCouncilor);
				base.StartCoroutine(this.UpdateCouncilorBackgroundNextFrame(viewofCouncilor, false));
				this.factionIcon.enabled = true;
				CouncilGridController.SetCouncilorXPText(this.myCouncilor, this.councilorXPText, true);
				this.per.text = viewofCouncilor.GetAttributeString(CouncilorAttribute.Persuasion);
				this.inv.text = viewofCouncilor.GetAttributeString(CouncilorAttribute.Investigation);
				this.esp.text = viewofCouncilor.GetAttributeString(CouncilorAttribute.Espionage);
				this.cmd.text = viewofCouncilor.GetAttributeString(CouncilorAttribute.Command);
				this.adm.text = viewofCouncilor.GetAttributeString(CouncilorAttribute.Administration);
				this.sci.text = viewofCouncilor.GetAttributeString(CouncilorAttribute.Science);
				this.sec.text = viewofCouncilor.GetAttributeString(CouncilorAttribute.Security);
				this.loy.text = viewofCouncilor.GetAttributeString(CouncilorAttribute.Loyalty);
				this.myLoyaltyTip.SetDelegate("BodyText", () => CouncilorMissionCanvasController.LoyaltyTip(base.activePlayer, this.myCouncilor));
				StringBuilder stringBuilder = new StringBuilder();
				if (this.myCouncilor.detained || viewofCouncilor.turned)
				{
					if (this.myCouncilor.detained)
					{
						if (this.myCouncilor.detainingFaction == this.myCouncilor.faction)
						{
							stringBuilder.Append(Loc.T("UI.Councilor.SelfDetainedTooltip", new object[] { this.myCouncilor.detainedReleaseDate.ToCustomDateString() })).AppendLine().AppendLine()
								.AppendLine(Loc.T("UI.Councilor.AddressSelfDetained"))
								.ToString();
						}
						else
						{
							stringBuilder.Append(Loc.T("UI.Councilor.DetainedTooltip", new object[]
							{
								this.myCouncilor.detainingFaction.displayNameWithColor,
								this.myCouncilor.detainedReleaseDate.ToCustomDateString()
							})).AppendLine().AppendLine()
								.AppendLine(Loc.T("UI.Councilor.AddressDetained"))
								.ToString();
						}
						stringBuilder.AppendLine();
						if (!viewofCouncilor.turned)
						{
							GameControl.assetLoader.LoadAssetForImageAssignment("councilor_missions/ICO_detain_off", this.statusIcon);
							this.statusText.SetText(Loc.T("UI.Councilor.Detained"));
						}
					}
					if (viewofCouncilor.turned)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment("councilor_missions/ICO_turn_off", this.statusIcon);
						this.statusText.SetText(Loc.T("UI.Councilor.Traitor"));
						stringBuilder.Append(Loc.T("UI.Councilor.TurnedTooltip", new object[]
						{
							this.myCouncilor.faction.displayName,
							viewofCouncilor.agentForFaction.displayNameWithColor
						})).Append(" ").AppendLine(Loc.T("UI.Councilor.AddressTraitor"));
					}
					this.statusIconTooltip.SetText("BodyText", stringBuilder.ToString());
					this.statusTextTooltip.SetText("BodyText", stringBuilder.ToString());
					this.statusText.gameObject.SetActive(true);
					this.statusIcon.gameObject.SetActive(true);
				}
				else
				{
					this.statusText.gameObject.SetActive(false);
					this.statusIcon.gameObject.SetActive(false);
				}
				if (this.myCouncilor.knowsIveBeenSeenBy.Count > 0)
				{
					this.trackingMeList.SetListSize<FactionIconGridItemController>(this.myCouncilor.knowsIveBeenSeenBy.Count, false, false);
					int num = 0;
					using (IEnumerator<object> enumerator = this.trackingMeList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (CouncilorMissionCanvasController.<>o__231.<>p__0 == null)
							{
								CouncilorMissionCanvasController.<>o__231.<>p__0 = CallSite<Func<CallSite, object, FactionIconGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionIconGridItemController), typeof(CouncilorMissionCanvasController)));
							}
							CouncilorMissionCanvasController.<>o__231.<>p__0.Target(CouncilorMissionCanvasController.<>o__231.<>p__0, enumerator.Current).SetListItem(this.myCouncilor.knowsIveBeenSeenBy[num++]);
						}
					}
					this.trackingMePanel.SetActive(true);
				}
				else
				{
					this.trackingMePanel.SetActive(false);
				}
				this.SetAutomateButtonText();
			}
		}

		// Token: 0x06004CBB RID: 19643 RVA: 0x002069D4 File Offset: 0x00204BD4
		private void UpdateEnemyCouncilorPanel()
		{
			if (this.enemyCouncilorInfoPanel.enabled)
			{
				CouncilorMissionCanvasController.<>c__DisplayClass232_0 CS$<>8__locals1;
				CS$<>8__locals1.enemyCouncilorView = new CouncilorView(this.enemyCouncilor, base.activePlayer);
				CS$<>8__locals1.activateDataPanel = false;
				if (!base.activePlayer.HasIntelOnCouncilorLocation(this.enemyCouncilor))
				{
					base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
					return;
				}
				if (base.activePlayer.HasIntelOnCouncilorBasicData(this.enemyCouncilor))
				{
					if (this.enemyCouncilor.isAlien)
					{
						this.enemyCouncilorHeader.SetText(Loc.T("UI.Councilor.AlienHeader"));
					}
					else
					{
						this.enemyCouncilorHeader.SetText(Loc.T("UI.Councilor.Header"));
					}
					this.<UpdateEnemyCouncilorPanel>g__SetTraitList|232_1(false, ref CS$<>8__locals1);
					CouncilorIllustrationData missionPhaseIllustrationData = CS$<>8__locals1.enemyCouncilorView.missionPhaseIllustrationData;
					GameControl.assetLoader.LoadAssetForImageAssignment(missionPhaseIllustrationData.illustrationPath, this.enemyCouncilorBackgroundImage);
					this.enemyCouncilorBackgroundImage.transform.localPosition = missionPhaseIllustrationData.GetIllustrationLocalPosition(this.enemyCouncilorBackgroundImage, this.enemyCouncilorBackgroundImageInitialPosition);
					if (TIPlayerProfileManager.useCouncilorVideo)
					{
						this.enemyCouncilorVideo.clip = GameControl.assetLoader.LoadAsset<VideoClip>(this.enemyCouncilor.videoResource);
						this.enemyCouncilorVideo.enabled = true;
						this.enemyCouncilorVideo.gameObject.SetActive(true);
						this.enemyCouncilorStillImage.sprite = null;
						this.enemyCouncilorStillImage.enabled = false;
						if (!this.enemyCouncilorVideo.isPlaying)
						{
							TIUtilities.TryPrepareVideo(this.enemyCouncilorVideo);
							base.StartCoroutine(this.PlayVideoWhenPrepared(this.enemyCouncilorVideo));
						}
					}
					else
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(this.enemyCouncilor.portraitResource, this.enemyCouncilorStillImage);
						this.enemyCouncilorStillImage.enabled = true;
						this.enemyCouncilorVideo.clip = null;
						this.enemyCouncilorVideo.Stop();
						this.enemyCouncilorVideo.enabled = false;
						this.enemyCouncilorVideo.gameObject.SetActive(false);
					}
				}
				else
				{
					this.enemyCouncilorHeader.SetText(Loc.T("UI.Councilor.Header"));
					this.enemyCouncilorStillImage.sprite = null;
					this.enemyCouncilorStillImage.enabled = false;
					this.enemyCouncilorVideo.gameObject.SetActive(false);
				}
				this.enemyCouncilorName.text = CS$<>8__locals1.enemyCouncilorView.displayNameCurrent;
				this.enemyCouncilorCurrentMission.text = CS$<>8__locals1.enemyCouncilorView.GetCurrentMissionString(true, false, true);
				this.enemyCouncilorType.text = CS$<>8__locals1.enemyCouncilorView.councilorJobStringCurrent;
				if (base.activePlayer.HasIntelOnCouncilorBasicData(this.enemyCouncilor))
				{
					this.enemyFactionIcon.sprite = this.enemyCouncilor.faction.factionIcon256;
					this.enemyFactionIcon.enabled = true;
				}
				else
				{
					this.enemyFactionIcon.enabled = false;
				}
				this.enemyPer.text = CS$<>8__locals1.enemyCouncilorView.GetAttributeString(CouncilorAttribute.Persuasion);
				this.enemyInv.text = CS$<>8__locals1.enemyCouncilorView.GetAttributeString(CouncilorAttribute.Investigation);
				this.enemyEsp.text = CS$<>8__locals1.enemyCouncilorView.GetAttributeString(CouncilorAttribute.Espionage);
				this.enemyCmd.text = CS$<>8__locals1.enemyCouncilorView.GetAttributeString(CouncilorAttribute.Command);
				this.enemyAdm.text = CS$<>8__locals1.enemyCouncilorView.GetAttributeString(CouncilorAttribute.Administration);
				this.enemySci.text = CS$<>8__locals1.enemyCouncilorView.GetAttributeString(CouncilorAttribute.Science);
				this.enemySec.text = CS$<>8__locals1.enemyCouncilorView.GetAttributeString(CouncilorAttribute.Security);
				this.enemyLoy.text = CS$<>8__locals1.enemyCouncilorView.GetAttributeString(CouncilorAttribute.Loyalty);
				this.enemyLoyaltyTip.SetDelegate("BodyText", () => CouncilorMissionCanvasController.LoyaltyTip(base.activePlayer, this.enemyCouncilor));
				this.enemyCouncilorOrgsButton.SetActive(false);
				this.enemyCouncilorMissionsButton.SetActive(false);
				StringBuilder sb = new StringBuilder();
				if (CS$<>8__locals1.enemyCouncilorView.detained || CS$<>8__locals1.enemyCouncilorView.turned)
				{
					if (CS$<>8__locals1.enemyCouncilorView.detained)
					{
						sb.Append(Loc.T("UI.Councilor.DetainedTooltip", new object[]
						{
							this.enemyCouncilor.detainingFaction.displayNameWithColor,
							this.enemyCouncilor.detainedReleaseDate.ToCustomDateString()
						}));
						if (!CS$<>8__locals1.enemyCouncilorView.turned)
						{
							GameControl.assetLoader.LoadAssetForImageAssignment("councilor_missions/ICO_detain_off", this.enemyCouncilorStatusIcon);
							this.enemyCouncilorStatusText.SetText(Loc.T("UI.Councilor.Detained"));
						}
					}
					if (CS$<>8__locals1.enemyCouncilorView.turned)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment("councilor_missions/ICO_turn_off", this.enemyCouncilorStatusIcon);
						this.enemyCouncilorStatusText.SetText(Loc.T("UI.Councilor.Turned"));
						sb.AppendLine().AppendLine().Append(Loc.T("UI.Councilor.TurnedTooltip", new object[]
						{
							this.enemyCouncilor.faction.displayName,
							CS$<>8__locals1.enemyCouncilorView.agentForFaction.displayNameWithColor
						}))
							.Append(" ")
							.AppendLine(Loc.T("UI.Councilor.AddressTraitor"))
							.ToString();
						if (CS$<>8__locals1.enemyCouncilorView.playerCouncilAgent)
						{
							this.turnedEnemyCouncilorFailurePanel.SetActive(true);
							this.turnedEnemyCouncilorSlider.value = this.enemyCouncilor.autofailMissionsValue * 100f;
							this.SetAutoFailText();
						}
						else
						{
							this.turnedEnemyCouncilorFailurePanel.SetActive(false);
						}
					}
					else
					{
						this.turnedEnemyCouncilorFailurePanel.SetActive(false);
					}
					this.enemyCouncilorIconStatusTooltip.SetDelegate("BodyText", () => sb.ToString());
					this.enemyCouncilorTextStatusTooltip.SetDelegate("BodyText", () => sb.ToString());
					this.enemyCouncilorStatusText.gameObject.SetActive(true);
					this.enemyCouncilorStatusIcon.gameObject.SetActive(true);
				}
				else
				{
					this.turnedEnemyCouncilorFailurePanel.SetActive(false);
					this.enemyCouncilorStatusIcon.gameObject.SetActive(false);
					this.enemyCouncilorStatusText.gameObject.SetActive(false);
				}
				if (base.activePlayer.HasIntelOnCouncilorDetails(this.enemyCouncilor))
				{
					int num = 0;
					List<TIMissionTemplate> missionsList = CS$<>8__locals1.enemyCouncilorView.GetMissionsList(this.enemyCouncilor);
					this.enemyCouncilorMissionsList.SetListSize<MissionsListItemController>(missionsList.Count, false, false);
					using (IEnumerator<object> enumerator = this.enemyCouncilorMissionsList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (CouncilorMissionCanvasController.<>o__232.<>p__2 == null)
							{
								CouncilorMissionCanvasController.<>o__232.<>p__2 = CallSite<Func<CallSite, object, MissionsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(MissionsListItemController), typeof(CouncilorMissionCanvasController)));
							}
							CouncilorMissionCanvasController.<>o__232.<>p__2.Target(CouncilorMissionCanvasController.<>o__232.<>p__2, enumerator.Current).SetSimpleListItem(missionsList[num++], this.enemyCouncilor);
						}
					}
					this.missionsTabController.SetSize(39f, 0f, 24f, missionsList.Count);
					int num2 = 0;
					List<TIOrgState> orgs = CS$<>8__locals1.enemyCouncilorView.orgs;
					this.enemyCouncilorOrgsList.SetListSize<OrgListItemController>(orgs.Count, false, false);
					using (IEnumerator<object> enumerator = this.enemyCouncilorOrgsList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (CouncilorMissionCanvasController.<>o__232.<>p__3 == null)
							{
								CouncilorMissionCanvasController.<>o__232.<>p__3 = CallSite<Func<CallSite, object, OrgListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrgListItemController), typeof(CouncilorMissionCanvasController)));
							}
							CouncilorMissionCanvasController.<>o__232.<>p__3.Target(CouncilorMissionCanvasController.<>o__232.<>p__3, enumerator.Current).UpdateListItem(orgs[num2++]);
						}
					}
					this.orgsTabController.SetSize(39f, 0f, 24f, orgs.Count);
					if (this.enemyCouncilor.isHuman)
					{
						this.enemyCouncilorHometownObject.SetActive(true);
						this.enemyCouncilorAgeObject.SetActive(true);
						this.enemyCouncilorTraitsListHeader.SetActive(true);
						this.enemyCouncilorHometown.SetText(Loc.T("UI.Councilor.VerboseHometown", new object[] { CS$<>8__locals1.enemyCouncilorView.councilorHomeTown }));
						this.enemyCouncilorAge.SetText(Loc.T("UI.Councilor.AgeColon", new object[] { CS$<>8__locals1.enemyCouncilorView.councilorAge }));
						this.<UpdateEnemyCouncilorPanel>g__SetTraitList|232_1(true, ref CS$<>8__locals1);
					}
					else
					{
						this.enemyCouncilorTraitsListHeader.SetActive(false);
						this.<UpdateEnemyCouncilorPanel>g__SetTraitList|232_1(false, ref CS$<>8__locals1);
					}
					CS$<>8__locals1.activateDataPanel = true;
					this.enemyCouncilorOrgsButton.SetActive(orgs.Count > 0);
					this.enemyCouncilorMissionsButton.SetActive(missionsList.Count > 0);
				}
				if (this.enemyCouncilorTabManager.activeTab != null)
				{
					if (!CS$<>8__locals1.activateDataPanel || !this.enemyCouncilorTabManager.activeTab.TabButton.gameObject.activeSelf)
					{
						this.enemyCouncilorTabManager.activeTab.Hide();
						this.enemyCouncilorTabManager.ClearActiveTab();
					}
					else
					{
						this.enemyCouncilorTabManager.activeTab.UpdateSize();
					}
				}
				this.UpdateEnemyCouncilorBackground(CS$<>8__locals1.enemyCouncilorView);
				base.StartCoroutine(this.UpdateCouncilorBackgroundNextFrame(CS$<>8__locals1.enemyCouncilorView, true));
				this.enemyCouncilorDataPanel.SetActive(CS$<>8__locals1.activateDataPanel);
			}
		}

		// Token: 0x06004CBC RID: 19644 RVA: 0x002072FC File Offset: 0x002054FC
		private void UpdateMyCouncilorBackground(CouncilorView councilorView)
		{
			if (this.myCouncilor == null)
			{
				return;
			}
			CouncilorIllustrationData illustrationData = this.myCouncilor.GetIllustrationData();
			GameControl.assetLoader.LoadAssetForImageAssignment(illustrationData.illustrationPath, this.councilorBackgroundImage);
			this.councilorBackgroundImage.transform.localPosition = illustrationData.GetIllustrationLocalPosition(this.councilorBackgroundImage, this.myCouncilorBackgroundImageInitialPosition);
		}

		// Token: 0x06004CBD RID: 19645 RVA: 0x0020735D File Offset: 0x0020555D
		private IEnumerator UpdateCouncilorBackgroundNextFrame(CouncilorView councilorView, bool isEnemyCouncilor)
		{
			yield return null;
			yield return new WaitForEndOfFrame();
			if (isEnemyCouncilor)
			{
				this.UpdateEnemyCouncilorBackground(councilorView);
			}
			else
			{
				this.UpdateMyCouncilorBackground(councilorView);
			}
			yield break;
		}

		// Token: 0x06004CBE RID: 19646 RVA: 0x0020737C File Offset: 0x0020557C
		private void UpdateEnemyCouncilorBackground(CouncilorView councilorView)
		{
			if (this.enemyCouncilor == null)
			{
				return;
			}
			if (base.activePlayer.HasIntelOnCouncilorBasicData(this.enemyCouncilor))
			{
				CouncilorIllustrationData missionPhaseIllustrationData = councilorView.missionPhaseIllustrationData;
				GameControl.assetLoader.LoadAssetForImageAssignment(missionPhaseIllustrationData.illustrationPath, this.enemyCouncilorBackgroundImage);
				this.enemyCouncilorBackgroundImage.transform.localPosition = missionPhaseIllustrationData.GetIllustrationLocalPosition(this.enemyCouncilorBackgroundImage, this.enemyCouncilorBackgroundImageInitialPosition);
				return;
			}
			CouncilorIllustrationData unknownIllustrationData = TICouncilorState.GetUnknownIllustrationData(this.enemyCouncilor);
			GameControl.assetLoader.LoadAssetForImageAssignment(unknownIllustrationData.illustrationPath, this.enemyCouncilorBackgroundImage);
			this.enemyCouncilorBackgroundImage.transform.localPosition = unknownIllustrationData.GetIllustrationLocalPosition(this.enemyCouncilorBackgroundImage, this.enemyCouncilorBackgroundImageInitialPosition);
		}

		// Token: 0x06004CBF RID: 19647 RVA: 0x00207432 File Offset: 0x00205632
		private IEnumerator PlayVideoWhenPrepared(VideoPlayer videoPlayer)
		{
			while (!videoPlayer.isPrepared)
			{
				yield return null;
			}
			if (videoPlayer.gameObject.activeInHierarchy)
			{
				string text = "Attempting to play CouncilorMissionCanvasController Video:";
				VideoClip clip = videoPlayer.clip;
				Debug.Log(text + ((clip != null) ? clip.name : null));
				TIUtilities.TryPlayVideo(videoPlayer);
				string text2 = "Started Playing CouncilorMissionCanvasController Video:";
				VideoClip clip2 = videoPlayer.clip;
				Debug.Log(text2 + ((clip2 != null) ? clip2.name : null));
			}
			yield break;
		}

		// Token: 0x06004CC0 RID: 19648 RVA: 0x00207444 File Offset: 0x00205644
		public static string LoyaltyTip(TIFactionState activePlayer, TICouncilorState viewedCouncilor)
		{
			if (activePlayer.HasIntelOnCouncilorSecrets(viewedCouncilor))
			{
				return Loc.T("UI.Councilor.CouncilorLoyaltyTip");
			}
			if (activePlayer.HasIntelOnCouncilorBasicData(viewedCouncilor) && activePlayer.lastRecordedLoyalty.ContainsKey(viewedCouncilor))
			{
				return Loc.T("UI.Councilor.LastKnownLoyaltyTip", new object[] { activePlayer.lastRecordedLoyalty[viewedCouncilor].ToString() });
			}
			return Loc.T("UI.Councilor.LastKnownLoyaltyPreTip");
		}

		// Token: 0x06004CC1 RID: 19649 RVA: 0x002074B0 File Offset: 0x002056B0
		public void CloseMyCouncilorPanel()
		{
			if (this.myCouncilorPanel != null)
			{
				this.myCouncilorPanel.enabled = false;
				this.myCouncilorPanelGraphicRaycaster.enabled = false;
			}
			if (this.actionsPanel != null)
			{
				this.actionsPanel.SetActive(false);
			}
			this.ShutdownTargetSelection(true);
			GeneralControlsController.ConditionalCancelSelectedAsset(this.myCouncilor);
			this.myCouncilor = null;
			if (TIPlayerProfileManager.useCouncilorVideo && this.myCouncilorVideo != null)
			{
				this.myCouncilorVideo.clip = null;
				this.myCouncilorVideo.Stop();
			}
			this.CheckForCloseCanvas();
			this.HideTutorials();
		}

		// Token: 0x06004CC2 RID: 19650 RVA: 0x00207550 File Offset: 0x00205750
		public void CloseEnemyCouncilorPanel()
		{
			if (this.enemyCouncilorInfoPanel != null)
			{
				this.enemyCouncilorInfoPanel.enabled = false;
				this.enemyCouncilorGraphicRaycaster.enabled = false;
			}
			GeneralControlsController.ConditionalCancelSelectedOtherState(this.enemyCouncilor);
			this.enemyCouncilor = null;
			if (TIPlayerProfileManager.useCouncilorVideo && this.enemyCouncilorVideo != null)
			{
				this.enemyCouncilorVideo.clip = null;
				this.enemyCouncilorVideo.Stop();
			}
			this.CheckForCloseCanvas();
		}

		// Token: 0x06004CC3 RID: 19651 RVA: 0x002075C7 File Offset: 0x002057C7
		private void ShowMissionDetails(bool active = true)
		{
			if (this.missionInfoPanel != null)
			{
				this.missionInfoPanel.SetActive(active);
			}
		}

		// Token: 0x06004CC4 RID: 19652 RVA: 0x002075E4 File Offset: 0x002057E4
		private void SetMissionInfo(TIMissionTemplate missionType)
		{
			this.namePanel.SetActive(true);
			this.ShowMissionDetails(true);
			this.missionDisplayName.SetText(missionType.displayName);
			this.missionDescription.SetText(missionType.description);
			for (int i = 0; i < this.clockPips.Length; i++)
			{
				if (i <= missionType.resolutionOrder)
				{
					this.clockPips[i].SetPipStatus(true, false);
				}
				else
				{
					this.clockPips[i].SetPipStatus(false, false);
				}
			}
		}

		// Token: 0x06004CC5 RID: 19653 RVA: 0x00207664 File Offset: 0x00205864
		private void ShowMissionIcons(bool active = true)
		{
			bool activeSelf = this.actionsPanel.activeSelf;
			this.actionsPanel.SetActive(active);
			if (active && !activeSelf)
			{
				base.canvasManager.SetActiveAssetPanel(AssetPanel.MyCouncilor, this.myCouncilorPanel.gameObject.GetComponent<RectTransform>().sizeDelta.y);
				this.CouncilorMissionCanvasUITutorial.HideTutorial();
				this.SelectCouncilorTutorialController.HoldTutorial(CampaignMilestone.UITutorial_SelectCouncilor, false, true);
			}
		}

		// Token: 0x06004CC6 RID: 19654 RVA: 0x002076D4 File Offset: 0x002058D4
		private CouncilorMissionButtonController FindActionButton(TIMissionTemplate mission)
		{
			using (IEnumerator<object> enumerator = this.actionList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilorMissionCanvasController.<>o__243.<>p__0 == null)
					{
						CouncilorMissionCanvasController.<>o__243.<>p__0 = CallSite<Func<CallSite, object, CouncilorMissionButtonController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorMissionButtonController), typeof(CouncilorMissionCanvasController)));
					}
					CouncilorMissionButtonController councilorMissionButtonController = CouncilorMissionCanvasController.<>o__243.<>p__0.Target(CouncilorMissionCanvasController.<>o__243.<>p__0, enumerator.Current);
					if (councilorMissionButtonController.missionType == mission)
					{
						return councilorMissionButtonController;
					}
				}
			}
			return null;
		}

		// Token: 0x06004CC7 RID: 19655 RVA: 0x0020776C File Offset: 0x0020596C
		private void UpdateCouncilorActionBar()
		{
			TIMissionTemplate timissionTemplate = null;
			if (this.activeButton != null)
			{
				timissionTemplate = this.activeButton.missionType;
			}
			List<TIMissionTemplate> possibleMissionList = this.myCouncilor.GetPossibleMissionList(true, true, true, null, false);
			this.actionList.SetListSize<CouncilorMissionButtonController>(possibleMissionList.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.actionList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilorMissionCanvasController.<>o__245.<>p__0 == null)
					{
						CouncilorMissionCanvasController.<>o__245.<>p__0 = CallSite<Func<CallSite, object, CouncilorMissionButtonController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorMissionButtonController), typeof(CouncilorMissionCanvasController)));
					}
					CouncilorMissionButtonController councilorMissionButtonController = CouncilorMissionCanvasController.<>o__245.<>p__0.Target(CouncilorMissionCanvasController.<>o__245.<>p__0, enumerator.Current);
					councilorMissionButtonController.Init(this);
					councilorMissionButtonController.SetMissionData(possibleMissionList[num++], this.myCouncilor);
					councilorMissionButtonController.gameObject.SetActive(true);
				}
			}
			if (possibleMissionList.Count > 22)
			{
				using (IEnumerator<object> enumerator = this.actionList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CouncilorMissionCanvasController.<>o__245.<>p__1 == null)
						{
							CouncilorMissionCanvasController.<>o__245.<>p__1 = CallSite<Func<CallSite, object, CouncilorMissionButtonController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorMissionButtonController), typeof(CouncilorMissionCanvasController)));
						}
						CouncilorMissionButtonController councilorMissionButtonController2 = CouncilorMissionCanvasController.<>o__245.<>p__1.Target(CouncilorMissionCanvasController.<>o__245.<>p__1, enumerator.Current);
						if (!councilorMissionButtonController2.interactable)
						{
							councilorMissionButtonController2.transform.SetAsLastSibling();
						}
					}
				}
				if (timissionTemplate != null)
				{
					using (IEnumerator<object> enumerator = this.actionList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (CouncilorMissionCanvasController.<>o__245.<>p__2 == null)
							{
								CouncilorMissionCanvasController.<>o__245.<>p__2 = CallSite<Func<CallSite, object, CouncilorMissionButtonController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorMissionButtonController), typeof(CouncilorMissionCanvasController)));
							}
							CouncilorMissionButtonController councilorMissionButtonController3 = CouncilorMissionCanvasController.<>o__245.<>p__2.Target(CouncilorMissionCanvasController.<>o__245.<>p__2, enumerator.Current);
							if (councilorMissionButtonController3.missionType == timissionTemplate)
							{
								this.activeButton = councilorMissionButtonController3;
							}
						}
					}
				}
			}
		}

		// Token: 0x06004CC8 RID: 19656 RVA: 0x00207980 File Offset: 0x00205B80
		public void OnMissionSelected(CouncilorMissionButtonController button, TIGameState forcedTarget = null)
		{
			if (button == null)
			{
				return;
			}
			if (this.activeButton != button || this.activeButton.missionType != button.missionType || forcedTarget != null)
			{
				this.ShutdownTargetSelection(false);
				this.actionOptionsPanel.SetActive(true);
				this.activeButton = button;
				this.SetMissionInfo(this.activeButton.missionType);
				this.InitTargetSelection(forcedTarget);
				this.SetResourceAmount(0f);
				this.UpdateAllMissionData(true);
			}
		}

		// Token: 0x06004CC9 RID: 19657 RVA: 0x00207A05 File Offset: 0x00205C05
		public void OnMissionPointerEnter(CouncilorMissionButtonController button)
		{
			if (this.activeButton == null && button != null)
			{
				this.SetMissionInfo(button.missionType);
			}
		}

		// Token: 0x06004CCA RID: 19658 RVA: 0x00207A2A File Offset: 0x00205C2A
		public void OnMissionPointerExit(CouncilorMissionButtonController button)
		{
			if (this.activeButton == null)
			{
				this.ShowMissionDetails(false);
			}
		}

		// Token: 0x06004CCB RID: 19659 RVA: 0x00207A41 File Offset: 0x00205C41
		private void OnFactionResourcesUpdated(FactionResourcesUpdated e)
		{
			if (this.MissionHasCost)
			{
				this.UpdateResourcePanel();
			}
		}

		// Token: 0x17000EA8 RID: 3752
		// (get) Token: 0x06004CCC RID: 19660 RVA: 0x00207A51 File Offset: 0x00205C51
		private bool MissionHasCost
		{
			get
			{
				return this.activeButton != null && this.activeButton.missionType.cost != null && this.activeButton.missionType.cost.MeetsCondition(this.myCouncilor);
			}
		}

		// Token: 0x17000EA9 RID: 3753
		// (get) Token: 0x06004CCD RID: 19661 RVA: 0x00207A90 File Offset: 0x00205C90
		private bool UseResourceSlider
		{
			get
			{
				return this.MissionHasCost && this.activeButton.missionType.cost is TIMissionCost_Bonus;
			}
		}

		// Token: 0x17000EAA RID: 3754
		// (get) Token: 0x06004CCE RID: 19662 RVA: 0x00207AB4 File Offset: 0x00205CB4
		private bool UseFixedResourceCost
		{
			get
			{
				return this.MissionHasCost && this.activeButton.missionType.cost is TIMissionCost_Flat;
			}
		}

		// Token: 0x06004CCF RID: 19663 RVA: 0x00207AD8 File Offset: 0x00205CD8
		private void UpdateResourcePanel()
		{
			if (this.UseResourceSlider)
			{
				this.costPanel.gameObject.SetActive(true);
				this.resourcesSlider.value = 0f;
				this.resourcesSlider.maxValue = (float)Mathf.Clamp(this.myCouncilor.CurrentMaxSliderSteps(this.activeButton.missionType, 1f), 0, this.myCouncilor.MaxSliderSteps());
				this.resourcesSpend = 0f;
				this.resourceValue.text = 0.ToString();
				this.resourcesType.SetText(new StringBuilder(TIUtilities.InlineResourceStr(this.activeButton.missionType.cost.resourceType)).Append(TIUtilities.GetResourceString(this.activeButton.missionType.cost.resourceType)));
				this.fixedResourcesType.enabled = false;
				this.resourcesSliderObject.SetActive(true);
				this.resourcesType.enabled = true;
				this.resourceValue.enabled = true;
				return;
			}
			if (this.UseFixedResourceCost)
			{
				this.costPanel.gameObject.SetActive(true);
				this.fixedResourcesType.SetText(new StringBuilder(TIUtilities.InlineResourceStr(this.activeButton.missionType.cost.resourceType)).Append(TIUtilities.GetResourceString(this.activeButton.missionType.cost.resourceType)));
				this.resourcesSpend = this.activeButton.missionType.cost.GetCost(0f, this.myCouncilor, null);
				this.resourceValue.text = this.resourcesSpend.ToString(TIUtilities.DecimalPlaces((double)this.resourcesSpend, 1, 0));
				this.fixedResourcesType.enabled = true;
				this.resourceValue.enabled = true;
				this.resourcesSliderObject.SetActive(false);
				this.resourcesType.enabled = false;
				return;
			}
			this.costPanel.gameObject.SetActive(false);
			this.resourcesSliderObject.SetActive(false);
			this.resourcesType.enabled = false;
			this.fixedResourcesType.enabled = false;
			this.resourceValue.enabled = false;
			this.resourcesSpend = 0f;
		}

		// Token: 0x06004CD0 RID: 19664 RVA: 0x00207D0F File Offset: 0x00205F0F
		private void SetResourceAmount(float newValue)
		{
			if (this.UseResourceSlider && (newValue != this.resourcesSlider.value || this.resourcesSpend != 0f))
			{
				this.resourcesSlider.value = newValue;
				this.OnResourceSliderChangedValue(newValue);
			}
		}

		// Token: 0x06004CD1 RID: 19665 RVA: 0x00207D48 File Offset: 0x00205F48
		public void OnResourceSliderChangedValue(float newValue)
		{
			if (this.UseResourceSlider)
			{
				this.resourcesSpend = this.activeButton.missionType.cost.GetCost(newValue, this.myCouncilor, null);
				this.resourceValue.text = this.resourcesSpend.ToString("N0");
				this.resourcesType.SetText(new StringBuilder(TIUtilities.InlineResourceStr(this.activeButton.missionType.cost.resourceType)).Append(TIUtilities.GetResourceString(this.activeButton.missionType.cost.resourceType)));
				this.fixedResourcesType.enabled = false;
				this.UpdateMissionModifiers();
				GameControl.eventManager.TriggerEvent(new ResourcesSliderUpdated(newValue, this.currentTarget), null, Array.Empty<object>());
				base.activePlayer.CompleteMilestone(CampaignMilestone.TutorialUseMissionSlider);
			}
		}

		// Token: 0x06004CD2 RID: 19666 RVA: 0x00207E24 File Offset: 0x00206024
		private void InitTargetSelection(TIGameState forcedTarget = null)
		{
			this.currentTarget = null;
			this.confirmButton.enabled = false;
			if (this.activeButton == null || this.activeButton.missionType == null)
			{
				this.namePanel.SetActive(false);
				return;
			}
			(base.canvasManager.NationInfo as NationInfoController).CloseNuclearWeaponsPanel();
			Type targetingMethodType = this.activeButton.missionType.targetingMethodType;
			if (targetingMethodType == null)
			{
				return;
			}
			this.currentTargeting = Activator.CreateInstance(targetingMethodType) as TIMissionTargeting;
			this.currentTargeting.Init(this.activeButton.missionType, this.myCouncilor);
			this.currentTargeting.Activate();
			if (this.currentTargeting == null)
			{
				this.namePanel.SetActive(false);
				return;
			}
			if (forcedTarget != null)
			{
				this.currentTarget = forcedTarget;
			}
			else
			{
				this.currentTarget = this.currentTargeting.GetTargetted();
			}
			if (this.currentTargeting is TIMissionTargeting_Org)
			{
				this.SetOrgTargetPanel(this.activeButton.missionType.GetValidTargets(this.myCouncilor));
				this.OpenOrgTargetingPanel();
			}
			if (this.currentTarget != null)
			{
				TIUtilities.GotoSelectedStateUI(this.currentTarget, true);
			}
			GameControl.eventManager.AddListener<MissionTargettedEvent>(new EventManager.EventDelegate<MissionTargettedEvent>(this.OnNewMissionTarget), null, null, false, false);
			if (this.targetDropdown.enabled)
			{
				this.SetDropdownCaption();
			}
		}

		// Token: 0x06004CD3 RID: 19667 RVA: 0x00207F84 File Offset: 0x00206184
		private void UpdateAllMissionData(bool populateTargets)
		{
			if (this.missionTemplate != null)
			{
				if (populateTargets)
				{
					this.FillOutTargetDropdown(this.currentTargeting.GetPossibleTargets);
				}
				this.missionName.SetText(this.missionTemplate.displayName);
				if (this.currentTarget != null && this.reverseTargetOptionData.ContainsKey(this.currentTarget))
				{
					this.targetDropdown.value = this.reverseTargetOptionData[this.currentTarget];
					this.namePanel.SetActive(false);
					this.confirmButton.enabled = true;
				}
				else
				{
					this.namePanel.SetActive(true);
					this.confirmButton.enabled = false;
					if (this.forceAllowMissions)
					{
						this.namePanel.SetActive(false);
						this.confirmButton.enabled = true;
					}
				}
				if (this.targetDropdown.enabled)
				{
					this.SetDropdownCaption();
				}
				if (!this.missionTemplate.ContestedMission)
				{
					this.ShowMissionOutome(false);
					this.UpdateResourcePanel();
					this.CouncilorMissionCanvasUITutorial.HideTutorial();
					return;
				}
				this.UpdateMissionModifiers();
				this.ShowMissionOutome(this.currentTarget != null);
				this.UpdateResourcePanel();
				this.CouncilorMissionCanvasUITutorial.HoldTutorial(CampaignMilestone.UITutorial_CouncilorMissionControlsCanvas1, false, true);
			}
		}

		// Token: 0x06004CD4 RID: 19668 RVA: 0x002080C0 File Offset: 0x002062C0
		public void ShutdownTargetSelection(bool reset)
		{
			if (this.currentTargeting != null)
			{
				GameControl.eventManager.RemoveListener<MissionTargettedEvent>(new EventManager.EventDelegate<MissionTargettedEvent>(this.OnNewMissionTarget), null);
				this.currentTargeting.Shutdown();
				this.currentTargeting = null;
				this.ShowMissionDetails(false);
				this.successOrFailurePanel.SetActive(false);
				this.actionOptionsPanel.SetActive(false);
				this.CouncilorMissionCanvasUITutorial.HideTutorial();
				if (reset)
				{
					this.SetResourceAmount(0f);
					this.activeButton = null;
				}
				this.CloseOrgTargetingPanel();
			}
		}

		// Token: 0x06004CD5 RID: 19669 RVA: 0x00208143 File Offset: 0x00206343
		private void OnNewMissionTarget(MissionTargettedEvent e)
		{
			this.currentTarget = e.target;
			this.UpdateAllMissionData(false);
		}

		// Token: 0x06004CD6 RID: 19670 RVA: 0x00208158 File Offset: 0x00206358
		private void FillOutTargetDropdown(IList<TIGameState> targets)
		{
			this.targetDropdown.ClearOptions();
			this.targetOptionData = new Dictionary<int, TIGameState>();
			this.reverseTargetOptionData = new Dictionary<TIGameState, int>();
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<TIGameState, string> dictionary = new Dictionary<TIGameState, string>();
			Dictionary<TIGameState, float> contestedMissionValues = new Dictionary<TIGameState, float>();
			if (this.missionTemplate.ContestedMission)
			{
				for (int i = 0; i < targets.Count; i++)
				{
					TIGameState tigameState = targets[i];
					float num;
					string successChanceString = this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, out num, this.myCouncilor, tigameState, 0f, false, 2);
					contestedMissionValues.Add(tigameState, num);
					dictionary.Add(tigameState, successChanceString);
				}
				targets = targets.OrderBy<TIGameState, float>((TIGameState x) => contestedMissionValues[x]).ToList<TIGameState>();
			}
			Func<TIFactionState, bool> <>9__2;
			Func<TIFactionState, bool> <>9__3;
			for (int j = 0; j < targets.Count; j++)
			{
				TIGameState tigameState2 = targets[j];
				stringBuilder.Clear();
				Sprite sprite = null;
				List<TIFactionState> list;
				if (!tigameState2.isCouncilorState)
				{
					list = tigameState2.ref_factions;
				}
				else
				{
					(list = new List<TIFactionState>()).Add(tigameState2.ref_faction);
				}
				List<TIFactionState> list2 = list;
				bool flag;
				if (!this.missionTemplate.specialPost || !tigameState2.isRegionXenoformingState)
				{
					if (this.missionTemplate.hate.Any<float>((float x) => x > 0f))
					{
						if (list2 != null)
						{
							IEnumerable<TIFactionState> enumerable = list2;
							Func<TIFactionState, bool> func;
							if ((func = <>9__2) == null)
							{
								func = (<>9__2 = (TIFactionState x) => x.HasNAP(this.activePlayer, true));
							}
							if (!enumerable.Any<TIFactionState>(func))
							{
								IEnumerable<TIFactionState> enumerable2 = list2;
								Func<TIFactionState, bool> func2;
								if ((func2 = <>9__3) == null)
								{
									func2 = (<>9__3 = (TIFactionState x) => x.HasTruce(this.activePlayer, true));
								}
								if (!enumerable2.Any<TIFactionState>(func2))
								{
									goto IL_01B2;
								}
							}
							flag = true;
							goto IL_0208;
						}
						IL_01B2:
						flag = tigameState2.ref_faction != null && (tigameState2.ref_faction.HasNAP(base.activePlayer, true) || tigameState2.ref_faction.HasTruce(base.activePlayer, true));
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = TemplateManager.global.factionHateForBurnXenoforming > 0f;
				}
				IL_0208:
				if (flag)
				{
					stringBuilder.Append(TemplateManager.global.friendlyRelationsInlineSpritePath);
				}
				if (tigameState2.isCouncilorState)
				{
					CouncilorView viewofCouncilor = base.activePlayer.GetViewofCouncilor(tigameState2.ref_councilor);
					if (!(tigameState2.ref_faction != base.activePlayer) || tigameState2.ref_councilor.isAlien)
					{
						goto IL_02CA;
					}
					if (!viewofCouncilor.traits.Any<TITraitTemplate>(delegate(TITraitTemplate x)
					{
						if (x.specialTraitRule != SpecialTraitRule.GlobalPropagandaIfKilled)
						{
							return x.tags.Any<string>((string y) => y == "Dangerous");
						}
						return true;
					}))
					{
						if (!viewofCouncilor.orgs.Any<TIOrgState>((TIOrgState x) => x.grantsMarked))
						{
							goto IL_02CA;
						}
					}
					stringBuilder.Append(TemplateManager.global.warningInlineSpritePath);
					IL_030D:
					stringBuilder.Append(viewofCouncilor.displayNameCurrent);
					stringBuilder.Append(Loc.T("UI.MissionPhase.TargetParen", new object[] { TIUtilities.GetLocationString(viewofCouncilor.location, true, false) }));
					sprite = viewofCouncilor.councilorActionIcon64CurrentSprite;
					goto IL_044F;
					IL_02CA:
					if (base.activePlayer.GetIntel(tigameState2) >= TemplateManager.global.intelToSeeCouncilorBasicData && base.activePlayer.GetIntel(tigameState2) < TemplateManager.global.intelToSeeCouncilorDetails)
					{
						stringBuilder.Append(TemplateManager.global.investigationInlineSpritePath);
						goto IL_030D;
					}
					goto IL_030D;
				}
				else if (tigameState2.isRegionState || tigameState2.isRegionAlienEntity || tigameState2.isRegionSpaceFacility)
				{
					stringBuilder.Append(tigameState2.GetDisplayName(base.activePlayer));
					stringBuilder.Append(Loc.T("UI.MissionPhase.TargetSeries", new object[] { tigameState2.ref_nation.displayName }));
				}
				else if (tigameState2.isHabState)
				{
					stringBuilder.Append(TIUtilities.GetLocationString(tigameState2.ref_hab, true, false));
				}
				else if (tigameState2.isSpaceFleetState)
				{
					stringBuilder.Append(TIUtilities.GetLocationString(tigameState2.ref_fleet, true, false));
				}
				else if (tigameState2.isSpaceShipState)
				{
					stringBuilder.Append(TIUtilities.GetLocationString(tigameState2.ref_ship, true, false));
				}
				else if (tigameState2.isControlPointState)
				{
					stringBuilder.Append(tigameState2.GetDisplayName(base.activePlayer));
					sprite = tigameState2.ref_controlPoint.GetIcon(true, false);
				}
				else
				{
					stringBuilder.Append(tigameState2.GetDisplayName(base.activePlayer));
				}
				IL_044F:
				if (sprite == null)
				{
					sprite = TIUtilities.GetStateIcon(base.activePlayer, tigameState2, false);
				}
				if (this.missionTemplate.ContestedMission)
				{
					stringBuilder.Append(Loc.T("UI.MissionPhase.ToHit", new object[] { dictionary[tigameState2] }));
				}
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
				{
					text = stringBuilder.ToString(),
					image = sprite
				};
				this.targetDropdown.options.Add(optionData);
				this.targetOptionData.Add(j, targets[j]);
				this.reverseTargetOptionData.Add(targets[j], j);
			}
			this.SetDropdownCaption();
		}

		// Token: 0x06004CD7 RID: 19671 RVA: 0x0020866C File Offset: 0x0020686C
		private void SetDropdownCaption()
		{
			if (this.currentTarget != null)
			{
				string text;
				if (this.currentTarget.isCouncilorState)
				{
					text = base.activePlayer.GetViewofCouncilor(this.currentTarget.ref_councilor).displayNameCurrent;
				}
				else
				{
					text = this.currentTarget.GetDisplayName(base.activePlayer);
				}
				if (this.missionTemplate.ContestedMission)
				{
					this.targetDropdown.captionText.horizontalAlignment = HorizontalAlignmentOptions.Left;
					float num;
					text = new StringBuilder(text).Append(Loc.T("UI.MissionPhase.ToHit", new object[] { this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, out num, this.myCouncilor, this.currentTarget, 0f, false, 2) })).ToString();
				}
				else
				{
					this.targetDropdown.captionText.horizontalAlignment = HorizontalAlignmentOptions.Center;
				}
				this.targetDropdown.captionText.SetText(text);
			}
		}

		// Token: 0x06004CD8 RID: 19672 RVA: 0x0020875C File Offset: 0x0020695C
		public void OnTargetDropDownChanged()
		{
			TIGameState tigameState = this.currentTarget;
			this.currentTargeting.ForceTarget(this.targetOptionData[this.targetDropdown.value]);
			this.SetDropdownCaption();
			if (this.currentTarget.isCouncilorState)
			{
				EventManager eventManager = GameControl.eventManager;
				GameEvent gameEvent = new CouncilorSelectedOffMap(this.currentTarget.ref_councilor);
				string text = null;
				object[] array = new object[2];
				int num = 0;
				TIGameState tigameState2 = TIMissionPhaseState.CouncilorLastKnownLocation(base.activePlayer, this.currentTarget.ref_councilor);
				array[num] = ((tigameState2 != null) ? tigameState2.ref_region : null);
				int num2 = 1;
				object obj;
				if (tigameState == null || !tigameState.isCouncilorState)
				{
					obj = null;
				}
				else
				{
					TIGameState tigameState3 = TIMissionPhaseState.CouncilorLastKnownLocation(base.activePlayer, tigameState.ref_councilor);
					obj = ((tigameState3 != null) ? tigameState3.ref_region : null);
				}
				array[num2] = obj;
				eventManager.TriggerEvent(gameEvent, text, array);
			}
		}

		// Token: 0x06004CD9 RID: 19673 RVA: 0x00208818 File Offset: 0x00206A18
		public void InitializeOrgList()
		{
			this.orgListHeader.SetText(Loc.T("UI.OrgTargeting.OrgListHeader"));
			this.orgTargetingCanvas.enabled = false;
			this.orgTargetingCanvasRaycaster.enabled = false;
			this.orgMaximizeListButton.gameObject.SetActive(false);
			this.orgTargetMaxCount = (GameStateManager.AllFactions().Length - 1) * 6 * TIGlobalConfig.globalConfig.councilorMaxOrgs + (GameStateManager.AllFactions().Length - 1) * TIGlobalConfig.globalConfig.maxFactionOrgPoolSize;
			this.orgNameColumnHeader.SetText(Loc.T("UI.OrgTargeting.OrgNameColumnHeader"));
			this.orgTechColumnHeader.SetText(Loc.T("UI.OrgTargeting.OrgTechColumnHeader"));
			this.orgTargetingToHitTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.ToHit")).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingCouncilorCantGainTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.CouncilorCantGain")).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingCouncilorTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.CouncilorTip")).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingTierTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Tier")).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingOrgNameTip.SetText("BodyText", new StringBuilder().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingPersuasionTip.SetText("BodyText", new StringBuilder(TIUtilities.GetAttributeString(CouncilorAttribute.Persuasion)).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingInvestigationTip.SetText("BodyText", new StringBuilder(TIUtilities.GetAttributeString(CouncilorAttribute.Investigation)).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingEspionageTip.SetText("BodyText", new StringBuilder(TIUtilities.GetAttributeString(CouncilorAttribute.Espionage)).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingCommandTip.SetText("BodyText", new StringBuilder(TIUtilities.GetAttributeString(CouncilorAttribute.Command)).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingAdministrationTip.SetText("BodyText", new StringBuilder(TIUtilities.GetAttributeString(CouncilorAttribute.Administration)).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingScienceTip.SetText("BodyText", new StringBuilder(TIUtilities.GetAttributeString(CouncilorAttribute.Science)).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingSecurityTip.SetText("BodyText", new StringBuilder(TIUtilities.GetAttributeString(CouncilorAttribute.Security)).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingMoneyTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.MonthlyIncome", new object[] { TIUtilities.GetResourceString(FactionResource.Money) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingInfluenceTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.MonthlyIncome", new object[] { TIUtilities.GetResourceString(FactionResource.Influence) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingOperationsTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.MonthlyIncome", new object[] { TIUtilities.GetResourceString(FactionResource.Operations) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingResearchTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.MonthlyIncome", new object[] { TIUtilities.GetResourceString(FactionResource.Research) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingBoostTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.MonthlyIncome", new object[] { TIUtilities.GetResourceString(FactionResource.Boost) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingMissionControlGrantTip.SetText("BodyText", new StringBuilder(TIUtilities.GetResourceString(FactionResource.MissionControl)).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingProjectsTip.SetText("BodyText", new StringBuilder(TIUtilities.GetResourceString(FactionResource.Projects)).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingEconomyTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Economy, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingWelfareTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Welfare, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingEnvironmentTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Environment, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingKnowledgeTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Knowledge, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingGovernmentTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Government, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingUnityTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Unity, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingMilitaryTip.SetDelegate("BodyText", () => new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Military, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Military_FoundMilitary, false) })).AppendLine(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Military_BuildArmy, false) }))
				.AppendLine(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Military_BuildNavy, false) }))
				.AppendLine(TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSpaceDefenses) ? Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Military_BuildSpaceDefenses, false) }) : string.Empty)
				.AppendLine(Loc.T("UI.OrgTargeting.Sort"))
				.ToString());
			this.orgTargetingOppressionTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Oppression, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingFundingTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Funding, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingSpoilsTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Spoils, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingSpaceflightTip.SetDelegate("BodyText", () => new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Civilian_InitiateSpaceflightProgram, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.LaunchFacilities, false) })).AppendLine(TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSpaceDefenses) ? Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.Military_BuildSTOSquadron, false) }) : string.Empty)
				.AppendLine(Loc.T("UI.OrgTargeting.Sort"))
				.ToString());
			this.orgTargetingMissionControlPriorityTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Priority", new object[] { TIUtilities.GetPriorityString(PriorityType.MissionControl, false) })).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingSpaceMiningTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.SpaceMiningBonus")).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingTechBonusTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.TechBonus")).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.orgTargetingMissionsTip.SetText("BodyText", new StringBuilder(Loc.T("UI.OrgTargeting.Missions")).AppendLine().AppendLine(Loc.T("UI.OrgTargeting.Sort")).ToString());
			this.CloseOrgTargetingPanel();
		}

		// Token: 0x06004CDA RID: 19674 RVA: 0x002090DB File Offset: 0x002072DB
		public void OpenOrgTargetingPanel()
		{
			this.orgTargetingCanvas.enabled = true;
			this.orgTargetingCanvasRaycaster.enabled = true;
			this.orgMaximizeListButton.gameObject.SetActive(false);
		}

		// Token: 0x06004CDB RID: 19675 RVA: 0x00209106 File Offset: 0x00207306
		public void CloseOrgTargetingPanel()
		{
			this.orgTargetingCanvas.enabled = false;
			this.orgTargetingCanvasRaycaster.enabled = false;
			this.orgMaximizeListButton.gameObject.SetActive(false);
			this.orgListManager.gameObject.SetActive(false);
		}

		// Token: 0x06004CDC RID: 19676 RVA: 0x00209144 File Offset: 0x00207344
		public void SetOrgTargetPanel(IList<TIGameState> targets)
		{
			this.orgListManager.gameObject.SetActive(true);
			this.orgTargetModels.Clear();
			this.orgEntries.Clear();
			int num = 0;
			for (int i = 0; i < this.orgTargetMaxCount; i++)
			{
				OrgTargetingListItemModel orgTargetingListItemModel = new OrgTargetingListItemModel();
				TargetOrgListItem_Data targetOrgListItem_Data = new TargetOrgListItem_Data();
				if (num < targets.Count)
				{
					targetOrgListItem_Data.org = targets[num].ref_org;
					targetOrgListItem_Data.controller = this;
					targetOrgListItem_Data.missionTemplate = this.missionTemplate;
					targetOrgListItem_Data.validTarget = true;
					targetOrgListItem_Data.showInList = true;
					targetOrgListItem_Data.SetTargetOrgData(targets[num].ref_org, this.myCouncilor, this.missionTemplate, true, this);
					orgTargetingListItemModel.targetOrgListItemData = targetOrgListItem_Data;
					this.orgTargetModels.Add(orgTargetingListItemModel);
				}
				num++;
			}
			this.orgTargetingListAdapter.SetItems(this.orgTargetModels);
		}

		// Token: 0x06004CDD RID: 19677 RVA: 0x00209221 File Offset: 0x00207421
		public void OnMinimizeOrgTargetingPanel()
		{
			this.orgTargetingCanvas.enabled = false;
			this.orgTargetingCanvasRaycaster.enabled = false;
			this.orgMaximizeListButton.gameObject.SetActive(true);
		}

		// Token: 0x06004CDE RID: 19678 RVA: 0x0020924C File Offset: 0x0020744C
		public void OnMaximizeOrgTargetingPanel()
		{
			this.orgTargetingCanvas.enabled = true;
			this.orgTargetingCanvasRaycaster.enabled = true;
			this.orgMaximizeListButton.gameObject.SetActive(false);
		}

		// Token: 0x06004CDF RID: 19679 RVA: 0x00209277 File Offset: 0x00207477
		public void OnConfirmOrgTarget()
		{
			this.orgTargetingCanvas.enabled = false;
			this.orgMaximizeListButton.gameObject.SetActive(false);
		}

		// Token: 0x06004CE0 RID: 19680 RVA: 0x00209298 File Offset: 0x00207498
		public void SortOrgTargetTable(int sortTarget)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			switch (sortTarget)
			{
			case 0:
				this.orgTargetModels = this.orgTargetModels.OrderByDescending<OrgTargetingListItemModel, float>((OrgTargetingListItemModel x) => x.targetOrgListItemData.toHitValue).ToList<OrgTargetingListItemModel>();
				break;
			case 1:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby this.myCouncilor.CanAddExternalOrgValidatedForFaction(x.targetOrgListItemData.org) descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 2:
				this.orgTargetModels = this.orgTargetModels.OrderByDescending<OrgTargetingListItemModel, string>((OrgTargetingListItemModel x) => x.targetOrgListItemData.org.factionOrbit.displayName).ThenByDescending<OrgTargetingListItemModel, string>(delegate(OrgTargetingListItemModel x)
				{
					TICouncilorState assignedCouncilor = x.targetOrgListItemData.org.assignedCouncilor;
					if (assignedCouncilor == null)
					{
						return null;
					}
					return assignedCouncilor.familyName;
				}).ThenByDescending<OrgTargetingListItemModel, int>((OrgTargetingListItemModel x) => x.targetOrgListItemData.org.tier)
					.ThenByDescending<OrgTargetingListItemModel, float>((OrgTargetingListItemModel x) => x.targetOrgListItemData.toHitValue)
					.ToList<OrgTargetingListItemModel>();
				break;
			case 4:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName
					select x).ThenByDescending<OrgTargetingListItemModel, string>(delegate(OrgTargetingListItemModel x)
				{
					TICouncilorState assignedCouncilor2 = x.targetOrgListItemData.org.assignedCouncilor;
					if (assignedCouncilor2 == null)
					{
						return null;
					}
					return assignedCouncilor2.familyName;
				}).ThenByDescending<OrgTargetingListItemModel, float>((OrgTargetingListItemModel x) => x.targetOrgListItemData.toHitValue).ToList<OrgTargetingListItemModel>();
				break;
			case 5:
				this.orgTargetModels = this.orgTargetModels.OrderBy<OrgTargetingListItemModel, string>((OrgTargetingListItemModel x) => x.targetOrgListItemData.org.displayName).ToList<OrgTargetingListItemModel>();
				break;
			case 6:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.persuasion descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 7:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.investigation descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 8:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.espionage descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 9:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.command descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 10:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.administration descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 11:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.science descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 12:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.security descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 13:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.adjustedIncomeMoney_month descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 14:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.adjustedIncomeInfluence_month descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 15:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.adjustedIncomeOps_month descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 16:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.adjustedIncomeResearch_month descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 17:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.adjustedIncomeBoost_month descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 18:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.incomeMissionControl descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 19:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.projectCapacityGranted descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 20:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.economyBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 21:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.welfareBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 22:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.environmentBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 23:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.knowledgeBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 24:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.governmentBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 25:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.unityBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 26:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.militaryBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 27:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.oppressionBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 28:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.spaceDevBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 29:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.spoilsBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 30:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.spaceflightBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 31:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.MCBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 32:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.miningBonus descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			case 33:
				this.orgTargetModels = this.orgTargetModels.OrderByDescending<OrgTargetingListItemModel, int>((OrgTargetingListItemModel x) => x.targetOrgListItemData.org.techBonuses.Length).ThenBy<OrgTargetingListItemModel, TechCategory>(delegate(OrgTargetingListItemModel x)
				{
					if (x.targetOrgListItemData.org.techBonuses.Length == 0)
					{
						return TechCategory.Xenology;
					}
					return x.targetOrgListItemData.org.techBonuses[0].category;
				}).ThenByDescending<OrgTargetingListItemModel, float>(delegate(OrgTargetingListItemModel x)
				{
					if (x.targetOrgListItemData.org.techBonuses.Length == 0)
					{
						return -1f;
					}
					return x.targetOrgListItemData.org.techBonuses[0].bonus;
				})
					.ThenByDescending<OrgTargetingListItemModel, int>((OrgTargetingListItemModel x) => x.targetOrgListItemData.org.tier)
					.ThenBy<OrgTargetingListItemModel, string>((OrgTargetingListItemModel x) => x.targetOrgListItemData.org.factionOrbit.displayName)
					.ThenByDescending<OrgTargetingListItemModel, float>((OrgTargetingListItemModel x) => x.targetOrgListItemData.toHitValue)
					.ToList<OrgTargetingListItemModel>();
				break;
			case 34:
				this.orgTargetModels = (from x in this.orgTargetModels
					orderby x.targetOrgListItemData.org.missionsGranted.Count descending, x.targetOrgListItemData.org.tier descending, x.targetOrgListItemData.org.factionOrbit.displayName, x.targetOrgListItemData.toHitValue descending
					select x).ToList<OrgTargetingListItemModel>();
				break;
			}
			this.orgTargetingListAdapter.SetItems(this.orgTargetModels);
		}

		// Token: 0x06004CE1 RID: 19681 RVA: 0x0020A8C0 File Offset: 0x00208AC0
		public TICouncilorState GetNextMissionlessCouncilor(TICouncilorState councilor)
		{
			TIFactionState faction = councilor.faction;
			TICouncilorState ticouncilorState = faction.GetNextCouncilor(councilor, true);
			while (ticouncilorState != councilor)
			{
				if (!ticouncilorState.HasMission && !ticouncilorState.detained)
				{
					return ticouncilorState;
				}
				ticouncilorState = faction.GetNextCouncilor(ticouncilorState, true);
			}
			return null;
		}

		// Token: 0x06004CE2 RID: 19682 RVA: 0x0020A904 File Offset: 0x00208B04
		public void OnConfirmMissionClick()
		{
			this.ShutdownTargetSelection(false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.myCouncilor.PlayMissionVoice(this.missionTemplate, TICouncilorVoiceTemplate.VoiceMissionSituation.Assigned, this.myCouncilor.OnEarth);
			TIFactionState faction = this.myCouncilor.faction;
			faction.playerControl.StartAction(new AssignCouncilorToMission(this.myCouncilor, this.missionTemplate, this.currentTarget, this.resourcesSpend, this.forceAllowMissions));
			faction.playerControl.StartAction(new SetCouncilorPermanentAssignment(this.myCouncilor, false));
			faction.playerControl.StartAction(new SetCouncilorRepeatMission(this.myCouncilor, false));
			this.SetResourceAmount(0f);
			this.activeButton = null;
			this.nextCouncilor = this.GetNextMissionlessCouncilor(this.myCouncilor);
			if (this.nextCouncilor != null && TIPlayerProfileManager.cycleNextCouncilorWhenAssigningMissions)
			{
				this.needToForceCouncilorSelection = true;
				base.Invoke("ForcePlayerNewCouncilor", 1f);
				return;
			}
			this.assignmentsArrowLeft.SetActive(true);
			this.assignmentsArrowRight.SetActive(true);
		}

		// Token: 0x06004CE3 RID: 19683 RVA: 0x0020AA10 File Offset: 0x00208C10
		public void OnMissionConfirmHover()
		{
			if (this.confirmButton.interactable)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverButton", false, false);
				this.missionName.SetText(Loc.T("UI.Objectives.CouncilorMissionControlsCanvas.Confirm.Name"));
			}
		}

		// Token: 0x06004CE4 RID: 19684 RVA: 0x0020AA40 File Offset: 0x00208C40
		public void OnMissionConfirmStopHover()
		{
			if (this.confirmButton.interactable)
			{
				this.missionName.SetText(this.missionTemplate.displayName);
			}
		}

		// Token: 0x06004CE5 RID: 19685 RVA: 0x0020AA65 File Offset: 0x00208C65
		protected void ShowMissionOutome(bool active = true)
		{
			if (this.successOrFailurePanel != null)
			{
				this.successOrFailurePanel.SetActive(active);
				if (!active)
				{
					this.modifierPanesOpen = false;
					return;
				}
				this.modifierPanesOpen = true;
			}
		}

		// Token: 0x06004CE6 RID: 19686 RVA: 0x0020AA93 File Offset: 0x00208C93
		public void ModifierToggleClicked()
		{
			AudioManager.PlayOneShot((!this.modifierPanesOpen) ? "event:/SFX/UI_SFX/trig_SFX_OpenFinder" : "event:/SFX/UI_SFX/trig_SFX_CloseFinder", false, false);
			this.modifierPanesOpen = !this.modifierPanesOpen;
		}

		// Token: 0x06004CE7 RID: 19687 RVA: 0x0020AAC0 File Offset: 0x00208CC0
		public void UpdateMissionModifiers()
		{
			if (this.currentTarget == null)
			{
				this.successOrFailureValue.text = "--%";
				return;
			}
			if (!this.missionTemplate.ContestedMission)
			{
				this.successOrFailureValue.SetText("100%");
				return;
			}
			TIMissionResolution_Contested timissionResolution_Contested = this.missionTemplate.resolutionMethod as TIMissionResolution_Contested;
			if (timissionResolution_Contested != null)
			{
				List<TIMissionModifier> attackingNonZeroModifiers = timissionResolution_Contested.GetAttackingNonZeroModifiers(this.missionTemplate, this.myCouncilor, this.currentTarget, this.resourcesSpend);
				this.UpdateModifierList(attackingNonZeroModifiers, this.bonusList, false, 0f);
				this.councilorHeaderText.SetText(Loc.T("UI.MissionPhase.CouncilorScore", new object[] { timissionResolution_Contested.SumAttackingModifiers(this.missionTemplate, this.myCouncilor, this.currentTarget, this.resourcesSpend).ToString("N1") }));
				List<TIMissionModifier> defendingNonZeroModifiers = timissionResolution_Contested.GetDefendingNonZeroModifiers(this.missionTemplate, this.myCouncilor, this.currentTarget, this.resourcesSpend);
				float num = timissionResolution_Contested.SumDefendingModifiers(this.missionTemplate, this.myCouncilor, this.currentTarget, this.resourcesSpend);
				this.UpdateModifierList(defendingNonZeroModifiers, this.penaltyList, this.currentTarget.isCouncilorState && base.activePlayer.GetViewofCouncilor(this.currentTarget.ref_councilor).factionCurrent == null, num);
				if (num < 0f)
				{
					this.targetHeaderText.SetText(Loc.T("UI.MissionPhase.NegativeTargetScore", new object[] { (-1f * num).ToString("N1") }));
				}
				else
				{
					this.targetHeaderText.SetText(Loc.T("UI.MissionPhase.TargetScore", new object[] { num.ToString("N1") }));
				}
				this.successOrFailureValue.text = this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, this.myCouncilor, this.currentTarget, this.resourcesSpend, false, 2);
			}
		}

		// Token: 0x06004CE8 RID: 19688 RVA: 0x0020ACBC File Offset: 0x00208EBC
		private void UpdateModifierList(List<TIMissionModifier> modifierList, ListManagerBase uiList, bool hidden, float total = 0f)
		{
			if (hidden)
			{
				uiList.SetListSize<ModifierListItemController>(1, false, false);
				using (IEnumerator<object> enumerator = uiList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CouncilorMissionCanvasController.<>o__331.<>p__0 == null)
						{
							CouncilorMissionCanvasController.<>o__331.<>p__0 = CallSite<Func<CallSite, object, ModifierListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ModifierListItemController), typeof(CouncilorMissionCanvasController)));
						}
						CouncilorMissionCanvasController.<>o__331.<>p__0.Target(CouncilorMissionCanvasController.<>o__331.<>p__0, enumerator.Current).SetModifiers(Loc.T("UI.MissionPhase.Unknown"), total);
					}
					return;
				}
			}
			uiList.SetListSize<ModifierListItemController>(modifierList.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = uiList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CouncilorMissionCanvasController.<>o__331.<>p__1 == null)
					{
						CouncilorMissionCanvasController.<>o__331.<>p__1 = CallSite<Func<CallSite, object, ModifierListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ModifierListItemController), typeof(CouncilorMissionCanvasController)));
					}
					ModifierListItemController modifierListItemController = CouncilorMissionCanvasController.<>o__331.<>p__1.Target(CouncilorMissionCanvasController.<>o__331.<>p__1, enumerator.Current);
					if (modifierListItemController != null)
					{
						modifierListItemController.Init(this);
						float modifier = modifierList[num].GetModifier(this.myCouncilor, this.currentTarget, this.resourcesSpend, this.activeButton.missionType.cost.resourceType);
						string displayName = modifierList[num++].displayName;
						modifierListItemController.SetModifiers(displayName, modifier);
					}
				}
			}
		}

		// Token: 0x06004CE9 RID: 19689 RVA: 0x0020AE44 File Offset: 0x00209044
		public void SetAutoFailText()
		{
			this.turnedEnemyCouncilorFailureText.SetText(Loc.T("UI.Councilor.TurnedSlider", new object[] { this.enemyCouncilor.autofailMissionsValue.ToPercent("P0") }));
		}

		// Token: 0x06004CEA RID: 19690 RVA: 0x0020AE7C File Offset: 0x0020907C
		public void OnTurnedSliderChangedValue()
		{
			if (!TIGameState.Valid(this.enemyCouncilor))
			{
				return;
			}
			base.activePlayer.playerControl.StartAction(new SetAutofailValueForTurnedCouncilorAction(this.enemyCouncilor, this.turnedEnemyCouncilorSlider.value / 100f));
			this.SetAutoFailText();
		}

		// Token: 0x06004CEB RID: 19691 RVA: 0x0020AEC9 File Offset: 0x002090C9
		private void HideTutorials()
		{
			this.CouncilorMissionCanvasUITutorial.HideTutorial();
			this.SelectCouncilorTutorialController.HideTutorial();
		}

		// Token: 0x06004CEC RID: 19692 RVA: 0x0020AEE4 File Offset: 0x002090E4
		private void UpdateCouncilList()
		{
			TIFactionState[] allGameStates = GameStateManager.GetAllGameStates<TIFactionState>(true);
			int num = allGameStates.Length;
			this.AdjustListLength(num, this.councilProgressContent, this.councilProgressListItemPrefab);
			for (int i = 0; i < num; i++)
			{
				this.councilProgressContent.GetChild(i).GetComponent<ProgressListItemController>().UpdateData(allGameStates[i]);
			}
		}

		// Token: 0x06004CED RID: 19693 RVA: 0x0020AF34 File Offset: 0x00209134
		private void AdjustListLength(int desiredLength, Transform listTransform, GameObject prefab)
		{
			int childCount = listTransform.childCount;
			if (desiredLength > childCount)
			{
				for (int i = childCount; i < desiredLength; i++)
				{
					global::UnityEngine.Object.Instantiate<GameObject>(prefab).transform.SetParent(listTransform, false);
				}
				return;
			}
			if (childCount > desiredLength)
			{
				for (int j = childCount - 1; j >= desiredLength; j--)
				{
					global::UnityEngine.Object.Destroy(listTransform.GetChild(j).gameObject);
				}
			}
		}

		// Token: 0x06004CEE RID: 19694 RVA: 0x0020AF90 File Offset: 0x00209190
		public override void OnDestroy()
		{
			GameControl.eventManager.RemoveListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.CouncilorSelected), null);
			GameControl.eventManager.RemoveListener<MissionPhaseRestart>(new EventManager.EventDelegate<MissionPhaseRestart>(this.ResetOnStart), null);
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ResetOnStart), "CouncilorMissionUpdate");
			GameControl.eventManager.RemoveListener<TimeEventComplete>(new EventManager.EventDelegate<TimeEventComplete>(this.ResetOnComplete), "CouncilorMissionUpdate");
			GameControl.eventManager.RemoveListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.OnCouncilCompositionChanged), null);
			GameControl.eventManager.RemoveListener<CouncilorSelectedOffMap>(new EventManager.EventDelegate<CouncilorSelectedOffMap>(this.CouncilorSelected), null);
			GameControl.eventManager.RemoveListener<FactionFinalizesMissions>(new EventManager.EventDelegate<FactionFinalizesMissions>(this.OnFactionFinalizesMissions), null);
			GameControl.eventManager.RemoveListener<SpaceBodySelectedEvent>(new EventManager.EventDelegate<SpaceBodySelectedEvent>(this.OnNaturalSpaceObjectSelected), null);
			GameControl.eventManager.RemoveListener<LagrangePointSelectedEvent>(new EventManager.EventDelegate<LagrangePointSelectedEvent>(this.OnNaturalSpaceObjectSelected), null);
			GameControl.eventManager.RemoveListener<MissionTargettedEvent>(new EventManager.EventDelegate<MissionTargettedEvent>(this.OnNewMissionTarget), null);
			this.RemoveEnemyCouncilorListeners();
			this.RemoveMyCouncilorListeners();
			base.OnDestroy();
		}

		// Token: 0x06004CF3 RID: 19699 RVA: 0x0020B0EC File Offset: 0x002092EC
		[CompilerGenerated]
		private void <UpdateEnemyCouncilorPanel>g__SetTraitList|232_1(bool moreDeets, ref CouncilorMissionCanvasController.<>c__DisplayClass232_0 A_2)
		{
			List<TITraitTemplate> traits = A_2.enemyCouncilorView.traits;
			if (traits.Count > 0)
			{
				if (this.enemyCouncilor.isHuman)
				{
					this.traitsTabText.SetText(Loc.T("UI.Councilor.Bio"));
					this.enemyCouncilorTraitsList.SetListSize<TraitsListItemController>(traits.Count, false, false);
					int num = 0;
					using (IEnumerator<object> enumerator = this.enemyCouncilorTraitsList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (CouncilorMissionCanvasController.<>o__232.<>p__0 == null)
							{
								CouncilorMissionCanvasController.<>o__232.<>p__0 = CallSite<Func<CallSite, object, TraitsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TraitsListItemController), typeof(CouncilorMissionCanvasController)));
							}
							CouncilorMissionCanvasController.<>o__232.<>p__0.Target(CouncilorMissionCanvasController.<>o__232.<>p__0, enumerator.Current).UpdateListItem(traits[num++], 0, this.enemyCouncilorTraitsList.size == 1);
						}
						goto IL_018D;
					}
				}
				this.traitsTabText.SetText(Loc.T("UI.Councilor.Traits"));
				this.enemyCouncilorTraitsList.SetListSize<TraitsListItemController>(traits.Count, false, false);
				int num2 = 0;
				using (IEnumerator<object> enumerator = this.enemyCouncilorTraitsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (CouncilorMissionCanvasController.<>o__232.<>p__1 == null)
						{
							CouncilorMissionCanvasController.<>o__232.<>p__1 = CallSite<Func<CallSite, object, TraitsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TraitsListItemController), typeof(CouncilorMissionCanvasController)));
						}
						CouncilorMissionCanvasController.<>o__232.<>p__1.Target(CouncilorMissionCanvasController.<>o__232.<>p__1, enumerator.Current).UpdateListItem(traits[num2++], 0, this.enemyCouncilorTraitsList.size == 1);
					}
				}
				IL_018D:
				if (moreDeets)
				{
					this.traitsTabController.SetSize(39f, 75f, 24f, traits.Count);
				}
				else
				{
					this.enemyCouncilorHometownObject.SetActive(false);
					this.enemyCouncilorAgeObject.SetActive(false);
					this.traitsTabController.SetSize(25f, 25f, 24f, traits.Count);
				}
				this.enemyCouncilorTraitsButton.SetActive(true);
				A_2.activateDataPanel = true;
			}
			else
			{
				this.enemyCouncilorTraitsButton.SetActive(false);
			}
			if (traits.Count == 0 && this.enemyCouncilorTabManager.activeTab == this.traitsTabController)
			{
				this.enemyCouncilorTabManager.ClearActiveTab();
			}
		}

		// Token: 0x04002E6B RID: 11883
		[Header("My Councilor Info")]
		public Canvas myCouncilorPanel;

		// Token: 0x04002E6C RID: 11884
		public GraphicRaycaster myCouncilorPanelGraphicRaycaster;

		// Token: 0x04002E6D RID: 11885
		public TMP_Text myCouncilorHeader;

		// Token: 0x04002E6E RID: 11886
		public TMP_Text councilorName;

		// Token: 0x04002E6F RID: 11887
		public TMP_Text councilorCurrentMission;

		// Token: 0x04002E70 RID: 11888
		public TMP_Text councilorType;

		// Token: 0x04002E71 RID: 11889
		public Image councilorBackgroundImage;

		// Token: 0x04002E72 RID: 11890
		public VideoPlayer myCouncilorVideo;

		// Token: 0x04002E73 RID: 11891
		public Image myCouncilorStillImage;

		// Token: 0x04002E74 RID: 11892
		public Image factionIcon;

		// Token: 0x04002E75 RID: 11893
		public TMP_Text per;

		// Token: 0x04002E76 RID: 11894
		public TMP_Text inv;

		// Token: 0x04002E77 RID: 11895
		public TMP_Text esp;

		// Token: 0x04002E78 RID: 11896
		public TMP_Text cmd;

		// Token: 0x04002E79 RID: 11897
		public TMP_Text adm;

		// Token: 0x04002E7A RID: 11898
		public TMP_Text sci;

		// Token: 0x04002E7B RID: 11899
		public TMP_Text sec;

		// Token: 0x04002E7C RID: 11900
		public TMP_Text loy;

		// Token: 0x04002E7D RID: 11901
		public TMP_Text perTitle;

		// Token: 0x04002E7E RID: 11902
		public TMP_Text invTitle;

		// Token: 0x04002E7F RID: 11903
		public TMP_Text espTitle;

		// Token: 0x04002E80 RID: 11904
		public TMP_Text cmdTitle;

		// Token: 0x04002E81 RID: 11905
		public TMP_Text admTitle;

		// Token: 0x04002E82 RID: 11906
		public TMP_Text sciTitle;

		// Token: 0x04002E83 RID: 11907
		public TMP_Text secTitle;

		// Token: 0x04002E84 RID: 11908
		public TMP_Text loyTitle;

		// Token: 0x04002E85 RID: 11909
		public TooltipTrigger myLoyaltyTip;

		// Token: 0x04002E86 RID: 11910
		public Image statusIcon;

		// Token: 0x04002E87 RID: 11911
		public TMP_Text statusText;

		// Token: 0x04002E88 RID: 11912
		public TooltipTrigger statusIconTooltip;

		// Token: 0x04002E89 RID: 11913
		public TooltipTrigger statusTextTooltip;

		// Token: 0x04002E8A RID: 11914
		private Vector3 myCouncilorBackgroundImageInitialPosition;

		// Token: 0x04002E8B RID: 11915
		private bool myCouncilorDataDirty;

		// Token: 0x04002E8C RID: 11916
		public GameObject trackingMePanel;

		// Token: 0x04002E8D RID: 11917
		public ListManagerBase trackingMeList;

		// Token: 0x04002E8E RID: 11918
		public TooltipTrigger trackingMeTip;

		// Token: 0x04002E8F RID: 11919
		public TMP_Text councilorXPText;

		// Token: 0x04002E90 RID: 11920
		public TMP_Text automateButtonText;

		// Token: 0x04002E91 RID: 11921
		public TooltipTrigger automateTooltip;

		// Token: 0x04002E92 RID: 11922
		[Header("Mission List")]
		public GameObject actionsPanel;

		// Token: 0x04002E93 RID: 11923
		public ListManagerBase actionList;

		// Token: 0x04002E94 RID: 11924
		[Header("Mission Info")]
		public GameObject missionInfoPanel;

		// Token: 0x04002E95 RID: 11925
		public TMP_Text missionDisplayName;

		// Token: 0x04002E96 RID: 11926
		public TMP_Text missionDescription;

		// Token: 0x04002E97 RID: 11927
		public GameObject namePanel;

		// Token: 0x04002E98 RID: 11928
		public PipListItemController[] clockPips;

		// Token: 0x04002E99 RID: 11929
		[Header("Mission Options")]
		public GameObject actionOptionsPanel;

		// Token: 0x04002E9A RID: 11930
		private Color originalTargetingColor;

		// Token: 0x04002E9B RID: 11931
		public Color cancelTargettingColor;

		// Token: 0x04002E9C RID: 11932
		public TMP_Text missionName;

		// Token: 0x04002E9D RID: 11933
		public Button confirmButton;

		// Token: 0x04002E9E RID: 11934
		public GameObject missionArrowLeft;

		// Token: 0x04002E9F RID: 11935
		public GameObject missionArrowRight;

		// Token: 0x04002EA0 RID: 11936
		private Transform costPanel;

		// Token: 0x04002EA1 RID: 11937
		public TMP_Text resourcesType;

		// Token: 0x04002EA2 RID: 11938
		public Slider resourcesSlider;

		// Token: 0x04002EA3 RID: 11939
		public TMP_Text resourceValue;

		// Token: 0x04002EA4 RID: 11940
		public TMP_Text fixedResourcesType;

		// Token: 0x04002EA5 RID: 11941
		public GameObject resourcesSliderObject;

		// Token: 0x04002EA6 RID: 11942
		public TMP_Dropdown targetDropdown;

		// Token: 0x04002EA7 RID: 11943
		[Header("Mission Chance")]
		public GameObject successOrFailurePanel;

		// Token: 0x04002EA8 RID: 11944
		public TMP_Text successOrFailureText;

		// Token: 0x04002EA9 RID: 11945
		public TMP_Text successOrFailureValue;

		// Token: 0x04002EAA RID: 11946
		public ListManagerBase bonusList;

		// Token: 0x04002EAB RID: 11947
		public ListManagerBase penaltyList;

		// Token: 0x04002EAC RID: 11948
		public TMP_Text councilorHeaderText;

		// Token: 0x04002EAD RID: 11949
		public TMP_Text targetHeaderText;

		// Token: 0x04002EAE RID: 11950
		private CouncilorMissionButtonController activeButton;

		// Token: 0x04002EAF RID: 11951
		private TICouncilorState myCouncilor;

		// Token: 0x04002EB1 RID: 11953
		private TICouncilorState nextCouncilor;

		// Token: 0x04002EB2 RID: 11954
		private TIMissionTargeting currentTargeting;

		// Token: 0x04002EB3 RID: 11955
		private TIGameState currentTarget;

		// Token: 0x04002EB4 RID: 11956
		private float resourcesSpend;

		// Token: 0x04002EB5 RID: 11957
		[Header("Enemy Councilor Info")]
		public Canvas enemyCouncilorInfoPanel;

		// Token: 0x04002EB6 RID: 11958
		public GraphicRaycaster enemyCouncilorGraphicRaycaster;

		// Token: 0x04002EB7 RID: 11959
		public TMP_Text enemyCouncilorHeader;

		// Token: 0x04002EB8 RID: 11960
		public TMP_Text enemyCouncilorName;

		// Token: 0x04002EB9 RID: 11961
		public TMP_Text enemyCouncilorCurrentMission;

		// Token: 0x04002EBA RID: 11962
		public TMP_Text enemyCouncilorType;

		// Token: 0x04002EBB RID: 11963
		public Image enemyCouncilorBackgroundImage;

		// Token: 0x04002EBC RID: 11964
		public VideoPlayer enemyCouncilorVideo;

		// Token: 0x04002EBD RID: 11965
		public Image enemyCouncilorStillImage;

		// Token: 0x04002EBE RID: 11966
		public Image enemyFactionIcon;

		// Token: 0x04002EBF RID: 11967
		public TMP_Text enemyPer;

		// Token: 0x04002EC0 RID: 11968
		public TMP_Text enemyInv;

		// Token: 0x04002EC1 RID: 11969
		public TMP_Text enemyEsp;

		// Token: 0x04002EC2 RID: 11970
		public TMP_Text enemyCmd;

		// Token: 0x04002EC3 RID: 11971
		public TMP_Text enemyAdm;

		// Token: 0x04002EC4 RID: 11972
		public TMP_Text enemySci;

		// Token: 0x04002EC5 RID: 11973
		public TMP_Text enemySec;

		// Token: 0x04002EC6 RID: 11974
		public TMP_Text enemyLoy;

		// Token: 0x04002EC7 RID: 11975
		public TMP_Text enemyPerTitle;

		// Token: 0x04002EC8 RID: 11976
		public TMP_Text enemyInvTitle;

		// Token: 0x04002EC9 RID: 11977
		public TMP_Text enemyEspTitle;

		// Token: 0x04002ECA RID: 11978
		public TMP_Text enemyCmdTitle;

		// Token: 0x04002ECB RID: 11979
		public TMP_Text enemyAdmTitle;

		// Token: 0x04002ECC RID: 11980
		public TMP_Text enemySciTitle;

		// Token: 0x04002ECD RID: 11981
		public TMP_Text enemySecTitle;

		// Token: 0x04002ECE RID: 11982
		public TMP_Text enemyLoyTitle;

		// Token: 0x04002ECF RID: 11983
		public GameObject enemyCouncilorHometownObject;

		// Token: 0x04002ED0 RID: 11984
		public TMP_Text enemyCouncilorHometown;

		// Token: 0x04002ED1 RID: 11985
		public GameObject enemyCouncilorAgeObject;

		// Token: 0x04002ED2 RID: 11986
		public TMP_Text enemyCouncilorAge;

		// Token: 0x04002ED3 RID: 11987
		public GameObject enemyCouncilorTraitsListHeader;

		// Token: 0x04002ED4 RID: 11988
		public TMP_Text enemyCouncilorTraitsHeader;

		// Token: 0x04002ED5 RID: 11989
		public TooltipTrigger enemyLoyaltyTip;

		// Token: 0x04002ED6 RID: 11990
		public Image enemyCouncilorStatusIcon;

		// Token: 0x04002ED7 RID: 11991
		public TMP_Text enemyCouncilorStatusText;

		// Token: 0x04002ED8 RID: 11992
		public TooltipTrigger enemyCouncilorIconStatusTooltip;

		// Token: 0x04002ED9 RID: 11993
		public TooltipTrigger enemyCouncilorTextStatusTooltip;

		// Token: 0x04002EDA RID: 11994
		public GameObject turnedEnemyCouncilorFailurePanel;

		// Token: 0x04002EDB RID: 11995
		public TMP_Text turnedEnemyCouncilorFailureText;

		// Token: 0x04002EDC RID: 11996
		public Slider turnedEnemyCouncilorSlider;

		// Token: 0x04002EDD RID: 11997
		private Vector3 enemyCouncilorBackgroundImageInitialPosition;

		// Token: 0x04002EDE RID: 11998
		private bool enemyCouncilorDataDirty;

		// Token: 0x04002EDF RID: 11999
		private Animator _modifierAnimator;

		// Token: 0x04002EE0 RID: 12000
		public GameObject enemyCouncilorDataPanel;

		// Token: 0x04002EE1 RID: 12001
		public GameObject enemyCouncilorMissionsButton;

		// Token: 0x04002EE2 RID: 12002
		public GameObject enemyCouncilorOrgsButton;

		// Token: 0x04002EE3 RID: 12003
		public GameObject enemyCouncilorTraitsButton;

		// Token: 0x04002EE4 RID: 12004
		public ListManagerBase enemyCouncilorTraitsList;

		// Token: 0x04002EE5 RID: 12005
		public ListManagerBase enemyCouncilorOrgsList;

		// Token: 0x04002EE6 RID: 12006
		public ListManagerBase enemyCouncilorMissionsList;

		// Token: 0x04002EE7 RID: 12007
		public TabbedPaneManager enemyCouncilorTabManager;

		// Token: 0x04002EE8 RID: 12008
		public TabbedPaneController traitsTabController;

		// Token: 0x04002EE9 RID: 12009
		public TabbedPaneController orgsTabController;

		// Token: 0x04002EEA RID: 12010
		public TabbedPaneController missionsTabController;

		// Token: 0x04002EEB RID: 12011
		public TMP_Text traitsTabText;

		// Token: 0x04002EEC RID: 12012
		public TMP_Text orgsTabText;

		// Token: 0x04002EED RID: 12013
		public TMP_Text missionsTabText;

		// Token: 0x04002EEE RID: 12014
		[Header("Assignments")]
		public Canvas missionPhaseControlsCanvas;

		// Token: 0x04002EEF RID: 12015
		public Button confirmAssignmentsButton;

		// Token: 0x04002EF0 RID: 12016
		public GameObject councilProgressListItemPrefab;

		// Token: 0x04002EF1 RID: 12017
		public GameObject councilStatusPanel;

		// Token: 0x04002EF2 RID: 12018
		public Transform councilProgressContent;

		// Token: 0x04002EF3 RID: 12019
		public TMP_Text confirmAssignmentsText;

		// Token: 0x04002EF4 RID: 12020
		public GameObject unassignedWarningPanel;

		// Token: 0x04002EF5 RID: 12021
		public TMP_Text unassignedWarningHeader;

		// Token: 0x04002EF6 RID: 12022
		public TMP_Text unassignedWarningPrompt;

		// Token: 0x04002EF7 RID: 12023
		public TMP_Text unassignedWarningConfirmButton;

		// Token: 0x04002EF8 RID: 12024
		public TMP_Text unassignedWarningDeclineButton;

		// Token: 0x04002EF9 RID: 12025
		private bool showCouncilList;

		// Token: 0x04002EFA RID: 12026
		private int AIPlayerCount;

		// Token: 0x04002EFB RID: 12027
		public GameObject assignmentsArrowLeft;

		// Token: 0x04002EFC RID: 12028
		public GameObject assignmentsArrowRight;

		// Token: 0x04002EFD RID: 12029
		[Header("Abort Mission UI")]
		public GameObject abortButtonGameObject;

		// Token: 0x04002EFE RID: 12030
		public TMP_Text abortButtonText;

		// Token: 0x04002EFF RID: 12031
		public GameObject abortConfirmUI;

		// Token: 0x04002F00 RID: 12032
		public TMP_Text abortWarningHeader;

		// Token: 0x04002F01 RID: 12033
		public TMP_Text abortWarningBody;

		// Token: 0x04002F02 RID: 12034
		public TMP_Text abortConfirmButtonText;

		// Token: 0x04002F03 RID: 12035
		public TMP_Text abortCancelButtonText;

		// Token: 0x04002F04 RID: 12036
		[Header("Tutorials")]
		public UITutorialController CouncilorMissionCanvasUITutorial;

		// Token: 0x04002F05 RID: 12037
		public UITutorialController SelectCouncilorTutorialController;

		// Token: 0x04002F06 RID: 12038
		public float confirmFlashOnTime;

		// Token: 0x04002F07 RID: 12039
		public float confirmFlashOffTime;

		// Token: 0x04002F08 RID: 12040
		private float _confirmAssignmentsNextFlashTime;

		// Token: 0x04002F09 RID: 12041
		private bool _isConfirmAssignmentsHovered;

		// Token: 0x04002F0A RID: 12042
		private bool _isConfirmAssignmentsFlashOn;

		// Token: 0x04002F0B RID: 12043
		private bool needToForceCouncilorSelection;

		// Token: 0x04002F0C RID: 12044
		[Header("Debug")]
		public bool forceAllowMissions;

		// Token: 0x04002F0D RID: 12045
		private bool clearingMyCouncilorVideo;

		// Token: 0x04002F0E RID: 12046
		private const int maxMissionsWidth = 22;

		// Token: 0x04002F0F RID: 12047
		private Dictionary<int, TIGameState> targetOptionData;

		// Token: 0x04002F10 RID: 12048
		private Dictionary<TIGameState, int> reverseTargetOptionData;

		// Token: 0x04002F11 RID: 12049
		[Header("Org Targeting Panel")]
		public Canvas orgTargetingCanvas;

		// Token: 0x04002F12 RID: 12050
		public GraphicRaycaster orgTargetingCanvasRaycaster;

		// Token: 0x04002F13 RID: 12051
		public Button orgMaximizeListButton;

		// Token: 0x04002F14 RID: 12052
		public ListManagerBase orgListManager;

		// Token: 0x04002F15 RID: 12053
		public List<OrgTargetingListItemModel> orgTargetModels = new List<OrgTargetingListItemModel>();

		// Token: 0x04002F16 RID: 12054
		public OrgTargetingListAdapter orgTargetingListAdapter;

		// Token: 0x04002F17 RID: 12055
		public TMP_Text orgListHeader;

		// Token: 0x04002F18 RID: 12056
		public TMP_Text orgToHitColumnHeader;

		// Token: 0x04002F19 RID: 12057
		public TMP_Text orgNameColumnHeader;

		// Token: 0x04002F1A RID: 12058
		public TMP_Text orgTechColumnHeader;

		// Token: 0x04002F1B RID: 12059
		public TooltipTrigger orgTargetingToHitTip;

		// Token: 0x04002F1C RID: 12060
		public TooltipTrigger orgTargetingCouncilorCantGainTip;

		// Token: 0x04002F1D RID: 12061
		public TooltipTrigger orgTargetingCouncilorTip;

		// Token: 0x04002F1E RID: 12062
		public TooltipTrigger orgTargetingTierTip;

		// Token: 0x04002F1F RID: 12063
		public TooltipTrigger orgTargetingOrgNameTip;

		// Token: 0x04002F20 RID: 12064
		public TooltipTrigger orgTargetingPersuasionTip;

		// Token: 0x04002F21 RID: 12065
		public TooltipTrigger orgTargetingInvestigationTip;

		// Token: 0x04002F22 RID: 12066
		public TooltipTrigger orgTargetingEspionageTip;

		// Token: 0x04002F23 RID: 12067
		public TooltipTrigger orgTargetingCommandTip;

		// Token: 0x04002F24 RID: 12068
		public TooltipTrigger orgTargetingAdministrationTip;

		// Token: 0x04002F25 RID: 12069
		public TooltipTrigger orgTargetingScienceTip;

		// Token: 0x04002F26 RID: 12070
		public TooltipTrigger orgTargetingSecurityTip;

		// Token: 0x04002F27 RID: 12071
		public TooltipTrigger orgTargetingMoneyTip;

		// Token: 0x04002F28 RID: 12072
		public TooltipTrigger orgTargetingInfluenceTip;

		// Token: 0x04002F29 RID: 12073
		public TooltipTrigger orgTargetingOperationsTip;

		// Token: 0x04002F2A RID: 12074
		public TooltipTrigger orgTargetingResearchTip;

		// Token: 0x04002F2B RID: 12075
		public TooltipTrigger orgTargetingBoostTip;

		// Token: 0x04002F2C RID: 12076
		public TooltipTrigger orgTargetingMissionControlGrantTip;

		// Token: 0x04002F2D RID: 12077
		public TooltipTrigger orgTargetingProjectsTip;

		// Token: 0x04002F2E RID: 12078
		public TooltipTrigger orgTargetingEconomyTip;

		// Token: 0x04002F2F RID: 12079
		public TooltipTrigger orgTargetingWelfareTip;

		// Token: 0x04002F30 RID: 12080
		public TooltipTrigger orgTargetingEnvironmentTip;

		// Token: 0x04002F31 RID: 12081
		public TooltipTrigger orgTargetingKnowledgeTip;

		// Token: 0x04002F32 RID: 12082
		public TooltipTrigger orgTargetingGovernmentTip;

		// Token: 0x04002F33 RID: 12083
		public TooltipTrigger orgTargetingUnityTip;

		// Token: 0x04002F34 RID: 12084
		public TooltipTrigger orgTargetingMilitaryTip;

		// Token: 0x04002F35 RID: 12085
		public TooltipTrigger orgTargetingOppressionTip;

		// Token: 0x04002F36 RID: 12086
		public TooltipTrigger orgTargetingFundingTip;

		// Token: 0x04002F37 RID: 12087
		public TooltipTrigger orgTargetingSpoilsTip;

		// Token: 0x04002F38 RID: 12088
		public TooltipTrigger orgTargetingSpaceflightTip;

		// Token: 0x04002F39 RID: 12089
		public TooltipTrigger orgTargetingMissionControlPriorityTip;

		// Token: 0x04002F3A RID: 12090
		public TooltipTrigger orgTargetingSpaceMiningTip;

		// Token: 0x04002F3B RID: 12091
		public TooltipTrigger orgTargetingTechBonusTip;

		// Token: 0x04002F3C RID: 12092
		public TooltipTrigger orgTargetingMissionsTip;

		// Token: 0x04002F3D RID: 12093
		private Dictionary<TIOrgState, TargetOrgListItemController> orgEntries = new Dictionary<TIOrgState, TargetOrgListItemController>();

		// Token: 0x04002F3E RID: 12094
		private bool orgSortAscending;

		// Token: 0x04002F3F RID: 12095
		private int orgTargetMaxCount;

		// Token: 0x02001053 RID: 4179
		private enum OrgSortColumn
		{
			// Token: 0x0400624F RID: 25167
			ToHit,
			// Token: 0x04006250 RID: 25168
			CouncilorCantGain,
			// Token: 0x04006251 RID: 25169
			Councilor,
			// Token: 0x04006252 RID: 25170
			Faction,
			// Token: 0x04006253 RID: 25171
			Tier,
			// Token: 0x04006254 RID: 25172
			OrgName,
			// Token: 0x04006255 RID: 25173
			Persuasion,
			// Token: 0x04006256 RID: 25174
			Investigation,
			// Token: 0x04006257 RID: 25175
			Espionage,
			// Token: 0x04006258 RID: 25176
			Command,
			// Token: 0x04006259 RID: 25177
			Administration,
			// Token: 0x0400625A RID: 25178
			Science,
			// Token: 0x0400625B RID: 25179
			Security,
			// Token: 0x0400625C RID: 25180
			Money,
			// Token: 0x0400625D RID: 25181
			Influence,
			// Token: 0x0400625E RID: 25182
			Operations,
			// Token: 0x0400625F RID: 25183
			Research,
			// Token: 0x04006260 RID: 25184
			Boost,
			// Token: 0x04006261 RID: 25185
			MissionControlGrant,
			// Token: 0x04006262 RID: 25186
			Projects,
			// Token: 0x04006263 RID: 25187
			Economy,
			// Token: 0x04006264 RID: 25188
			Welfare,
			// Token: 0x04006265 RID: 25189
			Environment,
			// Token: 0x04006266 RID: 25190
			Knowledge,
			// Token: 0x04006267 RID: 25191
			Government,
			// Token: 0x04006268 RID: 25192
			Unity,
			// Token: 0x04006269 RID: 25193
			Military,
			// Token: 0x0400626A RID: 25194
			Oppression,
			// Token: 0x0400626B RID: 25195
			Funding,
			// Token: 0x0400626C RID: 25196
			Spoils,
			// Token: 0x0400626D RID: 25197
			Spaceflight,
			// Token: 0x0400626E RID: 25198
			MissionControlPriority,
			// Token: 0x0400626F RID: 25199
			SpaceMining,
			// Token: 0x04006270 RID: 25200
			TechBonus,
			// Token: 0x04006271 RID: 25201
			Missions
		}
	}
}
