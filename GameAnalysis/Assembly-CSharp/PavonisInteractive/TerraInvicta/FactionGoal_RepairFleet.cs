using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000755 RID: 1877
	public class FactionGoal_RepairFleet : FactionGoal_FixUpFleet
	{
		// Token: 0x060030D7 RID: 12503 RVA: 0x00107FD2 File Offset: 0x001061D2
		public FactionGoal_RepairFleet()
		{
		}

		// Token: 0x060030D8 RID: 12504 RVA: 0x00107FDC File Offset: 0x001061DC
		public FactionGoal_RepairFleet(TIFactionState faction, TISpaceFleetState fleet, TIHabState habDestination = null)
		{
			this.faction = faction;
			base.SetImportance(5);
			if (habDestination == null)
			{
				List<TIHabState> list = fleet.faction.ShipConstructionHabs(false, false);
				habDestination = ((list != null) ? list.MinBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.MinDistanceBetweenTwoSpaceObjects_m(fleet, x.ref_spaceObject)) : null) ?? null;
			}
			base.destination = habDestination;
		}

		// Token: 0x060030D9 RID: 12505 RVA: 0x0010804B File Offset: 0x0010624B
		public static FactionGoal_RepairFleet CreateGoal(FactionGoal_RepairFleet p)
		{
			FactionGoal_RepairFleet factionGoal_RepairFleet = GameStateManager.CreateNewGameState<FactionGoal_RepairFleet>();
			factionGoal_RepairFleet.destination = p.destination;
			return factionGoal_RepairFleet;
		}

		// Token: 0x060030DA RID: 12506 RVA: 0x0010805E File Offset: 0x0010625E
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_RepairFleet>(base.ID, false);
		}

		// Token: 0x060030DB RID: 12507 RVA: 0x0010806D File Offset: 0x0010626D
		public override GoalType GetGoalType()
		{
			return GoalType.RepairFleet;
		}

		// Token: 0x060030DC RID: 12508 RVA: 0x00108071 File Offset: 0x00106271
		public override bool ValidNewGoal()
		{
			return base.destination != null;
		}

		// Token: 0x060030DD RID: 12509 RVA: 0x0010807F File Offset: 0x0010627F
		public override bool GoalFulfilled()
		{
			return base.assignedFleet != null && !base.assignedFleet.NeedsRepair();
		}

		// Token: 0x060030DE RID: 12510 RVA: 0x0010809F File Offset: 0x0010629F
		public override bool ShouldDiscardGoal()
		{
			return base.assignedFleet == null || base.destination == null || !base.destination.CanPartiallyRepairFleet(base.assignedFleet) || base.importance <= 0;
		}

		// Token: 0x060030DF RID: 12511 RVA: 0x001080DE File Offset: 0x001062DE
		public override bool InProgress()
		{
			return base.assignedFleet != null && base.destination != null && base.destination.CanPartiallyRepairFleet(base.assignedFleet);
		}

		// Token: 0x060030E0 RID: 12512 RVA: 0x0010810F File Offset: 0x0010630F
		public override bool NeedsShipsOrdered()
		{
			return false;
		}
	}
}
