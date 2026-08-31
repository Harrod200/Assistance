using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005DF RID: 1503
	public class ForceTrajectorySelectionUI_NoCurrentTrajectory : GameEvent
	{
		// Token: 0x06002804 RID: 10244 RVA: 0x000D9C66 File Offset: 0x000D7E66
		public ForceTrajectorySelectionUI_NoCurrentTrajectory(TISpaceFleetState fleet, TISpaceAssetState target, Trajectory[] validTrajectories = null)
		{
			this.fleet = fleet;
			this.target = target;
			this.validTrajectories = validTrajectories;
		}

		// Token: 0x04001E02 RID: 7682
		public TISpaceFleetState fleet;

		// Token: 0x04001E03 RID: 7683
		public TISpaceAssetState target;

		// Token: 0x04001E04 RID: 7684
		public Trajectory[] validTrajectories;
	}
}
