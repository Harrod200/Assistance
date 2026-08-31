using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000696 RID: 1686
	public class TargetOrgs : GameEvent
	{
		// Token: 0x060028BE RID: 10430 RVA: 0x000DA9EF File Offset: 0x000D8BEF
		public TargetOrgs(TICouncilorState councilor, TIMissionTemplate missionTemplate, IList<TIGameState> validTargets, TIOrgState starterTarget)
		{
			this.councilor = councilor;
			this.missionTemplate = missionTemplate;
			this.validTargets = validTargets;
			this.starterTarget = starterTarget;
		}

		// Token: 0x04001EFA RID: 7930
		public TICouncilorState councilor;

		// Token: 0x04001EFB RID: 7931
		public TIMissionTemplate missionTemplate;

		// Token: 0x04001EFC RID: 7932
		public IList<TIGameState> validTargets;

		// Token: 0x04001EFD RID: 7933
		public TIOrgState starterTarget;
	}
}
