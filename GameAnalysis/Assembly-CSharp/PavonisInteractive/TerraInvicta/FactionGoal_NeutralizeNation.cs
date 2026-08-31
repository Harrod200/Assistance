using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200072E RID: 1838
	public class FactionGoal_NeutralizeNation : FactionGoal_Nation
	{
		// Token: 0x06002DAD RID: 11693 RVA: 0x000FA95F File Offset: 0x000F8B5F
		public FactionGoal_NeutralizeNation()
		{
		}

		// Token: 0x06002DAE RID: 11694 RVA: 0x000FA967 File Offset: 0x000F8B67
		public FactionGoal_NeutralizeNation(TIFactionState faction, int importance, TINationState nation, TIObjectiveTemplate objective = null)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.nation = nation;
			this.objective = objective;
		}

		// Token: 0x06002DAF RID: 11695 RVA: 0x000FA98C File Offset: 0x000F8B8C
		public static FactionGoal_NeutralizeNation CreateGoal(FactionGoal_NeutralizeNation prospectiveGoal)
		{
			FactionGoal_NeutralizeNation factionGoal_NeutralizeNation = GameStateManager.CreateNewGameState<FactionGoal_NeutralizeNation>();
			factionGoal_NeutralizeNation.nation = prospectiveGoal.nation;
			factionGoal_NeutralizeNation.objective = prospectiveGoal.objective;
			return factionGoal_NeutralizeNation;
		}

		// Token: 0x06002DB0 RID: 11696 RVA: 0x000FA9AB File Offset: 0x000F8BAB
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_NeutralizeNation>(base.ID, false);
		}

		// Token: 0x06002DB1 RID: 11697 RVA: 0x000FA9BA File Offset: 0x000F8BBA
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002DB2 RID: 11698 RVA: 0x000FA9BD File Offset: 0x000F8BBD
		public override bool NationPrioritiesGoal()
		{
			return true;
		}

		// Token: 0x06002DB3 RID: 11699 RVA: 0x000FA9C0 File Offset: 0x000F8BC0
		public override GoalType GetGoalType()
		{
			return GoalType.NeutralizeNation;
		}

		// Token: 0x06002DB4 RID: 11700 RVA: 0x000FA9C4 File Offset: 0x000F8BC4
		public override TIGameState actor()
		{
			if (!(base.nation.executiveFaction == this.faction))
			{
				return this.faction.ref_gameState;
			}
			return base.nation.ref_gameState;
		}

		// Token: 0x06002DB5 RID: 11701 RVA: 0x000FA9F5 File Offset: 0x000F8BF5
		public override TIGameState target()
		{
			return base.nation;
		}

		// Token: 0x06002DB6 RID: 11702 RVA: 0x000FA9FD File Offset: 0x000F8BFD
		public override TIGameState location()
		{
			return base.nation.capital;
		}

		// Token: 0x06002DB7 RID: 11703 RVA: 0x000FAA0A File Offset: 0x000F8C0A
		public override TIGameState goalProduct()
		{
			return base.nation;
		}

		// Token: 0x06002DB8 RID: 11704 RVA: 0x000FAA12 File Offset: 0x000F8C12
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002DB9 RID: 11705 RVA: 0x000FAA27 File Offset: 0x000F8C27
		public override bool InProgress()
		{
			return true;
		}

		// Token: 0x06002DBA RID: 11706 RVA: 0x000FAA2A File Offset: 0x000F8C2A
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.nation == null || !base.nation.extant;
		}

		// Token: 0x06002DBB RID: 11707 RVA: 0x000FAA53 File Offset: 0x000F8C53
		public override bool GoalFulfilled()
		{
			return !base.nation.extant || (this.faction.IsAlienFaction && base.nation.CouncilControlPointFraction(GameStateManager.AlienProxy(), true, false) >= 1f);
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x06002DBC RID: 11708 RVA: 0x000FAA8F File Offset: 0x000F8C8F
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_NeutralizeNation.missionModifiers;
			}
		}

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x06002DBD RID: 11709 RVA: 0x000FAA96 File Offset: 0x000F8C96
		public override List<Type> armyOperations
		{
			get
			{
				return FactionGoal_NeutralizeNation.armyOps;
			}
		}

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06002DBE RID: 11710 RVA: 0x000FAA9D File Offset: 0x000F8C9D
		public override List<PolicyType> policiesAsNation
		{
			get
			{
				return PolicyManager.WeakenNationPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x06002DBF RID: 11711 RVA: 0x000FAAA4 File Offset: 0x000F8CA4
		public override List<PolicyType> factionLevelPoliciesAsNation
		{
			get
			{
				return PolicyManager.DegradeRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x06002DC0 RID: 11712 RVA: 0x000FAAAB File Offset: 0x000F8CAB
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return PolicyManager.DegradeRelationsPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x06002DC1 RID: 11713 RVA: 0x000FAAB2 File Offset: 0x000F8CB2
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.DegradeRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x06002DC2 RID: 11714 RVA: 0x000FAAB9 File Offset: 0x000F8CB9
		public override Dictionary<PriorityType, int> prioritiesAsNation
		{
			get
			{
				return FactionGoal_NeutralizeNation.prioritySettings;
			}
		}

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x06002DC3 RID: 11715 RVA: 0x000FAAC0 File Offset: 0x000F8CC0
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_NeutralizeNation.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x06002DC4 RID: 11716 RVA: 0x000FAAC7 File Offset: 0x000F8CC7
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x06002DC5 RID: 11717 RVA: 0x000FAACC File Offset: 0x000F8CCC
		public static bool ShouldNeutralizeNation(TIFactionState faction, TINationState enemy)
		{
			if (faction.enemyWarFactions.Contains(enemy.executiveFaction))
			{
				if (!faction.IsAlienFaction && enemy.alienNation)
				{
					return true;
				}
				if ((enemy.numControlPoints >= 4 || enemy.numStandardArmies > 0 || enemy.numNuclearWeapons > 0 || enemy.boostIncome_month_dekatons >= 1f || enemy.missionControl > 0) && enemy.CountFactionControlPoints(faction, true, true, true) == 0)
				{
					if (enemy.EnemyControlPoints(enemy.executiveFaction).All<TIControlPoint>((TIControlPoint x) => x.defended) && faction.FindGoals(TIFactionGoalState.CaptureNationGoals, faction, enemy, TIFactionState.GoalFilter.none, true).Count == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06002DC6 RID: 11718 RVA: 0x000FAB8C File Offset: 0x000F8D8C
		public override void DailyGoalMaintenance()
		{
			if (this.faction.IsAlienFaction)
			{
				return;
			}
			if (!FactionGoal_NeutralizeNation.ShouldNeutralizeNation(this.faction, base.nation))
			{
				base.SetImportance(0);
				return;
			}
			IEnumerable<FactionGoal_AttackWithFleet> enumerable = from x in this.faction.GoalsOfType(GoalType.AttackWithFleet, true, true).ConvertAll<FactionGoal_AttackWithFleet>((TIFactionGoalState x) => x as FactionGoal_AttackWithFleet)
				where x.bombardmentGoal && x.target().hasEarthMapObject
				select x;
			int num = enumerable.Count<FactionGoal_AttackWithFleet>();
			int num2 = this.faction.fleets.Count<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				TIOrbitState ref_orbit = x.ref_orbit;
				return ref_orbit != null && ref_orbit.isEarthLEO && x.AssignedGoal() == null && x.BombardmentValue(x.ref_spaceBody) > 0f;
			});
			int num3 = Mathf.Clamp(2 - num, 0, num2);
			IOrderedEnumerable<TIArmyState> orderedEnumerable = from x in base.nation.armies
				orderby x.IsFighting(false) descending, x.strength
				select x;
			if (num3 == 0)
			{
				IEnumerable<FactionGoal_AttackWithFleet> enumerable2 = enumerable.Where<FactionGoal_AttackWithFleet>((FactionGoal_AttackWithFleet x) => x.target().isArmyState);
				if (this.faction.GoalsOfType(GoalType.NeutralizeNation, false, true).Max<TIFactionGoalState>((TIFactionGoalState x) => x.importance) == base.importance && num2 == 0)
				{
					num3 = 1;
				}
				else if (orderedEnumerable.Any<TIArmyState>() && enumerable2.Count<FactionGoal_AttackWithFleet>() == 0)
				{
					num3 = 1;
				}
			}
			int num4 = 0;
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			List<TISpaceFleetState> list2 = new List<TISpaceFleetState>();
			int num5 = Mathf.Min(base.importance, 18);
			foreach (TIArmyState tiarmyState in orderedEnumerable)
			{
				TIFactionState faction = tiarmyState.faction;
				if ((faction == null || !faction.permanentAlly(this.faction)) && num4 < num3)
				{
					list.Add(this.faction.AddGoal(new FactionGoal_AttackWithFleet(this.faction, num5, tiarmyState, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null));
					num4++;
				}
				else
				{
					List<FactionGoal_AttackWithFleet> list3 = this.faction.GoalsWithTarget(tiarmyState, GoalType.AttackWithFleet, false).ConvertAll<FactionGoal_AttackWithFleet>((TIFactionGoalState x) => x as FactionGoal_AttackWithFleet);
					List<TISpaceFleetState> list4 = (from x in list3
						select x.assignedFleet into x
						where TIGameState.Valid(x)
						select x).ToList<TISpaceFleetState>();
					if (list4.Count > 0)
					{
						list2.AddRangeUnique<TISpaceFleetState>(list4);
					}
					list3.ForEach(delegate(FactionGoal_AttackWithFleet x)
					{
						x.SetImportance(0);
					});
				}
			}
			foreach (TIRegionSpaceFacilityState tiregionSpaceFacilityState in from x in base.nation.regions.SelectMany<TIRegionState, TIRegionSpaceFacilityState>((TIRegionState x) => x.spaceFacilities)
				where x.Extant()
				orderby x.region.antiSpaceDefenses, x.spaceFacilityType == SpaceFacilityType.missionControlFacility descending, x.GetAIValuation() descending
				select x)
			{
				if (num4 < num3)
				{
					list.Add(this.faction.AddGoal(new FactionGoal_AttackWithFleet(this.faction, num5 - 1, tiregionSpaceFacilityState, false, null, false), HandleDuplicateGoalRule.ResetImportance, null));
					num4++;
				}
				else
				{
					List<FactionGoal_AttackWithFleet> list5 = this.faction.GoalsWithTarget(tiregionSpaceFacilityState, GoalType.AttackWithFleet, false).ConvertAll<FactionGoal_AttackWithFleet>((TIFactionGoalState x) => x as FactionGoal_AttackWithFleet);
					List<TISpaceFleetState> list6 = (from x in list5
						select x.assignedFleet into x
						where TIGameState.Valid(x)
						select x).ToList<TISpaceFleetState>();
					if (list6.Count > 0)
					{
						list2.AddRangeUnique<TISpaceFleetState>(list6);
					}
					list5.ForEach(delegate(FactionGoal_AttackWithFleet x)
					{
						x.SetImportance(0);
					});
				}
			}
			foreach (TISpaceFleetState tispaceFleetState in list2.ToList<TISpaceFleetState>())
			{
				foreach (TIFactionGoalState tifactionGoalState in list.ToList<TIFactionGoalState>())
				{
					FactionGoal_Fleet factionGoal_Fleet = tifactionGoalState as FactionGoal_Fleet;
					if (factionGoal_Fleet.assignedFleet == null && tispaceFleetState.CanFulfillGoal(factionGoal_Fleet, false))
					{
						factionGoal_Fleet.AssignFleet(tispaceFleetState);
						list.Remove(tifactionGoalState);
						break;
					}
				}
			}
		}

		// Token: 0x06002DC7 RID: 11719 RVA: 0x000FB178 File Offset: 0x000F9378
		public override void OnGoalRemoved()
		{
			foreach (TIRegionSpaceFacilityState tiregionSpaceFacilityState in base.nation.regions.SelectMany<TIRegionState, TIRegionSpaceFacilityState>((TIRegionState x) => x.spaceFacilities))
			{
				this.faction.GoalsWithTarget(tiregionSpaceFacilityState, GoalType.AttackWithFleet, false).ForEach(delegate(TIFactionGoalState x)
				{
					x.SetImportance(0);
				});
			}
			foreach (TIArmyState tiarmyState in base.nation.armies)
			{
				this.faction.GoalsWithTarget(tiarmyState, GoalType.AttackWithFleet, false).ForEach(delegate(TIFactionGoalState x)
				{
					x.SetImportance(0);
				});
			}
		}

		// Token: 0x040021FA RID: 8698
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 0f },
			{ "Crackdown", 5f },
			{ "GainInfluence", 0f },
			{ "EnthrallElites", 1f },
			{ "EnthrallPublic", 1f },
			{ "EnthrallUnalignedElites", 1f },
			{ "Propaganda", 0f },
			{ "Purge", 0f },
			{ "Unrest", 10f },
			{ "SabotageFacilities", 10f },
			{ "TerrorizeRegion", 10f },
			{ "Stabilize", 0f },
			{ "Advise", 0f },
			{ "AssumeControl", 5f },
			{ "BuildFacility", 5f }
		};

		// Token: 0x040021FB RID: 8699
		private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int>
		{
			{
				PriorityType.Economy,
				0
			},
			{
				PriorityType.Welfare,
				0
			},
			{
				PriorityType.Military_BuildArmy,
				0
			},
			{
				PriorityType.Military_BuildNuclearWeapons,
				0
			},
			{
				PriorityType.Military_BuildSpaceDefenses,
				0
			},
			{
				PriorityType.Military_InitiateNuclearProgram,
				0
			},
			{
				PriorityType.Knowledge,
				0
			},
			{
				PriorityType.LaunchFacilities,
				0
			},
			{
				PriorityType.Military,
				0
			},
			{
				PriorityType.MissionControl,
				0
			},
			{
				PriorityType.Funding,
				0
			},
			{
				PriorityType.Civilian_InitiateSpaceflightProgram,
				0
			},
			{
				PriorityType.Spoils,
				3
			},
			{
				PriorityType.Unity,
				3
			},
			{
				PriorityType.Military_BuildNavy,
				0
			}
		};

		// Token: 0x040021FC RID: 8700
		private static readonly List<Type> armyOps = new List<Type>
		{
			typeof(DeployArmyOperation),
			typeof(RazeRegionOperation),
			typeof(AssaultSpaceFacilityOperation)
		};

		// Token: 0x040021FD RID: 8701
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.CaptureNationClean,
			GoalType.ExpandNation,
			GoalType.DevelopNation,
			GoalType.MilitarizeNation,
			GoalType.SpaceifyNation
		};
	}
}
