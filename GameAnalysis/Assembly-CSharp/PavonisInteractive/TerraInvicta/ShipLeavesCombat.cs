using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B4 RID: 1716
	public class ShipLeavesCombat : GameEvent
	{
		// Token: 0x060028E0 RID: 10464 RVA: 0x000DAC17 File Offset: 0x000D8E17
		public ShipLeavesCombat(TISpaceCombatState combat, TISpaceShipState ship)
		{
			this.combatState = combat;
			this.shipState = ship;
		}

		// Token: 0x04001F1F RID: 7967
		public TISpaceCombatState combatState;

		// Token: 0x04001F20 RID: 7968
		public TISpaceShipState shipState;
	}
}
