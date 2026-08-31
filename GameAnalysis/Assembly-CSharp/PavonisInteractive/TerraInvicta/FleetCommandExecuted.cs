using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006DC RID: 1756
	public class FleetCommandExecuted : GameEvent
	{
		// Token: 0x06002908 RID: 10504 RVA: 0x000DAF69 File Offset: 0x000D9169
		public FleetCommandExecuted(List<TISpaceShipState> ships, TIFleetCommandTemplate fleetCommand)
		{
			this.ships = ships;
			this.fleetCommand = fleetCommand;
		}

		// Token: 0x04001F6A RID: 8042
		public List<TISpaceShipState> ships;

		// Token: 0x04001F6B RID: 8043
		public TIFleetCommandTemplate fleetCommand;
	}
}
