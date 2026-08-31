using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000344 RID: 836
public abstract class LaunchMultipleProbesOperation : TISpaceBodyOperationTemplate
{
	// Token: 0x06000E78 RID: 3704 RVA: 0x00048901 File Offset: 0x00046B01
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x00048904 File Offset: 0x00046B04
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Presupplied);
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x00048910 File Offset: 0x00046B10
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x00048917 File Offset: 0x00046B17
	public override bool HasResourceCost()
	{
		return false;
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0004891A File Offset: 0x00046B1A
	public override int SortOrder()
	{
		return 0;
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0004891D File Offset: 0x00046B1D
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return false;
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x00048920 File Offset: 0x00046B20
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return false;
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x00048924 File Offset: 0x00046B24
	public override List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true)
	{
		LaunchProbeOperation launchProbeOperation = new LaunchProbeOperation();
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		Func<TIResourcesCost, bool> <>9__1;
		foreach (TIGameState tigameState in this.GetPossibleTargets(faction, null))
		{
			List<TIResourcesCost> list = (from x in launchProbeOperation.ResourceCostOptions(faction, tigameState, faction, false)
				where x.anyDebit
				select x).ToList<TIResourcesCost>();
			if (list.Count > 0)
			{
				IEnumerable<TIResourcesCost> enumerable = list;
				Func<TIResourcesCost, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (TIResourcesCost x) => x.CanAfford(faction, 1f, null, float.PositiveInfinity));
				}
				list = enumerable.OrderByDescending<TIResourcesCost, bool>(func).ThenBy<TIResourcesCost, float>((TIResourcesCost x) => x.completionTime_days).ThenBy<TIResourcesCost, float>((TIResourcesCost x) => x.GetSingleCostValue(FactionResource.Boost))
					.ToList<TIResourcesCost>();
				tiresourcesCost.SumCosts_NoDuration(list[0]);
			}
		}
		return new List<TIResourcesCost> { tiresourcesCost };
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x00048A78 File Offset: 0x00046C78
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		LaunchProbeOperation launchProbeOperation = new LaunchProbeOperation();
		Func<TIResourcesCost, bool> <>9__0;
		foreach (TIGameState tigameState in this.GetPossibleTargets(actorState, null))
		{
			List<TIResourcesCost> list = launchProbeOperation.ResourceCostOptions(actorState.ref_faction, tigameState, actorState.ref_faction, false);
			if (list.Count > 0)
			{
				IEnumerable<TIResourcesCost> enumerable = list;
				Func<TIResourcesCost, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (TIResourcesCost x) => x.CanAfford(actorState.ref_faction, 1f, null, float.PositiveInfinity));
				}
				list = enumerable.OrderByDescending<TIResourcesCost, bool>(func).ThenBy<TIResourcesCost, float>((TIResourcesCost x) => x.completionTime_days).ThenBy<TIResourcesCost, float>((TIResourcesCost x) => x.GetSingleCostValue(FactionResource.Boost))
					.ToList<TIResourcesCost>();
				TIResourcesCost tiresourcesCost = new TIResourcesCost();
				tiresourcesCost.SetCompletionTime_Days(list[0].completionTime_days);
				launchProbeOperation.OnOperationConfirm(actorState, tigameState, tiresourcesCost, null);
			}
		}
	}
}
