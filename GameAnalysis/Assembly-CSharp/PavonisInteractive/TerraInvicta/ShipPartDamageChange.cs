using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006CB RID: 1739
	public class ShipPartDamageChange : GameEvent
	{
		// Token: 0x060028F7 RID: 10487 RVA: 0x000DADE1 File Offset: 0x000D8FE1
		public ShipPartDamageChange(TISpaceShipState ship, ModuleDataEntry partData, bool partRepaired)
		{
			this.ship = ship;
			this.partData = partData;
			this.partRepaired = partRepaired;
		}

		// Token: 0x04001F46 RID: 8006
		public TISpaceShipState ship;

		// Token: 0x04001F47 RID: 8007
		public ModuleDataEntry partData;

		// Token: 0x04001F48 RID: 8008
		public bool partRepaired;
	}
}
