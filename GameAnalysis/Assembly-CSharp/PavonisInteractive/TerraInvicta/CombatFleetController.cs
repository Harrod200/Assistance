using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200070A RID: 1802
	public class CombatFleetController
	{
		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x06002AB1 RID: 10929 RVA: 0x000E7E3C File Offset: 0x000E603C
		// (set) Token: 0x06002AB2 RID: 10930 RVA: 0x000E7E44 File Offset: 0x000E6044
		public TISpaceFleetState fleetState { get; private set; }

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06002AB3 RID: 10931 RVA: 0x000E7E4D File Offset: 0x000E604D
		// (set) Token: 0x06002AB4 RID: 10932 RVA: 0x000E7E55 File Offset: 0x000E6055
		public GameObject strategyFleetObject { get; private set; }

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x06002AB5 RID: 10933 RVA: 0x000E7E5E File Offset: 0x000E605E
		public bool IsActivePlayerFleet
		{
			get
			{
				return this.fleetState.faction == GameControl.control.activePlayer;
			}
		}

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06002AB6 RID: 10934 RVA: 0x000E7E7A File Offset: 0x000E607A
		public bool IsFleetDestroyed
		{
			get
			{
				return this.fleetState.ships.Count == 0;
			}
		}

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x06002AB7 RID: 10935 RVA: 0x000E7E90 File Offset: 0x000E6090
		public string FleetID
		{
			get
			{
				return this.fleetState.ID.ToString();
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06002AB8 RID: 10936 RVA: 0x000E7EB6 File Offset: 0x000E60B6
		public bool IsUnderAIControl
		{
			get
			{
				return this._isUnderAIControl;
			}
		}

		// Token: 0x06002AB9 RID: 10937 RVA: 0x000E7EBE File Offset: 0x000E60BE
		private CombatFleetController()
		{
		}

		// Token: 0x06002ABA RID: 10938 RVA: 0x000E7ED4 File Offset: 0x000E60D4
		public CombatFleetController(int fleetIndex, float velocity, TISpaceFleetState fleetState, TIFactionState faction, List<CombatShipController> shipControllers, List<TISpaceShipState> reinforcements, GameObject strategyFleetObject)
		{
			this.FleetIndex = fleetIndex;
			this.InitialVelocty = velocity;
			this.fleetState = fleetState;
			this.faction = faction;
			this.activeShipControllers = shipControllers;
			this.reinforcements = reinforcements;
			this.strategyFleetObject = strategyFleetObject;
			this.HasFleetFiredThisCombat = false;
			this.UpdateCombatRating();
			this.UpdateAvgFleetManeuverabilityRatingPerCombatScore();
			this.AddFleetListeners();
		}

		// Token: 0x06002ABB RID: 10939 RVA: 0x000E7F40 File Offset: 0x000E6140
		public void EndCombatCleanUp()
		{
			this.RemoveFleetListeners();
		}

		// Token: 0x06002ABC RID: 10940 RVA: 0x000E7F48 File Offset: 0x000E6148
		public void UpdateCombatRating()
		{
			float num = 0f;
			foreach (TISpaceShipState tispaceShipState in this.fleetState.ships)
			{
				if (!tispaceShipState.ShipDestroyed() && tispaceShipState.AnyOffensiveWeaponCanFire())
				{
					num += tispaceShipState.SpaceCombatValue(true, 0f);
				}
			}
			this.CombatRating = num;
		}

		// Token: 0x06002ABD RID: 10941 RVA: 0x000E7FC8 File Offset: 0x000E61C8
		public void UpdateAvgFleetManeuverabilityRatingPerCombatScore()
		{
			float num = 0f;
			foreach (TISpaceShipState tispaceShipState in this.fleetState.ships)
			{
				num += tispaceShipState.angularAcceleration_degs2 * 60f * tispaceShipState.combatAcceleration_gs * tispaceShipState.SpaceCombatValue(true, 0f);
			}
			this.AvgFleetManeuverabilityRatingPerCombatScore = num / this.CombatRating;
		}

		// Token: 0x06002ABE RID: 10942 RVA: 0x000E8050 File Offset: 0x000E6250
		public Vector3 GetCenterOfMass()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			foreach (CombatShipController combatShipController in this.activeShipControllers)
			{
				float num5 = Mathf.Max((float)combatShipController.ShipState.dryMass_kg * combatShipController.ShipState.SpaceCombatValue(true, 0f), 1f);
				num4 += num5;
				num += combatShipController.position.x * num5;
				num2 += combatShipController.position.y * num5;
				num3 += combatShipController.position.z * num5;
			}
			float num6 = 1f / num4 * num;
			float num7 = 1f / num4 * num2;
			float num8 = 1f / num4 * num3;
			return new Vector3(num6, num7, num8);
		}

		// Token: 0x06002ABF RID: 10943 RVA: 0x000E8144 File Offset: 0x000E6344
		public Vector3 GetAveragePosition()
		{
			Vector3 vector = Vector3.zero;
			foreach (CombatShipController combatShipController in this.activeShipControllers)
			{
				vector += combatShipController.position;
			}
			vector /= (float)this.activeShipControllers.Count<CombatShipController>();
			return vector;
		}

		// Token: 0x06002AC0 RID: 10944 RVA: 0x000E81B4 File Offset: 0x000E63B4
		public Vector3 GetFleetVelocityVector()
		{
			Vector3 vector = default(Vector3);
			foreach (CombatShipController combatShipController in this.activeShipControllers)
			{
				vector += combatShipController.velocityVector;
			}
			return vector;
		}

		// Token: 0x06002AC1 RID: 10945 RVA: 0x000E8210 File Offset: 0x000E6410
		public bool AllActiveShipsDestroyed()
		{
			return this.activeShipControllers.Where<CombatShipController>((CombatShipController x) => x.isDestroyed).ToList<CombatShipController>().Count == this.activeShipControllers.Count;
		}

		// Token: 0x06002AC2 RID: 10946 RVA: 0x000E8260 File Offset: 0x000E6460
		public void AddFleetListeners()
		{
			GameControl.eventManager.AddListener<ShipWeaponFired>(new EventManager.EventDelegate<ShipWeaponFired>(this.OnShipWeaponFired), null, this.faction, true, true);
			GameControl.eventManager.AddListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.OnShipRemoved), null, null, false, false);
			GameControl.eventManager.AddListener<FleetAIControlChanged>(new EventManager.EventDelegate<FleetAIControlChanged>(this.OnFleetAIControlChanged), null, null, true, false);
		}

		// Token: 0x06002AC3 RID: 10947 RVA: 0x000E82C0 File Offset: 0x000E64C0
		public void RemoveFleetListeners()
		{
			GameControl.eventManager.RemoveListener<ShipWeaponFired>(new EventManager.EventDelegate<ShipWeaponFired>(this.OnShipWeaponFired), null);
			GameControl.eventManager.RemoveListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.OnShipRemoved), null);
			GameControl.eventManager.RemoveListener<FleetAIControlChanged>(new EventManager.EventDelegate<FleetAIControlChanged>(this.OnFleetAIControlChanged), null);
		}

		// Token: 0x06002AC4 RID: 10948 RVA: 0x000E8314 File Offset: 0x000E6514
		private void OnShipRemoved(ShipDestroyed e)
		{
			CombatShipController combatShipController = this.activeShipControllers.Where<CombatShipController>((CombatShipController o) => o.ShipState == e.ship).FirstOrDefault<CombatShipController>();
			if (combatShipController != null)
			{
				this.activeShipControllers.Remove(combatShipController);
			}
		}

		// Token: 0x06002AC5 RID: 10949 RVA: 0x000E8361 File Offset: 0x000E6561
		private void OnShipWeaponFired(ShipWeaponFired e)
		{
			if (!this.HasFleetFiredThisCombat && this.faction == e.ship.faction)
			{
				this.HasFleetFiredThisCombat = true;
			}
		}

		// Token: 0x06002AC6 RID: 10950 RVA: 0x000E838A File Offset: 0x000E658A
		private void OnFleetAIControlChanged(FleetAIControlChanged e)
		{
			if (e.fleet == this.fleetState)
			{
				this._isUnderAIControl = e.isAIControlEnabled;
			}
		}

		// Token: 0x040020B6 RID: 8374
		public IList<CombatShipController> activeShipControllers;

		// Token: 0x040020B7 RID: 8375
		public IList<TISpaceShipState> reinforcements;

		// Token: 0x040020B8 RID: 8376
		public IList<CombatShipController> disengagedShips = new List<CombatShipController>();

		// Token: 0x040020B9 RID: 8377
		public int FleetIndex;

		// Token: 0x040020BA RID: 8378
		public float InitialVelocty;

		// Token: 0x040020BB RID: 8379
		public float CombatRating;

		// Token: 0x040020BC RID: 8380
		public float AvgFleetManeuverabilityRatingPerCombatScore;

		// Token: 0x040020BD RID: 8381
		public TIFactionState faction;

		// Token: 0x040020C0 RID: 8384
		public bool HasFleetFiredThisCombat;

		// Token: 0x040020C1 RID: 8385
		private bool _isUnderAIControl;
	}
}
