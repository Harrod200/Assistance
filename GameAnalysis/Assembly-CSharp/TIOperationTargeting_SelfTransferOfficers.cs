using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002E8 RID: 744
public class TIOperationTargeting_SelfTransferOfficers : TIOperationTargeting
{
	// Token: 0x06000B22 RID: 2850 RVA: 0x0003D234 File Offset: 0x0003B434
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TISpaceFleetState) };
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0003D24B File Offset: 0x0003B44B
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.TransferOfficerManager;
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x0003D24E File Offset: 0x0003B44E
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			GameControl.eventManager.TriggerEvent(new InitiateTransferOfficers(), null, Array.Empty<object>());
		}
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x0003D274 File Offset: 0x0003B474
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new EndTransferOfficers(), null, Array.Empty<object>());
		}
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x0003D299 File Offset: 0x0003B499
	public override TIGameState GetDefaultTarget()
	{
		return null;
	}
}
