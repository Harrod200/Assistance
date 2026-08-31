using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000640 RID: 1600
	public class BeginBombardment : GameEvent
	{
		// Token: 0x06002865 RID: 10341 RVA: 0x000DA3AE File Offset: 0x000D85AE
		public BeginBombardment(TISpaceFleetState fleet, TIGameState target, TISpaceBodyState spaceBody)
		{
			this.fleet = fleet;
			this.target = target;
			this.spaceBody = spaceBody;
		}

		// Token: 0x04001E94 RID: 7828
		public TISpaceFleetState fleet;

		// Token: 0x04001E95 RID: 7829
		public TIGameState target;

		// Token: 0x04001E96 RID: 7830
		public TISpaceBodyState spaceBody;
	}
}
