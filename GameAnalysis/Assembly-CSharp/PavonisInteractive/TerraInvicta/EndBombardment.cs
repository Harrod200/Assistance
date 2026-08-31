using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000641 RID: 1601
	public class EndBombardment : GameEvent
	{
		// Token: 0x06002866 RID: 10342 RVA: 0x000DA3CB File Offset: 0x000D85CB
		public EndBombardment(TISpaceFleetState fleet, TIGameState target, TISpaceBodyState spaceBody)
		{
			this.fleet = fleet;
			this.target = target;
			this.spaceBody = spaceBody;
		}

		// Token: 0x04001E97 RID: 7831
		public TISpaceFleetState fleet;

		// Token: 0x04001E98 RID: 7832
		public TIGameState target;

		// Token: 0x04001E99 RID: 7833
		public TISpaceBodyState spaceBody;
	}
}
