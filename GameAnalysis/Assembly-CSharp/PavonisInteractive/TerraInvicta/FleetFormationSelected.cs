using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006AC RID: 1708
	public class FleetFormationSelected : GameEvent
	{
		// Token: 0x060028D8 RID: 10456 RVA: 0x000DAB91 File Offset: 0x000D8D91
		public FleetFormationSelected(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001F15 RID: 7957
		public TISpaceFleetState fleet;
	}
}
