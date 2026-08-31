using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006DE RID: 1758
	public class CombatCollisionAvoidanceStatusChange : GameEvent
	{
		// Token: 0x0600290A RID: 10506 RVA: 0x000DAF95 File Offset: 0x000D9195
		public CombatCollisionAvoidanceStatusChange(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F6E RID: 8046
		public TISpaceShipState ship;
	}
}
