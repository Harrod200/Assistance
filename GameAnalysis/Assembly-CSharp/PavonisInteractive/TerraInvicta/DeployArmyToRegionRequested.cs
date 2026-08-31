using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200068C RID: 1676
	public class DeployArmyToRegionRequested : GameEvent
	{
		// Token: 0x060028B4 RID: 10420 RVA: 0x000DA936 File Offset: 0x000D8B36
		public DeployArmyToRegionRequested(TIArmyState army, TIRegionState region, bool deployAll)
		{
			this.army = army;
			this.region = region;
			this.deployAll = deployAll;
		}

		// Token: 0x04001EEB RID: 7915
		public TIArmyState army;

		// Token: 0x04001EEC RID: 7916
		public TIRegionState region;

		// Token: 0x04001EED RID: 7917
		public bool deployAll;
	}
}
