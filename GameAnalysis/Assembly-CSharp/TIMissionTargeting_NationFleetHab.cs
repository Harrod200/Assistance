using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000285 RID: 645
public class TIMissionTargeting_NationFleetHab : TIMissionTargeting
{
	// Token: 0x0600089E RID: 2206 RVA: 0x0002828C File Offset: 0x0002648C
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TINationState),
			typeof(TISpaceFleetState),
			typeof(TIHabState)
		};
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x000282C4 File Offset: 0x000264C4
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new TargetGov(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<NationStateSelected>(new EventManager.EventDelegate<NationStateSelected>(this.NationSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null, null, true, false);
			if (this.councilor.OnEarth)
			{
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
			}
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x000283A5 File Offset: 0x000265A5
	private void NationSelectedForTargeting(NationStateSelected e)
	{
		base.SetTarget(e.nation);
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x000283B3 File Offset: 0x000265B3
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.SetTarget(e.hab);
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x000283C1 File Offset: 0x000265C1
	private void FleetSelectedForTargeting(FleetSelectedEvent e)
	{
		base.SetTarget(e.fleet);
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x000283D0 File Offset: 0x000265D0
	public override TIGameState GetDefaultTarget()
	{
		ICollection<TIGameState> possibleTargets = this.possibleTargets;
		TIMissionState activeMission = this.councilor.activeMission;
		if (possibleTargets.Contains((activeMission != null) ? activeMission.target : null))
		{
			TIMissionState activeMission2 = this.councilor.activeMission;
			if (activeMission2 == null)
			{
				return null;
			}
			return activeMission2.target;
		}
		else
		{
			if (this.possibleTargets.Contains(GeneralControlsController.UIOtherSelectedState))
			{
				return GeneralControlsController.UIOtherSelectedState;
			}
			ICollection<TIGameState> possibleTargets2 = this.possibleTargets;
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			if (possibleTargets2.Contains((uiotherSelectedState != null) ? uiotherSelectedState.ref_nation : null))
			{
				return GeneralControlsController.UIOtherSelectedState.ref_nation;
			}
			TINationState currentNation = this.councilor.currentNation;
			if (currentNation != null && this.possibleTargets.Contains(currentNation))
			{
				return currentNation;
			}
			TISpaceShipState tispaceShipState = this.councilor.location as TISpaceShipState;
			TISpaceFleetState tispaceFleetState = ((tispaceShipState != null) ? tispaceShipState.fleet : null);
			if (tispaceFleetState != null && this.possibleTargets.Contains(tispaceFleetState))
			{
				return tispaceFleetState;
			}
			TIHabState tihabState = this.councilor.location as TIHabState;
			if (tihabState != null && this.possibleTargets.Contains(tihabState))
			{
				return tihabState;
			}
			return base.GetDefaultTarget();
		}
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x000284E4 File Offset: 0x000266E4
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetGov(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<NationStateSelected>(new EventManager.EventDelegate<NationStateSelected>(this.NationSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null);
			if (this.councilor.OnEarth)
			{
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
			}
		}
	}
}
