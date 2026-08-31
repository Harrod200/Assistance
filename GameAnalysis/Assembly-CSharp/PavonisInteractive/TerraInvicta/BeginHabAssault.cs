using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000642 RID: 1602
	public class BeginHabAssault : GameEvent
	{
		// Token: 0x06002867 RID: 10343 RVA: 0x000DA3E8 File Offset: 0x000D85E8
		public BeginHabAssault(TIGameState fleet, TIHabState target)
		{
			this.fleet = fleet;
			this.target = target;
		}

		// Token: 0x04001E9A RID: 7834
		public TIGameState fleet;

		// Token: 0x04001E9B RID: 7835
		public TIHabState target;
	}
}
