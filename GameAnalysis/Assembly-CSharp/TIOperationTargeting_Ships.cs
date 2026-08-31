using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002E6 RID: 742
public class TIOperationTargeting_Ships : TIOperationTargeting
{
	// Token: 0x06000B16 RID: 2838 RVA: 0x0003D154 File Offset: 0x0003B354
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TISpaceShipState) };
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0003D16B File Offset: 0x0003B36B
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.ShipList;
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x0003D16E File Offset: 0x0003B36E
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			GameControl.eventManager.TriggerEvent(new TargetShipsForFleetSplit(), null, Array.Empty<object>());
		}
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0003D194 File Offset: 0x0003B394
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DetargetShipForFleetSplit(), null, Array.Empty<object>());
		}
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x0003D1B9 File Offset: 0x0003B3B9
	public override TIGameState GetDefaultTarget()
	{
		return null;
	}
}
