using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006CD RID: 1741
	public class ShipPartNoLongerBeingRepaired : GameEvent
	{
		// Token: 0x060028F9 RID: 10489 RVA: 0x000DAE14 File Offset: 0x000D9014
		public ShipPartNoLongerBeingRepaired(TISpaceShipState ship, ModuleDataEntry partData)
		{
			this.ship = ship;
			this.partData = partData;
		}

		// Token: 0x04001F4B RID: 8011
		public TISpaceShipState ship;

		// Token: 0x04001F4C RID: 8012
		public ModuleDataEntry partData;
	}
}
