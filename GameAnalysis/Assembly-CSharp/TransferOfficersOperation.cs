using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000327 RID: 807
public class TransferOfficersOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000D2A RID: 3370 RVA: 0x0004244B File Offset: 0x0004064B
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x0004244E File Offset: 0x0004064E
	public override int SortOrder()
	{
		return 30;
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x00042452 File Offset: 0x00040652
	public override bool UpdatePropulsionOnComplete()
	{
		return true;
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x00042455 File Offset: 0x00040655
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00042458 File Offset: 0x00040658
	public static List<TIResourcesCost> ResourceCostOptions(Dictionary<TIOfficerState, OfficerCarrierState> plannedShipToShipTransfers)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		foreach (TIOfficerState tiofficerState in plannedShipToShipTransfers.Keys)
		{
			tiresourcesCost.SumCosts_NoDuration(tiofficerState.CostToTransfer(plannedShipToShipTransfers[tiofficerState]));
		}
		return new List<TIResourcesCost> { tiresourcesCost };
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x000424CC File Offset: 0x000406CC
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[] { TemplateManager.global.officerTransferCostPerRank.ToString("N0") });
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x0004251C File Offset: 0x0004071C
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.officers.Count) > 0 || (actorState.ref_fleet.dockedAtHab && actorState.ref_hab.faction == actorState.ref_fleet.faction && actorState.ref_hab.officersOnBoard.Count > 0);
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x0004259C File Offset: 0x0004079C
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x000425A4 File Offset: 0x000407A4
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (this.OpVisibleToActor(actorState, target) && base.ActorCanPerformOperation_PassInterruptCheck(actorState) && !ref_fleet.transferAssigned && !ref_fleet.inCombatOrWaitingForCombat && (!ref_fleet.landed || !ref_fleet.underBombardment))
		{
			foreach (TISpaceShipState tispaceShipState in ref_fleet.ships)
			{
				using (List<TIOfficerState>.Enumerator enumerator2 = tispaceShipState.officers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.AnyEligibleTransfers(true))
						{
							return true;
						}
					}
				}
			}
			if (!ref_fleet.dockedAtHab || !(ref_fleet.dockedLocation.ref_faction == ref_fleet.faction))
			{
				return false;
			}
			TIHabState ref_hab = ref_fleet.dockedLocation.ref_hab;
			if (ref_hab.officersOnBoard.Count <= 0)
			{
				return false;
			}
			if (ref_hab.officersOnBoard.Any<TIOfficerState>((TIOfficerState x) => x.AnyEligibleTransfers(true)))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x000426EC File Offset: 0x000408EC
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_SelfTransferOfficers);
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x000426F8 File Offset: 0x000408F8
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x00042706 File Offset: 0x00040906
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		actorState.ref_fleet.ExecuteOfficerTransferPlan();
	}

	// Token: 0x04000EB1 RID: 3761
	public Dictionary<TIOfficerState, OfficerCarrierState> plannedShipToShipTransfers;
}
