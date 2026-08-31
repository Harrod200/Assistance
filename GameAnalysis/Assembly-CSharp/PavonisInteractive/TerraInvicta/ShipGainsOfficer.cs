using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200065D RID: 1629
	public class ShipGainsOfficer : GameEvent
	{
		// Token: 0x06002884 RID: 10372 RVA: 0x000DA5B3 File Offset: 0x000D87B3
		public ShipGainsOfficer(TIOfficerState officer)
		{
			this.officer = officer;
		}

		// Token: 0x04001EB9 RID: 7865
		public TIOfficerState officer;
	}
}
