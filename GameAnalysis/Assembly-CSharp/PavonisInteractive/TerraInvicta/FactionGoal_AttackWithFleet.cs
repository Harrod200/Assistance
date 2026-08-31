using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200074E RID: 1870
	public class FactionGoal_AttackWithFleet : FactionGoal_Fleet
	{
		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x06003009 RID: 12297 RVA: 0x00105729 File Offset: 0x00103929
		// (set) Token: 0x0600300A RID: 12298 RVA: 0x00105731 File Offset: 0x00103931
		public TIGameState attackTarget { get; protected set; }

		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x0600300B RID: 12299 RVA: 0x0010573A File Offset: 0x0010393A
		// (set) Token: 0x0600300C RID: 12300 RVA: 0x00105742 File Offset: 0x00103942
		public TIFactionState enemyFaction { get; protected set; }

		// Token: 0x0600300D RID: 12301 RVA: 0x0010574C File Offset: 0x0010394C
		public FactionGoal_AttackWithFleet()
		{
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x0600300E RID: 12302 RVA: 0x00105803 File Offset: 0x00103A03
		public bool bombardmentGoal
		{
			get
			{
				return this.attackTarget != null && ((this.attackTarget.isHabState && this.attackTarget.ref_hab.IsBase) || this.attackTarget.hasEarthMapObject);
			}
		}

		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x0600300F RID: 12303 RVA: 0x00105841 File Offset: 0x00103A41
		// (set) Token: 0x06003010 RID: 12304 RVA: 0x00105849 File Offset: 0x00103A49
		public TIGameState colonizationTarget { get; private set; }

		// Token: 0x06003011 RID: 12305 RVA: 0x00105854 File Offset: 0x00103A54
		public FactionGoal_AttackWithFleet(TIFactionState faction, int importance, TIGameState attackTarget, bool requiresWar = false, TIObjectiveTemplate objective = null, bool colonizeAfterwards = false)
		{
			this.faction = faction;
			base.SetImportance(importance);
			this.attackTarget = attackTarget;
			this.enemyFaction = attackTarget.ref_faction;
			this.requiresWar = requiresWar;
			this.objective = objective;
			if (colonizeAfterwards && this.target().ref_habSite != null)
			{
				this.colonizationTarget = this.target().ref_habSite;
			}
		}

		// Token: 0x06003012 RID: 12306 RVA: 0x00105964 File Offset: 0x00103B64
		public static FactionGoal_AttackWithFleet CreateGoal(FactionGoal_AttackWithFleet p)
		{
			FactionGoal_AttackWithFleet factionGoal_AttackWithFleet = GameStateManager.CreateNewGameState<FactionGoal_AttackWithFleet>();
			factionGoal_AttackWithFleet.attackTarget = p.attackTarget;
			factionGoal_AttackWithFleet.enemyFaction = p.enemyFaction;
			factionGoal_AttackWithFleet.requiresWar = p.requiresWar;
			factionGoal_AttackWithFleet.colonizationTarget = p.colonizationTarget;
			factionGoal_AttackWithFleet.objective = p.objective;
			return factionGoal_AttackWithFleet;
		}

		// Token: 0x06003013 RID: 12307 RVA: 0x001059B2 File Offset: 0x00103BB2
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_AttackWithFleet>(base.ID, false);
		}

		// Token: 0x06003014 RID: 12308 RVA: 0x001059C4 File Offset: 0x00103BC4
		public override void OnGoalComplete()
		{
			TISpaceFleetState assignedFleet = base.assignedFleet;
			TIGameState attackTarget = this.attackTarget;
			base.OnGoalComplete();
			if (assignedFleet != null)
			{
				TISpaceAssetState nearbySpaceAssetTarget = assignedFleet.GetNearbySpaceAssetTarget();
				if (nearbySpaceAssetTarget != null)
				{
					TIFactionGoalState tifactionGoalState = this.faction.AddGoal(new FactionGoal_AttackWithFleet(this.faction, 15, nearbySpaceAssetTarget, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					if (tifactionGoalState != null)
					{
						(tifactionGoalState as FactionGoal_Fleet).AssignFleet(assignedFleet);
					}
					else
					{
						string text = "Could not create goal to attack followup target ";
						TISpaceAssetState tispaceAssetState = nearbySpaceAssetTarget;
						Log.Error(text + ((tispaceAssetState != null) ? tispaceAssetState.ToString() : null), Array.Empty<object>());
					}
				}
			}
			if (this.colonizationTarget != null)
			{
				TIHabSiteState tihabSiteState = this.colonizationTarget as TIHabSiteState;
				if (tihabSiteState != null)
				{
					this.faction.AddGoal(new FactionGoal_FoundBase(this.faction, Mathf.Max(base.importance, 15), tihabSiteState, GoalType.BuildFullBase, null, GoalType.None, false, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				}
			}
		}

		// Token: 0x06003015 RID: 12309 RVA: 0x00105A9E File Offset: 0x00103C9E
		public override GoalType GetGoalType()
		{
			return GoalType.AttackWithFleet;
		}

		// Token: 0x06003016 RID: 12310 RVA: 0x00105AA2 File Offset: 0x00103CA2
		public override TIGameState actor()
		{
			return base.assignedFleet;
		}

		// Token: 0x06003017 RID: 12311 RVA: 0x00105AAA File Offset: 0x00103CAA
		public override TIGameState target()
		{
			return this.attackTarget;
		}

		// Token: 0x06003018 RID: 12312 RVA: 0x00105AB2 File Offset: 0x00103CB2
		public override TIGameState location()
		{
			return this.attackTarget;
		}

		// Token: 0x06003019 RID: 12313 RVA: 0x00105ABA File Offset: 0x00103CBA
		public override TIGameState goalProduct()
		{
			return base.assignedFleet;
		}

		// Token: 0x0600301A RID: 12314 RVA: 0x00105AC2 File Offset: 0x00103CC2
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x0600301B RID: 12315 RVA: 0x00105AC8 File Offset: 0x00103CC8
		public override bool ValidNewGoal()
		{
			if (TIGameState.Valid(this.attackTarget) && !this.attackTarget.archived && this.attackTarget.ref_spaceObject != null && this.faction.CanExplore(this.attackTarget.ref_spaceObject))
			{
				TIFactionState ref_faction = this.attackTarget.ref_faction;
				if (ref_faction == null || !ref_faction.permanentAlly(this.faction))
				{
					TISpaceFleetState ref_fleet = this.attackTarget.ref_fleet;
					bool? flag;
					if (ref_fleet == null)
					{
						flag = null;
					}
					else
					{
						Trajectory trajectory = ref_fleet.trajectory;
						flag = ((trajectory != null) ? new bool?(!trajectory.destroyOnArrival) : null);
					}
					return flag ?? true;
				}
			}
			return false;
		}

		// Token: 0x0600301C RID: 12316 RVA: 0x00105B95 File Offset: 0x00103D95
		public override bool InProgress()
		{
			return base.assignedFleet != null && (base.assignedFleet.inTransfer || base.assignedFleet.ref_system == this.attackTarget.ref_system);
		}

		// Token: 0x0600301D RID: 12317 RVA: 0x00105BD4 File Offset: 0x00103DD4
		public override bool LeaveMyFleetAlone()
		{
			if (!TIGameState.Valid(this.target()))
			{
				return false;
			}
			if (base.objectiveGoal)
			{
				return true;
			}
			if (!this.InProgress())
			{
				return false;
			}
			if (!this.requiresWar)
			{
				return true;
			}
			if (!this.faction.IsAlienFaction)
			{
				return true;
			}
			float num = AIEvaluators.FactionsGoToWarProgress(this.faction, this.target().ref_faction);
			float alienCallOffWarAttacksThreshold = TemplateManager.global.GetAlienCallOffWarAttacksThreshold();
			return num > alienCallOffWarAttacksThreshold;
		}

		// Token: 0x0600301E RID: 12318 RVA: 0x00105C44 File Offset: 0x00103E44
		public override bool ShouldDiscardGoal()
		{
			if (base.importance <= 0)
			{
				return true;
			}
			if (!TIGameState.Valid(this.attackTarget) || this.attackTarget.archived)
			{
				return true;
			}
			if (base.importance < 20 && !this.requiresWar && !base.objectiveGoal && base.Age_years >= 5f && !this.InProgress())
			{
				return true;
			}
			TIFactionState ref_faction = this.attackTarget.ref_faction;
			if (ref_faction != null && ref_faction.permanentAlly(this.faction))
			{
				return true;
			}
			if (this.enemyFaction != null && this.enemyFaction != this.attackTarget.ref_faction)
			{
				return true;
			}
			if (this.attackTarget.isSpaceFleetState)
			{
				if (!this.faction.CanExplore(this.attackTarget.ref_fleet.ref_naturalSpaceObject))
				{
					return true;
				}
				Trajectory trajectory = this.attackTarget.ref_fleet.trajectory;
				if (trajectory != null && trajectory.destroyOnArrival)
				{
					return true;
				}
			}
			if (this.LeaveMyFleetAlone())
			{
				return false;
			}
			if (!base.objectiveGoal && this.requiresWar && !this.faction.enemyWarFactions.Contains(this.attackTarget.ref_faction))
			{
				return true;
			}
			if (this.faction.IsAlienFaction && this.attackTarget.ref_system != null)
			{
				if ((from x in (from x in this.faction.GoalsOfType(GoalType.AttackWithFleet, false, false)
						select x as FactionGoal_AttackWithFleet into x
						where x != this
						select x.assignedFleet into x
						where x != null
						select x).Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
					{
						TIGameState tigameState;
						if (!x.inTransfer)
						{
							tigameState = x.ref_system;
						}
						else
						{
							TISpaceGameState destination = x.trajectory.destination;
							tigameState = ((destination != null) ? destination.ref_system : null);
						}
						return tigameState == this.attackTarget.ref_system;
					})
					where x.SpaceCombatValue() >= base.desiredFleetCombatValue
					select x).Any<TISpaceFleetState>())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600301F RID: 12319 RVA: 0x00105E58 File Offset: 0x00104058
		public override bool GoalFulfilled()
		{
			return this.attackTarget == null || this.attackTarget.archived || this.attackTarget.deleted || (this.target().isRegionSpaceFacility && !this.target().ref_regionSpaceFacility.Extant()) || (this.target().isRegionAlienEntity && !this.target().ref_regionAlienEntity.Extant());
		}

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06003020 RID: 12320 RVA: 0x00105ED0 File Offset: 0x001040D0
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_AttackWithFleet.fleetOps;
			}
		}

		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x06003021 RID: 12321 RVA: 0x00105ED7 File Offset: 0x001040D7
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_AttackWithFleet.incompatibleFleetGoals;
			}
		}

		// Token: 0x06003022 RID: 12322 RVA: 0x00105EDE File Offset: 0x001040DE
		public override bool SpaceCombatGoal()
		{
			return true;
		}

		// Token: 0x06003023 RID: 12323 RVA: 0x00105EE1 File Offset: 0x001040E1
		public override ShipRole GetPrimaryShipRole()
		{
			if (!this.bombardmentGoal)
			{
				return ShipRole.NoRole;
			}
			return ShipRole.LL_Bomber;
		}

		// Token: 0x06003024 RID: 12324 RVA: 0x00105EEF File Offset: 0x001040EF
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			if (!this.bombardmentGoal)
			{
				return this.preferredRoles_spaceTarget;
			}
			return this.preferredRoles_bombardment;
		}

		// Token: 0x06003025 RID: 12325 RVA: 0x00105F06 File Offset: 0x00104106
		public override void ChangeTarget(TIGameState newTarget)
		{
			this.attackTarget = newTarget;
			if (!this.ValidNewGoal())
			{
				base.SetImportance(0);
			}
		}

		// Token: 0x06003026 RID: 12326 RVA: 0x00105F1E File Offset: 0x0010411E
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x06003027 RID: 12327 RVA: 0x00105F24 File Offset: 0x00104124
		public static float ComputeDesiredFleetCombatValueForAttack(TIFactionState faction, TIGameState target, bool onlyConsiderTarget = false, bool isReinforcement = false)
		{
			if (target == null || target.ref_system == null)
			{
				return 0f;
			}
			float num = FactionGoal_Fleet.ComputeBaselineFleetCombatValue(faction, target);
			if (onlyConsiderTarget)
			{
				num = TemplateManager.global.minimumFleetStrength;
			}
			if (target.deleted || target.ref_faction == null)
			{
				return num;
			}
			float num2 = 0f;
			if (target.ref_hab != null && target.ref_hab.IsStation)
			{
				num2 = target.ref_hab.PerceivedAggregateDefensiveScore_Station(faction);
			}
			else if (target.isSpaceFleetState)
			{
				IEnumerable<TISpaceFleetState> enumerable;
				if (isReinforcement)
				{
					enumerable = Enumerable.Repeat<TISpaceFleetState>(target.ref_fleet, 1);
				}
				else
				{
					enumerable = TIFactionState.GetDefenders(target.ref_fleet);
				}
				num2 = enumerable.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()) * faction.GetPerceivedEnemyFleetStrengthFactor(target.ref_faction);
			}
			float num3 = num2 * AIEvaluators.GetAdjustedFleetSuperiorityFactor(faction) + 1f;
			IEnumerable<TISpaceShipState> enumerable2 = from x in faction.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships)
				where x.combatant
				select x;
			if (enumerable2.Any<TISpaceShipState>() && ((target.ref_hab != null && target.ref_hab.IsStation) || target.isSpaceFleetState))
			{
				float num4 = enumerable2.Average<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f));
				float num5;
				if (target.isSpaceFleetState)
				{
					num5 = Mathf.Max((float)target.ref_fleet.ships.Count, num2 / num4);
				}
				else
				{
					num5 = num2 / num4;
				}
				float num6 = 1f + Mathf.Pow(1f - Mathf.Clamp(num5 / 12f, 0f, 1f), 1.3f) * 1.5f;
				float num7 = num3 * num6;
				float num8 = 0f;
				TISpaceObjectState tispaceObjectState = target as TISpaceObjectState;
				if (tispaceObjectState != null)
				{
					int num9 = TIFactionState.GetDefenders(tispaceObjectState).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.ships.Count);
					if (target.ref_hab != null)
					{
						num9 += target.ref_hab.ActiveCombatModules().Count;
					}
					num8 = num4 * (float)num9 * Mathf.Pow(num6, 0.5f);
				}
				if (faction.IsAlienFaction)
				{
					num7 = num3 * Mathf.Pow(num7 / num3, 0.5f);
					num8 = num3 * Mathf.Pow(num8 / num3, 0.5f);
				}
				num3 = Mathf.Max(new float[] { num3, num7, num8 });
			}
			return Mathf.Max(num, num3);
		}

		// Token: 0x06003028 RID: 12328 RVA: 0x001061EA File Offset: 0x001043EA
		public override float ComputeDesiredFleetCombatValue()
		{
			return FactionGoal_AttackWithFleet.ComputeDesiredFleetCombatValueForAttack(this.faction, this.target(), false, false);
		}

		// Token: 0x06003029 RID: 12329 RVA: 0x00106200 File Offset: 0x00104400
		public override float GetMaximumFleetCombatValueRatio()
		{
			float num = 3f;
			if (this.faction.HasUpkeepInsecurity())
			{
				num = 2f;
			}
			else if (this.faction.FuelEfficiencyMode())
			{
				num = 2.5f;
			}
			float resourceBasedMaximumFleetSize = this.GetResourceBasedMaximumFleetSize();
			float num2 = resourceBasedMaximumFleetSize * this.faction.GetTypicalShipSpaceCombatValue() / base.desiredFleetCombatValue;
			num = Mathf.Min(num, num2);
			if (this.bombardmentGoal)
			{
				if (resourceBasedMaximumFleetSize * this.faction.GetTypicalShipBombardmentValue(this.target().ref_spaceBody) < this.GetDesiredBombardmentValue())
				{
					num = 0f;
				}
				else
				{
					num = num2;
				}
			}
			num = Mathf.Min(new float[]
			{
				num,
				TemplateManager.global.AI_GetDifficultyBasedMaxAttackFleetStrengthRatio(this.faction.IsAlienFaction),
				TemplateManager.global.maxAttackFleetRatio_AllCases
			});
			if (num < 1f)
			{
				num = 0f;
			}
			return num;
		}

		// Token: 0x0600302A RID: 12330 RVA: 0x001062D8 File Offset: 0x001044D8
		public static float ComputeKillValue(TIFactionState faction, TIGameState target)
		{
			if (((target != null) ? target.ref_faction : null) == null || (!target.isHabState && !target.isSpaceFleetState))
			{
				return 0f;
			}
			float num = 0f;
			float num2 = 0f;
			if ((target.isHabState && target.ref_hab.IsStation) || (target.isSpaceFleetState && target.ref_fleet.ref_fleet.dockedAtStation))
			{
				num += target.ref_hab.SpaceCombatValueFromDefendingFleets();
			}
			else if (target.isSpaceFleetState)
			{
				num += target.ref_fleet.SpaceCombatValue();
			}
			if (target.ref_hab != null && !target.ref_hab.faction.permanentAlly(faction))
			{
				num2 += (float)target.ref_hab.crew;
			}
			float num3 = num;
			TIFactionState ref_faction = target.ref_faction;
			float num4;
			if (ref_faction == null)
			{
				num4 = 0f;
			}
			else
			{
				num4 = ref_faction.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
			}
			float num5 = num3 / Mathf.Max(num4, 1f);
			float num6 = num2;
			TIFactionState ref_faction2 = target.ref_faction;
			int num7;
			if (ref_faction2 == null)
			{
				num7 = 0;
			}
			else
			{
				num7 = ref_faction2.habs.Sum<TIHabState>((TIHabState x) => x.crew);
			}
			float num8 = num5 + num6 / (float)Mathf.Max(num7, 1);
			TIFactionState ref_faction3 = target.ref_faction;
			if (ref_faction3 != null && ref_faction3.IsAlienFaction)
			{
				num8 *= 1.65f + faction.ideologyCoordinates.x / 2f;
				if (faction.enemyWarFactions.Contains(target.ref_faction))
				{
					num8 *= 1.5f;
				}
			}
			else
			{
				TIFactionState mostThreateningEnemyHumanFaction = faction.GetMostThreateningEnemyHumanFaction();
				if (target.ref_faction != null && target.ref_faction == mostThreateningEnemyHumanFaction)
				{
					num8 *= 10f;
				}
				TIFactionState mostThreateningWarEnemyHumanFaction = faction.GetMostThreateningWarEnemyHumanFaction();
				if (target.ref_faction != null && target.ref_faction == mostThreateningWarEnemyHumanFaction)
				{
					num8 *= 1.5f;
				}
			}
			return Mathf.Clamp01(num8);
		}

		// Token: 0x0600302B RID: 12331 RVA: 0x001064D0 File Offset: 0x001046D0
		public static float GetResourceBasedMaximumFleetSize(TIFactionState faction, TIGameState target, float relativeImportance = 1f, float timeCost_days = 0f, float dvCost_kps = 0f, IEnumerable<TISpaceShipTemplate> exampleShips = null, int hypotheticalShipCount = -1, float hypotheticalFleetStrength = -1f)
		{
			if (target == null)
			{
				return float.PositiveInfinity;
			}
			float num = FactionGoal_AttackWithFleet.ComputeKillValue(faction, target);
			float num2 = relativeImportance * 365.2422f * 0.1f * (0.1f + num * 25f);
			float num3 = (float)faction.ships.Count * Mathf.Clamp01(num2 / timeCost_days);
			if (hypotheticalFleetStrength < 0f || hypotheticalShipCount < 0)
			{
				hypotheticalFleetStrength = FactionGoal_AttackWithFleet.ComputeDesiredFleetCombatValueForAttack(faction, target, timeCost_days == 0f, false);
				hypotheticalShipCount = (hypotheticalFleetStrength / faction.GetTypicalShipSpaceCombatValue()).RoundUp();
			}
			float num4 = 0f;
			if ((target.ref_hab != null && target.ref_hab.IsBase && target.ref_hab.SpaceCombatValue() > 0f) || (target.ref_region != null && !target.ref_region.antiSpaceDefenses))
			{
				num4 = 0.15f;
			}
			else if ((target.ref_hab != null && target.ref_hab.SpaceCombatValue() > 0f) || target.ref_fleet != null)
			{
				TIHabState ref_hab = target.ref_hab;
				float num5 = ((ref_hab != null) ? ref_hab.PerceivedAggregateDefensiveScore_Station(faction) : faction.GetPerceivedEnemyFleetStrength(target.ref_fleet));
				float num6 = 1f;
				TIHabState ref_hab2 = target.ref_hab;
				float num7 = num6 - ((ref_hab2 != null) ? ref_hab2.SpaceCombatValue() : 0f) / num5;
				float num8 = 1f;
				float num9 = 1f;
				float num10 = 1.35f;
				TISpaceFleetState ref_fleet = target.ref_fleet;
				float num11 = Mathf.Pow(Mathf.Min((num8 + num9 / Mathf.Pow(num10, (float)(((ref_fleet != null) ? ref_fleet.ships.Count<TISpaceShipState>() : 0) + Mathf.Max(hypotheticalShipCount, 1)))) * num5 / hypotheticalFleetStrength, 1f), 1.8f);
				num4 = Mathf.Lerp(0.2f, num11, num7);
			}
			float maximumDaysOfIncomeUsed = relativeImportance * 30f * (0.1f + num * 20f);
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			if (exampleShips != null)
			{
				using (IEnumerator<TISpaceShipTemplate> enumerator = exampleShips.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceShipTemplate tispaceShipTemplate = enumerator.Current;
						float num12 = dvCost_kps / tispaceShipTemplate.baseCruiseDeltaV_kps(false);
						TIResourcesCost tiresourcesCost2 = tispaceShipTemplate.propellantTanksBuildCost(faction);
						tiresourcesCost.SumCosts_NoDuration(tiresourcesCost2.MultiplyCost(num12 / (float)exampleShips.Count<TISpaceShipTemplate>()));
					}
					goto IL_0267;
				}
			}
			tiresourcesCost = faction.GetTypicalShipFuelCostPerKPSSansRareMaterials();
			IL_0267:
			TIResourcesCost tiresourcesCost3 = faction.GetTypicalShipBuildCostSansRareMaterials().MultiplyCost(num4);
			tiresourcesCost3.SumCosts_NoDuration(tiresourcesCost.MultiplyCost(1f - num4));
			float num13 = tiresourcesCost3.resourceCosts.Min<ResourceValue>((ResourceValue x) => maximumDaysOfIncomeUsed * faction.GetDailyRevenue_AI(x.resource) / x.value);
			return Mathf.Min(num3, num13);
		}

		// Token: 0x0600302C RID: 12332 RVA: 0x0010679C File Offset: 0x0010499C
		public float GetResourceBasedMaximumFleetSize()
		{
			if (base.importance == 20 || base.assignedFleet == null)
			{
				return float.PositiveInfinity;
			}
			if (base.ExampleTrajectory == null)
			{
				return 0f;
			}
			if (base.ExampleTrajectory.DV_kps == 0.0)
			{
				return float.PositiveInfinity;
			}
			float num = (float)base.ExampleTrajectory.duration_d * 2f;
			if (this.target().isHabState && this.target().ref_hab.IsBase)
			{
				num += 30f;
			}
			int num2;
			if (base.assignedFleet.SpaceCombatValue() >= base.desiredFleetCombatValue)
			{
				num2 = ((float)base.assignedFleet.ships.Count * base.desiredFleetCombatValue / base.assignedFleet.SpaceCombatValue()).RoundUp();
			}
			else
			{
				num2 = base.assignedFleet.ships.Count + ((base.desiredFleetCombatValue - base.assignedFleet.SpaceCombatValue()) / this.faction.GetTypicalShipSpaceCombatValue()).RoundUp();
			}
			return FactionGoal_AttackWithFleet.GetResourceBasedMaximumFleetSize(this.faction, this.target(), base.FractionalImportance(0f), num, (float)base.ExampleTrajectory.DV_kps, base.assignedFleet.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState x) => x.template), num2, base.desiredFleetCombatValue);
		}

		// Token: 0x0600302D RID: 12333 RVA: 0x00106900 File Offset: 0x00104B00
		public static float GetDesiredBombardmentValue(TIFactionState bombardingFaction, TIGameState target, int failedAttackCount = 0)
		{
			if (target == null)
			{
				return -1f;
			}
			float num = (float)Mathf.Max(failedAttackCount - 1, 0);
			num = 1f + 0.25f * num * (num + 1f) / 2f;
			num = Mathf.Pow(num, 1.7f);
			if (target.isHabState)
			{
				return Mathf.Max(new float[]
				{
					20f,
					target.ref_hab.coreModule.AntiBombardmentArmor(false) * num,
					target.ref_hab.SpaceCombatValue() * num
				});
			}
			if (!target.hasEarthMapObject)
			{
				return -1f;
			}
			if (target.ref_region.antiSpaceDefenses)
			{
				if (!bombardingFaction.IsAlienFaction)
				{
					return 20f * target.ref_nation.militaryTechLevel;
				}
				return 40f * target.ref_nation.militaryTechLevel;
			}
			else
			{
				if (!bombardingFaction.IsAlienFaction)
				{
					return 10f * target.ref_nation.militaryTechLevel;
				}
				return 20f * target.ref_nation.militaryTechLevel;
			}
		}

		// Token: 0x0600302E RID: 12334 RVA: 0x00106A04 File Offset: 0x00104C04
		public float GetDesiredBombardmentValue()
		{
			if (!this.bombardmentGoal)
			{
				return -1f;
			}
			int num = 0;
			if (base.assignedFleet != null)
			{
				num = base.assignedFleet.GetFailedAttacksOnTargetCount(this.target());
			}
			return FactionGoal_AttackWithFleet.GetDesiredBombardmentValue(this.faction, this.target(), num);
		}

		// Token: 0x0600302F RID: 12335 RVA: 0x00106A53 File Offset: 0x00104C53
		public bool HasEnoughBombardmentValue(TISpaceFleetState fleet)
		{
			return !this.bombardmentGoal || fleet.BombardmentValue(this.target().ref_spaceBody) >= this.GetDesiredBombardmentValue();
		}

		// Token: 0x06003030 RID: 12336 RVA: 0x00106A7C File Offset: 0x00104C7C
		public override bool NeedsShipsOrdered()
		{
			if (base.NeedsShipsOrdered())
			{
				return true;
			}
			if (this.bombardmentGoal && this.target() != null)
			{
				TISpaceFleetState assignedFleet = base.assignedFleet;
				float? num = ((assignedFleet != null) ? new float?(assignedFleet.BombardmentValue(this.target().ref_spaceBody)) : null) + this.pendingFleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.BombardmentValue(this.target().ref_spaceBody)) + base.PendingShipTemplates().Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.BombardmentValue(this.target().ref_spaceBody));
				float desiredBombardmentValue = this.GetDesiredBombardmentValue();
				return (num.GetValueOrDefault() < desiredBombardmentValue) & (num != null);
			}
			return false;
		}

		// Token: 0x06003031 RID: 12337 RVA: 0x00106B71 File Offset: 0x00104D71
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x06003032 RID: 12338 RVA: 0x00106B78 File Offset: 0x00104D78
		public override bool ReadyForTransferToTarget(TISpaceFleetState fleet)
		{
			if (fleet == null || this.target() == null)
			{
				return false;
			}
			if (this.faction.IsAlienFaction && !AIEvaluators.ShouldAliensGoLoud())
			{
				return false;
			}
			if (fleet.BombardmentValue(this.target().ref_spaceBody) < this.GetDesiredBombardmentValue())
			{
				return false;
			}
			if (((fleet != null) ? fleet.SpaceCombatValue() : 0f) >= base.desiredFleetCombatValue)
			{
				return true;
			}
			if (!fleet.inTransfer && fleet.ref_system != null && fleet.ref_system != GameStateManager.Sol() && this.target() != null && this.target().isSpaceAssetState && (!this.target().isHabState || this.target().ref_hab.IsStation) && fleet.ref_system == this.target().ref_system && this.faction.IsAlienFaction)
			{
				float perceivedEnemySpaceAssetStrength_AndItsDefenders = this.faction.GetPerceivedEnemySpaceAssetStrength_AndItsDefenders(this.target().ref_spaceAsset);
				if (fleet.SpaceCombatValue() / perceivedEnemySpaceAssetStrength_AndItsDefenders > this.faction.GetMinimumSuperiorityForSpontaniousAttack())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04002257 RID: 8791
		public bool requiresWar;

		// Token: 0x04002259 RID: 8793
		private static readonly List<GoalType> incompatibleFleetGoals = new List<GoalType>
		{
			GoalType.DefendWithFleet,
			GoalType.CaptureHab,
			GoalType.TransportCouncilorsViaFleet
		};

		// Token: 0x0400225A RID: 8794
		private Dictionary<ShipRole, float> preferredRoles_spaceTarget = new Dictionary<ShipRole, float>
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
				ShipRole.LL_Intruder,
				1f
			},
			{
				ShipRole.LM_Interdictor,
				1f
			},
			{
				ShipRole.LS_Penetrator,
				1f
			},
			{
				ShipRole.LM_Protector,
				0.75f
			}
		};

		// Token: 0x0400225B RID: 8795
		private Dictionary<ShipRole, float> preferredRoles_bombardment = new Dictionary<ShipRole, float>
		{
			{
				ShipRole.LM_Interdictor,
				0.25f
			},
			{
				ShipRole.MM_SpaceSuperiority,
				0.25f
			},
			{
				ShipRole.LL_Bomber,
				5f
			},
			{
				ShipRole.LM_Protector,
				0.25f
			}
		};

		// Token: 0x0400225C RID: 8796
		public static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList)
		{
			typeof(BombardOperation_High),
			typeof(BombardOperation_Med),
			typeof(BombardOperation_Low),
			typeof(DestroyHabOperation)
		};
	}
}
