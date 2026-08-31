using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008DA RID: 2266
	public class SpaceObjectDetailController : CanvasControllerBase
	{
		// Token: 0x060056B1 RID: 22193 RVA: 0x0027AC8C File Offset: 0x00278E8C
		public override void Initialize()
		{
			base.Initialize();
			this.naturalBodyPanel.gameObject.SetActive(true);
			this.naturalBodyPanel.enabled = false;
			this.enemySpaceFleetPanel.gameObject.SetActive(true);
			this.enemySpaceFleetPanel.enabled = false;
			this.mySpaceFleetPanel.gameObject.SetActive(true);
			this.mySpaceFleetPanel.enabled = false;
			this.habPanel.gameObject.SetActive(true);
			this.habPanel.enabled = false;
			this.lagrangePointPanel.gameObject.SetActive(true);
			this.lagrangePointPanel.enabled = false;
			this.fleetName.SetText(Loc.T("UI.Space.Fleet.Header.Name"));
			this.fleetAltitude.SetText(Loc.T("UI.Space.AltitudeKM"));
			this.stationHeaderName.SetText(Loc.T("UI.Space.Stations.Header.Name"));
			this.stationHeaderAltitude.SetText(Loc.T("UI.Space.AltitudeKM"));
			this.stationHeaderControlPoints.SetText(Loc.T("UI.Space.Stations.Header.Sectors"));
			this.lagrange_stationHeaderName.SetText(Loc.T("UI.Space.Stations.Header.Name"));
			this.lagrange_stationHeaderAltitude.SetText(Loc.T("UI.Space.AltitudeKM"));
			this.lagrange_stationHeaderControlPoints.SetText(Loc.T("UI.Space.Stations.Header.Sectors"));
			this.fleetDVHeader.SetText(Loc.T("UI.Space.kps", new object[] { "" }).Trim());
			this.fleetSmallShipsHeader.SetText(Loc.T("UI.Space.FleetHeaderSmallShips"));
			this.fleetMediumShipsHeader.SetText(Loc.T("UI.Space.FleetHeaderMediumShips"));
			this.fleetLargeShipsHeader.SetText(Loc.T("UI.Space.FleetHeaderLargeShips"));
			this.moonsHeaderName.SetText(Loc.T("UI.Space.Moons.Header.Name"));
			this.moonsHeaderType.SetText(Loc.T("UI.Space.Moons.Header.Type"));
			this.moonsHeaderBases.SetText(Loc.T("UI.Space.Moons.Header.Bases"));
			this.councilorHeaderName.SetText(Loc.T("UI.Space.Councilors.Header.Name"));
			this.councilorHeaderLocation.SetText(Loc.T(string.Empty));
			this.lagrange_councilorHeaderName.SetText(Loc.T("UI.Space.Councilors.Header.Name"));
			this.lagrange_councilorHeaderLocation.SetText(string.Empty);
			this.natural_orbitHeaderName.SetText(Loc.T("UI.Space.Orbits.Header.Name"));
			this.natural_orbitHeaderAltitude.SetText(Loc.T("UI.Space.AltitudeKM"));
			this.lagrange_orbitHeaderName.SetText(Loc.T("UI.Space.Orbits.Header.Name"));
			this.lagrange_orbitHeaderAltitude.SetText(Loc.T("UI.Space.AltitudeKM"));
			this.habSectorHeaderName.SetText(Loc.T("UI.Space.Hab.Header.SectorName"));
			this.habCouncilorHeaderLocation.SetText(Loc.T("UI.Space.Councilors.Header.Location"));
			this.maxTierTip.SetDelegate("BodyText", delegate
			{
				string text = "UI.Space.MaxTier_SpaceBody";
				object[] array = new object[1];
				int num = 0;
				TIGameState tigameState = this.selectedSpaceObject;
				object obj;
				if (tigameState == null)
				{
					obj = null;
				}
				else
				{
					TINaturalSpaceObjectState ref_naturalSpaceObject = tigameState.ref_naturalSpaceObject;
					obj = ((ref_naturalSpaceObject != null) ? ref_naturalSpaceObject.maxHabTier.ToString() : null);
				}
				array[num] = obj;
				return Loc.T(text, array);
			});
			this.lp_maxTierTip.SetDelegate("BodyText", delegate
			{
				string text2 = "UI.Space.MaxTier_LagrangePoint";
				object[] array2 = new object[1];
				int num2 = 0;
				TIGameState tigameState2 = this.selectedSpaceObject;
				object obj2;
				if (tigameState2 == null)
				{
					obj2 = null;
				}
				else
				{
					TINaturalSpaceObjectState ref_naturalSpaceObject2 = tigameState2.ref_naturalSpaceObject;
					obj2 = ((ref_naturalSpaceObject2 != null) ? ref_naturalSpaceObject2.maxHabTier.ToString() : null);
				}
				array2[num2] = obj2;
				return Loc.T(text2, array2);
			});
			this.hab_fleetName.SetText(Loc.T("UI.Space.Fleet.Header.Name"));
			this.hab_fleetAltitudeHeader.SetText(Loc.T("UI.Space.Altitude"));
			this.hab_fleetDVHeader.SetText(Loc.T("UI.Space.kps", new object[] { "" }).Trim());
			this.hab_fleetSmallShipsHeader.SetText(Loc.T("UI.Space.FleetHeaderSmallShips"));
			this.hab_fleetMediumShipsHeader.SetText(Loc.T("UI.Space.FleetHeaderMediumShips"));
			this.hab_fleetLargeShipsHeader.SetText(Loc.T("UI.Space.FleetHeaderLargeShips"));
			this.lp_fleetName.SetText(Loc.T("UI.Space.Fleet.Header.Name"));
			this.lp_fleetAltitudeHeader.SetText(Loc.T("UI.Space.Altitude"));
			this.lp_fleetDVHeader.SetText(Loc.T("UI.Space.kps", new object[] { "" }).Trim());
			this.lp_fleetSmallShipsHeader.SetText(Loc.T("UI.Space.FleetHeaderSmallShips"));
			this.lp_fleetMediumShipsHeader.SetText(Loc.T("UI.Space.FleetHeaderMediumShips"));
			this.lp_fleetLargeShipsHeader.SetText(Loc.T("UI.Space.FleetHeaderLargeShips"));
			this.natural_orbitHeaderGs.SetText("BodyText", Loc.T("UI.Space.OrbitAccelGs"));
			this.lagrange_orbitHeaderGs.SetText("BodyText", Loc.T("UI.Space.OrbitAccelGs"));
			this.natural_amatHeader.SetText("BodyText", Loc.T("UI.Space.AMAT", new object[] { TemplateManager.global.antimatterInlineSpritePath }));
			this.assaultCombatTip.SetText("BodyText", Loc.T("UI.Habs.TwoAssaultValues"));
			GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathWaterIcon, this.waterIcon);
			GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathVolatilesIcon, this.volatilesIcon);
			GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathBaseMetalsIcon, this.metalsIcon);
			GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathNobleMetalsIcon, this.noblesIcon);
			GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathFissilesIcon, this.fissilesIcon);
			base.canvasManager.RegisterAssetPanelDisableOrder(AssetPanel.MyFleet, new Action(this.DisablePlayerFleetPanel));
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.HabDetail, new Action(this.DisableHabPanel));
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.LagrangeDetail, new Action(this.DisableLagrangePointPanel));
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.SpaceBodyDetail, new Action(this.DisableNaturalSpaceBodyPanel));
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.FleetDetail, new Action(this.DisableEnemyFleetPanel));
			this.lagrangeDescription.SetText(Loc.T("UI.Space.LagrangePoint"));
			this.lagrangeDescriptionLine2.SetText(Loc.T("UI.Space.LagrangePointDesc"));
			this.lagrangeOrbitTooltip.SetText("BodyText", Loc.T("UI.Space.SemimajorAxisTooltip", new object[]
			{
				string.Empty,
				string.Empty
			}));
			this.lagrange_orbitHeaderAMAT.SetText("BodyText", Loc.T("UI.Space.AMAT", new object[] { TemplateManager.global.antimatterInlineSpritePath }));
			this.myFleetShipsTabButtonText.SetText(Loc.T("UI.Space.Fleet.ShipDetailTab"));
			this.myFleetRefitButtonText.SetText(Loc.T("UI.Space.Fleet.RefitButton"));
			this.myFleetTransferSliderZeroPoint = this.myFleetTransferProgressLine.localPosition.x;
			this.myFleetTransferSliderRange = this.myFleetTransferProgressLine.sizeDelta.x - this.myFleetTransferSliderZeroPoint;
			this.gravityTip.SetText("BodyText", Loc.T("UI.Space.SurfaceGravityTooltip"));
			this.orbitTip.SetDelegate("BodyText", () => SpaceObjectDetailController.SemimajorAxisTooltip(this.selectedSpaceObject.ref_spaceBody));
			this.diameterTip.SetText("BodyText", Loc.T("UI.Space.DimensionsTooltip"));
			this.escapeVelocityTip.SetText("BodyText", Loc.T("UI.Space.OrbitalVelocityTooltip"));
			this.orbitPeriodTip.SetDelegate("BodyText", () => SpaceObjectDetailController.OrbitalPeriodTooltip(this.selectedSpaceObject.ref_spaceBody));
			this.enemyFleetShipsTabButtonText.SetText(Loc.T("UI.Space.Fleet.ShipDetailTab"));
			this.enemyFleetTransferSliderZeroPoint = this.enemyFleetTransferProgressLine.localPosition.x;
			this.enemyFleetTransferSliderRange = this.enemyFleetTransferProgressLine.sizeDelta.x - this.enemyFleetTransferSliderZeroPoint;
			this.alertFleetButtonTip.SetText("BodyText", Loc.T("UI.Alarm.FleetApproachingQuery"));
			this.shipListObject.SetActive(false);
			this.probeDataPanel.SetActive(false);
			this.populationPanel.SetActive(false);
			this.lp_populationPanel.SetActive(false);
			this.pusherTransform.sizeDelta = new Vector2(this.pusherTransform.sizeDelta.x, 465f);
			this.enemyFleetHeader.SetText(Loc.T("UI.Space.Fleet.Header"));
			this.myFleetHeader.SetText(Loc.T("UI.Space.Fleet.Header"));
			this.saveNameText.SetText(Loc.T("UI.Options.SaveName"));
			this.revertNameText.SetText(Loc.T("UI.Options.RevertName"));
			this.saveHabNameText.SetText(Loc.T("UI.Options.SaveName"));
			this.revertHabNameText.SetText(Loc.T("UI.Options.RevertName"));
			this.solarTip.SetDelegate("BodyText", delegate
			{
				TIGameState tigameState3 = this.selectedSpaceObject;
				if (tigameState3 == null)
				{
					return null;
				}
				TINaturalSpaceObjectState ref_naturalSpaceObject3 = tigameState3.ref_naturalSpaceObject;
				if (ref_naturalSpaceObject3 == null)
				{
					return null;
				}
				return ref_naturalSpaceObject3.SolarTip();
			});
			this.lagrangeSolarTip.SetDelegate("BodyText", delegate
			{
				TIGameState tigameState4 = this.selectedSpaceObject;
				if (tigameState4 == null)
				{
					return null;
				}
				TINaturalSpaceObjectState ref_naturalSpaceObject4 = tigameState4.ref_naturalSpaceObject;
				if (ref_naturalSpaceObject4 == null)
				{
					return null;
				}
				return ref_naturalSpaceObject4.SolarTip();
			});
			for (int i = 1; i <= 4; i++)
			{
				this.habSectorFactionImages[i].enabled = false;
			}
			this.DebugPanel.SetActive(false);
		}

		// Token: 0x060056B2 RID: 22194 RVA: 0x0027B518 File Offset: 0x00279718
		public override void Show()
		{
			GameControl.eventManager.AddListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null, null, true, false);
			base.Show();
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState != null && uiselectedAssetState.isSpaceFleetState)
			{
				this.UpdatePlayerFleetObjectCanvas(GeneralControlsController.UISelectedAssetState.ref_fleet);
			}
			this.mySpaceFleetPanel.gameObject.SetActive(true);
			this.enemySpaceFleetPanel.gameObject.SetActive(true);
			this.lagrangePointPanel.gameObject.SetActive(true);
			this.naturalBodyPanel.gameObject.SetActive(true);
			this.habPanel.gameObject.SetActive(true);
			this.Refresh();
		}

		// Token: 0x060056B3 RID: 22195 RVA: 0x0027B5C4 File Offset: 0x002797C4
		public override void Hide()
		{
			GameControl.eventManager.RemoveListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.OnInfoScreenOpened), null);
			this.naturalBodyPanel.gameObject.SetActive(false);
			this.enemySpaceFleetPanel.gameObject.SetActive(false);
			this.mySpaceFleetPanel.gameObject.SetActive(false);
			this.habPanel.gameObject.SetActive(false);
			this.lagrangePointPanel.gameObject.SetActive(false);
			if (this.modelInstance != null)
			{
				this.modelInstance.SetActive(false);
			}
			if (this.assetModelInstance != null)
			{
				this.assetModelInstance.SetActive(false);
			}
			if (this.selectionCameraInstance != null)
			{
				global::UnityEngine.Object.Destroy(this.selectionCameraInstance);
			}
			if (this.assetCameraInstance != null)
			{
				global::UnityEngine.Object.Destroy(this.assetCameraInstance);
			}
			base.Hide();
		}

		// Token: 0x060056B4 RID: 22196 RVA: 0x0027B6AC File Offset: 0x002798AC
		public override void Refresh()
		{
			if (this.mySpaceFleetPanel.enabled)
			{
				if (this.myFleetDataDirtyMinor || this.myFleetDataDirtyMajor)
				{
					this.UpdatePlayerFleetObjectCanvas(this.selectedAsset);
					if (this.myFleetDataDirtyMajor && this.mySpaceFleetPanel.enabled)
					{
						this.UpdateAssetPanelImage(this.selectedAsset, true);
					}
				}
				else if (this.selectedAsset.inTransfer && TIFrameCounter.FrameCount % 293 == 0)
				{
					this.UpdatePlayerFleetTransferData(this.selectedAsset);
				}
			}
			if (this.enemySpaceFleetPanel.enabled)
			{
				if (this.enemyFleetDataDirtyMinor || this.enemyFleetDataDirtyMajor)
				{
					this.UpdateEnemyFleetObjectCanvas(this.selectedSpaceObject.ref_fleet);
					if (this.enemyFleetDataDirtyMajor && this.enemySpaceFleetPanel.enabled)
					{
						this.UpdateInfoPanelImage(this.selectedSpaceObject.ref_fleet, true);
					}
				}
				else if (this.selectedSpaceObject.ref_fleet.inTransfer && TIFrameCounter.FrameCount % 293 == 0)
				{
					this.UpdateEnemyFleetTransferData(this.selectedSpaceObject.ref_fleet);
				}
			}
			else if (this.habPanel.enabled && (this.habDataDirtyMinor || this.habDataDirtyMajor))
			{
				this.UpdateHabCanvas(this.selectedSpaceObject.ref_hab);
				if (this.habDataDirtyMajor && this.habPanel.enabled)
				{
					this.UpdateInfoPanelImage(this.selectedSpaceObject.ref_hab, true);
				}
			}
			else if (this.naturalBodyPanel.enabled)
			{
				if (this.spaceBodyDataDirtyMajor)
				{
					this.UpdateNaturalSpaceBodyCanvas(this.selectedSpaceObject.ref_spaceBody);
				}
				else if (this.spaceBodyDataDirtyMinor)
				{
					this.UpdateNaturalSpaceBodyCanvasTransientData(this.selectedSpaceObject.ref_spaceBody);
				}
				else if (TIFrameCounter.FrameCount % 1321 == 0)
				{
					this.UpdateNaturalSpaceObjectLaunchWindowData(this.selectedSpaceObject.ref_spaceBody, this.naturalSpaceBodyLaunchWindow);
				}
			}
			else if (this.lagrangePointPanel.enabled)
			{
				if (this.lagrangeDataDirtyMinor)
				{
					this.UpdateLagrangePointCanvasTransientData(this.selectedSpaceObject.ref_lagrangePoint);
				}
				else if (TIFrameCounter.FrameCount % 1321 == 0)
				{
					this.UpdateNaturalSpaceObjectLaunchWindowData(this.selectedSpaceObject.ref_lagrangePoint, this.lagrangePointNextLaunchWindow);
				}
			}
			this.UpdateSpaceObjectBackButtons();
			this.myFleetDataDirtyMajor = false;
			this.myFleetDataDirtyMinor = false;
			this.enemyFleetDataDirtyMinor = false;
			this.enemyFleetDataDirtyMajor = false;
			this.habDataDirtyMajor = false;
			this.habDataDirtyMinor = false;
			this.spaceBodyDataDirtyMajor = false;
			this.spaceBodyDataDirtyMinor = false;
			this.lagrangeDataDirtyMinor = false;
		}

		// Token: 0x060056B5 RID: 22197 RVA: 0x0027B927 File Offset: 0x00279B27
		private void OnInfoScreenOpened(InfoScreenOpened e)
		{
			if (this.Visible())
			{
				this.Hide();
				GameControl.eventManager.AddListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreSpaceObjectDetailCanvas), null, null, true, false);
			}
		}

		// Token: 0x060056B6 RID: 22198 RVA: 0x0027B954 File Offset: 0x00279B54
		private void RestoreSpaceObjectDetailCanvas(InfoScreenClosed e)
		{
			this.Show();
			TISpaceObjectState tispaceObjectState = GeneralControlsController.UIOtherSelectedState as TISpaceObjectState;
			if (TIGameState.Valid(tispaceObjectState) && (tispaceObjectState.ref_hab != null || tispaceObjectState.ref_fleet != null || tispaceObjectState.ref_spaceBody != null) && (this.habPanel.enabled || this.naturalBodyPanel.enabled || this.enemySpaceFleetPanel.enabled))
			{
				this.UpdateInfoPanelImage(tispaceObjectState, true);
			}
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState != null && uiselectedAssetState.isSpaceFleetState && this.mySpaceFleetPanel.enabled)
			{
				this.UpdateAssetPanelImage(GeneralControlsController.UISelectedAssetState.ref_fleet, true);
			}
			GameControl.eventManager.RemoveListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreSpaceObjectDetailCanvas), null);
		}

		// Token: 0x060056B7 RID: 22199 RVA: 0x0027BA1C File Offset: 0x00279C1C
		private void OnGameStateArchived(GameStateArchived e)
		{
			if (e.gameState == this.selectedSpaceObject)
			{
				base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
				return;
			}
			if (e.gameState == this.selectedAsset)
			{
				base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
				return;
			}
			TIGameState tigameState = e.gameState as TISpaceAssetState;
			TICouncilorState ticouncilorState = e.gameState as TICouncilorState;
			if (tigameState != null || (ticouncilorState != null && (ticouncilorState.faction != null || ticouncilorState.status == CouncilorStatus.Dead)))
			{
				this.SetAllDirty();
			}
		}

		// Token: 0x060056B8 RID: 22200 RVA: 0x0027BAB8 File Offset: 0x00279CB8
		private void SetAllDirty()
		{
			if (this.mySpaceFleetPanel.enabled)
			{
				this.myFleetDataDirtyMajor = true;
				this.myFleetDataDirtyMinor = true;
			}
			if (this.enemySpaceFleetPanel.enabled)
			{
				this.enemyFleetDataDirtyMinor = true;
				this.enemyFleetDataDirtyMajor = true;
				return;
			}
			if (this.habPanel.enabled)
			{
				this.habDataDirtyMajor = true;
				this.habDataDirtyMinor = true;
				return;
			}
			if (this.naturalBodyPanel.enabled)
			{
				this.spaceBodyDataDirtyMajor = true;
				this.spaceBodyDataDirtyMinor = true;
				return;
			}
			if (this.lagrangePointPanel.enabled)
			{
				this.lagrangeDataDirtyMinor = true;
			}
		}

		// Token: 0x060056B9 RID: 22201 RVA: 0x0027BB48 File Offset: 0x00279D48
		private void OnMajorMyFleetUpdate(ShipsAddedToFleet e)
		{
			this.myFleetDataDirtyMajor = true;
		}

		// Token: 0x060056BA RID: 22202 RVA: 0x0027BB51 File Offset: 0x00279D51
		private void OnMajorMyFleetUpdate(ShipsRemovedFromFleet e)
		{
			this.myFleetDataDirtyMajor = true;
		}

		// Token: 0x060056BB RID: 22203 RVA: 0x0027BB5A File Offset: 0x00279D5A
		private void OnMajorMyFleetUpdate(CombatEnds e)
		{
			this.myFleetDataDirtyMajor = true;
		}

		// Token: 0x060056BC RID: 22204 RVA: 0x0027BB63 File Offset: 0x00279D63
		private void OnMyFleetUpdate(StartFleetOperation e)
		{
			this.myFleetDataDirtyMajor = true;
		}

		// Token: 0x060056BD RID: 22205 RVA: 0x0027BB6C File Offset: 0x00279D6C
		private void OnMyFleetUpdate(OperationExecuted e)
		{
			this.myFleetDataDirtyMinor = true;
		}

		// Token: 0x060056BE RID: 22206 RVA: 0x0027BB75 File Offset: 0x00279D75
		private void OnMyFleetUpdate(FleetArrivesAtDestination e)
		{
			if (e.planetarySurfaceChange)
			{
				this.myFleetDataDirtyMajor = true;
				return;
			}
			this.myFleetDataDirtyMinor = true;
		}

		// Token: 0x060056BF RID: 22207 RVA: 0x0027BB8E File Offset: 0x00279D8E
		private void OnMyFleetUpdate(CouncilorDepartsShip e)
		{
			this.myFleetDataDirtyMinor = true;
		}

		// Token: 0x060056C0 RID: 22208 RVA: 0x0027BB97 File Offset: 0x00279D97
		private void OnMyFleetUpdate(CouncilorVisibilityChanged e)
		{
			this.myFleetDataDirtyMinor = true;
		}

		// Token: 0x060056C1 RID: 22209 RVA: 0x0027BBA0 File Offset: 0x00279DA0
		private void OnMyFleetUpdate(CouncilorPositionUpdated e)
		{
			this.myFleetDataDirtyMinor = true;
		}

		// Token: 0x060056C2 RID: 22210 RVA: 0x0027BBA9 File Offset: 0x00279DA9
		private void OnMyFleetUpdate(ShipResupplied e)
		{
			this.myFleetDataDirtyMinor = true;
		}

		// Token: 0x060056C3 RID: 22211 RVA: 0x0027BBB2 File Offset: 0x00279DB2
		private void OnMyFleetUpdate(FleetUndocks e)
		{
			this.myFleetDataDirtyMinor = true;
		}

		// Token: 0x060056C4 RID: 22212 RVA: 0x0027BBBB File Offset: 0x00279DBB
		private void OnMyFleetUpdate(FleetAvailabilityChange e)
		{
			this.myFleetDataDirtyMinor = true;
		}

		// Token: 0x060056C5 RID: 22213 RVA: 0x0027BBC4 File Offset: 0x00279DC4
		private void OnMyFleetUpdate(FleetOperationWithDurationComplete e)
		{
			this.myFleetDataDirtyMinor = true;
		}

		// Token: 0x060056C6 RID: 22214 RVA: 0x0027BBCD File Offset: 0x00279DCD
		private void OnMyFleetUpdate(ShipSystemDamageChange e)
		{
			if (TISpaceShipState.visiblyDamagedSystems.Contains(e.system))
			{
				this.myFleetDataDirtyMinor = true;
			}
		}

		// Token: 0x060056C7 RID: 22215 RVA: 0x0027BBE8 File Offset: 0x00279DE8
		public void AddMyFleetListeners()
		{
			GameControl.eventManager.AddListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.OnMajorMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnMajorMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.OnMajorMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<StartFleetOperation>(new EventManager.EventDelegate<StartFleetOperation>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<CouncilorDepartsShip>(new EventManager.EventDelegate<CouncilorDepartsShip>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<ShipResupplied>(new EventManager.EventDelegate<ShipResupplied>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<FleetAvailabilityChange>(new EventManager.EventDelegate<FleetAvailabilityChange>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
			GameControl.eventManager.AddListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null, this.selectedAsset, false, true);
			GameControl.eventManager.AddListener<FleetOperationWithDurationComplete>(new EventManager.EventDelegate<FleetOperationWithDurationComplete>(this.OnMyFleetUpdate), null, this.selectedAsset, false, false);
			GameControl.eventManager.AddListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.OnMyFleetUpdate), null, this.selectedAsset, true, false);
		}

		// Token: 0x060056C8 RID: 22216 RVA: 0x0027BDC8 File Offset: 0x00279FC8
		public void RemoveMyFleetListeners()
		{
			GameControl.eventManager.RemoveListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.OnMajorMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnMajorMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.OnMajorMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<StartFleetOperation>(new EventManager.EventDelegate<StartFleetOperation>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorDepartsShip>(new EventManager.EventDelegate<CouncilorDepartsShip>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<ShipResupplied>(new EventManager.EventDelegate<ShipResupplied>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<FleetAvailabilityChange>(new EventManager.EventDelegate<FleetAvailabilityChange>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null);
			GameControl.eventManager.RemoveListener<FleetOperationWithDurationComplete>(new EventManager.EventDelegate<FleetOperationWithDurationComplete>(this.OnMyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.OnMyFleetUpdate), null);
		}

		// Token: 0x060056C9 RID: 22217 RVA: 0x0027BF2E File Offset: 0x0027A12E
		private void OnMajorEnemyFleetUpdate(ShipsAddedToFleet e)
		{
			this.enemyFleetDataDirtyMajor = true;
		}

		// Token: 0x060056CA RID: 22218 RVA: 0x0027BF37 File Offset: 0x0027A137
		private void OnMajorEnemyFleetUpdate(ShipsRemovedFromFleet e)
		{
			this.enemyFleetDataDirtyMajor = true;
		}

		// Token: 0x060056CB RID: 22219 RVA: 0x0027BF40 File Offset: 0x0027A140
		private void OnMajorEnemyFleetUpdate(CombatEnds e)
		{
			this.enemyFleetDataDirtyMajor = true;
		}

		// Token: 0x060056CC RID: 22220 RVA: 0x0027BF49 File Offset: 0x0027A149
		private void OnEnemyFleetUpdate(StartFleetOperation e)
		{
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056CD RID: 22221 RVA: 0x0027BF52 File Offset: 0x0027A152
		private void OnEnemyFleetUpdate(OperationExecuted e)
		{
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056CE RID: 22222 RVA: 0x0027BF5B File Offset: 0x0027A15B
		private void OnEnemyFleetUpdate(FleetArrivesAtDestination e)
		{
			if (e.planetarySurfaceChange)
			{
				this.enemyFleetDataDirtyMajor = true;
				return;
			}
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056CF RID: 22223 RVA: 0x0027BF74 File Offset: 0x0027A174
		private void OnEnemyFleetUpdate(CouncilorDepartsShip e)
		{
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056D0 RID: 22224 RVA: 0x0027BF7D File Offset: 0x0027A17D
		private void OnEnemyFleetUpdate(CouncilorVisibilityChanged e)
		{
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056D1 RID: 22225 RVA: 0x0027BF86 File Offset: 0x0027A186
		private void OnEnemyFleetUpdate(CouncilorPositionUpdated e)
		{
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056D2 RID: 22226 RVA: 0x0027BF8F File Offset: 0x0027A18F
		private void OnEnemyFleetUpdate(ShipResupplied e)
		{
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056D3 RID: 22227 RVA: 0x0027BF98 File Offset: 0x0027A198
		private void OnEnemyFleetUpdate(FleetUndocks e)
		{
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056D4 RID: 22228 RVA: 0x0027BFA1 File Offset: 0x0027A1A1
		private void OnEnemyFleetUpdate(FleetAvailabilityChange e)
		{
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056D5 RID: 22229 RVA: 0x0027BFAA File Offset: 0x0027A1AA
		private void OnEnemyFleetUpdate(FleetOperationWithDurationComplete e)
		{
			this.enemyFleetDataDirtyMinor = true;
		}

		// Token: 0x060056D6 RID: 22230 RVA: 0x0027BFB3 File Offset: 0x0027A1B3
		private void OnEnemyFleetShipDamaged(ShipSystemDamageChange e)
		{
			if (TISpaceShipState.visiblyDamagedSystems.Contains(e.system))
			{
				this.myFleetDataDirtyMinor = true;
			}
		}

		// Token: 0x060056D7 RID: 22231 RVA: 0x0027BFD0 File Offset: 0x0027A1D0
		private void AddEnemyFleetListeners()
		{
			GameControl.eventManager.AddListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.OnMajorEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnMajorEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.OnMajorEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<StartFleetOperation>(new EventManager.EventDelegate<StartFleetOperation>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorDepartsShip>(new EventManager.EventDelegate<CouncilorDepartsShip>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<ShipResupplied>(new EventManager.EventDelegate<ShipResupplied>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<FleetAvailabilityChange>(new EventManager.EventDelegate<FleetAvailabilityChange>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null, this.selectedSpaceObject, false, true);
			GameControl.eventManager.AddListener<FleetOperationWithDurationComplete>(new EventManager.EventDelegate<FleetOperationWithDurationComplete>(this.OnEnemyFleetUpdate), null, this.selectedSpaceObject, false, false);
			GameControl.eventManager.AddListener<AlarmTriggered>(new EventManager.EventDelegate<AlarmTriggered>(this.OnFleetAlarmTriggered), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.OnEnemyFleetShipDamaged), null, this.selectedAsset, true, false);
		}

		// Token: 0x060056D8 RID: 22232 RVA: 0x0027C1D0 File Offset: 0x0027A3D0
		private void RemoveEnemyFleetListeners()
		{
			GameControl.eventManager.RemoveListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.OnMajorEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnMajorEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.OnMajorEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<StartFleetOperation>(new EventManager.EventDelegate<StartFleetOperation>(this.OnEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.OnEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorDepartsShip>(new EventManager.EventDelegate<CouncilorDepartsShip>(this.OnEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<ShipResupplied>(new EventManager.EventDelegate<ShipResupplied>(this.OnEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null);
			GameControl.eventManager.RemoveListener<FleetOperationWithDurationComplete>(new EventManager.EventDelegate<FleetOperationWithDurationComplete>(this.OnEnemyFleetUpdate), null);
			GameControl.eventManager.RemoveListener<AlarmTriggered>(new EventManager.EventDelegate<AlarmTriggered>(this.OnFleetAlarmTriggered), null);
			GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.OnEnemyFleetShipDamaged), null);
		}

		// Token: 0x060056D9 RID: 22233 RVA: 0x0027C336 File Offset: 0x0027A536
		private void OnSpaceBodyUpdate(HabCreated e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056DA RID: 22234 RVA: 0x0027C33F File Offset: 0x0027A53F
		private void OnSpaceBodyUpdate(HabDestroyed e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056DB RID: 22235 RVA: 0x0027C348 File Offset: 0x0027A548
		private void OnSpaceBodyUpdate(HabModuleConstructionStatusChange e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056DC RID: 22236 RVA: 0x0027C351 File Offset: 0x0027A551
		private void OnSpaceBodyUpdate(SectorAssignedToFaction e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056DD RID: 22237 RVA: 0x0027C35A File Offset: 0x0027A55A
		private void OnSpaceBodyUpdate(HabModuleDestroyed e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056DE RID: 22238 RVA: 0x0027C363 File Offset: 0x0027A563
		private void OnSpaceBodyUpdate(CouncilorVisibilityChanged e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056DF RID: 22239 RVA: 0x0027C36C File Offset: 0x0027A56C
		private void OnSpaceBodyUpdate(CouncilorPositionUpdated e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056E0 RID: 22240 RVA: 0x0027C375 File Offset: 0x0027A575
		private void OnSpaceBodyUpdate(ProspectingBody e)
		{
			this.spaceBodyDataDirtyMajor = true;
		}

		// Token: 0x060056E1 RID: 22241 RVA: 0x0027C37E File Offset: 0x0027A57E
		private void OnSpaceBodyUpdate(SpaceBodyProspected e)
		{
			this.spaceBodyDataDirtyMajor = true;
		}

		// Token: 0x060056E2 RID: 22242 RVA: 0x0027C387 File Offset: 0x0027A587
		private void OnSpaceBodyUpdate(FleetArrivesAtDestination e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056E3 RID: 22243 RVA: 0x0027C390 File Offset: 0x0027A590
		private void OnSpaceBodyUpdate(ShipsAddedToFleet e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056E4 RID: 22244 RVA: 0x0027C399 File Offset: 0x0027A599
		private void OnSpaceBodyUpdate(ShipsRemovedFromFleet e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056E5 RID: 22245 RVA: 0x0027C3A2 File Offset: 0x0027A5A2
		private void OnSpaceBodyUpdate(FleetUndocks e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056E6 RID: 22246 RVA: 0x0027C3AB File Offset: 0x0027A5AB
		private void OnSpaceBodyUpdate(SpaceBodyTagChanged e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056E7 RID: 22247 RVA: 0x0027C3B4 File Offset: 0x0027A5B4
		private void OnSpaceBodyUpdate(BeginBombardment e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056E8 RID: 22248 RVA: 0x0027C3BD File Offset: 0x0027A5BD
		private void OnSpaceBodyUpdate(EndBombardment e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056E9 RID: 22249 RVA: 0x0027C3C6 File Offset: 0x0027A5C6
		private void OnSpaceBodyUpdate(BeginHabAssault e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056EA RID: 22250 RVA: 0x0027C3CF File Offset: 0x0027A5CF
		private void OnSpaceBodyUpdate(EndHabAssault e)
		{
			this.spaceBodyDataDirtyMinor = true;
		}

		// Token: 0x060056EB RID: 22251 RVA: 0x0027C3D8 File Offset: 0x0027A5D8
		private void AddNaturalSpaceBodyListeners()
		{
			GameControl.eventManager.AddListener<HabCreated>(new EventManager.EventDelegate<HabCreated>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<ProspectingBody>(new EventManager.EventDelegate<ProspectingBody>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<SpaceBodyProspected>(new EventManager.EventDelegate<SpaceBodyProspected>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<SpaceBodyTagChanged>(new EventManager.EventDelegate<SpaceBodyTagChanged>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.OnSpaceBodyUpdate), null, this.selectedSpaceObject, true, false);
		}

		// Token: 0x060056EC RID: 22252 RVA: 0x0027C614 File Offset: 0x0027A814
		private void RemoveNaturalSpaceBodyListeners()
		{
			GameControl.eventManager.RemoveListener<HabCreated>(new EventManager.EventDelegate<HabCreated>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<ProspectingBody>(new EventManager.EventDelegate<ProspectingBody>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<SpaceBodyProspected>(new EventManager.EventDelegate<SpaceBodyProspected>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<SpaceBodyTagChanged>(new EventManager.EventDelegate<SpaceBodyTagChanged>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.OnSpaceBodyUpdate), null);
			GameControl.eventManager.RemoveListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.OnSpaceBodyUpdate), null);
		}

		// Token: 0x060056ED RID: 22253 RVA: 0x0027C7BF File Offset: 0x0027A9BF
		private void OnHabStructureUpdate(HabModuleConstructionStatusChange e)
		{
			this.habDataDirtyMajor = true;
		}

		// Token: 0x060056EE RID: 22254 RVA: 0x0027C7C8 File Offset: 0x0027A9C8
		private void OnHabStructureUpdate(HabModuleDestroyed e)
		{
			this.habDataDirtyMajor = true;
		}

		// Token: 0x060056EF RID: 22255 RVA: 0x0027C7D1 File Offset: 0x0027A9D1
		private void OnHabStructureUpdate(HabDestroyed e)
		{
			this.habDataDirtyMajor = true;
		}

		// Token: 0x060056F0 RID: 22256 RVA: 0x0027C7DA File Offset: 0x0027A9DA
		private void OnHabUpdate(FleetArrivesAtDestination e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056F1 RID: 22257 RVA: 0x0027C7E3 File Offset: 0x0027A9E3
		private void OnHabUpdate(FleetUndocks e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056F2 RID: 22258 RVA: 0x0027C7EC File Offset: 0x0027A9EC
		private void OnHabUpdate(FleetDisbanded e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056F3 RID: 22259 RVA: 0x0027C7F5 File Offset: 0x0027A9F5
		private void OnHabUpdate(SectorAssignedToFaction e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056F4 RID: 22260 RVA: 0x0027C7FE File Offset: 0x0027A9FE
		private void OnHabUpdate(CouncilorDepartsHab e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056F5 RID: 22261 RVA: 0x0027C807 File Offset: 0x0027AA07
		private void OnHabUpdate(CouncilorVisibilityChanged e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056F6 RID: 22262 RVA: 0x0027C810 File Offset: 0x0027AA10
		private void OnHabUpdate(CouncilorPositionUpdated e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056F7 RID: 22263 RVA: 0x0027C819 File Offset: 0x0027AA19
		private void OnHabUpdate(HabDefendInterestsUpdated e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056F8 RID: 22264 RVA: 0x0027C822 File Offset: 0x0027AA22
		private void OnHabUpdate(BeginBombardment e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056F9 RID: 22265 RVA: 0x0027C82B File Offset: 0x0027AA2B
		private void OnHabUpdate(EndBombardment e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056FA RID: 22266 RVA: 0x0027C834 File Offset: 0x0027AA34
		private void OnHabUpdate(BeginHabAssault e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056FB RID: 22267 RVA: 0x0027C83D File Offset: 0x0027AA3D
		private void OnHabUpdate(EndHabAssault e)
		{
			this.habDataDirtyMinor = true;
		}

		// Token: 0x060056FC RID: 22268 RVA: 0x0027C848 File Offset: 0x0027AA48
		private void AddHabListeners()
		{
			GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<FleetDisbanded>(new EventManager.EventDelegate<FleetDisbanded>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnHabStructureUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnHabStructureUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorDepartsHab>(new EventManager.EventDelegate<CouncilorDepartsHab>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabStructureUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null, this.selectedSpaceObject, false, true);
			GameControl.eventManager.AddListener<HabDefendInterestsUpdated>(new EventManager.EventDelegate<HabDefendInterestsUpdated>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.OnHabUpdate), null, this.selectedSpaceObject, true, false);
		}

		// Token: 0x060056FD RID: 22269 RVA: 0x0027CA48 File Offset: 0x0027AC48
		private void RemoveHabListeners()
		{
			GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<FleetDisbanded>(new EventManager.EventDelegate<FleetDisbanded>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnHabStructureUpdate), null);
			GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnHabStructureUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorDepartsHab>(new EventManager.EventDelegate<CouncilorDepartsHab>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabStructureUpdate), null);
			GameControl.eventManager.RemoveListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null);
			GameControl.eventManager.RemoveListener<HabDefendInterestsUpdated>(new EventManager.EventDelegate<HabDefendInterestsUpdated>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.OnHabUpdate), null);
			GameControl.eventManager.RemoveListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.OnHabUpdate), null);
		}

		// Token: 0x060056FE RID: 22270 RVA: 0x0027CBC5 File Offset: 0x0027ADC5
		private void OnLagrangePointUpdate(HabCreated e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x060056FF RID: 22271 RVA: 0x0027CBCE File Offset: 0x0027ADCE
		private void OnLagrangePointUpdate(HabDestroyed e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005700 RID: 22272 RVA: 0x0027CBD7 File Offset: 0x0027ADD7
		private void OnLagrangePointUpdate(HabModuleConstructionStatusChange e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005701 RID: 22273 RVA: 0x0027CBE0 File Offset: 0x0027ADE0
		private void OnLagrangePointUpdate(SectorAssignedToFaction e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005702 RID: 22274 RVA: 0x0027CBE9 File Offset: 0x0027ADE9
		private void OnLagrangePointUpdate(HabModuleDestroyed e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005703 RID: 22275 RVA: 0x0027CBF2 File Offset: 0x0027ADF2
		private void OnLagrangePointUpdate(CouncilorVisibilityChanged e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005704 RID: 22276 RVA: 0x0027CBFB File Offset: 0x0027ADFB
		private void OnLagrangePointUpdate(CouncilorPositionUpdated e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005705 RID: 22277 RVA: 0x0027CC04 File Offset: 0x0027AE04
		private void OnLagrangePointUpdate(FleetArrivesAtDestination e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005706 RID: 22278 RVA: 0x0027CC0D File Offset: 0x0027AE0D
		private void OnLagrangePointUpdate(ShipsAddedToFleet e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005707 RID: 22279 RVA: 0x0027CC16 File Offset: 0x0027AE16
		private void OnLagrangePointUpdate(ShipsRemovedFromFleet e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005708 RID: 22280 RVA: 0x0027CC1F File Offset: 0x0027AE1F
		private void OnLagrangePointUpdate(FleetUndocks e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x06005709 RID: 22281 RVA: 0x0027CC28 File Offset: 0x0027AE28
		private void OnLagrangePointUpdate(BeginHabAssault e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x0600570A RID: 22282 RVA: 0x0027CC31 File Offset: 0x0027AE31
		private void OnLagrangePointUpdate(EndHabAssault e)
		{
			this.lagrangeDataDirtyMinor = true;
		}

		// Token: 0x0600570B RID: 22283 RVA: 0x0027CC3C File Offset: 0x0027AE3C
		private void AddLagrangePointListeners()
		{
			GameControl.eventManager.AddListener<HabCreated>(new EventManager.EventDelegate<HabCreated>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
			GameControl.eventManager.AddListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.OnLagrangePointUpdate), null, this.selectedSpaceObject, true, false);
		}

		// Token: 0x0600570C RID: 22284 RVA: 0x0027CDDC File Offset: 0x0027AFDC
		private void RemoveLagrangePointListeners()
		{
			GameControl.eventManager.RemoveListener<HabCreated>(new EventManager.EventDelegate<HabCreated>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.OnLagrangePointUpdate), null);
			GameControl.eventManager.RemoveListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.OnLagrangePointUpdate), null);
		}

		// Token: 0x0600570D RID: 22285 RVA: 0x0027CF14 File Offset: 0x0027B114
		public void NaturalBodyExitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x0600570E RID: 22286 RVA: 0x0027CF33 File Offset: 0x0027B133
		public void EnemyFleetExitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x0600570F RID: 22287 RVA: 0x0027CF52 File Offset: 0x0027B152
		public void HabExitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x06005710 RID: 22288 RVA: 0x0027CF71 File Offset: 0x0027B171
		public void LagrangePointExitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x06005711 RID: 22289 RVA: 0x0027CF90 File Offset: 0x0027B190
		public void MyFleetExitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
		}

		// Token: 0x06005712 RID: 22290 RVA: 0x0027CFAF File Offset: 0x0027B1AF
		public void GotoOtherButtonClicked()
		{
			SoundEffectController.PlaySelectSound(this.UIOtherSelectedState);
			TIUtilities.GotoGameState(this.UIOtherSelectedState, true, true, true, true, false, -1f);
		}

		// Token: 0x06005713 RID: 22291 RVA: 0x0027CFD1 File Offset: 0x0027B1D1
		public void GotoMyButtonClicked()
		{
			SoundEffectController.PlaySelectSound(this.UISelectedAssetState);
			TIUtilities.GotoGameState(this.UISelectedAssetState, true, true, true, true, false, -1f);
		}

		// Token: 0x06005714 RID: 22292 RVA: 0x0027CFF4 File Offset: 0x0027B1F4
		public void GotoParentButtonClicked()
		{
			if (this.habPanel.enabled && this.selectedSpaceObject.ref_hab.IsBase)
			{
				SoundEffectController.PlaySelectSound(this.UIOtherSelectedState.ref_spaceBody);
				TIUtilities.GotoGameState(this.UIOtherSelectedState.ref_spaceBody, true, true, true, true, false, -1f);
				return;
			}
			SoundEffectController.PlaySelectSound(this.UIOtherSelectedState.ref_spaceObject.barycenter);
			TIUtilities.GotoGameState(this.UIOtherSelectedState.ref_spaceObject.barycenter, true, true, true, true, false, -1f);
		}

		// Token: 0x06005715 RID: 22293 RVA: 0x0027D07F File Offset: 0x0027B27F
		public void GotoSecondaryButtonClicked()
		{
			SoundEffectController.PlaySelectSound(this.UIOtherSelectedState.ref_lagrangePoint.secondaryObject);
			TIUtilities.GotoGameState(this.UIOtherSelectedState.ref_lagrangePoint.secondaryObject, true, true, true, true, false, -1f);
		}

		// Token: 0x17000F19 RID: 3865
		// (get) Token: 0x06005716 RID: 22294 RVA: 0x0027D0B5 File Offset: 0x0027B2B5
		private TIGameState UIOtherSelectedState
		{
			get
			{
				return GeneralControlsController.UIOtherSelectedState;
			}
		}

		// Token: 0x17000F1A RID: 3866
		// (get) Token: 0x06005717 RID: 22295 RVA: 0x0027D0BC File Offset: 0x0027B2BC
		private TIGameState UISelectedAssetState
		{
			get
			{
				return GeneralControlsController.UISelectedAssetState;
			}
		}

		// Token: 0x06005718 RID: 22296 RVA: 0x0027D0C4 File Offset: 0x0027B2C4
		private void CheckForCloseCanvas()
		{
			if (this.mySpaceFleetPanel != null && this.enemySpaceFleetPanel != null && this.naturalBodyPanel != null && this.habPanel != null && this.lagrangePointPanel != null && !this.mySpaceFleetPanel.enabled && !this.enemySpaceFleetPanel.enabled && !this.naturalBodyPanel.enabled && !this.habPanel.enabled && !this.lagrangePointPanel.enabled)
			{
				this.Hide();
			}
		}

		// Token: 0x06005719 RID: 22297 RVA: 0x0027D160 File Offset: 0x0027B360
		public void ViewSpaceObject(TISpaceObjectState spaceObject, bool updatePrevious = true)
		{
			if (!spaceObject.isSun)
			{
				if (!this.Visible())
				{
					this.Show();
				}
				GeneralControlsController.SetSelectedState(spaceObject, true);
				if (updatePrevious)
				{
					this.AddClickedPreviousSpacebody(spaceObject);
				}
				switch (spaceObject.objectType)
				{
				case SpaceObjectType.Planet:
				case SpaceObjectType.DwarfPlanet:
				case SpaceObjectType.Asteroid:
				case SpaceObjectType.Comet:
				case SpaceObjectType.PlanetaryMoon:
				case SpaceObjectType.AsteroidalMoon:
					this.LaunchNaturalSpaceBodyDetail(spaceObject.ref_spaceBody);
					return;
				case SpaceObjectType.Fleet:
				{
					TISpaceFleetState tispaceFleetState = spaceObject as TISpaceFleetState;
					if (!(tispaceFleetState.faction == base.activePlayer))
					{
						this.LaunchDetailEnemyFleet(tispaceFleetState);
						return;
					}
					bool flag;
					if (GeneralControlsController.UIPlayerInTargetingMode)
					{
						if (!GeneralControlsController.CurrentValidTarget(tispaceFleetState))
						{
							flag = tispaceFleetState.ships.Any<TISpaceShipState>((TISpaceShipState x) => GeneralControlsController.CurrentValidTarget(x));
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						flag = false;
					}
					if (!flag)
					{
						this.LaunchDetailPlayerFleet(tispaceFleetState);
						return;
					}
					break;
				}
				case SpaceObjectType.Hab:
					this.LaunchDetailHab(spaceObject.ref_hab);
					return;
				case SpaceObjectType.LagrangePoint:
					this.LaunchDetailLagrangePoint(spaceObject.ref_lagrangePoint);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x0600571A RID: 22298 RVA: 0x0027D25C File Offset: 0x0027B45C
		public void ViewSpaceObject(GameObject selectedObject)
		{
			if (selectedObject != null)
			{
				SpaceObjectController component = selectedObject.GetComponent<SpaceObjectController>();
				if (component != null && component.spaceObjectState != null && component.spaceObjectState.objectType != SpaceObjectType.Star)
				{
					this.ViewSpaceObject(component.spaceObjectState, true);
				}
			}
		}

		// Token: 0x0600571B RID: 22299 RVA: 0x0027D2AC File Offset: 0x0027B4AC
		private void LaunchDetailPlayerFleet(TISpaceFleetState fleet)
		{
			if (this.mySpaceFleetPanel != null && !this.mySpaceFleetPanel.enabled)
			{
				this.mySpaceFleetPanel.enabled = true;
				base.canvasManager.SetActiveAssetPanel(AssetPanel.MyFleet, this.upperPanelTransform.sizeDelta.y + (this.playerFleetShipListOpen ? this.shipListTransform.sizeDelta.y : 0f));
			}
			else
			{
				this.RemoveMyFleetListeners();
			}
			this.selectedAsset = fleet;
			this.RevertRename();
			this.AddMyFleetListeners();
			this.UpdatePlayerFleetObjectCanvas(fleet);
			this.UpdateAssetPanelImage(fleet, false);
		}

		// Token: 0x0600571C RID: 22300 RVA: 0x0027D348 File Offset: 0x0027B548
		private void DisablePlayerFleetPanel()
		{
			if (this.mySpaceFleetPanel != null)
			{
				this.mySpaceFleetPanel.enabled = false;
				this.HideTutorials();
			}
			GeneralControlsController.ConditionalCancelSelectedOtherState(this.selectedAsset);
			this.selectedAsset = null;
			this.assetPanelImageState = null;
			this.RevertRename();
			this.RemoveMyFleetListeners();
			this.CheckForCloseCanvas();
		}

		// Token: 0x0600571D RID: 22301 RVA: 0x0027D3A0 File Offset: 0x0027B5A0
		private void LaunchDetailEnemyFleet(TISpaceFleetState fleet)
		{
			if (this.enemySpaceFleetPanel != null && !this.enemySpaceFleetPanel.enabled)
			{
				this.enemySpaceFleetPanel.enabled = true;
				this.enemyFleetUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_SpaceObjectDetailCanvas_EnemyFleet, false, true);
				base.canvasManager.SetActiveInfoPanel(InfoPanel.FleetDetail, 0f);
			}
			else
			{
				this.RemoveEnemyFleetListeners();
			}
			this.selectedSpaceObject = fleet;
			this.AddEnemyFleetListeners();
			this.UpdateEnemyFleetObjectCanvas(fleet);
			this.UpdateInfoPanelImage(fleet, false);
		}

		// Token: 0x0600571E RID: 22302 RVA: 0x0027D41C File Offset: 0x0027B61C
		private void DisableEnemyFleetPanel()
		{
			this.enemySpaceFleetPanel.enabled = false;
			this.HideTutorials();
			GeneralControlsController.ConditionalCancelSelectedOtherState(this.selectedSpaceObject);
			TISpaceFleetState ref_fleet = this.selectedSpaceObject.ref_fleet;
			TISpaceGameState tispaceGameState;
			if (ref_fleet != null && ref_fleet.dockedAtStation)
			{
				TISpaceFleetState ref_fleet2 = this.selectedSpaceObject.ref_fleet;
				tispaceGameState = ((ref_fleet2 != null) ? ref_fleet2.dockedLocation : null);
			}
			else
			{
				TISpaceObjectState ref_spaceObject = this.selectedSpaceObject.ref_spaceObject;
				tispaceGameState = ((ref_spaceObject != null) ? ref_spaceObject.barycenter : null);
			}
			bool inTransfer = this.selectedSpaceObject.ref_fleet.inTransfer;
			this.selectedSpaceObject = null;
			this.infoPanelImageState = null;
			this.RemoveEnemyFleetListeners();
			this.CheckForCloseCanvas();
			if (!inTransfer && GeneralControlsController.UIOtherSelectedState == null && tispaceGameState != null)
			{
				TIUtilities.GotoGameState(tispaceGameState, false, false, false, false, true, -1f);
			}
			this.DebugPanel.gameObject.SetActive(false);
		}

		// Token: 0x0600571F RID: 22303 RVA: 0x0027D4F4 File Offset: 0x0027B6F4
		private void LaunchNaturalSpaceBodyDetail(TISpaceBodyState spaceBody)
		{
			if (this.naturalBodyPanel != null && !this.naturalBodyPanel.enabled)
			{
				this.naturalBodyPanel.enabled = true;
				if (!spaceBody.isEarth)
				{
					this.spacebodyUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_SpaceObjectDetailCanvas_Spacebody, false, true);
				}
				base.canvasManager.SetActiveInfoPanel(InfoPanel.SpaceBodyDetail, 0f);
			}
			else
			{
				this.RemoveNaturalSpaceBodyListeners();
			}
			this.selectedSpaceObject = spaceBody;
			this.AddNaturalSpaceBodyListeners();
			this.UpdateNaturalSpaceBodyCanvas(spaceBody);
			this.UpdateInfoPanelImage(spaceBody, false);
		}

		// Token: 0x06005720 RID: 22304 RVA: 0x0027D578 File Offset: 0x0027B778
		private void DisableNaturalSpaceBodyPanel()
		{
			if (this.naturalBodyPanel != null)
			{
				this.naturalBodyPanel.enabled = false;
			}
			this.HideTutorials();
			GeneralControlsController.ConditionalCancelSelectedOtherState(this.selectedSpaceObject);
			this.selectedSpaceObject = null;
			this.infoPanelImageState = null;
			this.RemoveNaturalSpaceBodyListeners();
			this.CheckForCloseCanvas();
		}

		// Token: 0x06005721 RID: 22305 RVA: 0x0027D5CC File Offset: 0x0027B7CC
		private void LaunchDetailLagrangePoint(TILagrangePointState lagrangePoint)
		{
			if (this.lagrangePointPanel != null && !this.lagrangePointPanel.enabled)
			{
				this.lagrangePointPanel.enabled = true;
				this.lagrangeUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_SpaceObjectDetailCanvas_Lagrange, false, true);
				base.canvasManager.SetActiveInfoPanel(InfoPanel.LagrangeDetail, 0f);
			}
			else
			{
				this.RemoveLagrangePointListeners();
			}
			this.selectedSpaceObject = lagrangePoint;
			this.AddLagrangePointListeners();
			this.UpdateLagrangePointCanvas(lagrangePoint);
		}

		// Token: 0x06005722 RID: 22306 RVA: 0x0027D63F File Offset: 0x0027B83F
		private void DisableLagrangePointPanel()
		{
			if (this.lagrangePointPanel != null)
			{
				this.lagrangePointPanel.enabled = false;
			}
			this.HideTutorials();
			GeneralControlsController.ConditionalCancelSelectedOtherState(this.selectedSpaceObject);
			this.selectedSpaceObject = null;
			this.RemoveLagrangePointListeners();
			this.CheckForCloseCanvas();
		}

		// Token: 0x06005723 RID: 22307 RVA: 0x0027D680 File Offset: 0x0027B880
		private void LaunchDetailHab(TIHabState hab)
		{
			if (this.habPanel != null && !this.habPanel.enabled)
			{
				this.habPanel.enabled = true;
				this.habUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_SpaceObjectDetailCanvas_Hab, false, true);
				base.canvasManager.SetActiveInfoPanel(InfoPanel.HabDetail, 0f);
			}
			else
			{
				this.RemoveHabListeners();
			}
			this.selectedSpaceObject = hab;
			this.RevertHabRename();
			this.AddHabListeners();
			this.UpdateHabCanvas(hab);
			this.UpdateInfoPanelImage(hab, false);
		}

		// Token: 0x06005724 RID: 22308 RVA: 0x0027D704 File Offset: 0x0027B904
		private void DisableHabPanel()
		{
			if (this.habPanel != null)
			{
				this.habPanel.enabled = false;
			}
			this.HideTutorials();
			GeneralControlsController.ConditionalCancelSelectedOtherState(this.selectedSpaceObject);
			this.selectedSpaceObject = null;
			this.RevertHabRename();
			this.RemoveHabListeners();
			this.infoPanelImageState = null;
			this.CheckForCloseCanvas();
		}

		// Token: 0x06005725 RID: 22309 RVA: 0x0027D75C File Offset: 0x0027B95C
		public void HideTutorials()
		{
			this.enemyFleetUITutorialController.HideTutorial();
			this.habUITutorialController.HideTutorial();
			this.lagrangeUITutorialController.HideTutorial();
			this.spacebodyUITutorialController.HideTutorial();
		}

		// Token: 0x06005726 RID: 22310 RVA: 0x0027D78C File Offset: 0x0027B98C
		private void SetRotationRate()
		{
			TIGameState uiotherSelectedState = this.UIOtherSelectedState;
			SpaceObjectType? spaceObjectType;
			if (uiotherSelectedState == null)
			{
				spaceObjectType = null;
			}
			else
			{
				TISpaceObjectState ref_spaceObject = uiotherSelectedState.ref_spaceObject;
				spaceObjectType = ((ref_spaceObject != null) ? new SpaceObjectType?(ref_spaceObject.objectType) : null);
			}
			SpaceObjectType? spaceObjectType2 = spaceObjectType;
			if (spaceObjectType2 != null)
			{
				switch (spaceObjectType2.GetValueOrDefault())
				{
				case SpaceObjectType.Star:
				case SpaceObjectType.Planet:
				case SpaceObjectType.DwarfPlanet:
				case SpaceObjectType.Asteroid:
				case SpaceObjectType.Comet:
				case SpaceObjectType.PlanetaryMoon:
				case SpaceObjectType.AsteroidalMoon:
					this.modelRotationRate = 1.5f / (float)this.UIOtherSelectedState.ref_spaceBody.rotationPeriod_Hours;
					this.modelRotationAxis = -this.UIOtherSelectedState.ref_spaceBody.tilt_Deg * Vector3.down;
					this.modelRotationAxis = Quaternion.AngleAxis(this.UIOtherSelectedState.ref_spaceBody.tilt_Deg, Vector3.forward) * Vector3.down;
					return;
				case SpaceObjectType.Hab:
					this.modelRotationRate = 0.02f;
					this.modelRotationAxis = Vector3.up;
					return;
				}
			}
			this.modelRotationRate = 0f;
			this.modelRotationAxis = Vector3.down;
		}

		// Token: 0x06005727 RID: 22311 RVA: 0x0027D8A8 File Offset: 0x0027BAA8
		private Image BaseModuleIcon(int sector, int moduleNum)
		{
			switch (sector)
			{
			case 0:
				switch (moduleNum)
				{
				case 0:
					return this.S0M0;
				case 1:
					return this.S0M1;
				case 2:
					return this.S0M2;
				case 3:
					return this.S0M3;
				case 4:
					return this.S0M4;
				}
				break;
			case 1:
				switch (moduleNum)
				{
				case 0:
					return this.S1M0;
				case 1:
					return this.S1M1;
				case 2:
					return this.S1M2;
				case 3:
					return this.S1M3;
				}
				break;
			case 2:
				switch (moduleNum)
				{
				case 0:
					return this.S2M0;
				case 1:
					return this.S2M1;
				case 2:
					return this.S2M2;
				case 3:
					return this.S2M3;
				}
				break;
			case 3:
				switch (moduleNum)
				{
				case 0:
					return this.S3M0;
				case 1:
					return this.S3M1;
				case 2:
					return this.S3M2;
				case 3:
					return this.S3M3;
				}
				break;
			case 4:
				switch (moduleNum)
				{
				case 0:
					return this.S4M0;
				case 1:
					return this.S4M1;
				case 2:
					return this.S4M2;
				case 3:
					return this.S4M3;
				}
				break;
			}
			Log.Error("Bad sector/module passed", Array.Empty<object>());
			return null;
		}

		// Token: 0x06005728 RID: 22312 RVA: 0x0027D9FC File Offset: 0x0027BBFC
		public static void TurnOffNaughtyShaderForUI(GameObject gameObject)
		{
			foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>().ToList<MeshRenderer>())
			{
				int[] badIndices = (from i in meshRenderer.sharedMaterials.Select<Material, int>(delegate(Material x, int i)
					{
						if (!(x.shader.name == "FORGE3D/Planets HD/Atmosphere"))
						{
							return -1;
						}
						return i;
					})
					where i != -1
					select i).ToArray<int>();
				meshRenderer.sharedMaterials = meshRenderer.sharedMaterials.Where<Material>((Material x, int i) => !badIndices.Contains(i)).ToArray<Material>();
			}
		}

		// Token: 0x06005729 RID: 22313 RVA: 0x0027DAD4 File Offset: 0x0027BCD4
		private void UpdateInfoPanelImage(TISpaceObjectState spaceObjectState, bool forceUpdate = false)
		{
			if (this.infoPanelImageState == spaceObjectState && !forceUpdate)
			{
				return;
			}
			if (spaceObjectState.isSpaceBodyState || spaceObjectState.isSpaceFleetState || (spaceObjectState.isHabState && spaceObjectState.ref_hab.IsStation))
			{
				if (this.selectionCameraInstance == null)
				{
					this.selectionCameraInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.selectionCamera);
					this.previewPosition = this.selectionCameraInstance.transform.Find("InfoPanelPreviewPosition").gameObject;
					this.originalPreviewPosition = this.previewPosition.transform.localPosition;
					this.originalPreviewRotation = this.previewPosition.transform.localRotation.eulerAngles;
				}
				this.previewPosition.transform.localPosition = this.originalPreviewPosition;
				this.previewPosition.transform.localRotation = Quaternion.Euler(this.originalPreviewRotation);
				foreach (object obj in this.previewPosition.transform)
				{
					Transform transform = (Transform)obj;
					transform.parent = null;
					global::UnityEngine.Object.Destroy(transform.gameObject);
				}
				float num = 0.25f;
				GameObject gameObject;
				if (spaceObjectState.isSpaceBodyState)
				{
					gameObject = GameControl.assetLoader.LoadAsset<GameObject>(spaceObjectState.modelResource);
				}
				else
				{
					GameObject gameObject2 = spaceObjectState.gameObjectLink;
					if (gameObject2 == null)
					{
						Transform transform2 = GameControl.solarSystem.FindObject(spaceObjectState.ID.ToString());
						if (!(transform2 != null))
						{
							return;
						}
						gameObject2 = transform2.gameObject;
					}
					if (spaceObjectState.isSpaceFleetState)
					{
						Transform transform3 = gameObject2.transform.Find(string.Format("{0} Container", spaceObjectState.ID));
						if (!(transform3 != null))
						{
							return;
						}
						gameObject = transform3.gameObject;
					}
					else
					{
						if (!spaceObjectState.isHabState)
						{
							return;
						}
						Transform transform4 = gameObject2.transform.Find("Model");
						if (!(transform4 != null))
						{
							return;
						}
						gameObject = transform4.gameObject;
					}
				}
				if (!spaceObjectState.isSpaceFleetState)
				{
					this.modelInstance = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, this.previewPosition.transform);
					if (spaceObjectState.isEarth)
					{
						EarthNightLightShaderDriver componentInChildren = this.modelInstance.GetComponentInChildren<EarthNightLightShaderDriver>(true);
						componentInChildren.enabled = true;
						componentInChildren.Initialize();
						componentInChildren.enabled = false;
					}
					if (spaceObjectState.objectType == SpaceObjectType.Comet)
					{
						CometController cometController = global::UnityEngine.Object.Instantiate<CometController>(spaceObjectState.controller.GetComponentInChildren<CometController>(true));
						cometController.transform.SetParent(this.modelInstance.transform, false);
						cometController.transform.localPosition = Vector3.zero;
						cometController.InitiateOverrideRenderMode(spaceObjectState.ref_spaceBody, this.selectionCamera.GetComponent<Camera>(), true);
					}
				}
				else
				{
					this.modelInstance = new GameObject();
					this.modelInstance.transform.SetParent(this.previewPosition.transform);
					this.modelInstance.name = gameObject.name;
					this.modelInstance.transform.localPosition = gameObject.transform.localPosition;
					this.modelInstance.transform.localScale = gameObject.transform.localScale;
					for (int i = 0; i < gameObject.transform.childCount; i++)
					{
						if (i < 20)
						{
							global::UnityEngine.Object.Instantiate<Transform>(gameObject.transform.GetChild(i), this.modelInstance.transform).localRotation = Quaternion.identity;
						}
					}
				}
				this.modelInstance.SetActive(true);
				this.habRawImageObject.SetActive(true);
				if (spaceObjectState.isSpaceBodyState)
				{
					num = (float)(20.0 / (double)spaceObjectState.modelScale);
					this.modelInstance.transform.Rotate(spaceObjectState.ref_spaceBody.tilt_Deg, spaceObjectState.ref_spaceBody.rotationOffset_Deg, 0f);
					this.modelInstance.transform.localPosition = Vector3.zero;
					this.naturalSpaceBodyBackgroundImage.sprite = World.Active.GetExistingManager<CameraManager>().skyboxBackdrop;
					this.naturalSpaceBodyBackgroundImage.rectTransform.localPosition = new Vector3((float)global::UnityEngine.Random.Range(-208, 208), (float)global::UnityEngine.Random.Range(-185, 0), 0f);
					this.naturalSpaceBodyBackgroundImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, (float)global::UnityEngine.Random.Range(0, 359));
					SpaceObjectDetailController.TurnOffNaughtyShaderForUI(this.modelInstance);
					this.previewPosition.transform.localPosition = new Vector3(0f, 0f, 55f);
				}
				else if (spaceObjectState.isHabState)
				{
					this.baseIllustrationObject.SetActive(false);
					TIHabState ref_hab = spaceObjectState.ref_hab;
					num = ((ref_hab != null && ref_hab.activeSectors.Count == 1) ? 0.25f : 0.075f);
					this.modelInstance.transform.Rotate(Vector3.right, 155f);
					this.modelInstance.transform.Rotate(Vector3.forward, 180f);
					this.modelInstance.transform.localPosition = Vector3.zero;
					this.habBackgroundImage.sprite = World.Active.GetExistingManager<CameraManager>().skyboxBackdrop;
					this.habBackgroundImage.rectTransform.localPosition = new Vector3((float)global::UnityEngine.Random.Range(-208, 208), (float)global::UnityEngine.Random.Range(-185, 0), 0f);
					this.habBackgroundImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, (float)global::UnityEngine.Random.Range(0, 359));
					this.modelInstance.name = "Hab Model Duplicate";
					HabModelController component = this.modelInstance.GetComponent<HabModelController>();
					component.Initialize(spaceObjectState.ref_hab, false, null);
					component.GetModuleControllers().ForEach(delegate(HabModuleController x)
					{
						x.DuplicateMaterialsForUIDisplay();
					});
				}
				else if (spaceObjectState.isSpaceFleetState)
				{
					TISpaceFleetState ref_fleet = spaceObjectState.ref_fleet;
					for (int j = 0; j < this.modelInstance.transform.childCount; j++)
					{
						Transform transform5 = this.modelInstance.transform.GetChild(j).transform;
						if (transform5.childCount > 0)
						{
							ShipVisController visController = transform5.GetChild(0).GetComponent<ShipVisController>();
							if (visController != null)
							{
								visController.SetAsUIVisualization(ref_fleet.ships.SingleOrDefault<TISpaceShipState>((TISpaceShipState x) => x.ID.ToString() == visController.name), true);
							}
						}
					}
					if (ref_fleet.landed)
					{
						num = 0.15f;
						this.modelInstance.transform.Rotate(Vector3.right, 270f);
						GameControl.assetLoader.LoadAssetForImageAssignment(ref_fleet.ref_habSite.template.backgroundPath, this.enemyFleetBackgroundImage);
						this.enemyFleetBackgroundImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
						this.enemyFleetBackgroundImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
					}
					else
					{
						num = 0.3f;
						this.modelInstance.transform.Rotate(Vector3.right, 170f);
						this.modelInstance.transform.Rotate(Vector3.forward, 180f);
						this.enemyFleetBackgroundImage.sprite = World.Active.GetExistingManager<CameraManager>().skyboxBackdrop;
						this.enemyFleetBackgroundImage.rectTransform.localPosition = new Vector3((float)global::UnityEngine.Random.Range(-208, 208), (float)global::UnityEngine.Random.Range(-185, 0), 0f);
						this.enemyFleetBackgroundImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, (float)global::UnityEngine.Random.Range(0, 359));
						this.previewPosition.transform.localPosition = new Vector3(5f, 0f, 40f);
						this.previewPosition.transform.localRotation = Quaternion.Euler(-10f, 0f, 0f);
						float num2 = 150f;
						int num3 = 0;
						int num4 = 1;
						Vector3[] pipPosition = ref_fleet.pipPosition;
						for (int k = 0; k < this.modelInstance.transform.childCount; k++)
						{
							if (k > 20)
							{
								this.modelInstance.transform.GetChild(k).gameObject.SetActive(false);
							}
							else
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
						}
					}
				}
				this.modelInstance.transform.SetLayer(10, true);
				this.modelInstance.transform.localScale = Vector3.one * num;
				Collider[] componentsInChildren = this.modelInstance.transform.GetComponentsInChildren<Collider>();
				for (int l = 0; l < componentsInChildren.Length; l++)
				{
					componentsInChildren[l].enabled = false;
				}
				ShipUIController[] componentsInChildren2 = this.modelInstance.transform.GetComponentsInChildren<ShipUIController>();
				for (int l = 0; l < componentsInChildren2.Length; l++)
				{
					componentsInChildren2[l].gameObject.SetActive(false);
				}
				SpaceCouncilorController[] componentsInChildren3 = this.modelInstance.transform.GetComponentsInChildren<SpaceCouncilorController>();
				for (int l = 0; l < componentsInChildren3.Length; l++)
				{
					componentsInChildren3[l].gameObject.SetActive(false);
				}
				this.SetRotationRate();
				this.selectionCameraInstance.SetActive(true);
			}
			else
			{
				TIHabState ref_hab2 = spaceObjectState.ref_hab;
				this.habRawImageObject.SetActive(false);
				GameControl.assetLoader.LoadAssetForImageAssignment(ref_hab2.habSite.template.backgroundPath, this.habBackgroundImage);
				this.habBackgroundImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
				this.habBackgroundImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				for (int m = 0; m <= 4; m++)
				{
					TISectorState tisectorState = ref_hab2.sectors[m];
					for (int n = 0; n < tisectorState.habModules.Count; n++)
					{
						TIHabModuleState tihabModuleState = tisectorState.habModules[n];
						Image image = this.BaseModuleIcon(m, n);
						if (tihabModuleState.empty)
						{
							image.enabled = false;
						}
						else
						{
							GameControl.assetLoader.LoadAssetForImageAssignment(tihabModuleState.underConstruction ? tihabModuleState.moduleTemplate.constructionIconResource(HabType.Base) : tihabModuleState.moduleTemplate.baseIconResource, image);
							switch (tihabModuleState.moduleTemplate.tier)
							{
							case 1:
								image.rectTransform.sizeDelta = (tihabModuleState.moduleTemplate.mine ? new Vector2(40f, 10f) : new Vector2(10f, 10f));
								break;
							case 2:
								image.rectTransform.sizeDelta = (tihabModuleState.moduleTemplate.mine ? new Vector2(48f, 12f) : new Vector2(12f, 12f));
								break;
							case 3:
								image.rectTransform.sizeDelta = (tihabModuleState.moduleTemplate.mine ? new Vector2(64f, 16f) : new Vector2(16f, 16f));
								break;
							}
							image.enabled = true;
						}
					}
				}
				this.C034T.enabled = false;
				this.C04T.enabled = false;
				this.C03T.enabled = false;
				this.C24C.enabled = false;
				this.C13C.enabled = false;
				this.baseIllustrationObject.SetActive(true);
			}
			this.infoPanelImageState = spaceObjectState;
		}

		// Token: 0x0600572A RID: 22314 RVA: 0x0027E7A4 File Offset: 0x0027C9A4
		public void UpdateAssetPanelImage(TISpaceFleetState fleet, bool forceUpdate = false)
		{
			if ((this.assetPanelImageState == fleet && !forceUpdate) || fleet.ships.Count < 1 || fleet.archived)
			{
				return;
			}
			if (this.assetCameraInstance == null)
			{
				this.assetCameraInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.mySelectedObjectCamera);
				this.assetPosition = this.assetCameraInstance.transform.Find("AssetPanelPreviewPosition").gameObject;
				this.originalAssetPreviewPosition = this.assetPosition.transform.localPosition;
				this.originalAssetPreviewRotation = this.assetPosition.transform.localRotation.eulerAngles;
			}
			this.assetPosition.transform.localPosition = this.originalAssetPreviewPosition;
			this.assetPosition.transform.localRotation = Quaternion.Euler(this.originalAssetPreviewRotation);
			foreach (object obj in this.assetPosition.transform)
			{
				Transform transform = (Transform)obj;
				transform.parent = null;
				global::UnityEngine.Object.Destroy(transform.gameObject);
			}
			GameObject gameObject = fleet.gameObjectLink;
			if (base.gameObject == null)
			{
				gameObject = GameObject.Find(fleet.ID.ToString());
			}
			if (gameObject != null)
			{
				GameObject gameObject2 = gameObject.transform.Find(string.Format("{0} Container", fleet.ID)).gameObject;
				this.assetModelInstance = new GameObject();
				this.assetModelInstance.transform.SetParent(this.assetPosition.transform);
				this.assetModelInstance.name = gameObject2.name;
				this.assetModelInstance.transform.localPosition = gameObject2.transform.localPosition;
				this.assetModelInstance.transform.localScale = gameObject2.transform.localScale;
				for (int i = 0; i < gameObject2.transform.childCount; i++)
				{
					if (i < 20)
					{
						global::UnityEngine.Object.Instantiate<Transform>(gameObject2.transform.GetChild(i), this.assetModelInstance.transform).localRotation = Quaternion.identity;
					}
				}
				this.assetModelInstance.SetActive(false);
				float num = (fleet.landed ? 0.15f : (0.6f / ((float)fleet.ships.Count / 2f)));
				this.assetModelInstance.transform.localPosition = Vector3.zero;
				this.assetModelInstance.transform.SetLayer(10, true);
				this.assetModelInstance.transform.localScale = Vector3.one * num;
				for (int j = 0; j < this.assetModelInstance.transform.childCount; j++)
				{
					if (this.assetModelInstance.transform.GetChild(j).transform.childCount > 0)
					{
						ShipVisController visController = this.assetModelInstance.transform.GetChild(j).transform.GetChild(0).GetComponent<ShipVisController>();
						if (visController != null)
						{
							visController.SetAsUIVisualization(fleet.ships.SingleOrDefault<TISpaceShipState>((TISpaceShipState x) => x.ID.ToString() == visController.name), true);
						}
					}
				}
				if (fleet.landed)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(fleet.ref_habSite.template.backgroundPath, this.myFleetBackgroundImage);
					this.assetModelInstance.transform.Rotate(Vector3.right, 270f);
					this.myFleetBackgroundImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
					this.myFleetBackgroundImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				}
				else
				{
					this.myFleetBackgroundImage.sprite = World.Active.GetExistingManager<CameraManager>().skyboxBackdrop;
					this.assetModelInstance.transform.Rotate(Vector3.right, 170f);
					this.assetModelInstance.transform.Rotate(Vector3.forward, 180f);
					this.myFleetBackgroundImage.rectTransform.localPosition = new Vector3((float)global::UnityEngine.Random.Range(-180, 180), (float)global::UnityEngine.Random.Range(-90, 90), 0f);
					this.myFleetBackgroundImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, (float)global::UnityEngine.Random.Range(0, 359));
					this.assetPosition.transform.localPosition = new Vector3(5f, 0f, 40f);
					this.assetPosition.transform.localRotation = Quaternion.Euler(-10f, 0f, 0f);
					float num2 = 150f;
					int num3 = 0;
					int num4 = 1;
					Vector3[] pipPosition = fleet.pipPosition;
					for (int k = 0; k < this.assetModelInstance.transform.childCount; k++)
					{
						if (k > 20)
						{
							this.assetModelInstance.transform.GetChild(k).gameObject.SetActive(false);
						}
						else
						{
							if (this.assetModelInstance.transform.GetChild(k).ActiveChildCount() > 0)
							{
								this.assetModelInstance.transform.GetChild(k).localPosition = new Vector3(num2 * pipPosition[num3].x * (float)num4, num2 * pipPosition[num3].y * (float)num4, num2 * pipPosition[num3].z * (float)num4);
								this.assetModelInstance.transform.GetChild(k).GetChild(0).transform.localPosition = Vector3.zero;
								this.assetModelInstance.transform.GetChild(k).GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
								num3++;
							}
							if (num3 == 5)
							{
								num3 = 1;
								num4++;
							}
						}
					}
				}
				Collider[] componentsInChildren = this.assetModelInstance.transform.GetComponentsInChildren<Collider>();
				for (int l = 0; l < componentsInChildren.Length; l++)
				{
					componentsInChildren[l].enabled = false;
				}
				ShipUIController[] componentsInChildren2 = this.assetModelInstance.transform.GetComponentsInChildren<ShipUIController>();
				for (int l = 0; l < componentsInChildren2.Length; l++)
				{
					componentsInChildren2[l].gameObject.SetActive(false);
				}
				SpaceCouncilorController[] componentsInChildren3 = this.assetModelInstance.transform.GetComponentsInChildren<SpaceCouncilorController>();
				for (int l = 0; l < componentsInChildren3.Length; l++)
				{
					componentsInChildren3[l].gameObject.SetActive(false);
				}
				this.assetModelInstance.SetActive(true);
				this.assetCameraInstance.SetActive(true);
				this.assetPanelImageState = fleet;
				return;
			}
		}

		// Token: 0x0600572B RID: 22315 RVA: 0x0027EEA0 File Offset: 0x0027D0A0
		public static Sprite GetParentBodyIconResource(TISpaceFleetState fleet, out TIGameState parentState)
		{
			if (fleet.inTransfer)
			{
				parentState = null;
				return null;
			}
			if (!fleet.dockedOrLanded)
			{
				parentState = fleet.orbitState.barycenter;
				return fleet.orbitState.barycenter.icon;
			}
			if (fleet.dockedLocation.isHabState)
			{
				TIHabState ref_hab = fleet.dockedLocation.ref_hab;
				if (ref_hab.IsBase)
				{
					parentState = ref_hab.habSite.parentBody;
					return ref_hab.habSite.parentBody.icon;
				}
				parentState = ref_hab.orbitState.barycenter;
				return ref_hab.orbitState.barycenter.icon;
			}
			else
			{
				if (fleet.dockedLocation.isHabSiteState)
				{
					TIHabSiteState ref_habSite = fleet.dockedLocation.ref_habSite;
					parentState = ref_habSite.parentBody;
					return ref_habSite.parentBody.icon;
				}
				parentState = null;
				return null;
			}
		}

		// Token: 0x0600572C RID: 22316 RVA: 0x0027EF74 File Offset: 0x0027D174
		public void OnClickRename(int which)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			if (this.selectedAsset != null)
			{
				this.nameInputField.text = this.selectedAsset.GetDisplayName(this.selectedAsset.faction);
			}
			this.ShowRenameMyFleetPanel();
		}

		// Token: 0x0600572D RID: 22317 RVA: 0x0027EFC2 File Offset: 0x0027D1C2
		public void OnClickRevertRename()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.RevertRename();
		}

		// Token: 0x0600572E RID: 22318 RVA: 0x0027EFD6 File Offset: 0x0027D1D6
		public void RevertRename()
		{
			this.renameMyFleetPanel.SetActive(false);
			this.nameInputField.text = "";
		}

		// Token: 0x0600572F RID: 22319 RVA: 0x0027EFF4 File Offset: 0x0027D1F4
		public void OnClickSaveName()
		{
			if (this.selectedAsset == null)
			{
				this.RevertRename();
				return;
			}
			this.renameMyFleetPanel.SetActive(false);
			this.selectedAsset.faction.playerControl.StartAction(new ChangeFleetBio(this.selectedAsset, this.selectedAsset.faction, this.nameInputField.text));
			base.canvasManager.SpaceObjectDetail.Refresh();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.UpdatePlayerFleetObjectCanvas(this.selectedAsset);
		}

		// Token: 0x06005730 RID: 22320 RVA: 0x0027F080 File Offset: 0x0027D280
		public void ShowRenameMyFleetPanel()
		{
			this.renameMyFleetPanel.SetActive(true);
			this.nameInputField.Select();
		}

		// Token: 0x06005731 RID: 22321 RVA: 0x0027F09C File Offset: 0x0027D29C
		public void OnClickHabRename()
		{
			TIGameState tigameState = this.selectedSpaceObject;
			if (((tigameState != null) ? tigameState.ref_hab : null) != null)
			{
				if (this.selectedSpaceObject.ref_hab.faction != base.activePlayer)
				{
					return;
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
				this.habNameInputField.text = this.selectedSpaceObject.ref_hab.displayName;
				this.ShowRenameMyHabPanel();
			}
		}

		// Token: 0x06005732 RID: 22322 RVA: 0x0027F10E File Offset: 0x0027D30E
		public void OnClickRevertHabRename()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.RevertHabRename();
		}

		// Token: 0x06005733 RID: 22323 RVA: 0x0027F122 File Offset: 0x0027D322
		public void RevertHabRename()
		{
			this.renameHabPanel.SetActive(false);
			this.habNameInputField.text = "";
		}

		// Token: 0x06005734 RID: 22324 RVA: 0x0027F140 File Offset: 0x0027D340
		public void OnClickSaveHabName()
		{
			TIGameState tigameState = this.selectedSpaceObject;
			if (((tigameState != null) ? tigameState.ref_hab : null) == null)
			{
				this.RevertHabRename();
				return;
			}
			this.renameHabPanel.SetActive(false);
			this.selectedSpaceObject.ref_hab.faction.playerControl.StartAction(new ChangeHabBio(this.selectedSpaceObject.ref_hab, this.habNameInputField.text, this.selectedSpaceObject.ref_hab.customHabIconResource));
			base.canvasManager.SpaceObjectDetail.Refresh();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.UpdateHabCanvas(this.selectedSpaceObject.ref_hab);
			GameControl.eventManager.TriggerEvent(new GameStateNameChanged(this.selectedSpaceObject.ref_hab), null, Array.Empty<object>());
		}

		// Token: 0x06005735 RID: 22325 RVA: 0x0027F20C File Offset: 0x0027D40C
		public void ShowRenameMyHabPanel()
		{
			this.renameHabPanel.SetActive(true);
			this.habNameInputField.Select();
		}

		// Token: 0x06005736 RID: 22326 RVA: 0x0027F225 File Offset: 0x0027D425
		public void OnSelectInputBox()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x06005737 RID: 22327 RVA: 0x0027F22C File Offset: 0x0027D42C
		public void OnDeSelectInputBox()
		{
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x06005738 RID: 22328 RVA: 0x0027F234 File Offset: 0x0027D434
		public void CleanupTextures()
		{
			this.HabExitButtonClicked();
			this.EnemyFleetExitButtonClicked();
			this.MyFleetExitButtonClicked();
			this.LagrangePointExitButtonClicked();
			this.NaturalBodyExitButtonClicked();
			this.mySelectedObjectCamera = null;
			this.selectionCamera = null;
			if (this.assetCameraInstance != null)
			{
				Camera component = this.assetCameraInstance.GetComponent<Camera>();
				if (component.targetTexture != null)
				{
					RenderTexture targetTexture = component.targetTexture;
					component.targetTexture = null;
					targetTexture.Release();
				}
			}
			if (this.selectionCameraInstance != null)
			{
				Camera component2 = this.selectionCameraInstance.GetComponent<Camera>();
				if (component2.targetTexture != null)
				{
					RenderTexture targetTexture2 = component2.targetTexture;
					component2.targetTexture = null;
					targetTexture2.Release();
				}
			}
		}

		// Token: 0x06005739 RID: 22329 RVA: 0x0027F2E8 File Offset: 0x0027D4E8
		public static void UpdatePendingCombatIcon(Image icon, TISpaceFleetState fleet)
		{
			TIFactionState tifactionState = null;
			Trajectory trajectory = fleet.trajectory;
			if (TIGameState.Valid((trajectory != null) ? trajectory.destinationFleet : null))
			{
				Trajectory trajectory2 = fleet.trajectory;
				tifactionState = ((trajectory2 != null) ? trajectory2.destinationFleet.ref_faction : null);
			}
			else
			{
				Trajectory trajectory3 = fleet.trajectory;
				if (TIGameState.Valid((trajectory3 != null) ? trajectory3.destinationStation : null))
				{
					Trajectory trajectory4 = fleet.trajectory;
					tifactionState = ((trajectory4 != null) ? trajectory4.destinationStation.ref_faction : null);
				}
			}
			if (tifactionState != null)
			{
				icon.enabled = tifactionState != fleet.ref_faction && !fleet.ref_faction.permanentAlly(tifactionState);
				return;
			}
			icon.enabled = false;
		}

		// Token: 0x0600573A RID: 22330 RVA: 0x0027F394 File Offset: 0x0027D594
		private void UpdateEnemyFleetTransferData(TISpaceFleetState fleet)
		{
			this.enemyFleetDeltaV.SetText(Loc.T("UI.Fleets.DVValue", new object[]
			{
				TIUtilities.FormatBigOrSmallNumber(fleet.currentDeltaV_kps, 1, 7, 0, false, false),
				TIUtilities.FormatBigOrSmallNumber(fleet.maxDeltaV_kps, 1, 7, 0, false, false)
			}));
			if (FleetsSceenFleetListItemController.ShouldShowTransitData(fleet))
			{
				this.enemyTransferProgressIcon.rectTransform.localPosition = new Vector2(this.enemyFleetTransferSliderZeroPoint + (float)(fleet.TrajectoryFractionCompleted() * (double)this.enemyFleetTransferSliderRange), this.enemyTransferProgressIcon.rectTransform.localPosition.y);
				SpaceObjectDetailController.UpdatePendingCombatIcon(this.enemyTransferPendingCombatIcon, fleet);
				int num = 0;
				using (IEnumerator<object> enumerator = this.enemyFleetDetailList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (SpaceObjectDetailController.<>o__489.<>p__0 == null)
						{
							SpaceObjectDetailController.<>o__489.<>p__0 = CallSite<Func<CallSite, object, ShipsInFleetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipsInFleetListItemController), typeof(SpaceObjectDetailController)));
						}
						SpaceObjectDetailController.<>o__489.<>p__0.Target(SpaceObjectDetailController.<>o__489.<>p__0, enumerator.Current).SetListItem(fleet.ships[num++]);
					}
				}
			}
		}

		// Token: 0x0600573B RID: 22331 RVA: 0x0027F4D0 File Offset: 0x0027D6D0
		private void UpdateEnemyFleetObjectCanvas(TISpaceFleetState fleet)
		{
			if (!TIGameState.Valid(fleet) || fleet.ships.Count == 0)
			{
				base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
				return;
			}
			this.enemyFleetName.SetText(fleet.GetDisplayName(base.activePlayer));
			this.enemyFleetFactionIcon.sprite = fleet.faction.factionIcon64;
			GameControl.assetLoader.LoadAssetForImageAssignment(fleet.faction.template.gradientPath, this.enemyFleetFactionGradient);
			this.enemyFleetDeltaV.SetText(Loc.T("UI.Fleets.DVValue", new object[]
			{
				TIUtilities.FormatBigOrSmallNumber(fleet.currentDeltaV_kps, 1, 7, 0, false, false),
				TIUtilities.FormatBigOrSmallNumber(fleet.maxDeltaV_kps, 1, 7, 0, false, false)
			}));
			this.enemyFleetSize.SetText(fleet.ships.Count.ToString("N0"));
			this.enemyFleetAcceleration.SetText(Loc.T(FleetsScreenController.accelerationStr((double)fleet.cruiseAcceleration_gs, false, false, false)));
			this.enemyFleetCombatScore.SetText(TIUtilities.FormatBigOrSmallNumber(fleet.SpaceCombatValue(), 1, 0, 0, false, false));
			float num = fleet.AssaultCombatValue(false);
			this.enemyFleetAssaultScore.SetText(num.ToString("N0"));
			if (FleetsSceenFleetListItemController.ShouldShowTransitData(fleet))
			{
				this.alertFleetButtonContainerObject.SetActive(true);
				this.alertFleetButtonText.SetText(SpaceObjectDetailController.GetFleetAlarmText(base.activePlayer, fleet));
				this.enemyTransferOriginIcon.sprite = fleet.trajectory.originOrbit.barycenter.icon;
				this.eo1 = fleet.trajectory.originOrbit;
				TISpaceFleetState destinationFleet = fleet.trajectory.destinationFleet;
				if (destinationFleet != null && destinationFleet.inTransfer)
				{
					this.enemyTransferDestinationIcon.sprite = fleet.trajectory.destinationFleet.icon;
					this.ed1 = fleet.trajectory.destinationFleet;
					this.enemyTransferDestinationDetailIcon.enabled = false;
				}
				else
				{
					Trajectory trajectory = fleet.trajectory;
					if (trajectory.nextTrajectory != null)
					{
						trajectory = trajectory.nextTrajectory;
					}
					if (trajectory.endsInCrash)
					{
						this.enemyTransferDestinationIcon.sprite = trajectory.collisionTarget.icon;
						this.d1 = trajectory.collisionTarget;
						this.enemyTransferDestinationIcon.enabled = false;
					}
					else if (trajectory.exitsSolarSystem)
					{
						this.enemyTransferDestinationIcon.sprite = GameControl.assetLoader.LoadAsset<Sprite>("icons_2d/ICO_none");
					}
					else
					{
						if (TIGameState.Valid(trajectory.destinationOrbit))
						{
							this.enemyTransferDestinationIcon.sprite = trajectory.destinationOrbit.barycenter.icon;
							this.ed1 = trajectory.destinationOrbit;
						}
						if (trajectory.destinationFleet != null)
						{
							if (trajectory.destinationFleet.deleted)
							{
								this.enemyTransferDestinationDetailIcon.sprite = trajectory.GetBarycenterAtTime(new TIDateTime(trajectory.arrivalTime, -1.0)).icon;
								this.ed2 = trajectory.destinationFleet;
								this.enemyTransferDestinationDetailIcon.enabled = true;
							}
							else
							{
								this.enemyTransferDestinationDetailIcon.sprite = trajectory.destinationFleet.icon;
								this.ed2 = trajectory.destinationFleet;
								this.enemyTransferDestinationDetailIcon.enabled = true;
							}
						}
						else if (TIGameState.Valid(fleet.trajectory.destinationStation))
						{
							this.enemyTransferDestinationDetailIcon.sprite = trajectory.destinationStation.icon;
							this.ed2 = trajectory.destinationStation;
							this.enemyTransferDestinationDetailIcon.enabled = true;
						}
						else
						{
							this.enemyTransferDestinationDetailIcon.enabled = false;
						}
					}
				}
				this.enemyTransferTextDetail.SetText(SpaceObjectDetailController.FleetTransferTwoLiner(fleet, true));
				this.enemyTransferProgressIcon.rectTransform.localPosition = new Vector2(this.enemyFleetTransferSliderZeroPoint + (float)(fleet.TrajectoryFractionCompleted() * (double)this.enemyFleetTransferSliderRange), this.enemyTransferProgressIcon.rectTransform.localPosition.y);
				SpaceObjectDetailController.UpdatePendingCombatIcon(this.enemyTransferPendingCombatIcon, fleet);
				this.enemyTransferObject.SetActive(true);
				this.enemyGenericOpDetailObject.SetActive(false);
			}
			else
			{
				this.alertFleetButtonContainerObject.SetActive(false);
				this.enemyGenericOpLine1.SetText(fleet.GetLocationDescription(base.activePlayer, true, true));
				if (fleet.CurrentOperations().Count > 0 && !(fleet.CurrentOperations()[0].operation is TransferOperation))
				{
					TIOrbitState ref_orbit = fleet.ref_orbit;
					this.eo1 = ((ref_orbit != null) ? ref_orbit.barycenter : null) ?? fleet;
					OperationData operationData = fleet.CurrentOperations()[0];
					GameControl.assetLoader.LoadAssetForImageAssignment(operationData.operation.GetOperationIconImagePath_Off(), this.enemyGenericOpImage);
					StringBuilder stringBuilder = new StringBuilder(operationData.operation.GetDisplayName());
					if (fleet.bombarding)
					{
						this.eo2 = fleet.bombardmentTarget;
						if (this.eo2 != null)
						{
							this.enemyGenericOpSmallImage.sprite = this.eo2.ref_spaceBody.icon;
							this.enemyGenericOpSmallImage.enabled = true;
							stringBuilder.Append(" / ").Append(operationData.target.GetDisplayName(base.activePlayer));
						}
						else
						{
							this.enemyGenericOpSmallImage.enabled = false;
						}
					}
					else if (operationData.target != null && operationData.target != fleet)
					{
						stringBuilder.Append(" / ").Append(operationData.target.GetDisplayName(base.activePlayer));
						this.eo2 = operationData.target.ref_naturalSpaceObject;
						this.enemyGenericOpSmallImage.sprite = operationData.target.ref_naturalSpaceObject.icon;
						this.enemyGenericOpSmallImage.enabled = true;
					}
					else if (this.eo1 != null)
					{
						this.eo2 = this.eo1.ref_naturalSpaceObject;
						if (this.eo2 != null)
						{
							this.enemyGenericOpSmallImage.sprite = this.eo2.ref_naturalSpaceObject.icon;
							this.enemyGenericOpSmallImage.enabled = true;
						}
						else
						{
							this.enemyGenericOpSmallImage.enabled = false;
						}
					}
					else
					{
						this.enemyGenericOpImage.enabled = false;
					}
					if (operationData.completionDate != null)
					{
						stringBuilder.Append(" / ").Append(operationData.completionDate.ToCustomDateString());
					}
					this.enemyGenericOpLine2.SetText(stringBuilder.ToString());
				}
				else
				{
					this.eo1 = fleet.ref_naturalSpaceObject;
					this.enemyGenericOpImage.sprite = fleet.ref_naturalSpaceObject.icon;
					if (fleet.dockedAtHab)
					{
						this.eo2 = fleet.ref_hab;
						this.enemyGenericOpSmallImage.sprite = fleet.ref_hab.icon;
						this.enemyGenericOpSmallImage.enabled = true;
					}
					else
					{
						this.enemyGenericOpSmallImage.enabled = false;
					}
					if (fleet.unavailableForOperations)
					{
						if (Mathd.Abs(fleet.returnToOperationsTime.DifferenceInDays(TITimeState.Now())) >= 1.0)
						{
							this.enemyGenericOpLine2.SetText(Loc.T("UI.Space.Fleet.Unavailable", new object[] { fleet.returnToOperationsTime.ToCustomDateString() }));
						}
						else
						{
							this.enemyGenericOpLine2.SetText(Loc.T("UI.Space.Fleet.Unavailable", new object[] { fleet.returnToOperationsTime.ToShortTimeString() }));
						}
					}
					else
					{
						this.enemyGenericOpLine2.SetText(Loc.T("UI.Space.NoOp"));
					}
				}
				this.enemyTransferObject.SetActive(false);
				this.enemyGenericOpDetailObject.SetActive(true);
			}
			this.enemyFleetShipsGridList.SetListSize<FleetShipGridItemController>(Mathf.Min(fleet.ships.Count, 20), false, false);
			int num2 = 0;
			using (IEnumerator<object> enumerator = this.enemyFleetShipsGridList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__490.<>p__0 == null)
					{
						SpaceObjectDetailController.<>o__490.<>p__0 = CallSite<Func<CallSite, object, FleetShipGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FleetShipGridItemController), typeof(SpaceObjectDetailController)));
					}
					SpaceObjectDetailController.<>o__490.<>p__0.Target(SpaceObjectDetailController.<>o__490.<>p__0, enumerator.Current).SetGridItem((num2 >= 20 && fleet.ships.Count > 20) ? null : fleet.ships[num2++], fleet, true);
				}
			}
			List<TISpaceShipState> ships = fleet.ships;
			this.enemyFleetDetailList.SetListSize<ShipsInFleetListItemController>(ships.Count, false, false);
			num2 = 0;
			using (IEnumerator<object> enumerator = this.enemyFleetDetailList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__490.<>p__1 == null)
					{
						SpaceObjectDetailController.<>o__490.<>p__1 = CallSite<Func<CallSite, object, ShipsInFleetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipsInFleetListItemController), typeof(SpaceObjectDetailController)));
					}
					SpaceObjectDetailController.<>o__490.<>p__1.Target(SpaceObjectDetailController.<>o__490.<>p__1, enumerator.Current).SetListItem(ships[num2++]);
				}
			}
			this.enemyShipListTransform.sizeDelta = new Vector2(this.enemyShipListTransform.sizeDelta.x, (float)Mathf.Min(32 + ships.Count * 47, 435));
			List<CouncilorView> list = fleet.CouncilorViewsPresentAndKnownToFaction(base.activePlayer);
			this.enemyFleetCouncilorsGrid.SetListSize<CombatShipCouncilorGridItemController>(list.Count, false, false);
			num2 = 0;
			using (IEnumerator<object> enumerator = this.enemyFleetCouncilorsGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__490.<>p__2 == null)
					{
						SpaceObjectDetailController.<>o__490.<>p__2 = CallSite<Func<CallSite, object, CombatShipCouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombatShipCouncilorGridItemController), typeof(SpaceObjectDetailController)));
					}
					SpaceObjectDetailController.<>o__490.<>p__2.Target(SpaceObjectDetailController.<>o__490.<>p__2, enumerator.Current).SetGridItem(list[num2++]);
				}
			}
		}

		// Token: 0x0600573C RID: 22332 RVA: 0x0027FEAC File Offset: 0x0027E0AC
		public void OnEnemyFleetImagePressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect", false, false);
			GameControl.eventManager.TriggerEvent(new FleetDetailRequested(this.UIOtherSelectedState.ref_fleet), null, Array.Empty<object>());
		}

		// Token: 0x0600573D RID: 22333 RVA: 0x0027FEDC File Offset: 0x0027E0DC
		public void OnPressEnemyFleetShipsDetailButton()
		{
			this.enemyFleetShipListOpen = !this.enemyFleetShipListOpen;
			if (this.enemyFleetShipListOpen)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenFinder", false, false);
				this.enemyShipListObject.SetActive(true);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseFinder", false, false);
				this.enemyShipListObject.SetActive(false);
			}
			base.canvasManager.ActiveInfoPanelResized(this.enemyFleetUpperPanelTransform.sizeDelta.y + (this.enemyFleetShipListOpen ? this.enemyShipListTransform.sizeDelta.y : 0f));
		}

		// Token: 0x0600573E RID: 22334 RVA: 0x0027FF6D File Offset: 0x0027E16D
		public void OnEnemyFleetTransferOriginClicked()
		{
			if (this.eo1 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.eo1, false, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x0600573F RID: 22335 RVA: 0x0027FFAB File Offset: 0x0027E1AB
		public void OnEnemyFleetDestinationClicked()
		{
			if (this.ed1 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.ed1, false, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005740 RID: 22336 RVA: 0x0027FFE9 File Offset: 0x0027E1E9
		public void OnEnemyFleetSpecificDestinationClicked()
		{
			if (this.ed2 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.ed2, false, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005741 RID: 22337 RVA: 0x00280027 File Offset: 0x0027E227
		public void OnEnemyFleetOpLocationClicked()
		{
			if (this.eo1 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.eo1, true, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005742 RID: 22338 RVA: 0x00280065 File Offset: 0x0027E265
		public void OnEnemyFleetSmallOpLocationClicked()
		{
			if (this.eo2 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.eo2, true, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005743 RID: 22339 RVA: 0x002800A4 File Offset: 0x0027E2A4
		public void OnAlertFleetClicked()
		{
			if (base.activePlayer.alarms.Any<Alarm>((Alarm x) => x.associatedGameState == this.selectedSpaceObject))
			{
				base.activePlayer.playerControl.StartAction(new DeleteFleetAlarm(base.activePlayer, this.selectedSpaceObject));
			}
			else if (SpaceObjectDetailController.CreateFleetAlarm(base.activePlayer, this.selectedSpaceObject.ref_fleet))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			this.alertFleetButtonText.SetText(SpaceObjectDetailController.GetFleetAlarmText(base.activePlayer, this.selectedSpaceObject.ref_fleet));
		}

		// Token: 0x06005744 RID: 22340 RVA: 0x00280148 File Offset: 0x0027E348
		public static bool CreateFleetAlarm(TIFactionState settingPlayer, TISpaceFleetState enemyFleet)
		{
			if (enemyFleet != null && enemyFleet.transferAssigned)
			{
				double num = enemyFleet.trajectory.durationFromLaunchToFinalArrival_s * 0.949999988079071;
				TIDateTime tidateTime = new TIDateTime(enemyFleet.trajectory.launchTime);
				tidateTime.AddSeconds(num);
				if (tidateTime < TITimeState.Now() || tidateTime >= enemyFleet.trajectory.finalArrivalTime)
				{
					num = enemyFleet.trajectory.finalArrivalTime.DifferenceInSeconds(TITimeState.Now()) * 0.5;
					tidateTime = TITimeState.Now();
					tidateTime.AddSeconds(num);
				}
				settingPlayer.playerControl.StartAction(new SetUserAlarmAction(settingPlayer, enemyFleet, AlarmType.FleetApproaching, tidateTime, string.Empty));
				return true;
			}
			return false;
		}

		// Token: 0x06005745 RID: 22341 RVA: 0x00280200 File Offset: 0x0027E400
		public static string GetFleetAlarmText(TIFactionState settingPlayer, TISpaceFleetState enemyFleet)
		{
			if (enemyFleet != null)
			{
				foreach (Alarm alarm in settingPlayer.alarms)
				{
					if (alarm.associatedGameState == enemyFleet && alarm.alarmType == AlarmType.FleetApproaching)
					{
						return Loc.T("UI.Alarm.FleetApproachingSummary", new object[]
						{
							alarm.alarmEvent.time.ToCustomTimeString(),
							alarm.alarmEvent.time.ToCustomDateString()
						});
					}
				}
			}
			return string.Empty;
		}

		// Token: 0x06005746 RID: 22342 RVA: 0x002802AC File Offset: 0x0027E4AC
		public void OnFleetAlarmTriggered(AlarmTriggered e)
		{
			this.alertFleetButtonText.SetText(SpaceObjectDetailController.GetFleetAlarmText(base.activePlayer, this.selectedSpaceObject.ref_fleet));
		}

		// Token: 0x06005747 RID: 22343 RVA: 0x002802D0 File Offset: 0x0027E4D0
		private void UpdatePlayerFleetTransferData(TISpaceFleetState playerFleet)
		{
			this.myFleetDeltaV.SetText(Loc.T("UI.Fleets.DVValue", new object[]
			{
				TIUtilities.FormatBigOrSmallNumber(playerFleet.currentDeltaV_kps, 1, 7, 0, false, false),
				TIUtilities.FormatBigOrSmallNumber(playerFleet.maxDeltaV_kps, 1, 7, 0, false, false)
			}));
			if (playerFleet.inTransfer)
			{
				this.transferProgressIcon.rectTransform.localPosition = new Vector2(this.myFleetTransferSliderZeroPoint + (float)(playerFleet.TrajectoryFractionCompleted() * (double)this.myFleetTransferSliderRange), this.transferProgressIcon.rectTransform.localPosition.y);
				SpaceObjectDetailController.UpdatePendingCombatIcon(this.transferPendingCombatIcon, playerFleet);
				int num = 0;
				using (IEnumerator<object> enumerator = this.myFleetShipsFullList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (SpaceObjectDetailController.<>o__502.<>p__0 == null)
						{
							SpaceObjectDetailController.<>o__502.<>p__0 = CallSite<Func<CallSite, object, ShipsInFleetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipsInFleetListItemController), typeof(SpaceObjectDetailController)));
						}
						SpaceObjectDetailController.<>o__502.<>p__0.Target(SpaceObjectDetailController.<>o__502.<>p__0, enumerator.Current).SetListItem(playerFleet.ships[num++]);
					}
				}
			}
		}

		// Token: 0x06005748 RID: 22344 RVA: 0x0028040C File Offset: 0x0027E60C
		private void UpdatePlayerFleetObjectCanvas(TISpaceFleetState playerFleet)
		{
			if (playerFleet != null && !playerFleet.archived && playerFleet.ships.Count > 0)
			{
				this.selectedAsset = playerFleet;
				this.myFleetName.SetText(playerFleet.GetDisplayName(base.activePlayer));
				this.myFleetDeltaV.SetText(Loc.T("UI.Fleets.DVValue", new object[]
				{
					TIUtilities.FormatBigOrSmallNumber(playerFleet.currentDeltaV_kps, 1, 7, 0, false, false),
					TIUtilities.FormatBigOrSmallNumber(playerFleet.maxDeltaV_kps, 1, 7, 0, false, false)
				}));
				this.myFleetSize.SetText(playerFleet.ships.Count.ToString("N0"));
				this.myFleetAcceleration.SetText(Loc.T(FleetsScreenController.accelerationStr((double)playerFleet.cruiseAcceleration_gs, false, false, false)));
				this.myFleetCombatScore.SetText(TIUtilities.FormatBigOrSmallNumber(playerFleet.SpaceCombatValue(), 1, 0, 0, false, false));
				this.myFleetAssaultScore.SetText(playerFleet.AssaultCombatValue(false).ToString("N0"));
				if (playerFleet.GetFlagship().FleetMissionControlMultiplier() < 1f)
				{
					this.myFleetMissionControlConsumption.SetText(TIUtilities.GreenLine(playerFleet.MissionControlConsumption().ToString("N0")));
				}
				else
				{
					this.myFleetMissionControlConsumption.SetText(playerFleet.MissionControlConsumption().ToString("N0"));
				}
				this.myFleetCouncilIcon.sprite = playerFleet.faction.factionIcon64;
				GameControl.assetLoader.LoadAssetForImageAssignment(playerFleet.faction.template.gradientPath, this.myFleetCouncilGradient);
				if (playerFleet.homeport != null)
				{
					this.myFleetHomeport.SetText(playerFleet.homeport.displayName);
					this.myFleetHomeportObject.SetActive(true);
				}
				else
				{
					this.myFleetHomeportObject.SetActive(false);
				}
				if (playerFleet.inTransfer)
				{
					this.transferOriginIcon.sprite = playerFleet.trajectory.originOrbit.barycenter.icon;
					this.o1 = playerFleet.trajectory.originOrbit;
					TISpaceFleetState destinationFleet = playerFleet.trajectory.destinationFleet;
					if (destinationFleet != null && destinationFleet.inTransfer)
					{
						this.transferDestinationIcon.sprite = playerFleet.trajectory.destinationFleet.icon;
						this.d1 = playerFleet.trajectory.destinationFleet;
						this.transferDestinationDetailIcon.enabled = false;
					}
					else
					{
						Trajectory trajectory = playerFleet.trajectory;
						if (trajectory.nextTrajectory != null)
						{
							trajectory = trajectory.nextTrajectory;
						}
						if (trajectory.endsInCrash)
						{
							this.transferDestinationIcon.sprite = trajectory.collisionTarget.icon;
							this.d1 = trajectory.collisionTarget;
							this.transferDestinationIcon.enabled = false;
						}
						else if (trajectory.exitsSolarSystem)
						{
							this.transferDestinationIcon.sprite = GameControl.assetLoader.LoadAsset<Sprite>("icons_2d/ICO_none");
						}
						else
						{
							if (TIGameState.Valid(trajectory.destinationOrbit))
							{
								this.transferDestinationIcon.sprite = trajectory.destinationOrbit.barycenter.icon;
								this.d1 = trajectory.destinationOrbit;
							}
							if (TIGameState.Valid(trajectory.destinationFleet))
							{
								this.transferDestinationDetailIcon.sprite = trajectory.destinationFleet.icon;
								this.d2 = trajectory.destinationFleet;
								this.transferDestinationDetailIcon.enabled = true;
							}
							else if (TIGameState.Valid(trajectory.destinationStation))
							{
								this.transferDestinationDetailIcon.sprite = trajectory.destinationStation.icon;
								this.d2 = trajectory.destinationStation;
								this.transferDestinationDetailIcon.enabled = true;
							}
							else
							{
								this.transferDestinationDetailIcon.enabled = false;
							}
						}
					}
					this.transferTextDetail.SetText(SpaceObjectDetailController.FleetTransferTwoLiner(playerFleet, false));
					this.transferProgressIcon.rectTransform.localPosition = new Vector2(this.myFleetTransferSliderZeroPoint + (float)(playerFleet.TrajectoryFractionCompleted() * (double)this.myFleetTransferSliderRange), this.transferProgressIcon.rectTransform.localPosition.y);
					SpaceObjectDetailController.UpdatePendingCombatIcon(this.transferPendingCombatIcon, playerFleet);
					this.transferObject.SetActive(true);
					this.genericOpDetailObject.SetActive(false);
				}
				else
				{
					this.genericOpLine1.SetText(playerFleet.GetLocationDescription(base.activePlayer, true, true));
					if (playerFleet.CurrentOperations().Count > 0)
					{
						TIOrbitState ref_orbit = playerFleet.ref_orbit;
						this.o1 = ((ref_orbit != null) ? ref_orbit.barycenter : null) ?? playerFleet;
						OperationData operationData = playerFleet.CurrentOperations()[0];
						GameControl.assetLoader.LoadAssetForImageAssignment(operationData.operation.GetOperationIconImagePath_Off(), this.genericOpImage);
						StringBuilder stringBuilder = new StringBuilder(operationData.operation.GetDisplayName());
						if (operationData.target != null && operationData.target != playerFleet)
						{
							stringBuilder.Append(" / ").Append(operationData.target.GetDisplayName(base.activePlayer));
						}
						if (operationData.completionDate != null)
						{
							stringBuilder.Append(" / ").Append(operationData.completionDate.ToCustomDateString());
						}
						this.genericOpLine2.SetText(stringBuilder.ToString());
						this.genericOpSmallImage.enabled = false;
						if (playerFleet.bombarding)
						{
							this.o2 = playerFleet.bombardmentTarget.ref_spaceBody;
							if (this.o2 != null)
							{
								this.genericOpSmallImage.sprite = playerFleet.bombardmentTarget.ref_spaceBody.icon;
								this.genericOpSmallImage.enabled = true;
							}
							else
							{
								this.genericOpSmallImage.enabled = false;
							}
						}
					}
					else
					{
						this.o1 = playerFleet.ref_naturalSpaceObject;
						this.genericOpImage.sprite = playerFleet.ref_naturalSpaceObject.icon;
						if (playerFleet.dockedOrLanded && !playerFleet.landedInOutback)
						{
							this.o2 = playerFleet.ref_hab;
							this.genericOpSmallImage.sprite = playerFleet.ref_hab.icon;
							this.genericOpSmallImage.enabled = true;
						}
						else
						{
							this.genericOpSmallImage.enabled = false;
						}
						if (playerFleet.unavailableForOperations)
						{
							if (Mathd.Abs(playerFleet.returnToOperationsTime.DifferenceInDays(TITimeState.Now())) >= 1.0)
							{
								this.genericOpLine2.SetText(Loc.T("UI.Space.Fleet.Unavailable", new object[] { playerFleet.returnToOperationsTime.ToCustomDateString() }));
							}
							else
							{
								this.genericOpLine2.SetText(Loc.T("UI.Space.Fleet.Unavailable", new object[] { playerFleet.returnToOperationsTime.ToShortTimeString() }));
							}
						}
						else
						{
							this.genericOpLine2.SetText(Loc.T("UI.Space.NoOp"));
						}
					}
					this.transferObject.SetActive(false);
					this.fleetStandingOrderObject.SetActive(playerFleet.huntingXenofauna);
					this.genericOpDetailObject.SetActive(true);
				}
				if (playerFleet.dockedAtHab && playerFleet.dockedLocation.ref_hab.CompletedShipyards().Count > 0 && playerFleet.dockedLocation.ref_hab.faction == base.activePlayer)
				{
					bool flag = false;
					foreach (TISpaceShipState tispaceShipState in playerFleet.ships)
					{
						flag = (tispaceShipState.CanRefit || tispaceShipState.NeedsRefit) && !base.activePlayer.obsoleteShipDesigns.Contains(tispaceShipState.BestExistingRefit.dataName);
						if (flag)
						{
							break;
						}
					}
					this.myFleetRefitButton.gameObject.SetActive(flag);
					this.validRefitFleet = (flag ? playerFleet : null);
				}
				else
				{
					this.myFleetRefitButton.gameObject.SetActive(false);
					this.validRefitFleet = null;
				}
				this.myFleetShipsGridList.SetListSize<FleetShipGridItemController>(Mathf.Min(playerFleet.ships.Count, 20), false, false);
				int num = 0;
				using (IEnumerator<object> enumerator2 = this.myFleetShipsGridList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (SpaceObjectDetailController.<>o__503.<>p__0 == null)
						{
							SpaceObjectDetailController.<>o__503.<>p__0 = CallSite<Func<CallSite, object, FleetShipGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FleetShipGridItemController), typeof(SpaceObjectDetailController)));
						}
						SpaceObjectDetailController.<>o__503.<>p__0.Target(SpaceObjectDetailController.<>o__503.<>p__0, enumerator2.Current).SetGridItem((num >= 20 && playerFleet.ships.Count > 20) ? null : playerFleet.ships[num++], playerFleet, true);
					}
				}
				this.myFleetShipsFullList.SetListSize<ShipsInFleetListItemController>(playerFleet.ships.Count, false, false);
				this.shipListTransform.sizeDelta = new Vector2(this.shipListTransform.sizeDelta.x, Mathf.Min(382f * ((TIPlayerProfileManager.uiScaleSetting > 0) ? 0.86f : 1f), (float)(32 + playerFleet.ships.Count * 47)));
				num = 0;
				using (IEnumerator<object> enumerator2 = this.myFleetShipsFullList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (SpaceObjectDetailController.<>o__503.<>p__1 == null)
						{
							SpaceObjectDetailController.<>o__503.<>p__1 = CallSite<Func<CallSite, object, ShipsInFleetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipsInFleetListItemController), typeof(SpaceObjectDetailController)));
						}
						SpaceObjectDetailController.<>o__503.<>p__1.Target(SpaceObjectDetailController.<>o__503.<>p__1, enumerator2.Current).SetListItem(playerFleet.ships[num++]);
					}
				}
				List<CouncilorView> list = playerFleet.CouncilorViewsPresentAndKnownToFaction(base.activePlayer);
				this.myFleetCouncilorsGrid.SetListSize<CombatShipCouncilorGridItemController>(list.Count, false, false);
				num = 0;
				using (IEnumerator<object> enumerator2 = this.myFleetCouncilorsGrid.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (SpaceObjectDetailController.<>o__503.<>p__2 == null)
						{
							SpaceObjectDetailController.<>o__503.<>p__2 = CallSite<Func<CallSite, object, CombatShipCouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombatShipCouncilorGridItemController), typeof(SpaceObjectDetailController)));
						}
						SpaceObjectDetailController.<>o__503.<>p__2.Target(SpaceObjectDetailController.<>o__503.<>p__2, enumerator2.Current).SetGridItem(list[num++]);
					}
				}
				base.canvasManager.ActiveAssetPanelResized(this.upperPanelTransform.sizeDelta.y + (this.playerFleetShipListOpen ? this.shipListTransform.sizeDelta.y : 0f));
				return;
			}
			base.canvasManager.SetActiveAssetPanel(AssetPanel.None, 0f);
		}

		// Token: 0x06005749 RID: 22345 RVA: 0x00280E98 File Offset: 0x0027F098
		public void RefitMyFleet()
		{
			if (this.validRefitFleet == null)
			{
				this.myFleetRefitButton.gameObject.SetActive(false);
				return;
			}
			FleetsScreenController.gotoConstructionManager = true;
			GeneralControlsController.Singleton.Fleets();
			FleetsScreenController infoScreen = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<FleetsScreenController>();
			if (infoScreen != null)
			{
				infoScreen.ShowRefitTabWithFleetSelection(this.validRefitFleet);
			}
		}

		// Token: 0x0600574A RID: 22346 RVA: 0x00280EFC File Offset: 0x0027F0FC
		public static string FleetTransferTwoLiner(TISpaceFleetState fleet, bool obfuscate)
		{
			if (fleet.trajectory.exitsSolarSystem)
			{
				return Loc.T("UI.Space.Fleet.LeavingSolarSystemWithDate", new object[] { fleet.trajectory.finalArrivalTime.ToCustomTimeDateString() });
			}
			if (fleet.trajectory.endsInCrash)
			{
				return Loc.T("UI.Space.Fleet.CrashingWithDate", new object[]
				{
					fleet.GetLocationDescription(GameControl.control.activePlayer, true, true),
					fleet.trajectory.finalArrivalTime.ToCustomTimeDateString()
				});
			}
			TimeSpan timeSpan = fleet.trajectory.finalArrivalTime - TITimeState.Now();
			string text;
			if (timeSpan.TotalDays >= 1.0)
			{
				text = Loc.T("UI.Operations.Duration_days", new object[] { timeSpan.TotalDays.ToString("N1") });
			}
			else if (timeSpan.TotalHours >= 1.0)
			{
				text = Loc.T("UI.Operations.Duration_hours", new object[] { timeSpan.TotalHours.ToString("N1") });
			}
			else
			{
				text = Loc.T("UI.Operations.Duration_minutes", new object[] { timeSpan.TotalMinutes.ToString("N2") });
			}
			return Loc.T("UI.Space.Fleet.Arrival", new object[]
			{
				fleet.GetLocationDescription(GameControl.control.activePlayer, true, true),
				fleet.trajectory.finalArrivalTime.ToCustomTimeDateString(),
				text
			});
		}

		// Token: 0x0600574B RID: 22347 RVA: 0x00281074 File Offset: 0x0027F274
		public void OnPressPlayerFleetShipsDetailButton()
		{
			this.playerFleetShipListOpen = !this.playerFleetShipListOpen;
			if (this.playerFleetShipListOpen)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenFinder", false, false);
				this.shipListObject.SetActive(true);
				this.pusherTransform.sizeDelta = new Vector2(this.pusherTransform.sizeDelta.x, (float)Mathf.Min(465, this.selectedAsset.ships.Count * 50));
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseFinder", false, false);
				this.shipListObject.SetActive(false);
				this.pusherTransform.sizeDelta = new Vector2(this.pusherTransform.sizeDelta.x, 465f);
			}
			base.canvasManager.ActiveAssetPanelResized(this.upperPanelTransform.sizeDelta.y + (this.playerFleetShipListOpen ? this.shipListTransform.sizeDelta.y : 0f));
		}

		// Token: 0x0600574C RID: 22348 RVA: 0x00281168 File Offset: 0x0027F368
		public void CycleSiteHeader()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.showSiteNames = !this.showSiteNames;
			this.sitesHeaderName.SetText(Loc.T(this.showSiteNames ? "UI.Space.Sites.Header.SiteName" : "UI.Space.Sites.Header.BaseName"));
			using (IEnumerator<object> enumerator = this.siteList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__507.<>p__0 == null)
					{
						SpaceObjectDetailController.<>o__507.<>p__0 = CallSite<Func<CallSite, object, BaseSiteListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(BaseSiteListItemController), typeof(SpaceObjectDetailController)));
					}
					BaseSiteListItemController baseSiteListItemController = SpaceObjectDetailController.<>o__507.<>p__0.Target(SpaceObjectDetailController.<>o__507.<>p__0, enumerator.Current);
					if (baseSiteListItemController.gameObject.activeInHierarchy)
					{
						baseSiteListItemController.SetSiteNameText(this.showSiteNames);
					}
				}
			}
		}

		// Token: 0x0600574D RID: 22349 RVA: 0x00281248 File Offset: 0x0027F448
		public void OnMyFleetImageClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyFleetSelect", false, false);
			GameControl.eventManager.TriggerEvent(new FleetDetailRequested(this.UISelectedAssetState.ref_fleet), null, Array.Empty<object>());
		}

		// Token: 0x0600574E RID: 22350 RVA: 0x00281276 File Offset: 0x0027F476
		public void OnMyFleetTransferOriginClicked()
		{
			if (this.o1 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.o1, false, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x0600574F RID: 22351 RVA: 0x002812B4 File Offset: 0x0027F4B4
		public void OnMyFleetDestinationClicked()
		{
			if (this.d1 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.d1, false, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005750 RID: 22352 RVA: 0x002812F2 File Offset: 0x0027F4F2
		public void OnMyFleetSpecificDestinationClicked()
		{
			if (this.d2 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.d2, false, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005751 RID: 22353 RVA: 0x00281330 File Offset: 0x0027F530
		public void OnMyFleetOpLocationClicked()
		{
			if (this.o1 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.o1, true, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005752 RID: 22354 RVA: 0x0028136E File Offset: 0x0027F56E
		public void OnMyFleetSmallOpLocationClicked()
		{
			if (this.o2 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				TIUtilities.GotoGameState(this.o2, true, true, true, true, false, -1f);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005753 RID: 22355 RVA: 0x002813AC File Offset: 0x0027F5AC
		public void OnMyHomeportClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
			bool flag = GeneralControlsController.CurrentlyTargetingStateType(typeof(TIHabState));
			TIUtilities.GotoGameState(this.UISelectedAssetState.ref_fleet.homeport, !flag, true, true, !flag, !flag, -1f);
		}

		// Token: 0x06005754 RID: 22356 RVA: 0x00281400 File Offset: 0x0027F600
		private void UpdateNaturalSpaceBodyCanvasTransientData(TISpaceBodyState spaceBody)
		{
			this.SetPlanetTag();
			List<TIHabState> list = (from z in spaceBody.stationsInOrbit
				where z.VisibleToFaction(this.activePlayer)
				select z into q
				orderby q.altitude
				select q).ToList<TIHabState>();
			List<TISpaceFleetState> list2 = (from z in spaceBody.fleetsInOrbit
				where z.VisibleToFaction(this.activePlayer)
				select z into q
				orderby q.semiMajorAxis_km
				select q).Union<TISpaceFleetState>(spaceBody.landedFleets.Where<TISpaceFleetState>((TISpaceFleetState z) => z.VisibleToFaction(this.activePlayer))).ToList<TISpaceFleetState>();
			if (spaceBody.naturalSatellites.Count > 0)
			{
				List<TISpaceFleetState> list3 = new List<TISpaceFleetState>();
				list3.AddRange(from z in spaceBody.naturalSatellites.SelectMany<TISpaceBodyState, TISpaceFleetState>((TISpaceBodyState x) => x.fleetsInOrbit.Union<TISpaceFleetState>(x.landedFleets))
					where z.VisibleToFaction(this.activePlayer)
					select z);
				list3.AddRange(from z in spaceBody.lagrangePoints.SelectMany<TILagrangePointState, TISpaceFleetState>((TILagrangePointState x) => x.fleetsInOrbit)
					where z.VisibleToFaction(this.activePlayer)
					select z);
				list3 = (from x in list3
					orderby x.ref_naturalSpaceObject.semiMajorAxis_km, x.semiMajorAxis_km
					select x).ToList<TISpaceFleetState>();
				list2.AddRange(list3);
				List<TIHabState> list4 = new List<TIHabState>();
				list4.AddRange(from z in spaceBody.naturalSatellites.SelectMany<TISpaceBodyState, TIHabState>((TISpaceBodyState x) => x.stationsInOrbit)
					where z.VisibleToFaction(this.activePlayer)
					select z);
				list4.AddRange(from z in spaceBody.lagrangePoints.SelectMany<TILagrangePointState, TIHabState>((TILagrangePointState x) => x.stationsInOrbit)
					where z.VisibleToFaction(this.activePlayer)
					select z);
				list4 = (from x in list4
					orderby x.ref_naturalSpaceObject.semiMajorAxis_km, x.semiMajorAxis_km
					select x).ToList<TIHabState>();
				list.AddRange(list4);
			}
			List<TICouncilorState> list5 = (from x in GameControl.control.activePlayer.CurrentKnownCouncilors(false, null, false, true)
				where x.location.ref_spaceBody == spaceBody
				select x).ToList<TICouncilorState>();
			List<TIOrbitState> list6 = spaceBody.orbits.Where<TIOrbitState>((TIOrbitState x) => !x.isAdHocOrbit).ToList<TIOrbitState>();
			List<TISpaceBodyState> list7 = spaceBody.naturalSatellites.OrderBy<TISpaceBodyState, double>((TISpaceBodyState q) => q.semiMajorAxis_km).ToList<TISpaceBodyState>();
			if (list5.Count == 0)
			{
				this.councilorsButton.gameObject.SetActive(false);
				if (this.naturalBodyTabManager.activeTab == this.councilorsTab)
				{
					this.naturalBodyTabManager.Toggle(this.naturalBodyTabManager.activeTab);
					this.naturalBodyTabManager.ClearActiveTab();
				}
			}
			else
			{
				this.councilorsTabHeader.SetText(Loc.T("UI.Space.CouncilorsTabHeader", new object[] { list5.Count.ToString() }));
				this.councilorsButton.gameObject.SetActive(true);
				this.UpdateCouncilorsList(this.councilorList, list5, null, false);
				this.councilorsTab.SetSize(30f, 27f, 23f, list5.Count);
			}
			if (list2.Count == 0)
			{
				this.fleetsButton.gameObject.SetActive(false);
				if (this.naturalBodyTabManager.activeTab == this.fleetsTab)
				{
					this.naturalBodyTabManager.Toggle(this.naturalBodyTabManager.activeTab);
					this.naturalBodyTabManager.ClearActiveTab();
				}
			}
			else
			{
				if (list2.Count == 1)
				{
					this.fleetsTabHeader.SetText(Loc.T("UI.Space.FleetsTabHeader_One"));
				}
				else
				{
					this.fleetsTabHeader.SetText(Loc.T("UI.Space.FleetsTabHeader", new object[] { list2.Count.ToString() }));
				}
				this.fleetsButton.gameObject.SetActive(true);
				this.UpdateFleetsList(this.fleetList, list2);
				this.fleetsTab.SetSize(30f, 27f, 23f, list2.Count);
			}
			if (list.Count == 0)
			{
				this.stationsButton.gameObject.SetActive(false);
				if (this.naturalBodyTabManager.activeTab == this.stationsTab)
				{
					this.naturalBodyTabManager.Toggle(this.naturalBodyTabManager.activeTab);
					this.naturalBodyTabManager.ClearActiveTab();
				}
			}
			else
			{
				if (list.Count == 1)
				{
					this.stationsTabHeader.SetText(Loc.T("UI.Space.StationsTabHeader_One"));
				}
				else
				{
					this.stationsTabHeader.SetText(Loc.T("UI.Space.StationsTabHeader", new object[] { list.Count.ToString() }));
				}
				this.stationsButton.gameObject.SetActive(true);
				this.UpdateStationsList(this.stationList, list);
				this.stationsTab.SetSize(30f, 27f, 23f, list.Count);
			}
			if (spaceBody.habSites.Length != 0)
			{
				this.UpdateSiteList(spaceBody.habSites.ToList<TIHabSiteState>());
				this.sitesTab.SetSize(30f, 27f, 23f, spaceBody.habSites.Length);
				this.sitesHeaderName.SetText(Loc.T(this.showSiteNames ? "UI.Space.Sites.Header.SiteName" : "UI.Space.Sites.Header.BaseName"));
			}
			if (list6.Count == 0)
			{
				this.orbitsButton.gameObject.SetActive(false);
				if (this.naturalBodyTabManager.activeTab == this.orbitsTab)
				{
					this.naturalBodyTabManager.Toggle(this.naturalBodyTabManager.activeTab);
					this.naturalBodyTabManager.ClearActiveTab();
				}
			}
			else
			{
				if (list6.Count == 1)
				{
					this.orbitsTabHeader.SetText(Loc.T("UI.Space.OrbitsTabHeader_One"));
				}
				else
				{
					this.orbitsTabHeader.SetText(Loc.T("UI.Space.OrbitsTabHeader", new object[] { list6.Count.ToString() }));
				}
				this.orbitsButton.gameObject.SetActive(true);
				this.UpdateOrbitsList(this.orbitsList, list6);
				this.orbitsTab.SetSize(30f, 27f, 23f, list6.Count);
			}
			if (list7.Count > 0)
			{
				this.UpdateMoonList(spaceBody, list7);
				this.moonsTab.SetSize(30f, 27f, 23f, list7.Count);
			}
			if (this.naturalBodyTabManager.activeTab != null)
			{
				this.naturalBodyTabManager.activeTab.UpdateSize();
			}
			this.UpdateNaturalSpaceObjectLaunchWindowData(spaceBody, this.naturalSpaceBodyLaunchWindow);
		}

		// Token: 0x06005755 RID: 22357 RVA: 0x00281BB4 File Offset: 0x0027FDB4
		private void UpdateNaturalSpaceObjectLaunchWindowData(TINaturalSpaceObjectState spaceObject, TMP_Text text)
		{
			if (!spaceObject.isEarth && !spaceObject.barycenter.isEarth)
			{
				TILagrangePointState ref_lagrangePoint = spaceObject.ref_lagrangePoint;
				if (ref_lagrangePoint == null || !ref_lagrangePoint.secondaryObject.isEarth)
				{
					double num;
					TIDateTime nextHohmannLaunchWindowDate = TINaturalSpaceObjectState.GetNextHohmannLaunchWindowDate(base.activePlayer, GameStateManager.Earth(), spaceObject, TITimeState.Now(), out num);
					bool flag;
					double hohmannTimePenaltyFraction = TISpaceObjectState.GetHohmannTimePenaltyFraction(base.activePlayer, nextHohmannLaunchWindowDate, num, out flag);
					if (hohmannTimePenaltyFraction < 0.03)
					{
						text.SetText(TIUtilities.GreenLine(Loc.T("UI.Space.EarthLaunchWindowNow")));
						return;
					}
					text.SetText(Loc.T("UI.Space.EarthLaunchWindow", new object[]
					{
						nextHohmannLaunchWindowDate.ToCustomDateString(),
						hohmannTimePenaltyFraction.ToPercent("P0"),
						flag ? TemplateManager.global.upRedArrowInlineSpritePath : TemplateManager.global.downGreenArrowInlineSpritePath
					}));
					return;
				}
			}
			text.SetText(string.Empty);
		}

		// Token: 0x06005756 RID: 22358 RVA: 0x00281C90 File Offset: 0x0027FE90
		public static string SetTimePenaltyTip(TINaturalSpaceObjectState naturalObject)
		{
			double num;
			TIDateTime nextHohmannLaunchWindowDate = TINaturalSpaceObjectState.GetNextHohmannLaunchWindowDate(GameControl.control.activePlayer, GameStateManager.Earth(), naturalObject, TITimeState.Now(), out num);
			bool flag;
			double hohmannTimePenaltyFraction = TISpaceObjectState.GetHohmannTimePenaltyFraction(GameControl.control.activePlayer, nextHohmannLaunchWindowDate, num, out flag);
			return Loc.T("UI.Space.TimePenalty", new object[]
			{
				naturalObject.displayName,
				nextHohmannLaunchWindowDate.ToCustomDateString(),
				hohmannTimePenaltyFraction.ToPercent("P0"),
				TemplateManager.global.downGreenArrowInlineSpritePath,
				TemplateManager.global.upRedArrowInlineSpritePath
			});
		}

		// Token: 0x06005757 RID: 22359 RVA: 0x00281D18 File Offset: 0x0027FF18
		private void UpdateNaturalSpaceBodyCanvas(TISpaceBodyState spaceBody)
		{
			this.naturalBodyTabContainer.SetActive(true);
			this.spaceBodyName.SetText(spaceBody.displayName);
			TIHabSiteState[] habSites = spaceBody.habSites;
			this.naturalSpaceBodyQuickDescription.SetText(spaceBody.template.descriptor1);
			this.naturalSpaceBodyMiningProfile.SetText(spaceBody.GetMiningPotentialString());
			this.naturalSpaceBodyDiameter.SetText(SpaceObjectDetailController.SpaceBodyDiameterText(spaceBody));
			this.naturalSpaceBodyOrbit.SetText(SpaceObjectDetailController.OrbitAxisText(spaceBody));
			if (spaceBody.isEarth)
			{
				TMP_Text tmp_Text = this.naturalSpaceBodyLaunchWindow;
				string text = "UI.Space.Masskg";
				object[] array = new object[1];
				int num = 0;
				double num2 = spaceBody.mass_kg;
				array[num] = num2.ToString("E2");
				tmp_Text.SetText(Loc.T(text, array));
				this.naturalSpaceBodySurfaceGravity.SetText(new StringBuilder(TIGlobalConfig.globalConfig.gravityInlineSpritePath).Append(" ").Append(Loc.T("UI.Space.gravg", new object[] { "1" })));
			}
			else
			{
				TMP_Text tmp_Text2 = this.naturalSpaceBodySurfaceGravity;
				StringBuilder stringBuilder = new StringBuilder(TIGlobalConfig.globalConfig.gravityInlineSpritePath).Append(" ");
				string text2 = "UI.Space.gravg";
				object[] array2 = new object[1];
				int num3 = 0;
				double num2 = spaceBody.surfaceGravity_g;
				array2[num3] = num2.ToString((spaceBody.surfaceGravity_g > 0.02) ? "N2" : "N4");
				tmp_Text2.SetText(stringBuilder.Append(Loc.T(text2, array2)));
			}
			this.naturalSpaceBodyEscapeVelocity.SetText(Loc.T("UI.Space.kps", new object[] { TIUtilities.FormatSmallNumber(spaceBody.escapeVelocity_kps, 3, 0, true, false) }));
			this.naturalSpaceBodyOrbitPeriod.SetText(SpaceObjectDetailController.OrbitPeriodText(spaceBody));
			if (!base.activePlayer.Prospected(spaceBody))
			{
				if (base.activePlayer.ProspectorEnRoute(spaceBody))
				{
					TMP_Text tmp_Text3 = this.probeArrivalDate;
					string text3 = "UI.Space.ProbeArrival";
					object[] array3 = new object[1];
					int num4 = 0;
					TIDateTime tidateTime = base.activePlayer.ProspectorArrival(spaceBody);
					array3[num4] = ((tidateTime != null) ? tidateTime.ToCustomDateString() : null) ?? "ERROR";
					tmp_Text3.SetText(Loc.T(text3, array3));
					this.probeDataPanel.SetActive(true);
					this.probeIcon.sprite = AssetCacheManager.prospectingUnderway;
				}
				else
				{
					bool flag = false;
					foreach (TISpaceFleetState tispaceFleetState in base.activePlayer.fleets)
					{
						foreach (OperationData operationData in tispaceFleetState.CurrentOperations())
						{
							if (operationData.target == spaceBody && operationData.operationDataName == typeof(SurveyPlanetFromFleetOperation).ToString())
							{
								flag = true;
								this.probeArrivalDate.SetText(Loc.T("UI.Space.ProbeArrival", new object[] { operationData.completionDate.ToCustomDateString() }));
								this.probeIcon.sprite = AssetCacheManager.prospectingUnderway;
								this.probeDataPanel.SetActive(true);
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
					if (!flag)
					{
						if (base.activePlayer.CanExplore(spaceBody) && base.activePlayer.AlienTerritoryToAvoid(spaceBody))
						{
							this.probeIcon.sprite = GameStateManager.AlienFaction().factionIcon64;
							this.probeArrivalDate.SetText(Loc.T("UI.Space.AlienTerritoryShort"));
							this.probeDataPanel.SetActive(true);
						}
						else
						{
							this.probeDataPanel.SetActive(false);
						}
					}
				}
			}
			else
			{
				this.probeIcon.sprite = AssetCacheManager.prospectedIcon;
				this.probeArrivalDate.SetText(string.Empty);
				this.probeDataPanel.SetActive(true);
			}
			ulong population = spaceBody.population;
			if (population > 0UL)
			{
				this.populationValue.SetText(Loc.T("UI.Space.Population", new object[]
				{
					TIGlobalConfig.globalConfig.populationInlineSpritePath,
					population.ToString("N0")
				}));
				this.populationPanel.SetActive(true);
			}
			else
			{
				this.populationPanel.SetActive(false);
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(spaceBody.GetMaxTierIconPath(), this.maxTierIcon);
			this.maxTierIcon.gameObject.SetActive(true);
			if (spaceBody.isaMoon)
			{
				this.gotoParentButton.SetActive(true);
				this.parentIcon.sprite = spaceBody.barycenter.icon;
			}
			else
			{
				this.gotoParentButton.SetActive(false);
			}
			if (spaceBody.naturalSatellites.Count > 0)
			{
				this.satellitesButton.gameObject.SetActive(true);
				if (spaceBody.naturalSatellites.Count == 1)
				{
					this.satellitesTabHeader.SetText(Loc.T("UI.Space.SatellitesTabHeader_One", new object[] { spaceBody.naturalSatellites.Count.ToString() }));
				}
				else
				{
					this.satellitesTabHeader.SetText(Loc.T("UI.Space.SatellitesTabHeader", new object[] { spaceBody.naturalSatellites.Count.ToString() }));
				}
			}
			else
			{
				this.satellitesButton.gameObject.SetActive(false);
				if (this.naturalBodyTabManager.activeTab == this.moonsTab)
				{
					this.naturalBodyTabManager.Toggle(this.naturalBodyTabManager.activeTab);
					this.naturalBodyTabManager.ClearActiveTab();
				}
			}
			if (habSites.Length == 0)
			{
				this.baseSitesButton.gameObject.SetActive(false);
				if (this.naturalBodyTabManager.activeTab == this.sitesTab)
				{
					this.naturalBodyTabManager.Toggle(this.naturalBodyTabManager.activeTab);
					this.naturalBodyTabManager.ClearActiveTab();
				}
				this.waterPotentialObject.SetActive(false);
				this.volatilesPotentialObject.SetActive(false);
				this.metalsPotentialObject.SetActive(false);
				this.noblesPotentialObject.SetActive(false);
				this.fissilesPotentialObject.SetActive(false);
			}
			else
			{
				if (habSites.Length == 1)
				{
					this.baseSitesTabHeader.SetText(Loc.T("UI.Space.BaseSitesTabHeader_One", new object[] { habSites.Length.ToString() }));
				}
				else
				{
					this.baseSitesTabHeader.SetText(Loc.T("UI.Space.BaseSitesTabHeader", new object[] { habSites.Length.ToString() }));
				}
				this.baseSitesButton.gameObject.SetActive(true);
				bool flag2 = base.activePlayer.Prospected(spaceBody);
				GameControl.assetLoader.LoadAssetForImageAssignment(spaceBody.GetProfileRatingIconPath(FactionResource.Water, false, flag2), this.waterPotential);
				GameControl.assetLoader.LoadAssetForImageAssignment(spaceBody.GetProfileRatingIconPath(FactionResource.Volatiles, false, flag2), this.volatilesPotential);
				GameControl.assetLoader.LoadAssetForImageAssignment(spaceBody.GetProfileRatingIconPath(FactionResource.Metals, false, flag2), this.metalsPotential);
				GameControl.assetLoader.LoadAssetForImageAssignment(spaceBody.GetProfileRatingIconPath(FactionResource.NobleMetals, false, flag2), this.noblesPotential);
				GameControl.assetLoader.LoadAssetForImageAssignment(spaceBody.GetProfileRatingIconPath(FactionResource.Fissiles, false, flag2), this.fissilesPotential);
				this.waterPotentialObject.SetActive(true);
				this.volatilesPotentialObject.SetActive(true);
				this.metalsPotentialObject.SetActive(true);
				this.noblesPotentialObject.SetActive(true);
				this.fissilesPotentialObject.SetActive(true);
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(spaceBody.AtmosphereIconPath(), this.atmoIcon);
			this.atmoTip.SetDelegate("BodyText", () => spaceBody.AtmosphereDescription());
			GameControl.assetLoader.LoadAssetForImageAssignment(spaceBody.SolarInsolationIconPath(false), this.solarPotential);
			this.UpdateNaturalSpaceBodyCanvasTransientData(spaceBody);
			this.habSiteHohmannTip.SetDelegate("BodyText", () => SpaceObjectDetailController.SetTimePenaltyTip(spaceBody));
		}

		// Token: 0x06005758 RID: 22360 RVA: 0x0028256C File Offset: 0x0028076C
		private void AddClickedPreviousSpacebody(TISpaceObjectState spaceObject)
		{
			this.previousSpacebodies.Add(spaceObject);
			if (this.previousSpacebodies.Count > 11)
			{
				this.previousSpacebodies.RemoveAt(0);
			}
		}

		// Token: 0x06005759 RID: 22361 RVA: 0x00282598 File Offset: 0x00280798
		public void OnClickNaturalSpacebodyBack()
		{
			if ((this.previousSpacebodies != null) & (this.previousSpacebodies.Count > 1))
			{
				TISpaceObjectState tispaceObjectState = this.previousSpacebodies[this.previousSpacebodies.Count - 2];
				if (TIGameState.Valid(tispaceObjectState))
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
					this.ViewSpaceObject(tispaceObjectState, false);
				}
				this.previousSpacebodies.Remove(tispaceObjectState);
			}
			this.UpdateSpaceObjectBackButtons();
		}

		// Token: 0x0600575A RID: 22362 RVA: 0x00282608 File Offset: 0x00280808
		private void UpdateSpaceObjectBackButtons()
		{
			if (this.previousSpacebodies == null || this.previousSpacebodies.Count < 2)
			{
				this.backButtonNaturalSpacebodyDetail.SetActive(false);
				this.backButtonEnemyFleetDetail.SetActive(false);
				this.backButtonHabDetail.SetActive(false);
				this.backButtonLagrangeDetail.SetActive(false);
				return;
			}
			this.backButtonNaturalSpacebodyDetail.SetActive(true);
			this.backButtonEnemyFleetDetail.SetActive(true);
			this.backButtonHabDetail.SetActive(true);
			this.backButtonLagrangeDetail.SetActive(true);
		}

		// Token: 0x0600575B RID: 22363 RVA: 0x0028268C File Offset: 0x0028088C
		private void SetPlanetTag()
		{
			switch (this.selectedSpaceObject.ref_spaceBody.playerTag)
			{
			case PlayerTag.Red:
				this.playerTagButtonImage.color = SpaceObjectSymbolController.PlanetTagRed;
				return;
			case PlayerTag.Green:
				this.playerTagButtonImage.color = SpaceObjectSymbolController.PlanetTagGreen;
				return;
			}
			this.playerTagButtonImage.color = Color.white;
		}

		// Token: 0x0600575C RID: 22364 RVA: 0x002826F0 File Offset: 0x002808F0
		public void OnClickPlanetTag()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			int num = (int)(this.selectedSpaceObject.ref_spaceBody.playerTag + 1);
			PlayerTag[] array = (PlayerTag[])Enum.GetValues(typeof(PlayerTag));
			if (num >= array.Length)
			{
				num = 0;
			}
			base.activePlayer.playerControl.StartAction(new SetPlanetTagAction(this.selectedSpaceObject.ref_spaceBody, array[num]));
			this.SetPlanetTag();
		}

		// Token: 0x0600575D RID: 22365 RVA: 0x00282764 File Offset: 0x00280964
		private void UpdateLagrangePointCanvasTransientData(TILagrangePointState lagrangePoint)
		{
			List<TIHabState> list = (from z in lagrangePoint.stationsInOrbit
				where z.VisibleToFaction(this.activePlayer)
				select z into q
				orderby q.altitude
				select q).ToList<TIHabState>();
			List<TISpaceFleetState> list2 = (from z in lagrangePoint.fleetsInOrbit
				where z.VisibleToFaction(this.activePlayer)
				select z into q
				orderby q.semiMajorAxis_km
				select q).ToList<TISpaceFleetState>();
			List<TICouncilorState> list3 = (from x in GameControl.control.activePlayer.CurrentKnownCouncilors(false, null, false, true)
				where x.location.ref_naturalSpaceObject == lagrangePoint
				select x).ToList<TICouncilorState>();
			List<TIOrbitState> list4 = lagrangePoint.orbits.Where<TIOrbitState>((TIOrbitState x) => !x.isAdHocOrbit).ToList<TIOrbitState>();
			if (list.Count == 0)
			{
				this.lagrangeStationsTabButton.gameObject.SetActive(false);
				if (this.lagrangeTabbedPaneManager.activeTab == this.lagrangeStationsTab)
				{
					this.lagrangeTabbedPaneManager.Toggle(this.lagrangeTabbedPaneManager.activeTab);
					this.lagrangeTabbedPaneManager.ClearActiveTab();
				}
			}
			else
			{
				if (list.Count == 1)
				{
					this.lagrangeStationsTabHeader.SetText(Loc.T("UI.Space.StationsTabHeader_One"));
				}
				else
				{
					this.lagrangeStationsTabHeader.SetText(Loc.T("UI.Space.StationsTabHeader", new object[] { list.Count.ToString() }));
				}
				this.lagrangeStationsTabButton.gameObject.SetActive(true);
				this.UpdateStationsList(this.lagrangeStationsList, list);
				this.lagrangeStationsTab.SetSize(30f, 27f, 23f, list.Count);
			}
			if (list2.Count == 0)
			{
				this.lagrangeFleetsTabButton.gameObject.SetActive(false);
				if (this.lagrangeTabbedPaneManager.activeTab == this.lagrangeFleetsTab)
				{
					this.lagrangeTabbedPaneManager.Toggle(this.lagrangeTabbedPaneManager.activeTab);
					this.lagrangeTabbedPaneManager.ClearActiveTab();
				}
			}
			else
			{
				if (list2.Count == 1)
				{
					this.lagrangeFleetsTabHeader.SetText(Loc.T("UI.Space.FleetsTabHeader_One"));
				}
				else
				{
					this.lagrangeFleetsTabHeader.SetText(Loc.T("UI.Space.FleetsTabHeader", new object[] { list2.Count.ToString() }));
				}
				this.lagrangeFleetsTabButton.gameObject.SetActive(true);
				this.UpdateFleetsList(this.lagrangeFleetsList, list2);
				this.lagrangeFleetsTab.SetSize(30f, 27f, 23f, list2.Count);
			}
			if (list3.Count == 0)
			{
				this.lagrangeCouncilorsTabButton.gameObject.SetActive(false);
				if (this.lagrangeTabbedPaneManager.activeTab == this.lagrangeCouncilorsTab)
				{
					this.lagrangeTabbedPaneManager.Toggle(this.lagrangeTabbedPaneManager.activeTab);
					this.lagrangeTabbedPaneManager.ClearActiveTab();
				}
			}
			else
			{
				this.lagrangeCouncilorsTabHeader.SetText(Loc.T("UI.Space.CouncilorsTabHeader", new object[] { list3.Count.ToString() }));
				this.lagrangeCouncilorsTabButton.gameObject.SetActive(true);
				this.UpdateCouncilorsList(this.lagrangeCouncilorsList, list3, null, false);
				this.lagrangeCouncilorsTab.SetSize(30f, 27f, 23f, list3.Count);
			}
			this.UpdateOrbitsList(this.lagrangeOrbitsList, list4);
			this.lagrangeOrbitsTab.SetSize(30f, 27f, 23f, list4.Count);
			if (this.lagrangeTabbedPaneManager.activeTab != null)
			{
				this.lagrangeTabbedPaneManager.activeTab.UpdateSize();
			}
			this.UpdateNaturalSpaceObjectLaunchWindowData(lagrangePoint, this.lagrangePointNextLaunchWindow);
		}

		// Token: 0x0600575E RID: 22366 RVA: 0x00282B64 File Offset: 0x00280D64
		private void UpdateLagrangePointCanvas(TILagrangePointState lagrangePoint)
		{
			this.lagrangePointName.SetText(lagrangePoint.displayName);
			if (lagrangePoint.barycenter.objectType == SpaceObjectType.Star)
			{
				this.lagrangeOrbitRadius.SetText(Loc.T("UI.Space.DistAU", new object[] { lagrangePoint.semiMajorAxis_AU.ToString("N2") }));
			}
			else
			{
				this.lagrangeOrbitRadius.SetText(Loc.T("UI.Space.Distkm", new object[] { lagrangePoint.semiMajorAxis_km.ToString("N0") }));
			}
			if (!lagrangePoint.barycenter.isSun)
			{
				this.gotoLagrangeParentButton.gameObject.SetActive(true);
				this.lagrangeParentIcon.sprite = lagrangePoint.barycenter.icon;
			}
			else
			{
				this.gotoLagrangeParentButton.gameObject.SetActive(false);
			}
			ulong population = lagrangePoint.population;
			if (population > 0UL)
			{
				this.lp_populationValue.SetText(Loc.T("UI.Space.Population", new object[]
				{
					TIGlobalConfig.globalConfig.populationInlineSpritePath,
					population.ToString("N0")
				}));
				this.lp_populationPanel.SetActive(true);
			}
			else
			{
				this.lp_populationPanel.SetActive(false);
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(lagrangePoint.GetMaxTierIconPath(), this.lp_maxTierIcon);
			this.lagrangeSecondaryIcon.sprite = lagrangePoint.secondaryObject.icon;
			int num = lagrangePoint.orbits.Where<TIOrbitState>((TIOrbitState x) => !x.isAdHocOrbit).Count<TIOrbitState>();
			if (num == 1)
			{
				this.lagrangeOrbitsTabHeader.SetText(Loc.T("UI.Space.OrbitsTabHeader_One"));
			}
			else
			{
				this.lagrangeOrbitsTabHeader.SetText(Loc.T("UI.Space.OrbitsTabHeader", new object[] { num.ToString() }));
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(lagrangePoint.SolarInsolationIconPath(false), this.lagrangeSolarPotential);
			this.UpdateLagrangePointCanvasTransientData(lagrangePoint);
		}

		// Token: 0x0600575F RID: 22367 RVA: 0x00282D54 File Offset: 0x00280F54
		public static string SpaceBodyDiameterText(TISpaceBodyState spaceBodyState)
		{
			string text;
			if ((spaceBodyState.objectType == SpaceObjectType.Asteroid || spaceBodyState.objectType == SpaceObjectType.AsteroidalMoon || spaceBodyState.objectType == SpaceObjectType.Comet) && spaceBodyState.dimensionY_km != spaceBodyState.dimensionX_km)
			{
				text = Loc.T("UI.Space.Dimensions", new object[]
				{
					spaceBodyState.dimensionX_km.ToString("N0"),
					spaceBodyState.dimensionY_km.ToString("N0"),
					spaceBodyState.dimensionZ_km.ToString("N0")
				});
			}
			else
			{
				text = Loc.T("UI.Space.DiameterSphere", new object[] { spaceBodyState.dimensionX_km.ToString("N0") });
			}
			return text;
		}

		// Token: 0x06005760 RID: 22368 RVA: 0x00282E08 File Offset: 0x00281008
		public static string OrbitAxisText(TISpaceBodyState spaceBodyState)
		{
			if (spaceBodyState.barycenter == null)
			{
				return string.Empty;
			}
			if (spaceBodyState.barycenter.objectType == SpaceObjectType.Star)
			{
				return Loc.T("UI.Space.DistAU", new object[] { spaceBodyState.semiMajorAxis_AU.ToString("N2") });
			}
			return Loc.T("UI.Space.Distkm", new object[] { spaceBodyState.semiMajorAxis_km.ToString("N0") });
		}

		// Token: 0x06005761 RID: 22369 RVA: 0x00282E84 File Offset: 0x00281084
		public static string OrbitPeriodText(TISpaceBodyState spaceBody)
		{
			if (spaceBody.orbitalPeriod_Years >= 1.01)
			{
				return Loc.T("UI.Operations.Duration_years", new object[] { spaceBody.orbitalPeriod_Years.ToString("N2") });
			}
			if (spaceBody.orbitalPeriod_Days >= 2.0)
			{
				return Loc.T("UI.Operations.Duration_days", new object[] { spaceBody.orbitalPeriod_Days.ToString("N0") });
			}
			return Loc.T("UI.Operations.Duration_hours", new object[] { spaceBody.orbitalPeriod_Hours.ToString("N0") });
		}

		// Token: 0x06005762 RID: 22370 RVA: 0x00282F28 File Offset: 0x00281128
		private void UpdateMoonList(TISpaceBodyState spaceBodyState, List<TISpaceBodyState> moons)
		{
			this.moonList.SetListSize<MoonsListItemController>(spaceBodyState.canHaveMoons ? moons.Count : 0, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.moonList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__529.<>p__0 == null)
					{
						SpaceObjectDetailController.<>o__529.<>p__0 = CallSite<Func<CallSite, object, MoonsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(MoonsListItemController), typeof(SpaceObjectDetailController)));
					}
					SpaceObjectDetailController.<>o__529.<>p__0.Target(SpaceObjectDetailController.<>o__529.<>p__0, enumerator.Current).SetListItem(moons[num++]);
				}
			}
		}

		// Token: 0x06005763 RID: 22371 RVA: 0x00282FE0 File Offset: 0x002811E0
		private void UpdateSiteList(List<TIHabSiteState> habSites)
		{
			this.siteList.SetListSize<BaseSiteListItemController>(habSites.Count, false, false);
			int num = 0;
			List<TISpaceAssetState> conditionBlockingSpaceAssets = base.activePlayer.victoryTemplate.GetConditionBlockingSpaceAssets(base.activePlayer);
			using (IEnumerator<object> enumerator = this.siteList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__530.<>p__0 == null)
					{
						SpaceObjectDetailController.<>o__530.<>p__0 = CallSite<Func<CallSite, object, BaseSiteListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(BaseSiteListItemController), typeof(SpaceObjectDetailController)));
					}
					SpaceObjectDetailController.<>o__530.<>p__0.Target(SpaceObjectDetailController.<>o__530.<>p__0, enumerator.Current).SetListItem(habSites[num], base.activePlayer, this.showSiteNames, habSites[num].hasOperatingBase && conditionBlockingSpaceAssets.Contains(habSites[num].hab), this);
					num++;
				}
			}
		}

		// Token: 0x06005764 RID: 22372 RVA: 0x002830D8 File Offset: 0x002812D8
		private void UpdateOrbitsList(ListManagerBase orbitsList, List<TIOrbitState> orbits)
		{
			orbitsList.SetListSize<OrbitsListItemController>(orbits.Count, false, false);
			int num = 0;
			List<TIOrbitState> list = orbits.OrderBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_m).ToList<TIOrbitState>();
			using (IEnumerator<object> enumerator = orbitsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__531.<>p__0 == null)
					{
						SpaceObjectDetailController.<>o__531.<>p__0 = CallSite<Func<CallSite, object, OrbitsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrbitsListItemController), typeof(SpaceObjectDetailController)));
					}
					OrbitsListItemController orbitsListItemController = SpaceObjectDetailController.<>o__531.<>p__0.Target(SpaceObjectDetailController.<>o__531.<>p__0, enumerator.Current);
					orbitsListItemController.Init(list[num++]);
					orbitsListItemController.UpdateListItem();
				}
			}
		}

		// Token: 0x06005765 RID: 22373 RVA: 0x002831AC File Offset: 0x002813AC
		private void UpdateStationsList(ListManagerBase stationsList, List<TIHabState> stations)
		{
			stationsList.SetListSize<StationsListItemController>(stations.Count, false, false);
			int num = 0;
			List<TISpaceAssetState> conditionBlockingSpaceAssets = base.activePlayer.victoryTemplate.GetConditionBlockingSpaceAssets(base.activePlayer);
			using (IEnumerator<object> enumerator = stationsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__532.<>p__0 == null)
					{
						SpaceObjectDetailController.<>o__532.<>p__0 = CallSite<Func<CallSite, object, StationsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(StationsListItemController), typeof(SpaceObjectDetailController)));
					}
					SpaceObjectDetailController.<>o__532.<>p__0.Target(SpaceObjectDetailController.<>o__532.<>p__0, enumerator.Current).UpdateListItem(stations[num], this.selectedSpaceObject.ref_naturalSpaceObject, conditionBlockingSpaceAssets.Contains(stations[num]), this);
					num++;
				}
			}
		}

		// Token: 0x06005766 RID: 22374 RVA: 0x0028327C File Offset: 0x0028147C
		private void UpdateCouncilorsList(ListManagerBase councilorsList, List<TICouncilorState> councilors, List<TIOfficerState> officers, bool showProfession)
		{
			councilorsList.SetListSize<SpaceDetailCouncilorsListItemController>(councilors.Count + ((officers != null) ? officers.Count : 0), false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = councilorsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__533.<>p__0 == null)
					{
						SpaceObjectDetailController.<>o__533.<>p__0 = CallSite<Func<CallSite, object, SpaceDetailCouncilorsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SpaceDetailCouncilorsListItemController), typeof(SpaceObjectDetailController)));
					}
					SpaceDetailCouncilorsListItemController spaceDetailCouncilorsListItemController = SpaceObjectDetailController.<>o__533.<>p__0.Target(SpaceObjectDetailController.<>o__533.<>p__0, enumerator.Current);
					if (num < councilors.Count)
					{
						spaceDetailCouncilorsListItemController.SetListItem(councilors[num++], showProfession);
					}
					else
					{
						spaceDetailCouncilorsListItemController.SetListItem(officers[num - councilors.Count]);
						num++;
					}
				}
			}
		}

		// Token: 0x06005767 RID: 22375 RVA: 0x00283354 File Offset: 0x00281554
		private void UpdateFleetsList(ListManagerBase fleetsList, List<TISpaceFleetState> fleets)
		{
			fleetsList.SetListSize<FleetsInOrbitListItemController>(fleets.Count, false, false);
			int num = 0;
			List<TISpaceAssetState> conditionBlockingSpaceAssets = base.activePlayer.victoryTemplate.GetConditionBlockingSpaceAssets(base.activePlayer);
			using (IEnumerator<object> enumerator = fleetsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__534.<>p__0 == null)
					{
						SpaceObjectDetailController.<>o__534.<>p__0 = CallSite<Func<CallSite, object, FleetsInOrbitListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FleetsInOrbitListItemController), typeof(SpaceObjectDetailController)));
					}
					FleetsInOrbitListItemController fleetsInOrbitListItemController = SpaceObjectDetailController.<>o__534.<>p__0.Target(SpaceObjectDetailController.<>o__534.<>p__0, enumerator.Current);
					TISpaceFleetState tispaceFleetState = fleets[num];
					fleetsInOrbitListItemController.UpdateListItem(tispaceFleetState, this.selectedSpaceObject.ref_naturalSpaceObject, conditionBlockingSpaceAssets.Contains(tispaceFleetState));
					num++;
				}
			}
		}

		// Token: 0x06005768 RID: 22376 RVA: 0x00283420 File Offset: 0x00281620
		public void OnShipyardButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			GameControl.eventManager.TriggerEvent(new ShipyardUIRequested(base.activePlayer), null, new object[] { base.activePlayer });
		}

		// Token: 0x06005769 RID: 22377 RVA: 0x00283453 File Offset: 0x00281653
		public void OnSellSpaceResourcesClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			GameControl.eventManager.TriggerEvent(new SellSpaceResourcesRequested(base.activePlayer), null, new object[] { base.activePlayer });
		}

		// Token: 0x0600576A RID: 22378 RVA: 0x00283488 File Offset: 0x00281688
		private void UpdateHabCanvas(TIHabState hab)
		{
			if (hab == null || hab.archived || hab.AllModules().Count == 0)
			{
				base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
				return;
			}
			this.habName.SetText(hab.displayName);
			this.habParentIcon.sprite = hab.barycenter.icon;
			this.habHeaderText.SetText(Loc.T(hab.IsStation ? "UI.Space.StationFactionDescriptor" : "UI.Space.BaseFactionDescriptor", new object[] { hab.faction.adjective }));
			StringBuilder stringBuilder = new StringBuilder(hab.description);
			if (base.activePlayer.victoryTemplate.GetConditionBlockingSpaceAssets(base.activePlayer).Contains(hab))
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.victoryItemInlineSpritePath);
			}
			if (hab.underAssault || (hab.IsBase && hab.underBombardment) || (hab.IsStation && hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(hab.faction))))
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.armyBattleInlineSpritePath);
			}
			this.habSizeDescriptor.SetText(stringBuilder.ToString());
			if (hab.IsStation)
			{
				this.habOrbitDescriptor.SetText(TIUtilities.GetLocationString(hab.orbitState, true, false));
			}
			else
			{
				this.habOrbitDescriptor.SetText(TIUtilities.GetLocationString(hab.habSite, true, false));
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(hab.sectors[0].faction.template.stationIcon, this.habSectorFactionImages[0]);
			this.activePlayerShipyard.SetActive(hab.AllowsShipConstruction(base.activePlayer, true, false));
			this.activePlayerResupply.SetActive(hab.AllowsResupply(base.activePlayer, false, true));
			this.activePlayerConstruction.SetActive(hab.HabConstructHabOptions(base.activePlayer, true, false).Count > 0);
			this.activePlayerSellResourcesButtonObject.SetActive(hab.CanSellResources(base.activePlayer));
			this.habCombatScore.SetText(TIUtilities.FormatBigOrSmallNumber(hab.SpaceCombatValue(), 1, 0, 0, false, false));
			if (hab.IsBase)
			{
				float num = hab.ModifiedDefenseCombatValue(false);
				float num2 = hab.ModifiedDefenseCombatValue(true);
				if (num != num2)
				{
					this.habAssaultScore.SetText(Loc.T("UI.Space.Stations", new object[]
					{
						num.ToString("N0"),
						num2.ToString("N0")
					}));
					this.assaultCombatTip.enabled = true;
				}
				else
				{
					this.habAssaultScore.SetText(num.ToString("N0"));
					this.assaultCombatTip.enabled = false;
				}
			}
			else
			{
				this.habAssaultScore.SetText(hab.ModifiedDefenseCombatValue(false).ToString("N0"));
				this.assaultCombatTip.enabled = false;
			}
			TMP_Text tmp_Text = this.habSectorsTabButtonText;
			string text = "UI.Space.Sectors";
			object[] array = new object[1];
			array[0] = hab.AllModules().Count<TIHabModuleState>((TIHabModuleState x) => x.hasModule);
			tmp_Text.SetText(Loc.T(text, array));
			this.UpdateHabSectorsList(hab);
			List<TICouncilorState> list = hab.CouncilorsPresentAndKnownToFaction(base.activePlayer, false, null);
			List<TIOfficerState> list2 = (hab.ref_factions.Contains(base.activePlayer) ? hab.officersOnBoard : new List<TIOfficerState>());
			int num3 = list.Count + list2.Count;
			if (num3 > 0)
			{
				this.habCouncilorsTabButtonObject.SetActive(true);
				this.habCouncilorsTabButtonText.SetText(Loc.T("UI.Space.PersonnelTabHeader", new object[] { num3 }));
				this.UpdateCouncilorsList(this.habCouncilorsListManager, list, list2, true);
				this.habCouncilorsPaneController.SetSize(30f, 27f, 23f, num3);
			}
			else
			{
				this.habCouncilorsTabButtonObject.SetActive(false);
				if (this.habCouncilorsPaneController.isActiveAndEnabled && this.habPaneManager.activeTab == this.habCouncilorsPaneController)
				{
					this.habPaneManager.Toggle(this.habCouncilorsPaneController);
				}
			}
			IEnumerable<TISpaceFleetState> enumerable = hab.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState z) => z.VisibleToFaction(this.activePlayer));
			List<TISpaceFleetState> list3 = ((enumerable != null) ? enumerable.ToList<TISpaceFleetState>() : null);
			if (list3.Count > 0)
			{
				this.habDockedFleetsTabButtonObject.SetActive(true);
				this.habDockedfleetsTabButtonText.SetText(Loc.T("UI.Space.FleetsTabHeader", new object[] { list3.Count }));
				this.UpdateFleetsList(this.habFleetsListManager, list3);
				this.habFleetsPaneController.SetSize(30f, 27f, 23f, list3.Count);
			}
			else
			{
				this.habDockedFleetsTabButtonObject.SetActive(false);
				if (this.habFleetsPaneController.isActiveAndEnabled && this.habPaneManager.activeTab == this.habFleetsPaneController)
				{
					this.habPaneManager.Toggle(this.habFleetsPaneController);
				}
			}
			if (this.habPaneManager.activeTab != null)
			{
				this.habPaneManager.activeTab.UpdateSize();
			}
			if (hab.IsStation)
			{
				this.localgravity_gs.SetText(new StringBuilder(TIGlobalConfig.globalConfig.gravityInlineSpritePath).Append(" ").Append(FleetsScreenController.accelerationStr(hab.ref_orbit.localGravity_gs, false, false, true)).ToString());
				this.localGravityTip.SetText("BodyText", Loc.T("UI.Space.OrbitAccelGs"));
			}
			else
			{
				this.localgravity_gs.SetText(new StringBuilder(TIGlobalConfig.globalConfig.gravityInlineSpritePath).Append(" ").Append(FleetsScreenController.accelerationStr(hab.habSite.surfaceGravity_g, false, false, true)).ToString());
				this.localGravityTip.SetText("BodyText", Loc.T("UI.Space.SurfaceGravityTooltip"));
			}
			this.habCoreDefendInterest.SetActive(hab.coreDefended);
		}

		// Token: 0x0600576B RID: 22379 RVA: 0x00283B34 File Offset: 0x00281D34
		private static string SemimajorAxisTooltip(TISpaceBodyState spaceBodyState)
		{
			string text;
			string text2;
			if (spaceBodyState.isaMoon)
			{
				text = spaceBodyState.periapsis_km.ToString("N0");
				text2 = spaceBodyState.apoapsis_km.ToString("N0");
			}
			else
			{
				text = spaceBodyState.periapsis_AU.ToString("N2");
				text2 = spaceBodyState.apoapsis_AU.ToString("N2");
			}
			string text3;
			if (spaceBodyState.inclination_Rad * 57.295780181884766 > 1.0)
			{
				text3 = Loc.T("UI.Space.Inclination", new object[] { (spaceBodyState.inclination_Rad * 57.295780181884766).ToString("N0") });
			}
			else
			{
				text3 = string.Empty;
			}
			if (Mathd.Approximately(spaceBodyState.ecc, 0.0) || text == text2)
			{
				return Loc.T("UI.Space.SemimajorAxisTooltip", new object[]
				{
					string.Empty,
					text3
				});
			}
			if (spaceBodyState.isaMoon)
			{
				return Loc.T("UI.Space.SemimajorAxisTooltip", new object[]
				{
					Loc.T("UI.Space.ApsesTooltip_KM", new object[] { text, text2 }),
					text3
				});
			}
			return Loc.T("UI.Space.SemimajorAxisTooltip", new object[]
			{
				Loc.T("UI.Space.ApsesTooltip_AU", new object[] { text, text2 }),
				text3
			});
		}

		// Token: 0x0600576C RID: 22380 RVA: 0x00283C92 File Offset: 0x00281E92
		private static string OrbitalPeriodTooltip(TISpaceBodyState body)
		{
			if (body.barycenter != null)
			{
				return Loc.T("UI.Space.OrbitalPeriodTooltip", new object[] { body.barycenter.displayName });
			}
			return "The Sun orbits the center of the galaxy every 230,000,000 years.";
		}

		// Token: 0x0600576D RID: 22381 RVA: 0x00283CC8 File Offset: 0x00281EC8
		private void UpdateHabSectorsList(TIHabState hab)
		{
			List<TISectorState> list = (from x in hab.sectors
				where x.active
				orderby TISectorState.sectorDisplayNum(x.sectorNum, hab.habType)
				select x).ToList<TISectorState>();
			List<TIHabModuleState> list2 = (from x in list.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules)
				where x.hasModule
				orderby TISectorState.sectorDisplayNum(x.sector.sectorNum, hab.habType)
				select x).ToList<TIHabModuleState>();
			int num = list.Count + list2.Count;
			this.sectorsList.SetListSize<SectorsListItemController>(num, false, false);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			List<int> list3 = new List<int> { 0 };
			for (int i = 1; i < list.Count; i++)
			{
				list3.Add(list3[i - 1] + list[i - 1].habModules.Where<TIHabModuleState>((TIHabModuleState x) => !x.empty).Count<TIHabModuleState>() + 1);
			}
			using (IEnumerator<object> enumerator = this.sectorsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceObjectDetailController.<>o__540.<>p__0 == null)
					{
						SpaceObjectDetailController.<>o__540.<>p__0 = CallSite<Func<CallSite, object, SectorsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SectorsListItemController), typeof(SpaceObjectDetailController)));
					}
					SectorsListItemController sectorsListItemController = SpaceObjectDetailController.<>o__540.<>p__0.Target(SpaceObjectDetailController.<>o__540.<>p__0, enumerator.Current);
					if (list3.Contains(num2))
					{
						sectorsListItemController.UpdateHeaderItem(list[num3++]);
					}
					else
					{
						sectorsListItemController.UpdateListItem(list2[num4++]);
					}
					num2++;
				}
			}
			this.sectorsPaneController.SetSize(30f, 27f, 23f, num);
		}

		// Token: 0x0600576E RID: 22382 RVA: 0x00283EF8 File Offset: 0x002820F8
		public void HabImageClicked()
		{
			this.Hide();
			SoundEffectController.PlaySelectSound(GeneralControlsController.UIOtherSelectedState);
			TIUtilities.GotoGameState(GeneralControlsController.UIOtherSelectedState, true, true, true, true, false, -1f);
			GameControl.eventManager.TriggerEvent(new HabDetailRequested(this.UIOtherSelectedState.ref_hab, true), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreSpaceObjectDetailCanvas), null, null, true, false);
		}

		// Token: 0x0600576F RID: 22383 RVA: 0x00283F64 File Offset: 0x00282164
		public void HabSelectedFromSiteList(TIHabState hab)
		{
			this.Hide();
			TIUtilities.GotoGameState(hab, false, true, true, false, true, -1f);
			GameControl.eventManager.TriggerEvent(new HabDetailRequested(hab, true), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreSpaceObjectDetailCanvas), null, null, true, false);
		}

		// Token: 0x06005770 RID: 22384 RVA: 0x00283FB8 File Offset: 0x002821B8
		private void LateUpdate()
		{
			if (!TIGlobalValuesState.isSpaceCombatEnabled && (this.naturalBodyPanel.enabled || this.habPanel.enabled) && this.modelInstance != null && this.modelRotationRate != 0f)
			{
				this.modelInstance.transform.Rotate(this.modelRotationAxis, this.modelRotationRate, Space.World);
			}
		}

		// Token: 0x06005771 RID: 22385 RVA: 0x0028401E File Offset: 0x0028221E
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.UpdatePlayerFleetObjectCanvas(this.selectedAsset);
		}

		// Token: 0x04003DDF RID: 15839
		public static bool DisplayExtendedFleetUI;

		// Token: 0x04003DE0 RID: 15840
		public GameObject selectionCamera;

		// Token: 0x04003DE1 RID: 15841
		public GameObject mySelectedObjectCamera;

		// Token: 0x04003DE2 RID: 15842
		private TIGameState selectedSpaceObject;

		// Token: 0x04003DE3 RID: 15843
		private TISpaceFleetState selectedAsset;

		// Token: 0x04003DE4 RID: 15844
		public Canvas naturalBodyPanel;

		// Token: 0x04003DE5 RID: 15845
		public Canvas enemySpaceFleetPanel;

		// Token: 0x04003DE6 RID: 15846
		public Canvas mySpaceFleetPanel;

		// Token: 0x04003DE7 RID: 15847
		public Canvas habPanel;

		// Token: 0x04003DE8 RID: 15848
		public Canvas lagrangePointPanel;

		// Token: 0x04003DE9 RID: 15849
		[Header("Tutorials")]
		public UITutorialController spacebodyUITutorialController;

		// Token: 0x04003DEA RID: 15850
		public UITutorialController lagrangeUITutorialController;

		// Token: 0x04003DEB RID: 15851
		public UITutorialController habUITutorialController;

		// Token: 0x04003DEC RID: 15852
		public UITutorialController enemyFleetUITutorialController;

		// Token: 0x04003DED RID: 15853
		[Header("Natural Space Body Panel")]
		public TMP_Text spaceBodyName;

		// Token: 0x04003DEE RID: 15854
		public Button gotoButton;

		// Token: 0x04003DEF RID: 15855
		public GameObject backButtonNaturalSpacebodyDetail;

		// Token: 0x04003DF0 RID: 15856
		public Image naturalSpaceBodyBackgroundImage;

		// Token: 0x04003DF1 RID: 15857
		public GameObject gotoParentButton;

		// Token: 0x04003DF2 RID: 15858
		public Image parentIcon;

		// Token: 0x04003DF3 RID: 15859
		public TMP_Text naturalSpaceBodyQuickDescription;

		// Token: 0x04003DF4 RID: 15860
		public TMP_Text naturalSpaceBodyMiningProfile;

		// Token: 0x04003DF5 RID: 15861
		public TMP_Text naturalSpaceBodyDiameter;

		// Token: 0x04003DF6 RID: 15862
		public TMP_Text naturalSpaceBodyOrbit;

		// Token: 0x04003DF7 RID: 15863
		public TMP_Text naturalSpaceBodyLaunchWindow;

		// Token: 0x04003DF8 RID: 15864
		public TMP_Text naturalSpaceBodyEscapeVelocity;

		// Token: 0x04003DF9 RID: 15865
		public TMP_Text naturalSpaceBodySurfaceGravity;

		// Token: 0x04003DFA RID: 15866
		public TMP_Text naturalSpaceBodyOrbitPeriod;

		// Token: 0x04003DFB RID: 15867
		public TooltipTrigger diameterTip;

		// Token: 0x04003DFC RID: 15868
		public TooltipTrigger orbitTip;

		// Token: 0x04003DFD RID: 15869
		public TooltipTrigger gravityTip;

		// Token: 0x04003DFE RID: 15870
		public TooltipTrigger escapeVelocityTip;

		// Token: 0x04003DFF RID: 15871
		public TooltipTrigger orbitPeriodTip;

		// Token: 0x04003E00 RID: 15872
		public Button satellitesButton;

		// Token: 0x04003E01 RID: 15873
		public Button baseSitesButton;

		// Token: 0x04003E02 RID: 15874
		public Button orbitsButton;

		// Token: 0x04003E03 RID: 15875
		public Button councilorsButton;

		// Token: 0x04003E04 RID: 15876
		public Button fleetsButton;

		// Token: 0x04003E05 RID: 15877
		public Button stationsButton;

		// Token: 0x04003E06 RID: 15878
		public TMP_Text satellitesTabHeader;

		// Token: 0x04003E07 RID: 15879
		public TMP_Text baseSitesTabHeader;

		// Token: 0x04003E08 RID: 15880
		public TMP_Text orbitsTabHeader;

		// Token: 0x04003E09 RID: 15881
		public TMP_Text stationsTabHeader;

		// Token: 0x04003E0A RID: 15882
		public TMP_Text councilorsTabHeader;

		// Token: 0x04003E0B RID: 15883
		public TMP_Text fleetsTabHeader;

		// Token: 0x04003E0C RID: 15884
		public Image solarIcon;

		// Token: 0x04003E0D RID: 15885
		public TooltipTrigger solarTip;

		// Token: 0x04003E0E RID: 15886
		public Image atmoIcon;

		// Token: 0x04003E0F RID: 15887
		public TooltipTrigger atmoTip;

		// Token: 0x04003E10 RID: 15888
		public Image waterIcon;

		// Token: 0x04003E11 RID: 15889
		public Image volatilesIcon;

		// Token: 0x04003E12 RID: 15890
		public Image metalsIcon;

		// Token: 0x04003E13 RID: 15891
		public Image noblesIcon;

		// Token: 0x04003E14 RID: 15892
		public Image fissilesIcon;

		// Token: 0x04003E15 RID: 15893
		public Image solarPotential;

		// Token: 0x04003E16 RID: 15894
		public Image waterPotential;

		// Token: 0x04003E17 RID: 15895
		public Image volatilesPotential;

		// Token: 0x04003E18 RID: 15896
		public Image metalsPotential;

		// Token: 0x04003E19 RID: 15897
		public Image noblesPotential;

		// Token: 0x04003E1A RID: 15898
		public Image fissilesPotential;

		// Token: 0x04003E1B RID: 15899
		public GameObject solarPotentialObject;

		// Token: 0x04003E1C RID: 15900
		public GameObject waterPotentialObject;

		// Token: 0x04003E1D RID: 15901
		public GameObject volatilesPotentialObject;

		// Token: 0x04003E1E RID: 15902
		public GameObject metalsPotentialObject;

		// Token: 0x04003E1F RID: 15903
		public GameObject noblesPotentialObject;

		// Token: 0x04003E20 RID: 15904
		public GameObject fissilesPotentialObject;

		// Token: 0x04003E21 RID: 15905
		public GameObject naturalBodyTabContainer;

		// Token: 0x04003E22 RID: 15906
		public TabbedPaneManager naturalBodyTabManager;

		// Token: 0x04003E23 RID: 15907
		public TabbedPaneController orbitsTab;

		// Token: 0x04003E24 RID: 15908
		public TabbedPaneController moonsTab;

		// Token: 0x04003E25 RID: 15909
		public TabbedPaneController councilorsTab;

		// Token: 0x04003E26 RID: 15910
		public TabbedPaneController sitesTab;

		// Token: 0x04003E27 RID: 15911
		public TabbedPaneController fleetsTab;

		// Token: 0x04003E28 RID: 15912
		public TabbedPaneController stationsTab;

		// Token: 0x04003E29 RID: 15913
		public TMP_Text moonsHeaderName;

		// Token: 0x04003E2A RID: 15914
		public TMP_Text moonsHeaderType;

		// Token: 0x04003E2B RID: 15915
		public TMP_Text moonsHeaderBases;

		// Token: 0x04003E2C RID: 15916
		private bool showSiteNames = true;

		// Token: 0x04003E2D RID: 15917
		public TMP_Text sitesHeaderName;

		// Token: 0x04003E2E RID: 15918
		public TMP_Text natural_orbitHeaderName;

		// Token: 0x04003E2F RID: 15919
		public TMP_Text natural_orbitHeaderAltitude;

		// Token: 0x04003E30 RID: 15920
		public TMP_Text stationHeaderName;

		// Token: 0x04003E31 RID: 15921
		public TMP_Text stationHeaderAltitude;

		// Token: 0x04003E32 RID: 15922
		public TMP_Text stationHeaderControlPoints;

		// Token: 0x04003E33 RID: 15923
		public ListManagerBase moonList;

		// Token: 0x04003E34 RID: 15924
		public ListManagerBase siteList;

		// Token: 0x04003E35 RID: 15925
		public ListManagerBase orbitsList;

		// Token: 0x04003E36 RID: 15926
		public ListManagerBase stationList;

		// Token: 0x04003E37 RID: 15927
		public ListManagerBase fleetList;

		// Token: 0x04003E38 RID: 15928
		public ListManagerBase councilorList;

		// Token: 0x04003E39 RID: 15929
		public TMP_Text councilorHeaderName;

		// Token: 0x04003E3A RID: 15930
		public TMP_Text councilorHeaderLocation;

		// Token: 0x04003E3B RID: 15931
		public TMP_Text fleetName;

		// Token: 0x04003E3C RID: 15932
		public TMP_Text fleetAltitude;

		// Token: 0x04003E3D RID: 15933
		public TMP_Text fleetDVHeader;

		// Token: 0x04003E3E RID: 15934
		public TMP_Text fleetSmallShipsHeader;

		// Token: 0x04003E3F RID: 15935
		public TMP_Text fleetMediumShipsHeader;

		// Token: 0x04003E40 RID: 15936
		public TMP_Text fleetLargeShipsHeader;

		// Token: 0x04003E41 RID: 15937
		public TooltipTrigger natural_orbitHeaderGs;

		// Token: 0x04003E42 RID: 15938
		public TooltipTrigger natural_amatHeader;

		// Token: 0x04003E43 RID: 15939
		public GameObject probeDataPanel;

		// Token: 0x04003E44 RID: 15940
		public Image probeIcon;

		// Token: 0x04003E45 RID: 15941
		public TMP_Text probeArrivalDate;

		// Token: 0x04003E46 RID: 15942
		public GameObject populationPanel;

		// Token: 0x04003E47 RID: 15943
		public TMP_Text populationValue;

		// Token: 0x04003E48 RID: 15944
		public Image maxTierIcon;

		// Token: 0x04003E49 RID: 15945
		public TooltipTrigger maxTierTip;

		// Token: 0x04003E4A RID: 15946
		public TooltipTrigger habSiteHohmannTip;

		// Token: 0x04003E4B RID: 15947
		public Image playerTagButtonImage;

		// Token: 0x04003E4C RID: 15948
		[Header("Enemy Fleet Detail Panel")]
		public TMP_Text enemyFleetHeader;

		// Token: 0x04003E4D RID: 15949
		public TMP_Text enemyFleetName;

		// Token: 0x04003E4E RID: 15950
		public Image enemyFleetFactionIcon;

		// Token: 0x04003E4F RID: 15951
		public Image enemyFleetFactionGradient;

		// Token: 0x04003E50 RID: 15952
		public GameObject backButtonEnemyFleetDetail;

		// Token: 0x04003E51 RID: 15953
		public TMP_Text enemyFleetSize;

		// Token: 0x04003E52 RID: 15954
		public TMP_Text enemyFleetAcceleration;

		// Token: 0x04003E53 RID: 15955
		public TMP_Text enemyFleetDeltaV;

		// Token: 0x04003E54 RID: 15956
		public TMP_Text enemyFleetCombatScore;

		// Token: 0x04003E55 RID: 15957
		public TMP_Text enemyFleetAssaultScore;

		// Token: 0x04003E56 RID: 15958
		public RectTransform enemyFleetTransferProgressLine;

		// Token: 0x04003E57 RID: 15959
		private float enemyFleetTransferSliderRange;

		// Token: 0x04003E58 RID: 15960
		private float enemyFleetTransferSliderZeroPoint;

		// Token: 0x04003E59 RID: 15961
		public Image enemyTransferOriginIcon;

		// Token: 0x04003E5A RID: 15962
		public Image enemyTransferDestinationIcon;

		// Token: 0x04003E5B RID: 15963
		public Image enemyTransferDestinationDetailIcon;

		// Token: 0x04003E5C RID: 15964
		public Image enemyTransferProgressIcon;

		// Token: 0x04003E5D RID: 15965
		public Image enemyTransferPendingCombatIcon;

		// Token: 0x04003E5E RID: 15966
		public TMP_Text enemyTransferTextDetail;

		// Token: 0x04003E5F RID: 15967
		public Image enemyGenericOpImage;

		// Token: 0x04003E60 RID: 15968
		public Image enemyGenericOpSmallImage;

		// Token: 0x04003E61 RID: 15969
		public TMP_Text enemyGenericOpLine1;

		// Token: 0x04003E62 RID: 15970
		public TMP_Text enemyGenericOpLine2;

		// Token: 0x04003E63 RID: 15971
		public GameObject enemyTransferObject;

		// Token: 0x04003E64 RID: 15972
		public GameObject enemyGenericOpDetailObject;

		// Token: 0x04003E65 RID: 15973
		public ListManagerBase enemyFleetShipsGridList;

		// Token: 0x04003E66 RID: 15974
		public TMP_Text enemyFleetShipsTabButtonText;

		// Token: 0x04003E67 RID: 15975
		public GameObject enemyFleetRawCameraImageObject;

		// Token: 0x04003E68 RID: 15976
		public Image enemyFleetBackgroundImage;

		// Token: 0x04003E69 RID: 15977
		private bool enemyFleetShipListOpen;

		// Token: 0x04003E6A RID: 15978
		private TIGameState eo1;

		// Token: 0x04003E6B RID: 15979
		private TIGameState eo2;

		// Token: 0x04003E6C RID: 15980
		private TIGameState ed1;

		// Token: 0x04003E6D RID: 15981
		private TIGameState ed2;

		// Token: 0x04003E6E RID: 15982
		public GameObject enemyShipListObject;

		// Token: 0x04003E6F RID: 15983
		public RectTransform enemyFleetUpperPanelTransform;

		// Token: 0x04003E70 RID: 15984
		public RectTransform enemyShipListTransform;

		// Token: 0x04003E71 RID: 15985
		public ListManagerBase enemyFleetCouncilorsGrid;

		// Token: 0x04003E72 RID: 15986
		public ListManagerBase enemyFleetDetailList;

		// Token: 0x04003E73 RID: 15987
		public GameObject alertFleetButtonContainerObject;

		// Token: 0x04003E74 RID: 15988
		public TMP_Text alertFleetButtonText;

		// Token: 0x04003E75 RID: 15989
		public TooltipTrigger alertFleetButtonTip;

		// Token: 0x04003E76 RID: 15990
		[Header("My Fleet Detail Panel")]
		public TMP_Text myFleetHeader;

		// Token: 0x04003E77 RID: 15991
		public TMP_Text myFleetAcceleration;

		// Token: 0x04003E78 RID: 15992
		public TMP_Text myFleetDeltaV;

		// Token: 0x04003E79 RID: 15993
		public TMP_Text myFleetSize;

		// Token: 0x04003E7A RID: 15994
		public TMP_Text myFleetCombatScore;

		// Token: 0x04003E7B RID: 15995
		public TMP_Text myFleetAssaultScore;

		// Token: 0x04003E7C RID: 15996
		public TMP_Text myFleetMissionControlConsumption;

		// Token: 0x04003E7D RID: 15997
		public GameObject myFleetHomeportObject;

		// Token: 0x04003E7E RID: 15998
		public TMP_Text myFleetHomeport;

		// Token: 0x04003E7F RID: 15999
		public TMP_Text myFleetName;

		// Token: 0x04003E80 RID: 16000
		public Image myFleetCouncilIcon;

		// Token: 0x04003E81 RID: 16001
		public Image myFleetCouncilGradient;

		// Token: 0x04003E82 RID: 16002
		public Button myFleetRefitButton;

		// Token: 0x04003E83 RID: 16003
		public TMP_Text myFleetRefitButtonText;

		// Token: 0x04003E84 RID: 16004
		private TISpaceFleetState validRefitFleet;

		// Token: 0x04003E85 RID: 16005
		public GameObject transferObject;

		// Token: 0x04003E86 RID: 16006
		public Image transferOriginIcon;

		// Token: 0x04003E87 RID: 16007
		public Image transferDestinationIcon;

		// Token: 0x04003E88 RID: 16008
		public Image transferDestinationDetailIcon;

		// Token: 0x04003E89 RID: 16009
		public RectTransform myFleetTransferProgressLine;

		// Token: 0x04003E8A RID: 16010
		private float myFleetTransferSliderZeroPoint;

		// Token: 0x04003E8B RID: 16011
		private float myFleetTransferSliderRange;

		// Token: 0x04003E8C RID: 16012
		public Image transferProgressIcon;

		// Token: 0x04003E8D RID: 16013
		public Image transferPendingCombatIcon;

		// Token: 0x04003E8E RID: 16014
		public TMP_Text transferTextDetail;

		// Token: 0x04003E8F RID: 16015
		public GameObject genericOpDetailObject;

		// Token: 0x04003E90 RID: 16016
		public Image genericOpImage;

		// Token: 0x04003E91 RID: 16017
		public Image genericOpSmallImage;

		// Token: 0x04003E92 RID: 16018
		public TMP_Text genericOpLine1;

		// Token: 0x04003E93 RID: 16019
		public TMP_Text genericOpLine2;

		// Token: 0x04003E94 RID: 16020
		public GameObject fleetStandingOrderObject;

		// Token: 0x04003E95 RID: 16021
		public ListManagerBase myFleetShipsGridList;

		// Token: 0x04003E96 RID: 16022
		public TMP_Text myFleetShipsTabButtonText;

		// Token: 0x04003E97 RID: 16023
		public ListManagerBase myFleetShipsFullList;

		// Token: 0x04003E98 RID: 16024
		private bool playerFleetShipListOpen;

		// Token: 0x04003E99 RID: 16025
		public GameObject pusherObject;

		// Token: 0x04003E9A RID: 16026
		public RectTransform pusherTransform;

		// Token: 0x04003E9B RID: 16027
		public RectTransform upperPanelTransform;

		// Token: 0x04003E9C RID: 16028
		public GameObject shipListObject;

		// Token: 0x04003E9D RID: 16029
		public RectTransform shipListTransform;

		// Token: 0x04003E9E RID: 16030
		public GameObject myFleetRawCameraImageObject;

		// Token: 0x04003E9F RID: 16031
		public Image myFleetBackgroundImage;

		// Token: 0x04003EA0 RID: 16032
		public ListManagerBase myFleetCouncilorsGrid;

		// Token: 0x04003EA1 RID: 16033
		private TIGameState d1;

		// Token: 0x04003EA2 RID: 16034
		private TIGameState d2;

		// Token: 0x04003EA3 RID: 16035
		private TIGameState o1;

		// Token: 0x04003EA4 RID: 16036
		private TIGameState o2;

		// Token: 0x04003EA5 RID: 16037
		[Header("My Fleet Customization")]
		public GameObject renameMyFleetPanel;

		// Token: 0x04003EA6 RID: 16038
		public TextMeshProUGUI saveNameText;

		// Token: 0x04003EA7 RID: 16039
		public TextMeshProUGUI revertNameText;

		// Token: 0x04003EA8 RID: 16040
		public TMP_InputField nameInputField;

		// Token: 0x04003EA9 RID: 16041
		[Header("Hab Detail Panel")]
		public TMP_Text habHeaderText;

		// Token: 0x04003EAA RID: 16042
		public TMP_Text habName;

		// Token: 0x04003EAB RID: 16043
		public Button gotoHabParentButton;

		// Token: 0x04003EAC RID: 16044
		public Button sellSpaceResourcesButton;

		// Token: 0x04003EAD RID: 16045
		public GameObject backButtonHabDetail;

		// Token: 0x04003EAE RID: 16046
		public Image habParentIcon;

		// Token: 0x04003EAF RID: 16047
		public GameObject habCoreDefendInterest;

		// Token: 0x04003EB0 RID: 16048
		public TMP_Text habSizeDescriptor;

		// Token: 0x04003EB1 RID: 16049
		public TMP_Text habOrbitDescriptor;

		// Token: 0x04003EB2 RID: 16050
		public TMP_Text habSectorsTabButtonText;

		// Token: 0x04003EB3 RID: 16051
		public TMP_Text habCouncilorsTabButtonText;

		// Token: 0x04003EB4 RID: 16052
		public TMP_Text habDockedfleetsTabButtonText;

		// Token: 0x04003EB5 RID: 16053
		public GameObject habCouncilorsTabButtonObject;

		// Token: 0x04003EB6 RID: 16054
		public GameObject habDockedFleetsTabButtonObject;

		// Token: 0x04003EB7 RID: 16055
		public Image[] habSectorFactionImages;

		// Token: 0x04003EB8 RID: 16056
		public GameObject activePlayerShipyard;

		// Token: 0x04003EB9 RID: 16057
		public GameObject activePlayerResupply;

		// Token: 0x04003EBA RID: 16058
		public GameObject activePlayerConstruction;

		// Token: 0x04003EBB RID: 16059
		public GameObject activePlayerSellResourcesButtonObject;

		// Token: 0x04003EBC RID: 16060
		public GameObject baseIllustrationObject;

		// Token: 0x04003EBD RID: 16061
		public TMP_Text habCombatScore;

		// Token: 0x04003EBE RID: 16062
		public TMP_Text habAssaultScore;

		// Token: 0x04003EBF RID: 16063
		public TooltipTrigger assaultCombatTip;

		// Token: 0x04003EC0 RID: 16064
		public Image habBackgroundImage;

		// Token: 0x04003EC1 RID: 16065
		public Image S0M0;

		// Token: 0x04003EC2 RID: 16066
		public Image S0M1;

		// Token: 0x04003EC3 RID: 16067
		public Image S0M2;

		// Token: 0x04003EC4 RID: 16068
		public Image S0M3;

		// Token: 0x04003EC5 RID: 16069
		public Image S0M4;

		// Token: 0x04003EC6 RID: 16070
		public Image S1M0;

		// Token: 0x04003EC7 RID: 16071
		public Image S1M1;

		// Token: 0x04003EC8 RID: 16072
		public Image S1M2;

		// Token: 0x04003EC9 RID: 16073
		public Image S1M3;

		// Token: 0x04003ECA RID: 16074
		public Image S2M0;

		// Token: 0x04003ECB RID: 16075
		public Image S2M1;

		// Token: 0x04003ECC RID: 16076
		public Image S2M2;

		// Token: 0x04003ECD RID: 16077
		public Image S2M3;

		// Token: 0x04003ECE RID: 16078
		public Image S3M0;

		// Token: 0x04003ECF RID: 16079
		public Image S3M1;

		// Token: 0x04003ED0 RID: 16080
		public Image S3M2;

		// Token: 0x04003ED1 RID: 16081
		public Image S3M3;

		// Token: 0x04003ED2 RID: 16082
		public Image S4M0;

		// Token: 0x04003ED3 RID: 16083
		public Image S4M1;

		// Token: 0x04003ED4 RID: 16084
		public Image S4M2;

		// Token: 0x04003ED5 RID: 16085
		public Image S4M3;

		// Token: 0x04003ED6 RID: 16086
		public Image C034T;

		// Token: 0x04003ED7 RID: 16087
		public Image C04T;

		// Token: 0x04003ED8 RID: 16088
		public Image C03T;

		// Token: 0x04003ED9 RID: 16089
		public Image C24C;

		// Token: 0x04003EDA RID: 16090
		public Image C13C;

		// Token: 0x04003EDB RID: 16091
		public GameObject habRawImageObject;

		// Token: 0x04003EDC RID: 16092
		public ListManagerBase sectorsList;

		// Token: 0x04003EDD RID: 16093
		public ListManagerBase habCouncilorsListManager;

		// Token: 0x04003EDE RID: 16094
		public ListManagerBase habFleetsListManager;

		// Token: 0x04003EDF RID: 16095
		public TabbedPaneManager habPaneManager;

		// Token: 0x04003EE0 RID: 16096
		public TabbedPaneController sectorsPaneController;

		// Token: 0x04003EE1 RID: 16097
		public TabbedPaneController habCouncilorsPaneController;

		// Token: 0x04003EE2 RID: 16098
		public TabbedPaneController habFleetsPaneController;

		// Token: 0x04003EE3 RID: 16099
		public TMP_Text habSectorHeaderName;

		// Token: 0x04003EE4 RID: 16100
		public TMP_Text habCouncilorHeaderLocation;

		// Token: 0x04003EE5 RID: 16101
		public TMP_Text hab_fleetName;

		// Token: 0x04003EE6 RID: 16102
		public TMP_Text hab_fleetAltitudeHeader;

		// Token: 0x04003EE7 RID: 16103
		public TMP_Text hab_fleetDVHeader;

		// Token: 0x04003EE8 RID: 16104
		public TMP_Text hab_fleetSmallShipsHeader;

		// Token: 0x04003EE9 RID: 16105
		public TMP_Text hab_fleetMediumShipsHeader;

		// Token: 0x04003EEA RID: 16106
		public TMP_Text hab_fleetLargeShipsHeader;

		// Token: 0x04003EEB RID: 16107
		public GameObject habGravityPanel;

		// Token: 0x04003EEC RID: 16108
		public TMP_Text localgravity_gs;

		// Token: 0x04003EED RID: 16109
		public TooltipTrigger localGravityTip;

		// Token: 0x04003EEE RID: 16110
		[Header("Selected Hab Customization")]
		public GameObject renameHabPanel;

		// Token: 0x04003EEF RID: 16111
		public TextMeshProUGUI saveHabNameText;

		// Token: 0x04003EF0 RID: 16112
		public TextMeshProUGUI revertHabNameText;

		// Token: 0x04003EF1 RID: 16113
		public TMP_InputField habNameInputField;

		// Token: 0x04003EF2 RID: 16114
		[Header("Lagrange Point Panel")]
		public TMP_Text lagrangePointName;

		// Token: 0x04003EF3 RID: 16115
		public TMP_Text lagrangeOrbitRadius;

		// Token: 0x04003EF4 RID: 16116
		public TMP_Text lagrangeDescription;

		// Token: 0x04003EF5 RID: 16117
		public TMP_Text lagrangeDescriptionLine2;

		// Token: 0x04003EF6 RID: 16118
		public TMP_Text lagrangePointNextLaunchWindow;

		// Token: 0x04003EF7 RID: 16119
		public ListManagerBase lagrangeOrbitsList;

		// Token: 0x04003EF8 RID: 16120
		public ListManagerBase lagrangeStationsList;

		// Token: 0x04003EF9 RID: 16121
		public ListManagerBase lagrangeCouncilorsList;

		// Token: 0x04003EFA RID: 16122
		public ListManagerBase lagrangeFleetsList;

		// Token: 0x04003EFB RID: 16123
		public TMP_Text lagrangeStationsTabHeader;

		// Token: 0x04003EFC RID: 16124
		public TMP_Text lagrangeOrbitsTabHeader;

		// Token: 0x04003EFD RID: 16125
		public TMP_Text lagrangeCouncilorsTabHeader;

		// Token: 0x04003EFE RID: 16126
		public TMP_Text lagrangeFleetsTabHeader;

		// Token: 0x04003EFF RID: 16127
		public Button lagrangeOrbitsTabButton;

		// Token: 0x04003F00 RID: 16128
		public Button lagrangeFleetsTabButton;

		// Token: 0x04003F01 RID: 16129
		public Button lagrangeCouncilorsTabButton;

		// Token: 0x04003F02 RID: 16130
		public Button lagrangeStationsTabButton;

		// Token: 0x04003F03 RID: 16131
		public GameObject backButtonLagrangeDetail;

		// Token: 0x04003F04 RID: 16132
		public TabbedPaneManager lagrangeTabbedPaneManager;

		// Token: 0x04003F05 RID: 16133
		public TabbedPaneController lagrangeOrbitsTab;

		// Token: 0x04003F06 RID: 16134
		public TabbedPaneController lagrangeFleetsTab;

		// Token: 0x04003F07 RID: 16135
		public TabbedPaneController lagrangeCouncilorsTab;

		// Token: 0x04003F08 RID: 16136
		public TabbedPaneController lagrangeStationsTab;

		// Token: 0x04003F09 RID: 16137
		public Button gotoLagrangeParentButton;

		// Token: 0x04003F0A RID: 16138
		public Button gotoLagrangeSecondaryButton;

		// Token: 0x04003F0B RID: 16139
		public Image lagrangeParentIcon;

		// Token: 0x04003F0C RID: 16140
		public Image lagrangeSecondaryIcon;

		// Token: 0x04003F0D RID: 16141
		public TooltipTrigger lagrangeOrbitTooltip;

		// Token: 0x04003F0E RID: 16142
		public TMP_Text lagrange_orbitHeaderName;

		// Token: 0x04003F0F RID: 16143
		public TMP_Text lagrange_orbitHeaderAltitude;

		// Token: 0x04003F10 RID: 16144
		public TooltipTrigger lagrange_orbitHeaderGs;

		// Token: 0x04003F11 RID: 16145
		public TooltipTrigger lagrange_orbitHeaderAMAT;

		// Token: 0x04003F12 RID: 16146
		public TMP_Text lagrange_stationHeaderName;

		// Token: 0x04003F13 RID: 16147
		public TMP_Text lagrange_stationHeaderAltitude;

		// Token: 0x04003F14 RID: 16148
		public TMP_Text lagrange_stationHeaderControlPoints;

		// Token: 0x04003F15 RID: 16149
		public TMP_Text lagrange_councilorHeaderName;

		// Token: 0x04003F16 RID: 16150
		public TMP_Text lagrange_councilorHeaderLocation;

		// Token: 0x04003F17 RID: 16151
		public TMP_Text lp_fleetName;

		// Token: 0x04003F18 RID: 16152
		public TMP_Text lp_fleetAltitudeHeader;

		// Token: 0x04003F19 RID: 16153
		public TMP_Text lp_fleetDVHeader;

		// Token: 0x04003F1A RID: 16154
		public TMP_Text lp_fleetSmallShipsHeader;

		// Token: 0x04003F1B RID: 16155
		public TMP_Text lp_fleetMediumShipsHeader;

		// Token: 0x04003F1C RID: 16156
		public TMP_Text lp_fleetLargeShipsHeader;

		// Token: 0x04003F1D RID: 16157
		public GameObject lp_populationPanel;

		// Token: 0x04003F1E RID: 16158
		public TMP_Text lp_populationValue;

		// Token: 0x04003F1F RID: 16159
		public Image lp_maxTierIcon;

		// Token: 0x04003F20 RID: 16160
		public TooltipTrigger lp_maxTierTip;

		// Token: 0x04003F21 RID: 16161
		public Image lagrangeSolarPotential;

		// Token: 0x04003F22 RID: 16162
		public TooltipTrigger lagrangeSolarTip;

		// Token: 0x04003F23 RID: 16163
		[Header("Cameras")]
		[SerializeField]
		private GameObject selectionCameraInstance;

		// Token: 0x04003F24 RID: 16164
		[SerializeField]
		private GameObject previewPosition;

		// Token: 0x04003F25 RID: 16165
		[SerializeField]
		private GameObject modelInstance;

		// Token: 0x04003F26 RID: 16166
		private Vector3 originalPreviewPosition;

		// Token: 0x04003F27 RID: 16167
		private Vector3 originalPreviewRotation;

		// Token: 0x04003F28 RID: 16168
		private float modelRotationRate;

		// Token: 0x04003F29 RID: 16169
		private Vector3 modelRotationAxis;

		// Token: 0x04003F2A RID: 16170
		private TIGameState infoPanelImageState;

		// Token: 0x04003F2B RID: 16171
		[SerializeField]
		private GameObject assetCameraInstance;

		// Token: 0x04003F2C RID: 16172
		[SerializeField]
		private GameObject assetPosition;

		// Token: 0x04003F2D RID: 16173
		[SerializeField]
		private GameObject assetModelInstance;

		// Token: 0x04003F2E RID: 16174
		private Vector3 originalAssetPreviewPosition;

		// Token: 0x04003F2F RID: 16175
		private Vector3 originalAssetPreviewRotation;

		// Token: 0x04003F30 RID: 16176
		private TIGameState assetPanelImageState;

		// Token: 0x04003F31 RID: 16177
		private List<TISpaceObjectState> previousSpacebodies = new List<TISpaceObjectState>();

		// Token: 0x04003F32 RID: 16178
		private bool myFleetDataDirtyMajor;

		// Token: 0x04003F33 RID: 16179
		private bool myFleetDataDirtyMinor;

		// Token: 0x04003F34 RID: 16180
		private bool enemyFleetDataDirtyMajor;

		// Token: 0x04003F35 RID: 16181
		private bool enemyFleetDataDirtyMinor;

		// Token: 0x04003F36 RID: 16182
		private bool spaceBodyDataDirtyMajor;

		// Token: 0x04003F37 RID: 16183
		private bool spaceBodyDataDirtyMinor;

		// Token: 0x04003F38 RID: 16184
		private bool habDataDirtyMajor;

		// Token: 0x04003F39 RID: 16185
		private bool habDataDirtyMinor;

		// Token: 0x04003F3A RID: 16186
		private bool lagrangeDataDirtyMinor;

		// Token: 0x04003F3B RID: 16187
		[Header("Debug")]
		public GameObject DebugPanel;

		// Token: 0x04003F3C RID: 16188
		public TMP_Text DebugText;
	}
}
