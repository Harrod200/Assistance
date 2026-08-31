using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000756 RID: 1878
	public class FactionGoal_RefitFleet : FactionGoal_FixUpFleet
	{
		// Token: 0x060030E1 RID: 12513 RVA: 0x00108112 File Offset: 0x00106312
		public FactionGoal_RefitFleet()
		{
		}

		// Token: 0x060030E2 RID: 12514 RVA: 0x0010811C File Offset: 0x0010631C
		public FactionGoal_RefitFleet(TIFactionState faction, TISpaceFleetState fleet, TIHabState habDestination = null)
		{
			this.faction = faction;
			int num = Mathf.Clamp((3f * fleet.RelativeValueOfRefittedFleet).Round(), 1, 12);
			base.SetImportance(num);
			if (habDestination == null)
			{
				List<TIHabState> list = fleet.faction.ShipConstructionHabs(false, false);
				habDestination = ((list != null) ? list.MinBy<TIHabState, double>((TIHabState x) => TISpaceObjectState.MinDistanceBetweenTwoSpaceObjects_m(fleet, x.ref_spaceObject)) : null) ?? null;
			}
			base.destination = habDestination;
		}

		// Token: 0x060030E3 RID: 12515 RVA: 0x001081AA File Offset: 0x001063AA
		public static FactionGoal_RefitFleet CreateGoal(FactionGoal_RefitFleet p)
		{
			FactionGoal_RefitFleet factionGoal_RefitFleet = GameStateManager.CreateNewGameState<FactionGoal_RefitFleet>();
			factionGoal_RefitFleet.destination = p.destination;
			return factionGoal_RefitFleet;
		}

		// Token: 0x060030E4 RID: 12516 RVA: 0x001081BD File Offset: 0x001063BD
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_RefitFleet>(base.ID, false);
		}

		// Token: 0x060030E5 RID: 12517 RVA: 0x001081CC File Offset: 0x001063CC
		public override GoalType GetGoalType()
		{
			return GoalType.RefitFleet;
		}

		// Token: 0x060030E6 RID: 12518 RVA: 0x001081D0 File Offset: 0x001063D0
		public override bool ValidNewGoal()
		{
			return base.destination != null;
		}

		// Token: 0x060030E7 RID: 12519 RVA: 0x001081DE File Offset: 0x001063DE
		public override bool GoalFulfilled()
		{
			return base.assignedFleet != null && !base.assignedFleet.deleted && !base.assignedFleet.NeedsRefit();
		}

		// Token: 0x060030E8 RID: 12520 RVA: 0x0010820C File Offset: 0x0010640C
		public override bool ShouldDiscardGoal()
		{
			return base.assignedFleet == null || base.destination == null || base.assignedFleet.deleted || base.destination.deleted || !base.destination.AllowsShipConstruction(base.assignedFleet.faction, false, false) || base.importance <= 0;
		}

		// Token: 0x060030E9 RID: 12521 RVA: 0x00108278 File Offset: 0x00106478
		public override bool InProgress()
		{
			return base.assignedFleet != null && !base.assignedFleet.deleted && base.destination != null && base.destination.AllowsShipConstruction(base.assignedFleet.faction, false, false);
		}

		// Token: 0x060030EA RID: 12522 RVA: 0x001082C8 File Offset: 0x001064C8
		public override bool NeedsShipsOrdered()
		{
			return false;
		}
	}
}
