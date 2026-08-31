using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000741 RID: 1857
	public class FactionGoal_FoundSurveillanceStation : FactionGoal_FoundStation
	{
		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x06002F4E RID: 12110 RVA: 0x00102FF5 File Offset: 0x001011F5
		// (set) Token: 0x06002F4F RID: 12111 RVA: 0x00102FFD File Offset: 0x001011FD
		public int tier { get; protected set; }

		// Token: 0x06002F50 RID: 12112 RVA: 0x00103006 File Offset: 0x00101206
		public FactionGoal_FoundSurveillanceStation()
		{
		}

		// Token: 0x06002F51 RID: 12113 RVA: 0x00103010 File Offset: 0x00101210
		public FactionGoal_FoundSurveillanceStation(TIFactionState faction, int importance, TIOrbitState orbit, GoalType defendGoal, int tier)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.orbit = orbit;
			this.subsequentGoals = new List<GoalType>();
			if (defendGoal == GoalType.DefendWithFleet)
			{
				this.subsequentGoals.Add(defendGoal);
			}
			this.tier = tier;
			this.requiredModuleNames = new List<string> { TemplateManager.IterateByClass<TIHabModuleTemplate>(true).First<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.SpecialRules.Contains(HabModuleSpecialRule.AlienSurveillance) && x.specialRulesValue == (float)tier).dataName };
			base.setAsPrimaryHab = false;
		}

		// Token: 0x06002F52 RID: 12114 RVA: 0x001030A4 File Offset: 0x001012A4
		public override bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget)
		{
			if (!base.IsDuplicate(testGoal, testTarget))
			{
				return false;
			}
			FactionGoal_FoundSurveillanceStation factionGoal_FoundSurveillanceStation = testGoal as FactionGoal_FoundSurveillanceStation;
			int? num = ((factionGoal_FoundSurveillanceStation != null) ? new int?(factionGoal_FoundSurveillanceStation.tier) : null);
			int tier = this.tier;
			return (num.GetValueOrDefault() == tier) & (num != null);
		}

		// Token: 0x06002F53 RID: 12115 RVA: 0x001030F6 File Offset: 0x001012F6
		public override bool ShouldDiscardGoal()
		{
			return base.ShouldDiscardGoal();
		}

		// Token: 0x06002F54 RID: 12116 RVA: 0x00103103 File Offset: 0x00101303
		public static FactionGoal_FoundSurveillanceStation CreateGoal(FactionGoal_FoundSurveillanceStation p)
		{
			FactionGoal_FoundSurveillanceStation factionGoal_FoundSurveillanceStation = GameStateManager.CreateNewGameState<FactionGoal_FoundSurveillanceStation>();
			factionGoal_FoundSurveillanceStation.orbit = p.orbit;
			factionGoal_FoundSurveillanceStation.tier = p.tier;
			factionGoal_FoundSurveillanceStation.subsequentGoals = p.subsequentGoals;
			factionGoal_FoundSurveillanceStation.requiredModuleNames = new List<string>(p.requiredModuleNames);
			return factionGoal_FoundSurveillanceStation;
		}

		// Token: 0x06002F55 RID: 12117 RVA: 0x00103140 File Offset: 0x00101340
		public static List<TIOrbitState> candidateOrbits(int tier)
		{
			List<TIOrbitState> list = GameStateManager.Earth().OrbitsInSystem.ToList<TIOrbitState>();
			list.AddRange(from x in GameStateManager.Earth().lagrangePoints.Where<TILagrangePointState>((TILagrangePointState x) => x.template.lagrangeValue != LagrangeValue.L2).SelectMany<TILagrangePointState, TIOrbitState>((TILagrangePointState x) => x.orbits)
				where x.NewStationAllowed(tier, null)
				select x);
			return list;
		}

		// Token: 0x06002F56 RID: 12118 RVA: 0x001031D2 File Offset: 0x001013D2
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_FoundSurveillanceStation>(base.ID, false);
		}

		// Token: 0x06002F57 RID: 12119 RVA: 0x001031E1 File Offset: 0x001013E1
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x06002F58 RID: 12120 RVA: 0x001031E4 File Offset: 0x001013E4
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_FoundSurveillanceStation.fleetOps;
			}
		}

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x06002F59 RID: 12121 RVA: 0x001031EB File Offset: 0x001013EB
		public override List<Type> spaceOperations
		{
			get
			{
				return FactionGoal_FoundSurveillanceStation.spaceOps;
			}
		}

		// Token: 0x06002F5A RID: 12122 RVA: 0x001031F2 File Offset: 0x001013F2
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.InnerSystemColonyShip;
		}

		// Token: 0x06002F5B RID: 12123 RVA: 0x001031F5 File Offset: 0x001013F5
		public override GoalType GetGoalType()
		{
			return GoalType.FoundSurveillanceStation;
		}

		// Token: 0x06002F5C RID: 12124 RVA: 0x001031FC File Offset: 0x001013FC
		public override float ComputeDesiredFleetCombatValue()
		{
			if (this.ShouldPerformMissionMinimallyArmed)
			{
				return 0f;
			}
			TISpaceFleetState assignedFleet = base.assignedFleet;
			float num;
			if (assignedFleet == null)
			{
				num = 1f;
			}
			else
			{
				num = assignedFleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.role == this.GetPrimaryShipRole()).Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f));
			}
			return num + 1f + FactionGoal_Fleet.ComputeBaselineFleetCombatValue(this.faction, this.target()) * (0.25f + (0.25f + (float)this.tier));
		}

		// Token: 0x06002F5D RID: 12125 RVA: 0x0010328E File Offset: 0x0010148E
		public override IEnumerable<TIOrbitState> GetAlternativeOrbits()
		{
			return FactionGoal_FoundSurveillanceStation.candidateOrbits(this.tier);
		}

		// Token: 0x06002F5E RID: 12126 RVA: 0x0010329B File Offset: 0x0010149B
		public override bool ReadyForTransferToTarget(TISpaceFleetState fleet)
		{
			return (!this.faction.IsAlienFaction || AIEvaluators.GetAlienQuietness() <= 0.225f) && TIGlobalConfig.globalConfig.AI_AliensMaySurveil() && base.ReadyForTransferToTarget(fleet);
		}

		// Token: 0x0400223E RID: 8766
		private static readonly List<Type> spaceOps = new List<Type>
		{
			typeof(FoundAlienSurveillancePlatform),
			typeof(FoundAlienSurveillanceOrbital),
			typeof(FoundAlienSurveillanceRing)
		};

		// Token: 0x0400223F RID: 8767
		public new static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList)
		{
			typeof(FoundAlienSurveillancePlatform),
			typeof(FoundAlienSurveillanceOrbital),
			typeof(FoundAlienSurveillanceRing)
		};
	}
}
