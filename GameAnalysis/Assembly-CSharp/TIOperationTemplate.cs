using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002FA RID: 762
public abstract class TIOperationTemplate : TIDataTemplate, IOperation
{
	// Token: 0x06000BA5 RID: 2981 RVA: 0x0003EDC1 File Offset: 0x0003CFC1
	public string GetDisplayName()
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".displayName").ToString());
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x0003EDE7 File Offset: 0x0003CFE7
	public virtual string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString());
	}

	// Token: 0x06000BA7 RID: 2983
	public abstract int SortOrder();

	// Token: 0x06000BA8 RID: 2984 RVA: 0x0003EE0D File Offset: 0x0003D00D
	public virtual bool IsBlockingOperation()
	{
		return false;
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x0003EE10 File Offset: 0x0003D010
	public virtual bool RequiresThrustProfile()
	{
		return false;
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0003EE13 File Offset: 0x0003D013
	public virtual bool HasResourceCost()
	{
		return false;
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06000BAB RID: 2987 RVA: 0x0003EE16 File Offset: 0x0003D016
	public string operationIconImagePath
	{
		get
		{
			return new StringBuilder("operations/ICO_").Append(base.GetType().Name).ToString();
		}
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0003EE37 File Offset: 0x0003D037
	public string GetOperationIconImagePath_On()
	{
		return new StringBuilder(this.operationIconImagePath).Append("_on").ToString();
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0003EE53 File Offset: 0x0003D053
	public string GetOperationIconImagePath_Off()
	{
		return new StringBuilder(this.operationIconImagePath).Append("_off").ToString();
	}

	// Token: 0x06000BAE RID: 2990
	public abstract bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null);

	// Token: 0x06000BAF RID: 2991 RVA: 0x0003EE6F File Offset: 0x0003D06F
	public virtual bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.GetPossibleTargets(actorState, null).Count > 0;
	}

	// Token: 0x06000BB0 RID: 2992
	public abstract List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null);

	// Token: 0x06000BB1 RID: 2993
	public abstract Type GetTargetingMethod();

	// Token: 0x06000BB2 RID: 2994
	public abstract float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null);

	// Token: 0x06000BB3 RID: 2995 RVA: 0x0003EE81 File Offset: 0x0003D081
	public virtual bool UseResourceCostDuration()
	{
		return false;
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x0003EE84 File Offset: 0x0003D084
	public virtual bool UseAbsoluteCompletionDateFromTrajectory()
	{
		return false;
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0003EE87 File Offset: 0x0003D087
	public TIOperationTemplate GetTemplate()
	{
		return this;
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0003EE8A File Offset: 0x0003D08A
	public virtual List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true)
	{
		return null;
	}

	// Token: 0x06000BB7 RID: 2999
	public abstract bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate);

	// Token: 0x06000BB8 RID: 3000
	public abstract OperationTiming GetOperationTiming();

	// Token: 0x06000BB9 RID: 3001 RVA: 0x0003EE8D File Offset: 0x0003D08D
	public virtual bool Repeatable()
	{
		return false;
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x0003EE90 File Offset: 0x0003D090
	public virtual bool WarnTarget(TIGameState target)
	{
		return false;
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x0003EE94 File Offset: 0x0003D094
	public TIOperationTemplate()
	{
		base.dataName = base.GetType().ToString();
		this._displayName = this.GetDisplayName();
		if (TemplateManager.Find<TIOperationTemplate>(base.dataName, false) == null)
		{
			TemplateManager.Add(this, typeof(TIOperationTemplate), false);
		}
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0003EEE4 File Offset: 0x0003D0E4
	public bool ValidOperation(TIGameState actorState, TIGameState target, TIResourcesCost cost = null)
	{
		List<TIGameState> possibleTargets = this.GetPossibleTargets(actorState, target);
		return possibleTargets.Count > 0 && possibleTargets.Contains(target) && (cost == null || cost.CanAfford(actorState.ref_faction, 1f, null, float.PositiveInfinity));
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0003EF2C File Offset: 0x0003D12C
	protected bool OnOperationConfirm_Base(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		if (!this.ValidOperation(actorState, target, resourcesCost))
		{
			return false;
		}
		TIDateTime tidateTime = TITimeState.Now();
		float duration_days = this.GetDuration_days(actorState, target, trajectory);
		if ((resourcesCost == null || !this.UseResourceCostDuration()) && duration_days > 0f)
		{
			tidateTime.AddDays(duration_days);
		}
		if (resourcesCost != null)
		{
			if (this.UseResourceCostDuration() && resourcesCost.completionTime_days > 0f)
			{
				tidateTime.AddDays(resourcesCost.completionTime_days);
			}
			resourcesCost.PayCost(actorState.ref_faction, base.GetType().ToString());
		}
		if (this.UseAbsoluteCompletionDateFromTrajectory() && trajectory != null)
		{
			tidateTime = trajectory.launchTime;
			if (tidateTime < TITimeState.Now())
			{
				this.OnOperationExecute(actorState, target);
				return true;
			}
		}
		if (this.GetOperationTiming() == OperationTiming.InstantExecution)
		{
			this.OnOperationExecute(actorState, target);
			return true;
		}
		return this.OperationConfirmed(actorState, target, tidateTime);
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x0003EFF2 File Offset: 0x0003D1F2
	public virtual bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		return this.OnOperationConfirm_Base(actorState, target, resourcesCost, trajectory);
	}

	// Token: 0x06000BBF RID: 3007
	public abstract void ExecuteOperation(TIGameState actorState, TIGameState target);

	// Token: 0x06000BC0 RID: 3008 RVA: 0x0003F000 File Offset: 0x0003D200
	public void OnOperationExecute(TIGameState actorState, TIGameState target)
	{
		if (TIGameState.Valid(actorState))
		{
			this.ExecuteOperation(actorState, target);
			GameControl.eventManager.TriggerEvent(new OperationExecuted(actorState, this, target), null, (from x in new object[] { actorState, target }.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			TIFactionState ref_faction = actorState.ref_faction;
			if (ref_faction == null)
			{
				return;
			}
			ref_faction.CheckForMilestonesCompleteViaOperation(this, target);
		}
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x0003F07E File Offset: 0x0003D27E
	public virtual void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
	}
}
