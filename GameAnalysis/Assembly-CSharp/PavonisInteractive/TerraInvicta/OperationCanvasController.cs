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
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008B0 RID: 2224
	public class OperationCanvasController : CanvasControllerBase
	{
		// Token: 0x17000EF8 RID: 3832
		// (get) Token: 0x0600543F RID: 21567 RVA: 0x00261BCA File Offset: 0x0025FDCA
		// (set) Token: 0x06005440 RID: 21568 RVA: 0x00261BD1 File Offset: 0x0025FDD1
		public static OperationCanvasController Singleton { get; private set; }

		// Token: 0x17000EF9 RID: 3833
		// (get) Token: 0x06005441 RID: 21569 RVA: 0x00261BD9 File Offset: 0x0025FDD9
		// (set) Token: 0x06005442 RID: 21570 RVA: 0x00261BE1 File Offset: 0x0025FDE1
		public TIGameState targetBase { get; private set; }

		// Token: 0x17000EFA RID: 3834
		// (get) Token: 0x06005443 RID: 21571 RVA: 0x00261BEA File Offset: 0x0025FDEA
		// (set) Token: 0x06005444 RID: 21572 RVA: 0x00261BF2 File Offset: 0x0025FDF2
		public OperationActorState operationType { get; private set; }

		// Token: 0x17000EFB RID: 3835
		// (get) Token: 0x06005445 RID: 21573 RVA: 0x00261BFB File Offset: 0x0025FDFB
		private IOperation currentOperationTemplate
		{
			get
			{
				if (!(this.activeButton != null))
				{
					return this.operationTemplateForced;
				}
				return this.activeButton.operationType;
			}
		}

		// Token: 0x17000EFC RID: 3836
		// (get) Token: 0x06005446 RID: 21574 RVA: 0x00261C1D File Offset: 0x0025FE1D
		public IOperation SelectedOperation
		{
			get
			{
				return this.currentOperationTemplate;
			}
		}

		// Token: 0x17000EFD RID: 3837
		// (get) Token: 0x06005447 RID: 21575 RVA: 0x00261C25 File Offset: 0x0025FE25
		public bool IsVisible
		{
			get
			{
				return this.masterOperationCanvas.enabled;
			}
		}

		// Token: 0x17000EFE RID: 3838
		// (get) Token: 0x06005448 RID: 21576 RVA: 0x00261C32 File Offset: 0x0025FE32
		public bool IsInTargetSelectionMode
		{
			get
			{
				return this.IsVisible && this.selectingTarget;
			}
		}

		// Token: 0x17000EFF RID: 3839
		// (get) Token: 0x06005449 RID: 21577 RVA: 0x00261C44 File Offset: 0x0025FE44
		public TIGameState ProspectiveTarget
		{
			get
			{
				if (!this.IsInTargetSelectionMode)
				{
					return null;
				}
				return this.currentTarget;
			}
		}

		// Token: 0x0600544A RID: 21578 RVA: 0x00261C58 File Offset: 0x0025FE58
		public override void Initialize()
		{
			OperationCanvasController.Singleton = this;
			this.targetDropdownTemplateHeight = this.targetDropdown.template.sizeDelta.y;
			base.Initialize();
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnStartNewMissionPhase), "CouncilorMissionUpdate", null, false, false);
			GameControl.eventManager.AddListener<DeployArmyToRegionRequested>(new EventManager.EventDelegate<DeployArmyToRegionRequested>(this.OnReverseArmyDestinationTriggered), null, null, true, false);
			GameControl.eventManager.AddListener<StartArmyOperation>(new EventManager.EventDelegate<StartArmyOperation>(this.UpdateCanvas), null, null, true, false);
			GameControl.eventManager.AddListener<StartFleetOperation>(new EventManager.EventDelegate<StartFleetOperation>(this.UpdateCanvas), null, null, true, false);
			GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.UpdateCanvas), null, null, true, false);
			GameControl.eventManager.AddListener<StartFactionOperation>(new EventManager.EventDelegate<StartFactionOperation>(this.UpdateCanvas), null, null, true, false);
			GameControl.eventManager.AddListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.UpdateCanvas), null, null, true, false);
			GameControl.eventManager.AddListener<FleetOperationWithDurationComplete>(new EventManager.EventDelegate<FleetOperationWithDurationComplete>(this.UpdateCanvas), null, null, true, false);
			GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.OnMapActivationChanged), null, null, true, false);
			GameControl.eventManager.AddListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelected), null, null, true, false);
			GameControl.eventManager.AddListener<SpaceBodySelectedEvent>(new EventManager.EventDelegate<SpaceBodySelectedEvent>(this.NaturalSpaceObjectSelected), null, null, true, false);
			GameControl.eventManager.AddListener<LagrangePointSelectedEvent>(new EventManager.EventDelegate<LagrangePointSelectedEvent>(this.NaturalSpaceObjectSelected), null, null, true, false);
			GameControl.eventManager.AddListener<ArmyMapItemSelected>(new EventManager.EventDelegate<ArmyMapItemSelected>(this.ArmySelected), null, null, false, false);
			GameControl.eventManager.AddListener<CouncilorSelectedOffMap>(new EventManager.EventDelegate<CouncilorSelectedOffMap>(this.OnCouncilorSelected), null, null, true, false);
			GameControl.eventManager.AddListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.OnCouncilorSelected), null, null, true, false);
			GameControl.eventManager.AddListener<FleetAvailabilityChange>(new EventManager.EventDelegate<FleetAvailabilityChange>(this.OnFleetAvailabilityChange), null, null, true, false);
			GameControl.eventManager.AddListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.OnFleetCoreStatusChange), null, null, true, false);
			GameControl.eventManager.AddListener<ForceTrajectorySelectionUI>(new EventManager.EventDelegate<ForceTrajectorySelectionUI>(this.OnForceTrajectorySelectionUI), null, null, true, false);
			GameControl.eventManager.AddListener<ForceTrajectorySelectionUI_NoCurrentTrajectory>(new EventManager.EventDelegate<ForceTrajectorySelectionUI_NoCurrentTrajectory>(this.OnForceTrajectorySelectionUI_NoCurrentTrajectory), null, null, true, false);
			GameControl.eventManager.AddListener<ForceFleetOperation>(new EventManager.EventDelegate<ForceFleetOperation>(this.OnForceFleetOperation), null, null, true, false);
			this.masterOperationCanvas.enabled = false;
			this.operationMasterPanel.enabled = false;
			this.multiSelectArmyCanvas.enabled = false;
			this.changeTrajectoryCanvas.transform.parent.gameObject.SetActive(true);
			this.maximizeButtonGameObject.SetActive(false);
			this.fleetSplitPanel.SetActive(false);
			this.propellantSharingPanel.SetActive(false);
			this.durationReportObject.SetActive(false);
			base.canvasManager.RegisterAssetPanelDisableOrder(AssetPanel.MyFleet, new Action(this.OnDisableMyFleetPanel));
			base.canvasManager.RegisterAssetPanelDisableOrder(AssetPanel.MyArmy, new Action(this.OnDisableMyArmyPanel));
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.SpaceBodyDetail, new Action(this.OnDisableSpaceOpPanel));
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.LagrangeDetail, new Action(this.OnDisableSpaceOpPanel));
			this.targetNameObject.SetActive(true);
			this.changeTrajectoryPromptHeaderText.SetText(Loc.T("UI.Operations.ChangeTrajectoryPrompt"));
			this.changeTrajectoryPromptConfirmText.SetText(Loc.T("UI.Notifications.Confirm"));
			this.changeTrajectoryPromptCancelText.SetText(Loc.T("UI.Notifications.Cancel"));
			this.changeTrajectoryCanvas.enabled = false;
			TargetSelectionTool targetSelectionTool = this.targetSelectionTool;
			targetSelectionTool.onTargetSelected = (TargetSelectionTool.OnTargetSelected)Delegate.Combine(targetSelectionTool.onTargetSelected, new TargetSelectionTool.OnTargetSelected(this.OnTargetSelectionToolElementClicked));
			TargetSelectionTool targetSelectionTool2 = this.targetSelectionTool;
			targetSelectionTool2.onFilterSelected = (TargetSelectionTool.OnFilterSelected)Delegate.Combine(targetSelectionTool2.onFilterSelected, new TargetSelectionTool.OnFilterSelected(this.OnNavigationButtonClicked));
			this.targetSelectionTool.GetHeaderString = delegate(TargetSelectionTool targetSelectionTool_)
			{
				if (targetSelectionTool_.IsInOrbitSelectionMode)
				{
					return Loc.T("UI.Operations.SelectOrbit", new object[] { targetSelectionTool_.Filter.ref_naturalSpaceObject.displayName });
				}
				if (targetSelectionTool_.IsinHabSiteSelectionMode)
				{
					return Loc.T("UI.Operations.SelectHabSite", new object[] { targetSelectionTool_.Filter.ref_spaceBody.displayName });
				}
				return "Unsupported Selection Mode";
			};
			this.targetSelectionTool.Close();
			ThrustProfileTool thrustProfileTool = this.thrustProfileTool;
			thrustProfileTool.onCandidateTrajectoriesComputed = (ThrustProfileTool.OnCandidateTrajectoriesComputed)Delegate.Combine(thrustProfileTool.onCandidateTrajectoriesComputed, new ThrustProfileTool.OnCandidateTrajectoriesComputed(delegate
			{
				this.confirmButton.interactable = this.thrustProfileTool.CanReachTarget;
			}));
			this.thrustProfileTool.GetHeaderString = delegate(ThrustProfileTool thrustProfileTool_)
			{
				TIGameState target = thrustProfileTool_.Target;
				string text = ((target != null) ? target.GetDisplayName(GameControl.control.activePlayer) : null) ?? string.Empty;
				if (thrustProfileTool_.CanReachTarget)
				{
					return Loc.T("UI.Operations.SelectThrustProfile", new object[] { text });
				}
				return Loc.T("UI.Operations.NoThrustProfile", new object[] { text });
			};
			this.CloseThrustProfileTool();
			this.splitAllDamagedButtonText.SetText(Loc.T("UI.Operations.SetDamaged"));
			this.resetSplitFleetButtonText.SetText(Loc.T("UI.Operations.ResetSplitFleet"));
			this.propellantSharingHeader.SetText(Loc.T("UI.Operations.PropellantSharingHeader"));
			this.selectPropellantHeader.SetText(Loc.T("UI.Operations.PropellantTypeHeader"));
			this.giverColumnHeader.SetText(Loc.T("UI.Operations.GiverColumnHeader"));
			this.selectedTakerColumnHeader.SetText(Loc.T("UI.Operations.SelectedTakerColumnHeader"));
			this.availableTakerColumnHeader.SetText(Loc.T("UI.Operations.AvailableTakerColumnHeader"));
			this.resetTakersButtonText.SetText(Loc.T("UI.Operations.ResetSharePropellantButton"));
			this.equalDistroButtonText.SetText(Loc.T("UI.Operations.EqualizeDV"));
			this.ResetTakersButtonTip.SetText("BodyText", Loc.T("UI.Operations.ResetButtonTip"));
			this.EqualizeDistributionButtonTip.SetText("BodyText", Loc.T("UI.Operations.EqualizeButtonTip"));
			this.sharePropellantInstructions.SetText(Loc.T("UI.Operations.ShareInstructions"));
			this.multiSelectArmyHeaderText.SetText(Loc.T("UI.Operations.ArmyGroupHeader"));
			this.InitializeOfficerTransferCanvas();
		}

		// Token: 0x0600544B RID: 21579 RVA: 0x002621BC File Offset: 0x002603BC
		public override void Show()
		{
			base.Show();
			this.AddListeners();
			this.masterOperationCanvas.enabled = true;
			if (!this.changingInvalidTrajectory)
			{
				this.operationMasterPanel.enabled = true;
				this.UpdateOperationBar();
				return;
			}
			base.StartCoroutine(this.ForceTargetSelectLayoutRefresh());
		}

		// Token: 0x0600544C RID: 21580 RVA: 0x00262209 File Offset: 0x00260409
		public override void Hide()
		{
			base.Hide();
			this.RemoveListeners();
			if (this.masterOperationCanvas != null)
			{
				this.masterOperationCanvas.enabled = false;
				this.operationMasterPanel.enabled = false;
			}
		}

		// Token: 0x0600544D RID: 21581 RVA: 0x00262240 File Offset: 0x00260440
		public override void Refresh()
		{
			if ((this.actorState == null || this.actorState.archived) && !this.changingInvalidTrajectory)
			{
				this.Shutdown();
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
			if ((this.operationControlsDirty || (this.actorState != null && TIFrameCounter.FrameCount % 3607 == 0)) && !this.changingInvalidTrajectory)
			{
				this.UpdateOperationControls();
				this.operationControlsDirty = false;
			}
		}

		// Token: 0x0600544E RID: 21582 RVA: 0x002622DE File Offset: 0x002604DE
		private IEnumerator ForceTargetSelectLayoutRefresh()
		{
			yield return null;
			LayoutElement component = this.targetSelectionTool.GetComponent<LayoutElement>();
			float num = component.flexibleHeight;
			component.flexibleHeight = num + 1f;
			LayoutElement component2 = this.targetSelectionTool.GetComponent<LayoutElement>();
			num = component2.flexibleHeight;
			component2.flexibleHeight = num - 1f;
			yield break;
		}

		// Token: 0x0600544F RID: 21583 RVA: 0x002622F0 File Offset: 0x002604F0
		private void AddListeners()
		{
			GameControl.eventManager.AddListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnResourcesUpdated), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null, null, true, false);
			GameControl.eventManager.AddListener<TargetShipsForFleetSplit>(new EventManager.EventDelegate<TargetShipsForFleetSplit>(this.OnFleetSplitProposed), null, null, false, false);
			GameControl.eventManager.AddListener<DetargetShipForFleetSplit>(new EventManager.EventDelegate<DetargetShipForFleetSplit>(this.OnFleetSplitConcluded), null, null, false, false);
			GameControl.eventManager.AddListener<InitiateSharePropellant>(new EventManager.EventDelegate<InitiateSharePropellant>(this.OnSharePropellantProposed), null, null, false, false);
			GameControl.eventManager.AddListener<InitiateTransferOfficers>(new EventManager.EventDelegate<InitiateTransferOfficers>(this.OnTransferOfficersProposed), null, null, false, false);
			GameControl.eventManager.AddListener<TargetHabSites>(new EventManager.EventDelegate<TargetHabSites>(this.InitiateTargetHabSites), null, null, false, false);
			GameControl.eventManager.AddListener<DeTargetHabSites>(new EventManager.EventDelegate<DeTargetHabSites>(this.EndTargetHabSites), null, null, false, false);
			GameControl.eventManager.AddListener<TargetOrbits>(new EventManager.EventDelegate<TargetOrbits>(this.InitiateTargetOrbits), null, null, false, false);
			GameControl.eventManager.AddListener<DeTargetOrbits>(new EventManager.EventDelegate<DeTargetOrbits>(this.EndTargetOrbits), null, null, false, false);
			GameControl.eventManager.AddListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.OnSpaceCombatInitiated), null, null, false, false);
			GameControl.eventManager.AddListener<MultiSelectArmiesSelected>(new EventManager.EventDelegate<MultiSelectArmiesSelected>(this.OnMultiSelectArmiesSelected), null, null, false, false);
			GameControl.eventManager.AddListener<NarrativeEventPushedToPlayer>(new EventManager.EventDelegate<NarrativeEventPushedToPlayer>(this.OnNarrativeEventFired), null, null, false, false);
			GameControl.eventManager.AddListener<PolicyMenuPushedToPlayer>(new EventManager.EventDelegate<PolicyMenuPushedToPlayer>(this.OnPolicyMenuFired), null, null, false, false);
		}

		// Token: 0x06005450 RID: 21584 RVA: 0x00262470 File Offset: 0x00260670
		private void RemoveListeners()
		{
			GameControl.eventManager.RemoveListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnResourcesUpdated), null);
			GameControl.eventManager.RemoveListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null);
			GameControl.eventManager.RemoveListener<TargetShipsForFleetSplit>(new EventManager.EventDelegate<TargetShipsForFleetSplit>(this.OnFleetSplitProposed), null);
			GameControl.eventManager.RemoveListener<DetargetShipForFleetSplit>(new EventManager.EventDelegate<DetargetShipForFleetSplit>(this.OnFleetSplitConcluded), null);
			GameControl.eventManager.RemoveListener<InitiateSharePropellant>(new EventManager.EventDelegate<InitiateSharePropellant>(this.OnSharePropellantProposed), null);
			GameControl.eventManager.RemoveListener<InitiateTransferOfficers>(new EventManager.EventDelegate<InitiateTransferOfficers>(this.OnTransferOfficersProposed), null);
			GameControl.eventManager.RemoveListener<TargetHabSites>(new EventManager.EventDelegate<TargetHabSites>(this.InitiateTargetHabSites), null);
			GameControl.eventManager.RemoveListener<DeTargetHabSites>(new EventManager.EventDelegate<DeTargetHabSites>(this.EndTargetHabSites), null);
			GameControl.eventManager.RemoveListener<TargetOrbits>(new EventManager.EventDelegate<TargetOrbits>(this.InitiateTargetOrbits), null);
			GameControl.eventManager.RemoveListener<DeTargetOrbits>(new EventManager.EventDelegate<DeTargetOrbits>(this.EndTargetOrbits), null);
			GameControl.eventManager.RemoveListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.OnSpaceCombatInitiated), null);
			GameControl.eventManager.RemoveListener<MultiSelectArmiesSelected>(new EventManager.EventDelegate<MultiSelectArmiesSelected>(this.OnMultiSelectArmiesSelected), null);
		}

		// Token: 0x06005451 RID: 21585 RVA: 0x00262594 File Offset: 0x00260794
		public override void OnDestroy()
		{
			this.RemoveListeners();
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnStartNewMissionPhase), "CouncilorMissionUpdate");
			GameControl.eventManager.RemoveListener<DeployArmyToRegionRequested>(new EventManager.EventDelegate<DeployArmyToRegionRequested>(this.OnReverseArmyDestinationTriggered), null);
			GameControl.eventManager.RemoveListener<StartArmyOperation>(new EventManager.EventDelegate<StartArmyOperation>(this.UpdateCanvas), null);
			GameControl.eventManager.RemoveListener<StartFleetOperation>(new EventManager.EventDelegate<StartFleetOperation>(this.UpdateCanvas), null);
			GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.UpdateCanvas), null);
			GameControl.eventManager.RemoveListener<StartFactionOperation>(new EventManager.EventDelegate<StartFactionOperation>(this.UpdateCanvas), null);
			GameControl.eventManager.RemoveListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.UpdateCanvas), null);
			GameControl.eventManager.RemoveListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.OnMapActivationChanged), null);
			GameControl.eventManager.RemoveListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelected), null);
			GameControl.eventManager.RemoveListener<SpaceBodySelectedEvent>(new EventManager.EventDelegate<SpaceBodySelectedEvent>(this.NaturalSpaceObjectSelected), null);
			GameControl.eventManager.RemoveListener<LagrangePointSelectedEvent>(new EventManager.EventDelegate<LagrangePointSelectedEvent>(this.NaturalSpaceObjectSelected), null);
			GameControl.eventManager.RemoveListener<ArmyMapItemSelected>(new EventManager.EventDelegate<ArmyMapItemSelected>(this.ArmySelected), null);
			GameControl.eventManager.RemoveListener<CouncilorSelectedOffMap>(new EventManager.EventDelegate<CouncilorSelectedOffMap>(this.OnCouncilorSelected), null);
			GameControl.eventManager.RemoveListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.OnCouncilorSelected), null);
			GameControl.eventManager.RemoveListener<FleetOperationWithDurationComplete>(new EventManager.EventDelegate<FleetOperationWithDurationComplete>(this.UpdateCanvas), null);
			GameControl.eventManager.RemoveListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.OnFleetCoreStatusChange), null);
			GameControl.eventManager.RemoveListener<NarrativeEventPushedToPlayer>(new EventManager.EventDelegate<NarrativeEventPushedToPlayer>(this.OnNarrativeEventFired), null);
			GameControl.eventManager.RemoveListener<PolicyMenuPushedToPlayer>(new EventManager.EventDelegate<PolicyMenuPushedToPlayer>(this.OnPolicyMenuFired), null);
			base.OnDestroy();
		}

		// Token: 0x06005452 RID: 21586 RVA: 0x0026274F File Offset: 0x0026094F
		public void Shutdown()
		{
			if (this.Visible() && !this.changingInvalidTrajectory)
			{
				this.CloseActionPanel(true);
				this.Hide();
				return;
			}
			if (this.changingInvalidTrajectory)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
		}

		// Token: 0x06005453 RID: 21587 RVA: 0x00262783 File Offset: 0x00260983
		private void OnInfoScreenOpened(InfoScreenOpened e)
		{
			if (this.Visible())
			{
				this.Shutdown();
				GameControl.eventManager.AddListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreOperationsControlsCanvas), null, null, true, false);
			}
		}

		// Token: 0x06005454 RID: 21588 RVA: 0x002627AD File Offset: 0x002609AD
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.targetDropdown.template.sizeDelta = new Vector2(this.targetDropdown.template.sizeDelta.x, this.targetDropdownTemplateHeight / TIUtilities.UIScaleFactor());
		}

		// Token: 0x06005455 RID: 21589 RVA: 0x002627EB File Offset: 0x002609EB
		public void CloseForReverseSelection()
		{
			this.Shutdown();
		}

		// Token: 0x06005456 RID: 21590 RVA: 0x002627F3 File Offset: 0x002609F3
		private void RestoreOperationsControlsCanvas(InfoScreenClosed e)
		{
			this.Show();
			GameControl.eventManager.RemoveListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreOperationsControlsCanvas), null);
		}

		// Token: 0x06005457 RID: 21591 RVA: 0x00262812 File Offset: 0x00260A12
		private void UpdateCanvas(TIGameState actingState, bool globalForceUpdate = false)
		{
			if (actingState == this.actorState || globalForceUpdate)
			{
				this.UpdateOperationControls();
				return;
			}
			if (this.actorState == null)
			{
				this.CloseActionPanel(true);
			}
		}

		// Token: 0x06005458 RID: 21592 RVA: 0x00262840 File Offset: 0x00260A40
		private void UpdateCanvas(StartArmyOperation e)
		{
			this.UpdateCanvas(e.actingState, false);
		}

		// Token: 0x06005459 RID: 21593 RVA: 0x0026284F File Offset: 0x00260A4F
		private void UpdateCanvas(StartFleetOperation e)
		{
			this.UpdateCanvas(e.actingState, false);
		}

		// Token: 0x0600545A RID: 21594 RVA: 0x0026285E File Offset: 0x00260A5E
		private void UpdateCanvas(FleetArrivesAtDestination e)
		{
			this.UpdateCanvas(e.fleet, false);
		}

		// Token: 0x0600545B RID: 21595 RVA: 0x0026286D File Offset: 0x00260A6D
		private void UpdateCanvas(StartFactionOperation e)
		{
			this.UpdateCanvas(e.actingState, false);
		}

		// Token: 0x0600545C RID: 21596 RVA: 0x0026287C File Offset: 0x00260A7C
		private void UpdateCanvas(OperationExecuted e)
		{
			this.UpdateCanvas(e.actingState, false);
		}

		// Token: 0x0600545D RID: 21597 RVA: 0x0026288B File Offset: 0x00260A8B
		private void UpdateCanvas(FleetOperationWithDurationComplete e)
		{
			this.UpdateCanvas(e.fleet, false);
		}

		// Token: 0x0600545E RID: 21598 RVA: 0x0026289A File Offset: 0x00260A9A
		private void ArmySelected(ArmyMapItemSelected e)
		{
			this.ArmySelected(e.army);
		}

		// Token: 0x0600545F RID: 21599 RVA: 0x002628A8 File Offset: 0x00260AA8
		private void FleetSelected(FleetSelectedEvent e)
		{
			this.FleetSelected(e.fleet, false);
		}

		// Token: 0x06005460 RID: 21600 RVA: 0x002628B7 File Offset: 0x00260AB7
		private void NaturalSpaceObjectSelected(SpaceBodySelectedEvent e)
		{
			this.NaturalSpaceObjectSelected(e.spaceBody);
		}

		// Token: 0x06005461 RID: 21601 RVA: 0x002628C5 File Offset: 0x00260AC5
		private void NaturalSpaceObjectSelected(LagrangePointSelectedEvent e)
		{
			this.NaturalSpaceObjectSelected(e.lagrangePoint);
		}

		// Token: 0x06005462 RID: 21602 RVA: 0x002628D3 File Offset: 0x00260AD3
		private void OnCouncilorSelected(CouncilorMapItemSelected e)
		{
			this.OnCouncilorSelected(e.councilor);
		}

		// Token: 0x06005463 RID: 21603 RVA: 0x002628E1 File Offset: 0x00260AE1
		private void OnCouncilorSelected(CouncilorSelectedOffMap e)
		{
			this.OnCouncilorSelected(e.councilor);
		}

		// Token: 0x06005464 RID: 21604 RVA: 0x002628EF File Offset: 0x00260AEF
		private void OnCouncilorSelected(TICouncilorState councilor)
		{
			if (this.operationType == OperationActorState.SpaceBody && councilor.faction == base.activePlayer && TIMissionPhaseState.InMissionPhase())
			{
				this.DisableCurrentOperation(false);
			}
		}

		// Token: 0x06005465 RID: 21605 RVA: 0x0026291B File Offset: 0x00260B1B
		private void OnResourcesUpdated(FactionResourcesUpdated e)
		{
			if (this.iconsPanel.activeSelf)
			{
				this.operationControlsDirty = true;
			}
		}

		// Token: 0x06005466 RID: 21606 RVA: 0x00262934 File Offset: 0x00260B34
		private void OnMapActivationChanged(MapActivationChangedEvent e)
		{
			TIGameState tigameState = this.actorState;
			if (((tigameState != null) ? tigameState.ref_army : null) == this.actorState && GameControl.control.viewMgr.currentView != ViewType.PoliticalMap && !this.changingInvalidTrajectory)
			{
				this.Shutdown();
			}
		}

		// Token: 0x06005467 RID: 21607 RVA: 0x00262980 File Offset: 0x00260B80
		private void ArmyUpdated(ArmyStatusUpdate e)
		{
			if (e.army == this.actorState)
			{
				this.operationControlsDirty = true;
			}
		}

		// Token: 0x06005468 RID: 21608 RVA: 0x0026299C File Offset: 0x00260B9C
		private void OnArmyAssignedToFaction(ArmyAssignedToFaction e)
		{
			if (this.operationType == OperationActorState.Army)
			{
				TIGameState tigameState = this.actorState;
				if (((tigameState != null) ? tigameState.ref_army : null) == e.army && e.faction != base.activePlayer)
				{
					this.DisableCurrentOperation(false);
				}
			}
		}

		// Token: 0x06005469 RID: 21609 RVA: 0x002629EB File Offset: 0x00260BEB
		private void OnFleetAvailabilityChange(FleetAvailabilityChange e)
		{
			if (e.fleet == this.actorState)
			{
				this.operationControlsDirty = true;
			}
		}

		// Token: 0x0600546A RID: 21610 RVA: 0x00262A08 File Offset: 0x00260C08
		private void OnFleetCoreStatusChange(FleetCoreStatusChange e)
		{
			if (this.actorState != null && this.targetSelectionTool.gameObject.activeSelf)
			{
				this.targetSelectionTool.UpdateListUI();
				if (this.thrustProfileTool.gameObject.activeSelf && !TIGameState.Valid(this.thrustProfileTool.Target))
				{
					this.InitTargetSelection(null);
				}
			}
			if (this.operationType == OperationActorState.Fleet && this.currentTarget != null && !TIGameState.Valid(this.currentTarget))
			{
				this.CloseActionPanel(false);
			}
		}

		// Token: 0x0600546B RID: 21611 RVA: 0x00262A96 File Offset: 0x00260C96
		private void OnSpaceCombatInitiated(SpaceCombatInitiated e)
		{
			if (e.combat.IncludesFaction(GameControl.control.activePlayer))
			{
				this.DisableCurrentOperation(true);
			}
		}

		// Token: 0x0600546C RID: 21612 RVA: 0x00262AB6 File Offset: 0x00260CB6
		private void OnNarrativeEventFired(NarrativeEventPushedToPlayer e)
		{
			this.DisableCurrentOperation(true);
		}

		// Token: 0x0600546D RID: 21613 RVA: 0x00262ABF File Offset: 0x00260CBF
		private void OnPolicyMenuFired(PolicyMenuPushedToPlayer e)
		{
			this.DisableCurrentOperation(true);
		}

		// Token: 0x0600546E RID: 21614 RVA: 0x00262AC8 File Offset: 0x00260CC8
		private void OnReverseArmyDestinationTriggered(DeployArmyToRegionRequested e)
		{
			if (this.IsInTargetSelectionMode)
			{
				return;
			}
			TIArmyState army = e.army;
			TIRegionState tiregionState = army.currentRegion;
			TIRegionState region = e.region;
			Player playerControl = army.faction.playerControl;
			OperationData operationData = army.CurrentOperations().FirstOrDefault<OperationData>((OperationData x) => x.operation is DeployArmyOperation);
			if (army != this.actorState)
			{
				this.ArmySelected(army);
			}
			this.UpdateOperationControls();
			if (army.CurrentOperations().Count != 0)
			{
				if (!army.CurrentOperations().All<OperationData>((OperationData x) => x.operation is DeployArmyOperation))
				{
					goto IL_041A;
				}
			}
			bool isAltKeyDown = TIInputManager.IsAltKeyDown;
			bool flag = TIInputManager.IsAltKeyDown && operationData != null;
			if (flag)
			{
				if (army.destinationQueue.Count > 0)
				{
					tiregionState = army.destinationQueue.Last<TIRegionState>();
				}
				else
				{
					tiregionState = operationData.target.ref_region;
				}
			}
			List<TIRegionState> list = army.GetJourney_AvoidEnemyRegions(tiregionState, region);
			if (list == null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			if (tiregionState == region)
			{
				if (e.deployAll)
				{
					DeployArmiesOperation deployArmiesOperation = new DeployArmiesOperation(false);
					using (List<TIArmyState>.Enumerator enumerator = DeployArmiesOperation.GetEligibleArmies(army).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIArmyState tiarmyState = enumerator.Current;
							tiarmyState.ClearOperations();
							playerControl.StartAction(new ClearArmyDestinationQueueAction(tiarmyState));
							deployArmiesOperation.OnOperationConfirm(this.actorState, region, null, null);
						}
						goto IL_01A6;
					}
				}
				TIOperationTemplate tioperationTemplate = new DeployArmyOperation_OpenTarget(false);
				army.ClearOperations();
				playerControl.StartAction(new ClearArmyDestinationQueueAction(army));
				tioperationTemplate.OnOperationConfirm(this.actorState, region, null, null);
				IL_01A6:
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmArmyOperation", false, false);
				GameControl.eventManager.TriggerEvent(new ForceAllArmyUpdateInRegion(e.region), null, new object[] { e.region });
				return;
			}
			list = list.GetRange(1, list.Count - 1);
			if (isAltKeyDown && tiregionState.ConnectedRegions.Contains(region))
			{
				TIArmyState.IsTraversible(tiregionState, region, army);
			}
			if (false)
			{
				list = new List<TIRegionState> { region };
			}
			if (list.Count == 0)
			{
				return;
			}
			if (e.deployAll)
			{
				DeployArmiesOperation deployArmiesOperation2 = new DeployArmiesOperation(flag || region != army.currentRegion);
				if (deployArmiesOperation2.GetPossibleTargets(this.actorState, null).Contains(region))
				{
					if (!flag)
					{
						foreach (TIArmyState tiarmyState2 in DeployArmiesOperation.GetEligibleArmies(army))
						{
							tiarmyState2.ClearOperations();
							playerControl.StartAction(new ClearArmyDestinationQueueAction(tiarmyState2));
						}
						deployArmiesOperation2.OnOperationConfirm(this.actorState, list.First<TIRegionState>(), null, null);
						list = list.GetRange(1, list.Count - 1);
					}
					foreach (TIArmyState tiarmyState3 in DeployArmiesOperation.GetEligibleArmies(army))
					{
						foreach (TIRegionState tiregionState2 in list)
						{
							playerControl.StartAction(new QueueArmyDestinationAction(tiarmyState3, tiregionState2));
						}
					}
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmArmyOperation", false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
			}
			else if (new DeployArmyOperation_OpenTarget(true).GetPossibleTargets(this.actorState, null).Contains(region))
			{
				if (!flag)
				{
					army.ClearOperations();
					playerControl.StartAction(new ClearArmyDestinationQueueAction(army));
					new DeployArmyOperation_OpenTarget(false).OnOperationConfirm(this.actorState, list.First<TIRegionState>(), null, null);
					list = list.GetRange(1, list.Count - 1);
				}
				foreach (TIRegionState tiregionState3 in list)
				{
					playerControl.StartAction(new QueueArmyDestinationAction(army, tiregionState3));
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmArmyOperation", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			IL_041A:
			GameControl.eventManager.TriggerEvent(new ForceAllArmyUpdateInRegion(e.region), null, new object[] { e.region });
		}

		// Token: 0x0600546F RID: 21615 RVA: 0x00262F54 File Offset: 0x00261154
		private void OnForceTrajectorySelectionUI_NoCurrentTrajectory(ForceTrajectorySelectionUI_NoCurrentTrajectory e)
		{
			TIUtilities.GotoGameState(e.fleet, false, true, true, false, true, -1f);
			this.FleetSelected(e.fleet, false);
			this.OnOperationSelected(OperationsManager.operationsLookup[typeof(TransferOperation)] as TIOperationTemplate, e.target);
		}

		// Token: 0x06005470 RID: 21616 RVA: 0x00262FA8 File Offset: 0x002611A8
		private void OnForceFleetOperation(ForceFleetOperation e)
		{
			this.FleetSelected(e.fleet, true);
			TIUtilities.GotoGameState(e.fleet, false, true, true, false, true, -1f);
			this.OnOperationSelected(e.operation, e.target);
		}

		// Token: 0x06005471 RID: 21617 RVA: 0x00262FDE File Offset: 0x002611DE
		public void DisableCurrentOperation(bool fullShutdown = false)
		{
			this.CheckAndRemoveArmyListener();
			this.operationType = OperationActorState.None;
			this.actorState = null;
			this.CloseActionPanel(fullShutdown);
		}

		// Token: 0x06005472 RID: 21618 RVA: 0x00262FFC File Offset: 0x002611FC
		private void OnDisableMyFleetPanel()
		{
			if (this.operationType == OperationActorState.Fleet)
			{
				TIGameState tigameState = this.actorState;
				if (tigameState != null && tigameState.isSpaceFleetState)
				{
					this.DisableCurrentOperation(false);
				}
			}
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			if (uiotherSelectedState != null && uiotherSelectedState.isNaturalSpaceObjectState)
			{
				this.NaturalSpaceObjectSelected(GeneralControlsController.UIOtherSelectedState.ref_naturalSpaceObject);
			}
			this.HideTutorials();
		}

		// Token: 0x06005473 RID: 21619 RVA: 0x00263058 File Offset: 0x00261258
		private void OnDisableMyArmyPanel()
		{
			if (this.operationType == OperationActorState.Army)
			{
				TIGameState tigameState = this.actorState;
				if (tigameState != null && tigameState.isArmyState)
				{
					this.DisableCurrentOperation(false);
				}
			}
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			if (uiotherSelectedState != null && uiotherSelectedState.isNaturalSpaceObjectState)
			{
				this.NaturalSpaceObjectSelected(GeneralControlsController.UIOtherSelectedState.ref_naturalSpaceObject);
			}
			this.HideTutorials();
		}

		// Token: 0x06005474 RID: 21620 RVA: 0x002630B4 File Offset: 0x002612B4
		private void OnDisableSpaceOpPanel()
		{
			if (this.operationType == OperationActorState.SpaceBody)
			{
				TIGameState tigameState = this.actorState;
				if (tigameState != null && tigameState.isFactionState)
				{
					if (GeneralControlsController.UIPlayerInTargetingMode && GeneralControlsController.CurrentValidTarget(this.currentTarget))
					{
						return;
					}
					this.DisableCurrentOperation(false);
				}
			}
			if (this.operationType != OperationActorState.Fleet)
			{
				TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
				if (uiselectedAssetState != null && uiselectedAssetState.isSpaceFleetState)
				{
					this.FleetSelected(GeneralControlsController.UISelectedAssetState.ref_fleet, false);
					goto IL_00BF;
				}
			}
			if (this.operationType != OperationActorState.Army)
			{
				TIGameState uiselectedAssetState2 = GeneralControlsController.UISelectedAssetState;
				if (uiselectedAssetState2 != null && uiselectedAssetState2.isArmyState)
				{
					this.ArmySelected(GeneralControlsController.UISelectedAssetState.ref_army);
					goto IL_00BF;
				}
			}
			TIGameState uiselectedAssetState3 = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState3 != null && uiselectedAssetState3.isCouncilorState)
			{
				TIUtilities.GotoGameState(GeneralControlsController.UISelectedAssetState, false, true, true, true, false, -1f);
			}
			IL_00BF:
			this.HideTutorials();
		}

		// Token: 0x06005475 RID: 21621 RVA: 0x00263186 File Offset: 0x00261386
		private void OnStartNewMissionPhase(TimeEventStart e)
		{
			if (this.currentTargeting != null)
			{
				this.ShutdownTargetSelection();
			}
		}

		// Token: 0x06005476 RID: 21622 RVA: 0x00263198 File Offset: 0x00261398
		private void FleetSelected(TISpaceFleetState fleet, bool forceRefresh = false)
		{
			int num;
			if (GeneralControlsController.UIPlayerInTargetingMode)
			{
				if (!GeneralControlsController.CurrentValidTarget(fleet))
				{
					num = (fleet.ships.Any<TISpaceShipState>((TISpaceShipState x) => GeneralControlsController.CurrentValidTarget(x)) ? 1 : 0);
				}
				else
				{
					num = 1;
				}
			}
			else
			{
				num = 0;
			}
			if (num == 0 || forceRefresh)
			{
				if (fleet.faction == base.activePlayer)
				{
					if (!this.changingInvalidTrajectory && (fleet != this.actorState || !this.masterOperationCanvas.enabled || forceRefresh))
					{
						this.DisableCurrentOperation(false);
						this.ShutdownTargetSelection();
						this.AddCanvasToStack();
						this.actorState = fleet;
						this.targetBase = null;
						if (base.canvasManager.GetActiveInfoPanel() == InfoPanel.None)
						{
							GeneralControlsController.SetUIOtherSelectedState(null);
						}
						this.operationType = OperationActorState.Fleet;
						this.UpdateOperationControls();
						this.HideTutorials();
						this.fleetOperationsUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_OperationScreenCanvas_FleetOperations, false, true);
						return;
					}
				}
				else if (GeneralControlsController.UIPlayerInTargetingMode)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
			}
		}

		// Token: 0x06005477 RID: 21623 RVA: 0x002632A0 File Offset: 0x002614A0
		private void OnForceTrajectorySelectionUI(ForceTrajectorySelectionUI e)
		{
			this.changeTrajectoryFaction = e.maneuveringFleetFaction;
			this.changeTrajectoryFleet = e.maneuveringFleet;
			this.changeTrajectoryTargetFleet = e.targetFleet;
			this.masterOperationCanvas.enabled = true;
			this.operationMasterPanel.enabled = false;
			this.changeTrajectoryCanvas.enabled = true;
			this.changeTrajectoryCanvas.GetComponent<GraphicRaycaster>().enabled = true;
			this.thrustProfileTool.isChangeTrajectory = true;
			bool flag = false;
			if (this.changeTrajectoryTargetFleet != null)
			{
				this.changeTrajectoryPromptMEssageText.SetText(Loc.T("UI.Operations.ChangeTrajectoryInfo", new object[]
				{
					this.changeTrajectoryFleet.GetDisplayName(this.changeTrajectoryFaction),
					this.changeTrajectoryTargetFleet.GetDisplayName(this.changeTrajectoryFaction)
				}));
				this.targetSelectionTool.Close();
			}
			else
			{
				this.changeTrajectoryPromptMEssageText.SetText(Loc.T((this.changeTrajectoryFleet.ref_fleet.delayedTransferAbortNotification == null) ? "UI.Operations.ChangeTrajectoryInfoPostCombat" : "UI.Operations.ChangeTrajectoryInfoPostCombatAdHoc", new object[] { this.changeTrajectoryFleet.GetDisplayName(this.changeTrajectoryFaction) }));
				this.targetDropdownObject.SetActive(false);
				IOperation operation = OperationsManager.fleetOperations.First<IOperation>((IOperation x) => x is TransferOperation);
				this.operationTemplateForced = operation;
				this.targetSelectionTool.Operation = operation;
				this.NewOperationTargetBase(this.changeTrajectoryFleet.ref_fleet.barycenter);
				IOperation currentOperationTemplate = this.currentOperationTemplate;
				TIGameState tigameState = this.changeTrajectoryFleet;
				Trajectory trajectory = this.changeTrajectoryFleet.ref_fleet.trajectory;
				IEnumerable<TIGameState> possibleTargets = currentOperationTemplate.GetPossibleTargets(tigameState, (trajectory != null) ? trajectory.destination : null);
				this.targetSelectionTool.Open(possibleTargets, this.targetBase, null);
				this.actorState = this.changeTrajectoryFleet;
				this.operationType = OperationActorState.Fleet;
				this.OpenThrustProfileTool();
				this.targetNameObject.SetActive(false);
				flag = true;
			}
			this.changingInvalidTrajectory = true;
			if (flag)
			{
				this.InitTargetSelection(null);
			}
			if (flag)
			{
				this.targetSelectionTool.transform.localPosition = new Vector3(this.targetSelectionTool.transform.localPosition.x, 390f, this.targetSelectionTool.transform.localPosition.z);
			}
			ThrustProfileTool thrustProfileTool = this.thrustProfileTool;
			IMobileAsset maneuveringFleet = e.maneuveringFleet;
			TIGameState tigameState2;
			if ((tigameState2 = this.changeTrajectoryTargetFleet) == null)
			{
				Trajectory trajectory2 = e.maneuveringFleet.trajectory;
				tigameState2 = ((trajectory2 != null) ? trajectory2.destination : null);
			}
			thrustProfileTool.Open(maneuveringFleet, tigameState2, e.validTrajectories);
			this.changeTrajectoryConfirmButton.interactable = this.EligibleTransferChangeSelected();
			this.Show();
			this.operationMasterPanel.enabled = false;
		}

		// Token: 0x06005478 RID: 21624 RVA: 0x00263538 File Offset: 0x00261738
		public void OnConfirmTrajectoryChange()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.thrustProfileTool.isChangeTrajectory = false;
			TISpaceFleetState tispaceFleetState = this.thrustProfileTool.Actor as TISpaceFleetState;
			if (tispaceFleetState != null)
			{
				if (this.thrustProfileTool.CurrentTrajectory == null)
				{
					this.OnCancelTrajectoryChange();
					return;
				}
				tispaceFleetState.AssignTrajectory(this.thrustProfileTool.CurrentTrajectory);
				tispaceFleetState.destroyProposedTrajectories();
				tispaceFleetState.delayedTransferAbortNotification = null;
			}
			this.CleanupTrajectoryChange();
			TISpaceFleetState tispaceFleetState2 = this.thrustProfileTool.Actor as TISpaceFleetState;
			if (tispaceFleetState2 != null)
			{
				tispaceFleetState2.LaunchFleet(true);
			}
		}

		// Token: 0x06005479 RID: 21625 RVA: 0x002635C8 File Offset: 0x002617C8
		public void OnCancelTrajectoryChange()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			TISpaceFleetState tispaceFleetState = this.thrustProfileTool.Actor as TISpaceFleetState;
			if (tispaceFleetState != null)
			{
				if (tispaceFleetState.trajectory != null)
				{
					Trajectory trajectory = tispaceFleetState.trajectory;
					Trajectory trajectory2 = tispaceFleetState.trajectory;
					Trajectory trajectory3;
					if (trajectory2 == null)
					{
						trajectory3 = null;
					}
					else
					{
						Trajectory destinationFleetTrajectory = trajectory2.destinationFleetTrajectory;
						trajectory3 = ((destinationFleetTrajectory != null) ? destinationFleetTrajectory.ShallowCopy(tispaceFleetState) : null);
					}
					trajectory.nextTrajectory = trajectory3;
				}
				tispaceFleetState.destroyProposedTrajectories();
				tispaceFleetState.VerifyAssignedTransfer(false);
				if (tispaceFleetState.delayedTransferAbortNotification != null)
				{
					TINotificationQueueState.LogTrajectoryAborted(tispaceFleetState, tispaceFleetState.delayedTransferAbortNotification.cause, tispaceFleetState.delayedTransferAbortNotification.outcome, tispaceFleetState.delayedTransferAbortNotification.doomedFleet, tispaceFleetState.delayedTransferAbortNotification.collisionTarget);
					tispaceFleetState.delayedTransferAbortNotification = null;
				}
			}
			this.CleanupTrajectoryChange();
		}

		// Token: 0x0600547A RID: 21626 RVA: 0x00263680 File Offset: 0x00261880
		private void CleanupTrajectoryChange()
		{
			TIPromptQueueState.RemovePromptStatic(this.changeTrajectoryFaction, this.changeTrajectoryFleet, this.changeTrajectoryTargetFleet, "PromptChangeTrajectory", 0);
			this.changeTrajectoryFaction = null;
			this.changeTrajectoryFleet = null;
			this.changeTrajectoryTargetFleet = null;
			this.masterOperationCanvas.enabled = false;
			this.operationMasterPanel.enabled = false;
			this.changeTrajectoryCanvas.enabled = false;
			this.ShutdownTargetSelection();
			this.CloseThrustProfileTool();
			this.targetSelectionTool.Close();
			this.changingInvalidTrajectory = false;
		}

		// Token: 0x0600547B RID: 21627 RVA: 0x00263701 File Offset: 0x00261901
		private bool EligibleTransferChangeSelected()
		{
			return this.thrustProfileTool.CanReachTarget;
		}

		// Token: 0x0600547C RID: 21628 RVA: 0x00263714 File Offset: 0x00261914
		private void CheckAndRemoveArmyListener()
		{
			TIGameState tigameState = this.actorState;
			if (tigameState != null && tigameState.isArmyState)
			{
				GameControl.eventManager.RemoveListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.ArmyUpdated), this.actorState.ref_army.armyStatusUpdateEventName);
				GameControl.eventManager.RemoveListener<ArmyAssignedToFaction>(new EventManager.EventDelegate<ArmyAssignedToFaction>(this.OnArmyAssignedToFaction), null);
			}
		}

		// Token: 0x0600547D RID: 21629 RVA: 0x00263774 File Offset: 0x00261974
		private void ArmySelected(TIArmyState army)
		{
			if (!TIGameState.Valid(army))
			{
				return;
			}
			bool flag = GeneralControlsController.CurrentValidTarget(army);
			if (!this.changingInvalidTrajectory && army.faction == base.activePlayer && !flag)
			{
				this.DisableCurrentOperation(false);
				this.AddCanvasToStack();
				this.actorState = army;
				if (!this.armyGroup.Contains(army))
				{
					this.CloseMultiArmyPanel();
				}
				GameControl.eventManager.AddListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.ArmyUpdated), army.armyStatusUpdateEventName, army, true, false);
				GameControl.eventManager.AddListener<ArmyAssignedToFaction>(new EventManager.EventDelegate<ArmyAssignedToFaction>(this.OnArmyAssignedToFaction), null, army, true, false);
				if (base.canvasManager.GetActiveInfoPanel() == InfoPanel.None)
				{
					GeneralControlsController.SetUIOtherSelectedState(null);
				}
				this.targetBase = army.currentRegion;
				this.operationType = OperationActorState.Army;
				this.UpdateOperationControls();
				this.HideTutorials();
				this.armiesUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_OperationScreenCanvas_ArmyOperations, false, true);
				return;
			}
			if (GeneralControlsController.UIPlayerInTargetingMode && !flag)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
		}

		// Token: 0x0600547E RID: 21630 RVA: 0x00263874 File Offset: 0x00261A74
		private void NaturalSpaceObjectSelected(TINaturalSpaceObjectState spaceObject)
		{
			if (!this.changingInvalidTrajectory && !this.AttemptSetNewTargetBase(spaceObject) && !GeneralControlsController.CurrentValidTarget(spaceObject))
			{
				this.DisableCurrentOperation(false);
				this.AddCanvasToStack();
				this.actorState = base.activePlayer;
				this.operationType = OperationActorState.SpaceBody;
				this.targetBase = spaceObject;
				this.UpdateOperationControls();
				this.HideTutorials();
			}
			this.HideTutorials();
			this.spacebodyUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_OperationScreenCanvas_SpacebodyOperations, false, true);
			if (spaceObject.isEarth && this.iconsGridManager != null && this.iconsGridManager.size > 0)
			{
				using (IEnumerator<object> enumerator = this.iconsGridManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (OperationCanvasController.<>o__134.<>p__0 == null)
						{
							OperationCanvasController.<>o__134.<>p__0 = CallSite<Func<CallSite, object, OperationButtonController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OperationButtonController), typeof(OperationCanvasController)));
						}
						if (OperationCanvasController.<>o__134.<>p__0.Target(OperationCanvasController.<>o__134.<>p__0, enumerator.Current).operationType is LaunchSTOInterceptorsOperation)
						{
							this.launchExofighterTutorialController.HoldTutorial(CampaignMilestone.UITutorial_OperationsScreenCanvas_LaunchExofighters, false, true);
							break;
						}
					}
				}
			}
		}

		// Token: 0x0600547F RID: 21631 RVA: 0x002639AC File Offset: 0x00261BAC
		private void AddCanvasToStack()
		{
			if (!this.Visible())
			{
				GameControl.canvasStack.Show(base.gameObject);
				this.Show();
				this.operationMasterPanel.enabled = false;
				this.iconsPanel.SetActive(false);
				this.operationInfoPanel.SetActive(false);
				this.confirmPanel.SetActive(false);
			}
		}

		// Token: 0x06005480 RID: 21632 RVA: 0x00263A07 File Offset: 0x00261C07
		private void UpdateOperationControls()
		{
			this.operationMasterPanel.enabled = true;
			this.UpdateOperationBar();
		}

		// Token: 0x06005481 RID: 21633 RVA: 0x00263A1C File Offset: 0x00261C1C
		private OperationButtonController FindOperationButton(TIOperationTemplate operation)
		{
			using (IEnumerator<object> enumerator = this.iconsGridManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__137.<>p__0 == null)
					{
						OperationCanvasController.<>o__137.<>p__0 = CallSite<Func<CallSite, object, OperationButtonController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OperationButtonController), typeof(OperationCanvasController)));
					}
					OperationButtonController operationButtonController = OperationCanvasController.<>o__137.<>p__0.Target(OperationCanvasController.<>o__137.<>p__0, enumerator.Current);
					if (operationButtonController.operationType.GetTemplate().dataName == operation.dataName)
					{
						return operationButtonController;
					}
				}
			}
			return null;
		}

		// Token: 0x06005482 RID: 21634 RVA: 0x00263AC8 File Offset: 0x00261CC8
		private void HideTutorials()
		{
			this.armiesUITutorialController.HideTutorial();
			this.fleetOperationsUITutorialController.HideTutorial();
			this.fleetTransferTutorialController.HideTutorial();
			this.spacebodyUITutorialController.HideTutorial();
			this.launchExofighterTutorialController.HideTutorial();
		}

		// Token: 0x06005483 RID: 21635 RVA: 0x00263B04 File Offset: 0x00261D04
		public void Tutorial_HighlightLaunchExofighterOp()
		{
			GameObject gameObject = null;
			bool flag = false;
			if (this.iconsGridManager != null && this.iconsGridManager.size > 0)
			{
				using (IEnumerator<object> enumerator = this.iconsGridManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (OperationCanvasController.<>o__139.<>p__0 == null)
						{
							OperationCanvasController.<>o__139.<>p__0 = CallSite<Func<CallSite, object, OperationButtonController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OperationButtonController), typeof(OperationCanvasController)));
						}
						OperationButtonController operationButtonController = OperationCanvasController.<>o__139.<>p__0.Target(OperationCanvasController.<>o__139.<>p__0, enumerator.Current);
						if (operationButtonController.operationType is LaunchSTOInterceptorsOperation)
						{
							gameObject = operationButtonController.gameObject;
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				return;
			}
			if (gameObject != null)
			{
				RectTransform rectTransform = this.launchExofighterHighlightDummy.transform as RectTransform;
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

		// Token: 0x06005484 RID: 21636 RVA: 0x00263C28 File Offset: 0x00261E28
		public void OnClickCloseActionPanel(bool fullShutdown)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseActionPanel(fullShutdown);
		}

		// Token: 0x06005485 RID: 21637 RVA: 0x00263C40 File Offset: 0x00261E40
		public void CloseActionPanel(bool fullShutdown)
		{
			if (this.operationMasterPanel != null && this.operationMasterPanel.enabled)
			{
				this.HideTutorials();
				this.ShutdownTargetSelection();
				this.activeButton = null;
				this.operationInfoPanel.SetActive(false);
				if (!TIGameState.Valid(this.actorState) || this.actorState.archived || fullShutdown)
				{
					this.iconsPanel.SetActive(false);
					this.operationMasterPanel.enabled = false;
					return;
				}
				this.UpdateOperationBar();
			}
		}

		// Token: 0x06005486 RID: 21638 RVA: 0x00263CC8 File Offset: 0x00261EC8
		public void OnOperationSelected(OperationButtonController button, TIGameState forceTarget = null)
		{
			if (button != this.activeButton)
			{
				this.ShutdownTargetSelection();
				this.activeButton = button;
				this.operationInfoPanel.SetActive(true);
				this.SetOperationInfo(this.activeButton.operationType);
				if (this.activeButton.operationType is DeployArmyOperation)
				{
					this.AddArmyToMultiSelectGroup(this.actorState.ref_army);
					if (this.armyGroup.Count > 1)
					{
						this.OpenMultiArmyPanel();
					}
				}
				else
				{
					this.CloseMultiArmyPanel();
				}
				this.InitTargetSelection(forceTarget);
			}
		}

		// Token: 0x06005487 RID: 21639 RVA: 0x00263D54 File Offset: 0x00261F54
		public void OnOperationSelected(TIOperationTemplate operation, TIGameState forceTarget = null)
		{
			OperationButtonController operationButtonController = this.FindOperationButton(operation);
			this.OnOperationSelected(operationButtonController, forceTarget);
		}

		// Token: 0x06005488 RID: 21640 RVA: 0x00263D74 File Offset: 0x00261F74
		private void UpdateOperationBar()
		{
			if (!TIGameState.Valid(this.actorState))
			{
				this.DisableCurrentOperation(false);
				return;
			}
			if (GameStateManager.PromptQueue().HasAnyPromptofType("PromptAddressNarrativeEvent", false, false) || (GameControl.spaceCombat.HasActiveState() && GameControl.spaceCombat.combatState.IncludesFaction(GameControl.control.activePlayer)))
			{
				return;
			}
			List<IOperation> list = new List<IOperation>();
			switch (this.operationType)
			{
			case OperationActorState.Army:
				if (!TIGameState.Valid(this.actorState.ref_army))
				{
					this.DisableCurrentOperation(false);
					return;
				}
				list = this.actorState.ref_army.VisibleOperationList(null);
				break;
			case OperationActorState.SpaceBody:
				if (this.actorState.ref_faction != null)
				{
					TINaturalSpaceObjectState tinaturalSpaceObjectState = this.targetBase as TINaturalSpaceObjectState;
					if (tinaturalSpaceObjectState != null)
					{
						list = this.actorState.ref_faction.VisibleOperationList(tinaturalSpaceObjectState);
						break;
					}
				}
				this.DisableCurrentOperation(false);
				return;
			case OperationActorState.Fleet:
				if (!TIGameState.Valid(this.actorState.ref_fleet))
				{
					this.DisableCurrentOperation(false);
					return;
				}
				list = this.actorState.ref_fleet.VisibleOperationList(null);
				break;
			}
			if (list.Count > 18)
			{
				list = (from o in list
					orderby o.ActorCanPerformOperation(this.actorState, null) descending, o.SortOrder()
					select o).ToList<IOperation>();
			}
			else
			{
				list = list.OrderBy<IOperation, int>((IOperation o) => o.SortOrder()).ToList<IOperation>();
			}
			if (list.Count > 0)
			{
				this.iconsPanel.SetActive(true);
				this.iconsGridManager.SetListSize<OperationButtonController>(list.Count, false, false);
				int num = 0;
				IOperation operation;
				if (this.activeButton != null)
				{
					operation = this.activeButton.operationType;
				}
				else
				{
					operation = null;
				}
				IOperationCapableState operationCapableState = this.actorState as IOperationCapableState;
				TIGameState targetBase = this.targetBase;
				List<IOperation> list2 = operationCapableState.AvailableOperationList((targetBase != null) ? targetBase.ref_naturalSpaceObject : null);
				using (IEnumerator<object> enumerator = this.iconsGridManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (OperationCanvasController.<>o__144.<>p__0 == null)
						{
							OperationCanvasController.<>o__144.<>p__0 = CallSite<Func<CallSite, object, OperationButtonController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OperationButtonController), typeof(OperationCanvasController)));
						}
						OperationButtonController operationButtonController = OperationCanvasController.<>o__144.<>p__0.Target(OperationCanvasController.<>o__144.<>p__0, enumerator.Current);
						operationButtonController.Init(this);
						IOperation operation2 = list[num];
						operationButtonController.SetOperationData(list[num], this.actorState, list2.Contains(operation2) && operation2.ActorCanPerformOperation(this.actorState, this.targetBase), this.targetBase);
						if (this.activeButton != null && operation == operation2)
						{
							this.activeButton = operationButtonController;
						}
						num++;
					}
					return;
				}
			}
			this.iconsPanel.SetActive(false);
		}

		// Token: 0x06005489 RID: 21641 RVA: 0x00264074 File Offset: 0x00262274
		public void OnOperationConfirmHover()
		{
			if (this.confirmButton.interactable)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverButton", false, false);
			}
		}

		// Token: 0x0600548A RID: 21642 RVA: 0x00264090 File Offset: 0x00262290
		private void InitTargetSelection(TIGameState forceTarget = null)
		{
			this.currentTarget = null;
			if ((this.activeButton == null || this.activeButton.operationType == null) && !this.changingInvalidTrajectory)
			{
				this.confirmButton.interactable = false;
				this.UpdateTargetData();
				return;
			}
			IOperation currentOperationTemplate = this.currentOperationTemplate;
			Type targetingMethod = currentOperationTemplate.GetTargetingMethod();
			if (targetingMethod == null)
			{
				return;
			}
			(base.canvasManager.NationInfo as NationInfoController).CloseNuclearWeaponsPanel();
			GameControl.eventManager.AddListener<OperationTargettedEvent>(new EventManager.EventDelegate<OperationTargettedEvent>(this.NewOperationTarget), null, null, true, false);
			this.currentTargeting = Activator.CreateInstance(targetingMethod) as TIOperationTargeting;
			this.currentTargeting.Init(currentOperationTemplate, this.actorState, this.targetBase);
			this.currentTargeting.Activate(forceTarget);
			this.currentTarget = this.currentTargeting.GetTargetted();
			this.selectingTarget = true;
			this.confirmPanel.SetActive(true);
			switch (this.currentTargeting.UIType())
			{
			case OperationTargetingUIType.Standard:
				this.targetDropdownObject.SetActive(false);
				this.targetNameObject.SetActive(true);
				return;
			case OperationTargetingUIType.Dropdown:
			{
				IList<TIGameState> possibleTargets = this.currentOperationTemplate.GetPossibleTargets(this.actorState, this.targetBase);
				if (possibleTargets.Count > 1)
				{
					bool flag = false;
					Dictionary<TIGameState, float> dictionary = new Dictionary<TIGameState, float>();
					IContestedOperation contestedOperation = this.currentOperationTemplate as IContestedOperation;
					if (contestedOperation != null)
					{
						flag = true;
						foreach (TIGameState tigameState in possibleTargets)
						{
							dictionary.Add(tigameState, contestedOperation.GetSuccessChance(this.actorState, tigameState));
						}
					}
					this.FillOutTargetDropdown(possibleTargets, this.currentTarget ?? possibleTargets[0], flag, dictionary);
					this.targetDropdownObject.SetActive(true);
					this.targetNameObject.SetActive(false);
					return;
				}
				this.targetDropdownObject.SetActive(false);
				this.targetNameObject.SetActive(true);
				return;
			}
			case OperationTargetingUIType.TwoStage:
			{
				this.targetDropdownObject.SetActive(false);
				this.targetNameObject.SetActive(true);
				IEnumerable<TIGameState> possibleTargets2 = this.currentOperationTemplate.GetPossibleTargets(this.actorState, this.targetBase);
				this.targetSelectionTool.Open(possibleTargets2, this.targetBase, currentOperationTemplate);
				return;
			}
			case OperationTargetingUIType.Transfer:
			{
				this.targetDropdownObject.SetActive(false);
				this.targetSelectionTool.Operation = currentOperationTemplate;
				this.NewOperationTargetBase(this.actorState.ref_fleet.barycenter);
				IEnumerable<TIGameState> possibleTargets3 = this.currentOperationTemplate.GetPossibleTargets(this.actorState, this.targetBase);
				this.targetSelectionTool.Open(possibleTargets3, this.targetBase, currentOperationTemplate);
				this.OpenThrustProfileTool();
				this.targetNameObject.SetActive(false);
				return;
			}
			case OperationTargetingUIType.ShipList:
			case OperationTargetingUIType.RefuelManager:
			case OperationTargetingUIType.TransferOfficerManager:
			case OperationTargetingUIType.Self:
				this.targetDropdownObject.SetActive(false);
				this.targetNameObject.SetActive(false);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600548B RID: 21643 RVA: 0x00264374 File Offset: 0x00262574
		public bool AttemptSetNewTargetBase(TINaturalSpaceObjectState newTargetBase)
		{
			if (this.selectingTarget && this.currentTargeting is TIOperationTargeting_FleetDestination)
			{
				List<TIGameState> possibleTargets = this.currentOperationTemplate.GetPossibleTargets(this.actorState, null);
				List<TIOrbitState> orbits = newTargetBase.orbits;
				List<TISpaceFleetState> list = (from x in GameStateManager.IterateByClass<TISpaceFleetState>(false)
					where x.inTransfer && x.GetSphereOfInfluence(true) == newTargetBase
					select x).ToList<TISpaceFleetState>();
				if (possibleTargets.Intersect<TIGameState>(orbits).Any<TIGameState>() || possibleTargets.Intersect<TIGameState>(list).Any<TIGameState>())
				{
					this.NewOperationTargetBase(newTargetBase);
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600548C RID: 21644 RVA: 0x0026440A File Offset: 0x0026260A
		private void NewOperationTargetBase(TIGameState newTargetBase)
		{
			this.targetBase = newTargetBase;
			if (newTargetBase != null && newTargetBase.isNaturalSpaceObjectState)
			{
				this.targetSelectionTool.Filter = this.targetBase;
			}
		}

		// Token: 0x0600548D RID: 21645 RVA: 0x00264438 File Offset: 0x00262638
		private void NewOperationTarget(OperationTargettedEvent e)
		{
			if (!(this.ProspectiveTarget != null) || !(this.actorState != null) || !this.actorState.isArmyState || (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
			{
				this.NewOperationTarget(e.target);
				return;
			}
			TIArmyState ref_army = this.actorState.ref_army;
			TIRegionState tiregionState = ref_army.currentRegion;
			if (this.QueuedTargets.Count > 0)
			{
				tiregionState = this.QueuedTargets.Last<TIGameState>().ref_region;
			}
			TIRegionState ref_region = e.target.ref_region;
			if (tiregionState == ref_region)
			{
				return;
			}
			bool flag = tiregionState.ConnectedRegions.Contains(ref_region) && TIArmyState.IsTraversible(tiregionState, ref_region, ref_army);
			List<TIRegionState> list = new List<TIRegionState>();
			if (flag)
			{
				list.Add(ref_region);
			}
			else
			{
				List<TIRegionState> journey = TIArmyState.GetJourney(tiregionState, ref_region, ref_army);
				if (journey == null || journey.Count < 2)
				{
					this.CloseActionPanel(true);
					return;
				}
				list.AddRange(journey.GetRange(1, journey.Count - 1));
			}
			if (this.QueuedTargets.Count == 0)
			{
				this.currentTarget = list.First<TIRegionState>();
			}
			this.QueuedTargets.AddRange(list);
			this.UpdateDurationLabel();
			this.UpdateTargetData();
			GameControl.eventManager.TriggerEvent(new ArmyPathChanged(this.actorState.ref_army), null, new object[] { this.actorState.ref_army.currentRegion });
		}

		// Token: 0x0600548E RID: 21646 RVA: 0x002645B0 File Offset: 0x002627B0
		private void NewOperationTarget(TIGameState newTarget)
		{
			this.QueuedTargets.Clear();
			if (this.activeButton == null && !this.changingInvalidTrajectory)
			{
				this.CloseActionPanel(false);
				Log.Warn("New Operation Target got passed null activeButton with " + newTarget.displayName + " as target", Array.Empty<object>());
				return;
			}
			if ((this.currentOperationTemplate is DeployArmyOperation || this.currentOperationTemplate is DeployArmiesOperation) && this.actorState.ref_army.currentRegion != newTarget)
			{
				TIArmyState ref_army = this.actorState.ref_army;
				TIRegionState currentRegion = ref_army.currentRegion;
				TIRegionState ref_region = newTarget.ref_region;
				this.currentTargetArmyMoveFinalDestination = ref_region;
				if (currentRegion.ConnectedRegions.Contains(ref_region))
				{
					TIArmyState.IsTraversible(currentRegion, ref_region, ref_army);
				}
				if (false)
				{
					this.currentTarget = ref_region;
					this.QueuedTargets.Add(this.currentTarget);
				}
				else
				{
					List<TIRegionState> journey_AvoidEnemyRegions = ref_army.GetJourney_AvoidEnemyRegions(currentRegion, ref_region);
					this.QueuedTargets.AddRange(journey_AvoidEnemyRegions.GetRange(1, journey_AvoidEnemyRegions.Count - 1));
					this.currentTarget = this.QueuedTargets.First<TIGameState>();
				}
				List<TIArmyState> selectedArmies = this.GetSelectedArmies();
				if (selectedArmies.Count > 0)
				{
					this.prospectiveQueuedTargetsDictionary.Clear();
					foreach (TIArmyState tiarmyState in selectedArmies)
					{
						if (!(tiarmyState == ref_army) && !(tiarmyState.currentRegion == newTarget))
						{
							TIArmyState ref_army2 = this.actorState.ref_army;
							TIRegionState currentRegion2 = tiarmyState.currentRegion;
							TIRegionState ref_region2 = newTarget.ref_region;
							this.currentTargetArmyMoveFinalDestination = ref_region2;
							if (currentRegion.ConnectedRegions.Contains(ref_region2))
							{
								TIArmyState.IsTraversible(currentRegion2, ref_region2, ref_army2);
							}
							if (false)
							{
								this.currentTarget = ref_region2;
								this.prospectiveQueuedTargetsDictionary.Add(tiarmyState, new List<TIRegionState> { ref_region2 });
							}
							else
							{
								List<TIRegionState> journey_AvoidEnemyRegions2 = ref_army2.GetJourney_AvoidEnemyRegions(currentRegion2, ref_region2);
								if (journey_AvoidEnemyRegions2 != null)
								{
									this.prospectiveQueuedTargetsDictionary.Add(tiarmyState, journey_AvoidEnemyRegions2.GetRange(1, journey_AvoidEnemyRegions2.Count - 1));
								}
							}
						}
					}
				}
				this.UpdateMultiSelectedArmies();
				GameControl.eventManager.TriggerEvent(new ArmyPathChanged(this.actorState.ref_army), null, new object[] { this.actorState.ref_army.currentRegion });
			}
			else
			{
				this.currentTarget = newTarget;
			}
			bool flag = this.currentTarget != null;
			this.confirmButton.interactable = flag;
			if (!this.changingInvalidTrajectory)
			{
				if (flag)
				{
					this.resourceCostOptions = this.activeButton.operationType.ResourceCostOptions(base.activePlayer, this.currentTarget, this.actorState, true);
				}
				if (flag && this.activeButton.operationType.HasResourceCost() && (this.resourceCostOptions == null || this.resourceCostOptions.Count == 0))
				{
					this.currentTarget = null;
					this.confirmButton.interactable = false;
					this.paymentDropdownObject.SetActive(false);
					this.paymentDropdown.enabled = false;
					this.selectedResourceCostOption = null;
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
					this.UpdateTargetData();
					return;
				}
				if (flag && this.resourceCostOptions != null && this.resourceCostOptions.Count > 0)
				{
					this.durationReportObject.SetActive(false);
					this.paymentDropdownObject.SetActive(true);
					this.paymentDropdown.enabled = true;
					this.selectedResourceCostOption = this.resourceCostOptions[0];
				}
				else
				{
					this.paymentDropdownObject.SetActive(false);
					this.paymentDropdown.enabled = false;
					this.selectedResourceCostOption = null;
					if (flag && !this.currentOperationTemplate.RequiresThrustProfile())
					{
						this.UpdateDurationLabel();
					}
					else
					{
						this.durationReportObject.SetActive(false);
					}
				}
				if (!newTarget.isHabSiteState && !newTarget.isOrbitState && !this.currentOperationTemplate.RequiresThrustProfile())
				{
					this.targetSelectionTool.Close();
				}
				if (this.currentOperationTemplate.RequiresThrustProfile())
				{
					this.confirmButton.interactable = this.thrustProfileTool.CanReachTarget;
				}
			}
			if (flag)
			{
				this.UpdateTargetData();
			}
		}

		// Token: 0x0600548F RID: 21647 RVA: 0x002649D4 File Offset: 0x00262BD4
		private void UpdateDurationLabel()
		{
			float num = -1f;
			if (this.QueuedTargets.Count > 0)
			{
				if (this.activeButton.operationType is DeployArmyOperation || this.activeButton.operationType is DeployArmiesOperation)
				{
					TIArmyState army = this.actorState.ref_army;
					num = Enumerable.Range(0, this.QueuedTargets.Count).Sum<int>(delegate(int x)
					{
						TIRegionState tiregionState = ((x == 0) ? army.currentRegion : this.QueuedTargets[x - 1].ref_region);
						TIRegionState ref_region = this.QueuedTargets[x].ref_region;
						return TIArmyState.GetDeploymentToAdjacentRegionDuration_Days(tiregionState, ref_region, army);
					});
				}
			}
			else
			{
				num = this.activeButton.operationType.GetDuration_days(this.actorState, this.currentTarget, this.thrustProfileTool.CurrentTrajectory);
			}
			if (num >= 1f)
			{
				this.durationReportObject.SetActive(true);
				this.durationText.SetText(Loc.T("UI.Operations.Duration_days", new object[] { num.ToString(TIUtilities.DecimalPlaces((double)num, 2, 0)) }));
				return;
			}
			if (num >= 0.041666668f)
			{
				this.durationReportObject.SetActive(true);
				this.durationText.SetText(Loc.T("UI.Operations.Duration_hours", new object[] { (num * 24f).ToString(TIUtilities.DecimalPlaces((double)(num * 24f), 2, 0)) }));
				return;
			}
			if (num > 0f)
			{
				this.durationReportObject.SetActive(true);
				this.durationText.SetText(Loc.T("UI.Operations.Duration_minutes", new object[] { (num * 24f * 60f).ToString(TIUtilities.DecimalPlaces((double)(num * 24f * 60f), 2, 0)) }));
				return;
			}
			this.durationReportObject.SetActive(false);
		}

		// Token: 0x06005490 RID: 21648 RVA: 0x00264B84 File Offset: 0x00262D84
		private void UpdateTargetData()
		{
			if (this.currentTarget == this.actorState)
			{
				this.targetNameObject.SetActive(false);
				this.targetDropdownObject.SetActive(false);
			}
			else
			{
				TIGameState tigameState = this.currentTarget;
				List<TIGameState> queuedTargets = this.QueuedTargets;
				if (queuedTargets != null && queuedTargets.Count > 1)
				{
					tigameState = this.QueuedTargets.Last<TIGameState>();
				}
				if (this.targetNameObject.activeInHierarchy)
				{
					if (tigameState == null)
					{
						this.targetDisplayName.SetText(Loc.T("UI.Operations.NoTarget"));
					}
					else
					{
						string text = tigameState.GetDisplayName(base.activePlayer);
						if (this.currentOperationTemplate.WarnTarget(tigameState))
						{
							text = new StringBuilder(text).Append(TIGlobalConfig.globalConfig.warningInlineSpritePath).ToString();
						}
						IContestedOperation contestedOperation = this.currentOperationTemplate as IContestedOperation;
						if (contestedOperation != null)
						{
							this.targetDisplayName.SetText(Loc.T("UI.Operations.TargetWithChance", new object[]
							{
								text,
								contestedOperation.GetSuccessChance(this.actorState, tigameState).ToPercent("P0")
							}));
						}
						else
						{
							this.targetDisplayName.SetText(Loc.T("UI.Operations.Target", new object[] { text }));
						}
					}
				}
				else if (this.targetDropdownObject.activeInHierarchy && tigameState != null && this.reverseTargetOptionData.ContainsKey(tigameState))
				{
					this.targetDropdown.value = this.reverseTargetOptionData[tigameState];
				}
				if (this.currentOperationTemplate.RequiresThrustProfile())
				{
					if (tigameState != null)
					{
						this.thrustProfileTool.Target = this.currentTarget;
						this.changeTrajectoryConfirmButton.interactable = this.EligibleTransferChangeSelected();
					}
					else
					{
						this.CloseThrustProfileTool();
					}
				}
			}
			if (this.currentTarget != null && this.currentOperationTemplate.HasResourceCost() && this.resourceCostOptions != null && this.resourceCostOptions.Count > 0)
			{
				this.paymentDropdown.ClearOptions();
				foreach (TIResourcesCost tiresourcesCost in this.resourceCostOptions)
				{
					TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
					{
						text = tiresourcesCost.GetString("N1", false, true, false, 7, false, false, null, false, FactionResource.None)
					};
					this.paymentDropdown.options.Add(optionData);
				}
				if (this.selectedResourceCostOption == null)
				{
					this.selectedResourceCostOption = this.resourceCostOptions[this.paymentDropdown.value];
				}
				this.paymentDropdown.captionText.SetText(this.selectedResourceCostOption.GetString("N1", false, true, false, 7, false, false, null, false, FactionResource.None));
			}
		}

		// Token: 0x06005491 RID: 21649 RVA: 0x00264E44 File Offset: 0x00263044
		public void OnCostDropdownChanged()
		{
			this.selectedResourceCostOption = this.resourceCostOptions[this.paymentDropdown.value];
		}

		// Token: 0x06005492 RID: 21650 RVA: 0x00264E64 File Offset: 0x00263064
		public void OnTargetSelectionToolElementClicked(TIGameState targetState)
		{
			if (this.currentOperationTemplate == null)
			{
				Debug.LogError("Operation Canvas Controller Lost active button / currentOperationTemplate");
				this.ShutdownTargetSelection();
				return;
			}
			TIGameState tigameState = this.currentTarget;
			this.currentTarget = targetState;
			if (this.currentTarget != tigameState)
			{
				if (this.currentTarget.isNaturalSpaceObjectState)
				{
					this.currentTarget = this.targetSelectionTool.GetArbitraryTarget();
				}
				if (this.currentOperationTemplate.RequiresThrustProfile())
				{
					this.UpdateTargetData();
					return;
				}
				this.UpdateTargetData();
				TIGameState tigameState2 = this.currentTarget;
				if (tigameState2 != null && tigameState2.isOrbitState)
				{
					GameControl.eventManager.TriggerEvent(new OrbitSelectedEvent(this.currentTarget.ref_orbit), null, Array.Empty<object>());
					return;
				}
				TIGameState tigameState3 = this.currentTarget;
				if (tigameState3 != null && tigameState3.isHabSiteState)
				{
					GameControl.eventManager.TriggerEvent(new HabSiteSelectedEvent(this.currentTarget.ref_habSite), null, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06005493 RID: 21651 RVA: 0x00264F4A File Offset: 0x0026314A
		private void OnNavigationButtonClicked(TIGameState gameState)
		{
			this.NewOperationTargetBase(gameState);
		}

		// Token: 0x06005494 RID: 21652 RVA: 0x00264F54 File Offset: 0x00263154
		private void FillOutTargetDropdown(IList<TIGameState> targets, TIGameState initialTarget, bool contested, Dictionary<TIGameState, float> successChances)
		{
			this.targetDropdown.ClearOptions();
			this.targetOptionData = new Dictionary<int, TIGameState>();
			this.reverseTargetOptionData = new Dictionary<TIGameState, int>();
			for (int i = 0; i < targets.Count; i++)
			{
				if (targets[i] != null)
				{
					bool flag = false;
					if (targets[i].isHabState && !contested)
					{
						flag = true;
					}
					string text = targets[i].GetDisplayName(base.activePlayer);
					if (this.currentOperationTemplate.WarnTarget(targets[i]))
					{
						text = new StringBuilder(text).Append(TIGlobalConfig.globalConfig.warningInlineSpritePath).ToString();
					}
					TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
					{
						text = (contested ? Loc.T("UI.Operations.OperationTabbed", new object[]
						{
							text,
							successChances[targets[i]].ToPercent("P0")
						}) : text),
						image = TIUtilities.GetStateIcon(base.activePlayer, targets[i], flag)
					};
					this.targetDropdown.options.Add(optionData);
					this.targetOptionData.Add(i, targets[i]);
					this.reverseTargetOptionData.Add(targets[i], i);
				}
			}
			if (initialTarget != null && targets.Contains(initialTarget))
			{
				this.targetDropdown.value = this.reverseTargetOptionData[initialTarget];
				this.targetDropdown.captionText.text = (contested ? Loc.T("UI.Operations.OperationTabbed", new object[]
				{
					new StringBuilder(initialTarget.GetDisplayName(base.activePlayer)).Append("      ").ToString(),
					successChances[initialTarget].ToPercent("P0")
				}) : initialTarget.GetDisplayName(base.activePlayer));
				this.targetDropdown.captionText.horizontalAlignment = (contested ? HorizontalAlignmentOptions.Left : HorizontalAlignmentOptions.Center);
			}
		}

		// Token: 0x06005495 RID: 21653 RVA: 0x00265148 File Offset: 0x00263348
		public void OnTargetDropDownChanged()
		{
			if (this.currentTarget == this.targetOptionData[this.targetDropdown.value])
			{
				return;
			}
			this.currentTarget = this.targetOptionData[this.targetDropdown.value];
			this.currentTargeting.ForceTarget(this.targetOptionData[this.targetDropdown.value]);
		}

		// Token: 0x06005496 RID: 21654 RVA: 0x002651B8 File Offset: 0x002633B8
		private void ShutdownTargetSelection()
		{
			if (this.selectingTarget)
			{
				this.selectingTarget = false;
				this.currentTargeting.Shutdown();
				this.confirmPanel.SetActive(false);
				this.hollowNamePanel.SetActive(true);
				GameControl.eventManager.RemoveListener<OperationTargettedEvent>(new EventManager.EventDelegate<OperationTargettedEvent>(this.NewOperationTarget), null);
				this.CloseThrustProfileTool();
				this.CloseSplitFleetPanel();
				this.OnCloseSharePropellant();
				this.CloseOfficerTransferCanvas();
				this.CloseMultiArmyPanel();
				this.maximizeButtonGameObject.SetActive(false);
				this.targetSelectionTool.Close();
				this.UpdateOperationBar();
			}
		}

		// Token: 0x06005497 RID: 21655 RVA: 0x0026524C File Offset: 0x0026344C
		private void OpenThrustProfileTool()
		{
			this.thrustProfileTool.Open(this.actorState.ref_fleet, TIGameState.Valid(this.currentTarget) ? this.currentTarget : null, null);
			this.HideTutorials();
			this.fleetTransferTutorialController.HoldTutorial(CampaignMilestone.UITutorial_OperationScreenCanvas_FleetTransfer, false, true);
			TIPromptQueueState.AddPromptStatic(GameControl.control.activePlayer, null, null, "PromptSelectTrajectory", 0);
		}

		// Token: 0x06005498 RID: 21656 RVA: 0x002652B5 File Offset: 0x002634B5
		private void CloseThrustProfileTool()
		{
			this.thrustProfileTool.Close();
			TIPromptQueueState.RemovePromptStatic(GameControl.control.activePlayer, null, null, "PromptSelectTrajectory", 0);
		}

		// Token: 0x06005499 RID: 21657 RVA: 0x002652DC File Offset: 0x002634DC
		public void OnConfirmOperation()
		{
			IOperation operation = this.currentOperationTemplate;
			if (operation == null)
			{
				this.CloseActionPanel(this.actorState == null);
				this.UpdateOperationControls();
				return;
			}
			bool flag = operation is DeployArmyOperation || operation is DeployArmiesOperation;
			List<TIGameState> list = new List<TIGameState> { this.actorState };
			if (operation is DeployArmiesOperation)
			{
				list = (from x in DeployArmiesOperation.GetEligibleArmies(this.actorState.ref_army)
					select (x)).ToList<TIGameState>();
			}
			if (operation is DeployArmyOperation)
			{
				List<TIArmyState> selectedArmies = this.GetSelectedArmies();
				if (selectedArmies != null && selectedArmies.Count > 0)
				{
					list = selectedArmies.Select<TIArmyState, TIGameState>((TIArmyState x) => x).ToList<TIGameState>();
				}
			}
			switch (this.operationType)
			{
			case OperationActorState.Army:
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmArmyOperation", false, false);
				break;
			case OperationActorState.SpaceBody:
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmSpaceBodyOperation", false, false);
				break;
			case OperationActorState.Fleet:
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmFleetOperation", false, false);
				break;
			default:
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				break;
			}
			Player playerControl = base.activePlayer.playerControl;
			if (operation is SplitFleetOperation)
			{
				playerControl.StartAction(new SplitFleetOperationAction(this.actorState.ref_fleet, this.newFleetShips, null));
				this.CloseSplitFleetPanel();
			}
			else if (operation is ScuttleShipsOperation)
			{
				TIResourcesCost tiresourcesCost = new TIResourcesCost();
				foreach (TISpaceShipState tispaceShipState in this.newFleetShips)
				{
					tiresourcesCost.SumCosts_NoDuration(tispaceShipState.ScuttleCost());
				}
				if (tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
				{
					playerControl.StartAction(new ScuttleShipsOperationAction(this.actorState.ref_fleet, this.newFleetShips));
					this.CloseSplitFleetPanel();
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
			}
			else if (operation is InterfleetRefuelOperation)
			{
				playerControl.StartAction(new BeginInterfleetRefuelOperationAction(this.actorState.ref_fleet, this.propellantSharingEvents));
				this.OnCloseSharePropellant();
			}
			else if (operation is TransferOfficersOperation)
			{
				if (this.StartTransferOfficersOperation())
				{
					this.CloseOfficerTransferCanvas();
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
			}
			else
			{
				if (operation is TransferOperation)
				{
					if (this.actorState.ref_orbit == this.currentTarget)
					{
						(this.actorState as TISpaceFleetState).AbortTransfer(-1, null, false);
						this.CloseActionPanel(false);
						return;
					}
					this.thrustProfileTool.CurrentTrajectory.SetResupplyPlan(this.thrustProfileTool.thrustPanelRepairandResupplyToggle.isOn);
					TIGameState barycenter = this.actorState.ref_fleet.barycenter;
					TISpaceGameState destination = this.thrustProfileTool.CurrentTrajectory.destination;
					if (barycenter != (((destination != null) ? destination.barycenter.GetSunOrbitingRelatedObject : null) ?? this.actorState.ref_fleet.barycenter))
					{
						TIFactionState activePlayer = base.activePlayer;
						if (activePlayer != null)
						{
							activePlayer.UnlockAchievement("firstTransfer");
						}
					}
				}
				else if (operation is BombardOperation)
				{
					TIFactionState activePlayer2 = base.activePlayer;
					if (activePlayer2 != null)
					{
						activePlayer2.UnlockAchievement("bombardment");
					}
				}
				else if (flag)
				{
					foreach (TIArmyState tiarmyState in list.Select<TIGameState, TIArmyState>((TIGameState x) => x.ref_army))
					{
						playerControl.StartAction(new ClearArmyDestinationQueueAction(tiarmyState));
					}
					if (operation is DeployArmyOperation)
					{
						operation = new DeployArmyOperation_OpenTarget(false);
					}
					else
					{
						operation = new DeployArmiesOperation(false);
					}
				}
				TIGameState tigameState = this.currentTarget;
				if (this.QueuedTargets.Count > 0)
				{
					tigameState = this.QueuedTargets[0];
					this.QueuedTargets.RemoveAt(0);
				}
				if (tigameState == null)
				{
					this.CloseActionPanel(this.actorState == null);
					this.UpdateOperationControls();
					return;
				}
				if (operation is DeployArmyOperation)
				{
					foreach (TIGameState tigameState2 in list)
					{
						if (!(this.actorState == tigameState2) && this.prospectiveQueuedTargetsDictionary.ContainsKey(tigameState2.ref_army))
						{
							TIRegionState tiregionState = this.prospectiveQueuedTargetsDictionary[tigameState2.ref_army][0];
							playerControl.StartAction(new ConfirmOperationAction(tigameState2, tiregionState, operation, this.selectedResourceCostOption, this.thrustProfileTool.CurrentTrajectory));
						}
					}
				}
				playerControl.StartAction(new ConfirmOperationAction(this.actorState, tigameState, operation, this.selectedResourceCostOption, this.thrustProfileTool.CurrentTrajectory));
				if (this.QueuedTargets.Count > 0)
				{
					if (flag)
					{
						foreach (TIArmyState tiarmyState2 in list.Select<TIGameState, TIArmyState>((TIGameState x) => x.ref_army))
						{
							if (tiarmyState2 == this.actorState || operation is DeployArmiesOperation)
							{
								using (IEnumerator<TIRegionState> enumerator4 = this.QueuedTargets.Select<TIGameState, TIRegionState>((TIGameState x) => x.ref_region).GetEnumerator())
								{
									while (enumerator4.MoveNext())
									{
										TIRegionState tiregionState2 = enumerator4.Current;
										if (!tiarmyState2.ReachableRegions.Contains(tiregionState2))
										{
											break;
										}
										playerControl.StartAction(new QueueArmyDestinationAction(tiarmyState2, tiregionState2));
									}
									continue;
								}
							}
							if (this.prospectiveQueuedTargetsDictionary.ContainsKey(tiarmyState2) && this.prospectiveQueuedTargetsDictionary[tiarmyState2].Count > 1)
							{
								for (int i = 1; i < this.prospectiveQueuedTargetsDictionary[tiarmyState2].Count; i++)
								{
									TIRegionState tiregionState3 = this.prospectiveQueuedTargetsDictionary[tiarmyState2][i];
									if (!tiarmyState2.ReachableRegions.Contains(tiregionState3))
									{
										break;
									}
									playerControl.StartAction(new QueueArmyDestinationAction(tiarmyState2, tiregionState3));
								}
							}
						}
					}
					this.QueuedTargets.Clear();
				}
			}
			this.CloseActionPanel(operation is LaunchSTOInterceptorsOperation);
		}

		// Token: 0x0600549A RID: 21658 RVA: 0x002659CC File Offset: 0x00263BCC
		public void SetOperationInfo(IOperation operationType)
		{
			string displayName = operationType.GetDisplayName();
			this.operationName.SetText(displayName);
			this.operationDescription.SetText(operationType.GetDescription(this.actorState, this.targetBase));
			this.confirmOperationName.SetText(displayName);
		}

		// Token: 0x0600549B RID: 21659 RVA: 0x00265A15 File Offset: 0x00263C15
		private void UpdateOperationInfo()
		{
		}

		// Token: 0x0600549C RID: 21660 RVA: 0x00265A17 File Offset: 0x00263C17
		public void OnOperationPointerEnter(OperationButtonController button)
		{
			if (this.activeButton == null)
			{
				this.operationInfoPanel.SetActive(true);
				this.SetOperationInfo(button.operationType);
			}
		}

		// Token: 0x0600549D RID: 21661 RVA: 0x00265A3F File Offset: 0x00263C3F
		public void OnOperationPointerExit(OperationButtonController button)
		{
			if (this.activeButton == null)
			{
				this.operationInfoPanel.SetActive(false);
			}
		}

		// Token: 0x0600549E RID: 21662 RVA: 0x00265A5B File Offset: 0x00263C5B
		public void InitiateTargetOrbits(TargetOrbits e)
		{
			if (e.targetingState.ref_faction == base.activePlayer)
			{
				this.targetSelectionTool.Filter = e.barycenter;
			}
		}

		// Token: 0x0600549F RID: 21663 RVA: 0x00265A86 File Offset: 0x00263C86
		public void EndTargetOrbits(DeTargetOrbits e)
		{
			if (e.faction == base.activePlayer)
			{
				this.targetSelectionTool.Close();
			}
		}

		// Token: 0x060054A0 RID: 21664 RVA: 0x00265AA8 File Offset: 0x00263CA8
		public void InitiateTargetHabSites(TargetHabSites e)
		{
			if ((e.targetingState.isFactionState && e.targetingState.ref_faction == base.activePlayer) || (e.targetingState.isSpaceFleetState && e.targetingState.ref_fleet.faction == base.activePlayer))
			{
				TIGameState tigameState;
				if (e.targetingState.isFactionState)
				{
					tigameState = base.activePlayer;
				}
				else
				{
					tigameState = e.targetingState.ref_fleet;
				}
				this.targetSelectionTool.Open(this.currentOperationTemplate.GetPossibleTargets(tigameState, e.spaceBody), e.spaceBody, this.currentOperationTemplate);
			}
		}

		// Token: 0x060054A1 RID: 21665 RVA: 0x00265B4F File Offset: 0x00263D4F
		public void EndTargetHabSites(DeTargetHabSites e)
		{
			if (e.faction == base.activePlayer)
			{
				this.targetSelectionTool.Close();
			}
		}

		// Token: 0x060054A2 RID: 21666 RVA: 0x00265B6F File Offset: 0x00263D6F
		public void MinimizeTargetPanel()
		{
			this.targetSelectionTool.Close();
			this.maximizeButtonGameObject.SetActive(true);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
		}

		// Token: 0x060054A3 RID: 21667 RVA: 0x00265B94 File Offset: 0x00263D94
		public void MaximizeTargetPanel()
		{
			this.targetSelectionTool.Open();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.maximizeButtonGameObject.SetActive(false);
		}

		// Token: 0x060054A4 RID: 21668 RVA: 0x00265BB9 File Offset: 0x00263DB9
		public void OnFleetSplitProposed(TargetShipsForFleetSplit e)
		{
			this.InitializeSplitFleetPanel();
		}

		// Token: 0x060054A5 RID: 21669 RVA: 0x00265BC1 File Offset: 0x00263DC1
		public void OnFleetSplitConcluded(DetargetShipForFleetSplit e)
		{
			this.CloseSplitFleetPanel();
		}

		// Token: 0x060054A6 RID: 21670 RVA: 0x00265BC9 File Offset: 0x00263DC9
		public void CloseSplitFleetPanel()
		{
			this.fleetSplitPanel.SetActive(false);
			TIInputManager.blockSelectionRaycasts = false;
		}

		// Token: 0x060054A7 RID: 21671 RVA: 0x00265BE0 File Offset: 0x00263DE0
		public void InitializeSplitFleetPanel()
		{
			this.fleetSplitPanel.SetActive(true);
			this.targetNameObject.SetActive(false);
			this.paymentDropdownObject.SetActive(false);
			this.newFleetShips = new List<TISpaceShipState>();
			this.currentFleetName.SetText(this.actorState.displayName);
			if (this.currentOperationTemplate is SplitFleetOperation)
			{
				this.originFleetShips = SplitFleetOperation.EligibleShips(this.actorState.ref_fleet);
				this.fleetSplitPanelHeader.SetText(Loc.T("UI.Operations.SplitFleet"));
				this.newFleetText.SetText(Loc.T("UI.Operations.NewFleet"));
			}
			else if (this.currentOperationTemplate is ScuttleShipsOperation)
			{
				this.originFleetShips = new List<TISpaceShipState>(this.actorState.ref_fleet.ships);
				this.fleetSplitPanelHeader.SetText(Loc.T("UI.Operations.ScuttleShips"));
				this.newFleetText.SetText(Loc.T("UI.Operations.ShipsToScuttle"));
			}
			this.UpdateSplitFleetPanel();
		}

		// Token: 0x060054A8 RID: 21672 RVA: 0x00265CDC File Offset: 0x00263EDC
		public void UpdateSplitFleetPanel()
		{
			this.originFleetShips = (from x in this.originFleetShips
				orderby x.hull.length_m descending, x.wetMass_kg
				select x).ToList<TISpaceShipState>();
			this.newFleetShips = (from x in this.newFleetShips
				orderby x.hull.length_m descending, x.wetMass_kg
				select x).ToList<TISpaceShipState>();
			this.originFleetList.SetListSize<SplitFleetShipListItemController>(this.originFleetShips.Count, false, false);
			bool flag = this.currentOperationTemplate is ScuttleShipsOperation;
			int num = 0;
			using (IEnumerator<object> enumerator = this.originFleetList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__184.<>p__0 == null)
					{
						OperationCanvasController.<>o__184.<>p__0 = CallSite<Func<CallSite, object, SplitFleetShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SplitFleetShipListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__184.<>p__0.Target(OperationCanvasController.<>o__184.<>p__0, enumerator.Current).SetListItem(this, this.originFleetShips[num++], true, flag);
				}
			}
			num = 0;
			this.newFleetList.SetListSize<SplitFleetShipListItemController>(this.newFleetShips.Count, false, false);
			using (IEnumerator<object> enumerator = this.newFleetList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__184.<>p__1 == null)
					{
						OperationCanvasController.<>o__184.<>p__1 = CallSite<Func<CallSite, object, SplitFleetShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SplitFleetShipListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__184.<>p__1.Target(OperationCanvasController.<>o__184.<>p__1, enumerator.Current).SetListItem(this, this.newFleetShips[num++], false, flag);
				}
			}
			if (flag)
			{
				TIResourcesCost tiresourcesCost = new TIResourcesCost();
				foreach (TISpaceShipState tispaceShipState in this.newFleetShips)
				{
					tiresourcesCost.SumCosts_NoDuration(tispaceShipState.ScuttleCost());
				}
				this.confirmButton.interactable = this.newFleetShips.Count > 0 && tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
			}
			else
			{
				this.confirmButton.interactable = this.newFleetShips.Count > 0;
			}
			this.resetSplitFleetPanelButton.interactable = this.newFleetShips.Count > 0;
			this.splitAllDamagedButton.interactable = this.originFleetShips.Any<TISpaceShipState>((TISpaceShipState x) => x.damaged);
		}

		// Token: 0x060054A9 RID: 21673 RVA: 0x00265FEC File Offset: 0x002641EC
		public void OnClickMoveAllDamaged()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			foreach (TISpaceShipState tispaceShipState in this.originFleetShips.Where<TISpaceShipState>((TISpaceShipState x) => x.damaged).ToList<TISpaceShipState>())
			{
				this.originFleetShips.Remove(tispaceShipState);
				this.newFleetShips.Add(tispaceShipState);
			}
			this.UpdateSplitFleetPanel();
		}

		// Token: 0x060054AA RID: 21674 RVA: 0x0026608C File Offset: 0x0026428C
		public void OnClickResetSplitFleet()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			foreach (TISpaceShipState tispaceShipState in this.newFleetShips.ToList<TISpaceShipState>())
			{
				this.newFleetShips.Remove(tispaceShipState);
				this.originFleetShips.Add(tispaceShipState);
			}
			this.UpdateSplitFleetPanel();
		}

		// Token: 0x060054AB RID: 21675 RVA: 0x00266108 File Offset: 0x00264308
		public void SwapItem(TISpaceShipState ship, bool fromOriginFleet)
		{
			if (fromOriginFleet)
			{
				this.originFleetShips.Remove(ship);
				this.newFleetShips.Add(ship);
			}
			else
			{
				this.newFleetShips.Remove(ship);
				this.originFleetShips.Add(ship);
			}
			this.UpdateSplitFleetPanel();
		}

		// Token: 0x060054AC RID: 21676 RVA: 0x00266147 File Offset: 0x00264347
		public void OnClickExitSplitFleetPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseActionPanel(false);
		}

		// Token: 0x060054AD RID: 21677 RVA: 0x0026615C File Offset: 0x0026435C
		public void OnClickCancelSplitFleet()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.CloseActionPanel(false);
		}

		// Token: 0x17000F00 RID: 3840
		// (get) Token: 0x060054AE RID: 21678 RVA: 0x00266171 File Offset: 0x00264371
		private PropellantGroup selectedPropellantGroup
		{
			get
			{
				return this.propellantsInFleet[this.selectedPropellantGroupIdx];
			}
		}

		// Token: 0x17000F01 RID: 3841
		// (get) Token: 0x060054AF RID: 21679 RVA: 0x00266184 File Offset: 0x00264384
		private TISpaceFleetState sharingFleet
		{
			get
			{
				return this.actorState.ref_fleet;
			}
		}

		// Token: 0x060054B0 RID: 21680 RVA: 0x00266191 File Offset: 0x00264391
		public void OnSharePropellantProposed(InitiateSharePropellant e)
		{
			this.OnBeginPropellantSharing();
		}

		// Token: 0x060054B1 RID: 21681 RVA: 0x00266199 File Offset: 0x00264399
		private bool ValidGiver(TISpaceShipState ship)
		{
			return this.selectedPropellantGroup.ships.Contains(ship) && ship.propellant_tons > 0f && !this.selectedTakers.Contains(ship);
		}

		// Token: 0x060054B2 RID: 21682 RVA: 0x002661CC File Offset: 0x002643CC
		private bool ValidTaker(TISpaceShipState ship)
		{
			return (this.selectedPropellantGroup.ships.Contains(ship) || ship.propellant == Propellant.Anything) && ship.NeedsRefuel() && this.ProposedGive(ship) == 0f;
		}

		// Token: 0x060054B3 RID: 21683 RVA: 0x00266204 File Offset: 0x00264404
		public void OnBeginPropellantSharing()
		{
			this.currentTarget = this.sharingFleet;
			this.durationReportObject.SetActive(true);
			this.paymentDropdownObject.SetActive(false);
			this.paymentDropdown.enabled = false;
			this.availableGivers.Clear();
			this.availableTakers.Clear();
			this.selectedTakers.Clear();
			this.lockedTakers.Clear();
			this.propellantSharingEvents.Clear();
			this.propellantsInFleet = this.sharingFleet.BuildPropellantGroups();
			this.selectedPropellantGroupIdx = 0;
			this.propellantSharingPanel.SetActive(true);
			int num = 0;
			this.propellantTypeList.SetListSize<PropellantListItemController>(this.propellantsInFleet.Count, false, false);
			using (IEnumerator<object> enumerator = this.propellantTypeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__221.<>p__0 == null)
					{
						OperationCanvasController.<>o__221.<>p__0 = CallSite<Func<CallSite, object, PropellantListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PropellantListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__221.<>p__0.Target(OperationCanvasController.<>o__221.<>p__0, enumerator.Current).SetListItem(this.propellantsInFleet[num], this, num);
					num++;
				}
			}
			this.availableGiversList.SetListSize<FuelSharingListItemController>(this.sharingFleet.ships.Count, false, false);
			this.selectedTakersList.SetListSize<FuelSharingListItemController>(this.sharingFleet.ships.Count, false, false);
			this.availableTakersList.SetListSize<FuelSharingListItemController>(this.sharingFleet.ships.Count, false, false);
			num = 0;
			using (IEnumerator<object> enumerator = this.availableGiversList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__221.<>p__1 == null)
					{
						OperationCanvasController.<>o__221.<>p__1 = CallSite<Func<CallSite, object, FuelSharingListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FuelSharingListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__221.<>p__1.Target(OperationCanvasController.<>o__221.<>p__1, enumerator.Current).SetListItem(this.sharingFleet.ships[num++], 0, this);
				}
			}
			num = 0;
			using (IEnumerator<object> enumerator = this.selectedTakersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__221.<>p__2 == null)
					{
						OperationCanvasController.<>o__221.<>p__2 = CallSite<Func<CallSite, object, FuelSharingListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FuelSharingListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__221.<>p__2.Target(OperationCanvasController.<>o__221.<>p__2, enumerator.Current).SetListItem(this.sharingFleet.ships[num++], 1, this);
				}
			}
			num = 0;
			using (IEnumerator<object> enumerator = this.availableTakersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__221.<>p__3 == null)
					{
						OperationCanvasController.<>o__221.<>p__3 = CallSite<Func<CallSite, object, FuelSharingListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FuelSharingListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__221.<>p__3.Target(OperationCanvasController.<>o__221.<>p__3, enumerator.Current).SetListItem(this.sharingFleet.ships[num++], 2, this);
				}
			}
			this.OnPropellantSelected(0, true);
		}

		// Token: 0x060054B4 RID: 21684 RVA: 0x0026655C File Offset: 0x0026475C
		public void OnPropellantSelected(int idx, bool force)
		{
			if (idx != this.selectedPropellantGroupIdx || force)
			{
				this.selectedPropellantGroupIdx = idx;
				using (IEnumerator<object> enumerator = this.propellantTypeList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (OperationCanvasController.<>o__222.<>p__0 == null)
						{
							OperationCanvasController.<>o__222.<>p__0 = CallSite<Func<CallSite, object, PropellantListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PropellantListItemController), typeof(OperationCanvasController)));
						}
						PropellantListItemController propellantListItemController = OperationCanvasController.<>o__222.<>p__0.Target(OperationCanvasController.<>o__222.<>p__0, enumerator.Current);
						propellantListItemController.SetButtonHighlight(this.selectedPropellantGroupIdx == propellantListItemController.idx);
					}
				}
				this.availableGivers.Clear();
				this.selectedTakers.Clear();
				this.availableTakers.Clear();
				this.selectedTakers = (from x in this.propellantSharingEvents
					where this.selectedPropellantGroup.ships.Contains(x.giver)
					select x.taker).Distinct<TISpaceShipState>().ToList<TISpaceShipState>();
				foreach (TISpaceShipState tispaceShipState in this.sharingFleet.ships.Except<TISpaceShipState>(this.selectedTakers))
				{
					if (this.ValidGiver(tispaceShipState))
					{
						this.availableGivers.Add(tispaceShipState);
					}
					if (this.ValidTaker(tispaceShipState))
					{
						this.availableTakers.Add(tispaceShipState);
					}
				}
				this.UpdateAllSharingLists();
			}
		}

		// Token: 0x060054B5 RID: 21685 RVA: 0x002666F4 File Offset: 0x002648F4
		public void UpdateAllSharingLists()
		{
			using (IEnumerator<object> enumerator = this.availableGiversList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__223.<>p__0 == null)
					{
						OperationCanvasController.<>o__223.<>p__0 = CallSite<Func<CallSite, object, FuelSharingListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FuelSharingListItemController), typeof(OperationCanvasController)));
					}
					FuelSharingListItemController fuelSharingListItemController = OperationCanvasController.<>o__223.<>p__0.Target(OperationCanvasController.<>o__223.<>p__0, enumerator.Current);
					fuelSharingListItemController.UpdateForProposedTransfers();
					fuelSharingListItemController.gameObject.SetActive(this.availableGivers.Contains(fuelSharingListItemController.ship));
				}
			}
			using (IEnumerator<object> enumerator = this.selectedTakersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__223.<>p__1 == null)
					{
						OperationCanvasController.<>o__223.<>p__1 = CallSite<Func<CallSite, object, FuelSharingListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FuelSharingListItemController), typeof(OperationCanvasController)));
					}
					FuelSharingListItemController fuelSharingListItemController2 = OperationCanvasController.<>o__223.<>p__1.Target(OperationCanvasController.<>o__223.<>p__1, enumerator.Current);
					fuelSharingListItemController2.UpdateForProposedTransfers();
					fuelSharingListItemController2.gameObject.SetActive(this.selectedTakers.Contains(fuelSharingListItemController2.ship));
				}
			}
			using (IEnumerator<object> enumerator = this.availableTakersList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__223.<>p__2 == null)
					{
						OperationCanvasController.<>o__223.<>p__2 = CallSite<Func<CallSite, object, FuelSharingListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FuelSharingListItemController), typeof(OperationCanvasController)));
					}
					FuelSharingListItemController fuelSharingListItemController3 = OperationCanvasController.<>o__223.<>p__2.Target(OperationCanvasController.<>o__223.<>p__2, enumerator.Current);
					fuelSharingListItemController3.UpdateForProposedTransfers();
					fuelSharingListItemController3.gameObject.SetActive(this.availableTakers.Contains(fuelSharingListItemController3.ship));
				}
			}
			float refuelDuration_days = InterfleetRefuelOperation.GetRefuelDuration_days(this.propellantSharingEvents);
			this.durationText.SetText(Loc.T("UI.Operations.Duration_days", new object[] { refuelDuration_days.ToString(TIUtilities.DecimalPlaces((double)refuelDuration_days, 2, 0)) }));
			this.SetSharingButtions();
		}

		// Token: 0x060054B6 RID: 21686 RVA: 0x00266910 File Offset: 0x00264B10
		public void SetSharingButtions()
		{
			IEnumerable<TISpaceShipState> enumerable = (from x in this.propellantSharingEvents
				where this.selectedPropellantGroup.ships.Contains(x.giver)
				select x.taker).Distinct<TISpaceShipState>();
			this.ResetTakersButton.interactable = enumerable.Count<TISpaceShipState>() > enumerable.Intersect<TISpaceShipState>(this.lockedTakers).Count<TISpaceShipState>();
			this.EqualizeDistributionButton.interactable = this.availableGivers.Count + enumerable.Count<TISpaceShipState>() > 0 && enumerable.Intersect<TISpaceShipState>(this.lockedTakers).Count<TISpaceShipState>() == 0;
			this.confirmButton.interactable = this.propellantSharingEvents.Any<PropellantSharingEvent>((PropellantSharingEvent x) => x.amount_tons > 0f);
		}

		// Token: 0x060054B7 RID: 21687 RVA: 0x002669EE File Offset: 0x00264BEE
		public void OnTakerAdded(TISpaceShipState ship)
		{
			if (!this.selectedTakers.Contains(ship) && this.availableTakers.Remove(ship))
			{
				this.selectedTakers.Add(ship);
				this.availableGivers.Remove(ship);
				this.UpdateAllSharingLists();
			}
		}

		// Token: 0x060054B8 RID: 21688 RVA: 0x00266A2C File Offset: 0x00264C2C
		public void OnTakerRemoved(TISpaceShipState ship)
		{
			if (this.selectedTakers.Remove(ship))
			{
				this.propellantSharingEvents.RemoveAll((PropellantSharingEvent x) => x.taker == ship);
				if (this.ValidTaker(ship))
				{
					this.availableTakers.AddUnique(ship);
				}
				if (this.ValidGiver(ship))
				{
					this.availableGivers.AddUnique(ship);
				}
				this.UpdateAllSharingLists();
			}
		}

		// Token: 0x060054B9 RID: 21689 RVA: 0x00266AB7 File Offset: 0x00264CB7
		public void LockTaker(TISpaceShipState taker)
		{
			if (this.selectedTakers.Contains(taker))
			{
				this.lockedTakers.Add(taker);
				this.SetSharingButtions();
			}
		}

		// Token: 0x060054BA RID: 21690 RVA: 0x00266AD9 File Offset: 0x00264CD9
		public void UnlockTaker(TISpaceShipState taker)
		{
			if (this.lockedTakers.Contains(taker))
			{
				this.lockedTakers.Remove(taker);
				this.SetSharingButtions();
			}
		}

		// Token: 0x060054BB RID: 21691 RVA: 0x00266AFC File Offset: 0x00264CFC
		public float SetPropellantSharingEvent(TISpaceShipState giver, TISpaceShipState taker, float amount_tons)
		{
			float num = Mathf.Min(amount_tons, this.NeededTake(taker));
			bool flag = false;
			foreach (PropellantSharingEvent propellantSharingEvent in this.propellantSharingEvents.ToList<PropellantSharingEvent>())
			{
				if (propellantSharingEvent.giver == giver && propellantSharingEvent.taker == taker)
				{
					propellantSharingEvent.amount_tons += num;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.propellantSharingEvents.Add(new PropellantSharingEvent
				{
					giver = giver,
					taker = taker,
					amount_tons = num
				});
			}
			if (num > 0f)
			{
				this.availableTakers.Remove(giver);
			}
			this.UpdateAllSharingLists();
			return amount_tons - num;
		}

		// Token: 0x060054BC RID: 21692 RVA: 0x00266BD4 File Offset: 0x00264DD4
		public float ProposedTake(TISpaceShipState ship)
		{
			return this.propellantSharingEvents.Where<PropellantSharingEvent>((PropellantSharingEvent x) => x.taker == ship).Sum<PropellantSharingEvent>((PropellantSharingEvent x) => x.amount_tons);
		}

		// Token: 0x060054BD RID: 21693 RVA: 0x00266C2C File Offset: 0x00264E2C
		public float ProposedGive(TISpaceShipState ship)
		{
			return this.propellantSharingEvents.Where<PropellantSharingEvent>((PropellantSharingEvent x) => x.giver == ship).Sum<PropellantSharingEvent>((PropellantSharingEvent x) => x.amount_tons);
		}

		// Token: 0x060054BE RID: 21694 RVA: 0x00266C81 File Offset: 0x00264E81
		public float NeededTake(TISpaceShipState ship)
		{
			return ship.PropellantShortage_tons - this.ProposedTake(ship);
		}

		// Token: 0x060054BF RID: 21695 RVA: 0x00266C91 File Offset: 0x00264E91
		public float AvailableGive(TISpaceShipState ship)
		{
			return ship.propellant_tons - this.ProposedGive(ship);
		}

		// Token: 0x17000F02 RID: 3842
		// (get) Token: 0x060054C0 RID: 21696 RVA: 0x00266CA1 File Offset: 0x00264EA1
		public List<TISpaceShipState> freeTakers
		{
			get
			{
				return this.selectedTakers.Except<TISpaceShipState>(this.lockedTakers).ToList<TISpaceShipState>();
			}
		}

		// Token: 0x060054C1 RID: 21697 RVA: 0x00266CBC File Offset: 0x00264EBC
		public void AttemptGivePropellant(TISpaceShipState giver, float amount_tons)
		{
			amount_tons = Mathf.Min(amount_tons, this.AvailableGive(giver));
			float perTaker_tons = amount_tons / (float)this.freeTakers.Where<TISpaceShipState>((TISpaceShipState x) => this.NeededTake(x) > 0f).Count<TISpaceShipState>();
			if (this.freeTakers.All<TISpaceShipState>((TISpaceShipState x) => this.NeededTake(x) >= perTaker_tons))
			{
				using (IEnumerator<TISpaceShipState> enumerator = this.freeTakers.Except<TISpaceShipState>(this.lockedTakers).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceShipState tispaceShipState = enumerator.Current;
						this.SetPropellantSharingEvent(giver, tispaceShipState, perTaker_tons);
					}
					return;
				}
			}
			IEnumerable<TISpaceShipState> freeTakers = this.freeTakers;
			Func<TISpaceShipState, bool> <>9__3;
			Func<TISpaceShipState, bool> func;
			if ((func = <>9__3) == null)
			{
				func = (<>9__3 = (TISpaceShipState x) => this.NeededTake(x) < perTaker_tons);
			}
			foreach (TISpaceShipState tispaceShipState2 in freeTakers.Where<TISpaceShipState>(func))
			{
				float num = this.NeededTake(tispaceShipState2);
				this.SetPropellantSharingEvent(giver, tispaceShipState2, num);
				amount_tons -= num;
			}
			perTaker_tons = amount_tons / (float)this.freeTakers.Where<TISpaceShipState>((TISpaceShipState x) => this.NeededTake(x) > 0f).Count<TISpaceShipState>();
			foreach (TISpaceShipState tispaceShipState3 in this.freeTakers)
			{
				this.SetPropellantSharingEvent(giver, tispaceShipState3, perTaker_tons);
			}
		}

		// Token: 0x060054C2 RID: 21698 RVA: 0x00266E64 File Offset: 0x00265064
		public float ReturnPropellantToGiver(TISpaceShipState giver, TISpaceShipState taker, float amount_tons)
		{
			float num = 0f;
			foreach (PropellantSharingEvent propellantSharingEvent in this.propellantSharingEvents.ToList<PropellantSharingEvent>())
			{
				if (propellantSharingEvent.giver == giver && propellantSharingEvent.taker == taker)
				{
					propellantSharingEvent.amount_tons -= amount_tons;
					if (propellantSharingEvent.amount_tons <= 0f)
					{
						num = -propellantSharingEvent.amount_tons;
						this.propellantSharingEvents.Remove(propellantSharingEvent);
						break;
					}
					break;
				}
			}
			if (this.ValidTaker(giver))
			{
				this.availableTakers.AddUnique(giver);
			}
			this.UpdateAllSharingLists();
			return num;
		}

		// Token: 0x060054C3 RID: 21699 RVA: 0x00266F28 File Offset: 0x00265128
		public void ResetPropellantGiver(TISpaceShipState ship)
		{
			foreach (PropellantSharingEvent propellantSharingEvent in this.propellantSharingEvents.ToList<PropellantSharingEvent>())
			{
				if (propellantSharingEvent.giver == ship && !this.lockedTakers.Contains(propellantSharingEvent.taker))
				{
					this.propellantSharingEvents.Remove(propellantSharingEvent);
				}
			}
			if (this.ValidTaker(ship))
			{
				this.availableTakers.AddUnique(ship);
			}
			this.UpdateAllSharingLists();
		}

		// Token: 0x060054C4 RID: 21700 RVA: 0x00266FC4 File Offset: 0x002651C4
		public void ResetPropellantTaker(TISpaceShipState ship)
		{
			if (!this.lockedTakers.Contains(ship))
			{
				foreach (PropellantSharingEvent propellantSharingEvent in this.propellantSharingEvents.ToList<PropellantSharingEvent>())
				{
					if (propellantSharingEvent.taker == ship)
					{
						this.propellantSharingEvents.Remove(propellantSharingEvent);
						if (this.ValidTaker(ship))
						{
							this.availableTakers.AddUnique(ship);
						}
					}
				}
				this.UpdateAllSharingLists();
			}
		}

		// Token: 0x060054C5 RID: 21701 RVA: 0x0026705C File Offset: 0x0026525C
		public void OnResetReceiversClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			foreach (TISpaceShipState tispaceShipState in this.freeTakers.ToList<TISpaceShipState>())
			{
				this.OnTakerRemoved(tispaceShipState);
			}
		}

		// Token: 0x060054C6 RID: 21702 RVA: 0x002670C0 File Offset: 0x002652C0
		public void OnEqualDistroPropellantPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			List<TISpaceShipState> list = this.availableGivers.Union<TISpaceShipState>(this.selectedTakers).Union<TISpaceShipState>(this.availableTakers).Distinct<TISpaceShipState>()
				.ToList<TISpaceShipState>();
			foreach (TISpaceShipState tispaceShipState in list)
			{
				foreach (PropellantSharingEvent propellantSharingEvent in this.propellantSharingEvents.ToList<PropellantSharingEvent>())
				{
					if (propellantSharingEvent.giver == tispaceShipState || propellantSharingEvent.taker == tispaceShipState)
					{
						this.propellantSharingEvents.Remove(propellantSharingEvent);
					}
				}
			}
			List<PropellantSharingEvent> list2 = TISpaceFleetState.CreatePropellantSharingPlan_Equalization(list, false).ToList<PropellantSharingEvent>();
			foreach (PropellantSharingEvent propellantSharingEvent2 in list2)
			{
				this.availableTakers.Remove(propellantSharingEvent2.giver);
				this.OnTakerAdded(propellantSharingEvent2.taker);
			}
			this.propellantSharingEvents.AddRange(list2);
			this.UpdateAllSharingLists();
		}

		// Token: 0x060054C7 RID: 21703 RVA: 0x00267220 File Offset: 0x00265420
		public void OnCloseSharePropellant()
		{
			this.propellantSharingEvents.Clear();
			this.availableGivers.Clear();
			this.availableTakers.Clear();
			this.selectedTakers.Clear();
			this.lockedTakers.Clear();
			this.propellantTypeList.SetListSize<PropellantListItemController>(0, false, false);
			this.availableGiversList.SetListSize<FuelSharingListItemController>(0, false, false);
			this.selectedTakersList.SetListSize<FuelSharingListItemController>(0, false, false);
			this.availableTakersList.SetListSize<FuelSharingListItemController>(0, false, false);
			this.propellantSharingPanel.SetActive(false);
		}

		// Token: 0x060054C8 RID: 21704 RVA: 0x002672A8 File Offset: 0x002654A8
		private string AssetOfficerCapacityString(OfficerCarrierState asset)
		{
			bool? flag;
			if (asset == null)
			{
				flag = null;
			}
			else
			{
				TIGameState state = asset.GetState();
				flag = ((state != null) ? new bool?(state.isSpaceShipState) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				return Loc.T("UI.Operations.ShipOfficerCapacity", new object[] { asset.GetState().ref_ship.hull.maxOfficers });
			}
			bool? flag3;
			if (asset == null)
			{
				flag3 = null;
			}
			else
			{
				TIGameState state2 = asset.GetState();
				flag3 = ((state2 != null) ? new bool?(state2.isHabState) : null);
			}
			flag2 = flag3;
			if (flag2.GetValueOrDefault())
			{
				return Loc.T("UI.Operations.HabOfficerCapacity", new object[] { asset.GetState().ref_hab.MaxOfficerStorageAllowed() });
			}
			return string.Empty;
		}

		// Token: 0x060054C9 RID: 21705 RVA: 0x0026737F File Offset: 0x0026557F
		public void OnTransferOfficersProposed(InitiateTransferOfficers e)
		{
			this.SetupTransferOperation(this.actorState.ref_fleet);
		}

		// Token: 0x060054CA RID: 21706 RVA: 0x00267394 File Offset: 0x00265594
		public void InitializeOfficerTransferCanvas()
		{
			this.transferOfficersCanvas.gameObject.SetActive(false);
			this.transferOfficersCanvas.enabled = false;
			this.transferOfficerCanvasHeader.SetText(Loc.T("UI.Operations.TransferOfficersHeader"));
			this.givingAssetHeader.SetText(Loc.T("UI.Operations.AssetColumnHead"));
			this.receivingAssetHeader.SetText(Loc.T("UI.Operations.AssetColumnHead"));
			this.transferOfficersConfirmButtonText.SetText(Loc.T("UI.Operations.ConfirmOfficerTransfer"));
			this.transferOfficersResetButtonText.SetText(Loc.T("UI.Operations.ResetOfficerTransfer"));
			this.plannedOfficerTransfers = new Dictionary<TIOfficerState, OfficerCarrierState>();
		}

		// Token: 0x060054CB RID: 21707 RVA: 0x00267434 File Offset: 0x00265634
		public void ConfirmTransferOfficersPressed()
		{
			if (!this.plannedOfficerTransfers.Any<KeyValuePair<TIOfficerState, OfficerCarrierState>>())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			if (this.StartTransferOfficersOperation())
			{
				this.SetupTransferOperation(this.actorState.ref_fleet);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x060054CC RID: 21708 RVA: 0x00267484 File Offset: 0x00265684
		public bool StartTransferOfficersOperation()
		{
			if (this.plannedOfficerTransfers.Any<KeyValuePair<TIOfficerState, OfficerCarrierState>>() && TransferOfficersOperation.ResourceCostOptions(this.plannedOfficerTransfers).FirstOrDefault<TIResourcesCost>().CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
			{
				base.activePlayer.playerControl.StartAction(new BeginOfficerTransferOperationAction(this.actorState.ref_fleet, this.plannedOfficerTransfers));
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmFleetOperation", false, false);
				return true;
			}
			return false;
		}

		// Token: 0x060054CD RID: 21709 RVA: 0x002674FB File Offset: 0x002656FB
		public void ResetTransferOfficersPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.SetupTransferOperation(this.actorState.ref_fleet);
		}

		// Token: 0x060054CE RID: 21710 RVA: 0x0026751C File Offset: 0x0026571C
		public void CloseOfficerTransferCanvas()
		{
			this.transferOfficersCanvas.enabled = false;
			this.transferOfficersCanvas.gameObject.SetActive(false);
			Dictionary<TIOfficerState, OfficerCarrierState> dictionary = this.plannedOfficerTransfers;
			if (dictionary != null)
			{
				dictionary.Clear();
			}
			this.selectedOfficerGiver = null;
			this.selectedOfficerReceiver = null;
			this.givingAssetList.SetListSize<TransferOfficerAssetListItemController>(0, false, false);
			this.receivingAssetList.SetListSize<TransferOfficerAssetListItemController>(0, false, false);
			this.selectedAssetOfficerList.SetListSize<TransferOfficerListItemController>(0, false, false);
			this.receivingAssetOfficerList.SetListSize<TransferOfficerListItemController>(0, false, false);
		}

		// Token: 0x060054CF RID: 21711 RVA: 0x002675A0 File Offset: 0x002657A0
		public void SetupTransferOperation(TISpaceFleetState actingFleet)
		{
			this.selectedOfficerGiver = null;
			this.selectedOfficerReceiver = null;
			this.plannedOfficerTransfers.Clear();
			this.paymentDropdownObject.SetActive(false);
			this.paymentDropdown.enabled = false;
			this.selectedResourceCostOption = null;
			this.paymentDropdown.ClearOptions();
			this.officerTransferAssets = new List<OfficerCarrierState>(actingFleet.ships);
			if (actingFleet.dockedAtHab && actingFleet.ref_hab.faction == actingFleet.faction)
			{
				this.officerTransferAssets.AddRange(from x in actingFleet.ref_hab.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x != actingFleet && x.faction == actingFleet.faction).SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships)
					select (x));
				this.officerTransferAssets.Add(actingFleet.ref_hab);
			}
			this.givingAssetList.SetListSize<TransferOfficerAssetListItemController>(this.officerTransferAssets.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.givingAssetList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__270.<>p__0 == null)
					{
						OperationCanvasController.<>o__270.<>p__0 = CallSite<Func<CallSite, object, TransferOfficerAssetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TransferOfficerAssetListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__270.<>p__0.Target(OperationCanvasController.<>o__270.<>p__0, enumerator.Current).SetListItem(this.officerTransferAssets[num++], true, this);
				}
			}
			int num2 = 0;
			this.receivingAssetList.SetListSize<TransferOfficerAssetListItemController>(this.officerTransferAssets.Count, false, false);
			using (IEnumerator<object> enumerator = this.receivingAssetList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__270.<>p__1 == null)
					{
						OperationCanvasController.<>o__270.<>p__1 = CallSite<Func<CallSite, object, TransferOfficerAssetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TransferOfficerAssetListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__270.<>p__1.Target(OperationCanvasController.<>o__270.<>p__1, enumerator.Current).SetListItem(this.officerTransferAssets[num2++], false, this);
				}
			}
			this.selectedAssetOfficerList.SetListSize<TransferOfficerListItemController>(0, false, false);
			this.receivingAssetOfficerList.SetListSize<TransferOfficerListItemController>(0, false, false);
			this.givingSideOfficerListItems = new Dictionary<TIOfficerState, TransferOfficerListItemController>();
			this.receivingSideOfficerListItems = new Dictionary<TIOfficerState, TransferOfficerListItemController>();
			this.givingOfficerHeader.SetText(string.Empty);
			this.receivingOfficerHeader.SetText(string.Empty);
			this.givingAssetOfficerCapacity.SetText(string.Empty);
			this.receivingAssetOfficerCapacity.SetText(string.Empty);
			this.transferOfficersCanvas.enabled = true;
			this.transferOfficersCanvas.gameObject.SetActive(true);
		}

		// Token: 0x060054D0 RID: 21712 RVA: 0x002678B0 File Offset: 0x00265AB0
		public void SetOfficerTransferListItemsValid(ListManagerBase officerList, OfficerCarrierState officerListCarrier, OfficerCarrierState otherOfficerCarrier)
		{
			int num = this.plannedOfficerTransfers.Keys.Count<TIOfficerState>((TIOfficerState x) => officerListCarrier.GetOfficers().Contains(x));
			using (IEnumerator<object> enumerator = officerList.GetEnumerator())
			{
				Func<TIOfficerState, bool> <>9__1;
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__271.<>p__0 == null)
					{
						OperationCanvasController.<>o__271.<>p__0 = CallSite<Func<CallSite, object, TransferOfficerListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TransferOfficerListItemController), typeof(OperationCanvasController)));
					}
					TransferOfficerListItemController transferOfficerListItemController = OperationCanvasController.<>o__271.<>p__0.Target(OperationCanvasController.<>o__271.<>p__0, enumerator.Current);
					if (officerListCarrier == null || otherOfficerCarrier == null || officerListCarrier == otherOfficerCarrier)
					{
						transferOfficerListItemController.transferOfficerButton.interactable = false;
					}
					else if (!officerListCarrier.GetOfficers().Contains(transferOfficerListItemController.officer))
					{
						transferOfficerListItemController.transferOfficerButton.interactable = true;
					}
					else
					{
						Selectable transferOfficerButton = transferOfficerListItemController.transferOfficerButton;
						TIOfficerState officer = transferOfficerListItemController.officer;
						OfficerCarrierState officerListCarrier2 = officerListCarrier;
						OfficerCarrierState otherOfficerCarrier2 = otherOfficerCarrier;
						bool flag = false;
						TIOfficerState officer2 = transferOfficerListItemController.officer;
						OfficerCarrierState otherOfficerCarrier3 = otherOfficerCarrier;
						IEnumerable<TIOfficerState> keys = this.plannedOfficerTransfers.Keys;
						Func<TIOfficerState, bool> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = (TIOfficerState x) => this.plannedOfficerTransfers[x] == otherOfficerCarrier);
						}
						transferOfficerButton.interactable = officer.CanTransferOfficer(officerListCarrier2, otherOfficerCarrier2, flag, officer2.ProposedTransferIsSwap(otherOfficerCarrier3, keys.Where<TIOfficerState>(func).ToList<TIOfficerState>()), num);
					}
				}
			}
		}

		// Token: 0x060054D1 RID: 21713 RVA: 0x00267A3C File Offset: 0x00265C3C
		public void SetSelectedGiver(OfficerCarrierState selectedGiver)
		{
			bool flag = selectedGiver != this.selectedOfficerGiver;
			if (flag)
			{
				this.plannedOfficerTransfers.Clear();
				this.selectedOfficerGiver = selectedGiver;
				if (this.selectedOfficerGiver == this.selectedOfficerReceiver)
				{
					this.SetSelectedReciever(null);
				}
				TMP_Text tmp_Text = this.givingOfficerHeader;
				OfficerCarrierState officerCarrierState = this.selectedOfficerGiver;
				tmp_Text.SetText(((officerCarrierState != null) ? officerCarrierState.GetState().displayName : null) ?? string.Empty);
			}
			using (IEnumerator<object> enumerator = this.givingAssetList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__272.<>p__0 == null)
					{
						OperationCanvasController.<>o__272.<>p__0 = CallSite<Func<CallSite, object, TransferOfficerAssetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TransferOfficerAssetListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__272.<>p__0.Target(OperationCanvasController.<>o__272.<>p__0, enumerator.Current).HighlightButtonAfterSelection();
				}
			}
			this.givingSideOfficerListItems.Clear();
			OfficerCarrierState officerCarrierState2 = this.selectedOfficerGiver;
			List<TIOfficerState> list;
			if (officerCarrierState2 == null)
			{
				list = null;
			}
			else
			{
				list = (from x in officerCarrierState2.GetOfficers()
					orderby x.template.sortOrder
					select x).ToList<TIOfficerState>();
			}
			List<TIOfficerState> list2 = list ?? new List<TIOfficerState>();
			List<TIOfficerState> list3 = new List<TIOfficerState>();
			if (this.selectedOfficerReceiver != null && this.selectedOfficerGiver != null)
			{
				list3 = (from x in this.selectedOfficerReceiver.GetOfficers()
					orderby x.template.sortOrder
					select x).ToList<TIOfficerState>();
				list2.AddRange(list3);
			}
			this.selectedAssetOfficerList.SetListSize<TransferOfficerListItemController>(list2.Count, false, false);
			if (list2.Count > 0)
			{
				int num = 0;
				using (IEnumerator<object> enumerator = this.selectedAssetOfficerList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (OperationCanvasController.<>o__272.<>p__1 == null)
						{
							OperationCanvasController.<>o__272.<>p__1 = CallSite<Func<CallSite, object, TransferOfficerListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TransferOfficerListItemController), typeof(OperationCanvasController)));
						}
						TransferOfficerListItemController transferOfficerListItemController = OperationCanvasController.<>o__272.<>p__1.Target(OperationCanvasController.<>o__272.<>p__1, enumerator.Current);
						this.givingSideOfficerListItems.Add(list2[num], transferOfficerListItemController);
						transferOfficerListItemController.SetListItem(list2[num], this, true, list3.Contains(list2[num]), this.selectedOfficerGiver);
						num++;
					}
				}
			}
			this.givingAssetOfficerCapacity.SetText(this.AssetOfficerCapacityString(this.selectedOfficerGiver));
			if (this.selectedOfficerGiver != null)
			{
				this.SetOfficerTransferListItemsValid(this.selectedAssetOfficerList, this.selectedOfficerGiver, this.selectedOfficerReceiver);
			}
			if (flag)
			{
				this.SetSelectedReciever(this.selectedOfficerReceiver);
			}
		}

		// Token: 0x060054D2 RID: 21714 RVA: 0x00267CF0 File Offset: 0x00265EF0
		public void SetSelectedReciever(OfficerCarrierState selectedReceiver)
		{
			bool flag = selectedReceiver != this.selectedOfficerReceiver;
			if (flag)
			{
				this.plannedOfficerTransfers.Clear();
				this.selectedOfficerReceiver = selectedReceiver;
				if (this.selectedOfficerGiver == this.selectedOfficerReceiver)
				{
					this.SetSelectedGiver(null);
				}
				TMP_Text tmp_Text = this.receivingOfficerHeader;
				OfficerCarrierState officerCarrierState = this.selectedOfficerReceiver;
				tmp_Text.SetText(((officerCarrierState != null) ? officerCarrierState.GetState().displayName : null) ?? string.Empty);
			}
			using (IEnumerator<object> enumerator = this.receivingAssetList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__273.<>p__0 == null)
					{
						OperationCanvasController.<>o__273.<>p__0 = CallSite<Func<CallSite, object, TransferOfficerAssetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TransferOfficerAssetListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__273.<>p__0.Target(OperationCanvasController.<>o__273.<>p__0, enumerator.Current).HighlightButtonAfterSelection();
				}
			}
			this.receivingSideOfficerListItems.Clear();
			OfficerCarrierState officerCarrierState2 = this.selectedOfficerReceiver;
			List<TIOfficerState> list;
			if (officerCarrierState2 == null)
			{
				list = null;
			}
			else
			{
				list = (from x in officerCarrierState2.GetOfficers()
					orderby x.template.sortOrder
					select x).ToList<TIOfficerState>();
			}
			List<TIOfficerState> list2 = list ?? new List<TIOfficerState>();
			List<TIOfficerState> list3 = new List<TIOfficerState>();
			if (this.selectedOfficerGiver != null && this.selectedOfficerReceiver != null)
			{
				list3 = (from x in this.selectedOfficerGiver.GetOfficers()
					orderby x.template.sortOrder
					select x).ToList<TIOfficerState>();
				list2.AddRange(list3);
			}
			this.receivingAssetOfficerList.SetListSize<TransferOfficerListItemController>(list2.Count, false, false);
			if (list2.Count > 0)
			{
				int num = 0;
				using (IEnumerator<object> enumerator = this.receivingAssetOfficerList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (OperationCanvasController.<>o__273.<>p__1 == null)
						{
							OperationCanvasController.<>o__273.<>p__1 = CallSite<Func<CallSite, object, TransferOfficerListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TransferOfficerListItemController), typeof(OperationCanvasController)));
						}
						TransferOfficerListItemController transferOfficerListItemController = OperationCanvasController.<>o__273.<>p__1.Target(OperationCanvasController.<>o__273.<>p__1, enumerator.Current);
						this.receivingSideOfficerListItems.Add(list2[num], transferOfficerListItemController);
						transferOfficerListItemController.SetListItem(list2[num], this, false, list3.Contains(list2[num]), this.selectedOfficerReceiver);
						num++;
					}
				}
			}
			this.receivingAssetOfficerCapacity.SetText(this.AssetOfficerCapacityString(this.selectedOfficerReceiver));
			if (this.selectedOfficerReceiver != null)
			{
				this.SetOfficerTransferListItemsValid(this.receivingAssetOfficerList, this.selectedOfficerReceiver, this.selectedOfficerGiver);
			}
			if (flag)
			{
				this.SetSelectedGiver(this.selectedOfficerGiver);
			}
		}

		// Token: 0x060054D3 RID: 21715 RVA: 0x00267FA4 File Offset: 0x002661A4
		public void ProposeOfficerTransfer(TIOfficerState officer, bool fromGivers)
		{
			if (this.selectedOfficerGiver != null && this.selectedOfficerReceiver != null)
			{
				if (this.plannedOfficerTransfers.ContainsKey(officer))
				{
					fromGivers = !fromGivers;
					this.plannedOfficerTransfers.Remove(officer);
					if (fromGivers)
					{
						bool flag = this.selectedOfficerReceiver.GetState().isSpaceShipState && officer.ProposedTransferIsSwap(this.selectedOfficerReceiver.GetState().ref_ship, this.<ProposeOfficerTransfer>g__ProposedOfficerTransfersToReceiver|274_0());
						this.givingSideOfficerListItems[officer].gameObject.SetActive(true);
						this.receivingSideOfficerListItems[officer].gameObject.SetActive(false);
						if (flag)
						{
							TIOfficerState tiofficerState = officer.ProposedOfficerSwap(this.selectedOfficerReceiver.GetState().ref_ship, this.<ProposeOfficerTransfer>g__ProposedOfficerTransfersToReceiver|274_0());
							if (tiofficerState != null && this.plannedOfficerTransfers.ContainsKey(tiofficerState))
							{
								this.givingSideOfficerListItems[tiofficerState].Colorize(Color.white);
								this.givingSideOfficerListItems[tiofficerState].gameObject.SetActive(false);
								this.receivingSideOfficerListItems[tiofficerState].Colorize(Color.white);
								this.receivingSideOfficerListItems[tiofficerState].gameObject.SetActive(true);
								this.plannedOfficerTransfers.Remove(tiofficerState);
							}
						}
					}
					else
					{
						bool flag = this.selectedOfficerGiver.GetState().isSpaceShipState && officer.ProposedTransferIsSwap(this.selectedOfficerGiver.GetState().ref_ship, this.<ProposeOfficerTransfer>g__ProposedOfficerTransfersToGiver|274_1());
						this.givingSideOfficerListItems[officer].gameObject.SetActive(false);
						this.receivingSideOfficerListItems[officer].gameObject.SetActive(true);
						if (flag)
						{
							TIOfficerState tiofficerState2 = officer.ProposedOfficerSwap(this.selectedOfficerGiver.GetState().ref_ship, this.<ProposeOfficerTransfer>g__ProposedOfficerTransfersToGiver|274_1());
							if (tiofficerState2 != null && this.plannedOfficerTransfers.ContainsKey(tiofficerState2))
							{
								this.givingSideOfficerListItems[tiofficerState2].Colorize(Color.white);
								this.givingSideOfficerListItems[tiofficerState2].gameObject.SetActive(true);
								this.receivingSideOfficerListItems[tiofficerState2].Colorize(Color.white);
								this.receivingSideOfficerListItems[tiofficerState2].gameObject.SetActive(false);
								this.plannedOfficerTransfers.Remove(tiofficerState2);
							}
						}
					}
					this.givingSideOfficerListItems[officer].Colorize(Color.white);
					this.receivingSideOfficerListItems[officer].Colorize(Color.white);
				}
				else if (fromGivers)
				{
					bool flag = this.selectedOfficerReceiver.GetState().isSpaceShipState && officer.ProposedTransferIsSwap(this.selectedOfficerReceiver.GetState().ref_ship, this.<ProposeOfficerTransfer>g__ProposedOfficerTransfersToReceiver|274_0());
					int num = this.plannedOfficerTransfers.Keys.Count<TIOfficerState>((TIOfficerState x) => this.selectedOfficerGiver.GetOfficers().Contains(x));
					if (officer.CanTransferOfficer(this.selectedOfficerGiver, this.selectedOfficerReceiver, false, flag, num))
					{
						this.plannedOfficerTransfers.Add(officer, this.selectedOfficerReceiver);
						this.givingSideOfficerListItems[officer].gameObject.SetActive(false);
						this.receivingSideOfficerListItems[officer].gameObject.SetActive(true);
						this.receivingSideOfficerListItems[officer].Colorize(Color.green);
						if (flag)
						{
							TIOfficerState tiofficerState3 = officer.ProposedOfficerSwap(this.selectedOfficerReceiver.GetState().ref_ship, this.<ProposeOfficerTransfer>g__ProposedOfficerTransfersToReceiver|274_0());
							if (tiofficerState3 != null)
							{
								if (this.plannedOfficerTransfers.ContainsKey(tiofficerState3))
								{
									this.plannedOfficerTransfers.Remove(tiofficerState3);
								}
								else
								{
									this.plannedOfficerTransfers.Add(tiofficerState3, this.selectedOfficerGiver);
								}
								this.givingSideOfficerListItems[tiofficerState3].gameObject.SetActive(true);
								this.receivingSideOfficerListItems[tiofficerState3].gameObject.SetActive(false);
								this.givingSideOfficerListItems[tiofficerState3].Colorize(Color.green);
							}
						}
					}
				}
				else
				{
					bool flag = this.selectedOfficerGiver.GetState().isSpaceShipState && officer.ProposedTransferIsSwap(this.selectedOfficerGiver.GetState().ref_ship, this.<ProposeOfficerTransfer>g__ProposedOfficerTransfersToGiver|274_1());
					int num2 = this.plannedOfficerTransfers.Keys.Count<TIOfficerState>((TIOfficerState x) => this.selectedOfficerReceiver.GetOfficers().Contains(x));
					if (officer.CanTransferOfficer(this.selectedOfficerReceiver, this.selectedOfficerGiver, false, flag, num2))
					{
						this.plannedOfficerTransfers.Add(officer, this.selectedOfficerGiver);
						this.givingSideOfficerListItems[officer].gameObject.SetActive(true);
						this.receivingSideOfficerListItems[officer].gameObject.SetActive(false);
						this.givingSideOfficerListItems[officer].Colorize(Color.green);
						if (flag)
						{
							TIOfficerState tiofficerState4 = officer.ProposedOfficerSwap(this.selectedOfficerGiver.GetState().ref_ship, this.<ProposeOfficerTransfer>g__ProposedOfficerTransfersToGiver|274_1());
							if (tiofficerState4 != null)
							{
								if (this.plannedOfficerTransfers.ContainsKey(tiofficerState4))
								{
									this.plannedOfficerTransfers.Remove(tiofficerState4);
								}
								else
								{
									this.plannedOfficerTransfers.Add(tiofficerState4, this.selectedOfficerReceiver);
								}
								this.givingSideOfficerListItems[tiofficerState4].gameObject.SetActive(false);
								this.receivingSideOfficerListItems[tiofficerState4].gameObject.SetActive(true);
								this.receivingSideOfficerListItems[tiofficerState4].Colorize(Color.green);
							}
						}
					}
				}
			}
			this.SetOfficerTransferListItemsValid(this.selectedAssetOfficerList, this.selectedOfficerGiver, this.selectedOfficerReceiver);
			this.SetOfficerTransferListItemsValid(this.receivingAssetOfficerList, this.selectedOfficerReceiver, this.selectedOfficerGiver);
			if (this.plannedOfficerTransfers.Any<KeyValuePair<TIOfficerState, OfficerCarrierState>>())
			{
				this.paymentDropdownObject.SetActive(true);
				this.paymentDropdown.enabled = true;
				this.resourceCostOptions = TransferOfficersOperation.ResourceCostOptions(this.plannedOfficerTransfers);
				this.paymentDropdown.ClearOptions();
				foreach (TIResourcesCost tiresourcesCost in this.resourceCostOptions)
				{
					TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
					{
						text = tiresourcesCost.GetString("N1", false, true, false, 7, false, false, null, false, FactionResource.None)
					};
					this.paymentDropdown.options.Add(optionData);
				}
				this.selectedResourceCostOption = this.resourceCostOptions.FirstOrDefault<TIResourcesCost>();
				this.paymentDropdown.SetValueWithoutNotify(0);
				this.paymentDropdown.captionText.SetText(this.selectedResourceCostOption.GetString("N0", false, true, false, 7, false, false, null, false, FactionResource.None));
				return;
			}
			this.paymentDropdownObject.SetActive(false);
			this.paymentDropdown.enabled = false;
		}

		// Token: 0x060054D4 RID: 21716 RVA: 0x00268658 File Offset: 0x00266858
		public void OnMultiSelectArmiesSelected(MultiSelectArmiesSelected e)
		{
			this.OpenMultiArmyPanel();
			this.armyGroup.Clear();
			this.armyGroup.AddRange(e.armies);
			this.UpdateSelectedArmies();
		}

		// Token: 0x060054D5 RID: 21717 RVA: 0x00268684 File Offset: 0x00266884
		public void UpdateSelectedArmies()
		{
			this.armyList.SetListSize<OperationsArmyListItemController>(this.armyGroup.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.armyList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__280.<>p__0 == null)
					{
						OperationCanvasController.<>o__280.<>p__0 = CallSite<Func<CallSite, object, OperationsArmyListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OperationsArmyListItemController), typeof(OperationCanvasController)));
					}
					OperationsArmyListItemController operationsArmyListItemController = OperationCanvasController.<>o__280.<>p__0.Target(OperationCanvasController.<>o__280.<>p__0, enumerator.Current);
					operationsArmyListItemController.Initialize(this.armyGroup[num++]);
					operationsArmyListItemController.UpdateListItem((this.currentTargetArmyMoveFinalDestination != null && this.currentTargetArmyMoveFinalDestination.isRegionState) ? this.currentTargetArmyMoveFinalDestination.ref_region : null);
				}
			}
		}

		// Token: 0x060054D6 RID: 21718 RVA: 0x00268770 File Offset: 0x00266970
		public List<TIArmyState> GetSelectedArmies()
		{
			List<TIArmyState> list = new List<TIArmyState>();
			using (IEnumerator<object> enumerator = this.armyList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__281.<>p__0 == null)
					{
						OperationCanvasController.<>o__281.<>p__0 = CallSite<Func<CallSite, object, OperationsArmyListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OperationsArmyListItemController), typeof(OperationCanvasController)));
					}
					OperationsArmyListItemController operationsArmyListItemController = OperationCanvasController.<>o__281.<>p__0.Target(OperationCanvasController.<>o__281.<>p__0, enumerator.Current);
					if (operationsArmyListItemController.selectArmyToggle.isOn)
					{
						list.Add(operationsArmyListItemController.army);
					}
				}
			}
			return list;
		}

		// Token: 0x060054D7 RID: 21719 RVA: 0x00268818 File Offset: 0x00266A18
		public void AddArmyToMultiSelectGroup(TIArmyState armyToAdd)
		{
			if (!this.armyGroup.Contains(armyToAdd))
			{
				this.armyGroup.Add(armyToAdd);
				this.UpdateSelectedArmies();
			}
			else if (this.armyGroup.Contains(armyToAdd))
			{
				this.RemoveArmyFromMultiSelectGroup(armyToAdd);
			}
			if (this.armyGroup.Count > 1 && !this.multiSelectArmyCanvas.enabled && this.activeButton != null && this.activeButton.operationType is DeployArmyOperation)
			{
				this.OpenMultiArmyPanel();
			}
		}

		// Token: 0x060054D8 RID: 21720 RVA: 0x0026889D File Offset: 0x00266A9D
		public void RemoveArmyFromMultiSelectGroup(TIArmyState armyToRemove)
		{
			if (this.armyGroup.Contains(armyToRemove))
			{
				this.armyGroup.Remove(armyToRemove);
				this.UpdateSelectedArmies();
			}
		}

		// Token: 0x060054D9 RID: 21721 RVA: 0x002688C0 File Offset: 0x00266AC0
		private void UpdateMultiSelectedArmies()
		{
			using (IEnumerator<object> enumerator = this.armyList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OperationCanvasController.<>o__284.<>p__0 == null)
					{
						OperationCanvasController.<>o__284.<>p__0 = CallSite<Func<CallSite, object, OperationsArmyListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OperationsArmyListItemController), typeof(OperationCanvasController)));
					}
					OperationCanvasController.<>o__284.<>p__0.Target(OperationCanvasController.<>o__284.<>p__0, enumerator.Current).UpdateListItem((this.currentTargetArmyMoveFinalDestination != null && this.currentTargetArmyMoveFinalDestination.isRegionState) ? this.currentTargetArmyMoveFinalDestination.ref_region : null);
				}
			}
		}

		// Token: 0x060054DA RID: 21722 RVA: 0x00268974 File Offset: 0x00266B74
		public void OnClickCloseMultiArmyPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseMultiArmyPanel();
		}

		// Token: 0x060054DB RID: 21723 RVA: 0x00268988 File Offset: 0x00266B88
		public void CloseMultiArmyPanel()
		{
			this.armyList.SetListSize<OperationsArmyListItemController>(0, false, false);
			this.armyGroup.Clear();
			this.multiSelectArmyCanvas.enabled = false;
		}

		// Token: 0x060054DC RID: 21724 RVA: 0x002689AF File Offset: 0x00266BAF
		public void OpenMultiArmyPanel()
		{
			this.multiSelectArmyCanvas.enabled = true;
			this.UpdateMultiSelectedArmies();
		}

		// Token: 0x060054DD RID: 21725 RVA: 0x002689C3 File Offset: 0x00266BC3
		public bool CanSelectArmyGroup()
		{
			return this.activeButton != null && this.activeButton.operationType is DeployArmyOperation;
		}

		// Token: 0x060054E3 RID: 21731 RVA: 0x00268AB5 File Offset: 0x00266CB5
		[CompilerGenerated]
		private List<TIOfficerState> <ProposeOfficerTransfer>g__ProposedOfficerTransfersToReceiver|274_0()
		{
			return this.plannedOfficerTransfers.Keys.Where<TIOfficerState>((TIOfficerState x) => this.plannedOfficerTransfers[x] == this.selectedOfficerReceiver).ToList<TIOfficerState>();
		}

		// Token: 0x060054E5 RID: 21733 RVA: 0x00268AEE File Offset: 0x00266CEE
		[CompilerGenerated]
		private List<TIOfficerState> <ProposeOfficerTransfer>g__ProposedOfficerTransfersToGiver|274_1()
		{
			return this.plannedOfficerTransfers.Keys.Where<TIOfficerState>((TIOfficerState x) => this.plannedOfficerTransfers[x] == this.selectedOfficerGiver).ToList<TIOfficerState>();
		}

		// Token: 0x04003A89 RID: 14985
		private TIGameState actorState;

		// Token: 0x04003A8C RID: 14988
		public Canvas masterOperationCanvas;

		// Token: 0x04003A8D RID: 14989
		[Header("Operation Icons")]
		public GameObject iconsPanel;

		// Token: 0x04003A8E RID: 14990
		public ListManagerBase iconsGridManager;

		// Token: 0x04003A8F RID: 14991
		[Header("OperationData")]
		public GameObject hollowNamePanel;

		// Token: 0x04003A90 RID: 14992
		public TMP_Text operationName;

		// Token: 0x04003A91 RID: 14993
		public TMP_Text operationDescription;

		// Token: 0x04003A92 RID: 14994
		public Canvas operationMasterPanel;

		// Token: 0x04003A93 RID: 14995
		public GameObject operationInfoPanel;

		// Token: 0x04003A94 RID: 14996
		public GameObject confirmPanel;

		// Token: 0x04003A95 RID: 14997
		public TMP_Text confirmOperationName;

		// Token: 0x04003A96 RID: 14998
		public TMP_Text targetDisplayName;

		// Token: 0x04003A97 RID: 14999
		public Button confirmButton;

		// Token: 0x04003A98 RID: 15000
		public GameObject paymentDropdownObject;

		// Token: 0x04003A99 RID: 15001
		public TMP_Dropdown paymentDropdown;

		// Token: 0x04003A9A RID: 15002
		public GameObject targetNameObject;

		// Token: 0x04003A9B RID: 15003
		public GameObject targetDropdownObject;

		// Token: 0x04003A9C RID: 15004
		public TMP_Dropdown targetDropdown;

		// Token: 0x04003A9D RID: 15005
		public GameObject durationReportObject;

		// Token: 0x04003A9E RID: 15006
		public TMP_Text durationText;

		// Token: 0x04003A9F RID: 15007
		private bool selectingTarget;

		// Token: 0x04003AA0 RID: 15008
		private TIOperationTargeting currentTargeting;

		// Token: 0x04003AA1 RID: 15009
		private TIGameState currentTarget;

		// Token: 0x04003AA2 RID: 15010
		private TIGameState currentTargetArmyMoveFinalDestination;

		// Token: 0x04003AA3 RID: 15011
		private List<TIResourcesCost> resourceCostOptions;

		// Token: 0x04003AA4 RID: 15012
		private TIResourcesCost selectedResourceCostOption;

		// Token: 0x04003AA5 RID: 15013
		private OperationButtonController activeButton;

		// Token: 0x04003AA6 RID: 15014
		private float targetDropdownTemplateHeight;

		// Token: 0x04003AA7 RID: 15015
		[Header("Maximize Selection Panel")]
		public GameObject maximizeButtonGameObject;

		// Token: 0x04003AA8 RID: 15016
		[Header("Fleet Split/Scuttle Panel")]
		public TMP_Text currentFleetName;

		// Token: 0x04003AA9 RID: 15017
		public TMP_Text newFleetText;

		// Token: 0x04003AAA RID: 15018
		public GameObject fleetSplitPanel;

		// Token: 0x04003AAB RID: 15019
		public TMP_Text fleetSplitPanelHeader;

		// Token: 0x04003AAC RID: 15020
		public ListManagerBase originFleetList;

		// Token: 0x04003AAD RID: 15021
		public ListManagerBase newFleetList;

		// Token: 0x04003AAE RID: 15022
		public GameObject possibleTransitOrbit;

		// Token: 0x04003AAF RID: 15023
		[Header("TrajectoryChangePrompt")]
		public Canvas changeTrajectoryCanvas;

		// Token: 0x04003AB0 RID: 15024
		public TMP_Text changeTrajectoryPromptHeaderText;

		// Token: 0x04003AB1 RID: 15025
		public TMP_Text changeTrajectoryPromptMEssageText;

		// Token: 0x04003AB2 RID: 15026
		public TMP_Text changeTrajectoryPromptConfirmText;

		// Token: 0x04003AB3 RID: 15027
		public TMP_Text changeTrajectoryPromptCancelText;

		// Token: 0x04003AB4 RID: 15028
		public Button changeTrajectoryConfirmButton;

		// Token: 0x04003AB5 RID: 15029
		public Button changeTrajectoryCancelButton;

		// Token: 0x04003AB6 RID: 15030
		public bool changingInvalidTrajectory;

		// Token: 0x04003AB7 RID: 15031
		private TIFactionState changeTrajectoryFaction;

		// Token: 0x04003AB8 RID: 15032
		private TIGameState changeTrajectoryFleet;

		// Token: 0x04003AB9 RID: 15033
		private TIGameState changeTrajectoryTargetFleet;

		// Token: 0x04003ABA RID: 15034
		[Header("TutorialStuff")]
		public UITutorialController armiesUITutorialController;

		// Token: 0x04003ABB RID: 15035
		public UITutorialController spacebodyUITutorialController;

		// Token: 0x04003ABC RID: 15036
		public UITutorialController fleetOperationsUITutorialController;

		// Token: 0x04003ABD RID: 15037
		public UITutorialController fleetTransferTutorialController;

		// Token: 0x04003ABE RID: 15038
		public UITutorialController launchExofighterTutorialController;

		// Token: 0x04003ABF RID: 15039
		public GameObject launchExofighterHighlightDummy;

		// Token: 0x04003AC0 RID: 15040
		private IOperation operationTemplateForced;

		// Token: 0x04003AC1 RID: 15041
		[Header("Prefabs")]
		public TargetSelectionTool targetSelectionTool;

		// Token: 0x04003AC2 RID: 15042
		public ThrustProfileTool thrustProfileTool;

		// Token: 0x04003AC3 RID: 15043
		public List<TIGameState> QueuedTargets = new List<TIGameState>();

		// Token: 0x04003AC4 RID: 15044
		public List<List<TIGameState>> prospectiveQueuedTargets = new List<List<TIGameState>>();

		// Token: 0x04003AC5 RID: 15045
		public Dictionary<TIArmyState, List<TIRegionState>> prospectiveQueuedTargetsDictionary = new Dictionary<TIArmyState, List<TIRegionState>>();

		// Token: 0x04003AC6 RID: 15046
		private bool operationControlsDirty;

		// Token: 0x04003AC7 RID: 15047
		private Dictionary<int, TIGameState> targetOptionData;

		// Token: 0x04003AC8 RID: 15048
		private Dictionary<TIGameState, int> reverseTargetOptionData;

		// Token: 0x04003AC9 RID: 15049
		private List<TISpaceShipState> originFleetShips;

		// Token: 0x04003ACA RID: 15050
		private List<TISpaceShipState> newFleetShips;

		// Token: 0x04003ACB RID: 15051
		public Button splitAllDamagedButton;

		// Token: 0x04003ACC RID: 15052
		public Button resetSplitFleetPanelButton;

		// Token: 0x04003ACD RID: 15053
		public TMP_Text splitAllDamagedButtonText;

		// Token: 0x04003ACE RID: 15054
		public TMP_Text resetSplitFleetButtonText;

		// Token: 0x04003ACF RID: 15055
		private List<PropellantGroup> propellantsInFleet;

		// Token: 0x04003AD0 RID: 15056
		private int selectedPropellantGroupIdx;

		// Token: 0x04003AD1 RID: 15057
		[Header("PropellantSharing")]
		public GameObject propellantSharingPanel;

		// Token: 0x04003AD2 RID: 15058
		public ListManagerBase propellantTypeList;

		// Token: 0x04003AD3 RID: 15059
		public ListManagerBase availableGiversList;

		// Token: 0x04003AD4 RID: 15060
		public ListManagerBase selectedTakersList;

		// Token: 0x04003AD5 RID: 15061
		public ListManagerBase availableTakersList;

		// Token: 0x04003AD6 RID: 15062
		public Button ResetTakersButton;

		// Token: 0x04003AD7 RID: 15063
		public Button EqualizeDistributionButton;

		// Token: 0x04003AD8 RID: 15064
		public TooltipTrigger ResetTakersButtonTip;

		// Token: 0x04003AD9 RID: 15065
		public TooltipTrigger EqualizeDistributionButtonTip;

		// Token: 0x04003ADA RID: 15066
		public TMP_Text propellantSharingHeader;

		// Token: 0x04003ADB RID: 15067
		public TMP_Text selectPropellantHeader;

		// Token: 0x04003ADC RID: 15068
		public TMP_Text giverColumnHeader;

		// Token: 0x04003ADD RID: 15069
		public TMP_Text selectedTakerColumnHeader;

		// Token: 0x04003ADE RID: 15070
		public TMP_Text availableTakerColumnHeader;

		// Token: 0x04003ADF RID: 15071
		public TMP_Text sharePropellantInstructions;

		// Token: 0x04003AE0 RID: 15072
		public TMP_Text resetTakersButtonText;

		// Token: 0x04003AE1 RID: 15073
		public TMP_Text equalDistroButtonText;

		// Token: 0x04003AE2 RID: 15074
		public List<TISpaceShipState> availableGivers = new List<TISpaceShipState>();

		// Token: 0x04003AE3 RID: 15075
		public List<TISpaceShipState> selectedTakers = new List<TISpaceShipState>();

		// Token: 0x04003AE4 RID: 15076
		public List<TISpaceShipState> lockedTakers = new List<TISpaceShipState>();

		// Token: 0x04003AE5 RID: 15077
		public List<TISpaceShipState> availableTakers = new List<TISpaceShipState>();

		// Token: 0x04003AE6 RID: 15078
		public List<PropellantSharingEvent> propellantSharingEvents = new List<PropellantSharingEvent>();

		// Token: 0x04003AE7 RID: 15079
		[Header("TransferOfficers")]
		public Canvas transferOfficersCanvas;

		// Token: 0x04003AE8 RID: 15080
		public ListManagerBase givingAssetList;

		// Token: 0x04003AE9 RID: 15081
		public ListManagerBase selectedAssetOfficerList;

		// Token: 0x04003AEA RID: 15082
		public ListManagerBase receivingAssetOfficerList;

		// Token: 0x04003AEB RID: 15083
		public ListManagerBase receivingAssetList;

		// Token: 0x04003AEC RID: 15084
		public TMP_Text transferOfficerCanvasHeader;

		// Token: 0x04003AED RID: 15085
		public TMP_Text givingAssetHeader;

		// Token: 0x04003AEE RID: 15086
		public TMP_Text givingOfficerHeader;

		// Token: 0x04003AEF RID: 15087
		public TMP_Text receivingOfficerHeader;

		// Token: 0x04003AF0 RID: 15088
		public TMP_Text receivingAssetHeader;

		// Token: 0x04003AF1 RID: 15089
		public TMP_Text givingAssetOfficerCapacity;

		// Token: 0x04003AF2 RID: 15090
		public TMP_Text receivingAssetOfficerCapacity;

		// Token: 0x04003AF3 RID: 15091
		public TMP_Text transferOfficersConfirmButtonText;

		// Token: 0x04003AF4 RID: 15092
		public TMP_Text transferOfficersResetButtonText;

		// Token: 0x04003AF5 RID: 15093
		private Dictionary<TIOfficerState, OfficerCarrierState> plannedOfficerTransfers;

		// Token: 0x04003AF6 RID: 15094
		private List<OfficerCarrierState> officerTransferAssets;

		// Token: 0x04003AF7 RID: 15095
		public OfficerCarrierState selectedOfficerGiver;

		// Token: 0x04003AF8 RID: 15096
		public OfficerCarrierState selectedOfficerReceiver;

		// Token: 0x04003AF9 RID: 15097
		private Dictionary<TIOfficerState, TransferOfficerListItemController> givingSideOfficerListItems;

		// Token: 0x04003AFA RID: 15098
		private Dictionary<TIOfficerState, TransferOfficerListItemController> receivingSideOfficerListItems;

		// Token: 0x04003AFB RID: 15099
		[Header("Multi Select Army")]
		public Canvas multiSelectArmyCanvas;

		// Token: 0x04003AFC RID: 15100
		public TMP_Text multiSelectArmyHeaderText;

		// Token: 0x04003AFD RID: 15101
		public ListManagerBase armyList;

		// Token: 0x04003AFE RID: 15102
		public List<TIArmyState> armyGroup = new List<TIArmyState>();
	}
}
