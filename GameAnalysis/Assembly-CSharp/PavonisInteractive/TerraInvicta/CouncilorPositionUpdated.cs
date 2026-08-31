using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005FD RID: 1533
	public class CouncilorPositionUpdated : GameEvent
	{
		// Token: 0x06002822 RID: 10274 RVA: 0x000D9E52 File Offset: 0x000D8052
		public CouncilorPositionUpdated(TICouncilorState councilor, TIGameState location)
		{
			this.councilor = councilor;
			this.location = location;
		}

		// Token: 0x04001E26 RID: 7718
		public TICouncilorState councilor;

		// Token: 0x04001E27 RID: 7719
		public TIGameState location;
	}
}
