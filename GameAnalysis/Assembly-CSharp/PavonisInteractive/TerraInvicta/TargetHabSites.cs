using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000680 RID: 1664
	public class TargetHabSites : GameEvent
	{
		// Token: 0x060028A8 RID: 10408 RVA: 0x000DA8AC File Offset: 0x000D8AAC
		public TargetHabSites(TIGameState targetingState, TISpaceBodyState spaceBody = null)
		{
			this.targetingState = targetingState;
			this.spaceBody = spaceBody;
		}

		// Token: 0x04001EE5 RID: 7909
		public TIGameState targetingState;

		// Token: 0x04001EE6 RID: 7910
		public TISpaceBodyState spaceBody;
	}
}
