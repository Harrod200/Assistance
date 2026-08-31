using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000646 RID: 1606
	public class CompleteExtendRadiatorsEvent : GameEvent
	{
		// Token: 0x0600286B RID: 10347 RVA: 0x000DA432 File Offset: 0x000D8632
		public CompleteExtendRadiatorsEvent(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001EA0 RID: 7840
		public TISpaceShipState ship;
	}
}
