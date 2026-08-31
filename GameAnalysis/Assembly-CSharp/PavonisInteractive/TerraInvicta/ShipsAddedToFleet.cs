using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000639 RID: 1593
	public class ShipsAddedToFleet : GameEvent
	{
		// Token: 0x0600285E RID: 10334 RVA: 0x000DA2C0 File Offset: 0x000D84C0
		public ShipsAddedToFleet(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001E7E RID: 7806
		public TISpaceFleetState fleet;
	}
}
