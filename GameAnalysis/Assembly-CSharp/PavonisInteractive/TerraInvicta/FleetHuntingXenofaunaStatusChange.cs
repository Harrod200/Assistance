using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000661 RID: 1633
	public class FleetHuntingXenofaunaStatusChange : GameEvent
	{
		// Token: 0x06002888 RID: 10376 RVA: 0x000DA5FD File Offset: 0x000D87FD
		public FleetHuntingXenofaunaStatusChange(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001EBF RID: 7871
		public TISpaceFleetState fleet;
	}
}
