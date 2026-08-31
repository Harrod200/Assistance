using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000754 RID: 1876
	public class FactionGoal_ResupplyFleet : FactionGoal_FixUpFleet
	{
		// Token: 0x060030CD RID: 12493 RVA: 0x00107E2B File Offset: 0x0010602B
		public FactionGoal_ResupplyFleet()
		{
		}

		// Token: 0x060030CE RID: 12494 RVA: 0x00107E34 File Offset: 0x00106034
		public FactionGoal_ResupplyFleet(TIFactionState faction, TISpaceFleetState fleet, TIHabState habDestination = null)
		{
			this.faction = faction;
			base.SetImportance(5);
			if (habDestination == null)
			{
				IEnumerable<TIHabState> enumerable = from x in fleet.faction.ResupplyHabs(false, false)
					where x.IsSafeToVisit(fleet)
					select x;
				habDestination = ((enumerable != null) ? enumerable.MinBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.MinDistanceBetweenTwoSpaceObjects_m(fleet, x.ref_spaceObject)) : null) ?? null;
			}
			base.resupplyHab = (base.destination = habDestination);
		}

		// Token: 0x060030CF RID: 12495 RVA: 0x00107EBD File Offset: 0x001060BD
		public static FactionGoal_ResupplyFleet CreateGoal(FactionGoal_ResupplyFleet p)
		{
			FactionGoal_ResupplyFleet factionGoal_ResupplyFleet = GameStateManager.CreateNewGameState<FactionGoal_ResupplyFleet>();
			factionGoal_ResupplyFleet.destination = p.destination;
			return factionGoal_ResupplyFleet;
		}

		// Token: 0x060030D0 RID: 12496 RVA: 0x00107ED0 File Offset: 0x001060D0
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_ResupplyFleet>(base.ID, false);
		}

		// Token: 0x060030D1 RID: 12497 RVA: 0x00107EDF File Offset: 0x001060DF
		public override GoalType GetGoalType()
		{
			return GoalType.ResupplyFleet;
		}

		// Token: 0x060030D2 RID: 12498 RVA: 0x00107EE3 File Offset: 0x001060E3
		public override bool ValidNewGoal()
		{
			return base.destination != null;
		}

		// Token: 0x060030D3 RID: 12499 RVA: 0x00107EF1 File Offset: 0x001060F1
		public override bool GoalFulfilled()
		{
			return base.assignedFleet != null && !base.assignedFleet.deleted && !base.assignedFleet.NeedsRearm() && !base.assignedFleet.AI_NeedsRefuel();
		}

		// Token: 0x060030D4 RID: 12500 RVA: 0x00107F2C File Offset: 0x0010612C
		public override bool ShouldDiscardGoal()
		{
			return base.assignedFleet == null || base.destination == null || base.assignedFleet.deleted || base.destination.deleted || !base.destination.AllowsResupply(base.assignedFleet.faction, true, false) || base.importance <= 0;
		}

		// Token: 0x060030D5 RID: 12501 RVA: 0x00107F97 File Offset: 0x00106197
		public override bool InProgress()
		{
			return base.assignedFleet != null && base.destination != null && base.destination.AllowsResupply(base.assignedFleet.faction, true, false);
		}

		// Token: 0x060030D6 RID: 12502 RVA: 0x00107FCF File Offset: 0x001061CF
		public override bool NeedsShipsOrdered()
		{
			return false;
		}
	}
}
