using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005C1 RID: 1473
	public class GameStateArchived : GameEvent
	{
		// Token: 0x060027E6 RID: 10214 RVA: 0x000D9A64 File Offset: 0x000D7C64
		public GameStateArchived(TIGameState gameState)
		{
			this.gameState = gameState;
		}

		// Token: 0x04001DDB RID: 7643
		public TIGameState gameState;
	}
}
