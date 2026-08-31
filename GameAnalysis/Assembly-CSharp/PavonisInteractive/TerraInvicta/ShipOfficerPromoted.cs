using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200065F RID: 1631
	public class ShipOfficerPromoted : GameEvent
	{
		// Token: 0x06002886 RID: 10374 RVA: 0x000DA5DF File Offset: 0x000D87DF
		public ShipOfficerPromoted(TIOfficerState officer)
		{
			this.officer = officer;
		}

		// Token: 0x04001EBD RID: 7869
		public TIOfficerState officer;
	}
}
