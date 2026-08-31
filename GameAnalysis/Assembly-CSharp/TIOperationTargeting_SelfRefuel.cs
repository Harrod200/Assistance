using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002E7 RID: 743
public class TIOperationTargeting_SelfRefuel : TIOperationTargeting
{
	// Token: 0x06000B1C RID: 2844 RVA: 0x0003D1C4 File Offset: 0x0003B3C4
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TISpaceFleetState) };
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0003D1DB File Offset: 0x0003B3DB
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.RefuelManager;
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0003D1DE File Offset: 0x0003B3DE
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			GameControl.eventManager.TriggerEvent(new InitiateSharePropellant(), null, Array.Empty<object>());
		}
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0003D204 File Offset: 0x0003B404
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new EndSharePropellant(), null, Array.Empty<object>());
		}
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0003D229 File Offset: 0x0003B429
	public override TIGameState GetDefaultTarget()
	{
		return null;
	}
}
