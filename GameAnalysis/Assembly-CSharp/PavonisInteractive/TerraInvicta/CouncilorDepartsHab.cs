using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005FB RID: 1531
	public class CouncilorDepartsHab : GameEvent
	{
		// Token: 0x06002820 RID: 10272 RVA: 0x000D9E26 File Offset: 0x000D8026
		public CouncilorDepartsHab(TICouncilorState councilor, TIHabState hab)
		{
			this.councilor = councilor;
			this.hab = hab;
		}

		// Token: 0x04001E22 RID: 7714
		public TICouncilorState councilor;

		// Token: 0x04001E23 RID: 7715
		public TIHabState hab;
	}
}
