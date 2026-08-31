using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B6 RID: 1718
	public class ReinforcementArrived : GameEvent
	{
		// Token: 0x060028E2 RID: 10466 RVA: 0x000DAC43 File Offset: 0x000D8E43
		public ReinforcementArrived(TISpaceShipState ship)
		{
			this.shipState = ship;
		}

		// Token: 0x04001F23 RID: 7971
		public TISpaceShipState shipState;
	}
}
