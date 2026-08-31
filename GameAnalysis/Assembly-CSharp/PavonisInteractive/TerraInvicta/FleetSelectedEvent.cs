using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200066B RID: 1643
	public class FleetSelectedEvent : GameEvent
	{
		// Token: 0x06002892 RID: 10386 RVA: 0x000DA693 File Offset: 0x000D8893
		public FleetSelectedEvent(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001EC9 RID: 7881
		public TISpaceFleetState fleet;
	}
}
