using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002FC RID: 764
public class CancelArmyOperation : TIArmyOperationTemplate
{
	// Token: 0x06000BCE RID: 3022 RVA: 0x0003F1E0 File Offset: 0x0003D3E0
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x0003F1E3 File Offset: 0x0003D3E3
	public override int SortOrder()
	{
		return 9;
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x0003F1E7 File Offset: 0x0003D3E7
	public override bool IsCombatOperation()
	{
		return false;
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x0003F1EA File Offset: 0x0003D3EA
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_army.CurrentOperations().Count > 0;
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0003F1FF File Offset: 0x0003D3FF
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.ref_army.CurrentOperations().Count > 0 && actorState.ref_army.SeaTransitStage() != ArmySeaTransitStage.Sea_DestinationRegion;
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x0003F227 File Offset: 0x0003D427
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x0003F22E File Offset: 0x0003D42E
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x0003F23A File Offset: 0x0003D43A
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x0003F248 File Offset: 0x0003D448
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TIArmyState ref_army = actorState.ref_army;
		if (ref_army.SeaTransitStage() != ArmySeaTransitStage.None)
		{
			ref_army.CancelSeaTransit();
		}
		ref_army.ClearOperations();
		ref_army.SetArmyDataDirty();
		GameControl.eventManager.TriggerEvent(new ArmyArrivesInRegion(ref_army, ref_army.currentRegion), null, new object[] { ref_army, ref_army.currentRegion });
	}
}
