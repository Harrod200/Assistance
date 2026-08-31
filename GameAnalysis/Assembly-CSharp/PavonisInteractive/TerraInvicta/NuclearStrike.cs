using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200060C RID: 1548
	public class NuclearStrike : GameEvent
	{
		// Token: 0x06002831 RID: 10289 RVA: 0x000D9F8A File Offset: 0x000D818A
		public NuclearStrike(TINationState attackingNation, TIRegionState targetRegion)
		{
			this.attackingNation = attackingNation;
			this.targetRegion = targetRegion;
		}

		// Token: 0x04001E3C RID: 7740
		public TINationState attackingNation;

		// Token: 0x04001E3D RID: 7741
		public TIRegionState targetRegion;
	}
}
