using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001CD RID: 461
public class TIMissionEffect_Orbit : TIMissionEffect
{
	// Token: 0x06000680 RID: 1664 RVA: 0x0001D8F0 File Offset: 0x0001BAF0
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		if (mission.councilor.location.ref_region != null)
		{
			TIRegionSpaceFacilityState regionSpaceFacility = mission.councilor.ref_region.GetRegionSpaceFacility(SpaceFacilityType.launchFacility);
			GameControl.eventManager.TriggerEvent(new LaunchRocketEvent(regionSpaceFacility), null, new object[] { regionSpaceFacility });
		}
		else
		{
			TIHabState ref_hab = mission.councilor.ref_hab;
			GameControl.eventManager.TriggerEvent(new LaunchRocketFromHabEvent(ref_hab), null, new object[] { ref_hab });
		}
		mission.councilor.EnterTransit();
		mission.councilor.RemoveFromCurrentLocation();
		mission.councilor.SetLocation(target);
		mission.councilor.ExitTransit();
		GameControl.eventManager.TriggerEvent(new CouncilorPositionUpdated(mission.councilor, target), null, (from x in new object[]
			{
				mission.councilor,
				mission.councilor.faction,
				target,
				mission.councilor.location,
				mission.councilor.ref_fleet,
				mission.councilor.ref_naturalSpaceObject
			}.Distinct<object>()
			where x != null
			select x).ToArray<object>());
		return string.Empty;
	}
}
