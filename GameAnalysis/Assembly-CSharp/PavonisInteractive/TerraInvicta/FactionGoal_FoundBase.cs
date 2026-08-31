using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000742 RID: 1858
	public class FactionGoal_FoundBase : FactionGoal_FoundHab
	{
		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06002F61 RID: 12129 RVA: 0x00103366 File Offset: 0x00101566
		// (set) Token: 0x06002F62 RID: 12130 RVA: 0x0010336E File Offset: 0x0010156E
		public TIHabSiteState site { get; protected set; }

		// Token: 0x06002F63 RID: 12131 RVA: 0x00103377 File Offset: 0x00101577
		public FactionGoal_FoundBase()
		{
		}

		// Token: 0x06002F64 RID: 12132 RVA: 0x00103380 File Offset: 0x00101580
		public FactionGoal_FoundBase(TIFactionState faction, int importance, TIHabSiteState site, GoalType buildBaseGoal, List<TIHabModuleTemplate> requiredModules, GoalType buildStationGoal, bool setAsPrimaryHab = false, TIObjectiveTemplate objective = null)
		{
			this.faction = faction;
			base.SetImportance(importance);
			this.site = site;
			base.setAsPrimaryHab = setAsPrimaryHab;
			this.subsequentGoals = new List<GoalType> { buildBaseGoal, buildStationGoal };
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
			this.objective = objective;
		}

		// Token: 0x06002F65 RID: 12133 RVA: 0x00103414 File Offset: 0x00101614
		public static FactionGoal_FoundBase CreateGoal(FactionGoal_FoundBase p)
		{
			FactionGoal_FoundBase factionGoal_FoundBase = GameStateManager.CreateNewGameState<FactionGoal_FoundBase>();
			factionGoal_FoundBase.site = p.site;
			factionGoal_FoundBase.setAsPrimaryHab = p.setAsPrimaryHab;
			factionGoal_FoundBase.requiredModuleNames = new List<string>(p.requiredModuleNames);
			factionGoal_FoundBase.objective = p.objective;
			return factionGoal_FoundBase;
		}

		// Token: 0x06002F66 RID: 12134 RVA: 0x00103450 File Offset: 0x00101650
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_FoundBase>(base.ID, false);
		}

		// Token: 0x06002F67 RID: 12135 RVA: 0x00103460 File Offset: 0x00101660
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			int num = Mathf.Min(base.importance, 18);
			int num2 = 19;
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			TIGameState tigameState = this.goalProduct();
			TIHabState tihabState = ((tigameState != null) ? tigameState.ref_hab : null);
			if (TIGameState.Valid(tihabState))
			{
				using (List<GoalType>.Enumerator enumerator = this.subsequentGoals.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current)
						{
						case GoalType.BuildFullStation:
							list.Add(new FactionGoal_FoundMaxStation(this.faction, num, tihabState.ref_spaceBody.interfaceOrbits.SelectRandomItem<TIOrbitState>(), GoalType.BuildFullStation, null, GoalType.DefendWithFleet, false, null));
							break;
						case GoalType.BuildFullBase:
							list.Add(new FactionGoal_BuildFullBase(this.faction, num2, tihabState));
							break;
						case GoalType.BuildMiningBase:
							list.Add(new FactionGoal_BuildMiningBase(this.faction, num2, tihabState));
							break;
						case GoalType.BuildRefuellingStation:
							list.Add(new FactionGoal_FoundPlatform(this.faction, num, tihabState.ref_spaceBody.interfaceOrbits.SelectRandomItem<TIOrbitState>(), GoalType.BuildRefuellingStation, null, GoalType.None));
							break;
						case GoalType.BuildSpecialtyStation:
							list.Add(new FactionGoal_FoundMaxStation(this.faction, num, tihabState.ref_spaceBody.interfaceOrbits.SelectRandomItem<TIOrbitState>(), GoalType.BuildSpecialtyStation, base.RequiredModules(), GoalType.DefendWithFleet, false, null));
							break;
						case GoalType.BuildSpecialtyBase:
							list.Add(new FactionGoal_BuildSpecialtyBase(this.faction, num2, tihabState, base.RequiredModules(), base.setAsPrimaryHab, null));
							break;
						}
					}
				}
				if (!this.faction.IsAlienFaction)
				{
					if (tihabState.ref_spaceBody.habSites.Length <= 1)
					{
						if (!this.subsequentGoals.Any<GoalType>((GoalType x) => x == GoalType.BuildSpecialtyBase))
						{
							return list;
						}
					}
					int num3 = base.importance - 4 + tihabState.ref_spaceBody.habSites.Length;
					if (!this.subsequentGoals.Any<GoalType>((GoalType x) => x == GoalType.BuildFullBase))
					{
						if (!this.subsequentGoals.Any<GoalType>((GoalType x) => x == GoalType.BuildSpecialtyBase))
						{
							if (this.subsequentGoals.Any<GoalType>((GoalType x) => x == GoalType.BuildMiningBase))
							{
								num3 -= 6;
								goto IL_025C;
							}
							goto IL_025C;
						}
					}
					num3 += 3;
					IL_025C:
					num3 = Mathf.Min(num3, 18);
					TIFactionGoalState tifactionGoalState = this.faction.GoalsWithTarget(tihabState.ref_spaceBody, GoalType.DefendWithFleet, true).FirstOrDefault<TIFactionGoalState>();
					if (tifactionGoalState != null)
					{
						tifactionGoalState.SetImportance(Mathf.Max(tifactionGoalState.importance, num3));
					}
					else
					{
						list.Add(new FactionGoal_DefendWithFleet(this.faction, num3, tihabState.ref_spaceBody, ""));
					}
				}
			}
			return list;
		}

		// Token: 0x06002F68 RID: 12136 RVA: 0x0010374C File Offset: 0x0010194C
		public override GoalType GetGoalType()
		{
			return GoalType.FoundBase;
		}

		// Token: 0x06002F69 RID: 12137 RVA: 0x0010374F File Offset: 0x0010194F
		public override TIGameState target()
		{
			return this.site;
		}

		// Token: 0x06002F6A RID: 12138 RVA: 0x00103757 File Offset: 0x00101957
		public override TIGameState location()
		{
			return this.site;
		}

		// Token: 0x06002F6B RID: 12139 RVA: 0x0010375F File Offset: 0x0010195F
		public override TIGameState goalProduct()
		{
			return this.site.hab;
		}

		// Token: 0x06002F6C RID: 12140 RVA: 0x0010376C File Offset: 0x0010196C
		public override bool ValidNewGoal()
		{
			return this.faction.EligibleforColonization(this.site) && !this.site.hasPlannedOrOperatingBase;
		}

		// Token: 0x06002F6D RID: 12141 RVA: 0x00103791 File Offset: 0x00101991
		public override bool ShouldDiscardGoal()
		{
			return this.site == null || base.ShouldDiscardGoal();
		}

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06002F6E RID: 12142 RVA: 0x001037AE File Offset: 0x001019AE
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_FoundBase.fleetOps;
			}
		}

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x06002F6F RID: 12143 RVA: 0x001037B5 File Offset: 0x001019B5
		public override List<Type> spaceOperations
		{
			get
			{
				return FactionGoal_FoundBase.spaceOps;
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x06002F70 RID: 12144 RVA: 0x001037BC File Offset: 0x001019BC
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002F71 RID: 12145 RVA: 0x001037BF File Offset: 0x001019BF
		public override void ChangeTarget(TIGameState newTarget)
		{
			this.site = ((newTarget != null) ? newTarget.ref_habSite : null);
		}

		// Token: 0x06002F72 RID: 12146 RVA: 0x001037D4 File Offset: 0x001019D4
		public override void DailyGoalMaintenance()
		{
			base.DailyGoalMaintenance();
			if (!this.site.hasPlannedOrOperatingBase || !(this.site.hab.faction != this.faction))
			{
				return;
			}
			if (this.faction.IsAlienFaction && !AIEvaluators.ShouldAliensGoLoud())
			{
				base.SetImportance(0);
				return;
			}
			if (base.assignedFleet != null && (base.assignedFleet.transferAssigned || base.assignedFleet.ref_system == this.site.ref_system) && ((base.assignedFleet.BombardmentValue(this.site.ref_spaceBody) > 0f && !this.faction.permanentAlly(this.site.hab.faction) && !this.site.hab.anyCoreCompleted) || this.site.hab.SpaceCombatValue() == 0f))
			{
				return;
			}
			if (this.faction.AI_AtWarWithFaction(this.site.hab.faction))
			{
				if (this.faction.IsActiveHumanFaction && this.faction.fleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.AssaultCombatValue(false) > this.site.hab.AssaultCombatValue(true) && this.faction.GoalsWithTarget(this.site.hab, new List<GoalType> { GoalType.AttackWithFleet }, true).Count == 0))
				{
					GoalType goalType = GoalType.None;
					if (this.subsequentGoals.Count > 0)
					{
						if (this.subsequentGoals.Any<GoalType>((GoalType x) => TIFactionGoalState.BuildHabGoals.Contains(x)))
						{
							goalType = this.subsequentGoals.First<GoalType>((GoalType x) => TIFactionGoalState.BuildHabGoals.Contains(x));
						}
					}
					this.faction.AddGoal(new FactionGoal_CaptureHab(this.faction, base.importance, this.site.hab, goalType), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					base.SetImportance(0);
					return;
				}
				int num = Mathf.Min(base.importance, 18);
				this.faction.AddGoal(new FactionGoal_AttackWithFleet(this.faction, num, this.site.hab, true, null, true), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				base.SetImportance(0);
				return;
			}
			else
			{
				List<TIHabSiteState> list = this.site.parentBody.habSites.Where<TIHabSiteState>((TIHabSiteState x) => !x.hasPlannedOrOperatingBase).ToList<TIHabSiteState>();
				if (list.Count == 0)
				{
					list = this.site.parentBody.habSitesInSystem.Where<TIHabSiteState>((TIHabSiteState x) => !x.hasPlannedOrOperatingBase).ToList<TIHabSiteState>();
				}
				if (list.Count <= 0)
				{
					base.SetImportance(0);
					return;
				}
				AIEvaluators.EvaluateHabSite(this.faction, this.site, false, false, true);
				TIHabSiteState tihabSiteState = list.MaxBy<TIHabSiteState, float>((TIHabSiteState x) => AIEvaluators.EvaluateHabSite(this.faction, x, false, false, true));
				if (AIEvaluators.EvaluateHabSite(this.faction, this.site, false, false, true) * 0.5f <= AIEvaluators.EvaluateHabSite(this.faction, tihabSiteState, false, false, true))
				{
					this.ChangeTarget(tihabSiteState);
					return;
				}
				base.SetImportance(0);
				return;
			}
		}

		// Token: 0x04002241 RID: 8769
		private static readonly List<Type> spaceOps = new List<Type>
		{
			typeof(FoundOutpostOperation),
			typeof(FoundSettlementOperation),
			typeof(FoundColonyOperation)
		};

		// Token: 0x04002242 RID: 8770
		private static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList)
		{
			typeof(FoundFusionOutpostOperation),
			typeof(FoundFissionOutpostOperation),
			typeof(FoundSolarOutpostOperation),
			typeof(BombardOperation_Low),
			typeof(BombardOperation_Med),
			typeof(BombardOperation_High)
		};
	}
}
