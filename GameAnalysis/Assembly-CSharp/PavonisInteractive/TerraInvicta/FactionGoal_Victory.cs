using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200075B RID: 1883
	public class FactionGoal_Victory : TIFactionGoalState
	{
		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x0600314E RID: 12622 RVA: 0x00109548 File Offset: 0x00107748
		// (set) Token: 0x0600314F RID: 12623 RVA: 0x00109586 File Offset: 0x00107786
		public override TIObjectiveTemplate objective
		{
			get
			{
				if (!string.IsNullOrEmpty(this.victoryObjectiveTemplateName))
				{
					TIObjectiveTemplate tiobjectiveTemplate = TemplateManager.Find<TIObjectiveTemplate>(this.victoryObjectiveTemplateName, false);
					if (tiobjectiveTemplate != null)
					{
						this.objective = tiobjectiveTemplate;
					}
				}
				this.victoryObjectiveTemplateName = null;
				return base.objective;
			}
			set
			{
				base.objective = value;
			}
		}

		// Token: 0x06003150 RID: 12624 RVA: 0x0010958F File Offset: 0x0010778F
		public FactionGoal_Victory()
		{
		}

		// Token: 0x06003151 RID: 12625 RVA: 0x00109597 File Offset: 0x00107797
		public FactionGoal_Victory(TIFactionState faction, TIVictoryTemplate victoryTemplate, TIObjectiveTemplate victoryObjective)
		{
			this.faction = faction;
			base.SetImportance(20);
			this.victoryTemplateName = victoryTemplate.dataName;
			this.objective = victoryObjective;
		}

		// Token: 0x06003152 RID: 12626 RVA: 0x001095C1 File Offset: 0x001077C1
		public static FactionGoal_Victory CreateGoal(FactionGoal_Victory p)
		{
			FactionGoal_Victory factionGoal_Victory = GameStateManager.CreateNewGameState<FactionGoal_Victory>();
			factionGoal_Victory.victoryTemplateName = p.victoryTemplateName;
			factionGoal_Victory._victoryTemplate = p.victoryTemplate;
			return factionGoal_Victory;
		}

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x06003153 RID: 12627 RVA: 0x001095E0 File Offset: 0x001077E0
		public TIVictoryTemplate victoryTemplate
		{
			get
			{
				if (this._victoryTemplate == null)
				{
					this._victoryTemplate = TemplateManager.Find<TIVictoryTemplate>(this.victoryTemplateName, false);
				}
				return this._victoryTemplate;
			}
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06003154 RID: 12628 RVA: 0x00109602 File Offset: 0x00107802
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return new List<GoalType>();
			}
		}

		// Token: 0x06003155 RID: 12629 RVA: 0x00109609 File Offset: 0x00107809
		public override GoalType GetGoalType()
		{
			return GoalType.PursueVictory;
		}

		// Token: 0x06003156 RID: 12630 RVA: 0x0010960D File Offset: 0x0010780D
		public override TIGameState actor()
		{
			return this.faction;
		}

		// Token: 0x06003157 RID: 12631 RVA: 0x00109615 File Offset: 0x00107815
		public override TIGameState target()
		{
			return this.victoryMissionTarget;
		}

		// Token: 0x06003158 RID: 12632 RVA: 0x0010961D File Offset: 0x0010781D
		public override TIGameState location()
		{
			return this.target();
		}

		// Token: 0x06003159 RID: 12633 RVA: 0x00109625 File Offset: 0x00107825
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_Victory>(base.ID, false);
		}

		// Token: 0x0600315A RID: 12634 RVA: 0x00109634 File Offset: 0x00107834
		public override bool ValidNewGoal()
		{
			TIFactionState faction = this.faction;
			return faction != null && faction.unlockedVictoryObjective;
		}

		// Token: 0x0600315B RID: 12635 RVA: 0x00109647 File Offset: 0x00107847
		public override bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget)
		{
			return testGoal.faction.GoalsOfType(GoalType.PursueVictory, false, true).Count > 0;
		}

		// Token: 0x0600315C RID: 12636 RVA: 0x00109660 File Offset: 0x00107860
		public override bool InProgress()
		{
			return this.faction.unlockedVictoryObjective;
		}

		// Token: 0x0600315D RID: 12637 RVA: 0x0010966D File Offset: 0x0010786D
		public override bool ShouldDiscardGoal()
		{
			return false;
		}

		// Token: 0x0600315E RID: 12638 RVA: 0x00109670 File Offset: 0x00107870
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x0600315F RID: 12639 RVA: 0x00109673 File Offset: 0x00107873
		public override TIGameState goalProduct()
		{
			return null;
		}

		// Token: 0x06003160 RID: 12640 RVA: 0x00109676 File Offset: 0x00107876
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return new List<TIFactionGoalState>();
		}

		// Token: 0x06003161 RID: 12641 RVA: 0x0010967D File Offset: 0x0010787D
		public override void ChangeTarget(TIGameState newTarget)
		{
		}

		// Token: 0x06003162 RID: 12642 RVA: 0x0010967F File Offset: 0x0010787F
		public override void DailyGoalMaintenance()
		{
			base.DailyGoalMaintenance();
			this.ManageAttacks();
			this.ManageCouncilorTransport();
		}

		// Token: 0x06003163 RID: 12643 RVA: 0x00109694 File Offset: 0x00107894
		public void ManageCouncilorTransport()
		{
			ValueTuple<TICouncilorState, TIMissionTemplate> valueTuple = this.faction.councilors.Select<TICouncilorState, ValueTuple<TICouncilorState, TIMissionTemplate>>((TICouncilorState x) => new ValueTuple<TICouncilorState, TIMissionTemplate>(x, x.GetPossibleMissionList(false, false, true, null, false).FirstOrDefault<TIMissionTemplate>((TIMissionTemplate y) => y.IsVictoryMission))).FirstOrDefault<ValueTuple<TICouncilorState, TIMissionTemplate>>(([TupleElementNames(new string[] { "Councilor", "Mission" })] ValueTuple<TICouncilorState, TIMissionTemplate> x) => x.Item2 != null);
			TICouncilorState victoryCouncilor = valueTuple.Item1;
			TIMissionTemplate item = valueTuple.Item2;
			bool flag = false;
			if (this.faction.victoryTemplate.AllVictoryConditionsMet(this.faction))
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			if ((from x in this.faction.GoalsOfType(GoalType.TransportCouncilorsViaFleet, false, true)
				select x as FactionGoal_TransportCouncilorsWithFleet).Any<FactionGoal_TransportCouncilorsWithFleet>((FactionGoal_TransportCouncilorsWithFleet x) => x.assignedCouncilors.Contains(victoryCouncilor)))
			{
				return;
			}
			TIMissionTarget_VictoryMissionTarget timissionTarget_VictoryMissionTarget = ((item != null) ? item.target : null) as TIMissionTarget_VictoryMissionTarget;
			if (timissionTarget_VictoryMissionTarget != null)
			{
				TIGameState tigameState = (from x in timissionTarget_VictoryMissionTarget.GetVictoryTargets(this.faction)
					orderby x.ref_spaceBody == victoryCouncilor.ref_spaceBody descending, x.ref_system == victoryCouncilor.ref_system descending, x.ref_orbit == null && x.ref_fleet == null descending
					select x).ThenByDescending<TIGameState, bool>(delegate(TIGameState x)
				{
					TIFactionState ref_faction = x.ref_faction;
					return ref_faction != null && ref_faction.permanentAlly(this.faction);
				}).ThenByDescending<TIGameState, bool>(delegate(TIGameState x)
				{
					TIHabState ref_hab = x.ref_hab;
					return ref_hab != null && ref_hab.IsStation;
				}).ThenByDescending<TIGameState, float>(delegate(TIGameState x)
				{
					if (x.ref_hab == null)
					{
						return 0f;
					}
					float num = (x.ref_hab.IsStation ? x.ref_hab.AggregateDefensiveScore_Station() : x.ref_hab.SpaceCombatValue());
					TIFactionState ref_faction2 = x.ref_faction;
					if (ref_faction2 == null || !ref_faction2.permanentAlly(this.faction))
					{
						num = 1f / num;
					}
					return num;
				})
					.ToList<TIGameState>()
					.FirstOrDefault<TIGameState>();
				if (tigameState != null && (tigameState.ref_region == null || victoryCouncilor.OnEarth))
				{
					this.faction.AddGoal(new FactionGoal_TransportCouncilorsWithFleet(this.faction, 20, new List<TICouncilorState> { victoryCouncilor }, tigameState), HandleDuplicateGoalRule.ResetImportance, null);
				}
			}
		}

		// Token: 0x06003164 RID: 12644 RVA: 0x00109890 File Offset: 0x00107A90
		public void ManageAttacks()
		{
			IEnumerable<TIGameState> attackTargets = this.GetAttackTargets();
			List<FactionGoal_AttackWithFleet> existingObjectiveAttacks = (from x in this.faction.GoalsOfType(GoalType.AttackWithFleet, false, true)
				select x as FactionGoal_AttackWithFleet into x
				where x.objectiveGoal
				select x).ToList<FactionGoal_AttackWithFleet>();
			List<TIGameState> list = attackTargets.Where<TIGameState>((TIGameState x) => existingObjectiveAttacks.None<FactionGoal_AttackWithFleet>((FactionGoal_AttackWithFleet y) => y.target() == x)).ToList<TIGameState>();
			int num = 3;
			if (list.Count == 0 || existingObjectiveAttacks.Count >= num)
			{
				return;
			}
			for (int i = 0; i < num - existingObjectiveAttacks.Count; i++)
			{
				IEnumerable<TIGameState> enumerable = list;
				Dictionary<TISpaceBodyState, List<TIGameState>> dictionary = (from x in enumerable
					where x.ref_system != null
					group x by x.ref_system).ToDictionary<IGrouping<TISpaceBodyState, TIGameState>, TISpaceBodyState, List<TIGameState>>((IGrouping<TISpaceBodyState, TIGameState> x) => x.Key, (IGrouping<TISpaceBodyState, TIGameState> x) => x.ToList<TIGameState>());
				if (dictionary.Count > 0)
				{
					List<TIGameState> list2;
					List<TIGameState> list3;
					List<TIGameState> list4;
					if (dictionary.TryGetValue(GameStateManager.Earth(), out list2))
					{
						enumerable = list2;
					}
					else if (dictionary.TryGetValue(GameStateManager.Mars(), out list3))
					{
						enumerable = list3;
					}
					else if (dictionary.TryGetValue(GameStateManager.Mercury(), out list4))
					{
						enumerable = list4;
					}
					else
					{
						enumerable = dictionary.MinBy<KeyValuePair<TISpaceBodyState, List<TIGameState>>, double>((KeyValuePair<TISpaceBodyState, List<TIGameState>> x) => x.Key.semiMajorAxis_AU).Value;
					}
					TIGameState tigameState = enumerable.SelectRandomItem<TIGameState>();
					FactionGoal_AttackWithFleet factionGoal_AttackWithFleet = new FactionGoal_AttackWithFleet(this.faction, 19, tigameState, false, this.objective, false);
					this.faction.AddGoal(factionGoal_AttackWithFleet, HandleDuplicateGoalRule.ResetImportance, null);
					list.Remove(tigameState);
				}
			}
		}

		// Token: 0x06003165 RID: 12645 RVA: 0x00109AA8 File Offset: 0x00107CA8
		public IEnumerable<TIGameState> GetAttackTargets()
		{
			HashSet<TIGameState> hashSet = new HashSet<TIGameState>();
			HashSet<TIVictoryTemplate.VictoryConditionType> hashSet2 = new HashSet<TIVictoryTemplate.VictoryConditionType>
			{
				TIVictoryTemplate.VictoryConditionType.MaxProAlienFleetPower,
				TIVictoryTemplate.VictoryConditionType.MaxOtherFactionsFleetPower,
				TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliens,
				TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliensAndAllies,
				TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatEveryone,
				TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAntiAlienFactions,
				TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatNonVeryProAlienFactions,
				TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatExtremists,
				TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliens,
				TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliensAndAllies,
				TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatEveryone,
				TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAntiAlienFactions,
				TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatNonVeryProAlienFactions,
				TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatExtremists,
				TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliens,
				TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliensAndAllies,
				TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatEveryone,
				TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAntiAlienFactions,
				TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatNonVeryProAlienFactions,
				TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatExtremists
			};
			foreach (TIVictoryTemplate.VictoryCondition victoryCondition in this.victoryTemplate.victoryConditions)
			{
				if (hashSet2.Contains(victoryCondition.conditionType))
				{
					List<TISpaceAssetState> list;
					this.victoryTemplate.SingleVictoryConditionDescriptionWithScore(this.faction, victoryCondition, out list);
					hashSet.UnionWith(list);
				}
			}
			return hashSet.Where<TIGameState>((TIGameState x) => x != null);
		}

		// Token: 0x04002277 RID: 8823
		public TIGameState victoryMissionTarget;

		// Token: 0x04002278 RID: 8824
		public string victoryTemplateName;

		// Token: 0x04002279 RID: 8825
		private TIVictoryTemplate _victoryTemplate;

		// Token: 0x0400227A RID: 8826
		public string victoryObjectiveTemplateName;
	}
}
