using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005F9 RID: 1529
	public class CouncilorVisibilityChanged : GameEvent
	{
		// Token: 0x0600281E RID: 10270 RVA: 0x000D9DFA File Offset: 0x000D7FFA
		public CouncilorVisibilityChanged(TICouncilorState councilor, TIFactionState viewingFaction)
		{
			this.councilor = councilor;
			this.viewingFaction = viewingFaction;
		}

		// Token: 0x04001E1E RID: 7710
		public TICouncilorState councilor;

		// Token: 0x04001E1F RID: 7711
		public TIFactionState viewingFaction;
	}
}
