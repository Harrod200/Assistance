using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000301 RID: 769
public class AllArmiesGoHomeOperation : TIArmyOperationTemplate
{
	// Token: 0x06000BFF RID: 3071 RVA: 0x0003F979 File Offset: 0x0003DB79
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x0003F97C File Offset: 0x0003DB7C
	public override int SortOrder()
	{
		return 1;
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x0003F97F File Offset: 0x0003DB7F
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x0003F98B File Offset: 0x0003DB8B
	public override bool IsCombatOperation()
	{
		return false;
	}

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000C03 RID: 3075 RVA: 0x0003F98E File Offset: 0x0003DB8E
	public override bool isConvenienceOperation
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x0003F991 File Offset: 0x0003DB91
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x0003F998 File Offset: 0x0003DB98
	private List<TIArmyState> EligibleArmies(TIArmyState army)
	{
		List<TIArmyState> list = new List<TIArmyState>();
		ArmyGoHomeOperation armyGoHomeOperation = new ArmyGoHomeOperation();
		foreach (TIArmyState tiarmyState in army.currentRegion.armies)
		{
			if (tiarmyState.faction == army.faction && armyGoHomeOperation.ActorCanPerformOperation(army, null))
			{
				list.Add(tiarmyState);
			}
		}
		return list;
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x0003FA1C File Offset: 0x0003DC1C
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		List<TIArmyState> list = this.EligibleArmies(actorState.ref_army);
		return list.Count > 1 && list.Contains(actorState.ref_army);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x0003FA4D File Offset: 0x0003DC4D
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.OpVisibleToActor(actorState, target);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x0003FA58 File Offset: 0x0003DC58
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		if (actorState.isArmyState && actorState.ref_army.homeRegion != null)
		{
			TIArmyState ref_army = actorState.ref_army;
			return new List<TIGameState> { ref_army.homeRegion.ref_gameState };
		}
		return new List<TIGameState>();
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x0003FAA4 File Offset: 0x0003DCA4
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (actorState.isArmyState)
		{
			ArmyGoHomeOperation armyGoHomeOperation = new ArmyGoHomeOperation();
			foreach (TIArmyState tiarmyState in this.EligibleArmies(actorState.ref_army))
			{
				if (armyGoHomeOperation.ActorCanPerformOperation(tiarmyState, tiarmyState.homeRegion))
				{
					armyGoHomeOperation.OnOperationConfirm(tiarmyState, tiarmyState.homeRegion, null, null);
				}
			}
		}
	}
}
