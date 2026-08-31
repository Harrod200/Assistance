using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002EC RID: 748
public class TIOperationTargeting_FleetDefaultTargetOnly : TIOperationTargeting
{
	// Token: 0x06000B3D RID: 2877 RVA: 0x0003D6AF File Offset: 0x0003B8AF
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIGameState) };
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0003D6C6 File Offset: 0x0003B8C6
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Standard;
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x0003D6CC File Offset: 0x0003B8CC
	public override void Init(IOperation operationType, TIGameState actorState, TIGameState defaultTarget = null)
	{
		this.operationType = operationType;
		this.actorState = actorState;
		TISpaceFleetState tispaceFleetState = actorState as TISpaceFleetState;
		this.possibleTargets = operationType.GetPossibleTargets(tispaceFleetState, null);
		base.AttemptSetTarget(this.possibleTargets[0]);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0003D70E File Offset: 0x0003B90E
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
		}
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0003D71F File Offset: 0x0003B91F
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
		}
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x0003D72F File Offset: 0x0003B92F
	public override TIGameState GetDefaultTarget()
	{
		return this.possibleTargets[0];
	}
}
