using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005A4 RID: 1444
	public class SpaceObjectSymbolController : MonoBehaviour
	{
		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x0600270F RID: 9999 RVA: 0x000D5364 File Offset: 0x000D3564
		// (set) Token: 0x06002710 RID: 10000 RVA: 0x000D536C File Offset: 0x000D356C
		public bool visible { get; private set; }

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x06002711 RID: 10001 RVA: 0x000D5375 File Offset: 0x000D3575
		// (set) Token: 0x06002712 RID: 10002 RVA: 0x000D537D File Offset: 0x000D357D
		public float scaleSize { get; private set; }

		// Token: 0x06002713 RID: 10003 RVA: 0x000D5388 File Offset: 0x000D3588
		public void InitializeSymbol(TISpaceObjectState spaceObject, SpaceObjectController parentController)
		{
			this.SetActivePlayer();
			this.parentSpaceObjectController = parentController;
			this.selectionAnimObject.SetActive(false);
			this.spaceObject = spaceObject;
			this.topSectorPanel.SetActive(false);
			this.topSectorConnector.SetActive(false);
			this.bottomSectorPanel.SetActive(false);
			this.bottomSectorConnector.SetActive(false);
			this.rightSectorPanel.SetActive(false);
			this.leftSectorPanel.SetActive(false);
			this.rightSectorConnector.SetActive(false);
			this.leftSectorConnector.SetActive(false);
			this.habSectorsPanel.SetActive(false);
			this.probeImage.gameObject.SetActive(false);
			this.assaultCarrierIcon.gameObject.SetActive(false);
			this.fleetsPanel.SetActive(false);
			this.councilorsPanel.SetActive(false);
			this.stationsPanel.SetActive(false);
			this.basesPanel.SetActive(false);
			this.habClassificationIconImage.gameObject.SetActive(false);
			this.playerTagImage.gameObject.SetActive(false);
			switch (spaceObject.objectType)
			{
			case SpaceObjectType.Star:
				this.symbolType = SpaceObjectSymbolType.Star;
				base.gameObject.SetActive(false);
				this.primaryCanvas.enabled = false;
				this.primaryCanvasRaycaster.enabled = false;
				this.outline.enabled = false;
				this.buttonImage.enabled = false;
				break;
			case SpaceObjectType.Planet:
			case SpaceObjectType.DwarfPlanet:
			case SpaceObjectType.Asteroid:
			case SpaceObjectType.Comet:
			case SpaceObjectType.PlanetaryMoon:
			case SpaceObjectType.AsteroidalMoon:
				this.symbolType = SpaceObjectSymbolType.SpaceBody;
				this.spaceBody = spaceObject.ref_spaceBody;
				this.outline.enabled = false;
				this.buttonImage.enabled = true;
				break;
			case SpaceObjectType.Fleet:
				this.symbolType = SpaceObjectSymbolType.Fleet;
				this.fleet = spaceObject.ref_fleet;
				if (this.fleet.faction == this.activePlayer)
				{
					this.hoverImage.sprite = GeneralControlsController.redReticle;
				}
				else
				{
					this.hoverImage.sprite = GeneralControlsController.greenReticle;
				}
				this.outline.enabled = true;
				this.buttonImage.enabled = true;
				break;
			case SpaceObjectType.Hab:
				if (spaceObject.ref_hab.IsStation)
				{
					this.symbolType = SpaceObjectSymbolType.Station;
					this.station = spaceObject.ref_hab;
					this.outline.enabled = true;
					this.buttonImage.enabled = true;
				}
				break;
			case SpaceObjectType.LagrangePoint:
				this.symbolType = SpaceObjectSymbolType.LagrangePoint;
				this.lagrangePoint = spaceObject.ref_lagrangePoint;
				this.outline.enabled = false;
				this.buttonImage.enabled = true;
				break;
			default:
				Log.Error("Bad space object type passed to spaceObjectSymbolController:" + spaceObject.displayName, Array.Empty<object>());
				return;
			}
			this.scaleSize = this.GetCanvasScaleSize() / 45f;
			this.SetAllSymbolInformation();
			if (spaceObject.isNaturalSpaceObjectState)
			{
				ViewControl.naturalSpaceSymbolTooltips.Add(this.tooltip);
			}
			this.selectionAnimating = false;
			this.SetSelected(false);
			this.SetListeners();
			if (GameControl.control.skirmishMode)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				this.SetVisible(this.symbolType == SpaceObjectSymbolType.SpaceBody);
			}
			this.selection = World.Active.GetExistingManager<SpaceObjectSelection>();
			this.UpdateUIScale();
			Loc.SwapFonts(base.gameObject);
			this.initialized = true;
		}

		// Token: 0x06002714 RID: 10004 RVA: 0x000D56CA File Offset: 0x000D38CA
		private void OnDestroy()
		{
			ViewControl.naturalSpaceSymbolTooltips.Remove(this.tooltip);
			this.RemoveListeners();
		}

		// Token: 0x06002715 RID: 10005 RVA: 0x000D56E4 File Offset: 0x000D38E4
		private void OnDisable()
		{
			if (this.initialized && this.selection != null && this.selection.spaceObjectStateSelected == this.spaceObject && this.spaceObject.isSpaceAssetState && !GeneralControlsController.IsCurrentlySelectedGameState(this.spaceObject) && !this.parentSpaceObjectController.modelLink.activeInHierarchy)
			{
				this.selection.SelectObject(this.spaceObject.barycenter.gameObjectLink, false, false);
			}
		}

		// Token: 0x06002716 RID: 10006 RVA: 0x000D5762 File Offset: 0x000D3962
		public void SetVisible(bool displaySymbol)
		{
			this.visible = displaySymbol;
		}

		// Token: 0x06002717 RID: 10007 RVA: 0x000D576C File Offset: 0x000D396C
		public void VisibilityChange()
		{
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.SpaceBody:
				if (this.visible)
				{
					this.SetAllSymbolInformation();
				}
				if (this.spaceBody.isaMoon)
				{
					GameControl.eventManager.TriggerEvent(new MoonSymbolVisibilityChange(this.spaceBody, this.visible), null, new object[]
					{
						this.spaceBody,
						this.spaceBody.barycenter
					});
					return;
				}
				break;
			case SpaceObjectSymbolType.LagrangePoint:
				if (this.visible)
				{
					this.SetAllSymbolInformation();
				}
				break;
			case SpaceObjectSymbolType.Fleet:
			{
				EventManager eventManager = GameControl.eventManager;
				GameEvent gameEvent = new FleetSymbolVisibilityChange(this.fleet, this.visible);
				string text = null;
				object[] array = new object[4];
				array[0] = this.fleet;
				array[1] = this.fleet.ref_naturalSpaceObject;
				int num = 2;
				Trajectory trajectory = this.fleet.trajectory;
				object obj;
				if (trajectory == null)
				{
					obj = null;
				}
				else
				{
					TIOrbitState originOrbit = trajectory.originOrbit;
					obj = ((originOrbit != null) ? originOrbit.ref_naturalSpaceObject : null);
				}
				array[num] = obj;
				int num2 = 3;
				Trajectory trajectory2 = this.fleet.trajectory;
				object obj2;
				if (trajectory2 == null)
				{
					obj2 = null;
				}
				else
				{
					TIOrbitState destinationOrbit = trajectory2.destinationOrbit;
					obj2 = ((destinationOrbit != null) ? destinationOrbit.ref_naturalSpaceObject : null);
				}
				array[num2] = obj2;
				eventManager.TriggerEvent(gameEvent, text, (from x in array.Distinct<object>()
					where x != null
					select x).ToArray<object>());
				return;
			}
			case SpaceObjectSymbolType.Station:
				if (this.visible)
				{
					this.SetAllSymbolInformation();
				}
				GameControl.eventManager.TriggerEvent(new StationSymbolVisibilityChange(this.station, this.visible), null, new object[]
				{
					this.station,
					this.station.ref_naturalSpaceObject,
					this.station.ref_naturalSpaceObject.barycenter
				});
				return;
			default:
				return;
			}
		}

		// Token: 0x06002718 RID: 10008 RVA: 0x000D590C File Offset: 0x000D3B0C
		private void SetListeners()
		{
			GameControl.eventManager.AddListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null, null, true, false);
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.SpaceBody:
				GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.FleetInfoUpdate), null, this.spaceBody, true, false);
				GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.HabInfoUpdate), null, this.spaceBody, true, false);
				GameControl.eventManager.AddListener<SpaceAssetDetected>(new EventManager.EventDelegate<SpaceAssetDetected>(this.AssetInfoUpdate), null, this.spaceBody, true, false);
				GameControl.eventManager.AddListener<StationSymbolVisibilityChange>(new EventManager.EventDelegate<StationSymbolVisibilityChange>(this.HabInfoUpdate), null, this.spaceBody, true, false);
				GameControl.eventManager.AddListener<FleetSymbolVisibilityChange>(new EventManager.EventDelegate<FleetSymbolVisibilityChange>(this.FleetInfoUpdate), null, this.spaceBody, true, false);
				GameControl.eventManager.AddListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.FleetInfoUpdate), null, this.spaceBody, true, false);
				GameControl.eventManager.AddListener<SpaceBodyTagChanged>(new EventManager.EventDelegate<SpaceBodyTagChanged>(this.SetPlayerTagIcon), null, this.spaceBody, true, false);
				GameControl.eventManager.AddListener<ResetShowAllColonizedNames>(new EventManager.EventDelegate<ResetShowAllColonizedNames>(this.SetShowName), null, null, true, false);
				GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null, this.spaceBody, true, false);
				if (this.spaceBody.habSites.Length != 0)
				{
					GameControl.eventManager.AddListener<ProspectingBody>(new EventManager.EventDelegate<ProspectingBody>(this.SetProspectedStatusIcon), null, this.spaceBody, true, false);
					GameControl.eventManager.AddListener<SpaceBodyProspected>(new EventManager.EventDelegate<SpaceBodyProspected>(this.SetProspectedStatusIcon), null, this.spaceBody, true, false);
					GameControl.eventManager.AddListener<FactionExplorationRangeChanged>(new EventManager.EventDelegate<FactionExplorationRangeChanged>(this.SetProspectedStatusIcon), null, this.spaceBody, true, false);
					GameControl.eventManager.AddListener<ResetProspectSymbols>(new EventManager.EventDelegate<ResetProspectSymbols>(this.SetProspectedStatusIcon), null, null, true, false);
				}
				if (this.spaceBody.isaMoon || this.spaceBody.naturalSatellites.Count > 0)
				{
					GameControl.eventManager.AddListener<MoonSymbolVisibilityChange>(new EventManager.EventDelegate<MoonSymbolVisibilityChange>(this.SpaceBodyInfoUpdate), null, this.spaceBody, true, false);
					return;
				}
				break;
			case SpaceObjectSymbolType.LagrangePoint:
				GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.FleetInfoUpdate), null, this.lagrangePoint, true, false);
				GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.HabInfoUpdate), null, this.station, true, false);
				GameControl.eventManager.AddListener<SpaceAssetDetected>(new EventManager.EventDelegate<SpaceAssetDetected>(this.AssetInfoUpdate), null, this.lagrangePoint, true, false);
				GameControl.eventManager.AddListener<FleetSymbolVisibilityChange>(new EventManager.EventDelegate<FleetSymbolVisibilityChange>(this.FleetInfoUpdate), null, this.lagrangePoint, true, false);
				GameControl.eventManager.AddListener<StationSymbolVisibilityChange>(new EventManager.EventDelegate<StationSymbolVisibilityChange>(this.HabInfoUpdate), null, this.lagrangePoint, true, false);
				GameControl.eventManager.AddListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.FleetInfoUpdate), null, this.lagrangePoint, true, false);
				GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null, this.lagrangePoint, true, false);
				return;
			case SpaceObjectSymbolType.Fleet:
				GameControl.eventManager.AddListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.FleetInfoUpdate), null, this.fleet, true, false);
				GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.FleetInfoUpdate), null, this.fleet, true, false);
				GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.FleetInfoUpdate), null, this.fleet, true, false);
				GameControl.eventManager.AddListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.FleetInfoUpdate), null, this.fleet, true, false);
				GameControl.eventManager.AddListener<SpaceAssetDetected>(new EventManager.EventDelegate<SpaceAssetDetected>(this.AssetInfoUpdate), null, this.fleet, true, false);
				GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.FleetInfoUpdate), null, this.fleet, true, false);
				return;
			case SpaceObjectSymbolType.Station:
				GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.FleetInfoUpdate), null, this.station, true, false);
				GameControl.eventManager.AddListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.FleetInfoUpdate), null, this.station, true, false);
				GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.HabInfoUpdate), null, this.station, true, false);
				GameControl.eventManager.AddListener<SpaceAssetDetected>(new EventManager.EventDelegate<SpaceAssetDetected>(this.AssetInfoUpdate), null, this.station, true, false);
				GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null, this.station, true, false);
				GameControl.eventManager.AddListener<HabSymbolAssigned>(new EventManager.EventDelegate<HabSymbolAssigned>(this.HabInfoUpdate), null, this.station, true, false);
				break;
			default:
				return;
			}
		}

		// Token: 0x06002719 RID: 10009 RVA: 0x000D5D7C File Offset: 0x000D3F7C
		private void RemoveListeners()
		{
			GameControl.eventManager.RemoveListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null);
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.SpaceBody:
				GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.HabInfoUpdate), null);
				GameControl.eventManager.RemoveListener<SpaceAssetDetected>(new EventManager.EventDelegate<SpaceAssetDetected>(this.AssetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<StationSymbolVisibilityChange>(new EventManager.EventDelegate<StationSymbolVisibilityChange>(this.HabInfoUpdate), null);
				GameControl.eventManager.RemoveListener<FleetSymbolVisibilityChange>(new EventManager.EventDelegate<FleetSymbolVisibilityChange>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<SpaceBodyTagChanged>(new EventManager.EventDelegate<SpaceBodyTagChanged>(this.SetPlayerTagIcon), null);
				GameControl.eventManager.RemoveListener<ResetShowAllColonizedNames>(new EventManager.EventDelegate<ResetShowAllColonizedNames>(this.SetShowName), null);
				if (this.spaceBody.habSites.Length != 0)
				{
					GameControl.eventManager.RemoveListener<FactionExplorationRangeChanged>(new EventManager.EventDelegate<FactionExplorationRangeChanged>(this.SetProspectedStatusIcon), null);
					GameControl.eventManager.RemoveListener<ProspectingBody>(new EventManager.EventDelegate<ProspectingBody>(this.SetProspectedStatusIcon), null);
					GameControl.eventManager.RemoveListener<SpaceBodyProspected>(new EventManager.EventDelegate<SpaceBodyProspected>(this.SetProspectedStatusIcon), null);
					GameControl.eventManager.RemoveListener<ResetProspectSymbols>(new EventManager.EventDelegate<ResetProspectSymbols>(this.SetProspectedStatusIcon), null);
				}
				if (this.spaceBody.isaMoon || this.spaceBody.naturalSatellites.Count > 0)
				{
					GameControl.eventManager.RemoveListener<MoonSymbolVisibilityChange>(new EventManager.EventDelegate<MoonSymbolVisibilityChange>(this.SpaceBodyInfoUpdate), null);
					return;
				}
				break;
			case SpaceObjectSymbolType.LagrangePoint:
				GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.HabInfoUpdate), null);
				GameControl.eventManager.RemoveListener<SpaceAssetDetected>(new EventManager.EventDelegate<SpaceAssetDetected>(this.AssetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<FleetSymbolVisibilityChange>(new EventManager.EventDelegate<FleetSymbolVisibilityChange>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<StationSymbolVisibilityChange>(new EventManager.EventDelegate<StationSymbolVisibilityChange>(this.HabInfoUpdate), null);
				GameControl.eventManager.RemoveListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.FleetInfoUpdate), null);
				return;
			case SpaceObjectSymbolType.Fleet:
				GameControl.eventManager.RemoveListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<SpaceAssetDetected>(new EventManager.EventDelegate<SpaceAssetDetected>(this.AssetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.FleetInfoUpdate), null);
				return;
			case SpaceObjectSymbolType.Station:
				GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.HabInfoUpdate), null);
				GameControl.eventManager.RemoveListener<SpaceAssetDetected>(new EventManager.EventDelegate<SpaceAssetDetected>(this.AssetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<FleetSymbolVisibilityChange>(new EventManager.EventDelegate<FleetSymbolVisibilityChange>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null);
				GameControl.eventManager.RemoveListener<FleetCoreStatusChange>(new EventManager.EventDelegate<FleetCoreStatusChange>(this.FleetInfoUpdate), null);
				GameControl.eventManager.RemoveListener<HabSymbolAssigned>(new EventManager.EventDelegate<HabSymbolAssigned>(this.HabInfoUpdate), null);
				break;
			default:
				return;
			}
		}

		// Token: 0x0600271A RID: 10010 RVA: 0x000D60D4 File Offset: 0x000D42D4
		public bool ShouldShowDisplayName()
		{
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.SpaceBody:
				if (this.spaceBody.mass_kg >= 3E+19)
				{
					return true;
				}
				if (GameControl.solarSystem.showAllColonizedNames)
				{
					return this.anyVisibleBases(this.spaceBody) || this.anyVisibleStations(this.spaceBody);
				}
				return this.spaceBody.habs.Any<TIHabState>((TIHabState x) => x.faction == this.activePlayer);
			case SpaceObjectSymbolType.Fleet:
				return true;
			}
			return false;
		}

		// Token: 0x0600271B RID: 10011 RVA: 0x000D6164 File Offset: 0x000D4364
		public void ShowDisplayName()
		{
			if (this.symbolType == SpaceObjectSymbolType.Fleet)
			{
				if (this.fleet.dockedAtStation)
				{
					int num = this.fleet.ref_hab.dockedFleets.IndexOf(this.fleet) + 1;
					this.objectName.transform.localPosition = new Vector3(0f, (float)(-64 + num * -32), 0f);
				}
				else
				{
					this.objectName.transform.localPosition = new Vector3(0f, -64f, 0f);
				}
			}
			this.objectName.SetText(this.spaceObject.GetDisplayName(this.activePlayer));
			this.objectName.enabled = true;
		}

		// Token: 0x0600271C RID: 10012 RVA: 0x000D621A File Offset: 0x000D441A
		public void HideDisplayName()
		{
			this.objectName.enabled = false;
		}

		// Token: 0x0600271D RID: 10013 RVA: 0x000D6228 File Offset: 0x000D4428
		public void SetDisplayName()
		{
			if (this.ShouldShowDisplayName())
			{
				this.ShowDisplayName();
				return;
			}
			this.HideDisplayName();
		}

		// Token: 0x0600271E RID: 10014 RVA: 0x000D6240 File Offset: 0x000D4440
		public void SetHoverImage()
		{
			SpaceObjectSymbolType spaceObjectSymbolType = this.symbolType;
			if (spaceObjectSymbolType == SpaceObjectSymbolType.Fleet)
			{
				this.hoverImage.sprite = ((this.fleet.faction == this.activePlayer) ? GeneralControlsController.greenReticle : GeneralControlsController.redReticle);
				return;
			}
			if (spaceObjectSymbolType != SpaceObjectSymbolType.Station)
			{
				this.hoverImage.sprite = GeneralControlsController.cyanReticle;
				return;
			}
			this.hoverImage.sprite = ((this.station.faction == this.activePlayer) ? GeneralControlsController.greenReticle : GeneralControlsController.redReticle);
		}

		// Token: 0x0600271F RID: 10015 RVA: 0x000D62CE File Offset: 0x000D44CE
		private bool anyVisibleBases(TISpaceBodyState spaceBody)
		{
			return spaceBody.surfaceBases.Any<TIHabState>((TIHabState x) => x.VisibleToFaction(this.activePlayer));
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x000D62E7 File Offset: 0x000D44E7
		private bool anyVisibleStations(TINaturalSpaceObjectState naturalSpaceSpaceobject)
		{
			return naturalSpaceSpaceobject.stationsInOrbit.Any<TIHabState>((TIHabState x) => x.VisibleToFaction(this.activePlayer));
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x000D6300 File Offset: 0x000D4500
		private IEnumerable<TICouncilorState> visibleCouncilors(TISpaceFleetState fleet)
		{
			return fleet.CouncilorsPresentAndKnownToFaction(this.activePlayer);
		}

		// Token: 0x06002722 RID: 10018 RVA: 0x000D630E File Offset: 0x000D450E
		private IEnumerable<TICouncilorState> visibleCouncilors(TIHabState hab)
		{
			return hab.CouncilorsPresentAndKnownToFaction(this.activePlayer, false, null);
		}

		// Token: 0x06002723 RID: 10019 RVA: 0x000D6320 File Offset: 0x000D4520
		private bool ShouldShowSymbol()
		{
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.Star:
				return false;
			case SpaceObjectSymbolType.Fleet:
				return this.fleet.VisibleToFaction(this.activePlayer);
			case SpaceObjectSymbolType.Station:
				return this.station.VisibleToFaction(this.activePlayer);
			}
			return true;
		}

		// Token: 0x06002724 RID: 10020 RVA: 0x000D6377 File Offset: 0x000D4577
		public void SetActivePlayer()
		{
			this.activePlayer = GameControl.control.activePlayer;
			this.SetAllSymbolInformation();
		}

		// Token: 0x06002725 RID: 10021 RVA: 0x000D638F File Offset: 0x000D458F
		private void SetPrimaryImage()
		{
			this.buttonImage.sprite = this.spaceObject.icon;
		}

		// Token: 0x06002726 RID: 10022 RVA: 0x000D63A8 File Offset: 0x000D45A8
		private void SetTooltip()
		{
			SpaceObjectSymbolType spaceObjectSymbolType = this.symbolType;
			if (spaceObjectSymbolType - SpaceObjectSymbolType.SpaceBody <= 1)
			{
				this.tooltip.SetDelegate("BodyText", () => this.spaceObject.ref_naturalSpaceObject.SummaryTooltip(this.activePlayer));
				return;
			}
			if (spaceObjectSymbolType == SpaceObjectSymbolType.Fleet)
			{
				this.tooltip.SetDelegate("BodyText", () => this.spaceObject.ref_fleet.FleetQuickDescription(this.activePlayer));
				return;
			}
			this.tooltip.SetDelegate("BodyText", () => this.spaceObject.GetDisplayName(this.activePlayer));
		}

		// Token: 0x06002727 RID: 10023 RVA: 0x000D641C File Offset: 0x000D461C
		private void SetAllSymbolInformation()
		{
			if (!this.ShouldShowSymbol())
			{
				if (this.primaryCanvas != null)
				{
					this.primaryCanvas.enabled = false;
					this.primaryCanvasRaycaster.enabled = false;
				}
				return;
			}
			this.primaryCanvas.enabled = true;
			this.primaryCanvasRaycaster.enabled = true;
			this.SetDisplayName();
			this.SetPrimaryImage();
			this.SetHoverImage();
			this.SetTooltip();
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.SpaceBody:
				this.UpdateFleetsWithinObjectBounds();
				this.UpdateHabsWithinObjectBounds();
				this.UpdateCouncilorsWithinObjectBounds();
				this.SetProspectedStatusIcon();
				this.SetPlayerTagIcon();
				return;
			case SpaceObjectSymbolType.LagrangePoint:
				this.UpdateFleetsWithinObjectBounds();
				this.UpdateHabsWithinObjectBounds();
				this.UpdateCouncilorsWithinObjectBounds();
				return;
			case SpaceObjectSymbolType.Fleet:
				this.UpdateCouncilorsWithinObjectBounds();
				this.UpdateAssaultCarrierIcon();
				return;
			case SpaceObjectSymbolType.Station:
				this.HabInfoUpdate(this.station);
				this.UpdateFleetsWithinObjectBounds();
				this.UpdateCouncilorsWithinObjectBounds();
				return;
			default:
				return;
			}
		}

		// Token: 0x06002728 RID: 10024 RVA: 0x000D6504 File Offset: 0x000D4704
		private void AssetInfoUpdate(SpaceAssetDetected e)
		{
			if (e.faction == this.activePlayer)
			{
				if (e.asset.isSpaceFleetState)
				{
					this.FleetInfoUpdate(e.asset.ref_fleet);
					return;
				}
				if (e.asset.isHabState)
				{
					this.HabInfoUpdate(e.asset.ref_hab);
				}
			}
		}

		// Token: 0x06002729 RID: 10025 RVA: 0x000D6561 File Offset: 0x000D4761
		private void FleetInfoUpdate(FleetArrivesAtDestination e)
		{
			this.FleetInfoUpdate(e.fleet);
		}

		// Token: 0x0600272A RID: 10026 RVA: 0x000D656F File Offset: 0x000D476F
		private void FleetInfoUpdate(ShipsAddedToFleet e)
		{
			this.FleetInfoUpdate(e.fleet);
		}

		// Token: 0x0600272B RID: 10027 RVA: 0x000D657D File Offset: 0x000D477D
		private void FleetInfoUpdate(ShipsRemovedFromFleet e)
		{
			this.FleetInfoUpdate(e.fleet);
		}

		// Token: 0x0600272C RID: 10028 RVA: 0x000D658B File Offset: 0x000D478B
		private void FleetInfoUpdate(FleetSymbolVisibilityChange e)
		{
			this.FleetInfoUpdate(e.fleet);
		}

		// Token: 0x0600272D RID: 10029 RVA: 0x000D6599 File Offset: 0x000D4799
		private void FleetInfoUpdate(FleetCoreStatusChange e)
		{
			this.FleetInfoUpdate(e.fleet);
		}

		// Token: 0x0600272E RID: 10030 RVA: 0x000D65A7 File Offset: 0x000D47A7
		private void FleetInfoUpdate(FleetUndocks e)
		{
			this.FleetInfoUpdate(e.fleet);
		}

		// Token: 0x0600272F RID: 10031 RVA: 0x000D65B8 File Offset: 0x000D47B8
		private void FleetInfoUpdate(TISpaceFleetState updatedFleet)
		{
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.SpaceBody:
				this.UpdateFleetsWithinObjectBounds();
				return;
			case SpaceObjectSymbolType.LagrangePoint:
				this.UpdateFleetsWithinObjectBounds();
				return;
			case SpaceObjectSymbolType.Fleet:
				this.SetAllSymbolInformation();
				return;
			case SpaceObjectSymbolType.Station:
				this.UpdateFleetsWithinObjectBounds();
				return;
			default:
				return;
			}
		}

		// Token: 0x06002730 RID: 10032 RVA: 0x000D6600 File Offset: 0x000D4800
		private void CouncilorInfoUpdate()
		{
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.SpaceBody:
				if (!this.spaceBody.isEarth)
				{
					this.UpdateCouncilorsWithinObjectBounds();
					return;
				}
				break;
			case SpaceObjectSymbolType.LagrangePoint:
				this.UpdateCouncilorsWithinObjectBounds();
				return;
			case SpaceObjectSymbolType.Fleet:
				this.UpdateCouncilorsWithinObjectBounds();
				return;
			case SpaceObjectSymbolType.Station:
				this.UpdateCouncilorsWithinObjectBounds();
				break;
			default:
				return;
			}
		}

		// Token: 0x06002731 RID: 10033 RVA: 0x000D6655 File Offset: 0x000D4855
		private void SpaceBodyInfoUpdate(MoonSymbolVisibilityChange e)
		{
			this.SetAllSymbolInformation();
		}

		// Token: 0x06002732 RID: 10034 RVA: 0x000D665D File Offset: 0x000D485D
		private void HabInfoUpdate(SectorAssignedToFaction e)
		{
			this.HabInfoUpdate(e.sector.hab);
		}

		// Token: 0x06002733 RID: 10035 RVA: 0x000D6670 File Offset: 0x000D4870
		private void HabInfoUpdate(HabSymbolAssigned e)
		{
			this.HabInfoUpdate(e.hab);
		}

		// Token: 0x06002734 RID: 10036 RVA: 0x000D667E File Offset: 0x000D487E
		private void HabInfoUpdate(StationSymbolVisibilityChange e)
		{
			this.HabInfoUpdate(e.hab);
		}

		// Token: 0x06002735 RID: 10037 RVA: 0x000D668C File Offset: 0x000D488C
		private void OnHabDestroyed(HabDestroyed e)
		{
			this.SetAllSymbolInformation();
		}

		// Token: 0x06002736 RID: 10038 RVA: 0x000D6694 File Offset: 0x000D4894
		private void HabInfoUpdate(TIHabState updatedHab)
		{
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.SpaceBody:
			case SpaceObjectSymbolType.LagrangePoint:
				this.UpdateHabsWithinObjectBounds();
				return;
			case SpaceObjectSymbolType.Fleet:
				break;
			case SpaceObjectSymbolType.Station:
				if (this.station == updatedHab)
				{
					this.SetStationSymbol();
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06002737 RID: 10039 RVA: 0x000D66DC File Offset: 0x000D48DC
		private void SetStationSymbol()
		{
			this.habSectorsPanel.SetActive(true);
			switch (this.station.tier)
			{
			case 1:
				this.topSectorPanel.SetActive(false);
				this.topSectorConnector.SetActive(false);
				this.bottomSectorPanel.SetActive(false);
				this.bottomSectorConnector.SetActive(false);
				this.rightSectorPanel.SetActive(false);
				this.leftSectorPanel.SetActive(false);
				this.rightSectorConnector.SetActive(false);
				this.leftSectorConnector.SetActive(false);
				break;
			case 2:
				this.topSectorPanel.SetActive(false);
				this.topSectorConnector.SetActive(false);
				this.bottomSectorPanel.SetActive(false);
				this.bottomSectorConnector.SetActive(false);
				this.rightSectorPanel.SetActive(true);
				this.leftSectorPanel.SetActive(true);
				this.rightSectorConnector.SetActive(true);
				this.leftSectorConnector.SetActive(true);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.station.sectors[2].faction.template.habSectorIcon, this.rightSectorImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.station.sectors[4].faction.template.habSectorIcon, this.leftSectorImage);
				break;
			case 3:
				this.topSectorPanel.SetActive(true);
				this.topSectorConnector.SetActive(true);
				this.bottomSectorPanel.SetActive(true);
				this.bottomSectorConnector.SetActive(true);
				this.rightSectorPanel.SetActive(true);
				this.leftSectorPanel.SetActive(true);
				this.rightSectorConnector.SetActive(true);
				this.leftSectorConnector.SetActive(true);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.station.sectors[4].faction.template.habSectorIcon, this.leftSectorImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.station.sectors[3].faction.template.habSectorIcon, this.bottomSectorImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.station.sectors[2].faction.template.habSectorIcon, this.rightSectorImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.station.sectors[1].faction.template.habSectorIcon, this.topSectorImage);
				break;
			}
			if (!string.IsNullOrEmpty(this.station.customHabIconResource))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(this.station.customHabIconResource, this.habClassificationIconImage);
				this.habClassificationIconImage.gameObject.SetActive(true);
			}
			else
			{
				this.habClassificationIconImage.gameObject.SetActive(false);
			}
			this.UpdateCouncilorsWithinObjectBounds();
			this.UpdateFleetsWithinObjectBounds();
		}

		// Token: 0x06002738 RID: 10040 RVA: 0x000D69C0 File Offset: 0x000D4BC0
		public void OnSectorClicked(int sectorValue)
		{
			SoundEffectController.PlaySelectSound(this.station.sectors[sectorValue]);
			if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TISectorState)))
			{
				GameControl.eventManager.TriggerEvent(new SectorSelectedEvent(this.station.sectors[sectorValue]), null, new object[] { this.station.sectors[sectorValue] });
				return;
			}
			TIUtilities.GotoGameState(this.station, true, true, true, true, false, -1f);
		}

		// Token: 0x06002739 RID: 10041 RVA: 0x000D6A48 File Offset: 0x000D4C48
		private void UpdateFleetsWithinObjectBounds()
		{
			if (SpaceObjectSymbolController.fleetCache != TIFrameCounter.FrameCount)
			{
				SpaceObjectSymbolController._activePlayerKnownFleets = this.activePlayer.KnownFleets;
				SpaceObjectSymbolController.fleetCache = TIFrameCounter.FrameCount;
			}
			List<TIFactionState> list = new List<TIFactionState>();
			foreach (TISpaceFleetState tispaceFleetState in SpaceObjectSymbolController._activePlayerKnownFleets)
			{
				if (!list.Contains(tispaceFleetState.faction) && TIGameState.Valid(tispaceFleetState))
				{
					SpaceObjectController controller = tispaceFleetState.controller;
					if (controller != null && !controller.symbolController.visible)
					{
						if (tispaceFleetState.barycenter == this.spaceObject)
						{
							list.Add(tispaceFleetState.faction);
						}
						else if (tispaceFleetState.barycenter.isaMoon && tispaceFleetState.barycenter.barycenter == this.spaceObject && tispaceFleetState.barycenter.controller != null && !tispaceFleetState.barycenter.controller.symbolController.visible)
						{
							list.Add(tispaceFleetState.faction);
						}
						else if (tispaceFleetState.inTransfer)
						{
							TIOrbitState originOrbit = tispaceFleetState.trajectory.originOrbit;
							TIGameState tigameState;
							if (originOrbit == null)
							{
								tigameState = null;
							}
							else
							{
								TINaturalSpaceObjectState barycenter = originOrbit.barycenter;
								tigameState = ((barycenter != null) ? barycenter.GetSunOrbitingRelatedObject : null);
							}
							if (!(tigameState == this.spaceObject.GetSunOrbitingRelatedObject))
							{
								TISpaceGameState destination = tispaceFleetState.trajectory.destination;
								TIGameState tigameState2;
								if (destination == null)
								{
									tigameState2 = null;
								}
								else
								{
									TINaturalSpaceObjectState barycenter2 = destination.barycenter;
									tigameState2 = ((barycenter2 != null) ? barycenter2.GetSunOrbitingRelatedObject : null);
								}
								if (!(tigameState2 == this.spaceObject.GetSunOrbitingRelatedObject))
								{
									continue;
								}
							}
							if (tispaceFleetState.InSphereOfInfluence(this.spaceObject))
							{
								list.Add(tispaceFleetState.faction);
							}
						}
					}
				}
			}
			if (list.Count == 0)
			{
				this.fleetsPanel.SetActive(false);
				return;
			}
			list.Sort((TIFactionState a, TIFactionState b) => a.ID.CompareTo(b.ID));
			this.fleetsPanel.SetActive(true);
			this.fleetsList.SetListSize<SpaceObjectSymbolMarkerPanelGridItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator2 = this.fleetsList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (SpaceObjectSymbolController.<>o__95.<>p__0 == null)
					{
						SpaceObjectSymbolController.<>o__95.<>p__0 = CallSite<Func<CallSite, object, SpaceObjectSymbolMarkerPanelGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SpaceObjectSymbolMarkerPanelGridItemController), typeof(SpaceObjectSymbolController)));
					}
					SpaceObjectSymbolController.<>o__95.<>p__0.Target(SpaceObjectSymbolController.<>o__95.<>p__0, enumerator2.Current).UpdateFleetGridItem(list[num++]);
				}
			}
		}

		// Token: 0x0600273A RID: 10042 RVA: 0x000D6D18 File Offset: 0x000D4F18
		private void UpdateCouncilorsWithinObjectBounds()
		{
			if (new List<TICouncilorState>().Count == 0)
			{
				this.councilorsPanel.SetActive(false);
				return;
			}
			this.councilorsPanel.SetActive(true);
		}

		// Token: 0x0600273B RID: 10043 RVA: 0x000D6D40 File Offset: 0x000D4F40
		private void UpdateHabsWithinObjectBounds()
		{
			List<TIHabState> list = new List<TIHabState>(this.spaceObject.ref_naturalSpaceObject.habsInSystem);
			if (list.Count > 0)
			{
				switch (this.symbolType)
				{
				case SpaceObjectSymbolType.Star:
				case SpaceObjectSymbolType.LagrangePoint:
					list = list.Intersect<TIHabState>(this.activePlayer.KnownStations).ToList<TIHabState>();
					break;
				case SpaceObjectSymbolType.SpaceBody:
					list = list.Intersect<TIHabState>(this.activePlayer.KnownHabs).ToList<TIHabState>();
					break;
				}
			}
			List<TIFactionState> list2 = new List<TIFactionState>();
			List<TIFactionState> list3 = new List<TIFactionState>();
			foreach (TIHabState tihabState in list)
			{
				if (tihabState.barycenter == this.spaceObject || (tihabState.barycenter.isaMoon && tihabState.barycenter.barycenter == this.spaceObject && tihabState.barycenter.controller != null && !tihabState.barycenter.controller.symbolController.visible))
				{
					if (tihabState.IsStation)
					{
						list2.AddRange(tihabState.ref_factions);
					}
					else
					{
						list3.AddRange(tihabState.ref_factions);
					}
				}
			}
			if (list2.Count == 0)
			{
				this.stationsPanel.SetActive(false);
			}
			else
			{
				list2 = list2.Distinct<TIFactionState>().ToList<TIFactionState>();
				this.stationsPanel.SetActive(true);
				this.stationsList.SetListSize<SpaceObjectSymbolMarkerPanelGridItemController>(list2.Count, false, false);
				int num = 0;
				using (IEnumerator<object> enumerator2 = this.stationsList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (SpaceObjectSymbolController.<>o__97.<>p__0 == null)
						{
							SpaceObjectSymbolController.<>o__97.<>p__0 = CallSite<Func<CallSite, object, SpaceObjectSymbolMarkerPanelGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SpaceObjectSymbolMarkerPanelGridItemController), typeof(SpaceObjectSymbolController)));
						}
						SpaceObjectSymbolController.<>o__97.<>p__0.Target(SpaceObjectSymbolController.<>o__97.<>p__0, enumerator2.Current).UpdateStationGridItem(list2[num++]);
					}
				}
			}
			if (list3.Count == 0)
			{
				this.basesPanel.SetActive(false);
				return;
			}
			list3 = list3.Distinct<TIFactionState>().ToList<TIFactionState>();
			this.basesPanel.SetActive(true);
			this.basesList.SetListSize<SpaceObjectSymbolMarkerPanelGridItemController>(list3.Count, false, false);
			int num2 = 0;
			using (IEnumerator<object> enumerator2 = this.basesList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (SpaceObjectSymbolController.<>o__97.<>p__1 == null)
					{
						SpaceObjectSymbolController.<>o__97.<>p__1 = CallSite<Func<CallSite, object, SpaceObjectSymbolMarkerPanelGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SpaceObjectSymbolMarkerPanelGridItemController), typeof(SpaceObjectSymbolController)));
					}
					SpaceObjectSymbolController.<>o__97.<>p__1.Target(SpaceObjectSymbolController.<>o__97.<>p__1, enumerator2.Current).UpdateBaseGridItem(list3[num2++]);
				}
			}
		}

		// Token: 0x0600273C RID: 10044 RVA: 0x000D7034 File Offset: 0x000D5234
		public void SetSelected(bool selected)
		{
			if (selected)
			{
				SpaceObjectType objectType = this.spaceObject.objectType;
				int num;
				if (objectType - SpaceObjectType.Fleet <= 1)
				{
					num = ((this.spaceObject.ref_spaceAsset.faction == this.activePlayer) ? 0 : 1);
				}
				else
				{
					num = 2;
				}
				this.AssignAnimationToHighlightSprite(num);
				this.StartHighlightAnimation();
				return;
			}
			this.StopHighlightAnimation();
		}

		// Token: 0x0600273D RID: 10045 RVA: 0x000D7090 File Offset: 0x000D5290
		public void AssignAnimationToHighlightSprite(int animationValue)
		{
			Sprite sprite;
			RuntimeAnimatorController runtimeAnimatorController;
			switch (animationValue)
			{
			case 0:
				sprite = Resources.Load<Sprite>("Square Reticle/GreenSquare/GreenSquareReticleSS");
				runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/GreenSquare/GreenAnimator");
				this.selectionAnimatorController = runtimeAnimatorController;
				this.selectionRenderer.sprite = sprite;
				goto IL_0093;
			case 1:
				sprite = Resources.Load<Sprite>("Square Reticle/RedSquare/RedSquareReticleSS");
				runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/RedSquare/RedAnimator");
				this.selectionAnimatorController = runtimeAnimatorController;
				this.selectionRenderer.sprite = sprite;
				goto IL_0093;
			}
			sprite = Resources.Load<Sprite>("Square Reticle/CyanSquare/CyanSquareReticleSS");
			runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/CyanSquare/CyanAnimator");
			this.selectionAnimatorController = runtimeAnimatorController;
			this.selectionRenderer.sprite = sprite;
			IL_0093:
			this.selectionAnim.runtimeAnimatorController = runtimeAnimatorController;
		}

		// Token: 0x0600273E RID: 10046 RVA: 0x000D713C File Offset: 0x000D533C
		public void StartHighlightAnimation()
		{
			this.StopHighlightAnimation();
			this.selectionAnimObject.SetActive(true);
			this.selectionAnimating = true;
			if (this.selectionAnim.isActiveAndEnabled)
			{
				this.selectionAnim.ResetTrigger("Exit");
				this.selectionAnim.SetTrigger("Active");
			}
		}

		// Token: 0x0600273F RID: 10047 RVA: 0x000D7190 File Offset: 0x000D5390
		public void StopHighlightAnimation()
		{
			if (this.selectionAnimating)
			{
				if (this.selectionAnim.isActiveAndEnabled)
				{
					this.selectionAnim.SetTrigger("Exit");
					this.selectionAnim.ResetTrigger("Active");
				}
				this.selectionAnimObject.SetActive(false);
				this.selectionAnimating = false;
			}
		}

		// Token: 0x06002740 RID: 10048 RVA: 0x000D71E5 File Offset: 0x000D53E5
		public void SetButtonTexture(Sprite newTexture)
		{
			if (newTexture != null)
			{
				this.buttonImage.sprite = newTexture;
			}
		}

		// Token: 0x06002741 RID: 10049 RVA: 0x000D71FC File Offset: 0x000D53FC
		private float GetCanvasScaleSize()
		{
			switch (this.symbolType)
			{
			case SpaceObjectSymbolType.Star:
			case SpaceObjectSymbolType.SpaceBody:
				if (this.spaceObject.mass_kg >= 5E+26)
				{
					return 55f;
				}
				if (this.spaceObject.mass_kg >= 1E+25)
				{
					return 50f;
				}
				if (this.spaceObject.mass_kg >= 5E+23)
				{
					return 45f;
				}
				if (this.spaceObject.mass_kg >= 1E+22)
				{
					return 40f;
				}
				if (this.spaceObject.mass_kg >= 5E+20)
				{
					return 35f;
				}
				if (this.spaceObject.mass_kg >= 1E+19)
				{
					return 30f;
				}
				if (this.spaceObject.mass_kg >= 50000000000000000.0)
				{
					return 25f;
				}
				return 20f;
			case SpaceObjectSymbolType.LagrangePoint:
				return 25f;
			case SpaceObjectSymbolType.Fleet:
				return 45f;
			case SpaceObjectSymbolType.Station:
				return 45f;
			default:
				return 30f;
			}
		}

		// Token: 0x06002742 RID: 10050 RVA: 0x000D7310 File Offset: 0x000D5510
		private void OnUIScaleChanged(UIScaleSettingChange e)
		{
			this.UpdateUIScale();
		}

		// Token: 0x06002743 RID: 10051 RVA: 0x000D7318 File Offset: 0x000D5518
		private void UpdateUIScale()
		{
			float num = 24f;
			num *= TIUtilities.UIScaleFactor();
			this.objectName.fontSize = num;
		}

		// Token: 0x06002744 RID: 10052 RVA: 0x000D733F File Offset: 0x000D553F
		private void OnFireMissionOrderReceived(FireMissionOrder e)
		{
			if (this.fleet.ships.Contains(e.ship))
			{
				bool activeInHierarchy = base.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x06002745 RID: 10053 RVA: 0x000D7365 File Offset: 0x000D5565
		private void SetProspectedStatusIcon(FactionExplorationRangeChanged e)
		{
			this.SetProspectedStatusIcon();
		}

		// Token: 0x06002746 RID: 10054 RVA: 0x000D736D File Offset: 0x000D556D
		private void SetProspectedStatusIcon(ProspectingBody e)
		{
			this.SetProspectedStatusIcon();
		}

		// Token: 0x06002747 RID: 10055 RVA: 0x000D7375 File Offset: 0x000D5575
		private void SetProspectedStatusIcon(SpaceBodyProspected e)
		{
			this.SetProspectedStatusIcon();
		}

		// Token: 0x06002748 RID: 10056 RVA: 0x000D737D File Offset: 0x000D557D
		private void SetProspectedStatusIcon(ResetProspectSymbols e)
		{
			this.SetProspectedStatusIcon();
		}

		// Token: 0x06002749 RID: 10057 RVA: 0x000D7385 File Offset: 0x000D5585
		private void SetShowName(ResetShowAllColonizedNames e)
		{
			if (this.ShouldShowSymbol())
			{
				this.SetDisplayName();
			}
		}

		// Token: 0x0600274A RID: 10058 RVA: 0x000D7395 File Offset: 0x000D5595
		private void SetPlayerTagIcon(SpaceBodyTagChanged e)
		{
			this.SetPlayerTagIcon();
		}

		// Token: 0x0600274B RID: 10059 RVA: 0x000D73A0 File Offset: 0x000D55A0
		private void SetPlayerTagIcon()
		{
			switch (this.spaceBody.playerTag)
			{
			case PlayerTag.Red:
				this.playerTagImage.gameObject.SetActive(true);
				this.playerTagImage.color = SpaceObjectSymbolController.PlanetTagRed;
				return;
			case PlayerTag.Green:
				this.playerTagImage.gameObject.SetActive(true);
				this.playerTagImage.color = SpaceObjectSymbolController.PlanetTagGreen;
				return;
			}
			this.playerTagImage.gameObject.SetActive(false);
		}

		// Token: 0x0600274C RID: 10060 RVA: 0x000D7424 File Offset: 0x000D5624
		private void SetProspectedStatusIcon()
		{
			if (!GameControl.solarSystem.showProspectData || this.spaceBody.habSites.Length == 0 || !this.spaceBody.surfaceBases.None<TIHabState>((TIHabState x) => x.faction == this.activePlayer))
			{
				this.probeImage.gameObject.SetActive(false);
				return;
			}
			if (this.activePlayer.Prospected(this.spaceBody))
			{
				this.probeImage.gameObject.SetActive(true);
				this.probeImage.sprite = AssetCacheManager.prospectedIcon;
				return;
			}
			if (this.activePlayer.ProspectorEnRoute(this.spaceBody))
			{
				this.probeImage.gameObject.SetActive(true);
				this.probeImage.sprite = AssetCacheManager.prospectingUnderway;
				return;
			}
			if (this.activePlayer.CanExplore(this.spaceBody))
			{
				this.probeImage.gameObject.SetActive(true);
				this.probeImage.sprite = AssetCacheManager.notProspectedHabSiteIcon;
				return;
			}
			this.probeImage.gameObject.SetActive(false);
		}

		// Token: 0x0600274D RID: 10061 RVA: 0x000D7534 File Offset: 0x000D5734
		private void UpdateAssaultCarrierIcon()
		{
			if (this.fleet.IsAlien())
			{
				this.assaultCarrierIcon.gameObject.SetActive(this.fleet.InvasionFleet());
			}
		}

		// Token: 0x04001D03 RID: 7427
		private TISpaceObjectState spaceObject;

		// Token: 0x04001D04 RID: 7428
		private TIHabState station;

		// Token: 0x04001D05 RID: 7429
		private TISpaceFleetState fleet;

		// Token: 0x04001D06 RID: 7430
		private TILagrangePointState lagrangePoint;

		// Token: 0x04001D07 RID: 7431
		private TISpaceBodyState spaceBody;

		// Token: 0x04001D08 RID: 7432
		public Canvas primaryCanvas;

		// Token: 0x04001D09 RID: 7433
		public GraphicRaycaster primaryCanvasRaycaster;

		// Token: 0x04001D0A RID: 7434
		public GameObject selectionAnimObject;

		// Token: 0x04001D0B RID: 7435
		public Animator selectionAnim;

		// Token: 0x04001D0C RID: 7436
		public SpriteRenderer selectionRenderer;

		// Token: 0x04001D0D RID: 7437
		private RuntimeAnimatorController selectionAnimatorController;

		// Token: 0x04001D0E RID: 7438
		public Image hoverImage;

		// Token: 0x04001D0F RID: 7439
		public Image buttonImage;

		// Token: 0x04001D10 RID: 7440
		public TMP_Text objectName;

		// Token: 0x04001D11 RID: 7441
		public TooltipTrigger tooltip;

		// Token: 0x04001D12 RID: 7442
		private bool selectionAnimating;

		// Token: 0x04001D14 RID: 7444
		public Outline outline;

		// Token: 0x04001D15 RID: 7445
		public GameObject fleetsPanel;

		// Token: 0x04001D16 RID: 7446
		public GameObject councilorsPanel;

		// Token: 0x04001D17 RID: 7447
		public GameObject stationsPanel;

		// Token: 0x04001D18 RID: 7448
		public GameObject basesPanel;

		// Token: 0x04001D19 RID: 7449
		public ListManagerBase fleetsList;

		// Token: 0x04001D1A RID: 7450
		public ListManagerBase councilorsList;

		// Token: 0x04001D1B RID: 7451
		public ListManagerBase stationsList;

		// Token: 0x04001D1C RID: 7452
		public ListManagerBase basesList;

		// Token: 0x04001D1D RID: 7453
		public GameObject habSectorsPanel;

		// Token: 0x04001D1E RID: 7454
		public GameObject leftSectorPanel;

		// Token: 0x04001D1F RID: 7455
		public Image leftSectorImage;

		// Token: 0x04001D20 RID: 7456
		public GameObject leftSectorConnector;

		// Token: 0x04001D21 RID: 7457
		public GameObject topSectorPanel;

		// Token: 0x04001D22 RID: 7458
		public GameObject topSectorConnector;

		// Token: 0x04001D23 RID: 7459
		public Image topSectorImage;

		// Token: 0x04001D24 RID: 7460
		public GameObject rightSectorPanel;

		// Token: 0x04001D25 RID: 7461
		public GameObject rightSectorConnector;

		// Token: 0x04001D26 RID: 7462
		public Image rightSectorImage;

		// Token: 0x04001D27 RID: 7463
		public GameObject bottomSectorPanel;

		// Token: 0x04001D28 RID: 7464
		public GameObject bottomSectorConnector;

		// Token: 0x04001D29 RID: 7465
		public Image bottomSectorImage;

		// Token: 0x04001D2A RID: 7466
		public Image probeImage;

		// Token: 0x04001D2B RID: 7467
		public Image habClassificationIconImage;

		// Token: 0x04001D2C RID: 7468
		public Image assaultCarrierIcon;

		// Token: 0x04001D2D RID: 7469
		public Image playerTagImage;

		// Token: 0x04001D2E RID: 7470
		private SpaceObjectController parentSpaceObjectController;

		// Token: 0x04001D2F RID: 7471
		private SpaceObjectSelection selection;

		// Token: 0x04001D30 RID: 7472
		private TIFactionState activePlayer;

		// Token: 0x04001D32 RID: 7474
		[SerializeField]
		private SpaceObjectSymbolType symbolType;

		// Token: 0x04001D33 RID: 7475
		private bool initialized;

		// Token: 0x04001D34 RID: 7476
		private static List<TISpaceFleetState> _activePlayerKnownFleets;

		// Token: 0x04001D35 RID: 7477
		private static int fleetCache = -2;

		// Token: 0x04001D36 RID: 7478
		public static readonly Color PlanetTagRed = new Color(0.9254902f, 0.12941177f, 0f);

		// Token: 0x04001D37 RID: 7479
		public static readonly Color PlanetTagGreen = new Color(0.7058824f, 1f, 0.24313726f);
	}
}
