using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006A1 RID: 1697
	public class FleetTargetDestination : GameEvent
	{
		// Token: 0x060028C9 RID: 10441 RVA: 0x000DAAB1 File Offset: 0x000D8CB1
		public FleetTargetDestination(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001F09 RID: 7945
		public TISpaceFleetState fleet;
	}
}
