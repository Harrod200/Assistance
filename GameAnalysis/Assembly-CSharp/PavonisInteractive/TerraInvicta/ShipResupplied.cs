using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200063B RID: 1595
	public class ShipResupplied : GameEvent
	{
		// Token: 0x06002860 RID: 10336 RVA: 0x000DA2E5 File Offset: 0x000D84E5
		public ShipResupplied(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001E81 RID: 7809
		public TISpaceShipState ship;
	}
}
