using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200073E RID: 1854
	public class FactionGoal_FoundStation : FactionGoal_FoundHab
	{
		// Token: 0x06002F2C RID: 12076 RVA: 0x001027AE File Offset: 0x001009AE
		public FactionGoal_FoundStation()
		{
		}

		// Token: 0x06002F2D RID: 12077 RVA: 0x001027B8 File Offset: 0x001009B8
		public FactionGoal_FoundStation(TIFactionState faction, int importance, TIOrbitState orbit, GoalType buildStationGoal, List<TIHabModuleTemplate> requiredModules, GoalType defendGoal)
		{
			this.faction = faction;
			base.SetImportance(importance);
			this.orbit = orbit;
			List<string> list;
			if (requiredModules == null)
			{
				list = null;
			}
			else
			{
				list = requiredModules.Select<TIHabModuleTemplate, string>((TIHabModuleTemplate x) => x.dataName).ToList<string>();
			}
			this.requiredModuleNames = list ?? new List<string>();
			this.subsequentGoals = new List<GoalType> { buildStationGoal, defendGoal };
		}

		// Token: 0x06002F2E RID: 12078 RVA: 0x0010283C File Offset: 0x00100A3C
		public static FactionGoal_FoundStation CreateGoal(FactionGoal_FoundStation p)
		{
			FactionGoal_FoundStation factionGoal_FoundStation = GameStateManager.CreateNewGameState<FactionGoal_FoundStation>();
			factionGoal_FoundStation.orbit = p.orbit;
			factionGoal_FoundStation.requiredModuleNames = new List<string>(p.requiredModuleNames);
			return factionGoal_FoundStation;
		}

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x06002F2F RID: 12079 RVA: 0x00102860 File Offset: 0x00100A60
		// (set) Token: 0x06002F30 RID: 12080 RVA: 0x00102868 File Offset: 0x00100A68
		public TIOrbitState orbit { get; protected set; }

		// Token: 0x06002F31 RID: 12081 RVA: 0x00102871 File Offset: 0x00100A71
		public override TIGameState target()
		{
			return this.orbit;
		}

		// Token: 0x06002F32 RID: 12082 RVA: 0x00102879 File Offset: 0x00100A79
		public override TIGameState location()
		{
			return this.orbit;
		}

		// Token: 0x06002F33 RID: 12083 RVA: 0x00102881 File Offset: 0x00100A81
		public override bool ValidNewGoal()
		{
			return this.faction.EligibleforColonization(this.orbit);
		}

		// Token: 0x06002F34 RID: 12084 RVA: 0x00102894 File Offset: 0x00100A94
		public override bool GoalFulfilled()
		{
			return this.goalProduct() != null;
		}

		// Token: 0x06002F35 RID: 12085 RVA: 0x001028A2 File Offset: 0x00100AA2
		public override bool ShouldDiscardGoal()
		{
			return base.ShouldDiscardGoal() || !this.faction.EligibleforColonization(this.orbit);
		}

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x06002F36 RID: 12086 RVA: 0x001028C2 File Offset: 0x00100AC2
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_FoundStation.fleetOps;
			}
		}

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x06002F37 RID: 12087 RVA: 0x001028C9 File Offset: 0x00100AC9
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002F38 RID: 12088 RVA: 0x001028CC File Offset: 0x00100ACC
		public override void ChangeTarget(TIGameState newTarget)
		{
			this.orbit = ((newTarget != null) ? newTarget.ref_orbit : null);
		}

		// Token: 0x06002F39 RID: 12089 RVA: 0x001028E0 File Offset: 0x00100AE0
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			int num = Mathf.Min(base.importance, 18);
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			foreach (GoalType goalType in this.subsequentGoals)
			{
				TIGameState tigameState = this.goalProduct();
				if (((tigameState != null) ? tigameState.ref_hab : null) != null)
				{
					switch (goalType)
					{
					case GoalType.BuildFullStation:
						list.Add(new FactionGoal_BuildFullStation(this.faction, num, this.goalProduct().ref_hab));
						break;
					case GoalType.BuildFullBase:
					case GoalType.BuildMiningBase:
						break;
					case GoalType.BuildRefuellingStation:
						list.Add(new FactionGoal_BuildRefuellingStation(this.faction, num, this.goalProduct().ref_hab));
						break;
					case GoalType.BuildSpecialtyStation:
						list.Add(new FactionGoal_BuildSpecialtyStation(this.faction, num, this.goalProduct().ref_hab, base.specialModules, base.setAsPrimaryHab, this.objective));
						break;
					default:
						if (goalType == GoalType.DefendWithFleet)
						{
							list.Add(new FactionGoal_DefendWithFleet(this.faction, num, this.goalProduct().ref_hab, ""));
						}
						break;
					}
				}
			}
			return list;
		}

		// Token: 0x06002F3A RID: 12090 RVA: 0x00102A1C File Offset: 0x00100C1C
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_FoundStation>(base.ID, false);
		}

		// Token: 0x06002F3B RID: 12091 RVA: 0x00102A2B File Offset: 0x00100C2B
		public override GoalType GetGoalType()
		{
			return GoalType.FoundStation;
		}

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x06002F3C RID: 12092 RVA: 0x00102A2F File Offset: 0x00100C2F
		public override List<Type> spaceOperations
		{
			get
			{
				return FactionGoal_FoundStation.spaceOps;
			}
		}

		// Token: 0x06002F3D RID: 12093 RVA: 0x00102A38 File Offset: 0x00100C38
		public virtual IEnumerable<TIOrbitState> GetAlternativeOrbits()
		{
			TIOrbitState orbit = this.orbit;
			if (((orbit != null) ? orbit.ref_system : null) == null || this.orbit.ref_system == GameStateManager.Sol())
			{
				return Enumerable.Empty<TIOrbitState>();
			}
			return this.orbit.ref_system.OrbitsInSystem;
		}

		// Token: 0x06002F3E RID: 12094 RVA: 0x00102A8C File Offset: 0x00100C8C
		public override void DailyGoalMaintenance()
		{
			FactionGoal_FoundStation.<>c__DisplayClass25_0 CS$<>8__locals1 = new FactionGoal_FoundStation.<>c__DisplayClass25_0();
			CS$<>8__locals1.<>4__this = this;
			base.DailyGoalMaintenance();
			List<TIHabModuleTemplate> list = base.RequiredModules();
			FactionGoal_FoundStation.<>c__DisplayClass25_0 CS$<>8__locals2 = CS$<>8__locals1;
			int num;
			if (list.Count <= 0)
			{
				num = 0;
			}
			else
			{
				num = list.Max<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.tier);
			}
			CS$<>8__locals2.maxTier = num;
			if (!this.orbit.NewStationAllowed(CS$<>8__locals1.maxTier, null))
			{
				IEnumerable<TIOrbitState> enumerable = from x in this.GetAlternativeOrbits()
					where x.NewStationAllowed(CS$<>8__locals1.maxTier, null)
					select x;
				if (this.faction.IsActiveHumanFaction)
				{
					if (this.orbit.isEarthLEO)
					{
						enumerable = enumerable.Where<TIOrbitState>((TIOrbitState x) => x.isEarthLEO);
					}
					else if (!this.orbit.irradiated)
					{
						enumerable = enumerable.Where<TIOrbitState>((TIOrbitState x) => !x.IsIrradiated());
					}
				}
				if (enumerable.Any<TIOrbitState>())
				{
					this.ChangeTarget(enumerable.MinBy<TIOrbitState, double>((TIOrbitState x) => Mathd.Abs(CS$<>8__locals1.<>4__this.orbit.semiMajorAxis_km - x.semiMajorAxis_km)));
					return;
				}
				if (this.orbit.stationsInOrbit.Any<TIHabState>((TIHabState x) => CS$<>8__locals1.<>4__this.faction.AI_AtWarWithFaction(x.faction)))
				{
					IEnumerable<TIHabState> enumerable2 = this.orbit.stationsInOrbit.Where<TIHabState>((TIHabState x) => CS$<>8__locals1.<>4__this.faction.AI_AtWarWithFaction(x.faction) && CS$<>8__locals1.<>4__this.faction.GoalsWithTarget(x, new List<GoalType>
					{
						GoalType.AttackWithFleet,
						GoalType.CaptureHab
					}, true).Count == 0);
					if (enumerable2.Any<TIHabState>())
					{
						if (this.faction.IsActiveHumanFaction)
						{
							if (this.faction.fleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.AssaultCombatValue(false) > 0f))
							{
								TIHabState tihabState = enumerable2.MinBy<TIHabState, float>((TIHabState x) => x.AssaultCombatValue(true));
								GoalType goalType = GoalType.None;
								if (this.subsequentGoals.Count > 0)
								{
									if (this.subsequentGoals.Any<GoalType>((GoalType x) => TIFactionGoalState.BuildHabGoals.Contains(x)))
									{
										goalType = this.subsequentGoals.First<GoalType>((GoalType x) => TIFactionGoalState.BuildHabGoals.Contains(x));
									}
								}
								this.faction.AddGoal(new FactionGoal_CaptureHab(this.faction, base.importance, tihabState, goalType), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
								base.SetImportance(0);
								return;
							}
						}
						TIHabState tihabState2 = enumerable2.MinBy<TIHabState, float>((TIHabState x) => x.SpaceCombatValue());
						int num2 = Mathf.Min(base.importance, 18);
						this.faction.AddGoal(new FactionGoal_AttackWithFleet(this.faction, num2, tihabState2, true, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
						return;
					}
				}
				else
				{
					base.SetImportance(0);
				}
			}
		}

		// Token: 0x04002239 RID: 8761
		public static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList)
		{
			typeof(FoundFusionPlatformOperation),
			typeof(FoundFissionPlatformOperation),
			typeof(FoundSolarPlatformOperation)
		};

		// Token: 0x0400223A RID: 8762
		private static readonly List<Type> spaceOps = new List<Type>
		{
			typeof(FoundPlatformOperation),
			typeof(FoundOrbitalOperation),
			typeof(FoundRingOperation)
		};
	}
}
