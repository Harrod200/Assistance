using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000328 RID: 808
public class AlienLandArmyOperation : TISpaceFleetOperationTemplate_Special
{
	// Token: 0x06000D37 RID: 3383 RVA: 0x0004271B File Offset: 0x0004091B
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x0004271E File Offset: 0x0004091E
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.LandArmy };
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x0004272D File Offset: 0x0004092D
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x00042730 File Offset: 0x00040930
	public override bool isAlien()
	{
		return true;
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x00042733 File Offset: 0x00040933
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00042736 File Offset: 0x00040936
	public override int SortOrder()
	{
		return 3;
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00042739 File Offset: 0x00040939
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x00042745 File Offset: 0x00040945
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000D3F RID: 3391 RVA: 0x00042748 File Offset: 0x00040948
	public override bool MustAcceptCombat()
	{
		return true;
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0004274B File Offset: 0x0004094B
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.ShipsWithSpecialModuleRule(this.RequiredCapability()).Count > 0;
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x00042768 File Offset: 0x00040968
	public bool CanLandArmy(TIGameState actorState)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (!ref_fleet.transferAssigned && !ref_fleet.inCombatOrWaitingForCombat && ref_fleet.orbitState.interfaceOrbit && ref_fleet.orbitState.barycenter.isEarth)
		{
			return ref_fleet.ships.Where<TISpaceShipState>((TISpaceShipState ship) => ship.landArmyEligible).Any<TISpaceShipState>();
		}
		return false;
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x000427DC File Offset: 0x000409DC
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.CanLandArmy(actorState) && base.ActorCanPerformOperation_PassInterruptCheck(actorState);
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x000427F0 File Offset: 0x000409F0
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 3f;
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x000427F8 File Offset: 0x000409F8
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = new List<TIGameState>();
		foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
		{
			if (!tiregionState.alienLanding.landingPresent && (tiregionState.nation.alienNation || !tiregionState.antiSpaceDefenses || (tiregionState.nation.executiveFaction != null && tiregionState.nation.allies.Contains(GameStateManager.AlienNation()) && tiregionState.nation.executiveFaction.IsAlienProxy)))
			{
				list.Add(tiregionState);
			}
		}
		return list;
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x0004288C File Offset: 0x00040A8C
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (this.CanLandArmy(actorState))
		{
			TISpaceFleetState ref_fleet = actorState.ref_fleet;
			TIRegionState ref_region = target.ref_region;
			foreach (TISpaceShipState tispaceShipState in ref_fleet.ships)
			{
				if (tispaceShipState.landArmyEligible)
				{
					ref_region.alienLanding.TriggerLanding(-1f);
					foreach (TICouncilorState ticouncilorState in tispaceShipState.councilorPassengers)
					{
						ticouncilorState.SetLocation(ref_region);
					}
					tispaceShipState.DestroyShip(false, null);
					TIEffectsState.AddEffect(TemplateManager.Find<TIEffectTemplate>("Effect_ManyAliensOnEarth", false), GameStateManager.AlienFaction(), null, null, "");
					break;
				}
			}
		}
	}
}
