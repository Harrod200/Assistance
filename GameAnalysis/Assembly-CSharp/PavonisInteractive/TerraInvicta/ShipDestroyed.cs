using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D3 RID: 1747
	public class ShipDestroyed : GameEvent
	{
		// Token: 0x060028FF RID: 10495 RVA: 0x000DAE98 File Offset: 0x000D9098
		public ShipDestroyed(TISpaceShipState ship, TIGameState killer = null, TIShipWeaponTemplate killerWeapon = null, TIDateTime timeOfDeath = null)
		{
			this.ship = ship;
			this.killer = killer;
			this.killerWeapon = killerWeapon;
			this.timeOfDeath = timeOfDeath;
		}

		// Token: 0x04001F57 RID: 8023
		public TISpaceShipState ship;

		// Token: 0x04001F58 RID: 8024
		public TIGameState killer;

		// Token: 0x04001F59 RID: 8025
		public TIShipWeaponTemplate killerWeapon;

		// Token: 0x04001F5A RID: 8026
		public TIDateTime timeOfDeath;
	}
}
