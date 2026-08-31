using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000694 RID: 1684
	public class TargetCouncilors : GameEvent
	{
		// Token: 0x060028BC RID: 10428 RVA: 0x000DA9D1 File Offset: 0x000D8BD1
		public TargetCouncilors(TICouncilorState councilor, TIMissionTemplate missionTemplate)
		{
			this.councilor = councilor;
			this.missionTemplate = missionTemplate;
		}

		// Token: 0x04001EF8 RID: 7928
		public TICouncilorState councilor;

		// Token: 0x04001EF9 RID: 7929
		public TIMissionTemplate missionTemplate;
	}
}
