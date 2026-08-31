using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200034D RID: 845
public abstract class FoundBaseOperation : FoundHabOperation
{
	// Token: 0x06000EB0 RID: 3760 RVA: 0x000492D4 File Offset: 0x000474D4
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		TIFactionState ref_faction = actorState.ref_faction;
		if ((targetState.isSpaceBodyState || targetState.isHabSiteState) && targetState.ref_spaceBody.habSites.Length != 0)
		{
			TISpaceBodyState ref_spaceBody = targetState.ref_spaceBody;
			if (ref_spaceBody != null && ref_faction.EligibleForFoundingBase(ref_spaceBody) && TIEffectsState.CheckForAnyEffectInContext(this.GetRequiredConstructionTechEffectContext(), actorState))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x00049330 File Offset: 0x00047530
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return defaultTarget.ref_spaceBody.vacantHabSites.ToList<TIHabSiteState>().ConvertAll<TIGameState>((TIHabSiteState x) => x);
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x00049366 File Offset: 0x00047566
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_HabSite);
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x00049372 File Offset: 0x00047572
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost, Trajectory trajectory = null)
	{
		if (resourcesCost == null)
		{
			Log.Error("Null resources cost passed to FoundBaseOperation.OnOperationConfirm", Array.Empty<object>());
			return false;
		}
		this.deliveryDuration_days = resourcesCost.completionTime_days;
		if (base.OnOperationConfirm(actorState, target, resourcesCost, trajectory))
		{
			target.ref_habSite.MarkPendingHab();
			return true;
		}
		return false;
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x000493B0 File Offset: 0x000475B0
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TIHabSiteState ref_habSite = target.ref_habSite;
		ref_habSite.FoundHab();
		base.FoundHab(actorState.ref_faction, ref_habSite, this.GetTier());
	}
}
