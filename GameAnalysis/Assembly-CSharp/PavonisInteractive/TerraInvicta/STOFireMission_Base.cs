using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200063E RID: 1598
	public class STOFireMission_Base : GameEvent
	{
		// Token: 0x06002863 RID: 10339 RVA: 0x000DA36C File Offset: 0x000D856C
		public STOFireMission_Base(TIHabModuleState shooter, TISpaceShipState target, TIDateTime time)
		{
			this.shooter = shooter;
			this.target = target;
			this.time = time;
		}

		// Token: 0x04001E8D RID: 7821
		public TIHabModuleState shooter;

		// Token: 0x04001E8E RID: 7822
		public TISpaceShipState target;

		// Token: 0x04001E8F RID: 7823
		public TIDateTime time;
	}
}
