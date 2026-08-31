using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006DF RID: 1759
	public class CombatManeuverComplete : GameEvent
	{
		// Token: 0x0600290B RID: 10507 RVA: 0x000DAFA4 File Offset: 0x000D91A4
		public CombatManeuverComplete(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F6F RID: 8047
		public TISpaceShipState ship;
	}
}
