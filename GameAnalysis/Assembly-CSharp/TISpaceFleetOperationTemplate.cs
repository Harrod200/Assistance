using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.GameTime;

// Token: 0x0200030A RID: 778
public abstract class TISpaceFleetOperationTemplate : TIOperationTemplate
{
	// Token: 0x06000C6D RID: 3181 RVA: 0x000409D9 File Offset: 0x0003EBD9
	public virtual bool ExecuteUponCancel()
	{
		return false;
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x000409DC File Offset: 0x0003EBDC
	public virtual bool CanCancel()
	{
		return false;
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x000409DF File Offset: 0x0003EBDF
	public virtual bool isAlien()
	{
		return false;
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x000409E2 File Offset: 0x0003EBE2
	public virtual bool UpdatePropulsionOnComplete()
	{
		return true;
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x000409E5 File Offset: 0x0003EBE5
	public virtual bool MustAcceptCombat()
	{
		return false;
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x000409E8 File Offset: 0x0003EBE8
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		if (actor.isSpaceFleetState)
		{
			TISpaceFleetState ref_fleet = actor.ref_fleet;
			ref_fleet.currentOperations.Add(new OperationData(this, target, TITimeState.Now(), opCompleteDate));
			ref_fleet.currentOperations.OrderBy<OperationData, TIDateTime>((OperationData s) => s.completionDate);
			string fleetOperationCompleteName = ref_fleet.fleetOperationCompleteName;
			TITimeEvent.CreateNewTimeEvent(opCompleteDate, ref_fleet, target, this, fleetOperationCompleteName, true, false, TITimeQueueRepeatType.None, 1, true, false);
			GameControl.eventManager.TriggerEvent(new StartFleetOperation(actor, this, target), null, (from x in new object[] { actor, target }.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			return true;
		}
		return false;
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x00040AB4 File Offset: 0x0003ECB4
	public virtual List<Type> BreakthroughOps()
	{
		return new List<Type>();
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x00040ABB File Offset: 0x0003ECBB
	public virtual bool CancelUponCombat()
	{
		return false;
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x00040ABE File Offset: 0x0003ECBE
	public virtual bool CancelUponDepartHab()
	{
		return false;
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x00040AC4 File Offset: 0x0003ECC4
	public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
		base.OnOperationCancel(actorState, target, opCompleteDate);
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (this.GetOperationTiming() != OperationTiming.InstantExecution && ref_fleet != null && !ref_fleet.deleted)
		{
			GameTimeManager.Singleton.CancelTimeEvent(ref_fleet.fleetOperationCompleteName, actorState, target, this, opCompleteDate);
		}
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00040B10 File Offset: 0x0003ED10
	public TIDateTime RescheduleFleetOperation(TISpaceFleetState fleet, OperationData operationData, float daysChange)
	{
		TIDateTime tidateTime = new TIDateTime(operationData.completionDate);
		tidateTime.AddDays(daysChange);
		int num = (int)(daysChange * 24f * 60f * 60f);
		if (tidateTime <= TITimeState.Now())
		{
			tidateTime = TITimeState.Now();
			tidateTime.AddSeconds(1.0);
			num++;
		}
		operationData.Reschedule(tidateTime);
		GameTimeManager.Singleton.ExtendTimeEvent(fleet.fleetOperationCompleteName, fleet, operationData.target, this, num, TITimeQueueRepeatType.Second);
		return tidateTime;
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x00040B90 File Offset: 0x0003ED90
	protected bool ActorCanPerformOperation_PassInterruptCheck(TIGameState actorState)
	{
		foreach (OperationData operationData in actorState.ref_fleet.CurrentOperations())
		{
			if (operationData.operation.IsBlockingOperation())
			{
				if (!(operationData.operation as TISpaceFleetOperationTemplate).BreakthroughOps().ConvertAll<IOperation>((Type x) => OperationsManager.operationsLookup[x]).Contains(this))
				{
					return false;
				}
			}
		}
		return true;
	}
}
