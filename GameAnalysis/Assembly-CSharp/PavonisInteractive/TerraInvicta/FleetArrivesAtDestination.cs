using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000633 RID: 1587
	public class FleetArrivesAtDestination : GameEvent
	{
		// Token: 0x06002858 RID: 10328 RVA: 0x000DA23C File Offset: 0x000D843C
		public FleetArrivesAtDestination(TISpaceFleetState fleet, TIGameState destination, bool planetarySurfaceChange)
		{
			this.fleet = fleet;
			this.destination = destination;
			this.planetarySurfaceChange = planetarySurfaceChange;
		}

		// Token: 0x04001E72 RID: 7794
		public TISpaceFleetState fleet;

		// Token: 0x04001E73 RID: 7795
		public TIGameState destination;

		// Token: 0x04001E74 RID: 7796
		public bool planetarySurfaceChange;
	}
}
