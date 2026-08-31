using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009DD RID: 2525
	public class CombatSquadronController
	{
		// Token: 0x17001042 RID: 4162
		// (get) Token: 0x06005EC3 RID: 24259 RVA: 0x002CE861 File Offset: 0x002CCA61
		public CombatShipController SquadLeader
		{
			get
			{
				return this.squadLeaderController;
			}
		}

		// Token: 0x17001043 RID: 4163
		// (get) Token: 0x06005EC4 RID: 24260 RVA: 0x002CE869 File Offset: 0x002CCA69
		public AccelerationConstraints ManeuverConstraints
		{
			get
			{
				return this.maneuverabilityConstraints;
			}
		}

		// Token: 0x17001044 RID: 4164
		// (get) Token: 0x06005EC5 RID: 24261 RVA: 0x002CE871 File Offset: 0x002CCA71
		public bool SquadronReadyToManeuver
		{
			get
			{
				return this.shipControllers.Count - 1 == this.trajectoryMatchedShips.Count;
			}
		}

		// Token: 0x06005EC6 RID: 24262 RVA: 0x002CE890 File Offset: 0x002CCA90
		public CombatSquadronController(List<CombatShipController> ships)
		{
			this.shipControllers = ships;
			this.SortSquadronListLargestFirst();
			this.squadLeaderController = this.shipControllers[0];
			this.maneuverabilityConstraints = TIUtilities.GetAccelerationConstraintsForGroup(this.shipControllers, true);
			this.trajectoryMatchedShips = new List<CombatShipController>();
			GameControl.eventManager.AddListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.OnShipDestroyed), null, null, true, false);
			GameControl.eventManager.AddListener<CombatShipPropulsionValuesUpdated>(new EventManager.EventDelegate<CombatShipPropulsionValuesUpdated>(this.OnPropulsionValuesUpdated), null, null, false, false);
		}

		// Token: 0x06005EC7 RID: 24263 RVA: 0x002CE914 File Offset: 0x002CCB14
		~CombatSquadronController()
		{
			GameControl.eventManager.RemoveListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.OnShipDestroyed), null);
			GameControl.eventManager.RemoveListener<CombatShipPropulsionValuesUpdated>(new EventManager.EventDelegate<CombatShipPropulsionValuesUpdated>(this.OnPropulsionValuesUpdated), null);
		}

		// Token: 0x06005EC8 RID: 24264 RVA: 0x002CE968 File Offset: 0x002CCB68
		private void SortSquadronListLargestFirst()
		{
			this.shipControllers = this.shipControllers.OrderByDescending<CombatShipController, float>((CombatShipController x) => x.ShipState.hull.length_m).ThenByDescending<CombatShipController, double>((CombatShipController y) => y.ShipState.wetMass_tons).ToList<CombatShipController>();
		}

		// Token: 0x06005EC9 RID: 24265 RVA: 0x002CE9CE File Offset: 0x002CCBCE
		public bool ShipIsTrajectoryMatched(CombatShipController ship)
		{
			return this.trajectoryMatchedShips.Contains(ship);
		}

		// Token: 0x06005ECA RID: 24266 RVA: 0x002CE9DC File Offset: 0x002CCBDC
		public void UpdateTrajectoryMatchedShips(CombatShipController ship, bool hasMatchedTrajectory)
		{
			bool flag = this.trajectoryMatchedShips.Contains(ship);
			if (hasMatchedTrajectory && !flag)
			{
				this.trajectoryMatchedShips.Add(ship);
				return;
			}
			if (!hasMatchedTrajectory && flag)
			{
				this.trajectoryMatchedShips.Remove(ship);
			}
		}

		// Token: 0x06005ECB RID: 24267 RVA: 0x002CEA20 File Offset: 0x002CCC20
		public bool RemoveShipFromSquadron(TISpaceShipState removedShipState)
		{
			if (this.shipControllers.Exists((CombatShipController x) => x.ShipState == removedShipState))
			{
				CombatShipController combatShipController = this.shipControllers.Single<CombatShipController>((CombatShipController x) => x.ShipState == removedShipState);
				this.shipControllers.Remove(combatShipController);
				if (this.trajectoryMatchedShips.Contains(combatShipController))
				{
					this.trajectoryMatchedShips.Remove(combatShipController);
				}
				this.maneuverabilityConstraints = TIUtilities.GetAccelerationConstraintsForGroup(this.shipControllers, true);
				if (combatShipController == this.squadLeaderController)
				{
					if (this.shipControllers.Count > 0)
					{
						this.squadLeaderController = this.shipControllers[0];
						if (this.trajectoryMatchedShips.Contains(this.squadLeaderController))
						{
							this.trajectoryMatchedShips.Remove(this.squadLeaderController);
						}
					}
					else
					{
						this.squadLeaderController = null;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005ECC RID: 24268 RVA: 0x002CEB07 File Offset: 0x002CCD07
		private void OnShipDestroyed(ShipDestroyed e)
		{
			this.RemoveShipFromSquadron(e.ship);
		}

		// Token: 0x06005ECD RID: 24269 RVA: 0x002CEB18 File Offset: 0x002CCD18
		private void OnPropulsionValuesUpdated(CombatShipPropulsionValuesUpdated e)
		{
			if (this.shipControllers.Exists((CombatShipController x) => x.ShipState == e.ship))
			{
				this.maneuverabilityConstraints = TIUtilities.GetAccelerationConstraintsForGroup(this.shipControllers, true);
			}
		}

		// Token: 0x04004399 RID: 17305
		private List<CombatShipController> shipControllers;

		// Token: 0x0400439A RID: 17306
		private CombatShipController squadLeaderController;

		// Token: 0x0400439B RID: 17307
		private AccelerationConstraints maneuverabilityConstraints;

		// Token: 0x0400439C RID: 17308
		private List<CombatShipController> trajectoryMatchedShips;
	}
}
