using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006C8 RID: 1736
	public class CombatShipPropulsionValuesUpdated : GameEvent
	{
		// Token: 0x060028F4 RID: 10484 RVA: 0x000DAD9F File Offset: 0x000D8F9F
		public CombatShipPropulsionValuesUpdated(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F40 RID: 8000
		public TISpaceShipState ship;
	}
}
