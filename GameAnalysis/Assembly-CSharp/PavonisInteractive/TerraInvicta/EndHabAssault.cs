using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000643 RID: 1603
	public class EndHabAssault : GameEvent
	{
		// Token: 0x06002868 RID: 10344 RVA: 0x000DA3FE File Offset: 0x000D85FE
		public EndHabAssault(TIGameState fleet, TIHabState target)
		{
			this.fleet = fleet;
			this.target = target;
		}

		// Token: 0x04001E9C RID: 7836
		public TIGameState fleet;

		// Token: 0x04001E9D RID: 7837
		public TIHabState target;
	}
}
