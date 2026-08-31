using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006CA RID: 1738
	public class ShipSystemDamageChange : GameEvent
	{
		// Token: 0x060028F6 RID: 10486 RVA: 0x000DADC4 File Offset: 0x000D8FC4
		public ShipSystemDamageChange(TISpaceShipState ship, ShipSystem system, bool systemRepaired)
		{
			this.ship = ship;
			this.system = system;
			this.systemRepaired = systemRepaired;
		}

		// Token: 0x04001F43 RID: 8003
		public TISpaceShipState ship;

		// Token: 0x04001F44 RID: 8004
		public ShipSystem system;

		// Token: 0x04001F45 RID: 8005
		public bool systemRepaired;
	}
}
