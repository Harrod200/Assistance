using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000631 RID: 1585
	public class FleetAvailabilityChange : GameEvent
	{
		// Token: 0x06002856 RID: 10326 RVA: 0x000DA21E File Offset: 0x000D841E
		public FleetAvailabilityChange(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001E70 RID: 7792
		public TISpaceFleetState fleet;
	}
}
