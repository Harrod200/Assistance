using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006DB RID: 1755
	public class ShipCommandExecuted : GameEvent
	{
		// Token: 0x06002907 RID: 10503 RVA: 0x000DAF53 File Offset: 0x000D9153
		public ShipCommandExecuted(TISpaceShipState ship, TIShipCommandTemplate command)
		{
			this.ship = ship;
			this.command = command;
		}

		// Token: 0x04001F68 RID: 8040
		public TISpaceShipState ship;

		// Token: 0x04001F69 RID: 8041
		public TIShipCommandTemplate command;
	}
}
