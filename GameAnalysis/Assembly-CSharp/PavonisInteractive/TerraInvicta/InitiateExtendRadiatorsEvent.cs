using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000644 RID: 1604
	public class InitiateExtendRadiatorsEvent : GameEvent
	{
		// Token: 0x06002869 RID: 10345 RVA: 0x000DA414 File Offset: 0x000D8614
		public InitiateExtendRadiatorsEvent(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001E9E RID: 7838
		public TISpaceShipState ship;
	}
}
