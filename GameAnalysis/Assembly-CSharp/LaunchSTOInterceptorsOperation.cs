using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000341 RID: 833
public class LaunchSTOInterceptorsOperation : TISpaceBodyOperationTemplate
{
	// Token: 0x06000E53 RID: 3667 RVA: 0x00047DD5 File Offset: 0x00045FD5
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x00047DD8 File Offset: 0x00045FD8
	public override int SortOrder()
	{
		return -1;
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x00047DDC File Offset: 0x00045FDC
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		if (target != null && target.isSpaceBodyState && target.ref_spaceBody.isEarth && actorState.ref_faction.CanLaunchSTOFighters)
		{
			return GameStateManager.IterateByClass<TISpaceCombatState>(false).None<TISpaceCombatState>((TISpaceCombatState x) => x.active);
		}
		return false;
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x00047E40 File Offset: 0x00046040
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return this.ActorCanPerformOperation(actorState, targetState) && actorState.ref_faction.GetCurrentResourceAmount(FactionResource.Boost) >= actorState.ref_faction.cachedSTOFighterMinimumBoost && !GameControl.spaceCombat.HasActiveState();
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00047E74 File Offset: 0x00046074
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return actorState.ref_faction.TargetsForSTOFighters.ConvertAll<TIGameState>((TISpaceAssetState x) => x.ref_gameState);
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00047EA5 File Offset: 0x000460A5
	public override bool HasResourceCost()
	{
		return false;
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00047EA8 File Offset: 0x000460A8
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_FleetHab);
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x00047EB4 File Offset: 0x000460B4
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x00047EBB File Offset: 0x000460BB
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		return base.OnOperationConfirm(actorState, target, resourcesCost, trajectory);
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x00047EC8 File Offset: 0x000460C8
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		return true;
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x00047ECC File Offset: 0x000460CC
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState dummyFleet = TISpaceFleetState.CreateAtRunTime(actorState.ref_faction, new List<TISpaceShipState>(), target.ref_orbit, null, null, false, true, null);
		dummyFleet.dummyFleet = true;
		GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
		{
			dummyFleet.ForceDisplayName(x, Loc.T("LaunchSTOInterceptorsOperation.dummyFleetName", new object[] { actorState.ref_faction.adjective }));
		});
		dummyFleet.InitiateCombat(target.ref_fleet, target.ref_hab, true);
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x00047F4C File Offset: 0x0004614C
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[]
		{
			actorState.ref_faction.cachedSTOFighterMinimumBoost.ToString("N1"),
			TemplateManager.global.boostInlineSpritePath
		});
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x00047FA8 File Offset: 0x000461A8
	public override bool WarnTarget(TIGameState target)
	{
		return target.isHabState && target.ref_hab.AtrocitiesFromDestruction() > 0;
	}
}
