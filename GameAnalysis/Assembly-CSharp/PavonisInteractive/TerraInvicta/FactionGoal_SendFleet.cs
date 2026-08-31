using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000752 RID: 1874
	public class FactionGoal_SendFleet : FactionGoal_Fleet
	{
		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x0600309D RID: 12445 RVA: 0x00107B22 File Offset: 0x00105D22
		// (set) Token: 0x0600309E RID: 12446 RVA: 0x00107B2A File Offset: 0x00105D2A
		public TIOrbitState destination { get; protected set; }

		// Token: 0x0600309F RID: 12447 RVA: 0x00107B33 File Offset: 0x00105D33
		public FactionGoal_SendFleet(TIFactionState faction, TIOrbitState destination)
		{
			this.faction = faction;
			this.destination = destination;
			base.SetImportance(5);
		}

		// Token: 0x060030A0 RID: 12448 RVA: 0x00107B50 File Offset: 0x00105D50
		public FactionGoal_SendFleet()
		{
		}

		// Token: 0x060030A1 RID: 12449 RVA: 0x00107B58 File Offset: 0x00105D58
		public static FactionGoal_SendFleet CreateGoal(FactionGoal_SendFleet p)
		{
			FactionGoal_SendFleet factionGoal_SendFleet = GameStateManager.CreateNewGameState<FactionGoal_SendFleet>();
			factionGoal_SendFleet.destination = p.destination;
			return factionGoal_SendFleet;
		}

		// Token: 0x060030A2 RID: 12450 RVA: 0x00107B6B File Offset: 0x00105D6B
		public override TIGameState actor()
		{
			return base.assignedFleet;
		}

		// Token: 0x060030A3 RID: 12451 RVA: 0x00107B73 File Offset: 0x00105D73
		public override TIGameState target()
		{
			return this.destination;
		}

		// Token: 0x060030A4 RID: 12452 RVA: 0x00107B7B File Offset: 0x00105D7B
		public override TIGameState location()
		{
			return this.destination;
		}

		// Token: 0x060030A5 RID: 12453 RVA: 0x00107B83 File Offset: 0x00105D83
		public override TIGameState goalProduct()
		{
			return base.assignedFleet;
		}

		// Token: 0x060030A6 RID: 12454 RVA: 0x00107B8B File Offset: 0x00105D8B
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x060030A7 RID: 12455 RVA: 0x00107B8E File Offset: 0x00105D8E
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x060030A8 RID: 12456 RVA: 0x00107B91 File Offset: 0x00105D91
		public override float ComputeDesiredFleetCombatValue()
		{
			return 0f;
		}

		// Token: 0x060030A9 RID: 12457 RVA: 0x00107B98 File Offset: 0x00105D98
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x060030AA RID: 12458 RVA: 0x00107B9F File Offset: 0x00105D9F
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.NoRole;
		}

		// Token: 0x060030AB RID: 12459 RVA: 0x00107BA4 File Offset: 0x00105DA4
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return Enums.ShipRoles.ToDictionary<ShipRole, ShipRole, float>((ShipRole x) => x, (ShipRole x) => 1f);
		}

		// Token: 0x060030AC RID: 12460 RVA: 0x00107BFC File Offset: 0x00105DFC
		public override bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget)
		{
			FactionGoal_SendFleet factionGoal_SendFleet = testGoal as FactionGoal_SendFleet;
			return factionGoal_SendFleet != null && base.assignedFleet != null && base.assignedFleet == factionGoal_SendFleet.assignedFleet;
		}

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x060030AD RID: 12461 RVA: 0x00107C36 File Offset: 0x00105E36
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_SendFleet.incompatibleFleetGoals;
			}
		}

		// Token: 0x060030AE RID: 12462 RVA: 0x00107C3D File Offset: 0x00105E3D
		public override void ChangeTarget(TIGameState newTarget)
		{
			this.destination = ((newTarget != null) ? newTarget.ref_orbit : null);
		}

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x060030AF RID: 12463 RVA: 0x00107C51 File Offset: 0x00105E51
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_Fleet.coreFleetOpsList;
			}
		}

		// Token: 0x060030B0 RID: 12464 RVA: 0x00107C58 File Offset: 0x00105E58
		public override bool NeedsShipsOrdered()
		{
			return false;
		}

		// Token: 0x060030B1 RID: 12465 RVA: 0x00107C5B File Offset: 0x00105E5B
		public override GoalType GetGoalType()
		{
			return GoalType.SendFleet;
		}

		// Token: 0x060030B2 RID: 12466 RVA: 0x00107C5F File Offset: 0x00105E5F
		public override bool ValidNewGoal()
		{
			return TIGameState.Valid(this.destination);
		}

		// Token: 0x060030B3 RID: 12467 RVA: 0x00107C6C File Offset: 0x00105E6C
		public override bool InProgress()
		{
			return TIGameState.Valid(base.assignedFleet) && TIGameState.Valid(this.destination);
		}

		// Token: 0x060030B4 RID: 12468 RVA: 0x00107C88 File Offset: 0x00105E88
		public override bool GoalFulfilled()
		{
			TISpaceFleetState assignedFleet = base.assignedFleet;
			return ((assignedFleet != null) ? assignedFleet.ref_orbit : null) == this.destination;
		}

		// Token: 0x060030B5 RID: 12469 RVA: 0x00107CA7 File Offset: 0x00105EA7
		public override bool ShouldDiscardGoal()
		{
			return !TIGameState.Valid(base.assignedFleet) || !TIGameState.Valid(this.destination) || base.importance <= 0;
		}

		// Token: 0x060030B6 RID: 12470 RVA: 0x00107CD1 File Offset: 0x00105ED1
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_SendFleet>(base.ID, false);
		}

		// Token: 0x060030B7 RID: 12471 RVA: 0x00107CE0 File Offset: 0x00105EE0
		public override void OnGoalDiscarded()
		{
			base.OnGoalDiscarded();
		}

		// Token: 0x060030B8 RID: 12472 RVA: 0x00107CE8 File Offset: 0x00105EE8
		public override bool LeaveMyFleetAlone()
		{
			return true;
		}

		// Token: 0x0400226C RID: 8812
		private static readonly List<GoalType> incompatibleFleetGoals = new List<GoalType>
		{
			GoalType.AttackWithFleet,
			GoalType.CaptureHab
		};
	}
}
