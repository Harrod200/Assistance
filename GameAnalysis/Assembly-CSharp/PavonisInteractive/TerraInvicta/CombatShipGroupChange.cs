using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006E1 RID: 1761
	public class CombatShipGroupChange : GameEvent
	{
		// Token: 0x0600290D RID: 10509 RVA: 0x000DAFC2 File Offset: 0x000D91C2
		public CombatShipGroupChange(TISpaceShipState ship, int group)
		{
			this.ship = ship;
			this.group = group;
		}

		// Token: 0x04001F71 RID: 8049
		public TISpaceShipState ship;

		// Token: 0x04001F72 RID: 8050
		public int group;
	}
}
