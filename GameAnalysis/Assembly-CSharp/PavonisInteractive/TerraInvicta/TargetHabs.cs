using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000683 RID: 1667
	public class TargetHabs : GameEvent
	{
		// Token: 0x060028AB RID: 10411 RVA: 0x000DA8E0 File Offset: 0x000D8AE0
		public TargetHabs(TIGameState targetingState)
		{
			this.targetingState = targetingState;
		}

		// Token: 0x04001EE9 RID: 7913
		public TIGameState targetingState;
	}
}
