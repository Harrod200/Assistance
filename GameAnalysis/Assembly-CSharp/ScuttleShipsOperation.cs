using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200033E RID: 830
public class ScuttleShipsOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000E38 RID: 3640 RVA: 0x00047B1A File Offset: 0x00045D1A
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x00047B1D File Offset: 0x00045D1D
	public override int SortOrder()
	{
		return 24;
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x00047B21 File Offset: 0x00045D21
	public override bool UpdatePropulsionOnComplete()
	{
		return true;
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x00047B24 File Offset: 0x00045D24
	public override bool IsBlockingOperation()
	{
		return false;
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x00047B27 File Offset: 0x00045D27
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return true;
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x00047B2A File Offset: 0x00045D2A
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000E3E RID: 3646 RVA: 0x00047B34 File Offset: 0x00045D34
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		if (actorState.ref_fleet.currentOperations.Count == 0 && !actorState.ref_fleet.transferAssigned && !actorState.ref_fleet.inCombatOrWaitingForCombat && !actorState.ref_fleet.underBombardment)
		{
			TIHabState ref_hab = actorState.ref_fleet.ref_hab;
			if (ref_hab == null || !ref_hab.underBombardment)
			{
				return actorState.ref_fleet.councilorPassengers.Count<TICouncilorState>((TICouncilorState x) => x.faction == actorState.ref_faction) == 0 || actorState.ref_fleet.dockedAtHab;
			}
		}
		return false;
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00047BF6 File Offset: 0x00045DF6
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Ships);
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x00047C02 File Offset: 0x00045E02
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x00047C04 File Offset: 0x00045E04
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		return target != null && target.ref_fleet.ships.Count > 0;
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x00047C24 File Offset: 0x00045E24
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x00047C32 File Offset: 0x00045E32
	public static void ScuttleShipsFromFleet(TISpaceFleetState fleet, List<TISpaceShipState> shipsToScuttle)
	{
		fleet.ScuttleShips(shipsToScuttle);
	}
}
