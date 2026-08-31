using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000282 RID: 642
public class TIMissionTargeting_SpaceFacility : TIMissionTargeting
{
	// Token: 0x0600088C RID: 2188 RVA: 0x00027D24 File Offset: 0x00025F24
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(TIRegionSpaceFacilityState) };
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00027D3C File Offset: 0x00025F3C
	public override void Activate()
	{
		if (!base.activated)
		{
			base.SetActivation(this);
			this.possibleTargets = base.missionTemplate.target.GetValidTargets(base.missionTemplate, this.councilor);
			base.SetDefaultTarget();
			GameControl.eventManager.TriggerEvent(new CouncilorTargetSpaceFacilities(this.councilor, base.missionTemplate), null, Array.Empty<object>());
			GameControl.eventManager.AddListener<SpaceFacilityMapObjectSelected>(new EventManager.EventDelegate<SpaceFacilityMapObjectSelected>(this.SpaceFacilitySelectedForTargeting), null, null, true, false);
		}
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x00027DBB File Offset: 0x00025FBB
	public override void Shutdown()
	{
		if (base.activated)
		{
			GameControl.eventManager.TriggerEvent(new DeTargetSpaceFacilities(), null, Array.Empty<object>());
			GameControl.eventManager.RemoveListener<SpaceFacilityMapObjectSelected>(new EventManager.EventDelegate<SpaceFacilityMapObjectSelected>(this.SpaceFacilitySelectedForTargeting), null);
			base.SetShutdown();
		}
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00027DF7 File Offset: 0x00025FF7
	private void SpaceFacilitySelectedForTargeting(SpaceFacilityMapObjectSelected e)
	{
		base.SetTarget(e.regionSpaceFacility);
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x00027E08 File Offset: 0x00026008
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
			if (this.councilor.location.isRegionState)
			{
				TIRegionState ref_region = this.councilor.location.ref_region;
				if (ref_region.canLaunch && this.possibleTargets.Contains(ref_region.GetRegionSpaceFacility(SpaceFacilityType.launchFacility)))
				{
					return ref_region.GetRegionSpaceFacility(SpaceFacilityType.launchFacility);
				}
				if (ref_region.missionControl > 0 && this.possibleTargets.Contains(ref_region.GetRegionSpaceFacility(SpaceFacilityType.missionControlFacility)))
				{
					return ref_region.GetRegionSpaceFacility(SpaceFacilityType.missionControlFacility);
				}
				if (ref_region.antiSpaceDefenses && this.possibleTargets.Contains(ref_region.GetRegionSpaceFacility(SpaceFacilityType.spaceDefenseFacility)))
				{
					return ref_region.GetRegionSpaceFacility(SpaceFacilityType.spaceDefenseFacility);
				}
			}
			return base.GetDefaultTarget();
		}
	}
}
