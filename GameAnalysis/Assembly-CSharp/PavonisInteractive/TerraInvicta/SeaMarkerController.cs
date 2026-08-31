using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000559 RID: 1369
	public class SeaMarkerController : SingleMarkerController
	{
		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06002416 RID: 9238 RVA: 0x000BFA9E File Offset: 0x000BDC9E
		public IEnumerable<TIArmyState> Armies
		{
			get
			{
				return this.localArmies;
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06002417 RID: 9239 RVA: 0x000BFAA6 File Offset: 0x000BDCA6
		public IEnumerable<TIGameState> ArmyHandlers
		{
			get
			{
				return this.transportMarkers.Keys;
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06002418 RID: 9240 RVA: 0x000BFAB4 File Offset: 0x000BDCB4
		public IEnumerable<TIFactionState> HandlerFactions
		{
			get
			{
				return from x in this.ArmyHandlers
					select x as TIFactionState into x
					where x != null
					select x;
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06002419 RID: 9241 RVA: 0x000BFB10 File Offset: 0x000BDD10
		public IEnumerable<TINationState> HandlerNations
		{
			get
			{
				return from x in this.ArmyHandlers
					select x as TINationState into x
					where x != null
					select x;
			}
		}

		// Token: 0x0600241A RID: 9242 RVA: 0x000BFB6B File Offset: 0x000BDD6B
		public MarkerController GetMarkerController(TIFactionState faction)
		{
			if (!this.transportMarkers.ContainsKey(faction))
			{
				return null;
			}
			return this.transportMarkers[faction];
		}

		// Token: 0x0600241B RID: 9243 RVA: 0x000BFB89 File Offset: 0x000BDD89
		public MarkerController GetMarkerController(TINationState nation)
		{
			if (!this.transportMarkers.ContainsKey(nation))
			{
				return null;
			}
			return this.transportMarkers[nation];
		}

		// Token: 0x0600241C RID: 9244 RVA: 0x000BFBA8 File Offset: 0x000BDDA8
		public TIArmyState GetTopArmy(TIFactionState faction)
		{
			MarkerController markerController = this.GetMarkerController(faction);
			if (markerController != null)
			{
				return markerController.associatedState.ref_army;
			}
			return null;
		}

		// Token: 0x0600241D RID: 9245 RVA: 0x000BFBD4 File Offset: 0x000BDDD4
		public TIArmyState GetTopArmy(TINationState nation)
		{
			MarkerController markerController = this.GetMarkerController(nation);
			if (markerController != null)
			{
				return markerController.associatedState.ref_army;
			}
			return null;
		}

		// Token: 0x0600241E RID: 9246 RVA: 0x000BFBFF File Offset: 0x000BDDFF
		public void MoveToFront(TIArmyState army)
		{
			if (!this.localArmies.Contains(army))
			{
				return;
			}
			this.localArmies.Remove(army);
			this.localArmies.Add(army);
			this.UpdateMarker();
		}

		// Token: 0x0600241F RID: 9247 RVA: 0x000BFC30 File Offset: 0x000BDE30
		public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
		{
			base.InitializeWithRegion(regionController, container);
			GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.UpdateMarker), null, null, true, false);
			if (base.region.oceanType == WorldOceanType.Seasonal)
			{
				GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnMonthlyUpdate), "MonthlySeasonalUpdate", null, true, false);
			}
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnArmyEmbarks), base.region.ArmyEmbarkEventName, null, true, false);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnArmyTransitsIn), base.region.ArmySeaTransitEventName, null, true, false);
			GameControl.eventManager.AddListener<ArmySeaTransits>(new EventManager.EventDelegate<ArmySeaTransits>(this.OnArmyTransitsOut), null, base.region, true, false);
			GameControl.eventManager.AddListener<ArmySeaTransitCancelled>(new EventManager.EventDelegate<ArmySeaTransitCancelled>(this.OnSeaTransitCancelled), null, base.region, true, false);
			GameControl.eventManager.AddListener<ArmyArrivesInRegion>(new EventManager.EventDelegate<ArmyArrivesInRegion>(this.OnArmyArrivesOnLand), null, base.region, false, false);
			GameControl.eventManager.AddListener<ArmyMajorStatusUpdate>(new EventManager.EventDelegate<ArmyMajorStatusUpdate>(this.OnArmyUpdated), null, base.region, false, false);
			this.localArmies = new List<TIArmyState>();
			this.transportMarkers = new Dictionary<TIGameState, MarkerController>();
			foreach (TIArmyState tiarmyState in GameStateManager.IterateByClass<TIArmyState>(true))
			{
				ArmySeaTransitStage armySeaTransitStage = tiarmyState.SeaTransitStage();
				if ((armySeaTransitStage == ArmySeaTransitStage.Sea_HomeRegion && tiarmyState.currentRegion == base.region) || (armySeaTransitStage == ArmySeaTransitStage.Sea_DestinationRegion && tiarmyState.CurrentOperations()[0].target.ref_region == base.region))
				{
					this.AddArmyToSeaZone(tiarmyState);
				}
			}
			this.UpdateMarker();
		}

		// Token: 0x06002420 RID: 9248 RVA: 0x000BFDF0 File Offset: 0x000BDFF0
		public void Update()
		{
			if (this.markerDataDirty)
			{
				this.UpdateMarker();
				this.markerDataDirty = false;
			}
		}

		// Token: 0x06002421 RID: 9249 RVA: 0x000BFE07 File Offset: 0x000BE007
		public void AttemptUpdateMarker()
		{
			if (base.gameObject.activeSelf)
			{
				this.markerDataDirty = true;
				return;
			}
			this.UpdateMarker();
		}

		// Token: 0x06002422 RID: 9250 RVA: 0x000BFE24 File Offset: 0x000BE024
		private void AddArmyToSeaZone(TIArmyState army)
		{
			if (this.localArmies.Contains(army))
			{
				Log.Debug("SeaMarkerController.AddArmyToSeaZone: Tried to add duplicate army", Array.Empty<object>());
				return;
			}
			this.localArmies.Add(army);
		}

		// Token: 0x06002423 RID: 9251 RVA: 0x000BFE50 File Offset: 0x000BE050
		private void RemoveArmyFromSeaZone(TIArmyState army)
		{
			if (this.localArmies.Contains(army))
			{
				this.localArmies.Remove(army);
				this.UpdateMarker();
			}
		}

		// Token: 0x06002424 RID: 9252 RVA: 0x000BFE73 File Offset: 0x000BE073
		private void OnSeaTransitCancelled(ArmySeaTransitCancelled e)
		{
			this.RemoveArmyFromSeaZone(e.army);
		}

		// Token: 0x06002425 RID: 9253 RVA: 0x000BFE81 File Offset: 0x000BE081
		private void OnArmyUpdated(ArmyMajorStatusUpdate e)
		{
			if (e.army.destroyed)
			{
				this.RemoveArmyFromSeaZone(e.army);
			}
		}

		// Token: 0x06002426 RID: 9254 RVA: 0x000BFE9C File Offset: 0x000BE09C
		private void OnArmyEmbarks(TimeEventStart e)
		{
			TIGameState eventObject = e.eventObject;
			TIArmyState tiarmyState = ((eventObject != null) ? eventObject.ref_army : null);
			if (tiarmyState != null && tiarmyState.SeaTransitStage() != ArmySeaTransitStage.None && e.eventObject2 == tiarmyState.CurrentOperations()[0].target)
			{
				this.AddArmyToSeaZone(tiarmyState);
				this.UpdateMarker();
				GameControl.eventManager.TriggerEvent(new ArmyStatusUpdate(tiarmyState, null), tiarmyState.armyStatusUpdateEventName, new object[] { tiarmyState });
			}
		}

		// Token: 0x06002427 RID: 9255 RVA: 0x000BFF1C File Offset: 0x000BE11C
		private void OnArmyTransitsIn(TimeEventStart e)
		{
			TIGameState eventObject = e.eventObject;
			TIArmyState tiarmyState = ((eventObject != null) ? eventObject.ref_army : null);
			if (tiarmyState != null && tiarmyState.SeaTransitStage() != ArmySeaTransitStage.None && tiarmyState.CurrentOperations()[0].target == base.region)
			{
				GameControl.eventManager.TriggerEvent(new ArmySeaTransits(tiarmyState, base.region), null, new object[] { tiarmyState.currentRegion });
				this.AddArmyToSeaZone(tiarmyState);
				this.UpdateMarker();
			}
		}

		// Token: 0x06002428 RID: 9256 RVA: 0x000BFF9E File Offset: 0x000BE19E
		private void OnArmyTransitsOut(ArmySeaTransits e)
		{
			this.RemoveArmyFromSeaZone(e.army);
		}

		// Token: 0x06002429 RID: 9257 RVA: 0x000BFFAC File Offset: 0x000BE1AC
		private void OnArmyArrivesOnLand(ArmyArrivesInRegion e)
		{
			if (this.localArmies.Contains(e.army))
			{
				this.RemoveArmyFromSeaZone(e.army);
			}
		}

		// Token: 0x0600242A RID: 9258 RVA: 0x000BFFCD File Offset: 0x000BE1CD
		private void UpdateMarker(MapActivationChangedEvent e)
		{
			if (e.active)
			{
				this.AttemptUpdateMarker();
			}
		}

		// Token: 0x0600242B RID: 9259 RVA: 0x000BFFE0 File Offset: 0x000BE1E0
		public override void UpdateMarker()
		{
			this.transportMarkers.Values.ToList<MarkerController>().ForEach(delegate(MarkerController x)
			{
				base.container.ManageMarkerStack(x, true, MarkerType.NavalTransport, base.region, "", -1, false);
			});
			this.transportMarkers.Clear();
			using (List<TIArmyState>.Enumerator enumerator = this.localArmies.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIArmyState army = enumerator.Current;
					if (army.CurrentOperations().Count != 0)
					{
						TIFactionState faction = army.faction;
						TIGameState tigameState = ((faction != null) ? faction.ref_gameState : null) ?? army.homeNation.ref_gameState;
						if (!this.transportMarkers.ContainsKey(tigameState))
						{
							this.transportMarkers[tigameState] = base.container.ManageMarkerStack(null, false, MarkerType.NavalTransport, base.region, "NavyTransport", -1, false);
							this.transportMarkers[tigameState].SetCentralIcon(army.GetTransportIcon());
							this.transportMarkers[tigameState].SetPrimaryIconBackground(army.GetIconBackgroundSprite, army.GetIconBackgroundResourceColor, ClearFlag.TurnOn);
							this.transportMarkers[tigameState].SetNationImage(army.homeNation.flag, ClearFlag.NoChange);
							this.transportMarkers[tigameState].SetPercentage(army.strength, (army.strength < 1f) ? ClearFlag.TurnOn : ClearFlag.TurnOff);
							this.transportMarkers[tigameState].SetPercentColor((army.strength < 0.5f) ? Color.red : Color.green);
							this.transportMarkers[tigameState].SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnSeaTransportClick));
							this.transportMarkers[tigameState].associatedState = army;
							this.transportMarkers[tigameState].SetHoverSpriteByFaction(army.faction);
							this.transportMarkers[tigameState].SetTopRightIcon(null, ClearFlag.TurnOff);
							if (tigameState.isFactionState)
							{
								this.transportMarkers[tigameState].SetArmyFactionImage(tigameState.ref_faction.factionIcon64, ClearFlag.TurnOn);
							}
							else
							{
								this.transportMarkers[tigameState].SetArmyFactionImage(null, ClearFlag.TurnOff);
							}
							int num = (tigameState.isNationState ? this.localArmies.Count<TIArmyState>((TIArmyState x) => x.homeNation == army.homeNation) : this.localArmies.Count<TIArmyState>((TIArmyState x) => x.faction == army.faction));
							if (num > 1)
							{
								this.transportMarkers[tigameState].SetNumber(num.ToString(), ClearFlag.TurnOn, false);
								this.transportMarkers[tigameState].SetCentralIconShadow(true);
							}
							else
							{
								this.transportMarkers[tigameState].SetNumber(string.Empty, ClearFlag.TurnOff, false);
								this.transportMarkers[tigameState].SetCentralIconShadow(false);
							}
							this.transportMarkers[tigameState].AssignAnimationToCentralIconSprite(army, false, true);
							this.transportMarkers[tigameState].StartAnimations("SeaMove");
							this.transportMarkers[tigameState].SetTooltip(() => this.SeaMarkertooltip(army.ref_faction));
							this.transportMarkers[tigameState].armyMovementArrowImage.enabled = true;
							GameObject armyMovementArrow = this.transportMarkers[tigameState].armyMovementArrow;
							Vector3 heading = ArmyMarkerController.GetHeading(army);
							armyMovementArrow.transform.localRotation = Quaternion.identity;
							float num2 = Vector3.SignedAngle(armyMovementArrow.transform.rotation * Vector3.up, heading, armyMovementArrow.transform.rotation * Vector3.forward);
							armyMovementArrow.transform.localRotation = Quaternion.AngleAxis(num2, Vector3.forward);
							TIRegionState tiregionState = army.CurrentOperations()[0].target as TIRegionState;
							this.transportMarkers[tigameState].armyMovementArrowImage.color = (army.FriendlyRegion(tiregionState) ? new Color32(91, 109, 133, byte.MaxValue) : new Color32(236, 33, 0, byte.MaxValue));
						}
					}
				}
			}
			this.UpdateIceMarker();
			base.container.Refresh();
		}

		// Token: 0x0600242C RID: 9260 RVA: 0x000C0450 File Offset: 0x000BE650
		private void OnMonthlyUpdate(TimeEventStart e)
		{
			this.UpdateIceMarker();
		}

		// Token: 0x0600242D RID: 9261 RVA: 0x000C0458 File Offset: 0x000BE658
		private void UpdateIceMarker()
		{
			if (base.region.oceanType == WorldOceanType.Seasonal)
			{
				if (base.region.coastCurrentlyFrozen)
				{
					this.frozenMarker = base.container.ManageMarkerStack(this.frozenMarker, false, MarkerType.Frozen, base.region, "Frozen", -1, false);
					this.frozenMarker.SetCentralIcon("mapicons/ICO_geoscape_ice");
					this.frozenMarker.SetTooltip(() => Loc.T("UI.Markers.FrozenTip"));
					return;
				}
				if (this.frozenMarker != null)
				{
					this.frozenMarker = base.container.ManageMarkerStack(this.frozenMarker, true, MarkerType.Frozen, base.region, "Frozen", -1, false);
					return;
				}
			}
			else if (this.frozenMarker != null)
			{
				this.frozenMarker = base.container.ManageMarkerStack(this.frozenMarker, true, MarkerType.Frozen, base.region, "Frozen", -1, false);
				GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnMonthlyUpdate), "MonthlyUpdate");
			}
		}

		// Token: 0x0600242E RID: 9262 RVA: 0x000C056C File Offset: 0x000BE76C
		private string SeaMarkertooltip(TIFactionState faction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TIArmyState tiarmyState in this.localArmies)
			{
				if (tiarmyState.faction == faction)
				{
					if (tiarmyState.faction == null)
					{
						stringBuilder.AppendLine(Loc.T("UI.Markers.NoFactionArmy", new object[]
						{
							tiarmyState.displayName,
							tiarmyState.homeNation.displayName,
							tiarmyState.strength.ToPercent("P0"),
							tiarmyState.GetAttackValue().ToString("N3")
						}));
						stringBuilder.Append("    ").AppendLine(tiarmyState.OperationDescription());
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.Markers.FactionArmy", new object[]
						{
							tiarmyState.displayName,
							tiarmyState.homeNation.displayName,
							tiarmyState.faction.displayNameCapitalizedWithColor,
							tiarmyState.strength.ToPercent("P0"),
							tiarmyState.GetAttackValue().ToString("N3")
						}));
						stringBuilder.Append("    ").AppendLine(tiarmyState.OperationDescription());
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600242F RID: 9263 RVA: 0x000C06E4 File Offset: 0x000BE8E4
		private void OnSeaTransportClick(MarkerController controller)
		{
			TIArmyState ref_army = controller.associatedState.ref_army;
			this.localArmies = new List<TIArmyState>(this.localArmies).Except<TIArmyState>(new List<TIArmyState> { ref_army }).ToList<TIArmyState>();
			this.localArmies.Add(ref_army);
			this.UpdateMarker();
			TIArmyState tiarmyState = this.localArmies[0];
			SoundEffectController.PlaySelectSound(tiarmyState);
			GameControl.eventManager.TriggerEvent(new ArmyMapItemSelected(tiarmyState), null, Array.Empty<object>());
			if (controller.associatedState != null)
			{
				TIUtilities.GotoGameState(controller.associatedState, false, true, true, true, false, -1f);
			}
		}

		// Token: 0x04001B4B RID: 6987
		private List<TIArmyState> localArmies;

		// Token: 0x04001B4C RID: 6988
		private Dictionary<TIGameState, MarkerController> transportMarkers;

		// Token: 0x04001B4D RID: 6989
		private bool markerDataDirty;

		// Token: 0x04001B4E RID: 6990
		private MarkerController frozenMarker;
	}
}
