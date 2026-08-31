using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000632 RID: 1586
	public class FleetOperationWithDurationComplete : GameEvent
	{
		// Token: 0x06002857 RID: 10327 RVA: 0x000DA22D File Offset: 0x000D842D
		public FleetOperationWithDurationComplete(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001E71 RID: 7793
		public TISpaceFleetState fleet;
	}
}
