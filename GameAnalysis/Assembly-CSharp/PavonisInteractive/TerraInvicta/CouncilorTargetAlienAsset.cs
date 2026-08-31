using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200069C RID: 1692
	public class CouncilorTargetAlienAsset : GameEvent
	{
		// Token: 0x060028C4 RID: 10436 RVA: 0x000DAA6D File Offset: 0x000D8C6D
		public CouncilorTargetAlienAsset(TICouncilorState councilor, TIMissionTemplate missionTemplate)
		{
			this.councilor = councilor;
			this.missionTemplate = missionTemplate;
		}

		// Token: 0x04001F05 RID: 7941
		public TICouncilorState councilor;

		// Token: 0x04001F06 RID: 7942
		public TIMissionTemplate missionTemplate;
	}
}
