using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000647 RID: 1607
	public class CompleteRetractRadiatorsEvent : GameEvent
	{
		// Token: 0x0600286C RID: 10348 RVA: 0x000DA441 File Offset: 0x000D8641
		public CompleteRetractRadiatorsEvent(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001EA1 RID: 7841
		public TISpaceShipState ship;
	}
}
