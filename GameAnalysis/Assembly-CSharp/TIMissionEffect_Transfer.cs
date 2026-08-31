using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001CF RID: 463
public class TIMissionEffect_Transfer : TIMissionEffect
{
	// Token: 0x06000684 RID: 1668 RVA: 0x0001DB10 File Offset: 0x0001BD10
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
