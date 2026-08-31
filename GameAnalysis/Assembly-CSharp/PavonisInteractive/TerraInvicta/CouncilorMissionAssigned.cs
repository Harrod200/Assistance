using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005F6 RID: 1526
	public class CouncilorMissionAssigned : GameEvent
	{
		// Token: 0x0600281B RID: 10267 RVA: 0x000D9DBF File Offset: 0x000D7FBF
		public CouncilorMissionAssigned(TICouncilorState councilor, TIMissionState mission)
		{
			this.councilor = councilor;
			this.mission = mission;
		}

		// Token: 0x04001E19 RID: 7705
		public TICouncilorState councilor;

		// Token: 0x04001E1A RID: 7706
		public TIMissionState mission;
	}
}
