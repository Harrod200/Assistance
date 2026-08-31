using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000615 RID: 1557
	public class TIGameStateAttacking : GameEvent
	{
		// Token: 0x0600283A RID: 10298 RVA: 0x000DA018 File Offset: 0x000D8218
		public TIGameStateAttacking(TIGameState gameState)
		{
			this.gameState = gameState;
		}

		// Token: 0x04001E46 RID: 7750
		public TIGameState gameState;
	}
}
