using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000677 RID: 1655
	public class NationIPManagerRequested : GameEvent
	{
		// Token: 0x0600289F RID: 10399 RVA: 0x000DA809 File Offset: 0x000D8A09
		public NationIPManagerRequested(TINationState nation, TIRegionState region)
		{
			this.nation = nation;
			this.region = region;
		}

		// Token: 0x04001ED8 RID: 7896
		public TINationState nation;

		// Token: 0x04001ED9 RID: 7897
		public TIRegionState region;
	}
}
