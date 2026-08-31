using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200063F RID: 1599
	public class STOFireMission_Region : GameEvent
	{
		// Token: 0x06002864 RID: 10340 RVA: 0x000DA389 File Offset: 0x000D8589
		public STOFireMission_Region(TIRegionSpaceFacilityState shooter, TISpaceShipState target, TIDateTime time, int gameTimeSpeedIndex)
		{
			this.shooter = shooter;
			this.target = target;
			this.currentTime = time;
			this.gameTimeSpeedIndex = gameTimeSpeedIndex;
		}

		// Token: 0x04001E90 RID: 7824
		public TIRegionSpaceFacilityState shooter;

		// Token: 0x04001E91 RID: 7825
		public TISpaceShipState target;

		// Token: 0x04001E92 RID: 7826
		public TIDateTime currentTime;

		// Token: 0x04001E93 RID: 7827
		public int gameTimeSpeedIndex;
	}
}
