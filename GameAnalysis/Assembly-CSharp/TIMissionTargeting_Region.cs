using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000281 RID: 641
public class TIMissionTargeting_Region : TIMissionTargeting
{
	// Token: 0x06000886 RID: 2182 RVA: 0x00027B54 File Offset: 0x00025D54
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIRegionState) };
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x00027B6C File Offset: 0x00025D6C
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new TargetRegions(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null, null, true, false);
			GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
		}
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x00027C0C File Offset: 0x00025E0C
	public override void Shutdown()
	{
		if (base.activated)
		{
			GameControl.eventManager.RemoveListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.RegionSelectedForTargeting), null);
			GameControl.eventManager.TriggerEvent(new DeTargetRegions(), null, Array.Empty<object>());
			base.SetShutdown();
			GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ResetMapColors();
		}
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x00027C71 File Offset: 0x00025E71
	private void RegionSelectedForTargeting(RegionStateSelected e)
	{
		base.SetTarget(e.region);
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x00027C80 File Offset: 0x00025E80
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
			if (this.councilor.location.isRegionState && this.possibleTargets.Contains(this.councilor.location))
			{
				return this.councilor.location;
			}
			return base.GetDefaultTarget();
		}
	}
}
