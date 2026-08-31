using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D2 RID: 1746
	public class ShipRadiatorDestroyed : GameEvent
	{
		// Token: 0x060028FE RID: 10494 RVA: 0x000DAE82 File Offset: 0x000D9082
		public ShipRadiatorDestroyed(TISpaceShipState ship, ModuleDataEntry radiatorModule)
		{
			this.ship = ship;
			this.radiatorModule = radiatorModule;
		}

		// Token: 0x04001F55 RID: 8021
		public TISpaceShipState ship;

		// Token: 0x04001F56 RID: 8022
		public ModuleDataEntry radiatorModule;
	}
}
