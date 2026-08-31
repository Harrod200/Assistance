using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000758 RID: 1880
	public class FactionGoal_TransportCouncilorsWithFleet : FactionGoal_FleetCouncilorGoal
	{
		// Token: 0x060030F5 RID: 12533 RVA: 0x001084AC File Offset: 0x001066AC
		public FactionGoal_TransportCouncilorsWithFleet()
		{
			if (this.assignedCouncilors == null)
			{
				this.assignedCouncilors = new List<TICouncilorState>();
			}
		}

		// Token: 0x060030F6 RID: 12534 RVA: 0x00108510 File Offset: 0x00106710
		public FactionGoal_TransportCouncilorsWithFleet(TIFactionState faction, int importance, List<TICouncilorState> councilors, TIGameState destination)
		{
			this.faction = faction;
			base.SetImportance(importance);
			this.assignedCouncilors = new List<TICouncilorState>(councilors);
			base.councilorDestination = destination;
			if (faction.IsActiveHumanFaction)
			{
				this.preferredShipRoles.Add(ShipRole.TroopCarrier, 0.1f);
			}
		}

		// Token: 0x060030F7 RID: 12535 RVA: 0x0010859C File Offset: 0x0010679C
		public static FactionGoal_TransportCouncilorsWithFleet CreateGoal(FactionGoal_TransportCouncilorsWithFleet p)
		{
			FactionGoal_TransportCouncilorsWithFleet factionGoal_TransportCouncilorsWithFleet = GameStateManager.CreateNewGameState<FactionGoal_TransportCouncilorsWithFleet>();
			factionGoal_TransportCouncilorsWithFleet.councilorDestination = p.councilorDestination;
			factionGoal_TransportCouncilorsWithFleet.assignedCouncilors = new List<TICouncilorState>(p.assignedCouncilors);
			return factionGoal_TransportCouncilorsWithFleet;
		}

		// Token: 0x060030F8 RID: 12536 RVA: 0x001085C0 File Offset: 0x001067C0
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_TransportCouncilorsWithFleet>(base.ID, false);
		}

		// Token: 0x060030F9 RID: 12537 RVA: 0x001085CF File Offset: 0x001067CF
		public override GoalType GetGoalType()
		{
			return GoalType.TransportCouncilorsViaFleet;
		}

		// Token: 0x060030FA RID: 12538 RVA: 0x001085D3 File Offset: 0x001067D3
		public override TIGameState actor()
		{
			return base.assignedFleet;
		}

		// Token: 0x060030FB RID: 12539 RVA: 0x001085DB File Offset: 0x001067DB
		public override TIGameState target()
		{
			return base.councilorDestination;
		}

		// Token: 0x060030FC RID: 12540 RVA: 0x001085E3 File Offset: 0x001067E3
		public override TIGameState location()
		{
			return base.councilorDestination;
		}

		// Token: 0x060030FD RID: 12541 RVA: 0x001085EB File Offset: 0x001067EB
		public override TIGameState goalProduct()
		{
			return base.assignedFleet;
		}

		// Token: 0x060030FE RID: 12542 RVA: 0x001085F3 File Offset: 0x001067F3
		public override bool ValidNewGoal()
		{
			return base.councilorDestination != null && this.faction.CanExplore(base.councilorDestination.ref_spaceObject);
		}

		// Token: 0x060030FF RID: 12543 RVA: 0x0010861C File Offset: 0x0010681C
		public override bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget)
		{
			FactionGoal_TransportCouncilorsWithFleet factionGoal_TransportCouncilorsWithFleet = testGoal as FactionGoal_TransportCouncilorsWithFleet;
			if (factionGoal_TransportCouncilorsWithFleet == null)
			{
				return false;
			}
			if (this.assignedCouncilors.NotAll<TICouncilorState>(new Func<TICouncilorState, bool>(factionGoal_TransportCouncilorsWithFleet.assignedCouncilors.Contains)))
			{
				return false;
			}
			if (testTarget == null)
			{
				testTarget = testGoal.target();
			}
			return testTarget == this.location() || (testTarget.isRegionState && this.location().isRegionState && testTarget.ref_spaceBody == this.location().ref_spaceBody);
		}

		// Token: 0x06003100 RID: 12544 RVA: 0x001086A4 File Offset: 0x001068A4
		public override bool InProgress()
		{
			return base.assignedFleet != null && this.assignedCouncilors.Count > 0;
		}

		// Token: 0x06003101 RID: 12545 RVA: 0x001086C4 File Offset: 0x001068C4
		public override bool ShouldDiscardGoal()
		{
			if (TIGameState.Valid(base.councilorDestination) && base.importance > 0 && this.assignedCouncilors.Count != 0)
			{
				return this.assignedCouncilors.All<TICouncilorState>((TICouncilorState x) => x == null || x.archived || x.status != CouncilorStatus.Active);
			}
			return true;
		}

		// Token: 0x06003102 RID: 12546 RVA: 0x00108720 File Offset: 0x00106920
		public override bool GoalFulfilled()
		{
			return this.assignedCouncilors.All<TICouncilorState>(delegate(TICouncilorState x)
			{
				if (x.location == base.councilorDestination)
				{
					return true;
				}
				if (x.OnEarth && base.councilorDestination.isRegionState)
				{
					TISpaceBodyState ref_spaceBody = base.councilorDestination.ref_spaceBody;
					return ref_spaceBody != null && ref_spaceBody.isEarth;
				}
				return false;
			});
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x06003103 RID: 12547 RVA: 0x00108739 File Offset: 0x00106939
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003104 RID: 12548 RVA: 0x0010873C File Offset: 0x0010693C
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x06003105 RID: 12549 RVA: 0x0010873F File Offset: 0x0010693F
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_TransportCouncilorsWithFleet.fleetOps;
			}
		}

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x06003106 RID: 12550 RVA: 0x00108746 File Offset: 0x00106946
		public override bool buildFleetsSequentially
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003107 RID: 12551 RVA: 0x00108749 File Offset: 0x00106949
		public override void ChangeTarget(TIGameState newTarget)
		{
			base.councilorDestination = newTarget;
		}

		// Token: 0x06003108 RID: 12552 RVA: 0x00108752 File Offset: 0x00106952
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x06003109 RID: 12553 RVA: 0x00108755 File Offset: 0x00106955
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.CouncilorTransport;
		}

		// Token: 0x0600310A RID: 12554 RVA: 0x00108758 File Offset: 0x00106958
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return this.preferredShipRoles;
		}

		// Token: 0x0600310B RID: 12555 RVA: 0x00108760 File Offset: 0x00106960
		public override bool ReadyForTransferToTarget(TISpaceFleetState fleet)
		{
			if (!TIGameState.Valid(fleet))
			{
				return false;
			}
			IEnumerable<TICouncilorState> enumerable = this.assignedCouncilors.Where<TICouncilorState>((TICouncilorState x) => x.location.ref_fleet != fleet);
			bool flag = fleet.SpaceCombatValue() >= base.desiredFleetCombatValue;
			bool flag2 = fleet.CanFulfillGoal(this, false);
			if (enumerable.Any<TICouncilorState>())
			{
				return !enumerable.First<TICouncilorState>().location.ref_system.isEarth || (flag2 && flag);
			}
			return flag2 && flag;
		}

		// Token: 0x0600310C RID: 12556 RVA: 0x001087EE File Offset: 0x001069EE
		public override float ComputeDesiredFleetCombatValue()
		{
			if (this.ShouldPerformMissionMinimallyArmed)
			{
				return 0f;
			}
			return base.ComputeDesiredFleetCombatValue() * 1f;
		}

		// Token: 0x0600310D RID: 12557 RVA: 0x0010880A File Offset: 0x00106A0A
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x0600310E RID: 12558 RVA: 0x00108814 File Offset: 0x00106A14
		public override void DailyGoalMaintenance()
		{
			base.DailyGoalMaintenance();
			if (TITimeState.Now().day % 9 != 0)
			{
				return;
			}
			if (this.faction.IsAlienFaction)
			{
				TIGameState tigameState = this.target();
				if (((tigameState != null) ? tigameState.ref_region : null) != null)
				{
					TIFactionGoalState tifactionGoalState = (from x in this.faction.GoalsOfType(GoalType.TransportCouncilorsViaFleet, false, true)
						orderby x.importance descending, x.assignedDate
						select x).FirstOrDefault<TIFactionGoalState>();
					if (this == tifactionGoalState)
					{
						bool flag = !AIEvaluators.ShouldAliensGoLoud();
						int num = this.faction.councilors.Count<TICouncilorState>((TICouncilorState x) => x.OnEarth || x.OnAShip);
						int num2 = this.faction.councilors.Count - num;
						if (num == 0 || (flag && num2 > 0))
						{
							int num3 = 19;
							if (flag)
							{
								num3++;
							}
							base.SetImportance(num3);
						}
					}
					List<TIGameState> possibleTargets = new AlienCrashdownOperation().GetPossibleTargets(this.faction, null);
					bool flag2 = false;
					if (possibleTargets.Count == 0)
					{
						flag2 = true;
					}
					else if (!possibleTargets.Contains(this.target().ref_region))
					{
						TIRegionState tiregionState = AIEvaluators.SelectAlienCrashdownRegion(true, false);
						if (tiregionState != null && possibleTargets.Contains(tiregionState))
						{
							this.ChangeTarget(tiregionState);
							return;
						}
						flag2 = true;
					}
					if (flag2)
					{
						TIRegionState tiregionState2 = AIEvaluators.SelectAlienCrashdownRegion(true, true);
						if (tiregionState2.antiSpaceDefenses)
						{
							if (!(from x in this.faction.GoalsOfType(GoalType.AttackWithFleet, false, true)
								where x.target() is TISpaceDefensesFacilityState
								select x).ToList<TIFactionGoalState>().Any<TIFactionGoalState>())
							{
								int num4 = Mathf.Min(base.importance + 1, 19);
								this.faction.AddGoal(new FactionGoal_AttackWithFleet(this.faction, num4, tiregionState2.spaceDefenseFacility, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
								return;
							}
						}
						else
						{
							Log.Debug("There are no valid crashdown locations for alien armies and the AI does not know how to deal with it.", Array.Empty<object>());
						}
					}
				}
			}
		}

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x0600310F RID: 12559 RVA: 0x00108A32 File Offset: 0x00106C32
		public override bool WantsAdditionalCouncilors
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003110 RID: 12560 RVA: 0x00108A35 File Offset: 0x00106C35
		public override bool ShouldUnassignCouncilor(TICouncilorState councilor)
		{
			return !TIGameState.Valid(councilor);
		}

		// Token: 0x06003111 RID: 12561 RVA: 0x00108A40 File Offset: 0x00106C40
		[return: TupleElementNames(new string[] { "Mission", "Target" })]
		public override IEnumerable<ValueTuple<TIMissionTemplate, TIGameState>> GetMissionOptions(TICouncilorState councilor)
		{
			if (base.assignedFleet != null && base.assignedFleet.ref_spaceBody == councilor.ref_spaceBody)
			{
				TIMissionTemplate timissionTemplate;
				if (councilor.OnEarth)
				{
					timissionTemplate = TIFactionState.orbitMission;
				}
				else
				{
					timissionTemplate = TIFactionState.transferMission;
				}
				if (timissionTemplate.GetValidTargets(councilor).Intersect<TIGameState>(base.assignedFleet.ships).Any<TIGameState>())
				{
					IEnumerable<TISpaceShipState> enumerable = base.assignedFleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.role == ShipRole.CouncilorTransport);
					TISpaceShipState tispaceShipState;
					if (enumerable == null)
					{
						tispaceShipState = null;
					}
					else
					{
						tispaceShipState = enumerable.MaxBy<TISpaceShipState, float>((TISpaceShipState x) => x.AssaultCombatValue(false));
					}
					TISpaceShipState tispaceShipState2 = tispaceShipState;
					if (tispaceShipState2 == null)
					{
						IEnumerable<TISpaceShipState> enumerable2 = base.assignedFleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.role == ShipRole.TroopCarrier || (this.faction.IsAlienFaction && x.HasSpecialModuleRule(SpecialModuleRule.Crashdown, false)));
						TISpaceShipState tispaceShipState3;
						if (enumerable2 == null)
						{
							tispaceShipState3 = null;
						}
						else
						{
							tispaceShipState3 = enumerable2.MaxBy<TISpaceShipState, float>((TISpaceShipState x) => x.AssaultCombatValue(false));
						}
						tispaceShipState2 = tispaceShipState3;
						if (tispaceShipState2 == null && this.faction.IsActiveHumanFaction)
						{
							tispaceShipState2 = (from x in base.assignedFleet.ships
								orderby x.combatant descending, !x.damaged descending, Mathf.Pow(x.currentMaxDeltaV_kps, 1.5f) * x.cruiseAcceleration_mps2 descending
								select x).FirstOrDefault<TISpaceShipState>();
						}
					}
					if (tispaceShipState2 != null)
					{
						return Enumerable.Empty<ValueTuple<TIMissionTemplate, TIGameState>>().Append(new ValueTuple<TIMissionTemplate, TIGameState>(TIFactionState.orbitMission, tispaceShipState2));
					}
				}
			}
			return base.GetMissionOptions(councilor);
		}

		// Token: 0x04002271 RID: 8817
		public static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList) { typeof(AlienCrashdownOperation) };

		// Token: 0x04002272 RID: 8818
		private readonly Dictionary<ShipRole, float> preferredShipRoles = new Dictionary<ShipRole, float>
		{
			{
				ShipRole.LS_Penetrator,
				1f
			},
			{
				ShipRole.LM_Interdictor,
				1f
			},
			{
				ShipRole.LL_Intruder,
				1f
			},
			{
				ShipRole.LM_Protector,
				1f
			}
		};
	}
}
