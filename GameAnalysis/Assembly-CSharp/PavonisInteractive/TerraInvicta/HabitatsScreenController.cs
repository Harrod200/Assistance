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
using PavonisInteractive.TerraInvicta.Systems.Habs;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200086F RID: 2159
	public class HabitatsScreenController : CanvasControllerBase, IHabitatsPreviewer, IInfoScreen, ICanvas
	{
		// Token: 0x17000ECF RID: 3791
		// (get) Token: 0x0600502A RID: 20522 RVA: 0x00229FD0 File Offset: 0x002281D0
		public HabGridCell selectedModule
		{
			get
			{
				if (!this.habToDisplay.IsStation)
				{
					return this.selectedBaseModule;
				}
				return this.selectedStationModule;
			}
		}

		// Token: 0x0600502B RID: 20523 RVA: 0x00229FEC File Offset: 0x002281EC
		public override void Initialize()
		{
			base.Initialize();
			this.CacheComponents();
			this.habListMasterObject.SetActive(true);
			this.habPreviewInfoPanel.SetActive(false);
			this.moduleSelectionPanel.SetActive(false);
			this.habManageButtonText.SetText(Loc.T("UI.Habs.Manage"));
			this.closeHabManageButtonText.SetText(Loc.T("UI.Habs.BackToPreview"));
			this.manageHabTemplatesButtonText.SetText(Loc.T("UI.Habs.ViewHabTemplates"));
			this.quickBuildText.SetText(Loc.T("UI.Habs.QuickBuildToggle"));
			this.quickBuildTooltip.SetText("BodyText", Loc.T("UI.Habs.QuickBuildTip", new object[] { TemplateManager.global.boostInlineSpritePath }));
			this.quickBuildWithBoostText.SetText(Loc.T("UI.Habs.QuickBuildWithBoostToggle", new object[] { TemplateManager.global.boostInlineSpritePath }));
			this.manageHabTemplatesHeader.SetText(Loc.T("UI.Habs.FactionHabTemplatesHeader"));
			this.ChangeModuleMode(true);
			this.moduleInstalledText.SetText(Loc.T("UI.Habs.Installed"));
			this.resourceSummaryTitleLine.SetText(Loc.T("UI.Habs.ResourceSummaryTitle"));
			this.quickBuildToggle.isOn = TIGlobalValuesState.GlobalValues.habQuickBuildToggle;
			this.quickBuildWithBoostToggle.isOn = TIGlobalValuesState.GlobalValues.habQuickBuildWithBoostToggle;
			this.powerReportTextObject.SetActive(false);
			this.manageHabTemplatesPanel.SetActive(false);
			this.habSiteProductivityPanel.SetActive(false);
			this.maxTierTooltip.SetDelegate("BodyText", delegate
			{
				if (this.habToDisplay.maxTier > this.habToDisplay.tier)
				{
					return Loc.T("UI.Hab.MaxTier", new object[] { this.habToDisplay.maxTier });
				}
				return Loc.T("UI.Hab.AtMaxTier");
			});
			this.selectedHabGravity.SetText(string.Empty);
			this.selectedHabCrew.SetText(string.Empty);
			this.selectedHabCrewDisplayObject.SetActive(false);
			this.gravityDisplayObject.SetActive(false);
			this.maxTierIcon.gameObject.SetActive(false);
			this.habinfoIconDropdown.gameObject.SetActive(false);
			this.habSubtitleObject.SetActive(false);
			this.massTemplatesHeaderText.SetText(Loc.T("UI.Habs.MassApplyTemplateHeader"));
			this.applyingMassTemplates = false;
			this.massTemplatesHabIconSelectionDropdown.gameObject.SetActive(false);
			this.AddListeners();
		}

		// Token: 0x0600502C RID: 20524 RVA: 0x0022A21C File Offset: 0x0022841C
		public override void Show()
		{
			base.Show();
			this.primaryHabitatsCanvas.enabled = true;
			this.secondaryHabitatsCanvas.enabled = true;
			GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<HabPowerManagementUpdated>(new EventManager.EventDelegate<HabPowerManagementUpdated>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null, null, false, false);
			GameControl.eventManager.AddListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnFactionResourcesUpdated), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<HabModuleUnlocked>(new EventManager.EventDelegate<HabModuleUnlocked>(this.OnHabModuleUnlocked), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.SetDataDirty), null, null, true, false);
			this.SetToPreviewView();
			this.SetEmptyHabView();
			this.UpdateHabLists();
			this.PreviewHab();
			if (GameControl.control.activePlayer.habs.Count > 0)
			{
				this.HabScreenMainUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_HabScreenCanvas, false, true);
			}
		}

		// Token: 0x0600502D RID: 20525 RVA: 0x0022A3B0 File Offset: 0x002285B0
		public override void Hide()
		{
			this.habToDisplay = null;
			GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.SetDataDirty), null);
			GameControl.eventManager.RemoveListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.SetDataDirty), null);
			GameControl.eventManager.RemoveListener<HabPowerManagementUpdated>(new EventManager.EventDelegate<HabPowerManagementUpdated>(this.SetDataDirty), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.SetDataDirty), null);
			GameControl.eventManager.RemoveListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null);
			GameControl.eventManager.RemoveListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnFactionResourcesUpdated), null);
			GameControl.eventManager.RemoveListener<HabModuleUnlocked>(new EventManager.EventDelegate<HabModuleUnlocked>(this.OnHabModuleUnlocked), null);
			GameControl.eventManager.RemoveListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.SetDataDirty), null);
			GameControl.eventManager.RemoveListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.SetDataDirty), null);
			GameControl.eventManager.RemoveListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.SetDataDirty), null);
			GameControl.eventManager.RemoveListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.SetDataDirty), null);
			this.managingHab = false;
			this.HabScreenMainUITutorialController.HideTutorial();
			this.HabScreenManagementUITutorialController.HideTutorial();
			base.Hide();
		}

		// Token: 0x0600502E RID: 20526 RVA: 0x0022A4E4 File Offset: 0x002286E4
		public override void Refresh()
		{
			if (this.habToDisplay != null)
			{
				if (this.habDisplayDataDirty)
				{
					this.RefreshCanvas();
					this.habDisplayDataDirty = false;
				}
				if (RectTransformUtility.RectangleContainsScreenPoint(this.habScrollViewRectTransform, Input.mousePosition) && this.habitatsScreenPreviewMouseOverTracker.IsPointerHovering && !UIMagnifier.IsMagnifierActive)
				{
					this.habDisplayZoomSlider.value += Input.mouseScrollDelta.y * 10f;
				}
			}
		}

		// Token: 0x0600502F RID: 20527 RVA: 0x0022A564 File Offset: 0x00228764
		private bool IsMousedOverPreviewScrollRect()
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			foreach (RaycastResult raycastResult in list)
			{
				if (raycastResult.gameObject.Equals(this.habDisplayScrollView.viewport.gameObject))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005030 RID: 20528 RVA: 0x0022A600 File Offset: 0x00228800
		public void CloseInfoScreen(bool toggle = false)
		{
			this.habToDisplay = null;
			this.managingHab = false;
			if (this.primaryHabitatsCanvas != null)
			{
				this.primaryHabitatsCanvas.enabled = false;
			}
			if (this.secondaryHabitatsCanvas != null)
			{
				this.secondaryHabitatsCanvas.enabled = false;
			}
			this.SetToPreviewView();
			this.SetEmptyHabView();
			base.canvasManager.HideInfoScreen<HabitatsScreenController>(toggle);
		}

		// Token: 0x06005031 RID: 20529 RVA: 0x0022A667 File Offset: 0x00228867
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.habsMainPanel.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, (float)((base.VerticalScaleValueLimit() >= 940f) ? (-100) : (-85)));
		}

		// Token: 0x06005032 RID: 20530 RVA: 0x0022A6A4 File Offset: 0x002288A4
		private void OnHabDetailRequested(HabDetailRequested e)
		{
			if (!TIGameState.Valid(e.hab))
			{
				return;
			}
			this.habToDisplay = e.hab;
			if (this.habList_FilterForHabType != HabType.Any && this.habList_FilterForHabType != e.hab.habType)
			{
				if (!this.basesToggle.isOn)
				{
					this.basesToggle.SetIsOnWithoutNotify(true);
					this.OnBasesToggleClicked(false);
				}
				if (!this.stationsToggle.isOn)
				{
					this.stationsToggle.SetIsOnWithoutNotify(true);
					this.OnStationsToggleClicked(false);
				}
			}
			if (this.habList_FilterForFaction != null && this.habList_FilterForFaction != this.habToDisplay.coreFaction)
			{
				this.factionsDropdown.value = 0;
			}
			this.habList_FilterForSpaceObject != null;
			base.canvasManager.ShowInfoScreen<HabitatsScreenController>();
			GeneralControlsController.Singleton.EnableFinderCanvas(false);
			this.habToDisplay = e.hab;
			if (e.manage && this.habToDisplay.ref_factions.Contains(base.activePlayer))
			{
				this.selectedStationModule = null;
				this.selectedBaseModule = null;
				this.ManageHab();
			}
			else
			{
				this.SelectHabFromMenu(this.habToDisplay);
			}
			this.ResetHabDisplayZoom();
		}

		// Token: 0x06005033 RID: 20531 RVA: 0x0022A7D4 File Offset: 0x002289D4
		private void CacheComponents()
		{
			this.exitButton = base.gameObject.GetComponentOnChild<Button>("ExitButton");
			List<TIFactionState> list = (from x in GameStateManager.AllFactions()
				orderby x == GameControl.control.activePlayer descending, x.IsAlienFaction
				select x).ToList<TIFactionState>();
			this.habList_FilterForHabType = HabType.Any;
			this.factionsDropdown.captionText.SetText(Loc.T("UI.Habs.SelectFaction"));
			this.locationDropdown.captionText.SetText(Loc.T("UI.Habs.SelectLocation"));
			using (List<Button>.Enumerator enumerator = this.sortButtons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Button button = enumerator.Current;
					button.onClick.AddListener(delegate
					{
						this.OnClickHabSortButton(button);
					});
				}
			}
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.AllFactions")
			};
			TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.AllHumanFactions")
			};
			this.factionsDropdown.ClearOptions();
			this.factionsDropdown.options.Add(optionData);
			this.factionsDropdown.options.Add(optionData2);
			this.factionDropdownLookup = new Dictionary<int, TIFactionState>
			{
				{ 0, null },
				{ 1, null }
			};
			int num = 2;
			foreach (TIFactionState tifactionState in list)
			{
				TMP_Dropdown.OptionData optionData3 = new TMP_Dropdown.OptionData
				{
					text = tifactionState.displayNameCapitalized,
					image = tifactionState.factionIcon64UI
				};
				this.factionsDropdown.options.Add(optionData3);
				this.factionDropdownLookup.Add(num++, tifactionState);
			}
			this.factionsDropdown.value = 0;
			this.locationDropdown.ClearOptions();
			num = 0;
			this.locationDropdownLookup = new Dictionary<int, TISpaceBodyState>();
			foreach (string text in TargetSelectionTool.primaryNavigatorBodyTemplateNames)
			{
				TISpaceBodyState tispaceBodyState = GameStateManager.FindByTemplate<TISpaceBodyState>(text, false);
				if (tispaceBodyState != null)
				{
					TMP_Dropdown.OptionData optionData4 = new TMP_Dropdown.OptionData();
					switch (tispaceBodyState.objectType)
					{
					case SpaceObjectType.Star:
						optionData4.text = Loc.T("UI.Habs.AllLocations");
						break;
					case SpaceObjectType.Planet:
					case SpaceObjectType.PlanetaryMoon:
					case SpaceObjectType.AsteroidalMoon:
						goto IL_028C;
					case SpaceObjectType.DwarfPlanet:
					case SpaceObjectType.Asteroid:
					case SpaceObjectType.Comet:
						if (GameStateManager.InnerSystemAsteroids(true).Contains(tispaceBodyState))
						{
							optionData4.text = Loc.T("UI.Habs.InnerSystemAsteroids");
						}
						else if (GameStateManager.InnerAsteroidBelt(true).Contains(tispaceBodyState))
						{
							optionData4.text = Loc.T("UI.Habs.InnerBelt");
						}
						else if (GameStateManager.MidAsteroidBelt(true).Contains(tispaceBodyState))
						{
							optionData4.text = Loc.T("UI.Habs.MidBelt");
						}
						else if (GameStateManager.OuterAsteroidBelt(true).Contains(tispaceBodyState))
						{
							optionData4.text = Loc.T("UI.Habs.FarBelt");
						}
						else if (GameStateManager.Centaurs(true).Contains(tispaceBodyState))
						{
							optionData4.text = Loc.T("UI.Habs.Centaurs");
						}
						else if (GameStateManager.KuiperBeltObjects(true).Contains(tispaceBodyState))
						{
							optionData4.text = Loc.T("UI.Habs.KBO");
						}
						else
						{
							optionData4.text = Loc.T("UI.Habs.Other");
						}
						break;
					default:
						goto IL_028C;
					}
					IL_0382:
					optionData4.image = tispaceBodyState.icon;
					this.locationDropdown.options.Add(optionData4);
					this.locationDropdownLookup.Add(num, tispaceBodyState);
					num++;
					continue;
					IL_028C:
					optionData4.text = tispaceBodyState.displayName;
					goto IL_0382;
				}
			}
			this.habIconPaths = new List<string>
			{
				"",
				TemplateManager.global.pathMoneyIcon,
				TemplateManager.global.pathInfluenceIcon,
				TemplateManager.global.pathOpsIcon,
				TemplateManager.global.pathResearchIcon,
				TemplateManager.global.pathHabResupplyIcon,
				"icons_2d/ICO_hab_defense",
				TemplateManager.global.pathSpaceCombatScoreIcon,
				TemplateManager.global.pathSpaceAssaultScoreIcon,
				TemplateManager.global.pathFleetIcon,
				TemplateManager.global.pathHabShipyardIcon,
				TemplateManager.global.pathHabModuleConstructionIcon,
				TemplateManager.global.pathSpaceMiningIcon,
				TemplateManager.global.pathWaterIcon,
				TemplateManager.global.pathVolatilesIcon,
				TemplateManager.global.pathBaseMetalsIcon,
				TemplateManager.global.pathNobleMetalsIcon,
				TemplateManager.global.pathFissilesIcon,
				TemplateManager.global.pathAntimatterIcon,
				TemplateManager.global.pathMissionControlIcon,
				TemplateManager.global.pathProjectsIcon,
				TemplateManager.global.pathEnergyIcon,
				TemplateManager.global.pathMaterialsIcon,
				TemplateManager.global.pathSocialScienceIcon,
				TemplateManager.global.pathInformationScienceIcon,
				TemplateManager.global.pathLifeScienceIcon,
				TemplateManager.global.pathMilitaryScienceIcon,
				TemplateManager.global.pathSpaceScienceIcon,
				TemplateManager.global.pathXenologyIcon,
				TemplateManager.global.pathColonyIcon,
				TemplateManager.global.pathSunStylized
			};
			for (int i = 1; i < this.habIconPaths.Count; i++)
			{
				this.habinfoIconDropdown.options.Add(new TMP_Dropdown.OptionData());
				this.habinfoIconDropdown.options[i].image = GameControl.assetLoader.LoadAsset<Sprite>(this.habIconPaths[i]);
				this.habIconFilterDropdown.options.Add(new TMP_Dropdown.OptionData());
				this.habIconFilterDropdown.options[i].image = GameControl.assetLoader.LoadAsset<Sprite>(this.habIconPaths[i]);
				this.massTemplatesHabIconSelectionDropdown.options.Add(new TMP_Dropdown.OptionData());
				this.massTemplatesHabIconSelectionDropdown.options[i].image = GameControl.assetLoader.LoadAsset<Sprite>(this.habIconPaths[i]);
			}
			this.availableModuleDictionary = new Dictionary<TIHabModuleTemplate, HabModuleListItem>();
			this.installedModuleDictionary = new Dictionary<TIHabModuleState, HabModuleListItem>();
			this.availableModuleListItems = new List<HabModuleListItem>();
			this.installedModuleListItems = new List<HabModuleListItem>();
			this.CacheHabDisplay();
			this.CacheModuleListItems();
			this.CacheModuleManagementPanel();
			this.incomeGridHeader.SetText(Loc.T("UI.Habs.Incomes"));
			this.constructionCostHeader.SetText(Loc.T("UI.Habs.ConstructionCost"));
			this.supportCostHeader.SetText(Loc.T("UI.Habs.SupportCost"));
			this.upgradeHeader.SetText(Loc.T("UI.Habs.UpgradeHeader"));
			this.upgradeModuleButtonText.SetText(Loc.T("UI.Habs.Upgrade"));
			this.moduleUpgradeAllOfTypeButtonText.SetText(Loc.T("UI.Habs.UpgradeAllOfType"));
			this.habZoomText.SetText(Loc.T("UI.Habs.Zoom"));
			this.mainHeaderText.SetText(Loc.T("UI.Habs.MainHeader"));
			this.listHeaderText.SetText(Loc.T("UI.Habs.SelectAHab"));
			this.availableModulesHeaderText.SetText(Loc.T("UI.Habs.AvailableModules"));
			this.installedModulesHeaderText.SetText(Loc.T("UI.Habs.InstalledModules"));
			this.habTemplateTitleText.SetText(Loc.T("UI.Habs.TemplateTitle"));
			this.saveHabButtonText.SetText(Loc.T("UI.Habs.SaveTemplate"));
			this.manageHabTemplatesButtonText.SetText(Loc.T("UI.Habs.ViewHabTemplates"));
			this.globalRebuildButtonText.SetText(Loc.T("UI.Habs.RebuildAll"));
			this.globalUpgradeButtonText.SetText(Loc.T("UI.Habs.UpgradeAll"));
			this.PowerAllButtonText.SetText(Loc.T("UI.Habs.PowerAllButton"));
			this.DecommissionHabButtonText.SetText(Loc.T("UI.Habs.DecommissionHabButton"));
			this.connectors = new Dictionary<string, Sprite>();
			foreach (string text2 in this.connectorSpritePaths)
			{
				Sprite sprite = GameControl.assetLoader.LoadAsset<Sprite>(text2);
				this.connectors.Add(sprite.name, sprite);
			}
			this.connectorSwaps = new Dictionary<string, string>
			{
				{ "station_connector_A", "station_Alien_Connector" },
				{ "station_connector_B", "station_Alien_T_Connector" },
				{ "station_connector_C", "station_alien_Connector_C" },
				{ "base_connector_A", "base_connector_A_alien" },
				{ "base_connector_B", "base_connector_B_alien" },
				{ "base_connector_C", "base_connector_C_alien" },
				{ "base_connector_D", "base_connector_D_alien" }
			};
			this.managementQueryCancelButton.interactable = true;
			this.managementQueryCancelButtonObject.SetActive(true);
			this.managementQueryCancelButtonText.SetText(Loc.T("UI.Habs.CancelManagementQueryButtonText"));
			this.managementQueryToggleText.SetText(Loc.T("UI.Habs.ReplaceAllModules"));
		}

		// Token: 0x06005034 RID: 20532 RVA: 0x0022B1C4 File Offset: 0x002293C4
		private void CacheHabDisplay()
		{
			this.habDisplayScrollView = base.gameObject.GetComponentOnChild<ScrollRect>("HabDisplayScrollView");
			this.habitatsScreenPreviewMouseOverTracker = this.habDisplayScrollView.viewport.gameObject.GetComponentOnChild<UIPointerHoverTracker>("Viewport");
			this.habScrollViewRectTransform = this.habDisplayScrollView.gameObject.GetComponent<RectTransform>();
			this.habDisplayRectTransform = base.gameObject.GetComponentOnChild<RectTransform>("HabScrolledDisplay");
			this.habDisplayZoomSlider = base.gameObject.GetComponentOnChild<Slider>("HabZoomSlider");
			this.noHabSelected = base.gameObject.GetComponentOnChild<Transform>("NoHabSelected").gameObject;
			this.noHabSelectedText.SetText(Loc.T("UI.Habs.NoHabSelected"));
			this.CacheStationDisplay();
			this.CacheBaseDisplay();
		}

		// Token: 0x06005035 RID: 20533 RVA: 0x0022B288 File Offset: 0x00229488
		private void CacheStationDisplay()
		{
			this.stationDisplayCanvas = base.gameObject.GetComponentOnChild<Canvas>("StationDisplay");
			this.stationDisplayCanvas.gameObject.SetActive(true);
			this.stationDisplayGridLayout = base.gameObject.GetComponentOnChild<GridLayoutGroup>("StationGridDisplay");
			this.torusGrid = base.gameObject.GetComponentOnChild<GridLayoutGroup>("Toruses");
			this.torus1_2 = this.torusGrid.gameObject.GetComponentOnChild<Image>("Torus_1_2");
			this.torus2_3 = this.torusGrid.gameObject.GetComponentOnChild<Image>("Torus_2_3");
			this.torus3_4 = this.torusGrid.gameObject.GetComponentOnChild<Image>("Torus_3_4");
			this.torus4_1 = this.torusGrid.gameObject.GetComponentOnChild<Image>("Torus_4_1");
			this.stationDisplayGridRectTransform = this.stationDisplayGridLayout.GetComponent<RectTransform>();
			this.torusGridRectTransform = this.torusGrid.GetComponent<RectTransform>();
			this.stationGridCells = base.GetComponentsInChildren<StationGridCell>(true);
			this.stationCellDictionary = new Dictionary<string, StationGridCell>();
			for (int i = 0; i < this.stationGridCells.Length; i++)
			{
				this.stationGridCells[i].SetPreviewer(this);
				this.stationGridCells[i].SetControllerBase(this);
				this.stationCellDictionary.Add(this.stationGridCells[i].name, this.stationGridCells[i]);
			}
		}

		// Token: 0x06005036 RID: 20534 RVA: 0x0022B3E0 File Offset: 0x002295E0
		private void CacheBaseDisplay()
		{
			this.baseDisplayCanvas = base.gameObject.GetComponentOnChild<Canvas>("BaseDisplay");
			this.baseDisplayCanvas.gameObject.SetActive(true);
			this.baseDisplayGridLayout = base.gameObject.GetComponentOnChild<GridLayoutGroup>("BaseGridDisplay");
			this.baseDisplayGridRectTransform = this.baseDisplayGridLayout.GetComponent<RectTransform>();
			this.baseSurfaceImage = base.gameObject.GetComponentOnChild<Image>("BaseSurfaceImage");
			this.baseSurfaceRectTransform = this.baseSurfaceImage.rectTransform;
			this.baseGridCells = base.GetComponentsInChildren<BaseGridCell>(true);
			this.baseCellDictionary = new Dictionary<string, BaseGridCell>();
			for (int i = 0; i < this.baseGridCells.Length; i++)
			{
				this.baseGridCells[i].SetPreviewer(this);
				this.baseGridCells[i].SetControllerBase(this);
				this.baseCellDictionary.Add(this.baseGridCells[i].name, this.baseGridCells[i]);
			}
		}

		// Token: 0x06005037 RID: 20535 RVA: 0x0022B4CC File Offset: 0x002296CC
		private void CacheModuleListItems()
		{
			this.allTiersButtonText.SetText(Loc.T("UI.Habs.All"));
			this.allBenefitsButtonText.SetText(Loc.T("UI.Habs.All"));
			this.cachedButtonSprite = this.tierButtons[0].gameObject.GetComponent<Image>().sprite;
			GameObject gameObject = base.gameObject.GetComponentOnChild<Transform>("ModuleInfoList").gameObject;
			this.moduleInfoListItems = gameObject.GetComponentsInChildren<HabInfoListItem>();
			for (int i = 0; i < this.moduleInfoListItems.Length; i++)
			{
				string name = this.moduleInfoListItems[i].name;
				if (name != null && name == "Power")
				{
					this.modulePower = this.moduleInfoListItems[i];
					this.modulePowerToggle = this.modulePower.gameObject.GetComponentOnChild<Button>("ModulePowerToggle");
				}
			}
		}

		// Token: 0x06005038 RID: 20536 RVA: 0x0022B59C File Offset: 0x0022979C
		private void CacheModuleManagementPanel()
		{
			this.confirmModulePopupCanvas = base.gameObject.GetComponentOnChild<Canvas>("ModuleConfirm");
			this.confirmModulePopupCanvas.gameObject.SetActive(true);
			this.confirmModulePurchaseEarth = this.confirmModulePopupCanvas.gameObject.GetComponentOnChild<Transform>("ConfirmPurchaseEarth").gameObject;
			this.confirmModulePurchaseEarthButton = this.confirmModulePurchaseEarth.GetComponent<Button>();
			this.confirmModulePurchaseSpace = this.confirmModulePopupCanvas.gameObject.GetComponentOnChild<Transform>("ConfirmPurchaseSpace").gameObject;
			this.confirmModulePurchaseSpaceButton = this.confirmModulePurchaseSpace.GetComponent<Button>();
			this.confirmModulePurchaseFailure = this.confirmModulePopupCanvas.gameObject.GetComponentOnChild<Transform>("ConfirmFailure").gameObject;
			this.confirmModulePurchaseFailureButton = this.confirmModulePurchaseFailure.GetComponent<Button>();
			this.confirmModulePurchaseFailureButtonText.SetText(Loc.T("UI.Habs.Acknowledge"));
			this.cancelModulePurchaseButtonText.SetText(Loc.T("UI.Habs.Cancel"));
			this.cancelModulePurchase = this.confirmModulePopupCanvas.gameObject.GetComponentOnChild<Transform>("Cancel").gameObject;
			this.cancelModulePurchaseButton = this.cancelModulePurchase.GetComponent<Button>();
			this.confirmModulePopupCanvas.enabled = false;
			this.moduleSelectionPanel.SetActive(false);
			this.nextHabButtonsContainer.SetActive(false);
			this.previousHabButtonsContainer.SetActive(false);
		}

		// Token: 0x06005039 RID: 20537 RVA: 0x0022B6F0 File Offset: 0x002298F0
		private void AddListeners()
		{
			GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<HabPowerManagementUpdated>(new EventManager.EventDelegate<HabPowerManagementUpdated>(this.SetDataDirty), null, null, true, false);
			GameControl.eventManager.AddListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.SetDataDirty), null, null, true, false);
			this.exitButton.onClick.AddListener(new UnityAction(this.OnExitButtonClick));
			this.confirmModulePurchaseEarthButton.onClick.AddListener(new UnityAction(this.OnConfirmBuildModuleEarth));
			this.confirmModulePurchaseSpaceButton.onClick.AddListener(new UnityAction(this.OnConfirmBuildModuleSpace));
			this.confirmModulePurchaseFailureButton.onClick.AddListener(new UnityAction(this.OnConfirmModuleFailure));
			this.cancelModulePurchaseButton.onClick.AddListener(new UnityAction(this.OnCancelBuildModule));
			this.moduleUpgradeButton.onClick.AddListener(new UnityAction(this.OnModuleUpgrade));
			GameControl.eventManager.AddListener<HabDetailRequested>(new EventManager.EventDelegate<HabDetailRequested>(this.OnHabDetailRequested), null, null, true, false);
		}

		// Token: 0x0600503A RID: 20538 RVA: 0x0022B827 File Offset: 0x00229A27
		private void SetDataDirty(HabModuleConstructionStatusChange e)
		{
			if (e.sector.hab == this.habToDisplay)
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x0600503B RID: 20539 RVA: 0x0022B847 File Offset: 0x00229A47
		private void SetDataDirty(SectorAssignedToFaction e)
		{
			if (e.sector.hab == this.habToDisplay)
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x0600503C RID: 20540 RVA: 0x0022B867 File Offset: 0x00229A67
		private void SetDataDirty(HabPowerManagementUpdated e)
		{
			this.SetDataDirty();
			if (e.hab == this.habToDisplay)
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x0600503D RID: 20541 RVA: 0x0022B888 File Offset: 0x00229A88
		private void SetDataDirty(HabModuleDestroyed e)
		{
			this.SetDataDirty();
			if (e.habModule.hab == this.habToDisplay)
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x0600503E RID: 20542 RVA: 0x0022B8AE File Offset: 0x00229AAE
		private void SetDataDirty(BeginHabAssault e)
		{
			if (e.target == this.habToDisplay)
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x0600503F RID: 20543 RVA: 0x0022B8C9 File Offset: 0x00229AC9
		private void SetDataDirty(EndHabAssault e)
		{
			if (e.target == this.habToDisplay)
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x06005040 RID: 20544 RVA: 0x0022B8E4 File Offset: 0x00229AE4
		private void SetDataDirty(BeginBombardment e)
		{
			if (e.target == this.habToDisplay)
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x06005041 RID: 20545 RVA: 0x0022B8FF File Offset: 0x00229AFF
		private void SetDataDirty(EndBombardment e)
		{
			if (e.target == this.habToDisplay)
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x06005042 RID: 20546 RVA: 0x0022B91A File Offset: 0x00229B1A
		private void SetDataDirty()
		{
			this.habDisplayDataDirty = true;
		}

		// Token: 0x06005043 RID: 20547 RVA: 0x0022B924 File Offset: 0x00229B24
		private void OnHabDestroyed(HabDestroyed e)
		{
			if (e.hab == this.habToDisplay && this.managingHab)
			{
				this.habToDisplay = null;
				this.UpdateHabPreviewDisplay();
				this.CloseHabManagement();
			}
			this.SetHabModelData(e.hab);
			this.SetDataDirty();
		}

		// Token: 0x06005044 RID: 20548 RVA: 0x0022B974 File Offset: 0x00229B74
		private void OnFactionResourcesUpdated(FactionResourcesUpdated e)
		{
			if (this.managingHab)
			{
				int day = base.gameTime.currentTime.day;
				TIDateTime tidateTime = this.lastResourceUpdateCheck;
				int? num = ((tidateTime != null) ? new int?(tidateTime.day) : null);
				if (!((day == num.GetValueOrDefault()) & (num != null)))
				{
					this.UpdateModuleList(this.habToDisplay.habType);
					this.UpdateModulePreviewText(this.showAvailableModules, false);
					this.lastResourceUpdateCheck = base.gameTime.currentTime;
				}
				if (this.managementQueryObject.activeInHierarchy)
				{
					this.managementQueryObject.SetActive(false);
				}
				if (this.confirmModulePopupCanvas.enabled)
				{
					this.CloseModuleBuildPanel();
				}
			}
		}

		// Token: 0x06005045 RID: 20549 RVA: 0x0022BA2A File Offset: 0x00229C2A
		private void OnHabModuleUnlocked(HabModuleUnlocked e)
		{
			if (this.managingHab)
			{
				this.UpdateModuleList(this.habToDisplay.habType);
				return;
			}
			this.UpdateHabLists();
		}

		// Token: 0x06005046 RID: 20550 RVA: 0x0022BA4C File Offset: 0x00229C4C
		public HabitatsScreenController GetController()
		{
			return this;
		}

		// Token: 0x17000ED0 RID: 3792
		// (get) Token: 0x06005047 RID: 20551 RVA: 0x0022BA4F File Offset: 0x00229C4F
		public bool PlayerHab
		{
			get
			{
				return this.habToDisplay != null && this.habToDisplay.faction == base.activePlayer;
			}
		}

		// Token: 0x06005048 RID: 20552 RVA: 0x0022BA77 File Offset: 0x00229C77
		public void OnHabZoomSlider()
		{
			this.OnHabZoomSliderSet();
		}

		// Token: 0x06005049 RID: 20553 RVA: 0x0022BA80 File Offset: 0x00229C80
		public void OnHabZoomSliderIncrease()
		{
			this.habDisplayZoomSlider.SetValueWithoutNotify(Mathf.Min(this.habDisplayZoomSlider.maxValue, this.habDisplayZoomSlider.value + (this.habDisplayZoomSlider.maxValue - this.habDisplayZoomSlider.minValue) * 0.1f));
			this.OnHabZoomSliderSet();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverNonButton", false, false);
		}

		// Token: 0x0600504A RID: 20554 RVA: 0x0022BAE4 File Offset: 0x00229CE4
		public void OnHabZoomSliderDecrease()
		{
			this.habDisplayZoomSlider.SetValueWithoutNotify(Mathf.Max(this.habDisplayZoomSlider.minValue, this.habDisplayZoomSlider.value - (this.habDisplayZoomSlider.maxValue - this.habDisplayZoomSlider.minValue) * 0.1f));
			this.OnHabZoomSliderSet();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverNonButton", false, false);
		}

		// Token: 0x0600504B RID: 20555 RVA: 0x0022BB48 File Offset: 0x00229D48
		private void OnHabZoomSliderSet()
		{
			if (this.habToDisplay != null)
			{
				float num = this.habDisplayZoomSlider.value / 100f;
				float num2 = this.habDisplayCellSizeMin + num * (this.habDisplayCellSizeMax - this.habDisplayCellSizeMin);
				Vector2 vector = new Vector2(num2, num2);
				if (this.habToDisplay.IsBase)
				{
					this.baseDisplayGridLayout.cellSize = vector;
				}
				else
				{
					this.stationDisplayGridLayout.cellSize = vector;
					this.torusGrid.cellSize = 3.5f * this.stationDisplayGridLayout.cellSize;
				}
				float num3 = num2 / 5f;
				Vector2 vector2 = new Vector2(num3, num3);
				if (this.habToDisplay.IsStation)
				{
					for (int i = 0; i < this.stationGridCells.Length; i++)
					{
						this.stationGridCells[i].SetGridCellSize(vector2);
					}
				}
				else
				{
					for (int j = 0; j < this.baseGridCells.Length; j++)
					{
						this.baseGridCells[j].SetGridCellSize(vector2);
					}
				}
				this.UpdateHabFrameTransforms(vector);
			}
		}

		// Token: 0x0600504C RID: 20556 RVA: 0x0022BC54 File Offset: 0x00229E54
		private void UpdateHabFrameTransforms(Vector2 sizeDelta = default(Vector2))
		{
			if (sizeDelta == default(Vector2))
			{
				sizeDelta = (this.habToDisplay.IsStation ? this.stationDisplayGridRectTransform.sizeDelta : this.baseDisplayGridRectTransform.sizeDelta);
			}
			this.habDisplayRectTransform.sizeDelta = sizeDelta;
			if (this.habToDisplay.IsStation)
			{
				this.torusGridRectTransform.sizeDelta = sizeDelta;
				return;
			}
			if (sizeDelta == default(Vector2))
			{
				this.baseSurfaceRectTransform.sizeDelta = new Vector2(2048f, 2048f);
				return;
			}
			this.baseSurfaceRectTransform.sizeDelta = sizeDelta * 25.2f;
		}

		// Token: 0x0600504D RID: 20557 RVA: 0x0022BD04 File Offset: 0x00229F04
		private void ResetHabDisplayZoom()
		{
			if (this.habToDisplay != null)
			{
				this.habDisplayScrollView.verticalNormalizedPosition = 0.5f;
				this.habDisplayScrollView.horizontalNormalizedPosition = 0.5f;
				switch (this.habToDisplay.tier)
				{
				case 1:
					this.habDisplayZoomSlider.value = (this.habToDisplay.IsStation ? 160f : 70f);
					break;
				case 2:
					this.habDisplayZoomSlider.value = (this.habToDisplay.IsStation ? 18f : 20f);
					break;
				case 3:
					this.habDisplayZoomSlider.value = (this.habToDisplay.IsStation ? 15f : 10f);
					break;
				}
			}
			else
			{
				this.habDisplayZoomSlider.value = 10f;
			}
			this.OnHabZoomSliderSet();
		}

		// Token: 0x0600504E RID: 20558 RVA: 0x0022BDEF File Offset: 0x00229FEF
		public void OnResetTutorialClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.HabScreenMainUITutorialController.ResetTutorial(false);
			this.HabScreenManagementUITutorialController.ResetTutorial(false);
		}

		// Token: 0x0600504F RID: 20559 RVA: 0x0022BE15 File Offset: 0x0022A015
		public void OnCloseAndPlayClicked()
		{
			this.OnExitButtonClick();
			base.gameTime.Play();
		}

		// Token: 0x06005050 RID: 20560 RVA: 0x0022BE28 File Offset: 0x0022A028
		private void OnExitButtonClick()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.CloseInfoScreen(false);
		}

		// Token: 0x06005051 RID: 20561 RVA: 0x0022BE3D File Offset: 0x0022A03D
		private void UpdateHabLists()
		{
			this.SetHabModelData(null);
			this.UpdateHabSort(-1);
			if (!this.applyingMassTemplates)
			{
				this.HighlightSelectedHab(this.habToDisplay);
			}
			base.StartCoroutine(this.DelayedHabListHeaderUpdate());
		}

		// Token: 0x06005052 RID: 20562 RVA: 0x0022BE6E File Offset: 0x0022A06E
		private IEnumerator DelayedHabListHeaderUpdate()
		{
			yield return new WaitForEndOfFrame();
			this.habListScrollHeader.enabled = this.habListScrollBar.activeSelf;
			yield break;
		}

		// Token: 0x06005053 RID: 20563 RVA: 0x0022BE7D File Offset: 0x0022A07D
		private void UpdateHabModelData()
		{
			this.habListAdapter.SetItems(this.habModels);
		}

		// Token: 0x06005054 RID: 20564 RVA: 0x0022BE90 File Offset: 0x0022A090
		private void SetHabModelData(TIHabState destroyedHab = null)
		{
			List<TINaturalSpaceObjectState> list = new List<TINaturalSpaceObjectState>();
			TISpaceBodyState tispaceBodyState = this.habList_FilterForSpaceObject;
			if (tispaceBodyState != null && !tispaceBodyState.isSun)
			{
				list = TINaturalSpaceObjectState.GetFilteredSolarSystemGroupObjects(this.habList_FilterForSpaceObject, true);
				if (this.habList_FilterForSpaceObject.isEarth)
				{
					list.Remove(GameStateManager.Luna());
				}
				else if (this.habList_FilterForSpaceObject.isLuna)
				{
					list.Remove(GameStateManager.Earth());
				}
			}
			this.habModels.Clear();
			foreach (TIHabState tihabState in GameStateManager.IterateByClass<TIHabState>(false))
			{
				if (!tihabState.deleted && (!(destroyedHab != null) || !(tihabState == destroyedHab)))
				{
					HabScreenHabListItemModel habScreenHabListItemModel = new HabScreenHabListItemModel();
					HabScreenHabListItem_Data habScreenHabListItem_Data = new HabScreenHabListItem_Data();
					this.selectedHabList.Contains(tihabState);
					habScreenHabListItem_Data.controller = this;
					habScreenHabListItem_Data.previewer = this;
					habScreenHabListItem_Data.SetData(tihabState);
					HabType habType = this.habList_FilterForHabType;
					bool flag;
					if (habType != HabType.Station)
					{
						flag = habType != HabType.Base || tihabState.IsBase;
					}
					else
					{
						flag = tihabState.IsStation;
					}
					if (flag && this.habList_FilterForTemplate != null)
					{
						flag &= tihabState.CanApplySavedTemplate(this.habList_FilterForTemplate);
					}
					if (flag)
					{
						if (this.habList_FilterForFaction != null)
						{
							flag &= tihabState.coreFaction == this.habList_FilterForFaction;
						}
						else
						{
							flag &= !this.habList_FilterHumanFactionsOnly || !tihabState.coreFaction.IsAlienFaction;
						}
					}
					if (flag)
					{
						TISpaceBodyState tispaceBodyState2 = this.habList_FilterForSpaceObject;
						if (tispaceBodyState2 != null && !tispaceBodyState2.isSun)
						{
							flag &= list.Contains(tihabState.ref_naturalSpaceObject);
						}
					}
					if (flag)
					{
						flag &= base.activePlayer.HasIntelOnSpaceAssetLocation(tihabState);
					}
					if (flag && !string.IsNullOrEmpty(this.habs_nameFilterForHabs))
					{
						flag &= tihabState.displayName.ToLowerInvariant().Contains(this.habs_nameFilterForHabs.ToLowerInvariant());
					}
					if (flag && !string.IsNullOrEmpty(this.habList_FilterForHabIcon))
					{
						flag &= tihabState.customHabIconResource == this.habList_FilterForHabIcon;
					}
					habScreenHabListItem_Data.showInList = flag;
					habScreenHabListItemModel.HabScreenHabListItemData = habScreenHabListItem_Data;
					this.habModels.Add(habScreenHabListItemModel);
				}
			}
		}

		// Token: 0x06005055 RID: 20565 RVA: 0x0022C0EC File Offset: 0x0022A2EC
		public void OnClickHabSortButton(Button clickedButton)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			ColorBlock colorBlock;
			foreach (Button button in this.sortButtons)
			{
				colorBlock = button.colors;
				colorBlock.normalColor = new Color(TIUtilities.UIHighlightColor.r, TIUtilities.UIHighlightColor.g, TIUtilities.UIHighlightColor.b, 0f);
				button.colors = colorBlock;
			}
			colorBlock = clickedButton.colors;
			colorBlock.normalColor = new Color(TIUtilities.UIHighlightColor.r, TIUtilities.UIHighlightColor.g, TIUtilities.UIHighlightColor.b, 1f);
			clickedButton.colors = colorBlock;
		}

		// Token: 0x06005056 RID: 20566 RVA: 0x0022C1BC File Offset: 0x0022A3BC
		public void UpdateHabSort(int sortBy)
		{
			bool flag = true;
			if (sortBy == -1)
			{
				sortBy = this.lastSort;
				flag = false;
			}
			if (flag)
			{
				if (this.lastSort == sortBy)
				{
					this.reverseHabSort = !this.reverseHabSort;
				}
				else
				{
					this.reverseHabSort = false;
				}
			}
			this.currentHabSort = (HabitatsScreenController.SortHabDataBy)sortBy;
			this.lastSort = sortBy;
			switch (this.currentHabSort)
			{
			case HabitatsScreenController.SortHabDataBy.Alfa:
				if (!this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderBy<HabScreenHabListItemModel, string>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.habState.GetDisplayName(base.activePlayer)).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderByDescending<HabScreenHabListItemModel, string>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.habState.GetDisplayName(base.activePlayer)).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.MissionControl:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.MCSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.MCSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Water:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.WaterSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.WaterSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Volatiles:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.VolatilesSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.VolatilesSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Metals:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.MetalsSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.MetalsSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.NobleMetals:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.NobleMetalsSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.NobleMetalsSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Fissiles:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.FissilesSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.FissilesSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Antimatter:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.AntimatterSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.AntimatterSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Exotics:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ExoticsSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ExoticsSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Resupply:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ResupplySortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ResupplySortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Shipyard:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ShipyardSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ShipyardSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.CombatStrength:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.habState.AssaultCombatValue(true) descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.habState.AssaultCombatValue(true)
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Defended:
				if (!this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderByDescending<HabScreenHabListItemModel, bool>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.habState.coreDefended).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderBy<HabScreenHabListItemModel, bool>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.habState.coreDefended).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.UnderConstruction:
				if (!this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderByDescending<HabScreenHabListItemModel, bool>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.ConstructionSortValue).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderBy<HabScreenHabListItemModel, bool>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.ConstructionSortValue).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.SpaceAssaultScore:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.habState.SpaceCombatValue() descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.habState.SpaceCombatValue()
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.CustomIcon:
				if (!this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderByDescending<HabScreenHabListItemModel, string>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.habState.customHabIconResource).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderBy<HabScreenHabListItemModel, string>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.habState.customHabIconResource).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Location:
			{
				Func<HabScreenHabListItemModel, double?> orbitDistanceSelector = delegate(HabScreenHabListItemModel listItem)
				{
					double? num = null;
					TIHabState habState = listItem.HabScreenHabListItemData.habState;
					if (habState.IsStation)
					{
						TIOrbitState ref_orbit = habState.ref_orbit;
						double? num2 = ((ref_orbit != null) ? new double?(ref_orbit.semiMajorAxis_AU) : null);
						TIOrbitState ref_orbit2 = habState.ref_orbit;
						double? num3;
						if (ref_orbit2 == null)
						{
							num3 = null;
						}
						else
						{
							TISpaceObjectState ref_spaceObject = ref_orbit2.ref_spaceObject;
							num3 = ((ref_spaceObject != null) ? new double?(ref_spaceObject.semiMajorAxis_AU) : null);
						}
						num = num2 + num3;
						TIOrbitState ref_orbit3 = habState.ref_orbit;
						bool flag2;
						if (ref_orbit3 == null)
						{
							flag2 = false;
						}
						else
						{
							TISpaceObjectState ref_spaceObject2 = ref_orbit3.ref_spaceObject;
							bool? flag3 = ((ref_spaceObject2 != null) ? new bool?(ref_spaceObject2.isaMoon) : null);
							bool flag4 = true;
							flag2 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
						}
						if (flag2)
						{
							double? num4 = num;
							TINaturalSpaceObjectState barycenter = habState.ref_orbit.ref_spaceObject.barycenter;
							num = num4 + ((barycenter != null) ? new double?(barycenter.semiMajorAxis_AU) : null);
						}
					}
					else
					{
						TIHabSiteState ref_habSite = habState.ref_habSite;
						double? num5;
						if (ref_habSite == null)
						{
							num5 = null;
						}
						else
						{
							TISpaceBodyState parentBody = ref_habSite.parentBody;
							num5 = ((parentBody != null) ? new double?(parentBody.semiMajorAxis_AU) : null);
						}
						num = num5;
						TIHabSiteState ref_habSite2 = habState.ref_habSite;
						bool flag5;
						if (ref_habSite2 == null)
						{
							flag5 = false;
						}
						else
						{
							TISpaceBodyState parentBody2 = ref_habSite2.parentBody;
							bool? flag3 = ((parentBody2 != null) ? new bool?(parentBody2.isaMoon) : null);
							bool flag4 = true;
							flag5 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
						}
						if (flag5)
						{
							double? num2 = num;
							TIHabSiteState ref_habSite3 = habState.ref_habSite;
							double? num6;
							if (ref_habSite3 == null)
							{
								num6 = null;
							}
							else
							{
								TISpaceBodyState parentBody3 = ref_habSite3.parentBody;
								if (parentBody3 == null)
								{
									num6 = null;
								}
								else
								{
									TINaturalSpaceObjectState barycenter2 = parentBody3.barycenter;
									num6 = ((barycenter2 != null) ? new double?(barycenter2.semiMajorAxis_AU) : null);
								}
							}
							num = num2 + num6;
						}
					}
					return num;
				};
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby orbitDistanceSelector(o) == null, orbitDistanceSelector(o)
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby orbitDistanceSelector(o) == null descending, orbitDistanceSelector(o) descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			}
			case HabitatsScreenController.SortHabDataBy.Tier:
				if (!this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderByDescending<HabScreenHabListItemModel, int>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.TierSortValue).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderBy<HabScreenHabListItemModel, int>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.TierSortValue).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Population:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.PopulationSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.PopulationSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Power:
				if (!this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderByDescending<HabScreenHabListItemModel, bool>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.PowerSortValue).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderBy<HabScreenHabListItemModel, bool>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.PowerSortValue).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.ModuleConstruction:
				if (!this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderByDescending<HabScreenHabListItemModel, bool>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.ModuleConstructionSortValue).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = this.habModels.OrderBy<HabScreenHabListItemModel, bool>((HabScreenHabListItemModel o) => o.HabScreenHabListItemData.ModuleConstructionSortValue).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Money:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.MoneySortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.MoneySortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Influence:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.InfluenceSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.InfluenceSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Ops:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.OpsSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.OpsSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Research:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ResearchSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ResearchSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Projects:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ProjectsSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.ProjectsSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			case HabitatsScreenController.SortHabDataBy.Boost:
				if (!this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.BoostSortValue descending
						select o).ToList<HabScreenHabListItemModel>();
				}
				if (this.reverseHabSort)
				{
					this.habModels = (from o in this.habModels
						orderby o.HabScreenHabListItemData.habState.IsAlien(), o.HabScreenHabListItemData.BoostSortValue
						select o).ToList<HabScreenHabListItemModel>();
				}
				break;
			}
			this.UpdateHabModelData();
		}

		// Token: 0x06005057 RID: 20567 RVA: 0x0022D598 File Offset: 0x0022B798
		public void OnStationsToggleClicked(bool playSound = true)
		{
			if (playSound)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			if (this.stationsToggle.isOn)
			{
				if (this.habList_FilterForHabType == HabType.Base)
				{
					this.habList_FilterForHabType = HabType.Any;
				}
			}
			else if (this.habList_FilterForHabType == HabType.Any)
			{
				this.habList_FilterForHabType = HabType.Base;
			}
			if (!this.stationsToggle.isOn && !this.basesToggle.isOn)
			{
				this.basesToggle.SetIsOnWithoutNotify(true);
				this.OnBasesToggleClicked(false);
				this.habList_FilterForHabType = HabType.Base;
			}
			this.UpdateHabLists();
		}

		// Token: 0x06005058 RID: 20568 RVA: 0x0022D620 File Offset: 0x0022B820
		public void OnBasesToggleClicked(bool playSound = true)
		{
			if (playSound)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			if (this.basesToggle.isOn)
			{
				if (this.habList_FilterForHabType == HabType.Station)
				{
					this.habList_FilterForHabType = HabType.Any;
				}
			}
			else if (this.habList_FilterForHabType == HabType.Any)
			{
				this.habList_FilterForHabType = HabType.Station;
			}
			if (!this.stationsToggle.isOn && !this.basesToggle.isOn)
			{
				this.stationsToggle.SetIsOnWithoutNotify(true);
				this.OnStationsToggleClicked(false);
				this.habList_FilterForHabType = HabType.Station;
			}
			this.UpdateHabLists();
		}

		// Token: 0x06005059 RID: 20569 RVA: 0x0022D6A4 File Offset: 0x0022B8A4
		public void OnHabIconFilterDropdownChanged()
		{
			this.habList_FilterForHabIcon = this.habIconPaths[this.habIconFilterDropdown.value];
			this.UpdateHabLists();
		}

		// Token: 0x0600505A RID: 20570 RVA: 0x0022D6C8 File Offset: 0x0022B8C8
		public void OnFactionDropdownChanged()
		{
			if (this.factionsDropdown.value > 1)
			{
				this.habList_FilterForFaction = this.factionDropdownLookup[this.factionsDropdown.value];
			}
			else
			{
				this.habList_FilterForFaction = this.factionDropdownLookup[0];
			}
			this.habList_FilterHumanFactionsOnly = this.factionsDropdown.value == 1;
			if (this.factionsDropdown.value > 2)
			{
				this.habIconFilterDropdown.interactable = false;
				if (this.habIconFilterDropdown.value != 0)
				{
					this.habIconFilterDropdown.SetValueWithoutNotify(0);
					this.habList_FilterForHabIcon = this.habIconPaths[this.habIconFilterDropdown.value];
				}
			}
			else
			{
				this.habIconFilterDropdown.interactable = true;
			}
			this.UpdateHabLists();
		}

		// Token: 0x0600505B RID: 20571 RVA: 0x0022D78A File Offset: 0x0022B98A
		public void OnLocationDropdownChanged()
		{
			this.habList_FilterForSpaceObject = this.locationDropdownLookup[this.locationDropdown.value];
			this.UpdateHabLists();
		}

		// Token: 0x0600505C RID: 20572 RVA: 0x0022D7AE File Offset: 0x0022B9AE
		public void HighlightSelectedHab(TIHabState hab)
		{
			this.UnHighlightAllHabs();
			this.SetSelectedStatus(hab, true, false);
		}

		// Token: 0x0600505D RID: 20573 RVA: 0x0022D7BF File Offset: 0x0022B9BF
		public void UnHighlightAllHabs()
		{
			this.selectedHabList.Clear();
			this.UpdateHabModelData();
		}

		// Token: 0x0600505E RID: 20574 RVA: 0x0022D7D4 File Offset: 0x0022B9D4
		public void SetMenuToSelectedModule(HabModuleListItem module)
		{
			for (int i = 0; i < this.availableModuleListItems.Count; i++)
			{
				this.availableModuleListItems[i].SetHighlight(false);
			}
			for (int j = 0; j < this.installedModuleListItems.Count; j++)
			{
				this.installedModuleListItems[j].SetHighlight(false);
			}
			if (module != null)
			{
				module.SetHighlight(true);
			}
		}

		// Token: 0x0600505F RID: 20575 RVA: 0x0022D844 File Offset: 0x0022BA44
		public void SelectHabFromMenu(TIHabState hab)
		{
			this.HighlightSelectedHab(hab);
			if (this.habToDisplay == hab && hab.ref_factions.Contains(GameControl.control.activePlayer))
			{
				this.PreviewHab();
				this.OnClickHabManage();
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				this.habToDisplay = hab;
				this.PreviewHab();
			}
			this.RevertRename();
		}

		// Token: 0x06005060 RID: 20576 RVA: 0x0022D8AC File Offset: 0x0022BAAC
		public void Tutorial_SelectFirstPlayerHab()
		{
			int num = this.habListAdapter.Data.TakeWhile<HabScreenHabListItemModel>((HabScreenHabListItemModel x) => !x.HabScreenHabListItemData.habState.ref_faction.isActivePlayer).Count<HabScreenHabListItemModel>();
			if (num != this.habListAdapter.Data.Count)
			{
				this.habListAdapter.BringToView(num, null);
			}
			List<HabListItem> list = this.habListAdapter.GetComponentsInChildren<HabListItem>().Skip<HabListItem>(1).Reverse<HabListItem>()
				.ToList<HabListItem>();
			if (list != null && list.Count == 0)
			{
				return;
			}
			HabListItem habListItem = list.FirstOrDefault<HabListItem>((HabListItem x) => x.habState.ref_faction.isActivePlayer);
			if (habListItem == null)
			{
				return;
			}
			if (this.habToDisplay != habListItem.habState)
			{
				this.SelectHabFromMenu(habListItem.habState);
			}
		}

		// Token: 0x06005061 RID: 20577 RVA: 0x0022D98D File Offset: 0x0022BB8D
		public bool IsManaging()
		{
			return this.managingHab;
		}

		// Token: 0x06005062 RID: 20578 RVA: 0x0022D998 File Offset: 0x0022BB98
		public void ManageHab()
		{
			this.managingHab = true;
			this.managementQueryObject.SetActive(false);
			this.confirmModulePopupCanvas.enabled = false;
			this.SetToManagementView();
			this.RefreshManagementView(true);
			this.UpdateHabPreviewDisplay();
			base.StartCoroutine(this.ShowHabManagementTutorial());
			this.HabScreenMainUITutorialController.HideTutorial();
		}

		// Token: 0x06005063 RID: 20579 RVA: 0x0022D9F0 File Offset: 0x0022BBF0
		private void SetManageButtonStatusAndText()
		{
			if (this.IsManaging())
			{
				this.habManageButton.gameObject.SetActive(false);
				this.closeHabManageButton.gameObject.SetActive(true);
				if (this.habToDisplay.sectors == null || this.habToDisplay.sectors.Count == 0 || this.habToDisplay.sectors[0].habModules == null || this.habToDisplay.sectors[0].habModules.Count == 0)
				{
					this.globalRebuildButtonObject.SetActive(false);
					this.globalUpgradeButtonObject.SetActive(false);
				}
				else
				{
					this.globalRebuildButtonObject.SetActive(this.habToDisplay.RebuildCandidates().Count > 0);
					this.globalUpgradeButtonObject.SetActive(this.habToDisplay.UpgradeCandidates().Count > 0);
				}
				this.PowerAllFillerButtonObject.SetActive(!this.PowerAllButton.gameObject.activeSelf);
				this.globalRebuildButtonFillerObject.SetActive(!this.globalRebuildButtonObject.activeSelf);
				this.globalUpgradeButtonFillerObject.SetActive(!this.globalUpgradeButtonObject.activeSelf);
			}
			else
			{
				this.globalRebuildButtonObject.SetActive(false);
				this.globalUpgradeButtonObject.SetActive(false);
				this.PowerAllFillerButtonObject.SetActive(!this.PowerAllButton.gameObject.activeSelf);
				this.globalRebuildButtonFillerObject.SetActive(!this.globalRebuildButtonObject.activeSelf);
				this.globalUpgradeButtonFillerObject.SetActive(!this.globalUpgradeButtonObject.activeSelf);
				if (this.applyingMassTemplates)
				{
					this.managementQueryObject.SetActive(true);
					this.manageHabTemplatesButton.gameObject.SetActive(false);
					this.DecommissionHabButton.gameObject.SetActive(false);
					this.habManageButton.gameObject.SetActive(false);
					this.closeHabManageButton.gameObject.SetActive(false);
				}
				else
				{
					this.managementQueryObject.SetActive(false);
					this.manageHabTemplatesButton.gameObject.SetActive(false);
					this.DecommissionHabButton.gameObject.SetActive(false);
					if (this.PlayerHab)
					{
						this.closeHabManageButton.gameObject.SetActive(false);
						this.habManageButton.gameObject.SetActive(true);
					}
					else
					{
						this.habManageButton.gameObject.SetActive(false);
						this.closeHabManageButton.gameObject.SetActive(false);
					}
				}
			}
			if (this.habToDisplay != null)
			{
				this.habGotoButton.gameObject.SetActive(true);
				this.zoomContainer.SetActive(true);
			}
			else
			{
				this.habGotoButton.gameObject.SetActive(false);
				this.zoomContainer.SetActive(false);
			}
			this.SetShowCopySaveHabTemplateButtons();
		}

		// Token: 0x06005064 RID: 20580 RVA: 0x0022DCB8 File Offset: 0x0022BEB8
		private void SetToPreviewView()
		{
			if (this.habListMasterObject != null)
			{
				this.habListMasterObject.SetActive(true);
				this.habPreviewInfoPanel.SetActive(false);
				this.moduleSelectionPanel.SetActive(false);
				this.nextHabButtonsContainer.SetActive(false);
				this.previousHabButtonsContainer.SetActive(false);
				this.exoticsSortButtonObject.SetActive(false);
				this.antimatterSortButtonObject.SetActive(GameControl.control.activePlayer.ref_faction.UnlockedAntimatter && GameControl.control.activePlayer.ref_faction.GetDailyIncome(FactionResource.Antimatter, false, false) > 0f);
				this.editNameButton.SetActive(false);
				this.editNameIcon.SetActive(false);
				this.RevertRename();
				this.PopulateMassHabTemplateDropdown();
				this.OnClickMassHabTemplateCancel();
				this.ResetQuickHabTemplateDropdown();
				this.SetManageButtonStatusAndText();
				if (GameControl.control.activePlayer.habs.Count > 0)
				{
					this.HabScreenMainUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_HabScreenCanvas, false, true);
				}
				this.HabScreenManagementUITutorialController.HideTutorial();
			}
		}

		// Token: 0x06005065 RID: 20581 RVA: 0x0022DDCC File Offset: 0x0022BFCC
		private void SetToManagementView()
		{
			this.habListMasterObject.SetActive(false);
			this.habPreviewInfoPanel.SetActive(true);
			this.moduleSelectionPanel.SetActive(true);
			this.RefreshHabCycleButtonsNavigation();
			this.nextHabButtonsContainer.SetActive(true);
			this.previousHabButtonsContainer.SetActive(true);
			this.exoticsSortButtonObject.SetActive(GameControl.control.activePlayer.ref_faction.UnlockedExotics && GameControl.control.activePlayer.ref_faction.GetCurrentResourceAmount(FactionResource.Exotics) > 0f);
			this.antimatterSortButtonObject.SetActive(GameControl.control.activePlayer.ref_faction.UnlockedAntimatter && GameControl.control.activePlayer.ref_faction.GetDailyIncome(FactionResource.Antimatter, false, false) > 0f);
			this.PopulateQuickHabTemplateDropdown();
			this.ResetQuickHabTemplateDropdown();
			this.OnClickMassHabTemplateCancel();
			this.SetManageButtonStatusAndText();
			this.editNameButton.SetActive(true);
			this.editNameIcon.SetActive(true);
			this.HabScreenMainUITutorialController.HideTutorial();
		}

		// Token: 0x06005066 RID: 20582 RVA: 0x0022DEDB File Offset: 0x0022C0DB
		public void OnClickHabManage()
		{
			if (this.habToDisplay.faction == base.activePlayer)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				this.selectedStationModule = null;
				this.selectedBaseModule = null;
				this.ManageHab();
			}
		}

		// Token: 0x06005067 RID: 20583 RVA: 0x0022DF15 File Offset: 0x0022C115
		public void OnClickCloseHabManagement()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.selectedStationModule = null;
			this.selectedBaseModule = null;
			this.CloseHabManagement();
		}

		// Token: 0x06005068 RID: 20584 RVA: 0x0022DF37 File Offset: 0x0022C137
		public void CloseHabManagement()
		{
			this.managingHab = false;
			this.SetToPreviewView();
			this.CloseModuleBuildPanel();
			this.HabScreenManagementUITutorialController.HideTutorial();
		}

		// Token: 0x06005069 RID: 20585 RVA: 0x0022DF58 File Offset: 0x0022C158
		public void OnHabIconChanged()
		{
			if (this.habToDisplay != null)
			{
				this.habToDisplay.faction.playerControl.StartAction(new ChangeHabBio(this.habToDisplay, this.habToDisplay.displayName, this.habIconPaths[this.habinfoIconDropdown.value]));
				this.UpdateHabLists();
				(base.canvasManager.StrategyHud as GeneralControlsController).UpdateFinderList();
				this.RefreshHabIconCycleButtonsNavigation(null);
			}
		}

		// Token: 0x0600506A RID: 20586 RVA: 0x0022DFD6 File Offset: 0x0022C1D6
		public IEnumerator ShowHabManagementTutorial()
		{
			yield return null;
			this.HabScreenManagementUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_HabScreenCanvasManagement, false, true);
			yield break;
		}

		// Token: 0x0600506B RID: 20587 RVA: 0x0022DFE5 File Offset: 0x0022C1E5
		private void PreviewHab()
		{
			if (this.habToDisplay != null)
			{
				this.SetManageButtonStatusAndText();
				this.ResetHabDisplayZoom();
				this.UpdateHabPreviewDisplay();
				return;
			}
			this.SetManageButtonStatusAndText();
		}

		// Token: 0x0600506C RID: 20588 RVA: 0x0022E010 File Offset: 0x0022C210
		private void RefreshCanvas()
		{
			if (this.habToDisplay != null)
			{
				this.PreviewHab();
			}
			if (!this.managingHab)
			{
				this.UpdateHabLists();
				return;
			}
			if (this.habToDisplay != null)
			{
				this.RefreshManagementView(false);
				return;
			}
			this.SetToPreviewView();
			this.UpdateHabLists();
		}

		// Token: 0x0600506D RID: 20589 RVA: 0x0022E064 File Offset: 0x0022C264
		private void RefreshManagementView(bool canDeselectCurrentModule = false)
		{
			this.UpdateModuleList(this.habToDisplay.habType);
			this.UpdateFilterButtons();
			this.BuildHabSummary(this.habToDisplay);
			this.shipyardButton.gameObject.SetActive(false);
			if (canDeselectCurrentModule)
			{
				if (this.showAvailableModules)
				{
					if (!this.habToDisplay.AllowedModules(this.habToDisplay.faction).Contains(this.prospectiveModule))
					{
						this.prospectiveModule = null;
					}
				}
				else
				{
					this.displayModuleSector = -1;
					this.displayModuleSlot = -1;
				}
			}
			this.UpdateModulePreviewText(this.showAvailableModules, false);
			this.PopulateQuickHabTemplateDropdown();
			this.ResetQuickHabTemplateDropdown();
			this.RefreshHabCycleButtonsNavigation();
		}

		// Token: 0x0600506E RID: 20590 RVA: 0x0022E10C File Offset: 0x0022C30C
		private void SetEmptyHabView()
		{
			this.summaryTitleLine.SetText(Loc.T("UI.Habs.HabModuleMap"));
			this.UnHighlightAllHabs();
			this.habToDisplay = null;
			this.managingHab = false;
			this.CloseHabTemplateManager();
			this.OnClickMassHabTemplateCancel();
			this.SetManageButtonStatusAndText();
			this.noHabSelected.SetActive(true);
			this.selectedHabGravity.SetText(string.Empty);
			this.selectedHabCrew.SetText(string.Empty);
			this.powerReportText.SetText(string.Empty);
			this.selectedHabCrewDisplayObject.SetActive(false);
			this.gravityDisplayObject.SetActive(false);
			this.powerReportTextObject.SetActive(false);
			this.maxTierIcon.gameObject.SetActive(false);
			this.habinfoIconDropdown.gameObject.SetActive(false);
			this.habSubtitleObject.SetActive(false);
			this.habSiteProductivityPanel.SetActive(false);
			this.stationDisplayCanvas.enabled = false;
			this.baseDisplayCanvas.enabled = false;
			this.baseSurfaceImage.enabled = false;
			this.confirmModulePopupCanvas.enabled = false;
			this.councilorGridPanel.SetActive(false);
			this.editNameButton.SetActive(false);
			this.editNameIcon.SetActive(false);
			this.managementQueryObject.SetActive(false);
			this.RevertRename();
			this.ResetQuickHabTemplateDropdown();
		}

		// Token: 0x0600506F RID: 20591 RVA: 0x0022E25C File Offset: 0x0022C45C
		public static string GetExtendedHabName(TIHabState hab)
		{
			StringBuilder stringBuilder = new StringBuilder(hab.GetDisplayName(GameControl.control.activePlayer));
			if (GameControl.control.activePlayer.victoryTemplate.GetConditionBlockingSpaceAssets(GameControl.control.activePlayer).Contains(hab))
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.victoryItemInlineSpritePath);
			}
			if (hab.underAssault || hab.underBombardment || (hab.IsStation && hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(hab.faction))))
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.armyBattleInlineSpritePath);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005070 RID: 20592 RVA: 0x0022E32C File Offset: 0x0022C52C
		private void BuildHabSummary(TIHabState hab)
		{
			this.summaryTitleLine.SetText(hab.GetDisplayName(GameControl.control.activePlayer));
			GameControl.assetLoader.LoadAssetForImageAssignment(new StringBuilder("icons_2d/ICO_MaxTier").Append(this.habToDisplay.tier).ToString(), this.maxTierIcon);
			this.UpdateHabInfoIconDropdown();
			List<HabitatsScreenController.IncomeEntry> list = new List<HabitatsScreenController.IncomeEntry>();
			TIGlobalConfig global = TemplateManager.global;
			TIFactionState faction = hab.faction;
			int num = this.habToDisplay.NetPower(false, false);
			list.Add(new HabitatsScreenController.IncomeEntry((num >= 0) ? global.pathHabPowerIcon : global.pathHabPowerAlertIcon, num.ToString("N0")));
			foreach (FactionResource factionResource in Enums.FactionResources)
			{
				if (this.habToDisplay.HasResourceIncomeForFaction(factionResource, faction))
				{
					list.Add(new HabitatsScreenController.IncomeEntry(TIUtilities.PathResourceIcon(factionResource), TIUtilities.FormatBigOrSmallNumber(this.habToDisplay.GetNetCurrentMonthlyIncome(faction, factionResource, false, false), 1, 3, 0, factionResource == FactionResource.Antimatter, false)));
				}
			}
			int controlPointCapacityValue = this.habToDisplay.controlPointCapacityValue;
			if (controlPointCapacityValue != 0)
			{
				list.Add(new HabitatsScreenController.IncomeEntry(TemplateManager.global.pathEmptyControlPoint, controlPointCapacityValue.ToString("N0")));
			}
			foreach (TechCategory techCategory in Enums.TechCategories)
			{
				float netTechBonusByFaction = this.habToDisplay.GetNetTechBonusByFaction(techCategory, faction, false);
				if (netTechBonusByFaction != 0f)
				{
					list.Add(new HabitatsScreenController.IncomeEntry(TIGenericTechTemplate.PathTechCategoryIcon(techCategory), netTechBonusByFaction.ToPercent((netTechBonusByFaction * 100f % 1f > 0f) ? "P1" : "P0")));
				}
			}
			if (this.habToDisplay.AllowsResupply(faction, true, true))
			{
				list.Add(new HabitatsScreenController.IncomeEntry(global.pathHabResupplyIcon, string.Empty));
			}
			float moduleConstructionTimeModifier = this.habToDisplay.GetModuleConstructionTimeModifier(false, null);
			if (moduleConstructionTimeModifier < 1f)
			{
				list.Add(new HabitatsScreenController.IncomeEntry(global.pathHabModuleConstructionIcon, (1f - moduleConstructionTimeModifier).ToPercent("P0")));
			}
			if (this.habToDisplay.AllowsShipConstruction(faction, false, false))
			{
				list.Add(new HabitatsScreenController.IncomeEntry(global.pathHabShipyardIcon, string.Empty));
			}
			float num2 = this.habToDisplay.SpaceCombatValue();
			if (num2 != 0f)
			{
				list.Add(new HabitatsScreenController.IncomeEntry(global.pathHabDefenseIcon, num2.ToString("N0")));
			}
			this.summaryResourceGrid.SetListSize<ResourceGridItemController>(list.Count, false, false);
			int num3 = ((list.Count <= 24) ? 8 : ((list.Count <= 27) ? 9 : 10));
			float num4 = ((num3 == 8) ? 76f : ((num3 == 9) ? 74f : 66.5f));
			float num5 = 24f;
			ResourceGridItemController[] componentsInChildren = this.summaryResourceGrid.GetComponentsInChildren<ResourceGridItemController>();
			this.summaryResourceGridLayout.constraintCount = Mathf.Clamp(num3, 8, 10);
			this.summaryResourceGridLayout.cellSize = new Vector2(num4, num5);
			ResourceGridItemController[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				RectTransform rectTransform = array[i].transform.GetChild(1) as RectTransform;
				rectTransform.offsetMin = new Vector2(num5 - 1f, rectTransform.offsetMin.y);
			}
			int num6 = 0;
			using (IEnumerator<object> enumerator = this.summaryResourceGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (HabitatsScreenController.<>o__290.<>p__0 == null)
					{
						HabitatsScreenController.<>o__290.<>p__0 = CallSite<Func<CallSite, object, ResourceGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ResourceGridItemController), typeof(HabitatsScreenController)));
					}
					HabitatsScreenController.<>o__290.<>p__0.Target(HabitatsScreenController.<>o__290.<>p__0, enumerator.Current).UpdateListItem(list[num6].iconResourcePath, list[num6++].value);
				}
			}
			this.PowerAllButton.interactable = this.TotalPotentialPowerConsumption(this.habToDisplay) <= this.TotalCurrentAvailablePower(this.habToDisplay);
			GameObject gameObject = this.PowerAllButton.gameObject;
			bool flag;
			if (!this.habToDisplay.decommissioning)
			{
				flag = this.habToDisplay.AllModuleStates().Any<TIHabModuleState>((TIHabModuleState x) => !x.empty && !x.underConstruction && !x.decommissioning && !x.destroyed && !x.powered);
			}
			else
			{
				flag = false;
			}
			gameObject.SetActive(flag);
			this.PowerAllFillerButtonObject.SetActive(!this.PowerAllButton.gameObject.activeSelf);
			this.globalRebuildButtonFillerObject.SetActive(!this.globalRebuildButtonObject.activeSelf);
			this.globalUpgradeButtonFillerObject.SetActive(!this.globalUpgradeButtonObject.activeSelf);
			this.DecommissionHabButton.interactable = true;
			this.DecommissionHabButton.gameObject.SetActive(this.habToDisplay.CanDecommissionHab());
			this.SetMineProductivityValues();
		}

		// Token: 0x06005071 RID: 20593 RVA: 0x0022E808 File Offset: 0x0022CA08
		private void UpdateHabInfoIconDropdown()
		{
			if (this.habToDisplay.customHabIconResource != "")
			{
				for (int i = 0; i < this.habIconPaths.Count; i++)
				{
					if (this.habToDisplay.customHabIconResource == this.habIconPaths[i])
					{
						this.habinfoIconDropdown.SetValueWithoutNotify(i);
					}
				}
				return;
			}
			this.habinfoIconDropdown.SetValueWithoutNotify(0);
		}

		// Token: 0x06005072 RID: 20594 RVA: 0x0022E879 File Offset: 0x0022CA79
		public void OnGotoBarycenterButtonPressed()
		{
			TIGameState barycenter = this.habToDisplay.barycenter;
			this.CloseInfoScreen(false);
			TIUtilities.GotoGameState(barycenter, true, true, true, true, false, -1f);
		}

		// Token: 0x06005073 RID: 20595 RVA: 0x0022E89C File Offset: 0x0022CA9C
		public void OnGotoHabButtonPressed()
		{
			TIGameState tigameState = this.habToDisplay;
			this.CloseInfoScreen(false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyHabSelect", false, false);
			TIUtilities.GotoGameState(tigameState, true, true, true, true, false, -1f);
		}

		// Token: 0x06005074 RID: 20596 RVA: 0x0022E8C8 File Offset: 0x0022CAC8
		private List<TIHabState> GetDistanceSortedPlayerHabs()
		{
			Func<TIHabState, double?> orbitDistanceSelector = delegate(TIHabState hab)
			{
				double? num = null;
				if (hab.IsStation)
				{
					TIOrbitState ref_orbit = hab.ref_orbit;
					double? num2 = ((ref_orbit != null) ? new double?(ref_orbit.semiMajorAxis_AU) : null);
					TIOrbitState ref_orbit2 = hab.ref_orbit;
					double? num3;
					if (ref_orbit2 == null)
					{
						num3 = null;
					}
					else
					{
						TISpaceObjectState ref_spaceObject = ref_orbit2.ref_spaceObject;
						num3 = ((ref_spaceObject != null) ? new double?(ref_spaceObject.semiMajorAxis_AU) : null);
					}
					num = num2 + num3;
					TIOrbitState ref_orbit3 = hab.ref_orbit;
					bool flag;
					if (ref_orbit3 == null)
					{
						flag = false;
					}
					else
					{
						TISpaceObjectState ref_spaceObject2 = ref_orbit3.ref_spaceObject;
						bool? flag2 = ((ref_spaceObject2 != null) ? new bool?(ref_spaceObject2.isaMoon) : null);
						bool flag3 = true;
						flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					if (flag)
					{
						double? num4 = num;
						TINaturalSpaceObjectState barycenter = hab.ref_orbit.ref_spaceObject.barycenter;
						num = num4 + ((barycenter != null) ? new double?(barycenter.semiMajorAxis_AU) : null);
					}
				}
				else
				{
					TIHabSiteState ref_habSite = hab.ref_habSite;
					double? num5;
					if (ref_habSite == null)
					{
						num5 = null;
					}
					else
					{
						TISpaceBodyState parentBody = ref_habSite.parentBody;
						num5 = ((parentBody != null) ? new double?(parentBody.semiMajorAxis_AU) : null);
					}
					num = num5;
					TIHabSiteState ref_habSite2 = hab.ref_habSite;
					bool flag4;
					if (ref_habSite2 == null)
					{
						flag4 = false;
					}
					else
					{
						TISpaceBodyState parentBody2 = ref_habSite2.parentBody;
						bool? flag2 = ((parentBody2 != null) ? new bool?(parentBody2.isaMoon) : null);
						bool flag3 = true;
						flag4 = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					if (flag4)
					{
						double? num2 = num;
						TIHabSiteState ref_habSite3 = hab.ref_habSite;
						double? num6;
						if (ref_habSite3 == null)
						{
							num6 = null;
						}
						else
						{
							TISpaceBodyState parentBody3 = ref_habSite3.parentBody;
							if (parentBody3 == null)
							{
								num6 = null;
							}
							else
							{
								TINaturalSpaceObjectState barycenter2 = parentBody3.barycenter;
								num6 = ((barycenter2 != null) ? new double?(barycenter2.semiMajorAxis_AU) : null);
							}
						}
						num = num2 + num6;
					}
				}
				return num;
			};
			return (from x in GameStateManager.GetAllGameStates<TIHabState>(true)
				where x.ref_factions.Contains(this.activePlayer)
				select x into o
				orderby orbitDistanceSelector(o) == null, orbitDistanceSelector(o)
				select o).ToList<TIHabState>();
		}

		// Token: 0x06005075 RID: 20597 RVA: 0x0022E948 File Offset: 0x0022CB48
		private List<TIHabState> GetDistanceSortedPlayerHabsPivotedAroundDisplayHab(bool displayHabAtStart)
		{
			List<TIHabState> list = this.GetDistanceSortedPlayerHabs();
			if (this.habToDisplay != null)
			{
				int num = list.IndexOf(this.habToDisplay);
				if (num >= 0)
				{
					IEnumerable<TIHabState> enumerable = list.Take<TIHabState>(num);
					IEnumerable<TIHabState> enumerable2 = list.Skip<TIHabState>(num + 1);
					list = (displayHabAtStart ? new TIHabState[] { this.habToDisplay }.Concat<TIHabState>(enumerable2).Concat<TIHabState>(enumerable).ToList<TIHabState>() : enumerable2.Concat<TIHabState>(enumerable).Concat<TIHabState>(new TIHabState[] { this.habToDisplay }).ToList<TIHabState>());
				}
			}
			return list;
		}

		// Token: 0x06005076 RID: 20598 RVA: 0x0022E9D4 File Offset: 0x0022CBD4
		private List<TIHabState> GetDistanceSortedPlayerHabsPivotedAroundDisplayHabNeedingAttention(bool displayHabAtStart)
		{
			return (from x in this.GetDistanceSortedPlayerHabsPivotedAroundDisplayHab(displayHabAtStart)
				where x.AvailableSlots().Count > 0 || x.UpgradeCandidates().Count > 0 || x.UnpoweredModules().Count > 0
				select x).ToList<TIHabState>();
		}

		// Token: 0x06005077 RID: 20599 RVA: 0x0022EA06 File Offset: 0x0022CC06
		private List<TIHabState> GetDistanceSortedPlayerHabsPivotedAroundDisplayHabWithMatchingIcon(bool displayHabAtStart)
		{
			if (this.habToDisplay != null)
			{
				return (from x in this.GetDistanceSortedPlayerHabsPivotedAroundDisplayHab(displayHabAtStart)
					where x.customHabIconResource == this.habToDisplay.customHabIconResource
					select x).ToList<TIHabState>();
			}
			return new List<TIHabState>();
		}

		// Token: 0x06005078 RID: 20600 RVA: 0x0022EA3C File Offset: 0x0022CC3C
		private void DisplayNextValidHab(bool next, List<TIHabState> playerHabs)
		{
			int num = (next ? 0 : (playerHabs.Count - 1));
			int num2 = (next ? 1 : (-1));
			int num3 = num;
			while (next ? (num3 < playerHabs.Count) : (num3 >= 0))
			{
				if (!(playerHabs[num3] == this.habToDisplay))
				{
					this.SelectHabFromMenu(playerHabs[num3]);
					this.RefreshManagementView(true);
					return;
				}
				num3 += num2;
			}
		}

		// Token: 0x06005079 RID: 20601 RVA: 0x0022EAA8 File Offset: 0x0022CCA8
		private void RefreshHabCycleButtonsNavigation()
		{
			List<TIHabState> list = null;
			if (this.habToDisplay != null)
			{
				list = this.GetDistanceSortedPlayerHabs();
				bool flag = list.Count >= 2;
				this.nextHabButton.interactable = flag;
				this.previousHabButton.interactable = flag;
				int num = list.Count<TIHabState>((TIHabState x) => x.AvailableSlots().Count > 0 || x.UpgradeCandidates().Count > 0 || x.UnpoweredModules().Count > 0);
				bool flag2 = num >= 1;
				if (num == 1 && (this.habToDisplay.AvailableSlots().Count > 0 || this.habToDisplay.UpgradeCandidates().Count > 0 || this.habToDisplay.UnpoweredModules().Count > 0))
				{
					flag2 = false;
				}
				this.nextSmartHabButton.interactable = flag2;
				this.previousSmartHabButton.interactable = flag2;
			}
			else
			{
				this.nextHabButton.interactable = false;
				this.previousHabButton.interactable = false;
				this.nextSmartHabButton.interactable = false;
				this.previousSmartHabButton.interactable = false;
			}
			this.RefreshHabIconCycleButtonsNavigation(list);
		}

		// Token: 0x0600507A RID: 20602 RVA: 0x0022EBB0 File Offset: 0x0022CDB0
		private void RefreshHabIconCycleButtonsNavigation(List<TIHabState> habs = null)
		{
			if (this.habToDisplay != null && !string.IsNullOrEmpty(this.habToDisplay.customHabIconResource))
			{
				if (habs == null)
				{
					habs = this.GetDistanceSortedPlayerHabs();
				}
				bool flag = habs.Count<TIHabState>((TIHabState x) => x.customHabIconResource == this.habToDisplay.customHabIconResource) >= 2;
				this.nextIconHabButton.gameObject.SetActive(flag);
				this.previousIconHabButton.gameObject.SetActive(flag);
				if (flag)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(this.habToDisplay.customHabIconResource, this.nextIconHabButtonImage);
					GameControl.assetLoader.LoadAssetForImageAssignment(this.habToDisplay.customHabIconResource, this.previousIconHabButtonImage);
					return;
				}
			}
			else
			{
				this.nextIconHabButton.gameObject.SetActive(false);
				this.previousIconHabButton.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600507B RID: 20603 RVA: 0x0022EC85 File Offset: 0x0022CE85
		public void OnSwapHabButtonPressed(bool next)
		{
			this.CloseModuleBuildPanel();
			this.OnCancelHabManagementButtonPressed(true);
			if (next)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			}
			this.DisplayNextValidHab(next, this.GetDistanceSortedPlayerHabsPivotedAroundDisplayHab(next));
		}

		// Token: 0x0600507C RID: 20604 RVA: 0x0022ECBF File Offset: 0x0022CEBF
		public void OnSmartSwapHabButtonPressed(bool next)
		{
			this.CloseModuleBuildPanel();
			this.OnCancelHabManagementButtonPressed(true);
			if (next)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			}
			this.DisplayNextValidHab(next, this.GetDistanceSortedPlayerHabsPivotedAroundDisplayHabNeedingAttention(next));
		}

		// Token: 0x0600507D RID: 20605 RVA: 0x0022ECF9 File Offset: 0x0022CEF9
		public void OnIconSwapHabButtonPressed(bool next)
		{
			this.CloseModuleBuildPanel();
			this.OnCancelHabManagementButtonPressed(true);
			if (next)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			}
			this.DisplayNextValidHab(next, this.GetDistanceSortedPlayerHabsPivotedAroundDisplayHabWithMatchingIcon(next));
		}

		// Token: 0x0600507E RID: 20606 RVA: 0x0022ED34 File Offset: 0x0022CF34
		public void OnPowerAllButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_PowerModule", false, false);
			base.activePlayer.playerControl.StartAction(new UpdateHabPowerAllAction(this.habToDisplay));
			this.BuildHabSummary(this.habToDisplay);
			this.UpdateModulePreviewText(this.showAvailableModules, false);
		}

		// Token: 0x0600507F RID: 20607 RVA: 0x0022ED84 File Offset: 0x0022CF84
		public void OnDecommissionModulePressed()
		{
			this.CloseModuleBuildPanel();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			this.managementQueryObject.SetActive(true);
			if (this.selectedModule != null && this.selectedModule.habModule == null)
			{
				Log.Error("No module selected", Array.Empty<object>());
				return;
			}
			this.queryDecommissionModule = true;
			TIResourcesCost tiresourcesCost = this.selectedModule.habModule.DecommissionModuleCost();
			if (tiresourcesCost.completionTime_days > 0f)
			{
				this.managementQueryText.SetText(Loc.T("UI.Habs.DecommissionModuleQuery", new object[]
				{
					tiresourcesCost.ToString("Relevant", false, false, null, false, FactionResource.None),
					tiresourcesCost.completionTime_days.ToString()
				}));
			}
			else
			{
				this.managementQueryText.SetText(Loc.T("UI.Habs.DecommissionModuleQueryRefund"));
			}
			this.managementQueryConfirmButton.interactable = tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
			this.managementQueryConfirmButton.onClick.RemoveAllListeners();
			this.managementQueryConfirmButton.onClick.AddListener(new UnityAction(this.OnConfirmDecommissionModule));
			this.managementQueryConfirmButtonText.SetText(Loc.T("UI.Habs.Confirm"));
			this.managementQueryConfirmButtonObject.SetActive(true);
			this.managementQueryTemplateDropdownObject.SetActive(false);
			this.managementQuerySelectedHabDropdownObject.SetActive(false);
			this.managementQueryToggleObject.SetActive(false);
		}

		// Token: 0x06005080 RID: 20608 RVA: 0x0022EEF0 File Offset: 0x0022D0F0
		public void OnConfirmDecommissionModule()
		{
			if (this.selectedModule.habModule.moduleTemplate != null && this.selectedModule.habModule.DecommissionModuleCost().CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				base.activePlayer.playerControl.StartAction(new DecommissionHabModuleAction(this.selectedModule.habModule));
				this.selectedModule.SetDecommissioningVisuals();
			}
			this.managementQueryObject.SetActive(false);
			this.queryDecommissionModule = false;
			this.RefreshManagementView(false);
		}

		// Token: 0x06005081 RID: 20609 RVA: 0x0022EF88 File Offset: 0x0022D188
		public void OnDecommissionHabPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			this.managementQueryObject.SetActive(true);
			TIResourcesCost tiresourcesCost = this.habToDisplay.DecommissionHabCost();
			this.managementQueryText.SetText(Loc.T("UI.Habs.DecommissionHabQuery", new object[]
			{
				tiresourcesCost.ToString("Relevant", false, false, null, false, FactionResource.None),
				tiresourcesCost.completionTime_days.ToString()
			}));
			this.managementQueryConfirmButton.interactable = tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
			this.managementQueryConfirmButton.onClick.RemoveAllListeners();
			this.managementQueryConfirmButton.onClick.AddListener(new UnityAction(this.OnConfirmDecommissionHab));
			this.managementQueryConfirmButtonText.SetText(Loc.T("UI.Habs.Confirm"));
			this.managementQueryConfirmButtonObject.SetActive(true);
			this.managementQueryTemplateDropdownObject.SetActive(false);
			this.managementQuerySelectedHabDropdownObject.SetActive(false);
			this.managementQueryToggleObject.SetActive(false);
		}

		// Token: 0x06005082 RID: 20610 RVA: 0x0022F08C File Offset: 0x0022D28C
		public void OnConfirmDecommissionHab()
		{
			if (this.habToDisplay.DecommissionHabCost().CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				base.activePlayer.playerControl.StartAction(new DecommissionHabAction(this.habToDisplay));
				if (this.habToDisplay.IsBase)
				{
					using (Dictionary<string, BaseGridCell>.ValueCollection.Enumerator enumerator = this.baseCellDictionary.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							BaseGridCell baseGridCell = enumerator.Current;
							if (!baseGridCell.isEmpty)
							{
								baseGridCell.SetDecommissioningVisuals();
							}
						}
						goto IL_00DE;
					}
				}
				foreach (StationGridCell stationGridCell in this.stationCellDictionary.Values)
				{
					if (!stationGridCell.isEmpty)
					{
						stationGridCell.SetDecommissioningVisuals();
					}
				}
				IL_00DE:
				this.UpdateHabPreviewDisplay();
				this.UpdateModuleList(this.habToDisplay.habType);
			}
			this.managementQueryObject.SetActive(false);
			this.RefreshManagementView(false);
		}

		// Token: 0x06005083 RID: 20611 RVA: 0x0022F1C0 File Offset: 0x0022D3C0
		public void OnCancelHabManagementButtonPressed()
		{
			this.OnCancelHabManagementButtonPressed(false);
		}

		// Token: 0x06005084 RID: 20612 RVA: 0x0022F1CC File Offset: 0x0022D3CC
		public void OnCancelHabManagementButtonPressed(bool skipAudio)
		{
			if (this.managementQueryObject.activeSelf)
			{
				if (this.applyingMassTemplates)
				{
					this.OnClickMassHabTemplateCancel();
				}
				this.ResetQuickHabTemplateDropdown();
				if (skipAudio)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
				}
				this.managementQueryObject.SetActive(false);
				this.queryDecommissionModule = false;
			}
		}

		// Token: 0x06005085 RID: 20613 RVA: 0x0022F21C File Offset: 0x0022D41C
		private void UpdateHabPreviewDisplay()
		{
			if (this.habToDisplay == null || this.habToDisplay.deleted)
			{
				this.SetEmptyHabView();
				this.gravityDisplayObject.SetActive(false);
				this.maxTierIcon.gameObject.SetActive(false);
				this.habinfoIconDropdown.gameObject.SetActive(false);
				this.powerReportTextObject.SetActive(false);
				if (this.habToDisplay != null)
				{
					this.UpdateHabLists();
				}
				return;
			}
			this.summaryTitleLine.SetText(HabitatsScreenController.GetExtendedHabName(this.habToDisplay));
			GameControl.assetLoader.LoadAssetForImageAssignment(string.Format("icons_2d/ICO_MaxTier{0}", this.habToDisplay.tier), this.maxTierIcon);
			this.habMapLocationIcon.sprite = this.habToDisplay.barycenter.icon;
			this.habMapLocationText.SetText(this.habToDisplay.LocationName);
			this.habSubtitleObject.SetActive(true);
			Dictionary<TIHabModuleState, HabModuleListItem> dictionary = this.installedModuleDictionary;
			if (dictionary != null)
			{
				dictionary.Clear();
			}
			this.habMapTypeText.SetText(Loc.T("UI.Habs.TierCrew", new object[]
			{
				this.habToDisplay.description,
				this.habToDisplay.tier,
				this.habToDisplay.maxTier.ToString("N0")
			}));
			if (this.habToDisplay.IsStation)
			{
				this.PreviewStation();
			}
			else if (this.habToDisplay.IsBase)
			{
				this.PreviewBase();
			}
			string text;
			if (this.habToDisplay.IsStation)
			{
				if (this.habToDisplay.ref_orbit.localGravity_gs >= 1E-06)
				{
					text = FleetsScreenController.accelerationStr(this.habToDisplay.ref_orbit.localGravity_gs, false, false, true);
				}
				else
				{
					text = Loc.T("UI.Space.Negligible");
				}
			}
			else if (this.habToDisplay.ref_habSite.surfaceGravity_g >= 1E-06)
			{
				text = FleetsScreenController.accelerationStr(this.habToDisplay.ref_habSite.surfaceGravity_g, false, false, true);
			}
			else
			{
				text = Loc.T("UI.Space.Negligible");
			}
			this.selectedHabGravity.SetText(text);
			this.gravityDisplayObject.SetActive(true);
			this.maxTierIcon.gameObject.SetActive(true);
			this.UpdateHabInfoIconDropdown();
			this.habinfoIconDropdown.gameObject.SetActive(this.habToDisplay.ref_factions.Contains(GameControl.control.activePlayer));
			if (this.habToDisplay.ref_faction.IsActiveHumanFaction)
			{
				this.selectedHabCrew.SetText(this.habToDisplay.crew.ToString());
				this.selectedHabCrewDisplayObject.SetActive(true);
			}
			else
			{
				this.selectedHabCrewDisplayObject.SetActive(false);
			}
			this.UpdatePowerReport(this.habToDisplay);
			this.UpdateCouncilorGrid();
			if (this.installedModuleListItems != null)
			{
				for (int i = this.installedModuleListItems.Count - 1; i >= 0; i--)
				{
					global::UnityEngine.Object.Destroy(this.installedModuleListItems[i].gameObject);
				}
				this.installedModuleListItems.Clear();
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.selectedHabInfoContainerRT);
			this.UpdateModuleList(this.habToDisplay.habType);
			this.OnHabZoomSliderSet();
		}

		// Token: 0x06005086 RID: 20614 RVA: 0x0022F550 File Offset: 0x0022D750
		private void UpdatePowerReport(TIHabState hab)
		{
			if (this.habToDisplay.ref_factions.Contains(base.activePlayer))
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = hab.UnderConstructionModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower());
				if (num >= 0)
				{
					stringBuilder.Append("+").Append(num.ToString());
				}
				else
				{
					stringBuilder.Append(num.ToString());
				}
				TMP_Text tmp_Text = this.powerReportText;
				string text = "UI.Habs.PowerStatus";
				object[] array = new object[3];
				array[0] = (from x in hab.ActiveModules()
					where x.PowerConsumer()
					select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.PowerConsumed()).ToString("N0");
				array[1] = (from x in hab.ActiveModules()
					where x.PowerProvider()
					select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower()).ToString("N0");
				array[2] = stringBuilder.ToString();
				tmp_Text.SetText(Loc.T(text, array));
				this.powerReportTextObject.SetActive(true);
				this.powerReportTip.SetDelegate("BodyText", () => HabitatsScreenController.PowerReport(hab));
				return;
			}
			this.powerReportTextObject.SetActive(false);
		}

		// Token: 0x06005087 RID: 20615 RVA: 0x0022F708 File Offset: 0x0022D908
		private static string PowerReport(TIHabState hab)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(hab.displayName);
			int num = hab.NetPower(false, false);
			int num2 = (from x in hab.ActiveModules()
				where x.PowerProvider()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower());
			int num3 = (from x in hab.CompletedModules()
				where !x.powered && x.PowerProvider()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower());
			int num4 = (from x in hab.UnderConstructionModules()
				where x.PowerProvider()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower());
			int num5 = (from x in hab.ActiveModules()
				where x.PowerConsumer()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.PowerConsumed());
			int num6 = (from x in hab.CompletedModules()
				where !x.powered && x.PowerConsumer()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.PowerConsumed());
			int num7 = (from x in hab.UnderConstructionModules()
				where x.PowerConsumer()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.PowerConsumed());
			stringBuilder.AppendLine(Loc.T("UI.Habs.CurrentAvailablePower", new object[] { num.ToString("N0") }));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(Loc.T("UI.Habs.PowerProduced", new object[] { num2.ToString("N0") }));
			stringBuilder.AppendLine(Loc.T("UI.Habs.PPDM", new object[] { num3.ToString("N0") }));
			stringBuilder.AppendLine(Loc.T("UI.Habs.PPUCM", new object[] { num4.ToString("N0") }));
			stringBuilder.AppendLine(Loc.T("UI.Habs.TPAP", new object[] { (num2 + num3 + num4).ToString("N0") }));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(Loc.T("UI.Habs.PowerConsumed", new object[] { num5.ToString("N0") }));
			stringBuilder.AppendLine(Loc.T("UI.Habs.PPCDM", new object[] { num6.ToString("N0") }));
			stringBuilder.AppendLine(Loc.T("UI.Habs.PPCUCM", new object[] { num7.ToString("N0") }));
			stringBuilder.AppendLine(Loc.T("UI.Habs.TPPC", new object[] { (num5 + num6 + num7).ToString("N0") }));
			return stringBuilder.ToString();
		}

		// Token: 0x06005088 RID: 20616 RVA: 0x0022FA8C File Offset: 0x0022DC8C
		private float TotalPotentialPowerConsumption(TIHabState hab)
		{
			return (float)((from x in hab.ActiveModules()
				where x.PowerConsumer()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.PowerConsumed()) + (from x in hab.CompletedModules()
				where !x.powered && x.PowerConsumer()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.PowerConsumed()) + (from x in hab.UnderConstructionModules()
				where x.PowerConsumer()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.PowerConsumed()));
		}

		// Token: 0x06005089 RID: 20617 RVA: 0x0022FB88 File Offset: 0x0022DD88
		private float TotalPotentialAvailablePower(TIHabState hab)
		{
			return (float)((from x in hab.ActiveModules()
				where x.PowerProvider()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower()) + (from x in hab.CompletedModules()
				where !x.powered && x.PowerProvider()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower()) + (from x in hab.UnderConstructionModules()
				where x.PowerProvider()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower()));
		}

		// Token: 0x0600508A RID: 20618 RVA: 0x0022FC84 File Offset: 0x0022DE84
		private float TotalCurrentAvailablePower(TIHabState hab)
		{
			return (float)((from x in hab.ActiveModules()
				where x.PowerProvider()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower()) + (from x in hab.CompletedModules()
				where !x.powered && x.PowerProvider()
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower()));
		}

		// Token: 0x0600508B RID: 20619 RVA: 0x0022FD30 File Offset: 0x0022DF30
		private void UpdateCouncilorGrid()
		{
			List<TICouncilorState> list = this.habToDisplay.CouncilorsPresentAndKnownToFaction(base.activePlayer, false, null);
			if (list.Count > 0 || this.habToDisplay.officersOnBoard.Count > 0)
			{
				this.councilorGridPanel.SetActive(true);
				bool flag = base.activePlayer == this.habToDisplay.faction && this.habToDisplay.officersOnBoard.Count > 0;
				this.councilorGrid.SetListSize<HabCouncilorGridItemController>(list.Count + ((base.activePlayer == this.habToDisplay.faction && flag) ? 1 : 0), false, false);
				int num = 0;
				using (IEnumerator<object> enumerator = this.councilorGrid.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (HabitatsScreenController.<>o__317.<>p__0 == null)
						{
							HabitatsScreenController.<>o__317.<>p__0 = CallSite<Func<CallSite, object, HabCouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(HabCouncilorGridItemController), typeof(HabitatsScreenController)));
						}
						HabCouncilorGridItemController habCouncilorGridItemController = HabitatsScreenController.<>o__317.<>p__0.Target(HabitatsScreenController.<>o__317.<>p__0, enumerator.Current);
						if (flag && num == list.Count)
						{
							habCouncilorGridItemController.UpdateGridItem(this.habToDisplay.officersOnBoard);
						}
						else
						{
							habCouncilorGridItemController.UpdateGridItem(base.activePlayer, list[num++]);
						}
					}
					return;
				}
			}
			this.councilorGridPanel.SetActive(false);
		}

		// Token: 0x0600508C RID: 20620 RVA: 0x0022FEA4 File Offset: 0x0022E0A4
		private void PreviewStation()
		{
			this.noHabSelected.SetActive(false);
			this.habSiteProductivityPanel.SetActive(false);
			this.baseDisplayCanvas.enabled = false;
			this.stationDisplayCanvas.enabled = true;
			this.SetModulesInteractable(this.habToDisplay.habType, null);
			for (int i = 4; i >= 0; i--)
			{
				TISectorState tisectorState = this.habToDisplay.sectors[i];
				if (tisectorState.faction == null)
				{
					for (int j = 0; j < 4; j++)
					{
						this.PreviewStationModule("blank", i, j, new TIHabModuleState(), false, this.habToDisplay.IsAlien(), this.habToDisplay);
					}
				}
				else
				{
					TIHabModuleTemplate[] array = new TIHabModuleTemplate[5];
					for (int k = 4; k >= 0; k--)
					{
						if (tisectorState.habModules.Count > k)
						{
							array[k] = tisectorState.habModules[k].moduleTemplate;
						}
					}
					int num = 0;
					while (num < 5 && tisectorState.habModules.Count > num)
					{
						string text = string.Format("habmodules/T{0}_Empty_Module", this.habToDisplay.tier);
						if (array[num] != null)
						{
							text = array[num].stationIconResource;
						}
						this.PreviewStationModule(text, i, num, tisectorState.habModules[num], base.activePlayer == tisectorState.faction, this.habToDisplay.IsAlien(), this.habToDisplay);
						num++;
					}
				}
			}
			if (this.habToDisplay.IsAlien())
			{
				Sprite sprite = GameControl.assetLoader.LoadAsset<Sprite>("habModules/station_T3_Hydra_Ring_Torus");
				this.torus1_2.sprite = sprite;
				this.torus2_3.sprite = sprite;
				this.torus3_4.sprite = sprite;
				this.torus4_1.sprite = sprite;
			}
			else
			{
				Sprite sprite2 = GameControl.assetLoader.LoadAsset<Sprite>("habModules/station_T3_Torus");
				this.torus1_2.sprite = sprite2;
				this.torus2_3.sprite = sprite2;
				this.torus3_4.sprite = sprite2;
				this.torus4_1.sprite = sprite2;
			}
			this.torus1_2.enabled = this.habToDisplay.ringStruct.NE;
			this.torus2_3.enabled = this.habToDisplay.ringStruct.SE;
			this.torus3_4.enabled = this.habToDisplay.ringStruct.SW;
			this.torus4_1.enabled = this.habToDisplay.ringStruct.NW;
		}

		// Token: 0x0600508D RID: 20621 RVA: 0x00230124 File Offset: 0x0022E324
		private void PreviewStationModule(string resourceName, int sector, int moduleSlot, TIHabModuleState habModule, bool playerControlled, bool alien, TIHabState hab)
		{
			StationGridCell stationGridCell;
			if (this.stationCellDictionary.TryGetValue(string.Format("S{0}_M{1}", sector, moduleSlot), out stationGridCell))
			{
				stationGridCell.SetModule(resourceName, playerControlled, alien, habModule, this.habToDisplay);
			}
		}

		// Token: 0x0600508E RID: 20622 RVA: 0x0023016C File Offset: 0x0022E36C
		private string BaseModuleResourceName(TIHabModuleState moduleState, int sector, int module)
		{
			TIHabModuleTemplate moduleTemplate = moduleState.moduleTemplate;
			string text;
			if (moduleTemplate != null)
			{
				if (sector == 0 && module == 1 && moduleTemplate.destroyed)
				{
					TIHabModuleTemplate priorModuleTemplate = moduleState.priorModuleTemplate;
					if (priorModuleTemplate != null && priorModuleTemplate.automated)
					{
						text = "habModules/base_T1_AutomatedMiningComplex_Destroyed";
					}
					else if (moduleTemplate.alienModule)
					{
						switch (moduleTemplate.tier)
						{
						default:
							text = "habModules/base_T1_AlienOutpostMiningComplex_Destroyed";
							break;
						case 2:
							text = "habModules/base_T2_AlienSettlementMiningComplex_Destroyed";
							break;
						case 3:
							text = "habModules/base_T3_AlienColonyMiningComplex_Destroyed";
							break;
						}
					}
					else
					{
						switch (moduleTemplate.tier)
						{
						default:
							text = "habModules/base_T1_OutpostMiningComplex_Destroyed";
							break;
						case 2:
							text = "habModules/base_T2_SettlementMiningComplex_Destroyed";
							break;
						case 3:
							text = "habModules/base_T3_ColonyMiningComplex_Destroyed";
							break;
						}
					}
				}
				else
				{
					text = moduleTemplate.baseIconResource;
				}
			}
			else
			{
				text = string.Format("habmodules/T{0}_Empty_Module", this.habToDisplay.tier);
			}
			return text;
		}

		// Token: 0x0600508F RID: 20623 RVA: 0x00230250 File Offset: 0x0022E450
		private void SetMineProductivityValues()
		{
			TIHabState tihabState = this.habToDisplay;
			if (tihabState != null && tihabState.IsBase)
			{
				if (base.activePlayer.Prospected(this.habToDisplay.habSite))
				{
					if (this.habToDisplay.HasMine && !this.habToDisplay.mine.decommissioning)
					{
						this.<SetMineProductivityValues>g__SetMineText|321_0(this.siteWater, FactionResource.Water);
						this.<SetMineProductivityValues>g__SetMineText|321_0(this.siteVolatiles, FactionResource.Volatiles);
						this.<SetMineProductivityValues>g__SetMineText|321_0(this.siteMetals, FactionResource.Metals);
						this.<SetMineProductivityValues>g__SetMineText|321_0(this.siteNobles, FactionResource.NobleMetals);
						this.<SetMineProductivityValues>g__SetMineText|321_0(this.siteFissiles, FactionResource.Fissiles);
						this.siteSolar.SetText(TIUtilities.FormatSmallNumber(this.habToDisplay.habSite.solarMultiplier, 7, 0, true, false));
					}
					else
					{
						this.siteWater.SetText(TIUtilities.FormatSmallNumber(this.habToDisplay.habSite.GetMonthlyProduction(FactionResource.Water), 7, 0, true, false));
						this.siteVolatiles.SetText(TIUtilities.FormatSmallNumber(this.habToDisplay.habSite.GetMonthlyProduction(FactionResource.Volatiles), 7, 0, true, false));
						this.siteMetals.SetText(TIUtilities.FormatSmallNumber(this.habToDisplay.habSite.GetMonthlyProduction(FactionResource.Metals), 7, 0, true, false));
						this.siteNobles.SetText(TIUtilities.FormatSmallNumber(this.habToDisplay.habSite.GetMonthlyProduction(FactionResource.NobleMetals), 7, 0, true, false));
						this.siteFissiles.SetText(TIUtilities.FormatSmallNumber(this.habToDisplay.habSite.GetMonthlyProduction(FactionResource.Fissiles), 7, 0, true, false));
						this.siteSolar.SetText(TIUtilities.FormatSmallNumber(this.habToDisplay.habSite.solarMultiplier, 7, 0, true, false));
					}
				}
				else
				{
					this.siteWater.SetText(HabSiteController.GetInlineResourceOutputIcon(FactionResource.Water, this.habToDisplay.habSite.miningProfile, this.habToDisplay.habSite.GetHabSiteExpectedProductivity_day(FactionResource.Water)));
					this.siteVolatiles.SetText(HabSiteController.GetInlineResourceOutputIcon(FactionResource.Volatiles, this.habToDisplay.habSite.miningProfile, this.habToDisplay.habSite.GetHabSiteExpectedProductivity_day(FactionResource.Volatiles)));
					this.siteMetals.SetText(HabSiteController.GetInlineResourceOutputIcon(FactionResource.Metals, this.habToDisplay.habSite.miningProfile, this.habToDisplay.habSite.GetHabSiteExpectedProductivity_day(FactionResource.Metals)));
					this.siteNobles.SetText(HabSiteController.GetInlineResourceOutputIcon(FactionResource.NobleMetals, this.habToDisplay.habSite.miningProfile, this.habToDisplay.habSite.GetHabSiteExpectedProductivity_day(FactionResource.NobleMetals)));
					this.siteFissiles.SetText(HabSiteController.GetInlineResourceOutputIcon(FactionResource.Fissiles, this.habToDisplay.habSite.miningProfile, this.habToDisplay.habSite.GetHabSiteExpectedProductivity_day(FactionResource.Fissiles)));
					this.siteSolar.SetText(this.habToDisplay.habSite.ref_spaceBody.SolarInsolationIconPath(true));
				}
				this.habSiteProductivityPanel.SetActive(true);
				return;
			}
			this.habSiteProductivityPanel.SetActive(false);
		}

		// Token: 0x06005090 RID: 20624 RVA: 0x00230544 File Offset: 0x0022E744
		private void PreviewBase()
		{
			this.noHabSelected.SetActive(false);
			this.baseDisplayCanvas.enabled = true;
			GameControl.assetLoader.LoadAssetForImageAssignment(this.habToDisplay.habSite.template.backgroundPath, this.baseSurfaceImage);
			this.baseSurfaceImage.enabled = true;
			this.stationDisplayCanvas.enabled = false;
			this.SetModulesInteractable(this.habToDisplay.habType, null);
			for (int i = 0; i < 5; i++)
			{
				if (this.habToDisplay.sectors[i].faction == null)
				{
					for (int j = 0; j < 4; j++)
					{
						this.PreviewBaseModule("blank", i, j, this.habToDisplay.IsAlien());
					}
				}
				else
				{
					TISectorState tisectorState = this.habToDisplay.sectors[i];
					TIHabModuleTemplate[] array = new TIHabModuleTemplate[5];
					for (int k = 4; k >= 0; k--)
					{
						if (tisectorState.habModules.Count > k)
						{
							array[k] = tisectorState.habModules[k].moduleTemplate;
						}
					}
					int num = 0;
					while (num < 5 && tisectorState.habModules.Count > num)
					{
						this.PreviewBaseModule(this.BaseModuleResourceName(this.habToDisplay.sectors[i].habModules[num], i, num), i, num, base.activePlayer == tisectorState.faction);
						num++;
					}
				}
			}
			this.baseCellDictionary["C_42"].UpdateConnections(!this.habToDisplay.connStruct.C42);
			this.baseCellDictionary["C_16"].UpdateConnections(!this.habToDisplay.connStruct.C16);
			this.baseCellDictionary["C_36"].UpdateConnections(!this.habToDisplay.connStruct.C36);
			this.baseCellDictionary["C_46"].UpdateConnections(!this.habToDisplay.connStruct.C46);
			this.baseCellDictionary["C_56"].UpdateConnections(!this.habToDisplay.connStruct.C56);
			this.baseCellDictionary["C_76"].UpdateConnections(!this.habToDisplay.connStruct.C76);
			this.baseCellDictionary["C_42"].SetAllConnectionSprites(this.habToDisplay.IsAlien());
			this.baseCellDictionary["C_16"].SetAllConnectionSprites(this.habToDisplay.IsAlien());
			this.baseCellDictionary["C_36"].SetAllConnectionSprites(this.habToDisplay.IsAlien());
			this.baseCellDictionary["C_46"].SetAllConnectionSprites(this.habToDisplay.IsAlien());
			this.baseCellDictionary["C_56"].SetAllConnectionSprites(this.habToDisplay.IsAlien());
			this.baseCellDictionary["C_76"].SetAllConnectionSprites(this.habToDisplay.IsAlien());
			this.SetMineProductivityValues();
			this.OnHabZoomSliderSet();
		}

		// Token: 0x06005091 RID: 20625 RVA: 0x0023087C File Offset: 0x0022EA7C
		private void PreviewBaseModule(string resourceName, int sector, int module, bool playerControlled)
		{
			BaseGridCell baseGridCell;
			if (this.baseCellDictionary.TryGetValue(string.Format("S{0}_M{1}", sector, module), out baseGridCell))
			{
				baseGridCell.SetModule(resourceName, playerControlled, this.habToDisplay.IsAlien(), this.habToDisplay.sectors[sector].habModules[module], this.habToDisplay);
			}
		}

		// Token: 0x06005092 RID: 20626 RVA: 0x002308E4 File Offset: 0x0022EAE4
		public void SelectModule(HabGridCell item)
		{
			HabType habType = this.habToDisplay.habType;
			if (habType != HabType.Station)
			{
				if (habType == HabType.Base)
				{
					this.selectedBaseModule = item;
				}
			}
			else
			{
				this.selectedStationModule = item;
			}
			this.SetModulesInteractable(this.habToDisplay.habType, null);
			if (item != null)
			{
				this.displayModuleSector = item.sector;
				this.displayModuleSlot = item.module;
			}
			else
			{
				this.displayModuleSector = -1;
				this.displayModuleSlot = -1;
			}
			if (!item.isEmpty)
			{
				using (List<HabModuleListItem>.Enumerator enumerator = this.installedModuleListItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						HabModuleListItem habModuleListItem = enumerator.Current;
						if (habModuleListItem.habGridCell != null && habModuleListItem.habGridCell == item)
						{
							this.SetMenuToSelectedModule(habModuleListItem);
							break;
						}
					}
					goto IL_00CB;
				}
			}
			this.SetMenuToSelectedModule(null);
			IL_00CB:
			this.UpdateModulePreviewText(false, false);
			if (this.queryDecommissionModule)
			{
				this.managementQueryObject.SetActive(false);
				this.queryDecommissionModule = false;
			}
		}

		// Token: 0x06005093 RID: 20627 RVA: 0x002309F0 File Offset: 0x0022EBF0
		public void UpdateModulePreviewText(bool viewProspectiveModule, bool clear = false)
		{
			if (clear || (viewProspectiveModule && (this.prospectiveModule == null || (this.prospectiveModule.habType == HabType.Base && this.habToDisplay.IsStation) || (this.prospectiveModule.habType == HabType.Station && this.habToDisplay.IsBase))) || (!viewProspectiveModule && (this.displayModuleSector == -1 || this.habToDisplay.sectors[this.displayModuleSector].habModules[this.displayModuleSlot].empty)))
			{
				if (this.habToDisplay != null && this.displayModuleSector >= 0 && this.displayModuleSector < this.habToDisplay.sectors.Count && this.displayModuleSlot >= 0 && this.displayModuleSlot < this.habToDisplay.sectors[this.displayModuleSector].habModules.Count)
				{
					if (!viewProspectiveModule)
					{
						this.ChangeModuleMode(this.habToDisplay.sectors[this.displayModuleSector].habModules[this.displayModuleSlot].empty);
					}
					else
					{
						this.ChangeModuleMode(viewProspectiveModule);
					}
				}
				else
				{
					this.ChangeModuleMode(viewProspectiveModule);
				}
				if (this.habToDisplay != null && (this.habToDisplay.underBombardment || this.habToDisplay.underAssault || this.habToDisplay.decommissioning))
				{
					StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Habs.SelectAModule")).AppendLine().AppendLine(Loc.T("UI.Habs.CantBuild"));
					this.modulePanelHeaderText.SetText(stringBuilder);
				}
				else
				{
					this.modulePanelHeaderText.SetText(Loc.T("UI.Habs.SelectAModule"));
				}
				this.summaryScrollViewContainer.gameObject.SetActive(false);
				this.sectorOwnerGO.SetActive(false);
				this.summaryPanel.SetActive(false);
				this.moduleIcon.gameObject.SetActive(false);
				this.constructionDataPanel.SetActive(false);
				this.supportDataPanel.SetActive(false);
				this.incomeDataPanel.SetActive(false);
				this.upgradePanel.SetActive(false);
				this.powerPanel.SetActive(false);
				this.moduleInstalledPanel.SetActive(false);
				this.moduleUnderConstructionPanel.SetActive(false);
				this.DecommissionModuleButton.interactable = false;
				this.DecommissionModuleButton.gameObject.SetActive(false);
				return;
			}
			this.ChangeModuleMode(viewProspectiveModule);
			TIGlobalConfig global = TemplateManager.global;
			TIHabModuleState tihabModuleState = null;
			TIHabModuleTemplate moduleTemplate;
			if (!viewProspectiveModule)
			{
				tihabModuleState = this.habToDisplay.sectors[this.displayModuleSector].habModules[this.displayModuleSlot];
				moduleTemplate = tihabModuleState.moduleTemplate;
				string text;
				if (moduleTemplate == null)
				{
					text = null;
				}
				else
				{
					TIHabModuleTemplate upgradesTo = moduleTemplate.UpgradesTo;
					text = ((upgradesTo != null) ? upgradesTo.dataName : null);
				}
				this.moduleUpgradeDataName = text;
			}
			else
			{
				moduleTemplate = this.prospectiveModule;
			}
			this.modulePanelHeaderText.SetText(moduleTemplate.displayName);
			if (viewProspectiveModule)
			{
				this.sectorOwnerGO.SetActive(true);
				this.sectorOwner.sprite = base.activePlayer.factionIcon64UI;
				this.sectorText.SetText(Loc.T("UI.Habs.ProspectiveModuleData"));
				this.moduleInstalledPanel.SetActive(false);
				this.moduleUnderConstructionPanel.SetActive(false);
				this.constructionDataPanel.SetActive(true);
				StringBuilder stringBuilder2 = new StringBuilder();
				TIResourcesCost tiresourcesCost = moduleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, false, false, 0, false);
				stringBuilder2.AppendLine(Loc.T("UI.Habs.BaseSpaceCost", new object[] { tiresourcesCost.GetString("Relevant", false, false, false, 2, false, false, null, false, FactionResource.None) }));
				string text2 = Loc.T("TIResourceCost.OnEarth", new object[] { moduleTemplate.CostFromEarth(base.activePlayer, this.habToDisplay, false).GetString("Relevant", false, true, false, 7, false, false, base.activePlayer, false, FactionResource.None) });
				stringBuilder2.AppendLine(text2);
				string text3 = Loc.T("TIResourceCost.InSpace", new object[] { moduleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, false, true, 0, false).GetString("Relevant", false, true, false, 7, false, false, base.activePlayer, false, FactionResource.None) });
				stringBuilder2.AppendLine(text3);
				if (moduleTemplate.CanUpgrade(base.activePlayer) && this.habToDisplay.ModuleUpgradePrereqModuleAlreadyOnHab(moduleTemplate))
				{
					TIResourcesCost tiresourcesCost2 = moduleTemplate.CostFromEarth(base.activePlayer, this.habToDisplay, true);
					string text4 = Loc.T("UI.Habs.UpgradeFromEarth", new object[]
					{
						moduleTemplate.UpgradesFrom.displayName,
						tiresourcesCost2.GetString("Relevant", false, true, false, 7, false, false, base.activePlayer, false, FactionResource.None)
					});
					stringBuilder2.AppendLine(text4);
					int num;
					int num2;
					this.habToDisplay.GetUpgradeModuleLocation(moduleTemplate, out num, out num2);
					TIResourcesCost tiresourcesCost3 = moduleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, true, false, 0, false);
					string text5 = Loc.T("UI.Habs.UpgradeFromSpace", new object[]
					{
						moduleTemplate.UpgradesFrom.displayName,
						tiresourcesCost3.GetString("Relevant", false, true, false, 7, false, false, base.activePlayer, false, FactionResource.None)
					});
					stringBuilder2.AppendLine(text5);
				}
				this.crewText.SetText(Loc.T("UI.Habs.Crew", new object[] { moduleTemplate.crew.ToString("N0") }));
				this.constructionCostString.SetText(stringBuilder2.ToString());
				this.powerPanel.SetActive(false);
				this.DecommissionModuleButton.interactable = false;
				this.DecommissionModuleButton.gameObject.SetActive(false);
				this.shipyardButton.gameObject.SetActive(false);
			}
			else
			{
				TIHabModuleState tihabModuleState2 = this.habToDisplay.sectors[this.displayModuleSector].habModules[this.displayModuleSlot];
				this.sectorOwnerGO.SetActive(true);
				this.moduleInstalledPanel.SetActive(!tihabModuleState2.underConstruction && !tihabModuleState2.decommissioning);
				if (tihabModuleState2.underConstruction)
				{
					this.moduleUnderConstructionPanel.SetActive(true);
					if (tihabModuleState2.GetFaction().IsActiveHumanFaction || tihabModuleState2.GetFaction().permanentAlly(base.activePlayer))
					{
						this.moduleCompletionDateText.SetText(Loc.T("UI.Habs.CompletionDate", new object[] { tihabModuleState2.completionDate.ToShortDateString() }));
					}
					else
					{
						this.moduleCompletionDateText.SetText(Loc.T("UI.Habs.UnderConstruction"));
					}
				}
				else
				{
					this.moduleUnderConstructionPanel.SetActive(false);
				}
				this.crewText.SetText(Loc.T("UI.Habs.Crew", new object[] { tihabModuleState2.crew.ToString("N0") }));
				this.sectorOwner.sprite = this.habToDisplay.sectors[this.displayModuleSector].faction.factionIcon64UI;
				this.sectorText.SetText(this.habToDisplay.sectors[this.displayModuleSector].shortSectorString);
				this.constructionDataPanel.SetActive(false);
				this.powerPanel.SetActive(true);
				this.DecommissionModuleButton.interactable = tihabModuleState2.CanDecommissionModule(tihabModuleState.underConstruction && tihabModuleState2.DecommissionDuration_days() <= 0f);
				if (tihabModuleState2.DecommissionDuration_days() > 0f)
				{
					this.DecommissionModuleButtonText.SetText(Loc.T("UI.Habs.DecommissionModuleButton"));
				}
				else
				{
					this.DecommissionModuleButtonText.SetText(Loc.T("UI.Habs.CancelModuleConstructionButton"));
				}
				this.DecommissionModuleButton.gameObject.SetActive(this.DecommissionModuleButton.interactable);
				if (tihabModuleState2 != null && tihabModuleState2.hasModule)
				{
					this.shipyardButton.gameObject.SetActive(tihabModuleState2.moduleTemplate.allowsShipConstruction);
					this.shipyardButton.interactable = tihabModuleState2.moduleTemplate.allowsShipConstruction && tihabModuleState2.active;
				}
				else
				{
					this.shipyardButton.gameObject.SetActive(false);
				}
			}
			this.summaryPanel.SetActive(true);
			this.moduleIcon.gameObject.SetActive(true);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.habToDisplay.IsBase ? moduleTemplate.baseIconResource : moduleTemplate.stationIconResource, this.moduleIcon);
			this.tierText.SetText(Loc.T("UI.Habs.Tier", new object[] { moduleTemplate.tier.ToString("N0") }));
			this.tierFrame.sprite = this.tierFrameSprites[moduleTemplate.tier];
			this.massText.SetText(Loc.T("UI.Habs.Mass", new object[] { moduleTemplate.Mass_tons(this.habToDisplay.irradiatedMultiplier, this.habToDisplay.ref_spaceBody, this.habToDisplay.ref_naturalSpaceObject, base.activePlayer).ToString("N0") }));
			List<HabitatsScreenController.IncomeEntry> list = new List<HabitatsScreenController.IncomeEntry>();
			int num3 = moduleTemplate.ProspectivePower(this.habToDisplay);
			if (num3 > 0)
			{
				list.Add(new HabitatsScreenController.IncomeEntry((num3 >= 0) ? global.pathHabPowerIcon : global.pathHabPowerAlertIcon, num3.ToString("N0")));
			}
			Dictionary<FactionResource, float> dictionary = new Dictionary<FactionResource, float>();
			foreach (FactionResource factionResource in Enums.FactionResources)
			{
				dictionary[factionResource] = moduleTemplate.MonthlyResourceIncome(factionResource, this.habToDisplay, viewProspectiveModule ? base.activePlayer : this.habToDisplay.sectors[this.displayModuleSector].faction);
				string text6 = TIUtilities.FormatBigOrSmallNumber(dictionary[factionResource], 1, 3, 0, true, false);
				if (!viewProspectiveModule && !this.habToDisplay.sectors[this.displayModuleSector].habModules[this.displayModuleSlot].active)
				{
					text6 = TIUtilities.RedLine(text6);
				}
				if (dictionary[factionResource] > 0f)
				{
					list.Add(new HabitatsScreenController.IncomeEntry(TIUtilities.PathResourceIcon(factionResource), text6));
				}
			}
			int num4 = moduleTemplate.ControlPointCapacity(this.habToDisplay.inEarthLEO);
			if (num4 != 0)
			{
				list.Add(new HabitatsScreenController.IncomeEntry(TemplateManager.global.pathEmptyControlPoint, num4.ToString("N0")));
			}
			foreach (TechCategory techCategory in Enums.TechCategories)
			{
				float techBonusByCategory = moduleTemplate.GetTechBonusByCategory(techCategory);
				string text7 = techBonusByCategory.ToPercent((techBonusByCategory * 100f % 1f > 0f) ? "P1" : "P0");
				if (!viewProspectiveModule && !this.habToDisplay.sectors[this.displayModuleSector].habModules[this.displayModuleSlot].active)
				{
					text7 = TIUtilities.RedLine(text7);
				}
				if (techBonusByCategory != 0f)
				{
					list.Add(new HabitatsScreenController.IncomeEntry(TIGenericTechTemplate.PathTechCategoryIcon(techCategory), text7));
				}
			}
			if (moduleTemplate.allowsResupply)
			{
				list.Add(new HabitatsScreenController.IncomeEntry(global.pathHabResupplyIcon, string.Empty));
			}
			float moduleConstructionSpeedModifier = moduleTemplate.moduleConstructionSpeedModifier;
			if (moduleConstructionSpeedModifier > 1f)
			{
				list.Add(new HabitatsScreenController.IncomeEntry(global.pathHabModuleConstructionIcon, (1f - moduleConstructionSpeedModifier).ToPercent("P0")));
			}
			if (moduleTemplate.allowsShipConstruction)
			{
				list.Add(new HabitatsScreenController.IncomeEntry(global.pathHabShipyardIcon, string.Empty));
			}
			float num5;
			if (viewProspectiveModule)
			{
				num5 = moduleTemplate.SpaceCombatValue(base.activePlayer, this.habToDisplay, true);
			}
			else
			{
				num5 = tihabModuleState.SpaceCombatValue();
			}
			if (num5 != 0f)
			{
				string text8 = num5.ToString("N0");
				if (!viewProspectiveModule && !this.habToDisplay.sectors[this.displayModuleSector].habModules[this.displayModuleSlot].active)
				{
					text8 = TIUtilities.RedLine(text8);
				}
				list.Add(new HabitatsScreenController.IncomeEntry(global.pathHabDefenseIcon, text8));
			}
			if (list.Count > 0)
			{
				this.incomeDataPanel.SetActive(true);
				this.incomeGrid.SetListSize<ResourceGridItemController>(list.Count, false, false);
				int num6 = 0;
				using (IEnumerator<object> enumerator = this.incomeGrid.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (HabitatsScreenController.<>o__366.<>p__0 == null)
						{
							HabitatsScreenController.<>o__366.<>p__0 = CallSite<Func<CallSite, object, ResourceGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ResourceGridItemController), typeof(HabitatsScreenController)));
						}
						HabitatsScreenController.<>o__366.<>p__0.Target(HabitatsScreenController.<>o__366.<>p__0, enumerator.Current).UpdateListItem(list[num6].iconResourcePath, list[num6++].value);
					}
					goto IL_0C91;
				}
			}
			this.incomeDataPanel.SetActive(false);
			IL_0C91:
			this.supportDataPanel.SetActive(true);
			this.supportCostString.SetText(this.ModuleSupportIconList(moduleTemplate, viewProspectiveModule, (float)num3, dictionary[FactionResource.MissionControl]));
			StringBuilder stringBuilder3 = new StringBuilder();
			for (int j = 0; j < moduleTemplate.SpecialRules.Count; j++)
			{
				string text9 = Loc.T(new StringBuilder("UI.Habs.Summary.").Append(moduleTemplate.SpecialRules[j]).ToString());
				stringBuilder3.AppendLine(text9);
			}
			if (!viewProspectiveModule)
			{
				if (this.habToDisplay.AllowedModules(base.activePlayer).Contains(moduleTemplate.UpgradesTo) && this.habToDisplay.sectors[this.displayModuleSector].ValidModuleForSlot(moduleTemplate.UpgradesTo, this.displayModuleSlot) && !this.habToDisplay.sectors[this.displayModuleSector].habModules[this.displayModuleSlot].decommissioning)
				{
					this.upgradePanel.SetActive(true);
					StringBuilder stringBuilder4 = new StringBuilder(moduleTemplate.UpgradesTo.displayName);
					TIResourcesCost tiresourcesCost4 = moduleTemplate.UpgradesTo.CostFromEarth(base.activePlayer, this.habToDisplay, true);
					TIResourcesCost tiresourcesCost5 = moduleTemplate.UpgradesTo.CostFromSpace(base.activePlayer, this.habToDisplay, true, false, 0, false);
					stringBuilder4.AppendLine();
					stringBuilder4.AppendLine(Loc.T("UI.Habs.CostFromEarth", new object[] { tiresourcesCost4.GetString("Relevant", false, true, false, 2, false, false, null, false, FactionResource.None) }));
					stringBuilder4.AppendLine(Loc.T("UI.Habs.CostFromSpace", new object[] { tiresourcesCost5.GetString("Relevant", false, true, false, 2, false, false, null, false, FactionResource.None) }));
					this.upgradeModuleName.SetText(stringBuilder4.ToString());
					this.moduleUpgradeButton.interactable = tiresourcesCost4.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity) || tiresourcesCost5.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
					List<TIHabModuleState> list2 = this.habToDisplay.UpgradeCandidates(moduleTemplate);
					if (!this.habToDisplay.decommissioning && list2.Count > 1)
					{
						TIResourcesCost tiresourcesCost6 = this.habToDisplay.FullUpgradeCost(moduleTemplate, false);
						TIResourcesCost tiresourcesCost7 = this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost6, null);
						this.moduleUpgradeAllOfTypeButton.interactable = tiresourcesCost6.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity) || tiresourcesCost7.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
						this.moduleUpgradeAllOfTypeButton.gameObject.SetActive(true);
					}
					else
					{
						this.moduleUpgradeAllOfTypeButton.interactable = false;
						this.moduleUpgradeAllOfTypeButton.gameObject.SetActive(true);
					}
				}
				else
				{
					this.upgradePanel.SetActive(false);
					this.moduleUpgradeAllOfTypeButton.interactable = false;
					this.moduleUpgradeAllOfTypeButton.gameObject.SetActive(false);
				}
				this.UpdateModulePowerStatus();
				this.DecommissionModuleButton.interactable = tihabModuleState.CanDecommissionModule(tihabModuleState.underConstruction && tihabModuleState.DecommissionDuration_days() <= 0f);
				this.DecommissionModuleButton.gameObject.SetActive(true);
			}
			else
			{
				this.upgradePanel.SetActive(false);
				this.powerPanel.SetActive(false);
				this.DecommissionModuleButton.interactable = false;
				this.DecommissionModuleButton.gameObject.SetActive(false);
				this.moduleUpgradeAllOfTypeButton.interactable = false;
				this.moduleUpgradeAllOfTypeButton.gameObject.SetActive(false);
			}
			this.summaryScrollViewContainer.gameObject.SetActive(true);
		}

		// Token: 0x06005094 RID: 20628 RVA: 0x00231A3C File Offset: 0x0022FC3C
		private string ModuleSupportIconList(TIHabModuleTemplate module, bool showPower, float power, float missionControl)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (showPower && power < 0f)
			{
				stringBuilder.Append(TemplateManager.global.habPowerInlineSpritePath).Append(power.ToString("N0")).Append(" ");
			}
			if (missionControl < 0f)
			{
				stringBuilder.Append(TemplateManager.global.missionControlInlineSpritePath).Append(missionControl.ToString("N0")).Append(" ");
			}
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			tiresourcesCost.AddCost(FactionResource.Money, module.MonthlySupportCost(FactionResource.Money, true, base.activePlayer, this.habToDisplay), true);
			tiresourcesCost.AddCost(FactionResource.Boost, module.MonthlySupportCost(FactionResource.Boost, true, base.activePlayer, this.habToDisplay), true);
			tiresourcesCost.AddCost(FactionResource.Water, module.MonthlySupportCost(FactionResource.Water, true, base.activePlayer, this.habToDisplay), true);
			tiresourcesCost.AddCost(FactionResource.Volatiles, module.MonthlySupportCost(FactionResource.Volatiles, true, base.activePlayer, this.habToDisplay), true);
			tiresourcesCost.AddCost(FactionResource.Metals, module.MonthlySupportCost(FactionResource.Metals, true, base.activePlayer, this.habToDisplay), true);
			tiresourcesCost.AddCost(FactionResource.NobleMetals, module.MonthlySupportCost(FactionResource.NobleMetals, true, base.activePlayer, this.habToDisplay), true);
			tiresourcesCost.AddCost(FactionResource.Fissiles, module.MonthlySupportCost(FactionResource.Fissiles, true, base.activePlayer, this.habToDisplay), true);
			tiresourcesCost.AddCost(FactionResource.Antimatter, module.MonthlySupportCost(FactionResource.Antimatter, true, base.activePlayer, this.habToDisplay), true);
			tiresourcesCost.AddCost(FactionResource.Exotics, module.MonthlySupportCost(FactionResource.Exotics, true, base.activePlayer, this.habToDisplay), true);
			stringBuilder.Append(tiresourcesCost.GetString("N2", false, false, false, 7, false, false, null, false, FactionResource.None));
			return stringBuilder.ToString();
		}

		// Token: 0x06005095 RID: 20629 RVA: 0x00231BEA File Offset: 0x0022FDEA
		public void OnShipyardButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.CloseInfoScreen(false);
			GameControl.eventManager.TriggerEvent(new ShipyardUIRequested(base.activePlayer), null, new object[] { base.activePlayer });
		}

		// Token: 0x06005096 RID: 20630 RVA: 0x00231C24 File Offset: 0x0022FE24
		public void OnRebuildAllSelected()
		{
			if (this.habToDisplay.faction == base.activePlayer)
			{
				this.CloseModuleBuildPanel();
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
				this.managementQueryConfirmButtonText.SetText(Loc.T("UI.Habs.RebuildAll"));
				TIResourcesCost tiresourcesCost = this.habToDisplay.FullRebuildCost();
				bool flag = tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
				if (!flag)
				{
					tiresourcesCost2 = this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost, null);
					flag = tiresourcesCost2.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				}
				this.managementQueryConfirmButton.onClick.RemoveAllListeners();
				this.managementQueryConfirmButton.onClick.AddListener(new UnityAction(this.OnRebuildAllConfirmed));
				this.managementQueryConfirmButton.interactable = flag;
				string text = "UI.Habs.RebuildAllQuery";
				object[] array = new object[1];
				array[0] = TIUtilities.ConstructTextList((from x in this.habToDisplay.RebuildCandidates()
					select x.priorModuleTemplate).ToList<TIDataTemplate>(), false, false);
				StringBuilder stringBuilder = new StringBuilder(Loc.T(text, array));
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Habs.BaseCost", new object[] { tiresourcesCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				if (tiresourcesCost2.anyDebit)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.OurCost", new object[] { tiresourcesCost2.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				}
				if (!flag)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.CantAffordRebuild"));
				}
				this.managementQueryText.SetText(stringBuilder.ToString());
				this.managementQueryObject.SetActive(true);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005097 RID: 20631 RVA: 0x00231DFC File Offset: 0x0022FFFC
		public void OnRebuildAllConfirmed()
		{
			List<TIHabModuleState> list = this.habToDisplay.RebuildCandidates();
			bool flag = false;
			foreach (TIHabModuleState tihabModuleState in list)
			{
				TIResourcesCost tiresourcesCost = tihabModuleState.priorModuleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, false, false, 0, false);
				bool flag2 = true;
				if (!tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
				{
					tiresourcesCost = tihabModuleState.priorModuleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, false, true, 0, false);
					if (!tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					base.activePlayer.playerControl.StartAction(new BuildHabModuleAction(tihabModuleState.priorModuleTemplate, tihabModuleState.sector, tihabModuleState.slot, tiresourcesCost, null));
					flag = true;
				}
			}
			if (flag)
			{
				SoundEffectController.PlayBuildHabModuleSound(list[0].moduleTemplate, this.habToDisplay);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			this.managementQueryObject.SetActive(false);
			this.SetManageButtonStatusAndText();
		}

		// Token: 0x06005098 RID: 20632 RVA: 0x00231F38 File Offset: 0x00230138
		public void OnUpgradeAllSelected()
		{
			if (this.habToDisplay.faction == base.activePlayer)
			{
				this.CloseModuleBuildPanel();
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
				this.managementQueryConfirmButtonText.SetText(Loc.T("UI.Habs.UpgradeAll"));
				TIResourcesCost tiresourcesCost = this.habToDisplay.FullUpgradeCost();
				bool flag = tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
				if (!flag)
				{
					tiresourcesCost2 = this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost, null);
					flag = tiresourcesCost2.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				}
				this.managementQueryConfirmButton.onClick.RemoveAllListeners();
				this.managementQueryConfirmButton.onClick.AddListener(new UnityAction(this.OnUpgradeAllConfirmed));
				this.managementQueryConfirmButton.interactable = flag;
				string text = "UI.Habs.UpgradeAllQuery";
				object[] array = new object[2];
				array[0] = this.habToDisplay.UpgradeCandidates().Count;
				array[1] = this.habToDisplay.UpgradeCandidates().ToCommaSeparatedString<TIHabModuleState>((TIHabModuleState x) => Loc.T("UI.Habs.UpgradePath", new object[]
				{
					x.displayName,
					x.moduleTemplate.UpgradesTo.displayName
				}));
				StringBuilder stringBuilder = new StringBuilder(Loc.T(text, array));
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Habs.BaseCost", new object[] { tiresourcesCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				if (tiresourcesCost2.anyDebit)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.OurCost", new object[] { tiresourcesCost2.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				}
				if (!flag)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.CantAffordUpgradeAll"));
				}
				this.managementQueryText.SetText(stringBuilder.ToString());
				this.managementQueryObject.SetActive(true);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005099 RID: 20633 RVA: 0x0023211C File Offset: 0x0023031C
		public void OnUpgradeAllConfirmed()
		{
			List<TIHabModuleState> list = this.habToDisplay.UpgradeCandidates();
			this.UpgradeAllModulesSelected(list);
		}

		// Token: 0x0600509A RID: 20634 RVA: 0x0023213C File Offset: 0x0023033C
		private void UpgradeAllModulesSelected(List<TIHabModuleState> modules)
		{
			bool flag = false;
			foreach (TIHabModuleState tihabModuleState in modules)
			{
				TIResourcesCost tiresourcesCost = tihabModuleState.moduleTemplate.UpgradesTo.CostFromSpace(base.activePlayer, this.habToDisplay, true, false, 0, false);
				bool flag2 = true;
				if (!tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
				{
					tiresourcesCost = tihabModuleState.moduleTemplate.UpgradesTo.CostFromSpace(base.activePlayer, this.habToDisplay, true, true, 0, false);
					if (!tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					base.activePlayer.playerControl.StartAction(new BuildHabModuleAction(tihabModuleState.moduleTemplate.UpgradesTo, tihabModuleState.sector, tihabModuleState.slot, tiresourcesCost, null));
					flag = true;
				}
			}
			if (flag)
			{
				SoundEffectController.PlayBuildHabModuleSound(modules[0].moduleTemplate, this.habToDisplay);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			this.managementQueryObject.SetActive(false);
			this.SetManageButtonStatusAndText();
		}

		// Token: 0x0600509B RID: 20635 RVA: 0x00232274 File Offset: 0x00230474
		public void OnUpgradeAllOfTypeSelected()
		{
			if (this.habToDisplay.faction == base.activePlayer && this.selectedModule.habModule.moduleTemplate.CanUpgrade(this.habToDisplay.faction))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
				this.managementQueryConfirmButtonText.SetText(Loc.T("UI.Habs.UpgradeAllOfType"));
				TIResourcesCost tiresourcesCost = this.habToDisplay.FullUpgradeCost(this.selectedModule.habModule.moduleTemplate, false);
				bool flag = tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
				if (!flag)
				{
					tiresourcesCost2 = this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost, null);
					flag = tiresourcesCost2.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				}
				this.managementQueryConfirmButton.onClick.RemoveAllListeners();
				this.managementQueryConfirmButton.onClick.AddListener(new UnityAction(this.OnUpgradeAllOfTypeConfirmed));
				this.managementQueryConfirmButton.interactable = flag;
				StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Habs.UpgradeAllofTypeQuery", new object[]
				{
					this.habToDisplay.UpgradeCandidates(this.selectedModule.habModule.moduleTemplate).Count,
					this.selectedModule.habModule.moduleTemplate.displayName,
					this.selectedModule.habModule.moduleTemplate.UpgradesTo.displayName
				}));
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Habs.BaseCost", new object[] { tiresourcesCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				if (tiresourcesCost2.anyDebit)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.OurCost", new object[] { tiresourcesCost2.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				}
				if (!flag)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.CantAffordUpgradeAllofType"));
				}
				this.managementQueryText.SetText(stringBuilder.ToString());
				this.managementQueryObject.SetActive(true);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x0600509C RID: 20636 RVA: 0x0023249C File Offset: 0x0023069C
		public void OnUpgradeAllOfTypeConfirmed()
		{
			List<TIHabModuleState> list = this.habToDisplay.UpgradeCandidates(this.selectedModule.habModule.moduleTemplate);
			this.UpgradeAllModulesSelected(list);
		}

		// Token: 0x0600509D RID: 20637 RVA: 0x002324CC File Offset: 0x002306CC
		private void SetShowCopySaveHabTemplateButtons()
		{
			if (this.IsManaging())
			{
				this.saveHabButton.interactable = this.CanSaveHab();
				this.saveHabButton.gameObject.SetActive(true);
				this.manageHabTemplatesButton.interactable = base.activePlayer.habDesigns.Count > 0 && !this.manageHabTemplatesPanel.activeSelf;
				this.manageHabTemplatesButton.gameObject.SetActive(true);
			}
			else if (!this.applyingMassTemplates)
			{
				this.saveHabButton.gameObject.SetActive(false);
				this.managementQueryTemplateDropdownObject.SetActive(false);
				this.managementQuerySelectedHabDropdownObject.SetActive(false);
				this.managementQueryToggleObject.SetActive(false);
				this.manageHabTemplatesButton.interactable = false;
				this.manageHabTemplatesButton.gameObject.SetActive(false);
			}
			this.ResetQuickHabTemplateDropdown();
		}

		// Token: 0x0600509E RID: 20638 RVA: 0x002325A8 File Offset: 0x002307A8
		private bool CanSaveHab()
		{
			TIHabTemplate tihabTemplate = this.habToDisplay.ConvertToTemplate(base.activePlayer);
			return tihabTemplate != null && this.habToDisplay.faction == base.activePlayer && this.habToDisplay.OkayModules().Count >= 2 && tihabTemplate.AllModuleTemplates(false).Count == this.habToDisplay.OkayModules().Count && !base.activePlayer.IsDuplicateHabDesign(tihabTemplate);
		}

		// Token: 0x0600509F RID: 20639 RVA: 0x00232624 File Offset: 0x00230824
		public void OnSaveHabTemplateSelected()
		{
			if (this.CanSaveHab())
			{
				TIHabTemplate tihabTemplate = this.habToDisplay.ConvertToTemplate(base.activePlayer);
				base.activePlayer.playerControl.StartAction(new SaveHabDesignAction(base.activePlayer, tihabTemplate));
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			this.SetShowCopySaveHabTemplateButtons();
			this.PopulateQuickHabTemplateDropdown();
			this.OnCancelHabManagementButtonPressed(true);
			this.CloseModuleBuildPanel();
			if (this.manageHabTemplatesPanel.activeSelf)
			{
				this.RefreshHabTemplateManagerList();
			}
		}

		// Token: 0x060050A0 RID: 20640 RVA: 0x002326B0 File Offset: 0x002308B0
		public void OnCopyHabButtonPressed()
		{
			this.CloseModuleBuildPanel();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			this.managementQueryObject.SetActive(true);
			this.managementQueryConfirmButton.interactable = false;
			if (this.PopulateHabTemplateDropdown())
			{
				this.managementQueryText.SetText(Loc.T("UI.Habs.SelectHabTemplateQuery", new object[]
				{
					this.habToDisplay.displayName,
					base.activePlayer.displayNameWithColor,
					TemplateManager.global.habPowerInlineSpritePath
				}));
				this.managementQueryTemplateDropdownObject.SetActive(true);
				this.managementQuerySelectedHabDropdownObject.SetActive(false);
				this.managementQueryToggleObject.SetActive(true);
				this.managementQueryTemplateDropdown.captionText.SetText(Loc.T("UI.Habs.SelectTemplate"));
				this.managementQueryConfirmButtonObject.SetActive(true);
				this.managementQueryConfirmButtonText.SetText(Loc.T("UI.Habs.ApplySelectedTemplate"));
				this.managementQueryConfirmButton.onClick.RemoveAllListeners();
				this.managementQueryConfirmButton.onClick.AddListener(new UnityAction(this.OnConfirmApplyHabTemplateSelected));
			}
			else
			{
				this.managementQueryTemplateDropdownObject.SetActive(false);
				this.managementQuerySelectedHabDropdownObject.SetActive(false);
				this.managementQueryToggleObject.SetActive(false);
				this.managementQueryText.SetText("UI.Habs.NoValidTemplates");
			}
			this.SetShowCopySaveHabTemplateButtons();
		}

		// Token: 0x060050A1 RID: 20641 RVA: 0x00232800 File Offset: 0x00230A00
		public void OnHabTemplateSelected()
		{
			if (!string.IsNullOrEmpty(this.habTemplateDropdown[this.managementQueryTemplateDropdown.value]))
			{
				TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(this.habTemplateDropdown[this.managementQueryTemplateDropdown.value], false);
				TIResourcesCost tiresourcesCost;
				float num;
				List<TIHabModuleTemplate> list2;
				List<TIHabModuleTemplate> list = this.habToDisplay.ApplySavedTemplate(tihabTemplate, true, this.managementQueryToggle.isOn, out tiresourcesCost, out num, out list2);
				TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
				bool flag = true;
				if (!tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
				{
					tiresourcesCost2 = this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost, null);
					flag = tiresourcesCost2.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				}
				StringBuilder stringBuilder = new StringBuilder(tihabTemplate.displayName).AppendLine();
				if (flag)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					string text = "UI.Habs.ConfirmApplication";
					object[] array = new object[1];
					array[0] = TIUtilities.ConstructTextList(list.ConvertAll<TIDataTemplate>((TIHabModuleTemplate x) => x), false, false);
					stringBuilder2.AppendLine(Loc.T(text, array));
					if (list2.Count > 0)
					{
						StringBuilder stringBuilder3 = stringBuilder;
						string text2 = "UI.Habs.FailedBuild";
						object[] array2 = new object[1];
						array2[0] = TIUtilities.ConstructTextList(list2.ConvertAll<TIDataTemplate>((TIHabModuleTemplate x) => x), false, false);
						stringBuilder3.AppendLine(Loc.T(text2, array2));
					}
				}
				else
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.CantAffordTemplate"));
				}
				stringBuilder.AppendLine(Loc.T("UI.Habs.BaseCost", new object[] { tiresourcesCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				if (tiresourcesCost2.anyDebit)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.OurCost", new object[] { tiresourcesCost2.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				}
				if (this.habToDisplay.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.semiMajorAxis_AU > 1.0199999809265137)
				{
					if (tihabTemplate.AllModuleTemplates(true).Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.IsSolarPower))
					{
						stringBuilder.AppendLine(Loc.T("UI.Habs.SolarPowerWarning"));
					}
				}
				if (num < 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.TemplatePowerProblems", new object[]
					{
						(-num).ToString("N0"),
						TemplateManager.global.habPowerInlineSpritePath
					}));
				}
				this.managementQueryText.SetText(stringBuilder.ToString());
				this.managementQueryConfirmButton.interactable = flag && list.Count > 0 && this.habToDisplay.CanApplySavedTemplate(tihabTemplate);
				return;
			}
			this.managementQueryTemplateDropdown.captionText.SetText(Loc.T("UI.Habs.SelectTemplate"));
			this.managementQueryConfirmButton.interactable = false;
		}

		// Token: 0x060050A2 RID: 20642 RVA: 0x00232AE5 File Offset: 0x00230CE5
		public void OnHabManagementQueryToggleChanged()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (this.applyingMassTemplates)
			{
				this.SelectHabForMassTemplateInfoDisplay(this.managementQuerySelectedHabDropdown.value);
				return;
			}
			this.OnHabTemplateSelected();
		}

		// Token: 0x060050A3 RID: 20643 RVA: 0x00232B14 File Offset: 0x00230D14
		public bool PopulateHabTemplateDropdown()
		{
			if (this.habToDisplay == null)
			{
				return false;
			}
			bool flag = false;
			this.managementQueryTemplateDropdown.ClearOptions();
			this.habTemplateDropdown.Clear();
			this.managementQueryTemplateDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.SelectTemplate")
			});
			this.habTemplateDropdown.Add(0, null);
			this.managementQueryTemplateDropdown.captionText.SetText(Loc.T("UI.Habs.SelectTemplate"));
			int num = 1;
			foreach (TIHabTemplate tihabTemplate in base.activePlayer.habDesigns)
			{
				if (this.habToDisplay.CanApplySavedTemplate(tihabTemplate))
				{
					flag = true;
					this.habTemplateDropdown.Add(num, tihabTemplate.dataName);
					this.managementQueryTemplateDropdown.options.Add(new TMP_Dropdown.OptionData
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

		// Token: 0x060050A4 RID: 20644 RVA: 0x00232C74 File Offset: 0x00230E74
		public void SetSelectedTemplateInDropdown(TIHabTemplate toSelect)
		{
			int key = this.habTemplateDropdown.FirstOrDefault<KeyValuePair<int, string>>((KeyValuePair<int, string> pair) => pair.Value != null && pair.Value.Equals(toSelect.dataName)).Key;
			this.managementQueryTemplateDropdown.value = key;
		}

		// Token: 0x060050A5 RID: 20645 RVA: 0x00232CBC File Offset: 0x00230EBC
		public bool PopulateSelectedHabDropdown()
		{
			bool flag = false;
			this.managementQuerySelectedHabDropdown.ClearOptions();
			this.habSelectionDropdown.Clear();
			this.managementQuerySelectedHabDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.SelectAHab")
			});
			this.habSelectionDropdown.Add(0, null);
			this.managementQuerySelectedHabDropdown.captionText.SetText(Loc.T("UI.Habs.SelectAHab"));
			int num = 1;
			foreach (TIHabState tihabState in this.selectedHabList)
			{
				TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(this.massHabTemplateDropdown[this.massTemplatesDropdown.value], false);
				if (tihabTemplate != null)
				{
					TIResourcesCost tiresourcesCost;
					float num2;
					List<TIHabModuleTemplate> list;
					tihabState.ApplySavedTemplate(tihabTemplate, true, this.managementQueryToggle.isOn, out tiresourcesCost, out num2, out list);
					flag = true;
					this.habSelectionDropdown.Add(num, TIUtilities.CombineStrings(new string[]
					{
						tihabState.displayName,
						TIGlobalConfig.globalConfig.victoryItemInlineSpritePath
					}));
					StringBuilder stringBuilder = new StringBuilder();
					if (list.Count > 0)
					{
						stringBuilder.Append(TemplateManager.global.warningInlineSpritePath).Append(" ");
					}
					stringBuilder.Append(TIUtilities.CombineStrings(new string[]
					{
						tihabState.displayName,
						TIGlobalConfig.globalConfig.victoryItemInlineSpritePath
					}));
					this.managementQuerySelectedHabDropdown.options.Add(new TMP_Dropdown.OptionData
					{
						text = stringBuilder.ToString(),
						image = tihabState.icon
					});
					num++;
				}
			}
			return flag;
		}

		// Token: 0x060050A6 RID: 20646 RVA: 0x00232E78 File Offset: 0x00231078
		public bool PopulateMassHabTemplateDropdown()
		{
			bool flag = false;
			this.massTemplatesDropdown.ClearOptions();
			this.massHabTemplateDropdown.Clear();
			this.massTemplatesDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.SelectTemplate")
			});
			this.massHabTemplateDropdown.Add(0, null);
			this.massTemplatesDropdown.captionText.SetText(Loc.T("UI.Habs.SelectTemplate"));
			int num = 1;
			foreach (TIHabTemplate tihabTemplate in base.activePlayer.habDesigns)
			{
				flag = true;
				this.massHabTemplateDropdown.Add(num, tihabTemplate.dataName);
				this.massTemplatesDropdown.options.Add(new TMP_Dropdown.OptionData
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
			return flag;
		}

		// Token: 0x060050A7 RID: 20647 RVA: 0x00232FB8 File Offset: 0x002311B8
		public void OnSelectMassHabTemplateDropdown(int selected)
		{
			this.DeselectAllHabsForMassTemplate();
			if (selected == 0)
			{
				this.OnClickMassHabTemplateCancel();
				return;
			}
			this.applyingMassTemplates = true;
			this.listHeaderText.SetText(Loc.T("UI.Habs.MassTemplateSelectHabs"));
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.massTemplatesHabIconSelectionDropdown.gameObject.SetActive(true);
			this.factionsDropdown.interactable = false;
			this.factionsDropdown.value = this.factionDropdownLookup.FirstOrDefault<KeyValuePair<int, TIFactionState>>((KeyValuePair<int, TIFactionState> pair) => pair.Value != null && pair.Value.isActivePlayer).Key;
			this.habList_FilterForTemplate = TemplateManager.Find<TIHabTemplate>(this.massHabTemplateDropdown[this.massTemplatesDropdown.value], false);
			if (this.habList_FilterForTemplate.habType == HabType.Station)
			{
				if (!this.stationsToggle.isOn)
				{
					this.stationsToggle.SetIsOnWithoutNotify(true);
					this.OnStationsToggleClicked(false);
				}
				if (this.basesToggle.isOn)
				{
					this.basesToggle.SetIsOnWithoutNotify(false);
					this.OnBasesToggleClicked(false);
				}
			}
			else if (this.habList_FilterForTemplate.habType == HabType.Base)
			{
				if (!this.basesToggle.isOn)
				{
					this.basesToggle.SetIsOnWithoutNotify(true);
					this.OnBasesToggleClicked(false);
				}
				if (this.stationsToggle.isOn)
				{
					this.stationsToggle.SetIsOnWithoutNotify(false);
					this.OnStationsToggleClicked(false);
				}
			}
			this.basesToggle.interactable = false;
			this.stationsToggle.interactable = false;
			this.managementQueryText.SetText(Loc.T("UI.Habs.MassSelectHabTemplateQuery", new object[]
			{
				base.activePlayer.displayNameWithColor,
				TemplateManager.global.habPowerInlineSpritePath
			}));
			this.managementQueryTemplateDropdownObject.SetActive(false);
			this.managementQueryToggleObject.SetActive(true);
			this.managementQuerySelectedHabDropdownObject.SetActive(true);
			this.managementQueryConfirmButtonObject.SetActive(true);
			this.managementQueryConfirmButtonText.SetText(Loc.T("UI.Habs.ApplySelectedTemplateToAllSelectedHabs"));
			this.managementQueryConfirmButton.onClick.RemoveAllListeners();
			this.managementQueryConfirmButton.onClick.AddListener(new UnityAction(this.OnConfirmApplyMassHabTemplateSelected));
			this.managementQueryObject.SetActive(true);
			this.MassHabTemplateUpdateManagementQuery();
			this.SetManageButtonStatusAndText();
			this.UpdateHabLists();
		}

		// Token: 0x060050A8 RID: 20648 RVA: 0x002331F4 File Offset: 0x002313F4
		public void SelectHabForMassTemplateInfoDisplay(int selected)
		{
			if (selected == 0)
			{
				this.MassHabTemplateUpdateManagementQuery();
				return;
			}
			string text = this.massHabTemplateDropdown[this.massTemplatesDropdown.value];
			if (!string.IsNullOrEmpty(text))
			{
				TIHabState tihabState = this.selectedHabList[selected - 1];
				TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(text, false);
				TIResourcesCost tiresourcesCost;
				float num;
				List<TIHabModuleTemplate> list2;
				List<TIHabModuleTemplate> list = tihabState.ApplySavedTemplate(tihabTemplate, true, this.managementQueryToggle.isOn, out tiresourcesCost, out num, out list2);
				TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
				bool flag = true;
				if (!tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
				{
					tiresourcesCost2 = this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost, null);
					flag = tiresourcesCost2.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				}
				StringBuilder stringBuilder = new StringBuilder(tihabTemplate.displayName).AppendLine();
				if (flag)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					string text2 = "UI.Habs.ConfirmApplication";
					object[] array = new object[1];
					array[0] = TIUtilities.ConstructTextList(list.ConvertAll<TIDataTemplate>((TIHabModuleTemplate x) => x), false, false);
					stringBuilder2.AppendLine(Loc.T(text2, array));
					if (list2.Count > 0)
					{
						StringBuilder stringBuilder3 = stringBuilder;
						string text3 = "UI.Habs.FailedBuild";
						object[] array2 = new object[1];
						array2[0] = TIUtilities.ConstructTextList(list2.ConvertAll<TIDataTemplate>((TIHabModuleTemplate x) => x), false, false);
						stringBuilder3.AppendLine(Loc.T(text3, array2));
					}
				}
				else
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.CantAffordTemplate"));
				}
				stringBuilder.AppendLine(Loc.T("UI.Habs.BaseCost", new object[] { tiresourcesCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				if (tiresourcesCost2.anyDebit)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.OurCost", new object[] { tiresourcesCost2.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				}
				if (tihabState.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.semiMajorAxis_AU > 1.0199999809265137)
				{
					if (tihabTemplate.AllModuleTemplates(true).Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.IsSolarPower))
					{
						stringBuilder.AppendLine(Loc.T("UI.Habs.SolarPowerWarning"));
					}
				}
				if (num < 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.TemplatePowerProblems", new object[]
					{
						(-num).ToString("N0"),
						TemplateManager.global.habPowerInlineSpritePath
					}));
				}
				this.managementQueryText.SetText(stringBuilder.ToString());
				this.managementQueryConfirmButton.interactable = flag && list.Count > 0 && tihabState.CanApplySavedTemplate(tihabTemplate);
				return;
			}
			this.managementQueryTemplateDropdown.captionText.SetText(Loc.T("UI.Habs.SelectTemplate"));
			this.managementQueryConfirmButton.interactable = false;
		}

		// Token: 0x060050A9 RID: 20649 RVA: 0x002334D8 File Offset: 0x002316D8
		public void MassHabTemplateUpdateManagementQuery()
		{
			this.PopulateSelectedHabDropdown();
			if (this.managementQuerySelectedHabDropdown.value != 0)
			{
				this.SelectHabForMassTemplateInfoDisplay(this.managementQuerySelectedHabDropdown.value);
				return;
			}
			string text = this.massHabTemplateDropdown[this.massTemplatesDropdown.value];
			if (!string.IsNullOrEmpty(text))
			{
				TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(text, false);
				TIResourcesCost tiresourcesCost = new TIResourcesCost();
				TIResourcesCost tiresourcesCost2 = new TIResourcesCost();
				bool flag = true;
				bool flag2 = false;
				List<TIHabModuleTemplate> list = new List<TIHabModuleTemplate>();
				List<TIHabModuleTemplate> list2 = new List<TIHabModuleTemplate>();
				bool flag3 = false;
				bool flag4 = true;
				foreach (TIHabState tihabState in this.selectedHabList)
				{
					TIResourcesCost tiresourcesCost3;
					float num;
					List<TIHabModuleTemplate> list4;
					List<TIHabModuleTemplate> list3 = tihabState.ApplySavedTemplate(tihabTemplate, true, this.managementQueryToggle.isOn, out tiresourcesCost3, out num, out list4);
					TIResourcesCost tiresourcesCost4 = new TIResourcesCost();
					bool flag5 = true;
					if (!tiresourcesCost3.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
					{
						tiresourcesCost4 = this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost3, tihabState);
						flag5 = tiresourcesCost4.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
					}
					tiresourcesCost += tiresourcesCost3;
					tiresourcesCost2 += tiresourcesCost4;
					flag = flag && flag5;
					flag2 |= num < 0f;
					list.AddRange(list3);
					list2.AddRange(list4);
					bool flag6 = flag3;
					bool flag7;
					if (tihabState.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.semiMajorAxis_AU > 1.0199999809265137)
					{
						flag7 = tihabTemplate.AllModuleTemplates(true).Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.IsSolarPower);
					}
					else
					{
						flag7 = false;
					}
					flag3 = flag6 || flag7;
					flag4 &= tihabState.CanApplySavedTemplate(tihabTemplate);
				}
				StringBuilder stringBuilder = new StringBuilder(tihabTemplate.displayName).AppendLine().Append(Loc.T("UI.Habs.MassApplyTemplateInstructions")).AppendLine();
				stringBuilder.AppendLine(Loc.T("UI.Habs.BaseCost", new object[] { tiresourcesCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				if (tiresourcesCost2.anyDebit)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.OurCost", new object[] { tiresourcesCost2.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				}
				if (flag3)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.SolarPowerWarning"));
				}
				if (flag2)
				{
					stringBuilder.AppendLine(Loc.T("UI.Habs.MassTemplatePowerProblems", new object[] { TemplateManager.global.habPowerInlineSpritePath }));
				}
				this.managementQueryText.SetText(stringBuilder.ToString());
				this.managementQueryConfirmButton.interactable = flag && list.Count > 0 && flag4;
				return;
			}
			this.managementQueryTemplateDropdown.captionText.SetText(Loc.T("UI.Habs.SelectTemplate"));
			this.managementQueryConfirmButton.interactable = false;
		}

		// Token: 0x060050AA RID: 20650 RVA: 0x002337D4 File Offset: 0x002319D4
		public void OnClickMassHabTemplateCancel()
		{
			this.applyingMassTemplates = false;
			this.listHeaderText.SetText(Loc.T("UI.Habs.SelectAHab"));
			this.massTemplatesHabIconSelectionDropdown.gameObject.SetActive(false);
			this.factionsDropdown.interactable = true;
			this.basesToggle.interactable = true;
			this.stationsToggle.interactable = true;
			this.DeselectAllHabsForMassTemplate();
			this.massTemplatesDropdown.SetValueWithoutNotify(0);
			this.habList_FilterForTemplate = null;
			this.managementQueryObject.SetActive(false);
			this.UpdateHabLists();
			this.SetManageButtonStatusAndText();
		}

		// Token: 0x060050AB RID: 20651 RVA: 0x00233864 File Offset: 0x00231A64
		public void SetSelectedStatus(TIHabState habState, bool value, bool delayUpdate = false)
		{
			if (value && !this.selectedHabList.Contains(habState))
			{
				this.selectedHabList.Add(habState);
			}
			if (!value && this.selectedHabList.Contains(habState))
			{
				this.selectedHabList.Remove(habState);
			}
			if (!delayUpdate)
			{
				this.UpdateHabModelData();
			}
		}

		// Token: 0x060050AC RID: 20652 RVA: 0x002338B5 File Offset: 0x00231AB5
		private void DeselectAllHabsForMassTemplate()
		{
			this.UnHighlightAllHabs();
		}

		// Token: 0x060050AD RID: 20653 RVA: 0x002338C0 File Offset: 0x00231AC0
		public void OnMassTemplateHabIconSelectionDropdownChanged(int selected)
		{
			List<TIHabState> list = new List<TIHabState>();
			foreach (HabScreenHabListItemModel habScreenHabListItemModel in this.habModels)
			{
				TIHabState habState = habScreenHabListItemModel.HabScreenHabListItemData.habState;
				if (habState.ref_faction.isActivePlayer && habState.customHabIconResource == this.habIconPaths[selected])
				{
					list.Add(habState);
				}
			}
			if (list.Count == 0)
			{
				this.massTemplatesHabIconSelectionDropdown.SetValueWithoutNotify(0);
				return;
			}
			if (list.All<TIHabState>((TIHabState x) => this.selectedHabList.Contains(x)))
			{
				int num = 0;
				using (List<TIHabState>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIHabState tihabState = enumerator2.Current;
						this.SetSelectedStatus(tihabState, false, num != list.Count - 1);
						num++;
					}
					goto IL_012B;
				}
			}
			int num2 = 0;
			foreach (TIHabState tihabState2 in list)
			{
				this.SetSelectedStatus(tihabState2, true, num2 != list.Count - 1);
				num2++;
			}
			IL_012B:
			this.massTemplatesHabIconSelectionDropdown.SetValueWithoutNotify(0);
			this.MassHabTemplateUpdateManagementQuery();
		}

		// Token: 0x060050AE RID: 20654 RVA: 0x00233A34 File Offset: 0x00231C34
		public bool PopulateQuickHabTemplateDropdown()
		{
			if (this.habToDisplay == null)
			{
				return false;
			}
			bool flag = false;
			this.quickTemplatesDropdown.ClearOptions();
			this.quickHabTemplateDropdown.Clear();
			this.quickTemplatesDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.SelectTemplate")
			});
			this.quickHabTemplateDropdown.Add(0, null);
			this.quickTemplatesDropdown.captionText.SetText(Loc.T("UI.Habs.SelectTemplate"));
			int num = 1;
			foreach (TIHabTemplate tihabTemplate in base.activePlayer.habDesigns)
			{
				if (this.habToDisplay.CanApplySavedTemplate(tihabTemplate))
				{
					flag = true;
					this.quickHabTemplateDropdown.Add(num, tihabTemplate.dataName);
					this.quickTemplatesDropdown.options.Add(new TMP_Dropdown.OptionData
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

		// Token: 0x060050AF RID: 20655 RVA: 0x00233B94 File Offset: 0x00231D94
		public void OnSelectQuickHabTemplateDropdown(int selected)
		{
			if (this.habToDisplay == null)
			{
				return;
			}
			if (selected == 0)
			{
				return;
			}
			TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(this.quickHabTemplateDropdown[this.quickTemplatesDropdown.value], false);
			if (tihabTemplate == null)
			{
				return;
			}
			this.OnCopyHabButtonPressed();
			this.SetSelectedTemplateInDropdown(tihabTemplate);
		}

		// Token: 0x060050B0 RID: 20656 RVA: 0x00233BE4 File Offset: 0x00231DE4
		public void ResetQuickHabTemplateDropdown()
		{
			bool flag = this.IsManaging() && this.habToDisplay != null;
			this.quickTemplatesDropdown.gameObject.SetActive(flag);
			if (flag)
			{
				bool flag2 = false;
				foreach (TIHabTemplate tihabTemplate in base.activePlayer.habDesigns)
				{
					if (this.habToDisplay.CanApplySavedTemplate(tihabTemplate))
					{
						flag2 = true;
						break;
					}
				}
				this.quickTemplatesDropdown.SetValueWithoutNotify(0);
				this.quickTemplatesDropdown.interactable = flag2;
			}
		}

		// Token: 0x060050B1 RID: 20657 RVA: 0x00233C90 File Offset: 0x00231E90
		private TIResourcesCost CostWithNecessaryBoost(TIFactionState faction, TIResourcesCost baseLineCost, TIHabState habState = null)
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
						num2 += TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, (habState == null) ? this.habToDisplay : habState, resourceValue.value / TemplateManager.global.spaceResourceToTons);
					}
				}
				tiresourcesCost.AddCost(FactionResource.Money, num, true);
				tiresourcesCost.AddCost(FactionResource.Boost, (float)num2, true);
			}
			return tiresourcesCost;
		}

		// Token: 0x060050B2 RID: 20658 RVA: 0x00233DC4 File Offset: 0x00231FC4
		public void OnConfirmApplyHabTemplateSelected()
		{
			TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(this.habTemplateDropdown[this.managementQueryTemplateDropdown.value], false);
			TIResourcesCost tiresourcesCost;
			float num;
			List<TIHabModuleTemplate> list;
			this.habToDisplay.ApplySavedTemplate(tihabTemplate, true, this.managementQueryToggle.isOn, out tiresourcesCost, out num, out list);
			if (this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost, null).CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
			{
				base.activePlayer.playerControl.StartAction(new ApplyHabTemplateAction(this.habToDisplay, tihabTemplate, this.managementQueryToggle.isOn));
				SoundEffectController.PlayBuildHabModuleSound(tihabTemplate.sectors[0].habModules[0], this.habToDisplay);
				this.managementQueryObject.SetActive(false);
				this.managementQueryTemplateDropdownObject.SetActive(false);
				this.managementQuerySelectedHabDropdownObject.SetActive(false);
				this.managementQueryToggleObject.SetActive(false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			this.ResetQuickHabTemplateDropdown();
			this.SetManageButtonStatusAndText();
		}

		// Token: 0x060050B3 RID: 20659 RVA: 0x00233EC4 File Offset: 0x002320C4
		public void OnConfirmApplyMassHabTemplateSelected()
		{
			TIHabTemplate tihabTemplate = TemplateManager.Find<TIHabTemplate>(this.massHabTemplateDropdown[this.massTemplatesDropdown.value], false);
			int num = 0;
			foreach (TIHabState tihabState in this.selectedHabList)
			{
				TIResourcesCost tiresourcesCost;
				float num2;
				List<TIHabModuleTemplate> list;
				tihabState.ApplySavedTemplate(tihabTemplate, true, this.managementQueryToggle.isOn, out tiresourcesCost, out num2, out list);
				if (this.CostWithNecessaryBoost(base.activePlayer, tiresourcesCost, tihabState).CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
				{
					base.activePlayer.playerControl.StartAction(new ApplyHabTemplateAction(tihabState, tihabTemplate, this.managementQueryToggle.isOn));
					if (num == 0)
					{
						SoundEffectController.PlayBuildHabModuleSound(tihabTemplate.sectors[0].habModules[0], tihabState);
					}
					this.managementQueryObject.SetActive(false);
					this.managementQueryTemplateDropdownObject.SetActive(false);
					this.managementQuerySelectedHabDropdownObject.SetActive(false);
					this.managementQueryToggleObject.SetActive(false);
				}
				else if (num == 0)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				num++;
			}
			this.OnClickMassHabTemplateCancel();
		}

		// Token: 0x060050B4 RID: 20660 RVA: 0x00234000 File Offset: 0x00232200
		public void OnOpenHabTemplateManager()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			if (!this.manageHabTemplatesPanel.activeSelf)
			{
				this.manageHabTemplatesPanel.SetActive(true);
				this.RefreshHabTemplateManagerList();
				this.availableModuleListObject.SetActive(false);
			}
			else
			{
				this.CloseHabTemplateManager();
			}
			this.manageHabTemplatesButton.interactable = base.activePlayer.habDesigns.Count > 0 && !this.manageHabTemplatesPanel.activeSelf;
		}

		// Token: 0x060050B5 RID: 20661 RVA: 0x0023407C File Offset: 0x0023227C
		public void RefreshHabTemplateManagerList()
		{
			this.manageHabTemplatesList.SetListSize<HabDesignListItemController>(base.activePlayer.habDesigns.Count, false, false);
			List<TIHabTemplate> list = base.activePlayer.habDesigns.OrderByDescending<TIHabTemplate, bool>(delegate(TIHabTemplate x)
			{
				TIHabState tihabState = this.habToDisplay;
				if (tihabState != null && !tihabState.IsStation)
				{
					return x.habType == HabType.Base;
				}
				return x.habType == HabType.Station;
			}).ThenByDescending<TIHabTemplate, int>((TIHabTemplate x) => x.tier).ThenBy<TIHabTemplate, string>((TIHabTemplate x) => x.displayName)
				.ToList<TIHabTemplate>();
			int num = 0;
			using (IEnumerator<object> enumerator = this.manageHabTemplatesList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (HabitatsScreenController.<>o__419.<>p__0 == null)
					{
						HabitatsScreenController.<>o__419.<>p__0 = CallSite<Func<CallSite, object, HabDesignListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(HabDesignListItemController), typeof(HabitatsScreenController)));
					}
					HabitatsScreenController.<>o__419.<>p__0.Target(HabitatsScreenController.<>o__419.<>p__0, enumerator.Current).SetListItem(list[num++], this, num);
				}
			}
		}

		// Token: 0x060050B6 RID: 20662 RVA: 0x0023419C File Offset: 0x0023239C
		public void OnClickCloseHabTemplateManager()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseHabTemplateManager();
			this.managementQueryObject.SetActive(false);
		}

		// Token: 0x060050B7 RID: 20663 RVA: 0x002341BC File Offset: 0x002323BC
		public void CloseHabTemplateManager()
		{
			this.manageHabTemplatesPanel.SetActive(false);
			this.SetShowCopySaveHabTemplateButtons();
			if (this.managementQueryTemplateDropdown.isActiveAndEnabled)
			{
				this.PopulateHabTemplateDropdown();
			}
			this.availableModuleListObject.SetActive(true);
		}

		// Token: 0x060050B8 RID: 20664 RVA: 0x002341F0 File Offset: 0x002323F0
		public bool ModuleFilterConditionMet(TIHabModuleTemplate module, HabitatsScreenController.AvailableModuleFilters moduleFilter)
		{
			switch (moduleFilter)
			{
			default:
				return true;
			case HabitatsScreenController.AvailableModuleFilters.Core:
				return module.coreModule;
			case HabitatsScreenController.AvailableModuleFilters.PowerSupplier:
				return module.powerSource || module.SpecialRules.Contains(HabModuleSpecialRule.SolarMirror);
			case HabitatsScreenController.AvailableModuleFilters.ShipConstruction:
				return module.allowsShipConstruction;
			case HabitatsScreenController.AvailableModuleFilters.Resupply:
				return module.allowsResupply;
			case HabitatsScreenController.AvailableModuleFilters.ModuleConstruction:
				return module.constructionModule;
			case HabitatsScreenController.AvailableModuleFilters.Mine:
				return module.mine;
			case HabitatsScreenController.AvailableModuleFilters.Farm:
				return module.SpecialRules.Contains(HabModuleSpecialRule.Farm);
			case HabitatsScreenController.AvailableModuleFilters.SpaceCombat:
				return module.spaceCombatModule;
			case HabitatsScreenController.AvailableModuleFilters.AssaultCombat:
				return module.CombatTroops();
			case HabitatsScreenController.AvailableModuleFilters.Income_Money:
				return module.incomeMoney_month > 0f;
			case HabitatsScreenController.AvailableModuleFilters.Income_Influence:
				return module.incomeInfluence_month > 0f;
			case HabitatsScreenController.AvailableModuleFilters.Income_Research:
				return module.incomeResearch_month > 0f;
			case HabitatsScreenController.AvailableModuleFilters.Income_Projects:
				return module.incomeProjects > 0;
			case HabitatsScreenController.AvailableModuleFilters.Income_MissionControl:
				return module.missionControl > 0;
			case HabitatsScreenController.AvailableModuleFilters.Income_Antimatter:
				return module.incomeAntimatter_month > 0f || module.SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter);
			case HabitatsScreenController.AvailableModuleFilters.ControlPointCapacity:
				return module.ControlPointCapacity(this.habToDisplay.inEarthLEO) > 0;
			case HabitatsScreenController.AvailableModuleFilters.TechBonuses:
				return module.techBonuses.Any<TechBonus>((TechBonus x) => x.bonus > 0f);
			case HabitatsScreenController.AvailableModuleFilters.LEOBonuses:
				return module.HasLEOBonus();
			}
		}

		// Token: 0x060050B9 RID: 20665 RVA: 0x0023434C File Offset: 0x0023254C
		public void UpdateFilterButtons()
		{
			List<TIHabModuleTemplate> list = this.habToDisplay.AllowedModules(base.activePlayer);
			int k;
			int i;
			for (k = 1; k < 4; k = i + 1)
			{
				this.tierButtons[k].gameObject.SetActive(this.habToDisplay.maxTier >= k);
				this.tierButtons[k].interactable = list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.tier == k);
				i = k;
			}
			using (IEnumerator enumerator = Enum.GetValues(typeof(HabitatsScreenController.AvailableModuleFilters)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HabitatsScreenController.AvailableModuleFilters amfilter = (HabitatsScreenController.AvailableModuleFilters)enumerator.Current;
					this.benefitButtons[(int)amfilter].interactable = list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => this.ModuleFilterConditionMet(x, amfilter));
				}
			}
			this.moduleListAntimatterSortButtonObject.SetActive(GameControl.control.activePlayer.ref_faction.UnlockedAntimatter && GameControl.control.activePlayer.ref_faction.GetDailyIncome(FactionResource.Antimatter, false, false) > 0f);
			this.SetBenefitFilter((int)this.benefitFilter);
		}

		// Token: 0x060050BA RID: 20666 RVA: 0x002344C0 File Offset: 0x002326C0
		public void SetTierFilter(int tier)
		{
			this.tierButtons[this.tierFilter].image.sprite = this.cachedButtonSprite;
			this.tierFilter = tier;
			this.tierButtons[this.tierFilter].image.sprite = this.tierButtons[this.tierFilter].spriteState.selectedSprite;
			this.UpdateModuleList(this.habToDisplay.habType);
		}

		// Token: 0x060050BB RID: 20667 RVA: 0x00234533 File Offset: 0x00232733
		public void OnFilterModuleTier(int tier)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, false);
			this.SetTierFilter(tier);
		}

		// Token: 0x060050BC RID: 20668 RVA: 0x00234548 File Offset: 0x00232748
		public void SetBenefitFilter(int filter)
		{
			this.benefitButtons[(int)this.benefitFilter].image.sprite = this.cachedButtonSprite;
			this.benefitFilter = (HabitatsScreenController.AvailableModuleFilters)filter;
			this.benefitButtons[(int)this.benefitFilter].image.sprite = this.benefitButtons[(int)this.benefitFilter].spriteState.selectedSprite;
			this.UpdateModuleList(this.habToDisplay.habType);
			for (int i = 0; i < this.benefitButtons.Length; i++)
			{
				RectTransform rectTransform = (RectTransform)this.benefitButtons[i].transform;
				if (rectTransform)
				{
					rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (i != filter) ? 46f : 49f);
				}
			}
		}

		// Token: 0x060050BD RID: 20669 RVA: 0x0023460F File Offset: 0x0023280F
		public void OnFilterModuleBenefit(int filter)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, false);
			this.SetBenefitFilter(filter);
		}

		// Token: 0x060050BE RID: 20670 RVA: 0x00234624 File Offset: 0x00232824
		private void UpdateModuleList(HabType habType)
		{
			if (this.showAvailableModules)
			{
				this.UpdateAvailableModuleList(habType);
				return;
			}
			this.UpdateInstalledModuleList(habType);
		}

		// Token: 0x060050BF RID: 20671 RVA: 0x00234640 File Offset: 0x00232840
		private void UpdateAvailableModuleList(HabType habType)
		{
			if (this.habModuleTemplates == null)
			{
				this.habModuleTemplates = TemplateManager.GetAllTemplates<TIHabModuleTemplate>(true);
				this.habModuleDictionary = new Dictionary<string, TIHabModuleTemplate>();
				for (int i = 0; i < this.habModuleTemplates.Length; i++)
				{
					this.habModuleDictionary.Add(this.habModuleTemplates[i].dataName, this.habModuleTemplates[i]);
				}
			}
			List<TIHabModuleTemplate> list = this.habToDisplay.AllowedModules(base.activePlayer);
			int num = this.tierFilter;
			if (num - 1 <= 3)
			{
				list = list.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.tier == this.tierFilter).ToList<TIHabModuleTemplate>();
			}
			list = list.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => this.ModuleFilterConditionMet(x, this.benefitFilter)).ToList<TIHabModuleTemplate>();
			list = list.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.displayName.ToLowerInvariant().Contains(this.modules_nameFilterForModules.ToLowerInvariant())).ToList<TIHabModuleTemplate>();
			list = (from x in list
				orderby x.tier descending, x.displayName descending
				select x).ToList<TIHabModuleTemplate>();
			for (int j = 0; j < list.Count; j++)
			{
				TIHabModuleTemplate moduleTemplate = list[j];
				if (moduleTemplate.habType == habType || moduleTemplate.habType == HabType.Any)
				{
					HabModuleListItem component;
					if (!this.availableModuleDictionary.TryGetValue(moduleTemplate, out component))
					{
						GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.moduleListItemPrefab);
						Loc.SwapFonts(gameObject);
						gameObject.SetActive(true);
						component = gameObject.GetComponent<HabModuleListItem>();
						component.controller = this;
						component.transform.localPosition = Vector3.zero;
						component.transform.localScale = Vector3.one;
						this.availableModuleDictionary.Add(moduleTemplate, component);
						this.availableModuleListItems.Add(component);
					}
					bool flag = false;
					bool flag2 = this.habToDisplay.ModuleUpgradePrereqModuleAlreadyOnHab(moduleTemplate);
					if (!moduleTemplate.onePerHab || !flag2)
					{
						flag = moduleTemplate.CostFromEarth(base.activePlayer, this.habToDisplay, false).CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
						TIResourcesCost tiresourcesCost = moduleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, false, true, 0, false);
						flag |= tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
					}
					if (flag2)
					{
						TIResourcesCost tiresourcesCost2 = moduleTemplate.CostFromEarth(base.activePlayer, this.habToDisplay, true);
						flag |= tiresourcesCost2.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
						List<TIHabModuleState> list2 = this.habToDisplay.CompletedModules();
						if (((list2 != null) ? list2.Where<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate == moduleTemplate.UpgradesFrom).FirstOrDefault<TIHabModuleState>() : null) == null)
						{
							(from x in this.habToDisplay.AllModules()
								where x.underConstruction
								select x).MinBy<TIHabModuleState, DateTime>((TIHabModuleState x) => x.completionDate);
						}
						TIResourcesCost tiresourcesCost3 = moduleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, true, true, 0, false);
						flag |= tiresourcesCost3.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
					}
					TIHabModuleTemplate upgradesTo = moduleTemplate.UpgradesTo;
					if (component == null)
					{
						return;
					}
					component.draggable = flag;
					component.prospective = true;
					component.AssignTooltipDelegate();
					component.SetModuleTemplate(moduleTemplate, habType, flag2, null);
					component.Previewer = this;
				}
			}
			for (int k = 0; k < this.installedModuleListItems.Count; k++)
			{
				this.installedModuleListItems[k].gameObject.SetActive(false);
			}
			this.availableModuleListItems = (from x in this.availableModuleListItems
				orderby x.GetModuleTemplate().coreModule descending, x.GetModuleTemplate().tier descending, x.GetModuleTemplate().displayName
				select x).ToList<HabModuleListItem>();
			for (int l = 0; l < this.availableModuleListItems.Count; l++)
			{
				this.availableModuleListItems[l].controller = this;
				this.availableModuleListItems[l].transform.SetParent(this.availableModuleListObject.transform, false);
				this.availableModuleListItems[l].transform.SetSiblingIndex(l);
				if (!list.Contains(this.availableModuleListItems[l].GetModuleTemplate()))
				{
					this.availableModuleListItems[l].gameObject.SetActive(false);
				}
				else
				{
					this.availableModuleListItems[l].gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060050C0 RID: 20672 RVA: 0x00234B64 File Offset: 0x00232D64
		private void UpdateInstalledModuleList(HabType habType)
		{
			if (this.habModuleTemplates == null)
			{
				this.habModuleTemplates = TemplateManager.GetAllTemplates<TIHabModuleTemplate>(true);
				this.habModuleDictionary = new Dictionary<string, TIHabModuleTemplate>();
				for (int i = 0; i < this.habModuleTemplates.Length; i++)
				{
					this.habModuleDictionary.Add(this.habModuleTemplates[i].dataName, this.habModuleTemplates[i]);
				}
			}
			List<TIHabModuleState> list = new List<TIHabModuleState>();
			Dictionary<TIHabModuleState, HabGridCell> dictionary = new Dictionary<TIHabModuleState, HabGridCell>();
			if (this.habToDisplay.IsBase)
			{
				for (int j = 0; j < this.baseGridCells.Length; j++)
				{
					if (this.baseGridCells[j].habModule != null && this.baseGridCells[j].habModule.moduleTemplate != null)
					{
						list.Add(this.baseGridCells[j].habModule);
						dictionary[this.baseGridCells[j].habModule] = this.baseGridCells[j];
					}
				}
			}
			else if (this.habToDisplay.IsStation)
			{
				for (int k = 0; k < this.stationGridCells.Length; k++)
				{
					if (this.stationGridCells[k].habModule != null && this.stationGridCells[k].habModule.moduleTemplate != null)
					{
						list.Add(this.stationGridCells[k].habModule);
						dictionary[this.stationGridCells[k].habModule] = this.stationGridCells[k];
					}
				}
			}
			int num = this.tierFilter;
			if (num - 1 <= 3)
			{
				list = list.Where<TIHabModuleState>((TIHabModuleState x) => x.tier == this.tierFilter).ToList<TIHabModuleState>();
			}
			list = list.Where<TIHabModuleState>((TIHabModuleState x) => this.ModuleFilterConditionMet(x.moduleTemplate, this.benefitFilter)).ToList<TIHabModuleState>();
			list = list.Where<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.displayName.ToLowerInvariant().Contains(this.modules_nameFilterForModules.ToLowerInvariant())).ToList<TIHabModuleState>();
			list = (from x in list
				orderby x.tier descending, x.displayName descending
				select x).ToList<TIHabModuleState>();
			for (int l = 0; l < list.Count; l++)
			{
				TIHabModuleState tihabModuleState = list[l];
				TIHabModuleTemplate moduleTemplate = tihabModuleState.moduleTemplate;
				if (moduleTemplate.habType == habType || moduleTemplate.habType == HabType.Any)
				{
					HabModuleListItem component;
					if (!this.installedModuleDictionary.TryGetValue(tihabModuleState, out component))
					{
						GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.moduleListItemPrefab);
						Loc.SwapFonts(gameObject);
						gameObject.SetActive(true);
						component = gameObject.GetComponent<HabModuleListItem>();
						component.controller = this;
						component.transform.localPosition = Vector3.zero;
						component.transform.localScale = Vector3.one;
						this.installedModuleDictionary.Add(tihabModuleState, component);
						this.installedModuleListItems.Add(component);
					}
					if (component == null)
					{
						return;
					}
					component.draggable = false;
					component.prospective = false;
					component.AssignTooltipDelegate();
					HabGridCell habGridCell = null;
					if (l < list.Count)
					{
						habGridCell = dictionary[list[l]];
					}
					component.SetModule(tihabModuleState, habType, tihabModuleState.CanUpgrade(this.habToDisplay.faction), habGridCell);
					component.Previewer = this;
				}
			}
			for (int m = 0; m < this.availableModuleListItems.Count; m++)
			{
				this.availableModuleListItems[m].gameObject.SetActive(false);
			}
			this.installedModuleListItems = (from x in this.installedModuleListItems
				orderby x.GetModuleTemplate().coreModule descending, x.GetModuleTemplate().tier descending, x.GetModuleTemplate().displayName
				select x).ToList<HabModuleListItem>();
			for (int n = 0; n < this.installedModuleListItems.Count; n++)
			{
				this.installedModuleListItems[n].controller = this;
				this.installedModuleListItems[n].transform.SetParent(this.availableModuleListObject.transform, false);
				this.installedModuleListItems[n].transform.SetSiblingIndex(n);
				if (!list.Contains(this.installedModuleListItems[n].GetModuleState()))
				{
					this.installedModuleListItems[n].gameObject.SetActive(false);
				}
				else
				{
					this.installedModuleListItems[n].gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060050C1 RID: 20673 RVA: 0x00234FFC File Offset: 0x002331FC
		private void OnModuleUpgrade()
		{
			TIHabModuleTemplate tihabModuleTemplate;
			if (this.habModuleDictionary.TryGetValue(this.moduleUpgradeDataName, out tihabModuleTemplate))
			{
				this.StartModulePlacement(tihabModuleTemplate, this.displayModuleSector, this.displayModuleSlot);
			}
		}

		// Token: 0x060050C2 RID: 20674 RVA: 0x00235034 File Offset: 0x00233234
		public void OnModulePowerToggle()
		{
			TIHabModuleState habModule = this.selectedModule.habModule;
			if (habModule.destroyed)
			{
				if (habModule.priorModuleTemplate != null)
				{
					this.prospectiveModule = habModule.priorModuleTemplate;
					this.UpdateModulePreviewText(true, false);
					this.StartModulePlacement(this.prospectiveModule, this.displayModuleSector, this.displayModuleSlot);
					return;
				}
			}
			else
			{
				bool flag = !habModule.powered;
				if (flag && !habModule.CanPower())
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
					return;
				}
				if (!flag && !habModule.CanDepower())
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
					return;
				}
				bool powered = habModule.powered;
				base.activePlayer.playerControl.StartAction(new UpdateHabModulePowerStatus(habModule, !powered, new Action(this.UpdateModulePowerStatus)));
				if (powered == habModule.powered)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				else
				{
					AudioManager.PlayOneShot((!habModule.powered) ? "event:/SFX/UI_SFX/trig_SFX_DepowerModule" : "event:/SFX/UI_SFX/trig_SFX_PowerModule", false, false);
				}
				this.BuildHabSummary(this.habToDisplay);
				this.UpdateModulePreviewText(false, false);
			}
		}

		// Token: 0x060050C3 RID: 20675 RVA: 0x0023513C File Offset: 0x0023333C
		private void UpdateModulePowerStatus()
		{
			if (this.selectedModule == null || this.selectedModule.habModule == null)
			{
				this.powerPanel.SetActive(false);
				return;
			}
			TIHabModuleState habModule = this.selectedModule.habModule;
			GameObject gameObject = this.modulePowerToggle.gameObject;
			TIHabModuleTemplate moduleTemplate = habModule.moduleTemplate;
			gameObject.SetActive(moduleTemplate != null && moduleTemplate.CanTurnOff && !habModule.underConstruction && !habModule.decommissioning);
			if (habModule.underConstruction)
			{
				this.powerPanelTitle.SetText(Loc.T("UI.Habs.UnderConstruction2"));
				this.powerPanelValue.SetText(new StringBuilder(TemplateManager.global.habPowerInlineSpritePath).Append(0.ToString("N0")));
				this.selectedModule.SetPowerIcon(false);
				return;
			}
			if (habModule.destroyed && habModule.priorModuleTemplate != null)
			{
				this.powerPanelTitle.SetText(habModule.priorModuleTemplate.displayName);
				this.powerPanelValue.SetText(habModule.priorModuleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, false, true, 0, false).GetString("Relevant", false, false, false, 7, false, false, null, false, FactionResource.None));
				this.powerPanelOnOffButtonText.SetText(Loc.T("UI.Habs.Rebuild"));
				this.selectedModule.SetPowerIcon(false);
				this.powerButton.interactable = this.habToDisplay.AllowedModules(base.activePlayer).Contains(habModule.priorModuleTemplate);
				return;
			}
			if (habModule.moduleTemplate != null)
			{
				string text;
				if (habModule.PowerProvider())
				{
					text = (habModule.powered ? TIUtilities.GreenLine(Loc.T("UI.Habs.GeneratingPower")) : TIUtilities.RedLine(Loc.T("UI.Habs.GeneratingPower")));
				}
				else
				{
					text = (habModule.powered ? TIUtilities.GreenLine(Loc.T("UI.Habs.Powered")) : TIUtilities.RedLine(Loc.T("UI.Habs.UnPowered")));
				}
				this.powerPanelTitle.SetText(Loc.T(text));
				this.powerPanelOnOffButtonText.SetText(habModule.powered ? Loc.T("UI.Habs.TurnOff") : Loc.T("UI.Habs.TurnOn"));
				this.powerPanelValue.SetText(new StringBuilder(TemplateManager.global.habPowerInlineSpritePath).Append(habModule.ModulePower().ToString("N0")));
				this.selectedModule.SetPowerIcon(false);
				this.powerButton.interactable = (habModule.powered && habModule.CanDepower()) || (!habModule.powered && habModule.CanPower());
				this.UpdatePowerReport(this.habToDisplay);
				return;
			}
			this.powerPanel.SetActive(false);
			this.selectedModule.SetPowerIcon(true);
		}

		// Token: 0x060050C4 RID: 20676 RVA: 0x002353F4 File Offset: 0x002335F4
		private void SetModuleToPlace(TIHabModuleTemplate newModule)
		{
			this.oldModule = this.proposedModuleState.moduleTemplate;
			this.moduleToPlaceIsBuildOver = !this.proposedModuleState.empty && !this.proposedModuleState.destroyed && !this.proposedModuleState.decommissioning;
			this.proposedModuleTemplate = newModule;
			this.moduleToPlaceIsUpgrade = !string.IsNullOrEmpty(newModule.upgradesFromName) && this.oldModule != null && newModule.upgradesFromName == this.oldModule.dataName && this.proposedModuleState.constructionCompleted;
			this.spaceCost = this.GetSpaceCost(this.moduleToPlaceIsUpgrade, this.proposedModuleState);
			this.earthCost = this.GetEarthCost(this.moduleToPlaceIsUpgrade);
		}

		// Token: 0x060050C5 RID: 20677 RVA: 0x002354B8 File Offset: 0x002336B8
		public void StartModulePlacement(TIHabModuleTemplate newModule, int sector, int moduleSlot)
		{
			if (!TIGameState.Valid(this.habToDisplay))
			{
				return;
			}
			this.proposedModuleState = this.habToDisplay.sectors[sector].habModules[moduleSlot];
			this.SetModuleToPlace(newModule);
			this.sectorToPlace = sector;
			this.moduleSlotToPlace = moduleSlot;
			if (this.quickBuildToggle.isOn)
			{
				this.QuickBuildModule();
				return;
			}
			this.PopupModuleManagement();
		}

		// Token: 0x060050C6 RID: 20678 RVA: 0x00235524 File Offset: 0x00233724
		public void GetEmptyModuleSlot(out int sector, out int moduleSlot, bool mine)
		{
			sector = -1;
			moduleSlot = -1;
			for (int i = 0; i < this.habToDisplay.sectors.Count; i++)
			{
				for (int j = 0; j < this.habToDisplay.sectors[i].habModules.Count; j++)
				{
					TIHabModuleState tihabModuleState = this.habToDisplay.sectors[i].habModules[j];
					if ((tihabModuleState.empty || tihabModuleState.destroyed) && this.habToDisplay.sectors[i].active && mine == tihabModuleState.mineLocation)
					{
						sector = i;
						moduleSlot = j;
						return;
					}
				}
			}
			for (int k = 0; k < this.habToDisplay.sectors.Count; k++)
			{
				for (int l = 0; l < this.habToDisplay.sectors[k].habModules.Count; l++)
				{
					TIHabModuleState tihabModuleState2 = this.habToDisplay.sectors[k].habModules[l];
					if ((tihabModuleState2.empty || tihabModuleState2.destroyed) && this.habToDisplay.sectors[k].active && mine == tihabModuleState2.mineLocation)
					{
						sector = k;
						moduleSlot = l;
						return;
					}
				}
			}
		}

		// Token: 0x060050C7 RID: 20679 RVA: 0x00235678 File Offset: 0x00233878
		private void QuickBuildModule()
		{
			if (!this.IsLegalDrop())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			bool flag = this.earthCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
			bool flag2 = this.spaceCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
			if (!flag && !flag2)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			if (flag2 && (this.quickBuildWithBoostToggle.isOn || this.spaceCost.GetSingleCostValue(FactionResource.Boost) == 0f))
			{
				this.OnConfirmBuildModuleSpace();
				return;
			}
			if (this.quickBuildWithBoostToggle.isOn)
			{
				this.OnConfirmBuildModuleEarth();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x060050C8 RID: 20680 RVA: 0x00235730 File Offset: 0x00233930
		private void PopupModuleManagement()
		{
			this.OnCancelHabManagementButtonPressed(true);
			this.confirmModulePopupCanvas.enabled = true;
			if (!this.IsLegalDrop())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			bool flag = this.earthCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
			bool flag2 = this.spaceCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
			if (!flag && !flag2)
			{
				this.confirmModulePurchaseEarth.SetActive(false);
				this.confirmModulePurchaseSpace.SetActive(false);
				this.confirmModulePurchaseFailure.SetActive(true);
				this.confirmModuleQuery.SetText(Loc.T("UI.Habs.CantAfford"));
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			this.confirmModulePurchaseFailure.SetActive(false);
			this.cancelModulePurchase.SetActive(true);
			if (flag)
			{
				this.confirmModulePurchaseEarth.SetActive(true);
				if (this.moduleToPlaceIsUpgrade)
				{
					this.confirmModulePurchaseEarthCostText.SetText(Loc.T("UI.Habs.UpgradeFromEarthButtonText", new object[]
					{
						this.proposedModuleTemplate.UpgradesFrom.displayName,
						this.earthCost.GetString("Relevant", false, true, false, 2, false, false, null, false, FactionResource.None)
					}));
				}
				else
				{
					this.confirmModulePurchaseEarthCostText.SetText(Loc.T("UI.Habs.CostFromEarthButtonText", new object[] { this.earthCost.GetString("Relevant", false, true, false, 2, false, false, null, false, FactionResource.None) }));
				}
			}
			else
			{
				this.confirmModulePurchaseEarth.SetActive(false);
			}
			if (flag2)
			{
				this.confirmModulePurchaseSpace.SetActive(true);
				if (this.moduleToPlaceIsUpgrade)
				{
					this.confirmModulePurchaseSpaceCostText.SetText(Loc.T("UI.Habs.UpgradeFromSpaceButtonText", new object[]
					{
						this.proposedModuleTemplate.UpgradesFrom.displayName,
						this.spaceCost.GetString("Relevant", false, true, false, 2, false, false, null, false, FactionResource.None)
					}));
				}
				else
				{
					this.confirmModulePurchaseSpaceCostText.SetText(Loc.T("UI.Habs.CostFromSpaceButtonText", new object[] { this.spaceCost.GetString("Relevant", false, true, false, 2, false, false, null, false, FactionResource.None) }));
				}
			}
			else
			{
				this.confirmModulePurchaseSpace.SetActive(false);
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (flag && flag2)
			{
				stringBuilder.AppendLine(Loc.T("UI.Habs.BuildEither"));
			}
			else if (flag && !flag2)
			{
				stringBuilder.AppendLine(Loc.T("UI.Habs.BuildOnEarth"));
			}
			else if (!flag && flag2)
			{
				stringBuilder.AppendLine(Loc.T("UI.Habs.BuildInSpace"));
			}
			bool flag3 = false;
			stringBuilder.AppendLine();
			if (this.moduleToPlaceIsBuildOver && !this.moduleToPlaceIsUpgrade && !this.proposedModuleTemplate.coreModule)
			{
				stringBuilder.AppendLine(Loc.T("UI.Habs.NotUpgrade", new object[] { this.oldModule.displayName }));
				flag3 = true;
			}
			int num = this.proposedModuleTemplate.ProspectivePower(this.habToDisplay);
			if (this.moduleToPlaceIsBuildOver && this.oldModule != null)
			{
				num -= this.oldModule.ProspectivePower(this.habToDisplay);
			}
			if (num < 0 && this.habToDisplay.NetPower(true, true) < -num)
			{
				stringBuilder.AppendLine(Loc.T("UI.Habs.WarnInsufficientPower"));
				flag3 = true;
			}
			if (base.activePlayer.InsufficientBoostToSupportHabs())
			{
				stringBuilder.AppendLine(Loc.T("UI.Habs.WarnInsufficientSupport"));
				flag3 = true;
			}
			if (flag3)
			{
				stringBuilder.AppendLine(Loc.T("UI.Habs.WarnGenericQuery", new object[] { this.proposedModuleTemplate.displayName }));
			}
			this.confirmModuleQuery.SetText(stringBuilder.ToString());
		}

		// Token: 0x060050C9 RID: 20681 RVA: 0x00235AC0 File Offset: 0x00233CC0
		private bool IsLegalDrop()
		{
			if (!TIGameState.Valid(this.habToDisplay))
			{
				return false;
			}
			if (this.habToDisplay.sectors[this.sectorToPlace].ValidModuleForSlot(this.proposedModuleTemplate, this.moduleSlotToPlace) && !this.habToDisplay.sectors[this.sectorToPlace].habModules[this.moduleSlotToPlace].underConstruction)
			{
				return true;
			}
			if (this.sectorToPlace == 0 && this.moduleSlotToPlace == 0)
			{
				if (!this.proposedModuleTemplate.coreModule)
				{
					this.confirmModulePurchaseEarth.SetActive(false);
					this.confirmModulePurchaseSpace.SetActive(false);
					this.confirmModulePurchaseFailure.SetActive(true);
					this.cancelModulePurchase.SetActive(false);
					this.confirmModuleQuery.text = Loc.T("UI.Habs.CoreModuleOnly");
					return false;
				}
			}
			else if (this.proposedModuleTemplate.coreModule)
			{
				this.confirmModulePurchaseEarth.SetActive(false);
				this.confirmModulePurchaseSpace.SetActive(false);
				this.confirmModulePurchaseFailure.SetActive(true);
				this.cancelModulePurchase.SetActive(false);
				this.confirmModuleQuery.text = Loc.T("UI.Habs.NonCoreModuleOnly");
				return false;
			}
			if (this.habToDisplay.sectors[this.sectorToPlace].habModules[this.moduleSlotToPlace].decommissioning)
			{
				this.confirmModulePurchaseEarth.SetActive(false);
				this.confirmModulePurchaseSpace.SetActive(false);
				this.confirmModulePurchaseFailure.SetActive(true);
				this.cancelModulePurchase.SetActive(false);
				this.confirmModuleQuery.text = Loc.T("UI.Habs.DecommissionNoBuild");
				return false;
			}
			if (this.habToDisplay.IsBase && this.sectorToPlace == 0 && this.moduleSlotToPlace == 1 && !this.proposedModuleTemplate.mine)
			{
				this.confirmModulePurchaseEarth.SetActive(false);
				this.confirmModulePurchaseSpace.SetActive(false);
				this.confirmModulePurchaseFailure.SetActive(true);
				this.cancelModulePurchase.SetActive(false);
				this.confirmModuleQuery.text = Loc.T("UI.Habs.MineOnly");
				return false;
			}
			if (this.proposedModuleTemplate.mine && (this.sectorToPlace != 0 || (this.sectorToPlace == 0 && this.moduleSlotToPlace != 1)))
			{
				this.confirmModulePurchaseEarth.SetActive(false);
				this.confirmModulePurchaseSpace.SetActive(false);
				this.confirmModulePurchaseFailure.SetActive(true);
				this.cancelModulePurchase.SetActive(false);
				this.confirmModuleQuery.text = Loc.T("UI.Habs.NoMineHere");
				return false;
			}
			if (this.proposedModuleTemplate == this.oldModule)
			{
				this.confirmModulePurchaseEarth.SetActive(false);
				this.confirmModulePurchaseSpace.SetActive(false);
				this.confirmModulePurchaseFailure.SetActive(true);
				this.cancelModulePurchase.SetActive(false);
				this.confirmModuleQuery.text = Loc.T("UI.Habs.DontRepeat", new object[] { this.oldModule.displayName });
				return false;
			}
			if (this.habToDisplay.OnlyUpgradeAllowed(this.proposedModuleTemplate) && !this.proposedModuleTemplate.OnFutureOrPastUpgradePath(this.habToDisplay.sectors[this.sectorToPlace].habModules[this.moduleSlotToPlace].moduleTemplate))
			{
				this.confirmModulePurchaseEarth.SetActive(false);
				this.confirmModulePurchaseSpace.SetActive(false);
				this.confirmModulePurchaseFailure.SetActive(true);
				this.cancelModulePurchase.SetActive(false);
				this.confirmModuleQuery.text = Loc.T("UI.Habs.UpgradeOnly");
				return false;
			}
			if (this.habToDisplay.staticHab && (!this.habToDisplay.sectors[this.sectorToPlace].habModules[this.moduleSlotToPlace].destroyed || this.habToDisplay.sectors[this.sectorToPlace].habModules[this.moduleSlotToPlace].moduleTemplate != this.proposedModuleTemplate))
			{
				this.confirmModulePurchaseEarth.SetActive(false);
				this.confirmModulePurchaseSpace.SetActive(false);
				this.confirmModulePurchaseFailure.SetActive(true);
				this.cancelModulePurchase.SetActive(false);
				this.confirmModuleQuery.text = Loc.T("UI.Habs.StaticHab");
				return false;
			}
			return true;
		}

		// Token: 0x060050CA RID: 20682 RVA: 0x00235EF5 File Offset: 0x002340F5
		private TIResourcesCost GetEarthCost(bool moduleToPlaceIsUpgrade)
		{
			return this.proposedModuleTemplate.CostFromEarth(base.activePlayer, this.habToDisplay, moduleToPlaceIsUpgrade);
		}

		// Token: 0x060050CB RID: 20683 RVA: 0x00235F10 File Offset: 0x00234110
		private TIResourcesCost GetSpaceCost(bool moduleToPlaceIsUpgrade, TIHabModuleState moduleToUpgrade)
		{
			TIResourcesCost tiresourcesCost = this.proposedModuleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, moduleToPlaceIsUpgrade, false, 0, false);
			if (!tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
			{
				TIResourcesCost tiresourcesCost2 = this.proposedModuleTemplate.CostFromSpace(base.activePlayer, this.habToDisplay, moduleToPlaceIsUpgrade, true, 0, false);
				if (tiresourcesCost2.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
				{
					return tiresourcesCost2;
				}
			}
			return tiresourcesCost;
		}

		// Token: 0x060050CC RID: 20684 RVA: 0x00235F8C File Offset: 0x0023418C
		private void OnConfirmBuildModuleEarth()
		{
			SoundEffectController.PlayBuildHabModuleSound(this.proposedModuleTemplate, this.habToDisplay);
			if (this.proposedModuleTemplate.mine)
			{
				base.activePlayer.CompleteMilestone(CampaignMilestone.TutorialBuildMine);
			}
			this.AssignModule(this.GetEarthCost(this.moduleToPlaceIsUpgrade));
			this.prospectiveModule = this.proposedModuleTemplate;
			if (this.showAvailableModules)
			{
				this.SetMenuToSelectedModule(this.availableModuleListItems.FirstOrDefault<HabModuleListItem>((HabModuleListItem x) => x.GetModuleTemplate() == this.proposedModuleTemplate));
			}
			else
			{
				this.SetMenuToSelectedModule(this.installedModuleListItems.FirstOrDefault<HabModuleListItem>((HabModuleListItem x) => x.GetModuleTemplate() == this.proposedModuleTemplate));
			}
			this.UpdateModulePreviewText(this.showAvailableModules, false);
		}

		// Token: 0x060050CD RID: 20685 RVA: 0x00236034 File Offset: 0x00234234
		private void OnConfirmBuildModuleSpace()
		{
			SoundEffectController.PlayBuildHabModuleSound(this.proposedModuleTemplate, this.habToDisplay);
			if (this.proposedModuleTemplate.mine)
			{
				base.activePlayer.CompleteMilestone(CampaignMilestone.TutorialBuildMine);
			}
			this.AssignModule(this.GetSpaceCost(this.moduleToPlaceIsUpgrade, this.habToDisplay.GetModule(this.sectorToPlace, this.moduleSlotToPlace)));
			this.prospectiveModule = this.proposedModuleTemplate;
			if (this.showAvailableModules)
			{
				this.SetMenuToSelectedModule(this.availableModuleListItems.FirstOrDefault<HabModuleListItem>((HabModuleListItem x) => x.GetModuleTemplate() == this.proposedModuleTemplate));
			}
			else
			{
				this.SetMenuToSelectedModule(this.installedModuleListItems.FirstOrDefault<HabModuleListItem>((HabModuleListItem x) => x.GetModuleTemplate() == this.proposedModuleTemplate));
			}
			this.UpdateModulePreviewText(this.showAvailableModules, false);
		}

		// Token: 0x060050CE RID: 20686 RVA: 0x002360F2 File Offset: 0x002342F2
		private void OnConfirmModuleFailure()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.CloseModuleBuildPanel();
		}

		// Token: 0x060050CF RID: 20687 RVA: 0x00236106 File Offset: 0x00234306
		private void OnCancelBuildModule()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.CloseModuleBuildPanel();
		}

		// Token: 0x060050D0 RID: 20688 RVA: 0x0023611A File Offset: 0x0023431A
		private void CloseModuleBuildPanel()
		{
			this.confirmModulePopupCanvas.enabled = false;
		}

		// Token: 0x060050D1 RID: 20689 RVA: 0x00236128 File Offset: 0x00234328
		private void AssignModule(TIResourcesCost cost)
		{
			this.CloseModuleBuildPanel();
			this.SetupModuleBuild(cost);
			this.PreviewHab();
		}

		// Token: 0x060050D2 RID: 20690 RVA: 0x00236140 File Offset: 0x00234340
		private void SetupModuleBuild(TIResourcesCost cost)
		{
			base.activePlayer.playerControl.StartAction(new BuildHabModuleAction(this.proposedModuleTemplate, this.habToDisplay.sectors[this.sectorToPlace], this.moduleSlotToPlace, cost, new Action(this.BuildHab)));
		}

		// Token: 0x060050D3 RID: 20691 RVA: 0x00236191 File Offset: 0x00234391
		private void BuildHab()
		{
			if (this.habBuilder == null)
			{
				this.habBuilder = World.Active.GetExistingManager<HabBuilding>();
			}
			this.habBuilder.BuildHab(this.habToDisplay);
		}

		// Token: 0x060050D4 RID: 20692 RVA: 0x002361BC File Offset: 0x002343BC
		private void SetModulesInteractable(HabType habType, HabGridCell excludeItem = null)
		{
			if (habType == HabType.Station)
			{
				for (int i = 0; i < this.stationGridCells.Length; i++)
				{
					if (!(excludeItem != null) || !(excludeItem != this.stationGridCells[i]))
					{
						this.stationGridCells[i].SetInteractable(true);
					}
				}
				for (int j = 0; j < this.baseGridCells.Length; j++)
				{
					this.baseGridCells[j].SetInteractable(false);
				}
				return;
			}
			for (int k = 0; k < this.baseGridCells.Length; k++)
			{
				if (!(excludeItem != null) || !(excludeItem != this.baseGridCells[k]))
				{
					this.baseGridCells[k].SetInteractable(true);
				}
			}
			for (int l = 0; l < this.stationGridCells.Length; l++)
			{
				this.stationGridCells[l].SetInteractable(false);
			}
		}

		// Token: 0x060050D5 RID: 20693 RVA: 0x00236284 File Offset: 0x00234484
		public void OnClickRename()
		{
			if (this.habToDisplay == null || this.habToDisplay.faction != GameControl.control.activePlayer)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.ShowRenameMyHabPanel();
			if (this.habToDisplay != null)
			{
				this.nameInputField.text = this.habToDisplay.displayName;
			}
		}

		// Token: 0x060050D6 RID: 20694 RVA: 0x002362F2 File Offset: 0x002344F2
		public void OnClickRevertRename()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.RevertRename();
		}

		// Token: 0x060050D7 RID: 20695 RVA: 0x00236306 File Offset: 0x00234506
		public void RevertRename()
		{
			this.renameMyHabPanel.SetActive(false);
			this.nameInputField.text = "";
		}

		// Token: 0x060050D8 RID: 20696 RVA: 0x00236324 File Offset: 0x00234524
		public void OnClickSaveName()
		{
			if (this.habToDisplay == null)
			{
				return;
			}
			this.renameMyHabPanel.SetActive(false);
			this.habToDisplay.faction.playerControl.StartAction(new ChangeHabBio(this.habToDisplay, this.nameInputField.text, this.habToDisplay.customHabIconResource));
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.RefreshCanvas();
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
			GameControl.eventManager.TriggerEvent(new GameStateNameChanged(this.habToDisplay), null, Array.Empty<object>());
		}

		// Token: 0x060050D9 RID: 20697 RVA: 0x002363C0 File Offset: 0x002345C0
		public void OnClickChangeModuleMode(bool available)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, false);
			this.ChangeModuleMode(available);
		}

		// Token: 0x060050DA RID: 20698 RVA: 0x002363D8 File Offset: 0x002345D8
		public void ChangeModuleMode(bool available)
		{
			this.showAvailableModules = available;
			if (available)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui/UI_ActiveTab", this.availableModulesTabButtonImage);
				this.availableModulesTabButtonRT.sizeDelta = new Vector2(this.availableModulesTabButtonRT.sizeDelta.x, 28f);
				this.installedModulesTabButtonImage.sprite = this.originalButtonSprite;
				this.installedModulesTabButtonRT.sizeDelta = new Vector2(this.installedModulesTabButtonRT.sizeDelta.x, 25f);
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui/UI_ActiveTab", this.installedModulesTabButtonImage);
				this.installedModulesTabButtonRT.sizeDelta = new Vector2(this.installedModulesTabButtonRT.sizeDelta.x, 28f);
				this.availableModulesTabButtonImage.sprite = this.originalButtonSprite;
				this.availableModulesTabButtonRT.sizeDelta = new Vector2(this.availableModulesTabButtonRT.sizeDelta.x, 25f);
			}
			if (this.habToDisplay != null)
			{
				this.UpdateModuleList(this.habToDisplay.habType);
			}
		}

		// Token: 0x060050DB RID: 20699 RVA: 0x002364F0 File Offset: 0x002346F0
		public void OnToggleQuickBuildModules()
		{
			TIGlobalValuesState.GlobalValues.habQuickBuildToggle = this.quickBuildToggle.isOn;
			if (GameControl.loadcycle100)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
		}

		// Token: 0x060050DC RID: 20700 RVA: 0x0023651A File Offset: 0x0023471A
		public void OnToggleQuickBuildWithBoost()
		{
			if (GameControl.loadcycle100)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
		}

		// Token: 0x060050DD RID: 20701 RVA: 0x0023652F File Offset: 0x0023472F
		public void ShowRenameMyHabPanel()
		{
			this.renameMyHabPanel.SetActive(true);
			this.nameInputField.Select();
		}

		// Token: 0x060050DE RID: 20702 RVA: 0x00236548 File Offset: 0x00234748
		public void OnSelectInputBox()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x060050DF RID: 20703 RVA: 0x0023654F File Offset: 0x0023474F
		public void OnDeSelectInputBox()
		{
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x060050E0 RID: 20704 RVA: 0x00236556 File Offset: 0x00234756
		public void UpdateHabNameSortFilter()
		{
			if (this.habs_filterNameInputField.text.Equals(this.habs_nameFilterForHabs))
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.habs_nameFilterForHabs = this.habs_filterNameInputField.text;
			this.UpdateHabLists();
		}

		// Token: 0x060050E1 RID: 20705 RVA: 0x00236594 File Offset: 0x00234794
		public void UpdateModuleNameSortFilter()
		{
			if (this.modules_filterNameInputField.text.Equals(this.modules_nameFilterForModules) || this.habToDisplay == null)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.modules_nameFilterForModules = this.modules_filterNameInputField.text;
			this.UpdateModuleList(this.habToDisplay.habType);
		}

		// Token: 0x060050E2 RID: 20706 RVA: 0x002365F6 File Offset: 0x002347F6
		public override void OnDestroy()
		{
			base.OnDestroy();
			this.RemoveListeners();
		}

		// Token: 0x060050E3 RID: 20707 RVA: 0x00236604 File Offset: 0x00234804
		private void RemoveListeners()
		{
			this.exitButton.onClick.RemoveAllListeners();
			this.habManageButton.onClick.RemoveAllListeners();
			this.closeHabManageButton.onClick.RemoveAllListeners();
			this.moduleUpgradeButton.onClick.RemoveListener(new UnityAction(this.OnModuleUpgrade));
			this.confirmModulePurchaseEarthButton.onClick.RemoveListener(new UnityAction(this.OnConfirmBuildModuleEarth));
			this.confirmModulePurchaseSpaceButton.onClick.RemoveListener(new UnityAction(this.OnConfirmBuildModuleSpace));
			this.confirmModulePurchaseFailureButton.onClick.RemoveListener(new UnityAction(this.OnConfirmModuleFailure));
			this.cancelModulePurchaseButton.onClick.RemoveListener(new UnityAction(this.OnCancelBuildModule));
		}

		// Token: 0x060050EA RID: 20714 RVA: 0x002368E4 File Offset: 0x00234AE4
		[CompilerGenerated]
		private void <SetMineProductivityValues>g__SetMineText|321_0(TMP_Text textBlock, FactionResource resource)
		{
			float miningIncome_Month = this.habToDisplay.mine.moduleTemplate.GetMiningIncome_Month(this.habToDisplay.faction, this.habToDisplay.habSite, resource);
			string text = TIUtilities.FormatSmallNumber(miningIncome_Month, 7, 0, true, false);
			if (miningIncome_Month == 0f)
			{
				text = TIUtilities.CyanLine(text);
			}
			else if (this.habToDisplay.mine.active)
			{
				text = TIUtilities.GreenLine(text);
			}
			else
			{
				text = TIUtilities.RedLine(text);
			}
			textBlock.SetText(text);
		}

		// Token: 0x040033B1 RID: 13233
		private HabBuilding habBuilder;

		// Token: 0x040033B2 RID: 13234
		public UITutorialController HabScreenMainUITutorialController;

		// Token: 0x040033B3 RID: 13235
		public UITutorialController HabScreenManagementUITutorialController;

		// Token: 0x040033B4 RID: 13236
		public GameObject habsMainPanel;

		// Token: 0x040033B5 RID: 13237
		public GameObject habListItemPrefab;

		// Token: 0x040033B6 RID: 13238
		public GameObject moduleListItemPrefab;

		// Token: 0x040033B7 RID: 13239
		public Color habListItemSelectedColor;

		// Token: 0x040033B8 RID: 13240
		public float habDisplayCellSizeMin = 48f;

		// Token: 0x040033B9 RID: 13241
		public float habDisplayCellSizeMax = 200f;

		// Token: 0x040033BA RID: 13242
		private Button exitButton;

		// Token: 0x040033BB RID: 13243
		public TIHabState habToDisplay;

		// Token: 0x040033BC RID: 13244
		public List<HabScreenHabListItemModel> habModels = new List<HabScreenHabListItemModel>();

		// Token: 0x040033BD RID: 13245
		public HabScreenHabListAdapter habListAdapter;

		// Token: 0x040033BE RID: 13246
		private HabitatsScreenController.SortHabDataBy currentHabSort;

		// Token: 0x040033BF RID: 13247
		private bool reverseHabSort;

		// Token: 0x040033C0 RID: 13248
		public List<TIHabState> selectedHabList = new List<TIHabState>();

		// Token: 0x040033C1 RID: 13249
		private TIHabModuleTemplate[] habModuleTemplates;

		// Token: 0x040033C2 RID: 13250
		private Dictionary<string, TIHabModuleTemplate> habModuleDictionary;

		// Token: 0x040033C3 RID: 13251
		private bool showAvailableModules = true;

		// Token: 0x040033C4 RID: 13252
		public Image installedModulesTabButtonImage;

		// Token: 0x040033C5 RID: 13253
		public RectTransform installedModulesTabButtonRT;

		// Token: 0x040033C6 RID: 13254
		public Image availableModulesTabButtonImage;

		// Token: 0x040033C7 RID: 13255
		public RectTransform availableModulesTabButtonRT;

		// Token: 0x040033C8 RID: 13256
		public Sprite originalButtonSprite;

		// Token: 0x040033C9 RID: 13257
		private string habList_FilterForHabIcon;

		// Token: 0x040033CA RID: 13258
		private TIFactionState habList_FilterForFaction;

		// Token: 0x040033CB RID: 13259
		private TISpaceBodyState habList_FilterForSpaceObject;

		// Token: 0x040033CC RID: 13260
		private bool habList_FilterHumanFactionsOnly;

		// Token: 0x040033CD RID: 13261
		private HabType habList_FilterForHabType;

		// Token: 0x040033CE RID: 13262
		public TMP_Dropdown habIconFilterDropdown;

		// Token: 0x040033CF RID: 13263
		public TMP_Dropdown factionsDropdown;

		// Token: 0x040033D0 RID: 13264
		public TMP_Dropdown locationDropdown;

		// Token: 0x040033D1 RID: 13265
		private Dictionary<int, TIFactionState> factionDropdownLookup;

		// Token: 0x040033D2 RID: 13266
		private Dictionary<int, TISpaceBodyState> locationDropdownLookup;

		// Token: 0x040033D3 RID: 13267
		public bool applyingMassTemplates;

		// Token: 0x040033D4 RID: 13268
		private TIHabTemplate habList_FilterForTemplate;

		// Token: 0x040033D5 RID: 13269
		public TMP_Dropdown massTemplatesDropdown;

		// Token: 0x040033D6 RID: 13270
		public TMP_Text massTemplatesHeaderText;

		// Token: 0x040033D7 RID: 13271
		public TMP_Dropdown massTemplatesHabIconSelectionDropdown;

		// Token: 0x040033D8 RID: 13272
		public TMP_Dropdown quickTemplatesDropdown;

		// Token: 0x040033D9 RID: 13273
		public Toggle stationsToggle;

		// Token: 0x040033DA RID: 13274
		public Toggle basesToggle;

		// Token: 0x040033DB RID: 13275
		public List<Button> sortButtons = new List<Button>();

		// Token: 0x040033DC RID: 13276
		public GameObject antimatterSortButtonObject;

		// Token: 0x040033DD RID: 13277
		public GameObject exoticsSortButtonObject;

		// Token: 0x040033DE RID: 13278
		public Image habListScrollHeader;

		// Token: 0x040033DF RID: 13279
		public GameObject habListScrollBar;

		// Token: 0x040033E0 RID: 13280
		public RectTransform selectedHabInfoContainerRT;

		// Token: 0x040033E1 RID: 13281
		public GameObject gravityDisplayObject;

		// Token: 0x040033E2 RID: 13282
		public TMP_Text selectedHabGravity;

		// Token: 0x040033E3 RID: 13283
		public GameObject selectedHabCrewDisplayObject;

		// Token: 0x040033E4 RID: 13284
		public TMP_Text selectedHabCrew;

		// Token: 0x040033E5 RID: 13285
		public GameObject councilorGridPanel;

		// Token: 0x040033E6 RID: 13286
		public ListManagerBase councilorGrid;

		// Token: 0x040033E7 RID: 13287
		public Canvas primaryHabitatsCanvas;

		// Token: 0x040033E8 RID: 13288
		public Canvas secondaryHabitatsCanvas;

		// Token: 0x040033E9 RID: 13289
		public GameObject habListMasterObject;

		// Token: 0x040033EA RID: 13290
		public GameObject habPreviewInfoPanel;

		// Token: 0x040033EB RID: 13291
		public GameObject moduleSelectionPanel;

		// Token: 0x040033EC RID: 13292
		public GameObject nextHabButtonsContainer;

		// Token: 0x040033ED RID: 13293
		public GameObject previousHabButtonsContainer;

		// Token: 0x040033EE RID: 13294
		public Button nextHabButton;

		// Token: 0x040033EF RID: 13295
		public Button nextSmartHabButton;

		// Token: 0x040033F0 RID: 13296
		public Button nextIconHabButton;

		// Token: 0x040033F1 RID: 13297
		public Image nextIconHabButtonImage;

		// Token: 0x040033F2 RID: 13298
		public Button previousHabButton;

		// Token: 0x040033F3 RID: 13299
		public Button previousSmartHabButton;

		// Token: 0x040033F4 RID: 13300
		public Button previousIconHabButton;

		// Token: 0x040033F5 RID: 13301
		public Image previousIconHabButtonImage;

		// Token: 0x040033F6 RID: 13302
		private int displayModuleSector = -1;

		// Token: 0x040033F7 RID: 13303
		private int displayModuleSlot;

		// Token: 0x040033F8 RID: 13304
		private bool managingHab;

		// Token: 0x040033F9 RID: 13305
		private bool habDisplayDataDirty;

		// Token: 0x040033FA RID: 13306
		public TMP_Dropdown habinfoIconDropdown;

		// Token: 0x040033FB RID: 13307
		private List<string> habIconPaths;

		// Token: 0x040033FC RID: 13308
		private HabInfoListItem[] habInfoListItems;

		// Token: 0x040033FD RID: 13309
		private HabInfoListItem habNoneSelected;

		// Token: 0x040033FE RID: 13310
		private HabInfoListItem habTier;

		// Token: 0x040033FF RID: 13311
		private HabInfoListItem habLocation;

		// Token: 0x04003400 RID: 13312
		public Button PowerAllButton;

		// Token: 0x04003401 RID: 13313
		public GameObject PowerAllFillerButtonObject;

		// Token: 0x04003402 RID: 13314
		public Button DecommissionHabButton;

		// Token: 0x04003403 RID: 13315
		public Button DecommissionModuleButton;

		// Token: 0x04003404 RID: 13316
		public TMP_Text PowerAllButtonText;

		// Token: 0x04003405 RID: 13317
		public TMP_Text DecommissionHabButtonText;

		// Token: 0x04003406 RID: 13318
		public TMP_Text DecommissionModuleButtonText;

		// Token: 0x04003407 RID: 13319
		private int lastSort;

		// Token: 0x04003408 RID: 13320
		public TMP_InputField habs_filterNameInputField;

		// Token: 0x04003409 RID: 13321
		private string habs_nameFilterForHabs = "";

		// Token: 0x0400340A RID: 13322
		public TMP_InputField modules_filterNameInputField;

		// Token: 0x0400340B RID: 13323
		private string modules_nameFilterForModules = "";

		// Token: 0x0400340C RID: 13324
		private Dictionary<TIHabModuleTemplate, HabModuleListItem> availableModuleDictionary;

		// Token: 0x0400340D RID: 13325
		private Dictionary<TIHabModuleState, HabModuleListItem> installedModuleDictionary;

		// Token: 0x0400340E RID: 13326
		private List<HabModuleListItem> availableModuleListItems;

		// Token: 0x0400340F RID: 13327
		private List<HabModuleListItem> installedModuleListItems;

		// Token: 0x04003410 RID: 13328
		public GameObject availableModuleListObject;

		// Token: 0x04003411 RID: 13329
		private HabInfoListItem[] moduleInfoListItems;

		// Token: 0x04003412 RID: 13330
		private HabInfoListItem moduleNoneSelected;

		// Token: 0x04003413 RID: 13331
		private HabInfoListItem moduleEmpty;

		// Token: 0x04003414 RID: 13332
		private HabInfoListItem moduleName;

		// Token: 0x04003415 RID: 13333
		private HabInfoListItem moduleTier;

		// Token: 0x04003416 RID: 13334
		private HabInfoListItem moduleUpgrade;

		// Token: 0x04003417 RID: 13335
		private HabInfoListItem modulePower;

		// Token: 0x04003418 RID: 13336
		private HabInfoListItem moduleCrew;

		// Token: 0x04003419 RID: 13337
		private Button modulePowerToggle;

		// Token: 0x0400341A RID: 13338
		public Button habManageButton;

		// Token: 0x0400341B RID: 13339
		public Button closeHabManageButton;

		// Token: 0x0400341C RID: 13340
		public Button habGotoButton;

		// Token: 0x0400341D RID: 13341
		public GameObject zoomContainer;

		// Token: 0x0400341E RID: 13342
		public TMP_Text habManageButtonText;

		// Token: 0x0400341F RID: 13343
		public TMP_Text closeHabManageButtonText;

		// Token: 0x04003420 RID: 13344
		private ScrollRect habDisplayScrollView;

		// Token: 0x04003421 RID: 13345
		private UIPointerHoverTracker habitatsScreenPreviewMouseOverTracker;

		// Token: 0x04003422 RID: 13346
		private RectTransform habScrollViewRectTransform;

		// Token: 0x04003423 RID: 13347
		private Slider habDisplayZoomSlider;

		// Token: 0x04003424 RID: 13348
		private RectTransform habDisplayRectTransform;

		// Token: 0x04003425 RID: 13349
		private GameObject noHabSelected;

		// Token: 0x04003426 RID: 13350
		public TMP_Text noHabSelectedText;

		// Token: 0x04003427 RID: 13351
		public GameObject powerReportTextObject;

		// Token: 0x04003428 RID: 13352
		public TMP_Text powerReportText;

		// Token: 0x04003429 RID: 13353
		public TooltipTrigger powerReportTip;

		// Token: 0x0400342A RID: 13354
		public TMP_Text mainHeaderText;

		// Token: 0x0400342B RID: 13355
		public TMP_Text listHeaderText;

		// Token: 0x0400342C RID: 13356
		public TMP_Text availableModulesHeaderText;

		// Token: 0x0400342D RID: 13357
		public TMP_Text installedModulesHeaderText;

		// Token: 0x0400342E RID: 13358
		public TMP_Text habMapHeaderText;

		// Token: 0x0400342F RID: 13359
		public GameObject habSubtitleObject;

		// Token: 0x04003430 RID: 13360
		public TMP_Text habMapTypeText;

		// Token: 0x04003431 RID: 13361
		public Image habMapLocationIcon;

		// Token: 0x04003432 RID: 13362
		public TMP_Text habMapLocationText;

		// Token: 0x04003433 RID: 13363
		public TMP_Text habZoomText;

		// Token: 0x04003434 RID: 13364
		[HideInInspector]
		public Dictionary<string, Sprite> connectors = new Dictionary<string, Sprite>();

		// Token: 0x04003435 RID: 13365
		[HideInInspector]
		public Dictionary<string, string> connectorSwaps = new Dictionary<string, string>();

		// Token: 0x04003436 RID: 13366
		private Canvas stationDisplayCanvas;

		// Token: 0x04003437 RID: 13367
		private GridLayoutGroup stationDisplayGridLayout;

		// Token: 0x04003438 RID: 13368
		private GridLayoutGroup torusGrid;

		// Token: 0x04003439 RID: 13369
		private Image torus1_2;

		// Token: 0x0400343A RID: 13370
		private Image torus2_3;

		// Token: 0x0400343B RID: 13371
		private Image torus3_4;

		// Token: 0x0400343C RID: 13372
		private Image torus4_1;

		// Token: 0x0400343D RID: 13373
		private RectTransform stationDisplayGridRectTransform;

		// Token: 0x0400343E RID: 13374
		private RectTransform torusGridRectTransform;

		// Token: 0x0400343F RID: 13375
		private StationGridCell[] stationGridCells;

		// Token: 0x04003440 RID: 13376
		private Dictionary<string, StationGridCell> stationCellDictionary;

		// Token: 0x04003441 RID: 13377
		private HabGridCell selectedStationModule;

		// Token: 0x04003442 RID: 13378
		private Canvas baseDisplayCanvas;

		// Token: 0x04003443 RID: 13379
		private RectTransform baseDisplayRectTransform;

		// Token: 0x04003444 RID: 13380
		private GridLayoutGroup baseDisplayGridLayout;

		// Token: 0x04003445 RID: 13381
		private RectTransform baseDisplayGridRectTransform;

		// Token: 0x04003446 RID: 13382
		private Image baseSurfaceImage;

		// Token: 0x04003447 RID: 13383
		private RectTransform baseSurfaceRectTransform;

		// Token: 0x04003448 RID: 13384
		private BaseGridCell[] baseGridCells;

		// Token: 0x04003449 RID: 13385
		private Dictionary<string, BaseGridCell> baseCellDictionary;

		// Token: 0x0400344A RID: 13386
		private HabGridCell selectedBaseModule;

		// Token: 0x0400344B RID: 13387
		public GameObject habSiteProductivityPanel;

		// Token: 0x0400344C RID: 13388
		public TMP_Text siteWater;

		// Token: 0x0400344D RID: 13389
		public TMP_Text siteVolatiles;

		// Token: 0x0400344E RID: 13390
		public TMP_Text siteMetals;

		// Token: 0x0400344F RID: 13391
		public TMP_Text siteNobles;

		// Token: 0x04003450 RID: 13392
		public TMP_Text siteFissiles;

		// Token: 0x04003451 RID: 13393
		public TMP_Text siteSolar;

		// Token: 0x04003452 RID: 13394
		private Canvas confirmModulePopupCanvas;

		// Token: 0x04003453 RID: 13395
		public TMP_Text confirmModuleQuery;

		// Token: 0x04003454 RID: 13396
		private GameObject confirmModulePurchaseEarth;

		// Token: 0x04003455 RID: 13397
		private Button confirmModulePurchaseEarthButton;

		// Token: 0x04003456 RID: 13398
		public TMP_Text confirmModulePurchaseEarthCostText;

		// Token: 0x04003457 RID: 13399
		private GameObject confirmModulePurchaseSpace;

		// Token: 0x04003458 RID: 13400
		private Button confirmModulePurchaseSpaceButton;

		// Token: 0x04003459 RID: 13401
		public TMP_Text confirmModulePurchaseSpaceCostText;

		// Token: 0x0400345A RID: 13402
		private GameObject confirmModulePurchaseFailure;

		// Token: 0x0400345B RID: 13403
		private Button confirmModulePurchaseFailureButton;

		// Token: 0x0400345C RID: 13404
		public TMP_Text confirmModulePurchaseFailureButtonText;

		// Token: 0x0400345D RID: 13405
		private GameObject cancelModulePurchase;

		// Token: 0x0400345E RID: 13406
		private Button cancelModulePurchaseButton;

		// Token: 0x0400345F RID: 13407
		public TMP_Text cancelModulePurchaseButtonText;

		// Token: 0x04003460 RID: 13408
		private TIHabModuleTemplate proposedModuleTemplate;

		// Token: 0x04003461 RID: 13409
		private TIHabModuleState proposedModuleState;

		// Token: 0x04003462 RID: 13410
		private TIResourcesCost earthCost;

		// Token: 0x04003463 RID: 13411
		private TIResourcesCost spaceCost;

		// Token: 0x04003464 RID: 13412
		private bool moduleToPlaceIsUpgrade;

		// Token: 0x04003465 RID: 13413
		private bool moduleToPlaceIsBuildOver;

		// Token: 0x04003466 RID: 13414
		private int sectorToPlace;

		// Token: 0x04003467 RID: 13415
		private int moduleSlotToPlace;

		// Token: 0x04003468 RID: 13416
		public Toggle quickBuildToggle;

		// Token: 0x04003469 RID: 13417
		public TMP_Text quickBuildText;

		// Token: 0x0400346A RID: 13418
		public TooltipTrigger quickBuildTooltip;

		// Token: 0x0400346B RID: 13419
		public Toggle quickBuildWithBoostToggle;

		// Token: 0x0400346C RID: 13420
		public TMP_Text quickBuildWithBoostText;

		// Token: 0x0400346D RID: 13421
		[Header("My Hab Customization")]
		public GameObject renameMyHabPanel;

		// Token: 0x0400346E RID: 13422
		public TextMeshProUGUI saveNameText;

		// Token: 0x0400346F RID: 13423
		public TextMeshProUGUI revertNameText;

		// Token: 0x04003470 RID: 13424
		public TMP_InputField nameInputField;

		// Token: 0x04003471 RID: 13425
		public GameObject editNameButton;

		// Token: 0x04003472 RID: 13426
		public GameObject editNameIcon;

		// Token: 0x04003473 RID: 13427
		private readonly List<string> connectorSpritePaths = new List<string>
		{
			"habModules/station_connector_A", "habModules/station_connector_B", "habModules/station_connector_C", "habModules/base_connector_A", "habModules/base_connector_B", "habModules/base_connector_C", "habModules/base_connector_D", "habModules/station_Alien_Connector", "habModules/station_Alien_T_Connector", "habModules/station_Alien_4_Connector",
			"habModules/station_alien_Connector_C", "habModules/base_connector_A_alien", "habModules/base_connector_B_alien", "habModules/base_connector_C_alien", "habModules/base_connector_D_alien"
		};

		// Token: 0x04003474 RID: 13428
		private TIDateTime lastResourceUpdateCheck;

		// Token: 0x04003475 RID: 13429
		public TMP_Text resourceSummaryTitleLine;

		// Token: 0x04003476 RID: 13430
		public TMP_Text summaryTitleLine;

		// Token: 0x04003477 RID: 13431
		public ListManagerBase summaryResourceGrid;

		// Token: 0x04003478 RID: 13432
		public CenteredGridLayoutGroup summaryResourceGridLayout;

		// Token: 0x04003479 RID: 13433
		public GameObject managementQueryObject;

		// Token: 0x0400347A RID: 13434
		public TMP_Text managementQueryText;

		// Token: 0x0400347B RID: 13435
		public Button managementQueryConfirmButton;

		// Token: 0x0400347C RID: 13436
		public GameObject managementQueryConfirmButtonObject;

		// Token: 0x0400347D RID: 13437
		public TMP_Text managementQueryConfirmButtonText;

		// Token: 0x0400347E RID: 13438
		public Button managementQueryCancelButton;

		// Token: 0x0400347F RID: 13439
		public GameObject managementQueryCancelButtonObject;

		// Token: 0x04003480 RID: 13440
		public TMP_Text managementQueryCancelButtonText;

		// Token: 0x04003481 RID: 13441
		public GameObject managementQueryTemplateDropdownObject;

		// Token: 0x04003482 RID: 13442
		public TMP_Dropdown managementQueryTemplateDropdown;

		// Token: 0x04003483 RID: 13443
		public GameObject managementQuerySelectedHabDropdownObject;

		// Token: 0x04003484 RID: 13444
		public TMP_Dropdown managementQuerySelectedHabDropdown;

		// Token: 0x04003485 RID: 13445
		public GameObject managementQueryToggleObject;

		// Token: 0x04003486 RID: 13446
		public Toggle managementQueryToggle;

		// Token: 0x04003487 RID: 13447
		public TMP_Text managementQueryToggleText;

		// Token: 0x04003488 RID: 13448
		public Image maxTierIcon;

		// Token: 0x04003489 RID: 13449
		public TooltipTrigger maxTierTooltip;

		// Token: 0x0400348A RID: 13450
		public bool queryDecommissionModule;

		// Token: 0x0400348B RID: 13451
		[Header("Module data")]
		public TMP_Text modulePanelHeaderText;

		// Token: 0x0400348C RID: 13452
		public GameObject sectorOwnerGO;

		// Token: 0x0400348D RID: 13453
		public Image sectorOwner;

		// Token: 0x0400348E RID: 13454
		public TMP_Text sectorText;

		// Token: 0x0400348F RID: 13455
		public Image moduleIcon;

		// Token: 0x04003490 RID: 13456
		public GameObject summaryPanel;

		// Token: 0x04003491 RID: 13457
		public RectTransform summaryScrollViewContainer;

		// Token: 0x04003492 RID: 13458
		public TMP_Text tierText;

		// Token: 0x04003493 RID: 13459
		public Image tierFrame;

		// Token: 0x04003494 RID: 13460
		public Sprite[] tierFrameSprites;

		// Token: 0x04003495 RID: 13461
		public TMP_Text crewText;

		// Token: 0x04003496 RID: 13462
		public TMP_Text massText;

		// Token: 0x04003497 RID: 13463
		public ListManagerBase incomeGrid;

		// Token: 0x04003498 RID: 13464
		public GameObject incomeDataPanel;

		// Token: 0x04003499 RID: 13465
		public TMP_Text incomeGridHeader;

		// Token: 0x0400349A RID: 13466
		public TMP_Text constructionCostHeader;

		// Token: 0x0400349B RID: 13467
		public TMP_Text supportCostHeader;

		// Token: 0x0400349C RID: 13468
		public GameObject constructionDataPanel;

		// Token: 0x0400349D RID: 13469
		public TMP_Text constructionCostString;

		// Token: 0x0400349E RID: 13470
		public GameObject supportDataPanel;

		// Token: 0x0400349F RID: 13471
		public TMP_Text supportCostString;

		// Token: 0x040034A0 RID: 13472
		public GameObject upgradePanel;

		// Token: 0x040034A1 RID: 13473
		public TMP_Text upgradeHeader;

		// Token: 0x040034A2 RID: 13474
		public TMP_Text upgradeModuleName;

		// Token: 0x040034A3 RID: 13475
		public TMP_Text upgradeModuleButtonText;

		// Token: 0x040034A4 RID: 13476
		public Button moduleUpgradeButton;

		// Token: 0x040034A5 RID: 13477
		public Button moduleUpgradeAllOfTypeButton;

		// Token: 0x040034A6 RID: 13478
		public TMP_Text moduleUpgradeAllOfTypeButtonText;

		// Token: 0x040034A7 RID: 13479
		public GameObject powerPanel;

		// Token: 0x040034A8 RID: 13480
		public Button powerButton;

		// Token: 0x040034A9 RID: 13481
		public TMP_Text powerPanelTitle;

		// Token: 0x040034AA RID: 13482
		public TMP_Text powerPanelValue;

		// Token: 0x040034AB RID: 13483
		public TMP_Text powerPanelOnOffButtonText;

		// Token: 0x040034AC RID: 13484
		public Button shipyardButton;

		// Token: 0x040034AD RID: 13485
		[HideInInspector]
		public TIHabModuleTemplate prospectiveModule;

		// Token: 0x040034AE RID: 13486
		private string moduleUpgradeDataName;

		// Token: 0x040034AF RID: 13487
		public GameObject moduleInstalledPanel;

		// Token: 0x040034B0 RID: 13488
		public TMP_Text moduleInstalledText;

		// Token: 0x040034B1 RID: 13489
		public GameObject moduleUnderConstructionPanel;

		// Token: 0x040034B2 RID: 13490
		public TMP_Text moduleCompletionDateText;

		// Token: 0x040034B3 RID: 13491
		public GameObject globalRebuildButtonObject;

		// Token: 0x040034B4 RID: 13492
		public TMP_Text globalRebuildButtonText;

		// Token: 0x040034B5 RID: 13493
		public GameObject globalRebuildButtonFillerObject;

		// Token: 0x040034B6 RID: 13494
		public GameObject globalUpgradeButtonObject;

		// Token: 0x040034B7 RID: 13495
		public TMP_Text globalUpgradeButtonText;

		// Token: 0x040034B8 RID: 13496
		public GameObject globalUpgradeButtonFillerObject;

		// Token: 0x040034B9 RID: 13497
		public TMP_Text habTemplateTitleText;

		// Token: 0x040034BA RID: 13498
		public TMP_Text habTemplateNameText;

		// Token: 0x040034BB RID: 13499
		public Button saveHabButton;

		// Token: 0x040034BC RID: 13500
		public TMP_Text saveHabButtonText;

		// Token: 0x040034BD RID: 13501
		public Button manageHabTemplatesButton;

		// Token: 0x040034BE RID: 13502
		public TMP_Text manageHabTemplatesButtonText;

		// Token: 0x040034BF RID: 13503
		public TMP_Text manageHabTemplatesHeader;

		// Token: 0x040034C0 RID: 13504
		public GameObject manageHabTemplatesPanel;

		// Token: 0x040034C1 RID: 13505
		public ListManagerBase manageHabTemplatesList;

		// Token: 0x040034C2 RID: 13506
		private Dictionary<int, string> habTemplateDropdown = new Dictionary<int, string>();

		// Token: 0x040034C3 RID: 13507
		private Dictionary<int, string> habSelectionDropdown = new Dictionary<int, string>();

		// Token: 0x040034C4 RID: 13508
		private Dictionary<int, string> massHabTemplateDropdown = new Dictionary<int, string>();

		// Token: 0x040034C5 RID: 13509
		private Dictionary<int, string> quickHabTemplateDropdown = new Dictionary<int, string>();

		// Token: 0x040034C6 RID: 13510
		public Sprite cachedButtonSprite;

		// Token: 0x040034C7 RID: 13511
		public TMP_Text allTiersButtonText;

		// Token: 0x040034C8 RID: 13512
		public TMP_Text allBenefitsButtonText;

		// Token: 0x040034C9 RID: 13513
		public int tierFilter;

		// Token: 0x040034CA RID: 13514
		public Button[] tierButtons;

		// Token: 0x040034CB RID: 13515
		public HabitatsScreenController.AvailableModuleFilters benefitFilter;

		// Token: 0x040034CC RID: 13516
		public Button[] benefitButtons;

		// Token: 0x040034CD RID: 13517
		public GameObject moduleListAntimatterSortButtonObject;

		// Token: 0x040034CE RID: 13518
		private TIHabModuleTemplate oldModule;

		// Token: 0x020010AC RID: 4268
		private struct IncomeEntry
		{
			// Token: 0x06008469 RID: 33897 RVA: 0x0032EDAA File Offset: 0x0032CFAA
			public IncomeEntry(string ip, string v)
			{
				this.iconResourcePath = ip;
				this.value = v;
			}

			// Token: 0x0400640F RID: 25615
			public string iconResourcePath;

			// Token: 0x04006410 RID: 25616
			public string value;
		}

		// Token: 0x020010AD RID: 4269
		public enum AvailableModuleFilters
		{
			// Token: 0x04006412 RID: 25618
			None,
			// Token: 0x04006413 RID: 25619
			Core,
			// Token: 0x04006414 RID: 25620
			PowerSupplier,
			// Token: 0x04006415 RID: 25621
			ShipConstruction,
			// Token: 0x04006416 RID: 25622
			Resupply,
			// Token: 0x04006417 RID: 25623
			ModuleConstruction,
			// Token: 0x04006418 RID: 25624
			Mine,
			// Token: 0x04006419 RID: 25625
			Farm,
			// Token: 0x0400641A RID: 25626
			SpaceCombat,
			// Token: 0x0400641B RID: 25627
			AssaultCombat,
			// Token: 0x0400641C RID: 25628
			Income_Money,
			// Token: 0x0400641D RID: 25629
			Income_Influence,
			// Token: 0x0400641E RID: 25630
			Income_Research,
			// Token: 0x0400641F RID: 25631
			Income_Projects,
			// Token: 0x04006420 RID: 25632
			Income_MissionControl,
			// Token: 0x04006421 RID: 25633
			Income_Antimatter,
			// Token: 0x04006422 RID: 25634
			ControlPointCapacity,
			// Token: 0x04006423 RID: 25635
			TechBonuses,
			// Token: 0x04006424 RID: 25636
			LEOBonuses
		}

		// Token: 0x020010AE RID: 4270
		public enum SortHabDataBy
		{
			// Token: 0x04006426 RID: 25638
			Alfa,
			// Token: 0x04006427 RID: 25639
			MissionControl,
			// Token: 0x04006428 RID: 25640
			Water,
			// Token: 0x04006429 RID: 25641
			Volatiles,
			// Token: 0x0400642A RID: 25642
			Metals,
			// Token: 0x0400642B RID: 25643
			NobleMetals,
			// Token: 0x0400642C RID: 25644
			Fissiles,
			// Token: 0x0400642D RID: 25645
			Antimatter,
			// Token: 0x0400642E RID: 25646
			Exotics,
			// Token: 0x0400642F RID: 25647
			Resupply,
			// Token: 0x04006430 RID: 25648
			Shipyard,
			// Token: 0x04006431 RID: 25649
			CombatStrength,
			// Token: 0x04006432 RID: 25650
			Defended,
			// Token: 0x04006433 RID: 25651
			UnderConstruction,
			// Token: 0x04006434 RID: 25652
			SpaceAssaultScore,
			// Token: 0x04006435 RID: 25653
			CustomIcon,
			// Token: 0x04006436 RID: 25654
			Location,
			// Token: 0x04006437 RID: 25655
			Tier,
			// Token: 0x04006438 RID: 25656
			Population,
			// Token: 0x04006439 RID: 25657
			Power,
			// Token: 0x0400643A RID: 25658
			ModuleConstruction,
			// Token: 0x0400643B RID: 25659
			Money,
			// Token: 0x0400643C RID: 25660
			Influence,
			// Token: 0x0400643D RID: 25661
			Ops,
			// Token: 0x0400643E RID: 25662
			Research,
			// Token: 0x0400643F RID: 25663
			Projects,
			// Token: 0x04006440 RID: 25664
			Boost
		}
	}
}
