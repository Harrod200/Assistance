using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002F9 RID: 761
public interface IOperation
{
	// Token: 0x06000B90 RID: 2960
	string GetDisplayName();

	// Token: 0x06000B91 RID: 2961
	string GetDescription(TIGameState actorState = null, TIGameState targetState = null);

	// Token: 0x06000B92 RID: 2962
	int SortOrder();

	// Token: 0x06000B93 RID: 2963
	bool IsBlockingOperation();

	// Token: 0x06000B94 RID: 2964
	bool RequiresThrustProfile();

	// Token: 0x06000B95 RID: 2965
	bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null);

	// Token: 0x06000B96 RID: 2966
	bool ActorCanPerformOperation(TIGameState actorState, TIGameState targetState = null);

	// Token: 0x06000B97 RID: 2967
	List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null);

	// Token: 0x06000B98 RID: 2968
	Type GetTargetingMethod();

	// Token: 0x06000B99 RID: 2969
	string GetOperationIconImagePath_On();

	// Token: 0x06000B9A RID: 2970
	string GetOperationIconImagePath_Off();

	// Token: 0x06000B9B RID: 2971
	bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null);

	// Token: 0x06000B9C RID: 2972
	void OnOperationExecute(TIGameState actorState, TIGameState target);

	// Token: 0x06000B9D RID: 2973
	void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate);

	// Token: 0x06000B9E RID: 2974
	float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null);

	// Token: 0x06000B9F RID: 2975
	bool HasResourceCost();

	// Token: 0x06000BA0 RID: 2976
	List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true);

	// Token: 0x06000BA1 RID: 2977
	TIOperationTemplate GetTemplate();

	// Token: 0x06000BA2 RID: 2978
	OperationTiming GetOperationTiming();

	// Token: 0x06000BA3 RID: 2979
	bool WarnTarget(TIGameState target);

	// Token: 0x06000BA4 RID: 2980
	bool Repeatable();
}
