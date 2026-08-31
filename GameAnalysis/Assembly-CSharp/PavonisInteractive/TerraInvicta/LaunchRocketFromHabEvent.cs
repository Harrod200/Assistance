using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000608 RID: 1544
	public class LaunchRocketFromHabEvent : GameEvent
	{
		// Token: 0x0600282D RID: 10285 RVA: 0x000D9F29 File Offset: 0x000D8129
		public LaunchRocketFromHabEvent(TIHabState launchSite)
		{
			this.launchSite = launchSite;
		}

		// Token: 0x04001E38 RID: 7736
		public TIHabState launchSite;
	}
}
