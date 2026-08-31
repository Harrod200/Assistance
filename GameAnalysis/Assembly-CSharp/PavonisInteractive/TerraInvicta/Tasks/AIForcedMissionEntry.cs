using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000933 RID: 2355
	public class AIForcedMissionEntry
	{
		// Token: 0x06005A16 RID: 23062 RVA: 0x0029DBF3 File Offset: 0x0029BDF3
		public AIForcedMissionEntry()
		{
		}

		// Token: 0x06005A17 RID: 23063 RVA: 0x0029DBFB File Offset: 0x0029BDFB
		public AIForcedMissionEntry(TICouncilorState councilor, TIGameState target, TIMissionTemplate mission)
		{
			this.councilor = councilor;
			this.target = target;
			this.mission = mission;
		}

		// Token: 0x04004115 RID: 16661
		public TICouncilorState councilor;

		// Token: 0x04004116 RID: 16662
		public TIGameState target;

		// Token: 0x04004117 RID: 16663
		public TIMissionTemplate mission;
	}
}
