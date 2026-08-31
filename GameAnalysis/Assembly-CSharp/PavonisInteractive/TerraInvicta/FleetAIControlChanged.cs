using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006DD RID: 1757
	public class FleetAIControlChanged : GameEvent
	{
		// Token: 0x06002909 RID: 10505 RVA: 0x000DAF7F File Offset: 0x000D917F
		public FleetAIControlChanged(bool isAIControlEnabled, TISpaceFleetState fleet)
		{
			this.isAIControlEnabled = isAIControlEnabled;
			this.fleet = fleet;
		}

		// Token: 0x04001F6C RID: 8044
		public bool isAIControlEnabled;

		// Token: 0x04001F6D RID: 8045
		public TISpaceFleetState fleet;
	}
}
