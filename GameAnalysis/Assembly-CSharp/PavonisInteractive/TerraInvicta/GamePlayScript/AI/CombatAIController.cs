using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.GamePlayScript.PathFinding;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A47 RID: 2631
	public class CombatAIController
	{
		// Token: 0x1700111A RID: 4378
		// (get) Token: 0x060064D0 RID: 25808 RVA: 0x002F935A File Offset: 0x002F755A
		private TISpaceCombatState combatState
		{
			get
			{
				return this._combatManager.combatState;
			}
		}

		// Token: 0x060064D1 RID: 25809 RVA: 0x002F9368 File Offset: 0x002F7568
		public CombatAIController(SpaceCombatManager manager, TIDateTime currentTime, CombatFleetController[] fleetControllers, CombatHabModuleController[] habModuleControllers)
		{
			this._combatManager = manager;
			this._fleetControllers = fleetControllers;
			this._habModuleControllers = habModuleControllers;
			int num = 1;
			Vector3 vector = Vector3.zero;
			for (int k = 0; k < fleetControllers.Length; k++)
			{
				for (int j = 0; j < fleetControllers[k].activeShipControllers.Count; j++)
				{
					vector += fleetControllers[k].activeShipControllers[j].position;
					num++;
				}
			}
			vector /= (float)num;
			this._navVolume = new OcTree(120, 20f, vector);
			this._pathFinder = new Pathfinding(this._navVolume);
			this._shipBehaviours = new List<CombatShipBehaviourTree>(12);
			int i;
			Func<CombatFleetController, bool> <>9__0;
			int num2;
			for (i = 0; i < fleetControllers.Length; i = num2)
			{
				IEnumerable<CombatFleetController> fleetControllers2 = fleetControllers;
				Func<CombatFleetController, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (CombatFleetController x) => x.faction != fleetControllers[i].faction);
				}
				CombatFleetController combatFleetController = fleetControllers2.FirstOrDefault<CombatFleetController>(func);
				this.DivideShipsIntoSquadronsAndConfigureAI(fleetControllers[i].activeShipControllers, fleetControllers[i], combatFleetController, currentTime);
				num2 = i + 1;
			}
			this.lastTruceCheck = currentTime;
		}

		// Token: 0x060064D2 RID: 25810 RVA: 0x002F94C8 File Offset: 0x002F76C8
		public void DivideShipsIntoSquadronsAndConfigureAI(IList<CombatShipController> shipsToConfigure, CombatFleetController fleet, CombatFleetController enemyFleet, TIDateTime currentTime)
		{
			List<CombatShipController> list = new List<CombatShipController>();
			List<CombatShipController> list2 = new List<CombatShipController>();
			List<CombatShipController> list3 = new List<CombatShipController>();
			foreach (CombatShipController combatShipController in shipsToConfigure)
			{
				if (combatShipController.ShipState.nonCombatant)
				{
					list.Add(combatShipController);
				}
				else if (this.ShipQualifiesAsInterceptor(combatShipController, enemyFleet))
				{
					list3.Add(combatShipController);
				}
				else
				{
					list2.Add(combatShipController);
				}
			}
			int num = list3.Count + list2.Count;
			if (num > 0)
			{
				while ((float)list3.Count / (float)num > 0.34f)
				{
					CombatShipController combatShipController2 = list3.SelectRandomItem<CombatShipController>();
					list3.Remove(combatShipController2);
					list2.Add(combatShipController2);
				}
			}
			if (list.Count > 0)
			{
				this.AddShipOfTheLineSquadron(list, fleet, enemyFleet, currentTime);
			}
			if (list2.Count > 0)
			{
				this.AddShipOfTheLineSquadron(list2, fleet, enemyFleet, currentTime);
			}
			if (list3.Count > 0)
			{
				int num2 = 1 << LayerMask.NameToLayer("HurtBox");
				int num3 = ((list3.Count < 15) ? 5 : (list3.Count / 3));
				int num4 = -1;
				int num5 = 0;
				Dictionary<CombatShipController, int> dictionary = new Dictionary<CombatShipController, int>();
				int num6 = 0;
				while (dictionary.Count < list3.Count && num6 < 10)
				{
					for (int i = 0; i < list3.Count; i++)
					{
						if (i == list3.Count - 1)
						{
							num6++;
						}
						if (!dictionary.ContainsKey(list3[i]))
						{
							float num7 = 2f * SpaceCombatManager.km_to_scale((float)TIFormationTemplate.GetSpacingOffset_km(false, false)[(int)fleet.fleetState.formation.spacing].x);
							Collider[] array = Physics.OverlapSphere(list3[i].position, num7, num2);
							if (array.Length == 0 && !dictionary.ContainsKey(list3[i]))
							{
								num4++;
								dictionary.Add(list3[i], num4);
								break;
							}
							foreach (Collider collider in array)
							{
								if (!(collider.attachedRigidbody == null))
								{
									CombatShipController component = collider.attachedRigidbody.GetComponent<CombatShipController>();
									if (!(component == null) && list3.Contains(component) && !dictionary.ContainsKey(component))
									{
										if (num5 == 0)
										{
											num4++;
										}
										dictionary.Add(component, num4);
										num5++;
										if (num5 == num3)
										{
											num5 = 0;
											break;
										}
									}
								}
							}
						}
					}
				}
				List<List<CombatShipController>> list4 = new List<List<CombatShipController>>();
				for (int k = 0; k < num4 + 1; k++)
				{
					list4.Add(new List<CombatShipController>());
				}
				foreach (CombatShipController combatShipController3 in dictionary.Keys)
				{
					int num8 = -1;
					dictionary.TryGetValue(combatShipController3, out num8);
					list4[num8].Add(combatShipController3);
				}
				foreach (List<CombatShipController> list5 in list4)
				{
					this.AddInterceptorSquadron(list5, fleet, enemyFleet, currentTime);
				}
			}
		}

		// Token: 0x060064D3 RID: 25811 RVA: 0x002F9820 File Offset: 0x002F7A20
		public void AddShipOfTheLineSquadron(IList<CombatShipController> ships, CombatFleetController fleetController, CombatFleetController enemyFleet, TIDateTime currentTime)
		{
			CombatSquadronController combatSquadronController = new CombatSquadronController(ships.ToList<CombatShipController>());
			foreach (CombatShipController combatShipController in ships)
			{
				ShipOfTheLineBehaviourTree shipOfTheLineBehaviourTree = new ShipOfTheLineBehaviourTree(this._pathFinder, fleetController, enemyFleet, combatSquadronController, this._combatManager.waypointTimeDelta, CombatShipBehaviourTree.SharedBehaviourData.FleetPriority.Defensive, currentTime, combatShipController, this._habModuleControllers);
				this._shipBehaviours.Add(shipOfTheLineBehaviourTree);
			}
		}

		// Token: 0x060064D4 RID: 25812 RVA: 0x002F98A0 File Offset: 0x002F7AA0
		public void AddInterceptorSquadron(IList<CombatShipController> ships, CombatFleetController fleetController, CombatFleetController enemyFleet, TIDateTime currentTime)
		{
			CombatSquadronController combatSquadronController = new CombatSquadronController(ships.ToList<CombatShipController>());
			foreach (CombatShipController combatShipController in ships)
			{
				InterceptorBehaviourTree interceptorBehaviourTree = new InterceptorBehaviourTree(this._pathFinder, fleetController, enemyFleet, combatSquadronController, this._combatManager.waypointTimeDelta, CombatShipBehaviourTree.SharedBehaviourData.FleetPriority.Defensive, currentTime, combatShipController, this._habModuleControllers);
				this._shipBehaviours.Add(interceptorBehaviourTree);
			}
		}

		// Token: 0x060064D5 RID: 25813 RVA: 0x002F9920 File Offset: 0x002F7B20
		public void AddShipBehaviour(CombatShipController ship, CombatFleetController fleetController, CombatFleetController enemyFleet, TIDateTime currentTime)
		{
			if (this.ShipQualifiesAsInterceptor(ship, enemyFleet))
			{
				InterceptorBehaviourTree interceptorBehaviourTree = new InterceptorBehaviourTree(this._pathFinder, fleetController, enemyFleet, null, this._combatManager.waypointTimeDelta, CombatShipBehaviourTree.SharedBehaviourData.FleetPriority.Defensive, currentTime, ship, this._habModuleControllers);
				this._shipBehaviours.Add(interceptorBehaviourTree);
				return;
			}
			ShipOfTheLineBehaviourTree shipOfTheLineBehaviourTree = new ShipOfTheLineBehaviourTree(this._pathFinder, fleetController, enemyFleet, null, this._combatManager.waypointTimeDelta, CombatShipBehaviourTree.SharedBehaviourData.FleetPriority.Defensive, currentTime, ship, this._habModuleControllers);
			this._shipBehaviours.Add(shipOfTheLineBehaviourTree);
		}

		// Token: 0x060064D6 RID: 25814 RVA: 0x002F9998 File Offset: 0x002F7B98
		public void RemoveShipBehaviour(CombatShipController shipController)
		{
			for (int i = 0; i < this._shipBehaviours.Count; i++)
			{
				if (this._shipBehaviours[i].SharedData.ShipController == shipController)
				{
					this._shipBehaviours.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x060064D7 RID: 25815 RVA: 0x002F99E8 File Offset: 0x002F7BE8
		public void Update(TIDateTime currentTime)
		{
			foreach (CombatShipBehaviourTree combatShipBehaviourTree in this._shipBehaviours)
			{
				combatShipBehaviourTree.Update(currentTime);
			}
			if (currentTime.DifferenceInSeconds(this.lastTruceCheck) > 5.0)
			{
				this.UpdateMutualTruceVotes(currentTime);
				this.lastTruceCheck = currentTime;
			}
		}

		// Token: 0x060064D8 RID: 25816 RVA: 0x002F9A60 File Offset: 0x002F7C60
		private void UpdateMutualTruceVotes(TIDateTime currentTime)
		{
			if (this._combatManager.timeOfLastShotFired != null && currentTime.DifferenceInHours(this._combatManager.timeOfLastShotFired) >= 0.5)
			{
				foreach (TIFactionState tifactionState in this._combatManager.combatState.factions)
				{
					if (tifactionState.player.isAI && !this._combatManager.combatState.votedEndCombat[tifactionState])
					{
						tifactionState.playerControl.StartAction(new SetEndCombatVoteAction(tifactionState, true));
						Debug.Log("No Shots have been fired for a long time - Voted To End Combat");
					}
				}
				return;
			}
			foreach (TIFactionState tifactionState2 in this._combatManager.combatState.factions)
			{
				for (int j = 0; j < this._fleetControllers.Length; j++)
				{
					if (this._fleetControllers[j].faction == tifactionState2 && !this._fleetControllers[j].IsActivePlayerFleet)
					{
						bool flag = this.combatState.hab != null && this.combatState.hab.faction == this._fleetControllers[j].faction;
						float num = (flag ? this.combatState.hab.SpaceCombatValue() : 0f);
						if ((this._fleetControllers[j].AllActiveShipsDestroyed() && !flag) || (this._fleetControllers[j].AllActiveShipsDestroyed() && flag && num <= 0f))
						{
							if (this.combatState.votedEndCombat[this._fleetControllers[j].faction])
							{
								this._fleetControllers[j].faction.playerControl.StartAction(new SetEndCombatVoteAction(this._fleetControllers[j].faction, false));
							}
							return;
						}
					}
				}
			}
			for (int k = 0; k < this._fleetControllers.Length; k++)
			{
				this._fleetControllers[k].UpdateCombatRating();
			}
			Dictionary<TIFactionState, float> dictionary = new Dictionary<TIFactionState, float>();
			dictionary = this._fleetControllers.ToDictionary<CombatFleetController, TIFactionState, float>((CombatFleetController x) => x.faction, (CombatFleetController x) => x.CombatRating);
			if (this.combatState.hab != null)
			{
				if (!dictionary.ContainsKey(this.combatState.hab.faction))
				{
					dictionary.Add(this.combatState.hab.faction, this.combatState.hab.SpaceCombatValue());
				}
				else
				{
					Dictionary<TIFactionState, float> dictionary2 = dictionary;
					TIFactionState tifactionState3 = this.combatState.hab.faction;
					dictionary2[tifactionState3] += this.combatState.hab.SpaceCombatValue();
				}
			}
			for (int l = 0; l < this.combatState.factions.Length; l++)
			{
				TIFactionState tifactionState4 = this.combatState.factions[l];
				if (this._combatManager.liveBallistics.ContainsKey(tifactionState4))
				{
					Dictionary<TIFactionState, float> dictionary2 = dictionary;
					TIFactionState tifactionState3 = tifactionState4;
					dictionary2[tifactionState3] += (float)this._combatManager.liveBallistics[tifactionState4] * 0.2f;
				}
				if (this._combatManager.liveMissiles.ContainsKey(tifactionState4))
				{
					Dictionary<TIFactionState, float> dictionary2 = dictionary;
					TIFactionState tifactionState3 = tifactionState4;
					dictionary2[tifactionState3] += (float)this._combatManager.liveMissiles[tifactionState4];
					if (!tifactionState4.isActivePlayer && this._combatManager.liveMissiles[tifactionState4] > 0)
					{
						if (this.combatState.votedEndCombat[tifactionState4])
						{
							tifactionState4.playerControl.StartAction(new SetEndCombatVoteAction(tifactionState4, false));
						}
						return;
					}
				}
			}
			for (int m = 0; m < this._fleetControllers.Length; m++)
			{
				if (!this._fleetControllers[m].IsActivePlayerFleet && !(this._fleetControllers[m].faction == null))
				{
					float num2 = 0f;
					if (this._fleetControllers.Length > 1)
					{
						num2 = float.MaxValue;
						for (int n = 0; n < this._fleetControllers.Length; n++)
						{
							float num3 = dictionary[this._fleetControllers[n].faction];
							if (m != n && num3 < num2)
							{
								num2 = num3;
							}
						}
					}
					bool flag2 = dictionary[this._fleetControllers[m].faction] == 0f || dictionary[this._fleetControllers[m].faction] < num2;
					if (flag2 != this.combatState.votedEndCombat[this._fleetControllers[m].faction])
					{
						this._fleetControllers[m].faction.playerControl.StartAction(new SetEndCombatVoteAction(this._fleetControllers[m].faction, flag2));
					}
				}
			}
		}

		// Token: 0x060064D9 RID: 25817 RVA: 0x002F9F74 File Offset: 0x002F8174
		private bool ShipQualifiesAsInterceptor(CombatShipController ship, CombatFleetController enemyFleet)
		{
			float num = ship.GetDVConservingAceleration_kps2(true) / 0.00980665f;
			bool flag = ship.ShipState.angularAcceleration_degs2 >= 0.2f;
			bool flag2 = num >= 1f;
			bool flag3 = ship.ShipState.AvailableDeltaVForCombat_kps() >= 60f;
			if (flag && flag2 && flag3)
			{
				float manueverRating = ship.ShipState.manueverRating;
				float? num2 = ((enemyFleet != null) ? new float?(enemyFleet.AvgFleetManeuverabilityRatingPerCombatScore * 0.5f) : null);
				if ((manueverRating >= num2.GetValueOrDefault()) & (num2 != null))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040046E3 RID: 18147
		private const int NAV_VOLUME_SIZE = 120;

		// Token: 0x040046E4 RID: 18148
		private const float BASE_DEPTH_NODE_SIZE = 20f;

		// Token: 0x040046E5 RID: 18149
		private OcTree _navVolume;

		// Token: 0x040046E6 RID: 18150
		private Pathfinding _pathFinder;

		// Token: 0x040046E7 RID: 18151
		private List<CombatShipBehaviourTree> _shipBehaviours;

		// Token: 0x040046E8 RID: 18152
		private CombatFleetController[] _fleetControllers;

		// Token: 0x040046E9 RID: 18153
		private CombatHabModuleController[] _habModuleControllers;

		// Token: 0x040046EA RID: 18154
		private SpaceCombatManager _combatManager;

		// Token: 0x040046EB RID: 18155
		private TIDateTime lastTruceCheck;
	}
}
