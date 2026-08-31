using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000612 RID: 1554
	public class AlienLandingDamaged : GameEvent
	{
		// Token: 0x06002837 RID: 10295 RVA: 0x000D9FEB File Offset: 0x000D81EB
		public AlienLandingDamaged(TIRegionUFOLandingState landing)
		{
			this.landing = landing;
		}

		// Token: 0x04001E43 RID: 7747
		public TIRegionUFOLandingState landing;
	}
}
