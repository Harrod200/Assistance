using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000753 RID: 1875
	public abstract class FactionGoal_FixUpFleet : FactionGoal_Fleet
	{
		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x060030BA RID: 12474 RVA: 0x00107D07 File Offset: 0x00105F07
		// (set) Token: 0x060030BB RID: 12475 RVA: 0x00107D0F File Offset: 0x00105F0F
		public TIHabState destination { get; protected set; }

		// Token: 0x060030BC RID: 12476 RVA: 0x00107D18 File Offset: 0x00105F18
		public override TIGameState actor()
		{
			return base.assignedFleet;
		}

		// Token: 0x060030BD RID: 12477 RVA: 0x00107D20 File Offset: 0x00105F20
		public override TIGameState target()
		{
			return this.destination;
		}

		// Token: 0x060030BE RID: 12478 RVA: 0x00107D28 File Offset: 0x00105F28
		public override TIGameState location()
		{
			return this.destination;
		}

		// Token: 0x060030BF RID: 12479 RVA: 0x00107D30 File Offset: 0x00105F30
		public override TIGameState goalProduct()
		{
			return base.assignedFleet;
		}

		// Token: 0x060030C0 RID: 12480 RVA: 0x00107D38 File Offset: 0x00105F38
		public override bool RequiresFleet()
		{
			return true;
		}

		// Token: 0x060030C1 RID: 12481 RVA: 0x00107D3B File Offset: 0x00105F3B
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x060030C2 RID: 12482 RVA: 0x00107D3E File Offset: 0x00105F3E
		public override float ComputeDesiredFleetCombatValue()
		{
			return 0f;
		}

		// Token: 0x060030C3 RID: 12483 RVA: 0x00107D45 File Offset: 0x00105F45
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x060030C4 RID: 12484 RVA: 0x00107D4C File Offset: 0x00105F4C
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.NoRole;
		}

		// Token: 0x060030C5 RID: 12485 RVA: 0x00107D50 File Offset: 0x00105F50
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return Enums.ShipRoles.ToDictionary<ShipRole, ShipRole, float>((ShipRole x) => x, (ShipRole x) => 1f);
		}

		// Token: 0x060030C6 RID: 12486 RVA: 0x00107DA8 File Offset: 0x00105FA8
		public override bool IsDuplicate(TIFactionGoalState testGoal, TIGameState testTarget)
		{
			FactionGoal_FixUpFleet factionGoal_FixUpFleet = testGoal as FactionGoal_FixUpFleet;
			return factionGoal_FixUpFleet != null && base.assignedFleet != null && base.assignedFleet == factionGoal_FixUpFleet.assignedFleet;
		}

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x060030C7 RID: 12487 RVA: 0x00107DE2 File Offset: 0x00105FE2
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_FixUpFleet.incompatibleFleetGoals;
			}
		}

		// Token: 0x060030C8 RID: 12488 RVA: 0x00107DE9 File Offset: 0x00105FE9
		public override void ChangeTarget(TIGameState newTarget)
		{
			this.destination = ((newTarget != null) ? newTarget.ref_hab : null);
		}

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x060030C9 RID: 12489 RVA: 0x00107DFD File Offset: 0x00105FFD
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_Fleet.coreFleetOpsList;
			}
		}

		// Token: 0x060030CA RID: 12490 RVA: 0x00107E04 File Offset: 0x00106004
		public override bool NeedsShipsOrdered()
		{
			return false;
		}

		// Token: 0x0400226E RID: 8814
		private static readonly List<GoalType> incompatibleFleetGoals = new List<GoalType>
		{
			GoalType.AttackWithFleet,
			GoalType.CaptureHab
		};
	}
}
