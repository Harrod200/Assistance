using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000286 RID: 646
public class TIMissionTargeting_RegionFleetHab : TIMissionTargeting
{
	// Token: 0x060008A6 RID: 2214 RVA: 0x0002858F File Offset: 0x0002678F
	public override List<Type> TargetedGameStates()
	{
		return new List<Type>
		{
			typeof(TIRegionState),
			typeof(TISpaceFleetState),
			typeof(TIHabState)
		};
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x000285C8 File Offset: 0x000267C8
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new TargetRegions(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null, null, true, false);
			GameControl.eventManager.AddListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null, null, true, false);
			GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
		}
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0002869C File Offset: 0x0002689C
	private void RegionSelectedForTargeting(RegionStateSelected e)
	{
		base.SetTarget(e.region);
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x000286AA File Offset: 0x000268AA
	private void HabSelectedForTargeting(HabSelectedEvent e)
	{
		base.SetTarget(e.hab);
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x000286B8 File Offset: 0x000268B8
	private void FleetSelectedForTargeting(FleetSelectedEvent e)
	{
		base.SetTarget(e.fleet);
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x000286C8 File Offset: 0x000268C8
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
			if (this.possibleTargets.Contains(this.councilor.location))
			{
				return this.councilor.location;
			}
			TIRegionState ref_region = this.councilor.location.ref_region;
			if (ref_region != null && this.possibleTargets.Contains(ref_region))
			{
				return ref_region;
			}
			TISpaceFleetState ref_fleet = this.councilor.location.ref_fleet;
			if (ref_fleet != null && this.possibleTargets.Contains(ref_fleet))
			{
				return ref_fleet;
			}
			TIHabState ref_hab = this.councilor.location.ref_hab;
			if (ref_hab != null && this.possibleTargets.Contains(ref_hab))
			{
				return ref_hab;
			}
			return base.GetDefaultTarget();
		}
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x000287D0 File Offset: 0x000269D0
	public override void Shutdown()
	{
		if (base.activated)
		{
			base.SetShutdown();
			GameControl.eventManager.TriggerEvent(new DeTargetRegions(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<HabSelectedEvent>(new EventManager.EventDelegate<HabSelectedEvent>(this.HabSelectedForTargeting), null);
			GameControl.eventManager.RemoveListener<FleetSelectedEvent>(new EventManager.EventDelegate<FleetSelectedEvent>(this.FleetSelectedForTargeting), null);
			GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
		}
	}
}
