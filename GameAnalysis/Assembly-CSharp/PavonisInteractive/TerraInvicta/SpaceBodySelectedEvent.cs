using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200066C RID: 1644
	public class SpaceBodySelectedEvent : GameEvent
	{
		// Token: 0x06002893 RID: 10387 RVA: 0x000DA6A2 File Offset: 0x000D88A2
		public SpaceBodySelectedEvent(TISpaceBodyState spaceBody)
		{
			this.spaceBody = spaceBody;
		}

		// Token: 0x04001ECA RID: 7882
		public TISpaceBodyState spaceBody;
	}
}
