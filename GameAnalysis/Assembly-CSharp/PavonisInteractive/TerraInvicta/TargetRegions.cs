using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006A3 RID: 1699
	public class TargetRegions : GameEvent
	{
		// Token: 0x060028CB RID: 10443 RVA: 0x000DAAC8 File Offset: 0x000D8CC8
		public TargetRegions(TICouncilorState councilor, TIMissionTemplate missionTemplate)
		{
			this.councilor = councilor;
			this.missionTemplate = missionTemplate;
		}

		// Token: 0x04001F0A RID: 7946
		public TICouncilorState councilor;

		// Token: 0x04001F0B RID: 7947
		public TIMissionTemplate missionTemplate;
	}
}
