using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D0 RID: 1744
	public class ShipSecondaryExplosion : GameEvent
	{
		// Token: 0x060028FC RID: 10492 RVA: 0x000DAE56 File Offset: 0x000D9056
		public ShipSecondaryExplosion(TISpaceShipState ship, ModuleDataEntry partExploding)
		{
			this.ship = ship;
			this.partExploding = partExploding;
		}

		// Token: 0x04001F51 RID: 8017
		public TISpaceShipState ship;

		// Token: 0x04001F52 RID: 8018
		public ModuleDataEntry partExploding;
	}
}
