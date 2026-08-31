using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using AssetBundles;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using ModestTree;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using PavonisInteractive.TerraInvicta.Systems.UI;
using PavonisInteractive.TerraInvicta.UI;
using PavonisInteractive.TerraInvicta.UI.Canvas_Prefabs.FleetsScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200084F RID: 2127
	public class FleetsScreenController : CanvasControllerBase, IInfoScreen, ICanvas
	{
		// Token: 0x17000EAE RID: 3758
		// (get) Token: 0x06004D46 RID: 19782 RVA: 0x0020E1E9 File Offset: 0x0020C3E9
		private bool humanOnlyDesignerTest
		{
			get
			{
				return TemplateManager.global.debug_showAllShipParts;
			}
		}

		// Token: 0x17000EAF RID: 3759
		// (get) Token: 0x06004D47 RID: 19783 RVA: 0x0020E1F5 File Offset: 0x0020C3F5
		private bool fullDesignerTest
		{
			get
			{
				return TemplateManager.global.debug_showAllShipPartsIncludingAlien;
			}
		}

		// Token: 0x06004D48 RID: 19784 RVA: 0x0020E204 File Offset: 0x0020C404
		public override void Initialize()
		{
			base.Initialize();
			GameControl.eventManager.AddListener<FleetDetailRequested>(new EventManager.EventDelegate<FleetDetailRequested>(this.OnFleetDetailRequested), null, null, true, false);
			GameControl.eventManager.AddListener<ShipDetailRequested>(new EventManager.EventDelegate<ShipDetailRequested>(this.OnShipDetailRequested), null, null, true, false);
			GameControl.eventManager.AddListener<ShipyardUIRequested>(new EventManager.EventDelegate<ShipyardUIRequested>(this.OnShipyardRequested), null, base.activePlayer, true, false);
			this.fleetListCanvas.gameObject.SetActive(true);
			this.ShipDesignerCanvas.gameObject.SetActive(true);
			this.individualShipCanvas.gameObject.SetActive(true);
			this.shipClassListCanvas.gameObject.SetActive(true);
			this.constructionManagerCanvas.gameObject.SetActive(true);
			this.fleetListCanvas.enabled = true;
			this.restoreCanvas = this.fleetListCanvas;
			this.InitializeShipDesigner();
			this.fleetsScreenTitle.SetText(Loc.T("UI.Fleets.Title"));
			this.fleetsListTabText.SetText(Loc.T("UI.Fleets.FleetsListTab"));
			this.classListTabText.SetText(Loc.T("UI.Fleets.ShipyardButtonText"));
			this.shipDetailTabText.SetText(Loc.T("UI.Fleets.ShipDetailTab"));
			this.shipDesignerTabText.SetText(Loc.T("UI.Fleets.DesignerHeader"));
			this.constructionTabText.SetText(Loc.T("UI.Fleets.ConstructionManagerButtonText"));
			this.indiv_noseWeaponsHeader.SetText(Loc.T("UI.Fleets.NoseWeaponsHeader"));
			this.indiv_hullWeaponsHeader.SetText(Loc.T("UI.Fleets.HullWeaponsHeader"));
			this.indiv_utilityWeaponsHeader.SetText(Loc.T("UI.Fleets.UtilityModuleHeader"));
			this.fleetListToClassListButtonText.SetText(Loc.T("UI.Fleets.ShipyardButtonText"));
			this.fleetListToShipDesignerButtonText.SetText(Loc.T("UI.Fleets.ShipDesignerButtonText"));
			this.fleetListConstructionManagerButtonText.SetText(Loc.T("UI.Fleets.ConstructionManagerButtonText"));
			this.fleetListSortNameText.SetText(Loc.T("UI.Nations.Name"));
			this.fleetListSortAlertLevelText.SetText(Loc.T("UI.Fleets.AlertLevel"));
			this.fleetListSortArrivalTimeText.SetText(Loc.T("UI.Fleets.ArrivalTime"));
			this.fleetListSortOperationsText.SetText(Loc.T("UI.Objectives.NaturalSpaceObjectOps.1.NameShort"));
			this.construction_ShipListButton.SetText(Loc.T("UI.Fleets.ShipyardButtonText"));
			this.construction_ShipDesignerButton.SetText(Loc.T("UI.Fleets.ShipDesignerButtonText"));
			this.construction_FleetListButton.SetText(Loc.T("UI.Fleets.FleetListButtonText"));
			this.designerCoreDataHeader.SetText(Loc.T("UI.Fleets.DesignerCoreDataHeader"));
			this.designerShipDataHeader.SetText(Loc.T("UI.Fleets.DesignerShipDataHeader"));
			this.designerResetDesignButtonText.SetText(Loc.T("UI.Fleets.DesignerResetDesign"));
			this.designerSaveDesignButtonText.SetText(Loc.T("UI.Fleets.DesignerSaveDesign"));
			this.designerAutoDesignButtonText.SetText(Loc.T("UI.Fleets.Autodesign"));
			this.designerConfirmationHeaderText.SetText(Loc.T("UI.Fleets.Confirmation"));
			this.designerSaveTooltipText.SetText("BodyText", Loc.T("UI.Fleets.SaveRequirement"));
			this.refitTooltipText.SetText("BodyText", Loc.T("UI.Codex.codex_shipRefits0"));
			this.refitRefuelCostTooltip.SetText("BodyText", Loc.T("UI.Fleets.RefitRefuelWarning"));
			this.installModuleButtonText.SetText(Loc.T("UI.Fleets.InstallModule"));
			this.installedDeleteModuleButtonText.SetText(Loc.T("UI.Fleets.DeleteModule"));
			this.refitTabText.SetText(Loc.T("UI.Fleets.Refit"));
			this.constructTabText.SetText(Loc.T("UI.Fleets.Construct"));
			this.noShipyardsText.SetText(Loc.T("UI.Fleets.ConstructionManagerNoShipyards"));
			this.noShipyardsButtonText.SetText(Loc.T("UI.Fleets.GotoHabs"));
			this.noShipClassSelectedText.SetText(Loc.T("UI.Fleets.ConstructionManagerNoShipSelected"));
			this.noShipDesignsText.SetText(Loc.T("UI.Fleets.ConstructionManagerNoDesigns"));
			this.addToFastestQueueButtonText.SetText(Loc.T("UI.Fleets.AddToFastestQueue"));
			this.classListHeader.SetText(Loc.T("UI.Fleets.ClassListHeader", new object[] { base.activePlayer.adjective }));
			this.classListFactionGradient.sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(base.activePlayer.template.gradientPath);
			this.classListFactionIcon.sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(base.activePlayer.template.councilIcon256_ui);
			this.classListHideObsoleteText.SetText(Loc.T("UI.Fleets.HideObsolete"));
			this.valuesHeader.SetText(Loc.T("UI.Fleets.DerivedValuesHeader"));
			this.systemsHeader.SetText(Loc.T("UI.Fleets.PrimarySystemsHeader"));
			this.missionSystemsHeader.SetText(Loc.T("UI.Fleets.MissionSystemsHeader"));
			this.showAllShipsToggleText.SetText(Loc.T("UI.Fleets.ShowAllShips"));
			this.showOnlyShipsInSelectedFleetToggleText.SetText(Loc.T("UI.Fleets.OnlyInSameFleet"));
			this.damageHeader.SetText(Loc.T("UI.Fleets.DamageControl"));
			this.fleetClassListSortNameText.SetText(Loc.T("UI.Nations.Name"));
			this.fleetClassListSortHullText.SetText(Loc.T("UI.Fleets.Hull"));
			this.fleetClassListSortRoleText.SetText(Loc.T("UI.Space.Councilors.Header.Location"));
			this.fleetClassListSortMassText.SetText(Loc.T("UI.Fleets.ModuleTable.Mass"));
			this.fleetClassListSortBuildCostText.SetText(Loc.T("UI.Fleets.BuildCostHeader"));
			this.invertFleetClassSort = false;
			this.noseModulesTabText.SetText(Loc.T("UI.Fleets.NoseWeaponsTab"));
			this.hullModulesTabText.SetText(Loc.T("UI.Fleets.HullWeaponsTab"));
			this.utilitiesTabPane.tabText.SetText(Loc.T("UI.Fleets.UtilitiesTab"));
			this.radiatorsTabPane.tabText.SetText(Loc.T("UI.Fleets.RadiatorsTab"));
			this.batteriesTabPane.tabText.SetText(Loc.T("UI.Fleets.BatteriesTab"));
			this.powerPlantsTabPane.tabText.SetText(Loc.T("UI.Fleets.PowerPlantTab"));
			this.drivesTabPane.tabText.SetText(Loc.T("UI.Fleets.DriveTab"));
			this.armorTabPane.tabText.SetText(Loc.T("UI.Fleets.ArmorTab"));
			this.weaponsTabPane.tabText.SetText(Loc.T("UI.Fleets.AllWeaponsTab"));
			this.gunsTabPane.tabText.SetText(Loc.T("UI.Fleets.GunsTab"));
			this.missilesTabPane.tabText.SetText(Loc.T("UI.Fleets.MissilesTab"));
			this.magneticWeaponsTabPane.tabText.SetText(Loc.T("UI.Fleets.MagneticWeaponsTab"));
			this.plasmaWeaponsTabPane.tabText.SetText(Loc.T("UI.Fleets.PlasmaWeaponsTab"));
			this.lasersTabPane.tabText.SetText(Loc.T("UI.Fleets.LasersTab"));
			this.particleWeaponsTabPane.tabText.SetText(Loc.T("UI.Fleets.ParticleWeaponsTab"));
			this.noseWeaponsTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.hullWeaponsTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.utilityModulesTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.radiatorsTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.batteriesTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.powerPlantsTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.drivesTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.armorTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.allWeaponsTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.gunsTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.missilesTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.magneticWeaponsTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.plasmaWeaponsTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.lasersTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.particleWeaponsTooltip.SetText("BodyText", Loc.T("UI.Fleets.RightToToggleLists"));
			this.selectedShipClassSelectedLabel.SetText(Loc.T("UI.Fleets.Selected"));
			this.selectedShipClassArmorTab.SetText(Loc.T("UI.Fleets.ArmorNLT"));
			this.selectedModuleHeaderText.SetText(Loc.T("UI.Fleets.Designer.SelectedModule"));
			this.installedModuleHeaderText.SetText(Loc.T("UI.Fleets.Designer.InstalledModule"));
			this.selectedModuleObsoleteHeaderText.SetText(Loc.T("UI.Fleets.Obsolete"));
			this.installedModuleObsoleteHeaderText.SetText(Loc.T("UI.Fleets.Obsolete"));
			this.designerWetMassTabText.SetText(Loc.T("UI.Fleets.WetMassTab"));
			this.designerCrewTabText.SetText(Loc.T("UI.Fleets.CrewTab"));
			this.designerCruiseAccelerationTabText.SetText(Loc.T("UI.Fleets.CruiseAccelerationTab"));
			this.designerCombatAccelerationTabText.SetText(Loc.T("UI.Fleets.CombatAccelerationTab"));
			this.designerCruiseDeltaVTabText.SetText(Loc.T("UI.Fleets.CruiseDeltaVTab"));
			this.designerTurnRateTabText.SetText(Loc.T("UI.Fleets.TurnRateTab"));
			this.designerHeatSinkCapacityTabText.SetText(Loc.T("UI.Fleets.HeatSinkCapacityTab"));
			this.designerBatteryCapacityTabText.SetText(Loc.T("UI.Fleets.BatteryCapacityTab"));
			this.designerConstructionCostTabText.SetText(Loc.T("UI.Fleets.ConstructionCostTab"));
			this.designerConstructionTimeTabText.SetText(Loc.T("UI.Fleets.ConstructionTimeTab"));
			this.designerMaintenanceCostTabText.SetText(Loc.T("UI.Fleets.MaintenanceCostTab"));
			this.dockedShipsText.SetText(Loc.T("UI.Fleets.DockedShips"));
			this.refitClassesText.SetText(Loc.T("UI.Fleets.ValidRefitClasses"));
			this.construction_AddToFastestQueueButton.gameObject.SetActive(false);
			this.validRefitNotificationObject.SetActive(false);
			this.classListHideObsoleteToggle.isOn = !TIGlobalValuesState.GlobalValues.fleetScreenClassShowObsolete;
			this.first = true;
			this.PopulatePermanentDropdowns();
			this.masterDamageGridControllers = new Dictionary<Vector2Int, SpaceCombatDamageGridItemController>();
			int num = 0;
			int num2 = 0;
			foreach (object obj in this.masterDamageGridGroup.transform)
			{
				SpaceCombatDamageGridItemController component = ((Transform)obj).GetComponent<SpaceCombatDamageGridItemController>();
				Vector2Int vector2Int = new Vector2Int(num, num2);
				component.PreInitialize(vector2Int);
				component.Clear();
				this.masterDamageGridControllers.Add(vector2Int, component);
				num2++;
				if (num2 == 8)
				{
					num++;
					num2 = 0;
				}
			}
			this.UpdateLeftDetailPanel(0);
			this.UpdateRightDetailPanel(0, 0);
		}

		// Token: 0x06004D49 RID: 19785 RVA: 0x0020ECE4 File Offset: 0x0020CEE4
		public override void Show()
		{
			base.Show();
			this.shipDetailTabButtonObject.SetActive(base.activePlayer.knownShips.Count > 0);
			this.shipDesignerTabButtonObject.SetActive(FleetsScreenController.CanDesignShips(base.activePlayer, false));
			this.multiSelectedRefitShips.Clear();
			if (this.first)
			{
				this.fleets_filterForFaction = GameControl.control.activePlayer;
				this.factionsDropdown.captionText.SetText(this.fleets_filterForFaction.displayNameCapitalizedWithColor);
				this.first = false;
				this.ShowConstructTab();
			}
			if (!FleetsScreenController.gotoDesigner && !FleetsScreenController.gotoConstructionManager)
			{
				if (this.tabbedPaneManager.activeTab != this.fleetsListTab)
				{
					this.tabbedPaneManager.Toggle(this.fleetsListTab);
				}
				else
				{
					this.UpdateFleetsList();
				}
			}
			GameControl.eventManager.AddListener<ShipConstructionUpdated>(new EventManager.EventDelegate<ShipConstructionUpdated>(this.OnShipConstructionUpdated), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.OnFleetCoreStatusChanged), null, null, true, false);
			GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnShipsRemovedFromFleet), null, null, true, false);
			GameControl.eventManager.AddListener<FleetOperationWithDurationComplete>(new EventManager.EventDelegate<FleetOperationWithDurationComplete>(this.OnFleetOpComplete), null, null, true, false);
			GameControl.eventManager.AddListener<ShipPartUnlocked>(new EventManager.EventDelegate<ShipPartUnlocked>(this.OnShipPartUnlocked), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnHabModuleDestroyed), null, null, true, false);
			if (FleetsScreenController.gotoDesigner)
			{
				this.OnDesignShipButtonFromFleetListClicked();
				FleetsScreenController.gotoDesigner = false;
			}
			if (FleetsScreenController.gotoConstructionManager)
			{
				if (this.tabbedPaneManager.activeTab != this.constructionTab)
				{
					this.OpenConstructionManager();
				}
				else
				{
					this.UpdateConstructionManager(null);
				}
				FleetsScreenController.gotoConstructionManager = false;
			}
		}

		// Token: 0x06004D4A RID: 19786 RVA: 0x0020EEA4 File Offset: 0x0020D0A4
		public override void Hide()
		{
			base.Hide();
			this.shipListInitialized = false;
			if (this.damageControlPanel != null)
			{
				this.damageControlPanel.SetActive(false);
			}
			GameControl.eventManager.RemoveListener<ShipConstructionUpdated>(new EventManager.EventDelegate<ShipConstructionUpdated>(this.OnShipConstructionUpdated), null);
			GameControl.eventManager.RemoveListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.OnFleetCoreStatusChanged), null);
			GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnShipsRemovedFromFleet), null);
			GameControl.eventManager.RemoveListener<FleetOperationWithDurationComplete>(new EventManager.EventDelegate<FleetOperationWithDurationComplete>(this.OnFleetOpComplete), null);
			GameControl.eventManager.RemoveListener<ShipPartUnlocked>(new EventManager.EventDelegate<ShipPartUnlocked>(this.OnShipPartUnlocked), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnHabModuleDestroyed), null);
			global::UnityEngine.Object.Destroy(this.individualShipCameraObject, 0f);
			this.OnExitShipDesigner();
			this.HideTutorials();
		}

		// Token: 0x06004D4B RID: 19787 RVA: 0x0020EF80 File Offset: 0x0020D180
		public override void Refresh()
		{
			if (this.fleetListCanvas.enabled && TIFrameCounter.FrameCount % 293 == 0)
			{
				using (IEnumerator<object> enumerator = this.fleetsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__94.<>p__0 == null)
						{
							FleetsScreenController.<>o__94.<>p__0 = CallSite<Func<CallSite, object, FleetsSceenFleetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FleetsSceenFleetListItemController), typeof(FleetsScreenController)));
						}
						FleetsScreenController.<>o__94.<>p__0.Target(FleetsScreenController.<>o__94.<>p__0, enumerator.Current).RefreshTransitData();
					}
				}
			}
		}

		// Token: 0x06004D4C RID: 19788 RVA: 0x0020F024 File Offset: 0x0020D224
		public override bool Visible()
		{
			return base.Visible() && base.canvasManager.IsShowingInfoScreen<FleetsScreenController>();
		}

		// Token: 0x06004D4D RID: 19789 RVA: 0x0020F03B File Offset: 0x0020D23B
		private void OnFleetCoreStatusChanged(FleetCoreStatusChange e)
		{
			if (this.Visible())
			{
				this.UpdateFleetsList();
			}
		}

		// Token: 0x06004D4E RID: 19790 RVA: 0x0020F04B File Offset: 0x0020D24B
		private void OnShipsRemovedFromFleet(ShipsRemovedFromFleet e)
		{
			if (this.Visible())
			{
				this.fleetListDirty = true;
				this.ValidateFleetScreenGameStateModels();
			}
		}

		// Token: 0x06004D4F RID: 19791 RVA: 0x0020F062 File Offset: 0x0020D262
		private void OnFleetOpComplete(FleetOperationWithDurationComplete e)
		{
			if (this.Visible())
			{
				this.UpdateFleetsList();
			}
		}

		// Token: 0x06004D50 RID: 19792 RVA: 0x0020F074 File Offset: 0x0020D274
		private void OnFleetDetailRequested(FleetDetailRequested e)
		{
			base.canvasManager.ShowInfoScreen<FleetsScreenController>();
			this.fleetListCanvas.enabled = true;
			this.shipClassListCanvas.enabled = false;
			this.ShipDesignerCanvas.enabled = false;
			if (TIGameState.Valid(e.fleet) && this.fleets_filterForFaction != e.fleet.faction)
			{
				this.factionsDropdown.value = this.factionDropdownLookup.Keys.First<int>((int x) => this.factionDropdownLookup[x] == e.fleet.faction);
				this.factionsDropdown.RefreshShownValue();
			}
		}

		// Token: 0x06004D51 RID: 19793 RVA: 0x0020F126 File Offset: 0x0020D326
		private void OnShipDetailRequested(ShipDetailRequested e)
		{
			base.canvasManager.ShowInfoScreen<FleetsScreenController>();
			this.ShowIndividualDataScreen(e.ship, true);
		}

		// Token: 0x06004D52 RID: 19794 RVA: 0x0020F141 File Offset: 0x0020D341
		private void OnShipConstructionUpdated(ShipConstructionUpdated e)
		{
			this.RefreshConstructionManager();
		}

		// Token: 0x06004D53 RID: 19795 RVA: 0x0020F149 File Offset: 0x0020D349
		private void OnShipyardRequested(ShipyardUIRequested e)
		{
			base.canvasManager.ShowInfoScreen<FleetsScreenController>();
			this.tabbedPaneManager.Toggle(this.constructionTab);
		}

		// Token: 0x06004D54 RID: 19796 RVA: 0x0020F168 File Offset: 0x0020D368
		private void OnHabModuleDestroyed(HabModuleDestroyed e)
		{
			if (this.constructionManagerCanvas.enabled && e.habModule.ref_faction == base.activePlayer)
			{
				this.RefreshConstructionManager();
			}
		}

		// Token: 0x06004D55 RID: 19797 RVA: 0x0020F198 File Offset: 0x0020D398
		public void CloseInfoScreen(bool toggle = false)
		{
			if (this.ShipDesignerCanvas != null)
			{
				this.ShipDesignerCanvas.enabled = false;
			}
			if (this.individualShipCanvas != null)
			{
				this.individualShipCanvas.enabled = false;
			}
			if (this.shipClassListCanvas != null)
			{
				this.shipClassListCanvas.enabled = false;
			}
			if (this.fleetListCanvas != null)
			{
				this.fleetListCanvas.enabled = false;
			}
			if (this.constructionManagerCanvas != null)
			{
				this.constructionManagerCanvas.enabled = false;
			}
			base.canvasManager.HideInfoScreen<FleetsScreenController>(toggle);
		}

		// Token: 0x06004D56 RID: 19798 RVA: 0x0020F233 File Offset: 0x0020D433
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.primaryPanelTransform.anchoredPosition = new Vector2(0f, (float)((base.VerticalScaleValueLimit() >= 940f) ? (-100) : (-85)));
		}

		// Token: 0x06004D57 RID: 19799 RVA: 0x0020F264 File Offset: 0x0020D464
		public void OnClassListButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.shipClassListCanvas.enabled = true;
			this.fleetListCanvas.enabled = false;
			this.UpdateShipClassListScreen();
		}

		// Token: 0x06004D58 RID: 19800 RVA: 0x0020F290 File Offset: 0x0020D490
		public void OnConstructionManagerButtonClicked()
		{
			this.UpdateConstructionManager(null);
		}

		// Token: 0x06004D59 RID: 19801 RVA: 0x0020F299 File Offset: 0x0020D499
		public void OpenConstructionManager()
		{
			this.tabbedPaneManager.Toggle(this.constructionTab);
		}

		// Token: 0x06004D5A RID: 19802 RVA: 0x0020F2AC File Offset: 0x0020D4AC
		public void OnDesignShipButtonFromFleetListClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			if (this.tabbedPaneManager.activeTab != this.shipDesignerTab)
			{
				this.tabbedPaneManager.Toggle(this.shipDesignerTab);
				return;
			}
			this.shipDesignerTab.Show(false);
		}

		// Token: 0x06004D5B RID: 19803 RVA: 0x0020F2FB File Offset: 0x0020D4FB
		public void OnDesignShipButtonFromConstructionManagerClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.tabbedPaneManager.Toggle(this.shipDesignerTab);
		}

		// Token: 0x06004D5C RID: 19804 RVA: 0x0020F31A File Offset: 0x0020D51A
		public void OnClassListButtonFromConstructionManagerClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.tabbedPaneManager.Toggle(this.classListTab);
		}

		// Token: 0x06004D5D RID: 19805 RVA: 0x0020F339 File Offset: 0x0020D539
		public void OnExitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.HideTutorials();
			this.CloseInfoScreen(false);
		}

		// Token: 0x06004D5E RID: 19806 RVA: 0x0020F354 File Offset: 0x0020D554
		public void OnCloseAndPlaySelected()
		{
			this.OnExitButtonClicked();
			base.gameTime.Play();
		}

		// Token: 0x06004D5F RID: 19807 RVA: 0x0020F367 File Offset: 0x0020D567
		public void ShowFleetListTutorial()
		{
			if (base.activePlayer.fleets.Count > 0)
			{
				this.HideTutorials();
				this.FleetScreenUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_FleetsScreenCanvas_FleetsList, false, true);
			}
		}

		// Token: 0x06004D60 RID: 19808 RVA: 0x0020F394 File Offset: 0x0020D594
		public void ShowClassListTutorial()
		{
			if (base.activePlayer.shipDesigns.Count > 0)
			{
				this.HideTutorials();
				this.ShipClassUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_FleetsScreenCanvas_ClassList, false, true);
			}
		}

		// Token: 0x06004D61 RID: 19809 RVA: 0x0020F3C1 File Offset: 0x0020D5C1
		public void ShowConstructionTutorial()
		{
			if (base.activePlayer.nShipyardQueues.Keys.Count > 0)
			{
				this.HideTutorials();
				this.ShipConstructionUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_FleetsScreenCanvas_ConstructionManager, false, true);
			}
		}

		// Token: 0x06004D62 RID: 19810 RVA: 0x0020F3F3 File Offset: 0x0020D5F3
		public void ShowDesignerTutorial()
		{
			this.HideTutorials();
			this.ShipDesignerUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_FleetsScreenCanvas_ShipDesigner, false, true);
		}

		// Token: 0x06004D63 RID: 19811 RVA: 0x0020F40D File Offset: 0x0020D60D
		public void ShowShipDetailTutorial()
		{
			this.HideTutorials();
			this.ShipDetailUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_FleetsScreenCanvas_ShipDetail, false, true);
		}

		// Token: 0x06004D64 RID: 19812 RVA: 0x0020F427 File Offset: 0x0020D627
		public void HideTutorials()
		{
			this.FleetScreenUITutorialController.HideTutorial();
			this.ShipClassUITutorialController.HideTutorial();
			this.ShipConstructionUITutorialController.HideTutorial();
			this.ShipDesignerUITutorialController.HideTutorial();
			this.ShipDetailUITutorialController.HideTutorial();
		}

		// Token: 0x06004D65 RID: 19813 RVA: 0x0020F460 File Offset: 0x0020D660
		public void Tutorial_ExpandFirstFleet()
		{
			this.Tutorial_ChangeFirstFleetExpandState(false);
		}

		// Token: 0x06004D66 RID: 19814 RVA: 0x0020F469 File Offset: 0x0020D669
		public void Tutorial_UnExpandFirstFleet()
		{
			this.Tutorial_ChangeFirstFleetExpandState(true);
		}

		// Token: 0x06004D67 RID: 19815 RVA: 0x0020F474 File Offset: 0x0020D674
		private void Tutorial_ChangeFirstFleetExpandState(bool expand)
		{
			this.fleetScreenFleetListAdapter.ScrollTo(0, 0f, 0f);
			TIGameState firstFleetState = this.fleetScreenFleetListAdapter.Data.FirstOrDefault<FleetScreenFleetListItemModel>((FleetScreenFleetListItemModel x) => x.FleetScreenFleetListItemData.gameStateFleetOrShip != null && x.FleetScreenFleetListItemData.gameStateFleetOrShip.isSpaceFleetState).FleetScreenFleetListItemData.gameStateFleetOrShip;
			List<FleetsSceenFleetListItemController> list = this.fleetScreenFleetListAdapter.GetComponentsInChildren<FleetsSceenFleetListItemController>().TakeAllButLast<FleetsSceenFleetListItemController>().ToList<FleetsSceenFleetListItemController>();
			if (list != null && list.Count == 0)
			{
				return;
			}
			FleetsSceenFleetListItemController fleetsSceenFleetListItemController = list.FirstOrDefault<FleetsSceenFleetListItemController>((FleetsSceenFleetListItemController x) => !x.isGroupItem && x.fleetLineObject.activeInHierarchy && x.fleet.Equals(firstFleetState));
			if (fleetsSceenFleetListItemController == null || fleetsSceenFleetListItemController.fleet == null)
			{
				return;
			}
			if (this.fleetOpenedStatus.ContainsKey(fleetsSceenFleetListItemController.fleet) && this.fleetOpenedStatus[fleetsSceenFleetListItemController.fleet] == expand)
			{
				this.fleetsTutorialLock = true;
				fleetsSceenFleetListItemController.FleetButtonClicked();
				this.fleetsTutorialLock = false;
			}
		}

		// Token: 0x06004D68 RID: 19816 RVA: 0x0020F564 File Offset: 0x0020D764
		public void OnClickRename()
		{
			if (this.selectedShip == null)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.nameInputField.text = this.selectedShip.GetDisplayName(this.selectedShip.faction);
			this.ShowRenameMyFleetPanel();
		}

		// Token: 0x06004D69 RID: 19817 RVA: 0x0020F5B3 File Offset: 0x0020D7B3
		public void OnClickRevertRename()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.RevertRename();
		}

		// Token: 0x06004D6A RID: 19818 RVA: 0x0020F5C7 File Offset: 0x0020D7C7
		public void RevertRename()
		{
			this.renameMyShipPanel.SetActive(false);
			this.nameInputField.text = "";
		}

		// Token: 0x06004D6B RID: 19819 RVA: 0x0020F5E8 File Offset: 0x0020D7E8
		public void OnClickSaveName()
		{
			this.renameMyShipPanel.SetActive(false);
			this.selectedShip.faction.playerControl.StartAction(new ChangeShipBio(this.selectedShip, this.nameInputField.text));
			this.changesMadeToExistingClass = true;
			this.Refresh();
			this.SetIndividualShipList();
			this.indiv_ShipName.SetText(this.selectedShip.displayName);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		}

		// Token: 0x06004D6C RID: 19820 RVA: 0x0020F661 File Offset: 0x0020D861
		public void ShowRenameMyFleetPanel()
		{
			this.renameMyShipPanel.SetActive(true);
			this.nameInputField.Select();
		}

		// Token: 0x06004D6D RID: 19821 RVA: 0x0020F67A File Offset: 0x0020D87A
		public void OnSelectInputBox()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x06004D6E RID: 19822 RVA: 0x0020F681 File Offset: 0x0020D881
		public void OnDeSelectInputBox()
		{
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x06004D6F RID: 19823 RVA: 0x0020F688 File Offset: 0x0020D888
		public void ToggleFactionFleets(TIFactionState faction)
		{
			if (!this.hiddenFactions.Contains(faction))
			{
				this.hiddenFactions.Add(faction);
			}
			else
			{
				this.hiddenFactions.Remove(faction);
			}
			this.UpdateFleetsList();
		}

		// Token: 0x06004D70 RID: 19824 RVA: 0x0020F6BC File Offset: 0x0020D8BC
		public void OnFactionDropdownChanged()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.fleets_filterForFaction = this.factionDropdownLookup[this.factionsDropdown.value];
			this.fleetList_FilterHumanFactionsOnly = this.factionsDropdown.value == 2;
			this.UpdateFleetsList();
		}

		// Token: 0x06004D71 RID: 19825 RVA: 0x0020F70C File Offset: 0x0020D90C
		public void OnHighLocationDropdownChanged()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			List<int> bitIndices = this.locationDropdown_High.value.GetBitIndices();
			this.fleets_HighFilterForSpaceBody = new List<TISpaceBodyState>();
			foreach (int num in bitIndices)
			{
				this.fleets_HighFilterForSpaceBody.AddUnique(this.highLocationDropdownLookup[num]);
			}
			if (this.fleets_HighFilterForSpaceBody.Count > 0)
			{
				this.locationDropdown_Specific.gameObject.SetActive(true);
				this.PopulateLocalDropdown();
				this.fleets_SpecificFilterForNaturalSpaceObject = null;
				this.UpdateFleetsList();
				this.locationDropdown_Specific.value = 0;
				return;
			}
			this.fleets_SpecificFilterForNaturalSpaceObject = null;
			this.locationDropdown_Specific.gameObject.SetActive(false);
			this.UpdateFleetsList();
		}

		// Token: 0x06004D72 RID: 19826 RVA: 0x0020F7F0 File Offset: 0x0020D9F0
		public void OnSpecificLocationDropdownChanged()
		{
			List<int> bitIndices = this.locationDropdown_Specific.value.GetBitIndices();
			if (bitIndices.Contains(this.locationDropdown_Specific_EntryLimit))
			{
				int num = this.locationDropdown_Specific.value;
				num &= ~(1 << this.locationDropdown_Specific_EntryLimit);
				this.locationDropdown_Specific.SetValueWithoutNotify(num);
				bitIndices.Remove(this.locationDropdown_Specific_EntryLimit);
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.fleets_SpecificFilterForNaturalSpaceObject = new List<TINaturalSpaceObjectState>();
			foreach (int num2 in bitIndices)
			{
				this.fleets_SpecificFilterForNaturalSpaceObject.AddUnique(this.specificLocationDropdownLookup[num2]);
			}
			this.UpdateFleetsList();
		}

		// Token: 0x06004D73 RID: 19827 RVA: 0x0020F8C0 File Offset: 0x0020DAC0
		private void PopulatePermanentDropdowns()
		{
			List<TIFactionState> list = (from x in GameStateManager.AllFactions()
				orderby x == GameControl.control.activePlayer descending, x.IsAlienFaction
				select x).ToList<TIFactionState>();
			this.factionsDropdown.captionText.SetText(Loc.T("UI.Habs.SelectFaction"));
			this.factionsDropdown.ClearOptions();
			this.factionDropdownLookup = new Dictionary<int, TIFactionState>();
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
			{
				text = base.activePlayer.displayNameCapitalizedWithColor,
				image = base.activePlayer.factionIcon64UI
			};
			this.factionsDropdown.options.Add(optionData);
			this.factionDropdownLookup.Add(0, base.activePlayer);
			TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.AllFactions")
			};
			TMP_Dropdown.OptionData optionData3 = new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.AllHumanFactions")
			};
			this.factionsDropdown.options.Add(optionData2);
			this.factionsDropdown.options.Add(optionData3);
			this.factionDropdownLookup.Add(1, null);
			this.factionDropdownLookup.Add(2, null);
			int num = 3;
			foreach (TIFactionState tifactionState in list)
			{
				if (tifactionState != base.activePlayer)
				{
					TMP_Dropdown.OptionData optionData4 = new TMP_Dropdown.OptionData
					{
						text = tifactionState.displayNameCapitalizedWithColor,
						image = tifactionState.factionIcon64UI
					};
					this.factionsDropdown.options.Add(optionData4);
					this.factionDropdownLookup.Add(num++, tifactionState);
				}
			}
			this.locationDropdown_High.captionText.SetText(Loc.T("UI.Habs.SelectLocation"));
			this.locationDropdown_High.ClearOptions();
			num = 0;
			this.highLocationDropdownLookup = new Dictionary<int, TISpaceBodyState>();
			foreach (string text in TargetSelectionTool.primaryNavigatorBodyTemplateNames)
			{
				TISpaceBodyState tispaceBodyState = GameStateManager.FindByTemplate<TISpaceBodyState>(text, false);
				if (tispaceBodyState != null)
				{
					TMP_Dropdown.OptionData optionData5 = new TMP_Dropdown.OptionData();
					switch (tispaceBodyState.objectType)
					{
					case SpaceObjectType.Star:
						continue;
					default:
						optionData5.text = tispaceBodyState.displayName;
						break;
					case SpaceObjectType.DwarfPlanet:
					case SpaceObjectType.Asteroid:
					case SpaceObjectType.Comet:
						if (GameStateManager.InnerSystemAsteroids(true).Contains(tispaceBodyState))
						{
							optionData5.text = Loc.T("UI.Habs.InnerSystemAsteroids");
						}
						else if (GameStateManager.InnerAsteroidBelt(true).Contains(tispaceBodyState))
						{
							optionData5.text = Loc.T("UI.Habs.InnerBelt");
						}
						else if (GameStateManager.MidAsteroidBelt(true).Contains(tispaceBodyState))
						{
							optionData5.text = Loc.T("UI.Habs.MidBelt");
						}
						else if (GameStateManager.OuterAsteroidBelt(true).Contains(tispaceBodyState))
						{
							optionData5.text = Loc.T("UI.Habs.FarBelt");
						}
						else if (GameStateManager.Centaurs(true).Contains(tispaceBodyState))
						{
							optionData5.text = Loc.T("UI.Habs.Centaurs");
						}
						else if (GameStateManager.KuiperBeltObjects(true).Contains(tispaceBodyState))
						{
							optionData5.text = Loc.T("UI.Habs.KBO");
						}
						else
						{
							optionData5.text = Loc.T("UI.Habs.Other");
						}
						break;
					}
					optionData5.image = tispaceBodyState.icon;
					this.locationDropdown_High.options.Add(optionData5);
					this.highLocationDropdownLookup.Add(num++, tispaceBodyState);
				}
			}
			this.locationDropdown_Specific.gameObject.SetActive(false);
		}

		// Token: 0x06004D74 RID: 19828 RVA: 0x0020FCA0 File Offset: 0x0020DEA0
		private void PopulateLocalDropdown()
		{
			this.locationDropdown_Specific.ClearOptions();
			this.specificLocationDropdownLookup = new Dictionary<int, TINaturalSpaceObjectState>();
			List<TINaturalSpaceObjectState> list = new List<TINaturalSpaceObjectState>();
			foreach (TISpaceBodyState tispaceBodyState in this.fleets_HighFilterForSpaceBody)
			{
				list.AddRangeUnique<TINaturalSpaceObjectState>(TINaturalSpaceObjectState.GetFilteredSolarSystemGroupObjects(tispaceBodyState, true));
			}
			if (this.fleets_HighFilterForSpaceBody.Count == 1 && this.fleets_HighFilterForSpaceBody[0] == GameStateManager.Luna())
			{
				list.Remove(GameStateManager.Earth());
			}
			int num = 0;
			foreach (TINaturalSpaceObjectState tinaturalSpaceObjectState in list)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
				{
					text = tinaturalSpaceObjectState.displayName,
					image = tinaturalSpaceObjectState.icon
				};
				this.locationDropdown_Specific.options.Add(optionData);
				this.specificLocationDropdownLookup.Add(num, tinaturalSpaceObjectState);
				num++;
				if (num == this.locationDropdown_Specific_EntryLimit)
				{
					TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData
					{
						text = Loc.T("UI.Habs.TooManyLocations")
					};
					this.locationDropdown_Specific.options.Add(optionData2);
					this.specificLocationDropdownLookup.Add(num, null);
					break;
				}
			}
			this.locationDropdown_Specific.SetValueWithoutNotify(0);
			this.locationDropdown_Specific.captionText.SetText(Loc.T("UI.Habs.NoLocations"));
		}

		// Token: 0x06004D75 RID: 19829 RVA: 0x0020FE34 File Offset: 0x0020E034
		public IEnumerator InitFleetsList()
		{
			this.fleetScreenFleetListAdapter.gameObject.SetActive(true);
			yield return null;
			this.initFleetsList = true;
			this.UpdateFleetsList();
			yield break;
		}

		// Token: 0x06004D76 RID: 19830 RVA: 0x0020FE44 File Offset: 0x0020E044
		public void UpdateFleetsList()
		{
			if (!GameControl.loadcycle100)
			{
				return;
			}
			if (GameControl.loadcycle100 && !this.initFleetsList)
			{
				base.StartCoroutine(this.InitFleetsList());
				return;
			}
			this.fleetListDirty = false;
			TooltipManager.Instance.HideAll();
			if (!this.constructionManagerCanvas.enabled)
			{
				this.fleetListCanvas.enabled = true;
			}
			else
			{
				this.RefreshConstructionManager();
			}
			List<TIGameState> list = new List<TIGameState>();
			List<TISpaceFleetState> list2 = base.activePlayer.KnownFleets;
			if (this.fleets_filterForFaction != null)
			{
				list2 = list2.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction == this.fleets_filterForFaction).ToList<TISpaceFleetState>();
			}
			if (this.fleetList_FilterHumanFactionsOnly)
			{
				list2 = list2.Where<TISpaceFleetState>((TISpaceFleetState x) => !x.IsAlien()).ToList<TISpaceFleetState>();
			}
			if (this.fleets_SpecificFilterForNaturalSpaceObject != null && this.fleets_SpecificFilterForNaturalSpaceObject.Count > 0)
			{
				list2 = list2.Where<TISpaceFleetState>((TISpaceFleetState x) => this.fleets_SpecificFilterForNaturalSpaceObject.Contains(x.ref_naturalSpaceObject) || this.fleets_SpecificFilterForNaturalSpaceObject.Contains(TISpaceFleetState.FinalDestinationNaturalSpaceObject(x))).ToList<TISpaceFleetState>();
			}
			else if (this.fleets_HighFilterForSpaceBody != null && this.fleets_HighFilterForSpaceBody.Count > 0)
			{
				List<TINaturalSpaceObjectState> candidateObjects = new List<TINaturalSpaceObjectState>();
				foreach (TISpaceBodyState tispaceBodyState in this.fleets_HighFilterForSpaceBody)
				{
					candidateObjects.AddRange(TINaturalSpaceObjectState.GetFilteredSolarSystemGroupObjects(tispaceBodyState, true));
				}
				if (this.fleets_HighFilterForSpaceBody.Count == 1 && this.fleets_HighFilterForSpaceBody[0] == GameStateManager.Luna())
				{
					candidateObjects.Remove(GameStateManager.Earth());
				}
				list2 = list2.Where<TISpaceFleetState>((TISpaceFleetState x) => candidateObjects.Contains(x.ref_naturalSpaceObject) || candidateObjects.Contains(TISpaceFleetState.FinalDestinationNaturalSpaceObject(x))).ToList<TISpaceFleetState>();
			}
			list2 = this.SortFleetsList(list2);
			list2 = list2.OrderByDescending<TISpaceFleetState, TIFactionState>((TISpaceFleetState o) => o.ref_faction).ToList<TISpaceFleetState>();
			foreach (TISpaceFleetState tispaceFleetState in list2)
			{
				list.Add(tispaceFleetState);
				if (!this.fleetOpenedStatus.Keys.Contains(tispaceFleetState))
				{
					this.fleetOpenedStatus.Add(tispaceFleetState, false);
				}
				if (this.fleetOpenedStatus[tispaceFleetState])
				{
					List<TISpaceShipState> list3 = new List<TISpaceShipState>();
					foreach (TISpaceShipState tispaceShipState in tispaceFleetState.ships)
					{
						list3.Add(tispaceShipState);
					}
					list3 = list3.OrderByDescending<TISpaceShipState, int>((TISpaceShipState o) => o.hull.internalSize).ToList<TISpaceShipState>();
					list3 = list3.OrderByDescending<TISpaceShipState, float>((TISpaceShipState o) => o.hull.mass_tons).ToList<TISpaceShipState>();
					list = list.Concat<TIGameState>(list3).ToList<TIGameState>();
				}
			}
			this.SetFleetListModelData(list);
			this.designShipButton_FleetList.interactable = FleetsScreenController.CanDesignShips(base.activePlayer, false);
			if (!this.constructionManagerCanvas.enabled && !this.fleetsTutorialLock)
			{
				this.ShowFleetListTutorial();
			}
		}

		// Token: 0x06004D77 RID: 19831 RVA: 0x002101D8 File Offset: 0x0020E3D8
		public void SetFleetListModelData(List<TIGameState> fleetsAndShips)
		{
			this.fleetScreenFleetListModels.Clear();
			int count = fleetsAndShips.Count;
			List<TIFactionState> list = new List<TIFactionState>();
			for (int i = 0; i < count; i++)
			{
				FleetScreenFleetListItemModel fleetScreenFleetListItemModel = new FleetScreenFleetListItemModel();
				FleetScreenFleetListItem_Data fleetScreenFleetListItem_Data = new FleetScreenFleetListItem_Data();
				fleetScreenFleetListItem_Data.gameStateFleetOrShip = fleetsAndShips[i];
				fleetScreenFleetListItem_Data.controller = this;
				if (fleetsAndShips[i].isSpaceFleetState && !list.Contains(fleetsAndShips[i].ref_faction) && fleetsAndShips[i].ref_faction != null)
				{
					FleetScreenFleetListItemModel fleetScreenFleetListItemModel2 = new FleetScreenFleetListItemModel();
					FleetScreenFleetListItem_Data fleetScreenFleetListItem_Data2 = new FleetScreenFleetListItem_Data();
					fleetScreenFleetListItem_Data2.gameStateFleetOrShip = fleetsAndShips[i];
					fleetScreenFleetListItem_Data2.controller = this;
					list.Add(fleetsAndShips[i].ref_faction);
					fleetScreenFleetListItem_Data2.isGroupItem = true;
					fleetScreenFleetListItemModel2.FleetScreenFleetListItemData = fleetScreenFleetListItem_Data2;
					this.fleetScreenFleetListModels.Add(fleetScreenFleetListItemModel2);
				}
				if (!this.hiddenFactions.Contains(fleetsAndShips[i].ref_faction) && (fleetsAndShips[i].isSpaceFleetState || this.fleetOpenedStatus[fleetsAndShips[i].ref_fleet]))
				{
					fleetScreenFleetListItemModel.FleetScreenFleetListItemData = fleetScreenFleetListItem_Data;
					this.fleetScreenFleetListModels.Add(fleetScreenFleetListItemModel);
				}
			}
			this.fleetScreenFleetListAdapter.SetItems(this.fleetScreenFleetListModels);
		}

		// Token: 0x06004D78 RID: 19832 RVA: 0x00210324 File Offset: 0x0020E524
		private void ValidateFleetScreenGameStateModels()
		{
			List<FleetScreenFleetListItemModel> list = new List<FleetScreenFleetListItemModel>();
			foreach (FleetScreenFleetListItemModel fleetScreenFleetListItemModel in this.fleetScreenFleetListModels)
			{
				if (!TIGameState.Valid(fleetScreenFleetListItemModel.FleetScreenFleetListItemData.gameStateFleetOrShip))
				{
					list.Add(fleetScreenFleetListItemModel);
				}
			}
			foreach (FleetScreenFleetListItemModel fleetScreenFleetListItemModel2 in list)
			{
				this.fleetScreenFleetListModels.Remove(fleetScreenFleetListItemModel2);
			}
			this.fleetScreenFleetListAdapter.SetItems(this.fleetScreenFleetListModels);
		}

		// Token: 0x06004D79 RID: 19833 RVA: 0x002103E4 File Offset: 0x0020E5E4
		public void OnClickFleetSort(int sortBy)
		{
			SortFleetDataBy sortFleetDataBy = this.currentFleetSort;
			this.currentFleetSort = (SortFleetDataBy)sortBy;
			if (this.currentFleetSort != sortFleetDataBy)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				this.UpdateFleetsList();
			}
		}

		// Token: 0x06004D7A RID: 19834 RVA: 0x0021041C File Offset: 0x0020E61C
		public List<TISpaceFleetState> SortFleetsList(List<TISpaceFleetState> fleetStateList)
		{
			switch (this.currentFleetSort)
			{
			case SortFleetDataBy.Alfa:
				fleetStateList = fleetStateList.OrderBy<TISpaceFleetState, string>((TISpaceFleetState o) => o.ref_fleet.GetDisplayName(base.activePlayer)).ToList<TISpaceFleetState>();
				break;
			case SortFleetDataBy.CombatValue:
				fleetStateList = fleetStateList.OrderByDescending<TISpaceFleetState, float>((TISpaceFleetState o) => o.ref_fleet.SpaceCombatValue()).ToList<TISpaceFleetState>();
				break;
			case SortFleetDataBy.Ships:
				fleetStateList = fleetStateList.OrderByDescending<TISpaceFleetState, int>((TISpaceFleetState o) => o.ref_fleet.ships.Count<TISpaceShipState>()).ToList<TISpaceFleetState>();
				break;
			case SortFleetDataBy.OrbitInterest:
				fleetStateList = fleetStateList.OrderByDescending<TISpaceFleetState, int>((TISpaceFleetState o) => o.ref_fleet.GetFleetOrbitInterestLevel()).ToList<TISpaceFleetState>();
				break;
			case SortFleetDataBy.ArrivalTime:
				fleetStateList = fleetStateList.OrderBy<TISpaceFleetState, TIDateTime>((TISpaceFleetState o) => o.GetArrivalTimeSortWeight()).ToList<TISpaceFleetState>();
				break;
			case SortFleetDataBy.DV:
				fleetStateList = fleetStateList.OrderByDescending<TISpaceFleetState, float>((TISpaceFleetState o) => o.ref_fleet.currentDeltaV_kps).ToList<TISpaceFleetState>();
				break;
			case SortFleetDataBy.DamagedShips:
				fleetStateList = fleetStateList.OrderByDescending<TISpaceFleetState, bool>((TISpaceFleetState o) => o.ref_fleet.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.damaged)).ToList<TISpaceFleetState>();
				break;
			case SortFleetDataBy.Officers:
				fleetStateList = fleetStateList.OrderByDescending<TISpaceFleetState, int>((TISpaceFleetState o) => o.ref_fleet.GetOfficerCountInShips()).ToList<TISpaceFleetState>();
				break;
			case SortFleetDataBy.Accel:
				fleetStateList = fleetStateList.OrderByDescending<TISpaceFleetState, float>((TISpaceFleetState o) => o.ref_fleet.fullyLoadedAcceleration_gs).ToList<TISpaceFleetState>();
				break;
			case SortFleetDataBy.Operations:
				fleetStateList = fleetStateList.OrderByDescending<TISpaceFleetState, int>((TISpaceFleetState o) => o.ref_fleet.AllowedOpsList().Count).ToList<TISpaceFleetState>();
				break;
			}
			return fleetStateList;
		}

		// Token: 0x06004D7B RID: 19835 RVA: 0x00210630 File Offset: 0x0020E830
		public void UpdateShipClassListScreen()
		{
			this.shipClassListCanvas.enabled = true;
			int num = 0;
			List<TISpaceShipTemplate> list = base.activePlayer.shipDesigns;
			if (this.classListHideObsoleteToggle.isOn)
			{
				list = base.activePlayer.shipDesigns.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => !x.Obsolete(base.activePlayer)).ToList<TISpaceShipTemplate>();
			}
			list = this.SortFleetClassList(list);
			this.shipClassList.SetListSize<ShipClassListItemController>(list.Count, false, false);
			using (IEnumerator<object> enumerator = this.shipClassList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__185.<>p__0 == null)
					{
						FleetsScreenController.<>o__185.<>p__0 = CallSite<Func<CallSite, object, ShipClassListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipClassListItemController), typeof(FleetsScreenController)));
					}
					ShipClassListItemController shipClassListItemController = FleetsScreenController.<>o__185.<>p__0.Target(FleetsScreenController.<>o__185.<>p__0, enumerator.Current);
					shipClassListItemController.Init(this, list[num++]);
					shipClassListItemController.UpdateListItem();
				}
			}
			this.ShowClassListTutorial();
		}

		// Token: 0x06004D7C RID: 19836 RVA: 0x00210738 File Offset: 0x0020E938
		public void OnClickFleetClassSort(int sortBy)
		{
			SortFleetClassDataBy sortFleetClassDataBy = this.currentFleetClassSort;
			this.currentFleetClassSort = (SortFleetClassDataBy)sortBy;
			if (this.currentFleetClassSort == sortFleetClassDataBy)
			{
				this.invertFleetClassSort = !this.invertFleetClassSort;
			}
			else
			{
				this.invertFleetClassSort = false;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.UpdateShipClassListScreen();
		}

		// Token: 0x06004D7D RID: 19837 RVA: 0x00210788 File Offset: 0x0020E988
		public List<TISpaceShipTemplate> SortFleetClassList(List<TISpaceShipTemplate> fleetClassList)
		{
			switch (this.currentFleetClassSort)
			{
			case SortFleetClassDataBy.Alfa:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = fleetClassList.OrderBy<TISpaceShipTemplate, string>((TISpaceShipTemplate x) => x.className).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = fleetClassList.OrderByDescending<TISpaceShipTemplate, string>((TISpaceShipTemplate x) => x.className).ToList<TISpaceShipTemplate>();
				}
				break;
			case SortFleetClassDataBy.Hull:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = (from x in fleetClassList
						orderby x.hullTemplate.mass_tons, x.displayName
						select x).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = (from x in fleetClassList
						orderby x.hullTemplate.mass_tons descending, x.displayName
						select x).ToList<TISpaceShipTemplate>();
				}
				break;
			case SortFleetClassDataBy.Role:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = fleetClassList.OrderBy<TISpaceShipTemplate, string>((TISpaceShipTemplate x) => x.roleStr).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = fleetClassList.OrderByDescending<TISpaceShipTemplate, string>((TISpaceShipTemplate x) => x.roleStr).ToList<TISpaceShipTemplate>();
				}
				break;
			case SortFleetClassDataBy.Mass:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = fleetClassList.OrderBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.wetMass_tons).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = fleetClassList.OrderByDescending<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.wetMass_tons).ToList<TISpaceShipTemplate>();
				}
				break;
			case SortFleetClassDataBy.CombatValue:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = fleetClassList.OrderBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.TemplateSpaceCombatValue(false, -1f, 1f, false)).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = fleetClassList.OrderByDescending<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.TemplateSpaceCombatValue(false, -1f, 1f, false)).ToList<TISpaceShipTemplate>();
				}
				break;
			case SortFleetClassDataBy.AssaultValue:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = fleetClassList.OrderBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.AssaultCombatValue(false)).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = fleetClassList.OrderByDescending<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.AssaultCombatValue(false)).ToList<TISpaceShipTemplate>();
				}
				break;
			case SortFleetClassDataBy.CruiseAcceleration:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = fleetClassList.OrderBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.baseCruiseAcceleration_gs(false)).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = fleetClassList.OrderByDescending<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.baseCruiseAcceleration_gs(false)).ToList<TISpaceShipTemplate>();
				}
				break;
			case SortFleetClassDataBy.CombatAcceleration:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = fleetClassList.OrderBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.baseCombatAcceleration_gs).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = fleetClassList.OrderByDescending<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.baseCombatAcceleration_gs).ToList<TISpaceShipTemplate>();
				}
				break;
			case SortFleetClassDataBy.DV:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = fleetClassList.OrderBy<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.baseCruiseDeltaV_kps(false)).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = fleetClassList.OrderByDescending<TISpaceShipTemplate, float>((TISpaceShipTemplate x) => x.baseCruiseDeltaV_kps(false)).ToList<TISpaceShipTemplate>();
				}
				break;
			case SortFleetClassDataBy.Ships:
				if (!this.invertFleetClassSort)
				{
					fleetClassList = fleetClassList.OrderBy<TISpaceShipTemplate, int>((TISpaceShipTemplate x) => GameControl.control.activePlayer.ships.Count<TISpaceShipState>((TISpaceShipState o) => o.templateName == x.dataName)).ToList<TISpaceShipTemplate>();
				}
				else
				{
					fleetClassList = fleetClassList.OrderByDescending<TISpaceShipTemplate, int>((TISpaceShipTemplate x) => GameControl.control.activePlayer.ships.Count<TISpaceShipState>((TISpaceShipState o) => o.templateName == x.dataName)).ToList<TISpaceShipTemplate>();
				}
				break;
			}
			return fleetClassList;
		}

		// Token: 0x06004D7E RID: 19838 RVA: 0x00210C31 File Offset: 0x0020EE31
		public void OnCloseShipClassListScreenButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.shipClassListCanvas.enabled = false;
			this.fleetListCanvas.enabled = true;
			this.HideTutorials();
			this.ShowFleetListTutorial();
		}

		// Token: 0x06004D7F RID: 19839 RVA: 0x00210C63 File Offset: 0x0020EE63
		public void OnDesignShipButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.tabbedPaneManager.Toggle(this.shipDesignerTab);
		}

		// Token: 0x06004D80 RID: 19840 RVA: 0x00210C82 File Offset: 0x0020EE82
		public void ShowShipDesigner()
		{
			this.OnCreateNewShipClicked();
			this.UpdateShipModuleToggles();
			this.UpdateTransferInfo();
		}

		// Token: 0x06004D81 RID: 19841 RVA: 0x00210C98 File Offset: 0x0020EE98
		public void OnCloseIndividualShipDataScreen()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.selectedShip = null;
			this.individualShipCanvas.enabled = false;
			this.fleetListCanvas.enabled = true;
			global::UnityEngine.Object.Destroy(this.indivShipVisObject, 0f);
			if (this.fleetListDirty)
			{
				this.UpdateFleetsList();
			}
			this.HideTutorials();
			this.ShowFleetListTutorial();
		}

		// Token: 0x06004D82 RID: 19842 RVA: 0x00210CFC File Offset: 0x0020EEFC
		public void SetIndividualShipList()
		{
			List<TISpaceShipState> list = (from x in base.activePlayer.knownShips
				orderby x.faction == base.activePlayer descending, x.faction.IsActiveHumanFaction descending, x.fleet.faction, x.fleet.displayName, x.fleet.GetSunOrbitingRelatedObject.semiMajorAxis_AU
				select x).ToList<TISpaceShipState>();
			this.ShipDetailShipListModels.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].faction != null && list[i].fleet.ships.Contains(list[i]) && ((this.showEnemyShipsOnList || list[i].faction == base.activePlayer) && (!this.showOnlyShipsInSelectedFleet || list[i].fleet == this.selectedShip.fleet)))
				{
					ShipDetailShipListItemModel shipDetailShipListItemModel = new ShipDetailShipListItemModel();
					ShipDetailShipListItem_Data shipDetailShipListItem_Data = new ShipDetailShipListItem_Data
					{
						controller = this,
						shipState = list[i]
					};
					shipDetailShipListItemModel.ShipDetailShipListItemData = shipDetailShipListItem_Data;
					this.ShipDetailShipListModels.Add(shipDetailShipListItemModel);
				}
			}
			this.ShipDetailShipListAdapter.SetItems(this.ShipDetailShipListModels);
			this.shipListInitialized = true;
		}

		// Token: 0x06004D83 RID: 19843 RVA: 0x00210EB0 File Offset: 0x0020F0B0
		public void UpdateShipNames()
		{
			using (IEnumerator<object> enumerator = this.shipsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__263.<>p__0 == null)
					{
						FleetsScreenController.<>o__263.<>p__0 = CallSite<Func<CallSite, object, ShipScreenShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipScreenShipListItemController), typeof(FleetsScreenController)));
					}
					ShipScreenShipListItemController shipScreenShipListItemController = FleetsScreenController.<>o__263.<>p__0.Target(FleetsScreenController.<>o__263.<>p__0, enumerator.Current);
					shipScreenShipListItemController.UpdateNames(shipScreenShipListItemController.ship, this);
				}
			}
		}

		// Token: 0x06004D84 RID: 19844 RVA: 0x00210F44 File Offset: 0x0020F144
		public void ShowIndividualShipList()
		{
			using (IEnumerator<object> enumerator = this.shipsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__264.<>p__0 == null)
					{
						FleetsScreenController.<>o__264.<>p__0 = CallSite<Func<CallSite, object, ShipScreenShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipScreenShipListItemController), typeof(FleetsScreenController)));
					}
					ShipScreenShipListItemController shipScreenShipListItemController = FleetsScreenController.<>o__264.<>p__0.Target(FleetsScreenController.<>o__264.<>p__0, enumerator.Current);
					TISpaceShipState ship = shipScreenShipListItemController.ship;
					if (((ship != null) ? ship.faction : null) != null && shipScreenShipListItemController.ship.fleet.ships.Contains(shipScreenShipListItemController.ship))
					{
						shipScreenShipListItemController.gameObject.SetActive(this.showEnemyShipsOnList || shipScreenShipListItemController.ship.faction == base.activePlayer);
					}
					else
					{
						shipScreenShipListItemController.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06004D85 RID: 19845 RVA: 0x00211044 File Offset: 0x0020F244
		public void OnToggleShipList()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.showEnemyShipsOnList = this.showEnemyShipsToggle.isOn;
			this.SetIndividualShipList();
		}

		// Token: 0x06004D86 RID: 19846 RVA: 0x00211069 File Offset: 0x0020F269
		public void OnToggleShowOnlyShipsInSelectedFleet()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.showEnemyShipsToggle.interactable = !this.showOnlyShipsInSelectedFleetToggle.isOn;
			this.showOnlyShipsInSelectedFleet = this.showOnlyShipsInSelectedFleetToggle.isOn;
			this.SetIndividualShipList();
		}

		// Token: 0x06004D87 RID: 19847 RVA: 0x002110A8 File Offset: 0x0020F2A8
		public void ShowIndividualDataScreen(TISpaceShipState selectedShip, bool togglePane = true)
		{
			if (selectedShip != null)
			{
				this.showEnemyShipsToggle.isOn = selectedShip.faction != GameControl.control.activePlayer;
				this.selectedShip = selectedShip;
			}
			else
			{
				this.showEnemyShipsToggle.isOn = true;
			}
			if (togglePane)
			{
				this.tabbedPaneManager.Toggle(this.shipDetailTab);
			}
			if (!this.shipListInitialized)
			{
				this.showEnemyShipsOnList = this.showEnemyShipsToggle.isOn;
				this.SetIndividualShipList();
			}
			if (this.individualShipCameraObject == null)
			{
				this.individualShipCameraObject = global::UnityEngine.Object.Instantiate<GameObject>(this.individualShipCameraPrefab);
				this.individualShipCamera = this.individualShipCameraObject.GetComponent<Camera>();
			}
			if (selectedShip != null)
			{
				this.UpdateIndividualDataScreen(selectedShip);
				if (this.showOnlyShipsInSelectedFleet)
				{
					this.SetIndividualShipList();
				}
			}
			this.RevertRename();
			this.ShowShipDetailTutorial();
		}

		// Token: 0x06004D88 RID: 19848 RVA: 0x00211180 File Offset: 0x0020F380
		public void ShowIndividualDataScreenFromTab()
		{
			if (!this.shipListInitialized)
			{
				this.showEnemyShipsOnList = this.showEnemyShipsToggle.isOn;
				this.SetIndividualShipList();
			}
			if (this.selectedShip == null || this.selectedShip.deleted)
			{
				List<TISpaceShipState> list = (from x in base.activePlayer.knownShips
					orderby x.faction == base.activePlayer descending, x.faction.IsActiveHumanFaction descending, x.fleet.GetSunOrbitingRelatedObject.semiMajorAxis_AU, x.fleet.displayName
					select x).ToList<TISpaceShipState>();
				if (list.Count > 0)
				{
					TISpaceShipState tispaceShipState = list.First<TISpaceShipState>();
					this.selectedShip = tispaceShipState;
					this.ShowIndividualDataScreen(tispaceShipState, false);
					return;
				}
			}
			else
			{
				this.ShowIndividualDataScreen(this.selectedShip, false);
			}
		}

		// Token: 0x06004D89 RID: 19849 RVA: 0x00211287 File Offset: 0x0020F487
		public void ChangeViewAngleDragDown()
		{
			this.shipModelViewer.isDragging = true;
		}

		// Token: 0x06004D8A RID: 19850 RVA: 0x00211295 File Offset: 0x0020F495
		public void ChangeViewAngleDragUp()
		{
			this.shipModelViewer.isDragging = false;
		}

		// Token: 0x06004D8B RID: 19851 RVA: 0x002112A4 File Offset: 0x0020F4A4
		public static string dualAccelerationStr(TISpaceShipTemplate ship)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.Fleets.TwoValues", new object[]
			{
				FleetsScreenController.accelerationStr((double)ship.baseCruiseAcceleration_gs(false), false, false, true),
				FleetsScreenController.accelerationStr((double)ship.baseCombatAcceleration_gs, true, false, true)
			}));
			return stringBuilder.ToString();
		}

		// Token: 0x06004D8C RID: 19852 RVA: 0x002112FC File Offset: 0x0020F4FC
		public static string dualAccelerationStr(TISpaceShipState ship)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.Fleets.TwoValues", new object[]
			{
				FleetsScreenController.accelerationStr((double)ship.cruiseAcceleration_gs, false, false, true),
				FleetsScreenController.accelerationStr((double)ship.combatAcceleration_gs, true, false, true)
			}));
			return stringBuilder.ToString();
		}

		// Token: 0x06004D8D RID: 19853 RVA: 0x00211350 File Offset: 0x0020F550
		public static string accelerationStr(double accel_gs, bool combat, bool expandedText, bool abbreviate = false)
		{
			string text;
			if (accel_gs >= 1.0)
			{
				text = Loc.T("UI.Fleets.Accelgs", new object[] { accel_gs.ToString("N1") });
			}
			else if (abbreviate)
			{
				text = Loc.T("UI.Fleets.AccelmgsAbbr", new object[] { (accel_gs * 1000.0).ToString(TIUtilities.DecimalPlaces(accel_gs * 1000.0, 3, 0)) });
			}
			else
			{
				text = Loc.T("UI.Fleets.Accelmgs", new object[] { (accel_gs * 1000.0).ToString(TIUtilities.DecimalPlaces(accel_gs * 1000.0, 7, 0)) });
			}
			if (!expandedText)
			{
				return text;
			}
			if (combat)
			{
				return Loc.T("UI.Fleets.CombatAccel", new object[] { text });
			}
			return Loc.T("UI.Fleets.CruiseAccel", new object[] { text });
		}

		// Token: 0x06004D8E RID: 19854 RVA: 0x00211434 File Offset: 0x0020F634
		public void UpdateIndividualDataScreen(TISpaceShipState selectedShip)
		{
			if (!TIGameState.Valid(selectedShip))
			{
				this.SetIndividualShipList();
				return;
			}
			this.selectedShip = selectedShip;
			using (IEnumerator<object> enumerator = this.shipsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__281.<>p__0 == null)
					{
						FleetsScreenController.<>o__281.<>p__0 = CallSite<Func<CallSite, object, ShipScreenShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipScreenShipListItemController), typeof(FleetsScreenController)));
					}
					FleetsScreenController.<>o__281.<>p__0.Target(FleetsScreenController.<>o__281.<>p__0, enumerator.Current).OnNewShipSelected(selectedShip);
				}
			}
			bool flag = selectedShip.isAlien && TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(GameStateManager.Sol(), selectedShip.fleet) > 149597870700.0 * (double)(1.02f + TIEffectsState.SumEffectsModifiers(Context.OuterExplorationRange_AU, base.activePlayer, 1.02f, null));
			bool flag2 = selectedShip.isAlien && !base.activePlayer.finishedProjectNames.Contains("Project_TheirWarships");
			this.hideCrew = flag2 || flag;
			this.hidePowerPlant = flag;
			this.hideBattery = flag2 || flag;
			this.hideRadiator = flag;
			this.hideHeatSink = flag2 || flag;
			this.hideArmor = flag;
			this.hideWeapons = flag;
			this.indiv_ShipName.SetText(selectedShip.displayName);
			this.indiv_ShipClass.SetText(selectedShip.template.fullClassName);
			this.invid_RefuelCost.SetText(Loc.T("UI.Fleets.PropellantType", new object[] { selectedShip.template.propellantTanksBuildCost(selectedShip.faction).ToString("Relevant", false, false, null, false, FactionResource.None) }));
			this.indiv_LocationText.SetText(selectedShip.fleet.GetLocationDescription(base.activePlayer, true, true));
			this.indiv_CrewText.SetText(Loc.T("UI.Fleets.Crew", new object[] { this.hideCrew ? Loc.T("UI.Fleets.Unknown") : selectedShip.template.crewBillets.ToString("N0") }));
			this.indiv_DryMassText.SetText(Loc.T("UI.Fleets.DryMass", new object[] { selectedShip.dryMass_tons.ToString("N0") }));
			this.indiv_WetMassText.SetText(Loc.T("UI.Fleets.WetMass", new object[] { selectedShip.wetMass_tons.ToString("N0") }));
			this.indiv_CurrentMassText.SetText(Loc.T("UI.Fleets.CurrentMass", new object[] { selectedShip.currentMass_tons.ToString("N0") }));
			if (selectedShip.currentMaxDeltaV_kps != selectedShip.template.baseCruiseDeltaV_kps(false))
			{
				this.indiv_DeltaVText.SetText(Loc.T("UI.Fleets.ThreeDV", new object[]
				{
					TIUtilities.FormatBigOrSmallNumber(selectedShip.currentDeltaV_kps, 1, 7, 0, false, false),
					TIUtilities.FormatBigOrSmallNumber(selectedShip.currentMaxDeltaV_kps, 1, 7, 0, false, false),
					TIUtilities.FormatBigOrSmallNumber(selectedShip.template.baseCruiseDeltaV_kps(false), 1, 7, 0, false, false)
				}));
			}
			else
			{
				this.indiv_DeltaVText.SetText(Loc.T("UI.Fleets.DV", new object[]
				{
					TIUtilities.FormatBigOrSmallNumber(selectedShip.currentDeltaV_kps, 1, 7, 0, false, false),
					TIUtilities.FormatBigOrSmallNumber(selectedShip.currentMaxDeltaV_kps, 1, 7, 0, false, false)
				}));
			}
			this.indiv_CruiseAccelerationText.SetText(FleetsScreenController.accelerationStr((double)selectedShip.cruiseAcceleration_gs, false, true, false));
			this.indiv_CombatAccelerationText.SetText(FleetsScreenController.accelerationStr((double)selectedShip.combatAcceleration_gs, true, true, false));
			this.indiv_TurnRateText.SetText(Loc.T("UI.Fleets.TurnRate", new object[] { TIUtilities.FormatSmallNumber(Mathf.Min(selectedShip.angularAcceleration_degs2, selectedShip.maxAngularVelocity_rad_s * 57.29578f), 7, 0, true, false) }));
			this.indiv_LengthText.SetText(Loc.T("UI.Fleets.Length", new object[] { selectedShip.hull.length_m.ToString("N0") }));
			this.indiv_BeamText.SetText(Loc.T("UI.Fleets.Beam", new object[] { selectedShip.hull.width_m.ToString("N0") }));
			this.indiv_DriveText.SetText(Loc.T("UI.Fleets.DriveLine", new object[] { selectedShip.template.driveTemplate.displayName }));
			this.indiv_RoleText.SetText(Loc.T("UI.Fleets.RoleLine", new object[] { selectedShip.template.roleStr }));
			this.indiv_PowerPlantText.SetText(Loc.T("UI.Fleets.PowerPlantLine", new object[] { this.hidePowerPlant ? Loc.T("UI.Fleets.Unknown") : selectedShip.template.powerPlantTemplate.displayName }));
			this.indiv_BatteryText.SetText(Loc.T("UI.Fleets.BatteryLine", new object[] { this.hideBattery ? Loc.T("UI.Fleets.Unknown") : ((selectedShip.template.BatteryCapacity_GJ(false) > 0f) ? Loc.T("UI.Fleets.GJ", new object[] { selectedShip.template.BatteryCapacity_GJ(false).ToString("N0") }) : Loc.T("UI.Fleets.NoBatteries")) }));
			this.indiv_RadiatorsText.SetText(Loc.T("UI.Fleets.RadiatorsLine", new object[] { this.hideRadiator ? Loc.T("UI.Fleets.Unknown") : selectedShip.template.radiatorTemplate.displayName }));
			this.indiv_HeatSinkCapacityText.SetText(this.hideHeatSink ? Loc.T("UI.Fleets.HeatSinkCapacityUnk") : Loc.T("UI.Fleets.HeatSinkCapacity", new object[] { selectedShip.template.HeatCapacity_GJ(false).ToString("N0") }));
			this.indiv_NoseArmorMaterial.SetText(this.hideArmor ? Loc.T("UI.Fleets.NoseArmorMaterialunk") : Loc.T("UI.Fleets.NoseArmorMaterial", new object[]
			{
				(selectedShip.noseArmorThickness_m * 100f).ToString(TIUtilities.DecimalPlaces((double)(selectedShip.noseArmorThickness_m * 100f), 7, 0)),
				selectedShip.noseArmorTemplate.displayName
			}));
			this.indiv_NoseArmorRating.SetText(Loc.T("UI.Fleets.ArmorRating", new object[] { this.hideArmor ? Loc.T("UI.Fleets.Unknown") : selectedShip.noseArmorValue.ToString("N0") }));
			this.indiv_LateralArmorMaterial.SetText(this.hideArmor ? Loc.T("UI.Fleets.LateralArmorMaterialunk") : Loc.T("UI.Fleets.LateralArmorMaterial", new object[]
			{
				(selectedShip.lateralArmorThickness_m * 100f).ToString(TIUtilities.DecimalPlaces((double)(selectedShip.lateralArmorThickness_m * 100f), 7, 0)),
				selectedShip.lateralArmorTemplate.displayName
			}));
			this.indiv_LateralArmorRating.SetText(Loc.T("UI.Fleets.ArmorRating", new object[] { this.hideArmor ? Loc.T("UI.Fleets.Unknown") : selectedShip.template.lateralArmorValue.ToString("N0") }));
			this.indiv_TailArmorMaterial.SetText(this.hideArmor ? Loc.T("UI.Fleets.TailArmorMaterialunk") : Loc.T("UI.Fleets.TailArmorMaterial", new object[]
			{
				(selectedShip.tailArmorThickness_m * 100f).ToString(TIUtilities.DecimalPlaces((double)(selectedShip.tailArmorThickness_m * 100f), 7, 0)),
				selectedShip.tailArmorTemplate.displayName
			}));
			this.indiv_TailArmorRating.SetText(Loc.T("UI.Fleets.ArmorRating", new object[] { this.hideArmor ? Loc.T("UI.Fleets.Unknown") : selectedShip.tailArmorValue.ToString("N0") }));
			this.systemsHeader.SetText(Loc.T("UI.Fleets.PrimarySystemsHeader"));
			this.missionSystemsHeader.SetText(Loc.T("UI.Fleets.MissionSystemsHeader"));
			if (this.hideWeapons)
			{
				this.noseWeaponsList.gameObject.SetActive(false);
				this.hullWeaponsList.gameObject.SetActive(false);
				this.utilityModulesList.gameObject.SetActive(false);
			}
			else
			{
				if (selectedShip.noseWeapons.Count > 0)
				{
					this.noseWeaponsList.SetListSize<ShipDetailModuleListItemController>(selectedShip.noseWeapons.Count, false, false);
					int num = 0;
					using (IEnumerator<object> enumerator = this.noseWeaponsList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							FleetsScreenController.<>c__DisplayClass281_0 CS$<>8__locals1 = new FleetsScreenController.<>c__DisplayClass281_0();
							CS$<>8__locals1.<>4__this = this;
							FleetsScreenController.<>c__DisplayClass281_0 CS$<>8__locals2 = CS$<>8__locals1;
							if (FleetsScreenController.<>o__281.<>p__1 == null)
							{
								FleetsScreenController.<>o__281.<>p__1 = CallSite<Func<CallSite, object, ShipDetailModuleListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipDetailModuleListItemController), typeof(FleetsScreenController)));
							}
							CS$<>8__locals2.controller = FleetsScreenController.<>o__281.<>p__1.Target(FleetsScreenController.<>o__281.<>p__1, enumerator.Current);
							CS$<>8__locals1.controller.Init(this);
							CS$<>8__locals1.controller.UpdateListItem(selectedShip.noseWeapons[num], selectedShip.noseWeapons[num++].moduleTemplate, selectedShip);
							CS$<>8__locals1.controller.systemIndex = num;
							CS$<>8__locals1.controller.GetComponent<Button>().onClick.RemoveAllListeners();
							CS$<>8__locals1.controller.GetComponent<Button>().onClick.AddListener(delegate
							{
								CS$<>8__locals1.<>4__this.OnClickUpdateRightDetailPanel(1, CS$<>8__locals1.controller.systemIndex);
							});
						}
					}
					this.noseWeaponsList.gameObject.SetActive(true);
				}
				else
				{
					this.noseWeaponsList.gameObject.SetActive(false);
				}
				if (selectedShip.hullWeapons.Count > 0)
				{
					this.hullWeaponsList.SetListSize<ShipDetailModuleListItemController>(selectedShip.hullWeapons.Count, false, false);
					int num2 = 0;
					using (IEnumerator<object> enumerator = this.hullWeaponsList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							FleetsScreenController.<>c__DisplayClass281_1 CS$<>8__locals3 = new FleetsScreenController.<>c__DisplayClass281_1();
							CS$<>8__locals3.<>4__this = this;
							FleetsScreenController.<>c__DisplayClass281_1 CS$<>8__locals4 = CS$<>8__locals3;
							if (FleetsScreenController.<>o__281.<>p__2 == null)
							{
								FleetsScreenController.<>o__281.<>p__2 = CallSite<Func<CallSite, object, ShipDetailModuleListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipDetailModuleListItemController), typeof(FleetsScreenController)));
							}
							CS$<>8__locals4.controller = FleetsScreenController.<>o__281.<>p__2.Target(FleetsScreenController.<>o__281.<>p__2, enumerator.Current);
							CS$<>8__locals3.controller.Init(this);
							CS$<>8__locals3.controller.UpdateListItem(selectedShip.hullWeapons[num2], selectedShip.hullWeapons[num2++].moduleTemplate, selectedShip);
							CS$<>8__locals3.controller.systemIndex = num2;
							CS$<>8__locals3.controller.GetComponent<Button>().onClick.RemoveAllListeners();
							CS$<>8__locals3.controller.GetComponent<Button>().onClick.AddListener(delegate
							{
								CS$<>8__locals3.<>4__this.OnClickUpdateRightDetailPanel(2, CS$<>8__locals3.controller.systemIndex);
							});
						}
					}
					this.hullWeaponsList.gameObject.SetActive(true);
				}
				else
				{
					this.hullWeaponsList.gameObject.SetActive(false);
				}
				if (selectedShip.utilityModules.Count > 0)
				{
					int num3 = 0;
					this.utilityModulesList.SetListSize<ShipDetailModuleListItemController>(selectedShip.utilityModules.Count, false, false);
					using (IEnumerator<object> enumerator = this.utilityModulesList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							FleetsScreenController.<>c__DisplayClass281_2 CS$<>8__locals5 = new FleetsScreenController.<>c__DisplayClass281_2();
							CS$<>8__locals5.<>4__this = this;
							FleetsScreenController.<>c__DisplayClass281_2 CS$<>8__locals6 = CS$<>8__locals5;
							if (FleetsScreenController.<>o__281.<>p__3 == null)
							{
								FleetsScreenController.<>o__281.<>p__3 = CallSite<Func<CallSite, object, ShipDetailModuleListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipDetailModuleListItemController), typeof(FleetsScreenController)));
							}
							CS$<>8__locals6.controller = FleetsScreenController.<>o__281.<>p__3.Target(FleetsScreenController.<>o__281.<>p__3, enumerator.Current);
							CS$<>8__locals5.controller.Init(this);
							CS$<>8__locals5.controller.UpdateListItem(selectedShip.utilityModules[num3], selectedShip.utilityModules[num3++].moduleTemplate, selectedShip);
							CS$<>8__locals5.controller.systemIndex = num3;
							CS$<>8__locals5.controller.GetComponent<Button>().onClick.RemoveAllListeners();
							CS$<>8__locals5.controller.GetComponent<Button>().onClick.AddListener(delegate
							{
								CS$<>8__locals5.<>4__this.OnClickUpdateRightDetailPanel(3, CS$<>8__locals5.controller.systemIndex);
							});
						}
					}
					this.utilityModulesList.gameObject.SetActive(true);
				}
				else
				{
					this.utilityModulesList.gameObject.SetActive(false);
				}
			}
			if (selectedShip.faction == base.activePlayer)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(selectedShip.hull.largeCombatUIPath(selectedShip.template.GetHullAppearanceIndex), this.hullDamageControlImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(selectedShip.radiators.largecombatUI_On(selectedShip.hull, selectedShip.template.GetHullAppearanceIndex), this.radiatorDamageControlImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(selectedShip.drive.largeCombatUIPath(selectedShip.hull, selectedShip.template.GetHullAppearanceIndex), this.driveDamageControlImage);
				this.moduleDamageGrid = new Dictionary<ModuleDataEntry, SpaceCombatDamageGridItemController>();
				this.systemDamageGrid = new Dictionary<ShipSystem, SpaceCombatDamageGridItemController>();
				foreach (SpaceCombatDamageGridItemController spaceCombatDamageGridItemController in this.masterDamageGridControllers.Values)
				{
					spaceCombatDamageGridItemController.Clear();
				}
				foreach (ModuleDataEntry moduleDataEntry in selectedShip.AllWeaponModuleData())
				{
					SpaceCombatDamageGridItemController spaceCombatDamageGridItemController2 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetModuleDamageControllerPosition(selectedShip, moduleDataEntry)];
					spaceCombatDamageGridItemController2.Initialize(selectedShip, moduleDataEntry);
					this.moduleDamageGrid.Add(moduleDataEntry, spaceCombatDamageGridItemController2);
				}
				foreach (ModuleDataEntry moduleDataEntry2 in selectedShip.utilityModules)
				{
					SpaceCombatDamageGridItemController spaceCombatDamageGridItemController3 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetModuleDamageControllerPosition(selectedShip, moduleDataEntry2)];
					spaceCombatDamageGridItemController3.Initialize(selectedShip, moduleDataEntry2);
					this.moduleDamageGrid.Add(moduleDataEntry2, spaceCombatDamageGridItemController3);
				}
				SpaceCombatDamageGridItemController spaceCombatDamageGridItemController4 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetSystemDamageControllerPosition(selectedShip, ShipSystem.Drive)];
				ModuleDataEntry driveModule = selectedShip.driveModule;
				spaceCombatDamageGridItemController4.Initialize(selectedShip, driveModule);
				this.moduleDamageGrid.Add(driveModule, spaceCombatDamageGridItemController4);
				SpaceCombatDamageGridItemController spaceCombatDamageGridItemController5 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetSystemDamageControllerPosition(selectedShip, ShipSystem.Radiators)];
				ModuleDataEntry radiatorModule = selectedShip.radiatorModule;
				spaceCombatDamageGridItemController5.Initialize(selectedShip, radiatorModule);
				this.moduleDamageGrid.Add(radiatorModule, spaceCombatDamageGridItemController5);
				SpaceCombatDamageGridItemController spaceCombatDamageGridItemController6 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetSystemDamageControllerPosition(selectedShip, ShipSystem.PowerPlant)];
				ModuleDataEntry powerPlantModule = selectedShip.powerPlantModule;
				spaceCombatDamageGridItemController6.Initialize(selectedShip, powerPlantModule);
				this.moduleDamageGrid.Add(powerPlantModule, spaceCombatDamageGridItemController6);
				foreach (ShipSystem shipSystem in Enums.DamageableShipSystems)
				{
					SpaceCombatDamageGridItemController spaceCombatDamageGridItemController7 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetSystemDamageControllerPosition(selectedShip, shipSystem)];
					spaceCombatDamageGridItemController7.Initialize(selectedShip, shipSystem);
					this.systemDamageGrid.Add(shipSystem, spaceCombatDamageGridItemController7);
				}
				this.damageControlPanel.SetActive(true);
				List<TIOfficerState> list = selectedShip.officers.OrderBy<TIOfficerState, int>((TIOfficerState x) => x.template.sortOrder).ToList<TIOfficerState>();
				this.officersList.SetListSize<ShipOfficerGridItemController>(selectedShip.officers.Count, false, false);
				int num4 = 0;
				using (IEnumerator<object> enumerator = this.officersList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__281.<>p__4 == null)
						{
							FleetsScreenController.<>o__281.<>p__4 = CallSite<Func<CallSite, object, ShipOfficerGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipOfficerGridItemController), typeof(FleetsScreenController)));
						}
						FleetsScreenController.<>o__281.<>p__4.Target(FleetsScreenController.<>o__281.<>p__4, enumerator.Current).UpdateGridItem(list[num4++]);
					}
					goto IL_0FB5;
				}
			}
			this.damageControlPanel.SetActive(false);
			this.officersList.SetListSize<ShipOfficerGridItemController>(0, true, true);
			IL_0FB5:
			this.UpdateIndivCameraImage(selectedShip);
			this.UpdateLeftDetailPanel(0);
			this.UpdateRightDetailPanel(0, 0);
		}

		// Token: 0x06004D8F RID: 19855 RVA: 0x00212470 File Offset: 0x00210670
		public void OnClickUpdateLeftDetailPanel(int primarySystem)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
			this.UpdateLeftDetailPanel(primarySystem);
		}

		// Token: 0x06004D90 RID: 19856 RVA: 0x00212488 File Offset: 0x00210688
		public void UpdateLeftDetailPanel(int primarySystem)
		{
			if (this.selectedShip != null)
			{
				switch (primarySystem)
				{
				case 0:
					this.leftHandDetailPanelHeader.SetText("");
					this.leftSystemDetail.SetText("");
					return;
				case 1:
					this.leftHandDetailPanelHeader.SetText(this.selectedShip.drive.displayName);
					this.leftSystemDetail.SetText("");
					this.leftSystemDetail.SetText(this.selectedShip.template.driveTemplate.GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.Drive, false));
					return;
				case 2:
					if (this.hidePowerPlant)
					{
						this.leftSystemDetail.SetText("");
						this.leftHandDetailPanelHeader.SetText("");
						return;
					}
					this.leftHandDetailPanelHeader.SetText(this.selectedShip.powerPlant.displayName);
					this.leftSystemDetail.SetText("");
					this.leftSystemDetail.SetText(this.selectedShip.template.powerPlantTemplate.GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.PowerPlant, false));
					return;
				case 3:
					if (this.hideBattery || this.selectedShip.template.batteryTemplates.Count == 0)
					{
						this.leftSystemDetail.SetText("");
						this.leftHandDetailPanelHeader.SetText("");
						return;
					}
					this.leftHandDetailPanelHeader.SetText(this.selectedShip.template.batteryTemplates[0].displayName);
					this.leftSystemDetail.SetText("");
					this.leftSystemDetail.SetText(this.selectedShip.template.batteryTemplates[0].GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.Utility, false));
					return;
				case 4:
					if (this.hideRadiator)
					{
						this.leftHandDetailPanelHeader.SetText("");
						this.leftSystemDetail.SetText("");
						return;
					}
					this.leftHandDetailPanelHeader.SetText(this.selectedShip.radiators.displayName);
					this.leftSystemDetail.SetText("");
					this.leftSystemDetail.SetText(this.selectedShip.template.radiatorTemplate.GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.Radiator, false));
					return;
				case 5:
					if (this.hideArmor)
					{
						this.leftHandDetailPanelHeader.SetText("");
						this.leftSystemDetail.SetText("");
						return;
					}
					this.leftHandDetailPanelHeader.SetText(this.selectedShip.template.noseArmor.materialTemplate.displayName);
					this.leftSystemDetail.SetText("");
					this.leftSystemDetail.SetText(this.selectedShip.template.noseArmorTemplate.GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.NoseArmor, false));
					return;
				case 6:
					if (this.hideArmor)
					{
						this.leftHandDetailPanelHeader.SetText("");
						this.leftSystemDetail.SetText("");
						return;
					}
					this.leftHandDetailPanelHeader.SetText(this.selectedShip.template.lateralArmor.materialTemplate.displayName);
					this.leftSystemDetail.SetText("");
					this.leftSystemDetail.SetText(this.selectedShip.template.lateralArmorTemplate.GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.LateralArmor, false));
					return;
				case 7:
					if (this.hideArmor)
					{
						this.leftHandDetailPanelHeader.SetText("");
						this.leftSystemDetail.SetText("");
						return;
					}
					this.leftHandDetailPanelHeader.SetText(this.selectedShip.template.tailArmor.materialTemplate.displayName);
					this.leftSystemDetail.SetText("");
					this.leftSystemDetail.SetText(this.selectedShip.template.tailArmorTemplate.GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.TailArmor, false));
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06004D91 RID: 19857 RVA: 0x002128C9 File Offset: 0x00210AC9
		public void OnClickUpdateRightDetailPanel(int systemType, int systemIndex)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
			this.UpdateRightDetailPanel(systemType, systemIndex);
		}

		// Token: 0x06004D92 RID: 19858 RVA: 0x002128E0 File Offset: 0x00210AE0
		public void UpdateRightDetailPanel(int systemType, int systemIndex)
		{
			if (this.selectedShip != null)
			{
				switch (systemType)
				{
				case 0:
					this.rightHandDetailPanelHeader.SetText("");
					this.rightSystemDetail.SetText("");
					return;
				case 1:
					this.rightHandDetailPanelHeader.SetText(this.selectedShip.template.noseWeapons.ToList<ModuleDataEntry>()[systemIndex - 1].moduleTemplate.displayName);
					this.rightSystemDetail.SetText("");
					this.rightSystemDetail.SetText(Loc.T(this.selectedShip.template.noseWeaponTemplates.ToList<TIShipWeaponTemplate>()[systemIndex - 1].GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.None, true)));
					return;
				case 2:
					this.rightHandDetailPanelHeader.SetText(this.selectedShip.template.hullWeapons.ToList<ModuleDataEntry>()[systemIndex - 1].moduleTemplate.displayName);
					this.rightSystemDetail.SetText("");
					this.rightSystemDetail.SetText(Loc.T(this.selectedShip.template.hullWeaponTemplates.ToList<TIShipWeaponTemplate>()[systemIndex - 1].GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.None, true)));
					return;
				case 3:
				{
					List<ModuleDataEntry> list = this.selectedShip.template.utilityModules.ToList<ModuleDataEntry>();
					this.rightHandDetailPanelHeader.SetText(list[systemIndex - 1].moduleTemplate.displayName);
					this.rightSystemDetail.SetText("");
					this.rightSystemDetail.SetText(Loc.T(list[systemIndex - 1].moduleTemplate.GetFullDescription(this.selectedShip, this.selectedShip.template, false, ShipModuleSlotType.None, true)));
					break;
				}
				default:
					return;
				}
			}
		}

		// Token: 0x06004D93 RID: 19859 RVA: 0x00212AC4 File Offset: 0x00210CC4
		public void OnClickCycleSelectedShipDetail(bool forward)
		{
			int num = this.ShipDetailShipListModels.IndexOf(this.ShipDetailShipListModels.Where<ShipDetailShipListItemModel>((ShipDetailShipListItemModel x) => x.ShipDetailShipListItemData.shipState == this.selectedShip).First<ShipDetailShipListItemModel>());
			if (forward)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				num++;
				if (num > this.ShipDetailShipListModels.Count - 1)
				{
					num = 0;
				}
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
				num--;
				if (num < 0)
				{
					num = this.ShipDetailShipListModels.Count - 1;
				}
			}
			this.UpdateIndividualDataScreen(this.ShipDetailShipListModels[num].ShipDetailShipListItemData.shipState);
		}

		// Token: 0x06004D94 RID: 19860 RVA: 0x00212B60 File Offset: 0x00210D60
		private void InitializeShipDesigner()
		{
			this.shipModuleSlotGrid = this.ShipDesignerCanvas.gameObject.GetComponentOnChild<GridLayoutGroup>("ShipModules");
			this.moduleDragDestinations = base.gameObject.GetComponentsInChildren<ShipModuleDragDestination>();
			for (int i = 0; i < this.moduleDragDestinations.Length; i++)
			{
				this.moduleDragDestinations[i].SetControllerBase(this);
				this.moduleDragDestinations[i].transform.localPosition = this.moduleDragDestinations[i].defaultPosition;
			}
			this.shipModuleSlotDictionary = new Dictionary<Vector2Int, ShipModuleDragDestination>();
			for (int j = 0; j < this.moduleDragDestinations.Length; j++)
			{
				ShipModuleDragDestination shipModuleDragDestination = this.moduleDragDestinations[j];
				int num = Mathf.FloorToInt((float)j / 10f);
				int num2 = j - 10 * num;
				this.shipModuleSlotDictionary.Add(new Vector2Int(num2, num), shipModuleDragDestination);
			}
			this.CacheAllShipModules();
			this.automateRoleButtonTip.SetDelegate("BodyText", () => this.SetAutomateButtonTip());
			this.roleTip.SetDelegate("BodyText", () => this.SetRoleTip());
			this.ShowObsoletePartsText.SetText(Loc.T("UI.Science.SortObsolete"));
			this.ShipDesignerCanvas.enabled = false;
			this.ShowObsoletePartsToggle.isOn = base.activePlayer.showObsoleteParts;
			this.FilterAvailableShipModules();
			this.SelectedCompareModulesChanged(this.selectedModulesCompareToggle.isOn);
			this.selectedModuleCompareHeaderText.SetText(Loc.T("UI.Fleets.Compare"));
			this.installedModuleCompareHeaderText.SetText(Loc.T("UI.Fleets.Compare"));
		}

		// Token: 0x06004D95 RID: 19861 RVA: 0x00212CE0 File Offset: 0x00210EE0
		private void UpdateShipModuleToggles()
		{
			foreach (ShipModuleListItem shipModuleListItem in this.shipModuleListItems)
			{
				shipModuleListItem.UpdateToggle(base.activePlayer);
			}
			foreach (ShipModuleListItem shipModuleListItem2 in this.shipModuleListItemsB)
			{
				shipModuleListItem2.UpdateToggle(base.activePlayer);
			}
		}

		// Token: 0x06004D96 RID: 19862 RVA: 0x00212D7C File Offset: 0x00210F7C
		private void CacheAllShipModules()
		{
			this.allShipPartTemplates = (from x in TemplateManager.IterateByClass<TIShipPartTemplate>(true)
				where x.allowedSlots != null && x.allowedSlots.Any<ShipModuleSlotType>() && x.dataName != "Empty"
				select x).ToList<TIShipPartTemplate>();
			Log.Time("<color=#00cc00>LoadTime:</color> CacheAllShipModules", delegate
			{
				CoroutineDummy.Singleton.StartCoroutine(this.CacheAllShipModulesGradual());
			}, true, true);
		}

		// Token: 0x06004D97 RID: 19863 RVA: 0x00212DD6 File Offset: 0x00210FD6
		private IEnumerator CacheAllShipModulesGradual()
		{
			int count = 0;
			if (count == 2000)
			{
				count = 0;
				yield return null;
			}
			foreach (TIShipPartTemplate tishipPartTemplate in this.allShipPartTemplates)
			{
				Transform transform = null;
				Transform transform2 = null;
				switch (tishipPartTemplate.allowedSlots[0])
				{
				case ShipModuleSlotType.Utility:
					transform = this.utilitiesTabPane.icons.iconsContainer;
					transform2 = this.utilitiesTabPane.table.rowsContainer;
					break;
				case ShipModuleSlotType.PowerPlant:
					transform = this.powerPlantsTabPane.icons.iconsContainer;
					transform2 = this.powerPlantsTabPane.table.rowsContainer;
					break;
				case ShipModuleSlotType.Radiator:
					transform = this.radiatorsTabPane.icons.iconsContainer;
					transform2 = this.radiatorsTabPane.table.rowsContainer;
					break;
				case ShipModuleSlotType.Drive:
					if (tishipPartTemplate.ref_drive.thrusters > 1)
					{
						continue;
					}
					transform = this.drivesTabPane.icons.iconsContainer;
					transform2 = this.drivesTabPane.table.rowsContainer;
					break;
				case ShipModuleSlotType.NoseArmor:
				case ShipModuleSlotType.LateralArmor:
				case ShipModuleSlotType.TailArmor:
					transform = this.armorTabPane.icons.iconsContainer;
					transform2 = this.armorTabPane.table.rowsContainer;
					break;
				case ShipModuleSlotType.NoseHardPoint:
				case ShipModuleSlotType.HullHardPoint:
				{
					TIShipWeaponTemplate ref_weapon = tishipPartTemplate.ref_weapon;
					if (!ref_weapon.shipWeapon || ref_weapon.fighterOnlyWeapon)
					{
						continue;
					}
					Transform transform3 = null;
					if (ref_weapon.noseWeapon)
					{
						transform3 = this.weaponsTabPane.noseIcons.iconsContainer;
					}
					else if (ref_weapon.hullWeapon)
					{
						transform3 = this.weaponsTabPane.hullIcons.iconsContainer;
					}
					ShipModuleListItem shipModuleListItem = global::UnityEngine.Object.Instantiate<ShipModuleListItem>(this.shipModuleIconPrefab, transform3);
					shipModuleListItem.transform.localScale = Vector3.one;
					shipModuleListItem.SetController(this);
					shipModuleListItem.SetModuleTemplate(tishipPartTemplate);
					this.shipModuleListItems.Add(shipModuleListItem);
					if (tishipPartTemplate.isGunTypeWeapon)
					{
						TIGunTypeWeaponTemplate ref_gunWeapon = tishipPartTemplate.ref_gunWeapon;
						if (ref_gunWeapon.isMagneticGunWeapon)
						{
							if (ref_weapon.noseWeapon)
							{
								transform = this.magneticWeaponsTabPane.noseIcons.iconsContainer;
								transform2 = this.magneticWeaponsTabPane.noseTable.rowsContainer;
							}
							else
							{
								transform = this.magneticWeaponsTabPane.hullIcons.iconsContainer;
								transform2 = this.magneticWeaponsTabPane.hullTable.rowsContainer;
							}
						}
						else if (ref_gunWeapon.isPlasmaWeapon)
						{
							if (ref_weapon.noseWeapon)
							{
								transform = this.plasmaWeaponsTabPane.noseIcons.iconsContainer;
								transform2 = this.plasmaWeaponsTabPane.noseTable.rowsContainer;
							}
							else
							{
								transform = this.plasmaWeaponsTabPane.hullIcons.iconsContainer;
								transform2 = this.plasmaWeaponsTabPane.hullTable.rowsContainer;
							}
						}
						else if (ref_weapon.noseWeapon)
						{
							transform = this.gunsTabPane.noseIcons.iconsContainer;
							transform2 = this.gunsTabPane.noseTable.rowsContainer;
						}
						else
						{
							transform = this.gunsTabPane.hullIcons.iconsContainer;
							transform2 = this.gunsTabPane.hullTable.rowsContainer;
						}
					}
					else if (tishipPartTemplate.isMissileWeapon)
					{
						if (ref_weapon.noseWeapon)
						{
							transform = this.missilesTabPane.noseIcons.iconsContainer;
							transform2 = this.missilesTabPane.noseTable.rowsContainer;
						}
						else
						{
							transform = this.missilesTabPane.hullIcons.iconsContainer;
							transform2 = this.missilesTabPane.hullTable.rowsContainer;
						}
					}
					else if (tishipPartTemplate.isLaserWeapon)
					{
						if (ref_weapon.noseWeapon)
						{
							transform = this.lasersTabPane.noseIcons.iconsContainer;
							transform2 = this.lasersTabPane.noseTable.rowsContainer;
						}
						else
						{
							transform = this.lasersTabPane.hullIcons.iconsContainer;
							transform2 = this.lasersTabPane.hullTable.rowsContainer;
						}
					}
					else if (tishipPartTemplate.isParticleWeapon)
					{
						if (ref_weapon.noseWeapon)
						{
							transform = this.particleWeaponsTabPane.noseIcons.iconsContainer;
							transform2 = this.particleWeaponsTabPane.noseTable.rowsContainer;
						}
						else
						{
							transform = this.particleWeaponsTabPane.hullIcons.iconsContainer;
							transform2 = this.particleWeaponsTabPane.hullTable.rowsContainer;
						}
					}
					break;
				}
				}
				ShipModuleListItem shipModuleListItem2 = global::UnityEngine.Object.Instantiate<ShipModuleListItem>(this.shipModuleIconPrefab, transform);
				shipModuleListItem2.transform.localScale = Vector3.one;
				shipModuleListItem2.SetController(this);
				shipModuleListItem2.SetModuleTemplate(tishipPartTemplate);
				this.shipModuleListItems.Add(shipModuleListItem2);
				int num = count;
				count = num + 1;
				ShipModuleListItem shipModuleListItem3 = global::UnityEngine.Object.Instantiate<ShipModuleListItem>(this.shipModuleRowPrefab, transform2);
				shipModuleListItem3.transform.localScale = Vector3.one;
				shipModuleListItem3.SetController(this);
				shipModuleListItem3.SetModuleTemplate(tishipPartTemplate);
				this.shipModuleListItemsB.Add(shipModuleListItem3);
				Loc.SwapFonts(shipModuleListItem3.gameObject);
				num = count;
				count = num + 1;
			}
			Debug.Log("All designer modules loaded, " + count.ToString());
			yield break;
		}

		// Token: 0x06004D98 RID: 19864 RVA: 0x00212DE5 File Offset: 0x00210FE5
		public void OnClickCloseShipDesigner()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.ShipDesignerCanvas.enabled = false;
			this.OnExitShipDesigner();
			this.UpdateShipClassListScreen();
		}

		// Token: 0x06004D99 RID: 19865 RVA: 0x00212E0B File Offset: 0x0021100B
		private void OnExitShipDesigner()
		{
			this.HideTutorials();
			if (!this.shipDesignInProgress)
			{
				this.newShipTemplate = null;
			}
			global::UnityEngine.Object.Destroy(this.fleetSceneCameraInstance, 0f);
		}

		// Token: 0x06004D9A RID: 19866 RVA: 0x00212E34 File Offset: 0x00211034
		private void HideEmptyWeaponTabButtons()
		{
			foreach (ShipWeaponTabPane shipWeaponTabPane in base.GetComponentsInChildren<ShipWeaponTabPane>())
			{
				bool flag = shipWeaponTabPane.noseIcons.iconsContainer.ActiveChildCount() > 0;
				bool flag2 = shipWeaponTabPane.hullIcons.iconsContainer.ActiveChildCount() > 0;
				bool flag3 = false;
				if ((this.noseModulesTabPane.IsSelected && flag) || (this.hullModulesTabPane.IsSelected && flag2))
				{
					flag3 = true;
				}
				shipWeaponTabPane.tabPane.TabButton.gameObject.SetActive(flag3);
			}
		}

		// Token: 0x06004D9B RID: 19867 RVA: 0x00212EBC File Offset: 0x002110BC
		public void OnNoseHardPointsTabButtonLeftClicked()
		{
			this.weaponPaneManagerCanvas.enabled = true;
			this.modulesTabPaneManager.Toggle(this.noseModulesTabPane);
			this.HideEmptyWeaponTabButtons();
			if (this.showShipPartsAsIcons)
			{
				this.weaponsTabPane.OnTabLeftClick();
				return;
			}
			if (this.gunsTabPane.tabActive)
			{
				this.gunsTabPane.OnTabLeftClick();
				return;
			}
			if (this.missilesTabPane.tabActive)
			{
				this.missilesTabPane.OnTabLeftClick();
				return;
			}
			if (this.magneticWeaponsTabPane.tabActive)
			{
				this.magneticWeaponsTabPane.OnTabLeftClick();
				return;
			}
			if (this.plasmaWeaponsTabPane.tabActive)
			{
				this.plasmaWeaponsTabPane.OnTabLeftClick();
				return;
			}
			if (this.lasersTabPane.tabActive)
			{
				this.lasersTabPane.OnTabLeftClick();
				return;
			}
			if (this.particleWeaponsTabPane.tabActive)
			{
				this.particleWeaponsTabPane.OnTabLeftClick();
				return;
			}
			this.showShipPartsAsIcons = true;
			this.UpdateWeaponTabAllSubtabInteractive();
			this.weaponsTabPane.OnTabLeftClick();
		}

		// Token: 0x06004D9C RID: 19868 RVA: 0x00212FAE File Offset: 0x002111AE
		public void UpdateWeaponTabAllSubtabInteractive()
		{
			this.weaponTabAllSubTabButton.interactable = this.showShipPartsAsIcons;
		}

		// Token: 0x06004D9D RID: 19869 RVA: 0x00212FC4 File Offset: 0x002111C4
		public void OnHullHardPointsTabButtonLeftClicked()
		{
			this.weaponPaneManagerCanvas.enabled = true;
			this.modulesTabPaneManager.Toggle(this.hullModulesTabPane);
			this.HideEmptyWeaponTabButtons();
			if (this.showShipPartsAsIcons)
			{
				this.weaponsTabPane.OnTabLeftClick();
				return;
			}
			if (this.gunsTabPane.tabActive)
			{
				this.gunsTabPane.OnTabLeftClick();
				return;
			}
			if (this.missilesTabPane.tabActive)
			{
				this.missilesTabPane.OnTabLeftClick();
				return;
			}
			if (this.magneticWeaponsTabPane.tabActive)
			{
				this.magneticWeaponsTabPane.OnTabLeftClick();
				return;
			}
			if (this.plasmaWeaponsTabPane.tabActive)
			{
				this.plasmaWeaponsTabPane.OnTabLeftClick();
				return;
			}
			if (this.lasersTabPane.tabActive)
			{
				this.lasersTabPane.OnTabLeftClick();
				return;
			}
			if (this.particleWeaponsTabPane.tabActive)
			{
				this.particleWeaponsTabPane.OnTabLeftClick();
				return;
			}
			this.showShipPartsAsIcons = true;
			this.UpdateWeaponTabAllSubtabInteractive();
			this.weaponsTabPane.OnTabLeftClick();
		}

		// Token: 0x06004D9E RID: 19870 RVA: 0x002130B6 File Offset: 0x002112B6
		public void OnNonWeaponTabButtonClicked()
		{
			this.weaponPaneManagerCanvas.enabled = false;
		}

		// Token: 0x06004D9F RID: 19871 RVA: 0x002130C4 File Offset: 0x002112C4
		public static bool CanDesignShips(TIFactionState faction, bool ignoreTech = false)
		{
			return FleetsScreenController.AllowedShipHulls(faction, ignoreTech).Count > 0;
		}

		// Token: 0x06004DA0 RID: 19872 RVA: 0x002130D8 File Offset: 0x002112D8
		private static List<TIShipHullTemplate> AllowedShipHulls(TIFactionState faction, bool ignoreTech = false)
		{
			IEnumerable<TIShipHullTemplate> enumerable;
			if (!ignoreTech)
			{
				enumerable = faction.allowedShipHulls;
			}
			else
			{
				enumerable = from x in TemplateManager.IterateByClass<TIShipHullTemplate>(true)
					where !x.alien || TemplateManager.global.debug_showAllShipPartsIncludingAlien
					select x;
			}
			return enumerable.OrderBy<TIShipHullTemplate, float>((TIShipHullTemplate x) => x.volume_m3).ToList<TIShipHullTemplate>();
		}

		// Token: 0x06004DA1 RID: 19873 RVA: 0x00213144 File Offset: 0x00211344
		private void OnCreateNewShipClicked()
		{
			this.refitting = false;
			this.validRefitNotificationObject.SetActive(false);
			List<TIShipHullTemplate> list = FleetsScreenController.AllowedShipHulls(base.activePlayer, false);
			if (list.Count > 0)
			{
				this.PopulateClassSelectionDropdown(null);
				if (!this.shipDesignInProgress)
				{
					this.ResetShip(list[0].dataName, "", ShipRole.NoRole);
					this.SetupDesignerLayout();
				}
				else
				{
					this.UpdateShipDesignDataPanelAndImage(true, true, false);
				}
				this.FilterAvailableShipModules();
				this.changesMadeToExistingClass = true;
				this.PopulateRoleDropdown(this.shipDesignInProgress ? this.newShipTemplate.role : ShipRole.NoRole);
				if (this.drivesTabPane.tabActive && !this.shipDesignInProgress)
				{
					this.drivesTabPane.OnTabLeftClick();
				}
				this.ShowDesignerTutorial();
			}
			this.shipDesignInProgress = true;
		}

		// Token: 0x06004DA2 RID: 19874 RVA: 0x0021320B File Offset: 0x0021140B
		private void OnShipPartUnlocked(ShipPartUnlocked e)
		{
			if (this.ShipDesignerCanvas.enabled)
			{
				this.FilterAvailableShipModules();
			}
		}

		// Token: 0x06004DA3 RID: 19875 RVA: 0x00213220 File Offset: 0x00211420
		public void ResetDesigner(string hullName, string forceClassName = "", ShipRole forceRole = ShipRole.NoRole)
		{
			this.ResetShip(hullName, TISpaceShipTemplate.illegalShipClassNames.Contains(forceClassName) ? string.Empty : forceClassName, ShipRole.NoRole);
			this.PopulateClassSelectionDropdown(this.newShipTemplate.hullTemplate);
			this.PopulateRoleDropdown(forceRole);
			this.SetupDesignerLayout();
			this.changesMadeToExistingClass = true;
			this.refitting = false;
			this.validRefitNotificationObject.SetActive(false);
		}

		// Token: 0x06004DA4 RID: 19876 RVA: 0x00213282 File Offset: 0x00211482
		public void OnResetShipClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.ResetDesigner(this.newShipTemplate.hullName, this.newShipTemplate.displayName, this.newShipTemplate.role);
			this.changesMadeToExistingClass = true;
		}

		// Token: 0x06004DA5 RID: 19877 RVA: 0x002132BE File Offset: 0x002114BE
		public void OnSaveDesignClicked()
		{
			if (!this.newShipTemplate.ValidTemplate)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			if (this.newShipTemplate.baseCruiseDeltaV_kps(true) >= 30f)
			{
				this.SaveDesign();
				return;
			}
			this.ShowDVWarning();
		}

		// Token: 0x06004DA6 RID: 19878 RVA: 0x002132FC File Offset: 0x002114FC
		public void SaveDesign()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			if (this.refitting)
			{
				base.activePlayer.UnlockAchievement("upgradeShipClass");
				this.oldShipTemplate.refitIteration++;
				this.newShipTemplate.refitIteration = this.oldShipTemplate.refitIteration;
			}
			base.activePlayer.playerControl.StartAction(new SaveShipDesignAction(base.activePlayer, this.newShipTemplate));
			this.ResetDesigner(this.newShipTemplate.hullName, "", this.newShipTemplate.role);
			base.activePlayer.CompleteMilestone(CampaignMilestone.TutorialDesignShip);
		}

		// Token: 0x06004DA7 RID: 19879 RVA: 0x002133A8 File Offset: 0x002115A8
		public void ShowDVWarning()
		{
			this.dVwarningObject.SetActive(true);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			if (this.newShipTemplate.baseCruiseDeltaV_kps(true) > 8f)
			{
				this.dVWarningText.SetText(Loc.T("UI.Fleets.DesignerDVWarningBody1"));
				return;
			}
			this.dVWarningText.SetText(Loc.T("UI.Fleets.DesignerDVWarningBody2"));
		}

		// Token: 0x06004DA8 RID: 19880 RVA: 0x0021340B File Offset: 0x0021160B
		public void OnClickDVWarningNo()
		{
			this.dVwarningObject.SetActive(false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
		}

		// Token: 0x06004DA9 RID: 19881 RVA: 0x00213425 File Offset: 0x00211625
		public void OnClickDVWarningYes()
		{
			this.SaveDesign();
			this.dVwarningObject.SetActive(false);
		}

		// Token: 0x06004DAA RID: 19882 RVA: 0x0021343C File Offset: 0x0021163C
		private void ResetShip(string hullName, string forceClassName = "", ShipRole forceRole = ShipRole.NoRole)
		{
			this.newShipTemplate = new TISpaceShipTemplate(TemplateManager.GenerateDataName("playerShipTemplate"))
			{
				hullName = hullName,
				factionName = base.activePlayer.templateName
			};
			this.newShipTemplate.InitAtRunTime(false);
			this.newShipTemplate.role = forceRole;
			if (forceClassName != "")
			{
				this.newShipTemplate.SetDisplayName(forceClassName);
			}
			this.classNamePlaceholder.SetText(this.newShipTemplate.displayName);
			this.fullShipClassName.SetText(this.newShipTemplate.fullClassName);
			this.designerShipDataClassName.SetText(this.newShipTemplate.fullClassName);
			this.UpdateShipDesignDataPanelAndImage(true, true, false);
			this.GetMaxHullIndex(this.newShipTemplate);
			this.designerSaveDesignButton.interactable = this.CanSaveCurrentDesign;
			FleetsScreenController.lastSCVUpdateFrame = -1;
		}

		// Token: 0x06004DAB RID: 19883 RVA: 0x00213518 File Offset: 0x00211718
		private void FilterAvailableShipModules()
		{
			if (this.loadingExistingTemplate)
			{
				return;
			}
			bool fullDesignerTest = this.fullDesignerTest;
			bool humanOnlyDesignerTest = this.humanOnlyDesignerTest;
			for (int i = 0; i < this.shipModuleListItems.Count; i++)
			{
				TIShipPartTemplate moduleTemplate = this.shipModuleListItems[i].GetModuleTemplate();
				if ((base.activePlayer.UnlockedShipPart(moduleTemplate) && (this.partsSortShowObsolete || !base.activePlayer.obsoletedShipParts.Contains(moduleTemplate.dataName))) || fullDesignerTest || (humanOnlyDesignerTest && !moduleTemplate.isAlien))
				{
					DragItem dragItem = this.shipModuleListItems[i];
					TISpaceShipTemplate tispaceShipTemplate = this.newShipTemplate;
					dragItem.draggable = tispaceShipTemplate != null && tispaceShipTemplate.ValidPartForDesign(moduleTemplate);
					this.shipModuleListItems[i].SetAlpha(this.shipModuleListItems[i].draggable);
					this.shipModuleListItems[i].transform.localScale = Vector3.one;
					if (!this.shipModuleListItems[i].gameObject.activeSelf)
					{
						this.shipModuleListItems[i].gameObject.SetActive(true);
					}
				}
				else if (this.shipModuleListItems[i].gameObject.activeSelf)
				{
					this.shipModuleListItems[i].gameObject.SetActive(false);
				}
			}
			bool flag = false;
			for (int j = 0; j < this.shipModuleListItemsB.Count; j++)
			{
				TIShipPartTemplate moduleTemplate2 = this.shipModuleListItemsB[j].GetModuleTemplate();
				if ((base.activePlayer.UnlockedShipPart(moduleTemplate2) && (this.partsSortShowObsolete || !base.activePlayer.obsoletedShipParts.Contains(moduleTemplate2.dataName))) || fullDesignerTest || (humanOnlyDesignerTest && !moduleTemplate2.isAlien))
				{
					Selectable addModuleButton = this.shipModuleListItemsB[j].addModuleButton;
					TISpaceShipTemplate tispaceShipTemplate2 = this.newShipTemplate;
					addModuleButton.interactable = tispaceShipTemplate2 != null && tispaceShipTemplate2.ValidPartForDesign(moduleTemplate2);
					this.shipModuleListItemsB[j].SetAlpha(this.shipModuleListItemsB[j].addModuleButton.interactable);
					if (!this.shipModuleListItemsB[j].gameObject.activeSelf)
					{
						this.shipModuleListItemsB[j].gameObject.SetActive(true);
						flag = true;
					}
				}
				else if (this.shipModuleListItemsB[j].gameObject.activeSelf)
				{
					this.shipModuleListItemsB[j].gameObject.SetActive(false);
					flag = true;
				}
			}
			if (flag)
			{
				this.RefreshModuleTableWidths();
			}
		}

		// Token: 0x06004DAC RID: 19884 RVA: 0x002137C4 File Offset: 0x002119C4
		private void RefreshModuleTableWidths()
		{
			this.gunsTabPane.ForceUpdateColumnWidths();
			this.missilesTabPane.ForceUpdateColumnWidths();
			this.magneticWeaponsTabPane.ForceUpdateColumnWidths();
			this.plasmaWeaponsTabPane.ForceUpdateColumnWidths();
			this.lasersTabPane.ForceUpdateColumnWidths();
			this.particleWeaponsTabPane.ForceUpdateColumnWidths();
			this.utilitiesTabPane.ForceUpdateColumnWidths();
			this.radiatorsTabPane.ForceUpdateColumnWidths();
			this.batteriesTabPane.ForceUpdateColumnWidths();
			this.powerPlantsTabPane.ForceUpdateColumnWidths();
			this.drivesTabPane.ForceUpdateColumnWidths();
			this.armorTabPane.ForceUpdateColumnWidths();
		}

		// Token: 0x06004DAD RID: 19885 RVA: 0x00213858 File Offset: 0x00211A58
		private void SetupDesignerLayout()
		{
			for (int i = 0; i < this.moduleDragDestinations.Length; i++)
			{
				this.moduleDragDestinations[i].DisableDestination();
				this.moduleDragDestinations[i].currentPart = null;
			}
			TIShipHullTemplate hullTemplate = this.newShipTemplate.hullTemplate;
			foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in hullTemplate.shipModuleSlots)
			{
				if (shipModuleSlot.moduleSlotType != ShipModuleSlotType.None)
				{
					Vector2Int vector2Int = new Vector2Int(shipModuleSlot.x, shipModuleSlot.y);
					ShipModuleDragDestination shipModuleDragDestination;
					if (this.shipModuleSlotDictionary.TryGetValue(vector2Int, out shipModuleDragDestination))
					{
						shipModuleDragDestination.SetEmpty();
						shipModuleDragDestination.EnableDestination(shipModuleSlot.moduleSlotType, vector2Int);
					}
					else
					{
						Log.Error(string.Format("Could not find ShipModuleDragDestination at coordinates {0},{1}", shipModuleSlot.x, shipModuleSlot.y), Array.Empty<object>());
					}
				}
			}
			foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot2 in hullTemplate.shipModuleSlots)
			{
				ShipModuleDragDestination shipModuleDragDestination;
				if (this.shipModuleSlotDictionary.TryGetValue(new Vector2Int(shipModuleSlot2.x, shipModuleSlot2.y), out shipModuleDragDestination) && shipModuleDragDestination.IsArmor)
				{
					if (shipModuleDragDestination.shipModuleSlotType == ShipModuleSlotType.LateralArmor && (hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.TailArmor).x + hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.NoseArmor).x) % 2 == 1)
					{
						shipModuleDragDestination.SetLayoutOffset((float)(shipModuleDragDestination.iconSize / 2), (float)(shipModuleDragDestination.iconSize / 2));
					}
					else
					{
						shipModuleDragDestination.SetLayoutOffset(0f, (float)(shipModuleDragDestination.iconSize / 2));
					}
				}
			}
			this.selectedDragDestination = null;
			this.selectedShipPart = null;
			this.UpdateModuleDataPanel(true, null, true, ShipModuleSlotType.None);
			this.UpdateModuleDataPanel(false, null, true, ShipModuleSlotType.None);
		}

		// Token: 0x06004DAE RID: 19886 RVA: 0x00213A40 File Offset: 0x00211C40
		private void UpdateAllArmorSlots()
		{
		}

		// Token: 0x06004DAF RID: 19887 RVA: 0x00213A44 File Offset: 0x00211C44
		public void RemoveModuleFromSlot(Vector2Int coordinates, bool updateRole = true, bool suppressSCVUpdate = false)
		{
			TIShipHullTemplate.ShipModuleSlot slotByCoordinates = this.newShipTemplate.hullTemplate.GetSlotByCoordinates(coordinates);
			int num = this.newShipTemplate.hullTemplate.slotIndex(slotByCoordinates);
			TIShipPartTemplate partInHullSlot = this.newShipTemplate.GetPartInHullSlot(slotByCoordinates, true);
			if (partInHullSlot != null)
			{
				if (partInHullSlot.isDrive)
				{
					this.newShipTemplate.SetDriveTemplate(string.Empty);
				}
				else if (partInHullSlot.isPowerPlant)
				{
					this.newShipTemplate.SetPowerPlantTemplate(string.Empty);
				}
				else if (partInHullSlot.isRadiator)
				{
					this.newShipTemplate.SetRadiatorTemplate(string.Empty);
				}
				else if (partInHullSlot.ref_armor == partInHullSlot)
				{
					switch (slotByCoordinates.moduleSlotType)
					{
					case ShipModuleSlotType.NoseArmor:
						this.newShipTemplate.SetNoseArmorTemplate(string.Empty);
						break;
					case ShipModuleSlotType.LateralArmor:
						this.newShipTemplate.SetLateralArmorTemplate(string.Empty);
						break;
					case ShipModuleSlotType.TailArmor:
						this.newShipTemplate.SetTailArmorTemplate(string.Empty);
						break;
					}
				}
				else
				{
					ModuleDataTemplateEntry moduleDataTemplateEntry = new ModuleDataTemplateEntry
					{
						moduleName = partInHullSlot.dataName,
						slot = num
					};
					if (partInHullSlot.allowedSlots.Contains(ShipModuleSlotType.Utility) && slotByCoordinates.moduleSlotType == ShipModuleSlotType.Utility)
					{
						this.newShipTemplate.moduleTemplateEntries.Remove(moduleDataTemplateEntry);
						if (partInHullSlot.isUtilityModule)
						{
							TIUtilityModuleTemplate ref_utilityModule = partInHullSlot.ref_utilityModule;
							if (ref_utilityModule == null || ref_utilityModule.armorMaxBonus != 0f)
							{
								this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.NoseArmor)].UpdateSpinnerValue(-1);
								this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.LateralArmor)].UpdateSpinnerValue(-1);
								this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.TailArmor)].UpdateSpinnerValue(-1);
							}
						}
					}
					else if (partInHullSlot.isWeapon)
					{
						TIShipWeaponTemplate ref_weapon = partInHullSlot.ref_weapon;
						if (slotByCoordinates.moduleSlotType == ShipModuleSlotType.NoseHardPoint)
						{
							this.newShipTemplate.noseWeaponTemplateEntries.Remove(moduleDataTemplateEntry);
							this.newShipTemplate.fireModeTemplateEntries.Remove(this.newShipTemplate.GetFireModeDataEntryFromSlot(moduleDataTemplateEntry.slot));
						}
						else if (slotByCoordinates.moduleSlotType == ShipModuleSlotType.HullHardPoint)
						{
							this.newShipTemplate.hullWeaponTemplateEntries.Remove(moduleDataTemplateEntry);
							this.newShipTemplate.fireModeTemplateEntries.Remove(this.newShipTemplate.GetFireModeDataEntryFromSlot(moduleDataTemplateEntry.slot));
						}
						foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in this.newShipTemplate.hullTemplate.WeaponSlotSet(this.newShipTemplate.hullTemplate.GetSlotByCoordinates(coordinates), ref_weapon.mount))
						{
							this.shipModuleSlotDictionary[shipModuleSlot.slotPosition].SetEmpty();
						}
					}
				}
			}
			this.shipModuleSlotDictionary[coordinates].SetEmpty();
			this.newShipTemplate.spaceResourceConstructionCost(true, null, true, false, false);
			this.newShipTemplate.dryMass_tons(true);
			this.newShipTemplate.HeatCapacity_GJ(true);
			this.newShipTemplate.BatteryCapacity_GJ(true);
			this.UpdateModuleDataPanel(false, null, true, ShipModuleSlotType.None);
			this.UpdateShipDesignDataPanelAndImage(partInHullSlot != null && (partInHullSlot.hasModel || partInHullSlot.isDrive || partInHullSlot.isPowerPlant || partInHullSlot.isWeapon), false, suppressSCVUpdate);
			if (updateRole)
			{
				this.PopulateRoleDropdown(this.newShipTemplate.role);
			}
			this.FilterAvailableShipModules();
			this.UpdateTransferInfo();
		}

		// Token: 0x06004DB0 RID: 19888 RVA: 0x00213DEC File Offset: 0x00211FEC
		public void SetModuleInSlot(TIShipPartTemplate module, ShipModuleDragDestination dropDestination, bool updateModelAndDropdowns = true)
		{
			if (dropDestination.currentPart != module)
			{
				this.changesMadeToExistingClass = true;
				TIShipWeaponTemplate tishipWeaponTemplate = module as TIShipWeaponTemplate;
				if (module.isDrive)
				{
					this.newShipTemplate.SetDriveTemplate(module.dataName);
					this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.Drive)].UpdateSpinnerValue(this.newShipTemplate.thrusterCount);
				}
				else if (module.isPowerPlant)
				{
					this.newShipTemplate.SetPowerPlantTemplate(module.dataName);
				}
				else if (module.isRadiator)
				{
					this.newShipTemplate.SetRadiatorTemplate(module.dataName);
				}
				else if (module.isArmor)
				{
					switch (dropDestination.shipModuleSlotType)
					{
					case ShipModuleSlotType.NoseArmor:
						this.newShipTemplate.SetNoseArmorTemplate(module.dataName);
						dropDestination.UpdateSpinnerValue(this.newShipTemplate.noseArmorValue);
						break;
					case ShipModuleSlotType.LateralArmor:
						this.newShipTemplate.SetLateralArmorTemplate(module.dataName);
						dropDestination.UpdateSpinnerValue(this.newShipTemplate.lateralArmorValue);
						break;
					case ShipModuleSlotType.TailArmor:
						this.newShipTemplate.SetTailArmorTemplate(module.dataName);
						dropDestination.UpdateSpinnerValue(this.newShipTemplate.tailArmorValue);
						break;
					}
				}
				else if (module.allowedSlots.Contains(ShipModuleSlotType.Utility) && dropDestination.shipModuleSlotType == ShipModuleSlotType.Utility)
				{
					this.RemoveModuleFromSlot(dropDestination.SlotCoordinates, true, true);
					ModuleDataTemplateEntry moduleDataTemplateEntry = new ModuleDataTemplateEntry
					{
						moduleName = module.dataName,
						slot = this.newShipTemplate.hullTemplate.slotIndex(this.newShipTemplate.hullTemplate.GetSlotByCoordinates(dropDestination.SlotCoordinates))
					};
					this.newShipTemplate.moduleTemplateEntries.Add(moduleDataTemplateEntry);
					TIUtilityModuleTemplate ref_utilityModule = module.ref_utilityModule;
					if (ref_utilityModule == null || ref_utilityModule.armorMaxBonus != 0f)
					{
						this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.NoseArmor)].UpdateSpinnerValue(this.newShipTemplate.noseArmor.armorValue);
						this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.LateralArmor)].UpdateSpinnerValue(this.newShipTemplate.lateralArmor.armorValue);
						this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.TailArmor)].UpdateSpinnerValue(this.newShipTemplate.tailArmor.armorValue);
					}
				}
				else if (tishipWeaponTemplate != null)
				{
					dropDestination.cornerIcon.gameObject.SetActive(true);
					bool noseWeapon = tishipWeaponTemplate.noseWeapon;
					if (noseWeapon && dropDestination.shipModuleSlotType == ShipModuleSlotType.NoseHardPoint)
					{
						List<TIShipHullTemplate.ShipModuleSlot> list = this.newShipTemplate.hullTemplate.WeaponSlotSet(this.newShipTemplate.hullTemplate.GetSlotByCoordinates(dropDestination.SlotCoordinates), tishipWeaponTemplate.mount);
						foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in list)
						{
							this.RemoveModuleFromSlot(shipModuleSlot.slotPosition, updateModelAndDropdowns, true);
						}
						ModuleDataTemplateEntry moduleDataTemplateEntry2 = new ModuleDataTemplateEntry
						{
							moduleName = module.dataName,
							slot = this.newShipTemplate.hullTemplate.slotIndex(this.newShipTemplate.hullTemplate.GetSlotByCoordinates(dropDestination.SlotCoordinates))
						};
						this.newShipTemplate.noseWeaponTemplateEntries.Add(moduleDataTemplateEntry2);
						FireModeDataTemplateEntry fireModeDataTemplateEntry = this.newShipTemplate.GetFireModeDataEntryFromSlot(moduleDataTemplateEntry2.slot);
						if (fireModeDataTemplateEntry.slot == 0)
						{
							fireModeDataTemplateEntry = new FireModeDataTemplateEntry
							{
								slot = moduleDataTemplateEntry2.slot,
								fireMode = tishipWeaponTemplate.DefaultFireMode
							};
							this.newShipTemplate.fireModeTemplateEntries.Add(fireModeDataTemplateEntry);
						}
						this.UpdateFireModeUI(tishipWeaponTemplate, fireModeDataTemplateEntry.fireMode, dropDestination.cornerIcon);
						using (List<TIShipHullTemplate.ShipModuleSlot>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								TIShipHullTemplate.ShipModuleSlot shipModuleSlot2 = enumerator.Current;
								if (shipModuleSlot2.slotPosition != dropDestination.SlotCoordinates)
								{
									this.shipModuleSlotDictionary[shipModuleSlot2.slotPosition].BlockDestination();
								}
							}
							goto IL_0646;
						}
					}
					if (!noseWeapon && dropDestination.shipModuleSlotType == ShipModuleSlotType.HullHardPoint)
					{
						List<TIShipHullTemplate.ShipModuleSlot> list2 = this.newShipTemplate.hullTemplate.WeaponSlotSet(this.newShipTemplate.hullTemplate.GetSlotByCoordinates(dropDestination.SlotCoordinates), tishipWeaponTemplate.mount);
						foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot3 in list2)
						{
							this.RemoveModuleFromSlot(shipModuleSlot3.slotPosition, updateModelAndDropdowns, true);
						}
						ModuleDataTemplateEntry moduleDataTemplateEntry3 = new ModuleDataTemplateEntry
						{
							moduleName = module.dataName,
							slot = this.newShipTemplate.hullTemplate.slotIndex(this.newShipTemplate.hullTemplate.GetSlotByCoordinates(dropDestination.SlotCoordinates))
						};
						this.newShipTemplate.hullWeaponTemplateEntries.Add(moduleDataTemplateEntry3);
						FireModeDataTemplateEntry fireModeDataTemplateEntry2 = this.newShipTemplate.GetFireModeDataEntryFromSlot(moduleDataTemplateEntry3.slot);
						if (fireModeDataTemplateEntry2.slot == 0)
						{
							fireModeDataTemplateEntry2 = new FireModeDataTemplateEntry
							{
								slot = moduleDataTemplateEntry3.slot,
								fireMode = tishipWeaponTemplate.DefaultFireMode
							};
							this.newShipTemplate.fireModeTemplateEntries.Add(fireModeDataTemplateEntry2);
						}
						this.UpdateFireModeUI(tishipWeaponTemplate, fireModeDataTemplateEntry2.fireMode, dropDestination.cornerIcon);
						foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot4 in list2)
						{
							if (shipModuleSlot4.slotPosition != dropDestination.SlotCoordinates)
							{
								this.shipModuleSlotDictionary[shipModuleSlot4.slotPosition].BlockDestination();
							}
						}
					}
				}
				IL_0646:
				dropDestination.currentPart = module;
				dropDestination.SetFilled();
				if (module.iconResource != null)
				{
					dropDestination.SetImage(module.iconResource, (tishipWeaponTemplate != null) ? tishipWeaponTemplate.mount : Mount.Standard);
				}
				if (TooltipManager.Instance.TooltipContainer.transform.parent != null && TooltipManager.Instance.TooltipContainer.transform.parent.name == "ShipDesigner")
				{
					dropDestination.tooltip.ForceHideTooltip();
				}
				dropDestination.tooltip.SetDelegate("BodyText", () => module.displayName);
				this.designerSaveDesignButton.interactable = this.CanSaveCurrentDesign;
				this.newShipTemplate.spaceResourceConstructionCost(true, null, true, false, false);
				this.newShipTemplate.dryMass_tons(true);
				this.UpdateShipDesignDataPanelAndImage(updateModelAndDropdowns && (module.hasModel || module.isDrive || module.isPowerPlant), false, false);
				if (updateModelAndDropdowns)
				{
					this.FilterAvailableShipModules();
					this.PopulateRoleDropdown(this.newShipTemplate.role);
				}
				this.UpdateTransferInfo();
				return;
			}
		}

		// Token: 0x17000EB0 RID: 3760
		// (get) Token: 0x06004DB1 RID: 19889 RVA: 0x0021459C File Offset: 0x0021279C
		// (set) Token: 0x06004DB2 RID: 19890 RVA: 0x002145A4 File Offset: 0x002127A4
		public ShipModuleDragDestination selectedDragDestination { get; private set; }

		// Token: 0x17000EB1 RID: 3761
		// (get) Token: 0x06004DB3 RID: 19891 RVA: 0x002145AD File Offset: 0x002127AD
		// (set) Token: 0x06004DB4 RID: 19892 RVA: 0x002145B5 File Offset: 0x002127B5
		public TIShipPartTemplate selectedShipPart { get; private set; }

		// Token: 0x06004DB5 RID: 19893 RVA: 0x002145BE File Offset: 0x002127BE
		public void SetSelectedDragDestination(ShipModuleDragDestination destination)
		{
			this.selectedDragDestination = destination;
			this.selectedShipPart = destination.currentPart;
			this.UpdateModuleDataPanel(false, this.selectedShipPart, false, destination.shipModuleSlotType);
			this.HighlightLegalPartDestinations();
		}

		// Token: 0x06004DB6 RID: 19894 RVA: 0x002145ED File Offset: 0x002127ED
		public void SetSelectedShipPartFromMenu(TIShipPartTemplate part)
		{
			this.selectedDragDestination = this.GetBestDropDestinationForModule(part);
			this.selectedShipPart = part;
			this.UpdateModuleDataPanel(true, this.selectedShipPart, true, ShipModuleSlotType.None);
			this.HighlightLegalPartDestinations();
		}

		// Token: 0x06004DB7 RID: 19895 RVA: 0x00214618 File Offset: 0x00212818
		public void HighlightLegalPartDestinations()
		{
			foreach (ShipModuleDragDestination shipModuleDragDestination in this.shipModuleSlotDictionary.Values)
			{
				if (this.selectedDragDestination == null && this.selectedShipPart.allowedSlots.Contains(shipModuleDragDestination.shipModuleSlotType))
				{
					shipModuleDragDestination.HighlightDestination();
				}
				else
				{
					shipModuleDragDestination.DeHiglightDestination();
				}
			}
		}

		// Token: 0x17000EB2 RID: 3762
		// (get) Token: 0x06004DB8 RID: 19896 RVA: 0x002146A0 File Offset: 0x002128A0
		public bool CanSaveCurrentDesign
		{
			get
			{
				return this.newShipTemplate.ValidTemplate && this.changesMadeToExistingClass;
			}
		}

		// Token: 0x06004DB9 RID: 19897 RVA: 0x002146B8 File Offset: 0x002128B8
		public void LoadShipTemplateIntoUI(TISpaceShipTemplate ship)
		{
			this.loadingExistingTemplate = true;
			this.hullSelectionDropdown.value = this.reverseHullDropdownValues[ship.hullTemplate.dataName];
			this.SetModuleInSlot(ship.driveTemplate, this.shipModuleSlotDictionary[ship.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.Drive)], false);
			this.SetModuleInSlot(ship.powerPlantTemplate, this.shipModuleSlotDictionary[ship.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.PowerPlant)], false);
			this.SetModuleInSlot(ship.radiatorTemplate, this.shipModuleSlotDictionary[ship.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.Radiator)], false);
			this.SetModuleInSlot(ship.noseArmorTemplate, this.shipModuleSlotDictionary[ship.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.NoseArmor)], false);
			this.SetModuleInSlot(ship.lateralArmorTemplate, this.shipModuleSlotDictionary[ship.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.LateralArmor)], false);
			this.SetModuleInSlot(ship.tailArmorTemplate, this.shipModuleSlotDictionary[ship.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.TailArmor)], false);
			this.newShipTemplate.moduleTemplateEntries = new List<ModuleDataTemplateEntry>();
			this.newShipTemplate.fireModeTemplateEntries = new List<FireModeDataTemplateEntry>(ship.fireModeTemplateEntries);
			foreach (ModuleDataEntry moduleDataEntry in ship.utilityModules)
			{
				Vector2Int slotPosition = ship.hullTemplate.shipModuleSlots[moduleDataEntry.slotIndex].slotPosition;
				this.SetModuleInSlot(moduleDataEntry.moduleTemplate, this.shipModuleSlotDictionary[slotPosition], false);
			}
			this.newShipTemplate.noseWeaponTemplateEntries = new List<ModuleDataTemplateEntry>();
			foreach (ModuleDataEntry moduleDataEntry2 in ship.noseWeapons)
			{
				Vector2Int slotPosition2 = ship.hullTemplate.shipModuleSlots[moduleDataEntry2.slotIndex].slotPosition;
				this.SetModuleInSlot(moduleDataEntry2.moduleTemplate, this.shipModuleSlotDictionary[slotPosition2], false);
			}
			this.newShipTemplate.hullWeaponTemplateEntries = new List<ModuleDataTemplateEntry>();
			foreach (ModuleDataEntry moduleDataEntry3 in ship.hullWeapons)
			{
				Vector2Int slotPosition3 = ship.hullTemplate.shipModuleSlots[moduleDataEntry3.slotIndex].slotPosition;
				this.SetModuleInSlot(moduleDataEntry3.moduleTemplate, this.shipModuleSlotDictionary[slotPosition3], false);
			}
			this.newShipTemplate.propellantTanks = ship.propellantTanks;
			this.newShipTemplate.noseArmor.armorValue = ship.noseArmorValue;
			this.newShipTemplate.lateralArmor.armorValue = ship.lateralArmorValue;
			this.newShipTemplate.tailArmor.armorValue = ship.tailArmorValue;
			this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.Propellant)].UpdateSpinnerValue(this.newShipTemplate.propellantTanks);
			this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.NoseArmor)].UpdateSpinnerValue(this.newShipTemplate.noseArmor.armorValue);
			this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.LateralArmor)].UpdateSpinnerValue(this.newShipTemplate.lateralArmor.armorValue);
			this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.TailArmor)].UpdateSpinnerValue(this.newShipTemplate.tailArmor.armorValue);
			this.shipModuleSlotDictionary[this.newShipTemplate.hullTemplate.GetUniqueSlotCoordinates(ShipModuleSlotType.Drive)].UpdateSpinnerValue(this.newShipTemplate.thrusterCount);
			this.newShipTemplate.role = ship.role;
			this.roleSelectionDropdown.value = this.roleOptions[ship.role];
			this.designerAutoDesignButton.interactable = this.roleSelectionDropdown.value != 0;
			this.SetAltHull(ship.hullAppearanceIndex);
			this.UpdateShipDesignDataPanelAndImage(true, false, false);
			this.PopulateRoleDropdown(ship.role);
			this.loadingExistingTemplate = false;
			this.FilterAvailableShipModules();
		}

		// Token: 0x06004DBA RID: 19898 RVA: 0x00214B14 File Offset: 0x00212D14
		public void LoadExistingShipTemplate(TISpaceShipTemplate ship)
		{
			this.oldShipTemplate = ship;
			this.refitting = true;
			this.validRefitNotificationObject.SetActive(true);
			string refitSuffix = TISpaceShipTemplate.GetRefitSuffix(ship.refitIteration + 2);
			string text = ship.displayName;
			if (ship.displayName.Contains(Loc.T("UI.Fleets.RefitIterationSuffix")))
			{
				int iteration = 0;
				(from o in ship.displayName.Split(new char[] { ' ' })
					where int.TryParse(o, out iteration)
					select o).FirstOrDefault<string>();
				string refitSuffix2 = TISpaceShipTemplate.GetRefitSuffix(iteration);
				text = text.Replace(refitSuffix2, refitSuffix);
			}
			else
			{
				text = new StringBuilder(text).Append(refitSuffix).ToString();
			}
			if (TISpaceShipTemplate.illegalShipClassNames.Contains(text))
			{
				this.nameAttempts = 0;
				text = this.GetNextRefitName(text);
			}
			this.ResetShip(ship.hullName, text, ShipRole.NoRole);
			this.SetupDesignerLayout();
			this.PopulateClassSelectionDropdown(ship.hullTemplate);
			this.LoadShipTemplateIntoUI(ship);
			this.hullSelectionDropdown.captionText.SetText(ship.hullTemplate.displayName);
			this.hullSelectionDropdown.interactable = false;
			this.changesMadeToExistingClass = false;
			this.designerSaveDesignButton.interactable = false;
			this.shipDesignInProgress = true;
		}

		// Token: 0x06004DBB RID: 19899 RVA: 0x00214C50 File Offset: 0x00212E50
		public void RefitExistingShipTemplate(TISpaceShipTemplate ship)
		{
			this.ResetShip(ship.hullName, ship.displayName, ShipRole.NoRole);
			this.SetupDesignerLayout();
			this.LoadShipTemplateIntoUI(ship);
			this.hullSelectionDropdown.captionText.SetText(ship.hullTemplate.displayName);
			this.hullSelectionDropdown.interactable = false;
			this.changesMadeToExistingClass = false;
			this.designerSaveDesignButton.interactable = false;
			this.UpdateShipDesignDataPanelAndImage(true, true, false);
			this.roleSelectionDropdown.captionText.SetText(Loc.T(new StringBuilder("UI.Fleets.").Append(ship.role.ToString()).ToString()));
			this.oldShipTemplate = ship;
			this.shipDesignInProgress = true;
		}

		// Token: 0x17000EB3 RID: 3763
		// (get) Token: 0x06004DBC RID: 19900 RVA: 0x00214D08 File Offset: 0x00212F08
		private TransferPlanner TransferPlanner
		{
			get
			{
				return IntelScreenController.Singleton.TransferPlanner;
			}
		}

		// Token: 0x06004DBD RID: 19901 RVA: 0x00214D14 File Offset: 0x00212F14
		public void UpdateTransferInfo()
		{
			TransferPlannerLocationButton originButton = this.TransferPlanner.originButton;
			TransferPlannerLocationButton destinationButton = this.TransferPlanner.destinationButton;
			if (originButton.SelectedLocation == null || destinationButton.SelectedLocation == null || this.TransferPlanner.thrustProfileTool.transferResult == null)
			{
				this.TransferButtonText.text = Loc.T("UI.Fleets.Designer.Transfer.SetTransfer");
				this.TransferPlanText.text = Loc.T("UI.Fleets.Designer.Transfer.NoTransfer");
				this.TransferDurationText.text = "";
				this.TransferDurationText.transform.parent.gameObject.SetActive(false);
				return;
			}
			this.TransferPlanText.text = Loc.T("UI.Fleets.Designer.Transfer.ToFrom", new object[]
			{
				originButton.SelectedLocation.selfState().GetDisplayName(base.activePlayer),
				destinationButton.SelectedLocation.selfState().GetDisplayName(base.activePlayer)
			});
			if (this.newShipTemplate != null)
			{
				this.TransferDurationText.transform.parent.gameObject.SetActive(true);
				if (this.newShipTemplate.baseCruiseAcceleration_gs(true) > 0f)
				{
					this.UpdateTransferPlannerParameters();
					if (this.TransferPlanner.thrustProfileTool.transferResult != null && this.TransferPlanner.thrustProfileTool.transferResult.Result != TransferResult.Outcome.Success)
					{
						double num;
						if (this.TransferPlanner.thrustProfileTool.transferResult.TryGetMinimumDVneeded_mps(out num))
						{
							double num2 = Mathd.Ceil(num / 1000.0);
							this.TransferDurationText.text = Loc.T("UI.Fleets.Designer.Transfer.NeedDV", new object[] { num2.ToString("N0") });
							return;
						}
						double num3;
						if (this.TransferPlanner.thrustProfileTool.transferResult.TryGetMinimumAccelerationNeeded(out num3, (double)this.newShipTemplate.baseCruiseAcceleration_mps2(true)))
						{
							double num4 = Mathd.Ceil(num3 * 100000.0 / 9.806650161743164) / 100.0;
							this.TransferDurationText.text = Loc.T("UI.Fleets.Designer.Transfer.NeedAccel", new object[] { num4.ToString("N2") });
							return;
						}
						this.TransferDurationText.text = Loc.T("UI.Fleets.Designer.Transfer.NeedDV", new object[] { double.PositiveInfinity.ToString("N0") });
						return;
					}
					else if (this.TransferPlanner.thrustProfileTool.transferResult != null)
					{
						this.TransferDurationText.text = ThrustProfileTool.DigestibleTimeStr(this.TransferPlanner.thrustProfileTool.CurrentTrajectory.duration);
						return;
					}
				}
				else
				{
					this.TransferDurationText.text = Loc.T("UI.Fleets.Designer.Transfer.NoAcceleration");
				}
			}
		}

		// Token: 0x06004DBE RID: 19902 RVA: 0x00214FC8 File Offset: 0x002131C8
		public void UpdateTransferPlannerParameters()
		{
			if (this.newShipTemplate == null)
			{
				return;
			}
			this.TransferPlanner.accelerationInputField.text = (this.newShipTemplate.baseCruiseAcceleration_gs(true) * 1000f).ToString("N1");
			this.TransferPlanner.dvInputField.text = this.newShipTemplate.baseCruiseDeltaV_kps(true).ToString("N1");
		}

		// Token: 0x06004DBF RID: 19903 RVA: 0x00215038 File Offset: 0x00213238
		public void OnTransferButtonClicked()
		{
			this.UpdateTransferPlannerParameters();
			GeneralControlsController.Singleton.Intel();
			IntelScreenController.Singleton.transferTab.paneManager.Toggle(IntelScreenController.Singleton.transferTab);
			TransferPlanner transferPlanner = this.TransferPlanner;
			transferPlanner.OnNextClose = (Action)Delegate.Combine(transferPlanner.OnNextClose, new Action(delegate
			{
				FleetsScreenController.gotoDesigner = true;
				GeneralControlsController.Singleton.Fleets();
				this.UpdateTransferPlannerParameters();
				this.UpdateTransferInfo();
			}));
		}

		// Token: 0x06004DC0 RID: 19904 RVA: 0x0021509C File Offset: 0x0021329C
		public ShipModuleDragDestination GetBestDropDestinationForModule(TIShipPartTemplate module)
		{
			if (module == null)
			{
				return null;
			}
			List<ShipModuleDragDestination> list = new List<ShipModuleDragDestination>();
			foreach (ShipModuleDragDestination shipModuleDragDestination in this.shipModuleSlotDictionary.Values)
			{
				Vector2Int vector2Int;
				if (shipModuleDragDestination.LegalModuleForSlot(module, false, out vector2Int) && !shipModuleDragDestination.blocked && this.newShipTemplate.ValidPartForDesign(module))
				{
					list.Add(shipModuleDragDestination);
				}
			}
			if (list.Count > 0)
			{
				list = (from x in list
					orderby x.empty descending, module.allowedSlots[0] == x.shipModuleSlotType descending
					select x).ToList<ShipModuleDragDestination>();
				return list[0];
			}
			return null;
		}

		// Token: 0x06004DC1 RID: 19905 RVA: 0x0021518C File Offset: 0x0021338C
		public ShipModuleDragDestination FindModuleLocation(TIShipPartTemplate module)
		{
			if (module == null)
			{
				return null;
			}
			foreach (ShipModuleDragDestination shipModuleDragDestination in this.shipModuleSlotDictionary.Values)
			{
				if (shipModuleDragDestination.currentPart != null && shipModuleDragDestination.currentPart.dataName == module.dataName)
				{
					return shipModuleDragDestination;
				}
			}
			return null;
		}

		// Token: 0x06004DC2 RID: 19906 RVA: 0x0021520C File Offset: 0x0021340C
		public void OnDropModuleInSlot(ShipModuleDragDestination dropDestination)
		{
			DragItem currentItem = DragManager.currentItem;
			if (currentItem == null)
			{
				return;
			}
			TIShipPartTemplate moduleTemplate = ((ShipModuleListItem)currentItem).GetModuleTemplate();
			Vector2Int vector2Int;
			if (dropDestination.LegalModuleForSlot(moduleTemplate, true, out vector2Int))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_DropModuleInShipDesignSlot", false, false);
				this.SetModuleInSlot(moduleTemplate, this.shipModuleSlotDictionary[vector2Int], true);
				this.SetSelectedDragDestination(dropDestination);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			if (dropDestination.IsArmor)
			{
				dropDestination.UpdateSpinnerValue(-1);
			}
		}

		// Token: 0x06004DC3 RID: 19907 RVA: 0x00215288 File Offset: 0x00213488
		private void PopulateClassSelectionDropdown(TIShipHullTemplate forceTemplate = null)
		{
			List<TIShipHullTemplate> list = FleetsScreenController.AllowedShipHulls(base.activePlayer, false);
			this.hullDropdownValues = new Dictionary<int, TIShipHullTemplate>();
			this.reverseHullDropdownValues = new Dictionary<string, int>();
			this.hullSelectionDropdown.ClearOptions();
			int num = 0;
			int num2 = 0;
			foreach (TIShipHullTemplate tishipHullTemplate in list)
			{
				this.hullDropdownValues.Add(num2, tishipHullTemplate);
				this.reverseHullDropdownValues.Add(tishipHullTemplate.dataName, num2);
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
				{
					text = tishipHullTemplate.displayName
				};
				this.hullSelectionDropdown.options.Add(optionData);
				if (this.shipDesignInProgress && tishipHullTemplate.dataName == this.newShipTemplate.hullTemplate.dataName)
				{
					num = num2;
				}
				if (forceTemplate != null && tishipHullTemplate.dataName == forceTemplate.dataName)
				{
					num = num2;
				}
				num2++;
			}
			if (list.Count > 0)
			{
				if ((forceTemplate == null || !list.Contains(forceTemplate)) && !this.shipDesignInProgress)
				{
					this.hullSelectionDropdown.captionText.SetText(list[0].displayName);
				}
				else
				{
					this.hullSelectionDropdown.SetValueWithoutNotify(num);
					this.hullSelectionDropdown.captionText.SetText((forceTemplate != null) ? forceTemplate.displayName : this.newShipTemplate.hullTemplate.displayName);
				}
				this.hullSelectionDropdown.interactable = true;
			}
		}

		// Token: 0x06004DC4 RID: 19908 RVA: 0x00215414 File Offset: 0x00213614
		public void OnClassSelectionDropdownChanged()
		{
			if (!this.refitting)
			{
				this.ResetShip(this.hullDropdownValues[this.hullSelectionDropdown.value].dataName, "", this.newShipTemplate.role);
				this.OnEndEditClassName();
			}
			this.SetupDesignerLayout();
			this.FilterAvailableShipModules();
			this.GetMaxHullIndex(this.newShipTemplate);
		}

		// Token: 0x06004DC5 RID: 19909 RVA: 0x00215478 File Offset: 0x00213678
		private void GetMaxHullIndex(TISpaceShipTemplate ship)
		{
			this.maxHullIndex = 0;
			for (int i = 1; i < ship.hullTemplate.modelResource.Length; i++)
			{
				if (ship.hullTemplate.modelResource[i] != null && !string.IsNullOrEmpty(ship.hullTemplate.modelResource[i]) && ((i != 2 && i != 3) || AssetBundleManager.AreDLCBundlesLoaded(1)))
				{
					this.maxHullIndex++;
				}
			}
		}

		// Token: 0x06004DC6 RID: 19910 RVA: 0x002154E8 File Offset: 0x002136E8
		public void OnCycleAltHull(int index)
		{
			this.selectedHullIndex += index;
			if (index == 1)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			else if (index == -1)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			}
			if ((this.selectedHullIndex == 2 || this.selectedHullIndex == 3) && !AssetBundleManager.AreDLCBundlesLoaded(1))
			{
				this.selectedHullIndex += 4 - this.selectedHullIndex;
			}
			if (this.selectedHullIndex > this.maxHullIndex)
			{
				this.selectedHullIndex = 0;
			}
			if (this.selectedHullIndex < 0)
			{
				this.selectedHullIndex = this.maxHullIndex;
			}
			this.newShipTemplate.hullAppearanceIndex = this.selectedHullIndex;
			this.UpdateShipDesignDataPanelAndImage(true, false, false);
		}

		// Token: 0x06004DC7 RID: 19911 RVA: 0x00215598 File Offset: 0x00213798
		public void SetAltHull(int index)
		{
			this.selectedHullIndex = index;
			this.newShipTemplate.hullAppearanceIndex = index;
			this.UpdateShipDesignDataPanelAndImage(true, false, false);
		}

		// Token: 0x06004DC8 RID: 19912 RVA: 0x002155B8 File Offset: 0x002137B8
		private void PopulateRoleDropdown(ShipRole forceRole)
		{
			this.roleOptions.Clear();
			this.reverseRoleOptions.Clear();
			this.roleSelectionDropdown.ClearOptions();
			int num = 0;
			foreach (ShipRole shipRole in Enums.ShipRoles.Except<ShipRole>(this.hideShipRoles))
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
				{
					text = Loc.T(new StringBuilder("UI.Fleets.").Append(shipRole.ToString()).ToString())
				};
				this.roleSelectionDropdown.options.Add(optionData);
				this.roleOptions.Add(shipRole, num);
				this.reverseRoleOptions.Add(num, shipRole);
				num++;
			}
			this.UpdateRoleSelection(forceRole);
		}

		// Token: 0x06004DC9 RID: 19913 RVA: 0x00215694 File Offset: 0x00213894
		public string SetAutomateButtonTip()
		{
			return Loc.T("UI.Fleets.AssignRoleTip");
		}

		// Token: 0x06004DCA RID: 19914 RVA: 0x002156A0 File Offset: 0x002138A0
		public string SetRoleTip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.newShipTemplate.roleStr).AppendLine();
			stringBuilder.AppendLine(this.newShipTemplate.roleDescription).AppendLine();
			return stringBuilder.ToString();
		}

		// Token: 0x06004DCB RID: 19915 RVA: 0x002156DC File Offset: 0x002138DC
		public void OnAutomateRoleClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.roleSelectionDropdown.value = this.roleOptions[this.newShipTemplate.AssignRole()];
			this.designerSaveDesignButton.interactable = this.CanSaveCurrentDesign;
			this.designerAutoDesignButton.interactable = this.roleSelectionDropdown.value != 0;
		}

		// Token: 0x06004DCC RID: 19916 RVA: 0x00215740 File Offset: 0x00213940
		private void UpdateRoleSelection(ShipRole role)
		{
			this.roleSelectionDropdown.value = this.roleOptions[role];
			this.roleSelectionDropdown.captionText.SetText(this.newShipTemplate.roleStr);
			this.designerSaveDesignButton.interactable = this.CanSaveCurrentDesign;
			this.designerAutoDesignButton.interactable = role > ShipRole.NoRole;
		}

		// Token: 0x06004DCD RID: 19917 RVA: 0x002157A0 File Offset: 0x002139A0
		public void OnRoleSelectionDropdownChanged()
		{
			ShipRole shipRole = this.reverseRoleOptions[this.roleSelectionDropdown.value];
			bool flag = this.previousRole != shipRole;
			this.previousRole = shipRole;
			this.newShipTemplate.role = shipRole;
			this.roleSelectionDropdown.captionText.SetText(this.newShipTemplate.roleStr);
			this.designerSaveDesignButton.interactable = this.CanSaveCurrentDesign;
			this.designerAutoDesignButton.interactable = this.roleSelectionDropdown.value != 0;
			this.UpdateShipDesignDataPanelAndImage(false, true, !flag || shipRole == ShipRole.NoRole);
		}

		// Token: 0x06004DCE RID: 19918 RVA: 0x0021583B File Offset: 0x00213A3B
		public void TextEntryMode_Enter()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x06004DCF RID: 19919 RVA: 0x00215842 File Offset: 0x00213A42
		public void TextEntryMode_End()
		{
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x06004DD0 RID: 19920 RVA: 0x0021584C File Offset: 0x00213A4C
		public void OnDesignerClassNameChanged()
		{
			string text = this.classNameInputField.text.Trim();
			if (TISpaceShipTemplate.illegalShipClassNames.Contains(text) || text == string.Empty)
			{
				this.classNameInputField.textComponent.color = TIUtilities.UIRedTextColor;
				return;
			}
			this.classNameInputField.textComponent.color = TIUtilities.UITextColor;
		}

		// Token: 0x06004DD1 RID: 19921 RVA: 0x002158B0 File Offset: 0x00213AB0
		public void OnEndEditClassName()
		{
			string text = this.classNameInputField.text.Trim();
			if (TISpaceShipTemplate.illegalShipClassNames.Contains(text))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			if (text != string.Empty)
			{
				this.newShipTemplate.SetDisplayName(text);
				this.fullShipClassName.SetText(this.newShipTemplate.fullClassName);
				this.designerShipDataClassName.SetText(this.newShipTemplate.fullClassName);
			}
		}

		// Token: 0x06004DD2 RID: 19922 RVA: 0x00215930 File Offset: 0x00213B30
		private string GetNextRefitName(string name)
		{
			this.nameAttempts++;
			if (this.nameAttempts > 1000)
			{
				Debug.LogWarning("Failed to get refit name for " + name + ", generating new name.");
				return TemplateManager.GenerateDataName("playerShipTemplate");
			}
			int iteration = 0;
			string text = name;
			string text2 = Loc.T("UI.Fleets.RefitIterationSuffix");
			if (name.Contains(text2))
			{
				text = name.Split(new string[] { text2 }, StringSplitOptions.None)[1].TrimStart(Array.Empty<char>());
				text = Regex.Replace(text, "\\d", "");
			}
			(from o in text.Split(new char[] { ' ' })
				where int.TryParse(o, out iteration)
				select o).FirstOrDefault<string>();
			string refitSuffix = TISpaceShipTemplate.GetRefitSuffix(iteration);
			string refitSuffix2 = TISpaceShipTemplate.GetRefitSuffix(iteration + 1);
			string text3 = name.Replace(refitSuffix, refitSuffix2);
			if (text3 == name)
			{
				if (char.IsNumber(text3.Last<char>()))
				{
					text3 += "A";
				}
				else
				{
					char c = text3.Last<char>() + '\u0001';
					text3 = text3.Remove(text3.Length - 1) + c.ToString();
				}
			}
			if (TISpaceShipTemplate.illegalShipClassNames.Contains(text3))
			{
				Log.Info(text3, Array.Empty<object>());
				text3 = this.GetNextRefitName(text3);
			}
			return text3;
		}

		// Token: 0x06004DD3 RID: 19923 RVA: 0x00215A88 File Offset: 0x00213C88
		public void OnAutodesignSelected()
		{
			if (this.refitting)
			{
				TISpaceShipTemplate tispaceShipTemplate = base.activePlayer.DesignRefit(this.oldShipTemplate);
				if (tispaceShipTemplate != null)
				{
					this.ResetDesigner(tispaceShipTemplate.hullName, "", tispaceShipTemplate.role);
					this.LoadShipTemplateIntoUI(tispaceShipTemplate);
					this.changesMadeToExistingClass = true;
					this.designerSaveDesignButton.interactable = this.CanSaveCurrentDesign;
					this.refitting = true;
					this.validRefitNotificationObject.SetActive(true);
				}
				return;
			}
			if (this.newShipTemplate.role == ShipRole.NoRole)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			TISpaceShipTemplate tispaceShipTemplate2;
			if (base.activePlayer.DesignShip(true, this.newShipTemplate.role, out tispaceShipTemplate2, base.activePlayer.DesiredStrategicRange_AU(), base.activePlayer.UnlockedExotics && base.activePlayer.GetCurrentResourceAmount(FactionResource.Exotics) > 0f, base.activePlayer.UnlockedAntimatter && base.activePlayer.GetDailyIncome(FactionResource.Antimatter, false, false) > 0f, this.newShipTemplate.hullTemplate, null, false, null, null, float.PositiveInfinity, float.PositiveInfinity) == TIFactionState.ShipDesignerOutcome.Success)
			{
				this.ResetDesigner(tispaceShipTemplate2.hullName, "", tispaceShipTemplate2.role);
				this.ResetShip(tispaceShipTemplate2.hullName, tispaceShipTemplate2.displayName, ShipRole.NoRole);
				this.LoadShipTemplateIntoUI(tispaceShipTemplate2);
				this.changesMadeToExistingClass = true;
				this.designerSaveDesignButton.interactable = this.CanSaveCurrentDesign;
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004DD4 RID: 19924 RVA: 0x00215C08 File Offset: 0x00213E08
		public void OnShowObsoletePartsToggle()
		{
			this.partsSortShowObsolete = this.ShowObsoletePartsToggle.isOn;
			base.activePlayer.SetDesignerShowObsoletePartsSetting(this.ShowObsoletePartsToggle.isOn);
			if (GameControl.loadcycle100)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				this.FilterAvailableShipModules();
			}
		}

		// Token: 0x06004DD5 RID: 19925 RVA: 0x00215C58 File Offset: 0x00213E58
		public void OnPartObsoleteToggle(TIShipPartTemplate template, bool isOn)
		{
			List<ShipModuleListItem> list = this.shipModuleListItems.FindAll((ShipModuleListItem o) => o.GetModuleTemplate().dataName == template.dataName);
			List<ShipModuleListItem> list2 = this.shipModuleListItemsB.FindAll((ShipModuleListItem o) => o.GetModuleTemplate().dataName == template.dataName);
			foreach (ShipModuleListItem shipModuleListItem in list)
			{
				shipModuleListItem.obsoleteToggle.SetIsOnWithoutNotify(isOn);
				shipModuleListItem.UpdateIcon();
			}
			foreach (ShipModuleListItem shipModuleListItem2 in list2)
			{
				shipModuleListItem2.obsoleteToggle.SetIsOnWithoutNotify(isOn);
				shipModuleListItem2.UpdateIcon();
			}
			base.activePlayer.playerControl.StartAction(new HideShipPartAction(base.activePlayer, template, isOn));
			if (!this.partsSortShowObsolete)
			{
				this.FilterAvailableShipModules();
			}
			this.UpdateModuleObsoleteToggles();
		}

		// Token: 0x06004DD6 RID: 19926 RVA: 0x00215D68 File Offset: 0x00213F68
		public string DesignerMassBreakdown(TISpaceShipTemplate ship)
		{
			float num = Mathf.Max(1f, ship.wetMass_tons);
			StringBuilder stringBuilder = new StringBuilder().Append(Loc.T("UI.Fleets.WetMassTab.Description")).AppendLine().Append(Loc.T("UI.Fleets.HullMass", new object[]
			{
				TIUtilities.FormatSmallNumber(ship.hullTemplate.buildMass_tons(0f, 0f, 0f, 0f, false), 7, 0, true, false),
				(ship.hullTemplate.buildMass_tons(0f, 0f, 0f, 0f, false) / num).ToPercent("P0")
			}))
				.AppendLine();
			TIDriveTemplate driveTemplate = ship.driveTemplate;
			float num2 = ((driveTemplate != null) ? driveTemplate.buildMass_tons(0f, 0f, 0f, 0f, false) : 0f);
			if (num2 > 0f)
			{
				stringBuilder.Append(Loc.T("UI.Fleets.DriveMass", new object[]
				{
					TIUtilities.FormatSmallNumber(num2, 7, 0, true, false),
					(num2 / num).ToPercent("P0")
				})).AppendLine();
			}
			stringBuilder.Append(Loc.T("UI.Fleets.PowerPlantMass", new object[]
			{
				TIUtilities.FormatSmallNumber(ship.powerPlantMass_tons, 7, 0, true, false),
				(ship.powerPlantMass_tons / num).ToPercent("P0")
			})).AppendLine().Append(Loc.T("UI.Fleets.RadiatorMass", new object[]
			{
				TIUtilities.FormatSmallNumber(ship.radiatorMass_tons, 7, 0, true, false),
				(ship.radiatorMass_tons / num).ToPercent("P0")
			}))
				.AppendLine()
				.Append(Loc.T("UI.Fleets.BatteryMass", new object[]
				{
					TIUtilities.FormatSmallNumber(ship.allBatteriesMass_tons, 7, 0, true, false),
					(ship.allBatteriesMass_tons / num).ToPercent("P0")
				}))
				.AppendLine()
				.Append(Loc.T("UI.Fleets.NoseArmorMass", new object[]
				{
					TIUtilities.FormatSmallNumber(ship.noseArmorMass_tons, 7, 0, true, false),
					(ship.noseArmorMass_tons / num).ToPercent("P0")
				}))
				.AppendLine()
				.Append(Loc.T("UI.Fleets.LateralArmorMass", new object[]
				{
					TIUtilities.FormatSmallNumber(ship.lateralArmorMass_tons, 7, 0, true, false),
					(ship.lateralArmorMass_tons / num).ToPercent("P0")
				}))
				.AppendLine()
				.Append(Loc.T("UI.Fleets.TailArmorMass", new object[]
				{
					TIUtilities.FormatSmallNumber(ship.tailArmorMass_tons, 7, 0, true, false),
					(ship.tailArmorMass_tons / num).ToPercent("P0")
				}))
				.AppendLine()
				.Append(Loc.T("UI.Fleets.PropellantMass", new object[]
				{
					TIUtilities.FormatSmallNumber(ship.propellantMass_tons, 7, 0, true, false),
					(ship.propellantMass_tons / num).ToPercent("P0")
				}))
				.AppendLine()
				.Append(Loc.T("UI.Fleets.CrewMass", new object[]
				{
					TIUtilities.FormatSmallNumber(ship.crewMass_tons, 7, 0, true, false),
					(ship.crewMass_tons / num).ToPercent("P0")
				}))
				.AppendLine();
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in ship.allWeaponTemplates)
			{
				float num3 = tishipWeaponTemplate.buildMass_tons(ship.magazineModuleMultiplier, 0f, 0f, 0f, false);
				stringBuilder.Append(Loc.T("UI.Fleets.ModuleMass", new object[]
				{
					tishipWeaponTemplate.displayName,
					TIUtilities.FormatSmallNumber(num3, 7, 0, true, false),
					(num3 / num).ToPercent("P0")
				})).AppendLine();
			}
			foreach (TIShipModuleTemplate tishipModuleTemplate in ship.utilitySlotModuleTemplates)
			{
				if (!tishipModuleTemplate.isBattery)
				{
					stringBuilder.Append(Loc.T("UI.Fleets.ModuleMass", new object[]
					{
						tishipModuleTemplate.displayName,
						TIUtilities.FormatBigOrSmallNumber(tishipModuleTemplate.buildMass_tons(0f, 0f, 0f, 0f, false), 1, 7, 0, false, false),
						(tishipModuleTemplate.buildMass_tons(0f, 0f, 0f, 0f, false) / num).ToPercent("P0")
					})).AppendLine();
				}
			}
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06004DD7 RID: 19927 RVA: 0x00216204 File Offset: 0x00214404
		public string BuildMassToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.WetMassTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DD8 RID: 19928 RVA: 0x00216220 File Offset: 0x00214420
		public string BuildCrewToolTip()
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Fleets.CrewTab.Description"));
			stringBuilder.AppendLine().AppendLine(Loc.T("UI.Fleets.DamConCrew.Description"));
			stringBuilder.AppendLine().AppendLine(Loc.T("UI.Fleets.DamConCrew", new object[] { this.newShipTemplate.damConCrewBillets }));
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x06004DD9 RID: 19929 RVA: 0x0021628D File Offset: 0x0021448D
		public string BuildCruiseAccelerationToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.CruiseAccelerationTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DDA RID: 19930 RVA: 0x002162A8 File Offset: 0x002144A8
		public string BuildCombatAccelerationToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.CombatAccelerationTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DDB RID: 19931 RVA: 0x002162C3 File Offset: 0x002144C3
		public string BuildCruiseDeltaVToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.CruiseDeltaVTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DDC RID: 19932 RVA: 0x002162DE File Offset: 0x002144DE
		public string BuildTurnRateToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.TurnRateTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DDD RID: 19933 RVA: 0x002162F9 File Offset: 0x002144F9
		public string BuildHeatSinkCapacityToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.HeatSinkCapacityTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DDE RID: 19934 RVA: 0x00216314 File Offset: 0x00214514
		public string BuildBatteryCapactiyToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.BatteryCapacityTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DDF RID: 19935 RVA: 0x0021632F File Offset: 0x0021452F
		public string BuildConstructionCostToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.ConstructionCostTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DE0 RID: 19936 RVA: 0x0021634A File Offset: 0x0021454A
		public string BuildConstructionTimeToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.ConstructionTimeTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DE1 RID: 19937 RVA: 0x00216365 File Offset: 0x00214565
		public string BuildSupportToolTip()
		{
			return new StringBuilder(Loc.T("UI.Fleets.MaintenanceCostTab.Description")).ToString().Trim();
		}

		// Token: 0x06004DE2 RID: 19938 RVA: 0x00216380 File Offset: 0x00214580
		public string HullConstructionTimeBreakdown(TIShipHullTemplate hullTemplate, bool symbol)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (symbol)
			{
				stringBuilder.Append(TemplateManager.global.habShipyardPresentInlineSpritePath);
			}
			stringBuilder.Append(hullTemplate.constructionTime_Days(1, base.activePlayer).ToString("N0"));
			IEnumerable<TIHabModuleTemplate> enumerable = from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
				where x.FactionCanBuild(base.activePlayer)
				select x;
			if (enumerable.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate y) => y.tier == 2) != null)
			{
				stringBuilder.Append(Loc.T("UI.Fleets.ConstructionTimeLine", new object[] { hullTemplate.constructionTime_Days(2, base.activePlayer).ToString("N0") }));
			}
			if (enumerable.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate y) => y.tier == 3) != null)
			{
				stringBuilder.Append(Loc.T("UI.Fleets.ConstructionTimeLine", new object[] { hullTemplate.constructionTime_Days(3, base.activePlayer).ToString("N0") }));
			}
			return Loc.T("UI.Fleets.ConstructionTime", new object[] { stringBuilder.ToString() });
		}

		// Token: 0x06004DE3 RID: 19939 RVA: 0x002164B8 File Offset: 0x002146B8
		public void UpdateShipDesignDataPanelAndImage(bool updateImage, bool updateSpaceBackground = true, bool suppressSCVUpdate = false)
		{
			if (this.newShipTemplate.ValidTemplate)
			{
				if (!suppressSCVUpdate && FleetsScreenController.lastSCVUpdateFrame != TIFrameCounter.FrameCount)
				{
					this.designerCombatScoreText.SetText(this.newShipTemplate.TemplateSpaceCombatValue(true, -1f, 0.6f, false).ToString("N0"));
					FleetsScreenController.lastSCVUpdateFrame = TIFrameCounter.FrameCount;
				}
			}
			else
			{
				this.designerCombatScoreText.SetText("0");
				FleetsScreenController.lastSCVUpdateFrame = -1;
			}
			this.designerWetMassText.SetText(Loc.T("UI.Fleets.Tons", new object[] { this.newShipTemplate.wetMass_tons.ToString("N0") }));
			this.designerCrewText.SetText(this.newShipTemplate.crewBillets.ToString("N0"));
			if (this.newShipTemplate.driveTemplate != null)
			{
				float num = this.newShipTemplate.baseCruiseAcceleration_gs(true);
				string text = FleetsScreenController.accelerationStr((double)num, false, false, false);
				if (num < 0.0005f)
				{
					text = TIUtilities.RedLine(text);
				}
				else if (num < 0.01f)
				{
					text = TIUtilities.YellowLine(text);
				}
				this.designerCruiseAccelerationText.SetText(text);
				this.designerCombatAccelerationText.SetText(FleetsScreenController.accelerationStr((double)this.newShipTemplate.baseCombatAcceleration_gs, true, false, false));
				float num2 = this.newShipTemplate.baseCruiseDeltaV_kps(true);
				string text2 = TIUtilities.FormatBigOrSmallNumber(num2, 1, 7, 0, false, false);
				if (num2 < 8f)
				{
					text2 = TIUtilities.RedLine(text2);
				}
				else if (num2 < 30f)
				{
					text2 = TIUtilities.YellowLine(text2);
				}
				this.designerCruiseDeltaVText.SetText(Loc.T("UI.Fleets.SingleDV", new object[] { text2 }));
			}
			else
			{
				this.designerCombatAccelerationText.SetText(Loc.T("UI.Fleets.NeedDrive"));
				this.designerCruiseAccelerationText.SetText(Loc.T("UI.Fleets.NeedDrive"));
				this.designerCruiseDeltaVText.SetText(Loc.T("UI.Fleets.NeedDrive"));
			}
			this.designerTurnRateText.SetText(Loc.T("UI.Fleets.Degs2", new object[] { TIUtilities.FormatSmallNumber(Mathf.Min(this.newShipTemplate.baseAngularAcceleration_degs2, this.newShipTemplate.maxAngularVelocity_degs), 7, 0, true, false) }));
			this.designerHeatSinkCapacity.SetText(Loc.T("UI.Fleets.GJ", new object[] { this.newShipTemplate.HeatCapacity_GJ(false).ToString("N0") }));
			this.designerBatteryCapacity.SetText(Loc.T("UI.Fleets.GJ", new object[] { this.newShipTemplate.BatteryCapacity_GJ(false).ToString("N0") }));
			TIResourcesCost tiresourcesCost = this.newShipTemplate.spaceResourceConstructionCost(false, null, true, false, false);
			this.designerConstructionCostText.SetText(tiresourcesCost.ToString("Relevant", false, false, null, false, FactionResource.None));
			this.designerMaintenanceCostText.SetText(Loc.T("UI.Fleets.MaintenanceCostValue", new object[]
			{
				TemplateManager.global.missionControlInlineSpritePath,
				this.newShipTemplate.hullTemplate.missionControl,
				TemplateManager.global.moneyInlineSpritePath,
				-this.newShipTemplate.GetMonthlyExpenses(FactionResource.Money)
			}));
			this.designerConstructionTimeText.SetText(this.HullConstructionTimeBreakdown(this.newShipTemplate.hullTemplate, false));
			this.designerMassBreakdownToolTipText.SetDelegate("BodyText", () => this.DesignerMassBreakdown(this.newShipTemplate));
			this.designerCrewToolTipText.SetDelegate("BodyText", () => this.BuildCrewToolTip());
			this.designerCruiseAccelToolTipText.SetDelegate("BodyText", () => this.BuildCruiseAccelerationToolTip());
			this.designerCombatAccelToolTipText.SetDelegate("BodyText", () => this.BuildCombatAccelerationToolTip());
			this.designerCruiseDeltaVToolTipText.SetDelegate("BodyText", () => this.BuildCruiseDeltaVToolTip());
			this.designerTurnRateToolTipText.SetDelegate("BodyText", () => this.BuildTurnRateToolTip());
			this.designerHeatSinkCapacityToolTipText.SetDelegate("BodyText", () => this.BuildHeatSinkCapacityToolTip());
			this.designerBatteryCapacityToolTipText.SetDelegate("BodyText", () => this.BuildBatteryCapactiyToolTip());
			this.designerConstructionCostToolTipText.SetDelegate("BodyText", () => this.BuildConstructionCostToolTip());
			this.designerConstructionTimeToolTipText.SetDelegate("BodyText", () => this.BuildConstructionTimeToolTip());
			this.designerSupportToolTipText.SetDelegate("BodyText", () => this.BuildSupportToolTip());
			if (updateImage)
			{
				this.UpdateConstructionCameraImage(this.newShipTemplate, updateSpaceBackground);
			}
			string text3;
			if (this.newShipTemplate.IsAValidRefitFor(this.oldShipTemplate, out text3, true))
			{
				this.refitTooltipText.SetText("BodyText", Loc.T("UI.Codex.codex_shipRefits0"));
				this.designerValidRefitText.SetText(Loc.T("UI.Fleets.ValidRefitDesign"));
			}
			else
			{
				this.refitTooltipText.SetText("BodyText", new StringBuilder(Loc.T("UI.Codex.codex_shipRefits0")).Append(text3).ToString());
				this.designerValidRefitText.SetText(TIUtilities.RedLine(Loc.T("UI.Fleets.InvalidRefitDesign")));
			}
			CombatantListItemController.SetNoseImage(this.newShipTemplate, this.designerShipDataClassNose);
			CombatantListItemController.SetMidImage(this.newShipTemplate, this.designerShipDataClassHull);
			CombatantListItemController.SetTailImage(this.newShipTemplate, this.designerShipDataClassTail);
			if (this.newShipTemplate.radiatorTemplate != null)
			{
				this.designerShipDataClassRadiator.gameObject.SetActive(true);
				CombatantListItemController.SetRadiatorImage(this.newShipTemplate, this.designerShipDataClassRadiator);
			}
			else
			{
				this.designerShipDataClassRadiator.gameObject.SetActive(false);
			}
			if (this.newShipTemplate.driveTemplate != null)
			{
				this.designerShipDataClassDrive.gameObject.SetActive(true);
				CombatantListItemController.SetDriveImage(this.newShipTemplate, this.designerShipDataClassDrive);
				return;
			}
			this.designerShipDataClassDrive.gameObject.SetActive(false);
		}

		// Token: 0x06004DE4 RID: 19940 RVA: 0x00216A8C File Offset: 0x00214C8C
		public void UpdateConstructionCameraImage(TISpaceShipTemplate template, bool updateSpaceBackground = true)
		{
			global::UnityEngine.Object.Destroy(this.shipVisObject, 0f);
			if (this.fleetSceneCameraInstance == null)
			{
				this.fleetSceneCameraInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.fleetCamera);
			}
			if (this.previewPosition == null)
			{
				this.previewPosition = this.fleetSceneCameraInstance.transform.Find("FleetScreenShipBuilderCameraPreviewPosition").gameObject;
			}
			foreach (object obj in this.previewPosition.transform)
			{
				Transform transform = (Transform)obj;
				transform.parent = null;
				global::UnityEngine.Object.Destroy(transform.gameObject);
			}
			this.shipVisObject = global::UnityEngine.Object.Instantiate<GameObject>(this.shipPrefab, this.previewPosition.transform, false);
			this.shipVisObject.transform.localPosition = Vector3.zero;
			this.shipVisObject.transform.SetLayer(10, true);
			this.shipVisObject.GetComponent<ShipVisController>().InitializeModelOnly(template);
			if (updateSpaceBackground)
			{
				this.shipImageSpaceBackground.localPosition = new Vector3(TIUtilities.RandomRange(0f, 350f), TIUtilities.RandomRange(-350f, 350f), 0f);
			}
			Transform child = this.shipVisObject.transform.GetChild(0);
			child.SetLayer(10, true);
			child.Rotate(Vector3.left, 90f);
			child.localPosition = new Vector3(0f, 0f, 100f);
			float num = -template.hullTemplate.length_m / 1500f + 0.275f;
			child.transform.localScale = Vector3.one * num;
		}

		// Token: 0x06004DE5 RID: 19941 RVA: 0x00216C50 File Offset: 0x00214E50
		public void UpdateIndivCameraImage(TISpaceShipState ship)
		{
			global::UnityEngine.Object.Destroy(this.indivShipVisObject, 0f);
			if (this.indivPreviewPosition == null)
			{
				this.indivPreviewPosition = this.individualShipCameraObject.transform.Find("SingleShipCameraPreviewPosition").gameObject;
			}
			foreach (object obj in this.indivPreviewPosition.transform)
			{
				Transform transform = (Transform)obj;
				transform.parent = null;
				global::UnityEngine.Object.Destroy(transform.gameObject);
			}
			this.indivShipVisObject = global::UnityEngine.Object.Instantiate<GameObject>(this.shipPrefab, this.indivPreviewPosition.transform, false);
			this.indivShipVisObject.transform.localPosition = Vector3.zero;
			this.indivShipVisObject.transform.SetLayer(10, true);
			this.indivShipVisObject.GetComponent<ShipVisController>().InitializeShipVisualizer(ship.template, ship, null, null, false);
			Transform child = this.indivShipVisObject.transform.GetChild(0);
			child.SetLayer(10, true);
			child.localPosition = new Vector3(0f, 0f, 100f);
			child.localRotation = Quaternion.Euler(-30f, 130f, 0f);
			float num = 0.739f + ship.hull.length_m * -0.00255f;
			string dataName = ship.template.hullTemplate.dataName;
			if (dataName != null)
			{
				if (!(dataName == "Lancer"))
				{
					if (!(dataName == "Dreadnought"))
					{
						if (!(dataName == "Titan"))
						{
							if (!(dataName == "AlienAssaultCarrier"))
							{
								if (!(dataName == "AlienDreadnought"))
								{
									if (dataName == "AlienMothership")
									{
										num = 0.043f;
									}
								}
								else
								{
									num = 0.14f;
								}
							}
							else
							{
								num = 0.14f;
							}
						}
						else
						{
							num = 0.11f;
						}
					}
					else
					{
						num = 0.12f;
					}
				}
				else
				{
					num = 0.16f;
				}
			}
			child.transform.localScale = Vector3.one * num;
			this.shipModelViewer.shipT = child;
		}

		// Token: 0x06004DE6 RID: 19942 RVA: 0x00216E7C File Offset: 0x0021507C
		private int WhichPowerUnit(float powerValue)
		{
			if (powerValue < 1f)
			{
				return 0;
			}
			if (powerValue < 1000f)
			{
				return 1;
			}
			return 2;
		}

		// Token: 0x06004DE7 RID: 19943 RVA: 0x00216E94 File Offset: 0x00215094
		public void UpdateModuleDataPanel(bool isSelected, TIShipPartTemplate partTemplate, bool prospective, ShipModuleSlotType slotType = ShipModuleSlotType.None)
		{
			if (this.comparingModules)
			{
				this.selectedModuleDataContainer.gameObject.SetActive(true);
				this.installedModuleDataContainer.gameObject.SetActive(true);
			}
			else
			{
				this.selectedModuleDataContainer.gameObject.SetActive(isSelected);
				this.installedModuleDataContainer.gameObject.SetActive(!isSelected);
			}
			if (isSelected)
			{
				this.currentlySelectedModule = partTemplate;
			}
			else
			{
				this.currentlyInstalledModule = partTemplate;
			}
			RectTransform rectTransform = this.selectedModuleDataContainer;
			Scrollbar scrollbar = this.selectedModuleScrollbar;
			Toggle toggle = this.selectedModuleObsoleteToggle;
			RectTransform rectTransform2 = this.selectedModuleHeaderContainer;
			Image image = this.selectedModuleDataIcon;
			TMP_Text tmp_Text = this.selectedModuleDataHeaderText;
			TMP_Text tmp_Text2 = this.selectedModuleSecondaryHeader;
			TMP_Text tmp_Text3 = this.selectedModulePreTableText;
			ListManagerBase listManagerBase = this.selectedModuleTableList;
			TMP_Text tmp_Text4 = this.selectedModulePostTableText;
			TIShipPartTemplate tishipPartTemplate = this.currentlySelectedModule;
			if (!isSelected)
			{
				RectTransform rectTransform3 = this.installedModuleDataContainer;
				scrollbar = this.installedModuleScrollbar;
				toggle = this.installedModuleObsoleteToggle;
				rectTransform2 = this.installedModuleHeaderContainer;
				image = this.installedModuleDataIcon;
				tmp_Text = this.installedModuleDataHeaderText;
				tmp_Text2 = this.installedModuleSecondaryHeader;
				tmp_Text3 = this.installedModulePreTableText;
				listManagerBase = this.installedModuleTableList;
				tmp_Text4 = this.installedModulePostTableText;
				tishipPartTemplate = this.currentlyInstalledModule;
			}
			if (isSelected)
			{
				if (this.currentlySelectedModule != null)
				{
					ShipModuleDragDestination bestDropDestinationForModule = this.GetBestDropDestinationForModule(this.currentlySelectedModule);
					this.installModuleButton.gameObject.SetActive(bestDropDestinationForModule != null);
				}
				else
				{
					this.installModuleButton.gameObject.SetActive(false);
				}
				this.selectedModuleDataButtonsContainer.SetActive(this.installModuleButton.gameObject.activeSelf);
			}
			else
			{
				this.installedFireModeButton.gameObject.SetActive(!prospective && this.selectedDragDestination.currentPart != null && this.selectedDragDestination.currentPart.isWeapon);
				this.installedDeleteModuleButton.gameObject.SetActive(!prospective && this.selectedDragDestination.currentPart != null);
				this.installedModuleDataButtonsContainer.SetActive(this.installedFireModeButton.gameObject.activeSelf || this.installedDeleteModuleButton.gameObject.activeSelf);
			}
			listManagerBase.gameObject.SetActive(tishipPartTemplate != null);
			tmp_Text4.transform.parent.gameObject.SetActive(tishipPartTemplate != null);
			if (tishipPartTemplate == null)
			{
				string text = "";
				if (slotType == ShipModuleSlotType.Propellant && this.newShipTemplate.driveTemplate != null && this.newShipTemplate.propellantTanksBuildCost(base.activePlayer).resourceCosts.Count > 0)
				{
					text = text + this.newShipTemplate.propellantTanksBuildCost(base.activePlayer).ToString("Relevant", false, false, null, false, FactionResource.None) + "\n";
				}
				tmp_Text.SetText(Loc.T(new StringBuilder("UI.Fleets.").Append(slotType.ToString()).ToString()));
				text += Loc.T(new StringBuilder("UI.Fleets.").Append(slotType.ToString()).Append(".Description").ToString());
				tmp_Text3.text = text.Trim();
				tmp_Text2.gameObject.SetActive(false);
				toggle.gameObject.SetActive(false);
				if (slotType == ShipModuleSlotType.None)
				{
					rectTransform2.gameObject.SetActive(false);
					image.gameObject.SetActive(false);
					if (isSelected)
					{
						this.selectedModuleDataDisplay = false;
					}
					else
					{
						this.installedModuleDataDisplay = false;
					}
				}
				else
				{
					rectTransform2.gameObject.SetActive(true);
					if (slotType == ShipModuleSlotType.Propellant)
					{
						image.gameObject.SetActive(false);
					}
					else
					{
						image.gameObject.SetActive(true);
						GameControl.assetLoader.LoadAssetForImageAssignment(ShipModuleDragDestination.EmptySlotIconName(slotType), image);
					}
					if (isSelected)
					{
						this.selectedModuleDataDisplay = true;
					}
					else
					{
						this.installedModuleDataDisplay = true;
					}
				}
			}
			else
			{
				if (isSelected)
				{
					this.selectedModuleDataDisplay = true;
				}
				else
				{
					this.installedModuleDataDisplay = true;
				}
				tmp_Text.SetText(tishipPartTemplate.displayName);
				rectTransform2.gameObject.SetActive(true);
				image.gameObject.SetActive(true);
				GameControl.assetLoader.LoadAssetForImageAssignment(tishipPartTemplate.iconResource, image);
				toggle.gameObject.SetActive(true);
				this.UpdateModuleObsoleteToggles();
				StringBuilder stringBuilder = new StringBuilder();
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder.Append(tishipPartTemplate.description).AppendLine().AppendLine();
				string[] array = tishipPartTemplate.GetDescriptionData(null, this.newShipTemplate, prospective, slotType, false).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				int i = 0;
				if (tishipPartTemplate.isWeapon)
				{
					tmp_Text2.SetText(array[i]);
					tmp_Text2.gameObject.SetActive(true);
					i++;
				}
				else
				{
					tmp_Text2.gameObject.SetActive(false);
				}
				bool flag = false;
				bool flag2 = false;
				while (i < array.Length)
				{
					bool flag3 = array[i].Contains(':') || array[i].Contains('：');
					if (flag && !flag3)
					{
						flag2 = true;
					}
					flag = flag3;
					if (flag2)
					{
						stringBuilder2.Append(array[i]).AppendLine().AppendLine();
						i++;
					}
					else if (flag)
					{
						string[] array2 = array[i].Split(new char[] { ':', '：' }, StringSplitOptions.RemoveEmptyEntries);
						string text2 = array2[0].Trim();
						text2 = Regex.Replace(text2, "</?align.*?>", "");
						text2 = Regex.Replace(text2, "</?line-height.*?>", "");
						string text3 = ((array2.Length >= 2) ? array2[1].Trim() : "");
						text3 = Regex.Replace(text3, "</?align.*?>", "");
						text3 = Regex.Replace(text3, "</?line-height.*?>", "");
						if (i < array.Length - 1 && text3 == "")
						{
							text3 += array[i + 1].Trim();
						}
						text3 = Regex.Replace(text3, "</?align.*?>", "");
						text3 = Regex.Replace(text3, "</?line-height.*?>", "");
						list.Add(text2);
						list2.Add(text3);
						i += 2;
					}
					else
					{
						stringBuilder.Append(array[i]).AppendLine().AppendLine();
						i++;
					}
				}
				if (tishipPartTemplate.Explosive())
				{
					stringBuilder2.Append(Loc.T("UI.Fleets.Explosive")).AppendLine().AppendLine();
				}
				stringBuilder2.Append(Loc.T("UI.Fleets.RightToPlace"));
				tmp_Text3.text = stringBuilder.ToString().Trim();
				listManagerBase.SetListSize<ShipModuleDataListItem>(list.Count, false, false);
				int num = 0;
				using (IEnumerator<object> enumerator = listManagerBase.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__507.<>p__0 == null)
						{
							FleetsScreenController.<>o__507.<>p__0 = CallSite<Func<CallSite, object, ShipModuleDataListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipModuleDataListItem), typeof(FleetsScreenController)));
						}
						FleetsScreenController.<>o__507.<>p__0.Target(FleetsScreenController.<>o__507.<>p__0, enumerator.Current).Init(this, list[num], list2[num]);
						num++;
					}
				}
				tmp_Text4.text = stringBuilder2.ToString().Trim();
				if (!isSelected && this.installedFireModeButton.gameObject.activeInHierarchy)
				{
					TIShipWeaponTemplate tishipWeaponTemplate = tishipPartTemplate as TIShipWeaponTemplate;
					int num2 = this.newShipTemplate.hullTemplate.slotIndex(this.newShipTemplate.hullTemplate.GetSlotByCoordinates(this.selectedDragDestination.SlotCoordinates));
					FireMode fireMode = this.newShipTemplate.GetFireModeDataEntryFromSlot(num2).fireMode;
					this.UpdateFireModeUI(tishipWeaponTemplate, fireMode, this.selectedDragDestination.cornerIcon);
				}
			}
			scrollbar.value = 1f;
			this.selectedModuleLayoutElement.flexibleHeight = (this.selectedModuleDataDisplay ? 5f : 3f);
			this.installedModuleLayoutElement.flexibleHeight = (this.installedModuleDataDisplay ? 5f : 3f);
		}

		// Token: 0x06004DE8 RID: 19944 RVA: 0x0021769C File Offset: 0x0021589C
		public void UpdateModuleObsoleteToggles()
		{
			if (this.currentlySelectedModule != null)
			{
				this.selectedModuleObsoleteToggle.SetIsOnWithoutNotify(base.activePlayer.obsoletedShipParts.Contains(this.currentlySelectedModule.dataName));
			}
			if (this.currentlyInstalledModule != null)
			{
				this.installedModuleObsoleteToggle.SetIsOnWithoutNotify(base.activePlayer.obsoletedShipParts.Contains(this.currentlyInstalledModule.dataName));
			}
		}

		// Token: 0x06004DE9 RID: 19945 RVA: 0x00217705 File Offset: 0x00215905
		public void OnSelectedCompareModulesToggle(bool toggleValue)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.SelectedCompareModulesChanged(toggleValue);
		}

		// Token: 0x06004DEA RID: 19946 RVA: 0x0021771C File Offset: 0x0021591C
		public void SelectedCompareModulesChanged(bool toggleValue)
		{
			this.comparingModules = toggleValue;
			this.installedModulesCompareToggle.SetIsOnWithoutNotify(toggleValue);
			if (this.comparingModules)
			{
				this.selectedModuleDataContainer.gameObject.SetActive(true);
				this.installedModuleDataContainer.gameObject.SetActive(true);
				return;
			}
			this.selectedModuleDataContainer.gameObject.SetActive(true);
			this.installedModuleDataContainer.gameObject.SetActive(false);
		}

		// Token: 0x06004DEB RID: 19947 RVA: 0x00217789 File Offset: 0x00215989
		public void OnInstalledCompareModulesToggle(bool toggleValue)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.InstalledCompareModulesChanged(toggleValue);
		}

		// Token: 0x06004DEC RID: 19948 RVA: 0x002177A0 File Offset: 0x002159A0
		public void InstalledCompareModulesChanged(bool toggleValue)
		{
			this.comparingModules = toggleValue;
			this.selectedModulesCompareToggle.SetIsOnWithoutNotify(toggleValue);
			if (this.comparingModules)
			{
				this.selectedModuleDataContainer.gameObject.SetActive(true);
				this.installedModuleDataContainer.gameObject.SetActive(true);
				return;
			}
			this.selectedModuleDataContainer.gameObject.SetActive(false);
			this.installedModuleDataContainer.gameObject.SetActive(true);
		}

		// Token: 0x06004DED RID: 19949 RVA: 0x0021780D File Offset: 0x00215A0D
		public void OnSelectedModuleObsoleteToggle(bool toggleValue)
		{
			if (this.currentlySelectedModule == null)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.OnPartObsoleteToggle(this.currentlySelectedModule, toggleValue);
		}

		// Token: 0x06004DEE RID: 19950 RVA: 0x00217831 File Offset: 0x00215A31
		public void OnInstalledModuleObsoleteToggle(bool toggleValue)
		{
			if (this.currentlyInstalledModule == null)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.OnPartObsoleteToggle(this.currentlyInstalledModule, toggleValue);
		}

		// Token: 0x06004DEF RID: 19951 RVA: 0x00217855 File Offset: 0x00215A55
		public void ClearSlot(Vector2Int slotCoordinates)
		{
			this.RemoveModuleFromSlot(slotCoordinates, true, false);
			this.changesMadeToExistingClass = true;
			this.designerSaveDesignButton.interactable = this.CanSaveCurrentDesign;
		}

		// Token: 0x06004DF0 RID: 19952 RVA: 0x00217878 File Offset: 0x00215A78
		public void OnClickInstallModuleButton()
		{
			if (this.currentlySelectedModule == null)
			{
				return;
			}
			ShipModuleDragDestination bestDropDestinationForModule = this.GetBestDropDestinationForModule(this.currentlySelectedModule);
			if (bestDropDestinationForModule != null)
			{
				this.SetModuleInSlot(this.currentlySelectedModule, bestDropDestinationForModule, true);
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_DropModuleInShipDesignSlot", false, false);
				this.SetSelectedDragDestination(bestDropDestinationForModule);
			}
		}

		// Token: 0x06004DF1 RID: 19953 RVA: 0x002178C8 File Offset: 0x00215AC8
		public void OnClickDeleteModuleButton()
		{
			if (this.selectedDragDestination == null)
			{
				this.selectedDragDestination = this.FindModuleLocation(this.currentlyInstalledModule);
				if (this.selectedDragDestination == null)
				{
					ShipModuleDragDestination bestDropDestinationForModule = this.GetBestDropDestinationForModule(this.currentlyInstalledModule);
					if (!(bestDropDestinationForModule != null))
					{
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
						return;
					}
					this.SetSelectedDragDestination(bestDropDestinationForModule);
				}
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.ClearSlot(this.selectedDragDestination.SlotCoordinates);
		}

		// Token: 0x06004DF2 RID: 19954 RVA: 0x0021794C File Offset: 0x00215B4C
		public void OnClickFireModeButton(bool leftClick)
		{
			if (this.selectedDragDestination == null)
			{
				this.selectedDragDestination = this.FindModuleLocation(this.currentlyInstalledModule);
				if (this.selectedDragDestination == null)
				{
					ShipModuleDragDestination bestDropDestinationForModule = this.GetBestDropDestinationForModule(this.currentlyInstalledModule);
					if (!(bestDropDestinationForModule != null))
					{
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
						return;
					}
					this.SetSelectedDragDestination(bestDropDestinationForModule);
				}
			}
			TIShipWeaponTemplate tishipWeaponTemplate = this.selectedDragDestination.currentPart as TIShipWeaponTemplate;
			if (tishipWeaponTemplate == null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			TISpaceShipTemplate tispaceShipTemplate = this.newShipTemplate;
			if (((tispaceShipTemplate != null) ? tispaceShipTemplate.hullTemplate : null) == null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			int num = this.newShipTemplate.hullTemplate.slotIndex(this.newShipTemplate.hullTemplate.GetSlotByCoordinates(this.selectedDragDestination.SlotCoordinates));
			List<FireMode> actualFireModes = tishipWeaponTemplate.GetActualFireModes(true);
			if (actualFireModes == null || actualFireModes.Count == 0)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			int num2 = actualFireModes.IndexOf(this.newShipTemplate.GetFireModeDataEntryFromSlot(num).fireMode);
			if (leftClick)
			{
				num2 = (num2 + 1) % actualFireModes.Count;
			}
			else
			{
				num2--;
				if (num2 < 0)
				{
					num2 = actualFireModes.Count - 1;
				}
			}
			FireMode fireMode = actualFireModes[num2];
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatWeaponCycle", false, false);
			this.newShipTemplate.SetFireModeForSlot(num, fireMode);
			this.UpdateFireModeUI(tishipWeaponTemplate, fireMode, this.selectedDragDestination.cornerIcon);
		}

		// Token: 0x06004DF3 RID: 19955 RVA: 0x00217ABC File Offset: 0x00215CBC
		private void UpdateFireModeUI(TIShipWeaponTemplate weapon, FireMode fireMode, Image gridCornerIcon)
		{
			if (weapon.GetActualFireModes(true).Count == 1)
			{
				this.installedFireModeButton.interactable = false;
			}
			else
			{
				this.installedFireModeButton.interactable = true;
			}
			string text = "";
			switch (fireMode)
			{
			case FireMode.Idle:
				this.installedFireModeButtonText.SetText(Loc.T("UI.SpaceCombat.Idle"));
				this.installedFireModeTooltip.SetText("BodyText", this.FireModeTooltip("Idle"));
				text = "ui_spacecombat/BUT_mode_idle";
				break;
			case FireMode.Focus:
				this.installedFireModeButtonText.SetText(Loc.T("UI.SpaceCombat.Focus"));
				this.installedFireModeTooltip.SetText("BodyText", this.FireModeTooltip(Loc.T("Focus")));
				text = "ui_spacecombat/BUT_mode_focus_fire";
				break;
			case FireMode.Offense:
				this.installedFireModeButtonText.SetText(weapon.isMissileWeapon ? Loc.T("UI.SpaceCombat.MissileOffense") : Loc.T("UI.SpaceCombat.Offense"));
				this.installedFireModeTooltip.SetText("BodyText", weapon.isMissileWeapon ? this.FireModeTooltip("MissileOffense") : this.FireModeTooltip("Offense"));
				text = (weapon.isMissileWeapon ? "ui_spacecombat/BUT_mode_missileattack" : "ui_spacecombat/BUT_mode_attack_red");
				break;
			case FireMode.Defense:
				this.installedFireModeButtonText.SetText(weapon.isMissileWeapon ? Loc.T("UI.SpaceCombat.MissileDefense") : Loc.T("UI.SpaceCombat.Defense"));
				this.installedFireModeTooltip.SetText("BodyText", weapon.isMissileWeapon ? this.FireModeTooltip("MissileDefense") : this.FireModeTooltip("Defense"));
				text = (weapon.isMissileWeapon ? "ui_spacecombat/BUT_mode_missiledefense" : "ui_spacecombat/BUT_mode_defense");
				break;
			case FireMode.Guardian:
				this.installedFireModeButtonText.SetText(Loc.T("UI.SpaceCombat.Guardian"));
				this.installedFireModeTooltip.SetText("BodyText", this.FireModeTooltip("Guardian"));
				text = "ui_spacecombat/BUT_mode_guardian";
				break;
			case FireMode.Salvo:
				this.installedFireModeButtonText.SetText(Loc.T("UI.SpaceCombat.Salvo"));
				this.installedFireModeTooltip.SetText("BodyText", this.FireModeTooltip("Salvo"));
				text = "ui_spacecombat/BUT_mode_salvo_fire";
				break;
			case FireMode.Bracket:
				this.installedFireModeButtonText.SetText(Loc.T("UI.SpaceCombat.Bracket"));
				this.installedFireModeTooltip.SetText("BodyText", this.FireModeTooltip("Bracket"));
				text = "ui_spacecombat/BUT_mode_bracketing";
				break;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(text, this.installedFireModeIcon);
			GameControl.assetLoader.LoadAssetForImageAssignment(text, gridCornerIcon);
		}

		// Token: 0x06004DF4 RID: 19956 RVA: 0x00217D44 File Offset: 0x00215F44
		private string FireModeTooltip(string fireModeName)
		{
			object obj = new StringBuilder("UI.SpaceCombat.").Append(fireModeName);
			StringBuilder stringBuilder = new StringBuilder("UI.SpaceCombat.").Append(fireModeName).Append(".description");
			StringBuilder stringBuilder2 = new StringBuilder(Loc.T(obj.ToString())).AppendLine();
			stringBuilder2.AppendLine(Loc.T(stringBuilder.ToString()));
			return stringBuilder2.ToString();
		}

		// Token: 0x06004DF5 RID: 19957 RVA: 0x00217DA8 File Offset: 0x00215FA8
		public void OnClickExitConstructionManager()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.constructionManagerSelectedDesign = null;
			this.constructionManagerSelectedQueueItem = null;
			this.constructionManagerCanvas.enabled = false;
			this.restoreCanvas.enabled = true;
			this.HideTutorials();
			if (this.restoreCanvas == this.fleetListCanvas)
			{
				if (this.fleetListDirty)
				{
					this.UpdateFleetsList();
				}
				this.ShowFleetListTutorial();
				return;
			}
			if (this.restoreCanvas == this.ShipDesignerCanvas)
			{
				this.ShowShipDesigner();
			}
		}

		// Token: 0x06004DF6 RID: 19958 RVA: 0x00217E2E File Offset: 0x0021602E
		public void OnClickGotoHabScreen()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			base.canvasManager.ToggleInfoScreen<HabitatsScreenController>();
		}

		// Token: 0x06004DF7 RID: 19959 RVA: 0x00217E47 File Offset: 0x00216047
		public void OnClickHideObsoleteToggle(bool toggleValue)
		{
			if (GameControl.loadcycle100)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			this.showObsoleteClasses = !this.classListHideObsoleteToggle.isOn;
			TIGlobalValuesState.GlobalValues.fleetScreenClassShowObsolete = this.showObsoleteClasses;
			this.UpdateShipClassListScreen();
		}

		// Token: 0x06004DF8 RID: 19960 RVA: 0x00217E86 File Offset: 0x00216086
		public void OnClickRefitTab()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, false);
			this.ShowRefitTab();
		}

		// Token: 0x06004DF9 RID: 19961 RVA: 0x00217E9C File Offset: 0x0021609C
		public void ShowRefitTab()
		{
			this.refitScrollviews.SetActive(true);
			this.constructScrollViewObject.SetActive(false);
			this.refitTabButton.interactable = false;
			this.constructTabButton.interactable = true;
			this.designToRefitTo = null;
			this.shipSelectedForRefit = null;
			this.multiSelectedRefitShips.Clear();
			this.RefreshAddToFastestQueueButton();
			this.DeSelectRefitClasses();
			this.SetRefitFilterList();
			this.FilterShipLists();
			this.RefreshShipyards(true, null);
		}

		// Token: 0x06004DFA RID: 19962 RVA: 0x00217F14 File Offset: 0x00216114
		public void ShowRefitTabWithFleetSelection(TISpaceFleetState fleet)
		{
			this.ShowRefitTab();
			if (fleet.dockedAtHab)
			{
				TISpaceBodyState tispaceBodyState = fleet.dockedLocation.ref_hab.ref_spaceBody;
				if (tispaceBodyState == null)
				{
					tispaceBodyState = fleet.dockedLocation.ref_lagrangePoint.ref_spaceBody;
				}
				this.SetConstructionFilter(tispaceBodyState);
			}
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			TISpaceShipState flagShip = fleet.GetFlagship();
			if (flagShip != null && (flagShip.CanRefit || flagShip.NeedsRefit) && !base.activePlayer.obsoleteShipDesigns.Contains(flagShip.BestExistingRefit.dataName))
			{
				list.AddRange(fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.template == flagShip.template).ToArray<TISpaceShipState>());
			}
			else
			{
				using (List<TISpaceShipTemplate>.Enumerator enumerator = (from s in fleet.ships
					group s by s.template into g
					orderby g.Count<TISpaceShipState>() descending, g.Key.displayName
					select g.Key).ToList<TISpaceShipTemplate>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceShipTemplate template = enumerator.Current;
						TISpaceShipTemplate tispaceShipTemplate;
						if (base.activePlayer.HasRefitForTemplate(template, out tispaceShipTemplate) && !base.activePlayer.obsoleteShipDesigns.Contains(tispaceShipTemplate.dataName))
						{
							list.AddRange(fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.template == template).ToArray<TISpaceShipState>());
							break;
						}
					}
				}
			}
			if (list.Count > 1)
			{
				int num = 0;
				using (List<TISpaceShipState>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TISpaceShipState tispaceShipState = enumerator2.Current;
						this.SetSelectedShipClassFromClassList(tispaceShipState.template, num == list.Count - 1, tispaceShipState, true);
						num++;
					}
					return;
				}
			}
			if (list.Count == 1)
			{
				this.SetSelectedShipClassFromClassList(list[0].template, true, list[0], false);
			}
		}

		// Token: 0x06004DFB RID: 19963 RVA: 0x002181AC File Offset: 0x002163AC
		public void OnClickConstructTab()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, false);
			this.ShowConstructTab();
		}

		// Token: 0x06004DFC RID: 19964 RVA: 0x002181C0 File Offset: 0x002163C0
		public void ShowConstructTab()
		{
			this.shipSelectedForRefit = null;
			this.refitScrollviews.SetActive(false);
			this.constructScrollViewObject.SetActive(true);
			this.refitTabButton.interactable = true;
			this.constructTabButton.interactable = false;
			this.designToRefitTo = null;
			this.constructionManagerSelectedDesign = null;
			this.RefreshAddToFastestQueueButton();
			this.multiSelectedRefitShips.Clear();
			this.RefreshRefitTab(this.hasDockedFleet);
			this.RefreshShipyards(false, null);
			this.RefreshConstructionManager();
			this.SetConstructionFilterList();
			this.FilterShipLists();
			this.DeSelectConstructionClasses();
		}

		// Token: 0x06004DFD RID: 19965 RVA: 0x0021824F File Offset: 0x0021644F
		public void RefreshRefitTab(bool allowRefit = false)
		{
			if (allowRefit)
			{
				this.refitTabButton.interactable = true;
				return;
			}
			this.refitTabButton.interactable = false;
			this.constructTabButton.interactable = true;
		}

		// Token: 0x06004DFE RID: 19966 RVA: 0x0021827C File Offset: 0x0021647C
		public void UpdateConstructionManager(TISpaceShipTemplate presetDesign = null)
		{
			this.constructionManagerCanvas.enabled = true;
			List<TISpaceShipTemplate> list = (from x in base.activePlayer.shipDesigns
				where !base.activePlayer.obsoleteShipDesigns.Contains(x.dataName)
				orderby x.wetMass_tons descending
				select x).ToList<TISpaceShipTemplate>();
			this.constructionShipClassList.SetListSize<ConstructionShipClassListItemController>(list.Count, false, false);
			int num = 0;
			this.noShipDesignsText.enabled = list.Count == 0;
			using (IEnumerator<object> enumerator = this.constructionShipClassList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__597.<>p__0 == null)
					{
						FleetsScreenController.<>o__597.<>p__0 = CallSite<Func<CallSite, object, ConstructionShipClassListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ConstructionShipClassListItemController), typeof(FleetsScreenController)));
					}
					ConstructionShipClassListItemController constructionShipClassListItemController = FleetsScreenController.<>o__597.<>p__0.Target(FleetsScreenController.<>o__597.<>p__0, enumerator.Current);
					constructionShipClassListItemController.Init(this, list[num++]);
					constructionShipClassListItemController.UpdateListItem();
				}
			}
			this.ShowConstructTab();
			if (presetDesign != null)
			{
				this.SetSelectedShipClassFromClassList(presetDesign, false, null, false);
			}
			this.FillOutSelectedDesignPanel(this.constructionManagerSelectedDesign);
			this.SetConstructionFilterList();
			this.ShowConstructionTutorial();
		}

		// Token: 0x06004DFF RID: 19967 RVA: 0x002183C0 File Offset: 0x002165C0
		public void RefreshConstructionManager()
		{
			if (!this.showRefitFeature)
			{
				this.refitTabButton.gameObject.SetActive(false);
				this.constructTabButton.gameObject.SetActive(false);
			}
			this.hasDockedFleet = false;
			this.dockedShips.Clear();
			List<TIHabModuleState> list = (from x in base.activePlayer.nShipyardQueues.Keys
				orderby x.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.semiMajorAxis_AU, x.ref_spaceBody != null descending, x.hab.displayName descending, x.tier descending
				select x).ToList<TIHabModuleState>();
			this.shipyardGridList.SetListSize<ShipyardGridItemController>(list.Count, false, false);
			int num = 0;
			this.shipyardGrid.SetActive(list.Count > 0);
			this.noShipyardsPanel.SetActive(list.Count == 0);
			using (IEnumerator<object> enumerator = this.shipyardGridList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__598.<>p__0 == null)
					{
						FleetsScreenController.<>o__598.<>p__0 = CallSite<Func<CallSite, object, ShipyardGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipyardGridItemController), typeof(FleetsScreenController)));
					}
					ShipyardGridItemController shipyardGridItemController = FleetsScreenController.<>o__598.<>p__0.Target(FleetsScreenController.<>o__598.<>p__0, enumerator.Current);
					shipyardGridItemController.Init(this, list[num++]);
					shipyardGridItemController.UpdateGridItem();
					if (shipyardGridItemController.shipyardIdx.ref_hab.dockedFleets != null && shipyardGridItemController.shipyardIdx.ref_hab.dockedFleets.Count > 0)
					{
						foreach (TISpaceFleetState tispaceFleetState in shipyardGridItemController.shipyardIdx.ref_hab.dockedFleets)
						{
							if (tispaceFleetState.faction != null && tispaceFleetState.faction == base.activePlayer)
							{
								this.hasDockedFleet = true;
								foreach (TISpaceShipState tispaceShipState in tispaceFleetState.ships)
								{
									if (!this.dockedShips.Contains(tispaceShipState) && base.activePlayer.HasRefitForTemplate(tispaceShipState.template))
									{
										this.dockedShips.Add(tispaceShipState);
									}
								}
							}
						}
					}
				}
			}
			this.RefreshRefitTab(list.Count > 0 && this.hasDockedFleet);
			this.dockedShipsList.SetListSize<DockedShipListItemController>(this.dockedShips.Count, false, false);
			int num2 = 0;
			using (IEnumerator<object> enumerator = this.dockedShipsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__598.<>p__1 == null)
					{
						FleetsScreenController.<>o__598.<>p__1 = CallSite<Func<CallSite, object, DockedShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(DockedShipListItemController), typeof(FleetsScreenController)));
					}
					DockedShipListItemController dockedShipListItemController = FleetsScreenController.<>o__598.<>p__1.Target(FleetsScreenController.<>o__598.<>p__1, enumerator.Current);
					dockedShipListItemController.Init(this, this.dockedShips[num2].template, this.dockedShips[num2], this.dockedShips[num2++].displayName);
					dockedShipListItemController.UpdateListItem();
				}
			}
			this.construction_ShipDesignerButtonBtn.interactable = FleetsScreenController.AllowedShipHulls(base.activePlayer, false).Count > 0;
			this.FilterShipLists();
			TISpaceShipState tispaceShipState2 = this.shipSelectedForRefit;
			this.DisplayValidRefits((tispaceShipState2 != null) ? tispaceShipState2.template : null, this.shipSelectedForRefit);
		}

		// Token: 0x06004E00 RID: 19968 RVA: 0x002187FC File Offset: 0x002169FC
		public void RefreshShipyards(bool refit = false, TISpaceShipState shipToRefit = null)
		{
			(from x in base.activePlayer.nShipyardQueues.Keys
				orderby x.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.semiMajorAxis_AU, x.hab.displayName descending, x.tier descending
				select x).ToList<TIHabModuleState>();
			using (IEnumerator<object> enumerator = this.shipyardGridList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__599.<>p__0 == null)
					{
						FleetsScreenController.<>o__599.<>p__0 = CallSite<Func<CallSite, object, ShipyardGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipyardGridItemController), typeof(FleetsScreenController)));
					}
					ShipyardGridItemController shipyardGridItemController = FleetsScreenController.<>o__599.<>p__0.Target(FleetsScreenController.<>o__599.<>p__0, enumerator.Current);
					if (refit)
					{
						shipyardGridItemController.gameObject.SetActive(false);
						if (shipyardGridItemController.shipyardIdx.ref_hab.dockedFleets.Count <= 0)
						{
							continue;
						}
						using (List<TISpaceFleetState>.Enumerator enumerator2 = shipyardGridItemController.shipyardIdx.ref_hab.dockedFleets.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								TISpaceFleetState tispaceFleetState = enumerator2.Current;
								using (List<TISpaceShipState>.Enumerator enumerator3 = tispaceFleetState.ships.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										if (enumerator3.Current == shipToRefit)
										{
											shipyardGridItemController.gameObject.SetActive(true);
										}
									}
								}
							}
							continue;
						}
					}
					shipyardGridItemController.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06004E01 RID: 19969 RVA: 0x002189F8 File Offset: 0x00216BF8
		public void SetConstructionFilterList()
		{
			this.constructionBodies.Clear();
			this.constructionFilterDropdown.options.Clear();
			if (base.activePlayer.nShipyardQueues.Keys.Count > 0)
			{
				int num = 0;
				using (IEnumerator<object> enumerator = this.shipyardGridList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__602.<>p__0 == null)
						{
							FleetsScreenController.<>o__602.<>p__0 = CallSite<Func<CallSite, object, ShipyardGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipyardGridItemController), typeof(FleetsScreenController)));
						}
						TISpaceBodyState ref_spaceBody = FleetsScreenController.<>o__602.<>p__0.Target(FleetsScreenController.<>o__602.<>p__0, enumerator.Current).shipyardIdx.sector.hab.barycenter.ref_spaceBody;
						if (!this.constructionBodies.Contains(ref_spaceBody))
						{
							this.constructionBodies.Add(ref_spaceBody);
							TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(ref_spaceBody.displayName);
							this.constructionFilterDropdown.options.Add(optionData);
							num++;
						}
						if (num == this.constructionFilterDropdown_EntryLimit)
						{
							TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData
							{
								text = Loc.T("UI.Habs.TooManyLocations")
							};
							this.constructionFilterDropdown.options.Add(optionData2);
							break;
						}
					}
				}
			}
			this.constructionFilterDropdown.gameObject.SetActive(this.constructionBodies.Count >= 2);
			this.constructionFilterDropdown.SetValueWithoutNotify(0);
			this.constructionFilterDropdown.captionText.SetText(Loc.T("UI.Habs.NoLocations"));
		}

		// Token: 0x06004E02 RID: 19970 RVA: 0x00218B8C File Offset: 0x00216D8C
		public void SetRefitFilterList()
		{
			this.refitBodies.Clear();
			this.constructionFilterDropdown.options.Clear();
			if (base.activePlayer.nShipyardQueues.Keys.Count > 0)
			{
				int num = 0;
				using (IEnumerator<object> enumerator = this.shipyardGridList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__603.<>p__0 == null)
						{
							FleetsScreenController.<>o__603.<>p__0 = CallSite<Func<CallSite, object, ShipyardGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipyardGridItemController), typeof(FleetsScreenController)));
						}
						ShipyardGridItemController shipyardGridItemController = FleetsScreenController.<>o__603.<>p__0.Target(FleetsScreenController.<>o__603.<>p__0, enumerator.Current);
						if (shipyardGridItemController.shipyardIdx.sector.hab.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction != null && x.faction == base.activePlayer).ToList<TISpaceFleetState>().Count >= 1)
						{
							TISpaceBodyState ref_spaceBody = shipyardGridItemController.shipyardIdx.sector.hab.barycenter.ref_spaceBody;
							if (!this.refitBodies.Contains(ref_spaceBody))
							{
								this.refitBodies.Add(ref_spaceBody);
								TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(ref_spaceBody.displayName);
								this.constructionFilterDropdown.options.Add(optionData);
								num++;
							}
							if (num == this.constructionFilterDropdown_EntryLimit)
							{
								TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData
								{
									text = Loc.T("UI.Habs.TooManyLocations")
								};
								this.constructionFilterDropdown.options.Add(optionData2);
								break;
							}
						}
					}
				}
			}
			this.constructionFilterDropdown.gameObject.SetActive(this.refitBodies.Count >= 2);
			this.constructionFilterDropdown.SetValueWithoutNotify(0);
			this.constructionFilterDropdown.captionText.SetText(Loc.T("UI.Habs.NoLocations"));
		}

		// Token: 0x06004E03 RID: 19971 RVA: 0x00218D68 File Offset: 0x00216F68
		public void FilterShipLists()
		{
			List<int> bitIndices = this.constructionFilterDropdown.value.GetBitIndices();
			if (bitIndices.Contains(this.constructionFilterDropdown_EntryLimit))
			{
				int num = this.constructionFilterDropdown.value;
				num &= ~(1 << this.constructionFilterDropdown_EntryLimit);
				this.constructionFilterDropdown.SetValueWithoutNotify(num);
				bitIndices.Remove(this.constructionFilterDropdown_EntryLimit);
			}
			this.RefreshAddToFastestQueueButton();
			if (bitIndices.Count == 0)
			{
				if (!this.refitScrollviews.activeSelf)
				{
					using (IEnumerator<object> enumerator = this.shipyardGridList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (FleetsScreenController.<>o__604.<>p__0 == null)
							{
								FleetsScreenController.<>o__604.<>p__0 = CallSite<Func<CallSite, object, ShipyardGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipyardGridItemController), typeof(FleetsScreenController)));
							}
							FleetsScreenController.<>o__604.<>p__0.Target(FleetsScreenController.<>o__604.<>p__0, enumerator.Current).gameObject.SetActive(true);
						}
						return;
					}
				}
				using (IEnumerator<object> enumerator = this.dockedShipsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__604.<>p__1 == null)
						{
							FleetsScreenController.<>o__604.<>p__1 = CallSite<Func<CallSite, object, DockedShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(DockedShipListItemController), typeof(FleetsScreenController)));
						}
						FleetsScreenController.<>o__604.<>p__1.Target(FleetsScreenController.<>o__604.<>p__1, enumerator.Current).gameObject.SetActive(true);
					}
				}
				return;
			}
			if (!this.refitScrollviews.activeSelf)
			{
				using (IEnumerator<object> enumerator = this.shipyardGridList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__604.<>p__2 == null)
						{
							FleetsScreenController.<>o__604.<>p__2 = CallSite<Func<CallSite, object, ShipyardGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipyardGridItemController), typeof(FleetsScreenController)));
						}
						ShipyardGridItemController shipyardGridItemController = FleetsScreenController.<>o__604.<>p__2.Target(FleetsScreenController.<>o__604.<>p__2, enumerator.Current);
						List<TINaturalSpaceObjectState> list = new List<TINaturalSpaceObjectState>();
						foreach (int num2 in bitIndices)
						{
							list.AddUnique(this.constructionBodies[num2]);
						}
						shipyardGridItemController.gameObject.SetActive(list.Contains(shipyardGridItemController.shipyardIdx.sector.hab.barycenter.ref_spaceBody));
					}
					goto IL_0339;
				}
			}
			using (IEnumerator<object> enumerator = this.dockedShipsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__604.<>p__3 == null)
					{
						FleetsScreenController.<>o__604.<>p__3 = CallSite<Func<CallSite, object, DockedShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(DockedShipListItemController), typeof(FleetsScreenController)));
					}
					DockedShipListItemController dockedShipListItemController = FleetsScreenController.<>o__604.<>p__3.Target(FleetsScreenController.<>o__604.<>p__3, enumerator.Current);
					List<TINaturalSpaceObjectState> list2 = new List<TINaturalSpaceObjectState>();
					foreach (int num3 in bitIndices)
					{
						list2.AddUnique(this.refitBodies[num3]);
					}
					dockedShipListItemController.gameObject.SetActive(list2.Contains(dockedShipListItemController.shipState.fleet.dockedLocation.ref_hab.barycenter.ref_spaceBody));
				}
			}
			IL_0339:
			if (this.refitScrollviews.activeSelf && this.multiSelectedRefitShips.Count > 0)
			{
				bool flag = false;
				bool flag2 = false;
				foreach (TISpaceShipState tispaceShipState in this.multiSelectedRefitShips)
				{
					flag = tispaceShipState.template != this.multiSelectedRefitShips[0].template;
					flag2 = tispaceShipState.fleet != null && tispaceShipState.fleet.dockedLocation != this.multiSelectedRefitShips[0].fleet.dockedLocation;
					if (flag || flag2)
					{
						break;
					}
				}
				this.refitRefuelCostWarningObject.SetActive(flag || flag2);
				return;
			}
			this.refitRefuelCostWarningObject.SetActive(false);
		}

		// Token: 0x06004E04 RID: 19972 RVA: 0x002191E0 File Offset: 0x002173E0
		public void SetConstructionFilter(TISpaceBodyState spaceBody)
		{
			List<int> list = new List<int>();
			foreach (TMP_Dropdown.OptionData optionData in this.constructionFilterDropdown.options)
			{
				if (optionData.text == spaceBody.displayName)
				{
					list.Add(this.constructionFilterDropdown.options.IndexOf(optionData));
					break;
				}
			}
			this.constructionFilterDropdown.value = list.ToIntFromBitIndices();
		}

		// Token: 0x06004E05 RID: 19973 RVA: 0x00219274 File Offset: 0x00217474
		public void FillOutSelectedDesignPanel(TISpaceShipTemplate design)
		{
			if (design != null)
			{
				this.selectedShipClassHeader.SetText(design.fullClassName);
				this.selectedShipConstructionTime.SetText(this.HullConstructionTimeBreakdown(design.hullTemplate, true));
				CombatantListItemController.SetNoseImage(design, this.selectedShipClassNose);
				CombatantListItemController.SetMidImage(design, this.selectedShipClassHull);
				CombatantListItemController.SetTailImage(design, this.selectedShipClassTail);
				CombatantListItemController.SetRadiatorImage(design, this.selectedShipClassRadiator);
				CombatantListItemController.SetDriveImage(design, this.selectedShipClassDrive);
				this.selectedShipClassAccel.SetText(FleetsScreenController.accelerationStr((double)design.baseCruiseAcceleration_gs(true), false, false, false));
				this.selectedShipClassConstructionCost.SetText(design.spaceResourceConstructionCost(false, null, true, false, false).ToString("Relevant", false, false, null, false, FactionResource.None));
				this.selectedShipClassDV.SetText(Loc.T("UI.Fleets.SingleDV", new object[] { TIUtilities.FormatBigOrSmallNumber(design.baseCruiseDeltaV_kps(true), 1, 7, 0, false, false) }));
				this.selectedShipClassCombatValue.SetText(new StringBuilder(TemplateManager.global.spaceCombatScoreInlineSpritePath).Append(design.TemplateSpaceCombatValue(false, -1f, 1f, false).ToString("N0")));
				this.selectedShipClassRoleValue.SetText(design.roleStr);
				this.selectedShipClassArmorValue.SetText(Loc.T("UI.Fleets.ArmorSummaryValue", new object[] { design.noseArmorValue, design.lateralArmorValue, design.tailArmorValue }));
				List<TIShipWeaponTemplate> list = design.noseWeaponTemplates.ToList<TIShipWeaponTemplate>();
				List<TIShipWeaponTemplate> list2 = design.hullWeaponTemplates.ToList<TIShipWeaponTemplate>();
				List<TIShipModuleTemplate> list3 = design.utilitySlotModuleTemplates.ToList<TIShipModuleTemplate>();
				this.selectedShipClassNoseWeaponList.SetListSize<ConstructionScreenShipPartListItemController>(list.Count, false, false);
				this.selectedShipClassHullWeaponList.SetListSize<ConstructionScreenShipPartListItemController>(list2.Count, false, false);
				this.selectedShipClassUtilityModuleList.SetListSize<ConstructionScreenShipPartListItemController>(list3.Count, false, false);
				this.selectedShipClassNoseButtonObject.SetActive(list.Count > 0);
				this.selectedShipClassHullButtonObject.SetActive(list2.Count > 0);
				this.selectedShipClassUtilitiesButtonObject.SetActive(list3.Count > 0);
				int num = 0;
				using (IEnumerator<object> enumerator = this.selectedShipClassNoseWeaponList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__606.<>p__0 == null)
						{
							FleetsScreenController.<>o__606.<>p__0 = CallSite<Func<CallSite, object, ConstructionScreenShipPartListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ConstructionScreenShipPartListItemController), typeof(FleetsScreenController)));
						}
						FleetsScreenController.<>o__606.<>p__0.Target(FleetsScreenController.<>o__606.<>p__0, enumerator.Current).SetListItem(list[num++]);
					}
				}
				num = 0;
				using (IEnumerator<object> enumerator = this.selectedShipClassHullWeaponList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__606.<>p__1 == null)
						{
							FleetsScreenController.<>o__606.<>p__1 = CallSite<Func<CallSite, object, ConstructionScreenShipPartListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ConstructionScreenShipPartListItemController), typeof(FleetsScreenController)));
						}
						FleetsScreenController.<>o__606.<>p__1.Target(FleetsScreenController.<>o__606.<>p__1, enumerator.Current).SetListItem(list2[num++]);
					}
				}
				num = 0;
				using (IEnumerator<object> enumerator = this.selectedShipClassUtilityModuleList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__606.<>p__2 == null)
						{
							FleetsScreenController.<>o__606.<>p__2 = CallSite<Func<CallSite, object, ConstructionScreenShipPartListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ConstructionScreenShipPartListItemController), typeof(FleetsScreenController)));
						}
						FleetsScreenController.<>o__606.<>p__2.Target(FleetsScreenController.<>o__606.<>p__2, enumerator.Current).SetListItem(list3[num++]);
					}
				}
				if (this.selectedShipClassTabbedPaneManager.activeTab == null || (this.selectedShipClassTabbedPaneManager.activeTab == this.selectedShipClassNoseTabController && list.Count == 0) || (this.selectedShipClassTabbedPaneManager.activeTab == this.selectedShipClassHullTabController && list2.Count == 0) || (this.selectedShipClassTabbedPaneManager.activeTab == this.selectedShipClassUtilTabController && list3.Count == 0))
				{
					if (list.Count > 0)
					{
						this.selectedShipClassTabbedPaneManager.Toggle(this.selectedShipClassNoseTabController);
					}
					else if (list2.Count > 0)
					{
						this.selectedShipClassTabbedPaneManager.Toggle(this.selectedShipClassHullTabController);
					}
					else if (list3.Count > 0)
					{
						this.selectedShipClassTabbedPaneManager.Toggle(this.selectedShipClassUtilTabController);
					}
				}
				this.selectedShipClassDetailObject.SetActive(true);
				return;
			}
			this.selectedShipClassHeader.SetText(Loc.T("UI.Fleets.SelectAShipClass"));
			this.selectedShipClassDetailObject.SetActive(false);
		}

		// Token: 0x06004E06 RID: 19974 RVA: 0x00219724 File Offset: 0x00217924
		public void SetSelectedShipClassFromClassList(TISpaceShipTemplate design, bool refit = false, TISpaceShipState shipToRefit = null, bool shiftPressed = false)
		{
			this.constructionManagerSelectedDesign = design;
			this.RefreshAddToFastestQueueButton();
			using (IEnumerator<object> enumerator = this.constructionShipClassList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__608.<>p__0 == null)
					{
						FleetsScreenController.<>o__608.<>p__0 = CallSite<Func<CallSite, object, ConstructionShipClassListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ConstructionShipClassListItemController), typeof(FleetsScreenController)));
					}
					FleetsScreenController.<>o__608.<>p__0.Target(FleetsScreenController.<>o__608.<>p__0, enumerator.Current).HighlightButtonAfterSelection(this.constructionManagerSelectedDesign);
				}
			}
			if (shipToRefit != null)
			{
				if (shiftPressed)
				{
					if (this.multiSelectedRefitShips.Count > 0 && shipToRefit.template == this.multiSelectedRefitShips[0].template && shipToRefit.fleet.dockedLocation == this.multiSelectedRefitShips[0].fleet.dockedLocation)
					{
						if (!this.multiSelectedRefitShips.Contains(shipToRefit))
						{
							this.refitRefuelCostWarningObject.SetActive(true);
							this.multiSelectedRefitShips.Add(shipToRefit);
						}
						else
						{
							this.multiSelectedRefitShips.Remove(shipToRefit);
							this.refitRefuelCostWarningObject.SetActive(this.multiSelectedRefitShips.Count > 0);
						}
					}
					else
					{
						this.multiSelectedRefitShips.Clear();
						this.multiSelectedRefitShips.Add(shipToRefit);
						this.refitRefuelCostWarningObject.SetActive(false);
					}
				}
				else
				{
					this.multiSelectedRefitShips.Clear();
					this.multiSelectedRefitShips.Add(shipToRefit);
					this.refitRefuelCostWarningObject.SetActive(false);
				}
				using (IEnumerator<object> enumerator = this.dockedShipsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsScreenController.<>o__608.<>p__1 == null)
						{
							FleetsScreenController.<>o__608.<>p__1 = CallSite<Func<CallSite, object, DockedShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(DockedShipListItemController), typeof(FleetsScreenController)));
						}
						DockedShipListItemController dockedShipListItemController = FleetsScreenController.<>o__608.<>p__1.Target(FleetsScreenController.<>o__608.<>p__1, enumerator.Current);
						if (!shiftPressed)
						{
							dockedShipListItemController.HighlightButtonAfterSelection(shipToRefit);
						}
						else
						{
							dockedShipListItemController.HighlightButtonAfterSelection(this.multiSelectedRefitShips);
						}
					}
					goto IL_021F;
				}
			}
			if (this.shipSelectedForRefit == null)
			{
				this.designToRefitTo = null;
				this.multiSelectedRefitShips.Clear();
			}
			IL_021F:
			using (IEnumerator<object> enumerator = this.validRefitClassesList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__608.<>p__2 == null)
					{
						FleetsScreenController.<>o__608.<>p__2 = CallSite<Func<CallSite, object, RefitClassListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(RefitClassListItemController), typeof(FleetsScreenController)));
					}
					FleetsScreenController.<>o__608.<>p__2.Target(FleetsScreenController.<>o__608.<>p__2, enumerator.Current).HighlightButtonAfterSelection(this.constructionManagerSelectedDesign);
				}
			}
			this.FillOutSelectedDesignPanel(this.constructionManagerSelectedDesign);
			using (IEnumerator<object> enumerator = this.shipyardGridList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__608.<>p__3 == null)
					{
						FleetsScreenController.<>o__608.<>p__3 = CallSite<Func<CallSite, object, ShipyardGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipyardGridItemController), typeof(FleetsScreenController)));
					}
					FleetsScreenController.<>o__608.<>p__3.Target(FleetsScreenController.<>o__608.<>p__3, enumerator.Current).OnNewClassSelectedInFleetController();
				}
			}
			if (refit)
			{
				this.DisplayValidRefits(design, shipToRefit);
				if (shipToRefit != null)
				{
					this.shipSelectedForRefit = shipToRefit;
					this.RefreshShipyards(true, this.shipSelectedForRefit);
				}
			}
		}

		// Token: 0x06004E07 RID: 19975 RVA: 0x00219A98 File Offset: 0x00217C98
		private void DisplayValidRefits(TISpaceShipTemplate design, TISpaceShipState shipToRefit)
		{
			if (design == null || !TIGameState.Valid(shipToRefit))
			{
				this.validRefitClassesList.SetListSize<RefitClassListItemController>(0, false, false);
				return;
			}
			int num = 0;
			List<TISpaceShipTemplate> list = new List<TISpaceShipTemplate>();
			foreach (TISpaceShipTemplate tispaceShipTemplate in base.activePlayer.shipDesigns)
			{
				string text;
				if (design != tispaceShipTemplate && !base.activePlayer.obsoleteShipDesigns.Contains(tispaceShipTemplate.dataName) && tispaceShipTemplate.IsAValidRefitFor(design, out text, false))
				{
					num++;
					list.Add(tispaceShipTemplate);
				}
			}
			this.validRefitClassesList.SetListSize<RefitClassListItemController>(num, false, false);
			int num2 = 0;
			using (IEnumerator<object> enumerator2 = this.validRefitClassesList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (FleetsScreenController.<>o__609.<>p__0 == null)
					{
						FleetsScreenController.<>o__609.<>p__0 = CallSite<Func<CallSite, object, RefitClassListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(RefitClassListItemController), typeof(FleetsScreenController)));
					}
					RefitClassListItemController refitClassListItemController = FleetsScreenController.<>o__609.<>p__0.Target(FleetsScreenController.<>o__609.<>p__0, enumerator2.Current);
					refitClassListItemController.Init(this, list[num2++], design, shipToRefit);
					refitClassListItemController.UpdateListItem();
				}
			}
		}

		// Token: 0x06004E08 RID: 19976 RVA: 0x00219BE8 File Offset: 0x00217DE8
		public void SetSelectedConstructionQueueItem(ShipConstructionQueueItem item)
		{
			this.constructionManagerSelectedQueueItem = item;
			this.constructionManagerSelectedDesign = ((item != null) ? item.shipDesign : null);
			this.designToRefitTo = null;
			this.shipSelectedForRefit = null;
			this.DeSelectRefitClasses();
			using (IEnumerator<object> enumerator = this.constructionShipClassList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__610.<>p__0 == null)
					{
						FleetsScreenController.<>o__610.<>p__0 = CallSite<Func<CallSite, object, ConstructionShipClassListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ConstructionShipClassListItemController), typeof(FleetsScreenController)));
					}
					FleetsScreenController.<>o__610.<>p__0.Target(FleetsScreenController.<>o__610.<>p__0, enumerator.Current).HighlightButtonAfterSelection(this.constructionManagerSelectedDesign);
				}
			}
			this.FillOutSelectedDesignPanel(this.constructionManagerSelectedDesign);
			using (IEnumerator<object> enumerator = this.shipyardGridList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__610.<>p__1 == null)
					{
						FleetsScreenController.<>o__610.<>p__1 = CallSite<Func<CallSite, object, ShipyardGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipyardGridItemController), typeof(FleetsScreenController)));
					}
					FleetsScreenController.<>o__610.<>p__1.Target(FleetsScreenController.<>o__610.<>p__1, enumerator.Current).OnNewConstructionQueueItemSelected();
				}
			}
		}

		// Token: 0x06004E09 RID: 19977 RVA: 0x00219D2C File Offset: 0x00217F2C
		public void OnClickAddToFastestQueue()
		{
			if (this.constructionManagerSelectedDesign == null)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_InitiateBuildShip", false, false);
			float num = 99999f;
			TIHabModuleState tihabModuleState = null;
			bool flag = false;
			using (IEnumerator<object> enumerator = this.shipyardGridList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__611.<>p__0 == null)
					{
						FleetsScreenController.<>o__611.<>p__0 = CallSite<Func<CallSite, object, ShipyardGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipyardGridItemController), typeof(FleetsScreenController)));
					}
					ShipyardGridItemController shipyardGridItemController = FleetsScreenController.<>o__611.<>p__0.Target(FleetsScreenController.<>o__611.<>p__0, enumerator.Current);
					if (shipyardGridItemController.gameObject.activeSelf && shipyardGridItemController.shipyardIdx.powered)
					{
						float num2 = 0f;
						foreach (ShipConstructionQueueItem shipConstructionQueueItem in shipyardGridItemController.shipyardIdx.hab.faction.nShipyardQueues[shipyardGridItemController.shipyardIdx])
						{
							num2 += shipConstructionQueueItem.daysToCompletion;
						}
						float num3 = this.constructionManagerSelectedDesign.hullTemplate.constructionTime_Days(shipyardGridItemController.shipyardIdx);
						num2 += num3;
						if (num2 < num)
						{
							num = num2;
							tihabModuleState = shipyardGridItemController.shipyardIdx;
							flag = shipyardGridItemController.allowPayFromEarth;
						}
					}
				}
			}
			if (tihabModuleState == null)
			{
				return;
			}
			base.activePlayer.playerControl.StartAction(new AddShipDesignToConstructionQueueAction(tihabModuleState, this.constructionManagerSelectedDesign, flag, 1f, null, false, null, null));
			this.RefreshConstructionManager();
		}

		// Token: 0x06004E0A RID: 19978 RVA: 0x00219EEC File Offset: 0x002180EC
		private void RefreshAddToFastestQueueButton()
		{
			this.construction_AddToFastestQueueButton.gameObject.SetActive(this.constructionManagerSelectedDesign != null && this.constructionFilterDropdown.value.GetBitIndices().Count == 1 && !this.refitScrollviews.activeSelf);
		}

		// Token: 0x06004E0B RID: 19979 RVA: 0x00219F3C File Offset: 0x0021813C
		public void DeSelectConstructionClasses()
		{
			using (IEnumerator<object> enumerator = this.constructionShipClassList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__613.<>p__0 == null)
					{
						FleetsScreenController.<>o__613.<>p__0 = CallSite<Func<CallSite, object, ConstructionShipClassListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ConstructionShipClassListItemController), typeof(FleetsScreenController)));
					}
					FleetsScreenController.<>o__613.<>p__0.Target(FleetsScreenController.<>o__613.<>p__0, enumerator.Current).DeSelectButton();
				}
			}
		}

		// Token: 0x06004E0C RID: 19980 RVA: 0x00219FC8 File Offset: 0x002181C8
		public void DeSelectRefitClasses()
		{
			using (IEnumerator<object> enumerator = this.dockedShipsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__614.<>p__0 == null)
					{
						FleetsScreenController.<>o__614.<>p__0 = CallSite<Func<CallSite, object, DockedShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(DockedShipListItemController), typeof(FleetsScreenController)));
					}
					FleetsScreenController.<>o__614.<>p__0.Target(FleetsScreenController.<>o__614.<>p__0, enumerator.Current).DeSelectButton();
				}
			}
			using (IEnumerator<object> enumerator = this.validRefitClassesList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (FleetsScreenController.<>o__614.<>p__1 == null)
					{
						FleetsScreenController.<>o__614.<>p__1 = CallSite<Func<CallSite, object, RefitClassListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(RefitClassListItemController), typeof(FleetsScreenController)));
					}
					FleetsScreenController.<>o__614.<>p__1.Target(FleetsScreenController.<>o__614.<>p__1, enumerator.Current).DeSelectButton();
				}
			}
			this.refitRefuelCostWarningObject.SetActive(false);
		}

		// Token: 0x06004E0D RID: 19981 RVA: 0x0021A0D8 File Offset: 0x002182D8
		public override void OnDestroy()
		{
			GameControl.eventManager.RemoveListener<FleetDetailRequested>(new EventManager.EventDelegate<FleetDetailRequested>(this.OnFleetDetailRequested), null);
			GameControl.eventManager.RemoveListener<ShipDetailRequested>(new EventManager.EventDelegate<ShipDetailRequested>(this.OnShipDetailRequested), null);
			GameControl.eventManager.RemoveListener<ShipConstructionUpdated>(new EventManager.EventDelegate<ShipConstructionUpdated>(this.OnShipConstructionUpdated), null);
			GameControl.eventManager.RemoveListener<ShipyardUIRequested>(new EventManager.EventDelegate<ShipyardUIRequested>(this.OnShipyardRequested), null);
			GameControl.eventManager.RemoveListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.OnFleetCoreStatusChanged), null);
			GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnShipsRemovedFromFleet), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnHabModuleDestroyed), null);
			base.OnDestroy();
		}

		// Token: 0x06004E0E RID: 19982 RVA: 0x0021A18C File Offset: 0x0021838C
		public void CleanupTextures()
		{
			this.fleetCamera = null;
			this.individualShipCameraPrefab = null;
			if (this.fleetSceneCameraInstance != null)
			{
				Camera component = this.fleetSceneCameraInstance.GetComponent<Camera>();
				if (component.targetTexture != null)
				{
					RenderTexture targetTexture = component.targetTexture;
					component.targetTexture = null;
					targetTexture.Release();
				}
			}
			if (this.individualShipCamera != null && this.individualShipCamera.targetTexture != null)
			{
				RenderTexture targetTexture2 = this.individualShipCamera.targetTexture;
				this.individualShipCamera.targetTexture = null;
				targetTexture2.Release();
			}
		}

		// Token: 0x04003002 RID: 12290
		public TMP_Text fleetsScreenTitle;

		// Token: 0x04003003 RID: 12291
		public RectTransform primaryPanelTransform;

		// Token: 0x04003004 RID: 12292
		public TabbedPaneManager tabbedPaneManager;

		// Token: 0x04003005 RID: 12293
		public TabbedPaneController fleetsListTab;

		// Token: 0x04003006 RID: 12294
		public TabbedPaneController classListTab;

		// Token: 0x04003007 RID: 12295
		public TabbedPaneController shipDetailTab;

		// Token: 0x04003008 RID: 12296
		public TabbedPaneController shipDesignerTab;

		// Token: 0x04003009 RID: 12297
		public TabbedPaneController constructionTab;

		// Token: 0x0400300A RID: 12298
		public GameObject shipDetailTabButtonObject;

		// Token: 0x0400300B RID: 12299
		public GameObject shipDesignerTabButtonObject;

		// Token: 0x0400300C RID: 12300
		public TMP_Text fleetsListTabText;

		// Token: 0x0400300D RID: 12301
		public TMP_Text classListTabText;

		// Token: 0x0400300E RID: 12302
		public TMP_Text shipDetailTabText;

		// Token: 0x0400300F RID: 12303
		public TMP_Text shipDesignerTabText;

		// Token: 0x04003010 RID: 12304
		public TMP_Text constructionTabText;

		// Token: 0x04003011 RID: 12305
		public UITutorialController ShipClassUITutorialController;

		// Token: 0x04003012 RID: 12306
		public UITutorialController ShipConstructionUITutorialController;

		// Token: 0x04003013 RID: 12307
		public UITutorialController FleetScreenUITutorialController;

		// Token: 0x04003014 RID: 12308
		public UITutorialController ShipDetailUITutorialController;

		// Token: 0x04003015 RID: 12309
		public UITutorialController ShipDesignerUITutorialController;

		// Token: 0x04003016 RID: 12310
		private bool fleetsTutorialLock;

		// Token: 0x04003017 RID: 12311
		[Header("Ship Designer")]
		public Canvas ShipDesignerCanvas;

		// Token: 0x04003018 RID: 12312
		public ShipModuleListItem shipModuleIconPrefab;

		// Token: 0x04003019 RID: 12313
		public ShipModuleListItem shipModuleRowPrefab;

		// Token: 0x0400301A RID: 12314
		public GameObject dVwarningObject;

		// Token: 0x0400301B RID: 12315
		public TabbedPaneManager modulesTabPaneManager;

		// Token: 0x0400301C RID: 12316
		public TabbedPaneManager weaponsTabPaneManager;

		// Token: 0x0400301D RID: 12317
		public Canvas weaponPaneManagerCanvas;

		// Token: 0x0400301E RID: 12318
		public ShipWeaponTabPane weaponsTabPane;

		// Token: 0x0400301F RID: 12319
		public ShipWeaponTabPane gunsTabPane;

		// Token: 0x04003020 RID: 12320
		public ShipWeaponTabPane missilesTabPane;

		// Token: 0x04003021 RID: 12321
		public ShipWeaponTabPane magneticWeaponsTabPane;

		// Token: 0x04003022 RID: 12322
		public ShipWeaponTabPane plasmaWeaponsTabPane;

		// Token: 0x04003023 RID: 12323
		public ShipWeaponTabPane lasersTabPane;

		// Token: 0x04003024 RID: 12324
		public ShipWeaponTabPane particleWeaponsTabPane;

		// Token: 0x04003025 RID: 12325
		public ShipModuleTabPane utilitiesTabPane;

		// Token: 0x04003026 RID: 12326
		public ShipModuleTabPane radiatorsTabPane;

		// Token: 0x04003027 RID: 12327
		public ShipModuleTabPane batteriesTabPane;

		// Token: 0x04003028 RID: 12328
		public ShipModuleTabPane powerPlantsTabPane;

		// Token: 0x04003029 RID: 12329
		public ShipModuleTabPane drivesTabPane;

		// Token: 0x0400302A RID: 12330
		public ShipModuleTabPane armorTabPane;

		// Token: 0x0400302B RID: 12331
		public TabbedPaneController noseModulesTabPane;

		// Token: 0x0400302C RID: 12332
		public TabbedPaneController hullModulesTabPane;

		// Token: 0x0400302D RID: 12333
		public TMP_Text noseModulesTabText;

		// Token: 0x0400302E RID: 12334
		public TMP_Text hullModulesTabText;

		// Token: 0x0400302F RID: 12335
		public TMP_Text dVWarningText;

		// Token: 0x04003030 RID: 12336
		public Button weaponTabAllSubTabButton;

		// Token: 0x04003031 RID: 12337
		public TooltipTrigger noseWeaponsTooltip;

		// Token: 0x04003032 RID: 12338
		public TooltipTrigger hullWeaponsTooltip;

		// Token: 0x04003033 RID: 12339
		public TooltipTrigger utilityModulesTooltip;

		// Token: 0x04003034 RID: 12340
		public TooltipTrigger radiatorsTooltip;

		// Token: 0x04003035 RID: 12341
		public TooltipTrigger batteriesTooltip;

		// Token: 0x04003036 RID: 12342
		public TooltipTrigger powerPlantsTooltip;

		// Token: 0x04003037 RID: 12343
		public TooltipTrigger drivesTooltip;

		// Token: 0x04003038 RID: 12344
		public TooltipTrigger armorTooltip;

		// Token: 0x04003039 RID: 12345
		public TooltipTrigger allWeaponsTooltip;

		// Token: 0x0400303A RID: 12346
		public TooltipTrigger gunsTooltip;

		// Token: 0x0400303B RID: 12347
		public TooltipTrigger missilesTooltip;

		// Token: 0x0400303C RID: 12348
		public TooltipTrigger magneticWeaponsTooltip;

		// Token: 0x0400303D RID: 12349
		public TooltipTrigger plasmaWeaponsTooltip;

		// Token: 0x0400303E RID: 12350
		public TooltipTrigger lasersTooltip;

		// Token: 0x0400303F RID: 12351
		public TooltipTrigger particleWeaponsTooltip;

		// Token: 0x04003040 RID: 12352
		public TMP_Text TransferButtonText;

		// Token: 0x04003041 RID: 12353
		public TMP_Text TransferDurationText;

		// Token: 0x04003042 RID: 12354
		public TMP_Text TransferPlanText;

		// Token: 0x04003043 RID: 12355
		[Header("My Ship Customization")]
		public GameObject renameMyShipPanel;

		// Token: 0x04003044 RID: 12356
		public TMP_InputField nameInputField;

		// Token: 0x04003045 RID: 12357
		private List<TIShipPartTemplate> allShipPartTemplates;

		// Token: 0x04003046 RID: 12358
		private List<ShipModuleListItem> shipModuleListItems = new List<ShipModuleListItem>();

		// Token: 0x04003047 RID: 12359
		private List<ShipModuleListItem> shipModuleListItemsB = new List<ShipModuleListItem>();

		// Token: 0x04003048 RID: 12360
		private GridLayoutGroup shipModuleSlotGrid;

		// Token: 0x04003049 RID: 12361
		private ShipModuleDragDestination[] moduleDragDestinations;

		// Token: 0x0400304A RID: 12362
		private Dictionary<Vector2Int, ShipModuleDragDestination> shipModuleSlotDictionary;

		// Token: 0x0400304B RID: 12363
		[HideInInspector]
		public TISpaceShipTemplate newShipTemplate;

		// Token: 0x0400304C RID: 12364
		public Button designShipButton_FleetList;

		// Token: 0x0400304D RID: 12365
		public Toggle ShowObsoletePartsToggle;

		// Token: 0x0400304E RID: 12366
		public TMP_Text ShowObsoletePartsText;

		// Token: 0x0400304F RID: 12367
		private bool partsSortShowObsolete = true;

		// Token: 0x04003050 RID: 12368
		private bool first;

		// Token: 0x04003051 RID: 12369
		public static bool gotoDesigner;

		// Token: 0x04003052 RID: 12370
		public static bool gotoConstructionManager;

		// Token: 0x04003053 RID: 12371
		private const int MIN_DELTA_V_FOR_EARTH = 8;

		// Token: 0x04003054 RID: 12372
		private const int MIN_DELTA_V_FOR_MARS = 30;

		// Token: 0x04003055 RID: 12373
		private const int MIN_DELTA_V_FOR_BEYOND = 60;

		// Token: 0x04003056 RID: 12374
		private const float MIN_FUNCTIONAL_ACCEL = 0.0005f;

		// Token: 0x04003057 RID: 12375
		private const float MIN_AGILE_ACCEL = 0.01f;

		// Token: 0x04003058 RID: 12376
		public bool showShipPartsAsIcons = true;

		// Token: 0x04003059 RID: 12377
		[Header("Fleets List")]
		public ListManagerBase fleetsList;

		// Token: 0x0400305A RID: 12378
		public FleetScreenFleetListAdapter fleetScreenFleetListAdapter;

		// Token: 0x0400305B RID: 12379
		public List<FleetScreenFleetListItemModel> fleetScreenFleetListModels = new List<FleetScreenFleetListItemModel>();

		// Token: 0x0400305C RID: 12380
		public Canvas fleetListCanvas;

		// Token: 0x0400305D RID: 12381
		public TMP_Text fleetListToClassListButtonText;

		// Token: 0x0400305E RID: 12382
		public TMP_Text fleetListToShipDesignerButtonText;

		// Token: 0x0400305F RID: 12383
		public TMP_Text fleetListConstructionManagerButtonText;

		// Token: 0x04003060 RID: 12384
		public TMP_Text fleetListSortNameText;

		// Token: 0x04003061 RID: 12385
		public TMP_Text fleetListSortAlertLevelText;

		// Token: 0x04003062 RID: 12386
		public TMP_Text fleetListSortArrivalTimeText;

		// Token: 0x04003063 RID: 12387
		public TMP_Text fleetListSortOperationsText;

		// Token: 0x04003064 RID: 12388
		[HideInInspector]
		public Dictionary<TIGameState, bool> fleetOpenedStatus = new Dictionary<TIGameState, bool>();

		// Token: 0x04003065 RID: 12389
		public bool showEnemyFleets;

		// Token: 0x04003066 RID: 12390
		private bool fleetList_FilterHumanFactionsOnly;

		// Token: 0x04003067 RID: 12391
		private List<TIFactionState> hiddenFactions = new List<TIFactionState>();

		// Token: 0x04003068 RID: 12392
		public bool fleetListDirty;

		// Token: 0x04003069 RID: 12393
		private TIFactionState fleets_filterForFaction;

		// Token: 0x0400306A RID: 12394
		private List<TISpaceBodyState> fleets_HighFilterForSpaceBody;

		// Token: 0x0400306B RID: 12395
		private List<TINaturalSpaceObjectState> fleets_SpecificFilterForNaturalSpaceObject;

		// Token: 0x0400306C RID: 12396
		public TMP_Dropdown factionsDropdown;

		// Token: 0x0400306D RID: 12397
		public TMP_Dropdown locationDropdown_High;

		// Token: 0x0400306E RID: 12398
		public TMP_Dropdown locationDropdown_Specific;

		// Token: 0x0400306F RID: 12399
		private int locationDropdown_Specific_EntryLimit = 31;

		// Token: 0x04003070 RID: 12400
		private Dictionary<int, TIFactionState> factionDropdownLookup;

		// Token: 0x04003071 RID: 12401
		private Dictionary<int, TISpaceBodyState> highLocationDropdownLookup;

		// Token: 0x04003072 RID: 12402
		private Dictionary<int, TINaturalSpaceObjectState> specificLocationDropdownLookup;

		// Token: 0x04003073 RID: 12403
		private SortFleetDataBy currentFleetSort = SortFleetDataBy.ArrivalTime;

		// Token: 0x04003074 RID: 12404
		private bool initFleetsList;

		// Token: 0x04003075 RID: 12405
		[Header("Faction Class List")]
		public TMP_Text classListHeader;

		// Token: 0x04003076 RID: 12406
		public Image classListFactionGradient;

		// Token: 0x04003077 RID: 12407
		public Image classListFactionIcon;

		// Token: 0x04003078 RID: 12408
		public TMP_Text classListHideObsoleteText;

		// Token: 0x04003079 RID: 12409
		public Toggle classListHideObsoleteToggle;

		// Token: 0x0400307A RID: 12410
		public Canvas shipClassListCanvas;

		// Token: 0x0400307B RID: 12411
		public ListManagerBase shipClassList;

		// Token: 0x0400307C RID: 12412
		public TMP_Text fleetClassListSortNameText;

		// Token: 0x0400307D RID: 12413
		public TMP_Text fleetClassListSortHullText;

		// Token: 0x0400307E RID: 12414
		public TMP_Text fleetClassListSortRoleText;

		// Token: 0x0400307F RID: 12415
		public TMP_Text fleetClassListSortMassText;

		// Token: 0x04003080 RID: 12416
		public TMP_Text fleetClassListSortBuildCostText;

		// Token: 0x04003081 RID: 12417
		private SortFleetClassDataBy currentFleetClassSort;

		// Token: 0x04003082 RID: 12418
		private bool showObsoleteClasses = true;

		// Token: 0x04003083 RID: 12419
		private bool invertFleetClassSort;

		// Token: 0x04003084 RID: 12420
		[Header("Individual Ship Data Screen")]
		public Canvas individualShipCanvas;

		// Token: 0x04003085 RID: 12421
		[HideInInspector]
		public TISpaceShipState selectedShip;

		// Token: 0x04003086 RID: 12422
		public ListManagerBase shipsList;

		// Token: 0x04003087 RID: 12423
		public ShipDetailShipListAdapter ShipDetailShipListAdapter;

		// Token: 0x04003088 RID: 12424
		public List<ShipDetailShipListItemModel> ShipDetailShipListModels = new List<ShipDetailShipListItemModel>();

		// Token: 0x04003089 RID: 12425
		public TMP_Text showAllShipsToggleText;

		// Token: 0x0400308A RID: 12426
		public TMP_Text showOnlyShipsInSelectedFleetToggleText;

		// Token: 0x0400308B RID: 12427
		public TMP_Text valuesHeader;

		// Token: 0x0400308C RID: 12428
		public TMP_Text systemsHeader;

		// Token: 0x0400308D RID: 12429
		public TMP_Text missionSystemsHeader;

		// Token: 0x0400308E RID: 12430
		public TMP_Text damageHeader;

		// Token: 0x0400308F RID: 12431
		public TMP_Text selectedSystemHeader;

		// Token: 0x04003090 RID: 12432
		public TMP_Text selectedMissionSystemHeader;

		// Token: 0x04003091 RID: 12433
		public TMP_Text indiv_ShipName;

		// Token: 0x04003092 RID: 12434
		public TMP_Text indiv_LocationText;

		// Token: 0x04003093 RID: 12435
		public TMP_Text indiv_CrewText;

		// Token: 0x04003094 RID: 12436
		public TMP_Text indiv_DryMassText;

		// Token: 0x04003095 RID: 12437
		public TMP_Text indiv_WetMassText;

		// Token: 0x04003096 RID: 12438
		public TMP_Text indiv_CurrentMassText;

		// Token: 0x04003097 RID: 12439
		public TMP_Text indiv_DeltaVText;

		// Token: 0x04003098 RID: 12440
		public TMP_Text indiv_CruiseAccelerationText;

		// Token: 0x04003099 RID: 12441
		public TMP_Text indiv_CombatAccelerationText;

		// Token: 0x0400309A RID: 12442
		public TMP_Text indiv_TurnRateText;

		// Token: 0x0400309B RID: 12443
		public TMP_Text indiv_LengthText;

		// Token: 0x0400309C RID: 12444
		public TMP_Text indiv_BeamText;

		// Token: 0x0400309D RID: 12445
		public TMP_Text indiv_DriveText;

		// Token: 0x0400309E RID: 12446
		public TMP_Text indiv_RoleText;

		// Token: 0x0400309F RID: 12447
		public TMP_Text indiv_PowerPlantText;

		// Token: 0x040030A0 RID: 12448
		public TMP_Text indiv_BatteryText;

		// Token: 0x040030A1 RID: 12449
		public TMP_Text indiv_RadiatorsText;

		// Token: 0x040030A2 RID: 12450
		public TMP_Text indiv_HeatSinkCapacityText;

		// Token: 0x040030A3 RID: 12451
		public TMP_Text indiv_ShipClass;

		// Token: 0x040030A4 RID: 12452
		public TMP_Text invid_RefuelCost;

		// Token: 0x040030A5 RID: 12453
		public ListManagerBase noseWeaponsList;

		// Token: 0x040030A6 RID: 12454
		public ListManagerBase hullWeaponsList;

		// Token: 0x040030A7 RID: 12455
		public ListManagerBase utilityModulesList;

		// Token: 0x040030A8 RID: 12456
		public TMP_Text indiv_noseWeaponsHeader;

		// Token: 0x040030A9 RID: 12457
		public TMP_Text indiv_hullWeaponsHeader;

		// Token: 0x040030AA RID: 12458
		public TMP_Text indiv_utilityWeaponsHeader;

		// Token: 0x040030AB RID: 12459
		public TMP_Text indiv_NoseArmorMaterial;

		// Token: 0x040030AC RID: 12460
		public TMP_Text indiv_NoseArmorRating;

		// Token: 0x040030AD RID: 12461
		public TMP_Text indiv_LateralArmorMaterial;

		// Token: 0x040030AE RID: 12462
		public TMP_Text indiv_LateralArmorRating;

		// Token: 0x040030AF RID: 12463
		public TMP_Text indiv_TailArmorMaterial;

		// Token: 0x040030B0 RID: 12464
		public TMP_Text indiv_TailArmorRating;

		// Token: 0x040030B1 RID: 12465
		public ListManagerBase officersList;

		// Token: 0x040030B2 RID: 12466
		public ShipModelViewer shipModelViewer;

		// Token: 0x040030B3 RID: 12467
		public int primarySystem;

		// Token: 0x040030B4 RID: 12468
		public TMP_Text leftSystemDetail;

		// Token: 0x040030B5 RID: 12469
		public GameObject leftHandDetailPanel;

		// Token: 0x040030B6 RID: 12470
		public TMP_Text leftHandDetailPanelHeader;

		// Token: 0x040030B7 RID: 12471
		public TMP_Text rightSystemDetail;

		// Token: 0x040030B8 RID: 12472
		public GameObject rightHandDetailPanel;

		// Token: 0x040030B9 RID: 12473
		public TMP_Text rightHandDetailPanelHeader;

		// Token: 0x040030BA RID: 12474
		public Image hullDamageControlImage;

		// Token: 0x040030BB RID: 12475
		public Image radiatorDamageControlImage;

		// Token: 0x040030BC RID: 12476
		public Image driveDamageControlImage;

		// Token: 0x040030BD RID: 12477
		private bool shipListInitialized;

		// Token: 0x040030BE RID: 12478
		private bool showEnemyShipsOnList;

		// Token: 0x040030BF RID: 12479
		public Toggle showEnemyShipsToggle;

		// Token: 0x040030C0 RID: 12480
		private bool showOnlyShipsInSelectedFleet;

		// Token: 0x040030C1 RID: 12481
		public Toggle showOnlyShipsInSelectedFleetToggle;

		// Token: 0x040030C2 RID: 12482
		public GameObject damageControlPanel;

		// Token: 0x040030C3 RID: 12483
		public GridLayoutGroup masterDamageGridGroup;

		// Token: 0x040030C4 RID: 12484
		private Dictionary<Vector2Int, SpaceCombatDamageGridItemController> masterDamageGridControllers;

		// Token: 0x040030C5 RID: 12485
		private Dictionary<ModuleDataEntry, SpaceCombatDamageGridItemController> moduleDamageGrid;

		// Token: 0x040030C6 RID: 12486
		private Dictionary<ShipSystem, SpaceCombatDamageGridItemController> systemDamageGrid;

		// Token: 0x040030C7 RID: 12487
		public GameObject individualShipCameraPrefab;

		// Token: 0x040030C8 RID: 12488
		private GameObject individualShipCameraObject;

		// Token: 0x040030C9 RID: 12489
		private Camera individualShipCamera;

		// Token: 0x040030CA RID: 12490
		private bool hideCrew;

		// Token: 0x040030CB RID: 12491
		private bool hidePowerPlant;

		// Token: 0x040030CC RID: 12492
		private bool hideBattery;

		// Token: 0x040030CD RID: 12493
		private bool hideRadiator;

		// Token: 0x040030CE RID: 12494
		private bool hideHeatSink;

		// Token: 0x040030CF RID: 12495
		private bool hideArmor;

		// Token: 0x040030D0 RID: 12496
		private bool hideWeapons;

		// Token: 0x040030D1 RID: 12497
		private GameObject indivPreviewPosition;

		// Token: 0x040030D2 RID: 12498
		private GameObject indivShipVisObject;

		// Token: 0x040030D3 RID: 12499
		[Header("Ship Designer Part 2")]
		public TMP_Dropdown hullSelectionDropdown;

		// Token: 0x040030D4 RID: 12500
		public TMP_Text fullShipClassName;

		// Token: 0x040030D5 RID: 12501
		public TMP_Text designerShipDataClassName;

		// Token: 0x040030D6 RID: 12502
		public Image designerShipDataClassNose;

		// Token: 0x040030D7 RID: 12503
		public Image designerShipDataClassHull;

		// Token: 0x040030D8 RID: 12504
		public Image designerShipDataClassTail;

		// Token: 0x040030D9 RID: 12505
		public Image designerShipDataClassDrive;

		// Token: 0x040030DA RID: 12506
		public Image designerShipDataClassRadiator;

		// Token: 0x040030DB RID: 12507
		public TMP_Dropdown roleSelectionDropdown;

		// Token: 0x040030DC RID: 12508
		public TMP_InputField classNameInputField;

		// Token: 0x040030DD RID: 12509
		public TMP_Text classNamePlaceholder;

		// Token: 0x040030DE RID: 12510
		public TMP_Text classNameText;

		// Token: 0x040030DF RID: 12511
		private Dictionary<int, TIShipHullTemplate> hullDropdownValues;

		// Token: 0x040030E0 RID: 12512
		private Dictionary<string, int> reverseHullDropdownValues;

		// Token: 0x040030E1 RID: 12513
		public TMP_Text designerCoreDataHeader;

		// Token: 0x040030E2 RID: 12514
		public TMP_Text designerShipDataHeader;

		// Token: 0x040030E3 RID: 12515
		public TooltipTrigger designerMassBreakdownToolTipText;

		// Token: 0x040030E4 RID: 12516
		public TooltipTrigger designerCrewToolTipText;

		// Token: 0x040030E5 RID: 12517
		public TooltipTrigger designerCruiseAccelToolTipText;

		// Token: 0x040030E6 RID: 12518
		public TooltipTrigger designerCombatAccelToolTipText;

		// Token: 0x040030E7 RID: 12519
		public TooltipTrigger designerCruiseDeltaVToolTipText;

		// Token: 0x040030E8 RID: 12520
		public TooltipTrigger designerTurnRateToolTipText;

		// Token: 0x040030E9 RID: 12521
		public TooltipTrigger designerHeatSinkCapacityToolTipText;

		// Token: 0x040030EA RID: 12522
		public TooltipTrigger designerBatteryCapacityToolTipText;

		// Token: 0x040030EB RID: 12523
		public TooltipTrigger designerConstructionCostToolTipText;

		// Token: 0x040030EC RID: 12524
		public TooltipTrigger designerConstructionTimeToolTipText;

		// Token: 0x040030ED RID: 12525
		public TooltipTrigger designerSupportToolTipText;

		// Token: 0x040030EE RID: 12526
		public TMP_Text designerResetDesignButtonText;

		// Token: 0x040030EF RID: 12527
		public Button designerSaveDesignButton;

		// Token: 0x040030F0 RID: 12528
		public TMP_Text designerSaveDesignButtonText;

		// Token: 0x040030F1 RID: 12529
		public Button designerAutoDesignButton;

		// Token: 0x040030F2 RID: 12530
		public TMP_Text designerAutoDesignButtonText;

		// Token: 0x040030F3 RID: 12531
		public TMP_Text designerConfirmationHeaderText;

		// Token: 0x040030F4 RID: 12532
		public TMP_Text designerValidRefitText;

		// Token: 0x040030F5 RID: 12533
		public GameObject validRefitNotificationObject;

		// Token: 0x040030F6 RID: 12534
		public TooltipTrigger refitTooltipText;

		// Token: 0x040030F7 RID: 12535
		public TooltipTrigger designerSaveTooltipText;

		// Token: 0x040030F8 RID: 12536
		private int selectedHullIndex;

		// Token: 0x040030F9 RID: 12537
		private int maxHullIndex;

		// Token: 0x040030FA RID: 12538
		public TooltipTrigger altHullTooltip;

		// Token: 0x040030FB RID: 12539
		[HideInInspector]
		public bool changesMadeToExistingClass;

		// Token: 0x040030FC RID: 12540
		private bool shipDesignInProgress;

		// Token: 0x040030FF RID: 12543
		private readonly List<ShipRole> hideShipRoles = new List<ShipRole>
		{
			ShipRole.ArmyCarrier,
			ShipRole.EarthSurveillance
		};

		// Token: 0x04003100 RID: 12544
		private bool loadingExistingTemplate;

		// Token: 0x04003101 RID: 12545
		private Dictionary<ShipRole, int> roleOptions = new Dictionary<ShipRole, int>();

		// Token: 0x04003102 RID: 12546
		private Dictionary<int, ShipRole> reverseRoleOptions = new Dictionary<int, ShipRole>();

		// Token: 0x04003103 RID: 12547
		public TooltipTrigger automateRoleButtonTip;

		// Token: 0x04003104 RID: 12548
		public TooltipTrigger roleTip;

		// Token: 0x04003105 RID: 12549
		private ShipRole previousRole;

		// Token: 0x04003106 RID: 12550
		private int nameAttempts;

		// Token: 0x04003107 RID: 12551
		public TMP_Text designerCombatScoreText;

		// Token: 0x04003108 RID: 12552
		public TMP_Text designerWetMassTabText;

		// Token: 0x04003109 RID: 12553
		public TMP_Text designerWetMassText;

		// Token: 0x0400310A RID: 12554
		public TMP_Text designerCrewTabText;

		// Token: 0x0400310B RID: 12555
		public TMP_Text designerCrewText;

		// Token: 0x0400310C RID: 12556
		public TMP_Text designerCruiseAccelerationTabText;

		// Token: 0x0400310D RID: 12557
		public TMP_Text designerCruiseAccelerationText;

		// Token: 0x0400310E RID: 12558
		public TMP_Text designerCombatAccelerationTabText;

		// Token: 0x0400310F RID: 12559
		public TMP_Text designerCombatAccelerationText;

		// Token: 0x04003110 RID: 12560
		public TMP_Text designerCruiseDeltaVTabText;

		// Token: 0x04003111 RID: 12561
		public TMP_Text designerCruiseDeltaVText;

		// Token: 0x04003112 RID: 12562
		public TMP_Text designerTurnRateTabText;

		// Token: 0x04003113 RID: 12563
		public TMP_Text designerTurnRateText;

		// Token: 0x04003114 RID: 12564
		public TMP_Text designerHeatSinkCapacityTabText;

		// Token: 0x04003115 RID: 12565
		public TMP_Text designerHeatSinkCapacity;

		// Token: 0x04003116 RID: 12566
		public TMP_Text designerBatteryCapacityTabText;

		// Token: 0x04003117 RID: 12567
		public TMP_Text designerBatteryCapacity;

		// Token: 0x04003118 RID: 12568
		public TMP_Text designerConstructionCostTabText;

		// Token: 0x04003119 RID: 12569
		public TMP_Text designerConstructionCostText;

		// Token: 0x0400311A RID: 12570
		public TMP_Text designerConstructionTimeTabText;

		// Token: 0x0400311B RID: 12571
		public TMP_Text designerConstructionTimeText;

		// Token: 0x0400311C RID: 12572
		public TMP_Text designerMaintenanceCostTabText;

		// Token: 0x0400311D RID: 12573
		public TMP_Text designerMaintenanceCostText;

		// Token: 0x0400311E RID: 12574
		private static int lastSCVUpdateFrame;

		// Token: 0x0400311F RID: 12575
		public GameObject fleetCamera;

		// Token: 0x04003120 RID: 12576
		public GameObject cameraViewObject;

		// Token: 0x04003121 RID: 12577
		public RectTransform shipImageSpaceBackground;

		// Token: 0x04003122 RID: 12578
		public GameObject shipPrefab;

		// Token: 0x04003123 RID: 12579
		private GameObject fleetSceneCameraInstance;

		// Token: 0x04003124 RID: 12580
		private GameObject previewPosition;

		// Token: 0x04003125 RID: 12581
		private GameObject shipVisObject;

		// Token: 0x04003126 RID: 12582
		public RectTransform moduleDataContainer;

		// Token: 0x04003127 RID: 12583
		public Toggle selectedModulesCompareToggle;

		// Token: 0x04003128 RID: 12584
		public TMP_Text selectedModuleCompareHeaderText;

		// Token: 0x04003129 RID: 12585
		public Toggle installedModulesCompareToggle;

		// Token: 0x0400312A RID: 12586
		public TMP_Text installedModuleCompareHeaderText;

		// Token: 0x0400312B RID: 12587
		private bool comparingModules;

		// Token: 0x0400312C RID: 12588
		[Header("Ship Designer Part 2 - Selected Module Data")]
		public RectTransform selectedModuleDataContainer;

		// Token: 0x0400312D RID: 12589
		public Scrollbar selectedModuleScrollbar;

		// Token: 0x0400312E RID: 12590
		public TMP_Text selectedModuleHeaderText;

		// Token: 0x0400312F RID: 12591
		public Toggle selectedModuleObsoleteToggle;

		// Token: 0x04003130 RID: 12592
		public TMP_Text selectedModuleObsoleteHeaderText;

		// Token: 0x04003131 RID: 12593
		public RectTransform selectedModuleHeaderContainer;

		// Token: 0x04003132 RID: 12594
		public Image selectedModuleDataIcon;

		// Token: 0x04003133 RID: 12595
		public TMP_Text selectedModuleDataHeaderText;

		// Token: 0x04003134 RID: 12596
		public TMP_Text selectedModuleSecondaryHeader;

		// Token: 0x04003135 RID: 12597
		public TMP_Text selectedModulePreTableText;

		// Token: 0x04003136 RID: 12598
		public ListManagerBase selectedModuleTableList;

		// Token: 0x04003137 RID: 12599
		public TMP_Text selectedModulePostTableText;

		// Token: 0x04003138 RID: 12600
		public LayoutElement selectedModuleLayoutElement;

		// Token: 0x04003139 RID: 12601
		private TIShipPartTemplate currentlySelectedModule;

		// Token: 0x0400313A RID: 12602
		private bool selectedModuleDataDisplay;

		// Token: 0x0400313B RID: 12603
		public GameObject selectedModuleDataButtonsContainer;

		// Token: 0x0400313C RID: 12604
		public Button installModuleButton;

		// Token: 0x0400313D RID: 12605
		public TMP_Text installModuleButtonText;

		// Token: 0x0400313E RID: 12606
		[Header("Ship Designer Part 2 - Installed Module Data")]
		public RectTransform installedModuleDataContainer;

		// Token: 0x0400313F RID: 12607
		public Scrollbar installedModuleScrollbar;

		// Token: 0x04003140 RID: 12608
		public TMP_Text installedModuleHeaderText;

		// Token: 0x04003141 RID: 12609
		public Toggle installedModuleObsoleteToggle;

		// Token: 0x04003142 RID: 12610
		public TMP_Text installedModuleObsoleteHeaderText;

		// Token: 0x04003143 RID: 12611
		public RectTransform installedModuleHeaderContainer;

		// Token: 0x04003144 RID: 12612
		public Image installedModuleDataIcon;

		// Token: 0x04003145 RID: 12613
		public TMP_Text installedModuleDataHeaderText;

		// Token: 0x04003146 RID: 12614
		public TMP_Text installedModuleSecondaryHeader;

		// Token: 0x04003147 RID: 12615
		public TMP_Text installedModulePreTableText;

		// Token: 0x04003148 RID: 12616
		public ListManagerBase installedModuleTableList;

		// Token: 0x04003149 RID: 12617
		public TMP_Text installedModulePostTableText;

		// Token: 0x0400314A RID: 12618
		public LayoutElement installedModuleLayoutElement;

		// Token: 0x0400314B RID: 12619
		private TIShipPartTemplate currentlyInstalledModule;

		// Token: 0x0400314C RID: 12620
		private bool installedModuleDataDisplay;

		// Token: 0x0400314D RID: 12621
		public GameObject installedModuleDataButtonsContainer;

		// Token: 0x0400314E RID: 12622
		public Button installedDeleteModuleButton;

		// Token: 0x0400314F RID: 12623
		public TMP_Text installedDeleteModuleButtonText;

		// Token: 0x04003150 RID: 12624
		public Button installedFireModeButton;

		// Token: 0x04003151 RID: 12625
		public TMP_Text installedFireModeButtonText;

		// Token: 0x04003152 RID: 12626
		public Image installedFireModeIcon;

		// Token: 0x04003153 RID: 12627
		public TooltipTrigger installedFireModeTooltip;

		// Token: 0x04003154 RID: 12628
		private int _fireModeIndex;

		// Token: 0x04003155 RID: 12629
		[Header("Construction Manager")]
		public Canvas constructionManagerCanvas;

		// Token: 0x04003156 RID: 12630
		private Canvas restoreCanvas;

		// Token: 0x04003157 RID: 12631
		public ListManagerBase shipyardGridList;

		// Token: 0x04003158 RID: 12632
		public ListManagerBase constructionShipClassList;

		// Token: 0x04003159 RID: 12633
		public GameObject noShipyardsPanel;

		// Token: 0x0400315A RID: 12634
		public TMP_Text noShipyardsText;

		// Token: 0x0400315B RID: 12635
		public TMP_Text noShipyardsButtonText;

		// Token: 0x0400315C RID: 12636
		public TMP_Text addToFastestQueueButtonText;

		// Token: 0x0400315D RID: 12637
		public TMP_Text noShipClassSelectedText;

		// Token: 0x0400315E RID: 12638
		public TMP_Text noShipDesignsText;

		// Token: 0x0400315F RID: 12639
		public GameObject shipyardGrid;

		// Token: 0x04003160 RID: 12640
		public TMP_Text selectedShipClassHeader;

		// Token: 0x04003161 RID: 12641
		public GameObject selectedShipClassDetailObject;

		// Token: 0x04003162 RID: 12642
		public Image selectedShipClassNose;

		// Token: 0x04003163 RID: 12643
		public Image selectedShipClassHull;

		// Token: 0x04003164 RID: 12644
		public Image selectedShipClassTail;

		// Token: 0x04003165 RID: 12645
		public Image selectedShipClassDrive;

		// Token: 0x04003166 RID: 12646
		public Image selectedShipClassRadiator;

		// Token: 0x04003167 RID: 12647
		public TMP_Text selectedShipConstructionTime;

		// Token: 0x04003168 RID: 12648
		public TMP_Text selectedShipClassAccel;

		// Token: 0x04003169 RID: 12649
		public TMP_Text selectedShipClassDV;

		// Token: 0x0400316A RID: 12650
		public TMP_Text selectedShipClassCombatValue;

		// Token: 0x0400316B RID: 12651
		public TMP_Text selectedShipClassConstructionCost;

		// Token: 0x0400316C RID: 12652
		public TMP_Text selectedShipClassSelectedLabel;

		// Token: 0x0400316D RID: 12653
		public TMP_Text selectedShipClassRoleValue;

		// Token: 0x0400316E RID: 12654
		public TMP_Text selectedShipClassArmorTab;

		// Token: 0x0400316F RID: 12655
		public TMP_Text selectedShipClassArmorValue;

		// Token: 0x04003170 RID: 12656
		public ListManagerBase selectedShipClassNoseWeaponList;

		// Token: 0x04003171 RID: 12657
		public ListManagerBase selectedShipClassHullWeaponList;

		// Token: 0x04003172 RID: 12658
		public ListManagerBase selectedShipClassUtilityModuleList;

		// Token: 0x04003173 RID: 12659
		public TabbedPaneManager selectedShipClassTabbedPaneManager;

		// Token: 0x04003174 RID: 12660
		public TabbedPaneController selectedShipClassNoseTabController;

		// Token: 0x04003175 RID: 12661
		public TabbedPaneController selectedShipClassHullTabController;

		// Token: 0x04003176 RID: 12662
		public TabbedPaneController selectedShipClassUtilTabController;

		// Token: 0x04003177 RID: 12663
		public GameObject selectedShipClassNoseButtonObject;

		// Token: 0x04003178 RID: 12664
		public GameObject selectedShipClassHullButtonObject;

		// Token: 0x04003179 RID: 12665
		public GameObject selectedShipClassUtilitiesButtonObject;

		// Token: 0x0400317A RID: 12666
		private int constructionFilterDropdown_EntryLimit = 31;

		// Token: 0x0400317B RID: 12667
		public TMP_Dropdown constructionFilterDropdown;

		// Token: 0x0400317C RID: 12668
		[Header("Refits")]
		public GameObject constructScrollViewObject;

		// Token: 0x0400317D RID: 12669
		public GameObject refitScrollviews;

		// Token: 0x0400317E RID: 12670
		public GameObject refitRefuelCostWarningObject;

		// Token: 0x0400317F RID: 12671
		public ListManagerBase dockedShipsList;

		// Token: 0x04003180 RID: 12672
		public ListManagerBase validRefitClassesList;

		// Token: 0x04003181 RID: 12673
		public TMP_Text constructTabText;

		// Token: 0x04003182 RID: 12674
		public TMP_Text refitTabText;

		// Token: 0x04003183 RID: 12675
		public TMP_Text dockedShipsText;

		// Token: 0x04003184 RID: 12676
		public TMP_Text refitClassesText;

		// Token: 0x04003185 RID: 12677
		public TooltipTrigger refitRefuelCostTooltip;

		// Token: 0x04003186 RID: 12678
		public Button constructTabButton;

		// Token: 0x04003187 RID: 12679
		public Button refitTabButton;

		// Token: 0x04003188 RID: 12680
		public TISpaceShipTemplate oldShipTemplate;

		// Token: 0x04003189 RID: 12681
		public TISpaceShipTemplate designToRefitTo;

		// Token: 0x0400318A RID: 12682
		public TISpaceShipTemplate originalShipTemplate;

		// Token: 0x0400318B RID: 12683
		public TISpaceShipState shipSelectedForRefit;

		// Token: 0x0400318C RID: 12684
		private bool showRefitFeature = true;

		// Token: 0x0400318D RID: 12685
		private bool refitting = true;

		// Token: 0x0400318E RID: 12686
		private bool hasDockedFleet;

		// Token: 0x0400318F RID: 12687
		private int dockedShipsCount;

		// Token: 0x04003190 RID: 12688
		private List<TISpaceShipState> dockedShips = new List<TISpaceShipState>();

		// Token: 0x04003191 RID: 12689
		public TMP_Text construction_ShipListButton;

		// Token: 0x04003192 RID: 12690
		public TMP_Text construction_ShipDesignerButton;

		// Token: 0x04003193 RID: 12691
		public TMP_Text construction_FleetListButton;

		// Token: 0x04003194 RID: 12692
		public Button construction_ShipDesignerButtonBtn;

		// Token: 0x04003195 RID: 12693
		public Button construction_AddToFastestQueueButton;

		// Token: 0x04003196 RID: 12694
		[HideInInspector]
		public TISpaceShipTemplate constructionManagerSelectedDesign;

		// Token: 0x04003197 RID: 12695
		[HideInInspector]
		public ShipConstructionQueueItem constructionManagerSelectedQueueItem;

		// Token: 0x04003198 RID: 12696
		private List<TINaturalSpaceObjectState> constructionBodies = new List<TINaturalSpaceObjectState>();

		// Token: 0x04003199 RID: 12697
		private List<TINaturalSpaceObjectState> refitBodies = new List<TINaturalSpaceObjectState>();

		// Token: 0x0400319A RID: 12698
		public List<TISpaceShipState> multiSelectedRefitShips = new List<TISpaceShipState>();

		// Token: 0x02001063 RID: 4195
		public class WeaponMountLocation
		{
			// Token: 0x04006329 RID: 25385
			public List<int> mountSize = new List<int>();

			// Token: 0x0400632A RID: 25386
			public List<WeaponClass> weaponClass = new List<WeaponClass>();

			// Token: 0x0400632B RID: 25387
			public List<bool> isNose = new List<bool>();
		}
	}
}
