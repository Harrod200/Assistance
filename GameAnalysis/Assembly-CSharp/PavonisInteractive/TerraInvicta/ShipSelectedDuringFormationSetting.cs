using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006BD RID: 1725
	public class ShipSelectedDuringFormationSetting : GameEvent
	{
		// Token: 0x060028E9 RID: 10473 RVA: 0x000DACC1 File Offset: 0x000D8EC1
		public ShipSelectedDuringFormationSetting(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F2D RID: 7981
		public TISpaceShipState ship;
	}
}
