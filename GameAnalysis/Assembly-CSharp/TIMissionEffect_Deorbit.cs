using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001CE RID: 462
public class TIMissionEffect_Deorbit : TIMissionEffect
{
	// Token: 0x06000682 RID: 1666 RVA: 0x0001DA38 File Offset: 0x0001BC38
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
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
