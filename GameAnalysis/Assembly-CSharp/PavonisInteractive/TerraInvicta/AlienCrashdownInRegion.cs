using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000610 RID: 1552
	public class AlienCrashdownInRegion : GameEvent
	{
		// Token: 0x06002835 RID: 10293 RVA: 0x000D9FCD File Offset: 0x000D81CD
		public AlienCrashdownInRegion(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001E41 RID: 7745
		public TIRegionState region;
	}
}
