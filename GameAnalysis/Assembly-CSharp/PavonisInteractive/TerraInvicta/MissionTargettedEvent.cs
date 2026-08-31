using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000678 RID: 1656
	public class MissionTargettedEvent : GameEvent
	{
		// Token: 0x060028A0 RID: 10400 RVA: 0x000DA81F File Offset: 0x000D8A1F
		public MissionTargettedEvent(TIGameState target, TICouncilorState councilor, TIMissionTemplate mission)
		{
			this.target = target;
			this.councilor = councilor;
			this.mission = mission;
		}

		// Token: 0x04001EDA RID: 7898
		public TIGameState target;

		// Token: 0x04001EDB RID: 7899
		public TIMissionTemplate mission;

		// Token: 0x04001EDC RID: 7900
		public TICouncilorState councilor;
	}
}
