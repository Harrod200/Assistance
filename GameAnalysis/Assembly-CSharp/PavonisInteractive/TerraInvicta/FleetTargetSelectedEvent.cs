using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200067F RID: 1663
	public class FleetTargetSelectedEvent : GameEvent
	{
		// Token: 0x060028A7 RID: 10407 RVA: 0x000DA89D File Offset: 0x000D8A9D
		public FleetTargetSelectedEvent(TISpaceFleetState targetedFleet)
		{
			this.targetedFleet = targetedFleet;
		}

		// Token: 0x04001EE4 RID: 7908
		public TISpaceFleetState targetedFleet;
	}
}
