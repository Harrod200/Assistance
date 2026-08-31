using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B3 RID: 1715
	public class ShipEntersCombat : GameEvent
	{
		// Token: 0x060028DF RID: 10463 RVA: 0x000DAC01 File Offset: 0x000D8E01
		public ShipEntersCombat(TISpaceCombatState combat, TISpaceShipState ship)
		{
			this.combatState = combat;
			this.shipState = ship;
		}

		// Token: 0x04001F1D RID: 7965
		public TISpaceCombatState combatState;

		// Token: 0x04001F1E RID: 7966
		public TISpaceShipState shipState;
	}
}
