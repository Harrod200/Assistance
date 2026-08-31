using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200074C RID: 1868
	public class FactionGoal_DefendWithFleet : FactionGoal_Fleet
	{
		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x06002FD5 RID: 12245 RVA: 0x001049E3 File Offset: 0x00102BE3
		// (set) Token: 0x06002FD6 RID: 12246 RVA: 0x001049EB File Offset: 0x00102BEB
		public TIGameState defendTarget { get; protected set; }

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x06002FD7 RID: 12247 RVA: 0x001049F4 File Offset: 0x00102BF4
		// (set) Token: 0x06002FD8 RID: 12248 RVA: 0x001049FC File Offset: 0x00102BFC
		public string forceHullTemplateName { get; protected set; }

		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x06002FD9 RID: 12249 RVA: 0x00104A05 File Offset: 0x00102C05
		// (set) Token: 0x06002FDA RID: 12250 RVA: 0x00104A0D File Offset: 0x00102C0D
		public int EarmarkedFleetMC { get; set; }

		// Token: 0x06002FDB RID: 12251 RVA: 0x00104A16 File Offset: 0x00102C16
		public FactionGoal_DefendWithFleet()
		{
		}

		// Token: 0x06002FDC RID: 12252 RVA: 0x00104A1E File Offset: 0x00102C1E
		public FactionGoal_DefendWithFleet(TIFactionState faction, int importance, TIGameState defendTarget, string forceHullTemplateName = "")
		{
			this.faction = faction;
			base.SetImportance(importance);
			this.defendTarget = defendTarget;
			this.forceHullTemplateName = forceHullTemplateName;
		}

		// Token: 0x06002FDD RID: 12253 RVA: 0x00104A43 File Offset: 0x00102C43
		public static FactionGoal_DefendWithFleet CreateGoal(FactionGoal_DefendWithFleet p)
		{
			FactionGoal_DefendWithFleet factionGoal_DefendWithFleet = GameStateManager.CreateNewGameState<FactionGoal_DefendWithFleet>();
			factionGoal_DefendWithFleet.defendTarget = p.defendTarget;
			factionGoal_DefendWithFleet.forceHullTemplateName = p.forceHullTemplateName;
			return factionGoal_DefendWithFleet;
		}

		// Token: 0x06002FDE RID: 12254 RVA: 0x00104A62 File Offset: 0x00102C62
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_DefendWithFleet>(base.ID, false);
		}

		// Token: 0x06002FDF RID: 12255 RVA: 0x00104A71 File Offset: 0x00102C71
		public override GoalType GetGoalType()
		{
			return GoalType.DefendWithFleet;
		}

		// Token: 0x06002FE0 RID: 12256 RVA: 0x00104A75 File Offset: 0x00102C75
		public override TIGameState actor()
		{
			return base.assignedFleet;
		}

		// Token: 0x06002FE1 RID: 12257 RVA: 0x00104A7D File Offset: 0x00102C7D
		public override TIGameState target()
		{
			return this.defendTarget;
		}

		// Token: 0x06002FE2 RID: 12258 RVA: 0x00104A85 File Offset: 0x00102C85
		public override TIGameState location()
		{
			return this.defendTarget;
		}

		// Token: 0x06002FE3 RID: 12259 RVA: 0x00104A8D File Offset: 0x00102C8D
		public override TIGameState goalProduct()
		{
			return base.assignedFleet;
		}

		// Token: 0x06002FE4 RID: 12260 RVA: 0x00104A95 File Offset: 0x00102C95
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x06002FE5 RID: 12261 RVA: 0x00104A98 File Offset: 0x00102C98
		public override bool ValidNewGoal()
		{
			return TIGameState.Valid(this.defendTarget) && this.faction.CanExplore(this.defendTarget.ref_spaceObject);
		}

		// Token: 0x06002FE6 RID: 12262 RVA: 0x00104ABF File Offset: 0x00102CBF
		public override bool InProgress()
		{
			return base.assignedFleet != null;
		}

		// Token: 0x06002FE7 RID: 12263 RVA: 0x00104AD0 File Offset: 0x00102CD0
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || !TIGameState.Valid(this.defendTarget) || this.defendTarget.archived || (this.defendTarget.ref_faction != null && this.defendTarget.ref_faction != this.faction && !this.faction.permanentAlly(this.defendTarget.ref_faction)) || (this.defendTarget.isSpaceBodyState && this.defendTarget.ref_spaceBody.habSites.None<TIHabSiteState>((TIHabSiteState x) => x.hasPlannedOrOperatingBase && x.hab.faction == this.faction));
		}

		// Token: 0x06002FE8 RID: 12264 RVA: 0x00104B7B File Offset: 0x00102D7B
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x06002FE9 RID: 12265 RVA: 0x00104B7E File Offset: 0x00102D7E
		public override bool SpaceCombatGoal()
		{
			return true;
		}

		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06002FEA RID: 12266 RVA: 0x00104B81 File Offset: 0x00102D81
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_DefendWithFleet.fleetOps;
			}
		}

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06002FEB RID: 12267 RVA: 0x00104B88 File Offset: 0x00102D88
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_DefendWithFleet.incompatibleFleetGoals;
			}
		}

		// Token: 0x06002FEC RID: 12268 RVA: 0x00104B8F File Offset: 0x00102D8F
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.NoRole;
		}

		// Token: 0x06002FED RID: 12269 RVA: 0x00104B92 File Offset: 0x00102D92
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return FactionGoal_DefendWithFleet.preferredRoles;
		}

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x06002FEE RID: 12270 RVA: 0x00104B9C File Offset: 0x00102D9C
		public bool IsPrimarySystemDefender
		{
			get
			{
				TIHabState primaryHab = this.faction.primaryHab;
				if (((primaryHab != null) ? primaryHab.ref_system : null) != null)
				{
					TIGameState tigameState = this.target();
					return ((tigameState != null) ? tigameState.ref_system : null) == this.faction.primaryHab.ref_system;
				}
				return false;
			}
		}

		// Token: 0x06002FEF RID: 12271 RVA: 0x00104BF1 File Offset: 0x00102DF1
		public override void ChangeTarget(TIGameState newTarget)
		{
			this.defendTarget = newTarget;
		}

		// Token: 0x06002FF0 RID: 12272 RVA: 0x00104BFA File Offset: 0x00102DFA
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x06002FF1 RID: 12273 RVA: 0x00104C00 File Offset: 0x00102E00
		public override float ComputeDesiredFleetCombatValue()
		{
			FactionGoal_DefendWithFleet.<>c__DisplayClass41_0 CS$<>8__locals1 = new FactionGoal_DefendWithFleet.<>c__DisplayClass41_0();
			CS$<>8__locals1.<>4__this = this;
			float num = base.ComputeDesiredFleetCombatValue();
			TIGameState tigameState = this.target();
			TISpaceObjectState getSunOrbitingRelatedObject = tigameState.ref_spaceObject.GetSunOrbitingRelatedObject;
			TIHabState ref_hab = tigameState.ref_hab;
			CS$<>8__locals1.systemFleetStrengths = AIEvaluators.SystemFleetStrengths;
			CS$<>8__locals1.GetPerceivedSystemFleetStrength = delegate(TISpaceObjectState system, TIFactionState otherFaction)
			{
				Dictionary<TIFactionState, float> dictionary;
				float num16;
				if (CS$<>8__locals1.systemFleetStrengths.TryGetValue(system, out dictionary) && dictionary.TryGetValue(otherFaction, out num16))
				{
					return num16 * CS$<>8__locals1.<>4__this.faction.GetPerceivedEnemyFleetStrengthFactor(otherFaction);
				}
				return 0f;
			};
			float num2 = AIEvaluators.GetAttackableFactions(this.faction).Max<TIFactionState>(delegate(TIFactionState enemy)
			{
				FactionGoal_DefendWithFleet.<>c__DisplayClass41_1 CS$<>8__locals3 = new FactionGoal_DefendWithFleet.<>c__DisplayClass41_1();
				CS$<>8__locals3.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals3.enemy = enemy;
				float num17 = 0f;
				if (CS$<>8__locals1.systemFleetStrengths.Count > 0)
				{
					num17 = CS$<>8__locals1.systemFleetStrengths.Keys.Max<TISpaceObjectState>(new Func<TISpaceObjectState, float>(CS$<>8__locals3.<ComputeDesiredFleetCombatValue>g__selector|4)) * CS$<>8__locals1.<>4__this.faction.AI_ModifiedRiskAversion();
				}
				return AIEvaluators.GetRequiredDefenseStrength(CS$<>8__locals1.<>4__this.faction, CS$<>8__locals3.enemy, num17, null);
			});
			float num3 = this.faction.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
			float num4 = 0.15f / (float)(this.faction.stations.Count + (from x in this.faction.bases
				group x by x.ref_spaceBody).Count<IGrouping<TISpaceBodyState, TIHabState>>());
			float num5 = 0f;
			float num6 = 1f;
			if (this.faction.IsAlienFaction)
			{
				IEnumerable<TIFactionState> enumerable = from x in AIEvaluators.GetAttackableFactions(this.faction)
					where !x.isAlienAppeaser
					select x;
				TIFactionState strongestHumanFaction = AIEvaluators.GetStrongestHumanFaction((TIFactionState x) => !CS$<>8__locals1.<>4__this.faction.permanentAlly(x) && !x.isAlienAppeaser);
				TIHabState mainBaseInSystem = this.faction.GetMainBaseInSystem(this.faction.GetInnermostColonizedPlanet());
				float encroachingAU = (float)GameStateManager.Jupiter().semiMajorAxis_AU;
				if (mainBaseInSystem != null)
				{
					encroachingAU = (float)mainBaseInSystem.ref_system.semiMajorAxis_AU;
				}
				Func<TIHabState, bool> <>9__12;
				List<TIFactionState> list = enumerable.Where<TIFactionState>(delegate(TIFactionState x)
				{
					IEnumerable<TIHabState> habs = x.habs;
					Func<TIHabState, bool> func;
					if ((func = <>9__12) == null)
					{
						func = (<>9__12 = (TIHabState y) => y.ref_system.semiMajorAxis_AU >= (double)encroachingAU);
					}
					return habs.Any<TIHabState>(func);
				}).ToList<TIFactionState>();
				float num7 = ((list.Count > 0) ? 0.5f : 0f);
				if (list.Contains(strongestHumanFaction))
				{
					num7 = 1f;
				}
				float num8 = AIEvaluators.FactionsGoToWarProgress(this.faction, strongestHumanFaction);
				if (this.faction.enemyWarFactions.Count >= 3)
				{
					num8 = 1f;
				}
				float num9 = (float)(this.faction.enemyTotalWarFactions.Contains(strongestHumanFaction) ? 1 : 0);
				if (this.faction.enemyTotalWarFactions.Count >= 3)
				{
					num9 = 1f;
				}
				float num10 = Mathf.Clamp(strongestHumanFaction.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()) * this.faction.GetPerceivedEnemyFleetStrengthFactor(strongestHumanFaction) / num3, 0f, 1.5f);
				List<TIFactionState> list2 = (from x in enumerable
					where CS$<>8__locals1.<>4__this.faction.GetHighestIntel(x) >= TemplateManager.global.intelToSeeFactionObjectives
					where x.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Unlocked).Count > 0
					select x).ToList<TIFactionState>();
				float num11 = ((list2.Count > 0) ? 0.25f : 0f);
				if (list2.Where<TIFactionState>((TIFactionState x) => x.antiAlien).Count<TIFactionState>() > 0)
				{
					num11 = 0.5f;
				}
				if (strongestHumanFaction.antiAlien && list2.Contains(strongestHumanFaction))
				{
					num11 = 1f;
				}
				if (tigameState == this.faction.primaryHab.ref_spaceBody)
				{
					num4 = (0.09f + Mathf.Max(num10 * 0.15f, num7 * 0.05f + num8 * 0.05f + num9 * 0.05f)) * (1f + num11 * 0.5f);
					num5 = 0.2f + Mathf.Max(num8 * 0.05f + num9 * 0.05f, num11 * 0.2f);
					num6 = Mathf.Clamp(0.5f + num7 * 0.25f + num8 * 0.25f + num11 * 0.5f + num10 * 0.5f, 0f, 1f);
				}
				else if (getSunOrbitingRelatedObject == ((mainBaseInSystem != null) ? mainBaseInSystem.ref_system : null))
				{
					num4 = (0.17f + Mathf.Max(num10 * 0.08f, num7 * 0.025f + num8 * 0.025f + num9 * 0.03f)) * (1f - num11 * 0.15f);
					num5 = 0.4f;
				}
				else
				{
					num4 *= 1f - num11 * 0.6f;
					if (ref_hab != null && ref_hab.IsStation)
					{
						num4 *= (float)ref_hab.tier / 3f / 2f;
					}
				}
			}
			else if (ref_hab != null && ref_hab == this.faction.primaryHab)
			{
				num4 = 0.3f;
				num5 = 0.2f;
			}
			num2 *= num6;
			float num12 = num3 * num4;
			if (num2 > num12)
			{
				num2 = Mathf.Lerp(num12, num2, num5);
			}
			num = Mathf.Max(num, num2);
			foreach (TIFactionState tifactionState in AIEvaluators.GetAttackableFactions(this.faction))
			{
				float num13 = 0f;
				if (CS$<>8__locals1.systemFleetStrengths.ContainsKey(getSunOrbitingRelatedObject) && CS$<>8__locals1.systemFleetStrengths[getSunOrbitingRelatedObject].ContainsKey(tifactionState))
				{
					num13 = CS$<>8__locals1.systemFleetStrengths[getSunOrbitingRelatedObject][tifactionState] * this.faction.GetPerceivedEnemyFleetStrengthFactor(tifactionState);
				}
				if (AIEvaluators.IsDefenseFeasible(this.faction, tigameState, num13))
				{
					float requiredDefenseStrength = AIEvaluators.GetRequiredDefenseStrength(this.faction, tifactionState, num13, ref_hab);
					num = Mathf.Max(num, requiredDefenseStrength);
				}
			}
			if (AIEvaluators.ShouldSystemBeInDefenseMode(this.faction, this.target().ref_system))
			{
				if (this.IsPrimarySystemDefender)
				{
					num *= 1.5f;
				}
				else if (this.target() is TISpaceBodyState)
				{
					num *= 1.25f;
				}
			}
			if (ref_hab != null)
			{
				num -= ref_hab.SpaceCombatValue();
			}
			if (!this.faction.IsAlienFaction)
			{
				num *= 0.6f;
			}
			float num14 = this.faction.GetTypicalShipSpaceCombatValue() / this.faction.GetTypicalShipMissionControlConsumption();
			float num15 = (float)this.EarmarkedFleetMC * num14;
			this.desiredFleetCombatValue_sansEarmarked = Mathf.Max(TemplateManager.global.minimumFleetStrength, num);
			float strengthNeededToDealWithCampers = this.GetStrengthNeededToDealWithCampers();
			return Mathf.Max(new float[] { this.desiredFleetCombatValue_sansEarmarked, num15, strengthNeededToDealWithCampers });
		}

		// Token: 0x06002FF2 RID: 12274 RVA: 0x00105274 File Offset: 0x00103474
		public IEnumerable<TISpaceFleetState> GetCampers()
		{
			TIGameState tigameState = this.target();
			TIHabState tihabState = ((tigameState != null) ? tigameState.ref_hab : null);
			if (tihabState == null)
			{
				return Enumerable.Empty<TISpaceFleetState>();
			}
			return from x in tihabState.dockedFleets
				where x.faction != null
				where !this.faction.permanentAlly(x.faction)
				select x;
		}

		// Token: 0x06002FF3 RID: 12275 RVA: 0x001052E0 File Offset: 0x001034E0
		public float GetStrengthNeededToDealWithCampers()
		{
			TIGameState tigameState = this.target();
			if (((tigameState != null) ? tigameState.ref_hab : null) == null)
			{
				return 0f;
			}
			return this.GetCampers().Sum<TISpaceFleetState>((TISpaceFleetState x) => this.faction.GetPerceivedEnemyFleetStrength(x)) * AIEvaluators.GetAdjustedFleetSuperiorityFactor(this.faction);
		}

		// Token: 0x06002FF4 RID: 12276 RVA: 0x00105330 File Offset: 0x00103530
		public override float GetForcePursueFleetCombatValue(TISpaceFleetState enemyFleet, TIHabState hab)
		{
			if (hab != null)
			{
				TIGameState tigameState = this.target();
				if (hab == ((tigameState != null) ? tigameState.ref_hab : null))
				{
					return this.GetStrengthNeededToDealWithCampers();
				}
			}
			return base.GetForcePursueFleetCombatValue(enemyFleet, hab);
		}

		// Token: 0x06002FF5 RID: 12277 RVA: 0x00105364 File Offset: 0x00103564
		public override float GetMaximumFleetCombatValueRatio()
		{
			TIGameState tigameState = this.target();
			bool? flag;
			if (tigameState == null)
			{
				flag = null;
			}
			else
			{
				TISpaceBodyState ref_system = tigameState.ref_system;
				flag = ((ref_system != null) ? new bool?(ref_system.isEarth) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				return 1f;
			}
			if (this.faction.ShouldIncreaseStaticFleetFraction())
			{
				return float.PositiveInfinity;
			}
			if (!AIEvaluators.ShouldSystemBeInDefenseMode(this.faction, this.target().ref_system))
			{
				return 1f;
			}
			if (this.IsPrimarySystemDefender)
			{
				return 4f;
			}
			return 2f;
		}

		// Token: 0x06002FF6 RID: 12278 RVA: 0x001053F8 File Offset: 0x001035F8
		public override bool ReadyForTransferToTarget(TISpaceFleetState fleet)
		{
			if (base.ReadyForTransferToTarget(fleet))
			{
				return true;
			}
			if (fleet.ref_system != null)
			{
				TIGameState ref_system = fleet.ref_system;
				TIGameState tigameState = this.target();
				if (ref_system == ((tigameState != null) ? tigameState.ref_system : null))
				{
					return fleet.SpaceCombatValue() >= this.GetStrengthNeededToDealWithCampers();
				}
			}
			float num = base.desiredFleetCombatValue;
			if (this.desiredFleetCombatValue_sansEarmarked > 0f)
			{
				num = Mathf.Min(num, this.desiredFleetCombatValue_sansEarmarked);
			}
			return fleet.SpaceCombatValue() >= num;
		}

		// Token: 0x06002FF7 RID: 12279 RVA: 0x0010547C File Offset: 0x0010367C
		public override bool LeaveMyFleetAlone()
		{
			return this.EarmarkedFleetMC > 0;
		}

		// Token: 0x06002FF8 RID: 12280 RVA: 0x00105487 File Offset: 0x00103687
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x06002FF9 RID: 12281 RVA: 0x0010548E File Offset: 0x0010368E
		public override TIShipHullTemplate desiredFlagshipHull
		{
			get
			{
				return TemplateManager.Find<TIShipHullTemplate>(this.forceHullTemplateName, false);
			}
		}

		// Token: 0x04002250 RID: 8784
		private static readonly List<GoalType> incompatibleFleetGoals = new List<GoalType>
		{
			GoalType.AttackWithFleet,
			GoalType.CaptureHab,
			GoalType.TransportCouncilorsViaFleet
		};

		// Token: 0x04002251 RID: 8785
		public static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList)
		{
			typeof(BombardOperation_High),
			typeof(BombardOperation_Med),
			typeof(BombardOperation_Low),
			typeof(DestroyHabOperation),
			typeof(AssaultHabOperation)
		};

		// Token: 0x04002252 RID: 8786
		private static readonly Dictionary<ShipRole, float> preferredRoles = new Dictionary<ShipRole, float>
		{
			{
				ShipRole.ML_Standoff,
				0.5f
			},
			{
				ShipRole.MM_SpaceSuperiority,
				0.5f
			},
			{
				ShipRole.MS_Strike,
				0.5f
			},
			{
				ShipRole.SL_Defender,
				1f
			},
			{
				ShipRole.SM_Patrol,
				1f
			},
			{
				ShipRole.SS_Interceptor,
				1f
			},
			{
				ShipRole.LM_Protector,
				0.75f
			}
		};

		// Token: 0x04002253 RID: 8787
		private float desiredFleetCombatValue_sansEarmarked;
	}
}
