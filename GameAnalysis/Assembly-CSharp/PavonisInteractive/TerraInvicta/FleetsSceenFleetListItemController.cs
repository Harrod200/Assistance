using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200084D RID: 2125
	public class FleetsSceenFleetListItemController : MonoBehaviour
	{
		// Token: 0x17000EAD RID: 3757
		// (get) Token: 0x06004D19 RID: 19737 RVA: 0x0020C458 File Offset: 0x0020A658
		private TIFactionState activePlayer
		{
			get
			{
				return this.controller.activePlayer;
			}
		}

		// Token: 0x06004D1A RID: 19738 RVA: 0x0020C468 File Offset: 0x0020A668
		public void Init(FleetsScreenController controller)
		{
			this.controller = controller;
			this.setAlarmTooltip.SetText("BodyText", Loc.T("UI.Alarm.FleetApproachingQuery"));
			this.RevertRename();
			this.shipSummaryTooltip.SetDelegate("BodyText", () => this.ship.template.quickSummary(this.ship.isAlien && !GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships"), this.ship, TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(GameStateManager.Sol(), this.ship.fleet) > 149597870700.0 * (double)(1.02f + TIEffectsState.SumEffectsModifiers(Context.OuterExplorationRange_AU, this.activePlayer, 1.02f, null)), false, false));
		}

		// Token: 0x06004D1B RID: 19739 RVA: 0x0020C4B8 File Offset: 0x0020A6B8
		public static bool ShouldShowTransitData(TIGameState state)
		{
			if (state != null && state.isSpaceShipState)
			{
				state = state.ref_fleet;
			}
			return state != null && state.isSpaceFleetState && state.ref_fleet.transferAssigned && (state.ref_fleet.faction.permanentAlly(GameControl.control.activePlayer) || state.ref_fleet.trajectory.launched);
		}

		// Token: 0x06004D1C RID: 19740 RVA: 0x0020C524 File Offset: 0x0020A724
		public void RefreshTransitData()
		{
			if (this.fleet != null && FleetsSceenFleetListItemController.ShouldShowTransitData(this.fleet))
			{
				this.UpdateTransitData(this.fleet);
				return;
			}
			if (this.ship != null)
			{
				this.UpdateTransitData(this.ship);
			}
		}

		// Token: 0x06004D1D RID: 19741 RVA: 0x0020C574 File Offset: 0x0020A774
		private void AddFleetListeners()
		{
			this.RemoveFleetListeners();
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

		// Token: 0x06004D1E RID: 19742 RVA: 0x0020C6FC File Offset: 0x0020A8FC
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

		// Token: 0x06004D1F RID: 19743 RVA: 0x0020C81D File Offset: 0x0020AA1D
		private void OnMajorMyFleetUpdate(ShipsAddedToFleet e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D20 RID: 19744 RVA: 0x0020C82C File Offset: 0x0020AA2C
		private void OnMajorMyFleetUpdate(ShipsRemovedFromFleet e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D21 RID: 19745 RVA: 0x0020C83B File Offset: 0x0020AA3B
		private void OnMajorMyFleetUpdate(CombatEnds e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D22 RID: 19746 RVA: 0x0020C84A File Offset: 0x0020AA4A
		private void OnMyFleetUpdate(StartFleetOperation e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D23 RID: 19747 RVA: 0x0020C859 File Offset: 0x0020AA59
		private void OnMyFleetUpdate(OperationExecuted e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D24 RID: 19748 RVA: 0x0020C868 File Offset: 0x0020AA68
		private void OnMyFleetUpdate(FleetArrivesAtDestination e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D25 RID: 19749 RVA: 0x0020C877 File Offset: 0x0020AA77
		private void OnMyFleetUpdate(CouncilorDepartsShip e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D26 RID: 19750 RVA: 0x0020C886 File Offset: 0x0020AA86
		private void OnMyFleetUpdate(CouncilorVisibilityChanged e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D27 RID: 19751 RVA: 0x0020C895 File Offset: 0x0020AA95
		private void OnMyFleetUpdate(CouncilorPositionUpdated e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D28 RID: 19752 RVA: 0x0020C8A4 File Offset: 0x0020AAA4
		private void OnMyFleetUpdate(ShipResupplied e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D29 RID: 19753 RVA: 0x0020C8B3 File Offset: 0x0020AAB3
		private void OnMyFleetUpdate(FleetUndocks e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D2A RID: 19754 RVA: 0x0020C8C2 File Offset: 0x0020AAC2
		private void OnMyFleetUpdate(FleetAvailabilityChange e)
		{
			this.fleetDataDirty = true;
			this.UpdateFleet();
		}

		// Token: 0x06004D2B RID: 19755 RVA: 0x0020C8D1 File Offset: 0x0020AAD1
		private void OnEnable()
		{
		}

		// Token: 0x06004D2C RID: 19756 RVA: 0x0020C8D4 File Offset: 0x0020AAD4
		private void UpdateFleet()
		{
			if (this.fleetDataDirty && this.fleet != null && this.controller.Canvas.enabled && this.fleetLineObject != null && this.fleetLineObject.activeInHierarchy)
			{
				this.UpdateListItem(this.fleet);
			}
		}

		// Token: 0x06004D2D RID: 19757 RVA: 0x0020C930 File Offset: 0x0020AB30
		public void UpdateTransitData(TIGameState gameState)
		{
			if (gameState.isSpaceFleetState)
			{
				this.transferTextDetail.SetText(SpaceObjectDetailController.FleetTransferTwoLiner(gameState.ref_fleet, gameState.ref_faction != GameControl.control.activePlayer));
				if (this.transferProgressIcon != null && this.transferProgressIcon.rectTransform != null)
				{
					this.transferProgressIcon.rectTransform.localPosition = new Vector2(this.fleetTransferSliderZeroPoint + (float)(this.fleet.TrajectoryFractionCompleted() * (double)this.fleetTransferSliderRange), this.transferProgressIcon.rectTransform.localPosition.y);
				}
				if (this.transferPendingCombatIcon != null)
				{
					SpaceObjectDetailController.UpdatePendingCombatIcon(this.transferPendingCombatIcon, this.fleet);
				}
				this.deltaVText.SetText(Loc.T("UI.Fleets.DVValue", new object[]
				{
					TIUtilities.FormatBigOrSmallNumber(this.fleet.currentDeltaV_kps, 1, 7, 0, false, false),
					TIUtilities.FormatBigOrSmallNumber(this.fleet.maxDeltaV_kps, 1, 7, 0, false, false)
				}));
				return;
			}
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

		// Token: 0x06004D2E RID: 19758 RVA: 0x0020CAC8 File Offset: 0x0020ACC8
		public void UpdateListItem(TIGameState gameState)
		{
			if (this.fleet != gameState && gameState != null)
			{
				this.RemoveFleetListeners();
			}
			this.ship = null;
			this.fleet = null;
			this.faction = null;
			this.gotoButton.SetActive(true);
			this.editButton.SetActive(true);
			this.dvIcon.enabled = true;
			this.accelIcon.enabled = true;
			this.isGroupItem = false;
			this.BGImage.color = Color.white;
			this.operationDetailBG.enabled = true;
			bool flag = this.activePlayer.victoryTemplate.GetConditionBlockingSpaceAssets(this.activePlayer).Contains(gameState);
			if (gameState.isSpaceFleetState && gameState.ref_fleet.ships.Count > 0)
			{
				this.fleet = gameState.ref_fleet;
				this.fleetTransferSliderZeroPoint = 0f;
				this.fleetTransferSliderRange = this.fleetTransferProgressLine.sizeDelta.x - this.fleetTransferSliderZeroPoint;
				this.fleetLineObject.SetActive(true);
				this.shipLineObject.SetActive(false);
				string text;
				if (!this.fleet.faction.permanentAlly(this.activePlayer))
				{
					TIOrbitState tiorbitState;
					if (this.fleet.inTransfer)
					{
						tiorbitState = this.fleet.trajectory.destinationOrbit;
					}
					else
					{
						tiorbitState = this.fleet.orbitState;
					}
					if (tiorbitState != null)
					{
						switch (tiorbitState.OrbitInterestLevel(this.activePlayer))
						{
						case 1:
							text = TIUtilities.YellowLine(this.fleet.GetDisplayName(this.activePlayer));
							break;
						case 2:
							text = TIUtilities.HighlightLine(this.fleet.GetDisplayName(this.activePlayer));
							break;
						case 3:
							text = TIUtilities.RedLine(this.fleet.GetDisplayName(this.activePlayer));
							break;
						default:
							text = this.fleet.GetDisplayName(this.activePlayer);
							break;
						}
					}
					else
					{
						text = this.fleet.GetDisplayName(this.activePlayer);
					}
				}
				else
				{
					text = this.fleet.GetDisplayName(this.activePlayer);
				}
				if (flag)
				{
					text = new StringBuilder(text).Append(TIGlobalConfig.globalConfig.victoryItemInlineSpritePath).ToString();
				}
				this.fleetName.SetText(text);
				this.fleetShipsCount.SetText(this.fleet.ships.Count.ToString());
				this.fleetShipsCombatScore.SetText(TIUtilities.FormatBigNumber((double)this.fleet.SpaceCombatValue(), 1, false));
				this.fleetIcon.sprite = this.fleet.icon;
				this.fleetPendingCombat.enabled = false;
				this.damagedShipsInFleet.enabled = this.fleet.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.damaged);
				if (FleetsSceenFleetListItemController.ShouldShowTransitData(this.fleet))
				{
					if (this.fleet.trajectory.originOrbit != null)
					{
						this.transferOriginIcon.sprite = this.fleet.trajectory.originOrbit.barycenter.icon;
					}
					else
					{
						this.transferDestinationIcon.sprite = GameControl.assetLoader.LoadAsset<Sprite>("icons_2d/ICO_none");
					}
					this.o1 = this.fleet.trajectory.originOrbit;
					TISpaceFleetState destinationFleet = this.fleet.trajectory.destinationFleet;
					if (destinationFleet != null && destinationFleet.inTransfer)
					{
						this.transferDestinationIcon.sprite = this.fleet.trajectory.destinationFleet.icon;
						this.d1 = this.fleet.trajectory.destinationFleet;
						this.transferDestinationDetailIcon.enabled = false;
					}
					else if (this.fleet.trajectory.endsInCrash)
					{
						this.transferDestinationIcon.sprite = this.fleet.trajectory.collisionTarget.icon;
						this.d1 = this.fleet.trajectory.collisionTarget;
					}
					else if (this.fleet.trajectory.exitsSolarSystem)
					{
						this.transferDestinationIcon.sprite = GameControl.assetLoader.LoadAsset<Sprite>("icons_2d/ICO_none");
						this.d1 = GameStateManager.Sol();
					}
					else if (this.fleet.trajectory.destinationOrbit != null)
					{
						this.transferDestinationIcon.sprite = this.fleet.trajectory.destinationOrbit.barycenter.icon;
						this.d1 = this.fleet.trajectory.destinationOrbit;
						if (this.fleet.trajectory.destinationFleet != null)
						{
							this.transferDestinationDetailIcon.sprite = this.fleet.trajectory.destinationFleet.icon;
							this.d2 = this.fleet.trajectory.destinationFleet;
							this.transferDestinationDetailIcon.enabled = true;
						}
						else if (TIGameState.Valid(this.fleet.trajectory.destinationStation))
						{
							this.transferDestinationDetailIcon.sprite = this.fleet.trajectory.destinationStation.icon;
							this.d2 = this.fleet.trajectory.destinationStation;
							this.transferDestinationDetailIcon.enabled = true;
						}
						else
						{
							this.transferDestinationDetailIcon.enabled = false;
						}
					}
					else if (TIGameState.Valid(this.fleet.trajectory.destinationFleet))
					{
						this.transferDestinationIcon.sprite = this.fleet.trajectory.destinationFleet.icon;
						this.d1 = this.fleet.trajectory.destinationFleet;
						this.transferDestinationDetailIcon.enabled = false;
					}
					else if (TIGameState.Valid(this.fleet.trajectory.destinationStation))
					{
						this.transferDestinationIcon.sprite = this.fleet.trajectory.destinationStation.icon;
						this.d1 = this.fleet.trajectory.destinationStation;
						this.transferDestinationDetailIcon.enabled = false;
					}
					else
					{
						this.transferDestinationDetailIcon.enabled = false;
					}
					this.UpdateTransitData(this.fleet);
					this.transferDataPanel.SetActive(true);
					this.genericOpDetailPanel.SetActive(false);
					this.setAlarmButtonObject.SetActive(this.fleet.faction != this.activePlayer && this.fleet.trajectory.launched);
					if (this.setAlarmButtonObject.activeSelf)
					{
						this.setAlarmText.SetText(SpaceObjectDetailController.GetFleetAlarmText(this.activePlayer, this.fleet));
					}
				}
				else
				{
					this.setAlarmButtonObject.SetActive(false);
					this.genericOpLine1.SetText(this.fleet.GetLocationDescription(this.activePlayer, true, true));
					if (this.fleet.CurrentOperations().Count > 0 && (this.fleet.faction.isActivePlayer || !(this.fleet.CurrentOperations()[0].operation is TransferOperation)))
					{
						TIOrbitState ref_orbit = this.fleet.ref_orbit;
						this.o1 = ((ref_orbit != null) ? ref_orbit.barycenter : null) ?? this.fleet;
						OperationData operationData = this.fleet.CurrentOperations()[0];
						GameControl.assetLoader.LoadAssetForImageAssignment(operationData.operation.GetOperationIconImagePath_Off(), this.genericOpImage);
						StringBuilder stringBuilder = new StringBuilder(operationData.operation.GetDisplayName());
						if (operationData.target != null)
						{
							stringBuilder.Append("/").Append(operationData.target.GetDisplayName(this.activePlayer));
						}
						if (operationData.completionDate != null)
						{
							stringBuilder.Append("/").Append(operationData.completionDate.ToCustomDateString());
						}
						this.genericOpLine2.SetText(stringBuilder.ToString());
						this.genericOpImageSmall.enabled = false;
						if (this.fleet.bombarding)
						{
							this.o2 = this.fleet.bombardmentTarget.ref_spaceBody;
							if (this.o2 != null)
							{
								this.genericOpImageSmall.sprite = this.fleet.bombardmentTarget.ref_spaceBody.icon;
								this.genericOpImageSmall.enabled = true;
							}
						}
					}
					else
					{
						this.o1 = this.fleet.ref_naturalSpaceObject;
						this.genericOpImage.sprite = this.fleet.ref_naturalSpaceObject.icon;
						if (this.fleet.dockedAtHab)
						{
							this.o2 = this.fleet.ref_hab;
							this.genericOpImageSmall.sprite = this.fleet.ref_hab.icon;
							this.genericOpImageSmall.enabled = true;
						}
						else
						{
							this.genericOpImageSmall.enabled = false;
						}
						if (this.fleet.unavailableForOperations)
						{
							this.genericOpLine2.SetText(Loc.T("UI.Space.Fleet.Unavailable", new object[] { this.fleet.returnToOperationsTime.ToShortTimeString() }));
						}
						else
						{
							this.genericOpLine2.SetText(string.Empty);
						}
					}
					this.deltaVText.SetText(Loc.T("UI.Fleets.DVValue", new object[]
					{
						TIUtilities.FormatBigOrSmallNumber(this.fleet.currentDeltaV_kps, 1, 7, 0, false, false),
						TIUtilities.FormatBigOrSmallNumber(this.fleet.maxDeltaV_kps, 1, 7, 0, false, false)
					}));
					this.transferDataPanel.SetActive(false);
					this.genericOpDetailPanel.SetActive(true);
				}
				this.accelerationText.SetText(FleetsScreenController.accelerationStr((double)this.fleet.cruiseAcceleration_gs, false, false, true));
				List<CouncilorView> list = this.fleet.CouncilorViewsPresentAndKnownToFaction(this.activePlayer);
				if (!this.fleet.IsAlien())
				{
					List<IOperation> list2 = (from x in this.fleet.VisibleOperationList(null)
						where x is TISpaceFleetOperationTemplate_Special
						select x).ToList<IOperation>();
					this.fleetOperationsGrid.SetListSize<FleetsScreenFleetOperationGridItemController>(list2.Count, false, false);
					int num = 0;
					if (list2.Count > 7 || list.Count > 0)
					{
						this.fleetOperationsGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(19f, 19f);
					}
					else
					{
						this.fleetOperationsGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(38f, 38f);
					}
					using (IEnumerator<object> enumerator = this.fleetOperationsGrid.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (FleetsSceenFleetListItemController.<>o__88.<>p__0 == null)
							{
								FleetsSceenFleetListItemController.<>o__88.<>p__0 = CallSite<Func<CallSite, object, FleetsScreenFleetOperationGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FleetsScreenFleetOperationGridItemController), typeof(FleetsSceenFleetListItemController)));
							}
							FleetsSceenFleetListItemController.<>o__88.<>p__0.Target(FleetsSceenFleetListItemController.<>o__88.<>p__0, enumerator.Current).SetGridItem(list2[num++]);
						}
						goto IL_0B16;
					}
				}
				this.fleetOperationsGrid.SetListSize<FleetsScreenFleetOperationGridItemController>(0, true, false);
				IL_0B16:
				if (this.fleet.faction == this.activePlayer && this.fleet.homeport != null)
				{
					this.fleetHomeportText.SetText(TIUtilities.GetLocationString(this.fleet.homeport, true, false));
					this.fleetHomeportObject.SetActive(true);
				}
				else
				{
					this.fleetHomeportObject.SetActive(false);
				}
				this.fleetCouncilorIconGrid.SetListSize<CombatShipCouncilorGridItemController>(list.Count, false, false);
				int num2 = 0;
				if (list.Count <= 7)
				{
					if (!this.fleet.IsAlien())
					{
						if ((from x in this.fleet.VisibleOperationList(null)
							where x is TISpaceFleetOperationTemplate_Special
							select x).ToList<IOperation>().Count > 0)
						{
							goto IL_0BE0;
						}
					}
					this.fleetCouncilorIconGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(38f, 38f);
					goto IL_0C20;
				}
				IL_0BE0:
				this.fleetCouncilorIconGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(19f, 19f);
				IL_0C20:
				using (IEnumerator<object> enumerator = this.fleetCouncilorIconGrid.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (FleetsSceenFleetListItemController.<>o__88.<>p__1 == null)
						{
							FleetsSceenFleetListItemController.<>o__88.<>p__1 = CallSite<Func<CallSite, object, CombatShipCouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombatShipCouncilorGridItemController), typeof(FleetsSceenFleetListItemController)));
						}
						FleetsSceenFleetListItemController.<>o__88.<>p__1.Target(FleetsSceenFleetListItemController.<>o__88.<>p__1, enumerator.Current).SetGridItem(list[num2++]);
					}
				}
				this.fleetDataDirty = false;
				this.AddFleetListeners();
				return;
			}
			if (gameState.isSpaceShipState)
			{
				this.ship = gameState.ref_ship;
				this.fleetLineObject.SetActive(false);
				this.shipName.SetText(this.ship.NameWithDamageIcons());
				this.shipClass.SetText(this.ship.template.fullClassName);
				this.shipRole.SetText(Loc.T(new StringBuilder("UI.Fleets.").Append(this.ship.role.ToString()).ToString()));
				this.shipAcceleration.SetText(FleetsScreenController.dualAccelerationStr(this.ship));
				this.shipCombatScore.SetText(this.ship.SpaceCombatValue(false, 0f).ToString("N0"));
				List<CouncilorView> list3 = this.ship.CouncilorViewsPresentAndKnownToFaction(this.activePlayer);
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
				int num3 = 0;
				if (list3.Count == 0)
				{
					this.shipCouncilorIconGrid.gameObject.SetActive(false);
				}
				else
				{
					this.shipCouncilorIconGrid.SetListSize<CombatShipCouncilorGridItemController>(list3.Count, false, false);
					using (IEnumerator<object> enumerator = this.shipCouncilorIconGrid.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (FleetsSceenFleetListItemController.<>o__88.<>p__2 == null)
							{
								FleetsSceenFleetListItemController.<>o__88.<>p__2 = CallSite<Func<CallSite, object, CombatShipCouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombatShipCouncilorGridItemController), typeof(FleetsSceenFleetListItemController)));
							}
							FleetsSceenFleetListItemController.<>o__88.<>p__2.Target(FleetsSceenFleetListItemController.<>o__88.<>p__2, enumerator.Current).SetGridItem(list3[num3++]);
						}
					}
					this.shipCouncilorIconGrid.gameObject.SetActive(true);
				}
				if (!this.ship.isAlien)
				{
					this.shipOfficerIconGrid.SetListSize<ShipOfficerGridItemController>(this.ship.officers.Count, false, false);
					num3 = 0;
					List<TIOfficerState> list4 = this.ship.officers.OrderBy<TIOfficerState, int>((TIOfficerState x) => x.template.sortOrder).ToList<TIOfficerState>();
					using (IEnumerator<object> enumerator = this.shipOfficerIconGrid.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (FleetsSceenFleetListItemController.<>o__88.<>p__3 == null)
							{
								FleetsSceenFleetListItemController.<>o__88.<>p__3 = CallSite<Func<CallSite, object, ShipOfficerGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipOfficerGridItemController), typeof(FleetsSceenFleetListItemController)));
							}
							FleetsSceenFleetListItemController.<>o__88.<>p__3.Target(FleetsSceenFleetListItemController.<>o__88.<>p__3, enumerator.Current).UpdateGridItem(list4[num3++]);
						}
						goto IL_0FEB;
					}
				}
				this.shipOfficerIconGrid.SetListSize<ShipOfficerGridItemController>(0, false, false);
				IL_0FEB:
				this.shipLineObject.SetActive(true);
				this.setAlarmButtonObject.SetActive(false);
			}
		}

		// Token: 0x06004D2F RID: 19759 RVA: 0x0020DB0C File Offset: 0x0020BD0C
		public void OnClickToggleAlarm()
		{
			if (this.activePlayer.alarms.Any<Alarm>((Alarm x) => x.associatedGameState == this.fleet))
			{
				this.activePlayer.playerControl.StartAction(new DeleteFleetAlarm(this.activePlayer, this.fleet));
			}
			else if (SpaceObjectDetailController.CreateFleetAlarm(this.activePlayer, this.fleet))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			}
			this.setAlarmText.SetText(SpaceObjectDetailController.GetFleetAlarmText(this.activePlayer, this.fleet));
		}

		// Token: 0x06004D30 RID: 19760 RVA: 0x0020DBA4 File Offset: 0x0020BDA4
		public void CreateGroupItem(TIFactionState faction)
		{
			this.isGroupItem = true;
			this.fleet = null;
			this.ship = null;
			this.faction = faction;
			this.fleetName.SetText(faction.displayNameCapitalized);
			this.fleetLineObject.SetActive(true);
			float num = 0f;
			int num2 = 0;
			foreach (TISpaceFleetState tispaceFleetState in faction.fleets)
			{
				if (tispaceFleetState.VisibleToFaction(this.activePlayer))
				{
					num += tispaceFleetState.SpaceCombatValue();
					num2 += tispaceFleetState.ships.Count;
				}
			}
			this.fleetShipsCount.SetText(num2.ToString());
			this.BGImage.color = new Color32(80, 160, 171, byte.MaxValue);
			this.fleetIcon.sprite = faction.factionIcon128;
			this.shipLineObject.SetActive(false);
			this.damagedShipsInFleet.enabled = false;
			this.fleetShipsCombatScore.SetText(num.ToString("N0"));
			this.transferDataPanel.SetActive(false);
			this.accelerationText.SetText("");
			this.fleetHomeportObject.SetActive(false);
			this.fleetHomeportText.SetText(string.Empty);
			this.deltaVText.SetText("");
			this.renameMyFleetPanel.SetActive(false);
			this.fleetCouncilorIconGrid.SetListSize<CombatShipCouncilorGridItemController>(0, true, false);
			this.fleetOperationsGrid.SetListSize<FleetsScreenFleetOperationGridItemController>(0, true, false);
			this.gotoButton.SetActive(false);
			this.editButton.SetActive(false);
			this.genericOpDetailPanel.SetActive(false);
			this.operationDetailBG.enabled = false;
			this.dvIcon.enabled = false;
			this.accelIcon.enabled = false;
			this.fleetPendingCombat.enabled = false;
			this.setAlarmButtonObject.SetActive(false);
		}

		// Token: 0x06004D31 RID: 19761 RVA: 0x0020DDA0 File Offset: 0x0020BFA0
		private void ToggleCollapsed()
		{
			this.controller.ToggleFactionFleets(this.faction);
		}

		// Token: 0x06004D32 RID: 19762 RVA: 0x0020DDB4 File Offset: 0x0020BFB4
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

		// Token: 0x06004D33 RID: 19763 RVA: 0x0020DE34 File Offset: 0x0020C034
		public void OnClickRename()
		{
			if (this.fleet == null || this.fleet.faction == null)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.nameInputField.text = this.fleet.GetDisplayName(this.fleet.faction);
			this.ShowRenameMyFleetPanel();
		}

		// Token: 0x06004D34 RID: 19764 RVA: 0x0020DE96 File Offset: 0x0020C096
		public void OnClickRevertRename()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.RevertRename();
		}

		// Token: 0x06004D35 RID: 19765 RVA: 0x0020DEAA File Offset: 0x0020C0AA
		public void RevertRename()
		{
			this.renameMyFleetPanel.SetActive(false);
			this.nameInputField.text = "";
		}

		// Token: 0x06004D36 RID: 19766 RVA: 0x0020DEC8 File Offset: 0x0020C0C8
		public void OnClickSaveName()
		{
			this.renameMyFleetPanel.SetActive(false);
			this.fleet.faction.playerControl.StartAction(new ChangeFleetBio(this.fleet, this.activePlayer, this.nameInputField.text));
			this.UpdateFleet();
			this.controller.UpdateFleetsList();
			GameControl.eventManager.TriggerEvent(new GameStateNameChanged(this.fleet), null, Array.Empty<object>());
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		}

		// Token: 0x06004D37 RID: 19767 RVA: 0x0020DF4A File Offset: 0x0020C14A
		public void ShowRenameMyFleetPanel()
		{
			this.renameMyFleetPanel.SetActive(true);
			this.nameInputField.Select();
		}

		// Token: 0x06004D38 RID: 19768 RVA: 0x0020DF63 File Offset: 0x0020C163
		public void OnSelectInputBox()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x06004D39 RID: 19769 RVA: 0x0020DF6A File Offset: 0x0020C16A
		public void OnDeSelectInputBox()
		{
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x06004D3A RID: 19770 RVA: 0x0020DF71 File Offset: 0x0020C171
		public void OnOpenFleetButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.FleetButtonClicked();
		}

		// Token: 0x06004D3B RID: 19771 RVA: 0x0020DF88 File Offset: 0x0020C188
		public void FleetButtonClicked()
		{
			this.controller.fleetScreenFleetListAdapter.idToBringIsGroup = this.isGroupItem;
			if (this.isGroupItem)
			{
				this.ToggleCollapsed();
				return;
			}
			this.controller.fleetScreenFleetListAdapter.idToBringToView = this.fleet.ref_gameState.ID;
			this.controller.fleetOpenedStatus[this.fleet] = !this.controller.fleetOpenedStatus[this.fleet];
			this.controller.UpdateFleetsList();
		}

		// Token: 0x06004D3C RID: 19772 RVA: 0x0020E014 File Offset: 0x0020C214
		public void ShipButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.controller.ShowIndividualDataScreen(this.ship, true);
		}

		// Token: 0x06004D3D RID: 19773 RVA: 0x0020E034 File Offset: 0x0020C234
		public void LocationButtonPressed()
		{
			if (this.o1 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
				this.controller.CloseInfoScreen(false);
				TIUtilities.GotoGameState(this.o1, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x06004D3E RID: 19774 RVA: 0x0020E071 File Offset: 0x0020C271
		public void SmallLocationButtonPressed()
		{
			if (this.o2 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
				this.controller.CloseInfoScreen(false);
				TIUtilities.GotoGameState(this.o2, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x06004D3F RID: 19775 RVA: 0x0020E0AE File Offset: 0x0020C2AE
		public void BigDestinationButtonPressed()
		{
			if (this.d1 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
				this.controller.CloseInfoScreen(false);
				TIUtilities.GotoGameState(this.d1, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x06004D40 RID: 19776 RVA: 0x0020E0EB File Offset: 0x0020C2EB
		public void SmallDestinationButtonPressed()
		{
			if (this.d2 != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
				this.controller.CloseInfoScreen(false);
				TIUtilities.GotoGameState(this.d2, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x06004D41 RID: 19777 RVA: 0x0020E128 File Offset: 0x0020C328
		private void OnDestroy()
		{
			this.RemoveFleetListeners();
		}

		// Token: 0x04002FB9 RID: 12217
		private FleetsScreenController controller;

		// Token: 0x04002FBA RID: 12218
		public TISpaceFleetState fleet;

		// Token: 0x04002FBB RID: 12219
		public TISpaceShipState ship;

		// Token: 0x04002FBC RID: 12220
		private TIFactionState faction;

		// Token: 0x04002FBD RID: 12221
		public bool isGroupItem;

		// Token: 0x04002FBE RID: 12222
		[Header("Fleet Data")]
		public GameObject fleetLineObject;

		// Token: 0x04002FBF RID: 12223
		public Image fleetIcon;

		// Token: 0x04002FC0 RID: 12224
		public TMP_Text fleetName;

		// Token: 0x04002FC1 RID: 12225
		public TMP_Text fleetShipsCount;

		// Token: 0x04002FC2 RID: 12226
		public TMP_Text fleetShipsCombatScore;

		// Token: 0x04002FC3 RID: 12227
		public GameObject transferDataPanel;

		// Token: 0x04002FC4 RID: 12228
		public Image transferOriginIcon;

		// Token: 0x04002FC5 RID: 12229
		public Image transferDestinationIcon;

		// Token: 0x04002FC6 RID: 12230
		public Image transferDestinationDetailIcon;

		// Token: 0x04002FC7 RID: 12231
		public RectTransform fleetTransferProgressLine;

		// Token: 0x04002FC8 RID: 12232
		private float fleetTransferSliderZeroPoint;

		// Token: 0x04002FC9 RID: 12233
		private float fleetTransferSliderRange;

		// Token: 0x04002FCA RID: 12234
		public Image transferProgressIcon;

		// Token: 0x04002FCB RID: 12235
		public Image transferPendingCombatIcon;

		// Token: 0x04002FCC RID: 12236
		public TMP_Text transferTextDetail;

		// Token: 0x04002FCD RID: 12237
		public GameObject genericOpDetailPanel;

		// Token: 0x04002FCE RID: 12238
		public Image genericOpImage;

		// Token: 0x04002FCF RID: 12239
		public Image genericOpImageSmall;

		// Token: 0x04002FD0 RID: 12240
		public TMP_Text genericOpLine1;

		// Token: 0x04002FD1 RID: 12241
		public TMP_Text genericOpLine2;

		// Token: 0x04002FD2 RID: 12242
		public GameObject fleetHomeportObject;

		// Token: 0x04002FD3 RID: 12243
		public TMP_Text fleetHomeportText;

		// Token: 0x04002FD4 RID: 12244
		public GameObject setAlarmButtonObject;

		// Token: 0x04002FD5 RID: 12245
		public TooltipTrigger setAlarmTooltip;

		// Token: 0x04002FD6 RID: 12246
		public TMP_Text setAlarmText;

		// Token: 0x04002FD7 RID: 12247
		public TMP_Text accelerationText;

		// Token: 0x04002FD8 RID: 12248
		public TMP_Text deltaVText;

		// Token: 0x04002FD9 RID: 12249
		public Image fleetPendingCombat;

		// Token: 0x04002FDA RID: 12250
		public Image damagedShipsInFleet;

		// Token: 0x04002FDB RID: 12251
		public Image dvIcon;

		// Token: 0x04002FDC RID: 12252
		public Image accelIcon;

		// Token: 0x04002FDD RID: 12253
		public Image operationDetailBG;

		// Token: 0x04002FDE RID: 12254
		public Image BGImage;

		// Token: 0x04002FDF RID: 12255
		public GameObject gotoButton;

		// Token: 0x04002FE0 RID: 12256
		public GameObject editButton;

		// Token: 0x04002FE1 RID: 12257
		private TIGameState o1;

		// Token: 0x04002FE2 RID: 12258
		private TIGameState o2;

		// Token: 0x04002FE3 RID: 12259
		private TIGameState d1;

		// Token: 0x04002FE4 RID: 12260
		private TIGameState d2;

		// Token: 0x04002FE5 RID: 12261
		public ListManagerBase fleetOperationsGrid;

		// Token: 0x04002FE6 RID: 12262
		public ListManagerBase fleetCouncilorIconGrid;

		// Token: 0x04002FE7 RID: 12263
		[Header("My Fleet Customization")]
		public GameObject renameMyFleetPanel;

		// Token: 0x04002FE8 RID: 12264
		public TextMeshProUGUI saveNameText;

		// Token: 0x04002FE9 RID: 12265
		public TextMeshProUGUI revertNameText;

		// Token: 0x04002FEA RID: 12266
		public TMP_InputField nameInputField;

		// Token: 0x04002FEB RID: 12267
		[Header("Ship Data")]
		public GameObject shipLineObject;

		// Token: 0x04002FEC RID: 12268
		public Image nose;

		// Token: 0x04002FED RID: 12269
		public Image hull;

		// Token: 0x04002FEE RID: 12270
		public Image tail;

		// Token: 0x04002FEF RID: 12271
		public Image drive;

		// Token: 0x04002FF0 RID: 12272
		public Image radiator;

		// Token: 0x04002FF1 RID: 12273
		public TMP_Text shipName;

		// Token: 0x04002FF2 RID: 12274
		public TMP_Text shipClass;

		// Token: 0x04002FF3 RID: 12275
		public TMP_Text shipRole;

		// Token: 0x04002FF4 RID: 12276
		public TMP_Text shipCombatScore;

		// Token: 0x04002FF5 RID: 12277
		public TMP_Text shipAcceleration;

		// Token: 0x04002FF6 RID: 12278
		public TMP_Text shipDeltaV;

		// Token: 0x04002FF7 RID: 12279
		public ListManagerBase shipCouncilorIconGrid;

		// Token: 0x04002FF8 RID: 12280
		public TooltipTrigger shipSummaryTooltip;

		// Token: 0x04002FF9 RID: 12281
		public ListManagerBase shipOfficerIconGrid;

		// Token: 0x04002FFA RID: 12282
		private bool fleetDataDirty;
	}
}
