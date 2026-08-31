using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200063A RID: 1594
	public class ShipsRemovedFromFleet : GameEvent
	{
		// Token: 0x0600285F RID: 10335 RVA: 0x000DA2CF File Offset: 0x000D84CF
		public ShipsRemovedFromFleet(TISpaceFleetState fleet, TISpaceFleetState gainingFleet)
		{
			this.fleet = fleet;
			this.gainingFleet = gainingFleet;
		}

		// Token: 0x04001E7F RID: 7807
		public TISpaceFleetState fleet;

		// Token: 0x04001E80 RID: 7808
		public TISpaceFleetState gainingFleet;
	}
}
