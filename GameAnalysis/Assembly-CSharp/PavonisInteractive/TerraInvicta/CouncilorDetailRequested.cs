using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005D3 RID: 1491
	public class CouncilorDetailRequested : GameEvent
	{
		// Token: 0x060027F8 RID: 10232 RVA: 0x000D9B95 File Offset: 0x000D7D95
		public CouncilorDetailRequested(TICouncilorState councilor)
		{
			this.councilor = councilor;
		}

		// Token: 0x04001DF2 RID: 7666
		public TICouncilorState councilor;
	}
}
