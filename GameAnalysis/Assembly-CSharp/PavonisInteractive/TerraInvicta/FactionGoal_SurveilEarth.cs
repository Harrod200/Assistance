using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000759 RID: 1881
	public class FactionGoal_SurveilEarth : FactionGoal_Fleet
	{
		// Token: 0x06003115 RID: 12565 RVA: 0x00108CB0 File Offset: 0x00106EB0
		public FactionGoal_SurveilEarth()
		{
		}

		// Token: 0x06003116 RID: 12566 RVA: 0x00108CB8 File Offset: 0x00106EB8
		public FactionGoal_SurveilEarth(TIFactionState faction, int importance)
		{
			this.faction = faction;
			base.SetImportance(importance);
		}

		// Token: 0x06003117 RID: 12567 RVA: 0x00108CCE File Offset: 0x00106ECE
		public static FactionGoal_SurveilEarth CreateGoal(FactionGoal_SurveilEarth p)
		{
			return GameStateManager.CreateNewGameState<FactionGoal_SurveilEarth>();
		}

		// Token: 0x06003118 RID: 12568 RVA: 0x00108CD5 File Offset: 0x00106ED5
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_SurveilEarth>(base.ID, false);
		}

		// Token: 0x06003119 RID: 12569 RVA: 0x00108CE4 File Offset: 0x00106EE4
		public override GoalType GetGoalType()
		{
			return GoalType.SurveilEarth;
		}

		// Token: 0x0600311A RID: 12570 RVA: 0x00108CE8 File Offset: 0x00106EE8
		public override TIGameState actor()
		{
			return base.assignedFleet;
		}

		// Token: 0x0600311B RID: 12571 RVA: 0x00108CF0 File Offset: 0x00106EF0
		public override TIGameState target()
		{
			return GameStateManager.LEOStates()[0];
		}

		// Token: 0x0600311C RID: 12572 RVA: 0x00108CFD File Offset: 0x00106EFD
		public override TIGameState location()
		{
			return this.target();
		}

		// Token: 0x0600311D RID: 12573 RVA: 0x00108D05 File Offset: 0x00106F05
		public override TIGameState goalProduct()
		{
			return base.assignedFleet;
		}

		// Token: 0x0600311E RID: 12574 RVA: 0x00108D0D File Offset: 0x00106F0D
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x0600311F RID: 12575 RVA: 0x00108D10 File Offset: 0x00106F10
		public override bool ValidNewGoal()
		{
			return true;
		}

		// Token: 0x06003120 RID: 12576 RVA: 0x00108D13 File Offset: 0x00106F13
		public override bool InProgress()
		{
			return base.assignedFleet != null;
		}

		// Token: 0x06003121 RID: 12577 RVA: 0x00108D21 File Offset: 0x00106F21
		public override bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget)
		{
			return base.assignedFleet != null && testGoal.ref_fleetGoal.assignedFleet == base.assignedFleet;
		}

		// Token: 0x06003122 RID: 12578 RVA: 0x00108D49 File Offset: 0x00106F49
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0;
		}

		// Token: 0x06003123 RID: 12579 RVA: 0x00108D57 File Offset: 0x00106F57
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x06003124 RID: 12580 RVA: 0x00108D5A File Offset: 0x00106F5A
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_SurveilEarth.fleetOps;
			}
		}

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06003125 RID: 12581 RVA: 0x00108D61 File Offset: 0x00106F61
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003126 RID: 12582 RVA: 0x00108D64 File Offset: 0x00106F64
		public override void ChangeTarget(TIGameState newTarget)
		{
		}

		// Token: 0x06003127 RID: 12583 RVA: 0x00108D66 File Offset: 0x00106F66
		public override float ComputeDesiredFleetCombatValue()
		{
			if (this.ShouldPerformMissionMinimallyArmed)
			{
				return 0f;
			}
			return base.ComputeDesiredFleetCombatValue() / 2f;
		}

		// Token: 0x06003128 RID: 12584 RVA: 0x00108D82 File Offset: 0x00106F82
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x06003129 RID: 12585 RVA: 0x00108D8C File Offset: 0x00106F8C
		public override bool ReadyForTransferToTarget(TISpaceFleetState fleet)
		{
			return (!this.faction.IsAlienFaction || AIEvaluators.GetAlienQuietness() <= 0.5f) && TIGlobalConfig.globalConfig.AI_AliensMaySurveil() && (fleet != null && fleet.SpaceCombatValue() >= base.desiredFleetCombatValue) && fleet.CanFulfillGoal(this, false);
		}

		// Token: 0x0600312A RID: 12586 RVA: 0x00108DE3 File Offset: 0x00106FE3
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.EarthSurveillance;
		}

		// Token: 0x0600312B RID: 12587 RVA: 0x00108DE6 File Offset: 0x00106FE6
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return FactionGoal_SurveilEarth.preferredShipRoles;
		}

		// Token: 0x0600312C RID: 12588 RVA: 0x00108DED File Offset: 0x00106FED
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x04002273 RID: 8819
		public static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList) { typeof(AlienEarthSurveillanceOperation) };

		// Token: 0x04002274 RID: 8820
		private static readonly Dictionary<ShipRole, float> preferredShipRoles = new Dictionary<ShipRole, float>
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
