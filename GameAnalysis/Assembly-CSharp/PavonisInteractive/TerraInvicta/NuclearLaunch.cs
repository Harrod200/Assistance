using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200060B RID: 1547
	public class NuclearLaunch : GameEvent
	{
		// Token: 0x06002830 RID: 10288 RVA: 0x000D9F7B File Offset: 0x000D817B
		public NuclearLaunch(TIRegionState launchingRegion)
		{
			this.launchingRegion = launchingRegion;
		}

		// Token: 0x04001E3B RID: 7739
		public TIRegionState launchingRegion;
	}
}
