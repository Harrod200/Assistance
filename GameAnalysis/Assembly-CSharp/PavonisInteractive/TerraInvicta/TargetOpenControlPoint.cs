using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000692 RID: 1682
	public class TargetOpenControlPoint : GameEvent
	{
		// Token: 0x060028BA RID: 10426 RVA: 0x000DA9AC File Offset: 0x000D8BAC
		public TargetOpenControlPoint(TICouncilorState councilor, TIMissionTemplate missionTemplate, IList<TIGameState> validTargets)
		{
			this.councilor = councilor;
			this.missionTemplate = missionTemplate;
			this.validTargets = validTargets;
		}

		// Token: 0x04001EF5 RID: 7925
		public TICouncilorState councilor;

		// Token: 0x04001EF6 RID: 7926
		public TIMissionTemplate missionTemplate;

		// Token: 0x04001EF7 RID: 7927
		public IList<TIGameState> validTargets;
	}
}
