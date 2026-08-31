using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200069D RID: 1693
	public class CouncilorTargetAlienActivity : GameEvent
	{
		// Token: 0x060028C5 RID: 10437 RVA: 0x000DAA83 File Offset: 0x000D8C83
		public CouncilorTargetAlienActivity(TICouncilorState councilor, TIMissionTemplate missionTemplate)
		{
			this.councilor = councilor;
			this.missionTemplate = missionTemplate;
		}

		// Token: 0x04001F07 RID: 7943
		public TICouncilorState councilor;

		// Token: 0x04001F08 RID: 7944
		public TIMissionTemplate missionTemplate;
	}
}
