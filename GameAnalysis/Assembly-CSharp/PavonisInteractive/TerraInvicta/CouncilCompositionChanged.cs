using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005FE RID: 1534
	public class CouncilCompositionChanged : GameEvent
	{
		// Token: 0x06002823 RID: 10275 RVA: 0x000D9E68 File Offset: 0x000D8068
		public CouncilCompositionChanged(TIFactionState council, TICouncilorState councilor, TIGameState location, bool joining)
		{
			this.council = council;
			this.councilor = councilor;
			this.joining = joining;
			this.location = location;
		}

		// Token: 0x04001E28 RID: 7720
		public TIFactionState council;

		// Token: 0x04001E29 RID: 7721
		public TIGameState location;

		// Token: 0x04001E2A RID: 7722
		public TICouncilorState councilor;

		// Token: 0x04001E2B RID: 7723
		public bool joining;
	}
}
