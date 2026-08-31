using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005FC RID: 1532
	public class CouncilorDepartsShip : GameEvent
	{
		// Token: 0x06002821 RID: 10273 RVA: 0x000D9E3C File Offset: 0x000D803C
		public CouncilorDepartsShip(TICouncilorState councilor, TISpaceShipState ship)
		{
			this.councilor = councilor;
			this.ship = ship;
		}

		// Token: 0x04001E24 RID: 7716
		public TICouncilorState councilor;

		// Token: 0x04001E25 RID: 7717
		public TISpaceShipState ship;
	}
}
