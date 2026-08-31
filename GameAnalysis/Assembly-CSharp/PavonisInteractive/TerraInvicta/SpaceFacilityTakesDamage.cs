using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000626 RID: 1574
	public class SpaceFacilityTakesDamage : GameEvent
	{
		// Token: 0x0600284B RID: 10315 RVA: 0x000DA148 File Offset: 0x000D8348
		public SpaceFacilityTakesDamage(TIRegionSpaceFacilityState state)
		{
			this.state = state;
		}

		// Token: 0x04001E5E RID: 7774
		public TIRegionSpaceFacilityState state;
	}
}
