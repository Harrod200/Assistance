using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006CC RID: 1740
	public class ShipPartBeingRepaired : GameEvent
	{
		// Token: 0x060028F8 RID: 10488 RVA: 0x000DADFE File Offset: 0x000D8FFE
		public ShipPartBeingRepaired(TISpaceShipState ship, ModuleDataEntry partData)
		{
			this.ship = ship;
			this.partData = partData;
		}

		// Token: 0x04001F49 RID: 8009
		public TISpaceShipState ship;

		// Token: 0x04001F4A RID: 8010
		public ModuleDataEntry partData;
	}
}
