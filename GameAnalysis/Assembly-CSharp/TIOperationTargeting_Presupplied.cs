using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002ED RID: 749
public class TIOperationTargeting_Presupplied : TIOperationTargeting
{
	// Token: 0x06000B44 RID: 2884 RVA: 0x0003D745 File Offset: 0x0003B945
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIGameState) };
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0003D75C File Offset: 0x0003B95C
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Standard;
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x0003D75F File Offset: 0x0003B95F
	public override void Init(IOperation operationType, TIGameState actorState, TIGameState defaultTarget = null)
	{
		this.operationType = operationType;
		this.actorState = actorState;
		this.possibleTargets = new List<TIGameState> { defaultTarget };
		base.AttemptSetTarget(defaultTarget);
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0003D788 File Offset: 0x0003B988
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
		}
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0003D799 File Offset: 0x0003B999
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
		}
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x0003D7A9 File Offset: 0x0003B9A9
	public override TIGameState GetDefaultTarget()
	{
		return this.possibleTargets[0];
	}
}
