using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Systems.GameTime;

// Token: 0x020002FD RID: 765
public abstract class DeployArmyOperation : TIArmyOperationTemplate
{
	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x0003F2A8 File Offset: 0x0003D4A8
	// (set) Token: 0x06000BD9 RID: 3033 RVA: 0x0003F2B0 File Offset: 0x0003D4B0
	public bool JourneyMode { get; protected set; }

	// Token: 0x06000BDA RID: 3034 RVA: 0x0003F2B9 File Offset: 0x0003D4B9
	public void SetJourneyMode(bool allowJournies)
	{
		this.JourneyMode = allowJournies;
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x0003F2C2 File Offset: 0x0003D4C2
	public DeployArmyOperation(bool allowJournies_ = false)
	{
		this.JourneyMode = allowJournies_;
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x0003F2D1 File Offset: 0x0003D4D1
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0003F2D4 File Offset: 0x0003D4D4
	public override int SortOrder()
	{
		return 0;
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x0003F2D7 File Offset: 0x0003D4D7
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x0003F2E3 File Offset: 0x0003D4E3
	public override bool IsCombatOperation()
	{
		return false;
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x0003F2E8 File Offset: 0x0003D4E8
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		TIArmyState ref_army = actorState.ref_army;
		if (this.JourneyMode)
		{
			float num;
			ref_army.GetJourney(ref_army.currentRegion, target.ref_region, out num);
			return num;
		}
		return actorState.ref_army.GetDeploymentToAdjacentRegionDuration_Days(target.ref_region);
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x0003F32C File Offset: 0x0003D52C
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		TIArmyState ref_army = actorState.ref_army;
		TIRegionState ref_region = target.ref_region;
		if (this.JourneyMode && ref_army.currentRegion != ref_region)
		{
			float num;
			List<TIRegionState> journey = ref_army.GetJourney(ref_army.currentRegion, ref_region, out num);
			if (journey == null)
			{
				return false;
			}
			target = journey[1];
			if (ref_army.destinationQueue.Count == 0)
			{
				ref_army.destinationQueue.Add(ref_region);
			}
		}
		GameControl.eventManager.TriggerEvent(new ArmyPathChanged(ref_army), null, new object[] { ref_army.currentRegion });
		List<TIRegionState> list = ref_army.destinationQueue.ToList<TIRegionState>();
		bool flag = base.OnOperationConfirm(actorState, target, resourcesCost, trajectory);
		ref_army.destinationQueue = list;
		if (flag && ref_region != ref_army.currentRegion)
		{
			ref_army.SetIsMoving();
			if (ref_army.homeNation.wars.Contains(ref_region.nation) || (ref_army.AlienMegafaunaArmy && !ref_region.nation.alienNation))
			{
				TINotificationQueueState.LogArmyLaunchesTowardEnemyRegion(ref_army, ref_region);
			}
		}
		return flag;
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x0003F420 File Offset: 0x0003D620
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TIArmyState ref_army = actorState.ref_army;
		TIRegionState ref_region = target.ref_region;
		if (ref_army.currentRegion == target)
		{
			ref_army.currentOperations.RemoveAt(0);
			ref_army.SetNotMoving();
			return;
		}
		ref_army.MoveArmyToRegion(ref_region, false);
		if (ref_army.destinationQueue.Count > 0)
		{
			if (ref_army.destinationQueue.First<TIRegionState>() == ref_army.currentRegion)
			{
				ref_army.destinationQueue.RemoveAt(0);
			}
			if (ref_army.destinationQueue.Count > 0)
			{
				if (ref_army.ref_faction != null)
				{
					ref_army.ref_faction.playerControl.StartAction(new ConfirmOperationAction(actorState, ref_army.destinationQueue.First<TIRegionState>(), new DeployArmyOperation_OpenTarget(true), null, null));
				}
				else
				{
					new DeployArmyOperation_OpenTarget(false).OnOperationConfirm(ref_army, ref_army.destinationQueue.First<TIRegionState>(), null, null);
				}
			}
		}
		if (ref_army.destinationQueue.Count == 0 || !ref_army.InFriendlyRegion)
		{
			TINotificationQueueState.LogArmyArrivesInRegion(ref_army, ref_army.currentRegion);
		}
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x0003F51C File Offset: 0x0003D71C
	public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
		base.OnOperationCancel(actorState, target, opCompleteDate);
		if (!actorState.isArmyState)
		{
			return;
		}
		TIArmyState ref_army = actorState.ref_army;
		ref_army.destinationQueue.Clear();
		GameTimeManager.Singleton.CancelTimeEvent(ref_army.armyOperationCompleteEventName, ref_army, target, this, opCompleteDate);
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x0003F561 File Offset: 0x0003D761
	public override bool Equals(object obj)
	{
		return obj is DeployArmyOperation;
	}
}
