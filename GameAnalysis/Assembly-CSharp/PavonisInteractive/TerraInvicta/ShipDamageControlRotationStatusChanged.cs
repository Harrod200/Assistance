using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006C9 RID: 1737
	public class ShipDamageControlRotationStatusChanged : GameEvent
	{
		// Token: 0x060028F5 RID: 10485 RVA: 0x000DADAE File Offset: 0x000D8FAE
		public ShipDamageControlRotationStatusChanged(TISpaceShipState ship, bool isDamageControlEnabled)
		{
			this.ship = ship;
			this.damageControlEnabled = isDamageControlEnabled;
		}

		// Token: 0x04001F41 RID: 8001
		public TISpaceShipState ship;

		// Token: 0x04001F42 RID: 8002
		public bool damageControlEnabled;
	}
}
