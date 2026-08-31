using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000329 RID: 809
public class AlienEarthSurveillanceOperation : TISpaceFleetOperationTemplate_Special
{
	// Token: 0x06000D47 RID: 3399 RVA: 0x0004297C File Offset: 0x00040B7C
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x0004297F File Offset: 0x00040B7F
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.Surveillance };
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x0004298E File Offset: 0x00040B8E
	public override int SortOrder()
	{
		return 3;
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x00042991 File Offset: 0x00040B91
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x00042994 File Offset: 0x00040B94
	public override bool isAlien()
	{
		return true;
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x00042997 File Offset: 0x00040B97
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x0004299A File Offset: 0x00040B9A
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x0004299D File Offset: 0x00040B9D
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x000429AB File Offset: 0x00040BAB
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.ShipsWithSpecialModuleRule(this.RequiredCapability()).Count > 0;
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x000429C8 File Offset: 0x00040BC8
	private bool CanSurveil(TIGameState actorState)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		return !ref_fleet.transferAssigned && !ref_fleet.inCombatOrWaitingForCombat && ref_fleet.orbitState.interfaceOrbit && ref_fleet.orbitState.barycenter.isEarth && ref_fleet.ShipsWithSpecialModuleRule(this.RequiredCapability()).Count > 0;
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x00042A21 File Offset: 0x00040C21
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.CanSurveil(actorState) && base.ActorCanPerformOperation_PassInterruptCheck(actorState);
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x00042A35 File Offset: 0x00040C35
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 192f;
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00042A3C File Offset: 0x00040C3C
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00042A48 File Offset: 0x00040C48
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (this.CanSurveil(actorState))
		{
			int count = actorState.ref_fleet.ShipsWithSpecialModuleRule(this.RequiredCapability()).Count;
			foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
			{
				if (TIUtilities.RandomFloatValue() < tiregionState.populationInMillions / 10f)
				{
					tiregionState.ConductAbductions(actorState.ref_faction, count);
				}
			}
			actorState.ref_fleet.ExpendSpecialModuleCapability(this.RequiredCapability(), false, false);
		}
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00042AC0 File Offset: 0x00040CC0
	public override List<Type> BreakthroughOps()
	{
		return new List<Type>
		{
			typeof(CancelFleetOperation),
			typeof(MergeFleetOperation)
		};
	}
}
