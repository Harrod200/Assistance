using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002E3 RID: 739
public class TIOperationTargeting_Self : TIOperationTargeting
{
	// Token: 0x06000B01 RID: 2817 RVA: 0x0003CED1 File Offset: 0x0003B0D1
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Self;
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x0003CED4 File Offset: 0x0003B0D4
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIGameState) };
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0003CEEB File Offset: 0x0003B0EB
	public override void Activate(TIGameState forceTarget = null)
	{
		base.SetActivation(this);
		base.AttemptSetTarget(this.GetDefaultTarget());
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x0003CF00 File Offset: 0x0003B100
	public override TIGameState GetDefaultTarget()
	{
		return this.actorState;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0003CF08 File Offset: 0x0003B108
	public override void Shutdown()
	{
		base.SetShutdown();
	}
}
