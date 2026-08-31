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
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using TMPro.Examples;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200085F RID: 2143
	public class GeneralControlsController : CanvasControllerBase, IHud, ICanvas
	{
		// Token: 0x17000EBD RID: 3773
		// (get) Token: 0x06004EB9 RID: 20153 RVA: 0x0021E685 File Offset: 0x0021C885
		private float finderLargeUIScaleHeightFactor
		{
			get
			{
				return (float)TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] / 1080f;
			}
		}

		// Token: 0x17000EBE RID: 3774
		// (get) Token: 0x06004EBA RID: 20154 RVA: 0x0021E69E File Offset: 0x0021C89E
		// (set) Token: 0x06004EBB RID: 20155 RVA: 0x0021E6A5 File Offset: 0x0021C8A5
		[HideInInspector]
		public static bool UIPlayerInTargetingMode { get; private set; }

		// Token: 0x17000EBF RID: 3775
		// (get) Token: 0x06004EBC RID: 20156 RVA: 0x0021E6AD File Offset: 0x0021C8AD
		// (set) Token: 0x06004EBD RID: 20157 RVA: 0x0021E6B4 File Offset: 0x0021C8B4
		[HideInInspector]
		public static TITargeting UITargetingMode { get; private set; }

		// Token: 0x17000EC0 RID: 3776
		// (get) Token: 0x06004EBE RID: 20158 RVA: 0x0021E6BC File Offset: 0x0021C8BC
		// (set) Token: 0x06004EBF RID: 20159 RVA: 0x0021E6C3 File Offset: 0x0021C8C3
		[HideInInspector]
		public static TIGameState UISelectedAssetState { get; private set; }

		// Token: 0x17000EC1 RID: 3777
		// (get) Token: 0x06004EC0 RID: 20160 RVA: 0x0021E6CB File Offset: 0x0021C8CB
		// (set) Token: 0x06004EC1 RID: 20161 RVA: 0x0021E6D2 File Offset: 0x0021C8D2
		[HideInInspector]
		public static TIGameState UIOtherSelectedState { get; private set; }

		// Token: 0x17000EC2 RID: 3778
		// (get) Token: 0x06004EC2 RID: 20162 RVA: 0x0021E6DA File Offset: 0x0021C8DA
		// (set) Token: 0x06004EC3 RID: 20163 RVA: 0x0021E6E1 File Offset: 0x0021C8E1
		[HideInInspector]
		public static TIGameState UITargetedState { get; private set; }

		// Token: 0x17000EC3 RID: 3779
		// (get) Token: 0x06004EC4 RID: 20164 RVA: 0x0021E6E9 File Offset: 0x0021C8E9
		// (set) Token: 0x06004EC5 RID: 20165 RVA: 0x0021E6F0 File Offset: 0x0021C8F0
		public static GeneralControlsController Singleton { get; private set; }

		// Token: 0x17000EC4 RID: 3780
		// (get) Token: 0x06004EC6 RID: 20166 RVA: 0x0021E6F8 File Offset: 0x0021C8F8
		// (set) Token: 0x06004EC7 RID: 20167 RVA: 0x0021E6FF File Offset: 0x0021C8FF
		public static Sprite redReticle { get; private set; }

		// Token: 0x17000EC5 RID: 3781
		// (get) Token: 0x06004EC8 RID: 20168 RVA: 0x0021E707 File Offset: 0x0021C907
		// (set) Token: 0x06004EC9 RID: 20169 RVA: 0x0021E70E File Offset: 0x0021C90E
		public static Sprite cyanReticle { get; private set; }

		// Token: 0x17000EC6 RID: 3782
		// (get) Token: 0x06004ECA RID: 20170 RVA: 0x0021E716 File Offset: 0x0021C916
		// (set) Token: 0x06004ECB RID: 20171 RVA: 0x0021E71D File Offset: 0x0021C91D
		public static Sprite greenReticle { get; private set; }

		// Token: 0x06004ECC RID: 20172 RVA: 0x0021E728 File Offset: 0x0021C928
		public override void Initialize()
		{
			base.Initialize();
			GeneralControlsController.Singleton = this;
			this.cameraManager = World.Active.GetExistingManager<CameraManager>();
			if (this.notifications == null)
			{
				this.notifications = base.canvasManager.Notifications as NotificationScreenController;
			}
			this.targetingHeaderString.SetText(Loc.T("UI.GeneralControls.Targeting"));
			this.showAssignedCouncilorsText.SetText(Loc.T("UI.GeneralControls.ShowAssignedCouncilors"));
			this.resourceSaleHeader.SetText(Loc.T("UI.GeneralControls.SellResourcesHeader"));
			this.confirmSaleButtonText.SetText(Loc.T("UI.GeneralControls.ConfirmSale"));
			this.resetButtonText.SetText(Loc.T("UI.GeneralControls.ResetSale"));
			this.cancelButtonText.SetText(Loc.T("UI.GeneralControls.CancelSale"));
			this.totalSaleText.SetText(Loc.T("UI.GeneralControls.TotalSale"));
			this.resourceSalePanel.SetActive(false);
			this.targetingPanel.gameObject.SetActive(true);
			this.councilorChatAnimator.gameObject.SetActive(true);
			this.targetingPanel.enabled = false;
			this.speedTooltipTrigger.enabled = true;
			GameControl.eventManager.AddListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null, null, true, false);
			GameControl.eventManager.AddListener<MissionOptionsForTargetRequested>(new EventManager.EventDelegate<MissionOptionsForTargetRequested>(this.OnMissionOptionsForTargetRequested), null, null, true, false);
			GameControl.eventManager.AddListener<MyAssetPanelOpened>(new EventManager.EventDelegate<MyAssetPanelOpened>(this.OnAssetPanelOpened), null, null, false, false);
			GameControl.eventManager.AddListener<MyAssetPanelEntirelyClosed>(new EventManager.EventDelegate<MyAssetPanelEntirelyClosed>(this.OnAssetPanelClosed), null, null, false, false);
			GameControl.eventManager.AddListener<MyActiveAssetPanelResized>(new EventManager.EventDelegate<MyActiveAssetPanelResized>(this.OnAssetPanelResized), null, null, false, false);
			GameControl.eventManager.AddListener<GameStateNameChanged>(new EventManager.EventDelegate<GameStateNameChanged>(this.OnGameStateNameChanged), null, null, false, false);
			GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.MapViewChanged), null, null, false, false);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnMissionPhaseStart), "CouncilorMissionUpdate", null, true, false);
			GameControl.eventManager.AddListener<TimeEventComplete>(new EventManager.EventDelegate<TimeEventComplete>(this.OnMissionPhaseComplete), "CouncilorMissionUpdate", null, false, false);
			GameControl.eventManager.AddListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.OnCouncilCompositionChanged), null, null, true, false);
			GameControl.eventManager.AddListener<CouncilorMissionAssigned>(new EventManager.EventDelegate<CouncilorMissionAssigned>(this.OnCouncilorMissionAssigned), null, null, true, false);
			GameControl.eventManager.AddListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null, null, true, false);
			GameControl.eventManager.AddListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null, null, false, false);
			GameControl.eventManager.AddListener<BlockingPromptUpdated>(new EventManager.EventDelegate<BlockingPromptUpdated>(this.OnBlockingPromptUpdated), null, null, false, false);
			GameControl.eventManager.AddListener<PromptQueueCleared>(new EventManager.EventDelegate<PromptQueueCleared>(this.OnPromptQueueCleared), null, null, false, false);
			GameControl.eventManager.AddListener<MilestoneComplete>(new EventManager.EventDelegate<MilestoneComplete>(this.OnMilestoneCompleted), null, null, true, false);
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnHabConstructionStatusChanged), null, null, true, false);
			this.targetListForLocation = new List<TIGameState>();
			this.notificationQueue = GameStateManager.NotificationQueue();
			this.InitializeFinderList();
			GeneralControlsController.greenReticle = Resources.LoadAll<Sprite>("Square Reticle/GreenSquare/GreenSquareReticleSS")[0];
			GeneralControlsController.redReticle = Resources.LoadAll<Sprite>("Square Reticle/RedSquare/RedSquareReticleSS")[0];
			GeneralControlsController.cyanReticle = Resources.LoadAll<Sprite>("Square Reticle/CyanSquare/CyanSquareReticleSS")[0];
			this.earthButtonTooltip.SetText("BodyText", Loc.T("UI.GeneralControls.EarthButtonTooltip"));
			this.spaceButtonTooltip.SetText("BodyText", Loc.T("UI.GeneralControls.SpaceButtonTooltip"));
			this.councilButtonTooltip.SetText("BodyText", Loc.T("UI.GeneralControls.CouncilButtonTooltip"));
			this.nationsButtonTooltip.SetText("BodyText", Loc.T("UI.GeneralControls.NationsButtonTooltip"));
			this.habsButtonTooltip.SetText("BodyText", Loc.T("UI.GeneralControls.HabsButtonTooltip"));
			this.fleetsButtonTooltip.SetText("BodyText", Loc.T("UI.GeneralControls.FleetsButtonTooltip"));
			this.researchButtonTooltip.SetText("BodyText", Loc.T("UI.GeneralControls.ScienceButtonTooltip"));
			this.intelButtonTooltip.SetText("BodyText", Loc.T("UI.GeneralControls.IntelButtonTooltip"));
			this.eventSummaryButtonTooltipExpand.SetText("BodyText", Loc.T("UI.GeneralControls.EventsSummaryButtonTooltip"));
			this.eventSummaryButtonTooltipMinimize.SetText("BodyText", Loc.T("UI.GeneralControls.EventsSummaryButtonTooltip"));
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.Nations"));
			this.finderTitleText.SetText(Loc.T("UI.GeneralControls.FinderTitle"));
			this.globalSearchTextTitle.SetText(Loc.T("UI.GeneralControls.GlobalSearch"));
			this.playButtonComponent = this.PlayButton.GetComponent<Button>();
			GameControl.eventManager.AddListener<StartupComplete>(new EventManager.EventDelegate<StartupComplete>(this.OnStartupComplete), null, null, false, true);
			this.speedTooltipTrigger.SetDelegate("BodyText", () => this.SetSpeedTooltipTrigger());
			this.finderMaxHeight = 845f - (float)((TIUtilities.GetScreenRatio() > 2.3f) ? 90 : 0);
			GeneralControlsController.ShutdownUIGlobalTargetingMode(GameControl.control.activePlayer);
			this.UpdateMapColorDropDownOptions();
			this.RecolorEarthMap(base.activePlayer.mapColorationStyle);
			this.RefreshMilestoneUI();
			this.UpdateActivePlayerUIElements(true);
			this.missionPhaseReportButtonExpand.SetActive(true);
			this.missionPhaseReportButtonMinimize.SetActive(false);
			this.alienThreatPanel.SetActive(false);
			this.alienAlertAlienIcon.sprite = GameStateManager.AlienFaction().factionIcon64UI;
			this.alienAlertTip.SetDelegate("BodyText", () => this.alienThreatTip);
			this.TimePipsList.SetListSize<PipListItemController>(GameTimeManager.Singleton.currentSpeeds.Count - 1, false, false);
			if (TIPlayerProfileManager.displaySystemClock)
			{
				if (!this.SystemClockObject.activeInHierarchy)
				{
					this.SystemClockObject.SetActive(true);
				}
				this.SystemClockText.SetText(DateTime.Now.ToShortTimeString());
			}
			else if (this.SystemClockObject.activeInHierarchy)
			{
				this.SystemClockObject.SetActive(false);
			}
			this.UpdateResearchLeadersLights();
			this.InitializeAlarmPanel();
			this.searchObject.SetActive(true);
			CoroutineDummy.Singleton.StartCoroutine(this.HideSearchAfterInit());
			CoroutineDummy.Singleton.StartCoroutine(this.CheckSellResourcesTutorialStartup());
		}

		// Token: 0x06004ECD RID: 20173 RVA: 0x0021ED3C File Offset: 0x0021CF3C
		private void UpdateMapColorDropDownOptions()
		{
			this.mapModeDropdown.ClearOptions();
			TMP_Dropdown.OptionDataList optionDataList = new TMP_Dropdown.OptionDataList();
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.Nations")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.Terrain")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.FactionExecutive")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.FactionPopularity")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.Population")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.InvestmentPoints")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.PerCapitaGDP")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.ControlPoints")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.MilitaryTechLevel")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.BoostIncome")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.Unrest")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.Democracy")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.Sustainability")));
			if (base.activePlayer.MilestoneCompleted(CampaignMilestone.DetectXenoforming))
			{
				optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.XenoformingLevel")));
			}
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.IsFederatedNation")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.SelectedNationAlliances")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.SelectedNationClaims")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.SelectedNationFederation")));
			optionDataList.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.GeneralControls.MapMode.SelectedNationCanJoinFederation")));
			this.mapModeDropdown.AddOptions(optionDataList.options);
		}

		// Token: 0x06004ECE RID: 20174 RVA: 0x0021EF68 File Offset: 0x0021D168
		public override void UpdateActivePlayerUIElements(bool startup)
		{
			if (!startup)
			{
				GameControl.eventManager.RemoveListener<SellSpaceResourcesRequested>(new EventManager.EventDelegate<SellSpaceResourcesRequested>(this.OnSellSpaceResourcesRequested), null);
				GameControl.eventManager.RemoveListener<ArmyAssignedToFaction>(new EventManager.EventDelegate<ArmyAssignedToFaction>(this.OnArmyAssignedToFaction), null);
				GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnSectorAssignedToFaction), null);
				GameControl.eventManager.RemoveListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.OnFleetCoreStatusChanged), null);
				GameControl.eventManager.RemoveListener<AlienThreatUpdated>(new EventManager.EventDelegate<AlienThreatUpdated>(this.RefreshAlienThreatPanel), null);
			}
			this.factionIcon.sprite = base.activePlayer.factionIcon64UI;
			this.objectivesTooltipTrigger.SetText("BodyText", Loc.T("UI.GeneralControls.Objectives", new object[] { base.activePlayer.adjective }));
			GameControl.eventManager.AddListener<SellSpaceResourcesRequested>(new EventManager.EventDelegate<SellSpaceResourcesRequested>(this.OnSellSpaceResourcesRequested), null, base.activePlayer, false, false);
			GameControl.eventManager.AddListener<ArmyAssignedToFaction>(new EventManager.EventDelegate<ArmyAssignedToFaction>(this.OnArmyAssignedToFaction), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnSectorAssignedToFaction), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.OnFleetCoreStatusChanged), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<AlienThreatUpdated>(new EventManager.EventDelegate<AlienThreatUpdated>(this.RefreshAlienThreatPanel), null, base.activePlayer, true, false);
			this.SetResourceTooltipDelegates(base.activePlayer);
			TIInputManager.SetDefaultCursor(true);
			this.controlPointImage.color = base.activePlayer.template.color;
		}

		// Token: 0x06004ECF RID: 20175 RVA: 0x0021F0F7 File Offset: 0x0021D2F7
		public void OnStartupComplete(StartupComplete e)
		{
			if (TIPlayerProfileManager.uiScaleSetting > 0)
			{
				this.finderMaxHeight *= this.finderLargeUIScaleHeightFactor;
			}
			this.OpenFinderCanvas();
			this.UpdateSpeedText();
		}

		// Token: 0x06004ED0 RID: 20176 RVA: 0x0021F120 File Offset: 0x0021D320
		public override void Show()
		{
			base.Show();
			this.UpdateResourceData(base.activePlayer);
			this.finderRootCanvas.enabled = true;
			this.finderDataDirty = true;
			base.gameTime.SpeedChanged += this.UpdateSpeed;
			GameControl.eventManager.AddListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.UpdateResourceData), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.UpdateResourceData), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<ShipConstructionCompleted>(new EventManager.EventDelegate<ShipConstructionCompleted>(this.UpdateResourceData), null, base.activePlayer, true, false);
			this.RefreshAlienThreatPanel();
		}

		// Token: 0x06004ED1 RID: 20177 RVA: 0x0021F1CC File Offset: 0x0021D3CC
		public override void Hide()
		{
			base.Hide();
			base.gameTime.SpeedChanged -= this.UpdateSpeed;
			GameControl.eventManager.RemoveListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.UpdateResourceData), null);
			GameControl.eventManager.RemoveListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.UpdateResourceData), null);
			GameControl.eventManager.RemoveListener<ShipConstructionCompleted>(new EventManager.EventDelegate<ShipConstructionCompleted>(this.UpdateResourceData), null);
			this.alienThreatPanel.SetActive(false);
			this.finderCanvas.enabled = false;
			this.finderRootCanvas.enabled = false;
			this.targetingPanel.enabled = false;
		}

		// Token: 0x06004ED2 RID: 20178 RVA: 0x0021F26C File Offset: 0x0021D46C
		public override void Refresh()
		{
			if (TIFrameCounter.FrameCount % 17 == 0)
			{
				DateTime dateTime = base.gameTime.currentTime.ExportTime();
				if (dateTime != this.displayedSimTime)
				{
					this.displayedSimTime = dateTime;
					this.timeText.SetText(base.gameTime.currentTime.ToCustomTimeString());
					this.dateText.SetText(base.gameTime.currentTime.ToCustomDateString());
				}
				if (TIPlayerProfileManager.displaySystemClock)
				{
					if (!this.SystemClockObject.activeInHierarchy)
					{
						this.SystemClockObject.SetActive(true);
					}
					this.SystemClockText.SetText(DateTime.Now.ToShortTimeString());
				}
				else if (this.SystemClockObject.activeInHierarchy)
				{
					this.SystemClockObject.SetActive(false);
				}
			}
			if (TIFrameCounter.FrameCount % 1217 == 0)
			{
				this.CouncilorChat();
			}
			if (this.resourcesDataDirty)
			{
				this.UpdateResourceData(base.activePlayer);
				this.UpdateResearchLeadersLights();
				this.resourcesDataDirty = false;
			}
			if (this.finderDataDirty)
			{
				this.UpdateFinderList();
				this.finderDataDirty = false;
			}
			this.CheckKeys();
			if (Input.GetKey(KeyCode.LeftControl))
			{
				Input.GetKeyUp(KeyCode.U);
			}
		}

		// Token: 0x06004ED3 RID: 20179 RVA: 0x0021F398 File Offset: 0x0021D598
		public void Cleanup()
		{
			this.finderListModels.Clear();
			if (this.infoScreenOpen)
			{
				base.canvasManager.CloseActiveInfoScreen();
			}
			GeneralControlsController.UITargetingMode = null;
			GeneralControlsController.UISelectedAssetState = null;
			GeneralControlsController.UIOtherSelectedState = null;
			GeneralControlsController.UITargetedState = null;
		}

		// Token: 0x06004ED4 RID: 20180 RVA: 0x0021F3D0 File Offset: 0x0021D5D0
		public static bool ActivePlayerAsset(TIGameState state)
		{
			return state != null && (state.isCouncilorState || state.isArmyState || state.isSpaceFleetState) && state.ref_faction == GameControl.control.activePlayer;
		}

		// Token: 0x06004ED5 RID: 20181 RVA: 0x0021F40A File Offset: 0x0021D60A
		private static bool SelectableState(TIGameState state)
		{
			return state.isCouncilorState || state.isRegionState || state.isNationState || state.isArmyState || state.isSpaceObjectState || state.isRegionAlienEntity || state.isRegionSpaceFacility;
		}

		// Token: 0x06004ED6 RID: 20182 RVA: 0x0021F444 File Offset: 0x0021D644
		public static void SetSelectedState(TIGameState state, bool setTargetAsOther)
		{
			if (state == null || GeneralControlsController.SelectableState(state))
			{
				bool flag = GeneralControlsController.ActivePlayerAsset(state);
				if (GeneralControlsController.UIPlayerInTargetingMode && setTargetAsOther && !flag)
				{
					GeneralControlsController.SetUIOtherSelectedState(state);
					return;
				}
				if (flag)
				{
					GeneralControlsController.SetUISelectedAssetState(state);
					return;
				}
				GeneralControlsController.SetUIOtherSelectedState(state);
			}
		}

		// Token: 0x06004ED7 RID: 20183 RVA: 0x0021F48C File Offset: 0x0021D68C
		public static void SetUIGlobalTargetingMode(TIGameState state, TITargeting targetingMode)
		{
			GeneralControlsController.UIPlayerInTargetingMode = true;
			GeneralControlsController.UITargetingMode = targetingMode;
			GeneralControlsController.SetUITargetedState(state);
			if (!TIGlobalValuesState.isSpaceCombatEnabled)
			{
				TIInputManager.SetCursor(TIInputManager.targetCursorValid, true);
				return;
			}
			TIInputManager.SetCursor(TIInputManager.targetCursor, true);
		}

		// Token: 0x06004ED8 RID: 20184 RVA: 0x0021F4C0 File Offset: 0x0021D6C0
		public static void ShutdownUIGlobalTargetingMode(TIFactionState faction)
		{
			GeneralControlsController.UIPlayerInTargetingMode = false;
			GeneralControlsController.UITargetingMode = null;
			GeneralControlsController.SetUITargetedState(null);
			TIInputManager.inTargetingMode = false;
			if (GameControl.control.activePlayer == faction)
			{
				TIInputManager.SetCursor(TIInputManager.GetFactionCursor(GameControl.control.activePlayer.template.defaultPresetName), false);
			}
		}

		// Token: 0x06004ED9 RID: 20185 RVA: 0x0021F516 File Offset: 0x0021D716
		public static void ConditionalCancelSelectedAsset(TIGameState state)
		{
			if (GeneralControlsController.UISelectedAssetState == state)
			{
				GeneralControlsController.SetUISelectedAssetState(null);
			}
		}

		// Token: 0x06004EDA RID: 20186 RVA: 0x0021F52C File Offset: 0x0021D72C
		public static void SetUISelectedAssetState(TIGameState state)
		{
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			TIGameState uiselectedAssetState2 = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState2 != null && uiselectedAssetState2.isArmyState)
			{
				GeneralControlsController.UISelectedAssetState.ref_army.SetArmyDataDirty();
			}
			GeneralControlsController.UISelectedAssetState = state;
			TIGameState uiselectedAssetState3 = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState3 != null && uiselectedAssetState3.isArmyState)
			{
				GeneralControlsController.UISelectedAssetState.ref_army.SetArmyDataDirty();
			}
			if (uiselectedAssetState != null)
			{
				GameControl.eventManager.TriggerEvent(new CurrentAssetDeSelected(uiselectedAssetState, state), null, new object[] { uiselectedAssetState });
				if (uiselectedAssetState.isSpaceAssetState)
				{
					SpaceObjectSelection existingManager = World.Active.GetExistingManager<SpaceObjectSelection>();
					if (existingManager.spaceObjectStateSelected == uiselectedAssetState && !uiselectedAssetState.ref_spaceObject.controller.symbolController.isActiveAndEnabled && !uiselectedAssetState.ref_spaceObject.controller.modelLink.activeInHierarchy)
					{
						existingManager.SelectObject(uiselectedAssetState.ref_spaceObject.barycenter.gameObjectLink, false, false);
					}
				}
			}
			if (state != null)
			{
				GeneralControlsController generalControlsController = GameControl.canvasStack.StrategyHud as GeneralControlsController;
				if (generalControlsController != null)
				{
					generalControlsController.SetFinderIndex(state);
				}
			}
		}

		// Token: 0x06004EDB RID: 20187 RVA: 0x0021F641 File Offset: 0x0021D841
		public static void ConditionalCancelSelectedOtherState(TIGameState state)
		{
			if (GeneralControlsController.UIOtherSelectedState == state)
			{
				GeneralControlsController.SetUIOtherSelectedState(null);
			}
		}

		// Token: 0x06004EDC RID: 20188 RVA: 0x0021F658 File Offset: 0x0021D858
		public static void SetUIOtherSelectedState(TIGameState state)
		{
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			if (uiotherSelectedState != null)
			{
				if (uiotherSelectedState == state)
				{
					return;
				}
				if (GeneralControlsController.UIOtherSelectedState.isArmyState)
				{
					GeneralControlsController.UIOtherSelectedState.ref_army.SetArmyDataDirty();
				}
				else if (GeneralControlsController.UIOtherSelectedState.isRegionSpaceFacility)
				{
					GeneralControlsController.UIOtherSelectedState.ref_regionSpaceFacility.SetRegionEntityDataDirty();
				}
				else if (GeneralControlsController.UIOtherSelectedState.isRegionAlienEntity)
				{
					GeneralControlsController.UIOtherSelectedState.ref_regionAlienEntity.SetRegionEntityDataDirty();
				}
			}
			GeneralControlsController.UIOtherSelectedState = state;
			if (state != null)
			{
				if (GeneralControlsController.UIOtherSelectedState.isArmyState)
				{
					GeneralControlsController.UIOtherSelectedState.ref_army.SetArmyDataDirty();
				}
				else if (GeneralControlsController.UIOtherSelectedState.isRegionSpaceFacility)
				{
					GeneralControlsController.UIOtherSelectedState.ref_regionSpaceFacility.SetRegionEntityDataDirty();
				}
				else if (GeneralControlsController.UIOtherSelectedState.isRegionAlienEntity)
				{
					GeneralControlsController.UIOtherSelectedState.ref_regionAlienEntity.SetRegionEntityDataDirty();
				}
			}
			if (uiotherSelectedState != null && uiotherSelectedState != state)
			{
				GameControl.eventManager.TriggerEvent(new CurrentOtherStateDeselected(uiotherSelectedState, state), null, new object[] { uiotherSelectedState });
				if (uiotherSelectedState.isRegionState)
				{
					GameControl.eventManager.TriggerEvent(new CurrentOtherStateDeselected(uiotherSelectedState.ref_nation, state), null, new object[] { uiotherSelectedState.ref_nation });
				}
				else if (uiotherSelectedState.isSpaceAssetState)
				{
					SpaceObjectSelection existingManager = World.Active.GetExistingManager<SpaceObjectSelection>();
					if (existingManager.spaceObjectStateSelected == uiotherSelectedState && !uiotherSelectedState.ref_spaceObject.controller.symbolController.isActiveAndEnabled && !uiotherSelectedState.ref_spaceObject.controller.modelLink.activeInHierarchy)
					{
						existingManager.SelectObject(uiotherSelectedState.ref_spaceObject.barycenter.gameObjectLink, false, false);
					}
				}
			}
			if ((state == null && GeneralControlsController.IsMapColorationStyleNationMapMode(GeneralControlsController.mapColorationStyle)) || (state != null && !state.isRegionState && GeneralControlsController.IsMapColorationStyleNationMapMode(GeneralControlsController.mapColorationStyle)))
			{
				GeneralControlsController generalControlsController = GameControl.canvasStack.StrategyHud as GeneralControlsController;
				if (generalControlsController != null)
				{
					generalControlsController.suppressCycleMapModeAudio = true;
					generalControlsController.CycleRecolorEarthMap(true);
					generalControlsController.suppressCycleMapModeAudio = false;
				}
			}
			if (state != null && state.isRegionState)
			{
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
			}
			if (GeneralControlsController.IsMapColorationStyleNationMapMode(GeneralControlsController.mapColorationStyle) && state != null && state.isRegionState && state.ref_nation != null)
			{
				GeneralControlsController generalControlsController2 = GameControl.canvasStack.StrategyHud as GeneralControlsController;
				if (generalControlsController2 != null)
				{
					generalControlsController2.SetSelectedNationFlag(state.ref_nation);
				}
			}
		}

		// Token: 0x06004EDD RID: 20189 RVA: 0x0021F8E4 File Offset: 0x0021DAE4
		public static void SetUITargetedState(TIGameState state)
		{
			TIGameState uitargetedState = GeneralControlsController.UITargetedState;
			if (uitargetedState != null)
			{
				if (uitargetedState.isArmyState)
				{
					GeneralControlsController.UITargetedState.ref_army.SetArmyDataDirty();
				}
				else if (GeneralControlsController.UITargetedState.isRegionSpaceFacility)
				{
					GeneralControlsController.UITargetedState.ref_regionSpaceFacility.SetRegionEntityDataDirty();
				}
				else if (GeneralControlsController.UITargetedState.isRegionAlienEntity)
				{
					GeneralControlsController.UITargetedState.ref_regionAlienEntity.SetRegionEntityDataDirty();
				}
			}
			GeneralControlsController.UITargetedState = state;
			if (state != null)
			{
				if (GeneralControlsController.UITargetedState.isArmyState)
				{
					GeneralControlsController.UITargetedState.ref_army.SetArmyDataDirty();
				}
				else if (GeneralControlsController.UITargetedState.isRegionSpaceFacility)
				{
					GeneralControlsController.UITargetedState.ref_regionSpaceFacility.SetRegionEntityDataDirty();
				}
				else if (GeneralControlsController.UITargetedState.isRegionAlienEntity)
				{
					GeneralControlsController.UITargetedState.ref_regionAlienEntity.SetRegionEntityDataDirty();
				}
			}
			if (uitargetedState != null)
			{
				GameControl.eventManager.TriggerEvent(new CurrentTargetDetargeted(uitargetedState, state), null, new object[] { uitargetedState });
			}
		}

		// Token: 0x06004EDE RID: 20190 RVA: 0x0021F9DA File Offset: 0x0021DBDA
		public static void UpdateBlockedPause()
		{
			(World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController).UpdateSpeedText();
		}

		// Token: 0x06004EDF RID: 20191 RVA: 0x0021F9F5 File Offset: 0x0021DBF5
		public static bool CurrentlyTargetingStateType(Type stateType)
		{
			if (GeneralControlsController.UIPlayerInTargetingMode)
			{
				TITargeting uitargetingMode = GeneralControlsController.UITargetingMode;
				return uitargetingMode != null && uitargetingMode.TargetedGameStates().Contains(stateType);
			}
			return false;
		}

		// Token: 0x06004EE0 RID: 20192 RVA: 0x0021FA18 File Offset: 0x0021DC18
		public static bool CurrentValidTarget(TIGameState state)
		{
			if (GeneralControlsController.UIPlayerInTargetingMode)
			{
				TITargeting uitargetingMode = GeneralControlsController.UITargetingMode;
				bool? flag;
				if (uitargetingMode == null)
				{
					flag = null;
				}
				else
				{
					IList<TIGameState> getPossibleTargets = uitargetingMode.GetPossibleTargets;
					flag = ((getPossibleTargets != null) ? new bool?(getPossibleTargets.Contains(state)) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
			return false;
		}

		// Token: 0x06004EE1 RID: 20193 RVA: 0x0021FA69 File Offset: 0x0021DC69
		public static bool IsMapColorationStyleNationMapMode(MapColorationStyle style)
		{
			return style == MapColorationStyle.bySelectedNationAlliances || style == MapColorationStyle.bySelectedNationCanJoinFederation || style == MapColorationStyle.bySelectedNationClaims || style == MapColorationStyle.bySelectedNationFederation;
		}

		// Token: 0x06004EE2 RID: 20194 RVA: 0x0021FA84 File Offset: 0x0021DC84
		public void MainMenu()
		{
			if (base.canvasManager.OptionsScreen.Visible())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_UnPause", false, false);
				base.canvasManager.OptionsScreen.Hide();
				TIInputManager.acceptingInput = true;
				CoroutineDummy.Singleton.UnpauseAll();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_Pause", false, false);
			base.canvasManager.OptionsScreen.Show();
			CodexController.HideCodexPanel();
			TIInputManager.acceptingInput = false;
			CoroutineDummy.Singleton.PauseAll();
		}

		// Token: 0x06004EE3 RID: 20195 RVA: 0x0021FB01 File Offset: 0x0021DD01
		public void SolarSystem()
		{
			base.canvasManager.CloseActiveInfoScreen();
			GameControl.control.viewMgr.GotoView(ViewType.SolarSystem);
			this.cameraManager.Zoom(1047185094900.0, true);
		}

		// Token: 0x06004EE4 RID: 20196 RVA: 0x0021FB33 File Offset: 0x0021DD33
		public void PoliticalMap()
		{
			base.canvasManager.CloseActiveInfoScreen();
			GameControl.control.viewMgr.GotoView(ViewType.PoliticalMap);
		}

		// Token: 0x06004EE5 RID: 20197 RVA: 0x0021FB50 File Offset: 0x0021DD50
		public void Nations()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
			base.canvasManager.ToggleInfoScreen<NationsScreenController>();
		}

		// Token: 0x06004EE6 RID: 20198 RVA: 0x0021FB69 File Offset: 0x0021DD69
		public void Councilors()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
			base.canvasManager.ToggleInfoScreen<CouncilGridController>();
		}

		// Token: 0x06004EE7 RID: 20199 RVA: 0x0021FB82 File Offset: 0x0021DD82
		public void Research()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
			base.canvasManager.ToggleInfoScreen<ResearchScreenController>();
		}

		// Token: 0x06004EE8 RID: 20200 RVA: 0x0021FB9B File Offset: 0x0021DD9B
		public void Intel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
			base.canvasManager.ToggleInfoScreen<IntelScreenController>();
		}

		// Token: 0x06004EE9 RID: 20201 RVA: 0x0021FBB4 File Offset: 0x0021DDB4
		public void Habitats()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
			base.canvasManager.ToggleInfoScreen<HabitatsScreenController>();
		}

		// Token: 0x06004EEA RID: 20202 RVA: 0x0021FBD0 File Offset: 0x0021DDD0
		public void Fleets()
		{
			if (!FleetsScreenController.gotoConstructionManager && !FleetsScreenController.gotoDesigner)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
			}
			if (!base.canvasManager.IsShowingInfoScreen<FleetsScreenController>())
			{
				base.canvasManager.ToggleInfoScreen<FleetsScreenController>();
				return;
			}
			if (FleetsScreenController.gotoDesigner)
			{
				(base.canvasManager.ActiveInfoScreen as FleetsScreenController).OnDesignShipButtonFromFleetListClicked();
				FleetsScreenController.gotoDesigner = false;
				return;
			}
			if (FleetsScreenController.gotoConstructionManager)
			{
				(base.canvasManager.ActiveInfoScreen as FleetsScreenController).OpenConstructionManager();
				FleetsScreenController.gotoConstructionManager = false;
				return;
			}
			if (base.canvasManager.IsShowingInfoScreen<FleetsScreenController>())
			{
				base.canvasManager.ToggleInfoScreen<FleetsScreenController>();
			}
		}

		// Token: 0x06004EEB RID: 20203 RVA: 0x0021FC6E File Offset: 0x0021DE6E
		public void Objectives()
		{
			if (!base.canvasManager.OptionsScreen.Visible())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
				base.canvasManager.ToggleInfoScreen<ObjectivesScreenController>();
			}
		}

		// Token: 0x06004EEC RID: 20204 RVA: 0x0021FC9C File Offset: 0x0021DE9C
		public void PauseSpeed()
		{
			if (this.NotificationDelayingSpeedChange())
			{
				return;
			}
			if (base.gameTime.TogglePause())
			{
				if (base.gameTime.currentSpeedIndex == 0)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_Pause", false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_UnPause", false, false);
				}
				this.UpdateSpeedText();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004EED RID: 20205 RVA: 0x0021FCF9 File Offset: 0x0021DEF9
		private IEnumerator DelayedUnpauseAssignmentPhaseEnd()
		{
			yield return null;
			this.PauseSpeed();
			yield break;
		}

		// Token: 0x06004EEE RID: 20206 RVA: 0x0021FD08 File Offset: 0x0021DF08
		public void PauseSpeedNoToggle()
		{
			base.gameTime.Pause();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_Pause", false, false);
			this.UpdateSpeedText();
		}

		// Token: 0x06004EEF RID: 20207 RVA: 0x0021FD27 File Offset: 0x0021DF27
		public void SetSpeed(int speedIndex)
		{
			if (this.NotificationDelayingSpeedChange())
			{
				return;
			}
			base.gameTime.SetSpeed(speedIndex, true);
		}

		// Token: 0x06004EF0 RID: 20208 RVA: 0x0021FD3F File Offset: 0x0021DF3F
		public void IncreaseSpeed()
		{
			if (base.gameTime.IncreaseSpeed())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SpeedUp", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			this.UpdateSpeedText();
		}

		// Token: 0x06004EF1 RID: 20209 RVA: 0x0021FD6E File Offset: 0x0021DF6E
		public void DecreaseSpeed()
		{
			if (base.gameTime.DecreaseSpeed())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SlowDown", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			this.UpdateSpeedText();
		}

		// Token: 0x06004EF2 RID: 20210 RVA: 0x0021FDA0 File Offset: 0x0021DFA0
		private bool NotificationDelayingSpeedChange()
		{
			if (this.notifications == null)
			{
				this.notifications = base.canvasManager.Notifications as NotificationScreenController;
			}
			return this.notifications.singleAlertBox.activeInHierarchy && Time.time <= this.notifications.notificationPushTime + TemplateManager.global.notificationReceiveInputDelay;
		}

		// Token: 0x06004EF3 RID: 20211 RVA: 0x0021FE03 File Offset: 0x0021E003
		private void OnGameTimeSpeedChanged(GameTimeSpeedChanged e)
		{
			this.UpdateSpeedText();
		}

		// Token: 0x06004EF4 RID: 20212 RVA: 0x0021FE0B File Offset: 0x0021E00B
		private void OnBlockingPromptUpdated(BlockingPromptUpdated e)
		{
			this.UpdateSpeedText();
		}

		// Token: 0x06004EF5 RID: 20213 RVA: 0x0021FE13 File Offset: 0x0021E013
		private void OnPromptQueueCleared(PromptQueueCleared e)
		{
			this.UpdateSpeedText();
		}

		// Token: 0x06004EF6 RID: 20214 RVA: 0x0021FE1C File Offset: 0x0021E01C
		private void OnHabConstructionStatusChanged(HabModuleConstructionStatusChange e)
		{
			if (e.habModule.moduleTemplate == null)
			{
				return;
			}
			if (e.habModule.moduleTemplate.coreModule && e.habModule.constructionCompleted && e.hab.ref_faction == GameControl.control.activePlayer && e.hab.IsStation && e.hab.orbitState.interfaceOrbit && e.hab.orbitState.ref_spaceBody.isEarth)
			{
				this.sellResourcesTutorialController.HoldTutorial(CampaignMilestone.UITutorial_GeneralControlsCanvas_SellResources, false, true);
			}
		}

		// Token: 0x06004EF7 RID: 20215 RVA: 0x0021FEBA File Offset: 0x0021E0BA
		private IEnumerator CheckSellResourcesTutorialStartup()
		{
			yield return new WaitUntil(() => GameControl.loadcycle100);
			yield return new WaitForSeconds(3f);
			if (base.activePlayer.CanSellSpaceResourcesOnEarth)
			{
				this.sellResourcesTutorialController.HoldTutorial(CampaignMilestone.UITutorial_GeneralControlsCanvas_SellResources, false, true);
			}
			yield break;
		}

		// Token: 0x06004EF8 RID: 20216 RVA: 0x0021FEC9 File Offset: 0x0021E0C9
		private void UpdateSpeed(SpeedSetting speed)
		{
			this.UpdateSpeedText();
		}

		// Token: 0x06004EF9 RID: 20217 RVA: 0x0021FED4 File Offset: 0x0021E0D4
		private void UpdateSpeedText()
		{
			SpeedSetting currentSpeedSetting = base.gameTime.CurrentSpeedSetting;
			if (currentSpeedSetting.multiplier == 0f)
			{
				this.speedText.text = currentSpeedSetting.description;
			}
			else
			{
				this.speedText.text = "";
			}
			int num = 1;
			using (IEnumerator<object> enumerator = this.TimePipsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GeneralControlsController.<>o__233.<>p__0 == null)
					{
						GeneralControlsController.<>o__233.<>p__0 = CallSite<Func<CallSite, object, PipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PipListItemController), typeof(GeneralControlsController)));
					}
					PipListItemController pipListItemController = GeneralControlsController.<>o__233.<>p__0.Target(GeneralControlsController.<>o__233.<>p__0, enumerator.Current);
					if (currentSpeedSetting.multiplier != 0f)
					{
						if (base.gameTime.currentSpeedIndex < num)
						{
							pipListItemController.SetPipStatus(false, false);
						}
						else
						{
							pipListItemController.SetPipStatus(true, false);
						}
					}
					else if (base.gameTime.lastSpeedIndex < num)
					{
						pipListItemController.SetPipStatus(false, false);
					}
					else
					{
						pipListItemController.SetPipStatus(true, base.gameTime.isBlockedByPrompt);
					}
					num++;
				}
			}
			if (base.gameTime.CurrentSpeedSetting.multiplier == 0f)
			{
				this.PauseButton.SetActive(false);
				this.PlayButton.SetActive(true);
				if (base.gameTime.isBlockedByPrompt)
				{
					this.playButtonComponent.interactable = false;
					this.pauseBlockedImage.gameObject.SetActive(true);
					this.speedText.color = TIUtilities.UIColorIndicatorNegative;
				}
				if (!base.gameTime.isBlockedByPrompt)
				{
					this.playButtonComponent.interactable = true;
					this.pauseBlockedImage.gameObject.SetActive(false);
					this.speedText.color = TIUtilities.UIColorIndicatorNeutral;
					return;
				}
			}
			else
			{
				this.PauseButton.SetActive(true);
				this.PlayButton.SetActive(false);
			}
		}

		// Token: 0x06004EFA RID: 20218 RVA: 0x002200BC File Offset: 0x0021E2BC
		private string SetSpeedTooltipTrigger()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (TIPromptQueueState.anyBlockingPrompt)
			{
				stringBuilder.AppendLine(TIPromptQueueState.GetBlockingDetailStr());
			}
			if (!TIMissionPhaseState.InMissionPhase())
			{
				TIDateTime nextMissionPhase = TIMissionPhaseState.nextMissionPhase;
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.NextMissionPhase", new object[] { nextMissionPhase.ToCustomDateString() }));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004EFB RID: 20219 RVA: 0x00220118 File Offset: 0x0021E318
		public void ToggleFPSWidget()
		{
			if (base.gameObject.GetComponent<TMP_FrameRateCounter>() != null)
			{
				if (base.gameObject.GetComponent<TMP_FrameRateCounter>().enabled)
				{
					base.gameObject.GetComponent<TMP_FrameRateCounter>().Clear();
					base.gameObject.GetComponent<TMP_FrameRateCounter>().enabled = false;
					return;
				}
				if (!base.gameObject.GetComponent<TMP_FrameRateCounter>().enabled)
				{
					base.gameObject.GetComponent<TMP_FrameRateCounter>().enabled = true;
					return;
				}
			}
		}

		// Token: 0x06004EFC RID: 20220 RVA: 0x00220190 File Offset: 0x0021E390
		public void ToggleMapColorControlPanel()
		{
			if (this.mapColorControlPanel.activeSelf)
			{
				this.mapColorControlPanel.SetActive(false);
				return;
			}
			if (!this.mapColorControlPanel.activeSelf)
			{
				this.mapColorControlPanel.SetActive(true);
			}
		}

		// Token: 0x06004EFD RID: 20221 RVA: 0x002201C8 File Offset: 0x0021E3C8
		public void CycleRecolorEarthMap(bool forward = true)
		{
			if (!this.suppressCycleMapModeAudio)
			{
				AudioManager.PlayOneShot(forward ? "event:/SFX/UI_SFX/trig_SFX_CycleForward" : "event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			Dictionary<MapColorationStyle, Action> dictionary = new Dictionary<MapColorationStyle, Action>();
			dictionary[MapColorationStyle.byNation] = new Action(this.SetColorMapByNation);
			dictionary[MapColorationStyle.byPopulation] = new Action(this.SetColorMapByPopulation);
			dictionary[MapColorationStyle.byInvestmentPoints] = new Action(this.SetColorMapByInvestmentPoints);
			dictionary[MapColorationStyle.byPerCapitaGDP] = new Action(this.SetColorMapByPerCapitaGDP);
			dictionary[MapColorationStyle.byControlPoints] = new Action(this.SetColorMapByControlPoints);
			dictionary[MapColorationStyle.byMilitaryTechLevel] = new Action(this.SetColorMapByMilitaryTechLevel);
			dictionary[MapColorationStyle.byBoostIncome] = new Action(this.SetColorMapByBoostIncome);
			dictionary[MapColorationStyle.byUnrest] = new Action(this.SetColorMapByUnrest);
			dictionary[MapColorationStyle.byDemocracy] = new Action(this.SetColorMapByDemocracy);
			dictionary[MapColorationStyle.bySustainability] = new Action(this.SetColorMapBySustainability);
			dictionary[MapColorationStyle.byXenoformingLevel] = new Action(this.SetColorMapByXenoformingLevel);
			dictionary[MapColorationStyle.byFactionPopularity] = new Action(this.SetColorMapByFactionPopularity);
			dictionary[MapColorationStyle.byExecutiveFaction] = new Action(this.SetColorMapByExecutiveFaction);
			dictionary[MapColorationStyle.byTerrain] = new Action(this.SetColorMapByTerrain);
			dictionary[MapColorationStyle.byIsFederatedNation] = new Action(this.SetColorMapByIsFederatedNation);
			dictionary[MapColorationStyle.bySelectedNationAlliances] = new Action(this.SetColorMapBySelectedNationAlliances);
			dictionary[MapColorationStyle.bySelectedNationClaims] = new Action(this.SetColorMapBySelectedNationClaims);
			dictionary[MapColorationStyle.bySelectedNationFederation] = new Action(this.SetColorMapBySelectedNationFederation);
			dictionary[MapColorationStyle.bySelectedNationCanJoinFederation] = new Action(this.SetColorMapBySelectedNationCanJoinFederation);
			List<MapColorationStyle> list = dictionary.Keys.ToList<MapColorationStyle>();
			int num = list.IndexOf(GeneralControlsController.mapColorationStyle);
			if (forward)
			{
				num++;
				if (num >= list.Count)
				{
					num = 0;
				}
				if (list[num] == MapColorationStyle.byXenoformingLevel && !GameControl.control.activePlayer.MilestoneCompleted(CampaignMilestone.DetectXenoforming))
				{
					num++;
					if (num >= list.Count)
					{
						num = 0;
					}
				}
				if ((list[num] == MapColorationStyle.bySelectedNationAlliances || list[num] == MapColorationStyle.bySelectedNationClaims || list[num] == MapColorationStyle.bySelectedNationFederation || list[num] == MapColorationStyle.bySelectedNationCanJoinFederation) && (GeneralControlsController.UIOtherSelectedState == null || !GeneralControlsController.UIOtherSelectedState.isRegionState))
				{
					num += 4;
					if (num >= list.Count)
					{
						num = 0;
					}
				}
			}
			else
			{
				num--;
				if (num < 0)
				{
					num = list.Count - 1;
				}
				if (list[num] == MapColorationStyle.byXenoformingLevel && !GameControl.control.activePlayer.MilestoneCompleted(CampaignMilestone.DetectXenoforming))
				{
					num--;
					if (num < 0)
					{
						num = list.Count - 1;
					}
				}
				if ((list[num] == MapColorationStyle.bySelectedNationAlliances || list[num] == MapColorationStyle.bySelectedNationClaims || list[num] == MapColorationStyle.bySelectedNationFederation || list[num] == MapColorationStyle.bySelectedNationCanJoinFederation) && (GeneralControlsController.UIOtherSelectedState == null || !GeneralControlsController.UIOtherSelectedState.isRegionState))
				{
					num -= 4;
					if (num < 0)
					{
						num = list.Count - 1;
					}
				}
			}
			dictionary[list[num]]();
			base.activePlayer.mapColorationStyle = list[num];
			if (GeneralControlsController.IsMapColorationStyleNationMapMode(base.activePlayer.mapColorationStyle))
			{
				this.MapModeFlagObject.SetActive(true);
				this.MapModeEarthObject.SetActive(false);
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				this.SetSelectedNationFlag((uiotherSelectedState != null) ? uiotherSelectedState.ref_nation : null);
			}
			else
			{
				this.MapModeEarthObject.SetActive(true);
				this.MapModeFlagObject.SetActive(false);
			}
			GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
		}

		// Token: 0x06004EFE RID: 20222 RVA: 0x0022055C File Offset: 0x0021E75C
		private void RecolorEarthMap(MapColorationStyle colorMode)
		{
			switch (colorMode)
			{
			case MapColorationStyle.byNation:
				this.SetColorMapByNation();
				break;
			case MapColorationStyle.byTerrain:
				this.SetColorMapByTerrain();
				break;
			case MapColorationStyle.byExecutiveFaction:
				this.SetColorMapByExecutiveFaction();
				break;
			case MapColorationStyle.byFactionPopularity:
				this.SetColorMapByFactionPopularity();
				break;
			case MapColorationStyle.byPopulation:
				this.SetColorMapByPopulation();
				break;
			case MapColorationStyle.byInvestmentPoints:
				this.SetColorMapByInvestmentPoints();
				break;
			case MapColorationStyle.byPerCapitaGDP:
				this.SetColorMapByPerCapitaGDP();
				break;
			case MapColorationStyle.byControlPoints:
				this.SetColorMapByControlPoints();
				break;
			case MapColorationStyle.byMilitaryTechLevel:
				this.SetColorMapByMilitaryTechLevel();
				break;
			case MapColorationStyle.byBoostIncome:
				this.SetColorMapByBoostIncome();
				break;
			case MapColorationStyle.byUnrest:
				this.SetColorMapByUnrest();
				break;
			case MapColorationStyle.byDemocracy:
				this.SetColorMapByDemocracy();
				break;
			case MapColorationStyle.bySustainability:
				this.SetColorMapBySustainability();
				break;
			case MapColorationStyle.byXenoformingLevel:
				this.SetColorMapByXenoformingLevel();
				break;
			case MapColorationStyle.byIsFederatedNation:
				this.SetColorMapByIsFederatedNation();
				break;
			case MapColorationStyle.bySelectedNationAlliances:
				this.SetColorMapBySelectedNationAlliances();
				break;
			case MapColorationStyle.bySelectedNationClaims:
				this.SetColorMapBySelectedNationClaims();
				break;
			case MapColorationStyle.bySelectedNationFederation:
				this.SetColorMapBySelectedNationFederation();
				break;
			case MapColorationStyle.bySelectedNationCanJoinFederation:
				this.SetColorMapBySelectedNationCanJoinFederation();
				break;
			}
			base.activePlayer.mapColorationStyle = GeneralControlsController.mapColorationStyle;
		}

		// Token: 0x06004EFF RID: 20223 RVA: 0x0022066C File Offset: 0x0021E86C
		public void OnDropdownChangedMapColorMode(int mapMode)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (!base.activePlayer.MilestoneCompleted(CampaignMilestone.DetectXenoforming) && mapMode > 12)
			{
				mapMode++;
			}
			this.RecolorEarthMap((MapColorationStyle)mapMode);
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			this.SetSelectedNationFlag((uiotherSelectedState != null) ? uiotherSelectedState.ref_nation : null);
			GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
		}

		// Token: 0x06004F00 RID: 20224 RVA: 0x002206DA File Offset: 0x0021E8DA
		private void UpdateMapDropDownValue(int mapMode)
		{
			if (!base.activePlayer.MilestoneCompleted(CampaignMilestone.DetectXenoforming) && mapMode > 12)
			{
				mapMode--;
			}
			this.mapModeDropdown.SetValueWithoutNotify(mapMode);
		}

		// Token: 0x06004F01 RID: 20225 RVA: 0x00220701 File Offset: 0x0021E901
		public void SetColorMapByNation()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.Nations"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byNation;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F02 RID: 20226 RVA: 0x00220729 File Offset: 0x0021E929
		public void SetColorMapByTerrain()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.Terrain"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byTerrain;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F03 RID: 20227 RVA: 0x00220751 File Offset: 0x0021E951
		public void SetColorMapByExecutiveFaction()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.FactionExecutive"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byExecutiveFaction;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F04 RID: 20228 RVA: 0x00220779 File Offset: 0x0021E979
		public void SetColorMapByFactionPopularity()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.FactionPopularity"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byFactionPopularity;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F05 RID: 20229 RVA: 0x002207A1 File Offset: 0x0021E9A1
		public void SetColorMapByPopulation()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.Population"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byPopulation;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F06 RID: 20230 RVA: 0x002207C9 File Offset: 0x0021E9C9
		public void SetColorMapByInvestmentPoints()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.InvestmentPoints"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byInvestmentPoints;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F07 RID: 20231 RVA: 0x002207F1 File Offset: 0x0021E9F1
		public void SetColorMapByPerCapitaGDP()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.PerCapitaGDP"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byPerCapitaGDP;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F08 RID: 20232 RVA: 0x00220819 File Offset: 0x0021EA19
		public void SetColorMapByControlPoints()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.ControlPoints"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byControlPoints;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F09 RID: 20233 RVA: 0x00220841 File Offset: 0x0021EA41
		public void SetColorMapByMilitaryTechLevel()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.MilitaryTechLevel"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byMilitaryTechLevel;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F0A RID: 20234 RVA: 0x00220869 File Offset: 0x0021EA69
		public void SetColorMapByBoostIncome()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.BoostIncome"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byBoostIncome;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F0B RID: 20235 RVA: 0x00220892 File Offset: 0x0021EA92
		public void SetColorMapByUnrest()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.Unrest"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byUnrest;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F0C RID: 20236 RVA: 0x002208BB File Offset: 0x0021EABB
		public void SetColorMapByDemocracy()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.Democracy"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byDemocracy;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F0D RID: 20237 RVA: 0x002208E4 File Offset: 0x0021EAE4
		public void SetColorMapBySustainability()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.Sustainability"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.bySustainability;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F0E RID: 20238 RVA: 0x0022090D File Offset: 0x0021EB0D
		public void SetColorMapByXenoformingLevel()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.XenoformingLevel"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byXenoformingLevel;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F0F RID: 20239 RVA: 0x00220936 File Offset: 0x0021EB36
		public void SetColorMapByIsFederatedNation()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.IsFederatedNation"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.byIsFederatedNation;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F10 RID: 20240 RVA: 0x0022095F File Offset: 0x0021EB5F
		public void SetColorMapBySelectedNationAlliances()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.SelectedNationAlliances"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.bySelectedNationAlliances;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F11 RID: 20241 RVA: 0x00220988 File Offset: 0x0021EB88
		public void SetColorMapBySelectedNationClaims()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.SelectedNationClaims"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.bySelectedNationClaims;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F12 RID: 20242 RVA: 0x002209B1 File Offset: 0x0021EBB1
		public void SetColorMapBySelectedNationFederation()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.SelectedNationFederation"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.bySelectedNationFederation;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F13 RID: 20243 RVA: 0x002209DA File Offset: 0x0021EBDA
		public void SetColorMapBySelectedNationCanJoinFederation()
		{
			this.mapColorDescText.SetText(Loc.T("UI.GeneralControls.MapMode.SelectedNationCanJoinFederation"));
			GeneralControlsController.mapColorationStyle = MapColorationStyle.bySelectedNationCanJoinFederation;
			this.UpdateMapDropDownValue((int)GeneralControlsController.mapColorationStyle);
		}

		// Token: 0x06004F14 RID: 20244 RVA: 0x00220A04 File Offset: 0x0021EC04
		public void SetSelectedNationFlag(TINationState nation)
		{
			if (nation != null && GeneralControlsController.IsMapColorationStyleNationMapMode(GeneralControlsController.mapColorationStyle))
			{
				this.MapModeFlagSprite.sprite = nation.flag;
				this.MapModeFlagObject.SetActive(true);
				this.MapModeEarthObject.SetActive(false);
				return;
			}
			this.MapModeFlagObject.SetActive(false);
			this.MapModeEarthObject.SetActive(true);
		}

		// Token: 0x06004F15 RID: 20245 RVA: 0x00220A68 File Offset: 0x0021EC68
		private void MapViewChanged(MapActivationChangedEvent e)
		{
			this.ToggleMapColorControlPanel();
			if (!e.active && !GeneralControlsController.UIPlayerInTargetingMode && !TIGlobalValuesState.isSpaceCombatEnabled)
			{
				TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
				if (uiselectedAssetState == null || !uiselectedAssetState.isSpaceObjectState)
				{
					TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
					if (uiotherSelectedState == null || !uiotherSelectedState.isSpaceObjectState)
					{
						InfoPanel activeInfoPanel = base.canvasManager.GetActiveInfoPanel();
						if (activeInfoPanel <= InfoPanel.NationDetail)
						{
							if (activeInfoPanel != InfoPanel.None)
							{
								if (activeInfoPanel != InfoPanel.NationDetail)
								{
									return;
								}
								if ((base.canvasManager.NationInfo as NationInfoController).nation.ref_spaceBody.isEarth)
								{
									TIUtilities.GotoGameState(GameStateManager.Earth(), false, true, !TIMissionPhaseState.InMissionPhase(), false, true, -1f);
									return;
								}
								return;
							}
						}
						else if (activeInfoPanel != InfoPanel.CouncilorDetail)
						{
							if (activeInfoPanel != InfoPanel.EarthMapObjectDetail)
							{
								return;
							}
						}
						else
						{
							if ((base.canvasManager.CouncilorMissionController as CouncilorMissionCanvasController).enemyCouncilor.OnEarth)
							{
								TIUtilities.GotoGameState(GameStateManager.Earth(), false, true, !TIMissionPhaseState.InMissionPhase(), false, true, -1f);
								return;
							}
							return;
						}
						SpaceObjectSelection existingManager = World.Active.GetExistingManager<SpaceObjectSelection>();
						if (!existingManager.HasSelection || existingManager.spaceObjectStateSelected.isEarth)
						{
							TIUtilities.GotoGameState(GameStateManager.Earth(), false, true, !TIMissionPhaseState.InMissionPhase(), false, true, -1f);
							return;
						}
					}
				}
			}
		}

		// Token: 0x06004F16 RID: 20246 RVA: 0x00220BA5 File Offset: 0x0021EDA5
		public void ToggleOrbitTrails()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			GameControl.solarSystem.ToggleOrbitTrails();
		}

		// Token: 0x06004F17 RID: 20247 RVA: 0x00220BBD File Offset: 0x0021EDBD
		public void ToggleDistantSymbols()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			GameControl.solarSystem.ToggleDistantSymbols();
		}

		// Token: 0x06004F18 RID: 20248 RVA: 0x00220BD5 File Offset: 0x0021EDD5
		public void ToggleProspectData()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			GameControl.solarSystem.ToggleProspectData();
		}

		// Token: 0x06004F19 RID: 20249 RVA: 0x00220BED File Offset: 0x0021EDED
		public void ToggleShowAllColonizedNames()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			GameControl.solarSystem.ToggleShowAllColonizedBodyNames();
		}

		// Token: 0x06004F1A RID: 20250 RVA: 0x00220C05 File Offset: 0x0021EE05
		public void OnClickResources()
		{
			if (!this.resourceSalePanel.activeInHierarchy && base.activePlayer.CanSellSpaceResourcesOnEarth)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				this.ShowResourcesPanel();
			}
		}

		// Token: 0x06004F1B RID: 20251 RVA: 0x00220C33 File Offset: 0x0021EE33
		private bool IsButtonClickable(Button button)
		{
			return button.gameObject.activeInHierarchy && button.enabled && button.interactable && button.GetComponentInParent<Canvas>().enabled;
		}

		// Token: 0x06004F1C RID: 20252 RVA: 0x00220C60 File Offset: 0x0021EE60
		public void CheckKeys()
		{
			if (TIInputManager.acceptingInput && !GameControl.handlingException)
			{
				if (this.notifications == null)
				{
					this.notifications = base.canvasManager.Notifications as NotificationScreenController;
				}
				if (Input.GetKeyDown(KeyCode.Escape) && !this.notifications.cinematicObject.activeSelf)
				{
					CodexController.HideCodexPanel();
					if (this.infoScreenOpen)
					{
						base.canvasManager.CloseActiveInfoScreen();
					}
					else if (this.notifications.singleAlertBox.activeInHierarchy && Time.time > this.notifications.notificationPushTime + TemplateManager.global.notificationReceiveInputDelay)
					{
						if (this.IsButtonClickable(this.notifications.closeButton))
						{
							this.notifications.CloseButtonPressed();
						}
						else if (this.IsButtonClickable(this.notifications.okayButton))
						{
							this.notifications.OkayButtonPressed();
						}
					}
					else if (this.resourceSalePanel.activeInHierarchy)
					{
						this.OnCloseSellResourcesPanelClicked();
					}
					else if (this.notifications.IsSummaryPanelOpen())
					{
						this.notifications.CloseSummaryReportPanel();
					}
					else if (base.canvasManager.GetActiveInfoPanel() != InfoPanel.None)
					{
						bool flag = false;
						InfoPanel activeInfoPanel = base.canvasManager.GetActiveInfoPanel();
						if (activeInfoPanel != InfoPanel.NationDetail)
						{
							if (activeInfoPanel - InfoPanel.SpaceBodyDetail > 6)
							{
							}
						}
						else
						{
							flag = (base.canvasManager.NationInfo as NationInfoController).CloseAnySecondaryPanels(null, false);
							TooltipManager.Instance.HideAll();
						}
						if (!flag)
						{
							base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
						}
					}
					else if (base.canvasManager.GetActiveAssetPanel() != AssetPanel.None)
					{
						if (GeneralControlsController.UIPlayerInTargetingMode)
						{
							AssetPanel activeAssetPanel = base.canvasManager.GetActiveAssetPanel();
							if (activeAssetPanel != AssetPanel.MyCouncilor)
							{
								if (activeAssetPanel - AssetPanel.MyArmy <= 1)
								{
									(base.canvasManager.OperationCanvasController as OperationCanvasController).Shutdown();
								}
							}
							else
							{
								(base.canvasManager.CouncilorMissionController as CouncilorMissionCanvasController).OnClickCloseTargetSelectionButton();
							}
						}
						else
						{
							base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
						}
					}
					else
					{
						this.MainMenu();
					}
				}
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					NotificationScreenController notificationScreenController = base.canvasManager.Notifications as NotificationScreenController;
					if (!notificationScreenController.cinematicObject.activeSelf && notificationScreenController.singleAlertBox.activeInHierarchy)
					{
						if (this.IsButtonClickable(notificationScreenController.okayButton))
						{
							notificationScreenController.OkayButtonPressed();
						}
						else if (this.IsButtonClickable(notificationScreenController.closeButton))
						{
							notificationScreenController.CloseButtonPressed();
						}
					}
				}
				if (Input.GetKeyUp(KeyCode.Tab) && !(GeneralControlsController.UITargetingMode is TIMissionTargeting) && !(GeneralControlsController.UITargetingMode is TIOperationTargeting))
				{
					if (TIInputManager.IsShiftKeyDown)
					{
						this.FinderCycleBackward();
					}
					else
					{
						this.FinderCycleForward();
					}
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.Objectives, TIInputManager.KeyPressMode.Down))
				{
					this.Objectives();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.PoliticalEarth, TIInputManager.KeyPressMode.Down))
				{
					this.PoliticalMap();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.SolarSystem, TIInputManager.KeyPressMode.Down))
				{
					this.SolarSystem();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.Councilors, TIInputManager.KeyPressMode.Down))
				{
					this.Councilors();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.Nations, TIInputManager.KeyPressMode.Down))
				{
					this.Nations();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.Habitats, TIInputManager.KeyPressMode.Down))
				{
					this.Habitats();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.Fleets, TIInputManager.KeyPressMode.Down))
				{
					this.Fleets();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.OpenShipDesigner, TIInputManager.KeyPressMode.Down))
				{
					if (FleetsScreenController.CanDesignShips(base.activePlayer, false))
					{
						FleetsScreenController.gotoDesigner = true;
						this.Fleets();
					}
					else
					{
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
					}
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.OpenConstructionManager, TIInputManager.KeyPressMode.Down))
				{
					FleetsScreenController.gotoConstructionManager = true;
					this.Fleets();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.Research, TIInputManager.KeyPressMode.Down))
				{
					this.Research();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.Intel, TIInputManager.KeyPressMode.Down))
				{
					this.Intel();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.CycleRecolorEarthMap, TIInputManager.KeyPressMode.Down))
				{
					this.CycleRecolorEarthMap(true);
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.ToggleOrbitTrails, TIInputManager.KeyPressMode.Down))
				{
					this.ToggleOrbitTrails();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.IncreaseSpeed, TIInputManager.KeyPressMode.Up))
				{
					this.IncreaseSpeed();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.DecreaseSpeed, TIInputManager.KeyPressMode.Up))
				{
					this.DecreaseSpeed();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.PauseSpeedNoToggle, TIInputManager.KeyPressMode.Up))
				{
					this.PauseSpeedNoToggle();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.PauseSpeed, TIInputManager.KeyPressMode.Up))
				{
					this.PauseSpeed();
				}
				if (!TIInputManager.receivingInputForNarrativeHotkeys)
				{
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.SetSpeedIndex1, TIInputManager.KeyPressMode.Up))
					{
						this.SetSpeed(1);
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.SetSpeedIndex2, TIInputManager.KeyPressMode.Up))
					{
						this.SetSpeed(2);
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.SetSpeedIndex3, TIInputManager.KeyPressMode.Up))
					{
						this.SetSpeed(3);
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.SetSpeedIndex4, TIInputManager.KeyPressMode.Up))
					{
						this.SetSpeed(4);
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.SetSpeedIndex5, TIInputManager.KeyPressMode.Up))
					{
						this.SetSpeed(5);
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.SetSpeedIndex6, TIInputManager.KeyPressMode.Up))
					{
						this.SetSpeed(6);
					}
				}
				if (Input.GetKeyUp(KeyCode.Alpha1))
				{
					(base.canvasManager.Notifications as NotificationScreenController).OnOptionButtonPressed(0);
				}
				if (Input.GetKeyUp(KeyCode.Alpha2))
				{
					(base.canvasManager.Notifications as NotificationScreenController).OnOptionButtonPressed(1);
				}
				if (Input.GetKeyUp(KeyCode.Alpha3))
				{
					(base.canvasManager.Notifications as NotificationScreenController).OnOptionButtonPressed(2);
				}
				if (Input.GetKeyUp(KeyCode.Alpha4))
				{
					(base.canvasManager.Notifications as NotificationScreenController).OnOptionButtonPressed(3);
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.ToggleExpandNewsFeed, TIInputManager.KeyPressMode.Up))
				{
					(base.canvasManager.Notifications as NotificationScreenController).ToggleExpandedNewsFeed();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.ToggleDistanceSymbols, TIInputManager.KeyPressMode.Up))
				{
					this.ToggleDistantSymbols();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.ToggleProspectData, TIInputManager.KeyPressMode.Up))
				{
					this.ToggleProspectData();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.ToggleShowAllColonizedBodyNames, TIInputManager.KeyPressMode.Up))
				{
					this.ToggleShowAllColonizedNames();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.QuickSave, TIInputManager.KeyPressMode.Down))
				{
					if (!SaveMenuController.SavingIsBlocked())
					{
						GameStateManager.SaveAllGameStates(StartMenuController.quickSaveFilepath, false);
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
						this.OnChatButtonClicked();
						base.StartCoroutine(this.SendQuicksaveNotification());
					}
					else
					{
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
					}
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.ToggleHelper, TIInputManager.KeyPressMode.Down))
				{
					this.OnFinderButtonSelected();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.toggleFPSWidget, TIInputManager.KeyPressMode.Up))
				{
					this.ToggleFPSWidget();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.OpenGlobalSearch, TIInputManager.KeyPressMode.Down))
				{
					this.ShowGlobalSearchPanel();
					return;
				}
			}
			else if (Input.GetKeyDown(KeyCode.Escape) && base.canvasManager.OptionsScreen.Visible() && !GameControl.handlingException)
			{
				this.MainMenu();
			}
		}

		// Token: 0x06004F1D RID: 20253 RVA: 0x00221278 File Offset: 0x0021F478
		public void DebugToggleUI()
		{
			if (base.canvasManager.StrategyHud.Visible())
			{
				base.canvasManager.StrategyHud.Hide();
				base.canvasManager.Notifications.Hide();
				return;
			}
			base.canvasManager.StrategyHud.Show();
			base.canvasManager.Notifications.Show();
		}

		// Token: 0x06004F1E RID: 20254 RVA: 0x002212D8 File Offset: 0x0021F4D8
		public void RestoreHiddenUI()
		{
			if (!base.canvasManager.StrategyHud.Visible())
			{
				base.canvasManager.StrategyHud.Show();
				base.canvasManager.Notifications.Show();
			}
		}

		// Token: 0x06004F1F RID: 20255 RVA: 0x0022130C File Offset: 0x0021F50C
		private void OnInfoScreenOpened(InfoScreenOpened e)
		{
			this.storedFinderStatus = this.finderCanvas.enabled;
			this.EnableFinderCanvas(false);
			this.DisableTargetingPanel();
			this.infoScreenOpen = true;
			GameControl.eventManager.RemoveListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null);
			GameControl.eventManager.AddListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.OnInfoScreenClosed), null, null, false, false);
		}

		// Token: 0x06004F20 RID: 20256 RVA: 0x00221370 File Offset: 0x0021F570
		private void OnInfoScreenClosed(InfoScreenClosed e)
		{
			this.EnableFinderCanvas(this.storedFinderStatus);
			this.infoScreenOpen = false;
			GameControl.eventManager.AddListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null, null, false, false);
			GameControl.eventManager.RemoveListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.OnInfoScreenClosed), null);
		}

		// Token: 0x06004F21 RID: 20257 RVA: 0x002213C1 File Offset: 0x0021F5C1
		private void OnCouncilCompositionChanged(CouncilCompositionChanged e)
		{
			if (e.council == base.activePlayer)
			{
				this.finderDataDirty = true;
				if (this.targetingPanel.enabled)
				{
					this.RefreshTargetingPanel();
				}
			}
		}

		// Token: 0x06004F22 RID: 20258 RVA: 0x002213F0 File Offset: 0x0021F5F0
		private void OnCouncilorMissionAssigned(CouncilorMissionAssigned e)
		{
			if (e.councilor.faction == base.activePlayer && this.targetingPanel.enabled)
			{
				this.RefreshTargetingPanel();
			}
		}

		// Token: 0x06004F23 RID: 20259 RVA: 0x0022141D File Offset: 0x0021F61D
		private void OnArmyAssignedToFaction(ArmyAssignedToFaction e)
		{
			this.finderDataDirty = true;
		}

		// Token: 0x06004F24 RID: 20260 RVA: 0x00221426 File Offset: 0x0021F626
		private void OnSectorAssignedToFaction(SectorAssignedToFaction e)
		{
			this.finderDataDirty = true;
		}

		// Token: 0x06004F25 RID: 20261 RVA: 0x0022142F File Offset: 0x0021F62F
		private void OnFleetCoreStatusChanged(FleetCoreStatusChange e)
		{
			this.finderDataDirty = true;
		}

		// Token: 0x06004F26 RID: 20262 RVA: 0x00221438 File Offset: 0x0021F638
		private void OnMissionOptionsForTargetRequested(MissionOptionsForTargetRequested e)
		{
			GameControl.eventManager.ClearPendingEvents(e, null, Array.Empty<object>());
			if (!(e.target != null))
			{
				this.targetingPanel.enabled = false;
				return;
			}
			TIGameState tigameState = TIUtilities.ObjectToExactLocation(e.target);
			if (tigameState == null)
			{
				Log.Error(e.target.templateName, Array.Empty<object>());
				this.targetingPanel.enabled = false;
				return;
			}
			if (e.target == this.originalTarget)
			{
				if (tigameState.isRegionState && this.targetListForLocation.Count > 0)
				{
					this.GetNextTargetableGameStateInRegion(this.targetListForLocation);
				}
			}
			else
			{
				this.currentTarget = e.target;
				TIGameState tigameState2 = this.currentTarget;
				if (this.originalTarget != null)
				{
					tigameState2 = TIUtilities.ObjectToExactLocation(this.originalTarget);
				}
				this.originalTarget = this.currentTarget;
				if (this.currentTarget.isRegionState)
				{
					this.targetListForLocation = this.GetTargetableGameStatesInRegion(this.currentTarget.ref_region);
				}
				else if (tigameState2 != tigameState && tigameState.isRegionState)
				{
					this.targetListForLocation = this.GetTargetableGameStatesInRegion(tigameState.ref_region);
				}
			}
			this.targetingPanel.enabled = true;
			if (this.targetingPanelMaxItems < 3)
			{
				base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
			}
			this.RefreshTargetingPanel();
		}

		// Token: 0x06004F27 RID: 20263 RVA: 0x0022159C File Offset: 0x0021F79C
		public void OnFinderButtonSelected()
		{
			this.DisableTargetingPanel();
			if (base.canvasManager.IsShowingInfoScreen())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenFinder", false, false);
				base.canvasManager.CloseActiveInfoScreen();
				return;
			}
			if (this.finderCanvas.enabled)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseFinder", false, false);
				this.storedFinderStatus = false;
				this.EnableFinderCanvas(false);
			}
			else
			{
				if (this.finderMaxItems < 3)
				{
					base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenFinder", false, false);
				this.OpenFinderCanvas();
			}
			TIUtilities.UpdateButtonSpritesPlusMinus(this.finderMinimizeButton, this.finderCanvas.enabled, false);
		}

		// Token: 0x06004F28 RID: 20264 RVA: 0x00221640 File Offset: 0x0021F840
		public void RefreshMilestoneUI()
		{
			foreach (TIObjectiveTemplate tiobjectiveTemplate in base.activePlayer.GetObjectivesByTypeAndStatus(ObjectiveType.Tutorial, ObjectiveStatus.Unlocked))
			{
				if (!base.activePlayer.milestones.Contains(base.activePlayer.GetMileStoneFromObjective(tiobjectiveTemplate)) && (tiobjectiveTemplate.isChildObjective || TIObjectiveTemplate.HasChildMilestone(base.activePlayer, tiobjectiveTemplate)))
				{
					this.milestoneText.SetText(new StringBuilder(TemplateManager.global.tutorialInlineSpritePath).Append(tiobjectiveTemplate.displayName(base.activePlayer)).ToString());
					this.milestoneTooltip.SetText("BodyText", new StringBuilder(TemplateManager.global.tutorialInlineSpritePath).Append(tiobjectiveTemplate.milestoneDescription(base.activePlayer)).ToString());
					this.milestonePanel.SetActive(true);
					return;
				}
			}
			this.milestonePanel.SetActive(false);
		}

		// Token: 0x06004F29 RID: 20265 RVA: 0x00221750 File Offset: 0x0021F950
		public void OnMilestoneCompleted(MilestoneComplete e)
		{
			this.RefreshMilestoneUI();
			if (e.milestone == CampaignMilestone.DetectXenoforming)
			{
				this.UpdateMapColorDropDownOptions();
			}
		}

		// Token: 0x06004F2A RID: 20266 RVA: 0x00221768 File Offset: 0x0021F968
		public void OnOpenTutorialClicked(int index)
		{
			if (this.isHoldingTutorial && this.heldTutorialItem.Count > index)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				GeneralControlsController.HeldTutorialItem heldTutorialItem = this.heldTutorialItem[index];
				if (heldTutorialItem == null)
				{
					return;
				}
				UITutorialController uitutorialController = ((heldTutorialItem != null) ? heldTutorialItem.heldTutorialController : null);
				if (uitutorialController == null)
				{
					return;
				}
				heldTutorialItem.heldTutorialController.ShowTutorialTips(heldTutorialItem.heldTutorialMilestone, heldTutorialItem.heldTutorialOverrideMilestone, heldTutorialItem.heldTutorialNextFrame);
				this.ClearHeldTutorial(uitutorialController);
			}
		}

		// Token: 0x06004F2B RID: 20267 RVA: 0x002217E4 File Offset: 0x0021F9E4
		public void OnCloseTutorialClicked(int index)
		{
			if (UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_Intro)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			if (this.isHoldingTutorial && this.heldTutorialItem.Count > index)
			{
				UITutorialController.SetTutorialMilestone(this.heldTutorialItem[index].heldTutorialMilestone);
				this.heldTutorialItem[index].heldTutorialController.CompleteTutorial(true);
			}
		}

		// Token: 0x06004F2C RID: 20268 RVA: 0x00221850 File Offset: 0x0021FA50
		public void HoldUITutorial(UITutorialController controller, CampaignMilestone milestone, bool overrideMilestone, bool nextFrame)
		{
			int num = (((float)Screen.width / (float)Screen.height < 1.75f) ? 2 : 3);
			if (TIPlayerProfileManager.uiScaleSetting > 0)
			{
				num--;
			}
			if (this.heldTutorialItem.Count >= num)
			{
				return;
			}
			using (List<GeneralControlsController.HeldTutorialItem>.Enumerator enumerator = this.heldTutorialItem.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.heldTutorialController == controller)
					{
						return;
					}
				}
			}
			GeneralControlsController.HeldTutorialItem heldTutorialItem = new GeneralControlsController.HeldTutorialItem
			{
				heldTutorialController = controller,
				heldTutorialMilestone = milestone,
				heldTutorialOverrideMilestone = overrideMilestone,
				heldTutorialNextFrame = nextFrame
			};
			this.heldTutorialItem.Add(heldTutorialItem);
			this.isHoldingTutorial = true;
			this.RefreshHeldTutorialTooltips();
		}

		// Token: 0x06004F2D RID: 20269 RVA: 0x0022191C File Offset: 0x0021FB1C
		public void ClearHeldTutorial(UITutorialController controller)
		{
			if (this.heldTutorialItem.Count == 0)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < this.heldTutorialItem.Count; i++)
			{
				if (this.heldTutorialItem[i].heldTutorialController == controller)
				{
					this.heldTutorialItem.Remove(this.heldTutorialItem[i]);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			if (this.heldTutorialItem.Count == 0)
			{
				this.isHoldingTutorial = false;
			}
			for (int j = 0; j < this.openTutorialObject.Count; j++)
			{
				if (j > this.heldTutorialItem.Count - 1)
				{
					this.openTutorialObject[j].SetActive(false);
				}
			}
			this.RefreshHeldTutorialTooltips();
		}

		// Token: 0x06004F2E RID: 20270 RVA: 0x002219DC File Offset: 0x0021FBDC
		private void RefreshHeldTutorialTooltips()
		{
			for (int i = 0; i < this.heldTutorialItem.Count; i++)
			{
				StringBuilder stringBuilder = new StringBuilder(this.heldTutorialItem[i].heldTutorialController.uiTutorialTip[0].tipLOCName).Append("Short");
				this.openTutorialObject[i].GetComponentInChildren<TooltipTrigger>().SetText("BodyText", Loc.T(stringBuilder.ToString()));
				this.tutorialDescriptorText[i].SetText(Loc.T(stringBuilder.ToString()));
				this.openTutorialObject[i].SetActive(true);
			}
		}

		// Token: 0x06004F2F RID: 20271 RVA: 0x00221A8C File Offset: 0x0021FC8C
		public void Tutorial_HighlightIntroOptin()
		{
			GameObject gameObject = null;
			if (this.heldTutorialItem != null && this.heldTutorialItem.Count > 0)
			{
				for (int i = 0; i < this.heldTutorialItem.Count; i++)
				{
					if (this.heldTutorialItem[i].heldTutorialMilestone == CampaignMilestone.UITutorial_GeneralControlsCanvas && this.openTutorialObject[i].activeInHierarchy)
					{
						gameObject = this.openTutorialObject[i];
						break;
					}
				}
			}
			if (gameObject != null)
			{
				RectTransform rectTransform = this.introOptinHighlightDummy.transform as RectTransform;
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

		// Token: 0x06004F30 RID: 20272 RVA: 0x00221B57 File Offset: 0x0021FD57
		public void EnableFinderCanvas(bool setting)
		{
			this.finderCanvas.enabled = setting;
			if (setting)
			{
				this.DisableTargetingPanel();
			}
		}

		// Token: 0x06004F31 RID: 20273 RVA: 0x00221B6E File Offset: 0x0021FD6E
		public void OpenFinderCanvas()
		{
			this.UpdateFinderList();
			this.EnableFinderCanvas(true);
		}

		// Token: 0x06004F32 RID: 20274 RVA: 0x00221B7D File Offset: 0x0021FD7D
		private void OnMissionPhaseStart(TimeEventStart e)
		{
			if (TIPlayerProfileManager.missionPhaseReportStartOpen)
			{
				(base.canvasManager.Notifications as NotificationScreenController).ToggleSummaryLogPanel(SummaryCategory.None, false);
			}
			base.StartCoroutine(this.WaitFiveAndCall());
		}

		// Token: 0x06004F33 RID: 20275 RVA: 0x00221BAA File Offset: 0x0021FDAA
		private IEnumerator WaitFiveAndCall()
		{
			yield return this.fewSecs;
			this.CouncilorChat();
			yield break;
		}

		// Token: 0x06004F34 RID: 20276 RVA: 0x00221BBC File Offset: 0x0021FDBC
		private void OnMissionPhaseComplete(TimeEventComplete e)
		{
			this.DisableTargetingPanel();
			this.missionPhaseReportButtonExpand.SetActive(true);
			this.missionPhaseReportButtonMinimize.SetActive(false);
			(base.canvasManager.Notifications as NotificationScreenController).summaryLogReportObject.SetActive(false);
			if (TIPlayerProfileManager.unpauseAfterMissionAssignment)
			{
				base.StartCoroutine(this.DelayedUnpauseAssignmentPhaseEnd());
			}
			List<TIFactionState.Advice> list = new List<TIFactionState.Advice>
			{
				TIFactionState.Advice.CouncilorTargetedByEnemyMission,
				TIFactionState.Advice.FactionTargetedByEnemyMission
			};
			foreach (TICouncilorState ticouncilorState in base.activePlayer.activeCouncilors)
			{
				TIFactionState.AdviceData adviceData = TIFactionState.GetAdvice(ticouncilorState, 1, new List<TIFactionState.Advice>(list)).FirstOrDefault<TIFactionState.AdviceData>();
				if (!string.IsNullOrEmpty(adviceData.adviceText))
				{
					GameStateManager.NotificationQueue().councilorMessages.Enqueue(new CouncilorMessage(ticouncilorState, adviceData.adviceText));
					if (adviceData.adviceType == TIFactionState.Advice.FactionTargetedByEnemyMission)
					{
						list.Remove(adviceData.adviceType);
					}
				}
			}
		}

		// Token: 0x06004F35 RID: 20277 RVA: 0x00221CC0 File Offset: 0x0021FEC0
		public void OnClickToggleMissionPhaseReport()
		{
			(base.canvasManager.Notifications as NotificationScreenController).ToggleSummaryLogPanel(SummaryCategory.None, true);
		}

		// Token: 0x06004F36 RID: 20278 RVA: 0x00221CDC File Offset: 0x0021FEDC
		public void SetSummaryLogReportButton()
		{
			if ((base.canvasManager.Notifications as NotificationScreenController).summaryLogReportObject.activeSelf)
			{
				this.missionPhaseReportButtonExpand.SetActive(false);
				this.missionPhaseReportButtonMinimize.SetActive(true);
				return;
			}
			this.missionPhaseReportButtonExpand.SetActive(true);
			this.missionPhaseReportButtonMinimize.SetActive(false);
		}

		// Token: 0x06004F37 RID: 20279 RVA: 0x00221D36 File Offset: 0x0021FF36
		private void OnSellSpaceResourcesRequested(SellSpaceResourcesRequested e)
		{
			this.ShowResourcesPanel();
		}

		// Token: 0x06004F38 RID: 20280 RVA: 0x00221D3E File Offset: 0x0021FF3E
		private void UpdateResourceData(FactionResourcesUpdated e)
		{
			this.UpdateResourceData();
			if (this.resourceSalePanel.activeInHierarchy)
			{
				this.ResetResourcesPanel();
			}
		}

		// Token: 0x06004F39 RID: 20281 RVA: 0x00221D59 File Offset: 0x0021FF59
		private void UpdateResourceData(HabModuleConstructionStatusChange e)
		{
			this.UpdateResourceData();
		}

		// Token: 0x06004F3A RID: 20282 RVA: 0x00221D61 File Offset: 0x0021FF61
		private void UpdateResourceData(ShipConstructionCompleted e)
		{
			this.UpdateResourceData();
		}

		// Token: 0x06004F3B RID: 20283 RVA: 0x00221D69 File Offset: 0x0021FF69
		private void UpdateResourceData()
		{
			this.resourcesDataDirty = true;
		}

		// Token: 0x17000EC7 RID: 3783
		// (get) Token: 0x06004F3C RID: 20284 RVA: 0x00221D72 File Offset: 0x0021FF72
		private int finderMaxItems
		{
			get
			{
				return (int)(this.finderMaxHeight - 6f) / 29;
			}
		}

		// Token: 0x17000EC8 RID: 3784
		// (get) Token: 0x06004F3D RID: 20285 RVA: 0x00221D84 File Offset: 0x0021FF84
		private int targetingPanelMaxItems
		{
			get
			{
				return (int)(this.finderMaxHeight - 80f) / 50;
			}
		}

		// Token: 0x06004F3E RID: 20286 RVA: 0x00221D98 File Offset: 0x0021FF98
		public void OnAssetPanelOpened(MyAssetPanelOpened e)
		{
			float height_px = e.height_px;
			this.finderMaxHeight = 845f - height_px - 10f - (float)((TIUtilities.GetScreenRatio() > 2.3f) ? 90 : 0);
			if (TIPlayerProfileManager.uiScaleSetting > 0)
			{
				this.finderMaxHeight *= this.finderLargeUIScaleHeightFactor;
			}
			if (this.finderMaxHeight < 200f)
			{
				this.storedFinderStatus = this.finderCanvas.enabled;
				this.frameCheckedStoredFinderStatus = TIFrameCounter.FrameCount;
				this.EnableFinderCanvas(false);
				this.DisableTargetingPanel();
				return;
			}
			if (this.finderCanvas.enabled || this.storedFinderStatus)
			{
				this.UpdateFinderList();
				return;
			}
			if (this.targetingPanel.enabled)
			{
				this.RefreshTargetingPanel();
			}
		}

		// Token: 0x06004F3F RID: 20287 RVA: 0x00221E54 File Offset: 0x00220054
		public void OnAssetPanelClosed(MyAssetPanelEntirelyClosed e)
		{
			this.finderMaxHeight = 845f - (float)((TIUtilities.GetScreenRatio() > 2.3f) ? 90 : 0);
			if (TIPlayerProfileManager.uiScaleSetting > 0)
			{
				this.finderMaxHeight *= this.finderLargeUIScaleHeightFactor;
			}
			if (this.targetingPanel.enabled)
			{
				this.RefreshTargetingPanel();
				return;
			}
			if (this.finderCanvas.enabled)
			{
				this.UpdateFinderList();
				return;
			}
			if (this.storedFinderStatus && !this.infoScreenOpen)
			{
				this.OpenFinderCanvas();
				this.storedFinderStatus = false;
			}
		}

		// Token: 0x06004F40 RID: 20288 RVA: 0x00221EE0 File Offset: 0x002200E0
		public void OnAssetPanelResized(MyActiveAssetPanelResized e)
		{
			this.finderMaxHeight = 845f - e.height_px - 25f - (float)((TIUtilities.GetScreenRatio() > 2.3f) ? 90 : 0);
			if (TIPlayerProfileManager.uiScaleSetting > 0)
			{
				this.finderMaxHeight *= this.finderLargeUIScaleHeightFactor;
			}
			if (this.finderMaxHeight <= 200f)
			{
				if (TIFrameCounter.FrameCount != this.frameCheckedStoredFinderStatus)
				{
					this.storedFinderStatus = this.finderCanvas.enabled;
				}
				this.frameCheckedStoredFinderStatus = TIFrameCounter.FrameCount;
				this.EnableFinderCanvas(false);
				this.DisableTargetingPanel();
				return;
			}
			if (this.storedFinderStatus)
			{
				this.finderCanvas.enabled = true;
			}
			if (this.finderCanvas.enabled)
			{
				this.UpdateFinderList();
				return;
			}
			if (this.targetingPanel.enabled)
			{
				this.RefreshTargetingPanel();
			}
		}

		// Token: 0x06004F41 RID: 20289 RVA: 0x00221FB4 File Offset: 0x002201B4
		private void OnGameStateArchived(GameStateArchived e)
		{
			if (e.gameState == GeneralControlsController.UISelectedAssetState)
			{
				GeneralControlsController.SetUISelectedAssetState(null);
			}
			else if (e.gameState == GeneralControlsController.UIOtherSelectedState)
			{
				GeneralControlsController.SetUIOtherSelectedState(null);
			}
			if ((e.gameState.isArmyState || e.gameState.isCouncilorState || e.gameState.isHabState || e.gameState.isSpaceFleetState) && e.gameState.ref_faction == base.activePlayer)
			{
				this.UpdateFinderList();
			}
		}

		// Token: 0x06004F42 RID: 20290 RVA: 0x00222045 File Offset: 0x00220245
		public static bool IsCurrentlySelectedGameState(TIGameState state)
		{
			return state != null && (GeneralControlsController.UISelectedAssetState == state || GeneralControlsController.UIOtherSelectedState == state || GeneralControlsController.UITargetedState == state);
		}

		// Token: 0x06004F43 RID: 20291 RVA: 0x00222079 File Offset: 0x00220279
		public void OnGameStateNameChanged(GameStateNameChanged e)
		{
			this.UpdateFinderList();
		}

		// Token: 0x17000EC9 RID: 3785
		// (get) Token: 0x06004F44 RID: 20292 RVA: 0x00222084 File Offset: 0x00220284
		private bool ChatAnimatorIsPlaying
		{
			get
			{
				return !this.councilorChatAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle");
			}
		}

		// Token: 0x06004F45 RID: 20293 RVA: 0x002220B0 File Offset: 0x002202B0
		private void CouncilorChat()
		{
			if (this.notificationQueue.councilorMessages.Count > 0 && !this.ChatAnimatorIsPlaying && !base.canvasManager.IsShowingInfoScreen() && GameStateManager.NotificationQueue() != null)
			{
				CouncilorMessage nextCouncilorMessage = TINotificationQueueState.GetNextCouncilorMessage();
				TIGameState speaker = nextCouncilorMessage.speaker;
				if (speaker != null && speaker.isCouncilorState)
				{
					TICouncilorState ref_councilor = nextCouncilorMessage.speaker.ref_councilor;
					if (ref_councilor == null || !ref_councilor.active)
					{
						return;
					}
					string text = TINotificationQueueState.councilorGUIIconPath(nextCouncilorMessage.speaker.ref_councilor);
					GameControl.assetLoader.LoadAssetForImageAssignment(text, this.councilorChatImage);
				}
				else
				{
					TIGameState speaker2 = nextCouncilorMessage.speaker;
					if (speaker2 == null || !speaker2.isFactionState)
					{
						return;
					}
					this.councilorChatImage.sprite = nextCouncilorMessage.speaker.ref_faction.leaderIcon;
				}
				this.councilorChatText.SetText(nextCouncilorMessage.message);
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_IncomingChatMessage", false, false);
				this.councilorChatAnimator.SetTrigger("Slide");
			}
		}

		// Token: 0x06004F46 RID: 20294 RVA: 0x002221B5 File Offset: 0x002203B5
		public void OnChatButtonClicked()
		{
			if (this.ChatAnimatorIsPlaying)
			{
				this.councilorChatAnimator.SetTrigger("CloseNow");
			}
		}

		// Token: 0x06004F47 RID: 20295 RVA: 0x002221CF File Offset: 0x002203CF
		private IEnumerator SendQuicksaveNotification()
		{
			yield return new WaitForSeconds(1f);
			TINotificationQueueState.AddCouncilorMessage(base.activePlayer, CouncilorChatType.GameQuicksaved, base.activePlayer);
			this.CouncilorChat();
			yield break;
		}

		// Token: 0x06004F48 RID: 20296 RVA: 0x002221E0 File Offset: 0x002203E0
		private string AssembleFivePointReport(TIFactionState faction, FactionResource resourceType, string summaryTooltipPath, string detailTooltipPath, string incomeTooltipPath)
		{
			float currentResourceAmount = faction.GetCurrentResourceAmount(resourceType);
			float dailyIncome = faction.GetDailyIncome(resourceType, false, false);
			string resourceString = TIUtilities.GetResourceString(resourceType);
			StringBuilder stringBuilder = new StringBuilder(Loc.T(summaryTooltipPath, new object[] { TIUtilities.FormatSmallNumber(currentResourceAmount, 7, 0, (double)Mathf.Abs(currentResourceAmount) >= 1E-07, false) })).AppendLine();
			if (dailyIncome == 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.NoIncomeTip", new object[] { resourceString }));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.IncomeTip", new object[] { dailyIncome.ToString("N2") }));
				float dailyIncomeFromHQ = faction.GetDailyIncomeFromHQ(resourceType);
				float dailyIncomeFromCouncilors = faction.GetDailyIncomeFromCouncilors(resourceType);
				float dailyIncomeFromNations = faction.GetDailyIncomeFromNations(resourceType, false);
				float dailyIncomeFromHabs = faction.GetDailyIncomeFromHabs(resourceType);
				float negativeDailyIncomeFromUnassignedOrgs = faction.GetNegativeDailyIncomeFromUnassignedOrgs(resourceType);
				if (dailyIncomeFromHQ != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.BaseIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromHQ, 7, 0, (double)dailyIncomeFromHQ >= 0.001, false) }));
				}
				if (dailyIncomeFromCouncilors != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.CouncilIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromCouncilors, 7, 0, (double)dailyIncomeFromCouncilors >= 0.001, false) }));
				}
				if (dailyIncomeFromNations != 0f)
				{
					if (resourceType == FactionResource.Influence)
					{
						stringBuilder.AppendLine(Loc.T("UI.GeneralControls.NationInfluenceIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromNations, 7, 0, (double)dailyIncomeFromNations >= 0.001, false) }));
						float num = -base.activePlayer.GetAnnualControlPointMaintenanceCost() / 365.2422f;
						if (num < 0f)
						{
							stringBuilder.AppendLine(Loc.T("UI.GeneralControls.InfluenceCPCost", new object[] { TIUtilities.FormatSmallNumber(num, 7, 0, true, false) }));
						}
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.GeneralControls.NationIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromNations, 7, 0, (double)dailyIncomeFromNations >= 0.001, false) }));
					}
				}
				if (dailyIncomeFromHabs != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.HabIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromHabs, 7, 0, (double)dailyIncomeFromHabs >= 0.001, false) }));
				}
				if (negativeDailyIncomeFromUnassignedOrgs != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.UnassignedOrgIncomeTip", new object[] { TIUtilities.FormatSmallNumber(negativeDailyIncomeFromUnassignedOrgs, 7, 0, (double)negativeDailyIncomeFromUnassignedOrgs >= 0.001, false) }));
				}
				if (TISpaceShipState.relevantIncomeResources.Contains(resourceType))
				{
					float dailyNetIncomeFromShips = faction.GetDailyNetIncomeFromShips(resourceType);
					if (dailyNetIncomeFromShips != 0f)
					{
						stringBuilder.AppendLine(Loc.T("UI.GeneralControls.ShipIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyNetIncomeFromShips, 7, 0, (double)dailyNetIncomeFromShips >= 0.001, false) }));
					}
				}
				if (resourceType == FactionResource.Money)
				{
					float dailyIncomeFromExcessMissionControl = faction.GetDailyIncomeFromExcessMissionControl(resourceType);
					if (dailyIncomeFromExcessMissionControl != 0f)
					{
						stringBuilder.AppendLine(Loc.T("UI.GeneralControls.ExcessMissionControlTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromExcessMissionControl, 7, 0, true, false) }));
					}
					if (faction.lastWeeksSpoils > 0f)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.GeneralControls.Spoils", new object[] { TIUtilities.FormatSmallNumber(faction.recentDailySpoilsIncome, 7, 0, true, false) }));
					}
				}
				float monthlyIncome = faction.GetMonthlyIncome(resourceType, false, false);
				float yearlyIncome = faction.GetYearlyIncome(resourceType, false, false, false);
				stringBuilder.AppendLine(TIUtilities.separator).AppendLine(Loc.T("UI.GeneralControls.Monthly", new object[]
				{
					TIUtilities.FormatSmallNumber(monthlyIncome, 7, 0, (double)monthlyIncome >= 1E-06, false),
					TIUtilities.InlineResourceStr(resourceType)
				})).AppendLine(Loc.T("UI.GeneralControls.Annual", new object[]
				{
					TIUtilities.FormatSmallNumber(yearlyIncome, 7, 0, (double)yearlyIncome >= 1E-06, false),
					TIUtilities.InlineResourceStr(resourceType)
				}));
			}
			stringBuilder.AppendLine(TIUtilities.separator);
			stringBuilder.Append(Loc.T(detailTooltipPath));
			if (incomeTooltipPath != string.Empty)
			{
				stringBuilder.Append(" ").Append(Loc.T(incomeTooltipPath)).AppendLine();
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004F49 RID: 20297 RVA: 0x00222620 File Offset: 0x00220820
		private string AssembleResearchReport(TIFactionState faction)
		{
			float dailyIncome = faction.GetDailyIncome(FactionResource.Research, false, false);
			StringBuilder stringBuilder = new StringBuilder();
			if (dailyIncome == 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.NoResearchIncomeTip", new object[] { TIUtilities.GetResourceString(FactionResource.Research) }));
			}
			else
			{
				float num = dailyIncome * faction.BonusPctFromDistribution;
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.ResearchIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncome + num, 7, 0, true, false) }));
				float dailyIncomeFromHQ = faction.GetDailyIncomeFromHQ(FactionResource.Research);
				float dailyIncomeFromCouncilors = faction.GetDailyIncomeFromCouncilors(FactionResource.Research);
				float dailyIncomeFromNations = faction.GetDailyIncomeFromNations(FactionResource.Research, true);
				float dailyIncomeFromHabs = faction.GetDailyIncomeFromHabs(FactionResource.Research);
				float dailyIncomeFromExcessMissionControl = faction.GetDailyIncomeFromExcessMissionControl(FactionResource.Research);
				if (dailyIncomeFromHQ != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.BaseIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromHQ, 7, 0, true, false) }));
				}
				if (dailyIncomeFromCouncilors != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.CouncilIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromCouncilors, 7, 0, true, false) }));
				}
				if (dailyIncomeFromNations != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.NationIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromNations, 7, 0, true, false) }));
				}
				if (dailyIncomeFromHabs != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.HabIncomeTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromHabs, 7, 0, true, false) }));
				}
				if (dailyIncomeFromExcessMissionControl != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.ExcessMissionControlTip", new object[] { TIUtilities.FormatSmallNumber(dailyIncomeFromExcessMissionControl, 7, 0, true, false) }));
				}
				if (num != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.TechDistributionBonus", new object[]
					{
						TIUtilities.FormatSmallNumber(num, 7, 0, true, false),
						faction.ContributingToSlots(faction.OrgProjectAllowed(), faction.HabProjectAllowed()).ToString()
					}));
				}
				stringBuilder.AppendLine(TIUtilities.separator).AppendLine(Loc.T("UI.GeneralControls.Monthly", new object[]
				{
					TIUtilities.FormatBigOrSmallNumber(faction.GetMonthlyIncome(FactionResource.Research, false, false) * (1f + faction.BonusPctFromDistribution), 1, 7, 0, false, false),
					TemplateManager.global.researchInlineSpritePath
				})).AppendLine(Loc.T("UI.GeneralControls.Annual", new object[]
				{
					TIUtilities.FormatBigOrSmallNumber(faction.GetYearlyIncome(FactionResource.Research, false, false, false) * (1f + faction.BonusPctFromDistribution), 1, 7, 0, false, false),
					TemplateManager.global.researchInlineSpritePath
				}));
				stringBuilder.AppendLine(TIUtilities.separator).AppendLine(Loc.T("UI.GeneralControls.ResearchDetail"));
			}
			StringBuilder stringBuilder2 = new StringBuilder(Loc.T("UI.GeneralControls.ResearchBonusesTip")).AppendLine();
			bool flag = false;
			foreach (TechCategory techCategory in Enums.TechCategories)
			{
				float num2 = faction.SumCategoryModifiers(techCategory);
				if (num2 != 0f)
				{
					flag = true;
					stringBuilder2.AppendLine(Loc.T("UI.GeneralControls.ResearchBonusesListItem", new object[]
					{
						TIGenericTechTemplate.GetTechCategoryString(techCategory),
						num2.ToPercent("P0")
					}));
				}
			}
			if (flag)
			{
				stringBuilder.Append(stringBuilder2);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004F4A RID: 20298 RVA: 0x0022293C File Offset: 0x00220B3C
		private string AssembleMissionControlReport(TIFactionState faction)
		{
			float num = (float)faction.GetMaxMissionControl();
			float num2 = (float)(faction.GetMissionControlRequirementFromShips() + faction.GetMissionControlFromRefits());
			float num3 = (float)faction.GetMissionControlRequirementFromHabs(false);
			float num4 = (float)faction.GetMissionControlRequirementFromMineNetwork(-1);
			float num5 = num2 + num3;
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.GeneralControls.MissionControlTip", new object[]
			{
				num.ToString("N0"),
				num5.ToString()
			})).AppendLine();
			float yearlyIncomeFromHQ = faction.GetYearlyIncomeFromHQ(FactionResource.MissionControl);
			float num6 = (float)faction.GetMissionControlFromCouncilors();
			float num7 = (float)faction.GetMissionControlFromNations();
			float num8 = (float)faction.GetMissionControlContributionFromHabs();
			float num9 = TIEffectsState.SumEffectsModifiers(Context.MissionControlDisruption_PCT, faction, 1f, null);
			if (yearlyIncomeFromHQ != 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.BaseIncomeTip", new object[] { yearlyIncomeFromHQ.ToString("N0") }));
			}
			if (num6 != 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.CouncilIncomeTip", new object[] { num6.ToString("N0") }));
			}
			if (num7 != 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.NationIncomeTip", new object[] { num7.ToString("N0") }));
			}
			if (num8 != 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.HabIncomeTip", new object[] { num8.ToString("N0") }));
			}
			if (num9 != 0f)
			{
				stringBuilder.AppendLine().AppendLine(TIUtilities.RedLine(Loc.T("UI.GeneralControls.MissionControlDisruption", new object[] { Mathf.Abs(num9).ToPercent("P0") }))).AppendLine();
			}
			if (num5 > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.MissionControlBreakdown"));
				if (num2 > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.ShipCostTip", new object[] { num2.ToString("N0") }));
				}
				if (num3 > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.HabCostTip", new object[] { num3.ToString("N0") }));
				}
				if (num4 > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.MiningOverage", new object[] { num4.ToString("N0") }));
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.GeneralControls.ManyMinesExtraCost", new object[]
					{
						num4,
						(float)faction.GetMissionControlRequirementFromNextMine(null) + num4,
						faction.SafeMineNextworkSize,
						TemplateManager.global.missionControlInlineSpritePath
					}));
				}
				else if (faction.SafeMineNextworkSize > 0)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append(Loc.T("UI.GeneralControls.MineNetwork", new object[]
					{
						faction.MineNetworkSize,
						faction.SafeMineNextworkSize,
						TemplateManager.global.missionControlInlineSpritePath
					}));
					if (faction.MineNetworkSize == faction.SafeMineNextworkSize)
					{
						stringBuilder.Append(Loc.T("UI.GeneralControls.MineNetworkNext", new object[]
						{
							faction.GetMissionControlRequirementFromNextMine(null),
							TemplateManager.global.missionControlInlineSpritePath
						}));
					}
					stringBuilder.AppendLine();
				}
			}
			if (faction.habModules.Any<TIHabModuleState>((TIHabModuleState x) => x.underConstruction && x.moduleTemplate.missionControl != 0))
			{
				int num10 = faction.habModules.Where<TIHabModuleState>((TIHabModuleState x) => x.underConstruction && x.hab.coreModule != x).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.missionControl);
				if (num10 > 0)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.GeneralControls.MCNetGainUnderConstruction", new object[]
					{
						TIUtilities.GreenLine(num10.ToString("N0")),
						TemplateManager.global.missionControlInlineSpritePath
					}));
				}
				else if (num10 < 0)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.GeneralControls.MCNetSpendUnderConstruction", new object[]
					{
						TIUtilities.RedLine((-num10).ToString("N0")),
						TemplateManager.global.missionControlInlineSpritePath
					}));
				}
			}
			if (faction.habModules.Any<TIHabModuleState>((TIHabModuleState x) => x.functional && !x.powered && x.moduleTemplate.missionControl != 0))
			{
				int num11 = faction.habModules.Where<TIHabModuleState>((TIHabModuleState x) => x.functional && !x.powered).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.missionControl);
				if (num11 >= 0)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.GeneralControls.MCNetGainTurnedOff", new object[]
					{
						TIUtilities.GreenLine(num11.ToString("N0")),
						TemplateManager.global.missionControlInlineSpritePath
					}));
				}
				else if (num11 < 0)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.GeneralControls.MCNetSpendTurnedOff", new object[]
					{
						TIUtilities.RedLine((-num11).ToString("N0")),
						TemplateManager.global.missionControlInlineSpritePath
					}));
				}
			}
			int futureMissionControlfromUnderConstructionShips = faction.GetFutureMissionControlfromUnderConstructionShips(false);
			if (futureMissionControlfromUnderConstructionShips != 0)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.GeneralControls.MCFromUnderConstructionShips", new object[]
				{
					TIUtilities.RedLine(futureMissionControlfromUnderConstructionShips.ToString("N0")),
					TemplateManager.global.missionControlInlineSpritePath
				}));
			}
			stringBuilder.AppendLine(TIUtilities.separator).AppendLine(Loc.T("UI.GeneralControls.MissionControlDetail"));
			return stringBuilder.ToString();
		}

		// Token: 0x06004F4B RID: 20299 RVA: 0x00222F10 File Offset: 0x00221110
		private string AssembleProjectsReport(TIFactionState faction)
		{
			float num = (float)faction.GetMaxSimultaneousProjects();
			if (num == 1f)
			{
				return Loc.T("UI.GeneralControls.EngineeringTipSingle", new object[] { faction.displayNameCapitalized });
			}
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.GeneralControls.EngineeringTipMult", new object[]
			{
				faction.displayNameCapitalized,
				num.ToString("N0")
			})).AppendLine();
			stringBuilder.AppendLine(Loc.T("UI.GeneralControls.BaseIncomeTip", new object[] { "1" }));
			if (faction.OrgProjectAllowed())
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.CouncilIncomeTip", new object[] { "1" }));
			}
			if (faction.HabProjectAllowed())
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.HabIncomeTip", new object[] { "1" }));
			}
			float num2 = faction.MultipleFacilitiesMultiplier(faction.TraitProjectCount(), faction.OrgProjectCount(), faction.HabProjectCount());
			stringBuilder.AppendLine(TIUtilities.separator);
			if (num2 > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.EngineeringDetail", new object[] { num2.ToPercent("P0") }));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.GeneralControls.EngineeringDetailNone"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000ECA RID: 3786
		// (get) Token: 0x06004F4C RID: 20300 RVA: 0x00223052 File Offset: 0x00221252
		private static bool showMonthlyIncomes
		{
			get
			{
				return GameControl.control.activePlayer.showMonthlyIncomesInTopBarAndIntel;
			}
		}

		// Token: 0x06004F4D RID: 20301 RVA: 0x00223064 File Offset: 0x00221264
		public static string ResourceReportString(TIFactionState faction, FactionResource resourceType)
		{
			string text = string.Empty;
			switch (resourceType)
			{
			case FactionResource.Money:
			case FactionResource.Influence:
			case FactionResource.Operations:
			case FactionResource.Boost:
			case FactionResource.Water:
			case FactionResource.Volatiles:
			case FactionResource.Metals:
			case FactionResource.NobleMetals:
			case FactionResource.Fissiles:
			case FactionResource.Exotics:
			{
				float num;
				if (GeneralControlsController.showMonthlyIncomes)
				{
					num = faction.GetMonthlyIncome(resourceType, false, false);
				}
				else
				{
					num = faction.GetDailyIncome(resourceType, false, false);
				}
				float currentResourceAmount = faction.GetCurrentResourceAmount(resourceType);
				if (num == 0f)
				{
					text = TIUtilities.FormatBigOrSmallNumber(currentResourceAmount, 1, 7, 0, false, false);
				}
				else if (num > 0f)
				{
					int num2 = ((num >= 100f) ? 0 : 2);
					text = Loc.T("UI.GeneralControls.ResourcesGain", new object[]
					{
						TIUtilities.FormatBigNumber((double)currentResourceAmount, 1, false),
						TIUtilities.FormatBigOrSmallNumber(num, 1, num2, 0, false, false)
					});
				}
				else if (num <= -0.01f)
				{
					text = Loc.T("UI.GeneralControls.ResourcesLoss", new object[]
					{
						TIUtilities.FormatBigNumber((double)currentResourceAmount, 1, false),
						TIUtilities.FormatBigOrSmallNumber(num, 0, 2, 0, false, false)
					});
				}
				else
				{
					text = Loc.T("UI.GeneralControls.ResourcesSmallLoss", new object[]
					{
						TIUtilities.FormatBigNumber((double)currentResourceAmount, 1, false),
						"0"
					});
				}
				break;
			}
			case FactionResource.Research:
			{
				float num3;
				if (GeneralControlsController.showMonthlyIncomes)
				{
					num3 = faction.GetMonthlyIncome(resourceType, false, false) * (1f + faction.BonusPctFromDistribution);
				}
				else
				{
					num3 = faction.GetDailyIncome(resourceType, false, false) * (1f + faction.BonusPctFromDistribution);
				}
				text = TIUtilities.FormatBigNumber((double)num3, 1, false);
				break;
			}
			case FactionResource.Projects:
				text = faction.GetDailyIncome(resourceType, false, false).ToString("N0");
				break;
			case FactionResource.MissionControl:
			{
				float dailyIncome = faction.GetDailyIncome(resourceType, false, false);
				float num4 = (float)faction.GetMissionControlUsage();
				if (num4 > dailyIncome)
				{
					text = Loc.T("UI.GeneralControls.ResourcesUsage", new object[]
					{
						new StringBuilder("<color=#B26A60>").Append(num4.ToString()).Append("</color>"),
						dailyIncome.ToString("N0")
					});
				}
				else if (faction.MineNetworkSize > faction.SafeMineNextworkSize)
				{
					text = Loc.T("UI.GeneralControls.ResourcesUsage", new object[]
					{
						new StringBuilder("<color=#EC9933>").Append(num4.ToString()).Append("</color>"),
						dailyIncome.ToString("N0")
					});
				}
				else
				{
					text = Loc.T("UI.GeneralControls.ResourcesUsage", new object[]
					{
						num4.ToString("N0"),
						dailyIncome.ToString("N0")
					});
				}
				break;
			}
			case FactionResource.Antimatter:
			{
				float num5;
				if (GeneralControlsController.showMonthlyIncomes)
				{
					num5 = faction.GetMonthlyIncome(resourceType, false, false);
				}
				else
				{
					num5 = faction.GetDailyIncome(resourceType, false, false);
				}
				float currentResourceAmount2 = faction.GetCurrentResourceAmount(resourceType);
				if (num5 == 0f)
				{
					text = TIUtilities.FormatBigOrSmallNumber(currentResourceAmount2, 1, 7, 0, true, false);
				}
				else if (num5 > 0f)
				{
					text = Loc.T("UI.GeneralControls.ResourcesGain", new object[]
					{
						TIUtilities.FormatBigOrSmallNumber(currentResourceAmount2, 1, 7, 0, true, false),
						TIUtilities.FormatBigOrSmallNumber(num5, 1, 7, 0, true, false)
					});
				}
				else if (num5 <= -1f)
				{
					text = Loc.T("UI.GeneralControls.ResourcesLoss", new object[]
					{
						TIUtilities.FormatBigOrSmallNumber(currentResourceAmount2, 1, 7, 0, true, false),
						Math.Truncate((double)num5).ToString("N0")
					});
				}
				else
				{
					text = Loc.T("UI.GeneralControls.ResourcesSmallLoss", new object[]
					{
						TIUtilities.FormatBigOrSmallNumber(currentResourceAmount2, 1, 7, 0, true, false),
						TIUtilities.FormatBigOrSmallNumber(num5, 1, 7, 0, true, false)
					});
				}
				break;
			}
			}
			return text;
		}

		// Token: 0x06004F4E RID: 20302 RVA: 0x002233E0 File Offset: 0x002215E0
		private void SetSpaceResourceValuesInBar(TIFactionState faction, FactionResource resourceType, TMP_Text reportText, Transform panel)
		{
			if ((float)Screen.width / (float)Screen.height >= 1.5f)
			{
				panel.gameObject.GetComponent<LayoutElement>().preferredWidth = 100f;
				reportText.SetText(GeneralControlsController.ResourceReportString(faction, resourceType));
				return;
			}
			panel.gameObject.GetComponent<LayoutElement>().preferredWidth = 65f;
			float currentResourceAmount = faction.GetCurrentResourceAmount(resourceType);
			if (resourceType == FactionResource.Antimatter)
			{
				reportText.SetText(TIUtilities.FormatBigOrSmallNumber(currentResourceAmount, 1, 7, 0, false, false));
				return;
			}
			reportText.SetText(TIUtilities.FormatBigNumber((double)currentResourceAmount, 1, false));
		}

		// Token: 0x06004F4F RID: 20303 RVA: 0x0022346C File Offset: 0x0022166C
		private string AssembleControlPointMaintenanceTooltip(TIFactionState faction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.GeneralControls.CPCapDetails1", new object[]
			{
				TIUtilities.FormatSmallNumber(faction.GetBaselineControlPointMaintenanceCost(false), 7, 1, true, false),
				TIUtilities.FormatSmallNumber(faction.GetControlPointMaintenanceFreebieCap(), 7, 0, true, false)
			}));
			float annualControlPointMaintenanceCost = faction.GetAnnualControlPointMaintenanceCost();
			if (annualControlPointMaintenanceCost > 0f)
			{
				stringBuilder.Append(Loc.T("UI.GeneralControls.CPCapDetailsCost", new object[]
				{
					TemplateManager.global.influenceInlineSpritePath,
					TIUtilities.FormatSmallNumber(annualControlPointMaintenanceCost / 365.2422f, 7, 0, true, false)
				}));
			}
			else
			{
				stringBuilder.Append(Loc.T("UI.GeneralControls.CPCapDetailsWarn", new object[] { TemplateManager.global.influenceInlineSpritePath }));
			}
			float averagedControlPointCapPenaltyToMissions = faction.GetAveragedControlPointCapPenaltyToMissions();
			if (averagedControlPointCapPenaltyToMissions > 0f)
			{
				stringBuilder.Append(Loc.T("UI.GeneralControls.CPCapOverageCurrent", new object[] { TIUtilities.FormatSmallNumber(averagedControlPointCapPenaltyToMissions, 7, 0, true, false) }));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004F50 RID: 20304 RVA: 0x00223560 File Offset: 0x00221760
		private string AssembleSpaceResourceTooltip(TIFactionState faction, FactionResource resourceType, string detailTooltipPath)
		{
			float currentResourceAmount = faction.GetCurrentResourceAmount(resourceType);
			float dailyIncome = faction.GetDailyIncome(resourceType, false, false);
			StringBuilder stringBuilder;
			if (currentResourceAmount > 0f && (double)currentResourceAmount < 1E-06)
			{
				if (dailyIncome > 0f && (double)dailyIncome < 1E-06)
				{
					stringBuilder = new StringBuilder(Loc.T("UI.GeneralControls.SpaceResourcesTooltip", new object[]
					{
						TIUtilities.FormatSmallNumber(currentResourceAmount, 7, 0, false, false),
						TIUtilities.GetResourceString(resourceType),
						TIUtilities.FormatSmallNumber(dailyIncome, 7, 0, false, false),
						Loc.T("UI.GeneralControls.SpaceResourcesParentheticalValue", new object[] { currentResourceAmount.ToString() }),
						Loc.T("UI.GeneralControls.SpaceResourcesParentheticalValue", new object[] { dailyIncome.ToString() })
					})).AppendLine();
				}
				else
				{
					stringBuilder = new StringBuilder(Loc.T("UI.GeneralControls.SpaceResourcesTooltip", new object[]
					{
						TIUtilities.FormatSmallNumber(currentResourceAmount, 7, 0, false, false),
						TIUtilities.GetResourceString(resourceType),
						TIUtilities.FormatSmallNumber(dailyIncome, 7, 0, true, false),
						Loc.T("UI.GeneralControls.SpaceResourcesParentheticalValue", new object[] { currentResourceAmount.ToString() }),
						string.Empty
					})).AppendLine();
				}
			}
			else if (dailyIncome > 0f && (double)dailyIncome < 1E-06)
			{
				stringBuilder = new StringBuilder(Loc.T("UI.GeneralControls.SpaceResourcesTooltip", new object[]
				{
					TIUtilities.FormatSmallNumber(currentResourceAmount, 7, 0, false, false),
					TIUtilities.GetResourceString(resourceType),
					TIUtilities.FormatSmallNumber(dailyIncome, 7, 0, true, false),
					string.Empty,
					Loc.T("UI.GeneralControls.SpaceResourcesParentheticalValue", new object[] { dailyIncome.ToString() })
				})).AppendLine();
			}
			else
			{
				stringBuilder = new StringBuilder(Loc.T("UI.GeneralControls.SpaceResourcesTooltip", new object[]
				{
					TIUtilities.FormatSmallNumber(currentResourceAmount, 7, 0, true, false),
					TIUtilities.GetResourceString(resourceType),
					TIUtilities.FormatSmallNumber(dailyIncome, 7, 0, true, false),
					string.Empty,
					string.Empty
				})).AppendLine();
			}
			if (dailyIncome < 0f && currentResourceAmount <= 0f)
			{
				stringBuilder.AppendLine();
				if (faction.DailyHabBoostShortage() <= 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.InsufficientSpaceResource"));
				}
				else
				{
					stringBuilder.AppendLine(Loc.T("UI.GeneralControls.InsufficientBoost"));
				}
			}
			if (dailyIncome != 0f)
			{
				float monthlyIncome = faction.GetMonthlyIncome(resourceType, false, false);
				float yearlyIncome = faction.GetYearlyIncome(resourceType, false, false, false);
				stringBuilder.AppendLine(TIUtilities.separator).AppendLine(Loc.T("UI.GeneralControls.Monthly", new object[]
				{
					TIUtilities.FormatSmallNumber(monthlyIncome, 7, 0, (double)monthlyIncome >= 1E-06, false),
					TIUtilities.InlineResourceStr(resourceType)
				})).AppendLine(Loc.T("UI.GeneralControls.Annual", new object[]
				{
					TIUtilities.FormatSmallNumber(yearlyIncome, 7, 0, (double)yearlyIncome < 1E-06, false),
					TIUtilities.InlineResourceStr(resourceType)
				}));
			}
			if (resourceType - FactionResource.Water <= 4)
			{
				float num = faction.GetCurrentMiningMultiplierFromOrgsAndEffects(resourceType) - 1f;
				if (num != 0f)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.GeneralControls.MiningMultiplier", new object[]
					{
						num.ToPercent("P0"),
						TIUtilities.InlineResourceStr(resourceType)
					}));
				}
			}
			stringBuilder.AppendLine(TIUtilities.separator).AppendLine(Loc.T(detailTooltipPath));
			return stringBuilder.ToString();
		}

		// Token: 0x06004F51 RID: 20305 RVA: 0x002238B8 File Offset: 0x00221AB8
		public static string ControlPointMaintenanceString(TIFactionState faction)
		{
			float baselineControlPointMaintenanceCost = faction.GetBaselineControlPointMaintenanceCost(false);
			float controlPointMaintenanceFreebieCap = faction.GetControlPointMaintenanceFreebieCap();
			string text = Loc.T("UI.GeneralControls.ResourcesUsage", new object[]
			{
				baselineControlPointMaintenanceCost.ToString("N0"),
				controlPointMaintenanceFreebieCap.ToString("N0")
			});
			if (baselineControlPointMaintenanceCost > controlPointMaintenanceFreebieCap)
			{
				text = TIUtilities.RedLine(text);
			}
			return text;
		}

		// Token: 0x06004F52 RID: 20306 RVA: 0x00223910 File Offset: 0x00221B10
		private void SetResourceTooltipDelegates(TIFactionState faction)
		{
			this.moneyTooltipTrigger.SetDelegate("BodyText", () => this.AssembleFivePointReport(this.activePlayer, FactionResource.Money, "UI.GeneralControls.MoneyTip", "UI.GeneralControls.MoneyDetail", "UI.GeneralControls.MoneySource"));
			this.influenceTooltipTrigger.SetDelegate("BodyText", () => this.AssembleFivePointReport(faction, FactionResource.Influence, "UI.GeneralControls.InfluenceTip", "UI.GeneralControls.InfluenceDetail", "UI.GeneralControls.InfluenceSource"));
			this.opsTooltipTrigger.SetDelegate("BodyText", () => this.AssembleFivePointReport(faction, FactionResource.Operations, "UI.GeneralControls.OpsTip", "UI.GeneralControls.OpsDetail", "UI.GeneralControls.OpsSource"));
			this.boostTooltipTrigger.SetDelegate("BodyText", () => this.AssembleFivePointReport(faction, FactionResource.Boost, "UI.GeneralControls.BoostTip", "UI.GeneralControls.BoostDetail", "UI.GeneralControls.BoostSource"));
			this.missionControlTooltipTrigger.SetDelegate("BodyText", () => this.AssembleMissionControlReport(faction));
			this.researchTooltipTrigger.SetDelegate("BodyText", () => this.AssembleResearchReport(faction));
			this.controlPointMaintenanceTrigger.SetDelegate("BodyText", () => this.AssembleControlPointMaintenanceTooltip(faction));
			this.waterTooltipTrigger.SetDelegate("BodyText", () => this.AssembleSpaceResourceTooltip(faction, FactionResource.Water, "UI.GeneralControls.WaterDetail"));
			this.volatilesTooltipTrigger.SetDelegate("BodyText", () => this.AssembleSpaceResourceTooltip(faction, FactionResource.Volatiles, "UI.GeneralControls.VolatilesDetail"));
			this.baseMetalsTooltipTrigger.SetDelegate("BodyText", () => this.AssembleSpaceResourceTooltip(faction, FactionResource.Metals, "UI.GeneralControls.BaseMetalsDetail"));
			this.nobleMetalsTooltipTrigger.SetDelegate("BodyText", () => this.AssembleSpaceResourceTooltip(faction, FactionResource.NobleMetals, "UI.GeneralControls.NobleMetalsDetail"));
			this.fissilesTooltipTrigger.SetDelegate("BodyText", () => this.AssembleSpaceResourceTooltip(faction, FactionResource.Fissiles, "UI.GeneralControls.FissilesDetail"));
			this.antimatterTooltipTrigger.SetDelegate("BodyText", () => this.AssembleSpaceResourceTooltip(faction, FactionResource.Antimatter, "UI.GeneralControls.AntimatterDetail"));
			this.exoticsTooltipTrigger.SetDelegate("BodyText", () => this.AssembleSpaceResourceTooltip(faction, FactionResource.Exotics, "UI.GeneralControls.ExoticsDetail"));
		}

		// Token: 0x06004F53 RID: 20307 RVA: 0x00223ABC File Offset: 0x00221CBC
		private void UpdateResourceData(TIFactionState faction)
		{
			this.incomeInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Money));
			this.influenceInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Influence));
			this.operationInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Operations));
			this.boostInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Boost));
			this.researchInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.Research));
			this.missionControlInfoText.SetText(GeneralControlsController.ResourceReportString(faction, FactionResource.MissionControl));
			this.controlPointMaintenanceText.SetText(GeneralControlsController.ControlPointMaintenanceString(faction));
			bool unlockedSpaceResources = faction.UnlockedSpaceResources;
			bool unlockedAntimatter = faction.UnlockedAntimatter;
			bool unlockedExotics = faction.UnlockedExotics;
			this.waterPanel.gameObject.SetActive(unlockedSpaceResources);
			this.volatilesPanel.gameObject.SetActive(unlockedSpaceResources);
			this.baseMetalsPanel.gameObject.SetActive(unlockedSpaceResources);
			this.nobleMetalsPanel.gameObject.SetActive(unlockedSpaceResources);
			this.fissilesPanel.gameObject.SetActive(unlockedSpaceResources);
			this.antimatterPanel.gameObject.SetActive(unlockedAntimatter);
			this.exoticsPanel.gameObject.SetActive(unlockedExotics);
			if (unlockedSpaceResources)
			{
				this.SetSpaceResourceValuesInBar(faction, FactionResource.Water, this.waterInfoText, this.waterPanel);
				this.SetSpaceResourceValuesInBar(faction, FactionResource.Volatiles, this.volatilesInfoText, this.volatilesPanel);
				this.SetSpaceResourceValuesInBar(faction, FactionResource.Metals, this.baseMetalsInfoText, this.baseMetalsPanel);
				this.SetSpaceResourceValuesInBar(faction, FactionResource.NobleMetals, this.nobleMetalsInfoText, this.nobleMetalsPanel);
				this.SetSpaceResourceValuesInBar(faction, FactionResource.Fissiles, this.fissilesInfoText, this.fissilesPanel);
			}
			if (unlockedAntimatter)
			{
				this.SetSpaceResourceValuesInBar(faction, FactionResource.Antimatter, this.antimatterInfoText, this.antimatterPanel);
			}
			if (unlockedExotics)
			{
				this.SetSpaceResourceValuesInBar(faction, FactionResource.Exotics, this.exoticsInfoText, this.exoticsPanel);
			}
		}

		// Token: 0x06004F54 RID: 20308 RVA: 0x00223C70 File Offset: 0x00221E70
		private void UpdateResearchLeadersLights()
		{
			for (int i = 0; i < 3; i++)
			{
				TechProgress techProgress = GameStateManager.GlobalResearch().GetTechProgress(i);
				if (techProgress.accumulatedResearch > 0f)
				{
					TIFactionState key = techProgress.factionContributions.Aggregate<KeyValuePair<TIFactionState, float>>(delegate(KeyValuePair<TIFactionState, float> l, KeyValuePair<TIFactionState, float> r)
					{
						if (l.Value <= r.Value)
						{
							return r;
						}
						return l;
					}).Key;
					if (key != null)
					{
						this.techWinnerLights[i].color = key.template.color;
						bool flag = (double)(key.template.color.r * 255f * 0.2126f + key.template.color.g * 255f * 0.7152f) + (double)(key.template.color.b * 255f) * 0.0722 >= 128.0;
						if (techProgress.CantLose(GameControl.control.activePlayer))
						{
							this.techWinnerIndicators[i].enabled = false;
							this.techWinnerIndicators[i + 3].enabled = false;
							this.techWinnerIndicators[i].color = (flag ? Color.black : Color.white);
						}
						else if (techProgress.CantWin(GameControl.control.activePlayer))
						{
							this.techWinnerIndicators[i].enabled = false;
							this.techWinnerIndicators[i + 3].enabled = true;
							this.techWinnerIndicators[i + 3].color = (flag ? Color.black : Color.white);
						}
						else
						{
							this.techWinnerIndicators[i].enabled = true;
							this.techWinnerIndicators[i + 3].enabled = false;
						}
					}
					else
					{
						this.techWinnerLights[i].color = Color.gray;
						this.techWinnerIndicators[i].enabled = false;
						this.techWinnerIndicators[i + 3].enabled = false;
					}
				}
				else
				{
					this.techWinnerLights[i].color = Color.gray;
					this.techWinnerIndicators[i].enabled = false;
					this.techWinnerIndicators[i + 3].enabled = false;
				}
			}
		}

		// Token: 0x06004F55 RID: 20309 RVA: 0x00223E98 File Offset: 0x00222098
		public void ActivateTargetingPanel(List<MissionOption> missionOptions, TIGameState baseTarget)
		{
			this.storedFinderStatus = this.finderCanvas.enabled;
			this.EnableFinderCanvas(false);
			this.targetingList.SetListSize<TargetingListItemController>(missionOptions.Count, false, false);
			if (missionOptions.Count > 0)
			{
				this.targetingHeaderTargetString.SetText(TIUtilities.GetStateDisplayName(missionOptions[0].target, base.activePlayer, false, true, false, false, true));
				int num = 0;
				using (IEnumerator<object> enumerator = this.targetingList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GeneralControlsController.<>o__340.<>p__0 == null)
						{
							GeneralControlsController.<>o__340.<>p__0 = CallSite<Func<CallSite, object, TargetingListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TargetingListItemController), typeof(GeneralControlsController)));
						}
						TargetingListItemController targetingListItemController = GeneralControlsController.<>o__340.<>p__0.Target(GeneralControlsController.<>o__340.<>p__0, enumerator.Current);
						MissionOption missionOption = missionOptions[num++];
						targetingListItemController.UpdateListItem(missionOption.mission, missionOption.councilor, missionOption.baseChanceString, missionOption.target);
					}
					goto IL_010D;
				}
			}
			this.targetingHeaderTargetString.SetText(TIUtilities.GetStateDisplayName(baseTarget, base.activePlayer, false, true, false, false, true));
			IL_010D:
			this.targetingPanelTransform.sizeDelta = new Vector2(this.targetingPanelTransform.sizeDelta.x, (float)(80 + 50 * Mathf.Min(this.targetingPanelMaxItems, Math.Max(missionOptions.Count, 1))));
			this.targetingPanel.enabled = true;
		}

		// Token: 0x06004F56 RID: 20310 RVA: 0x0022400C File Offset: 0x0022220C
		public void RefreshTargetingPanel()
		{
			if (this.targetingPanel.enabled)
			{
				if (this.currentTarget != null)
				{
					List<MissionOption> missionOptionsForTarget = base.activePlayer.GetMissionOptionsForTarget(this.currentTarget);
					List<MissionOption> list;
					if (this.showAssignedCouncilorsInTargetingPanel)
					{
						list = missionOptionsForTarget.Where<MissionOption>((MissionOption x) => x.mission.CanAfford(x.councilor.faction, x.councilor)).ToList<MissionOption>();
					}
					else
					{
						list = missionOptionsForTarget.Where<MissionOption>((MissionOption x) => !x.councilor.HasMission && x.mission.CanAfford(x.councilor.faction, x.councilor)).ToList<MissionOption>();
					}
					this.ActivateTargetingPanel(list, this.currentTarget);
					return;
				}
				this.targetingPanel.enabled = false;
			}
		}

		// Token: 0x06004F57 RID: 20311 RVA: 0x002240C0 File Offset: 0x002222C0
		public void ToggleShowAssignedCouncilorsInTargetingPanel()
		{
			this.showAssignedCouncilorsInTargetingPanel = !this.showAssignedCouncilorsInTargetingPanel;
			this.RefreshTargetingPanel();
		}

		// Token: 0x06004F58 RID: 20312 RVA: 0x002240D8 File Offset: 0x002222D8
		private List<TIGameState> GetTargetableGameStatesInRegion(TIRegionState selectedRegion)
		{
			List<TIGameState> list = new List<TIGameState> { selectedRegion, selectedRegion.nation };
			list.AddRange(selectedRegion.nation.EnemyControlPoints(base.activePlayer));
			list.AddRange(selectedRegion.GetVisibleCouncilorsInRegion(base.activePlayer));
			list.AddRange(selectedRegion.spaceFacilities.Where<TIRegionSpaceFacilityState>((TIRegionSpaceFacilityState x) => x.Extant()).ToList<TIRegionSpaceFacilityState>());
			if (selectedRegion.alienActivity.VisibleToFaction(base.activePlayer))
			{
				list.Add(selectedRegion.alienActivity);
			}
			if (selectedRegion.alienCrashdown.Extant() && selectedRegion.alienCrashdown.VisibleToFaction(base.activePlayer))
			{
				list.Add(selectedRegion.alienCrashdown);
			}
			if (selectedRegion.alienLanding.Extant() && selectedRegion.alienLanding.VisibleToFaction(base.activePlayer))
			{
				list.Add(selectedRegion.alienLanding);
			}
			if (selectedRegion.alienFacility.Extant() && selectedRegion.alienFacility.VisibleToFaction(base.activePlayer))
			{
				list.Add(selectedRegion.alienFacility);
			}
			if (selectedRegion.xenoforming.Extant() && selectedRegion.xenoforming.VisibleToFaction(base.activePlayer))
			{
				list.Add(selectedRegion.xenoforming);
			}
			return list;
		}

		// Token: 0x06004F59 RID: 20313 RVA: 0x0022422C File Offset: 0x0022242C
		private void GetNextTargetableGameStateInRegion(List<TIGameState> targetListForRegion)
		{
			int num = targetListForRegion.FindIndex((TIGameState x) => x == this.currentTarget);
			if (num == targetListForRegion.Count - 1)
			{
				this.currentTarget = targetListForRegion[0];
				return;
			}
			this.currentTarget = targetListForRegion[num + 1];
		}

		// Token: 0x06004F5A RID: 20314 RVA: 0x00224274 File Offset: 0x00222474
		public void DisableTargetingPanel()
		{
			if (this.targetingPanel.enabled)
			{
				this.targetingPanel.enabled = false;
				this.originalTarget = null;
				if (this.storedFinderStatus)
				{
					this.OpenFinderCanvas();
				}
			}
		}

		// Token: 0x06004F5B RID: 20315 RVA: 0x002242A4 File Offset: 0x002224A4
		private void InitializeFinderList()
		{
			this.finderInit = true;
			this.showFinderArmies = TIGlobalValuesState.GlobalValues.showFinderArmies;
			this.showFinderCouncilors = TIGlobalValuesState.GlobalValues.showFinderCouncilors;
			this.showFinderFleets = TIGlobalValuesState.GlobalValues.showFinderFleets;
			this.showFinderHabs = TIGlobalValuesState.GlobalValues.showFinderHabs;
			this.UpdateFinderToggleSprites();
			base.StartCoroutine(this.UpdateFinderWithDelay());
		}

		// Token: 0x06004F5C RID: 20316 RVA: 0x0022430B File Offset: 0x0022250B
		private IEnumerator UpdateFinderWithDelay()
		{
			yield return null;
			this.UpdateFinderList();
			yield break;
		}

		// Token: 0x06004F5D RID: 20317 RVA: 0x0022431C File Offset: 0x0022251C
		public void UpdateFinderList()
		{
			if (!this.finderInit)
			{
				return;
			}
			List<TIGameState> list = this.FinderItems(true);
			int num = list.Count;
			this.finderListModels.Clear();
			foreach (TIGameState tigameState in list)
			{
				FinderListItemModel finderListItemModel = new FinderListItemModel();
				FinderListItem_Data finderListItem_Data = new FinderListItem_Data();
				bool flag = false;
				if (tigameState.isCouncilorState && !this.showFinderCouncilors)
				{
					flag = true;
				}
				if (tigameState.isArmyState && !this.showFinderArmies)
				{
					flag = true;
				}
				if (tigameState.isHabState && !this.showFinderHabs)
				{
					flag = true;
				}
				if (tigameState.isSpaceFleetState && !this.showFinderFleets)
				{
					flag = true;
				}
				finderListItem_Data.gameState = tigameState;
				finderListItem_Data.showInList = !flag;
				finderListItem_Data.controller = this;
				finderListItem_Data.showEditMode = this.finderEditModeEnabled;
				if (flag)
				{
					num--;
				}
				finderListItemModel.finderListItemData = finderListItem_Data;
				this.finderListModels.Add(finderListItemModel);
			}
			this.finderListAdapter.SetItems(this.finderListModels);
			int num2 = Mathf.Min(this.finderMaxItems, num);
			this.finderTransform.sizeDelta = new Vector2(this.finderTransform.sizeDelta.x, (float)((num2 > 0) ? (6 + 29 * num2) : 29));
		}

		// Token: 0x06004F5E RID: 20318 RVA: 0x0022447C File Offset: 0x0022267C
		public void OnToggleFinderEditMode()
		{
			AudioManager.PlayOneShot(this.finderEditModeEnabled ? "event:/SFX/UI_SFX/trig_SFX_CloseSmall" : "event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.finderEditModeEnabled = !this.finderEditModeEnabled;
			this.UpdateFinderList();
		}

		// Token: 0x06004F5F RID: 20319 RVA: 0x002244AE File Offset: 0x002226AE
		public void UpdateFinderMaxHeight()
		{
			if (TIPlayerProfileManager.uiScaleSetting > 0)
			{
				this.finderMaxHeight *= this.finderLargeUIScaleHeightFactor;
				return;
			}
			this.finderMaxHeight /= this.finderLargeUIScaleHeightFactor;
		}

		// Token: 0x06004F60 RID: 20320 RVA: 0x002244DF File Offset: 0x002226DF
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.UpdateFinderMaxHeight();
			this.UpdateFinderList();
		}

		// Token: 0x06004F61 RID: 20321 RVA: 0x002244F4 File Offset: 0x002226F4
		private void UpdateFinderToggleSprites()
		{
			this.finderArmiesButton.image.sprite = (this.showFinderArmies ? this.finderFilterOnSprite : this.finderFilterOffSprite);
			this.finderCouncilorsButton.image.sprite = (this.showFinderCouncilors ? this.finderFilterOnSprite : this.finderFilterOffSprite);
			this.finderFleetsButton.image.sprite = (this.showFinderFleets ? this.finderFilterOnSprite : this.finderFilterOffSprite);
			this.finderHabsButton.image.sprite = (this.showFinderHabs ? this.finderFilterOnSprite : this.finderFilterOffSprite);
		}

		// Token: 0x06004F62 RID: 20322 RVA: 0x0022459C File Offset: 0x0022279C
		public void ToggleFinderCouncilors(bool onlyThis = false)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (!this.showFinderCouncilors)
			{
				if (!this.showFinderCouncilors)
				{
					if (!onlyThis)
					{
						this.showFinderCouncilors = true;
						TIGlobalValuesState.GlobalValues.showFinderCouncilors = true;
						this.finderCouncilorsButton.image.sprite = this.finderFilterOnSprite;
						this.UpdateFinderList();
						return;
					}
					this.FinderFilterExclusiveCouncilors();
				}
				return;
			}
			if (!onlyThis)
			{
				this.showFinderCouncilors = false;
				TIGlobalValuesState.GlobalValues.showFinderCouncilors = false;
				this.finderCouncilorsButton.image.sprite = this.finderFilterOffSprite;
				this.UpdateFinderList();
				return;
			}
			if (!this.finderToggleExclusive)
			{
				this.ToggleAllFinderFiltersOn();
				return;
			}
			this.FinderFilterExclusiveCouncilors();
			this.finderToggleExclusive = false;
		}

		// Token: 0x06004F63 RID: 20323 RVA: 0x0022464C File Offset: 0x0022284C
		public void ToggleFinderArmies(bool onlyThis = false)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (!this.showFinderArmies)
			{
				if (!this.showFinderArmies)
				{
					if (!onlyThis)
					{
						this.showFinderArmies = true;
						TIGlobalValuesState.GlobalValues.showFinderArmies = true;
						this.finderArmiesButton.image.sprite = this.finderFilterOnSprite;
						this.UpdateFinderList();
						return;
					}
					this.FinderFilterExclusiveArmies();
				}
				return;
			}
			if (!onlyThis)
			{
				this.showFinderArmies = false;
				TIGlobalValuesState.GlobalValues.showFinderArmies = false;
				this.finderArmiesButton.image.sprite = this.finderFilterOffSprite;
				this.UpdateFinderList();
				return;
			}
			if (!this.finderToggleExclusive)
			{
				this.ToggleAllFinderFiltersOn();
				return;
			}
			this.FinderFilterExclusiveArmies();
			this.finderToggleExclusive = false;
		}

		// Token: 0x06004F64 RID: 20324 RVA: 0x002246FC File Offset: 0x002228FC
		public void ToggleFinderHabs(bool onlyThis = false)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (!this.showFinderHabs)
			{
				if (!this.showFinderHabs)
				{
					if (!onlyThis)
					{
						this.showFinderHabs = true;
						TIGlobalValuesState.GlobalValues.showFinderHabs = true;
						this.finderHabsButton.image.sprite = this.finderFilterOnSprite;
						this.UpdateFinderList();
						return;
					}
					this.FinderFilterExclusiveHabs();
				}
				return;
			}
			if (!onlyThis)
			{
				this.showFinderHabs = false;
				TIGlobalValuesState.GlobalValues.showFinderHabs = false;
				this.finderHabsButton.image.sprite = this.finderFilterOffSprite;
				this.UpdateFinderList();
				return;
			}
			if (!this.finderToggleExclusive)
			{
				this.ToggleAllFinderFiltersOn();
				return;
			}
			this.FinderFilterExclusiveHabs();
			this.finderToggleExclusive = false;
		}

		// Token: 0x06004F65 RID: 20325 RVA: 0x002247AC File Offset: 0x002229AC
		public void ToggleFinderFleets(bool onlyThis = false)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (!this.showFinderFleets)
			{
				if (!this.showFinderFleets)
				{
					if (!onlyThis)
					{
						this.showFinderFleets = true;
						TIGlobalValuesState.GlobalValues.showFinderFleets = true;
						this.finderFleetsButton.image.sprite = this.finderFilterOnSprite;
						this.UpdateFinderList();
						return;
					}
					this.FinderFilterExclusiveFleets();
				}
				return;
			}
			if (!onlyThis)
			{
				this.showFinderFleets = false;
				TIGlobalValuesState.GlobalValues.showFinderFleets = false;
				this.finderFleetsButton.image.sprite = this.finderFilterOffSprite;
				this.UpdateFinderList();
				return;
			}
			if (!this.finderToggleExclusive)
			{
				this.ToggleAllFinderFiltersOn();
				return;
			}
			this.FinderFilterExclusiveFleets();
			this.finderToggleExclusive = false;
		}

		// Token: 0x06004F66 RID: 20326 RVA: 0x0022485C File Offset: 0x00222A5C
		private void ToggleAllFinderFiltersOn()
		{
			this.showFinderArmies = true;
			this.showFinderCouncilors = true;
			this.showFinderFleets = true;
			this.showFinderHabs = true;
			TIGlobalValuesState.GlobalValues.showFinderArmies = true;
			TIGlobalValuesState.GlobalValues.showFinderCouncilors = true;
			TIGlobalValuesState.GlobalValues.showFinderFleets = true;
			TIGlobalValuesState.GlobalValues.showFinderHabs = true;
			this.UpdateFinderToggleSprites();
			this.finderToggleExclusive = true;
			this.UpdateFinderList();
		}

		// Token: 0x06004F67 RID: 20327 RVA: 0x002248C4 File Offset: 0x00222AC4
		private void FinderFilterExclusiveArmies()
		{
			this.showFinderArmies = true;
			this.showFinderCouncilors = false;
			this.showFinderFleets = false;
			this.showFinderHabs = false;
			TIGlobalValuesState.GlobalValues.showFinderArmies = true;
			TIGlobalValuesState.GlobalValues.showFinderCouncilors = false;
			TIGlobalValuesState.GlobalValues.showFinderFleets = false;
			TIGlobalValuesState.GlobalValues.showFinderHabs = false;
			this.UpdateFinderToggleSprites();
			this.UpdateFinderList();
		}

		// Token: 0x06004F68 RID: 20328 RVA: 0x00224928 File Offset: 0x00222B28
		private void FinderFilterExclusiveCouncilors()
		{
			this.showFinderArmies = false;
			this.showFinderCouncilors = true;
			this.showFinderFleets = false;
			this.showFinderHabs = false;
			TIGlobalValuesState.GlobalValues.showFinderArmies = false;
			TIGlobalValuesState.GlobalValues.showFinderCouncilors = true;
			TIGlobalValuesState.GlobalValues.showFinderFleets = false;
			TIGlobalValuesState.GlobalValues.showFinderHabs = false;
			this.UpdateFinderToggleSprites();
			this.UpdateFinderList();
		}

		// Token: 0x06004F69 RID: 20329 RVA: 0x0022498C File Offset: 0x00222B8C
		private void FinderFilterExclusiveHabs()
		{
			this.showFinderArmies = false;
			this.showFinderCouncilors = false;
			this.showFinderFleets = false;
			this.showFinderHabs = true;
			TIGlobalValuesState.GlobalValues.showFinderArmies = false;
			TIGlobalValuesState.GlobalValues.showFinderCouncilors = false;
			TIGlobalValuesState.GlobalValues.showFinderFleets = false;
			TIGlobalValuesState.GlobalValues.showFinderHabs = true;
			this.UpdateFinderToggleSprites();
			this.UpdateFinderList();
		}

		// Token: 0x06004F6A RID: 20330 RVA: 0x002249F0 File Offset: 0x00222BF0
		private void FinderFilterExclusiveFleets()
		{
			this.showFinderArmies = false;
			this.showFinderCouncilors = false;
			this.showFinderFleets = true;
			this.showFinderHabs = false;
			TIGlobalValuesState.GlobalValues.showFinderArmies = false;
			TIGlobalValuesState.GlobalValues.showFinderCouncilors = false;
			TIGlobalValuesState.GlobalValues.showFinderFleets = true;
			TIGlobalValuesState.GlobalValues.showFinderHabs = false;
			this.UpdateFinderToggleSprites();
			this.UpdateFinderList();
		}

		// Token: 0x06004F6B RID: 20331 RVA: 0x00224A51 File Offset: 0x00222C51
		public void FinderCycleForward()
		{
		}

		// Token: 0x06004F6C RID: 20332 RVA: 0x00224A53 File Offset: 0x00222C53
		public void FinderCycleBackward()
		{
		}

		// Token: 0x06004F6D RID: 20333 RVA: 0x00224A58 File Offset: 0x00222C58
		public void SetFinderIndex(TIGameState gameState)
		{
			List<TIGameState> list = this.FinderItems(true).ToList<TIGameState>();
			for (int i = 0; i < list.Count; i++)
			{
				if (gameState == list[i])
				{
					this.finderSelectedIndex = i;
				}
			}
		}

		// Token: 0x06004F6E RID: 20334 RVA: 0x00224A99 File Offset: 0x00222C99
		public void HighlightFinderItem(bool reset = false)
		{
		}

		// Token: 0x06004F6F RID: 20335 RVA: 0x00224A9B File Offset: 0x00222C9B
		private int VisibleFinderItems()
		{
			return this.finderListModels.Count<FinderListItemModel>((FinderListItemModel x) => x.finderListItemData.showInList);
		}

		// Token: 0x06004F70 RID: 20336 RVA: 0x00224AC8 File Offset: 0x00222CC8
		public List<TIGameState> FinderItems(bool init = true)
		{
			List<TIGameState> list = new List<TIGameState>();
			List<TIGameState> list2 = new List<TIGameState>();
			List<TIGameState> list3 = new List<TIGameState>();
			List<TIGameState> list4 = new List<TIGameState>();
			List<TIGameState> list5 = new List<TIGameState>();
			list2.AddRange(base.activePlayer.councilors);
			list2.AddRange(base.activePlayer.turnedCouncilors);
			list3.AddRange(from x in base.activePlayer.armies
				orderby x.AlienMegafaunaArmy descending
				orderby x.AlienRegularArmy descending
				orderby x.techLevel descending
				select x);
			list4.AddRange(from x in base.activePlayer.habs
				orderby x.IsStation descending
				orderby x.ref_hab.tier descending
				select x);
			list5.AddRange(base.activePlayer.fleets);
			if (init)
			{
				this.InitSortOverrides(list2);
				this.InitSortOverrides(list3);
				this.InitSortOverrides(list4);
				this.InitSortOverrides(list5);
			}
			list.AddRange(list2.OrderBy<TIGameState, int>((TIGameState x) => x.finderSortOverride));
			if (base.activePlayer.armies != null)
			{
				list.AddRange(list3.OrderBy<TIGameState, int>((TIGameState x) => x.finderSortOverride));
			}
			list.AddRange(list4.OrderBy<TIGameState, int>((TIGameState x) => x.finderSortOverride));
			list.AddRange(list5.OrderBy<TIGameState, int>((TIGameState x) => x.finderSortOverride));
			return list;
		}

		// Token: 0x06004F71 RID: 20337 RVA: 0x00224CDC File Offset: 0x00222EDC
		public void InitSortOverrides(List<TIGameState> gameStates)
		{
			foreach (TIGameState tigameState in gameStates)
			{
				if (tigameState.finderSortOverride == -1)
				{
					tigameState.finderSortOverride = gameStates.OrderByDescending<TIGameState, int>((TIGameState x) => x.finderSortOverride).FirstOrDefault<TIGameState>().finderSortOverride + 1;
				}
			}
			this.CleanFinderSortIndices(gameStates);
		}

		// Token: 0x06004F72 RID: 20338 RVA: 0x00224D6C File Offset: 0x00222F6C
		public void CleanFinderSortIndices(List<TIGameState> gameStates)
		{
			List<TIGameState> list = gameStates.OrderBy<TIGameState, int>((TIGameState x) => x.finderSortOverride).ToList<TIGameState>();
			int num = -1;
			for (int i = 0; i < list.Count; i++)
			{
				if (num >= 0 && list[i].finderSortOverride != num + 1)
				{
					int num2 = list[i].finderSortOverride - num;
					list[i].finderSortOverride -= num2 - 1;
				}
				num = list[i].finderSortOverride;
			}
		}

		// Token: 0x06004F73 RID: 20339 RVA: 0x00224DFE File Offset: 0x00222FFE
		private IEnumerator HideSearchAfterInit()
		{
			yield return null;
			this.HideGlobalSearchPanel(false);
			yield break;
		}

		// Token: 0x06004F74 RID: 20340 RVA: 0x00224E0D File Offset: 0x0022300D
		public void ShowGlobalSearchPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.searchObject.SetActive(true);
			this.OnGlobalSearchInputUpdated();
			this.searchInputField.ActivateInputField();
		}

		// Token: 0x06004F75 RID: 20341 RVA: 0x00224E38 File Offset: 0x00223038
		public void HideGlobalSearchPanel(bool playAudio = true)
		{
			if (playAudio)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			}
			this.searchObject.SetActive(false);
		}

		// Token: 0x06004F76 RID: 20342 RVA: 0x00224E58 File Offset: 0x00223058
		public void OnGlobalSearchInputUpdated()
		{
			string searchString = this.searchInputField.text.ToLower();
			searchString = TIUtilities.StripDiacriticsFromString(searchString);
			if (this.searchInputField.text.Length > 1)
			{
				List<TIGameState> list = (from x in GameStateManager.IterateByClass<TIGameState>(true)
					where x.GetDisplayName(this.activePlayer) != null && TIUtilities.StripDiacriticsFromString(x.GetDisplayName(this.activePlayer).ToLower()).Contains(searchString) && x.searchable > Searchable.never
					select x).ToList<TIGameState>();
				this.SetGlobalSearchListModelData(list);
				return;
			}
			this.globalSearchListItemModels.Clear();
			this.globalSearchListAdapter.SetItems(this.globalSearchListItemModels);
		}

		// Token: 0x06004F77 RID: 20343 RVA: 0x00224EF0 File Offset: 0x002230F0
		public void SetGlobalSearchListModelData(List<TIGameState> gameStates)
		{
			this.globalSearchListItemModels.Clear();
			for (int i = 0; i < gameStates.Count; i++)
			{
				TIGameState tigameState = gameStates[i];
				if (tigameState.searchable != Searchable.withIntel || (this.CanDisplaySearchableGameStateWithIntel(tigameState) && (!tigameState.isSpaceShipState || TIGameState.Valid(tigameState.ref_fleet))))
				{
					GlobalSearchListItemModel globalSearchListItemModel = new GlobalSearchListItemModel();
					globalSearchListItemModel.globalSearchListItemData = new GlobalSearchListItem_Data
					{
						gameState = gameStates[i],
						controller = this
					};
					this.globalSearchListItemModels.Add(globalSearchListItemModel);
				}
			}
			this.globalSearchListAdapter.SetItems(this.globalSearchListItemModels);
		}

		// Token: 0x06004F78 RID: 20344 RVA: 0x00224F8C File Offset: 0x0022318C
		public bool CanDisplaySearchableGameStateWithIntel(TIGameState gameState)
		{
			if (gameState.isSpaceAssetState && !base.activePlayer.HasIntelOnSpaceAssetLocation(gameState.ref_spaceAsset))
			{
				return false;
			}
			if (gameState.isCouncilorState && !base.activePlayer.HasIntelOnCouncilorLocation(gameState.ref_councilor))
			{
				return false;
			}
			if (gameState.isOrgState)
			{
				if (gameState.ref_org.assignedCouncilor != null && !base.activePlayer.HasIntelOnCouncilorDetails(gameState.ref_org.ref_councilor))
				{
					return false;
				}
				if (gameState.ref_org.factionOrbit != null && (gameState.ref_org.factionOrbit.unassignedOrgs.Contains(gameState.ref_org) || base.activePlayer != gameState.ref_org.factionOrbit))
				{
					return false;
				}
				if (gameState.ref_org.factionOrbit == null)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004F79 RID: 20345 RVA: 0x0022506C File Offset: 0x0022326C
		private void ShowResourcesPanel()
		{
			this.resourceSalePanel.SetActive(true);
			this.proposedResourceSales = new Dictionary<FactionResource, int>();
			List<FactionResource> list = base.activePlayer.SellableResourcesOnEarth();
			foreach (FactionResource factionResource in list)
			{
				this.proposedResourceSales.Add(factionResource, 0);
			}
			this.resourceSalesList.SetListSize<SellResourceListItemController>(this.proposedResourceSales.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator2 = this.resourceSalesList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (GeneralControlsController.<>o__377.<>p__0 == null)
					{
						GeneralControlsController.<>o__377.<>p__0 = CallSite<Func<CallSite, object, SellResourceListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SellResourceListItemController), typeof(GeneralControlsController)));
					}
					SellResourceListItemController sellResourceListItemController = GeneralControlsController.<>o__377.<>p__0.Target(GeneralControlsController.<>o__377.<>p__0, enumerator2.Current);
					sellResourceListItemController.Initialize(this, list[num++]);
					sellResourceListItemController.UpdateListItem(0);
				}
			}
			this.totalSaleValueText.SetText("0");
			this.confirmSaleButton.interactable = false;
			base.gameTime.Pause();
		}

		// Token: 0x06004F7A RID: 20346 RVA: 0x002251B8 File Offset: 0x002233B8
		private void UpdateResourcesPanel()
		{
			using (IEnumerator<object> enumerator = this.resourceSalesList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GeneralControlsController.<>o__378.<>p__0 == null)
					{
						GeneralControlsController.<>o__378.<>p__0 = CallSite<Func<CallSite, object, SellResourceListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SellResourceListItemController), typeof(GeneralControlsController)));
					}
					SellResourceListItemController sellResourceListItemController = GeneralControlsController.<>o__378.<>p__0.Target(GeneralControlsController.<>o__378.<>p__0, enumerator.Current);
					if (sellResourceListItemController.resource != FactionResource.None)
					{
						sellResourceListItemController.UpdateListItem(this.proposedResourceSales[sellResourceListItemController.resource]);
					}
				}
			}
			float num = this.proposedResourceSales.Sum<KeyValuePair<FactionResource, int>>((KeyValuePair<FactionResource, int> x) => (float)x.Value * TIGlobalValuesState.GlobalValues.GetModifiedResourceMarketValueForSelling(base.activePlayer, x.Key));
			this.totalSaleValueText.SetText(TIUtilities.FormatSmallNumber(num, 7, 0, true, false));
			this.confirmSaleButton.interactable = num > 0f;
		}

		// Token: 0x06004F7B RID: 20347 RVA: 0x002252A0 File Offset: 0x002234A0
		private void ResetResourcesPanel()
		{
			using (IEnumerator<object> enumerator = this.resourceSalesList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GeneralControlsController.<>o__379.<>p__0 == null)
					{
						GeneralControlsController.<>o__379.<>p__0 = CallSite<Func<CallSite, object, SellResourceListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SellResourceListItemController), typeof(GeneralControlsController)));
					}
					SellResourceListItemController sellResourceListItemController = GeneralControlsController.<>o__379.<>p__0.Target(GeneralControlsController.<>o__379.<>p__0, enumerator.Current);
					this.proposedResourceSales[sellResourceListItemController.resource] = 0;
				}
			}
			this.UpdateResourcesPanel();
		}

		// Token: 0x06004F7C RID: 20348 RVA: 0x00225340 File Offset: 0x00223540
		public void OnSelectInputField()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x06004F7D RID: 20349 RVA: 0x00225347 File Offset: 0x00223547
		public void OnDeSelectInputField()
		{
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x06004F7E RID: 20350 RVA: 0x00225350 File Offset: 0x00223550
		public void ChangeProposedSale(FactionResource resource, int value, bool increment = true)
		{
			if (increment)
			{
				Dictionary<FactionResource, int> dictionary = this.proposedResourceSales;
				dictionary[resource] += value;
			}
			else
			{
				this.proposedResourceSales[resource] = value;
			}
			this.proposedResourceSales[resource] = Mathf.Clamp(this.proposedResourceSales[resource], 0, (int)Math.Truncate((double)base.activePlayer.GetCurrentResourceAmount(resource)));
			this.UpdateResourcesPanel();
		}

		// Token: 0x06004F7F RID: 20351 RVA: 0x002253BF File Offset: 0x002235BF
		public void OnResetSaleClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.ResetResourcesPanel();
		}

		// Token: 0x06004F80 RID: 20352 RVA: 0x002253D3 File Offset: 0x002235D3
		public void OnConfirmSaleClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new SellSpaceResourcesToEarthAction(base.activePlayer, this.proposedResourceSales));
			this.ResetResourcesPanel();
		}

		// Token: 0x06004F81 RID: 20353 RVA: 0x00225408 File Offset: 0x00223608
		public void OnCloseSellResourcesPanelClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.resourceSalePanel.SetActive(false);
		}

		// Token: 0x06004F82 RID: 20354 RVA: 0x00225422 File Offset: 0x00223622
		public void RefreshAlienThreatPanel(AlienThreatUpdated e)
		{
			this.RefreshAlienThreatPanel();
		}

		// Token: 0x17000ECB RID: 3787
		// (get) Token: 0x06004F83 RID: 20355 RVA: 0x0022542C File Offset: 0x0022362C
		public string alienThreatTip
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Intel.AlienThreatTip"));
				if (base.activePlayer.GetLastDateofFixedAlienHate() != null)
				{
					stringBuilder.AppendLine().AppendLine().AppendLine(Loc.T("UI.Intel.AlienThreatFixed", new object[] { base.activePlayer.GetLastDateofFixedAlienHate().ToCustomDateString() }));
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x06004F84 RID: 20356 RVA: 0x00225498 File Offset: 0x00223698
		public void RefreshAlienThreatPanel()
		{
			if (!base.activePlayer.IsAlienProxy && TIEffectsState.SumEffectsModifiers(Context.DetectAlienActivity, base.activePlayer, 0f, null) >= 3f && !TIGlobalValuesState.isSpaceCombatEnabled)
			{
				this.alienThreatPanel.SetActive(true);
				float alienFactionHateWarValue = TemplateManager.global.alienFactionHateWarValue;
				float estimatedAlienHate = base.activePlayer.GetEstimatedAlienHate();
				TIFactionGoalState tifactionGoalState = GameStateManager.AlienFaction().FindGoals(GoalType.WarOnFaction, GameStateManager.AlienFaction(), base.activePlayer, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>();
				float num = ((tifactionGoalState != null) ? 1f : (estimatedAlienHate / alienFactionHateWarValue));
				if (tifactionGoalState != null)
				{
					FactionGoal_WarOnFaction factionGoal_WarOnFaction = tifactionGoalState as FactionGoal_WarOnFaction;
					if (factionGoal_WarOnFaction != null && factionGoal_WarOnFaction.IsTotalWar)
					{
						this.alienAlertLights[0].color = new Color(1f, 0.2f, 0.2f);
						this.alienAlertLights[1].color = new Color(1f, 0.2f, 0.2f);
						this.alienAlertLights[2].color = new Color(1f, 0.2f, 0.2f);
						this.alienAlertLights[3].color = new Color(1f, 0.2f, 0.2f);
						this.alienAlertLights[4].color = new Color(1f, 0.2f, 0.2f);
						goto IL_0298;
					}
				}
				this.alienAlertLights[0].color = ((num >= 0.2f) ? new Color(0f, 0.5f, 0f) : new Color(0.25f, 0.25f, 0.25f));
				this.alienAlertLights[1].color = ((num >= 0.4f) ? new Color(1f, 1f, 0f) : new Color(0.25f, 0.25f, 0.25f));
				this.alienAlertLights[2].color = ((num >= 0.6f) ? new Color(1f, 0.8f, 0f) : new Color(0.25f, 0.25f, 0.25f));
				this.alienAlertLights[3].color = ((num >= 0.8f) ? new Color(1f, 0.6f, 0f) : new Color(0.25f, 0.25f, 0.25f));
				this.alienAlertLights[4].color = ((num >= 1f) ? new Color(1f, 0.2f, 0.2f) : new Color(0.25f, 0.25f, 0.25f));
				IL_0298:
				this.alienThreatUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_IntelScreenCanvas_AlienThreat, false, true);
				return;
			}
			this.alienThreatPanel.SetActive(false);
		}

		// Token: 0x06004F85 RID: 20357 RVA: 0x0022575C File Offset: 0x0022395C
		private void InitializeAlarmPanel()
		{
			this.alarmPanel.SetActive(false);
			this.alarmPanelHeader.SetText(Loc.T("UI.GeneralControls.Alarm.SetAlarmHeader"));
			this.resetProposedAlarmTimeButtonText.SetText(Loc.T("UI.GeneralControls.Alarm.ResetTime"));
			this.confirmAlarmAndCloseButtonText.SetText(Loc.T("UI.GeneralControls.Alarm.ConfirmAlarmAndCloseButtonText"));
			this.confirmAlarmAndDoAnotherButtonText.SetText(Loc.T("UI.GeneralControls.Alarm.ConfirmAlarmAndDoAnotherButtonText"));
			this.cancelAlarmButtonText.SetText(Loc.T("UI.GeneralControls.Alarm.CancelAlarmButtonText"));
			this.proposedAlarmText.SetTextWithoutNotify(Loc.T("UI.GeneralControls.Alarm.Helper"));
			this.placeholderText.SetText(Loc.T("UI.GeneralControls.Alarm.Helper"));
			this.openCalendarButtonText.SetText(Loc.T("UI.GeneralControls.Alarm.OpenCalendarButtonText"));
			this.alarmMinuteDropdown.ClearOptions();
			for (int i = 0; i <= 59; i++)
			{
				this.alarmMinuteDropdown.options.Add(new TMP_Dropdown.OptionData
				{
					text = i.ToString("D2")
				});
			}
			this.alarmHourDropdown.ClearOptions();
			for (int j = 0; j < 24; j++)
			{
				DateTime dateTime = new DateTime(2000, 1, 1, j, 0, 0);
				this.alarmHourDropdown.options.Add(new TMP_Dropdown.OptionData
				{
					text = dateTime.ToString("HH")
				});
			}
			this.alarmMonthDropdown.ClearOptions();
			for (int k = 1; k <= 12; k++)
			{
				this.alarmMonthDropdown.options.Add(new TMP_Dropdown.OptionData
				{
					text = TIDateTime.GetMonthString(k)
				});
			}
		}

		// Token: 0x06004F86 RID: 20358 RVA: 0x002258E8 File Offset: 0x00223AE8
		public void OpenAlarmPanel(TIDateTime setTime, string defaultString = "")
		{
			TIInputManager.BlockKeybindings();
			base.gameTime.PauseAndBlock();
			this.alarmYearDropdown.ClearOptions();
			for (int i = 0; i < 5; i++)
			{
				this.alarmYearDropdown.options.Add(new TMP_Dropdown.OptionData
				{
					text = (TITimeState.Now().year + i).ToString()
				});
			}
			this.SetDayDropdown(setTime.month, setTime.year);
			this.ProposeAlarm(setTime);
			if (defaultString != "")
			{
				this.proposedAlarmText.text = defaultString;
			}
			else
			{
				this.proposedAlarmText.SetTextWithoutNotify(string.Empty);
			}
			this.openCalendarButton.gameObject.SetActive(!base.canvasManager.IsShowingInfoScreen<CouncilGridController>());
			this.alarmPanel.SetActive(true);
		}

		// Token: 0x06004F87 RID: 20359 RVA: 0x002259BC File Offset: 0x00223BBC
		public void OnResetAlarmPanelTime()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddHours(1.0);
			this.ProposeAlarm(tidateTime);
		}

		// Token: 0x06004F88 RID: 20360 RVA: 0x002259F4 File Offset: 0x00223BF4
		public void OnOpenAlarmPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddHours(1.0);
			this.OpenAlarmPanel(tidateTime, "");
		}

		// Token: 0x06004F89 RID: 20361 RVA: 0x00225A30 File Offset: 0x00223C30
		private void ProposeAlarm(TIDateTime setTime)
		{
			TIDateTime tidateTime = TITimeState.Now();
			if (setTime <= tidateTime)
			{
				setTime = new TIDateTime(tidateTime);
				setTime.AddSeconds(60.0);
			}
			TIDateTime tidateTime2 = new TIDateTime(tidateTime);
			tidateTime2.AddYears(5);
			tidateTime2.AddSeconds(60.0);
			if (setTime > tidateTime2)
			{
				setTime = new TIDateTime(tidateTime2);
				setTime.AddSeconds(-60.0);
			}
			this.proposedAlarmTime = new TIDateTime(setTime);
			this.SetDayDropdown(setTime.month, setTime.year);
			this.alarmMinuteDropdown.SetValueWithoutNotify(this.proposedAlarmTime.minute);
			this.alarmMinuteDropdown.captionText.SetText(this.proposedAlarmTime.minute.ToString("D2"));
			this.alarmMinuteDropdown.RefreshShownValue();
			this.alarmHourDropdown.SetValueWithoutNotify(this.proposedAlarmTime.hour);
			this.alarmHourDropdown.captionText.SetText(this.proposedAlarmTime.hour.ToString("HH"));
			this.alarmHourDropdown.RefreshShownValue();
			this.alarmDayDropdown.SetValueWithoutNotify(this.proposedAlarmTime.day - 1);
			this.alarmDayDropdown.captionText.SetText(this.proposedAlarmTime.day.ToString());
			this.alarmDayDropdown.RefreshShownValue();
			this.alarmMonthDropdown.SetValueWithoutNotify(this.proposedAlarmTime.month - 1);
			this.alarmMonthDropdown.captionText.SetText(TIDateTime.GetMonthString(this.proposedAlarmTime.month));
			this.alarmMonthDropdown.RefreshShownValue();
			this.alarmYearDropdown.SetValueWithoutNotify(this.proposedAlarmTime.year - tidateTime.year);
			this.alarmYearDropdown.captionText.SetText(this.proposedAlarmTime.year.ToString());
			this.alarmYearDropdown.RefreshShownValue();
			this.confirmCloseButton.interactable = this.proposedAlarmTime > tidateTime && this.proposedAlarmTime < tidateTime2;
			this.confirmContinueButton.interactable = this.proposedAlarmTime > tidateTime && this.proposedAlarmTime < tidateTime2;
			this.decreaseMinuteButton.interactable = this.proposedAlarmTime > tidateTime;
			this.increaseMinuteButton.interactable = this.proposedAlarmTime < tidateTime2;
			this.decreaseHourButton.interactable = this.proposedAlarmTime > tidateTime;
			this.increaseHourButton.interactable = this.proposedAlarmTime < tidateTime2;
			this.decreaseDayButton.interactable = this.proposedAlarmTime > tidateTime;
			this.increaseDayButton.interactable = this.proposedAlarmTime < tidateTime2;
			this.decreaseMonthButton.interactable = this.proposedAlarmTime > tidateTime;
			this.increaseMonthButton.interactable = this.proposedAlarmTime < tidateTime2;
			this.decreaseYearButton.interactable = this.proposedAlarmTime > tidateTime;
			this.increaseYearButton.interactable = this.proposedAlarmTime < tidateTime2;
		}

		// Token: 0x06004F8A RID: 20362 RVA: 0x00225D50 File Offset: 0x00223F50
		public void ProposeAlarmFromSettings()
		{
			if (this.proposedAlarmTime.month != this.alarmMonthDropdown.value + 1 || (this.alarmMonthDropdown.value == 1 && this.proposedAlarmTime.year - TITimeState.Now().year != this.alarmYearDropdown.value))
			{
				int day = this.proposedAlarmTime.day;
				this.proposedAlarmTime.month = this.alarmMonthDropdown.value + 1;
				int num = this.SetDayDropdown(this.proposedAlarmTime.month, this.proposedAlarmTime.year);
				if (day > num)
				{
					this.proposedAlarmTime.day = num;
				}
				else
				{
					this.proposedAlarmTime.day = day;
					this.alarmDayDropdown.SetValueWithoutNotify(this.proposedAlarmTime.day);
				}
			}
			TIDateTime tidateTime = new TIDateTime
			{
				minute = this.alarmMinuteDropdown.value,
				hour = this.alarmHourDropdown.value,
				day = this.alarmDayDropdown.value + 1,
				month = this.alarmMonthDropdown.value + 1,
				year = TITimeState.Now().year + this.alarmYearDropdown.value
			};
			this.ProposeAlarm(tidateTime);
		}

		// Token: 0x06004F8B RID: 20363 RVA: 0x00225E91 File Offset: 0x00224091
		public void OnAlarmDropdownChanged()
		{
			this.ProposeAlarmFromSettings();
		}

		// Token: 0x06004F8C RID: 20364 RVA: 0x00225E9C File Offset: 0x0022409C
		private int SetDayDropdown(int month, int year)
		{
			int num = DateTime.DaysInMonth(year, month);
			this.alarmDayDropdown.ClearOptions();
			for (int i = 1; i <= num; i++)
			{
				this.alarmDayDropdown.options.Add(new TMP_Dropdown.OptionData
				{
					text = i.ToString()
				});
			}
			return num;
		}

		// Token: 0x06004F8D RID: 20365 RVA: 0x00225EEB File Offset: 0x002240EB
		public void OnOpenCalendarFromAlarmPanel()
		{
			this.Councilors();
			base.canvasManager.GetInfoScreen<CouncilGridController>().ForceOpenCalendar();
			this.openCalendarButton.gameObject.SetActive(false);
		}

		// Token: 0x06004F8E RID: 20366 RVA: 0x00225F14 File Offset: 0x00224114
		public void OnCloseAlarmPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.alarmPanel.SetActive(false);
			base.gameTime.UnBlock();
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x06004F8F RID: 20367 RVA: 0x00225F40 File Offset: 0x00224140
		public void OnConfirmAlarm(bool alsoClose)
		{
			if (!(this.proposedAlarmTime > TITimeState.Now()))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			base.activePlayer.playerControl.StartAction(new SetUserAlarmAction(base.activePlayer, base.activePlayer, AlarmType.PlayerAlarm, this.proposedAlarmTime, this.proposedAlarmText.text));
			if (alsoClose)
			{
				this.OnCloseAlarmPanel();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		}

		// Token: 0x06004F90 RID: 20368 RVA: 0x00225FB8 File Offset: 0x002241B8
		public void CycleMinute(bool forward)
		{
			TIDateTime tidateTime = new TIDateTime(this.proposedAlarmTime);
			tidateTime.AddSeconds((double)(forward ? 60 : (-60)));
			AudioManager.PlayOneShot(forward ? "event:/SFX/UI_SFX/trig_SFX_CycleForward" : "event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.ProposeAlarm(tidateTime);
		}

		// Token: 0x06004F91 RID: 20369 RVA: 0x00226000 File Offset: 0x00224200
		public void CycleHour(bool forward)
		{
			TIDateTime tidateTime = new TIDateTime(this.proposedAlarmTime);
			tidateTime.AddHours((double)(forward ? 1 : (-1)));
			AudioManager.PlayOneShot(forward ? "event:/SFX/UI_SFX/trig_SFX_CycleForward" : "event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.ProposeAlarm(tidateTime);
		}

		// Token: 0x06004F92 RID: 20370 RVA: 0x00226044 File Offset: 0x00224244
		public void CycleDay(bool forward)
		{
			TIDateTime tidateTime = new TIDateTime(this.proposedAlarmTime);
			tidateTime.AddDays((float)(forward ? 1 : (-1)));
			AudioManager.PlayOneShot(forward ? "event:/SFX/UI_SFX/trig_SFX_CycleForward" : "event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.ProposeAlarm(tidateTime);
		}

		// Token: 0x06004F93 RID: 20371 RVA: 0x00226088 File Offset: 0x00224288
		public void CycleMonth(bool forward)
		{
			TIDateTime tidateTime = new TIDateTime(this.proposedAlarmTime);
			tidateTime.AddMonths(forward ? 1 : (-1));
			AudioManager.PlayOneShot(forward ? "event:/SFX/UI_SFX/trig_SFX_CycleForward" : "event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.ProposeAlarm(tidateTime);
		}

		// Token: 0x06004F94 RID: 20372 RVA: 0x002260CC File Offset: 0x002242CC
		public void CycleYear(bool forward)
		{
			TIDateTime tidateTime = new TIDateTime(this.proposedAlarmTime);
			tidateTime.AddYears(forward ? 1 : (-1));
			AudioManager.PlayOneShot(forward ? "event:/SFX/UI_SFX/trig_SFX_CycleForward" : "event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.ProposeAlarm(tidateTime);
		}

		// Token: 0x06004F95 RID: 20373 RVA: 0x0022610F File Offset: 0x0022430F
		public override void SetUltraWideScaling()
		{
			base.SetUltraWideScaling();
			if (!TutorialTip.InstanceNull)
			{
				TutorialTip.Instance.SetCanvasScaling();
			}
		}

		// Token: 0x04003236 RID: 12854
		public Image factionIcon;

		// Token: 0x04003237 RID: 12855
		public TooltipTrigger objectivesTooltipTrigger;

		// Token: 0x04003238 RID: 12856
		[Header("Tutorials")]
		public UITutorialController mainHUDTutorialController;

		// Token: 0x04003239 RID: 12857
		public UITutorialController introTutorialNewController;

		// Token: 0x0400323A RID: 12858
		public UITutorialController sellResourcesTutorialController;

		// Token: 0x0400323B RID: 12859
		public GameObject fakeTutorialWindow;

		// Token: 0x0400323C RID: 12860
		public GameObject introOptinHighlightDummy;

		// Token: 0x0400323D RID: 12861
		[Header("Resources Data")]
		public TMP_Text incomeInfoText;

		// Token: 0x0400323E RID: 12862
		public TooltipTrigger moneyTooltipTrigger;

		// Token: 0x0400323F RID: 12863
		public TMP_Text influenceInfoText;

		// Token: 0x04003240 RID: 12864
		public TooltipTrigger influenceTooltipTrigger;

		// Token: 0x04003241 RID: 12865
		public TMP_Text operationInfoText;

		// Token: 0x04003242 RID: 12866
		public TooltipTrigger opsTooltipTrigger;

		// Token: 0x04003243 RID: 12867
		public TMP_Text researchInfoText;

		// Token: 0x04003244 RID: 12868
		public TooltipTrigger researchTooltipTrigger;

		// Token: 0x04003245 RID: 12869
		public Image controlPointImage;

		// Token: 0x04003246 RID: 12870
		public TMP_Text controlPointMaintenanceText;

		// Token: 0x04003247 RID: 12871
		public TooltipTrigger controlPointMaintenanceTrigger;

		// Token: 0x04003248 RID: 12872
		public TMP_Text boostInfoText;

		// Token: 0x04003249 RID: 12873
		public TooltipTrigger boostTooltipTrigger;

		// Token: 0x0400324A RID: 12874
		public TMP_Text missionControlInfoText;

		// Token: 0x0400324B RID: 12875
		public TooltipTrigger missionControlTooltipTrigger;

		// Token: 0x0400324C RID: 12876
		public TMP_Text waterInfoText;

		// Token: 0x0400324D RID: 12877
		public TooltipTrigger waterTooltipTrigger;

		// Token: 0x0400324E RID: 12878
		public TMP_Text volatilesInfoText;

		// Token: 0x0400324F RID: 12879
		public TooltipTrigger volatilesTooltipTrigger;

		// Token: 0x04003250 RID: 12880
		public TMP_Text baseMetalsInfoText;

		// Token: 0x04003251 RID: 12881
		public TooltipTrigger baseMetalsTooltipTrigger;

		// Token: 0x04003252 RID: 12882
		public TMP_Text nobleMetalsInfoText;

		// Token: 0x04003253 RID: 12883
		public TooltipTrigger nobleMetalsTooltipTrigger;

		// Token: 0x04003254 RID: 12884
		public TMP_Text fissilesInfoText;

		// Token: 0x04003255 RID: 12885
		public TooltipTrigger fissilesTooltipTrigger;

		// Token: 0x04003256 RID: 12886
		public TMP_Text antimatterInfoText;

		// Token: 0x04003257 RID: 12887
		public TooltipTrigger antimatterTooltipTrigger;

		// Token: 0x04003258 RID: 12888
		public TMP_Text exoticsInfoText;

		// Token: 0x04003259 RID: 12889
		public TooltipTrigger exoticsTooltipTrigger;

		// Token: 0x0400325A RID: 12890
		public Transform waterPanel;

		// Token: 0x0400325B RID: 12891
		public Transform volatilesPanel;

		// Token: 0x0400325C RID: 12892
		public Transform baseMetalsPanel;

		// Token: 0x0400325D RID: 12893
		public Transform nobleMetalsPanel;

		// Token: 0x0400325E RID: 12894
		public Transform fissilesPanel;

		// Token: 0x0400325F RID: 12895
		public Transform antimatterPanel;

		// Token: 0x04003260 RID: 12896
		public Transform exoticsPanel;

		// Token: 0x04003261 RID: 12897
		private bool resourcesDataDirty;

		// Token: 0x04003262 RID: 12898
		[Header("Button Tips")]
		public TooltipTrigger earthButtonTooltip;

		// Token: 0x04003263 RID: 12899
		public TooltipTrigger spaceButtonTooltip;

		// Token: 0x04003264 RID: 12900
		public TooltipTrigger councilButtonTooltip;

		// Token: 0x04003265 RID: 12901
		public TooltipTrigger nationsButtonTooltip;

		// Token: 0x04003266 RID: 12902
		public TooltipTrigger habsButtonTooltip;

		// Token: 0x04003267 RID: 12903
		public TooltipTrigger fleetsButtonTooltip;

		// Token: 0x04003268 RID: 12904
		public TooltipTrigger researchButtonTooltip;

		// Token: 0x04003269 RID: 12905
		public TooltipTrigger intelButtonTooltip;

		// Token: 0x0400326A RID: 12906
		public TooltipTrigger eventSummaryButtonTooltipExpand;

		// Token: 0x0400326B RID: 12907
		public TooltipTrigger eventSummaryButtonTooltipMinimize;

		// Token: 0x0400326C RID: 12908
		public Image[] techWinnerLights;

		// Token: 0x0400326D RID: 12909
		public Image[] techWinnerIndicators;

		// Token: 0x0400326E RID: 12910
		[Header("Speed Controls")]
		public TMP_Text timeText;

		// Token: 0x0400326F RID: 12911
		public TMP_Text dateText;

		// Token: 0x04003270 RID: 12912
		public TMP_Text speedText;

		// Token: 0x04003271 RID: 12913
		public TMP_Text speedPausedText;

		// Token: 0x04003272 RID: 12914
		public ListManagerBase TimePipsList;

		// Token: 0x04003273 RID: 12915
		public TooltipTrigger speedTooltipTrigger;

		// Token: 0x04003274 RID: 12916
		public Image pauseBlockedImage;

		// Token: 0x04003275 RID: 12917
		private DateTime displayedSimTime;

		// Token: 0x04003276 RID: 12918
		[Header("Finder")]
		public Canvas finderCanvas;

		// Token: 0x04003277 RID: 12919
		public Canvas finderRootCanvas;

		// Token: 0x04003278 RID: 12920
		public RectTransform finderTransform;

		// Token: 0x04003279 RID: 12921
		public RectTransform finderViewport;

		// Token: 0x0400327A RID: 12922
		public RectTransform finderContent;

		// Token: 0x0400327B RID: 12923
		public FinderListAdapter finderListAdapter;

		// Token: 0x0400327C RID: 12924
		public List<FinderListItemModel> finderListModels = new List<FinderListItemModel>();

		// Token: 0x0400327D RID: 12925
		public TMP_Dropdown mapModeDropdown;

		// Token: 0x0400327E RID: 12926
		public TMP_Text mapColorDescText;

		// Token: 0x0400327F RID: 12927
		public TMP_Text finderTitleText;

		// Token: 0x04003280 RID: 12928
		public GameObject mapColorControlPanel;

		// Token: 0x04003281 RID: 12929
		public GameObject MapModeFlagObject;

		// Token: 0x04003282 RID: 12930
		public GameObject MapModeEarthObject;

		// Token: 0x04003283 RID: 12931
		public Image MapModeFlagSprite;

		// Token: 0x04003284 RID: 12932
		public Button finderMinimizeButton;

		// Token: 0x04003285 RID: 12933
		public Button finderCouncilorsButton;

		// Token: 0x04003286 RID: 12934
		public Button finderArmiesButton;

		// Token: 0x04003287 RID: 12935
		public Button finderHabsButton;

		// Token: 0x04003288 RID: 12936
		public Button finderFleetsButton;

		// Token: 0x04003289 RID: 12937
		public Sprite finderFilterOffSprite;

		// Token: 0x0400328A RID: 12938
		public Sprite finderFilterOnSprite;

		// Token: 0x0400328B RID: 12939
		private int finderSelectedIndex;

		// Token: 0x0400328C RID: 12940
		private bool showFinderCouncilors = true;

		// Token: 0x0400328D RID: 12941
		private bool showFinderArmies = true;

		// Token: 0x0400328E RID: 12942
		private bool showFinderHabs = true;

		// Token: 0x0400328F RID: 12943
		private bool showFinderFleets = true;

		// Token: 0x04003290 RID: 12944
		private bool finderEditModeEnabled;

		// Token: 0x04003291 RID: 12945
		private bool storedFinderStatus;

		// Token: 0x04003292 RID: 12946
		private bool finderDataDirty;

		// Token: 0x04003293 RID: 12947
		private bool finderToggleExclusive;

		// Token: 0x04003294 RID: 12948
		private bool finderInit;

		// Token: 0x04003295 RID: 12949
		[Header("Targeting Panel")]
		public Canvas targetingPanel;

		// Token: 0x04003296 RID: 12950
		public RectTransform targetingPanelTransform;

		// Token: 0x04003297 RID: 12951
		public TMP_Text targetingHeaderString;

		// Token: 0x04003298 RID: 12952
		public TMP_Text targetingHeaderTargetString;

		// Token: 0x04003299 RID: 12953
		public ListManagerBase targetingList;

		// Token: 0x0400329A RID: 12954
		private TIGameState originalTarget;

		// Token: 0x0400329B RID: 12955
		private TIGameState currentTarget;

		// Token: 0x0400329C RID: 12956
		private List<TIGameState> targetListForLocation;

		// Token: 0x0400329D RID: 12957
		private bool showAssignedCouncilorsInTargetingPanel;

		// Token: 0x0400329E RID: 12958
		public TMP_Text showAssignedCouncilorsText;

		// Token: 0x0400329F RID: 12959
		[Header("Councilor Chat")]
		private TINotificationQueueState notificationQueue;

		// Token: 0x040032A0 RID: 12960
		public Animator councilorChatAnimator;

		// Token: 0x040032A1 RID: 12961
		public Image councilorChatImage;

		// Token: 0x040032A2 RID: 12962
		public TMP_Text councilorChatText;

		// Token: 0x040032A3 RID: 12963
		[Header("Resource Sale Panel")]
		public GameObject resourceSalePanel;

		// Token: 0x040032A4 RID: 12964
		public TMP_Text resourceSaleHeader;

		// Token: 0x040032A5 RID: 12965
		public TMP_Text confirmSaleButtonText;

		// Token: 0x040032A6 RID: 12966
		public TMP_Text resetButtonText;

		// Token: 0x040032A7 RID: 12967
		public TMP_Text cancelButtonText;

		// Token: 0x040032A8 RID: 12968
		public TMP_Text totalSaleText;

		// Token: 0x040032A9 RID: 12969
		public TMP_Text totalSaleValueText;

		// Token: 0x040032AA RID: 12970
		private Dictionary<FactionResource, int> proposedResourceSales;

		// Token: 0x040032AB RID: 12971
		public ListManagerBase resourceSalesList;

		// Token: 0x040032AC RID: 12972
		public Button confirmSaleButton;

		// Token: 0x040032AD RID: 12973
		[Header("Alien Threat Panel")]
		public UITutorialController alienThreatUITutorialController;

		// Token: 0x040032AE RID: 12974
		public GameObject alienThreatPanel;

		// Token: 0x040032AF RID: 12975
		public Image alienAlertAlienIcon;

		// Token: 0x040032B0 RID: 12976
		public Image[] alienAlertLights;

		// Token: 0x040032B1 RID: 12977
		public TooltipTrigger alienAlertTip;

		// Token: 0x040032B2 RID: 12978
		[Header("Global Search Panel")]
		public GameObject searchObject;

		// Token: 0x040032B3 RID: 12979
		public TMP_Text globalSearchTextTitle;

		// Token: 0x040032B4 RID: 12980
		public GlobalSearchListAdapter globalSearchListAdapter;

		// Token: 0x040032B5 RID: 12981
		public List<GlobalSearchListItemModel> globalSearchListItemModels = new List<GlobalSearchListItemModel>();

		// Token: 0x040032B6 RID: 12982
		public TMP_InputField searchInputField;

		// Token: 0x040032B7 RID: 12983
		[Header("Other")]
		public GameObject PauseButton;

		// Token: 0x040032B8 RID: 12984
		public GameObject PlayButton;

		// Token: 0x040032B9 RID: 12985
		public Button playButtonComponent;

		// Token: 0x040032BA RID: 12986
		public GameObject missionPhaseReportButtonExpand;

		// Token: 0x040032BB RID: 12987
		public GameObject missionPhaseReportButtonMinimize;

		// Token: 0x040032BC RID: 12988
		public GameObject milestonePanel;

		// Token: 0x040032BD RID: 12989
		public Image milestoneIcon;

		// Token: 0x040032BE RID: 12990
		public TMP_Text milestoneText;

		// Token: 0x040032BF RID: 12991
		public TooltipTrigger milestoneTooltip;

		// Token: 0x040032C0 RID: 12992
		public List<GameObject> openTutorialObject = new List<GameObject>();

		// Token: 0x040032C1 RID: 12993
		public List<TMP_Text> tutorialDescriptorText = new List<TMP_Text>();

		// Token: 0x040032C2 RID: 12994
		public GameObject SystemClockObject;

		// Token: 0x040032C3 RID: 12995
		public TMP_Text SystemClockText;

		// Token: 0x040032C4 RID: 12996
		private NotificationScreenController notifications;

		// Token: 0x040032C5 RID: 12997
		public List<GeneralControlsController.HeldTutorialItem> heldTutorialItem = new List<GeneralControlsController.HeldTutorialItem>();

		// Token: 0x040032C6 RID: 12998
		public bool isHoldingTutorial;

		// Token: 0x040032C7 RID: 12999
		[HideInInspector]
		public static MapColorationStyle mapColorationStyle;

		// Token: 0x040032CD RID: 13005
		private bool infoScreenOpen;

		// Token: 0x040032CE RID: 13006
		private CameraManager cameraManager;

		// Token: 0x040032D0 RID: 13008
		private static readonly GeneralControlsController.FinderItemComparer comparer = new GeneralControlsController.FinderItemComparer();

		// Token: 0x040032D4 RID: 13012
		private bool suppressCycleMapModeAudio;

		// Token: 0x040032D5 RID: 13013
		private readonly WaitForSeconds fewSecs = new WaitForSeconds(3f);

		// Token: 0x040032D6 RID: 13014
		private const int finderPanelBaseSize = 6;

		// Token: 0x040032D7 RID: 13015
		private const int finderItemHeight = 29;

		// Token: 0x040032D8 RID: 13016
		private const int targetingPanelBaseSize = 80;

		// Token: 0x040032D9 RID: 13017
		private const int targetingPanelItemHeight = 50;

		// Token: 0x040032DA RID: 13018
		private const int finderMaxHeightUltraWideReduction = 90;

		// Token: 0x040032DB RID: 13019
		private const float finderMinMaxHeightToShutOff = 200f;

		// Token: 0x040032DC RID: 13020
		private const float finderMaxMaxHeight = 845f;

		// Token: 0x040032DD RID: 13021
		public float finderMaxHeight;

		// Token: 0x040032DE RID: 13022
		private int frameCheckedStoredFinderStatus;

		// Token: 0x040032DF RID: 13023
		[Header("Alarm Panel")]
		public GameObject alarmPanel;

		// Token: 0x040032E0 RID: 13024
		public TMP_Text alarmPanelHeader;

		// Token: 0x040032E1 RID: 13025
		public TMP_Dropdown alarmMinuteDropdown;

		// Token: 0x040032E2 RID: 13026
		public TMP_Dropdown alarmHourDropdown;

		// Token: 0x040032E3 RID: 13027
		public TMP_Dropdown alarmDayDropdown;

		// Token: 0x040032E4 RID: 13028
		public TMP_Dropdown alarmMonthDropdown;

		// Token: 0x040032E5 RID: 13029
		public TMP_Dropdown alarmYearDropdown;

		// Token: 0x040032E6 RID: 13030
		public TMP_Text resetProposedAlarmTimeButtonText;

		// Token: 0x040032E7 RID: 13031
		public TMP_Text confirmAlarmAndCloseButtonText;

		// Token: 0x040032E8 RID: 13032
		public TMP_Text confirmAlarmAndDoAnotherButtonText;

		// Token: 0x040032E9 RID: 13033
		public TMP_Text cancelAlarmButtonText;

		// Token: 0x040032EA RID: 13034
		public Button confirmCloseButton;

		// Token: 0x040032EB RID: 13035
		public Button confirmContinueButton;

		// Token: 0x040032EC RID: 13036
		public Button decreaseMinuteButton;

		// Token: 0x040032ED RID: 13037
		public Button increaseMinuteButton;

		// Token: 0x040032EE RID: 13038
		public Button decreaseHourButton;

		// Token: 0x040032EF RID: 13039
		public Button increaseHourButton;

		// Token: 0x040032F0 RID: 13040
		public Button decreaseDayButton;

		// Token: 0x040032F1 RID: 13041
		public Button increaseDayButton;

		// Token: 0x040032F2 RID: 13042
		public Button decreaseMonthButton;

		// Token: 0x040032F3 RID: 13043
		public Button increaseMonthButton;

		// Token: 0x040032F4 RID: 13044
		public Button decreaseYearButton;

		// Token: 0x040032F5 RID: 13045
		public Button increaseYearButton;

		// Token: 0x040032F6 RID: 13046
		public Button openCalendarButton;

		// Token: 0x040032F7 RID: 13047
		public TMP_Text openCalendarButtonText;

		// Token: 0x040032F8 RID: 13048
		private TIDateTime proposedAlarmTime;

		// Token: 0x040032F9 RID: 13049
		public TMP_InputField proposedAlarmText;

		// Token: 0x040032FA RID: 13050
		public TMP_Text placeholderText;

		// Token: 0x02001094 RID: 4244
		public class HeldTutorialItem
		{
			// Token: 0x040063C5 RID: 25541
			public UITutorialController heldTutorialController;

			// Token: 0x040063C6 RID: 25542
			public CampaignMilestone heldTutorialMilestone;

			// Token: 0x040063C7 RID: 25543
			public bool heldTutorialOverrideMilestone;

			// Token: 0x040063C8 RID: 25544
			public bool heldTutorialNextFrame;
		}

		// Token: 0x02001095 RID: 4245
		private class FinderItemComparer : IComparer<TIGameState>
		{
			// Token: 0x060083FD RID: 33789 RVA: 0x0032E486 File Offset: 0x0032C686
			public int Compare(TIGameState a, TIGameState b)
			{
				return GeneralControlsController.FinderItemComparer.Weight(a) - GeneralControlsController.FinderItemComparer.Weight(b);
			}

			// Token: 0x060083FE RID: 33790 RVA: 0x0032E498 File Offset: 0x0032C698
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static int Weight(TIGameState state)
			{
				if (state.isCouncilorState)
				{
					return state.ID + (1 + ((state.ref_faction == GameControl.control.activePlayer) ? 0 : 1) << 24);
				}
				if (state.isArmyState)
				{
					if (state.ref_army.AlienMegafaunaArmy)
					{
						return state.ID + 83886080;
					}
					if (state.ref_army.AlienRegularArmy)
					{
						return state.ID + 67108864;
					}
					return state.ID + (29 - (int)state.ref_army.techLevel << 24);
				}
				else
				{
					if (state.isSpaceFleetState)
					{
						return state.ID + 503316480;
					}
					if (state.isHabState)
					{
						return state.ID + (31 + (state.ref_hab.IsStation ? 3 : 6) - state.ref_hab.tier << 24);
					}
					return state.ID + 536870912;
				}
			}
		}
	}
}
