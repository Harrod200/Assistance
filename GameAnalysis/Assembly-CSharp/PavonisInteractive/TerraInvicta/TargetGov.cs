using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200068D RID: 1677
	public class TargetGov : GameEvent
	{
		// Token: 0x060028B5 RID: 10421 RVA: 0x000DA953 File Offset: 0x000D8B53
		public TargetGov(TICouncilorState councilor, TIMissionTemplate missionTemplate)
		{
			this.councilor = councilor;
			this.missionTemplate = missionTemplate;
		}

		// Token: 0x04001EEE RID: 7918
		public TIMissionTemplate missionTemplate;

		// Token: 0x04001EEF RID: 7919
		public TICouncilorState councilor;
	}
}
