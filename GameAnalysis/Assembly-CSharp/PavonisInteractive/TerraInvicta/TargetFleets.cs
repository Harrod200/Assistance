using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200067D RID: 1661
	public class TargetFleets : GameEvent
	{
		// Token: 0x060028A5 RID: 10405 RVA: 0x000DA886 File Offset: 0x000D8A86
		public TargetFleets(TIGameState targetingState)
		{
			this.targetingState = targetingState;
		}

		// Token: 0x04001EE3 RID: 7907
		public TIGameState targetingState;
	}
}
