using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006BA RID: 1722
	public class ShipManeuverTargetSelected : GameEvent
	{
		// Token: 0x060028E6 RID: 10470 RVA: 0x000DAC86 File Offset: 0x000D8E86
		public ShipManeuverTargetSelected(TISpaceShipState ship, CombatTargetableState target)
		{
			this.ship = ship;
			this.target = target;
		}

		// Token: 0x04001F28 RID: 7976
		public TISpaceShipState ship;

		// Token: 0x04001F29 RID: 7977
		public CombatTargetableState target;
	}
}
