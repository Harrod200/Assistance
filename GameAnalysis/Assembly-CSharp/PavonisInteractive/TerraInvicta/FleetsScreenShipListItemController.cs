using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000852 RID: 2130
	public class FleetsScreenShipListItemController : MonoBehaviour
	{
		// Token: 0x17000EB4 RID: 3764
		// (get) Token: 0x06004E29 RID: 20009 RVA: 0x0021A492 File Offset: 0x00218692
		private TIFactionState activePlayer
		{
			get
			{
				return this.controller.activePlayer;
			}
		}

		// Token: 0x06004E2A RID: 20010 RVA: 0x0021A49F File Offset: 0x0021869F
		public void Init(FleetsScreenController controller)
		{
			this.controller = controller;
			this.shipSummaryTooltip.SetDelegate("BodyText", () => this.ship.template.quickSummary(this.ship.isAlien && !GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships"), this.ship, TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(GameStateManager.Sol(), this.ship.fleet) > 149597870700.0 * (double)(1.02f + TIEffectsState.SumEffectsModifiers(Context.OuterExplorationRange_AU, this.activePlayer, 1.02f, null)), false, false));
		}

		// Token: 0x06004E2B RID: 20011 RVA: 0x0021A4C4 File Offset: 0x002186C4
		public static bool ShouldShowTransitData(TIGameState state)
		{
			if (state != null && state.isSpaceShipState)
			{
				state = state.ref_fleet;
			}
			return state != null && state.isSpaceFleetState && state.ref_fleet.transferAssigned && (state.ref_fleet.faction.permanentAlly(GameControl.control.activePlayer) || state.ref_fleet.trajectory.launched);
		}

		// Token: 0x06004E2C RID: 20012 RVA: 0x0021A530 File Offset: 0x00218730
		public void RefreshTransitData()
		{
			if (this.fleet != null && FleetsScreenShipListItemController.ShouldShowTransitData(this.fleet))
			{
				this.UpdateTransitData(this.fleet);
				return;
			}
			if (this.ship != null)
			{
				this.UpdateTransitData(this.ship);
			}
		}

		// Token: 0x06004E2D RID: 20013 RVA: 0x0021A580 File Offset: 0x00218780
		private void AddFleetListeners()
		{
			GameControl.eventManager.AddListener<ShipsAddedToFleet>(new EventManager.EventDelegate<ShipsAddedToFleet>(this.OnMajorMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<ShipsRemovedFromFleet>(new EventManager.EventDelegate<ShipsRemovedFromFleet>(this.OnMajorMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.OnMajorMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<StartFleetOperation>(new EventManager.EventDelegate<StartFleetOperation>(this.OnMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.OnMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.OnMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<CouncilorDepartsShip>(new EventManager.EventDelegate<CouncilorDepartsShip>(this.OnMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.OnMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.OnMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<ShipResupplied>(new EventManager.EventDelegate<ShipResupplied>(this.OnMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.OnMyFleetUpdate), null, this.fleet, true, false);
			GameControl.eventManager.AddListener<FleetAvailabilityChange>(new EventManager.EventDelegate<FleetAvailabilityChange>(this.OnMyFleetUpdate), null, this.fleet, true, false);
		}

		// Token: 0x06004E2E RID: 20014 RVA: 0x0021A704 File Offset: 0x00218904
		private void RemoveFleetListeners()
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
		}

		// Token: 0x06004E2F RID: 20015 RVA: 0x0021A825 File Offset: 0x00218A25
		private void OnMajorMyFleetUpdate(ShipsAddedToFleet e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E30 RID: 20016 RVA: 0x0021A834 File Offset: 0x00218A34
		private void OnMajorMyFleetUpdate(ShipsRemovedFromFleet e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E31 RID: 20017 RVA: 0x0021A843 File Offset: 0x00218A43
		private void OnMajorMyFleetUpdate(CombatEnds e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E32 RID: 20018 RVA: 0x0021A852 File Offset: 0x00218A52
		private void OnMyFleetUpdate(StartFleetOperation e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E33 RID: 20019 RVA: 0x0021A861 File Offset: 0x00218A61
		private void OnMyFleetUpdate(OperationExecuted e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E34 RID: 20020 RVA: 0x0021A870 File Offset: 0x00218A70
		private void OnMyFleetUpdate(FleetArrivesAtDestination e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E35 RID: 20021 RVA: 0x0021A87F File Offset: 0x00218A7F
		private void OnMyFleetUpdate(CouncilorDepartsShip e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E36 RID: 20022 RVA: 0x0021A88E File Offset: 0x00218A8E
		private void OnMyFleetUpdate(CouncilorVisibilityChanged e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E37 RID: 20023 RVA: 0x0021A89D File Offset: 0x00218A9D
		private void OnMyFleetUpdate(CouncilorPositionUpdated e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E38 RID: 20024 RVA: 0x0021A8AC File Offset: 0x00218AAC
		private void OnMyFleetUpdate(ShipResupplied e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E39 RID: 20025 RVA: 0x0021A8BB File Offset: 0x00218ABB
		private void OnMyFleetUpdate(FleetUndocks e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E3A RID: 20026 RVA: 0x0021A8CA File Offset: 0x00218ACA
		private void OnMyFleetUpdate(FleetAvailabilityChange e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004E3B RID: 20027 RVA: 0x0021A8D9 File Offset: 0x00218AD9
		private void OnEnable()
		{
			this.UpdateFleet();
		}

		// Token: 0x06004E3C RID: 20028 RVA: 0x0021A8E4 File Offset: 0x00218AE4
		public void UpdateTransitData(TIGameState gameState)
		{
			this.shipDeltaV.SetText(Loc.T("UI.Fleets.TwoColumn", new object[]
			{
				Loc.T("UI.Fleets.DVValue", new object[]
				{
					TIUtilities.FormatBigOrSmallNumber(this.ship.currentDeltaV_kps, 1, 3, 0, false, false),
					TIUtilities.FormatBigOrSmallNumber(this.ship.currentMaxDeltaV_kps, 1, 3, 0, false, false)
				}),
				this.ship.drive.PropellantIcons(true, this.ship.faction)
			}));
		}

		// Token: 0x06004E3D RID: 20029 RVA: 0x0021A970 File Offset: 0x00218B70
		public void UpdateListItem(TIGameState gameState)
		{
			if (this.fleet != gameState && gameState != null)
			{
				this.RemoveFleetListeners();
			}
			this.ship = null;
			this.fleet = null;
			this.faction = null;
			this.isGroupItem = false;
			if (gameState.isSpaceShipState)
			{
				this.ship = gameState.ref_ship;
				RectTransform component = base.GetComponent<RectTransform>();
				component.sizeDelta = new Vector2(component.sizeDelta.x, 20f);
				this.shipName.SetText(this.ship.NameWithDamageIcons());
				this.shipClass.SetText(this.ship.template.fullClassName);
				this.shipRole.SetText(Loc.T(new StringBuilder("UI.Fleets.").Append(this.ship.role.ToString()).ToString()));
				this.shipAcceleration.SetText(FleetsScreenController.dualAccelerationStr(this.ship));
				this.shipCombatScore.SetText(this.ship.SpaceCombatValue(false, 0f).ToString("N0"));
				List<CouncilorView> list = this.ship.CouncilorViewsPresentAndKnownToFaction(this.activePlayer);
				CombatantListItemController.SetNoseImage(this.ship, this.nose);
				CombatantListItemController.SetMidImage(this.ship, this.hull);
				CombatantListItemController.SetTailImage(this.ship, this.tail);
				if (this.ship.isAlien)
				{
					this.radiator.enabled = false;
					this.drive.enabled = false;
				}
				else
				{
					CombatantListItemController.SetRadiatorImage(this.ship, this.radiator);
					CombatantListItemController.SetDriveImage(this.ship, this.drive);
					this.radiator.enabled = true;
					this.drive.enabled = true;
				}
				this.UpdateTransitData(this.ship);
				int num = 0;
				if (list.Count == 0)
				{
					this.shipCouncilorIconGrid.gameObject.SetActive(false);
				}
				else
				{
					this.shipCouncilorIconGrid.SetListSize<CombatShipCouncilorGridItemController>(list.Count, false, false);
					using (IEnumerator<object> enumerator = this.shipCouncilorIconGrid.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (FleetsScreenShipListItemController.<>o__42.<>p__0 == null)
							{
								FleetsScreenShipListItemController.<>o__42.<>p__0 = CallSite<Func<CallSite, object, CombatShipCouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombatShipCouncilorGridItemController), typeof(FleetsScreenShipListItemController)));
							}
							FleetsScreenShipListItemController.<>o__42.<>p__0.Target(FleetsScreenShipListItemController.<>o__42.<>p__0, enumerator.Current).SetGridItem(list[num++]);
						}
					}
					this.shipCouncilorIconGrid.gameObject.SetActive(true);
				}
				if (!this.ship.isAlien)
				{
					this.shipOfficerIconGrid.SetListSize<ShipOfficerGridItemController>(this.ship.officers.Count, false, false);
					num = 0;
					List<TIOfficerState> list2 = this.ship.officers.OrderBy<TIOfficerState, int>((TIOfficerState x) => x.template.sortOrder).ToList<TIOfficerState>();
					using (IEnumerator<object> enumerator = this.shipOfficerIconGrid.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (FleetsScreenShipListItemController.<>o__42.<>p__1 == null)
							{
								FleetsScreenShipListItemController.<>o__42.<>p__1 = CallSite<Func<CallSite, object, ShipOfficerGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipOfficerGridItemController), typeof(FleetsScreenShipListItemController)));
							}
							FleetsScreenShipListItemController.<>o__42.<>p__1.Target(FleetsScreenShipListItemController.<>o__42.<>p__1, enumerator.Current).UpdateGridItem(list2[num++]);
						}
						goto IL_0381;
					}
				}
				this.shipOfficerIconGrid.SetListSize<ShipOfficerGridItemController>(0, false, false);
				IL_0381:
				this.shipLineObject.SetActive(true);
			}
		}

		// Token: 0x06004E3E RID: 20030 RVA: 0x0021AD28 File Offset: 0x00218F28
		private void UpdateFleet()
		{
			if (this.fleetDataDirty && this.fleet != null && this.controller.Canvas.enabled)
			{
				this.UpdateListItem(this.fleet);
			}
		}

		// Token: 0x06004E3F RID: 20031 RVA: 0x0021AD5E File Offset: 0x00218F5E
		private void ToggleCollapsed()
		{
			this.controller.ToggleFactionFleets(this.faction);
		}

		// Token: 0x06004E40 RID: 20032 RVA: 0x0021AD74 File Offset: 0x00218F74
		public void OnGotoFleetButtonPressed()
		{
			if (this.fleet.faction == GameControl.control.activePlayer)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyFleetSelect", false, false);
			}
			else if (this.fleet.faction == GameStateManager.AlienFaction())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienFleetSelect", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect", false, false);
			}
			TIUtilities.GotoGameState(this.fleet, true, true, true, true, false, -1f);
		}

		// Token: 0x06004E41 RID: 20033 RVA: 0x0021ADF4 File Offset: 0x00218FF4
		public void OnOpenFleetButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			if (this.isGroupItem)
			{
				this.ToggleCollapsed();
				this.controller.UpdateFleetsList();
				return;
			}
			this.controller.fleetOpenedStatus[this.fleet] = !this.controller.fleetOpenedStatus[this.fleet];
			this.controller.UpdateFleetsList();
		}

		// Token: 0x06004E42 RID: 20034 RVA: 0x0021AE61 File Offset: 0x00219061
		public void ShipButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.controller.ShowIndividualDataScreen(this.ship, true);
		}

		// Token: 0x06004E43 RID: 20035 RVA: 0x0021AE81 File Offset: 0x00219081
		private void OnDestroy()
		{
			this.RemoveFleetListeners();
		}

		// Token: 0x040031B1 RID: 12721
		private FleetsScreenController controller;

		// Token: 0x040031B2 RID: 12722
		public TISpaceFleetState fleet;

		// Token: 0x040031B3 RID: 12723
		public TISpaceShipState ship;

		// Token: 0x040031B4 RID: 12724
		private TIFactionState faction;

		// Token: 0x040031B5 RID: 12725
		public bool isGroupItem;

		// Token: 0x040031B6 RID: 12726
		[Header("Ship Data")]
		public GameObject shipLineObject;

		// Token: 0x040031B7 RID: 12727
		public Image nose;

		// Token: 0x040031B8 RID: 12728
		public Image hull;

		// Token: 0x040031B9 RID: 12729
		public Image tail;

		// Token: 0x040031BA RID: 12730
		public Image drive;

		// Token: 0x040031BB RID: 12731
		public Image radiator;

		// Token: 0x040031BC RID: 12732
		public TMP_Text shipName;

		// Token: 0x040031BD RID: 12733
		public TMP_Text shipClass;

		// Token: 0x040031BE RID: 12734
		public TMP_Text shipRole;

		// Token: 0x040031BF RID: 12735
		public TMP_Text shipCombatScore;

		// Token: 0x040031C0 RID: 12736
		public TMP_Text shipAcceleration;

		// Token: 0x040031C1 RID: 12737
		public TMP_Text shipDeltaV;

		// Token: 0x040031C2 RID: 12738
		public ListManagerBase shipCouncilorIconGrid;

		// Token: 0x040031C3 RID: 12739
		public TooltipTrigger shipSummaryTooltip;

		// Token: 0x040031C4 RID: 12740
		public ListManagerBase shipOfficerIconGrid;

		// Token: 0x040031C5 RID: 12741
		private bool fleetDataDirty;
	}
}
