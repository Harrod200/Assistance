using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000649 RID: 1609
	public class ResetFleetFormationVisuals : GameEvent
	{
		// Token: 0x0600286E RID: 10350 RVA: 0x000DA45F File Offset: 0x000D865F
		public ResetFleetFormationVisuals(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001EA3 RID: 7843
		public TISpaceFleetState fleet;
	}
}
