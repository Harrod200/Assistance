using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005D6 RID: 1494
	public class ShipDetailRequested : GameEvent
	{
		// Token: 0x060027FB RID: 10235 RVA: 0x000D9BC9 File Offset: 0x000D7DC9
		public ShipDetailRequested(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001DF6 RID: 7670
		public TISpaceShipState ship;
	}
}
