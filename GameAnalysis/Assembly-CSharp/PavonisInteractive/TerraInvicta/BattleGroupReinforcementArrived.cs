using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B5 RID: 1717
	public class BattleGroupReinforcementArrived : GameEvent
	{
		// Token: 0x060028E1 RID: 10465 RVA: 0x000DAC2D File Offset: 0x000D8E2D
		public BattleGroupReinforcementArrived(int numReinforcements, TISpaceShipState ship)
		{
			this.battleGroupSize = numReinforcements;
			this.shipState = ship;
		}

		// Token: 0x04001F21 RID: 7969
		public int battleGroupSize;

		// Token: 0x04001F22 RID: 7970
		public TISpaceShipState shipState;
	}
}
