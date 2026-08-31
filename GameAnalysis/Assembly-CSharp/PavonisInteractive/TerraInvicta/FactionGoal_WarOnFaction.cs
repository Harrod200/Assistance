using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000736 RID: 1846
	public class FactionGoal_WarOnFaction : FactionGoal_Faction
	{
		// Token: 0x06002E64 RID: 11876 RVA: 0x000FC4D5 File Offset: 0x000FA6D5
		public FactionGoal_WarOnFaction()
		{
		}

		// Token: 0x06002E65 RID: 11877 RVA: 0x000FC4E0 File Offset: 0x000FA6E0
		public FactionGoal_WarOnFaction(TIFactionState faction, int importance, TIFactionState enemyFaction, TIObjectiveTemplate objective = null)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.targetFaction = enemyFaction;
			this.objective = objective;
			string displayName = faction.displayName;
			string text = " starts war with ";
			TIFactionState targetFaction = base.targetFaction;
			TIFactionState.LogAI(displayName + text + ((targetFaction != null) ? targetFaction.displayName : null), false);
		}

		// Token: 0x06002E66 RID: 11878 RVA: 0x000FC538 File Offset: 0x000FA738
		public static FactionGoal_WarOnFaction CreateGoal(FactionGoal_WarOnFaction prospectiveGoal)
		{
			FactionGoal_WarOnFaction factionGoal_WarOnFaction = GameStateManager.CreateNewGameState<FactionGoal_WarOnFaction>();
			factionGoal_WarOnFaction.targetFaction = prospectiveGoal.targetFaction;
			factionGoal_WarOnFaction.objective = prospectiveGoal.objective;
			return factionGoal_WarOnFaction;
		}

		// Token: 0x06002E67 RID: 11879 RVA: 0x000FC557 File Offset: 0x000FA757
		public override void RemoveState()
		{
			string displayName = this.faction.displayName;
			string text = " ends war with ";
			TIGameState tigameState = this.target();
			TIFactionState.LogAI(displayName + text + ((tigameState != null) ? tigameState.displayName : null), false);
			GameStateManager.RemoveGameState<FactionGoal_WarOnFaction>(base.ID, false);
		}

		// Token: 0x06002E68 RID: 11880 RVA: 0x000FC593 File Offset: 0x000FA793
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002E69 RID: 11881 RVA: 0x000FC596 File Offset: 0x000FA796
		public override bool FactionMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002E6A RID: 11882 RVA: 0x000FC599 File Offset: 0x000FA799
		public override bool PoliciesAtTargetNationGoal()
		{
			return true;
		}

		// Token: 0x06002E6B RID: 11883 RVA: 0x000FC59C File Offset: 0x000FA79C
		public override GoalType GetGoalType()
		{
			return GoalType.WarOnFaction;
		}

		// Token: 0x06002E6C RID: 11884 RVA: 0x000FC5A0 File Offset: 0x000FA7A0
		public override TIGameState actor()
		{
			return this.faction;
		}

		// Token: 0x06002E6D RID: 11885 RVA: 0x000FC5A8 File Offset: 0x000FA7A8
		public override TIGameState target()
		{
			return base.targetFaction;
		}

		// Token: 0x06002E6E RID: 11886 RVA: 0x000FC5B0 File Offset: 0x000FA7B0
		public override TIGameState location()
		{
			return null;
		}

		// Token: 0x06002E6F RID: 11887 RVA: 0x000FC5B3 File Offset: 0x000FA7B3
		public override TIGameState goalProduct()
		{
			return base.targetFaction;
		}

		// Token: 0x06002E70 RID: 11888 RVA: 0x000FC5BB File Offset: 0x000FA7BB
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002E71 RID: 11889 RVA: 0x000FC5D0 File Offset: 0x000FA7D0
		public override bool InProgress()
		{
			return true;
		}

		// Token: 0x06002E72 RID: 11890 RVA: 0x000FC5D4 File Offset: 0x000FA7D4
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.targetFaction == null || (!base.objectiveGoal && !this.IsTotalWar && !AIEvaluators.FactionsGoToWar(this.faction, base.targetFaction));
		}

		// Token: 0x06002E73 RID: 11891 RVA: 0x000FC620 File Offset: 0x000FA820
		public override bool GoalFulfilled()
		{
			return base.targetFaction != null && (base.targetFaction.defeated || (base.targetFaction.councilors.Count == 0 && base.targetFaction.habs.Count == 0 && base.targetFaction.fleets.Count == 0 && !this.IsTotalWar));
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x06002E74 RID: 11892 RVA: 0x000FC68D File Offset: 0x000FA88D
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_WarOnFaction.missionModifiers;
			}
		}

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x06002E75 RID: 11893 RVA: 0x000FC694 File Offset: 0x000FA894
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x06002E76 RID: 11894 RVA: 0x000FC697 File Offset: 0x000FA897
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return PolicyManager.DegradeRelationsPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x06002E77 RID: 11895 RVA: 0x000FC69E File Offset: 0x000FA89E
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.DegradeRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x06002E78 RID: 11896 RVA: 0x000FC6A5 File Offset: 0x000FA8A5
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_WarOnFaction.incompatibleGoalsForFaction;
			}
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x06002E79 RID: 11897 RVA: 0x000FC6AC File Offset: 0x000FA8AC
		// (set) Token: 0x06002E7A RID: 11898 RVA: 0x000FC6B4 File Offset: 0x000FA8B4
		public bool IsTotalWar { get; private set; }

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06002E7B RID: 11899 RVA: 0x000FC6BD File Offset: 0x000FA8BD
		public static float AlienTotalWarHateThreshold
		{
			get
			{
				return TemplateManager.global.alienFactionHateWarValue * 4f;
			}
		}

		// Token: 0x06002E7C RID: 11900 RVA: 0x000FC6D0 File Offset: 0x000FA8D0
		public override void OnGoalAssigned()
		{
			this.faction.playerControl.StartAction(new BreakPactAction(this.faction, base.targetFaction, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.Truce }));
			this.faction.playerControl.StartAction(new BreakPactAction(this.faction, base.targetFaction, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.NAP }));
		}

		// Token: 0x06002E7D RID: 11901 RVA: 0x000FC738 File Offset: 0x000FA938
		public override void DailyGoalMaintenance()
		{
			FactionGoal_WarOnFaction.<>c__DisplayClass36_0 CS$<>8__locals1 = new FactionGoal_WarOnFaction.<>c__DisplayClass36_0();
			CS$<>8__locals1.<>4__this = this;
			int num = GameStateManager.AllFactions().IndexOf(this.faction);
			if (this.firstMaintenanceCompleted && (TITimeState.Now().day + num) % 7 != 0)
			{
				return;
			}
			this.firstMaintenanceCompleted = true;
			if (TemplateManager.global.IsAlienTotalWarPossible() && this.faction.IsAlienFaction && !base.targetFaction.veryProAlien && this.faction.GetFactionHate(base.targetFaction) >= FactionGoal_WarOnFaction.AlienTotalWarHateThreshold)
			{
				this.IsTotalWar = true;
			}
			FactionGoal_WarOnFaction factionGoal_WarOnFaction = (from x in this.faction.GoalsOfType(GoalType.WarOnFaction, false, true)
				select x as FactionGoal_WarOnFaction into x
				orderby CS$<>8__locals1.<>4__this.targetFaction.fleets.Any<TISpaceFleetState>() descending
				orderby x.importance descending, x.IsTotalWar descending
				select x).FirstOrDefault<FactionGoal_WarOnFaction>();
			CS$<>8__locals1.IsMostImportantWar = this == factionGoal_WarOnFaction;
			CS$<>8__locals1.captureGoals = this.faction.factionGoals[GoalType.CaptureHab].Select<TIFactionGoalState, FactionGoal_CaptureHab>((TIFactionGoalState x) => x as FactionGoal_CaptureHab).Where<FactionGoal_CaptureHab>(delegate(FactionGoal_CaptureHab x)
			{
				TIGameState tigameState = x.target();
				return ((tigameState != null) ? tigameState.ref_faction : null) == CS$<>8__locals1.<>4__this.targetFaction;
			}).ToList<FactionGoal_CaptureHab>();
			foreach (TISpaceBodyState tispaceBodyState in from x in this.faction.habs.Select<TIHabState, TISpaceBodyState>((TIHabState x) => x.ref_system).Distinct<TISpaceBodyState>()
				where !CS$<>8__locals1.captureGoals.Any<FactionGoal_CaptureHab>((FactionGoal_CaptureHab y) => y.target().ref_system == x)
				select x)
			{
				TIFactionState faction = this.faction;
				TIFactionState targetFaction = base.targetFaction;
				IEnumerable<TIHabState> habsInSystem = tispaceBodyState.habsInSystem;
				Func<TIHabState, bool> func;
				if ((func = CS$<>8__locals1.<>9__21) == null)
				{
					func = (CS$<>8__locals1.<>9__21 = (TIHabState x) => x.faction == CS$<>8__locals1.<>4__this.targetFaction);
				}
				TIHabState tihabState = AIEvaluators.SelectHabToCapture(faction, targetFaction, from x in habsInSystem.Where<TIHabState>(func)
					where x.IsBase
					select x, AIEvaluators.HabCapturingLogic.LowEffortHighReward, false);
				FactionGoal_CaptureHab factionGoal_CaptureHab = new FactionGoal_CaptureHab(this.faction, 19, tihabState, GoalType.None);
				factionGoal_CaptureHab = this.faction.AddGoal(factionGoal_CaptureHab, HandleDuplicateGoalRule.ResetImportanceIfHigher, null) as FactionGoal_CaptureHab;
				if (factionGoal_CaptureHab != null)
				{
					CS$<>8__locals1.captureGoals.Add(factionGoal_CaptureHab);
				}
			}
			CS$<>8__locals1.attackGoals = new HashSet<FactionGoal_AttackWithFleet>(this.faction.factionGoals[GoalType.AttackWithFleet].Select<TIFactionGoalState, FactionGoal_AttackWithFleet>((TIFactionGoalState x) => x as FactionGoal_AttackWithFleet).Where<FactionGoal_AttackWithFleet>(delegate(FactionGoal_AttackWithFleet x)
			{
				TIGameState tigameState2 = x.target();
				return ((tigameState2 != null) ? tigameState2.ref_faction : null) == CS$<>8__locals1.<>4__this.targetFaction;
			}));
			bool flag = this.faction.IsAlienFaction && TemplateManager.global.DoAliensHaveReducedWarAttacks();
			IEnumerable<TISpaceFleetState> enumerable = base.targetFaction.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => CS$<>8__locals1.attackGoals.None<FactionGoal_AttackWithFleet>((FactionGoal_AttackWithFleet y) => y.target() == x));
			CS$<>8__locals1.fleetTarget0 = AIEvaluators.SelectFleetToAttack(this.faction, enumerable, -1f);
			TISpaceFleetState tispaceFleetState = null;
			if (!flag)
			{
				IEnumerable<TISpaceFleetState> enumerable2 = enumerable.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
				{
					TISpaceFleetState fleetTarget = CS$<>8__locals1.fleetTarget0;
					if (!(((fleetTarget != null) ? fleetTarget.ref_system : null) == null))
					{
						TISpaceFleetState fleetTarget2 = CS$<>8__locals1.fleetTarget0;
						return ((fleetTarget2 != null) ? fleetTarget2.ref_system : null) != x.ref_system;
					}
					return true;
				});
				tispaceFleetState = AIEvaluators.SelectFleetToAttack(this.faction, enumerable2, -1f);
			}
			List<TISpaceBodyState> list = (from x in base.targetFaction.habs
				where CS$<>8__locals1.captureGoals.Union<TIFactionGoalState>(CS$<>8__locals1.attackGoals).None<TIFactionGoalState>((TIFactionGoalState y) => y.target() == x)
				select x.ref_system).Distinct<TISpaceBodyState>().ToList<TISpaceBodyState>();
			List<TISpaceBodyState> list2 = list.Where<TISpaceBodyState>((TISpaceBodyState x) => x.habSitesInSystem.Count >= 4).ToList<TISpaceBodyState>();
			if (list2.Count > 0)
			{
				list = list2;
			}
			IEnumerable<TIHabState> enumerable3 = from x in list.SelectMany<TISpaceBodyState, TIHabState>((TISpaceBodyState x) => x.habsInSystem).Intersect<TIHabState>(base.targetFaction.habs)
				where CS$<>8__locals1.captureGoals.Union<TIFactionGoalState>(CS$<>8__locals1.attackGoals).None<TIFactionGoalState>((TIFactionGoalState t) => t.target() == x)
				select x;
			TIHabState tihabState2 = AIEvaluators.SelectStationToAttack(this.faction, enumerable3.Where<TIHabState>((TIHabState x) => x.IsStation), -1f);
			TIHabState tihabState3 = AIEvaluators.SelectBaseToAttack(this.faction, enumerable3.Where<TIHabState>((TIHabState x) => x.IsBase));
			if (flag && tihabState2 != null && tihabState3 != null)
			{
				if (TIUtilities.RandomFloatValue() < 0.5f)
				{
					tihabState2 = null;
				}
				else
				{
					tihabState3 = null;
				}
			}
			List<TIGameState> list3 = new List<TIGameState>();
			list3.Add(CS$<>8__locals1.fleetTarget0);
			list3.Add(tispaceFleetState);
			list3.Add(tihabState2);
			list3.Add(tihabState3);
			int num2 = 0;
			using (List<TIGameState>.Enumerator enumerator2 = list3.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					TIGameState target = enumerator2.Current;
					if (!(target == null) && !CS$<>8__locals1.attackGoals.Any<FactionGoal_AttackWithFleet>((FactionGoal_AttackWithFleet x) => x.target() == target))
					{
						if (target.isSpaceFleetState)
						{
							if (CS$<>8__locals1.attackGoals.Where<FactionGoal_AttackWithFleet>(delegate(FactionGoal_AttackWithFleet x)
							{
								TIGameState tigameState3 = x.target();
								return tigameState3 != null && tigameState3.isSpaceFleetState;
							}).Count<FactionGoal_AttackWithFleet>((FactionGoal_AttackWithFleet x) => x.requiresWar) >= 2)
							{
								continue;
							}
						}
						if (target.isHabState)
						{
							if ((from x in CS$<>8__locals1.attackGoals.Where<FactionGoal_AttackWithFleet>(delegate(FactionGoal_AttackWithFleet x)
								{
									TIGameState tigameState4 = x.target();
									return tigameState4 != null && tigameState4.isHabState;
								})
								where x.target().ref_hab.IsBase == target.ref_hab.IsBase
								select x).Any<FactionGoal_AttackWithFleet>())
							{
								continue;
							}
						}
						FactionGoal_AttackWithFleet factionGoal_AttackWithFleet = new FactionGoal_AttackWithFleet(this.faction, CS$<>8__locals1.<DailyGoalMaintenance>g__GetAttackImportance|18(target), target, true, this.objective, false);
						factionGoal_AttackWithFleet = this.faction.AddGoal(factionGoal_AttackWithFleet, HandleDuplicateGoalRule.ResetImportanceIfHigher, null) as FactionGoal_AttackWithFleet;
						if (factionGoal_AttackWithFleet != null)
						{
							CS$<>8__locals1.attackGoals.Add(factionGoal_AttackWithFleet);
							num2++;
						}
					}
				}
			}
			if ((num2 == 0) & CS$<>8__locals1.IsMostImportantWar)
			{
				if (CS$<>8__locals1.attackGoals.All<FactionGoal_AttackWithFleet>((FactionGoal_AttackWithFleet x) => x.assignedFleet != null))
				{
					IEnumerable<TISpaceShipState> enumerable4 = (from x in this.faction.fleets
						where x.AssignedGoal() == null
						where !x.NeedsRepair() && !x.NeedsRefuel() && !x.NeedsRearm()
						select from x in x.ships
							where x.combatant
							select x).MaxBy<IEnumerable<TISpaceShipState>, int>((IEnumerable<TISpaceShipState> x) => x.Count<TISpaceShipState>());
					int num3 = ((enumerable4 != null) ? enumerable4.Count<TISpaceShipState>() : 0);
					int num4 = 4;
					if (flag)
					{
						num4 -= 2;
					}
					int num5 = num4;
					if (this.faction.IsAlienFaction)
					{
						num5 += TemplateManager.global.GetAlienMaxExtraWarAttacks();
					}
					if (num3 > 7 && CS$<>8__locals1.attackGoals.Count < num5)
					{
						TISpaceAssetState tispaceAssetState;
						if (enumerable3.Any<TIHabState>() && (!enumerable.Any<TISpaceFleetState>() || TIUtilities.RandomFloatValue() < 0.33f))
						{
							tispaceAssetState = AIEvaluators.SelectHabToAttack(this.faction, enumerable3);
						}
						else
						{
							tispaceAssetState = AIEvaluators.SelectFleetToAttack(this.faction, enumerable, -1f);
						}
						if (tispaceAssetState != null)
						{
							FactionGoal_AttackWithFleet factionGoal_AttackWithFleet2 = new FactionGoal_AttackWithFleet(this.faction, CS$<>8__locals1.<DailyGoalMaintenance>g__GetAttackImportance|18(tispaceAssetState), tispaceAssetState, true, this.objective, false);
							factionGoal_AttackWithFleet2 = this.faction.AddGoal(factionGoal_AttackWithFleet2, HandleDuplicateGoalRule.ResetImportanceIfHigher, null) as FactionGoal_AttackWithFleet;
							if (factionGoal_AttackWithFleet2 != null)
							{
								CS$<>8__locals1.attackGoals.Add(factionGoal_AttackWithFleet2);
								num2++;
							}
						}
					}
				}
			}
			int num6 = 0;
			using (IEnumerator<TINationState> enumerator3 = (from x in this.target().ref_faction.executiveNations
				orderby x.numControlPoints_unclamped descending, x.numStandardArmies descending
				select x).GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					TINationState nation = enumerator3.Current;
					if (FactionGoal_NeutralizeNation.ShouldNeutralizeNation(this.faction, nation))
					{
						foreach (TIFactionGoalState tifactionGoalState in this.faction.FindGoals(TIFactionGoalState.CaptureNationGoals, this.faction, nation, TIFactionState.GoalFilter.none, true).ToList<TIFactionGoalState>())
						{
							this.faction.RemoveGoal(tifactionGoalState);
						}
						this.faction.AddGoal(new FactionGoal_NeutralizeNation(this.faction, 15 + (CS$<>8__locals1.IsMostImportantWar ? 1 : 0), nation, this.objective), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					}
					else
					{
						TIFactionGoalState tifactionGoalState2 = this.faction.FindGoals(GoalType.NeutralizeNation, this.faction, nation, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>();
						if (tifactionGoalState2 != null)
						{
							this.faction.RemoveGoal(tifactionGoalState2);
						}
						if (num6 < 6 || this.faction.executiveNations.Any<TINationState>((TINationState x) => x.IsAdjacentToNation(nation, false)))
						{
							this.faction.AddGoal(new FactionGoal_CaptureNation_Dirty(this.faction, 15 + (CS$<>8__locals1.IsMostImportantWar ? 1 : 0), nation, GoalType.None, this.objective), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
						}
					}
					num6++;
				}
			}
			if (this.faction.IsAlienFaction)
			{
				this.faction.AddGoal(new FactionGoal_SecureEarthSpace(this.faction, Mathf.Min(base.importance, 19)), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
			}
			if (base.targetFaction.primaryHab != null && (base.targetFaction.IsActiveHumanFaction || !base.targetFaction.proAlien))
			{
				this.faction.AddGoal(new FactionGoal_AttackWithFleet(this.faction, ((!this.faction.proAlien && base.targetFaction.proAlien) || (this.faction.proAlien && !base.targetFaction.proAlien)) ? 20 : 10, base.targetFaction.primaryHab, true, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
			}
			foreach (FactionGoal_AttackWithFleet factionGoal_AttackWithFleet3 in CS$<>8__locals1.attackGoals)
			{
				if (!factionGoal_AttackWithFleet3.LeaveMyFleetAlone() && TIUtilities.RandomFloatValue() < 0.023333333f)
				{
					factionGoal_AttackWithFleet3.SetImportance(0);
				}
			}
		}

		// Token: 0x04002212 RID: 8722
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 1f },
			{ "Crackdown", 3f },
			{ "EnthrallElites", 3f },
			{ "EnthrallPublic", 2f },
			{ "EnthrallOrg", 30f },
			{ "Purge", 1f },
			{ "Unrest", 1f },
			{ "Propaganda", 2f },
			{ "SabotageFacilities", 1f },
			{ "TerrorizeRegion", 2f },
			{ "Assassinate", 1f },
			{ "DetectCouncilActivity", 3f },
			{ "Detain", 1f },
			{ "HostileTakeover", 30f },
			{ "InvestigateCouncilor", 2f },
			{ "SabotageProject", 1f },
			{ "StealProject", 2f },
			{ "Turn", 10f },
			{ "SeizeSpaceAsset", 1f },
			{ "ControlSpaceAsset", 1f },
			{ "SabotageHabModule", 1f }
		};

		// Token: 0x04002213 RID: 8723
		private static readonly List<GoalType> incompatibleGoalsForFaction = new List<GoalType>
		{
			GoalType.TruceWithFaction,
			GoalType.NonAggressionPact
		};

		// Token: 0x04002215 RID: 8725
		[SerializeField]
		private bool firstMaintenanceCompleted;
	}
}
