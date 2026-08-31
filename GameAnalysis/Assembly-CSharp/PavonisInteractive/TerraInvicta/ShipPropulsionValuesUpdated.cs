using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006C7 RID: 1735
	public class ShipPropulsionValuesUpdated : GameEvent
	{
		// Token: 0x060028F3 RID: 10483 RVA: 0x000DAD90 File Offset: 0x000D8F90
		public ShipPropulsionValuesUpdated(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F3F RID: 7999
		public TISpaceShipState ship;
	}
}
