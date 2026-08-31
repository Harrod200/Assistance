using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B7 RID: 1719
	public class ShipRetreatsFromCombat : GameEvent
	{
		// Token: 0x060028E3 RID: 10467 RVA: 0x000DAC52 File Offset: 0x000D8E52
		public ShipRetreatsFromCombat(TISpaceShipState ship)
		{
			this.shipState = ship;
		}

		// Token: 0x04001F24 RID: 7972
		public TISpaceShipState shipState;
	}
}
