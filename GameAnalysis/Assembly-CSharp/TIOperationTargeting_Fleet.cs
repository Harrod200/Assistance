using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002E4 RID: 740
public class TIOperationTargeting_Fleet : TIOperationTargeting
{
	// Token: 0x06000B07 RID: 2823 RVA: 0x0003CF18 File Offset: 0x0003B118
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TISpaceFleetState) };
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x0003CF2F File Offset: 0x0003B12F
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Dropdown;
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0003CF34 File Offset: 0x0003B134
	public override void Activate(TIGameState forceTarget = null)
	{
		base.SetDefaultTarget(forceTarget);
		base.SetActivation(this);
		GameControl.eventManager.TriggerEvent(new TargetFleets(this.actorState), null, Array.Empty<object>());
		GameControl.eventManager.AddListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null, null, true, false);
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0003CF84 File Offset: 0x0003B184
	public override void Shutdown()
	{
		base.SetShutdown();
		GameControl.eventManager.TriggerEvent(new DeTargetFleets(), null, Array.Empty<object>());
		GameControl.eventManager.RemoveListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null);
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0003CFB8 File Offset: 0x0003B1B8
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

	// Token: 0x06000B0C RID: 2828 RVA: 0x0003CFEE File Offset: 0x0003B1EE
	public void FleetSelectedForTargeting(FleetSelectedEvent e)
	{
		base.AttemptSetTarget(e.fleet);
	}
}
