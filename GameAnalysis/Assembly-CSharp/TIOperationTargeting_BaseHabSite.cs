using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002F3 RID: 755
public class TIOperationTargeting_BaseHabSite : TIOperationTargeting
{
	// Token: 0x06000B6B RID: 2923 RVA: 0x0003E0F3 File Offset: 0x0003C2F3
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.TwoStage;
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0003E0F6 File Offset: 0x0003C2F6
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TIHabSiteState),
			typeof(TIHabState)
		};
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x0003E120 File Offset: 0x0003C320
	public override void Init(IOperation operationType, TIGameState actorState, TIGameState defaultTarget = null)
	{
		this.operationType = operationType;
		this.actorState = actorState;
		this.fleet = actorState as TISpaceFleetState;
		this.faction = this.fleet.faction;
		this.spaceBody = this.fleet.barycenter as TISpaceBodyState;
		this.possibleTargets = operationType.GetPossibleTargets(actorState, defaultTarget);
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x0003E17C File Offset: 0x0003C37C
	private void HabSiteSelectedForTargeting(HabSiteSelectedEvent e)
	{
		base.AttemptSetTarget(e.habSite);
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0003E18A File Offset: 0x0003C38A
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.AttemptSetTarget(e.hab);
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x0003E198 File Offset: 0x0003C398
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			base.SetDefaultTarget(forceTarget);
			GameControl.eventManager.TriggerEvent(new TargetHabSites(this.actorState, this.spaceBody), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<HabSiteSelectedEvent>(new EventManager.EventDelegate<HabSiteSelectedEvent>(this.HabSiteSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0003E210 File Offset: 0x0003C410
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetHabSites(this.faction), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<HabSiteSelectedEvent>(new EventManager.EventDelegate<HabSiteSelectedEvent>(this.HabSiteSelectedForTargeting), null);
		}
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x0003E274 File Offset: 0x0003C474
	public override TIGameState GetDefaultTarget()
	{
		if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
		{
			return GeneralControlsController.UIOtherSelectedState;
		}
		return this.possibleTargets[0];
	}

	// Token: 0x04000E93 RID: 3731
	private TISpaceBodyState spaceBody;

	// Token: 0x04000E94 RID: 3732
	private TIFactionState faction;

	// Token: 0x04000E95 RID: 3733
	private TISpaceFleetState fleet;
}
