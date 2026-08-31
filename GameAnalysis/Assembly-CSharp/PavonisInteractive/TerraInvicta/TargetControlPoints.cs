using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000690 RID: 1680
	public class TargetControlPoints : GameEvent
	{
		// Token: 0x060028B8 RID: 10424 RVA: 0x000DA987 File Offset: 0x000D8B87
		public TargetControlPoints(TICouncilorState councilor, TIMissionTemplate missionTemplate, IList<TIGameState> validTargets)
		{
			this.councilor = councilor;
			this.missionTemplate = missionTemplate;
			this.validTargets = validTargets;
		}

		// Token: 0x04001EF2 RID: 7922
		public TICouncilorState councilor;

		// Token: 0x04001EF3 RID: 7923
		public TIMissionTemplate missionTemplate;

		// Token: 0x04001EF4 RID: 7924
		public IList<TIGameState> validTargets;
	}
}
