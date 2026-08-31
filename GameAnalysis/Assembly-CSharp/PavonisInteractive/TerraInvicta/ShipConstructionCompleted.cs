using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000638 RID: 1592
	public class ShipConstructionCompleted : GameEvent
	{
		// Token: 0x0600285D RID: 10333 RVA: 0x000DA2B1 File Offset: 0x000D84B1
		public ShipConstructionCompleted(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001E7D RID: 7805
		public TISpaceShipState ship;
	}
}
