using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000334 RID: 820
public abstract class FixUpFleetOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000DB9 RID: 3513 RVA: 0x00043F3B File Offset: 0x0004213B
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x00043F3E File Offset: 0x0004213E
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x00043F41 File Offset: 0x00042141
	public override bool UseResourceCostDuration()
	{
		return true;
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x00043F44 File Offset: 0x00042144
	public override bool CanCancel()
	{
		return true;
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x00043F47 File Offset: 0x00042147
	public override bool CancelUponDepartHab()
	{
		return true;
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x00043F4A File Offset: 0x0004214A
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x00043F51 File Offset: 0x00042151
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00043F5D File Offset: 0x0004215D
	public override bool HasResourceCost()
	{
		return true;
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x00043F60 File Offset: 0x00042160
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x00043F6E File Offset: 0x0004216E
	public override List<Type> BreakthroughOps()
	{
		return new List<Type>
		{
			typeof(CancelFleetOperation),
			typeof(MergeFleetOperation)
		};
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x00043F98 File Offset: 0x00042198
	protected void HandlePartialCompletion(TISpaceFleetState fleet, TIDateTime opCompleteDate)
	{
		OperationData operationData = fleet.CurrentOperations().FirstOrDefault<OperationData>((OperationData x) => x.operation == this);
		TIDateTime tidateTime;
		if ((tidateTime = ((operationData != null) ? operationData.startDate : null) ?? null) == null)
		{
			IEnumerable<TISpaceShipState> enumerable = fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.plannedResupplyAndRepair.active);
			if (enumerable == null)
			{
				tidateTime = null;
			}
			else
			{
				tidateTime = enumerable.Max<TISpaceShipState, TIDateTime>((TISpaceShipState x) => x.plannedResupplyAndRepair.startDate);
			}
		}
		TIDateTime tidateTime2 = tidateTime;
		float num3;
		if (tidateTime2 != null)
		{
			double num;
			if (opCompleteDate == null)
			{
				num = (double)fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.plannedResupplyAndRepair.active).Sum<TISpaceShipState>((TISpaceShipState x) => x.plannedResupplyAndRepair.duration_days);
			}
			else
			{
				num = opCompleteDate.DifferenceInDays(tidateTime2);
			}
			double num2 = num;
			if (num2 > 0.0)
			{
				num3 = Mathf.Clamp((float)(TITimeState.Now().DifferenceInDays(tidateTime2) / num2), 0f, 1f);
			}
			else
			{
				num3 = 1f;
			}
		}
		else
		{
			num3 = 1f;
		}
		fleet.TruncateResupplyAndRepair(num3);
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x000440D0 File Offset: 0x000422D0
	protected void CleanUpFleetRepairData(TISpaceFleetState fleet)
	{
		foreach (TISpaceShipState tispaceShipState in fleet.ships)
		{
			tispaceShipState.plannedResupplyAndRepair.ClearAllResupplyAndRepair();
		}
	}
}
