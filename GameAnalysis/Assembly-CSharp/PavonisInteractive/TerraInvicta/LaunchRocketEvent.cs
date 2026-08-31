using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000607 RID: 1543
	public class LaunchRocketEvent : GameEvent
	{
		// Token: 0x0600282C RID: 10284 RVA: 0x000D9F1A File Offset: 0x000D811A
		public LaunchRocketEvent(TIRegionSpaceFacilityState launchSite)
		{
			this.launchSite = launchSite;
		}

		// Token: 0x04001E37 RID: 7735
		public TIRegionSpaceFacilityState launchSite;
	}
}
