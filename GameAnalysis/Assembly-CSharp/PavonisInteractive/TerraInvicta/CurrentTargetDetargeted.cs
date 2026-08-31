using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000675 RID: 1653
	public class CurrentTargetDetargeted : GameEvent
	{
		// Token: 0x0600289D RID: 10397 RVA: 0x000DA7E4 File Offset: 0x000D89E4
		public CurrentTargetDetargeted(TIGameState oldTarget, TIGameState newTarget = null)
		{
			this.oldTarget = oldTarget;
			this.newTarget = newTarget;
		}

		// Token: 0x04001ED5 RID: 7893
		public TIGameState oldTarget;

		// Token: 0x04001ED6 RID: 7894
		public TIGameState newTarget;
	}
}
