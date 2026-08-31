using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005FF RID: 1535
	public class CouncilorValuesChanged : GameEvent
	{
		// Token: 0x06002824 RID: 10276 RVA: 0x000D9E8D File Offset: 0x000D808D
		public CouncilorValuesChanged(TICouncilorState councilor)
		{
			this.councilor = councilor;
		}

		// Token: 0x04001E2C RID: 7724
		public TICouncilorState councilor;
	}
}
