using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000668 RID: 1640
	public class ShipSelectedEvent : GameEvent
	{
		// Token: 0x0600288F RID: 10383 RVA: 0x000DA666 File Offset: 0x000D8866
		public ShipSelectedEvent(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001EC6 RID: 7878
		public TISpaceShipState ship;
	}
}
