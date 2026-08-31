using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200032A RID: 810
public class AlienCrashdownOperation : TISpaceFleetOperationTemplate_Special
{
	// Token: 0x06000D57 RID: 3415 RVA: 0x00042AEF File Offset: 0x00040CEF
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x00042AF2 File Offset: 0x00040CF2
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.Crashdown };
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x00042B01 File Offset: 0x00040D01
	public override int SortOrder()
	{
		return 3;
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x00042B04 File Offset: 0x00040D04
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x00042B07 File Offset: 0x00040D07
	public override bool isAlien()
	{
		return true;
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x00042B0A File Offset: 0x00040D0A
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00042B0D File Offset: 0x00040D0D
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x00042B10 File Offset: 0x00040D10
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.ShipsWithSpecialModuleRule(this.RequiredCapability()).Count > 0;
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x00042B2C File Offset: 0x00040D2C
	public bool CanCrashdown(TIGameState actorState)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (!ref_fleet.transferAssigned && !ref_fleet.inCombatOrWaitingForCombat && ref_fleet.orbitState.interfaceOrbit && ref_fleet.orbitState.barycenter.isEarth)
		{
			return ref_fleet.ships.Where<TISpaceShipState>((TISpaceShipState ship) => ship.crashdownEligible).Any<TISpaceShipState>();
		}
		return false;
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x00042BA0 File Offset: 0x00040DA0
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.CanCrashdown(actorState) && base.ActorCanPerformOperation_PassInterruptCheck(actorState);
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x00042BB4 File Offset: 0x00040DB4
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0.75f;
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x00042BBB File Offset: 0x00040DBB
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x00042BC8 File Offset: 0x00040DC8
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = new List<TIGameState>();
		List<TIFactionIdeologyTemplate> list2 = (from x in GameStateManager.AllFactions()
			where x.ideology.proAlien
			select x into y
			select y.ideology).ToList<TIFactionIdeologyTemplate>();
		foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
		{
			TIFactionState executiveFaction = tiregionState.nation.executiveFaction;
			if (!tiregionState.antiSpaceDefenses || (!(executiveFaction == null) && list2.Contains(executiveFaction.ideology)))
			{
				list.Add(tiregionState);
			}
		}
		return list;
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x00042C80 File Offset: 0x00040E80
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (this.CanCrashdown(actorState))
		{
			TIRegionState ref_region = target.ref_region;
			TISpaceFleetState ref_fleet = actorState.ref_fleet;
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			bool flag = false;
			foreach (TISpaceShipState tispaceShipState in ref_fleet.ships.Where<TISpaceShipState>((TISpaceShipState ship) => ship.crashdownEligible))
			{
				foreach (TICouncilorState ticouncilorState in tispaceShipState.councilorPassengers)
				{
					flag = true;
					ticouncilorState.SetLocation(target);
					foreach (TIOrgState tiorgState in ticouncilorState.orgs)
					{
						if (tiorgState.templateName == TemplateManager.global.alienShockTroopOrgDataName)
						{
							tiorgState.SetHomeRegion(ref_region);
						}
					}
					list.Add(tispaceShipState);
				}
			}
			if (flag)
			{
				ref_region.alienCrashdown.TriggerCrashdown(false);
				foreach (TISpaceShipState tispaceShipState2 in list)
				{
					tispaceShipState2.DestroyShip(false, null);
				}
			}
		}
	}
}
