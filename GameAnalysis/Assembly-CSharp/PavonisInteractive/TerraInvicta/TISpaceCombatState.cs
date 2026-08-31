using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using FullSerializer;
using ModestTree;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007B2 RID: 1970
	public class TISpaceCombatState : TIGameState
	{
		// Token: 0x17000C98 RID: 3224
		// (get) Token: 0x060042C2 RID: 17090 RVA: 0x001AF1A9 File Offset: 0x001AD3A9
		public static TISpaceCombatState CurrentActiveCombat
		{
			get
			{
				return GameControl.spaceCombat.combatState;
			}
		}

		// Token: 0x17000C99 RID: 3225
		// (get) Token: 0x060042C3 RID: 17091 RVA: 0x001AF1B5 File Offset: 0x001AD3B5
		public TISpaceCombatTemplate template
		{
			get
			{
				return this.GetMyTemplate<TISpaceCombatTemplate>();
			}
		}

		// Token: 0x17000C9A RID: 3226
		// (get) Token: 0x060042C4 RID: 17092 RVA: 0x001AF1BD File Offset: 0x001AD3BD
		// (set) Token: 0x060042C5 RID: 17093 RVA: 0x001AF1C5 File Offset: 0x001AD3C5
		public Dictionary<TIFactionState, CombatStance> stances { get; private set; }

		// Token: 0x17000C9B RID: 3227
		// (get) Token: 0x060042C6 RID: 17094 RVA: 0x001AF1CE File Offset: 0x001AD3CE
		// (set) Token: 0x060042C7 RID: 17095 RVA: 0x001AF1D6 File Offset: 0x001AD3D6
		public Dictionary<TIFactionState, float> bids_kps { get; private set; }

		// Token: 0x17000C9C RID: 3228
		// (get) Token: 0x060042C8 RID: 17096 RVA: 0x001AF1DF File Offset: 0x001AD3DF
		// (set) Token: 0x060042C9 RID: 17097 RVA: 0x001AF1E7 File Offset: 0x001AD3E7
		public double precombatDuration_s { get; private set; }

		// Token: 0x17000C9D RID: 3229
		// (get) Token: 0x060042CA RID: 17098 RVA: 0x001AF1F0 File Offset: 0x001AD3F0
		// (set) Token: 0x060042CB RID: 17099 RVA: 0x001AF1F8 File Offset: 0x001AD3F8
		public float combatBalance { get; private set; }

		// Token: 0x17000C9E RID: 3230
		// (get) Token: 0x060042CC RID: 17100 RVA: 0x001AF201 File Offset: 0x001AD401
		// (set) Token: 0x060042CD RID: 17101 RVA: 0x001AF209 File Offset: 0x001AD409
		public bool initialized { get; private set; }

		// Token: 0x17000C9F RID: 3231
		// (get) Token: 0x060042CE RID: 17102 RVA: 0x001AF212 File Offset: 0x001AD412
		// (set) Token: 0x060042CF RID: 17103 RVA: 0x001AF21A File Offset: 0x001AD41A
		public bool fightersInitialized { get; private set; }

		// Token: 0x17000CA0 RID: 3232
		// (get) Token: 0x060042D0 RID: 17104 RVA: 0x001AF223 File Offset: 0x001AD423
		public TISpaceFleetState attacker
		{
			get
			{
				return this.fleets[0];
			}
		}

		// Token: 0x17000CA1 RID: 3233
		// (get) Token: 0x060042D1 RID: 17105 RVA: 0x001AF22D File Offset: 0x001AD42D
		public TIFactionState attackingFaction
		{
			get
			{
				return this.fleets[0].faction;
			}
		}

		// Token: 0x17000CA2 RID: 3234
		// (get) Token: 0x060042D2 RID: 17106 RVA: 0x001AF23C File Offset: 0x001AD43C
		public TIFactionState defendingFaction
		{
			get
			{
				TISpaceFleetState tispaceFleetState = this.fleets[1];
				return ((tispaceFleetState != null) ? tispaceFleetState.faction : null) ?? this.hab.faction;
			}
		}

		// Token: 0x17000CA3 RID: 3235
		// (get) Token: 0x060042D3 RID: 17107 RVA: 0x001AF261 File Offset: 0x001AD461
		// (set) Token: 0x060042D4 RID: 17108 RVA: 0x001AF269 File Offset: 0x001AD469
		public TIFactionState winner { get; private set; }

		// Token: 0x17000CA4 RID: 3236
		// (get) Token: 0x060042D5 RID: 17109 RVA: 0x001AF272 File Offset: 0x001AD472
		// (set) Token: 0x060042D6 RID: 17110 RVA: 0x001AF27A File Offset: 0x001AD47A
		public TIFactionState loser { get; private set; }

		// Token: 0x17000CA5 RID: 3237
		// (get) Token: 0x060042D7 RID: 17111 RVA: 0x001AF284 File Offset: 0x001AD484
		public TISpaceFleetState winningFleet
		{
			get
			{
				if (this.draw)
				{
					return null;
				}
				if (this.fleets[0] != null && this.fleets[0].faction == this.winner)
				{
					return this.fleets[0];
				}
				if (this.fleets[1] != null && this.fleets[1].faction == this.winner)
				{
					return this.fleets[1];
				}
				return null;
			}
		}

		// Token: 0x17000CA6 RID: 3238
		// (get) Token: 0x060042D8 RID: 17112 RVA: 0x001AF304 File Offset: 0x001AD504
		public TISpaceFleetState losingFleet
		{
			get
			{
				if (this.draw)
				{
					return null;
				}
				if (this.fleets[0] != null && this.fleets[0].faction == this.loser)
				{
					return this.fleets[0];
				}
				if (this.fleets[1] != null && this.fleets[1].faction == this.loser)
				{
					return this.fleets[1];
				}
				return null;
			}
		}

		// Token: 0x17000CA7 RID: 3239
		// (get) Token: 0x060042D9 RID: 17113 RVA: 0x001AF382 File Offset: 0x001AD582
		// (set) Token: 0x060042DA RID: 17114 RVA: 0x001AF38A File Offset: 0x001AD58A
		public bool draw { get; private set; }

		// Token: 0x17000CA8 RID: 3240
		// (get) Token: 0x060042DB RID: 17115 RVA: 0x001AF393 File Offset: 0x001AD593
		// (set) Token: 0x060042DC RID: 17116 RVA: 0x001AF39B File Offset: 0x001AD59B
		public bool bothSidesDestroyed { get; private set; }

		// Token: 0x17000CA9 RID: 3241
		// (get) Token: 0x060042DD RID: 17117 RVA: 0x001AF3A4 File Offset: 0x001AD5A4
		// (set) Token: 0x060042DE RID: 17118 RVA: 0x001AF3AC File Offset: 0x001AD5AC
		public bool oneSideDestroyed { get; private set; }

		// Token: 0x17000CAA RID: 3242
		// (get) Token: 0x060042DF RID: 17119 RVA: 0x001AF3B5 File Offset: 0x001AD5B5
		public override List<TIFactionState> ref_factions
		{
			get
			{
				return this.factions.ToList<TIFactionState>();
			}
		}

		// Token: 0x17000CAB RID: 3243
		// (get) Token: 0x060042E0 RID: 17120 RVA: 0x001AF3C2 File Offset: 0x001AD5C2
		public override TIOrbitState ref_orbit
		{
			get
			{
				if (!(this.hab != null))
				{
					return this.fleets[0].ref_orbit;
				}
				return this.hab.ref_orbit;
			}
		}

		// Token: 0x17000CAC RID: 3244
		// (get) Token: 0x060042E1 RID: 17121 RVA: 0x001AF3EB File Offset: 0x001AD5EB
		public override TIHabState ref_hab
		{
			get
			{
				return this.hab;
			}
		}

		// Token: 0x17000CAD RID: 3245
		// (get) Token: 0x060042E2 RID: 17122 RVA: 0x001AF3F3 File Offset: 0x001AD5F3
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				TIOrbitState ref_orbit = this.ref_orbit;
				if (ref_orbit == null)
				{
					return null;
				}
				return ref_orbit.ref_naturalSpaceObject;
			}
		}

		// Token: 0x17000CAE RID: 3246
		// (get) Token: 0x060042E3 RID: 17123 RVA: 0x001AF406 File Offset: 0x001AD606
		public override TILagrangePointState ref_lagrangePoint
		{
			get
			{
				TIOrbitState ref_orbit = this.ref_orbit;
				if (ref_orbit == null)
				{
					return null;
				}
				return ref_orbit.ref_lagrangePoint;
			}
		}

		// Token: 0x17000CAF RID: 3247
		// (get) Token: 0x060042E4 RID: 17124 RVA: 0x001AF419 File Offset: 0x001AD619
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				TIOrbitState ref_orbit = this.ref_orbit;
				if (ref_orbit == null)
				{
					return null;
				}
				return ref_orbit.ref_spaceBody;
			}
		}

		// Token: 0x060042E5 RID: 17125 RVA: 0x001AF42C File Offset: 0x001AD62C
		public TIFactionState primaryCombatFaction(TIFactionState faction)
		{
			if (!this.factions.Contains(faction))
			{
				return this.factions.First<TIFactionState>((TIFactionState x) => x.permanentAlly(faction));
			}
			return faction;
		}

		// Token: 0x17000CB0 RID: 3248
		// (get) Token: 0x060042E6 RID: 17126 RVA: 0x001AF477 File Offset: 0x001AD677
		public override bool inSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000CB1 RID: 3249
		// (get) Token: 0x060042E7 RID: 17127 RVA: 0x001AF47A File Offset: 0x001AD67A
		public bool HaveStancesBeenSelected
		{
			get
			{
				if (this.stances.Count == 2)
				{
					return this.stances.None<KeyValuePair<TIFactionState, CombatStance>>((KeyValuePair<TIFactionState, CombatStance> x) => x.Value == CombatStance.NotYetSet);
				}
				return false;
			}
		}

		// Token: 0x17000CB2 RID: 3250
		// (get) Token: 0x060042E8 RID: 17128 RVA: 0x001AF4B6 File Offset: 0x001AD6B6
		public bool HaveBidsBeenSubmitted
		{
			get
			{
				return this.bids_kps.Count == 2;
			}
		}

		// Token: 0x060042E9 RID: 17129 RVA: 0x001AF4C8 File Offset: 0x001AD6C8
		public TISpaceFleetState GetFleet(TIFactionState faction)
		{
			if (!(faction != null))
			{
				return null;
			}
			TISpaceAssetState tispaceAssetState = this.assets[faction].FirstOrDefault<TISpaceAssetState>((TISpaceAssetState x) => x.objectType == SpaceObjectType.Fleet);
			return ((tispaceAssetState != null) ? tispaceAssetState.ref_fleet : null) ?? null;
		}

		// Token: 0x17000CB3 RID: 3251
		// (get) Token: 0x060042EA RID: 17130 RVA: 0x001AF521 File Offset: 0x001AD721
		public TISpaceFleetState fleeingFleet
		{
			get
			{
				return this.GetFleet(this.stances.Keys.FirstOrDefault<TIFactionState>((TIFactionState x) => this.stances[x] == CombatStance.Evade));
			}
		}

		// Token: 0x17000CB4 RID: 3252
		// (get) Token: 0x060042EB RID: 17131 RVA: 0x001AF545 File Offset: 0x001AD745
		public TISpaceFleetState chasingFleet
		{
			get
			{
				return this.GetFleet(this.stances.Keys.FirstOrDefault<TIFactionState>((TIFactionState x) => this.stances[x] == CombatStance.Pursue || this.stances[x] == CombatStance.ExtendedPursuit_Stretch || this.stances[x] == CombatStance.ExtendedPursuit_Envelop));
			}
		}

		// Token: 0x17000CB5 RID: 3253
		// (get) Token: 0x060042EC RID: 17132 RVA: 0x001AF569 File Offset: 0x001AD769
		public float LowestPursuitDVBid_kps
		{
			get
			{
				Dictionary<TIFactionState, float>.ValueCollection values = this.bids_kps.Values;
				if (values == null)
				{
					return 0f;
				}
				return values.Min();
			}
		}

		// Token: 0x17000CB6 RID: 3254
		// (get) Token: 0x060042ED RID: 17133 RVA: 0x001AF585 File Offset: 0x001AD785
		// (set) Token: 0x060042EE RID: 17134 RVA: 0x001AF58D File Offset: 0x001AD78D
		public Dictionary<TIFactionState, List<CombatStance>> allowedStances { get; private set; }

		// Token: 0x17000CB7 RID: 3255
		// (get) Token: 0x060042EF RID: 17135 RVA: 0x001AF596 File Offset: 0x001AD796
		// (set) Token: 0x060042F0 RID: 17136 RVA: 0x001AF59E File Offset: 0x001AD79E
		[fsIgnore]
		public bool requiresBidding { get; private set; }

		// Token: 0x17000CB8 RID: 3256
		// (get) Token: 0x060042F1 RID: 17137 RVA: 0x001AF5A7 File Offset: 0x001AD7A7
		// (set) Token: 0x060042F2 RID: 17138 RVA: 0x001AF5AF File Offset: 0x001AD7AF
		[fsIgnore]
		public bool combatOccurs { get; private set; }

		// Token: 0x060042F3 RID: 17139 RVA: 0x001AF5B8 File Offset: 0x001AD7B8
		public TIHabState AlliedHab(CombatWeaponCarrierState combatant)
		{
			if (!(this.hab != null) || !combatant.GetFaction().permanentAlly(this.hab.faction))
			{
				return null;
			}
			return this.hab;
		}

		// Token: 0x060042F4 RID: 17140 RVA: 0x001AF5E8 File Offset: 0x001AD7E8
		public TIHabState AlliedHab(CombatTargetableState combatant)
		{
			if (!(this.hab != null) || !combatant.GetTargetableState().ref_faction.permanentAlly(this.hab.faction))
			{
				return null;
			}
			return this.hab;
		}

		// Token: 0x060042F5 RID: 17141 RVA: 0x001AF61D File Offset: 0x001AD81D
		public TIHabState AlliedHab(TIFactionState faction)
		{
			if (!(this.hab != null) || !faction.permanentAlly(this.hab.faction))
			{
				return null;
			}
			return this.hab;
		}

		// Token: 0x060042F6 RID: 17142 RVA: 0x001AF648 File Offset: 0x001AD848
		public override bool Initialize()
		{
			this.shipWaypoints = new Dictionary<TISpaceShipState, List<TISpaceCombatWaypointState>>();
			this.fleets = new TISpaceFleetState[2];
			this.factions = new TIFactionState[2];
			this.stances = new Dictionary<TIFactionState, CombatStance>();
			this.bids_kps = new Dictionary<TIFactionState, float>();
			this.assets = new Dictionary<TIFactionState, List<TISpaceAssetState>>();
			this.allowedStances = new Dictionary<TIFactionState, List<CombatStance>>();
			this.active = false;
			this.precombatDuration_s = 0.0;
			this.preservedFleetCompositions = new Dictionary<TIFactionState, List<PreservedFleetRecord>>();
			return true;
		}

		// Token: 0x060042F7 RID: 17143 RVA: 0x001AF6C6 File Offset: 0x001AD8C6
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (this.preservedFleetCompositions == null)
			{
				this.preservedFleetCompositions = new Dictionary<TIFactionState, List<PreservedFleetRecord>>();
			}
		}

		// Token: 0x060042F8 RID: 17144 RVA: 0x001AF6DC File Offset: 0x001AD8DC
		public override void PostInitializationInit_4()
		{
			if (this.bids_kps == null)
			{
				this.bids_kps = new Dictionary<TIFactionState, float>();
			}
		}

		// Token: 0x060042F9 RID: 17145 RVA: 0x001AF6FE File Offset: 0x001AD8FE
		public bool STOFighterEligibleCombat(TIFactionState faction)
		{
			TIOrbitState ref_orbit = this.ref_orbit;
			return ref_orbit != null && ref_orbit.isEarthLEO && ((faction.IsAlienFaction && GameStateManager.AlienNation().extant) || TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSTOSquadron));
		}

		// Token: 0x060042FA RID: 17146 RVA: 0x001AF733 File Offset: 0x001AD933
		public bool CanContributeSTOFightersToCombat(TIFactionState faction)
		{
			TIOrbitState ref_orbit = this.ref_orbit;
			return ref_orbit != null && ref_orbit.isEarthLEO && faction.EarthSTOFightersAvailable > 0;
		}

		// Token: 0x060042FB RID: 17147 RVA: 0x001AF754 File Offset: 0x001AD954
		public void AddFightersToCombat(TIFactionState faction, Dictionary<TINationState, PlannedFighters> fighterPlan)
		{
			this.STOFighterPlans[faction] = fighterPlan;
		}

		// Token: 0x060042FC RID: 17148 RVA: 0x001AF764 File Offset: 0x001AD964
		public void CreateFighterGameStates()
		{
			this.fightersInitialized = true;
			using (Dictionary<TIFactionState, Dictionary<TINationState, PlannedFighters>>.KeyCollection.Enumerator enumerator = this.STOFighterPlans.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIFactionState faction = enumerator.Current;
					Func<TISpaceFleetState, bool> <>9__0;
					foreach (TINationState tinationState in this.STOFighterPlans[faction].Keys)
					{
						int count = this.STOFighterPlans[faction][tinationState].count;
						if (count > 0)
						{
							TISpaceShipTemplate fighter = this.STOFighterPlans[faction][tinationState].fighter;
							TemplateManager.Add(fighter, typeof(TISpaceShipTemplate), true);
							List<TISpaceShipState> list = new List<TISpaceShipState>();
							for (int i = 0; i < count; i++)
							{
								TIResourcesCost tiresourcesCost = new TIResourcesCost();
								tiresourcesCost.AddCost(FactionResource.Boost, fighter.wetMass_tons * TemplateManager.global.spaceResourceToTons, true);
								if (tiresourcesCost.CanAfford(faction, 1f, null, float.PositiveInfinity))
								{
									tiresourcesCost.PayCost(faction, "Fighter Launch");
									TISpaceShipState tispaceShipState = (TISpaceShipState)fighter.CreateGameState();
									tispaceShipState.InitWithTemplate(fighter);
									TIGameState tigameState = tispaceShipState;
									string text = "UI.Precombat.FighterCallsign";
									object[] array = new object[2];
									array[0] = fighter.displayName;
									int num = 1;
									int j = i + 1;
									array[num] = j.ToString();
									tigameState.displayName = Loc.T(text, array);
									list.Add(tispaceShipState);
									this.STOFighterPlans[faction][tinationState].AddFighterState(tispaceShipState);
								}
								else
								{
									this.STOFighterPlans[faction][tinationState].count--;
								}
							}
							IEnumerable<TISpaceFleetState> enumerable = this.fleets;
							Func<TISpaceFleetState, bool> func;
							if ((func = <>9__0) == null)
							{
								func = (<>9__0 = delegate(TISpaceFleetState x)
								{
									TIFactionState faction2 = x.faction;
									return faction2 != null && faction2.permanentAlly(faction);
								});
							}
							TISpaceFleetState tispaceFleetState = enumerable.First<TISpaceFleetState>(func);
							TISpaceFleetState tispaceFleetState2 = TISpaceFleetState.CreateAtRunTime(faction, list, this.GetLocation(faction).ref_orbit, tispaceFleetState, null, false, true, null);
							this.dummyFleetStates.Add(tispaceFleetState2);
							tispaceFleetState.AddShipsToFleet(new List<TISpaceShipState>(list), tispaceFleetState2, false, false);
						}
					}
				}
			}
			foreach (TIFactionState tifactionState in this.factions)
			{
				foreach (TISpaceAssetState tispaceAssetState in this.assets[tifactionState].ToList<TISpaceAssetState>())
				{
					if (tispaceAssetState.isSpaceFleetState)
					{
						List<TISpaceShipState> ships = tispaceAssetState.ref_fleet.ships;
						if (ships != null && ships.Count == 0)
						{
							TISpaceFleetState ref_fleet = tispaceAssetState.ref_fleet;
							this.assets[tifactionState].Remove(ref_fleet);
						}
					}
				}
			}
		}

		// Token: 0x060042FD RID: 17149 RVA: 0x001AFAC4 File Offset: 0x001ADCC4
		public void CleanUpFighterGameStates()
		{
			List<TISpaceShipTemplate> list = new List<TISpaceShipTemplate>();
			using (Dictionary<TIFactionState, Dictionary<TINationState, PlannedFighters>>.KeyCollection.Enumerator enumerator = this.STOFighterPlans.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIFactionState faction = enumerator.Current;
					Func<TIFactionState, bool> <>9__1;
					foreach (TINationState tinationState in this.STOFighterPlans[faction].Keys)
					{
						foreach (TISpaceShipState tispaceShipState in this.STOFighterPlans[faction][tinationState].fighterStates)
						{
							if (!TIGameState.Valid(tispaceShipState) || tispaceShipState.ShipDestroyed())
							{
								tinationState.regions.Where<TIRegionState>((TIRegionState x) => x.numSTOFighters > 0).SelectRandomItem<TIRegionState>().numSTOFighters--;
								if (tispaceShipState != null)
								{
									TISpaceShipState tispaceShipState2 = tispaceShipState;
									bool flag = false;
									IEnumerable<TIFactionState> enumerable = this.factions;
									Func<TIFactionState, bool> func;
									if ((func = <>9__1) == null)
									{
										func = (<>9__1 = (TIFactionState x) => !x.permanentAlly(faction));
									}
									tispaceShipState2.DestroyShip(flag, enumerable.FirstOrDefault<TIFactionState>(func));
								}
							}
							else if (tispaceShipState.badlyDamaged)
							{
								tispaceShipState.DestroyShip(false, null);
								tinationState.regions.Where<TIRegionState>((TIRegionState x) => x.numSTOFighters > 0).SelectRandomItem<TIRegionState>().SetSTOFighterOnCooldown(14);
							}
							else
							{
								tispaceShipState.DestroyShip(false, null);
								tinationState.regions.Where<TIRegionState>((TIRegionState x) => x.numSTOFighters > 0).SelectRandomItem<TIRegionState>().SetSTOFighterOnCooldown(7);
							}
						}
						list.AddUnique(this.STOFighterPlans[faction][tinationState].fighter);
					}
				}
			}
			list.ForEach(delegate(TISpaceShipTemplate x)
			{
				TemplateManager.Remove<TISpaceShipTemplate>(x);
			});
		}

		// Token: 0x060042FE RID: 17150 RVA: 0x001AFD60 File Offset: 0x001ADF60
		public void CacheCombatValues()
		{
			this.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x != null).SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).ToList<TISpaceShipState>()
				.ForEach(delegate(TISpaceShipState ship)
				{
					this.maxDeltaVAvailableForCombat_kps.Add(ship, ship.AvailableDeltaVForCombat_kps());
				});
			foreach (TISpaceFleetState tispaceFleetState in this.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x != null))
			{
				Dictionary<TIFactionState, float> dictionary = this.initialFactionFleetStrengths;
				TIFactionState faction = tispaceFleetState.faction;
				dictionary[faction] += tispaceFleetState.SpaceCombatValue();
			}
			float num = this.assets[this.factions[0]].Sum<TISpaceAssetState>((TISpaceAssetState x) => x.SpaceCombatValue());
			float num2 = this.assets[this.factions[0]].Sum<TISpaceAssetState>((TISpaceAssetState x) => x.SpaceCombatValue()) + this.assets[this.factions[1]].Sum<TISpaceAssetState>((TISpaceAssetState x) => x.SpaceCombatValue());
			this.combatBalance = ((num2 == 0f) ? 10f : (num / num2));
		}

		// Token: 0x060042FF RID: 17151 RVA: 0x001AFF18 File Offset: 0x001AE118
		public void SetRequiresBidding()
		{
			if (!this.HaveStancesBeenSelected)
			{
				return;
			}
			this.CreateFighterGameStates();
			this.CacheCombatValues();
			CombatStance[] array = this.stances.Values.ToArray<CombatStance>();
			this.requiresBidding = (array[0] == CombatStance.Pursue && array[1] == CombatStance.Evade) || (array[0] == CombatStance.Evade && array[1] == CombatStance.Pursue);
		}

		// Token: 0x06004300 RID: 17152 RVA: 0x001AFF6F File Offset: 0x001AE16F
		public static bool OnTieBidDoesTheFirstFleetWin(TISpaceFleetState fleet0, TISpaceFleetState fleet1)
		{
			return TISpaceCombatState.OnTieBidDoesTheFirstFleetWin(fleet0.pursuitAcceleration_mps2, fleet0.availableDeltaVforPrecombat_mps, fleet1.pursuitAcceleration_mps2, fleet1.availableDeltaVforPrecombat_mps, TISpaceCombatState.GetPursuitDistance_m(fleet0, fleet1));
		}

		// Token: 0x06004301 RID: 17153 RVA: 0x001AFF98 File Offset: 0x001AE198
		public static bool OnTieBidDoesTheFirstFleetWin(float fleet0_accel_mps2, float fleet0_DV_mps, float fleet1_accel_mps2, float fleet1_DV_mps, float distancePursuitCovers_m)
		{
			float num = Mathf.Min(fleet0_DV_mps, fleet1_DV_mps);
			float num2 = TISpaceCombatState.MaxDVBidForPursuit_mps(fleet0_accel_mps2, fleet0_DV_mps, fleet1_accel_mps2, fleet1_DV_mps, distancePursuitCovers_m);
			if (num <= num2)
			{
				return fleet0_DV_mps > fleet1_DV_mps;
			}
			return fleet0_accel_mps2 > fleet1_accel_mps2;
		}

		// Token: 0x06004302 RID: 17154 RVA: 0x001AFFC4 File Offset: 0x001AE1C4
		public static List<TISpaceShipState> PursuerSubsetThatCanCatchEnemyFleet(TISpaceFleetState pursuingFleet, TISpaceFleetState fleeingFleet, out bool envelopment)
		{
			List<TISpaceShipState> shipsCanCatch = new List<TISpaceShipState>();
			float pursuitDistance_m = TISpaceCombatState.GetPursuitDistance_m(pursuingFleet, fleeingFleet);
			float pursuitAcceleration_mps = fleeingFleet.pursuitAcceleration_mps2;
			float availableDeltaVforPrecombat_mps = fleeingFleet.availableDeltaVforPrecombat_mps;
			foreach (TISpaceShipState tispaceShipState in pursuingFleet.ships)
			{
				if (TISpaceCombatState.OnTieBidDoesTheFirstFleetWin(tispaceShipState.pursuitAcceleration_mps2, tispaceShipState.AvailableDeltaVForCombat_kps(), pursuitAcceleration_mps, availableDeltaVforPrecombat_mps, pursuitDistance_m))
				{
					shipsCanCatch.Add(tispaceShipState);
				}
			}
			float num = Mathf.Min(pursuitAcceleration_mps, pursuingFleet.ships.Max<TISpaceShipState>((TISpaceShipState x) => x.combatAcceleration_mps2));
			Dictionary<int, List<TISpaceShipState>> dictionary = new Dictionary<int, List<TISpaceShipState>>();
			if (num > 0f)
			{
				int num2 = 0;
				using (IEnumerator<float> enumerator2 = pursuingFleet.ships.Select<TISpaceShipState, float>((TISpaceShipState x) => x.combatAcceleration_mps2).Distinct<float>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						float accelerationTesting_mps = enumerator2.Current;
						List<TISpaceShipState> list = pursuingFleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.combatAcceleration_mps2 >= accelerationTesting_mps && x.AvailableDeltaVForCombat_kps() > 0f).ToList<TISpaceShipState>();
						if (list.Count > 0)
						{
							float num3 = TISpaceCombatState.DVBurnToEnvelop_kps(list, pursuingFleet, fleeingFleet);
							foreach (TISpaceShipState tispaceShipState2 in list.ToList<TISpaceShipState>())
							{
								if (tispaceShipState2.AvailableDeltaVForCombat_kps() <= num3)
								{
									list.Remove(tispaceShipState2);
								}
							}
							dictionary.Add(num2, new List<TISpaceShipState>());
							if (list.Count > 0)
							{
								float num4 = accelerationTesting_mps / pursuitAcceleration_mps;
								ulong num5 = (ulong)Mathf.Max(3f, 1f + Mathf.Round(Mathf.Pow(1f / num4, 3.1415927f)));
								if (num5 < 2147483647UL)
								{
									int num6 = (int)num5;
									if (list.Count / num6 >= 1)
									{
										list = list.OrderByDescending<TISpaceShipState, float>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f)).ToList<TISpaceShipState>();
										Dictionary<int, List<TISpaceShipState>> dictionary2 = new Dictionary<int, List<TISpaceShipState>>();
										for (int i = 0; i < num6; i++)
										{
											dictionary2.Add(i, new List<TISpaceShipState>());
										}
										int num7 = 0;
										bool flag = false;
										foreach (TISpaceShipState tispaceShipState3 in list.ToList<TISpaceShipState>())
										{
											dictionary2[num7].Add(tispaceShipState3);
											if (flag)
											{
												num7--;
												if (num7 < 0)
												{
													num7 = 0;
													flag = false;
												}
											}
											else
											{
												num7++;
												if (num7 >= num6)
												{
													num7 = num6 - 1;
													flag = true;
												}
											}
										}
										dictionary[num2].AddRange(dictionary2.Values.SelectRandomItem<List<TISpaceShipState>>());
									}
								}
							}
						}
						num2++;
					}
				}
			}
			List<TISpaceShipState> list2 = new List<TISpaceShipState>();
			if (dictionary.Values.Any<List<TISpaceShipState>>((List<TISpaceShipState> x) => x.Count > 0))
			{
				list2 = dictionary.Values.MaxBy<List<TISpaceShipState>, float>((List<TISpaceShipState> x) => x.Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f)));
				if (shipsCanCatch.Count == 0 || (list2.Count > 0 && list2.Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, TISpaceCombatState.ExtendedDVBurn_kps(shipsCanCatch, pursuingFleet, fleeingFleet, true))) > shipsCanCatch.Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, TISpaceCombatState.ExtendedDVBurn_kps(shipsCanCatch, pursuingFleet, fleeingFleet, false)))))
				{
					envelopment = true;
					return list2;
				}
			}
			envelopment = false;
			return shipsCanCatch;
		}

		// Token: 0x06004303 RID: 17155 RVA: 0x001B042C File Offset: 0x001AE62C
		public void RemoveShipsFromBattleInExtendedPursuit(List<TISpaceShipState> shipsStayingInBattle, TISpaceFleetState battleFleet)
		{
			this.newFleetsCreatedByExtendedPursuit = new List<TISpaceFleetState>();
			Dictionary<TIFactionState, List<TISpaceShipState>> dictionary = new Dictionary<TIFactionState, List<TISpaceShipState>>();
			using (List<TISpaceShipState>.Enumerator enumerator = battleFleet.ships.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TISpaceShipState ship = enumerator.Current;
					if (!shipsStayingInBattle.Contains(ship))
					{
						TIFactionState tifactionState = null;
						if (this.preservedFleetCompositions.Count > 0)
						{
							Func<PreservedFleetRecord, bool> <>9__1;
							tifactionState = this.preservedFleetCompositions.Keys.FirstOrDefault<TIFactionState>(delegate(TIFactionState x)
							{
								IEnumerable<PreservedFleetRecord> enumerable = this.preservedFleetCompositions[x];
								Func<PreservedFleetRecord, bool> func;
								if ((func = <>9__1) == null)
								{
									func = (<>9__1 = (PreservedFleetRecord x) => x.ships.Contains(ship));
								}
								return enumerable.Any<PreservedFleetRecord>(func);
							});
						}
						if (tifactionState == null)
						{
							tifactionState = ship.faction;
						}
						if (!dictionary.ContainsKey(tifactionState))
						{
							dictionary.Add(tifactionState, new List<TISpaceShipState>());
						}
						dictionary[tifactionState].Add(ship);
					}
				}
			}
			foreach (TIFactionState tifactionState2 in dictionary.Keys)
			{
				this.newFleetsCreatedByExtendedPursuit.Add(TISpaceFleetState.CreateAtRunTime(tifactionState2, dictionary[tifactionState2], this.GetLocation(tifactionState2), battleFleet, null, false, false, null));
			}
		}

		// Token: 0x06004304 RID: 17156 RVA: 0x001B057C File Offset: 0x001AE77C
		private bool Approximately(float a, float b)
		{
			return Mathf.Abs(a - b) < 1E-05f;
		}

		// Token: 0x06004305 RID: 17157 RVA: 0x001B0590 File Offset: 0x001AE790
		public void SetRequiresCombat()
		{
			if (!this.HaveStancesBeenSelected)
			{
				return;
			}
			if (this.requiresBidding && !this.HaveBidsBeenSubmitted)
			{
				return;
			}
			if (this.requiresBidding)
			{
				if (this.stances.Values.Any<CombatStance>((CombatStance x) => x == CombatStance.ExtendedPursuit_Envelop || x == CombatStance.ExtendedPursuit_Stretch))
				{
					this.combatOccurs = true;
					return;
				}
				if (this.stances[this.factions[0]] == CombatStance.Pursue)
				{
					if (this.Approximately(this.bids_kps[this.factions[0]], this.bids_kps[this.factions[1]]) && this.Approximately(this.bids_kps[this.factions[0]], this.MaxDVBidForPursuit_mps(this.FleetFor(this.factions[0]), this.FleetFor(this.factions[1])) / 1000f))
					{
						this.combatOccurs = TISpaceCombatState.OnTieBidDoesTheFirstFleetWin(this.FleetFor(this.factions[0]), this.FleetFor(this.factions[1]));
						return;
					}
					this.combatOccurs = this.bids_kps[this.factions[0]] > this.bids_kps[this.factions[1]];
					return;
				}
				else if (this.stances[this.factions[1]] == CombatStance.Pursue)
				{
					if (this.Approximately(this.bids_kps[this.factions[0]], this.bids_kps[this.factions[1]]) && this.Approximately(this.bids_kps[this.factions[0]], this.MaxDVBidForPursuit_mps(this.FleetFor(this.factions[0]), this.FleetFor(this.factions[1])) / 1000f))
					{
						this.combatOccurs = TISpaceCombatState.OnTieBidDoesTheFirstFleetWin(this.FleetFor(this.factions[1]), this.FleetFor(this.factions[0]));
						return;
					}
					this.combatOccurs = this.bids_kps[this.factions[1]] > this.bids_kps[this.factions[0]];
					return;
				}
			}
			else
			{
				this.combatOccurs = this.stances[this.factions[0]] == CombatStance.Pursue || this.stances[this.factions[1]] == CombatStance.Pursue;
			}
		}

		// Token: 0x06004306 RID: 17158 RVA: 0x001B07F4 File Offset: 0x001AE9F4
		public bool IncludesFaction(TIFactionState faction)
		{
			return this.factions.Any<TIFactionState>((TIFactionState x) => x == faction);
		}

		// Token: 0x06004307 RID: 17159 RVA: 0x001B0828 File Offset: 0x001AEA28
		public TISpaceFleetState FleetFor(TIFactionState faction)
		{
			return this.fleets.SingleOrDefault<TISpaceFleetState>((TISpaceFleetState fleet) => ((fleet != null) ? fleet.faction : null) == faction);
		}

		// Token: 0x06004308 RID: 17160 RVA: 0x001B085C File Offset: 0x001AEA5C
		public TISpaceFleetState FleetAgainst(TIFactionState faction)
		{
			return this.fleets.SingleOrDefault<TISpaceFleetState>((TISpaceFleetState fleet) => ((fleet != null) ? fleet.faction : null) != faction);
		}

		// Token: 0x17000CB9 RID: 3257
		// (get) Token: 0x06004309 RID: 17161 RVA: 0x001B088D File Offset: 0x001AEA8D
		// (set) Token: 0x0600430A RID: 17162 RVA: 0x001B0895 File Offset: 0x001AEA95
		public TISpaceFleetState cachedFleet1 { get; private set; }

		// Token: 0x17000CBA RID: 3258
		// (get) Token: 0x0600430B RID: 17163 RVA: 0x001B089E File Offset: 0x001AEA9E
		// (set) Token: 0x0600430C RID: 17164 RVA: 0x001B08A6 File Offset: 0x001AEAA6
		public TISpaceFleetState cachedFleet2 { get; private set; }

		// Token: 0x17000CBB RID: 3259
		// (get) Token: 0x0600430D RID: 17165 RVA: 0x001B08AF File Offset: 0x001AEAAF
		// (set) Token: 0x0600430E RID: 17166 RVA: 0x001B08B7 File Offset: 0x001AEAB7
		public TIHabState cachedHab { get; private set; }

		// Token: 0x0600430F RID: 17167 RVA: 0x001B08C0 File Offset: 0x001AEAC0
		public void CacheCombatAssets(TISpaceFleetState fleet1, TISpaceFleetState fleet2, TIHabState hab)
		{
			this.cachedFleet1 = fleet1;
			this.cachedFleet2 = fleet2;
			this.cachedHab = hab;
			this.active = false;
			if (GameStateManager.GetAllGameStates<TISpaceCombatState>(true).All<TISpaceCombatState>((TISpaceCombatState x) => !x.active) && !TIPromptQueueState.ActivePlayerHasSaveBlockingPrompt() && (fleet1.faction == GameControl.control.activePlayer || (fleet2 != null && fleet2.faction == GameControl.control.activePlayer) || (hab != null && hab.faction == GameControl.control.activePlayer && hab.ActiveCombatModules().Count > 0)))
			{
				GameStateManager.SaveAllGameStates(StartMenuController.combatAutoSaveFilepath, false);
			}
		}

		// Token: 0x06004310 RID: 17168 RVA: 0x001B0990 File Offset: 0x001AEB90
		public void StartCombatFromStrategyLayer()
		{
			bool flag = false;
			bool flag2 = TIGameState.Valid(this.cachedFleet1) && (this.cachedFleet1.ships.Count > 0 || this.allowNoAttackingFleetAtInitialization);
			bool flag3 = TIGameState.Valid(this.cachedFleet2) && this.cachedFleet2.ships.Count > 0;
			bool flag4 = TIGameState.Valid(this.cachedHab);
			if (!flag4)
			{
				this.cachedHab = null;
			}
			if (flag2 && (flag3 || flag4))
			{
				List<TIFactionState> list = new List<TIFactionState> { this.cachedFleet1.faction };
				if (flag3)
				{
					list.AddUnique(this.cachedFleet2.faction);
				}
				else if (flag4)
				{
					list.AddUnique(this.cachedHab.coreFaction);
					if (this.cachedHab.ref_orbit.isEarthLEO && this.cachedHab.coreFaction.EarthSTOFightersAvailable > 0)
					{
						TISpaceFleetState dummyFleet = TISpaceFleetState.CreateAtRunTime(this.cachedHab.ref_faction, new List<TISpaceShipState>(), this.cachedHab.ref_orbit, null, null, false, true, null);
						dummyFleet.dummyFleet = true;
						this.dummyFleetStates.Add(dummyFleet);
						GameStateManager.AllFactions().ForEach<TIFactionState>(delegate(TIFactionState x)
						{
							dummyFleet.ForceDisplayName(x, Loc.T("LaunchSTOInterceptorsOperation.dummyFleetName", new object[] { this.cachedHab.ref_faction.adjective }));
						});
						this.cachedFleet2 = dummyFleet;
					}
				}
				if (list.None<TIFactionState>((TIFactionState x) => x == null) && list.Count == 2)
				{
					flag = true;
					for (int i = 0; i < list.Count; i++)
					{
						TIFactionState tifactionState = list[i];
						if (tifactionState != null && tifactionState.isActivePlayer)
						{
							Mood.TriggerEvent(Mood.Event.SDKL_Alarm);
							break;
						}
					}
				}
			}
			if (!flag)
			{
				if (this.cachedFleet1 != null)
				{
					this.cachedFleet1.inCombat = false;
					this.cachedFleet1.combatState = null;
					if (this.cachedFleet1.dummyFleet)
					{
						this.cachedFleet1.RemoveShipsFromFleet(this.cachedFleet1.ships, null);
					}
				}
				if (this.cachedFleet2 != null)
				{
					this.cachedFleet2.inCombat = false;
					this.cachedFleet2.combatState = null;
					if (this.cachedFleet2.dummyFleet)
					{
						this.cachedFleet2.RemoveShipsFromFleet(this.cachedFleet2.ships, null);
					}
				}
				TIHabState cachedHab = this.cachedHab;
				if (cachedHab != null && cachedHab.inCombat)
				{
					this.cachedHab.inCombat = false;
				}
				base.ArchiveState(true);
				GameStateManager.RemoveGameState<TISpaceCombatState>(base.ID, false);
				GameControl.spaceCombat.SetCombat(null);
				return;
			}
			List<TISpaceShipState> list2 = new List<TISpaceShipState>();
			if (this.cachedFleet1 != null)
			{
				list2.AddRange(this.cachedFleet1.ships);
			}
			if (this.cachedFleet2 != null)
			{
				list2.AddRange(this.cachedFleet2.ships);
			}
			this.combatLog = new TIFactionState.CombatLog(list2, this.cachedHab);
			List<TISpaceCombatState> list3 = new List<TISpaceCombatState>();
			foreach (TISpaceCombatState tispaceCombatState in from x in GameStateManager.IterateByClass<TISpaceCombatState>(false)
				where x != this
				select x)
			{
				if (tispaceCombatState.cachedFleet1 == this.cachedFleet1 && tispaceCombatState.cachedFleet2 == this.cachedFleet2 && tispaceCombatState.cachedHab == this.cachedHab && !tispaceCombatState.archived && !tispaceCombatState.active)
				{
					Log.Error("Duplicate combatState setup found. Removing. Gameplay should continue but please report circumstances of save/load.", Array.Empty<object>());
					list3.Add(tispaceCombatState);
				}
				if (!tispaceCombatState.cachedFleet1.archived)
				{
					TISpaceFleetState cachedFleet = tispaceCombatState.cachedFleet2;
					if (cachedFleet == null || !cachedFleet.archived)
					{
						TIHabState cachedHab2 = tispaceCombatState.cachedHab;
						if (cachedHab2 == null || !cachedHab2.archived)
						{
							continue;
						}
					}
				}
				list3.Add(tispaceCombatState);
			}
			foreach (TISpaceCombatState tispaceCombatState2 in list3)
			{
				tispaceCombatState2.ArchiveState(true);
				GameStateManager.RemoveGameState<TISpaceCombatState>(tispaceCombatState2.ID, false);
			}
			this.active = this.InitializeCombat(this.cachedFleet1, this.cachedFleet2, this.cachedHab);
			TISpaceFleetState cachedFleet2 = this.cachedFleet1;
			if (cachedFleet2 != null)
			{
				cachedFleet2.AddFleetLog("PreCombat");
			}
			TISpaceFleetState cachedFleet3 = this.cachedFleet2;
			if (cachedFleet3 == null)
			{
				return;
			}
			cachedFleet3.AddFleetLog("PreCombat");
		}

		// Token: 0x06004311 RID: 17169 RVA: 0x001B0E34 File Offset: 0x001AF034
		public bool InitializeCombat(TISpaceFleetState fleet1, TISpaceFleetState fleet2, TIHabState hab)
		{
			if (this.initialized)
			{
				Log.Error("Combat already initialized", Array.Empty<object>());
			}
			if (this.assets.Count > 0)
			{
				Log.Error("Combat has assets set already.", Array.Empty<object>());
				this.Initialize();
			}
			this.precombatDuration_s = 0.0;
			this.fleets[0] = fleet1;
			this.factions[0] = fleet1.faction;
			this.assets.Add(this.factions[0], new List<TISpaceAssetState> { this.fleets[0] });
			this.fleets[1] = fleet2;
			this.hab = hab;
			if ((fleet2 != null && (fleet2.ships == null || (fleet2.ships.Count < 1 && !fleet2.dummyFleet))) || (fleet1.ships.Count < 1 && !this.allowNoAttackingFleetAtInitialization))
			{
				Log.Warn("canceling combat, invalid fleetstate with no ships", Array.Empty<object>());
				base.ArchiveState(true);
				GameStateManager.RemoveGameState<TISpaceCombatState>(base.ID, false);
				return false;
			}
			if (fleet2 != null)
			{
				this.factions[1] = fleet2.faction;
			}
			else
			{
				this.factions[1] = hab.ref_faction;
			}
			this.assets.Add(this.factions[1], new List<TISpaceAssetState>());
			if (fleet2 != null)
			{
				this.assets[this.factions[1]].Add(fleet2);
			}
			if (hab != null)
			{
				hab.inCombat = true;
				if (GameControl.control.skirmishMode)
				{
					this.assets[this.factions[(hab.faction == this.factions[0]) ? 0 : 1]].Add(hab);
				}
				else
				{
					this.assets[this.factions[1]].Add(hab);
				}
			}
			this.fleets[0].inCombat = true;
			this.fleets[0].combatState = this;
			if (this.fleets[1] != null)
			{
				this.fleets[1].inCombat = true;
				this.fleets[1].combatState = this;
			}
			this.allowedStances.Add(this.factions[0], new List<CombatStance> { CombatStance.Defend });
			this.combatOccurs = GameControl.control.skirmishMode;
			this.requiresBidding = false;
			if (this.allowNoAttackingFleetAtInitialization)
			{
				this.allowedStances[this.factions[0]].Add(CombatStance.Pursue);
			}
			else if (this.fleets[0].pursuitAcceleration_mps2 > 0f && this.fleets[0].allShipsHaveDeltaV)
			{
				this.allowedStances[this.factions[0]].Add(CombatStance.Pursue);
				this.allowedStances[this.factions[0]].Add(CombatStance.Evade);
			}
			this.allowedStances.Add(this.factions[1], new List<CombatStance> { CombatStance.Defend });
			if (hab == null && !this.fleets[1].MustAcceptCombatAsDefender())
			{
				this.allowedStances[this.factions[1]].Add(CombatStance.Pursue);
				this.allowedStances[this.factions[1]].Add(CombatStance.Evade);
			}
			else if (hab != null)
			{
				TISpaceFleetState tispaceFleetState = this.fleets[1];
				bool flag;
				if (tispaceFleetState == null)
				{
					flag = false;
				}
				else
				{
					List<TISpaceShipState> ships = tispaceFleetState.ships;
					int? num = ((ships != null) ? new int?(ships.Count) : null);
					int num2 = 0;
					flag = (num.GetValueOrDefault() > num2) & (num != null);
				}
				if (flag)
				{
					this.allowedStances[this.factions[1]].AddUnique(CombatStance.Pursue);
				}
			}
			this.autoresolve = !GameControl.control.skirmishMode;
			this.combatStartDateTime = TITimeState.Now();
			if (hab != null)
			{
				this.displayName = Loc.T("UI.SpaceCombat.BattleName", new object[] { hab.GetDisplayName(GameControl.control.activePlayer) });
				this.nearbyNaturalSpaceObject = hab.ref_naturalSpaceObject;
			}
			else
			{
				TINaturalSpaceObjectState sphereOfInfluence = this.fleets[0].GetSphereOfInfluence(true);
				this.nearbyNaturalSpaceObject = sphereOfInfluence;
				if (sphereOfInfluence.isSun)
				{
					this.displayName = Loc.T("UI.SpaceCombat.BattleNameDeepSpace");
				}
				else
				{
					this.displayName = Loc.T("UI.SpaceCombat.BattleName", new object[] { sphereOfInfluence.displayName });
				}
			}
			this.combatGlobalPosition = this.fleets[0].ToGlobalCartesianStateAtTime(this.combatStartDateTime);
			this.combatRecord = new CombatRecord
			{
				combatName = this.displayName,
				fleet1Name = this.fleets[0].GetDisplayName(GameControl.control.activePlayer),
				faction1 = this.fleets[0].faction,
				habName = (((hab != null) ? hab.displayName : null) ?? "No Hab")
			};
			if (fleet2 != null)
			{
				this.combatRecord.fleet2Name = this.fleets[1].GetDisplayName(GameControl.control.activePlayer);
				this.combatRecord.faction2 = this.fleets[1].faction;
			}
			else
			{
				this.combatRecord.fleet2Name = hab.GetDisplayName(GameControl.control.activePlayer);
				this.combatRecord.faction2 = hab.faction;
			}
			this.maxDeltaVAvailableForCombat_kps = new Dictionary<TISpaceShipState, float>();
			this.initialFactionFleetStrengths = this.factions.Where<TIFactionState>((TIFactionState x) => x != null).ToDictionary<TIFactionState, TIFactionState, float>((TIFactionState x) => x, (TIFactionState x) => 0f);
			this.initialHabStrength = ((hab != null) ? hab.SpaceCombatValue() : 0f);
			GameControl.eventManager.TriggerEvent(new SpaceCombatInitiated(this), null, Array.Empty<object>());
			if (hab != null)
			{
				GameControl.eventManager.TriggerEvent(new HabEntersCombat(this, hab), null, new object[] { hab });
			}
			if (GameControl.control.skirmishMode)
			{
				this.CacheCombatValues();
			}
			this.initialized = true;
			return true;
		}

		// Token: 0x06004312 RID: 17170 RVA: 0x001B1456 File Offset: 0x001AF656
		public void UpdateMaxDeltaVForShip(TISpaceShipState ship)
		{
			if (this.maxDeltaVAvailableForCombat_kps.ContainsKey(ship))
			{
				this.maxDeltaVAvailableForCombat_kps[ship] = ship.AvailableDeltaVForCombat_kps();
				GameControl.eventManager.TriggerEvent(new ShipDeltaVChange(ship), null, new object[] { this });
			}
		}

		// Token: 0x17000CBC RID: 3260
		// (get) Token: 0x06004313 RID: 17171 RVA: 0x001B1493 File Offset: 0x001AF693
		public static bool UseFixedPursuitDistance
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06004314 RID: 17172 RVA: 0x001B1496 File Offset: 0x001AF696
		public static float GetPursuitDistance_m(TISpaceFleetState fleet1, TISpaceFleetState fleet2)
		{
			if (!TISpaceCombatState.UseFixedPursuitDistance)
			{
				return Mathf.Max(fleet1.CombatRange_km(), fleet2.CombatRange_km()) * 1000f * 200f;
			}
			return -1f;
		}

		// Token: 0x06004315 RID: 17173 RVA: 0x001B14C4 File Offset: 0x001AF6C4
		public float MaxDVBidForPursuit_mps(TISpaceFleetState myFleet, TISpaceFleetState otherFleet)
		{
			float pursuitDistance_m = TISpaceCombatState.GetPursuitDistance_m(myFleet, otherFleet);
			return TISpaceCombatState.MaxDVBidForPursuit_mps(myFleet.pursuitAcceleration_mps2, myFleet.availableDeltaVforPrecombat_kps * 1000f, otherFleet.pursuitAcceleration_mps2, otherFleet.availableDeltaVforPrecombat_kps * 1000f, pursuitDistance_m);
		}

		// Token: 0x06004316 RID: 17174 RVA: 0x001B1504 File Offset: 0x001AF704
		protected static float MaxDVBidForPursuit_mps(float fleet0_accel_mps2, float fleet0_DV_mps, float fleet1_accel_mps2, float fleet1_DV_mps, float distancePursuitCovers_m)
		{
			float num = Mathf.Min(fleet0_accel_mps2, fleet1_accel_mps2);
			float num2 = Mathf.Max(fleet0_accel_mps2, fleet1_accel_mps2);
			float num3 = num2 - num;
			float num4 = Mathf.Sqrt(2f * distancePursuitCovers_m * num / (num3 * (num3 + num))) * num2;
			if (float.IsNaN(num4))
			{
				return Mathf.Min(fleet0_DV_mps, fleet1_DV_mps);
			}
			return Mathf.Clamp(num4, 0f, Mathf.Min(fleet0_DV_mps, fleet1_DV_mps));
		}

		// Token: 0x06004317 RID: 17175 RVA: 0x001B1560 File Offset: 0x001AF760
		public static float ExtendedDVBurn_kps(List<TISpaceShipState> extendingShips, TISpaceFleetState chasingFleet, TISpaceFleetState fleeingFleet, bool envelop)
		{
			float num = extendingShips.Min<TISpaceShipState>((TISpaceShipState x) => x.pursuitAcceleration_mps2);
			float num2 = extendingShips.Min<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() / 1000f);
			float num3 = fleeingFleet.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.pursuitAcceleration_mps2);
			float num4 = fleeingFleet.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() / 1000f);
			float pursuitDistance_m = TISpaceCombatState.GetPursuitDistance_m(chasingFleet, fleeingFleet);
			float num5 = TISpaceCombatState.MaxDVBidForPursuit_mps(num, num2, num3, num4, pursuitDistance_m) / 1000f;
			if (envelop)
			{
				return TISpaceCombatState.DVBurnToEnvelop_kps(extendingShips, chasingFleet, fleeingFleet);
			}
			return num5;
		}

		// Token: 0x06004318 RID: 17176 RVA: 0x001B1638 File Offset: 0x001AF838
		public static float DVBurnToEnvelop_kps(List<TISpaceShipState> envelopingShips, TISpaceFleetState chasingFleet, TISpaceFleetState fleeingFleet)
		{
			float num = envelopingShips.Min<TISpaceShipState>((TISpaceShipState x) => x.pursuitAcceleration_mps2);
			float num2 = fleeingFleet.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.pursuitAcceleration_mps2);
			float num3 = Mathf.Min(num, num2);
			float num4 = Mathf.Max(num, num2);
			float pursuitDistance_m = TISpaceCombatState.GetPursuitDistance_m(chasingFleet, fleeingFleet);
			float num5 = Mathf.Max(num4 - num3, 1f);
			float num6 = Mathf.Min(0.5f * num4 * Mathf.Sqrt(pursuitDistance_m * num3 / (num5 * (num5 + num3))), fleeingFleet.availableDeltaVforPrecombat_mps);
			if (num > num2)
			{
				num6 *= 0.25f;
			}
			return num6 / 1000f;
		}

		// Token: 0x06004319 RID: 17177 RVA: 0x001B16F6 File Offset: 0x001AF8F6
		public float PrecombatDVSpend_kps(TISpaceShipState ship, TISpaceFleetState chasingFleet, TISpaceFleetState fleeingFleet, TISpaceFleetState extendingFleet)
		{
			return this.PrecombatDVSpend_kps(ship, chasingFleet, fleeingFleet, ((extendingFleet != null) ? extendingFleet.ships : null) ?? null);
		}

		// Token: 0x0600431A RID: 17178 RVA: 0x001B1714 File Offset: 0x001AF914
		public float PrecombatDVSpend_kps(TISpaceShipState ship, TISpaceFleetState chasingFleet, TISpaceFleetState fleeingFleet, List<TISpaceShipState> extendingFleet)
		{
			if (!this.requiresBidding)
			{
				return 0f;
			}
			if (!this.combatOccurs)
			{
				return this.LowestPursuitDVBid_kps;
			}
			CombatStance combatStance = this.stances[chasingFleet.faction];
			if (combatStance != CombatStance.ExtendedPursuit_Envelop)
			{
				if (combatStance != CombatStance.ExtendedPursuit_Stretch)
				{
					return this.LowestPursuitDVBid_kps;
				}
				if (fleeingFleet.ships.Contains(ship))
				{
					return this.LowestPursuitDVBid_kps;
				}
				if (extendingFleet.Contains(ship))
				{
					return TISpaceCombatState.ExtendedDVBurn_kps(extendingFleet, chasingFleet, fleeingFleet, false);
				}
				this.newFleetsCreatedByExtendedPursuit.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).Contains(ship);
				return this.LowestPursuitDVBid_kps;
			}
			else if (fleeingFleet.ships.Contains(ship))
			{
				if (TISpaceCombatState.ExtendedDVBurn_kps(extendingFleet, chasingFleet, fleeingFleet, true) == 0f)
				{
					return 0f;
				}
				return Mathf.Min(this.LowestPursuitDVBid_kps * 0.25f, TISpaceCombatState.ExtendedDVBurn_kps(extendingFleet, chasingFleet, fleeingFleet, true));
			}
			else
			{
				if (extendingFleet.Contains(ship))
				{
					return TISpaceCombatState.ExtendedDVBurn_kps(extendingFleet, chasingFleet, fleeingFleet, true);
				}
				if (this.newFleetsCreatedByExtendedPursuit.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).Contains(ship))
				{
					return Mathf.Min(this.LowestPursuitDVBid_kps * 0.25f, TISpaceCombatState.ExtendedDVBurn_kps(extendingFleet, chasingFleet, fleeingFleet, true));
				}
				return this.LowestPursuitDVBid_kps;
			}
		}

		// Token: 0x0600431B RID: 17179 RVA: 0x001B1874 File Offset: 0x001AFA74
		public void HandlePrecombat()
		{
			TISpaceFleetState tispaceFleetState = null;
			if (this.requiresBidding && this.combatOccurs && (this.stances[this.chasingFleet.faction] == CombatStance.ExtendedPursuit_Stretch || this.stances[this.chasingFleet.faction] == CombatStance.ExtendedPursuit_Envelop))
			{
				tispaceFleetState = this.chasingFleet;
			}
			List<TISpaceFleetState> list = new List<TISpaceFleetState>(this.fleets);
			list.AddRangeUnique<TISpaceFleetState>(this.newFleetsCreatedByExtendedPursuit);
			list = list.Where<TISpaceFleetState>((TISpaceFleetState x) => TIGameState.Valid(x)).ToList<TISpaceFleetState>();
			foreach (TISpaceFleetState tispaceFleetState2 in list)
			{
				if (tispaceFleetState2.ships.Count > 0)
				{
					float num = this.PrecombatDVSpend_kps(tispaceFleetState2.ships[0], this.requiresBidding ? this.chasingFleet : null, this.requiresBidding ? this.fleeingFleet : null, tispaceFleetState);
					if (num > 0f)
					{
						foreach (TISpaceShipState tispaceShipState in tispaceFleetState2.ships)
						{
							tispaceShipState.ConsumeDeltaV(num, true);
						}
					}
				}
			}
			if (this.requiresBidding)
			{
				int num2 = 3600;
				float num3 = this.fleets.Min<TISpaceFleetState>((TISpaceFleetState x) => x.pursuitAcceleration_mps2);
				if (this.LowestPursuitDVBid_kps > 0f && num3 > 0f)
				{
					if (Mathf.Approximately(this.fleets[0].pursuitAcceleration_mps2, this.fleets[1].pursuitAcceleration_mps2))
					{
						this.SetPrecombatDuration((double)Mathf.Max(this.LowestPursuitDVBid_kps / num3, (float)num2));
					}
					else
					{
						float num4 = Mathf.Sqrt(2f * TISpaceCombatState.GetPursuitDistance_m(this.fleets[0], this.fleets[1]) / (Mathf.Max(this.fleets[0].pursuitAcceleration_mps2, this.fleets[1].pursuitAcceleration_mps2) - Mathf.Min(this.fleets[0].pursuitAcceleration_mps2, this.fleets[1].pursuitAcceleration_mps2)));
						this.SetPrecombatDuration((double)Mathf.Max((float)num2, num4));
					}
				}
				else
				{
					this.SetPrecombatDuration((double)num2);
				}
			}
			if ((this.requiresBidding && this.LowestPursuitDVBid_kps > 0f) || this.combatOccurs)
			{
				foreach (TISpaceFleetState tispaceFleetState3 in list)
				{
					List<OperationData> list2 = tispaceFleetState3.CurrentOperations();
					List<OperationData> list3 = new List<OperationData>();
					foreach (OperationData operationData in list2)
					{
						if ((operationData.operation as TISpaceFleetOperationTemplate).CancelUponCombat())
						{
							list3.Add(operationData);
						}
					}
					foreach (OperationData operationData2 in list3)
					{
						tispaceFleetState3.CancelOperation(operationData2);
					}
				}
			}
			if (this.combatOccurs && this.hab != null && this.stances.ContainsKey(this.hab.faction) && (this.stances[this.hab.faction] == CombatStance.Pursue || this.stances[this.hab.faction] == CombatStance.ExtendedPursuit_Stretch || this.stances[this.hab.faction] == CombatStance.ExtendedPursuit_Envelop))
			{
				if (this.stances.Any<KeyValuePair<TIFactionState, CombatStance>>((KeyValuePair<TIFactionState, CombatStance> x) => x.Value == CombatStance.Evade))
				{
					this.hab = null;
				}
			}
			if (this.combatOccurs && this.hab != null && this.hab.ActiveCombatModules().Count == 0)
			{
				if (!this.fleets.None<TISpaceFleetState>((TISpaceFleetState x) => ((x != null) ? x.faction : null) == this.hab.faction))
				{
					TISpaceFleetState tispaceFleetState4 = this.FleetFor(this.hab.faction);
					if (tispaceFleetState4 == null || tispaceFleetState4.ships.Count != 0)
					{
						return;
					}
				}
				if (this.attacker.ships.All<TISpaceShipState>((TISpaceShipState x) => x.hull.simpleHull) && this.defendingFaction.permanentAlly(this.hab.faction))
				{
					this.autoDestroyHab = true;
					return;
				}
				this.combatOccurs = false;
			}
		}

		// Token: 0x0600431C RID: 17180 RVA: 0x001B1D70 File Offset: 0x001AFF70
		public void SetPrecombatDuration(double precombatDuration_s)
		{
			this.precombatDuration_s = precombatDuration_s;
		}

		// Token: 0x0600431D RID: 17181 RVA: 0x001B1D7C File Offset: 0x001AFF7C
		public void GainCombatFactionHate(TISpaceAssetState victim, TIFactionState causingFaction, float value)
		{
			if (!GameControl.control.skirmishMode && victim != null && causingFaction != null)
			{
				if (victim.isSpaceFleetState)
				{
					TISpaceFleetState ref_fleet = victim.ref_fleet;
					ref_fleet.AssignedGoal();
					List<ValueTuple<GoalType, TIGameState, TIFactionState>> recentGoalInfo = ref_fleet.GetRecentGoalInfo(14f);
					bool flag;
					if (ref_fleet.faction.enemyWarFactions.Contains(causingFaction))
					{
						flag = recentGoalInfo.Any<ValueTuple<GoalType, TIGameState, TIFactionState>>(([TupleElementNames(new string[] { "Type", "Target", "TargetFaction" })] ValueTuple<GoalType, TIGameState, TIFactionState> x) => x.Item1 == GoalType.AttackWithFleet);
					}
					else
					{
						flag = false;
					}
					bool flag2 = flag;
					bool flag3 = (from x in recentGoalInfo
						where TIFactionGoalState.OffensiveFleetGoals.Contains(x.Item1)
						where x.Item3 == causingFaction
						select x).Any<ValueTuple<GoalType, TIGameState, TIFactionState>>();
					bool flag4 = victim.ref_fleet == this.attacker;
					bool flag5 = this.hab != null && this.hab.faction == causingFaction;
					if (!flag4 && !flag3 && !flag2 && !flag5)
					{
						ref_fleet.faction.GainFactionHate(causingFaction, value, false, "Space Combat", true);
						return;
					}
				}
				else
				{
					victim.faction.GainFactionHate(causingFaction, value, false, "Space Combat", true);
				}
			}
		}

		// Token: 0x0600431E RID: 17182 RVA: 0x001B1EDC File Offset: 0x001B00DC
		public void RecordOfficerKilled(TIOfficerState officer)
		{
			TISpaceShipState ship = officer.ship;
			TISpaceFleetState fleet = ship.fleet;
			TIFactionState faction = fleet.faction;
			if (!fleet.combatState.deadOfficers.ContainsKey(ship))
			{
				fleet.combatState.deadOfficers.Add(ship, new List<TIOfficerState>());
			}
			fleet.combatState.deadOfficers[ship].Add(officer);
			if (!fleet.combatState.officerDeathsRecord.ContainsKey(faction))
			{
				fleet.combatState.officerDeathsRecord.Add(faction, new List<string>());
			}
			fleet.combatState.officerDeathsRecord[faction].Add(officer.DisplayNameAndShipAndJob);
		}

		// Token: 0x0600431F RID: 17183 RVA: 0x001B1F83 File Offset: 0x001B0183
		public void RecordShipDisengaged(TISpaceShipState disengagedShip)
		{
			this.combatRecord.AddAssetSurvivedRecord(disengagedShip, true, SingleAssetCombatOutcome.None);
		}

		// Token: 0x06004320 RID: 17184 RVA: 0x001B1F94 File Offset: 0x001B0194
		public void RecordShipDestroyed(TISpaceShipState destroyedShip, TIGameState killer, TIFactionState killerFaction, TIShipWeaponTemplate killerWeapon)
		{
			if (killerFaction != null && !killerFaction.permanentAlly(destroyedShip.faction))
			{
				if (killer != null)
				{
					TISpaceShipState ref_ship = killer.ref_ship;
					if (ref_ship != null)
					{
						ref_ship.RecordKill(destroyedShip.hull);
					}
				}
				this.GainCombatFactionHate(destroyedShip.fleet, killerFaction, (float)destroyedShip.hull.structuralIntegrity * TemplateManager.global.factionHateSIFactorPerShipDestroyed);
				if (killerFaction.isActivePlayer && destroyedShip.hull.dataName == "AlienMothership")
				{
					killerFaction.UnlockAchievement("destroyMothership");
				}
			}
			if (destroyedShip.officers.Count > 0)
			{
				if (!this.officerDeathsRecord.ContainsKey(destroyedShip.faction))
				{
					this.officerDeathsRecord.Add(destroyedShip.faction, new List<string>());
				}
				foreach (TIOfficerState tiofficerState in destroyedShip.officers)
				{
					this.officerDeathsRecord[destroyedShip.faction].Add(tiofficerState.DisplayNameAndShipAndJob);
				}
			}
			if (this.ref_orbit != null && !this.ref_orbit.isAdHocOrbit)
			{
				this.ref_orbit.DestroyedAssetsChange(1);
			}
			this.combatRecord.AddAssetDestroyedRecord(destroyedShip, killer, killerWeapon);
			this.shipDestructionsRecorded++;
		}

		// Token: 0x06004321 RID: 17185 RVA: 0x001B20F0 File Offset: 0x001B02F0
		public void RecordSurvivors()
		{
			foreach (TISpaceFleetState tispaceFleetState in this.fleets)
			{
				if (tispaceFleetState != null)
				{
					foreach (TISpaceShipState tispaceShipState in tispaceFleetState.ships)
					{
						if (!tispaceShipState.hasDisengaged && !tispaceShipState.ShipDestroyed())
						{
							this.combatRecord.AddAssetSurvivedRecord(tispaceShipState, false, SingleAssetCombatOutcome.None);
						}
					}
				}
			}
			if (this.hab != null)
			{
				if (this.autoDestroyHab)
				{
					this.combatRecord.singleAssetRecords.Add(new CombatRecord.SingleAssetCombatRecord
					{
						faction = this.hab.ref_faction,
						assetName = this.hab.GetDisplayName(GameControl.control.activePlayer),
						outcome = SingleAssetCombatOutcome.Destroyed,
						fled = false,
						asset = this.hab,
						assetSummary = this.hab.GetLocalizedHabModuleList()
					});
					return;
				}
				this.combatRecord.AddAssetSurvivedRecord(this.hab, false, SingleAssetCombatOutcome.None);
			}
		}

		// Token: 0x06004322 RID: 17186 RVA: 0x001B2224 File Offset: 0x001B0424
		public void SetWinnerAndLoser()
		{
			this.bothSidesDestroyed = false;
			this.draw = false;
			this.winner = null;
			this.loser = null;
			if (this.combatOccurs)
			{
				TISpaceCombatState.<>c__DisplayClass177_0 CS$<>8__locals1 = new TISpaceCombatState.<>c__DisplayClass177_0();
				List<CombatRecord.SingleAssetCombatRecord> singleAssetRecords = this.combatRecord.singleAssetRecords;
				List<CombatRecord.SingleAssetCombatRecord> list = ((singleAssetRecords != null) ? singleAssetRecords.Where<CombatRecord.SingleAssetCombatRecord>(delegate(CombatRecord.SingleAssetCombatRecord x)
				{
					TIFactionState faction3 = x.faction;
					return faction3 != null && faction3.permanentAlly(this.factions[0]);
				}).ToList<CombatRecord.SingleAssetCombatRecord>() : null);
				List<CombatRecord.SingleAssetCombatRecord> singleAssetRecords2 = this.combatRecord.singleAssetRecords;
				List<CombatRecord.SingleAssetCombatRecord> list2 = ((singleAssetRecords2 != null) ? singleAssetRecords2.Where<CombatRecord.SingleAssetCombatRecord>(delegate(CombatRecord.SingleAssetCombatRecord x)
				{
					TIFactionState faction2 = x.faction;
					return faction2 != null && faction2.permanentAlly(this.factions[1]);
				}).ToList<CombatRecord.SingleAssetCombatRecord>() : null);
				bool flag = list2.All<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord y) => y.outcome == SingleAssetCombatOutcome.Destroyed || y.outcome == SingleAssetCombatOutcome.HabDisabled || y.outcome == SingleAssetCombatOutcome.HabNoncombatant || y.fled);
				bool flag2 = list.All<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord y) => y.outcome == SingleAssetCombatOutcome.Destroyed || y.outcome == SingleAssetCombatOutcome.HabDisabled || y.outcome == SingleAssetCombatOutcome.HabNoncombatant || y.fled);
				TISpaceCombatState.<>c__DisplayClass177_0 CS$<>8__locals2 = CS$<>8__locals1;
				TIHabState tihabState = this.hab;
				CS$<>8__locals2.habFaction = ((tihabState != null) ? tihabState.faction : null);
				if (flag && flag2)
				{
					if (CS$<>8__locals1.habFaction == null)
					{
						this.bothSidesDestroyed = true;
					}
					else
					{
						this.winner = CS$<>8__locals1.habFaction;
						this.loser = this.factions.First<TIFactionState>((TIFactionState x) => x != CS$<>8__locals1.habFaction);
					}
				}
				else if (!flag && !flag2)
				{
					if (CS$<>8__locals1.habFaction == null)
					{
						if (this.votedEndCombatFirst == this.factions[0])
						{
							this.loser = this.factions[0];
							this.winner = this.factions[1];
						}
						else if (this.votedEndCombatFirst == this.factions[1])
						{
							this.winner = this.factions[0];
							this.loser = this.factions[1];
						}
						else
						{
							this.draw = true;
						}
					}
					else
					{
						this.winner = CS$<>8__locals1.habFaction;
						this.loser = this.factions.First<TIFactionState>((TIFactionState x) => x != CS$<>8__locals1.habFaction);
					}
				}
				else
				{
					this.oneSideDestroyed = true;
					if (flag)
					{
						this.winner = this.factions[0];
						this.loser = this.factions[1];
						if (this.loser.isActivePlayer && !GameControl.control.skirmishMode)
						{
							if (list2.All<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord o) => o.outcome == SingleAssetCombatOutcome.Destroyed))
							{
								this.loser.UnlockAchievement("loseCombatNoSurvivors");
							}
						}
						if (this.winner.isActivePlayer && !GameControl.control.skirmishMode)
						{
							if (list.None<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord o) => o.outcome == SingleAssetCombatOutcome.Destroyed))
							{
								this.winner.UnlockAchievement("winCombatNoLosses");
							}
						}
					}
					else
					{
						this.winner = this.factions[1];
						this.loser = this.factions[0];
						if (this.loser.isActivePlayer && this.factions.Contains(this.loser) && !GameControl.control.skirmishMode)
						{
							if (list.All<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord o) => o.outcome == SingleAssetCombatOutcome.Destroyed))
							{
								this.loser.UnlockAchievement("loseCombatNoSurvivors");
							}
						}
						if (this.winner.isActivePlayer && this.factions.Contains(this.winner) && !GameControl.control.skirmishMode)
						{
							if (list2.None<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord o) => o.outcome == SingleAssetCombatOutcome.Destroyed))
							{
								this.winner.UnlockAchievement("winCombatNoLosses");
							}
						}
					}
				}
				if (!GameControl.control.skirmishMode && this.winner != null && this.winner.isActivePlayer && this.factions.Contains(this.winner))
				{
					this.winner.UnlockAchievement("winCombat");
					if (this.initialFactionFleetStrengths[this.loser] != 0f && this.initialFactionFleetStrengths[this.winner] != 0f && this.initialFactionFleetStrengths[this.loser] / this.initialFactionFleetStrengths[this.winner] >= 5f)
					{
						this.winner.UnlockAchievement("winBattleOutmatched");
					}
				}
			}
			else if (this.stances[this.factions[0]] == this.stances[this.factions[1]])
			{
				this.draw = true;
			}
			else if (this.stances[this.factions[0]] == CombatStance.Evade)
			{
				this.winner = this.factions[1];
				this.loser = this.factions[0];
			}
			else
			{
				this.winner = this.factions[0];
				this.loser = this.factions[1];
			}
			List<TIFactionState> list3 = this.factions.Where<TIFactionState>((TIFactionState x) => x != null).ToList<TIFactionState>();
			if (!this.combatOccurs || list3.Count != 2)
			{
				return;
			}
			using (List<TIFactionState>.Enumerator enumerator = list3.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIFactionState faction = enumerator.Current;
					float num = this.initialFactionFleetStrengths[faction];
					TIFactionState tifactionState = list3.First<TIFactionState>((TIFactionState x) => x != faction);
					float perceivedEnemyFleetStrengthFactor = faction.GetPerceivedEnemyFleetStrengthFactor(tifactionState);
					float num2 = this.initialFactionFleetStrengths[tifactionState] * perceivedEnemyFleetStrengthFactor;
					if (num2 != 0f)
					{
						float num3 = num2;
						if (this.hab != null)
						{
							if (this.hab.faction == faction)
							{
								num += this.initialHabStrength;
							}
							else
							{
								num3 += this.initialHabStrength;
							}
						}
						bool flag3 = num > num3;
						bool flag4 = this.winner == faction;
						float num4 = 1f;
						if (flag3 != flag4 || this.draw)
						{
							float num5 = 1f + (num - num3) / num2;
							num5 = Mathf.Max(num5, 0.1f);
							float num6 = num / num3;
							if (num6 >= 1f)
							{
								num6 /= num5;
							}
							else
							{
								num6 = num5 / num6;
							}
							num6 *= 0.1f;
							if (this.draw)
							{
								num6 /= 2f;
							}
							if (num5 >= 1f)
							{
								num4 = Mathf.Lerp(1f, num5, num6);
							}
							else
							{
								num4 = 1f / Mathf.Lerp(1f, 1f / num5, num6);
							}
						}
						else if (flag3)
						{
							num4 = 0.99f;
						}
						num4 = Mathf.Clamp(num4, 0.86956525f, 1.15f);
						faction.AdjustPerceivedEnemyFleetStrengthFactor(tifactionState, num4);
					}
				}
			}
		}

		// Token: 0x06004323 RID: 17187 RVA: 0x001B295C File Offset: 0x001B0B5C
		public void EndCombatForStrategyGame(double combatDuration_s)
		{
			this.SetWinnerAndLoser();
			if (this.winner != null && this.oneSideDestroyed)
			{
				this.combatLog.Winner = this.winner;
			}
			this.factions[0].AddCombatLog(this.combatLog);
			List<TISpaceFleetState> list = this.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x != null).ToList<TISpaceFleetState>();
			foreach (TISpaceFleetState tispaceFleetState in list)
			{
				tispaceFleetState.AddFleetLog("PostCombat");
			}
			if (this.combatOccurs && this.hab != null && !this.autoDestroyHab)
			{
				this.GainCombatFactionHate(this.hab, this.factions.FirstOrDefault<TIFactionState>((TIFactionState x) => this.primaryCombatFaction(this.hab.faction) != x), (float)(this.hab.tier * 2));
				if (this.hab.faction == this.loser)
				{
					TIOrbitState ref_orbit = this.ref_orbit;
					if (ref_orbit != null && ref_orbit.isEarthLEO)
					{
						List<CombatRecord.SingleAssetCombatRecord> singleAssetRecords = this.combatRecord.singleAssetRecords;
						bool? flag;
						if (singleAssetRecords == null)
						{
							flag = null;
						}
						else
						{
							IEnumerable<CombatRecord.SingleAssetCombatRecord> enumerable = singleAssetRecords.Where<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.faction == this.factions[0]);
							if (enumerable == null)
							{
								flag = null;
							}
							else
							{
								flag = new bool?(enumerable.Any<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.asset.isSpaceShipState && x.asset.ref_ship.hull.simpleHull && x.outcome != SingleAssetCombatOutcome.Destroyed && !x.fled));
							}
						}
						bool? flag2 = flag;
						bool valueOrDefault = flag2.GetValueOrDefault();
						List<CombatRecord.SingleAssetCombatRecord> singleAssetRecords2 = this.combatRecord.singleAssetRecords;
						bool? flag3;
						if (singleAssetRecords2 == null)
						{
							flag3 = null;
						}
						else
						{
							IEnumerable<CombatRecord.SingleAssetCombatRecord> enumerable2 = singleAssetRecords2.Where<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.faction == this.factions[0]);
							if (enumerable2 == null)
							{
								flag3 = null;
							}
							else
							{
								flag3 = new bool?(enumerable2.None<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.asset.isSpaceShipState && !x.asset.ref_ship.hull.simpleHull && x.outcome != SingleAssetCombatOutcome.Destroyed));
							}
						}
						flag2 = flag3;
						bool valueOrDefault2 = flag2.GetValueOrDefault();
						if (valueOrDefault && valueOrDefault2)
						{
							this.autoDestroyHab = true;
						}
					}
					if (this.autoDestroyHab)
					{
						goto IL_02B8;
					}
					using (List<TIHabModuleState>.Enumerator enumerator2 = this.hab.OkayModules().ToList<TIHabModuleState>().GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							TIHabModuleState tihabModuleState = enumerator2.Current;
							if (tihabModuleState.isCombatModule)
							{
								this.hab.DestroyModule(this.winner, tihabModuleState, true, true, true, 0f, true, false);
							}
						}
						goto IL_02B8;
					}
				}
				List<TISpaceFleetState> list2 = list.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction == this.loser).ToList<TISpaceFleetState>();
				if (list2.Count > 0)
				{
					list2.ForEach(delegate(TISpaceFleetState x)
					{
						x.RecordFailedAttackOnTarget(this.hab, 1f, true);
					});
				}
			}
			IL_02B8:
			if (this.combatOccurs)
			{
				List<CombatRecord.SingleAssetCombatRecord> singleAssetRecords3 = this.combatRecord.singleAssetRecords;
				int? num;
				if (singleAssetRecords3 == null)
				{
					num = null;
				}
				else
				{
					IEnumerable<CombatRecord.SingleAssetCombatRecord> enumerable3 = singleAssetRecords3.Where<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.faction == this.factions[0]);
					if (enumerable3 == null)
					{
						num = null;
					}
					else
					{
						num = new int?(enumerable3.Count<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.outcome == SingleAssetCombatOutcome.Destroyed));
					}
				}
				int? num2 = num;
				int valueOrDefault3 = num2.GetValueOrDefault();
				List<CombatRecord.SingleAssetCombatRecord> singleAssetRecords4 = this.combatRecord.singleAssetRecords;
				int? num3;
				if (singleAssetRecords4 == null)
				{
					num3 = null;
				}
				else
				{
					IEnumerable<CombatRecord.SingleAssetCombatRecord> enumerable4 = singleAssetRecords4.Where<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.faction == this.factions[1]);
					if (enumerable4 == null)
					{
						num3 = null;
					}
					else
					{
						num3 = new int?(enumerable4.Count<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.outcome == SingleAssetCombatOutcome.Destroyed));
					}
				}
				num2 = num3;
				int valueOrDefault4 = num2.GetValueOrDefault();
				if (this.combatRecord.singleAssetRecords.Any<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.outcome == SingleAssetCombatOutcome.Destroyed))
				{
					this.officerPromotions = new Dictionary<TIFactionState, List<TIOfficerState>>();
					float num4 = Mathf.Pow(2f * (1f - this.combatBalance), 2f);
					float num5 = Mathf.Pow(2f * this.combatBalance, 2f);
					foreach (TISpaceFleetState tispaceFleetState2 in list)
					{
						if (tispaceFleetState2.faction != null && tispaceFleetState2.ships.Count > 0)
						{
							if (!this.officerPromotions.ContainsKey(tispaceFleetState2.faction))
							{
								this.officerPromotions.Add(tispaceFleetState2.faction, new List<TIOfficerState>());
							}
							foreach (TISpaceShipState tispaceShipState in tispaceFleetState2.ships.ToList<TISpaceShipState>().Shuffle<TISpaceShipState>())
							{
								if (!tispaceShipState.ShipDestroyed())
								{
									this.officerPromotions[tispaceFleetState2.faction].AddRange(tispaceShipState.CheckForOfficerPromotionEvent(OfficerSpawnEventType.SurviveCombat, (tispaceFleetState2.faction == this.factions[0]) ? num4 : num5, true, this.officerPromotions[tispaceFleetState2.faction].ToList<TIOfficerState>()));
									if (tispaceFleetState2.faction == this.winner)
									{
										this.officerPromotions[tispaceFleetState2.faction].AddRange(tispaceShipState.CheckForOfficerPromotionEvent(OfficerSpawnEventType.WinCombat, (tispaceFleetState2.faction == this.factions[0]) ? num4 : num5, true, this.officerPromotions[tispaceFleetState2.faction].ToList<TIOfficerState>()));
									}
								}
							}
							foreach (CombatRecord.SingleAssetCombatRecord singleAssetCombatRecord in this.combatRecord.singleAssetRecords.Shuffle<CombatRecord.SingleAssetCombatRecord>())
							{
								if (singleAssetCombatRecord.outcome == SingleAssetCombatOutcome.Destroyed && TIGameState.Valid(singleAssetCombatRecord.killer) && singleAssetCombatRecord.killer.isSpaceShipState && !singleAssetCombatRecord.killer.ref_ship.ShipDestroyed() && singleAssetCombatRecord.killer.ref_faction != singleAssetCombatRecord.faction)
								{
									this.officerPromotions[tispaceFleetState2.faction].AddRange(singleAssetCombatRecord.killer.ref_ship.CheckForOfficerPromotionEvent(OfficerSpawnEventType.CombatKill, 0f, false, this.officerPromotions[tispaceFleetState2.faction].ToList<TIOfficerState>()));
									if (!string.IsNullOrEmpty(singleAssetCombatRecord.killerWeaponTemplateName))
									{
										TIShipWeaponTemplate tishipWeaponTemplate = TemplateManager.Find<TIShipWeaponTemplate>(singleAssetCombatRecord.killerWeaponTemplateName, true);
										if (tishipWeaponTemplate != null)
										{
											if (tishipWeaponTemplate.isBeamWeapon)
											{
												this.officerPromotions[tispaceFleetState2.faction].AddRange(singleAssetCombatRecord.killer.ref_ship.CheckForOfficerPromotionEvent(OfficerSpawnEventType.CombatKill_Beam, 0f, false, this.officerPromotions[tispaceFleetState2.faction].ToList<TIOfficerState>()));
											}
											else if (tishipWeaponTemplate.isGunTypeWeapon)
											{
												this.officerPromotions[tispaceFleetState2.faction].AddRange(singleAssetCombatRecord.killer.ref_ship.CheckForOfficerPromotionEvent(OfficerSpawnEventType.CombatKill_Guns, 0f, false, this.officerPromotions[tispaceFleetState2.faction].ToList<TIOfficerState>()));
											}
											else if (tishipWeaponTemplate.isMissileWeapon)
											{
												this.officerPromotions[tispaceFleetState2.faction].AddRange(singleAssetCombatRecord.killer.ref_ship.CheckForOfficerPromotionEvent(OfficerSpawnEventType.CombatKill_Missiles, 0f, false, this.officerPromotions[tispaceFleetState2.faction].ToList<TIOfficerState>()));
											}
										}
									}
								}
							}
						}
					}
				}
				TINotificationQueueState.LogSpaceBattleTakesPlace(this.factions[0], this.factions[1], this.winner, this.loser, this.fleets.FirstOrDefault<TISpaceFleetState>(), this.fleets.LastOrDefault<TISpaceFleetState>(), this.hab, (this.hab == null) ? this.nearbyNaturalSpaceObject.ref_spaceObject : this.hab.ref_spaceObject, this.displayName, valueOrDefault3, valueOrDefault4);
			}
			else
			{
				TISpaceObjectState tispaceObjectState = ((this.hab == null) ? this.nearbyNaturalSpaceObject.ref_spaceObject : this.hab.ref_spaceObject);
				if (tispaceObjectState.isLagrangePointState)
				{
					tispaceObjectState = tispaceObjectState.ref_lagrangePoint.ref_spaceBody;
				}
				TINotificationQueueState.LogNoSpaceBattleTakesPlace(this.factions[0], this.factions[1], tispaceObjectState, this.displayName);
			}
			List<TISpaceFleetState> list3 = new List<TISpaceFleetState>(list);
			foreach (TIFactionState tifactionState in this.preservedFleetCompositions.Keys)
			{
				TIGameState location = this.GetLocation(tifactionState);
				foreach (PreservedFleetRecord preservedFleetRecord in this.preservedFleetCompositions[tifactionState])
				{
					if (preservedFleetRecord.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.exists && !x.ShipDestroyed()))
					{
						TISpaceFleetState originFleet = null;
						if (location != null && location.isSpaceFleetState)
						{
							originFleet = location.ref_fleet;
						}
						TISpaceFleetState tispaceFleetState3 = TISpaceFleetState.CreateAtRunTime(tifactionState, preservedFleetRecord.ships.Where<TISpaceShipState>((TISpaceShipState x) => originFleet.ships.Contains(x) && x.exists && !x.ShipDestroyed()).ToList<TISpaceShipState>(), location, originFleet, null, false, false, null);
						if (TIGameState.Valid(preservedFleetRecord.homeport))
						{
							tispaceFleetState3.SetHomePort(preservedFleetRecord.homeport);
						}
						FactionGoal_Fleet goal = preservedFleetRecord.goal;
						if (goal != null)
						{
							goal.AssignFleet(tispaceFleetState3);
						}
						foreach (TIFactionState tifactionState2 in preservedFleetRecord.namesByFaction.Keys.ToList<TIFactionState>())
						{
							tispaceFleetState3.ForceDisplayName(tifactionState2, preservedFleetRecord.namesByFaction[tifactionState2]);
						}
						list3.Add(tispaceFleetState3);
					}
				}
			}
			list3 = list3.Where<TISpaceFleetState>((TISpaceFleetState x) => TIGameState.Valid(x)).ToList<TISpaceFleetState>();
			if (this.newFleetsCreatedByExtendedPursuit != null)
			{
				MergeFleetOperation mergeFleetOperation = new MergeFleetOperation();
				foreach (TISpaceFleetState tispaceFleetState4 in this.newFleetsCreatedByExtendedPursuit)
				{
					if (TIGameState.Valid(tispaceFleetState4) && TIGameState.Valid(tispaceFleetState4.parentFleet))
					{
						list3.AddUnique(tispaceFleetState4.parentFleet);
						list3.Remove(tispaceFleetState4);
						mergeFleetOperation.OnOperationExecute(tispaceFleetState4.parentFleet, tispaceFleetState4);
					}
				}
			}
			list3 = list3.Where<TISpaceFleetState>((TISpaceFleetState x) => TIGameState.Valid(x)).ToList<TISpaceFleetState>();
			if (!this.draw)
			{
				list3 = list3.OrderBy<TISpaceFleetState, int>(delegate(TISpaceFleetState x)
				{
					if (!x.faction.permanentAlly(this.loser))
					{
						return 1;
					}
					return 0;
				}).ToList<TISpaceFleetState>();
			}
			foreach (TISpaceFleetState tispaceFleetState5 in list3)
			{
				tispaceFleetState5.PostCombat(this, this.precombatDuration_s + combatDuration_s, !tispaceFleetState5.inTransfer && (tispaceFleetState5.faction == this.loser || (this.draw && this.fleets.IndexOf(tispaceFleetState5) == 1)));
				if (tispaceFleetState5.returnToOperationsTime != null && tispaceFleetState5.returnToOperationsTime <= TITimeState.Now())
				{
					tispaceFleetState5.CombatRecovery();
				}
			}
			if (this.autoDestroyHab)
			{
				this.hab.DestroyHab(this.factions.First<TIFactionState>((TIFactionState x) => x != this.hab.faction), 0f, false, this.fleets.First<TISpaceFleetState>((TISpaceFleetState x) => x.faction != this.hab.faction), 0f);
			}
			else if (TIGameState.Valid(this.hab))
			{
				this.hab.PostCombat();
			}
			this.CleanUpFighterGameStates();
			if (this.combatOccurs)
			{
				if (this.winner != null && this.combatRecord.winnerSalvage != null)
				{
					TIResourcesCost winnerSalvage = this.combatRecord.winnerSalvage;
					float num6 = 0.2f;
					TISpaceFleetState winningFleet = this.winningFleet;
					this.combatRecord.winnerSalvage = winnerSalvage.MultiplyCost((num6 + ((winningFleet != null) ? new float?(winningFleet.SalvageBonus()) : null)).GetValueOrDefault());
					this.combatRecord.winnerSalvage.RefundCost(this.winner, "Salvage");
				}
				TIFactionState loser = this.loser;
				if (loser != null && loser.IsAlienFaction)
				{
					if (this.combatRecord.singleAssetRecords.Any<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.outcome == SingleAssetCombatOutcome.Destroyed && x.faction.IsAlienFaction))
					{
						this.winner.CompleteMilestone(CampaignMilestone.AccessAlienShip);
					}
					TIGlobalValuesState.GlobalValues.CheckGlobalMilestone(GlobalMilestone.FirstSpaceCombatVictoryAgainstAliens, this.winner, this.winner);
					TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienAwareness_Public);
				}
				GameControl.eventManager.TriggerEvent(new CombatEnds(this), null, Array.Empty<object>());
			}
			this.active = false;
			foreach (TISpaceCombatProjectileState tispaceCombatProjectileState in GameStateManager.IterateByClass<TISpaceCombatProjectileState>(false).ToList<TISpaceCombatProjectileState>())
			{
				GameStateManager.RemoveGameState<TISpaceCombatProjectileState>(tispaceCombatProjectileState.ID, false);
			}
			base.ArchiveState(true);
			GameStateManager.RemoveGameState<TISpaceCombatState>(base.ID, false);
			foreach (TISpaceFleetState tispaceFleetState6 in list3.ToList<TISpaceFleetState>())
			{
				if (tispaceFleetState6 != null && tispaceFleetState6.deleted)
				{
					Log.Info("Post Combat Deleted Fleet: " + tispaceFleetState6.ID.ToString(), Array.Empty<object>());
					foreach (TIFactionState tifactionState3 in GameStateManager.AllFactions())
					{
						if (tifactionState3.fleets.Contains(tispaceFleetState6))
						{
							tifactionState3.fleets.Remove(tispaceFleetState6);
							Log.Error("Was NOT removed from faction list.", Array.Empty<object>());
						}
					}
				}
				if (tispaceFleetState6 != null && tispaceFleetState6.dummyFleet)
				{
					tispaceFleetState6.RemoveShipsFromFleet(tispaceFleetState6.ships, null);
				}
			}
		}

		// Token: 0x06004324 RID: 17188 RVA: 0x001B391C File Offset: 0x001B1B1C
		public void CancelCombat()
		{
			this.active = false;
			foreach (TISpaceFleetState tispaceFleetState in this.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x != null).ToList<TISpaceFleetState>().ToList<TISpaceFleetState>())
			{
				tispaceFleetState.PostCombat(this, 0.0, false);
			}
			GameStateManager.RemoveGameState<TISpaceCombatState>(base.ID, false);
		}

		// Token: 0x06004325 RID: 17189 RVA: 0x001B39BC File Offset: 0x001B1BBC
		public TIGameState GetLocation(TIFactionState faction)
		{
			List<TISpaceFleetState> list = this.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x != null).ToList<TISpaceFleetState>();
			TISpaceFleetState tispaceFleetState = list.FirstOrDefault<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				TIFactionState faction2 = x.faction;
				return faction2 != null && faction2.permanentAlly(faction);
			});
			TIGameState tigameState = ((tispaceFleetState != null) ? tispaceFleetState.ref_gameState : null);
			if (tigameState == null)
			{
				if (this.hab != null)
				{
					tigameState = this.hab;
				}
				if (tigameState == null)
				{
					TISpaceFleetState tispaceFleetState2 = list.FirstOrDefault<TISpaceFleetState>();
					tigameState = ((tispaceFleetState2 != null) ? tispaceFleetState2.ref_gameState : null);
					if (tigameState == null)
					{
						TISpaceFleetState tispaceFleetState3 = list.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => x.orbitState != null);
						tigameState = ((tispaceFleetState3 != null) ? tispaceFleetState3.ref_orbit : null);
						if (tigameState == null)
						{
							tigameState = (faction.IsAlienFaction ? GameStateManager.AlienFaction().primaryHab.ref_spaceBody.orbits[0] : GameStateManager.Earth().orbits[0]);
							Log.Error("Could not find location for " + faction.displayName + " preserved fleets.", Array.Empty<object>());
						}
					}
				}
			}
			return tigameState;
		}

		// Token: 0x17000CBD RID: 3261
		// (get) Token: 0x06004326 RID: 17190 RVA: 0x001B3B0B File Offset: 0x001B1D0B
		public float AutoresolveSecondsElapsed
		{
			get
			{
				Stopwatch stopwatch = this.autoResolveStopwatch;
				return (float)((stopwatch != null) ? stopwatch.GetElapsedSeconds() : 0.0);
			}
		}

		// Token: 0x17000CBE RID: 3262
		// (get) Token: 0x06004327 RID: 17191 RVA: 0x001B3B28 File Offset: 0x001B1D28
		// (set) Token: 0x06004328 RID: 17192 RVA: 0x001B3B30 File Offset: 0x001B1D30
		public SimulatedCombat SimulatedCombat { get; private set; }

		// Token: 0x06004329 RID: 17193 RVA: 0x001B3B3C File Offset: 0x001B1D3C
		public void Autoresolve()
		{
			if (this.fleets[1] != null || this.hab != null)
			{
				this.autoResolveStopwatch = Stopwatch.StartNew();
				this.autoresolving = true;
				this.SimulatedCombat = SimulatedCombat.Simulate(this, 7200f, delegate(SimulatedCombat simulatedCombat)
				{
					if (this.attackingFaction.isActivePlayer || this.defendingFaction.isActivePlayer)
					{
						GameControl.eventManager.TriggerEvent(new CombatSimulationUpdated(simulatedCombat, 1f), null, Array.Empty<object>());
					}
					else
					{
						this.ApplySimulatedCombat();
					}
					this.autoresolving = false;
					Log.Debug("Autoresolve() : " + this.autoResolveStopwatch.GetElapsedSeconds().ToString() + "s", Array.Empty<object>());
				});
				return;
			}
			this.<Autoresolve>g__EndCombat|193_0(0f);
		}

		// Token: 0x0600432A RID: 17194 RVA: 0x001B3BA4 File Offset: 0x001B1DA4
		public void ApplySimulatedCombat()
		{
			foreach (SimulatedCombat.SimulatedCombatHabModule simulatedCombatHabModule in this.SimulatedCombat.CombatHabModules.Where<SimulatedCombat.SimulatedCombatHabModule>((SimulatedCombat.SimulatedCombatHabModule x) => x.isDestroyed))
			{
				simulatedCombatHabModule.Module.hab.DestroyModule(this.attackingFaction, simulatedCombatHabModule.Module, true, true, true, 0f, true, false);
			}
			using (List<SimulatedCombat.SimulatedShip>.Enumerator enumerator2 = this.SimulatedCombat.ShipsA.Concat<SimulatedCombat.SimulatedShip>(this.SimulatedCombat.ShipsB).ToList<SimulatedCombat.SimulatedShip>().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SimulatedCombat.SimulatedShip simulatedShip = enumerator2.Current;
					foreach (TIOfficerState tiofficerState in simulatedShip.DeadSimulatedOfficers.Select<TIOfficerState, TIOfficerState>((TIOfficerState x) => simulatedShip.SimulatedOfficersToRealOfficers[x]).ToList<TIOfficerState>())
					{
						tiofficerState.DeleteOfficer(true);
						this.RecordOfficerKilled(tiofficerState);
					}
					simulatedShip.OriginalShip.BecomeCopyOf(simulatedShip.CopyShip);
					if (simulatedShip.isDestroyed)
					{
						SimulatedCombat.SimulatedCombatant destroyer = simulatedShip.Destroyer;
						TIFactionState tifactionState = ((destroyer != null) ? destroyer.GetFaction() : null);
						TISpaceShipState originalShip = simulatedShip.OriginalShip;
						SimulatedCombat.SimulatedCombatant destroyer2 = simulatedShip.Destroyer;
						TIGameState tigameState = ((destroyer2 != null) ? destroyer2.OriginalGameState : null);
						SimulatedCombat.SimulatedWeapon destroyerWeapon = simulatedShip.DestroyerWeapon;
						TIFactionState tifactionState2 = ((destroyerWeapon != null) ? destroyerWeapon.Combatant.GetFaction() : null);
						SimulatedCombat.SimulatedWeapon destroyerWeapon2 = simulatedShip.DestroyerWeapon;
						this.RecordShipDestroyed(originalShip, tigameState, tifactionState2, (destroyerWeapon2 != null) ? destroyerWeapon2.Template : null);
						simulatedShip.OriginalShip.DestroyShip(true, tifactionState);
					}
				}
			}
			foreach (TISpaceFleetState tispaceFleetState in this.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => TIGameState.Valid(x)))
			{
				tispaceFleetState.TeleportAllToFormation(false, false);
			}
			this.RecordSurvivors();
			this.EndCombatForStrategyGame((double)this.SimulatedCombat.ElapsedTime_s);
			GameControl.spaceCombat.SetCombat(null);
		}

		// Token: 0x17000CBF RID: 3263
		// (get) Token: 0x0600432B RID: 17195 RVA: 0x001B3E74 File Offset: 0x001B2074
		public CombatInfo CombatInfo
		{
			get
			{
				CombatInfo combatInfo = new CombatInfo();
				combatInfo.Combat = this;
				combatInfo.ships = this.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x != null).SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships);
				combatInfo.hab = this.hab;
				combatInfo.combatRecord = this.combatRecord;
				return combatInfo;
			}
		}

		// Token: 0x0600433E RID: 17214 RVA: 0x001B40F8 File Offset: 0x001B22F8
		[CompilerGenerated]
		private void <Autoresolve>g__EndCombat|193_0(float combatDuration_s)
		{
			this.RecordSurvivors();
			this.EndCombatForStrategyGame((double)combatDuration_s);
			GameControl.spaceCombat.SetCombat(null);
		}

		// Token: 0x040027EB RID: 10219
		public const float PURSUIT_DISTANCE_m = -1f;

		// Token: 0x040027EC RID: 10220
		public const float PURSUIT_DISTANCE_RANGE_MULTIPLIER = 200f;

		// Token: 0x040027ED RID: 10221
		public TIDateTime combatStartDateTime;

		// Token: 0x040027EE RID: 10222
		[SerializeField]
		public CartesianState combatGlobalPosition;

		// Token: 0x040027EF RID: 10223
		public TINaturalSpaceObjectState nearbyNaturalSpaceObject;

		// Token: 0x040027F0 RID: 10224
		public TISpaceFleetState[] fleets;

		// Token: 0x040027F1 RID: 10225
		public TIHabState hab;

		// Token: 0x040027F2 RID: 10226
		public TIFactionState[] factions;

		// Token: 0x040027F3 RID: 10227
		public Dictionary<TIFactionState, List<TISpaceAssetState>> assets;

		// Token: 0x040027F4 RID: 10228
		private List<TISpaceFleetState> dummyFleetStates = new List<TISpaceFleetState>();

		// Token: 0x040027F5 RID: 10229
		public bool active;

		// Token: 0x040027F6 RID: 10230
		public Dictionary<TIFactionState, float> initialFactionFleetStrengths;

		// Token: 0x040027F7 RID: 10231
		public float initialHabStrength;

		// Token: 0x040027FC RID: 10236
		public bool autoresolve;

		// Token: 0x040027FD RID: 10237
		public bool autoDestroyHab;

		// Token: 0x04002800 RID: 10240
		public Dictionary<TISpaceShipState, List<TISpaceCombatWaypointState>> shipWaypoints;

		// Token: 0x04002801 RID: 10241
		public Dictionary<TIFactionState, bool> votedEndCombat = new Dictionary<TIFactionState, bool>();

		// Token: 0x04002802 RID: 10242
		public TIFactionState votedEndCombatFirst;

		// Token: 0x04002803 RID: 10243
		public int shipDestroyedTriggers;

		// Token: 0x04002804 RID: 10244
		public int shipDestructionsRecorded;

		// Token: 0x04002805 RID: 10245
		public Dictionary<TIFactionState, List<PreservedFleetRecord>> preservedFleetCompositions;

		// Token: 0x04002806 RID: 10246
		private List<TISpaceFleetState> newFleetsCreatedByExtendedPursuit = new List<TISpaceFleetState>();

		// Token: 0x04002807 RID: 10247
		public bool allowNoAttackingFleetAtInitialization;

		// Token: 0x0400280D RID: 10253
		public Dictionary<TISpaceShipState, float> maxDeltaVAvailableForCombat_kps = new Dictionary<TISpaceShipState, float>();

		// Token: 0x0400280E RID: 10254
		public Dictionary<TIFactionState, Dictionary<TINationState, PlannedFighters>> STOFighterPlans = new Dictionary<TIFactionState, Dictionary<TINationState, PlannedFighters>>();

		// Token: 0x0400280F RID: 10255
		public TIFactionState.CombatLog combatLog;

		// Token: 0x04002811 RID: 10257
		public CombatRecord combatRecord;

		// Token: 0x04002817 RID: 10263
		public Dictionary<TIFactionState, List<TIOfficerState>> officerPromotions = new Dictionary<TIFactionState, List<TIOfficerState>>();

		// Token: 0x04002818 RID: 10264
		public Dictionary<TIFactionState, List<string>> officerDeathsRecord = new Dictionary<TIFactionState, List<string>>();

		// Token: 0x04002819 RID: 10265
		public Dictionary<TISpaceShipState, List<TIOfficerState>> deadOfficers = new Dictionary<TISpaceShipState, List<TIOfficerState>>();

		// Token: 0x0400281A RID: 10266
		private Stopwatch autoResolveStopwatch;

		// Token: 0x0400281B RID: 10267
		public bool autoresolving;

		// Token: 0x0400281C RID: 10268
		public bool mayRejectAutoresolve = true;
	}
}
