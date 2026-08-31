using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005F5 RID: 1525
	public class CouncilorMissionUpdated : GameEvent
	{
		// Token: 0x0600281A RID: 10266 RVA: 0x000D9DA9 File Offset: 0x000D7FA9
		public CouncilorMissionUpdated(TICouncilorState councilor, TIMissionState mission)
		{
			this.councilor = councilor;
			this.mission = mission;
		}

		// Token: 0x04001E17 RID: 7703
		public TICouncilorState councilor;

		// Token: 0x04001E18 RID: 7704
		public TIMissionState mission;
	}
}
