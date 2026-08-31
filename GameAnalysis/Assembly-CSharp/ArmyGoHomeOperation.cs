using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000300 RID: 768
public class ArmyGoHomeOperation : TIArmyOperationTemplate
{
	// Token: 0x06000BF5 RID: 3061 RVA: 0x0003F816 File Offset: 0x0003DA16
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x0003F819 File Offset: 0x0003DA19
	public override int SortOrder()
	{
		return 1;
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x0003F81C File Offset: 0x0003DA1C
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x0003F828 File Offset: 0x0003DA28
	public override bool IsCombatOperation()
	{
		return false;
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x0003F82B File Offset: 0x0003DA2B
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 45f;
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x0003F834 File Offset: 0x0003DA34
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_army.armyType == ArmyType.Human && actorState.ref_army.currentRegion != actorState.ref_army.homeRegion && !actorState.ref_army.atSea && !actorState.ref_army.CanGetTo(actorState.ref_army.homeRegion, null, null, null);
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x0003F896 File Offset: 0x0003DA96
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		if (this.OpVisibleToActor(actorState, target) && actorState.ref_army.CurrentOperations().Count == 0)
		{
			TINationState homeNation = actorState.ref_army.homeNation;
			return homeNation != null && homeNation.wars.Count == 0;
		}
		return false;
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x0003F8D4 File Offset: 0x0003DAD4
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		if (actorState.isArmyState && actorState.ref_army.homeRegion != null)
		{
			TIArmyState ref_army = actorState.ref_army;
			return new List<TIGameState> { ref_army.homeRegion.ref_gameState };
		}
		return new List<TIGameState>();
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x0003F920 File Offset: 0x0003DB20
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TIArmyState ref_army = actorState.ref_army;
		TIRegionState ref_region = target.ref_region;
		if (actorState.ref_army.homeNation.wars.Count == 0)
		{
			ref_army.MoveArmyToRegion(ref_region, false);
			TINotificationQueueState.LogArmyArrivesInRegion(ref_army, ref_army.currentRegion);
		}
		ref_army.homeNation.SetArmyAccessibilityDirty();
	}
}
