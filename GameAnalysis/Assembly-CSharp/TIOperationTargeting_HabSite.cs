using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002F1 RID: 753
public class TIOperationTargeting_HabSite : TIOperationTargeting
{
	// Token: 0x06000B56 RID: 2902 RVA: 0x0003D946 File Offset: 0x0003BB46
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIHabSiteState) };
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x0003D95D File Offset: 0x0003BB5D
	public override OperationTargetingUIType UIType()
	{
		return OperationTargetingUIType.TwoStage;
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x0003D960 File Offset: 0x0003BB60
	public override void Init(IOperation operationType, TIGameState actorState, TIGameState defaultTarget = null)
	{
		this.operationType = operationType;
		this.actorState = actorState;
		this.faction = actorState.ref_faction;
		if (actorState.isSpaceFleetState)
		{
			this.fleet = actorState.ref_fleet;
			this.spaceBody = this.fleet.barycenter.ref_spaceBody;
		}
		else
		{
			this.spaceBody = defaultTarget.ref_spaceBody;
		}
		this.possibleTargets = operationType.GetPossibleTargets(actorState, defaultTarget);
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x0003D9CD File Offset: 0x0003BBCD
	private void HabSiteSelectedForTargeting(HabSiteSelectedEvent e)
	{
		base.AttemptSetTarget(e.habSite);
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x0003D9DC File Offset: 0x0003BBDC
	public override void Activate(TIGameState forceTarget = null)
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			base.SetDefaultTarget(forceTarget);
			GameControl.eventManager.TriggerEvent(new TargetHabSites(this.actorState, this.spaceBody), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<HabSiteSelectedEvent>(new EventManager.EventDelegate<HabSiteSelectedEvent>(this.HabSiteSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x0003DA3C File Offset: 0x0003BC3C
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetHabSites(this.faction), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<HabSiteSelectedEvent>(new EventManager.EventDelegate<HabSiteSelectedEvent>(this.HabSiteSelectedForTargeting), null);
		}
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x0003DA89 File Offset: 0x0003BC89
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

	// Token: 0x04000E8E RID: 3726
	private TISpaceBodyState spaceBody;

	// Token: 0x04000E8F RID: 3727
	private TIFactionState faction;

	// Token: 0x04000E90 RID: 3728
	private TISpaceFleetState fleet;
}
