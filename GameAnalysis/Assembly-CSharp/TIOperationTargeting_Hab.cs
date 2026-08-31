using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002F5 RID: 757
public class TIOperationTargeting_Hab : TIOperationTargeting
{
	// Token: 0x06000B82 RID: 2946 RVA: 0x0003E5D8 File Offset: 0x0003C7D8
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIHabState) };
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x0003E5EF File Offset: 0x0003C7EF
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.Dropdown;
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x0003E5F2 File Offset: 0x0003C7F2
	public override void Init(IOperation operationType, TIGameState actorState, TIGameState defaultTarget = null)
	{
		this.operationType = operationType;
		this.actorState = actorState;
		this.fleet = actorState as TISpaceFleetState;
		this.faction = this.fleet.faction;
		this.possibleTargets = operationType.GetPossibleTargets(actorState, defaultTarget);
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x0003E630 File Offset: 0x0003C830
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			base.SetDefaultTarget(forceTarget);
			GameControl.eventManager.TriggerEvent(new TargetHabs(this.fleet), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x0003E688 File Offset: 0x0003C888
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetHabs(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
		}
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x0003E6C4 File Offset: 0x0003C8C4
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.AttemptSetTarget(e.hab);
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0003E6D4 File Offset: 0x0003C8D4
	public override TIGameState GetDefaultTarget()
	{
		if (this.fleet.dockedAtHab)
		{
			TIHabState ref_hab = this.fleet.ref_hab;
			if (((ref_hab != null) ? ref_hab.ref_faction : null) != this.fleet.faction && this.possibleTargets.Contains(this.fleet.ref_hab))
			{
				return this.fleet.ref_hab;
			}
		}
		if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
		{
			return GeneralControlsController.UIOtherSelectedState;
		}
		return this.possibleTargets[0];
	}

	// Token: 0x04000E99 RID: 3737
	private TISpaceFleetState fleet;

	// Token: 0x04000E9A RID: 3738
	private TIFactionState faction;
}
