using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.Tasks;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200073A RID: 1850
	public abstract class FactionGoal_Fleet : TIFactionGoalState
	{
		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x06002EB9 RID: 11961 RVA: 0x000FDEE4 File Offset: 0x000FC0E4
		// (set) Token: 0x06002EBA RID: 11962 RVA: 0x000FDEEC File Offset: 0x000FC0EC
		public TISpaceFleetState assignedFleet { get; private set; }

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x06002EBB RID: 11963
		public abstract List<Type> fleetOperations { get; }

		// Token: 0x06002EBC RID: 11964
		public abstract bool RequiresFleet();

		// Token: 0x06002EBD RID: 11965 RVA: 0x000FDEF5 File Offset: 0x000FC0F5
		public bool CanUseFleet()
		{
			if (!this.RequiresFleet())
			{
				List<Type> fleetOperations = this.fleetOperations;
				return fleetOperations != null && fleetOperations.Count > 0;
			}
			return true;
		}

		// Token: 0x06002EBE RID: 11966 RVA: 0x000FDF15 File Offset: 0x000FC115
		public bool LookingForFleet()
		{
			return this.CanUseFleet() && (this.assignedFleet == null || this.assignedFleet.deleted || this.assignedFleet.ships.Count == 0);
		}

		// Token: 0x06002EBF RID: 11967 RVA: 0x000FDF51 File Offset: 0x000FC151
		public virtual bool SpaceCombatGoal()
		{
			return false;
		}

		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06002EC0 RID: 11968 RVA: 0x000FDF54 File Offset: 0x000FC154
		public virtual bool FleetCouncilorGoal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06002EC1 RID: 11969 RVA: 0x000FDF58 File Offset: 0x000FC158
		public Trajectory ExampleTrajectory
		{
			get
			{
				if (this.assignedFleet == null)
				{
					return null;
				}
				float num = float.PositiveInfinity;
				if (this.exampleTrajectoryCacheDatestamp != null)
				{
					num = (float)(TITimeState.Now() - this.exampleTrajectoryCacheDatestamp).TotalDays;
				}
				if (num > 30f)
				{
					TIGameState tigameState = this.target();
					if ((tigameState != null && tigameState.ref_orbit == null) || (tigameState.isSpaceFleetState && tigameState.ref_fleet.landed))
					{
						TIOrbitState tiorbitState = tigameState.ref_naturalSpaceObject.orbits.OrderByDescending<TIOrbitState, bool>((TIOrbitState x) => x.interfaceOrbit).FirstOrDefault<TIOrbitState>();
						if (tiorbitState != null)
						{
							tigameState = tiorbitState;
						}
					}
					if (tigameState != null)
					{
						List<IMobileAsset> list = new List<IMobileAsset>();
						list.Add(this.assignedFleet);
						float num2 = this.assignedFleet.faction.ships.Average<TISpaceShipState>((TISpaceShipState x) => x.template.baseCruiseAcceleration_mps2(false));
						this.assignedFleet.faction.ships.Average<TISpaceShipState>((TISpaceShipState x) => x.template.baseCruiseDeltaV_mps(false));
						float num3 = Mathf.Max(num2, this.assignedFleet.cruiseAcceleration_mps2);
						float num4 = Mathf.Max(num2, this.assignedFleet.currentDeltaV_mps);
						list.Add(new TIVirtualSpaceFleet(this.assignedFleet, num3, num4, this.assignedFleet.faction));
						list.Add(new TIVirtualSpaceFleet(this.assignedFleet, num3, (num4 + 10f) * 2f, this.assignedFleet.faction));
						list.Add(new TIVirtualSpaceFleet(this.assignedFleet, Mathf.Max(num3, 1f), Mathf.Max(num4, 100f), this.assignedFleet.faction));
						list.Add(new TIVirtualSpaceFleet(this.assignedFleet, Mathf.Max(num3, 5f), Mathf.Max(num4, 400f), this.assignedFleet.faction));
						foreach (IMobileAsset mobileAsset in list)
						{
							TransferResult transferResult;
							AIDailyFactionPlanner.SelectTrajectoryAsync(mobileAsset, tigameState, 0.6f, out transferResult, delegate(Trajectory x)
							{
								this.cachedExampleTrajectory = x;
							}, false, 0.2);
							if (this.cachedExampleTrajectory != null)
							{
								break;
							}
						}
					}
					this.exampleTrajectoryCacheDatestamp = TITimeState.Now();
					this.exampleTrajectoryCacheDatestamp.AddDays(-7f * TIUtilities.RandomFloatValue());
				}
				return this.cachedExampleTrajectory;
			}
		}

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x06002EC2 RID: 11970 RVA: 0x000FE210 File Offset: 0x000FC410
		// (set) Token: 0x06002EC3 RID: 11971 RVA: 0x000FE218 File Offset: 0x000FC418
		public TIHabState resupplyHab { get; set; }

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x06002EC4 RID: 11972 RVA: 0x000FE221 File Offset: 0x000FC421
		// (set) Token: 0x06002EC5 RID: 11973 RVA: 0x000FE229 File Offset: 0x000FC429
		public TISpaceBodyState flyByLocation { get; protected set; }

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x06002EC6 RID: 11974 RVA: 0x000FE232 File Offset: 0x000FC432
		public override bool isFleetGoal
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x06002EC7 RID: 11975 RVA: 0x000FE235 File Offset: 0x000FC435
		public override FactionGoal_Fleet ref_fleetGoal
		{
			get
			{
				return this;
			}
		}

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06002EC8 RID: 11976 RVA: 0x000FE238 File Offset: 0x000FC438
		public bool needsFleet
		{
			get
			{
				return this.RequiresFleet() && this.assignedFleet == null;
			}
		}

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x06002EC9 RID: 11977 RVA: 0x000FE250 File Offset: 0x000FC450
		public virtual bool buildFleetsSequentially
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x06002ECA RID: 11978 RVA: 0x000FE254 File Offset: 0x000FC454
		public bool IsFrontGoal
		{
			get
			{
				return (from x in (from x in this.faction.GoalsOfType(this.GetGoalType(), false, true)
						select x as FactionGoal_Fleet into x
						where !x.skipGoal && !x.ShouldPauseGoal()
						select x).Where<FactionGoal_Fleet>(delegate(FactionGoal_Fleet x)
					{
						if (!x.NeedsShipsOrdered())
						{
							return x.PendingShips().Any<ShipConstructionQueueItem>((ShipConstructionQueueItem y) => !y.costPaid);
						}
						return true;
					})
					orderby x.assignedDate
					select x).FirstOrDefault<FactionGoal_Fleet>() == this;
			}
		}

		// Token: 0x06002ECB RID: 11979 RVA: 0x000FE310 File Offset: 0x000FC510
		private ShipConstructionQueueItem FindConstructionQueueItemForPendingShip(string dataName)
		{
			foreach (List<ShipConstructionQueueItem> list in this.faction.nShipyardQueues.Values)
			{
				foreach (ShipConstructionQueueItem shipConstructionQueueItem in list)
				{
					if (shipConstructionQueueItem.AIFactionGoal == this && shipConstructionQueueItem.shipDesignTemplateName == dataName)
					{
						return shipConstructionQueueItem;
					}
				}
			}
			return null;
		}

		// Token: 0x06002ECC RID: 11980 RVA: 0x000FE3C0 File Offset: 0x000FC5C0
		public TIGameState GetBombardmentTarget(TISpaceFleetState fleet, IEnumerable<TIGameState> emergencyTargets)
		{
			BombardOperation_Med bombardOperation_Med = new BombardOperation_Med();
			List<TIGameState> possibleTargets = bombardOperation_Med.GetPossibleTargets(fleet, null);
			IEnumerable<TIGameState> enumerable = emergencyTargets.Intersect<TIGameState>(possibleTargets);
			if (possibleTargets.Contains(this.target()))
			{
				return this.target();
			}
			TIGameState tigameState = this.target();
			if (((tigameState != null) ? tigameState.ref_spaceBody : null) == null)
			{
				return null;
			}
			if (fleet.ref_spaceBody.isEarth)
			{
				if (this is FactionGoal_AttackWithFleet)
				{
					if (this.target().isArmyState || this.target().isRegionSpaceFacility || this.target().isRegionAlienAsset)
					{
						return this.target();
					}
					if (this.target().isNationState && (this.faction.executiveNations.Any<TINationState>((TINationState x) => x.wars.Contains(this.target())) || this.faction.executiveNations.Count == 0))
					{
						TINationState ref_nation = this.target().ref_nation;
						List<TIArmyState> armies = ref_nation.armies;
						if (armies.Count > 0)
						{
							TIArmyState tiarmyState = armies.FirstOrDefault<TIArmyState>((TIArmyState x) => x.InEnemyCapital());
							if (tiarmyState == null)
							{
								if (base.importance < 20)
								{
									armies.RemoveAll((TIArmyState x) => x.ref_region.antiSpaceDefenses);
								}
								if (base.importance < 15)
								{
									armies.RemoveAll((TIArmyState x) => x.InFriendlyRegion);
								}
								tiarmyState = armies.FirstOrDefault<TIArmyState>((TIArmyState x) => x.InBattleWithArmiesOrRegionDefenses());
								if (tiarmyState == null && armies.Count > 0)
								{
									tiarmyState = armies.SelectRandomItem<TIArmyState>();
								}
							}
							if (tiarmyState != null)
							{
								return tiarmyState;
							}
						}
						List<TIRegionSpaceFacilityState> list = ref_nation.regions.SelectMany<TIRegionState, TIRegionSpaceFacilityState>((TIRegionState x) => x.spaceFacilities).ToList<TIRegionSpaceFacilityState>();
						if (list.Count > 0)
						{
							if (base.importance < 20)
							{
								list.RemoveAll((TIRegionSpaceFacilityState x) => x.ref_region.antiSpaceDefenses);
							}
							if (list.Count > 0)
							{
								return list.MaxBy<TIRegionSpaceFacilityState, float>((TIRegionSpaceFacilityState x) => x.ref_region.boostPerMonth_dekatons / 3f + (float)x.ref_region.missionControl);
							}
						}
					}
				}
				else
				{
					IEnumerable<TINationState> enumerable2 = (from x in this.faction.executiveNations.SelectMany<TINationState, TINationState>((TINationState x) => x.wars).Distinct<TINationState>()
						where !this.faction.executiveNations.Contains(x)
						select x).ToList<TINationState>();
					Dictionary<TIGameState, int> neutralizeNations = this.faction.GoalsOfType(GoalType.NeutralizeNation, false, true).ToDictionary<TIFactionGoalState, TIGameState, int>((TIFactionGoalState x) => x.target(), (TIFactionGoalState x) => x.importance);
					IOrderedEnumerable<TINationState> orderedEnumerable = enumerable2.OrderByDescending<TINationState, float>(delegate(TINationState x)
					{
						if (!(x.executiveFaction != null))
						{
							return -1f;
						}
						return this.faction.GetFactionHate(x.executiveFaction);
					}).ThenByDescending<TINationState, int>(delegate(TINationState x)
					{
						if (!neutralizeNations.ContainsKey(x))
						{
							return -1;
						}
						return neutralizeNations[x];
					});
					List<TIArmyState> list2 = (from x in orderedEnumerable.SelectMany<TINationState, TIArmyState>((TINationState x) => x.armies)
						where possibleTargets.Contains(x)
						where !x.currentRegion.antiSpaceDefenses || !x.FriendlyRegion(x.currentRegion)
						select x).ToList<TIArmyState>();
					list2.RemoveAll(delegate(TIArmyState x)
					{
						TIFactionState faction = x.faction;
						return faction != null && faction.permanentAlly(this.faction);
					});
					List<TIArmyState> list3 = list2.Where<TIArmyState>((TIArmyState x) => this.faction.executiveNations.Contains(x.currentRegion.nation)).ToList<TIArmyState>();
					if (list3.Any<TIArmyState>())
					{
						list2 = list3;
					}
					else
					{
						list2.RemoveAll((TIArmyState x) => !x.InBattleWithArmiesOrRegionDefenses() && this.faction != null && (this.faction.HasNAP(x.faction, true) || this.faction.HasTruce(x.faction, true)));
					}
					if (list2.Any<TIArmyState>())
					{
						TIArmyState tiarmyState2 = (from x in list2
							orderby x.InBattleWithArmiesOrRegionDefenses() descending, x.currentRegion == x.currentNation.capital descending
							select x).FirstOrDefault<TIArmyState>();
						if (tiarmyState2 != null)
						{
							return tiarmyState2;
						}
					}
					TIRegionSpaceFacilityState tiregionSpaceFacilityState = (from x in orderedEnumerable.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions).SelectMany<TIRegionState, TIRegionSpaceFacilityState>((TIRegionState x) => x.spaceFacilities)
						where possibleTargets.Contains(x)
						select x).FirstOrDefault<TIRegionSpaceFacilityState>();
					if (tiregionSpaceFacilityState != null)
					{
						return tiregionSpaceFacilityState;
					}
				}
				return null;
			}
			IEnumerable<TIGameState> enumerable3;
			if (enumerable.Any<TIGameState>())
			{
				enumerable3 = enumerable;
			}
			else
			{
				enumerable3 = possibleTargets.Where<TIGameState>(delegate(TIGameState x)
				{
					if (x.ref_faction != null)
					{
						TIFactionState faction2 = this.faction;
						return faction2 != null && faction2.AI_AtWarWithFaction(x.ref_faction);
					}
					return false;
				});
			}
			IEnumerable<TIGameState> enumerable4 = enumerable3.Where<TIGameState>(delegate(TIGameState x)
			{
				TIHabSiteState ref_habSite = x.ref_habSite;
				return ((ref_habSite != null) ? ref_habSite.ref_hab : null) == null || x.ref_habSite.ref_hab.SpaceCombatValue() == 0f;
			});
			if (enumerable4.Count<TIGameState>() > 0)
			{
				return enumerable4.SelectRandomItem<TIGameState>();
			}
			return enumerable3.Select<TIGameState, TIHabState>((TIGameState x) => x.ref_hab).MinBy<TIHabState, float>((TIHabState x) => x.SpaceCombatValue());
		}

		// Token: 0x06002ECD RID: 11981 RVA: 0x000FE970 File Offset: 0x000FCB70
		public override TIDataTemplate SavingForTemplate(TIFactionState faction, out bool alreadyOrdered, out TIHabModuleState shipyard)
		{
			shipyard = null;
			alreadyOrdered = false;
			foreach (ShipConstructionQueueItem shipConstructionQueueItem in this.PendingShips())
			{
				if (!shipConstructionQueueItem.costPaid)
				{
					alreadyOrdered = true;
					shipyard = shipConstructionQueueItem.shipyard;
					return shipConstructionQueueItem.shipDesign;
				}
			}
			if (this.NeedsShipsOrdered())
			{
				return faction.GetDesiredShipToBuild(this, false);
			}
			return null;
		}

		// Token: 0x06002ECE RID: 11982 RVA: 0x000FE9EC File Offset: 0x000FCBEC
		public virtual void OnTransferComplete()
		{
			this.learnedPerformanceRequirements = new LearnedPerformanceRequirements();
		}

		// Token: 0x06002ECF RID: 11983 RVA: 0x000FE9FC File Offset: 0x000FCBFC
		public override void OnGoalComplete()
		{
			TISpaceFleetState assignedFleet = this.assignedFleet;
			if (this.faction != null)
			{
				this.faction.RemoveGoal(this);
				List<GoalType> subsequentGoals = this.subsequentGoals;
				if (subsequentGoals != null && subsequentGoals.Count > 0)
				{
					IEnumerable<TIFactionGoalState> enumerable = this.BuildSubsequentGoals();
					foreach (TIFactionGoalState tifactionGoalState in (enumerable ?? Enumerable.Empty<TIFactionGoalState>()))
					{
						if (tifactionGoalState != null)
						{
							this.faction.AddGoal(tifactionGoalState, HandleDuplicateGoalRule.ResetImportance, assignedFleet);
						}
					}
				}
			}
		}

		// Token: 0x06002ED0 RID: 11984 RVA: 0x000FEAA0 File Offset: 0x000FCCA0
		public virtual void AssignFleet(TISpaceFleetState fleet)
		{
			if (fleet != this.assignedFleet)
			{
				if (fleet != null)
				{
					FactionGoal_Fleet factionGoal_Fleet = fleet.AssignedGoal();
					if (factionGoal_Fleet != null)
					{
						factionGoal_Fleet.UnassignFleet();
					}
				}
				if (this.assignedFleet != null)
				{
					this.UnassignFleet();
				}
			}
			this.assignedFleet = fleet;
			if (this.assignedFleet != null)
			{
				fleet.faction.fleetGoalTracker[this.assignedFleet] = this;
			}
			if (fleet != null)
			{
				fleet.AddFleetLog("Assigned");
			}
		}

		// Token: 0x06002ED1 RID: 11985 RVA: 0x000FEB20 File Offset: 0x000FCD20
		public virtual void UnassignFleet()
		{
			if (this.assignedFleet != null && this.faction.fleetGoalTracker.ContainsKey(this.assignedFleet))
			{
				this.faction.fleetGoalTracker[this.assignedFleet] = null;
			}
			TISpaceFleetState assignedFleet = this.assignedFleet;
			this.assignedFleet = null;
			if (assignedFleet == null)
			{
				return;
			}
			assignedFleet.AddFleetLog("Unassigned");
		}

		// Token: 0x06002ED2 RID: 11986 RVA: 0x000FEB86 File Offset: 0x000FCD86
		public virtual bool LeaveMyFleetAlone()
		{
			return this.assignedFleet != null && this.assignedFleet.inTransfer;
		}

		// Token: 0x06002ED3 RID: 11987 RVA: 0x000FEBA4 File Offset: 0x000FCDA4
		public override void DailyGoalMaintenance()
		{
			if (!TIGameState.Valid(this.assignedFleet))
			{
				this.UnassignFleet();
			}
			foreach (TISpaceFleetState tispaceFleetState in this.pendingFleets.ToList<TISpaceFleetState>())
			{
				if (!TIGameState.Valid(tispaceFleetState))
				{
					this.RemovePendingFleet(tispaceFleetState);
				}
			}
		}

		// Token: 0x06002ED4 RID: 11988 RVA: 0x000FEC18 File Offset: 0x000FCE18
		public IEnumerable<ShipConstructionQueueItem> PendingShips()
		{
			return from x in this.faction.nShipyardQueues.SelectMany<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>, ShipConstructionQueueItem>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Value)
				where x.AIFactionGoal == this
				select x;
		}

		// Token: 0x06002ED5 RID: 11989 RVA: 0x000FEC65 File Offset: 0x000FCE65
		public List<TISpaceShipTemplate> PendingShipTemplates()
		{
			return (from x in this.PendingShips()
				select x.shipDesign).ToList<TISpaceShipTemplate>();
		}

		// Token: 0x06002ED6 RID: 11990 RVA: 0x000FEC96 File Offset: 0x000FCE96
		public List<string> PendingShipDataNames()
		{
			return (from x in this.PendingShips()
				select x.shipDesignTemplateName).ToList<string>();
		}

		// Token: 0x06002ED7 RID: 11991 RVA: 0x000FECC7 File Offset: 0x000FCEC7
		public bool AddPendingFleet(TISpaceFleetState pendingFleet)
		{
			if (TIGameState.Valid(pendingFleet) && !this.pendingFleets.Contains(pendingFleet))
			{
				this.pendingFleets.Add(pendingFleet);
				return true;
			}
			return false;
		}

		// Token: 0x06002ED8 RID: 11992 RVA: 0x000FECEE File Offset: 0x000FCEEE
		public bool RemovePendingFleet(TISpaceFleetState pendingFleet)
		{
			return this.pendingFleets.Remove(pendingFleet);
		}

		// Token: 0x06002ED9 RID: 11993 RVA: 0x000FECFC File Offset: 0x000FCEFC
		public virtual bool ReadyForTransferToTarget(TISpaceFleetState fleet)
		{
			if (!TIGameState.Valid(fleet) || !TIGameState.Valid(this.target()))
			{
				return false;
			}
			if (!fleet.CanFulfillGoal(this, false))
			{
				return false;
			}
			if (fleet.SpaceCombatValue() < this.desiredFleetCombatValue)
			{
				if (this.SpaceCombatGoal())
				{
					return false;
				}
				if (fleet.ref_system != null && fleet.ref_system != this.target().ref_system)
				{
					return false;
				}
				if (fleet.dockedAtHab && fleet.ref_hab.faction.permanentAlly(fleet.faction))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06002EDA RID: 11994 RVA: 0x000FED90 File Offset: 0x000FCF90
		public virtual bool NeedsFlagshipOrdered(List<TISpaceShipTemplate> pendingShips)
		{
			if (this.desiredFlagshipHull != null)
			{
				TISpaceFleetState assignedFleet = this.assignedFleet;
				bool flag;
				if (assignedFleet == null)
				{
					flag = true;
				}
				else
				{
					flag = !assignedFleet.ships.Select<TISpaceShipState, string>((TISpaceShipState x) => x.hull.dataName).Contains(this.desiredFlagshipHull.dataName);
				}
				if (flag)
				{
					if (!(from x in this.pendingFleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships)
						select x.hull.dataName).Contains(this.desiredFlagshipHull.dataName))
					{
						return !pendingShips.Select<TISpaceShipTemplate, string>((TISpaceShipTemplate x) => x.hullTemplate.dataName).Contains(this.desiredFlagshipHull.dataName);
					}
				}
			}
			return false;
		}

		// Token: 0x06002EDB RID: 11995 RVA: 0x000FEE90 File Offset: 0x000FD090
		public virtual bool NeedsShipsOrdered()
		{
			List<TISpaceShipTemplate> list = this.PendingShipTemplates();
			if (this.NeedsFlagshipOrdered(list))
			{
				return true;
			}
			if (this.NeedsPrimaryRoleOrdered(list))
			{
				return true;
			}
			this.DailyGoalMaintenance();
			TISpaceFleetState assignedFleet = this.assignedFleet;
			if ((assignedFleet == null || !assignedFleet.CanFulfillGoal(this, false)) && this.pendingFleets.None<TISpaceFleetState>((TISpaceFleetState x) => x.CanFulfillGoal(this, false)) && list.None<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.CanFulfillGoal(this)))
			{
				return true;
			}
			TISpaceFleetState assignedFleet2 = this.assignedFleet;
			return ((assignedFleet2 != null) ? assignedFleet2.SpaceCombatValue() : 0f) + this.pendingFleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()) + list.Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.TemplateSpaceCombatValue(false, -1f, 1f, false)) < this.desiredFleetCombatValue;
		}

		// Token: 0x06002EDC RID: 11996
		public abstract ShipRole GetPrimaryShipRole();

		// Token: 0x06002EDD RID: 11997 RVA: 0x000FEF7C File Offset: 0x000FD17C
		public virtual bool NeedsPrimaryRoleOrdered(List<TISpaceShipTemplate> pendingShipTemplates)
		{
			ShipRole primaryRole = this.GetPrimaryShipRole();
			if (primaryRole == ShipRole.NoRole)
			{
				return false;
			}
			TISpaceFleetState assignedFleet = this.assignedFleet;
			Func<TISpaceShipState, bool> <>9__3;
			return (assignedFleet == null || !assignedFleet.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.role == primaryRole)) && !this.pendingFleets.Any<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				IEnumerable<TISpaceShipState> ships = x.ships;
				Func<TISpaceShipState, bool> func;
				if ((func = <>9__3) == null)
				{
					func = (<>9__3 = (TISpaceShipState y) => y.role == primaryRole);
				}
				return ships.Any<TISpaceShipState>(func);
			}) && !pendingShipTemplates.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.role == primaryRole);
		}

		// Token: 0x06002EDE RID: 11998
		public abstract Dictionary<ShipRole, float> GetSecondaryShipRoles();

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x06002EDF RID: 11999 RVA: 0x000FF000 File Offset: 0x000FD200
		public List<ShipRole> allRoles
		{
			get
			{
				return (from x in new List<ShipRole> { this.GetPrimaryShipRole() }.Union<ShipRole>(this.GetSecondaryShipRoles().Keys)
					where x > ShipRole.NoRole
					select x).ToList<ShipRole>();
			}
		}

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x06002EE0 RID: 12000 RVA: 0x000FF057 File Offset: 0x000FD257
		public List<ShipRole> allPrimaryRoles
		{
			get
			{
				List<ShipRole> list = new List<ShipRole>();
				list.Add(this.GetPrimaryShipRole());
				return list.Where<ShipRole>((ShipRole x) => x > ShipRole.NoRole).ToList<ShipRole>();
			}
		}

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06002EE1 RID: 12001 RVA: 0x000FF094 File Offset: 0x000FD294
		public float desiredFleetCombatValue
		{
			get
			{
				if (this.desiredFleetCombatValueCachedDate == null || (TITimeState.Now() - this.desiredFleetCombatValueCachedDate).TotalDays >= 1.0)
				{
					this.cachedDesiredFleetCombatValue = this.ComputeDesiredFleetCombatValue();
					this.desiredFleetCombatValueCachedDate = TITimeState.Now();
				}
				return this.cachedDesiredFleetCombatValue;
			}
		}

		// Token: 0x06002EE2 RID: 12002 RVA: 0x000FF0EF File Offset: 0x000FD2EF
		public static float ComputeBaselineFleetCombatValue(TIFactionState faction, TIGameState location)
		{
			return Mathf.Max(AIEvaluators.GetRiskAdjustedThreatLevelAtLocation(faction, location, !faction.IsAlienFaction), TemplateManager.global.minimumFleetStrength);
		}

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06002EE3 RID: 12003 RVA: 0x000FF110 File Offset: 0x000FD310
		public virtual bool ShouldPerformMissionMinimallyArmed
		{
			get
			{
				return !this.SpaceCombatGoal() && (this.faction.IsAlienFaction && !AIEvaluators.ShouldAliensGoLoud());
			}
		}

		// Token: 0x06002EE4 RID: 12004 RVA: 0x000FF133 File Offset: 0x000FD333
		public virtual float ComputeDesiredFleetCombatValue()
		{
			if (this.ShouldPerformMissionMinimallyArmed)
			{
				return 0f;
			}
			return FactionGoal_Fleet.ComputeBaselineFleetCombatValue(this.faction, this.target());
		}

		// Token: 0x06002EE5 RID: 12005 RVA: 0x000FF154 File Offset: 0x000FD354
		public bool HasEnoughSpaceCombatValue(TISpaceFleetState fleet)
		{
			return fleet.SpaceCombatValue() >= this.desiredFleetCombatValue;
		}

		// Token: 0x06002EE6 RID: 12006 RVA: 0x000FF167 File Offset: 0x000FD367
		public virtual float GetMaximumFleetCombatValueRatio()
		{
			return 1.5f;
		}

		// Token: 0x06002EE7 RID: 12007 RVA: 0x000FF16E File Offset: 0x000FD36E
		public float GetMaximumFleetCombatValue()
		{
			return this.desiredFleetCombatValue * this.GetMaximumFleetCombatValueRatio();
		}

		// Token: 0x06002EE8 RID: 12008 RVA: 0x000FF17D File Offset: 0x000FD37D
		public virtual float GetForcePursueFleetCombatValue(TISpaceFleetState enemyFleet, TIHabState hab)
		{
			return this.desiredFleetCombatValue;
		}

		// Token: 0x06002EE9 RID: 12009 RVA: 0x000FF188 File Offset: 0x000FD388
		public virtual bool MayIncreaseFleetSize()
		{
			if (this.assignedFleet == null)
			{
				return true;
			}
			float maximumFleetCombatValue = this.GetMaximumFleetCombatValue();
			if (this.desiredFleetCombatValue >= maximumFleetCombatValue)
			{
				return false;
			}
			float num = Mathf.Max(maximumFleetCombatValue * 0.9f, this.desiredFleetCombatValue);
			return this.assignedFleet.SpaceCombatValue() < num;
		}

		// Token: 0x06002EEA RID: 12010
		public abstract float GetDesiredAssaultCombatValue();

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x06002EEB RID: 12011 RVA: 0x000FF1D8 File Offset: 0x000FD3D8
		public virtual TIShipHullTemplate desiredFlagshipHull
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002EEC RID: 12012 RVA: 0x000FF1DC File Offset: 0x000FD3DC
		public virtual Type GetBestOperation(TISpaceFleetState fleet, List<Type> candidateOperations, out TIGameState operationTarget)
		{
			FactionGoal_Fleet.<>c__DisplayClass79_0 CS$<>8__locals1 = new FactionGoal_Fleet.<>c__DisplayClass79_0();
			CS$<>8__locals1.fleet = fleet;
			CS$<>8__locals1.<>4__this = this;
			if (candidateOperations.Count == 0)
			{
				operationTarget = null;
				return null;
			}
			if (CS$<>8__locals1.fleet.IsAlien())
			{
				TISpaceBodyState ref_spaceBody = CS$<>8__locals1.fleet.ref_spaceBody;
				if (ref_spaceBody != null && ref_spaceBody.isEarth)
				{
					if (candidateOperations.Contains(typeof(AlienCrashdownOperation)))
					{
						if (new AlienCrashdownOperation().ValidOperation(CS$<>8__locals1.fleet, this.target(), null))
						{
							operationTarget = this.target();
						}
						else
						{
							operationTarget = AIEvaluators.SelectAlienCrashdownRegion(true, false);
						}
						return typeof(AlienCrashdownOperation);
					}
					if (candidateOperations.Contains(typeof(AlienLandArmyOperation)))
					{
						operationTarget = AIEvaluators.SelectAlienArmyLandingRegion(false);
						return typeof(AlienLandArmyOperation);
					}
				}
			}
			bool flag = this.faction.IsAlienFaction && !AIEvaluators.ShouldAliensGoLoud();
			FactionGoal_DefendWithFleet factionGoal_DefendWithFleet = this as FactionGoal_DefendWithFleet;
			bool flag2 = factionGoal_DefendWithFleet != null;
			bool flag3 = this.target() != null && this.target().ref_system == CS$<>8__locals1.fleet.ref_system;
			CS$<>8__locals1.isBossDefenseFleet = flag2 && AIEvaluators.GetBossDefenseGoals(this.faction).Contains(factionGoal_DefendWithFleet);
			bool flag4 = flag2 && flag3 && this.target().isSpaceBodyState;
			FactionGoal_SecureEarthSpace factionGoal_SecureEarthSpace = this as FactionGoal_SecureEarthSpace;
			bool flag5 = this is FactionGoal_AttackWithFleet && !this.ReadyForTransferToTarget(CS$<>8__locals1.fleet) && this.faction.IsAlienFaction;
			candidateOperations.Contains(typeof(BombardOperation_High));
			bool flag6;
			if (this.faction.IsAlienFaction)
			{
				TIHabState primaryHab = this.faction.primaryHab;
				flag6 = ((primaryHab != null) ? primaryHab.ref_system : null) == CS$<>8__locals1.fleet.ref_system;
			}
			else
			{
				flag6 = false;
			}
			bool flag7 = flag6;
			TIGameState tigameState = this as FactionGoal_FoundHab;
			FactionGoal_FoundBase factionGoal_FoundBase = this as FactionGoal_FoundBase;
			bool flag10;
			if (tigameState != null)
			{
				bool? flag8;
				if (factionGoal_FoundBase == null)
				{
					flag8 = null;
				}
				else
				{
					TIHabSiteState site = factionGoal_FoundBase.site;
					flag8 = ((site != null) ? new bool?(site.hasPlannedOrOperatingBase) : null);
				}
				bool? flag9 = flag8;
				if (flag9.GetValueOrDefault() && !this.faction.permanentAlly(factionGoal_FoundBase.site.hab.faction))
				{
					flag10 = CS$<>8__locals1.fleet.ref_system == factionGoal_FoundBase.site.ref_system;
					goto IL_0248;
				}
			}
			flag10 = false;
			IL_0248:
			bool flag11 = flag10;
			TIHabSiteState tihabSiteState = (flag11 ? factionGoal_FoundBase.site : null);
			CS$<>8__locals1.isAssaultFleet = CS$<>8__locals1.fleet.AssaultCombatValue(false) > 0f;
			List<TIGameState> list = new List<TIGameState>();
			List<ValueTuple<TIHabState, float>> list2 = (from x in (from x in this.faction.GoalsOfType(GoalType.CaptureHab, false, true)
					select x as FactionGoal_CaptureHab into x
					where x.target() is TIHabState
					where x.target().ref_system == CS$<>8__locals1.fleet.ref_system
					select x).ToList<FactionGoal_CaptureHab>()
				select x.target() as TIHabState into x
				select new ValueTuple<TIHabState, float>(x, new AssaultHabOperation().GetSuccessChance(CS$<>8__locals1.fleet, x))).ToList<ValueTuple<TIHabState, float>>();
			IEnumerable<ValueTuple<TIHabState, float>> enumerable = list2.Where<ValueTuple<TIHabState, float>>(([TupleElementNames(new string[] { "Hab", "SuccessChance" })] ValueTuple<TIHabState, float> x) => x.Item1.IsBase);
			IEnumerable<TIHabState> enumerable2 = from x in list2
				where CS$<>8__locals1.isAssaultFleet && x.Item2 > 0.25f && (x.Item1.IsBase || x.Item1.SpaceCombatValue() == 0f)
				select x.Item1;
			IEnumerable<TIHabState> enumerable3 = enumerable2.Intersect<TIHabState>(enumerable.Select<ValueTuple<TIHabState, float>, TIHabState>(([TupleElementNames(new string[] { "Hab", "SuccessChance" })] ValueTuple<TIHabState, float> x) => x.Item1));
			if ((this is FactionGoal_DefendWithFleet || flag5 || flag11) && this.target() != null)
			{
				bool flag12 = CS$<>8__locals1.fleet.dockedAtHab && !this.faction.permanentAlly(CS$<>8__locals1.fleet.ref_hab.faction);
				bool jobIsToGuardAStation = this.target().isHabState && factionGoal_DefendWithFleet != null && factionGoal_DefendWithFleet.target().ref_hab.IsStation;
				bool attackAnyEnemy = TIUtilities.RandomFloatValue() < 0f;
				bool attackMostThreateningEnemy = (TIFrameCounter.FrameCount + GameStateManager.AllFactions().IndexOf(this.faction) + this.faction.fleets.IndexOf(CS$<>8__locals1.fleet)) % 10 == 1;
				List<TISpaceFleetState> list3 = (from x in (CS$<>8__locals1.fleet.ref_lagrangePoint != null) ? CS$<>8__locals1.fleet.ref_lagrangePoint.fleetsInOrbit : CS$<>8__locals1.fleet.ref_system.fleetsInSystem
					where TIGameState.Valid(x)
					where x.faction != null
					select x).ToList<TISpaceFleetState>();
				bool flag13 = list3.Where<TISpaceFleetState>((TISpaceFleetState x) => x.CombatFleet()).Any<TISpaceFleetState>((TISpaceFleetState x) => !CS$<>8__locals1.<>4__this.faction.permanentAlly(x.faction));
				List<TIHabState> list4 = (from x in (CS$<>8__locals1.fleet.ref_lagrangePoint != null) ? CS$<>8__locals1.fleet.ref_lagrangePoint.habs : CS$<>8__locals1.fleet.ref_system.habsInSystem
					where TIGameState.Valid(x)
					where x.faction != null
					select x).ToList<TIHabState>();
				float desiredSuperiority = this.faction.GetDesiredSuperiorityForSpontaniousAttack();
				bool flag14 = false;
				if (jobIsToGuardAStation)
				{
					if (!flag13)
					{
						flag14 = true;
					}
					else if (!this.faction.IsAlienFaction)
					{
						flag14 = CS$<>8__locals1.fleet.dockedLocation == this.target() && this.target().ref_system.isEarth && (attackAnyEnemy | attackMostThreateningEnemy);
					}
				}
				list.AddRange(list3.Where<TISpaceFleetState>((TISpaceFleetState x) => CS$<>8__locals1.fleet.SpaceCombatValue() / CS$<>8__locals1.<>4__this.faction.GetPerceivedEnemyFleetStrength(x) >= desiredSuperiority && AIEvaluators.ShouldLaunchEmergencyAttackAgainstAsset(CS$<>8__locals1.<>4__this.faction, x, true)));
				list.AddRange(list4.Where<TIHabState>(delegate(TIHabState x)
				{
					if (x.IsStation)
					{
						if (CS$<>8__locals1.fleet.SpaceCombatValue() / x.PerceivedAggregateDefensiveScore_Station(CS$<>8__locals1.<>4__this.faction) < desiredSuperiority)
						{
							return false;
						}
					}
					else if (x.SpaceCombatValue() > 0f && CS$<>8__locals1.fleet.BombardmentValue(x.ref_spaceBody) < 1.5f * FactionGoal_AttackWithFleet.GetDesiredBombardmentValue(CS$<>8__locals1.fleet.faction, x, 0))
					{
						return false;
					}
					return AIEvaluators.ShouldLaunchEmergencyAttackAgainstAsset(CS$<>8__locals1.<>4__this.faction, x, true);
				}));
				if (tihabSiteState != null)
				{
					list = new List<TIGameState> { tihabSiteState.hab };
				}
				if (list.Count > 0)
				{
					flag14 = true;
				}
				bool flag15 = (!jobIsToGuardAStation || flag14) && !flag12 && !CS$<>8__locals1.fleet.AI_NeedsRearmBadly() && !CS$<>8__locals1.fleet.AI_NeedsRefuelBadly() && !CS$<>8__locals1.fleet.AI_NeedsRepairBadly() && !flag;
				if (flag7 && !flag12)
				{
					flag15 = true;
				}
				if (flag15)
				{
					float num = CS$<>8__locals1.fleet.SpaceCombatValue();
					TIFactionState mostThreateningEnemy = this.faction.GetMostThreateningWarEnemyHumanFaction();
					IEnumerable<TISpaceFleetState> enumerable4;
					if (flag7)
					{
						enumerable4 = list3.Where<TISpaceFleetState>((TISpaceFleetState x) => !CS$<>8__locals1.<>4__this.faction.permanentAlly(x.faction));
					}
					else if (list.Count > 0)
					{
						enumerable4 = from x in list
							where x.isSpaceFleetState
							select x.ref_fleet;
					}
					else
					{
						enumerable4 = (from x in list3
							where CS$<>8__locals1.<>4__this.faction.AI_AtWarWithFaction(x.faction) || CS$<>8__locals1.<>4__this.faction.IsTrespassing(x)
							where (!jobIsToGuardAStation | attackAnyEnemy) || (attackMostThreateningEnemy && mostThreateningEnemy == x.faction)
							where !x.dockedOrLanded
							select x).ToList<TISpaceFleetState>();
					}
					if (enumerable4.Any<TISpaceFleetState>())
					{
						if (flag7)
						{
							operationTarget = enumerable4.MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue());
						}
						else
						{
							operationTarget = AIEvaluators.SelectFleetToAttack(this.faction, enumerable4, num);
						}
						if (operationTarget != null)
						{
							this.dynamicAttackTarget = operationTarget;
							return typeof(TransferOperation);
						}
					}
					IEnumerable<TIHabState> enumerable5;
					if (list.Count > 0)
					{
						enumerable5 = from x in list
							where x.isHabState && x.ref_hab.IsStation
							select x.ref_hab;
					}
					else if (enumerable2.Any<TIHabState>())
					{
						enumerable5 = enumerable2.Where<TIHabState>((TIHabState x) => x.IsStation);
					}
					else
					{
						enumerable5 = (from x in CS$<>8__locals1.fleet.ref_system.habsInSystem
							where x.IsStation
							where CS$<>8__locals1.fleet.faction.AI_AtWarWithFaction(x.faction) || CS$<>8__locals1.<>4__this.faction.IsTrespassing(x)
							select x).ToList<TIHabState>();
					}
					if (enumerable5.Any<TIHabState>())
					{
						TIHabState tihabState = AIEvaluators.SelectStationToAttack(this.faction, enumerable5, num);
						if (tihabState != null)
						{
							this.dynamicAttackTarget = tihabState;
							operationTarget = tihabState;
							return typeof(TransferOperation);
						}
					}
					IEnumerable<TIHabState> enumerable6 = Enumerable.Empty<TIHabState>();
					if (list.Count > 0)
					{
						enumerable6 = from x in list
							where x.isHabState && x.ref_hab.IsBase
							select x.ref_hab;
					}
					else if (flag2 && flag3)
					{
						if (enumerable3.Any<TIHabState>())
						{
							enumerable6 = enumerable3;
						}
						else
						{
							enumerable6 = (from x in CS$<>8__locals1.fleet.ref_system.habsInSystem
								where x.IsBase
								where CS$<>8__locals1.fleet.BombardmentValue(x.ref_spaceBody) >= FactionGoal_AttackWithFleet.GetDesiredBombardmentValue(CS$<>8__locals1.fleet.faction, x, 0)
								where CS$<>8__locals1.fleet.faction.AI_AtWarWithFaction(x.faction) || CS$<>8__locals1.<>4__this.faction.IsTrespassing(x)
								select x).ToList<TIHabState>();
							if (jobIsToGuardAStation)
							{
								enumerable6 = enumerable6.Where<TIHabState>((TIHabState x) => x.SpaceCombatValue() <= 0f);
							}
						}
					}
					if (enumerable6.Any<TIHabState>())
					{
						bool flag16 = enumerable6.Any<TIHabState>((TIHabState x) => x.ref_spaceBody == CS$<>8__locals1.fleet.ref_spaceBody);
						if (flag16 && CS$<>8__locals1.fleet.dockedAtStation && CS$<>8__locals1.fleet.ref_orbit.interfaceOrbit)
						{
							operationTarget = CS$<>8__locals1.fleet.ref_orbit;
							return typeof(UndockFromStationOperation);
						}
						if (!flag16)
						{
							TIHabState tihabState2 = (from x in enumerable6
								orderby x.SpaceCombatValue(), x.mass_kg descending
								select x).First<TIHabState>();
							operationTarget = tihabState2.ref_spaceBody.interfaceOrbits.First<TIOrbitState>();
							return typeof(TransferOperation);
						}
						if (!CS$<>8__locals1.fleet.ref_orbit.interfaceOrbit)
						{
							operationTarget = CS$<>8__locals1.fleet.ref_spaceBody.interfaceOrbits.First<TIOrbitState>();
							return typeof(TransferOperation);
						}
					}
					this.dynamicAttackTarget = null;
				}
			}
			if (candidateOperations.Contains(typeof(MergeFleetOperation)))
			{
				if (this.target() != null && CS$<>8__locals1.fleet.CanMerge(this.target().ref_fleet))
				{
					operationTarget = this.target().ref_fleet;
					return typeof(MergeFleetOperation);
				}
				if (!CS$<>8__locals1.fleet.AI_NeedsRefuel())
				{
					foreach (TISpaceFleetState tispaceFleetState in CS$<>8__locals1.fleet.faction.fleets)
					{
						if (this.faction.fleetGoalTracker.ContainsKey(tispaceFleetState))
						{
							FactionGoal_Fleet factionGoal_Fleet = this.faction.fleetGoalTracker[tispaceFleetState];
							if (CS$<>8__locals1.fleet != tispaceFleetState && factionGoal_Fleet != null && (this.faction.fleetGoalTracker[tispaceFleetState] == this || (this.GetGoalType() == GoalType.JoinFleet && factionGoal_Fleet.GetGoalType() == GoalType.JoinFleet && this.target() == factionGoal_Fleet.target())) && CS$<>8__locals1.fleet.CanMerge(tispaceFleetState) && !tispaceFleetState.AI_NeedsRefuel())
							{
								operationTarget = tispaceFleetState;
								return typeof(MergeFleetOperation);
							}
						}
					}
				}
			}
			bool flag17 = false;
			List<TICouncilorState> list5 = new List<TICouncilorState>();
			FactionGoal_TransportCouncilorsWithFleet factionGoal_TransportCouncilorsWithFleet = null;
			FactionGoal_CaptureHab factionGoal_CaptureHab = null;
			if (this.FleetCouncilorGoal)
			{
				GoalType goalType = this.GetGoalType();
				if (goalType != GoalType.CaptureHab)
				{
					if (goalType == GoalType.TransportCouncilorsViaFleet)
					{
						factionGoal_TransportCouncilorsWithFleet = this as FactionGoal_TransportCouncilorsWithFleet;
						list5 = new List<TICouncilorState>(factionGoal_TransportCouncilorsWithFleet.assignedCouncilors);
					}
				}
				else
				{
					factionGoal_CaptureHab = this as FactionGoal_CaptureHab;
					list5 = new List<TICouncilorState> { factionGoal_CaptureHab.assignedCouncilor };
				}
				if (factionGoal_TransportCouncilorsWithFleet != null && factionGoal_TransportCouncilorsWithFleet.assignedCouncilors.Any<TICouncilorState>((TICouncilorState x) => x.location.ref_fleet != CS$<>8__locals1.<>4__this.assignedFleet))
				{
					flag17 = true;
				}
				else if (factionGoal_CaptureHab != null && TIGameState.Valid(factionGoal_CaptureHab.assignedCouncilor) && factionGoal_CaptureHab.assignedCouncilor.location.ref_fleet != CS$<>8__locals1.fleet)
				{
					flag17 = true;
				}
			}
			bool flag18 = CS$<>8__locals1.fleet.AI_NeedsRefuel();
			bool flag19 = CS$<>8__locals1.fleet.NeedsRearm();
			bool flag20 = CS$<>8__locals1.fleet.NeedsRepair();
			if (!flag18 && !flag19 && !flag20)
			{
				this.resupplyHab = null;
			}
			else if ((flag18 || flag19) && CS$<>8__locals1.fleet.dockedAtHab && CS$<>8__locals1.fleet.ref_hab.AllowsResupply(this.faction, false, false))
			{
				this.resupplyHab = CS$<>8__locals1.fleet.ref_hab;
			}
			else if (flag20 && CS$<>8__locals1.fleet.dockedAtHab && CS$<>8__locals1.fleet.ref_hab.CanFullyRepairFleet(CS$<>8__locals1.fleet))
			{
				this.resupplyHab = CS$<>8__locals1.fleet.ref_hab;
			}
			if (CS$<>8__locals1.fleet.dockedAtHab)
			{
				if (candidateOperations.Contains(typeof(AssaultHabOperation)))
				{
					bool flag21 = list2.Any<ValueTuple<TIHabState, float>>(([TupleElementNames(new string[] { "Hab", "SuccessChance" })] ValueTuple<TIHabState, float> x) => x.Item1 == CS$<>8__locals1.fleet.ref_hab);
					float successChance = new AssaultHabOperation().GetSuccessChance(CS$<>8__locals1.fleet, CS$<>8__locals1.fleet.ref_hab);
					if (flag21 | ((this.faction.IsAlienFaction || this.faction.AvailableMissionControl >= 5) && CS$<>8__locals1.fleet.faction.AI_AtWarWithFaction(CS$<>8__locals1.fleet.ref_hab.faction) && successChance > 0.3f))
					{
						operationTarget = CS$<>8__locals1.fleet.ref_hab;
						return typeof(AssaultHabOperation);
					}
				}
				bool flag22;
				if (!CS$<>8__locals1.fleet.faction.AI_AtWarWithFaction(CS$<>8__locals1.fleet.ref_hab.faction) && !list.Contains(CS$<>8__locals1.fleet.ref_hab) && !(this.dynamicAttackTarget == CS$<>8__locals1.fleet.ref_hab) && !this.faction.IsTrespassing(CS$<>8__locals1.fleet.ref_hab))
				{
					FactionGoal_AttackWithFleet factionGoal_AttackWithFleet = CS$<>8__locals1.fleet.AssignedGoal() as FactionGoal_AttackWithFleet;
					flag22 = factionGoal_AttackWithFleet != null && factionGoal_AttackWithFleet.target() == CS$<>8__locals1.fleet.ref_hab;
				}
				else
				{
					flag22 = true;
				}
				if (flag22 && candidateOperations.Contains(typeof(DestroyHabOperation)))
				{
					operationTarget = CS$<>8__locals1.fleet.ref_hab;
					this.dynamicAttackTarget = operationTarget;
					return typeof(DestroyHabOperation);
				}
				if (!this.faction.permanentAlly(CS$<>8__locals1.fleet.ref_hab.faction))
				{
					operationTarget = CS$<>8__locals1.fleet.ref_orbit;
					return typeof(UndockFromStationOperation);
				}
				if (candidateOperations.Contains(typeof(RepairFleetOperation)))
				{
					operationTarget = CS$<>8__locals1.fleet;
					return typeof(RepairFleetOperation);
				}
				if (candidateOperations.Contains(typeof(ResupplyOperation)))
				{
					operationTarget = CS$<>8__locals1.fleet;
					return typeof(ResupplyOperation);
				}
				if (CS$<>8__locals1.fleet.ref_hab == this.resupplyHab && CS$<>8__locals1.fleet.AssignedGoal() is FactionGoal_ResupplyFleet)
				{
					operationTarget = null;
					return null;
				}
				if (flag17 && list5.Any<TICouncilorState>((TICouncilorState x) => x.location == CS$<>8__locals1.fleet.ref_hab))
				{
					operationTarget = null;
					return null;
				}
				if (this.target() != null && CS$<>8__locals1.fleet.dockedAtStation && this.GetGoalType() == GoalType.DefendWithFleet && ((this.target().isHabState && this.target().ref_hab.IsBase) || this.target().isSpaceBodyState) && this.target().ref_spaceBody == CS$<>8__locals1.fleet.ref_spaceBody && !CS$<>8__locals1.<GetBestOperation>g__IsDangerousToGoAlone|1())
				{
					operationTarget = CS$<>8__locals1.fleet.dockedLocation.ref_orbit;
					return typeof(UndockFromStationOperation);
				}
				if (!CS$<>8__locals1.fleet.ref_hab.faction.permanentAlly(this.faction))
				{
					operationTarget = CS$<>8__locals1.fleet.dockedLocation.ref_orbit;
					return typeof(UndockFromStationOperation);
				}
			}
			else if (candidateOperations.Contains(typeof(AssaultHabOperation)))
			{
				IEnumerable<TIHabState> enumerable7 = enumerable3.Where<TIHabState>((TIHabState x) => x.ref_spaceBody == CS$<>8__locals1.fleet.ref_spaceBody);
				TIHabState tihabState3 = null;
				if (enumerable7.Any<TIHabState>())
				{
					tihabState3 = enumerable7.MinBy<TIHabState, float>((TIHabState x) => x.AssaultCombatValue(true));
				}
				else if (CS$<>8__locals1.fleet.ref_spaceBody != null && (this.faction.IsAlienFaction || this.faction.AvailableMissionControl >= 5))
				{
					AssaultHabOperation captureOperation = new AssaultHabOperation();
					TIHabState tihabState4 = (from x in CS$<>8__locals1.fleet.ref_spaceBody.surfaceBases
						where CS$<>8__locals1.fleet.faction.AI_AtWarWithFaction(x.faction)
						where captureOperation.ValidOperation(CS$<>8__locals1.fleet, x, null)
						select x).MinBy<TIHabState, float>((TIHabState x) => x.AssaultCombatValue(true));
					if (tihabState4 != null && new AssaultHabOperation().GetSuccessChance(CS$<>8__locals1.fleet, tihabState4) > 0.25f)
					{
						tihabState3 = tihabState4;
					}
				}
				if (tihabState3 != null)
				{
					operationTarget = tihabState3;
					return typeof(AssaultHabOperation);
				}
			}
			if (CS$<>8__locals1.fleet.dockedAtHab && CS$<>8__locals1.fleet.ref_hab.AllowsResupply(this.faction, false, false) && CS$<>8__locals1.fleet.NeedsRefuel())
			{
				operationTarget = null;
				return null;
			}
			if (CS$<>8__locals1.fleet.landed && candidateOperations.Contains(typeof(LaunchFromSurfaceOperation)))
			{
				operationTarget = CS$<>8__locals1.fleet.ref_spaceBody.orbits.First<TIOrbitState>((TIOrbitState x) => x.interfaceOrbit);
				return typeof(LaunchFromSurfaceOperation);
			}
			bool flag23 = false;
			if (CS$<>8__locals1.fleet.trajectory == null)
			{
				List<TIGameState> list6 = (from x in list
					where x.ref_habSite != null || x.ref_region != null
					where CS$<>8__locals1.fleet.ref_spaceBody == x.ref_spaceBody
					select x).ToList<TIGameState>();
				bool flag24 = (TIGameState.Valid(this.target()) && this.target().ref_spaceBody == CS$<>8__locals1.fleet.ref_spaceBody) || flag4 || list6.Count > 0;
				if (CS$<>8__locals1.fleet.ref_spaceBody != null)
				{
					TIOrbitState ref_orbit = CS$<>8__locals1.fleet.ref_orbit;
					if (ref_orbit != null && ref_orbit.interfaceOrbit)
					{
						TIGameState ref_spaceBody2 = CS$<>8__locals1.fleet.ref_spaceBody;
						TIGameState tigameState2 = this.target();
						if (ref_spaceBody2 == ((tigameState2 != null) ? tigameState2.ref_spaceBody : null) || flag24)
						{
							if (candidateOperations.Contains(typeof(SurveyPlanetFromFleetOperation)))
							{
								operationTarget = this.target();
								return typeof(SurveyPlanetFromFleetOperation);
							}
							if (candidateOperations.Contains(typeof(FoundFusionOutpostOperation)))
							{
								operationTarget = this.target();
								return typeof(FoundFusionOutpostOperation);
							}
							if (candidateOperations.Contains(typeof(FoundFissionOutpostOperation)))
							{
								operationTarget = this.target();
								return typeof(FoundFissionOutpostOperation);
							}
							if (candidateOperations.Contains(typeof(FoundSolarOutpostOperation)))
							{
								operationTarget = this.target();
								return typeof(FoundSolarOutpostOperation);
							}
							if (candidateOperations.Contains(typeof(BombardOperation_High)))
							{
								if (flag24)
								{
									operationTarget = this.GetBombardmentTarget(CS$<>8__locals1.fleet, list6);
									if (operationTarget != null)
									{
										FactionGoal_AttackWithFleet factionGoal_AttackWithFleet2 = this as FactionGoal_AttackWithFleet;
										float num2;
										if (factionGoal_AttackWithFleet2 != null)
										{
											num2 = factionGoal_AttackWithFleet2.GetDesiredBombardmentValue();
										}
										else
										{
											num2 = FactionGoal_AttackWithFleet.GetDesiredBombardmentValue(CS$<>8__locals1.fleet.faction, operationTarget, 0);
										}
										float num3 = CS$<>8__locals1.fleet.BombardmentValue(CS$<>8__locals1.fleet.ref_spaceBody);
										bool flag25 = false;
										if (list6.Contains(operationTarget) && num3 > 0f)
										{
											TIHabState tihabState5 = operationTarget as TIHabState;
											if (tihabState5 != null && tihabState5.SpaceCombatValue() == 0f)
											{
												flag25 = true;
											}
										}
										if (!flag25 && num3 < num2 * 0.8f)
										{
											flag24 = false;
											operationTarget = null;
											if (!flag2)
											{
												flag23 = true;
											}
										}
									}
								}
								else
								{
									operationTarget = null;
								}
								if (flag24 && operationTarget != null)
								{
									TIHabState ref_hab = operationTarget.ref_hab;
									List<TIHabModuleState> list7 = ((ref_hab != null) ? ref_hab.ActiveCombatModules() : null) ?? new List<TIHabModuleState>();
									if (list7.Count == 0 || base.importance == 20 || !CS$<>8__locals1.fleet.AI_NeedsRepairBadly())
									{
										this.dynamicAttackTarget = operationTarget;
										int num4 = (int)CS$<>8__locals1.fleet.GetFailedAttacksOnTargetValue(operationTarget);
										if (num4 > ((list7.Count == 0) ? 6 : 3))
										{
											this.UnassignFleet();
											operationTarget = null;
											return null;
										}
										int num5;
										if ((operationTarget.isHabState && list7.Count == 0) || (operationTarget.ref_region != null && (!operationTarget.ref_region.antiSpaceDefenses || operationTarget.ref_region.spaceDefenseFacility.ref_factions.Contains(CS$<>8__locals1.fleet.faction))))
										{
											num5 = 0;
										}
										else
										{
											List<float> list8 = new List<float>();
											list8.Add(CS$<>8__locals1.fleet.BombardmentValue(operationTarget.ref_spaceBody, TemplateManager.global.lowBombardmentAltitude_km));
											list8.Add(CS$<>8__locals1.fleet.BombardmentValue(operationTarget.ref_spaceBody, TemplateManager.global.medBombardmentAltitude_km));
											list8.Add(CS$<>8__locals1.fleet.BombardmentValue(operationTarget.ref_spaceBody, TemplateManager.global.highBombardmentAltitude_km));
											num5 = list8.IndexOf(list8.Max());
											int num6;
											int num7;
											float num8;
											if (operationTarget.isHabState)
											{
												num6 = operationTarget.ref_hab.ActiveCombatModules().Max<TIHabModuleState>((TIHabModuleState x) => x.tier);
												num7 = list7.Sum<TIHabModuleState>((TIHabModuleState x) => x.tier);
												num8 = (float)TILaserWeaponTemplate.GetBestHeavyDefenseLaser(operationTarget.ref_faction, operationTarget.ref_spaceBody, num6).wavelength_nm;
											}
											else
											{
												num6 = 3;
												num7 = 3;
												num8 = (float)TILaserWeaponTemplate.GetBestHeavyDefenseLaser(operationTarget.ref_faction, operationTarget.ref_spaceBody, 0).wavelength_nm;
											}
											int num9;
											if (num8 <= 420f)
											{
												num9 = 3 * num6 * num7;
											}
											else if (num8 <= 630f)
											{
												num9 = 2 * num6 * num7;
											}
											else
											{
												num9 = num6 * num7;
											}
											num9 -= (int)CS$<>8__locals1.fleet.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.noseArmorValue) / 20;
											if (num4 == 0)
											{
												if (CS$<>8__locals1.fleet.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.noseWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isMagneticGunWeapon)))
												{
													num5++;
												}
											}
											if (num9 >= 15)
											{
												num5 += 2;
											}
											else if (num9 >= 8)
											{
												num5++;
											}
											if (base.importance > 18)
											{
												num5--;
											}
											num5 -= num4;
											num5 = Mathf.Clamp(num5, 0, 2);
											if (num5 == 0 && num4 >= 3)
											{
												num5 = -1;
											}
										}
										switch (num5)
										{
										case -1:
											break;
										default:
											return typeof(BombardOperation_Low);
										case 1:
											return typeof(BombardOperation_Med);
										case 2:
											return typeof(BombardOperation_High);
										}
									}
								}
							}
							if (CS$<>8__locals1.fleet.IsAlien())
							{
								TISpaceBodyState ref_spaceBody3 = CS$<>8__locals1.fleet.ref_spaceBody;
								if (ref_spaceBody3 != null && ref_spaceBody3.isEarth && candidateOperations.Contains(typeof(AlienEarthSurveillanceOperation)))
								{
									operationTarget = this.actor();
									return typeof(AlienEarthSurveillanceOperation);
								}
							}
						}
					}
				}
				if (CS$<>8__locals1.fleet.ref_orbit == this.target())
				{
					if (candidateOperations.Contains(typeof(FoundFusionPlatformOperation)))
					{
						operationTarget = this.target();
						return typeof(FoundFusionPlatformOperation);
					}
					if (candidateOperations.Contains(typeof(FoundFissionPlatformOperation)))
					{
						operationTarget = this.target();
						return typeof(FoundFissionPlatformOperation);
					}
					if (candidateOperations.Contains(typeof(FoundSolarPlatformOperation)))
					{
						operationTarget = this.target();
						return typeof(FoundSolarPlatformOperation);
					}
					if (candidateOperations.Contains(typeof(FoundAlienSurveillanceRing)))
					{
						operationTarget = this.target();
						return typeof(FoundAlienSurveillanceRing);
					}
					if (candidateOperations.Contains(typeof(FoundAlienSurveillanceOrbital)))
					{
						operationTarget = this.target();
						return typeof(FoundAlienSurveillanceOrbital);
					}
					if (candidateOperations.Contains(typeof(FoundAlienSurveillancePlatform)))
					{
						operationTarget = this.target();
						return typeof(FoundAlienSurveillancePlatform);
					}
				}
				if (this.resupplyHab != null && (this.resupplyHab.deleted || !this.resupplyHab.CanDock(CS$<>8__locals1.fleet, true)))
				{
					this.resupplyHab = null;
				}
				bool flag26 = this.ReadyForTransferToTarget(CS$<>8__locals1.fleet);
				bool flag27 = TIFactionGoalState.OffensiveFleetGoals.Contains(this.GetGoalType()) && this.target() != null && CS$<>8__locals1.fleet.ref_system == this.target().ref_system;
				bool flag28 = false;
				if (this is FactionGoal_FoundHab)
				{
					if (CS$<>8__locals1.fleet.location == this.target())
					{
						flag28 = true;
					}
					else if (this.target().isHabSiteState && CS$<>8__locals1.fleet.ref_spaceBody == this.target().ref_spaceBody)
					{
						TIOrbitState ref_orbit2 = CS$<>8__locals1.fleet.location.ref_orbit;
						if (ref_orbit2 != null && ref_orbit2.interfaceOrbit)
						{
							flag28 = true;
						}
					}
				}
				bool flag29 = flag28 || (this.GetGoalType() == GoalType.JoinFleet && this.target().ref_system == CS$<>8__locals1.fleet.ref_system) || (flag26 && flag27 && !flag23 && !CS$<>8__locals1.fleet.AI_NeedsRepairBadly() && !CS$<>8__locals1.fleet.AI_NeedsRearmBadly());
				if (CS$<>8__locals1.fleet.AssignedGoal().GetGoalType() == GoalType.RepairFleet || (!flag29 && this.fleetOperations.Contains(typeof(RepairFleetOperation)) && CS$<>8__locals1.fleet.NeedsRepair()))
				{
					FactionGoal_Fleet.<>c__DisplayClass79_4 CS$<>8__locals5 = new FactionGoal_Fleet.<>c__DisplayClass79_4();
					CS$<>8__locals5.CS$<>8__locals4 = CS$<>8__locals1;
					CS$<>8__locals5.needsSpecialRepairHab = CS$<>8__locals5.CS$<>8__locals4.fleet.AI_SeekSpecialHabRepairIfNecessary();
					if (this.resupplyHab != null && (!CS$<>8__locals5.<GetBestOperation>g__qualifyingRepairHab|59(this.resupplyHab, false) || !CS$<>8__locals5.CS$<>8__locals4.<GetBestOperation>g__IsSafeToVisit|0(this.resupplyHab)))
					{
						this.resupplyHab = null;
					}
					if (this.resupplyHab == null)
					{
						IEnumerable<TIHabState> enumerable8 = CS$<>8__locals5.CS$<>8__locals4.fleet.ref_naturalSpaceObject.stationsInOrbit.Where<TIHabState>((TIHabState x) => base.<GetBestOperation>g__qualifyingRepairHab|59(x, false)).Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals5.CS$<>8__locals4.<GetBestOperation>g__IsSafeToVisit|0));
						if (enumerable8.Any<TIHabState>())
						{
							IEnumerable<TIHabState> enumerable9 = enumerable8.Where<TIHabState>((TIHabState x) => x.ref_orbit == CS$<>8__locals5.CS$<>8__locals4.fleet.ref_orbit);
							TIHabState tihabState6;
							if (enumerable9 == null)
							{
								tihabState6 = null;
							}
							else
							{
								tihabState6 = enumerable9.MaxBy<TIHabState, int>((TIHabState x) => x.tier);
							}
							this.resupplyHab = tihabState6;
							if (this.resupplyHab == null)
							{
								this.resupplyHab = enumerable8.MaxBy<TIHabState, int>((TIHabState x) => x.tier);
							}
						}
						if (this.resupplyHab == null)
						{
							TISpaceBodyState ref_spaceBody4 = CS$<>8__locals5.CS$<>8__locals4.fleet.ref_spaceBody;
							enumerable8 = ((ref_spaceBody4 != null) ? ref_spaceBody4.surfaceBases.Where<TIHabState>((TIHabState x) => base.<GetBestOperation>g__qualifyingRepairHab|59(x, false)) : null) ?? Enumerable.Empty<TIHabState>().Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals5.CS$<>8__locals4.<GetBestOperation>g__IsSafeToVisit|0));
							if (enumerable8.Any<TIHabState>())
							{
								this.resupplyHab = enumerable8.MaxBy<TIHabState, int>((TIHabState x) => x.tier);
							}
							if (this.resupplyHab == null)
							{
								enumerable8 = this.faction.habs.Where<TIHabState>((TIHabState x) => x.GetSunOrbitingRelatedObject == CS$<>8__locals5.CS$<>8__locals4.fleet.GetSunOrbitingRelatedObject && base.<GetBestOperation>g__qualifyingRepairHab|59(x, false)).Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals5.CS$<>8__locals4.<GetBestOperation>g__IsSafeToVisit|0));
								if (enumerable8.Any<TIHabState>())
								{
									this.resupplyHab = enumerable8.MinBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.AverageDistanceBetweenTwoSpaceObjects_m(x, CS$<>8__locals5.CS$<>8__locals4.fleet));
								}
								else
								{
									double AU2 = CS$<>8__locals5.CS$<>8__locals4.fleet.GetSunOrbitingRelatedObject.semiMajorAxis_AU;
									IEnumerable<TIHabState> enumerable10 = this.faction.habs.Where<TIHabState>((TIHabState x) => base.<GetBestOperation>g__qualifyingRepairHab|59(x, false)).Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals5.CS$<>8__locals4.<GetBestOperation>g__IsSafeToVisit|0));
									if (enumerable10.Any<TIHabState>())
									{
										double minDist2 = enumerable10.Min<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU2));
										enumerable10 = enumerable10.Where<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU2) == minDist2);
										this.resupplyHab = enumerable10.MaxBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.AverageDistanceBetweenTwoSpaceObjects_m(x, x.ref_naturalSpaceObject));
									}
									else
									{
										enumerable10 = this.faction.habs.Where<TIHabState>((TIHabState x) => base.<GetBestOperation>g__qualifyingRepairHab|59(x, false));
										if (enumerable10.Any<TIHabState>())
										{
											double minDist3 = enumerable10.Min<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU2));
											enumerable10 = enumerable10.Where<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU2) == minDist3);
											this.resupplyHab = enumerable10.MaxBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.AverageDistanceBetweenTwoSpaceObjects_m(x, x.ref_naturalSpaceObject));
										}
										else
										{
											enumerable10 = this.faction.habs.Where<TIHabState>((TIHabState x) => base.<GetBestOperation>g__qualifyingRepairHab|59(x, true));
											if (enumerable10.Any<TIHabState>())
											{
												double minDist4 = enumerable10.Min<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU2));
												enumerable10 = enumerable10.Where<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU2) == minDist4);
												this.resupplyHab = enumerable10.MaxBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.AverageDistanceBetweenTwoSpaceObjects_m(x, x.ref_naturalSpaceObject));
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					FactionGoal_Fleet.<>c__DisplayClass79_9 CS$<>8__locals10 = new FactionGoal_Fleet.<>c__DisplayClass79_9();
					CS$<>8__locals10.CS$<>8__locals8 = CS$<>8__locals1;
					bool flag30 = CS$<>8__locals10.CS$<>8__locals8.<GetBestOperation>g__ShouldFlee|2();
					CS$<>8__locals10.isFleeing = false;
					bool flag31 = CS$<>8__locals10.CS$<>8__locals8.fleet.unreachableLocations.Contains(this.target()) && CS$<>8__locals10.CS$<>8__locals8.fleet.AI_NeedsRefuel();
					CS$<>8__locals10.refuellingBecauseCantGetThere = false;
					bool flag32 = CS$<>8__locals10.CS$<>8__locals8.fleet.AI_NeedsRefuelBadly();
					bool flag33 = CS$<>8__locals10.CS$<>8__locals8.fleet.AI_NeedsRearmBadly();
					CS$<>8__locals10.needsRefuelOrRearmBadly = flag32 || flag33;
					if ((this.fleetOperations.Contains(typeof(ResupplyOperation)) && (CS$<>8__locals10.CS$<>8__locals8.fleet.AssignedGoal().GetGoalType() == GoalType.ResupplyFleet || (!flag29 & CS$<>8__locals10.needsRefuelOrRearmBadly) || (CS$<>8__locals10.isFleeing = flag30) || flag23)) || (CS$<>8__locals10.refuellingBecauseCantGetThere = flag31))
					{
						if (this.resupplyHab != null && !CS$<>8__locals10.<GetBestOperation>g__IsValidSafeResupplyHab|81(this.resupplyHab))
						{
							this.resupplyHab = null;
						}
						if (this.resupplyHab == null)
						{
							if (CS$<>8__locals10.isFleeing && this.faction.primaryStation != null && CS$<>8__locals10.CS$<>8__locals8.fleet.ref_system == this.faction.primaryStation.ref_system)
							{
								this.resupplyHab = this.faction.primaryStation;
							}
							IEnumerable<TIHabState> enumerable11 = CS$<>8__locals10.CS$<>8__locals8.fleet.ref_naturalSpaceObject.stationsInOrbit.Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals10.<GetBestOperation>g__IsValidSafeResupplyHab|81));
							if (this.resupplyHab == null && enumerable11.Any<TIHabState>())
							{
								IEnumerable<TIHabState> enumerable12 = enumerable11.Where<TIHabState>((TIHabState x) => x.ref_orbit == CS$<>8__locals10.CS$<>8__locals8.fleet.ref_orbit);
								TIHabState tihabState7;
								if (enumerable12 == null)
								{
									tihabState7 = null;
								}
								else
								{
									tihabState7 = enumerable12.MaxBy<TIHabState, int>((TIHabState x) => x.tier);
								}
								this.resupplyHab = tihabState7;
								if (this.resupplyHab == null)
								{
									this.resupplyHab = enumerable11.MaxBy<TIHabState, int>((TIHabState x) => x.tier);
								}
							}
							if (this.resupplyHab == null)
							{
								TISpaceBodyState ref_spaceBody5 = CS$<>8__locals10.CS$<>8__locals8.fleet.ref_spaceBody;
								enumerable11 = ((ref_spaceBody5 != null) ? ref_spaceBody5.surfaceBases.Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals10.<GetBestOperation>g__IsValidSafeResupplyHab|81)) : null);
								if (enumerable11 != null && enumerable11.Any<TIHabState>())
								{
									this.resupplyHab = enumerable11.MaxBy<TIHabState, int>((TIHabState x) => x.tier);
								}
								if (this.resupplyHab == null)
								{
									enumerable11 = this.faction.habs.Where<TIHabState>((TIHabState x) => x.GetSunOrbitingRelatedObject == CS$<>8__locals10.CS$<>8__locals8.fleet.GetSunOrbitingRelatedObject).Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals10.<GetBestOperation>g__IsValidSafeResupplyHab|81));
									if (enumerable11.Any<TIHabState>())
									{
										this.resupplyHab = enumerable11.MinBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.AverageDistanceBetweenTwoSpaceObjects_m(x, CS$<>8__locals10.CS$<>8__locals8.fleet));
									}
									else
									{
										double AU = CS$<>8__locals10.CS$<>8__locals8.fleet.GetSunOrbitingRelatedObject.semiMajorAxis_AU;
										IEnumerable<TIHabState> enumerable13 = this.faction.habs.Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals10.<GetBestOperation>g__IsValidSafeResupplyHab|81));
										if (enumerable13.Any<TIHabState>())
										{
											double minDist5 = enumerable13.Min<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU));
											enumerable13 = enumerable13.Where<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU) == minDist5);
											this.resupplyHab = enumerable13.MaxBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.AverageDistanceBetweenTwoSpaceObjects_m(x, x.ref_naturalSpaceObject));
										}
										else if (!CS$<>8__locals10.isFleeing)
										{
											enumerable13 = this.faction.habs.Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals10.<GetBestOperation>g__IsValidResupplyHab|80));
											if (enumerable13.Any<TIHabState>())
											{
												double minDist = enumerable13.Min<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU));
												enumerable13 = enumerable13.Where<TIHabState>((TIHabState x) => Mathd.Abs(x.GetSunOrbitingRelatedObject.semiMajorAxis_AU - AU) == minDist);
												this.resupplyHab = enumerable13.MaxBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.AverageDistanceBetweenTwoSpaceObjects_m(x, x.ref_naturalSpaceObject));
											}
										}
										else
										{
											enumerable13 = from x in CS$<>8__locals10.CS$<>8__locals8.fleet.ref_system.habsInSystem
												where x.IsStation
												where x.faction == CS$<>8__locals10.CS$<>8__locals8.<>4__this.faction
												select x;
											if (enumerable13.Any<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals10.<GetBestOperation>g__IsValidResupplyHab|80)))
											{
												enumerable13 = enumerable13.Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals10.<GetBestOperation>g__IsValidResupplyHab|80));
											}
											if (enumerable13.Any<TIHabState>())
											{
												this.resupplyHab = enumerable13.MaxBy<TIHabState, float>((TIHabState x) => x.AggregateDefensiveScore_Station());
											}
											else
											{
												CS$<>8__locals10.isFleeing = false;
											}
										}
									}
								}
							}
						}
					}
				}
				TIHabState resupplyHab = this.resupplyHab;
				if (resupplyHab != null && resupplyHab.IsBase && CS$<>8__locals1.fleet.ref_orbit != null && CS$<>8__locals1.fleet.ref_orbit.ref_spaceBody == this.resupplyHab.ref_spaceBody && candidateOperations.Contains(typeof(LandOnSurfaceOperation)))
				{
					operationTarget = this.resupplyHab.ref_habSite;
					return typeof(LandOnSurfaceOperation);
				}
				if (CS$<>8__locals1.fleet.AI_NeedsRefuelBadly() && CS$<>8__locals1.fleet.AI_InterfleetRefuelCandidate() && candidateOperations.Contains(typeof(InterfleetRefuelOperation)))
				{
					TIHabState resupplyHab2 = this.resupplyHab;
					if (((resupplyHab2 != null) ? resupplyHab2.ref_orbit : null) != CS$<>8__locals1.fleet.ref_orbit && CS$<>8__locals1.fleet.AI_CreatePropellantSharingPlan_Equalization())
					{
						operationTarget = CS$<>8__locals1.fleet;
						return typeof(InterfleetRefuelOperation);
					}
				}
				if (flag17)
				{
					IEnumerable<TICouncilorState> enumerable14 = list5.Where<TICouncilorState>(delegate(TICouncilorState x)
					{
						TIHabState ref_hab3 = x.ref_hab;
						if (ref_hab3 != null && ref_hab3.IsBase)
						{
							TIHabSiteState ref_habSite = x.ref_habSite;
							TIGameState tigameState4 = ((ref_habSite != null) ? ref_habSite.parentBody : null);
							TIOrbitState ref_orbit3 = CS$<>8__locals1.fleet.ref_orbit;
							return tigameState4 == ((ref_orbit3 != null) ? ref_orbit3.barycenter : null);
						}
						return false;
					});
					TICouncilorState ticouncilorState = enumerable14.FirstOrDefault<TICouncilorState>();
					operationTarget = ((ticouncilorState != null) ? ticouncilorState.ref_habSite : null);
					if (operationTarget != null)
					{
						return typeof(LandOnSurfaceOperation);
					}
				}
				bool flag34 = CS$<>8__locals1.fleet.RallyingFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.transferAssigned && (x.trajectory.arrivalTime - TITimeState.Now()).TotalDays < 3.0);
				bool flag35 = false;
				if (this.resupplyHab != null && CS$<>8__locals1.fleet.dockedLocation == this.resupplyHab)
				{
					if ((flag18 || flag19) && this.resupplyHab.AllowsResupply(this.faction, false, false))
					{
						flag35 = true;
					}
					else if (flag20 && this.resupplyHab.CanPartiallyRepairFleet(CS$<>8__locals1.fleet))
					{
						flag35 = true;
					}
				}
				bool flag36 = false;
				if (flag4)
				{
					flag36 = true;
				}
				if (candidateOperations.Contains(typeof(TransferOperation)) && ((flag26 && !flag36) || this.resupplyHab != null) && !flag34 && !flag35)
				{
					operationTarget = null;
					if (this.resupplyHab != null && CS$<>8__locals1.fleet.ref_hab != this.resupplyHab)
					{
						if (this.resupplyHab.IsStation)
						{
							operationTarget = this.resupplyHab;
						}
						else
						{
							operationTarget = this.resupplyHab.ref_spaceBody.orbits.First<TIOrbitState>((TIOrbitState x) => x.interfaceOrbit);
						}
					}
					else if (flag26)
					{
						if (flag17)
						{
							TICouncilorState ticouncilorState2 = list5.First<TICouncilorState>((TICouncilorState x) => x.location.ref_fleet != CS$<>8__locals1.fleet);
							if (ticouncilorState2.OnEarth)
							{
								operationTarget = this.faction.LEOStations.MaxBy<TIHabState, float>((TIHabState x) => x.SpaceCombatValue());
							}
							else if (ticouncilorState2.AtABase)
							{
								if (ticouncilorState2.ref_hab.faction.permanentAlly(this.faction) || ticouncilorState2.ref_hab.SpaceCombatValue() == 0f)
								{
									operationTarget = ticouncilorState2.ref_spaceBody.orbits.First<TIOrbitState>((TIOrbitState x) => x.interfaceOrbit);
								}
							}
							else if (ticouncilorState2.OnAStation)
							{
								if (ticouncilorState2.ref_hab.faction.permanentAlly(this.faction) || ticouncilorState2.ref_hab.SpaceCombatValue() == 0f)
								{
									operationTarget = ticouncilorState2.ref_hab;
								}
							}
							else if (ticouncilorState2.OnAShip && ticouncilorState2.ref_ship.faction.permanentAlly(this.faction))
							{
								operationTarget = ticouncilorState2.ref_fleet;
							}
						}
						else if (this.flyByLocation != null)
						{
							operationTarget = this.flyByLocation;
							this.flyByLocation = null;
						}
						else
						{
							bool flag37 = false;
							bool flag38 = CS$<>8__locals1.fleet.location != null && this.target() != null && CS$<>8__locals1.fleet.location.ref_system == this.target().ref_system;
							bool flag39 = TIFactionGoalState.OffensiveFleetGoals.Contains(this.GetGoalType());
							bool flag40;
							if (this.target() != null && flag39 && !flag38 && this.target().ref_system != GameStateManager.Sol())
							{
								TISpaceFleetState ref_fleet = this.target().ref_fleet;
								flag40 = ref_fleet == null || !ref_fleet.inTransfer;
							}
							else
							{
								flag40 = false;
							}
							bool flag41 = flag40;
							bool flag42 = this.target() != null && (this.target().ref_nation != null || this.target().ref_habSite != null || this.target().isSpaceBodyState);
							if (flag42 && factionGoal_SecureEarthSpace != null)
							{
								TISpaceBodyState ref_system = CS$<>8__locals1.fleet.ref_system;
								if (ref_system != null && ref_system.isEarth && CS$<>8__locals1.fleet.dockedAtStation && this.faction.permanentAlly(CS$<>8__locals1.fleet.dockedLocation.ref_hab.faction))
								{
									operationTarget = null;
									return null;
								}
							}
							if (flag42 || flag41)
							{
								TIOrbitState tiorbitState;
								if ((tiorbitState = this.target().ref_naturalSpaceObject.orbits.OrderByDescending<TIOrbitState, bool>((TIOrbitState x) => x.interfaceOrbit).FirstOrDefault<TIOrbitState>()) == null)
								{
									tiorbitState = this.target().ref_system.orbits.OrderByDescending<TIOrbitState, bool>((TIOrbitState x) => x.interfaceOrbit).FirstOrDefault<TIOrbitState>();
								}
								TIOrbitState tiorbitState2 = tiorbitState;
								if (CS$<>8__locals1.fleet.location == tiorbitState2)
								{
									flag37 = true;
								}
								else
								{
									operationTarget = tiorbitState2;
								}
							}
							else
							{
								TIGameState tigameState3 = this.target();
								if (tigameState3 != null && tigameState3.isLagrangePointState)
								{
									TIOrbitState tiorbitState3 = this.target().ref_lagrangePoint.orbits[0];
									if (CS$<>8__locals1.fleet.location == tiorbitState3)
									{
										flag37 = true;
									}
									else
									{
										operationTarget = tiorbitState3;
									}
								}
							}
							if (this.target() != null && (flag37 || operationTarget == null))
							{
								GoalType goalType = this.GetGoalType();
								if (goalType != GoalType.DefendWithFleet)
								{
									if (goalType - GoalType.AttackWithFleet <= 1)
									{
										operationTarget = this.target();
										if (this.target().isOrbitState)
										{
											operationTarget = this.target().ref_orbit.assetsInOrbit.Where<TISpaceAssetState>((TISpaceAssetState x) => CS$<>8__locals1.<>4__this.faction.AI_AtWarWithFaction(x.ref_faction)).MinBy<TISpaceAssetState, float>((TISpaceAssetState x) => x.SpaceCombatValue());
										}
										else if (this.target().isNaturalSpaceObjectState)
										{
											operationTarget = (from x in this.target().ref_naturalSpaceObject.orbits.SelectMany<TIOrbitState, TISpaceAssetState>((TIOrbitState x) => x.assetsInOrbit)
												where CS$<>8__locals1.<>4__this.faction.AI_AtWarWithFaction(x.ref_faction)
												select x).MinBy<TISpaceAssetState, float>((TISpaceAssetState x) => x.SpaceCombatValue());
										}
									}
									else
									{
										operationTarget = this.target();
									}
								}
								else
								{
									TIHabState ref_hab2 = this.target().ref_hab;
									if (ref_hab2 != null && ref_hab2.IsStation)
									{
										operationTarget = this.target();
									}
								}
							}
						}
					}
					if (operationTarget != null && CS$<>8__locals1.fleet.location != operationTarget)
					{
						return typeof(TransferOperation);
					}
				}
			}
			else
			{
				bool inTransfer = CS$<>8__locals1.fleet.inTransfer;
			}
			operationTarget = null;
			return null;
		}

		// Token: 0x04002222 RID: 8738
		public LearnedPerformanceRequirements learnedPerformanceRequirements = new LearnedPerformanceRequirements();

		// Token: 0x04002223 RID: 8739
		private Trajectory cachedExampleTrajectory;

		// Token: 0x04002224 RID: 8740
		private TIDateTime exampleTrajectoryCacheDatestamp;

		// Token: 0x04002225 RID: 8741
		protected static readonly List<Type> coreFleetOpsList = new List<Type>
		{
			typeof(TransferOperation),
			typeof(RepairFleetOperation),
			typeof(ResupplyOperation),
			typeof(InterfleetRefuelOperation),
			typeof(LaunchFromSurfaceOperation),
			typeof(LandOnSurfaceOperation)
		};

		// Token: 0x04002226 RID: 8742
		public List<TISpaceFleetState> pendingFleets = new List<TISpaceFleetState>();

		// Token: 0x04002227 RID: 8743
		public TIGameState dynamicAttackTarget;

		// Token: 0x0400222A RID: 8746
		protected const float bombardmentDefenseMultiplier = 0.1f;

		// Token: 0x0400222B RID: 8747
		private float cachedDesiredFleetCombatValue;

		// Token: 0x0400222C RID: 8748
		private TIDateTime desiredFleetCombatValueCachedDate;
	}
}
