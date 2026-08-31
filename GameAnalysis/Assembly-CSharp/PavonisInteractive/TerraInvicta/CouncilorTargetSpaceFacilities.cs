using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000699 RID: 1689
	public class CouncilorTargetSpaceFacilities : GameEvent
	{
		// Token: 0x060028C1 RID: 10433 RVA: 0x000DAA2B File Offset: 0x000D8C2B
		public CouncilorTargetSpaceFacilities(TICouncilorState councilor, TIMissionTemplate missionTemplate)
		{
			this.councilor = councilor;
			this.missionTemplate = missionTemplate;
		}

		// Token: 0x04001EFF RID: 7935
		public TICouncilorState councilor;

		// Token: 0x04001F00 RID: 7936
		public TIMissionTemplate missionTemplate;
	}
}
