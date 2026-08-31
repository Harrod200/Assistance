using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000660 RID: 1632
	public class GameStateNameChanged : GameEvent
	{
		// Token: 0x06002887 RID: 10375 RVA: 0x000DA5EE File Offset: 0x000D87EE
		public GameStateNameChanged(TIGameState state)
		{
			this.state = state;
		}

		// Token: 0x04001EBE RID: 7870
		public TIGameState state;
	}
}
