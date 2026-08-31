using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000645 RID: 1605
	public class InitiateRetractRadiatorsEvent : GameEvent
	{
		// Token: 0x0600286A RID: 10346 RVA: 0x000DA423 File Offset: 0x000D8623
		public InitiateRetractRadiatorsEvent(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001E9F RID: 7839
		public TISpaceShipState ship;
	}
}
