using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005D5 RID: 1493
	public class FleetDetailRequested : GameEvent
	{
		// Token: 0x060027FA RID: 10234 RVA: 0x000D9BBA File Offset: 0x000D7DBA
		public FleetDetailRequested(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001DF5 RID: 7669
		public TISpaceFleetState fleet;
	}
}
