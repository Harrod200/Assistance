using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B8 RID: 1720
	public class ShipPrimaryTargetSelected : GameEvent
	{
		// Token: 0x060028E4 RID: 10468 RVA: 0x000DAC61 File Offset: 0x000D8E61
		public ShipPrimaryTargetSelected(TISpaceShipState ship, CombatTargetableState target)
		{
			this.ship = ship;
			this.target = target;
		}

		// Token: 0x04001F25 RID: 7973
		public TISpaceShipState ship;

		// Token: 0x04001F26 RID: 7974
		public CombatTargetableState target;
	}
}
