using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200027F RID: 639
public class TIMissionTargeting_ControlPoint : TIMissionTargeting
{
	// Token: 0x06000879 RID: 2169 RVA: 0x000276E4 File Offset: 0x000258E4
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIControlPoint) };
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x000276FC File Offset: 0x000258FC
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new TargetControlPoints(this.councilor, base.missionTemplate, this.possibleTargets), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<ControlPointTargetSelected>(new EventManager.EventDelegate<ControlPointTargetSelected>(this.ControlPointSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<NationStateSelected>(new EventManager.EventDelegate<NationStateSelected>(this.ControlPointNationSelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x000277A0 File Offset: 0x000259A0
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetControlPoints(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<ControlPointTargetSelected>(new EventManager.EventDelegate<ControlPointTargetSelected>(this.ControlPointSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<NationStateSelected>(new EventManager.EventDelegate<NationStateSelected>(this.ControlPointNationSelectedForTargeting), null);
		}
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x000277FE File Offset: 0x000259FE
	private void ControlPointSelectedForTargeting(ControlPointTargetSelected e)
	{
		base.SetTarget(e.controlPoint);
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0002780C File Offset: 0x00025A0C
	private void ControlPointNationSelectedForTargeting(NationStateSelected e)
	{
		TIGameState currentTarget = this.currentTarget;
		if (((currentTarget != null) ? currentTarget.ref_nation : null) != e.nation)
		{
			TIControlPoint ticontrolPoint = e.nation.LowestOtherFactionControlPoint(this.councilor.faction);
			if (ticontrolPoint != null && this.possibleTargets.Contains(ticontrolPoint))
			{
				base.SetTarget(ticontrolPoint);
			}
		}
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x00027870 File Offset: 0x00025A70
	public override TIGameState GetDefaultTarget()
	{
		TIMissionState activeMission = this.councilor.activeMission;
		if (activeMission != null && activeMission.missionTemplate.targetingMethodType == typeof(TIMissionTargeting_ControlPoint))
		{
			TIGameState target = activeMission.target;
			if (target != null && target.isControlPointState)
			{
				return activeMission.target;
			}
		}
		TINationState currentNation = this.councilor.currentNation;
		TIGameState tigameState = ((currentNation != null) ? currentNation.LowestOtherFactionControlPoint(this.councilor.faction) : null);
		if (tigameState != null && this.possibleTargets.Contains(tigameState))
		{
			return tigameState;
		}
		tigameState = GeneralControlsController.UIOtherSelectedState;
		if (tigameState != null && tigameState.ref_nation != null)
		{
			tigameState = tigameState.ref_nation.LowestOtherFactionControlPoint(this.councilor.faction);
			if (tigameState != null && this.possibleTargets.Contains(tigameState))
			{
				return tigameState;
			}
		}
		return base.GetDefaultTarget();
	}
}
