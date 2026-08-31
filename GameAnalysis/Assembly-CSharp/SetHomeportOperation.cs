using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000339 RID: 825
public class SetHomeportOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000DFC RID: 3580 RVA: 0x00046AD4 File Offset: 0x00044CD4
	public override bool IsBlockingOperation()
	{
		return false;
	}

	// Token: 0x06000DFD RID: 3581 RVA: 0x00046AD7 File Offset: 0x00044CD7
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x00046ADA File Offset: 0x00044CDA
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x00046ADD File Offset: 0x00044CDD
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return true;
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x00046AE0 File Offset: 0x00044CE0
	public override int SortOrder()
	{
		return 25;
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x00046AE4 File Offset: 0x00044CE4
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return true;
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x00046AE7 File Offset: 0x00044CE7
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00046AF0 File Offset: 0x00044CF0
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = actorState.ref_faction.habs.ConvertAll<TIGameState>((TIHabState x) => x.ref_gameState);
		if (actorState.ref_fleet.dockedAtHab && actorState.ref_fleet.dockedLocation.ref_hab.ref_factions.Contains(actorState.ref_faction))
		{
			int num = list.IndexOf(actorState.ref_fleet.ref_hab);
			if (num > -1)
			{
				list.RemoveAt(num);
				list.Insert(0, actorState.ref_fleet.ref_hab);
			}
		}
		else if (actorState.ref_fleet.ref_naturalSpaceObject != null)
		{
			TIGameState tigameState = list.FirstOrDefault<TIGameState>((TIGameState x) => x.ref_naturalSpaceObject == actorState.ref_fleet.ref_naturalSpaceObject);
			if (tigameState != null)
			{
				int num2 = list.IndexOf(tigameState);
				if (num2 > -1)
				{
					list.RemoveAt(num2);
					list.Insert(0, tigameState);
				}
			}
		}
		else if (actorState.ref_fleet.inTransfer)
		{
			TIGameState tigameState2 = list.FirstOrDefault<TIGameState>((TIGameState x) => x.ref_orbit == actorState.ref_fleet.trajectory.originOrbit);
			if (tigameState2 == null)
			{
				tigameState2 = list.FirstOrDefault<TIGameState>((TIGameState x) => x == actorState.ref_fleet.trajectory.destinationStation);
				if (tigameState2 == null)
				{
					tigameState2 = list.FirstOrDefault<TIGameState>((TIGameState x) => x.ref_orbit == actorState.ref_fleet.trajectory.destinationOrbit);
				}
			}
			if (tigameState2 != null)
			{
				int num3 = list.IndexOf(tigameState2);
				if (num3 > -1)
				{
					list.RemoveAt(num3);
					list.Insert(0, tigameState2);
				}
			}
		}
		return list;
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x00046CA8 File Offset: 0x00044EA8
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Hab);
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x00046CB4 File Offset: 0x00044EB4
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (target == null)
		{
			actorState.ref_fleet.SetHomePort(null);
			return;
		}
		actorState.ref_fleet.SetHomePort(target.ref_hab);
	}
}
