using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002E5 RID: 741
public class TIOperationTargeting_FleetHab : TIOperationTargeting
{
	// Token: 0x06000B0E RID: 2830 RVA: 0x0003D004 File Offset: 0x0003B204
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TISpaceFleetState),
			typeof(TIHabState)
		};
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x0003D02B File Offset: 0x0003B22B
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Dropdown;
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0003D030 File Offset: 0x0003B230
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetDefaultTarget(forceTarget);
			base.SetActivation(this);
			GameControl.eventManager.TriggerEvent(new TargetFleets(this.actorState), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x0003D0A4 File Offset: 0x0003B2A4
	public override void Shutdown()
	{
		base.SetShutdown();
		GameControl.eventManager.TriggerEvent(new DeTargetFleets(), null, Array.Empty<object>());
		GameControl.eventManager.RemoveListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null);
		GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0003D0FA File Offset: 0x0003B2FA
	public override TIGameState GetDefaultTarget()
	{
		if (this.possibleTargets.Count <= 0)
		{
			return null;
		}
		if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
		{
			return GeneralControlsController.UIOtherSelectedState;
		}
		return this.possibleTargets[0];
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0003D130 File Offset: 0x0003B330
	public void FleetSelectedForTargeting(FleetSelectedEvent e)
	{
		base.AttemptSetTarget(e.fleet);
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x0003D13E File Offset: 0x0003B33E
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.AttemptSetTarget(e.hab);
	}
}
