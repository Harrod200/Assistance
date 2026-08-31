using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000634 RID: 1588
	public class FleetUndocks : GameEvent
	{
		// Token: 0x06002859 RID: 10329 RVA: 0x000DA259 File Offset: 0x000D8459
		public FleetUndocks(TISpaceFleetState fleet, TIGameState dockedLocation)
		{
			this.fleet = fleet;
			this.dockedLocation = dockedLocation;
		}

		// Token: 0x04001E75 RID: 7797
		public TISpaceFleetState fleet;

		// Token: 0x04001E76 RID: 7798
		public TIGameState dockedLocation;
	}
}
